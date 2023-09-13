using System;
using System.Linq;

using Leads.Services.Contracts;

using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace Leads.IntegrationTests.Tests.SubAreas
{
    public class SubAreasRegistrations
    {
        public static Action<IServiceCollection> RegisterSubAreasMocks(Mock<ISubAreasService> subAreasServiceMock) =>
            services =>
            {
                // Remove the existing registration if any
                ServiceDescriptor descriptor =
                    services.SingleOrDefault(d => d.ServiceType == typeof(ISubAreasService));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddSingleton(provider => subAreasServiceMock.Object);
            };
    }
}