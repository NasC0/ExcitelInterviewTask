using System.Net.Http;
using System.Threading.Tasks;

using Leads.IntegrationTests.Common;

namespace Leads.IntegrationTests.ApiClients
{
    public class SubAreasApiClient
    {
        private const string GetAllUrl = "api/SubAreas";
        private const string FilterByPinCodeUrl = "api/SubAreas/Filter/PinCode/{0}";
        
        private readonly HttpClient _client;

        public SubAreasApiClient(HttpClient baseClient)
        {
            _client = baseClient;
        }
        
        public async Task<CallWrapper<TResult>> GetAsync<TResult>()
        {
            HttpResponseMessage response = await _client.GetAsync(GetAllUrl);
            TResult result = await response.Content.ReadAsAsync<TResult>();
            
            return new CallWrapper<TResult>
            {
                Request = response.RequestMessage,
                Response = response,
                Result = result
            };
        }
        
        public async Task<CallWrapper<string>> GetAsync()
        {
            HttpResponseMessage response = await _client.GetAsync(GetAllUrl);
            string result = await response.Content.ReadAsStringAsync();
            
            return new CallWrapper<string>
            {
                Request = response.RequestMessage,
                Response = response,
                Result = result
            };
        }

        public async Task<CallWrapper<TResult>> FilterByPinCodeAsync<TResult>(string pinCode)
        {
            string url = string.Format(FilterByPinCodeUrl, pinCode);
            HttpResponseMessage response = await _client.GetAsync(url);
            TResult result = await response.Content.ReadAsAsync<TResult>();
            
            return new CallWrapper<TResult>
            {
                Request = response.RequestMessage,
                Response = response,
                Result = result
            };
        }

        public async Task<CallWrapper<string>> FilterByPinCodeAsync(string pinCode)
        {
            string url = string.Format(FilterByPinCodeUrl, pinCode);
            HttpResponseMessage response = await _client.GetAsync(url);
            string result = await response.Content.ReadAsStringAsync();
            
            return new CallWrapper<string>
            {
                Request = response.RequestMessage,
                Response = response,
                Result = result
            };
        }
    }
}