using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;

using Leads.IntegrationTests.ApiClients;
using Leads.IntegrationTests.Common;
using Leads.IntegrationTests.Fixtures;
using Leads.Models;
using Leads.Services.Contracts;

using Moq;

using Xunit;

namespace Leads.IntegrationTests.Tests.SubAreas
{
    public class SubAreasTests : IClassFixture<LeadsWebApplicationFactory>
    {
        private const string TestValidPinCode = "123";
        
        private readonly LeadsWebApplicationFactory _webbAppFactory;
        private readonly ClientsFactory _clientsFactory;

        public SubAreasTests(LeadsWebApplicationFactory webAppFactory)
        {
            _clientsFactory = new ClientsFactory();
            _webbAppFactory = webAppFactory;
        }

        [Fact(DisplayName = "Getting all SubAreas should succeed")]
        public async Task GetAllSubAreas_ShouldSucceed()
        {
            // Arrange
            SubAreasApiClient client = _clientsFactory.GetSubAreasApiClient(_webbAppFactory.CreateClient());
            
            // Act
            CallWrapper<List<SubAreaViewModel>> result = await client.GetAsync<List<SubAreaViewModel>>();

            // Assert
            result.Result.Should().NotBeNullOrEmpty();
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact(DisplayName = "Getting all SubAreas should fail with InternalServerError when service throws exception")]
        public async Task GetAllSubAreas_OnServiceException_ShouldFailWithInternalServerError()
        {
            // Arrange
            Mock<ISubAreasService> subAreasServiceMock = new Mock<ISubAreasService>();
            HttpClient client =
                _webbAppFactory.SetupServiceMocksAndCreateClient(
                    SubAreasRegistrations.RegisterMocks(subAreasServiceMock));

            subAreasServiceMock
                .Setup(sas => sas.GetAll())
                .ThrowsAsync(new Exception());

            SubAreasApiClient subAreasApiClient = _clientsFactory.GetSubAreasApiClient(client);

            // Act
            CallWrapper<string> result = await subAreasApiClient.GetAsync();

            // Assert
            result.Response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            subAreasServiceMock.Verify(sas => sas.GetAll(), Times.Once);
        }

        [Fact(DisplayName = "Getting SubAreas by PinCode should return a collection of SubAreas")]
        public async Task GetByPinCode_ShouldReturnCollectionOfSubAreas()
        {
            // Arrange
            SubAreasApiClient client = _clientsFactory.GetSubAreasApiClient(_webbAppFactory.CreateClient());

            // Act
            CallWrapper<List<SubAreaViewModel>> result = await client.FilterByPinCodeAsync<List<SubAreaViewModel>>(TestValidPinCode);
            
            // Assert
            result.Result.Should().NotBeNullOrEmpty();
            result.Result.Should().AllSatisfy(savm => savm.PinCode.Should().Be(TestValidPinCode));
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact(DisplayName = "Getting SubAreas by PinCode should return an empty collection when no SubAreas are found")]
        public async Task GetByPinCode_NonExistingPinCode_ShouldReturnEmptyCollection()
        {
            // Arrange
            SubAreasApiClient client = _clientsFactory.GetSubAreasApiClient(_webbAppFactory.CreateClient());
            string pinCode = Guid.NewGuid().ToString();
            
            // Act
            CallWrapper<List<SubAreaViewModel>> result = await client.FilterByPinCodeAsync<List<SubAreaViewModel>>(pinCode);
            
            // Assert
            result.Result.Should().BeEmpty();
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory(DisplayName = "Getting SubAreas by PinCode should return NotFound when PinCode is null or empty")]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetByPinCode_NullOrEmptyPinCode_ShouldReturnNotFound(string pinCode)
        {
            // Arrange
            SubAreasApiClient client = _clientsFactory.GetSubAreasApiClient(_webbAppFactory.CreateClient());
            
            // Act
            CallWrapper<string> result = await client.FilterByPinCodeAsync(pinCode);
            
            // Assert
            result.Result.Should().BeEmpty();
            result.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "Getting SubAreas by PinCode should fail with InternalServerError when service throws exception")]
        public async Task GetByPinCode_OnServiceException_ShouldFailWithInternalServerError()
        {
            // Arrange
            Mock<ISubAreasService> subAreasServiceMock = new Mock<ISubAreasService>();
            HttpClient client =
                _webbAppFactory.SetupServiceMocksAndCreateClient(
                    SubAreasRegistrations.RegisterMocks(subAreasServiceMock));

            subAreasServiceMock
                .Setup(sas => sas.GetByPinCode(TestValidPinCode))
                .ThrowsAsync(new Exception());

            SubAreasApiClient subAreasApiClient = _clientsFactory.GetSubAreasApiClient(client);

            // Act
            CallWrapper<string> result = await subAreasApiClient.FilterByPinCodeAsync(TestValidPinCode);

            // Assert
            result.Response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            subAreasServiceMock.Verify(sas => sas.GetByPinCode(TestValidPinCode), Times.Once);
        }
    }
}