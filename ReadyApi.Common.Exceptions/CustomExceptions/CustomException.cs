using System;
using System.Collections.Generic;
using ReadyApi.Common.Exceptions.ProbDetails;

namespace ReadyApi.Common.Exceptions.CustomExceptions
{
    public class CustomException : Exception
    {
        public List<ExceptionTags> ExceptionTags { get; } = new List<ExceptionTags>();
        public ProblemDetails ProblemDetail { get; }

        public CustomException(string message) : this(new BasicProblemDetails(message))
        {
        }

        public CustomException(ProblemDetails problemDetail) :
            this(problemDetail, new ExceptionTags[0])
        {
        }

        public CustomException(ProblemDetails problemDetail, Exception ex) : this(problemDetail, ex, new ExceptionTags[0])
        {
        }

        public CustomException(ProblemDetails problemDetail, params ExceptionTags[] exceptionTags) : this(problemDetail, null, exceptionTags)
        {
        }

        public CustomException(ProblemDetails problemDetail, Exception ex, params ExceptionTags[] exceptionTags) : base(problemDetail?.Title, ex)
        {
            ProblemDetail = problemDetail ?? throw new ArgumentException("Parameter should not be null", nameof(problemDetail));
            ExceptionTags.AddRange(exceptionTags ?? new ExceptionTags[0]);
        }

        public override string ToString()
        {
            string message = $"{ProblemDetail}{Environment.NewLine}" +
                             $"{base.ToString()}";
            return message;
        }
    }
}