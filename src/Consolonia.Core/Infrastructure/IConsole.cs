using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    /// IConsole is made up of IConsoleInput and IConsoleOutput
    /// </summary>
    public interface IConsole : IConsoleInput, IConsoleOutput
    {
        public void PauseIO(Task task);
    }
}