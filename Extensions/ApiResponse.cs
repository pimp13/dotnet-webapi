namespace MyFirstApi.Extensions;


public class ApiResponse<T>
{
  public bool Ok { get; set; }
  public string? Message { get; set; }
  public T? Data { get; set; }

  public ApiResponse(bool ok, string? message, T? data = default)
  {
    Ok = ok;
    Message = message;
    Data = data;
  }

  public static ApiResponse<T> Success(T? data, string message = "Success")
      => new(true, message, data);

  public static ApiResponse<T> Fail(string message)
      => new(false, message);

}