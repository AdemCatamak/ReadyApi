using ReadyApi.Model.Enumaration;

namespace ReadyApi.Model.Helper
{
    public class Message
    {
        public string MessageContent { get; set; }
        public MessageTypes Type { get; set; }

        public Message(MessageTypes type, string message)
        {
            Type = type;
            MessageContent = message;
        }
    }
}