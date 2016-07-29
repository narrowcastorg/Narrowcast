using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;

namespace Narrowcast
{
    static class Program
    {

        static Mutex mutexGUID = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // get GUID to lock.
            Assembly assembly = Assembly.GetExecutingAssembly();
            mutexGUID = new Mutex(true, assembly.GetType().GUID.ToString());

            if (mutexGUID.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new NarrowcastFormJoin());
                mutexGUID.ReleaseMutex();
            }

            // send message to active/original instance that we tried to open and exit. (main instance will popup the main dialog)
            else
                NarrowcastNative.PostMessage((IntPtr)NarrowcastNative.HWND_BROADCAST, NarrowcastNative.WM_SHOWNARROWCAST, IntPtr.Zero, IntPtr.Zero);
        
        }
    }
}
