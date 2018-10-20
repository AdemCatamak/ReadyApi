using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ReadyApi.Core
{
    public static class CommonExtensions
    {
        public static void Log(this ILogger logger, LogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    logger.LogTrace(message);
                    break;
                case LogLevel.Debug:
                    logger.LogDebug(message);
                    break;
                case LogLevel.Information:
                    logger.LogInformation(message);
                    break;
                case LogLevel.Warning:
                    logger.LogWarning(message);
                    break;
                case LogLevel.Error:
                    logger.LogError(message);
                    break;
                case LogLevel.Critical:
                    logger.LogCritical(message);
                    break;
                default:
                    logger.LogError(message);
                    break;
            }
        }

        public static async Task<string> Stringfy(this HttpRequest request, bool handleError = true)
        {
            string result = $"{request.Scheme} {request.Host}{request.Path} {request.QueryString}";

            try
            {
                var buffer = new byte[Convert.ToInt32(request.ContentLength)];
                if (request.Body.CanRead)
                {
                    await request.Body.ReadAsync(buffer, 0, buffer.Length);
                }

                string bodyAsText = Encoding.UTF8.GetString(buffer);

                result = $"{result} {bodyAsText}";
            }
            catch (Exception e)
            {
                if (!handleError)
                {
                    throw;
                }

                result = $"{result} - Body could not be read [{e.Message}]";
            }

            return result;
        }
    }
}