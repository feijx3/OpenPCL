using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageOtherHelpDetail : MyPageRight, IRefreshable
    {
        public PageOtherHelpDetail()
        {
            base.Loaded += this.PageOtherHelpDetail_Loaded;
            this.InitializeComponent();
        }

        public void Refresh()
        {
            this.Init(new ModMain.HelpEntry(this.Entry.RawPath));
        }

        private void PageOtherHelpDetail_Loaded(object sender, RoutedEventArgs e)
        {
            this.PanBack.ScrollToTop();
        }

        public bool Init(ModMain.HelpEntry Entry)
        {
            string text = Entry.XamlContent ?? "";
            if (Operators.CompareString(text, "", false) == 0)
            {
                throw new Exception("帮助 xaml 文件为空");
            }
            bool flag;
            try
            {
                text = ModMain.ArgumentReplace(text, new Func<string, string>(StringUtils.XmlEscape), true);
                while (text.Contains("xmlns"))
                {
                    text = text.RegexReplace("xmlns[^\"']*(\"|')[^\"']*(\"|')", "", RegexOptions.None).Replace("xmlns", "");
                }
                text = "<StackPanel xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:sys=\"clr-namespace:System;assembly=mscorlib\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:local=\"clr-namespace:PCL;assembly=Plain Craft Launcher 2\" xmlns:core=\"clr-namespace:MeloongCore;assembly=MeloongCore\" xmlns:corewpf=\"clr-namespace:MeloongCore.Wpf;assembly=MeloongCore.Wpf\">" + text + "</StackPanel>";
                this.Entry = Entry;
                this.PanCustom.Children.Clear();
                this.PanCustom.Children.Add((UIElement)ModBase.GetObjectFromXML(text));
                flag = true;
            }
            catch (Exception ex)
            {
                Logger.Info(string.Format("自定义信息内容：{0}{1}", "\r\n", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherHelpDetail.xaml.vb");
                Logger.Error(ex, "加载帮助 XAML 文件失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherHelpDetail.xaml.vb");
                flag = false;
            }
            return flag;
        }

        internal virtual MyScrollViewer PanBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanCustom
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public ModMain.HelpEntry Entry;
    }
}
