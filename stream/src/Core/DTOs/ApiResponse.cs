namespace Core.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}