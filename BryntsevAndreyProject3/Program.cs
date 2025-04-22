/// ФИО: Брынцев Андрей Вячеславович.
/// Группа: БПИ246 (1).
/// Вариант: 4.

using ConsoleMenuLibrary;
using Project3;
using System.Text;
using VisitorLibrary;

/// <summary>
/// Класс программы.
/// </summary>
public class Program
{
    /// <summary>
    /// Точка входа в программу.
    /// </summary>
    public static void Main()
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;
        ConsoleMenu menu = InitConsoleMenu();
        ListOfVisitors visitors = new();
        string selectedItem = "";
        do
        {
            selectedItem = menu.GetItemFromUser();
            CommandsHandler.HandleConsoleCommand(selectedItem, ref visitors);
            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
        } while (selectedItem != "Выход");
    }

    /// <summary>
    /// Инициализирует консольное меню.
    /// </summary>
    /// <returns>Консольное меню.</returns>
    private static ConsoleMenu InitConsoleMenu()
    {
        Menu mainMenu = new(new List<MenuItem>
        {
            new MenuItem("Ввести данные (консоль/файл)", new Menu(new List<MenuItem>{
                new MenuItem("Ввести данные через консоль"),
                new MenuItem("Прочитать данные из файла"),
                new MenuItem("Назад")
            })),
            new MenuItem("Отфильтровать данные"),
            new MenuItem("Отсортировать данные"),
            new MenuItem("Отобразить ключевую информацию о посетителе"),
            new MenuItem("Конвертировать данные в/из Excel", new Menu(new List<MenuItem>
            {
                new MenuItem("Прочитать данные из Excel-таблицы"),
                new MenuItem("Записать данные в Excel-таблицу"),
                new MenuItem("Назад")
            })),
            new MenuItem("Вывести данные (консоль/файл)", new Menu(new List<MenuItem>
            {
                new MenuItem("Вывести данные в консоль"),
                new MenuItem("Записать данные в файл"),
                new MenuItem("Назад")
            })),
            new MenuItem("Выход")
        });
        ConsoleMenu menu = new(mainMenu);
        return menu;
    }
}