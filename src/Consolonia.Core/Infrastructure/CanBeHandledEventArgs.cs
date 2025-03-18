using System;

namespace Consolonia.Core.Infrastructure
{
    public class CanBeHandledEventArgs : EventArgs
    {
        public bool Handled { get; set; }
        
        public static CanBeHandledEventArgs Default => new();
    }
}