using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageOtherAbout : OurPageRight
    {
        private bool _isInitialized;

        public PageOtherAbout()
        {
            base.Loaded += this.PageOtherAbout_Loaded;
            this.InitializeComponent();
        }

        private void PageOtherAbout_Loaded(object sender, RoutedEventArgs e)
        {
            this.PanBack.ScrollToHome();
            if (!this._isInitialized)
            {
                this._isInitialized = true;
                this.ItemAboutPcl.Info = this.ItemAboutPcl.Info.Replace("%VERSION%", "快照版 2.99.0.0").Replace("%VERSIONCODE%", Conversions.ToString(414)).Replace("%BRANCH%", Conversions.ToString(0));
            }
        }
    }
}
