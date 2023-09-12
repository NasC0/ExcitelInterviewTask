using System.Collections.Generic;
using System.Threading.Tasks;

using Leads.Models;

namespace Leads.Services.Contracts
{
    public interface ISubAreasService
    {
        Task<List<SubAreaViewModel>> GetAll();
        
        Task<List<SubAreaViewModel>> GetByPinCode(string pinCode);
    }
}