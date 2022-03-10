
namespace Cu.Yemekhane.Common;
public struct ApiResponse<T>
{
    public string ErrorMessage { get; set; }
    public T? Data { get; set; }
}