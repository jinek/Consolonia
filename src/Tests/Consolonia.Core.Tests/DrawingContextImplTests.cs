using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using Consolonia.Core.Drawing;
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
    public class DrawingContextImplTests
    {
        private ClassicDesktopStyleApplicationLifetime _lifetime;

        [OneTimeSetUp]
        public void Setup()
        {
            var scope = AvaloniaLocator.EnterScope();
            _lifetime = ApplicationStartup.BuildLifetime<ContextApp>(new DummyConsole(), Array.Empty<string>());
        }

        [Test]
        public void BufferInitialized()
        {
            var consoleWindow = new ConsoleWindow();
            var buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);

            for (ushort y = 0; y < buffer.Height; y++)
                for (ushort x = 0; x < buffer.Width; x++)
                {
                    var pixel = buffer[x, y];
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
            var buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);

            for (ushort x = 0; x < 10; x++)
            {
                DrawText(dc, x, 0, x.ToString(), Brushes.White);
            }
            for (ushort x = 0; x < 10; x++)
            {
                Assert.IsTrue(buffer[x, 0].Width == 1);
                Assert.IsTrue(buffer[x, 0].Foreground.Symbol.Text == x.ToString());
            }
        }

        [Test]
        public void DrawSingleWide()
        {
            var consoleWindow = new ConsoleWindow();
            var buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);

            for (ushort x = 0; x < 10; x++)
            {
                DrawText(dc, x, 0, x.ToString(), Brushes.White);
            }
            DrawText(dc, 5, 0, "X".ToString(), Brushes.Blue);
            for (ushort x = 0; x < 10; x++)
            {
                if (x == 5)
                {
                    Assert.IsTrue(buffer[5, 0].Width == 1);
                    Assert.IsTrue(buffer[5, 0].Foreground.Symbol.Text == "X");
                    Assert.IsTrue(buffer[5, 0].Foreground.Color == Colors.Blue);

                }
                else
                {
                    Assert.IsTrue(buffer[x, 0].Width == 1);
                    Assert.IsTrue(buffer[x, 0].Foreground.Symbol.Text == x.ToString());
                }
            }
        }

        [Test]
        public void DrawDoubleWide()
        {
            var consoleWindow = new ConsoleWindow();
            var buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);

            for (ushort x = 0; x < 10; x++)
            {
                DrawText(dc, x, 0, x.ToString(), Brushes.White);
            }
            DrawText(dc, 5, 0, "🏳️‍🌈", Brushes.Blue);
            for (ushort x = 0; x < 10; x++)
            {
                var pixel = buffer[x, 0];
                if (x == 5)
                {
                    Assert.IsTrue(pixel.Width == 2);
                    Assert.IsTrue(pixel.Foreground.Symbol.Text == "🏳️‍🌈");
                    Assert.IsTrue(pixel.Foreground.Color == Colors.Blue);
                }
                else if (x == 6)
                {
                    Assert.IsTrue(pixel.Width == 0);
                    Assert.IsTrue(pixel.Foreground.Symbol!.Text.Length == 0);
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
            var buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);

            for (ushort x = 0; x < 10; x++)
            {
                DrawText(dc, x, 0, x.ToString(), Brushes.White);
            }
            DrawText(dc, 5, 0, "🏳️‍🌈", Brushes.Blue);
            DrawText(dc, 5, 0, "X", Brushes.Red);
            for (ushort x = 0; x < 10; x++)
            {
                var pixel = buffer[x, 0];
                if (x == 5)
                {
                    Assert.IsTrue(pixel.Width == 1);
                    Assert.IsTrue(pixel.Foreground.Symbol.Text == "X");
                    Assert.IsTrue(pixel.Foreground.Color == Colors.Red);
                }
                else if (x == 6)
                {
                    Assert.IsTrue(pixel.Width == 1);
                    Assert.IsTrue(pixel.Foreground.Symbol!.Text == " ");
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
            var buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);

            for (ushort x = 0; x < 10; x++)
            {
                DrawText(dc, x, 0, x.ToString(), Brushes.White);
            }
            DrawText(dc, 5, 0, "🏳️‍🌈", Brushes.Blue);
            DrawText(dc, 6, 0, "X", Brushes.Red);
            for (ushort x = 0; x < 10; x++)
            {
                var pixel = buffer[x, 0];
                if (x == 5)
                {
                    Assert.IsTrue(pixel.Width == 1);
                    Assert.IsTrue(pixel.Foreground.Symbol.Text == " ");
                    Assert.IsTrue(pixel.Foreground.Color == Colors.Transparent);
                }
                else if (x == 6)
                {
                    Assert.IsTrue(pixel.Width == 1);
                    Assert.IsTrue(pixel.Foreground.Symbol!.Text == "X");
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
        public void DrawHorizontalLine()
        {
            var consoleWindow = new ConsoleWindow();
            var buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);

            dc.DrawLine(new Pen(Brushes.White), new Point(1, 0), new Point(5, 0));
            for (ushort x = 0; x <= 5; x++)
            {
                if (x == 0 || x == 5)
                {
                    Assert.IsTrue(buffer[x, 0].Foreground.Symbol.Text == " ");
                    Assert.IsTrue(buffer[x, 0].Foreground.Color == Colors.Transparent);
                }
                else
                {
                    Assert.IsTrue(buffer[x, 0].Foreground.Symbol.Text == "─");
                    Assert.IsTrue(buffer[x, 0].Foreground.Color == Colors.White);
                }
            }
        }

        [Test]
        public void DrawVerticalLine()
        {
            var consoleWindow = new ConsoleWindow();
            var buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);

            dc.DrawLine(new Pen(Brushes.White), new Point(0, 1), new Point(0, 5));
            for (ushort y = 0; y <= 5; y++)
            {
                if (y == 0 || y == 5)
                {
                    Assert.IsTrue(buffer[0, y].Foreground.Symbol.Text == " ");
                    Assert.IsTrue(buffer[0, y].Foreground.Color == Colors.Transparent);
                }
                else
                {
                    Assert.IsTrue(buffer[0, y].Foreground.Symbol.Text == "│");
                    Assert.IsTrue(buffer[0, y].Foreground.Color == Colors.White);
                }
            }
        }

        [Test]
        public void DrawLinesCrossingMakeCross()
        {
            var consoleWindow = new ConsoleWindow();
            var buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);

            for (int y = 1; y < 4; y++)
                dc.DrawLine(new Pen(Brushes.White), new Point(1, y), new Point(4, y));

            for (int x = 1; x < 4; x++)
                dc.DrawLine(new Pen(Brushes.White), new Point(x, 1), new Point(x, 4));

            // assert every line was crossed
            for (ushort y = 1; y < 4; y++)
                for (ushort x = 1; x < 4; x++)
                {
                    Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "┼");
                    Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.White);
                }


            // make sure no lines outside the bounds
            for (ushort i = 0; i < 5; i++)
            {
                Assert.IsTrue(buffer[0, i].Foreground.Symbol.Text == " ");
                Assert.IsTrue(buffer[i, 0].Foreground.Symbol.Text == " ");
                Assert.IsTrue(buffer[5, i].Foreground.Symbol.Text == " ");
                Assert.IsTrue(buffer[i, 5].Foreground.Symbol.Text == " ");
            }
        }

        [Test]
        public void DrawSolidRectangle()
        {
            var consoleWindow = new ConsoleWindow();
            var buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);
            ushort left = 1;
            ushort top = 1;
            var width = 3;
            var height = 3;
            var right = left + width;
            var bottom = top + height;
            dc.DrawRectangle(Brushes.Blue, null, new Rect(left, top, width, height));

            for (ushort y = 0; y <= right; y++)
                for (ushort x = 0; x <= right; x++)
                {
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
        }

        [Test]
        public void DrawSingleBox()
        {
            var consoleWindow = new ConsoleWindow();
            var buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);
            ushort left = 1;
            ushort top = 1;
            var width = 3;
            var height = 3;
            var right = left + width;
            var bottom = top + height;
            dc.DrawRectangle(Brushes.Blue, new Pen(Brushes.Red), new Rect(left, top, width, height));

            for (ushort y = 0; y <= height + 1; y++)
                for (ushort x = 0; x <= width + 1; x++)
                {
                    if ((x == 0 || x == right + 1) ||
                         (y == 0 || y == bottom + 1))
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
                    else if (y == top && x >= left && x < right)
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
                    else if (y == bottom && x >= left && x < bottom)
                    {
                        // bottom side
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "─");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else
                    {
                        // inside
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == " ");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Transparent);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                }
        }

        [Test]
        public void DrawDoubleBox()
        {
            var consoleWindow = new ConsoleWindow();
            var buffer = consoleWindow.PixelBuffer;
            var dc = new DrawingContextImpl(consoleWindow);
            ushort left = 1;
            ushort top = 1;
            var width = 3;
            var height = 3;
            var right = left + width;
            var bottom = top + height;
            var brush = new LineBrush() { Brush = Brushes.Red, LineStyle = LineStyle.DoubleLine };
            dc.DrawRectangle(Brushes.Blue, new Pen(brush), new Rect(left, top, width, height));

            for (ushort y = 0; y <= height + 1; y++)
                for (ushort x = 0; x <= width + 1; x++)
                {
                    if ((x == 0 || x == right + 1) ||
                         (y == 0 || y == bottom + 1))
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
                    else if (y == bottom && x >= left && x < bottom)
                    {
                        // bottom side
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == "═");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Red);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                    else
                    {
                        // inside
                        Assert.IsTrue(buffer[x, y].Foreground.Symbol.Text == " ");
                        Assert.IsTrue(buffer[x, y].Foreground.Color == Colors.Transparent);
                        Assert.IsTrue(buffer[x, y].Background.Color == Colors.Blue);
                    }
                }
        }

        internal static void DrawText(DrawingContextImpl dc, ushort x, ushort y, string text, IBrush brush)
        {
            dc.Transform = new Matrix(1, 0, 0, 1, x, y);
            var platformRender = AvaloniaLocator.Current.GetService<IPlatformRenderInterface>();
            var textShaper = AvaloniaLocator.Current.GetService<ITextShaperImpl>();
            var fontManager = AvaloniaLocator.Current.GetService<IFontManagerImpl>();
            fontManager.TryCreateGlyphTypeface("Cascadia Mono", FontStyle.Normal, FontWeight.Normal, FontStretch.Normal, out var typeface);
            ArgumentNullException.ThrowIfNull(typeface);
            ShapedBuffer glyphs =
                textShaper.ShapeText(text.AsMemory(), new TextShaperOptions(typeface, typeface.Metrics.DesignEmHeight));
            var shapedText = new GlyphRun(typeface,
                1,
                text.AsMemory(),
                glyphs,
                default(Point),
                0);
            var glyphRunImpl = platformRender.CreateGlyphRun(typeface, 1, glyphs, default(Point));
            dc.DrawGlyphRun(brush, glyphRunImpl);
        }

    }
}