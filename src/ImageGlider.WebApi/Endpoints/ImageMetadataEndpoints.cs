using ImageGlider.WebApi.Endpoints.Internal;
using ImageGlider.WebApi.Model;
using ImageGlider.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageGlider.WebApi.Endpoints;

public class ImageMetadataEndpoints : IEndpoint
{
    public static void UseEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/metadata");

        // 清理元数据
        group.MapPost("/strip", StripMetadata)
            .DisableAntiforgery();
    }

    [Tags("图片元数据处理")]
    [EndpointDescription("清理图片元数据")]
    private static async Task<IResult> StripMetadata(
        [FromForm] ImageMetadataRequest request,
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

        try
        {
            var originalExtension = Path.GetExtension(request.file.FileName);
            var (tempFilePath, outputPath) = await fileService.SaveUploadedFile(request.file, originalExtension);

            // 清理元数据
            var success = ImageGlider.ImageConverter.StripMetadata(
                tempFilePath,
                outputPath,
                request.StripAll,
                request.StripExif,
                request.StripIcc,
                request.StripXmp,
                request.Quality);

            if (success)
            {
                logger.LogInformation($"{DateTime.Now} 文件 {request.file.FileName} 元数据清理成功");
                fileService.DeleteFile(tempFilePath);
                return Results.Json(new ApiResponse
                {
                    Message = "元数据清理成功",
                    Data = fileService.GetFileName(outputPath)
                });
            }
            else
            {
                fileService.DeleteFile(tempFilePath);
                fileService.DeleteFile(outputPath);
                return Results.BadRequest(new ApiResponse
                {
                    StatusCode = 400,
                    Successful = false,
                    Message = "元数据清理失败"
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "元数据清理过程中发生错误");
            return Results.StatusCode(500);
        }
    }

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileService, FileService>();
    }
}