using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class GateRepository
{
    private readonly DatabaseService _databaseService;

    public GateRepository(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<IEnumerable<Gate>> GetAllGatesWithAirportsAsync()
    {
        string sql = @"
            SELECT g.*, a.AirportCode, a.AirportName, a.City, a.Country
            FROM Gate g
            INNER JOIN Airport a ON g.AirportID = a.AirportID
            WHERE g.IsOperational = 1
            ORDER BY a.AirportName, g.GateName";

        return await _databaseService.QueryAsync<Gate, Airport, Gate>(
            sql,
            (gate, airport) =>
            {
                gate.Airport = airport;
                return gate;
            },
            splitOn: "AirportCode");
    }

    public async Task<Gate> GetGateByIdAsync(int gateId)
    {
        string sql = @"
            SELECT g.*, a.AirportCode, a.AirportName, a.City, a.Country
            FROM Gate g
            INNER JOIN Airport a ON g.AirportID = a.AirportID
            WHERE g.GateID = @GateId";

        var result = await _databaseService.QueryAsync<Gate, Airport, Gate>(
            sql,
            (gate, airport) =>
            {
                gate.Airport = airport;
                return gate;
            },
            new { GateId = gateId },
            splitOn: "AirportCode");

        return result.FirstOrDefault();
    }
}
