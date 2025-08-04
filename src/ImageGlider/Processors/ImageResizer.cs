using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using ImageGlider.Core;
using ImageGlider.Enums;
using ImageGlider.Utilities;

namespace ImageGlider.Processors;

/// <summary>
/// 图像尺寸调整器，专门处理图片尺寸调整功能
/// </summary>
public class ImageResizer : IImageResizer
{
    /// <summary>
    /// 处理单个图像文件（实现接口方法）
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="quality">质量参数</param>
    /// <returns>处理是否成功</returns>
    public bool ProcessImage(string sourceFilePath, string targetFilePath, int quality = 90)
    {
        // 默认保持原始尺寸
        using var image = Image.Load(sourceFilePath);
        return ResizeImage(sourceFilePath, targetFilePath, image.Width, image.Height, ImageGlider.Enums.ResizeMode.KeepAspectRatio, quality);
    }

    /// <summary>
    /// 调整单个图片文件的尺寸
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="width">目标宽度（像素）</param>
    /// <param name="height">目标高度（像素）</param>
    /// <param name="resizeMode">调整模式</param>
    /// <param name="quality">JPEG 质量（仅对 JPEG 格式有效，范围 1-100）</param>
    /// <returns>调整是否成功</returns>
    public bool ResizeImage(string sourceFilePath, string targetFilePath, int? width = null, int? height = null, ImageGlider.Enums.ResizeMode resizeMode = ImageGlider.Enums.ResizeMode.KeepAspectRatio, int quality = 90)
    {
        try
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException($"源文件不存在: {sourceFilePath}");
            }

            if (width == null && height == null)
            {
                throw new ArgumentException("宽度和高度至少需要指定一个");
            }

            // 确保目标目录存在
            var targetDir = Path.GetDirectoryName(targetFilePath);
            if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            using var image = Image.Load(sourceFilePath);
            var originalWidth = image.Width;
            var originalHeight = image.Height;

            // 计算目标尺寸
            var targetSize = ImageSizeCalculator.CalculateTargetSize(originalWidth, originalHeight, width, height, resizeMode);

            // 执行尺寸调整
            image.Mutate(x => x.Resize(targetSize.Width, targetSize.Height));

            // 保存图片
            var targetExt = Path.GetExtension(targetFilePath).ToLowerInvariant();
            if (targetExt == ".jpeg" || targetExt == ".jpg")
            {
                var encoder = new JpegEncoder { Quality = Math.Clamp(quality, 1, 100) };
                image.Save(targetFilePath, encoder);
            }
            else
            {
                image.Save(targetFilePath);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 批量调整指定目录下图片文件的尺寸
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名（如 .jpg）</param>
    /// <param name="width">目标宽度（像素）</param>
    /// <param name="height">目标高度（像素）</param>
    /// <param name="resizeMode">调整模式</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>调整结果信息</returns>
    public ConversionResult BatchResize(
        string sourceDirectory,
        string outputDirectory,
        string sourceExtension,
        int? width = null,
        int? height = null,
        ImageGlider.Enums.ResizeMode resizeMode = ImageGlider.Enums.ResizeMode.KeepAspectRatio,
        int quality = 90)
    {
        var result = new ConversionResult();

        try
        {
            if (!Directory.Exists(sourceDirectory))
            {
                throw new DirectoryNotFoundException($"源目录不存在: {sourceDirectory}");
            }

            if (width == null && height == null)
            {
                throw new ArgumentException("宽度和高度至少需要指定一个");
            }

            // 确保扩展名以点开头
            if (!sourceExtension.StartsWith("."))
                sourceExtension = "." + sourceExtension;

            // 创建输出目录
            Directory.CreateDirectory(outputDirectory);

            // 查找所有匹配的文件
            var files = Directory.GetFiles(sourceDirectory, "*" + sourceExtension);
            result.TotalFiles = files.Length;

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var fileExt = Path.GetExtension(file);
                var targetFileName = fileName + "_resized" + fileExt;
                var targetFilePath = Path.Combine(outputDirectory, targetFileName);

                if (ResizeImage(file, targetFilePath, width, height, resizeMode, quality))
                {
                    result.SuccessfulConversions++;
                    result.SuccessfulFiles.Add(Path.GetFileName(file));
                }
                else
                {
                    result.FailedConversions++;
                    result.FailedFiles.Add(Path.GetFileName(file));
                }
            }
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 生成单个图片的缩略图
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="maxSize">缩略图最大边长（像素）</param>
    /// <param name="quality">JPEG 质量（仅对 JPEG 格式有效，范围 1-100）</param>
    /// <returns>生成是否成功</returns>
    public bool GenerateThumbnail(string sourceFilePath, string targetFilePath, int maxSize = 150, int quality = 90)
    {
        try
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException($"源文件不存在: {sourceFilePath}");
            }

            if (maxSize <= 0)
            {
                throw new ArgumentException("缩略图尺寸必须大于0");
            }

            // 确保目标目录存在
            var targetDir = Path.GetDirectoryName(targetFilePath);
            if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            using var image = Image.Load(sourceFilePath);
            var originalWidth = image.Width;
            var originalHeight = image.Height;

            // 计算缩略图尺寸（保持宽高比，按长边缩放）
            int targetWidth, targetHeight;
            if (originalWidth > originalHeight)
            {
                // 宽度为长边
                targetWidth = maxSize;
                targetHeight = (int)Math.Round((double)originalHeight * maxSize / originalWidth);
            }
            else
            {
                // 高度为长边
                targetHeight = maxSize;
                targetWidth = (int)Math.Round((double)originalWidth * maxSize / originalHeight);
            }

            // 执行尺寸调整
            image.Mutate(x => x.Resize(targetWidth, targetHeight));

            // 保存缩略图
            var targetExt = Path.GetExtension(targetFilePath).ToLowerInvariant();
            if (targetExt == ".jpeg" || targetExt == ".jpg")
            {
                var encoder = new JpegEncoder { Quality = Math.Clamp(quality, 1, 100) };
                image.Save(targetFilePath, encoder);
            }
            else
            {
                image.Save(targetFilePath);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 批量生成指定目录下图片文件的缩略图
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名（如 .jpg）</param>
    /// <param name="maxSize">缩略图最大边长（像素）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>生成结果信息</returns>
    public ConversionResult BatchGenerateThumbnails(
        string sourceDirectory,
        string outputDirectory,
        string sourceExtension,
        int maxSize = 150,
        int quality = 90)
    {
        var result = new ConversionResult();

        try
        {
            if (!Directory.Exists(sourceDirectory))
            {
                throw new DirectoryNotFoundException($"源目录不存在: {sourceDirectory}");
            }

            if (maxSize <= 0)
            {
                throw new ArgumentException("缩略图尺寸必须大于0");
            }

            // 确保扩展名以点开头
            if (!sourceExtension.StartsWith("."))
                sourceExtension = "." + sourceExtension;

            // 创建输出目录
            Directory.CreateDirectory(outputDirectory);

            // 查找所有匹配的文件
            var files = Directory.GetFiles(sourceDirectory, "*" + sourceExtension);
            result.TotalFiles = files.Length;

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var fileExt = Path.GetExtension(file);
                var targetFileName = fileName + "_thumb" + fileExt;
                var targetFilePath = Path.Combine(outputDirectory, targetFileName);

                if (GenerateThumbnail(file, targetFilePath, maxSize, quality))
                {
                    result.SuccessfulConversions++;
                    result.SuccessfulFiles.Add(Path.GetFileName(file));
                }
                else
                {
                    result.FailedConversions++;
                    result.FailedFiles.Add(Path.GetFileName(file));
                }
            }
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
        }

        return result;
    }
}