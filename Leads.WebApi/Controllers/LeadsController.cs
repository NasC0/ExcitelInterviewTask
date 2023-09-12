using System;
using System.Threading.Tasks;

using Leads.Models;
using Leads.Services.Contracts;
using Leads.WebApi.Models;

using Microsoft.AspNetCore.Mvc;

namespace Leads.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadsController : ControllerBase
    {
        private readonly ILeadsService leadsService;

        public LeadsController(ILeadsService leadsService)
        {
            this.leadsService = leadsService;
        }

        /// <summary>
        /// Gets Lead by id
        /// </summary>
        /// <param name="id">Guid of the lead</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "Get")]
        [ProducesResponseType(200, Type = typeof(LeadViewModel))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<LeadViewModel>> Get(Guid id)
        {
            var lead = await leadsService.Get(id);
            if (lead is null)
            {
                return this.NotFound();
            }

            return this.Ok(lead);
        }

        /// <summary>
        /// Saves new lead
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(LeadsSaveSuccessModel))]
        [ProducesResponseType(400, Type = typeof(ErrorViewModel))]
        public async Task<ActionResult<LeadsSaveSuccessModel>> Post([FromBody] LeadsSaveViewModel candidate)
        {
            if (!candidate.SubAreaId.HasValue)
            {
                return this.BadRequest(new ErrorViewModel("SubArea must have valid value"));
            }
            try
            {
                var leadSaveModel = new LeadSaveModel
                                        {
                                            Name = candidate.Name,
                                            Address = candidate.Address,
                                            PinCode = candidate.PinCode,
                                            MobileNumber = candidate.MobileNumber,
                                            Email = candidate.Email,
                                            SubAreaId = candidate.SubAreaId.Value
                                        };

                var result = await leadsService.Save(leadSaveModel);

                return this.Ok(new LeadsSaveSuccessModel(result));
            }
            catch (Exception e)
            {
                return this.BadRequest(new ErrorViewModel(e.Message));
            }
        }
    }
}
