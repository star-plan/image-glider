using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ImageGlider.WebApi.Exceptions;
using System.Text.Json;

namespace ImageGlider.WebApi.Tests.Unit;

/// <summary>
/// 全局异常处理器单元测试
/// </summary>
public class GlobalExceptionHandlerTests
{
    private readonly Mock<IProblemDetailsService> _mockProblemDetailsService;
    private readonly Mock<ILogger<GlobalExceptionHandler>> _mockLogger;
    private readonly GlobalExceptionHandler _exceptionHandler;
    
    public GlobalExceptionHandlerTests()
    {
        _mockProblemDetailsService = new Mock<IProblemDetailsService>();
        _mockLogger = new Mock<ILogger<GlobalExceptionHandler>>();
        _exceptionHandler = new GlobalExceptionHandler(_mockProblemDetailsService.Object, _mockLogger.Object);
    }
    
    [Fact]
    public async Task TryHandleAsync_WithApplicationException_ShouldReturn400StatusCode()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = new ApplicationException("Test application exception");
        var cancellationToken = CancellationToken.None;
        
        _mockProblemDetailsService
            .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(true);
        
        // Act
        var result = await _exceptionHandler.TryHandleAsync(httpContext, exception, cancellationToken);
        
        // Assert
        result.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        
        // 验证日志记录
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("An unhandled exception occurred.")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
    
    [Fact]
    public async Task TryHandleAsync_WithGenericException_ShouldReturn500StatusCode()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = new InvalidOperationException("Test generic exception");
        var cancellationToken = CancellationToken.None;
        
        _mockProblemDetailsService
            .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(true);
        
        // Act
        var result = await _exceptionHandler.TryHandleAsync(httpContext, exception, cancellationToken);
        
        // Assert
        result.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
    
    [Fact]
    public async Task TryHandleAsync_ShouldCallProblemDetailsServiceWithCorrectContext()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = new ApplicationException("Test exception");
        var cancellationToken = CancellationToken.None;
        
        ProblemDetailsContext? capturedContext = null;
        _mockProblemDetailsService
            .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(context => capturedContext = context)
            .ReturnsAsync(true);
        
        // Act
        await _exceptionHandler.TryHandleAsync(httpContext, exception, cancellationToken);
        
        // Assert
        capturedContext.Should().NotBeNull();
        capturedContext!.HttpContext.Should().Be(httpContext);
        capturedContext.Exception.Should().Be(exception);
        capturedContext.ProblemDetails.Should().NotBeNull();
        
        var problemDetails = capturedContext.ProblemDetails;
        problemDetails.Type.Should().Be("ApplicationException");
        problemDetails.Title.Should().Be("处理请求时发生异常");
        problemDetails.Detail.Should().Be("Test exception");
        problemDetails.Status.Should().Be(StatusCodes.Status400BadRequest);
        problemDetails.Instance.Should().Be(httpContext.Request.Path);
    }
    
    [Theory]
    [InlineData(typeof(ApplicationException), StatusCodes.Status400BadRequest)]
    [InlineData(typeof(InvalidOperationException), StatusCodes.Status500InternalServerError)]
    [InlineData(typeof(ArgumentException), StatusCodes.Status500InternalServerError)]
    [InlineData(typeof(FileNotFoundException), StatusCodes.Status500InternalServerError)]
    public async Task TryHandleAsync_WithDifferentExceptionTypes_ShouldReturnCorrectStatusCode(Type exceptionType, int expectedStatusCode)
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = (Exception)Activator.CreateInstance(exceptionType, "Test exception")!;
        var cancellationToken = CancellationToken.None;
        
        _mockProblemDetailsService
            .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(true);
        
        // Act
        var result = await _exceptionHandler.TryHandleAsync(httpContext, exception, cancellationToken);
        
        // Assert
        result.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(expectedStatusCode);
    }
    
    [Fact]
    public async Task TryHandleAsync_ShouldLogExceptionWithCorrectLevel()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = new Exception("Test exception");
        var cancellationToken = CancellationToken.None;
        
        _mockProblemDetailsService
            .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(true);
        
        // Act
        await _exceptionHandler.TryHandleAsync(httpContext, exception, cancellationToken);
        
        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
    
    [Fact]
    public async Task TryHandleAsync_WithNullException_ShouldHandleGracefully()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        Exception? exception = null;
        var cancellationToken = CancellationToken.None;
        
        _mockProblemDetailsService
            .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(true);
        
        // Act & Assert
        var action = async () => await _exceptionHandler.TryHandleAsync(httpContext, exception!, cancellationToken);
        await action.Should().NotThrowAsync();
    }
    
    [Fact]
    public async Task TryHandleAsync_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = new Exception("Test exception");
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        _mockProblemDetailsService
            .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ThrowsAsync(new OperationCanceledException());
        
        // Act & Assert
        var action = async () => await _exceptionHandler.TryHandleAsync(httpContext, exception, cancellationTokenSource.Token);
        await action.Should().ThrowAsync<OperationCanceledException>();
    }
    
    /// <summary>
    /// 创建测试用的 HttpContext
    /// </summary>
    /// <returns>HttpContext 实例</returns>
    private static HttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/test";
        context.Request.Method = "POST";
        return context;
    }
}