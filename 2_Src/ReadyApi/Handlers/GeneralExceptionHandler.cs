using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Alternatives.CustomExceptions;
using Alternatives.Extensions;
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

            if (context.Exception is FriendlyException friendlyException)
            {
                _loggerMaestro.Info($"{friendlyException.FriendlyMessage} :{Environment.NewLine}" +
                                    $"{friendlyException.InnerException}");

                errorResponse.AddErrorMessage(friendlyException.FriendlyMessage);
                context.Response.StatusCode = HttpStatusCode.BadRequest;
                context.Response.ReasonPhrase = friendlyException.FriendlyMessage;
            }
            else if (context.Exception is BusinessException businessException)
            {
                _loggerMaestro.Warning($"{businessException.ErrorMessage} :{Environment.NewLine}" +
                                       $"{businessException.InnerException}");

                errorResponse.AddErrorMessage(businessException.ErrorMessage);
                context.Response.StatusCode = HttpStatusCode.InternalServerError;
                context.Response.ReasonPhrase = businessException.ErrorMessage;
            }
            else
            {
                _loggerMaestro.Error(context.Exception);
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