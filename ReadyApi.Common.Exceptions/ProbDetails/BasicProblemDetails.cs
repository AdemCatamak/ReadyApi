namespace ReadyApi.Common.Exceptions.ProbDetails
{
    public class BasicProblemDetails : ProblemDetails
    {
        public BasicProblemDetails(string title) : this(title, string.Empty)
        {
        }

        public BasicProblemDetails(string title, string detail) : base(title, detail)
        {
        }
    }
}