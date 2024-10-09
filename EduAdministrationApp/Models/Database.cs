using System.Data;
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
        TeacherNullCourseList,
        NullReference,
        CourseNotFound,
        CourseAlreadyExists,
        StaffNotFound,
        StaffAlreadyExists,
        StudentNotFound,
        StudentAlreadyExists
    }

    private enum Filename
    {
        Staff,
        Courses,
        Students
    }

    private static readonly string _dataPath = $"{Environment.CurrentDirectory}/data/";
    private static readonly string _filePath = _dataPath + "{0}.json";

    public static readonly Dictionary<Prompt, string> Prompts = new()
    {
        {Prompt.NotEnoughTeachers, "Kursen {0} behöver minst en lärare."}, //0 - course ID
        {Prompt.CourseNullList, "Kursen {0} fick en noll lista. Kontrollera JSON-data."}, //0 - course ID
        {Prompt.TeacherNullCourseList, "Läraren {0} fick en noll kurslista. Kontrollera JSON-data."}, //0 - teacher ID
        {Prompt.NullReference, "Fick noll referens när försökte läsa JSON-objekt för {0}. Kontrollera JSON-data."}, //0 - object type
        {Prompt.CourseNotFound, "Kunde inte hitta någon kurs med angivet ID."},
        {Prompt.CourseAlreadyExists, "Där redan finns en kurs med samma ID ({0})."}, //0 - course ID
        {Prompt.StaffNotFound, "Kunde inte hitta någon personal med angivet ID."},
        {Prompt.StaffAlreadyExists, "Där redan finns en anställd med samma ID ({0})."}, //0 - staff ID
        {Prompt.StudentNotFound, "Kunde inte hitta någon student med angivet ID."},
        {Prompt.StudentAlreadyExists, "Där redan finns en student med samma ID ({0})."}, //0 - student ID
    };

    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    private static readonly List<Teacher> s_staff;
    public static readonly IReadOnlyList<Teacher> Staff;
    private static readonly List<Course> s_courses;
    public static readonly IReadOnlyList<Course> Courses;
    private static readonly List<Student> s_students;
    public static readonly IReadOnlyList<Student> Students;

    static Database()
    {
        s_staff = File.Exists(GetFilePath(Filename.Staff)) ? ReadObjectFromFile<List<Teacher>>(GetFilePath(Filename.Staff)) : [];
        Staff = s_staff.AsReadOnly();

        s_courses = File.Exists(GetFilePath(Filename.Courses)) ? ReadObjectFromFile<List<Course>>(GetFilePath(Filename.Courses)) : [];
        Courses = s_courses.AsReadOnly();

        s_students = File.Exists(GetFilePath(Filename.Students)) ? ReadObjectFromFile<List<Student>>(GetFilePath(Filename.Students)) : [];
        Students = s_students.AsReadOnly();
    }

    private static string GetFilePath(Filename filename)
    {
        return string.Format(_filePath, filename.ToString().ToLower());
    }

    private static void SaveObjectToFile<T>(string path, T serializableObject)
    {
        File.WriteAllText(path, JsonSerializer.Serialize(serializableObject, s_jsonOptions));
    }

    private static T ReadObjectFromFile<T>(string path)
    {
        T? tempObject = JsonSerializer.Deserialize<T>(File.ReadAllText(path), s_jsonOptions) ??
         throw new NullReferenceException(string.Format(Prompts[Prompt.NullReference], typeof(T)));
        return tempObject;
    }

    public static void SaveDataToFile()
    {
        SaveObjectToFile(GetFilePath(Filename.Staff), s_staff);
        SaveObjectToFile(GetFilePath(Filename.Courses), s_courses);
        SaveObjectToFile(GetFilePath(Filename.Students), s_students);
    }

    private static void AddItem<T>(T item, int itemID, List<T> list, Prompt duplicateExceptionPrompt)
    {
        if (list.Contains(item))
        {
            throw new DuplicateNameException(string.Format(Prompts[duplicateExceptionPrompt], itemID));
        }
        list.Add(item);
    }

    public static void AddStaff(Teacher staff)
    {
        AddItem(staff, staff.ID, s_staff, Prompt.StaffAlreadyExists);
    }

    public static void AddCourse(Course course)
    {
        AddItem(course, course.ID, s_courses, Prompt.CourseAlreadyExists);
    }

    public static void AddStudent(Student student)
    {
        AddItem(student, student.ID, s_students, Prompt.StudentAlreadyExists);
    }

    public static void RemoveCourse(int courseID)
    {
        Course courseToRemove = s_courses.FirstOrDefault(course => course.ID == courseID) ??
         throw new NullReferenceException(Prompts[Prompt.CourseNotFound]);
        s_courses.Remove(courseToRemove);
    }

    public static void RemoveStaff(int staffID)
    {
        Teacher staffToRemove = s_staff.FirstOrDefault(staff => staff.ID == staffID) ??
         throw new NullReferenceException(Prompts[Prompt.StaffNotFound]);
        s_staff.Remove(staffToRemove);
    }

    public static void RemoveStudent(int studentID)
    {
        Student studentToRemove = s_students.FirstOrDefault(student => student.ID == studentID) ??
         throw new NullReferenceException(Prompts[Prompt.StudentNotFound]);
        s_students.Remove(studentToRemove);
    }

}
