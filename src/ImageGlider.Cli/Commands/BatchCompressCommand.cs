using System;
using System.IO;
using System.Threading.Tasks;
using ImageGlider;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 批量压缩命令
    /// </summary>
    public class BatchCompressCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "batch-compress";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "批量压缩优化图片文件";

        /// <summary>
        /// 执行批量压缩命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
        {
            string? sourceExt = null;
            string sourceDir = Directory.GetCurrentDirectory();
            string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
            string logDir = Path.Combine(Directory.GetCurrentDirectory(), "log");
            int compressionLevel = 75;
            bool preserveMetadata = false;

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
                    case "--level":
                    case "-l":
                        compressionLevel = ParseIntParameter(args, ref i, "--level", 1, 100);
                        break;
                    case "--preserve-meta":
                    case "-pm":
                        preserveMetadata = true;
                        break;
                }
            }

            // 验证必需参数
            ValidateRequiredParameter(sourceExt, "--source-ext");

            using var logger = new LoggingService(logDir);
            
            logger.Log("开始批量压缩...");
            logger.Log($"源目录: {sourceDir}");
            logger.Log($"输出目录: {outputDir}");
            logger.Log($"源扩展名: {sourceExt}");
            logger.Log($"压缩级别: {compressionLevel}");
            logger.Log($"保留元数据: {preserveMetadata}");

            var result = ImageConverter.BatchCompress(sourceDir, outputDir, sourceExt!, compressionLevel, preserveMetadata);

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                logger.LogError($"批量压缩失败: {result.ErrorMessage}");
                Console.WriteLine($"批量压缩失败: {result.ErrorMessage}");
                return 1;
            }

            logger.Log($"压缩完成！总文件数: {result.TotalFiles}, 成功: {result.SuccessfulConversions}, 失败: {result.FailedConversions}");
            
            if (result.SuccessfulFiles.Count > 0)
            {
                logger.Log("成功压缩的文件:");
                foreach (var file in result.SuccessfulFiles)
                {
                    logger.Log($"  - {file}");
                }
            }

            if (result.FailedFiles.Count > 0)
            {
                logger.LogWarning("压缩失败的文件:");
                foreach (var file in result.FailedFiles)
                {
                    logger.LogWarning($"  - {file}");
                }
            }

            Console.WriteLine($"\n压缩完成！请查看输出目录: {outputDir}");
            Console.WriteLine($"成功压缩: {result.SuccessfulConversions} 个文件");
            if (result.FailedConversions > 0)
            {
                Console.WriteLine($"压缩失败: {result.FailedConversions} 个文件");
            }
            Console.WriteLine($"日志文件: {logger.LogFilePath}");
            
            return result.FailedConversions > 0 ? 1 : 0;
        }

        /// <summary>
        /// 显示批量压缩命令的帮助信息
        /// </summary>
        public override void ShowHelp()
        {
            Console.WriteLine("用法:");
            Console.WriteLine("  imageglider batch-compress --source-ext <源扩展名> [选项]");
        Console.WriteLine();
        Console.WriteLine("选项:");
        Console.WriteLine("  --source-ext, -se    源文件扩展名 (必需, 如: .jpg)");
        Console.WriteLine("  --source-dir, -sd    源目录路径 (默认: 当前目录)");
        Console.WriteLine("  --output-dir, -od    输出目录路径 (默认: ./output)");
        Console.WriteLine("  --level, -l          压缩级别 (1-100, 数值越小压缩越强, 默认: 75)");
        Console.WriteLine("  --preserve-meta, -pm 保留元数据 (默认: false)");
        Console.WriteLine("  --log-dir, -ld       日志目录路径 (默认: ./log)");
        Console.WriteLine();
        Console.WriteLine("压缩级别说明:");
        Console.WriteLine("  1-30     高压缩率，文件较小，质量较低");
        Console.WriteLine("  31-70    中等压缩率，平衡文件大小和质量");
        Console.WriteLine("  71-100   低压缩率，文件较大，质量较高");
        Console.WriteLine();
        Console.WriteLine("示例:");
        Console.WriteLine("  imageglider batch-compress -se .jpg -l 50 --preserve-meta");
        }
    }
}