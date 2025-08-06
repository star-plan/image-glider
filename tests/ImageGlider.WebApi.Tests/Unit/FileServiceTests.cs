using FluentAssertions;
using Microsoft.AspNetCore.Http;
using ImageGlider.WebApi.Services;
using ImageGlider.WebApi.Tests.TestHelpers;
using Moq;
using Xunit;

namespace ImageGlider.WebApi.Tests.Unit;

/// <summary>
/// 文件服务单元测试
/// </summary>
public class FileServiceTests : IDisposable
{
    private readonly FileService _fileService;
    private readonly string _tempDirectory;
    
    public FileServiceTests()
    {
        _fileService = new FileService();
        _tempDirectory = WebTestHelper.CreateTempDirectory();
    }
    
    [Fact]
    public async Task SaveUploadedFile_WithValidFile_ShouldReturnCorrectPaths()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 100, 100);
        var fileExtension = ".jpg";
        
        // Act
        var (tempFilePath, outputPath) = await _fileService.SaveUploadedFile(testFile, fileExtension);
        
        // Assert
        tempFilePath.Should().NotBeNullOrEmpty();
        outputPath.Should().NotBeNullOrEmpty();
        
        File.Exists(tempFilePath).Should().BeTrue();
        Path.GetExtension(tempFilePath).Should().Be(".jpg");
        Path.GetExtension(outputPath).Should().Be(".jpg");
        
        // 验证文件内容不为空
        var fileInfo = new FileInfo(tempFilePath);
        fileInfo.Length.Should().BeGreaterThan(0);
        
        // 清理
        WebTestHelper.CleanupFile(tempFilePath);
    }
    
    [Fact]
    public async Task SaveUploadedFile_WithDifferentExtension_ShouldUseProvidedExtension()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 100, 100);
        var customExtension = ".png";
        
        // Act
        var (tempFilePath, outputPath) = await _fileService.SaveUploadedFile(testFile, customExtension);
        
        // Assert
        Path.GetExtension(outputPath).Should().Be(".png");
        
        // 清理
        WebTestHelper.CleanupFile(tempFilePath);
    }
    
    [Fact]
    public async Task SaveUploadedFile_WithExtensionWithoutDot_ShouldAddDot()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 100, 100);
        var extensionWithoutDot = "png";
        
        // Act
        var (tempFilePath, outputPath) = await _fileService.SaveUploadedFile(testFile, extensionWithoutDot);
        
        // Assert
        Path.GetExtension(outputPath).Should().Be(".png");
        
        // 清理
        WebTestHelper.CleanupFile(tempFilePath);
    }
    
    [Theory]
    [InlineData("test.jpg", true)]
    [InlineData("test.png", true)]
    [InlineData("test.jpeg", true)]
    [InlineData("test.gif", true)]
    [InlineData("test.bmp", true)]
    [InlineData("test.webp", true)]
    [InlineData("test.txt", false)]
    [InlineData("test.pdf", false)]
    [InlineData("test.doc", false)]
    public void IsFileValid_WithDifferentFileTypes_ShouldReturnExpectedResult(string fileName, bool expectedResult)
    {
        // Arrange
        IFormFile testFile;
        if (expectedResult)
        {
            testFile = WebTestHelper.CreateTestFormFile(fileName, 100, 100);
        }
        else
        {
            testFile = WebTestHelper.CreateInvalidTestFile(fileName);
        }
        
        // Act
        var result = _fileService.IsFileValid(testFile);
        
        // Assert
        result.Should().Be(expectedResult);
    }
    
    [Fact]
    public void IsFileValid_WithNullFile_ShouldReturnFalse()
    {
        // Act
        var result = _fileService.IsFileValid(null!);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void IsFileValid_WithEmptyFileName_ShouldReturnFalse()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns(string.Empty);
        
        // Act
        var result = _fileService.IsFileValid(mockFile.Object);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void IsFileValid_WithZeroLength_ShouldReturnFalse()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test.jpg");
        mockFile.Setup(f => f.Length).Returns(0);
        
        // Act
        var result = _fileService.IsFileValid(mockFile.Object);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task ReadFileToMemoryAsync_WithExistingFile_ShouldReturnFileData()
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile("test.jpg", 100, 100);
        var (tempFilePath, outputPath) = await _fileService.SaveUploadedFile(testFile, ".jpg");
        
        // 将临时文件复制到输出目录以模拟处理完成的文件
        var outputDir = Path.GetDirectoryName(outputPath)!;
        Directory.CreateDirectory(outputDir);
        File.Copy(tempFilePath, outputPath, true);
        
        var fileName = Path.GetFileName(outputPath);
        
        // Act
        var (mimeType, memoryStream) = await _fileService.ReadFileToMemoryAsync(fileName);
        
        // Assert
        mimeType.Should().Be("image/jpeg");
        memoryStream.Should().NotBeNull();
        memoryStream.Length.Should().BeGreaterThan(0);
        
        // 清理
        memoryStream.Dispose();
        WebTestHelper.CleanupFile(tempFilePath);
        WebTestHelper.CleanupFile(outputPath);
    }
    
    [Fact]
    public async Task ReadFileToMemoryAsync_WithNonExistentFile_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var nonExistentFileName = "nonexistent.jpg";
        
        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => 
            _fileService.ReadFileToMemoryAsync(nonExistentFileName));
    }
    
    [Fact]
    public void DeleteFile_WithExistingFile_ShouldDeleteFile()
    {
        // Arrange
        var testFilePath = Path.Combine(_tempDirectory, "test_delete.txt");
        File.WriteAllText(testFilePath, "test content");
        File.Exists(testFilePath).Should().BeTrue();
        
        // Act
        _fileService.DeleteFile(testFilePath);
        
        // Assert
        File.Exists(testFilePath).Should().BeFalse();
    }
    
    [Fact]
    public void DeleteFile_WithNonExistentFile_ShouldNotThrow()
    {
        // Arrange
        var nonExistentPath = Path.Combine(_tempDirectory, "nonexistent.txt");
        
        // Act & Assert
        var action = () => _fileService.DeleteFile(nonExistentPath);
        action.Should().NotThrow();
    }
    
    [Fact]
    public void GetFileName_WithValidPath_ShouldReturnFileName()
    {
        // Arrange
        var filePath = "/path/to/file/test.jpg";
        
        // Act
        var fileName = _fileService.GetFileName(filePath);
        
        // Assert
        fileName.Should().Be("test.jpg");
    }
    
    [Theory]
    [InlineData("test.jpg", "image/jpeg")]
    [InlineData("test.png", "image/png")]
    [InlineData("test.gif", "image/gif")]
    [InlineData("test.bmp", "image/bmp")]
    [InlineData("test.webp", "image/webp")]
    [InlineData("test.unknown", "application/octet-stream")]
    public async Task ReadFileToMemoryAsync_WithDifferentFormats_ShouldReturnCorrectMimeType(string fileName, string expectedMimeType)
    {
        // Arrange
        var testFile = WebTestHelper.CreateTestFormFile(fileName, 100, 100);
        var (tempFilePath, outputPath) = await _fileService.SaveUploadedFile(testFile, Path.GetExtension(fileName));
        
        // 将临时文件复制到输出目录
        var outputDir = Path.GetDirectoryName(outputPath)!;
        Directory.CreateDirectory(outputDir);
        File.Copy(tempFilePath, outputPath, true);
        
        var outputFileName = Path.GetFileName(outputPath);
        
        // Act
        var (mimeType, memoryStream) = await _fileService.ReadFileToMemoryAsync(outputFileName);
        
        // Assert
        mimeType.Should().Be(expectedMimeType);
        
        // 清理
        memoryStream.Dispose();
        WebTestHelper.CleanupFile(tempFilePath);
        WebTestHelper.CleanupFile(outputPath);
    }
    
    public void Dispose()
    {
        WebTestHelper.CleanupDirectory(_tempDirectory);
        GC.SuppressFinalize(this);
    }
}