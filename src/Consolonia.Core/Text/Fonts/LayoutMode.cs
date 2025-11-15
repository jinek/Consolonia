using System;

namespace Consolonia.Core.Text.Fonts
{
    /// <summary>
    ///     FIGlet/TLF font layout modes for character smushing and kerning.
    ///     These flags control how adjacent characters are combined when rendering ASCII art fonts.
    /// </summary>
    [Flags]
    public enum LayoutMode
    {
        /// <summary>
        ///     No smushing - characters are rendered at full width with spacing
        /// </summary>
        None = 0,

        /// <summary>
        ///     Equal character smushing - two identical characters can overlap
        /// </summary>
        Equal = 0b00000001,

        /// <summary>
        ///     Underscore smushing - underscores can be replaced by certain characters
        /// </summary>
        Lowline = 0b00000010,

        /// <summary>
        ///     Hierarchy smushing - characters are ranked and lower-ranked can be replaced by higher-ranked
        /// </summary>
        Hierarchy = 0b00000100,

        /// <summary>
        ///     Pair smushing - specific character pairs can be combined (e.g., [ ] becomes |)
        /// </summary>
        Pair = 0b00001000,

        /// <summary>
        ///     Big X smushing - "/" can smush with "\" to form an X
        /// </summary>
        BigX = 0b00010000,

        /// <summary>
        ///     Hardblank smushing - hardblanks can be smushed with certain characters
        /// </summary>
        Hardblank = 0b00100000,

        /// <summary>
        ///     Kerning mode - characters are moved closer together but not overlapped
        /// </summary>
        Kern = 0b01000000,

        /// <summary>
        ///     Smushing enabled - when set, indicates that smushing rules should be applied
        /// </summary>
        Smush = 0b10000000
    }
}