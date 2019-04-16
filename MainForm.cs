using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Collections;
using WindowsInput;
using System.Diagnostics;
using Clicker.DataSection;
using System.Drawing.Imaging;
using WindowsInput.Native;

namespace Clicker
{  
    //[Flags]
    public enum ClickType
    {
        OpenX2 = 0,
        click = 1,
        rightClick = 2 ,
        DoubleClick = 3,
       // SendKeys = 3,
        Tab=4,
        Enter=5,
        ShifF10=6,
        UpArrow = 7,
        DownArrow = 8,
        leftArrow = 9,
        RightArrow = 10,
        //FinalButton=13,        
        Write_Moghoofeh = 11//Developer
        ,Write_Raghabeh = 12,//Developer
        SearchFolder = 13,
        SearchImgName=14

    }
    public partial class MainForm : Form
    {
        //____________Reserved Code____________________________________
        //private const int BN_CLICKED = 245;
        ////Get a handle for the "1" button
		//hwndChild = FindWindowEx((IntPtr)hwnd,IntPtr.Zero,"Button","1");		
		//send BN_CLICKED message
		//SendMessage((int)hwndChild,BN_CLICKED,0,IntPtr.Zero);
        //
        //SendMessage(hrefHWndTarget, WM_SETTEXT, IntPtr.Zero, strTextToSet);
        //********************************
        //var isCapsLockOn = InputSimulator.IsTogglingKeyInEffect(VirtualKeyCode.CAPITAL);
        //_____________________________________________________________

        #region WinAPI
        //&&&&&&&&&&&&&&&&&&&&&&&
        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        const int MYACTION_HOTKEY_ID = 1;
        //&&&&&&&&&&&&

        private const int WM_CLOSE = 16;//Developer
        public const int BM_CLICK =245;// 0x00F5;//Developer  //245
        public const int WM_SETTEXT = 0x000C;
        const int WM_GETTEXT = 0x000D;
        const int WM_GETTEXTLENGTH = 0x000E;

        [DllImport("user32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int RegisterWindowMessage(string lpString);
        public void RegisterControlforMessages()
        {
            RegisterWindowMessage("WM_GETTEXT");
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);


        [DllImport("kernel32.dll", SetLastError = true)]//Developer
        static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]//Developer
        public static extern IntPtr FindWindow(
        string lpClassName, // class name
        string lpWindowName // window name
        );


        [DllImport("user32.dll")]//Developer
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);


        [DllImport("user32.dll")]//Developer
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]//Developer
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        static extern bool SetWindowText(IntPtr hWnd, string lpString);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);
       

        public delegate bool EnumedWindow(IntPtr handleWindow, ArrayList handles);

         [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumedWindow lpEnumFunc, ArrayList lParam);




         [DllImport("user32.dll", SetLastError = true)]
         static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

         // When you don't want the ProcessId, use this overload and pass IntPtr.Zero for the second parameter
         [DllImport("user32.dll")]
         static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);



         private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
         [DllImport("user32.dll")]
         [return: MarshalAs(UnmanagedType.Bool)]
         static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);


         public static List<IntPtr> GetChildWindows(IntPtr parent)
         {
             List<IntPtr> result = new List<IntPtr>();
             GCHandle listHandle = GCHandle.Alloc(result);
             try
             {
                 EnumWindowsProc childProc = new EnumWindowsProc(EnumWindow);
                 EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
             }
             finally
             {
                 if (listHandle.IsAllocated)
                     listHandle.Free();
             }
             return result;
         }

         private static bool EnumWindow(IntPtr handle, IntPtr pointer)
         {
             GCHandle gch = GCHandle.FromIntPtr(pointer);
             List<IntPtr> list = gch.Target as List<IntPtr>;
             if (list == null)
             {
                 throw new InvalidCastException("GCHandle Target could not be cast as List&lt;IntPtr&gt;");
             }
             list.Add(handle);
             // You can modify this to check to see if you want to cancel the operation, then return a null here
             return true;
         }


        /// <summary>
        /// The SendMessage API
        /// </summary>
        /// <param name="hWnd">handle to the required window</param>
        /// <param name="msg">the system/Custom message to send</param>
        /// <param name="wParam">first message parameter</param>
        /// <param name="lParam">second message parameter</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)] //
        public static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

        /// <summary>
        /// The FindWindowEx API
        /// </summary>
        /// <param name="parentHandle">a handle to the parent window </param>
        /// <param name="childAfter">a handle to the child window to start search after</param>
        /// <param name="className">the class name for the window to search for</param>
        /// <param name="windowTitle">the name of the window to search for</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);
        #endregion
        #region Fields
        private const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        private const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */
        private const int MOUSEEVENTF_RIGHTUP = 0x0010; /* right button up */
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; /* middle button down */
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040; /* middle button up */
        private const int MOUSEEVENTF_XDOWN = 0x0080; /* x button down */
        private const int MOUSEEVENTF_XUP = 0x0100; /* x button down */
        private const int MOUSEEVENTF_WHEEL = 0x0800; /* wheel button rolled */
        private const int MOUSEEVENTF_VIRTUALDESK = 0x4000; /* map to entire virtual desktop */
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000; /* absolute move */

        public const byte VK_TAB = 0x09;
        const byte VK_RETURN = 0x0D;//Developer
        const uint KEYEVENTF_KEYUP = 0x02;//Developer

        public const byte VK_LSHIFT = 0xA0; // left shift key Developer

        const byte VK_F10 = 0x79;//Developer
        const byte F10 = 0x79;//Developer


        private SynchronizationContext context = null;
        private DateTime start, end;
        private bool first = true;
        private List<ActionEntry> actions;
        private Thread runActionThread;
        private bool byTextEntry;
        private bool FrmTitleEntry;
        private Hashtable schedualeList;
        #endregion

        #region Construction
        public MainForm()
        {
            InitializeComponent();
            context = SynchronizationContext.Current;
            actions = new List<ActionEntry>();
            schedualeList = new Hashtable();
            // Modifier keys codes: Alt = 1, Ctrl = 2, Shift = 4, Win = 8
            // Compute the addition of each combination of the keys you want to be pressed
            // ALT+CTRL = 1 + 2 = 3 , CTRL+SHIFT = 2 + 4 = 6...
            RegisterHotKey(this.Handle, MYACTION_HOTKEY_ID, 6, (int)Keys.F12);
        }
        #endregion

        #region Private Methods
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == MYACTION_HOTKEY_ID)
            {
                // My hotkey has been typed

                // Do what you want here
              
                btnCancel.PerformClick();
               this.Focus();
               //MessageBox.Show("فرایند توسط کاربر متوقف شد");

            }
            base.WndProc(ref m);
        }
      public static double CurrentCodeTafzili=0;
      List<int> ClassesIds = null;//به این دلیل که خطای ترد می داد
      string ImgUrl = "Start/";
      Int64 ImgId = 0;
      string PatternName = "NoName";
      public static string Username = "1000012";
      public static string Pass = "531246";
        /// <summary>
        /// احتمالا توی تایمر بذارمش
        /// </summary>
        /// <returns></returns>
      private bool ApplicationIsOpen()
      {
          IntPtr hwnd = FindWindow(null, "اطلاعات  موقوفات،رقبات،نیات");
          if (hwnd != IntPtr.Zero)
              SetForegroundWindow(hwnd);
         // else
             // MessageBox.Show("");           
              return true ? hwnd != IntPtr.Zero : false;;
          
      }
      InputSimulator u = new InputSimulator();
        /// <summary>
        /// شروع کار و باز کردن برنامه و ورود به برنامه
        /// </summary>
      private void StartUpAction()
      { 
          //int hwnd = 0;
          //IntPtr hwndChild = IntPtr.Zero;

          ////Get a handle for the Keyhan Application main window
          //hwnd = (int)FindWindow(null, "Keyhan Client");
          //if (hwnd == 0)
          //{
          //    Process.Start(@"C:\Program Files\PayamPardaz\Keyhan\KClient.exe");
          //    Thread.Sleep(4000);
          //    InputSimulator.SimulateTextEntry("12345");
          //    InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
          //    Thread.Sleep(1000);
          //    InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
          //    Thread.Sleep(1000);
          //    InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);

          //    //hwndChild = FindWindowEx((IntPtr)hwnd, IntPtr.Zero, "Button", null);

          //    // if (hwndChild != IntPtr.Zero)
          //    //   SendMessage((int)hwndChild, BM_CLICK, 0, IntPtr.Zero);


          //    Thread.Sleep(15000);
          //}
          //-------------------------------2x----------------------------------------------
              Process.Start(@"C:\Program Files\2X\Client\APPServerClient.exe");
              Thread.Sleep(10000); u.Keyboard.KeyPress(VirtualKeyCode.TAB);
            //Unicode InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
              Thread.Sleep(1000);
              //InputSimulator.SimulateKeyDown(VirtualKeyCode.SHIFT);
              //InputSimulator.SimulateKeyDown(VirtualKeyCode.F10);
              //Thread.Sleep(100);
              //InputSimulator.SimulateKeyUp(VirtualKeyCode.F10);
              //InputSimulator.SimulateKeyUp(VirtualKeyCode.SHIFT);

              //Thread.Sleep(1000);
              //InputSimulator.SimulateKeyPress(VirtualKeyCode.DOWN);
              //Thread.Sleep(1000);
              //InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
              //Thread.Sleep(10000);
              //Unicode  InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
              u.Keyboard.KeyPress(VirtualKeyCode.RETURN);
              Thread.Sleep(17000);
              //InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
              //Thread.Sleep(1000);
              //InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
              //Thread.Sleep(1000);
              //InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
              //Thread.Sleep(1000);
              //InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
              //Thread.Sleep(1000);
          //-------------------------------------------------------------------------------
              //Unicode InputSimulator.SimulateTextEntry(Username);
              u.Keyboard.TextEntry(Username);
              Thread.Sleep(2000);
              //Unicode InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
              u.Keyboard.KeyPress(VirtualKeyCode.TAB);
              Thread.Sleep(1000);
              //Unicode InputSimulator.SimulateTextEntry(Pass);
              u.Keyboard.TextEntry(Pass);
              Thread.Sleep(2000);
              //Unicode InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
              u.Keyboard.KeyPress(VirtualKeyCode.RETURN);
              Thread.Sleep(5000);

              //Unicode InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
              u.Keyboard.KeyPress(VirtualKeyCode.RETURN);
              Thread.Sleep(5000);

              //Unicode InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
              u.Keyboard.KeyPress(VirtualKeyCode.RETURN);
              Thread.Sleep(5000);


          
         // if (Marshal.GetLastWin32Error() > 0)
           //   MessageBox.Show(Marshal.GetLastWin32Error().ToString());
          //if (hwnd != 0)
            //  SendMessage(hwnd, WM_CLOSE, 0, IntPtr.Zero);
      }
        private void RunAction()//oon amali ke tgread baid anjam bede
        {
           
            //StartUpAction();
           bool repeat_perCode = false;
            bool Repeat_PerImage=false;
            
            bool Repeat_PerRelatedClass = false;
           int i = 0;
                foreach (ActionEntry action in actions)
                {
                    ExecuteAction(action);
                    i++;
                    //if (action.Text != string.Empty && FindWindow(null, action.Text) != IntPtr.Zero)//action.Text=عنوان فرم
                    if (action.Text =="1")//action.Text=عنوان فرم
                    {
                        repeat_perCode = true;
                        break;
                    }
                }
                if(repeat_perCode)//az ghesmate formi ke code tafzili ro baiad search kone
                    foreach (double item in Mylist)
                    {
                        CurrentCodeTafzili = item;//.کد_تفضیلی;
                            int j;
                            int k=0;
                            for (j = i; j <actions.Count; j++)
                            {
                                ExecuteAction(actions[j]);
                                //if (actions[j].Text != string.Empty && FindWindow(null, actions[j].Text) != IntPtr.Zero)
                                //if (actions[j].Text =="2")//فرم رقبه
                                //{ Repeat_PerRelatedClass = true; break; }

                                if (actions[j].Text == "3")//فرم آپلود
                                { Repeat_PerImage = true; break; }
                            }
                            /*if (Repeat_PerRelatedClass)
                            {
                                List<int> ClassesIds = checkedListBox_Classes.Items.OfType<FileClass>().Select(s =>s.ClassID).ToList();
                                 foreach (var images in DataSection.BusseinessMethods.ClassesCountInEachCode(item.کد_تفضیلی,ClassesIds))
                                {
                                    for ( k = j; k <actions.Count ; k++)
                                    {
                                        ExecuteAction(actions[k]);
                                        if (actions[k].Text == "3")//فرم آپلود
                                        { Repeat_PerImage = true; break; }
                                    }
                                }
                            }*/
                            int z=0;
                            if (Repeat_PerImage)
                            {
                                //img haye code ke in cass ha hastan
                                
                                List<FilesINBarCode> ImgList = DataSection.BusseinessMethods.ImagesInEachCodeWithRelatedClass(item, ClassesIds);
                                foreach (var images in ImgList)
                                {
                                     ImgUrl = images.FileURL;
                                    ImgId=images.FileId;
                                    for (z = k == 0 ? j : k; z < actions.Count; z++)
                                    {
                                        if (actions[z].Text == "4")//بستن پنجره
                                        {
                                            //updateBank
                                            DataSection.BusseinessMethods.UpdateStateAfterSuccessfullUpload_eachImg(ImgId);
                                            break;
                                        }
                                        ExecuteAction(actions[z]);
                                    }
                                }
                            }
                            for (int w = z; w < actions.Count; w++)//بستن پنجره
                                if (actions[w].Text == "4")
                                         ExecuteAction(actions[w]);
                    }

                Thread.Sleep(500);//developer
            
                ThreadPool.QueueUserWorkItem(new WaitCallback(WorkEnableButtons), null);//////???????????                        
        }

        private void ExecuteAction(ActionEntry action)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorkClick), action);
            CaptureScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y,
                Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height);

            int tmpIntervl = action.Interval.Equals(0) ? 0 : action.Interval * 1000 - 100;
            Thread.Sleep(tmpIntervl);
        }
        private void WorkSendKeys(object state)
        {
            this.context.Send(new SendOrPostCallback(delegate(object _state)
            {
                ActionEntry action = state as ActionEntry;
                SendKeys.Send(action.Text);
            }), state);
        }
        private void WorkClick(object state)
        {
            this.context.Send(new SendOrPostCallback(delegate(object _state)
            {
                ActionEntry action = state as ActionEntry;
                SetCursorPos(action.X, action.Y);
                Thread.Sleep(100);
                if (action.Type.Equals(ClickType.click))
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }
                else if (action.Type.Equals(ClickType.Enter))
                {
                    u.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                }
                else if (action.Type.Equals(ClickType.DoubleClick))
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    Thread.Sleep(100);
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }
                else if (action.Type.Equals(ClickType.rightClick))
                {
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                }
                else if (action.Type.Equals(ClickType.Write_Moghoofeh))//Developer
                {
                    /* mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                     mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                     Thread.Sleep(100);
                     mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                     mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);*/
                    //check kone age form=="" bood va objecti ke focuse dare=="userTextBox" folan chizo write kone age folan form masalan kode tafzili

                    /* IntPtr mainw= GetForegroundWindow();
                     IntPtr MyTexBox = FindWindowEx(mainw, IntPtr.Zero, "TextBox",null);
                     SendMessage(FindWindowEx(mainw, IntPtr.Zero, "TextBox", string.Empty), WM_SETTEXT, 0, string.Empty);
                     SetWindowText(mainw, "123");
                     Thread.Sleep(100);

                     foreach (Process pList in Process.GetProcesses())
                     {
                         string a = pList.ProcessName;
                         //if (pList.ProcessName)
                         //{
                         //    hWnd = pList.MainWindowHandle;
                         //}
                     }25bahman*/
                    string CurrentCodeTafzili_str = CurrentCodeTafzili.ToString();
                    if (CurrentCodeTafzili_str.StartsWith("2"))
                        CurrentCodeTafzili_str = "1" + CurrentCodeTafzili_str.Substring(1, 7) + "000000";
                    //unicode InputSimulator.SimulateTextEntry(CurrentCodeTafzili_str);//*************write code tafzili moghoofeh in textbox
                    u.Keyboard.TextEntry(CurrentCodeTafzili_str);
                }
                else if (action.Type.Equals(ClickType.Write_Raghabeh))//Developer
                {
                    string CurrentCodeTafzili_str = CurrentCodeTafzili.ToString();
                    if (CurrentCodeTafzili_str.StartsWith("1"))
                    {//خطا
                        if (runActionThread != null && runActionThread.IsAlive)
                        {
                            runActionThread.Abort();
                            enableButtons(true);
                        }
                        MessageBox.Show("کد بازیابی شده کد موقوفه  است درحالیکه درخواست جستحو کد رقبه را دارید");
                        return;
                    }
                    //  CurrentCodeTafzili_str = "1" + CurrentCodeTafzili_str.Substring(1, 7) + "000000";
                    //unicode InputSimulator.SimulateTextEntry(CurrentCodeTafzili.ToString());//*************write code tafzili raghabe in textbox
                    u.Keyboard.TextEntry(CurrentCodeTafzili.ToString());
                }
                ////else if (action.Type.Equals(ClickType.FinalButton))//Developer
                ////{                    
                ////    //bad bank khodemoono update mikone
                ////    //
                ////    DataSection.BusseinessMethods.UpdateStateAfterSuccessfullUpload(CurrentCodeTafzili);
                ////    //panjera ro mibande
                ////    /* IntPtr hwnd = FindWindow(null, "آپلودتصاویر 1");
                ////     if (hwnd != IntPtr.Zero)
                ////         SendMessage(hwnd, WM_CLOSE, 0, string.Empty);25bahman*/
                ////}
                else if (action.Type.Equals(ClickType.SearchFolder))//Developer
                {
                    /* InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
                     Thread.Sleep(1000);
                     InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
                     Thread.Sleep(1000);
                     InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
                     Thread.Sleep(1000);
                     InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
                     Thread.Sleep(1000);
                     InputSimulator.SimulateKeyPress(VirtualKeyCode.SPACE);
                     Thread.Sleep(1000);25bahmab*/


                    // InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);//rooye dokme upload
                    //Thread.Sleep(2000);//fasele ta panjere file brows baz she
                    //ImgUrl
                    try
                    {
                        string x = ImgUrl.Split('/').First(); ImgUrl = ImgUrl.Replace(x + "/", string.Empty);
                        //unicode  InputSimulator.SimulateTextEntry(@"\\tsclient\" + textBox_DefFolder.Text.Replace(":", "") + @"\" + ImgUrl.Remove(ImgUrl.LastIndexOf('/')).Replace("/", @"\"));
                        u.Keyboard.TextEntry(@"\\tsclient\" + textBox_DefFolder.Text.Replace(":", "") + @"\" + ImgUrl.Remove(ImgUrl.LastIndexOf('/')).Replace("/", @"\"));
                        //InputSimulator.SimulateTextEntry(@"\\tsclient\"+textBox_DefFolder.Text+"\\" + CurrentCodeTafzili.ToString());

                        Thread.Sleep(2000);//fasele ta bere too folder
                        //select All
                        /* InputSimulator.SimulateKeyDown(VirtualKeyCode.CONTROL);
                         Thread.Sleep(100);
                         InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_A);
                         Thread.Sleep(100);
                         InputSimulator.SimulateKeyUp(VirtualKeyCode.CONTROL);
                         Thread.Sleep(100);
                         InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_A); 25 bahman*/

                        // Thread.Sleep(2000);//fasele ta select  anjam she
                        //InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
                        //25bahman Thread.Sleep(180);//fasele 180 sanie,3daghighe!!!!!

                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.ToString() + " " + ImgUrl);
                    }
                }
                else if (action.Type.Equals(ClickType.SearchImgName))//Developer
                {

                    try
                    {
                        //unicode InputSimulator.SimulateTextEntry(ImgUrl.Split('/').Last());
                        u.Keyboard.TextEntry(ImgUrl.Split('/').Last());
                        Thread.Sleep(100);
                        //unicode InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);//mire too folder
                        //u.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                        Thread.Sleep(1000);//fasele ta bere too folder

                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.ToString() + " " + ImgUrl);
                    }
                }
                else if (action.Type.Equals(ClickType.Tab))//Developer
                {
                    //  keybd_event(0x09, 0x0f, 0x0002, 0);
                    // keybd_event(0x09, 0x0f, 0, 0); // Tab Press
                    //unocode InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
                    u.Keyboard.KeyPress(VirtualKeyCode.TAB);
                }
                else if (action.Type.Equals(ClickType.ShifF10))//Developer
                {
                    /* InputSimulator.SimulateKeyDown(VirtualKeyCode.SHIFT);
                     InputSimulator.SimulateKeyDown(VirtualKeyCode.F10);
                     Thread.Sleep(100);
                     InputSimulator.SimulateKeyUp(VirtualKeyCode.F10);
                     InputSimulator.SimulateKeyUp(VirtualKeyCode.SHIFT);*/
                    //press the shift key
                    keybd_event(VK_LSHIFT, 0x45, 0, 0);

                    //press the f10 key
                    keybd_event(VK_F10, 0x45, 0, 0);
                    Thread.Sleep(100);

                    keybd_event(VK_F10, 0x45, KEYEVENTF_KEYUP, 0);

                    //release the shift key
                    keybd_event(VK_LSHIFT, 0x45, KEYEVENTF_KEYUP, 0);
                }
                else if (action.Type.Equals(ClickType.leftArrow))//Developer
                {
                    //unocode InputSimulator.SimulateKeyPress(VirtualKeyCode.LEFT);
                    u.Keyboard.KeyPress(VirtualKeyCode.LEFT);
                }
                else if (action.Type.Equals(ClickType.RightArrow))//Developer
                {
                    //unocode InputSimulator.SimulateKeyPress(VirtualKeyCode.RIGHT);
                    u.Keyboard.KeyPress(VirtualKeyCode.RIGHT);
                }
                else if (action.Type.Equals(ClickType.DownArrow))//Developer
                {
                    //unocode InputSimulator.SimulateKeyPress(VirtualKeyCode.DOWN);
                    u.Keyboard.KeyPress(VirtualKeyCode.DOWN);
                }
                else if (action.Type.Equals(ClickType.UpArrow))//Developer
                {
                    //unocode InputSimulator.SimulateKeyPress(VirtualKeyCode.UP);
                    u.Keyboard.KeyPress(VirtualKeyCode.UP);
                }
                
            }), state);
        }
        private void WorkEnableButtons(object state)
        {
            this.context.Send(new SendOrPostCallback(delegate(object _state)
            {
                enableButtons(true);
            }), state);
        }
        //-----------------------------az inja method hayi ke dare sabt mikone ke che amali badan tavasote thred anjam baiad anjam she
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (byTextEntry) return;
            if (FrmTitleEntry) return;

            if (e.KeyChar.Equals('c') || e.KeyChar.Equals('D')
                || e.KeyChar.Equals('r') || e.KeyChar.Equals('t') || e.KeyChar.Equals('w') || e.KeyChar.Equals('W') || e.KeyChar.Equals('i') 
                || e.KeyChar.Equals('e') || e.KeyChar.Equals('f')  || e.KeyChar.Equals('l')
                || e.KeyChar.Equals('u') || e.KeyChar.Equals('d') || e.KeyChar.Equals('R') || e.KeyChar.Equals('n')
                )//Developer
            {
                end = DateTime.Now;
                if (first)
                {
                    start = end;
                    first = false;
                }

                ClickType ct = ClickType.click;
                if (e.KeyChar.Equals('c'))
                {
                    //cl = ClickType.click;
                }
                else if (e.KeyChar.Equals('D'))
                {
                    ct = ClickType.DoubleClick;
                }
                else if (e.KeyChar.Equals('R'))
                {
                    ct = ClickType.rightClick;
                }
                else if (e.KeyChar.Equals('w'))//Developer
                {
                    ct = ClickType.Write_Raghabeh;
                }
                else if (e.KeyChar.Equals('W'))//Developer
                {
                    ct = ClickType.Write_Moghoofeh;
                }
                else if (e.KeyChar.Equals('i'))//Developer
                {
                    ct = ClickType.SearchFolder;
                }
                else if (e.KeyChar.Equals('n'))//Developer
                {
                    ct = ClickType.SearchImgName;
                }
                else if (e.KeyChar.Equals('n'))//Developer
                {
                    ct = ClickType.SearchImgName;
                }
                //else if (e.KeyChar.Equals('t'))
                //{
                //    ct = ClickType.SendKeys;
                //}
                else if(e.KeyChar.Equals('t'))//Developer
                {
                    ct = ClickType.Tab;
                }
                else if (e.KeyChar.Equals('e'))//Developer
                {
                    ct = ClickType.Enter;
                }
                else if (e.KeyChar.Equals('f'))//Developer
                {
                    ct = ClickType.ShifF10;
                }
                else if (e.KeyChar.Equals('r'))//Developer
                {
                    ct = ClickType.RightArrow;
                }
                else if (e.KeyChar.Equals('u'))//Developer
                {
                    ct = ClickType.UpArrow;
                }
                else if (e.KeyChar.Equals('d'))//Developer
                {
                    ct = ClickType.DownArrow;
                }
                else if (e.KeyChar.Equals('l'))//Developer
                {
                    ct = ClickType.leftArrow;
                }

                int x = Cursor.Position.X;
                int y = Cursor.Position.Y;
                TimeSpan ts = end - start;
                double sec = 0;
                if (nWait.Value.Equals(0))
                {
                    sec = ts.TotalSeconds;
                    sec = Math.Round(sec, 1);
                }
                else
                {
                    sec = (double)nWait.Value;
                }
                start = end;
                string point = x.ToString() + "," + y.ToString();

                //string text = ct.Equals(ClickType.SendKeys) ? textBox_FormTitle.Text : string.Empty;
                string text =  ((ComboboxItem)comboBox_Forms.SelectedItem).Value.ToString();
                string Displaytext = ((ComboboxItem)comboBox_Forms.SelectedItem).Text.ToString();
                comboBox_Forms.SelectedIndex = 0;
                //25bahmab textBox_FormTitle.Text; textBox_FormTitle.Text = string.Empty;
                ListViewItem lvi = new ListViewItem(new string[] { point, ct.ToString(), "0", text, Displaytext });
                ActionEntry acion = new ActionEntry(x, y, text, 0, ct);
                lvi.Tag = acion;
                lvActions.Items.Add(lvi);
                int index = lvActions.Items.Count;
                if (index > 1)
                {
                    lvActions.Items[index - 2].SubItems[2].Text = sec.ToString();
                    (lvActions.Items[index - 2].Tag as ActionEntry).Interval = (int)sec;
                }
            }
            if (e.KeyChar.Equals('S'))
            {
                btnStart.PerformClick();
            }
            else if (e.KeyChar.Equals((char)Keys.Escape))//Esc
            {
                btnCancel.PerformClick();
                this.Focus();
            }
            
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lvActions.Items.Clear();
            first = true;
        }
       public List<double> Mylist;
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox_DefFolder.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("لطفا مسیر پیش  فرض را مشخص کنید.");
                    return;
                }
                UiUtility.WriteInLogFile(textBox_DefFolder.Text);
                if (checkedListBox_Classes.CheckedItems.Count == 0)
                {
                    MessageBox.Show("لطفا دسته بندی های مورد نظر را از لیست انتخاب کنید");
                    return;
                }
                ClassesIds = checkedListBox_Classes.CheckedItems.OfType<FileClass>().Select(s => s.ClassID).ToList();

                //Mylist = radioButton_Moghoofeh.Checked ? Clicker.DataSection.BusseinessMethods.ListForUpLoad() :
                //  Clicker.DataSection.BusseinessMethods.GetListOfRaghabeForUpload();

                Mylist = radioButton_Moghoofeh.Checked ? Clicker.DataSection.BusseinessMethods.ListForUpLoad_WithRelatedClass(ClassesIds) :
                  Clicker.DataSection.BusseinessMethods.GetListOfRaghabeForUpload_WithRelatedClass(ClassesIds);
                if (Mylist == null || Mylist.Count == 0)
                {
                    MessageBox.Show("موردی یافت نشد");
                    return;
                }



                enableButtons(false);
                ApplicationIsOpen();

                if (runActionThread == null || !runActionThread.IsAlive)
                {
                    actions.Clear();
                    foreach (ListViewItem lvi in lvActions.Items)
                    {
                        actions.Add(lvi.Tag as ActionEntry);
                    }
                    runActionThread = new Thread(RunAction);
                    runActionThread.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString()); 
            }
            

        }
        private void enableButtons(bool enabel)
        {
            btnClear.Enabled = enabel;
            btnOpen.Enabled = enabel;
            btnSave.Enabled = enabel;
            lvActions.Enabled = enabel;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (runActionThread != null && runActionThread.IsAlive)
            {
                runActionThread.Abort();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (runActionThread != null && runActionThread.IsAlive)
            {
                runActionThread.Abort();
                enableButtons(true);
            }
        }

        private void txbEntry_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals((Char)Keys.Escape))//Esc
            {
                nWait.Focus();
            }
        }

        private void txbEntry_Enter(object sender, EventArgs e)
        {
            byTextEntry = true;
        }

        private void txbEntry_Leave(object sender, EventArgs e)
        {
            byTextEntry = false;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int cout = lvActions.Items.Count;
            int coutselect = lvActions.SelectedItems.Count;
            if (cout.Equals(coutselect))
            {
                btnClear.PerformClick();
            }
            else
            {
                for (int i = coutselect - 1; i >= 0; --i)
                {
                    int index = lvActions.SelectedItems[i].Index;
                    lvActions.Items[index].Remove();
                }
            }
        }

        private void lvActions_MouseDown(object sender, MouseEventArgs e)
        {
            int coutselect = lvActions.SelectedItems.Count;
            deleteToolStripMenuItem.Available = coutselect > 0;
            editToolStripMenuItem.Available = coutselect == 1;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog file = new SaveFileDialog();
            file.Filter = "XML File |*.xml";
            if (file.ShowDialog() == DialogResult.OK)
            {
                XmlSerializer ser = new XmlSerializer(typeof(ActionsEntry));
                ActionsEntry tmpAction = new ActionsEntry();
                List<ActionsEntryAction> tmpActionsEntryActions = new List<ActionsEntryAction>();
                foreach (ListViewItem lvi in lvActions.Items)
                {
                    ActionEntry tmpActionEntry = lvi.Tag as ActionEntry;
                    ActionsEntryAction tmpActionsEntryAction = new ActionsEntryAction();
                    tmpActionsEntryAction.X = tmpActionEntry.X;
                    tmpActionsEntryAction.Y = tmpActionEntry.Y;
                    tmpActionsEntryAction.Text = tmpActionEntry.Text;
                    tmpActionsEntryAction.interval = tmpActionEntry.Interval;
                    tmpActionsEntryAction.Type = (int)tmpActionEntry.Type;
                    tmpActionsEntryActions.Add(tmpActionsEntryAction);
                }
                tmpAction.Action = tmpActionsEntryActions.ToArray();
                
               
                using (XmlWriter writer = XmlWriter.Create(file.FileName))
                {
                    ser.Serialize(writer, tmpAction);
                    
                }

            }
            PatternInfo PatternInfoOb = new PatternInfo();
            PatternInfoOb._NeededCode = radioButton_Moghoofeh.Checked ? "Moghoofeh" : "Raghabeh";
            //---------------
            List<string> Result = new List<string>();
            List<FileClass> ggy = new List<FileClass>();
            ggy = checkedListBox_Classes.CheckedItems.OfType<FileClass>().ToList();
           

           //List<SelectedPictureClasses> SelectedPictureClassesOb=new List<SelectedPictureClasses>();

          // SelectedPictureClasses x;
           foreach (var item in ggy)
           {
               Result.Add(item.FileClassNameEN);
           //  x = new SelectedPictureClasses();
             //  x._PictureClasses =item.FileClassNameEN;
               //SelectedPictureClassesOb.Add(x);
           }                       
          //  PatternInfoOb._NededdClass = SelectedPictureClassesOb.ToArray();
            PatternInfoOb._NededdClassString = Result.ToArray();
            XmlSerializer ser2 = new XmlSerializer(typeof(PatternInfo));

            using (XmlWriter writer = XmlWriter.Create(file.FileName.Split('.').First() + "_pat.xml"))
            {
                ser2.Serialize(writer, PatternInfoOb);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            bool runIt = false;
            if (MessageBox.Show("آیا پس از بازکردن الگو، بلافاصله اجرا شود؟", "Clicker", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                 == DialogResult.Yes)
            {
                runIt = true;
            }
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "XML File |*.xml";
            file.Multiselect = false;
            if (file.ShowDialog() == DialogResult.OK)
            {
                OpenFileXml(runIt, file.FileName);
                string name = file.SafeFileName;
                 PatternName = name.Substring(0, name.Length - 4);
                this.Text = "الگو - " + PatternName;
                OpenFileXml_Pattern(file.FileName.Split('.').First()+"_pat.xml");                
            }
        }

        private void OpenFileXml(bool runIt, string file)
        {
            //Get data from XML file
            XmlSerializer ser = new XmlSerializer(typeof(ActionsEntry));
            using (FileStream fs = System.IO.File.Open(file, FileMode.Open))
            {
                try
                {
                    ActionsEntry entry = (ActionsEntry)ser.Deserialize(fs);
                    lvActions.Items.Clear();
                    foreach (ActionsEntryAction ae in entry.Action)
                    {
                        string point = ae.X.ToString() + "," + ae.Y.ToString();
                        string interval = (ae.interval).ToString();
                        string Displaytext = "";
                        switch (ae.Text)
                        {
                            case "0": Displaytext = "";
                                break;
                            case "1": Displaytext = "فرم جستجو موقوفه";
                                break;
                            case "2": Displaytext = "فرم جستجو رقبه";
                                break;
                            case "3": Displaytext = "فرم آپلود تصاویر";
                                break;
                            case "4": Displaytext = "بستن فرم";
                                break;
                               
                        }
                        ListViewItem lvi = new ListViewItem(new string[] { point, ((ClickType)(ae.Type)).ToString(), interval, ae.Text, Displaytext });
                        ActionEntry acion = new ActionEntry(ae.X, ae.Y, ae.Text, ae.interval, (ClickType)(ae.Type));
                        lvi.Tag = acion;
                        lvActions.Items.Add(lvi);
                    }
                    
                    if (runIt)
                    {
                        btnStart.PerformClick();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Clicer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        /// <summary>
        /// open hj,kj
        /// </summary>
        /// <param name="runIt"></param>
        /// <param name="file"></param>
        private void OpenFileXml_Pattern(string file)//Developer
        {
            //Get data from XML file
            XmlSerializer ser = new XmlSerializer(typeof(PatternInfo));
            using (FileStream fs = System.IO.File.Open(file, FileMode.Open))
            {
                try
                {
                    PatternInfo entry = (PatternInfo)ser.Deserialize(fs);
                    if (entry._NeededCode == "Moghoofeh")
                    {
                        radioButton_Moghoofeh.Checked = true;
                        radioButton_Raghabe.Checked = false;
                    }
                    else
                    {
                        radioButton_Raghabe.Checked = true;
                        radioButton_Moghoofeh.Checked = false;
                    }
                    for (int i = 0; i < checkedListBox_Classes.Items.Count; i++)//aval hame uncheck she
                              checkedListBox_Classes.SetItemCheckState(i, CheckState.Unchecked);
                 List<string>  listboxItem= checkedListBox_Classes.Items.OfType<FileClass>().Select(s => s.FileClassNameEN).ToList();
                    foreach (string myclass in entry._NededdClassString)
                    {
                        for (int i = 0; i < checkedListBox_Classes.Items.Count; i++)
                                 checkedListBox_Classes.SetItemCheckState
                                     (i, (listboxItem[i].IndexOf(myclass) >= 0 ? CheckState.Checked : checkedListBox_Classes.GetItemCheckState(i)));
                    }
                    //listboxItem[i].IndexOf(myclass) >= 0 ? CheckState.Checked : 
                    

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Clicer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
       
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActionEntry action = lvActions.SelectedItems[0].Tag as ActionEntry;
            ویرایش frm = new ویرایش(action);
            frm.Actionentry = action;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                action = frm.Actionentry;
                lvActions.SelectedItems[0].Tag = action;
                lvActions.SelectedItems[0].Text = action.X + "," + action.Y;
                lvActions.SelectedItems[0].SubItems[1].Text = action.Type.ToString();
                lvActions.SelectedItems[0].SubItems[2].Text = action.Interval.ToString();
                lvActions.SelectedItems[0].SubItems[3].Text = action.Text;
                switch (action.Text)
                {
                    case "0": lvActions.SelectedItems[0].SubItems[4].Text = "";
                        break;
                    case "1": lvActions.SelectedItems[0].SubItems[4].Text = "فرم جستجو موقوفه";
                        break;
                    case "2": lvActions.SelectedItems[0].SubItems[4].Text = "فرم جستجو رقبه";
                        break;
                    case "3": lvActions.SelectedItems[0].SubItems[4].Text = "فرم آپلود تصاویر";
                        break;
                    case "4": lvActions.SelectedItems[0].SubItems[4].Text = "بستن فرم";
                        break;

                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
           // this.linkLabel.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("http://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys.aspx");

        }

        private void lvActions_DoubleClick(object sender, EventArgs e)
        {
            if (editToolStripMenuItem.Available)
            {
                editToolStripMenuItem.PerformClick();
            }
        }
        #endregion
        #region keyboard
      /*  void EnterCode(int zzzzz)
        {
            string[] x = SplitDigits(zzzzz);
            for (int j = 0; j < x.Length-1; j++)
            {
                switch (x[j])
                {
                    case "0": 
                        InputSimulator.SimulateKeyPress(VirtualKeyCode.NUMPAD0);
                       
                        break;
                    case "1":
                        InputSimulator.SimulateKeyPress(VirtualKeyCode.NUMPAD1);
                       
                        break;
                    case "2":
                        InputSimulator.SimulateKeyPress(VirtualKeyCode.NUMPAD2);
                       
                        break;
                    case "3": 
                        InputSimulator.SimulateKeyPress(VirtualKeyCode.NUMPAD3);
                       
                        break;
                    case "4": 
                        InputSimulator.SimulateKeyPress(VirtualKeyCode.NUMPAD4);
                       
                        break;
                    case "5":
                        InputSimulator.SimulateKeyPress(VirtualKeyCode.NUMPAD5);
                       
                        break;
                    case "6": 
                        InputSimulator.SimulateKeyPress(VirtualKeyCode.NUMPAD6);
                       
                        break;
                    case "7": 
                        InputSimulator.SimulateKeyPress(VirtualKeyCode.NUMPAD7);
                      
                        break;
                    case "8": 
                        InputSimulator.SimulateKeyPress(VirtualKeyCode.NUMPAD8);
                       
                        break;
                    case "9":
                        InputSimulator.SimulateKeyPress(VirtualKeyCode.NUMPAD9);
                        
                        break;
 
                } 
            }
            InputSimulator.SimulateKeyPress(VirtualKeyCode.NUMPAD2);

        }
        string[]  SplitDigits(int CodeParam)
        {
            string[] x = new string[CodeParam.ToString().Length];
            for (int i = 0; i < CodeParam.ToString().Length; i++)
            {
                x[i] = CodeParam.ToString().Substring(i, 1);
            }
            return x;
        }*/
        #endregion
        #region new
        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBOARD_INPUT
        {
            public const uint Type = 1;
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        };

        [StructLayout(LayoutKind.Explicit)]
        struct KEYBDINPUT
        {
            [FieldOffset(0)]
            public ushort wVk;
            [FieldOffset(2)]
            public ushort wScan;
            [FieldOffset(4)]
            public uint dwFlags;
            [FieldOffset(8)]
            public uint time;
            [FieldOffset(12)]
            public IntPtr dwExtraInfo;
        };

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        };

        [StructLayout(LayoutKind.Explicit)]
        struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        };
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, IntPtr pInput, int cbSize);
        [DllImport("user32.dll")]
        static extern bool keybd_event(byte bVk, byte bScan, uint dwFlags,
           int dwExtraInfo);
        void dtyt()
        {
            keybd_event(0x09, 0x0f, 0x0002, 0);
            keybd_event(0x09, 0x0f, 0, 0); // Tab Press
        }
        #endregion

       /* private void textBox_FormTitle_Enter(object sender, EventArgs e)
        {
            FrmTitleEntry = true;
        }

        private void textBox_FormTitle_Leave(object sender, EventArgs e)
        {
            FrmTitleEntry = false;
        }

        private void textBox_FormTitle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals((Char)Keys.Escape))//Esc
            {
                lvActions.Focus();
            }
        }*/

        private void button_FolderBrows_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowNewFolderButton = false;
            //this.folderBrowserDialog1.RootFolder 
    //System.Environment.SpecialFolder.MyComputer;
            DialogResult result = folderBrowserDialog1.ShowDialog();
             if (result == DialogResult.OK)
             {
                 textBox_DefFolder.Text = folderBrowserDialog1.SelectedPath;
                 // the code here will be executed if the user presses Open in
                 // the dialog.
             }
        }
        public class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            textBox_DefFolder.Text = UiUtility.ReadPath();

            comboBox_Forms.DisplayMember = "Text";
            comboBox_Forms.ValueMember = "Value";

            ComboboxItem item0 = new ComboboxItem();
            item0.Text = "اتنخاب کنید"; item0.Value = "0";
            comboBox_Forms.Items.Add(item0);

            ComboboxItem item = new ComboboxItem();
            item.Text="فرم جستجو موقوفه";item.Value="1";
            comboBox_Forms.Items.Add(item);

            ComboboxItem item2 = new ComboboxItem();
            item2.Text = "فرم جستجو رقبه"; item2.Value = "2";
            comboBox_Forms.Items.Add(item2);

            ComboboxItem item3 = new ComboboxItem();
            item3.Text = "فرم آپلود تصاویر"; item3.Value = "3";
            comboBox_Forms.Items.Add(item3);

            ComboboxItem item4 = new ComboboxItem();
            item4.Text = "بستن فرم"; item4.Value = "4";
            comboBox_Forms.Items.Add(item4);

            comboBox_Forms.SelectedIndex = 0;
            ((ListBox)checkedListBox_Classes).DataSource = Clicker.DataSection.BusseinessMethods.GetFileClassesList();
            ((ListBox)checkedListBox_Classes).DisplayMember="FileClassNameFa";
            ((ListBox)checkedListBox_Classes).ValueMember="FileClassNameEN";            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> Result = new List<string>();
            List<FileClass> ggy = new List<FileClass>();
            ggy = checkedListBox_Classes.CheckedItems.OfType<FileClass>().ToList();
            foreach (var item in ggy)
            {
                Result.Add(item.FileClassNameEN);
            }

           // List<FileClass> ggy = new List<FileClass>();
            //ggy = checkedListBox_Classes.CheckedItems.OfType<FileClass>().ToList();
            //string s;
            //List<string> ggy = new List<string>();
            //DataRowView dw;
            //for (int i = 0; i < checkedListBox_Classes.CheckedItems.Count; i++)
            //{
            //    dw = (DataRowView)(checkedListBox_Classes.CheckedItems[i]);
            //    string xxx = dw.Row[checkedListBox_Classes.DisplayMember].ToString();
            //}                      
        }

        private bool x2IsRunning()
        {
            Process[] processlist = Process.GetProcesses();
            foreach (Process process in processlist)
            {
                if (process.ProcessName.Contains("APPServerClient"))
                    return true;

            }
            return false;
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            Process[] processlist = Process.GetProcesses();
           
            foreach (Process process in processlist)
            {
                if (process.MainWindowTitle!=string.Empty)
                {
                    string a = process.ProcessName;
                    string b = process.MainWindowTitle;
                   // Console.WriteLine("Process: {0} ID: {1} Window title: {2}", process.ProcessName, process.Id, process.MainWindowTitle);
                }
            }

            IntPtr id = IntPtr.Zero; ;
            Process[] processes = Process.GetProcessesByName("mstsc");
            IntPtr windowHandle = IntPtr.Zero;
            foreach (Process p in processes)
            {
                // id = p.Id;//p.HandleCount
                 windowHandle = p.MainWindowHandle;
                 id = p.Handle;
                //p.MainWindowTitle
                GetWindowThreadProcessId(windowHandle, IntPtr.Zero);

                // do something with windowHandle
            }
            
          //GetWindows();
            //
        List<IntPtr> hhh=    GetChildWindows(windowHandle);

        foreach (IntPtr item in hhh)
        {
            int length0 = GetWindowTextLength(item);
            StringBuilder sb0 = new StringBuilder(length0 + 1);
            
            GetWindowText(item, sb0, sb0.Capacity);
            string dsgfs0 = sb0.ToString();
        }
       
            //-----------------
        foreach (IntPtr item in hhh)
        {
            StringBuilder title = new StringBuilder();

            // Get the size of the string required to hold the window title. 
            Int32 size = SendMessage(item, WM_GETTEXTLENGTH, 0,null);

            // If the return is 0, there is no title. 
            if (size > 0)
            {
                title = new StringBuilder(size + 1);

                SendMessage(item, (int)WM_GETTEXT, title.Capacity, title);


            }
             title.ToString();
        }
            //-----------------


        int length = GetWindowTextLength(hhh[3]);
        int length2 = GetWindowTextLength(hhh[4]);
        int length3 = GetWindowTextLength(hhh[5]);
        int length4 = GetWindowTextLength(hhh[6]);
        StringBuilder sb2 = new StringBuilder(length2 + 1);
        GetWindowText(hhh[4], sb2, sb2.Capacity);
        string dsgfs2= sb2.ToString();

        StringBuilder sb3 = new StringBuilder(length3 + 1);
        GetWindowText(hhh[5], sb3, sb3.Capacity);
        string dsgfs3 = sb3.ToString();

        StringBuilder sb4 = new StringBuilder(length4 + 1);
        GetWindowText(hhh[6], sb4, sb4.Capacity);
        string dsgfs4 = sb4.ToString();

        StringBuilder sb = new StringBuilder(length + 1);
        GetWindowText(hhh[3], sb, sb.Capacity);
        string dsgfs = sb.ToString();



        
            FindWindow(null, "اطلاعات موقوفات،رقبات،نیات");
                FindWindow(null,"اطلاعات موفوفات ، رقبات ، نیات");
                    FindWindow(null,"اطلاعات موقوفات ،رقبات ،نیات");
                        FindWindow(null,"اطلاعات موقوفات، رقبات، نیات");
            
            
        }




       // public delegate bool EnumedWindow(IntPtr handleWindow, ArrayList handles);
        public static ArrayList GetWindows()
        {
            ArrayList windowHandles = new ArrayList();
            EnumedWindow callBackPtr = GetWindowHandle;
            EnumWindows(callBackPtr, windowHandles);

            return windowHandles;
        }

        private static bool GetWindowHandle(IntPtr windowHandle, ArrayList windowHandles)
        {
            windowHandles.Add(windowHandle);
            return true;
        }



        private void button2_Click(object sender, EventArgs e)
        {

        }





        /* void hkj()
         {
             IntPtr keyboardLayout = GetKeyboardLayout(0);
             //Get the virtual key of the char.
             short vKey = VkKeyScanEx(char.Parse(m_Splitted[i]), keyboardLayout);
             //Get the low byte from the virtual key.
             byte m_LOWBYTE = (Byte)(vKey & 0xFF);

             //Get the scan code of the key.
             byte sScan = (byte)MapVirtualKey(m_LOWBYTE, 0);

             //Press the key.
             //Key down event, as indicated by the 3rd parameter that is 0.
             keybd_event(m_LOWBYTE, sScan, 0, 0);
         }*/
        #region CaptureScreen
        public void CaptureScreen(double x, double y, double width, double height)
        {

            //INetworkListManager.get_IsConnectedToInternet()
            int ix, iy, iw, ih;
            ix = Convert.ToInt32(x);
            iy = Convert.ToInt32(y);
            iw = Convert.ToInt32(width);
            ih = Convert.ToInt32(height);
            Bitmap image = new Bitmap(iw, ih,
                   System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(image);
            g.CopyFromScreen(ix, iy, ix, iy,
                     new System.Drawing.Size(iw, ih),
                     CopyPixelOperation.SourceCopy);
            string Root = BLLDate.getPersianDateTime(System.DateTime.Now).Replace("/", "-").Replace(":", ".").Substring(0,13);
            //string Root = BLLDate.getPersianDate(System.DateTime.Now).Replace("/", "-").Replace(":", ".");
            //if (!Directory.Exists("d:\\" + Root))
            //    Directory.CreateDirectory("d:\\" + Root);
            String Class = ImgUrl.Substring(0, ImgUrl.LastIndexOf("/")).Split('/').Last();
            if (!Directory.Exists("d:\\" + Root + "\\" +PatternName+"\\"+ CurrentCodeTafzili.ToString() + "\\" + Class))
                Directory.CreateDirectory("d:\\" + Root + "\\"+PatternName+"\\"+  CurrentCodeTafzili.ToString() + "\\" + Class);

            image.Save("d:\\" + Root + "\\" + PatternName + "\\" + CurrentCodeTafzili.ToString() + "\\" + Class + "\\" + DateTime.Now.TimeOfDay.ToString().Replace(":", ".") + ".png", ImageFormat.Png);
          //image.Save("d:\\" + CurrentCodeTafzili.ToString() + DateTime.Now.Second.ToString() + ".png", ImageFormat.Png);
        }
        #endregion

        private void button1_Click_2(object sender, EventArgs e)
        {
            x2IsRunning();
        }

        private void button_reserve_Click(object sender, EventArgs e)
        {
            Setting_Reserve frm = new Setting_Reserve();
            //if (
            frm.ShowDialog();
                    //== DialogResult.OK
            //)
                
        }
    }
}
