using System.ComponentModel;

namespace ImageGlider.WebApi.Model;

/// <summary>
/// 图片压缩请求模型
/// </summary>
public class ImageCompressRequest
{
    /// <summary>
    /// 图片文件
    /// </summary>
    public IFormFile file { get; set; }

    /// <summary>
    /// 压缩级别（1-100，数值越小压缩越强）
    /// </summary>
    [property: Description("压缩级别（1-100，数值越小压缩越强）")]
    public int CompressionLevel { get; set; } = 75;
    
    /// <summary>
    /// 是否保留元数据
    /// </summary>
    [property: Description("是否保留元数据")]
    public bool PreserveMetadata { get; set; } = false;
}