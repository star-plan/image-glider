namespace ImageGlider.WebApi.Services;

public class FileService:IFileService
{
    /// <summary>
    /// 保存上传的文件
    /// </summary>
    /// <param name="file">文件</param>
    /// <param name="fileExt">文件扩展名</param>
    /// <returns>(原文件路径, 输出路径)</returns>
    public async Task<(string, string)> SaveUploadedFile(IFormFile file, string? fileExt)
    {
        var tempDir = GetRootDirectory();
        var tempFilePath = Path.Combine(tempDir, $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
        
        await using var stream = new FileStream(tempFilePath, FileMode.Create);
        await file.CopyToAsync(stream);
        
        // 生成输出路径
        var extension = !string.IsNullOrEmpty(fileExt) 
            ? fileExt.StartsWith(".") ? fileExt : $".{fileExt}" 
            : Path.GetExtension(file.FileName);

        var outName = $"{Guid.NewGuid()}{extension}";
        var outputPath = Path.Combine(GetRootDirectory("output"), outName);
        
        return (tempFilePath, outputPath);
    }
    /// <summary>
    /// 读取文件到内存
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <returns>(文件类型, 文件数据)</returns>
    public async Task<(string,MemoryStream)> ReadFileToMemoryAsync(string fileName)
    {
        var filePath = Path.Combine(GetRootDirectory("output"), fileName);
        if (!File.Exists(filePath))
            throw new FileNotFoundException("文件不存在");
        
        var mimeType = GetMimeType(filePath);
        var memoryStream = new MemoryStream();
        await using (var fileStream = File.OpenRead(filePath))
        {
            await fileStream.CopyToAsync(memoryStream);
        }
        memoryStream.Position = 0;
        
        // todo: 删除转换后文件暂时未想到好的方法
        // DeleteFile(filePath);
        return (mimeType, memoryStream);
    }
    public string GetFileName(string filePath)
    {
        return Path.GetFileName(filePath);
    }
    // 文件格式验证
    public bool IsFileValid(IFormFile file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp" }; // 支持的图片格式
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return file.Length is > 0 and < 10 * 1024 * 1024 && // 10MB
               allowedExtensions.Contains(extension);
    }
    // 删除文件
    public void DeleteFile(string tempFilePath)
    {
        if (File.Exists(tempFilePath))
            File.Delete(tempFilePath);
    }
    // 返回根目录
    private string GetRootDirectory(string subDir = "source")
    {
        // wwwrooo/source
        var rootDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", subDir);
        Directory.CreateDirectory(rootDir);
        return rootDir;
    }
    // 根据文件扩展名获取MIME类型
    private static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".tiff" => "image/tiff",
            ".webp" => "image/.webp",
            _ => throw new ArgumentException("不支持的图片格式")
        };
    }
}