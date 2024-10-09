using System.Text.Json.Serialization;

namespace EduAdministrationApp.Models;

public class Course : IIdentifiable
{
    public required int ID { get; init; }
    public required string Title { get; init; }
    public required int LengthInWeeks { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required bool IsDistanceBased { get; init; }

    private readonly List<int> _studentIDs;
    public IReadOnlyList<int> StudentIDs { get; }

    [JsonConstructor]
    public Course(List<int> studentIDs)
    {
        _studentIDs = studentIDs ??
         throw new NullReferenceException(string.Format(Database.Prompts[Database.Prompt.CourseNullList], ID));
        StudentIDs = _studentIDs.AsReadOnly();
    }

    public void AddStudent(int studentID)
    {
        //TODO: validation
        _studentIDs.Add(studentID);
    }

    public void RemoveStudent(int studentID)
    {
        //TODO: validation
        _studentIDs.Remove(studentID);
    }
}
