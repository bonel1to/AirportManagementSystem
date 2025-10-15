public class AirportRepository
{
    private readonly DatabaseService _databaseService;

    public AirportRepository(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<IEnumerable<Airport>> GetAllAirportsAsync()
    {
        string sql = "SELECT * FROM Airport WHERE IsActive = 1 ORDER BY AirportCode";
        return await _databaseService.QueryAsync<Airport>(sql);
    }

    public async Task<Airport> GetAirportByIdAsync(int airportId)
    {
        string sql = "SELECT * FROM Airport WHERE AirportID = @AirportId";
        return await _databaseService.QueryFirstOrDefaultAsync<Airport>(sql, new { AirportId = airportId });
    }

    public async Task<Airport> GetAirportByCodeAsync(string airportCode)
    {
        string sql = "SELECT * FROM Airport WHERE AirportCode = @AirportCode";
        return await _databaseService.QueryFirstOrDefaultAsync<Airport>(sql, new { AirportCode = airportCode });
    }
}
