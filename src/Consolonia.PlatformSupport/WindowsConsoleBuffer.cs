#pragma warning disable CA1416 // Validate platform compatibility
using System;
using System.IO;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using Vanara.PInvoke;
using static Vanara.PInvoke.Kernel32;
using FileAccess = System.IO.FileAccess;

namespace Consolonia.PlatformSupport
{
    /// <summary>
    ///     WindowsConsoleBuffer - A class for managing console alternate screen buffers on windows
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class WindowsConsoleBuffer : IDisposable
    {
        private readonly CancellationTokenSource _cts = new();
        private int _bufferHeight;
        private int _bufferWidth;

        internal WindowsConsoleBuffer(SafeHFILE bufferHandle, bool autoSize = false)
        {
            Handle = bufferHandle;

            if (autoSize)
                _ = MonitorWindowSizeAsync();
        }


        public SafeHFILE Handle { get; }

        public void Dispose()
        {
            ((IDisposable)_cts).Dispose();
        }

        /// <summary>
        ///     This will fire an event when the buffer is resized.
        /// </summary>
        public event Action Resized;

        /// <summary>
        ///     Sets the buffer as the currently active buffer, redirecting all output to the secondary buffer.
        /// </summary>
        /// <returns>Returns true on success, false on error.</returns>
        public bool SetAsActiveBuffer()
        {
            if (!SetConsoleActiveScreenBuffer(Handle)) return false;

            // change stdout handle to point to the new buffer
            if (!SetStdHandle(StdHandleType.STD_OUTPUT_HANDLE, Handle)) return false;

            // change stdout stream to point to the new buffer
            var handle = new SafeFileHandle(Handle.DangerousGetHandle(), false);
            Console.SetOut(new StreamWriter(new FileStream(handle, FileAccess.Write)) { AutoFlush = true });

            if (!SetConsoleCP(65001) || !SetConsoleOutputCP(65001))
                throw GetLastError().GetException();

            // set console mode
            if (!SetConsoleMode(Handle, CONSOLE_OUTPUT_MODE.DISABLE_NEWLINE_AUTO_RETURN |
                                        CONSOLE_OUTPUT_MODE.ENABLE_LVB_GRID_WORLDWIDE))
                throw GetLastError().GetException();

            _bufferHeight = Console.BufferHeight;
            _bufferWidth = Console.BufferWidth;
            return true;
        }

        /// <summary>
        ///     Creates a new console screen buffer.
        /// </summary>
        /// <param name="autoSizeBuffer">Auto size buffer height to window height so that there are no scroll bars</param>
        /// <returns>Returns a new ConsoleScreenBuffer on success, throws on error.</returns>
        public static WindowsConsoleBuffer Create(bool autoSizeBuffer = true)
        {
            try
            {
                SafeHFILE buffer = CreateConsoleScreenBuffer(ACCESS_MASK.GENERIC_READ | ACCESS_MASK.GENERIC_WRITE,
                    FileShare.Read | FileShare.Write);
                return new WindowsConsoleBuffer(buffer, autoSizeBuffer);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException("Creation of the Console Buffer was unsuccessful.", ex);
            }
        }

        /// <summary>
        ///     Gets the current console screen buffer.
        /// </summary>
        /// <returns>Returns a new ConsoleBuffer on success, throws on error.</returns>
        public static WindowsConsoleBuffer GetCurrent()
        {
            HFILE buffer = GetStdHandle(StdHandleType.STD_OUTPUT_HANDLE);
            return new WindowsConsoleBuffer(new SafeHFILE(buffer.DangerousGetHandle(), false));
        }

        private async Task MonitorWindowSizeAsync()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(300, _cts.Token);

                    // if this window is the output buffer, resize it to match the window size
                    if (GetStdHandle(StdHandleType.STD_OUTPUT_HANDLE) == Handle)
                    {
                        if (Console.BufferHeight > Console.WindowHeight)
                        {
                            try
                            {
                                Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                // this can happen as we are resizing.
                                continue;
                            }

                            Resized?.Invoke();
                        }
                        else if (Console.BufferHeight != _bufferHeight || Console.BufferWidth != _bufferWidth)
                        {
                            Resized?.Invoke();
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

    }
}