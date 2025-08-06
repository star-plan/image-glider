using System.ComponentModel;

namespace ImageGlider.WebApi.Model;

/// <summary>
/// 图片裁剪请求模型
/// </summary>
public class ImageCropRequest: ImageProcessingRequest
{
    /// <summary>
    /// 裁剪区域左上角X坐标（像素）
    /// </summary>
    [property: Description("裁剪区域左上角X坐标（像素）")]
    public int X { get; set; }
    
    /// <summary>
    /// 裁剪区域左上角Y坐标（像素）
    /// </summary>
    [property: Description("裁剪区域左上角Y坐标（像素）")]
    public int Y { get; set; }
    
    /// <summary>
    /// 裁剪区域宽度（像素）
    /// </summary>
    [property: Description("裁剪区域宽度（像素）")]
    public int Width { get; set; }
    
    /// <summary>
    /// 裁剪区域高度（像素）
    /// </summary>
    [property: Description("裁剪区域高度（像素）")]
    public int Height { get; set; }
}

/// <summary>
/// 中心裁剪请求模型
/// </summary>
public class ImageCenterCropRequest: ImageProcessingRequest
{
    /// <summary>
    /// 裁剪区域宽度（像素）
    /// </summary>
    public int Width { get; set; }
    
    /// <summary>
    /// 裁剪区域高度（像素）
    /// </summary>
    public int Height { get; set; }
}

/// <summary>
/// 百分比裁剪请求模型
/// </summary>
public class ImagePercentCropRequest: ImageProcessingRequest
{
    /// <summary>
    /// 裁剪区域左上角X坐标百分比（0-100）
    /// </summary>
    public float XPercent { get; set; }
    
    /// <summary>
    /// 裁剪区域左上角Y坐标百分比（0-100）
    /// </summary>
    public float YPercent { get; set; }
    
    /// <summary>
    /// 裁剪区域宽度百分比（0-100）
    /// </summary>
    public float WidthPercent { get; set; }
    
    /// <summary>
    /// 裁剪区域高度百分比（0-100）
    /// </summary>
    public float HeightPercent { get; set; }
}