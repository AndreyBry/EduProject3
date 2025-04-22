namespace VisitorLibrary
{
    /// <summary>
    /// Класс, описывающий свойства триггера.
    /// </summary>
    public class XTriggerItem : IJSONObject
    {
        /// <summary>
        /// Дефолтный конструктор. Устанавливает пустой словарь.
        /// </summary>
        public XTriggerItem() : this(new Dictionary<string, object>()) { }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="properties">Словарь свойств триггера.</param>
        public XTriggerItem(Dictionary<string, object> properties)
        {
            Properties = properties;
        }

        /// <summary>Словарь, содержащий свойства триггера.</summary>
        public Dictionary<string, object> Properties { get; private set; }

        /// <summary>
        /// Возвращает названия всех полей.
        /// </summary>
        /// <returns>Названия всех полей.</returns>
        public IEnumerable<string> GetAllFields()
        {
            return Properties.Keys;
        }

        /// <summary>
        /// Возвращает значение поля.
        /// </summary>
        /// <param name="fieldName">Название поля.</param>
        /// <returns>Значение поля.</returns>
        public string GetField(string fieldName)
        {
            if (Properties.TryGetValue(fieldName.ToLower(), out object? fieldValue))
            {
                if (int.TryParse(fieldValue.ToString(), out int intFieldValue))
                {
                    return intFieldValue.ToString();
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
            Properties[fieldName.ToLower()] = value;
        }
    }
}
