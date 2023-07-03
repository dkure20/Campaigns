using Campaign.Model;
using Campaign.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Campaign.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaign _campaign;
        public CampaignController(ICampaign campaign)
        {
            _campaign = campaign;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(RequestCampaign campaign)
        {
           await _campaign.CreateCampaign(campaign);
            return Ok(campaign);
        }

        [HttpPut("ChangeState")]
        public async Task<IActionResult> StateChange(string campaignName, State state)
        {
            await _campaign.ChangeCampaignState(campaignName, state);
            return Ok();
        }
        [HttpPut("Edit")]
        public async Task<IActionResult> MakeEdit(CampaignInfo campaign)
        {
            await _campaign.Edit(campaign);
            return Ok();
        }
        [HttpPut("ChangeStatus")]
        public async Task<IActionResult> StatusChange(string campaignName, Status status)
        {
            await _campaign.ChangeCampaignStatus(campaignName, status);
            return Ok();
        }
        [HttpDelete("DeleteCampaign")]
        public async Task<IActionResult> CampaignDelete(int id)
        {
            await _campaign.DeleteCampaign(id);
            return Ok();
        }
        [HttpPost("CloneCampaign")]
        public async Task<IActionResult> CloneCampaign(int id)
        {
            await _campaign.Clone(id);
            return Ok();
        }
        [HttpPost("Filter")]
        public async Task<IActionResult> Filter([FromBody]Filter filter)
        {
            var result = await _campaign.FilterCampaigns(filter);
            return Ok(result);
        }

    }
}
