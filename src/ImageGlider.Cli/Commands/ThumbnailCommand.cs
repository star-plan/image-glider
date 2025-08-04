using System;
using System.Threading.Tasks;
using ImageGlider;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 单文件缩略图生成命令
    /// </summary>
    public class ThumbnailCommand : BaseCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public override string Name => "thumbnail";

        /// <summary>
        /// 命令描述
        /// </summary>
        public override string Description => "生成单个图片文件的缩略图";

        /// <summary>
        /// 执行缩略图生成命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected override async Task<int> ExecuteInternalAsync(string[] args)
        {
            string? sourceFile = null;
            string? targetFile = null;
            int maxSize = 150;
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
                    case "--size":
                    case "--max-size":
                    case "-sz":
                        maxSize = ParseIntParameter(args, ref i, "--size", 1, int.MaxValue);
                        break;
                    case "--quality":
                    case "-q":
                        quality = ParseIntParameter(args, ref i, "--quality", 1, 100);
                        break;
                }
            }

            // 验证必需参数
            ValidateRequiredParameter(sourceFile, "--source");
            ValidateRequiredParameter(targetFile, "--target");

            Console.WriteLine($"正在生成缩略图: {sourceFile} -> {targetFile}");
            Console.WriteLine($"最大边长: {maxSize} 像素");
            Console.WriteLine($"JPEG 质量: {quality}");

            var success = ImageConverter.GenerateThumbnail(sourceFile!, targetFile!, maxSize, quality);

            if (success)
            {
                Console.WriteLine("✓ 缩略图生成成功！");
                return 0;
            }
            else
            {
                Console.WriteLine("✗ 缩略图生成失败！");
                return 1;
            }
        }

        /// <summary>
        /// 显示缩略图生成命令的帮助信息
        /// </summary>
        public override void ShowHelp()
        {
            Console.WriteLine("用法:");
            Console.WriteLine("  ImageGlider.Cli thumbnail --source <源文件> --target <目标文件> [选项]");
            Console.WriteLine();
            Console.WriteLine("选项:");
            Console.WriteLine("  --source, -s       源文件路径 (必需)");
            Console.WriteLine("  --target, -t       目标文件路径 (必需)");
            Console.WriteLine("  --size, -sz        缩略图最大边长 (像素, 默认: 150)");
            Console.WriteLine("  --quality, -q      JPEG 质量 (1-100, 默认: 90)");
            Console.WriteLine();
            Console.WriteLine("说明:");
            Console.WriteLine("  缩略图会按照长边等比缩放，保持原始图片的宽高比");
            Console.WriteLine("  如果原图宽度大于高度，则宽度缩放到指定尺寸");
            Console.WriteLine("  如果原图高度大于宽度，则高度缩放到指定尺寸");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  ImageGlider.Cli thumbnail -s input.jpg -t thumb.jpg --size 200");
            Console.WriteLine("  ImageGlider.Cli thumbnail -s photo.png -t thumbnail.png -sz 100 -q 85");
        }
    }
}