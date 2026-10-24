using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;

namespace PCL
{
    [DesignerGenerated]
    public partial class OurMsgLogin : Grid
    {
        public OurMsgLogin(ModMain.OurMsgBoxConverter Converter)
        {
            base.Loaded += new RoutedEventHandler(this.Load);
            this.Uuid = ModBase.GetUuid();
            try
            {
                this.InitializeComponent();
                this.PanBorder_WithEvents = this.PanBorder;
                this.LabTitle_WithEvents = this.LabTitle;
                this.Btn1_WithEvents = this.Btn1;
                this.Btn3_WithEvents = this.Btn3;
                this.Btn1.Name = this.Btn1.Name + Conversions.ToString(ModBase.GetUuid());
                this.Btn2.Name = this.Btn2.Name + Conversions.ToString(ModBase.GetUuid());
                this.Btn3.Name = this.Btn3.Name + Conversions.ToString(ModBase.GetUuid());
                this.OurConverter = Converter;
                this.ShapeLine.StrokeThickness = ModBase.GetWPFSize(1.0);
                this.Data = (JObject)Converter.Content;
                this.Init();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "登录弹窗初始化失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\OurMsgLogin.xaml.vb");
            }
        }

        private void Load(object sender, EventArgs e)
        {
            try
            {
                base.Opacity = 0.0;
                ModAnimation.AniStart(ModAnimation.AaColor(ModMain.FrmMain.PanMsg, Panel.BackgroundProperty, (this.OurConverter.IsWarn ? new ModBase.OurColor(140.0, 80.0, 0.0, 0.0) : new ModBase.OurColor(90.0, 0.0, 0.0, 0.0)) - ModMain.FrmMain.PanMsg.Background, 200, 0, null, false), "PanMsg Background", false);
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaOpacity(this, 1.0, 120, 60, null, false),
                    ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___7_0), -this.TransformPos.Y, 300, 60, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false),
                    ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___7_1), -this.TransformRotate.Angle, 300, 60, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false)
                }, "OurMsgBox " + Conversions.ToString(this.Uuid), false);
                Logger.Info(string.Format("登录弹窗：{0}{1}{2}", this.LabTitle.Text, "\r\n", this.LabCaption.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\OurMsgLogin.xaml.vb");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "登录弹窗加载失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\OurMsgLogin.xaml.vb");
            }
        }

        private void Close()
        {
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaCode((OurMsgLogin._Closure___._I8_0 == null) ? (OurMsgLogin._Closure___._I8_0 = new ThreadStart(OurMsgLogin._Closure___._I._Lambda___8_0)) : OurMsgLogin._Closure___._I8_0, 30, false),
                ModAnimation.AaOpacity(this, -base.Opacity, 80, 20, null, false),
                ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___8_1), 20.0 - this.TransformPos.Y, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___8_2), 6.0 - this.TransformRotate.Angle, 150, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), false),
                ModAnimation.AaCode(new ThreadStart(this._Lambda___8_3), 0, true)
            }, "OurMsgBox " + Conversions.ToString(this.Uuid), false);
        }

        public void Btn1_Click()
        {
        }

        public void Btn3_Click()
        {
            this.Finished(new OperationCanceledException());
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            int num = 0;
            int num4;
            object obj;
            int num2 = 0;
            int __onErrorFrom_IL_0093 = -1;
            ONERR_IL_0093:
            try
            {
                if (__onErrorFrom_IL_0093 >= 0)
                {
                    if (__onErrorFrom_IL_0093 < 2) goto IL_0007;
                    if (__onErrorFrom_IL_0093 < 3) goto IL_0028;
                    goto IL_0093;
                }
                IL_0000:
                ProjectData.ClearProjectError();
                num = 1;
                IL_0007:
                num2 = 2;
                if (e.GetPosition(this.ShapeLine).Y > 2.0)
                {
                    goto IL_0034;
                }
                IL_0028:
                num2 = 3;
                ModMain.FrmMain.DragMove();
                IL_0034:
                goto IL_0093;
            }
            catch (Exception) when (num != 0)
            {
                __onErrorFrom_IL_0093 = num2;
                goto ONERR_IL_0093;
            }
            IL_0093:;
        }

        private void Finished(object Result)
        {
            if (!this.OurConverter.IsExited)
            {
                this.OurConverter.IsExited = true;
                this.OurConverter.Result = RuntimeHelpers.GetObjectValue(Result);
                ModBase.RunInUi(new Action(this.Close), false);
                Thread.Sleep(200);
                ModMain.FrmMain.ShowWindowToTop();
            }
        }

        private void Init()
        {
            this.UserCode = (string)this.Data["user_code"];
            this.DeviceCode = (string)this.Data["device_code"];
            this.Website = (string)this.Data["verification_uri"];
            this.LabTitle.Text = "登录 Minecraft";
            this.LabCaption.Text = string.Format("登录网页将自动开启，请在网页中输入 {0}（已自动复制）。", this.UserCode) + "\r\n\r\n如果网络环境不佳，网页可能一直加载不出来，届时请使用加速器或 VPN 改善网络环境。\r\n" + string.Format("你也可以用其他设备打开 {0} 并输入上述代码。", this.Website);
            CustomEventService.SetEventData(this.Btn1, this.Website);
            CustomEventService.SetEventData(this.Btn2, this.UserCode);
            ModBase.RunInNewThread(new Action(this.WorkThread), "OurMsgLogin", ThreadPriority.Normal);
        }

        private void WorkThread()
        {
            Thread.Sleep(2000);
            checked
            {
                if (!this.OurConverter.IsExited)
                {
                    ModBase.OpenWebsite(this.Website);
                    ModBase.ClipboardSet(this.UserCode, true);
                    Thread.Sleep((this.Data["interval"].ToObject<int>() - 1) * 1000);
                    int num = 0;
                    while (!this.OurConverter.IsExited)
                    {
                        try
                        {
                            JObject jobject = (JObject)ModNet.NetRequestByClient("https://login.microsoftonline.com/consumers/oauth2/v2.0/token", HttpMethod.Post, "grant_type=urn:ietf:params:oauth:grant-type:device_code&client_id=fe72edc2-3a6f-4280-90e8-e2beb64ce7e1&device_code=" + this.DeviceCode + "&scope=XboxLive.signin%20offline_access", "application/x-www-form-urlencoded", "*/*", 5000 + num * 5000, null, true, null, false, false).DeserializeJson();
                            ModLaunch.McLaunchLog(string.Format("令牌过期时间：{0} 秒", jobject["expires_in"]), LogLevel.Info);
                            ModMain.Hint("网页登录成功！", ModMain.HintType.Green, true);
                            this.Finished(new string[]
                            {
                                jobject["access_token"].ToString(),
                                jobject["refresh_token"].ToString()
                            });
                            break;
                        }
                        catch (Exception ex)
                        {
                            if (ex is HttpRequestCodeException)
                            {
                                string response = ((HttpRequestCodeException)ex).Response;
                                if (response.Contains("authorization_declined"))
                                {
                                    this.Finished(new Exception("$你拒绝了 PCL 申请的权限……"));
                                    break;
                                }
                                if (response.Contains("expired_token"))
                                {
                                    this.Finished(new Exception("$登录用时太长啦，重新试试吧！"));
                                    break;
                                }
                                if (response.Contains("Account security interrupt"))
                                {
                                    this.Finished(new Exception("$该账号由于安全问题无法登陆，请前往微软账户页获取更多信息。"));
                                    break;
                                }
                                if (response.Contains("service abuse"))
                                {
                                    this.Finished(new Exception("$非常抱歉，该账号已被微软封禁，无法登录。"));
                                    break;
                                }
                                if (response.Contains("AADSTS70000"))
                                {
                                    this.Finished(new ModBase.RestartException());
                                    break;
                                }
                                if (response.Contains("authorization_pending"))
                                {
                                    Thread.Sleep(2000);
                                    continue;
                                }
                            }
                            if (num > 2)
                            {
                                this.Finished(new Exception("登录轮询失败", ex));
                                break;
                            }
                            num++;
                            Logger.Warn(ex, string.Format("登录轮询第 {0} 次失败", num), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\OurMsgLogin.xaml.vb");
                            Thread.Sleep(2000);
                        }
                    }
                }
            }
        }



        internal virtual Border PanBorder_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._PanBorder;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this.Drag);
                Border border = this._PanBorder;
                if (border != null)
                {
                    border.MouseLeftButtonDown -= mouseButtonEventHandler;
                }
                this._PanBorder = value;
                border = this._PanBorder;
                if (border != null)
                {
                    border.MouseLeftButtonDown += mouseButtonEventHandler;
                }
            }
        }



        internal virtual TextBlock LabTitle_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._LabTitle;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this.Drag);
                TextBlock textBlock = this._LabTitle;
                if (textBlock != null)
                {
                    textBlock.MouseLeftButtonDown -= mouseButtonEventHandler;
                }
                this._LabTitle = value;
                textBlock = this._LabTitle;
                if (textBlock != null)
                {
                    textBlock.MouseLeftButtonDown += mouseButtonEventHandler;
                }
            }
        }





        public virtual OurButton Btn1_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._Btn1;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R57_1);
                OurButton ourButton = this._Btn1;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._Btn1 = value;
                ourButton = this._Btn1;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }


        public virtual OurButton Btn3_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._Btn3;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R65_2);
                OurButton ourButton = this._Btn3;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._Btn3 = value;
                ourButton = this._Btn3;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___7_0(object i)
        {
            TranslateTransform transformPos;
            (transformPos = this.TransformPos).Y = Conversions.ToDouble(Operators.AddObject(transformPos.Y, i));
        }

        [CompilerGenerated]
        private void _Lambda___7_1(object i)
        {
            RotateTransform transformRotate;
            (transformRotate = this.TransformRotate).Angle = Conversions.ToDouble(Operators.AddObject(transformRotate.Angle, i));
        }

        [CompilerGenerated]
        private void _Lambda___8_1(object i)
        {
            TranslateTransform transformPos;
            (transformPos = this.TransformPos).Y = Conversions.ToDouble(Operators.AddObject(transformPos.Y, i));
        }

        [CompilerGenerated]
        private void _Lambda___8_2(object i)
        {
            RotateTransform transformRotate;
            (transformRotate = this.TransformRotate).Angle = Conversions.ToDouble(Operators.AddObject(transformRotate.Angle, i));
        }

        [CompilerGenerated]
        private void _Lambda___8_3()
        {
            ((Grid)base.Parent).Children.Remove(this);
        }

        [CompilerGenerated]
        private void _Lambda___R57_1(object a0, MouseButtonEventArgs a1)
        {
            this.Btn1_Click();
        }

        [CompilerGenerated]
        private void _Lambda___R65_2(object a0, MouseButtonEventArgs a1)
        {
            this.Btn3_Click();
        }

        private JObject Data;

        private string UserCode;

        private string DeviceCode;

        private string Website;

        private readonly ModMain.OurMsgBoxConverter OurConverter;

        private readonly int Uuid;

        [CompilerGenerated]
        [AccessedThroughProperty("TransformRotate")]
        private RotateTransform _TransformRotate;

        [CompilerGenerated]
        [AccessedThroughProperty("TransformPos")]
        private TranslateTransform _TransformPos;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBorder")]
        private Border _PanBorder;

        [CompilerGenerated]
        [AccessedThroughProperty("EffectShadow")]
        private DropShadowEffect _EffectShadow;

        [CompilerGenerated]
        [AccessedThroughProperty("PanMain")]
        private Grid _PanMain;

        [CompilerGenerated]
        [AccessedThroughProperty("LabTitle")]
        private TextBlock _LabTitle;

        [CompilerGenerated]
        [AccessedThroughProperty("ShapeLine")]
        private Rectangle _ShapeLine;

        [CompilerGenerated]
        [AccessedThroughProperty("PanCaption")]
        private OurScrollViewer _PanCaption;

        [CompilerGenerated]
        [AccessedThroughProperty("LabCaption")]
        private TextBlock _LabCaption;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBtn")]
        private StackPanel _PanBtn;

        [CompilerGenerated]
        [AccessedThroughProperty("Btn1")]
        private OurButton _Btn1;

        [CompilerGenerated]
        [AccessedThroughProperty("Btn2")]
        private OurButton _Btn2;

        [CompilerGenerated]
        [AccessedThroughProperty("Btn3")]
        private OurButton _Btn3;

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            internal void _Lambda___8_0()
            {
                if (!ModMain.WaitingOurMsgBox.Any<ModMain.OurMsgBoxConverter>())
                {
                    ModAnimation.AniStart(ModAnimation.AaColor(ModMain.FrmMain.PanMsg, Panel.BackgroundProperty, new ModBase.OurColor(0.0, 0.0, 0.0, 0.0) - ModMain.FrmMain.PanMsg.Background, 200, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false), "", false);
                }
            }

            public static readonly OurMsgLogin._Closure___ _I = new OurMsgLogin._Closure___();

            public static ThreadStart _I8_0;
        }
    }
}
