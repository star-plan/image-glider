using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ImageGlider.WebApi.Model;
using ImageGlider.WebApi.Tests.TestHelpers;

namespace ImageGlider.WebApi.Tests.Integration;

/// <summary>
/// 图片信息端点集成测试
/// </summary>
public class ImageInfoEndpointsTests : WebApiTestBase
{
    public ImageInfoEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task GetImageInfo_WithValidJpgImage_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 300, 200);
        var content = CreateMultipartContent(testFile);
        
        // Act
        var response = await Client.PostAsync("/api/info", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("图片信息获取成功");
        apiResponse.Data.Should().NotBeNull();
        
        // 验证返回的图片信息包含基本属性
        var imageInfo = apiResponse.Data as JsonElement?;
        imageInfo.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetImageInfo_WithValidPngImage_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.png", 400, 300);
        var content = CreateMultipartContent(testFile);
        
        // Act
        var response = await Client.PostAsync("/api/info", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("图片信息获取成功");
        apiResponse.Data.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetImageInfo_WithInvalidFile_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidFile = WebTestHelper.CreateInvalidTestFile("test.txt");
        var content = CreateMultipartContent(invalidFile);
        
        // Act
        var response = await Client.PostAsync("/api/info", content);
        
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
    public async Task GetImageInfo_WithNoFile_ShouldReturnBadRequest()
    {
        // Arrange
        var content = new MultipartFormDataContent();
        
        // Act
        var response = await Client.PostAsync("/api/info", content);
        
        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity);
    }
    
    [Theory]
    [InlineData("jpg", 100, 100)]
    [InlineData("png", 200, 150)]
    [InlineData("jpeg", 500, 300)]
    [InlineData("bmp", 50, 50)]
    public async Task GetImageInfo_WithDifferentFormatsAndSizes_ShouldReturnSuccess(string format, int width, int height)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile($"test.{format}", width, height);
        var content = CreateMultipartContent(testFile);
        
        // Act
        var response = await Client.PostAsync("/api/info", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetImageInfo_WithLargeImage_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateLargeTestFormFile("large_test.jpg", 2000, 1500);
        var content = CreateMultipartContent(testFile);
        
        // Act
        var response = await Client.PostAsync("/api/info", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("图片信息获取成功");
    }
    
    [Fact]
    public async Task GetImageInfo_WithSmallImage_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("small_test.jpg", 10, 10);
        var content = CreateMultipartContent(testFile);
        
        // Act
        var response = await Client.PostAsync("/api/info", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetImageInfo_WithCorruptedFile_ShouldReturnBadRequest()
    {
        // Arrange - 创建一个看起来像图片但实际损坏的文件
        var corruptedContent = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 }; // JPEG header but incomplete
        var corruptedFile = WebTestHelper.CreateFormFileFromBytes("corrupted.jpg", corruptedContent, "image/jpeg");
        var content = CreateMultipartContent(corruptedFile);
        
        // Act
        var response = await Client.PostAsync("/api/info", content);
        
        // Assert
        // 损坏的文件应该返回错误或者处理失败
        response.IsSuccessStatusCode.Should().BeFalse();
    }
}
