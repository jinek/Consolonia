using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Consolonia.Core.Infrastructure;
using Consolonia.Core.InternalHelpers;

// ReSharper disable UnusedMember.Local
#pragma warning disable CS0649

namespace Consolonia.Windows
{
    public class WinConsole : InputLessDefaultNetConsole
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

        public WinConsole()
        {
            _windowsConsole = new WindowsConsole();

            StartEventLoop();
        }

        /// <summary>
        ///     From https://stackoverflow.com/a/66275102/2362847
        /// </summary>
        public static IEnumerable<T> GetFlags<T>(T en) where T : struct, Enum
        {
            return Enum.GetValues<T>().Where(member => en.HasFlag(member));
        }

        private void StartEventLoop()
        {
            Task.Run(() =>
            {
                while (!Disposed /*inject ThreadAbortException*/)
                {
                    var readConsoleInput = _windowsConsole.ReadConsoleInput();
                    if (!readConsoleInput.Any())
                        throw new NotImplementedException();
                    foreach (WindowsConsole.InputRecord inputRecord in readConsoleInput)
                        switch (inputRecord.EventType)
                        {
                            case WindowsConsole.EventType.WindowBufferSize:
                                ActualizeTheSize();
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
            var point = new Avalonia.Point(mouseEvent.MousePosition.X,
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
                    foreach (RawPointerEventType pointerEventType in GetFlags(
                                     (WindowsConsole.ButtonState)(xor & incomeMouseState))
                                 .Select(MouseButtonFlagTranslator.Translate))
                        //todo: вернуть mouse gesture на элементы
                    {
                        RaiseMouseEvent(pointerEventType,
                            point,
                            null,
                            inputModifiers);
                        Debug.WriteLine($"Mouse {pointerEventType} {point} {inputModifiers}");
                    }

                    //todo: refactor: code clone
                    foreach (RawPointerEventType pointerEventType in GetFlags(
                                     (WindowsConsole.ButtonState)(xor & _mouseButtonsState))
                                 .Select(MouseButtonFlagTranslator.Translate))
                    {
                        RawPointerEventType rawPointerEventType = pointerEventType + 1;
                        RaiseMouseEvent(rawPointerEventType,
                            point,
                            null,
                            inputModifiers);
                        Debug.WriteLine($"Mouse {rawPointerEventType} {point} {inputModifiers}");
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
                    throw new InvalidOperationException();
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