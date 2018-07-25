using System;

namespace ReadyApi.Common.Model
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
