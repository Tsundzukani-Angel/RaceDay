namespace RaceDay.MVC.Models;

public class EventDto
{
    public int EventID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public double Distance { get; set; }
    public string EventType { get; set; } = string.Empty;
    public int MaxParticipants { get; set; }
    public DateTime? RegistrationDeadline { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? BannerUrl { get; set; }
    public List<CategoryDto> Categories { get; set; } = new();
}

public class CategoryDto
{
    public int CategoryID { get; set; }
    public int EventID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryType { get; set; }
    public decimal EntryFee { get; set; }
}
public class EnrolmentDto
{
    public int EnrolmentID { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public string? RaceNumber { get; set; }
    public EnrolmentEventDto Event { get; set; } = null!;
    public EnrolmentCategoryDto Category { get; set; } = null!;
}

public class EnrolmentEventDto
{
    public int EventID { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
}

public class EnrolmentCategoryDto
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal EntryFee { get; set; }
}

public class OrganiserEnrolmentDto
{
    public int EnrolmentID { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public string? RaceNumber { get; set; }
    public OrganiserParticipantDto Participant { get; set; } = null!;
    public string Category { get; set; } = string.Empty;
}

public class OrganiserParticipantDto
{
    public int UserID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class ResultDto
{
    public int ResultID { get; set; }
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Category { get; set; } = string.Empty;
    public TimeSpan FinishTime { get; set; }
    public int Position { get; set; }
    public string Status { get; set; } = string.Empty;
}