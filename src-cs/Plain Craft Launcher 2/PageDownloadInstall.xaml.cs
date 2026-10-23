using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;
using PCLCS;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageDownloadInstall : MyPageRight
    {
        public PageDownloadInstall()
        {
            base.Initialized += delegate(object a0, EventArgs a1)
            {
                this.LoaderInit();
            };
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.Init();
            };
            this.IsLoad = false;
            this.IsInSelectPage = false;
            this.SelectedOptiFine = null;
            this.SelectedLiteLoader = null;
            this.SelectedForge = null;
            this.SelectedNeoForge = null;
            this.SelectedFabric = null;
            this.SelectedFabricApi = null;
            this.SelectedOptiFabric = null;
            this.IsSelectNameEdited = false;
            this.AutoSelectedFabricApi = false;
            this.AutoSelectedOptiFabric = false;
            this.InitializeComponent();
        }

        public void ControlInit()
        {
            this.DisabledPageAnimControls.Add(this.BtnStart);
            this.LoadOptiFine.State = ModDownload.DlOptiFineListLoader;
            this.LoadLiteLoader.State = ModDownload.DlLiteLoaderListLoader;
            this.LoadFabric.State = ModDownload.DlFabricListLoader;
            this.LoadFabricApi.State = ModDownload.DlFabricApiLoader;
            this.LoadNeoForge.State = ModDownload.DlNeoForgeListLoader;
            this.LoadOptiFabric.State = ModDownload.DlOptiFabricLoader;
            ModDownloadLib.McDownloadForgeRecommendedRefresh();
        }

        public virtual void LoaderInit()
        {
            this.ControlInit();
            base.PageLoaderInit<string, ModDownload.DlClientListResult>(this.LoadMinecraft, this.PanLoad, this.PanAllBack, null, new ModLoader.LoaderTask<string, ModDownload.DlClientListResult>("关闭下载", (PageDownloadInstall._Closure$__.$IR3-3 == null) ? (PageDownloadInstall._Closure$__.$IR3-3 = delegate(ModLoader.LoaderTask<string, ModDownload.DlClientListResult> a0)
            {
                ((PageDownloadInstall._Closure$__.$I3-0 == null) ? (PageDownloadInstall._Closure$__.$I3-0 = delegate
                {
                    throw new Exception("为遵守相关法律法规，PCL 暂时不再提供 MC 下载功能，非常抱歉 qwq……");
                }) : PageDownloadInstall._Closure$__.$I3-0)();
            }) : PageDownloadInstall._Closure$__.$IR3-3, null, ThreadPriority.Normal), delegate(ModLoader.LoaderTask<string, ModDownload.DlClientListResult> a0)
            {
                this.LoadMinecraft_OnFinish();
            }, null, true);
        }

        public virtual void Init()
        {
            if (this.IsLoad)
            {
                this.IsSelectNameEdited = false;
                this.ExitSelectPage();
                if (ModDownload.DlClientListLoader.State == ModBase.LoadState.Finished)
                {
                    this.TrySelectWaitingVersion();
                }
            }
            this.IsLoad = true;
            this.PanBack.ScrollToHome();
            ModDownload.DlOptiFineListLoader.Start(null, false);
            ModDownload.DlLiteLoaderListLoader.Start(null, false);
            ModDownload.DlFabricListLoader.Start(null, false);
            ModDownload.DlNeoForgeListLoader.Start(null, false);
            this.TextSelectName.ValidateRules = new Collection<Validate>
            {
                new ValidateFolderName(ModMinecraft.McFolderSelected + "versions", true, null)
            };
            this.TextSelectName.Validate();
            this.ReloadSelected();
        }

        public void PrepareSelectPage()
        {
            this.IsInSelectPage = true;
            this.PanInner.Margin = new Thickness(25.0, 10.0, 25.0, 40.0);
            this.PanSelect.Visibility = Visibility.Visible;
            this.PanSelect.IsHitTestVisible = true;
            this.PanMinecraft.IsHitTestVisible = false;
            this.PanBack.IsHitTestVisible = false;
            this.PanBack.ScrollToHome();
            this.DisabledPageAnimControls.Remove(this.BtnStart);
            this.BtnStart.Show = true;
            MyCard[] array = new MyCard[] { this.CardOptiFine, this.CardLiteLoader, this.CardForge, this.CardNeoForge, this.CardFabric, this.CardFabricApi, this.CardOptiFabric };
            checked
            {
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].IsSwapped = true;
                }
                try
                {
                    foreach (FrameworkElement frameworkElement in base.GetAllAnimControls(this.PanSelect, false))
                    {
                        frameworkElement.Opacity = 1.0;
                        if (frameworkElement.RenderTransform == null || frameworkElement.RenderTransform is TranslateTransform)
                        {
                            frameworkElement.RenderTransform = new TranslateTransform();
                        }
                    }
                }
                finally
                {
                    IEnumerator<FrameworkElement> enumerator;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
            }
        }

        private void EnterSelectPage()
        {
            if (!this.IsInSelectPage)
            {
                this.AutoSelectedFabricApi = false;
                this.AutoSelectedOptiFabric = false;
                this.PrepareSelectPage();
                if (McVersion.IsStandardFormat(this.VanillaName))
                {
                    ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>> loaderTask = new ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>("DlForgeVersion " + this.VanillaName, new Action<ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>>(ModDownload.DlForgeVersionMain), null, ThreadPriority.Normal);
                    this.LoadForge.State = loaderTask;
                    loaderTask.Start(this.VanillaName, false);
                }
                ModDownload.DlFabricApiLoader.Start(null, false);
                ModDownload.DlOptiFabricLoader.Start(null, false);
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaOpacity(this.PanMinecraft, -this.PanMinecraft.Opacity, 70, 10, null, false),
                    ModAnimation.AaTranslateX(this.PanMinecraft, -50.0 - ((TranslateTransform)this.PanMinecraft.RenderTransform).X, 90, 10, null, false),
                    ModAnimation.AaCode(delegate
                    {
                        this.PanBack.ScrollToHome();
                        this.ReloadComponentLists();
                        this.PanMinecraft.Visibility = Visibility.Collapsed;
                    }, 0, true),
                    ModAnimation.AaOpacity(this.PanSelect, 1.0 - this.PanSelect.Opacity, 70, 100, null, false),
                    ModAnimation.AaTranslateX(this.PanSelect, -((TranslateTransform)this.PanSelect.RenderTransform).X, 160, 100, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.ExtraStrong), false),
                    ModAnimation.AaCode(delegate
                    {
                        this.PanBack.IsHitTestVisible = true;
                    }, 0, true)
                }, "FrmDownloadInstall SelectPageSwitch", true);
            }
        }

        public void ReloadComponentLists()
        {
            this.TextSelectName.Validate();
            this.OptiFine_Loaded();
            this.LiteLoader_Loaded();
            this.Forge_Loaded();
            this.NeoForge_Loaded();
            this.Fabric_Loaded();
            this.FabricApi_Loaded();
            this.OptiFabric_Loaded();
            this.ReloadSelected();
        }

        public void ExitSelectPage()
        {
            if (this.IsInSelectPage)
            {
                this.IsInSelectPage = false;
                this.PanInner.Margin = new Thickness(25.0, 10.0, 25.0, 25.0);
                this.DisabledPageAnimControls.Add(this.BtnStart);
                this.BtnStart.Show = false;
                this.ClearSelected();
                this.PanMinecraft.Visibility = Visibility.Visible;
                this.PanSelect.IsHitTestVisible = false;
                this.PanMinecraft.IsHitTestVisible = true;
                this.PanBack.IsHitTestVisible = false;
                this.PanBack.ScrollToHome();
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaOpacity(this.PanSelect, -this.PanSelect.Opacity, 70, 10, null, false),
                    ModAnimation.AaTranslateX(this.PanSelect, 50.0 - ((TranslateTransform)this.PanSelect.RenderTransform).X, 90, 10, null, false),
                    ModAnimation.AaCode(delegate
                    {
                        this.PanBack.ScrollToHome();
                    }, 0, true),
                    ModAnimation.AaOpacity(this.PanMinecraft, 1.0 - this.PanMinecraft.Opacity, 70, 100, null, false),
                    ModAnimation.AaTranslateX(this.PanMinecraft, -((TranslateTransform)this.PanMinecraft.RenderTransform).X, 160, 100, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.ExtraStrong), false),
                    ModAnimation.AaCode(delegate
                    {
                        this.PanSelect.Visibility = Visibility.Collapsed;
                        this.PanBack.IsHitTestVisible = true;
                    }, 0, true)
                }, "FrmDownloadInstall SelectPageSwitch", false);
            }
        }

        public virtual void MinecraftSelected(MyListItem sender, MouseButtonEventArgs e)
        {
            this.VanillaName = sender.Title;
            this.VanillaData = (JObject)sender.Tag;
            this.VanillaIcon = sender.Logo;
            this.LabMinecraftVersion.Text = this.VanillaName;
            this.EnterSelectPage();
        }

        private int VanillaDrop
        {
            get
            {
                return McVersion.VersionToDrop(this.VanillaName);
            }
        }

        public virtual void ReloadSelected()
        {
            if (this.$STATIC$ReloadSelected$2001$Ongoing$Init == null)
            {
                Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$ReloadSelected$2001$Ongoing$Init, new StaticLocalInitFlag(), null);
            }
            bool flag = false;
            try
            {
                Monitor.Enter(this.$STATIC$ReloadSelected$2001$Ongoing$Init, ref flag);
                if (this.$STATIC$ReloadSelected$2001$Ongoing$Init.State == 0)
                {
                    this.$STATIC$ReloadSelected$2001$Ongoing$Init.State = 2;
                    this.$STATIC$ReloadSelected$2001$Ongoing = false;
                }
                else if (this.$STATIC$ReloadSelected$2001$Ongoing$Init.State == 2)
                {
                    throw new IncompleteInitialization();
                }
            }
            finally
            {
                this.$STATIC$ReloadSelected$2001$Ongoing$Init.State = 1;
                if (flag)
                {
                    Monitor.Exit(this.$STATIC$ReloadSelected$2001$Ongoing$Init);
                }
            }
            if (this.VanillaName != null && !this.$STATIC$ReloadSelected$2001$Ongoing)
            {
                this.$STATIC$ReloadSelected$2001$Ongoing = true;
                this.SelectNameUpdate();
                this.ImgLogo.Source = this.GetSelectLogo();
                this.ImgMinecraft.Source = this.VanillaIcon;
                string text = this.LoadOptiFineGetError();
                this.CardOptiFine.MainSwap.Visibility = ((text == null) ? Visibility.Visible : Visibility.Collapsed);
                if (text != null)
                {
                    this.CardOptiFine.IsSwapped = true;
                }
                this.SetPanelVisibility(this.PanOptiFineInfo, this.CardOptiFine.IsSwapped);
                if (this.SelectedOptiFine == null)
                {
                    this.BtnOptiFineClear.Visibility = Visibility.Collapsed;
                    this.ImgOptiFine.Visibility = Visibility.Collapsed;
                    this.LabOptiFine.Text = text ?? "可以添加";
                    this.LabOptiFine.Foreground = ModSecret.ColorGray4;
                }
                else
                {
                    this.BtnOptiFineClear.Visibility = Visibility.Visible;
                    this.ImgOptiFine.Visibility = Visibility.Visible;
                    this.LabOptiFine.Text = this.SelectedOptiFine.DisplayName.Replace(this.VanillaName + " ", "");
                    this.LabOptiFine.Foreground = ModSecret.ColorGray1;
                }
                if (this.VanillaDrop >= 130)
                {
                    this.CardLiteLoader.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.CardLiteLoader.Visibility = Visibility.Visible;
                    string text2 = this.LoadLiteLoaderGetError();
                    this.CardLiteLoader.MainSwap.Visibility = ((text2 == null) ? Visibility.Visible : Visibility.Collapsed);
                    if (text2 != null)
                    {
                        this.CardLiteLoader.IsSwapped = true;
                    }
                    this.SetPanelVisibility(this.PanLiteLoaderInfo, this.CardLiteLoader.IsSwapped);
                    if (this.SelectedLiteLoader == null)
                    {
                        this.BtnLiteLoaderClear.Visibility = Visibility.Collapsed;
                        this.ImgLiteLoader.Visibility = Visibility.Collapsed;
                        this.LabLiteLoader.Text = text2 ?? "可以添加";
                        this.LabLiteLoader.Foreground = ModSecret.ColorGray4;
                    }
                    else
                    {
                        this.BtnLiteLoaderClear.Visibility = Visibility.Visible;
                        this.ImgLiteLoader.Visibility = Visibility.Visible;
                        this.LabLiteLoader.Text = this.SelectedLiteLoader.Inherit;
                        this.LabLiteLoader.Foreground = ModSecret.ColorGray1;
                    }
                }
                if (!McVersion.IsStandardFormat(this.VanillaName))
                {
                    this.CardForge.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.CardForge.Visibility = Visibility.Visible;
                    string text3 = this.LoadForgeGetError();
                    this.CardForge.MainSwap.Visibility = ((text3 == null) ? Visibility.Visible : Visibility.Collapsed);
                    if (text3 != null)
                    {
                        this.CardForge.IsSwapped = true;
                    }
                    this.SetPanelVisibility(this.PanForgeInfo, this.CardForge.IsSwapped);
                    if (this.SelectedForge == null)
                    {
                        this.BtnForgeClear.Visibility = Visibility.Collapsed;
                        this.ImgForge.Visibility = Visibility.Collapsed;
                        this.LabForge.Text = text3 ?? "可以添加";
                        this.LabForge.Foreground = ModSecret.ColorGray4;
                    }
                    else
                    {
                        this.BtnForgeClear.Visibility = Visibility.Visible;
                        this.ImgForge.Visibility = Visibility.Visible;
                        this.LabForge.Text = this.SelectedForge.VersionName;
                        this.LabForge.Foreground = ModSecret.ColorGray1;
                    }
                }
                if (DateTime.Compare(this.VanillaData["releaseTime"].ToObject<DateTime>(), new DateTime(2023, 6, 11)) < 0)
                {
                    this.CardNeoForge.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.CardNeoForge.Visibility = Visibility.Visible;
                    string text4 = this.LoadNeoForgeGetError();
                    this.CardNeoForge.MainSwap.Visibility = ((text4 == null) ? Visibility.Visible : Visibility.Collapsed);
                    if (text4 != null)
                    {
                        this.CardNeoForge.IsSwapped = true;
                    }
                    this.SetPanelVisibility(this.PanNeoForgeInfo, this.CardNeoForge.IsSwapped);
                    if (this.SelectedNeoForge == null)
                    {
                        this.BtnNeoForgeClear.Visibility = Visibility.Collapsed;
                        this.ImgNeoForge.Visibility = Visibility.Collapsed;
                        this.LabNeoForge.Text = text4 ?? "可以添加";
                        this.LabNeoForge.Foreground = ModSecret.ColorGray4;
                    }
                    else
                    {
                        this.BtnNeoForgeClear.Visibility = Visibility.Visible;
                        this.ImgNeoForge.Visibility = Visibility.Visible;
                        this.LabNeoForge.Text = this.SelectedNeoForge.VersionName;
                        this.LabNeoForge.Foreground = ModSecret.ColorGray1;
                    }
                }
                if (this.VanillaDrop <= 130)
                {
                    this.CardFabric.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.CardFabric.Visibility = Visibility.Visible;
                    string text5 = this.LoadFabricGetError();
                    this.CardFabric.MainSwap.Visibility = ((text5 == null) ? Visibility.Visible : Visibility.Collapsed);
                    if (text5 != null)
                    {
                        this.CardFabric.IsSwapped = true;
                    }
                    this.SetPanelVisibility(this.PanFabricInfo, this.CardFabric.IsSwapped);
                    if (this.SelectedFabric == null)
                    {
                        this.BtnFabricClear.Visibility = Visibility.Collapsed;
                        this.ImgFabric.Visibility = Visibility.Collapsed;
                        this.LabFabric.Text = text5 ?? "可以添加";
                        this.LabFabric.Foreground = ModSecret.ColorGray4;
                    }
                    else
                    {
                        this.BtnFabricClear.Visibility = Visibility.Visible;
                        this.ImgFabric.Visibility = Visibility.Visible;
                        this.LabFabric.Text = this.SelectedFabric.Replace("+build", "");
                        this.LabFabric.Foreground = ModSecret.ColorGray1;
                    }
                }
                if (this.SelectedFabric == null)
                {
                    this.CardFabricApi.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.CardFabricApi.Visibility = Visibility.Visible;
                    string text6 = this.LoadFabricApiGetError();
                    this.CardFabricApi.MainSwap.Visibility = ((text6 == null) ? Visibility.Visible : Visibility.Collapsed);
                    if (text6 != null || this.SelectedFabric == null)
                    {
                        this.CardFabricApi.IsSwapped = true;
                    }
                    this.SetPanelVisibility(this.PanFabricApiInfo, this.CardFabricApi.IsSwapped);
                    if (this.SelectedFabricApi == null)
                    {
                        this.BtnFabricApiClear.Visibility = Visibility.Collapsed;
                        this.ImgFabricApi.Visibility = Visibility.Collapsed;
                        this.LabFabricApi.Text = text6 ?? "可以添加";
                        this.LabFabricApi.Foreground = ModSecret.ColorGray4;
                    }
                    else
                    {
                        this.BtnFabricApiClear.Visibility = Visibility.Visible;
                        this.ImgFabricApi.Visibility = Visibility.Visible;
                        this.LabFabricApi.Text = this.SelectedFabricApi.Display.Split("]", false)[1].Replace("Fabric API ", "").Replace(" build ", ".").Split("+", false)
                            .First<string>()
                            .Trim();
                        this.LabFabricApi.Foreground = ModSecret.ColorGray1;
                    }
                }
                if (this.SelectedFabric == null || this.SelectedOptiFine == null)
                {
                    this.CardOptiFabric.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.CardOptiFabric.Visibility = Visibility.Visible;
                    string text7 = this.LoadOptiFabricGetError();
                    this.CardOptiFabric.MainSwap.Visibility = ((text7 == null) ? Visibility.Visible : Visibility.Collapsed);
                    if (text7 != null || this.SelectedFabric == null)
                    {
                        this.CardOptiFabric.IsSwapped = true;
                    }
                    this.SetPanelVisibility(this.PanOptiFabricInfo, this.CardOptiFabric.IsSwapped);
                    if (this.SelectedOptiFabric == null)
                    {
                        this.BtnOptiFabricClear.Visibility = Visibility.Collapsed;
                        this.ImgOptiFabric.Visibility = Visibility.Collapsed;
                        this.LabOptiFabric.Text = text7 ?? "可以添加";
                        this.LabOptiFabric.Foreground = ModSecret.ColorGray4;
                    }
                    else
                    {
                        this.BtnOptiFabricClear.Visibility = Visibility.Visible;
                        this.ImgOptiFabric.Visibility = Visibility.Visible;
                        this.LabOptiFabric.Text = this.SelectedOptiFabric.Display.Lower().Replace("optifabric-", "").Replace(".jar", "")
                            .Trim()
                            .TrimStart(new char[] { 'v' });
                        this.LabOptiFabric.Foreground = ModSecret.ColorGray1;
                    }
                }
                if (this.SelectedFabric != null && this.SelectedFabricApi == null)
                {
                    this.HintFabricAPI.Visibility = Visibility.Visible;
                }
                else
                {
                    this.HintFabricAPI.Visibility = Visibility.Collapsed;
                }
                if (this.SelectedFabric != null && this.SelectedOptiFine != null && this.SelectedOptiFabric == null)
                {
                    if (this.VanillaDrop >= 140 && this.VanillaDrop <= 150)
                    {
                        this.HintOptiFabric.Visibility = Visibility.Collapsed;
                        this.HintOptiFabricOld.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.HintOptiFabric.Visibility = Visibility.Visible;
                        this.HintOptiFabricOld.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    this.HintOptiFabric.Visibility = Visibility.Collapsed;
                    this.HintOptiFabricOld.Visibility = Visibility.Collapsed;
                }
                if (this.VanillaDrop >= 160 && this.SelectedOptiFine != null && (this.SelectedForge != null || this.SelectedFabric != null))
                {
                    this.HintModOptiFine.Visibility = Visibility.Visible;
                }
                else
                {
                    this.HintModOptiFine.Visibility = Visibility.Collapsed;
                }
                this.$STATIC$ReloadSelected$2001$Ongoing = false;
            }
        }

        public void ClearSelected()
        {
            this.VanillaName = null;
            this.VanillaData = null;
            this.VanillaIcon = null;
            this.SelectedOptiFine = null;
            this.SelectedLiteLoader = null;
            this.SelectedForge = null;
            this.SelectedNeoForge = null;
            this.SelectedFabric = null;
            this.SelectedFabricApi = null;
            this.SelectedOptiFabric = null;
            MyIconButton[] array = new MyIconButton[] { this.BtnOptiFineClear, this.BtnLiteLoaderClear, this.BtnForgeClear, this.BtnNeoForgeClear, this.BtnFabricClear, this.BtnFabricApiClear, this.BtnOptiFabricClear };
            checked
            {
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].Visibility = Visibility.Collapsed;
                }
            }
        }

        private void SetPanelVisibility(Grid Panel, bool IsVisible)
        {
            if (!Operators.ConditionalCompareObjectEqual(Panel.Tag, IsVisible.ToString(), false))
            {
                Panel.Tag = IsVisible.ToString();
                if (IsVisible)
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaTranslateY(Panel, -((TranslateTransform)Panel.RenderTransform).Y, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                        ModAnimation.AaOpacity(Panel, 1.0 - Panel.Opacity, 60, 0, null, false)
                    }, "PageDownloadInstall Visibility " + Panel.Name, false);
                    return;
                }
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaTranslateY(Panel, 6.0 - ((TranslateTransform)Panel.RenderTransform).Y, 60, 0, null, false),
                    ModAnimation.AaOpacity(Panel, -Panel.Opacity, 60, 0, null, false)
                }, "PageDownloadInstall Visibility " + Panel.Name, false);
            }
        }

        public virtual string GetSelectLogo()
        {
            string text;
            if (this.SelectedFabric != null)
            {
                text = "pack://application:,,,/images/Blocks/Fabric.png";
            }
            else if (this.SelectedForge != null)
            {
                text = "pack://application:,,,/images/Blocks/Anvil.png";
            }
            else if (this.SelectedNeoForge != null)
            {
                text = "pack://application:,,,/images/Blocks/NeoForge.png";
            }
            else if (this.SelectedLiteLoader != null)
            {
                text = "pack://application:,,,/images/Blocks/Egg.png";
            }
            else if (this.SelectedOptiFine != null)
            {
                text = "pack://application:,,,/images/Blocks/GrassPath.png";
            }
            else
            {
                text = this.VanillaIcon;
            }
            return text;
        }

        public string GetSelectName()
        {
            string text = this.VanillaName;
            if (this.SelectedFabric != null)
            {
                text = text + "-Fabric " + this.SelectedFabric.BeforeFirst("+", false);
            }
            if (this.SelectedForge != null)
            {
                text = text + "-Forge_" + this.SelectedForge.VersionName;
            }
            if (this.SelectedNeoForge != null)
            {
                text = text + "-NeoForge_" + this.SelectedNeoForge.VersionName.BeforeFirst("+", false);
            }
            if (this.SelectedLiteLoader != null)
            {
                text += "-LiteLoader";
            }
            if (this.SelectedOptiFine != null)
            {
                text = text + "-OptiFine_" + this.SelectedOptiFine.DisplayName.Replace(this.VanillaName + " ", "").Replace(" ", "_");
            }
            return text;
        }

        private void SelectNameUpdate()
        {
            if (!this.IsSelectNameEdited)
            {
                this.TextSelectName.Text = this.GetSelectName();
                this.IsSelectNameEdited = false;
            }
        }

        private void TextSelectName_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IsSelectNameEdited = true;
        }

        private void TextSelectName_ValidateChanged(object sender, EventArgs e)
        {
            this.BtnStart.IsEnabled = this.TextSelectName.IsValidated;
            this.HintSelectName.Text = this.TextSelectName.ValidateResult;
            this.HintSelectName.Visibility = (this.TextSelectName.IsValidated ? Visibility.Collapsed : Visibility.Visible);
        }

        private void LoadMinecraft_OnFinish()
        {
            this.MinecraftVersions = (JArray)ModDownload.DlClientListLoader.Output.Value["versions"].DeepClone();
            this.ShowMinecraftVersionList();
            this.TrySelectWaitingVersion();
        }

        public virtual void ShowMinecraftVersionList()
        {
            this.ExitSelectPage();
            try
            {
                Dictionary<string, List<JObject>> dictionary = new Dictionary<string, List<JObject>>
                {
                    {
                        "正式版",
                        new List<JObject>()
                    },
                    {
                        "预览版",
                        new List<JObject>()
                    },
                    {
                        "远古版",
                        new List<JObject>()
                    },
                    {
                        "愚人节版",
                        new List<JObject>()
                    }
                };
                try
                {
                    foreach (JToken jtoken in this.MinecraftVersions)
                    {
                        JObject jobject = (JObject)jtoken;
                        string text = (string)jobject["type"];
                        if (Operators.CompareString(text, "release", false) != 0)
                        {
                            if (Operators.CompareString(text, "snapshot", false) != 0)
                            {
                                if (Operators.CompareString(text, "special", false) != 0)
                                {
                                    text = "远古版";
                                }
                                else
                                {
                                    text = "愚人节版";
                                }
                            }
                            else
                            {
                                text = "预览版";
                                if (jobject["id"].ToString().StartsWithF("1.", false) && !jobject["id"].ToString().Lower().Contains("combat") && !jobject["id"].ToString().Lower().Contains("rc") && !jobject["id"].ToString().Lower().Contains("experimental") && !jobject["id"].ToString().Lower().Contains("pre"))
                                {
                                    text = "正式版";
                                    jobject["type"] = "release";
                                }
                                string text2 = jobject["id"].ToString().Lower();
                                uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
                                if (num <= 2812350463U)
                                {
                                    if (num <= 1442174922U)
                                    {
                                        if (num != 673058499U)
                                        {
                                            if (num != 1442174922U)
                                            {
                                                goto IL_03F7;
                                            }
                                            if (Operators.CompareString(text2, "26w14a", false) != 0)
                                            {
                                                goto IL_03F7;
                                            }
                                        }
                                        else if (Operators.CompareString(text2, "2.0", false) != 0)
                                        {
                                            goto IL_03F7;
                                        }
                                    }
                                    else if (num != 1946126673U)
                                    {
                                        if (num != 2783260049U)
                                        {
                                            if (num != 2812350463U)
                                            {
                                                goto IL_03F7;
                                            }
                                            if (Operators.CompareString(text2, "23w13a_or_b", false) != 0)
                                            {
                                                goto IL_03F7;
                                            }
                                        }
                                        else if (Operators.CompareString(text2, "1.rv-pre1", false) != 0)
                                        {
                                            goto IL_03F7;
                                        }
                                    }
                                    else if (Operators.CompareString(text2, "25w14craftmine", false) != 0)
                                    {
                                        goto IL_03F7;
                                    }
                                }
                                else if (num <= 3741345573U)
                                {
                                    if (num != 2819146544U)
                                    {
                                        if (num != 3653809737U)
                                        {
                                            if (num != 3741345573U)
                                            {
                                                goto IL_03F7;
                                            }
                                            if (Operators.CompareString(text2, "20w14infinite", false) != 0)
                                            {
                                                goto IL_03F7;
                                            }
                                        }
                                        else if (Operators.CompareString(text2, "20w14∞", false) != 0)
                                        {
                                            goto IL_03F7;
                                        }
                                        text = "愚人节版";
                                        jobject["id"] = "20w14∞";
                                        jobject["type"] = "special";
                                        jobject.Add("lore", ModMinecraft.GetMcFoolName((string)jobject["id"]));
                                        goto IL_0468;
                                    }
                                    if (Operators.CompareString(text2, "15w14a", false) != 0)
                                    {
                                        goto IL_03F7;
                                    }
                                }
                                else if (num != 3766348906U)
                                {
                                    if (num != 3830073160U)
                                    {
                                        if (num != 4285466550U)
                                        {
                                            goto IL_03F7;
                                        }
                                        if (Operators.CompareString(text2, "24w14potato", false) != 0)
                                        {
                                            goto IL_03F7;
                                        }
                                    }
                                    else if (Operators.CompareString(text2, "22w13oneblockatatime", false) != 0)
                                    {
                                        goto IL_03F7;
                                    }
                                }
                                else if (Operators.CompareString(text2, "3d shareware v1.34", false) != 0)
                                {
                                    goto IL_03F7;
                                }
                                text = "愚人节版";
                                jobject["type"] = "special";
                                jobject.Add("lore", ModMinecraft.GetMcFoolName((string)jobject["id"]));
                                goto IL_0468;
                                IL_03F7:
                                DateTime dateTime = Extensions.Value<DateTime>(jobject["releaseTime"]).ToUniversalTime().AddHours(2.0);
                                if (dateTime.Month == 4 && dateTime.Day == 1)
                                {
                                    text = "愚人节版";
                                    jobject["type"] = "special";
                                }
                            }
                        }
                        else
                        {
                            text = "正式版";
                        }
                        IL_0468:
                        dictionary[text].Add(jobject);
                    }
                }
                finally
                {
                    IEnumerator<JToken> enumerator;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                try
                {
                    foreach (KeyValuePair<string, List<JObject>> keyValuePair in dictionary.ToList<KeyValuePair<string, List<JObject>>>())
                    {
                        dictionary[keyValuePair.Key] = keyValuePair.Value.OrderByDescending<JObject, DateTime>((PageDownloadInstall._Closure$__.$I35-0 == null) ? (PageDownloadInstall._Closure$__.$I35-0 = (JObject j) => Extensions.Value<DateTime>(j["releaseTime"])) : PageDownloadInstall._Closure$__.$I35-0).ToList<JObject>();
                    }
                }
                finally
                {
                    List<KeyValuePair<string, List<JObject>>>.Enumerator enumerator2;
                    ((IDisposable)enumerator2).Dispose();
                }
                this.PanMinecraft.Children.Clear();
                MyCard myCard = new MyCard
                {
                    Title = "最新版本",
                    Margin = new Thickness(0.0, 15.0, 0.0, 15.0)
                };
                List<JObject> list = new List<JObject>();
                JObject jobject2 = (JObject)dictionary["正式版"][0].DeepClone();
                jobject2["lore"] = "最新正式版，发布于 " + Extensions.Value<DateTime>(jobject2["releaseTime"]).ToString("yyyy'/'MM'/'dd HH':'mm");
                list.Add(jobject2);
                if (DateTime.Compare(Extensions.Value<DateTime>(dictionary["正式版"][0]["releaseTime"]), Extensions.Value<DateTime>(dictionary["预览版"][0]["releaseTime"])) < 0)
                {
                    JObject jobject3 = (JObject)dictionary["预览版"][0].DeepClone();
                    jobject3["lore"] = "最新预览版，发布于 " + Extensions.Value<DateTime>(jobject3["releaseTime"]).ToString("yyyy'/'MM'/'dd HH':'mm");
                    list.Add(jobject3);
                }
                StackPanel stackPanel = new StackPanel
                {
                    Margin = new Thickness(20.0, 40.0, 18.0, 0.0),
                    VerticalAlignment = VerticalAlignment.Top,
                    RenderTransform = new TranslateTransform(0.0, 0.0)
                };
                myCard.Children.Add(stackPanel);
                myCard.SwapControl = stackPanel;
                myCard.SetContentBuilder<JObject>(list, (JObject Version) => ModDownloadLib.McDownloadListItem(Version, delegate(object sender, MouseButtonEventArgs e)
                {
                    this.MinecraftSelected((MyListItem)sender, e);
                }, false));
                myCard.BuildContent();
                this.PanMinecraft.Children.Insert(0, myCard);
                try
                {
                    foreach (KeyValuePair<string, List<JObject>> keyValuePair2 in dictionary)
                    {
                        if (keyValuePair2.Value.Any<JObject>())
                        {
                            MyCard myCard2 = new MyCard
                            {
                                Title = keyValuePair2.Key,
                                Margin = new Thickness(0.0, 0.0, 0.0, 15.0)
                            };
                            StackPanel stackPanel2 = new StackPanel
                            {
                                Margin = new Thickness(20.0, 40.0, 18.0, 0.0),
                                VerticalAlignment = VerticalAlignment.Top,
                                RenderTransform = new TranslateTransform(0.0, 0.0)
                            };
                            myCard2.Children.Add(stackPanel2);
                            myCard2.SwapControl = stackPanel2;
                            myCard2.SetContentBuilder<JObject>(keyValuePair2.Value, (JObject Version) => ModDownloadLib.McDownloadListItem(Version, delegate(object sender, MouseButtonEventArgs e)
                            {
                                this.MinecraftSelected((MyListItem)sender, e);
                            }, false));
                            myCard2.IsSwapped = true;
                            this.PanMinecraft.Children.Add(myCard2);
                        }
                    }
                }
                finally
                {
                    Dictionary<string, List<JObject>>.Enumerator enumerator3;
                    ((IDisposable)enumerator3).Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "可视化安装版本列表出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadInstall.xaml.vb");
            }
        }

        private void TrySelectWaitingVersion()
        {
            if (PageDownloadInstall.VersionWaitingSelect != null)
            {
                try
                {
                    foreach (JToken jtoken in this.MinecraftVersions)
                    {
                        JObject jobject = (JObject)jtoken;
                        if (Operators.CompareString(jobject["id"].ToString(), PageDownloadInstall.VersionWaitingSelect, false) == 0)
                        {
                            Logger.Info(string.Format("自动选择 MC 版本：{0}", PageDownloadInstall.VersionWaitingSelect), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadInstall.xaml.vb");
                            this.MinecraftSelected(ModDownloadLib.McDownloadListItem(jobject, null, false).Init(), null);
                            PageDownloadInstall.VersionWaitingSelect = null;
                            break;
                        }
                    }
                }
                finally
                {
                    IEnumerator<JToken> enumerator;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
            }
        }

        public string LoadOptiFineGetError()
        {
            string text;
            if (this.SelectedNeoForge != null)
            {
                text = "与 NeoForge 不兼容";
            }
            else if (this.SelectedForge != null && McUtils.CompareVersionGE(this.VanillaName, "1.13") && McUtils.CompareVersionGE("1.14.3", this.VanillaName))
            {
                text = "与 Forge 不兼容";
            }
            else if (this.SelectedFabric != null && McUtils.CompareVersion(this.VanillaName, "1.20.4") > 0)
            {
                text = "与 Fabric 不兼容";
            }
            else if (this.GetLoaderError(this.LoadOptiFine) != null)
            {
                text = this.GetLoaderError(this.LoadOptiFine);
            }
            else
            {
                bool flag = false;
                bool flag2 = false;
                try
                {
                    foreach (ModDownload.DlOptiFineListEntry dlOptiFineListEntry in ModDownload.DlOptiFineListLoader.Output.Value)
                    {
                        if (dlOptiFineListEntry.DisplayName.StartsWithF(this.VanillaName + " ", false))
                        {
                            flag = true;
                            if (this.SelectedForge == null)
                            {
                                return null;
                            }
                            if (PageDownloadInstall.IsOptiFineSuitForForge(dlOptiFineListEntry, this.SelectedForge))
                            {
                                return null;
                            }
                            if (dlOptiFineListEntry.RequiredForgeVersion != null)
                            {
                                flag2 = true;
                            }
                        }
                    }
                }
                finally
                {
                    List<ModDownload.DlOptiFineListEntry>.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
                if (!flag)
                {
                    text = "无";
                }
                else if (flag2)
                {
                    text = "仅兼容特定版本的 Forge";
                }
                else
                {
                    text = "与 Forge 不兼容";
                }
            }
            return text;
        }

        public static bool IsOptiFineSuitForForge(ModDownload.DlOptiFineListEntry OptiFine, ModDownload.DlForgeVersionEntry Forge)
        {
            bool flag;
            if (Operators.CompareString(Forge.Inherit, OptiFine.Inherit, false) != 0)
            {
                flag = false;
            }
            else if (OptiFine.RequiredForgeVersion == null)
            {
                flag = false;
            }
            else if (string.IsNullOrWhiteSpace(OptiFine.RequiredForgeVersion))
            {
                flag = true;
            }
            else if (OptiFine.RequiredForgeVersion.Contains("."))
            {
                flag = McUtils.CompareVersion(Forge.Version.ToString(), OptiFine.RequiredForgeVersion) == 0;
            }
            else
            {
                flag = (double)Forge.Version.Revision == Conversions.ToDouble(OptiFine.RequiredForgeVersion);
            }
            return flag;
        }

        private void CardOptiFine_PreviewSwap(object sender, ModBase.RouteEventArgs e)
        {
            if (this.LoadOptiFineGetError() != null)
            {
                e.Handled = true;
            }
        }

        private void OptiFine_Loaded()
        {
            try
            {
                if (ModDownload.DlOptiFineListLoader.State == ModBase.LoadState.Finished)
                {
                    List<ModDownload.DlOptiFineListEntry> list = new List<ModDownload.DlOptiFineListEntry>();
                    try
                    {
                        foreach (ModDownload.DlOptiFineListEntry dlOptiFineListEntry in ModDownload.DlOptiFineListLoader.Output.Value)
                        {
                            if ((this.SelectedForge == null || PageDownloadInstall.IsOptiFineSuitForForge(dlOptiFineListEntry, this.SelectedForge)) && dlOptiFineListEntry.DisplayName.StartsWithF(this.VanillaName + " ", false))
                            {
                                list.Add(dlOptiFineListEntry);
                            }
                        }
                    }
                    finally
                    {
                        List<ModDownload.DlOptiFineListEntry>.Enumerator enumerator;
                        ((IDisposable)enumerator).Dispose();
                    }
                    if (list.Any<ModDownload.DlOptiFineListEntry>())
                    {
                        list = list.SortByComparison<ModDownload.DlOptiFineListEntry>((PageDownloadInstall._Closure$__.$I41-0 == null) ? (PageDownloadInstall._Closure$__.$I41-0 = (ModDownload.DlOptiFineListEntry Left, ModDownload.DlOptiFineListEntry Right) => (!Left.IsPreview && Right.IsPreview) || ((!Left.IsPreview || Right.IsPreview) && McUtils.CompareVersionGE(Left.DisplayName, Right.DisplayName))) : PageDownloadInstall._Closure$__.$I41-0);
                        this.PanOptiFine.Children.Clear();
                        try
                        {
                            foreach (ModDownload.DlOptiFineListEntry dlOptiFineListEntry2 in list)
                            {
                                this.PanOptiFine.Children.Add(ModDownloadLib.OptiFineDownloadListItem(dlOptiFineListEntry2, delegate(object a0, MouseButtonEventArgs a1)
                                {
                                    this.OptiFine_Selected((MyListItem)a0, a1);
                                }, false));
                            }
                        }
                        finally
                        {
                            List<ModDownload.DlOptiFineListEntry>.Enumerator enumerator2;
                            ((IDisposable)enumerator2).Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "可视化 OptiFine 安装版本列表出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadInstall.xaml.vb");
            }
        }

        private void OptiFine_Selected(MyListItem sender, EventArgs e)
        {
            this.SelectedOptiFine = (ModDownload.DlOptiFineListEntry)sender.Tag;
            if (this.SelectedForge != null && !PageDownloadInstall.IsOptiFineSuitForForge(this.SelectedOptiFine, this.SelectedForge))
            {
                this.SelectedForge = null;
            }
            this.OptiFabric_Loaded();
            this.Forge_Loaded();
            this.NeoForge_Loaded();
            this.CardOptiFine.IsSwapped = true;
            this.ReloadSelected();
        }

        private void OptiFine_Clear(object sender, EventArgs e)
        {
            this.SelectedOptiFine = null;
            this.SelectedOptiFabric = null;
            this.BtnOptiFabricClear.Visibility = Visibility.Collapsed;
            this.AutoSelectedOptiFabric = false;
            this.CardOptiFine.IsSwapped = true;
            this.Forge_Loaded();
            this.NeoForge_Loaded();
            this.ReloadSelected();
        }

        public string LoadLiteLoaderGetError()
        {
            string text;
            if (this.GetLoaderError(this.LoadLiteLoader) != null)
            {
                text = this.GetLoaderError(this.LoadLiteLoader);
            }
            else
            {
                text = (ModDownload.DlLiteLoaderListLoader.Output.Value.Any<ModDownload.DlLiteLoaderListEntry>((ModDownload.DlLiteLoaderListEntry v) => Operators.CompareString(v.Inherit, this.VanillaName, false) == 0) ? null : "无");
            }
            return text;
        }

        private void CardLiteLoader_PreviewSwap(object sender, ModBase.RouteEventArgs e)
        {
            if (this.LoadLiteLoaderGetError() != null)
            {
                e.Handled = true;
            }
        }

        private void LiteLoader_Loaded()
        {
            try
            {
                if (ModDownload.DlLiteLoaderListLoader.State == ModBase.LoadState.Finished)
                {
                    List<ModDownload.DlLiteLoaderListEntry> list = new List<ModDownload.DlLiteLoaderListEntry>();
                    try
                    {
                        foreach (ModDownload.DlLiteLoaderListEntry dlLiteLoaderListEntry in ModDownload.DlLiteLoaderListLoader.Output.Value)
                        {
                            if (Operators.CompareString(dlLiteLoaderListEntry.Inherit, this.VanillaName, false) == 0)
                            {
                                list.Add(dlLiteLoaderListEntry);
                            }
                        }
                    }
                    finally
                    {
                        List<ModDownload.DlLiteLoaderListEntry>.Enumerator enumerator;
                        ((IDisposable)enumerator).Dispose();
                    }
                    if (list.Any<ModDownload.DlLiteLoaderListEntry>())
                    {
                        this.PanLiteLoader.Children.Clear();
                        try
                        {
                            foreach (ModDownload.DlLiteLoaderListEntry dlLiteLoaderListEntry2 in list)
                            {
                                this.PanLiteLoader.Children.Add(ModDownloadLib.LiteLoaderDownloadListItem(dlLiteLoaderListEntry2, delegate(object a0, MouseButtonEventArgs a1)
                                {
                                    this.LiteLoader_Selected((MyListItem)a0, a1);
                                }, false));
                            }
                        }
                        finally
                        {
                            List<ModDownload.DlLiteLoaderListEntry>.Enumerator enumerator2;
                            ((IDisposable)enumerator2).Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "可视化 LiteLoader 安装版本列表出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadInstall.xaml.vb");
            }
        }

        private void LiteLoader_Selected(MyListItem sender, EventArgs e)
        {
            this.SelectedLiteLoader = (ModDownload.DlLiteLoaderListEntry)sender.Tag;
            this.CardLiteLoader.IsSwapped = true;
            this.ReloadSelected();
        }

        private void LiteLoader_Clear(object sender, EventArgs e)
        {
            this.SelectedLiteLoader = null;
            this.CardLiteLoader.IsSwapped = true;
            this.ReloadSelected();
        }

        public string LoadForgeGetError()
        {
            string text;
            if (McUtils.CompareVersionGE("1.5.1", this.VanillaName) && McUtils.CompareVersionGE(this.VanillaName, "1.1"))
            {
                text = "无";
            }
            else if (this.GetLoaderError(this.LoadForge) != null)
            {
                text = this.GetLoaderError(this.LoadForge);
            }
            else
            {
                ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>> loaderTask = (ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>)this.LoadForge.State;
                if (Operators.CompareString(this.VanillaName, loaderTask.Input, false) != 0)
                {
                    text = "获取中……";
                }
                else
                {
                    try
                    {
                        foreach (ModDownload.DlForgeVersionEntry dlForgeVersionEntry in loaderTask.Output)
                        {
                            if (Operators.CompareString(dlForgeVersionEntry.Category, "universal", false) != 0 && Operators.CompareString(dlForgeVersionEntry.Category, "client", false) != 0)
                            {
                                if (this.SelectedNeoForge != null)
                                {
                                    return "与 NeoForge 不兼容";
                                }
                                if (this.SelectedFabric != null)
                                {
                                    return "与 Fabric 不兼容";
                                }
                                if (this.SelectedOptiFine != null && McUtils.CompareVersionGE(this.VanillaName, "1.13") && McUtils.CompareVersionGE("1.14.3", this.VanillaName))
                                {
                                    return "与 OptiFine 不兼容";
                                }
                                if (this.SelectedOptiFine == null || PageDownloadInstall.IsOptiFineSuitForForge(this.SelectedOptiFine, dlForgeVersionEntry))
                                {
                                    return null;
                                }
                            }
                        }
                    }
                    finally
                    {
                        List<ModDownload.DlForgeVersionEntry>.Enumerator enumerator;
                        ((IDisposable)enumerator).Dispose();
                    }
                    text = "与 OptiFine 不兼容";
                }
            }
            return text;
        }

        private void CardForge_PreviewSwap(object sender, ModBase.RouteEventArgs e)
        {
            if (this.LoadForgeGetError() != null)
            {
                e.Handled = true;
            }
        }

        private void Forge_Loaded()
        {
            try
            {
                if (this.LoadForge.State.IsLoader)
                {
                    ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>> loaderTask = (ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>)this.LoadForge.State;
                    if (Operators.CompareString(this.VanillaName, loaderTask.Input, false) == 0)
                    {
                        if (loaderTask.State == ModBase.LoadState.Finished)
                        {
                            List<ModDownload.DlForgeVersionEntry> list = loaderTask.Output.ToList<ModDownload.DlForgeVersionEntry>();
                            if (loaderTask.Output.Any<ModDownload.DlForgeVersionEntry>())
                            {
                                this.PanForge.Children.Clear();
                                list = list.Where<ModDownload.DlForgeVersionEntry>((ModDownload.DlForgeVersionEntry v) => Operators.CompareString(v.Category, "universal", false) != 0 && Operators.CompareString(v.Category, "client", false) != 0 && (this.SelectedOptiFine == null || PageDownloadInstall.IsOptiFineSuitForForge(this.SelectedOptiFine, v))).OrderByDescending<ModDownload.DlForgeVersionEntry, ModDownload.DlForgeVersionEntry>((PageDownloadInstall._Closure$__.$I51-1 == null) ? (PageDownloadInstall._Closure$__.$I51-1 = (ModDownload.DlForgeVersionEntry v) => v) : PageDownloadInstall._Closure$__.$I51-1).ToList<ModDownload.DlForgeVersionEntry>();
                                ModDownloadLib.ForgeDownloadListItemPreload(this.PanForge, list, delegate(object a0, MouseButtonEventArgs a1)
                                {
                                    this.Forge_Selected((MyListItem)a0, a1);
                                }, false);
                                try
                                {
                                    foreach (ModDownload.DlForgeVersionEntry dlForgeVersionEntry in list)
                                    {
                                        this.PanForge.Children.Add(ModDownloadLib.ForgeDownloadListItem(dlForgeVersionEntry, delegate(object a0, MouseButtonEventArgs a1)
                                        {
                                            this.Forge_Selected((MyListItem)a0, a1);
                                        }, false));
                                    }
                                }
                                finally
                                {
                                    List<ModDownload.DlForgeVersionEntry>.Enumerator enumerator;
                                    ((IDisposable)enumerator).Dispose();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "可视化 Forge 安装版本列表出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadInstall.xaml.vb");
            }
        }

        private void Forge_Selected(MyListItem sender, EventArgs e)
        {
            this.SelectedForge = (ModDownload.DlForgeVersionEntry)sender.Tag;
            this.CardForge.IsSwapped = true;
            if (this.SelectedOptiFine != null && !PageDownloadInstall.IsOptiFineSuitForForge(this.SelectedOptiFine, this.SelectedForge))
            {
                this.SelectedOptiFine = null;
            }
            this.OptiFine_Loaded();
            this.ReloadSelected();
        }

        private void Forge_Clear(object sender, EventArgs e)
        {
            this.SelectedForge = null;
            this.CardForge.IsSwapped = true;
            this.OptiFine_Loaded();
            this.ReloadSelected();
        }

        public string LoadNeoForgeGetError()
        {
            string text;
            if (this.SelectedOptiFine != null)
            {
                text = "与 OptiFine 不兼容";
            }
            else if (this.SelectedForge != null)
            {
                text = "与 Forge 不兼容";
            }
            else if (this.SelectedFabric != null)
            {
                text = "与 Fabric 不兼容";
            }
            else if (this.GetLoaderError(this.LoadNeoForge) != null)
            {
                text = this.GetLoaderError(this.LoadNeoForge);
            }
            else
            {
                text = (ModDownload.DlNeoForgeListLoader.Output.Value.Any<ModDownload.DlNeoForgeListEntry>((ModDownload.DlNeoForgeListEntry v) => Operators.CompareString(v.Inherit, this.VanillaName, false) == 0) ? null : "无");
            }
            return text;
        }

        private void CardNeoForge_PreviewSwap(object sender, ModBase.RouteEventArgs e)
        {
            if (this.LoadNeoForgeGetError() != null)
            {
                e.Handled = true;
            }
        }

        private void NeoForge_Loaded()
        {
            try
            {
                if (ModDownload.DlNeoForgeListLoader.State == ModBase.LoadState.Finished)
                {
                    List<ModDownload.DlNeoForgeListEntry> list = ModDownload.DlNeoForgeListLoader.Output.Value.Where<ModDownload.DlNeoForgeListEntry>((ModDownload.DlNeoForgeListEntry v) => Operators.CompareString(v.Inherit, this.VanillaName, false) == 0).ToList<ModDownload.DlNeoForgeListEntry>();
                    if (list.Any<ModDownload.DlNeoForgeListEntry>())
                    {
                        this.PanNeoForge.Children.Clear();
                        ModDownloadLib.NeoForgeDownloadListItemPreload(this.PanNeoForge, list, delegate(object a0, MouseButtonEventArgs a1)
                        {
                            this.NeoForge_Selected((MyListItem)a0, a1);
                        }, false);
                        try
                        {
                            foreach (ModDownload.DlNeoForgeListEntry dlNeoForgeListEntry in list)
                            {
                                this.PanNeoForge.Children.Add(ModDownloadLib.NeoForgeDownloadListItem(dlNeoForgeListEntry, delegate(object a0, MouseButtonEventArgs a1)
                                {
                                    this.NeoForge_Selected((MyListItem)a0, a1);
                                }, false));
                            }
                        }
                        finally
                        {
                            List<ModDownload.DlNeoForgeListEntry>.Enumerator enumerator;
                            ((IDisposable)enumerator).Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "可视化 NeoForge 安装版本列表出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadInstall.xaml.vb");
            }
        }

        private void NeoForge_Selected(MyListItem sender, EventArgs e)
        {
            this.SelectedNeoForge = (ModDownload.DlNeoForgeListEntry)sender.Tag;
            this.CardNeoForge.IsSwapped = true;
            this.OptiFine_Loaded();
            this.ReloadSelected();
        }

        private void NeoForge_Clear(object sender, EventArgs e)
        {
            this.SelectedNeoForge = null;
            this.CardNeoForge.IsSwapped = true;
            this.OptiFine_Loaded();
            this.ReloadSelected();
        }

        public string LoadFabricGetError()
        {
            string text;
            if (this.SelectedOptiFine != null && McUtils.CompareVersionGE(this.VanillaName, "1.20.5"))
            {
                text = "与 OptiFine 不兼容";
            }
            else if (this.GetLoaderError(this.LoadFabric) != null)
            {
                text = this.GetLoaderError(this.LoadFabric);
            }
            else
            {
                try
                {
                    IEnumerator<JToken> enumerator = ModDownload.DlFabricListLoader.Output.Value["game"].GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (Operators.CompareString(((JObject)enumerator.Current)["version"].ToString(), this.VanillaName.Replace("∞", "infinite").Replace("Combat Test 7c", "1.16_combat-3"), false) == 0)
                        {
                            if (this.SelectedForge != null)
                            {
                                return "与 Forge 不兼容";
                            }
                            if (this.SelectedNeoForge != null)
                            {
                                return "与 NeoForge 不兼容";
                            }
                            return null;
                        }
                    }
                }
                finally
                {
                    IEnumerator<JToken> enumerator;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                text = "无";
            }
            return text;
        }

        private void CardFabric_PreviewSwap(object sender, ModBase.RouteEventArgs e)
        {
            if (this.LoadFabricGetError() != null)
            {
                e.Handled = true;
            }
        }

        private void Fabric_Loaded()
        {
            try
            {
                if (ModDownload.DlFabricListLoader.State == ModBase.LoadState.Finished)
                {
                    JArray jarray = (JArray)ModDownload.DlFabricListLoader.Output.Value["loader"];
                    if (jarray.Any<JToken>())
                    {
                        this.PanFabric.Children.Clear();
                        this.CardFabric.SwapControl = this.PanFabric;
                        this.CardFabric.SetContentBuilder<JObject>(jarray.OfType<JObject>(), (JObject Version) => ModDownloadLib.FabricDownloadListItem(Version, delegate(object a0, MouseButtonEventArgs a1)
                        {
                            this.Fabric_Selected((MyListItem)a0, a1);
                        }));
                        if (!this.CardFabric.IsSwapped)
                        {
                            this.CardFabric.BuildContent();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "可视化 Fabric 安装版本列表出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadInstall.xaml.vb");
            }
        }

        public void Fabric_Selected(MyListItem sender, EventArgs e)
        {
            this.SelectedFabric = NewLateBinding.LateIndexGet(sender.Tag, new object[] { "version" }, null).ToString();
            this.FabricApi_Loaded();
            this.OptiFabric_Loaded();
            this.CardFabric.IsSwapped = true;
            this.ReloadSelected();
        }

        private void Fabric_Clear(object sender, EventArgs e)
        {
            this.SelectedFabric = null;
            this.SelectedFabricApi = null;
            this.BtnFabricApiClear.Visibility = Visibility.Collapsed;
            this.AutoSelectedFabricApi = false;
            this.SelectedOptiFabric = null;
            this.BtnOptiFabricClear.Visibility = Visibility.Collapsed;
            this.AutoSelectedOptiFabric = false;
            this.CardFabric.IsSwapped = true;
            this.ReloadSelected();
        }

        public static bool IsFabricApiCompatible(ResourceVersion FabricApi, string MinecraftVersion)
        {
            string text = FabricApi.Display;
            checked
            {
                bool flag;
                try
                {
                    if (text == null || MinecraftVersion == null)
                    {
                        flag = false;
                    }
                    else
                    {
                        text = text.Lower();
                        string text2 = MinecraftVersion.Replace("∞", "infinite").Replace("Combat Test 7c", "1.16_combat-3").Lower();
                        if (text.StartsWithF("[" + text2 + "]", false))
                        {
                            flag = true;
                        }
                        else if (!text.Contains("/") || !text.Contains("]"))
                        {
                            flag = false;
                        }
                        else
                        {
                            string[] array = text.BeforeFirst("]", false).TrimStart(new char[] { '[' }).Split("/", false);
                            for (int i = 0; i < array.Length; i++)
                            {
                                if (Operators.CompareString(array[i], text2, false) == 0)
                                {
                                    return true;
                                }
                            }
                            List<string> list = text.BeforeFirst("]", false).RegexSearch("[a-z/]+|[0-9/]+", RegexOptions.None).ToList<string>();
                            List<string> list2 = text2.BeforeFirst("]", false).RegexSearch("[a-z/]+|[0-9/]+", RegexOptions.None).ToList<string>();
                            int num = 0;
                            while (list.Count - 1 >= num || list2.Count - 1 >= num)
                            {
                                string text3 = ((list.Count - 1 < num) ? "-1" : list[num]);
                                string text4 = ((list2.Count - 1 < num) ? "-1" : list2[num]);
                                if (!text3.Contains("/"))
                                {
                                    if (Operators.CompareString(text3, text4, false) != 0)
                                    {
                                        return false;
                                    }
                                }
                                else if (!text3.Contains(text4))
                                {
                                    return false;
                                }
                                num++;
                            }
                            flag = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, string.Format("判断 Fabric API 版本适配性出错（{0}, {1}）", text, MinecraftVersion), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadInstall.xaml.vb");
                    flag = false;
                }
                return flag;
            }
        }

        public string LoadFabricApiGetError()
        {
            string text;
            if (this.GetLoaderError(this.LoadFabricApi) != null)
            {
                text = this.GetLoaderError(this.LoadFabricApi);
            }
            else if (ModDownload.DlFabricApiLoader.Output == null)
            {
                text = ((this.SelectedFabric == null) ? "需要安装 Fabric" : "获取中……");
            }
            else if (ModDownload.DlFabricApiLoader.Output.Any<ResourceVersion>((ResourceVersion f) => PageDownloadInstall.IsFabricApiCompatible(f, this.VanillaName)))
            {
                text = ((this.SelectedFabric == null) ? "需要安装 Fabric" : null);
            }
            else
            {
                text = "无";
            }
            return text;
        }

        private void CardFabricApi_PreviewSwap(object sender, ModBase.RouteEventArgs e)
        {
            if (this.LoadFabricApiGetError() != null)
            {
                e.Handled = true;
            }
        }

        private void FabricApi_Loaded()
        {
            try
            {
                if (ModDownload.DlFabricApiLoader.State == ModBase.LoadState.Finished)
                {
                    if (this.VanillaName != null && this.SelectedFabric != null)
                    {
                        List<ResourceVersion> list = new List<ResourceVersion>();
                        try
                        {
                            foreach (ResourceVersion resourceVersion in ModDownload.DlFabricApiLoader.Output)
                            {
                                if (PageDownloadInstall.IsFabricApiCompatible(resourceVersion, this.VanillaName))
                                {
                                    if (!resourceVersion.Display.StartsWithF("[", false))
                                    {
                                        Logger.Warn(string.Format("已特判修改 Fabric API 显示名：{0}", resourceVersion.Display), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadInstall.xaml.vb");
                                        resourceVersion.Display = "[" + this.VanillaName + "] " + resourceVersion.Display;
                                    }
                                    list.Add(resourceVersion);
                                }
                            }
                        }
                        finally
                        {
                            List<ResourceVersion>.Enumerator enumerator;
                            ((IDisposable)enumerator).Dispose();
                        }
                        if (list.Any<ResourceVersion>())
                        {
                            list = list.OrderByDescending<ResourceVersion, DateTime>((PageDownloadInstall._Closure$__.$I68-0 == null) ? (PageDownloadInstall._Closure$__.$I68-0 = (ResourceVersion v) => v.ReleaseDate) : PageDownloadInstall._Closure$__.$I68-0).ToList<ResourceVersion>();
                            this.PanFabricApi.Children.Clear();
                            try
                            {
                                foreach (ResourceVersion resourceVersion2 in list)
                                {
                                    this.PanFabricApi.Children.Add(ModDownloadLib.FabricApiDownloadListItem(resourceVersion2, delegate(object a0, MouseButtonEventArgs a1)
                                    {
                                        this.FabricApi_Selected((MyListItem)a0, a1);
                                    }));
                                }
                            }
                            finally
                            {
                                List<ResourceVersion>.Enumerator enumerator2;
                                ((IDisposable)enumerator2).Dispose();
                            }
                            if (!this.AutoSelectedFabricApi)
                            {
                                this.AutoSelectedFabricApi = true;
                                MyListItem myListItem = (MyListItem)MyVirtualizingElement.TryInit((FrameworkElement)this.PanFabricApi.Children[0]);
                                Logger.Info(string.Format("已自动选择 Fabric API：{0}", myListItem.Title), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadInstall.xaml.vb");
                                this.FabricApi_Selected(myListItem, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "可视化 Fabric API 安装版本列表出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadInstall.xaml.vb");
            }
        }

        private void FabricApi_Selected(MyListItem sender, EventArgs e)
        {
            this.SelectedFabricApi = (ResourceVersion)sender.Tag;
            this.CardFabricApi.IsSwapped = true;
            this.ReloadSelected();
        }

        private void FabricApi_Clear(object sender, EventArgs e)
        {
            this.SelectedFabricApi = null;
            this.CardFabricApi.IsSwapped = true;
            this.ReloadSelected();
        }

        public static bool IsOptiFabricCompatible(ResourceVersion ModFile, string MinecraftVersion)
        {
            bool flag;
            try
            {
                if (MinecraftVersion == null)
                {
                    flag = false;
                }
                else
                {
                    flag = ModFile.GameVersions.Contains(MinecraftVersion);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, string.Format("判断 OptiFabric 版本适配性出错（{0}）", MinecraftVersion), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadInstall.xaml.vb");
                flag = false;
            }
            return flag;
        }

        public string LoadOptiFabricGetError()
        {
            string text;
            if (this.VanillaDrop >= 140 && this.VanillaDrop <= 150)
            {
                text = "不兼容老版本 Fabric，请手动下载 OptiFabric Origins";
            }
            else if (this.GetLoaderError(this.LoadOptiFabric) != null)
            {
                text = this.GetLoaderError(this.LoadOptiFabric);
            }
            else if (ModDownload.DlOptiFabricLoader.Output == null)
            {
                if (this.SelectedFabric == null && this.SelectedOptiFine == null)
                {
                    text = "需要安装 OptiFine 与 Fabric";
                }
                else if (this.SelectedFabric == null)
                {
                    text = "需要安装 Fabric";
                }
                else if (this.SelectedOptiFine == null)
                {
                    text = "需要安装 OptiFine";
                }
                else
                {
                    text = "获取中……";
                }
            }
            else
            {
                try
                {
                    List<ResourceVersion>.Enumerator enumerator = ModDownload.DlOptiFabricLoader.Output.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (PageDownloadInstall.IsOptiFabricCompatible(enumerator.Current, this.VanillaName))
                        {
                            if (this.SelectedFabric == null && this.SelectedOptiFine == null)
                            {
                                return "需要安装 OptiFine 与 Fabric";
                            }
                            if (this.SelectedFabric == null)
                            {
                                return "需要安装 Fabric";
                            }
                            if (this.SelectedOptiFine == null)
                            {
                                return "需要安装 OptiFine";
                            }
                            return null;
                        }
                    }
                }
                finally
                {
                    List<ResourceVersion>.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
                text = "无";
            }
            return text;
        }

        private void CardOptiFabric_PreviewSwap(object sender, ModBase.RouteEventArgs e)
        {
            if (this.LoadOptiFabricGetError() != null)
            {
                e.Handled = true;
            }
        }

        private void OptiFabric_Loaded()
        {
            try
            {
                if (ModDownload.DlOptiFabricLoader.State == ModBase.LoadState.Finished)
                {
                    if (this.VanillaName != null && this.SelectedFabric != null && this.SelectedOptiFine != null)
                    {
                        List<ResourceVersion> list = new List<ResourceVersion>();
                        try
                        {
                            foreach (ResourceVersion resourceVersion in ModDownload.DlOptiFabricLoader.Output)
                            {
                                if (PageDownloadInstall.IsOptiFabricCompatible(resourceVersion, this.VanillaName))
                                {
                                    list.Add(resourceVersion);
                                }
                            }
                        }
                        finally
                        {
                            List<ResourceVersion>.Enumerator enumerator;
                            ((IDisposable)enumerator).Dispose();
                        }
                        if (list.Any<ResourceVersion>())
                        {
                            list = list.OrderByDescending<ResourceVersion, DateTime>((PageDownloadInstall._Closure$__.$I75-0 == null) ? (PageDownloadInstall._Closure$__.$I75-0 = (ResourceVersion v) => v.ReleaseDate) : PageDownloadInstall._Closure$__.$I75-0).ToList<ResourceVersion>();
                            this.PanOptiFabric.Children.Clear();
                            try
                            {
                                foreach (ResourceVersion resourceVersion2 in list)
                                {
                                    this.PanOptiFabric.Children.Add(ModDownloadLib.OptiFabricDownloadListItem(resourceVersion2, delegate(object a0, MouseButtonEventArgs a1)
                                    {
                                        this.OptiFabric_Selected((MyListItem)a0, a1);
                                    }));
                                }
                            }
                            finally
                            {
                                List<ResourceVersion>.Enumerator enumerator2;
                                ((IDisposable)enumerator2).Dispose();
                            }
                            if (!this.AutoSelectedOptiFabric && (this.VanillaDrop < 140 || this.VanillaDrop > 150))
                            {
                                this.AutoSelectedOptiFabric = true;
                                MyListItem myListItem = (MyListItem)MyVirtualizingElement.TryInit((FrameworkElement)this.PanOptiFabric.Children[0]);
                                Logger.Info(string.Format("已自动选择 OptiFabric：{0}", myListItem.Title), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadInstall.xaml.vb");
                                this.OptiFabric_Selected(myListItem, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "可视化 OptiFabric 安装版本列表出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadInstall.xaml.vb");
            }
        }

        private void OptiFabric_Selected(MyListItem sender, EventArgs e)
        {
            this.SelectedOptiFabric = (ResourceVersion)sender.Tag;
            this.CardOptiFabric.IsSwapped = true;
            this.ReloadSelected();
        }

        private void OptiFabric_Clear(object sender, EventArgs e)
        {
            this.SelectedOptiFabric = null;
            this.CardOptiFabric.IsSwapped = true;
            this.ReloadSelected();
        }

        public McComponents GetSelectedComponents()
        {
            string vanillaName = this.VanillaName;
            JObject jobject = ((this.SelectedOptiFine == null) ? null : JObject.FromObject(this.SelectedOptiFine));
            ModDownload.DlForgeVersionEntry selectedForge = this.SelectedForge;
            string text = ((selectedForge != null) ? selectedForge.VersionName : null);
            ModDownload.DlNeoForgeListEntry selectedNeoForge = this.SelectedNeoForge;
            string text2 = ((selectedNeoForge != null) ? selectedNeoForge.VersionName : null);
            string selectedFabric = this.SelectedFabric;
            ResourceVersion selectedFabricApi = this.SelectedFabricApi;
            string text3;
            if ((text3 = ((selectedFabricApi != null) ? selectedFabricApi.Version : null)) == null)
            {
                ResourceVersion selectedFabricApi2 = this.SelectedFabricApi;
                if ((text3 = ((selectedFabricApi2 != null) ? selectedFabricApi2.Id : null)) == null)
                {
                    ResourceVersion selectedFabricApi3 = this.SelectedFabricApi;
                    text3 = ((selectedFabricApi3 != null) ? selectedFabricApi3.Display : null);
                }
            }
            ResourceVersion selectedOptiFabric = this.SelectedOptiFabric;
            string text4;
            if ((text4 = ((selectedOptiFabric != null) ? selectedOptiFabric.Version : null)) == null)
            {
                ResourceVersion selectedOptiFabric2 = this.SelectedOptiFabric;
                if ((text4 = ((selectedOptiFabric2 != null) ? selectedOptiFabric2.Id : null)) == null)
                {
                    ResourceVersion selectedOptiFabric3 = this.SelectedOptiFabric;
                    text4 = ((selectedOptiFabric3 != null) ? selectedOptiFabric3.Display : null);
                }
            }
            ModDownload.DlLiteLoaderListEntry selectedLiteLoader = this.SelectedLiteLoader;
            string text5;
            if (selectedLiteLoader == null)
            {
                text5 = null;
            }
            else
            {
                JToken jsonToken = selectedLiteLoader.JsonToken;
                text5 = ((jsonToken != null) ? jsonToken["version"].ToString() : null);
            }
            return new McComponents(vanillaName, jobject, text, text2, selectedFabric, text3, text4, text5);
        }

        private void TextSelectName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && this.IsInSelectPage && this.BtnStart.IsEnabled)
            {
                this.BtnStart_Click();
            }
        }

        private void BtnStart_Click()
        {
            string text = this.TextSelectName.Text;
            ModDownloadLib.McInstallRequest mcInstallRequest = new ModDownloadLib.McInstallRequest
            {
                NewInstanceName = text,
                InstanceFolder = string.Format("{0}versions\\{1}\\", ModMinecraft.McFolderSelected, text),
                Components = this.GetSelectedComponents(),
                MinecraftJson = this.VanillaData["url"].ToString(),
                ForgeEntry = this.SelectedForge,
                NeoForgeEntry = this.SelectedNeoForge,
                FabricApi = this.SelectedFabricApi,
                OptiFabric = this.SelectedOptiFabric,
                LiteLoaderEntry = this.SelectedLiteLoader
            };
            this.StartInstall(mcInstallRequest);
        }

        public virtual void StartInstall(ModDownloadLib.McInstallRequest Request)
        {
            if (((this.SelectedForge == null && this.SelectedNeoForge == null && this.SelectedFabric == null) || (Settings.Get<int>("LaunchArgumentIndieV2", null) != 0 && Settings.Get<int>("LaunchArgumentIndieV2", null) != 2) || ModMain.MyMsgBox("你尚未开启版本隔离，多个 MC 版本会共用同一个 Mod 文件夹。\r\n因此，游戏可能会因为读取到与当前版本不符的 Mod 而崩溃。\r\n推荐先在 设置 → 启动选项 → 默认版本隔离 中开启版本隔离！", "版本隔离提示", "取消下载", "继续", "", false, true, false, null, null, null) != 1) && ModDownloadLib.McInstallStart(Request))
            {
                this.ExitSelectPage();
            }
        }

        private string GetLoaderError(MyLoading Loader)
        {
            string text;
            if (Loader == null)
            {
                text = "获取中……";
            }
            else if (!Loader.State.IsLoader)
            {
                text = "获取中……";
            }
            else
            {
                switch (Loader.State.LoadingState)
                {
                case MyLoading.MyLoadingState.Unloaded:
                    return "未知错误，状态为 Unloaded";
                case MyLoading.MyLoadingState.Run:
                    return "获取中……";
                case MyLoading.MyLoadingState.Error:
                {
                    string message = ((ModLoader.LoaderBase)Loader.State).Error.Message;
                    return (Operators.CompareString(message, "无", false) == 0) ? "无" : ("获取失败：" + message);
                }
                }
                text = null;
            }
            return text;
        }

        internal virtual Grid PanAllBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyScrollViewer PanBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanInner
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanMinecraft
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanSelect
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard CardSelectName
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnBack
        {
            [CompilerGenerated]
            get
            {
                return this._BtnBack;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = delegate(object a0, EventArgs a1)
                {
                    this.ExitSelectPage();
                };
                MyIconButton myIconButton = this._BtnBack;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnBack = value;
                myIconButton = this._BtnBack;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyImage ImgLogo
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyTextBox TextSelectName
        {
            [CompilerGenerated]
            get
            {
                return this._TextSelectName;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                TextChangedEventHandler textChangedEventHandler = new TextChangedEventHandler(this.TextSelectName_TextChanged);
                MyTextBox.ValidateChangedEventHandler validateChangedEventHandler = new MyTextBox.ValidateChangedEventHandler(this.TextSelectName_ValidateChanged);
                KeyEventHandler keyEventHandler = new KeyEventHandler(this.TextSelectName_KeyDown);
                MyTextBox myTextBox = this._TextSelectName;
                if (myTextBox != null)
                {
                    myTextBox.TextChanged -= textChangedEventHandler;
                    MyTextBox.ValidateChanged -= validateChangedEventHandler;
                    myTextBox.KeyDown -= keyEventHandler;
                }
                this._TextSelectName = value;
                myTextBox = this._TextSelectName;
                if (myTextBox != null)
                {
                    myTextBox.TextChanged += textChangedEventHandler;
                    MyTextBox.ValidateChanged += validateChangedEventHandler;
                    myTextBox.KeyDown += keyEventHandler;
                }
            }
        }

        internal virtual TextBlock LabSelectName
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnOriginalNameRestore
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyHint HintSelectName
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyHint HintFabricAPI
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyHint HintOptiFabric
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyHint HintOptiFabricOld
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyHint HintModOptiFine
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard CardMinecraft
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanMinecraftInfo
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyImage ImgMinecraft
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabMinecraftVersion
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanMinecraftCompare
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyImage ImgMinecraftOld
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabMinecraftOld
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyImage ImgMinecraftNew
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabMinecraftNew
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnMinecraftRestore
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard CardForge
        {
            [CompilerGenerated]
            get
            {
                return this._CardForge;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCard.SwapEventHandler swapEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.ReloadSelected();
                };
                MyCard.PreviewSwapEventHandler previewSwapEventHandler = new MyCard.PreviewSwapEventHandler(this.CardForge_PreviewSwap);
                MyCard myCard = this._CardForge;
                if (myCard != null)
                {
                    myCard.Swap -= swapEventHandler;
                    myCard.PreviewSwap -= previewSwapEventHandler;
                }
                this._CardForge = value;
                myCard = this._CardForge;
                if (myCard != null)
                {
                    myCard.Swap += swapEventHandler;
                    myCard.PreviewSwap += previewSwapEventHandler;
                }
            }
        }

        internal virtual StackPanel PanForge
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanForgeInfo
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyImage ImgForge
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabForge
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanForgeCompare
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabForgeOld
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnForgeClear
        {
            [CompilerGenerated]
            get
            {
                return this._BtnForgeClear;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.Forge_Clear);
                MyIconButton myIconButton = this._BtnForgeClear;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnForgeClear = value;
                myIconButton = this._BtnForgeClear;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyIconButton BtnForgeRestore
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard CardNeoForge
        {
            [CompilerGenerated]
            get
            {
                return this._CardNeoForge;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCard.SwapEventHandler swapEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.ReloadSelected();
                };
                MyCard.PreviewSwapEventHandler previewSwapEventHandler = new MyCard.PreviewSwapEventHandler(this.CardNeoForge_PreviewSwap);
                MyCard myCard = this._CardNeoForge;
                if (myCard != null)
                {
                    myCard.Swap -= swapEventHandler;
                    myCard.PreviewSwap -= previewSwapEventHandler;
                }
                this._CardNeoForge = value;
                myCard = this._CardNeoForge;
                if (myCard != null)
                {
                    myCard.Swap += swapEventHandler;
                    myCard.PreviewSwap += previewSwapEventHandler;
                }
            }
        }

        internal virtual StackPanel PanNeoForge
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanNeoForgeInfo
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyImage ImgNeoForge
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabNeoForge
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanNeoForgeCompare
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabNeoForgeOld
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnNeoForgeClear
        {
            [CompilerGenerated]
            get
            {
                return this._BtnNeoForgeClear;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.NeoForge_Clear);
                MyIconButton myIconButton = this._BtnNeoForgeClear;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnNeoForgeClear = value;
                myIconButton = this._BtnNeoForgeClear;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyIconButton BtnNeoForgeRestore
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard CardFabric
        {
            [CompilerGenerated]
            get
            {
                return this._CardFabric;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCard.SwapEventHandler swapEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.ReloadSelected();
                };
                MyCard.PreviewSwapEventHandler previewSwapEventHandler = new MyCard.PreviewSwapEventHandler(this.CardFabric_PreviewSwap);
                MyCard myCard = this._CardFabric;
                if (myCard != null)
                {
                    myCard.Swap -= swapEventHandler;
                    myCard.PreviewSwap -= previewSwapEventHandler;
                }
                this._CardFabric = value;
                myCard = this._CardFabric;
                if (myCard != null)
                {
                    myCard.Swap += swapEventHandler;
                    myCard.PreviewSwap += previewSwapEventHandler;
                }
            }
        }

        internal virtual StackPanel PanFabric
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanFabricInfo
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyImage ImgFabric
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabFabric
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanFabricCompare
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabFabricOld
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnFabricClear
        {
            [CompilerGenerated]
            get
            {
                return this._BtnFabricClear;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.Fabric_Clear);
                MyIconButton myIconButton = this._BtnFabricClear;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnFabricClear = value;
                myIconButton = this._BtnFabricClear;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyIconButton BtnFabricRestore
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard CardFabricApi
        {
            [CompilerGenerated]
            get
            {
                return this._CardFabricApi;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCard.SwapEventHandler swapEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.ReloadSelected();
                };
                MyCard.PreviewSwapEventHandler previewSwapEventHandler = new MyCard.PreviewSwapEventHandler(this.CardFabricApi_PreviewSwap);
                MyCard myCard = this._CardFabricApi;
                if (myCard != null)
                {
                    myCard.Swap -= swapEventHandler;
                    myCard.PreviewSwap -= previewSwapEventHandler;
                }
                this._CardFabricApi = value;
                myCard = this._CardFabricApi;
                if (myCard != null)
                {
                    myCard.Swap += swapEventHandler;
                    myCard.PreviewSwap += previewSwapEventHandler;
                }
            }
        }

        internal virtual StackPanel PanFabricApi
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanFabricApiInfo
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyImage ImgFabricApi
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabFabricApi
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanFabricApiCompare
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabFabricApiOld
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnFabricApiClear
        {
            [CompilerGenerated]
            get
            {
                return this._BtnFabricApiClear;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.FabricApi_Clear);
                MyIconButton myIconButton = this._BtnFabricApiClear;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnFabricApiClear = value;
                myIconButton = this._BtnFabricApiClear;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyIconButton BtnFabricApiRestore
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard CardOptiFine
        {
            [CompilerGenerated]
            get
            {
                return this._CardOptiFine;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCard.SwapEventHandler swapEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.ReloadSelected();
                };
                MyCard.PreviewSwapEventHandler previewSwapEventHandler = new MyCard.PreviewSwapEventHandler(this.CardOptiFine_PreviewSwap);
                MyCard myCard = this._CardOptiFine;
                if (myCard != null)
                {
                    myCard.Swap -= swapEventHandler;
                    myCard.PreviewSwap -= previewSwapEventHandler;
                }
                this._CardOptiFine = value;
                myCard = this._CardOptiFine;
                if (myCard != null)
                {
                    myCard.Swap += swapEventHandler;
                    myCard.PreviewSwap += previewSwapEventHandler;
                }
            }
        }

        internal virtual StackPanel PanOptiFine
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanOptiFineInfo
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyImage ImgOptiFine
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabOptiFine
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanOptiFineCompare
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabOptiFineOld
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnOptiFineClear
        {
            [CompilerGenerated]
            get
            {
                return this._BtnOptiFineClear;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.OptiFine_Clear);
                MyIconButton myIconButton = this._BtnOptiFineClear;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnOptiFineClear = value;
                myIconButton = this._BtnOptiFineClear;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyIconButton BtnOptiFineRestore
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard CardOptiFabric
        {
            [CompilerGenerated]
            get
            {
                return this._CardOptiFabric;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCard.SwapEventHandler swapEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.ReloadSelected();
                };
                MyCard.PreviewSwapEventHandler previewSwapEventHandler = new MyCard.PreviewSwapEventHandler(this.CardOptiFabric_PreviewSwap);
                MyCard myCard = this._CardOptiFabric;
                if (myCard != null)
                {
                    myCard.Swap -= swapEventHandler;
                    myCard.PreviewSwap -= previewSwapEventHandler;
                }
                this._CardOptiFabric = value;
                myCard = this._CardOptiFabric;
                if (myCard != null)
                {
                    myCard.Swap += swapEventHandler;
                    myCard.PreviewSwap += previewSwapEventHandler;
                }
            }
        }

        internal virtual StackPanel PanOptiFabric
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanOptiFabricInfo
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyImage ImgOptiFabric
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabOptiFabric
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanOptiFabricCompare
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabOptiFabricOld
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnOptiFabricClear
        {
            [CompilerGenerated]
            get
            {
                return this._BtnOptiFabricClear;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.OptiFabric_Clear);
                MyIconButton myIconButton = this._BtnOptiFabricClear;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnOptiFabricClear = value;
                myIconButton = this._BtnOptiFabricClear;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyIconButton BtnOptiFabricRestore
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard CardLiteLoader
        {
            [CompilerGenerated]
            get
            {
                return this._CardLiteLoader;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCard.SwapEventHandler swapEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.ReloadSelected();
                };
                MyCard.PreviewSwapEventHandler previewSwapEventHandler = new MyCard.PreviewSwapEventHandler(this.CardLiteLoader_PreviewSwap);
                MyCard myCard = this._CardLiteLoader;
                if (myCard != null)
                {
                    myCard.Swap -= swapEventHandler;
                    myCard.PreviewSwap -= previewSwapEventHandler;
                }
                this._CardLiteLoader = value;
                myCard = this._CardLiteLoader;
                if (myCard != null)
                {
                    myCard.Swap += swapEventHandler;
                    myCard.PreviewSwap += previewSwapEventHandler;
                }
            }
        }

        internal virtual StackPanel PanLiteLoader
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanLiteLoaderInfo
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyImage ImgLiteLoader
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabLiteLoader
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanLiteLoaderCompare
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabLiteLoaderOld
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnLiteLoaderClear
        {
            [CompilerGenerated]
            get
            {
                return this._BtnLiteLoaderClear;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.LiteLoader_Clear);
                MyIconButton myIconButton = this._BtnLiteLoaderClear;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnLiteLoaderClear = value;
                myIconButton = this._BtnLiteLoaderClear;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyIconButton BtnLiteLoaderRestore
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyExtraTextButton BtnStart
        {
            [CompilerGenerated]
            get
            {
                return this._BtnStart;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyExtraTextButton.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.BtnStart_Click();
                };
                MyExtraTextButton myExtraTextButton = this._BtnStart;
                if (myExtraTextButton != null)
                {
                    myExtraTextButton.Click -= clickEventHandler;
                }
                this._BtnStart = value;
                myExtraTextButton = this._BtnStart;
                if (myExtraTextButton != null)
                {
                    myExtraTextButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyCard PanLoad
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyLoading LoadMinecraft
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyLoading LoadOptiFine
        {
            [CompilerGenerated]
            get
            {
                return this._LoadOptiFine;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyLoading.StateChangedEventHandler stateChangedEventHandler = delegate(object a0, MyLoading.MyLoadingState a1, MyLoading.MyLoadingState a2)
                {
                    this.ReloadSelected();
                };
                MyLoading.StateChangedEventHandler stateChangedEventHandler2 = delegate(object a0, MyLoading.MyLoadingState a1, MyLoading.MyLoadingState a2)
                {
                    this.OptiFine_Loaded();
                };
                MyLoading myLoading = this._LoadOptiFine;
                if (myLoading != null)
                {
                    myLoading.StateChanged -= stateChangedEventHandler;
                    myLoading.StateChanged -= stateChangedEventHandler2;
                }
                this._LoadOptiFine = value;
                myLoading = this._LoadOptiFine;
                if (myLoading != null)
                {
                    myLoading.StateChanged += stateChangedEventHandler;
                    myLoading.StateChanged += stateChangedEventHandler2;
                }
            }
        }

        internal virtual MyLoading LoadForge
        {
            [CompilerGenerated]
            get
            {
                return this._LoadForge;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyLoading.StateChangedEventHandler stateChangedEventHandler = delegate(object a0, MyLoading.MyLoadingState a1, MyLoading.MyLoadingState a2)
                {
                    this.ReloadSelected();
                };
                MyLoading.StateChangedEventHandler stateChangedEventHandler2 = delegate(object a0, MyLoading.MyLoadingState a1, MyLoading.MyLoadingState a2)
                {
                    this.Forge_Loaded();
                };
                MyLoading myLoading = this._LoadForge;
                if (myLoading != null)
                {
                    myLoading.StateChanged -= stateChangedEventHandler;
                    myLoading.StateChanged -= stateChangedEventHandler2;
                }
                this._LoadForge = value;
                myLoading = this._LoadForge;
                if (myLoading != null)
                {
                    myLoading.StateChanged += stateChangedEventHandler;
                    myLoading.StateChanged += stateChangedEventHandler2;
                }
            }
        }

        internal virtual MyLoading LoadNeoForge
        {
            [CompilerGenerated]
            get
            {
                return this._LoadNeoForge;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyLoading.StateChangedEventHandler stateChangedEventHandler = delegate(object a0, MyLoading.MyLoadingState a1, MyLoading.MyLoadingState a2)
                {
                    this.ReloadSelected();
                };
                MyLoading.StateChangedEventHandler stateChangedEventHandler2 = delegate(object a0, MyLoading.MyLoadingState a1, MyLoading.MyLoadingState a2)
                {
                    this.NeoForge_Loaded();
                };
                MyLoading myLoading = this._LoadNeoForge;
                if (myLoading != null)
                {
                    myLoading.StateChanged -= stateChangedEventHandler;
                    myLoading.StateChanged -= stateChangedEventHandler2;
                }
                this._LoadNeoForge = value;
                myLoading = this._LoadNeoForge;
                if (myLoading != null)
                {
                    myLoading.StateChanged += stateChangedEventHandler;
                    myLoading.StateChanged += stateChangedEventHandler2;
                }
            }
        }

        internal virtual MyLoading LoadLiteLoader
        {
            [CompilerGenerated]
            get
            {
                return this._LoadLiteLoader;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyLoading.StateChangedEventHandler stateChangedEventHandler = delegate(object a0, MyLoading.MyLoadingState a1, MyLoading.MyLoadingState a2)
                {
                    this.ReloadSelected();
                };
                MyLoading.StateChangedEventHandler stateChangedEventHandler2 = delegate(object a0, MyLoading.MyLoadingState a1, MyLoading.MyLoadingState a2)
                {
                    this.LiteLoader_Loaded();
                };
                MyLoading myLoading = this._LoadLiteLoader;
                if (myLoading != null)
                {
                    myLoading.StateChanged -= stateChangedEventHandler;
                    myLoading.StateChanged -= stateChangedEventHandler2;
                }
                this._LoadLiteLoader = value;
                myLoading = this._LoadLiteLoader;
                if (myLoading != null)
                {
                    myLoading.StateChanged += stateChangedEventHandler;
                    myLoading.StateChanged += stateChangedEventHandler2;
                }
            }
        }

        internal virtual MyLoading LoadFabric
        {
            [CompilerGenerated]
            get
            {
                return this._LoadFabric;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyLoading.StateChangedEventHandler stateChangedEventHandler = delegate(object a0, MyLoading.MyLoadingState a1, MyLoading.MyLoadingState a2)
                {
                    this.ReloadSelected();
                };
                MyLoading.StateChangedEventHandler stateChangedEventHandler2 = delegate(object a0, MyLoading.MyLoadingState a1, MyLoading.MyLoadingState a2)
                {
                    this.Fabric_Loaded();
                };
                MyLoading myLoading = this._LoadFabric;
                if (myLoading != null)
                {
                    myLoading.StateChanged -= stateChangedEventHandler;
                    myLoading.StateChanged -= stateChangedEventHandler2;
                }
                this._LoadFabric = value;
                myLoading = this._LoadFabric;
                if (myLoading != null)
                {
                    myLoading.StateChanged += stateChangedEventHandler;
                    myLoading.StateChanged += stateChangedEventHandler2;
                }
            }
        }

        internal virtual MyLoading LoadFabricApi
        {
            [CompilerGenerated]
            get
            {
                return this._LoadFabricApi;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyLoading.StateChangedEventHandler stateChangedEventHandler = delegate(object a0, MyLoading.MyLoadingState a1, MyLoading.MyLoadingState a2)
                {
                    this.ReloadSelected();
                };
                MyLoading.StateChangedEventHandler stateChangedEventHandler2 = delegate(object a0, MyLoading.MyLoadingState a1, MyLoading.MyLoadingState a2)
                {
                    this.FabricApi_Loaded();
                };
                MyLoading myLoading = this._LoadFabricApi;
                if (myLoading != null)
                {
                    myLoading.StateChanged -= stateChangedEventHandler;
                    myLoading.StateChanged -= stateChangedEventHandler2;
                }
                this._LoadFabricApi = value;
                myLoading = this._LoadFabricApi;
                if (myLoading != null)
                {
                    myLoading.StateChanged += stateChangedEventHandler;
                    myLoading.StateChanged += stateChangedEventHandler2;
                }
            }
        }

        internal virtual MyLoading LoadOptiFabric
        {
            [CompilerGenerated]
            get
            {
                return this._LoadOptiFabric;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyLoading.StateChangedEventHandler stateChangedEventHandler = delegate(object a0, MyLoading.MyLoadingState a1, MyLoading.MyLoadingState a2)
                {
                    this.ReloadSelected();
                };
                MyLoading.StateChangedEventHandler stateChangedEventHandler2 = delegate(object a0, MyLoading.MyLoadingState a1, MyLoading.MyLoadingState a2)
                {
                    this.OptiFabric_Loaded();
                };
                MyLoading myLoading = this._LoadOptiFabric;
                if (myLoading != null)
                {
                    myLoading.StateChanged -= stateChangedEventHandler;
                    myLoading.StateChanged -= stateChangedEventHandler2;
                }
                this._LoadOptiFabric = value;
                myLoading = this._LoadOptiFabric;
                if (myLoading != null)
                {
                    myLoading.StateChanged += stateChangedEventHandler;
                    myLoading.StateChanged += stateChangedEventHandler2;
                }
            }
        }

        private bool IsLoad;

        public bool IsInSelectPage;

        public string VanillaName;

        public JObject VanillaData;

        public string VanillaIcon;

        public ModDownload.DlOptiFineListEntry SelectedOptiFine;

        public ModDownload.DlLiteLoaderListEntry SelectedLiteLoader;

        public ModDownload.DlForgeVersionEntry SelectedForge;

        public ModDownload.DlNeoForgeListEntry SelectedNeoForge;

        public string SelectedFabric;

        public ResourceVersion SelectedFabricApi;

        public ResourceVersion SelectedOptiFabric;

        public bool IsSelectNameEdited;

        public JArray MinecraftVersions;

        public static string VersionWaitingSelect = null;

        public bool AutoSelectedFabricApi;

        public bool AutoSelectedOptiFabric;

        private bool $STATIC$ReloadSelected$2001$Ongoing;

        private StaticLocalInitFlag $STATIC$ReloadSelected$2001$Ongoing$Init;
    }
}
