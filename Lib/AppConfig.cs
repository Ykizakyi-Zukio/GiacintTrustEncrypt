using System.Text.Json;
using System.Text.Json.Serialization;

namespace GiacintTrustEncrypt.Lib
{
    [JsonSerializable(typeof(AppConfig))]
    internal class AppConfig
    {
        //ARRAYS
        [JsonIgnore]
        internal readonly string[] BinaryExtensions = {
        ".exe", ".dll", ".bin", ".so", ".out", ".elf", ".apk", ".app",
        ".lib", ".obj", ".a", ".pyd",
        ".iso", ".img", ".rom", ".hex", ".cue",
        ".dat", ".pak", ".pck", ".wad", ".arc", ".db", ".sav", ".binlog",
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp",
        ".mp3", ".wav", ".ogg", ".flac", ".aac", ".m4a",
        ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm",
        ".class", ".pyc", ".jar", ".swf", ".pdf",
        ".ttf", ".otf", ".woff", ".woff2",
        ".resx", ".rsc", ".icns", ".ico",
        ".msi", ".cab", ".vhd", ".vhdx"
        };
        [JsonIgnore]
        internal readonly string[] Cmds = {
        "@m@git - open author github",
        "@update - view latest release",
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
        "@d@bin <filePath> - decrypt binary file (video, audio)",
        "@ec - encrypt selected file (works if file opened by assotiation)",
        "@dc - decrypt selected file (works if file opened by assotiation)",
        };

        //INT
        [JsonInclude]
        internal int MinimalPassLength { get; set; } = 8;
        [JsonInclude]
        internal int MinimalRecommendedPassLength { get; set; } = 12;

        internal void ToJson(string path)
        {
            string json = JsonSerializer.Serialize(this);
            StorageHelper.CreateFile(path, json);
        }

        public AppConfig FromJson(string json)
        {
            AppConfig obj = new();
            try
            {
                obj = JsonSerializer.Deserialize<AppConfig>(json);
            }
            catch (Exception ex) { Debug.Error(ex);}

            return obj;
        }

    }
}
