using OfficeOpenXml;
using VisitorLibrary;

namespace BryntsevAndreyProject3
{
    /// <summary>
    /// Класс, содержащий методы чтения/записи для данных из/в Excel.
    /// </summary>
    public static class ExcelParser
    {
        /// <summary>
        /// Записывает в Excel-таблицу данные о посетителях.
        /// </summary>
        /// <param name="visitors">Ссылка на объект, содержащий список посетителей.</param>
        /// <param name="filePath">Путь до файла.</param>
        public static void Write(ListOfVisitors visitors, string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                byte[] bytes = GenerateFileWithData(visitors);
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Читает из Excel-таблицы данные о посетителях.
        /// </summary>
        /// <param name="visitors">Ссылка на объект, содержащий список посетителей.</param>
        /// <param name="filePath">Путь до файла.</param>
        public static void Read(ListOfVisitors visitors, string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                List<string> fieldNames = new();
                ExcelWorksheet sheet = package.Workbook.Worksheets[0];
                int rowCount = sheet.Dimension.End.Row;
                int colCount = sheet.Dimension.End.Column;
                // Получает заголовки таблицы.
                for (int j = 0; j < colCount; j++)
                {
                    fieldNames.Add(sheet.Cells[1, j + 1].Text);
                }
                for (int i = 0; i < rowCount - 1; i++)
                {
                    Visitor visitor = new();
                    // Заполняет поля посетителя значениями.
                    for (int j = 0; j < colCount; j++)
                    {
                        string value = sheet.Cells[i + 2, j + 1].Text.Trim('"');
                        if (value == "null")
                            continue;
                        string[] pathToValue = fieldNames[j].Split('/');
                        // Если значение для не вложенного объекта.
                        if (pathToValue.Length == 1)
                        {
                            visitor.SetField(pathToValue[0], value);
                        }
                        // Если значение для вложенного объекта.
                        else
                        {
                            switch (pathToValue[0])
                            {
                                case "aspects":
                                    visitor.Aspects.SetField(pathToValue[1], value);
                                    break;
                                // Если значение для триггера - это не вложенный объект.
                                case "xtriggers" when pathToValue.Length == 2 && fieldNames.Where(fieldName => fieldName.Contains(pathToValue[1])).ToList().Count == 1:
                                    visitor.XTriggers.SetField(pathToValue[1], value);
                                    break;
                                // Если значение для триггера - это вложенный объект.
                                case "xtriggers":
                                    if (!visitor.XTriggers.XTriggers.Keys.Contains(pathToValue[1]))
                                    {
                                        visitor.XTriggers.XTriggers[pathToValue[1]] = new List<object>();
                                    }
                                    List<object> xtriggerItems = (List<object>)visitor.XTriggers.XTriggers[pathToValue[1]];
                                    if (pathToValue.Length == 2)
                                    {
                                        xtriggerItems.Add(value);
                                    }
                                    else
                                    {
                                        if (xtriggerItems.Where(item => item is XTriggerItem).ToList().Count == 0)
                                        {
                                            xtriggerItems.Add(new XTriggerItem());
                                        }
                                        XTriggerItem item = (XTriggerItem)xtriggerItems.Where(item => item is XTriggerItem).ToList()[0];
                                        item.SetField(pathToValue[2], value);
                                    }
                                    break;
                                case "xexts":
                                    visitor.XExts.SetField(pathToValue[1], value);
                                    break;
                            }
                        }
                    }
                    visitors.Visitors.Add(visitor);
                }
            }
        }

        /// <summary>
        /// Создает пакет Excel-таблицы и заполняет ее данными о посетителях.
        /// </summary>
        /// <param name="visitors">Ссылка на объект, содержащий список посетителей.</param>
        /// <returns>Массив байтов пакета.</returns>
        private static byte[] GenerateFileWithData(ListOfVisitors visitors)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Получает множество всех возможные ключей.
            List<string> fieldNames = visitors.Visitors.SelectMany(visitor => visitor.GetOwnAndInternalFields()).Distinct().ToList();
            ExcelPackage package = new();
            ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Visitors");
            // Заполняет заголовки таблицы.
            for (int i = 0; i < fieldNames.Count; i++)
            {
                sheet.Cells[1, i + 1].Value = fieldNames[i];
            }
            // Выделяем строку с заголовками.
            using (ExcelRange range = sheet.Cells[1, 1, 1, fieldNames.Count])
            {
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                range.Style.Font.Bold = true;
            }
            // Заполняет строки таблицы, пробегая по каждому посетителю.
            for (int i = 0; i < visitors.Visitors.Count; i++)
            {
                Visitor visitor = visitors.Visitors[i];
                // Проходит по каждому столбцу таблицы.
                for (int j = 0; j < fieldNames.Count; j++)
                {
                    string[] pathToValue = fieldNames[j].Split('/');
                    // Если значение - это не вложенный объект, записывает его в ячейку.
                    if (pathToValue.Length == 1)
                    {
                        sheet.Cells[i + 2, j + 1].Value = visitor.GetField(pathToValue[0]).Trim('"');
                    }
                    // Если значение - это вложенный объект.
                    else
                    {
                        switch (pathToValue[0])
                        {
                            case "aspects":
                                sheet.Cells[i + 2, j + 1].Value = visitor.Aspects.GetField(pathToValue[1]);
                                break;
                            // Если у объекта нет такого триггера.
                            case "xtriggers" when visitor.XTriggers.GetField(pathToValue[1]) == "null":
                                sheet.Cells[i + 2, j + 1].Value = "null";
                                break;
                            // Если значение триггера - это не вложенный объект.
                            case "xtriggers" when pathToValue.Length == 2 && fieldNames.Where(fieldName => fieldName.Contains($"{pathToValue[1]}/")).ToList().Count == 0:
                                sheet.Cells[i + 2, j + 1].Value = visitor.XTriggers.GetField(pathToValue[1]).Trim('"');
                                break;
                            // Если значение триггера - это вложенный объект.
                            case "xtriggers":
                                List<object> xtriggerItems = new();
                                JsonParser.ParseListOfJsonObjectsAndString<XTriggerItem>(visitor.XTriggers.GetField(pathToValue[1]).Trim('"'), xtriggerItems);
                                // Если в этой ячейке должна быть строка из списка объектов.
                                if (pathToValue.Length == 2)
                                {
                                    sheet.Cells[i + 2, j + 1].Value = xtriggerItems[0].ToString();
                                }
                                // Если в этой ячейке должно быть значение из объекта XTriggerItem.
                                else
                                {
                                    XTriggerItem xTriggerItem = (XTriggerItem)xtriggerItems.Where(item => item is XTriggerItem).ToList()[0];
                                    sheet.Cells[i + 2, j + 1].Value = xTriggerItem.GetField(pathToValue[2]).Trim('"');
                                }
                                break;
                            case "xexts":
                                sheet.Cells[i + 2, j + 1].Value = visitor.XExts.GetField(pathToValue[1]).Trim('"');
                                break;
                        }
                    }
                }
            }
            return package.GetAsByteArray();
        }
    }
}
