using ImageGlider.WebApi.Endpoints.Internal;
using ImageGlider.WebApi.Model;
using ImageGlider.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageGlider.WebApi.Endpoints;

public class ImageInfoEndpoints : IEndpoint
{
    public static void UseEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/info");

        // 获取图片信息
        group.MapPost("/", GetImageInfo)
            .DisableAntiforgery();
    }

    [Tags("图片信息处理")]
    [EndpointDescription("获取图片信息")]
    private static async Task<IResult> GetImageInfo(
        [FromForm] IFormFile file,
        IFileService fileService,
        ILogger<Program> logger)
    {
        if (!fileService.IsFileValid(file))
        {
            return Results.BadRequest(new ApiResponse
            {
                StatusCode = 400,
                Successful = false,
                Message = "图片格式不支持"
            });
        }

        string? tempFilePath = null;

        try
        {
            var originalExtension = Path.GetExtension(file.FileName);
            var (tempPath, _) = await fileService.SaveUploadedFile(file, originalExtension);
            tempFilePath = tempPath;

            // 获取图片信息
            var imageInfo = ImageConverter.GetImageInfo(tempFilePath);

            if (imageInfo != null)
            {
                logger.LogInformation($"{DateTime.Now} 文件 {file.FileName} 信息提取成功");
                return Results.Json(new ApiResponse
                {
                    Message = "图片信息获取成功",
                    Data = imageInfo
                });
            }
            else
            {
                return Results.BadRequest(new ApiResponse
                {
                    StatusCode = 400,
                    Successful = false,
                    Message = "无法获取图片信息"
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取图片信息过程中发生错误");
            return Results.StatusCode(500);
        }
        finally
        {
            if (!string.IsNullOrEmpty(tempFilePath))
            {
                fileService.DeleteFile(tempFilePath);
            }
        }
    }

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileService, FileService>();
    }
}