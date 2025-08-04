namespace ImageGlider.Core;

/// <summary>
/// 批量转换结果信息
/// </summary>
public class ConversionResult
{
    /// <summary>
    /// 总文件数
    /// </summary>
    public int TotalFiles { get; set; }

    /// <summary>
    /// 成功转换的文件数
    /// </summary>
    public int SuccessfulConversions { get; set; }

    /// <summary>
    /// 失败转换的文件数
    /// </summary>
    public int FailedConversions { get; set; }

    /// <summary>
    /// 成功转换的文件列表
    /// </summary>
    public List<string> SuccessfulFiles { get; set; } = new();

    /// <summary>
    /// 失败转换的文件列表
    /// </summary>
    public List<string> FailedFiles { get; set; } = new();

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 转换是否成功
    /// </summary>
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage) && FailedConversions == 0;
}