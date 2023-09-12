using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using Leads.DbAdapter;
using Leads.Models;

using Moq;

using Xunit;

namespace Leads.Services.UnitTests.Tests
{
    [Trait("Category", "Service")]
    [Trait("Category", "Unit")]
    public class SubAreasServiceTests
    {
        private readonly Mock<ISubAreasDb> _subAreasDbModk = new Mock<ISubAreasDb>();
        private readonly SubAreasService _subAreasService;
        private readonly Fixture _fixture;
        
        public SubAreasServiceTests()
        {
            _fixture = new Fixture();
            _subAreasService = new SubAreasService(_subAreasDbModk.Object);
        }

        [Fact(DisplayName = "GetAll returns collection of SubAreaViewModels")]
        public async Task GetAll_ReturnsCollectionOfSubAreaViewModels()
        {
            // Arrange
            List<SubAreaViewModel> expected = _fixture.CreateMany<SubAreaViewModel>(3).ToList();

            _subAreasDbModk
                .Setup(sad => sad.GetAll())
                .ReturnsAsync(expected);
            
            // Act
            List<SubAreaViewModel> result = await _subAreasService.GetAll();
            
            // Assert
            result.Should().Equal(expected);
            _subAreasDbModk.Verify(sad => sad.GetAll(), Times.Once);
        }

        [Fact(DisplayName = "GetAll throws exception up stack when SubAreasDb throws exception")]
        public async Task GetAll_OnSubAreasDbException_ThrowsUpStack()
        {
            // Arrange
            _subAreasDbModk
                .Setup(sad => sad.GetAll())
                .ThrowsAsync(new Exception());
            
            // Act/Assert
            await _subAreasService
                .Invoking(sas => sas.GetAll())
                .Should()
                .ThrowAsync<Exception>();
            
            _subAreasDbModk.Verify(sad => sad.GetAll(), Times.Once);
        }
        
        [Fact(DisplayName = "GetByPinCode returns collection of SubAreaViewModels")]
        public async Task GetByPinCode_ReturnsCollectionOfSubAreaViewModels()
        {
            // Arrange
            string pinCode = "12345";
            List<SubAreaViewModel> expected = _fixture.Build<SubAreaViewModel>()
                .With(savm => savm.PinCode, pinCode)
                .CreateMany(3)
                .ToList();

            _subAreasDbModk
                .Setup(sad => sad.GetByPinCode(pinCode))
                .ReturnsAsync(expected);
            
            // Act
            List<SubAreaViewModel> result = await _subAreasService.GetByPinCode(pinCode);
            
            // Assert
            result.Should().Equal(expected);
            _subAreasDbModk.Verify(sad => sad.GetByPinCode(pinCode), Times.Once);
        }

        [Theory(DisplayName = "GetByPinCode throws ArgumentException when PinCode is null or empty")]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetByPinCode_NullOrEmptyPinCode_ThrowsArgumentException(string pinCode)
        {
            // Act/Assert
            await _subAreasService
                .Invoking(sas => sas.GetByPinCode(pinCode))
                .Should()
                .ThrowAsync<ArgumentException>();
            
            _subAreasDbModk.Verify(sad => sad.GetByPinCode(pinCode), Times.Never);
        }
        
        [Fact(DisplayName = "GetByPinCode throws exception up stack when SubAreasDb throws exception")]
        public async Task GetByPinCode_OnSubAreasDbException_ThrowsUpStack()
        {
            // Arrange
            string pinCode = "12345";
            _subAreasDbModk
                .Setup(sad => sad.GetByPinCode(pinCode))
                .ThrowsAsync(new Exception());
            
            // Act/Assert
            await _subAreasService
                .Invoking(sas => sas.GetByPinCode(pinCode))
                .Should()
                .ThrowAsync<Exception>();
            
            _subAreasDbModk.Verify(sad => sad.GetByPinCode(pinCode), Times.Once);
        }
    }
}