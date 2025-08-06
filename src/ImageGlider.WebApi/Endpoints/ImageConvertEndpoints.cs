using ImageGlider.WebApi.Endpoints.Internal;
using ImageGlider.WebApi.Model;
using ImageGlider.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageGlider.WebApi.Endpoints;

public class ImageConvertEndpoints : IEndpoint
{
    public static void UseEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/convert");
        // 图片格式转换
        group.MapPost("/", ConvertImage)
            .DisableAntiforgery();
    }

    [Tags("图片格式转换")]
    [EndpointDescription("上传图片文件并返回文件名")]
    private static async Task<IResult> ConvertImage(
        [FromForm] ImageConvertRequest request,
        IFileService fileService,
        ILogger<Program> logger)
    {
        if (!fileService.IsFileValid(request.file))
        {
            return Results.BadRequest(new ApiResponse
            {
                StatusCode = 400,
                Successful = false,
                Message = "图片格式不支持"
            });
        }

        var (tempFilePath, outputPath) = await fileService.SaveUploadedFile(request.file, request.FileExt.ToLower());

        // 执行转换
        ImageConverter.ConvertImage(tempFilePath, outputPath, request.Quality);

        logger.LogInformation($"{DateTime.Now} 文件 {request.file.FileName} 转换成功");

        fileService.DeleteFile(tempFilePath);
        // 返回文件名
        return Results.Json(new ApiResponse { Message = "转换成功", Data = fileService.GetFileName(outputPath) });
    }

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileService, FileService>();
    }
}