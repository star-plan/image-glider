using System;
using System.IO;
using System.Threading.Tasks;
using ImageGlider;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 批量转换命令
    /// </summary>
    public class BatchConvertCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "batch-convert";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "批量转换图片文件";

        /// <summary>
        /// 执行批量转换命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
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
                        sourceExt = ParseStringParameter(args, ref i, "--source-ext");
                        break;
                    case "--target-ext":
                    case "-te":
                        targetExt = ParseStringParameter(args, ref i, "--target-ext");
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
                    case "--quality":
                    case "-q":
                        quality = ParseIntParameter(args, ref i, "--quality", 1, 100);
                        break;
                }
            }

            // 验证必需参数
            ValidateRequiredParameter(sourceExt, "--source-ext");
            ValidateRequiredParameter(targetExt, "--target-ext");

            using var logger = new LoggingService(logDir);
            
            logger.Log("开始批量转换...");
            logger.Log($"源目录: {sourceDir}");
            logger.Log($"输出目录: {outputDir}");
            logger.Log($"源扩展名: {sourceExt}");
            logger.Log($"目标扩展名: {targetExt}");
            logger.Log($"JPEG 质量: {quality}");

            var result = ImageConverter.BatchConvert(sourceDir, outputDir, sourceExt!, targetExt!, quality);

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                logger.LogError($"批量转换失败: {result.ErrorMessage}");
                Console.WriteLine($"批量转换失败: {result.ErrorMessage}");
                return 1;
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
            
            return result.FailedConversions > 0 ? 1 : 0;
        }

        /// <summary>
        /// 显示批量转换命令的帮助信息
        /// </summary>
        public override void ShowHelp()
        {
            Console.WriteLine("用法:");
            Console.WriteLine("  ImageGlider.Cli batch-convert --source-ext <源扩展名> --target-ext <目标扩展名> [选项]");
            Console.WriteLine();
            Console.WriteLine("选项:");
            Console.WriteLine("  --source-ext, -se    源文件扩展名 (必需, 如: .jfif)");
            Console.WriteLine("  --target-ext, -te    目标文件扩展名 (必需, 如: .jpeg)");
            Console.WriteLine("  --source-dir, -sd    源目录路径 (默认: 当前目录)");
            Console.WriteLine("  --output-dir, -od    输出目录路径 (默认: ./output)");
            Console.WriteLine("  --quality, -q        JPEG 质量 (1-100, 默认: 90)");
            Console.WriteLine("  --log-dir, -ld       日志目录路径 (默认: ./log)");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  ImageGlider.Cli batch-convert -se .jfif -te .jpeg -q 90");
        }
    }
}