using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ImageGlider.WebApi.Model;
using ImageGlider.WebApi.Tests.TestHelpers;

namespace ImageGlider.WebApi.Tests.Integration;

/// <summary>
/// 图片裁剪端点集成测试
/// </summary>
public class ImageCropEndpointsTests : WebApiTestBase
{
    public ImageCropEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task CropImage_WithValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 400, 400);
        var additionalFields = new Dictionary<string, string>
        {
            ["X"] = "50",
            ["Y"] = "50",
            ["Width"] = "200",
            ["Height"] = "200",
            ["Quality"] = "90"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/crop", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("裁剪成功");
        apiResponse.Data.Should().NotBeNull();
    }
    
    [Fact]
    public async Task CropImageCenter_WithValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 400, 400);
        var additionalFields = new Dictionary<string, string>
        {
            ["Width"] = "200",
            ["Height"] = "200",
            ["Quality"] = "85"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/crop/center", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("中心裁剪成功");
        apiResponse.Data.Should().NotBeNull();
    }
    
    [Fact]
    public async Task CropImageByPercent_WithValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 400, 400);
        var additionalFields = new Dictionary<string, string>
        {
            ["XPercent"] = "25",
            ["YPercent"] = "25",
            ["WidthPercent"] = "50",
            ["HeightPercent"] = "50",
            ["Quality"] = "80"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/crop/percent", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("百分比裁剪成功");
        apiResponse.Data.Should().NotBeNull();
    }
    
    [Fact]
    public async Task CropImage_WithInvalidFile_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidFile = WebTestHelper.CreateInvalidTestFile("test.txt");
        var additionalFields = new Dictionary<string, string>
        {
            ["X"] = "50",
            ["Y"] = "50",
            ["Width"] = "200",
            ["Height"] = "200",
            ["Quality"] = "90"
        };
        
        var content = CreateMultipartContent(invalidFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/crop", content);
        
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
    [InlineData(-10, 50, 200, 200)] // 负数X坐标
    [InlineData(50, -10, 200, 200)] // 负数Y坐标
    [InlineData(50, 50, 0, 200)]    // 零宽度
    [InlineData(50, 50, 200, 0)]    // 零高度
    public async Task CropImage_WithInvalidCoordinates_ShouldHandleGracefully(int x, int y, int width, int height)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 400, 400);
        var additionalFields = new Dictionary<string, string>
        {
            ["X"] = x.ToString(),
            ["Y"] = y.ToString(),
            ["Width"] = width.ToString(),
            ["Height"] = height.ToString(),
            ["Quality"] = "90"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/crop", content);
        
        // Assert
        // 应该返回错误或者处理失败
        response.IsSuccessStatusCode.Should().BeFalse();
    }
    
    [Theory]
    [InlineData(0, 0, 50, 50)]      // 左上角裁剪
    [InlineData(50, 50, 25, 25)]    // 中心区域裁剪
    [InlineData(10, 10, 80, 80)]    // 大部分区域裁剪
    public async Task CropImageByPercent_WithValidPercentages_ShouldReturnSuccess(float xPercent, float yPercent, float widthPercent, float heightPercent)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 400, 400);
        var additionalFields = new Dictionary<string, string>
        {
            ["XPercent"] = xPercent.ToString(),
            ["YPercent"] = yPercent.ToString(),
            ["WidthPercent"] = widthPercent.ToString(),
            ["HeightPercent"] = heightPercent.ToString(),
            ["Quality"] = "85"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/crop/percent", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
    
    [Fact]
    public async Task CropImageCenter_WithMissingParameters_ShouldReturnBadRequest()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 400, 400);
        var content = CreateMultipartContent(testFile); // 缺少必要参数
        
        // Act
        var response = await Client.PostAsync("/api/crop/center", content);
        
        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity);
    }
    
    [Theory]
    [InlineData("jpg")]
    [InlineData("png")]
    [InlineData("jpeg")]
    public async Task CropImage_WithDifferentFormats_ShouldReturnSuccess(string format)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile($"test.{format}", 400, 400);
        var additionalFields = new Dictionary<string, string>
        {
            ["X"] = "50",
            ["Y"] = "50",
            ["Width"] = "200",
            ["Height"] = "200",
            ["Quality"] = "90"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/crop", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
}
