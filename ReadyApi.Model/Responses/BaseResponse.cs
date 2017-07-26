using System.Collections.Generic;

namespace ReadyApi.Model.Responses
{
    public abstract class BaseResponse
    {
        public List<string> ErrorMessageList { get; } = new List<string>();
        public List<string> InfoMessageList { get; } = new List<string>();
        public bool Success { get; private set; } = true;

        public void AddErrorMessage(string message)
        {
            Success = false;
            ErrorMessageList.Add(message);
        }
        public void AddInfoMessage(string message)
        {
            InfoMessageList.Add(message);
        }
    }
}
