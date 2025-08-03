using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ImageGlider;

/// <summary>
/// 图片格式转换服务类，提供图片格式转换的核心功能
/// </summary>
public class ImageConverter
{
    /// <summary>
    /// 转换单个图片文件格式
    /// </summary>
    /// <param name="sourceFilePath">源文件路径</param>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="quality">JPEG 质量（仅对 JPEG 格式有效，范围 1-100）</param>
    /// <returns>转换是否成功</returns>
    public static bool ConvertImage(string sourceFilePath, string targetFilePath, int quality = 90)
    {
        try
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException($"源文件不存在: {sourceFilePath}");
            }

            // 确保目标目录存在
            var targetDir = Path.GetDirectoryName(targetFilePath);
            if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            using var image = Image.Load(sourceFilePath);
            var targetExt = Path.GetExtension(targetFilePath).ToLowerInvariant();

            // 如果目标格式为 JPEG，使用指定质量
            if (targetExt == ".jpeg" || targetExt == ".jpg")
            {
                var encoder = new JpegEncoder { Quality = Math.Clamp(quality, 1, 100) };
                image.Save(targetFilePath, encoder);
            }
            else
            {
                image.Save(targetFilePath);
            }

            return true;
        }
        catch
        {
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
    public static ConversionResult BatchConvert(
        string sourceDirectory,
        string outputDirectory,
        string sourceExtension,
        string targetExtension,
        int quality = 90)
    {
        var result = new ConversionResult();

        try
        {
            if (!Directory.Exists(sourceDirectory))
            {
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

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var targetFileName = fileName + targetExtension;
                var targetFilePath = Path.Combine(outputDirectory, targetFileName);

                if (ConvertImage(file, targetFilePath, quality))
                {
                    result.SuccessfulConversions++;
                    result.SuccessfulFiles.Add(Path.GetFileName(file));
                }
                else
                {
                    result.FailedConversions++;
                    result.FailedFiles.Add(Path.GetFileName(file));
                }
            }
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
        }

        return result;
    }
}

/// <summary>
/// 批量转换结果信息
/// </summary>
public class ConversionResult
{
    /// <summary>
    /// 总文件数
    /// </summary>
    public int TotalFiles { get; set; }

    /// <summary>
    /// 成功转换的文件数
    /// </summary>
    public int SuccessfulConversions { get; set; }

    /// <summary>
    /// 失败转换的文件数
    /// </summary>
    public int FailedConversions { get; set; }

    /// <summary>
    /// 成功转换的文件列表
    /// </summary>
    public List<string> SuccessfulFiles { get; set; } = new();

    /// <summary>
    /// 失败转换的文件列表
    /// </summary>
    public List<string> FailedFiles { get; set; } = new();

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 转换是否成功
    /// </summary>
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage) && FailedConversions == 0;
}