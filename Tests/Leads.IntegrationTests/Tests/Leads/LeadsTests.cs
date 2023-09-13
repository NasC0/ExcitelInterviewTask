using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using Leads.IntegrationTests.ApiClients;
using Leads.IntegrationTests.Common;
using Leads.IntegrationTests.Fixtures;
using Leads.IntegrationTests.TestData;
using Leads.Models;
using Leads.Services.Contracts;
using Leads.WebApi.Models;

using Moq;

using Xunit;

namespace Leads.IntegrationTests.Tests.Leads
{
    public class LeadsTests : IClassFixture<LeadsWebApplicationFactory>
    {
        private const string TestValidPinCode = "123";
        private const int TestValidSubAreaId = 1;

        private readonly LeadsWebApplicationFactory _webbAppFactory;
        private readonly ClientsFactory _clientsFactory;
        private readonly Fixture _fixture;

        public LeadsTests(LeadsWebApplicationFactory webAppFactory)
        {
            _webbAppFactory = webAppFactory;
            _clientsFactory = new ClientsFactory();
            _fixture = new Fixture();
        }

        [Fact(DisplayName = "Posting a valid LeadSaveViewModel should return the id of the created lead")]
        public async Task Post_WithValidBody_ShouldReturnIdOfCreatedLead()
        {
            // Arrange
            LeadsApiClient client = _clientsFactory.GetLeadsApiClient(_webbAppFactory.CreateClient());
            LeadsSaveViewModel leadsModel = _fixture.Build<LeadsSaveViewModel>()
                .With(lsvm => lsvm.SubAreaId, TestValidSubAreaId)
                .With(lsvm => lsvm.PinCode, TestValidPinCode)
                .Create();

            // Act
            CallWrapper<LeadsSaveSuccessModel> result = await client.PostAsync<LeadsSaveSuccessModel>(leadsModel);

            // Assert
            result.Result.Id.Should().NotBeEmpty();
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact(DisplayName = "Posting a LeadSaveViewModel with missing SubAreaId should return BadRequest")]
        public async Task Post_MissingSubAreaId_ShouldReturnBadRequest()
        {
            // Arrange
            LeadsApiClient client = _clientsFactory.GetLeadsApiClient(_webbAppFactory.CreateClient());
            LeadsSaveViewModel leadsModel = _fixture.Build<LeadsSaveViewModel>()
                .Without(lsvm => lsvm.SubAreaId)
                .Create();

            // Act
            CallWrapper<ErrorViewModel> result = await client.PostAsync<ErrorViewModel>(leadsModel);

            // Assert
            result.Result.Should().NotBeNull()
                .And.Subject.As<ErrorViewModel>()
                .Message.Should().NotBeNullOrEmpty();

            result.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact(DisplayName = "Posting a LeadSaveViewModel with mismatched SubAreaId and PinCode should return BadRequest")]
        public async Task Post_MismatchedSubAreaAndPinCode_ShouldReturnBadRequest()
        {
            // Arrange
            LeadsApiClient client = _clientsFactory.GetLeadsApiClient(_webbAppFactory.CreateClient());
            LeadsSaveViewModel leadsModel = _fixture.Build<LeadsSaveViewModel>()
                .With(lsvm => lsvm.SubAreaId, TestValidSubAreaId)
                .Create();

            // Act
            CallWrapper<ErrorViewModel> result = await client.PostAsync<ErrorViewModel>(leadsModel);

            // Assert
            result.Result.Should().NotBeNull()
                .And.Subject.As<ErrorViewModel>()
                .Message.Should().NotBeNullOrEmpty();

            result.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory(DisplayName = "Posting an invalid LeadSaveViewModel should return BadRequest")]
        [ClassData(typeof(InvalidLeadSaveViewModelData))]
        public async Task Post_InvalidLeadSaveViewModel_ShouldReturnBadRequest(LeadsSaveViewModel saveModel)
        {
            // Arrange
            LeadsApiClient client = _clientsFactory.GetLeadsApiClient(_webbAppFactory.CreateClient());
            
            // Act
            CallWrapper<ErrorViewModel> result = await client.PostAsync<ErrorViewModel>(saveModel);
            
            // Assert
            result.Result.Should().NotBeNull();
            result.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact(DisplayName = "Posting a LeadSaveViewModel should fail with BadRequest when service throws exception")]
        public async Task Post_OnServiceException_ShouldFailWithBadRequest()
        {
            // Arrange
            string exceptionMessage = "Test Exception";
            LeadsSaveViewModel leadsModel = _fixture.Create<LeadsSaveViewModel>();
            Mock<ILeadsService> leadsServiceMock = new Mock<ILeadsService>();
            
            HttpClient baseClient =
                _webbAppFactory.SetupServiceMocksAndCreateClient(LeadsRegistrations.RegisterMocks(leadsServiceMock));
            LeadsApiClient client = _clientsFactory.GetLeadsApiClient(baseClient);

            leadsServiceMock
                .Setup(ls => ls.Save(It.IsAny<LeadSaveModel>()))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            CallWrapper<ErrorViewModel> result = await client.PostAsync<ErrorViewModel>(leadsModel);

            // Assert
            result.Result.Should().NotBeNull()
                .And.Subject.As<ErrorViewModel>()
                .Message.Should().Be(exceptionMessage);

            result.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            leadsServiceMock.Verify(ls => ls.Save(It.IsAny<LeadSaveModel>()), Times.Once);
        }
        
        [Fact(DisplayName = "Getting an existing Lead by valid id should return LeadViewModel")]
        public async Task Get_WithValidId_ShouldReturnLeadViewModel()
        {
            // Arrange
            LeadsSaveViewModel leadSaveModel = _fixture.Build<LeadsSaveViewModel>()
                .With(lsvm => lsvm.SubAreaId, TestValidSubAreaId)
                .With(lsvm => lsvm.PinCode, TestValidPinCode)
                .Create();

            LeadsApiClient client = _clientsFactory.GetLeadsApiClient(_webbAppFactory.CreateClient());
            CallWrapper<LeadsSaveSuccessModel> saveResult = await client.PostAsync<LeadsSaveSuccessModel>(leadSaveModel);
            Guid validId = saveResult.Result.Id;

            // Act
            CallWrapper<LeadViewModel> result = await client.GetByIdAsync<LeadViewModel>(validId.ToString());

            // Assert
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Result.Should().BeEquivalentTo(leadSaveModel);
        }

        [Fact(DisplayName = "Getting a non-existing Lead by id should return NotFound")]
        public async Task Get_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            LeadsApiClient client = _clientsFactory.GetLeadsApiClient(_webbAppFactory.CreateClient());
            Guid nonExistingId = Guid.NewGuid();
            
            // Act
            CallWrapper<string> result = await client.GetByIdAsync(nonExistingId.ToString());
            
            // Assert
            result.Result.Should().BeEmpty();
            result.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory(DisplayName = "Getting a Lead by null or empty id should return NotFound")]
        [InlineData(null)]
        [InlineData("")]
        public async Task Get_NullOrEmptyId_ShouldReturnNotFound(string id)
        {
            // Arrange
            LeadsApiClient client = _clientsFactory.GetLeadsApiClient(_webbAppFactory.CreateClient());
            
            // Act
            CallWrapper<string> result = await client.GetByIdAsync(id);
            
            // Assert
            result.Result.Should().BeEmpty();
            result.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "Getting a Lead by invalid id should return BadRequest")]
        public async Task Get_InvalidId_ShouldReturnNotFound()
        {
            // Arrange
            LeadsApiClient client = _clientsFactory.GetLeadsApiClient(_webbAppFactory.CreateClient());
            string invalidId = "InvalidId";
            
            // Act
            CallWrapper<string> result = await client.GetByIdAsync(invalidId);
            
            // Assert
            result.Result.Should().NotBeNullOrWhiteSpace();
            result.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact(DisplayName = "Getting a lead by id should fail with InternalServerError when service throws exception")]
        public async Task Get_OnServiceException_ShouldFailWithInternalServerError()
        {
            // Arrange
            string exceptionMessage = "Test Exception";
            Guid leadId = Guid.NewGuid();
            
            Mock<ILeadsService> leadsServiceMock = new Mock<ILeadsService>();
            
            HttpClient baseClient =
                _webbAppFactory.SetupServiceMocksAndCreateClient(LeadsRegistrations.RegisterMocks(leadsServiceMock));
            LeadsApiClient client = _clientsFactory.GetLeadsApiClient(baseClient);

            leadsServiceMock
                .Setup(ls => ls.Get(leadId))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            CallWrapper<string> result = await client.GetByIdAsync(leadId.ToString());

            // Assert
            result.Result.Should().NotBeNullOrWhiteSpace();

            result.Response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            leadsServiceMock.Verify(ls => ls.Get(leadId), Times.Once);
        }
    }
}