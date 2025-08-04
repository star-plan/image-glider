using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;
using ImageGlider.Core;
using ImageGlider.Enums;
using ImageGlider.Utilities;

namespace ImageGlider.Processors;

/// <summary>
/// 图像元数据清理器，专门处理图片元数据清理功能
/// </summary>
public class ImageMetadataStripper : IImageMetadataStripper
{
    /// <summary>
    /// 处理单个图像文件（实现基础接口方法）
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="quality">质量参数</param>
    /// <returns>处理是否成功</returns>
    public bool ProcessImage(string sourceFilePath, string targetFilePath, int quality = 90)
    {
        return StripMetadata(sourceFilePath, targetFilePath, stripAll: true, quality: quality);
    }

    /// <summary>
    /// 清理单个图片文件的元数据
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="stripAll">是否清理所有元数据（包括EXIF、ICC、XMP等）</param>
    /// <param name="stripExif">是否清理EXIF数据</param>
    /// <param name="stripIcc">是否清理ICC配置文件</param>
    /// <param name="stripXmp">是否清理XMP数据</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>清理是否成功</returns>
    public bool StripMetadata(string sourceFilePath, string targetFilePath, bool stripAll = true, bool stripExif = true, bool stripIcc = false, bool stripXmp = true, int quality = 90)
    {
        try
        {
            // 验证输入参数
            if (string.IsNullOrEmpty(sourceFilePath) || !File.Exists(sourceFilePath))
            {
                return false;
            }

            if (string.IsNullOrEmpty(targetFilePath))
            {
                return false;
            }

            // 确保目标目录存在
            var targetDirectory = Path.GetDirectoryName(targetFilePath);
            if (!string.IsNullOrEmpty(targetDirectory) && !Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            // 加载图像
            using var image = Image.Load(sourceFilePath);
            
            // 清理元数据
            if (stripAll)
            {
                // 清理所有元数据
                image.Metadata.ExifProfile = null;
                image.Metadata.IccProfile = null;
                image.Metadata.XmpProfile = null;
                image.Metadata.IptcProfile = null;
            }
            else
            {
                // 根据参数选择性清理
                if (stripExif)
                {
                    image.Metadata.ExifProfile = null;
                }
                
                if (stripIcc)
                {
                    image.Metadata.IccProfile = null;
                }
                
                if (stripXmp)
                {
                    image.Metadata.XmpProfile = null;
                }
            }

            // 保存图像
            return SaveImage(image, targetFilePath, quality);
        }
        catch (Exception ex)
        {
            // 可以在这里记录日志
            // Console.WriteLine($"清理元数据失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 批量清理指定目录下图片文件的元数据
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名</param>
    /// <param name="stripAll">是否清理所有元数据（包括EXIF、ICC、XMP等）</param>
    /// <param name="stripExif">是否清理EXIF数据</param>
    /// <param name="stripIcc">是否清理ICC配置文件</param>
    /// <param name="stripXmp">是否清理XMP数据</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>清理结果信息</returns>
    public BatchStripResult BatchStripMetadata(string sourceDirectory, string outputDirectory, string sourceExtension, bool stripAll = true, bool stripExif = true, bool stripIcc = false, bool stripXmp = true, int quality = 90)
    {
        return BatchProcessImages(sourceDirectory, outputDirectory, sourceExtension, 
            (source, target) => StripMetadata(source, target, stripAll, stripExif, stripIcc, stripXmp, quality));
    }

    /// <summary>
    /// 保存图像到指定路径
    /// </summary>
    /// <param name="image">图像对象</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="quality">质量参数</param>
    /// <returns>保存是否成功</returns>
    private bool SaveImage(Image image, string targetFilePath, int quality)
    {
        try
        {
            var extension = Path.GetExtension(targetFilePath).ToLowerInvariant();
            
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    var jpegEncoder = new JpegEncoder
                    {
                        Quality = Math.Max(1, Math.Min(100, quality))
                    };
                    image.SaveAsJpeg(targetFilePath, jpegEncoder);
                    break;
                    
                case ".png":
                    var pngEncoder = new PngEncoder();
                    image.SaveAsPng(targetFilePath, pngEncoder);
                    break;
                    
                case ".webp":
                    var webpEncoder = new WebpEncoder
                    {
                        Quality = Math.Max(1, Math.Min(100, quality))
                    };
                    image.SaveAsWebp(targetFilePath, webpEncoder);
                    break;
                    
                default:
                    // 对于其他格式，使用默认编码器
                    image.Save(targetFilePath);
                    break;
            }
            
            return true;
        }
        catch (Exception ex)
        {
            // Console.WriteLine($"保存图像失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 批量处理图像的通用方法
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名</param>
    /// <param name="processFunc">处理函数</param>
    /// <returns>处理结果信息</returns>
    private BatchStripResult BatchProcessImages(string sourceDirectory, string outputDirectory, string sourceExtension, Func<string, string, bool> processFunc)
    {
        var result = new BatchStripResult();

        try
        {
            // 验证输入参数
            if (string.IsNullOrEmpty(sourceDirectory) || !Directory.Exists(sourceDirectory))
            {
                result.ErrorMessage = "源目录不存在";
                return result;
            }

            if (string.IsNullOrEmpty(outputDirectory))
            {
                result.ErrorMessage = "输出目录不能为空";
                return result;
            }

            // 确保输出目录存在
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // 获取源文件列表
            var searchPattern = string.IsNullOrEmpty(sourceExtension) ? "*.*" : $"*.{sourceExtension.TrimStart('.')}";
            var sourceFiles = Directory.GetFiles(sourceDirectory, searchPattern, SearchOption.TopDirectoryOnly);

            if (sourceFiles.Length == 0)
            {
                result.ErrorMessage = "未找到匹配的源文件";
                return result;
            }

            // 处理每个文件
            foreach (var sourceFile in sourceFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(sourceFile);
                var extension = Path.GetExtension(sourceFile);
                var targetFile = Path.Combine(outputDirectory, $"{fileName}{extension}");

                if (processFunc(sourceFile, targetFile))
                {
                    result.SuccessCount++;
                    result.ProcessedFiles.Add(new ProcessedFileInfo
                    {
                        SourcePath = sourceFile,
                        TargetPath = targetFile,
                        Success = true
                    });
                }
                else
                {
                    result.FailureCount++;
                    result.ProcessedFiles.Add(new ProcessedFileInfo
                    {
                        SourcePath = sourceFile,
                        TargetPath = targetFile,
                        Success = false,
                        ErrorMessage = "元数据清理失败"
                    });
                }
            }

            result.TotalFiles = sourceFiles.Length;

            return result;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = $"批量处理失败: {ex.Message}";
            return result;
        }
    }
}