using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace Consolonia.PlatformSupport.Clipboard
{
    /// <summary>A clipboard implementation for Linux. This implementation uses the xclip command to access the clipboard.</summary>
    /// <remarks>If xclip is not installed, this implementation will not work.</remarks>
    internal class XClipClipboard : IClipboardBase
    {
        private readonly bool _isSupported;
        private readonly string _xclipPath = string.Empty;

        public XClipClipboard()
        {
            (int exitCode, string result) = ClipboardProcessRunner.Bash("which xclip", waitForOutput: true);

            if (exitCode == 0 && result.FileExists())
            {
                _xclipPath = result;

                _isSupported = true;
            }
            else
            {
                _isSupported = false;
            }
        }


        public Task ClearAsync()
        {
            return Task.CompletedTask;
        }

        public Task FlushAsync()
        {
            return Task.CompletedTask;
        }

        public Task<string[]> GetFormatsAsync()
        {
            return Task.FromResult(new[] { "text", "unicodetext" });
        }

        public async Task<string> GetTextAsync()
        {
            if (!_isSupported) throw new NotSupportedException("xclip is not installed.");

            string tempFileName = Path.GetTempFileName();
            string xclipargs = "-selection clipboard -o";

            try
            {
                (int exitCode, string _) =
                    ClipboardProcessRunner.Bash($"{_xclipPath} {xclipargs} > {tempFileName}", waitForOutput: false);

                if (exitCode == 0) return await File.ReadAllTextAsync(tempFileName);
                else
                    throw new NotSupportedException($"\"{_xclipPath} {xclipargs}\" failed. {exitCode}");
            }
            finally
            {
                File.Delete(tempFileName);
            }
        }

        public Task SetDataObjectAsync(IDataObject data)
        {
            throw new NotImplementedException();
        }

        public Task SetTextAsync(string text)
        {
            if (!_isSupported) throw new NotSupportedException("xclip is not installed.");

            string xclipargs = "-selection clipboard -i";

            (int exitCode, _) = ClipboardProcessRunner.Bash($"{_xclipPath} {xclipargs}", text);
            if (exitCode != 0) throw new NotSupportedException($"\"{_xclipPath} {xclipargs} < {text}\" failed");

            return Task.CompletedTask;
        }

        public Task<IDataObject> TryGetInProcessDataObjectAsync()
        {
            return Task.FromResult<IDataObject>(null);
        }
    }
}