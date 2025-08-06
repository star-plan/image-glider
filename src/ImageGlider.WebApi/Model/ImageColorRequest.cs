using System.ComponentModel;

namespace ImageGlider.WebApi.Model;

/// <summary>
/// 图片颜色调整请求模型
/// </summary>
public class ImageColorRequest: ImageProcessingRequest
{
    /// <summary>
    /// 亮度调整（-100到100，0为不调整）
    /// </summary>
    [property: Description("亮度调整（-100到100，0为不调整）")]
    public float Brightness { get; set; } = 0;
    
    /// <summary>
    /// 对比度调整（-100到100，0为不调整）
    /// </summary>
    [property: Description("对比度调整（-100到100，0为不调整）")]
    public float Contrast { get; set; } = 0;
    
    /// <summary>
    /// 饱和度调整（-100到100，0为不调整）
    /// </summary>
    [property: Description("饱和度调整（-100到100，0为不调整）")]
    public float Saturation { get; set; } = 0;
    
    /// <summary>
    /// 色相调整（-180到180，0为不调整）
    /// </summary>
    [property: Description("色相调整（-180到180，0为不调整）")]
    public float Hue { get; set; } = 0;
    
    /// <summary>
    /// 伽马值调整（0.1到3.0，1.0为不调整）
    /// </summary>
    [property: Description("伽马值调整（0.1到3.0，1.0为不调整）")]
    public float Gamma { get; set; } = 1.0f;
}