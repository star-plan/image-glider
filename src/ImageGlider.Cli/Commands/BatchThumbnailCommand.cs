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
            Console.WriteLine("  imageglider batch-thumbnail [选项]");
        Console.WriteLine();
        Console.WriteLine("参数:");
        Console.WriteLine("  -sd, --source-dir <目录>    源目录路径 (默认: 当前目录)");
        Console.WriteLine("  -od, --output-dir <目录>    输出目录路径 (默认: ./thumbnails)");
        Console.WriteLine("  -ext, --extension <扩展名>  处理的文件扩展名 (如 .jpg，默认: 所有支持的格式)");
        Console.WriteLine("  -p, --pattern <模式>        文件名匹配模式 (如 *.jpg，默认: *.*)");
        Console.WriteLine("  -r, --recursive             递归处理子目录");
        Console.WriteLine("  --size <尺寸>               缩略图尺寸 (像素，默认: 150)");
        Console.WriteLine("  -q, --quality <质量>        JPEG 质量 (1-100，默认: 85)");
        Console.WriteLine("  --help                      显示此帮助信息");
        Console.WriteLine();
        Console.WriteLine("说明:");
        Console.WriteLine("  - 生成正方形缩略图，保持宽高比");
        Console.WriteLine("  - 自动裁剪到指定尺寸的正方形");
        Console.WriteLine();
        Console.WriteLine("示例:");
        Console.WriteLine("  imageglider batch-thumbnail -sd ./photos -od ./thumbs -ext .png --size 200");
        Console.WriteLine("  imageglider batch-thumbnail -sd C:\\Images -od C:\\Thumbnails -ext .jpg -sz 100");
        }
    }
}