using System;
using System.IO;
using System.Threading.Tasks;
using ImageGlider;
using ImageGlider.Enums;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 批量尺寸调整命令
    /// </summary>
    public class BatchResizeCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "batch-resize";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "批量调整图片文件尺寸";

        /// <summary>
        /// 执行批量尺寸调整命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
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
                        sourceExt = ParseStringParameter(args, ref i, "--source-ext");
                        break;
                    case "--source-dir":
                    case "-sd":
                        sourceDir = ParseStringParameter(args, ref i, "--source-dir");
                        break;
                    case "--output-dir":
                    case "-od":
                        outputDir = ParseStringParameter(args, ref i, "--output-dir");
                        break;
                    case "--log-dir":
                    case "-ld":
                        logDir = ParseStringParameter(args, ref i, "--log-dir");
                        break;
                    case "--width":
                    case "-w":
                        width = ParseIntParameter(args, ref i, "--width", 1, int.MaxValue);
                        break;
                    case "--height":
                    case "-h":
                        height = ParseIntParameter(args, ref i, "--height", 1, int.MaxValue);
                        break;
                    case "--mode":
                    case "-m":
                        var mode = ParseStringParameter(args, ref i, "--mode").ToLowerInvariant();
                        resizeMode = mode switch
                        {
                            "keep" => ResizeMode.KeepAspectRatio,
                            "stretch" => ResizeMode.Stretch,
                            "crop" => ResizeMode.Crop,
                            _ => throw new ArgumentException($"无效的调整模式: {mode}。支持的模式: keep, stretch, crop")
                        };
                        break;
                    case "--quality":
                    case "-q":
                        quality = ParseIntParameter(args, ref i, "--quality", 1, 100);
                        break;
                }
            }

            // 验证必需参数
            ValidateRequiredParameter(sourceExt, "--source-ext");

            if (width == null && height == null)
            {
                throw new ArgumentException("宽度和高度至少需要指定一个");
            }

            using var logger = new LoggingService(logDir);
            
            logger.Log("开始批量尺寸调整...");
            logger.Log($"源目录: {sourceDir}");
            logger.Log($"输出目录: {outputDir}");
            logger.Log($"源扩展名: {sourceExt}");
            logger.Log($"目标尺寸: {width?.ToString() ?? "auto"} x {height?.ToString() ?? "auto"}");
            logger.Log($"调整模式: {resizeMode}");
            logger.Log($"JPEG 质量: {quality}");

            var result = ImageConverter.BatchResize(sourceDir, outputDir, sourceExt!, width, height, resizeMode, quality);

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                logger.LogError($"批量尺寸调整失败: {result.ErrorMessage}");
                Console.WriteLine($"批量尺寸调整失败: {result.ErrorMessage}");
                return 1;
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
            
            return result.FailedConversions > 0 ? 1 : 0;
        }

        /// <summary>
        /// 显示批量尺寸调整命令的帮助信息
        /// </summary>
        public override void ShowHelp()
        {
            Console.WriteLine("用法:");
            Console.WriteLine("  imageglider batch-resize --source-ext <源扩展名> [选项]");
        Console.WriteLine();
        Console.WriteLine("参数:");
        Console.WriteLine("  -se, --source-ext <扩展名>  源文件扩展名 (如 .jpg，必需)");
        Console.WriteLine("  -te, --target-ext <扩展名>  目标文件扩展名 (如 .png，默认: 与源相同)");
        Console.WriteLine("  -sd, --source-dir <目录>    源目录路径 (默认: 当前目录)");
        Console.WriteLine("  -td, --target-dir <目录>    目标目录路径 (默认: 源目录)");
        Console.WriteLine("  -w, --width <像素>          目标宽度");
        Console.WriteLine("  -h, --height <像素>         目标高度");
        Console.WriteLine("  --mode <模式>               调整模式 (keep|fill|crop|stretch，默认: keep)");
        Console.WriteLine("  --bg-color <颜色>           背景颜色 (十六进制，如 #FFFFFF，默认: 透明)");
        Console.WriteLine("  -p, --pattern <模式>        文件名匹配模式 (如 *.jpg，默认: *源扩展名)");
        Console.WriteLine("  -r, --recursive             递归处理子目录");
        Console.WriteLine("  --quality <质量>            JPEG 质量 (1-100，默认: 85)");
        Console.WriteLine("  --help                      显示此帮助信息");
        Console.WriteLine();
        Console.WriteLine("调整模式说明:");
        Console.WriteLine("  keep    - 保持宽高比，可能有留白");
        Console.WriteLine("  fill    - 保持宽高比，裁剪多余部分");
        Console.WriteLine("  crop    - 保持宽高比，居中裁剪");
        Console.WriteLine("  stretch - 拉伸到指定尺寸，可能变形");
        Console.WriteLine();
        Console.WriteLine("示例:");
        Console.WriteLine("  imageglider batch-resize -se .jpg -w 1920 --mode keep");
        }
    }
}