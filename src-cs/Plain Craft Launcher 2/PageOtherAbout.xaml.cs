using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageOtherAbout : MyPageRight
    {
        public PageOtherAbout()
        {
            base.Loaded += this.PageOtherAbout_Loaded;
            this.IsLoaded = false;
            this.InitializeComponent();
        }

        private void PageOtherAbout_Loaded(object sender, RoutedEventArgs e)
        {
            this.PanBack.ScrollToHome();
            if (!this.IsLoaded)
            {
                this.IsLoaded = true;
                this.ItemAboutPcl.Info = this.ItemAboutPcl.Info.Replace("%VERSION%", "快照版 2.99.0.0").Replace("%VERSIONCODE%", Conversions.ToString(414)).Replace("%BRANCH%", Conversions.ToString(0));
            }
        }

        public static void CopyIdentify()
        {
            ModBase.ClipboardSet(ModBase.Identify, true);
        }

        private void BtnDonateCodeInput_Click()
        {
            ModSecret.InputPotatoCode(false);
        }

        private void BtnDonateOutput_Click(object sender, EventArgs e)
        {
            ModSecret.GeneratePotatoCode();
        }

        private void BtnAboutUpdate_Click(object sender, MouseButtonEventArgs e)
        {
            ModSecret.UpdateCheckByButton();
        }

        internal virtual MyScrollViewer PanBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanMain
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyListItem ItemAboutPcl
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnAboutUpdate
        {
            [CompilerGenerated]
            get
            {
                return this._BtnAboutUpdate;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnAboutUpdate_Click);
                MyButton myButton = this._BtnAboutUpdate;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnAboutUpdate = value;
                myButton = this._BtnAboutUpdate;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnDonateOutput
        {
            [CompilerGenerated]
            get
            {
                return this._BtnDonateOutput;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnDonateOutput_Click);
                MyButton myButton = this._BtnDonateOutput;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnDonateOutput = value;
                myButton = this._BtnDonateOutput;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnDonateDonate
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnDonateCopy
        {
            [CompilerGenerated]
            get
            {
                return this._BtnDonateCopy;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = ((PageOtherAbout._Closure$__.$IR33-1 == null) ? (PageOtherAbout._Closure$__.$IR33-1 = delegate(object a0, MouseButtonEventArgs a1)
                {
                    PageOtherAbout.CopyIdentify();
                }) : PageOtherAbout._Closure$__.$IR33-1);
                MyButton myButton = this._BtnDonateCopy;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnDonateCopy = value;
                myButton = this._BtnDonateCopy;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnDonateInput
        {
            [CompilerGenerated]
            get
            {
                return this._BtnDonateInput;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.BtnDonateCodeInput_Click();
                };
                MyButton myButton = this._BtnDonateInput;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnDonateInput = value;
                myButton = this._BtnDonateInput;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        private new bool IsLoaded;
    }
}
