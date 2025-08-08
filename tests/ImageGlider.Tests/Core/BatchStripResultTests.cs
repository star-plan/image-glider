using Xunit;
using ImageGlider.Core;

namespace ImageGlider.Tests.Core;

/// <summary>
/// BatchStripResult 和 ProcessedFileInfo 类的单元测试
/// </summary>
public class BatchStripResultTests
{
    [Fact]
    public void BatchStripResult_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var result = new BatchStripResult();
        
        // Assert
        Assert.Equal(0, result.TotalFiles);
        Assert.Equal(0, result.SuccessCount);
        Assert.Equal(0, result.FailureCount);
        Assert.NotNull(result.ProcessedFiles);
        Assert.Empty(result.ProcessedFiles);
        Assert.Null(result.ErrorMessage);
        Assert.True(result.Success);
    }
    
    [Fact]
    public void BatchStripResult_SetProperties_ShouldWork()
    {
        // Arrange
        var result = new BatchStripResult();
        var processedFiles = new List<ProcessedFileInfo>
        {
            new() { SourcePath = "source1.jpg", TargetPath = "target1.jpg", Success = true },
            new() { SourcePath = "source2.png", TargetPath = "target2.png", Success = false, ErrorMessage = "Failed" }
        };
        
        // Act
        result.TotalFiles = 2;
        result.SuccessCount = 1;
        result.FailureCount = 1;
        result.ProcessedFiles = processedFiles;
        result.ErrorMessage = "Some processing failed";
        
        // Assert
        Assert.Equal(2, result.TotalFiles);
        Assert.Equal(1, result.SuccessCount);
        Assert.Equal(1, result.FailureCount);
        Assert.Equal(processedFiles, result.ProcessedFiles);
        Assert.Equal("Some processing failed", result.ErrorMessage);
        Assert.False(result.Success); // 因为有错误消息
    }
    
    [Fact]
    public void Success_WithNoErrorsAndNoFailures_ShouldReturnTrue()
    {
        // Arrange
        var result = new BatchStripResult
        {
            TotalFiles = 3,
            SuccessCount = 3,
            FailureCount = 0,
            ErrorMessage = null
        };
        
        // Act & Assert
        Assert.True(result.Success);
    }
    
    [Fact]
    public void Success_WithEmptyErrorMessageAndNoFailures_ShouldReturnTrue()
    {
        // Arrange
        var result = new BatchStripResult
        {
            TotalFiles = 2,
            SuccessCount = 2,
            FailureCount = 0,
            ErrorMessage = ""
        };
        
        // Act & Assert
        Assert.True(result.Success);
    }
    
    [Fact]
    public void Success_WithErrorMessage_ShouldReturnFalse()
    {
        // Arrange
        var result = new BatchStripResult
        {
            TotalFiles = 2,
            SuccessCount = 2,
            FailureCount = 0,
            ErrorMessage = "Some error occurred"
        };
        
        // Act & Assert
        Assert.False(result.Success);
    }
    
    [Fact]
    public void Success_WithFailures_ShouldReturnFalse()
    {
        // Arrange
        var result = new BatchStripResult
        {
            TotalFiles = 3,
            SuccessCount = 2,
            FailureCount = 1,
            ErrorMessage = null
        };
        
        // Act & Assert
        Assert.False(result.Success);
    }
    
    [Theory]
    [InlineData(0, 0, 0, null, true)]
    [InlineData(5, 5, 0, null, true)]
    [InlineData(5, 5, 0, "", true)]
    [InlineData(5, 4, 1, null, false)]
    [InlineData(5, 5, 0, "Error", false)]
    [InlineData(5, 3, 2, "Multiple errors", false)]
    public void Success_VariousScenarios_ShouldReturnExpectedResult(
        int totalFiles, 
        int successCount, 
        int failureCount, 
        string? errorMessage, 
        bool expectedSuccess)
    {
        // Arrange
        var result = new BatchStripResult
        {
            TotalFiles = totalFiles,
            SuccessCount = successCount,
            FailureCount = failureCount,
            ErrorMessage = errorMessage
        };
        
        // Act & Assert
        Assert.Equal(expectedSuccess, result.Success);
    }
}

/// <summary>
/// ProcessedFileInfo 类的单元测试
/// </summary>
public class ProcessedFileInfoTests
{
    [Fact]
    public void ProcessedFileInfo_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var fileInfo = new ProcessedFileInfo();
        
        // Assert
        Assert.Equal(string.Empty, fileInfo.SourcePath);
        Assert.Equal(string.Empty, fileInfo.TargetPath);
        Assert.False(fileInfo.Success);
        Assert.Null(fileInfo.ErrorMessage);
    }
    
    [Fact]
    public void ProcessedFileInfo_SetProperties_ShouldWork()
    {
        // Arrange
        var fileInfo = new ProcessedFileInfo();
        
        // Act
        fileInfo.SourcePath = "source.jpg";
        fileInfo.TargetPath = "target.png";
        fileInfo.Success = true;
        fileInfo.ErrorMessage = "No error";
        
        // Assert
        Assert.Equal("source.jpg", fileInfo.SourcePath);
        Assert.Equal("target.png", fileInfo.TargetPath);
        Assert.True(fileInfo.Success);
        Assert.Equal("No error", fileInfo.ErrorMessage);
    }
    
    [Fact]
    public void ProcessedFileInfo_SuccessfulProcessing_ShouldHaveCorrectState()
    {
        // Arrange & Act
        var fileInfo = new ProcessedFileInfo
        {
            SourcePath = "/path/to/source.jpg",
            TargetPath = "/path/to/target.jpg",
            Success = true,
            ErrorMessage = null
        };
        
        // Assert
        Assert.Equal("/path/to/source.jpg", fileInfo.SourcePath);
        Assert.Equal("/path/to/target.jpg", fileInfo.TargetPath);
        Assert.True(fileInfo.Success);
        Assert.Null(fileInfo.ErrorMessage);
    }
    
    [Fact]
    public void ProcessedFileInfo_FailedProcessing_ShouldHaveCorrectState()
    {
        // Arrange & Act
        var fileInfo = new ProcessedFileInfo
        {
            SourcePath = "/path/to/source.jpg",
            TargetPath = "/path/to/target.jpg",
            Success = false,
            ErrorMessage = "File format not supported"
        };
        
        // Assert
        Assert.Equal("/path/to/source.jpg", fileInfo.SourcePath);
        Assert.Equal("/path/to/target.jpg", fileInfo.TargetPath);
        Assert.False(fileInfo.Success);
        Assert.Equal("File format not supported", fileInfo.ErrorMessage);
    }
    
    [Theory]
    [InlineData("", "", false, null)]
    [InlineData("source.jpg", "target.png", true, null)]
    [InlineData("source.jpg", "target.png", false, "Error occurred")]
    [InlineData("/long/path/to/source.jpg", "/long/path/to/target.png", true, "")]
    public void ProcessedFileInfo_VariousInputs_ShouldSetPropertiesCorrectly(
        string sourcePath, 
        string targetPath, 
        bool success, 
        string? errorMessage)
    {
        // Arrange & Act
        var fileInfo = new ProcessedFileInfo
        {
            SourcePath = sourcePath,
            TargetPath = targetPath,
            Success = success,
            ErrorMessage = errorMessage
        };
        
        // Assert
        Assert.Equal(sourcePath, fileInfo.SourcePath);
        Assert.Equal(targetPath, fileInfo.TargetPath);
        Assert.Equal(success, fileInfo.Success);
        Assert.Equal(errorMessage, fileInfo.ErrorMessage);
    }
}
