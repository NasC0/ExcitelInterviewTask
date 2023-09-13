using System;
using System.Linq;

using Leads.Services.Contracts;

using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace Leads.IntegrationTests.Tests.Leads
{
    public class LeadsRegistrations
    {
        public static Action<IServiceCollection> RegisterMocks(Mock<ILeadsService> leadsServiceMock) =>
            services =>
            {
                // Remove the existing registration if any
                ServiceDescriptor descriptor =
                    services.SingleOrDefault(d => d.ServiceType == typeof(ILeadsService));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddSingleton(provider => leadsServiceMock.Object);
            };
    }
}