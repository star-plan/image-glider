using System.ComponentModel;

namespace ImageGlider.WebApi.Model;

/// <summary>
/// 图片元数据清理请求模型
/// </summary>
public class ImageMetadataRequest: ImageProcessingRequest
{
    /// <summary>
    /// 是否清理所有元数据（包括EXIF、ICC、XMP等）
    /// </summary>
    [property: Description("是否清理所有元数据（包括EXIF、ICC、XMP等）")]
    public bool StripAll { get; set; } = true;
    
    /// <summary>
    /// 是否清理EXIF数据
    /// </summary>
    [property: Description("是否清理EXIF数据")]
    public bool StripExif { get; set; } = true;
    
    /// <summary>
    /// 是否清理ICC配置文件
    /// </summary>
    [property: Description("是否清理ICC配置文件")]
    public bool StripIcc { get; set; } = false;
    
    /// <summary>
    /// 是否清理XMP数据
    /// </summary>
    [property: Description("是否清理XMP数据")]
    public bool StripXmp { get; set; } = true;
}