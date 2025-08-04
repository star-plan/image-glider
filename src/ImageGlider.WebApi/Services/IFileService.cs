namespace ImageGlider.WebApi.Services;

public interface IFileService
{
    // 保存上传的文件, 并返回文件路径和输出路径
    Task<(string, string)> SaveUploadedFile(IFormFile file, string fileExt);
    // 读取文件到内存流, 并返回文件类型和内存流
    Task<(string, MemoryStream)> ReadFileToMemoryAsync(string fileName);
    // 解析路径返回文件名
    string GetFileName(string filePath);
    // 文件格式验证
    bool IsFileValid(IFormFile file);
    // 删除文件
    void DeleteFile(string tempFilePath);
}