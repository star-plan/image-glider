using Xunit;
using ImageGlider.Core;
using ImageGlider.Processors;

namespace ImageGlider.Tests;

/// <summary>
/// ImageProcessorFactory 工厂类的单元测试
/// </summary>
public class ImageProcessorFactoryTests
{
    /// <summary>
    /// 测试创建格式转换器
    /// </summary>
    [Fact]
    public void CreateFormatConverter_ReturnsValidInstance()
    {
        // Act
        var converter = ImageProcessorFactory.CreateFormatConverter();

        // Assert
        Assert.NotNull(converter);
        Assert.IsType<ImageFormatConverter>(converter);
        Assert.IsAssignableFrom<IImageFormatConverter>(converter);
        Assert.IsAssignableFrom<IImageProcessor>(converter);
    }

    /// <summary>
    /// 测试创建尺寸调整器
    /// </summary>
    [Fact]
    public void CreateResizer_ReturnsValidInstance()
    {
        // Act
        var resizer = ImageProcessorFactory.CreateResizer();

        // Assert
        Assert.NotNull(resizer);
        Assert.IsType<ImageResizer>(resizer);
        Assert.IsAssignableFrom<IImageResizer>(resizer);
        Assert.IsAssignableFrom<IImageProcessor>(resizer);
    }

    /// <summary>
    /// 测试创建压缩器
    /// </summary>
    [Fact]
    public void CreateCompressor_ReturnsValidInstance()
    {
        // Act
        var compressor = ImageProcessorFactory.CreateCompressor();

        // Assert
        Assert.NotNull(compressor);
        Assert.IsType<ImageCompressor>(compressor);
        Assert.IsAssignableFrom<IImageCompressor>(compressor);
        Assert.IsAssignableFrom<IImageProcessor>(compressor);
    }

    /// <summary>
    /// 测试创建裁剪器
    /// </summary>
    [Fact]
    public void CreateCropper_ReturnsValidInstance()
    {
        // Act
        var cropper = ImageProcessorFactory.CreateCropper();

        // Assert
        Assert.NotNull(cropper);
        Assert.IsType<ImageCropper>(cropper);
        Assert.IsAssignableFrom<IImageCropper>(cropper);
        Assert.IsAssignableFrom<IImageProcessor>(cropper);
    }

    /// <summary>
    /// 测试创建水印处理器
    /// </summary>
    [Fact]
    public void CreateWatermark_ReturnsValidInstance()
    {
        // Act
        var watermark = ImageProcessorFactory.CreateWatermark();

        // Assert
        Assert.NotNull(watermark);
        Assert.IsType<ImageWatermark>(watermark);
        Assert.IsAssignableFrom<IImageWatermark>(watermark);
        Assert.IsAssignableFrom<IImageProcessor>(watermark);
    }

    /// <summary>
    /// 测试创建元数据清理器
    /// </summary>
    [Fact]
    public void CreateMetadataStripper_ReturnsValidInstance()
    {
        // Act
        var stripper = ImageProcessorFactory.CreateMetadataStripper();

        // Assert
        Assert.NotNull(stripper);
        Assert.IsType<ImageMetadataStripper>(stripper);
        Assert.IsAssignableFrom<IImageMetadataStripper>(stripper);
        Assert.IsAssignableFrom<IImageProcessor>(stripper);
    }

    /// <summary>
    /// 测试创建颜色调整器
    /// </summary>
    [Fact]
    public void CreateColorAdjuster_ReturnsValidInstance()
    {
        // Act
        var adjuster = ImageProcessorFactory.CreateColorAdjuster();

        // Assert
        Assert.NotNull(adjuster);
        Assert.IsType<ImageColorAdjuster>(adjuster);
        Assert.IsAssignableFrom<IImageColorAdjuster>(adjuster);
        Assert.IsAssignableFrom<IImageProcessor>(adjuster);
    }

    /// <summary>
    /// 测试创建信息提取器
    /// </summary>
    [Fact]
    public void CreateInfoExtractor_ReturnsValidInstance()
    {
        // Act
        var extractor = ImageProcessorFactory.CreateInfoExtractor();

        // Assert
        Assert.NotNull(extractor);
        Assert.IsType<ImageInfoExtractor>(extractor);
        Assert.IsAssignableFrom<IImageInfoExtractor>(extractor);
        Assert.IsAssignableFrom<IImageProcessor>(extractor);
    }

    /// <summary>
    /// 测试通过类型创建处理器 - 格式转换器
    /// </summary>
    [Fact]
    public void CreateProcessor_FormatConverter_ReturnsValidInstance()
    {
        // Act
        var processor = ImageProcessorFactory.CreateProcessor(ImageProcessorType.FormatConverter);

        // Assert
        Assert.NotNull(processor);
        Assert.IsType<ImageFormatConverter>(processor);
        Assert.IsAssignableFrom<IImageFormatConverter>(processor);
    }

    /// <summary>
    /// 测试通过类型创建处理器 - 尺寸调整器
    /// </summary>
    [Fact]
    public void CreateProcessor_Resizer_ReturnsValidInstance()
    {
        // Act
        var processor = ImageProcessorFactory.CreateProcessor(ImageProcessorType.Resizer);

        // Assert
        Assert.NotNull(processor);
        Assert.IsType<ImageResizer>(processor);
        Assert.IsAssignableFrom<IImageResizer>(processor);
    }

    /// <summary>
    /// 测试通过类型创建处理器 - 压缩器
    /// </summary>
    [Fact]
    public void CreateProcessor_Compressor_ReturnsValidInstance()
    {
        // Act
        var processor = ImageProcessorFactory.CreateProcessor(ImageProcessorType.Compressor);

        // Assert
        Assert.NotNull(processor);
        Assert.IsType<ImageCompressor>(processor);
        Assert.IsAssignableFrom<IImageCompressor>(processor);
    }

    /// <summary>
    /// 测试通过类型创建处理器 - 裁剪器
    /// </summary>
    [Fact]
    public void CreateProcessor_Cropper_ReturnsValidInstance()
    {
        // Act
        var processor = ImageProcessorFactory.CreateProcessor(ImageProcessorType.Cropper);

        // Assert
        Assert.NotNull(processor);
        Assert.IsType<ImageCropper>(processor);
        Assert.IsAssignableFrom<IImageCropper>(processor);
    }

    /// <summary>
    /// 测试通过类型创建处理器 - 水印处理器
    /// </summary>
    [Fact]
    public void CreateProcessor_Watermark_ReturnsValidInstance()
    {
        // Act
        var processor = ImageProcessorFactory.CreateProcessor(ImageProcessorType.Watermark);

        // Assert
        Assert.NotNull(processor);
        Assert.IsType<ImageWatermark>(processor);
        Assert.IsAssignableFrom<IImageWatermark>(processor);
    }

    /// <summary>
    /// 测试通过类型创建处理器 - 元数据清理器
    /// </summary>
    [Fact]
    public void CreateProcessor_MetadataStripper_ReturnsValidInstance()
    {
        // Act
        var processor = ImageProcessorFactory.CreateProcessor(ImageProcessorType.MetadataStripper);

        // Assert
        Assert.NotNull(processor);
        Assert.IsType<ImageMetadataStripper>(processor);
        Assert.IsAssignableFrom<IImageMetadataStripper>(processor);
    }

    /// <summary>
    /// 测试通过类型创建处理器 - 颜色调整器
    /// </summary>
    [Fact]
    public void CreateProcessor_ColorAdjuster_ReturnsValidInstance()
    {
        // Act
        var processor = ImageProcessorFactory.CreateProcessor(ImageProcessorType.ColorAdjuster);

        // Assert
        Assert.NotNull(processor);
        Assert.IsType<ImageColorAdjuster>(processor);
        Assert.IsAssignableFrom<IImageColorAdjuster>(processor);
    }

    /// <summary>
    /// 测试通过类型创建处理器 - 信息提取器
    /// </summary>
    [Fact]
    public void CreateProcessor_InfoExtractor_ReturnsValidInstance()
    {
        // Act
        var processor = ImageProcessorFactory.CreateProcessor(ImageProcessorType.InfoExtractor);

        // Assert
        Assert.NotNull(processor);
        Assert.IsType<ImageInfoExtractor>(processor);
        Assert.IsAssignableFrom<IImageInfoExtractor>(processor);
    }

    /// <summary>
    /// 测试创建处理器时传入无效类型应抛出异常
    /// </summary>
    [Fact]
    public void CreateProcessor_InvalidType_ThrowsArgumentException()
    {
        // Arrange
        var invalidType = (ImageProcessorType)999;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            ImageProcessorFactory.CreateProcessor(invalidType));

        Assert.Contains("不支持的处理器类型", exception.Message);
        Assert.Contains("999", exception.Message);
    }

    /// <summary>
    /// 测试所有处理器类型枚举值都能正确创建处理器
    /// </summary>
    [Theory]
    [InlineData(ImageProcessorType.FormatConverter, typeof(ImageFormatConverter))]
    [InlineData(ImageProcessorType.Resizer, typeof(ImageResizer))]
    [InlineData(ImageProcessorType.Compressor, typeof(ImageCompressor))]
    [InlineData(ImageProcessorType.Cropper, typeof(ImageCropper))]
    [InlineData(ImageProcessorType.Watermark, typeof(ImageWatermark))]
    [InlineData(ImageProcessorType.MetadataStripper, typeof(ImageMetadataStripper))]
    [InlineData(ImageProcessorType.ColorAdjuster, typeof(ImageColorAdjuster))]
    [InlineData(ImageProcessorType.InfoExtractor, typeof(ImageInfoExtractor))]
    public void CreateProcessor_AllValidTypes_ReturnsCorrectType(ImageProcessorType processorType, Type expectedType)
    {
        // Act
        var processor = ImageProcessorFactory.CreateProcessor(processorType);

        // Assert
        Assert.NotNull(processor);
        Assert.IsType(expectedType, processor);
        Assert.IsAssignableFrom<IImageProcessor>(processor);
    }



    /// <summary>
    /// 测试通过类型创建处理器 - 格式转换器
    /// </summary>
    [Fact]
    public void CreateProcessor_FormatConverter_ReturnsCorrectType()
    {
        // Act
        var processor = ImageProcessorFactory.CreateProcessor(ImageProcessorType.FormatConverter);

        // Assert
        Assert.NotNull(processor);
        Assert.IsType<ImageFormatConverter>(processor);
    }

    /// <summary>
    /// 测试通过类型创建处理器 - 尺寸调整器
    /// </summary>
    [Fact]
    public void CreateProcessor_Resizer_ReturnsCorrectType()
    {
        // Act
        var processor = ImageProcessorFactory.CreateProcessor(ImageProcessorType.Resizer);

        // Assert
        Assert.NotNull(processor);
        Assert.IsType<ImageResizer>(processor);
    }

    /// <summary>
    /// 测试通过类型创建处理器 - 压缩器
    /// </summary>
    [Fact]
    public void CreateProcessor_Compressor_ReturnsCorrectType()
    {
        // Act
        var processor = ImageProcessorFactory.CreateProcessor(ImageProcessorType.Compressor);

        // Assert
        Assert.NotNull(processor);
        Assert.IsType<ImageCompressor>(processor);
    }

    /// <summary>
    /// 测试通过类型创建处理器 - 裁剪器
    /// </summary>
    [Fact]
    public void CreateProcessor_Cropper_ReturnsCorrectType()
    {
        // Act
        var processor = ImageProcessorFactory.CreateProcessor(ImageProcessorType.Cropper);

        // Assert
        Assert.NotNull(processor);
        Assert.IsType<ImageCropper>(processor);
    }

    /// <summary>
    /// 测试通过类型创建处理器 - 水印处理器
    /// </summary>
    [Fact]
    public void CreateProcessor_Watermark_ReturnsCorrectType()
    {
        // Act
        var processor = ImageProcessorFactory.CreateProcessor(ImageProcessorType.Watermark);

        // Assert
        Assert.NotNull(processor);
        Assert.IsType<ImageWatermark>(processor);
    }

    /// <summary>
    /// 测试通过类型创建处理器 - 元数据清理器
    /// </summary>
    [Fact]
    public void CreateProcessor_MetadataStripper_ReturnsCorrectType()
    {
        // Act
        var processor = ImageProcessorFactory.CreateProcessor(ImageProcessorType.MetadataStripper);

        // Assert
        Assert.NotNull(processor);
        Assert.IsType<ImageMetadataStripper>(processor);
    }

    /// <summary>
    /// 测试通过类型创建处理器 - 颜色调整器
    /// </summary>
    [Fact]
    public void CreateProcessor_ColorAdjuster_ReturnsCorrectType()
    {
        // Act
        var processor = ImageProcessorFactory.CreateProcessor(ImageProcessorType.ColorAdjuster);

        // Assert
        Assert.NotNull(processor);
        Assert.IsType<ImageColorAdjuster>(processor);
    }

    /// <summary>
    /// 测试通过类型创建处理器 - 信息提取器
    /// </summary>
    [Fact]
    public void CreateProcessor_InfoExtractor_ReturnsCorrectType()
    {
        // Act
        var processor = ImageProcessorFactory.CreateProcessor(ImageProcessorType.InfoExtractor);

        // Assert
        Assert.NotNull(processor);
        Assert.IsType<ImageInfoExtractor>(processor);
    }



    /// <summary>
    /// 测试工厂方法返回的实例是独立的
    /// </summary>
    [Fact]
    public void CreateProcessor_MultipleInstances_AreIndependent()
    {
        // Act
        var converter1 = ImageProcessorFactory.CreateFormatConverter();
        var converter2 = ImageProcessorFactory.CreateFormatConverter();

        // Assert
        Assert.NotNull(converter1);
        Assert.NotNull(converter2);
        Assert.NotSame(converter1, converter2); // 应该是不同的实例
        Assert.Equal(converter1.GetType(), converter2.GetType()); // 但类型相同
    }

    /// <summary>
    /// 测试所有处理器类型枚举值都有对应的实现
    /// </summary>
    [Fact]
    public void CreateProcessor_AllEnumValues_HaveImplementations()
    {
        // Arrange
        var allTypes = Enum.GetValues<ImageProcessorType>();

        // Act & Assert
        foreach (var type in allTypes)
        {
            var processor = ImageProcessorFactory.CreateProcessor(type);
            Assert.NotNull(processor);
            Assert.IsAssignableFrom<IImageProcessor>(processor);
        }
    }
}
