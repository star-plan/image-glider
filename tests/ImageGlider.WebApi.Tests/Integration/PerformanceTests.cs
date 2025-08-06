using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using ImageGlider.WebApi.Tests.TestHelpers;

namespace ImageGlider.WebApi.Tests.Integration;

/// <summary>
/// API 性能测试
/// </summary>
public class PerformanceTests : WebApiTestBase
{
    public PerformanceTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task ResizeEndpoint_UnderLoad_ShouldMaintainPerformance()
    {
        // Arrange
        const int requestCount = 10;
        const int maxAcceptableResponseTimeMs = 5000; // 5秒
        
        var tasks = new List<Task<(HttpResponseMessage Response, long ElapsedMs)>>();
        
        // Act
        for (int i = 0; i < requestCount; i++)
        {
            tasks.Add(MeasureResizeRequestTime(i));
        }
        
        var results = await Task.WhenAll(tasks);
        
        // Assert
        var successfulRequests = results.Where(r => r.Response.IsSuccessStatusCode).ToList();
        var averageResponseTime = successfulRequests.Average(r => r.ElapsedMs);
        var maxResponseTime = successfulRequests.Max(r => r.ElapsedMs);
        
        // 至少 80% 的请求应该成功
        (successfulRequests.Count / (double)requestCount).Should().BeGreaterThan(0.8);
        
        // 平均响应时间应该在可接受范围内
        averageResponseTime.Should().BeLessThan(maxAcceptableResponseTimeMs);
        
        // 最大响应时间不应该过长
        maxResponseTime.Should().BeLessThan(maxAcceptableResponseTimeMs * 2);
        
        // 清理响应
        foreach (var result in results)
        {
            result.Response.Dispose();
        }
    }
    
    [Fact]
    public async Task CompressEndpoint_WithLargeImages_ShouldCompleteInReasonableTime()
    {
        // Arrange
        const int maxAcceptableResponseTimeMs = 10000; // 10秒
        var largeImageFile = WebTestHelper.CreateTestFormFile("large_image.jpg", 2000, 2000); // 大图片
        
        var fields = new Dictionary<string, string>
        {
            ["Quality"] = "75"
        };
        
        var content = CreateMultipartContent(largeImageFile, fields);
        var stopwatch = Stopwatch.StartNew();
        
        // Act
        var response = await Client.PostAsync("/api/compress", content);
        stopwatch.Stop();
        
        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(maxAcceptableResponseTimeMs);
        
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().NotBeNullOrEmpty();
        }
        
        response.Dispose();
    }
    
    [Fact]
    public async Task ConcurrentRequests_DifferentEndpoints_ShouldHandleGracefully()
    {
        // Arrange
        const int concurrentRequests = 5;
        var tasks = new List<Task<HttpResponseMessage>>();
        
        // Act - 同时调用不同的端点
        for (int i = 0; i < concurrentRequests; i++)
        {
            var index = i;
            if (index % 3 == 0)
            {
                // 调整大小端点
                tasks.Add(CreateResizeRequest());
            }
            else if (index % 3 == 1)
            {
                // 压缩端点
                tasks.Add(CreateCompressRequest());
            }
            else
            {
                // 缩略图端点
                tasks.Add(CreateThumbnailRequest());
            }
        }
        
        var responses = await Task.WhenAll(tasks);
        
        // Assert
        var successfulResponses = responses.Where(r => r.IsSuccessStatusCode).ToList();
        
        // 至少应该有一些成功的响应
        successfulResponses.Count.Should().BeGreaterThan(0);
        
        // 没有响应应该返回服务器错误（500+）
        responses.Should().NotContain(r => (int)r.StatusCode >= 500);
        
        // 清理响应
        foreach (var response in responses)
        {
            response.Dispose();
        }
    }
    
    [Fact]
    public async Task MemoryUsage_MultipleRequests_ShouldNotCauseMemoryLeak()
    {
        // Arrange
        const int requestCount = 20;
        var initialMemory = GC.GetTotalMemory(true);
        
        // Act
        for (int i = 0; i < requestCount; i++)
        {
            var file = WebTestHelper.CreateTestFormFile($"memory_test_{i}.jpg", 500, 500);
            var fields = new Dictionary<string, string>
            {
                ["Width"] = "250",
                ["Height"] = "250",
                ["ResizeMode"] = "Stretch",
                ["Quality"] = "80"
            };
            
            var content = CreateMultipartContent(file, fields);
            var response = await Client.PostAsync("/api/resize", content);
            
            // 立即释放响应以避免累积
            response.Dispose();
            
            // 每5个请求强制垃圾回收
            if (i % 5 == 0)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
        
        // 最终垃圾回收
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        var finalMemory = GC.GetTotalMemory(false);
        
        // Assert
        // 内存增长不应该超过初始内存的 3 倍
        var memoryGrowthRatio = (double)finalMemory / initialMemory;
        memoryGrowthRatio.Should().BeLessThan(3.0, 
            $"Memory grew from {initialMemory:N0} to {finalMemory:N0} bytes (ratio: {memoryGrowthRatio:F2})");
    }
    
    [Fact]
    public async Task ResponseTime_SmallImages_ShouldBeFast()
    {
        // Arrange
        const int maxAcceptableResponseTimeMs = 2000; // 2秒
        var smallImageFile = WebTestHelper.CreateTestFormFile("small_image.jpg", 100, 100);
        
        var fields = new Dictionary<string, string>
        {
            ["Width"] = "50",
            ["Height"] = "50",
            ["ResizeMode"] = "Stretch",
            ["Quality"] = "85"
        };
        
        var content = CreateMultipartContent(smallImageFile, fields);
        var stopwatch = Stopwatch.StartNew();
        
        // Act
        var response = await Client.PostAsync("/api/resize", content);
        stopwatch.Stop();
        
        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(maxAcceptableResponseTimeMs);
        
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().NotBeNullOrEmpty();
        }
        
        response.Dispose();
    }
    
    [Theory]
    [InlineData(10)] // 10个并发请求
    [InlineData(20)] // 20个并发请求
    public async Task ConcurrentSameEndpoint_ShouldHandleLoad(int concurrentCount)
    {
        // Arrange
        var tasks = new List<Task<HttpResponseMessage>>();
        
        // Act
        for (int i = 0; i < concurrentCount; i++)
        {
            tasks.Add(CreateResizeRequest());
        }
        
        var stopwatch = Stopwatch.StartNew();
        var responses = await Task.WhenAll(tasks);
        stopwatch.Stop();
        
        // Assert
        var successfulResponses = responses.Where(r => r.IsSuccessStatusCode).ToList();
        var errorResponses = responses.Where(r => !r.IsSuccessStatusCode).ToList();
        
        // 成功率应该至少 70%
        var successRate = (double)successfulResponses.Count / concurrentCount;
        successRate.Should().BeGreaterThan(0.7, 
            $"Success rate was {successRate:P2} with {successfulResponses.Count}/{concurrentCount} successful requests");
        
        // 总时间不应该过长（不应该是串行执行）
        var averageTimePerRequest = stopwatch.ElapsedMilliseconds / (double)concurrentCount;
        averageTimePerRequest.Should().BeLessThan(5000); // 平均每个请求不超过5秒
        
        // 检查错误响应的状态码
        foreach (var errorResponse in errorResponses)
        {
            // 应该是客户端错误或服务繁忙，而不是服务器内部错误
            ((int)errorResponse.StatusCode).Should().BeOneOf(400, 429, 503);
        }
        
        // 清理响应
        foreach (var response in responses)
        {
            response.Dispose();
        }
    }
    
    /// <summary>
    /// 测量调整大小请求的响应时间
    /// </summary>
    /// <param name="requestIndex">请求索引</param>
    /// <returns>响应和耗时</returns>
    private async Task<(HttpResponseMessage Response, long ElapsedMs)> MeasureResizeRequestTime(int requestIndex)
    {
        var file = WebTestHelper.CreateTestFormFile($"perf_test_{requestIndex}.jpg", 400, 400);
        var fields = new Dictionary<string, string>
        {
            ["Width"] = "200",
            ["Height"] = "200",
            ["ResizeMode"] = "Stretch",
            ["Quality"] = "80"
        };
        
        var content = CreateMultipartContent(file, fields);
        var stopwatch = Stopwatch.StartNew();
        
        var response = await Client.PostAsync("/api/resize", content);
        stopwatch.Stop();
        
        return (response, stopwatch.ElapsedMilliseconds);
    }
    
    /// <summary>
    /// 创建调整大小请求
    /// </summary>
    /// <returns>HTTP响应</returns>
    private async Task<HttpResponseMessage> CreateResizeRequest()
    {
        var file = WebTestHelper.CreateTestFormFile("resize_test.jpg", 300, 300);
        var fields = new Dictionary<string, string>
        {
            ["Width"] = "150",
            ["Height"] = "150",
            ["ResizeMode"] = "Stretch",
            ["Quality"] = "80"
        };
        
        var content = CreateMultipartContent(file, fields);
        return await Client.PostAsync("/api/resize", content);
    }
    
    /// <summary>
    /// 创建压缩请求
    /// </summary>
    /// <returns>HTTP响应</returns>
    private async Task<HttpResponseMessage> CreateCompressRequest()
    {
        var file = WebTestHelper.CreateTestFormFile("compress_test.jpg", 400, 400);
        var fields = new Dictionary<string, string>
        {
            ["Quality"] = "70"
        };
        
        var content = CreateMultipartContent(file, fields);
        return await Client.PostAsync("/api/compress", content);
    }
    
    /// <summary>
    /// 创建缩略图请求
    /// </summary>
    /// <returns>HTTP响应</returns>
    private async Task<HttpResponseMessage> CreateThumbnailRequest()
    {
        var file = WebTestHelper.CreateTestFormFile("thumbnail_test.jpg", 500, 500);
        var fields = new Dictionary<string, string>
        {
            ["Size"] = "100",
            ["Quality"] = "85"
        };
        
        var content = CreateMultipartContent(file, fields);
        return await Client.PostAsync("/api/resize/thumbnail", content);
    }
}