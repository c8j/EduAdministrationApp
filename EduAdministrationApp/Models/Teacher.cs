namespace EduAdministrationApp.Models;

public class Teacher : IIdentifiable
{
    public int ID { get; init; }
    public required ContactDetails ContactDetails { get; init; }
    public required string Department { get; init; }
    public List<Course>? CoursesHandled { get; init; }
}
