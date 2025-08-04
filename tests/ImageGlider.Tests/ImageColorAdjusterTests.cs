using System;
using System.IO;
using Xunit;
using ImageGlider;
using ImageGlider.Tests.TestHelpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageGlider.Tests;

/// <summary>
/// ImageConverter 图像颜色调整功能的单元测试
/// </summary>
public class ImageColorAdjusterTests
{
    /// <summary>
    /// 测试颜色调整 - 文件不存在的情况
    /// </summary>
    [Fact]
    public void AdjustColor_FileNotExists_ReturnsFalse()
    {
        // Arrange
        var sourceFile = "nonexistent.jpg";
        var targetFile = "output.jpg";

        // Act
        var result = ImageConverter.AdjustColor(sourceFile, targetFile);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试颜色调整 - 成功调整亮度
    /// </summary>
    [Fact]
    public void AdjustColor_ValidFile_AdjustsBrightnessSuccessfully()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act
            var result = ImageConverter.AdjustColor(sourceFile, targetFile, brightness: 20);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证输出文件是有效的图像
            using var image = Image.Load(targetFile);
            Assert.Equal(100, image.Width);
            Assert.Equal(100, image.Height);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试颜色调整 - 成功调整对比度
    /// </summary>
    [Fact]
    public void AdjustColor_ValidFile_AdjustsContrastSuccessfully()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act
            var result = ImageConverter.AdjustColor(sourceFile, targetFile, contrast: 15);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证输出文件是有效的图像
            using var image = Image.Load(targetFile);
            Assert.Equal(100, image.Width);
            Assert.Equal(100, image.Height);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试颜色调整 - 成功调整饱和度
    /// </summary>
    [Fact]
    public void AdjustColor_ValidFile_AdjustsSaturationSuccessfully()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act
            var result = ImageConverter.AdjustColor(sourceFile, targetFile, saturation: 30);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证输出文件是有效的图像
            using var image = Image.Load(targetFile);
            Assert.Equal(100, image.Width);
            Assert.Equal(100, image.Height);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试颜色调整 - 成功调整色相
    /// </summary>
    [Fact]
    public void AdjustColor_ValidFile_AdjustsHueSuccessfully()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act
            var result = ImageConverter.AdjustColor(sourceFile, targetFile, hue: 45);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证输出文件是有效的图像
            using var image = Image.Load(targetFile);
            Assert.Equal(100, image.Width);
            Assert.Equal(100, image.Height);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试颜色调整 - 成功调整伽马值
    /// </summary>
    [Fact]
    public void AdjustColor_ValidFile_AdjustsGammaSuccessfully()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act
            var result = ImageConverter.AdjustColor(sourceFile, targetFile, gamma: 1.5f);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证输出文件是有效的图像
            using var image = Image.Load(targetFile);
            Assert.Equal(100, image.Width);
            Assert.Equal(100, image.Height);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试颜色调整 - 组合调整多个参数
    /// </summary>
    [Fact]
    public void AdjustColor_ValidFile_AdjustsMultipleParametersSuccessfully()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act
            var result = ImageConverter.AdjustColor(sourceFile, targetFile, 
                brightness: 10, contrast: 15, saturation: 20, hue: 30, gamma: 1.2f);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证输出文件是有效的图像
            using var image = Image.Load(targetFile);
            Assert.Equal(100, image.Width);
            Assert.Equal(100, image.Height);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试批量颜色调整 - 源目录不存在
    /// </summary>
    [Fact]
    public void BatchAdjustColor_SourceDirectoryNotExists_ReturnsError()
    {
        // Arrange
        var sourceDir = "nonexistent_directory";
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        // Act
        var result = ImageConverter.BatchAdjustColor(sourceDir, outputDir, "jpg");

        // Assert
        Assert.Equal(0, result.TotalFiles);
        Assert.Equal(0, result.SuccessfulConversions);
        Assert.Equal(0, result.FailedConversions);
        Assert.Contains("源目录不存在", result.ErrorMessage);
    }

    /// <summary>
    /// 测试批量颜色调整 - 成功处理多个文件
    /// </summary>
    [Fact]
    public void BatchAdjustColor_ValidDirectory_ProcessesFilesSuccessfully()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceDir = Path.Combine(tempDir, "source");
        var outputDir = Path.Combine(tempDir, "output");
        
        try
        {
            Directory.CreateDirectory(sourceDir);
            
            // 创建测试图像文件
            var sourceFile1 = Path.Combine(sourceDir, "test1.jpg");
            var sourceFile2 = Path.Combine(sourceDir, "test2.jpg");
            TestImageHelper.CreateTestImage(sourceFile1, 100, 100);
            TestImageHelper.CreateTestImage(sourceFile2, 150, 150);

            // Act
            var result = ImageConverter.BatchAdjustColor(sourceDir, outputDir, "jpg", brightness: 20);

            // Assert
            Assert.Equal(2, result.TotalFiles);
            Assert.Equal(2, result.SuccessfulConversions);
            Assert.Equal(0, result.FailedConversions);
            Assert.True(string.IsNullOrEmpty(result.ErrorMessage));
            
            // 验证输出文件存在
            Assert.True(File.Exists(Path.Combine(outputDir, "test1_adjusted.jpg")));
            Assert.True(File.Exists(Path.Combine(outputDir, "test2_adjusted.jpg")));
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试参数边界值处理
    /// </summary>
    [Fact]
    public void AdjustColor_BoundaryValues_ClampsParametersCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act - 使用超出范围的参数值
            var result = ImageConverter.AdjustColor(sourceFile, targetFile, 
                brightness: 150, contrast: -150, saturation: 200, hue: 250, gamma: 5.0f);

            // Assert - 应该成功处理（参数会被限制在有效范围内）
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}