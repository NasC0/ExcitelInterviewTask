namespace Leads.Models
{
    using System;

    public class LeadViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PinCode { get; set; }
        public int SubAreaId { get; set; }
        public string Address { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }

        public SubAreaViewModel SubArea { get; set; }
    }
}
