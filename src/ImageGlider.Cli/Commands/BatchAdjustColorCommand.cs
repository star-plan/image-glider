using System;
using System.Threading.Tasks;
using ImageGlider;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 批量颜色调整命令
    /// </summary>
    public class BatchAdjustColorCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "batch-adjust";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "批量调整指定目录下图片文件的颜色（亮度、对比度、饱和度、色相、伽马值）";

        /// <summary>
        /// 执行批量颜色调整命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
        {
            string? sourceDirectory = null;
            string? outputDirectory = null;
            string sourceExtension = "jpg";
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
                    case "--source-dir":
                    case "-sd":
                        sourceDirectory = ParseStringParameter(args, ref i, "--source-dir");
                        break;
                    case "--output-dir":
                    case "-od":
                        outputDirectory = ParseStringParameter(args, ref i, "--output-dir");
                        break;
                    case "--extension":
                    case "-e":
                        sourceExtension = ParseStringParameter(args, ref i, "--extension").TrimStart('.');
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
            ValidateRequiredParameter(sourceDirectory, "--source-dir");
            ValidateRequiredParameter(outputDirectory, "--output-dir");

            // 检查是否至少指定了一个调整参数
            if (Math.Abs(brightness) < 0.01f && Math.Abs(contrast) < 0.01f && 
                Math.Abs(saturation) < 0.01f && Math.Abs(hue) < 0.01f && 
                Math.Abs(gamma - 1.0f) < 0.01f)
            {
                Console.WriteLine("警告: 未指定任何颜色调整参数，将直接复制原图");
            }

            Console.WriteLine($"正在批量调整颜色:");
            Console.WriteLine($"源目录: {sourceDirectory}");
            Console.WriteLine($"输出目录: {outputDirectory}");
            Console.WriteLine($"文件扩展名: *.{sourceExtension}");
            if (Math.Abs(brightness) > 0.01f) Console.WriteLine($"亮度调整: {brightness:+0;-0;0}");
            if (Math.Abs(contrast) > 0.01f) Console.WriteLine($"对比度调整: {contrast:+0;-0;0}");
            if (Math.Abs(saturation) > 0.01f) Console.WriteLine($"饱和度调整: {saturation:+0;-0;0}");
            if (Math.Abs(hue) > 0.01f) Console.WriteLine($"色相调整: {hue:+0;-0;0}°");
            if (Math.Abs(gamma - 1.0f) > 0.01f) Console.WriteLine($"伽马值调整: {gamma:F2}");
            Console.WriteLine($"JPEG 质量: {quality}");
            Console.WriteLine();

            var result = ImageConverter.BatchAdjustColor(sourceDirectory!, outputDirectory!, sourceExtension, 
                brightness, contrast, saturation, hue, gamma, quality);

            // 显示结果
            Console.WriteLine($"处理完成!");
            Console.WriteLine($"总文件数: {result.TotalFiles}");
            Console.WriteLine($"成功处理: {result.SuccessfulConversions}");
            Console.WriteLine($"处理失败: {result.FailedConversions}");

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                Console.WriteLine($"错误信息: {result.ErrorMessage}");
            }

            if (result.SuccessfulConversions > 0)
            {
                Console.WriteLine("✓ 批量颜色调整完成！");
                return 0;
            }
            else
            {
                Console.WriteLine("✗ 批量颜色调整失败！");
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
            Console.WriteLine("  -sd, --source-dir <目录>   源图片目录路径 (必需)");
            Console.WriteLine("  -od, --output-dir <目录>   输出图片目录路径 (必需)");
            Console.WriteLine("  -e,  --extension <扩展名>  源文件扩展名 (默认: jpg)");
            Console.WriteLine("  -b,  --brightness <值>     亮度调整 (-100 到 100，默认: 0)");
            Console.WriteLine("  -c,  --contrast <值>       对比度调整 (-100 到 100，默认: 0)");
            Console.WriteLine("       --saturation <值>     饱和度调整 (-100 到 100，默认: 0)");
            Console.WriteLine("       --sat <值>            饱和度调整的简写");
            Console.WriteLine("  -h,  --hue <值>            色相调整 (-180 到 180，默认: 0)");
            Console.WriteLine("  -g,  --gamma <值>          伽马值调整 (0.1 到 3.0，默认: 1.0)");
            Console.WriteLine("  -q,  --quality <值>        JPEG 质量 (1-100，默认: 90)");
            Console.WriteLine("       --help                显示此帮助信息");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine($"  imageglider {Name} -sd ./photos -od ./adjusted --brightness 20");
            Console.WriteLine($"  imageglider {Name} -sd C:\\Images -od C:\\Output -e png -b 10 -c 15");
            Console.WriteLine($"  imageglider {Name} -sd ./input -od ./output --sat 30 --hue 45 --gamma 1.2");
        }
    }
}