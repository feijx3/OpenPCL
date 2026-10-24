using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;
using PCL.Our;
using PCLCS;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageSelectRight : OurPageRight
    {
        public PageSelectRight()
        {
            base.Loaded += this.PageSelectRight_Loaded;
            base.Initialized += this._Lambda___R0_1;
            this.ShowHidden = false;
            this.InitializeComponent();
            this.BtnEmptyDownload_WithEvents = this.BtnEmptyDownload;
            this.Load_WithEvents = this.Load;
        }

        private void PageSelectRight_Loaded(object sender, RoutedEventArgs e)
        {
            ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.RunOnUpdated, 1, "versions\\", false);
            this.PanBack.ScrollToHome();
        }

        private void LoaderInit()
        {
            base.PageLoaderInit<string, int>(this.Load, this.PanLoad, this.PanAllBack, null, ModMinecraft.McInstanceListLoader, new Action<ModLoader.LoaderTask<string, int>>(this.McInstanceListUI), null, false);
        }

        private void Load_Click(object sender, MouseButtonEventArgs e)
        {
            if (ModMinecraft.McInstanceListLoader.State == ModBase.LoadState.Failed)
            {
                ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
            }
        }

        private void McInstanceListUI(ModLoader.LoaderTask<string, int> Loader)
        {
            try
            {
                this.PanMain.Children.Clear();
                foreach (KeyValuePair<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>> keyValuePair in ModMinecraft.McInstanceList.ToArray<KeyValuePair<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>>>())
                {
                    if (!((keyValuePair.Key == ModMinecraft.McInstanceCardType.Hidden) ^ this.ShowHidden))
                    {
                        List<ModMinecraft.McInstance> list = keyValuePair.Value.Where<ModMinecraft.McInstance>((PageSelectRight._Closure___._I5_0 == null) ? (PageSelectRight._Closure___._I5_0 = new Func<ModMinecraft.McInstance, bool>(PageSelectRight._Closure___._I._Lambda___5_0)) : PageSelectRight._Closure___._I5_0).ToList<ModMinecraft.McInstance>();
                        if (list.Any<ModMinecraft.McInstance>())
                        {
                            string text;
                            switch (keyValuePair.Key)
                            {
                            case ModMinecraft.McInstanceCardType.Star:
                                text = "收藏夹";
                                break;
                            case ModMinecraft.McInstanceCardType.Auto:
                                goto IL_01D9;
                            case ModMinecraft.McInstanceCardType.Hidden:
                                text = "隐藏的版本";
                                break;
                            case ModMinecraft.McInstanceCardType.API:
                            {
                                bool flag = false;
                                bool flag2 = false;
                                bool flag3 = false;
                                bool flag4 = false;
                                try
                                {
                                    foreach (ModMinecraft.McInstance mcInstance in list)
                                    {
                                        if (mcInstance.Version.ModLoaders.HasFlagF(ModLoaders.Fabric))
                                        {
                                            flag3 = true;
                                        }
                                        if (mcInstance.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader))
                                        {
                                            flag4 = true;
                                        }
                                        if (mcInstance.Version.ModLoaders.HasFlagF(ModLoaders.Forge))
                                        {
                                            flag = true;
                                        }
                                        if (mcInstance.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge))
                                        {
                                            flag2 = true;
                                        }
                                    }
                                }
                                finally
                                {
                                    List<ModMinecraft.McInstance>.Enumerator enumerator = default;
                                    ((IDisposable)enumerator).Dispose();
                                }
                                if (checked((flag4 ? 1 : 0) + (flag ? 1 : 0) + (flag3 ? 1 : 0) + (flag2 ? 1 : 0)) > 1)
                                {
                                    text = "可安装 Mod";
                                }
                                else if (flag)
                                {
                                    text = "Forge 版本";
                                }
                                else if (flag2)
                                {
                                    text = "NeoForge 版本";
                                }
                                else if (flag4)
                                {
                                    text = "LiteLoader 版本";
                                }
                                else
                                {
                                    text = "Fabric 版本";
                                }
                                break;
                            }
                            case ModMinecraft.McInstanceCardType.OriginalLike:
                                text = "常规版本";
                                break;
                            case ModMinecraft.McInstanceCardType.Rubbish:
                                text = "不常用版本";
                                break;
                            case ModMinecraft.McInstanceCardType.Fool:
                                text = "愚人节版本";
                                break;
                            case ModMinecraft.McInstanceCardType.Error:
                                text = "错误的版本";
                                break;
                            default:
                                goto IL_01D9;
                            }
                            string text2 = text + ((Operators.CompareString(text, "收藏夹", false) == 0) ? "" : (" (" + Conversions.ToString(list.Count) + ")"));
                            OurCard ourCard = new OurCard
                            {
                                Title = text2,
                                Margin = new Thickness(0.0, 0.0, 0.0, 15.0)
                            };
                            StackPanel stackPanel = new StackPanel
                            {
                                Margin = new Thickness(20.0, 40.0, 18.0, 0.0),
                                VerticalAlignment = VerticalAlignment.Top,
                                RenderTransform = new TranslateTransform(0.0, 0.0)
                            };
                            ourCard.Children.Add(stackPanel);
                            ourCard.SwapControl = stackPanel;
                            ourCard.SetContentBuilder<ModMinecraft.McInstance>(list, (PageSelectRight._Closure___._I5_1 == null) ? (PageSelectRight._Closure___._I5_1 = new Func<ModMinecraft.McInstance, UIElement>(PageSelectRight._Closure___._I._Lambda___5_1)) : PageSelectRight._Closure___._I5_1);
                            this.PanMain.Children.Add(ourCard);
                            if (keyValuePair.Key == ModMinecraft.McInstanceCardType.Rubbish || keyValuePair.Key == ModMinecraft.McInstanceCardType.Error || keyValuePair.Key == ModMinecraft.McInstanceCardType.Fool)
                            {
                                ourCard.IsSwapped = true;
                                goto IL_0351;
                            }
                            ourCard.BuildContent();
                            goto IL_0351;
                            IL_01D9:
                            throw new ArgumentException("未知的卡片种类（" + Conversions.ToString((int)keyValuePair.Key) + "）");
                        }
                    }
                    IL_0351:;
                }
                if (this.PanMain.Children.Count == 1 && ((OurCard)this.PanMain.Children[0]).IsSwapped)
                {
                    ((OurCard)this.PanMain.Children[0]).IsSwapped = false;
                }
                if (this.PanMain.Children.Count == 0)
                {
                    this.PanEmpty.Visibility = Visibility.Visible;
                    this.PanBack.Visibility = Visibility.Collapsed;
                    if (this.ShowHidden)
                    {
                        this.LabEmptyTitle.Text = "无隐藏版本";
                        this.LabEmptyContent.Text = "没有版本被隐藏，你可以在版本设置的版本分类选项中隐藏版本。\r\n再次按下 F11 即可退出隐藏版本查看模式。";
                        this.BtnEmptyDownload.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this.LabEmptyTitle.Text = "无可用版本";
                        this.LabEmptyContent.Text = "未找到任何版本的游戏，请先下载任意版本的游戏。\r\n若有已存在的游戏，请在左边的列表中选择添加文件夹，选择 .minecraft 文件夹将其导入。";
                        this.BtnEmptyDownload.Visibility = ((Settings.Get<bool>("UiHiddenPageDownload", null) && !PageSetupUI.HiddenForceShow) ? Visibility.Collapsed : Visibility.Visible);
                    }
                }
                else
                {
                    this.PanBack.Visibility = Visibility.Visible;
                    this.PanEmpty.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "将版本列表转换显示时失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSelectRight.xaml.vb");
            }
        }

        public static void McInstanceListContent(OurListItem sender, EventArgs e)
        {
            PageSelectRight._Closure___6_0 CS_locals1 = new PageSelectRight._Closure___6_0();
            CS_locals1._VB_Local_sender = sender;
            CS_locals1._VB_Local_Instance = (ModMinecraft.McInstance)CS_locals1._VB_Local_sender.Tag;
            CS_locals1._VB_Local_sender.Click += ((PageSelectRight._Closure___._IR6_2 == null) ? (PageSelectRight._Closure___._IR6_2 = new OurListItem.ClickEventHandler(PageSelectRight._Closure___._I._Lambda___R6_2)) : PageSelectRight._Closure___._IR6_2);
            OurIconButton ourIconButton = new OurIconButton();
            if (CS_locals1._VB_Local_Instance.IsStar)
            {
                ourIconButton.ToolTip = "取消收藏";
                ourIconButton.LogoScale = 1.1;
                ourIconButton.Logo = "M700.856 155.543c-74.769 0-144.295 72.696-190.046 127.26-45.737-54.576-115.247-127.26-190.056-127.26-134.79 0-244.443 105.78-244.443 235.799 0 77.57 39.278 131.988 70.845 175.713C238.908 694.053 469.62 852.094 479.39 858.757c9.41 6.414 20.424 9.629 31.401 9.629 11.006 0 21.998-3.215 31.398-9.63 9.782-6.662 240.514-164.703 332.238-291.701 31.587-43.724 70.874-98.143 70.874-175.713-0.001-130.02-109.656-235.8-244.445-235.8z m0 0";
            }
            else
            {
                ourIconButton.ToolTip = "收藏";
                ourIconButton.LogoScale = 1.1;
                ourIconButton.Logo = "M512 896a42.666667 42.666667 0 0 1-30.293333-12.373333l-331.52-331.946667a224.426667 224.426667 0 0 1 0-315.733333 223.573333 223.573333 0 0 1 315.733333 0L512 282.026667l46.08-46.08a223.573333 223.573333 0 0 1 315.733333 0 224.426667 224.426667 0 0 1 0 315.733333l-331.52 331.946667A42.666667 42.666667 0 0 1 512 896zM308.053333 256a136.533333 136.533333 0 0 0-97.28 40.106667 138.24 138.24 0 0 0 0 194.986666L512 792.746667l301.226667-301.653334a138.24 138.24 0 0 0 0-194.986666 141.653333 141.653333 0 0 0-194.56 0l-76.373334 76.8a42.666667 42.666667 0 0 1-60.586666 0L405.333333 296.106667A136.533333 136.533333 0 0 0 308.053333 256z";
            }
            ourIconButton.Click += CS_locals1._Lambda___R3;
            OurIconButton ourIconButton2 = new OurIconButton
            {
                LogoScale = 1.1,
                Logo = "M520.192 0C408.43 0 317.44 82.87 313.563 186.734H52.736c-29.038 0-52.663 21.943-52.663 49.079s23.625 49.152 52.663 49.152h58.075v550.473c0 103.35 75.118 187.757 167.717 187.757h472.43c92.599 0 167.716-83.894 167.716-187.757V285.477h52.59c29.038 0 52.59-21.943 52.663-49.08-0.073-27.135-23.625-49.151-52.663-49.151H726.235C723.237 83.017 631.955 0 520.192 0zM404.846 177.957c3.803-50.03 50.176-89.015 107.447-89.015 57.197 0 103.57 38.985 106.788 89.015H404.92zM284.379 933.669c-33.353 0-69.997-39.351-69.997-95.525v-549.01H833.39v549.522c0 56.247-36.645 95.525-69.998 95.525H284.379v-0.512z M357.23 800.695a48.274 48.274 0 0 0 47.616-49.006V471.7a48.274 48.274 0 0 0-47.543-49.08 48.274 48.274 0 0 0-47.69 49.006V751.69c0 27.282 20.846 49.006 47.617 49.006z m166.62 0a48.274 48.274 0 0 0 47.688-49.006V471.7a48.274 48.274 0 0 0-47.689-49.08 48.274 48.274 0 0 0-47.543 49.006V751.69c0 27.282 21.431 49.006 47.543 49.006z m142.92 0a48.274 48.274 0 0 0 47.543-49.006V471.7a48.274 48.274 0 0 0-47.543-49.08 48.274 48.274 0 0 0-47.616 49.006V751.69c0 27.282 20.773 49.006 47.543 49.006z"
            };
            ourIconButton2.ToolTip = "删除";
            ourIconButton2.Click += CS_locals1._Lambda___R4;
            if (CS_locals1._VB_Local_Instance.State != ModMinecraft.McInstance.McInstanceState.Error)
            {
                OurIconButton ourIconButton3 = new OurIconButton
                {
                    LogoScale = 1.1,
                    Logo = "M651.946667 1001.813333c-22.186667 0-42.666667-10.24-61.44-27.306666-23.893333-23.893333-49.493333-35.84-75.093334-35.84-29.013333 0-56.32 11.946667-73.386666 30.72v3.413333c-17.066667 17.066667-42.666667 27.306667-66.56 27.306667h-6.826667c-6.826667 0-11.946667-1.706667-15.36-1.706667l-6.826667-1.706667c-64.853333-20.48-121.173333-54.613333-168.96-98.986666-29.013333-23.893333-37.546667-63.146667-25.6-95.573334 8.533333-23.893333 5.12-51.2-10.24-75.093333-15.36-27.306667-34.133333-40.96-59.733333-47.786667h-1.706667l-5.12-1.706666c-35.84-8.533333-61.44-34.133333-66.56-69.973334C1.706667 575.146667 0 537.6 0 512c0-32.426667 3.413333-63.146667 8.533333-93.866667v-6.826666l3.413334-8.533334c10.24-23.893333 23.893333-40.96 44.373333-51.2 5.12-3.413333 11.946667-6.826667 20.48-8.533333 27.306667-8.533333 51.2-25.6 63.146667-44.373333 13.653333-23.893333 17.066667-52.906667 10.24-81.92-11.946667-34.133333 0-71.68 30.72-93.866667 44.373333-37.546667 97.28-68.266667 158.72-93.866667l3.413333-1.706666c44.373333-13.653333 75.093333 3.413333 92.16 20.48 23.893333 23.893333 49.493333 35.84 75.093333 35.84 30.72 0 56.32-10.24 71.68-30.72l3.413334-3.413334c27.306667-27.306667 63.146667-35.84 93.866666-22.186666 63.146667 22.186667 117.76 54.613333 165.546667 97.28 29.013333 23.893333 37.546667 63.146667 25.6 95.573333-8.533333 23.893333-5.12 51.2 10.24 75.093333 15.36 27.306667 34.133333 40.96 59.733333 47.786667h1.706667l5.12 1.706667c35.84 8.533333 61.44 34.133333 66.56 71.68 6.826667 30.72 10.24 63.146667 11.946667 93.866666v3.413334c0 32.426667-3.413333 63.146667-8.533334 93.866666v6.826667l-3.413333 8.533333c-10.24 23.893333-23.893333 40.96-44.373333 51.2-5.12 3.413333-11.946667 6.826667-20.48 8.533334-27.306667 8.533333-51.2 25.6-63.146667 46.08-13.653333 23.893333-17.066667 52.906667-10.24 81.92 11.946667 35.84-1.706667 75.093333-30.72 95.573333-44.373333 35.84-95.573333 66.56-157.013333 92.16-15.36 3.413333-27.306667 3.413333-35.84 3.413333z m3.413333-83.626666z m1.706667 0zM517.12 853.333333c47.786667 0 93.866667 20.48 134.826667 59.733334 1.706667 1.706667 3.413333 1.706667 3.413333 3.413333 52.906667-22.186667 97.28-49.493333 136.533333-80.213333l1.706667-1.706667v-3.413333c-13.653333-52.906667-8.533333-104.106667 17.066667-148.48 23.893333-39.253333 64.853333-69.973333 114.346666-85.333334 1.706667 0 3.413333-1.706667 6.826667-6.826666 5.12-25.6 8.533333-51.2 8.533333-78.506667-1.706667-29.013333-3.413333-56.32-10.24-81.92v-5.12h-1.706666c-51.2-11.946667-90.453333-39.253333-119.466667-87.04-27.306667-44.373333-34.133333-100.693333-17.066667-148.48l-1.706666-1.706667h-3.413334c-39.253333-35.84-85.333333-63.146667-136.533333-80.213333H648.533333s-1.706667 1.706667-3.413333 1.706667c-32.426667 39.253333-80.213333 59.733333-136.533333 59.733333-47.786667 0-93.866667-20.48-134.826667-59.733333l-1.706667-1.706667h-1.706666c-54.613333 22.186667-98.986667 49.493333-136.533334 80.213333l-1.706666 1.706667v3.413333c13.653333 52.906667 8.533333 104.106667-17.066667 148.48-23.893333 39.253333-64.853333 69.973333-114.346667 85.333334-1.706667 0-3.413333 1.706667-6.826666 6.826666-6.826667 25.6-8.533333 51.2-8.533334 78.506667 0 30.72 3.413333 58.026667 6.826667 76.8l1.706667 5.12h1.706666c51.2 11.946667 90.453333 39.253333 119.466667 87.04 27.306667 44.373333 34.133333 100.693333 17.066667 148.48l1.706666 1.706667 1.706667 1.706666c37.546667 35.84 83.626667 63.146667 134.826667 80.213334 1.706667 0 3.413333 0 3.413333 1.706666h1.706667s1.706667 0 5.12-1.706666c34.133333-37.546667 81.92-59.733333 136.533333-59.733334z m-6.826667-146.773333c-110.933333 0-199.68-85.333333-199.68-196.266667 0-109.226667 87.04-196.266667 199.68-196.266666s199.68 85.333333 199.68 196.266666c-1.706667 109.226667-88.746667 196.266667-199.68 196.266667z m0-307.2c-63.146667 0-114.346667 49.493333-114.346666 110.933333 0 63.146667 49.493333 110.933333 114.346666 110.933334 30.72 0 59.733333-11.946667 80.213334-32.426667 20.48-20.48 32.426667-49.493333 32.426666-78.506667 0-63.146667-49.493333-110.933333-112.64-110.933333z"
                };
                ourIconButton3.ToolTip = "设置";
                ourIconButton3.Click += CS_locals1._Lambda___R5;
                CS_locals1._VB_Local_sender.MouseRightButtonUp += CS_locals1._Lambda___R6;
                CS_locals1._VB_Local_sender.Buttons = new OurIconButton[] { ourIconButton, ourIconButton2, ourIconButton3 };
                return;
            }
            OurIconButton ourIconButton4 = new OurIconButton
            {
                LogoScale = 1.15,
                Logo = "M889.018182 418.909091H884.363636V316.509091a93.090909 93.090909 0 0 0-99.607272-89.832727h-302.545455l-93.090909-76.334546A46.545455 46.545455 0 0 0 358.865455 139.636364H146.152727A93.090909 93.090909 0 0 0 46.545455 229.469091V837.818182a46.545455 46.545455 0 0 0 46.545454 46.545454 46.545455 46.545455 0 0 0 16.756364-3.258181 109.381818 109.381818 0 0 0 25.134545 3.258181h586.472727a85.178182 85.178182 0 0 0 87.04-63.301818l163.374546-302.545454a46.545455 46.545455 0 0 0 5.585454-21.876364A82.385455 82.385455 0 0 0 889.018182 418.909091z m-744.727273-186.181818h198.283636l93.09091 76.334545a46.545455 46.545455 0 0 0 29.323636 10.705455h319.301818a12.101818 12.101818 0 0 1 6.516364 0V418.909091H302.545455a85.178182 85.178182 0 0 0-87.04 63.301818L139.636364 622.778182V232.727273a19.549091 19.549091 0 0 1 6.516363 0z m578.094546 552.029091a27.461818 27.461818 0 0 0-2.792728 6.516363H154.530909l147.083636-272.290909a27.461818 27.461818 0 0 0 2.792728-6.981818h565.061818z"
            };
            ourIconButton4.ToolTip = "打开文件夹";
            ourIconButton4.Click += CS_locals1._Lambda___R7;
            CS_locals1._VB_Local_sender.MouseRightButtonUp += CS_locals1._Lambda___R8;
            CS_locals1._VB_Local_sender.Buttons = new OurIconButton[] { ourIconButton, ourIconButton2, ourIconButton4 };
        }

        public static void Item_Click(OurListItem sender, EventArgs e)
        {
            ModMinecraft.McInstance mcInstance = (ModMinecraft.McInstance)sender.Tag;
            if (ModMinecraft.McInstanceLock.IsLocked(mcInstance))
            {
                ModMain.Hint("该版本正在被修改，请稍后再选择！", ModMain.HintType.Red, true);
                return;
            }
            if (new ModMinecraft.McInstance(mcInstance.PathVersion).Check())
            {
                ModMinecraft.McInstanceSelected = mcInstance;
                ModMain.FrmMain.PageBack();
                return;
            }
            PageInstanceOverall.OpenInstanceFolder(mcInstance);
        }

        private void BtnDownload_Click(object sender, EventArgs e)
        {
            ModMain.FrmMain.PageChange(FormMain.PageType.Download, FormMain.PageSubType.Default);
        }

        public static void DeleteInstance(OurListItem Item, ModMinecraft.McInstance Instance)
        {
            try
            {
                if (ModMinecraft.McInstanceLock.IsLocked(Instance))
                {
                    ModMain.Hint("该版本正在被修改，请稍后再删除！", ModMain.HintType.Red, true);
                }
                else
                {
                    bool shiftKeyDown = OurWpfExtension.Computer.Keyboard.ShiftKeyDown;
                    bool flag = Instance.State != ModMinecraft.McInstance.McInstanceState.Error && Operators.CompareString(Instance.PathIndie, ModMinecraft.McFolderSelected, false) != 0;
                    DirectoryInfo info = DirectoryUtils.GetInfo(Instance.PathIndie + "saves\\");
                    List<string> list = ((flag && info.Exists) ? info.EnumerateDirectories().OrderByDescending<DirectoryInfo, DateTime>((PageSelectRight._Closure___._I9_0 == null) ? (PageSelectRight._Closure___._I9_0 = new Func<DirectoryInfo, DateTime>(PageSelectRight._Closure___._I._Lambda___9_0)) : PageSelectRight._Closure___._I9_0).Select<DirectoryInfo, string>((PageSelectRight._Closure___._I9_1 == null) ? (PageSelectRight._Closure___._I9_1 = new Func<DirectoryInfo, string>(PageSelectRight._Closure___._I._Lambda___9_1)) : PageSelectRight._Closure___._I9_1)
                        .ToList<string>() : new List<string>());
                    int num = ModMain.OurMsgBox(string.Format("你确定要{0}删除版本 {1} 吗？", shiftKeyDown ? "永久" : "", Instance.Name) + (flag ? "\r\n该版本对应的存档、资源包、Mod 等文件也将被一并删除！" : "") + (list.Any<string>() ? ("\r\n\r\n这会删除以下存档：\r\n· " + list.Join("\r\n· ")) : ""), "版本删除确认", "确定", "取消", "", true, true, false, null, null, null);
                    if (num != 1)
                    {
                        if (num == 2)
                        {
                            return;
                        }
                    }
                    else
                    {
                        Instance.ResetSettingsCache();
                        DirectoryUtils.Delete(Instance.PathVersion, !shiftKeyDown);
                        ModMain.Hint("版本 " + Instance.Name + " 已删除！", ModMain.HintType.Green, true);
                    }
                    if (Instance.DisplayType == ModMinecraft.McInstanceCardType.Hidden || !Instance.IsStar)
                    {
                        StackPanel stackPanel = (StackPanel)Item.Parent;
                        if (stackPanel.Children.Count > 2)
                        {
                            OurCard ourCard = (OurCard)stackPanel.Parent;
                            ourCard.Title = checked(ourCard.Title.Replace(Conversions.ToString(stackPanel.Children.Count - 1), Conversions.ToString(stackPanel.Children.Count - 2)));
                            stackPanel.Children.Remove(Item);
                            if (((object)(ModMinecraft.McInstanceSelected) != (object)null) && PathUtils.IsSame(Instance.PathVersion, ModMinecraft.McInstanceSelected.PathVersion))
                            {
                                ModMinecraft.McInstanceSelected = (ModMinecraft.McInstance)OurVirtualizingElement.TryInit((FrameworkElement)stackPanel.Children[0]).Tag;
                            }
                            ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.UpdateOnly, 1, "versions\\", false);
                        }
                        else
                        {
                            ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
                        }
                    }
                    else
                    {
                        ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                Logger.Warn(ex, string.Format("删除版本 {0} 被主动取消", Instance.Name), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSelectRight.xaml.vb");
            }
            catch (Exception ex2)
            {
                Logger.Error(ex2, string.Format("删除版本 {0} 失败", Instance.Name), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSelectRight.xaml.vb");
            }
        }

        public void BtnEmptyDownload_Loaded()
        {
            Visibility visibility = (((Settings.Get<bool>("UiHiddenPageDownload", null) && !PageSetupUI.HiddenForceShow) || this.ShowHidden) ? Visibility.Collapsed : Visibility.Visible);
            if (this.BtnEmptyDownload.Visibility != visibility)
            {
                this.BtnEmptyDownload.Visibility = visibility;
                this.PanLoad.TriggerForceResize();
            }
        }







        internal virtual OurButton BtnEmptyDownload_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnEmptyDownload;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnDownload_Click);
                RoutedEventHandler routedEventHandler = new RoutedEventHandler(this._Lambda___R37_9);
                OurButton ourButton = this._BtnEmptyDownload;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                    ourButton.Loaded -= routedEventHandler;
                }
                this._BtnEmptyDownload = value;
                ourButton = this._BtnEmptyDownload;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                    ourButton.Loaded += routedEventHandler;
                }
            }
        }


        internal virtual OurLoading Load_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._Load;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurLoading.ClickEventHandler clickEventHandler = new OurLoading.ClickEventHandler(this.Load_Click);
                OurLoading ourLoading = this._Load;
                if (ourLoading != null)
                {
                    ourLoading.Click -= clickEventHandler;
                }
                this._Load = value;
                ourLoading = this._Load;
                if (ourLoading != null)
                {
                    ourLoading.Click += clickEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R0_1(object a0, EventArgs a1)
        {
            this.LoaderInit();
        }

        [CompilerGenerated]
        private void _Lambda___R37_9(object a0, RoutedEventArgs a1)
        {
            this.BtnEmptyDownload_Loaded();
        }

        public bool ShowHidden;

        [CompilerGenerated]
        [AccessedThroughProperty("PanAllBack")]
        private Grid _PanAllBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurScrollViewer _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanMain")]
        private StackPanel _PanMain;

        [CompilerGenerated]
        [AccessedThroughProperty("PanEmpty")]
        private OurCard _PanEmpty;

        [CompilerGenerated]
        [AccessedThroughProperty("LabEmptyTitle")]
        private TextBlock _LabEmptyTitle;

        [CompilerGenerated]
        [AccessedThroughProperty("LabEmptyContent")]
        private TextBlock _LabEmptyContent;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnEmptyDownload")]
        private OurButton _BtnEmptyDownload;

        [CompilerGenerated]
        [AccessedThroughProperty("PanLoad")]
        private OurCard _PanLoad;

        [CompilerGenerated]
        [AccessedThroughProperty("Load")]
        private OurLoading _Load;

        [CompilerGenerated]
        [Serializable]
        private new sealed class _Closure___
        {
            internal bool _Lambda___5_0(ModMinecraft.McInstance Instance)
            {
                return !ModMinecraft.McInstanceLock.IsLocked(Instance);
            }

            internal UIElement _Lambda___5_1(ModMinecraft.McInstance Instance)
            {
                return Instance.ToListItem(new Action<OurListItem, EventArgs>(PageSelectRight.McInstanceListContent));
            }

            internal void _Lambda___R6_2(object a0, MouseButtonEventArgs a1)
            {
                PageSelectRight.Item_Click((OurListItem)a0, a1);
            }

            internal DateTime _Lambda___9_0(DirectoryInfo Save)
            {
                return Save.LastWriteTime;
            }

            internal string _Lambda___9_1(DirectoryInfo Save)
            {
                return string.Format("{0}（上次修改：{1}）", Save.Name, StringUtils.FormatTimeSpan(Save.LastWriteTime - DateTime.Now, false));
            }

            public static readonly PageSelectRight._Closure___ _I = new PageSelectRight._Closure___();

            public static Func<ModMinecraft.McInstance, bool> _I5_0;

            public static Func<ModMinecraft.McInstance, UIElement> _I5_1;

            public static OurListItem.ClickEventHandler _IR6_2;

            public static Func<DirectoryInfo, DateTime> _I9_0;

            public static Func<DirectoryInfo, string> _I9_1;
        }

        [CompilerGenerated]
        private sealed class _Closure___6_0
        {
            internal void _Lambda___R3(object a0, EventArgs a1)
            {
                this._Lambda___0();
            }

            internal void _Lambda___0()
            {
                ModBase.WriteIni(this._VB_Local_Instance.PathVersion + "PCL\\Setup.ini", "IsStar", Conversions.ToString(!this._VB_Local_Instance.IsStar));
                ModMinecraft.McInstanceListForceRefresh = true;
                ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
            }

            internal void _Lambda___R4(object a0, EventArgs a1)
            {
                this._Lambda___1();
            }

            internal void _Lambda___1()
            {
                PageSelectRight.DeleteInstance(this._VB_Local_sender, this._VB_Local_Instance);
            }

            internal void _Lambda___R5(object a0, EventArgs a1)
            {
                this._Lambda___2();
            }

            internal void _Lambda___2()
            {
                PageInstanceLeft.Instance = this._VB_Local_Instance;
                ModMain.FrmMain.PageChange(FormMain.PageType.InstanceSetup, FormMain.PageSubType.Default);
            }

            internal void _Lambda___R6(object a0, MouseButtonEventArgs a1)
            {
                this._Lambda___3();
            }

            internal void _Lambda___3()
            {
                PageInstanceLeft.Instance = this._VB_Local_Instance;
                ModMain.FrmMain.PageChange(FormMain.PageType.InstanceSetup, FormMain.PageSubType.Default);
            }

            internal void _Lambda___R7(object a0, EventArgs a1)
            {
                this._Lambda___4();
            }

            internal void _Lambda___4()
            {
                PageInstanceOverall.OpenInstanceFolder(this._VB_Local_Instance);
            }

            internal void _Lambda___R8(object a0, MouseButtonEventArgs a1)
            {
                this._Lambda___5();
            }

            internal void _Lambda___5()
            {
                PageInstanceOverall.OpenInstanceFolder(this._VB_Local_Instance);
            }

            public ModMinecraft.McInstance _VB_Local_Instance;

            public OurListItem _VB_Local_sender;
        }
    }
}
