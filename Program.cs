using System.Text;
using GiacintTrustEncrypt.Lib;
using System.Reflection;

namespace GiacintTrustEncrypt;

class Program
{
    Key key;
    Encryptor aes;
    BinaryEncryptor binAes;

    AppConfig config = new();

    private string currentFile;

    static void Main(string[] args)
    {
        //IF STARTUP BY ADMIN PERMS, FIXING A COLORS
        Root.EnableVirtualTerminal();

        if (args.Length == 0)
        {
            try
            {
                if (StorageHelper.CreateDat(@$"{Environment.CurrentDirectory}\assotiation.dat", "1") == false)
                {
                    string runExt = StorageHelper.ReturnExists(@$"{Environment.CurrentDirectory}\GiacintTrustEncrypt.exe", @$"{Environment.CurrentDirectory}\GiacintTrustEncrypt");
                    Association association = new(".gte", "GiacintTrustEncrypt", runExt);


                    association.SetAssociation();
                    Debug.Success("Assotiation created\r\n");
                }
                else
                    Debug.Success("Assotiation processed\r\n");
            }
            catch (Exception e) { Debug.Warning("Assotiation not created, if you want use file assotiations open app by administrator permissions.\r\n"); File.Delete(@$"{Environment.CurrentDirectory}\assotiation.dat"); }
        }

        Program main = new();
        Console.OutputEncoding = Encoding.UTF8;
        AppDomain.CurrentDomain.ProcessExit += main.OnExit;

        if (File.Exists($"{Environment.CurrentDirectory}/config.json"))
        {
            main.config = main.config.FromJson(StorageHelper.ReadFile($"{Environment.CurrentDirectory}/config.json"));
            Debug.Success("Config loaded");
        }

        if (args.Length > 0)
        {
            main.currentFile = args[0];
            FileInfo fi = new FileInfo(main.currentFile);
            string shortContent = fi.Length > 256 ? StorageHelper.ReadFile(main.currentFile).Remove(256) + $"... {fi.Length}" : StorageHelper.ReadFile(main.currentFile);

            Debug.Success($"Processed file: {main.currentFile}");
            Console.Write(
                        Color.BMain + "\r\n" +
                        fi.Name + "\r\n "+
                        "Creation, last edit time: \r\n" +
                        File.GetCreationTime(main.currentFile) + "\r\n" +
                        File.GetLastWriteTime(main.currentFile) +
                        
                        "\r\n\r\nShort content: \r\n" +
                        shortContent + "\r\n\r\n" +
                        "Use @pass to enter password\r\n" +
                        "Use @ec / @dc to encrypt / decrypt\r\n\r\n");
            Console.ForegroundColor = ConsoleColor.White;
            main.CommandsInit();
        }
        else
        {
            //DEFUALT RUN
            main.WelcomeMessage();
            main.CommandsInit();
        }
    }

    private void WelcomeMessage()
    {

        Console.Write($"{Color.AMain}/)  /)~ ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓\r\n" +
                                    "( •-• )  ~ ♡ Welcome Giacint Trust Encrypt 1.7 ♡ ~\r\n" +
                                    "♡/づづ   Author: @YZukio\r\n" +
                                    "         Github: https://github.com/Ykizakyi-Zukio \r\n" +
                                    "      ~ ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛\r\n\r\n\r\n");

        Console.Write("Write your pass: \r\n" +
            "@pass <pass>\r\n\r\n" +
            "@help (Get all commands)\r\n" +
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
                    case "@update":
                        WebHelper.OpenURL("https://github.com/Ykizakyi-Zukio/GiacintTrustEncrypt/releases/latest");
                        break;
                    case "@m@git":
                        WebHelper.OpenURL("https://github.com/Ykizakyi-Zukio");
                        break;
                    case "@pass":
                        Debug.Info("Write your pass: ");
                        string pass = Entry.ReadPassword();
                        Console.WriteLine("\r\n");
                        if (pass.Length < 2) { Debug.Warning("Invalid pass"); break; }
                        if (pass.Length < config.MinimalRecommendedPassLength) Debug.Warning("Recommended minimal length of pass is 12 symbols UTF-8");
                        if (pass.Length >= config.MinimalPassLength)
                        {
                            key = new(HashHelper.Hash(pass));
                            Debug.Success("Your encryption key created");
                            Debug.Info("To view his use: @pass@v");
                            aes = new(key.Get());
                            binAes = new(key.Get());
                        }
                        else
                            Debug.Warning("No valid pass, minimal length is 8");
                        break;
                    case "@pass@v":
                        if (key != null)
                            Console.WriteLine(key.Get());
                        else
                            Debug.Warning("Pass not initializated!");
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
                        if (config.BinaryExtensions.Contains(args[1].Substring(args[1].IndexOf("."))))
                        {
                            goto case "@e@bin";
                        }

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
                        if (config.BinaryExtensions.Contains(args[1].Substring(args[1].IndexOf(".")).Replace(".gte", "")))
                        {
                            goto case "@d@bin";
                        }

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

                        Console.WriteLine($"{Color.BMain} Directory {args[1]}:)");
                        StorageHelper.ProcessDirectory(args[1], search);
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case "@root":
                        Console.Clear();
                        RootCommands();
                        return;
                }
            } 
            catch (Exception ex) 
            { 
                if (ex.ToString().StartsWith("System.Security.Cryptography.CryptographicException: Padding is invalid and cannot be removed."))
                {
                    Debug.Error(new Exception("Your pass is incorrect of pass by file was encrypted!"));
                }
                else
                    Debug.Error(ex); 
            }
        }
        
    }

    private void RootCommands()
    {
        Debug.Info("You use root commands, <exit> to defualt mode\r\n");
        while (true)
        {
            string message = Console.ReadLine() ?? "@null";
            string[] args = message.Split(" ");

            switch (args[0])
            {
                case "param@set":
                    try
                    {
                        if (args[1] == null) { Debug.Warning("Invalid param"); break; }
                        if (args[2] == null) { Debug.Warning("Invalid content"); break; }
                        switch (args[1])
                        {
                            case "minPass":
                                config.MinimalPassLength = Convert.ToInt32(args[2]);
                                break;
                        }
                    } catch { Debug.Error(new Exception("Error in parameter set")); }
                    break;
                case "association@unlink":
                    string runExt = StorageHelper.ReturnExists(@$"{Environment.CurrentDirectory}\GiacintTrustEncrypt.exe", @$"{Environment.CurrentDirectory}\GiacintTrustEncrypt");
                    Association association = new(".gte", "GiacintTrustEncrypt", runExt);
                    association.RemoveAssociation();
                    Debug.Info("Association .gte unlinked");
                    break;
                case "exit":
                    Console.Clear();
                    WelcomeMessage();
                    CommandsInit();
                    return;
            }

            
        }
    }

    private void HelpMessage()
    {
        Console.WriteLine(Color.BMain);

        for (int i = 0; i < config.Cmds.Length; i++) { Console.WriteLine(config.Cmds[i]); }

        Console.WriteLine("\r\n");
        Console.ForegroundColor= ConsoleColor.White;
    }

    private void OnExit(object sender, EventArgs e)
    {
        config.ToJson(@$"{Environment.CurrentDirectory}/config.json");
    }
}