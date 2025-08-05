using ImageGlider.WebApi.Endpoints.Internal;
using ImageGlider.WebApi.Model;
using ImageGlider.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageGlider.WebApi.Endpoints;

public class ImageCropEndpoints : IEndpoint
{
    public static void UseEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/crop");

        // 精确裁剪
        group.MapPost("/", CropImage)
            .DisableAntiforgery();

        // 中心裁剪
        group.MapPost("/center", CropImageCenter)
            .DisableAntiforgery();

        // 百分比裁剪
        group.MapPost("/percent", CropImageByPercent)
            .DisableAntiforgery();
    }

    [Tags("图片裁剪")]
    [EndpointDescription("精确裁剪图片")]
    private static async Task<IResult> CropImage(
        [FromForm] ImageCropRequest request,
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

            // 执行裁剪
            var success = ImageConverter.CropImage(
                tempFilePath,
                outputPath,
                request.X,
                request.Y,
                request.Width,
                request.Height,
                request.Quality);

            if (success)
            {
                logger.LogInformation($"{DateTime.Now} 文件 {request.file.FileName} 裁剪成功");
                fileService.DeleteFile(tempFilePath);
                return Results.Json(new ApiResponse
                {
                    Message = "裁剪成功",
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
                    Message = "裁剪失败"
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "裁剪过程中发生错误");
            return Results.StatusCode(500);
        }
    }

    [Tags("图片裁剪")]
    [EndpointDescription("中心裁剪图片")]
    private static async Task<IResult> CropImageCenter(
        [FromForm] ImageCenterCropRequest request,
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

            // 执行中心裁剪
            var success = ImageConverter.CropImageCenter(
                tempFilePath,
                outputPath,
                request.Width,
                request.Height,
                request.Quality);

            if (success)
            {
                logger.LogInformation($"{DateTime.Now} 文件 {request.file.FileName} 中心裁剪成功");
                fileService.DeleteFile(tempFilePath);
                return Results.Json(new ApiResponse
                {
                    Message = "中心裁剪成功",
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
                    Message = "中心裁剪失败"
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "中心裁剪过程中发生错误");
            return Results.StatusCode(500);
        }
    }

    [Tags("图片裁剪")]
    [EndpointDescription("按百分比裁剪图片")]
    private static async Task<IResult> CropImageByPercent(
        [FromForm] ImagePercentCropRequest request,
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

            // 执行百分比裁剪
            var success = ImageConverter.CropImageByPercent(
                tempFilePath,
                outputPath,
                request.XPercent,
                request.YPercent,
                request.WidthPercent,
                request.HeightPercent,
                request.Quality);

            if (success)
            {
                logger.LogInformation($"{DateTime.Now} 文件 {request.file.FileName} 百分比裁剪成功");
                fileService.DeleteFile(tempFilePath);
                return Results.Json(new ApiResponse
                {
                    Message = "百分比裁剪成功",
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
                    Message = "百分比裁剪失败"
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "百分比裁剪过程中发生错误");
            return Results.StatusCode(500);
        }
    }

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileService, FileService>();
    }
}