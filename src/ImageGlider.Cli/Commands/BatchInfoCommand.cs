using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageGlider;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 批量图像信息查看命令
    /// </summary>
    public class BatchInfoCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "batch-info";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "批量查看目录下图片文件的详细信息";

        /// <summary>
        /// 执行批量信息查看命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
        {
            string? sourceDirectory = null;
            string searchPattern = "*.*";
            bool recursive = false;
            bool jsonOutput = false;
            string? outputFile = null;

            // 解析参数
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--source-dir":
                    case "-sd":
                        sourceDirectory = ParseStringParameter(args, ref i, "--source-dir");
                        break;
                    case "--pattern":
                    case "-p":
                        searchPattern = ParseStringParameter(args, ref i, "--pattern");
                        break;
                    case "--recursive":
                    case "-r":
                        recursive = true;
                        break;
                    case "--json":
                    case "-j":
                        jsonOutput = true;
                        break;
                    case "--output":
                    case "-o":
                        outputFile = ParseStringParameter(args, ref i, "--output");
                        break;
                }
            }

            // 验证必需参数
            ValidateRequiredParameter(sourceDirectory, "--source-dir");

            if (!Directory.Exists(sourceDirectory!))
            {
                Console.WriteLine($"✗ 目录不存在: {sourceDirectory}");
                return 1;
            }

            try
            {
                Console.WriteLine($"正在扫描目录: {sourceDirectory}");
                Console.WriteLine($"搜索模式: {searchPattern}");
                Console.WriteLine($"递归搜索: {(recursive ? "是" : "否")}");
                Console.WriteLine();
                
                var imageInfos = ImageConverter.BatchGetImageInfo(sourceDirectory!, searchPattern, recursive);

                if (imageInfos.Count == 0)
                {
                    Console.WriteLine("✗ 未找到任何图片文件");
                    return 1;
                }

                Console.WriteLine($"找到 {imageInfos.Count} 个图片文件");
                Console.WriteLine();

                if (jsonOutput)
                {
                    var jsonResult = System.Text.Json.JsonSerializer.Serialize(imageInfos, new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                    });

                    if (!string.IsNullOrEmpty(outputFile))
                    {
                        await File.WriteAllTextAsync(outputFile, jsonResult);
                        Console.WriteLine($"✓ 结果已保存到: {outputFile}");
                    }
                    else
                    {
                        Console.WriteLine(jsonResult);
                    }
                }
                else
                {
                    var output = GenerateTextReport(imageInfos);
                    
                    if (!string.IsNullOrEmpty(outputFile))
                    {
                        await File.WriteAllTextAsync(outputFile, output);
                        Console.WriteLine($"✓ 结果已保存到: {outputFile}");
                    }
                    else
                    {
                        Console.WriteLine(output);
                    }
                }

                Console.WriteLine($"\n✓ 批量信息提取完成！处理了 {imageInfos.Count} 个文件");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 批量提取信息失败: {ex.Message}");
                return 1;
            }
        }

        /// <summary>
        /// 生成文本格式的报告
        /// </summary>
        /// <param name="imageInfos">图像信息列表</param>
        /// <returns>格式化的文本报告</returns>
        private static string GenerateTextReport(System.Collections.Generic.List<ImageGlider.Core.ImageInfo> imageInfos)
        {
            var report = new System.Text.StringBuilder();
            
            report.AppendLine("=== 批量图像信息报告 ===");
            report.AppendLine($"生成时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"总文件数: {imageInfos.Count}");
            report.AppendLine();

            // 统计信息
            var totalSize = imageInfos.Sum(i => i.FileSize);
            var formatGroups = imageInfos.GroupBy(i => i.Format).OrderByDescending(g => g.Count());
            var avgWidth = imageInfos.Average(i => i.Width);
            var avgHeight = imageInfos.Average(i => i.Height);

            report.AppendLine("=== 统计摘要 ===");
            report.AppendLine($"总大小: {totalSize:N0} 字节 ({totalSize / 1024.0 / 1024.0:F2} MB)");
            report.AppendLine($"平均尺寸: {avgWidth:F0} x {avgHeight:F0} 像素");
            report.AppendLine();

            report.AppendLine("格式分布:");
            foreach (var group in formatGroups)
            {
                var percentage = (double)group.Count() / imageInfos.Count * 100;
                report.AppendLine($"  {group.Key}: {group.Count()} 个文件 ({percentage:F1}%)");
            }
            report.AppendLine();

            // 详细信息
            report.AppendLine("=== 详细信息 ===");
            for (int i = 0; i < imageInfos.Count; i++)
            {
                var info = imageInfos[i];
                report.AppendLine($"[{i + 1:D3}] {Path.GetFileName(info.FilePath)}");
                report.AppendLine($"     路径: {info.FilePath}");
                report.AppendLine($"     大小: {info.FileSize:N0} 字节 ({info.FileSize / 1024.0 / 1024.0:F2} MB)");
                report.AppendLine($"     尺寸: {info.Width} x {info.Height} 像素");
                report.AppendLine($"     格式: {info.Format}");
                report.AppendLine($"     色深: {info.BitDepth} 位");
                report.AppendLine($"     DPI: {info.HorizontalDpi:F1} x {info.VerticalDpi:F1}");
                report.AppendLine($"     颜色空间: {info.ColorSpace}");
                report.AppendLine($"     透明通道: {(info.HasAlpha ? "是" : "否")}");
                report.AppendLine($"     压缩: {info.Compression}");
                report.AppendLine($"     元数据: {(info.HasMetadata ? $"是 ({info.MetadataSize:N0} 字节)" : "否")}");
                
                if (info.AdditionalMetadata.Count > 0)
                {
                    report.AppendLine("     额外信息:");
                    foreach (var kvp in info.AdditionalMetadata)
                    {
                        report.AppendLine($"       {kvp.Key}: {kvp.Value}");
                    }
                }
                
                report.AppendLine();
            }

            return report.ToString();
        }

        /// <summary>
        /// 显示批量信息查看命令的帮助信息
        /// </summary>
        public override void ShowHelp()
        {
            Console.WriteLine("用法:");
            Console.WriteLine("  imageglider batch-info --source-dir <目录> [选项]");
            Console.WriteLine();
            Console.WriteLine("参数:");
            Console.WriteLine("  --source-dir, -sd    要扫描的目录路径");
            Console.WriteLine("  --pattern, -p        文件搜索模式（默认: *.*）");
            Console.WriteLine("  --recursive, -r      递归搜索子目录");
            Console.WriteLine("  --json, -j           以JSON格式输出结果");
            Console.WriteLine("  --output, -o         输出文件路径（可选）");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  imageglider batch-info -sd ./photos");
            Console.WriteLine("  imageglider batch-info -sd ./photos -p *.jpg -r");
            Console.WriteLine("  imageglider batch-info -sd ./photos --json -o report.json");
            Console.WriteLine("  imageglider batch-info -sd ./photos -o report.txt");
            Console.WriteLine();
            Console.WriteLine("支持的格式:");
            Console.WriteLine("  JPEG, PNG, GIF, BMP, TIFF, WebP");
        }
    }
}