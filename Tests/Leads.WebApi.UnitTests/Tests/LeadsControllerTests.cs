using System;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using Leads.Models;
using Leads.Services.Contracts;
using Leads.WebApi.Controllers;
using Leads.WebApi.Models;

using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace Leads.WebApi.UnitTests.Tests
{
    [Trait("Category", "Controller")]
    [Trait("Category", "Unit")]
    public class LeadsControllerTests
    {
        private readonly Mock<ILeadsService> _leadsMock = new Mock<ILeadsService>();
        private readonly LeadsController _controller;
        private readonly Fixture _fixture;

        public LeadsControllerTests()
        {
            _controller = new LeadsController(_leadsMock.Object);
            _fixture = new Fixture();
        }

        [Fact(DisplayName = "Calling the Get endpoint should return a LeadViewModel")]
        public async Task Get_ValidId_ShouldReturnLeadViewModel()
        {
            // Arrange
            Guid leadId = Guid.NewGuid();
            LeadViewModel lead = _fixture.Build<LeadViewModel>()
                .With(l => l.Id, leadId)
                .Create();

            _leadsMock
                .Setup(ls => ls.Get(leadId))
                .ReturnsAsync(lead)
                .Verifiable();

            // Act
            ActionResult<LeadViewModel> result = await _controller.Get(leadId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            result.Result.As<OkObjectResult>().Value.Should().Be(lead);
            _leadsMock.Verify(ls => ls.Get(leadId), Times.Once);
        }

        [Fact(DisplayName = "Get endpoint should return NotFound when the id is not found")]
        public async Task Get_NonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            Guid leadId = Guid.NewGuid();
            _leadsMock
                .Setup(ls => ls.Get(leadId))
                .ReturnsAsync((LeadViewModel)null)
                .Verifiable();

            // Act
            ActionResult<LeadViewModel> result = await _controller.Get(leadId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
            result.Result.As<NotFoundResult>().StatusCode.Should().Be(404);
            _leadsMock.Verify(ls => ls.Get(leadId), Times.Once);
        }

        [Fact(DisplayName = "Get endpoint should throw up the stack when an exception occurs")]
        public async Task Get_OnException_ThrowsUpStack()
        {
            // Arrange
            Guid leadId = Guid.NewGuid();

            _leadsMock
                .Setup(ls => ls.Get(leadId))
                .ThrowsAsync(new Exception())
                .Verifiable();

            // Act/Assert
            await _controller
                .Invoking(c => c.Get(leadId))
                .Should()
                .ThrowAsync<Exception>();

            _leadsMock.Verify(ls => ls.Get(leadId), Times.Once);
        }

        [Fact(DisplayName =
            "Calling the Post endpoint with a valid LeadsSaveViewModel should return a LeadsSaveSuccessModel")]
        public async Task Post_ValidLead_ShouldReturnLeadsSaveSuccessModel()
        {
            // Arrange
            LeadsSaveViewModel lead = _fixture.Create<LeadsSaveViewModel>();

            Guid savedId = Guid.NewGuid();
            _leadsMock
                .Setup(ls => ls.Save(It.IsAny<LeadSaveModel>()))
                .ReturnsAsync(savedId)
                .Verifiable();

            // Act
            ActionResult<LeadsSaveSuccessModel> result = await _controller.Post(lead);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            result.Result.As<OkObjectResult>().Value.Should().BeOfType<LeadsSaveSuccessModel>();
            result.Result.As<OkObjectResult>().Value.As<LeadsSaveSuccessModel>().Id.Should().Be(savedId);
            _leadsMock.Verify(ls => ls.Save(It.IsAny<LeadSaveModel>()), Times.Once);
        }

        [Fact(DisplayName = "Calling the Post endpoint with a null SubAreaId should return a BadRequest")]
        public async Task Post_NullSubAreaId_ShouldReturnBadRequest()
        {
            // Arrange
            LeadsSaveViewModel lead = _fixture.Build<LeadsSaveViewModel>()
                .Without(l => l.SubAreaId)
                .Create();

            _leadsMock
                .Setup(ls => ls.Save(It.IsAny<LeadSaveModel>()))
                .Verifiable();

            // Act
            ActionResult<LeadsSaveSuccessModel> result = await _controller.Post(lead);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            result
                .Result.As<BadRequestObjectResult>()
                .Value.Should().BeOfType<ErrorViewModel>();

            _leadsMock.Verify(ls => ls.Save(It.IsAny<LeadSaveModel>()), Times.Never);
        }

        [Fact(DisplayName = "Post endpoint should return a BadRequest when an exception occurs")]
        public async Task Post_OnException_ShouldReturnBadRequest()
        {
            // Arrange
            LeadsSaveViewModel lead = _fixture.Create<LeadsSaveViewModel>();

            _leadsMock
                .Setup(ls => ls.Save(It.IsAny<LeadSaveModel>()))
                .ThrowsAsync(new Exception())
                .Verifiable();

            // Act
            ActionResult<LeadsSaveSuccessModel> result = await _controller.Post(lead);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            result
                .Result.As<BadRequestObjectResult>()
                .Value.Should().BeOfType<ErrorViewModel>();

            _leadsMock.Verify(ls => ls.Save(It.IsAny<LeadSaveModel>()), Times.Once);
        }
    }
}