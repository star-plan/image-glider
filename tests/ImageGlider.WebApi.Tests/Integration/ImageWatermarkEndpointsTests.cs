using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ImageGlider.WebApi.Model;
using ImageGlider.WebApi.Tests.TestHelpers;
using ImageGlider.Enums;

namespace ImageGlider.WebApi.Tests.Integration;

/// <summary>
/// 图片水印端点集成测试
/// </summary>
public class ImageWatermarkEndpointsTests : WebApiTestBase
{
    public ImageWatermarkEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task AddTextWatermark_WithValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 400, 300);
        var additionalFields = new Dictionary<string, string>
        {
            ["Text"] = "Test Watermark",
            ["Position"] = WatermarkPosition.BottomRight.ToString(),
            ["Opacity"] = "50",
            ["FontSize"] = "24",
            ["FontColor"] = "#FFFFFF",
            ["Quality"] = "90"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/watermark/text", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("文本水印添加成功");
        apiResponse.Data.Should().NotBeNull();
    }
    
    [Fact]
    public async Task AddTextWatermark_WithEmptyText_ShouldReturnBadRequest()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 400, 300);
        var additionalFields = new Dictionary<string, string>
        {
            ["Text"] = "",
            ["Position"] = WatermarkPosition.BottomRight.ToString(),
            ["Opacity"] = "50",
            ["FontSize"] = "24",
            ["FontColor"] = "#FFFFFF",
            ["Quality"] = "90"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/watermark/text", content);
        
        // Assert
        AssertErrorResponse(response, HttpStatusCode.BadRequest);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeFalse();
        apiResponse.Message.Should().Be("水印文本不能为空");
    }
    
    [Fact]
    public async Task AddTextWatermark_WithInvalidFile_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidFile = WebTestHelper.CreateInvalidTestFile("test.txt");
        var additionalFields = new Dictionary<string, string>
        {
            ["Text"] = "Test Watermark",
            ["Position"] = WatermarkPosition.BottomRight.ToString(),
            ["Opacity"] = "50",
            ["FontSize"] = "24",
            ["FontColor"] = "#FFFFFF",
            ["Quality"] = "90"
        };
        
        var content = CreateMultipartContent(invalidFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/watermark/text", content);
        
        // Assert
        AssertErrorResponse(response, HttpStatusCode.BadRequest);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeFalse();
        apiResponse.Message.Should().Be("图片格式不支持");
    }
    
    [Theory]
    [InlineData(WatermarkPosition.TopLeft)]
    [InlineData(WatermarkPosition.TopCenter)]
    [InlineData(WatermarkPosition.TopRight)]
    [InlineData(WatermarkPosition.MiddleLeft)]
    [InlineData(WatermarkPosition.Center)]
    [InlineData(WatermarkPosition.MiddleRight)]
    [InlineData(WatermarkPosition.BottomLeft)]
    [InlineData(WatermarkPosition.BottomCenter)]
    [InlineData(WatermarkPosition.BottomRight)]
    public async Task AddTextWatermark_WithDifferentPositions_ShouldReturnSuccess(WatermarkPosition position)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 400, 300);
        var additionalFields = new Dictionary<string, string>
        {
            ["Text"] = "Test Watermark",
            ["Position"] = position.ToString(),
            ["Opacity"] = "50",
            ["FontSize"] = "24",
            ["FontColor"] = "#FFFFFF",
            ["Quality"] = "90"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/watermark/text", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
    
    [Theory]
    [InlineData(10, 12, "#000000")]
    [InlineData(24, 24, "#FFFFFF")]
    [InlineData(48, 36, "#FF0000")]
    [InlineData(72, 48, "#00FF00")]
    public async Task AddTextWatermark_WithDifferentFontSettings_ShouldReturnSuccess(int opacity, int fontSize, string fontColor)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 400, 300);
        var additionalFields = new Dictionary<string, string>
        {
            ["Text"] = "Test Watermark",
            ["Position"] = WatermarkPosition.BottomRight.ToString(),
            ["Opacity"] = opacity.ToString(),
            ["FontSize"] = fontSize.ToString(),
            ["FontColor"] = fontColor,
            ["Quality"] = "90"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/watermark/text", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
    
    [Fact]
    public async Task AddImageWatermark_WithValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 400, 300);
        var watermarkFile = WebTestHelper.CreateTestFormFile("watermark.png", 100, 50);
        
        var content = new MultipartFormDataContent();
        
        // 添加主图片文件
        var fileContent = new StreamContent(testFile.OpenReadStream());
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(testFile.ContentType);
        content.Add(fileContent, "file", testFile.FileName);
        
        // 添加水印文件
        var watermarkContent = new StreamContent(watermarkFile.OpenReadStream());
        watermarkContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(watermarkFile.ContentType);
        content.Add(watermarkContent, "watermarkFile", watermarkFile.FileName);
        
        // 添加其他参数
        content.Add(new StringContent(WatermarkPosition.BottomRight.ToString()), "Position");
        content.Add(new StringContent("50"), "Opacity");
        content.Add(new StringContent("1.0"), "Scale");
        content.Add(new StringContent("90"), "Quality");
        
        // Act
        var response = await Client.PostAsync("/api/watermark/image", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("图片水印添加成功");
        apiResponse.Data.Should().NotBeNull();
    }
    
    [Fact]
    public async Task AddImageWatermark_WithInvalidMainFile_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidFile = WebTestHelper.CreateInvalidTestFile("test.txt");
        var watermarkFile = WebTestHelper.CreateTestFormFile("watermark.png", 100, 50);
        
        var content = new MultipartFormDataContent();
        
        var fileContent = new StreamContent(invalidFile.OpenReadStream());
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(invalidFile.ContentType);
        content.Add(fileContent, "file", invalidFile.FileName);
        
        var watermarkContent = new StreamContent(watermarkFile.OpenReadStream());
        watermarkContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(watermarkFile.ContentType);
        content.Add(watermarkContent, "watermarkFile", watermarkFile.FileName);
        
        content.Add(new StringContent(WatermarkPosition.BottomRight.ToString()), "Position");
        content.Add(new StringContent("50"), "Opacity");
        content.Add(new StringContent("1.0"), "Scale");
        content.Add(new StringContent("90"), "Quality");
        
        // Act
        var response = await Client.PostAsync("/api/watermark/image", content);
        
        // Assert
        AssertErrorResponse(response, HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task AddImageWatermark_WithInvalidWatermarkFile_ShouldReturnBadRequest()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 400, 300);
        var invalidWatermark = WebTestHelper.CreateInvalidTestFile("watermark.txt");
        
        var content = new MultipartFormDataContent();
        
        var fileContent = new StreamContent(testFile.OpenReadStream());
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(testFile.ContentType);
        content.Add(fileContent, "file", testFile.FileName);
        
        var watermarkContent = new StreamContent(invalidWatermark.OpenReadStream());
        watermarkContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(invalidWatermark.ContentType);
        content.Add(watermarkContent, "watermarkFile", invalidWatermark.FileName);
        
        content.Add(new StringContent(WatermarkPosition.BottomRight.ToString()), "Position");
        content.Add(new StringContent("50"), "Opacity");
        content.Add(new StringContent("1.0"), "Scale");
        content.Add(new StringContent("90"), "Quality");
        
        // Act
        var response = await Client.PostAsync("/api/watermark/image", content);
        
        // Assert
        AssertErrorResponse(response, HttpStatusCode.BadRequest);
    }
}
