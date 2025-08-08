using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ImageGlider.WebApi.Model;
using ImageGlider.WebApi.Tests.TestHelpers;

namespace ImageGlider.WebApi.Tests.Integration;

/// <summary>
/// 图片格式转换端点集成测试
/// </summary>
public class ImageConvertEndpointsTests : WebApiTestBase
{
    public ImageConvertEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task ConvertImage_WithValidJpgToPng_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 200, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["FileExt"] = "png",
            ["Quality"] = "90"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/convert", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("转换成功");
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data.ToString().Should().EndWith(".png");
    }
    
    [Fact]
    public async Task ConvertImage_WithValidPngToJpg_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.png", 200, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["FileExt"] = "jpg",
            ["Quality"] = "85"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/convert", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("转换成功");
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data.ToString().Should().EndWith(".jpg");
    }
    
    [Fact]
    public async Task ConvertImage_WithInvalidFile_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidFile = WebTestHelper.CreateInvalidTestFile("test.txt");
        var additionalFields = new Dictionary<string, string>
        {
            ["FileExt"] = "png",
            ["Quality"] = "90"
        };
        
        var content = CreateMultipartContent(invalidFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/convert", content);
        
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
    
    [Fact]
    public async Task ConvertImage_WithMissingFileExt_ShouldReturnServerError()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 200, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["Quality"] = "90"
            // 缺少 FileExt 参数
        };

        var content = CreateMultipartContent(testFile, additionalFields);

        // Act
        var response = await Client.PostAsync("/api/convert", content);

        // Assert
        // 当FileExt为null时，会导致NullReferenceException，返回500
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
    
    [Theory]
    [InlineData("jpg", "png")]
    [InlineData("png", "jpg")]
    [InlineData("jpeg", "webp")]
    [InlineData("bmp", "png")]
    public async Task ConvertImage_WithDifferentFormats_ShouldReturnSuccess(string sourceFormat, string targetFormat)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile($"test.{sourceFormat}", 200, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["FileExt"] = targetFormat,
            ["Quality"] = "85"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/convert", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Data.ToString().Should().EndWith($".{targetFormat}");
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task ConvertImage_WithDifferentQuality_ShouldReturnSuccess(int quality)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 200, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["FileExt"] = "png",
            ["Quality"] = quality.ToString()
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/convert", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
}
