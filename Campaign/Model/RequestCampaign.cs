namespace Campaign.Model
{
    public class RequestCampaign
    {
        public string CampaignName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Reward RewardType { get; set; }
    }
}
