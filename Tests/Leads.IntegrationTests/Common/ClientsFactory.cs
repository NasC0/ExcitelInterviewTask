using System.Net.Http;

using Leads.IntegrationTests.ApiClients;

namespace Leads.IntegrationTests.Common
{
    public class ClientsFactory
    {
        public SubAreasApiClient GetSubAreasApiClient(HttpClient baseClient) => new SubAreasApiClient(baseClient);
        
        public LeadsApiClient GetLeadsApiClient(HttpClient baseClient) => new LeadsApiClient(baseClient);
    }
}