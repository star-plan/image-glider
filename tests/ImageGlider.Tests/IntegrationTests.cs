using System.IO;
using Xunit;
using ImageGlider;
using ImageGlider.Tests.TestHelpers;
using ImageGlider.Core;
using ImageGlider.Processors;
using ImageGlider.Enums;

namespace ImageGlider.Tests;

/// <summary>
/// 集成测试场景，测试多个处理器的组合使用
/// </summary>
public class IntegrationTests : IDisposable
{
    private readonly string _tempDir;
    private readonly string _sourceDir;
    private readonly string _outputDir;
    
    public IntegrationTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _sourceDir = Path.Combine(_tempDir, "source");
        _outputDir = Path.Combine(_tempDir, "output");
        
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_outputDir);
    }
    
    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }
    
    [Fact]
    public void CompleteImageProcessingWorkflow_ShouldSucceed()
    {
        // Arrange - 创建测试图片
        var originalImage = Path.Combine(_sourceDir, "original.jpg");
        TestImageHelper.CreateTestImage(originalImage, 800, 600);
        
        var step1Output = Path.Combine(_outputDir, "step1_resized.jpg");
        var step2Output = Path.Combine(_outputDir, "step2_cropped.jpg");
        var step3Output = Path.Combine(_outputDir, "step3_watermarked.jpg");
        var step4Output = Path.Combine(_outputDir, "step4_compressed.jpg");
        var finalOutput = Path.Combine(_outputDir, "final_converted.png");
        
        // Act - 执行完整的处理流程
        
        // 步骤1: 调整尺寸
        var resizeSuccess = ImageConverter.ResizeImage(originalImage, step1Output, 400, 300, ResizeMode.KeepAspectRatio, 90);
        
        // 步骤2: 裁剪
        var cropSuccess = ImageConverter.CropImage(step1Output, step2Output, 50, 50, 200, 150, 90);
        
        // 步骤3: 添加水印
        var watermarkSuccess = ImageConverter.AddTextWatermark(step2Output, step3Output, "© Test", WatermarkPosition.BottomRight, 50, 16, "#FFFFFF", 90);
        
        // 步骤4: 压缩
        var compressSuccess = ImageConverter.CompressImage(step3Output, step4Output, 70);
        
        // 步骤5: 格式转换
        var convertSuccess = ImageConverter.ConvertImage(step4Output, finalOutput, 85);
        
        // Assert - 验证每个步骤都成功
        Assert.True(resizeSuccess, "调整尺寸失败");
        Assert.True(cropSuccess, "裁剪失败");
        Assert.True(watermarkSuccess, "添加水印失败");
        Assert.True(compressSuccess, "压缩失败");
        Assert.True(convertSuccess, "格式转换失败");
        
        // 验证所有中间文件和最终文件都存在
        Assert.True(File.Exists(step1Output));
        Assert.True(File.Exists(step2Output));
        Assert.True(File.Exists(step3Output));
        Assert.True(File.Exists(step4Output));
        Assert.True(File.Exists(finalOutput));
        
        // 验证最终文件的信息
        var finalInfo = ImageConverter.GetImageInfo(finalOutput);
        Assert.NotNull(finalInfo);
        Assert.Equal("PNG", finalInfo.Format);
        Assert.Equal(200, finalInfo.Width);
        Assert.Equal(150, finalInfo.Height);
    }
    
    [Fact]
    public void ProcessorFactoryChainedProcessing_ShouldSucceed()
    {
        // Arrange
        var sourceImage = Path.Combine(_sourceDir, "source.jpg");
        TestImageHelper.CreateTestImage(sourceImage, 600, 400);
        
        var resizedImage = Path.Combine(_outputDir, "resized.jpg");
        var croppedImage = Path.Combine(_outputDir, "cropped.jpg");
        var watermarkedImage = Path.Combine(_outputDir, "watermarked.jpg");
        var finalImage = Path.Combine(_outputDir, "final.png");
        
        // Act - 使用处理器工厂创建处理器链
        var resizer = ImageProcessorFactory.CreateResizer();
        var cropper = ImageProcessorFactory.CreateCropper();
        var watermark = ImageProcessorFactory.CreateWatermark();
        var converter = ImageProcessorFactory.CreateFormatConverter();
        
        // 执行链式处理
        var step1 = resizer.ProcessImage(sourceImage, resizedImage, 90);
        var step2 = cropper.ProcessImage(resizedImage, croppedImage, 90);
        var step3 = watermark.ProcessImage(croppedImage, watermarkedImage, 90);
        var step4 = converter.ProcessImage(watermarkedImage, finalImage, 85);
        
        // Assert
        Assert.True(step1);
        Assert.True(step2);
        Assert.True(step3);
        Assert.True(step4);
        Assert.True(File.Exists(finalImage));
    }
    
    [Fact]
    public void BatchProcessingWithMultipleOperations_ShouldSucceed()
    {
        // Arrange - 创建多个测试图片
        var images = new[]
        {
            "image1.jpg",
            "image2.jpg", 
            "image3.jpg"
        };
        
        foreach (var imageName in images)
        {
            var imagePath = Path.Combine(_sourceDir, imageName);
            TestImageHelper.CreateTestImage(imagePath, 400, 300);
        }
        
        var resizedDir = Path.Combine(_outputDir, "resized");
        var compressedDir = Path.Combine(_outputDir, "compressed");
        var convertedDir = Path.Combine(_outputDir, "converted");
        
        Directory.CreateDirectory(resizedDir);
        Directory.CreateDirectory(compressedDir);
        Directory.CreateDirectory(convertedDir);
        
        // Act - 执行批量处理流程
        
        // 批量调整尺寸
        var resizeResult = ImageConverter.BatchResize(_sourceDir, resizedDir, ".jpg", 200, 150, ResizeMode.KeepAspectRatio, 90);
        
        // 批量压缩
        var compressResult = ImageConverter.BatchCompress(resizedDir, compressedDir, ".jpg", 70);
        
        // 批量格式转换
        var convertResult = ImageConverter.BatchConvert(compressedDir, convertedDir, ".jpg", ".png", 85);
        
        // Assert
        Assert.True(resizeResult.IsSuccess);
        Assert.Equal(3, resizeResult.SuccessfulConversions);
        
        Assert.True(compressResult.IsSuccess);
        Assert.Equal(3, compressResult.SuccessfulConversions);
        
        Assert.True(convertResult.IsSuccess);
        Assert.Equal(3, convertResult.SuccessfulConversions);
        
        // 验证最终输出文件
        foreach (var imageName in images)
        {
            var finalImageName = Path.GetFileNameWithoutExtension(imageName) + ".png";
            var finalImagePath = Path.Combine(convertedDir, finalImageName);
            Assert.True(File.Exists(finalImagePath));
        }
    }
    
    [Fact]
    public void ColorAdjustmentAndMetadataStripping_ShouldSucceed()
    {
        // Arrange
        var sourceImage = Path.Combine(_sourceDir, "source.jpg");
        TestImageHelper.CreateTestImage(sourceImage, 300, 200);
        
        var colorAdjustedImage = Path.Combine(_outputDir, "color_adjusted.jpg");
        var metadataStrippedImage = Path.Combine(_outputDir, "metadata_stripped.jpg");
        
        // Act
        // 颜色调整
        var colorAdjustSuccess = ImageConverter.AdjustColor(sourceImage, colorAdjustedImage, 10, 5, 15, 0, 1.1f, 90);
        
        // 清理元数据
        var metadataStripSuccess = ImageConverter.StripMetadata(colorAdjustedImage, metadataStrippedImage, true, true, false, true, 85);
        
        // Assert
        Assert.True(colorAdjustSuccess);
        Assert.True(metadataStripSuccess);
        Assert.True(File.Exists(colorAdjustedImage));
        Assert.True(File.Exists(metadataStrippedImage));
        
        // 验证文件信息
        var originalInfo = ImageConverter.GetImageInfo(sourceImage);
        var finalInfo = ImageConverter.GetImageInfo(metadataStrippedImage);
        
        Assert.NotNull(originalInfo);
        Assert.NotNull(finalInfo);
        Assert.Equal(originalInfo.Width, finalInfo.Width);
        Assert.Equal(originalInfo.Height, finalInfo.Height);
    }
    
    [Fact]
    public void ErrorHandlingInChainedProcessing_ShouldHandleGracefully()
    {
        // Arrange
        var sourceImage = Path.Combine(_sourceDir, "source.jpg");
        TestImageHelper.CreateTestImage(sourceImage, 200, 200);
        
        var step1Output = Path.Combine(_outputDir, "step1.jpg");
        var invalidOutput = Path.Combine("invalid_directory_that_does_not_exist", "step2.jpg");
        var step3Output = Path.Combine(_outputDir, "step3.jpg");
        
        // Act
        var step1Success = ImageConverter.ResizeImage(sourceImage, step1Output, 150, 150, ResizeMode.KeepAspectRatio, 90);
        var step2Success = ImageConverter.CropImage(step1Output, invalidOutput, 25, 25, 100, 100, 90); // 这应该失败
        var step3Success = ImageConverter.CropImage(step1Output, step3Output, 25, 25, 100, 100, 90); // 这应该成功
        
        // Assert
        Assert.True(step1Success, "第一步应该成功");
        Assert.False(step2Success, "第二步应该失败（无效输出路径）");
        Assert.True(step3Success, "第三步应该成功");
        
        Assert.True(File.Exists(step1Output));
        Assert.False(File.Exists(invalidOutput));
        Assert.True(File.Exists(step3Output));
    }
    
    [Fact]
    public void ComplexBatchProcessingWithErrorHandling_ShouldPartiallySucceed()
    {
        // Arrange - 创建混合的文件（有效和无效）
        var validImage1 = Path.Combine(_sourceDir, "valid1.jpg");
        var validImage2 = Path.Combine(_sourceDir, "valid2.jpg");
        var invalidFile = Path.Combine(_sourceDir, "invalid.jpg");
        
        TestImageHelper.CreateTestImage(validImage1, 300, 200);
        TestImageHelper.CreateTestImage(validImage2, 400, 300);
        File.WriteAllText(invalidFile, "This is not an image");
        
        // Act - 批量处理
        var result = ImageConverter.BatchConvert(_sourceDir, _outputDir, ".jpg", ".png", 85);
        
        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess); // 因为有失败的文件
        Assert.Equal(3, result.TotalFiles);
        Assert.Equal(2, result.SuccessfulConversions);
        Assert.Equal(1, result.FailedConversions);
        Assert.Equal(2, result.SuccessfulFiles.Count);
        Assert.Single(result.FailedFiles);
        
        // 验证成功的文件存在
        Assert.True(File.Exists(Path.Combine(_outputDir, "valid1.png")));
        Assert.True(File.Exists(Path.Combine(_outputDir, "valid2.png")));
        Assert.False(File.Exists(Path.Combine(_outputDir, "invalid.png")));
    }
    
    [Fact]
    public void ProcessorTypeEnumeration_ShouldCreateAllProcessors()
    {
        // Arrange & Act - 测试所有处理器类型都能正确创建
        var processorTypes = Enum.GetValues<ImageProcessorType>();
        
        // Assert
        foreach (var processorType in processorTypes)
        {
            var processor = ImageProcessorFactory.CreateProcessor(processorType);
            Assert.NotNull(processor);
            Assert.IsAssignableFrom<IImageProcessor>(processor);
        }
    }
}
