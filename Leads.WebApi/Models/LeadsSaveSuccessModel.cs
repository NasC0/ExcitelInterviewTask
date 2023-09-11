namespace Leads.WebApi.Models
{
    using System;

    public class LeadsSaveSuccessModel
    {
        public LeadsSaveSuccessModel(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }
}
