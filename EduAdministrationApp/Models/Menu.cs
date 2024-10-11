namespace EduAdministrationApp.Models;

public class Menu(Action? dataAction, List<MenuItem> menuItems)
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
        {Prompt.Exit, "Avsluta programmet och spara data"},
        {Prompt.InvalidOption, "Vänligen infoga ett giltigt alternativ."},
    };

    private readonly List<MenuItem> _menuItems = menuItems;
    private readonly Action? _dataAction = dataAction;
    private static int s_depth = -1;

    private void DisplayMenu()
    {
        if (_dataAction is not null)
        {
            _dataAction();
        }

        Console.WriteLine(new string('-', 100));
        for (int i = 0; i < _menuItems.Count; i++)
        {
            Console.WriteLine($"{i + 1}: {_menuItems[i].Text}");
        }
        Console.WriteLine(
            $"{_menuItems.Count + 1}: {(s_depth == 0 ? Prompts[Prompt.Exit] : Prompts[Prompt.Return])}");
        Console.WriteLine(new string('-', 100));
    }

    private uint GetInput()
    {
        DisplayMenu();
        Console.Write(Prompts[Prompt.Input]);
        uint choice = uint.TryParse(Console.ReadLine(), out choice) ? choice : 0;
        while (choice < 1 || choice > _menuItems.Count + 1)
        {
            Console.Clear();
            DisplayMenu();
            Console.WriteLine(
                $"{Prompts[Prompt.InvalidOption]}");
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
            uint choice = GetInput();
            Console.Clear();
            if (choice == _menuItems.Count + 1)
            {
                break;
            }
            else
            {
                _menuItems[(int)choice - 1].Action();
            }
        }
        s_depth--;
    }

}
