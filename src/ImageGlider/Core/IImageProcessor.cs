using ImageGlider.Enums;

namespace ImageGlider.Core;

/// <summary>
/// 图像处理器接口
/// </summary>
public interface IImageProcessor
{
    /// <summary>
    /// 处理单个图像文件
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="quality">质量参数（仅对 JPEG 格式有效，范围 1-100）</param>
    /// <returns>处理是否成功</returns>
    bool ProcessImage(string sourceFilePath, string targetFilePath, int quality = 90);
}

/// <summary>
/// 图像格式转换器接口
/// </summary>
public interface IImageFormatConverter : IImageProcessor
{
    /// <summary>
    /// 批量转换指定目录下的图片文件格式
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名</param>
    /// <param name="targetExtension">目标文件扩展名</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>转换结果信息</returns>
    ConversionResult BatchConvert(string sourceDirectory, string outputDirectory, string sourceExtension, string targetExtension, int quality = 90);
}

/// <summary>
/// 图像尺寸调整器接口
/// </summary>
public interface IImageResizer : IImageProcessor
{
    /// <summary>
    /// 调整单个图片文件的尺寸
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="width">目标宽度（像素）</param>
    /// <param name="height">目标高度（像素）</param>
    /// <param name="resizeMode">调整模式</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>调整是否成功</returns>
    bool ResizeImage(string sourceFilePath, string targetFilePath, int? width = null, int? height = null, ImageGlider.Enums.ResizeMode resizeMode = ImageGlider.Enums.ResizeMode.KeepAspectRatio, int quality = 90);

    /// <summary>
    /// 批量调整指定目录下图片文件的尺寸
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名</param>
    /// <param name="width">目标宽度（像素）</param>
    /// <param name="height">目标高度（像素）</param>
    /// <param name="resizeMode">调整模式</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>调整结果信息</returns>
    ConversionResult BatchResize(string sourceDirectory, string outputDirectory, string sourceExtension, int? width = null, int? height = null, ImageGlider.Enums.ResizeMode resizeMode = ImageGlider.Enums.ResizeMode.KeepAspectRatio, int quality = 90);
}