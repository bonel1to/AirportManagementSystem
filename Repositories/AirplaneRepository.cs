public class AirplaneRepository
{
    private readonly DatabaseService _databaseService;

    public AirplaneRepository(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<IEnumerable<Airplane>> GetAllAirplanesAsync()
    {
        // Загружаем самолеты с JOIN для ворот и аэропортов
        string sql = @"
            SELECT
                ap.AirplaneID, ap.Model, ap.Capacity, ap.Airline, ap.StatusID, ap.GateID,
                ap.RegistrationNumber, ap.ManufactureDate, ap.LastMaintenanceDate,
                ap.NextMaintenanceDate, ap.CreatedDate,
                g.GateName, g.GateType, g.AirportID, g.Capacity as GateCapacity, g.IsOperational,
                a.AirportCode, a.AirportName, a.City, a.Country,
                s.StatusID, s.StatusName, s.StatusDescription, s.IsActive, s.CreatedDate as StatusCreatedDate
            FROM Airplane ap
            LEFT JOIN Gate g ON ap.GateID = g.GateID
            LEFT JOIN Airport a ON g.AirportID = a.AirportID
            LEFT JOIN Status s ON ap.StatusID = s.StatusID
            ORDER BY ap.AirplaneID";

        return await _databaseService.QueryAsync<Airplane, Gate, Airport, Status, Airplane>(
            sql,
            (airplane, gate, airport, status) =>
            {
                if (gate != null)
                {
                    airplane.Gate = gate;
                    gate.Airport = airport;
                }
                if (status != null)
                {
                    airplane.Status = status;
                }
                return airplane;
            },
            splitOn: "GateName,AirportCode,StatusID");
    }

    public async Task<Airplane> GetAirplaneByIdAsync(int airplaneId)
    {
        string sql = "SELECT * FROM Airplane WHERE AirplaneID = @AirplaneId";
        return await _databaseService.QueryFirstOrDefaultAsync<Airplane>(sql, new { AirplaneId = airplaneId });
    }

    public async Task<IEnumerable<Airplane>> GetAirplanesByStatusAsync(int statusId)
    {
        string sql = "SELECT * FROM Airplane WHERE StatusID = @StatusId ORDER BY AirplaneID";
        return await _databaseService.QueryAsync<Airplane>(sql, new { StatusId = statusId });
    }

    public async Task<IEnumerable<Airplane>> GetAirplanesByAirlineAsync(string airline)
    {
        string sql = "SELECT * FROM Airplane WHERE Airline = @Airline ORDER BY AirplaneID";
        return await _databaseService.QueryAsync<Airplane>(sql, new { Airline = airline });
    }

    // Метод для получения всех самолетов с деталями
    public async Task<IEnumerable<Airplane>> GetAllAirplanesWithDetailsAsync()
    {
        // GetAllAirplanesAsync теперь уже загружает все связанные данные
        return await GetAllAirplanesAsync();
    }

    // Метод для получения самолета с деталями (для случаев, когда нужен только один самолет)
    public async Task<Airplane> GetAirplaneWithDetailsAsync(int airplaneId)
    {
        string sql = @"
            SELECT
                ap.AirplaneID, ap.Model, ap.Capacity, ap.Airline, ap.StatusID, ap.GateID,
                ap.RegistrationNumber, ap.ManufactureDate, ap.LastMaintenanceDate,
                ap.NextMaintenanceDate, ap.CreatedDate,
                g.GateName, g.GateType, g.AirportID, g.Capacity as GateCapacity, g.IsOperational,
                a.AirportCode, a.AirportName, a.City, a.Country,
                s.StatusID, s.StatusName, s.StatusDescription, s.IsActive, s.CreatedDate as StatusCreatedDate
            FROM Airplane ap
            LEFT JOIN Gate g ON ap.GateID = g.GateID
            LEFT JOIN Airport a ON g.AirportID = a.AirportID
            LEFT JOIN Status s ON ap.StatusID = s.StatusID
            WHERE ap.AirplaneID = @AirplaneId";

        var result = await _databaseService.QueryAsync<Airplane, Gate, Airport, Status, Airplane>(
            sql,
            (airplane, gate, airport, status) =>
            {
                if (gate != null)
                {
                    airplane.Gate = gate;
                    gate.Airport = airport;
                }
                if (status != null)
                {
                    airplane.Status = status;
                }
                return airplane;
            },
            new { AirplaneId = airplaneId },
            splitOn: "GateName,AirportCode,StatusID");

        return result.FirstOrDefault();
    }

    // CRUD операции для Airplane

    public async Task<int> CreateAirplaneAsync(Airplane airplane)
    {
        string sql = @"
            INSERT INTO Airplane (Model, Capacity, Airline, StatusID, GateID,
                                RegistrationNumber, ManufactureDate, LastMaintenanceDate,
                                NextMaintenanceDate, CreatedDate)
            VALUES (@Model, @Capacity, @Airline, @StatusID, @GateID,
                   @RegistrationNumber, @ManufactureDate, @LastMaintenanceDate,
                   @NextMaintenanceDate, @CreatedDate);
            SELECT CAST(SCOPE_IDENTITY() as int)";

        return await _databaseService.ExecuteScalarAsync<int>(sql, airplane);
    }

    public async Task<bool> UpdateAirplaneAsync(Airplane airplane)
    {
        string sql = @"
            UPDATE Airplane
            SET Model = @Model, Capacity = @Capacity, Airline = @Airline,
                StatusID = @StatusID, GateID = @GateID, RegistrationNumber = @RegistrationNumber,
                ManufactureDate = @ManufactureDate, LastMaintenanceDate = @LastMaintenanceDate,
                NextMaintenanceDate = @NextMaintenanceDate
            WHERE AirplaneID = @AirplaneID";

        int rowsAffected = await _databaseService.ExecuteAsync(sql, airplane);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAirplaneAsync(int airplaneId)
    {
        string sql = "DELETE FROM Airplane WHERE AirplaneID = @AirplaneId";
        int rowsAffected = await _databaseService.ExecuteAsync(sql, new { AirplaneId = airplaneId });
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateAirplaneStatusAsync(int airplaneId, int statusId)
    {
        string sql = "UPDATE Airplane SET StatusID = @StatusId WHERE AirplaneID = @AirplaneId";
        int rowsAffected = await _databaseService.ExecuteAsync(sql, new { AirplaneId = airplaneId, StatusId = statusId });
        return rowsAffected > 0;
    }

    public async Task<bool> AssignAirplaneToGateAsync(int airplaneId, int? gateId)
    {
        string sql = "UPDATE Airplane SET GateID = @GateId WHERE AirplaneID = @AirplaneId";
        int rowsAffected = await _databaseService.ExecuteAsync(sql, new { AirplaneId = airplaneId, GateId = gateId });
        return rowsAffected > 0;
    }
}
