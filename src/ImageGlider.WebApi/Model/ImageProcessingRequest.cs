using System.ComponentModel;

namespace ImageGlider.WebApi.Model;

/// <summary>
/// 图片处理请求基类
/// </summary>
public abstract class ImageProcessingRequest
{
    /// <summary>
    /// 图片文件
    /// </summary>
    public IFormFile file { get; set; }

    /// <summary>
    /// JPEG 质量（1-100）
    /// </summary>
    [property: Description("JPEG 质量（1-100）")]
    public int Quality { get; set; } = 90;
}