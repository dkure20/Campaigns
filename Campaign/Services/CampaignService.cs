using Campaign.Model;
using Campaign.Model.Config;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Campaign.Services
{
    public class CampaignService : ICampaign
    {
        private readonly Connection _Connection;
        public CampaignService(IOptions<Connection> connectionString)
        {
            _Connection = connectionString.Value;
        }

        public async Task ChangeCampaignState(int id, State newState)
        {
            using (var connect = new NpgsqlConnection(_Connection.ConnectionString))
            {
                var query = "SELECT state from campaign where id = @id";
                var previousState = await connect.QuerySingleOrDefaultAsync<State>(query, new { id });
                if (previousState == State.UnPublished && newState == State.Published)
                {
                    var updateUnpublishedState = "Update campaign set state = @newState, status = 0 where id = @id";
                    await connect.ExecuteAsync(updateUnpublishedState, new { newState, id });
                }
            }
        }

        public async Task ChangeCampaignStatus(int id, Status status)
        {
            using (var connect = new NpgsqlConnection(_Connection.ConnectionString))
            {
                if(status == Status.Active || status == Status.ReActivated)
                {
                var query = "Update campaign set state = 1, status = @status where id = @id";
                await connect.ExecuteAsync(query, new { status, id });
                }
                else if (status == Status.Cancelled)
                {
                    var query = "Update campaign set state = 0, status = @status where id = @id";
                    await connect.ExecuteAsync(query, new { status, id });
                }
            }
        }

        public async Task<int> CreateCampaign(RequestCampaign requestCampaign)
        {
            CampaignInfo campaignInfo = new CampaignInfo();
            campaignInfo.CreateDate = DateTime.UtcNow;
            campaignInfo.CampaignName = requestCampaign.CampaignName;
            campaignInfo.StartDate = requestCampaign.StartDate;
            campaignInfo.EndDate = requestCampaign.EndDate;
            campaignInfo.RewardType = requestCampaign.RewardType;
            campaignInfo.State = State.UnPublished;
            campaignInfo.Status = Status.Initialized;
            campaignInfo.IsDeleted = false;
            try
            {
                using (var connect = new NpgsqlConnection(_Connection.ConnectionString))
                {
                    var query = @"
                         INSERT INTO 
            campaign (create_date, campaign_name, start_date, end_date, reward_type, status, state, is_deleted) 
            VALUES (@CreateDate, @CampaignName, @StartDate, @EndDate, @RewardType, @Status, @State, @IsDeleted)
                    RETURNING id;";
                    int insertedCampaignId = await connect.ExecuteScalarAsync<int>(query, campaignInfo);
                    return insertedCampaignId;
                }
            }
            catch
            {
                throw new Exception("Error Occured while creating new Campaign");
            }

            
        }

        public async Task DeleteCampaign(int id)
        {
            using (var connect = new NpgsqlConnection(_Connection.ConnectionString))
            {
                var query = "SELECT status from campaign where id = @id";
                var res = await connect.QuerySingleOrDefaultAsync<Status>(query);
                if (res == Status.Active || res == Status.ReActivated)
                {
                    throw new Exception("You can't delete active campaign");
                }
                else
                {
                    var deleteQuery = "update campaign set is_deleted = true where id = @id";
                    await connect.ExecuteAsync(deleteQuery, new { id });
                }
            }
        }

        public async Task Edit(CampaignInfo campaign)
        {
            using (var connect = new NpgsqlConnection(_Connection.ConnectionString))
            {
                var query = "SELECT state from campaign where id = @Id";
                var st = await connect.QuerySingleOrDefaultAsync<State>(query, new { campaign.Id });
                if (st == State.Published)
                {
                    var publishedUpdate = "UPDATE campaign set end_date = @EndDate,reward_type = @RewardType where id = @Id";
                    await connect.ExecuteAsync(publishedUpdate, new { campaign.EndDate, campaign.RewardType, campaign.Id });
                }
                else
                {
                    var unPublishedUpdate = "UPDATE campaign set campaign_name = @CampaignName, start_date = @StartDate,end_date = @EndDate,reward_type = @RewardType where id = @Id";
                    await connect.ExecuteAsync(unPublishedUpdate, new { campaign.CampaignName, campaign.StartDate, campaign.EndDate, campaign.RewardType, campaign.Id });

                }
            }
        }
        public async Task Clone(int id)
        {
            using (var connect = new NpgsqlConnection(_Connection.ConnectionString))
            {
                var query = "SELECT start_date as StartDate, campaign_name as CampaignName, end_date as EndDate, reward_type as RewardType from campaign where id = @id";
                var res = await connect.QuerySingleOrDefaultAsync<CampaignInfo>(query, new { id });
                var str = res.CampaignName;
                if (str.IndexOf("(") != -1)
                {
                    str = str.Substring(0, str.IndexOf("("));
                }
                var secondQuery = "select count(*) from campaign where campaign_name like @str || '%'";
                var queryNum = await connect.QuerySingleOrDefaultAsync<int>(secondQuery, new { str });
                CampaignInfo campaignInfo = new CampaignInfo();
                campaignInfo.CreateDate = DateTime.UtcNow;
                campaignInfo.CampaignName = str + "(" + queryNum + ")";
                campaignInfo.StartDate = res.StartDate;
                campaignInfo.EndDate = res.EndDate;
                campaignInfo.RewardType = res.RewardType;
                campaignInfo.State = State.UnPublished;
                campaignInfo.Status = Status.Initialized;
                campaignInfo.IsDeleted = false;
                var cloneQuery = "INSERT INTO campaign (create_date,campaign_name,start_date, end_date, reward_type,status,state,is_deleted) values (@CreateDate, @CampaignName,@StartDate,@EndDate,@RewardType,@Status,@State,@IsDeleted);";
                await connect.ExecuteAsync(cloneQuery, campaignInfo);
            }


        }

        public async Task<FilteredCampaigns> FilterCampaigns(Filter filter)
        {
            FilteredCampaigns filteredCampaigns = new FilteredCampaigns();
            using (var connect = new NpgsqlConnection(_Connection.ConnectionString))
            {
                var query = @"select create_date as CreateDate
                        , campaign_name as CampaignName
                        , start_date as StartDate
                        , end_date as EndDate
                        , reward_type as RewardType
                        , state as State
                        , status as Status 
                        from campaign 
                            where (campaign_name like @CampaignName || '%' or @CampaignName is null) 
                            and (reward_type = @RewardType or @RewardType is null)
                            and (state = @State or @State is null) 
                            and (status = @Status or @Status is Null) and
                            (start_date >= @StartDate or @StartDate is null) 
                            and (end_date <= @EndDate or @EndDate is null) 
                                limit @AmountOfCampaign 
                                offset @PageId";
                var totalAmountQuery = "SELECT count(*) from campaign";
                var result = await connect.QueryAsync<CampaignInfo>(query, filter);
                var totalResult = await connect.QuerySingleOrDefaultAsync<int> (totalAmountQuery);
                filteredCampaigns.CampaignList = result.ToList();
                filteredCampaigns.TotalCampaigns = totalResult;
                return filteredCampaigns;
            }
        }
    }
}
