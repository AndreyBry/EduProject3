namespace VisitorLibrary
{
    /// <summary>
    /// Интерфейс, описывающий методы, которые должен содержать JSON-объект.
    /// </summary>
    public interface IJSONObject
    {
        /// <summary>
        /// Возвращает названия всех полей.
        /// </summary>
        /// <returns>Названия всех полей.</returns>
        public IEnumerable<string> GetAllFields();

        /// <summary>
        /// Возвращает значение поля.
        /// </summary>
        /// <param name="fieldName">Название поля.</param>
        /// <returns>Значение поля.</returns>
        public string GetField(string fieldName);

        /// <summary>
        /// Устанавливает значение поля.
        /// </summary>
        /// <param name="fieldName">Название поля.</param>
        /// <param name="value">Значение поля.</param>
        public void SetField(string fieldName, string value);
    }
}
