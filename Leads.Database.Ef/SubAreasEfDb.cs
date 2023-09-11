namespace Leads.Database.Ef
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Leads.DbAdapter;
    using Leads.Models;

    public class SubAreasEfDb : ISubAreasDb
    {
        private readonly LeadsContext dbContext;

        public SubAreasEfDb(LeadsContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<List<SubAreaViewModel>> GetAll()
        {
            return this.dbContext.SubAreas
                .Select(x => new SubAreaViewModel { Id = x.Id, Name = x.Name, PinCode = x.PinCode})
                .ToListAsync();
        }

        public Task<SubAreaViewModel> GetById(int id)
        {
            return this.dbContext.SubAreas
                .Where(x => x.Id == id)
                .Select(x => new SubAreaViewModel { Id = x.Id, Name = x.Name, PinCode = x.PinCode })
                .SingleOrDefaultAsync();
        }

        public Task<List<SubAreaViewModel>> GetByPinCode(string pinCode)
        {
            return this.dbContext.SubAreas
                .Where(x => x.PinCode == pinCode)
                .Select(x => new SubAreaViewModel { Id = x.Id, Name = x.Name, PinCode = x.PinCode })
                .ToListAsync();
        }
    }
}
