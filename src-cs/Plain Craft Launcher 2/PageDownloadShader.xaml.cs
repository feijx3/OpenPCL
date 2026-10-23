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
    public partial class PageDownloadShader : MyPageRight
    {
        public PageDownloadShader()
        {
            this.InitializeComponent();
        }

        internal virtual PageResource Content
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }
    }
}
