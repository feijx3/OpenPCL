using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Interop;

namespace PCL
{
    public class DragHelper
    {
        public DragHelper()
        {
        }

        public event EventHandler DragDrop
        {
            [CompilerGenerated]
            add
            {
                EventHandler eventHandler = this.DragDropEvent;
                EventHandler eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    EventHandler eventHandler3 = (EventHandler)Delegate.Combine(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.DragDropEvent, eventHandler3, eventHandler2);
                }
                while (eventHandler != eventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                EventHandler eventHandler = this.DragDropEvent;
                EventHandler eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    EventHandler eventHandler3 = (EventHandler)Delegate.Remove(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.DragDropEvent, eventHandler3, eventHandler2);
                }
                while (eventHandler != eventHandler2);
            }
        }

        public string[] DropFilePaths
        {
            get
            {
                return this._DropFilePathsBackingField;
            }
            private set
            {
                this._DropFilePathsBackingField = value;
            }
        }

        public DragHelper.POINT DropPoint
        {
            get
            {
                return this._DropPointBackingField;
            }
            private set
            {
                this._DropPointBackingField = value;
            }
        }

        public HwndSource HwndIntPtrSource
        {
            [CompilerGenerated]
            get
            {
                return this._HwndIntPtrSource;
            }
            [CompilerGenerated]
            set
            {
                this._HwndIntPtrSource = value;
            }
        }

        public void AddHook()
        {
            this.RemoveDragHook();
            this.HwndIntPtrSource.AddHook(new HwndSourceHook(this.WndProc));
            IntPtr handle = this.HwndIntPtrSource.Handle;
            if (DragHelper.IsUserAnAdmin())
            {
                DragHelper.RevokeDragDrop(handle);
            }
            DragHelper.DragAcceptFiles(handle, true);
            DragHelper.ChangeMessageFilter(handle);
        }

        public void RemoveDragHook()
        {
            this.HwndIntPtrSource.RemoveHook(new HwndSourceHook(this.WndProc));
            DragHelper.DragAcceptFiles(this.HwndIntPtrSource.Handle, false);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            string[] array = null;
            DragHelper.POINT point = default(DragHelper.POINT);
            if (DragHelper.TryGetDropInfo(msg, wParam, ref array, ref point))
            {
                this.DropPoint = point;
                this.DropFilePaths = array;
                EventHandler dragDropEvent = this.DragDropEvent;
                if (dragDropEvent != null)
                {
                    dragDropEvent(this, EventArgs.Empty);
                }
                handled = true;
            }
            return IntPtr.Zero;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ChangeWindowMessageFilterEx(IntPtr hWnd, uint msg, uint action, ref DragHelper.CHANGEFILTERSTRUCT pChangeFilterStruct);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ChangeWindowMessageFilter(uint msg, uint flags);

        [DllImport("shell32.dll")]
        private static extern void DragAcceptFiles(IntPtr hWnd, bool fAccept);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint DragQueryFile(IntPtr hWnd, uint iFile, StringBuilder lpszFile, int cch);

        [DllImport("shell32.dll")]
        private static extern bool DragQueryPoint(IntPtr hDrop, ref DragHelper.POINT lppt);

        [DllImport("shell32.dll")]
        private static extern void DragFinish(IntPtr hDrop);

        [DllImport("ole32.dll")]
        private static extern int RevokeDragDrop(IntPtr hWnd);

        [DllImport("shell32.dll")]
        private static extern bool IsUserAnAdmin();

        private static void ChangeMessageFilter(IntPtr handle)
        {
            Version version = Environment.OSVersion.Version;
            bool flag = version >= new Version(6, 0);
            bool flag2 = version >= new Version(6, 1);
            if (flag)
            {
                DragHelper.CHANGEFILTERSTRUCT changefilterstruct = default(DragHelper.CHANGEFILTERSTRUCT);
                changefilterstruct.cbSize = 8U;
                foreach (uint num in new uint[] { 563U, 73U, 74U })
                {
                    bool flag3;
                    if (flag2)
                    {
                        flag3 = !DragHelper.ChangeWindowMessageFilterEx(handle, num, 1U, ref changefilterstruct);
                    }
                    else
                    {
                        flag3 = !DragHelper.ChangeWindowMessageFilter(num, 1U);
                    }
                    if (flag3)
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
            }
        }

        private static bool TryGetDropInfo(int msg, IntPtr wParam, ref string[] dropFilePaths, ref DragHelper.POINT dropPoint)
        {
            dropFilePaths = null;
            dropPoint = default(DragHelper.POINT);
            bool flag;
            if ((long)msg != 563L)
            {
                flag = false;
            }
            else
            {
                uint num = DragHelper.DragQueryFile(wParam, uint.MaxValue, null, 0);
                checked
                {
                    dropFilePaths = new string[(int)num - 1 + 1];
                    uint num2 = (uint)((int)num - 1);
                    for (uint num3 = 0U; num3 <= num2; num3 += 1U)
                    {
                        StringBuilder stringBuilder = new StringBuilder(260);
                        if (unchecked((ulong)DragHelper.DragQueryFile(wParam, num3, stringBuilder, stringBuilder.Capacity)) > 0UL)
                        {
                            dropFilePaths[(int)num3] = stringBuilder.ToString();
                        }
                    }
                    DragHelper.DragQueryPoint(wParam, ref dropPoint);
                    DragHelper.DragFinish(wParam);
                    flag = true;
                }
            }
            return flag;
        }

        [CompilerGenerated]
        private EventHandler DragDropEvent;

        private string[] _DropFilePathsBackingField;

        private DragHelper.POINT _DropPointBackingField;

        [CompilerGenerated]
        private HwndSource _HwndIntPtrSource;

        private const uint WM_COPYGLOBALDATA = 73U;

        private const uint WM_COPYDATA = 74U;

        private const uint WM_DROPFILES = 563U;

        private const uint MSGFLT_ALLOW = 1U;

        private const uint MSGFLT_ADD = 1U;

        private const int MAX_PATH = 260;

        public struct POINT
        {
            public int X;

            public int Y;
        }

        private struct CHANGEFILTERSTRUCT
        {
            public uint cbSize;

            public uint ExtStatus;
        }
    }
}
