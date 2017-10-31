using System.Collections.Generic;
using System.Linq;
using ReadyApi.Model.Enumaration;
using ReadyApi.Model.Helper;

namespace ReadyApi.Model
{
    public abstract class BaseResponse
    {
        public List<Message> Messages { get; set; } = new List<Message>();

        public bool Success => Messages.All(m => m.Type != MessageTypes.ErrorMessage);

        public void AddError(string message)
        {
            Messages.Add(new Message(MessageTypes.ErrorMessage, message));
        }

        public void AddInfo(string message)
        {
            Messages.Add(new Message(MessageTypes.InfoMessage, message));
        }
    }
}