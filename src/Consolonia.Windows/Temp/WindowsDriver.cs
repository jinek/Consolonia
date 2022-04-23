using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
// ReSharper disable ArrangeTypeMemberModifiers

namespace Consolonia.Windows
{
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
    [SuppressMessage("Security", "CA5392:Use DefaultDllImportSearchPaths attribute for P/Invokes")]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    internal class WindowsConsole {
		public const int STD_OUTPUT_HANDLE = -11;
		public const int STD_INPUT_HANDLE = -10;
		public const int STD_ERROR_HANDLE = -12;

		internal IntPtr InputHandle, OutputHandle;
		IntPtr ScreenBuffer;
		readonly uint originalConsoleMode;
		CursorVisibility? initialCursorVisibility ;
		CursorVisibility? currentCursorVisibility ;
		CursorVisibility? pendingCursorVisibility ;

		public WindowsConsole ()
		{
			InputHandle = GetStdHandle (STD_INPUT_HANDLE);
			OutputHandle = GetStdHandle (STD_OUTPUT_HANDLE);
			originalConsoleMode = ConsoleMode;
			var newConsoleMode = originalConsoleMode;
			newConsoleMode |= (uint)(ConsoleModes.EnableMouseInput | ConsoleModes.EnableExtendedFlags);
			newConsoleMode &= ~(uint)ConsoleModes.EnableQuickEditMode;
			newConsoleMode &= ~(uint)ConsoleModes.EnableProcessedInput;
			ConsoleMode = newConsoleMode;
		}

		public CharInfo [] OriginalStdOutChars;

		public bool WriteToConsole (Size size, CharInfo [] charInfoBuffer, Coord coords, SmallRect window)
		{
			if (ScreenBuffer == IntPtr.Zero) {
				ReadFromConsoleOutput (size, coords, ref window);
			}

			return WriteConsoleOutput (ScreenBuffer, charInfoBuffer, coords, new Coord () { X = window.Left, Y = window.Top }, ref window);
		}

		public void ReadFromConsoleOutput (Size size, Coord coords, ref SmallRect window)
		{
			ScreenBuffer = CreateConsoleScreenBuffer (
				DesiredAccess.GenericRead | DesiredAccess.GenericWrite,
				ShareMode.FileShareRead | ShareMode.FileShareWrite,
				IntPtr.Zero,
				1,
				IntPtr.Zero
			);
			if (ScreenBuffer == INVALID_HANDLE_VALUE) {
				var err = Marshal.GetLastWin32Error ();

				if (err != 0)
					throw new System.ComponentModel.Win32Exception (err);
			}

			if (!initialCursorVisibility.HasValue && GetCursorVisibility (out CursorVisibility visibility)) {
				initialCursorVisibility = visibility;
			}

			if (!SetConsoleActiveScreenBuffer (ScreenBuffer)) {
				throw new System.ComponentModel.Win32Exception (Marshal.GetLastWin32Error ());
			}

			OriginalStdOutChars = new CharInfo [size.Height * size.Width];

			if (!ReadConsoleOutput (ScreenBuffer, OriginalStdOutChars, coords, new Coord () { X = 0, Y = 0 }, ref window)) {
				throw new System.ComponentModel.Win32Exception (Marshal.GetLastWin32Error ());
			}
		}

		public bool SetCursorPosition (Coord position)
		{
			return SetConsoleCursorPosition (ScreenBuffer, position);
		}

		public void SetInitialCursorVisibility ()
		{
			if (initialCursorVisibility.HasValue == false && GetCursorVisibility (out CursorVisibility visibility)) {
				initialCursorVisibility = visibility;
			}
		}

		public bool GetCursorVisibility (out CursorVisibility visibility)
		{
			if (!GetConsoleCursorInfo (ScreenBuffer, out ConsoleCursorInfo info)) {
				var err = Marshal.GetLastWin32Error ();
				if (err != 0) {
					throw new System.ComponentModel.Win32Exception (err);
				}
				visibility = CursorVisibility.Default;

				return false;
			}

			if (!info.bVisible)
				visibility = CursorVisibility.Invisible;
			else if (info.dwSize > 50)
				visibility = CursorVisibility.Box;
			else
				visibility = CursorVisibility.Underline;

			return true;
		}

		public bool EnsureCursorVisibility ()
		{
			if (initialCursorVisibility.HasValue && pendingCursorVisibility.HasValue && SetCursorVisibility (pendingCursorVisibility.Value)) {
				pendingCursorVisibility = null;

				return true;
			}

			return false;
		}

		public void ForceRefreshCursorVisibility ()
		{
			if (currentCursorVisibility.HasValue) {
				pendingCursorVisibility = currentCursorVisibility;
				currentCursorVisibility = null;
			}
		}

		public bool SetCursorVisibility (CursorVisibility visibility)
		{
			if (initialCursorVisibility.HasValue == false) {
				pendingCursorVisibility = visibility;

				return false;
			}

			if (currentCursorVisibility.HasValue == false || currentCursorVisibility.Value != visibility) {
				ConsoleCursorInfo info = new ConsoleCursorInfo {
					dwSize = (uint)visibility & 0x00FF,
					bVisible = ((uint)visibility & 0xFF00) != 0
				};

				if (!SetConsoleCursorInfo (ScreenBuffer, ref info))
					return false;

				currentCursorVisibility = visibility;
			}

			return true;
		}

		public void Cleanup ()
		{
			if (initialCursorVisibility.HasValue) {
				SetCursorVisibility (initialCursorVisibility.Value);
			}

			SetConsoleOutputWindow (out _);

			ConsoleMode = originalConsoleMode;
			//ContinueListeningForConsoleEvents = false;
			if (!SetConsoleActiveScreenBuffer (OutputHandle)) {
				var err = Marshal.GetLastWin32Error ();
				Console.WriteLine ("Error: {0}", err);
			}

			if (ScreenBuffer != IntPtr.Zero)
				CloseHandle (ScreenBuffer);

			ScreenBuffer = IntPtr.Zero;
		}

		internal Size GetConsoleBufferWindow (out Point position)
		{
			if (ScreenBuffer == IntPtr.Zero) {
				position = Point.Empty;
				return Size.Empty;
			}

			var csbi = new CONSOLE_SCREEN_BUFFER_INFOEX ();
			csbi.cbSize = (uint)Marshal.SizeOf (csbi);
			if (!GetConsoleScreenBufferInfoEx (ScreenBuffer, ref csbi)) {
				//throw new System.ComponentModel.Win32Exception (Marshal.GetLastWin32Error ());
				position = Point.Empty;
				return Size.Empty;
			}
			var sz = new Size (csbi.srWindow.Right - csbi.srWindow.Left + 1,
				csbi.srWindow.Bottom - csbi.srWindow.Top + 1);
			position = new Point (csbi.srWindow.Left, csbi.srWindow.Top);

			return sz;
		}

		internal Size GetConsoleOutputWindow (out Point position)
		{
			var csbi = new CONSOLE_SCREEN_BUFFER_INFOEX ();
			csbi.cbSize = (uint)Marshal.SizeOf (csbi);
			if (!GetConsoleScreenBufferInfoEx (OutputHandle, ref csbi)) {
				throw new System.ComponentModel.Win32Exception (Marshal.GetLastWin32Error ());
			}
			var sz = new Size (csbi.srWindow.Right - csbi.srWindow.Left + 1,
				csbi.srWindow.Bottom - csbi.srWindow.Top + 1);
			position = new Point (csbi.srWindow.Left, csbi.srWindow.Top);

			return sz;
		}

		internal Size SetConsoleWindow (short cols, short rows)
		{
			var csbi = new CONSOLE_SCREEN_BUFFER_INFOEX ();
			csbi.cbSize = (uint)Marshal.SizeOf (csbi);
			if (!GetConsoleScreenBufferInfoEx (ScreenBuffer, ref csbi)) {
				throw new System.ComponentModel.Win32Exception (Marshal.GetLastWin32Error ());
			}
			var maxWinSize = GetLargestConsoleWindowSize (ScreenBuffer);
			var newCols = Math.Min (cols, maxWinSize.X);
			var newRows = Math.Min (rows, maxWinSize.Y);
			csbi.dwSize = new Coord (newCols, Math.Max (newRows, (short)1));
			csbi.srWindow = new SmallRect (0, 0, newCols, newRows);
			csbi.dwMaximumWindowSize = new Coord (newCols, newRows);
			if (!SetConsoleScreenBufferInfoEx (ScreenBuffer, ref csbi)) {
				throw new System.ComponentModel.Win32Exception (Marshal.GetLastWin32Error ());
			}
			var winRect = new SmallRect (0, 0, (short)(newCols - 1), (short)Math.Max (newRows - 1, 0));
			if (!SetConsoleWindowInfo (ScreenBuffer, true, ref winRect)) {
				//throw new System.ComponentModel.Win32Exception (Marshal.GetLastWin32Error ());
				return new Size (cols, rows);
			}
			SetConsoleOutputWindow (csbi);
			return new Size (winRect.Right + 1, newRows - 1 < 0 ? 0 : winRect.Bottom + 1);
		}

		void SetConsoleOutputWindow (CONSOLE_SCREEN_BUFFER_INFOEX csbi)
		{
			if (ScreenBuffer != IntPtr.Zero && !SetConsoleScreenBufferInfoEx (OutputHandle, ref csbi)) {
				throw new System.ComponentModel.Win32Exception (Marshal.GetLastWin32Error ());
			}
		}

		internal Size SetConsoleOutputWindow (out Point position)
		{
			if (ScreenBuffer == IntPtr.Zero) {
				position = Point.Empty;
				return Size.Empty;
			}

			var csbi = new CONSOLE_SCREEN_BUFFER_INFOEX ();
			csbi.cbSize = (uint)Marshal.SizeOf (csbi);
			if (!GetConsoleScreenBufferInfoEx (ScreenBuffer, ref csbi)) {
				throw new System.ComponentModel.Win32Exception (Marshal.GetLastWin32Error ());
			}
			var sz = new Size (csbi.srWindow.Right - csbi.srWindow.Left + 1,
				Math.Max (csbi.srWindow.Bottom - csbi.srWindow.Top + 1, 0));
			position = new Point (csbi.srWindow.Left, csbi.srWindow.Top);
			SetConsoleOutputWindow (csbi);
			var winRect = new SmallRect (0, 0, (short)(sz.Width - 1), (short)Math.Max (sz.Height - 1, 0));
			if (!SetConsoleWindowInfo (OutputHandle, true, ref winRect)) {
				throw new System.ComponentModel.Win32Exception (Marshal.GetLastWin32Error ());
			}

			return sz;
		}

		//bool ContinueListeningForConsoleEvents = true;

		public uint ConsoleMode {
			get {
				GetConsoleMode (InputHandle, out uint v);
				return v;
			}
			set {
				SetConsoleMode (InputHandle, value);
			}
		}

		[Flags]
		public enum ConsoleModes : uint {
			EnableProcessedInput = 1,
			EnableMouseInput = 16,
			EnableQuickEditMode = 64,
			EnableExtendedFlags = 128,
		}

		[StructLayout (LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct KeyEventRecord {
			[FieldOffset (0), MarshalAs (UnmanagedType.Bool)]
			public bool bKeyDown;
			[FieldOffset (4), MarshalAs (UnmanagedType.U2)]
			public ushort wRepeatCount;
			[FieldOffset (6), MarshalAs (UnmanagedType.U2)]
			public ushort wVirtualKeyCode;
			[FieldOffset (8), MarshalAs (UnmanagedType.U2)]
			public ushort wVirtualScanCode;
			[FieldOffset (10)]
			public char UnicodeChar;
			[FieldOffset (12), MarshalAs (UnmanagedType.U4)]
			public ControlKeyState dwControlKeyState;
		}

		[Flags]
		public enum ButtonState {
			Button1Pressed = 1,
			Button2Pressed = 4,
			Button3Pressed = 8,
			Button4Pressed = 16,
			RightmostButtonPressed = 2
		}

		[Flags]
		public enum ControlKeyState {
			RightAltPressed = 1,
			LeftAltPressed = 2,
			RightControlPressed = 4,
			LeftControlPressed = 8,
			ShiftPressed = 16,
			NumlockOn = 32,
			ScrolllockOn = 64,
			CapslockOn = 128,
			EnhancedKey = 256
		}

		[Flags]
		public enum EventFlags {
			MouseMoved = 1,
			DoubleClick = 2,
			MouseWheeled = 4,
			MouseHorizontalWheeled = 8
		}

		[StructLayout (LayoutKind.Explicit)]
		public struct MouseEventRecord {
			[FieldOffset (0)]
			public Coord MousePosition;
			[FieldOffset (4)]
			public ButtonState ButtonState;
			[FieldOffset (8)]
			public ControlKeyState ControlKeyState;
			[FieldOffset (12)]
			public EventFlags EventFlags;

			public override string ToString ()
			{
				return $"[Mouse({MousePosition},{ButtonState},{ControlKeyState},{EventFlags}";
			}
		}

		public struct WindowBufferSizeRecord {
			public Coord size;

			public WindowBufferSizeRecord (short x, short y)
			{
				this.size = new Coord (x, y);
			}

			public override string ToString () => $"[WindowBufferSize{size}";
		}

		[StructLayout (LayoutKind.Sequential)]
		public struct MenuEventRecord {
			public uint dwCommandId;
		}

		[StructLayout (LayoutKind.Sequential)]
		public struct FocusEventRecord {
			public uint bSetFocus;
		}

		public enum EventType : ushort {
			Focus = 0x10,
			Key = 0x1,
			Menu = 0x8,
			Mouse = 2,
			WindowBufferSize = 4
		}

		[StructLayout (LayoutKind.Explicit)]
		public struct InputRecord {
			[FieldOffset (0)]
			public EventType EventType;
			[FieldOffset (4)]
			public KeyEventRecord KeyEvent;
			[FieldOffset (4)]
			public MouseEventRecord MouseEvent;
			[FieldOffset (4)]
			public WindowBufferSizeRecord WindowBufferSizeEvent;
			[FieldOffset (4)]
			public MenuEventRecord MenuEvent;
			[FieldOffset (4)]
			public FocusEventRecord FocusEvent;

			public override string ToString ()
			{
				switch (EventType) {
				case EventType.Focus:
					return FocusEvent.ToString ();
				case EventType.Key:
					return KeyEvent.ToString ();
				case EventType.Menu:
					return MenuEvent.ToString ();
				case EventType.Mouse:
					return MouseEvent.ToString ();
				case EventType.WindowBufferSize:
					return WindowBufferSizeEvent.ToString ();
				default:
					return "Unknown event type: " + EventType;
				}
			}
		};

		[Flags]
		enum ShareMode : uint {
			FileShareRead = 1,
			FileShareWrite = 2,
		}

		[Flags]
		enum DesiredAccess : uint {
			GenericRead = 2147483648,
			GenericWrite = 1073741824,
		}

		[StructLayout (LayoutKind.Sequential)]
		public struct ConsoleScreenBufferInfo {
			public Coord dwSize;
			public Coord dwCursorPosition;
			public ushort wAttributes;
			public SmallRect srWindow;
			public Coord dwMaximumWindowSize;
		}

		[StructLayout (LayoutKind.Sequential)]
		public struct Coord {
			public short X;
			public short Y;

			public Coord (short X, short Y)
			{
				this.X = X;
				this.Y = Y;
			}
			public override string ToString () => $"({X},{Y})";
		};

		[StructLayout (LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct CharUnion {
			[FieldOffset (0)] public char UnicodeChar;
			[FieldOffset (0)] public byte AsciiChar;
		}

		[StructLayout (LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct CharInfo {
			[FieldOffset (0)] public CharUnion Char;
			[FieldOffset (2)] public ushort Attributes;
		}

		[StructLayout (LayoutKind.Sequential)]
		public struct SmallRect {
			public short Left;
			public short Top;
			public short Right;
			public short Bottom;

			public SmallRect (short left, short top, short right, short bottom)
			{
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}

			public static void MakeEmpty (ref SmallRect rect)
			{
				rect.Left = -1;
			}

			public static void Update (ref SmallRect rect, short col, short row)
			{
				if (rect.Left == -1) {
					//System.Diagnostics.Debugger.Log (0, "debug", $"damager From Empty {col},{row}\n");
					rect.Left = rect.Right = col;
					rect.Bottom = rect.Top = row;
					return;
				}
				if (col >= rect.Left && col <= rect.Right && row >= rect.Top && row <= rect.Bottom)
					return;
				if (col < rect.Left)
					rect.Left = col;
				if (col > rect.Right)
					rect.Right = col;
				if (row < rect.Top)
					rect.Top = row;
				if (row > rect.Bottom)
					rect.Bottom = row;
				//System.Diagnostics.Debugger.Log (0, "debug", $"Expanding {rect.ToString ()}\n");
			}

			public override string ToString ()
			{
				return $"Left={Left},Top={Top},Right={Right},Bottom={Bottom}";
			}
		}

		[StructLayout (LayoutKind.Sequential)]
		public struct ConsoleKeyInfoEx {
			public ConsoleKeyInfo consoleKeyInfo;
			public bool CapsLock;
			public bool NumLock;

			public ConsoleKeyInfoEx (ConsoleKeyInfo consoleKeyInfo, bool capslock, bool numlock)
			{
				this.consoleKeyInfo = consoleKeyInfo;
				CapsLock = capslock;
				NumLock = numlock;
			}
		}

		[DllImport ("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetStdHandle (int nStdHandle);

		[DllImport ("kernel32.dll", SetLastError = true)]
		static extern bool CloseHandle (IntPtr handle);

		[DllImport ("kernel32.dll", EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode)]
		public static extern bool ReadConsoleInput (
			IntPtr hConsoleInput,
			IntPtr lpBuffer,
			uint nLength,
			out uint lpNumberOfEventsRead);

		[DllImport ("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		static extern bool ReadConsoleOutput (
			IntPtr hConsoleOutput,
			[Out] CharInfo [] lpBuffer,
			Coord dwBufferSize,
			Coord dwBufferCoord,
			ref SmallRect lpReadRegion
		);

		[DllImport ("kernel32.dll", EntryPoint = "WriteConsoleOutput", SetLastError = true, CharSet = CharSet.Unicode)]
		static extern bool WriteConsoleOutput (
			IntPtr hConsoleOutput,
			CharInfo [] lpBuffer,
			Coord dwBufferSize,
			Coord dwBufferCoord,
			ref SmallRect lpWriteRegion
		);

		[DllImport ("kernel32.dll")]
		static extern bool SetConsoleCursorPosition (IntPtr hConsoleOutput, Coord dwCursorPosition);

		[StructLayout (LayoutKind.Sequential)]
		public struct ConsoleCursorInfo {
			public uint dwSize;
			public bool bVisible;
		}

		[DllImport ("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleCursorInfo (IntPtr hConsoleOutput, [In] ref ConsoleCursorInfo lpConsoleCursorInfo);

		[DllImport ("kernel32.dll", SetLastError = true)]
		static extern bool GetConsoleCursorInfo (IntPtr hConsoleOutput, out ConsoleCursorInfo lpConsoleCursorInfo);

		[DllImport ("kernel32.dll")]
		static extern bool GetConsoleMode (IntPtr hConsoleHandle, out uint lpMode);


		[DllImport ("kernel32.dll")]
		static extern bool SetConsoleMode (IntPtr hConsoleHandle, uint dwMode);

		[DllImport ("kernel32.dll", SetLastError = true)]
		static extern IntPtr CreateConsoleScreenBuffer (
			DesiredAccess dwDesiredAccess,
			ShareMode dwShareMode,
			IntPtr secutiryAttributes,
			uint flags,
			IntPtr screenBufferData
		);

		internal static IntPtr INVALID_HANDLE_VALUE = new IntPtr (-1);

		[DllImport ("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleActiveScreenBuffer (IntPtr Handle);

		[DllImport ("kernel32.dll", SetLastError = true)]
		static extern bool GetNumberOfConsoleInputEvents (IntPtr handle, out uint lpcNumberOfEvents);
		public uint InputEventCount {
			get {
				GetNumberOfConsoleInputEvents (InputHandle, out uint v);
				return v;
			}
		}

		public InputRecord [] ReadConsoleInput ()
		{
			const int bufferSize = 1;
			var pRecord = Marshal.AllocHGlobal (Marshal.SizeOf<InputRecord> () * bufferSize);
			try {
				ReadConsoleInput (InputHandle, pRecord, bufferSize,
					out var numberEventsRead);

				return numberEventsRead == 0
					? null
					: new [] { Marshal.PtrToStructure<InputRecord> (pRecord) };
			} catch (Exception) {
				return null;
			} finally {
				Marshal.FreeHGlobal (pRecord);
			}
		}

#if false      // Not needed on the constructor. Perhaps could be used on resizing. To study.
		[DllImport ("kernel32.dll", ExactSpelling = true)]
		static extern IntPtr GetConsoleWindow ();

		[DllImport ("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern bool ShowWindow (IntPtr hWnd, int nCmdShow);

		public const int HIDE = 0;
		public const int MAXIMIZE = 3;
		public const int MINIMIZE = 6;
		public const int RESTORE = 9;

		internal void ShowWindow (int state)
		{
			IntPtr thisConsole = GetConsoleWindow ();
			ShowWindow (thisConsole, state);
		}
#endif
		// See: https://github.com/migueldeicaza/gui.cs/issues/357

		[StructLayout (LayoutKind.Sequential)]
		public struct CONSOLE_SCREEN_BUFFER_INFOEX {
			public uint cbSize;
			public Coord dwSize;
			public Coord dwCursorPosition;
			public ushort wAttributes;
			public SmallRect srWindow;
			public Coord dwMaximumWindowSize;
			public ushort wPopupAttributes;
			public bool bFullscreenSupported;

			[MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
			public COLORREF [] ColorTable;
		}

		[StructLayout (LayoutKind.Explicit, Size = 4)]
		public struct COLORREF {
			public COLORREF (byte r, byte g, byte b)
			{
				Value = 0;
				R = r;
				G = g;
				B = b;
			}

			public COLORREF (uint value)
			{
				R = 0;
				G = 0;
				B = 0;
				Value = value & 0x00FFFFFF;
			}

			[FieldOffset (0)]
			public byte R;
			[FieldOffset (1)]
			public byte G;
			[FieldOffset (2)]
			public byte B;

			[FieldOffset (0)]
			public uint Value;
		}

		[DllImport ("kernel32.dll", SetLastError = true)]
		static extern bool GetConsoleScreenBufferInfoEx (IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFOEX csbi);

		[DllImport ("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleScreenBufferInfoEx (IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFOEX ConsoleScreenBufferInfo);

		[DllImport ("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleWindowInfo (
			IntPtr hConsoleOutput,
			bool bAbsolute,
			[In] ref SmallRect lpConsoleWindow);

		[DllImport ("kernel32.dll", SetLastError = true)]
		static extern Coord GetLargestConsoleWindowSize (
			IntPtr hConsoleOutput);
	}
}