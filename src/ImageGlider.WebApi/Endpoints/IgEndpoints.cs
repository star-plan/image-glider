using ImageGlider.WebApi.Endpoints.Internal;
using ImageGlider.WebApi.Model;
using ImageGlider.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageGlider.WebApi.Endpoints;

public class IgEndpoints: IEndpoint
{
    public static void UseEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api");
        // 上传图片文件完成转换并返回转换后文件名
        group.MapPost("/upload", UploadFile)
            .DisableAntiforgery();
        // 下载转换后的图片文件格式，并返回文件流
        group.MapPost("/dowloads/{fileName}", DownloadFile);
    }

    [EndpointDescription("上传图片文件并返回文件名")]
    private static async Task<IResult> UploadFile(
        [FromForm] IFormFile file,
        string fileExt,
        int? quality,
        IFileService fileService,
        ILogger<Program> logger)
    {
        if (!fileService.IsFileValid(file))
        {
            throw new ApplicationException("图片格式不支持");
        }
        
        var (tempFilePath, outputPath) = await fileService.SaveUploadedFile(file, fileExt.ToLower());
        
        // 执行转换
        ImageConverter.ConvertImage(tempFilePath, outputPath, quality ?? 80);
        
        logger.LogInformation($"{DateTime.Now} 文件 {file.FileName} 转换成功");
        
        fileService.DeleteFile(tempFilePath);
        // 返回文件名
        return Results.Json(new ApiResponse { Message = "转换成功", Data = fileService.GetFileName(outputPath) });
    }
    [EndpointDescription("下载转换后的图片文件")]
    private static async Task<IResult> DownloadFile(
        string fileName,
        IFileService fileService,
        ILogger<Program> logger)
    {
        var (mimeType, memoryStream) = await fileService.ReadFileToMemoryAsync(fileName);
        return Results.File(memoryStream, mimeType, fileName);
    }

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileService, FileService>();
    }
}