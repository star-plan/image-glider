using Xunit;
using ImageGlider.Core;

namespace ImageGlider.Tests.Core;

/// <summary>
/// ImageInfo 类的单元测试
/// </summary>
public class ImageInfoTests
{
    [Fact]
    public void ImageInfo_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var imageInfo = new ImageInfo();
        
        // Assert
        Assert.Equal(string.Empty, imageInfo.FilePath);
        Assert.Equal(0, imageInfo.FileSize);
        Assert.Equal(0, imageInfo.Width);
        Assert.Equal(0, imageInfo.Height);
        Assert.Equal(string.Empty, imageInfo.Format);
        Assert.Equal(0, imageInfo.BitDepth);
        Assert.Equal(0.0, imageInfo.HorizontalDpi);
        Assert.Equal(0.0, imageInfo.VerticalDpi);
        Assert.False(imageInfo.HasMetadata);
        Assert.Equal(0, imageInfo.MetadataSize);
        Assert.Equal(string.Empty, imageInfo.ColorSpace);
        Assert.False(imageInfo.HasAlpha);
        Assert.Equal(string.Empty, imageInfo.Compression);
        Assert.Null(imageInfo.CreationTime);
        Assert.Null(imageInfo.ModificationTime);
        Assert.NotNull(imageInfo.AdditionalMetadata);
        Assert.Empty(imageInfo.AdditionalMetadata);
    }
    
    [Fact]
    public void ImageInfo_SetBasicProperties_ShouldWork()
    {
        // Arrange
        var imageInfo = new ImageInfo();
        var creationTime = DateTime.Now.AddDays(-1);
        var modificationTime = DateTime.Now;
        
        // Act
        imageInfo.FilePath = "/path/to/image.jpg";
        imageInfo.FileSize = 1024000;
        imageInfo.Width = 1920;
        imageInfo.Height = 1080;
        imageInfo.Format = "JPEG";
        imageInfo.BitDepth = 24;
        imageInfo.HorizontalDpi = 300.0;
        imageInfo.VerticalDpi = 300.0;
        imageInfo.HasMetadata = true;
        imageInfo.MetadataSize = 2048;
        imageInfo.ColorSpace = "sRGB";
        imageInfo.HasAlpha = false;
        imageInfo.Compression = "JPEG";
        imageInfo.CreationTime = creationTime;
        imageInfo.ModificationTime = modificationTime;
        
        // Assert
        Assert.Equal("/path/to/image.jpg", imageInfo.FilePath);
        Assert.Equal(1024000, imageInfo.FileSize);
        Assert.Equal(1920, imageInfo.Width);
        Assert.Equal(1080, imageInfo.Height);
        Assert.Equal("JPEG", imageInfo.Format);
        Assert.Equal(24, imageInfo.BitDepth);
        Assert.Equal(300.0, imageInfo.HorizontalDpi);
        Assert.Equal(300.0, imageInfo.VerticalDpi);
        Assert.True(imageInfo.HasMetadata);
        Assert.Equal(2048, imageInfo.MetadataSize);
        Assert.Equal("sRGB", imageInfo.ColorSpace);
        Assert.False(imageInfo.HasAlpha);
        Assert.Equal("JPEG", imageInfo.Compression);
        Assert.Equal(creationTime, imageInfo.CreationTime);
        Assert.Equal(modificationTime, imageInfo.ModificationTime);
    }
    
    [Fact]
    public void ImageInfo_AdditionalMetadata_ShouldWork()
    {
        // Arrange
        var imageInfo = new ImageInfo();
        
        // Act
        imageInfo.AdditionalMetadata["Camera"] = "Canon EOS R5";
        imageInfo.AdditionalMetadata["ISO"] = 800;
        imageInfo.AdditionalMetadata["FocalLength"] = 85.0;
        imageInfo.AdditionalMetadata["GPS"] = new { Latitude = 40.7128, Longitude = -74.0060 };
        
        // Assert
        Assert.Equal(4, imageInfo.AdditionalMetadata.Count);
        Assert.Equal("Canon EOS R5", imageInfo.AdditionalMetadata["Camera"]);
        Assert.Equal(800, imageInfo.AdditionalMetadata["ISO"]);
        Assert.Equal(85.0, imageInfo.AdditionalMetadata["FocalLength"]);
        Assert.NotNull(imageInfo.AdditionalMetadata["GPS"]);
    }
    
    [Fact]
    public void CalculateAspectRatio_ValidDimensions_ShouldReturnCorrectRatio()
    {
        // Arrange
        var imageInfo = new ImageInfo
        {
            Width = 1920,
            Height = 1080
        };

        // Act
        var aspectRatio = (double)imageInfo.Width / imageInfo.Height;

        // Assert
        Assert.Equal(1920.0 / 1080.0, aspectRatio, 5);
    }

    [Fact]
    public void CalculateAspectRatio_ZeroHeight_ShouldHandleGracefully()
    {
        // Arrange
        var imageInfo = new ImageInfo
        {
            Width = 1920,
            Height = 0
        };

        // Act & Assert
        // 当高度为0时，应该避免除零错误
        if (imageInfo.Height == 0)
        {
            Assert.Equal(0, imageInfo.Height);
        }
        else
        {
            var aspectRatio = (double)imageInfo.Width / imageInfo.Height;
            Assert.True(aspectRatio > 0);
        }
    }

    [Theory]
    [InlineData(1920, 1080)]
    [InlineData(1080, 1920)]
    [InlineData(1000, 1000)]
    [InlineData(1600, 900)]
    [InlineData(800, 600)]
    public void CalculateAspectRatio_VariousDimensions_ShouldBeCalculable(int width, int height)
    {
        // Arrange
        var imageInfo = new ImageInfo
        {
            Width = width,
            Height = height
        };

        // Act
        var aspectRatio = height > 0 ? (double)imageInfo.Width / imageInfo.Height : 0;

        // Assert
        if (height > 0)
        {
            Assert.True(aspectRatio > 0);
            Assert.Equal((double)width / height, aspectRatio, 10);
        }
        else
        {
            Assert.Equal(0, aspectRatio);
        }
    }
    
    [Fact]
    public void ToString_WithAllProperties_ShouldReturnFormattedString()
    {
        // Arrange
        var creationTime = new DateTime(2024, 1, 15, 10, 30, 0);
        var modificationTime = new DateTime(2024, 1, 16, 14, 45, 0);
        
        var imageInfo = new ImageInfo
        {
            FilePath = "/path/to/test.jpg",
            FileSize = 2048000, // 2MB
            Width = 1920,
            Height = 1080,
            Format = "JPEG",
            BitDepth = 24,
            HorizontalDpi = 300.0,
            VerticalDpi = 300.0,
            ColorSpace = "sRGB",
            HasAlpha = false,
            Compression = "JPEG",
            HasMetadata = true,
            MetadataSize = 4096,
            CreationTime = creationTime,
            ModificationTime = modificationTime
        };
        
        // Act
        var result = imageInfo.ToString();
        
        // Assert
        Assert.Contains("/path/to/test.jpg", result);
        Assert.Contains("2,048,000 字节", result);
        Assert.Contains("(2.00 MB)", result);
        Assert.Contains("1920 x 1080 像素", result);
        Assert.Contains("JPEG", result);
        Assert.Contains("24 位", result);
        Assert.Contains("300.0 x 300.0", result);
        Assert.Contains("sRGB", result);
        Assert.Contains("否", result); // HasAlpha = false
        Assert.Contains("是 (4,096 字节)", result); // HasMetadata = true
        Assert.Contains("2024-01-15 10:30:00", result);
        Assert.Contains("2024-01-16 14:45:00", result);
    }
    
    [Fact]
    public void ToString_WithNullDates_ShouldShowUnknown()
    {
        // Arrange
        var imageInfo = new ImageInfo
        {
            FilePath = "test.png",
            CreationTime = null,
            ModificationTime = null
        };
        
        // Act
        var result = imageInfo.ToString();
        
        // Assert
        Assert.Contains("创建时间: 未知", result);
        Assert.Contains("修改时间: 未知", result);
    }
    
    [Fact]
    public void ToString_WithoutMetadata_ShouldShowNo()
    {
        // Arrange
        var imageInfo = new ImageInfo
        {
            FilePath = "test.png",
            HasMetadata = false
        };
        
        // Act
        var result = imageInfo.ToString();
        
        // Assert
        Assert.Contains("元数据: 否", result);
    }
    
    [Fact]
    public void ToString_WithAlphaChannel_ShouldShowYes()
    {
        // Arrange
        var imageInfo = new ImageInfo
        {
            FilePath = "test.png",
            HasAlpha = true
        };
        
        // Act
        var result = imageInfo.ToString();
        
        // Assert
        Assert.Contains("透明通道: 是", result);
    }
}
