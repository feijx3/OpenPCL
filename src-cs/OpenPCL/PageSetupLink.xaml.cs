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
    public partial class PageSetupLink : OurPageRight
    {
        public PageSetupLink()
        {
            base.Loaded += this.PageSetupLink_Loaded;
            this.InitializeComponent();
        }

        private void PageSetupLink_Loaded(object sender, RoutedEventArgs e)
        {
            this.PanBack.ScrollToHome();
            if (this._STATIC_PageSetupLink_Loaded_20211C128141_IsLoaded_Init == null)
            {
                Interlocked.CompareExchange<StaticLocalInitFlag>(ref this._STATIC_PageSetupLink_Loaded_20211C128141_IsLoaded_Init, new StaticLocalInitFlag(), null);
            }
            bool flag = false;
            try
            {
                Monitor.Enter(this._STATIC_PageSetupLink_Loaded_20211C128141_IsLoaded_Init, ref flag);
                if (this._STATIC_PageSetupLink_Loaded_20211C128141_IsLoaded_Init.State == 0)
                {
                    this._STATIC_PageSetupLink_Loaded_20211C128141_IsLoaded_Init.State = 2;
                    this._STATIC_PageSetupLink_Loaded_20211C128141_IsLoaded = false;
                }
                else if (this._STATIC_PageSetupLink_Loaded_20211C128141_IsLoaded_Init.State == 2)
                {
                    throw new IncompleteInitialization();
                }
            }
            finally
            {
                this._STATIC_PageSetupLink_Loaded_20211C128141_IsLoaded_Init.State = 1;
                if (flag)
                {
                    Monitor.Exit(this._STATIC_PageSetupLink_Loaded_20211C128141_IsLoaded_Init);
                }
            }
            checked
            {
                if (!this._STATIC_PageSetupLink_Loaded_20211C128141_IsLoaded)
                {
                    this._STATIC_PageSetupLink_Loaded_20211C128141_IsLoaded = true;
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






        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurScrollViewer _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanMain")]
        private StackPanel _PanMain;

        [CompilerGenerated]
        [AccessedThroughProperty("CardArgument")]
        private OurCard _CardArgument;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboLatencyMode")]
        private OurComboBox _ComboLatencyMode;

        [CompilerGenerated]
        [AccessedThroughProperty("TextCustomPeer")]
        private OurTextBox _TextCustomPeer;

        private bool _STATIC_PageSetupLink_Loaded_20211C128141_IsLoaded;

        private StaticLocalInitFlag _STATIC_PageSetupLink_Loaded_20211C128141_IsLoaded_Init;
    }
}
