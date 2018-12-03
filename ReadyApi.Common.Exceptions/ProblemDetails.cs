using System;

namespace ReadyApi.Common.Exceptions
{
    public abstract class ProblemDetails
    {
        public string Title { get; }
        public string Detail { get; }

        public ProblemDetails(string title) : this(title, string.Empty)
        {
        }

        public ProblemDetails(string title, string detail)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Argument should not be empty", nameof(title));

            Title = title;
            Detail = detail ?? string.Empty;
        }

        public override string ToString()
        {
            string message = $"Title : {Title}";

            if (!string.IsNullOrEmpty(Detail))
            {
                message += $"{Environment.NewLine}" +
                           $"Detail : {Detail}";
            }

            return message;
        }
    }
}