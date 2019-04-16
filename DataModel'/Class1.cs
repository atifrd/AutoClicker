using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace DataModel_
{
    public class Class1
    {

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        static extern IntPtr AttachThreadInput(IntPtr idAttach,
                             IntPtr idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        static extern IntPtr GetFocus();



        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            //  labelHandle.Text = "hWnd: " + 
            //                   FocusedControlInActiveWindow().ToString();
        }

        //private IntPtr FocusedControlInActiveWindow()
        //{
        //    IntPtr activeWindowHandle = GetForegroundWindow();

        //    IntPtr activeWindowThread =
        //      GetWindowThreadProcessId(activeWindowHandle, IntPtr.Zero);
        //    IntPtr thisWindowThread = GetWindowThreadProcessId(this.Handle, IntPtr.Zero);

        //    AttachThreadInput(activeWindowThread, thisWindowThread, true);
        //    IntPtr focusedControlHandle = GetFocus();
        //    AttachThreadInput(activeWindowThread, thisWindowThread, false);

        //    return focusedControlHandle;
        //}
    }
}

