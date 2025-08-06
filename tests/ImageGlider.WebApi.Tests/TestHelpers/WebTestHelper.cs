using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageGlider.WebApi.Tests.TestHelpers;

/// <summary>
/// Web API 测试辅助工具类
/// </summary>
public static class WebTestHelper
{
    /// <summary>
    /// 创建测试用的 IFormFile 对象
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="width">图片宽度</param>
    /// <param name="height">图片高度</param>
    /// <param name="color">图片颜色</param>
    /// <returns>IFormFile 对象</returns>
    public static IFormFile CreateTestFormFile(string fileName = "test.jpg", int width = 100, int height = 100, Color? color = null)
    {
        var actualColor = color ?? Color.Blue;
        var stream = new MemoryStream();
        
        using (var image = new Image<Rgba32>(width, height))
        {
            image.Mutate(x => x.BackgroundColor(actualColor));
            
            if (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                image.SaveAsPng(stream);
            }
            else if (fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || 
                     fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                image.SaveAsJpeg(stream);
            }
            else
            {
                image.SaveAsJpeg(stream);
            }
        }
        
        stream.Position = 0;
        
        var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = GetContentType(fileName)
        };
        
        return formFile;
    }
    
    /// <summary>
    /// 创建无效的测试文件（非图片格式）
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="content">文件内容</param>
    /// <returns>IFormFile 对象</returns>
    public static IFormFile CreateInvalidTestFile(string fileName = "test.txt", string content = "This is not an image")
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        
        var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };
        
        return formFile;
    }
    
    /// <summary>
    /// 创建大尺寸测试图片
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="width">图片宽度</param>
    /// <param name="height">图片高度</param>
    /// <returns>IFormFile 对象</returns>
    public static IFormFile CreateLargeTestFormFile(string fileName = "large_test.jpg", int width = 2000, int height = 2000)
    {
        return CreateTestFormFile(fileName, width, height, Color.Green);
    }
    
    /// <summary>
    /// 根据文件扩展名获取 Content-Type
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <returns>Content-Type 字符串</returns>
    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
    
    /// <summary>
    /// 从字节数组创建 IFormFile
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="content">文件内容</param>
    /// <param name="contentType">内容类型</param>
    /// <returns>IFormFile 对象</returns>
    public static IFormFile CreateFormFileFromBytes(string fileName, byte[] content, string contentType)
    {
        var stream = new MemoryStream(content);
        return new FormFile(stream, 0, content.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
    
    /// <summary>
    /// 创建临时目录
    /// </summary>
    /// <returns>临时目录路径</returns>
    public static string CreateTempDirectory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "ImageGliderWebApiTests_" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(tempDir);
        return tempDir;
    }
    
    /// <summary>
    /// 清理目录
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    public static void CleanupDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            try
            {
                Directory.Delete(directoryPath, true);
            }
            catch
            {
                // 忽略清理错误
            }
        }
    }
    
    /// <summary>
    /// 清理文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    public static void CleanupFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch
            {
                // 忽略清理错误
            }
        }
    }
}