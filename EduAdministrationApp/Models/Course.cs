using System.Text.Json.Serialization;

namespace EduAdministrationApp.Models;

public class Course
{
    public int ID { get; init; }
    public required string Title { get; init; }
    public int LengthInWeeks { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public bool IsDistanceBased { get; init; }
    public List<int> StudentIDs { get; }
    public List<int> TeacherIDs { get; }

    [JsonConstructor]
    public Course(List<int> teacherIDs, List<int> studentIDs)
    {
        if (teacherIDs is null || teacherIDs.Count <= 0)
        {
            throw new Exception(string.Format(Database.Prompts[Database.Prompt.NotEnoughTeachers], ID));
        }

        TeacherIDs = teacherIDs;
        if (studentIDs is null)
        {
            throw new NullReferenceException(string.Format(Database.Prompts[Database.Prompt.CourseNullList], ID));
        }
        StudentIDs = studentIDs;
    }
}
