using System;
using System.Threading.Tasks;
using ImageGlider;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 单文件元数据清理命令
    /// </summary>
    public class StripMetadataCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "strip-metadata";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "清理单个图片文件的元数据";

        /// <summary>
        /// 执行元数据清理命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
        {
            string? sourceFile = null;
            string? targetFile = null;
            bool stripAll = true;
            bool stripExif = true;
            bool stripIcc = false;
            bool stripXmp = true;
            int quality = 90;

            // 解析命令行参数
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--source":
                    case "-s":
                        if (i + 1 < args.Length)
                        {
                            sourceFile = args[++i];
                        }
                        else
                        {
                            throw new ArgumentException("--source 参数需要指定文件路径");
                        }
                        break;

                    case "--target":
                    case "-t":
                        if (i + 1 < args.Length)
                        {
                            targetFile = args[++i];
                        }
                        else
                        {
                            throw new ArgumentException("--target 参数需要指定文件路径");
                        }
                        break;

                    case "--quality":
                    case "-q":
                        if (i + 1 < args.Length && int.TryParse(args[++i], out var q))
                        {
                            quality = Math.Max(1, Math.Min(100, q));
                        }
                        else
                        {
                            throw new ArgumentException("--quality 参数需要指定1-100之间的整数");
                        }
                        break;

                    case "--all":
                        stripAll = true;
                        stripExif = true;
                        stripIcc = true;
                        stripXmp = true;
                        break;

                    case "--exif":
                        stripAll = false;
                        stripExif = true;
                        break;

                    case "--icc":
                        stripAll = false;
                        stripIcc = true;
                        break;

                    case "--xmp":
                        stripAll = false;
                        stripXmp = true;
                        break;

                    case "--no-exif":
                        stripExif = false;
                        break;

                    case "--no-icc":
                        stripIcc = false;
                        break;

                    case "--no-xmp":
                        stripXmp = false;
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

            Console.WriteLine($"正在清理元数据: {sourceFile} -> {targetFile}");
            Console.WriteLine($"清理选项: 全部={stripAll}, EXIF={stripExif}, ICC={stripIcc}, XMP={stripXmp}");
            Console.WriteLine($"JPEG 质量: {quality}");
            Console.WriteLine();

            var success = ImageConverter.StripMetadata(sourceFile, targetFile, stripAll, stripExif, stripIcc, stripXmp, quality);

            if (success)
            {
                Console.WriteLine("✓ 元数据清理成功！");
                return 0;
            }
            else
            {
                Console.WriteLine("✗ 元数据清理失败！");
                return 1;
            }
        }

        /// <summary>
        /// 显示元数据清理命令的帮助信息
        /// </summary>
        public override void ShowHelp()
        {
            Console.WriteLine("用法:");
            Console.WriteLine("  ImageGlider.Cli strip-metadata --source <源文件> --target <目标文件> [选项]");
            Console.WriteLine();
            Console.WriteLine("必需参数:");
            Console.WriteLine("  --source, -s     源文件路径");
            Console.WriteLine("  --target, -t     目标文件路径");
            Console.WriteLine();
            Console.WriteLine("选项:");
            Console.WriteLine("  --quality, -q    JPEG 质量 (1-100, 默认: 90)");
            Console.WriteLine();
            Console.WriteLine("元数据清理选项:");
            Console.WriteLine("  --all            清理所有元数据 (默认)");
            Console.WriteLine("  --exif           仅清理 EXIF 数据");
            Console.WriteLine("  --icc            仅清理 ICC 配置文件");
            Console.WriteLine("  --xmp            仅清理 XMP 数据");
            Console.WriteLine("  --no-exif        不清理 EXIF 数据");
            Console.WriteLine("  --no-icc         不清理 ICC 配置文件");
            Console.WriteLine("  --no-xmp         不清理 XMP 数据");
            Console.WriteLine();
            Console.WriteLine("说明:");
            Console.WriteLine("  清理图片文件中的元数据信息，包括 EXIF、ICC、XMP 等");
            Console.WriteLine("  可以保护隐私并减小文件体积");
            Console.WriteLine("  默认清理所有元数据，但保留 ICC 配置文件以保持颜色准确性");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  ImageGlider.Cli strip-metadata -s photo.jpg -t clean_photo.jpg");
            Console.WriteLine("  ImageGlider.Cli strip-metadata -s image.png -t output.png --all");
            Console.WriteLine("  ImageGlider.Cli strip-metadata -s input.jpg -t output.jpg --exif --xmp");
            Console.WriteLine("  ImageGlider.Cli strip-metadata -s photo.jpg -t clean.jpg --all --no-icc");
        }
    }
}