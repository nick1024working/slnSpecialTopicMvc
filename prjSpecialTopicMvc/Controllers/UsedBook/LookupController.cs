using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using prjSpecialTopicMvc.Features.Usedbook.Application.Services;

namespace prjSpecialTopicMvc.Controllers.UsedBook
{
    [ApiController]
    [Route("api/lookup")]
    public class LookupController : ControllerBase
    {
        private readonly LookupService _lookupService;

        public LookupController(
            LookupService lookupService)
        {
            _lookupService = lookupService;
        }

        [HttpGet("districts")]
        public async Task<ActionResult<List<SelectListItem>>> District([FromQuery] int countyId, CancellationToken ct)
        {
            var queryResult = await _lookupService.GetDistrictSelectListByCountyIdAsync(countyId, ct);

            if (!queryResult.IsSuccess)
                return BadRequest(queryResult.ErrorMessage);

            return Ok(queryResult);
        }
    }
}
