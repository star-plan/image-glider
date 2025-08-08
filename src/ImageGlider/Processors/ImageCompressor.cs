using ImageGlider.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

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

            // 获取源文件大小
            var sourceFileInfo = new FileInfo(sourceFilePath);
            var sourceFileSize = sourceFileInfo.Length;

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
                    // 智能选择PNG编码参数以获得更好的压缩效果
                    var pngEncoder = CreateOptimizedPngEncoder(image, compressionLevel, preserveMetadata);
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

            // 检查压缩后的文件大小
            var targetFileInfo = new FileInfo(targetFilePath);
            var targetFileSize = targetFileInfo.Length;

            // 如果压缩后文件更大，则用原文件替换
            if (targetFileSize > sourceFileSize)
            {
                File.Copy(sourceFilePath, targetFilePath, true);
            }

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
                    var fileName = Path.GetFileName(sourceFile);
                    var targetFilePath = Path.Combine(outputDirectory, fileName);

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
    /// 创建优化的PNG编码器
    /// </summary>
    /// <param name="image">图像对象</param>
    /// <param name="compressionLevel">压缩级别（1-100）</param>
    /// <param name="preserveMetadata">是否保留元数据</param>
    /// <returns>优化的PNG编码器</returns>
    private static PngEncoder CreateOptimizedPngEncoder(Image<Rgba32> image, int compressionLevel, bool preserveMetadata)
    {
        var pngCompressionLevel = GetPngCompressionLevel(compressionLevel);
        
        // 检测图像特征以选择最佳编码参数
        var hasTransparency = ImageHasTransparency(image);
        var colorCount = EstimateColorCount(image);
        
        // 对于所有情况都使用量化器来减小文件大小
        // Wu量化器在保持质量的同时能显著减小文件大小
        // 根据压缩级别调整颜色数量：压缩级别越高，颜色数量越少
        var maxColors = compressionLevel <= 30 ? 256 : 
                       compressionLevel <= 60 ? 128 : 
                       compressionLevel <= 80 ? 64 : 32;
        
        var quantizer = new WuQuantizer(new QuantizerOptions
        {
            MaxColors = Math.Min(maxColors, colorCount <= 256 ? colorCount : maxColors)
        });
        
        // 根据图像特征选择最佳的颜色类型和位深度
        if (colorCount <= 256 && !hasTransparency)
        {
            // 颜色较少且无透明度，使用调色板模式
            return new PngEncoder
            {
                CompressionLevel = pngCompressionLevel,
                ChunkFilter = preserveMetadata ? null : PngChunkFilter.ExcludeAll,
                ColorType = PngColorType.Palette,
                BitDepth = colorCount <= 16 ? PngBitDepth.Bit4 : PngBitDepth.Bit8,
                FilterMethod = PngFilterMethod.None, // 调色板图像使用None过滤器更好
                Quantizer = quantizer
            };
        }
        else if (colorCount <= 256 && hasTransparency)
        {
            // 颜色较少但有透明度，使用带透明度的调色板
            return new PngEncoder
            {
                CompressionLevel = pngCompressionLevel,
                ChunkFilter = preserveMetadata ? null : PngChunkFilter.ExcludeAll,
                ColorType = PngColorType.Palette,
                BitDepth = PngBitDepth.Bit8,
                FilterMethod = PngFilterMethod.None,
                Quantizer = quantizer
            };
        }
        else
        {
            // 颜色丰富的图像，强制使用调色板模式和量化器来减小文件大小
            // 这是解决ImageSharp PNG文件增大问题的关键
            return new PngEncoder
            {
                CompressionLevel = pngCompressionLevel,
                ChunkFilter = preserveMetadata ? null : PngChunkFilter.ExcludeAll,
                ColorType = PngColorType.Palette,
                BitDepth = PngBitDepth.Bit8,
                FilterMethod = PngFilterMethod.None,
                Quantizer = quantizer
            };
        }
    }
    
    /// <summary>
    /// 检测图像是否包含透明度
    /// </summary>
    /// <param name="image">图像对象</param>
    /// <returns>是否包含透明度</returns>
    private static bool ImageHasTransparency(Image<Rgba32> image)
    {
        // 简单采样检测透明度（检查部分像素以提高性能）
        var sampleStep = Math.Max(1, Math.Max(image.Width, image.Height) / 100);
        
        for (int y = 0; y < image.Height; y += sampleStep)
        {
            for (int x = 0; x < image.Width; x += sampleStep)
            {
                if (image[x, y].A < 255)
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 估算图像的颜色数量
    /// </summary>
    /// <param name="image">图像对象</param>
    /// <returns>估算的颜色数量</returns>
    private static int EstimateColorCount(Image<Rgba32> image)
    {
        // 简单采样估算颜色数量（为了性能，不进行完整扫描）
        var colorSet = new HashSet<uint>();
        var sampleStep = Math.Max(1, Math.Max(image.Width, image.Height) / 50);
        
        for (int y = 0; y < image.Height; y += sampleStep)
        {
            for (int x = 0; x < image.Width; x += sampleStep)
            {
                var pixel = image[x, y];
                var colorValue = (uint)(pixel.R << 24 | pixel.G << 16 | pixel.B << 8 | pixel.A);
                colorSet.Add(colorValue);
                
                // 如果颜色数量已经超过256，就不需要继续检测了
                if (colorSet.Count > 256)
                {
                    return 1000; // 返回一个大于256的值表示颜色丰富
                }
            }
        }
        
        return colorSet.Count;
    }

    /// <summary>
    /// 将压缩级别转换为 PNG 压缩级别
    /// </summary>
    /// <param name="compressionLevel">压缩级别（1-100）</param>
    /// <returns>PNG 压缩级别（1-9）</returns>
    private static PngCompressionLevel GetPngCompressionLevel(int compressionLevel)
    {
        // 将 1-100 的范围映射到 PNG 的 1-9 压缩级别
        // 我们的设计：数值越小压缩越强
        // PNG的CompressionLevel：数值越大压缩越强
        // 所以需要反向映射：1->9, 100->1
        var level = 9 - ((compressionLevel - 1) * 8 / 99);
        level = Math.Max(1, Math.Min(9, level));
        
        return (PngCompressionLevel)level;
    }
}