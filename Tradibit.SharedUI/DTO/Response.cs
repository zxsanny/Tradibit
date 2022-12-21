namespace Tradibit.SharedUI.DTO;

public class Response
{
    public bool Success { get; set; }
    public string Message { get; set; }
    
    public string StackTrace { get; set; }
        
    public Response()
    {
        Success = true;
    }
        
    public Response(Exception error)
    {
        Success = false;
        Message = error.GetAllMessages();
        StackTrace = error.StackTrace;
    }
}

public class Response<T> : Response
{
    public T Data { get; set; }
    
    public Response(T data)
    {
        Data = data;
    }
}

public class PagedResponse<T> : Response
{
    public int TotalCount { get; set; }
    public IReadOnlyList<T> Data { get; set; } = new List<T>();
}