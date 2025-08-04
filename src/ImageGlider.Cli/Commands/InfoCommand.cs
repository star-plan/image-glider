using System;
using System.IO;
using System.Threading.Tasks;
using ImageGlider;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 图像信息查看命令
    /// </summary>
    public class InfoCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "info";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "查看图片文件的详细信息";

        /// <summary>
        /// 执行信息查看命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
        {
            string? sourceFile = null;
            bool jsonOutput = false;

            // 解析参数
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--source":
                    case "-s":
                        sourceFile = ParseStringParameter(args, ref i, "--source");
                        break;
                    case "--json":
                    case "-j":
                        jsonOutput = true;
                        break;
                }
            }

            // 验证必需参数
            ValidateRequiredParameter(sourceFile, "--source");

            if (!File.Exists(sourceFile!))
            {
                Console.WriteLine($"✗ 文件不存在: {sourceFile}");
                return 1;
            }

            try
            {
                Console.WriteLine($"正在分析图片: {sourceFile}");
                
                var imageInfo = ImageConverter.GetImageInfo(sourceFile!);

                if (jsonOutput)
                {
                    Console.WriteLine(imageInfo.ToJson());
                }
                else
                {
                    Console.WriteLine("\n=== 图片信息 ===");
                    Console.WriteLine(imageInfo.ToString());
                    
                    // 如果有额外的元数据，显示它们
                    if (imageInfo.AdditionalMetadata.Count > 0)
                    {
                        Console.WriteLine("\n=== 额外元数据 ===");
                        foreach (var kvp in imageInfo.AdditionalMetadata)
                        {
                            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                        }
                    }
                }

                Console.WriteLine("\n✓ 信息提取完成！");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 提取信息失败: {ex.Message}");
                return 1;
            }
        }

        /// <summary>
        /// 显示信息查看命令的帮助信息
        /// </summary>
        public override void ShowHelp()
        {
            Console.WriteLine("用法:");
            Console.WriteLine("  ImageGlider.Cli info --source <图片文件> [--json]");
            Console.WriteLine();
            Console.WriteLine("参数:");
            Console.WriteLine("  --source, -s    要分析的图片文件路径");
            Console.WriteLine("  --json, -j      以JSON格式输出结果");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  ImageGlider.Cli info -s photo.jpg");
            Console.WriteLine("  ImageGlider.Cli info -s photo.jpg --json");
            Console.WriteLine();
            Console.WriteLine("支持的格式:");
            Console.WriteLine("  JPEG, PNG, GIF, BMP, TIFF, WebP");
        }
    }
}