using System;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;

namespace ImageGlider.Utilities;

/// <summary>
/// 图像文件验证器，提供检测文件是否为有效图片的功能
/// </summary>
public static class ImageValidator {
    /// <summary>
    /// 支持的图片文件扩展名
    /// </summary>
    private static readonly string[] SupportedExtensions =
    {
        ".jpg", ".jpeg", ".png", ".gif", ".bmp",
        ".tiff", ".tif", ".webp", ".avif", ".jfif"
    };

    /// <summary>
    /// 图片文件魔数签名（文件头标识）
    /// </summary>
    private static readonly Dictionary<string, byte[]> ImageSignatures = new()
    {
        { "JPEG", new byte[] { 0xFF, 0xD8, 0xFF } },
        { "PNG", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } },
        { "GIF87a", new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 } },
        { "GIF89a", new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 } },
        { "BMP", new byte[] { 0x42, 0x4D } },
        { "TIFF_LE", new byte[] { 0x49, 0x49, 0x2A, 0x00 } }, // Little Endian
        { "TIFF_BE", new byte[] { 0x4D, 0x4D, 0x00, 0x2A } }, // Big Endian
        { "WEBP", new byte[] { 0x52, 0x49, 0x46, 0x46 } }, // RIFF header, WebP在第8-11字节有"WEBP"
    };

    /// <summary>
    /// 检测文件是否为有效的图片文件（综合检测）
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="useDeepValidation">是否使用深度验证（通过ImageSharp加载验证）</param>
    /// <returns>如果是有效图片文件返回 true，否则返回 false</returns>
    public static bool IsValidImageFile(string filePath, bool useDeepValidation = false) {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) {
            return false;
        }

        // 首先进行扩展名检查
        if (!IsValidImageExtension(filePath)) {
            return false;
        }

        // 进行文件头检查
        if (!IsValidImageBySignature(filePath)) {
            return false;
        }

        // 如果需要深度验证，尝试加载图片
        if (useDeepValidation) {
            return IsValidImageByLoading(filePath);
        }

        return true;
    }

    /// <summary>
    /// 基于文件扩展名检测是否为图片文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>如果扩展名匹配支持的图片格式返回 true，否则返回 false</returns>
    public static bool IsValidImageExtension(string filePath) {
        if (string.IsNullOrWhiteSpace(filePath)) {
            return false;
        }

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return SupportedExtensions.Contains(extension);
    }

    /// <summary>
    /// 基于文件头签名检测是否为图片文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>如果文件头匹配已知图片格式返回 true，否则返回 false</returns>
    public static bool IsValidImageBySignature(string filePath) {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) {
            return false;
        }

        try {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var buffer = new byte[12]; // 读取前12字节足够检测大部分格式
            var bytesRead = fileStream.Read(buffer, 0, buffer.Length);

            if (bytesRead < 2) {
                return false;
            }

            // 检查各种图片格式的文件头
            foreach (var signature in ImageSignatures.Values) {
                if (bytesRead >= signature.Length && IsSignatureMatch(buffer, signature)) {
                    // 对于WebP格式，需要额外检查第8-11字节是否为"WEBP"
                    if (signature == ImageSignatures["WEBP"] && bytesRead >= 12) {
                        var webpSignature = new byte[] { 0x57, 0x45, 0x42, 0x50 }; // "WEBP"
                        return IsSignatureMatch(buffer, webpSignature, 8);
                    }
                    return true;
                }
            }

            return false;
        }
        catch {
            return false;
        }
    }

    /// <summary>
    /// 通过ImageSharp库加载验证是否为有效图片文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>如果能成功加载为图片返回 true，否则返回 false</returns>
    public static bool IsValidImageByLoading(string filePath) {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) {
            return false;
        }

        try {
            using var image = Image.Load(filePath);
            return image.Width > 0 && image.Height > 0;
        }
        catch {
            return false;
        }
    }

    /// <summary>
    /// 获取支持的图片文件扩展名列表
    /// </summary>
    /// <returns>支持的扩展名数组</returns>
    public static string[] GetSupportedExtensions() {
        return (string[])SupportedExtensions.Clone();
    }

    /// <summary>
    /// 检查字节数组是否匹配指定的签名
    /// </summary>
    /// <param name="buffer">要检查的字节数组</param>
    /// <param name="signature">签名字节数组</param>
    /// <param name="offset">开始比较的偏移量</param>
    /// <returns>如果匹配返回 true，否则返回 false</returns>
    private static bool IsSignatureMatch(byte[] buffer, byte[] signature, int offset = 0) {
        if (buffer.Length < offset + signature.Length) {
            return false;
        }

        for (int i = 0; i < signature.Length; i++) {
            if (buffer[offset + i] != signature[i]) {
                return false;
            }
        }

        return true;
    }
}