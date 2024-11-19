using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.ModelBinding;
using static WebAPIServiceTemplate1.Configurations.HttpRequestResponse;

namespace WebAPIServiceTemplate1.Configurations
{
    public static class HttpRequestMessageExtensions
    {
        public static HttpResponseMessage CreateSuccessResponse<T>(this HttpRequestMessage httpRequestMessage, HttpStatusCode statusCode, string message, T value)
        {
            int code = Convert.ToInt32(statusCode);
            var responseMessage = new ServiceResponse<T>(code, message, value);
            string jsonResponse = JsonConvert.SerializeObject(responseMessage, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json"),
                ReasonPhrase = "Operación exitosa",
                StatusCode = HttpStatusCode.OK,
                RequestMessage = httpRequestMessage
            };
            return response;
        }

        public static HttpResponseMessage CreateSuccessResponse<T>(this HttpRequestMessage httpRequestMessage, T value)
        {
            int code = Convert.ToInt32(HttpStatusCode.OK);
            string message = "Operación exitosa";
            var responseMessage = new ServiceResponse<T>(code, message, value);
            string jsonResponse = JsonConvert.SerializeObject(responseMessage, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json"),
                ReasonPhrase = "Operación exitosa",
                StatusCode = HttpStatusCode.OK,
                RequestMessage = httpRequestMessage
            };
            return response;
        }

        public static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage httpRequestMessage, int code, string message)
        {
            var responseMessage = new ServiceErrorResponse(code, message);
            string jsonResponse = JsonConvert.SerializeObject(responseMessage, Formatting.Indented);

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json"),
                ReasonPhrase = "El servicio devolvio un error",
                StatusCode = HttpStatusCode.InternalServerError,
                RequestMessage = httpRequestMessage
            };

            return response;
        }

        public static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage httpRequestMessage, Exception ex)
        {
            var responseMessage = new ServiceErrorResponse(ex);
            int code = Convert.ToInt32(HttpStatusCode.InternalServerError);
            var response = new ServiceResponse<string>(code, "Ocurrio un error inesperadp", ex.Message);
            string jsonResponse = JsonConvert.SerializeObject(responseMessage, Formatting.Indented);

            var httpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json"),
                ReasonPhrase = "El servicio devolvio un error",
                StatusCode = HttpStatusCode.InternalServerError,
                RequestMessage = httpRequestMessage
            };

            return httpResponse;
        }

        public static HttpResponseMessage CreateErrorResponse<T>(this HttpRequestMessage httpRequestMessage, int code, string message, T value)
        {
            var response = new ServiceResponse<T>(code, "Ocurrio un error inesperadp", value);
            string jsonResponse = JsonConvert.SerializeObject(response, Formatting.Indented);

            var httpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json"),
                ReasonPhrase = "El servicio devolvio un error",
                StatusCode = HttpStatusCode.InternalServerError,
                RequestMessage = httpRequestMessage
            };

            return httpResponse;
        }

        public static HttpResponseMessage CreateErrorResponseBadRequest(this HttpRequestMessage httpRequestMessage, ModelStateDictionary modelState)
        {
            int code = Convert.ToInt32(HttpStatusCode.BadRequest);
            var response = new ServiceResponse<ModelStateDictionary>(code, "Debe enviar todos los campos requeridos", modelState);
            string jsonResponse = JsonConvert.SerializeObject(response, Formatting.Indented);

            var httpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json"),
                ReasonPhrase = "El servicio devolvio un error",
                StatusCode = HttpStatusCode.BadRequest,
                RequestMessage = httpRequestMessage
            };

            return httpResponse;
        }

    }

    public class HttpRequestResponse
    {
        public class ServiceResponse<T>
        {
            [JsonProperty(Order = 1, PropertyName = "codigo")]
            public int Code { get; set; }

            [JsonProperty(Order = 2, PropertyName = "descripcion")]
            public string Description { get; set; }

            [JsonProperty(Order = 3, PropertyName = "detalle")]
            public T Response { get; set; }

            public ServiceResponse(int code, string desc, T response)
            {
                Code = code;
                Description = desc;
                Response = response;
            }
            public ServiceResponse(T response)
            {
                Code = Convert.ToInt32(System.Net.HttpStatusCode.OK);
                Description = "Operación exitosa";
                Response = response;
            }
        }
        public class ServiceErrorResponse
        {
            [JsonProperty(Order = 1, PropertyName = "codigo")]
            public int Code { get; set; }

            [JsonProperty(Order = 2, PropertyName = "descripcion")]
            public string Description { get; set; }

            public ServiceErrorResponse(int code, string desc)
            {
                Code = code;
                Description = desc;
            }
            public ServiceErrorResponse(Exception ex)
            {
                var message = string.IsNullOrEmpty(ex.Message) ? ex.InnerException.Message : ex.Message;
                Code = Convert.ToInt32(System.Net.HttpStatusCode.InternalServerError);
                Description = message;
            }
        }
    }
}