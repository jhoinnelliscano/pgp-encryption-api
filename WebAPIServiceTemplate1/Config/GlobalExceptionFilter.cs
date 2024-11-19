using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http.Filters;
using WebAPIServiceTemplate1.Logs;
using static WebAPIServiceTemplate1.Configurations.HttpRequestResponse;

namespace WebAPIServiceTemplate1.Configurations
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            string exceptionMessage = string.Empty;
            if (actionExecutedContext.Exception.InnerException == null)
            {
                exceptionMessage = actionExecutedContext.Exception.Message;
            }
            else
            {
                exceptionMessage = actionExecutedContext.Exception.InnerException.Message;
            }

            RequestException exception = new RequestException(actionExecutedContext);
            var parameters = exception.ActionArguments;
            exception.ActionArguments = "*[content]*";
            var jsonRequest = JsonConvert.SerializeObject(exception, Formatting.Indented);
            jsonRequest = jsonRequest.Replace("*[content]*", parameters);

            LogManager.RegisterLog(exceptionMessage, jsonRequest, exception.Controller, exception.Action);

            int code = Convert.ToInt32(System.Net.HttpStatusCode.InternalServerError);
            string message = "Ocurrio un error interno. Por favor consulte al administrador.";
            var serviceError = new ServiceErrorResponse(code, message);
            string jsonResponse = JsonConvert.SerializeObject(serviceError, Formatting.Indented);

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json"),
                ReasonPhrase = "El servicio devolvio un error"
            };

            actionExecutedContext.Response = response;
        }

    }

    public class RequestException
    {
        public string RequestUri { get; set; }
        public string Method { get; set; }
        public string Headers { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ActionArguments { get; set; }


        public RequestException(HttpActionExecutedContext actionExecutedContext)
        {
            RequestUri = actionExecutedContext.Request.RequestUri.ToString();
            Method = actionExecutedContext.Request.Method.ToString();
            Headers = actionExecutedContext.Request.Headers.ToString();
            Controller = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + "." + actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            //Action = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            ActionArguments = JsonConvert.SerializeObject(actionExecutedContext.ActionContext.ActionArguments, Formatting.Indented).ToString();

            if (actionExecutedContext.Exception != null && actionExecutedContext.Exception.StackTrace != null)
            {
                var stackTraceArray = actionExecutedContext.Exception.StackTrace.Split(new string[] { "---" }, StringSplitOptions.None);
                if (stackTraceArray.Any())
                {
                    Action = stackTraceArray[0];
                }
            }

        }
    }
}

