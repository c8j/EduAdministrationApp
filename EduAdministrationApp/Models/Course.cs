namespace EduAdministrationApp.Models;

public class Course : IIdentifiable
{

    public required int ID { get; init; }
    public required string Title { get; init; }
    public required int LengthInWeeks { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required bool IsDistanceBased { get; init; }

    private readonly List<int> _studentIDs = [];
    public IReadOnlyList<int> StudentIDs { get; }

    public Course()
    {
        StudentIDs = _studentIDs.AsReadOnly();
    }

    public Course(List<int> studentIDs) : this()
    {
        _studentIDs.AddRange(studentIDs);
    }

    public void AddStudent(int studentID)
    {
        _studentIDs.Add(studentID);
    }

    public void RemoveStudent(int studentID)
    {
        _studentIDs.Remove(studentID);
    }

    public override string ToString()
    {
        return
            $"ID: {ID}, {Title} - Längd: {LengthInWeeks} {(LengthInWeeks > 1 ? "veckor" : "vecka")}, " +
            $"{StartDate:yyyy-MM-dd} : {EndDate:yyyy-MM-dd}, Distans: {(IsDistanceBased ? "Ja" : "Nej")}";
    }
}
