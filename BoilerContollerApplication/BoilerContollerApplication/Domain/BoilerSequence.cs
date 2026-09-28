namespace BoilerContollerApplication.Domain;

public class BoilerSequence
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
}
