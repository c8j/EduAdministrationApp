using System.Text.Json.Serialization;

namespace EduAdministrationApp.Models;

public class Course
{
    public required int ID { get; init; }
    public required string Title { get; init; }
    public required int LengthInWeeks { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required bool IsDistanceBased { get; init; }

    private readonly List<int> _studentIDs;
    public IReadOnlyList<int> StudentIDs { get; }
    // private readonly List<int> _teacherIDs;
    // public IReadOnlyList<int> TeacherIDs { get; }

    [JsonConstructor]
    public Course(List<int> studentIDs)
    {
        // if (teacherIDs is null || teacherIDs.Count <= 0)
        // {
        //     throw new Exception(string.Format(Database.Prompts[Database.Prompt.NotEnoughTeachers], ID));
        // }
        // _teacherIDs = teacherIDs;
        // TeacherIDs = _teacherIDs.AsReadOnly();

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

    // public void AddTeacher(int teacherID)
    // {
    //     //TODO: validation
    //     _teacherIDs.Add(teacherID);
    // }

    // public void RemoveTeacher(int teacherID)
    // {
    //     //TODO: validation
    //     _teacherIDs.Remove(teacherID);
    // }
}
