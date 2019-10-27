using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SenderApplication
{
    public sealed class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate Next;

        readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            Next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await Next(context);
            }
            catch (System.Exception ex)
            {
                this._logger.Log(LogLevel.Error, ex.Message);

                await HandledExceptionAsync(context, ex);
            }
        }

        private static Task HandledExceptionAsync(HttpContext context, System.Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            var result = JsonConvert.SerializeObject(new { messages = new string[] { exception.Message } });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }

        public class ExceptionFilter : IExceptionFilter
        {
            public void OnException(ExceptionContext context)
            {
                int status = ExtractHttpStatus(context);

                context.Result = new ObjectResult((dynamic)context.Exception)
                {
                    StatusCode = status
                };
            }

            private static int ExtractHttpStatus(ExceptionContext context)
            {
                var status = 500;

                if ( context.Exception is ApiException)
                    status = 400;

                return status;
            }
        }

        public class ApiException : Exception
        {
            public ApiException(string messsage) : base(messsage)
            {

            }
        }
    }
}