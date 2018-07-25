using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Alternatives.CustomExceptions;
using Newtonsoft.Json;
using RapidLogger;
using ReadyApi.Common.Model;

namespace ReadyApi.Handlers
{
    public class GeneralExceptionHandler : ExceptionFilterAttribute
    {
        private readonly LoggerMaestro _loggerMaestro;
        private BasicErrorResponse _basicErrorResponse;

        public GeneralExceptionHandler(LoggerMaestro loggerMaestro, BasicErrorResponse basicErrorResponse = null)
        {
            _loggerMaestro = loggerMaestro;
            _basicErrorResponse = basicErrorResponse;
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            _basicErrorResponse = _basicErrorResponse ?? new BasicErrorResponse();
            HttpStatusCode resultHttpStatusCode;
            if (context.Exception is CustomApiException customApiException)
            {
                _basicErrorResponse.AddErrorMessage(customApiException.FriendlyMessage);
                resultHttpStatusCode = customApiException.ReturnHttpStatusCode;
            }
            else
            {
                _basicErrorResponse.AddErrorMessage("Unexpected error occured");
                resultHttpStatusCode = HttpStatusCode.InternalServerError;
            }

            context.Response = new HttpResponseMessage()
            {
                StatusCode = resultHttpStatusCode,
                Content = new StringContent(JsonConvert.SerializeObject(_basicErrorResponse))
            };

            DoLogging(context.Exception, resultHttpStatusCode);
        }

        private void DoLogging(Exception contextException, HttpStatusCode resultHttpStatusCode)
        {
            if ((int)resultHttpStatusCode < 400)
            {
                _loggerMaestro.Info(contextException.Message + Environment.NewLine + contextException);
            }
            else if ((int)resultHttpStatusCode < 500)
            {
                _loggerMaestro.Warning(contextException.Message + Environment.NewLine + contextException);
            }
            else
            {
                _loggerMaestro.Error(contextException.Message, contextException);
            }
        }
    }
}