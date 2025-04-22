namespace VisitorLibrary
{
    /// <summary>
    /// Содержит методы для создания отступов.
    /// </summary>
    public static class IndentUtils
    {
        /// <summary>
        /// Создает отступ для объекта.
        /// </summary>
        /// <param name="objectLevel">Уровень объекта.</param>
        /// <param name="numberOfSpacesInLevel">Количество пробелов в отступе одного уровня.</param>
        /// <returns>Отступ.</returns>
        public static string GetIndent(int objectLevel, int numberOfSpacesInLevel = 2) => new string(' ', objectLevel * numberOfSpacesInLevel);
    }
}
