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
/// 图像裁剪器，专门处理图片裁剪功能
/// </summary>
public class ImageCropper : IImageCropper
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
        // 默认不进行裁剪，直接复制原图
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
    /// 裁剪单个图片文件
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="x">裁剪区域左上角X坐标（像素）</param>
    /// <param name="y">裁剪区域左上角Y坐标（像素）</param>
    /// <param name="width">裁剪区域宽度（像素）</param>
    /// <param name="height">裁剪区域高度（像素）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>裁剪是否成功</returns>
    public bool CropImage(string sourceFilePath, string targetFilePath, int x, int y, int width, int height, int quality = 90)
    {
        try
        {
            // 验证输入参数
            if (!File.Exists(sourceFilePath))
            {
                // Console.WriteLine($"源文件不存在: {sourceFilePath}");
                return false;
            }

            if (x < 0 || y < 0 || width <= 0 || height <= 0)
            {
                // Console.WriteLine("裁剪参数无效：坐标不能为负数，宽度和高度必须大于0");
                return false;
            }

            // 验证目标文件路径是否有效
            try
            {
                Path.GetFullPath(targetFilePath);
                var invalidChars = Path.GetInvalidPathChars();
                if (targetFilePath.IndexOfAny(invalidChars) >= 0)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            // 确保目标目录存在
            var targetDirectory = Path.GetDirectoryName(targetFilePath);
            if (!string.IsNullOrEmpty(targetDirectory) && !Directory.Exists(targetDirectory))
            {
                try
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                catch
                {
                    // 无法创建目录（可能是无效路径）
                    return false;
                }
            }

            using var image = Image.Load(sourceFilePath);
            
            // 验证裁剪区域是否在图像范围内
            if (x >= image.Width || y >= image.Height)
            {
                // Console.WriteLine($"裁剪起始坐标超出图像范围。图像尺寸: {image.Width}x{image.Height}, 起始坐标: ({x}, {y})");
                return false;
            }

            // 验证裁剪区域是否完全超出图像边界
            if (x + width > image.Width || y + height > image.Height)
            {
                // Console.WriteLine($"裁剪区域超出图像边界。图像尺寸: {image.Width}x{image.Height}, 裁剪区域: ({x}, {y}, {width}, {height})");
                return false;
            }

            // 执行裁剪操作
            image.Mutate(ctx => ctx.Crop(new Rectangle(x, y, width, height)));

            // 保存裁剪后的图像
            var result = SaveImage(image, targetFilePath, quality);
            
            if (result)
            {
                // Console.WriteLine($"图像裁剪成功: {sourceFilePath} -> {targetFilePath}, 裁剪区域: ({x}, {y}, {width}, {height})");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            // Console.WriteLine($"裁剪图像失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 按百分比裁剪单个图片文件
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="xPercent">裁剪区域左上角X坐标百分比（0-100）</param>
    /// <param name="yPercent">裁剪区域左上角Y坐标百分比（0-100）</param>
    /// <param name="widthPercent">裁剪区域宽度百分比（0-100）</param>
    /// <param name="heightPercent">裁剪区域高度百分比（0-100）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>裁剪是否成功</returns>
    public bool CropImageByPercent(string sourceFilePath, string targetFilePath, float xPercent, float yPercent, float widthPercent, float heightPercent, int quality = 90)
    {
        try
        {
            // 验证百分比参数
            if (xPercent < 0 || xPercent > 100 || yPercent < 0 || yPercent > 100 ||
                widthPercent <= 0 || widthPercent > 100 || heightPercent <= 0 || heightPercent > 100)
            {
                // Console.WriteLine("百分比参数无效：坐标百分比应在0-100范围内，宽度和高度百分比应在0-100范围内且大于0");
                return false;
            }

            if (xPercent + widthPercent > 100 || yPercent + heightPercent > 100)
            {
                // Console.WriteLine("裁剪区域超出图像边界：起始坐标百分比 + 尺寸百分比不能超过100%");
                return false;
            }

            using var image = Image.Load(sourceFilePath);
            
            // 将百分比转换为像素坐标
            var x = (int)(image.Width * xPercent / 100);
            var y = (int)(image.Height * yPercent / 100);
            var width = (int)(image.Width * widthPercent / 100);
            var height = (int)(image.Height * heightPercent / 100);

            // Console.WriteLine($"百分比裁剪转换为像素坐标: ({xPercent}%, {yPercent}%, {widthPercent}%, {heightPercent}%) -> ({x}, {y}, {width}, {height})");

            // 调用像素坐标裁剪方法
            return CropImage(sourceFilePath, targetFilePath, x, y, width, height, quality);
        }
        catch (Exception ex)
        {
            // Console.WriteLine($"按百分比裁剪图像失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 中心裁剪单个图片文件
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="width">裁剪区域宽度（像素）</param>
    /// <param name="height">裁剪区域高度（像素）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>裁剪是否成功</returns>
    public bool CropImageCenter(string sourceFilePath, string targetFilePath, int width, int height, int quality = 90)
    {
        try
        {
            if (width <= 0 || height <= 0)
            {
                // Console.WriteLine("裁剪尺寸无效：宽度和高度必须大于0");
                return false;
            }

            using var image = Image.Load(sourceFilePath);
            
            // 计算中心裁剪的起始坐标
            var x = Math.Max(0, (image.Width - width) / 2);
            var y = Math.Max(0, (image.Height - height) / 2);
            
            // 调整裁剪尺寸以适应图像大小
            var actualWidth = Math.Min(width, image.Width);
            var actualHeight = Math.Min(height, image.Height);

            // Console.WriteLine($"中心裁剪计算: 图像尺寸 {image.Width}x{image.Height}, 目标尺寸 {width}x{height}, 实际裁剪 ({x}, {y}, {actualWidth}, {actualHeight})");

            // 调用像素坐标裁剪方法
            return CropImage(sourceFilePath, targetFilePath, x, y, actualWidth, actualHeight, quality);
        }
        catch (Exception ex)
        {
            // Console.WriteLine($"中心裁剪图像失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 批量裁剪指定目录下的图片文件
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名</param>
    /// <param name="x">裁剪区域左上角X坐标（像素）</param>
    /// <param name="y">裁剪区域左上角Y坐标（像素）</param>
    /// <param name="width">裁剪区域宽度（像素）</param>
    /// <param name="height">裁剪区域高度（像素）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>裁剪结果信息</returns>
    public ConversionResult BatchCrop(string sourceDirectory, string outputDirectory, string sourceExtension, int x, int y, int width, int height, int quality = 90)
    {
        var result = new ConversionResult();
        
        try
        {
            // 验证目录
            if (!Directory.Exists(sourceDirectory))
            {
                // Console.WriteLine($"源目录不存在: {sourceDirectory}");
                result.ErrorMessage = "源目录不存在";
                return result;
            }

            // 创建输出目录
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // 获取指定扩展名的文件
            var searchPattern = $"*.{sourceExtension.TrimStart('.')}"; 
            var sourceFiles = Directory.GetFiles(sourceDirectory, searchPattern, SearchOption.TopDirectoryOnly);
            
            if (sourceFiles.Length == 0)
            {
                // Console.WriteLine($"在目录 {sourceDirectory} 中未找到 {sourceExtension} 格式的文件");
                result.ErrorMessage = "未找到匹配的文件";
                return result;
            }

            // Console.WriteLine($"开始批量裁剪，共找到 {sourceFiles.Length} 个文件");

            foreach (var sourceFile in sourceFiles)
            {
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(sourceFile);
                    var targetFile = Path.Combine(outputDirectory, $"{fileName}{Path.GetExtension(sourceFile)}");
                    
                    if (CropImage(sourceFile, targetFile, x, y, width, height, quality))
                    {
                        result.SuccessfulConversions++;
                        // Console.WriteLine($"裁剪成功: {Path.GetFileName(sourceFile)}");
                    }
                    else
                    {
                        result.FailedConversions++;
                        // Console.WriteLine($"裁剪失败: {Path.GetFileName(sourceFile)}");
                    }
                }
                catch (Exception ex)
                {
                    result.FailedConversions++;
                    // Console.WriteLine($"处理文件 {Path.GetFileName(sourceFile)} 时发生错误: {ex.Message}");
                }
            }

            result.TotalFiles = result.SuccessfulConversions + result.FailedConversions;
            // Console.WriteLine($"批量裁剪完成。总计: {result.TotalFiles}, 成功: {result.SuccessfulConversions}, 失败: {result.FailedConversions}");
        }
        catch (Exception ex)
        {
            // Console.WriteLine($"批量裁剪过程中发生错误: {ex.Message}");
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 批量中心裁剪指定目录下的图片文件
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名</param>
    /// <param name="width">裁剪区域宽度（像素）</param>
    /// <param name="height">裁剪区域高度（像素）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>裁剪结果信息</returns>
    public ConversionResult BatchCropCenter(string sourceDirectory, string outputDirectory, string sourceExtension, int width, int height, int quality = 90)
    {
        var result = new ConversionResult();
        
        try
        {
            // 验证目录
            if (!Directory.Exists(sourceDirectory))
            {
                // Console.WriteLine($"源目录不存在: {sourceDirectory}");
                result.ErrorMessage = "源目录不存在";
                return result;
            }

            // 创建输出目录
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // 获取指定扩展名的文件
            var searchPattern = $"*.{sourceExtension.TrimStart('.')}";
            var sourceFiles = Directory.GetFiles(sourceDirectory, searchPattern);

            if (sourceFiles.Length == 0)
            {
                // Console.WriteLine($"在目录 {sourceDirectory} 中未找到 {sourceExtension} 格式的文件");
                result.ErrorMessage = "未找到匹配的文件";
                return result;
            }

            // Console.WriteLine($"开始批量中心裁剪，共找到 {sourceFiles.Length} 个文件");

            foreach (var sourceFile in sourceFiles)
            {
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(sourceFile);
                    var extension = Path.GetExtension(sourceFile);
                    var targetFile = Path.Combine(outputDirectory, $"{fileName}{extension}");

                    if (CropImageCenter(sourceFile, targetFile, width, height, quality))
                    {
                        result.SuccessfulConversions++;
                        // Console.WriteLine($"中心裁剪成功: {Path.GetFileName(sourceFile)}");
                    }
                    else
                    {
                        result.FailedConversions++;
                        // Console.WriteLine($"中心裁剪失败: {Path.GetFileName(sourceFile)}");
                    }
                }
                catch (Exception ex)
                {
                    result.FailedConversions++;
                    // Console.WriteLine($"处理文件 {Path.GetFileName(sourceFile)} 时发生错误: {ex.Message}");
                }
            }

            result.TotalFiles = result.SuccessfulConversions + result.FailedConversions;
            // Console.WriteLine($"批量中心裁剪完成。总计: {result.TotalFiles}, 成功: {result.SuccessfulConversions}, 失败: {result.FailedConversions}");
        }
        catch (Exception ex)
        {
            // Console.WriteLine($"批量中心裁剪过程中发生错误: {ex.Message}");
            result.ErrorMessage = ex.Message;
        }

        return result;
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
                    var jpegEncoder = new JpegEncoder
                    {
                        Quality = Math.Clamp(quality, 1, 100)
                    };
                    image.SaveAsJpeg(targetFilePath, jpegEncoder);
                    break;
                    
                default:
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
}