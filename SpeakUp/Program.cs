using System;
using System.Threading;
using System.Windows.Forms;
using SpeakUP.Dependencies;

namespace SpeakUP
{
    static class Program
    {
        static Mutex mutex = new Mutex(true, "{B7148336-4AE0-4D7A-9AE9-B2FCA899360E}");
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
                mutex.ReleaseMutex();
            }
            else
            {
                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SHOWSPEAKUP,
                    IntPtr.Zero,
                    IntPtr.Zero);
            }
        }
    }
}
