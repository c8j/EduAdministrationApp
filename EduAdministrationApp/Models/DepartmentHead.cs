namespace EduAdministrationApp.Models;

public class DepartmentHead(List<int> courseIDs) : Teacher(courseIDs)
{
    public required DateTime EmployedOn { get; init; }
}
