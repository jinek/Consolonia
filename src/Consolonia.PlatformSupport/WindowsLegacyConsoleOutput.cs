#pragma warning disable CA1416 // Validate platform compatibility
using System;
using System.Text;
using Consolonia.Core.Infrastructure;

namespace Consolonia.PlatformSupport
{
    /// <summary>
    ///     This creates an alternate screen buffer using windows apis,
    ///     and switches Console output to use it.
    /// </summary>
    public class WindowsLegacyConsoleOutput : DefaultNetConsoleOutput
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

        public override void RestoreConsole()
        {
            _originalBuffer.SetAsActiveBuffer();
            base.RestoreConsole();
            Flush();
        }
    }
}