namespace GiacintTrustEncrypt.Lib
{
    internal class StorageHelper
    {
        internal static string ReadFile(string path)
        {
            FileStream s = new(path, FileMode.Open);
            StreamReader sr = new(s);

            var content = sr.ReadToEnd();

            sr.Close();
            s.Close();

            return content;
        }

        internal static void CreateFile(string path, string content)
        {
            FileStream s = new(path, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new(s);

            sw.Write(content);

            sw.Close();
            s.Close();
        }

        internal static void ProcessDirectory(string targetDirectory, string search = "*")
        {
            try
            {
                string[] files = Directory.GetFiles(targetDirectory, search);

                foreach (string file in files) { Console.WriteLine("Processed file '{0}.'", file); }
            }
            catch (Exception e) { Console.WriteLine(Color.red + e.ToString() + Color.ultraLightPink); }
        }

        internal static void ProcessDirectoryAll(string targetDirectory)
        {
            try
            {
                // Process the list of files found in the directory.
                string[] fileEntries = Directory.GetFiles(targetDirectory);
                foreach (string fileName in fileEntries)
                    ProcessFile(fileName);

                // Recurse into subdirectories of this directory.
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string subdirectory in subdirectoryEntries)
                    ProcessDirectoryAll(subdirectory);
            }
            catch (Exception e) { Console.WriteLine(Color.red + e.ToString() + Color.ultraLightPink); }
        }

        internal static void ProcessFile(string path)
        {
            Console.WriteLine("Processed file '{0}'.", path);
        }

        internal static bool CreateDat(string path, string content)
        {
            if (File.Exists(path))
                return true;
            else
                CreateFile(path, content);
            
            return false;
        }

        internal static string ReturnExists(string first, string second)
        {
            if (File.Exists(first))
                return first;
            else if (File.Exists(second))
                return second;

            throw new Exception($"Not any file exists: {first}, {second}");
        }
    }
}
