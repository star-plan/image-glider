using ImageGlider;
using ImageGlider.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using ResizeMode = ImageGlider.Enums.ResizeMode;

/// <summary>
/// ImageGlider 示例程序，展示核心类库的典型用法
/// </summary>
class Program
{
    /// <summary>
    /// 程序主入口点
    /// </summary>
    /// <param name="args">命令行参数</param>
    static void Main(string[] args)
    {
        Console.WriteLine("=== ImageGlider 示例程序 ===");
        Console.WriteLine();

        // 示例1：单文件转换
        Console.WriteLine("示例1：单文件转换");
        SingleFileConversionExample();
        Console.WriteLine();

        // 示例2：批量转换
        Console.WriteLine("示例2：批量转换");
        BatchConversionExample();
        Console.WriteLine();

        // 示例3：图像尺寸调整
        Console.WriteLine("示例3：图像尺寸调整");
        ResizeImageExample();
        Console.WriteLine();

        // 示例4：批量尺寸调整
        Console.WriteLine("示例4：批量尺寸调整");
        BatchResizeExample();
        Console.WriteLine();

        // 创建测试PNG图片
        Console.WriteLine("创建测试PNG图片");
        CreateTestPngImage();
        Console.WriteLine();

        // 示例5：图像压缩优化
        Console.WriteLine("示例5：图像压缩优化");
        CompressImageExample();
        Console.WriteLine();

        // 示例6：批量压缩优化
        Console.WriteLine("示例6：批量压缩优化");
        BatchCompressExample();
        Console.WriteLine();

        // 示例7：使用日志服务
        Console.WriteLine("示例7：使用日志服务");
        LoggingServiceExample();
        Console.WriteLine();

        Console.WriteLine("示例程序执行完成！");
    }

    /// <summary>
    /// 单文件转换示例
    /// </summary>
    private static void SingleFileConversionExample()
    {
        // 创建示例目录和文件
        var exampleDir = Path.Combine(Directory.GetCurrentDirectory(), "example");
        Directory.CreateDirectory(exampleDir);

        // 注意：这里只是演示API用法，实际使用时需要有真实的图片文件
        var sourceFile = Path.Combine(exampleDir, "sample.jfif");
        var targetFile = Path.Combine(exampleDir, "sample.jpeg");

        Console.WriteLine($"源文件: {sourceFile}");
        Console.WriteLine($"目标文件: {targetFile}");

        // 检查源文件是否存在
        if (!File.Exists(sourceFile))
        {
            Console.WriteLine("源文件不存在，跳过转换示例。");
            Console.WriteLine("提示：请将图片文件放置在 example 目录下并命名为 sample.jfif 来测试转换功能。");
            return;
        }

        // 执行转换
        bool success = ImageConverter.ConvertImage(sourceFile, targetFile, quality: 85);
        
        if (success)
        {
            Console.WriteLine("✓ 转换成功！");
        }
        else
        {
            Console.WriteLine("✗ 转换失败！");
        }
    }

    /// <summary>
    /// 批量转换示例
    /// </summary>
    private static void BatchConversionExample()
    {
        var sourceDir = Path.Combine(Directory.GetCurrentDirectory(), "example", "input");
        var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "example", "output");
        
        // 创建示例目录
        Directory.CreateDirectory(sourceDir);
        Directory.CreateDirectory(outputDir);

        Console.WriteLine($"源目录: {sourceDir}");
        Console.WriteLine($"输出目录: {outputDir}");
        Console.WriteLine("转换规则: .jfif -> .jpeg");

        // 执行批量转换
        var result = ImageConverter.BatchConvert(
            sourceDirectory: sourceDir,
            outputDirectory: outputDir,
            sourceExtension: ".jfif",
            targetExtension: ".jpeg",
            quality: 90
        );

        // 显示转换结果
        Console.WriteLine($"总文件数: {result.TotalFiles}");
        Console.WriteLine($"成功转换: {result.SuccessfulConversions}");
        Console.WriteLine($"转换失败: {result.FailedConversions}");
        
        if (!string.IsNullOrEmpty(result.ErrorMessage))
        {
            Console.WriteLine($"错误信息: {result.ErrorMessage}");
        }

        if (result.TotalFiles == 0)
        {
            Console.WriteLine("提示：请将 .jfif 格式的图片文件放置在 example/input 目录下来测试批量转换功能。");
        }
    }

    /// <summary>
    /// 日志服务使用示例
    /// </summary>
    private static void LoggingServiceExample()
    {
        var logDir = Path.Combine(Directory.GetCurrentDirectory(), "example", "logs");
        
        Console.WriteLine($"日志目录: {logDir}");

        // 使用日志服务
        using var logger = new LoggingService(logDir, "Example");
        
        logger.Log("这是一条普通日志信息");
        logger.LogInfo("这是一条信息日志");
        logger.LogWarning("这是一条警告日志");
        logger.LogError("这是一条错误日志");
        
        // 演示异常日志
        try
        {
            throw new InvalidOperationException("这是一个示例异常");
        }
        catch (Exception ex)
        {
            logger.LogError("捕获到异常", ex);
        }

        Console.WriteLine($"日志已写入文件: {logger.LogFilePath}");
        Console.WriteLine("提示：日志同时输出到控制台和文件。");
    }

    /// <summary>
    /// 创建测试PNG图片
    /// </summary>
    private static void CreateTestPngImage()
    {
        try
        {
            // 创建一个简单的小图片
            using var image = new Image<Rgba32>(200, 200);
            image.Mutate(x => x.BackgroundColor(Color.Red));
            
            var filePath = "test_image.png";
            image.Save(filePath, new PngEncoder());
            Console.WriteLine($"测试PNG图片已创建: {filePath}");
            
            var fileInfo = new FileInfo(filePath);
            Console.WriteLine($"文件大小: {fileInfo.Length:N0} 字节");
            
            // 创建一个已经高度压缩的小PNG图片（用于测试文件大小检查）
            using var smallImage = new Image<Rgba32>(10, 10);
            smallImage.Mutate(x => x.BackgroundColor(Color.Blue));
            
            var smallFilePath = "small_test.png";
            // 使用最高压缩级别保存
            var encoder = new PngEncoder
            {
                CompressionLevel = SixLabors.ImageSharp.Formats.Png.PngCompressionLevel.BestCompression
            };
            smallImage.Save(smallFilePath, encoder);
            
            var smallFileInfo = new FileInfo(smallFilePath);
            Console.WriteLine($"小测试图片已创建: {smallFilePath}");
            Console.WriteLine($"小图片文件大小: {smallFileInfo.Length:N0} 字节");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"创建测试图片失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 图像尺寸调整示例
    /// </summary>
    private static void ResizeImageExample()
    {
        // 创建示例目录
        var exampleDir = Path.Combine(Directory.GetCurrentDirectory(), "example");
        Directory.CreateDirectory(exampleDir);

        var sourceFile = Path.Combine(exampleDir, "sample.jpeg");
        var resizedFile = Path.Combine(exampleDir, "sample_resized.jpeg");

        Console.WriteLine($"源文件: {sourceFile}");
        Console.WriteLine($"调整后文件: {resizedFile}");
        Console.WriteLine("目标尺寸: 800x600 (保持宽高比)");

        // 检查源文件是否存在
        if (!File.Exists(sourceFile))
        {
            Console.WriteLine("源文件不存在，跳过尺寸调整示例。");
            Console.WriteLine("提示：请将图片文件放置在 example 目录下并命名为 sample.jpeg 来测试尺寸调整功能。");
            return;
        }

        // 执行尺寸调整
        bool success = ImageConverter.ResizeImage(
            sourceFile, 
            resizedFile, 
            width: 800, 
            height: 600, 
            resizeMode: ResizeMode.KeepAspectRatio, 
            quality: 90
        );
        
        if (success)
        {
            Console.WriteLine("✓ 尺寸调整成功！");
        }
        else
        {
            Console.WriteLine("✗ 尺寸调整失败！");
        }
    }

    /// <summary>
    /// 批量尺寸调整示例
    /// </summary>
    private static void BatchResizeExample()
    {
        var sourceDir = Path.Combine(Directory.GetCurrentDirectory(), "example", "input");
        var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "example", "resized");
        
        // 创建示例目录
        Directory.CreateDirectory(sourceDir);
        Directory.CreateDirectory(outputDir);

        Console.WriteLine($"源目录: {sourceDir}");
        Console.WriteLine($"输出目录: {outputDir}");
        Console.WriteLine("调整规则: .jpeg 文件调整为 1024x768 (保持宽高比)");

        // 执行批量尺寸调整
        var result = ImageConverter.BatchResize(
            sourceDirectory: sourceDir,
            outputDirectory: outputDir,
            sourceExtension: ".jpeg",
            width: 1024,
            height: 768,
            resizeMode: ResizeMode.KeepAspectRatio,
            quality: 85
        );

        // 显示调整结果
        Console.WriteLine($"总文件数: {result.TotalFiles}");
        Console.WriteLine($"成功调整: {result.SuccessfulConversions}");
        Console.WriteLine($"调整失败: {result.FailedConversions}");
        
        if (!string.IsNullOrEmpty(result.ErrorMessage))
        {
            Console.WriteLine($"错误信息: {result.ErrorMessage}");
        }

        if (result.TotalFiles == 0)
        {
            Console.WriteLine("提示：请将 .jpeg 格式的图片文件放置在 example/input 目录下来测试批量尺寸调整功能。");
        }
    }

    /// <summary>
    /// 图像压缩优化示例
    /// </summary>
    private static void CompressImageExample()
    {
        // 创建示例目录
        var exampleDir = Path.Combine(Directory.GetCurrentDirectory(), "example");
        Directory.CreateDirectory(exampleDir);

        var sourceFile = Path.Combine(exampleDir, "sample.jpg");
        var targetFile = Path.Combine(exampleDir, "sample_compressed.jpg");

        Console.WriteLine($"源文件: {sourceFile}");
        Console.WriteLine($"目标文件: {targetFile}");

        // 检查源文件是否存在
        if (!File.Exists(sourceFile))
        {
            Console.WriteLine("源文件不存在，跳过压缩示例。");
            Console.WriteLine("提示：请将图片文件放置在 example 目录下并命名为 sample.jpg 来测试压缩功能。");
            return;
        }

        // 执行压缩（压缩级别60，不保留元数据）
        bool success = ImageConverter.CompressImage(
            sourceFilePath: sourceFile,
            targetFilePath: targetFile,
            compressionLevel: 60,
            preserveMetadata: false
        );
        
        if (success)
        {
            Console.WriteLine("✓ 压缩成功！");
            
            // 显示文件大小对比
            if (File.Exists(sourceFile) && File.Exists(targetFile))
            {
                var originalSize = new FileInfo(sourceFile).Length;
                var compressedSize = new FileInfo(targetFile).Length;
                var compressionRatio = (1.0 - (double)compressedSize / originalSize) * 100;
                
                Console.WriteLine($"原始文件大小: {originalSize:N0} 字节");
                Console.WriteLine($"压缩后大小: {compressedSize:N0} 字节");
                Console.WriteLine($"压缩率: {compressionRatio:F1}%");
            }
        }
        else
        {
            Console.WriteLine("✗ 压缩失败！");
        }
    }

    /// <summary>
    /// 批量压缩优化示例
    /// </summary>
    private static void BatchCompressExample()
    {
        // 创建示例目录结构
        var exampleDir = Path.Combine(Directory.GetCurrentDirectory(), "example");
        var inputDir = Path.Combine(exampleDir, "input");
        var outputDir = Path.Combine(exampleDir, "compressed_output");
        
        Directory.CreateDirectory(inputDir);
        Directory.CreateDirectory(outputDir);

        Console.WriteLine($"源目录: {inputDir}");
        Console.WriteLine($"输出目录: {outputDir}");
        Console.WriteLine("压缩规则: .jpg 文件压缩级别50，保留元数据");

        // 执行批量压缩
        var result = ImageConverter.BatchCompress(
            sourceDirectory: inputDir,
            outputDirectory: outputDir,
            sourceExtension: ".jpg",
            compressionLevel: 50,
            preserveMetadata: true
        );

        // 显示压缩结果
        Console.WriteLine($"总文件数: {result.TotalFiles}");
        Console.WriteLine($"成功压缩: {result.SuccessfulConversions}");
        Console.WriteLine($"压缩失败: {result.FailedConversions}");
        
        if (!string.IsNullOrEmpty(result.ErrorMessage))
        {
            Console.WriteLine($"错误信息: {result.ErrorMessage}");
        }

        if (result.TotalFiles == 0)
        {
            Console.WriteLine("提示：请将 .jpg 格式的图片文件放置在 example/input 目录下来测试批量压缩功能。");
        }
    }
}
