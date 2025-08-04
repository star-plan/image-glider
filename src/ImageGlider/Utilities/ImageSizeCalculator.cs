using ImageGlider.Enums;

namespace ImageGlider.Utilities;

/// <summary>
/// 图像尺寸计算器，负责计算目标尺寸
/// </summary>
public static class ImageSizeCalculator
{
    /// <summary>
    /// 计算目标尺寸
    /// </summary>
    /// <param name="originalWidth">原始宽度</param>
    /// <param name="originalHeight">原始高度</param>
    /// <param name="targetWidth">目标宽度</param>
    /// <param name="targetHeight">目标高度</param>
    /// <param name="resizeMode">调整模式</param>
    /// <returns>计算后的目标尺寸</returns>
    public static (int Width, int Height) CalculateTargetSize(int originalWidth, int originalHeight, int? targetWidth, int? targetHeight, ResizeMode resizeMode)
    {
        switch (resizeMode)
        {
            case ResizeMode.Stretch:
                return (targetWidth ?? originalWidth, targetHeight ?? originalHeight);

            case ResizeMode.KeepAspectRatio:
                if (targetWidth.HasValue && targetHeight.HasValue)
                {
                    // 两个尺寸都指定时，选择较小的缩放比例以保持宽高比
                    var scaleX = (double)targetWidth.Value / originalWidth;
                    var scaleY = (double)targetHeight.Value / originalHeight;
                    var scale = Math.Min(scaleX, scaleY);
                    return ((int)(originalWidth * scale), (int)(originalHeight * scale));
                }
                else if (targetWidth.HasValue)
                {
                    // 只指定宽度，按比例计算高度
                    var scale = (double)targetWidth.Value / originalWidth;
                    return (targetWidth.Value, (int)(originalHeight * scale));
                }
                else if (targetHeight.HasValue)
                {
                    // 只指定高度，按比例计算宽度
                    var scale = (double)targetHeight.Value / originalHeight;
                    return ((int)(originalWidth * scale), targetHeight.Value);
                }
                break;

            case ResizeMode.Crop:
                if (targetWidth.HasValue && targetHeight.HasValue)
                {
                    // 裁剪模式：选择较大的缩放比例，然后裁剪
                    var scaleX = (double)targetWidth.Value / originalWidth;
                    var scaleY = (double)targetHeight.Value / originalHeight;
                    var scale = Math.Max(scaleX, scaleY);
                    return ((int)(originalWidth * scale), (int)(originalHeight * scale));
                }
                else
                {
                    // 裁剪模式需要同时指定宽度和高度
                    throw new ArgumentException("裁剪模式需要同时指定宽度和高度");
                }
        }

        return (originalWidth, originalHeight);
    }
}