namespace SpiderNet.Domain.Common;

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T Data { get; set; } = default!;
    public string ErrorMessage { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new List<string>();
    
    public static Result<T> Success(T data) => new() {IsSuccess = true, Data = data};
    public static Result<T> Failure(string errorMessage) => new() {IsSuccess = false, ErrorMessage = errorMessage};
    public static Result<T> Failure(List<string> errors) => new() {IsSuccess = false, Errors = errors};
}