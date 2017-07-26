using ReadyApi.Model.Enumaration;

namespace ReadyApi.Model.Helper
{
    public class Message
    {
        public Message(MessageTypes type, string content)
        {
            Type = type;
            Content = content;
        }

        public MessageTypes Type { get; private set; }
        public string Content { get; private set; }
    }
}