using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MeloongCore.Extensions
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class WebExtensions
    {
        public static HttpRequestMessage Clone(this HttpRequestMessage request)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Content = request.Content.Clone(),
                Version = request.Version
            };
            foreach (KeyValuePair<string, object> keyValuePair in request.Properties)
            {
                httpRequestMessage.Properties.Add(keyValuePair);
            }
            foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair2 in request.Headers)
            {
                httpRequestMessage.Headers.TryAddWithoutValidation(keyValuePair2.Key, keyValuePair2.Value);
            }
            return httpRequestMessage;
        }

        public static async Task<HttpRequestMessage> CloneAsync(this HttpRequestMessage request)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(request.Method, request.RequestUri);
            HttpRequestMessage httpRequestMessage2 = httpRequestMessage;
            HttpContent httpContent = await request.Content.CloneAsync().NoCapture<HttpContent>();
            httpRequestMessage2.Content = httpContent;
            httpRequestMessage.Version = request.Version;
            HttpRequestMessage httpRequestMessage3 = httpRequestMessage;
            httpRequestMessage2 = null;
            httpRequestMessage = null;
            foreach (KeyValuePair<string, object> keyValuePair in request.Properties)
            {
                httpRequestMessage3.Properties.Add(keyValuePair);
            }
            foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair2 in request.Headers)
            {
                httpRequestMessage3.Headers.TryAddWithoutValidation(keyValuePair2.Key, keyValuePair2.Value);
            }
            return httpRequestMessage3;
        }

        [NullableContext(2)]
        public static HttpContent Clone(this HttpContent content)
        {
            if (content == null)
            {
                return null;
            }
            MemoryStream memoryStream = new MemoryStream();
            content.CopyToAsync(memoryStream).Wait();
            memoryStream.Position = 0L;
            StreamContent streamContent = new StreamContent(memoryStream);
            foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in content.Headers)
            {
                streamContent.Headers.Add(keyValuePair.Key, keyValuePair.Value);
            }
            return streamContent;
        }

        [NullableContext(2)]
        [return: Nullable(new byte[] { 1, 2 })]
        public static async Task<HttpContent> CloneAsync(this HttpContent content)
        {
            HttpContent httpContent;
            if (content == null)
            {
                httpContent = null;
            }
            else
            {
                MemoryStream ms = new MemoryStream();
                await content.CopyToAsync(ms).NoCapture();
                ms.Position = 0L;
                StreamContent streamContent = new StreamContent(ms);
                foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in content.Headers)
                {
                    streamContent.Headers.Add(keyValuePair.Key, keyValuePair.Value);
                }
                httpContent = streamContent;
            }
            return httpContent;
        }
    }
}
