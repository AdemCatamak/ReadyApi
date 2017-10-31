using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Alternatives.Extensions;
using RapidLogger;
using ReadyApi.Model;
using ReadyApi.Model.CustomExceptions;

namespace ReadyApi.Handlers
{
    public class GeneralExceptionHandler : ExceptionFilterAttribute
    {
        private readonly LoggerMaestro _loggerMaestro;

        public GeneralExceptionHandler(LoggerMaestro loggerMaestro)
        {
            _loggerMaestro = loggerMaestro;
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            context.Response = new HttpResponseMessage();
            ErrorResponse errorResponse = new ErrorResponse();
            if (context.Exception is FriendlyException friendlyException)
            {
                _loggerMaestro.Info($"{friendlyException.FriendlyMessage} :{Environment.NewLine}" +
                                    $"{friendlyException.InnerException}");

                context.Response.StatusCode = HttpStatusCode.BadRequest;
                errorResponse.AddError(friendlyException.FriendlyMessage);
            }
            else if (context.Exception is BusinessException businessException)
            {
                _loggerMaestro.Warning($"{businessException.ErrorMessage} :{Environment.NewLine}" +
                                       $"{businessException.InnerException}");

                context.Response.StatusCode = HttpStatusCode.InternalServerError;
                errorResponse.AddError(businessException.ErrorMessage);
            }
            else
            {
                _loggerMaestro.Error(context.Exception.Message, context.Exception);

                context.Response.StatusCode = HttpStatusCode.InternalServerError;
                errorResponse.AddError("Unexpected Error");
            }

            context.Response.Content = new StringContent(errorResponse.Serialize());
        }

        private class ErrorResponse : BaseResponse
        {
        }
    }
}