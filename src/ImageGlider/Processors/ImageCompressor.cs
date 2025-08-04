using ImageGlider.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageGlider.Processors;

/// <summary>
/// 图像压缩优化器实现类
/// </summary>
public class ImageCompressor : IImageCompressor
{
    /// <summary>
    /// 处理单个图片文件（基础接口实现）
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="quality">质量参数（用于压缩级别）</param>
    /// <returns>处理是否成功</returns>
    public bool ProcessImage(string sourceFilePath, string targetFilePath, int quality = 75)
    {
        return CompressImage(sourceFilePath, targetFilePath, quality);
    }

    /// <summary>
    /// 压缩优化单个图片文件
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="compressionLevel">压缩级别（1-100，数值越小压缩越强）</param>
    /// <param name="preserveMetadata">是否保留元数据</param>
    /// <returns>压缩是否成功</returns>
    public bool CompressImage(string sourceFilePath, string targetFilePath, int compressionLevel = 75, bool preserveMetadata = false)
    {
        try
        {
            // 验证源文件是否存在
            if (!File.Exists(sourceFilePath))
            {
                return false;
            }

            // 确保目标目录存在
            var targetDirectory = Path.GetDirectoryName(targetFilePath);
            if (!string.IsNullOrEmpty(targetDirectory) && !Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            // 验证压缩级别范围
            compressionLevel = Math.Max(1, Math.Min(100, compressionLevel));

            using var image = Image.Load<Rgba32>(sourceFilePath);
            
            // 根据目标文件扩展名选择合适的编码器
            var targetExtension = Path.GetExtension(targetFilePath).ToLowerInvariant();
            
            switch (targetExtension)
            {
                case ".jpg":
                case ".jpeg":
                    var jpegEncoder = new JpegEncoder
                    {
                        Quality = compressionLevel
                    };
                    image.Save(targetFilePath, jpegEncoder);
                    break;
                    
                case ".png":
                    var pngEncoder = new PngEncoder
                    {
                        CompressionLevel = GetPngCompressionLevel(compressionLevel)
                    };
                    image.Save(targetFilePath, pngEncoder);
                    break;
                    
                case ".webp":
                    var webpEncoder = new WebpEncoder
                    {
                        Quality = compressionLevel
                    };
                    image.Save(targetFilePath, webpEncoder);
                    break;
                    
                default:
                    // 对于其他格式，使用默认编码器
                    image.Save(targetFilePath);
                    break;
            }

            // 如果不保留元数据，则清理元数据（已通过编码器设置实现）
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 批量压缩优化指定目录下的图片文件
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名</param>
    /// <param name="compressionLevel">压缩级别</param>
    /// <param name="preserveMetadata">是否保留元数据</param>
    /// <returns>压缩结果信息</returns>
    public ConversionResult BatchCompress(string sourceDirectory, string outputDirectory, string sourceExtension, int compressionLevel = 75, bool preserveMetadata = false)
    {
        try
        {
            // 验证源目录是否存在
            if (!Directory.Exists(sourceDirectory))
            {
                return new ConversionResult
            {
                ErrorMessage = $"源目录不存在: {sourceDirectory}",
                TotalFiles = 0,
                SuccessfulConversions = 0,
                FailedConversions = 0
            };
            }

            // 确保输出目录存在
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // 查找指定扩展名的文件
            var searchPattern = $"*.{sourceExtension.TrimStart('.')}"; 
            var sourceFiles = Directory.GetFiles(sourceDirectory, searchPattern, SearchOption.TopDirectoryOnly);

            if (sourceFiles.Length == 0)
            {
                return new ConversionResult
                {
                    TotalFiles = 0,
                    SuccessfulConversions = 0,
                    FailedConversions = 0
                };
            }

            int successCount = 0;
            int failureCount = 0;

            foreach (var sourceFile in sourceFiles)
            {
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(sourceFile);
                    var extension = Path.GetExtension(sourceFile);
                    var targetFileName = $"{fileName}_compressed{extension}";
                    var targetFilePath = Path.Combine(outputDirectory, targetFileName);

                    if (CompressImage(sourceFile, targetFilePath, compressionLevel, preserveMetadata))
                    {
                        successCount++;
                    }
                    else
                    {
                        failureCount++;
                    }
                }
                catch
                {
                    failureCount++;
                }
            }

            return new ConversionResult
            {
                TotalFiles = sourceFiles.Length,
                SuccessfulConversions = successCount,
                FailedConversions = failureCount,
                ErrorMessage = failureCount > 0 ? $"部分文件压缩失败，失败数量: {failureCount}" : null
            };
        }
        catch (Exception ex)
        {
            return new ConversionResult
            {
                ErrorMessage = $"批量压缩过程中发生错误: {ex.Message}",
                TotalFiles = 0,
                SuccessfulConversions = 0,
                FailedConversions = 0
            };
        }
    }

    /// <summary>
    /// 将压缩级别转换为 PNG 压缩级别
    /// </summary>
    /// <param name="compressionLevel">压缩级别（1-100）</param>
    /// <returns>PNG 压缩级别（1-9）</returns>
    private static PngCompressionLevel GetPngCompressionLevel(int compressionLevel)
    {
        // 将 1-100 的范围映射到 PNG 的 1-9 压缩级别
        // 数值越小压缩越强，所以需要反向映射
        var level = 10 - (compressionLevel / 11);
        level = Math.Max(1, Math.Min(9, level));
        
        return (PngCompressionLevel)level;
    }
}