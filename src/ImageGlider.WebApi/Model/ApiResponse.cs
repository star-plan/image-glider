namespace ImageGlider.WebApi.Model;

public class ApiResponse
{
    public int StatusCode { get; set; } = 200;
    public bool Successful { get; set; } = true;
    public string? Message { get; set; } = "操作成功";
    public object? Data { get; set; }
}