namespace ConsoleMenuLibrary
{
    /// <summary>
    /// Класс, описывающий меню.
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// Дефолтный конструктор. Установит пустой список пунктов меню.
        /// </summary>
        public Menu() : this(new List<MenuItem>()) { }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="items">Список пунктов меню.</param>
        public Menu(List<MenuItem> items)
        {
            Items = items;
        }

        /// <summary>Список пунктов меню.</summary>
        public List<MenuItem> Items { get; init; }
    }
}