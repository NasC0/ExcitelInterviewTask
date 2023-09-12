using System.Collections;
using System.Collections.Generic;

using AutoFixture;

using Leads.Models;

namespace Leads.Services.UnitTests.TestData
{
    public class InvalidLeadSaveModelData : IEnumerable<object[]>
    {
        private readonly Fixture _fixture = new Fixture();

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { null };
        
            yield return new object[] { _fixture.Build<LeadSaveModel>()
                .Without(lsm => lsm.Name)
                .Create() };
        
            yield return new object[] { _fixture.Build<LeadSaveModel>()
                .Without(lsm => lsm.PinCode)
                .Create() };
        
            yield return new object[] { _fixture.Build<LeadSaveModel>()
                .Without(lsm => lsm.Address)
                .Create() };
        
            yield return new object[] { _fixture.Build<LeadSaveModel>()
                .With(lsm => lsm.Name, string.Empty)
                .Create() };
        
            yield return new object[] { _fixture.Build<LeadSaveModel>()
                .With(lsm => lsm.PinCode, string.Empty)
                .Create() };
        
            yield return new object[] { _fixture.Build<LeadSaveModel>()
                .With(lsm => lsm.Address, string.Empty)
                .Create() };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}