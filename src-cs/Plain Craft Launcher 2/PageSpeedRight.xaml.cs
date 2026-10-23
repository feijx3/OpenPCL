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
    public partial class PageSpeedRight : MyPageRight
    {
        public PageSpeedRight()
        {
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.Init();
            };
            this.InitializeComponent();
        }

        private void Init()
        {
            this.PanBack.ScrollToHome();
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
    }
}
