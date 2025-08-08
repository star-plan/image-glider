using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ImageGlider.WebApi.Model;
using ImageGlider.WebApi.Tests.TestHelpers;

namespace ImageGlider.WebApi.Tests.Integration;

/// <summary>
/// 图片元数据处理端点集成测试
/// </summary>
public class ImageMetadataEndpointsTests : WebApiTestBase
{
    public ImageMetadataEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task StripMetadata_WithValidImageAndStripAll_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 300, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["StripAll"] = "true",
            ["Quality"] = "90"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/metadata/strip", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("元数据清理成功");
        apiResponse.Data.Should().NotBeNull();
    }
    
    [Fact]
    public async Task StripMetadata_WithValidImageAndStripExifOnly_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 300, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["StripAll"] = "false",
            ["StripExif"] = "true",
            ["StripIcc"] = "false",
            ["StripXmp"] = "false",
            ["Quality"] = "85"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/metadata/strip", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("元数据清理成功");
        apiResponse.Data.Should().NotBeNull();
    }
    
    [Fact]
    public async Task StripMetadata_WithValidImageAndSelectiveStripping_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 300, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["StripAll"] = "false",
            ["StripExif"] = "true",
            ["StripIcc"] = "true",
            ["StripXmp"] = "false",
            ["Quality"] = "80"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/metadata/strip", content);
        
        // Assert
        AssertSuccessResponse(response);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        apiResponse.Should().NotBeNull();
        apiResponse!.Successful.Should().BeTrue();
        apiResponse.Message.Should().Be("元数据清理成功");
    }
    
    [Fact]
    public async Task StripMetadata_WithInvalidFile_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidFile = WebTestHelper.CreateInvalidTestFile("test.txt");
        var additionalFields = new Dictionary<string, string>
        {
            ["StripAll"] = "true",
            ["Quality"] = "90"
        };
        
        var content = CreateMultipartContent(invalidFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/metadata/strip", content);
        
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
    public async Task StripMetadata_WithNoFile_ShouldReturnBadRequest()
    {
        // Arrange
        var content = new MultipartFormDataContent();
        content.Add(new StringContent("true"), "StripAll");
        content.Add(new StringContent("90"), "Quality");
        
        // Act
        var response = await Client.PostAsync("/api/metadata/strip", content);
        
        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity);
    }
    
    [Theory]
    [InlineData("jpg")]
    [InlineData("png")]
    [InlineData("jpeg")]
    [InlineData("bmp")]
    public async Task StripMetadata_WithDifferentFormats_ShouldReturnSuccess(string format)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile($"test.{format}", 300, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["StripAll"] = "true",
            ["Quality"] = "85"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/metadata/strip", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task StripMetadata_WithDifferentQuality_ShouldReturnSuccess(int quality)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 300, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["StripAll"] = "true",
            ["Quality"] = quality.ToString()
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/metadata/strip", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
    
    [Fact]
    public async Task StripMetadata_WithLargeImage_ShouldReturnSuccess()
    {
        // Arrange
        var testFile = WebTestHelper.CreateLargeTestFormFile("large_test.jpg", 1500, 1000);
        var additionalFields = new Dictionary<string, string>
        {
            ["StripAll"] = "true",
            ["Quality"] = "85"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/metadata/strip", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
    
    [Theory]
    [InlineData(true, false, false, false)]  // 只清理所有
    [InlineData(false, true, false, false)]  // 只清理EXIF
    [InlineData(false, false, true, false)]  // 只清理ICC
    [InlineData(false, false, false, true)]  // 只清理XMP
    [InlineData(false, true, true, true)]    // 清理EXIF、ICC、XMP
    public async Task StripMetadata_WithDifferentStripOptions_ShouldReturnSuccess(bool stripAll, bool stripExif, bool stripIcc, bool stripXmp)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 300, 200);
        var additionalFields = new Dictionary<string, string>
        {
            ["StripAll"] = stripAll.ToString().ToLower(),
            ["StripExif"] = stripExif.ToString().ToLower(),
            ["StripIcc"] = stripIcc.ToString().ToLower(),
            ["StripXmp"] = stripXmp.ToString().ToLower(),
            ["Quality"] = "85"
        };
        
        var content = CreateMultipartContent(testFile, additionalFields);
        
        // Act
        var response = await Client.PostAsync("/api/metadata/strip", content);
        
        // Assert
        AssertSuccessResponse(response);
    }
}
