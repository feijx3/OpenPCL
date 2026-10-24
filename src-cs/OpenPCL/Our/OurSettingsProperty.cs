using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL.Our
{
    [StandardModule]
    [HideModuleName]
    [DebuggerNonUserCode]
    [CompilerGenerated]
    internal static class OurSettingsProperty
    {
        [HelpKeyword("My.Settings")]
        internal static OurSettings Settings
        {
            get
            {
                return OurSettings.Default;
            }
        }
    }
}
