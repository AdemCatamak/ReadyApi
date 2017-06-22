using System.Collections.Generic;

namespace AdemCatamak.Api.Model.Responses
{
    public abstract class BaseResponse
    {
        public List<string> ErrorMessageList { get; } = new List<string>();
        public bool Success { get; private set; } = true;

        public void AddErrorMessage(string message)
        {
            Success = false;
            ErrorMessageList.Add(message);
        }
    }
}
