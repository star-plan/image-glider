using System;
using System.Threading.Tasks;
using ImageGlider;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 单文件转换命令
    /// </summary>
    public class ConvertCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "convert";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "转换单个图片文件";

        /// <summary>
        /// 执行转换命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
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
                        sourceFile = ParseStringParameter(args, ref i, "--source");
                        break;
                    case "--target":
                    case "-t":
                        targetFile = ParseStringParameter(args, ref i, "--target");
                        break;
                    case "--quality":
                    case "-q":
                        quality = ParseIntParameter(args, ref i, "--quality", 1, 100);
                        break;
                }
            }

            // 验证必需参数
            ValidateRequiredParameter(sourceFile, "--source");
            ValidateRequiredParameter(targetFile, "--target");

            Console.WriteLine($"正在转换: {sourceFile} -> {targetFile}");
            Console.WriteLine($"JPEG 质量: {quality}");

            var success = ImageConverter.ConvertImage(sourceFile!, targetFile!, quality);

            if (success)
            {
                Console.WriteLine("✓ 转换成功！");
                return 0;
            }
            else
            {
                Console.WriteLine("✗ 转换失败！");
                return 1;
            }
        }

        /// <summary>
        /// 显示转换命令的帮助信息
        /// </summary>
        public override void ShowHelp()
        {
            Console.WriteLine("用法:");
            Console.WriteLine("  ImageGlider.Cli convert --source <源文件> --target <目标文件> [--quality <质量>]");
            Console.WriteLine();
            Console.WriteLine("选项:");
            Console.WriteLine("  --source, -s     源文件路径 (必需)");
            Console.WriteLine("  --target, -t     目标文件路径 (必需)");
            Console.WriteLine("  --quality, -q    JPEG 质量 (1-100, 默认: 90)");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  ImageGlider.Cli convert -s image.jfif -t image.jpeg -q 85");
        }
    }
}