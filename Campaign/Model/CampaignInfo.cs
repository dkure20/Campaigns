namespace Campaign.Model
{
    public class CampaignInfo
    {
        public int Id { get; set; }
        public DateTime CreateDate {  get; set; }
        public String CampaignName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public State State { get; set; }
        public Status Status { get; set; }
        public Reward RewardType { get; set; }
        public bool IsDeleted { get; set; }
    }

    public enum State
    {
        UnPublished = 0, Published = 1
    }
    public enum Status
    {
        Active = 0,
        Cancelled = 1,
        ReActivated = 2,
        Initialized = 3,
        Finished = 4
    }
    public enum Reward
    {
        Cash = 0,
        FreeSpin = 1,
        FreeBet = 2
    }
}
