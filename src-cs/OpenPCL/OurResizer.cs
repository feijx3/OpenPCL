using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.VisualBasic;

namespace PCL
{
    public class OurResizer
    {
        // Note: this type is marked as 'beforefieldinit'.
        static OurResizer()
        {
        }

        public OurResizer(Window target)
        {
            this.target = null;
            this.resizeRight = false;
            this.resizeLeft = false;
            this.resizeUp = false;
            this.resizeDown = false;
            this.leftElements = new Dictionary<UIElement, short>();
            this.rightElements = new Dictionary<UIElement, short>();
            this.upElements = new Dictionary<UIElement, short>();
            this.downElements = new Dictionary<UIElement, short>();
            this.startMousePoint = default(OurResizer.PointAPI);
            this.startWindowSize = default(Size);
            this.startWindowLeftUpPoint = default(OurResizer.POINT);
            this.target = target;
            if (Information.IsNothing(target))
            {
                throw new Exception("Invalid Window handle");
            }
            target.SourceInitialized += this.OurMacClass_SourceInitialized;
        }

        private void OurMacClass_SourceInitialized(object sender, EventArgs e)
        {
            this.hs = PresentationSource.FromVisual((Visual)sender) as HwndSource;
            this.hs.AddHook(new HwndSourceHook(this.WndProc));
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 36)
            {
                OurResizer.WmGetMinMaxInfo(hwnd, lParam);
                handled = true;
            }
            return (IntPtr)0;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            object obj = Marshal.PtrToStructure(lParam, typeof(OurResizer.MINMAXINFO));
            OurResizer.MINMAXINFO minmaxinfo = ((obj != null) ? ((OurResizer.MINMAXINFO)obj) : default(OurResizer.MINMAXINFO));
            int num = 2;
            IntPtr intPtr = OurResizer.MonitorFromWindow(hwnd, num);
            checked
            {
                if (intPtr != IntPtr.Zero)
                {
                    OurResizer.MONITORINFO monitorinfo = new OurResizer.MONITORINFO();
                    OurResizer.GetMonitorInfo(intPtr, monitorinfo);
                    OurResizer.RECT rcWork = monitorinfo.rcWork;
                    OurResizer.RECT rcMonitor = monitorinfo.rcMonitor;
                    minmaxinfo.ptMaxPosition.x = Math.Abs(rcWork.left - rcMonitor.left);
                    minmaxinfo.ptMaxPosition.y = Math.Abs(rcWork.top - rcMonitor.top);
                    minmaxinfo.ptMaxSize.y = Math.Abs(rcWork.bottom - rcWork.top);
                    OurResizer.workAreaMaxHeight = minmaxinfo.ptMaxSize.y;
                    if (rcWork.Height == rcMonitor.Height)
                    {
                        ref int ptr = ref minmaxinfo.ptMaxSize.y;
                        minmaxinfo.ptMaxSize.y = ptr - 2;
                    }
                }
                Marshal.StructureToPtr<OurResizer.MINMAXINFO>(minmaxinfo, lParam, true);
            }
        }

        [DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, OurResizer.MONITORINFO lpmi);

        [DllImport("User32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        private void connectMouseHandlers(UIElement element)
        {
            element.MouseLeftButtonDown += this.element_MouseLeftButtonDown;
        }

        public void addResizerRight(UIElement element)
        {
            this.connectMouseHandlers(element);
            this.rightElements.Add(element, 0);
        }

        public void addResizerLeft(UIElement element)
        {
            this.connectMouseHandlers(element);
            this.leftElements.Add(element, 0);
        }

        public void addResizerUp(UIElement element)
        {
            this.connectMouseHandlers(element);
            this.upElements.Add(element, 0);
        }

        public void addResizerDown(UIElement element)
        {
            this.connectMouseHandlers(element);
            this.downElements.Add(element, 0);
        }

        public void addResizerRightDown(UIElement element)
        {
            this.connectMouseHandlers(element);
            this.rightElements.Add(element, 0);
            this.downElements.Add(element, 0);
        }

        public void addResizerLeftDown(UIElement element)
        {
            this.connectMouseHandlers(element);
            this.leftElements.Add(element, 0);
            this.downElements.Add(element, 0);
        }

        public void addResizerRightUp(UIElement element)
        {
            this.connectMouseHandlers(element);
            this.rightElements.Add(element, 0);
            this.upElements.Add(element, 0);
        }

        public void addResizerLeftUp(UIElement element)
        {
            this.connectMouseHandlers(element);
            this.leftElements.Add(element, 0);
            this.upElements.Add(element, 0);
        }

        private void element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OurResizer.GetCursorPos(out this.startMousePoint);
            checked
            {
                this.startMousePoint.X = (int)Math.Round(ModBase.GetWPFSize((double)this.startMousePoint.X));
                this.startMousePoint.Y = (int)Math.Round(ModBase.GetWPFSize((double)this.startMousePoint.Y));
                this.startWindowSize = new Size(this.target.Width, this.target.Height);
                this.startWindowLeftUpPoint = new OurResizer.POINT((int)Math.Round(this.target.Left), (int)Math.Round(this.target.Top));
                UIElement uielement = (UIElement)sender;
                if (this.leftElements.ContainsKey(uielement))
                {
                    this.resizeLeft = true;
                }
                if (this.rightElements.ContainsKey(uielement))
                {
                    this.resizeRight = true;
                }
                if (this.upElements.ContainsKey(uielement))
                {
                    this.resizeUp = true;
                }
                if (this.downElements.ContainsKey(uielement))
                {
                    this.resizeDown = true;
                }
                ModBase.RunInNewThread(new Action(this.updateSizeLoop), "窗口大小调整检测", ThreadPriority.Normal);
            }
        }

        private void updateSizeLoop()
        {
            try
            {
                while (this.resizeDown || this.resizeLeft || this.resizeRight || this.resizeUp)
                {
                    this.target.Dispatcher.Invoke(new Action(this.updateSize), DispatcherPriority.Render);
                    this.target.Dispatcher.Invoke(new Action(this.updateMouseDown), DispatcherPriority.Render);
                    Thread.Sleep(0);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void updateSize()
        {
            OurResizer.PointAPI pointAPI = default(OurResizer.PointAPI);
            OurResizer.GetCursorPos(out pointAPI);
            checked
            {
                pointAPI.X = (int)Math.Round(ModBase.GetWPFSize((double)pointAPI.X));
                pointAPI.Y = (int)Math.Round(ModBase.GetWPFSize((double)pointAPI.Y));
            }
            try
            {
                double num = -1.0;
                double num2 = -1.0;
                double num3 = -10000.0;
                double num4 = -10000.0;
                if (this.resizeRight)
                {
                    if (this.target.Width == this.target.MinWidth)
                    {
                        if (this.startMousePoint.X < pointAPI.X)
                        {
                            num = this.startWindowSize.Width - (double)(checked(this.startMousePoint.X - pointAPI.X));
                        }
                    }
                    else if (this.startWindowSize.Width - (double)(checked(this.startMousePoint.X - pointAPI.X)) >= this.target.MinWidth)
                    {
                        num = this.startWindowSize.Width - (double)(checked(this.startMousePoint.X - pointAPI.X));
                    }
                    else
                    {
                        num = this.target.MinWidth;
                    }
                }
                if (this.resizeDown)
                {
                    if (this.target.Height == this.target.MinHeight)
                    {
                        if (this.startMousePoint.Y < pointAPI.Y)
                        {
                            if (OurResizer.workAreaMaxHeight > 0)
                            {
                                num2 = ((this.startWindowSize.Height - (double)(checked(this.startMousePoint.Y - pointAPI.Y)) + this.target.Top <= (double)OurResizer.workAreaMaxHeight) ? (this.startWindowSize.Height - (double)(checked(this.startMousePoint.Y - pointAPI.Y))) : ((double)OurResizer.workAreaMaxHeight - this.target.Top));
                            }
                            else
                            {
                                num2 = this.startWindowSize.Height - (double)(checked(this.startMousePoint.Y - pointAPI.Y));
                            }
                        }
                    }
                    else if (this.startWindowSize.Height - (double)(checked(this.startMousePoint.Y - pointAPI.Y)) >= this.target.MinHeight)
                    {
                        if (OurResizer.workAreaMaxHeight > 0)
                        {
                            num2 = ((this.startWindowSize.Height - (double)(checked(this.startMousePoint.Y - pointAPI.Y)) + this.target.Top <= (double)OurResizer.workAreaMaxHeight) ? (this.startWindowSize.Height - (double)(checked(this.startMousePoint.Y - pointAPI.Y))) : ((double)OurResizer.workAreaMaxHeight - this.target.Top));
                        }
                        else
                        {
                            num2 = this.startWindowSize.Height - (double)(checked(this.startMousePoint.Y - pointAPI.Y));
                        }
                    }
                    else
                    {
                        num2 = this.target.MinHeight;
                    }
                }
                if (this.resizeLeft)
                {
                    if (this.target.Width == this.target.MinWidth)
                    {
                        if (this.startMousePoint.X > pointAPI.X)
                        {
                            num = this.startWindowSize.Width + (double)this.startMousePoint.X - (double)pointAPI.X;
                            num3 = (double)(checked(this.startWindowLeftUpPoint.x - (this.startMousePoint.X - pointAPI.X)));
                        }
                        else
                        {
                            num = this.target.MinWidth;
                            num3 = (double)this.startWindowLeftUpPoint.x + this.startWindowSize.Width - this.target.Width;
                        }
                    }
                    else if (this.startWindowSize.Width + (double)(checked(this.startMousePoint.X - pointAPI.X)) >= this.target.MinWidth)
                    {
                        num = this.startWindowSize.Width + (double)(checked(this.startMousePoint.X - pointAPI.X));
                        num3 = (double)(checked(this.startWindowLeftUpPoint.x - (this.startMousePoint.X - pointAPI.X)));
                    }
                    else
                    {
                        num = this.target.MinWidth;
                        num3 = (double)this.startWindowLeftUpPoint.x + this.startWindowSize.Width - this.target.Width;
                    }
                }
                if (this.resizeUp)
                {
                    if (this.target.Height == this.target.MinHeight)
                    {
                        if (this.startMousePoint.Y > pointAPI.Y)
                        {
                            num2 = this.startWindowSize.Height + (double)this.startMousePoint.Y - (double)pointAPI.Y;
                            num4 = (double)(checked(this.startWindowLeftUpPoint.y - (this.startMousePoint.Y - pointAPI.Y)));
                        }
                        else
                        {
                            num2 = this.target.MinHeight;
                            num4 = (double)this.startWindowLeftUpPoint.y + this.startWindowSize.Height - this.target.Height;
                        }
                    }
                    else if (this.startWindowSize.Height + (double)(checked(this.startMousePoint.Y - pointAPI.Y)) >= this.target.MinHeight)
                    {
                        num2 = this.startWindowSize.Height + (double)this.startMousePoint.Y - (double)pointAPI.Y;
                        num4 = (double)(checked(this.startWindowLeftUpPoint.y - (this.startMousePoint.Y - pointAPI.Y)));
                    }
                    else
                    {
                        num2 = this.target.MinHeight;
                        num4 = (double)this.startWindowLeftUpPoint.y + this.startWindowSize.Height - this.target.Height;
                    }
                }
                if (num > 10.0 && Math.Abs(num - this.target.Width) > 0.7)
                {
                    this.target.Width = num;
                }
                if (num2 > 10.0 && Math.Abs(num2 - this.target.Height) > 0.7)
                {
                    this.target.Height = num2;
                }
                if (num3 > -9999.0 && Math.Abs(num3 - this.target.Left) > 0.7)
                {
                    this.target.Left = num3;
                }
                if (num4 > -9999.0 && Math.Abs(num4 - this.target.Top) > 0.7)
                {
                    this.target.Top = num4;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void updateMouseDown()
        {
            if (((int)OurResizer.GetAsyncKeyState(1) & 32768) == 0)
            {
                this.resizeRight = false;
                this.resizeLeft = false;
                this.resizeUp = false;
                this.resizeDown = false;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern bool GetCursorPos(out OurResizer.PointAPI lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern short GetAsyncKeyState(int vKey);

        private readonly Window target;

        private bool resizeRight;

        private bool resizeLeft;

        private bool resizeUp;

        private bool resizeDown;

        private readonly Dictionary<UIElement, short> leftElements;

        private readonly Dictionary<UIElement, short> rightElements;

        private readonly Dictionary<UIElement, short> upElements;

        private readonly Dictionary<UIElement, short> downElements;

        private OurResizer.PointAPI startMousePoint;

        private Size startWindowSize;

        private OurResizer.POINT startWindowLeftUpPoint;

        private static int workAreaMaxHeight = -1;

        private HwndSource hs;

        private delegate void RefreshDelegate();

        private struct POINT
        {
            public POINT(int x, int y)
            {
                this = default(OurResizer.POINT);
                this.x = x;
                this.y = y;
            }

            public int x;

            public int y;
        }

        private struct MINMAXINFO
        {
            public OurResizer.POINT ptReserved;

            public OurResizer.POINT ptMaxSize;

            public OurResizer.POINT ptMaxPosition;

            public OurResizer.POINT ptMinTrackSize;

            public OurResizer.POINT ptMaxTrackSize;
        }

        private struct RECT
        {
            // Note: this type is marked as 'beforefieldinit'.
            static RECT()
            {
            }

            public int Width
            {
                get
                {
                    return Math.Abs(checked(this.right - this.left));
                }
            }

            public int Height
            {
                get
                {
                    return checked(this.bottom - this.top);
                }
            }

            public RECT(int left, int top, int right, int bottom)
            {
                this = default(OurResizer.RECT);
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(OurResizer.RECT rcSrc)
            {
                this = default(OurResizer.RECT);
                this.left = rcSrc.left;
                this.top = rcSrc.top;
                this.right = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }

            public int left;

            public int top;

            public int right;

            public int bottom;

            public static OurResizer.RECT Empty = default(OurResizer.RECT);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MONITORINFO
        {
            public MONITORINFO()
            {
                this.cbSize = Marshal.SizeOf(typeof(OurResizer.MONITORINFO));
                this.rcMonitor = default(OurResizer.RECT);
                this.rcWork = default(OurResizer.RECT);
                this.dwFlags = 0;
            }

            public int cbSize;

            public OurResizer.RECT rcMonitor;

            public OurResizer.RECT rcWork;

            public int dwFlags;
        }

        private struct PointAPI
        {
            public int X;

            public int Y;
        }
    }
}
