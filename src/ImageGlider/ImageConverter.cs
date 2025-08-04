using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using ImageGlider.Core;
using ImageGlider.Enums;

namespace ImageGlider;

/// <summary>
/// 图片格式转换服务类，提供图片格式转换的核心功能
/// 注意：此类已重构为使用组合模式，内部委托给专门的处理器类
/// </summary>
public class ImageConverter
{
    private static readonly IImageFormatConverter _formatConverter = ImageProcessorFactory.CreateFormatConverter();
    private static readonly IImageResizer _resizer = ImageProcessorFactory.CreateResizer();
    /// <summary>
    /// 转换单个图片文件格式
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="quality">JPEG 质量（仅对 JPEG 格式有效，范围 1-100）</param>
    /// <returns>转换是否成功</returns>
    public static bool ConvertImage(string sourceFilePath, string targetFilePath, int quality = 90)
    {
        return _formatConverter.ProcessImage(sourceFilePath, targetFilePath, quality);
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
        return _formatConverter.BatchConvert(sourceDirectory, outputDirectory, sourceExtension, targetExtension, quality);
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
    public static bool ResizeImage(string sourceFilePath, string targetFilePath, int? width = null, int? height = null, ImageGlider.Enums.ResizeMode resizeMode = ImageGlider.Enums.ResizeMode.KeepAspectRatio, int quality = 90)
    {
        return _resizer.ResizeImage(sourceFilePath, targetFilePath, width, height, resizeMode, quality);
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
        ImageGlider.Enums.ResizeMode resizeMode = ImageGlider.Enums.ResizeMode.KeepAspectRatio,
        int quality = 90)
    {
        return _resizer.BatchResize(sourceDirectory, outputDirectory, sourceExtension, width, height, resizeMode, quality);
    }

}