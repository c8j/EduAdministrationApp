namespace EduAdministrationApp.Models;

public class Teacher : IIdentifiable
{
    public required int ID { get; init; }
    public required ContactDetails ContactDetails { get; init; }
    public required string Department { get; init; }

    private readonly List<int> _courseIDs;
    public IReadOnlyList<int> CourseIDs { get; }

    public Teacher(List<int> courseIDs)
    {
        _courseIDs = courseIDs ??
         throw new Exception(string.Format(Database.Prompts[Database.Prompt.TeacherNullCourseList], ID));
        CourseIDs = _courseIDs.AsReadOnly();
    }

    public void AddCourse(int courseID)
    {
        //TODO: validation
        _courseIDs.Add(courseID);
    }

    public void RemoveCourse(int courseID)
    {
        //TODO: validation
        _courseIDs.Remove(courseID);
    }
}
