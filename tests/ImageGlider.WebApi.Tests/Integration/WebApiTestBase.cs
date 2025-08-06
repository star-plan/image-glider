using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using ImageGlider.WebApi.Tests.TestHelpers;

namespace ImageGlider.WebApi.Tests.Integration;

/// <summary>
/// Web API 集成测试基类
/// </summary>
public class WebApiTestBase : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly string TempDirectory;
    
    public WebApiTestBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // 配置测试服务
                services.AddLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Warning);
                });
            });
        });
        
        Client = Factory.CreateClient();
        TempDirectory = WebTestHelper.CreateTempDirectory();
    }
    
    /// <summary>
    /// 创建多部分表单内容
    /// </summary>
    /// <param name="formFile">表单文件</param>
    /// <param name="additionalFields">额外字段</param>
    /// <returns>MultipartFormDataContent 对象</returns>
    protected MultipartFormDataContent CreateMultipartContent(IFormFile formFile, Dictionary<string, string>? additionalFields = null)
    {
        var content = new MultipartFormDataContent();
        
        // 添加文件
        var fileContent = new StreamContent(formFile.OpenReadStream());
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(formFile.ContentType);
        content.Add(fileContent, "file", formFile.FileName);
        
        // 添加额外字段
        if (additionalFields != null)
        {
            foreach (var field in additionalFields)
            {
                content.Add(new StringContent(field.Value), field.Key);
            }
        }
        
        return content;
    }
    
    /// <summary>
    /// 验证响应是否成功
    /// </summary>
    /// <param name="response">HTTP 响应</param>
    protected static void AssertSuccessResponse(HttpResponseMessage response)
    {
        response.IsSuccessStatusCode.Should().BeTrue(
            $"Expected success status code, but got {response.StatusCode}. Content: {response.Content.ReadAsStringAsync().Result}");
    }
    
    /// <summary>
    /// 验证响应是否为错误状态
    /// </summary>
    /// <param name="response">HTTP 响应</param>
    /// <param name="expectedStatusCode">期望的状态码</param>
    protected static void AssertErrorResponse(HttpResponseMessage response, System.Net.HttpStatusCode expectedStatusCode)
    {
        response.StatusCode.Should().Be(expectedStatusCode);
    }
    
    public virtual void Dispose()
    {
        Client?.Dispose();
        WebTestHelper.CleanupDirectory(TempDirectory);
        GC.SuppressFinalize(this);
    }
}