using BryntsevAndreyProject3;
using ConsoleMenuLibrary;
using VisitorLibrary;

namespace Project3
{
    /// <summary>
    /// Содержит методы для обработки и выполнения команд, которые выбирает пользователь в консоли.
    /// </summary>
    public static class CommandsHandler
    {
        /// <summary>
        /// Валидные расширения для Excel-таблицы.
        /// </summary>
        private static string[] validExtensions = { ".xlsx", ".xls", ".xlsm", ".xlsb", ".xltx", ".xltm" };

        /// <summary>
        /// Обрабатывает команду, которую выбрал пользователь в консоли, и вызывает соответствующий метод.
        /// </summary>
        /// <param name="command">Команда.</param>
        /// <param name="visitors">Ссылка на объект, содержащий список посетителей.</param>
        public static void HandleConsoleCommand(string command, ref ListOfVisitors visitors)
        {
            switch (command)
            {
                case "Ввести данные через консоль":
                    ReadDataFromConsole(visitors);
                    break;
                case "Прочитать данные из файла":
                    ReadDataFromFile(visitors);
                    break;
                case "Отфильтровать данные":
                    FilterData(visitors);
                    break;
                case "Отсортировать данные":
                    SortData(visitors);
                    break;
                case "Отобразить ключевую информацию о посетителе":
                    DisplayVisitorData(visitors);
                    break;
                case "Прочитать данные из Excel-таблицы":
                    ReadDataFromExcel(visitors);
                    break;
                case "Записать данные в Excel-таблицу":
                    WriteDataToExcel(visitors);
                    break;
                case "Вывести данные в консоль":
                    WriteDataToConsole(visitors);
                    break;
                case "Записать данные в файл":
                    WriteDataToFile(visitors);
                    break;
            }
        }

        /// <summary>
        /// Читает json из консоли и перезаписывает список посетителей на те, которые были получены после обработки парсером.
        /// </summary>
        /// <param name="visitors">Ссылка на объект, содержащий список посетителей.</param>
        private static void ReadDataFromConsole(ListOfVisitors visitors)
        {
            Console.WriteLine("Чтобы завершить ввод, нажмите Ctrl + Z (Windows) или Ctrl + D (Linux/macOS)");
            Console.WriteLine("Напишите json: ");
            string jsonString = ConsoleUtils.ReadLines();
            visitors.Visitors.Clear();
            try
            {
                JsonParser.ParseJsonObject(jsonString, ref visitors);
                ConsoleUtils.WriteLine("Данные успешно прочитаны.", MessageLevel.Success);
            }
            catch (Exception ex) when (ex is JsonParserException || ex is KeyNotFoundException || ex is FormatException)
            {
                visitors.Visitors.Clear();
                ConsoleUtils.WriteLine(ex.Message, MessageLevel.Error);
            }
        }

        /// <summary>
        /// Читает json из файла и перезаписывает список посетителей на те, которые были получены после обработки парсером.
        /// </summary>
        /// <param name="visitors">Ссылка на объект, содержащий список посетителей.</param>
        private static void ReadDataFromFile(ListOfVisitors visitors)
        {
            visitors.Visitors.Clear();
            // Запрашивает файл, пока не будет введен корректный.
            do
            {
                Console.Write("Напишите путь до файла: ");
                string? filePath = Console.ReadLine();
                if (File.Exists(filePath))
                {
                    try
                    {
                        JsonParser.ReadJson(filePath, ref visitors);
                        ConsoleUtils.WriteLine("Данные успешно прочитаны.", MessageLevel.Success);
                    }
                    catch (Exception ex) when (ex is JsonParserException || ex is KeyNotFoundException || ex is FormatException)
                    {
                        visitors.Visitors.Clear();
                        ConsoleUtils.WriteLine(ex.Message, MessageLevel.Error);
                    }
                    catch (IOException)
                    {
                        ConsoleUtils.WriteLine("Произошла ошибка при открытии файла.", MessageLevel.Error);
                    }
                }
                else
                {
                    ConsoleUtils.WriteLine("Файл не был найден на диске.", MessageLevel.Warning);
                }
            } while (visitors.Visitors.Count == 0);
        }

        /// <summary>
        /// Производит фильтрацию посетителей.
        /// </summary>
        /// <param name="visitors">Ссылка на объект, содержащий список посетителей.</param>
        private static void FilterData(ListOfVisitors visitors)
        {
            if (!visitors.IsEmpty())
            {
                Console.WriteLine("Доступна фильтрация по следующим полям: ");
                // Получает названия полей, содержащихся хотя бы в одном из объектов.
                List<string> fields = visitors.Visitors.SelectMany(visitor => visitor.GetNonNestedFields()).Distinct().ToList();
                for (int i = 0; i < fields.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {fields[i]}");
                }
                Console.Write("Введите номер поля: ");
                string? filter = Console.ReadLine();
                if (int.TryParse(filter, out int filterNumber) && filterNumber >= 1 && filterNumber <= fields.Count)
                {
                    List<string> values = new();
                    Console.WriteLine("Чтобы завершить ввод, нажмите Ctrl + Z (Windows) или Ctrl + D (Linux/macOS) или отправьте пустое значение.");
                    do
                    {
                        Console.Write("Введите значение: ");
                        string? value = Console.ReadLine();
                        if (!string.IsNullOrEmpty(value)) values.Add(value);
                        else if (values.Count > 0) break;
                        else ConsoleUtils.WriteLine("Необходимо ввести хотя бы одно значение.", MessageLevel.Warning);
                    } while (true);
                    visitors.Filter(fields[filterNumber - 1], values);
                    if (!visitors.IsEmpty("Совпадения не найдены."))
                    {
                        ConsoleUtils.WriteLine($"Фильтрация по полю \"{fields[filterNumber - 1]}\": ", MessageLevel.Success);
                        // Сериализует список посетителей.
                        string serializedVisitors = Serializer.SerializeListOfJsonObjects("elements", visitors.Visitors);
                        Console.WriteLine(serializedVisitors);
                    }
                }
                else
                {
                    ConsoleUtils.WriteLine("Введен некорректный номер поля.", MessageLevel.Warning);
                }
            }
        }

        /// <summary>
        /// Производит сортировку посетителей.
        /// </summary>
        /// <param name="visitors">Ссылка на объект, содержащий список посетителей.</param>
        private static void SortData(ListOfVisitors visitors)
        {
            if (!visitors.IsEmpty())
            {
                Console.WriteLine("Доступна сортировка по следующим полям: ");
                // Получает названия полей, содержащихся хотя бы в одном из объектов.
                List<string> fields = visitors.Visitors.SelectMany(visitor => visitor.GetNonNestedFields()).Distinct().ToList();
                for (int i = 0; i < fields.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {fields[i]}");
                }
                Console.Write("Введите номер поля: ");
                string? sorter = Console.ReadLine();
                if (int.TryParse(sorter, out int sorterNumber) && sorterNumber >= 1 && sorterNumber <= fields.Count)
                {
                    Console.WriteLine("Выберите тип сортировки: ");
                    List<string> sortTypes = ["По возрастанию", "По убыванию"];
                    for (int i = 0; i < sortTypes.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {sortTypes[i]}");
                    }
                    Console.Write("Введите номер сортировки: ");
                    string? sortType = Console.ReadLine();
                    if (int.TryParse(sortType, out int sortTypeNumber) && sortTypeNumber >= 1 && sortTypeNumber <= fields.Count)
                    {
                        visitors.Sort(fields[sorterNumber - 1], sortTypes[sortTypeNumber - 1]);
                        ConsoleUtils.WriteLine($"Сортировка по полю \"{fields[sorterNumber - 1]}\" ({sortTypes[sortTypeNumber - 1].ToLower()}): ", MessageLevel.Success);
                        // Сериализует список посетителей.
                        string serializedVisitors = Serializer.SerializeListOfJsonObjects("elements", visitors.Visitors);
                        Console.WriteLine(serializedVisitors);
                    }
                }
                else
                {
                    ConsoleUtils.WriteLine("Введен некорректный номер поля.", MessageLevel.Warning);
                }
            }
        }

        /// <summary>
        /// Выводит в консоль ключевую информацию о посетителе.
        /// </summary>
        /// <param name="visitors">Ссылка на объект, содержащий список посетителей.</param>        
        private static void DisplayVisitorData(ListOfVisitors visitors)
        {
            if (!visitors.IsEmpty())
            {
                Console.WriteLine("Посетители: ");
                // Получает айдишники всех посетителей.
                List<string> visitorIds = visitors.Visitors.Select(visitor => visitor.Id).ToList();
                for (int i = 0; i < visitorIds.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {visitorIds[i]}");
                }
                Console.Write("Введите номер посетителя: ");
                string? visitorId = Console.ReadLine();
                if (int.TryParse(visitorId, out int id) && id >= 1 && id <= visitorIds.Count)
                {
                    // Получает выбранного посетителя.
                    Visitor visitor = visitors.Visitors
                        .Where(visitor => visitor.Id == visitorIds[id - 1]).ToList()[0];
                    Console.WriteLine(visitor);
                }
                else
                {
                    ConsoleUtils.WriteLine("Введен некорректный номер посетителя.", MessageLevel.Warning);
                }
            }
        }

        /// <summary>
        /// Получает данные из Excel-таблицы и перезаписывает список посетителей.
        /// </summary>
        /// <param name="visitors">Ссылка на объект, содержащий список посетителей.</param>
        private static void ReadDataFromExcel(ListOfVisitors visitors)
        {
            visitors.Visitors.Clear();
            // Запрашивает файл, пока не будет введен корректный.
            do
            {
                Console.Write("Напишите путь до файла: ");
                string? filePath = Console.ReadLine();
                if (File.Exists(filePath))
                {
                    if (validExtensions.Contains(Path.GetExtension(filePath).ToLower()))
                    {
                        try
                        {
                            ExcelParser.Read(visitors, filePath);
                            ConsoleUtils.WriteLine("Данные успешно прочитаны.", MessageLevel.Success);
                        }
                        catch (IOException)
                        {
                            ConsoleUtils.WriteLine("Произошла ошибка при открытии файла.", MessageLevel.Error);
                        }
                        catch (Exception)
                        {
                            visitors.Visitors.Clear();
                            ConsoleUtils.WriteLine("Произошла ошибка при чтении данных из Excel-таблицы.", MessageLevel.Error);
                        }
                    }
                    else
                    {
                        ConsoleUtils.WriteLine("Введено некорректное имя для Excel-таблицы.", MessageLevel.Warning);
                    }
                }
                else
                {
                    ConsoleUtils.WriteLine("Файл не был найден на диске.", MessageLevel.Warning);
                }
            } while (visitors.Visitors.Count == 0);
        }

        /// <summary>
        /// Записывает в Excel-таблицу данные о посетителях.
        /// </summary>
        /// <param name="visitors">Ссылка на объект, содержащий список посетителей.</param>
        private static void WriteDataToExcel(ListOfVisitors visitors)
        {
            if (!visitors.IsEmpty())
            {
                // Запрашивает файл, пока не будет введен корректный.
                do
                {
                    Console.Write("Напишите путь до Excel файла: ");
                    string? filePath = Console.ReadLine();
                    // Проверяет название файла на корректность.
                    if (!string.IsNullOrWhiteSpace(filePath) && filePath.IndexOfAny(Path.GetInvalidFileNameChars()) == -1 && validExtensions.Contains(Path.GetExtension(filePath).ToLower()))
                    {
                        try
                        {
                            ExcelParser.Write(visitors, filePath);
                            ConsoleUtils.WriteLine("Данные успешно записаны в Excel-таблицу.", MessageLevel.Success);
                            break;
                        }
                        catch (IOException)
                        {
                            ConsoleUtils.WriteLine("Произошла ошибка при создании файла.", MessageLevel.Error);
                        }
                        catch (Exception)
                        {
                            ConsoleUtils.WriteLine("Произошла ошибка при записи данных в Excel-таблицу.", MessageLevel.Error);
                        }
                    }
                    else
                    {
                        ConsoleUtils.WriteLine("Введено некорректное имя для файла.", MessageLevel.Warning);
                    }
                } while (true);
            }
        }

        /// <summary>
        /// Выводит на экран сериализованный список посетителей.
        /// </summary>
        /// <param name="visitors">Ссылка на объект, содержащий список посетителей.</param>
        private static void WriteDataToConsole(ListOfVisitors visitors)
        {
            if (!visitors.IsEmpty())
            {
                // Сериализует список посетителей.
                string serializedVisitors = Serializer.SerializeListOfJsonObjects("elements", visitors.Visitors);
                Console.WriteLine(serializedVisitors);
            }
        }

        /// <summary>
        /// Записывает в файл сериализованный список посетителей.
        /// </summary>
        /// <param name="visitors">Ссылка на объект, содержащий список посетителей.</param>
        private static void WriteDataToFile(ListOfVisitors visitors)
        {
            if (!visitors.IsEmpty())
            {
                // Запрашивает файл, пока не будет введен корректный.
                do
                {
                    Console.Write("Напишите путь до файла: ");
                    string? filePath = Console.ReadLine();
                    // Проверяет название файла на корректность.
                    if (!string.IsNullOrWhiteSpace(filePath) && filePath.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
                    {
                        try
                        {
                            // Сериализует список посетителей.
                            string serializedVisitors = Serializer.SerializeListOfJsonObjects("elements", visitors.Visitors);
                            JsonParser.WriteJson(serializedVisitors, filePath);
                            ConsoleUtils.WriteLine("Данные успешно записаны.", MessageLevel.Success);
                            break;
                        }
                        catch (IOException)
                        {
                            ConsoleUtils.WriteLine("Произошла ошибка при создании файла.", MessageLevel.Error);
                        }
                    }
                    else
                    {
                        ConsoleUtils.WriteLine("Введено некорректное имя для файла.", MessageLevel.Warning);
                    }
                } while (true);
            }
        }
    }
}
