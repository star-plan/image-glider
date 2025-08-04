using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Metadata.Profiles.Icc;

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
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        return tempDir;
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

    /// <summary>
    /// 创建带有元数据的测试图片
    /// </summary>
    /// <param name="filePath">文件路径，如果为空则创建临时文件</param>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <returns>创建的文件路径</returns>
    public static string CreateTestImageWithMetadata(string? filePath = null, int width = 100, int height = 100)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            filePath = Path.GetTempFileName() + ".jpg";
        }

        using var image = new Image<Rgba32>(width, height);
        image.Mutate(x => x.BackgroundColor(Color.Red));

        // 添加EXIF数据
        var exifProfile = new ExifProfile();
        exifProfile.SetValue(ExifTag.Make, "Test Camera");
        exifProfile.SetValue(ExifTag.Model, "Test Model");
        exifProfile.SetValue(ExifTag.Software, "Test Software");
        exifProfile.SetValue(ExifTag.DateTime, DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"));
        image.Metadata.ExifProfile = exifProfile;

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        image.Save(filePath, new JpegEncoder());
        return filePath;
    }

    /// <summary>
    /// 清理文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    public static void CleanupFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch
            {
                // 忽略删除失败的情况
            }
        }
    }
}