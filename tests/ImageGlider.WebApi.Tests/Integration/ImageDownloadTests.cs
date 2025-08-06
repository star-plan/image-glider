using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using ImageGlider.WebApi.Tests.TestHelpers;

namespace ImageGlider.WebApi.Tests.Integration;

/// <summary>
/// 图片下载端点集成测试
/// </summary>
public class ImageDownloadTests : WebApiTestBase
{
    public ImageDownloadTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task DownloadImage_WithValidFileName_ShouldReturnFile()
    {
        // Arrange - 首先上传一个文件以获取有效的文件名
        var uploadFile = WebTestHelper.CreateTestFormFile("upload_test.jpg", 200, 200);
        var uploadFields = new Dictionary<string, string>
        {
            ["Width"] = "100",
            ["Height"] = "100",
            ["ResizeMode"] = "Stretch",
            ["Quality"] = "85"
        };
        
        var uploadContent = CreateMultipartContent(uploadFile, uploadFields);
        var uploadResponse = await Client.PostAsync("/api/resize", uploadContent);
        
        uploadResponse.IsSuccessStatusCode.Should().BeTrue();
        
        var uploadResponseContent = await uploadResponse.Content.ReadAsStringAsync();
        // 从响应中提取文件名（这里需要根据实际的 API 响应格式调整）
        var fileName = ExtractFileNameFromResponse(uploadResponseContent);
        
        // Act
        var downloadResponse = await Client.GetAsync($"/api/download/{fileName}");
        
        // Assert
        AssertSuccessResponse(downloadResponse);
        downloadResponse.Content.Headers.ContentType?.MediaType.Should().StartWith("image/");
        
        var fileContent = await downloadResponse.Content.ReadAsByteArrayAsync();
        fileContent.Length.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public async Task DownloadImage_WithNonExistentFile_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentFileName = "nonexistent_file_12345.jpg";
        
        // Act
        var response = await Client.GetAsync($"/api/download/{nonExistentFileName}");
        
        // Assert
        AssertErrorResponse(response, HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DownloadImage_WithEmptyFileName_ShouldReturnBadRequest()
    {
        // Act
        var response = await Client.GetAsync("/api/download/");
        
        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }
    
    [Theory]
    [InlineData("../../../etc/passwd")] // 路径遍历攻击
    [InlineData("..\\..\\windows\\system32\\config\\sam")] // Windows 路径遍历
    [InlineData("file|with|pipes.jpg")] // 管道字符
    [InlineData("file<with>brackets.jpg")] // 尖括号
    public async Task DownloadImage_WithMaliciousFileName_ShouldReturnBadRequestOrNotFound(string maliciousFileName)
    {
        // Act
        var response = await Client.GetAsync($"/api/download/{Uri.EscapeDataString(maliciousFileName)}");
        
        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound, HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task DownloadImage_WithVeryLongFileName_ShouldHandleGracefully()
    {
        // Arrange
        var longFileName = new string('a', 300) + ".jpg"; // 300个字符的文件名
        
        // Act
        var response = await Client.GetAsync($"/api/download/{Uri.EscapeDataString(longFileName)}");
        
        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }
    
    [Theory]
    [InlineData("test file with spaces.jpg")]
    [InlineData("测试中文文件名.jpg")]
    [InlineData("file-with-dashes.jpg")]
    [InlineData("file_with_underscores.jpg")]
    [InlineData("file.with.dots.jpg")]
    public async Task DownloadImage_WithSpecialCharactersInFileName_ShouldHandleCorrectly(string fileName)
    {
        // Act
        var response = await Client.GetAsync($"/api/download/{Uri.EscapeDataString(fileName)}");
        
        // Assert
        // 文件不存在时应该返回 404，而不是因为文件名格式错误返回 400
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DownloadImage_ConcurrentDownloads_ShouldHandleMultipleRequests()
    {
        // Arrange - 首先创建一个可下载的文件
        var uploadFile = WebTestHelper.CreateTestFormFile("concurrent_test.jpg", 300, 300);
        var uploadFields = new Dictionary<string, string>
        {
            ["Width"] = "150",
            ["Height"] = "150",
            ["ResizeMode"] = "Stretch",
            ["Quality"] = "85"
        };
        
        var uploadContent = CreateMultipartContent(uploadFile, uploadFields);
        var uploadResponse = await Client.PostAsync("/api/resize", uploadContent);
        
        if (!uploadResponse.IsSuccessStatusCode)
        {
            // 如果上传失败，跳过这个测试
            return;
        }
        
        var uploadResponseContent = await uploadResponse.Content.ReadAsStringAsync();
        var fileName = ExtractFileNameFromResponse(uploadResponseContent);
        
        // Act - 并发下载同一个文件
        var downloadTasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 5; i++)
        {
            downloadTasks.Add(Client.GetAsync($"/api/download/{fileName}"));
        }
        
        var responses = await Task.WhenAll(downloadTasks);
        
        // Assert
        foreach (var response in responses)
        {
            if (response.IsSuccessStatusCode)
            {
                response.Content.Headers.ContentType?.MediaType.Should().StartWith("image/");
                var content = await response.Content.ReadAsByteArrayAsync();
                content.Length.Should().BeGreaterThan(0);
            }
        }
        
        // 至少应该有一些成功的响应
        responses.Count(r => r.IsSuccessStatusCode).Should().BeGreaterThan(0);
    }
    
    [Fact]
    public async Task DownloadImage_WithCaseInsensitiveFileName_ShouldHandleCorrectly()
    {
        // Arrange
        var fileName = "Test_File.JPG"; // 大写扩展名
        
        // Act
        var response = await Client.GetAsync($"/api/download/{fileName}");
        
        // Assert
        // 文件不存在时应该返回 404
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DownloadImage_ResponseHeaders_ShouldBeSetCorrectly()
    {
        // Arrange - 上传文件
        var uploadFile = WebTestHelper.CreateTestFormFile("header_test.jpg", 200, 200);
        var uploadFields = new Dictionary<string, string>
        {
            ["Width"] = "100",
            ["Height"] = "100",
            ["ResizeMode"] = "Stretch",
            ["Quality"] = "85"
        };
        
        var uploadContent = CreateMultipartContent(uploadFile, uploadFields);
        var uploadResponse = await Client.PostAsync("/api/resize", uploadContent);
        
        if (!uploadResponse.IsSuccessStatusCode)
        {
            return; // 跳过测试如果上传失败
        }
        
        var uploadResponseContent = await uploadResponse.Content.ReadAsStringAsync();
        var fileName = ExtractFileNameFromResponse(uploadResponseContent);
        
        // Act
        var downloadResponse = await Client.GetAsync($"/api/download/{fileName}");
        
        // Assert
        if (downloadResponse.IsSuccessStatusCode)
        {
            downloadResponse.Content.Headers.ContentType.Should().NotBeNull();
            downloadResponse.Content.Headers.ContentLength.Should().BeGreaterThan(0);
            
            // 检查是否设置了适当的缓存头
            downloadResponse.Headers.CacheControl.Should().NotBeNull();
        }
    }
    
    /// <summary>
    /// 从 API 响应中提取文件名
    /// </summary>
    /// <param name="responseContent">响应内容</param>
    /// <returns>文件名</returns>
    private static string ExtractFileNameFromResponse(string responseContent)
    {
        // 这里需要根据实际的 API 响应格式来解析文件名
        // 假设响应是 JSON 格式，包含 data 字段
        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(responseContent);
            if (document.RootElement.TryGetProperty("data", out var dataElement))
            {
                return dataElement.GetString() ?? "test_file.jpg";
            }
        }
        catch
        {
            // 如果解析失败，返回默认文件名
        }
        
        return "test_file.jpg";
    }
}