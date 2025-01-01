using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Avalonia.Input;
using Consolonia.Core.InternalHelpers;

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    /// IConsole implementation which purely uses Console API 
    /// </summary>
    /// <remarks>
    /// This implements uses standard Console.ReadKey to get input and
    /// calls the base IConsoleOutput for output (or default of Console Output)
    /// </remarks>
    public class DefaultNetConsole : ConsoleBase
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
            { (ConsoleKey)18, Key.LeftAlt },
            { (ConsoleKey)16, Key.LeftShift },
            { (ConsoleKey)17, Key.LeftCtrl }
        };

        private static readonly FlagTranslator<ConsoleModifiers, RawInputModifiers> ModifiersFlagsTranslator = new([
            (ConsoleModifiers.Control, RawInputModifiers.Control),
            (ConsoleModifiers.Shift, RawInputModifiers.Shift), (ConsoleModifiers.Alt, RawInputModifiers.Alt)
        ]);

        public override bool SupportsAltSolo => true;

        public override bool SupportsMouse => false;

        public override bool SupportsMouseMove => false;

        public DefaultNetConsole()
            : base(new DefaultNetConsoleOutput())
        {
            // ReSharper disable VirtualMemberCallInConstructor
            PrepareConsole();

            StartSizeCheckTimerAsync();
            StartInputReading();
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

            if (!Enum.IsDefined(consoleKey))
                throw new NotImplementedException();

            if (!Enum.TryParse(consoleKey.ToString(), out key))
                throw new NotImplementedException();
            return key;
        }

    }
}