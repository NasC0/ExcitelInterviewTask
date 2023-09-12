using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Leads.DbAdapter;
using Leads.Models;
using Leads.Services.Contracts;

[assembly: InternalsVisibleTo("LeadsService.Services.Tests")]
namespace Leads.Services
{
    public class SubAreasService : ISubAreasService
    {
        private readonly ISubAreasDb subAreasDb;

        public SubAreasService(ISubAreasDb subAreasDb)
        {
            this.subAreasDb = subAreasDb;
        }

        public Task<List<SubAreaViewModel>> GetAll()
        {
            return this.subAreasDb.GetAll();
        }

        public Task<List<SubAreaViewModel>> GetByPinCode(string pinCode)
        {
            if (string.IsNullOrWhiteSpace(pinCode))
            {
                throw new ArgumentException("PinCode cannot be null or empty");
            }

            return this.subAreasDb.GetByPinCode(pinCode);
        }
    }
}
