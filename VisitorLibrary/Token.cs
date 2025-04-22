namespace VisitorLibrary
{
    /// <summary>
    /// Класс, описывающий токен.
    /// </summary>
    internal struct Token
    {
        /// <summary>
        /// Дефолтный конструктор. Создаст нулевой токен.
        /// </summary>
        public Token() : this(TokenType.Null, "null") { }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="type">Тип токена.</param>
        /// <param name="value">Значение токена.</param>
        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        /// <summary>Тип токена.</summary>
        public TokenType Type { get; init; }
        /// <summary>Значение токена.</summary>
        public string Value { get; init; }
    }
}
