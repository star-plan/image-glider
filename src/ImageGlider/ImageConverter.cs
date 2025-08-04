using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace ImageGlider;

/// <summary>
/// 图片格式转换服务类，提供图片格式转换的核心功能
/// </summary>
public class ImageConverter
{
    /// <summary>
    /// 转换单个图片文件格式
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="quality">JPEG 质量（仅对 JPEG 格式有效，范围 1-100）</param>
    /// <returns>转换是否成功</returns>
    public static bool ConvertImage(string sourceFilePath, string targetFilePath, int quality = 90)
    {
        try
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException($"源文件不存在: {sourceFilePath}");
            }

            // 确保目标目录存在
            var targetDir = Path.GetDirectoryName(targetFilePath);
            if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            using var image = Image.Load(sourceFilePath);
            var targetExt = Path.GetExtension(targetFilePath).ToLowerInvariant();

            // 如果目标格式为 JPEG，使用指定质量
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
    /// 批量转换指定目录下的图片文件
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名（如 .jfif）</param>
    /// <param name="targetExtension">目标文件扩展名（如 .jpeg）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>转换结果信息</returns>
    public static ConversionResult BatchConvert(
        string sourceDirectory,
        string outputDirectory,
        string sourceExtension,
        string targetExtension,
        int quality = 90)
    {
        var result = new ConversionResult();

        try
        {
            if (!Directory.Exists(sourceDirectory))
            {
                throw new DirectoryNotFoundException($"源目录不存在: {sourceDirectory}");
            }

            // 确保扩展名以点开头
            if (!sourceExtension.StartsWith("."))
                sourceExtension = "." + sourceExtension;
            if (!targetExtension.StartsWith("."))
                targetExtension = "." + targetExtension;

            // 创建输出目录
            Directory.CreateDirectory(outputDirectory);

            // 查找所有匹配的文件
            var files = Directory.GetFiles(sourceDirectory, "*" + sourceExtension);
            result.TotalFiles = files.Length;

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var targetFileName = fileName + targetExtension;
                var targetFilePath = Path.Combine(outputDirectory, targetFileName);

                if (ConvertImage(file, targetFilePath, quality))
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
    /// 调整单个图片文件的尺寸
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="width">目标宽度（像素）</param>
    /// <param name="height">目标高度（像素）</param>
    /// <param name="resizeMode">调整模式</param>
    /// <param name="quality">JPEG 质量（仅对 JPEG 格式有效，范围 1-100）</param>
    /// <returns>调整是否成功</returns>
    public static bool ResizeImage(string sourceFilePath, string targetFilePath, int? width = null, int? height = null, ResizeMode resizeMode = ResizeMode.KeepAspectRatio, int quality = 90)
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
            var targetSize = CalculateTargetSize(originalWidth, originalHeight, width, height, resizeMode);

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
    public static ConversionResult BatchResize(
        string sourceDirectory,
        string outputDirectory,
        string sourceExtension,
        int? width = null,
        int? height = null,
        ResizeMode resizeMode = ResizeMode.KeepAspectRatio,
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
    /// 计算目标尺寸
    /// </summary>
    /// <param name="originalWidth">原始宽度</param>
    /// <param name="originalHeight">原始高度</param>
    /// <param name="targetWidth">目标宽度</param>
    /// <param name="targetHeight">目标高度</param>
    /// <param name="resizeMode">调整模式</param>
    /// <returns>计算后的目标尺寸</returns>
    private static (int Width, int Height) CalculateTargetSize(int originalWidth, int originalHeight, int? targetWidth, int? targetHeight, ResizeMode resizeMode)
    {
        switch (resizeMode)
        {
            case ResizeMode.Stretch:
                return (targetWidth ?? originalWidth, targetHeight ?? originalHeight);

            case ResizeMode.KeepAspectRatio:
                if (targetWidth.HasValue && targetHeight.HasValue)
                {
                    // 两个尺寸都指定时，选择较小的缩放比例以保持宽高比
                    var scaleX = (double)targetWidth.Value / originalWidth;
                    var scaleY = (double)targetHeight.Value / originalHeight;
                    var scale = Math.Min(scaleX, scaleY);
                    return ((int)(originalWidth * scale), (int)(originalHeight * scale));
                }
                else if (targetWidth.HasValue)
                {
                    // 只指定宽度，按比例计算高度
                    var scale = (double)targetWidth.Value / originalWidth;
                    return (targetWidth.Value, (int)(originalHeight * scale));
                }
                else if (targetHeight.HasValue)
                {
                    // 只指定高度，按比例计算宽度
                    var scale = (double)targetHeight.Value / originalHeight;
                    return ((int)(originalWidth * scale), targetHeight.Value);
                }
                break;

            case ResizeMode.Crop:
                if (targetWidth.HasValue && targetHeight.HasValue)
                {
                    // 裁剪模式：选择较大的缩放比例，然后裁剪
                    var scaleX = (double)targetWidth.Value / originalWidth;
                    var scaleY = (double)targetHeight.Value / originalHeight;
                    var scale = Math.Max(scaleX, scaleY);
                    return ((int)(originalWidth * scale), (int)(originalHeight * scale));
                }
                else
                {
                    // 裁剪模式需要同时指定宽度和高度
                    throw new ArgumentException("裁剪模式需要同时指定宽度和高度");
                }
        }

        return (originalWidth, originalHeight);
    }
}

/// <summary>
/// 图像尺寸调整模式
/// </summary>
public enum ResizeMode
{
    /// <summary>
    /// 拉伸模式：直接拉伸到目标尺寸，可能改变宽高比
    /// </summary>
    Stretch,

    /// <summary>
    /// 保持宽高比模式：按比例缩放，保持原始宽高比
    /// </summary>
    KeepAspectRatio,

    /// <summary>
    /// 裁剪模式：按比例缩放后裁剪到目标尺寸
    /// </summary>
    Crop
}

/// <summary>
/// 批量转换结果信息
/// </summary>
public class ConversionResult
{
    /// <summary>
    /// 总文件数
    /// </summary>
    public int TotalFiles { get; set; }

    /// <summary>
    /// 成功转换的文件数
    /// </summary>
    public int SuccessfulConversions { get; set; }

    /// <summary>
    /// 失败转换的文件数
    /// </summary>
    public int FailedConversions { get; set; }

    /// <summary>
    /// 成功转换的文件列表
    /// </summary>
    public List<string> SuccessfulFiles { get; set; } = new();

    /// <summary>
    /// 失败转换的文件列表
    /// </summary>
    public List<string> FailedFiles { get; set; } = new();

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 转换是否成功
    /// </summary>
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage) && FailedConversions == 0;
}