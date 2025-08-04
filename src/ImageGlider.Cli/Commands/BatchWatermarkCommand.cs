using System;
using System.Threading.Tasks;
using ImageGlider;
using ImageGlider.Core;
using ImageGlider.Enums;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 批量水印命令
    /// </summary>
    public class BatchWatermarkCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "batch-watermark";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "批量为图片文件添加水印";

        /// <summary>
        /// 执行批量水印命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
        {
            string sourceDirectory = null;
            string outputDirectory = null;
            string sourceExtension = null;
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
            ValidateRequiredParameter(sourceDirectory, "--source-dir");
            ValidateRequiredParameter(outputDirectory, "--output-dir");

            if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(watermarkImage))
            {
                throw new ArgumentException("必须指定文本水印 (--text) 或图片水印 (--image)");
            }

            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(watermarkImage))
            {
                throw new ArgumentException("不能同时指定文本水印和图片水印");
            }

            Console.WriteLine($"正在批量添加水印...");
            Console.WriteLine($"源目录: {sourceDirectory}");
            Console.WriteLine($"输出目录: {outputDirectory}");
            Console.WriteLine($"文件扩展名: {sourceExtension ?? "所有支持的格式"}");

            ConversionResult result;
            if (!string.IsNullOrEmpty(text))
            {
                Console.WriteLine($"水印文本: {text}");
                Console.WriteLine($"位置: {position}");
                Console.WriteLine($"透明度: {opacity}%");
                Console.WriteLine($"字体大小: {fontSize}");
                Console.WriteLine($"字体颜色: {fontColor}");
                
                result = ImageConverter.BatchAddTextWatermark(sourceDirectory, outputDirectory, sourceExtension, text, position, opacity, fontSize, fontColor, quality);
            }
            else
            {
                Console.WriteLine($"水印图片: {watermarkImage}");
                Console.WriteLine($"位置: {position}");
                Console.WriteLine($"透明度: {opacity}%");
                Console.WriteLine($"缩放比例: {scale}");
                
                result = ImageConverter.BatchAddImageWatermark(sourceDirectory, outputDirectory, sourceExtension, watermarkImage, position, opacity, scale, quality);
            }

            Console.WriteLine();
            Console.WriteLine("=== 批量水印处理结果 ===");
            Console.WriteLine($"总文件数: {result.TotalFiles}");
            Console.WriteLine($"成功: {result.SuccessfulFiles}");
            Console.WriteLine($"失败: {result.FailedFiles}");

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                Console.WriteLine($"错误信息: {result.ErrorMessage}");
            }

            if (result.IsSuccess)
            {
                Console.WriteLine("✅ 批量水印添加完成！");
                return 0;
            }
            else
            {
                Console.WriteLine("❌ 批量水印添加过程中出现错误！");
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
            Console.WriteLine("  -sd, --source-dir <目录>  源图片目录路径 (必需)");
            Console.WriteLine("  -od, --output-dir <目录>  输出图片目录路径 (必需)");
            Console.WriteLine("  -ext, --extension <扩展名> 源文件扩展名 (可选，如 .jpg)");
            Console.WriteLine("  --text <文本>             文本水印内容");
            Console.WriteLine("  -i, --image <文件>        图片水印文件路径");
            Console.WriteLine("  -p, --position <位置>     水印位置 (默认: BottomRight)");
            Console.WriteLine("                            可选值: TopLeft, TopCenter, TopRight,");
            Console.WriteLine("                                   MiddleLeft, Center, MiddleRight,");
            Console.WriteLine("                                   BottomLeft, BottomCenter, BottomRight");
            Console.WriteLine("  -o, --opacity <数值>      透明度 0-100 (默认: 50)");
            Console.WriteLine("  -fs, --font-size <数值>   字体大小 8-200 (默认: 24，仅文本水印)");
            Console.WriteLine("  -fc, --font-color <颜色>  字体颜色，十六进制格式 (默认: #FFFFFF，仅文本水印)");
            Console.WriteLine("  --scale <数值>            缩放比例 0.1-2.0 (默认: 1.0，仅图片水印)");
            Console.WriteLine("  -q, --quality <数值>      JPEG 质量 1-100 (默认: 90)");
            Console.WriteLine("  -h, --help                显示此帮助信息");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine($"  imageglider {Name} -sd ./images -od ./output --text \"© 2024 我的公司\"");
            Console.WriteLine($"  imageglider {Name} -sd ./images -od ./output -i logo.png -ext .jpg -p TopRight");
            Console.WriteLine($"  imageglider {Name} -sd ./images -od ./output --text \"水印\" -p Center -o 30");
        }
    }
}