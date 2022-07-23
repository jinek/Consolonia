using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Avalonia.Input;
using Consolonia.Core.InternalHelpers;

namespace Consolonia.Core.Infrastructure
{
    public class DefaultNetConsole : InputLessDefaultNetConsole
    {
        private static readonly Dictionary<ConsoleKey, Key> KeyMapping = new()
        {
            { ConsoleKey.LeftWindows, Key.LWin },
            { ConsoleKey.RightWindows, Key.RWin },
            { ConsoleKey.Spacebar, Key.Space },
            { ConsoleKey.RightArrow, Key.Right },
            { ConsoleKey.LeftArrow, Key.Left },
            { ConsoleKey.UpArrow, Key.Up },
            { ConsoleKey.DownArrow, Key.Down },
            { ConsoleKey.Backspace, Key.Back }
        };

        private static readonly FlagTranslator<ConsoleModifiers, RawInputModifiers> ModifiersFlagsTranslator = new(new[]
        {
            (ConsoleModifiers.Control, RawInputModifiers.Control),
            (ConsoleModifiers.Shift, RawInputModifiers.Shift), (ConsoleModifiers.Alt, RawInputModifiers.Alt)
        });

        public DefaultNetConsole()
        {
            StartSizeCheckTimerAsync();
            StartInputReading();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            RaiseFocusEvent(false);
        }

        private void StartInputReading()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                while (!Disposed)
                {
                    ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);

                    Key key = ConvertToKey(consoleKeyInfo.Key);

                    RawInputModifiers rawInputModifiers = ModifiersFlagsTranslator.Translate(consoleKeyInfo.Modifiers);

                    RaiseKeyPress(key, consoleKeyInfo.KeyChar, rawInputModifiers, true,
                        (ulong)Stopwatch.GetTimestamp());
                    Thread.Yield();
                    RaiseKeyPress(key, consoleKeyInfo.KeyChar, rawInputModifiers, false,
                        (ulong)Stopwatch.GetTimestamp());
                    Thread.Yield();
                }
            });
        }

        public static Key ConvertToKey(ConsoleKey consoleKey)
        {
            if (KeyMapping.TryGetValue(consoleKey, out Key key)) return key;

            if (!Enum.TryParse(consoleKey.ToString(), out key))
                throw new NotImplementedException();
            return key;
        }
    }
}