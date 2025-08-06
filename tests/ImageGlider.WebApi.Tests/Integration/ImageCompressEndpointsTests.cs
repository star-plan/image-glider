using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ImageGlider.WebApi.Model;
using ImageGlider.WebApi.Tests.TestHelpers;

namespace ImageGlider.WebApi.Tests.Integration;

/// <summary>
/// 图片压缩端点集成测试
/// </summary>
public class ImageCompressEndpointsTests : WebApiTestBase
{
    public ImageCompressEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task CompressImage_WithValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateLargeTestFormFile("large_test.jpg", 1000, 1000);
        var additionalFields = new Dictionary<string, string>
        {
            ["Quality"] = "60",
            ["MaxFileSize"] = "500000" // 500KB
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/compress", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("压缩成功");
        apiResponse.Data.Should().NotBeNull();
    }
    
    [Fact]
    public async Task CompressImage_WithInvalidFile_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidFile = WebTestHelper.CreateInvalidTestFile("document.txt");
        var additionalFields = new Dictionary<string, string>
        {
            ["Quality"] = "60",
            ["MaxFileSize"] = "500000"
        };
        
        var content = CreateMultipartContent(invalidFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/compress", content);
        
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
    [InlineData(0)] // 无效质量
    [InlineData(101)] // 超出范围质量
    [InlineData(-10)] // 负数质量
    public async Task CompressImage_WithInvalidQuality_ShouldReturnBadRequest(int quality)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 200, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["Quality"] = quality.ToString(),
            ["MaxFileSize"] = "500000"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/compress", content);
        
        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
    }
    
    [Fact]
    public async Task CompressImage_WithMissingParameters_ShouldReturnBadRequest()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 200, 200);
        var content = CreateMultipartContent(testFile); // 缺少必要参数
        
        // Act
        var response = await Client.PostAsync("/api/compress", content);
        
        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity);
    }
    
    [Theory]
    [InlineData("jpg")]
    [InlineData("jpeg")]
    [InlineData("png")]
    public async Task CompressImage_WithDifferentFormats_ShouldReturnSuccess(string format)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile($"test.{format}", 300, 300);
        var additionalFields = new Dictionary<string, string>
        {
            ["Quality"] = "70",
            ["MaxFileSize"] = "1000000"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/compress", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
    
    [Fact]
    public async Task CompressImage_WithHighQuality_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 400, 400);
        var additionalFields = new Dictionary<string, string>
        {
            ["Quality"] = "95", // 高质量
            ["MaxFileSize"] = "2000000"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/compress", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
    
    [Fact]
    public async Task CompressImage_WithLowQuality_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 400, 400);
        var additionalFields = new Dictionary<string, string>
        {
            ["Quality"] = "30", // 低质量
            ["MaxFileSize"] = "100000"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/compress", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
    
    [Fact]
    public async Task CompressImage_WithVerySmallMaxFileSize_ShouldStillProcess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 200, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["Quality"] = "50",
            ["MaxFileSize"] = "1000" // 很小的文件大小限制
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/compress", content);
        
        // Assert
        // 即使文件大小限制很小，API 也应该尝试处理
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task CompressImage_WithLargeImage_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateLargeTestFormFile("large_test.jpg", 2000, 1500);
        var additionalFields = new Dictionary<string, string>
        {
            ["Quality"] = "75",
            ["MaxFileSize"] = "1000000" // 1MB
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/compress", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
    
    [Fact]
    public async Task CompressImage_ConcurrentRequests_ShouldHandleMultipleRequests()
    {
        // Arrange
        var tasks = new List<Task<HttpResponseMessage>>();
        
        for (int i = 0; i < 5; i++)
        {
            var testFile = WebTestHelper.CreateTestFormFile($"test_{i}.jpg", 300, 300);
            var additionalFields = new Dictionary<string, string>
            {
                ["Quality"] = "70",
                ["MaxFileSize"] = "500000"
            };
            
            var content = CreateMultipartContent(testFile, additionalFields);
            tasks.Add(Client.PostAsync("/api/compress", content));
        }
        
        // Act
        var responses = await Task.WhenAll(tasks);
        
        // Assert
        foreach (var response in responses)
        {
            AssertSuccessResponse(response);
        }
    }
}