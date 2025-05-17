﻿namespace GiacintTrustEncrypt.Lib
{
    internal static class Color
    {
        internal static string AMain = "\u001b[38;5;218m";  // Розово-лиловый из 256-цветной палитры
        internal static string BMain = "\u001b[38;5;225m"; // Постельно розовый, более яркий
        internal static string Reset = "\u001b[0m";        // Сброс форматирования
        internal static string Error = "\u001b[38;5;217m"; // светло-розово-красный (почти пастельный)
        internal static string Success = "\u001b[38;5;151m"; // светлый салатово-зелёный
        internal static string Warning = "\x1b[38;5;230m"; // Постельно желтый
        internal static string Info = "\u001b[38;5;153m"; // Постельно голубой
    }
}
