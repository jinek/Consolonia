using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Dummy
{
    public class DummyConsole : IConsole
    {
        private PixelBufferCoordinate _caretPosition = new(0, 0);
        private bool _disposed;

        public DummyConsole()
            : this(80, 25)
        {
        }

        public DummyConsole(ushort width, ushort height)
        {
            Size = new PixelBufferSize(width, height);
        }

        public PixelBufferSize Size { get; set; }

        public bool CaretVisible
        {
            get => false;
            set { }
        }

        public bool SupportsComplexEmoji => true;
        public bool SupportsAltSolo => false;
        public bool SupportsMouse => false;
        public bool SupportsMouseMove => false;

        public void ClearOutput()
        {
            SetCaretPosition(new PixelBufferCoordinate(0, 0));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public PixelBufferCoordinate GetCaretPosition()
        {
            return _caretPosition;
        }

        public void PauseIO(Task task)
        {
        }

        public void Print(PixelBufferCoordinate bufferPoint, Color background, Color foreground, FontStyle? style,
            FontWeight? weight, TextDecorationLocation? textDecoration, string str)
        {
        }

        public void SetCaretPosition(PixelBufferCoordinate bufferPoint)
        {
            _caretPosition = bufferPoint;
        }

        public void SetTitle(string title)
        {
        }

        public void WriteText(string str)
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources here.
                }

                // Dispose unmanaged resources here.

                _disposed = true;
            }
        }


#pragma warning disable CS0067 // never used
        public event Action Resized;
        public event Action<Key, char, RawInputModifiers, bool, ulong> KeyEvent;
        public event Action<RawPointerEventType, Point, Vector?, RawInputModifiers> MouseEvent;
        public event Action<bool> FocusEvent;
#pragma warning restore CS0067
    }
}