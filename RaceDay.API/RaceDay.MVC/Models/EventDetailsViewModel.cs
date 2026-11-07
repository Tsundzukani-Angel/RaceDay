namespace RaceDay.MVC.Models;

public class EventDetailsViewModel
{
    public EventDto Event { get; set; } = null!;
    public bool IsParticipant { get; set; }
    public bool AlreadyEnrolled { get; set; }
}