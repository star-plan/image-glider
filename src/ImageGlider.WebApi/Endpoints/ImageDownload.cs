using ImageGlider.WebApi.Endpoints.Internal;
using ImageGlider.WebApi.Services;

namespace ImageGlider.WebApi.Endpoints;

public class ImageDownload : IEndpoint
{
    public static void UseEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/downloads");
        // 下载转换后的图片文件格式，并返回文件流
        group.MapPost("/{fileName}", DownloadFile);
    }

    [Tags("下载图片")]
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