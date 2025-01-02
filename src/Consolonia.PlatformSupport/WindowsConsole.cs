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
            ModifiersFlagTranslator = new(
            [
                (CONTROL_KEY_STATE.SHIFT_PRESSED, RawInputModifiers.Shift),
                (CONTROL_KEY_STATE.LEFT_ALT_PRESSED, RawInputModifiers.Alt),
                (CONTROL_KEY_STATE.RIGHT_ALT_PRESSED, RawInputModifiers.Alt),
                (CONTROL_KEY_STATE.LEFT_CTRL_PRESSED, RawInputModifiers.Control),
                (CONTROL_KEY_STATE.RIGHT_CTRL_PRESSED, RawInputModifiers.Control)
            ]);

        private static readonly FlagTranslator<MOUSE_BUTTON_STATE, RawPointerEventType>
            MouseButtonFlagTranslator = new(
            [
                (MOUSE_BUTTON_STATE.FROM_LEFT_1ST_BUTTON_PRESSED, RawPointerEventType.LeftButtonDown),
                (MOUSE_BUTTON_STATE.RIGHTMOST_BUTTON_PRESSED, RawPointerEventType.RightButtonDown),
                (MOUSE_BUTTON_STATE.FROM_LEFT_2ND_BUTTON_PRESSED, RawPointerEventType.MiddleButtonDown),
                (MOUSE_BUTTON_STATE.FROM_LEFT_3RD_BUTTON_PRESSED, RawPointerEventType.XButton1Down),
                (MOUSE_BUTTON_STATE.FROM_LEFT_4TH_BUTTON_PRESSED, RawPointerEventType.XButton2Down)
            ]);

        private static readonly FlagTranslator<MOUSE_BUTTON_STATE, RawInputModifiers>
            MouseModifiersFlagTranslator = new(
            [
                (MOUSE_BUTTON_STATE.FROM_LEFT_1ST_BUTTON_PRESSED, RawInputModifiers.LeftMouseButton),
                (MOUSE_BUTTON_STATE.RIGHTMOST_BUTTON_PRESSED, RawInputModifiers.RightMouseButton),
                (MOUSE_BUTTON_STATE.FROM_LEFT_2ND_BUTTON_PRESSED, RawInputModifiers.MiddleMouseButton),
                (MOUSE_BUTTON_STATE.FROM_LEFT_3RD_BUTTON_PRESSED, RawInputModifiers.XButton1MouseButton),
                (MOUSE_BUTTON_STATE.FROM_LEFT_4TH_BUTTON_PRESSED, RawInputModifiers.XButton2MouseButton)
            ]);

        private readonly WindowsConsole _windowsConsole;

        private MOUSE_BUTTON_STATE _mouseButtonsState;

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

                                if (HandleMouseInput(mouseEvent)) return; //todo: implement
                                break;
                        }
                }
            });
        }

        private bool HandleMouseInput(MOUSE_EVENT_RECORD mouseEvent)
        {
            var point = new Point(mouseEvent.dwMousePosition.X,
                mouseEvent.dwMousePosition.Y);
            var modifiers = ModifiersFlagTranslator.Translate(mouseEvent.dwControlKeyState);
            var modifiers2 = MouseModifiersFlagTranslator.Translate(mouseEvent.dwButtonState);
            RawInputModifiers inputModifiers = modifiers | modifiers;
            RawPointerEventType eventType = default;
            Vector? wheelDelta = null;
            short repeat = 1;

            switch (mouseEvent.dwEventFlags)
            {
                case MOUSE_EVENT_FLAG.DOUBLE_CLICK:
                    repeat = 2; //todo: now supporting only leftbutton
                    eventType = RawPointerEventType.LeftButtonDown;
                    break;
                case default(MOUSE_EVENT_FLAG):
                    MOUSE_BUTTON_STATE xor = _mouseButtonsState ^ mouseEvent.dwButtonState;
                    foreach (RawPointerEventType pointerEventType in (xor &
                                 mouseEvent.dwButtonState).GetFlags()
                             .Select(MouseButtonFlagTranslator.Translate))
                        //todo: вернуть mouse gesture на элементы
                        RaiseMouseEvent(pointerEventType,
                            point,
                            null,
                            inputModifiers);

                    //todo: refactor: code clone
                    foreach (RawPointerEventType pointerEventType in (xor &
                                 _mouseButtonsState).GetFlags()
                             .Select(MouseButtonFlagTranslator.Translate))
                    {
                        RawPointerEventType rawPointerEventType = pointerEventType + 1;
                        RaiseMouseEvent(rawPointerEventType,
                            point,
                            null,
                            inputModifiers);
                    }

                    _mouseButtonsState = mouseEvent.dwButtonState;
                    repeat = 0;
                    break;
                case MOUSE_EVENT_FLAG.MOUSE_WHEELED:
                    double velocity = mouseEvent.dwButtonState < 0 ? -1 : 1;
                    wheelDelta = new Vector(0, velocity);
                    eventType = RawPointerEventType.Wheel;
                    break;
                case MOUSE_EVENT_FLAG.MOUSE_HWHEELED:
                    return true;
                case MOUSE_EVENT_FLAG.MOUSE_MOVED:
                    eventType = RawPointerEventType.Move;
                    break;
                case MOUSE_EVENT_FLAG.MOUSE_MOVED | MOUSE_EVENT_FLAG.DOUBLE_CLICK:
                    RaiseMouseEvent(RawPointerEventType.LeftButtonDown, point, null, inputModifiers);
                    RaiseMouseEvent(RawPointerEventType.Move, point, null, inputModifiers);
                    return false;
                default:
                    throw new InvalidOperationException(mouseEvent.dwEventFlags.ToString());
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

        private void HandleKeyInput(KEY_EVENT_RECORD keyEvent)
        {
            char character = keyEvent.uChar;
            RawInputModifiers modifiers =
                ModifiersFlagTranslator.Translate(keyEvent.dwControlKeyState);
            Key key = DefaultNetConsole.ConvertToKey((ConsoleKey)keyEvent.wVirtualKeyCode);
            if (key == Key.LeftAlt || key == Key.RightAlt)
                modifiers |= RawInputModifiers.Alt;
            RaiseKeyPress(key,
                character, modifiers, keyEvent.bKeyDown, (ulong)Stopwatch.GetTimestamp());
        }
    }
}