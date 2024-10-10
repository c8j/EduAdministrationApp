using EduAdministrationApp.Models;

MenuManager menuManager = Database.MenuManager;
menuManager.Run();
Database.SaveAllDataToFiles();
