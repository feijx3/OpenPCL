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
    public partial class PageOtherAbout : OurPageRight
    {
        public PageOtherAbout()
        {
            base.Loaded += this.PageOtherAbout_Loaded;
            this.IsLoaded = false;
            this.InitializeComponent();
            this.BtnAboutUpdate_WithEvents = this.BtnAboutUpdate;
            this.BtnDonateOutput_WithEvents = this.BtnDonateOutput;
            this.BtnDonateCopy_WithEvents = this.BtnDonateCopy;
            this.BtnDonateInput_WithEvents = this.BtnDonateInput;
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




        internal virtual OurButton BtnAboutUpdate_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnAboutUpdate_Click);
                OurButton ourButton = this._BtnAboutUpdate;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnAboutUpdate = value;
                ourButton = this._BtnAboutUpdate;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnDonateOutput_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnDonateOutput_Click);
                OurButton ourButton = this._BtnDonateOutput;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnDonateOutput = value;
                ourButton = this._BtnDonateOutput;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }


        internal virtual OurButton BtnDonateCopy_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = ((PageOtherAbout._Closure___._IR33_1 == null) ? (PageOtherAbout._Closure___._IR33_1 = new OurButton.ClickEventHandler(PageOtherAbout._Closure___._I._Lambda___R33_1)) : PageOtherAbout._Closure___._IR33_1);
                OurButton ourButton = this._BtnDonateCopy;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnDonateCopy = value;
                ourButton = this._BtnDonateCopy;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnDonateInput_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R37_2);
                OurButton ourButton = this._BtnDonateInput;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnDonateInput = value;
                ourButton = this._BtnDonateInput;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R37_2(object a0, MouseButtonEventArgs a1)
        {
            this.BtnDonateCodeInput_Click();
        }

        private new bool IsLoaded;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurScrollViewer _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanMain")]
        private StackPanel _PanMain;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemAboutPcl")]
        private OurListItem _ItemAboutPcl;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnAboutUpdate")]
        private OurButton _BtnAboutUpdate;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnDonateOutput")]
        private OurButton _BtnDonateOutput;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnDonateDonate")]
        private OurButton _BtnDonateDonate;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnDonateCopy")]
        private OurButton _BtnDonateCopy;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnDonateInput")]
        private OurButton _BtnDonateInput;

        [CompilerGenerated]
        [Serializable]
        private new sealed class _Closure___
        {
            internal void _Lambda___R33_1(object a0, MouseButtonEventArgs a1)
            {
                PageOtherAbout.CopyIdentify();
            }

            public static readonly PageOtherAbout._Closure___ _I = new PageOtherAbout._Closure___();

            public static OurButton.ClickEventHandler _IR33_1;
        }
    }
}
