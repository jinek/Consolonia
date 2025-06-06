using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Media;
using Avalonia.Threading;
using Consolonia.Controls;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;
using Window = Avalonia.Controls.Window;

namespace Consolonia.NUnit
{
    public sealed class UnitTestConsole : IConsole
    {
        private PixelBufferCoordinate _fakeCaretPosition;
        private ConsoloniaLifetime _lifetime;

        public UnitTestConsole(PixelBufferSize size)
        {
            Size = size;
            PixelBuffer = new PixelBuffer(size.Width, size.Height);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public PixelBuffer PixelBuffer { get; }

        public PixelBufferSize Size { get; set; }

        public bool SupportsComplexEmoji => true;
        public bool SupportsAltSolo => true;
        public bool SupportsMouse => false;
        public bool SupportsMouseMove => false;

        public void SetTitle(string title)
        {
            Console.WriteLine($"Title changed to {title}");
        }

        public void SetCaretPosition(PixelBufferCoordinate bufferPoint)
        {
            _fakeCaretPosition = bufferPoint;
        }

        public PixelBufferCoordinate GetCaretPosition()
        {
            return _fakeCaretPosition;
        }

        public void Print(PixelBufferCoordinate bufferPoint, Color background, Color foreground, FontStyle? style,
            FontWeight? weight, TextDecorationLocation? textDecoration, string str)
        {
            (ushort x, ushort y) = bufferPoint;

            int i = 0;
            foreach (Rune rune in str.EnumerateRunes())
            {
                PixelBuffer.Set(new PixelBufferCoordinate((ushort)(x + i), y), _ =>
                    // ReSharper disable once AccessToModifiedClosure we are sure about inline execution
                    new Pixel(
                        new PixelForeground(new SimpleSymbol(rune), foreground, style: style, weight: weight,
                            textDecoration: textDecoration),
                        new PixelBackground(background)));
                i++;
            }
        }

        public void WriteText(string str)
        {
            // ignore
        }

        public void PauseIO(Task task)
        {
            throw new NotSupportedException();
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

        public void Dispose()
        {
            _lifetime = null;
        }

        public void ClearOutput()
        {
            // screen of the unit test console is always clear, we are writing only to the buffer
            Resized?.Invoke();
        }


        public async Task StringInput(string input)
        {
            foreach (char c in input)
            {
                const Key key = Key.None;
                ulong timestamp = (ulong)Environment.TickCount64;
                // todo: check why Yield is not enough: https://github.com/jinek/Consolonia/runs/7055199426?check_suite_focus=true
                const ulong interval = 50;
                KeyEvent?.Invoke(key, c, RawInputModifiers.None, true, timestamp, true);
                await Task.Delay((int)interval).ConfigureAwait(false);
                KeyEvent?.Invoke(key, c, RawInputModifiers.None, false, timestamp + interval, true);
                await Task.Delay((int)interval).ConfigureAwait(false);
            }

            await WaitRendered().ConfigureAwait(true);
        }

        public async Task WaitRendered()
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var mainWindow = _lifetime.TopLevel as Window;
                mainWindow!.InvalidateVisual();
                await mainWindow.PlatformImpl!.Compositor!.RequestCompositionBatchCommitAsync().Rendered
                    .ConfigureAwait(true);
                await mainWindow.PlatformImpl!.Compositor!.RequestCompositionBatchCommitAsync().Processed
                    .ConfigureAwait(true);
            }, DispatcherPriority.Render).ConfigureAwait(true);

            await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Render);
        }

        public async Task KeyInput(params Key[] keys)
        {
            foreach (Key key in keys) await KeyInput(key).ConfigureAwait(true);
        }

        public async Task KeyInput(int repeat, Key key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            foreach (int _ in Enumerable.Range(0, repeat)) await KeyInput(key, modifiers).ConfigureAwait(true);
        }

        public async Task KeyInput(Key key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            ulong timestamp = (ulong)Environment.TickCount64;
            KeyEvent?.Invoke(key, char.MinValue /*will be skipped as control character*/, modifiers, true, timestamp,
                true);
            await Task.Yield();
            KeyEvent?.Invoke(key, char.MinValue /*will be skipped as control character*/, modifiers, false,
                timestamp + 1, true);
            await Task.Yield();

            await WaitRendered().ConfigureAwait(true);
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
                    Pixel pixel = PixelBuffer[new PixelBufferCoordinate(i, j)];
                    string text = pixel.IsCaret() ? "Ꮖ" : pixel.Foreground.Symbol.Text;
                    //todo: check why cursor is not drawing
                    stringBuilder.Append(text);
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        public void SetupLifetime(ConsoloniaLifetime lifetime)
        {
            _lifetime = lifetime;
        }


#pragma warning disable CS0067
        public event Action Resized;
        public event Action<Key, char, RawInputModifiers, bool, ulong, bool> KeyEvent;
        public event Action<RawPointerEventType, Point, Vector?, RawInputModifiers> MouseEvent;
        public event Action<bool> FocusEvent;
        public event Action<string, ulong, CanBeHandledEventArgs> TextInputEvent;
#pragma warning restore CS0067
    }
}