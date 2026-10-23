using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace MeloongCore
{
    [NullableContext(2)]
    [Nullable(0)]
    public class HttpRequestCodeException : HttpRequestException
    {
        [NullableContext(1)]
        public HttpRequestCodeException(string message, HttpStatusCode statusCode, [Nullable(2)] string response)
            : base(message)
        {
        }

        public HttpStatusCode StatusCode { get; } = statusCode;

        public string Response { get; } = response;
    }
}
