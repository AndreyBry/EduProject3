using ConsoleMenuLibrary;
using System.Text;

namespace VisitorLibrary
{
    /// <summary>
    /// Содержит методы для парсинга json-объектов.
    /// </summary>
    public static class JsonParser
    {
        /// <summary>
        /// Записывает в файл сериализованный json-объект.
        /// </summary>
        /// <param name="jsonString">Сериализованный json-объект.</param>
        /// <param name="filePath">Путь до файла.</param>
        public static void WriteJson(string jsonString, string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    // Перенаправляет стандартный поток вывода в файл.
                    Console.SetOut(writer);
                    Console.WriteLine(jsonString);
                }
            }
            // Возвращается к стандартному потоку вывода.
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        /// <summary>
        /// Читает из файла сериализованный json-объект и отправляет его на десериализацию.
        /// </summary>
        /// <typeparam name="T">Тип объекта, реализующего интерфейс IJSONObject.</typeparam>
        /// <param name="filePath">Путь до файла.</param>
        /// <param name="jsonObject">Json-объект, поля которого нужно заполнить значениями из json-файла.</param>
        public static void ReadJson<T>(string filePath, ref T jsonObject) where T : IJSONObject
        {
            string jsonString;
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    // Перенаправляет стандартный поток ввода в файл.
                    Console.SetIn(reader);
                    jsonString = ConsoleUtils.ReadLines();
                }
            }
            // Возвращается к стандартному потоку ввода.
            Console.SetIn(new StreamReader(Console.OpenStandardInput()));
            ParseJsonObject(jsonString, ref jsonObject);
        }

        /// <summary>
        /// Парсит список json-объектов.
        /// </summary>
        /// <typeparam name="T">Тип объекта, реализующего интерфейс IJSONObject.</typeparam>
        /// <param name="jsonString">Сериализованный список json-объектов.</param>
        /// <param name="jsonObjects">Список, который нужно заполнить десериализованными json-объектами.</param>
        /// <exception cref="JsonParserException">
        /// Выбрасывается, если сериализованный список json-объектов имеет неправильную структуру.
        /// </exception>
        public static void ParseListOfJsonObjects<T>(string jsonString, List<T> jsonObjects) where T : IJSONObject, new()
        {
            int curPosition = 0;
            if (GetNextToken(jsonString, ref curPosition).Type != TokenType.LeftBracket)
                throw new JsonParserException("Ожидается '[' в начале списка объектов.");
            do
            {
                string value = GetValue(jsonString, ref curPosition);
                jsonObjects.Add(CreateAndParseJsonObject<T>(value));
                Token comma = GetNextToken(jsonString, ref curPosition);
                if (comma.Type == TokenType.RightBracket)
                    break;
                else if (comma.Type != TokenType.Comma)
                    throw new JsonParserException("Ожидается ',' или ']' после значения объекта.");
            } while (true);
        }

        /// <summary>
        /// Парсит список json-объектов и строк.
        /// </summary>
        /// <typeparam name="T">Тип объекта, реализующего интерфейс IJSONObject.</typeparam>
        /// <param name="jsonString">Сериализованный список json-объектов и строк.</param>
        /// <param name="objects">Список, который нужно заполнить десериализованными json-объектами и строками.</param>
        /// <exception cref="JsonParserException">
        /// Выбрасывается, если сериализованный список json-объектов и строк имеет неправильную структуру.
        /// </exception>
        public static void ParseListOfJsonObjectsAndString<T>(string jsonString, List<object> objects) where T : IJSONObject, new()
        {
            int curPosition = 0;
            if (GetNextToken(jsonString, ref curPosition).Type != TokenType.LeftBracket)
                throw new JsonParserException("Ожидается '[' в начале списка объектов.");
            do
            {
                string value = GetValue(jsonString, ref curPosition);
                if (value.StartsWith("{"))
                {
                    objects.Add(CreateAndParseJsonObject<T>(value));
                }
                else
                {
                    objects.Add(value);
                }
                Token comma = GetNextToken(jsonString, ref curPosition);
                if (comma.Type == TokenType.RightBracket)
                    break;
                else if (comma.Type != TokenType.Comma)
                    throw new JsonParserException("Ожидается ',' или ']' после значения объекта.");
            } while (true);
        }

        /// <summary>
        /// Парсит json-объект.
        /// </summary>
        /// <typeparam name="T">Тип объекта, реализующего интерфейс IJSONObject.</typeparam>
        /// <param name="jsonString">Сериализованный json-объект.</param>
        /// <param name="jsonObject">Json-объект, поля которого нужно заполнить.</param>
        /// <exception cref="JsonParserException">
        /// Выбрасывается, если сериализованный json-объект имеет неправильную структуру.
        /// </exception>
        public static void ParseJsonObject<T>(string jsonString, ref T jsonObject) where T : IJSONObject
        {
            int curPosition = 0;
            if (GetNextToken(jsonString, ref curPosition).Type != TokenType.LeftBrace)
                throw new JsonParserException("Ожидается '{' в начале объекта.");
            do
            {
                // Ищет ключ или закрывающую скобку.
                Token key = GetNextToken(jsonString, ref curPosition);
                if (key.Type == TokenType.RightBrace)
                    break;
                else if (key.Type != TokenType.String)
                    throw new JsonParserException("Ожидается строка в качестве ключа объекта.");
                // Ищет двоеточие.
                if (GetNextToken(jsonString, ref curPosition).Type != TokenType.Colon)
                    throw new JsonParserException("Ожидается ':' после ключа объекта.");
                jsonObject.SetField(key.Value, GetValue(jsonString, ref curPosition));
                // Ищет запятую или закрывающую скобку.
                Token comma = GetNextToken(jsonString, ref curPosition);
                if (comma.Type == TokenType.RightBrace)
                    break;
                else if (comma.Type != TokenType.Comma)
                    throw new JsonParserException("Ожидается ',' или '}' после значения объекта.");
            } while (true);
        }

        /// <summary>
        /// Создает json-объект и заполняет его поля значениями.
        /// </summary>
        /// <typeparam name="T">Тип объекта, реализующего интерфейс IJSONObject.</typeparam>
        /// <param name="jsonString">Сериализованный json-объект.</param>
        /// <returns>Сформированный json-объект.</returns>
        private static T CreateAndParseJsonObject<T>(string jsonString) where T : IJSONObject, new()
        {
            // Создаем экземпляр json-объекта.
            T jsonObject = new();
            // Заполняем его поля.
            ParseJsonObject(jsonString, ref jsonObject);
            return jsonObject;
        }

        /// <summary>
        /// Получает значение поля.
        /// </summary>
        /// <param name="jsonString">Сериализованный json-объект.</param>
        /// <param name="curPosition">Индекс текущего символа в строке.</param>
        /// <returns>Значение поля.</returns>
        /// <exception cref="JsonParserException">
        /// Выбрасывается, если сериализованный json-объект имеет неправильную структуру.
        /// </exception>
        private static string GetValue(string jsonString, ref int curPosition)
        {
            Token value = GetNextToken(jsonString, ref curPosition);
            return value.Type switch
            {
                TokenType.LeftBrace or TokenType.LeftBracket => GetObjectAsString(jsonString, ref curPosition),
                TokenType.String or TokenType.Number or TokenType.Null => value.Value,
                _ => throw new JsonParserException($"Неизвестный тип значения: {value.Value}")
            };
        }

        /// <summary>
        /// Возвращает следующий json-объект в виде строки.
        /// </summary>
        /// <param name="jsonString">Сериализованный json-объект.</param>
        /// <param name="curPosition">Индекс текущего символа в строке.</param>
        /// <returns>Json-объект в виде строки.</returns>
        private static string GetObjectAsString(string jsonString, ref int curPosition)
        {
            int leftPosition = --curPosition;
            SkipJsonObject(jsonString, ref curPosition);
            return jsonString.Substring(leftPosition, curPosition - leftPosition);
        }

        /// <summary>
        /// Получает следующий токен (символ, строка или число).
        /// </summary>
        /// <param name="jsonString">Сериализованный json-объект.</param>
        /// <param name="curPosition">Индекс текущего символа в строке.</param>
        /// <returns>Токен.</returns>
        /// <exception cref="JsonParserException">
        /// Выбрасывается, если сериализованный json-объект имеет неправильную структуру.
        /// </exception>
        private static Token GetNextToken(string jsonString, ref int curPosition)
        {
            // Пропускает пробельные символы.
            while (curPosition < jsonString.Length && char.IsWhiteSpace(jsonString[curPosition]))
                curPosition++;
            if (curPosition >= jsonString.Length)
                throw new JsonParserException("Ожидается '}' в конце файла.");
            switch (jsonString[curPosition])
            {
                case '{':
                    return new Token(TokenType.LeftBrace, jsonString[curPosition++].ToString());
                case '[':
                    return new Token(TokenType.LeftBracket, jsonString[curPosition++].ToString());
                case '}':
                    return new Token(TokenType.RightBrace, jsonString[curPosition++].ToString());
                case ']':
                    return new Token(TokenType.RightBracket, jsonString[curPosition++].ToString());
                case ':':
                    return new Token(TokenType.Colon, jsonString[curPosition++].ToString());
                case ',':
                    return new Token(TokenType.Comma, jsonString[curPosition++].ToString());
                case '"':
                    return GetString(jsonString, ref curPosition);
                case '-':
                case char c when char.IsDigit(c):
                    return GetNumber(jsonString, ref curPosition);
                case 'n' when jsonString.Length - curPosition >= 4 && jsonString.Substring(curPosition, 4) == "null":
                    curPosition += 4;
                    return new Token(TokenType.Null, "");
                default:
                    return new Token(TokenType.Unfamiliar, jsonString[curPosition++].ToString());
            }
        }

        /// <summary>
        /// Пропускает вложенный json-объект.
        /// </summary>
        /// <param name="jsonString">Сериализованный json-объект.</param>
        /// <param name="curPosition">Индекс текущего символа в строке.</param>
        /// <exception cref="JsonParserException">
        /// Выбрасывается, если сериализованный json-объект имеет неправильную структуру.
        /// </exception>
        private static void SkipJsonObject(string jsonString, ref int curPosition)
        {
            // Алгоритм, который для каждой открывающей скобки находит закрывающую.
            Stack<Token> bracketStack = new();
            do
            {
                Token token = GetNextToken(jsonString, ref curPosition);
                switch (token.Type)
                {
                    case TokenType.LeftBrace:
                    case TokenType.LeftBracket:
                        bracketStack.Push(token);
                        break;
                    case TokenType.RightBrace:
                    case TokenType.RightBracket:
                        if (bracketStack.Count == 0)
                            throw new JsonParserException("Ожидается открывающая скобка в начале json-объекта.");
                        Token previousToken = bracketStack.Pop();
                        // Проверяет соответствие закрывающей скобки с последней открывающей.
                        if ((token.Type == TokenType.RightBrace && previousToken.Type != TokenType.LeftBrace) ||
                            (token.Type == TokenType.RightBracket && previousToken.Type != TokenType.LeftBracket))
                            throw new JsonParserException("Ожидается открывающая скобка в начале json-объекта.");
                        break;
                }
            } while (curPosition < jsonString.Length && bracketStack.Count != 0);
            // Проверяем, что все скобки были закрыты.
            if (curPosition >= jsonString.Length && bracketStack.Count > 0)
                throw new JsonParserException("Ожидается закрывающая скобка в конце json-объекта.");
        }

        /// <summary>
        /// Получает строку, лежащую от '"' до '"'.
        /// </summary>
        /// <param name="jsonString">Сериализованный json-объект.</param>
        /// <param name="curPosition">Индекс текущего символа в строке.</param>
        /// <returns>Токен, содержащий строку.</returns>
        private static Token GetString(string jsonString, ref int curPosition)
        {
            // Пропускает открывающую кавычку.
            curPosition++;
            StringBuilder sb = new StringBuilder();
            // Собирает символы, которые являются частью строки.
            while (curPosition < jsonString.Length && jsonString[curPosition] != '"')
                sb.Append(jsonString[curPosition++]);
            if (curPosition >= jsonString.Length)
                throw new JsonParserException("Ожидается закрывающая кавычка после строкового значения.");
            // Пропускает закрывающую кавычку.
            curPosition++;
            return new Token(TokenType.String, sb.ToString());
        }

        /// <summary>
        /// Получает числовое значение.
        /// </summary>
        /// <param name="jsonString">мСериализованный json-объект.</param>
        /// <param name="curPosition">Индекс текущего символа в строке.</param>
        /// <returns>Токен, содержащий числовое значение.</returns>
        private static Token GetNumber(string jsonString, ref int curPosition)
        {
            StringBuilder sb = new StringBuilder();
            // Собирает символы, которые являются частью числа.
            while (curPosition < jsonString.Length &&
                (char.IsDigit(jsonString[curPosition]) || jsonString[curPosition] is '.' or '-'))
                sb.Append(jsonString[curPosition++]);
            if (curPosition >= jsonString.Length)
                throw new JsonParserException("Ожидается закрывающая скобка в конце json-объекта.");
            return new Token(TokenType.Number, sb.ToString());
        }
    }
}
