using System;

namespace Consolonia.Core.Text.Fonts
{
    /// <summary>
    ///     Legacy FIGlet layout mode flags (pre-FIGlet 2a).
    ///     This parameter is deprecated and replaced by SmushMode (fullLayout) in FIGlet 2a+.
    ///     Preserved for compatibility and documentation purposes.
    /// </summary>
    [Flags]
    public enum OldLayoutMode
    {
        None = 0,

        /// <summary>
        ///     Horizontal fitting (equal character smushing) enabled
        /// </summary>
        HorizontalFitting = 0b0001,

        /// <summary>
        ///     Horizontal kerning enabled - move characters closer together
        /// </summary>
        HorizontalKerning = 0b0010,

        /// <summary>
        ///     Vertical fitting enabled - smush lines vertically
        /// </summary>
        VerticalFitting = 0b0100,

        /// <summary>
        ///     Vertical kerning enabled - reduce line spacing
        /// </summary>
        VerticalKerning = 0b1000
    }
}