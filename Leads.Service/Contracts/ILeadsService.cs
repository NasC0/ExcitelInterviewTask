using System;
using System.Threading.Tasks;

using Leads.Models;

namespace Leads.Services.Contracts
{
    public interface ILeadsService
    {
        Task<Guid> Save(LeadSaveModel lead);
        
        Task<LeadViewModel> Get(Guid id);
    }
}