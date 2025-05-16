namespace GiacintTrustEncrypt.Lib
{
    internal class Entry
    {
        internal static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(intercept: true); // Не отображаем на экране

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b"); // Стираем символ в консоли
                }
                else if (!char.IsControl(key.KeyChar)) // Пропускаем служебные клавиши
                {
                    password += key.KeyChar;
                    Console.Write("*"); // Показываем звёздочку вместо символа
                }
            } while (key.Key != ConsoleKey.Enter);

            return password;
        }
    }
}
