using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consolonia.Controls
{
    public interface IConsoleCapabilities
    {
        /// <summary>
        ///     This is true if console supports composing multiple emojis together (like: 👨‍👩‍👧‍👦).
        /// </summary>
        bool SupportsComplexEmoji { get; }
    }
}