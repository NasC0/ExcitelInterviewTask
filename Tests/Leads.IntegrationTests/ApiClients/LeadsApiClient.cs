using System.Net.Http;
using System.Threading.Tasks;

using Leads.IntegrationTests.Common;

namespace Leads.IntegrationTests.ApiClients
{
    public class LeadsApiClient
    {
        private const string GetByIdUrl = "api/Leads/{0}";
        private const string PostUrl = "api/Leads";
        
        private readonly HttpClient _client;

        public LeadsApiClient(HttpClient baseClient)
        {
            _client = baseClient;
        }

        public async Task<CallWrapper<TResult>> GetByIdAsync<TResult>(string id)
        {
            string url = string.Format(GetByIdUrl, id);
            HttpResponseMessage response = await _client.GetAsync(url);
            TResult result = await response.Content.ReadAsAsync<TResult>();
            
            return new CallWrapper<TResult>
            {
                Request = response.RequestMessage,
                Response = response,
                Result = result
            };
        }

        public async Task<CallWrapper<string>> GetByIdAsync(string id)
        {
            string url = string.Format(GetByIdUrl, id);
            HttpResponseMessage response = await _client.GetAsync(url);
            string result = await response.Content.ReadAsStringAsync();
            
            return new CallWrapper<string>
            {
                Request = response.RequestMessage,
                Response = response,
                Result = result
            };
        }
        
        public async Task<CallWrapper<TResult>> PostAsync<TResult>(object model)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync(PostUrl, model);
            TResult result = await response.Content.ReadAsAsync<TResult>();
            
            return new CallWrapper<TResult>
            {
                Request = response.RequestMessage,
                Response = response,
                Result = result
            };
        }
    }
}