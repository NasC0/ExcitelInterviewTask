namespace Leads.DbAdapter
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Models;

    public interface ISubAreasDb
    {
        Task<List<SubAreaViewModel>> GetAll();

        Task<List<SubAreaViewModel>> GetByPinCode(string pinCode);

        Task<SubAreaViewModel> GetById(int id);
    }
}
