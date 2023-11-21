using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Consolonia.Core.Infrastructure;
using Consolonia.Core.InternalHelpers;
using Terminal.Gui;
using Point = Avalonia.Point;

// ReSharper disable UnusedMember.Local
#pragma warning disable CS0649

namespace Consolonia.PlatformSupport
{
    public class Win32Console : InputLessDefaultNetConsole
    {
        private static readonly FlagTranslator<WindowsConsole.ControlKeyState, RawInputModifiers>
            ModifiersFlagTranslator = new(new[]
            {
                (WindowsConsole.ControlKeyState.ShiftPressed, RawInputModifiers.Shift),
                (WindowsConsole.ControlKeyState.LeftAltPressed, RawInputModifiers.Alt),
                (WindowsConsole.ControlKeyState.RightAltPressed, RawInputModifiers.Alt),
                (WindowsConsole.ControlKeyState.LeftControlPressed, RawInputModifiers.Control),
                (WindowsConsole.ControlKeyState.RightControlPressed, RawInputModifiers.Control)
            });

        private static readonly FlagTranslator<WindowsConsole.ButtonState, RawPointerEventType>
            MouseButtonFlagTranslator = new(new[]
            {
                (WindowsConsole.ButtonState.Button1Pressed, RawPointerEventType.LeftButtonDown),
                (WindowsConsole.ButtonState.RightmostButtonPressed, RawPointerEventType.RightButtonDown),
                (WindowsConsole.ButtonState.Button2Pressed, RawPointerEventType.MiddleButtonDown),
                (WindowsConsole.ButtonState.Button3Pressed, RawPointerEventType.XButton1Down),
                (WindowsConsole.ButtonState.Button4Pressed, RawPointerEventType.XButton2Down)
            });

        private static readonly FlagTranslator<WindowsConsole.ButtonState, RawInputModifiers>
            MouseModifiersFlagTranslator = new(new[]
            {
                (WindowsConsole.ButtonState.Button1Pressed, RawInputModifiers.LeftMouseButton),
                (WindowsConsole.ButtonState.RightmostButtonPressed, RawInputModifiers.RightMouseButton),
                (WindowsConsole.ButtonState.Button2Pressed, RawInputModifiers.MiddleMouseButton),
                (WindowsConsole.ButtonState.Button3Pressed, RawInputModifiers.XButton1MouseButton),
                (WindowsConsole.ButtonState.Button4Pressed, RawInputModifiers.XButton2MouseButton)
            });

        private readonly WindowsConsole _windowsConsole;

        private int _mouseButtonsState;

        public Win32Console()
        {
            _windowsConsole = new WindowsConsole();

            StartEventLoop();
        }

        #region chatGPT

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT_RECORD
        {
            public ushort EventType;
            public UnionRecord Event;

            [StructLayout(LayoutKind.Explicit)]
            public struct UnionRecord
            {
                [FieldOffset(0)]
                public KEY_EVENT_RECORD KeyEvent;
                [FieldOffset(0)]
                public FOCUS_EVENT_RECORD FocusEvent;
                // Other event types omitted for brevity
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEY_EVENT_RECORD
        {
#pragma warning disable IDE1006
            public bool bKeyDown;
#pragma warning restore IDE1006
            // Other fields omitted for brevity
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FOCUS_EVENT_RECORD
        {
#pragma warning disable IDE1006
            public bool bSetFocus;
#pragma warning restore IDE1006
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
#pragma warning disable CA5392
        private static extern bool WriteConsoleInput(
#pragma warning restore CA5392
            IntPtr hConsoleInput,
            INPUT_RECORD[] lpBuffer,
            uint nLength,
            out uint lpNumberOfEventsWritten);

        #endregion
        
        public override void PauseIO(Task task)
        {
            base.PauseIO(task);
            
            var inputRecords = new INPUT_RECORD[1];

            // Create a focus event
            inputRecords[0].EventType = 0x0010; // FOCUS_EVENT
            inputRecords[0].Event.FocusEvent = new FOCUS_EVENT_RECORD
            {
                bSetFocus = true
            };

            if (!WriteConsoleInput(_windowsConsole.InputHandle, inputRecords, 1, out uint eventsWritten))
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
                while (!Disposed /*inject ThreadAbortException*/)
                {
                    PauseTask?.Wait();
                    var readConsoleInput = _windowsConsole.ReadConsoleInput();
                    if (!readConsoleInput.Any())
                        throw new NotImplementedException();
                    foreach (WindowsConsole.InputRecord inputRecord in readConsoleInput)
                        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                        switch (inputRecord.EventType)
                        {
                            case WindowsConsole.EventType.WindowBufferSize:
                                ActualizeSize();
                                break;
                            case WindowsConsole.EventType.Focus:
                                WindowsConsole.FocusEventRecord focusEvent = inputRecord.FocusEvent;
                                RaiseFocusEvent(focusEvent.bSetFocus != 0);
                                break;
                            case WindowsConsole.EventType.Key:
                                HandleKeyInput(inputRecord);
                                break;
                            case WindowsConsole.EventType.Mouse:

                                WindowsConsole.MouseEventRecord mouseEvent = inputRecord.MouseEvent;

                                if (HandleMouseInput(mouseEvent)) return; //todo: implement
                                break;
                        }
                }
            });
        }

        private bool HandleMouseInput(WindowsConsole.MouseEventRecord mouseEvent)
        {
            var point = new Point(mouseEvent.MousePosition.X,
                mouseEvent.MousePosition.Y);
            int incomeMouseState = (int)mouseEvent.ButtonState;
            RawInputModifiers inputModifiers =
                ModifiersFlagTranslator.Translate(mouseEvent.ControlKeyState) |
                MouseModifiersFlagTranslator.Translate((WindowsConsole.ButtonState)incomeMouseState);

            RawPointerEventType eventType = default;
            Vector? wheelDelta = null;
            short repeat = 1;

            switch (mouseEvent.EventFlags)
            {
                case WindowsConsole.EventFlags.DoubleClick:
                    repeat = 2; //todo: now supporting only leftbutton
                    eventType = RawPointerEventType.LeftButtonDown;
                    break;
                case default(WindowsConsole.EventFlags):
                    int xor = _mouseButtonsState ^ incomeMouseState;
                    foreach (RawPointerEventType pointerEventType in ((WindowsConsole.ButtonState)(xor &
                                incomeMouseState)).GetFlags()
                            .Select(MouseButtonFlagTranslator.Translate))
                        //todo: вернуть mouse gesture на элементы
                        RaiseMouseEvent(pointerEventType,
                            point,
                            null,
                            inputModifiers);

                    //todo: refactor: code clone
                    foreach (RawPointerEventType pointerEventType in ((WindowsConsole.ButtonState)(xor &
                            _mouseButtonsState)).GetFlags()
                        .Select(MouseButtonFlagTranslator.Translate))
                    {
                        RawPointerEventType rawPointerEventType = pointerEventType + 1;
                        RaiseMouseEvent(rawPointerEventType,
                            point,
                            null,
                            inputModifiers);
                    }

                    _mouseButtonsState = incomeMouseState;
                    repeat = 0;
                    break;
                case WindowsConsole.EventFlags.MouseWheeled:
                    double velocity = 1 / 12D;
                    if (incomeMouseState == -7864320)
                        velocity *= -1;
                    wheelDelta = new Vector(0, velocity);
                    eventType = RawPointerEventType.Wheel;
                    break;
                case WindowsConsole.EventFlags.MouseHorizontalWheeled:
                    return true;
                case WindowsConsole.EventFlags.MouseMoved:
                    eventType = RawPointerEventType.Move;
                    break;
                default:
                    throw new InvalidOperationException(mouseEvent.EventFlags.ToString());
            }

            for (short i = 0; i < repeat; i++)
            {
                RaiseMouseEvent(eventType,
                    point,
                    wheelDelta,
                    inputModifiers);

                if (eventType <= RawPointerEventType.XButton2Down)
                    RaiseMouseEvent(eventType + 1,
                        point,
                        wheelDelta,
                        inputModifiers);
            }

            return false;
        }

        private void HandleKeyInput(WindowsConsole.InputRecord inputRecord)
        {
            WindowsConsole.KeyEventRecord keyEvent = inputRecord.KeyEvent;
            char character = keyEvent.UnicodeChar;
            RawInputModifiers modifiers =
                ModifiersFlagTranslator.Translate(keyEvent.dwControlKeyState);
            RaiseKeyPress(DefaultNetConsole.ConvertToKey((ConsoleKey)keyEvent.wVirtualKeyCode),
                character, modifiers, keyEvent.bKeyDown, (ulong)Stopwatch.GetTimestamp());
        }
    }
}