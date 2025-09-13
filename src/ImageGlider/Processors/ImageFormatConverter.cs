using System;
using System.Diagnostics;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using ImageMagick;
using ImageGlider.Core;
using ImageGlider.Enums;

namespace ImageGlider.Processors;

/// <summary>
/// 图像格式转换器，专门处理图片格式转换功能
/// </summary>
public class ImageFormatConverter : IImageFormatConverter {
    /// <summary>
    /// 转换单个图片文件格式
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="quality">JPEG 质量（仅对 JPEG 格式有效，范围 1-100）</param>
    /// <returns>转换是否成功</returns>
    public bool ProcessImage(string sourceFilePath, string targetFilePath, int quality = 90) {
        return ConvertImage(sourceFilePath, targetFilePath, quality);
    }

    /// <summary>
    /// 转换单个图片文件格式
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="quality">JPEG/AVIF 质量（范围 1-100）</param>
    /// <returns>转换是否成功</returns>
    public static bool ConvertImage(string sourceFilePath, string targetFilePath, int quality = 90) {
        try {
            if (!File.Exists(sourceFilePath)) {
                throw new FileNotFoundException($"源文件不存在: {sourceFilePath}");
            }

            // 确保目标目录存在
            var targetDir = Path.GetDirectoryName(targetFilePath);
            if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir)) {
                Directory.CreateDirectory(targetDir);
            }

            var targetExt = Path.GetExtension(targetFilePath).ToLowerInvariant();
            var sourceExt = Path.GetExtension(sourceFilePath).ToLowerInvariant();

            // 检查是否需要使用特殊处理 AVIF 格式
            if (targetExt == ".avif" || sourceExt == ".avif") {
                // 优先使用 FFmpeg，如果不可用则使用 ImageMagick
                if (IsFFmpegAvailable()) {
                    return ConvertWithFFmpeg(sourceFilePath, targetFilePath, quality);
                }
                else {
                    return ConvertWithImageMagick(sourceFilePath, targetFilePath, quality);
                }
            }

            // 使用 ImageSharp 处理其他格式
            using var image = Image.Load(sourceFilePath);

            // 如果目标格式为 JPEG，使用指定质量
            if (targetExt == ".jpeg" || targetExt == ".jpg") {
                var encoder = new JpegEncoder { Quality = Math.Clamp(quality, 1, 100) };
                image.Save(targetFilePath, encoder);
            }
            else {
                image.Save(targetFilePath);
            }

            return true;
        }
        catch {
            return false;
        }
    }

    /// <summary>
    /// 检查系统中是否安装了 FFmpeg
    /// </summary>
    /// <returns>如果找到 FFmpeg 返回 true，否则返回 false</returns>
    private static bool IsFFmpegAvailable() {
        try {
            var process = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = "ffmpeg",
                    Arguments = "-version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit(3000); // 等待最多3秒
            return process.ExitCode == 0;
        }
        catch {
            return false;
        }
    }

    /// <summary>
    /// 使用 FFmpeg 转换图片格式（主要用于 AVIF 格式支持）
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="quality">图片质量（范围 1-100，AVIF 格式建议 20-60）</param>
    /// <returns>转换是否成功</returns>
    /// <summary>
    /// 使用 FFmpeg 进行图像格式转换
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="quality">质量参数 (0-100)</param>
    /// <param name="cpuUsed">CPU 使用级别 (0-8，越大越快但质量稍差)</param>
    /// <param name="threads">线程数</param>
    /// <param name="timeoutMs">超时时间（毫秒）</param>
    /// <returns>转换是否成功</returns>
    private static bool ConvertWithFFmpeg(string sourceFilePath, string targetFilePath, int quality = 30,
        int cpuUsed = 6, int threads = 8, int timeoutMs = 30000) {
        try {
            var targetExt = Path.GetExtension(targetFilePath).ToLowerInvariant();

            // 构建 FFmpeg 命令参数
            string arguments;
            if (targetExt == ".avif") {
                // 使用 libaom-av1 编码器转换为 AVIF
                // -cpu-used: 编码速度（0-8，越大越快但质量稍差）
                // -row-mt 1: 启用行并行处理
                // -threads: 使用指定数量的线程
                // -crf: 质量控制（0-63，越小质量越好）
                var crf = Math.Clamp(quality * 63 / 100, 15, 50); // 将质量转换为 CRF 值
                var clampedCpuUsed = Math.Clamp(cpuUsed, 0, 8);
                var clampedThreads = Math.Clamp(threads, 1, 16);
                arguments = $"-i \"{sourceFilePath}\" -c:v libaom-av1 -cpu-used {clampedCpuUsed} -row-mt 1 -threads {clampedThreads} -crf {crf} -y \"{targetFilePath}\"";
            }
            else {
                // 对于其他格式，使用基本转换
                arguments = $"-i \"{sourceFilePath}\" -q:v {Math.Clamp(quality * 31 / 100, 2, 31)} -y \"{targetFilePath}\"";
            }

            var process = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = "ffmpeg",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit(timeoutMs);

            return process.ExitCode == 0 && File.Exists(targetFilePath);
        }
        catch {
            return false;
        }
    }

    /// <summary>
    /// 使用 ImageMagick 转换图片格式（作为 FFmpeg 的备选方案）
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="quality">图片质量（范围 1-100，AVIF 格式建议 20-60）</param>
    /// <returns>转换是否成功</returns>
    private static bool ConvertWithImageMagick(string sourceFilePath, string targetFilePath, int quality = 40) {
        try {
            using var image = new MagickImage(sourceFilePath);

            // 设置质量参数
            image.Quality = (uint)Math.Clamp(quality, 1, 100);

            var targetExt = Path.GetExtension(targetFilePath).ToLowerInvariant();

            // 针对 AVIF 格式进行特殊配置
            if (targetExt == ".avif") {
                image.Format = MagickFormat.Avif;
                // 设置 AVIF 编码参数以获得更好的质量
                image.Settings.SetDefine(MagickFormat.Avif, "method", "4");
                image.Settings.SetDefine(MagickFormat.Avif, "speed", "6");
            }

            image.Write(targetFilePath);
            return true;
        }
        catch {
            return false;
        }
    }

    /// <summary>
    /// 批量转换指定目录下的图片文件
    /// </summary>
    /// <param name="sourceDirectory">源目录</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="sourceExtension">源文件扩展名（如 .jfif）</param>
    /// <param name="targetExtension">目标文件扩展名（如 .jpeg）</param>
    /// <param name="quality">JPEG 质量</param>
    /// <returns>转换结果信息</returns>
    public ConversionResult BatchConvert(
        string sourceDirectory,
        string outputDirectory,
        string sourceExtension,
        string targetExtension,
        int quality = 90) {
        var result = new ConversionResult();

        try {
            if (!Directory.Exists(sourceDirectory)) {
                throw new DirectoryNotFoundException($"源目录不存在: {sourceDirectory}");
            }

            // 确保扩展名以点开头
            if (!sourceExtension.StartsWith("."))
                sourceExtension = "." + sourceExtension;
            if (!targetExtension.StartsWith("."))
                targetExtension = "." + targetExtension;

            // 创建输出目录
            Directory.CreateDirectory(outputDirectory);

            // 查找所有匹配的文件
            var files = Directory.GetFiles(sourceDirectory, "*" + sourceExtension);
            result.TotalFiles = files.Length;

            foreach (var file in files) {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var targetFileName = fileName + targetExtension;
                var targetFilePath = Path.Combine(outputDirectory, targetFileName);

                if (ConvertImage(file, targetFilePath, quality)) {
                    result.SuccessfulConversions++;
                    result.SuccessfulFiles.Add(Path.GetFileName(file));
                }
                else {
                    result.FailedConversions++;
                    result.FailedFiles.Add(Path.GetFileName(file));
                }
            }
        }
        catch (Exception ex) {
            result.ErrorMessage = ex.Message;
        }

        return result;
    }
}