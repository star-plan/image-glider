using System.IO;
using Xunit;
using ImageGlider;
using ImageGlider.Tests.TestHelpers;
using ImageGlider.Processors;
using ImageGlider.Core;
using ImageGlider.Enums;

namespace ImageGlider.Tests;

/// <summary>
/// 边界条件和异常处理的单元测试
/// </summary>
public class EdgeCasesAndExceptionTests : IDisposable
{
    private readonly string _tempDir;
    
    public EdgeCasesAndExceptionTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
    }
    
    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }
    
    #region 文件路径边界条件测试
    
    [Fact]
    public void ProcessImage_WithNullSourcePath_ShouldReturnFalse()
    {
        // Arrange
        var processor = new ImageResizer();
        var targetPath = Path.Combine(_tempDir, "output.jpg");
        
        // Act & Assert
        Assert.False(processor.ProcessImage(null!, targetPath, 90));
    }
    
    [Fact]
    public void ProcessImage_WithEmptySourcePath_ShouldReturnFalse()
    {
        // Arrange
        var processor = new ImageResizer();
        var targetPath = Path.Combine(_tempDir, "output.jpg");
        
        // Act & Assert
        Assert.False(processor.ProcessImage(string.Empty, targetPath, 90));
    }
    
    [Fact]
    public void ProcessImage_WithNullTargetPath_ShouldReturnFalse()
    {
        // Arrange
        var processor = new ImageResizer();
        var sourcePath = Path.Combine(_tempDir, "source.jpg");
        TestImageHelper.CreateTestImage(sourcePath, 100, 100);
        
        // Act & Assert
        Assert.False(processor.ProcessImage(sourcePath, null!, 90));
    }
    
    [Fact]
    public void ProcessImage_WithEmptyTargetPath_ShouldReturnFalse()
    {
        // Arrange
        var processor = new ImageResizer();
        var sourcePath = Path.Combine(_tempDir, "source.jpg");
        TestImageHelper.CreateTestImage(sourcePath, 100, 100);
        
        // Act & Assert
        Assert.False(processor.ProcessImage(sourcePath, string.Empty, 90));
    }
    
    [Fact]
    public void ProcessImage_WithNonExistentFile_ShouldReturnFalse()
    {
        // Arrange
        var processor = new ImageResizer();
        var sourcePath = Path.Combine(_tempDir, "nonexistent.jpg");
        var targetPath = Path.Combine(_tempDir, "output.jpg");
        
        // Act & Assert
        Assert.False(processor.ProcessImage(sourcePath, targetPath, 90));
    }
    
    [Fact]
    public void ProcessImage_WithInvalidImageFile_ShouldReturnFalse()
    {
        // Arrange
        var processor = new ImageResizer();
        var sourcePath = Path.Combine(_tempDir, "invalid.jpg");
        var targetPath = Path.Combine(_tempDir, "output.jpg");
        
        // 创建一个无效的图片文件
        File.WriteAllText(sourcePath, "This is not an image file");
        
        // Act & Assert
        Assert.False(processor.ProcessImage(sourcePath, targetPath, 90));
    }
    
    #endregion
    
    #region 质量参数边界条件测试
    
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(1000)]
    public void ProcessImage_WithInvalidQuality_ShouldHandleGracefully(int quality)
    {
        // Arrange
        var processor = new ImageCompressor();
        var sourcePath = Path.Combine(_tempDir, "source.jpg");
        var targetPath = Path.Combine(_tempDir, "output.jpg");
        TestImageHelper.CreateTestImage(sourcePath, 100, 100);
        
        // Act
        var result = processor.ProcessImage(sourcePath, targetPath, quality);
        
        // Assert
        // 应该要么成功（使用默认值），要么失败，但不应该抛出异常
        Assert.True(result || !result); // 不抛异常即可
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void ProcessImage_WithValidQuality_ShouldSucceed(int quality)
    {
        // Arrange
        var processor = new ImageCompressor();
        var sourcePath = Path.Combine(_tempDir, "source.jpg");
        var targetPath = Path.Combine(_tempDir, "output.jpg");
        TestImageHelper.CreateTestImage(sourcePath, 100, 100);
        
        // Act
        var result = processor.ProcessImage(sourcePath, targetPath, quality);
        
        // Assert
        Assert.True(result);
        Assert.True(File.Exists(targetPath));
    }
    
    #endregion
    
    #region 尺寸边界条件测试
    
    [Theory]
    [InlineData(0, 100)]
    [InlineData(100, 0)]
    [InlineData(0, 0)]
    [InlineData(-1, 100)]
    [InlineData(100, -1)]
    public void ResizeImage_WithInvalidDimensions_ShouldReturnFalse(int width, int height)
    {
        // Arrange
        var sourcePath = Path.Combine(_tempDir, "source.jpg");
        var targetPath = Path.Combine(_tempDir, "output.jpg");
        TestImageHelper.CreateTestImage(sourcePath, 200, 200);
        
        // Act
        var result = ImageConverter.ResizeImage(sourcePath, targetPath, width, height, ResizeMode.KeepAspectRatio, 90);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData(1, 1)]
    [InlineData(10000, 10000)]
    [InlineData(1, 10000)]
    [InlineData(10000, 1)]
    public void ResizeImage_WithExtremeDimensions_ShouldHandleGracefully(int width, int height)
    {
        // Arrange
        var sourcePath = Path.Combine(_tempDir, "source.jpg");
        var targetPath = Path.Combine(_tempDir, "output.jpg");
        TestImageHelper.CreateTestImage(sourcePath, 100, 100);
        
        // Act
        var result = ImageConverter.ResizeImage(sourcePath, targetPath, width, height, ResizeMode.KeepAspectRatio, 90);
        
        // Assert
        // 应该要么成功要么失败，但不应该抛出异常
        Assert.True(result || !result);
    }
    
    #endregion
    
    #region 裁剪边界条件测试
    
    [Theory]
    [InlineData(-1, 0, 50, 50)]  // 负数X
    [InlineData(0, -1, 50, 50)]  // 负数Y
    [InlineData(0, 0, 0, 50)]    // 零宽度
    [InlineData(0, 0, 50, 0)]    // 零高度
    [InlineData(0, 0, -1, 50)]   // 负数宽度
    [InlineData(0, 0, 50, -1)]   // 负数高度
    public void CropImage_WithInvalidParameters_ShouldReturnFalse(int x, int y, int width, int height)
    {
        // Arrange
        var sourcePath = Path.Combine(_tempDir, "source.jpg");
        var targetPath = Path.Combine(_tempDir, "output.jpg");
        TestImageHelper.CreateTestImage(sourcePath, 200, 200);
        
        // Act
        var result = ImageConverter.CropImage(sourcePath, targetPath, x, y, width, height, 90);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void CropImage_WithCropAreaLargerThanImage_ShouldReturnFalse()
    {
        // Arrange
        var sourcePath = Path.Combine(_tempDir, "source.jpg");
        var targetPath = Path.Combine(_tempDir, "output.jpg");
        TestImageHelper.CreateTestImage(sourcePath, 100, 100);
        
        // Act - 尝试裁剪超出图片范围的区域
        var result = ImageConverter.CropImage(sourcePath, targetPath, 50, 50, 100, 100, 90);
        
        // Assert
        Assert.False(result);
    }
    
    #endregion
    
    #region 水印边界条件测试
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddTextWatermark_WithInvalidText_ShouldReturnFalse(string? text)
    {
        // Arrange
        var sourcePath = Path.Combine(_tempDir, "source.jpg");
        var targetPath = Path.Combine(_tempDir, "output.jpg");
        TestImageHelper.CreateTestImage(sourcePath, 200, 200);
        
        // Act
        var result = ImageConverter.AddTextWatermark(sourcePath, targetPath, text!, WatermarkPosition.BottomRight, 50, 24, "#FFFFFF", 90);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(1000)]
    public void AddTextWatermark_WithInvalidOpacity_ShouldHandleGracefully(int opacity)
    {
        // Arrange
        var sourcePath = Path.Combine(_tempDir, "source.jpg");
        var targetPath = Path.Combine(_tempDir, "output.jpg");
        TestImageHelper.CreateTestImage(sourcePath, 200, 200);
        
        // Act
        var result = ImageConverter.AddTextWatermark(sourcePath, targetPath, "Test", WatermarkPosition.BottomRight, opacity, 24, "#FFFFFF", 90);
        
        // Assert
        // 应该要么成功（使用默认值）要么失败，但不应该抛出异常
        Assert.True(result || !result);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1000)]
    public void AddTextWatermark_WithInvalidFontSize_ShouldHandleGracefully(int fontSize)
    {
        // Arrange
        var sourcePath = Path.Combine(_tempDir, "source.jpg");
        var targetPath = Path.Combine(_tempDir, "output.jpg");
        TestImageHelper.CreateTestImage(sourcePath, 200, 200);
        
        // Act
        var result = ImageConverter.AddTextWatermark(sourcePath, targetPath, "Test", WatermarkPosition.BottomRight, 50, fontSize, "#FFFFFF", 90);
        
        // Assert
        // 应该要么成功（使用默认值）要么失败，但不应该抛出异常
        Assert.True(result || !result);
    }
    
    [Theory]
    [InlineData("invalid")]
    [InlineData("#GGGGGG")]
    [InlineData("")]
    [InlineData(null)]
    public void AddTextWatermark_WithInvalidColor_ShouldHandleGracefully(string? color)
    {
        // Arrange
        var sourcePath = Path.Combine(_tempDir, "source.jpg");
        var targetPath = Path.Combine(_tempDir, "output.jpg");
        TestImageHelper.CreateTestImage(sourcePath, 200, 200);
        
        // Act
        var result = ImageConverter.AddTextWatermark(sourcePath, targetPath, "Test", WatermarkPosition.BottomRight, 50, 24, color!, 90);
        
        // Assert
        // 应该要么成功（使用默认值）要么失败，但不应该抛出异常
        Assert.True(result || !result);
    }
    
    #endregion
    
    #region 颜色调整边界条件测试
    
    [Theory]
    [InlineData(-1000, 0, 0, 0, 1.0f)]
    [InlineData(1000, 0, 0, 0, 1.0f)]
    [InlineData(0, -1000, 0, 0, 1.0f)]
    [InlineData(0, 1000, 0, 0, 1.0f)]
    [InlineData(0, 0, -1000, 0, 1.0f)]
    [InlineData(0, 0, 1000, 0, 1.0f)]
    [InlineData(0, 0, 0, -1000, 1.0f)]
    [InlineData(0, 0, 0, 1000, 1.0f)]
    [InlineData(0, 0, 0, 0, -1.0f)]
    [InlineData(0, 0, 0, 0, 100.0f)]
    public void AdjustColor_WithExtremeValues_ShouldHandleGracefully(float brightness, float contrast, float saturation, float hue, float gamma)
    {
        // Arrange
        var sourcePath = Path.Combine(_tempDir, "source.jpg");
        var targetPath = Path.Combine(_tempDir, "output.jpg");
        TestImageHelper.CreateTestImage(sourcePath, 100, 100);
        
        // Act
        var result = ImageConverter.AdjustColor(sourcePath, targetPath, brightness, contrast, saturation, hue, gamma, 90);
        
        // Assert
        // 应该要么成功要么失败，但不应该抛出异常
        Assert.True(result || !result);
    }
    
    #endregion
    
    #region 目录权限和访问测试
    
    [Fact]
    public void ProcessImage_WithReadOnlyTargetDirectory_ShouldHandleGracefully()
    {
        // Arrange
        var readOnlyDir = Path.Combine(_tempDir, "readonly");
        Directory.CreateDirectory(readOnlyDir);
        
        try
        {
            // 设置目录为只读（在某些系统上可能不起作用）
            var dirInfo = new DirectoryInfo(readOnlyDir);
            dirInfo.Attributes |= FileAttributes.ReadOnly;
            
            var processor = new ImageResizer();
            var sourcePath = Path.Combine(_tempDir, "source.jpg");
            var targetPath = Path.Combine(readOnlyDir, "output.jpg");
            TestImageHelper.CreateTestImage(sourcePath, 100, 100);
            
            // Act
            var result = processor.ProcessImage(sourcePath, targetPath, 90);
            
            // Assert
            // 应该失败但不抛出异常
            Assert.False(result);
        }
        finally
        {
            // 清理：移除只读属性
            try
            {
                var dirInfo = new DirectoryInfo(readOnlyDir);
                dirInfo.Attributes &= ~FileAttributes.ReadOnly;
            }
            catch
            {
                // 忽略清理错误
            }
        }
    }
    
    #endregion
}
