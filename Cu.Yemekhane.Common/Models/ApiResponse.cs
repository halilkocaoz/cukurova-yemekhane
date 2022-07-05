namespace Cu.Yemekhane.Common.Models;
public struct ApiResponse<T>
{
    public bool Success => string.IsNullOrEmpty(ErrorMessage);
    public string ErrorMessage { get; set; }
    public T? Data { get; set; }
}