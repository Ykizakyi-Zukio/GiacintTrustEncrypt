namespace GiacintTrustEncrypt.Lib
{
    internal static class Debug
    {
        internal static void Error(Exception ex)
        {
            Console.Error.WriteLine($"{Color.red}× {ex.ToString()}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        internal static void Warning(string message)
        {
            Console.WriteLine($"{Color.yellow}⚠ {message}");
        }

        internal static void Success(string message)
        {
            Console.WriteLine($"{Color.green}✓ {message}");
        }
    }
}
