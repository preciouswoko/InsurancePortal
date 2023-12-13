using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InsuranceCore.Interfaces
{
    public interface IHttpClientService
    {
        Task<T> GetAsync<T>(string uri, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);
        Task<T> PostAsync<T>(string uri, object data, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);
        Task<T> PutAsync<T>(string uri, object data, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string uri, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);
    }

}
