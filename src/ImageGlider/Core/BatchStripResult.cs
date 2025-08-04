namespace ImageGlider.Core;

/// <summary>
/// 批量元数据清理结果信息
/// </summary>
public class BatchStripResult
{
    /// <summary>
    /// 总文件数
    /// </summary>
    public int TotalFiles { get; set; }

    /// <summary>
    /// 成功处理的文件数
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// 失败处理的文件数
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// 处理的文件详情列表
    /// </summary>
    public List<ProcessedFileInfo> ProcessedFiles { get; set; } = new();

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 处理是否成功
    /// </summary>
    public bool Success => string.IsNullOrEmpty(ErrorMessage) && FailureCount == 0;
}

/// <summary>
/// 处理文件信息
/// </summary>
public class ProcessedFileInfo
{
    /// <summary>
    /// 源文件路径
    /// </summary>
    public string SourcePath { get; set; } = string.Empty;

    /// <summary>
    /// 目标文件路径
    /// </summary>
    public string TargetPath { get; set; } = string.Empty;

    /// <summary>
    /// 处理是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }
}