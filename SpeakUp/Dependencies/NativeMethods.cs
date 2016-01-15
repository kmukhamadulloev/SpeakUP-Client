using System;
using System.Runtime.InteropServices;

namespace SpeakUP.Dependencies
{
    internal class NativeMethods
    {
        public const int HWND_BROADCAST = 0xffff;
        public static readonly int WM_SHOWSPEAKUP = RegisterWindowMessage("WM_SHOWSPEAKUP");
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);
    }
}
