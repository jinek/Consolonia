#nullable enable
using Avalonia;
using Avalonia.Controls;

#if DEBUG
using Consolonia.PreviewHost;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Threading;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Newtonsoft.Json;
#endif

namespace Consolonia.Designer
{
    /// <summary>
    ///     ConsolePreview is a control that can be used to preview the output of a console AXAML file in visual studio.
    /// </summary>
    /// <remarks>
    ///     This depends on Consolonia.PreviewHost application to render the view. It
    ///     launches it as a child process with a --buffer width height arguments to tell it
    ///     to output the PixelBuffer as json. The control then reads the json and renders it as a full pixel avalonia control.
    /// </remarks>
    public class ConsolePreview : UserControl
    {
        public static readonly StyledProperty<string> FileNameProperty =
            AvaloniaProperty.Register<ConsolePreview, string>(nameof(FileName));

        public static readonly StyledProperty<ushort> RowsProperty =
            AvaloniaProperty.Register<ConsolePreview, ushort>(nameof(Rows));

        public static readonly StyledProperty<ushort> ColumnsProperty =
            AvaloniaProperty.Register<ConsolePreview, ushort>(nameof(Columns));

        public static readonly StyledProperty<bool> MonitorChangesProperty =
            AvaloniaProperty.Register<ConsolePreview, bool>(nameof(MonitorChanges));

        private bool _disposedValue;

        public ConsolePreview()
        {
#if DEBUG
            _process = null;
            FontFamily = FontFamily.Parse("Cascadia Mono");
            Initialized += (_, _) => LoadXaml();

            PropertyChanged += (_, e) =>
            {
                if (e.Property == FileNameProperty) LoadXaml();
            };
#endif
        }


        /// <summary>
        ///     name of the axaml file to preview.
        /// </summary>
        public string FileName
        {
            get => GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }

        /// <summary>
        ///     Number of columns in the console
        /// </summary>
        public ushort Columns
        {
            get => GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        /// <summary>
        ///     Number of rows in the console
        /// </summary>
        public ushort Rows
        {
            get => GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        /// <summary>
        ///     If set to true then this control will monitor the file for changes and update the preview
        /// </summary>
        public bool MonitorChanges
        {
            get => GetValue(MonitorChangesProperty);
            set => SetValue(MonitorChangesProperty, value);
        }


        protected void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
#if DEBUG
#pragma warning disable CA1416 // Validate platform compatibility
                    if (_process != null)
                    {
                        _process.Kill();
                        _process.Dispose();
                        _process = null;
                    }
#pragma warning restore CA1416 // Validate platform compatibility
#endif
                }

                _disposedValue = true;
            }
        }


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
        }

#if DEBUG
        private Process? _process;
        private readonly Typeface _typeface = new("Cascadia Mono");
        private double _charWidth;
        private double _charHeight;
#endif

#if DEBUG

        private void LoadXaml()
        {
            string xamlPath;
            if (string.IsNullOrEmpty(FileName))
            {
                Content = new StackPanel();
                return;
            }

            if (!Path.IsPathFullyQualified(FileName))
            {
                string startFolder = Path.GetDirectoryName(Path.GetFullPath(FileName))!;
                // walk up parent folder until we find a .csproj file, then look for the file in that folder or below
                xamlPath = FindFile(startFolder, FileName);
            }
            else
            {
                xamlPath = FileName;
            }


            string? xaml;
#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                xaml = File.ReadAllText(xamlPath);
                ArgumentNullException.ThrowIfNull(xaml);
            }
            catch (UnauthorizedAccessException ex)
            {
                Content = new TextBlock { Text = $"Unable to access XAML file. {ex.Message}", Foreground = Brushes.Red };
                return;
            }
            catch (IOException ex)
            {
                Content = new TextBlock { Text = $"Unable to load XAML file. {ex.Message}", Foreground = Brushes.Red };
                return;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            ComputeCharWidth();
            (ushort designWidth, ushort designHeight) = GetDesignWidthAndHeight(xaml);
            ushort cols = Columns;
            ushort rows = Rows;
            if (cols == 0)
                cols = (ushort)(designWidth / (ushort)_charWidth);
            if (rows == 0)
                rows = (ushort)(designHeight / (ushort)_charHeight);

            if (_process == null)
            {
                string previewHostPath =
                    typeof(App).Assembly.Location.Replace(".dll", ".exe", StringComparison.OrdinalIgnoreCase);
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = previewHostPath,
                    Arguments = $"{xamlPath} --buffer {cols} {rows}",
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardInputEncoding = Encoding.UTF8,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = false,
                    CreateNoWindow = true
                };
                _process = Process.Start(processStartInfo);

                if (Design.IsDesignMode)
                {
                    string? line = _process!.StandardOutput.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        Debug.WriteLine("BUFFER RECEIVED");
                        var buffer = JsonConvert.DeserializeObject<PixelBuffer>(line)!;
                        Dispatcher.UIThread.Invoke(() => Content = RenderPixelBuffer(buffer));
                    }

                    _process.Kill();
                    _process.Dispose();
                    _process = null;
                    return;
                }

                ListenForChanges();
            }
            else
            {
                // send request to previewHost to load the new xaml file.
                _process.StandardInput.WriteLine(xamlPath);
            }
        }

        private void ListenForChanges()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    if (_process == null || _process.HasExited)
                        return;

                    string? line = await _process!.StandardOutput.ReadLineAsync().ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(line))
                    {
                        Debug.WriteLine("BUFFER RECEIVED");
#pragma warning disable CA1031 // Do not catch general exception types
                        try
                        {
                            var buffer = JsonConvert.DeserializeObject<PixelBuffer>(line)!;
                            Dispatcher.UIThread.Invoke(() => Content = RenderPixelBuffer(buffer));
                        }
                        catch (JsonException ex)
                        {
                            // process was probably shut down, we continue to check the proces.
                            Debug.WriteLine($"Error deserializing pixel buffer: {ex.Message}");
                            if (_process != null)
                            {
                                if (!_process.HasExited) _process.Kill();
                                _process.Dispose();
                                _process = null;
                                return;
                            }
                        }
#pragma warning restore CA1031 // Do not catch general exception types
                    }
                }
            });
        }

        private Control RenderPixelBuffer(PixelBuffer buffer)
        {
            var lines = new StackPanel
            {
                // TODO This should be theme Background brush
                Background = new SolidColorBrush(Colors.Black),
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top
            };

            for (ushort y = 0; y < buffer.Height; y++)
            {
                var line = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                var composer = new TextBlockComposer(line, _charWidth);
                for (ushort x = 0; x < buffer.Width;)
                {
                    Pixel pixel = buffer[x, y];

                    composer.WritePixel(pixel);
                    ushort widthAdjust = pixel.Foreground.Symbol.Width == 0 ? (ushort)1 : pixel.Foreground.Symbol.Width;
                    x += widthAdjust;
                }

                composer.Flush();

                lines.Children.Add(line);
            }

            return lines;
        }

        private static string FindFile(string startFolder, string fileName)
        {
            string? currentFolder = startFolder;

            while (currentFolder != null)
            {
                // Check if any .csproj file exists in the current directory
                if (Directory.GetFiles(currentFolder, "*.csproj").Length > 0 ||
                    Directory.GetFiles(currentFolder, "*.sln").Length > 0)
                {
                    // Search for the specified file in the current directory and its subdirectories
                    string[] files = Directory.GetFiles(currentFolder, fileName, SearchOption.AllDirectories);
                    if (files.Length > 0) return files[0]; // Return the first match
                }

                // Move up to the parent directory
                currentFolder = Directory.GetParent(currentFolder)?.FullName;
            }

            throw new FileNotFoundException(
                $"The file '{fileName}' in start folder: {startFolder} was not found in any parent directory containing a .csproj file.");
        }


        private void ComputeCharWidth()
        {
            if (_charWidth == 0 && _charHeight == 0)
            {
                TextShaper ts = TextShaper.Current;
                ShapedBuffer shapedMeasure = ts.ShapeText("\u258c", new TextShaperOptions(_typeface.GlyphTypeface, 16));
                var runMeasure = new ShapedTextRun(shapedMeasure, new GenericTextRunProperties(_typeface, 16));
                _charWidth = Math.Ceiling(runMeasure.Size.Width);
                _charHeight = Math.Ceiling(runMeasure.Size.Height);
            }

            if (_charWidth == 0)
                _charWidth = 10;
            if (_charHeight == 0)
                _charHeight = 20;
        }


        private static (ushort designWidth, ushort designHeight) GetDesignWidthAndHeight(string xaml)
        {
            ushort designWidth = 80;
            ushort designHeight = 25;
            int iStart = xaml.IndexOf("d:DesignWidth=\"", StringComparison.Ordinal);
            if (iStart > 0)
            {
                iStart += 15;
                int iEnd = xaml.IndexOf("\"", iStart, StringComparison.Ordinal);
                string num = xaml.Substring(iStart, iEnd - iStart);
                _ = ushort.TryParse(num, out designWidth);
                iStart = xaml.IndexOf("d:DesignHeight=\"", StringComparison.Ordinal);
                {
                    iStart += 16;
                    iEnd = xaml.IndexOf("\"", iStart, StringComparison.Ordinal);
                    num = xaml.Substring(iStart, iEnd - iStart);
                    _ = ushort.TryParse(num, out designHeight);
                }
            }

            return (designWidth, designHeight);
        }


        private class TextBlockComposer
        {
            private readonly double _charWidth;

            //todo: move class out
            private readonly StackPanel _panel;
            private readonly StringBuilder _textBuilder;
            private Color _lastBackgroundColor;
            private Color _lastForegroundColor;
            private FontStyle _lastStyle = FontStyle.Normal;
            private TextDecorationLocation? _lastTextDecorations;
            private FontWeight _lastWeight = FontWeight.Normal;
            private double _textRunCharWidth;

            public TextBlockComposer(StackPanel panel, double charWidth)
            {
                _panel = panel;
                _charWidth = charWidth;
                _textBuilder = new StringBuilder();
                _textRunCharWidth = 0;
            }

            public void WritePixel(Pixel pixel)
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
                    _textBuilder.Append('X');
                    _textRunCharWidth += 1;
                }
                else
                {
                    _textBuilder.Append(pixel.Foreground.Symbol.Text);
                    _textRunCharWidth += pixel.Foreground.Symbol.Width;

                    if (pixel.Foreground.Symbol.Width > 1)
                        Flush();
                }
            }

            public void Flush()
            {
                if (_textBuilder.Length == 0)
                    return;

                string text = _textBuilder.ToString();

                var textBlock = new TextBlock
                {
                    Text = text,
                    Foreground = new SolidColorBrush(_lastForegroundColor),
                    Background = new SolidColorBrush(_lastBackgroundColor),
                    FontWeight = _lastWeight,
                    FontStyle = _lastStyle,
                    FontSize = 17,
                    TextDecorations = _lastTextDecorations switch
                    {
                        TextDecorationLocation.Underline => TextDecorations.Underline,
                        TextDecorationLocation.Strikethrough => TextDecorations.Strikethrough,
                        _ => null
                    }
                };

                textBlock.Width = _charWidth * _textRunCharWidth;

                _panel.Children.Add(textBlock);
                _textBuilder.Clear();
                _textRunCharWidth = 0;
            }
        }
#endif
    }
}