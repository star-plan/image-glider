using System;
using System.Threading.Tasks;
using ImageGlider;
using ImageGlider.Core;
using ImageGlider.Enums;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 单文件水印命令
    /// </summary>
    public class WatermarkCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "watermark";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "为单个图片文件添加水印";

        /// <summary>
        /// 执行水印命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
        {
            string sourceFile = null;
            string targetFile = null;
            string text = null;
            string watermarkImage = null;
            WatermarkPosition position = WatermarkPosition.BottomRight;
            int opacity = 50;
            int fontSize = 24;
            string fontColor = "#FFFFFF";
            float scale = 1.0f;
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
                    case "--text":
                        text = ParseStringParameter(args, ref i, "--text");
                        break;
                    case "--image":
                    case "-i":
                        watermarkImage = ParseStringParameter(args, ref i, "--image");
                        break;
                    case "--position":
                    case "-p":
                        var positionStr = ParseStringParameter(args, ref i, "--position");
                        if (!Enum.TryParse<WatermarkPosition>(positionStr, true, out position))
                        {
                            throw new ArgumentException($"无效的水印位置: {positionStr}。有效值: TopLeft, TopCenter, TopRight, MiddleLeft, Center, MiddleRight, BottomLeft, BottomCenter, BottomRight");
                        }
                        break;
                    case "--opacity":
                    case "-o":
                        opacity = ParseIntParameter(args, ref i, "--opacity", 0, 100);
                        break;
                    case "--font-size":
                    case "-fs":
                        fontSize = ParseIntParameter(args, ref i, "--font-size", 8, 200);
                        break;
                    case "--font-color":
                    case "-fc":
                        fontColor = ParseStringParameter(args, ref i, "--font-color");
                        break;
                    case "--scale":
                        scale = ParseFloatParameter(args, ref i, "--scale", 0.1f, 2.0f);
                        break;
                    case "--quality":
                    case "-q":
                        quality = ParseIntParameter(args, ref i, "--quality", 1, 100);
                        break;
                    default:
                        throw new ArgumentException($"未知参数: {args[i]}");
                }
            }

            // 验证必需参数
            ValidateRequiredParameter(sourceFile, "--source");
            ValidateRequiredParameter(targetFile, "--target");

            if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(watermarkImage))
            {
                throw new ArgumentException("必须指定文本水印 (--text) 或图片水印 (--image)");
            }

            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(watermarkImage))
            {
                throw new ArgumentException("不能同时指定文本水印和图片水印");
            }

            Console.WriteLine($"正在为图片添加水印...");
            Console.WriteLine($"源文件: {sourceFile}");
            Console.WriteLine($"目标文件: {targetFile}");

            bool success;
            if (!string.IsNullOrEmpty(text))
            {
                Console.WriteLine($"水印文本: {text}");
                Console.WriteLine($"位置: {position}");
                Console.WriteLine($"透明度: {opacity}%");
                Console.WriteLine($"字体大小: {fontSize}");
                Console.WriteLine($"字体颜色: {fontColor}");
                
                success = ImageConverter.AddTextWatermark(sourceFile, targetFile, text, position, opacity, fontSize, fontColor, quality);
            }
            else
            {
                Console.WriteLine($"水印图片: {watermarkImage}");
                Console.WriteLine($"位置: {position}");
                Console.WriteLine($"透明度: {opacity}%");
                Console.WriteLine($"缩放比例: {scale}");
                
                success = ImageConverter.AddImageWatermark(sourceFile, targetFile, watermarkImage, position, opacity, scale, quality);
            }

            if (success)
            {
                Console.WriteLine("✅ 水印添加成功！");
                return 0;
            }
            else
            {
                Console.WriteLine("❌ 水印添加失败！");
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
            Console.WriteLine("  --text <文本>            文本水印内容");
            Console.WriteLine("  -i, --image <文件>       图片水印文件路径");
            Console.WriteLine("  -p, --position <位置>    水印位置 (默认: BottomRight)");
            Console.WriteLine("                           可选值: TopLeft, TopCenter, TopRight,");
            Console.WriteLine("                                  MiddleLeft, Center, MiddleRight,");
            Console.WriteLine("                                  BottomLeft, BottomCenter, BottomRight");
            Console.WriteLine("  -o, --opacity <数值>     透明度 0-100 (默认: 50)");
            Console.WriteLine("  -fs, --font-size <数值>  字体大小 8-200 (默认: 24，仅文本水印)");
            Console.WriteLine("  -fc, --font-color <颜色> 字体颜色，十六进制格式 (默认: #FFFFFF，仅文本水印)");
            Console.WriteLine("  --scale <数值>           缩放比例 0.1-2.0 (默认: 1.0，仅图片水印)");
            Console.WriteLine("  -q, --quality <数值>     JPEG 质量 1-100 (默认: 90)");
            Console.WriteLine("  -h, --help               显示此帮助信息");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine($"  imageglider {Name} -s input.jpg -t output.jpg --text \"© 2024 我的公司\"");
            Console.WriteLine($"  imageglider {Name} -s input.jpg -t output.jpg -i logo.png -p TopRight -o 30");
            Console.WriteLine($"  imageglider {Name} -s input.jpg -t output.jpg --text \"水印文本\" -p Center -fs 32 -fc #FF0000");
        }
    }
}