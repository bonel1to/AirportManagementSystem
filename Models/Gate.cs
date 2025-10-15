public class Gate
{
    public int GateID { get; set; }
    public string GateName { get; set; }
    public int AirportID { get; set; }
    public string GateType { get; set; }
    public int? Capacity { get; set; }
    public bool IsOperational { get; set; }
    public DateTime CreatedDate { get; set; }

    // Навигационные свойства
    public Airport? Airport { get; set; }
    public List<Airplane> Airplanes { get; set; } = new List<Airplane>();
}
