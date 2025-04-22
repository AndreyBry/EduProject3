using ConsoleMenuLibrary;

namespace VisitorLibrary
{
    /// <summary>
    /// Класс, описывающий список посетителей.
    /// </summary>
    public class ListOfVisitors : IJSONObject
    {
        /// <summary>
        /// Дефолтный конструктор. Устанавливает пустой список посетителей.
        /// </summary>
        public ListOfVisitors() : this(new List<Visitor>()) { }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="visitors">Список посетителей.</param>
        public ListOfVisitors(List<Visitor> visitors)
        {
            Visitors = visitors;
        }

        /// <summary>Список посетителей.</summary>
        public List<Visitor> Visitors { get; private set; }

        /// <summary>
        /// Возвращает названия всех полей.
        /// </summary>
        /// <returns>Названия всех полей.</returns>
        public IEnumerable<string> GetAllFields() => ["elements"];

        /// <summary>
        /// Возвращает значение поля.
        /// </summary>
        /// <param name="fieldName">Название поля.</param>
        /// <returns>Значение поля.</returns>
        public string GetField(string fieldName)
        {
            return fieldName.ToLower() == "elements" ? 
                $"\"{Serializer.SerializeListOfJsonObjects("elements", Visitors)}\"" : 
                "null";
        }

        /// <summary>
        /// Устанавливает значение поля.
        /// </summary>
        /// <param name="fieldName">Название поля.</param>
        /// <param name="value">Значение поля.</param>
        /// <exception cref="KeyNotFoundException">
        /// Выбрасывается, если не удалось найти поле с таким именем.
        /// </exception>
        public void SetField(string fieldName, string value)
        {
            if (fieldName.ToLower() != "elements")
                throw new KeyNotFoundException("Поля с таким ключом нет.");
            JsonParser.ParseListOfJsonObjects(value, Visitors);
        }

        /// <summary>
        /// Производит фильтрацию посетителей.
        /// </summary>
        /// <param name="fieldName">Название поля.</param>
        /// <param name="values">Значения поля.</param>
        public void Filter(string fieldName, List<string> values)
        {
            Visitors = Visitors
                .Where(visitor => values.Contains(visitor.GetField(fieldName).Trim('"')))
                .ToList();
        }

        /// <summary>
        /// Производит сортировку посетителей.
        /// </summary>
        /// <param name="fieldName">Название поля.</param>
        /// <param name="sortType">Тип сортировки.</param>
        public void Sort(string fieldName, string sortType)
        {
            IEnumerable<Visitor> visitors;
            if (int.TryParse(Visitors[0].GetField(fieldName), out int _))
            {
                if (sortType == "По возрастанию")
                {
                    visitors = Visitors.OrderBy(f => int.TryParse(f.GetField(fieldName), out int val) ? val : 0);
                }
                else
                {
                    visitors = Visitors.OrderByDescending(f => int.TryParse(f.GetField(fieldName), out int val) ? val : 0);
                }
            }
            else
            {
                if (sortType == "По возрастанию")
                {
                    visitors = Visitors.OrderBy(f => f.GetField(fieldName) is not "null" ? f.GetField(fieldName).Trim('"') : "");
                }
                else
                {
                    visitors = Visitors.OrderByDescending(f => f.GetField(fieldName) is not "null" ? f.GetField(fieldName).Trim('"') : "");
                }
            }
            Visitors = visitors.ToList();
        }

        /// <summary>
        /// Проверяет, пустой ли список посетителей.
        /// </summary>
        /// <param name="emptyMessage">Сообщение, которое будет выведено в консоль, в случае если список посетителей пуст.</param>
        /// <returns>Булевое значение, показывающее пуст ли список посетителей.</returns>
        public bool IsEmpty(string emptyMessage = "Сначала необходимо ввести данные.")
        {
            if (Visitors.Count > 0) 
                return false;
            ConsoleUtils.WriteLine(emptyMessage, MessageLevel.Warning);
            return true;
        }
    }
}
