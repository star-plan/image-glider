namespace ImageGlider.Enums;

/// <summary>
/// 图像尺寸调整模式
/// </summary>
public enum ResizeMode
{
    /// <summary>
    /// 拉伸模式：直接拉伸到目标尺寸，可能改变宽高比
    /// </summary>
    Stretch,

    /// <summary>
    /// 保持宽高比模式：按比例缩放，保持原始宽高比
    /// </summary>
    KeepAspectRatio,

    /// <summary>
    /// 裁剪模式：按比例缩放后裁剪到目标尺寸
    /// </summary>
    Crop
}