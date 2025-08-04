using System;
using System.IO;
using System.Linq;
using Xunit;
using ImageGlider.Core;
using ImageGlider.Processors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageGlider.Tests;

/// <summary>
/// 图像信息提取器测试类
/// </summary>
public class ImageInfoExtractorTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly ImageInfoExtractor _extractor;

    /// <summary>
    /// 初始化测试
    /// </summary>
    public ImageInfoExtractorTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "ImageGliderTests_" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(_testDirectory);
        _extractor = new ImageInfoExtractor();
    }

    /// <summary>
    /// 清理测试资源
    /// </summary>
    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }

    /// <summary>
    /// 测试提取基本图像信息
    /// </summary>
    [Fact]
    public void ExtractImageInfo_ShouldReturnBasicInfo()
    {
        // Arrange
        var testImagePath = CreateTestImage("test.png", 800, 600, Color.Blue);

        // Act
        var result = _extractor.ExtractImageInfo(testImagePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testImagePath, result.FilePath);
        Assert.Equal(800, result.Width);
        Assert.Equal(600, result.Height);
        Assert.Equal("PNG", result.Format);
        Assert.True(result.FileSize > 0);
        Assert.True(result.BitDepth > 0);
    }

    /// <summary>
    /// 测试提取JPEG图像信息
    /// </summary>
    [Fact]
    public void ExtractImageInfo_JpegImage_ShouldReturnCorrectFormat()
    {
        // Arrange
        var testImagePath = CreateTestImage("test.jpg", 1024, 768, Color.Red);

        // Act
        var result = _extractor.ExtractImageInfo(testImagePath);

        // Assert
        Assert.Equal("JPEG", result.Format);
        Assert.Equal(1024, result.Width);
        Assert.Equal(768, result.Height);
        Assert.Contains("JPEG", result.Compression);
    }

    /// <summary>
    /// 测试文件不存在的情况
    /// </summary>
    [Fact]
    public void ExtractImageInfo_FileNotExists_ShouldThrowException()
    {
        // Arrange
        var nonExistentPath = Path.Combine(_testDirectory, "nonexistent.jpg");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _extractor.ExtractImageInfo(nonExistentPath));
        Assert.Contains("文件不存在", exception.Message);
    }

    /// <summary>
    /// 测试批量提取图像信息
    /// </summary>
    [Fact]
    public void BatchExtractImageInfo_ShouldReturnMultipleResults()
    {
        // Arrange
        CreateTestImage("image1.png", 100, 100, Color.Red);
        CreateTestImage("image2.jpg", 200, 200, Color.Green);
        CreateTestImage("image3.png", 300, 300, Color.Blue);
        CreateTestImage("document.txt", content: "This is not an image"); // 非图像文件

        // Act
        var results = _extractor.BatchExtractImageInfo(_testDirectory);

        // Assert
        Assert.Equal(3, results.Count); // 应该只返回3个图像文件
        Assert.All(results, r => Assert.True(r.Width > 0 && r.Height > 0));
        
        var pngFiles = results.Where(r => r.Format == "PNG").ToList();
        var jpegFiles = results.Where(r => r.Format == "JPEG").ToList();
        
        Assert.Equal(2, pngFiles.Count);
        Assert.Single(jpegFiles);
    }

    /// <summary>
    /// 测试批量提取时目录不存在的情况
    /// </summary>
    [Fact]
    public void BatchExtractImageInfo_DirectoryNotExists_ShouldThrowException()
    {
        // Arrange
        var nonExistentDir = Path.Combine(_testDirectory, "nonexistent");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _extractor.BatchExtractImageInfo(nonExistentDir));
        Assert.Contains("目录不存在", exception.Message);
    }

    /// <summary>
    /// 测试递归搜索功能
    /// </summary>
    [Fact]
    public void BatchExtractImageInfo_Recursive_ShouldFindImagesInSubdirectories()
    {
        // Arrange
        var subDir = Path.Combine(_testDirectory, "subdir");
        Directory.CreateDirectory(subDir);
        
        CreateTestImage("root.png", 100, 100, Color.Red);
        CreateTestImage(Path.Combine("subdir", "sub.jpg"), 200, 200, Color.Green);

        // Act - 非递归搜索
        var nonRecursiveResults = _extractor.BatchExtractImageInfo(_testDirectory, "*.*", false);
        
        // Act - 递归搜索
        var recursiveResults = _extractor.BatchExtractImageInfo(_testDirectory, "*.*", true);

        // Assert
        Assert.Single(nonRecursiveResults); // 只找到根目录的文件
        Assert.Equal(2, recursiveResults.Count); // 找到所有文件
    }

    /// <summary>
    /// 测试搜索模式过滤
    /// </summary>
    [Fact]
    public void BatchExtractImageInfo_SearchPattern_ShouldFilterCorrectly()
    {
        // Arrange
        CreateTestImage("image1.png", 100, 100, Color.Red);
        CreateTestImage("image2.jpg", 200, 200, Color.Green);
        CreateTestImage("image3.png", 300, 300, Color.Blue);

        // Act
        var pngResults = _extractor.BatchExtractImageInfo(_testDirectory, "*.png");
        var jpgResults = _extractor.BatchExtractImageInfo(_testDirectory, "*.jpg");

        // Assert
        Assert.Equal(2, pngResults.Count);
        Assert.Single(jpgResults);
        Assert.All(pngResults, r => Assert.Equal("PNG", r.Format));
        Assert.All(jpgResults, r => Assert.Equal("JPEG", r.Format));
    }

    /// <summary>
    /// 测试DPI信息提取
    /// </summary>
    [Fact]
    public void ExtractImageInfo_ShouldExtractDpiInfo()
    {
        // Arrange
        var testImagePath = CreateTestImage("test_dpi.png", 400, 300, Color.Yellow);

        // Act
        var result = _extractor.ExtractImageInfo(testImagePath);

        // Assert
        Assert.True(result.HorizontalDpi > 0);
        Assert.True(result.VerticalDpi > 0);
    }

    /// <summary>
    /// 测试透明通道检测
    /// </summary>
    [Fact]
    public void ExtractImageInfo_PngWithAlpha_ShouldDetectAlphaChannel()
    {
        // Arrange - 创建带透明通道的PNG
        var testImagePath = Path.Combine(_testDirectory, "alpha_test.png");
        using (var image = new Image<Rgba32>(100, 100))
        {
            image.Mutate(x => x.BackgroundColor(Color.FromRgba(255, 0, 0, 128))); // 半透明红色
            image.SaveAsPng(testImagePath);
        }

        // Act
        var result = _extractor.ExtractImageInfo(testImagePath);

        // Assert
        Assert.True(result.HasAlpha);
    }

    /// <summary>
    /// 创建测试图像文件
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="color">颜色</param>
    /// <returns>创建的文件路径</returns>
    private string CreateTestImage(string fileName, int width = 100, int height = 100, Color? color = null)
    {
        var filePath = Path.Combine(_testDirectory, fileName);
        var directory = Path.GetDirectoryName(filePath)!;
        
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var image = new Image<Rgb24>(width, height);
        image.Mutate(x => x.BackgroundColor(color ?? Color.White));
        
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        switch (extension)
        {
            case ".jpg":
            case ".jpeg":
                image.SaveAsJpeg(filePath);
                break;
            case ".png":
                image.SaveAsPng(filePath);
                break;
            case ".bmp":
                image.SaveAsBmp(filePath);
                break;
            default:
                image.SaveAsPng(filePath);
                break;
        }

        return filePath;
    }

    /// <summary>
    /// 创建测试文本文件
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="content">文件内容</param>
    /// <returns>创建的文件路径</returns>
    private string CreateTestImage(string fileName, string content)
    {
        var filePath = Path.Combine(_testDirectory, fileName);
        File.WriteAllText(filePath, content);
        return filePath;
    }
}