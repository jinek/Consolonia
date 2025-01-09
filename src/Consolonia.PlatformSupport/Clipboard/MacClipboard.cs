using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace Consolonia.PlatformSupport.Clipboard
{
    /// <summary>
    ///     A clipboard implementation for MacOSX. This implementation uses the Mac clipboard API (via P/Invoke) to
    ///     copy/paste. The existence of the Mac pbcopy and pbpaste commands is used to determine if copy/paste is supported.
    /// </summary>
    internal class MacClipboard : IClipboard
    {
        private readonly nint _allocRegister = sel_registerName("alloc");
        private readonly nint _clearContentsRegister = sel_registerName("clearContents");
        private readonly nint _generalPasteboard;
        private readonly nint _generalPasteboardRegister = sel_registerName("generalPasteboard");
        private readonly nint _initWithUtf8Register = sel_registerName("initWithUTF8String:");
        private readonly nint _nsPasteboard = objc_getClass("NSPasteboard");
        private readonly nint _nsString = objc_getClass("NSString");
        private readonly nint _nsStringPboardType;
        private readonly nint _setStringRegister = sel_registerName("setString:forType:");
        private readonly nint _stringForTypeRegister = sel_registerName("stringForType:");
        private readonly nint _utf8Register = sel_registerName("UTF8String");
        private readonly nint _utfTextType;

        public MacClipboard()
        {
            _utfTextType = objc_msgSend(
                objc_msgSend(_nsString, _allocRegister),
                _initWithUtf8Register,
                "public.utf8-plain-text"
            );

            _nsStringPboardType = objc_msgSend(
                objc_msgSend(_nsString, _allocRegister),
                _initWithUtf8Register,
                "NSStringPboardType"
            );
            _generalPasteboard = objc_msgSend(_nsPasteboard, _generalPasteboardRegister);
            if (!CheckSupport())
                throw new NotSupportedException(
                    "clipboard operations are not supported pbcopy and pbpaste are not available on this system.");
        }

        public Task<string> GetTextAsync()
        {
            nint ptr = objc_msgSend(_generalPasteboard, _stringForTypeRegister, _nsStringPboardType);
            nint charArray = objc_msgSend(ptr, _utf8Register);

            return Task.FromResult(Marshal.PtrToStringAnsi(charArray));
        }

        public Task SetTextAsync(string text)
        {
            nint str = default;

            try
            {
                str = objc_msgSend(objc_msgSend(_nsString, _allocRegister), _initWithUtf8Register, text);
                objc_msgSend(_generalPasteboard, _clearContentsRegister);
                objc_msgSend(_generalPasteboard, _setStringRegister, str, _utfTextType);
            }
            finally
            {
                if (str != default) objc_msgSend(str, sel_registerName("release"));
            }

            return Task.CompletedTask;
        }

        public Task ClearAsync()
        {
            return SetTextAsync(string.Empty);
        }

        public Task SetDataObjectAsync(IDataObject data)
        {
            throw new NotImplementedException();
        }

        public Task<string[]> GetFormatsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<object> GetDataAsync(string format)
        {
            throw new NotImplementedException();
        }

        private static bool CheckSupport()
        {
            (int exitCode, string result) = ClipboardProcessRunner.Bash("which pbcopy", waitForOutput: true);

            if (exitCode != 0 || !result.FileExists()) return false;

            (exitCode, result) = ClipboardProcessRunner.Bash("which pbpaste", waitForOutput: true);

            return exitCode == 0 && result.FileExists();
        }

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit", CharSet = CharSet.Unicode)]
        private static extern nint objc_getClass(string className);

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        private static extern nint objc_msgSend(nint receiver, nint selector);

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit", CharSet = CharSet.Unicode)]
        private static extern nint objc_msgSend(nint receiver, nint selector, string arg1);

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        private static extern nint objc_msgSend(nint receiver, nint selector, nint arg1);

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        private static extern nint objc_msgSend(nint receiver, nint selector, nint arg1, nint arg2);

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit", CharSet = CharSet.Unicode)]
        private static extern nint sel_registerName(string selectorName);
    }
}