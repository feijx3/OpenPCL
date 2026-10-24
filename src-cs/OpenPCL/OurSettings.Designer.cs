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
    internal sealed partial class OurSettings : ApplicationSettingsBase
    {
        public static OurSettings Default
        {
            get
            {
                return OurSettings.defaultInstance;
            }
        }

        private static OurSettings defaultInstance = (OurSettings)SettingsBase.Synchronized(new OurSettings());
    }
}
