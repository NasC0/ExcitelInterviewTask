using System;
using System.Threading.Tasks;

using FluentAssertions;

using Leads.Models;
using Leads.Services.Contracts;
using Leads.WebApi.Controllers;
using Leads.WebApi.Models;

using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace Leads.WebApi.UnitTests
{
    [Trait("Category", "Controller")]
    [Trait("Category", "Unit")]
    public class LeadsControllerTests
    {
        private readonly Mock<ILeadsService> _leadsMock = new Mock<ILeadsService>();
        private readonly LeadsController _controller;

        public LeadsControllerTests() => _controller = new LeadsController(_leadsMock.Object);

        [Fact(DisplayName = "Calling the Get endpoint should return a LeadViewModel")]
        public async Task Get_ValidId_ShouldReturnLeadViewModel()
        {
            // Arrange
            LeadViewModel lead = new LeadViewModel
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                SubAreaId = 1
            };

            _leadsMock
                .Setup(ls => ls.Get(It.IsAny<Guid>()))
                .ReturnsAsync(lead)
                .Verifiable();

            // Act
            ActionResult<LeadViewModel> result = await _controller.Get(Guid.NewGuid());

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            result.Result.As<OkObjectResult>().Value.Should().Be(lead);
            _leadsMock.Verify(ls => ls.Get(It.IsAny<Guid>()), Times.Once);
        }

        [Fact(DisplayName = "Get endpoint should return NotFound when the id is not found")]
        public async Task Get_NonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            _leadsMock
                .Setup(ls => ls.Get(It.IsAny<Guid>()))
                .ReturnsAsync((LeadViewModel)null)
                .Verifiable();

            // Act
            ActionResult<LeadViewModel> result = await _controller.Get(Guid.NewGuid());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
            result.Result.As<NotFoundResult>().StatusCode.Should().Be(404);
            _leadsMock.Verify(ls => ls.Get(It.IsAny<Guid>()), Times.Once);
        }

        [Fact(DisplayName = "Get endpoint should throw up the stack when an exception occurs")]
        public async Task Get_OnException_ThrowsUpStack()
        {
            // Arrange
            _leadsMock
                .Setup(ls => ls.Get(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception())
                .Verifiable();

            // Act/Assert
            await _controller
                .Invoking(c => c.Get(It.IsAny<Guid>()))
                .Should()
                .ThrowAsync<Exception>();

            _leadsMock.Verify(ls => ls.Get(It.IsAny<Guid>()), Times.Once);
        }

        [Fact(DisplayName =
            "Calling the Post endpoint with a valid LeadsSaveViewModel should return a LeadsSaveSuccessModel")]
        public async Task Post_ValidLead_ShouldReturnLeadsSaveSuccessModel()
        {
            // Arrange
            LeadsSaveViewModel lead = new LeadsSaveViewModel
            {
                Name = "Test",
                SubAreaId = 1
            };

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
            LeadsSaveViewModel lead = new LeadsSaveViewModel
            {
                Name = "Test"
            };

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
            LeadsSaveViewModel lead = new LeadsSaveViewModel
            {
                Name = "Test",
                SubAreaId = 1
            };

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