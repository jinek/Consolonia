using System;
using System.Linq;
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
    internal class WslClipboard : IClipboard
    {
        private static string _powershellPath = string.Empty;

        private readonly bool _isSupported;

        public WslClipboard()
        {
            if (string.IsNullOrEmpty(_powershellPath))
            {
                // Specify pwsh.exe (not pwsh) to ensure we get the Windows version (invoked via WSL)
                (int exitCode, string result) = ClipboardProcessRunner.Bash("which pwsh.exe", waitForOutput: true);

                if (exitCode > 0)
                    (exitCode, result) = ClipboardProcessRunner.Bash("which powershell.exe", waitForOutput: true);

                if (exitCode == 0) _powershellPath = result;
            }

            _isSupported = !string.IsNullOrEmpty(_powershellPath);
        }

        public async Task ClearAsync()
        {
            await SetTextAsync(string.Empty);
        }

        public Task FlushAsync()
        {
            return Task.CompletedTask;
        }

        public Task<object> GetDataAsync(string format)
        {
            throw new NotImplementedException();
        }

        public Task<string[]> GetFormatsAsync()
        {

            // implement
            if (!_isSupported) return Task.FromResult(Array.Empty<string>());
            (int exitCode, string output) =
                ClipboardProcessRunner.Process(_powershellPath, "-noprofile -command \"Get-Clipboard -Format List\"");
            if (exitCode != 0) return Task.FromResult(Array.Empty<string>());

            return Task.FromResult(output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(format =>
                {
                    // Remove the "Format: " prefix
                    if (format.StartsWith("Format: "))
                    {
                        return format[8..];
                    }
                    return format;
                }).ToArray());

        }

        public Task<string> GetTextAsync()
        {
            if (!_isSupported) return Task.FromResult(string.Empty);

            (int exitCode, string output) =
                ClipboardProcessRunner.Process(_powershellPath, "-noprofile -command \"Get-Clipboard\"");

            if (exitCode == 0) return Task.FromResult(output);

            return Task.FromResult(string.Empty);
        }

        public Task SetDataObjectAsync(IDataObject data)
        {
            throw new NotImplementedException();
        }

        public Task SetTextAsync(string text)
        {
            if (_isSupported)
            {
                (int exitCode, string output) = ClipboardProcessRunner.Process(
                    _powershellPath,
                    $"-noprofile -command \"Set-Clipboard -Value \\\"{text}\\\"\""
                );

                if (exitCode != 0)
                    throw new InvalidOperationException($"Failed to set clipboard text: {output} using powershell");
            }

            return Task.CompletedTask;
        }

        public Task<IDataObject> TryGetInProcessDataObjectAsync()
        {
            throw new NotImplementedException();
        }
    }
}