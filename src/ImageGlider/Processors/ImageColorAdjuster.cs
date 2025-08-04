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
/// 图像颜色调整器，专门处理图片颜色调整功能
/// </summary>
public class ImageColorAdjuster : IImageColorAdjuster
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
        // 默认不进行颜色调整，直接复制原图
        return AdjustColor(sourceFilePath, targetFilePath, quality: quality);
    }

    /// <summary>
    /// 调整单个图片文件的颜色
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="brightness">亮度调整（-100到100，0为不调整）</param>
    /// <param name="contrast">对比度调整（-100到100，0为不调整）</param>
    /// <param name="saturation">饱和度调整（-100到100，0为不调整）</param>
    /// <param name="hue">色相调整（-180到180，0为不调整）</param>
    /// <param name="gamma">伽马值调整（0.1到3.0，1.0为不调整）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>调整是否成功</returns>
    public bool AdjustColor(string sourceFilePath, string targetFilePath, float brightness = 0, float contrast = 0, float saturation = 0, float hue = 0, float gamma = 1.0f, int quality = 90)
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

            // 验证参数范围
            brightness = Math.Clamp(brightness, -100, 100);
            contrast = Math.Clamp(contrast, -100, 100);
            saturation = Math.Clamp(saturation, -100, 100);
            hue = Math.Clamp(hue, -180, 180);
            gamma = Math.Clamp(gamma, 0.1f, 3.0f);
            quality = Math.Clamp(quality, 1, 100);

            // 确保目标目录存在
            var targetDirectory = Path.GetDirectoryName(targetFilePath);
            if (!string.IsNullOrEmpty(targetDirectory) && !Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            // 加载图像并应用颜色调整
            using var image = Image.Load(sourceFilePath);
            
            // 应用颜色调整
            image.Mutate(x =>
            {
                // 亮度调整
                if (Math.Abs(brightness) > 0.01f)
                {
                    x.Brightness(brightness / 100f);
                }

                // 对比度调整
                if (Math.Abs(contrast) > 0.01f)
                {
                    x.Contrast(1.0f + (contrast / 100f));
                }

                // 饱和度调整
                if (Math.Abs(saturation) > 0.01f)
                {
                    x.Saturate(1.0f + (saturation / 100f));
                }

                // 色相调整
                if (Math.Abs(hue) > 0.01f)
                {
                    x.Hue(hue);
                }

                // 伽马值调整 - ImageSharp没有内置的GammaCorrection方法，需要手动实现
                if (Math.Abs(gamma - 1.0f) > 0.01f)
                {
                    // 通过像素级操作实现伽马校正
                    x.ProcessPixelRowsAsVector4((span) =>
                    {
                        for (int i = 0; i < span.Length; i++)
                        {
                            ref var pixel = ref span[i];
                            // 应用伽马校正公式: output = input^(1/gamma)
                            var invGamma = 1.0f / gamma;
                            pixel.X = (float)Math.Pow(pixel.X, invGamma); // Red
                            pixel.Y = (float)Math.Pow(pixel.Y, invGamma); // Green
                            pixel.Z = (float)Math.Pow(pixel.Z, invGamma); // Blue
                            // Alpha通道保持不变
                        }
                    });
                }
            });

            // 保存图像
            return SaveImage(image, targetFilePath, quality);
        }
        catch (Exception ex)
        {
            // 可以在这里记录日志
            // Console.WriteLine($"颜色调整失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 批量调整指定目录下图片文件的颜色
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名</param>
    /// <param name="brightness">亮度调整（-100到100，0为不调整）</param>
    /// <param name="contrast">对比度调整（-100到100，0为不调整）</param>
    /// <param name="saturation">饱和度调整（-100到100，0为不调整）</param>
    /// <param name="hue">色相调整（-180到180，0为不调整）</param>
    /// <param name="gamma">伽马值调整（0.1到3.0，1.0为不调整）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>调整结果信息</returns>
    public ConversionResult BatchAdjustColor(string sourceDirectory, string outputDirectory, string sourceExtension, float brightness = 0, float contrast = 0, float saturation = 0, float hue = 0, float gamma = 1.0f, int quality = 90)
    {
        var result = new ConversionResult();

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
                result.ErrorMessage = $"在目录 {sourceDirectory} 中未找到扩展名为 {sourceExtension} 的文件";
                return result;
            }

            result.TotalFiles = sourceFiles.Length;

            // 处理每个文件
            foreach (var sourceFile in sourceFiles)
            {
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(sourceFile);
                    var fileExtension = Path.GetExtension(sourceFile);
                    var targetFile = Path.Combine(outputDirectory, $"{fileName}_adjusted{fileExtension}");

                    if (AdjustColor(sourceFile, targetFile, brightness, contrast, saturation, hue, gamma, quality))
                    {
                        result.SuccessfulConversions++;
                    }
                    else
                    {
                        result.FailedConversions++;
                    }
                }
                catch (Exception ex)
                {
                    result.FailedConversions++;
                    // 可以在这里记录具体的错误信息
                    // Console.WriteLine($"处理文件 {sourceFile} 时出错: {ex.Message}");
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = $"批量颜色调整过程中发生错误: {ex.Message}";
            return result;
        }
    }

    /// <summary>
    /// 保存图像到指定路径
    /// </summary>
    /// <param name="image">要保存的图像</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>保存是否成功</returns>
    private static bool SaveImage(Image image, string targetFilePath, int quality)
    {
        try
        {
            var extension = Path.GetExtension(targetFilePath).ToLowerInvariant();
            
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    image.SaveAsJpeg(targetFilePath, new JpegEncoder { Quality = quality });
                    break;
                case ".png":
                    image.SaveAsPng(targetFilePath);
                    break;
                case ".webp":
                    image.SaveAsWebp(targetFilePath);
                    break;
                case ".bmp":
                    image.SaveAsBmp(targetFilePath);
                    break;
                case ".tiff":
                case ".tif":
                    image.SaveAsTiff(targetFilePath);
                    break;
                default:
                    // 默认保存为 JPEG
                    image.SaveAsJpeg(targetFilePath, new JpegEncoder { Quality = quality });
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
}