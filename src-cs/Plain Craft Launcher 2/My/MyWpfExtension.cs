using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic.Devices;
using Microsoft.VisualBasic.Logging;

namespace PCL.My
{
    [StandardModule]
    [HideModuleName]
    internal sealed class MyWpfExtension
    {
        internal static Application Application
        {
            get
            {
                return (Application)global::System.Windows.Application.Current;
            }
        }

        internal static Computer Computer
        {
            get
            {
                return MyWpfExtension.s_Computer.GetInstance;
            }
        }

        internal static User User
        {
            get
            {
                return MyWpfExtension.s_User.GetInstance;
            }
        }

        internal static Log Log
        {
            get
            {
                return MyWpfExtension.s_Log.GetInstance;
            }
        }

        internal static MyWpfExtension.MyWindows Windows
        {
            [DebuggerHidden]
            get
            {
                return MyWpfExtension.s_Windows.GetInstance;
            }
        }

        private static MyProject.ThreadSafeObjectProvider<Computer> s_Computer = new MyProject.ThreadSafeObjectProvider<Computer>();

        private static MyProject.ThreadSafeObjectProvider<User> s_User = new MyProject.ThreadSafeObjectProvider<User>();

        private static MyProject.ThreadSafeObjectProvider<MyWpfExtension.MyWindows> s_Windows = new MyProject.ThreadSafeObjectProvider<MyWpfExtension.MyWindows>();

        private static MyProject.ThreadSafeObjectProvider<Log> s_Log = new MyProject.ThreadSafeObjectProvider<Log>();

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MyGroupCollection("System.Windows.Window", "Create__Instance__", "Dispose__Instance__", "My.MyWpfExtenstionModule.Windows")]
        internal sealed class MyWindows
        {
            [DebuggerHidden]
            private static T Create__Instance__<T>(T Instance) where T : Window, new()
            {
                T t;
                if (Instance == null)
                {
                    if (MyWpfExtension.MyWindows.s_WindowBeingCreated != null)
                    {
                        if (MyWpfExtension.MyWindows.s_WindowBeingCreated.ContainsKey(typeof(T)))
                        {
                            throw new InvalidOperationException("The window cannot be accessed via My.Windows from the Window constructor.");
                        }
                    }
                    else
                    {
                        MyWpfExtension.MyWindows.s_WindowBeingCreated = new Hashtable();
                    }
                    MyWpfExtension.MyWindows.s_WindowBeingCreated.Add(typeof(T), null);
                    t = new T();
                }
                else
                {
                    t = Instance;
                }
                return t;
            }

            [DebuggerHidden]
            private void Dispose__Instance__<T>(ref T instance) where T : Window
            {
                instance = default(T);
            }

            [DebuggerHidden]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public MyWindows()
            {
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public override bool Equals(object o)
            {
                return base.Equals(RuntimeHelpers.GetObjectValue(o));
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            internal new Type GetType()
            {
                return typeof(MyWpfExtension.MyWindows);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public override string ToString()
            {
                return base.ToString();
            }

            public FormMain FormMain
            {
                get
                {
                    this.m_FormMain = MyWpfExtension.MyWindows.Create__Instance__<FormMain>(this.m_FormMain);
                    return this.m_FormMain;
                }
                set
                {
                    if (value != this.m_FormMain)
                    {
                        if (value != null)
                        {
                            throw new ArgumentException("Property can only be set to Nothing");
                        }
                        this.Dispose__Instance__<FormMain>(ref this.m_FormMain);
                    }
                }
            }

            [ThreadStatic]
            private static Hashtable s_WindowBeingCreated;

            [EditorBrowsable(EditorBrowsableState.Never)]
            public FormMain m_FormMain;
        }
    }
}
