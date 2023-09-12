using System.Collections.Generic;
using System.Threading.Tasks;

using Leads.Models;
using Leads.Services.Contracts;

using Microsoft.AspNetCore.Mvc;

namespace Leads.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubAreasController : ControllerBase
    {
        private readonly ISubAreasService subAreasService;

        public SubAreasController(ISubAreasService subAreasService)
        {
            this.subAreasService = subAreasService;
        }

        /// <summary>
        /// All SubAreas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<List<SubAreaViewModel>> Get()
        {
            return this.subAreasService.GetAll();
        }

        /// <summary>
        /// Filter subareas by PinCode
        /// </summary>
        /// <param name="pinCode"></param>
        /// <returns></returns>
        [HttpGet("Filter/PinCode/{pinCode}", Name = "GetByPinCode")]
        public Task<List<SubAreaViewModel>> Get(string pinCode)
        {
            return this.subAreasService.GetByPinCode(pinCode);
        }
    }
}
