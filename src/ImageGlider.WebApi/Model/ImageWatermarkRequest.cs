using System.ComponentModel;
using ImageGlider.Enums;

namespace ImageGlider.WebApi.Model;

/// <summary>
/// 文本水印请求模型
/// </summary>
public class TextWatermarkRequest: ImageProcessingRequest
{
    /// <summary>
    /// 水印文本
    /// </summary>
    [property: Description("水印文本")]
    public string Text { get; set; } = string.Empty;
    
    /// <summary>
    /// 水印位置
    /// </summary>
    [property: Description("水印位置 左上角|TopLeft,上方中央|TopCenter,右上角|TopRight,左侧中央|MiddleLeft,中央|Center,右侧中央|MiddleRight,左下角|BottomLeft,下方中央|BottomCenter,右下角|BottomRight")]
    public WatermarkPosition Position { get; set; } = WatermarkPosition.BottomRight;
    
    /// <summary>
    /// 透明度（0-100）
    /// </summary>
    [property: Description("透明度（0-100）")]
    public int Opacity { get; set; } = 50;
    
    /// <summary>
    /// 字体大小
    /// </summary>
    [property: Description("字体大小")]
    public int FontSize { get; set; } = 24;
    
    /// <summary>
    /// 字体颜色（十六进制，如 #FFFFFF）
    /// </summary>
    [property: Description("字体颜色（十六进制，如 #FFFFFF）")]
    public string FontColor { get; set; } = "#FFFFFF";
}

/// <summary>
/// 图片水印请求模型
/// </summary>
public class ImageWatermarkRequest: ImageProcessingRequest
{
    /// <summary>
    /// 图片水印文件
    /// </summary>
    [property: Description("图片水印文件")]
    public IFormFile watermarkFile { get; set; }
    /// <summary>
    /// 水印位置
    /// </summary>
    [property: Description("水印位置")]
    public WatermarkPosition Position { get; set; } = WatermarkPosition.BottomRight;
    
    /// <summary>
    /// 透明度（0-100）
    /// </summary>
    [property: Description("透明度（0-100）")]
    public int Opacity { get; set; } = 50;
    
    /// <summary>
    /// 缩放比例（0.1-2.0）
    /// </summary>
    [property: Description("缩放比例（0.1-2.0）")]
    public float Scale { get; set; } = 1.0f;
}