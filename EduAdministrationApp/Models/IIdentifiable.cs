namespace EduAdministrationApp.Models;

public interface IIdentifiable
{
    public int ID { get; init; }
    public ContactDetails ContactDetails { get; init; }
}
