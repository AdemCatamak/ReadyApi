using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ReadyApi.AspNetCore.Middleware
{
    internal static class CommonExtensions
    {
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