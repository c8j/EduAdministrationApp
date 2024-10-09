namespace EduAdministrationApp.Models;

public class MenuItem(string prompt, Action action)
{
    public string Text { get; } = prompt;
    public Action Action { get; } = action;
}
