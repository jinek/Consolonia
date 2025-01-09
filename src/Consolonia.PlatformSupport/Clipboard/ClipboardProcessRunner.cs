using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consolonia.PlatformSupport.Clipboard
{
    /// <summary>
    ///     Helper class for console drivers to invoke shell commands to interact with the clipboard. 
    /// </summary>
    internal static class ClipboardProcessRunner
    {
        public static (int exitCode, string result) Bash(
            string commandLine,
            string inputText = "",
            bool waitForOutput = false
        )
        {
            var arguments = $"-c \"{commandLine}\"";
            (int exitCode, string result) = Process("bash", arguments, inputText, waitForOutput);

            return (exitCode, result.TrimEnd());
        }

        public static bool DoubleWaitForExit(this Process process)
        {
            bool result = process.WaitForExit(500);

            if (result)
            {
                process.WaitForExit();
            }

            return result;
        }

        public static bool FileExists(this string value) { return !string.IsNullOrEmpty(value) && !value.Contains("not found", StringComparison.Ordinal); }

        public static (int exitCode, string result) Process(
            string cmd,
            string arguments,
            string input = null,
            bool waitForOutput = true
        )
        {
            var output = string.Empty;

            using (var process = new Process
            {
                StartInfo = new()
                {
                    FileName = cmd,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            })
            {
                TaskCompletionSource<bool> eventHandled = new();
                process.Start();

                if (!string.IsNullOrEmpty(input))
                {
                    process.StandardInput.Write(input);
                    process.StandardInput.Close();
                }

                if (!process.WaitForExit(5000))
                {
                    var timeoutError =
                        $@"Process timed out. Command line: {process.StartInfo.FileName} {process.StartInfo.Arguments}.";

                    throw new TimeoutException(timeoutError);
                }

                if (waitForOutput && process.StandardOutput.Peek() != -1)
                {
                    output = process.StandardOutput.ReadToEnd();
                }

                if (process.ExitCode > 0)
                {
                    output = $@"Process failed to run. Command line: {cmd} {arguments}.
										Output: {output}
										Error: {process.StandardError.ReadToEnd()}";
                }

                return (process.ExitCode, output);
            }
        }
    }
}