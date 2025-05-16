using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using GiacintTrustEncrypt.Lib;

namespace GiacintTrustEncrypt;

class Program
{
    Key key;
    Encryptor aes;
    BinaryEncryptor binAes;

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
        "@e <filePath> - encrypt file and create .gte file",
        "@d <filePath> - decrypt file and create file withount .gte", 
        "@dir <*.extension>pathDirectory> or @dir <pathDirectory> - show all files from directory",
        "@e@bin <filePath> - encrypt binary file (video, audio)",
        "@d@bin <filePath> - decrypt binary file (video, audio)"
    };

    private string currentFile;

    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            try
            {
                if (StorageHelper.CreateDat(@$"{Environment.CurrentDirectory}\assotiation.dat", "1") == false)
                {
                    string runExt = StorageHelper.ReturnExists(@$"{Environment.CurrentDirectory}\GiacintTrustEncrypt.exe", @$"{Environment.CurrentDirectory}\GiacintTrustEncrypt");
                    Association association = new(".gte", "GiacintTrustEncrypt", runExt);
                    association.SetAssociation();
                    Debug.Success("Assotiation created");
                }
                else
                    Debug.Success("Assotiation processed");
            }
            catch (Exception e) { Debug.Error(e); File.Delete(@$"{Environment.CurrentDirectory}\assotiation.dat"); }
        }

        Program main = new();
        Console.OutputEncoding = Encoding.UTF8;

        if (args.Length > 0 )
        {
            main.currentFile = args[0];
            Debug.Success($"Processed file: {main.currentFile}");
            Console.Write(
                        Color.ultraLightPink + "\r\n" +
                        "Use @pass to enter password\r\n" +
                        "Use @ec / @dc to encrypt / decrypt\r\n\r\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        //DEFUALT RUN
        main.WelcomeMessage();
        main.CommandsInit();
    }

    private void WelcomeMessage()
    {

        Console.Write($"{Color.pink}/)  /)~ ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓\r\n" +
                                    "( •-• )  ~ ♡ Welcome to Giacint Trust Encrypt\r\n" +
                                    "♡/づづ   Author: @YZukio\r\n" +
                                    "         Github: https://github.com/Ykizakyi-Zukio \r\n" +
                                    "      ~ ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛\r\n\r\n\r\n");

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
                        if (args.Length < 2) { Debug.Warning("Invalid pass"); break; }
                        if (args[1].Length > 11)
                        {
                            key = new(HashHelper.Hash(args[1]));
                            Console.WriteLine(key.Get() + ", " + key.Get().Length);
                            aes = new(key.Get());
                            binAes = new(key.Get());
                        }
                        else
                            Debug.Warning("No valid pass, minimal length is 12");
                        break;
                    case "@pass@v":
                        if (key != null)
                            Console.WriteLine(key.Get());
                        else
                            Console.WriteLine("Pass not initializated!");
                        break;
                    case "@pass@check":
                        if (key != null && args.Length > 1)
                            if (key.Get() == HashHelper.Hash(args[1])) { Debug.Success($"Correct pass!"); }
                            else
                                Debug.Warning("Uncorrect pass");

                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case "@hash@v":
                        if (args[1].Length > 0)
                        {
                            var hash = HashHelper.Hash(args[1]);
                            Console.WriteLine(hash + ", " + hash.Length);
                        }
                        break;
                    case "@aes@ve":
                        if (key == null || aes == null) { Console.WriteLine("Invalid pass!"); break; };
                        if (args[1].Length == 0) { Debug.Warning("Invalid plane text!"); break; }

                        Console.WriteLine(aes.Encrypt(args[1]));
                        break;

                    case "@aes@vd":
                        if (key == null || aes == null) { Console.WriteLine("Invalid pass!"); break; }
                        if (args[1].Length == 0) { Debug.Warning("Invalid plane text!"); break; };

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
                    case "@e":
                        if (args.Length <= 1) break;
                        args[1] = args[1].Trim('\"');

                        if (aes == null) { Debug.Warning("Invalid pass"); break; }
                        if (!File.Exists(args[1])) { Debug.Warning("File not exists"); break; }
                        if (args[1].EndsWith(".gte")) { Debug.Warning("File already encrypted!"); break; }

                        // Читаем оригинальный файл и шифруем
                        string fileData = StorageHelper.ReadFile(args[1]);
                        string encryptedData = aes.Encrypt(fileData);

                        // Сохраняем зашифрованный файл
                        StorageHelper.CreateFile(args[1] + ".gte", encryptedData);

                        Console.WriteLine("File successfully encrypted!");
                        break;
                    case "@ec":
                        if (currentFile == null) { Debug.Warning("Invalid current file"); break; }
                        args = new string[2];
                        args[1] = currentFile;
                        goto case "@e";
                    case "@d":
                        if (args.Length <= 1) break;
                        args[1] = args[1].Trim('\"');

                        if (aes == null) { Debug.Warning("Invalid pass"); break; }
                        if (!File.Exists(args[1])) { Debug.Warning("File not exists"); break; }
                        if (!args[1].EndsWith(".gte")) { Debug.Warning("File not encrypted"); break; }

                        // Читаем зашифрованный файл и расшифровываем
                        string encryptedFile = StorageHelper.ReadFile(args[1]);
                        string decryptedData = aes.Decrypt(encryptedFile);

                        // Сохраняем расшифрованный файл (убираем .gte)
                        StorageHelper.CreateFile(args[1].Substring(0, args[1].Length - 4), decryptedData);

                        Console.WriteLine("File successfully decrypted!");
                        break;
                    case "@dc":
                        if (currentFile == null) { Debug.Warning("Invalid current file"); break; }
                        args = new string[2];
                        args[1] = currentFile;
                        goto case "@d";
                    case "@e@bin":
                        if (args.Length <= 1) break;
                        args[1] = args[1].Trim('\"');

                        if (aes == null) { Debug.Warning("Invalid pass"); break; }
                        if (!File.Exists(args[1])) { Debug.Warning("File not exists"); break; }
                        if (args[1].EndsWith(".gte")) { Debug.Warning("File already encrypted!"); break; }

                        byte[] fileBytes = File.ReadAllBytes(args[1]);
                        byte[] encryptedBytes = binAes.Encrypt(fileBytes);

                        File.WriteAllBytes(args[1] + ".gte", encryptedBytes);

                        Console.WriteLine("File sucessfully encrypted!");
                        break;
                    case "@d@bin":
                        if (args.Length <= 1) break;
                        args[1] = args[1].Trim('\"');

                        if (aes == null) { Debug.Warning("Invalid pass"); break; }
                        if (!File.Exists(args[1])) { Debug.Warning("File not exists"); break; }
                        if (!args[1].EndsWith(".gte")) { Debug.Warning("File not encrypted"); break; }

                        byte[] _encryptedBytes = File.ReadAllBytes(args[1]);
                        byte[] _decryptedBytes = binAes.Decrypt(_encryptedBytes);

                        File.WriteAllBytes(args[1].Substring(0, args[1].Length - 4), _decryptedBytes);

                        Console.WriteLine("File successfully decrypted!");
                        break;
                    case "@dir":
                        if (args.Length < 2) { Debug.Warning("Invalid args"); break; }
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
            } catch (Exception ex) { Debug.Error(ex); }
        }
        
    }

    private void HelpMessage()
    {
        Console.WriteLine(Color.ultraLightPink);

        for (int i = 0; i < cmds.Count; i++) { Console.WriteLine(cmds[i]); }

        Console.WriteLine("\r\n");
        Console.ForegroundColor= ConsoleColor.White;
    }
}