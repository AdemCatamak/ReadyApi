using System;
using System.Collections.Generic;
using System.Text;
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
    }
}