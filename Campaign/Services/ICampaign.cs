using Campaign.Model;

namespace Campaign.Services
{
    public interface ICampaign
    {
        Task ChangeCampaignState(int id, State state);
        Task ChangeCampaignStatus(int id, Status status);
        Task Clone(int id);
        Task<int> CreateCampaign(RequestCampaign campaign);
        Task DeleteCampaign(int id);
        Task Edit(CampaignInfo campaign);
        Task<FilteredCampaigns> FilterCampaigns(Filter filter);
    }
}
