using System.Collections.Generic;
using ReadyApi.Model.Enumaration;
using ReadyApi.Model.Helper;

namespace ReadyApi.Model.Responses
{
    public abstract class BaseResponse
    {
        public List<Message> MessageList { get; } = new List<Message>();
        public bool Success { get; private set; } = true;

        public void AddErrorMessage(string message)
        {
            Success = false;
            MessageList.Add(new Message(MessageTypes.ErrorMessage, message));
        }
        public void AddInfoMessage(string message)
        {
            MessageList.Add(new Message(MessageTypes.InfoMessage, message));
        }
    }
}
