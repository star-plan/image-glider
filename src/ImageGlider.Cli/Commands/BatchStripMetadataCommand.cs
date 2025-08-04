using System;
using System.IO;
using System.Threading.Tasks;
using ImageGlider;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 批量元数据清理命令
    /// </summary>
    public class BatchStripMetadataCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "batch-strip-metadata";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "批量清理指定目录下图片文件的元数据";

        /// <summary>
        /// 执行批量元数据清理命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
        {
            string sourceDirectory = Directory.GetCurrentDirectory();
            string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "cleaned");
            string sourceExtension = ".jpg";
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
                    case "--source-dir":
                    case "-sd":
                        if (i + 1 < args.Length)
                        {
                            sourceDirectory = args[++i];
                        }
                        else
                        {
                            throw new ArgumentException("--source-dir 参数需要指定目录路径");
                        }
                        break;

                    case "--output-dir":
                    case "-od":
                        if (i + 1 < args.Length)
                        {
                            outputDirectory = args[++i];
                        }
                        else
                        {
                            throw new ArgumentException("--output-dir 参数需要指定目录路径");
                        }
                        break;

                    case "--extension":
                    case "-ext":
                        if (i + 1 < args.Length)
                        {
                            sourceExtension = args[++i];
                            if (!sourceExtension.StartsWith("."))
                            {
                                sourceExtension = "." + sourceExtension;
                            }
                        }
                        else
                        {
                            throw new ArgumentException("--extension 参数需要指定文件扩展名");
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

            Console.WriteLine($"正在批量清理元数据...");
            Console.WriteLine($"源目录: {sourceDirectory}");
            Console.WriteLine($"输出目录: {outputDirectory}");
            Console.WriteLine($"文件扩展名: {sourceExtension}");
            Console.WriteLine($"清理选项: 全部={stripAll}, EXIF={stripExif}, ICC={stripIcc}, XMP={stripXmp}");
            Console.WriteLine($"JPEG 质量: {quality}");
            Console.WriteLine();

            var result = ImageConverter.BatchStripMetadata(sourceDirectory, outputDirectory, sourceExtension, stripAll, stripExif, stripIcc, stripXmp, quality);

            if (result.Success)
            {
                Console.WriteLine($"✓ 批量元数据清理完成！");
                Console.WriteLine($"总文件数: {result.TotalFiles}");
                Console.WriteLine($"成功: {result.SuccessCount}");
                Console.WriteLine($"失败: {result.FailureCount}");
                
                if (result.ProcessedFiles.Count > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("处理详情:");
                    foreach (var file in result.ProcessedFiles)
                    {
                        var status = file.Success ? "✓" : "✗";
                        Console.WriteLine($"  {status} {Path.GetFileName(file.SourcePath)}");
                        if (!file.Success && !string.IsNullOrEmpty(file.ErrorMessage))
                        {
                            Console.WriteLine($"    错误: {file.ErrorMessage}");
                        }
                    }
                }
                
                return 0;
            }
            else
            {
                Console.WriteLine($"✗ 批量元数据清理失败！");
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Console.WriteLine($"错误: {result.ErrorMessage}");
                }
                return 1;
            }
        }

        /// <summary>
        /// 显示批量元数据清理命令的帮助信息
        /// </summary>
        public override void ShowHelp()
        {
            Console.WriteLine("用法:");
            Console.WriteLine("  imageglider batch-strip-metadata [选项]");
            Console.WriteLine();
            Console.WriteLine("选项:");
            Console.WriteLine("  --source-dir, -sd    源目录路径 (默认: 当前目录)");
            Console.WriteLine("  --output-dir, -od    输出目录路径 (默认: ./cleaned)");
            Console.WriteLine("  --extension, -ext    源文件扩展名 (默认: .jpg)");
            Console.WriteLine("  --quality, -q        JPEG 质量 (1-100, 默认: 90)");
            Console.WriteLine();
            Console.WriteLine("元数据清理选项:");
            Console.WriteLine("  --all                清理所有元数据 (默认)");
            Console.WriteLine("  --exif               仅清理 EXIF 数据");
            Console.WriteLine("  --icc                仅清理 ICC 配置文件");
            Console.WriteLine("  --xmp                仅清理 XMP 数据");
            Console.WriteLine("  --no-exif            不清理 EXIF 数据");
            Console.WriteLine("  --no-icc             不清理 ICC 配置文件");
            Console.WriteLine("  --no-xmp             不清理 XMP 数据");
            Console.WriteLine();
            Console.WriteLine("说明:");
            Console.WriteLine("  批量处理指定目录下的所有匹配文件");
            Console.WriteLine("  清理图片文件中的元数据信息，包括 EXIF、ICC、XMP 等");
            Console.WriteLine("  可以保护隐私并减小文件体积");
            Console.WriteLine("  默认清理所有元数据，但保留 ICC 配置文件以保持颜色准确性");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  imageglider batch-strip-metadata -sd ./photos -od ./cleaned");
            Console.WriteLine("  imageglider batch-strip-metadata -sd C:\\Images -od C:\\CleanImages -ext .png");
            Console.WriteLine("  imageglider batch-strip-metadata -sd ./input -od ./output --all");
            Console.WriteLine("  imageglider batch-strip-metadata -sd ./photos --exif --xmp --no-icc");
        }
    }
}