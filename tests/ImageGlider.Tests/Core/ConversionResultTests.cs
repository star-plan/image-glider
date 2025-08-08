using Xunit;
using ImageGlider.Core;

namespace ImageGlider.Tests.Core;

/// <summary>
/// ConversionResult 类的单元测试
/// </summary>
public class ConversionResultTests
{
    [Fact]
    public void ConversionResult_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var result = new ConversionResult();
        
        // Assert
        Assert.Equal(0, result.TotalFiles);
        Assert.Equal(0, result.SuccessfulConversions);
        Assert.Equal(0, result.FailedConversions);
        Assert.NotNull(result.SuccessfulFiles);
        Assert.Empty(result.SuccessfulFiles);
        Assert.NotNull(result.FailedFiles);
        Assert.Empty(result.FailedFiles);
        Assert.Null(result.ErrorMessage);
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public void ConversionResult_SetProperties_ShouldWork()
    {
        // Arrange
        var result = new ConversionResult();
        var successfulFiles = new List<string> { "file1.jpg", "file2.png" };
        var failedFiles = new List<string> { "file3.bmp" };
        
        // Act
        result.TotalFiles = 3;
        result.SuccessfulConversions = 2;
        result.FailedConversions = 1;
        result.SuccessfulFiles = successfulFiles;
        result.FailedFiles = failedFiles;
        result.ErrorMessage = "Some files failed";
        
        // Assert
        Assert.Equal(3, result.TotalFiles);
        Assert.Equal(2, result.SuccessfulConversions);
        Assert.Equal(1, result.FailedConversions);
        Assert.Equal(successfulFiles, result.SuccessfulFiles);
        Assert.Equal(failedFiles, result.FailedFiles);
        Assert.Equal("Some files failed", result.ErrorMessage);
        Assert.False(result.IsSuccess); // 因为有错误消息
    }
    
    [Fact]
    public void IsSuccess_WithNoErrorsAndNoFailures_ShouldReturnTrue()
    {
        // Arrange
        var result = new ConversionResult
        {
            TotalFiles = 2,
            SuccessfulConversions = 2,
            FailedConversions = 0,
            ErrorMessage = null
        };
        
        // Act & Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public void IsSuccess_WithEmptyErrorMessageAndNoFailures_ShouldReturnTrue()
    {
        // Arrange
        var result = new ConversionResult
        {
            TotalFiles = 2,
            SuccessfulConversions = 2,
            FailedConversions = 0,
            ErrorMessage = ""
        };
        
        // Act & Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public void IsSuccess_WithErrorMessage_ShouldReturnFalse()
    {
        // Arrange
        var result = new ConversionResult
        {
            TotalFiles = 2,
            SuccessfulConversions = 2,
            FailedConversions = 0,
            ErrorMessage = "Some error occurred"
        };
        
        // Act & Assert
        Assert.False(result.IsSuccess);
    }
    
    [Fact]
    public void IsSuccess_WithFailedConversions_ShouldReturnFalse()
    {
        // Arrange
        var result = new ConversionResult
        {
            TotalFiles = 3,
            SuccessfulConversions = 2,
            FailedConversions = 1,
            ErrorMessage = null
        };
        
        // Act & Assert
        Assert.False(result.IsSuccess);
    }
    
    [Fact]
    public void IsSuccess_WithBothErrorMessageAndFailures_ShouldReturnFalse()
    {
        // Arrange
        var result = new ConversionResult
        {
            TotalFiles = 3,
            SuccessfulConversions = 1,
            FailedConversions = 2,
            ErrorMessage = "Multiple errors occurred"
        };
        
        // Act & Assert
        Assert.False(result.IsSuccess);
    }
    
    [Fact]
    public void SuccessfulFiles_AddFiles_ShouldWork()
    {
        // Arrange
        var result = new ConversionResult();
        
        // Act
        result.SuccessfulFiles.Add("file1.jpg");
        result.SuccessfulFiles.Add("file2.png");
        
        // Assert
        Assert.Equal(2, result.SuccessfulFiles.Count);
        Assert.Contains("file1.jpg", result.SuccessfulFiles);
        Assert.Contains("file2.png", result.SuccessfulFiles);
    }
    
    [Fact]
    public void FailedFiles_AddFiles_ShouldWork()
    {
        // Arrange
        var result = new ConversionResult();
        
        // Act
        result.FailedFiles.Add("failed1.bmp");
        result.FailedFiles.Add("failed2.tiff");
        
        // Assert
        Assert.Equal(2, result.FailedFiles.Count);
        Assert.Contains("failed1.bmp", result.FailedFiles);
        Assert.Contains("failed2.tiff", result.FailedFiles);
    }
    
    [Theory]
    [InlineData(0, 0, 0, null, true)]
    [InlineData(5, 5, 0, null, true)]
    [InlineData(5, 5, 0, "", true)]
    [InlineData(5, 4, 1, null, false)]
    [InlineData(5, 5, 0, "Error", false)]
    [InlineData(5, 3, 2, "Multiple errors", false)]
    public void IsSuccess_VariousScenarios_ShouldReturnExpectedResult(
        int totalFiles, 
        int successfulConversions, 
        int failedConversions, 
        string? errorMessage, 
        bool expectedIsSuccess)
    {
        // Arrange
        var result = new ConversionResult
        {
            TotalFiles = totalFiles,
            SuccessfulConversions = successfulConversions,
            FailedConversions = failedConversions,
            ErrorMessage = errorMessage
        };
        
        // Act & Assert
        Assert.Equal(expectedIsSuccess, result.IsSuccess);
    }
}
