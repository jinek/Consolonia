using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Threading;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Newtonsoft.Json;
using SteamCMD.ConPTY;
using SteamCMD.ConPTY.Interop.Definitions;

namespace Consolonia.Core.Designer
{
    /// <summary>
    /// ConsolePreview is a control that can be used to preview the output of a console AXAML file in visual studio.
    /// </summary>
    public class ConsolePreview : UserControl
    {
        private ProcessInfo _process;
        private WindowsPseudoConsole _psuedoConsole;
        private Typeface _typeface = new Typeface("Consolas");
        private double _charWidth = 0;
        private double _charHeight = 0;

        public static readonly StyledProperty<string> FileNameProperty =
            AvaloniaProperty.Register<ConsolePreview, string>(nameof(FileName));

        public static readonly StyledProperty<ushort> RowsProperty =
            AvaloniaProperty.Register<ConsolePreview, ushort>(nameof(Rows));

        public static readonly StyledProperty<ushort> ColumnsProperty =
            AvaloniaProperty.Register<ConsolePreview, ushort>(nameof(Columns));

        public ConsolePreview()
        {
            this.FontFamily = FontFamily.Parse("Consolas, Courier New");

            Initialized += (sender, e) => LoadXaml();
        }


        /// <summary>
        /// name of the axaml file to preview.
        /// </summary>
        public string FileName { get => GetValue(FileNameProperty); set => SetValue(FileNameProperty, value); }

        /// <summary>
        /// Number of columns in the console 
        /// </summary>
        public ushort Columns { get => GetValue(ColumnsProperty); set => SetValue(ColumnsProperty, value); }

        /// <summary>
        /// Number of rows in the console
        /// </summary>
        public ushort Rows { get => GetValue(ColumnsProperty); set => SetValue(RowsProperty, value); }

        private void LoadXaml()
        {
            // string xamlPath = @"S:\github\Consolonia\src\Consolonia.Gallery\Gallery\GalleryViews\GalleryTextBlock.axaml";
            string xamlPath;
            if (!Path.IsPathFullyQualified(FileName))
            {
                var startFolder = Path.GetDirectoryName(Path.GetFullPath(FileName));
                // walk up parent folder until we find a .csproj file, then look for the file in that folder or below
                xamlPath = FindFile(startFolder, FileName);
            }
            else
                xamlPath = FileName;

            string xaml = File.ReadAllText(xamlPath);
            var (designWidth, designHeight) = GetDesignWidthAndHeight(xaml);

            _psuedoConsole = new WindowsPseudoConsole()
            {
                FilterControlSequences = true,
                FileName = @"s:\github\Consolonia\src\Consolonia.Previewer\bin\Debug\net8.0\Consolonia.Previewer.exe",
                WorkingDirectory = Path.GetDirectoryName(xamlPath),
                Arguments = $"{xamlPath} --buffer",
            };

            _psuedoConsole.Exited += (sender, exitCode) =>
            {
                // restart the process
                _process = _psuedoConsole.Start((short)designWidth, (short)designHeight);

                StartBufferListener(xamlPath);
            };

            _process = _psuedoConsole.Start((short)designWidth, (short)designHeight);

            StartBufferListener(xamlPath);
        }

        private void StartBufferListener(string xamlPath)
        {
            Task.Run(async () =>
            {
                var pipeName = Path.GetFileName(xamlPath);
                using var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In);

                Debug.WriteLine($"Listening on {pipeName}");

                await pipeServer.WaitForConnectionAsync();
                Debug.WriteLine("Client connected");

                // Read user input and send that to the client process.
                using (StreamReader reader = new StreamReader(pipeServer))
                {
                    // Send a 'sync message' and wait for client to receive it.
                    while (true)
                    {
                        var line = await reader.ReadLineAsync();
                        Debug.WriteLine("BUFFER RECEIVED");
                        if (!string.IsNullOrEmpty(line))
                        {
                            var buffer = JsonConvert.DeserializeObject<PixelBuffer>(line);
                            Dispatcher.UIThread.Invoke(() => this.Content = RenderPixelBuffer(buffer));
                        }
                        else if (line == null)
                        {
                            // end of stream
                            return;
                        }
                    }
                }
            });
        }

        private Control RenderPixelBuffer(PixelBuffer? buffer)
        {
            ComputeCharWidth();

            StackPanel rows = new StackPanel()
            {
                Orientation = Orientation.Vertical,
            };

            for (ushort y = 0; y < buffer.Height; y++)
            {
                var columns = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    //VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };

                TextBlockComposer composer = new TextBlockComposer(columns, _charWidth);
                for (ushort x = 0; x < buffer.Width;)
                {
                    var pixel = buffer[x, y];

                    composer.WritePixel(new PixelBufferCoordinate(x, y), pixel);
                    var widthAdjust = (pixel.Foreground.Symbol.Width == 0) ? (ushort)1 : (ushort)pixel.Foreground.Symbol.Width;
                    x += widthAdjust;
                }
                composer.Flush();

                rows.Children.Add(columns);
            }

            return rows;
        }

        private string FindFile(string startFolder, string fileName)
        {
            string currentFolder = startFolder;

            while (currentFolder != null)
            {
                // Check if any .csproj file exists in the current directory
                if (Directory.GetFiles(currentFolder, "*.csproj").Length > 0)
                {
                    // Search for the specified file in the current directory and its subdirectories
                    var files = Directory.GetFiles(currentFolder, fileName, SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        return files[0]; // Return the first match
                    }
                }

                // Move up to the parent directory
                currentFolder = Directory.GetParent(currentFolder)?.FullName;
            }

            throw new FileNotFoundException($"The file '{fileName}' was not found in any parent directory containing a .csproj file.");
        }


        private void ComputeCharWidth()
        {
            if (_charWidth == 0 && _charHeight == 0)
            {
                var ts = TextShaper.Current;
                ShapedBuffer shapedMeasure = ts.ShapeText($"X", new TextShaperOptions(_typeface.GlyphTypeface, FontSize));
                var runMeasure = new ShapedTextRun(shapedMeasure, new GenericTextRunProperties(_typeface, FontSize));
                _charWidth = runMeasure.Size.Width;
                _charHeight = runMeasure.Size.Height;
            }
        }


        private static (ushort designWidth, ushort designHeight) GetDesignWidthAndHeight(string xaml)
        {
            ushort designWidth = 80;
            ushort designHeight = 25;
            var iStart = xaml.IndexOf("d:DesignWidth=\"");
            if (iStart > 0)
            {
                iStart += 15;
                var iEnd = xaml.IndexOf("\"", iStart);
                designWidth = ushort.Parse(xaml.Substring(iStart, iEnd - iStart));
                iStart = xaml.IndexOf("d:DesignHeight=\"");
                {
                    iStart += 16;
                    iEnd = xaml.IndexOf("\"", iStart);
                    designHeight = ushort.Parse(xaml.Substring(iStart, iEnd - iStart));
                }
            }
            return (designWidth, designHeight);
        }


        private class TextBlockComposer
        {
            //todo: move class out
            private readonly StackPanel _panel;
            private readonly double _charWidth;
            private readonly StringBuilder _textBuilder;
            private double _textRunCharWidth;
            private Color _lastBackgroundColor;
            private Color _lastForegroundColor;
            private FontStyle _lastStyle = FontStyle.Normal;
            private FontWeight _lastWeight = FontWeight.Normal;
            private TextDecorationLocation? _lastTextDecorations = null;

            public TextBlockComposer(StackPanel panel, double charWidth)
            {
                _panel = panel;
                _charWidth = charWidth;
                _textBuilder = new StringBuilder();
                _textRunCharWidth = 0;
            }

            public ushort TextRunCharWidth => (ushort)_textRunCharWidth;

            public void WritePixel(PixelBufferCoordinate bufferPoint, Pixel pixel)
            {
                if (_textBuilder.Length > 0 &&
                        (pixel.Foreground.Symbol.Width > 1 ||
                        _lastForegroundColor != pixel.Foreground.Color ||
                        _lastBackgroundColor != pixel.Background.Color ||
                        _lastWeight != pixel.Foreground.Weight ||
                        _lastStyle != pixel.Foreground.Style ||
                        _lastTextDecorations != pixel.Foreground.TextDecoration))
                    Flush();

                if (_textBuilder.Length == 0)
                {
                    _lastBackgroundColor = pixel.Background.Color;
                    _lastForegroundColor = pixel.Foreground.Color;
                    _lastStyle = pixel.Foreground.Style;
                    _lastWeight = pixel.Foreground.Weight;
                    _lastTextDecorations = pixel.Foreground.TextDecoration;
                }

                if (pixel.Foreground.Symbol.Width == 0)
                {
                    _textBuilder.Append("X");
                    _textRunCharWidth += 1;
                }
                else
                {
                    _textBuilder.Append(pixel.Foreground.Symbol.Text);
                    _textRunCharWidth += pixel.Foreground.Symbol.Width;

                    if (pixel.Foreground.Symbol.Width > 1)
                        Flush(true);

                }
            }

            public void Flush(bool forceWidth = false)
            {
                if (_textBuilder.Length == 0)
                    return;

                var text = _textBuilder.ToString();

                var textBlock = new TextBlock()
                {
                    Text = text,
                    Foreground = new SolidColorBrush(_lastForegroundColor),
                    Background = new SolidColorBrush(_lastBackgroundColor),
                    FontWeight = _lastWeight,
                    FontStyle = _lastStyle,
                    TextDecorations = _lastTextDecorations switch
                    {
                        TextDecorationLocation.Underline => TextDecorations.Underline,
                        TextDecorationLocation.Strikethrough => TextDecorations.Strikethrough,
                        _ => null,
                    },
                };

                if (forceWidth)
                    textBlock.Width = _charWidth * _textRunCharWidth;

                _panel.Children.Add(textBlock);
                _textBuilder.Clear();
                _textRunCharWidth = 0;
            }
        }
    }
}
