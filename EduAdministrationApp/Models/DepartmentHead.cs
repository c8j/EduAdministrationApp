namespace EduAdministrationApp.Models;

public class DepartmentHead : Teacher
{
    public required DateTime EmployedOn { get; init; }

    public override string ToString()
    {
        return base.ToString() + $"Anställd: {EmployedOn:yyyy-MM-dd}";
    }
}
