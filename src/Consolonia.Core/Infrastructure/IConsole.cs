using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    ///     IConsole is made up of IConsoleInput and IConsoleOutput
    /// </summary>
    public interface IConsole : IConsoleOutput
    {
        /// <summary>
        ///     Does this support detection of Alt key by itself
        /// </summary>
        bool SupportsAltSolo { get; }

        /// <summary>
        ///     Does this support mouse input
        /// </summary>
        bool SupportsMouse { get; }

        /// <summary>
        ///     Does this support mouse move input
        /// </summary>
        bool SupportsMouseMove { get; }

        public event Action Resized;

        event Action<Key, char, RawInputModifiers, bool, ulong, bool> KeyEvent;
        event Action<string, ulong, CanBeHandledEventArgs> TextInputEvent;
        event Action<RawPointerEventType, Point, Vector?, RawInputModifiers> MouseEvent;

        event Action<bool> FocusEvent;

        public void PauseIO(Task task);
    }
}