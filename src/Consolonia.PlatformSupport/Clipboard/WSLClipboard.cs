using System;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace Consolonia.PlatformSupport.Clipboard
{

    /// <summary>
    ///     A clipboard implementation for Linux, when running under WSL. This implementation uses the Windows clipboard
    ///     to store the data, and uses Windows' powershell.exe (launched via WSL interop services) to set/get the Windows
    ///     clipboard.
    /// </summary>
    internal class WSLClipboard : IClipboard
    {
        private static string _powershellPath = string.Empty;

        private bool _isSupported;
        public WSLClipboard()
        {
            if (string.IsNullOrEmpty(_powershellPath))
            {
                // Specify pwsh.exe (not pwsh) to ensure we get the Windows version (invoked via WSL)
                (int exitCode, string result) = ClipboardProcessRunner.Bash("which pwsh.exe", waitForOutput: true);

                if (exitCode > 0)
                {
                    (exitCode, result) = ClipboardProcessRunner.Bash("which powershell.exe", waitForOutput: true);
                }

                if (exitCode == 0)
                {
                    _powershellPath = result;
                }
            }

            _isSupported = !string.IsNullOrEmpty(_powershellPath);
        }

        public async Task ClearAsync()
        {
            await SetTextAsync(string.Empty);
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
                return string.Empty;
            }

            (int exitCode, string output) =
                ClipboardProcessRunner.Process(_powershellPath, "-noprofile -command \"Get-Clipboard\"");

            if (exitCode == 0)
            {
                return output;
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
                return;
            }

            (int exitCode, string output) = ClipboardProcessRunner.Process(
                                                                            _powershellPath,
                                                                            $"-noprofile -command \"Set-Clipboard -Value \\\"{text}\\\"\""
                                                                           );

            if (exitCode != 0)
            {
                throw new InvalidOperationException($"Failed to set clipboard text: {output} using powershell");
            }
        }
    }
}
