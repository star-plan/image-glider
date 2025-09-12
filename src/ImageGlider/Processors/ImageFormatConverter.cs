using System;
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

            // 检查是否需要使用 ImageMagick 处理 AVIF 格式
            if (targetExt == ".avif" || sourceExt == ".avif") {
                return ConvertWithImageMagick(sourceFilePath, targetFilePath, quality);
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
    /// 使用 ImageMagick 转换图片格式（主要用于 AVIF 格式支持）
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="quality">图片质量（范围 1-100）</param>
    /// <returns>转换是否成功</returns>
    private static bool ConvertWithImageMagick(string sourceFilePath, string targetFilePath, int quality = 90) {
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