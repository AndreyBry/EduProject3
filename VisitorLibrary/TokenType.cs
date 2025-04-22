namespace VisitorLibrary
{
    /// <summary>
    /// Перечисление, содержащее типы принимаемых парсером токенов.
    /// </summary>
    internal enum TokenType
    {
        LeftBrace,
        LeftBracket,
        RightBrace,
        RightBracket,
        Colon,
        Comma,
        String,
        Number,
        Null,
        Unfamiliar
    }
}
