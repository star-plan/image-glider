using System.Collections.Generic;

namespace ImageGlider.Core;

/// <summary>
/// 图像信息数据模型，包含图片的基本信息
/// </summary>
public class ImageInfo
{
    /// <summary>
    /// 文件路径
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 图像宽度（像素）
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 图像高度（像素）
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// 图像格式
    /// </summary>
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// 色彩深度（位）
    /// </summary>
    public int BitDepth { get; set; }

    /// <summary>
    /// 水平DPI
    /// </summary>
    public double HorizontalDpi { get; set; }

    /// <summary>
    /// 垂直DPI
    /// </summary>
    public double VerticalDpi { get; set; }

    /// <summary>
    /// 是否包含元数据
    /// </summary>
    public bool HasMetadata { get; set; }

    /// <summary>
    /// 元数据大小（字节）
    /// </summary>
    public long MetadataSize { get; set; }

    /// <summary>
    /// 颜色空间
    /// </summary>
    public string ColorSpace { get; set; } = string.Empty;

    /// <summary>
    /// 是否有透明通道
    /// </summary>
    public bool HasAlpha { get; set; }

    /// <summary>
    /// 压缩类型
    /// </summary>
    public string Compression { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreationTime { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    public DateTime? ModificationTime { get; set; }

    /// <summary>
    /// 额外的元数据信息
    /// </summary>
    public Dictionary<string, object> AdditionalMetadata { get; set; } = new();

    /// <summary>
    /// 转换为JSON格式字符串
    /// </summary>
    /// <returns>JSON格式的图像信息</returns>
    public string ToJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(this, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }

    /// <summary>
    /// 转换为格式化的字符串
    /// </summary>
    /// <returns>格式化的图像信息字符串</returns>
    public override string ToString()
    {
        return $"文件: {FilePath}\n" +
               $"大小: {FileSize:N0} 字节 ({FileSize / 1024.0 / 1024.0:F2} MB)\n" +
               $"尺寸: {Width} x {Height} 像素\n" +
               $"格式: {Format}\n" +
               $"色深: {BitDepth} 位\n" +
               $"DPI: {HorizontalDpi:F1} x {VerticalDpi:F1}\n" +
               $"颜色空间: {ColorSpace}\n" +
               $"透明通道: {(HasAlpha ? "是" : "否")}\n" +
               $"压缩: {Compression}\n" +
               $"元数据: {(HasMetadata ? $"是 ({MetadataSize:N0} 字节)" : "否")}\n" +
               $"创建时间: {CreationTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "未知"}\n" +
               $"修改时间: {ModificationTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "未知"}";
    }
}