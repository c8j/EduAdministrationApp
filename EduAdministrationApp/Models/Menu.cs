namespace EduAdministrationApp.Models;

public class Menu()
{
    enum Prompt
    {
        Input,
        Return,
        Exit,
        InvalidOption
    }

    private static readonly Dictionary<Prompt, string> Prompts = new()
    {
        {Prompt.Input, "Välj alternativ: "},
        {Prompt.Return, "Gå tillbaka"},
        {Prompt.Exit, "Avsluta programmet"},
        {Prompt.InvalidOption, "Vänligen ange ett giltigt alternativ."},
    };

    public readonly List<MenuItem> MenuItems = [];
    private static int s_depth = -1;

    public void Display()
    {
        Console.Clear();
        for (int i = 0; i < MenuItems.Count; i++)
        {
            Console.WriteLine($"{i + 1}: {MenuItems[i].Text}");
        }
        Console.WriteLine(
            $"{MenuItems.Count + 1}: {(s_depth == 0 ? Prompts[Prompt.Exit] : Prompts[Prompt.Return])}");
        Console.WriteLine(new string('-', 100));
    }

    private uint GetInput()
    {
        Console.Write(Prompts[Prompt.Input]);
        uint choice = uint.TryParse(Console.ReadLine(), out choice) ? choice : 0;
        while (choice < 1 || choice > MenuItems.Count + 1)
        {
            Console.WriteLine(
                $"{Environment.NewLine}{Prompts[Prompt.InvalidOption]}{Environment.NewLine}");
            Console.Write(Prompts[Prompt.Input]);
            choice = uint.TryParse(Console.ReadLine(), out choice) ? choice : 0;
        }
        return choice;
    }

    public void Run()
    {
        s_depth++;
        while (true)
        {
            Display();
            uint choice = GetInput();
            if (choice == MenuItems.Count + 1)
            {
                break;
            }
            else
            {
                MenuItems[(int)choice - 1].Action();
            }
        }
        s_depth--;
    }
}
