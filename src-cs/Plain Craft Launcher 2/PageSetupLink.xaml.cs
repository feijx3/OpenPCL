using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageSetupLink : MyPageRight
    {
        public PageSetupLink()
        {
            base.Loaded += this.PageSetupLink_Loaded;
            this.InitializeComponent();
        }

        private void PageSetupLink_Loaded(object sender, RoutedEventArgs e)
        {
            this.PanBack.ScrollToHome();
            if (this.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init == null)
            {
                Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init, new StaticLocalInitFlag(), null);
            }
            bool flag = false;
            try
            {
                Monitor.Enter(this.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init, ref flag);
                if (this.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init.State == 0)
                {
                    this.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init.State = 2;
                    this.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded = false;
                }
                else if (this.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init.State == 2)
                {
                    throw new IncompleteInitialization();
                }
            }
            finally
            {
                this.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init.State = 1;
                if (flag)
                {
                    Monitor.Exit(this.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init);
                }
            }
            checked
            {
                if (!this.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded)
                {
                    this.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded = true;
                    ModAnimation.AniControlEnabled++;
                    this.Reload();
                    ModAnimation.AniControlEnabled--;
                }
            }
        }

        public void Reload()
        {
            SettingService.RefreshSettings(this);
        }

        public void Reset()
        {
            try
            {
                SettingService.ResetSettings(this);
                Logger.Info("已初始化联机页设置", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupLink.xaml.vb");
                ModMain.Hint("已初始化联机页设置！", ModMain.HintType.Green, false);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "初始化联机页设置失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupLink.xaml.vb");
            }
            this.Reload();
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

        internal virtual MyCard CardArgument
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyComboBox ComboLatencyMode
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyTextBox TextCustomPeer
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        private bool $STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded;

        private StaticLocalInitFlag $STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init;
    }
}
