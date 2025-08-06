using ImageGlider.WebApi.Endpoints.Internal;
using ImageGlider.WebApi.Services;

namespace ImageGlider.WebApi.Endpoints;

public class ImageDownload : IEndpoint
{
    public static void UseEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/download");
        // 下载转换后的图片文件格式，并返回文件流
        group.MapGet("/{fileName}", DownloadFile);
    }

    [Tags("下载图片")]
    [EndpointDescription("下载转换后的图片文件")]
    private static async Task<IResult> DownloadFile(
        string fileName,
        IFileService fileService,
        ILogger<Program> logger,
        HttpContext context)
    {
        try
        {
            var (mimeType, memoryStream) = await fileService.ReadFileToMemoryAsync(fileName);
            
            // 添加缓存控制头
            context.Response.Headers.CacheControl = "public, max-age=3600";
            
            return Results.File(memoryStream, mimeType, fileName);
        }
        catch (FileNotFoundException)
        {
            return Results.NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "下载文件时发生错误: {FileName}", fileName);
            return Results.StatusCode(500);
        }
    }

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileService, FileService>();
    }
}