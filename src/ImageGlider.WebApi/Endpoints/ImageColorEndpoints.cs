using ImageGlider.WebApi.Endpoints.Internal;
using ImageGlider.WebApi.Model;
using ImageGlider.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageGlider.WebApi.Endpoints;

public class ImageColorEndpoints : IEndpoint
{
    public static void UseEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/color");

        // 颜色调整
        group.MapPost("/adjust", AdjustColor)
            .DisableAntiforgery();
    }

    [Tags("图片颜色处理")]
    [EndpointDescription("调整图片颜色")]
    private static async Task<IResult> AdjustColor(
        [FromForm] ImageColorRequest request,
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

            // 执行颜色调整
            var success = ImageGlider.ImageConverter.AdjustColor(
                tempFilePath,
                outputPath,
                request.Brightness,
                request.Contrast,
                request.Saturation,
                request.Hue,
                request.Gamma,
                request.Quality);

            if (success)
            {
                logger.LogInformation($"{DateTime.Now} 文件 {request.file.FileName} 颜色调整成功");
                fileService.DeleteFile(tempFilePath);
                return Results.Json(new ApiResponse
                {
                    Message = "颜色调整成功",
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
                    Message = "颜色调整失败"
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "颜色调整过程中发生错误");
            return Results.StatusCode(500);
        }
    }

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileService, FileService>();
    }
}