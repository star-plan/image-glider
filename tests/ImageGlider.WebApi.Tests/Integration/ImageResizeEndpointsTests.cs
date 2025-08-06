using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ImageGlider.WebApi.Model;
using ImageGlider.WebApi.Tests.TestHelpers;
using ImageGlider.Enums;

namespace ImageGlider.WebApi.Tests.Integration;

/// <summary>
/// 图片调整端点集成测试
/// </summary>
public class ImageResizeEndpointsTests : WebApiTestBase
{
    public ImageResizeEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task ResizeImage_WithValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 200, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["Width"] = "100",
            ["Height"] = "100",
            ["ResizeMode"] = ResizeMode.Stretch.ToString(),
            ["Quality"] = "85"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/resize", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("尺寸调整成功");
        apiResponse.Data.Should().NotBeNull();
    }
    
    [Fact]
    public async Task ResizeImage_WithInvalidFile_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidFile = WebTestHelper.CreateInvalidTestFile("test.txt");
        var additionalFields = new Dictionary<string, string>
        {
            ["Width"] = "100",
            ["Height"] = "100",
            ["ResizeMode"] = ResizeMode.Stretch.ToString(),
            ["Quality"] = "85"
        };
        
        var content = CreateMultipartContent(invalidFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/resize", content);
        
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
    public async Task ResizeImage_WithMissingParameters_ShouldReturnBadRequest()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 200, 200);
        var content = CreateMultipartContent(testFile); // 缺少必要参数
        
        // Act
        var response = await Client.PostAsync("/api/resize", content);
        
        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task GenerateThumbnail_WithValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateLargeTestFormFile("large_test.jpg", 800, 600);
        var additionalFields = new Dictionary<string, string>
        {
            ["MaxSize"] = "150",
            ["Quality"] = "80"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/resize/thumbnail", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("缩略图生成成功");
        apiResponse.Data.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GenerateThumbnail_WithInvalidFile_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidFile = WebTestHelper.CreateInvalidTestFile("document.pdf");
        var additionalFields = new Dictionary<string, string>
        {
            ["MaxSize"] = "150",
            ["Quality"] = "80"
        };
        
        var content = CreateMultipartContent(invalidFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/resize/thumbnail", content);
        
        // Assert
        AssertErrorResponse(response, HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData(0, 100)] // 无效宽度
    [InlineData(100, 0)] // 无效高度
    [InlineData(-10, 100)] // 负数宽度
    [InlineData(100, -10)] // 负数高度
    public async Task ResizeImage_WithInvalidDimensions_ShouldReturnBadRequest(int width, int height)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 200, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["Width"] = width.ToString(),
            ["Height"] = height.ToString(),
            ["ResizeMode"] = ResizeMode.Stretch.ToString(),
            ["Quality"] = "85"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/resize", content);
        
        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
    }
    
    [Theory]
    [InlineData("jpg")]
    [InlineData("png")]
    [InlineData("jpeg")]
    public async Task ResizeImage_WithDifferentFormats_ShouldReturnSuccess(string format)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile($"test.{format}", 200, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["Width"] = "100",
            ["Height"] = "100",
            ["ResizeMode"] = ResizeMode.Stretch.ToString(),
            ["Quality"] = "85"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/resize", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
}