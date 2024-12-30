using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    /// Define properties and events for input.
    /// </summary>
    public interface IConsoleInput 
    {
        bool SupportsMouse { get; }

        bool SupportsMouseMove { get; }

        event Action<Key, char, RawInputModifiers, bool, ulong> KeyEvent;
        event Action<RawPointerEventType, Point, Vector?, RawInputModifiers> MouseEvent;

        event Action<bool> FocusEvent;
    }
}