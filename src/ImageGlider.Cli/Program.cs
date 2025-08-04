using ImageGlider;
using ImageGlider.Enums;

/// <summary>
/// ImageGlider 命令行工具入口点
/// </summary>
class Program
{
    /// <summary>
    /// 程序主入口点
    /// </summary>
    /// <param name="args">命令行参数</param>
    static void Main(string[] args)
    {
        Console.WriteLine("=== ImageGlider 命令行工具 ===");
        Console.WriteLine();

        if (args.Length == 0)
        {
            ShowHelp();
            return;
        }

        var command = args[0].ToLowerInvariant();

        switch (command)
        {
            case "convert":
                HandleConvertCommand(args);
                break;
            case "batch":
                HandleBatchCommand(args);
                break;
            case "resize":
                HandleResizeCommand(args);
                break;
            case "batch-resize":
                HandleBatchResizeCommand(args);
                break;
            case "help":
            case "--help":
            case "-h":
                ShowHelp();
                break;
            default:
                Console.WriteLine($"未知命令: {command}");
                ShowHelp();
                break;
        }
    }

    /// <summary>
    /// 显示帮助信息
    /// </summary>
    private static void ShowHelp()
    {
        Console.WriteLine("用法:");
        Console.WriteLine("  ImageGlider.Cli convert --source <源文件> --target <目标文件> [--quality <质量>]");
        Console.WriteLine("  ImageGlider.Cli batch --source-ext <源扩展名> --target-ext <目标扩展名> [选项]");
        Console.WriteLine("  ImageGlider.Cli resize --source <源文件> --target <目标文件> [选项]");
        Console.WriteLine("  ImageGlider.Cli batch-resize --source-ext <源扩展名> [选项]");
        Console.WriteLine();
        Console.WriteLine("命令:");
        Console.WriteLine("  convert       转换单个图片文件");
        Console.WriteLine("  batch         批量转换图片文件");
        Console.WriteLine("  resize        调整单个图片文件尺寸");
        Console.WriteLine("  batch-resize  批量调整图片文件尺寸");
        Console.WriteLine();
        Console.WriteLine("convert 选项:");
        Console.WriteLine("  --source, -s     源文件路径 (必需)");
        Console.WriteLine("  --target, -t     目标文件路径 (必需)");
        Console.WriteLine("  --quality, -q    JPEG 质量 (1-100, 默认: 90)");
        Console.WriteLine();
        Console.WriteLine("batch 选项:");
        Console.WriteLine("  --source-ext, -se    源文件扩展名 (必需, 如: .jfif)");
        Console.WriteLine("  --target-ext, -te    目标文件扩展名 (必需, 如: .jpeg)");
        Console.WriteLine("  --source-dir, -sd    源目录路径 (默认: 当前目录)");
        Console.WriteLine("  --output-dir, -od    输出目录路径 (默认: ./output)");
        Console.WriteLine("  --quality, -q        JPEG 质量 (1-100, 默认: 90)");
        Console.WriteLine("  --log-dir, -ld       日志目录路径 (默认: ./log)");
        Console.WriteLine();
        Console.WriteLine("resize 选项:");
        Console.WriteLine("  --source, -s     源文件路径 (必需)");
        Console.WriteLine("  --target, -t     目标文件路径 (必需)");
        Console.WriteLine("  --width, -w      目标宽度 (像素)");
        Console.WriteLine("  --height, -h     目标高度 (像素)");
        Console.WriteLine("  --mode, -m       调整模式 (keep|stretch|crop, 默认: keep)");
        Console.WriteLine("  --quality, -q    JPEG 质量 (1-100, 默认: 90)");
        Console.WriteLine();
        Console.WriteLine("batch-resize 选项:");
        Console.WriteLine("  --source-ext, -se    源文件扩展名 (必需, 如: .jpg)");
        Console.WriteLine("  --source-dir, -sd    源目录路径 (默认: 当前目录)");
        Console.WriteLine("  --output-dir, -od    输出目录路径 (默认: ./output)");
        Console.WriteLine("  --width, -w          目标宽度 (像素)");
        Console.WriteLine("  --height, -h         目标高度 (像素)");
        Console.WriteLine("  --mode, -m           调整模式 (keep|stretch|crop, 默认: keep)");
        Console.WriteLine("  --quality, -q        JPEG 质量 (1-100, 默认: 90)");
        Console.WriteLine("  --log-dir, -ld       日志目录路径 (默认: ./log)");
        Console.WriteLine();
        Console.WriteLine("示例:");
        Console.WriteLine("  ImageGlider.Cli convert -s image.jfif -t image.jpeg -q 85");
        Console.WriteLine("  ImageGlider.Cli batch -se .jfif -te .jpeg -q 90");
        Console.WriteLine("  ImageGlider.Cli resize -s input.jpg -t output.jpg -w 800 -h 600 --mode keep");
        Console.WriteLine("  ImageGlider.Cli batch-resize -se .jpg -w 1920 --mode keep");
    }

    /// <summary>
    /// 处理单文件转换命令
    /// </summary>
    /// <param name="args">命令行参数</param>
    private static void HandleConvertCommand(string[] args)
    {
        string? sourceFile = null;
        string? targetFile = null;
        int quality = 90;

        // 解析参数
        for (int i = 1; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--source":
                case "-s":
                    if (i + 1 < args.Length)
                        sourceFile = args[++i];
                    break;
                case "--target":
                case "-t":
                    if (i + 1 < args.Length)
                        targetFile = args[++i];
                    break;
                case "--quality":
                case "-q":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int q))
                        quality = Math.Clamp(q, 1, 100);
                    break;
            }
        }

        // 验证必需参数
        if (string.IsNullOrEmpty(sourceFile) || string.IsNullOrEmpty(targetFile))
        {
            Console.WriteLine("错误: 缺少必需参数 --source 和 --target");
            ShowHelp();
            return;
        }

        Console.WriteLine($"正在转换: {sourceFile} -> {targetFile}");
        Console.WriteLine($"JPEG 质量: {quality}");

        if (ImageConverter.ConvertImage(sourceFile, targetFile, quality))
        {
            Console.WriteLine("✓ 转换成功！");
        }
        else
        {
            Console.WriteLine("✗ 转换失败！");
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// 处理批量转换命令
    /// </summary>
    /// <param name="args">命令行参数</param>
    private static void HandleBatchCommand(string[] args)
    {
        string? sourceExt = null;
        string? targetExt = null;
        string sourceDir = Directory.GetCurrentDirectory();
        string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
        string logDir = Path.Combine(Directory.GetCurrentDirectory(), "log");
        int quality = 90;

        // 解析参数
        for (int i = 1; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--source-ext":
                case "-se":
                    if (i + 1 < args.Length)
                        sourceExt = args[++i];
                    break;
                case "--target-ext":
                case "-te":
                    if (i + 1 < args.Length)
                        targetExt = args[++i];
                    break;
                case "--source-dir":
                case "-sd":
                    if (i + 1 < args.Length)
                        sourceDir = args[++i];
                    break;
                case "--output-dir":
                case "-od":
                    if (i + 1 < args.Length)
                        outputDir = args[++i];
                    break;
                case "--log-dir":
                case "-ld":
                    if (i + 1 < args.Length)
                        logDir = args[++i];
                    break;
                case "--quality":
                case "-q":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int q))
                        quality = Math.Clamp(q, 1, 100);
                    break;
            }
        }

        // 验证必需参数
        if (string.IsNullOrEmpty(sourceExt) || string.IsNullOrEmpty(targetExt))
        {
            Console.WriteLine("错误: 缺少必需参数 --source-ext 和 --target-ext");
            ShowHelp();
            return;
        }

        using var logger = new LoggingService(logDir);
        
        logger.Log("开始批量转换...");
        logger.Log($"源目录: {sourceDir}");
        logger.Log($"输出目录: {outputDir}");
        logger.Log($"源扩展名: {sourceExt}");
        logger.Log($"目标扩展名: {targetExt}");
        logger.Log($"JPEG 质量: {quality}");

        var result = ImageConverter.BatchConvert(sourceDir, outputDir, sourceExt, targetExt, quality);

        if (!string.IsNullOrEmpty(result.ErrorMessage))
        {
            logger.LogError($"批量转换失败: {result.ErrorMessage}");
            Environment.Exit(1);
            return;
        }

        logger.Log($"转换完成！总文件数: {result.TotalFiles}, 成功: {result.SuccessfulConversions}, 失败: {result.FailedConversions}");
        
        if (result.SuccessfulFiles.Count > 0)
        {
            logger.Log("成功转换的文件:");
            foreach (var file in result.SuccessfulFiles)
            {
                logger.Log($"  - {file}");
            }
        }

        if (result.FailedFiles.Count > 0)
        {
            logger.LogWarning("转换失败的文件:");
            foreach (var file in result.FailedFiles)
            {
                logger.LogWarning($"  - {file}");
            }
        }

        Console.WriteLine($"\n转换完成！请查看输出目录: {outputDir}");
        Console.WriteLine($"日志文件: {logger.LogFilePath}");
    }

    /// <summary>
    /// 处理单文件尺寸调整命令
    /// </summary>
    /// <param name="args">命令行参数</param>
    private static void HandleResizeCommand(string[] args)
    {
        string? sourceFile = null;
        string? targetFile = null;
        int? width = null;
        int? height = null;
        var resizeMode = ResizeMode.KeepAspectRatio;
        int quality = 90;

        // 解析参数
        for (int i = 1; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--source":
                case "-s":
                    if (i + 1 < args.Length)
                        sourceFile = args[++i];
                    break;
                case "--target":
                case "-t":
                    if (i + 1 < args.Length)
                        targetFile = args[++i];
                    break;
                case "--width":
                case "-w":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int w))
                        width = w;
                    break;
                case "--height":
                case "-h":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int h))
                        height = h;
                    break;
                case "--mode":
                case "-m":
                    if (i + 1 < args.Length)
                    {
                        var mode = args[++i].ToLowerInvariant();
                        resizeMode = mode switch
                        {
                            "keep" => ResizeMode.KeepAspectRatio,
                            "stretch" => ResizeMode.Stretch,
                            "crop" => ResizeMode.Crop,
                            _ => ResizeMode.KeepAspectRatio
                        };
                    }
                    break;
                case "--quality":
                case "-q":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int q))
                        quality = Math.Clamp(q, 1, 100);
                    break;
            }
        }

        // 验证必需参数
        if (string.IsNullOrEmpty(sourceFile) || string.IsNullOrEmpty(targetFile))
        {
            Console.WriteLine("错误: 缺少必需参数 --source 和 --target");
            ShowHelp();
            return;
        }

        if (width == null && height == null)
        {
            Console.WriteLine("错误: 宽度和高度至少需要指定一个");
            ShowHelp();
            return;
        }

        Console.WriteLine($"正在调整尺寸: {sourceFile} -> {targetFile}");
        Console.WriteLine($"目标尺寸: {width?.ToString() ?? "auto"} x {height?.ToString() ?? "auto"}");
        Console.WriteLine($"调整模式: {resizeMode}");
        Console.WriteLine($"JPEG 质量: {quality}");

        if (ImageConverter.ResizeImage(sourceFile, targetFile, width, height, resizeMode, quality))
        {
            Console.WriteLine("✓ 尺寸调整成功！");
        }
        else
        {
            Console.WriteLine("✗ 尺寸调整失败！");
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// 处理批量尺寸调整命令
    /// </summary>
    /// <param name="args">命令行参数</param>
    private static void HandleBatchResizeCommand(string[] args)
    {
        string? sourceExt = null;
        string sourceDir = Directory.GetCurrentDirectory();
        string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
        string logDir = Path.Combine(Directory.GetCurrentDirectory(), "log");
        int? width = null;
        int? height = null;
        var resizeMode = ResizeMode.KeepAspectRatio;
        int quality = 90;

        // 解析参数
        for (int i = 1; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--source-ext":
                case "-se":
                    if (i + 1 < args.Length)
                        sourceExt = args[++i];
                    break;
                case "--source-dir":
                case "-sd":
                    if (i + 1 < args.Length)
                        sourceDir = args[++i];
                    break;
                case "--output-dir":
                case "-od":
                    if (i + 1 < args.Length)
                        outputDir = args[++i];
                    break;
                case "--log-dir":
                case "-ld":
                    if (i + 1 < args.Length)
                        logDir = args[++i];
                    break;
                case "--width":
                case "-w":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int w))
                        width = w;
                    break;
                case "--height":
                case "-h":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int h))
                        height = h;
                    break;
                case "--mode":
                case "-m":
                    if (i + 1 < args.Length)
                    {
                        var mode = args[++i].ToLowerInvariant();
                        resizeMode = mode switch
                        {
                            "keep" => ResizeMode.KeepAspectRatio,
                            "stretch" => ResizeMode.Stretch,
                            "crop" => ResizeMode.Crop,
                            _ => ResizeMode.KeepAspectRatio
                        };
                    }
                    break;
                case "--quality":
                case "-q":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int q))
                        quality = Math.Clamp(q, 1, 100);
                    break;
            }
        }

        // 验证必需参数
        if (string.IsNullOrEmpty(sourceExt))
        {
            Console.WriteLine("错误: 缺少必需参数 --source-ext");
            ShowHelp();
            return;
        }

        if (width == null && height == null)
        {
            Console.WriteLine("错误: 宽度和高度至少需要指定一个");
            ShowHelp();
            return;
        }

        using var logger = new LoggingService(logDir);
        
        logger.Log("开始批量尺寸调整...");
        logger.Log($"源目录: {sourceDir}");
        logger.Log($"输出目录: {outputDir}");
        logger.Log($"源扩展名: {sourceExt}");
        logger.Log($"目标尺寸: {width?.ToString() ?? "auto"} x {height?.ToString() ?? "auto"}");
        logger.Log($"调整模式: {resizeMode}");
        logger.Log($"JPEG 质量: {quality}");

        var result = ImageConverter.BatchResize(sourceDir, outputDir, sourceExt, width, height, resizeMode, quality);

        if (!string.IsNullOrEmpty(result.ErrorMessage))
        {
            logger.LogError($"批量尺寸调整失败: {result.ErrorMessage}");
            Environment.Exit(1);
            return;
        }

        logger.Log($"尺寸调整完成！总文件数: {result.TotalFiles}, 成功: {result.SuccessfulConversions}, 失败: {result.FailedConversions}");
        
        if (result.SuccessfulFiles.Count > 0)
        {
            logger.Log("成功调整的文件:");
            foreach (var file in result.SuccessfulFiles)
            {
                logger.Log($"  - {file}");
            }
        }

        if (result.FailedFiles.Count > 0)
        {
            logger.LogWarning("调整失败的文件:");
            foreach (var file in result.FailedFiles)
            {
                logger.LogWarning($"  - {file}");
            }
        }

        Console.WriteLine($"\n尺寸调整完成！请查看输出目录: {outputDir}");
        Console.WriteLine($"日志文件: {logger.LogFilePath}");
    }
}
