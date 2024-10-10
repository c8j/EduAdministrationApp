namespace EduAdministrationApp.Models;

public class MenuManager
{
    public readonly IReadOnlyList<Menu> Menus;
    private readonly List<Menu> _menus;

    public MenuManager(List<Menu> menus)
    {
        _menus = menus;
        Menus = _menus.AsReadOnly();
    }

    public void Run()
    {
        _menus[0].Run();
    }
}
