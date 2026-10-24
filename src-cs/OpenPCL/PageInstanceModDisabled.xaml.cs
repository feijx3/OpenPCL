using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageInstanceModDisabled : OurPageRight
    {
        public PageInstanceModDisabled()
        {
            this.InitializeComponent();
            this.BtnDownload_WithEvents = this.BtnDownload;
            this.BtnVersion_WithEvents = this.BtnVersion;
        }

        private void BtnDownload_Click(object sender, EventArgs e)
        {
            ModMain.FrmMain.PageChange(FormMain.PageType.Download, FormMain.PageSubType.Default);
        }

        private void BtnVersion_Click(object sender, EventArgs e)
        {
            ModMain.FrmMain.PageChange(FormMain.PageType.Launch, FormMain.PageSubType.Default);
            ModMain.FrmMain.PageChange(FormMain.PageType.InstanceSelect, FormMain.PageSubType.Default);
        }

        public void BtnDownload_Loaded()
        {
            Visibility visibility = (((Settings.Get<bool>("UiHiddenPageDownload", null) && !PageSetupUI.HiddenForceShow) || (ModMain.FrmSelectRight != null && ModMain.FrmSelectRight.ShowHidden)) ? Visibility.Collapsed : Visibility.Visible);
            if (this.BtnDownload.Visibility != visibility)
            {
                this.BtnDownload.Visibility = visibility;
                this.PanMain.TriggerForceResize();
            }
        }


        internal virtual OurButton BtnDownload_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnDownload;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnDownload_Click);
                RoutedEventHandler routedEventHandler = new RoutedEventHandler(this._Lambda___R10_1);
                OurButton ourButton = this._BtnDownload;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                    ourButton.Loaded -= routedEventHandler;
                }
                this._BtnDownload = value;
                ourButton = this._BtnDownload;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                    ourButton.Loaded += routedEventHandler;
                }
            }
        }

        internal virtual OurButton BtnVersion_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnVersion;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnVersion_Click);
                OurButton ourButton = this._BtnVersion;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnVersion = value;
                ourButton = this._BtnVersion;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R10_1(object a0, RoutedEventArgs a1)
        {
            this.BtnDownload_Loaded();
        }

        [CompilerGenerated]
        [AccessedThroughProperty("PanMain")]
        private OurCard _PanMain;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnDownload")]
        private OurButton _BtnDownload;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnVersion")]
        private OurButton _BtnVersion;
    }
}
