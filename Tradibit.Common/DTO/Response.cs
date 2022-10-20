using Newtonsoft.Json;
using Tradibit.Common.Extensions;

namespace Tradibit.Common.DTO;

/// <summary> Base Response class. Since Web is using Refit, needs to get clean json instead of ActionResult </summary>
public class Response
{
    /// <summary> Is Request was successful </summary>
    public bool Success { get; set; }
        
    /// <summary> Error message in case of non successful request. Otherwise empty </summary>
    public string Message { get; set; }
    
    
    /// <summary> Error stack trace in case of non successful request. Otherwise empty </summary>
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string StackTrace { get; set; }
        
    /// <summary> Creates success result by default </summary>
    public Response()
    {
        Success = true;
    }
        
    /// <summary> Creates error result with <paramref name="error"/>> message </summary>
    public Response(Exception error)
    {
        Success = false;
        Message = error.GetAllMessages();
        StackTrace = error.StackTrace;
    }
}
/// <summary> Base Response template class with additional data </summary>
/// <typeparam name="T">Type of data</typeparam>
public class Response<T> : Response
{
    /// <summary> Custom data </summary>
    public T Data { get; set; }

    /// <summary>Empty ctor, Needed for Loyalty.AdminApi.Web side </summary>
    public Response()
    {
        Success = false;
    }
        
    /// <summary> Creates success result with data </summary>
    /// <param name="data"></param>
    public Response(T data)
    {
        Data = data;
    }
        
    /// <summary> Creates error result with <paramref name="error"/>> message </summary>
    public Response(Exception error): base(error){}
}