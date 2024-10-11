using System.Text.Json.Serialization;

namespace EduAdministrationApp.Models;

[JsonDerivedType(typeof(DepartmentHead), typeDiscriminator: "depHead")]
[JsonDerivedType(typeof(Administrator), typeDiscriminator: "admin")]
public class Teacher : IIdentifiable, IContactable
{
    public required int ID { get; init; }
    public required ContactDetails ContactDetails { get; init; }
    public required string Department { get; init; }
    public required List<int> CourseIDs { get; init; }

    private string GetCourseNames()
    {
        StringWriter sw = new();
        foreach (int id in CourseIDs)
        {
            Course course = Database.Courses.First(course => course.ID == id);
            sw.Write($"{id} - {course.Title}, ");
        }
        return sw.ToString().TrimEnd().TrimEnd(',');
    }

    public override string ToString()
    {
        return
            $"{(CourseIDs.Count > 0 ? $" ↓ Kurser: [{GetCourseNames()}]{Environment.NewLine}" : "")}" +
            $"ID: {ID}, Utbildningsområde: {Department}, {ContactDetails}";
    }
}
