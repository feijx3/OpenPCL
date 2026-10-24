using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageLinkMain : OurPageRight
    {
        public PageLinkMain()
        {
            base.Initialized += this._Lambda___R1_1;
            base.PageEnter += this.UpdatePanelVisibility;
            base.Loaded += this._Lambda___R1_2;
            base.Loaded += this._Lambda___R1_3;
            this.FailReason = "准备初始化";
            this.PathEasyTier = Paths.AppDataThenName + "EasyTier\\";
            this.ClientAddress = null;
            this.DiscoverNodeID = -1;
            this.LinkLoader = new ModLoader.LoaderCombo<int>("联机", new ModLoader.LoaderBase[]
            {
                new ModLoader.LoaderTask<int, int>("获取配置", new Action<ModLoader.LoaderTask<int, int>>(this.InitConfig), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 8.0
                },
                new ModLoader.LoaderTask<int, List<ModNet.NetFile>>("准备下载联机模块", new Action<ModLoader.LoaderTask<int, List<ModNet.NetFile>>>(this.InitPrepareDownload), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 2.0
                },
                new ModNet.LoaderDownload("下载联机模块", new List<ModNet.NetFile>())
                {
                    ProgressWeight = 40.0
                },
                new ModLoader.LoaderTask<int, int>("启动联机模块", new Action<ModLoader.LoaderTask<int, int>>(this.InitLaunch), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 50.0
                }
            });
            this.LogHistory = new Queue<string>(10);
            this.Peers = null;
            this.NATType = PageLinkMain.NATTypes.Pending;
            this.RefreshPeerLoader = new ModLoader.LoaderTask<int, List<PageLinkMain.Peer>>("EasyTier CLI", new Action<ModLoader.LoaderTask<int, List<PageLinkMain.Peer>>>(this._Lambda___R1_4), null, ThreadPriority.Normal);
            this.FailCount = 0;
            this.InitializeComponent();
            this.CardLoad_WithEvents = this.CardLoad;
            this.BtnLoadCancel_WithEvents = this.BtnLoadCancel;
            this.PanSelectCreate_WithEvents = this.PanSelectCreate;
            this.PanSelectJoin_WithEvents = this.PanSelectJoin;
            this.BtnFinishCopy_WithEvents = this.BtnFinishCopy;
            this.BtnFinishExit_WithEvents = this.BtnFinishExit;
            this.BtnFinishIp_WithEvents = this.BtnFinishIp;
        }

        private void ChangeState(PageLinkMain.LinkStates NewState)
        {
            PageLinkMain._Closure___5_0 CS_locals1 = new PageLinkMain._Closure___5_0();
            CS_locals1._VB_Me = this;
            CS_locals1._VB_Local_NewState = NewState;
            if (PageLinkMain.LinkState != CS_locals1._VB_Local_NewState)
            {
                CS_locals1._VB_Local_OldState = PageLinkMain.LinkState;
                PageLinkMain.LinkState = CS_locals1._VB_Local_NewState;
                Logger.Info(string.Format("主状态由 {0} 变更为 {1}", CS_locals1._VB_Local_OldState, CS_locals1._VB_Local_NewState), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                ModBase.RunInUi(new Action(CS_locals1._Lambda___0), false);
            }
        }

        private void LoaderInit()
        {
            base.PageLoaderInit<int>(this.Load, this.PanLoad, this.PanContent, this.PanAlways, this.LinkLoader, null, null, false);
            this.LinkLoader.OnStateChangedUi += this._Lambda___6_0;
        }

        private void UpdatePanelVisibility()
        {
            ModMain.FrmLinkMain.PanSelect.Visibility = ((PageLinkMain.LinkState == PageLinkMain.LinkStates.Waiting) ? Visibility.Visible : Visibility.Collapsed);
            ModMain.FrmLinkMain.PanFinish.Visibility = ((PageLinkMain.LinkState == PageLinkMain.LinkStates.Finished) ? Visibility.Visible : Visibility.Collapsed);
        }

        private void SwitchToWaiting(PageLinkMain.LinkStates OldState)
        {
            this.LinkLoader.Cancel();
            object lockState = this.LinkLoader.LockState;
            ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
            lock (lockState)
            {
                this.LinkLoader.State = ModBase.LoadState.Waiting;
            }
            PageLinkMain.ProcessStop();
            if (OldState != PageLinkMain.LinkStates.Finished)
            {
                this.UpdatePanelVisibility();
            }
            base.PageOnContentExit();
        }

        private void Create_MouseLeftButtonUp()
        {
            string text = ModMain.OurMsgBoxInput("输入端口", string.Format("在单人游戏的暂停菜单选择 {0}对局域网开放{1}，然后输入端口数字。{2}甚至可以输入其他游戏的端口……嗯……", ModBase.vbLQ, ModBase.vbRQ, "\r\n"), null, new Collection<Validate>
            {
                new ValidateInteger(1024, 65535)
            }, "端口号", "确定", "取消", false);
            if (text != null)
            {
                this.IsServerSide = true;
                this.ServerPort = Conversions.ToInteger(text);
                VB_AnonymousDelegate_0<string> vb_AnonymousDelegate_ = ((PageLinkMain._Closure___._I9_0 == null) ? (PageLinkMain._Closure___._I9_0 = new VB_AnonymousDelegate_0<string>(PageLinkMain._Closure___._I._Lambda___9_0)) : PageLinkMain._Closure___._I9_0);
                this.NetworkName = string.Format("P{0}-{1}", this.ServerPort.ConvertRadix(16).PadLeft(4, '0'), vb_AnonymousDelegate_());
                this.NetworkSecret = vb_AnonymousDelegate_();
                this.DiscoverNodeID = -1;
                Logger.Info(string.Format("尝试创建房间，网络名 {0}，网络密码 {1}，端口 {2}", this.NetworkName, this.NetworkSecret, this.ServerPort), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                this.ChangeState(PageLinkMain.LinkStates.Loading);
            }
        }

        public static void Join()
        {
            string text = ModMain.OurMsgBoxInput("输入邀请码", "输入房主发给你的邀请码。", null, null, string.IsNullOrEmpty(PageLinkMain.LastCode) ? "" : "使用上一次的邀请码", "确定", "取消", false);
            if (!string.IsNullOrEmpty(PageLinkMain.LastCode) && text != null && Operators.CompareString(text, "", false) == 0)
            {
                text = PageLinkMain.LastCode;
            }
            if (text != null)
            {
                PageLinkMain.Join(text);
            }
        }

        public void JoinInternal(string Code)
        {
            if (Code != null)
            {
                string text = PageLinkMain.ValidateCodeFormat(Code);
                if (text != null)
                {
                    ModMain.Hint(text, ModMain.HintType.Red, true);
                    return;
                }
                Code = PageLinkMain.FixCodeFormat(Code);
                Logger.Info(string.Format("实际使用的邀请码：{0}", Code), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                this.IsServerSide = false;
                this.ServerPort = Conversions.ToInteger(Code.Substring(1, 4).Upper().ConvertRadix(16, 10));
                this.NetworkName = Code.Substring(0, 11);
                this.NetworkSecret = Code.Substring(12, 5);
                if (Operators.CompareString(Code.Substring(20, 3), "000", false) == 0)
                {
                    this.DiscoverNodeID = -2;
                }
                else
                {
                    this.DiscoverNodeID = Conversions.ToInteger(Code.Substring(20, 3).Upper().ConvertRadix(16, 10));
                }
                Logger.Info(string.Format("尝试加入房间，网络名 {0}，网络密码 {1}，端口 {2}，发现节点 {3}", new object[] { this.NetworkName, this.NetworkSecret, this.ServerPort, this.DiscoverNodeID }), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                PageLinkMain.LastCode = Code;
                this.ChangeState(PageLinkMain.LinkStates.Loading);
            }
        }

        public static string ValidateCodeFormat(string Code)
        {
            string text;
            if (Code == null)
            {
                text = "邀请码为空！";
            }
            else
            {
                Code = PageLinkMain.FixCodeFormat(Code);
                if (Code.Length < 14 || Code[0] != 'P' || Code[5] != '-' || Code[11] != '-')
                {
                    if (Code.StartsWithF("U/", false))
                    {
                        text = "请让房主使用 PCL 创建房间！";
                    }
                    else if (Code.Length == 10)
                    {
                        text = "请让房主使用非社区版的 PCL 创建房间！";
                    }
                    else
                    {
                        text = "邀请码有误，请让房主使用 PCL 创建房间！";
                    }
                }
                else if (Code.Length >= 23 && Code[17] == '-' && ModBase.Val(Code.Substring(18, 2)) > 2.0)
                {
                    text = "你的 PCL 版本太老了，请在更新 PCL 之后再联机！";
                }
                else
                {
                    text = null;
                }
            }
            return text;
        }

        private static string FixCodeFormat(string Code)
        {
            Code = Code.Between("【", "】", false).Between("[", "]", false);
            Code = Code.Upper().Replace("O", "0").Replace("I", "1");
            if (Code.Length >= 17 && (Code.Length < 23 || (Code.Length >= 18 && Code[17] != '-')))
            {
                Code = Code.Substring(0, 17) + "-0105E";
            }
            return Code;
        }

        public static void Join(string Code)
        {
            if (PageLinkMain.LinkState != PageLinkMain.LinkStates.Waiting)
            {
                ModMain.Hint("你已经在联机房间中了！", ModMain.HintType.Red, true);
                return;
            }
            if (ModMain.FrmMain.PageCurrent == FormMain.PageType.Link)
            {
                ModMain.FrmLinkMain.JoinInternal(Code);
                return;
            }
            PageLinkMain.AutoJoinCode = Code;
            ModMain.FrmMain.PageChange(FormMain.PageType.Link, FormMain.PageSubType.Default);
        }

        private void PanSelectJoin_Loaded()
        {
            if (PageLinkMain.AutoJoinCode != null)
            {
                if (PageLinkMain.LinkState == PageLinkMain.LinkStates.Waiting)
                {
                    this.JoinInternal(PageLinkMain.AutoJoinCode);
                }
                PageLinkMain.AutoJoinCode = null;
            }
        }

        private void SwitchToLoading(PageLinkMain.LinkStates OldState)
        {
            this.Peers = null;
            this.FailCount = 0;
            this.LinkLoader.Start(null, true);
            this.UpdateProgressBar(0.0);
            this.LabLoadTitle.Text = (this.IsServerSide ? "创建房间中" : "加入房间中");
            this.UpdateLoadingPage("正在初始化……", "准备初始化");
        }

        private void SwitchToFailed(PageLinkMain.LinkStates OldState)
        {
            this.LinkLoader.Cancel();
            PageLinkMain.ProcessStop();
            this.UpdateProgressBar(1.0);
            this.LabLoadTitle.Text = this.FailReason;
            string text = this.LinkLoader.Error.GetDisplay(false);
            Exception ex = this.LinkLoader.Error;
            if (ex.Message.StartsWithF("$", false))
            {
                text = ex.Message;
            }
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                if (ex.Message.StartsWithF("$", false))
                {
                    text = ex.Message;
                }
            }
            this.LabLoadDesc.Text = (text.StartsWithF("$", false) ? text.TrimStart(new char[] { '$' }) : this.LinkLoader.Error.GetDisplay(true));
            Logger.Warn(this.LinkLoader.Error, this.LabLoadTitle.Text, LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
        }

        private void UpdateLoadingPage(string Title, string FailBrief)
        {
            PageLinkMain._Closure___20_0 CS_locals1 = new PageLinkMain._Closure___20_0();
            CS_locals1._VB_Local_Title = Title;
            if (Operators.CompareString(this.FailReason, FailBrief + "失败", false) != 0)
            {
                this.FailReason = FailBrief + "失败";
                Logger.Info(string.Format("开始步骤：{0}", CS_locals1._VB_Local_Title), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                ModBase.RunInUiWait(new Action(CS_locals1._Lambda___0));
            }
        }

        private void BtnLoadCancel_Click()
        {
            this.ChangeState(PageLinkMain.LinkStates.Waiting);
        }

        private void CardLoad_MouseLeftButtonUp()
        {
            this.ChangeState(PageLinkMain.LinkStates.Loading);
        }

        private void UpdateProgressBar(double Value = -1.0)
        {
            if (Value == -1.0)
            {
                Value = this.LinkLoader.Progress;
            }
            double value = this.ColumnProgressA.Width.Value;
            if (Math.Round(Value - value, 3) != 0.0)
            {
                if (value <= Value)
                {
                    double num = ((Value == 1.0) ? 1.0 : (value + Math.Pow(Value - value, 2.0) * 0.5));
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaGridLengthWidth(this.ColumnProgressA, num - this.ColumnProgressA.Width.Value, 200, 0, null, false),
                        ModAnimation.AaGridLengthWidth(this.ColumnProgressB, 1.0 - num - this.ColumnProgressB.Width.Value, 200, 0, null, false)
                    }, "Link Progress", false);
                    return;
                }
                this.ColumnProgressA.Width = new GridLength(Value, GridUnitType.Star);
                this.ColumnProgressB.Width = new GridLength(1.0 - Value, GridUnitType.Star);
                ModAnimation.AniStop("Link Progress");
            }
        }

        private void CardLoad_SizeChanged()
        {
            this.RectProgressClip.Rect = new Rect(0.0, 0.0, this.CardLoad.ActualWidth, 12.0);
        }

        private void SwitchToFinished(PageLinkMain.LinkStates OldState)
        {
            this.UpdatePanelVisibility();
            if (this.IsServerSide)
            {
                this.LabFinishTitle.Text = "已创建房间";
                this.LabFinishDesc.Text = string.Format("把邀请码发给朋友，让大家加入房间吧！{0}邀请码：{1}", "\r\n", this.GetInviteCode());
                this.BtnFinishExit.Text = "关闭";
                this.BtnFinishPing.ToolTip = "网络延迟";
                this.BtnFinishCopy.Visibility = Visibility.Visible;
                this.Copy();
                this.BtnFinishPort.Visibility = Visibility.Visible;
                this.LabFinishPort.Visibility = Visibility.Visible;
                this.BtnFinishIp.Visibility = Visibility.Collapsed;
                this.LabFinishIp.Visibility = Visibility.Collapsed;
                this.LabFinishPort.Text = Conversions.ToString(this.ServerPort);
            }
            else
            {
                this.LabFinishTitle.Text = "已加入房间";
                this.LabFinishDesc.Text = string.Format("在多人游戏页面的最下方就能找到联机房间！{0}注意：使用离线登录时不要手动输入 IP！", "\r\n");
                this.BtnFinishExit.Text = "离开";
                this.BtnFinishPing.ToolTip = "与房主的延迟";
                this.BtnFinishCopy.Visibility = Visibility.Collapsed;
                this.BtnFinishPort.Visibility = Visibility.Collapsed;
                this.LabFinishPort.Visibility = Visibility.Collapsed;
                this.LabFinishIp.Text = this.ClientAddress;
                this.BtnFinishIp.Visibility = Visibility.Visible;
                this.LabFinishIp.Visibility = Visibility.Visible;
            }
            this.Update();
        }

        private void BtnFinishExit_Click()
        {
            this.TryExit(false, false);
        }

        public bool TryExit(bool Silent, bool Closing)
        {
            bool flag;
            if (PageLinkMain.LinkState == PageLinkMain.LinkStates.Waiting || PageLinkMain.LinkState == PageLinkMain.LinkStates.Failed)
            {
                flag = false;
            }
            else
            {
                if (!Silent)
                {
                    if (this.IsServerSide && this.PeopleCount > 1)
                    {
                        if (ModMain.OurMsgBox("你确定要关闭联机房间吗？\r\n所有玩家都需要重新输入邀请码才可加入游戏！", "退出联机", "确定", "取消", "", true, true, false, null, null, null) == 2)
                        {
                            return true;
                        }
                    }
                    else if (Closing && ModMain.OurMsgBox(this.IsServerSide ? "你确定要关闭联机房间吗？" : "你确定要离开联机房间吗？", "退出联机", "确定", "取消", "", false, true, false, null, null, null) == 2)
                    {
                        return true;
                    }
                }
                this.ChangeState(PageLinkMain.LinkStates.Waiting);
                flag = false;
            }
            return flag;
        }

        private void Copy()
        {
            string text = string.Format("在 PCL 启动器中输入邀请码【{0}】，即可加入联机房间！", this.GetInviteCode());
            ModBase.ClipboardSet(text, false);
            Settings.Set("LinkLastAutoJoinInviteCode", text, null);
            ModMain.Hint("已复制邀请码！", ModMain.HintType.Green, true);
        }

        private string GetInviteCode()
        {
            return string.Format("{0}-{1}-{2}{3}", new object[]
            {
                this.NetworkName,
                this.NetworkSecret,
                2.ToString().PadLeft(2, '0'),
                ((this.DiscoverNodeID == -1) ? 0 : this.DiscoverNodeID).ConvertRadix(16).PadLeft(3, '0')
            });
        }

        private void BtnFinishIp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ModBase.ClipboardSet(this.ClientAddress, false);
            ModMain.Hint("已复制服务器地址！", ModMain.HintType.Green, true);
        }

        private ModLoader.LoaderCombo<int> LinkLoader
        {
            [CompilerGenerated]
            get
            {
                return this._LinkLoader;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._LinkLoader = value;
            }
        }

        private void InitConfig(ModLoader.LoaderTask<int, int> Task)
        {
            this.UpdateLoadingPage("正在联网获取配置……", "联网获取配置");
            ModSecret.ServerLoader.WaitForExit(null, Task, false, default(CancellationToken), null);
            if (ModSecret.ServerConfig == null)
            {
                throw new Exception("无法从服务器获取配置");
            }
            JToken jtoken = ModSecret.ServerConfig["Link"];
            string text;
            if (jtoken == null)
            {
                text = null;
            }
            else
            {
                JToken jtoken2 = jtoken["DisableReason2"];
                text = ((jtoken2 != null) ? jtoken2.ToString() : null);
            }
            string text2 = text;
            if (!string.IsNullOrEmpty(text2))
            {
                throw new Exception("$" + text2);
            }
            if (((JObject)ModSecret.ServerConfig["Link"]).ContainsKey("MinVersionCode") && 414 < ModSecret.ServerConfig["Link"]["MinVersionCode"].ToObject<int>())
            {
                throw new Exception("$你的 PCL 版本太老了，请在更新 PCL 之后再联机！");
            }
        }

        private void InitPrepareDownload(ModLoader.LoaderTask<int, List<ModNet.NetFile>> Task)
        {
            PageLinkMain._Closure___47_0 CS_locals1 = new PageLinkMain._Closure___47_0();
            this.UpdateLoadingPage("正在获取 CPU 架构……", "获取 CPU 架构");
            CS_locals1._VB_Local_Architecture = Conversions.ToString((int)typeof(string).Assembly.GetName().ProcessorArchitecture);
            string _VB_Local_Architecture = CS_locals1._VB_Local_Architecture;
            if (Operators.CompareString(_VB_Local_Architecture, Conversions.ToString(2), false) == 0)
            {
                CS_locals1._VB_Local_Architecture = "i686";
            }
            else if (Operators.CompareString(_VB_Local_Architecture, Conversions.ToString(4), false) == 0)
            {
                CS_locals1._VB_Local_Architecture = "x86_64";
            }
            else if (Operators.CompareString(_VB_Local_Architecture, Conversions.ToString(5), false) == 0)
            {
                CS_locals1._VB_Local_Architecture = "arm64";
            }
            else
            {
                Logger.Warn(string.Format("CPU 是不支持的 {0} 架构，这可能会导致联机模块无法启动！", CS_locals1._VB_Local_Architecture), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                CS_locals1._VB_Local_Architecture = "arm64";
            }
            Logger.Info(string.Format("CPU 架构：{0}", CS_locals1._VB_Local_Architecture), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
            ModMain.Telemetry("联机开始", new string[0]);
            this.UpdateLoadingPage("正在检查联机模块版本……", "检查联机模块版本");
            if (!FileUtils.Exists(this.PathEasyTier + "联机模块 CLI.exe") || !FileUtils.Exists(this.PathEasyTier + "联机模块.exe") || !FileUtils.Exists(this.PathEasyTier + "Packet.dll"))
            {
                Settings.Set("LinkEasyTierVersion", -1, null);
            }
            int num = Settings.Get<int>("LinkEasyTierVersion", null);
            this.ServerVersion = (int)ModSecret.ServerConfig["Link"]["EasyTierVersion"];
            List<ModNet.NetFile> list = new List<ModNet.NetFile>();
            Logger.Info(string.Format("EasyTier 本地版本：{0}，需求版本：{1}", num, this.ServerVersion), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
            if (num < this.ServerVersion)
            {
                list.Add(new ModNet.NetFile(ModSecret.ServerConfig["Link"]["Downloads"].Select<JToken, string>(new Func<JToken, string>(CS_locals1._Lambda___0)), this.PathEasyTier + "EasyTier.zip", new FileChecker
                {
                    MinSize = 2097152L
                }, false));
            }
            this.UpdateLoadingPage("正在下载联机模块……", "下载联机模块");
            Task.Output = list;
        }

        private void InitLaunch(ModLoader.LoaderTask<int, int> Task)
        {
            this.UpdateLoadingPage("正在解压联机模块……", "解压联机模块");
            if (FileUtils.Exists(this.PathEasyTier + "EasyTier.zip"))
            {
                DirectoryUtils.Copy(DirectoryUtils.EnumerateDirectories(ModMain.RequestTaskTempFolder(false), true, "*").First<string>(), this.PathEasyTier);
                FileUtils.Move(this.PathEasyTier + "easytier-core.exe", this.PathEasyTier + "联机模块.exe", false);
                FileUtils.Move(this.PathEasyTier + "easytier-cli.exe", this.PathEasyTier + "联机模块 CLI.exe", false);
                FileUtils.Delete(this.PathEasyTier + "EasyTier.zip", false);
                Settings.Set("LinkEasyTierVersion", this.ServerVersion, null);
            }
            Task.Progress = 0.07;
            this.UpdateLoadingPage("正在获取节点列表……", "获取节点列表");
            string text = Settings.Get<string>("LinkCustomPeer", null);
            List<string> list;
            if (string.IsNullOrWhiteSpace(text))
            {
                if (this.DiscoverNodeID == -2 && !this.IsServerSide)
                {
                    this.Panic("未填写自定义节点设置", string.Format("$你必须在 {0}自定义节点{1} 设置中填写与房主相同的内容，{2}才能进入该房间！", ModBase.vbLQ, ModBase.vbRQ, "\r\n"));
                    return;
                }
                list = this.GetTargetPeers();
            }
            else
            {
                if (this.DiscoverNodeID != -2 && !this.IsServerSide)
                {
                    ModMain.Hint("房主可能没有使用自定义节点设置，请确认你们的自定义节点设置是否一致！", ModMain.HintType.Blue, true);
                }
                list = text.Split("，,".ToCharArray()).Select<string, string>((PageLinkMain._Closure___._I48_0 == null) ? (PageLinkMain._Closure___._I48_0 = new Func<string, string>(PageLinkMain._Closure___._I._Lambda___48_0)) : PageLinkMain._Closure___._I48_0).Where<string>((PageLinkMain._Closure___._I48_1 == null) ? (PageLinkMain._Closure___._I48_1 = new Func<string, bool>(PageLinkMain._Closure___._I._Lambda___48_1)) : PageLinkMain._Closure___._I48_1)
                    .ToList<string>();
                Logger.Info(string.Format("使用自定义节点：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
            }
            Task.Progress = 0.13;
            this.UpdateLoadingPage("正在启动联机模块……", "启动联机模块");
            List<int> list2 = ModNet.FindFreePorts(5, new int[] { this.ServerPort });
            this.ClientPort = list2[0];
            this.RPCPort = list2[1];
            int num = list2[2];
            string text2 = (string)ModSecret.ServerConfig["Link"]["Argument"];
            text2 += string.Format(" --network-name={0} --network-secret={1} --listeners {2} --rpc-portal {3} --private-mode true", new object[] { this.NetworkName, this.NetworkSecret, num, this.RPCPort });
            string text3 = (this.IsServerSide ? "Server-" : "Client-") + ModBase.Identify.GetStableHashCode().ConvertRadix(36).TrimStart(new char[] { '-' })
                .EnsureLength('0', 6);
            if (this.IsServerSide)
            {
                text2 += string.Format(" -i 10.114.114.114 --hostname={0} --tcp-whitelist={1} --udp-whitelist={2}", text3, this.ServerPort, this.ServerPort);
            }
            else
            {
                text2 += string.Format(" -d --hostname={0} --tcp-whitelist=0 --udp-whitelist=0", text3);
                this.ClientAddress = string.Format("localhost:{0}", this.ClientPort);
                text2 += string.Format(" --port-forward tcp://[{0}]:{1}/10.114.114.114:{2}", IPAddress.IPv6Loopback, this.ClientPort, this.ServerPort);
                text2 += string.Format(" --port-forward udp://[{0}]:{1}/10.114.114.114:{2}", IPAddress.IPv6Loopback, this.ClientPort, this.ServerPort);
                text2 += string.Format(" --port-forward tcp://{0}:{1}/10.114.114.114:{2}", IPAddress.Loopback, this.ClientPort, this.ServerPort);
                text2 += string.Format(" --port-forward udp://{0}:{1}/10.114.114.114:{2}", IPAddress.Loopback, this.ClientPort, this.ServerPort);
            }
            try
            {
                foreach (string text4 in list)
                {
                    text2 += string.Format(" -p=\"{0}\"", text4);
                }
            }
            finally
            {
                List<string>.Enumerator enumerator = default;
                ((IDisposable)enumerator).Dispose();
            }
            if (Settings.Get<int>("LinkLatencyMode", null) == 1)
            {
                text2 += " --latency-first";
            }
            this.ProcessStart(text2);
            Task.Progress = 0.15;
            int num2 = 0;
            double peerPing;
            for (;;)
            {
                Thread.Sleep(500);
                this.CheckCrash();
                this.RefreshPeerLoader.WaitForExit(null, null, true, default(CancellationToken), null);
                peerPing = this.GetPeerPing();
                if (peerPing > 0.0)
                {
                    break;
                }
                double progress = Task.Progress;
                int num3 = ((this.Peers == null) ? (-1) : this.Peers.Where<PageLinkMain.Peer>((PageLinkMain._Closure___._I48_2 == null) ? (PageLinkMain._Closure___._I48_2 = new Func<PageLinkMain.Peer, bool>(PageLinkMain._Closure___._I._Lambda___48_2)) : PageLinkMain._Closure___._I48_2).Count<PageLinkMain.Peer>());
                if (num3 != -1)
                {
                    if (num3 != 0)
                    {
                        this.UpdateLoadingPage("正在连接到房主……", "连接房主");
                        Task.Progress = (Task.Progress + 0.02).Clamp(Math.Min(0.45 + (double)num3 * 0.05, 0.65), 0.95);
                    }
                    else
                    {
                        this.UpdateLoadingPage("正在连接到节点……", "连接节点");
                        Task.Progress = (Task.Progress + 0.02).Clamp(this.IsServerSide ? 0.5 : 0.3, this.IsServerSide ? 0.95 : 0.5);
                    }
                }
                else
                {
                    Task.Progress = (Task.Progress + 0.02).Clamp(0.15, 0.25);
                }
                checked
                {
                    if (progress != Task.Progress)
                    {
                        num2 = 0;
                    }
                    else if (num2 < 60)
                    {
                        num2++;
                    }
                    else if (num3 != -1)
                    {
                        if (num3 != 0)
                        {
                            this.Panic("无法连接到房主", string.Format("可能的原因：{0}- 你或者房主的网络环境不佳{1}- 房主已关闭房间{2}- 邀请码输错了", "\r\n", "\r\n", "\r\n"));
                        }
                        else
                        {
                            this.Panic("无法连接到节点", "请检查你的网络环境是否良好。");
                        }
                    }
                    else
                    {
                        this.Panic("无法启动联机模块", "近期日志：\r\n" + this.LogHistory.Join("\r\n"));
                    }
                    if (Task.IsCanceled)
                    {
                        goto IL_06E7;
                    }
                }
            }
            Logger.Info(string.Format("已与目标建立连接，当前 Ping 为 {0:0.0}ms", peerPing), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
            ModMain.Telemetry("联机成功", new string[]
            {
                "Server",
                Conversions.ToString(this.IsServerSide),
                "NAT",
                Conversions.ToString((int)this.NATType)
            });
            IL_06E7:
            if (Task.IsCanceled)
            {
                throw new OperationCanceledException();
            }
            checked
            {
                if (!this.IsServerSide)
                {
                    this.UpdateLoadingPage("连接优化中……", "优化连接");
                    Task.Progress = 0.999;
                    int num4 = 1;
                    for (;;)
                    {
                        PageLinkMain.Peer targetPeer = this.GetTargetPeer();
                        if (targetPeer != null && !targetPeer.Relay && targetPeer.Ping < 100.0)
                        {
                            return;
                        }
                        if (Task.IsCanceled)
                        {
                            break;
                        }
                        Thread.Sleep(100);
                        num4++;
                        if (num4 > 50)
                        {
                            return;
                        }
                    }
                    throw new OperationCanceledException();
                }
            }
        }

        private List<string> GetTargetPeers()
        {
            List<string> list = new List<string>();
            int num = -1;
            int num2 = ModSecret.ServerConfig["Link"]["DiscoverPeerId"].ToObject<int>();
            string text = ModSecret.ServerConfig["Link"]["DiscoverPeer"].ToString();
            try
            {
                bool flag = false;
                JObject jobject;
                try
                {
                    jobject = (JObject)ModNet.NetRequestByClient("https://uptime.easytier.cn/api/nodes?page=1&per_page=1000", null, null, null, "*/*", 25000, null, true, null, false, true).DeserializeJson();
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "从源站获取节点列表失败，将使用 CDN 缓存", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                    jobject = (JObject)ModNet.NetRequestByClientRetry("https://easytier.meloong.com/?page=1&per_page=1000", null, null, null, null, "*/*", null, true, null, false).DeserializeJson();
                    flag = true;
                }
                List<JObject> list2 = new List<JObject>();
                List<string> list3 = ModSecret.ServerConfig["Link"]["PeersBlackList"].Select<JToken, string>((PageLinkMain._Closure___._I49_0 == null) ? (PageLinkMain._Closure___._I49_0 = new Func<JToken, string>(PageLinkMain._Closure___._I._Lambda___49_0)) : PageLinkMain._Closure___._I49_0).ToList<string>();
                try
                {
                    foreach (JToken jtoken in ((IEnumerable<JToken>)jobject["data"]["items"]))
                    {
                        JObject jobject2 = (JObject)jtoken;
                        int num3 = jobject2["id"].ToObject<int>();
                        if (jobject2["is_active"].ToObject<bool>() && jobject2["is_approved"].ToObject<bool>())
                        {
                            if (num3 != this.DiscoverNodeID)
                            {
                                if (num3 == num2 || list3.Contains(jobject2["address"].ToString()) || (jobject2["usage_percentage"].ToObject<double>() == 0.0 && ModBase.RandomInteger(1, 100) != 1))
                                {
                                    continue;
                                }
                                List<string> list4 = jobject2["tags"].Select<JToken, string>((PageLinkMain._Closure___._I49_1 == null) ? (PageLinkMain._Closure___._I49_1 = new Func<JToken, string>(PageLinkMain._Closure___._I._Lambda___49_1)) : PageLinkMain._Closure___._I49_1).ToList<string>();
                                if (!list4.Contains("国内") || !list4.Contains("MC中继"))
                                {
                                    continue;
                                }
                            }
                            double num4 = jobject2["usage_percentage"].ToObject<double>();
                            num4 *= 110.0 - jobject2["health_percentage_24h"].ToObject<double>();
                            jobject2["load"] = num4;
                            list2.Add(jobject2);
                        }
                    }
                }
                finally
                {
                    IEnumerator<JToken> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                if (!flag)
                {
                    list2 = list2.OrderBy<JObject, double>((PageLinkMain._Closure___._I49_2 == null) ? (PageLinkMain._Closure___._I49_2 = new Func<JObject, double>(PageLinkMain._Closure___._I._Lambda___49_2)) : PageLinkMain._Closure___._I49_2).ToList<JObject>();
                }
                else
                {
                    list2 = list2.Shuffle<JObject>().ToList<JObject>();
                }
                JObject jobject3 = null;
                if (this.DiscoverNodeID == -1)
                {
                    jobject3 = list2.FirstOrDefault<JObject>((PageLinkMain._Closure___._I49_3 == null) ? (PageLinkMain._Closure___._I49_3 = new Func<JObject, bool>(PageLinkMain._Closure___._I._Lambda___49_3)) : PageLinkMain._Closure___._I49_3);
                }
                else if (this.DiscoverNodeID > 0)
                {
                    jobject3 = list2.FirstOrDefault<JObject>(new Func<JObject, bool>(this._Lambda___49_4));
                    if (jobject3 == null)
                    {
                        Logger.Warn(string.Format("未找到 ID {0} 的发现节点", this.DiscoverNodeID), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                        this.Panic("房间已过期", "请让房主重新创建房间！");
                        throw new OperationCanceledException();
                    }
                }
                if (jobject3 == null)
                {
                    jobject3 = new JObject
                    {
                        { "address", text },
                        { "id", num2 }
                    };
                    Logger.Warn("将使用回退发现节点", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                }
                list.Add(jobject3["address"].ToString());
                num = jobject3["id"].ToObject<int>();
                Logger.Info(string.Format("发现节点：{0} (ID: {1})", jobject3["address"], num), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                if (ModBase.ModeDebug || !this.IsServerSide)
                {
                    int num5 = ModSecret.ServerConfig["Link"]["RandomPeer"].ToObject<int>();
                    List<JObject> list5 = list2.Where<JObject>((PageLinkMain._Closure___._I49_5 == null) ? (PageLinkMain._Closure___._I49_5 = new Func<JObject, bool>(PageLinkMain._Closure___._I._Lambda___49_5)) : PageLinkMain._Closure___._I49_5).ToList<JObject>();
                    if (list5.Count < num5)
                    {
                        throw new Exception(string.Format("可用的中继节点数量不足，需要 {0} 个，实际 {1} 个", num5, list5.Count));
                    }
                    list.AddRange(list5.Take<JObject>(num5).Select<JObject, string>((PageLinkMain._Closure___._I49_6 == null) ? (PageLinkMain._Closure___._I49_6 = new Func<JObject, string>(PageLinkMain._Closure___._I._Lambda___49_6)) : PageLinkMain._Closure___._I49_6));
                }
            }
            catch (Exception ex2)
            {
                ex2.ThrowIfCanceled();
                Logger.Error(ex2, "获取节点列表失败，联机质量可能受到影响", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                list.AddRange(ModSecret.ServerConfig["Link"]["Peers"].Select<JToken, string>((PageLinkMain._Closure___._I49_7 == null) ? (PageLinkMain._Closure___._I49_7 = new Func<JToken, string>(PageLinkMain._Closure___._I._Lambda___49_7)) : PageLinkMain._Closure___._I49_7));
                if (num <= 0)
                {
                    list.Add(text);
                    num = num2;
                }
            }
            if (((JObject)ModSecret.ServerConfig["Link"]).ContainsKey("EnableForcedPeer"))
            {
                list.Add("tcp://mc1.easytier.cn:55558");
                if (num <= 0)
                {
                    num = 94;
                }
            }
            list.AddRange(ModSecret.ServerConfig["Link"]["MandatoryPeers"].Select<JToken, string>((PageLinkMain._Closure___._I49_8 == null) ? (PageLinkMain._Closure___._I49_8 = new Func<JToken, string>(PageLinkMain._Closure___._I._Lambda___49_8)) : PageLinkMain._Closure___._I49_8));
            this.DiscoverNodeID = num;
            return list.Distinct<string>().ToList<string>();
        }

        public static void ProcessStop()
        {
            try
            {
                foreach (Process process in Process.GetProcesses())
                {
                    int id = process.Id;
                    Process processCore = PageLinkMain.ProcessCore;
                    int? num = ((processCore != null) ? new int?(processCore.Id) : null);
                    if (!((num != null) ? new bool?(id == num.GetValueOrDefault()) : null).GetValueOrDefault() && (Operators.CompareString(process.ProcessName, "联机模块", false) == 0 || Operators.CompareString(process.ProcessName, "联机模块 CLI", false) == 0))
                    {
                        try
                        {
                            Logger.Info(string.Format("停止残留的联机模块，PID：{0}，进程名：{1}", process.Id, process.ProcessName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                            process.Kill();
                            process.Close();
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, string.Format("结束进程失败（{0}，PID {1}）", process.ProcessName, process.Id), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                        }
                    }
                }
                if (PageLinkMain.ProcessCore != null && !PageLinkMain.ProcessCore.HasExited)
                {
                    Logger.Info(string.Format("停止所启动的联机模块，PID：{0}", PageLinkMain.ProcessCore.Id), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                    PageLinkMain.ProcessCore.Kill();
                    PageLinkMain.ProcessCore.Close();
                    PageLinkMain.ProcessCore = null;
                }
                AutoResetEvent processOutputHandle = PageLinkMain.ProcessOutputHandle;
                if (processOutputHandle != null)
                {
                    processOutputHandle.Dispose();
                }
                AutoResetEvent processErrorHandle = PageLinkMain.ProcessErrorHandle;
                if (processErrorHandle != null)
                {
                    processErrorHandle.Dispose();
                }
            }
            catch (Exception ex2)
            {
                Logger.Warn(ex2, "停止联机模块失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
            }
        }

        private void Panic(string Brief, string Detail)
        {
            if (Detail.Contains("failed to listen on "))
            {
                Detail = string.Format("监听端口失败。{0}请点击重试，如果还是出现此错误，可以重启电脑解决。{1}{2}{3}", new object[] { "\r\n", "\r\n", "\r\n", Detail });
            }
            if (PageLinkMain.LinkState == PageLinkMain.LinkStates.Loading)
            {
                this.FailReason = Brief;
                this.LinkLoader.Failed(new Exception(Detail));
                return;
            }
            ModMain.OurMsgBox(Detail, "联机出错：" + Brief, "确定", "", "", true, true, false, null, null, null);
            this.ChangeState(PageLinkMain.LinkStates.Waiting);
            ModMain.Telemetry("联机失败", new string[]
            {
                "Exception",
                ModMinecraft.FilterUserName(Brief + "：" + Detail, '*')
            });
        }

        private void ProcessStart(string Arguments)
        {
            PageLinkMain._Closure___55_0 CS_locals1 = new PageLinkMain._Closure___55_0();
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = this.PathEasyTier + "联机模块.exe",
                Arguments = Arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };
            Logger.Info(string.Format("正在启动 EasyTier：{0}", Arguments), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
            PageLinkMain.ProcessCore = new Process
            {
                StartInfo = processStartInfo
            };
            CS_locals1._VB_Local_LogLineHandler = new VB_AnonymousDelegate_13<object, DataReceivedEventArgs, AutoResetEvent>(this._Lambda___55_0);
            PageLinkMain.ProcessOutputHandle = new AutoResetEvent(false);
            PageLinkMain.ProcessCore.OutputDataReceived += CS_locals1._Lambda___1;
            PageLinkMain.ProcessErrorHandle = new AutoResetEvent(false);
            PageLinkMain.ProcessCore.ErrorDataReceived += CS_locals1._Lambda___2;
            PageLinkMain.ProcessCore.Start();
            PageLinkMain.ProcessCore.BeginOutputReadLine();
            PageLinkMain.ProcessCore.BeginErrorReadLine();
        }

        private void ProcessLogLine(string Line)
        {
            Logger.Info(Line, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
            this.LogHistory.Enqueue(Line);
            if (this.LogHistory.Count >= 10)
            {
                this.LogHistory.Dequeue();
            }
            if (Line.ContainsIgnoreCase("new peer connection added"))
            {
                Logger.Warn(string.Format("已建立连接：{0}", Line.RegexSeek("(?<=remote_addr.+?\")[^\"}]{3,}", RegexOptions.None) ?? Line), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                this.Update();
                return;
            }
            if (Line.ContainsIgnoreCase("peer connection removed"))
            {
                Logger.Warn(string.Format("已断开连接：{0}", Line.RegexSeek("(?<=remote_addr.+?\")[^\"}]{3,}", RegexOptions.None) ?? Line), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                this.Update();
            }
        }

        private void RefreshPeer()
        {
            try
            {
                string text = "";
                if (!text.Contains("lat_ms"))
                {
                    throw new Exception("CLI 调用失败：\r\n" + text);
                }
                if (ModBase.GetUuid() % (ModBase.ModeDebug ? 23 : 103) == 0)
                {
                    Logger.Info(string.Format("CLI 输出抽样：{0}{1}", "\r\n", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                }
                List<PageLinkMain.Peer> list = new List<PageLinkMain.Peer>();
                try
                {
                    foreach (JToken jtoken in ((JArray)text.DeserializeJson()))
                    {
                        JObject jobject = (JObject)jtoken;
                        try
                        {
                            PageLinkMain.Peer peer = new PageLinkMain.Peer(jobject);
                            if (peer.Type == PageLinkMain.Peer.Types.Self)
                            {
                                this.NATType = peer.NATType;
                            }
                            else
                            {
                                list.Add(peer);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, string.Format("错误的信息（{0}）", jobject), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                        }
                    }
                }
                finally
                {
                    IEnumerator<JToken> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                this.Peers = list;
                this.FailCount = 0;
            }
            catch (Exception ex2)
            {
                Logger.Warn(ex2, "获取节点信息失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                if (PageLinkMain.LinkState == PageLinkMain.LinkStates.Finished)
                {
                    ref int ptr = ref this.FailCount;
                    this.FailCount = checked(ptr + 1);
                    if (this.FailCount >= 4)
                    {
                        this.Panic("获取节点信息失败", ex2.Message);
                    }
                }
            }
        }

        private double GetPeerPing()
        {
            PageLinkMain.Peer targetPeer = this.GetTargetPeer();
            double num;
            if (targetPeer == null)
            {
                num = 0.0;
            }
            else
            {
                num = targetPeer.Ping;
            }
            return num;
        }

        private PageLinkMain.Peer GetTargetPeer()
        {
            PageLinkMain._Closure___66_0 CS_locals1 = new PageLinkMain._Closure___66_0();
            PageLinkMain.Peer peer;
            if (this.Peers == null)
            {
                peer = null;
            }
            else
            {
                IEnumerable<PageLinkMain.Peer> enumerable;
                if (this.IsServerSide)
                {
                    enumerable = this.Peers.Where<PageLinkMain.Peer>((PageLinkMain._Closure___._I66_0 == null) ? (PageLinkMain._Closure___._I66_0 = new Func<PageLinkMain.Peer, bool>(PageLinkMain._Closure___._I._Lambda___66_0)) : PageLinkMain._Closure___._I66_0);
                }
                else
                {
                    enumerable = this.Peers.Where<PageLinkMain.Peer>((PageLinkMain._Closure___._I66_1 == null) ? (PageLinkMain._Closure___._I66_1 = new Func<PageLinkMain.Peer, bool>(PageLinkMain._Closure___._I._Lambda___66_1)) : PageLinkMain._Closure___._I66_1);
                }
                if (!enumerable.Any<PageLinkMain.Peer>())
                {
                    peer = null;
                }
                else
                {
                    CS_locals1._VB_Local_MinPing = enumerable.Min<PageLinkMain.Peer>((PageLinkMain._Closure___._I66_2 == null) ? (PageLinkMain._Closure___._I66_2 = new Func<PageLinkMain.Peer, double>(PageLinkMain._Closure___._I._Lambda___66_2)) : PageLinkMain._Closure___._I66_2);
                    peer = enumerable.First<PageLinkMain.Peer>(new Func<PageLinkMain.Peer, bool>(CS_locals1._Lambda___3));
                }
            }
            return peer;
        }

        private int PeopleCount
        {
            get
            {
                int num;
                if (this.Peers == null)
                {
                    num = 0;
                }
                else
                {
                    num = checked(this.Peers.Where<PageLinkMain.Peer>((PageLinkMain._Closure___._I68_0 == null) ? (PageLinkMain._Closure___._I68_0 = new Func<PageLinkMain.Peer, bool>(PageLinkMain._Closure___._I._Lambda___68_0)) : PageLinkMain._Closure___._I68_0).Count<PageLinkMain.Peer>() + 1);
                }
                return num;
            }
        }

        private void StartTimerThread()
        {
            if (this._STATIC_StartTimerThread_2001_IsTimerStarted_Init == null)
            {
                Interlocked.CompareExchange<StaticLocalInitFlag>(ref this._STATIC_StartTimerThread_2001_IsTimerStarted_Init, new StaticLocalInitFlag(), null);
            }
            bool flag = false;
            try
            {
                Monitor.Enter(this._STATIC_StartTimerThread_2001_IsTimerStarted_Init, ref flag);
                if (this._STATIC_StartTimerThread_2001_IsTimerStarted_Init.State == 0)
                {
                    this._STATIC_StartTimerThread_2001_IsTimerStarted_Init.State = 2;
                    this._STATIC_StartTimerThread_2001_IsTimerStarted = false;
                }
                else if (this._STATIC_StartTimerThread_2001_IsTimerStarted_Init.State == 2)
                {
                    throw new IncompleteInitialization();
                }
            }
            finally
            {
                this._STATIC_StartTimerThread_2001_IsTimerStarted_Init.State = 1;
                if (flag)
                {
                    Monitor.Exit(this._STATIC_StartTimerThread_2001_IsTimerStarted_Init);
                }
            }
            if (!this._STATIC_StartTimerThread_2001_IsTimerStarted)
            {
                ModBase.RunInNewThread(new Action(this._Lambda___69_0), "Link Timer", ThreadPriority.Normal);
            }
        }

        private void Update()
        {
            if (PageLinkMain.LinkState == PageLinkMain.LinkStates.Finished)
            {
                object lockState = this.RefreshPeerLoader.LockState;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                lock (lockState)
                {
                    if (this.RefreshPeerLoader.State != ModBase.LoadState.Loading)
                    {
                        this.RefreshPeerLoader.Start(null, true);
                    }
                }
                if (ModMain.FrmMain.PageCurrent == FormMain.PageType.Link)
                {
                    ModBase.RunInUi(new Action(this._Lambda___70_0), false);
                }
                this.CheckCrash();
                if (!this.Peers.Any<PageLinkMain.Peer>())
                {
                    this.Panic("网络连接已断开", "请检查你的网络环境是否正常。");
                    return;
                }
                if (!this.IsServerSide && !this.Peers.Any<PageLinkMain.Peer>((PageLinkMain._Closure___._I70_4 == null) ? (PageLinkMain._Closure___._I70_4 = new Func<PageLinkMain.Peer, bool>(PageLinkMain._Closure___._I._Lambda___70_4)) : PageLinkMain._Closure___._I70_4))
                {
                    ModMain.OurMsgBox("房主已离开房间！", "联机结束", "确定", "", "", false, true, false, null, null, null);
                    this.ChangeState(PageLinkMain.LinkStates.Waiting);
                    return;
                }
                if (!this.IsServerSide)
                {
                    try
                    {
                        if (this._STATIC_Update_2001_BroadcastSocket_Init == null)
                        {
                            Interlocked.CompareExchange<StaticLocalInitFlag>(ref this._STATIC_Update_2001_BroadcastSocket_Init, new StaticLocalInitFlag(), null);
                        }
                        bool flag2 = false;
                        try
                        {
                            Monitor.Enter(this._STATIC_Update_2001_BroadcastSocket_Init, ref flag2);
                            if (this._STATIC_Update_2001_BroadcastSocket_Init.State == 0)
                            {
                                this._STATIC_Update_2001_BroadcastSocket_Init.State = 2;
                                this._STATIC_Update_2001_BroadcastSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);
                            }
                            else if (this._STATIC_Update_2001_BroadcastSocket_Init.State == 2)
                            {
                                throw new IncompleteInitialization();
                            }
                        }
                        finally
                        {
                            this._STATIC_Update_2001_BroadcastSocket_Init.State = 1;
                            if (flag2)
                            {
                                Monitor.Exit(this._STATIC_Update_2001_BroadcastSocket_Init);
                            }
                        }
                        this._STATIC_Update_2001_BroadcastSocket.SendTo(Encoding.UTF8.GetBytes(string.Format("[MOTD]PCL 联机房间[/MOTD][AD]{0}[/AD]", this.ClientPort)), SocketFlags.None, new IPEndPoint(IPAddress.Loopback, 4445));
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "广播联机房间端口失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                    }
                }
            }
        }

        private void CheckCrash()
        {
            if (PageLinkMain.ProcessCore == null || PageLinkMain.ProcessCore.HasExited)
            {
                this.Panic("联机模块已崩溃", "近期日志：\r\n" + this.LogHistory.Join("\r\n"));
            }
        }


        internal virtual OurCard CardLoad_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._CardLoad;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this._Lambda___R78_6);
                SizeChangedEventHandler sizeChangedEventHandler = new SizeChangedEventHandler(this._Lambda___R78_7);
                OurCard ourCard = this._CardLoad;
                if (ourCard != null)
                {
                    ourCard.MouseLeftButtonUp -= mouseButtonEventHandler;
                    ourCard.SizeChanged -= sizeChangedEventHandler;
                }
                this._CardLoad = value;
                ourCard = this._CardLoad;
                if (ourCard != null)
                {
                    ourCard.MouseLeftButtonUp += mouseButtonEventHandler;
                    ourCard.SizeChanged += sizeChangedEventHandler;
                }
            }
        }




        internal virtual OurIconButton BtnLoadCancel_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnLoadCancel;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this._Lambda___R94_8);
                OurIconButton ourIconButton = this._BtnLoadCancel;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                this._BtnLoadCancel = value;
                ourIconButton = this._BtnLoadCancel;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }






        internal virtual OurCard PanSelectCreate_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._PanSelectCreate;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this._Lambda___R118_9);
                OurCard ourCard = this._PanSelectCreate;
                if (ourCard != null)
                {
                    ourCard.MouseLeftButtonUp -= mouseButtonEventHandler;
                }
                this._PanSelectCreate = value;
                ourCard = this._PanSelectCreate;
                if (ourCard != null)
                {
                    ourCard.MouseLeftButtonUp += mouseButtonEventHandler;
                }
            }
        }

        internal virtual OurCard PanSelectJoin_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._PanSelectJoin;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = ((PageLinkMain._Closure___._IR122_10 == null) ? (PageLinkMain._Closure___._IR122_10 = new MouseButtonEventHandler(PageLinkMain._Closure___._I._Lambda___R122_10)) : PageLinkMain._Closure___._IR122_10);
                RoutedEventHandler routedEventHandler = new RoutedEventHandler(this._Lambda___R122_11);
                OurCard ourCard = this._PanSelectJoin;
                if (ourCard != null)
                {
                    ourCard.MouseLeftButtonUp -= mouseButtonEventHandler;
                    ourCard.Loaded -= routedEventHandler;
                }
                this._PanSelectJoin = value;
                ourCard = this._PanSelectJoin;
                if (ourCard != null)
                {
                    ourCard.MouseLeftButtonUp += mouseButtonEventHandler;
                    ourCard.Loaded += routedEventHandler;
                }
            }
        }






        internal virtual OurButton BtnFinishCopy_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnFinishCopy;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R146_12);
                OurButton ourButton = this._BtnFinishCopy;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnFinishCopy = value;
                ourButton = this._BtnFinishCopy;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnFinishExit_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnFinishExit;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R150_13);
                OurButton ourButton = this._BtnFinishExit;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnFinishExit = value;
                ourButton = this._BtnFinishExit;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }








        internal virtual StackPanel BtnFinishIp_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnFinishIp;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this.BtnFinishIp_MouseLeftButtonUp);
                StackPanel stackPanel = this._BtnFinishIp;
                if (stackPanel != null)
                {
                    stackPanel.MouseLeftButtonUp -= mouseButtonEventHandler;
                }
                this._BtnFinishIp = value;
                stackPanel = this._BtnFinishIp;
                if (stackPanel != null)
                {
                    stackPanel.MouseLeftButtonUp += mouseButtonEventHandler;
                }
            }
        }



        [CompilerGenerated]
        private void _Lambda___R1_1(object a0, EventArgs a1)
        {
            this.LoaderInit();
        }

        [CompilerGenerated]
        private void _Lambda___R1_2(object a0, RoutedEventArgs a1)
        {
            this.StartTimerThread();
        }

        [CompilerGenerated]
        private void _Lambda___R1_3(object a0, RoutedEventArgs a1)
        {
            this.Update();
        }

        [CompilerGenerated]
        private void _Lambda___R1_4(ModLoader.LoaderTask<int, List<PageLinkMain.Peer>> a0)
        {
            this.RefreshPeer();
        }

        [CompilerGenerated]
        private void _Lambda___6_0(ModLoader.LoaderBase Loader, ModBase.LoadState NewState, ModBase.LoadState OldState)
        {
            if (NewState == ModBase.LoadState.Finished)
            {
                this.ChangeState(PageLinkMain.LinkStates.Finished);
                return;
            }
            if (NewState != ModBase.LoadState.Failed)
            {
                return;
            }
            ModMain.Telemetry("联机失败", new string[]
            {
                "Exception",
                ModMinecraft.FilterUserName(Loader.Error.GetDisplay(true), '*')
            });
            this.ChangeState(PageLinkMain.LinkStates.Failed);
        }

        [CompilerGenerated]
        private bool _Lambda___49_4(JObject n)
        {
            return n["id"].ToObject<int>() == this.DiscoverNodeID;
        }

        [CompilerGenerated]
        private void _Lambda___55_0(object sender, DataReceivedEventArgs e, AutoResetEvent Handle)
        {
            try
            {
                if (e.Data == null)
                {
                    Handle.Set();
                }
                else
                {
                    this.ProcessLogLine(e.Data);
                }
            }
            catch (ObjectDisposedException ex)
            {
            }
            catch (Exception ex2)
            {
                Logger.Warn(ex2, "读取 EasyTier 信息失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
            }
        }

        [CompilerGenerated]
        private void _Lambda___69_0()
        {
            int num = 0;
            checked
            {
                for (;;)
                {
                    try
                    {
                        Thread.Sleep(200);
                        num += 200;
                        if (num >= 1000)
                        {
                            num = 0;
                            this.Update();
                        }
                        if (PageLinkMain.LinkState == PageLinkMain.LinkStates.Loading)
                        {
                            ModBase.RunInUi(new Action(this._Lambda___R69_5), false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "联机模块主时钟出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLink\\PageLinkMain.xaml.vb");
                        Thread.Sleep(10000);
                    }
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R69_5()
        {
            this.UpdateProgressBar(-1.0);
        }

        [CompilerGenerated]
        private void _Lambda___70_0()
        {
            ModAnimation.AniStart(ModAnimation.AaRotateTransform(this.ImgFinishLogo, 500.0, 5000, 0, null, false), "Link Logo Rotation", false);
            double peerPing = this.GetPeerPing();
            bool flag = peerPing % 500.0 == 0.0 || Operators.CompareString(string.Format("{0:0.0}", peerPing), "1.0", false) == 0 || this.FailCount > 0;
            if (flag)
            {
                this.LabFinishPing.Text = "连接优化中";
            }
            else
            {
                this.LabFinishPing.Text = ((peerPing >= 10.0) ? string.Format("{0:0}\u200ams", peerPing) : string.Format("{0:0.0}\u200ams", peerPing));
            }
            this.LabFinishPlayer.Text = Conversions.ToString(this.PeopleCount) + "\u200a\u200a人";
            this.HintFinish.Visibility = Visibility.Collapsed;
            if (!this.IsServerSide && !flag)
            {
                PageLinkMain.Peer targetPeer = this.GetTargetPeer();
                if (targetPeer != null)
                {
                    if (targetPeer.Relay)
                    {
                        if (this.NATType >= PageLinkMain.NATTypes.Symmetric && targetPeer.NATType >= PageLinkMain.NATTypes.Symmetric)
                        {
                            this.HintFinish.Text = "你和房主的网络环境都不太好，";
                        }
                        else if (this.NATType >= PageLinkMain.NATTypes.Symmetric)
                        {
                            this.HintFinish.Text = "你的网络环境不太好，";
                        }
                        else if (targetPeer.NATType >= PageLinkMain.NATTypes.Symmetric)
                        {
                            this.HintFinish.Text = "房主的网络环境不太好，";
                        }
                        else
                        {
                            this.HintFinish.Text = "你或者房主的网络环境不太好，";
                        }
                        if (string.IsNullOrWhiteSpace(Settings.Get<string>("LinkCustomPeer", null)))
                        {
                            OurHint ourHint;
                            (ourHint = this.HintFinish).Text = ourHint.Text + "正使用社区节点进行中继。";
                        }
                        else
                        {
                            OurHint ourHint;
                            (ourHint = this.HintFinish).Text = ourHint.Text + "正通过自定义节点进行中继。";
                        }
                        this.HintFinish.Visibility = Visibility.Visible;
                        this.HintFinish.Theme = OurHint.Themes.Yellow;
                        return;
                    }
                    if (this.NATType < PageLinkMain.NATTypes.PortRestricted && targetPeer.NATType >= PageLinkMain.NATTypes.PortRestricted)
                    {
                        List<PageLinkMain.Peer> list = this.Peers.Where<PageLinkMain.Peer>((PageLinkMain._Closure___._I70_1 == null) ? (PageLinkMain._Closure___._I70_1 = new Func<PageLinkMain.Peer, bool>(PageLinkMain._Closure___._I._Lambda___70_1)) : PageLinkMain._Closure___._I70_1).ToList<PageLinkMain.Peer>();
                        if (list.Any<PageLinkMain.Peer>() && !list.Any<PageLinkMain.Peer>((PageLinkMain._Closure___._I70_2 == null) ? (PageLinkMain._Closure___._I70_2 = new Func<PageLinkMain.Peer, bool>(PageLinkMain._Closure___._I._Lambda___70_2)) : PageLinkMain._Closure___._I70_2) && !list.All<PageLinkMain.Peer>((PageLinkMain._Closure___._I70_3 == null) ? (PageLinkMain._Closure___._I70_3 = new Func<PageLinkMain.Peer, bool>(PageLinkMain._Closure___._I._Lambda___70_3)) : PageLinkMain._Closure___._I70_3))
                        {
                            this.HintFinish.Visibility = Visibility.Visible;
                            this.HintFinish.Theme = OurHint.Themes.Blue;
                            this.HintFinish.Text = "你的网络环境比房主更好！如果你来当房主，其他玩家或许能更加流畅！";
                        }
                    }
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R78_6(object a0, MouseButtonEventArgs a1)
        {
            this.CardLoad_MouseLeftButtonUp();
        }

        [CompilerGenerated]
        private void _Lambda___R78_7(object a0, SizeChangedEventArgs a1)
        {
            this.CardLoad_SizeChanged();
        }

        [CompilerGenerated]
        private void _Lambda___R94_8(object a0, EventArgs a1)
        {
            this.BtnLoadCancel_Click();
        }

        [CompilerGenerated]
        private void _Lambda___R118_9(object a0, MouseButtonEventArgs a1)
        {
            this.Create_MouseLeftButtonUp();
        }

        [CompilerGenerated]
        private void _Lambda___R122_11(object a0, RoutedEventArgs a1)
        {
            this.PanSelectJoin_Loaded();
        }

        [CompilerGenerated]
        private void _Lambda___R146_12(object a0, MouseButtonEventArgs a1)
        {
            this.Copy();
        }

        [CompilerGenerated]
        private void _Lambda___R150_13(object a0, MouseButtonEventArgs a1)
        {
            this.BtnFinishExit_Click();
        }

        private const int INVITE_CODE_VERSION = 2;

        public static PageLinkMain.LinkStates LinkState = PageLinkMain.LinkStates.Waiting;

        private static string LastCode = null;

        private static string AutoJoinCode = null;

        private string FailReason;

        private string PathEasyTier;

        private bool IsServerSide;

        private int ServerPort;

        private int ClientPort;

        private int RPCPort;

        private string ClientAddress;

        private int DiscoverNodeID;

        private string NetworkName;

        private string NetworkSecret;

        [CompilerGenerated]
        [AccessedThroughProperty("LinkLoader")]
        private ModLoader.LoaderCombo<int> _LinkLoader;

        private int ServerVersion;

        private static Process ProcessCore;

        private static AutoResetEvent ProcessOutputHandle;

        private static AutoResetEvent ProcessErrorHandle;

        private Queue<string> LogHistory;

        private List<PageLinkMain.Peer> Peers;

        private PageLinkMain.NATTypes NATType;

        private ModLoader.LoaderTask<int, List<PageLinkMain.Peer>> RefreshPeerLoader;

        private int FailCount;

        [CompilerGenerated]
        [AccessedThroughProperty("PanLoad")]
        private Grid _PanLoad;

        [CompilerGenerated]
        [AccessedThroughProperty("CardLoad")]
        private OurCard _CardLoad;

        [CompilerGenerated]
        [AccessedThroughProperty("Load")]
        private OurLoading _Load;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLoadTitle")]
        private TextBlock _LabLoadTitle;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLoadDesc")]
        private TextBlock _LabLoadDesc;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnLoadCancel")]
        private OurIconButton _BtnLoadCancel;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnProgressA")]
        private ColumnDefinition _ColumnProgressA;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnProgressB")]
        private ColumnDefinition _ColumnProgressB;

        [CompilerGenerated]
        [AccessedThroughProperty("RectProgressClip")]
        private RectangleGeometry _RectProgressClip;

        [CompilerGenerated]
        [AccessedThroughProperty("PanContent")]
        private Grid _PanContent;

        [CompilerGenerated]
        [AccessedThroughProperty("PanSelect")]
        private Grid _PanSelect;

        [CompilerGenerated]
        [AccessedThroughProperty("PanSelectCreate")]
        private OurCard _PanSelectCreate;

        [CompilerGenerated]
        [AccessedThroughProperty("PanSelectJoin")]
        private OurCard _PanSelectJoin;

        [CompilerGenerated]
        [AccessedThroughProperty("PanFinish")]
        private Grid _PanFinish;

        [CompilerGenerated]
        [AccessedThroughProperty("HintFinish")]
        private OurHint _HintFinish;

        [CompilerGenerated]
        [AccessedThroughProperty("ImgFinishLogo")]
        private Path _ImgFinishLogo;

        [CompilerGenerated]
        [AccessedThroughProperty("LabFinishTitle")]
        private TextBlock _LabFinishTitle;

        [CompilerGenerated]
        [AccessedThroughProperty("LabFinishDesc")]
        private TextBlock _LabFinishDesc;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnFinishCopy")]
        private OurButton _BtnFinishCopy;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnFinishExit")]
        private OurButton _BtnFinishExit;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnFinishPing")]
        private StackPanel _BtnFinishPing;

        [CompilerGenerated]
        [AccessedThroughProperty("LabFinishPing")]
        private TextBlock _LabFinishPing;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnFinishPlayer")]
        private StackPanel _BtnFinishPlayer;

        [CompilerGenerated]
        [AccessedThroughProperty("LabFinishPlayer")]
        private TextBlock _LabFinishPlayer;

        [CompilerGenerated]
        [AccessedThroughProperty("LineFinishPort")]
        private TextBlock _LineFinishPort;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnFinishPort")]
        private StackPanel _BtnFinishPort;

        [CompilerGenerated]
        [AccessedThroughProperty("LabFinishPort")]
        private TextBlock _LabFinishPort;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnFinishIp")]
        private StackPanel _BtnFinishIp;

        [CompilerGenerated]
        [AccessedThroughProperty("LabFinishIp")]
        private TextBlock _LabFinishIp;

        [CompilerGenerated]
        [AccessedThroughProperty("PanAlways")]
        private Grid _PanAlways;

        private bool _STATIC_StartTimerThread_2001_IsTimerStarted;

        private StaticLocalInitFlag _STATIC_StartTimerThread_2001_IsTimerStarted_Init;

        private Socket _STATIC_Update_2001_BroadcastSocket;

        private StaticLocalInitFlag _STATIC_Update_2001_BroadcastSocket_Init;

        public enum LinkStates
        {
            Waiting,
            Loading,
            Failed,
            Finished
        }

        private class Peer
        {
            public Peer(JObject Info)
            {
                string text = Info["hostname"].ToString();
                if (Operators.CompareString(Info["cost"].ToString(), "Local", false) == 0)
                {
                    this.Type = PageLinkMain.Peer.Types.Self;
                }
                else if (text.StartsWithF("Client", false))
                {
                    this.Type = PageLinkMain.Peer.Types.Client;
                }
                else if ((string)Info["ipv4"] == "10.114.114.114")
                {
                    this.Type = PageLinkMain.Peer.Types.Server;
                }
                else
                {
                    this.Type = PageLinkMain.Peer.Types.Misc;
                }
                double.TryParse((string)Info["lat_ms"], NumberStyles.Any, CultureInfo.InvariantCulture, out this.Ping);
                this.Name = text;
                this.Relay = Info["cost"].ToString().ContainsIgnoreCase("relay");
                this.NATType = EnumUtils.FromString<PageLinkMain.NATTypes>((string)Info["nat_type"]);
            }

            public override string ToString()
            {
                return string.Format("{0} - {1} - Ping {2:0.0}ms [中继? {3}] - NAT {4}", new object[] { this.Type, this.Name, this.Ping, this.Relay, this.NATType });
            }

            public readonly PageLinkMain.Peer.Types Type;

            public readonly double Ping;

            public readonly string Name;

            public readonly bool Relay;

            public readonly PageLinkMain.NATTypes NATType;

            public enum Types
            {
                Client,
                Server,
                Misc,
                Self
            }
        }

        private enum NATTypes
        {
            Unknown,
            OpenInternet,
            NoPAT,
            FullCone,
            Restricted,
            PortRestricted,
            Symmetric,
            SymUdpFirewall,
            SymmetricEasyInc,
            SymmetricEasyDec,
            Pending
        }

        [CompilerGenerated]
        [Serializable]
        private new sealed class _Closure___
        {
            internal string _Lambda___9_0()
            {
                string text = "";
                int num = 1;
                checked
                {
                    do
                    {
                        text += ModBase.RandomInteger(0, 35).ConvertRadix(36);
                        num++;
                    }
                    while (num <= 5);
                    return text.Replace("O", "0").Replace("I", "1");
                }
            }

            internal string _Lambda___48_0(string p)
            {
                return p.Trim();
            }

            internal bool _Lambda___48_1(string p)
            {
                return !string.IsNullOrEmpty(p);
            }

            internal bool _Lambda___48_2(PageLinkMain.Peer p)
            {
                return p.Ping > 0.0;
            }

            internal string _Lambda___49_0(JToken p)
            {
                return p.ToString();
            }

            internal string _Lambda___49_1(JToken t)
            {
                return t.ToString();
            }

            internal double _Lambda___49_2(JObject n)
            {
                return n["load"].ToObject<double>();
            }

            internal bool _Lambda___49_3(JObject n)
            {
                return !n["allow_relay"].ToObject<bool>();
            }

            internal bool _Lambda___49_5(JObject n)
            {
                return n["allow_relay"].ToObject<bool>();
            }

            internal string _Lambda___49_6(JObject n)
            {
                return n["address"].ToString();
            }

            internal string _Lambda___49_7(JToken p)
            {
                return p.ToString();
            }

            internal string _Lambda___49_8(JToken p)
            {
                return p.ToString();
            }

            internal bool _Lambda___66_0(PageLinkMain.Peer p)
            {
                return p.Ping > 0.0;
            }

            internal bool _Lambda___66_1(PageLinkMain.Peer p)
            {
                return p.Type == PageLinkMain.Peer.Types.Server && p.Ping > 0.0;
            }

            internal double _Lambda___66_2(PageLinkMain.Peer p)
            {
                return p.Ping;
            }

            internal bool _Lambda___68_0(PageLinkMain.Peer p)
            {
                return p.Type != PageLinkMain.Peer.Types.Misc;
            }

            internal bool _Lambda___70_1(PageLinkMain.Peer p)
            {
                return p.Type == PageLinkMain.Peer.Types.Client;
            }

            internal bool _Lambda___70_2(PageLinkMain.Peer p)
            {
                return p.Relay;
            }

            internal bool _Lambda___70_3(PageLinkMain.Peer p)
            {
                return p.NATType < PageLinkMain.NATTypes.PortRestricted;
            }

            internal bool _Lambda___70_4(PageLinkMain.Peer p)
            {
                return p.Type == PageLinkMain.Peer.Types.Server;
            }

            internal void _Lambda___R122_10(object a0, MouseButtonEventArgs a1)
            {
                PageLinkMain.Join();
            }

            public static readonly PageLinkMain._Closure___ _I = new PageLinkMain._Closure___();

            public static VB_AnonymousDelegate_0<string> _I9_0;

            public static Func<string, string> _I48_0;

            public static Func<string, bool> _I48_1;

            public static Func<PageLinkMain.Peer, bool> _I48_2;

            public static Func<JToken, string> _I49_0;

            public static Func<JToken, string> _I49_1;

            public static Func<JObject, double> _I49_2;

            public static Func<JObject, bool> _I49_3;

            public static Func<JObject, bool> _I49_5;

            public static Func<JObject, string> _I49_6;

            public static Func<JToken, string> _I49_7;

            public static Func<JToken, string> _I49_8;

            public static Func<PageLinkMain.Peer, bool> _I66_0;

            public static Func<PageLinkMain.Peer, bool> _I66_1;

            public static Func<PageLinkMain.Peer, double> _I66_2;

            public static Func<PageLinkMain.Peer, bool> _I68_0;

            public static Func<PageLinkMain.Peer, bool> _I70_1;

            public static Func<PageLinkMain.Peer, bool> _I70_2;

            public static Func<PageLinkMain.Peer, bool> _I70_3;

            public static Func<PageLinkMain.Peer, bool> _I70_4;

            public static MouseButtonEventHandler _IR122_10;
        }

        [CompilerGenerated]
        private sealed class _Closure___20_0
        {
            internal void _Lambda___0()
            {
                if (ModMain.FrmLinkMain != null && ModMain.FrmLinkMain.LabLoadDesc.IsLoaded)
                {
                    ModMain.FrmLinkMain.LabLoadDesc.Text = this._VB_Local_Title;
                }
            }

            public string _VB_Local_Title;
        }

        [CompilerGenerated]
        private sealed class _Closure___47_0
        {
            internal string _Lambda___0(JToken UrlEntry)
            {
                return UrlEntry.ToString().Replace("{arch}", this._VB_Local_Architecture);
            }

            public string _VB_Local_Architecture;
        }

        [CompilerGenerated]
        private sealed class _Closure___5_0
        {
            internal void _Lambda___0()
            {
                switch (this._VB_Local_NewState)
                {
                case PageLinkMain.LinkStates.Waiting:
                    this._VB_Me.SwitchToWaiting(this._VB_Local_OldState);
                    return;
                case PageLinkMain.LinkStates.Loading:
                    this._VB_Me.SwitchToLoading(this._VB_Local_OldState);
                    return;
                case PageLinkMain.LinkStates.Failed:
                    this._VB_Me.SwitchToFailed(this._VB_Local_OldState);
                    return;
                case PageLinkMain.LinkStates.Finished:
                    this._VB_Me.SwitchToFinished(this._VB_Local_OldState);
                    return;
                default:
                    return;
                }
            }

            public PageLinkMain.LinkStates _VB_Local_NewState;

            public PageLinkMain.LinkStates _VB_Local_OldState;

            public PageLinkMain _VB_Me;
        }

        [CompilerGenerated]
        private sealed class _Closure___55_0
        {
            internal void _Lambda___1(object sender, DataReceivedEventArgs e)
            {
                this._VB_Local_LogLineHandler(RuntimeHelpers.GetObjectValue(sender), e, PageLinkMain.ProcessOutputHandle);
            }

            internal void _Lambda___2(object sender, DataReceivedEventArgs e)
            {
                this._VB_Local_LogLineHandler(RuntimeHelpers.GetObjectValue(sender), e, PageLinkMain.ProcessErrorHandle);
            }

            public VB_AnonymousDelegate_13<object, DataReceivedEventArgs, AutoResetEvent> _VB_Local_LogLineHandler;
        }

        [CompilerGenerated]
        private sealed class _Closure___66_0
        {
            internal bool _Lambda___3(PageLinkMain.Peer p)
            {
                return p.Ping == this._VB_Local_MinPing;
            }

            public double _VB_Local_MinPing;
        }
    }
}
