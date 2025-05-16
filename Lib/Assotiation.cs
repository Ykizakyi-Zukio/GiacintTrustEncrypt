using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace GiacintTrustEncrypt.Lib;

public class Association
{
    private readonly string _extension;
    private readonly string _appName;
    private readonly string _appPath;
    private readonly string _fileDescription;
    private readonly string _mimeType;

    /// <summary>
    /// Инициализирует менеджер ассоциаций файлов.
    /// </summary>
    /// <param name="extension">Расширение файла (например, ".myapp")</param>
    /// <param name="appName">Название приложения (например, "MyApp")</param>
    /// <param name="appPath">Полный путь к исполняемому файлу</param>
    /// <param name="fileDescription">Описание типа файла (опционально)</param>
    /// <param name="mimeType">MIME-тип (для Linux/macOS, опционально)</param>
    public Association(
        string extension,
        string appName,
        string appPath,
        string fileDescription = null,
        string mimeType = null)
    {
        _extension = extension;
        _appName = appName;
        _appPath = appPath;
        _fileDescription = fileDescription ?? $"{appName} File";
        _mimeType = mimeType ?? $"application/x-{appName.ToLower()}";
    }

    /// <summary>
    /// Устанавливает ассоциацию файлов для текущей ОС.
    /// </summary>
    public void SetAssociation()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            SetWindowsAssociation();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            SetLinuxAssociation();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            SetMacAssociation();
        }
        else
        {
            throw new PlatformNotSupportedException("Unsupported operating system.");
        }
    }

    /// <summary>
    /// Удаляет ассоциацию файлов для текущей ОС.
    /// </summary>
    public void RemoveAssociation()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            RemoveWindowsAssociation();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            RemoveLinuxAssociation();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            RemoveMacAssociation();
        }
        else
        {
            throw new PlatformNotSupportedException("Unsupported operating system.");
        }
    }

    // ===== Windows =====
    private void SetWindowsAssociation()
    {
        string progId = $"{_appName}.File";

        try
        {
            // 1. Создаём ProgID
            using (RegistryKey progKey = Registry.ClassesRoot.CreateSubKey(progId))
            {
                progKey.SetValue("", _fileDescription);

                // Иконка (опционально)
                using (RegistryKey iconKey = progKey.CreateSubKey("DefaultIcon"))
                {
                    iconKey.SetValue("", $"\"{_appPath}\",0");
                }

                // Команда открытия
                using (RegistryKey commandKey = progKey.CreateSubKey(@"shell\open\command"))
                {
                    commandKey.SetValue("", $"\"{_appPath}\" \"%1\"");
                }
            }

            // 2. Связываем расширение с ProgID
            using (RegistryKey extKey = Registry.ClassesRoot.CreateSubKey(_extension))
            {
                extKey.SetValue("", progId);
            }

            // 3. Обновляем проводник
            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
        }
        catch (UnauthorizedAccessException)
        {
            throw new InvalidOperationException("Admin rights required for file association on Windows.");
        }
    }

    private void RemoveWindowsAssociation()
    {
        string progId = $"{_appName}.File";

        try
        {
            Registry.ClassesRoot.DeleteSubKeyTree(progId, throwOnMissingSubKey: false);
            Registry.ClassesRoot.DeleteSubKeyTree(_extension, throwOnMissingSubKey: false);
            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
        }
        catch (UnauthorizedAccessException)
        {
            throw new InvalidOperationException("Admin rights required to remove file association on Windows.");
        }
    }

    // ===== Linux =====
    private void SetLinuxAssociation()
    {
        string mimeFile = $"/usr/share/mime/packages/{_mimeType}.xml";
        string desktopFile = $"/usr/share/applications/{_appName.ToLower()}.desktop";

        try
        {
            // 1. Создаём MIME-тип
            string mimeContent = $"""
                <?xml version="1.0" encoding="UTF-8"?>
                <mime-info xmlns="http://www.freedesktop.org/standards/shared-mime-info">
                  <mime-type type="{_mimeType}">
                    <comment>{_fileDescription}</comment>
                    <glob pattern="*{_extension}"/>
                  </mime-type>
                </mime-info>
                """;
            File.WriteAllText(mimeFile, mimeContent);

            // 2. Создаём .desktop-файл
            string desktopContent = $"""
                [Desktop Entry]
                Type=Application
                Name={_appName}
                Exec={_appPath} %f
                MimeType={_mimeType}
                """;
            File.WriteAllText(desktopFile, desktopContent);

            // 3. Обновляем кэш
            RunCommand("update-mime-database", "/usr/share/mime");
            RunCommand("update-desktop-database", "/usr/share/applications");
            RunCommand("xdg-mime", $"default {desktopFile} {_mimeType}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to set Linux file association: {ex.Message}");
        }
    }

    private void RemoveLinuxAssociation()
    {
        string mimeFile = $"/usr/share/mime/packages/{_mimeType}.xml";
        string desktopFile = $"/usr/share/applications/{_appName.ToLower()}.desktop";

        try
        {
            if (File.Exists(mimeFile)) File.Delete(mimeFile);
            if (File.Exists(desktopFile)) File.Delete(desktopFile);

            RunCommand("update-mime-database", "/usr/share/mime");
            RunCommand("update-desktop-database", "/usr/share/applications");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to remove Linux file association: {ex.Message}");
        }
    }

    // ===== macOS =====
    private void SetMacAssociation()
    {
        try
        {
            // 1. Регистрируем приложение в Launch Services
            RunCommand("/System/Library/Frameworks/CoreServices.framework/Frameworks/LaunchServices.framework/Support/lsregister", $"-f \"{_appPath}\"");

            // 2. Устанавливаем ассоциацию через `duti` (требует установки: `brew install duti`)
            RunCommand("duti", $"-s {_appName} {_extension} all");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to set macOS file association: {ex.Message}");
        }
    }

    private void RemoveMacAssociation()
    {
        // В macOS нет простого способа удалить ассоциацию, кроме ручного редактирования plist-файлов.
        Console.WriteLine("Note: macOS file associations are managed by Launch Services. Manual cleanup may be required.");
    }

    // ===== Вспомогательные методы =====
    private static void RunCommand(string command, string args)
    {
        using (var process = new Process())
        {
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
        }
    }

    // Windows API для обновления проводника
    [DllImport("shell32.dll")]
    private static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);

    private const int SHCNE_ASSOCCHANGED = 0x08000000;
    private const int SHCNF_IDLIST = 0x0000;
}