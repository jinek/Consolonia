#pragma warning disable CA1416 // Validate platform compatibility
using System;
using System.Text;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;
using static Vanara.PInvoke.Kernel32;

namespace Consolonia.PlatformSupport
{
    /// <summary>
    ///     This creates an alternate screen buffer using windows apis,
    ///     and switches Console output to use it.
    /// </summary>
    internal class WindowsLegacyConsoleOutput : DefaultNetConsoleOutput
    {
        private WindowsConsoleBuffer _consoleBuffer;
        private WindowsConsoleBuffer _originalBuffer;

        public override void PrepareConsole()
        {
            base.PrepareConsole();

            Console.OutputEncoding = Encoding.Unicode;
            _originalBuffer = WindowsConsoleBuffer.GetCurrent();

            // create secondary buffer
            _consoleBuffer = WindowsConsoleBuffer.Create();
            _consoleBuffer.SetAsActiveBuffer();
        }

        public override void Print(PixelBufferCoordinate bufferPoint, Color background, Color foreground,
            FontStyle? style, FontWeight? weight, TextDecorationLocation? textDecoration, string str)
        {
            base.Print(bufferPoint, background, foreground, style, weight, textDecoration, str);

            if (textDecoration == TextDecorationLocation.Underline)
            {
                uint length = (uint)str.Length;
                var attributes = new CHARACTER_ATTRIBUTE[length];
                var coord = new COORD(bufferPoint.X, bufferPoint.Y);
                ReadConsoleOutputAttribute(_consoleBuffer.Handle, attributes, length, coord, out uint nRead);
                if (nRead == length)
                {
                    for (int i = 0; i < length; i++)
                        attributes[i] |= CHARACTER_ATTRIBUTE.COMMON_LVB_UNDERSCORE;

                    WriteConsoleOutputAttribute(_consoleBuffer.Handle, attributes, length, coord, out _);
                }
            }
        }

        public override void RestoreConsole()
        {
            _originalBuffer.SetAsActiveBuffer();
            base.RestoreConsole();
        }
    }
}