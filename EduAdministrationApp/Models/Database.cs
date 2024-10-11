using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace EduAdministrationApp.Models;

public static class Database
{
    private enum Prompt
    {
        NoTeachingStaff,
        EmptyDataset,
        NotEnoughTeachers,
        CourseNullList,
        TeacherNullCourseList,
        NullReference,
        CourseNotFound,
        CourseAlreadyExists,
        StaffNotFound,
        StudentNotFound,
        StudentAlreadyExists,
        TeacherNotFound,
        TeacherAlreadyExists,
        DepartmentHeadNotFound,
        DepartmentHeadAlreadyExists,
        AdministratorNotFound,
        AdministratorAlreadyExists,
        InvalidEntry,
        CourseID,
        CourseTitle,
        CourseStartDate,
        CourseLengthInWeeks,
        CourseType,
        CourseTeachers,
        CourseStudents,
        CourseCreated,
        TeacherID,
        DepartmentHeadID,
        AdministratorID,
        FirstName,
        LastName,
        PersonalNumber,
        AddressLine,
        PostalCode,
        City,
        Department,
        EmployedOn,
        StudentID,
        StudentAlreadyAssigned,
        StudentNotAssigned,
        TeacherAlreadyAssigned,
        TeacherNotAssigned,
        DepartmentHeadAlreadyAssigned,
        DepartmentHeadNotAssigned,
        AdministratorAlreadyAssigned,
        AdministratorNotAssigned,
    }

    private enum MenuOptionPrompt
    {
        ListCourses,
        ListStudents,
        ListTeachers,
        ListDepartmentHeads,
        ListAdministrators,
        AddCourse,
        RemoveCourse,
        AddStudent,
        RemoveStudent,
        AddTeacher,
        RemoveTeacher,
        AddDepartmentHead,
        RemoveDepartmentHead,
        AddAdministrator,
        RemoveAdministrator,
        AddStudentToCourse,
        RemoveStudentFromCourse,
        AddCourseToTeacher,
        RemoveCourseFromTeacher,
        AddCourseToDepartmentHead,
        RemoveCourseFromDepartmentHead,
        AddCourseToAdministrator,
        RemoveCourseFromAdministrator
    }

    private enum Filename
    {
        Staff,
        Courses,
        Students,
        Prompts,
        MenuPrompts
    }

    private enum StaffType
    {
        Teacher,
        DepartmentHead,
        Administrator
    }

    public enum EntityType
    {
        Student,
        Teacher
    }

    private static readonly string _dataPath = $"{Environment.CurrentDirectory}/data/";
    private static readonly string _filePath = _dataPath + "{0}.json";

    private static readonly Dictionary<Prompt, string> s_prompts;
    private static readonly Dictionary<MenuOptionPrompt, string> s_menuOptionPrompts;

    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        IncludeFields = true
    };

    private static readonly List<Teacher> s_staff;
    private static readonly List<Course> s_courses;
    private static readonly List<Student> s_students;
    public static readonly IReadOnlyList<Teacher> Staff;
    public static readonly IReadOnlyList<Course> Courses;
    public static readonly IReadOnlyList<Student> Students;
    public static MenuManager MenuManager { get; }

    static Database()
    {
        s_prompts = ReadObjectFromFile<Dictionary<Prompt, string>>(Filename.Prompts);
        s_menuOptionPrompts = ReadObjectFromFile<Dictionary<MenuOptionPrompt, string>>(Filename.MenuPrompts);
        s_staff = LoadFilelist<Teacher>(Filename.Staff);
        Staff = s_staff.AsReadOnly();
        s_courses = LoadFilelist<Course>(Filename.Courses);
        Courses = s_courses.AsReadOnly();
        s_students = LoadFilelist<Student>(Filename.Students);
        Students = s_students.AsReadOnly();
        MenuManager = CreateHardcodedMenu();
    }

    private static string GetFilePath(Filename filename)
    {
        return string.Format(_filePath, filename.ToString().ToLower());
    }

    private static void SaveObjectToFile<T>(Filename filename, T serializableObject)
    {
        File.WriteAllText(GetFilePath(filename), JsonSerializer.Serialize(serializableObject, s_jsonOptions));
    }

    private static T ReadObjectFromFile<T>(Filename filename)
    {
        T? tempObject = JsonSerializer.Deserialize<T>(File.ReadAllText(GetFilePath(filename)), s_jsonOptions) ??
         throw new NullReferenceException(string.Format(s_prompts[Prompt.NullReference], typeof(T)));
        return tempObject;
    }

    private static List<T> LoadFilelist<T>(Filename filename)
    {
        return File.Exists(GetFilePath(filename)) ? ReadObjectFromFile<List<T>>(filename) : [];
    }

    public static void SaveAllDataToFiles()
    {
        SaveObjectToFile(Filename.Staff, s_staff);
        SaveObjectToFile(Filename.Courses, s_courses);
        SaveObjectToFile(Filename.Students, s_students);
    }

    private static void DisplayData<T>(List<T> dataList) where T : IIdentifiable
    {
        if (dataList.Count > 0)
        {
            dataList.Sort((x, y) => x.ID.CompareTo(y.ID));
            foreach (T item in dataList)
            {
                Console.WriteLine(item);
            }
        }
        else
        {
            Console.WriteLine($"{s_prompts[Prompt.EmptyDataset]}");
        };
    }

    private static string? GetInput(Prompt prompt)
    {
        Console.WriteLine();
        Console.Write($"{s_prompts[prompt]}: ");
        return Console.ReadLine();
    }

    private static void AddCourse()
    {

        //ID
        int id;
        while (!int.TryParse(GetInput(Prompt.CourseID), out id))
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
        }
        if (s_courses.Any(course => course.ID == id))
        {
            Console.WriteLine(string.Format(s_prompts[Prompt.CourseAlreadyExists], id));
            return;
        }

        //Title
        string? title = GetInput(Prompt.CourseTitle);
        while (title is null)
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
            title = GetInput(Prompt.CourseTitle);
        }

        //Start date
        DateTime startDate;
        while (!DateTime.TryParse(GetInput(Prompt.CourseStartDate), out startDate))
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
        }

        //Length in weeks
        int weeks;
        while (!int.TryParse(GetInput(Prompt.CourseLengthInWeeks), out weeks))
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
        }

        //End date
        DateTime endDate = startDate.AddDays(weeks * 7);

        //Course type
        bool isDistanceBased = false;
        string? answer;
        var correctAnswers = new List<string> { "Y", "N" };
        while (true)
        {
            answer = GetInput(Prompt.CourseType);
            if (answer is null)
            {
                Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
                continue;
            }
            if (!correctAnswers.Contains(answer, StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
                continue;
            }
            if (answer == "Y" || answer == "y")
            {
                isDistanceBased = true;
            }
            break;
        }

        //Teacher(s)
        string[] words;
        bool valid;
        while (true)
        {
            valid = true;
            answer = GetInput(Prompt.CourseTeachers);
            if (answer is null)
            {
                continue;
            }
            if (answer == string.Empty)
            {
                break;
            }
            words = answer.Split(
                ',',
                StringSplitOptions.TrimEntries &
                StringSplitOptions.RemoveEmptyEntries
            );
            foreach (string word in words)
            {
                if (!int.TryParse(word, out int tempID))
                {
                    Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
                    valid = false;
                    break;
                }
                if (s_staff.FirstOrDefault(staffMember => staffMember.ID == tempID) is null)
                {
                    Console.WriteLine(s_prompts[Prompt.StaffNotFound]);
                    valid = false;
                    break;
                }
                s_staff.First(staffMember => staffMember.ID == tempID).CourseIDs.Add(id);
            }
            if (!valid)
            {
                continue;
            }
            break;
        }

        //Students
        List<int> studentIDs = [];
        while (true)
        {
            valid = true;
            answer = GetInput(Prompt.CourseStudents);
            if (answer is null)
            {
                continue;
            }
            if (answer == string.Empty)
            {
                break;
            }
            words = answer.Split(
                ',',
                StringSplitOptions.TrimEntries &
                StringSplitOptions.RemoveEmptyEntries
            );
            foreach (string word in words)
            {
                if (!int.TryParse(word, out int tempID))
                {
                    Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
                    valid = false;
                    break;
                }
                if (s_students.FirstOrDefault(student => student.ID == tempID) is null)
                {
                    Console.WriteLine(s_prompts[Prompt.StudentNotFound]);
                    valid = false;
                    break;
                }
                studentIDs.Add(tempID);
            }
            if (!valid)
            {
                continue;
            }
            break;
        }

        //Finally create course object
        s_courses.Add(
            new Course()
            {
                ID = id,
                Title = title,
                StartDate = startDate,
                LengthInWeeks = weeks,
                EndDate = endDate,
                IsDistanceBased = isDistanceBased,
                StudentIDs = studentIDs
            }
        );

        Console.Clear();
    }

    private static void RemoveCourse()
    {
        while (true)
        {
            if (!int.TryParse(GetInput(Prompt.CourseID), out int courseID))
            {
                Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
                continue;
            }
            Course? courseToRemove = s_courses.FirstOrDefault(course => course.ID == courseID);
            if (courseToRemove is null)
            {
                Console.WriteLine(s_prompts[Prompt.CourseNotFound]);
                return;
            }
            s_courses.Remove(courseToRemove);
            foreach (Teacher relatedTeacher in s_staff.FindAll(staffMember => staffMember.CourseIDs.Contains(courseID)))
            {
                relatedTeacher.CourseIDs.Remove(courseID);
            }
            break;
        }

        Console.Clear();
    }

    private static void AddStaff(StaffType staffType)
    {

        Prompt idPrompt;
        Prompt alreadyExistsPrompt;
        if (staffType is StaffType.Teacher)
        {
            idPrompt = Prompt.TeacherID;
            alreadyExistsPrompt = Prompt.TeacherAlreadyExists;
        }
        else if (staffType is StaffType.DepartmentHead)
        {
            idPrompt = Prompt.DepartmentHeadID;
            alreadyExistsPrompt = Prompt.DepartmentHeadAlreadyExists;
        }
        else
        {
            idPrompt = Prompt.AdministratorID;
            alreadyExistsPrompt = Prompt.AdministratorAlreadyExists;
        }

        //ID
        int id;
        while (!int.TryParse(GetInput(idPrompt), out id))
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
        }
        if (s_staff.Any(staffMember => staffMember.ID == id))
        {
            Console.WriteLine(string.Format(s_prompts[alreadyExistsPrompt], id));
            return;
        }

        /*** Contact details ***/

        //First name
        string? firstName = GetInput(Prompt.FirstName);
        while (firstName is null)
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
            firstName = GetInput(Prompt.FirstName);
        }

        //Last name
        string? lastName = GetInput(Prompt.LastName);
        while (lastName is null)
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
            lastName = GetInput(Prompt.LastName);
        }

        //First name
        string? personalNumber = GetInput(Prompt.PersonalNumber);
        while (personalNumber is null)
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
            personalNumber = GetInput(Prompt.PersonalNumber);
        }

        //Address line
        string? addressLine = GetInput(Prompt.AddressLine);
        while (addressLine is null)
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
            addressLine = GetInput(Prompt.AddressLine);
        }

        //Postal code
        string? postalCode = GetInput(Prompt.PostalCode);
        while (postalCode is null)
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
            postalCode = GetInput(Prompt.PostalCode);
        }

        //City
        string? city = GetInput(Prompt.City);
        while (city is null)
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
            city = GetInput(Prompt.City);
        }
        /*******************/

        //Department
        string? department = GetInput(Prompt.Department);
        while (department is null)
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
            department = GetInput(Prompt.Department);
        }

        /*** Create staff object ***/
        //Create common contact details object
        ContactDetails contactDetails = new()
        {
            FirstName = firstName,
            LastName = lastName,
            PersonalNumber = personalNumber,
            AddressLine = addressLine,
            PostalCode = postalCode,
            City = city
        };

        //Add to database
        if (staffType is StaffType.Teacher)
        {
            s_staff.Add(
                new Teacher()
                {
                    ID = id,
                    ContactDetails = contactDetails,
                    Department = department,
                    CourseIDs = []
                }
            );
        }
        else
        {
            //Employment date
            DateTime employedOn;
            while (!DateTime.TryParse(GetInput(Prompt.EmployedOn), out employedOn))
            {
                Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
            }

            if (staffType is StaffType.DepartmentHead)
            {
                s_staff.Add(
                    new DepartmentHead()
                    {
                        ID = id,
                        ContactDetails = contactDetails,
                        Department = department,
                        EmployedOn = employedOn,
                        CourseIDs = []
                    }
                );
            }
            else
            {
                s_staff.Add(
                    new Administrator()
                    {
                        ID = id,
                        ContactDetails = contactDetails,
                        Department = department,
                        EmployedOn = employedOn,
                        CourseIDs = []
                    }
                );
            }
        }

        Console.Clear();
    }

    private static void RemoveStaff(StaffType staffType)
    {
        Prompt idPrompt;
        Prompt notFoundPrompt;
        if (staffType is StaffType.Teacher)
        {
            idPrompt = Prompt.TeacherID;
            notFoundPrompt = Prompt.TeacherNotFound;
        }
        else if (staffType is StaffType.DepartmentHead)
        {
            idPrompt = Prompt.DepartmentHeadID;
            notFoundPrompt = Prompt.DepartmentHeadNotFound;
        }
        else
        {
            idPrompt = Prompt.AdministratorID;
            notFoundPrompt = Prompt.AdministratorNotFound;
        }

        while (true)
        {
            if (!int.TryParse(GetInput(idPrompt), out int id))
            {
                Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
                continue;
            }
            Teacher? staffMemberToRemove = s_staff.FirstOrDefault(staffMember => staffMember.ID == id);
            if (staffMemberToRemove is null)
            {
                Console.WriteLine(s_prompts[notFoundPrompt]);
                return;
            }
            s_staff.Remove(staffMemberToRemove);
            break;
        }

        Console.Clear();
    }

    private static void AddStudent()
    {
        //ID
        int id;
        while (!int.TryParse(GetInput(Prompt.StudentID), out id))
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
        }
        if (s_students.Any(student => student.ID == id))
        {
            Console.WriteLine(string.Format(s_prompts[Prompt.StudentAlreadyExists], id));
            return;
        }

        /*** Contact details ***/

        //First name
        string? firstName = GetInput(Prompt.FirstName);
        while (firstName is null)
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
            firstName = GetInput(Prompt.FirstName);
        }

        //Last name
        string? lastName = GetInput(Prompt.LastName);
        while (lastName is null)
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
            lastName = GetInput(Prompt.LastName);
        }

        //First name
        string? personalNumber = GetInput(Prompt.PersonalNumber);
        while (personalNumber is null)
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
            personalNumber = GetInput(Prompt.PersonalNumber);
        }

        //Address line
        string? addressLine = GetInput(Prompt.AddressLine);
        while (addressLine is null)
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
            addressLine = GetInput(Prompt.AddressLine);
        }

        //Postal code
        string? postalCode = GetInput(Prompt.PostalCode);
        while (postalCode is null)
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
            postalCode = GetInput(Prompt.PostalCode);
        }

        //City
        string? city = GetInput(Prompt.City);
        while (city is null)
        {
            Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
            city = GetInput(Prompt.City);
        }
        /*******************/

        s_students.Add(
            new Student()
            {
                ID = id,
                ContactDetails = new()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    PersonalNumber = personalNumber,
                    AddressLine = addressLine,
                    PostalCode = postalCode,
                    City = city
                }
            }
        );

        Console.Clear();
    }

    private static void RemoveStudent()
    {
        while (true)
        {
            if (!int.TryParse(GetInput(Prompt.StudentID), out int studentID))
            {
                Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
                continue;
            }
            Student? studentToRemove = s_students.FirstOrDefault(student => student.ID == studentID);
            if (studentToRemove is null)
            {
                Console.WriteLine(s_prompts[Prompt.StudentNotFound]);
                return;
            }
            s_students.Remove(studentToRemove);
            foreach (Course relatedCourse in s_courses.FindAll(course => course.StudentIDs.Contains(studentID)))
            {
                relatedCourse.StudentIDs.Remove(studentID);
            }
            break;
        }

        Console.Clear();
    }

    private static void AddStudentToCourse()
    {
        while (true)
        {
            if (!int.TryParse(GetInput(Prompt.CourseID), out int courseID))
            {
                Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
                continue;
            }
            Course? courseToAddTo = s_courses.FirstOrDefault(course => course.ID == courseID);
            if (courseToAddTo is null)
            {
                Console.WriteLine(s_prompts[Prompt.CourseNotFound]);
                return;
            }
            if (!int.TryParse(GetInput(Prompt.StudentID), out int studentID))
            {
                Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
                continue;
            }
            Student? studentToAdd = s_students.FirstOrDefault(student => student.ID == studentID);
            if (studentToAdd is null)
            {
                Console.WriteLine(s_prompts[Prompt.StudentNotFound]);
                return;
            }
            if (courseToAddTo.StudentIDs.Contains(studentID))
            {
                Console.WriteLine(s_prompts[Prompt.StudentAlreadyAssigned]);
                return;
            }
            courseToAddTo.StudentIDs.Add(studentID);
            break;
        }

        Console.Clear();
    }

    private static void RemoveStudentFromCourse()
    {
        while (true)
        {
            if (!int.TryParse(GetInput(Prompt.CourseID), out int courseID))
            {
                Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
                continue;
            }
            Course? courseToRemoveFrom = s_courses.FirstOrDefault(course => course.ID == courseID);
            if (courseToRemoveFrom is null)
            {
                Console.WriteLine(s_prompts[Prompt.CourseNotFound]);
                return;
            }
            if (!int.TryParse(GetInput(Prompt.StudentID), out int studentID))
            {
                Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
                continue;
            }
            Student? studentToRemove = s_students.FirstOrDefault(student => student.ID == studentID);
            if (studentToRemove is null)
            {
                Console.WriteLine(s_prompts[Prompt.StudentNotFound]);
                return;
            }
            if (!courseToRemoveFrom.StudentIDs.Contains(studentID))
            {
                Console.WriteLine(s_prompts[Prompt.StudentNotAssigned]);
                return;
            }
            courseToRemoveFrom.StudentIDs.Remove(studentID);
            break;
        }

        Console.Clear();
    }

    private static void AddCourseToStaff(StaffType staffType)
    {
        Prompt staffIDPrompt;
        Prompt staffNotFoundPrompt;
        Prompt staffAlreadyAssignedPrompt;
        if (staffType is StaffType.Teacher)
        {
            staffIDPrompt = Prompt.TeacherID;
            staffNotFoundPrompt = Prompt.TeacherNotFound;
            staffAlreadyAssignedPrompt = Prompt.TeacherAlreadyAssigned;
        }
        else if (staffType is StaffType.DepartmentHead)
        {
            staffIDPrompt = Prompt.DepartmentHeadID;
            staffNotFoundPrompt = Prompt.DepartmentHeadNotFound;
            staffAlreadyAssignedPrompt = Prompt.DepartmentHeadAlreadyAssigned;
        }
        else
        {
            staffIDPrompt = Prompt.AdministratorID;
            staffNotFoundPrompt = Prompt.AdministratorNotFound;
            staffAlreadyAssignedPrompt = Prompt.AdministratorAlreadyAssigned;
        }

        while (true)
        {
            if (!int.TryParse(GetInput(staffIDPrompt), out int staffID))
            {
                Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
                continue;
            }
            Teacher? staffToAddTo = s_staff.FirstOrDefault(staffMember => staffMember.ID == staffID);
            if (staffToAddTo is null)
            {
                Console.WriteLine(s_prompts[staffNotFoundPrompt]);
                return;
            }
            if (!int.TryParse(GetInput(Prompt.CourseID), out int courseID))
            {
                Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
                continue;
            }
            Course? courseToAdd = s_courses.FirstOrDefault(course => course.ID == courseID);
            if (courseToAdd is null)
            {
                Console.WriteLine(s_prompts[Prompt.CourseNotFound]);
                return;
            }
            if (staffToAddTo.CourseIDs.Contains(courseID))
            {
                Console.WriteLine(s_prompts[staffAlreadyAssignedPrompt]);
                return;
            }
            staffToAddTo.CourseIDs.Add(courseID);
            break;
        }

        Console.Clear();
    }

    private static void RemoveCourseFromStaff(StaffType staffType)
    {
        Prompt staffIDPrompt;
        Prompt staffNotFoundPrompt;
        Prompt staffNotAssignedPrompt;
        if (staffType is StaffType.Teacher)
        {
            staffIDPrompt = Prompt.TeacherID;
            staffNotFoundPrompt = Prompt.TeacherNotFound;
            staffNotAssignedPrompt = Prompt.TeacherNotAssigned;
        }
        else if (staffType is StaffType.DepartmentHead)
        {
            staffIDPrompt = Prompt.DepartmentHeadID;
            staffNotFoundPrompt = Prompt.DepartmentHeadNotFound;
            staffNotAssignedPrompt = Prompt.DepartmentHeadNotAssigned;
        }
        else
        {
            staffIDPrompt = Prompt.AdministratorID;
            staffNotFoundPrompt = Prompt.AdministratorNotFound;
            staffNotAssignedPrompt = Prompt.AdministratorNotAssigned;
        }

        while (true)
        {
            if (!int.TryParse(GetInput(staffIDPrompt), out int staffID))
            {
                Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
                continue;
            }
            Teacher? staffToRemoveFrom = s_staff.FirstOrDefault(staffMember => staffMember.ID == staffID);
            if (staffToRemoveFrom is null)
            {
                Console.WriteLine(s_prompts[staffNotFoundPrompt]);
                return;
            }
            if (!int.TryParse(GetInput(Prompt.CourseID), out int courseID))
            {
                Console.WriteLine(s_prompts[Prompt.InvalidEntry]);
                continue;
            }
            Course? courseToRemove = s_courses.FirstOrDefault(course => course.ID == courseID);
            if (courseToRemove is null)
            {
                Console.WriteLine(s_prompts[Prompt.CourseNotFound]);
                return;
            }
            if (!staffToRemoveFrom.CourseIDs.Contains(courseID))
            {
                Console.WriteLine(s_prompts[staffNotAssignedPrompt]);
                return;
            }
            staffToRemoveFrom.CourseIDs.Remove(courseID);
            break;
        }

        Console.Clear();
    }
    private static MenuManager CreateHardcodedMenu()
    {
        var courseMenu = new Menu(
            () =>
            {
                DisplayData(s_courses);
            },
            [
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.AddCourse],
                    AddCourse
                ),
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.RemoveCourse],
                    RemoveCourse
                ),
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.AddStudentToCourse],
                    AddStudentToCourse
                ),
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.RemoveStudentFromCourse],
                    RemoveStudentFromCourse
                )
            ]
        );

        var studentMenu = new Menu(
            () =>
            {
                DisplayData(s_students);
            },
            [
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.AddStudent],
                    AddStudent
                ),
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.RemoveStudent],
                    RemoveStudent
                )
            ]
        );

        var teacherMenu = new Menu(
            () =>
            {
                DisplayData(s_staff.OfType<Teacher>().ToList());
            },
            [
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.AddTeacher],
                    () =>
                    {
                        AddStaff(StaffType.Teacher);
                    }
                ),
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.RemoveTeacher],
                    () =>
                    {
                        RemoveStaff(StaffType.Teacher);
                    }
                ),
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.AddCourseToTeacher],
                    () =>
                    {
                        AddCourseToStaff(StaffType.Teacher);
                    }
                ),
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.RemoveCourseFromTeacher],
                    () =>
                    {
                        RemoveCourseFromStaff(StaffType.Teacher);
                    }
                )
            ]
        );

        var departmentHeadMenu = new Menu(
            () =>
            {
                DisplayData(s_staff.OfType<DepartmentHead>().ToList());
            },
            [
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.AddDepartmentHead],
                    () =>
                    {
                        AddStaff(StaffType.DepartmentHead);
                    }
                ),
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.RemoveDepartmentHead],
                    () =>
                    {
                        RemoveStaff(StaffType.DepartmentHead);
                    }
                ),
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.AddCourseToDepartmentHead],
                    () =>
                    {
                        AddCourseToStaff(StaffType.DepartmentHead);
                    }
                ),
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.RemoveCourseFromDepartmentHead],
                    () =>
                    {
                        RemoveCourseFromStaff(StaffType.DepartmentHead);
                    }
                )
            ]
        );

        var administratorMenu = new Menu(
            () =>
            {
                DisplayData(s_staff.OfType<Administrator>().ToList());
            },
            [
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.AddAdministrator],
                    () =>
                    {
                        AddStaff(StaffType.Administrator);
                    }
                ),
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.RemoveAdministrator],
                    () =>
                    {
                        RemoveStaff(StaffType.Administrator);
                    }
                ),
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.AddCourseToAdministrator],
                    () =>
                    {
                        AddCourseToStaff(StaffType.Administrator);
                    }
                ),
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.RemoveCourseFromAdministrator],
                    () =>
                    {
                        RemoveCourseFromStaff(StaffType.Administrator);
                    }
                )
            ]
        );

        var mainMenu = new Menu(
            null,
            [
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.ListCourses],
                    courseMenu.Run
                ),
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.ListStudents],
                    studentMenu.Run
                ),
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.ListTeachers],
                    teacherMenu.Run
                ),
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.ListDepartmentHeads],
                    departmentHeadMenu.Run
                ),
                new MenuItem(
                    s_menuOptionPrompts[MenuOptionPrompt.ListAdministrators],
                    administratorMenu.Run
                )
            ]
        );

        return new MenuManager(
            [
                mainMenu,
                courseMenu,
                studentMenu,
                teacherMenu,
                departmentHeadMenu,
                administratorMenu
            ]
        );
    }

}
