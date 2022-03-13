using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Threading;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;

namespace Consolonia.TestsCore
{
    public sealed class UnitTestConsole : IConsole
    {
        private readonly PixelBufferSize _size;
        private PixelBufferCoordinate _fakeCaretPosition;
        private ClassicDesktopStyleApplicationLifetime _lifetime;

        public UnitTestConsole(PixelBufferSize size)
        {
            _size = size;
            PixelBuffer = new PixelBuffer(size.Width, size.Height);
        }

        public PixelBuffer PixelBuffer { get; }

        public void Dispose()
        {
            _lifetime = null;
        }

        PixelBufferSize IConsole.Size => _size;
        bool IConsole.CaretVisible { get; set; }

        void IConsole.SetCaretPosition(PixelBufferCoordinate bufferPoint)
        {
            _fakeCaretPosition = bufferPoint;
        }

        PixelBufferCoordinate IConsole.GetCaretPosition()
        {
            return _fakeCaretPosition;
        }

        void IConsole.Print(PixelBufferCoordinate bufferPoint, ConsoleColor backgroundColor,
            ConsoleColor foregroundColor,
            string str)
        {
            (ushort x, ushort y) = bufferPoint;

            for (int i = 0; i < str.Length; i++)
                PixelBuffer.Set(new PixelBufferCoordinate((ushort)(x + i), y), _ =>
                    new Pixel(new PixelForeground(new SimpleSymbol(str[i]), foregroundColor),
                        new PixelBackground(PixelBackgroundMode.Colored, backgroundColor)));
        }

        public event Action Resized;
        public event Action<Key, char, RawInputModifiers> KeyPress;

        public async Task StringInput(string input)
        {
            foreach (char c in input)
            {
                if (!Enum.TryParse(c.ToString(), true, out Key key))
                    throw new InvalidOperationException(
                        $"Character {(int)c} can not be converted to {typeof(Key).FullName}");
                KeyPress?.Invoke(key, c, RawInputModifiers.None);
            }

            await WaitDispatched();
        }

        public async Task WaitDispatched()
        {
            bool noDirtyRegions = false;
            while (!noDirtyRegions)
                await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        noDirtyRegions = ((ConsoleWindow)_lifetime.MainWindow.PlatformImpl)
                            .InvalidatedRects.Count == 0;
                    },
                    DispatcherPriority.ContextIdle);
        }

        public async Task KeyInput(params Key[] keys)
        {
            foreach (Key key in keys) await KeyInput(key);
        }

        public async Task KeyInput(int repeat, Key key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            foreach (int _ in Enumerable.Range(0, repeat)) await KeyInput(key, modifiers);
        }

        public async Task KeyInput(Key key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            KeyPress(key, char.MinValue /*will be skipped as control character*/, modifiers);

            await WaitDispatched();
        }

        internal string PrintBuffer()
        {
            var stringBuilder = new StringBuilder();

            for (ushort j = 0; j < PixelBuffer.Height; j++)
            {
                for (ushort i = 0; i < PixelBuffer.Width; i++)
                {
                    if (i == PixelBuffer.Width - 1 && j == PixelBuffer.Height - 1)
                        break;
                    stringBuilder.Append(PixelBuffer[new PixelBufferCoordinate(i, j)].Foreground.Symbol.GetCharacter());
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        public void SetupLifetime(ClassicDesktopStyleApplicationLifetime lifetime)
        {
            _lifetime = lifetime;
        }
    }
}