using System;
using System.Threading.Tasks;
using ImageGlider;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 单文件压缩命令
    /// </summary>
    public class CompressCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "compress";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "压缩优化单个图片文件";

        /// <summary>
        /// 执行压缩命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
        {
            string? sourceFile = null;
            string? targetFile = null;
            int compressionLevel = 75;
            bool preserveMetadata = false;

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
                    case "--level":
                    case "-l":
                        compressionLevel = ParseIntParameter(args, ref i, "--level", 1, 100);
                        break;
                    case "--preserve-meta":
                    case "-pm":
                        preserveMetadata = true;
                        break;
                }
            }

            // 验证必需参数
            ValidateRequiredParameter(sourceFile, "--source");
            ValidateRequiredParameter(targetFile, "--target");

            Console.WriteLine($"正在压缩: {sourceFile} -> {targetFile}");
            Console.WriteLine($"压缩级别: {compressionLevel}");
            Console.WriteLine($"保留元数据: {preserveMetadata}");

            var success = ImageConverter.CompressImage(sourceFile!, targetFile!, compressionLevel, preserveMetadata);

            if (success)
            {
                Console.WriteLine("✓ 压缩成功！");
                return 0;
            }
            else
            {
                Console.WriteLine("✗ 压缩失败！请检查源文件是否存在以及目标路径是否有效。");
                return 1;
            }
        }

        /// <summary>
        /// 显示压缩命令的帮助信息
        /// </summary>
        public override void ShowHelp()
        {
            Console.WriteLine("用法:");
            Console.WriteLine("  imageglider compress --source <源文件> --target <目标文件> [选项]");
        Console.WriteLine();
        Console.WriteLine("参数:");
        Console.WriteLine("  -s, --source <文件>     源图片文件路径 (必需)");
        Console.WriteLine("  -t, --target <文件>     目标图片文件路径 (必需)");
        Console.WriteLine("  -l, --level <级别>      压缩级别 (1-100，默认: 75)");
        Console.WriteLine("  --preserve-meta         保留元数据");
        Console.WriteLine("  --help                  显示此帮助信息");
        Console.WriteLine();
        Console.WriteLine("说明:");
        Console.WriteLine("  - 压缩级别越低，文件越小，质量越差");
        Console.WriteLine("  - 默认会移除 EXIF 等元数据以减小文件大小");
        Console.WriteLine("  - 使用 --preserve-meta 保留原始元数据");
        Console.WriteLine();
        Console.WriteLine("示例:");
        Console.WriteLine("  imageglider compress -s input.jpg -t output.jpg -l 60");
        Console.WriteLine("  imageglider compress -s input.jpg -t output.jpg -l 50 --preserve-meta");
        }
    }
}