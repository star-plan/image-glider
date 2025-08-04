using Xunit;
using ImageGlider.Utilities;
using ImageGlider.Enums;

namespace ImageGlider.Tests;

/// <summary>
/// ImageSizeCalculator 尺寸计算功能的单元测试
/// </summary>
public class ImageSizeCalculatorTests
{
    /// <summary>
    /// 测试拉伸模式 - 指定宽度和高度
    /// </summary>
    [Fact]
    public void CalculateTargetSize_StretchMode_BothDimensions_ReturnsTargetSize()
    {
        // Arrange
        var originalWidth = 200;
        var originalHeight = 100;
        var targetWidth = 400;
        var targetHeight = 300;

        // Act
        var result = ImageSizeCalculator.CalculateTargetSize(
            originalWidth, originalHeight, targetWidth, targetHeight, ResizeMode.Stretch);

        // Assert
        Assert.Equal(400, result.Width);
        Assert.Equal(300, result.Height);
    }

    /// <summary>
    /// 测试拉伸模式 - 只指定宽度
    /// </summary>
    [Fact]
    public void CalculateTargetSize_StretchMode_WidthOnly_ReturnsTargetWidthOriginalHeight()
    {
        // Arrange
        var originalWidth = 200;
        var originalHeight = 100;
        var targetWidth = 400;

        // Act
        var result = ImageSizeCalculator.CalculateTargetSize(
            originalWidth, originalHeight, targetWidth, null, ResizeMode.Stretch);

        // Assert
        Assert.Equal(400, result.Width);
        Assert.Equal(100, result.Height); // 保持原始高度
    }

    /// <summary>
    /// 测试拉伸模式 - 只指定高度
    /// </summary>
    [Fact]
    public void CalculateTargetSize_StretchMode_HeightOnly_ReturnsOriginalWidthTargetHeight()
    {
        // Arrange
        var originalWidth = 200;
        var originalHeight = 100;
        var targetHeight = 300;

        // Act
        var result = ImageSizeCalculator.CalculateTargetSize(
            originalWidth, originalHeight, null, targetHeight, ResizeMode.Stretch);

        // Assert
        Assert.Equal(200, result.Width); // 保持原始宽度
        Assert.Equal(300, result.Height);
    }

    /// <summary>
    /// 测试拉伸模式 - 都不指定
    /// </summary>
    [Fact]
    public void CalculateTargetSize_StretchMode_NoDimensions_ReturnsOriginalSize()
    {
        // Arrange
        var originalWidth = 200;
        var originalHeight = 100;

        // Act
        var result = ImageSizeCalculator.CalculateTargetSize(
            originalWidth, originalHeight, null, null, ResizeMode.Stretch);

        // Assert
        Assert.Equal(200, result.Width);
        Assert.Equal(100, result.Height);
    }

    /// <summary>
    /// 测试保持宽高比模式 - 指定宽度和高度，宽度限制
    /// </summary>
    [Fact]
    public void CalculateTargetSize_KeepAspectRatio_BothDimensions_WidthLimiting()
    {
        // Arrange
        var originalWidth = 400;
        var originalHeight = 200; // 2:1 宽高比
        var targetWidth = 200;
        var targetHeight = 200;

        // Act
        var result = ImageSizeCalculator.CalculateTargetSize(
            originalWidth, originalHeight, targetWidth, targetHeight, ResizeMode.KeepAspectRatio);

        // Assert
        Assert.Equal(200, result.Width);
        Assert.Equal(100, result.Height); // 保持2:1宽高比
    }

    /// <summary>
    /// 测试保持宽高比模式 - 指定宽度和高度，高度限制
    /// </summary>
    [Fact]
    public void CalculateTargetSize_KeepAspectRatio_BothDimensions_HeightLimiting()
    {
        // Arrange
        var originalWidth = 200;
        var originalHeight = 400; // 1:2 宽高比
        var targetWidth = 200;
        var targetHeight = 200;

        // Act
        var result = ImageSizeCalculator.CalculateTargetSize(
            originalWidth, originalHeight, targetWidth, targetHeight, ResizeMode.KeepAspectRatio);

        // Assert
        Assert.Equal(100, result.Width); // 保持1:2宽高比
        Assert.Equal(200, result.Height);
    }

    /// <summary>
    /// 测试保持宽高比模式 - 只指定宽度
    /// </summary>
    [Fact]
    public void CalculateTargetSize_KeepAspectRatio_WidthOnly_CalculatesHeight()
    {
        // Arrange
        var originalWidth = 400;
        var originalHeight = 200; // 2:1 宽高比
        var targetWidth = 100;

        // Act
        var result = ImageSizeCalculator.CalculateTargetSize(
            originalWidth, originalHeight, targetWidth, null, ResizeMode.KeepAspectRatio);

        // Assert
        Assert.Equal(100, result.Width);
        Assert.Equal(50, result.Height); // 保持2:1宽高比
    }

    /// <summary>
    /// 测试保持宽高比模式 - 只指定高度
    /// </summary>
    [Fact]
    public void CalculateTargetSize_KeepAspectRatio_HeightOnly_CalculatesWidth()
    {
        // Arrange
        var originalWidth = 400;
        var originalHeight = 200; // 2:1 宽高比
        var targetHeight = 100;

        // Act
        var result = ImageSizeCalculator.CalculateTargetSize(
            originalWidth, originalHeight, null, targetHeight, ResizeMode.KeepAspectRatio);

        // Assert
        Assert.Equal(200, result.Width); // 保持2:1宽高比
        Assert.Equal(100, result.Height);
    }

    /// <summary>
    /// 测试保持宽高比模式 - 都不指定
    /// </summary>
    [Fact]
    public void CalculateTargetSize_KeepAspectRatio_NoDimensions_ReturnsOriginalSize()
    {
        // Arrange
        var originalWidth = 400;
        var originalHeight = 200;

        // Act
        var result = ImageSizeCalculator.CalculateTargetSize(
            originalWidth, originalHeight, null, null, ResizeMode.KeepAspectRatio);

        // Assert
        Assert.Equal(400, result.Width);
        Assert.Equal(200, result.Height);
    }

    /// <summary>
    /// 测试裁剪模式 - 指定宽度和高度
    /// </summary>
    [Fact]
    public void CalculateTargetSize_CropMode_BothDimensions_ReturnsScaledSize()
    {
        // Arrange
        var originalWidth = 400;
        var originalHeight = 200; // 2:1 宽高比
        var targetWidth = 200;
        var targetHeight = 200; // 1:1 宽高比

        // Act
        var result = ImageSizeCalculator.CalculateTargetSize(
            originalWidth, originalHeight, targetWidth, targetHeight, ResizeMode.Crop);

        // Assert
        // 应该选择较大的缩放比例（高度的缩放比例更大）
        Assert.Equal(400, result.Width); // 200 * 2 = 400
        Assert.Equal(200, result.Height); // 200 * 1 = 200
    }

    /// <summary>
    /// 测试裁剪模式 - 只指定宽度，应该抛出异常
    /// </summary>
    [Fact]
    public void CalculateTargetSize_CropMode_WidthOnly_ThrowsException()
    {
        // Arrange
        var originalWidth = 400;
        var originalHeight = 200;
        var targetWidth = 200;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            ImageSizeCalculator.CalculateTargetSize(
                originalWidth, originalHeight, targetWidth, null, ResizeMode.Crop));
    }

    /// <summary>
    /// 测试裁剪模式 - 只指定高度，应该抛出异常
    /// </summary>
    [Fact]
    public void CalculateTargetSize_CropMode_HeightOnly_ThrowsException()
    {
        // Arrange
        var originalWidth = 400;
        var originalHeight = 200;
        var targetHeight = 200;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            ImageSizeCalculator.CalculateTargetSize(
                originalWidth, originalHeight, null, targetHeight, ResizeMode.Crop));
    }

    /// <summary>
    /// 测试裁剪模式 - 都不指定，应该抛出异常
    /// </summary>
    [Fact]
    public void CalculateTargetSize_CropMode_NoDimensions_ThrowsException()
    {
        // Arrange
        var originalWidth = 400;
        var originalHeight = 200;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            ImageSizeCalculator.CalculateTargetSize(
                originalWidth, originalHeight, null, null, ResizeMode.Crop));
    }

    /// <summary>
    /// 测试边界情况 - 零尺寸
    /// </summary>
    [Theory]
    [InlineData(0, 100)]
    [InlineData(100, 0)]
    [InlineData(0, 0)]
    public void CalculateTargetSize_ZeroDimensions_HandlesGracefully(int originalWidth, int originalHeight)
    {
        // Act
        var result = ImageSizeCalculator.CalculateTargetSize(
            originalWidth, originalHeight, 100, 100, ResizeMode.Stretch);

        // Assert
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    /// <summary>
    /// 测试边界情况 - 非常大的尺寸
    /// </summary>
    [Fact]
    public void CalculateTargetSize_LargeDimensions_HandlesCorrectly()
    {
        // Arrange
        var originalWidth = 10000;
        var originalHeight = 5000;
        var targetWidth = 1000;
        var targetHeight = 1000;

        // Act
        var result = ImageSizeCalculator.CalculateTargetSize(
            originalWidth, originalHeight, targetWidth, targetHeight, ResizeMode.KeepAspectRatio);

        // Assert
        Assert.Equal(1000, result.Width);
        Assert.Equal(500, result.Height); // 保持2:1宽高比
    }

    /// <summary>
    /// 测试精度 - 小数计算结果
    /// </summary>
    [Fact]
    public void CalculateTargetSize_FractionalResults_RoundsToInteger()
    {
        // Arrange
        var originalWidth = 300;
        var originalHeight = 200;
        var targetWidth = 100;

        // Act
        var result = ImageSizeCalculator.CalculateTargetSize(
            originalWidth, originalHeight, targetWidth, null, ResizeMode.KeepAspectRatio);

        // Assert
        Assert.Equal(100, result.Width);
        Assert.Equal(66, result.Height); // 200 * (100/300) = 66.666... -> 66
    }
}
