using System.Threading.Tasks;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 命令接口，定义所有CLI命令的基本契约
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 命令描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        Task<int> ExecuteAsync(string[] args);

        /// <summary>
        /// 显示命令帮助信息
        /// </summary>
        void ShowHelp();
    }
}