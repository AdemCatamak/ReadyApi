using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Alternatives;
using Alternatives.CustomExceptions;
using Autofac.Integration.WebApi;
using RapidLogger;
using ReadyApi.Model.Responses;
using ReadyApi.Model.Responses.Imp;

namespace ReadyApi.Handlers
{
    public class GeneralExceptionHandler : ExceptionFilterAttribute, IAutofacExceptionFilter
    {
        private readonly LoggerMaestro _loggerMaestro;

        public GeneralExceptionHandler(LoggerMaestro loggerMaestro)
        {
            _loggerMaestro = loggerMaestro;
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            BaseResponse errorResponse = new ErrorResponse();

            FriendlyException friendlyException = context.Exception as FriendlyException;
            if (friendlyException != null)
            {
                if (friendlyException.InnerException != null)
                {
                    _loggerMaestro.ErrorAsync(friendlyException.FriendlyMessage, friendlyException.InnerException);
                }
                errorResponse.AddErrorMessage(friendlyException.FriendlyMessage);
                context.Response.StatusCode = HttpStatusCode.BadRequest;
                context.Response.ReasonPhrase = friendlyException.FriendlyMessage;
            }
            else
            {
                _loggerMaestro.ErrorAsync(context.Exception);
                errorResponse.AddErrorMessage("Unexpected Error");
                context.Response.StatusCode = HttpStatusCode.InternalServerError;
                context.Response.ReasonPhrase = "Unexpected Error";
            }


            context.Response = new HttpResponseMessage()
                               {
                                   Content = new StringContent(errorResponse.Serialize())
                               };
        }
    }
}