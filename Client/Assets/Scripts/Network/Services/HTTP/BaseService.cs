using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Assets.Scripts.Network.Services.HTTP
{
    public class BaseService
    {
        protected HttpClient _client;

        public BaseService()
        {
            _client = RequestManagerHttp.Client;
        }

        protected T Get<T>(string route)
        {
            var response = _client.GetAsync(route).GetAwaiter().GetResult();

            var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            return JsonConvert.DeserializeObject<T>(json);
        }

        protected T Post<T>(string route, object content)
        {
            var jsonContent = JsonConvert.SerializeObject(content);
            var buffer = System.Text.Encoding.UTF8.GetBytes(jsonContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = _client.PostAsync(route, byteContent).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                // TODO: replace with generic logger.
                UnityEngine.Debug.Log($"Error executing post request for route:{route} and payload: {jsonContent}");
                return default(T);
            }

            var responseJson = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (responseJson != null)
            {
                return JsonConvert.DeserializeObject<T>(responseJson, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
            }
            else
            {
                return default(T);
            }
        }
    }
}
