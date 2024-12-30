using System;
using Avalonia.Media;
using Consolonia.Core.Drawing;
using Consolonia.Core.Drawing.PixelBufferImplementation;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    /// Defines properties and methods for console output
    /// </summary>
    public interface IConsoleOutput 
    {
        PixelBufferSize Size { get; }

        bool CaretVisible { get; }

        /// <summary>
        ///     This is true if console supports composing multiple emojis together (like: 👨‍👩‍👧‍👦).
        /// </summary>
        bool SupportsComplexEmoji { get; }

        /// <summary>
        /// Does this support
        /// </summary>
        bool SupportsAltSolo { get; }

        /// <summary>
        /// Console has been resized
        /// </summary>
        event Action Resized;

        /// <summary>
        ///  Set the title of the console window
        /// </summary>
        /// <param name="title"></param>
        void SetTitle(string title);

        /// <summary>
        /// Set physical caret position
        /// </summary>
        /// <param name="bufferPoint"></param>
        void SetCaretPosition(PixelBufferCoordinate bufferPoint);

        /// <summary>
        /// Get physical Caret position
        /// </summary>
        /// <returns></returns>
        PixelBufferCoordinate GetCaretPosition();

        /// <summary>
        /// Change caret style
        /// </summary>
        /// <param name="caretStyle"></param>
        void SetCaretStyle(CaretStyle caretStyle);

        /// <summary>
        /// Hide the caret
        /// </summary>
        void HideCaret();

        /// <summary>
        /// Show the caret
        /// </summary>
        void ShowCaret();

        /// <summary>
        /// Prepare the console
        /// </summary>
        void PrepareConsole();

        /// <summary>
        /// Restore the console
        /// </summary>
        void RestoreConsole();

        /// <summary>
        /// Clear the screen
        /// </summary>
        void ClearScreen();

        /// <summary>
        /// Check the size of the console
        /// </summary>
        /// <returns>true if size has changed</returns>
        bool CheckSize();

        /// <summary>
        /// Print formatted text to the console
        /// </summary>
        /// <param name="bufferPoint"></param>
        /// <param name="background"></param>
        /// <param name="foreground"></param>
        /// <param name="style"></param>
        /// <param name="weight"></param>
        /// <param name="textDecoration"></param>
        /// <param name="str"></param>
        void Print(PixelBufferCoordinate bufferPoint, Color background, Color foreground, FontStyle? style,
            FontWeight? weight, TextDecorationLocation? textDecoration, string str);

        /// <summary>
        /// Write raw text to the console
        /// </summary>
        /// <param name="str"></param>
        void WriteText(string str);
    }
}