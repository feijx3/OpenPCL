using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageSpeedLeft : MyPageLeft
    {
        public PageSpeedLeft()
        {
            base.Loaded += this.Page_Loaded;
            this.IsLoad = false;
            this.RightCards = new Dictionary<string, MyCard>();
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.Watcher();
            this.TryReturnToHome();
            if (!this.IsLoad)
            {
                this.IsLoad = true;
                DispatcherTimer dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
                dispatcherTimer.Tick += delegate(object a0, EventArgs a1)
                {
                    this.Watcher();
                };
                dispatcherTimer.Start();
                if (!ModBase.ModeDebug)
                {
                    base.RowDefinitions[12].Height = new GridLength(0.0);
                    base.RowDefinitions[13].Height = new GridLength(0.0);
                    base.RowDefinitions[14].Height = new GridLength(0.0);
                    base.RowDefinitions[15].Height = new GridLength(0.0);
                }
            }
        }

        private void Watcher()
        {
            if (ModMain.FrmMain.PageCurrent == FormMain.PageType.TaskManager)
            {
                try
                {
                    if (!ModLoader.LoaderTaskbar.Any<ModLoader.LoaderBase>())
                    {
                        this.LabProgress.Text = "100 %";
                        this.LabSpeed.Text = "0 B/s";
                        this.LabFile.Text = "0";
                        this.LabThread.Text = "0 / " + Conversions.ToString(ModNet.NetTaskThreadLimit);
                    }
                    else
                    {
                        List<ModLoader.LoaderBase> list = ModLoader.LoaderTaskbar.Where<ModLoader.LoaderBase>((PageSpeedLeft._Closure$__.$I5-0 == null) ? (PageSpeedLeft._Closure$__.$I5-0 = (ModLoader.LoaderBase l) => l.Show) : PageSpeedLeft._Closure$__.$I5-0).ToList<ModLoader.LoaderBase>();
                        double num = (list.Any<ModLoader.LoaderBase>() ? list.Select<ModLoader.LoaderBase, double>((PageSpeedLeft._Closure$__.$I5-1 == null) ? (PageSpeedLeft._Closure$__.$I5-1 = (ModLoader.LoaderBase l) => l.Progress) : PageSpeedLeft._Closure$__.$I5-1).Average().Clamp(0.0, 1.0) : 1.0);
                        string text = Conversions.ToString(Math.Floor(num * 100.0)) + "." + Math.Floor((num * 100.0 - Math.Floor(num * 100.0)) * 100.0).ToString().EnsureLength('0', 2) + " %";
                        this.LabProgress.Text = ((num > 0.999999) ? "100 %" : text);
                        this.LabSpeed.Text = StringUtils.FormatByteSize(ModNet.NetManager.Speed) + "/s";
                        this.LabFile.Text = Conversions.ToString((ModNet.NetManager.FileRemain < 0) ? "0*" : ModNet.NetManager.FileRemain);
                        this.LabThread.Text = Conversions.ToString(ModNet.NetTaskThreadCount) + " / " + Conversions.ToString(ModNet.NetTaskThreadLimit);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "任务管理左栏监视出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSpeedLeft.xaml.vb");
                }
                if (ModMain.FrmSpeedRight != null && ModMain.FrmSpeedRight.PanMain != null)
                {
                    try
                    {
                        try
                        {
                            foreach (ModLoader.LoaderBase loaderBase in ModLoader.LoaderTaskbar.ToList<ModLoader.LoaderBase>())
                            {
                                this.TaskRefresh(loaderBase);
                            }
                        }
                        finally
                        {
                            List<ModLoader.LoaderBase>.Enumerator enumerator;
                            ((IDisposable)enumerator).Dispose();
                        }
                    }
                    catch (Exception ex2)
                    {
                        Logger.Error(ex2, "任务管理右栏监视出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSpeedLeft.xaml.vb");
                    }
                }
            }
        }

        public void TaskRefresh(ModLoader.LoaderBase Loader)
        {
            PageSpeedLeft._Closure$__6-1 CS$<>8__locals1 = new PageSpeedLeft._Closure$__6-1(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Me = this;
            CS$<>8__locals1.$VB$Local_Loader = Loader;
            if (CS$<>8__locals1.$VB$Local_Loader != null && CS$<>8__locals1.$VB$Local_Loader.Show)
            {
                try
                {
                    List<ModLoader.LoaderBase> list = (List<ModLoader.LoaderBase>)NewLateBinding.LateGet(CS$<>8__locals1.$VB$Local_Loader, null, "GetLoaderList", new object[0], null, null, null);
                    if (this.RightCards.ContainsKey(CS$<>8__locals1.$VB$Local_Loader.Name))
                    {
                        Grid grid = this.RightCards[CS$<>8__locals1.$VB$Local_Loader.Name];
                        double num = CS$<>8__locals1.$VB$Local_Loader.Progress + (double)CS$<>8__locals1.$VB$Local_Loader.State;
                        if (ModBase.Val(RuntimeHelpers.GetObjectValue(grid.Tag)) == num)
                        {
                            return;
                        }
                        grid.Tag = num;
                        if (grid.Children.Count <= 3)
                        {
                            Logger.Warn(string.Format("元素不足的卡片：{0}", CS$<>8__locals1.$VB$Local_Loader.Name), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSpeedLeft.xaml.vb");
                            return;
                        }
                        grid = (Grid)grid.Children[3];
                        checked
                        {
                            try
                            {
                                switch (CS$<>8__locals1.$VB$Local_Loader.State)
                                {
                                case ModBase.LoadState.Waiting:
                                case ModBase.LoadState.Loading:
                                    try
                                    {
                                        if (grid.Children.Count < list.Count * 2)
                                        {
                                            Logger.Warn(string.Format("刷新任务管理卡片 {0} 失败：卡片中仅有 {1} 个子项，要求至少有 {2} 个子项", CS$<>8__locals1.$VB$Local_Loader.Name, grid.Children.Count, list.Count * 2), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSpeedLeft.xaml.vb");
                                        }
                                        else
                                        {
                                            int num2 = 0;
                                            try
                                            {
                                                foreach (ModLoader.LoaderBase loaderBase in list)
                                                {
                                                    switch (loaderBase.State)
                                                    {
                                                    case ModBase.LoadState.Waiting:
                                                        if (Operators.ConditionalCompareObjectNotEqual(((FrameworkElement)grid.Children[num2 * 2]).Tag, "Waiting", false))
                                                        {
                                                            grid.Children.RemoveAt(num2 * 2);
                                                            grid.Children.Insert(num2 * 2, (UIElement)ModBase.GetObjectFromXML("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:local=\"clr-namespace:PCL;assembly=Plain Craft Launcher 2\" Stretch=\"Uniform\" Tag=\"Waiting\" Data=\"F1 M5,0 a5,5 360 1 0 0,0.0001 m15,0 a5,5 360 1 0 0,0.0001 m15,0 a5,5 360 1 0 0,0.0001 Z\" Width=\"18\" HorizontalAlignment=\"Center\" Grid.Column=\"0\" Grid.Row=\"" + Conversions.ToString(num2) + "\" Fill=\"{DynamicResource ColorBrush3}\" Margin=\"0,7,0,0\" VerticalAlignment=\"Top\" Height=\"6\"/>"));
                                                        }
                                                        break;
                                                    case ModBase.LoadState.Loading:
                                                        if (Operators.ConditionalCompareObjectNotEqual(((FrameworkElement)grid.Children[num2 * 2]).Tag, "Loading", false))
                                                        {
                                                            grid.Children.RemoveAt(num2 * 2);
                                                            grid.Children.Insert(num2 * 2, (UIElement)ModBase.GetObjectFromXML(string.Concat(new string[]
                                                            {
                                                                "<TextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:local=\"clr-namespace:PCL;assembly=Plain Craft Launcher 2\" Text=\"",
                                                                Conversions.ToString(Math.Floor(unchecked(loaderBase.Progress * 100.0))),
                                                                "%\" Tag=\"Loading\" HorizontalAlignment=\"Center\" Grid.Column=\"0\" Grid.Row=\"",
                                                                Conversions.ToString(num2),
                                                                "\" Foreground=\"{DynamicResource ColorBrush3}\"/>"
                                                            })));
                                                        }
                                                        else
                                                        {
                                                            ((TextBlock)grid.Children[num2 * 2]).Text = Conversions.ToString(Math.Floor(unchecked(loaderBase.Progress * 100.0))) + "%";
                                                        }
                                                        break;
                                                    case ModBase.LoadState.Finished:
                                                        if (Operators.ConditionalCompareObjectNotEqual(((FrameworkElement)grid.Children[num2 * 2]).Tag, "Finished", false))
                                                        {
                                                            grid.Children.RemoveAt(num2 * 2);
                                                            grid.Children.Insert(num2 * 2, (UIElement)ModBase.GetObjectFromXML("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:local=\"clr-namespace:PCL;assembly=Plain Craft Launcher 2\" Stretch=\"Uniform\" Tag=\"Finished\" Data=\"F1 M 23.7501,33.25L 34.8334,44.3333L 52.2499,22.1668L 56.9999,26.9168L 34.8334,53.8333L 19.0001,38L 23.7501,33.25 Z\" Height=\"16\" Width=\"15\" HorizontalAlignment=\"Center\" Grid.Column=\"0\" Grid.Row=\"" + Conversions.ToString(num2) + "\" Fill=\"{DynamicResource ColorBrush3}\" Margin=\"0,3,0,0\" VerticalAlignment=\"Top\"/>"));
                                                        }
                                                        break;
                                                    }
                                                    num2++;
                                                }
                                            }
                                            finally
                                            {
                                                List<ModLoader.LoaderBase>.Enumerator enumerator;
                                                ((IDisposable)enumerator).Dispose();
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex, string.Format("刷新任务管理卡片 {0} 失败", CS$<>8__locals1.$VB$Local_Loader.Name), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSpeedLeft.xaml.vb");
                                    }
                                    break;
                                case ModBase.LoadState.Finished:
                                case ModBase.LoadState.Canceled:
                                    ModAnimation.AniDispose((MyCard)grid.Parent, true, delegate(object a0)
                                    {
                                        this.TryReturnToHome();
                                    });
                                    break;
                                case ModBase.LoadState.Failed:
                                {
                                    grid.RowDefinitions.Clear();
                                    grid.Children.Clear();
                                    grid.Children.Add((UIElement)ModBase.GetObjectFromXML("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Stretch=\"Uniform\" Tag=\"Failed\" Data=\"F1 M2.5,0 L0,2.5 7.5,10 0,17.5 2.5,20 10,12.5 17.5,20 20,17.5 12.5,10 20,2.5 17.5,0 10,7.5 2.5,0Z\" Height=\"15\" Width=\"15\" HorizontalAlignment=\"Center\" Grid.Column=\"0\" Grid.Row=\"0\" Fill=\"{DynamicResource ColorBrush3}\" Margin=\"0,1,0,0\" VerticalAlignment=\"Top\"/>"));
                                    TextBlock textBlock = (TextBlock)ModBase.GetObjectFromXML("<TextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" TextWrapping=\"Wrap\" HorizontalAlignment=\"Left\" ToolTip=\"单击复制错误详情\" Grid.Column=\"1\" Grid.Row=\"0\" Margin=\"0,0,0,5\" />");
                                    textBlock.Text = CS$<>8__locals1.$VB$Local_Loader.Error.GetDisplay(true);
                                    textBlock.MouseLeftButtonDown += ((PageSpeedLeft._Closure$__.$IR6-2 == null) ? (PageSpeedLeft._Closure$__.$IR6-2 = delegate(object a0, MouseButtonEventArgs a1)
                                    {
                                        ((PageSpeedLeft._Closure$__.$I6-0 == null) ? (PageSpeedLeft._Closure$__.$I6-0 = delegate(TextBlock sender, EventArgs e)
                                        {
                                            ModBase.ClipboardSet(sender.Text, false);
                                            ModMain.Hint("已复制错误详情！", ModMain.HintType.Green, true);
                                        }) : PageSpeedLeft._Closure$__.$I6-0)((TextBlock)a0, a1);
                                    }) : PageSpeedLeft._Closure$__.$IR6-2);
                                    grid.Children.Add(textBlock);
                                    break;
                                }
                                }
                                goto IL_08A1;
                            }
                            catch (Exception ex2)
                            {
                                Logger.Error(ex2, string.Format("更新任务管理显示失败（{0}）", CS$<>8__locals1.$VB$Local_Loader.State), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSpeedLeft.xaml.vb");
                                goto IL_08A1;
                            }
                        }
                    }
                    if (CS$<>8__locals1.$VB$Local_Loader.State != ModBase.LoadState.Canceled && CS$<>8__locals1.$VB$Local_Loader.State != ModBase.LoadState.Finished)
                    {
                        try
                        {
                            PageSpeedLeft._Closure$__6-0 CS$<>8__locals2 = new PageSpeedLeft._Closure$__6-0(CS$<>8__locals2);
                            CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1;
                            string text = string.Concat(new string[]
                            {
                                "\r\n                        <local:MyCard xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:local=\"clr-namespace:PCL;assembly=Plain Craft Launcher 2\"\r\n                            Tag=\"",
                                Conversions.ToString(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Loader.Progress + (double)CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Loader.State),
                                "\" Title=\"",
                                StringUtils.XmlEscape(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Loader.Name),
                                "\" Margin=\"0,0,0,15\">\r\n                            <Grid Margin=\"14,40,15,10\">\r\n                                <Grid.ColumnDefinitions>\r\n                                    <ColumnDefinition Width=\"50\"/>\r\n                                    <ColumnDefinition/>\r\n                                </Grid.ColumnDefinitions>\r\n                                <Grid.RowDefinitions>"
                            });
                            try
                            {
                                foreach (ModLoader.LoaderBase loaderBase2 in list)
                                {
                                    text += "<RowDefinition Height=\"26\"/>";
                                }
                            }
                            finally
                            {
                                List<ModLoader.LoaderBase>.Enumerator enumerator2;
                                ((IDisposable)enumerator2).Dispose();
                            }
                            text += "</Grid.RowDefinitions>";
                            int num3 = 0;
                            try
                            {
                                foreach (ModLoader.LoaderBase loaderBase3 in list)
                                {
                                    switch (loaderBase3.State)
                                    {
                                    case ModBase.LoadState.Waiting:
                                        text = text + "<Path Stretch=\"Uniform\" Tag=\"Waiting\" Data=\"F1 M5,0 a5,5 360 1 0 0,0.0001 m15,0 a5,5 360 1 0 0,0.0001 m15,0 a5,5 360 1 0 0,0.0001 Z\" Width=\"18\" HorizontalAlignment=\"Center\" Grid.Column=\"0\" Grid.Row=\"" + Conversions.ToString(num3) + "\" Fill=\"{DynamicResource ColorBrush3}\" Margin=\"0,7,0,0\" VerticalAlignment=\"Top\" Height=\"6\"/>";
                                        break;
                                    case ModBase.LoadState.Loading:
                                        text = string.Concat(new string[]
                                        {
                                            text,
                                            "<TextBlock Text=\"",
                                            Conversions.ToString(Math.Floor(loaderBase3.Progress * 100.0)),
                                            "%\" Tag=\"Loading\" HorizontalAlignment=\"Center\" Grid.Column=\"0\" Grid.Row=\"",
                                            Conversions.ToString(num3),
                                            "\" Foreground=\"{DynamicResource ColorBrush3}\" />"
                                        });
                                        break;
                                    case ModBase.LoadState.Finished:
                                        text = text + "<Path Stretch=\"Uniform\" Tag=\"Finished\" Data=\"F1 M 23.7501,33.25L 34.8334,44.3333L 52.2499,22.1668L 56.9999,26.9168L 34.8334,53.8333L 19.0001,38L 23.7501,33.25 Z\" Height=\"16\" Width=\"15\" HorizontalAlignment=\"Center\" Grid.Column=\"0\" Grid.Row=\"" + Conversions.ToString(num3) + "\" Fill=\"{DynamicResource ColorBrush3}\" Margin=\"0,3,0,0\" VerticalAlignment=\"Top\"/>";
                                        break;
                                    default:
                                        text = text + "<Path Stretch=\"Uniform\" Tag=\"Failed\" Data=\"F1 M2.5,0 L0,2.5 7.5,10 0,17.5 2.5,20 10,12.5 17.5,20 20,17.5 12.5,10 20,2.5 17.5,0 10,7.5 2.5,0Z\" Height=\"15\" Width=\"15\" HorizontalAlignment=\"Center\" Grid.Column=\"0\" Grid.Row=\"" + Conversions.ToString(num3) + "\" Fill=\"{DynamicResource ColorBrush3}\" Margin=\"0,1,0,0\" VerticalAlignment=\"Top\"/>";
                                        break;
                                    }
                                    text = string.Concat(new string[]
                                    {
                                        text,
                                        "<TextBlock Text=\"",
                                        StringUtils.XmlEscape(loaderBase3.Name),
                                        "\" HorizontalAlignment=\"Left\" Grid.Column=\"1\" Grid.Row=\"",
                                        Conversions.ToString(num3),
                                        "\"/>"
                                    });
                                    checked
                                    {
                                        num3++;
                                    }
                                }
                            }
                            finally
                            {
                                List<ModLoader.LoaderBase>.Enumerator enumerator3;
                                ((IDisposable)enumerator3).Dispose();
                            }
                            text += "</Grid></local:MyCard>";
                            try
                            {
                                CS$<>8__locals2.$VB$Local_Card = (MyCard)ModBase.GetObjectFromXML(text);
                            }
                            catch (Exception ex3)
                            {
                                Logger.Warn(ex3, "新建任务管理卡片失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSpeedLeft.xaml.vb");
                                Logger.Info(string.Format("出错的卡片内容：{0}{1}", "\r\n", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSpeedLeft.xaml.vb");
                                throw;
                            }
                            ModMain.FrmSpeedRight.PanMain.Children.Insert(0, CS$<>8__locals2.$VB$Local_Card);
                            this.RightCards.Add(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Loader.Name, CS$<>8__locals2.$VB$Local_Card);
                            Logger.Info(string.Format("新建任务管理卡片：{0}", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Loader.Name), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSpeedLeft.xaml.vb");
                            MyIconButton myIconButton = new MyIconButton
                            {
                                Name = "BtnCancel",
                                Logo = "F1 M2,0 L0,2 8,10 0,18 2,20 10,12 18,20 20,18 12,10 20,2 18,0 10,8 2,0Z",
                                Height = 20.0,
                                Margin = new Thickness(0.0, 10.0, 10.0, 0.0),
                                LogoScale = 1.1,
                                HorizontalAlignment = HorizontalAlignment.Right,
                                VerticalAlignment = VerticalAlignment.Top
                            };
                            CS$<>8__locals2.$VB$Local_Card.Children.Add(myIconButton);
                            myIconButton.Click += delegate(object a0, EventArgs a1)
                            {
                                base._Lambda$__1((MyIconButton)a0, a1);
                            };
                            if (CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Loader.State == ModBase.LoadState.Failed)
                            {
                                CS$<>8__locals2.$VB$Local_Card.Tag = null;
                                this.TaskRefresh(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Loader);
                            }
                        }
                        catch (Exception ex4)
                        {
                            Logger.Error(ex4, "添加任务管理卡片失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSpeedLeft.xaml.vb");
                        }
                    }
                    IL_08A1:;
                }
                catch (Exception ex5)
                {
                    Logger.Error(ex5, "刷新任务管理显示失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSpeedLeft.xaml.vb");
                }
            }
        }

        public void TaskRemove(object Loader)
        {
            if (this.RightCards.ContainsKey(Conversions.ToString(NewLateBinding.LateGet(Loader, null, "Name", new object[0], null, null, null))))
            {
                ModBase.RunInUiWait(delegate
                {
                    Grid grid = this.RightCards[Conversions.ToString(NewLateBinding.LateGet(Loader, null, "Name", new object[0], null, null, null))];
                    ModMain.FrmSpeedRight.PanMain.Children.Remove(grid);
                    this.RightCards.Remove(Conversions.ToString(NewLateBinding.LateGet(Loader, null, "Name", new object[0], null, null, null)));
                    Logger.Info(string.Format("移除任务管理卡片：{0}", RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(Loader, null, "Name", new object[0], null, null, null))), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSpeedLeft.xaml.vb");
                });
            }
        }

        private void TryReturnToHome()
        {
            if (ModMain.FrmSpeedRight.PanMain.Children.Count == 0 && ModMain.FrmMain.PageCurrent == FormMain.PageType.TaskManager)
            {
                ModMain.FrmMain.PageBack();
            }
        }

        internal virtual TextBlock LabProgress
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabSpeed
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabFile
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabThread
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        private const int WatcherInterval = 300;

        private bool IsLoad;

        private readonly Dictionary<string, MyCard> RightCards;
    }
}
