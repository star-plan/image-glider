using System;
using System.Threading.Tasks;
using ImageGlider;
using ImageGlider.Enums;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 单文件尺寸调整命令
    /// </summary>
    public class ResizeCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "resize";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "调整单个图片文件尺寸";

        /// <summary>
        /// 执行尺寸调整命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
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
                        sourceFile = ParseStringParameter(args, ref i, "--source");
                        break;
                    case "--target":
                    case "-t":
                        targetFile = ParseStringParameter(args, ref i, "--target");
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
            ValidateRequiredParameter(sourceFile, "--source");
            ValidateRequiredParameter(targetFile, "--target");

            if (width == null && height == null)
            {
                throw new ArgumentException("宽度和高度至少需要指定一个");
            }

            Console.WriteLine($"正在调整尺寸: {sourceFile} -> {targetFile}");
            Console.WriteLine($"目标尺寸: {width?.ToString() ?? "auto"} x {height?.ToString() ?? "auto"}");
            Console.WriteLine($"调整模式: {resizeMode}");
            Console.WriteLine($"JPEG 质量: {quality}");

            var success = ImageConverter.ResizeImage(sourceFile!, targetFile!, width, height, resizeMode, quality);

            if (success)
            {
                Console.WriteLine("✓ 尺寸调整成功！");
                return 0;
            }
            else
            {
                Console.WriteLine("✗ 尺寸调整失败！");
                return 1;
            }
        }

        /// <summary>
        /// 显示尺寸调整命令的帮助信息
        /// </summary>
        public override void ShowHelp()
        {
            Console.WriteLine("用法:");
            Console.WriteLine("  imageglider resize --source <源文件> --target <目标文件> [选项]");
        Console.WriteLine();
        Console.WriteLine("选项:");
        Console.WriteLine("  --source, -s     源文件路径 (必需)");
        Console.WriteLine("  --target, -t     目标文件路径 (必需)");
        Console.WriteLine("  --width, -w      目标宽度 (像素)");
        Console.WriteLine("  --height, -h     目标高度 (像素)");
        Console.WriteLine("  --mode, -m       调整模式 (keep|stretch|crop, 默认: keep)");
        Console.WriteLine("  --quality, -q    JPEG 质量 (1-100, 默认: 90)");
        Console.WriteLine();
        Console.WriteLine("调整模式说明:");
        Console.WriteLine("  keep     保持宽高比，可能会有空白区域");
        Console.WriteLine("  stretch  拉伸到指定尺寸，可能会变形");
        Console.WriteLine("  crop     裁剪到指定尺寸，保持宽高比");
        Console.WriteLine();
        Console.WriteLine("示例:");
        Console.WriteLine("  imageglider resize -s input.jpg -t output.jpg -w 800 -h 600 --mode keep");
        }
    }
}