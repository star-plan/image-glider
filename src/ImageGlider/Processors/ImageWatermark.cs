using System;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using ImageGlider.Core;
using ImageGlider.Enums;
using ImageGlider.Utilities;
using IOPath = System.IO.Path;

namespace ImageGlider.Processors;

/// <summary>
/// 图像水印处理器，专门处理图片水印功能
/// </summary>
public class ImageWatermark : IImageWatermark
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
        // 默认不添加水印，直接复制原图
        try
        {
            using var image = Image.Load(sourceFilePath);
            return SaveImage(image, targetFilePath, quality);
        }
        catch (Exception ex)
        {
            // Console.WriteLine($"处理图像文件失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 添加文本水印到单个图片文件
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="text">水印文本</param>
    /// <param name="position">水印位置</param>
    /// <param name="opacity">透明度（0-100）</param>
    /// <param name="fontSize">字体大小</param>
    /// <param name="fontColor">字体颜色（十六进制，如 #FFFFFF）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>添加水印是否成功</returns>
    public bool AddTextWatermark(string sourceFilePath, string targetFilePath, string text, WatermarkPosition position = WatermarkPosition.BottomRight, int opacity = 50, int fontSize = 24, string fontColor = "#FFFFFF", int quality = 90)
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

            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            // 确保目标目录存在
            var targetDir = IOPath.GetDirectoryName(targetFilePath);
            if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            // 加载图像
            using var image = Image.Load(sourceFilePath);

            // 解析颜色
            var color = ParseColor(fontColor);
            var rgba = color.ToPixel<Rgba32>();
            var watermarkColor = Color.FromRgba(rgba.R, rgba.G, rgba.B, (byte)(255 * opacity / 100));

            // 创建字体 - 使用跨平台兼容的字体回退机制
            Font font;
            try
            {
                // 尝试使用 Arial 字体
                font = SystemFonts.CreateFont("Arial", fontSize);
            }
            catch
            {
                try
                {
                    // 如果 Arial 不可用，尝试使用 DejaVu Sans（Linux 常见字体）
                    font = SystemFonts.CreateFont("DejaVu Sans", fontSize);
                }
                catch
                {
                    try
                    {
                        // 如果 DejaVu Sans 不可用，尝试使用 Liberation Sans
                        font = SystemFonts.CreateFont("Liberation Sans", fontSize);
                    }
                    catch
                    {
                        // 最后回退到系统默认字体
                        var availableFonts = SystemFonts.Families.ToArray();
                        var defaultFontFamily = availableFonts.Length > 0 ? availableFonts[0].Name : "sans-serif";
                        font = SystemFonts.CreateFont(defaultFontFamily, fontSize);
                    }
                }
            }

            // 计算文本位置
            var textOptions = new TextOptions(font);
            var textSize = TextMeasurer.MeasureSize(text, textOptions);
            var textPosition = CalculateTextPosition(image.Width, image.Height, textSize, position);

            // 添加文本水印
            image.Mutate(x => x.DrawText(text, font, watermarkColor, textPosition));

            // 保存图像
            return SaveImage(image, targetFilePath, quality);
        }
        catch (Exception ex)
        {
            // Console.WriteLine($"添加文本水印失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 添加图片水印到单个图片文件
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="watermarkImagePath">水印图片路径</param>
    /// <param name="position">水印位置</param>
    /// <param name="opacity">透明度（0-100）</param>
    /// <param name="scale">缩放比例（0.1-2.0）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>添加水印是否成功</returns>
    public bool AddImageWatermark(string sourceFilePath, string targetFilePath, string watermarkImagePath, WatermarkPosition position = WatermarkPosition.BottomRight, int opacity = 50, float scale = 1.0f, int quality = 90)
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

            if (string.IsNullOrEmpty(watermarkImagePath) || !File.Exists(watermarkImagePath))
            {
                return false;
            }

            if (scale < 0.1f || scale > 2.0f)
            {
                return false;
            }

            // 确保目标目录存在
            var targetDir = IOPath.GetDirectoryName(targetFilePath);
            if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            // 加载主图像和水印图像
            using var image = Image.Load(sourceFilePath);
            using var watermarkImage = Image.Load(watermarkImagePath);

            // 调整水印图像大小
            var newWidth = (int)(watermarkImage.Width * scale);
            var newHeight = (int)(watermarkImage.Height * scale);
            watermarkImage.Mutate(x => x.Resize(newWidth, newHeight));

            // 设置透明度
            watermarkImage.Mutate(x => x.Opacity(opacity / 100.0f));

            // 计算水印位置
            var watermarkPosition = CalculateImagePosition(image.Width, image.Height, watermarkImage.Width, watermarkImage.Height, position);

            // 添加图片水印
            image.Mutate(x => x.DrawImage(watermarkImage, watermarkPosition, 1.0f));

            // 保存图像
            return SaveImage(image, targetFilePath, quality);
        }
        catch (Exception ex)
        {
            // Console.WriteLine($"添加图片水印失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 批量添加文本水印到指定目录下的图片文件
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名</param>
    /// <param name="text">水印文本</param>
    /// <param name="position">水印位置</param>
    /// <param name="opacity">透明度（0-100）</param>
    /// <param name="fontSize">字体大小</param>
    /// <param name="fontColor">字体颜色（十六进制，如 #FFFFFF）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>添加水印结果信息</returns>
    public ConversionResult BatchAddTextWatermark(string sourceDirectory, string outputDirectory, string sourceExtension, string text, WatermarkPosition position = WatermarkPosition.BottomRight, int opacity = 50, int fontSize = 24, string fontColor = "#FFFFFF", int quality = 90)
    {
        return BatchProcessImages(sourceDirectory, outputDirectory, sourceExtension, 
            (src, dest) => AddTextWatermark(src, dest, text, position, opacity, fontSize, fontColor, quality));
    }

    /// <summary>
    /// 批量添加图片水印到指定目录下的图片文件
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名</param>
    /// <param name="watermarkImagePath">水印图片路径</param>
    /// <param name="position">水印位置</param>
    /// <param name="opacity">透明度（0-100）</param>
    /// <param name="scale">缩放比例（0.1-2.0）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>添加水印结果信息</returns>
    public ConversionResult BatchAddImageWatermark(string sourceDirectory, string outputDirectory, string sourceExtension, string watermarkImagePath, WatermarkPosition position = WatermarkPosition.BottomRight, int opacity = 50, float scale = 1.0f, int quality = 90)
    {
        return BatchProcessImages(sourceDirectory, outputDirectory, sourceExtension, 
            (src, dest) => AddImageWatermark(src, dest, watermarkImagePath, position, opacity, scale, quality));
    }

    /// <summary>
    /// 批量处理图像的通用方法
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名</param>
    /// <param name="processFunc">处理函数</param>
    /// <returns>处理结果信息</returns>
    private ConversionResult BatchProcessImages(string sourceDirectory, string outputDirectory, string sourceExtension, Func<string, string, bool> processFunc)
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
                result.ErrorMessage = "未找到匹配的源文件";
                return result;
            }

            result.TotalFiles = sourceFiles.Length;

            // 处理每个文件
            foreach (var sourceFile in sourceFiles)
            {
                try
                {
                    var fileName = IOPath.GetFileNameWithoutExtension(sourceFile);
                    var extension = IOPath.GetExtension(sourceFile);
                    var targetFile = IOPath.Combine(outputDirectory, $"{fileName}_watermarked{extension}");

                    if (processFunc(sourceFile, targetFile))
                    {
                        result.SuccessfulConversions++;
                        result.SuccessfulFiles.Add(IOPath.GetFileName(sourceFile));
                    }
                    else
                    {
                        result.FailedConversions++;
                        result.FailedFiles.Add(IOPath.GetFileName(sourceFile));
                    }
                }
                catch (Exception ex)
                {
                    result.FailedConversions++;
                    result.FailedFiles.Add(IOPath.GetFileName(sourceFile));
                    // Console.WriteLine($"处理文件 {sourceFile} 失败: {ex.Message}");
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
    /// 保存图像到文件
    /// </summary>
    /// <param name="image">图像对象</param>
    /// <param name="filePath">文件路径</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>保存是否成功</returns>
    private bool SaveImage(Image image, string filePath, int quality)
    {
        try
        {
            var extension = IOPath.GetExtension(filePath).ToLowerInvariant();
            
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    var jpegEncoder = new JpegEncoder { Quality = quality };
                    image.SaveAsJpeg(filePath, jpegEncoder);
                    break;
                default:
                    image.Save(filePath);
                    break;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 解析颜色字符串
    /// </summary>
    /// <param name="colorString">颜色字符串（十六进制格式，如 #FFFFFF）</param>
    /// <returns>颜色对象</returns>
    private Color ParseColor(string colorString)
    {
        try
        {
            if (string.IsNullOrEmpty(colorString))
            {
                return Color.White;
            }

            // 移除 # 前缀
            var hex = colorString.TrimStart('#');
            
            if (hex.Length == 6)
            {
                var r = Convert.ToByte(hex.Substring(0, 2), 16);
                var g = Convert.ToByte(hex.Substring(2, 2), 16);
                var b = Convert.ToByte(hex.Substring(4, 2), 16);
                return Color.FromRgb(r, g, b);
            }
            else if (hex.Length == 8)
            {
                var a = Convert.ToByte(hex.Substring(0, 2), 16);
                var r = Convert.ToByte(hex.Substring(2, 2), 16);
                var g = Convert.ToByte(hex.Substring(4, 2), 16);
                var b = Convert.ToByte(hex.Substring(6, 2), 16);
                return Color.FromRgba(r, g, b, a);
            }

            return Color.White;
        }
        catch
        {
            return Color.White;
        }
    }

    /// <summary>
    /// 计算文本水印位置
    /// </summary>
    /// <param name="imageWidth">图像宽度</param>
    /// <param name="imageHeight">图像高度</param>
    /// <param name="textSize">文本尺寸</param>
    /// <param name="position">水印位置</param>
    /// <returns>文本位置坐标</returns>
    private PointF CalculateTextPosition(int imageWidth, int imageHeight, FontRectangle textSize, WatermarkPosition position)
    {
        const int margin = 20; // 边距
        
        return position switch
        {
            WatermarkPosition.TopLeft => new PointF(margin, margin),
            WatermarkPosition.TopCenter => new PointF((imageWidth - textSize.Width) / 2, margin),
            WatermarkPosition.TopRight => new PointF(imageWidth - textSize.Width - margin, margin),
            WatermarkPosition.MiddleLeft => new PointF(margin, (imageHeight - textSize.Height) / 2),
            WatermarkPosition.Center => new PointF((imageWidth - textSize.Width) / 2, (imageHeight - textSize.Height) / 2),
            WatermarkPosition.MiddleRight => new PointF(imageWidth - textSize.Width - margin, (imageHeight - textSize.Height) / 2),
            WatermarkPosition.BottomLeft => new PointF(margin, imageHeight - textSize.Height - margin),
            WatermarkPosition.BottomCenter => new PointF((imageWidth - textSize.Width) / 2, imageHeight - textSize.Height - margin),
            WatermarkPosition.BottomRight => new PointF(imageWidth - textSize.Width - margin, imageHeight - textSize.Height - margin),
            _ => new PointF(imageWidth - textSize.Width - margin, imageHeight - textSize.Height - margin)
        };
    }

    /// <summary>
    /// 计算图片水印位置
    /// </summary>
    /// <param name="imageWidth">主图像宽度</param>
    /// <param name="imageHeight">主图像高度</param>
    /// <param name="watermarkWidth">水印图像宽度</param>
    /// <param name="watermarkHeight">水印图像高度</param>
    /// <param name="position">水印位置</param>
    /// <returns>水印位置坐标</returns>
    private Point CalculateImagePosition(int imageWidth, int imageHeight, int watermarkWidth, int watermarkHeight, WatermarkPosition position)
    {
        const int margin = 20; // 边距
        
        return position switch
        {
            WatermarkPosition.TopLeft => new Point(margin, margin),
            WatermarkPosition.TopCenter => new Point((imageWidth - watermarkWidth) / 2, margin),
            WatermarkPosition.TopRight => new Point(imageWidth - watermarkWidth - margin, margin),
            WatermarkPosition.MiddleLeft => new Point(margin, (imageHeight - watermarkHeight) / 2),
            WatermarkPosition.Center => new Point((imageWidth - watermarkWidth) / 2, (imageHeight - watermarkHeight) / 2),
            WatermarkPosition.MiddleRight => new Point(imageWidth - watermarkWidth - margin, (imageHeight - watermarkHeight) / 2),
            WatermarkPosition.BottomLeft => new Point(margin, imageHeight - watermarkHeight - margin),
            WatermarkPosition.BottomCenter => new Point((imageWidth - watermarkWidth) / 2, imageHeight - watermarkHeight - margin),
            WatermarkPosition.BottomRight => new Point(imageWidth - watermarkWidth - margin, imageHeight - watermarkHeight - margin),
            _ => new Point(imageWidth - watermarkWidth - margin, imageHeight - watermarkHeight - margin)
        };
    }
}