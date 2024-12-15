// ReSharper disable All
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;


namespace Terminal.Gui
{
	internal class WindowsConsole {
		public const int STD_INPUT_HANDLE = -10;

		internal IntPtr InputHandle;
		readonly uint originalConsoleMode;

		public WindowsConsole ()
		{
			InputHandle = GetStdHandle (STD_INPUT_HANDLE);
			originalConsoleMode = ConsoleMode;
			var newConsoleMode = originalConsoleMode;
			newConsoleMode |= (uint)(ConsoleModes.EnableMouseInput | ConsoleModes.EnableExtendedFlags);
			newConsoleMode &= ~(uint)ConsoleModes.EnableQuickEditMode;
			newConsoleMode &= ~(uint)ConsoleModes.EnableProcessedInput;
			ConsoleMode = newConsoleMode;
		}
		
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
#pragma warning disable CS0649
			public Coord size;
#pragma warning restore CS0649
		
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


			public override string ToString ()
			{
				return $"Left={Left},Top={Top},Right={Right},Bottom={Bottom}";
			}
		}

		[DllImport ("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetStdHandle (int nStdHandle);

		[DllImport ("kernel32.dll", EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode)]
		public static extern bool ReadConsoleInput (
			IntPtr hConsoleInput,
			IntPtr lpBuffer,
			uint nLength,
			out uint lpNumberOfEventsRead);

	
		[StructLayout (LayoutKind.Sequential)]
		public struct ConsoleCursorInfo {
			public uint dwSize;
			public bool bVisible;
		}

		[DllImport ("kernel32.dll")]
		static extern bool GetConsoleMode (IntPtr hConsoleHandle, out uint lpMode);


		[DllImport ("kernel32.dll")]
		static extern bool SetConsoleMode (IntPtr hConsoleHandle, uint dwMode);

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
    }
}