using System;
using System.Threading.Tasks;
using ImageGlider;
using ImageGlider.Core;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 批量裁剪命令
    /// </summary>
    public class BatchCropCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "batch-crop";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "批量裁剪指定目录下的图片文件";

        /// <summary>
        /// 执行批量裁剪命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
        {
            string sourceDir = null;
            string targetDir = null;
            string pattern = "*";
            int? x = null, y = null, width = null, height = null;
            float? xPercent = null, yPercent = null, widthPercent = null, heightPercent = null;
            bool centerCrop = false;
            int quality = 90;

            // 解析参数
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--source-dir":
                    case "-sd":
                        sourceDir = ParseStringParameter(args, ref i, "--source-dir");
                        break;
                    case "--target-dir":
                    case "-td":
                        targetDir = ParseStringParameter(args, ref i, "--target-dir");
                        break;
                    case "--pattern":
                    case "-p":
                        pattern = ParseStringParameter(args, ref i, "--pattern");
                        break;
                    case "--x":
                        x = ParseIntParameter(args, ref i, "--x");
                        break;
                    case "--y":
                        y = ParseIntParameter(args, ref i, "--y");
                        break;
                    case "--width":
                    case "-w":
                        width = ParseIntParameter(args, ref i, "--width");
                        break;
                    case "--height":
                    case "-h":
                        height = ParseIntParameter(args, ref i, "--height");
                        break;
                    case "--x-percent":
                    case "-xp":
                        xPercent = ParseFloatParameter(args, ref i, "--x-percent");
                        break;
                    case "--y-percent":
                    case "-yp":
                        yPercent = ParseFloatParameter(args, ref i, "--y-percent");
                        break;
                    case "--width-percent":
                    case "-wp":
                        widthPercent = ParseFloatParameter(args, ref i, "--width-percent");
                        break;
                    case "--height-percent":
                    case "-hp":
                        heightPercent = ParseFloatParameter(args, ref i, "--height-percent");
                        break;
                    case "--center":
                    case "-c":
                        centerCrop = true;
                        break;
                    case "--quality":
                    case "-q":
                        quality = ParseIntParameter(args, ref i, "--quality");
                        if (quality < 1 || quality > 100)
                        {
                            throw new ArgumentException("质量参数必须在 1-100 范围内");
                        }
                        break;
                    default:
                        throw new ArgumentException($"未知参数: {args[i]}");
                }
            }

            // 验证必需参数
            if (string.IsNullOrEmpty(sourceDir))
            {
                throw new ArgumentException("必须指定源目录路径 (--source-dir)");
            }

            if (string.IsNullOrEmpty(targetDir))
            {
                throw new ArgumentException("必须指定目标目录路径 (--target-dir)");
            }

            Console.WriteLine($"开始批量裁剪图片:");
            Console.WriteLine($"源目录: {sourceDir}");
            Console.WriteLine($"目标目录: {targetDir}");
            Console.WriteLine($"文件模式: {pattern}");

            ConversionResult result;

            // 根据参数类型选择裁剪方式
            if (centerCrop)
            {
                // 中心裁剪
                if (!width.HasValue || !height.HasValue)
                {
                    throw new ArgumentException("中心裁剪模式需要指定宽度和高度 (--width, --height)");
                }
                result = ImageConverter.BatchCropCenter(sourceDir, targetDir, pattern, width.Value, height.Value, quality);
                Console.WriteLine($"中心裁剪: 尺寸 {width}x{height}");
            }
            else if (xPercent.HasValue && yPercent.HasValue && widthPercent.HasValue && heightPercent.HasValue)
            {
                // 百分比裁剪
                result = ImageConverter.BatchCrop(sourceDir, targetDir, pattern, (int)xPercent.Value, (int)yPercent.Value, (int)widthPercent.Value, (int)heightPercent.Value, quality);
                Console.WriteLine($"百分比裁剪: 位置 ({xPercent}%, {yPercent}%), 尺寸 ({widthPercent}%, {heightPercent}%)");
            }
            else if (x.HasValue && y.HasValue && width.HasValue && height.HasValue)
            {
                // 像素坐标裁剪
                result = ImageConverter.BatchCrop(sourceDir, targetDir, pattern, x.Value, y.Value, width.Value, height.Value, quality);
                Console.WriteLine($"像素裁剪: 位置 ({x}, {y}), 尺寸 {width}x{height}");
            }
            else
            {
                throw new ArgumentException("必须指定裁剪参数：\n" +
                    "- 像素裁剪: --x, --y, --width, --height\n" +
                    "- 百分比裁剪: --x-percent, --y-percent, --width-percent, --height-percent\n" +
                    "- 中心裁剪: --center, --width, --height");
            }

            if (result.SuccessfulConversions > 0)
            {
                Console.WriteLine("批量裁剪完成！");
                return 0;
            }
            else
            {
                Console.WriteLine("批量裁剪失败！");
                return 1;
            }
        }

        /// <summary>
        /// 显示批量裁剪命令的帮助信息
        /// </summary>
        public override void ShowHelp()
        {
            Console.WriteLine("用法:");
            Console.WriteLine("  imageglider batch-crop --source-dir <源目录> --target-dir <目标目录> [裁剪参数] [选项]");
            Console.WriteLine();
            Console.WriteLine("参数:");
            Console.WriteLine("  -sd, --source-dir <目录>    源目录路径 (必需)");
            Console.WriteLine("  -td, --target-dir <目录>    目标目录路径 (必需)");
            Console.WriteLine("  -x, --x <像素>              裁剪起始 X 坐标");
            Console.WriteLine("  -y, --y <像素>              裁剪起始 Y 坐标");
            Console.WriteLine("  -w, --width <像素>          裁剪宽度");
            Console.WriteLine("  -h, --height <像素>         裁剪高度");
            Console.WriteLine("  -xp, --x-percent <百分比>   裁剪起始 X 坐标百分比 (0-100)");
            Console.WriteLine("  -yp, --y-percent <百分比>   裁剪起始 Y 坐标百分比 (0-100)");
            Console.WriteLine("  -wp, --width-percent <百分比> 裁剪宽度百分比 (0-100)");
            Console.WriteLine("  -hp, --height-percent <百分比> 裁剪高度百分比 (0-100)");
            Console.WriteLine("  --center                    居中裁剪 (需要指定宽度和高度)");
            Console.WriteLine("  -p, --pattern <模式>        文件名匹配模式 (如 *.jpg，默认: *.*)");
            Console.WriteLine("  -r, --recursive             递归处理子目录");
            Console.WriteLine("  --quality <质量>            JPEG 质量 (1-100，默认: 85)");
            Console.WriteLine("  --help                      显示此帮助信息");
            Console.WriteLine();
            Console.WriteLine("说明:");
            Console.WriteLine("  - 可以使用像素坐标或百分比坐标");
            Console.WriteLine("  - 百分比坐标相对于原图尺寸计算");
            Console.WriteLine("  - 使用 --center 选项可以居中裁剪指定尺寸");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  imageglider batch-crop -sd ./input -td ./output --x 100 --y 50 -w 400 -h 300");
            Console.WriteLine("  # 使用百分比坐标裁剪中心 50% 区域");
            Console.WriteLine("  imageglider batch-crop -sd ./input -td ./output -xp 25 -yp 25 -wp 50 -hp 50");
            Console.WriteLine("  # 居中裁剪为 800x600 尺寸");
            Console.WriteLine("  imageglider batch-crop -sd ./input -td ./output --center -w 800 -h 600 -p *.jpg");
        }
    }
}