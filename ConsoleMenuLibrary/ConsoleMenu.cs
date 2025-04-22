namespace ConsoleMenuLibrary
{
    /// <summary>
    /// Класс, описывающий консольное меню. 
    /// </summary>
    public class ConsoleMenu
    {
        /// <summary>
        /// Дефолтный конструктор. Установит в качестве основного пустое меню.
        /// </summary>
        public ConsoleMenu() : this(new Menu()) { }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="mainMenu">Основное меню.</param>
        public ConsoleMenu(Menu mainMenu)
        {
            CurMenu = mainMenu;
            MainMenu = mainMenu;
        }

        /// <summary>Текущее меню.</summary>
        private Menu CurMenu { get; set; }
        /// <summary>Основное меню.</summary>
        private Menu MainMenu { get; init; }
        /// <summary>Индекс выбранного пункта.</summary>
        private int SelectedItemIndex { get; set; }
        /// <summary>Булевое значение, показывающее нужно ли выводить меню в консоль.</summary>
        private bool IsVisible { get; set; }

        /// <summary>
        /// Получает пункт меню от пользователя.
        /// </summary>
        /// <returns>Текст пункта меню.</returns>
        public string GetItemFromUser()
        {
            Console.CursorVisible = false;
            IsVisible = true;
            do
            {
                ConsoleUtils.Clear();
                Print();
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                HandleKeyPress(keyInfo);
            } while (IsVisible);
            Console.CursorVisible = true;
            return CurMenu.Items[SelectedItemIndex].Text;
        }

        /// <summary>
        /// Обрабатывает нажатие пользователя на клавишу.
        /// </summary>
        /// <param name="keyInfo">Информация о нажатой клавише.</param>
        private void HandleKeyPress(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    SelectedItemIndex = SelectedItemIndex == 0 ? CurMenu.Items.Count - 1 : SelectedItemIndex - 1;
                    break;
                case ConsoleKey.DownArrow:
                    SelectedItemIndex = SelectedItemIndex == CurMenu.Items.Count - 1 ? 0 : SelectedItemIndex + 1;
                    break;
                case ConsoleKey.Enter when CurMenu.Items[SelectedItemIndex].SubMenu is Menu subMenu:
                    CurMenu = subMenu;
                    SelectedItemIndex = 0;
                    break;
                case ConsoleKey.Enter when SelectedItemIndex == CurMenu.Items.Count - 1 && CurMenu != MainMenu:
                    CurMenu = MainMenu;
                    SelectedItemIndex = 0;
                    break;
                case ConsoleKey.Enter:
                    IsVisible = false;
                    break;
            }
        }

        /// <summary>
        /// Выводит в консоль пункты меню.
        /// </summary>
        private void Print()
        {
            for (int i = 0; i < CurMenu.Items.Count; i++)
            {
                if (i == SelectedItemIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"> {CurMenu.Items[i].Text}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {CurMenu.Items[i].Text}");
                }
            }
        }
    }
}
