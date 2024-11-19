using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using Consolonia.Core.Drawing;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Dummy;
using Consolonia.Core.Infrastructure;
using NUnit.Framework;

#pragma warning disable CA1305 // Specify IFormatProvider

namespace Consolonia.Core.Tests
{
    public class ContextApp : Application
    {
    }

    [TestFixture]
    public class DrawingContextImplTests : IDisposable
    {
        [OneTimeSetUp]
        public void Setup()
        {
            _scope = AvaloniaLocator.EnterScope();
            _lifetime = ApplicationStartup.BuildLifetime<ContextApp>(new DummyConsole(), Array.Empty<string>());
        }

        private IDisposable _scope;
        private ClassicDesktopStyleApplicationLifetime _lifetime;
        private bool _disposedValue;

        [Test]
        public void BufferInitialized()
        {
            ArgumentNullException.ThrowIfNull(_lifetime);
            var consoleWindow = new ConsoleWindow();
            PixelBuffer buffer = consoleWindow.PixelBuffer;

            for (ushort y = 0; y < buffer.Height; y++)
                for (ushort x = 0; x < buffer.Width; x++)
                {
                    Pixel pixel = buffer[x, y];
                    Assert.IsTrue(pixel.Width == 1);
                    Assert.IsTrue(pixel.Foreground.Symbol.Text == " ");
                    Assert.IsTrue(pixel.Foreground.Color == Colors.Transparent);
                    Assert.IsTrue(pixel.Background.Color == Colors.Transparent);
                }
        }

        [Test]
        public void DrawText()
        {
            var consoleWindow = new ConsoleWindow();
            PixelBuffer buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);
            for (ushort x = 1; x < 6; x++) DrawText(dc, x, 2, x.ToString(), Brushes.White);
            for (ushort x = 1; x < 6; x++)
            {
                Assert.IsTrue(buffer[x, 2].Width == 1);
                Assert.IsTrue(buffer[x, 2].Foreground.Symbol.Text == x.ToString());
            }
        }

        [Test]
        public void DrawSingleWide()
        {
            var consoleWindow = new ConsoleWindow();
            PixelBuffer buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);
            for (ushort x = 0; x < 10; x++) DrawText(dc, x, 1, x.ToString(), Brushes.White);
            DrawText(dc, 5, 1, "X", Brushes.Blue);
            for (ushort x = 0; x < 10; x++)
                if (x == 5)
                {
                    Assert.IsTrue(buffer[x, 1].Width == 1);
                    Assert.IsTrue(buffer[x, 1].Foreground.Symbol.Text == "X");
                    Assert.IsTrue(buffer[x, 1].Foreground.Color == Colors.Blue);
                }
                else
                {
                    Assert.IsTrue(buffer[x, 1].Width == 1);
                    Assert.IsTrue(buffer[x, 1].Foreground.Symbol.Text == x.ToString());
                }
        }

        [Test]
        public void DrawDoubleWide()
        {
            var consoleWindow = new ConsoleWindow();
            PixelBuffer buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);

            for (ushort x = 0; x < 10; x++) DrawText(dc, x, 0, x.ToString(), Brushes.White);
            DrawText(dc, 5, 0, "🏳️‍🌈", Brushes.Blue);
            for (ushort x = 0; x < 10; x++)
            {
                Pixel pixel = buffer[x, 0];
                if (x == 5)
                {
                    Assert.IsTrue(pixel.Width == 2);
                    Assert.IsTrue(pixel.Foreground.Symbol.Text == "🏳️‍🌈");
                    Assert.IsTrue(pixel.Foreground.Color == Colors.Blue);
                }
                else if (x == 6)
                {
                    Assert.IsTrue(pixel.Width == 0);
                    Assert.IsTrue(pixel.Foreground.Symbol.Text.Length == 0);
                }
                else
                {
                    Assert.IsTrue(pixel.Width == 1);
                    Assert.IsTrue(pixel.Foreground.Symbol.Text == x.ToString());
                }
            }
        }

        [Test]
        public void DrawOverDoubleWideFirstChar()
        {
            var consoleWindow = new ConsoleWindow();
            PixelBuffer buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);

            for (ushort x = 0; x < 10; x++) DrawText(dc, x, 0, x.ToString(), Brushes.White);
            DrawText(dc, 5, 0, "🏳️‍🌈", Brushes.Blue);
            DrawText(dc, 5, 0, "X", Brushes.Red);
            for (ushort x = 0; x < 10; x++)
            {
                Pixel pixel = buffer[x, 0];
                if (x == 5)
                {
                    Assert.IsTrue(pixel.Width == 1);
                    Assert.IsTrue(pixel.Foreground.Symbol.Text == "X");
                    Assert.IsTrue(pixel.Foreground.Color == Colors.Red);
                }
                else if (x == 6)
                {
                    Assert.IsTrue(pixel.Width == 1);
                    Assert.IsTrue(pixel.Foreground.Symbol.Text == " ");
                }
                else
                {
                    Assert.IsTrue(pixel.Width == 1);
                    Assert.IsTrue(pixel.Foreground.Symbol.Text == x.ToString());
                }
            }
        }

        [Test]
        public void DrawOverDoubleWideSecondChar()
        {
            var consoleWindow = new ConsoleWindow();
            PixelBuffer buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);

            for (ushort x = 0; x < 10; x++) DrawText(dc, x, 0, x.ToString(), Brushes.White);
            DrawText(dc, 5, 0, "🏳️‍🌈", Brushes.Blue);
            DrawText(dc, 6, 0, "X", Brushes.Red);
            for (ushort x = 0; x < 10; x++)
            {
                Pixel pixel = buffer[x, 0];
                if (x == 5)
                {
                    Assert.IsTrue(pixel.Width == 1);
                    Assert.IsTrue(pixel.Foreground.Symbol.Text == " ");
                    Assert.IsTrue(pixel.Foreground.Color == Colors.Transparent);
                }
                else if (x == 6)
                {
                    Assert.IsTrue(pixel.Width == 1);
                    Assert.IsTrue(pixel.Foreground.Symbol.Text == "X");
                    Assert.IsTrue(pixel.Foreground.Color == Colors.Red);
                }
                else
                {
                    Assert.IsTrue(pixel.Width == 1);
                    Assert.IsTrue(pixel.Foreground.Symbol.Text == x.ToString());
                }
            }
        }


        [Test]
        public void DrawLineStrikethrough()
        {
            var consoleWindow = new ConsoleWindow();
            PixelBuffer buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);

            DrawText(dc, 1, 0, "hello", Brushes.Blue);
            SetOrigin(dc, 1, 0);
            dc.DrawLine(new Pen(Brushes.White, DrawingContextImpl.StrikethroughThickness), new Point(0, 0), new Point(6, 0));
            Assert.IsTrue(buffer[0, 0].Foreground.TextDecoration == null);
            Assert.IsTrue(buffer[1, 0].Foreground.TextDecoration == TextDecorationLocation.Strikethrough);
            Assert.IsTrue(buffer[2, 0].Foreground.TextDecoration == TextDecorationLocation.Strikethrough);
            Assert.IsTrue(buffer[3, 0].Foreground.TextDecoration == TextDecorationLocation.Strikethrough);
            Assert.IsTrue(buffer[4, 0].Foreground.TextDecoration == TextDecorationLocation.Strikethrough);
            Assert.IsTrue(buffer[5, 0].Foreground.TextDecoration == TextDecorationLocation.Strikethrough);
            Assert.IsTrue(buffer[6, 0].Foreground.TextDecoration == null);
        }

        [Test]
        public void DrawLineUnderline()
        {
            var consoleWindow = new ConsoleWindow();
            PixelBuffer buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);

            DrawText(dc, 1, 1, "hello", Brushes.Blue);
            SetOrigin(dc, 1, 1);
            dc.DrawLine(new Pen(Brushes.White, DrawingContextImpl.UnderlineThickness), new Point(0, 0), new Point(6, 0));
            Assert.IsTrue(buffer[0, 1].Foreground.TextDecoration == null);
            Assert.IsTrue(buffer[1, 1].Foreground.TextDecoration == TextDecorationLocation.Underline);
            Assert.IsTrue(buffer[2, 1].Foreground.TextDecoration == TextDecorationLocation.Underline);
            Assert.IsTrue(buffer[3, 1].Foreground.TextDecoration == TextDecorationLocation.Underline);
            Assert.IsTrue(buffer[4, 1].Foreground.TextDecoration == TextDecorationLocation.Underline);
            Assert.IsTrue(buffer[5, 1].Foreground.TextDecoration == TextDecorationLocation.Underline);
            Assert.IsTrue(buffer[6, 1].Foreground.TextDecoration == null);
        }


        [Test]
        public void DrawHorizontalLine()
        {
            var consoleWindow = new ConsoleWindow();
            PixelBuffer buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);
            SetOrigin(dc, 1, 1);
            dc.DrawLine(new Pen(Brushes.White), new Point(0, 0), new Point(4, 0));
            for (ushort x = 0; x <= 6; x++)
                if (x == 0 || x == 6)
                {
                    Assert.IsTrue(buffer[x, 1].Foreground.Symbol.Text == " ");
                    Assert.IsTrue(buffer[x, 1].Foreground.Color == Colors.Transparent);
                }
                else
                {
                    Assert.IsTrue(buffer[x, 1].Foreground.Symbol.Text == "─");
                    Assert.IsTrue(buffer[x, 1].Foreground.Color == Colors.White);
                }
        }

        [Test]
        public void DrawVerticalLine()
        {
            var consoleWindow = new ConsoleWindow();
            PixelBuffer buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);
            SetOrigin(dc, 1, 1);

            dc.DrawLine(new Pen(Brushes.White), new Point(0, 0), new Point(0, 4));
            for (ushort y = 0; y <= 6; y++)
                if (y == 0 || y == 6)
                {
                    Assert.IsTrue(buffer[1, y].Foreground.Symbol.Text == " ");
                    Assert.IsTrue(buffer[1, y].Foreground.Color == Colors.Transparent);
                }
                else
                {
                    Assert.IsTrue(buffer[1, y].Foreground.Symbol.Text == "│");
                    Assert.IsTrue(buffer[1, y].Foreground.Color == Colors.White);
                }
        }

        [Test]
        public void DrawLinesCrossingMakeCross()
        {
            var consoleWindow = new ConsoleWindow();
            PixelBuffer buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);

            SetOrigin(dc, 1, 1);
            for (int y = 0; y < 5; y += 2)
                dc.DrawLine(new Pen(Brushes.White), new Point(0, y), new Point(4, y));

            for (int x = 0; x < 5; x += 2)
                dc.DrawLine(new Pen(Brushes.White), new Point(x, 0), new Point(x, 4));

            // line 1
            Assert.IsTrue(buffer[1, 1].Foreground.Symbol.Text == "┌");
            Assert.IsTrue(buffer[2, 1].Foreground.Symbol.Text == "─");
            Assert.IsTrue(buffer[3, 1].Foreground.Symbol.Text == "┬");
            Assert.IsTrue(buffer[4, 1].Foreground.Symbol.Text == "─");
            Assert.IsTrue(buffer[5, 1].Foreground.Symbol.Text == "┐");

            // line 2 
            Assert.IsTrue(buffer[1, 2].Foreground.Symbol.Text == "│");
            Assert.IsTrue(buffer[2, 2].Foreground.Symbol.Text == " ");
            Assert.IsTrue(buffer[3, 2].Foreground.Symbol.Text == "│");
            Assert.IsTrue(buffer[4, 2].Foreground.Symbol.Text == " ");
            Assert.IsTrue(buffer[5, 2].Foreground.Symbol.Text == "│");

            // line 3
            Assert.IsTrue(buffer[1, 3].Foreground.Symbol.Text == "├");
            Assert.IsTrue(buffer[2, 3].Foreground.Symbol.Text == "─");
            Assert.IsTrue(buffer[3, 3].Foreground.Symbol.Text == "┼");
            Assert.IsTrue(buffer[4, 3].Foreground.Symbol.Text == "─");
            Assert.IsTrue(buffer[5, 3].Foreground.Symbol.Text == "┤");


            // line 4
            Assert.IsTrue(buffer[1, 4].Foreground.Symbol.Text == "│");
            Assert.IsTrue(buffer[2, 4].Foreground.Symbol.Text == " ");
            Assert.IsTrue(buffer[3, 4].Foreground.Symbol.Text == "│");
            Assert.IsTrue(buffer[4, 4].Foreground.Symbol.Text == " ");
            Assert.IsTrue(buffer[5, 4].Foreground.Symbol.Text == "│");

            // line 5
            Assert.IsTrue(buffer[1, 5].Foreground.Symbol.Text == "└");
            Assert.IsTrue(buffer[2, 5].Foreground.Symbol.Text == "─");
            Assert.IsTrue(buffer[3, 5].Foreground.Symbol.Text == "┴");
            Assert.IsTrue(buffer[4, 5].Foreground.Symbol.Text == "─");
            Assert.IsTrue(buffer[5, 5].Foreground.Symbol.Text == "┘");
        }

        [Test]
        public void DrawSolidRectangle()
        {
            var consoleWindow = new ConsoleWindow();
            PixelBuffer buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);
            SetOrigin(dc, 1, 1);
            ushort left = 0;
            ushort top = 0;
            int width = 3;
            int height = 3;
            int right = left + width;
            int bottom = top + height;
            dc.DrawRectangle(Brushes.Blue, null, new Rect(left, top, width, height));

            top++;
            left++;
            bottom++;
            right++;
            for (ushort y = 0; y <= bottom; y++)
                for (ushort x = 0; x <= right; x++)
                    if (x == 0 || x == right)
                    {
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == " ");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Transparent);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Transparent);
                    }
                    else if (y == 0 || y == bottom)
                    {
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == " ");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Transparent);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Transparent);
                    }
                    else
                    {
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == " ");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Transparent);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
        }

        [Test]
        public void DrawSingleBox()
        {
            var consoleWindow = new ConsoleWindow();
            PixelBuffer buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);
            SetOrigin(dc, 1, 1);
            ushort left = 0;
            ushort top = 0;
            int width = 3;
            int height = 3;
            int right = left + width;
            int bottom = top + height;
            dc.DrawRectangle(Brushes.Blue, new Pen(Brushes.Red), new Rect(left, top, width, height));

            top++;
            left++;
            bottom++;
            right++;
            for (ushort y = 0; y <= bottom; y++)
                for (ushort x = 0; x <= right; x++)
                    if (x == 0 || x == right +1||
                        y == 0 || y == bottom + 1)
                    {
                        // outside of box
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == " ");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Transparent);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Transparent);
                    }
                    else if (x == left && y == top)
                    {
                        // upper left corner
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "┌");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else if (x == right && y == top)
                    {
                        // upper right corner
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "┐");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else if (x == right && y == bottom)
                    {
                        // lower right corner
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "┘");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else if (x == left && y == bottom)
                    {
                        // lower left corner
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "└");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else if (x == left && y >= top && y < bottom)
                    {
                        // left side
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "│");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else if (y == top && x >= left + 1 && x < right - 1)
                    {
                        //top side
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "─");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else if (x == right && y >= top && y < bottom)
                    {
                        // right side
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "│");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else if (y == bottom && x >= left && x < right)
                    {
                        // bottom side
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "─");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else if ((x >= left+1 && x < right - 1) ||
                             (y >= top+1 && y < bottom-1))
                    {
                        // inside
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == " ");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Transparent);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
        }

        [Test]
        public void DrawDoubleBox()
        {
            var consoleWindow = new ConsoleWindow();
            PixelBuffer buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);
            SetOrigin(dc, 1, 1);
            ushort left = 0;
            ushort top = 0;
            int width = 3;
            int height = 3;
            int right = left + width;
            int bottom = top + height;
            var brush = new LineBrush { Brush = Brushes.Red, LineStyle = LineStyle.DoubleLine };
            dc.DrawRectangle(Brushes.Blue, new Pen(brush), new Rect(left, top, width, height));

            top++;
            left++;
            bottom++;
            right++;
            for (ushort y = 0; y <= bottom; y++)
                for (ushort x = 0; x <= right; x++)
                    if (x == 0 || x == right + 1 ||
                        y == 0 || y == bottom + 1)
                    {
                        // outside of box
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == " ");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Transparent);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Transparent);
                    }
                    else if (x == left && y == top)
                    {
                        // upper left corner
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "╔");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else if (x == right && y == top)
                    {
                        // upper right corner
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "╗");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else if (x == right && y == bottom)
                    {
                        // lower right corner
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "╝");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else if (x == left && y == bottom)
                    {
                        // lower left corner
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "╚");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else if (x == left && y >= top && y < bottom)
                    {
                        // left side
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "║");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else if (y == top && x >= left && x < right)
                    {
                        //top side
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "═");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else if (x == right && y >= top && y < bottom)
                    {
                        // right side
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "║");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else if (y == bottom && x >= left && x < right)
                    {
                        // bottom side
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "═");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else if ((x >= left + 1 && x < right - 1) ||
                             (y >= top + 1 && y < bottom - 1))
                    {
                        // inside
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == " ");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Transparent);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
        }

        internal static void DrawText(DrawingContextImpl dc, ushort x, ushort y, string text, IBrush brush)
        {
            SetOrigin(dc, x, y);
            var platformRender = AvaloniaLocator.Current.GetService<IPlatformRenderInterface>();
            var textShaper = AvaloniaLocator.Current.GetService<ITextShaperImpl>();
            var fontManager = AvaloniaLocator.Current.GetService<IFontManagerImpl>();
            fontManager.TryCreateGlyphTypeface("Cascadia Mono", FontStyle.Normal, FontWeight.Normal, FontStretch.Normal,
                out IGlyphTypeface typeface);
            ArgumentNullException.ThrowIfNull(typeface);
            ShapedBuffer glyphs =
                textShaper.ShapeText(text.AsMemory(), new TextShaperOptions(typeface, typeface.Metrics.DesignEmHeight));
            //var shapedText = new GlyphRun(typeface,
            //    1,
            //    text.AsMemory(),
            //    glyphs,
            //    default(Point),
            //    0);
            IGlyphRunImpl glyphRunImpl = platformRender.CreateGlyphRun(typeface, 1, glyphs, default);
            dc.DrawGlyphRun(brush, glyphRunImpl);
        }

        internal static PixelBufferCoordinate SetOrigin(DrawingContextImpl dc, ushort x, ushort y)
        {
            dc.Transform = new Matrix(1, 0, 0, 1, x, y);
            return new PixelBufferCoordinate(x, y);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    _scope?.Dispose();
                    _lifetime?.Dispose();
                    _scope = null;
                    _lifetime = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DrawingContextImplTests()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}