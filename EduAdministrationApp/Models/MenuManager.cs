namespace EduAdministrationApp.Models;

public static class MenuManager
{
    public static readonly IReadOnlyList<Menu> Menus;
    private static readonly List<Menu> s_menus;

    static MenuManager()
    {
        if (File.Exists(Database.MenuFilePath()))
        {
            s_menus = Database.LoadMenu();
        }
        else
        {
            s_menus = CreateHardcodedMenu();
        }
        Menus = s_menus.AsReadOnly();
    }

    public static void Run()
    {
        s_menus[0].Run();
    }

    private static void DisplayData<T>(IReadOnlyList<T> dataList)
    {
        foreach (T item in dataList)
        {
            Console.WriteLine(item);
        }
    }

    /*  private static Course CreateCourse()
     {

     } */

    private static List<Menu> CreateHardcodedMenu()
    {
        //TODO: FINISH THIS
        var mainMenu = new Menu();
        var courseMenu = new Menu();

        courseMenu.MenuItems.Add(
            new MenuItem(
                "Add course",
                () =>
                {

                }
            )
        );

        mainMenu.MenuItems.Add(
            new MenuItem(
                "List all courses",
                () =>
                {
                    DisplayData(Database.Courses);
                }
            )
        );

        mainMenu.MenuItems.Add(
            new MenuItem(
                "List all students",
                () =>
                {
                    DisplayData(Database.Students);
                }
            )
        );
        var menuList = new List<Menu>();
        menuList.Add(mainMenu);
        menuList.Add(courseMenu);
        return menuList;
    }
}
