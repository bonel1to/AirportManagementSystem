public class Airport
{
    public int AirportID { get; set; }
    public string AirportCode { get; set; }
    public string AirportName { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }

    // Навигационное свойство
    public List<Gate>? Gates { get; set; }
}
