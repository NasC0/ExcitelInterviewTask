namespace Leads.Database.Ef
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Leads.Database.Ef.Models;
    using Leads.DbAdapter;
    using Leads.Models;

    public class LeadsEfDb : ILeadsDb
    {
        private readonly LeadsContext dbContext;

        public LeadsEfDb(LeadsContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Guid> Save(LeadSaveModel lead)
        {
            var leadModel = new Lead
                                {
                                    Name = lead.Name,
                                    PinCode = lead.PinCode,
                                    Address = lead.Address,
                                    SubAreaId = lead.SubAreaId,
                                    Email = lead.Email,
                                    MobileNumber = lead.MobileNumber
                                };

            await dbContext.Leads.AddAsync(leadModel);
            await dbContext.SaveChangesAsync();

            return leadModel.Id;
        }

        public Task<LeadViewModel> GetById(Guid id)
        {
            return dbContext.Leads
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new LeadViewModel
                                 {
                                     Id = x.Id,
                                     Address = x.Address,
                                     Name = x.Name,
                                     PinCode = x.PinCode,
                                     MobileNumber = x.MobileNumber,
                                     Email = x.Email,
                                     SubAreaId = x.SubAreaId
                                 })
                .SingleOrDefaultAsync();
        }
    }
}
