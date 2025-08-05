using ImageGlider.WebApi.Endpoints.Internal;
using ImageGlider.WebApi.Model;
using ImageGlider.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageGlider.WebApi.Endpoints;

public class ImageResizeEndpoints : IEndpoint
{
    public static void UseEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/resize");

        // 图片尺寸调整
        group.MapPost("/", ResizeImage)
            .DisableAntiforgery();

        // 生成缩略图
        group.MapPost("/thumbnail", GenerateThumbnail)
            .DisableAntiforgery();
    }

    [Tags("图片尺寸调整")]
    [EndpointDescription("调整图片尺寸")]
    private static async Task<IResult> ResizeImage(
        [FromForm] ImageResizeRequest request,
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

            // 执行尺寸调整
            var success = ImageConverter.ResizeImage(
                tempFilePath,
                outputPath,
                request.Width,
                request.Height,
                request.ResizeMode,
                request.Quality);

            if (success)
            {
                logger.LogInformation($"{DateTime.Now} 文件 {request.file.FileName} 尺寸调整成功");
                fileService.DeleteFile(tempFilePath);
                return Results.Json(new ApiResponse
                {
                    Message = "尺寸调整成功",
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
                    Message = "尺寸调整失败"
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "尺寸调整过程中发生错误");
            return Results.StatusCode(500);
        }
    }

    [Tags("图片尺寸调整")]
    [EndpointDescription("生成图片缩略图")]
    private static async Task<IResult> GenerateThumbnail(
        [FromForm] ThumbnailRequest request,
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

            // 生成缩略图
            var success = ImageConverter.GenerateThumbnail(
                tempFilePath,
                outputPath,
                request.MaxSize,
                request.Quality);

            if (success)
            {
                logger.LogInformation($"{DateTime.Now} 文件 {request.file.FileName} 缩略图生成成功");
                fileService.DeleteFile(tempFilePath);
                return Results.Json(new ApiResponse
                {
                    Message = "缩略图生成成功",
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
                    Message = "缩略图生成失败"
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "缩略图生成过程中发生错误");
            return Results.StatusCode(500);
        }
    }

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileService, FileService>();
    }
}