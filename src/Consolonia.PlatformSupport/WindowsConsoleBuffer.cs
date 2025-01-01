#pragma warning disable CA1416 // Validate platform compatibility
using System;
using Vanara.PInvoke;
using static Vanara.PInvoke.Kernel32;

namespace Consolonia.PlatformSupport
{
    /// <summary>
    /// Represents a single Windows console buffer
    /// </summary>
    internal sealed class WindowsConsoleBuffer
    {
        /// <summary>
        /// Internal handle for the created buffer
        /// </summary>
        internal SafeHFILE BufferHandle { get; private set; }

        /// <summary>
        /// Internal constructor. To create a new ConsoleBuffer, call either Create or GetCurrentConsoleScreenBuffer
        /// </summary>
        /// <param name="bufferHandle">The handle of the existing screen buffer</param>
        internal WindowsConsoleBuffer(IntPtr bufferHandle)
            : this(new SafeHFILE(bufferHandle))
        {
        }

        internal WindowsConsoleBuffer(SafeHFILE bufferHandle)
        {
            this.BufferHandle = bufferHandle;
        }

        /// <summary>
        /// Sets the buffer as the currently active buffer
        /// </summary>
        /// <returns>Returns true on success, false on error.</returns>
        internal bool SetAsActiveBuffer()
        {
            lock(this.BufferHandle)
            {
                return SetConsoleActiveScreenBuffer(this.BufferHandle);
            }
        }

        /// <summary>
        /// Creates a new console buffer.
        /// </summary>
        /// <returns>Returns a new ConsoleBuffer on success, throws on error.</returns>
        internal static WindowsConsoleBuffer Create()
        {
            try
            {
                var buffer = CreateConsoleScreenBuffer(ACCESS_MASK.GENERIC_READ | ACCESS_MASK.GENERIC_WRITE,
                    System.IO.FileShare.Read | System.IO.FileShare.Write,
                    null,
                    CONSOLE_TEXTMODE.CONSOLE_TEXTMODE_BUFFER);
                return new WindowsConsoleBuffer(buffer);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException("Creation of the Console Buffer was unsuccessful.", ex);
            }
        }

        /// <summary>
        /// Gets the current console buffer.
        /// </summary>
        /// <returns>Returns a new ConsoleBuffer on success, throws on error.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")] //Is not a property, so this message is suppressed
        internal static WindowsConsoleBuffer GetCurrentConsoleScreenBuffer()
        {
            try
            {
                var buffer = GetStdHandle(StdHandleType.STD_OUTPUT_HANDLE);
                return new WindowsConsoleBuffer(buffer.DangerousGetHandle());
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException("Getting the current buffer was unsuccessful", ex);
            }
        }

        /// <summary>
        /// Draw data to the console buffer.
        /// </summary>
        /// <param name="data">A CHAR_INFO array containing the data. Should be as big as the data to be written.</param>
        /// <param name="x">X coordinate on the output buffer</param>
        /// <param name="y">Y coordinate on the output buffer</param>
        /// <param name="width">Width of the data</param>
        /// <param name="height">Height of the data</param>
        /// <returns>Returns true on success, false on error.</returns>
        internal bool DrawInternal(CHAR_INFO[] data, short x, short y, short width, short height)
        {
            SMALL_RECT rect = new SMALL_RECT() { Left = x, Top = y, Right = (short)(x + width), Bottom = (short)(y + height) };
            return WriteConsoleOutput(this.BufferHandle, data, new COORD() { X = width, Y = height }, new COORD() { X = 0, Y = 0 }, ref rect);
        }

        /// <summary>
        /// Draws text to the console buffer.
        /// </summary>
        /// <param name="text">The text to be drawn</param>
        /// <param name="x">X coordinate on the output buffer</param>
        /// <param name="y">Y coordinate on the output buffer</param>
        /// <param name="attributes">Attributes of the text</param>
        /// <returns>Returns true on success, false on error.</returns>
        internal bool DrawString(
            string text,
            short x,
            short y,
            CHARACTER_ATTRIBUTE attributes)
        {
            lock (this.BufferHandle)
            {
                if (text == null) return false;
                if (text.Length > short.MaxValue) throw new ArgumentOutOfRangeException(nameof(text), "Text must not be longer than short.MaxValue");

                CHAR_INFO[] data = new CHAR_INFO[text.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i].Char = text[i];
                    data[i].Attributes = attributes;
                }

                return DrawInternal(data, x, y, (short)text.Length, 1);
            }
        }

        internal bool Clear()
        {
            lock (this.BufferHandle)
            {
                GetConsoleScreenBufferInfo(this.BufferHandle, out CONSOLE_SCREEN_BUFFER_INFO info);
                var length = info.dwSize.X * info.dwSize.Y;
                CHAR_INFO[] line = new CHAR_INFO[length];
                CHAR_INFO cell = new CHAR_INFO() { Char = ' ', Attributes = 0 };

                for (int i = 0; i < length; i++)
                    line[i] = cell;

                if (!DrawInternal(line, (short)0, (short)0, info.dwSize.X, info.dwSize.Y))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
