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

    /// <summary>
    /// 生成单个图片的缩略图
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="maxSize">缩略图最大边长（像素）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>生成是否成功</returns>
    bool GenerateThumbnail(string sourceFilePath, string targetFilePath, int maxSize = 150, int quality = 90);

    /// <summary>
    /// 批量生成指定目录下图片文件的缩略图
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名</param>
    /// <param name="maxSize">缩略图最大边长（像素）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>生成结果信息</returns>
    ConversionResult BatchGenerateThumbnails(string sourceDirectory, string outputDirectory, string sourceExtension, int maxSize = 150, int quality = 90);
}

/// <summary>
/// 图像压缩优化器接口
/// </summary>
public interface IImageCompressor : IImageProcessor
{
    /// <summary>
    /// 压缩优化单个图片文件
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="compressionLevel">压缩级别（1-100，数值越小压缩越强）</param>
    /// <param name="preserveMetadata">是否保留元数据</param>
    /// <returns>压缩是否成功</returns>
    bool CompressImage(string sourceFilePath, string targetFilePath, int compressionLevel = 75, bool preserveMetadata = false);

    /// <summary>
    /// 批量压缩优化指定目录下的图片文件
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名</param>
    /// <param name="compressionLevel">压缩级别</param>
    /// <param name="preserveMetadata">是否保留元数据</param>
    /// <returns>压缩结果信息</returns>
    ConversionResult BatchCompress(string sourceDirectory, string outputDirectory, string sourceExtension, int compressionLevel = 75, bool preserveMetadata = false);
}

/// <summary>
/// 图像裁剪器接口
/// </summary>
public interface IImageCropper : IImageProcessor
{
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
    bool CropImage(string sourceFilePath, string targetFilePath, int x, int y, int width, int height, int quality = 90);

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
    bool CropImageByPercent(string sourceFilePath, string targetFilePath, float xPercent, float yPercent, float widthPercent, float heightPercent, int quality = 90);

    /// <summary>
    /// 中心裁剪单个图片文件
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="width">裁剪区域宽度（像素）</param>
    /// <param name="height">裁剪区域高度（像素）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>裁剪是否成功</returns>
    bool CropImageCenter(string sourceFilePath, string targetFilePath, int width, int height, int quality = 90);

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
    ConversionResult BatchCrop(string sourceDirectory, string outputDirectory, string sourceExtension, int x, int y, int width, int height, int quality = 90);

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
    ConversionResult BatchCropCenter(string sourceDirectory, string outputDirectory, string sourceExtension, int width, int height, int quality = 90);
}

/// <summary>
/// 图像水印处理器接口
/// </summary>
public interface IImageWatermark : IImageProcessor
{
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
    bool AddTextWatermark(string sourceFilePath, string targetFilePath, string text, WatermarkPosition position = WatermarkPosition.BottomRight, int opacity = 50, int fontSize = 24, string fontColor = "#FFFFFF", int quality = 90);

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
    bool AddImageWatermark(string sourceFilePath, string targetFilePath, string watermarkImagePath, WatermarkPosition position = WatermarkPosition.BottomRight, int opacity = 50, float scale = 1.0f, int quality = 90);

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
    ConversionResult BatchAddTextWatermark(string sourceDirectory, string outputDirectory, string sourceExtension, string text, WatermarkPosition position = WatermarkPosition.BottomRight, int opacity = 50, int fontSize = 24, string fontColor = "#FFFFFF", int quality = 90);

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
    ConversionResult BatchAddImageWatermark(string sourceDirectory, string outputDirectory, string sourceExtension, string watermarkImagePath, WatermarkPosition position = WatermarkPosition.BottomRight, int opacity = 50, float scale = 1.0f, int quality = 90);
}

/// <summary>
/// 图像元数据清理器接口
/// </summary>
public interface IImageMetadataStripper : IImageProcessor
{
    /// <summary>
    /// 清理单个图片文件的元数据
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="stripAll">是否清理所有元数据（包括EXIF、ICC、XMP等）</param>
    /// <param name="stripExif">是否清理EXIF数据</param>
    /// <param name="stripIcc">是否清理ICC配置文件</param>
    /// <param name="stripXmp">是否清理XMP数据</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>清理是否成功</returns>
    bool StripMetadata(string sourceFilePath, string targetFilePath, bool stripAll = true, bool stripExif = true, bool stripIcc = false, bool stripXmp = true, int quality = 90);

    /// <summary>
    /// 批量清理指定目录下图片文件的元数据
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名</param>
    /// <param name="stripAll">是否清理所有元数据（包括EXIF、ICC、XMP等）</param>
    /// <param name="stripExif">是否清理EXIF数据</param>
    /// <param name="stripIcc">是否清理ICC配置文件</param>
    /// <param name="stripXmp">是否清理XMP数据</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>清理结果信息</returns>
    BatchStripResult BatchStripMetadata(string sourceDirectory, string outputDirectory, string sourceExtension, bool stripAll = true, bool stripExif = true, bool stripIcc = false, bool stripXmp = true, int quality = 90);
}