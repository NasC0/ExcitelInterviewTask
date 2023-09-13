using System.Collections;
using System.Collections.Generic;

using AutoFixture;

using Leads.WebApi.Models;

namespace Leads.IntegrationTests.TestData
{
    public class InvalidLeadSaveViewModelData : IEnumerable<object[]>
    {
        private Fixture _fixture = new Fixture();
        
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { null };
        
            yield return new object[] { _fixture.Build<LeadsSaveViewModel>()
                .Without(lsm => lsm.Name)
                .Create() };
        
            yield return new object[] { _fixture.Build<LeadsSaveViewModel>()
                .Without(lsm => lsm.PinCode)
                .Create() };
        
            yield return new object[] { _fixture.Build<LeadsSaveViewModel>()
                .Without(lsm => lsm.Address)
                .Create() };
        
            yield return new object[] { _fixture.Build<LeadsSaveViewModel>()
                .With(lsm => lsm.Name, string.Empty)
                .Create() };
        
            yield return new object[] { _fixture.Build<LeadsSaveViewModel>()
                .With(lsm => lsm.PinCode, string.Empty)
                .Create() };
        
            yield return new object[] { _fixture.Build<LeadsSaveViewModel>()
                .With(lsm => lsm.Address, string.Empty)
                .Create() };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}