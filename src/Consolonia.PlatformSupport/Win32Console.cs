using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;
using Consolonia.Core.InternalHelpers;
using Key = Avalonia.Input.Key;
using Point = Avalonia.Point;
using static Vanara.PInvoke.Kernel32;
using Vanara.PInvoke;
using Consolonia.Core.Text;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

// ReSharper disable UnusedMember.Local
#pragma warning disable CS0649

namespace Consolonia.PlatformSupport
{
    [SupportedOSPlatform("windows")]
    public class Win32Console : ConsoleBase
    {
        private static readonly FlagTranslator<CONTROL_KEY_STATE, RawInputModifiers>
            KeyModifiersTranslator = new(
            [
                (CONTROL_KEY_STATE.SHIFT_PRESSED, RawInputModifiers.Shift),
                (CONTROL_KEY_STATE.LEFT_ALT_PRESSED, RawInputModifiers.Alt),
                (CONTROL_KEY_STATE.RIGHT_ALT_PRESSED, RawInputModifiers.Alt),
                (CONTROL_KEY_STATE.LEFT_CTRL_PRESSED, RawInputModifiers.Control),
                (CONTROL_KEY_STATE.RIGHT_CTRL_PRESSED, RawInputModifiers.Control)
            ]);

        private static readonly FlagTranslator<MOUSE_BUTTON_STATE, RawInputModifiers>
            MouseModifiersTranslator = new(
            [
                (MOUSE_BUTTON_STATE.FROM_LEFT_1ST_BUTTON_PRESSED, RawInputModifiers.LeftMouseButton),
                (MOUSE_BUTTON_STATE.RIGHTMOST_BUTTON_PRESSED, RawInputModifiers.RightMouseButton),
                (MOUSE_BUTTON_STATE.FROM_LEFT_2ND_BUTTON_PRESSED, RawInputModifiers.MiddleMouseButton),
                (MOUSE_BUTTON_STATE.FROM_LEFT_3RD_BUTTON_PRESSED, RawInputModifiers.XButton1MouseButton),
                (MOUSE_BUTTON_STATE.FROM_LEFT_4TH_BUTTON_PRESSED, RawInputModifiers.XButton2MouseButton)
            ]);


        private static readonly FlagTranslator<MOUSE_BUTTON_STATE, RawPointerEventType>
            MouseButtonDownEventTypeTranslator = new(
            [
                (MOUSE_BUTTON_STATE.FROM_LEFT_1ST_BUTTON_PRESSED, RawPointerEventType.LeftButtonDown),
                (MOUSE_BUTTON_STATE.RIGHTMOST_BUTTON_PRESSED, RawPointerEventType.RightButtonDown),
                (MOUSE_BUTTON_STATE.FROM_LEFT_2ND_BUTTON_PRESSED, RawPointerEventType.MiddleButtonDown),
                (MOUSE_BUTTON_STATE.FROM_LEFT_3RD_BUTTON_PRESSED, RawPointerEventType.XButton1Down),
                (MOUSE_BUTTON_STATE.FROM_LEFT_4TH_BUTTON_PRESSED, RawPointerEventType.XButton2Down)
            ]);

        private static readonly FlagTranslator<MOUSE_BUTTON_STATE, RawPointerEventType>
            MouseButtonUpEventTypeTranslator = new(
            [
                (MOUSE_BUTTON_STATE.FROM_LEFT_1ST_BUTTON_PRESSED, RawPointerEventType.LeftButtonUp),
                (MOUSE_BUTTON_STATE.RIGHTMOST_BUTTON_PRESSED, RawPointerEventType.RightButtonUp),
                (MOUSE_BUTTON_STATE.FROM_LEFT_2ND_BUTTON_PRESSED, RawPointerEventType.MiddleButtonUp),
                (MOUSE_BUTTON_STATE.FROM_LEFT_3RD_BUTTON_PRESSED, RawPointerEventType.XButton1Up),
                (MOUSE_BUTTON_STATE.FROM_LEFT_4TH_BUTTON_PRESSED, RawPointerEventType.XButton2Up)
            ]);

        private static readonly Dictionary<string, ConsoleKey> EscapeSequenceToVirtualKeyCodeMap = new Dictionary<string, ConsoleKey>()
        {
            { Esc.HomeKey, ConsoleKey.Home },
            { Esc.EndKey, ConsoleKey.End },
            { Esc.F1Key, ConsoleKey.F1 },
            { Esc.F2Key, ConsoleKey.F2 },
            { Esc.F3Key, ConsoleKey.F3 },
            { Esc.F4Key, ConsoleKey.F4 },
            { Esc.F5Key, ConsoleKey.F5 },
            { Esc.F6Key, ConsoleKey.F6 },
            { Esc.F7Key, ConsoleKey.F7 },
            { Esc.F8Key, ConsoleKey.F8 },
            { Esc.F9Key, ConsoleKey.F9 },
            { Esc.F10Key, ConsoleKey.F10 },
            { Esc.F11Key, ConsoleKey.F11 },
            { Esc.F12Key, ConsoleKey.F12 },
            { Esc.UpKey, ConsoleKey.UpArrow },
            { Esc.DownKey, ConsoleKey.DownArrow },
            { Esc.LeftKey, ConsoleKey.LeftArrow },
            { Esc.RightKey, ConsoleKey.RightArrow },
            { Esc.PageUpKey, ConsoleKey.PageUp },
            { Esc.PageDownKey, ConsoleKey.PageDown },
            { Esc.InsertKey, ConsoleKey.Insert },
            { Esc.DeleteKey, ConsoleKey.Delete },
        };

        private readonly HFILE _inputHandle;
        private readonly HFILE _outputHandle;

        private MOUSE_BUTTON_STATE _mouseButtonsState = MOUSE_BUTTON_STATE.NONE;

        private static bool SupportsAnsiInput => Environment.OSVersion.Version >= new Version(10, 0, 10586);
        private static bool SupportsAnsiOutput => (Environment.OSVersion.Version >= new Version(10, 0, 10586)) &&
                        (Environment.GetEnvironmentVariable("WT_SESSION") is not null ||
                         Environment.GetEnvironmentVariable("VSAPPIDNAME") != null);

        public override bool SupportsAltSolo => true;

        public override bool SupportsMouse => true;

        public override bool SupportsMouseMove => true;

        public Win32Console()
            : base(SupportsAnsiOutput ? new WindowsLegacyConsoleOutput() : new WindowsLegacyConsoleOutput())
        {
            // _windowsConsole = new WindowsConsole();
            _inputHandle = GetStdHandle(StdHandleType.STD_INPUT_HANDLE);
            _outputHandle = GetStdHandle(StdHandleType.STD_OUTPUT_HANDLE);

            // ReSharper disable VirtualMemberCallInConstructor
            PrepareConsole();

            StartEventLoop();
        }

        public override void PrepareConsole()
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            // perpare input 
            if (!GetConsoleMode(_inputHandle, out CONSOLE_INPUT_MODE inputMode))
                throw GetLastError().GetException();

            inputMode |= (CONSOLE_INPUT_MODE.ENABLE_MOUSE_INPUT
                    | CONSOLE_INPUT_MODE.ENABLE_WINDOW_INPUT 
                    | CONSOLE_INPUT_MODE.ENABLE_EXTENDED_FLAGS
                    | CONSOLE_INPUT_MODE.ENABLE_VIRTUAL_TERMINAL_INPUT
                    );
            inputMode &= ~CONSOLE_INPUT_MODE.ENABLE_QUICK_EDIT_MODE;
            inputMode &= ~CONSOLE_INPUT_MODE.ENABLE_ECHO_INPUT;
            inputMode &= ~CONSOLE_INPUT_MODE.ENABLE_PROCESSED_INPUT;
            inputMode &= ~CONSOLE_INPUT_MODE.ENABLE_LINE_INPUT;

            if (!SupportsAnsiInput)
                inputMode &= ~CONSOLE_INPUT_MODE.ENABLE_VIRTUAL_TERMINAL_INPUT;

            if (!SetConsoleMode(_inputHandle, inputMode))
                throw GetLastError().GetException();

            // prepare output
            if (!GetConsoleMode(_outputHandle, out CONSOLE_OUTPUT_MODE originalOutputMode))
                throw GetLastError().GetException();

            SetConsoleMode(_outputHandle, CONSOLE_OUTPUT_MODE.DISABLE_NEWLINE_AUTO_RETURN |
                                   CONSOLE_OUTPUT_MODE.ENABLE_VIRTUAL_TERMINAL_PROCESSING |
                                   CONSOLE_OUTPUT_MODE.ENABLE_LVB_GRID_WORLDWIDE);

            if (SupportsAnsiOutput)
            {
                Console.WriteLine(Esc.EnableMouseTracking);
                Console.WriteLine(Esc.EnableMouseMotionTracking);
                Console.WriteLine(Esc.EnableExtendedMouseTracking);
            }

            base.PrepareConsole();
        }

        public override void PauseIO(Task task)
        {
            base.PauseIO(task);

            var inputRecords = new INPUT_RECORD[1];

            // Create a focus event
            inputRecords[0].EventType = EVENT_TYPE.FOCUS_EVENT; // FOCUS_EVENT
            inputRecords[0].Event.FocusEvent = new FOCUS_EVENT_RECORD
            {
                bSetFocus = true
            };

            // ReSharper disable once InvertIf
            if (!WriteConsoleInput(_inputHandle, inputRecords, 1, out uint _))
            {
                // Handle error
                int error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error, $"Error writing console input: {error}");
            }
        }

        private void StartEventLoop()
        {
            Task.Run(() =>
            {
                StringBuilder sbSequence = new StringBuilder();
                const int bufferSize = 1;
                while (!Disposed /*inject ThreadAbortException*/)
                {
                    PauseTask?.Wait();

                    if (SupportsAnsiInput)
                    {
                        // ansi mode just reads text in and parses it for ANSI sequences.
                        char[] buffer = new char[bufferSize];
                        var nRead = Console.In.Read(buffer, 0, bufferSize);
                        ParseAnsiSequence(sbSequence, buffer[0]);
                    }
                    else
                    {
                        // Native mode uses native Console API to read in events
                        var inputRecords = new INPUT_RECORD[bufferSize];
                        if (!Kernel32.ReadConsoleInput(_inputHandle, inputRecords, bufferSize,
                            out var numberEventsRead))
                            throw GetLastError().GetException();
                        Debug.Write($"{numberEventsRead} Events=>");

                        for (int i = 0; i < numberEventsRead; i++)
                        {
                            var inputRecord = inputRecords[i];
                            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                            switch (inputRecord.EventType)
                            {
                                case EVENT_TYPE.WINDOW_BUFFER_SIZE_EVENT:
                                    WINDOW_BUFFER_SIZE_RECORD windowBufferSize = inputRecord.Event.WindowBufferSizeEvent;
                                    Size = new PixelBufferSize((ushort)windowBufferSize.dwSize.X,
                                        (ushort)windowBufferSize.dwSize.Y);
                                    break;
                                case EVENT_TYPE.FOCUS_EVENT:
                                    FOCUS_EVENT_RECORD focusEvent = inputRecord.Event.FocusEvent;
                                    RaiseFocusEvent(focusEvent.bSetFocus != 0);
                                    break;
                                case EVENT_TYPE.KEY_EVENT:
                                    HandleKeyInput(inputRecord.Event.KeyEvent);
                                    break;
                                case EVENT_TYPE.MOUSE_EVENT:
                                    MOUSE_EVENT_RECORD mouseEvent = inputRecord.Event.MouseEvent;
                                    HandleMouseInput(mouseEvent);
                                    break;
                            }
                        }
                    }
                }
            });
        }


        // ReSharper disable ExpressionIsAlwaysNull

        private void HandleMouseInput(MOUSE_EVENT_RECORD mouseEvent)
        {
            Debug.WriteLine($"{mouseEvent.dwEventFlags.ToString()} {mouseEvent.dwMousePosition.X},{mouseEvent.dwMousePosition.Y} {mouseEvent.dwButtonState.ToString()} {mouseEvent.dwControlKeyState.ToString()}");
            var point = new Point(mouseEvent.dwMousePosition.X, mouseEvent.dwMousePosition.Y);
            RawInputModifiers inputModifiers =
                KeyModifiersTranslator.Translate(mouseEvent.dwControlKeyState) |
                MouseModifiersTranslator.Translate(mouseEvent.dwButtonState);

            var eventType = RawPointerEventType.Move;
            Vector? wheelDelta = null;

            switch (mouseEvent.dwEventFlags)
            {
                case MOUSE_EVENT_FLAG.DOUBLE_CLICK:
                    RawPointerEventType downButtonEvent =
                        MouseButtonDownEventTypeTranslator.Translate(mouseEvent.dwButtonState);
                    RawPointerEventType upButtonEvent =
                        MouseButtonUpEventTypeTranslator.Translate(mouseEvent.dwButtonState);
                    for (int i = 0; i < 2; i++)
                    {
                        RaiseMouseEvent(downButtonEvent,
                            point,
                            wheelDelta,
                            inputModifiers);

                        RaiseMouseEvent(upButtonEvent,
                            point,
                            wheelDelta,
                            inputModifiers);
                    }

                    break;

                case MOUSE_EVENT_FLAG.NONE:
                    foreach (MOUSE_BUTTON_STATE flag in Enum.GetValues<MOUSE_BUTTON_STATE>())
                        if (!_mouseButtonsState.HasFlag(flag) && mouseEvent.dwButtonState.HasFlag(flag))
                        {
                            // If we went from flag off to flag on
                            RawPointerEventType buttonDownEventType =
                                MouseButtonDownEventTypeTranslator.Translate(flag);
#pragma warning disable IDE0034
                            // Simplify 'default' expression
                            if (buttonDownEventType != default)
                                RaiseMouseEvent(buttonDownEventType,
                                    point,
                                    null,
                                    inputModifiers);
#pragma warning restore IDE0034
                            // Simplify 'default' expression
                        }

                        else if (_mouseButtonsState.HasFlag(flag) && !mouseEvent.dwButtonState.HasFlag(flag))
                        {
                            // If we went from flag On to flag off
                            RawPointerEventType buttonEventType = MouseButtonUpEventTypeTranslator.Translate(flag);
#pragma warning disable IDE0034
                            // Simplify 'default' expression
                            if (buttonEventType != default)
                                RaiseMouseEvent(buttonEventType,
                                    point,
                                    null,
                                    inputModifiers);
#pragma warning restore IDE0034
                            // Simplify 'default' expression
                        }
                        else
                        {
                            RaiseMouseEvent(eventType,
                                point,
                                null,
                                inputModifiers);
                        }

                    break;

                case MOUSE_EVENT_FLAG.MOUSE_WHEELED:
                    double velocity = mouseEvent.dwButtonState < 0 ? -1 : 1;
                    wheelDelta = new Vector(0, velocity);
                    RaiseMouseEvent(RawPointerEventType.Wheel,
                        point,
                        wheelDelta,
                        inputModifiers);
                    break;
                case MOUSE_EVENT_FLAG.MOUSE_HWHEELED:
                    break;
                case MOUSE_EVENT_FLAG.MOUSE_MOVED:
                    RaiseMouseEvent(RawPointerEventType.Move,
                        point,
                        wheelDelta,
                        inputModifiers);
                    _mouseButtonsState = mouseEvent.dwButtonState;
                    break;
                case MOUSE_EVENT_FLAG.MOUSE_MOVED | MOUSE_EVENT_FLAG.DOUBLE_CLICK:
                    RaiseMouseEvent(RawPointerEventType.LeftButtonDown, point, null, inputModifiers);
                    RaiseMouseEvent(RawPointerEventType.Move, point, null, inputModifiers);
                    //RaiseMouseEvent(RawPointerEventType.LeftButtonUp, point, null, inputModifiers);
                    break;
                default:
                    throw new InvalidOperationException(mouseEvent.dwEventFlags.ToString());
            }

            _mouseButtonsState = mouseEvent.dwButtonState;
        }

        private void HandleKeyInput(KEY_EVENT_RECORD keyEvent)
        {
            Debug.WriteLine($"Char:{keyEvent.uChar} Code:{keyEvent.wVirtualKeyCode} Down:{keyEvent.bKeyDown} {keyEvent.dwControlKeyState.ToString()}");

            RawInputModifiers modifiers =
                KeyModifiersTranslator.Translate(keyEvent.dwControlKeyState);
            Key key = DefaultNetConsole.ConvertToKey((ConsoleKey)keyEvent.wVirtualKeyCode);
            if (key == Key.LeftAlt || key == Key.RightAlt)
                modifiers |= RawInputModifiers.Alt;
            RaiseKeyPress(key,
                keyEvent.uChar, modifiers, keyEvent.bKeyDown, (ulong)Stopwatch.GetTimestamp());
        }

        private void ParseAnsiSequence(StringBuilder sbSequence, char ch)
        {
            // if we are gathering an escape sequence
            if (sbSequence.Length > 0)
            {
                // we we were in an escape sequence and we hit another escape sequence, it must have just been text
                if (ch == Esc.Escape)
                {
                    // emit the gathered up key input
                    foreach (var key in sbSequence.ToString())
                    {
                        EmitKeyPressEvents(key);
                    }
                    sbSequence.Clear();

                    // and start processing it as a new possible escape sequence
                }

                sbSequence.Append(ch);
                var currentSequence = sbSequence.ToString();

                // If CTRL/ALT/SHIFT etc is used with things like RightArrow KeyModifierPrefix sequence will be emited
                if (currentSequence.StartsWith(Esc.KeyModifierPrefix, false, CultureInfo.InvariantCulture) &&
                    currentSequence.Length == (Esc.KeyModifierPrefix.Length + 2))
                {
                    var index = Esc.KeyModifierPrefix.Length;
                    var modifier = currentSequence[index..++index];
                    var code = currentSequence[index..++index];

                    if (EscapeSequenceToVirtualKeyCodeMap.TryGetValue($"{Esc.CSI}{code}", out ConsoleKey consoleKey))
                    {
                        EmitKeyPressEvents(Esc.Escape, (ushort)consoleKey, modifier switch
                        {
                            "2" => CONTROL_KEY_STATE.SHIFT_PRESSED,
                            "3" => CONTROL_KEY_STATE.LEFT_ALT_PRESSED,
                            "5" => CONTROL_KEY_STATE.LEFT_CTRL_PRESSED,
                            "6" => CONTROL_KEY_STATE.LEFT_CTRL_PRESSED | CONTROL_KEY_STATE.SHIFT_PRESSED,
                            "7" => CONTROL_KEY_STATE.LEFT_CTRL_PRESSED | CONTROL_KEY_STATE.LEFT_ALT_PRESSED,
                            _ => CONTROL_KEY_STATE.NONE
                        });
                        sbSequence.Clear();
                    }
                }
                // if it is a known escape sequence, we can just emit the key
                else if (EscapeSequenceToVirtualKeyCodeMap.TryGetValue(currentSequence, out ConsoleKey consoleKey))
                {
                    // emit it and reset
                    EmitKeyPressEvents(Esc.Escape, (ushort)consoleKey);
                    sbSequence.Clear();
                }
                // if it is a mouse sequence
                else if (Esc.TryParseMouse(currentSequence, out var codes, out var x, out var y, out var buttonPressed, out var wheelDelta))
                {
                    EmitMouseEvent(codes, ref x, ref y, buttonPressed, wheelDelta);
                    sbSequence.Clear();
                }
            }
            else
            {
                if (ch == Esc.Escape)
                {
                    // start tracking escape sequence
                    sbSequence.Append(ch);
                }
                else
                {
                    // it's just a keypress
                    EmitKeyPressEvents(ch);
                }
            }
        }

        private void EmitMouseEvent(AnsiMouseCodes codes, ref short x, ref short y, bool buttonPressed, int wheelDelta)
        {
            MOUSE_EVENT_RECORD newMouseEvent = new MOUSE_EVENT_RECORD()
            {
                dwMousePosition = new COORD { X = --x, Y = --y }
            };

            // determine the event type
            if (codes.HasFlag(AnsiMouseCodes.MouseWheel))
            {
                newMouseEvent.dwEventFlags |= MOUSE_EVENT_FLAG.MOUSE_WHEELED;
                newMouseEvent.dwButtonState = (MOUSE_BUTTON_STATE)wheelDelta;
            }
            else if (codes.HasFlag(AnsiMouseCodes.MouseMove))
                newMouseEvent.dwEventFlags |= MOUSE_EVENT_FLAG.MOUSE_MOVED;
            else
                newMouseEvent.dwEventFlags |= MOUSE_EVENT_FLAG.NONE;

            // get the modifiers state
            if (codes.HasFlag(AnsiMouseCodes.Alt))
                newMouseEvent.dwControlKeyState |= CONTROL_KEY_STATE.LEFT_ALT_PRESSED;
            if (codes.HasFlag(AnsiMouseCodes.Shift))
                newMouseEvent.dwControlKeyState |= CONTROL_KEY_STATE.SHIFT_PRESSED;
            if (codes.HasFlag(AnsiMouseCodes.Control))
                newMouseEvent.dwControlKeyState |= CONTROL_KEY_STATE.LEFT_CTRL_PRESSED;

            // determine mouse button state when pressed.
            if (buttonPressed)
            {
                if (codes.HasFlag(AnsiMouseCodes.Button2))
                    newMouseEvent.dwButtonState |= MOUSE_BUTTON_STATE.FROM_LEFT_2ND_BUTTON_PRESSED;
                else if (codes.HasFlag(AnsiMouseCodes.Button3))
                    newMouseEvent.dwButtonState |= MOUSE_BUTTON_STATE.RIGHTMOST_BUTTON_PRESSED;
                else if (codes.HasFlag(AnsiMouseCodes.Button1))
                    newMouseEvent.dwButtonState |= MOUSE_BUTTON_STATE.FROM_LEFT_1ST_BUTTON_PRESSED;
            }
            HandleMouseInput(newMouseEvent);
        }

        private void EmitKeyPressEvents(char key, ushort keyCode = 0, CONTROL_KEY_STATE keyState = CONTROL_KEY_STATE.NONE)
        {
            HandleKeyInput(new KEY_EVENT_RECORD()
            {
                bKeyDown = true,
                uChar = key,
                wVirtualKeyCode = keyCode,
                dwControlKeyState = keyState
            });

            HandleKeyInput(new KEY_EVENT_RECORD()
            {
                bKeyDown = false,
                uChar = key,
                wVirtualKeyCode = keyCode,
                dwControlKeyState = keyState
            });
        }
    }
}