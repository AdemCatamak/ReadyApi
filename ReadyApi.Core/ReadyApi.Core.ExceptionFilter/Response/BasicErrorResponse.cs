using System;

namespace ReadyApi.Core.ExceptionFilter.Response
{
    public class BasicErrorResponse
    {
        public string FriendlyMessage { get; private set; }

        public void AddErrorMessage(string message)
        {
            FriendlyMessage = string.IsNullOrEmpty(FriendlyMessage)
                                  ? message
                                  : $"{FriendlyMessage}{Environment.NewLine}{message}";
        }
    }
}