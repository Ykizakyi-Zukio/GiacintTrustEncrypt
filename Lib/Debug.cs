using System.Text;

namespace GiacintTrustEncrypt.Lib
{
    internal static class Debug
    {
        internal static void Error(Exception ex)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Error.WriteLine($"{Color.Error}× {ex.ToString()}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        internal static void Warning(string message)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine($"{Color.Warning}⚠  {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        internal static void Success(string message)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine($"{Color.Success}✓  {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        internal static void Info(string message)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine($"{Color.Info}ⓘ  {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
