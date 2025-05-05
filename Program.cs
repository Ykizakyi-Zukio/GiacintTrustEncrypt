using System.Linq.Expressions;
using System.Text;
using GiacintTrustEncrypt.Lib;

namespace GiacintTrustEncrypt;

class Program
{
    Key key;
    Encryptor aes;

    List<string> cmds = new List<string> { 
        "@m@git - open author github", 
        "@pass <pass> - password for encryption", 
        "@pass@v - view hash key by pass", 
        "@pass@check <pass> - check validate of pass", 
        "@hash@v <data> - data to hash", 
        "@aes@ve <data> - fast encrypt data",
        "@aes@vd <data> - fast decrypt data",
        "@help - view all commands",
        "@c - clear cmd",
        "@e@file <filePath> - encrypt file and create .gte file",
        "@d@file <filePath> - decrypt file and create file withount .gte", 
        "@dir <*.extension>pathDirectory> or @dir <pathDirectory> - show all files from directory"
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

            try {
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
                    case "@dir":
                        if (args.Length < 2) { Console.WriteLine("Invalid directory arg."); break; }
                        args[1] = args[1].Trim('\"');
                        string search = "*";
                        if (args[1].StartsWith('*'))
                        {
                            search = args[1].Remove(args[1].IndexOf('@')).TrimEnd('@');
                            args[1] = args[1].Substring(args[1].IndexOf('@')).TrimStart('@');
                        }

                        //if (Directory.Exists(args[1])) { Console.WriteLine("Invalid directory."); break; }

                        Console.WriteLine($"{Color.ultraLightPink} Directory {args[1]}:)");
                        StorageHelper.ProcessDirectory(args[1], search);
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
            } catch (Exception ex) { Console.WriteLine($"× Error {Color.red}{ex.ToString()}"); Console.ForegroundColor = ConsoleColor.White; }
        }
        
    }

    private void HelpMessage()
    {
        Console.WriteLine(Color.ultraLightPink);

        for (int i = 0; i < cmds.Count; i++) { Console.WriteLine(cmds[i]); }

        Console.WriteLine("\r\n" + Color.reset);
    }
}