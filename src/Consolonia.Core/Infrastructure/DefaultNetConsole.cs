using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Core.InternalHelpers;

namespace Consolonia.Core.Infrastructure
{
    public class DefaultNetConsole : InputLessDefaultNetConsole
    {
        private static readonly Dictionary<ConsoleKey, Key> KeyMapping = new()
        {
            { ConsoleKey.Applications, Key.Apps },
            { ConsoleKey.Attention, Key.Attn },
            { ConsoleKey.LaunchApp1, Key.LaunchApplication1 },
            { ConsoleKey.LaunchApp2, Key.LaunchApplication2 },
            { ConsoleKey.MediaNext, Key.MediaNextTrack },
            { ConsoleKey.MediaPrevious, Key.MediaPreviousTrack },
            { ConsoleKey.MediaStop, Key.MediaStop },
            { ConsoleKey.MediaPlay, Key.MediaPlayPause },
            { ConsoleKey.LaunchMediaSelect, Key.SelectMedia },
            { ConsoleKey.EraseEndOfFile, Key.EraseEof },
            { ConsoleKey.LeftWindows, Key.LWin },
            { ConsoleKey.RightWindows, Key.RWin },
            { ConsoleKey.Spacebar, Key.Space },
            { ConsoleKey.RightArrow, Key.Right },
            { ConsoleKey.LeftArrow, Key.Left },
            { ConsoleKey.UpArrow, Key.Up },
            { ConsoleKey.DownArrow, Key.Down },
            { ConsoleKey.Backspace, Key.Back },
            { (ConsoleKey)18, Key.LeftAlt }
        };

        private static readonly FlagTranslator<ConsoleModifiers, RawInputModifiers> ModifiersFlagsTranslator = new([
            (ConsoleModifiers.Control, RawInputModifiers.Control),
            (ConsoleModifiers.Shift, RawInputModifiers.Shift), (ConsoleModifiers.Alt, RawInputModifiers.Alt)
        ]);

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

        public override void PauseIO(Task task)
        {
            base.PauseIO(task);

            TextReader defaultIn = Console.In;
            Console.SetIn(new StringReader(string.Empty));
            Console.SetIn(defaultIn);
        }

        private void StartInputReading()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                while (!Disposed)
                {
                    PauseTask?.Wait();

                    ConsoleKeyInfo consoleKeyInfo;
                    try
                    {
                        consoleKeyInfo = Console.ReadKey(true);
                    }
                    catch (InvalidOperationException)
                    {
                        continue;
                    }

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

            if(!Enum.IsDefined(consoleKey))
                throw new NotImplementedException();

            if (!Enum.TryParse(consoleKey.ToString(), out key))
                throw new NotImplementedException();
            return key;
        }
    }
}