using Avalonia.Media;
using Consolonia.Core.Drawing;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Dummy
{
    public class DummyConsole : ConsoleBase
    {
        public DummyConsole()
            : this(80, 25)
        {
        }

        public DummyConsole(ushort width, ushort height)
            : base(new DummyConsoleOutput())
        {
        }

        public override bool SupportsMouse => false;

        public override bool SupportsMouseMove => false;
    }

    public class DummyConsoleOutput : IConsoleOutput
    {
        private PixelBufferCoordinate _caretPosition = new(0, 0);
        private bool _disposed;

        public DummyConsoleOutput()
            : this(80, 25)
        {
        }

        public DummyConsoleOutput(ushort width, ushort height)
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

        public void ClearOutput()
        {
            SetCaretPosition(new PixelBufferCoordinate(0, 0));
        }

        public PixelBufferCoordinate GetCaretPosition()
        {
            return _caretPosition;
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

        public void SetCaretStyle(CaretStyle caretStyle)
        {
        }

        public void HideCaret()
        {
        }

        public void ShowCaret()
        {
        }

        public void PrepareConsole()
        {
        }

        public void RestoreConsole()
        {
        }

        public void ClearScreen()
        {
        }
    }
}