using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 命令注册器接口
    /// </summary>
    public interface ICommandRegistry
    {
        /// <summary>
        /// 注册命令
        /// </summary>
        /// <param name="command">要注册的命令</param>
        void RegisterCommand(ICommand command);

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        Task<int> ExecuteAsync(string[] args);

        /// <summary>
        /// 显示所有命令的帮助信息
        /// </summary>
        void ShowHelp();
    }

    /// <summary>
    /// 命令注册器实现
    /// </summary>
    public class CommandRegistry : ICommandRegistry
    {
        private readonly Dictionary<string, ICommand> _commands = new();

        /// <summary>
        /// 注册命令
        /// </summary>
        /// <param name="command">要注册的命令</param>
        public void RegisterCommand(ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            _commands[command.Name.ToLowerInvariant()] = command;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        public async Task<int> ExecuteAsync(string[] args)
        {
            Console.WriteLine("=== ImageGlider 命令行工具 ===");
            Console.WriteLine();

            if (args.Length == 0)
            {
                ShowHelp();
                return 0;
            }

            var commandName = args[0].ToLowerInvariant();

            // 处理帮助命令
            if (commandName == "help" || commandName == "--help" || commandName == "-h")
            {
                if (args.Length > 1)
                {
                    // 显示特定命令的帮助
                    var specificCommand = args[1].ToLowerInvariant();
                    if (_commands.TryGetValue(specificCommand, out var cmd))
                    {
                        cmd.ShowHelp();
                        return 0;
                    }
                    else
                    {
                        Console.WriteLine($"未知命令: {specificCommand}");
                        ShowHelp();
                        return 1;
                    }
                }
                else
                {
                    ShowHelp();
                    return 0;
                }
            }

            // 查找并执行命令
            if (_commands.TryGetValue(commandName, out var command))
            {
                return await command.ExecuteAsync(args);
            }
            else
            {
                Console.WriteLine($"未知命令: {commandName}");
                Console.WriteLine();
                ShowHelp();
                return 1;
            }
        }

        /// <summary>
        /// 显示所有命令的帮助信息
        /// </summary>
        public void ShowHelp()
        {
            Console.WriteLine("用法:");
            Console.WriteLine("  ImageGlider.Cli <命令> [选项]");
            Console.WriteLine("  ImageGlider.Cli help [命令]    显示帮助信息");
            Console.WriteLine();
            Console.WriteLine("可用命令:");

            var maxNameLength = _commands.Values.Max(c => c.Name.Length);
            foreach (var command in _commands.Values.OrderBy(c => c.Name))
            {
                Console.WriteLine($"  {command.Name.PadRight(maxNameLength)}  {command.Description}");
            }

            Console.WriteLine();
            Console.WriteLine("使用 'ImageGlider.Cli help <命令>' 查看特定命令的详细帮助。");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  ImageGlider.Cli convert -s image.jfif -t image.jpeg -q 85");
            Console.WriteLine("  ImageGlider.Cli batch -se .jfif -te .jpeg -q 90");
            Console.WriteLine("  ImageGlider.Cli resize -s input.jpg -t output.jpg -w 800 -h 600");
            Console.WriteLine("  ImageGlider.Cli compress -s input.jpg -t output.jpg -l 60");
            Console.WriteLine("  ImageGlider.Cli help convert");
        }
    }
}