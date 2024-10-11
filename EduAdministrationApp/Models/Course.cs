namespace EduAdministrationApp.Models;

public class Course : IIdentifiable
{

    public required int ID { get; init; }
    public required string Title { get; init; }
    public required int LengthInWeeks { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required bool IsDistanceBased { get; init; }
    public required List<int> StudentIDs { get; init; }

    private string GetStudentNames()
    {
        StringWriter sw = new();
        foreach (int id in StudentIDs)
        {
            Student student = Database.Students.First(student => student.ID == id);
            sw.Write($"{id} - {student.ContactDetails.FirstName} {student.ContactDetails.LastName}, ");
        }
        return sw.ToString().TrimEnd(',');
    }

    public override string ToString()
    {
        return
            $"ID: {ID}, {Title} - Längd: {LengthInWeeks} {(LengthInWeeks > 1 ? "veckor" : "vecka")}, " +
            $"{StartDate:yyyy-MM-dd} : {EndDate:yyyy-MM-dd}, Distans: {(IsDistanceBased ? "Ja" : "Nej")}" +
            $"{(StudentIDs.Count > 0 ? $"{Environment.NewLine} > Studenter: [{GetStudentNames()}]" : "")}";
    }
}
