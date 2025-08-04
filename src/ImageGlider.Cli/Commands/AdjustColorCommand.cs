using System;
using System.Threading.Tasks;
using ImageGlider;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 单文件颜色调整命令
    /// </summary>
    public class AdjustColorCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "adjust";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "调整单个图片文件的颜色（亮度、对比度、饱和度、色相、伽马值）";

        /// <summary>
        /// 执行颜色调整命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
        {
            string? sourceFile = null;
            string? targetFile = null;
            float brightness = 0;
            float contrast = 0;
            float saturation = 0;
            float hue = 0;
            float gamma = 1.0f;
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
                    case "--brightness":
                    case "-b":
                        brightness = ParseFloatParameter(args, ref i, "--brightness", -100, 100);
                        break;
                    case "--contrast":
                    case "-c":
                        contrast = ParseFloatParameter(args, ref i, "--contrast", -100, 100);
                        break;
                    case "--saturation":
                    case "--sat":
                        saturation = ParseFloatParameter(args, ref i, "--saturation", -100, 100);
                        break;
                    case "--hue":
                    case "-h":
                        hue = ParseFloatParameter(args, ref i, "--hue", -180, 180);
                        break;
                    case "--gamma":
                    case "-g":
                        gamma = ParseFloatParameter(args, ref i, "--gamma", 0.1f, 3.0f);
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

            // 检查是否至少指定了一个调整参数
            if (Math.Abs(brightness) < 0.01f && Math.Abs(contrast) < 0.01f && 
                Math.Abs(saturation) < 0.01f && Math.Abs(hue) < 0.01f && 
                Math.Abs(gamma - 1.0f) < 0.01f)
            {
                Console.WriteLine("警告: 未指定任何颜色调整参数，将直接复制原图");
            }

            Console.WriteLine($"正在调整颜色: {sourceFile} -> {targetFile}");
            if (Math.Abs(brightness) > 0.01f) Console.WriteLine($"亮度调整: {brightness:+0;-0;0}");
            if (Math.Abs(contrast) > 0.01f) Console.WriteLine($"对比度调整: {contrast:+0;-0;0}");
            if (Math.Abs(saturation) > 0.01f) Console.WriteLine($"饱和度调整: {saturation:+0;-0;0}");
            if (Math.Abs(hue) > 0.01f) Console.WriteLine($"色相调整: {hue:+0;-0;0}°");
            if (Math.Abs(gamma - 1.0f) > 0.01f) Console.WriteLine($"伽马值调整: {gamma:F2}");
            Console.WriteLine($"JPEG 质量: {quality}");

            var success = ImageConverter.AdjustColor(sourceFile!, targetFile!, brightness, contrast, saturation, hue, gamma, quality);

            if (success)
            {
                Console.WriteLine("✓ 颜色调整成功！");
                return 0;
            }
            else
            {
                Console.WriteLine("✗ 颜色调整失败！");
                return 1;
            }
        }

        /// <summary>
        /// 显示命令帮助信息
        /// </summary>
        public override void ShowHelp()
        {
            Console.WriteLine($"使用方法: imageglider {Name} [选项]");
            Console.WriteLine();
            Console.WriteLine(Description);
            Console.WriteLine();
            Console.WriteLine("选项:");
            Console.WriteLine("  -s, --source <文件>      源图片文件路径 (必需)");
            Console.WriteLine("  -t, --target <文件>      目标图片文件路径 (必需)");
            Console.WriteLine("  -b, --brightness <值>    亮度调整 (-100 到 100，默认: 0)");
            Console.WriteLine("  -c, --contrast <值>      对比度调整 (-100 到 100，默认: 0)");
            Console.WriteLine("      --saturation <值>    饱和度调整 (-100 到 100，默认: 0)");
            Console.WriteLine("      --sat <值>           饱和度调整的简写");
            Console.WriteLine("  -h, --hue <值>           色相调整 (-180 到 180，默认: 0)");
            Console.WriteLine("  -g, --gamma <值>         伽马值调整 (0.1 到 3.0，默认: 1.0)");
            Console.WriteLine("  -q, --quality <值>       JPEG 质量 (1-100，默认: 90)");
            Console.WriteLine("      --help               显示此帮助信息");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine($"  imageglider {Name} -s input.jpg -t output.jpg --brightness 20");
            Console.WriteLine($"  imageglider {Name} -s photo.png -t adjusted.png -b 10 -c 15 --sat 20");
            Console.WriteLine($"  imageglider {Name} -s image.jpg -t result.jpg --hue 45 --gamma 1.2");
        }
    }
}