
using System.Diagnostics;

namespace GiacintTrustEncrypt.Lib
{
    internal class WebHelper
    {
        internal static void OpenURL(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // Обязательно для открытия в браузере
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Couldn't to open url: " + ex.Message);
            }
        }
    }
}
