using System;
using System.IO;
using Xunit;
using ImageGlider;
using ImageGlider.Tests.TestHelpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageGlider.Tests;

/// <summary>
/// ImageConverter 图像裁剪功能的单元测试
/// </summary>
public class ImageCropperTests
{
    /// <summary>
    /// 测试裁剪图像 - 文件不存在的情况
    /// </summary>
    [Fact]
    public void CropImage_FileNotExists_ReturnsFalse()
    {
        // Arrange
        var sourceFile = "nonexistent.jpg";
        var targetFile = "output.jpg";

        // Act
        var result = ImageConverter.CropImage(sourceFile, targetFile, 0, 0, 100, 100);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试裁剪图像 - 成功裁剪
    /// </summary>
    [Fact]
    public void CropImage_ValidFile_CropsSuccessfully()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "cropped.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 200, 200);

            // Act
            var result = ImageConverter.CropImage(sourceFile, targetFile, 50, 50, 100, 100);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证裁剪后的图像尺寸
            using var croppedImage = Image.Load(targetFile);
            Assert.Equal(100, croppedImage.Width);
            Assert.Equal(100, croppedImage.Height);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试裁剪图像 - 无效参数
    /// </summary>
    [Fact]
    public void CropImage_InvalidParameters_ReturnsFalse()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "cropped.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act & Assert - 负坐标
            var result1 = ImageConverter.CropImage(sourceFile, targetFile, -10, 0, 50, 50);
            Assert.False(result1);

            // Act & Assert - 零宽度
            var result2 = ImageConverter.CropImage(sourceFile, targetFile, 0, 0, 0, 50);
            Assert.False(result2);

            // Act & Assert - 零高度
            var result3 = ImageConverter.CropImage(sourceFile, targetFile, 0, 0, 50, 0);
            Assert.False(result3);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试裁剪图像 - 超出边界返回失败
    /// </summary>
    [Fact]
    public void CropImage_OutOfBounds_ReturnsFalse()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "cropped.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act - 裁剪区域超出图像边界
            var result = ImageConverter.CropImage(sourceFile, targetFile, 50, 50, 100, 100);

            // Assert
            Assert.False(result);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试按百分比裁剪图像 - 成功裁剪
    /// </summary>
    [Fact]
    public void CropImageByPercent_ValidParameters_CropsSuccessfully()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "cropped.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 200, 200);

            // Act - 从25%位置开始，裁剪50%的宽度和高度
            var result = ImageConverter.CropImageByPercent(sourceFile, targetFile, 25f, 25f, 50f, 50f);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证裁剪后的图像尺寸（200 * 50% = 100）
            using var croppedImage = Image.Load(targetFile);
            Assert.Equal(100, croppedImage.Width);
            Assert.Equal(100, croppedImage.Height);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试按百分比裁剪图像 - 无效百分比
    /// </summary>
    [Fact]
    public void CropImageByPercent_InvalidPercent_ReturnsFalse()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "cropped.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act & Assert - 负百分比
            var result1 = ImageConverter.CropImageByPercent(sourceFile, targetFile, -10f, 0f, 50f, 50f);
            Assert.False(result1);

            // Act & Assert - 超过100%
            var result2 = ImageConverter.CropImageByPercent(sourceFile, targetFile, 0f, 0f, 150f, 50f);
            Assert.False(result2);

            // Act & Assert - 起始位置+尺寸超过100%
            var result3 = ImageConverter.CropImageByPercent(sourceFile, targetFile, 60f, 60f, 50f, 50f);
            Assert.False(result3);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试中心裁剪图像 - 成功裁剪
    /// </summary>
    [Fact]
    public void CropImageCenter_ValidParameters_CropsSuccessfully()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "cropped.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 200, 200);

            // Act - 中心裁剪为100x100
            var result = ImageConverter.CropImageCenter(sourceFile, targetFile, 100, 100);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证裁剪后的图像尺寸
            using var croppedImage = Image.Load(targetFile);
            Assert.Equal(100, croppedImage.Width);
            Assert.Equal(100, croppedImage.Height);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试中心裁剪图像 - 裁剪尺寸大于原图
    /// </summary>
    [Fact]
    public void CropImageCenter_LargerThanSource_AdjustsToSourceSize()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "cropped.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act - 尝试中心裁剪为200x200（大于原图100x100）
            var result = ImageConverter.CropImageCenter(sourceFile, targetFile, 200, 200);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证裁剪后的图像尺寸应该等于原图尺寸
            using var croppedImage = Image.Load(targetFile);
            Assert.Equal(100, croppedImage.Width);
            Assert.Equal(100, croppedImage.Height);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试批量裁剪 - 成功处理多个文件
    /// </summary>
    [Fact]
    public void BatchCrop_ValidFiles_ProcessesSuccessfully()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceDir = Path.Combine(tempDir, "source");
        var outputDir = Path.Combine(tempDir, "output");
        
        try
        {
            Directory.CreateDirectory(sourceDir);
            Directory.CreateDirectory(outputDir);
            
            // 创建测试图像
            var sourceFile1 = Path.Combine(sourceDir, "test1.jpg");
            var sourceFile2 = Path.Combine(sourceDir, "test2.jpg");
            TestImageHelper.CreateTestImage(sourceFile1, 200, 200);
            TestImageHelper.CreateTestImage(sourceFile2, 200, 200);

            // Act
            var result = ImageConverter.BatchCrop(sourceDir, outputDir, "jpg", 50, 50, 100, 100);

            // Assert
            Assert.Equal(2, result.SuccessfulConversions);
            Assert.Equal(0, result.FailedConversions);
            Assert.Equal(2, result.TotalFiles);
            
            // 验证输出文件存在
            var outputFile1 = Path.Combine(outputDir, "test1.jpg");
            var outputFile2 = Path.Combine(outputDir, "test2.jpg");
            Assert.True(File.Exists(outputFile1));
            Assert.True(File.Exists(outputFile2));
            
            // 验证裁剪后的图像尺寸
            using var croppedImage1 = Image.Load(outputFile1);
            using var croppedImage2 = Image.Load(outputFile2);
            Assert.Equal(100, croppedImage1.Width);
            Assert.Equal(100, croppedImage1.Height);
            Assert.Equal(100, croppedImage2.Width);
            Assert.Equal(100, croppedImage2.Height);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试批量裁剪 - 源目录不存在
    /// </summary>
    [Fact]
    public void BatchCrop_SourceDirectoryNotExists_ReturnsError()
    {
        // Arrange
        var nonExistentDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        // Act
        var result = ImageConverter.BatchCrop(nonExistentDir, outputDir, "jpg", 0, 0, 100, 100);

        // Assert
        Assert.Equal(0, result.SuccessfulConversions);
        Assert.Equal(0, result.FailedConversions);
        Assert.Equal(0, result.TotalFiles);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("源目录不存在", result.ErrorMessage);
    }

    /// <summary>
    /// 测试批量裁剪 - 没有匹配的文件
    /// </summary>
    [Fact]
    public void BatchCrop_NoMatchingFiles_ReturnsError()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceDir = Path.Combine(tempDir, "source");
        var outputDir = Path.Combine(tempDir, "output");
        
        try
        {
            Directory.CreateDirectory(sourceDir);
            
            // 创建一个非jpg文件
            var pngFile = Path.Combine(sourceDir, "test.png");
            TestImageHelper.CreateTestImage(pngFile, 100, 100);

            // Act - 查找jpg文件
            var result = ImageConverter.BatchCrop(sourceDir, outputDir, "jpg", 0, 0, 50, 50);

            // Assert
            Assert.Equal(0, result.SuccessfulConversions);
            Assert.Equal(0, result.FailedConversions);
            Assert.Equal(0, result.TotalFiles);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("未找到匹配的文件", result.ErrorMessage);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}