namespace EduAdministrationApp.Models;

public class Student : IIdentifiable
{
    public required int ID { get; init; }
    public required ContactDetails ContactDetails { get; init; }

    public override string ToString()
    {
        return $"ID: {ID}, " + ContactDetails;
    }
}
