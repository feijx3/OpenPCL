using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL.Our.Resources
{
    [StandardModule]
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [DebuggerNonUserCode]
    [CompilerGenerated]
    [HideModuleName]
    internal static class Resources
    {
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(Resources.resourceMan, null))
                {
                    Resources.resourceMan = new ResourceManager("PCL.Resources", typeof(Resources).Assembly);
                }
                return Resources.resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
            {
                return Resources.resourceCulture;
            }
            set
            {
                Resources.resourceCulture = value;
            }
        }

        internal static byte[] CacheCow_Client
        {
            get
            {
                return (byte[])RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("CacheCow.Client", Resources.resourceCulture));
            }
        }

        internal static byte[] CacheCow_Client_FileStore
        {
            get
            {
                return (byte[])RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("CacheCow.Client.FileStore", Resources.resourceCulture));
            }
        }

        internal static byte[] CacheCow_Common
        {
            get
            {
                return (byte[])RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("CacheCow.Common", Resources.resourceCulture));
            }
        }

        internal static byte[] ThrottleDebounce
        {
            get
            {
                return (byte[])RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("ThrottleDebounce", Resources.resourceCulture));
            }
        }

        internal static byte[] Custom
        {
            get
            {
                return (byte[])RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("Custom", Resources.resourceCulture));
            }
        }

        internal static byte[] ForgeInstaller
        {
            get
            {
                return (byte[])RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("ForgeInstaller", Resources.resourceCulture));
            }
        }

        internal static byte[] Help
        {
            get
            {
                return (byte[])RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("Help", Resources.resourceCulture));
            }
        }

        internal static byte[] Imazen_WebP
        {
            get
            {
                return (byte[])RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("Imazen.WebP", Resources.resourceCulture));
            }
        }

        internal static byte[] libwebp64
        {
            get
            {
                return (byte[])RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("libwebp64", Resources.resourceCulture));
            }
        }

        internal static byte[] MeloongCore
        {
            get
            {
                return (byte[])RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("MeloongCore", Resources.resourceCulture));
            }
        }

        internal static byte[] MeloongCore_Wpf
        {
            get
            {
                return (byte[])RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("MeloongCore.Wpf", Resources.resourceCulture));
            }
        }

        internal static byte[] NAudio
        {
            get
            {
                return (byte[])RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("NAudio", Resources.resourceCulture));
            }
        }

        internal static byte[] Newtonsoft_Json
        {
            get
            {
                return (byte[])RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("Newtonsoft.Json", Resources.resourceCulture));
            }
        }

        internal static byte[] Ookii_Dialogs_Wpf
        {
            get
            {
                return (byte[])RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("Ookii.Dialogs.Wpf", Resources.resourceCulture));
            }
        }

        internal static byte[] PCLCS
        {
            get
            {
                return (byte[])RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("PCLCS", Resources.resourceCulture));
            }
        }

        internal static byte[] System_Net_Http_Formatting
        {
            get
            {
                return (byte[])RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("System.Net.Http.Formatting", Resources.resourceCulture));
            }
        }

        internal static byte[] Transformer
        {
            get
            {
                return (byte[])RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("Transformer", Resources.resourceCulture));
            }
        }

        private static ResourceManager resourceMan;

        private static CultureInfo resourceCulture;
    }
}
