using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL.Our
{
    [StandardModule]
    [HideModuleName]
    [GeneratedCode("OurTemplate", "11.0.0.0")]
    internal static class OurProject
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ComVisible(false)]
        internal sealed class ThreadSafeObjectProvider<T> where T : new()
        {
            internal T GetInstance
            {
                [DebuggerHidden]
                get
                {
                    if (OurProject.ThreadSafeObjectProvider<T>.m_ThreadStaticValue == null)
                    {
                        OurProject.ThreadSafeObjectProvider<T>.m_ThreadStaticValue = new T();
                    }
                    return OurProject.ThreadSafeObjectProvider<T>.m_ThreadStaticValue;
                }
            }

            [DebuggerHidden]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public ThreadSafeObjectProvider()
            {
            }

            [CompilerGenerated]
            [ThreadStatic]
            private static T m_ThreadStaticValue;
        }
    }
}
