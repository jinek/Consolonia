using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Unix.Terminal;

namespace Consolonia.PlatformSupport.Clipboard
{

    /// <summary>A clipboard implementation for Linux. This implementation uses the xclip command to access the clipboard.</summary>
    /// <remarks>If xclip is not installed, this implementation will not work.</remarks>
    internal class XClipClipboard : IClipboard
    {
        private string _xclipPath = string.Empty;
        private readonly bool _isSupported;

        public XClipClipboard()
        {
            (int exitCode, string result) = ClipboardProcessRunner.Bash("which xclip", waitForOutput: true);

            if (exitCode == 0 && result.FileExists())
            {
                _xclipPath = result;

                _isSupported = true;
            }
            else 
                _isSupported = false;
        }


        public Task ClearAsync()
        {
            throw new NotImplementedException();
        }

        public Task<object> GetDataAsync(string format)
        {
            throw new NotImplementedException();
        }

        public Task<string[]> GetFormatsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetTextAsync()
        {
            if (!_isSupported)
            {
                throw new NotSupportedException("xclip is not installed.");
            }

            string tempFileName = Path.GetTempFileName();
            var xclipargs = "-selection clipboard -o";

            try
            {
                (int exitCode, string result) =
                    ClipboardProcessRunner.Bash($"{_xclipPath} {xclipargs} > {tempFileName}", waitForOutput: false);

                if (exitCode == 0)
                {
                    return await File.ReadAllTextAsync(tempFileName);
                }
            }
            catch (Exception e)
            {
                throw new NotSupportedException($"\"{_xclipPath} {xclipargs}\" failed.", e);
            }
            finally
            {
                File.Delete(tempFileName);
            }

            return string.Empty;
        }

        public Task SetDataObjectAsync(IDataObject data)
        {
            throw new NotImplementedException();
        }

        public async Task SetTextAsync(string text)
        {
            await Task.CompletedTask;
            if (!_isSupported)
            {
                throw new NotSupportedException("xclip is not installed.");
            }

            var xclipargs = "-selection clipboard -i";

            try
            {
                (int exitCode, _) = ClipboardProcessRunner.Bash($"{_xclipPath} {xclipargs}", text);
            }
            catch (Exception e)
            {
                throw new NotSupportedException($"\"{_xclipPath} {xclipargs} < {text}\" failed", e);
            }
        }
    }
}
