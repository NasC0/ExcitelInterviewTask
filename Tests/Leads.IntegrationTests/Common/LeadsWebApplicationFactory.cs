using System;
using System.Net.Http;

using Leads.WebApi;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Leads.IntegrationTests.Common
{
    public class LeadsWebApplicationFactory : WebApplicationFactory<Startup>
    {
        public HttpClient SetupServiceMocksAndCreateClient(Action<IServiceCollection> setupServiceMocks)
        {
            HttpClient client = WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(setupServiceMocks);
                })
                .CreateClient();

            return client;
        }
    }
}