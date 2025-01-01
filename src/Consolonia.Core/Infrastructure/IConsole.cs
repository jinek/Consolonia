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
    /// IConsole is made up of IConsoleInput and IConsoleOutput
    /// </summary>
    public interface IConsole : IConsoleOutput
    {
        bool SupportsMouse { get; }

        bool SupportsMouseMove { get; }

        public event Action Resized;

        event Action<Key, char, RawInputModifiers, bool, ulong> KeyEvent;
        event Action<RawPointerEventType, Point, Vector?, RawInputModifiers> MouseEvent;

        event Action<bool> FocusEvent;

        public void PauseIO(Task task);
    }
}