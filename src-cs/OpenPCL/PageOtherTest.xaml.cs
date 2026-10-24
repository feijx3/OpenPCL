using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using MeloongCore;
using MeloongCore.Extensions;
using MeloongCore.Wpf;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using PCL.Our;
using PCLCS;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageOtherTest : OurPageRight
    {
        public PageOtherTest()
        {
            base.Loaded += this._Lambda___R1_1;
            this.IsLoad = false;
            this.IsCaveEnabled = true;
            this.InitializeComponent();
            this.BtnJrrp_WithEvents = this.BtnJrrp;
            this.BtnMemory_WithEvents = this.BtnMemory;
            this.BtnClear_WithEvents = this.BtnClear;
            this.BtnClick_WithEvents = this.BtnClick;
            this.CardCave_WithEvents = this.CardCave;
            this.BtnCave_WithEvents = this.BtnCave;
            this.TextDownloadUrl_WithEvents = this.TextDownloadUrl;
            this.TextDownloadFolder_WithEvents = this.TextDownloadFolder;
            this.TextDownloadName_WithEvents = this.TextDownloadName;
            this.BtnDownloadSelect_WithEvents = this.BtnDownloadSelect;
            this.BtnDownloadStart_WithEvents = this.BtnDownloadStart;
            this.BtnDownloadOpen_WithEvents = this.BtnDownloadOpen;
        }

        private void MeLoaded()
        {
            if (!this.IsLoad)
            {
                this.IsLoad = true;
                this.TextDownloadFolder.Text = Settings.Get<string>("CacheDownloadFolder", null);
                this.TextDownloadFolder.Validate();
                if (!this.TextDownloadFolder.IsValidated || Operators.CompareString(this.TextDownloadFolder.Text, "", false) == 0)
                {
                    this.TextDownloadFolder.Text = Paths.Base + "PCL\\MyDownload\\";
                }
                this.TextDownloadFolder.Validate();
                this.TextDownloadName.Validate();
            }
        }

        private void SaveCacheDownloadFolder()
        {
            Settings.Set("CacheDownloadFolder", this.TextDownloadFolder.Text, null);
            ((ValidateFileName)this.TextDownloadName.ValidateRules.First<Validate>()).ParentFolder = this.TextDownloadFolder.Text;
            this.TextDownloadName.Validate();
        }

        private void StartButtonRefresh()
        {
            this.BtnDownloadStart.IsEnabled = this.TextDownloadFolder.IsValidated && this.TextDownloadUrl.IsValidated && this.TextDownloadName.IsValidated;
            this.BtnDownloadOpen.IsEnabled = this.TextDownloadFolder.IsValidated;
        }

        private void TextDownloadUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && this.BtnDownloadStart.IsEnabled)
            {
                this.BtnDownloadStart_Click();
            }
        }

        private void TextDownloadUrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            int num = 0;
            int num4;
            object obj;
            int num2 = 0;
            int __onErrorFrom_IL_00BA = -1;
            ONERR_IL_00BA:
            try
            {
                if (__onErrorFrom_IL_00BA >= 0)
                {
                    if (__onErrorFrom_IL_00BA < 2) goto IL_0007;
                    if (__onErrorFrom_IL_00BA < 3) goto IL_0039;
                    goto IL_00BA;
                }
                IL_0000:
                ProjectData.ClearProjectError();
                num = 1;
                IL_0007:
                num2 = 2;
                if (Operators.CompareString(this.TextDownloadName.Text, "", false) != 0 || Operators.CompareString(this.TextDownloadUrl.Text, "", false) == 0)
                {
                    goto IL_005B;
                }
                IL_0039:
                num2 = 3;
                this.TextDownloadName.Text = PathUtils.GetLastPart(WebUtility.UrlDecode(this.TextDownloadUrl.Text));
                IL_005B:
                goto IL_00BA;
            }
            catch (Exception) when (num != 0)
            {
                __onErrorFrom_IL_00BA = num2;
                goto ONERR_IL_00BA;
            }
            IL_00BA:;
        }

        private void BtnDownloadStart_Click()
        {
            PageOtherTest.StartCustomDownload(this.TextDownloadUrl.Text, this.TextDownloadName.Text, this.TextDownloadFolder.Text);
            this.TextDownloadUrl.Text = "";
            this.TextDownloadUrl.Validate();
            this.TextDownloadUrl.ForceShowAsSuccess();
            this.TextDownloadName.Text = "";
            this.TextDownloadName.Validate();
            this.TextDownloadName.ForceShowAsSuccess();
            this.StartButtonRefresh();
        }

        public static void StartCustomDownload(string Url, string FileName, string Folder = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Folder))
                {
                    Folder = Dialogs.SaveFile("选择文件保存位置", FileName, null, null);
                    if (Folder == null)
                    {
                        return;
                    }
                    FileName = PathUtils.GetLastPart(Folder);
                    Folder = PathUtils.RemoveLastPart(Folder);
                }
                Folder = PathUtils.AddSlashSuffix(Folder.Replace("/", "\\"));
                try
                {
                    DirectoryUtils.Create(Folder);
                    ModBase.CheckPermissionWithException(Folder);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, string.Format("访问文件夹失败（{0}）", Folder), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherTest.xaml.vb");
                    return;
                }
                Logger.Info(string.Format("自定义下载文件名：{0}", FileName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherTest.xaml.vb");
                Logger.Info(string.Format("自定义下载文件目标：{0}", Folder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherTest.xaml.vb");
                int uuid = ModBase.GetUuid();
                ModNet.LoaderDownload loaderDownload = new ModNet.LoaderDownload("自定义下载文件：" + FileName + " ", new List<ModNet.NetFile>
                {
                    new ModNet.NetFile(new string[] { Url }, Folder + FileName, null, true)
                });
                ModLoader.LoaderCombo<int> loaderCombo = new ModLoader.LoaderCombo<int>("自定义下载 (" + Conversions.ToString(uuid) + ") ", new ModLoader.LoaderBase[] { loaderDownload });
                loaderCombo.OnStateChanged = ((PageOtherTest._Closure___._IR9_2 == null) ? (PageOtherTest._Closure___._IR9_2 = new Action<ModLoader.LoaderBase>(PageOtherTest._Closure___._I._Lambda___R9_2)) : PageOtherTest._Closure___._IR9_2);
                loaderCombo.Start(null, false);
                ModLoader.LoaderTaskbarAdd<int>(loaderCombo);
                ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                ModMain.FrmMain.BtnExtraDownload.Ribble();
            }
            catch (Exception ex2)
            {
                Logger.Error(ex2, "开始自定义下载失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherTest.xaml.vb");
            }
        }

        private void BtnDownloadSelect_Click(object sender, EventArgs e)
        {
            string text = Dialogs.SelectFolder("选择目标文件夹", false, null).FirstOrDefault<string>();
            if (text != null)
            {
                this.TextDownloadFolder.Text = text;
            }
        }

        private void BtnDownloadOpen_Click()
        {
            try
            {
                string text = this.TextDownloadFolder.Text;
                DirectoryUtils.Create(text);
                ModBase.StartProcess(text, "");
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "打开下载文件夹失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherTest.xaml.vb");
            }
        }

        private static void DownloadState(ModLoader.LoaderCombo<int> Loader)
        {
            int num = 0;
            int num4;
            object obj;
            int num2 = 0;
            int __onErrorFrom_IL_010D = -1;
            ONERR_IL_010D:
            try
            {
                if (__onErrorFrom_IL_010D >= 0)
                {
                    if (__onErrorFrom_IL_010D < 2) goto IL_0007;
                    goto IL_010D;
                }
                IL_0000:
                ProjectData.ClearProjectError();
                num = 1;
                IL_0007:
                num2 = 2;
                switch (Loader.State)
                {
                case ModBase.LoadState.Finished:
                    IL_0026:
                    num2 = 4;
                    ModMain.Hint(Loader.Name + "完成！", ModMain.HintType.Green, true);
                    IL_003F:
                    num2 = 5;
                    Interaction.Beep();
                    break;
                case ModBase.LoadState.Failed:
                    IL_0048:
                    num2 = 7;
                    Logger.Error(Loader.Error, string.Format("{0}失败", Loader.Name), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherTest.xaml.vb");
                    IL_006B:
                    num2 = 8;
                    Interaction.Beep();
                    break;
                case ModBase.LoadState.Canceled:
                    IL_0074:
                    num2 = 10;
                    ModMain.Hint(Loader.Name + "已取消！", ModMain.HintType.Blue, true);
                    break;
                }
                IL_008E:
                goto IL_010D;
            }
            catch (Exception) when (num != 0)
            {
                __onErrorFrom_IL_010D = num2;
                goto ONERR_IL_010D;
            }
            IL_010D:;
        }

        private void BtnClick_Click(object sender, EventArgs e)
        {
            int num = 0;
            int num5;
            object obj;
            int num2 = 0;
            List<int> list = default;
            List<int>.Enumerator enumerator = default;
            int __onErrorFrom_IL_0623 = -1;
            ONERR_IL_0623:
            try
            {
                if (__onErrorFrom_IL_0623 >= 0)
                {
                    if (__onErrorFrom_IL_0623 < 2) goto IL_0007;
                    if (__onErrorFrom_IL_0623 < 7) goto IL_0039;
                    if (__onErrorFrom_IL_0623 < 8) goto IL_005D;
                    if (__onErrorFrom_IL_0623 < 9) goto IL_006C;
                    if (__onErrorFrom_IL_0623 < 10) goto IL_0078;
                    if (__onErrorFrom_IL_0623 < 13) goto IL_00B6;
                    if (__onErrorFrom_IL_0623 < 14) goto IL_00ED;
                    if (__onErrorFrom_IL_0623 < 61) goto IL_04B6;
                    if (__onErrorFrom_IL_0623 < 62) goto IL_04C6;
                    goto IL_0623;
                }
                IL_0000:
                ProjectData.ClearProjectError();
                num = 1;
                IL_0007:
                num2 = 2;
                list = new List<int> { ModBase.RandomInteger(0, 7) };
                for (;;)
                {
                    IL_002D:
                    num2 = 4;
                    if (ModBase.RandomInteger(0, 1) != 1)
                    {
                        break;
                    }
                    IL_001E:
                    num2 = 5;
                    list.Add(ModBase.RandomInteger(1, 6));
                }
                IL_0039:
                num2 = 7;
                if (DateTime.Now.Month != 4 || DateTime.Now.Day != 1)
                {
                    goto IL_006C;
                }
                IL_005D:
                num2 = 8;
                list = new List<int> { 7 };
                IL_006C:
                num2 = 9;
                if (!list.Contains(1))
                {
                    goto IL_00B6;
                }
                IL_0078:
                num2 = 10;
                if (ModMain.OurMsgBox("当暴露在点击确定后的场景时，有极小部分人群会引发癲痫。这种情形可能是由于某些未查出的癫病症状引起，即使该人员并没有患癫痫病史也有可能造成此类病症。如果您的家人或任何家庭成员曾有过类似症状，请在点击确定前咨询您的医生或医师。如果您在稍后出现任何症状，包括头晕、目眩、眼部或肌肉抽搐、失去意识、失去方向感、抽搐或出现任何自己无法控制的动作，请立即关闭 PCL 并咨询您的医生或医师。\\n这是最后的警告，是否继续操作？".Replace("\\n", "\r\n"), "警告", "确定", "取消", "", true, true, false, null, null, null) == 2)
                {
                    goto IL_04D5;
                }
                goto IL_00ED;
                IL_00B6:
                num2 = 13;
                ModMain.OurMsgBox("PCL 作者不会受理由于点击千万别点造成的任何 Bug。\\n这是最后的警告，是否继续操作？".Replace("\\n", "\r\n"), "警告", "确定", "确定", "确定", true, true, false, null, null, null);
                IL_00ED:
                num2 = 14;
                enumerator = list.GetEnumerator();
                checked
                {
                    while (enumerator.MoveNext())
                    {
                        int num3 = enumerator.Current;
                        IL_0106:
                        num2 = 15;
                        switch (num3)
                        {
                        case 0:
                            IL_0135:
                            num2 = 17;
                            ModSecret.ThemeDontClick = 1;
                            IL_013E:
                            num2 = 18;
                            ModSecret.ThemeRefresh(-1);
                            break;
                        case 1:
                            IL_014C:
                            num2 = 20;
                            ModSecret.ThemeDontClick = 2;
                            IL_0155:
                            num2 = 21;
                            ModSecret.ThemeRefresh(-1);
                            break;
                        case 2:
                            IL_0163:
                            num2 = 23;
                            if (ModMain.FrmMain.PanBack.LayoutTransform != null)
                            {
                                IL_0177:
                                num2 = 24;
                                ModMain.FrmMain.PanBack.RenderTransformOrigin = new Point(1.25, 1.25);
                            }
                            IL_01A0:
                            num2 = 25;
                            switch (ModBase.RandomInteger(0, 2))
                            {
                            case 0:
                                IL_01C4:
                                num2 = 27;
                                ModMain.FrmMain.PanBack.RenderTransform = new ScaleTransform(1.0, -1.0);
                                break;
                            case 1:
                                IL_01F2:
                                num2 = 29;
                                ModMain.FrmMain.PanBack.RenderTransform = new ScaleTransform(-1.0, -1.0);
                                break;
                            case 2:
                                IL_0220:
                                num2 = 31;
                                ModMain.FrmMain.PanBack.RenderTransform = new ScaleTransform(-1.0, 1.0);
                                break;
                            }
                            break;
                        case 3:
                            IL_024E:
                            num2 = 34;
                            ModMain.FrmMain.IsSizeSaveable = false;
                            IL_025C:
                            num2 = 35;
                            ModMain.FrmMain.PanBack.LayoutTransform = new ScaleTransform(2.5, 2.5);
                            IL_0285:
                            num2 = 36;
                            ModMain.FrmMain.Height = (double)(OurWpfExtension.Computer.Screen.WorkingArea.Height - 200);
                            IL_02B1:
                            num2 = 37;
                            ModMain.FrmMain.Width = (double)(OurWpfExtension.Computer.Screen.WorkingArea.Width - 200);
                            IL_02DD:
                            num2 = 38;
                            ModMain.FrmMain.Left = 0.0;
                            IL_02F3:
                            num2 = 39;
                            ModMain.FrmMain.Top = 0.0;
                            IL_0309:
                            num2 = 40;
                            ModAnimation.AniStop("Don't Click Scale");
                            break;
                        case 4:
                            IL_031B:
                            num2 = 42;
                            ModMain.FrmMain.IsSizeSaveable = false;
                            IL_0329:
                            num2 = 43;
                            ModMain.FrmMain.RenderTransform = new ScaleTransform();
                            IL_033B:
                            num2 = 44;
                            ModAnimation.AniStart(ModAnimation.AaScaleTransform(ModMain.FrmMain, -0.85, 5000, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.ExtraStrong), false), "Don't Click Scale", false);
                            IL_0369:
                            num2 = 45;
                            ModAnimation.AniStop("Don't Click Rotate");
                            break;
                        case 5:
                            IL_037B:
                            num2 = 47;
                            ModMain.FrmMain.RenderTransform = new RotateTransform();
                            IL_038D:
                            num2 = 48;
                            ModAnimation.AniStart(ModAnimation.AaRotateTransform(ModMain.FrmMain, (double)(1000000 * ModBase.RandomOne<int>(new int[] { 1, -1 })), 10000000, 0, null, false), "Don't Click Rotate", false);
                            IL_03C7:
                            num2 = 49;
                            ModAnimation.AniStop("Don't Click Scale");
                            break;
                        case 6:
                            IL_03D9:
                            num2 = 51;
                            ModMain.FrmMain.IsSizeSaveable = false;
                            IL_03E7:
                            num2 = 52;
                            if (ModBase.RandomInteger(0, 1) != 0)
                            {
                                IL_03F6:
                                num2 = 53;
                                ModAnimation.AniStart(ModAnimation.AaX(ModMain.FrmMain, (double)(10000000 * ModBase.RandomOne<int>(new int[] { 1, -1 })), 30000000, 0, null, false), "Don't Click Move X", false);
                            }
                            else
                            {
                                IL_0432:
                                num2 = 55;
                                ModAnimation.AniStart(ModAnimation.AaY(ModMain.FrmMain, (double)(10000000 * ModBase.RandomOne<int>(new int[] { 1, -1 })), 50000000, 0, null, false), "Don't Click Move Y", false);
                            }
                            IL_046C:
                            num2 = 56;
                            ModBase.RunInThread((PageOtherTest._Closure___._I13_0 == null) ? (PageOtherTest._Closure___._I13_0 = new Action(PageOtherTest._Closure___._I._Lambda___13_0)) : PageOtherTest._Closure___._I13_0);
                            break;
                        case 7:
                            IL_049A:
                            num2 = 58;
                            ModBase.OpenWebsite("https://www.bilibili.com/video/BV1GJ411x7h7");
                            break;
                        }
                        IL_04A7:
                        num2 = 60;
                    }
                }
                IL_04B6:
                num2 = 61;
                ((IDisposable)enumerator).Dispose();
                IL_04C6:
                num2 = 62;
                this.BtnClick.Visibility = Visibility.Collapsed;
                IL_04D5:
                goto IL_0623;
            }
            catch (Exception) when (num != 0)
            {
                __onErrorFrom_IL_0623 = num2;
                goto ONERR_IL_0623;
            }
            IL_0623:;
        }

        public static void Jrrp()
        {
            checked
            {
                int num = (int)Math.Round(Math.Abs(unchecked(string.Format("asdfgbn{0}12#3$45{1}IUY", DateTime.Now.DayOfYear, DateTime.Now.Year).GetStableHashCode() / 3.0 + string.Format("QWERTY{0}0*8&6{1}kjhg", ModBase.Identify, DateTime.Now.Day).GetStableHashCode() / 3.0) / 527.0) % 1001.0);
                int num2;
                if (num >= 970)
                {
                    num2 = 100;
                }
                else
                {
                    num2 = (int)Math.Round(unchecked((double)num / 969.0 * 99.0));
                }
                string text;
                if (num2 == 100)
                {
                    text = "！100！100！！！！！" + (ModSecret.ThemeUnlock(13, false, null) ? "\r\n隐藏主题 欧皇彩 已解锁！" : "");
                }
                else if (num2 >= 98)
                {
                    text = "！差点就到 100 了呢……";
                }
                else if (num2 >= 90)
                {
                    text = "！好评如潮！";
                }
                else if (num2 >= 65)
                {
                    text = "！今天运气不错呢！";
                }
                else if (num2 > 50)
                {
                    text = "！还行啦，还行啦。";
                }
                else if (num2 == 50)
                {
                    text = "！五五开……";
                }
                else if (num2 >= 40)
                {
                    text = "！勉强还行吧……？";
                }
                else if (num2 >= 20)
                {
                    text = "！呜……";
                }
                else if (num2 >= 11)
                {
                    text = "？！不会吧……";
                }
                else if (num2 >= 1)
                {
                    text = "……（是百分制哦）";
                }
                else
                {
                    text = "？！";
                    if (ModMain.OurMsgBox("在查看结果前，请先同意以下附加使用条款：\r\n\r\n1. 我知晓并了解 PCL 的今日人品功能完全没有出 Bug。\r\n2. PCL 不对使用本软件所间接造成的一切财产损失（如砸电脑等）等负责。", "今日人品 - 附加使用条款", "同意并查看结果", "再见", "", true, true, false, null, null, null) == 2)
                    {
                        return;
                    }
                }
                ModMain.OurMsgBox((ModMain.FrmMain.IsSystemTimeChanged ? "在这个时候，你的人品值会是：" : "你今天的人品值是：") + Conversions.ToString(num2) + text, (ModMain.FrmMain.IsSystemTimeChanged ? "今日人品? - " : "今日人品 - ") + DateTime.Now.ToString("yyyy'/'M'/'d"), "我知道了", "", "", num2 < 20, true, false, null, null, null);
            }
        }

        public static void RubbishClear()
        {
            ModBase.RunInUi((PageOtherTest._Closure___._I15_0 == null) ? (PageOtherTest._Closure___._I15_0 = new Action(PageOtherTest._Closure___._I._Lambda___15_0)) : PageOtherTest._Closure___._I15_0, false);
            ModBase.RunInNewThread((PageOtherTest._Closure___._I15_1 == null) ? (PageOtherTest._Closure___._I15_1 = new Action(PageOtherTest._Closure___._I._Lambda___15_1)) : PageOtherTest._Closure___._I15_1, "Rubbish Clear", ThreadPriority.Normal);
        }

        private void BtnMemory_Click()
        {
            ModBase.RunInThread((PageOtherTest._Closure___._I16_0 == null) ? (PageOtherTest._Closure___._I16_0 = new Action(PageOtherTest._Closure___._I._Lambda___16_0)) : PageOtherTest._Closure___._I16_0);
        }

        public static void MemoryOptimize(bool ShowHint)
        {
            checked
            {
                if (PageOtherTest.IsMemoryOptimizeRunning)
                {
                    if (ShowHint)
                    {
                        ModMain.Hint("内存优化尚未结束，请稍等！", ModMain.HintType.Blue, true);
                        return;
                    }
                }
                else
                {
                    PageOtherTest.IsMemoryOptimizeRunning = true;
                    if (ShowHint)
                    {
                        ModMain.Hint("正在进行内存优化……", ModMain.HintType.Blue, true);
                    }
                    long num;
                    if (WindowsUtils.HasAdminRole())
                    {
                        num = (long)OurWpfExtension.Computer.Info.AvailablePhysicalMemory;
                        try
                        {
                            PageOtherTest.MemoryOptimizeInternal(ShowHint);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, "内存优化失败", ShowHint ? LogBehavior.Toast : LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherTest.xaml.vb");
                            return;
                        }
                        finally
                        {
                            PageOtherTest.IsMemoryOptimizeRunning = false;
                        }
                        num = Convert.ToInt64(decimal.Subtract(new decimal(OurWpfExtension.Computer.Info.AvailablePhysicalMemory), new decimal(num)));
                    }
                    else
                    {
                        Logger.Info("没有管理员权限，将以命令行方式进行内存优化", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherTest.xaml.vb");
                        try
                        {
                            num = unchecked((long)ModBase.RunAsAdmin("--memory")) * 1024L;
                        }
                        catch (Exception ex2)
                        {
                            Logger.Warn(ex2, "命令行形式内存优化失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherTest.xaml.vb");
                            if (ShowHint)
                            {
                                ModMain.Hint(string.Concat(new string[]
                                {
                                    "获取管理员权限失败，请尝试右键 PCL，选择 ",
                                    Conversions.ToString(ModBase.vbLQ),
                                    "以管理员身份运行",
                                    Conversions.ToString(ModBase.vbRQ),
                                    "！"
                                }), ModMain.HintType.Red, true);
                            }
                            return;
                        }
                        finally
                        {
                            PageOtherTest.IsMemoryOptimizeRunning = false;
                        }
                        if (num < 0L)
                        {
                            return;
                        }
                    }
                    string text = StringUtils.FormatByteSize((long)OurWpfExtension.Computer.Info.AvailablePhysicalMemory);
                    Logger.Info(string.Format("内存优化完成，可用内存改变量：{0}，大致剩余内存：{1}", StringUtils.FormatByteSize(num), text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherTest.xaml.vb");
                    if (num > 0L)
                    {
                        if (ShowHint)
                        {
                            ModMain.Hint(string.Format("内存优化完成，可用内存增加了 {0}，目前剩余内存 {1}！", StringUtils.FormatByteSize((long)Math.Round(unchecked((double)num * 0.8))), text), ModMain.HintType.Green, true);
                            return;
                        }
                    }
                    else if (ShowHint)
                    {
                        ModMain.Hint(string.Format("内存优化完成，已经优化到了最佳状态，目前剩余内存 {0}！", text), ModMain.HintType.Blue, true);
                    }
                }
            }
        }

        public static void MemoryOptimizeInternal(bool ShowHint)
        {
            if (!WindowsUtils.HasAdminRole())
            {
                throw new Exception("内存优化功能需要管理员权限！\r\n如果需要自动以管理员身份启动 PCL，可以右键 PCL，打开 属性 → 兼容性 → 以管理员身份运行此程序。");
            }
            Logger.Info("获取内存优化权限", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherTest.xaml.vb");
            using (WindowsIdentity current = WindowsIdentity.GetCurrent(TokenAccessLevels.Query | TokenAccessLevels.AdjustPrivileges))
            {
                PageOtherTest.PrivilegeToken privilegeToken = default(PageOtherTest.PrivilegeToken);
                privilegeToken.PrivilegeCount = 1;
                privilegeToken.Luid = 0L;
                privilegeToken.Attributes = 2;
                string text = null;
                string text2 = "SeProfileSingleProcessPrivilege";
                if (PageOtherTest.LookupPrivilegeValue(ref text, ref text2, ref privilegeToken.Luid))
                {
                    IntPtr token = current.Token;
                    bool flag = false;
                    int num = Marshal.SizeOf<PageOtherTest.PrivilegeToken>(privilegeToken);
                    IntPtr zero = IntPtr.Zero;
                    int num2 = 0;
                    if (PageOtherTest.AdjustTokenPrivileges(token, flag, ref privilegeToken, num, ref zero, ref num2) && Marshal.GetLastWin32Error() == 0)
                    {
                        goto IL_00B6;
                    }
                }
                throw new Exception(string.Format("获取内存优化权限失败（错误代码：{0}）", Marshal.GetLastWin32Error()));
            }
            IL_00B6:
            int num3 = 2;
            checked
            {
                int num4;
                for (;;)
                {
                    Logger.Info(string.Format("内存优化操作 {0} 开始", num3), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherTest.xaml.vb");
                    GCHandle gchandle = GCHandle.Alloc(num3, GCHandleType.Pinned);
                    num4 = (int)PageOtherTest.NtSetSystemInformation(80, gchandle.AddrOfPinnedObject(), Marshal.SizeOf<int>(num3));
                    gchandle.Free();
                    if (num4 != 0)
                    {
                        break;
                    }
                    num3++;
                    if (num3 > 4)
                    {
                        return;
                    }
                }
                throw new Exception(string.Format("内存优化操作 {0} 失败（错误代码：{1}）", num3, num4));
            }
        }

        [DllImport("ntdll.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern uint NtSetSystemInformation(int SystemInformationClass, IntPtr SystemInformation, int SystemInformationLength);

        [DllImport("advapi32.dll", CharSet = CharSet.Ansi, EntryPoint = "LookupPrivilegeValueA", ExactSpelling = true, SetLastError = true)]
        private static extern bool LookupPrivilegeValue([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpSystemName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpName, ref long lpLuid);

        [DllImport("advapi32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, ref PageOtherTest.PrivilegeToken NewState, int BufferLength, ref IntPtr PreviousState, ref int ReturnLength);

        private void BtnCave_Click(object sender, EventArgs e)
        {
            ModBase.OpenWebsite("https://jinshuju.net/f/esXHQF");
        }

        private void CaveHand()
        {
            this.BtnCave.Visibility = ((ModSecret.CurrentRank >= DonationRank.Rank23) ? Visibility.Visible : Visibility.Collapsed);
        }

        private void CardCave_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsCaveEnabled)
            {
                string randomCave = PageOtherTest.GetRandomCave();
                string text = this.LabCave.Text;
                this.LabCave.Text = randomCave;
                this.IsCaveEnabled = false;
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaOpacity(this.LabCave, -1.0, 5, 0, null, false),
                    ModAnimation.AaOpacity(this.LabCave, 1.0, 5, 30, null, false),
                    ModAnimation.AaOpacity(this.LabCave, -1.0, 5, 70, null, false),
                    ModAnimation.AaOpacity(this.LabCave, 0.9, 5, 85, null, false),
                    ModAnimation.AaOpacity(this.LabCave, -1.0, 5, 125, null, false),
                    ModAnimation.AaOpacity(this.LabCave, 0.8, 5, 145, null, false),
                    ModAnimation.AaOpacity(this.LabCave, -1.0, 5, 165, null, false),
                    ModAnimation.AaOpacity(this.LabCave, 0.7, 5, 195, null, false),
                    ModAnimation.AaOpacity(this.LabCave, -1.0, 5, 210, null, false),
                    ModAnimation.AaOpacity(this.LabCave, 0.5, 5, 235, null, false),
                    ModAnimation.AaOpacity(this.LabCave, -1.0, 5, 250, null, false),
                    ModAnimation.AaOpacity(this.LabCave, 1.0, 1, 400, null, true),
                    ModAnimation.AaTextAppear(this.LabCave, false, true, 60, 400, null, false),
                    ModAnimation.AaCode(new ThreadStart(this._Lambda___27_0), 0, true)
                }, "Cave", false);
                this.LabCave.Text = text;
            }
        }

        public static string GetRandomCave()
        {
            return ModBase.RandomOne<string>(new string[]
            {
                ModBase.RandomOne<string>(new string[]
                {
                    "来硬的！", "钻石！", "我们需要再深入些！", "结束了？", "见鬼去吧！", "君临天下！", "与火共舞！", "本地的酿造厂！", "为什么会变成这样呢？", "信标工程师！",
                    "不稳定的同盟！", "天空即为极限！", "甜蜜的梦！", "探索的时光！", "狙击手的对决！", "这是？工作台！", "永恒的伙伴！", "腥味十足的生意！", "结束了。", "开始了？",
                    "这交易不错！", "你的世界！", "/summon Creeper ~ ~ ~ {Fuse:0}", "MC-98587!", "紫黑格子波纹疾走！", "命令方块不适合作为武器！", "Don't try Minecraft Legend!", "新增了一堆 Bug！", "Also try Create!", "这是刻意的游戏设计！",
                    "附魔神圣橡胶树树苗！", "有频道的 AE 不是好 AE！", "你好中国！", "/give @a hugs 64", "这是特性！", "Minecraft Legend!", "Creeper?", "Minecraft 2.0!", "Hello, Herobrine!", "Herobrine...xia?",
                    "It's a FEATURE! Not a BUG!", "我 Mojang 绝不跳票！", "苦力怕！不是爬行者！", "蠢虫？毒虫？蠹虫？", "粉色羊是隐性纯合子！", "Can't keep up! Did the system time change or is the server overloaded?", "比钻石更强！", "BUGJANG!", "猪灵劲曲！", "床里面藏着 TNT！",
                    "这么多留言中你偏偏看见我的，爱你么么哒 ╰(\uffe3ω\uffe3ｏ)！", "我铸剑，你冰球，我们都有光明的未来！", "Deadline 是第一生产力！", "核电，轻而易举啊！", "今天也努力当咸鱼了呢（瘫）", "MC 是我们的青春啊！", "烧烤炉制作教程：机械动力+瓦尔基里+我的世界+低配电脑", "(=\uff40ω \u00b4=)", "Also try 新闻主页！", "由于目标计算机积极拒绝，无法连接。",
                    "夹子！", "菌变，极限模式，随机种子，直接开始！", "猫酱补药载旧活新整了！", "龙猫得了 MVP！", "我的世界，启动！", "Look familiar？见此消息者，视为自愿加入超级地球！", "《龙猫你玩 MC 吗》", "正于此地，愿你找到想要的书。", "Also try SCP-CN-660-J!", "正于此地，愿你找到想要的 Mod 和整合包！",
                    "Also try SCP-CN-048-J!", "愿光之种在每个人的心中萌发！", "任天堂就是世界的主宰！", "比撒的给台！", "To be continued.", "脑子放假去了！", "黄瓜鸡蛋粥，好吃又美味！", "刷到此条的，一切都会如愿，所想所念一定能成功！", "当你觉得我咕了，但是我没咕，这也是一种咕！", "没有钻石块的可以用下界合金块代替！",
                    "心脏停跳文学社！", "Vamos!", "50382 警告！", "Priority Crash Launcher!", "自己都不相信自己，那怎么会成功？", "Minecraft saves the world!", "那就当做没有 Bug 好了！", "试试泰拉瑞亚！", "向骰子低头！", "隐藏主题解锁给点提示行不行啊 QAQ！",
                    "破产了启动器！", "泡菜龙启动器！", "劈柴驴启动器！", "碰瓷狼启动器！", "等泰拉瑞亚 2、半条命 3、巫师 4、辐射 5、老滚 6 发售了我就能玩上了，我真幸运！", "Keep your DETERMINATION.", "我东方永不过气！", "PPCCLL!", "想拉萌新朋友一起玩 MC 的话选一些简单的整合包更好入手哦！上手有任务指导合成有 JEI 内容还比原版丰富，肯定会喜欢玩的！", "陌生人，虽然不认识你，但祝你早安，午安，晚安，事事平安 :)"
                }),
                ModBase.RandomOne<string>(new string[]
                {
                    "希望这个游戏可以给你带来更多的快乐！", "Also try Celeste!", "Also try The Witness!", "结束了？结束了。真结束了吗？如结束。", "一定不要贪小便宜去非微软平台购买 MC 账号，百分之九十五是黑卡账户！", "要恰饭的嘛！", "人生没有标准答案，去做自己认为正确的事！", "帅得体面，猫得明白！", "王泪天下第一！", "FLAG IS WIN!",
                    "凌波微步，快乐的舞步！", "这个叫 TAS 的人是不是开挂了？", "For Super Earth!", "爽啊！", "支持正版 MC，加油！", "失之东隅，收之桑榆。", "让我们拜请白日铸炉！", "我再发回声洞我就是勾，骗你的我本来就是 qwq", "你好，我是末影龙，如果你看到这条留言，说明我已经被你打败了 awa，不过游戏还没结束。还有一个叫凋灵的 BOSS 等着你，出发吧史蒂夫，祝君好运！", "请为坠落的龙猫取名……",
                    "小友，你在期待什么？", "我说有认知偏差。我说有认知偏", "你的旅途……到此为止……", "Also try PCL CE!", "Also try HMCL!", "那么，我们开始吧？", "突然想起来有回声洞！", "我可以一个人很好，但有你会更好。", "You should try our sister launcher, PLC!", "AAA专业投稿",
                    "咕咕嘎嘎！", "众所周知，我的世界的开发商是 BugJump。", "别杀怪物，你这个海豚！", "我永远都不相信我的今日人品会是 0！", "千万得点！", "是的，这是一条用来凑数的信息。", "你说的对但是你说的对所以你说的对！", "你必封印在众人梦中散布瘟疫的障目之光。", "看到这个评论的人请去喝一杯水！", "相信我，一切都会变好的！",
                    "Professional Crush Launcher!", "长风破浪会有时，直挂云帆济沧海！", "要断章取义 ——取自《不要断章取义》", "Ciallo～(∠・ω< )⌒☆", "我跟你讲你这样是要被夹的！", "当重新创建一个存档，不要忘记创建这个存档最初的意义。", "Missing No.", "管管孩子，救救游戏！", "今天好累啊！", "我想要说的前人们都说过了！",
                    "希望人有事.jpg", "修了的叫 Bug，没修的叫特性。", "我想 PCL 玩家都是些很有趣的，很好的人，祝大家天天开心！", "我是绝对不会为了回声洞发电的……吗？", "多线程被卡住了不玩手机玩什么呀！（绝望）", "Ba+2Na=Banana", "你被强化了！快送！", "这里可以写广告嘛？", "你每天会忘记上千件事，为何不把这件事也忘记？", "我差一血打死怪，怪差一血不打死我！",
                    "不打钱就削土豆！", "众所周知，1+1=王！", "世界就是一个巨大的瑞士卷，有的人去得了瑞士，有的人只取得了卷。", "如果你能看到我这条，说明我们有缘，加我一起玩啊！", "PCL 服务器第一朵蘑菇云始于 Candy_Pink！", "龙腾猫跃 简称 龙猫 但 不是 龙猫", "世界和平！", "Minecraft 2.0 正在启动中！", "\"44 45 4C 45 54 45 20 53 59 53 54 45\\n4D 20 33 32 20 54 4F 20 57 49 4E !\"", "お－お－お－お－お－お－お－お－お－お－お－お",
                    "方！兴！衢！不！是！方！兴！衡！", "开学哩！", "--...-....-...- -.-.-.--..---.. --.-.--..-...-. -..----.--.....", "别点了！手不累吗？", "很好的 MOD 使我存档损坏", "叶底藏花一度，梦里踏雪几回。", "另类本身就是一种出类拔萃！", "麻辣鸡翅真好吃！", "咩！（你好！）", "已深度思考（用时 0 秒）\\n\\n服务器繁忙，请稍后再试。",
                    "For the Emperor!", "阿玛忒拉斯！", "如果你看到了这句话一定会认真的读下去，读到最后就会发现这句话没有任何意义，恭喜浪费了一分钟！", "Wake up.", "It's time to go to school.", "反复下载的是游戏，回不到的是童年。", "汉中乃北伐剑锋，文长可敢担太守之责？", "有的人毕业了，有的人还没开学。", "在 38 度的太阳底下放寒假。", "请输入文本",
                    "Made in Abyss!", "这就是被央视点名表扬的游戏吗？", "吃水不忘挖井人，不要忘记这超赞的 PCL 是龙猫在支撑……", "我的世界我做主！", "撞车欧灵打出93杠16的战绩得取10000分，出两次所向披靡一次聚变部署", "早上好，中午好，晚上好！", "希望能在一些地方留下自己的一点痕迹。", "如果你看到这个，说明我的存在是有意义的，我实现了我的价值，虽然很小。", "MC，启动！", "各个国家有各个国家的国歌（用粤语说",
                    "挖矿免费玩！", "宁可没用，不能没有！", "龙猫是龙猫，龙猫是龙猫，但龙猫不是龙猫！", "拜托，反复点击回声洞来看自己的投稿也太逊了！", "埋骨何须桑梓地，人生无处不青山。"
                }),
                ModBase.RandomOne<string>(new string[]
                {
                    "基岩真好吃！", "弱小和无知不是生存的障碍，傲慢才是。——《三体》", "在没有错误日志的情况下诊断任何问题无异于闭眼开车。", "有的说，但是没得说。", "希望可爱的龙猫猫能更容易看懂自己写的代码。", "PCL 的代码只有龙猫和上帝知道，然而龙猫忘记了。", "挖三填一：刚进入 MC 的玩家可能都不知道这个游戏存在危险，只知道东看看西瞅瞅，所以到了晚上的时候才知道 MC 的可怕。", "Also try Terraria.", "龙猫画的饼很大！", "事实证明，乱打序顺会不影响的人阅读。",
                    "你知道龙猫 MC 的 ID 是 LTCat 吗？", "据我所知，这些材料至少得用一背包或者一大箱子的石头，都有这么多石头了，干嘛还要去做刷石机？", "私はLです。", "龙腾猫跃 > 龙猫 > lm", "你知道吗：2022 年 3 月 10 日 Mojang 账号是最后自愿迁移至 Microsoft 账号的期限。", "日常踩雷！", "当你看到这条回声洞，你就看到了这条回声洞。", "《关于解锁档位为了投稿回声洞，然后发现不知道说啥这件事》", "不要尝试在岩浆里喝热水！", "博士，您还有许多事情需要处理，现在还不能休息哦。",
                    "真的有人会看这些吗？", "我的人品值很差，只有 1 的 100 倍（bushi）", "NeoForge yyds!", "Also try Minecraft with RTX!", "public static void main(String[] args) {}", "Ctrl + S", "腐竹，我刷沙机忘关了！", "反手一个超级加倍，闷声发大财。", "腐竹，我不小心在末地种了个蘑菇！", "↑↑↓↓←→←→BABA",
                    "腐竹，我忘记做处死装置了！", "38324？14122！", "我的评价是：我不予评价。", "建筑，我所欲也，红石，亦我所欲也；二者不可得兼……喊俩大佬就可以得兼了。", "我的梦想是天天咕咕咕（逃", "锟斤拷锟斤拷䵣笓靹攮濄魊！", "* It fills you with determination.", "点击千万别点，可以获得所有主题。", "I want to play a game.", "如果忧郁是种天赋，那我天赋异禀。",
                    "温馨提示：你可点击红色按钮，剩余要靠你的智慧啦~", "康神开播了？", "使用左键以和铁傀儡友好交流！", "PCL (\u309c-\u309c)つロ 亁杯~", "THE END IS NEVER THE END IS NEVER THE END IS NEVER THE END", "雄火龙又双叒叕被绿了。", "净他妈扯淡！", "乌拉！！！！！", "不要尝试带着猫挑衅苦力怕！", "Welcome to……O…PCL!",
                    "快来试试 PCL 下载器！", "这根本不是起泡胶制作教程！", "新的 Bug 删除了，旧的 Bug 增加了。", "我知道你想睡觉了，但是，不熬夜游戏不会健康。", "Death is not an escape.", "你知道吗？你不知道。", "欸我 10000000 Mods 的整合包打不开了，这启动器不行啊。", "你知道吗？人会对看到的文字进行自动排序。别读了，你再读一遍也是这样。", "回声~回声~回声~", "360：我 TM 觉得你很可疑",
                    "我讲两句，咳咳！饿了要吃饭，渴了要喝水，困了要睡觉，鼠了要埋掉。", "爷爷，你关注的龙猫终于更新啦！", "rm -rf /*", "半命无出三，故吾曰 G 胖不三。PCL 不出三，是龙猫不三乎？曰：非然也。", "让人类永远保持理智是一种奢望！", "点千万别点会发生好玩的事！", "PCL 是我的世界启动器，不是下载器！", "我已启动。——鸡煲", "俺不中嘞，这网咋这么卡嘞 QAQ", "hi，我是 QZTJX，很高兴你能看到我的留言。",
                    "KO NO DIO DA!", "我是史蒂夫，我为我童年代言。", "* 还剩 17 个。=)", "哇！金色传说！", "赶紧去给我开 EVA！", "我绝对不会因为回声洞给龙腾猫跃发电的！", "我绝对不会发回声洞的！", "爷想被夹！", "淡黄的长裙~蓬松的头发~", "奇怪的知识增加了！",
                    "小朋友，你是否有很多问号？", "生鱼忧患，死鱼安乐。", "在？不在。", "Ease my mind.", "小善，如果你是女孩子的话啊……", "啊啊啊啊，宝宝你是一个一个一个……", "Get Over Here!", "孩子们，假期要开始加速了，没抖音还问！", "震惊龙猫一整天！", "你看这个光影多棒啊，开一下试试，反正电脑好，诶我主机怎么烧",
                    "PCL 高速下载器！", "右键开始游戏按钮是没有彩蛋的！", "> 点此启动内置小游戏 <", "衬衫的价格是九镑十五便士。", "「胜败乃兵家常事，但是下一次我们会赢回来的！」", "少女折寿中... φ(*\uffe30\uffe3)", "然则天下之事，但知其一，不知其二者多矣，可据理臆断欤？", "Java (TM) SE binary 未响应\\n· 尝试恢复此程序\\n· 等待程序响应\\n· 关闭程序", "我这里有负荆请罪 IV、弹射物吸引 V、经验腐蚀的钻石甲，要吗？（", "Plain musiC pLayer 2!"
                }),
                ModBase.RandomOne<string>(new string[]
                {
                    "犹豫，就会白给！", "PCL YES!", "龙猫是龙还是猫？", "我发言了，就这样~喵~（粗犷大叔音）", "我爱吃薯条 awa", "恭喜你，你被恭喜了！", "一支笔一盏灯一个奇迹！加油，开学生！", "你这什么游戏啊？手机能玩吗？要不要钱啊？", "Press F to enter the tank!", "多行不义必自毙。",
                    "活跃橙好难拿！", "进攻 D 点！", "我是我无敌的名无名，可以在 B 站上搜我，请记住我，我不想我噶了没人认识我。", "汤米，你……是肿瘤男孩！", "在我没想到好的留言之前请不要把我放到回声洞中！", "咕咕咕鸽鸽鸽嗝！", "3T3B", "没通关过。", "在游戏尽头的城市！", "幻翼感受到了动能……",
                    "喵喵喵，在这里放一只梵猫~他会盯着你的 awa", "我的刀盾~", "现在核电发电量是10A", "/kill @e[type=creeper]", "/gamemode 1", "你炸出了 TNT 之神,我将赐予你 TNT！", "不残血不会玩.jpg", "Mojang 作为英语或瑞典语时的读法不一样哦。", "主页自定义看起来很难，但是你仔细研究发现你可以套娃。", "众所周知，别人的世界和我的世界不是同一款游戏。",
                    "PCL 盒子！", "你的好友 xxx 正在游戏\\nWallpaper Engine", "第五人格牛福！", "装数据包时不要把数据包的 zip 文件也当成资源包装上！", "破产了启动器，你值得拥有。", "Wryyyyyyyyyyyyyy", "您，人？神！砰砰砰！", "龙腾猫跃最棒了！トトロが一番跳ねる！", "欧皇！！！", "这启动器也太好用了其实捐RMB你可以获得更多功能但不捐基础功能也是很多的",
                    "好想要正版账号", "我东洋天下第一！", "Mojang 在瑞典语中意思是东西！", "所以……我不知道有啥好说的，反正氵一个留言就行了 qwq", "众所周知，地狱有水。", "Also try 看云模拟器（bushi）！", "游戏一小时，看云 59min。", "我们需要再深入些。抱歉，今天不行。", "Non terrae plus ultra", "TIPS: 大量的红键会使屏幕起火！",
                    "喵呜~你好可爱~", "如果游戏没有声音可以尝试按下 F3+S！", "今早雾霾蔽日，但是不要害怕，太阳依旧在云端！", "犹豫就会白给！", "你知道吗？PCL 的前身是 PCL1！", "Mojang AB = Mojang And Bug", "Point Cloud Library 启动器！", "SCP 基金会已介入调查！", "很快就到你家门口挖矿！", "10 岁以下的儿童不宜食用小块块！（PS：我的世界 ESRB 分级为 10+）",
                    "Removed Herobrine!", "PCL 是个我的世界启动器吗？不，是音乐播放器，Mod 下载器和下载软件。", "龙猫的解密也太难了点吧！", "一定要点上面那个千万别点！！！", "手持两把锟斤拷，口中疾呼烫烫烫。脚踏千朵屯屯屯，笑看万物锘锘锘。", "白帝圣剑！御剑跟着我！", "今晚，深渊结算。", "(let ([s \"(let ([s ~s]) (printf s s)\"]) (printf s s)", "花开花落，再灿烂的星光也终将消失。", "DNF IS TRUE!!!",
                    "欢迎使用史上最复杂的解密启动器！（doge）", "原始人，起洞！", "某黑客：你会编程吗？龙猫：我不会，PCL 不是我编的。", "* (通过对着语音转换系统乱叫，这只龙猫偶然编出了这个软件。)", "宇宙多么浩瀚，偏偏我们在此相遇！", "UGxhaW4gQ3JhZnQgTGF1bmNoZXIgMu+8gQ==", "人品测试，每天一次！", "彭翠兰启动器！", "生而为人，我很抱歉！（看到这条留言的人一定很温柔吧！）", "没错，是百分制√",
                    "ckya blyat!", "好快の刀！", "freedom d↓ve！", "这是一条回声！", "泷泽萝拉哒！！！", "今天你猫车了吗？", "我国有一套完整的未成年人保护法！", "在内群有个人经常会把龙猫称为游戏《黎明杀机》里面的夹子杀手……", "回声洞\u3000\u3000\u3000\u3000\u3000\u3000\u3000投稿", "初音未来，我好喜欢你啊（",
                    "典明粥的制作配方：花京院典明的爱心宝宝粥*1 + 替身使者 死神13 的便便*1", "二次元，金发，吸血鬼，可爱……没错，这些都是在形容迪奥·布兰度。", "我在想为什么没有人一开始就用最强攻击呢？", "花儿在绽放，小鸟在歌唱……", "就 该 在 地 狱 中 燃 烧", "腾讯** 百度网盘**", "我的世界一直都不只是一款马赛克游戏！", "聆听怒海潮生，长空雷震，獠牙在耳，万籁黄昏，最动人，莫过喧嚣红尘。", "这次一定！", "繚乱！虹ヶ咲！"
                }),
                ModBase.RandomOne<string>(new string[]
                {
                    "PCL 中还有两种隐藏主题：真·滑稽彩和星空蓝。要怎么获得他们呢？提示：你猜！", "秒了，有什么好说的。", "塔塔开！塔塔开！", "平角裤，平角裤！", "白日放鸽须纵酒，龙猫作伴好还乡。", "龙猫说快改完 Bug 了，他一定没改完，就像老婆饼里没有老婆，康师傅牛肉面里没有牛肉。", "当你觉得臭鸽子龙猫又要咕咕咕的时候他真的咕咕咕了，这亦是一种不咕。", "December=Dec", "左手画圆，右手画方！", "咕咕咕咕？咕咕咕咕咕咕！咕咕咕咕咕咕咕？？？咕咕咕！（咕咕咕咕咕咕咕咕咕",
                    ".. -....- .-.. --- ...- . -....- .--. -.-. .-.. ..---", "芜湖，起飞！", "努力不一定会成功，但一定会有结果，无论好坏输赢，都不会后悔，因为，你也曾为此奋斗劳累拼命，人生千姿百态，努力一次，谁知怎样呢。", "I see the player you mean.", "催更催更，我是急急国王！", "精 神 小 伙", "总有些话，是反的，是倒的！", "大图？高清？", "我的世界是好游戏啊，再差的电脑也能带起来，再好的电脑也带不动……", "龙猫：太复杂了，不修了，这是特性，特性！",
                    "Why do you see this? Shouldn't you play your game?", "你给路达哟！不说了开游戏了，林肯死大头！", "龙猫的今日人品我们谁也不知道！", "我们是如何走到这一地步的？", "有个按钮可以让启动器缩小，旋转……", "众所周知，电子游戏不需要视力！", "We're no strangers to love~", "还记得第一次玩我的世界的时候吗？那曾是很美好的回忆……", "该说什么好呢 说什么好呢 什么好呢 么好呢 好呢 呢 （回声真大", "如果你找不到第三方登录的话不要到版本选择点那三个点，更不要点那个设置，就算你点了那千万不要往下翻。",
                    "千万别点千万别点！", "El Primo!!!", "在时间的流逝中，没有什么事是一成不变的。", "The Escapists 2？彳亍，开始逃狱！", "熬夜对身体不好，所以我建议你们……玩通宵。", "啊wee改哈鞥嫦娥我刚不疤痕处哈维楚王嗡阿格王朔！！！", "对于一个像我一样身高两米一的巨人来说，挖三填一是不可取的。", "众所周知，柯南和基德才是真爱！", "雷石东直放站！", "现在玩的嗨，待会被夹更嗨！",
                    "GrandpaBr!", "人群当中钻出来一个光！头！", "啊呜呜呜~~", "伊莉雅：美游来过这里吗？", "买不起正版的穷鬼举个爪。", "17 张牌你能秒我？你能秒杀我？", "有——回——声——吗—— 有-回-声-吗- 有回声吗", "Notch is coming back!", "夹子启动器！", "我不会告诉你，鼠标悬浮在隐藏主题上，会显示出什么不可告人的秘密！",
                    "国足 NB！", "结束了？不，你还有很多事要做，现在还不能休息哦……", "勇士总冠军，库里 FMVP！", "握~着~我~的~抱~枕~", "我爱吃滑稽果！", "跟你们讲一个笑话：龙猫", "你每天要忘记成千上万件事，为什么不把这件事也忘了。", "迫害群友需谨慎，不然夹子就离你不远了 awa", "Minecraft: Dungeons 又名 我的裤子动了！", "100 年清朝老兵，申请出战！",
                    "你现在不能睡觉，你的朋友在开派对！", "木叶飞舞之处，火亦生生不息！", "我爱学习，学习使我妈快乐，我妈快乐，全家快乐！", "人被逼急了啥都能做出来，除了数学题！", "M to the C to the V！", "* 保持你的决心，FUCK！", "向鸽者文明致敬！", "* 今天是多么美好的一天啊。小鸟在歌唱，花朵在绽放。在这样的一天里，像你这样的孩子……应当被龙猫夹起来扔垃圾桶里。", "关掉，关掉，一定要关掉！", "远古残骸真的存在吗？",
                    "啊呦 EVERYBODY 在你头上暴扣！", "Plain Craft Launcher→PCL→PC L→电脑 L→电脑垃圾\\n思考.jpg", "月色与雪色之间，你是第三种绝色。", "群服务器，时不时来群里 Can't keep up！", "或许你并不是不想睡觉，而是周围有怪物在游荡！", "我们的 LTW 真是太好玩了！", "阿瓦达啃大瓜！", "非酋该怎么在这个世界上生存（小声）……", "妇 科 圣 手", "少年没有乌托邦！",
                    "嗨，同志，您知道列宁格勒和斯大林格勒在哪吗？我在地图上找不到它。", "生活就像打电话，不是你先挂就是我先挂！", "好好睡一觉， 就是人生的重启方式呀。", "为什么披萨会考糊？", "老鼠偷了大米，人们说它狡猾；人类偷了蜂蜜，却说蜜蜂勤劳。", "千万别点千万别点千万别点！", "CHINA!!! CHINA!!! CHINA!!!", "看着龙猫的秀发，我不禁陷入沉思……哦！原来龙猫没有头发！", "休息区里有一个特殊的休息室，通往整个后室里最棒的派对喔！=)", "咖啡党永不为奴！",
                    "再点一下吧！", "这个回声洞莫得 CD，定个小目标，点它一亿下！", "Tip: 小鸽子们不要挑食哦，不管是烤鸭还是欧芹都要吃~", "到点了，Visual Studio 上号！", "人间，处处是仙境，何欲而求天？已有大于未有，莫要失去而追悔莫及！", "芜湖~咕咕咕起飞~", "死机蓝！", "Tip: 热知识：这是一条…烫烫烫烫烫！的热知识！", "再次点击查看下一位沙雕网友乱七八糟的留言！", "咕咕咕——\\n翻译：鸽，下次也不一定。"
                }),
                ModBase.RandomOne<string>(new string[]
                {
                    "在 MC 中，沙子可以下落，说明 MC 还是很科学的！（确信", "唔咕，要饭大失败……（眼神死", "你蓝了，你白了，你没了！", "这是回声洞~是回声洞~回声洞~声洞~洞~", "往往结束才是开始！", "检测到 Minecraft 进程意外退出，错误分析已开始……", "你不能游荡，周围有怪物在休息！", "玻璃，放错，退游，一气呵成！", "你已被服务器封禁！理由：请自证 1145 CPS！", "当你打开这条回声洞的时候……还不快去想想你的作业写完了没！",
                    "看到这条留言，请为疫情中献身的人们默哀一分钟，并献出至高的敬意。", "投稿回声洞的人数为 n，你看到这条的概率是 n 分之一，所以能看到这条的都是欧皇！", "汀！汀！莱万汀！汀！汀！莱万汀！", "直到我的钱包中了一箭！", "阿能我老婆！阿噗噜派！", "游艺街✘ 戒赌街✔", "忠告：请不要在喝水的时候看回声洞，否则所造成的一切后果与 PCL II 无关。", "生活枯燥无味，龙猫模仿人类！", "龙猫不是鸽子，只是太忙了（确信）！", "在这个时候，你的人品值会是：100！100！100！！！！！",
                    "问：龙猫今天的人品值是多少？\\n答：我连宇宙尽头在哪里都不知道，怎么会知道这个。", "愿世界永葆和平！", "ENDERMAN PENTA KILL! ACE!", "众里寻他千百度。蓦然回首，那人却在，灯火阑珊处。", "前进，然后变得更好！", "小丑竟是你自己！", "敲传统木鱼，见观音如来。\\n敲电子木鱼，见初音未来，法号弥苦。", "The cake is a lie.", "别戳了，这里没有你想找的东西！", "lm：蓝猫",
                    "主不在乎。", "抱歉，今天不行？不吃这套，谢谢。", "做工程是不可能不咕的啦，这辈子都不可能不咕的啦。", "挖三填一！", "Make Minecraft Great Again!", "众所周知，阳光菇不爱阳光。", "要是哪一天我电脑打 MC 炸了我都不稀奇！", "你笨拙的表现犹如黏糊的麦片粥，继续努力吧！", "恭喜你，你的鼠标左键没坏！", "炮造毕，何不置珍珠？",
                    "祝我下个池子出水大叔！大叔，为了你，对蓝色恶魔使用石头吧！", "Minecraft 1.7.10 - 单人游戏（未响应）", "我趣，是吴奇隆！", "悲しみの...向こうへと...", "* 移除了 Herobrine\\n* 修复了一个 Bug\\n* 增加了一个Bug", "长官，我们双脚着地，率先踏入地狱！", "Plain Craft Launcher 的中文翻译是：普通飞行器发射器！", "特别是其搭载 690 战术核显卡的改进版本，一发就可以摧毁一个航母战斗群。", "天上的卡兹不说话，地上的刀哥想妈妈（doge）！", "要用咕咕对抗咕咕。——鲁迅",
                    "We are the universe. We are everything you think isn't you.\\n——终末之诗", "众所周知，在服务器中按 Alt+F4 可以开启飞行！", "下降率大点没事！", "快门一按，行车中断，造成事故，移交法办！", "中国联通提醒您：警惕移动电信诈骗！", "就我个人来说，PCL 很好用对我的意义，不能不说非常重大。这样看来，就我个人来说，PCL 很好用对我的意义，不能不说非常重大。而这些并不是完全重要，更加重要的问题是，莎士比亚曾经说过，意志命运往往背道而驰，决心到最后会全部推倒。这句话语虽然很短, 但令我浮想联翩。", "夹子，夹子，更多的夹子，夹子在蔓延……", "谁言别后终无悔，寒月清宵绮梦回；深知身在情长在，前尘不共彩云飞。", "Bugjump 自古特性多，可与育碧争霸王！", "众所周知，Bug 修掉一个还会有第二个 Bug 伴随着修掉的 Bug 出现！",
                    "芜湖，我直接成为懒狗起飞！", "邪王真眼是最强的！", "反馈 Bug 前……先想一想这是不是特性！", "这是回声洞还是回字洞？", "歪比巴卜？", "为什么不试试在调试选项中把动画速度调成 0.1x 或是 3x 呢？", "神社倒闭之日。", "爱丽丝做的布朗尼果然好吃呢！面团口感湿润但却不发黏，有种清爽的甜味。可可粉是用万豪顿牌的吗？", "大大大~大工ong~，你的铠铠甲怎怎么漏漏漏 漏~电啊~\\n——面对二阶段深海骑士的格林", "错误代码：-118\\n无法载入网页（未知错误）",
                    "获得成就：别人的世界！", "你这圣遗物怎么不强化（", "你知道吗，当你看了这条信息会发现看了跟不看一个样！", "你知道吗？其实你什么都不知道！", "冷知识：这其实是一条热知识！", "回声洞里面没有米勒星球！", "你说对，但原神，米自研，冒险游，提瓦特，神选中，授神眼，引元素。扮角色，邂同伴，击强敌，找亲人，掘真相。", "孤独是山峰给予征服者的礼物。", "你知道吗：千万别惹玄素，否则会被夹得很惨！", "她牵着对立的手。因为她们将会继续前进。\\n因此，就像这样，命运的齿轮在这里继续运转……而远方不会再有等候着的命运。",
                    "众所周知，塔科夫是一款恐怖游戏。", "Click Circle!", "问君能有几多愁？恰似一缸龙猫向东流。", "这游戏真凡尔赛！", "以声之色，塑花之形，将你之名，刻于我心！", "Make Minecraft great again!", "任何罪恶终将绳之以法！", "看到我了吗？你没有！ヽ(•\u0300ω•\u0301 )ゝ", "Non terrae plus ultra!", "中继器是直放站！",
                    "Hex Dragon！", "什么？Java 版不支持光追？显卡白买了……", "育碧就是一颗大土豆！", "夹了！都给我夹了！", "自从接触了 CraftTweaker，GPT3 人工智能都被折磨疯了。", "我起了，一枪秒了，能怎样？", "结束了？开始了？不，还没开始呢，咕咕咕！", "今日大无语事件：外卖点了个黑椒牛排套餐，结果商家忘了放牛排……", "手握两把锟斤拷，口中疾呼烫烫烫，脚踏千朵屯屯屯，笑看万物锘锘锘！", "虽然上面那个按钮叫千万别点，但是还有好多人去点它！"
                }),
                ModBase.RandomOne<string>(new string[]
                {
                    "damedane, dameyo……", "你说得对，但是，后面忘了。", "哇！白色普通！", "PCL 的蓝色图标是用硫酸铜染色的！", "记得使用 Steam 启动 PCL 哦！", "果断，就会白给！", "各位同学们，作业写完了吗？", "I am the storm that is approoooooooooooooooaching!!!!!!!", "你不崩谁崩？", "天云轻轨交通二号线！开！通！啦！",
                    "前有 IDM，后有 XDM，今有 PDM！", "玩了红石以后腰不酸了腿不痛了。就是脑袋有点凉。", "野蜂飞过，经过了平凡与伟大。却追随着无悔！", "竜神の剣を喰らえ！", "你有没有听见孩子们的悲鸣！", "您这辈子都别想进入 Grand Theft Auto V 在线模式！", "我刚太认真写反馈，结果把笔头含嘴里了！", "友谊是魔法！", "不懂就问，我要问什么？", "年轻人不讲武德！",
                    "打五把 CSGO！", "* 没人知道龙猫收到多少回声洞投稿。\\n* 没人知道有人投了什么投稿。\\n* 没人知道龙猫的心态。\\n* 因为你根本不在乎龙猫。", "巨硬公司™ Hugehard™ Huge hardsoft！", "建议您 50 包邮并往里面塞 200 元，更容易卖出！", "建议您白送我，更容易卖出！", "怎样才能让龙猫选择你的投稿？当然是多发几遍！（bushi", "什么？这不是饼干，这是我生产的压缩毛巾……", "You should try our sister game, Minceraft!", "番茄条+土豆酱！", "* 你没有看见什么留言，这里只有几根鸽子留下的羽毛。",
                    "龙猫什么时候才能整点大活呢（小声", "奇变偶不变，_________！", "巴山楚水凄凉地，Q 得 cm△t", "井底之蛙，不曾见过大海之辽阔，却知晓天空之蓝！", "0 errors, 0 warnings!", "这个好诶！", "我们都是阴沟里的虫子，但总还是得有人仰望星空。", "开学愉快！", "祂从天空陨落，于是人们看到了神明……", "蛋白不会做蛋糕，但他会做糕蛋~",
                    "皇帝家是干什么的呢……人人都在叫这个叫皇帝的人，想必干活也一定是用的……额……金锄头？不不不，可能是钴锄头……", "温馨提示：按 Alt+F4 有惊喜！", "天不生我键盘侠，键道万古如长夜，键来！", "E！S！M！跑！！！", "你的无畏来源于无知！", "我的人品必不可能是 0！系统有 Bug！", "二次元，金发，吸血鬼，可爱……没错，这次真的是芙兰朵露了！", "祝各位音游人们在今后的打歌过程中好运连连，后宫成群（", "如果今天是你的生日，那我祝你生日快乐，如果不是，那我祝你早上中午下午晚上好！", "你 要 被 夹",
                    "我保留了千万别点，这样你才知道你用的是 PCL。", "海皮咳嗽是一个……高 Ping 战士快乐基地。", "龙咕（", "4 月 1 日打开 PCL 有惊喜！", "鸡汤来咯~~~~", "猜猜你要点多少次才能再次看到我！", "天苍苍，地茫茫，龙猫走路像牛羊。", "你充 Minecoin 吗？", "今天是个看人品值的好日子啊~", "虽然 Java 版不支持光追，但是现在基岩版白送了！哈哈哈哈！",
                    "啥时候出 PCL III？\\n回：不可能的……", "这里没有人~我们都是鬼~", "Long may the sunshine!", "If you can.", "Tell me your secret.", "近朱者赤，近墨者黑。近网易者，就是个寄吧！", "【新华字典】里一共有几个字？", "寻找远古城市，可以在高海拔地区例如冰封山峰、尖峭山峰、草旬等生物群系往下挖！", "我的化学老师说没有 PCl2 这种化学物质！", "真的会有傻子买 23.33 来玩回声洞吗？",
                    "这解密主题到底怎么解啊啊啊啊啊啊啊！", "或许有意义的人生，才是完美的人生！", "（阴暗的爬行）（蠕动）（尖叫）（同化）（不分对象攻击）", "在吗？明天 DDL 了。", "V 我 50 吃个 KFC 谢谢喵！", "《我 们 拥 有 最 真 实 的 物 理 引 擎》", "爱上一个人是快乐的，但是爱下去让你痛苦了，就要学着放弃，对吧。", "我有 20 铁嗷，你怕不怕！（不是", "《关于升级后不知道干啥……》", "Grove Street, home At least before I fu**ed up these things.",
                    "想来把昆特牌吗？我可是村子里最厉害的！", "我曾背井离乡，后来全村的人都渴死了。", "为 PCL 和伟大的咕咕咕事业而欢呼！", "胜利之风，正从我 DIO 背后吹来！", "今日人品？", "你每天都会忘掉很多事，为什么不把这件事也忘掉呢？", "点击千万别点，会送正版哦！", "我超！冰！", "再次点击这里以查看更多的 PCL 作者和各位沙雕网友乱七八糟的留言！", "glvE Me l0m c0iNs 0r Rep0rTinG U",
                    "不要再打羊驼了啊！！！", "温馨提示：当你拿打火石右键苦力怕时，苦力怕将会消失。", "嗨嗨嗨，我的世界好玩吗，不说了，喝鸡汤去了。", "你打开内群，试图在群文件找些有用的东西，却发现里面都是错误报告……", "沙雕解谜：整个 PCL II 将为你闪烁", "TECHNOBLADE YOU NERDSSSSSSS!", "你知道吗？龙腾猫跃这个名字实际上是从龙腾虎跃这个名字为基础改过来的哦~", "Bug 变 Feature，妙啊！", "你在狗叫什么？", "听我说谢谢你~因为有你~温暖了四季~"
                }),
                ModBase.RandomOne<string>(new string[]
                {
                    ModBase.RandomOne<string>(new string[]
                    {
                        "不要相信灰色，直接上！", "点一下不够就点两下！", "今日人品：100！", "鱼人节快乐！", "滑稽节是一个节日呢！", "ASCII 总是三位数！", "帮助的英文是 Help！", "砸反馈就完事了！", "属于宇宙的数字！", "众所周知，十大主题一定有十一个！",
                        "回声洞能带来灵感！", "！读着倒要话候时有", "从罗马开始！", "MCBBS 的本体是箱子！", "化学，文档，网格！", "网址就是来路！", "越过屏障！", "卢恩与去路！", "地下埋藏着宝藏！", "从老线索中发现新东西！",
                        "重组碎片……", "橙色线，藏着线和点！", "于历史中发掘秘密！", "线索在游戏之外！", "穷举不能让你变得更强！", "OBSIDIAN！", "深蓝色的极客！", "不要忽视背景！", "结束了？开始了。", "开始了？结束了。"
                    }),
                    PageOtherTest.GetRandomPresetHint()
                })
            }).ToString().Replace("\\n", "\r\n");
        }

        public static string GetRandomHint()
        {
            try
            {
                if (FileUtils.Exists(Paths.Base + "PCL\\hints.txt"))
                {
                    List<string> list = FileUtils.ReadAsLines(Paths.Base + "PCL\\hints.txt", false, null, null).Where<string>((PageOtherTest._Closure___._I29_0 == null) ? (PageOtherTest._Closure___._I29_0 = new Func<string, bool>(PageOtherTest._Closure___._I._Lambda___29_0)) : PageOtherTest._Closure___._I29_0).ToList<string>();
                    if (list.Any<string>())
                    {
                        return ModBase.RandomOne<string>(list);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "获取自定义 你知道吗 提示失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherTest.xaml.vb");
            }
            return PageOtherTest.GetRandomPresetHint();
        }

        public static string GetRandomPresetHint()
        {
            return ModBase.RandomOne<string>(new string[]
            {
                "在版本选择页面，右键某个版本也能进入版本设置页面！",
                "你可以在版本设置中调整分类，以将特定版本隐藏！",
                "使用 --memory 参数启动 PCL 可以静默进行内存优化！",
                "在第一次启动游戏时，PCL 会自动将语言设置为中文！",
                "在版本设置中可以开启第三方登录验证，例如 Little Skin 登录！",
                "自动配置游戏内存时，PCL 将会根据剩余内存与 Mod 数量动态决定分配的内存！",
                "主页可以使用特定的预设，在设置中看看吧！",
                "在高级启动选项中，可以设置在游戏启动前运行特定程序！",
                "将鼠标悬浮在设置页的左边栏，可以找到重置设置按钮！",
                "版本设置只对当前版本生效，而设置页面的设置对所有版本生效！",
                string.Format("要将已有的 MC 文件夹加入 PCL，可以在版本选择页的左侧选择 {0}添加已有文件夹{1}！", ModBase.vbLQ, ModBase.vbRQ),
                "如果同时安装了 OptiFine 与对应的原版，PCL 会展示 OptiFine 版本，折叠原版！",
                "版本选择的 常规版本 分类中，只会列出最新的一个快照或预发布版。",
                "在版本选择页面，右键游戏文件夹可以进行打开、重命名、删除等操作！",
                string.Format("如果你在其他地方修改了皮肤，需要手动选择 {0}刷新皮肤{1} 才能更新登录页面的头像……", ModBase.vbLQ, ModBase.vbRQ),
                "下载 Mod 时，PCL 会自动定位对应版本的 Mod 文件夹！",
                "如果缺少 Java，PCL 也能自动下载，不必自己安装啦！",
                "如果你打开了调试模式，启动页右侧就会显示启动日志！",
                "将鼠标悬浮在下载页的左边栏，可以找到刷新按钮！",
                string.Format("将鼠标指向下载页的 MC 版本，可以在右侧找到 {0}更新日志{1} 选项！", ModBase.vbLQ, ModBase.vbRQ),
                "直接把 Mod 或整合包拖进 PCL 窗口就能安装了！",
                string.Format("在 PCL 文件夹下新建 hints.txt，可以自定义 {0}你知道吗{1} 的内容！", ModBase.vbLQ, ModBase.vbRQ),
                "设置中可以自定义离线皮肤，但这只对单人游戏有效！",
                "如果不想用某项功能，可以在个性化设置中把它隐藏掉！",
                string.Format("如果打开了 {0}游戏更新提示{1} 功能，当 MC 更新时 PCL 会弹窗进行提醒！", ModBase.vbLQ, ModBase.vbRQ),
                "你可以使用主页来自定义快捷方式！",
                "点击版本右侧的心形就能将该版本加入收藏夹，便于查找。",
                "PCL 的第一个内部版本制作于 2018 年 8 月 13 日！",
                "据调查，有 90.3% 的用户点击了百宝箱中的千万别点按钮！",
                "PCL 的绝大多数代码都在 GitHub 开源了！",
                "PCL 的开发者龙腾猫跃经常被简称为龙猫，但和那只龙猫没有任何关系！"
            });
        }


        internal virtual OurButton BtnJrrp_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnJrrp;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = ((PageOtherTest._Closure___._IR37_3 == null) ? (PageOtherTest._Closure___._IR37_3 = new OurButton.ClickEventHandler(PageOtherTest._Closure___._I._Lambda___R37_3)) : PageOtherTest._Closure___._IR37_3);
                OurButton ourButton = this._BtnJrrp;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnJrrp = value;
                ourButton = this._BtnJrrp;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnMemory_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnMemory;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R41_4);
                OurButton ourButton = this._BtnMemory;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnMemory = value;
                ourButton = this._BtnMemory;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnClear_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnClear;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = ((PageOtherTest._Closure___._IR45_5 == null) ? (PageOtherTest._Closure___._IR45_5 = new OurButton.ClickEventHandler(PageOtherTest._Closure___._I._Lambda___R45_5)) : PageOtherTest._Closure___._IR45_5);
                OurButton ourButton = this._BtnClear;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnClear = value;
                ourButton = this._BtnClear;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnClick_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnClick;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnClick_Click);
                OurButton ourButton = this._BtnClick;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnClick = value;
                ourButton = this._BtnClick;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurCard CardCave_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._CardCave;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this.CardCave_MouseLeftButtonUp);
                OurCard ourCard = this._CardCave;
                if (ourCard != null)
                {
                    ourCard.MouseLeftButtonUp -= mouseButtonEventHandler;
                }
                this._CardCave = value;
                ourCard = this._CardCave;
                if (ourCard != null)
                {
                    ourCard.MouseLeftButtonUp += mouseButtonEventHandler;
                }
            }
        }


        internal virtual OurTextButton BtnCave_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnCave;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurTextButton.ClickEventHandler clickEventHandler = new OurTextButton.ClickEventHandler(this.BtnCave_Click);
                RoutedEventHandler routedEventHandler = new RoutedEventHandler(this._Lambda___R61_6);
                OurTextButton ourTextButton = this._BtnCave;
                if (ourTextButton != null)
                {
                    ourTextButton.Click -= clickEventHandler;
                    ourTextButton.Loaded -= routedEventHandler;
                }
                this._BtnCave = value;
                ourTextButton = this._BtnCave;
                if (ourTextButton != null)
                {
                    ourTextButton.Click += clickEventHandler;
                    ourTextButton.Loaded += routedEventHandler;
                }
            }
        }

        internal virtual OurTextBox TextDownloadUrl_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._TextDownloadUrl;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurTextBox.ValidateChangedEventHandler validateChangedEventHandler = new OurTextBox.ValidateChangedEventHandler(this._Lambda___R65_7);
                KeyEventHandler keyEventHandler = new KeyEventHandler(this.TextDownloadUrl_KeyDown);
                RoutedEventHandler routedEventHandler = new RoutedEventHandler(this._Lambda___R65_8);
                OurTextBox ourTextBox = this._TextDownloadUrl;
                if (ourTextBox != null)
                {
                    OurTextBox.ValidateChanged -= validateChangedEventHandler;
                    ourTextBox.KeyDown -= keyEventHandler;
                    ourTextBox.ValidatedTextChanged -= routedEventHandler;
                }
                this._TextDownloadUrl = value;
                ourTextBox = this._TextDownloadUrl;
                if (ourTextBox != null)
                {
                    OurTextBox.ValidateChanged += validateChangedEventHandler;
                    ourTextBox.KeyDown += keyEventHandler;
                    ourTextBox.ValidatedTextChanged += routedEventHandler;
                }
            }
        }

        internal virtual OurTextBox TextDownloadFolder_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._TextDownloadFolder;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                RoutedEventHandler routedEventHandler = new RoutedEventHandler(this._Lambda___R69_9);
                OurTextBox.ValidateChangedEventHandler validateChangedEventHandler = new OurTextBox.ValidateChangedEventHandler(this._Lambda___R69_10);
                KeyEventHandler keyEventHandler = new KeyEventHandler(this.TextDownloadUrl_KeyDown);
                OurTextBox ourTextBox = this._TextDownloadFolder;
                if (ourTextBox != null)
                {
                    ourTextBox.ValidatedTextChanged -= routedEventHandler;
                    OurTextBox.ValidateChanged -= validateChangedEventHandler;
                    ourTextBox.KeyDown -= keyEventHandler;
                }
                this._TextDownloadFolder = value;
                ourTextBox = this._TextDownloadFolder;
                if (ourTextBox != null)
                {
                    ourTextBox.ValidatedTextChanged += routedEventHandler;
                    OurTextBox.ValidateChanged += validateChangedEventHandler;
                    ourTextBox.KeyDown += keyEventHandler;
                }
            }
        }

        internal virtual OurTextBox TextDownloadName_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._TextDownloadName;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurTextBox.ValidateChangedEventHandler validateChangedEventHandler = new OurTextBox.ValidateChangedEventHandler(this._Lambda___R73_11);
                KeyEventHandler keyEventHandler = new KeyEventHandler(this.TextDownloadUrl_KeyDown);
                OurTextBox ourTextBox = this._TextDownloadName;
                if (ourTextBox != null)
                {
                    OurTextBox.ValidateChanged -= validateChangedEventHandler;
                    ourTextBox.KeyDown -= keyEventHandler;
                }
                this._TextDownloadName = value;
                ourTextBox = this._TextDownloadName;
                if (ourTextBox != null)
                {
                    OurTextBox.ValidateChanged += validateChangedEventHandler;
                    ourTextBox.KeyDown += keyEventHandler;
                }
            }
        }

        internal virtual OurTextButton BtnDownloadSelect_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnDownloadSelect;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurTextButton.ClickEventHandler clickEventHandler = new OurTextButton.ClickEventHandler(this.BtnDownloadSelect_Click);
                OurTextButton ourTextButton = this._BtnDownloadSelect;
                if (ourTextButton != null)
                {
                    ourTextButton.Click -= clickEventHandler;
                }
                this._BtnDownloadSelect = value;
                ourTextButton = this._BtnDownloadSelect;
                if (ourTextButton != null)
                {
                    ourTextButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnDownloadStart_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnDownloadStart;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R81_12);
                OurButton ourButton = this._BtnDownloadStart;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnDownloadStart = value;
                ourButton = this._BtnDownloadStart;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnDownloadOpen_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnDownloadOpen;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R85_13);
                OurButton ourButton = this._BtnDownloadOpen;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnDownloadOpen = value;
                ourButton = this._BtnDownloadOpen;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R1_1(object a0, RoutedEventArgs a1)
        {
            this.MeLoaded();
        }

        [CompilerGenerated]
        private void _Lambda___27_0()
        {
            this.IsCaveEnabled = true;
        }

        [CompilerGenerated]
        private void _Lambda___R41_4(object a0, MouseButtonEventArgs a1)
        {
            this.BtnMemory_Click();
        }

        [CompilerGenerated]
        private void _Lambda___R61_6(object a0, RoutedEventArgs a1)
        {
            this.CaveHand();
        }

        [CompilerGenerated]
        private void _Lambda___R65_7(object a0, EventArgs a1)
        {
            this.StartButtonRefresh();
        }

        [CompilerGenerated]
        private void _Lambda___R65_8(object a0, RoutedEventArgs a1)
        {
            this.TextDownloadUrl_TextChanged(RuntimeHelpers.GetObjectValue(a0), (TextChangedEventArgs)a1);
        }

        [CompilerGenerated]
        private void _Lambda___R69_9(object a0, RoutedEventArgs a1)
        {
            this.SaveCacheDownloadFolder();
        }

        [CompilerGenerated]
        private void _Lambda___R69_10(object a0, EventArgs a1)
        {
            this.StartButtonRefresh();
        }

        [CompilerGenerated]
        private void _Lambda___R73_11(object a0, EventArgs a1)
        {
            this.StartButtonRefresh();
        }

        [CompilerGenerated]
        private void _Lambda___R81_12(object a0, MouseButtonEventArgs a1)
        {
            this.BtnDownloadStart_Click();
        }

        [CompilerGenerated]
        private void _Lambda___R85_13(object a0, MouseButtonEventArgs a1)
        {
            this.BtnDownloadOpen_Click();
        }

        private bool IsLoad;

        private static bool IsMemoryOptimizeRunning = false;

        private bool IsCaveEnabled;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurScrollViewer _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnJrrp")]
        private OurButton _BtnJrrp;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnMemory")]
        private OurButton _BtnMemory;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnClear")]
        private OurButton _BtnClear;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnClick")]
        private OurButton _BtnClick;

        [CompilerGenerated]
        [AccessedThroughProperty("CardCave")]
        private OurCard _CardCave;

        [CompilerGenerated]
        [AccessedThroughProperty("LabCave")]
        private TextBlock _LabCave;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnCave")]
        private OurTextButton _BtnCave;

        [CompilerGenerated]
        [AccessedThroughProperty("TextDownloadUrl")]
        private OurTextBox _TextDownloadUrl;

        [CompilerGenerated]
        [AccessedThroughProperty("TextDownloadFolder")]
        private OurTextBox _TextDownloadFolder;

        [CompilerGenerated]
        [AccessedThroughProperty("TextDownloadName")]
        private OurTextBox _TextDownloadName;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnDownloadSelect")]
        private OurTextButton _BtnDownloadSelect;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnDownloadStart")]
        private OurButton _BtnDownloadStart;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnDownloadOpen")]
        private OurButton _BtnDownloadOpen;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PrivilegeToken
        {
            public int PrivilegeCount;

            public long Luid;

            public int Attributes;
        }

        [CompilerGenerated]
        [Serializable]
        private new sealed class _Closure___
        {
            internal void _Lambda___R9_2(ModLoader.LoaderBase a0)
            {
                PageOtherTest.DownloadState((ModLoader.LoaderCombo<int>)a0);
            }

            internal void _Lambda___13_0()
            {
                for (;;)
                {
                    ModBase.RunInUi((PageOtherTest._Closure___._I13_1 == null) ? (PageOtherTest._Closure___._I13_1 = new Action(PageOtherTest._Closure___._I._Lambda___13_1)) : PageOtherTest._Closure___._I13_1, false);
                    Thread.Sleep(10);
                }
            }

            internal void _Lambda___13_1()
            {
                if (ModMain.FrmMain.Top < -ModMain.FrmMain.Height)
                {
                    ModMain.FrmMain.Top = (double)(checked(OurWpfExtension.Computer.Screen.Bounds.Height + 49));
                }
                if (ModMain.FrmMain.Top > (double)(checked(OurWpfExtension.Computer.Screen.Bounds.Height + 50)))
                {
                    ModMain.FrmMain.Top = -ModMain.FrmMain.Height + 1.0;
                }
                if (ModMain.FrmMain.Left < -ModMain.FrmMain.Width)
                {
                    ModMain.FrmMain.Left = (double)(checked(OurWpfExtension.Computer.Screen.Bounds.Width + 49));
                }
                if (ModMain.FrmMain.Left > (double)(checked(OurWpfExtension.Computer.Screen.Bounds.Width + 50)))
                {
                    ModMain.FrmMain.Left = -ModMain.FrmMain.Width + 1.0;
                }
            }

            internal void _Lambda___15_0()
            {
                PageOtherTest frmOtherTest = ModMain.FrmOtherTest;
                bool flag;
                if (frmOtherTest == null)
                {
                    flag = false;
                }
                else
                {
                    OurButton btnClear = frmOtherTest.BtnClear;
                    flag = btnClear != null && btnClear.IsLoaded;
                }
                if (flag)
                {
                    ModMain.FrmOtherTest.BtnClear.IsEnabled = false;
                }
            }

            internal void _Lambda___15_1()
            {
                try
                {
                    if (ModWatcher.HasRunningMinecraft || ModLaunch.McLaunchLoader.State == ModBase.LoadState.Loading)
                    {
                        ModMain.Hint("请先关闭所有运行中的游戏！", ModMain.HintType.Blue, true);
                    }
                    else if (ModNet.HasDownloadingTask(false))
                    {
                        ModMain.Hint("请在所有下载任务完成后再来清理吧！", ModMain.HintType.Blue, true);
                    }
                    else
                    {
                        if (!ModMinecraft.McFolderList.Any<ModMinecraft.McFolder>())
                        {
                            ModMinecraft.McFolderListLoader.Start(null, false);
                        }
                        if (PathUtils.IsSame(ModBase.PathTemp, Path.GetTempPath() + "PCL\\"))
                        {
                            if (Settings.Get<int>("HintClearRubbish", null) <= 2)
                            {
                                if (ModMain.OurMsgBox("即将清理游戏日志、错误报告、缓存等文件。\r\n虽然应该没人往这些地方放重要文件，但还是问一下，是否确认继续？\r\n\r\n在完成清理后，需要重启 PCL。", "清理确认", "确定", "取消", "", false, true, false, null, null, null) == 2)
                                {
                                    return;
                                }
                                Settings.Set("HintClearRubbish", checked(Settings.Get<int>("HintClearRubbish", null) + 1), null);
                            }
                        }
                        else if (ModMain.OurMsgBox("即将清理游戏日志、错误报告、缓存等文件。\r\n\r\n你已将缓存文件夹手动修改为：" + ModBase.PathTemp + "\r\n清理过程中，将删除该文件夹中的所有内容，且无法恢复。请确认其中没有除了 PCL 缓存以外的重要文件！\r\n\r\n在完成清理后，需要重启 PCL。", "清理确认", "确定", "取消", "", false, true, false, null, null, null) == 2)
                        {
                            return;
                        }
                        List<string> list = new List<string>();
                        if (!ModMinecraft.McFolderList.Any<ModMinecraft.McFolder>())
                        {
                            ModMinecraft.McFolderListLoader.WaitForExit(null, null, false, default(CancellationToken), null);
                        }
                        try
                        {
                            foreach (ModMinecraft.McFolder mcFolder in ModMinecraft.McFolderList)
                            {
                                list.Add(mcFolder.Location);
                                list.AddRange(DirectoryUtils.EnumerateDirectories(mcFolder.Location + "versions", false, "*"));
                            }
                        }
                        finally
                        {
                            List<ModMinecraft.McFolder>.Enumerator enumerator = default;
                            ((IDisposable)enumerator).Dispose();
                        }
                        Action<string> action = ((PageOtherTest._Closure___._I15_2 == null) ? (PageOtherTest._Closure___._I15_2 = new Action<string>(PageOtherTest._Closure___._I._Lambda___15_2)) : PageOtherTest._Closure___._I15_2);
                        try
                        {
                            foreach (string text in list)
                            {
                                action(Path.Combine(text, ".mixin.out"));
                                action(Path.Combine(text, "crash-reports"));
                                action(Path.Combine(text, "debug"));
                                action(Path.Combine(text, "logs"));
                                try
                                {
                                    foreach (string text2 in DirectoryUtils.EnumerateFiles(text, false, "*"))
                                    {
                                        string lastPart = PathUtils.GetLastPart(text2);
                                        if (lastPart.StartsWithF("hs_err_pid", false) || Operators.CompareString(PathUtils.GetExtension(lastPart), "log", false) == 0 || Operators.CompareString(lastPart, "WailaErrorOutput.txt", false) == 0)
                                        {
                                            FileUtils.Delete(text2, false);
                                        }
                                    }
                                }
                                finally
                                {
                                    IEnumerator<string> enumerator3 = default;
                                    if (enumerator3 != null)
                                    {
                                        enumerator3.Dispose();
                                    }
                                }
                                try
                                {
                                    foreach (string text3 in DirectoryUtils.EnumerateDirectories(text, false, "*"))
                                    {
                                        if (Operators.CompareString(PathUtils.GetLastPart(text3), PathUtils.GetLastPart(text) + "-natives", false) == 0 || Operators.CompareString(PathUtils.GetLastPart(text3), "natives-windows-x86_64", false) == 0)
                                        {
                                            action(text3);
                                        }
                                    }
                                }
                                finally
                                {
                                    IEnumerator<string> enumerator4 = default;
                                    if (enumerator4 != null)
                                    {
                                        enumerator4.Dispose();
                                    }
                                }
                            }
                        }
                        finally
                        {
                            List<string>.Enumerator enumerator2 = default;
                            ((IDisposable)enumerator2).Dispose();
                        }
                        action(ModBase.PathTemp);
                        action(ModBase.OsDrive + "ProgramData\\PCL\\");
                        if (ModMain.OurMsgBox("垃圾已经清理好啦！", "清理完成", "关闭 PCL", "重启 PCL", "", false, true, true, null, null, null) == 2)
                        {
                            ModBase.StartProcess(new ProcessStartInfo(ModBase.PathExe, "--wait"));
                        }
                        FormMain.EndProgramForce(ModBase.ProcessReturnValues.Success);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "清理垃圾失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherTest.xaml.vb");
                }
                finally
                {
                    ModBase.RunInUiWait((PageOtherTest._Closure___._I15_3 == null) ? (PageOtherTest._Closure___._I15_3 = new Action(PageOtherTest._Closure___._I._Lambda___15_3)) : PageOtherTest._Closure___._I15_3);
                }
            }

            internal void _Lambda___15_2(string Dir)
            {
                try
                {
                    DirectoryUtils.Delete(Dir, false);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, string.Format("清理文件夹失败（{0}）", Dir), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherTest.xaml.vb");
                }
            }

            internal void _Lambda___15_3()
            {
                PageOtherTest frmOtherTest = ModMain.FrmOtherTest;
                bool flag;
                if (frmOtherTest == null)
                {
                    flag = false;
                }
                else
                {
                    OurButton btnClear = frmOtherTest.BtnClear;
                    flag = btnClear != null && btnClear.IsLoaded;
                }
                if (flag)
                {
                    ModMain.FrmOtherTest.BtnClear.IsEnabled = true;
                }
            }

            internal void _Lambda___16_0()
            {
                PageOtherTest.MemoryOptimize(true);
            }

            internal bool _Lambda___29_0(string s)
            {
                return !string.IsNullOrWhiteSpace(s);
            }

            internal void _Lambda___R37_3(object a0, MouseButtonEventArgs a1)
            {
                PageOtherTest.Jrrp();
            }

            internal void _Lambda___R45_5(object a0, MouseButtonEventArgs a1)
            {
                PageOtherTest.RubbishClear();
            }

            public static readonly PageOtherTest._Closure___ _I = new PageOtherTest._Closure___();

            public static Action<ModLoader.LoaderBase> _IR9_2;

            public static Action _I13_1;

            public static Action _I13_0;

            public static Action _I15_0;

            public static Action<string> _I15_2;

            public static Action _I15_3;

            public static Action _I15_1;

            public static Action _I16_0;

            public static Func<string, bool> _I29_0;

            public static OurButton.ClickEventHandler _IR37_3;

            public static OurButton.ClickEventHandler _IR45_5;
        }
    }
}
