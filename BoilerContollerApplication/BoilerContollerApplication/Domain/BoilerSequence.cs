namespace BoilerContollerApplication.Domain;

public class BoilerSequence
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
}
