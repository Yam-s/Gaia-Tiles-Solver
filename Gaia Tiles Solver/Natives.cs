using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Gaia_Tiles_Solver
{
	public class Natives
	{
		[DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern int AllocConsole();

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

		[DllImport("User32.dll")]
		public static extern bool GetWindowRect(IntPtr handle, ref Rectangle rect);

		[DllImport("User32.dll", SetLastError = true)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
	}
}
