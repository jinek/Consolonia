// ReSharper disable All
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Vanara.PInvoke;
using static Vanara.PInvoke.Kernel32;

namespace Terminal.Gui
{
    [SupportedOSPlatform("windows")]
    internal class WindowsConsole
    {

        internal HFILE InputHandle;
        readonly CONSOLE_INPUT_MODE originalConsoleMode;

        public WindowsConsole()
        {
            InputHandle = GetStdHandle(StdHandleType.STD_INPUT_HANDLE);
            originalConsoleMode = ConsoleMode;
            var newConsoleMode = originalConsoleMode;
            newConsoleMode |= (CONSOLE_INPUT_MODE.ENABLE_MOUSE_INPUT |
                               CONSOLE_INPUT_MODE.ENABLE_EXTENDED_FLAGS);
            newConsoleMode &= ~CONSOLE_INPUT_MODE.ENABLE_QUICK_EDIT_MODE;
            newConsoleMode &= ~CONSOLE_INPUT_MODE.ENABLE_PROCESSED_INPUT;
            ConsoleMode = newConsoleMode;
        }

        public CONSOLE_INPUT_MODE ConsoleMode
        {
            get
            {
                if (!GetConsoleMode(InputHandle, out CONSOLE_INPUT_MODE v))
                    throw GetLastError().GetException();
                return v;
            }
            set
            {
                if (!SetConsoleMode(InputHandle, value))
                    throw GetLastError().GetException();
            }
        }

        public INPUT_RECORD[] ReadConsoleInput()
        {
            const int bufferSize = 1;
            var records = new INPUT_RECORD[bufferSize];

            if (!Kernel32.ReadConsoleInput(InputHandle, records, bufferSize,
                out var numberEventsRead))
                throw GetLastError().GetException();

            return numberEventsRead == 0
                ? Array.Empty<INPUT_RECORD>()
                : records;
        }
    }
}