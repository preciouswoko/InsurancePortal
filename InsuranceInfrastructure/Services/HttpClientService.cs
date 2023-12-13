using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using InsuranceCore.Interfaces;
using System.Net.Http;

namespace InsuranceInfrastructure.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<T> GetAsync<T>(string uri, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient();

            AddHeaders(client, headers);

            var response = await client.GetAsync(uri, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }

            return default(T);
        }

        public async Task<T> PostAsync<T>(string uri, object data, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient();

            AddHeaders(client, headers);

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(uri, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseContent);
            }

            return default(T);
        }

        public async Task<T> PutAsync<T>(string uri, object data, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient();

            AddHeaders(client, headers);

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(uri, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseContent);
            }

            return default(T);
        }

        public async Task<bool> DeleteAsync(string uri, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient();

            AddHeaders(client, headers);

            var response = await client.DeleteAsync(uri, cancellationToken);

            return response.IsSuccessStatusCode;
        }

        private void AddHeaders(HttpClient client, Dictionary<string, string> headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
        }
    }
}
