namespace EduAdministrationApp.Models;

public class Teacher : IIdentifiable, IContactable
{
    public required int ID { get; init; }
    public required ContactDetails ContactDetails { get; init; }
    public required string Department { get; init; }

    private readonly List<int> _courseIDs = [];
    public IReadOnlyList<int> CourseIDs { get; }

    public Teacher()
    {
        CourseIDs = _courseIDs.AsReadOnly();
    }

    public Teacher(List<int> courseIDs) : this()
    {
        _courseIDs.AddRange(courseIDs);
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

    private string GetCourseNames()
    {
        StringWriter sw = new();
        foreach (int id in _courseIDs)
        {
            Course course = Database.Courses.First(course => course.ID == id);
            sw.Write($"{id} - {course.Title}, ");
        }
        return sw.ToString().TrimEnd(',');
    }

    public override string ToString()
    {
        return
            $"ID: {ID}, Utbildningsområde: {Department}, {ContactDetails}" +
            $"{(_courseIDs.Count > 0 ? "{Environment.NewLine} > Kurser: [{GetCourseNames()}]" : "")}";
    }
}
