namespace GiacintTrustEncrypt.Lib
{
    internal class AppConfig
    {
        internal readonly string[] binaryExtensions = {
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

        //INT
        internal int minimalPassLength { get; set; } = 8;
        internal int minimalRecommendedPassLength { get; set; } = 12;

    }
}
