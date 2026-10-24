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

namespace PCL.Our
{
    [StandardModule]
    [HideModuleName]
    internal static class OurWpfExtension
    {
        // Note: this type is marked as 'beforefieldinit'.
        static OurWpfExtension()
        {
        }

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
                return OurWpfExtension.s_Computer.GetInstance;
            }
        }

        internal static User User
        {
            get
            {
                return OurWpfExtension.s_User.GetInstance;
            }
        }

        internal static Log Log
        {
            get
            {
                return OurWpfExtension.s_Log.GetInstance;
            }
        }

        internal static OurWpfExtension.OurWindows Windows
        {
            [DebuggerHidden]
            get
            {
                return OurWpfExtension.s_Windows.GetInstance;
            }
        }

        private static OurProject.ThreadSafeObjectProvider<Computer> s_Computer = new OurProject.ThreadSafeObjectProvider<Computer>();

        private static OurProject.ThreadSafeObjectProvider<User> s_User = new OurProject.ThreadSafeObjectProvider<User>();

        private static OurProject.ThreadSafeObjectProvider<OurWpfExtension.OurWindows> s_Windows = new OurProject.ThreadSafeObjectProvider<OurWpfExtension.OurWindows>();

        private static OurProject.ThreadSafeObjectProvider<Log> s_Log = new OurProject.ThreadSafeObjectProvider<Log>();

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MyGroupCollection("System.Windows.Window", "Create__Instance__", "Dispose__Instance__", "My.OurWpfExtenstionModule.Windows")]
        internal sealed class OurWindows
        {
            [DebuggerHidden]
            private static T Create__Instance__<T>(T Instance) where T : Window, new()
            {
                T t;
                if (Instance == null)
                {
                    if (OurWpfExtension.OurWindows.s_WindowBeingCreated != null)
                    {
                        if (OurWpfExtension.OurWindows.s_WindowBeingCreated.ContainsKey(typeof(T)))
                        {
                            throw new InvalidOperationException("The window cannot be accessed via My.Windows from the Window constructor.");
                        }
                    }
                    else
                    {
                        OurWpfExtension.OurWindows.s_WindowBeingCreated = new Hashtable();
                    }
                    OurWpfExtension.OurWindows.s_WindowBeingCreated.Add(typeof(T), null);
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
            public OurWindows()
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
                return typeof(OurWpfExtension.OurWindows);
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
                    this.m_FormMain = OurWpfExtension.OurWindows.Create__Instance__<FormMain>(this.m_FormMain);
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
