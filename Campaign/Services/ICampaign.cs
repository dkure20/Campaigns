using Campaign.Model;

namespace Campaign.Services
{
    public interface ICampaign
    {
        Task ChangeCampaignState(string campaignName, State state);
        Task ChangeCampaignStatus(string campaignName, Status status);
        Task Clone(int id);
        Task CreateCampaign(RequestCampaign campaign);
        Task DeleteCampaign(int id);
        Task Edit(CampaignInfo campaign);
        Task<List<CampaignInfo>> FilterCampaigns(Filter filter);
    }
}
