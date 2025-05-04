using System.Text;
using GiacintTrustEncrypt.Lib;


namespace Giacint.TrustEncrypt;

class Program
{
    Key key;
    Encryptor aes;

    string pink = "\u001b[38;5;218m";  // Розово-лиловый из 256-цветной палитры
    string ultraLightPink = "\u001b[38;5;225m";
    string reset = "\u001b[0m";        // Сброс форматирования

    List<string> cmds = new List<string> { 
        "@m@git", "@pass", "@pass@v", "@pass@e", "@hash@v", "@aes@v", "@help"
    };

    static void Main()
    {
        Program main = new();
        Console.OutputEncoding = Encoding.UTF8;
        main.WelcomeMessage();
        main.CommandsInit();
    }

    private void WelcomeMessage()
    {

        Console.Write($"{pink}/)  /)~ ┏━━━━━━━━━━━━━━━━━┓\r\n" +
            "( •-• )  ~ ♡ Welcome to Giacint Trust Encrypt\r\n" +
            "♡/づづ   Author: @YZukio\r\n" +
            "         Github: https://github.com/Ykizakyi-Zukio \r\n" +
            "      ~ ┗━━━━━━━━━━━━━━━━━┛\r\n\r\n\r\n");

        Console.Write("To start using write your pass for contuite encrypt/decrypt: \r\n" +
            "@pass <pass>\r\n" +
            "@help (Get all commands" +
            "@m@git (Go to author github)\r\n\r\n");

        Console.ForegroundColor = ConsoleColor.White;
    }

    private void CommandsInit()
    {
        string message;
        string[] args;
        string mc = "@null";

        while (true)
        {
            message = Console.ReadLine() ?? "@null";
            args = message.Split(" ");
            if (message != "@null") mc = args[0].ToString() ?? "";

            switch (mc)
            {
                //MAIN COMMANDS
                case "@m@git":
                    WebHelper.OpenURL("https://github.com/Ykizakyi-Zukio");
                    break;
                case "@pass":
                    if (args[1].Length > 11)
                    {
                        key = new(HashHelper.Hash(args[1], 24));
                        aes = new(key.Get());
                    }
                    else
                        Console.WriteLine("No valid pass, minimal length is 12");
                        break;
                case "@pass@v":
                    if (key != null)
                        Console.WriteLine(key.Get());
                    else
                        Console.WriteLine("Pass not initializated!");
                    break;
                case "@pass@e":
                    if (key != null)
                        Console.WriteLine(key.ToString());
                    else
                        Console.WriteLine("Pass not initializated!");
                    break;
                case "@hash@v":
                    if (args[1].Length > 0)
                        Console.WriteLine(HashHelper.Hash(args[1], 16));
                    break;
                case "@aes@v":
                    if (key == null) Console.WriteLine("Invalid pass!");
                    if (args[1].Length == 0) Console.WriteLine("Invalid plane text!");

                    Console.WriteLine(aes.Encrypt(args[1]));
                    break;
                case "@help":
                    HelpMessage();
                    break;

                //FILES & DIRECTORIES
                case "@e@file":
                    if (args.Length > 1)
                        args[1] = args[1].Trim('\"');
                    else break;

                    if (aes == null) { Console.WriteLine("Invalid pass"); break; }
                    if (!File.Exists(args[1])) break;
                    if (args[1].EndsWith(".gte")) { Console.WriteLine("File arleady encrypted"); break; }

                    var data = aes.Encrypt(StorageHelper.ReadFile(args[1]));
                    
                    StorageHelper.CreateFile(args[1] + ".gte", data);
                    StorageHelper.ReadFile(args[1]);

                    Console.WriteLine("File success encrypted");
                    break;
                case "@d@file":
                    if (args.Length > 1)
                        args[1] = args[1].Trim('\"');
                    else break;

                    if (aes == null) { Console.WriteLine("Invalid pass"); break; }
                    if (!File.Exists(args[1])) break;
                    if (args[1].EndsWith(".gte") != true) { Console.WriteLine("File not encrypted!"); break; }

                    var _data = aes.Decrypt(StorageHelper.ReadFile(args[1]));

                    StorageHelper.CreateFile(args[1].TrimEnd(".gte".ToCharArray()), _data);
                    StorageHelper.ReadFile(args[1]);

                    Console.WriteLine("File success decrypted!");
                    break;
            }
        }
    }

    private void HelpMessage()
    {
        Console.WriteLine(ultraLightPink);

        for (int i = 0; i < cmds.Count; i++) { Console.WriteLine(cmds[i]); }
    }
}