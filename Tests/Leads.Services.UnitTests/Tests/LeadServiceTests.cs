using System;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using Leads.DbAdapter;
using Leads.Models;
using Leads.Services.UnitTests.TestData;

using Moq;

using Xunit;

namespace Leads.Services.UnitTests.Tests
{
    [Trait("Category", "Service")]
    [Trait("Category", "Unit")]
    public class LeadsServiceTests
    {
        private readonly Mock<ILeadsDb> _leadsDbMock = new Mock<ILeadsDb>();
        private readonly Mock<ISubAreasDb> _subAreasDbMock = new Mock<ISubAreasDb>();
        private readonly LeadsService _leadsService;
        private readonly Fixture _fixture;

        public LeadsServiceTests()
        {
            _leadsService = new LeadsService(_leadsDbMock.Object, _subAreasDbMock.Object);
            _fixture = new Fixture();
        }

        [Fact(DisplayName = "Saving a valid LeadSaveModel should succeed")]
        public async Task SaveLeadSaveModel_ValidModel_ShouldSucceed()
        {
            // Arrange
            LeadSaveModel leadSaveModel = _fixture.Create<LeadSaveModel>();

            Guid savedId = Guid.NewGuid();

            _leadsDbMock
                .Setup(ld => ld.Save(leadSaveModel))
                .ReturnsAsync(savedId);

            _subAreasDbMock.Setup(sad => sad.GetById(leadSaveModel.SubAreaId))
                .ReturnsAsync(
                    _fixture.Build<SubAreaViewModel>()
                        .With(savm => savm.PinCode, leadSaveModel.PinCode)
                        .Create());

            // Act
            Guid result = await _leadsService.Save(leadSaveModel);

            // Assert
            result.Should().Be(savedId);
            _leadsDbMock.Verify(ld => ld.Save(leadSaveModel), Times.Once);
            _subAreasDbMock.Verify(sad => sad.GetById(leadSaveModel.SubAreaId), Times.Once);
        }

        [Fact(DisplayName = "Saving a LeadSaveModel trims whitespace before saving")]
        public async Task SaveLeadSaveModel_TrimsWhiteSpace_BeforeSaving()
        {
            // Arrange
            LeadSaveModel leadSaveModel = _fixture.Build<LeadSaveModel>()
                .With(lsm => lsm.Name, "  Test  ")
                .With(lsm => lsm.PinCode, "  12345  ")
                .With(lsm => lsm.Address, "  Test Address  ")
                .Create();

            Guid savedId = Guid.NewGuid();

            _leadsDbMock
                .Setup(ld => ld.Save(leadSaveModel))
                .ReturnsAsync(savedId);

            _subAreasDbMock.Setup(sad => sad.GetById(leadSaveModel.SubAreaId))
                .ReturnsAsync(
                    _fixture.Build<SubAreaViewModel>()
                        .With(savm => savm.PinCode, "12345")
                        .Create());

            // Act
            Guid result = await _leadsService.Save(leadSaveModel);

            // Assert
            leadSaveModel.PinCode.Should().Be("12345");
            leadSaveModel.Name.Should().Be("Test");
            leadSaveModel.Address.Should().Be("Test Address");

            result.Should().Be(savedId);
            _leadsDbMock
                .Verify(ld => ld.Save(leadSaveModel), Times.Once);
            _subAreasDbMock.Verify(sad => sad.GetById(leadSaveModel.SubAreaId), Times.Once);
        }

        [Fact(DisplayName = "Saving a LeadSaveModel validates mismatched Lead PinCode and SubArea PinCode and throws")]
        public async Task SaveLeadSaveModel_ValidatingMismatchedLeadPinAndSubAreaPin_ShouldThrow()
        {
            // Arrange
            LeadSaveModel leadSaveModel = _fixture.Create<LeadSaveModel>();

            _subAreasDbMock.Setup(sad => sad.GetById(leadSaveModel.SubAreaId))
                .ReturnsAsync(_fixture.Create<SubAreaViewModel>());

            // Act/Assert
            await _leadsService
                .Invoking(ls => ls.Save(leadSaveModel))
                .Should()
                .ThrowAsync<ArgumentException>();

            _leadsDbMock.Verify(ld => ld.Save(leadSaveModel), Times.Never);
            _subAreasDbMock.Verify(sad => sad.GetById(leadSaveModel.SubAreaId), Times.Once);
        }

        [Theory(DisplayName = "Saving a LeadSaveModel validates data and throws when invalid")]
        [ClassData(typeof(InvalidLeadSaveModelData))]
        public async Task SaveLeadSaveModel_InvalidModel_ShouldThrow(LeadSaveModel leadSaveModel)
        {
            // Act/Assert
            await _leadsService
                .Invoking(ls => ls.Save(leadSaveModel))
                .Should()
                .ThrowAsync<ArgumentException>();

            _leadsDbMock.Verify(ld => ld.Save(It.IsAny<LeadSaveModel>()), Times.Never);
            _subAreasDbMock.Verify(sad => sad.GetById(It.IsAny<int>()), Times.Never);
        }

        [Fact(DisplayName = "Saving a LeadSaveModel throws up stack when SubAreasDb throws")]
        public async Task SaveLeadSaveModel_OnSubAreasDbException_ThrowsUpStack()
        {
            // Arrange
            _subAreasDbMock
                .Setup(sad => sad.GetById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            LeadSaveModel leadSaveModel = _fixture.Create<LeadSaveModel>();

            // Act/Assert
            await _leadsService
                .Invoking(ls => ls.Save(leadSaveModel))
                .Should()
                .ThrowAsync<Exception>();

            _subAreasDbMock.Verify(sad => sad.GetById(leadSaveModel.SubAreaId), Times.Once);
            _leadsDbMock.Verify(sad => sad.Save(It.IsAny<LeadSaveModel>()), Times.Never);
        }

        [Fact(DisplayName = "Saving a LeadSaveModel throws up stack when LeadDb throws")]
        public async Task SaveLeadSaveModel_OnLeadDbException_ThrowsUpStack()
        {
            // Arrange
            LeadSaveModel leadSaveModel = _fixture.Create<LeadSaveModel>();

            _leadsDbMock
                .Setup(ld => ld.Save(leadSaveModel))
                .ThrowsAsync(new Exception());

            _subAreasDbMock.Setup(sad => sad.GetById(leadSaveModel.SubAreaId))
                .ReturnsAsync(
                    _fixture.Build<SubAreaViewModel>()
                        .With(savm => savm.PinCode, leadSaveModel.PinCode)
                        .Create());

            // Act/Assert
            await _leadsService
                .Invoking(ls => ls.Save(leadSaveModel))
                .Should()
                .ThrowAsync<Exception>();

            _subAreasDbMock.Verify(sad => sad.GetById(leadSaveModel.SubAreaId), Times.Once);
            _leadsDbMock.Verify(sad => sad.Save(leadSaveModel), Times.Once);
        }

        [Fact(DisplayName = "Getting a LeadViewModel with a valid Id should return a valid model")]
        public async Task GetLeadViewModel_ValidId_ReturnsValidModel()
        {
            // Arrange
            LeadViewModel leadViewModel = _fixture.Build<LeadViewModel>()
                .Without(lvm => lvm.SubArea)
                .Create();

            Guid leadId = Guid.NewGuid();

            SubAreaViewModel subAreaModel = _fixture.Create<SubAreaViewModel>();

            _leadsDbMock
                .Setup(ld => ld.GetById(leadId))
                .ReturnsAsync(leadViewModel);

            _subAreasDbMock
                .Setup(sad => sad.GetById(leadViewModel.SubAreaId))
                .ReturnsAsync(subAreaModel);

            // Act
            LeadViewModel result = await _leadsService.Get(leadId);

            // Assert
            result.Should().BeEquivalentTo(leadViewModel);
            _leadsDbMock.Verify(ld => ld.GetById(leadId), Times.Once);
            _subAreasDbMock.Verify(sad => sad.GetById(leadViewModel.SubAreaId), Times.Once);
        }

        [Fact(DisplayName = "Getting a LeadViewModel with a non-existing Id should return null")]
        public async Task GetLeadViewModel_NonExistingLead_ReturnsNull()
        {
            // Arrange
            Guid leadId = Guid.NewGuid();
            _leadsDbMock
                .Setup(ld => ld.GetById(leadId))
                .ReturnsAsync((LeadViewModel)null);

            // Act
            LeadViewModel result = await _leadsService.Get(leadId);

            // Assert
            result.Should().BeNull();
            _leadsDbMock.Verify(ld => ld.GetById(leadId), Times.Once);
            _subAreasDbMock.Verify(sad => sad.GetById(It.IsAny<int>()), Times.Never);
        }

        [Fact(DisplayName = "Getting a LeadViewModel with a null subarea assigns null to SubArea property")]
        public async Task GetLeadViewModel_OnNullSubArea_ThrowsNullReferenceException()
        {
            // Arrange
            LeadViewModel leadViewModel = _fixture.Build<LeadViewModel>()
                .Without(lvm => lvm.SubArea)
                .Create();

            Guid leadId = Guid.NewGuid();

            _leadsDbMock
                .Setup(ld => ld.GetById(leadId))
                .ReturnsAsync(leadViewModel);

            _subAreasDbMock
                .Setup(sad => sad.GetById(leadViewModel.SubAreaId))
                .ReturnsAsync((SubAreaViewModel)null);
            
            // Act
            LeadViewModel result = await _leadsService.Get(leadId);

            // Act/Assert
            result.Should().Be(leadViewModel);
            result.SubArea.Should().BeNull();
            _leadsDbMock.Verify(ld => ld.GetById(leadId), Times.Once);
            _subAreasDbMock.Verify(sad => sad.GetById(leadViewModel.SubAreaId), Times.Once);
        }

        [Fact(DisplayName = "Getting a LeadSaveModel throws up stack when LeadDb throws")]
        public async Task GetLeadSaveModel_OnLeadDbException_ThrowsUpStack()
        {
            // Arrange
            Guid leadId = Guid.NewGuid();
            
            _leadsDbMock
                .Setup(ld => ld.GetById(leadId))
                .ThrowsAsync(new Exception());

            // Act/Assert
            await _leadsService
                .Invoking(ls => ls.Get(leadId))
                .Should()
                .ThrowAsync<Exception>();

            _subAreasDbMock.Verify(sad => sad.GetById(It.IsAny<int>()), Times.Never);
            _leadsDbMock.Verify(sad => sad.GetById(leadId), Times.Once);
        }

        [Fact(DisplayName = "Getting a LeadSaveModel throws up stack when SubAreaDb throws")]
        public async Task GetLeadSaveModel_OnSubAreaDbException_ThrowsUpStack()
        {
            // Arrange
            Guid leadId = Guid.NewGuid();
            
            LeadViewModel leadViewModel = _fixture.Build<LeadViewModel>()
                .Without(lvm => lvm.SubArea)
                .Create();

            _leadsDbMock
                .Setup(ld => ld.GetById(leadId))
                .ReturnsAsync(leadViewModel);

            _subAreasDbMock
                .Setup(sad => sad.GetById(leadViewModel.SubAreaId))
                .ThrowsAsync(new Exception());

            // Act/Assert
            await _leadsService
                .Invoking(ls => ls.Get(leadId))
                .Should()
                .ThrowAsync<Exception>();

            _subAreasDbMock.Verify(sad => sad.GetById(leadViewModel.SubAreaId), Times.Once);
            _leadsDbMock.Verify(sad => sad.GetById(leadId), Times.Once);
        }
    }
}