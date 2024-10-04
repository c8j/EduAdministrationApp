namespace EduAdministrationApp.Models;

public static class Database
{

    public enum Prompt
    {
        NotEnoughTeachers,
        CourseNullList
    }
    public static readonly Dictionary<Prompt, string> Prompts = new()
    {
        {Prompt.NotEnoughTeachers, "Kursen {0} behöver minst en lärare."}, //0 - course ID
        {Prompt.CourseNullList, "Kursen {0} fick en noll lista. Kontrollera JSON-data."} //0 - course ID
    };
}
