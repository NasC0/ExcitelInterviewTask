using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using Leads.Models;
using Leads.Services.Contracts;
using Leads.WebApi.Controllers;

using Moq;

using Xunit;

namespace Leads.WebApi.UnitTests.Tests
{
    [Trait("Category", "Controller")]
    [Trait("Category", "Unit")]
    public class SubAreasControllerTests
    {
        private readonly Mock<ISubAreasService> _subAreasMock = new Mock<ISubAreasService>();
        private readonly SubAreasController _controller;
        private readonly Fixture _fixutre;

        public SubAreasControllerTests()
        {
            _controller = new SubAreasController(_subAreasMock.Object);
            _fixutre = new Fixture();
        }

        [Fact(DisplayName = "Calling the Get endpoint should return a list of SubAreaViewModel")]
        public async Task Get_ReturnsSubAreasList()
        {
            // Arrange
            List<SubAreaViewModel> subAreas = _fixutre.CreateMany<SubAreaViewModel>(3).ToList();

            _subAreasMock
                .Setup(x => x.GetAll())
                .ReturnsAsync(subAreas)
                .Verifiable();

            // Act
            List<SubAreaViewModel> result = await _controller.Get();

            // Assert
            result.Should().BeSameAs(subAreas);
            _subAreasMock.Verify(sas => sas.GetAll(), Times.Once);
        }

        [Fact(DisplayName = "Get endpoint should throw up the stack when an exception occurs")]
        public async Task Get_OnException_ThrowsUpStack()
        {
            // Arrange
            _subAreasMock
                .Setup(x => x.GetAll())
                .ThrowsAsync(new Exception())
                .Verifiable();

            // Act/Assert
            await _controller
                .Invoking(c => c.Get())
                .Should()
                .ThrowAsync<Exception>();

            _subAreasMock.Verify(sas => sas.GetAll(), Times.Once);
        }

        [Fact(DisplayName =
            "Calling the GetByPinCode endpoint with a valid PinCode should return a list of SubAreaViewModel")]
        public async Task GetByPinCode_WithValidPinCode_ReturnsFilteredSubAreasList()
        {
            // Arrange
            List<SubAreaViewModel> subAreas = _fixutre.CreateMany<SubAreaViewModel>(3).ToList();
            string pinCode = "1234";

            _subAreasMock
                .Setup(x => x.GetByPinCode(pinCode))
                .ReturnsAsync(subAreas)
                .Verifiable();

            // Act
            List<SubAreaViewModel> result = await _controller.Get(pinCode);

            // Assert
            result.Should().BeSameAs(subAreas);
            _subAreasMock.Verify(sas => sas.GetByPinCode(pinCode), Times.Once);
        }

        [Theory(DisplayName =
            "Calling the GetByPinCode endpoint with a null or empty PinCode should still call the service")]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetByPinCode_WithNullPinCode_ReturnsFilteredSubAreasList(string invalidPinCode)
        {
            // Arrange
            List<SubAreaViewModel> subAreas = _fixutre.CreateMany<SubAreaViewModel>(3).ToList();

            _subAreasMock
                .Setup(sas => sas.GetByPinCode(invalidPinCode))
                .ReturnsAsync(subAreas)
                .Verifiable();

            // Act
            List<SubAreaViewModel> result = await _controller.Get(invalidPinCode);

            // Assert
            result.Should().BeSameAs(subAreas);
            _subAreasMock.Verify(sas => sas.GetByPinCode(invalidPinCode), Times.Once);
        }

        [Fact(DisplayName = "GetByPinCode endpoint should throw up the stack when an exception occurs")]
        public async Task GetByPinCode_OnException_ThrowsUpTheStack()
        {
            // Arrange
            _subAreasMock
                .Setup(x => x.GetByPinCode(It.IsAny<string>()))
                .ThrowsAsync(new Exception())
                .Verifiable();

            // Act/Assert
            await _controller
                .Invoking(c => c.Get(It.IsAny<string>()))
                .Should()
                .ThrowAsync<Exception>();

            _subAreasMock.Verify(sas => sas.GetByPinCode(It.IsAny<string>()), Times.Once);
        }
    }
}