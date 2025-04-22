namespace VisitorLibrary
{
    /// <summary>
    /// Класс, описывающий словарь аспектов.
    /// </summary>
    public class DictionaryOfAspects : IJSONObject
    {
        /// <summary>
        /// Дефолтный конструктор. Устанавливает пустой словарь.
        /// </summary>
        public DictionaryOfAspects() : this(new Dictionary<string, int>()) { }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="aspects">Словарь аспектов.</param>
        public DictionaryOfAspects(Dictionary<string, int> aspects)
        {
            Aspects = aspects;
        }

        /// <summary>Словарь, содержащий аспекты.</summary>
        public Dictionary<string, int> Aspects { get; private set; }

        /// <summary>
        /// Возвращает названия всех полей.
        /// </summary>
        /// <returns>Названия всех полей.</returns>
        public IEnumerable<string> GetAllFields()
        {
            return Aspects.Keys;
        }

        /// <summary>
        /// Возвращает значение поля.
        /// </summary>
        /// <param name="fieldName">Название поля.</param>
        /// <returns>Значение поля.</returns>
        public string GetField(string fieldName)
        {
            if (Aspects.TryGetValue(fieldName.ToLower(), out int fieldValue))
            {
                return fieldValue.ToString();
            }
            return "null";
        }

        /// <summary>
        /// Устанавливает значение поля.
        /// </summary>
        /// <param name="fieldName">Название поля.</param>
        /// <param name="value">Значение поля.</param>
        /// <exception cref="FormatException">
        /// Выбрасывается, если не удалось преобразовать к целочисленному типу.
        /// </exception>
        public void SetField(string fieldName, string value)
        {
            if (int.TryParse(value, out int fieldValue))
            {
                Aspects[fieldName.ToLower()] = fieldValue;
            }
            else
            {
                throw new FormatException("Значение поля должно быть целым числом.");
            }
        }
    }
}
