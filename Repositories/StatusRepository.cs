public class StatusRepository
{
    private readonly DatabaseService _databaseService;

    public StatusRepository(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<IEnumerable<Status>> GetAllStatusesAsync()
    {
        string sql = "SELECT * FROM Status WHERE IsActive = 1 ORDER BY StatusID";
        return await _databaseService.QueryAsync<Status>(sql);
    }

    public async Task<Status> GetStatusByIdAsync(int statusId)
    {
        string sql = "SELECT * FROM Status WHERE StatusID = @StatusId";
        return await _databaseService.QueryFirstOrDefaultAsync<Status>(sql, new { StatusId = statusId });
    }

    public async Task<Status> GetStatusByNameAsync(string statusName)
    {
        string sql = "SELECT * FROM Status WHERE StatusName = @StatusName";
        return await _databaseService.QueryFirstOrDefaultAsync<Status>(sql, new { StatusName = statusName });
    }
}
