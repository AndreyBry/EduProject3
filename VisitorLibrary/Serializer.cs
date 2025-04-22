using System.Text;

namespace VisitorLibrary
{
    /// <summary>
    /// Содержит методы для сериализации объектов, реализующих интерфейс IJSONObject.
    /// </summary>
    public static class Serializer
    {
        /// <summary>Перенос строки.</summary>
        private static readonly string NEWLINE = Environment.NewLine;

        /// <summary>
        /// Сериализует объект.
        /// </summary>
        /// <param name="jsonObject">Объект.</param>
        /// <param name="objectLevel">Уровень объекта.</param>
        /// <returns>Сериализованный объект.</returns>
        /// <typeparam name="T">Тип объекта, реализующего интерфейс IJSONObject.</typeparam>
        public static string SerializeJsonObject<T>(T jsonObject, int objectLevel = 0) where T : IJSONObject
        {
            StringBuilder sb = new();
            if (objectLevel < 3)
                sb.Append(IndentUtils.GetIndent(objectLevel));
            sb.Append($"{{{NEWLINE}");
            // Сериализует поля объекта.
            List<string> fields = jsonObject.GetAllFields().ToList();
            for (int i = 0; i < fields.Count; i++)
            {
                string fieldName = fields[i];
                sb.Append($"{IndentUtils.GetIndent(objectLevel + 1)}\"{fieldName}\": ");
                string fieldValue = jsonObject.GetField(fieldName);
                // Если поле вложенное, то оно добавляется без кавычек.
                sb.Append((fieldValue.StartsWith("\"{") || fieldValue.StartsWith("\"[")) && fieldName != "desc" ? fieldValue.Trim('"') : fieldValue);
                if (i != fields.Count - 1)
                    sb.Append(",");
                sb.Append(NEWLINE);
            }
            sb.Append($"{IndentUtils.GetIndent(objectLevel)}}}");
            return sb.ToString();
        }

        /// <summary>
        /// Сериализует массив объектов и строк.
        /// </summary>
        /// <param name="objects">Лист объектов.</param>
        /// <param name="objectLevel">Уровень объекта.</param>
        /// <returns>Сериализованный массив объектов.</returns>
        public static string SerializeListOfJsonObjectsAndString<T>(List<object> objects, int objectLevel = 4) where T : IJSONObject
        {
            StringBuilder sb = new();
            sb.Append($"[{NEWLINE}");
            for (int objectNumber = 0; objectNumber < objects.Count; objectNumber++)
            {
                object item = objects[objectNumber];
                sb.Append(IndentUtils.GetIndent(objectLevel + 1));
                sb.Append(item is T complexItem ? SerializeJsonObject(complexItem, objectLevel + 1) : $"\"{item}\"");
                if (objectNumber != objects.Count - 1)
                    sb.Append(",");
                sb.Append(NEWLINE);
            }
            sb.Append($"{IndentUtils.GetIndent(objectLevel)}]");
            return sb.ToString();
        }

        /// <summary>
        /// Сериализует массив объектов.
        /// </summary>
        /// <param name="fieldName">Имя поля, содержащего список объектов.</param>
        /// <param name="jsonObjects">Лист объектов.</param>
        /// <param name="objectLevel">Уровень объекта.</param>
        /// <returns>Сериализованный массив объектов.</returns>
        public static string SerializeListOfJsonObjects<T>(string fieldName, List<T> jsonObjects, int objectLevel = 1) where T : IJSONObject
        {
            StringBuilder sb = new();
            sb.Append($"{{{NEWLINE}{IndentUtils.GetIndent(objectLevel)}");
            sb.Append($"\"{fieldName}\": ");
            sb.Append($"[{NEWLINE}");
            // Сериализует каждый объект.
            for (int jsonObjectNumber = 0; jsonObjectNumber < jsonObjects.Count; jsonObjectNumber++)
            {
                sb.Append(SerializeJsonObject(jsonObjects[jsonObjectNumber], objectLevel + 1));
                if (jsonObjectNumber != jsonObjects.Count - 1)
                    sb.Append(",");
                sb.Append(NEWLINE);
            }
            sb.Append($"{IndentUtils.GetIndent(objectLevel)}]{NEWLINE}}}");
            return sb.ToString();
        }
    }
}
