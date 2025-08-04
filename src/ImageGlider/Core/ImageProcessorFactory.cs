using ImageGlider.Processors;

namespace ImageGlider.Core;

/// <summary>
/// 图像处理器工厂类，用于创建和管理不同的图像处理器
/// </summary>
public static class ImageProcessorFactory
{
    /// <summary>
    /// 创建图像格式转换器实例
    /// </summary>
    /// <returns>图像格式转换器实例</returns>
    public static IImageFormatConverter CreateFormatConverter()
    {
        return new ImageFormatConverter();
    }

    /// <summary>
    /// 创建图像尺寸调整器实例
    /// </summary>
    /// <returns>图像尺寸调整器实例</returns>
    public static IImageResizer CreateResizer()
    {
        return new ImageResizer();
    }

    /// <summary>
    /// 创建图像压缩器实例
    /// </summary>
    /// <returns>图像压缩器实例</returns>
    public static IImageCompressor CreateCompressor()
    {
        return new ImageCompressor();
    }

    /// <summary>
    /// 创建图像裁剪器实例
    /// </summary>
    /// <returns>图像裁剪器实例</returns>
    public static IImageCropper CreateCropper()
    {
        return new ImageCropper();
    }

    /// <summary>
    /// 创建图像水印处理器实例
    /// </summary>
    /// <returns>图像水印处理器实例</returns>
    public static IImageWatermark CreateWatermark()
    {
        return new ImageWatermark();
    }

    /// <summary>
    /// 根据处理类型创建对应的处理器
    /// </summary>
    /// <param name="processorType">处理器类型</param>
    /// <returns>图像处理器实例</returns>
    /// <exception cref="ArgumentException">不支持的处理器类型</exception>
    public static IImageProcessor CreateProcessor(ImageProcessorType processorType)
    {
        return processorType switch
        {
            ImageProcessorType.FormatConverter => CreateFormatConverter(),
            ImageProcessorType.Resizer => CreateResizer(),
            ImageProcessorType.Compressor => CreateCompressor(),
            ImageProcessorType.Cropper => CreateCropper(),
            ImageProcessorType.Watermark => CreateWatermark(),
            _ => throw new ArgumentException($"不支持的处理器类型: {processorType}")
        };
    }
}

/// <summary>
/// 图像处理器类型枚举
/// </summary>
public enum ImageProcessorType
{
    /// <summary>
    /// 格式转换器
    /// </summary>
    FormatConverter,

    /// <summary>
    /// 尺寸调整器
    /// </summary>
    Resizer,

    /// <summary>
    /// 压缩优化器
    /// </summary>
    Compressor,

    /// <summary>
    /// 图像裁剪器
    /// </summary>
    Cropper,

    /// <summary>
    /// 图像水印处理器
    /// </summary>
    Watermark
}