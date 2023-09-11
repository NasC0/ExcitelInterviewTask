namespace Leads.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Leads.Models;
    using Leads.Services;

    [Route("api/[controller]")]
    [ApiController]
    public class SubAreasController : ControllerBase
    {
        private readonly SubAreasService subAreasService;

        public SubAreasController(SubAreasService subAreasService)
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
