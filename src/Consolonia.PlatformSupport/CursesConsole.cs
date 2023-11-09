//This file is partly copypaste (idea is very same) from:
//
// Driver.cs: Curses-based Driver
//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Consolonia.Core.Infrastructure;
using Consolonia.Core.InternalHelpers;
using Unix.Terminal;
using Key = Terminal.Gui.Key;
using KeyModifiers = Terminal.Gui.KeyModifiers;

namespace Consolonia.PlatformSupport
{
    public class CursesConsole : InputLessDefaultNetConsole
    {
        private static readonly FlagTranslator<Key, RawInputModifiers>
            KeyModifiersFlagTranslator = new(new[]
            {
                (Key.ShiftMask, RawInputModifiers.Shift),
                (Key.AltMask, RawInputModifiers.Alt),
                (Key.CtrlMask, RawInputModifiers.Control),
                (Key.BackTab, RawInputModifiers.Shift)
            });

        private static readonly FlagTranslator<Key, ConsoleKey>
            KeyFlagTranslator = new(new[]
            {
                (Key.BackTab, ConsoleKey.Tab),
                (Key.Backspace, ConsoleKey.Backspace),
                (Key.CursorDown, ConsoleKey.DownArrow),
                (Key.CursorLeft, ConsoleKey.LeftArrow),
                (Key.CursorRight, ConsoleKey.RightArrow),
                (Key.CursorUp, ConsoleKey.UpArrow),
                (Key.DeleteChar, ConsoleKey.Delete),
                (Key.Delete, ConsoleKey.Delete),
                (Key.End, ConsoleKey.End),
                (Key.Enter, ConsoleKey.Enter),
                (Key.Esc, ConsoleKey.Escape),
                (Key.F1, ConsoleKey.F1),
                (Key.F2, ConsoleKey.F2),
                (Key.F3, ConsoleKey.F3),
                (Key.F4, ConsoleKey.F4),
                (Key.F5, ConsoleKey.F5),
                (Key.F6, ConsoleKey.F6),
                (Key.F7, ConsoleKey.F7),
                (Key.F8, ConsoleKey.F8),
                (Key.F9, ConsoleKey.F9),
                (Key.F10, ConsoleKey.F10),
                (Key.F11, ConsoleKey.F11),
                (Key.F12, ConsoleKey.F12),
                (Key.Home, ConsoleKey.Home),
                (Key.InsertChar, ConsoleKey.Insert),
                (Key.PageDown, ConsoleKey.PageDown),
                (Key.PageUp, ConsoleKey.PageUp),
                (Key.Space, ConsoleKey.Spacebar),
                (Key.Tab, ConsoleKey.Tab),
                // Proposed by ChatGPT, I've found supporting source: https://devblogs.microsoft.com/dotnet/console-readkey-improvements-in-net-7/   
                (Key.Unknown, ConsoleKey.NoName)
            });

        private static readonly FlagTranslator<Curses.Event, RawInputModifiers>
            MouseModifiersFlagTranslator = new(new[]
            {
                (Curses.Event.ButtonAlt, RawInputModifiers.Alt),
                (Curses.Event.ButtonCtrl, RawInputModifiers.Control),
                (Curses.Event.ButtonShift, RawInputModifiers.Shift)
            });

        private static readonly FlagTranslator<Curses.Event, RawPointerEventType>
            MouseEventFlagTranslator = new(new[]
            {
                (Curses.Event.Button1Pressed, RawPointerEventType.LeftButtonDown),
                (Curses.Event.Button1Clicked, RawPointerEventType.LeftButtonDown),
                (Curses.Event.Button1Released, RawPointerEventType.LeftButtonUp),
                (Curses.Event.Button2Pressed, RawPointerEventType.RightButtonDown),
                (Curses.Event.Button2Clicked, RawPointerEventType.RightButtonDown),
                (Curses.Event.Button2Released, RawPointerEventType.RightButtonUp),
                (Curses.Event.Button3Pressed, RawPointerEventType.MiddleButtonDown),
                (Curses.Event.Button3Clicked, RawPointerEventType.MiddleButtonDown),
                (Curses.Event.Button3Released, RawPointerEventType.MiddleButtonUp),
                (Curses.Event.Button4Pressed, RawPointerEventType.XButton1Down),
                (Curses.Event.Button4Clicked, RawPointerEventType.XButton1Down),
                (Curses.Event.Button4Released, RawPointerEventType.XButton1Up),
                (Curses.Event.ReportMousePosition, RawPointerEventType.Move),
                (Curses.Event.ButtonWheeledDown, RawPointerEventType.Wheel),
                (Curses.Event.ButtonWheeledUp, RawPointerEventType.Wheel)
            });

        private KeyModifiers _keyModifiers;

        public CursesConsole()
        {
            StartSizeCheckTimerAsync(2500);
            StartEventLoop();
        }

        private void StartEventLoop()
        {
            //todo: cleanup
            Curses.initscr();
            Curses.noecho();
            Curses.cbreak();
            Curses.doupdate();
            Curses.raw();
            Curses.Window.Standard.keypad(true);
            Curses.mousemask(
                Curses.Event.AllEvents | Curses.Event.ReportMousePosition,
                out Curses.Event _);
            /*Console.Out.Write("\x1b[?1003h");
            Console.Out.Flush();*/

            Task _ = Task.Run(() =>
            {
                while (!Disposed) ProcessInput();
            });
        }

        ///this is copied from CursesDriver.cs -> ProcessInput
        private void ProcessInput()
        {
            int code = Curses.get_wch(out int wch);
            if (code == Curses.ERR)
                return;

            _keyModifiers = new KeyModifiers();
            var k = Key.Unknown;

            if (code == Curses.KEY_CODE_YES)
            {
                switch (wch)
                {
                    case Curses.KeyResize when Curses.CheckWinChange():
                        CheckActualizeTheSize();
                        return;
                    case Curses.KeyMouse:
                        Curses.getmouse(out Curses.MouseEvent ev);
                        HandleMouseInput(ev);
                        return;
                }

                k = MapCursesKey(wch);

                switch (wch)
                {
                    case >= 277 and <= 288:
                        // Shift+(F1 - F12)
                        wch -= 12;
                        k = Key.ShiftMask | MapCursesKey(wch);
                        break;
                    case >= 289 and <= 300:
                        // Ctrl+(F1 - F12)
                        wch -= 24;
                        k = Key.CtrlMask | MapCursesKey(wch);
                        break;
                    case >= 301 and <= 312:
                        // Ctrl+Shift+(F1 - F12)
                        wch -= 36;
                        k = Key.CtrlMask | Key.ShiftMask | MapCursesKey(wch);
                        break;
                    case >= 313 and <= 324:
                        // Alt+(F1 - F12)
                        wch -= 48;
                        k = Key.AltMask | MapCursesKey(wch);
                        break;
                    case >= 325 and <= 327:
                        // Shift+Alt+(F1 - F3)
                        wch -= 60;
                        k = Key.ShiftMask | Key.AltMask | MapCursesKey(wch);
                        break;
                }

                RaiseKeyPressInternal(k);
                return;
            }

            switch (wch)
            {
                // Special handling for ESC, we want to try to catch ESC+letter to simulate alt-letter as well as Alt-Fkey
                case 27:
                {
                    Curses.timeout(200);

                    code = Curses.get_wch(out int wch2);

                    if (code == Curses.KEY_CODE_YES) k = Key.AltMask | MapCursesKey(wch);

                    if (code == 0)
                    {
                        //KeyEvent key;

                        // The ESC-number handling, debatable.
                        // Simulates the AltMask itself by pressing Alt + Space.
                        if (wch2 == (int)Key.Space)
                            k = Key.AltMask;
                        else if (wch2 - (int)Key.Space >= (uint)Key.A && wch2 - (int)Key.Space <= (uint)Key.Z)
                            k = (Key)((uint)Key.AltMask + (wch2 - (int)Key.Space));
                        else if (wch2 >= (uint)Key.A - 64 && wch2 <= (uint)Key.Z - 64)
                            k = (Key)((uint)(Key.AltMask | Key.CtrlMask) + (wch2 + 64));
                        else if (wch2 >= (uint)Key.D0 && wch2 <= (uint)Key.D9)
                            k = (Key)((uint)Key.AltMask + (uint)Key.D0 + (wch2 - (uint)Key.D0));
                        else
                            switch (wch2)
                            {
                                case 27:
                                    k = (Key)wch2;
                                    break;
                                case Curses.KEY_CODE_SEQ:
                                {
                                    int[] c = null;
                                    while (code == 0)
                                    {
                                        code = Curses.get_wch(out wch2);
                                        if (wch2 <= 0) continue;
                                        int length = 1;
                                        if (c != null)
                                            length += c.Length;
                                        Array.Resize(ref c, length);

                                        c[^1] = wch2;
                                    }

                                    switch (c![0])
                                    {
                                        case 49 when c[1] == 59 && c[2] == 55 && c[3] >= 80 && c[3] <= 83:
                                            // Ctrl+Alt+(F1 - F4)
                                            wch2 = c[3] + 185;
                                            k = Key.CtrlMask | Key.AltMask | MapCursesKey(wch2);
                                            break;
                                        case 49
                                            when c[2] == 59 && c[3] == 55 && c[4] == 126 && c[1] >= 53 && c[1] <= 57:
                                            // Ctrl+Alt+(F5 - F8)
                                            wch2 = c[1] == 53 ? c[1] + 216 : c[1] + 215;
                                            k = Key.CtrlMask | Key.AltMask | MapCursesKey(wch2);
                                            break;
                                        case 50
                                            when c[2] == 59 && c[3] == 55 && c[4] == 126 && c[1] >= 48 && c[1] <= 52:
                                            // Ctrl+Alt+(F9 - F12)
                                            wch2 = c[1] < 51 ? c[1] + 225 : c[1] + 224;
                                            k = Key.CtrlMask | Key.AltMask | MapCursesKey(wch2);
                                            break;
                                        case 49 when c[1] == 59 && c[2] == 56 && c[3] >= 80 && c[3] <= 83:
                                            // Ctrl+Shift+Alt+(F1 - F4)
                                            wch2 = c[3] + 185;
                                            k = Key.CtrlMask | Key.ShiftMask | Key.AltMask | MapCursesKey(wch2);
                                            break;
                                        case 49
                                            when c[2] == 59 && c[3] == 56 && c[4] == 126 && c[1] >= 53 && c[1] <= 57:
                                            // Ctrl+Shift+Alt+(F5 - F8)
                                            wch2 = c[1] == 53 ? c[1] + 216 : c[1] + 215;
                                            k = Key.CtrlMask | Key.ShiftMask | Key.AltMask | MapCursesKey(wch2);
                                            break;
                                        case 50
                                            when c[2] == 59 && c[3] == 56 && c[4] == 126 && c[1] >= 48 && c[1] <= 52:
                                            // Ctrl+Shift+Alt+(F9 - F12)
                                            wch2 = c[1] < 51 ? c[1] + 225 : c[1] + 224;
                                            k = Key.CtrlMask | Key.ShiftMask | Key.AltMask | MapCursesKey(wch2);
                                            break;
                                        case 49 when c[1] == 59 && c[2] == 52 && c[3] == 83:
                                            // Shift+Alt+(F4)
                                            wch2 = 268;
                                            k = Key.ShiftMask | Key.AltMask | MapCursesKey(wch2);
                                            break;
                                        case 49
                                            when c[2] == 59 && c[3] == 52 && c[4] == 126 && c[1] >= 53 && c[1] <= 57:
                                            // Shift+Alt+(F5 - F8)
                                            wch2 = c[1] < 55 ? c[1] + 216 : c[1] + 215;
                                            k = Key.ShiftMask | Key.AltMask | MapCursesKey(wch2);
                                            break;
                                        case 50
                                            when c[2] == 59 && c[3] == 52 && c[4] == 126 && c[1] >= 48 && c[1] <= 52:
                                            // Shift+Alt+(F9 - F12)
                                            wch2 = c[1] < 51 ? c[1] + 225 : c[1] + 224;
                                            k = Key.ShiftMask | Key.AltMask | MapCursesKey(wch2);
                                            break;
                                        case 54 when c[1] == 59 && c[2] == 56 && c[3] == 126:
                                            // Shift+Ctrl+Alt+KeyNPage
                                            k = Key.ShiftMask | Key.CtrlMask | Key.AltMask | Key.PageDown;
                                            break;
                                        case 53 when c[1] == 59 && c[2] == 56 && c[3] == 126:
                                            // Shift+Ctrl+Alt+KeyPPage
                                            k = Key.ShiftMask | Key.CtrlMask | Key.AltMask | Key.PageUp;
                                            break;
                                        case 49 when c[1] == 59 && c[2] == 56 && c[3] == 72:
                                            // Shift+Ctrl+Alt+KeyHome
                                            k = Key.ShiftMask | Key.CtrlMask | Key.AltMask | Key.Home;
                                            break;
                                        case 49 when c[1] == 59 && c[2] == 56 && c[3] == 70:
                                            // Shift+Ctrl+Alt+KeyEnd
                                            k = Key.ShiftMask | Key.CtrlMask | Key.AltMask | Key.End;
                                            break;
                                        default:
                                            k = MapCursesKey(wch2);
                                            break;
                                    }

                                    break;
                                }
                                default:
                                {
                                    // Unfortunately there are no way to differentiate Ctrl+Alt+alfa and Ctrl+Shift+Alt+alfa.
                                    if (((Key)wch2 & Key.CtrlMask) != 0) _keyModifiers.Ctrl = true;

                                    if (wch2 == 0)
                                    {
                                        k = Key.CtrlMask | Key.AltMask | Key.Space;
                                    }
                                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse todo: check why
                                    else if (wch >= (uint)Key.A && wch <= (uint)Key.Z)
                                    {
                                        _keyModifiers.Shift = true;
                                        _keyModifiers.Alt = true;
                                    }
                                    else if (wch2 < 256)
                                    {
                                        k = (Key)wch2;
                                        _keyModifiers.Alt = true;
                                    }
                                    else
                                    {
                                        k = (Key)((uint)(Key.AltMask | Key.CtrlMask) + wch2);
                                    }

                                    break;
                                }
                            }

                        RaiseKeyPressInternal(k);
                    }
                    else
                    {
                        k = Key.Esc;
                        RaiseKeyPressInternal(k);
                    }

                    break;
                }
                case Curses.KeyTab:
                    k = MapCursesKey(wch);
                    RaiseKeyPressInternal(k);
                    break;
                default:
                {
                    // Unfortunately there are no way to differentiate Ctrl+alfa and Ctrl+Shift+alfa.
                    k = (Key)wch;
                    if (wch == 0)
                    {
                        k = Key.CtrlMask | Key.Space;
                    }
                    else if (wch >= (uint)Key.A - 64 && wch <= (uint)Key.Z - 64)
                    {
                        if ((Key)(wch + 64) != Key.J) k = Key.CtrlMask | (Key)(wch + 64);
                    }
                    else if (wch >= (uint)Key.A && wch <= (uint)Key.Z)
                    {
                        _keyModifiers.Shift = true;
                    }

                    RaiseKeyPressInternal(k);
                    break;
                }
            }
        }


        private void RaiseKeyPressInternal(Key key)
        {
            int keyValue = (int)key;
            RawInputModifiers modifiers = KeyModifiersFlagTranslator.Translate(key);
            char character;
            ConsoleKey consoleKey =
                KeyFlagTranslator.Translate(key & ~Key.CtrlMask & ~Key.ShiftMask & ~Key.AltMask, true);
            if (consoleKey == default)
            {
                bool _ = Enum.TryParse(key.ToString(), true, out consoleKey);
            }

            if (((uint)keyValue & (uint)Key.CharMask) > 27)
            {
                character = (char)keyValue;
            }
            else
            {
                if (consoleKey == default)
                    throw new NotImplementedException();
                character = char.MinValue;
                if (char.IsUpper(character))
                    modifiers |= RawInputModifiers.Shift;
            }

            Avalonia.Input.Key convertToKey = DefaultNetConsole.ConvertToKey(consoleKey);

            RaiseKeyPress(convertToKey,
                character, modifiers, true, (ulong)Stopwatch.GetTimestamp());
            Thread.Yield();
            RaiseKeyPress(convertToKey,
                character, modifiers, false, (ulong)Stopwatch.GetTimestamp());
        }

        private void HandleMouseInput(Curses.MouseEvent ev)
        {
            const double velocity = 1 / 12D;

            RawInputModifiers rawInputModifiers = MouseModifiersFlagTranslator.Translate(ev.ButtonState);

            foreach (Curses.Event flag in ev.ButtonState.GetFlags())
            {
                RawPointerEventType rawPointerEventType = MouseEventFlagTranslator.Translate(flag);
                if (rawPointerEventType == 0) continue;

                Vector? wheelDelta = null;
                if (ev.ButtonState.HasFlag(Curses.Event.ButtonWheeledDown))
                    wheelDelta = new Vector(0, -velocity);
                if (ev.ButtonState.HasFlag(Curses.Event.ButtonWheeledUp))
                    wheelDelta = new Vector(0, velocity);

                RaiseMouseEvent(rawPointerEventType, new Point(ev.X, ev.Y), wheelDelta, rawInputModifiers);
                if (flag is not (Curses.Event.Button1Clicked or Curses.Event.Button2Clicked
                    or Curses.Event.Button3Clicked
                    or Curses.Event.Button4Clicked)) continue;
                Thread.Yield();
                RaiseMouseEvent(rawPointerEventType + 1, new Point(ev.X, ev.Y), null, rawInputModifiers);
            }
        }

        private static Key MapCursesKey(int cursesKey)
        {
            switch (cursesKey)
            {
                case Curses.KeyF1: return Key.F1;
                case Curses.KeyF2: return Key.F2;
                case Curses.KeyF3: return Key.F3;
                case Curses.KeyF4: return Key.F4;
                case Curses.KeyF5: return Key.F5;
                case Curses.KeyF6: return Key.F6;
                case Curses.KeyF7: return Key.F7;
                case Curses.KeyF8: return Key.F8;
                case Curses.KeyF9: return Key.F9;
                case Curses.KeyF10: return Key.F10;
                case Curses.KeyF11: return Key.F11;
                case Curses.KeyF12: return Key.F12;
                case Curses.KeyUp: return Key.CursorUp;
                case Curses.KeyDown: return Key.CursorDown;
                case Curses.KeyLeft: return Key.CursorLeft;
                case Curses.KeyRight: return Key.CursorRight;
                case Curses.KeyHome: return Key.Home;
                case Curses.KeyEnd: return Key.End;
                case Curses.KeyNPage: return Key.PageDown;
                case Curses.KeyPPage: return Key.PageUp;
                case Curses.KeyDeleteChar: return Key.DeleteChar;
                case Curses.KeyInsertChar: return Key.InsertChar;
                case Curses.KeyTab: return Key.Tab;
                case Curses.KeyBackTab: return Key.BackTab;
                case Curses.KeyBackspace: return Key.Backspace;
                case Curses.ShiftKeyUp: return Key.CursorUp | Key.ShiftMask;
                case Curses.ShiftKeyDown: return Key.CursorDown | Key.ShiftMask;
                case Curses.ShiftKeyLeft: return Key.CursorLeft | Key.ShiftMask;
                case Curses.ShiftKeyRight: return Key.CursorRight | Key.ShiftMask;
                case Curses.ShiftKeyHome: return Key.Home | Key.ShiftMask;
                case Curses.ShiftKeyEnd: return Key.End | Key.ShiftMask;
                case Curses.ShiftKeyNPage: return Key.PageDown | Key.ShiftMask;
                case Curses.ShiftKeyPPage: return Key.PageUp | Key.ShiftMask;
                case Curses.AltKeyUp: return Key.CursorUp | Key.AltMask;
                case Curses.AltKeyDown: return Key.CursorDown | Key.AltMask;
                case Curses.AltKeyLeft: return Key.CursorLeft | Key.AltMask;
                case Curses.AltKeyRight: return Key.CursorRight | Key.AltMask;
                case Curses.AltKeyHome: return Key.Home | Key.AltMask;
                case Curses.AltKeyEnd: return Key.End | Key.AltMask;
                case Curses.AltKeyNPage: return Key.PageDown | Key.AltMask;
                case Curses.AltKeyPPage: return Key.PageUp | Key.AltMask;
                case Curses.CtrlKeyUp: return Key.CursorUp | Key.CtrlMask;
                case Curses.CtrlKeyDown: return Key.CursorDown | Key.CtrlMask;
                case Curses.CtrlKeyLeft: return Key.CursorLeft | Key.CtrlMask;
                case Curses.CtrlKeyRight: return Key.CursorRight | Key.CtrlMask;
                case Curses.CtrlKeyHome: return Key.Home | Key.CtrlMask;
                case Curses.CtrlKeyEnd: return Key.End | Key.CtrlMask;
                case Curses.CtrlKeyNPage: return Key.PageDown | Key.CtrlMask;
                case Curses.CtrlKeyPPage: return Key.PageUp | Key.CtrlMask;
                case Curses.ShiftCtrlKeyUp: return Key.CursorUp | Key.ShiftMask | Key.CtrlMask;
                case Curses.ShiftCtrlKeyDown: return Key.CursorDown | Key.ShiftMask | Key.CtrlMask;
                case Curses.ShiftCtrlKeyLeft: return Key.CursorLeft | Key.ShiftMask | Key.CtrlMask;
                case Curses.ShiftCtrlKeyRight: return Key.CursorRight | Key.ShiftMask | Key.CtrlMask;
                case Curses.ShiftCtrlKeyHome: return Key.Home | Key.ShiftMask | Key.CtrlMask;
                case Curses.ShiftCtrlKeyEnd: return Key.End | Key.ShiftMask | Key.CtrlMask;
                case Curses.ShiftCtrlKeyNPage: return Key.PageDown | Key.ShiftMask | Key.CtrlMask;
                case Curses.ShiftCtrlKeyPPage: return Key.PageUp | Key.ShiftMask | Key.CtrlMask;
                case Curses.ShiftAltKeyUp: return Key.CursorUp | Key.ShiftMask | Key.AltMask;
                case Curses.ShiftAltKeyDown: return Key.CursorDown | Key.ShiftMask | Key.AltMask;
                case Curses.ShiftAltKeyLeft: return Key.CursorLeft | Key.ShiftMask | Key.AltMask;
                case Curses.ShiftAltKeyRight: return Key.CursorRight | Key.ShiftMask | Key.AltMask;
                case Curses.ShiftAltKeyNPage: return Key.PageDown | Key.ShiftMask | Key.AltMask;
                case Curses.ShiftAltKeyPPage: return Key.PageUp | Key.ShiftMask | Key.AltMask;
                case Curses.ShiftAltKeyHome: return Key.Home | Key.ShiftMask | Key.AltMask;
                case Curses.ShiftAltKeyEnd: return Key.End | Key.ShiftMask | Key.AltMask;
                case Curses.AltCtrlKeyNPage: return Key.PageDown | Key.AltMask | Key.CtrlMask;
                case Curses.AltCtrlKeyPPage: return Key.PageUp | Key.AltMask | Key.CtrlMask;
                case Curses.AltCtrlKeyHome: return Key.Home | Key.AltMask | Key.CtrlMask;
                case Curses.AltCtrlKeyEnd: return Key.End | Key.AltMask | Key.CtrlMask;
                default: return Key.Unknown;
            }
        }
    }
}