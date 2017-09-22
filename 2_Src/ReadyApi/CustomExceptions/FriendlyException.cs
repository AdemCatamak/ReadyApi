using System;

namespace ReadyApi.CustomExceptions
{
    public class FriendlyException : Exception
    {
        public string FriendlyMessage { get; }

        public FriendlyException(string friendlyMessage) : base (null)
        {
            FriendlyMessage = friendlyMessage;
        }

        public FriendlyException(string friendlyMessage, Exception ex) : base(ex.Message, ex)
        {
            FriendlyMessage = friendlyMessage;
        }
    }
}