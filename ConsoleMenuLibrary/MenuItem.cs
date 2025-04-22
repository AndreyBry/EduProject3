namespace ConsoleMenuLibrary
{
    /// <summary>
    /// Класс, описывающий пункт меню.
    /// </summary>
    public class MenuItem
    {
        /// <summary>
        /// Дефолтный конструктор. Установит пустую строку в качестве текста пункта.
        /// </summary>
        public MenuItem() : this("") { }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="text">Текст пункта.</param>
        /// <param name="subMenu">Вложенное меню.</param>
        public MenuItem(string text, Menu? subMenu = null)
        {
            Text = text;
            SubMenu = subMenu;
        }

        /// <summary>Текст пункта.</summary>
        public string Text { get; init; }
        /// <summary>Вложенное меню.</summary>
        public Menu? SubMenu { get; init; }
    }
}
