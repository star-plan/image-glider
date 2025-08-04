using System;
using System.Threading.Tasks;
using ImageGlider;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 单文件裁剪命令
    /// </summary>
    public class CropCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "crop";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "裁剪单个图片文件";

        /// <summary>
        /// 执行裁剪命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
        {
            string sourceFile = null;
            string targetFile = null;
            int? x = null, y = null, width = null, height = null;
            float? xPercent = null, yPercent = null, widthPercent = null, heightPercent = null;
            bool centerCrop = false;
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
            if (string.IsNullOrEmpty(sourceFile))
            {
                throw new ArgumentException("必须指定源文件路径 (--source)");
            }

            if (string.IsNullOrEmpty(targetFile))
            {
                throw new ArgumentException("必须指定目标文件路径 (--target)");
            }

            Console.WriteLine($"开始裁剪图片: {sourceFile} -> {targetFile}");

            bool result;

            // 根据参数类型选择裁剪方式
            if (centerCrop)
            {
                // 中心裁剪
                if (!width.HasValue || !height.HasValue)
                {
                    throw new ArgumentException("中心裁剪模式需要指定宽度和高度 (--width, --height)");
                }
                result = ImageConverter.CropImageCenter(sourceFile, targetFile, width.Value, height.Value, quality);
                Console.WriteLine($"中心裁剪: 尺寸 {width}x{height}");
            }
            else if (xPercent.HasValue && yPercent.HasValue && widthPercent.HasValue && heightPercent.HasValue)
            {
                // 百分比裁剪
                result = ImageConverter.CropImageByPercent(sourceFile, targetFile, (int)xPercent.Value, (int)yPercent.Value, (int)widthPercent.Value, (int)heightPercent.Value, quality);
                Console.WriteLine($"百分比裁剪: 位置 ({xPercent}%, {yPercent}%), 尺寸 ({widthPercent}%, {heightPercent}%)");
            }
            else if (x.HasValue && y.HasValue && width.HasValue && height.HasValue)
            {
                // 像素坐标裁剪
                result = ImageConverter.CropImage(sourceFile, targetFile, x.Value, y.Value, width.Value, height.Value, quality);
                Console.WriteLine($"像素裁剪: 位置 ({x}, {y}), 尺寸 {width}x{height}");
            }
            else
            {
                throw new ArgumentException("必须指定裁剪参数：\n" +
                    "- 像素裁剪: --x, --y, --width, --height\n" +
                    "- 百分比裁剪: --x-percent, --y-percent, --width-percent, --height-percent\n" +
                    "- 中心裁剪: --center, --width, --height");
            }

            if (result)
            {
                Console.WriteLine("裁剪完成！");
                return 0;
            }
            else
            {
                Console.WriteLine("裁剪失败！");
                return 1;
            }
        }

        /// <summary>
        /// 显示裁剪命令的帮助信息
        /// </summary>
        public override void ShowHelp()
        {
            Console.WriteLine("用法:");
            Console.WriteLine("  ImageGlider.Cli crop --source <源文件> --target <目标文件> [裁剪参数] [选项]");
            Console.WriteLine();
            Console.WriteLine("必需参数:");
            Console.WriteLine("  --source, -s     源文件路径");
            Console.WriteLine("  --target, -t     目标文件路径");
            Console.WriteLine();
            Console.WriteLine("裁剪参数 (三选一):");
            Console.WriteLine("  像素坐标裁剪:");
            Console.WriteLine("    --x              裁剪区域左上角X坐标（像素）");
            Console.WriteLine("    --y              裁剪区域左上角Y坐标（像素）");
            Console.WriteLine("    --width, -w      裁剪区域宽度（像素）");
            Console.WriteLine("    --height, -h     裁剪区域高度（像素）");
            Console.WriteLine();
            Console.WriteLine("  百分比裁剪:");
            Console.WriteLine("    --x-percent, -xp      裁剪区域左上角X坐标百分比（0-100）");
            Console.WriteLine("    --y-percent, -yp      裁剪区域左上角Y坐标百分比（0-100）");
            Console.WriteLine("    --width-percent, -wp  裁剪区域宽度百分比（0-100）");
            Console.WriteLine("    --height-percent, -hp 裁剪区域高度百分比（0-100）");
            Console.WriteLine();
            Console.WriteLine("  中心裁剪:");
            Console.WriteLine("    --center, -c     启用中心裁剪模式");
            Console.WriteLine("    --width, -w      裁剪区域宽度（像素）");
            Console.WriteLine("    --height, -h     裁剪区域高度（像素）");
            Console.WriteLine();
            Console.WriteLine("可选参数:");
            Console.WriteLine("  --quality, -q    JPEG 质量 (1-100, 默认: 90)");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  # 像素坐标裁剪");
            Console.WriteLine("  ImageGlider.Cli crop -s input.jpg -t output.jpg --x 100 --y 50 -w 400 -h 300");
            Console.WriteLine();
            Console.WriteLine("  # 百分比裁剪");
            Console.WriteLine("  ImageGlider.Cli crop -s input.jpg -t output.jpg -xp 25 -yp 25 -wp 50 -hp 50");
            Console.WriteLine();
            Console.WriteLine("  # 中心裁剪");
            Console.WriteLine("  ImageGlider.Cli crop -s input.jpg -t output.jpg --center -w 800 -h 600");
        }
    }
}