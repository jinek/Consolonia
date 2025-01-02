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
using Key = Avalonia.Input.Key;
using Point = Avalonia.Point;
using static Vanara.PInvoke.Kernel32;
using System.Runtime.Versioning;
// ReSharper disable UnusedMember.Local
#pragma warning disable CS0649

namespace Consolonia.PlatformSupport
{
    [SupportedOSPlatform("windows")]
    public class Win32Console : InputLessDefaultNetConsole
    {
        private static readonly FlagTranslator<CONTROL_KEY_STATE, RawInputModifiers>
            KeyModifiersTranslator = new(
            [
                (CONTROL_KEY_STATE.NONE, RawInputModifiers.None),
                (CONTROL_KEY_STATE.SHIFT_PRESSED, RawInputModifiers.Shift),
                (CONTROL_KEY_STATE.LEFT_ALT_PRESSED, RawInputModifiers.Alt),
                (CONTROL_KEY_STATE.RIGHT_ALT_PRESSED, RawInputModifiers.Alt),
                (CONTROL_KEY_STATE.LEFT_CTRL_PRESSED, RawInputModifiers.Control),
                (CONTROL_KEY_STATE.RIGHT_CTRL_PRESSED, RawInputModifiers.Control),
            ]);

        private static readonly FlagTranslator<MOUSE_BUTTON_STATE, RawInputModifiers>
            MouseModifiersTranslator = new(
            [
                (MOUSE_BUTTON_STATE.NONE, RawInputModifiers.None),
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
                (MOUSE_BUTTON_STATE.FROM_LEFT_4TH_BUTTON_PRESSED, RawPointerEventType.XButton2Down),
                (MOUSE_BUTTON_STATE.NONE, RawPointerEventType.LeaveWindow) // ugh. that's default
            ]);

        private static readonly FlagTranslator<MOUSE_BUTTON_STATE, RawPointerEventType>
            MouseButtonUpEventTypeTranslator = new(
            [
                (MOUSE_BUTTON_STATE.FROM_LEFT_1ST_BUTTON_PRESSED, RawPointerEventType.LeftButtonUp),
                (MOUSE_BUTTON_STATE.RIGHTMOST_BUTTON_PRESSED, RawPointerEventType.RightButtonUp),
                (MOUSE_BUTTON_STATE.FROM_LEFT_2ND_BUTTON_PRESSED, RawPointerEventType.MiddleButtonUp),
                (MOUSE_BUTTON_STATE.FROM_LEFT_3RD_BUTTON_PRESSED, RawPointerEventType.XButton1Up),
                (MOUSE_BUTTON_STATE.FROM_LEFT_4TH_BUTTON_PRESSED, RawPointerEventType.XButton2Up),
                (MOUSE_BUTTON_STATE.NONE, RawPointerEventType.LeaveWindow) // ugh. that's default
            ]);


        private readonly WindowsConsole _windowsConsole;

        private MOUSE_BUTTON_STATE _mouseButtonsState = MOUSE_BUTTON_STATE.NONE;

        public Win32Console()
        {
            _windowsConsole = new WindowsConsole();

            StartEventLoop();
        }

        public override bool SupportsAltSolo => true;
        public override bool SupportsMouse => true;
        public override bool SupportsMouseMove => true;

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
            if (!WriteConsoleInput(_windowsConsole.InputHandle, inputRecords, 1, out uint _))
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
                    foreach (var inputRecord in readConsoleInput)
                        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                        switch (inputRecord.EventType)
                        {
                            case EVENT_TYPE.WINDOW_BUFFER_SIZE_EVENT:
                                //var  windowBufferSize = inputRecord.Event.WindowBufferSizeEvent;
                                //Size = new PixelBufferSize((ushort)windowBufferSize.dwSize.X, (ushort)windowBufferSize.dwSize.Y);
                                ActualizeSize();
                                break;
                            case EVENT_TYPE.FOCUS_EVENT:
                                var focusEvent = inputRecord.Event.FocusEvent;
                                RaiseFocusEvent(focusEvent.bSetFocus != 0);
                                break;
                            case EVENT_TYPE.KEY_EVENT:
                                HandleKeyInput(inputRecord.Event.KeyEvent);
                                break;
                            case EVENT_TYPE.MOUSE_EVENT:
                                var mouseEvent = inputRecord.Event.MouseEvent;
                                HandleMouseInput(mouseEvent);
                                break;
                        }
                }
            });
        }

        private bool HandleMouseInput(MOUSE_EVENT_RECORD mouseEvent)
        {
            var point = new Point(mouseEvent.dwMousePosition.X, mouseEvent.dwMousePosition.Y);
            RawInputModifiers inputModifiers =
                            KeyModifiersTranslator.Translate(mouseEvent.dwControlKeyState) |
                            MouseModifiersTranslator.Translate(mouseEvent.dwButtonState);

            RawPointerEventType eventType = RawPointerEventType.Move;
            Vector? wheelDelta = null;

            switch (mouseEvent.dwEventFlags)
            {
                case MOUSE_EVENT_FLAG.DOUBLE_CLICK:
                    var downButtonEvent = MouseButtonDownEventTypeTranslator.Translate(mouseEvent.dwButtonState);
                    if (downButtonEvent != RawPointerEventType.LeaveWindow)
                    {
                        var upButtonEvent = MouseButtonUpEventTypeTranslator.Translate(mouseEvent.dwButtonState);
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
                    }
                    break;

                case MOUSE_EVENT_FLAG.NONE:
                    foreach (var flag in Enum.GetValues<MOUSE_BUTTON_STATE>())
                    {
                        if (!_mouseButtonsState.HasFlag(flag) && mouseEvent.dwButtonState.HasFlag(flag))
                        {
                            // If we went from flag off to flag on
                            var buttonEventType = MouseButtonDownEventTypeTranslator.Translate(flag);
                            if (buttonEventType != default)
                            {
                                RaiseMouseEvent(buttonEventType,
                                    point,
                                    null,
                                    inputModifiers);
                            }
                        }

                        else if (_mouseButtonsState.HasFlag(flag) && !mouseEvent.dwButtonState.HasFlag(flag))
                        {
                            // If we went from flag On to flag off
                            var buttonEventType = MouseButtonUpEventTypeTranslator.Translate(flag);
                            if (buttonEventType != default)
                            {
                                RaiseMouseEvent(buttonEventType,
                                    point,
                                    null,
                                    inputModifiers);
                            }
                        }
                        else
                        {
                            RaiseMouseEvent(eventType,
                                point,
                                null,
                                inputModifiers);
                        }
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
            return true;
        }

        private void HandleKeyInput(KEY_EVENT_RECORD keyEvent)
        {
            char character = keyEvent.uChar;
            RawInputModifiers modifiers =
                KeyModifiersTranslator.Translate(keyEvent.dwControlKeyState);
            Key key = DefaultNetConsole.ConvertToKey((ConsoleKey)keyEvent.wVirtualKeyCode);
            if (key == Key.LeftAlt || key == Key.RightAlt)
                modifiers |= RawInputModifiers.Alt;
            RaiseKeyPress(key,
                character, modifiers, keyEvent.bKeyDown, (ulong)Stopwatch.GetTimestamp());
        }
    }
}