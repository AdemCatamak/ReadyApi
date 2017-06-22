using System.Net.Http;
using System.Web.Http.Filters;
using AdemCatamak.Api.Model.Responses;
using AdemCatamak.Api.Model.Responses.Imp;
using AdemCatamak.Logger;
using AdemCatamak.Utilities;
using Autofac.Integration.WebApi;

namespace AdemCatamak.Api.Handlers
{
    public class GeneralExceptionHandler : ExceptionFilterAttribute, IAutofacExceptionFilter
    {
        private readonly ILogWrapper _logWrapper;

        public GeneralExceptionHandler(ILogWrapper logWrapper)
        {
            _logWrapper = logWrapper;
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            BaseResponse errorResponse = new ErrorResponse();

            FriendlyException friendlyException = context.Exception as FriendlyException;
            if (friendlyException == null)
            {
                _logWrapper.ErrorAsync(context.Exception);
                errorResponse.AddErrorMessage(context.Exception.Message);
            }
            else
            {
                if (friendlyException.InnerException != null)
                {
                    _logWrapper.ErrorAsync(friendlyException.FriendlyMessage, friendlyException.InnerException);
                }
                errorResponse.AddErrorMessage(friendlyException.FriendlyMessage);
            }


            context.Response = new HttpResponseMessage()
                               {
                                   Content = new StringContent(errorResponse.Serialize())
                               };
        }
    }
}