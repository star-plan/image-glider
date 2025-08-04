using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageGlider.Tests.TestHelpers;

/// <summary>
/// 测试辅助类，提供创建测试图像的公共方法
/// </summary>
public static class TestImageHelper
{
    /// <summary>
    /// 创建测试用的图片文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="color">背景颜色，默认为蓝色</param>
    public static void CreateTestImage(string filePath, int width, int height, Color? color = null)
    {
        using var image = new Image<Rgba32>(width, height);
        // 填充背景色
        image.Mutate(x => x.BackgroundColor(color ?? Color.Blue));
        
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        image.Save(filePath, new JpegEncoder());
    }

    /// <summary>
    /// 创建临时目录
    /// </summary>
    /// <returns>临时目录路径</returns>
    public static string CreateTempDirectory()
    {
        return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    }

    /// <summary>
    /// 清理临时目录
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    public static void CleanupDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            Directory.Delete(directoryPath, true);
        }
    }

    /// <summary>
    /// 验证图像尺寸
    /// </summary>
    /// <param name="imagePath">图像文件路径</param>
    /// <param name="expectedWidth">期望的宽度</param>
    /// <param name="expectedHeight">期望的高度</param>
    /// <returns>尺寸是否匹配</returns>
    public static bool VerifyImageSize(string imagePath, int expectedWidth, int expectedHeight)
    {
        if (!File.Exists(imagePath))
            return false;

        using var image = Image.Load(imagePath);
        return image.Width == expectedWidth && image.Height == expectedHeight;
    }
}