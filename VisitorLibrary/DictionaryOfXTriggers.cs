namespace VisitorLibrary
{
    /// <summary>
    /// Класс, описывающий словарь триггеров.
    /// </summary>
    public class DictionaryOfXTriggers : IJSONObject
    {
        /// <summary>
        /// Дефолтный конструктор. Устанавливает пустой словарь.
        /// </summary>
        public DictionaryOfXTriggers() : this(new Dictionary<string, object>()) { }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="xtriggers">Словарь триггеров.</param>
        public DictionaryOfXTriggers(Dictionary<string, object> xtriggers)
        {
            XTriggers = xtriggers;
        }

        /// <summary>Словарь, содержащий триггеры.</summary>
        public Dictionary<string, object> XTriggers { get; private set; }

        /// <summary>
        /// Возвращает названия всех полей.
        /// </summary>
        /// <returns>Названия всех полей.</returns>
        public IEnumerable<string> GetAllFields()
        {
            return XTriggers.Keys;
        }

        /// <summary>
        /// Возвращает значение поля.
        /// </summary>
        /// <param name="fieldName">Название поля.</param>
        /// <returns>Значение поля.</returns>
        public string GetField(string fieldName)
        {
            if (XTriggers.TryGetValue(fieldName.ToLower(), out object? fieldValue))
            {
                if (fieldValue is List<object> xtriggerItems)
                {
                    return $"\"{Serializer.SerializeListOfJsonObjectsAndString<XTriggerItem>(xtriggerItems)}\"";
                }
                else if (fieldValue is not null)
                {
                    return $"\"{fieldValue}\"";
                }
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
            if (value.StartsWith("["))
            {
                List<object> xtriggerItems = new();
                JsonParser.ParseListOfJsonObjectsAndString<XTriggerItem>(value, xtriggerItems);
                XTriggers[fieldName] = xtriggerItems;
            }
            else
            {
                XTriggers[fieldName.ToLower()] = value;
            }
        }
    }
}
