namespace Leads.Database.Static
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Leads.Models;
    using Leads.DbAdapter;

    public class SubAreasStaticDatabase : ISubAreasDb
    {
        private readonly List<SubAreaViewModel> subareas = new List<SubAreaViewModel>()
              {
                   new SubAreaViewModel {Id = 1, PinCode = "123", Name = "Name1"}
                  ,new SubAreaViewModel {Id = 2, PinCode = "123", Name = "Name2"}
                  ,new SubAreaViewModel {Id = 3, PinCode = "123", Name = "Name3"}
                  ,new SubAreaViewModel {Id = 4, PinCode = "567", Name = "Name4"}
                  ,new SubAreaViewModel {Id = 5, PinCode = "567", Name = "Name5"}
                  ,new SubAreaViewModel {Id = 6, PinCode = "567", Name = "Name6"}
              };

        public Task<List<SubAreaViewModel>> GetAll()
        {
            return Task.FromResult(
                this.subareas
                    .Select(item => new SubAreaViewModel { Id = item.Id, Name = item.Name, PinCode = item.PinCode })
                    .ToList()
                );
        }

        public Task<List<SubAreaViewModel>> GetByPinCode(string pinCode)
        {
            return Task.FromResult(
                this.subareas
                    .Where(x => x.PinCode == pinCode)
                    .Select(item => new SubAreaViewModel { Id = item.Id, Name = item.Name, PinCode = item.PinCode })
                    .ToList()
            );
        }

        public Task<SubAreaViewModel> GetById(int id)
        {
            return Task.FromResult(
                this.subareas
                    .Where(x => x.Id == id)
                    .Select(item => new SubAreaViewModel { Id = item.Id, Name = item.Name, PinCode = item.PinCode })
                    .FirstOrDefault()
            );
        }
    }
}
