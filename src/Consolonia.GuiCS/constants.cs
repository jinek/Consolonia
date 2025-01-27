/*
 * This file is autogenerated by the attrib.c program, do not edit
 */

//#define XTERM1006
// ReSharper disable All
using System;

namespace Unix.Terminal {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public partial class Curses {
		public const int A_NORMAL = unchecked((int)0x0);
		public const int A_STANDOUT = unchecked((int)0x10000);
		public const int A_UNDERLINE = unchecked((int)0x20000);
		public const int A_REVERSE = unchecked((int)0x40000);
		public const int A_BLINK = unchecked((int)0x80000);
		public const int A_DIM = unchecked((int)0x100000);
		public const int A_BOLD = unchecked((int)0x200000);
		public const int A_PROTECT = unchecked((int)0x1000000);
		public const int A_INVIS = unchecked((int)0x800000);
		public const int ACS_LLCORNER = unchecked((int)0x40006d);
		public const int ACS_LRCORNER = unchecked((int)0x40006a);
		public const int ACS_HLINE = unchecked((int)0x400071);
		public const int ACS_ULCORNER = unchecked((int)0x40006c);
		public const int ACS_URCORNER = unchecked((int)0x40006b);
		public const int ACS_VLINE = unchecked((int)0x400078);
		public const int ACS_LTEE = unchecked((int)0x400074);
		public const int ACS_RTEE = unchecked((int)0x400075);
		public const int ACS_BTEE = unchecked((int)0x400076);
		public const int ACS_TTEE = unchecked((int)0x400077);
		public const int ACS_PLUS = unchecked((int)0x40006e);
		public const int ACS_S1 = unchecked((int)0x40006f);
		public const int ACS_S9 = unchecked((int)0x400073);
		public const int ACS_DIAMOND = unchecked((int)0x400060);
		public const int ACS_CKBOARD = unchecked((int)0x400061);
		public const int ACS_DEGREE = unchecked((int)0x400066);
		public const int ACS_PLMINUS = unchecked((int)0x400067);
		public const int ACS_BULLET = unchecked((int)0x40007e);
		public const int ACS_LARROW = unchecked((int)0x40002c);
		public const int ACS_RARROW = unchecked((int)0x40002b);
		public const int ACS_DARROW = unchecked((int)0x40002e);
		public const int ACS_UARROW = unchecked((int)0x40002d);
		public const int ACS_BOARD = unchecked((int)0x400068);
		public const int ACS_LANTERN = unchecked((int)0x400069);
		public const int ACS_BLOCK = unchecked((int)0x400030);
		public const int COLOR_BLACK = unchecked((int)0x0);
		public const int COLOR_RED = unchecked((int)0x1);
		public const int COLOR_GREEN = unchecked((int)0x2);
		public const int COLOR_YELLOW = unchecked((int)0x3);
		public const int COLOR_BLUE = unchecked((int)0x4);
		public const int COLOR_MAGENTA = unchecked((int)0x5);
		public const int COLOR_CYAN = unchecked((int)0x6);
		public const int COLOR_WHITE = unchecked((int)0x7);
		public const int COLOR_GRAY = unchecked((int)0x8);
		public const int KEY_CODE_YES = unchecked((int)0x100);
		public const int KEY_CODE_SEQ = unchecked((int)0x5b);
		public const int ERR = unchecked((int)0xffffffff);
		public const int TIOCGWINSZ  = unchecked((int)0x5413);
		public const int TIOCGWINSZ_MAC  = unchecked((int)0x40087468);

		[Flags]
		public enum Event : long {
			Button1Pressed = unchecked((int)0x2),
			Button1Released = unchecked((int)0x1),
			Button1Clicked = unchecked((int)0x4),
			Button1DoubleClicked = unchecked((int)0x8),
			Button1TripleClicked = unchecked((int)0x10),
			Button2Pressed = unchecked((int)0x40),
			Button2Released = unchecked((int)0x20),
			Button2Clicked = unchecked((int)0x80),
			Button2DoubleClicked = unchecked((int)0x100),
			Button2TripleClicked = unchecked((int)0x200),
			Button3Pressed = unchecked((int)0x800),
			Button3Released = unchecked((int)0x400),
			Button3Clicked = unchecked((int)0x1000),
			Button3DoubleClicked = unchecked((int)0x2000),
			Button3TripleClicked = unchecked((int)0x4000),
			ButtonWheeledUp = unchecked((int)0x10000),
			ButtonWheeledDown = unchecked((int)0x200000),
			Button4Pressed = unchecked((int)0x80000),
			Button4Released = unchecked((int)0x40000),
			Button4Clicked = unchecked((int)0x100000),
			Button4DoubleClicked = unchecked((int)0x20000),
			Button4TripleClicked = unchecked((int)0x400000),
			ButtonShift = unchecked((int)0x4000000),
			ButtonCtrl = unchecked((int)0x2000000),
			ButtonAlt = unchecked((int)0x8000000),
			ReportMousePosition = unchecked((int)0x10000000),
			AllEvents = unchecked((int)0x7ffffff),
		}
#if XTERM1006
		public const int LeftRightUpNPagePPage= unchecked((int)0x8);
		public const int DownEnd = unchecked((int)0x6);
		public const int Home = unchecked((int)0x7);
#else
		public const int LeftRightUpNPagePPage = unchecked((int)0x0);
		public const int DownEnd = unchecked((int)0x0);
		public const int Home = unchecked((int)0x0);
#endif
		public const int KeyBackspace = unchecked((int)0x107);
		public const int KeyUp = unchecked((int)0x103);
		public const int KeyDown = unchecked((int)0x102);
		public const int KeyLeft = unchecked((int)0x104);
		public const int KeyRight = unchecked((int)0x105);
		public const int KeyNPage = unchecked((int)0x152);
		public const int KeyPPage = unchecked((int)0x153);
		public const int KeyHome = unchecked((int)0x106);
		public const int KeyMouse = unchecked((int)0x199);
		public const int KeyEnd = unchecked((int)0x168);
		public const int KeyDeleteChar = unchecked((int)0x14a);
		public const int KeyInsertChar = unchecked((int)0x14b);
		public const int KeyTab = unchecked((int)0x009);
		public const int KeyBackTab = unchecked((int)0x161);
		public const int KeyF1 = unchecked((int)0x109);
		public const int KeyF2 = unchecked((int)0x10a);
		public const int KeyF3 = unchecked((int)0x10b);
		public const int KeyF4 = unchecked((int)0x10c);
		public const int KeyF5 = unchecked((int)0x10d);
		public const int KeyF6 = unchecked((int)0x10e);
		public const int KeyF7 = unchecked((int)0x10f);
		public const int KeyF8 = unchecked((int)0x110);
		public const int KeyF9 = unchecked((int)0x111);
		public const int KeyF10 = unchecked((int)0x112);
		public const int KeyF11 = unchecked((int)0x113);
		public const int KeyF12 = unchecked((int)0x114);
		public const int KeyResize = unchecked((int)0x19a);
		public const int ShiftKeyUp = unchecked((int)0x151);
		public const int ShiftKeyDown = unchecked((int)0x150);
		public const int ShiftKeyLeft = unchecked((int)0x189);
		public const int ShiftKeyRight = unchecked((int)0x192);
		public const int ShiftKeyNPage = unchecked((int)0x18c);
		public const int ShiftKeyPPage = unchecked((int)0x18e);
		public const int ShiftKeyHome = unchecked((int)0x187);
		public const int ShiftKeyEnd = unchecked((int)0x182);
		public const int AltKeyUp = unchecked((int)0x234 + LeftRightUpNPagePPage);
		public const int AltKeyDown = unchecked((int)0x20b + DownEnd);
		public const int AltKeyLeft = unchecked((int)0x21f + LeftRightUpNPagePPage);
		public const int AltKeyRight = unchecked((int)0x22e + LeftRightUpNPagePPage);
		public const int AltKeyNPage = unchecked((int)0x224 + LeftRightUpNPagePPage);
		public const int AltKeyPPage = unchecked((int)0x229 + LeftRightUpNPagePPage);
		public const int AltKeyHome = unchecked((int)0x215 + Home);
		public const int AltKeyEnd = unchecked((int)0x210 + DownEnd);
		public const int CtrlKeyUp = unchecked((int)0x236 + LeftRightUpNPagePPage);
		public const int CtrlKeyDown = unchecked((int)0x20d + DownEnd);
		public const int CtrlKeyLeft = unchecked((int)0x221 + LeftRightUpNPagePPage);
		public const int CtrlKeyRight = unchecked((int)0x230 + LeftRightUpNPagePPage);
		public const int CtrlKeyNPage = unchecked((int)0x226 + LeftRightUpNPagePPage);
		public const int CtrlKeyPPage = unchecked((int)0x22b + LeftRightUpNPagePPage);
		public const int CtrlKeyHome = unchecked((int)0x217 + Home);
		public const int CtrlKeyEnd = unchecked((int)0x212 + DownEnd);
		public const int ShiftCtrlKeyUp = unchecked((int)0x237 + LeftRightUpNPagePPage);
		public const int ShiftCtrlKeyDown = unchecked((int)0x20e + DownEnd);
		public const int ShiftCtrlKeyLeft = unchecked((int)0x222 + LeftRightUpNPagePPage);
		public const int ShiftCtrlKeyRight = unchecked((int)0x231 + LeftRightUpNPagePPage);
		public const int ShiftCtrlKeyNPage = unchecked((int)0x227 + LeftRightUpNPagePPage);
		public const int ShiftCtrlKeyPPage = unchecked((int)0x22c + LeftRightUpNPagePPage);
		public const int ShiftCtrlKeyHome = unchecked((int)0x218 + Home);
		public const int ShiftCtrlKeyEnd = unchecked((int)0x213 + DownEnd);
		public const int ShiftAltKeyUp = unchecked((int)0x235 + LeftRightUpNPagePPage);
		public const int ShiftAltKeyDown = unchecked((int)0x20c + DownEnd);
		public const int ShiftAltKeyLeft = unchecked((int)0x220 + LeftRightUpNPagePPage);
		public const int ShiftAltKeyRight = unchecked((int)0x22f + LeftRightUpNPagePPage);
		public const int ShiftAltKeyNPage = unchecked((int)0x225 + LeftRightUpNPagePPage);
		public const int ShiftAltKeyPPage = unchecked((int)0x22a + LeftRightUpNPagePPage);
		public const int ShiftAltKeyHome = unchecked((int)0x216 + Home);
		public const int ShiftAltKeyEnd = unchecked((int)0x211 + DownEnd);
		public const int AltCtrlKeyNPage = unchecked((int)0x228 + LeftRightUpNPagePPage);
		public const int AltCtrlKeyPPage = unchecked((int)0x22d + LeftRightUpNPagePPage);
		public const int AltCtrlKeyHome = unchecked((int)0x219 + Home);
		public const int AltCtrlKeyEnd = unchecked((int)0x214 + DownEnd);

		// see #949
		static public int LC_ALL { get; private set; }
		static Curses ()
		{
			LC_ALL = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform (System.Runtime.InteropServices.OSPlatform.OSX) ? 0 : 6;
		}

		static public int ColorPair (int n)
		{
			return 0 + n * 256;
		}
	}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
