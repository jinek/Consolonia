using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace Consolonia.PlatformSupport.Clipboard
{
    /// <summary>
    ///     A clipboard implementation for Win32 using PINvoke
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class Win32Clipboard : IClipboard
    {
        public Task<string> GetTextAsync()
        {
            if (!OpenClipboard(IntPtr.Zero))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            try
            {
                if (!IsClipboardFormatAvailable(CF_UNICODETEXT))
                    return Task.FromResult(String.Empty);

                IntPtr handle = GetClipboardData(CF_UNICODETEXT);
                if (handle == IntPtr.Zero)
                    return Task.FromResult(String.Empty);

                IntPtr pointer = GlobalLock(handle);
                if (pointer == IntPtr.Zero)
                    return Task.FromResult(String.Empty);

                try
                {
                    string text = Marshal.PtrToStringUni(pointer);
                    return Task.FromResult(text);
                }
                finally
                {
                    GlobalUnlock(handle);
                }
            }
            finally
            {
                CloseClipboard();
            }
        }

        public Task SetTextAsync(string text)
        {
            if (!OpenClipboard(IntPtr.Zero))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            try
            {
                EmptyClipboard();

                var handle = Marshal.AllocHGlobal((text.Length + 1) * sizeof(char));
                if (handle == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                var pointer = GlobalLock(handle);
                if (pointer == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                try
                {
                    Marshal.Copy(text.ToCharArray(), 0, pointer, text.Length);
                    Marshal.WriteByte(pointer + text.Length * sizeof(char), 0); // NULL termination

                    if (SetClipboardData(CF_UNICODETEXT, handle) == IntPtr.Zero)
                        throw new Win32Exception(Marshal.GetLastWin32Error());

                    handle = IntPtr.Zero; // Ownership transferred to clipboard
                }
                finally
                {
                    if (handle != IntPtr.Zero)
                        GlobalFree(handle);

                    GlobalUnlock(pointer);
                }
            }
            finally
            {
                CloseClipboard();
            }
            return Task.CompletedTask;
        }

        public Task ClearAsync()
        {
            if (!OpenClipboard(IntPtr.Zero))
                throw new InvalidOperationException("Could not open clipboard.");

            try
            {
                EmptyClipboard();
                return Task.CompletedTask;
            }
            finally
            {
                CloseClipboard();
            }
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

#pragma warning disable CA5392 // Use DefaultDllImportSearchPaths attribute for P/Invokes

        // ReSharper disable InconsistentNaming
#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const uint CF_UNICODETEXT = 13;
#pragma warning restore CA1707 // Identifiers should not contain underscores
        // ReSharper enable InconsistentNaming

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        private static extern bool EmptyClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalFree(IntPtr hMem);
#pragma warning restore CA5392 // Use DefaultDllImportSearchPaths attribute for P/Invokes
    }
}