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
    public partial class PageInstanceModDisabled : MyPageRight
    {
        public PageInstanceModDisabled()
        {
            this.InitializeComponent();
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

        internal virtual MyCard PanMain
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnDownload
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnDownload_Click);
                RoutedEventHandler routedEventHandler = delegate(object a0, RoutedEventArgs a1)
                {
                    this.BtnDownload_Loaded();
                };
                MyButton myButton = this._BtnDownload;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                    myButton.Loaded -= routedEventHandler;
                }
                this._BtnDownload = value;
                myButton = this._BtnDownload;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                    myButton.Loaded += routedEventHandler;
                }
            }
        }

        internal virtual MyButton BtnVersion
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnVersion_Click);
                MyButton myButton = this._BtnVersion;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnVersion = value;
                myButton = this._BtnVersion;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }
    }
}
