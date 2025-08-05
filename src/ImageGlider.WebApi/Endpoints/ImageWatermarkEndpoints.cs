using ImageGlider.WebApi.Endpoints.Internal;
using ImageGlider.WebApi.Model;
using ImageGlider.WebApi.Services;
using ImageGlider.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ImageGlider.WebApi.Endpoints;

public class ImageWatermarkEndpoints : IEndpoint
{
    public static void UseEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/watermark");

        // 添加文本水印
        group.MapPost("/text", AddTextWatermark)
            .DisableAntiforgery();

        // 添加图片水印
        group.MapPost("/image", AddImageWatermark)
            .DisableAntiforgery();
    }

    [Tags("添加水印")]
    [EndpointDescription("添加文本水印")]
    private static async Task<IResult> AddTextWatermark(
        [FromForm] TextWatermarkRequest request,
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

        if (string.IsNullOrEmpty(request.Text))
        {
            return Results.BadRequest(new ApiResponse
            {
                StatusCode = 400,
                Successful = false,
                Message = "水印文本不能为空"
            });
        }

        try
        {
            var originalExtension = Path.GetExtension(request.file.FileName);
            var (tempFilePath, outputPath) = await fileService.SaveUploadedFile(request.file, originalExtension);

            // 添加文本水印
            var success = ImageGlider.ImageConverter.AddTextWatermark(
                tempFilePath,
                outputPath,
                request.Text,
                request.Position,
                request.Opacity,
                request.FontSize,
                request.FontColor,
                request.Quality);

            if (success)
            {
                logger.LogInformation($"{DateTime.Now} 文件 {request.file.FileName} 添加文本水印成功");
                fileService.DeleteFile(tempFilePath);
                return Results.Json(new ApiResponse
                {
                    Message = "文本水印添加成功",
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
                    Message = "文本水印添加失败"
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "添加文本水印过程中发生错误");
            return Results.StatusCode(500);
        }
    }

    [Tags("添加水印")]
    [EndpointDescription("添加图片水印")]
    private static async Task<IResult> AddImageWatermark(
        [FromForm] ImageWatermarkRequest request,
        IFileService fileService,
        ILogger<Program> logger)
    {
        if (!fileService.IsFileValid(request.file) || !fileService.IsFileValid(request.watermarkFile))
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
            var watermarkExtension = Path.GetExtension(request.watermarkFile.FileName);
            var (tempFilePath, outputPath) = await fileService.SaveUploadedFile(request.file, originalExtension);
            var (watermarkTempPath, _) = await fileService.SaveUploadedFile(request.watermarkFile, watermarkExtension);

            // 添加图片水印
            var success = ImageGlider.ImageConverter.AddImageWatermark(
                tempFilePath,
                outputPath,
                watermarkTempPath,
                request.Position,
                request.Opacity,
                request.Scale,
                request.Quality);

            if (success)
            {
                logger.LogInformation($"{DateTime.Now} 文件 {request.file.FileName} 添加图片水印成功");
                fileService.DeleteFile(tempFilePath);
                fileService.DeleteFile(watermarkTempPath);
                return Results.Json(new ApiResponse
                {
                    Message = "图片水印添加成功",
                    Data = fileService.GetFileName(outputPath)
                });
            }
            else
            {
                fileService.DeleteFile(tempFilePath);
                fileService.DeleteFile(watermarkTempPath);
                fileService.DeleteFile(outputPath);
                return Results.BadRequest(new ApiResponse
                {
                    StatusCode = 400,
                    Successful = false,
                    Message = "图片水印添加失败"
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "添加图片水印过程中发生错误");
            return Results.StatusCode(500);
        }
    }

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileService, FileService>();
    }
}