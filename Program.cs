using System.Text;
using GiacintTrustEncrypt.Lib;

namespace GiacintTrustEncrypt;

class Program
{
    Key key;
    Encryptor aes;

    List<string> cmds = new List<string> { 
        "@m@git", "@pass", "@pass@v", "@pass@e", "@hash@v", "@aes@v", "@help", "@c", "@e@file", "@d@file"
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

        Console.Write($"{Color.pink}/)  /)~ ┏━━━━━━━━━━━━━━━━━┓\r\n" +
            "( •-• )  ~ ♡ Welcome to Giacint Trust Encrypt\r\n" +
            "♡/づづ   Author: @YZukio\r\n" +
            "         Github: https://github.com/Ykizakyi-Zukio \r\n" +
            "      ~ ┗━━━━━━━━━━━━━━━━━┛\r\n\r\n\r\n");

        Console.Write("To start using write your pass for contuite encrypt/decrypt: \r\n" +
            "@pass <pass>\r\n" +
            "@help (Get all commands\r\n" +
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
                        key = new(HashHelper.Hash(args[1]));
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
                case "@pass@check":
                    if (key != null && args.Length > 1)
                        if (key.Get() == HashHelper.Hash(args[1])) { Console.WriteLine($"{Color.green}✓ Correct pass!"); }
                    else
                        Console.WriteLine($"{Color.red}× Uncorrect pass");

                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case "@hash@v":
                    if (args[1].Length > 0)
                        Console.WriteLine(HashHelper.Hash(args[1]));
                    break;
                case "@aes@ve":
                    if (key == null) Console.WriteLine("Invalid pass!");
                    if (args[1].Length == 0) Console.WriteLine("Invalid plane text!");

                    Console.WriteLine(aes.Encrypt(args[1]));
                    break;

                case "@aes@vd":
                    if (key == null) Console.WriteLine("Invalid pass!");
                    if (args[1].Length == 0) Console.WriteLine("Invalid plane text!");

                    Console.WriteLine(aes.Decrypt(args[1]));
                    break;
                case "@help":
                    HelpMessage();
                    break;
                case "@c":
                    Console.Clear();
                    WelcomeMessage();
                    break;

                //FILES & DIRECTORIES
                case "@e@file":
                    if (args.Length <= 1) break;
                    args[1] = args[1].Trim('\"');

                    if (aes == null) { Console.WriteLine("Invalid pass"); break; }
                    if (!File.Exists(args[1])) { Console.WriteLine("File not found"); break; }
                    if (args[1].EndsWith(".gte")) { Console.WriteLine("File already encrypted!"); break; }

                    // Читаем оригинальный файл и шифруем
                    string fileData = StorageHelper.ReadFile(args[1]);
                    string encryptedData = aes.Encrypt(fileData);

                    // Сохраняем зашифрованный файл
                    StorageHelper.CreateFile(args[1] + ".gte", encryptedData);

                    Console.WriteLine("File successfully encrypted!");
                    break;

                case "@d@file":
                    if (args.Length <= 1) break;
                    args[1] = args[1].Trim('\"');

                    if (aes == null) { Console.WriteLine("Invalid pass"); break; }
                    if (!File.Exists(args[1])) { Console.WriteLine("File not found"); break; }
                    if (!args[1].EndsWith(".gte")) { Console.WriteLine("File not encrypted!"); break; }

                    // Читаем зашифрованный файл и расшифровываем
                    string encryptedFile = StorageHelper.ReadFile(args[1]);
                    string decryptedData = aes.Decrypt(encryptedFile);

                    // Сохраняем расшифрованный файл (убираем .gte)
                    StorageHelper.CreateFile(args[1].Substring(0, args[1].Length - 4), decryptedData);

                    Console.WriteLine("File successfully decrypted!");
                    break;

            }
        }
    }

    private void HelpMessage()
    {
        Console.WriteLine(Color.ultraLightPink);

        for (int i = 0; i < cmds.Count; i++) { Console.WriteLine(cmds[i]); }

        Console.WriteLine("\r\n" + Color.reset);
    }
}