using System;
using System.Threading.Tasks;
using ImageGlider.Cli.Commands;

/// <summary>
/// ImageGlider 命令行工具入口点
/// </summary>
class Program
{
    /// <summary>
    /// 程序主入口点
    /// </summary>
    /// <param name="args">命令行参数</param>
    static async Task<int> Main(string[] args)
    {
        var commandRegistry = ConfigureCommands();
        return await commandRegistry.ExecuteAsync(args);
    }

    /// <summary>
    /// 配置和注册所有命令
    /// </summary>
    /// <returns>配置好的命令注册器</returns>
    private static ICommandRegistry ConfigureCommands()
    {
        var registry = new CommandRegistry();
        
        // 注册所有命令
        registry.RegisterCommand(new ConvertCommand());
        registry.RegisterCommand(new BatchConvertCommand());
        registry.RegisterCommand(new ResizeCommand());
        registry.RegisterCommand(new BatchResizeCommand());
        registry.RegisterCommand(new CompressCommand());
        registry.RegisterCommand(new BatchCompressCommand());
        registry.RegisterCommand(new CropCommand());
        registry.RegisterCommand(new BatchCropCommand());
        registry.RegisterCommand(new ThumbnailCommand());
        registry.RegisterCommand(new BatchThumbnailCommand());
        registry.RegisterCommand(new WatermarkCommand());
        registry.RegisterCommand(new BatchWatermarkCommand());
        
        return registry;
    }
}
