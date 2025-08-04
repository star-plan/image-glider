using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;
using ImageGlider.Core;
using ImageInfo = ImageGlider.Core.ImageInfo;

namespace ImageGlider.Processors;

/// <summary>
/// 图像信息提取器，专门处理图像信息提取功能
/// </summary>
public class ImageInfoExtractor : IImageInfoExtractor
{
    /// <summary>
    /// 处理单个图像文件（实现基础接口方法）
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径（此处不使用）</param>
    /// <param name="quality">质量参数（此处不使用）</param>
    /// <returns>处理是否成功</returns>
    public bool ProcessImage(string sourceFilePath, string targetFilePath, int quality = 90)
    {
        try
        {
            var info = ExtractImageInfo(sourceFilePath);
            return info != null;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// 提取单个图片文件的信息
    /// </summary>
    /// <param name="filePath">图片文件路径</param>
    /// <returns>图片信息对象</returns>
    /// <exception cref="ArgumentException">文件不存在时抛出</exception>
    /// <exception cref="InvalidOperationException">无法读取图片时抛出</exception>
    public ImageInfo ExtractImageInfo(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new ArgumentException($"文件不存在: {filePath}");
        }

        var fileInfo = new FileInfo(filePath);
        var imageInfo = new ImageInfo
        {
            FilePath = filePath,
            FileSize = fileInfo.Length,
            CreationTime = fileInfo.CreationTime,
            ModificationTime = fileInfo.LastWriteTime
        };

        try
        {
            using var image = Image.Load(filePath);
            
            // 基本图像信息
            imageInfo.Width = image.Width;
            imageInfo.Height = image.Height;
            imageInfo.Format = image.Metadata.DecodedImageFormat?.Name ?? "Unknown";
            
            // DPI信息
            imageInfo.HorizontalDpi = image.Metadata.HorizontalResolution;
            imageInfo.VerticalDpi = image.Metadata.VerticalResolution;
            
            // 获取像素格式信息
            ExtractPixelFormatInfo(image, imageInfo);
            
            // 获取元数据信息
            ExtractMetadataInfo(image, imageInfo);
            
            // 获取压缩信息
            ExtractCompressionInfo(image, imageInfo);
            
            return imageInfo;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"无法读取图片文件 {filePath}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 批量提取指定目录下的图片信息
    /// </summary>
    /// <param name="directory">目录路径</param>
    /// <param name="searchPattern">搜索模式（如 "*.jpg"）</param>
    /// <param name="recursive">是否递归搜索子目录</param>
    /// <returns>图片信息列表</returns>
    public List<ImageInfo> BatchExtractImageInfo(string directory, string searchPattern = "*.*", bool recursive = false)
    {
        if (!Directory.Exists(directory))
        {
            throw new ArgumentException($"目录不存在: {directory}");
        }

        var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".tif", ".webp" };
        
        var files = Directory.GetFiles(directory, searchPattern, searchOption)
            .Where(file => supportedExtensions.Contains(Path.GetExtension(file).ToLowerInvariant()))
            .ToList();

        var results = new List<ImageInfo>();
        
        foreach (var file in files)
        {
            try
            {
                var info = ExtractImageInfo(file);
                results.Add(info);
            }
            catch (Exception ex)
            {
                // 记录错误但继续处理其他文件
                Console.WriteLine($"警告: 无法处理文件 {file}: {ex.Message}");
            }
        }

        return results;
    }

    /// <summary>
    /// 提取像素格式信息
    /// </summary>
    /// <param name="image">图像对象</param>
    /// <param name="imageInfo">图像信息对象</param>
    private static void ExtractPixelFormatInfo(Image image, ImageInfo imageInfo)
    {
        // 获取像素类型信息
        var pixelType = image.PixelType;
        imageInfo.BitDepth = pixelType.BitsPerPixel;
        
        // 检查是否有透明通道
        imageInfo.HasAlpha = pixelType.AlphaRepresentation != PixelAlphaRepresentation.None;
        
        // 简化颜色空间信息获取
        imageInfo.ColorSpace = GetColorSpaceFromFormat(image.Metadata.DecodedImageFormat?.Name, imageInfo.HasAlpha);
    }

    /// <summary>
    /// 提取元数据信息
    /// </summary>
    /// <param name="image">图像对象</param>
    /// <param name="imageInfo">图像信息对象</param>
    private static void ExtractMetadataInfo(Image image, ImageInfo imageInfo)
    {
        var metadata = image.Metadata;
        
        // 检查是否有EXIF数据
        var exifProfile = metadata.ExifProfile;
        if (exifProfile != null)
        {
            imageInfo.HasMetadata = true;
            imageInfo.MetadataSize += EstimateExifSize(exifProfile);
            
            // 提取一些常用的EXIF信息
            ExtractExifData(exifProfile, imageInfo);
        }
        
        // 检查ICC配置文件
        var iccProfile = metadata.IccProfile;
        if (iccProfile != null)
        {
            imageInfo.HasMetadata = true;
            imageInfo.MetadataSize += iccProfile.ToByteArray().Length;
        }
        
        // 检查XMP数据
        var xmpProfile = metadata.XmpProfile;
        if (xmpProfile != null)
        {
            imageInfo.HasMetadata = true;
            imageInfo.MetadataSize += xmpProfile.ToByteArray().Length;
        }
    }

    /// <summary>
    /// 提取压缩信息
    /// </summary>
    /// <param name="image">图像对象</param>
    /// <param name="imageInfo">图像信息对象</param>
    private static void ExtractCompressionInfo(Image image, ImageInfo imageInfo)
    {
        var format = image.Metadata.DecodedImageFormat?.Name?.ToLowerInvariant();
        
        imageInfo.Compression = format switch
        {
            "jpeg" => "JPEG (有损压缩)",
            "png" => "PNG (无损压缩)",
            "gif" => "GIF (LZW压缩)",
            "webp" => "WebP (可变压缩)",
            "bmp" => "无压缩",
            "tiff" or "tif" => "TIFF (可变压缩)",
            _ => "未知"
        };
    }

    /// <summary>
    /// 根据图像格式获取颜色空间描述
    /// </summary>
    /// <param name="formatName">格式名称</param>
    /// <param name="hasAlpha">是否有透明通道</param>
    /// <returns>颜色空间描述</returns>
    private static string GetColorSpaceFromFormat(string? formatName, bool hasAlpha)
    {
        var format = formatName?.ToLowerInvariant();
        return format switch
        {
            "jpeg" => "RGB",
            "png" => hasAlpha ? "RGBA" : "RGB",
            "gif" => hasAlpha ? "索引色+Alpha" : "索引色",
            "bmp" => hasAlpha ? "RGBA" : "RGB",
            "webp" => hasAlpha ? "RGBA" : "RGB",
            _ => hasAlpha ? "未知+Alpha" : "未知"
        };
    }

    /// <summary>
    /// 估算EXIF数据大小
    /// </summary>
    /// <param name="exifProfile">EXIF配置文件</param>
    /// <returns>估算的大小（字节）</returns>
    private static long EstimateExifSize(ExifProfile exifProfile)
    {
        try
        {
            return exifProfile.ToByteArray().Length;
        }
        catch
        {
            // 如果无法获取准确大小，返回估算值
            return exifProfile.Values?.Count * 20 ?? 100; // 每个EXIF标签大约20字节
        }
    }

    /// <summary>
    /// 提取EXIF数据
    /// </summary>
    /// <param name="exifProfile">EXIF配置文件</param>
    /// <param name="imageInfo">图像信息对象</param>
    private static void ExtractExifData(ExifProfile exifProfile, ImageInfo imageInfo)
    {
        try
        {
            // 提取相机制造商
            if (exifProfile.TryGetValue(ExifTag.Make, out var make))
            {
                imageInfo.AdditionalMetadata["相机制造商"] = make.Value?.ToString() ?? "";
            }
            
            // 提取相机型号
            if (exifProfile.TryGetValue(ExifTag.Model, out var model))
            {
                imageInfo.AdditionalMetadata["相机型号"] = model.Value?.ToString() ?? "";
            }
            
            // 提取拍摄时间
            if (exifProfile.TryGetValue(ExifTag.DateTime, out var dateTime))
            {
                if (DateTime.TryParse(dateTime.Value?.ToString(), out var shootingTime))
                {
                    imageInfo.AdditionalMetadata["拍摄时间"] = shootingTime;
                }
            }
            
            // 提取ISO感光度
            if (exifProfile.TryGetValue(ExifTag.ISOSpeedRatings, out var iso))
            {
                imageInfo.AdditionalMetadata["ISO"] = iso.Value?.ToString() ?? "";
            }
            
            // 提取光圈值
            if (exifProfile.TryGetValue(ExifTag.FNumber, out var fNumber))
            {
                imageInfo.AdditionalMetadata["光圈"] = fNumber.Value.ToString();
            }
            
            // 提取快门速度
            if (exifProfile.TryGetValue(ExifTag.ExposureTime, out var exposureTime))
            {
                imageInfo.AdditionalMetadata["快门速度"] = exposureTime.Value.ToString();
            }
            
            // 提取焦距
            if (exifProfile.TryGetValue(ExifTag.FocalLength, out var focalLength))
            {
                imageInfo.AdditionalMetadata["焦距"] = focalLength.Value.ToString();
            }
        }
        catch (Exception ex)
        {
            // 如果提取EXIF数据失败，记录错误但不影响主要功能
            imageInfo.AdditionalMetadata["EXIF提取错误"] = ex.Message;
        }
    }
}