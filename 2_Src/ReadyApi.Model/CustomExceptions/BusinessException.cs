using System;

namespace ReadyApi.Model.CustomExceptions
{
    public class BusinessException : Exception
    {
        public string ErrorMessage { get; }

        public BusinessException(string errorMessage) : base(null)
        {
            ErrorMessage = errorMessage;
        }

        public BusinessException(string errorMessage, Exception ex) : base(ex.Message, ex)
        {
            ErrorMessage = errorMessage;
        }
    }
}