namespace Campaign.Model
{
    public class Filter
    {
        public string? CampaignName { get; set; }
        public Reward? RewardType { get; set; }
        public State? State { get; set; }
        public Status? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int AmountOfCampaign { get; set; }
        public int PageId { get; set; }
    }
}
