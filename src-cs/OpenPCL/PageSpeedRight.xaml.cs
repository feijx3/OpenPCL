using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageSpeedRight : OurPageRight
    {
        public PageSpeedRight()
        {
            base.Loaded += this._Lambda___R0_1;
            this.InitializeComponent();
        }

        private void Init()
        {
            this.PanBack.ScrollToHome();
        }



        [CompilerGenerated]
        private void _Lambda___R0_1(object a0, RoutedEventArgs a1)
        {
            this.Init();
        }

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurScrollViewer _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanMain")]
        private StackPanel _PanMain;
    }
}
