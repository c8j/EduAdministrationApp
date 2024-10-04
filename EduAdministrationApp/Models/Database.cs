using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace EduAdministrationApp.Models;

public static class Database
{

    public enum Prompt
    {
        NotEnoughTeachers,
        CourseNullList,
        NullReference
    }
    public static readonly Dictionary<Prompt, string> Prompts = new()
    {
        {Prompt.NotEnoughTeachers, "Kursen {0} behöver minst en lärare."}, //0 - course ID
        {Prompt.CourseNullList, "Kursen {0} fick en noll lista. Kontrollera JSON-data."}, //0 - course ID
        {Prompt.NullReference, "Fick noll referens när försökte läsa JSON-objekt."}
    };

    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    public static void SaveObjectToFile<T>(string path, T serializableObject)
    {
        File.WriteAllText(path, JsonSerializer.Serialize(serializableObject, s_jsonOptions));
    }

    public static T ReadObjectFromFile<T>(string path)
    {
        T? tempObject = JsonSerializer.Deserialize<T>(File.ReadAllText(path), s_jsonOptions) ??
         throw new NullReferenceException(Prompts[Prompt.NullReference]);
        return tempObject;
    }
}
