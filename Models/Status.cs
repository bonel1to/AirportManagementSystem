public class Status
{
    public int StatusID { get; set; }
    public string StatusName { get; set; }
    public string StatusDescription { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }

    // Навигационное свойство
    public List<Airplane>? Airplanes { get; set; }
}
