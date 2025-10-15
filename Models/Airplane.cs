public class Airplane
{
    public int AirplaneID { get; set; }
    public string Model { get; set; }
    public int Capacity { get; set; }
    public string Airline { get; set; }
    public int? StatusID { get; set; }
    public int? GateID { get; set; }
    public string RegistrationNumber { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public DateTime CreatedDate { get; set; }

    // Навигационные свойства
    public Gate? Gate { get; set; }
    public Status? Status { get; set; }
}
