using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleMenuLibrary
{
    /// <summary>
    /// Содержит методы для чтения данных из основного потока ввода и вывода сообщения разных уровней в консоль.
    /// </summary>
    public static class ConsoleUtils
    {
        /// <summary>
        /// Читает данные из основного потока ввода, пока он не будет завершен.
        /// </summary>
        /// <returns>Строка, собранная из введенных строк.</returns>
        public static string ReadLines()
        {
            StringBuilder sb = new();
            string? line;
            while ((line = Console.ReadLine()) != null)
            {
                sb.Append(line);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Выводит сообщения разных уровней в консоль.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        /// <param name="messageLevel">Уровень сообщения.</param>
        public static void WriteLine(string message, MessageLevel messageLevel)
        {
            switch (messageLevel)
            {
                case MessageLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case MessageLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case MessageLevel.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Очищает буфер консоли.
        /// </summary>
        public static void Clear()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.Clear();
            }
            else
            {
                Console.Write("\x1b[2J\x1b[H");
            }
            Console.Write("\x1b[3J");
        }
    }
}
