using System;
using System.Threading.Tasks;
using ImageGlider;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 批量缩略图生成命令
    /// </summary>
    public class BatchThumbnailCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "batch-thumbnail";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "批量生成指定目录下图片文件的缩略图";

        /// <summary>
        /// 执行批量缩略图生成命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
        {
            string sourceDirectory = ".";
            string outputDirectory = "./thumbnails";
            string sourceExtension = ".jpg";
            int maxSize = 150;
            int quality = 90;

            // 解析参数
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--source-dir":
                    case "-sd":
                        sourceDirectory = ParseStringParameter(args, ref i, "--source-dir");
                        break;
                    case "--output-dir":
                    case "-od":
                        outputDirectory = ParseStringParameter(args, ref i, "--output-dir");
                        break;
                    case "--extension":
                    case "-ext":
                        sourceExtension = ParseStringParameter(args, ref i, "--extension");
                        break;
                    case "--size":
                    case "--max-size":
                    case "-sz":
                        maxSize = ParseIntParameter(args, ref i, "--size", 1, int.MaxValue);
                        break;
                    case "--quality":
                    case "-q":
                        quality = ParseIntParameter(args, ref i, "--quality", 1, 100);
                        break;
                }
            }

            Console.WriteLine($"正在批量生成缩略图...");
            Console.WriteLine($"源目录: {sourceDirectory}");
            Console.WriteLine($"输出目录: {outputDirectory}");
            Console.WriteLine($"文件扩展名: {sourceExtension}");
            Console.WriteLine($"最大边长: {maxSize} 像素");
            Console.WriteLine($"JPEG 质量: {quality}");
            Console.WriteLine();

            var result = ImageConverter.BatchGenerateThumbnails(
                sourceDirectory,
                outputDirectory,
                sourceExtension,
                maxSize,
                quality);

            // 显示结果
            Console.WriteLine($"处理完成！");
            Console.WriteLine($"总文件数: {result.TotalFiles}");
            Console.WriteLine($"成功: {result.SuccessfulConversions}");
            Console.WriteLine($"失败: {result.FailedConversions}");

            if (result.SuccessfulFiles.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("成功生成缩略图的文件:");
                foreach (var file in result.SuccessfulFiles)
                {
                    Console.WriteLine($"  ✓ {file}");
                }
            }

            if (result.FailedFiles.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("生成缩略图失败的文件:");
                foreach (var file in result.FailedFiles)
                {
                    Console.WriteLine($"  ✗ {file}");
                }
            }

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                Console.WriteLine();
                Console.WriteLine($"错误信息: {result.ErrorMessage}");
                return 1;
            }

            return result.FailedConversions > 0 ? 1 : 0;
        }

        /// <summary>
        /// 显示批量缩略图生成命令的帮助信息
        /// </summary>
        public override void ShowHelp()
        {
            Console.WriteLine("用法:");
            Console.WriteLine("  ImageGlider.Cli batch-thumbnail [选项]");
            Console.WriteLine();
            Console.WriteLine("选项:");
            Console.WriteLine("  --source-dir, -sd    源目录路径 (默认: 当前目录)");
            Console.WriteLine("  --output-dir, -od    输出目录路径 (默认: ./thumbnails)");
            Console.WriteLine("  --extension, -ext    源文件扩展名 (默认: .jpg)");
            Console.WriteLine("  --size, -sz          缩略图最大边长 (像素, 默认: 150)");
            Console.WriteLine("  --quality, -q        JPEG 质量 (1-100, 默认: 90)");
            Console.WriteLine();
            Console.WriteLine("说明:");
            Console.WriteLine("  批量处理指定目录下的所有匹配文件");
            Console.WriteLine("  生成的缩略图文件名会添加 '_thumb' 后缀");
            Console.WriteLine("  缩略图会按照长边等比缩放，保持原始图片的宽高比");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  ImageGlider.Cli batch-thumbnail -sd ./photos -od ./thumbs -ext .png --size 200");
            Console.WriteLine("  ImageGlider.Cli batch-thumbnail -sd C:\\Images -od C:\\Thumbnails -ext .jpg -sz 100");
        }
    }
}