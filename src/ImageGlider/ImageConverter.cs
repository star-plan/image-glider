using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using ImageGlider.Core;
using ImageGlider.Enums;
using static ImageGlider.Enums.WatermarkPosition;

namespace ImageGlider;

/// <summary>
/// 图片格式转换服务类，提供图片格式转换的核心功能
/// 注意：此类已重构为使用组合模式，内部委托给专门的处理器类
/// </summary>
public class ImageConverter
{
    private static readonly IImageFormatConverter _formatConverter = ImageProcessorFactory.CreateFormatConverter();
    private static readonly IImageResizer _resizer = ImageProcessorFactory.CreateResizer();
    private static readonly IImageCompressor _compressor = ImageProcessorFactory.CreateCompressor();
    private static readonly IImageCropper _cropper = ImageProcessorFactory.CreateCropper();
    private static readonly IImageWatermark _watermark = ImageProcessorFactory.CreateWatermark();
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

    /// <summary>
    /// 压缩优化单个图片文件
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="compressionLevel">压缩级别（1-100，数值越小压缩越强）</param>
    /// <param name="preserveMetadata">是否保留元数据</param>
    /// <returns>压缩是否成功</returns>
    public static bool CompressImage(string sourceFilePath, string targetFilePath, int compressionLevel = 75, bool preserveMetadata = false)
    {
        return _compressor.CompressImage(sourceFilePath, targetFilePath, compressionLevel, preserveMetadata);
    }

    /// <summary>
    /// 批量压缩优化指定目录下的图片文件
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名（如 .jpg）</param>
    /// <param name="compressionLevel">压缩级别（1-100，数值越小压缩越强）</param>
    /// <param name="preserveMetadata">是否保留元数据</param>
    /// <returns>压缩结果信息</returns>
    public static ConversionResult BatchCompress(
        string sourceDirectory,
        string outputDirectory,
        string sourceExtension,
        int compressionLevel = 75,
        bool preserveMetadata = false)
    {
        return _compressor.BatchCompress(sourceDirectory, outputDirectory, sourceExtension, compressionLevel, preserveMetadata);
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
    /// <param name="quality">JPEG 质量（仅对 JPEG 格式有效，范围 1-100）</param>
    /// <returns>裁剪是否成功</returns>
    public static bool CropImage(string sourceFilePath, string targetFilePath, int x, int y, int width, int height, int quality = 90)
    {
        return _cropper.CropImage(sourceFilePath, targetFilePath, x, y, width, height, quality);
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
    /// <param name="quality">JPEG 质量（仅对 JPEG 格式有效，范围 1-100）</param>
    /// <returns>裁剪是否成功</returns>
    public static bool CropImageByPercent(string sourceFilePath, string targetFilePath, float xPercent, float yPercent, float widthPercent, float heightPercent, int quality = 90)
    {
        return _cropper.CropImageByPercent(sourceFilePath, targetFilePath, xPercent, yPercent, widthPercent, heightPercent, quality);
    }

    /// <summary>
    /// 中心裁剪单个图片文件
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="width">裁剪区域宽度（像素）</param>
    /// <param name="height">裁剪区域高度（像素）</param>
    /// <param name="quality">JPEG 质量（仅对 JPEG 格式有效，范围 1-100）</param>
    /// <returns>裁剪是否成功</returns>
    public static bool CropImageCenter(string sourceFilePath, string targetFilePath, int width, int height, int quality = 90)
    {
        return _cropper.CropImageCenter(sourceFilePath, targetFilePath, width, height, quality);
    }

    /// <summary>
    /// 批量裁剪指定目录下的图片文件
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名（如 .jpg）</param>
    /// <param name="x">裁剪区域左上角X坐标（像素）</param>
    /// <param name="y">裁剪区域左上角Y坐标（像素）</param>
    /// <param name="width">裁剪区域宽度（像素）</param>
    /// <param name="height">裁剪区域高度（像素）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>裁剪结果信息</returns>
    public static ConversionResult BatchCrop(
        string sourceDirectory,
        string outputDirectory,
        string sourceExtension,
        int x,
        int y,
        int width,
        int height,
        int quality = 90)
    {
        return _cropper.BatchCrop(sourceDirectory, outputDirectory, sourceExtension, x, y, width, height, quality);
    }

    /// <summary>
    /// 批量中心裁剪指定目录下的图片文件
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名（如 .jpg）</param>
    /// <param name="width">裁剪区域宽度（像素）</param>
    /// <param name="height">裁剪区域高度（像素）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>裁剪结果信息</returns>
    public static ConversionResult BatchCropCenter(
        string sourceDirectory,
        string outputDirectory,
        string sourceExtension,
        int width,
        int height,
        int quality = 90)
    {
        return _cropper.BatchCropCenter(sourceDirectory, outputDirectory, sourceExtension, width, height, quality);
    }

    #region 缩略图功能

    /// <summary>
    /// 生成单个图片的缩略图
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="maxSize">缩略图最大边长（像素）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>生成是否成功</returns>
    public static bool GenerateThumbnail(string sourceFilePath, string targetFilePath, int maxSize = 150, int quality = 90)
    {
        return _resizer.GenerateThumbnail(sourceFilePath, targetFilePath, maxSize, quality);
    }

    /// <summary>
    /// 批量生成指定目录下图片文件的缩略图
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名</param>
    /// <param name="maxSize">缩略图最大边长（像素）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>生成结果信息</returns>
    public static ConversionResult BatchGenerateThumbnails(
        string sourceDirectory,
        string outputDirectory,
        string sourceExtension,
        int maxSize = 150,
        int quality = 90)
    {
        return _resizer.BatchGenerateThumbnails(sourceDirectory, outputDirectory, sourceExtension, maxSize, quality);
    }

    #endregion

    #region 水印功能

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
    public static bool AddTextWatermark(string sourceFilePath, string targetFilePath, string text, WatermarkPosition position = WatermarkPosition.BottomRight, int opacity = 50, int fontSize = 24, string fontColor = "#FFFFFF", int quality = 90)
    {
        return _watermark.AddTextWatermark(sourceFilePath, targetFilePath, text, position, opacity, fontSize, fontColor, quality);
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
    public static bool AddImageWatermark(string sourceFilePath, string targetFilePath, string watermarkImagePath, WatermarkPosition position = WatermarkPosition.BottomRight, int opacity = 50, float scale = 1.0f, int quality = 90)
    {
        return _watermark.AddImageWatermark(sourceFilePath, targetFilePath, watermarkImagePath, position, opacity, scale, quality);
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
    public static ConversionResult BatchAddTextWatermark(
        string sourceDirectory,
        string outputDirectory,
        string sourceExtension,
        string text,
        WatermarkPosition position = WatermarkPosition.BottomRight,
        int opacity = 50,
        int fontSize = 24,
        string fontColor = "#FFFFFF",
        int quality = 90)
    {
        return _watermark.BatchAddTextWatermark(sourceDirectory, outputDirectory, sourceExtension, text, position, opacity, fontSize, fontColor, quality);
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
    public static ConversionResult BatchAddImageWatermark(
        string sourceDirectory,
        string outputDirectory,
        string sourceExtension,
        string watermarkImagePath,
        WatermarkPosition position = WatermarkPosition.BottomRight,
        int opacity = 50,
        float scale = 1.0f,
        int quality = 90)
    {
        return _watermark.BatchAddImageWatermark(sourceDirectory, outputDirectory, sourceExtension, watermarkImagePath, position, opacity, scale, quality);
    }

    #endregion

}