using System.Net;
using Microsoft.AspNetCore.Components;
using Radzen;
using Refit;
using Tradibit.Common.DTO;

namespace Tradibit.Client.Shared
{
    public class RequestExt
    {
        private readonly NotificationService _noticeService;
        private readonly NavigationManager _navigation;

        public RequestExt(NotificationService noticeService, NavigationManager navigation)
        {
            _noticeService = noticeService;
            _navigation = navigation;

        }

        public async Task<Response?> Send(Func<Task<Response?>> func) => 
            await SendInternal(func);

        public async Task<T?> Send<T>(Func<Task<Response<T>?>> func)
        {
            var response = await SendInternal(func);
            return response == null ? default : response.Data;
        }

        public async Task<PagedResponse<T>?> Send<T>(Func<Task<PagedResponse<T>?>> func) =>
            await SendInternal(func);

        private async Task<T?> SendInternal<T>(Func<Task<T>> func) where T: Response?
        {
            T? resp = default;
            try
            {
                resp = await func();
            }
            catch (ApiException e)
            {
                if (e.StatusCode == HttpStatusCode.Unauthorized) 
                    _navigation.NavigateTo("/Account/Login", true);
                else
                {
                    await ShowError(e.Content ?? "", e.StackTrace ?? "");
                    return default;
                }
            }
            if (resp?.Success ?? false)
                return resp;

            await ShowError(resp?.Message ?? "", resp?.StackTrace ?? "");
            return default;
        }
        
#pragma warning disable CS1998
        public async Task ShowError(string message, string stackTrace = null)
#pragma warning restore CS1998
        {
            var notificationMessage =
#if DEBUG
                new NotificationMessage
                {
                    Summary = $"Error! {message}",
                    Detail = stackTrace ?? "",
                    Duration = TimeSpan.FromMilliseconds(30000).TotalMilliseconds,
                    Style = "width: 80%;"
                };
#else
                new NotificationMessage
                {
                    Summary = $"Error!",
                    Detail = message,
                    Duration = TimeSpan.FromMilliseconds(30000).TotalMilliseconds
                };
#endif
            //deliberately not awaiting
#pragma warning disable CS4014
            _noticeService.Notify(notificationMessage);
#pragma warning restore CS4014
        }
    }
}