using System.Text;

namespace Tradibit.SharedUI.Extensions;

/// <summary> Exceptions extensions </summary>
public static class ExceptionExtensions
{
    /// <summary> Gets all exceptions messages, including all inner exceptions, line by line </summary>
    public static string GetAllMessages(this Exception e)
    {
        var currentException = e;
        var message = new StringBuilder();
        while (currentException != null)
        {
            message.AppendLine(currentException.Message);
            currentException = currentException.InnerException;
        }
        return message.ToString();
    }
}