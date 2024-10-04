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

    public Course(List<int> teacherIDs, List<int> studentIDs)
    {
        if (teacherIDs is null || teacherIDs.Count <= 0)
        {
            throw new Exception($"Kursen {ID} behöver minst en lärare.");
        }

        TeacherIDs = teacherIDs;
        if (studentIDs is null)
        {
            throw new NullReferenceException($"Kursen {ID} fick en noll lista. Kontrollera JSON-data.");
        }
        StudentIDs = studentIDs;
    }
}
