using System.ComponentModel;

namespace ImageGlider.WebApi.Model;

public class ImageConvertRequest: ImageProcessingRequest
{
    /// <summary>
    /// 转换后目标后缀名
    /// </summary>
    [property: Description("转换后目标后缀名")]
    public string FileExt { get; set; }
}