using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ImageGlider.WebApi.Model;
using ImageGlider.WebApi.Tests.TestHelpers;

namespace ImageGlider.WebApi.Tests.Integration;

/// <summary>
/// 图片颜色调整端点集成测试
/// </summary>
public class ImageColorEndpointsTests : WebApiTestBase
{
    public ImageColorEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task AdjustColor_WithValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 300, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["Brightness"] = "10",
            ["Contrast"] = "5",
            ["Saturation"] = "15",
            ["Hue"] = "0",
            ["Gamma"] = "1.0",
            ["Quality"] = "90"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/color/adjust", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("颜色调整成功");
        apiResponse.Data.Should().NotBeNull();
    }
    
    [Fact]
    public async Task AdjustColor_WithNoAdjustments_ShouldReturnSuccess()
    {
        // Arrange - 所有参数都为默认值（不调整）
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 300, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["Brightness"] = "0",
            ["Contrast"] = "0",
            ["Saturation"] = "0",
            ["Hue"] = "0",
            ["Gamma"] = "1.0",
            ["Quality"] = "90"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/color/adjust", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("颜色调整成功");
    }
    
    [Fact]
    public async Task AdjustColor_WithInvalidFile_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidFile = WebTestHelper.CreateInvalidTestFile("test.txt");
        var additionalFields = new Dictionary<string, string>
        {
            ["Brightness"] = "10",
            ["Contrast"] = "5",
            ["Saturation"] = "15",
            ["Hue"] = "0",
            ["Gamma"] = "1.0",
            ["Quality"] = "90"
        };
        
        var content = CreateMultipartContent(invalidFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/color/adjust", content);
        
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
    [InlineData(10, 0, 0, 0, 1.0f)]      // 轻微的亮度调整
    [InlineData(0, 10, 0, 0, 1.0f)]      // 轻微的对比度调整
    [InlineData(0, 0, 10, 0, 1.0f)]      // 轻微的饱和度调整
    [InlineData(0, 0, 0, 15, 1.0f)]      // 轻微的色相调整
    [InlineData(0, 0, 0, 0, 1.1f)]       // 轻微的伽马调整
    [InlineData(5, 5, 5, 10, 1.05f)]     // 组合调整
    public async Task AdjustColor_WithModerateValues_ShouldReturnSuccess(float brightness, float contrast, float saturation, float hue, float gamma)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 300, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["Brightness"] = brightness.ToString(),
            ["Contrast"] = contrast.ToString(),
            ["Saturation"] = saturation.ToString(),
            ["Hue"] = hue.ToString(),
            ["Gamma"] = gamma.ToString(),
            ["Quality"] = "90"
        };

        var content = CreateMultipartContent(testFile, additionalFields);

        // Act
        var response = await Client.PostAsync("/api/color/adjust", content);

        // Assert
        AssertSuccessResponse(response);
    }
    
    [Theory]
    [InlineData(0, 0, 0, -180, 1.0f)]    // 最小色相
    [InlineData(0, 0, 0, 180, 1.0f)]     // 最大色相
    [InlineData(0, 0, 0, 0, 0.1f)]       // 最小伽马值
    [InlineData(0, 0, 0, 0, 3.0f)]       // 最大伽马值
    public async Task AdjustColor_WithHueAndGammaBoundaryValues_ShouldReturnSuccess(float brightness, float contrast, float saturation, float hue, float gamma)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 300, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["Brightness"] = brightness.ToString(),
            ["Contrast"] = contrast.ToString(),
            ["Saturation"] = saturation.ToString(),
            ["Hue"] = hue.ToString(),
            ["Gamma"] = gamma.ToString(),
            ["Quality"] = "90"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/color/adjust", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
    
    [Theory]
    [InlineData("jpg")]
    [InlineData("png")]
    [InlineData("jpeg")]
    [InlineData("bmp")]
    public async Task AdjustColor_WithDifferentFormats_ShouldReturnSuccess(string format)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile($"test.{format}", 300, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["Brightness"] = "20",
            ["Contrast"] = "10",
            ["Saturation"] = "15",
            ["Hue"] = "30",
            ["Gamma"] = "1.2",
            ["Quality"] = "85"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/color/adjust", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task AdjustColor_WithDifferentQuality_ShouldReturnSuccess(int quality)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 300, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["Brightness"] = "10",
            ["Contrast"] = "5",
            ["Saturation"] = "15",
            ["Hue"] = "0",
            ["Gamma"] = "1.0",
            ["Quality"] = quality.ToString()
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/color/adjust", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
    
    [Fact]
    public async Task AdjustColor_WithLargeImage_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateLargeTestFormFile("large_test.jpg", 1500, 1000);
        var additionalFields = new Dictionary<string, string>
        {
            ["Brightness"] = "15",
            ["Contrast"] = "10",
            ["Saturation"] = "20",
            ["Hue"] = "45",
            ["Gamma"] = "1.1",
            ["Quality"] = "85"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/color/adjust", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
    
    [Fact]
    public async Task AdjustColor_WithMissingParameters_ShouldUseDefaults()
    {
        // Arrange - 只提供文件，其他参数使用默认值
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 300, 200);
        var content = CreateMultipartContent(testFile);
        
        // Act
        var response = await Client.PostAsync("/api/color/adjust", content);
        
        // Assert
        // 应该使用默认值并成功处理
        AssertSuccessResponse(response);
    }
}
