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

    /// <summary>
    /// 创建一个非常小的测试图像
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>创建的文件路径</returns>
    public static string CreateTinyTestImage(string? filePath = null)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            filePath = Path.GetTempFileName() + ".jpg";
        }

        CreateTestImage(filePath, 1, 1, Color.White);
        return filePath;
    }

    /// <summary>
    /// 创建一个大尺寸的测试图像
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="width">宽度，默认2000</param>
    /// <param name="height">高度，默认2000</param>
    /// <returns>创建的文件路径</returns>
    public static string CreateLargeTestImage(string? filePath = null, int width = 2000, int height = 2000)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            filePath = Path.GetTempFileName() + ".jpg";
        }

        CreateTestImage(filePath, width, height, Color.Green);
        return filePath;
    }

    /// <summary>
    /// 创建一个损坏的图像文件（实际上是文本文件）
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>创建的文件路径</returns>
    public static string CreateCorruptImageFile(string? filePath = null)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            filePath = Path.GetTempFileName() + ".jpg";
        }

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, "This is not a valid image file content");
        return filePath;
    }

    /// <summary>
    /// 验证文件是否为有效的图像文件
    /// </summary>
    /// <param name="imagePath">图像文件路径</param>
    /// <returns>是否为有效图像</returns>
    public static bool IsValidImageFile(string imagePath)
    {
        if (!File.Exists(imagePath))
            return false;

        try
        {
            using var image = Image.Load(imagePath);
            return image.Width > 0 && image.Height > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 获取图像文件的大小信息
    /// </summary>
    /// <param name="imagePath">图像文件路径</param>
    /// <returns>图像尺寸，如果无效则返回(0,0)</returns>
    public static (int Width, int Height) GetImageSize(string imagePath)
    {
        if (!File.Exists(imagePath))
            return (0, 0);

        try
        {
            using var image = Image.Load(imagePath);
            return (image.Width, image.Height);
        }
        catch
        {
            return (0, 0);
        }
    }

    /// <summary>
    /// 比较两个图像文件的尺寸是否相同
    /// </summary>
    /// <param name="imagePath1">第一个图像路径</param>
    /// <param name="imagePath2">第二个图像路径</param>
    /// <returns>尺寸是否相同</returns>
    public static bool CompareImageSizes(string imagePath1, string imagePath2)
    {
        var size1 = GetImageSize(imagePath1);
        var size2 = GetImageSize(imagePath2);
        return size1.Width == size2.Width && size1.Height == size2.Height;
    }
}