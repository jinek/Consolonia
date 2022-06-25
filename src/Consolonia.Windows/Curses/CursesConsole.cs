using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Consolonia.Core.Infrastructure;
using Consolonia.Core.InternalHelpers;
using Terminal.Gui;
using Key = Terminal.Gui.Key;
using KeyModifiers = Terminal.Gui.KeyModifiers;

namespace Consolonia.Windows.Curses
{
    public class CursesConsole : InputLessDefaultNetConsole
    {
        //private UnixMainLoop _unixMainLoop;

        public CursesConsole()
        {
          //  _unixMainLoop = new UnixMainLoop();
            StartEventLoop();
            //https://docs.python.org/3/howto/curses.html
        }

        private void StartEventLoop()
        {
            //https://docs.python.org/3/library/curses.html
            var window = Unix.Terminal.Curses.initscr();
            Unix.Terminal.Curses.noecho ();
            Unix.Terminal.Curses.cbreak();
            
            Unix.Terminal.Curses.doupdate();
            
            Unix.Terminal.Curses.raw ();
            

            Unix.Terminal.Curses.Window.Standard.keypad (true);

            Unix.Terminal.Curses.Event oldMouseEvents;
            var reportableMouseEvents = Unix.Terminal.Curses.mousemask (Unix.Terminal.Curses.Event.AllEvents | Unix.Terminal.Curses.Event.ReportMousePosition, out oldMouseEvents);

            /*Console.Out.Write ("\x1b[?1003h");
            Console.Out.Flush ();*/
            
            //_unixMainLoop.Setup();

            /*_unixMainLoop.AddWatch(0, UnixMainLoop.Condition.PollIn, () =>
            {
                ProcessInput();
                return true;
            });*/
            
            /*.WinC_unixMainLoophanged += () => {
                if (Curses.CheckWinChange ()) {
                    Clip = new Rect (0, 0, Cols, Rows);
                    UpdateOffScreen ();
                    TerminalResized?.Invoke ();
                }
            };*/

            Task _ = Task.Run(() =>
            {
                while (!Disposed)
                {
                    //_unixMainLoop.EventsPending(true);
                    //Thread.Sleep(10);
                    //_unixMainLoop.MainIteration();
                    ProcessInput();
                    //ProcessInput2();
                }
            });
        }

        /*[MethodImpl(MethodImplOptions.Synchronized)]
        private void CancelWaiting(bool raise)
        {
            if (_waitingMoreKeys != null)
            {
                _waitingMoreKeys?.Cancel(true);
                _waitingMoreKeys = null;
            }
            
            if(raise)
                RaiseKeyPressInternal(new ConsoleKeyInfo(char.MinValue, ConsoleKey.Escape, false, false, false));
        }
        
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void ProcessInput2()
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            Debug.WriteLine($"{keyInfo.Key} ({keyInfo.KeyChar}) {string.Join(", ",keyInfo.Modifiers.GetFlags().Select(modifiers => modifiers.ToString()))}");
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                CancelWaiting(true);
                _waitingMoreKeys = new CancellationTokenSource();
                Task.Delay(200 /*copied#1#, _waitingMoreKeys.Token)
                    .ContinueWith(task => CancelWaiting(task.IsCompleted));
            }
            else
            {
                if (_waitingMoreKeys != null)
                {
                    CancelWaiting(false);
                    if (keyInfo.Key == ConsoleKey.M)
                    {
                        //ProcessMouse();
                        ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                        Unix.Terminal.Curses.getmouse(out Unix.Terminal.Curses.MouseEvent ev);
                        
                        System.Diagnostics.Debug.WriteLine ($"ButtonState: {ev.ButtonState}; ID: {ev.ID}; X: {ev.X}; Y: {ev.Y}; Z: {ev.Z}");
                        HandleMouseInput(ev);
                        return;
                    }
                }

                //throw new NotImplementedException();
            }
        }*/

        /*
        private void ProcessMouse()
        {
            var mouseCode = Console.ReadKey(true);
            byte x = (byte)Console.ReadKey(true).KeyChar;
            byte y = (byte)Console.ReadKey(true).KeyChar;
            
            Debug.WriteLine($"Mouse: {mouseCode.Key} ({mouseCode.KeyChar}) {x}x{y}  modifiders: {string.Join(", ",mouseCode.Modifiers.GetFlags().Select(modifiers => modifiers.ToString()))}");
        }
        */

        KeyModifiers keyModifiers;

        private void ProcessInput()
        {
            /*var consoleKeyInfo = Console.ReadKey(true);
            Debug.WriteLine($"{consoleKeyInfo} {consoleKeyInfo.Key} {consoleKeyInfo.Modifiers} {consoleKeyInfo.KeyChar}");
            if (consoleKeyInfo.Key != ((ConsoleKey)414142)) return;*/
            int wch;
            int code = Unix.Terminal.Curses.get_wch(out wch);
            //int code = Unix.Terminal.Curses.A_NORMAL;
            //wch = Console.ReadKey(true).KeyChar;
            //System.Diagnostics.Debug.WriteLine ($"code: {code}; wch: {wch}");
            if (code == Unix.Terminal.Curses.ERR)
                return;

            keyModifiers = new KeyModifiers();
            Key k = Key.Unknown;

            if (code == Unix.Terminal.Curses.KEY_CODE_YES)
            {
                if (wch == Unix.Terminal.Curses.KeyResize)
                {
                    if (Unix.Terminal.Curses.CheckWinChange())
                    {
                        ActualizeTheSize();
                        return;
                    }
                }

                if (wch == Unix.Terminal.Curses.KeyMouse)
                {
                    Unix.Terminal.Curses.getmouse(out Unix.Terminal.Curses.MouseEvent ev);
                    //System.Diagnostics.Debug.WriteLine ($"ButtonState: {ev.ButtonState}; ID: {ev.ID}; X: {ev.X}; Y: {ev.Y}; Z: {ev.Z}");
                    HandleMouseInput(ev);
                    return;
                }

                k = MapCursesKey(wch);
                
                if (wch >= 277 && wch <= 288)
                {
                    // Shift+(F1 - F12)
                    wch -= 12;
                    k = Key.ShiftMask | MapCursesKey(wch);
                }
                else if (wch >= 289 && wch <= 300)
                {
                    // Ctrl+(F1 - F12)
                    wch -= 24;
                    k = Key.CtrlMask | MapCursesKey(wch);
                }
                else if (wch >= 301 && wch <= 312)
                {
                    // Ctrl+Shift+(F1 - F12)
                    wch -= 36;
                    k = Key.CtrlMask | Key.ShiftMask | MapCursesKey(wch);
                }
                else if (wch >= 313 && wch <= 324)
                {
                    // Alt+(F1 - F12)
                    wch -= 48;
                    k = Key.AltMask | MapCursesKey(wch);
                }
                else if (wch >= 325 && wch <= 327)
                {
                    // Shift+Alt+(F1 - F3)
                    wch -= 60;
                    k = Key.ShiftMask | Key.AltMask | MapCursesKey(wch);
                }

                Debug.WriteLine(string.Join(" - ",k.GetFlags().Select(flag=>flag.ToString())));
                RaiseKeyPressInternal(k);
                //todo: keyDownHandler(new KeyEvent(k, MapKeyModifiers(k)));
                //todo: keyHandler(new KeyEvent(k, MapKeyModifiers(k)));
                //todo: keyUpHandler(new KeyEvent(k, MapKeyModifiers(k)));
                return;
            }

            // Special handling for ESC, we want to try to catch ESC+letter to simulate alt-letter as well as Alt-Fkey
            if (wch == 27)
            {
                Unix.Terminal.Curses.timeout(200);

                code = Unix.Terminal.Curses.get_wch(out int wch2);

                if (code == Unix.Terminal.Curses.KEY_CODE_YES)
                {
                    k = Key.AltMask | MapCursesKey(wch);
                }

                if (code == 0)
                {
                    //KeyEvent key;

                    // The ESC-number handling, debatable.
                    // Simulates the AltMask itself by pressing Alt + Space.
                    if (wch2 == (int)Key.Space)
                    {
                        k = Key.AltMask;
                    }
                    else if (wch2 - (int)Key.Space >= (uint)Key.A && wch2 - (int)Key.Space <= (uint)Key.Z)
                    {
                        k = (Key)((uint)Key.AltMask + (wch2 - (int)Key.Space));
                    }
                    else if (wch2 >= (uint)Key.A - 64 && wch2 <= (uint)Key.Z - 64)
                    {
                        k = (Key)((uint)(Key.AltMask | Key.CtrlMask) + (wch2 + 64));
                    }
                    else if (wch2 >= (uint)Key.D0 && wch2 <= (uint)Key.D9)
                    {
                        k = (Key)((uint)Key.AltMask + (uint)Key.D0 + (wch2 - (uint)Key.D0));
                    }
                    else if (wch2 == 27)
                    {
                        k = (Key)wch2;
                    }
                    else if (wch2 == Unix.Terminal.Curses.KEY_CODE_SEQ)
                    {
                        int[] c = null;
                        while (code == 0)
                        {
                            code = Unix.Terminal.Curses.get_wch(out wch2);
                            if (wch2 > 0)
                            {
                                Array.Resize(ref c, c == null ? 1 : c.Length + 1);
                                c[c.Length - 1] = wch2;
                            }
                        }

                        if (c[0] == 49 && c[1] == 59 && c[2] == 55 && c[3] >= 80 && c[3] <= 83)
                        {
                            // Ctrl+Alt+(F1 - F4)
                            wch2 = c[3] + 185;
                            k = Key.CtrlMask | Key.AltMask | MapCursesKey(wch2);
                        }
                        else if (c[0] == 49 && c[2] == 59 && c[3] == 55 && c[4] == 126 && c[1] >= 53 && c[1] <= 57)
                        {
                            // Ctrl+Alt+(F5 - F8)
                            wch2 = c[1] == 53 ? c[1] + 216 : c[1] + 215;
                            k = Key.CtrlMask | Key.AltMask | MapCursesKey(wch2);
                        }
                        else if (c[0] == 50 && c[2] == 59 && c[3] == 55 && c[4] == 126 && c[1] >= 48 && c[1] <= 52)
                        {
                            // Ctrl+Alt+(F9 - F12)
                            wch2 = c[1] < 51 ? c[1] + 225 : c[1] + 224;
                            k = Key.CtrlMask | Key.AltMask | MapCursesKey(wch2);
                        }
                        else if (c[0] == 49 && c[1] == 59 && c[2] == 56 && c[3] >= 80 && c[3] <= 83)
                        {
                            // Ctrl+Shift+Alt+(F1 - F4)
                            wch2 = c[3] + 185;
                            k = Key.CtrlMask | Key.ShiftMask | Key.AltMask | MapCursesKey(wch2);
                        }
                        else if (c[0] == 49 && c[2] == 59 && c[3] == 56 && c[4] == 126 && c[1] >= 53 && c[1] <= 57)
                        {
                            // Ctrl+Shift+Alt+(F5 - F8)
                            wch2 = c[1] == 53 ? c[1] + 216 : c[1] + 215;
                            k = Key.CtrlMask | Key.ShiftMask | Key.AltMask | MapCursesKey(wch2);
                        }
                        else if (c[0] == 50 && c[2] == 59 && c[3] == 56 && c[4] == 126 && c[1] >= 48 && c[1] <= 52)
                        {
                            // Ctrl+Shift+Alt+(F9 - F12)
                            wch2 = c[1] < 51 ? c[1] + 225 : c[1] + 224;
                            k = Key.CtrlMask | Key.ShiftMask | Key.AltMask | MapCursesKey(wch2);
                        }
                        else if (c[0] == 49 && c[1] == 59 && c[2] == 52 && c[3] == 83)
                        {
                            // Shift+Alt+(F4)
                            wch2 = 268;
                            k = Key.ShiftMask | Key.AltMask | MapCursesKey(wch2);
                        }
                        else if (c[0] == 49 && c[2] == 59 && c[3] == 52 && c[4] == 126 && c[1] >= 53 && c[1] <= 57)
                        {
                            // Shift+Alt+(F5 - F8)
                            wch2 = c[1] < 55 ? c[1] + 216 : c[1] + 215;
                            k = Key.ShiftMask | Key.AltMask | MapCursesKey(wch2);
                        }
                        else if (c[0] == 50 && c[2] == 59 && c[3] == 52 && c[4] == 126 && c[1] >= 48 && c[1] <= 52)
                        {
                            // Shift+Alt+(F9 - F12)
                            wch2 = c[1] < 51 ? c[1] + 225 : c[1] + 224;
                            k = Key.ShiftMask | Key.AltMask | MapCursesKey(wch2);
                        }
                        else if (c[0] == 54 && c[1] == 59 && c[2] == 56 && c[3] == 126)
                        {
                            // Shift+Ctrl+Alt+KeyNPage
                            k = Key.ShiftMask | Key.CtrlMask | Key.AltMask | Key.PageDown;
                        }
                        else if (c[0] == 53 && c[1] == 59 && c[2] == 56 && c[3] == 126)
                        {
                            // Shift+Ctrl+Alt+KeyPPage
                            k = Key.ShiftMask | Key.CtrlMask | Key.AltMask | Key.PageUp;
                        }
                        else if (c[0] == 49 && c[1] == 59 && c[2] == 56 && c[3] == 72)
                        {
                            // Shift+Ctrl+Alt+KeyHome
                            k = Key.ShiftMask | Key.CtrlMask | Key.AltMask | Key.Home;
                        }
                        else if (c[0] == 49 && c[1] == 59 && c[2] == 56 && c[3] == 70)
                        {
                            // Shift+Ctrl+Alt+KeyEnd
                            k = Key.ShiftMask | Key.CtrlMask | Key.AltMask | Key.End;
                        }
                        else
                        {
                            k = MapCursesKey(wch2);
                        }
                    }
                    else
                    {
                        // Unfortunately there are no way to differentiate Ctrl+Alt+alfa and Ctrl+Shift+Alt+alfa.
                        if (((Key)wch2 & Key.CtrlMask) != 0)
                        {
                            keyModifiers.Ctrl = true;
                        }

                        if (wch2 == 0)
                        {
                            k = Key.CtrlMask | Key.AltMask | Key.Space;
                        }
                        else if (wch >= (uint)Key.A && wch <= (uint)Key.Z)
                        {
                            keyModifiers.Shift = true;
                            keyModifiers.Alt = true;
                        }
                        else if (wch2 < 256)
                        {
                            k = (Key)wch2;
                            keyModifiers.Alt = true;
                        }
                        else
                        {
                            k = (Key)((uint)(Key.AltMask | Key.CtrlMask) + wch2);
                        }
                    }

                    //key = new KeyEvent(k, MapKeyModifiers(k));
                    RaiseKeyPressInternal(k);
                    //todo: keyDownHandler(key);
                    //todo: keyHandler(key);
                }
                else
                {
                    k = Key.Esc;
                    RaiseKeyPressInternal(k);
                    //todo: keyHandler(new KeyEvent(k, MapKeyModifiers(k)));
                }
            }
            else if (wch == Unix.Terminal.Curses.KeyTab)
            {
                k = MapCursesKey(wch);
                RaiseKeyPressInternal(k);
                //todo: keyDownHandler(new KeyEvent(k, MapKeyModifiers(k)));
                //todo: keyHandler(new KeyEvent(k, MapKeyModifiers(k)));
            }
            else
            {
                // Unfortunately there are no way to differentiate Ctrl+alfa and Ctrl+Shift+alfa.
                k = (Key)wch;
                if (wch == 0)
                {
                    k = Key.CtrlMask | Key.Space;
                }
                else if (wch >= (uint)Key.A - 64 && wch <= (uint)Key.Z - 64)
                {
                    if ((Key)(wch + 64) != Key.J)
                    {
                        k = Key.CtrlMask | (Key)(wch + 64);
                    }
                }
                else if (wch >= (uint)Key.A && wch <= (uint)Key.Z)
                {
                    keyModifiers.Shift = true;
                }
                RaiseKeyPressInternal(k);
                //todo: keyDownHandler(new KeyEvent(k, MapKeyModifiers(k)));
                //todo: keyHandler(new KeyEvent(k, MapKeyModifiers(k)));
                //todo: keyUpHandler(new KeyEvent(k, MapKeyModifiers(k)));
            }
            
            /*Debug.WriteLine(string.Join(" - ",k.GetFlags().Select(flag=>flag.ToString())));
            Debug.WriteLine($"or the charactor was {(char)wch}");*/
            // Cause OnKeyUp and OnKeyPressed. Note that the special handling for ESC above 
            // will not impact KeyUp.
            // This is causing ESC firing even if another keystroke was handled.
            //if (wch == Curses.KeyTab) {
            //	keyUpHandler (new KeyEvent (MapCursesKey (wch), keyModifiers));
            //} else {
            //	keyUpHandler (new KeyEvent ((Key)wch, keyModifiers));
            //}
        }
        
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
                (Key.Tab, ConsoleKey.Tab)
            });
        
        

        private void RaiseKeyPressInternal(Key key)
        {
            int KeyValue = (int)key;
            
//todo:             msg += $"{(((uint)KeyValue & (uint)Key.CharMask) > 27 ? $"{(char)KeyValue}" : $"{key}")}";


            RawInputModifiers modifiers = KeyModifiersFlagTranslator.Translate(key);
            char character;
            ConsoleKey consoleKey = KeyFlagTranslator.Translate(key & ~Key.CtrlMask  & ~Key.ShiftMask & ~Key.AltMask, true);
            if (consoleKey == default)
            {
                bool _ = Enum.TryParse(key.ToString(), true, out consoleKey);
            }
            
            if (((uint)KeyValue & (uint)Key.CharMask) > 27)
            {
                character = (char)KeyValue;
            }
            else
            {
                if (consoleKey == default)
                    throw new NotImplementedException();
                character = char.MinValue;
                if(char.IsUpper(character))
                    modifiers |= RawInputModifiers.Shift;
            }
            
            Avalonia.Input.Key convertToKey = DefaultNetConsole.ConvertToKey(consoleKey);
 
            RaiseKeyPress(convertToKey,
                character, modifiers, true, (ulong)Stopwatch.GetTimestamp());
            Thread.Yield();
            RaiseKeyPress(convertToKey,
                character, modifiers, false, (ulong)Stopwatch.GetTimestamp());
        }

        private static readonly FlagTranslator<Unix.Terminal.Curses.Event, RawInputModifiers>
            MouseModifiersFlagTranslator = new(new[]
            {
                (Unix.Terminal.Curses.Event.ButtonAlt,RawInputModifiers.Alt),
                (Unix.Terminal.Curses.Event.ButtonCtrl,RawInputModifiers.Control),
                (Unix.Terminal.Curses.Event.ButtonShift,RawInputModifiers.Shift)
            });
        
        private static readonly FlagTranslator<Unix.Terminal.Curses.Event, RawPointerEventType>
            MouseEventFlagTranslator = new(new[]
            {
                (Unix.Terminal.Curses.Event.Button1Pressed, RawPointerEventType.LeftButtonDown),
                (Unix.Terminal.Curses.Event.Button1Clicked, RawPointerEventType.LeftButtonDown),
                (Unix.Terminal.Curses.Event.Button1Released, RawPointerEventType.LeftButtonUp),
                (Unix.Terminal.Curses.Event.Button2Pressed, RawPointerEventType.RightButtonDown),
                (Unix.Terminal.Curses.Event.Button2Clicked, RawPointerEventType.RightButtonDown),
                (Unix.Terminal.Curses.Event.Button2Released, RawPointerEventType.RightButtonUp),
                (Unix.Terminal.Curses.Event.Button3Pressed, RawPointerEventType.MiddleButtonDown),
                (Unix.Terminal.Curses.Event.Button3Clicked, RawPointerEventType.MiddleButtonDown),
                (Unix.Terminal.Curses.Event.Button3Released, RawPointerEventType.MiddleButtonUp),
                (Unix.Terminal.Curses.Event.Button4Pressed, RawPointerEventType.XButton1Down),
                (Unix.Terminal.Curses.Event.Button4Clicked, RawPointerEventType.XButton1Down),
                (Unix.Terminal.Curses.Event.Button4Released, RawPointerEventType.XButton1Up),
                (Unix.Terminal.Curses.Event.ReportMousePosition, RawPointerEventType.Move),
                (Unix.Terminal.Curses.Event.ButtonWheeledDown, RawPointerEventType.Wheel),
                (Unix.Terminal.Curses.Event.ButtonWheeledUp, RawPointerEventType.Wheel)
            });

        private void HandleMouseInput(Unix.Terminal.Curses.MouseEvent ev)
        {
            const double velocity = 1 / 12D;
            
            RawInputModifiers rawInputModifiers = MouseModifiersFlagTranslator.Translate(ev.ButtonState);

            foreach (Unix.Terminal.Curses.Event flag in ev.ButtonState.GetFlags())
            {
                RawPointerEventType rawPointerEventType = MouseEventFlagTranslator.Translate(flag);
                if (rawPointerEventType == 0) continue;

                Vector? wheelDelta = null;
                if (ev.ButtonState.HasFlag(Unix.Terminal.Curses.Event.ButtonWheeledDown))
                    wheelDelta = new Vector(0, -velocity);
                if (ev.ButtonState.HasFlag(Unix.Terminal.Curses.Event.ButtonWheeledUp))
                    wheelDelta = new Vector(0, velocity);

                RaiseMouseEvent(rawPointerEventType, new Avalonia.Point(ev.X, ev.Y), wheelDelta, rawInputModifiers);
                if (flag is Unix.Terminal.Curses.Event.Button1Clicked or Unix.Terminal.Curses.Event.Button2Clicked
                    or Unix.Terminal.Curses.Event.Button3Clicked or Unix.Terminal.Curses.Event.Button4Clicked)
                {
                    Thread.Yield();
                    RaiseMouseEvent(rawPointerEventType+1, new Avalonia.Point(ev.X, ev.Y), null, rawInputModifiers);
                }
            }
        }

        static Key MapCursesKey(int cursesKey)
        {
            switch (cursesKey)
            {
                case Unix.Terminal.Curses.KeyF1: return Key.F1;
                case Unix.Terminal.Curses.KeyF2: return Key.F2;
                case Unix.Terminal.Curses.KeyF3: return Key.F3;
                case Unix.Terminal.Curses.KeyF4: return Key.F4;
                case Unix.Terminal.Curses.KeyF5: return Key.F5;
                case Unix.Terminal.Curses.KeyF6: return Key.F6;
                case Unix.Terminal.Curses.KeyF7: return Key.F7;
                case Unix.Terminal.Curses.KeyF8: return Key.F8;
                case Unix.Terminal.Curses.KeyF9: return Key.F9;
                case Unix.Terminal.Curses.KeyF10: return Key.F10;
                case Unix.Terminal.Curses.KeyF11: return Key.F11;
                case Unix.Terminal.Curses.KeyF12: return Key.F12;
                case Unix.Terminal.Curses.KeyUp: return Key.CursorUp;
                case Unix.Terminal.Curses.KeyDown: return Key.CursorDown;
                case Unix.Terminal.Curses.KeyLeft: return Key.CursorLeft;
                case Unix.Terminal.Curses.KeyRight: return Key.CursorRight;
                case Unix.Terminal.Curses.KeyHome: return Key.Home;
                case Unix.Terminal.Curses.KeyEnd: return Key.End;
                case Unix.Terminal.Curses.KeyNPage: return Key.PageDown;
                case Unix.Terminal.Curses.KeyPPage: return Key.PageUp;
                case Unix.Terminal.Curses.KeyDeleteChar: return Key.DeleteChar;
                case Unix.Terminal.Curses.KeyInsertChar: return Key.InsertChar;
                case Unix.Terminal.Curses.KeyTab: return Key.Tab;
                case Unix.Terminal.Curses.KeyBackTab: return Key.BackTab;
                case Unix.Terminal.Curses.KeyBackspace: return Key.Backspace;
                case Unix.Terminal.Curses.ShiftKeyUp: return Key.CursorUp | Key.ShiftMask;
                case Unix.Terminal.Curses.ShiftKeyDown: return Key.CursorDown | Key.ShiftMask;
                case Unix.Terminal.Curses.ShiftKeyLeft: return Key.CursorLeft | Key.ShiftMask;
                case Unix.Terminal.Curses.ShiftKeyRight: return Key.CursorRight | Key.ShiftMask;
                case Unix.Terminal.Curses.ShiftKeyHome: return Key.Home | Key.ShiftMask;
                case Unix.Terminal.Curses.ShiftKeyEnd: return Key.End | Key.ShiftMask;
                case Unix.Terminal.Curses.ShiftKeyNPage: return Key.PageDown | Key.ShiftMask;
                case Unix.Terminal.Curses.ShiftKeyPPage: return Key.PageUp | Key.ShiftMask;
                case Unix.Terminal.Curses.AltKeyUp: return Key.CursorUp | Key.AltMask;
                case Unix.Terminal.Curses.AltKeyDown: return Key.CursorDown | Key.AltMask;
                case Unix.Terminal.Curses.AltKeyLeft: return Key.CursorLeft | Key.AltMask;
                case Unix.Terminal.Curses.AltKeyRight: return Key.CursorRight | Key.AltMask;
                case Unix.Terminal.Curses.AltKeyHome: return Key.Home | Key.AltMask;
                case Unix.Terminal.Curses.AltKeyEnd: return Key.End | Key.AltMask;
                case Unix.Terminal.Curses.AltKeyNPage: return Key.PageDown | Key.AltMask;
                case Unix.Terminal.Curses.AltKeyPPage: return Key.PageUp | Key.AltMask;
                case Unix.Terminal.Curses.CtrlKeyUp: return Key.CursorUp | Key.CtrlMask;
                case Unix.Terminal.Curses.CtrlKeyDown: return Key.CursorDown | Key.CtrlMask;
                case Unix.Terminal.Curses.CtrlKeyLeft: return Key.CursorLeft | Key.CtrlMask;
                case Unix.Terminal.Curses.CtrlKeyRight: return Key.CursorRight | Key.CtrlMask;
                case Unix.Terminal.Curses.CtrlKeyHome: return Key.Home | Key.CtrlMask;
                case Unix.Terminal.Curses.CtrlKeyEnd: return Key.End | Key.CtrlMask;
                case Unix.Terminal.Curses.CtrlKeyNPage: return Key.PageDown | Key.CtrlMask;
                case Unix.Terminal.Curses.CtrlKeyPPage: return Key.PageUp | Key.CtrlMask;
                case Unix.Terminal.Curses.ShiftCtrlKeyUp: return Key.CursorUp | Key.ShiftMask | Key.CtrlMask;
                case Unix.Terminal.Curses.ShiftCtrlKeyDown: return Key.CursorDown | Key.ShiftMask | Key.CtrlMask;
                case Unix.Terminal.Curses.ShiftCtrlKeyLeft: return Key.CursorLeft | Key.ShiftMask | Key.CtrlMask;
                case Unix.Terminal.Curses.ShiftCtrlKeyRight: return Key.CursorRight | Key.ShiftMask | Key.CtrlMask;
                case Unix.Terminal.Curses.ShiftCtrlKeyHome: return Key.Home | Key.ShiftMask | Key.CtrlMask;
                case Unix.Terminal.Curses.ShiftCtrlKeyEnd: return Key.End | Key.ShiftMask | Key.CtrlMask;
                case Unix.Terminal.Curses.ShiftCtrlKeyNPage: return Key.PageDown | Key.ShiftMask | Key.CtrlMask;
                case Unix.Terminal.Curses.ShiftCtrlKeyPPage: return Key.PageUp | Key.ShiftMask | Key.CtrlMask;
                case Unix.Terminal.Curses.ShiftAltKeyUp: return Key.CursorUp | Key.ShiftMask | Key.AltMask;
                case Unix.Terminal.Curses.ShiftAltKeyDown: return Key.CursorDown | Key.ShiftMask | Key.AltMask;
                case Unix.Terminal.Curses.ShiftAltKeyLeft: return Key.CursorLeft | Key.ShiftMask | Key.AltMask;
                case Unix.Terminal.Curses.ShiftAltKeyRight: return Key.CursorRight | Key.ShiftMask | Key.AltMask;
                case Unix.Terminal.Curses.ShiftAltKeyNPage: return Key.PageDown | Key.ShiftMask | Key.AltMask;
                case Unix.Terminal.Curses.ShiftAltKeyPPage: return Key.PageUp | Key.ShiftMask | Key.AltMask;
                case Unix.Terminal.Curses.ShiftAltKeyHome: return Key.Home | Key.ShiftMask | Key.AltMask;
                case Unix.Terminal.Curses.ShiftAltKeyEnd: return Key.End | Key.ShiftMask | Key.AltMask;
                case Unix.Terminal.Curses.AltCtrlKeyNPage: return Key.PageDown | Key.AltMask | Key.CtrlMask;
                case Unix.Terminal.Curses.AltCtrlKeyPPage: return Key.PageUp | Key.AltMask | Key.CtrlMask;
                case Unix.Terminal.Curses.AltCtrlKeyHome: return Key.Home | Key.AltMask | Key.CtrlMask;
                case Unix.Terminal.Curses.AltCtrlKeyEnd: return Key.End | Key.AltMask | Key.CtrlMask;
                default: return Key.Unknown;
            }
        }
        
        KeyModifiers MapKeyModifiers (Key key)
        {
            if (keyModifiers == null)
                keyModifiers = new KeyModifiers ();

            if (!keyModifiers.Shift && (key & Key.ShiftMask) != 0)
                keyModifiers.Shift = true;
            if (!keyModifiers.Alt && (key & Key.AltMask) != 0)
                keyModifiers.Alt = true;
            if (!keyModifiers.Ctrl && (key & Key.CtrlMask) != 0)
                keyModifiers.Ctrl = true;

            return keyModifiers;
        }
        

    }
}