using System;
using System.Threading.Tasks;

namespace ImageGlider.Cli.Commands
{
    /// <summary>
    /// 命令基类，提供通用的参数解析和错误处理功能
    /// </summary>
    public abstract class BaseCommand : ICommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 命令描述
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        public async Task<int> ExecuteAsync(string[] args)
        {
            try
            {
                return await ExecuteInternalAsync(args);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"参数错误: {ex.Message}");
                ShowHelp();
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行失败: {ex.Message}");
                return 1;
            }
        }

        /// <summary>
        /// 内部执行逻辑，由子类实现
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码，0表示成功</returns>
        protected abstract Task<int> ExecuteInternalAsync(string[] args);

        /// <summary>
        /// 显示命令帮助信息
        /// </summary>
        public abstract void ShowHelp();

        /// <summary>
        /// 解析字符串参数
        /// </summary>
        /// <param name="args">参数数组</param>
        /// <param name="index">当前索引</param>
        /// <param name="paramName">参数名称</param>
        /// <returns>参数值</returns>
        protected string ParseStringParameter(string[] args, ref int index, string paramName)
        {
            if (index + 1 >= args.Length)
            {
                throw new ArgumentException($"参数 {paramName} 缺少值");
            }
            return args[++index];
        }

        /// <summary>
        /// 解析整数参数
        /// </summary>
        /// <param name="args">参数数组</param>
        /// <param name="index">当前索引</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>参数值</returns>
        protected int ParseIntParameter(string[] args, ref int index, string paramName, int min = int.MinValue, int max = int.MaxValue)
        {
            var valueStr = ParseStringParameter(args, ref index, paramName);
            if (!int.TryParse(valueStr, out int value))
            {
                throw new ArgumentException($"参数 {paramName} 必须是有效的整数");
            }
            if (value < min || value > max)
            {
                throw new ArgumentException($"参数 {paramName} 必须在 {min} 到 {max} 之间");
            }
            return value;
        }

        /// <summary>
        /// 验证必需参数
        /// </summary>
        /// <param name="value">参数值</param>
        /// <param name="paramName">参数名称</param>
        protected void ValidateRequiredParameter(string? value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"缺少必需参数: {paramName}");
            }
        }
    }
}