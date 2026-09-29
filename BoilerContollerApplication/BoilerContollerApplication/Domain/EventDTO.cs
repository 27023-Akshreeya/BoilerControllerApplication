namespace BoilerContollerApplication.Domain;

public class EventDTO
{
    public string Status { get; set; }

    public TimeSpan TimeLeft { get; set; }

    public EventDTO(string status, TimeSpan time)
    {
        Status = status;
        TimeLeft = time;
    }
}
