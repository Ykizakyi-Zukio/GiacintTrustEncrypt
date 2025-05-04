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
    }
}
