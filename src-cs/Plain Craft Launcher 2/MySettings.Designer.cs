using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace PCL
{
    [CompilerGenerated]
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.0.3.0")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal sealed partial class MySettings : ApplicationSettingsBase
    {
        public static MySettings Default
        {
            get
            {
                return MySettings.defaultInstance;
            }
        }

        private static MySettings defaultInstance = (MySettings)SettingsBase.Synchronized(new MySettings());
    }
}
