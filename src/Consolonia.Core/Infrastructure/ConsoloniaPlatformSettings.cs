using Avalonia.Platform;

namespace Consolonia.Core.Infrastructure
{
    public class ConsoloniaPlatformSettings : DefaultPlatformSettings
    {
        //todo: does make sense to move colormode into here?

        /// <summary>
        /// Whether not supported input (like unrecognized keys) should be ignored, or should throw an exception. True by default
        /// <seealso cref="ConsoloniaPlatform.NotSupported"/>
        /// </summary>
        public bool UnsafeInput { get; init; } = true;

        /// <summary>
        /// Whether not supported rendering (like unsupported shapes, colors, etc.) should be ignored, or should throw an exception. True by default
        /// <seealso cref="ConsoloniaPlatform.NotSupported"/>
        /// </summary>
        public bool UnsafeRendering { get; init; } = true;
    }
}