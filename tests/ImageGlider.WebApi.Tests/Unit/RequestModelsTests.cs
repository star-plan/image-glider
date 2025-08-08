using ImageGlider.WebApi.Model;
using ImageGlider.Enums;
using Microsoft.AspNetCore.Http;

namespace ImageGlider.WebApi.Tests.Unit;

/// <summary>
/// 请求模型单元测试
/// </summary>
public class RequestModelsTests
{
    [Fact]
    public void ImageConvertRequest_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var request = new ImageConvertRequest();
        
        // Assert
        request.Quality.Should().Be(90);
        request.FileExt.Should().BeNull();
        request.file.Should().BeNull();
    }
    
    [Fact]
    public void ImageConvertRequest_SetProperties_ShouldWork()
    {
        // Arrange
        var request = new ImageConvertRequest();
        var mockFile = CreateMockFormFile("test.jpg");
        
        // Act
        request.file = mockFile;
        request.FileExt = "png";
        request.Quality = 85;
        
        // Assert
        request.file.Should().Be(mockFile);
        request.FileExt.Should().Be("png");
        request.Quality.Should().Be(85);
    }
    
    [Fact]
    public void ImageCropRequest_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var request = new ImageCropRequest();
        
        // Assert
        request.Quality.Should().Be(90);
        request.X.Should().Be(0);
        request.Y.Should().Be(0);
        request.Width.Should().Be(0);
        request.Height.Should().Be(0);
    }
    
    [Fact]
    public void ImageCropRequest_SetProperties_ShouldWork()
    {
        // Arrange
        var request = new ImageCropRequest();
        var mockFile = CreateMockFormFile("test.jpg");
        
        // Act
        request.file = mockFile;
        request.X = 50;
        request.Y = 100;
        request.Width = 200;
        request.Height = 150;
        request.Quality = 80;
        
        // Assert
        request.file.Should().Be(mockFile);
        request.X.Should().Be(50);
        request.Y.Should().Be(100);
        request.Width.Should().Be(200);
        request.Height.Should().Be(150);
        request.Quality.Should().Be(80);
    }
    
    [Fact]
    public void ImageCenterCropRequest_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var request = new ImageCenterCropRequest();
        
        // Assert
        request.Quality.Should().Be(90);
        request.Width.Should().Be(0);
        request.Height.Should().Be(0);
    }
    
    [Fact]
    public void ImagePercentCropRequest_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var request = new ImagePercentCropRequest();
        
        // Assert
        request.Quality.Should().Be(90);
        request.XPercent.Should().Be(0);
        request.YPercent.Should().Be(0);
        request.WidthPercent.Should().Be(0);
        request.HeightPercent.Should().Be(0);
    }
    
    [Fact]
    public void ImageColorRequest_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var request = new ImageColorRequest();
        
        // Assert
        request.Quality.Should().Be(90);
        request.Brightness.Should().Be(0);
        request.Contrast.Should().Be(0);
        request.Saturation.Should().Be(0);
        request.Hue.Should().Be(0);
        request.Gamma.Should().Be(1.0f);
    }
    
    [Fact]
    public void ImageColorRequest_SetProperties_ShouldWork()
    {
        // Arrange
        var request = new ImageColorRequest();
        var mockFile = CreateMockFormFile("test.jpg");
        
        // Act
        request.file = mockFile;
        request.Brightness = 10.5f;
        request.Contrast = -5.2f;
        request.Saturation = 15.8f;
        request.Hue = 45.0f;
        request.Gamma = 1.2f;
        request.Quality = 85;
        
        // Assert
        request.file.Should().Be(mockFile);
        request.Brightness.Should().Be(10.5f);
        request.Contrast.Should().Be(-5.2f);
        request.Saturation.Should().Be(15.8f);
        request.Hue.Should().Be(45.0f);
        request.Gamma.Should().Be(1.2f);
        request.Quality.Should().Be(85);
    }
    
    [Fact]
    public void ImageMetadataRequest_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var request = new ImageMetadataRequest();
        
        // Assert
        request.Quality.Should().Be(90);
        request.StripAll.Should().BeTrue();
        request.StripExif.Should().BeTrue();
        request.StripIcc.Should().BeFalse();
        request.StripXmp.Should().BeTrue();
    }
    
    [Fact]
    public void ImageMetadataRequest_SetProperties_ShouldWork()
    {
        // Arrange
        var request = new ImageMetadataRequest();
        var mockFile = CreateMockFormFile("test.jpg");
        
        // Act
        request.file = mockFile;
        request.StripAll = false;
        request.StripExif = false;
        request.StripIcc = true;
        request.StripXmp = false;
        request.Quality = 75;
        
        // Assert
        request.file.Should().Be(mockFile);
        request.StripAll.Should().BeFalse();
        request.StripExif.Should().BeFalse();
        request.StripIcc.Should().BeTrue();
        request.StripXmp.Should().BeFalse();
        request.Quality.Should().Be(75);
    }
    
    [Fact]
    public void TextWatermarkRequest_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var request = new TextWatermarkRequest();
        
        // Assert
        request.Quality.Should().Be(90);
        request.Text.Should().Be(string.Empty);
        request.Position.Should().Be(WatermarkPosition.BottomRight);
        request.Opacity.Should().Be(50);
        request.FontSize.Should().Be(24);
        request.FontColor.Should().Be("#FFFFFF");
    }
    
    [Fact]
    public void TextWatermarkRequest_SetProperties_ShouldWork()
    {
        // Arrange
        var request = new TextWatermarkRequest();
        var mockFile = CreateMockFormFile("test.jpg");
        
        // Act
        request.file = mockFile;
        request.Text = "Test Watermark";
        request.Position = WatermarkPosition.TopLeft;
        request.Opacity = 75;
        request.FontSize = 36;
        request.FontColor = "#FF0000";
        request.Quality = 85;
        
        // Assert
        request.file.Should().Be(mockFile);
        request.Text.Should().Be("Test Watermark");
        request.Position.Should().Be(WatermarkPosition.TopLeft);
        request.Opacity.Should().Be(75);
        request.FontSize.Should().Be(36);
        request.FontColor.Should().Be("#FF0000");
        request.Quality.Should().Be(85);
    }
    
    [Fact]
    public void ImageWatermarkRequest_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var request = new ImageWatermarkRequest();
        
        // Assert
        request.Quality.Should().Be(90);
        request.Position.Should().Be(WatermarkPosition.BottomRight);
        request.Opacity.Should().Be(50);
        request.Scale.Should().Be(1.0f);
        request.watermarkFile.Should().BeNull();
    }
    
    [Fact]
    public void ImageWatermarkRequest_SetProperties_ShouldWork()
    {
        // Arrange
        var request = new ImageWatermarkRequest();
        var mockFile = CreateMockFormFile("test.jpg");
        var mockWatermark = CreateMockFormFile("watermark.png");
        
        // Act
        request.file = mockFile;
        request.watermarkFile = mockWatermark;
        request.Position = WatermarkPosition.Center;
        request.Opacity = 30;
        request.Scale = 0.5f;
        request.Quality = 80;
        
        // Assert
        request.file.Should().Be(mockFile);
        request.watermarkFile.Should().Be(mockWatermark);
        request.Position.Should().Be(WatermarkPosition.Center);
        request.Opacity.Should().Be(30);
        request.Scale.Should().Be(0.5f);
        request.Quality.Should().Be(80);
    }
    
    [Fact]
    public void ImageResizeRequest_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var request = new ImageResizeRequest();
        
        // Assert
        request.Quality.Should().Be(90);
        request.Width.Should().BeNull();
        request.Height.Should().BeNull();
        request.ResizeMode.Should().Be(ResizeMode.KeepAspectRatio);
    }
    
    [Fact]
    public void ImageResizeRequest_SetProperties_ShouldWork()
    {
        // Arrange
        var request = new ImageResizeRequest();
        var mockFile = CreateMockFormFile("test.jpg");
        
        // Act
        request.file = mockFile;
        request.Width = 800;
        request.Height = 600;
        request.ResizeMode = ResizeMode.Stretch;
        request.Quality = 85;
        
        // Assert
        request.file.Should().Be(mockFile);
        request.Width.Should().Be(800);
        request.Height.Should().Be(600);
        request.ResizeMode.Should().Be(ResizeMode.Stretch);
        request.Quality.Should().Be(85);
    }
    
    private static IFormFile CreateMockFormFile(string fileName)
    {
        var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5 });
        return new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
    }
}
