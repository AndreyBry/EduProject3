namespace VisitorLibrary
{
    /// <summary>
    /// Класс, описывающий словарь экстсов.
    /// </summary>
    public class DictionaryOfXExts : IJSONObject
    {
        /// <summary>
        /// Дефолтный конструктор. Устанавливает пустой словарь.
        /// </summary>
        public DictionaryOfXExts() : this(new Dictionary<string, string>()) { }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="xexts">Словарь экстсов.</param>
        public DictionaryOfXExts(Dictionary<string, string> xexts)
        {
            XExts = xexts;
        }

        /// <summary>Словарь, содержащий экстсов.</summary>
        public Dictionary<string, string> XExts { get; private set; }

        /// <summary>
        /// Возвращает названия всех полей.
        /// </summary>
        /// <returns>Названия всех полей.</returns>
        public IEnumerable<string> GetAllFields()
        {
            return XExts.Keys;
        }

        /// <summary>
        /// Возвращает значение поля.
        /// </summary>
        /// <param name="fieldName">Название поля.</param>
        /// <returns>Значение поля.</returns>
        public string GetField(string fieldName)
        {
            if (XExts.TryGetValue(fieldName.ToLower(), out string? fieldValue) && fieldValue is not null)
            {
                return $"\"{fieldValue}\"";
            }
            return "null";
        }

        /// <summary>
        /// Устанавливает значение поля.
        /// </summary>
        /// <param name="fieldName">Название поля.</param>
        /// <param name="value">Значение поля.</param>
        public void SetField(string fieldName, string value)
        {
            XExts[fieldName.ToLower()] = value;
        }
    }
}
