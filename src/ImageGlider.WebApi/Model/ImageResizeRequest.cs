using System.ComponentModel;
using ImageGlider.Enums;

namespace ImageGlider.WebApi.Model;

/// <summary>
/// 图片尺寸调整请求模型
/// </summary>
public class ImageResizeRequest: ImageProcessingRequest
{
    /// <summary>
    /// 目标宽度（像素）
    /// </summary>
    [property: Description("目标宽度（像素）")]
    public int? Width { get; set; }
    
    /// <summary>
    /// 目标高度（像素）
    /// </summary>
    [property: Description("目标高度（像素）")]
    public int? Height { get; set; }
    
    /// <summary>
    /// 调整模式
    /// </summary>
    [property: Description("调整模式 Stretch|拉伸模式, KeepAspectRatio|保持宽高比模式, Crop|裁剪模式")]
    public ResizeMode ResizeMode { get; set; } = ResizeMode.KeepAspectRatio;
}

/// <summary>
/// 缩略图生成请求模型
/// </summary>
public class ThumbnailRequest: ImageProcessingRequest
{
    /// <summary>
    /// 缩略图最大边长（像素）
    /// </summary>
    [property: Description("缩略图最大边长（像素）")]
    public int MaxSize { get; set; } = 150;
}