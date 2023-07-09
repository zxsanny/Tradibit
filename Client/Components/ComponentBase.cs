using System.Net;
using Microsoft.AspNetCore.Components;
using Radzen;
using Refit;

namespace Tradibit.Client.Components;

public class SenderBase : ComponentBase
{
    [Inject]
    protected NotificationService? NoticeService { get; set; }
    
    [Inject]
    protected NavigationManager? Navigation { get; set; }

    protected async Task Send(Func<Task>? func)
    {
        try
        {
            if (func != null) 
                await func();
        }
        catch (ApiException e)
        {
            if (e.StatusCode == HttpStatusCode.Unauthorized)
                Navigation?.NavigateTo("/Account/Login", true);
            ShowError(e.Content ?? "", e.StackTrace ?? "");
        }
        catch (Exception e)
        {
            ShowError(e.Message ?? "");
        }
    }

    protected async Task<T?> Send<T>(Func<Task<T>> func)
    {
        try
        {
            return await func();
        }
        catch (ApiException e)
        {
            if (e.StatusCode == HttpStatusCode.Unauthorized)
            {
                Navigation?.NavigateTo("/Account/Login", true);
                return default;
            }
            
            ShowError(e.Content ?? "", e.StackTrace ?? "");
            return default;
        }
        catch (Exception e)
        {
            ShowError(e.Message ?? "");
            return default;
        }
    }

    private void ShowError(string message, string? stackTrace = null)
    {
        var notificationMessage =
#if DEBUG
            new NotificationMessage
            {
                Summary = $"Error! {message}",
                Detail = stackTrace ?? "",
                Duration = TimeSpan.FromSeconds(50).TotalMilliseconds,
                Style = "width: 80%;"
            };
#else
            new NotificationMessage
            {


                Summary = $"Error!",
                Detail = message,
                Duration = TimeSpan.FromSeconds(5).TotalMilliseconds
            };
#endif
        NoticeService?.Notify(notificationMessage);
    }
}
