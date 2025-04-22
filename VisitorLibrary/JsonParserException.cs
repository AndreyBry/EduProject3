namespace VisitorLibrary
{
    /// <summary>
    /// Класс исключений, возникающих при парсинге некорретного json.
    /// </summary>
    public class JsonParserException : Exception
    {
        /// <summary>
        /// Дефолтный конструктор.
        /// </summary>
        public JsonParserException() { }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public JsonParserException(string message) : base($"[JsonParserException] {message}") { }
    }
}
