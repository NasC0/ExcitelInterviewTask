[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("LeadsService.Services.Tests")]
namespace Leads.Services
{
    using System;
    using System.Threading.Tasks;

    using DbAdapter;
    using Models;

    public class LeadsService
    {
        private readonly ILeadsDb leadsDb;
        private readonly ISubAreasDb subAreasDb;

        public LeadsService(ILeadsDb leadsDb, ISubAreasDb subAreasDb)
        {
            this.leadsDb = leadsDb;
            this.subAreasDb = subAreasDb;
        }

        public async Task<Guid> Save(LeadSaveModel lead)
        {
            ValidateSaveModel(lead);
            await ValidateSubArea(lead.SubAreaId, lead.PinCode)
                .ConfigureAwait(false);
            return await this.leadsDb.Save(lead)
                .ConfigureAwait(false);
        }

        private async Task ValidateSubArea(int leadSubAreaId, string leadPinCode)
        {
            var subarea = await this.subAreasDb.GetById(leadSubAreaId)
                              .ConfigureAwait(false);
            if (subarea == null || subarea.PinCode != leadPinCode)
            {
                throw new ArgumentException("SubArea is invalid");
            }
        }

        public async Task<LeadViewModel> Get(Guid id)
        {
            var lead = await this.leadsDb.GetById(id);
            if (lead is null)
            {
                return null;
            }

            lead.SubArea = await this.subAreasDb.GetById(lead.SubAreaId);

            return lead;
        }

        private void ValidateSaveModel(LeadSaveModel candidate)
        {
            if (candidate is null)
            {
                throw new ArgumentNullException(nameof(candidate), "cannot be null");
            }

            if (string.IsNullOrWhiteSpace(candidate.Name))
            {
                throw new ArgumentException("Name cannot be empty", nameof(candidate.Name));
            }

            if (string.IsNullOrWhiteSpace(candidate.PinCode))
            {
                throw new ArgumentException("PinCode cannot be empty", nameof(candidate.PinCode));
            }

            if (string.IsNullOrWhiteSpace(candidate.Address))
            {
                throw new ArgumentException("Address cannot be empty", nameof(candidate.Address));
            }

            candidate.Name = candidate.Name.Trim();
            candidate.PinCode = candidate.PinCode.Trim();
            candidate.Address = candidate.Address.Trim();
        }
    }
}
