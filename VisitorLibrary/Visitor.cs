using System.Text;

namespace VisitorLibrary
{
    /// <summary>
    /// Класс, описывающий посетителя.
    /// </summary>
    public struct Visitor : IJSONObject
    {
        /// <summary>
        /// Дефолтный конструктор. Устанавливает значение айди "Unfamiliar" и пустые словари для аспектов и триггеров.
        /// </summary>
        public Visitor()
        {
            Id = "Unfamiliar";
            Aspects = new DictionaryOfAspects();
            XTriggers = new DictionaryOfXTriggers();
            XExts = new DictionaryOfXExts();
        }

        /// <summary>Айди.</summary>
        public string Id { get; private set; }
        /// <summary>Подпись.</summary>
        public string? Label { get; private set; }
        /// <summary>Описание.</summary>
        public string? Desc { get; private set; }
        /// <summary>Иконка.</summary>
        public string? Icon { get; private set; }
        /// <summary>Аудио.</summary>
        public string? Audio { get; private set; }
        /// <summary>Наследуется от.</summary>
        public string? Inherits { get; private set; }
        /// <summary>Словарь аспектов.</summary>
        public DictionaryOfAspects Aspects { get; private set; }
        /// <summary>Комментарии.</summary>
        public string? Comments { get; private set; }
        /// <summary>Распадается на.</summary>
        public string? DecayTo { get; private set; }
        /// <summary>Время жизни.</summary>
        public int? LifeTime { get; private set; }
        /// <summary>Словарь триггеров.</summary>
        public DictionaryOfXTriggers XTriggers { get; private set; }
        /// <summary>Словарь экстсов.</summary>
        public DictionaryOfXExts XExts { get; private set; }


        /// <summary>
        /// Возвращает названия невложенных ненулевых полей.
        /// </summary>
        /// <returns>Названия невложенных ненулевых полей.</returns>
        public IEnumerable<string> GetNonNestedFields()
        {
            Visitor self = this;
            return GetType().GetProperties()
                    .Where(property => property.GetValue(self) is not null and not IJSONObject)
                    .Select(property => property.Name.ToLower());
        }

        /// <summary>
        /// Возвращает названия всех полей, значениями которых являются строки или числа, то есть элементарные объекты.
        /// </summary>
        /// <returns>Список названий полей.</returns>
        public IEnumerable<string> GetOwnAndInternalFields()
        {
            List<string> fields = GetNonNestedFields().ToList();
            fields.AddRange(Aspects.GetAllFields().Select(fieldName => $"aspects/{fieldName}"));
            foreach (string fieldName in XTriggers.GetAllFields())
            {
                string fieldValue = XTriggers.GetField(fieldName);
                if (fieldValue.StartsWith("\"["))
                {
                    List<object> items = new();
                    // Получает список строк и объектов XTriggerItem.
                    JsonParser.ParseListOfJsonObjectsAndString<XTriggerItem>(fieldValue.Trim('"'), items);
                    if (items.Any(item => item is string))
                        fields.Add($"xtriggers/{fieldName}");
                    foreach (XTriggerItem item in items.Where(item => item is XTriggerItem))
                        fields.AddRange(item.GetAllFields().Select(field => $"xtriggers/{fieldName}/{field}"));
                }
                else
                {
                    fields.Add($"xtriggers/{fieldName}");
                }
            }
            fields.AddRange(XExts.GetAllFields().Select(fieldName => $"xexts/{fieldName}"));
            return fields;
        }

        /// <summary>
        /// Возвращает названия ненулевых полей.
        /// </summary>
        /// <returns>Названия ненулевых полей.</returns>
        public IEnumerable<string> GetAllFields()
        {
            Visitor self = this;
            return GetType().GetProperties()
                    .Where(property =>
                    {
                        object? value = property.GetValue(self);
                        return value is not null && (!(value is IJSONObject jsonObject) || jsonObject.GetAllFields().Any());
                    })
                    .Select(property => property.Name.ToLower());
        }

        /// <summary>
        /// Возвращает значение поля.
        /// </summary>
        /// <param name="fieldName">Название поля.</param>
        /// <returns>Значение поля.</returns>
        public string GetField(string fieldName)
        {
            return fieldName.ToLower() switch
            {
                "id" => $"\"{Id}\"",
                "label" => $"\"{Label}\"",
                "desc" => $"\"{Desc}\"",
                "icon" => $"\"{Icon}\"",
                "audio" => $"\"{Audio}\"",
                "inherits" => $"\"{Inherits}\"",
                "aspects" => $"\"{Serializer.SerializeJsonObject(Aspects, 3)}\"",
                "comments" => $"\"{Comments}\"",
                "decayto" => $"\"{DecayTo}\"",
                "lifetime" => LifeTime?.ToString() ?? "null",
                "xtriggers" => $"\"{Serializer.SerializeJsonObject(XTriggers, 3)}\"",
                "xexts" => $"\"{Serializer.SerializeJsonObject(XExts, 3)}\"",
                _ => "null"
            };
        }

        /// <summary>
        /// Устанавливает значение поля.
        /// </summary>
        /// <param name="fieldName">Название поля.</param>
        /// <param name="value">Значение поля.</param>
        /// <exception cref="FormatException">
        /// Выбрасывается, если не удалось преобразовать к целочисленному типу.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// Выбрасывается, если не удалось найти поле с таким именем.
        /// </exception>
        public void SetField(string fieldName, string value)
        {
            switch (fieldName.ToLower())
            {
                case "id":
                    Id = value;
                    break;
                case "label":
                    Label = value;
                    break;
                case "desc":
                    Desc = value;
                    break;
                case "icon":
                    Icon = value;
                    break;
                case "audio":
                    Audio = value;
                    break;
                case "inherits":
                    Inherits = value;
                    break;
                case "aspects":
                    DictionaryOfAspects aspects = Aspects;
                    JsonParser.ParseJsonObject(value, ref aspects);
                    break;
                case "comments":
                    Comments = value;
                    break;
                case "decayto":
                    DecayTo = value;
                    break;
                case "lifetime":
                    LifeTime = int.TryParse(value, out int intValue) ? intValue :
                        throw new FormatException("Значение поля должно быть целым числом.");
                    break;
                case "xtriggers":
                    DictionaryOfXTriggers xtriggers = XTriggers;
                    JsonParser.ParseJsonObject(value, ref xtriggers);
                    break;
                case "xexts":
                    DictionaryOfXExts xexts = XExts;
                    JsonParser.ParseJsonObject(value, ref xexts);
                    break;
                default:
                    Console.WriteLine(fieldName);
                    throw new KeyNotFoundException("Поля с таким ключом нет.");
            }
        }

        /// <summary>
        /// Возвращакт ключевую информацию о посетителе.
        /// </summary>
        /// <returns>Строковое представление ключевой информации.</returns>
        public override string ToString()
        {
            string newLine = Environment.NewLine;
            StringBuilder sb = new();
            sb.Append($"Персонаж:  {Label} ({Id}){newLine}{newLine}");
            sb.Append($"Описание:  {Desc}{newLine}{newLine}");
            sb.Append($"Аспекты:{newLine}");
            List<string> aspectsInfo = new();
            foreach (string aspectName in Aspects.GetAllFields())
                aspectsInfo.Add($"{IndentUtils.GetIndent(2)}{aspectName}: {Aspects.GetField(aspectName)}");
            sb.Append(string.Join(newLine, aspectsInfo));
            return sb.ToString();
        }
    }
}
