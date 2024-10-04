namespace EduAdministrationApp.Models;

public class Student : IIdentifiable
{
    public int ID { get; init; }
    public required ContactDetails ContactDetails { get; init; }
}
