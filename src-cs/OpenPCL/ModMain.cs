using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;
using PCL.Our;
using PCLCS;

namespace PCL
{
    [StandardModule]
    public static class ModMain
    {
        // Note: this type is marked as 'beforefieldinit'.
        static ModMain()
        {
        }

        public static void RaiseCustomEvent(this DependencyObject Control)
        {
            ModMain._Closure___1_0 CS_locals1 = new ModMain._Closure___1_0(null);
            CS_locals1._VB_Local_Events = CustomEventService.GetEvents(Control).ToList<CustomEvent>();
            CustomEvent.EventType eventType = CustomEventService.GetEventType(Control);
            if (eventType != CustomEvent.EventType.None)
            {
                CS_locals1._VB_Local_Events.Add(new CustomEvent(eventType, CustomEventService.GetEventData(Control)));
            }
            if (CS_locals1._VB_Local_Events.Any<CustomEvent>())
            {
                ModBase.RunInNewThread(new Action(CS_locals1._Lambda___0), "执行自定义事件 " + Conversions.ToString(ModBase.GetUuid()), ThreadPriority.Normal);
            }
        }

        public static void Hint(string Text, ModMain.HintType Type = ModMain.HintType.Blue, bool Log = true)
        {
            ModMain.HintWaiting.Enqueue(new ModMain.HintMessage
            {
                Text = (Text ?? ""),
                Type = Type,
                Log = Log
            });
        }

        private static void HintTick()
        {
            try
            {
                ModMain.HintMessage hintMessage = default(ModMain.HintMessage);
                while (ModMain.HintWaiting.TryDequeue(out hintMessage))
                {
                    ModMain._Closure___6_0 CS_locals1 = new ModMain._Closure___6_0(null);
                    hintMessage.Text = hintMessage.Text.ReplaceLineEndings(" ", true);
                    if (ModMain.FrmMain.PanHint.Children.Count < 20)
                    {
                        CS_locals1._VB_Local_DoubleStack = null;
                        try
                        {
                            foreach (object obj in ModMain.FrmMain.PanHint.Children)
                            {
                                Border border = (Border)obj;
                                if (Conversions.ToBoolean(Conversions.ToBoolean(NewLateBinding.LateIndexGet(border.Tag, new object[] { 0 }, null)) && Operators.CompareString(((TextBlock)border.Child).Text, hintMessage.Text, false) == 0))
                                {
                                    CS_locals1._VB_Local_DoubleStack = border;
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator enumerator = default;
                            if (enumerator is IDisposable)
                            {
                                (enumerator as IDisposable).Dispose();
                            }
                        }
                        CS_locals1._VB_Local_Percent = 0.3;
                        ModMain.HintType type = hintMessage.Type;
                        if (type != ModMain.HintType.Blue)
                        {
                            if (type != ModMain.HintType.Green)
                            {
                                CS_locals1._VB_Local_TargetColor0 = new ModBase.OurColor(215.0, 255.0, 53.0, 11.0);
                                CS_locals1._VB_Local_TargetColor1 = new ModBase.OurColor(215.0, 255.0, 43.0, 0.0);
                            }
                            else
                            {
                                CS_locals1._VB_Local_TargetColor0 = new ModBase.OurColor(215.0, 33.0, 177.0, 33.0);
                                CS_locals1._VB_Local_TargetColor1 = new ModBase.OurColor(215.0, 29.0, 160.0, 29.0);
                            }
                        }
                        else
                        {
                            CS_locals1._VB_Local_TargetColor0 = new ModBase.OurColor(215.0, 37.0, 155.0, 252.0);
                            CS_locals1._VB_Local_TargetColor1 = new ModBase.OurColor(215.0, 10.0, 142.0, 252.0);
                        }
                        if (!Information.IsNothing(CS_locals1._VB_Local_DoubleStack))
                        {
                            if (!ModAnimation.AniIsRun(Conversions.ToString(Operators.ConcatenateObject("Hint Show ", NewLateBinding.LateIndexGet(CS_locals1._VB_Local_DoubleStack.Tag, new object[] { 1 }, null)))))
                            {
                                ModAnimation.AniStop(Conversions.ToString(Operators.ConcatenateObject("Hint Hide ", NewLateBinding.LateIndexGet(CS_locals1._VB_Local_DoubleStack.Tag, new object[] { 1 }, null))));
                                double num = (double)(checked(800 + hintMessage.Text.Length.Clamp(5, 23) * 180)) * ModAnimation.AniSpeed;
                                checked
                                {
                                    ModAnimation.AniStart(new ModAnimation.AniData[]
                                    {
                                        ModAnimation.AaX(CS_locals1._VB_Local_DoubleStack, unchecked(-12.0 - CS_locals1._VB_Local_DoubleStack.Margin.Left), 50, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                                        ModAnimation.AaX(CS_locals1._VB_Local_DoubleStack, -8.0, 50, 50, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false),
                                        ModAnimation.AaX(CS_locals1._VB_Local_DoubleStack, 8.0, 50, 100, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                                        ModAnimation.AaX(CS_locals1._VB_Local_DoubleStack, -8.0, 50, 150, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false),
                                        ModAnimation.AaDouble(new ParameterizedThreadStart(CS_locals1._Lambda___0), 0.7, 250, 0, null, false),
                                        ModAnimation.AaX(CS_locals1._VB_Local_DoubleStack, -50.0, 200, (int)Math.Round(num), new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false),
                                        ModAnimation.AaOpacity(CS_locals1._VB_Local_DoubleStack, -1.0, 150, (int)Math.Round(num), null, false),
                                        ModAnimation.AaCode(new ThreadStart(CS_locals1._Lambda___1), (int)Math.Round(num), false),
                                        ModAnimation.AaHeight(CS_locals1._VB_Local_DoubleStack, -26.0, 100, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), true),
                                        ModAnimation.AaCode(new ThreadStart(CS_locals1._Lambda___2), 0, true)
                                    }, Conversions.ToString(Operators.ConcatenateObject("Hint Hide ", NewLateBinding.LateIndexGet(CS_locals1._VB_Local_DoubleStack.Tag, new object[] { 1 }, null))), false);
                                }
                            }
                        }
                        else
                        {
                            ModMain._Closure___6_1 CS_locals2 = new ModMain._Closure___6_1(null);
                            CS_locals2._VB_NonLocal__VB_Closure_2 = CS_locals1;
                            CS_locals2._VB_Local_NewHintControl = new Border
                            {
                                Tag = new object[]
                                {
                                    true,
                                    ModBase.GetUuid()
                                },
                                Margin = new Thickness(-70.0, 0.0, 20.0, 0.0),
                                Opacity = 0.0,
                                Height = 0.0,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                CornerRadius = new CornerRadius(0.0, 6.0, 6.0, 0.0)
                            };
                            CS_locals2._VB_Local_NewHintControl.Background = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>
                            {
                                new GradientStop(CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_TargetColor0 * CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_Percent + new ModBase.OurColor(255.0, 255.0, 255.0) * (1.0 - CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_Percent), 0.0),
                                new GradientStop(CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_TargetColor1 * CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_Percent + new ModBase.OurColor(255.0, 255.0, 255.0) * (1.0 - CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_Percent), 1.0)
                            }), 90.0);
                            CS_locals2._VB_Local_NewHintControl.Child = new TextBlock
                            {
                                TextTrimming = TextTrimming.CharacterEllipsis,
                                FontSize = 13.0,
                                Text = hintMessage.Text,
                                Foreground = new ModBase.OurColor(255.0, 255.0, 255.0),
                                Margin = new Thickness(33.0, 5.0, 8.0, 5.0)
                            };
                            ModMain.FrmMain.PanHint.Children.Add(CS_locals2._VB_Local_NewHintControl);
                            List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
                            if (ModMain.FrmMain.PanHint.Children.Count > 1)
                            {
                                list.Add(ModAnimation.AaHeight(CS_locals2._VB_Local_NewHintControl, 26.0, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false));
                            }
                            else
                            {
                                CS_locals2._VB_Local_NewHintControl.Height = 26.0;
                            }
                            list.AddRange(new ModAnimation.AniData[]
                            {
                                ModAnimation.AaX(CS_locals2._VB_Local_NewHintControl, 30.0, 400, 0, new ModAnimation.AniEaseOutElastic(ModAnimation.AniEasePower.Weak), false),
                                ModAnimation.AaX(CS_locals2._VB_Local_NewHintControl, 20.0, 200, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                                ModAnimation.AaOpacity(CS_locals2._VB_Local_NewHintControl, 1.0, 100, 0, null, false),
                                ModAnimation.AaDouble(new ParameterizedThreadStart(CS_locals2._Lambda___3), 0.7, 250, 100, null, false)
                            });
                            ModAnimation.AniStart(list, Conversions.ToString(Operators.ConcatenateObject("Hint Show ", NewLateBinding.LateIndexGet(CS_locals2._VB_Local_NewHintControl.Tag, new object[] { 1 }, null))), false);
                            double num2 = (double)(checked(800 + hintMessage.Text.Length.Clamp(5, 23) * 180)) * ModAnimation.AniSpeed;
                            ModAnimation.AniStart(checked(new ModAnimation.AniData[]
                            {
                                ModAnimation.AaX(CS_locals2._VB_Local_NewHintControl, -50.0, 200, (int)Math.Round(num2), new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false),
                                ModAnimation.AaOpacity(CS_locals2._VB_Local_NewHintControl, -1.0, 150, (int)Math.Round(num2), null, false),
                                ModAnimation.AaCode(new ThreadStart(CS_locals2._Lambda___4), (int)Math.Round(num2), false),
                                ModAnimation.AaHeight(CS_locals2._VB_Local_NewHintControl, -26.0, 100, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), true),
                                ModAnimation.AaCode(new ThreadStart(CS_locals2._Lambda___5), 0, true)
                            }), Conversions.ToString(Operators.ConcatenateObject("Hint Hide ", NewLateBinding.LateIndexGet(CS_locals2._VB_Local_NewHintControl.Tag, new object[] { 1 }, null))), false);
                        }
                    }
                    if (hintMessage.Log)
                    {
                        Logger.Info(string.Format("弹出提示：{0}", hintMessage.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Info(ex, "显示弹出提示失败", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
            }
        }

        private static void HideAllHint()
        {
            try
            {
                IEnumerator enumerator = ModMain.FrmMain.PanHint.Children.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ModMain._Closure___7_0 CS_locals1 = new ModMain._Closure___7_0(null);
                    CS_locals1._VB_Local_Control = (Border)enumerator.Current;
                    CS_locals1._VB_Local_Control.IsHitTestVisible = false;
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaX(CS_locals1._VB_Local_Control, -50.0, 200, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false),
                        ModAnimation.AaOpacity(CS_locals1._VB_Local_Control, -1.0, 150, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false),
                        ModAnimation.AaCode(new ThreadStart(CS_locals1._Lambda___0), 0, false),
                        ModAnimation.AaHeight(CS_locals1._VB_Local_Control, -26.0, 100, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), true),
                        ModAnimation.AaCode(new ThreadStart(CS_locals1._Lambda___1), 0, true)
                    }, Conversions.ToString(Operators.ConcatenateObject("Hint Hide ", NewLateBinding.LateIndexGet(CS_locals1._VB_Local_Control.Tag, new object[] { 1 }, null))), false);
                }
            }
            finally
            {
                IEnumerator enumerator = default;
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
        }

        public static int OurMsgBox(string Caption, string Title = "提示", string Button1 = "确定", string Button2 = "", string Button3 = "", bool IsWarn = false, bool HighLight = true, bool ForceWait = false, Action Button1Action = null, Action Button2Action = null, Action Button3Action = null)
        {
            ModMain.OurMsgBoxConverter ourMsgBoxConverter = new ModMain.OurMsgBoxConverter
            {
                Type = ModMain.OurMsgBoxType.Text,
                Button1 = Button1,
                Button2 = Button2,
                Button3 = Button3,
                Text = Caption,
                IsWarn = IsWarn,
                Title = Title,
                HighLight = HighLight,
                ForceWait = true,
                Button1Action = Button1Action,
                Button2Action = Button2Action,
                Button3Action = Button3Action
            };
            checked
            {
                int num;
                if (Button2.Length > 0 || ForceWait)
                {
                    if (ModMain.FrmMain == null || (ModMain.FrmMain.PanMsg == null && ModBase.RunInUi()))
                    {
                        if (Button2.Length > 0)
                        {
                            MsgBoxResult msgBoxResult = Interaction.MsgBox(Caption, ((Button3.Length > 0) ? MsgBoxStyle.YesNoCancel : MsgBoxStyle.YesNo) + (IsWarn ? 16 : 32), Title);
                            if (msgBoxResult != MsgBoxResult.Cancel)
                            {
                                if (msgBoxResult != MsgBoxResult.Yes)
                                {
                                    if (msgBoxResult == MsgBoxResult.No)
                                    {
                                        ourMsgBoxConverter.Result = 2;
                                    }
                                }
                                else
                                {
                                    ourMsgBoxConverter.Result = 1;
                                }
                            }
                            else
                            {
                                ourMsgBoxConverter.Result = 3;
                            }
                        }
                        else
                        {
                            Interaction.MsgBox(Caption, MsgBoxStyle.OkOnly + (IsWarn ? 16 : 32), Title);
                            ourMsgBoxConverter.Result = 1;
                        }
                        Logger.Warn(string.Format("主窗体加载完成前出现意料外的等待弹窗：{0},{1},{2}", Button1, Button2, Button3), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                    }
                    else
                    {
                        ModMain.WaitingOurMsgBox.Enqueue(ourMsgBoxConverter);
                        try
                        {
                            ModMain.FrmMain.DragStop();
                            if (ModBase.RunInUi())
                            {
                                ModMain.OurMsgBoxTick();
                            }
                            ComponentDispatcher.PushModal();
                            Dispatcher.PushFrame(ourMsgBoxConverter.WaitFrame);
                        }
                        finally
                        {
                            ComponentDispatcher.PopModal();
                        }
                    }
                    Logger.Info(string.Format("普通弹框返回：{0}", RuntimeHelpers.GetObjectValue(ourMsgBoxConverter.Result ?? "null")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                    num = Conversions.ToInteger(ourMsgBoxConverter.Result);
                }
                else
                {
                    ModMain.WaitingOurMsgBox.Enqueue(ourMsgBoxConverter);
                    num = 1;
                }
                return num;
            }
        }

        public static string OurMsgBoxInput(string Title, string Text = null, string DefaultInput = null, Collection<Validate> ValidateRules = null, string HintText = "", string Button1 = "确定", string Button2 = "取消", bool IsWarn = false)
        {
            ModMain.OurMsgBoxConverter ourMsgBoxConverter = new ModMain.OurMsgBoxConverter
            {
                Text = (Text ?? ""),
                HintText = (HintText ?? ""),
                Type = ModMain.OurMsgBoxType.Input,
                ValidateRules = (ValidateRules ?? new Collection<Validate>()),
                Button1 = Button1,
                Button2 = Button2,
                Content = DefaultInput,
                IsWarn = IsWarn,
                Title = Title
            };
            ModMain.WaitingOurMsgBox.Enqueue(ourMsgBoxConverter);
            try
            {
                if (ModMain.FrmMain != null)
                {
                    ModMain.FrmMain.DragStop();
                }
                if (ModBase.RunInUi())
                {
                    ModMain.OurMsgBoxTick();
                }
                ComponentDispatcher.PushModal();
                Dispatcher.PushFrame(ourMsgBoxConverter.WaitFrame);
            }
            finally
            {
                ComponentDispatcher.PopModal();
            }
            Logger.Info(string.Format("输入弹框返回：{0}", RuntimeHelpers.GetObjectValue(ourMsgBoxConverter.Result ?? "null")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
            return Conversions.ToString(ourMsgBoxConverter.Result);
        }

        public static int? OurMsgBoxSelect(IEnumerable<IOurRadio> Selections, string Title = "提示", string Button1 = "确定", string Button2 = "", bool IsWarn = false)
        {
            ModMain.OurMsgBoxConverter ourMsgBoxConverter = new ModMain.OurMsgBoxConverter
            {
                Type = ModMain.OurMsgBoxType.Select,
                Button1 = Button1,
                Button2 = Button2,
                Content = Selections,
                IsWarn = IsWarn,
                Title = Title
            };
            ModMain.WaitingOurMsgBox.Enqueue(ourMsgBoxConverter);
            try
            {
                if (ModMain.FrmMain != null)
                {
                    ModMain.FrmMain.DragStop();
                }
                if (ModBase.RunInUi())
                {
                    ModMain.OurMsgBoxTick();
                }
                ComponentDispatcher.PushModal();
                Dispatcher.PushFrame(ourMsgBoxConverter.WaitFrame);
            }
            finally
            {
                ComponentDispatcher.PopModal();
            }
            Logger.Info(string.Format("选择弹框返回：{0}", RuntimeHelpers.GetObjectValue(ourMsgBoxConverter.Result ?? "null")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
            return (int?)ourMsgBoxConverter.Result;
        }

        public static void OurMsgBoxTick()
        {
            try
            {
                FormMain frmMain = ModMain.FrmMain;
                if (((frmMain != null) ? frmMain.PanMsg : null) != null && ModMain.FrmMain.WindowState != WindowState.Minimized)
                {
                    ModMain.OurMsgBoxConverter ourMsgBoxConverter = null;
                    if (ModMain.FrmMain.PanMsg.Children.Count > 0)
                    {
                        ModMain.FrmMain.PanMsg.Visibility = Visibility.Visible;
                    }
                    else if (ModMain.WaitingOurMsgBox.TryDequeue(out ourMsgBoxConverter))
                    {
                        ModMain.FrmMain.PanMsg.Visibility = Visibility.Visible;
                        switch (ourMsgBoxConverter.Type)
                        {
                        case ModMain.OurMsgBoxType.Text:
                            ModMain.FrmMain.PanMsg.Children.Add(new OurMsgText(ourMsgBoxConverter));
                            break;
                        case ModMain.OurMsgBoxType.Select:
                            ModMain.FrmMain.PanMsg.Children.Add(new OurMsgSelect(ourMsgBoxConverter));
                            break;
                        case ModMain.OurMsgBoxType.Input:
                            ModMain.FrmMain.PanMsg.Children.Add(new OurMsgInput(ourMsgBoxConverter));
                            break;
                        case ModMain.OurMsgBoxType.Login:
                            ModMain.FrmMain.PanMsg.Children.Add(new OurMsgLogin(ourMsgBoxConverter));
                            break;
                        }
                    }
                    else if (ModMain.FrmMain.PanMsg.Visibility != Visibility.Collapsed)
                    {
                        ModMain.FrmMain.PanMsg.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "处理等待中的弹窗失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
            }
        }

        private static void HelpLoad(ModLoader.LoaderTask<int, List<ModMain.HelpEntry>> Loader)
        {
            object helpLoadLock = ModMain.HelpLoadLock;
            ObjectFlowControl.CheckForSyncLockOnValueType(helpLoadLock);
            checked
            {
                lock (helpLoadLock)
                {
                    try
                    {
                        ModMain.HelpTryExtract();
                        List<string> list = new List<string>();
                        try
                        {
                            List<string> list2 = new List<string>();
                            if (DirectoryUtils.Exists(Paths.Base + "PCL\\Help\\"))
                            {
                                try
                                {
                                    foreach (string text in DirectoryUtils.EnumerateFiles(Paths.Base + "PCL\\Help\\", true, "*"))
                                    {
                                        string extension = PathUtils.GetExtension(text);
                                        if (Operators.CompareString(extension, "helpignore", false) != 0)
                                        {
                                            if (Operators.CompareString(extension, "json", false) == 0)
                                            {
                                                list.Add(text);
                                            }
                                        }
                                        else
                                        {
                                            Logger.Info(string.Format("发现 .helpignore 文件：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                                            string[] array = FileUtils.ReadAsLines(text, true, null, null);
                                            for (int i = 0; i < array.Length; i++)
                                            {
                                                string text2 = array[i].BeforeFirst("#", false).Trim();
                                                if (!string.IsNullOrWhiteSpace(text2))
                                                {
                                                    list2.Add(text2);
                                                    Logger.Trace(string.Format("> {0}", text2), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                                                }
                                            }
                                        }
                                    }
                                }
                                finally
                                {
                                    IEnumerator<string> enumerator = default;
                                    if (enumerator != null)
                                    {
                                        enumerator.Dispose();
                                    }
                                }
                            }
                            Logger.Info(string.Format("已扫描 PCL 文件夹下的帮助文件，目前总计 {0} 条", list.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                            try
                            {
                                IEnumerator<string> enumerator2 = DirectoryUtils.EnumerateFiles(ModBase.PathTemp + "Help", true, "*").GetEnumerator();
                                IL_0241:
                                while (enumerator2.MoveNext())
                                {
                                    string text3 = enumerator2.Current;
                                    if (!PathUtils.RemoveLastPart(text3).Substring((ModBase.PathTemp + "Help").Length).Contains("\\.") && Operators.CompareString(PathUtils.GetExtension(text3), "json", false) == 0)
                                    {
                                        string text4 = text3.Substring((ModBase.PathTemp + "Help\\").Length);
                                        try
                                        {
                                            foreach (string text5 in list2)
                                            {
                                                if (text4.RegexCheck(text5, RegexOptions.None))
                                                {
                                                    Logger.Trace(string.Format("已忽略 {0}：{1}", text4, text5), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                                                    goto IL_0241;
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            List<string>.Enumerator enumerator3 = default;
                                            ((IDisposable)enumerator3).Dispose();
                                        }
                                        list.Add(text3);
                                    }
                                }
                            }
                            finally
                            {
                                IEnumerator<string> enumerator2 = default;
                                if (enumerator2 != null)
                                {
                                    enumerator2.Dispose();
                                }
                            }
                            Logger.Info(string.Format("已扫描缓存文件夹下的帮助文件，目前总计 {0} 条", list.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "检查帮助文件夹失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                        }
                        if (!Loader.IsCanceled)
                        {
                            List<ModMain.HelpEntry> list3 = new List<ModMain.HelpEntry>();
                            try
                            {
                                foreach (string text6 in list)
                                {
                                    try
                                    {
                                        ModMain.HelpEntry helpEntry = new ModMain.HelpEntry(text6);
                                        list3.Add(helpEntry);
                                        Logger.Trace(string.Format("已加载的帮助条目：{0} ← {1}", helpEntry.Title, text6), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                                    }
                                    catch (Exception ex2)
                                    {
                                        Logger.Error(ex2, string.Format("初始化帮助条目失败（{0}）", text6), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                                    }
                                }
                            }
                            finally
                            {
                                List<string>.Enumerator enumerator4 = default;
                                ((IDisposable)enumerator4).Dispose();
                            }
                            if (!list3.Any<ModMain.HelpEntry>())
                            {
                                throw new Exception("未找到可用的帮助；若不需要帮助页面，可以在 设置 → 个性化 → 功能隐藏 中将其隐藏");
                            }
                            if (!Loader.IsCanceled)
                            {
                                Loader.Output = list3;
                            }
                        }
                    }
                    catch (Exception ex3)
                    {
                        Logger.Warn(ex3, "帮助列表初始化失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                        throw;
                    }
                }
            }
        }

        public static void HelpTryExtract()
        {
            if (Settings.Get<int>("SystemHelpVersion", null) != 414 || !FileUtils.Exists(ModBase.PathTemp + "Help\\启动器\\备份设置.xaml"))
            {
                DirectoryUtils.Delete(ModBase.PathTemp + "Help", false);
                ModBase.ExtractResources(ModBase.PathTemp + "Cache\\Help.zip", "Help");
                FileUtils.ExtractToDirectory(ModBase.PathTemp + "Cache\\Help.zip", ModBase.PathTemp + "Help", default(CancellationToken), null);
                Settings.Set("SystemHelpVersion", 414, null);
                Logger.Info(string.Format("已解压内置帮助文件，目前状态：{0}", FileUtils.Exists(ModBase.PathTemp + "Help\\启动器\\备份设置.xaml")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
            }
        }

        public static bool IsAprilEnabled
        {
            get
            {
                if (ModMain._IsAprilEnabled == null)
                {
                    ModMain._IsAprilEnabled = new bool?(DateTime.Now.Month == 4 && DateTime.Now.Day == 1 && Settings.Get<int>("AprilYear", null) != DateTime.Now.Year);
                }
                return ModMain._IsAprilEnabled.Value;
            }
        }

        private static void TimerFool()
        {
            try
            {
                if (ModMain.FrmLaunchLeft != null && ModMain.FrmLaunchLeft.AprilPosTrans != null && ModMain.FrmMain.lastMouseArg != null)
                {
                    if (!ModMain.IsAprilGiveup && !(ModMain.FrmMain.PageCurrent != FormMain.PageType.Launch) && ModAnimation.AniControlEnabled == 0 && ModMain.FrmLaunchLeft.BtnLaunch.IsLoaded)
                    {
                        Point position = ModMain.FrmMain.lastMouseArg.GetPosition(ModMain.FrmMain);
                        if (ModMain._STATIC_TimerFool_001_IdieCount_Init == null)
                        {
                            Interlocked.CompareExchange<StaticLocalInitFlag>(ref ModMain._STATIC_TimerFool_001_IdieCount_Init, new StaticLocalInitFlag(), null);
                        }
                        bool flag = false;
                        try
                        {
                            Monitor.Enter(ModMain._STATIC_TimerFool_001_IdieCount_Init, ref flag);
                            if (ModMain._STATIC_TimerFool_001_IdieCount_Init.State == 0)
                            {
                                ModMain._STATIC_TimerFool_001_IdieCount_Init.State = 2;
                                ModMain._STATIC_TimerFool_001_IdieCount = 0;
                            }
                            else if (ModMain._STATIC_TimerFool_001_IdieCount_Init.State == 2)
                            {
                                throw new IncompleteInitialization();
                            }
                        }
                        finally
                        {
                            ModMain._STATIC_TimerFool_001_IdieCount_Init.State = 1;
                            if (flag)
                            {
                                Monitor.Exit(ModMain._STATIC_TimerFool_001_IdieCount_Init);
                            }
                        }
                        if (ModMain._STATIC_TimerFool_001_MousePosLast_Init == null)
                        {
                            Interlocked.CompareExchange<StaticLocalInitFlag>(ref ModMain._STATIC_TimerFool_001_MousePosLast_Init, new StaticLocalInitFlag(), null);
                        }
                        bool flag2 = false;
                        try
                        {
                            Monitor.Enter(ModMain._STATIC_TimerFool_001_MousePosLast_Init, ref flag2);
                            if (ModMain._STATIC_TimerFool_001_MousePosLast_Init.State == 0)
                            {
                                ModMain._STATIC_TimerFool_001_MousePosLast_Init.State = 2;
                                ModMain._STATIC_TimerFool_001_MousePosLast = new Point(0.0, 0.0);
                            }
                            else if (ModMain._STATIC_TimerFool_001_MousePosLast_Init.State == 2)
                            {
                                throw new IncompleteInitialization();
                            }
                        }
                        finally
                        {
                            ModMain._STATIC_TimerFool_001_MousePosLast_Init.State = 1;
                            if (flag2)
                            {
                                Monitor.Exit(ModMain._STATIC_TimerFool_001_MousePosLast_Init);
                            }
                        }
                        double num;
                        double num2;
                        Vector vector;
                        Vector vector3;
                        checked
                        {
                            if (position == ModMain._STATIC_TimerFool_001_MousePosLast)
                            {
                                ModMain._STATIC_TimerFool_001_IdieCount++;
                            }
                            else
                            {
                                ModMain._STATIC_TimerFool_001_MousePosLast = position;
                                ModMain._STATIC_TimerFool_001_IdieCount = 0;
                            }
                            num = ModMain.FrmLaunchLeft.BtnLaunch.ActualWidth / 2.0;
                            num2 = ModMain.FrmLaunchLeft.BtnLaunch.ActualHeight / 2.0;
                            vector = (Vector)(ModMain.FrmMain.lastMouseArg.GetPosition(ModMain.FrmLaunchLeft.BtnLaunch) - new Vector(num, num2));
                            Vector vector2 = new Vector(vector.X, vector.Y);
                            vector2.Normalize();
                            vector3 = -vector2;
                        }
                        double length = new Vector(Math.Max(0.0, Math.Abs(vector.X) - num), Math.Max(0.0, Math.Abs(vector.Y) - num2)).Length;
                        Math.Sin((double)ModMain.Timer150Count / 37.5 * 3.141592653589793);
                        double num3 = 1.0 / (1.0 + Math.Pow((double)ModMain.AprilDifficultiedDistance / 6000.0, 2.0)) + 0.25;
                        Vector vector4 = Math.Max(0.0, -0.65 - Math.Log((length / num3 + 0.4) / 200.0)) * vector3 * num3;
                        if (ModMain._STATIC_TimerFool_001_IdieCount >= 320)
                        {
                            Vector vector5 = (Vector)(ModMain.FrmMain.lastMouseArg.GetPosition(ModMain.FrmMain.PanMain) - new Vector(num, ModMain.FrmMain.PanMain.ActualHeight - num2 * 3.0));
                            Vector vector6 = new Vector(ModMain.FrmLaunchLeft.AprilPosTrans.X, ModMain.FrmLaunchLeft.AprilPosTrans.Y);
                            if (vector5.Length > 250.0 && vector6.Length > 0.4)
                            {
                                vector4 -= vector6 * 0.0005;
                                vector6.Normalize();
                                vector4 -= vector6 * 0.15;
                            }
                        }
                        if (ModMain._STATIC_TimerFool_001_Speed_Init == null)
                        {
                            Interlocked.CompareExchange<StaticLocalInitFlag>(ref ModMain._STATIC_TimerFool_001_Speed_Init, new StaticLocalInitFlag(), null);
                        }
                        bool flag3 = false;
                        try
                        {
                            Monitor.Enter(ModMain._STATIC_TimerFool_001_Speed_Init, ref flag3);
                            if (ModMain._STATIC_TimerFool_001_Speed_Init.State == 0)
                            {
                                ModMain._STATIC_TimerFool_001_Speed_Init.State = 2;
                                ModMain._STATIC_TimerFool_001_Speed = new Vector(0.0, 0.0);
                            }
                            else if (ModMain._STATIC_TimerFool_001_Speed_Init.State == 2)
                            {
                                throw new IncompleteInitialization();
                            }
                        }
                        finally
                        {
                            ModMain._STATIC_TimerFool_001_Speed_Init.State = 1;
                            if (flag3)
                            {
                                Monitor.Exit(ModMain._STATIC_TimerFool_001_Speed_Init);
                            }
                        }
                        Point point = ModMain.FrmLaunchLeft.BtnLaunch.TranslatePoint(new Point(0.0, 0.0), ModMain.FrmMain.PanForm);
                        if (point.X < -num * 2.0)
                        {
                            TranslateTransform translateTransform;
                            (translateTransform = ModMain.FrmLaunchLeft.AprilPosTrans).X = translateTransform.X + (ModMain.FrmMain.PanForm.ActualWidth + num * 2.0);
                            ModMain._STATIC_TimerFool_001_Speed.X = ModMain._STATIC_TimerFool_001_Speed.X - 80.0;
                            if (point.Y < 0.0)
                            {
                                (translateTransform = ModMain.FrmLaunchLeft.AprilPosTrans).Y = translateTransform.Y + num2 * 2.5;
                            }
                            else if (point.Y > ModMain.FrmMain.PanForm.ActualHeight - num2 * 2.0)
                            {
                                (translateTransform = ModMain.FrmLaunchLeft.AprilPosTrans).Y = translateTransform.Y - num2 * 2.5;
                            }
                        }
                        else if (point.X > ModMain.FrmMain.PanForm.ActualWidth)
                        {
                            TranslateTransform translateTransform;
                            (translateTransform = ModMain.FrmLaunchLeft.AprilPosTrans).X = translateTransform.X - (ModMain.FrmMain.PanForm.ActualWidth + num * 2.0);
                            ModMain._STATIC_TimerFool_001_Speed.X = ModMain._STATIC_TimerFool_001_Speed.X + 80.0;
                            if (point.Y < 0.0)
                            {
                                (translateTransform = ModMain.FrmLaunchLeft.AprilPosTrans).Y = translateTransform.Y + num2 * 2.5;
                            }
                            else if (point.Y > ModMain.FrmMain.PanForm.ActualHeight - num2 * 2.0)
                            {
                                (translateTransform = ModMain.FrmLaunchLeft.AprilPosTrans).Y = translateTransform.Y - num2 * 2.5;
                            }
                        }
                        else if (point.Y < -num2 * 2.0)
                        {
                            TranslateTransform translateTransform;
                            (translateTransform = ModMain.FrmLaunchLeft.AprilPosTrans).Y = translateTransform.Y + (ModMain.FrmMain.PanForm.ActualHeight + num2 * 2.0);
                            ModMain._STATIC_TimerFool_001_Speed.Y = ModMain._STATIC_TimerFool_001_Speed.Y - 25.0;
                            if (point.X < 0.0)
                            {
                                (translateTransform = ModMain.FrmLaunchLeft.AprilPosTrans).X = translateTransform.X + num * 2.0;
                            }
                            else if (point.X > ModMain.FrmMain.PanForm.ActualWidth - num * 2.0)
                            {
                                (translateTransform = ModMain.FrmLaunchLeft.AprilPosTrans).X = translateTransform.X - num * 2.0;
                            }
                        }
                        else if (point.Y > ModMain.FrmMain.PanForm.ActualHeight)
                        {
                            TranslateTransform translateTransform;
                            (translateTransform = ModMain.FrmLaunchLeft.AprilPosTrans).Y = translateTransform.Y - (ModMain.FrmMain.PanForm.ActualHeight + num2 * 2.0);
                            ModMain._STATIC_TimerFool_001_Speed.Y = ModMain._STATIC_TimerFool_001_Speed.Y + 25.0;
                            if (point.X < 0.0)
                            {
                                (translateTransform = ModMain.FrmLaunchLeft.AprilPosTrans).X = translateTransform.X + num * 2.0;
                            }
                            else if (point.X > ModMain.FrmMain.PanForm.ActualWidth - num * 2.0)
                            {
                                (translateTransform = ModMain.FrmLaunchLeft.AprilPosTrans).X = translateTransform.X - num * 2.0;
                            }
                        }
                        ModMain._STATIC_TimerFool_001_Speed = ModMain._STATIC_TimerFool_001_Speed * 0.8 + vector4;
                        double num4 = Math.Min(60.0, ModMain._STATIC_TimerFool_001_Speed.Length);
                        if (num4 >= 0.01)
                        {
                            ModMain._STATIC_TimerFool_001_Speed.Normalize();
                            ModMain._STATIC_TimerFool_001_Speed *= num4;
                            checked
                            {
                                ModMain.AprilDifficultiedDistance = (int)Math.Round(unchecked((double)ModMain.AprilDifficultiedDistance + num4));
                            }
                            TranslateTransform translateTransform;
                            (translateTransform = ModMain.FrmLaunchLeft.AprilPosTrans).X = translateTransform.X + ModMain._STATIC_TimerFool_001_Speed.X;
                            (translateTransform = ModMain.FrmLaunchLeft.AprilPosTrans).Y = translateTransform.Y + ModMain._STATIC_TimerFool_001_Speed.Y;
                            ModMain.FrmLaunchLeft.AprilScaleTrans.ScaleX = (1.0 - (Math.Abs(vector3.X) - Math.Abs(vector3.Y)) * (num4 / 160.0)).Clamp(0.2, 1.8);
                            ModMain.FrmLaunchLeft.AprilScaleTrans.ScaleY = (1.0 - (Math.Abs(vector3.Y) - Math.Abs(vector3.X)) * (num4 / 100.0)).Clamp(0.2, 1.8);
                            if (ModMain._STATIC_TimerFool_001_GiveUpDistance_Init == null)
                            {
                                Interlocked.CompareExchange<StaticLocalInitFlag>(ref ModMain._STATIC_TimerFool_001_GiveUpDistance_Init, new StaticLocalInitFlag(), null);
                            }
                            bool flag4 = false;
                            try
                            {
                                Monitor.Enter(ModMain._STATIC_TimerFool_001_GiveUpDistance_Init, ref flag4);
                                if (ModMain._STATIC_TimerFool_001_GiveUpDistance_Init.State == 0)
                                {
                                    ModMain._STATIC_TimerFool_001_GiveUpDistance_Init.State = 2;
                                    ModMain._STATIC_TimerFool_001_GiveUpDistance = -1500.0;
                                }
                                else if (ModMain._STATIC_TimerFool_001_GiveUpDistance_Init.State == 2)
                                {
                                    throw new IncompleteInitialization();
                                }
                            }
                            finally
                            {
                                ModMain._STATIC_TimerFool_001_GiveUpDistance_Init.State = 1;
                                if (flag4)
                                {
                                    Monitor.Exit(ModMain._STATIC_TimerFool_001_GiveUpDistance_Init);
                                }
                            }
                            ModMain._STATIC_TimerFool_001_GiveUpDistance += num4;
                            if (ModMain._STATIC_TimerFool_001_GiveUpDistance > 2500.0)
                            {
                                ModMain._STATIC_TimerFool_001_GiveUpDistance = 0.0;
                                switch (ModBase.RandomInteger(0, 3))
                                {
                                case 0:
                                    ModMain.Hint("放弃吧！只需要点一下右下角的小白旗……", ModMain.HintType.Red, true);
                                    break;
                                case 1:
                                    ModMain.Hint("看到右下角的那面小白旗了吗？", ModMain.HintType.Red, true);
                                    break;
                                case 2:
                                    ModMain.Hint("这里建议点一下右下角的小白旗投降呢.jpg", ModMain.HintType.Red, true);
                                    break;
                                case 3:
                                    ModMain.Hint("右下角的小白旗永远等着你……", ModMain.HintType.Red, true);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "愚人节移动出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
            }
        }

        public static void ShowWindowToTop(IntPtr Handle)
        {
            try
            {
                ModMain.PostMessage(Handle, 6402U, 0L, 0L);
                ModMain.SetForegroundWindow(Handle);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "设置窗口置顶失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
            }
        }

        [DllImport("user32", CharSet = CharSet.Ansi, EntryPoint = "FindWindowA", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr FindWindow([MarshalAs(UnmanagedType.VBByRefStr)] ref string ClassName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string WindowName);

        [DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern int SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32", CharSet = CharSet.Ansi, EntryPoint = "PostMessageA", ExactSpelling = true, SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, long wParam, long lParam);

        public static void SetGPUPreference(string Executeable)
        {
            object item = RegistryUtils.Read("HKEY_CURRENT_USER\\Software\\Microsoft\\DirectX\\UserGpuPreferences", Executeable).Item1;
            if (Operators.CompareString((item != null) ? item.ToString() : null, "GpuPreference=2;", false) == 0)
            {
                Logger.Info(string.Format("无需调整显卡设置：{0}", Executeable), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                return;
            }
            RegistryUtils.Write("HKEY_CURRENT_USER\\Software\\Microsoft\\DirectX\\UserGpuPreferences", Executeable, "GpuPreference=2;");
            Logger.Info(string.Format("已调整显卡设置：{0}", Executeable), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
        }

        public static string ArgumentReplace(string Text, Func<string, string> EscapeHandler = null, bool ReplaceTime = true)
        {
            ModMain._Closure___71_0 CS_locals1 = new ModMain._Closure___71_0();
            CS_locals1._VB_Local_EscapeHandler = EscapeHandler;
            string text;
            if (Text == null || !Text.Contains("{"))
            {
                text = Text;
            }
            else
            {
                if (ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer_Init == null)
                {
                    Interlocked.CompareExchange<StaticLocalInitFlag>(ref ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer_Init, new StaticLocalInitFlag(), null);
                }
                bool flag = false;
                try
                {
                    Monitor.Enter(ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer_Init, ref flag);
                    if (ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer_Init.State == 0)
                    {
                        ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer_Init.State = 2;
                        ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer = new Func<string, string>(CS_locals1._Lambda___0);
                    }
                    else if (ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer_Init.State == 2)
                    {
                        throw new IncompleteInitialization();
                    }
                }
                finally
                {
                    ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer_Init.State = 1;
                    if (flag)
                    {
                        Monitor.Exit(ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer_Init);
                    }
                }
                Text = Text.Replace("{pcl_version}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer("2.99.0.0"));
                Text = Text.Replace("{pcl_version_code}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(Conversions.ToString(414)));
                Text = Text.Replace("{pcl_version_branch}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer("快照版"));
                Text = Text.Replace("{pcl_build_type}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(ModBase.BuildTypes.Snapshot.ToString()));
                Text = Text.Replace("{pcl_branch}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer("Official"));
                Text = Text.Replace("{identify}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(ModBase.Identify));
                Text = Text.Replace("{path}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(Paths.Base));
                Text = Text.Replace("{path_with_name}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(ModBase.PathExe));
                Text = Text.Replace("{path_temp}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(ModBase.PathTemp));
                Text = Text.Replace("{pcl_md5}", (ModMain._Closure___._I71_1 == null) ? (ModMain._Closure___._I71_1 = new Func<string>(ModMain._Closure___._I._Lambda___71_1)) : ModMain._Closure___._I71_1);
                Text = Text.Replace("{pcl_sha1}", (ModMain._Closure___._I71_2 == null) ? (ModMain._Closure___._I71_2 = new Func<string>(ModMain._Closure___._I._Lambda___71_2)) : ModMain._Closure___._I71_2);
                if (ReplaceTime)
                {
                    Text = Text.Replace("{date}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(DateTime.Now.ToString("yyyy'/'M'/'d")));
                    Text = Text.Replace("{time}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(DateTime.Now.ToString("HH':'mm':'ss")));
                }
                string text2 = Text;
                string text3 = "{java}";
                Func<string, string> func = ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer;
                Java mcLaunchJavaSelected = ModLaunch.McLaunchJavaSelected;
                Text = text2.Replace(text3, func((mcLaunchJavaSelected != null) ? mcLaunchJavaSelected.Folder : null));
                Text = Text.Replace("{minecraft}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(ModMinecraft.McFolderSelected));
                ModMinecraft.McInstance mcInstanceSelected = ModMinecraft.McInstanceSelected;
                if (((object)mcInstanceSelected != (object)null) && mcInstanceSelected.IsLoaded)
                {
                    Text = Text.Replace("{version_path}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(ModMinecraft.McInstanceSelected.PathVersion));
                    Text = Text.Replace("{verpath}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(ModMinecraft.McInstanceSelected.PathVersion));
                    Text = Text.Replace("{version_indie}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(ModMinecraft.McInstanceSelected.PathIndie));
                    Text = Text.Replace("{verindie}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(ModMinecraft.McInstanceSelected.PathIndie));
                    Text = Text.Replace("{name}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(ModMinecraft.McInstanceSelected.Name));
                    if (new string[] { "unknown", "old", "pending" }.Contains(ModMinecraft.McInstanceSelected.Version.VanillaName.Lower()))
                    {
                        Text = Text.Replace("{version}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(ModMinecraft.McInstanceSelected.Name));
                    }
                    else
                    {
                        Text = Text.Replace("{version}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(ModMinecraft.McInstanceSelected.Version.VanillaName));
                    }
                    Text = Text.Replace("{forge}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(ModMinecraft.McInstanceSelected.Version.ModLoaders.HasFlagF(ModLoaders.Forge) ? ModMinecraft.McInstanceSelected.Version.ModLoaderVersions[ModLoaders.Forge] : ""));
                    Text = Text.Replace("{fabric}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(ModMinecraft.McInstanceSelected.Version.ModLoaders.HasFlagF(ModLoaders.Fabric) ? ModMinecraft.McInstanceSelected.Version.ModLoaderVersions[ModLoaders.Fabric] : ""));
                    Text = Text.Replace("{neoforge}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(ModMinecraft.McInstanceSelected.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge) ? ModMinecraft.McInstanceSelected.Version.ModLoaderVersions[ModLoaders.NeoForge] : ""));
                    Text = Text.Replace("{liteloader}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(ModMinecraft.McInstanceSelected.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader) ? "true" : ""));
                }
                else
                {
                    Text = Text.Replace("{version_path}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(null));
                    Text = Text.Replace("{verpath}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(null));
                    Text = Text.Replace("{version_indie}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(null));
                    Text = Text.Replace("{verindie}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(null));
                    Text = Text.Replace("{name}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(null));
                    Text = Text.Replace("{version}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(null));
                    Text = Text.Replace("{forge}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(null));
                    Text = Text.Replace("{fabric}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(null));
                    Text = Text.Replace("{neoforge}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(null));
                    Text = Text.Replace("{liteloader}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(null));
                }
                if (ModLaunch.McLoginLoader.State == ModBase.LoadState.Finished)
                {
                    Text = Text.Replace("{user}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(ModLaunch.McLoginLoader.Output.Name));
                    string text4 = Text;
                    string text5 = "{uuid}";
                    Func<string, string> func2 = ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer;
                    string uuid = ModLaunch.McLoginLoader.Output.Uuid;
                    Text = text4.Replace(text5, func2((uuid != null) ? uuid.Lower() : null));
                    switch (ModLaunch.McLoginLoader.Input.Type)
                    {
                    case ModLaunch.McLoginType.Legacy:
                        Text = Text.Replace("{login}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer("离线"));
                        break;
                    case ModLaunch.McLoginType.Nide:
                        Text = Text.Replace("{login}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer("统一通行证"));
                        break;
                    case ModLaunch.McLoginType.Auth:
                        Text = Text.Replace("{login}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer("Authlib-Injector"));
                        break;
                    case ModLaunch.McLoginType.Ms:
                        Text = Text.Replace("{login}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer("正版"));
                        break;
                    }
                }
                else
                {
                    Text = Text.Replace("{user}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(null));
                    Text = Text.Replace("{uuid}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(null));
                    Text = Text.Replace("{login}", ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(null));
                }
                Text = Text.RegexReplace("\\{hint\\}", (ModMain._Closure___._IR71_1 == null) ? (ModMain._Closure___._IR71_1 = new MatchEvaluator(ModMain._Closure___._I._Lambda___R71_1)) : ModMain._Closure___._IR71_1, RegexOptions.None);
                Text = Text.RegexReplace("\\{cave\\}", (ModMain._Closure___._IR71_2 == null) ? (ModMain._Closure___._IR71_2 = new MatchEvaluator(ModMain._Closure___._I._Lambda___R71_2)) : ModMain._Closure___._IR71_2, RegexOptions.None);
                Text = Text.RegexReplace("\\{setup:([a-zA-Z0-9]+)\\}", (ModMain._Closure___._I71_5 == null) ? (ModMain._Closure___._I71_5 = new MatchEvaluator(ModMain._Closure___._I._Lambda___71_5)) : ModMain._Closure___._I71_5, RegexOptions.None);
                Text = Text.RegexReplace("\\{varible:([^:\\}]+)(?::([^\\}]+))?\\}", (ModMain._Closure___._I71_6 == null) ? (ModMain._Closure___._I71_6 = new MatchEvaluator(ModMain._Closure___._I._Lambda___71_6)) : ModMain._Closure___._I71_6, RegexOptions.None);
                Text = Text.RegexReplace("\\{variable:([^:\\}]+)(?::([^\\}]+))?\\}", (ModMain._Closure___._I71_7 == null) ? (ModMain._Closure___._I71_7 = new MatchEvaluator(ModMain._Closure___._I._Lambda___71_7)) : ModMain._Closure___._I71_7, RegexOptions.None);
                text = Text;
            }
            return text;
        }

        public static void TryClearTaskTemp()
        {
            if (!ModMain.IsTaskTempCleared)
            {
                ModMain.IsTaskTempCleared = true;
                ModMain.IsTaskTempClearing = true;
                try
                {
                    Logger.Info("开始清理任务缓存文件夹", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                    DirectoryUtils.Delete(string.Format("{0}ProgramData\\PCL\\TaskTemp\\", ModBase.OsDrive), false);
                    DirectoryUtils.Delete(string.Format("{0}TaskTemp\\", ModBase.PathTemp), false);
                    Logger.Info("已清理任务缓存文件夹", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                    return;
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "清理任务缓存文件夹失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                    return;
                }
                finally
                {
                    ModMain.IsTaskTempClearing = false;
                }
            }
            if (ModMain.IsTaskTempClearing)
            {
                while (ModMain.IsTaskTempClearing)
                {
                    Thread.Sleep(1);
                }
            }
        }

        public static string RequestTaskTempFolder(bool RequireNonSpace = false)
        {
            ModMain.TryClearTaskTemp();
            string text;
            try
            {
                text = string.Format("{0}TaskTemp\\{1}-{2}\\", ModBase.PathTemp, ModBase.GetUuid(), ModBase.RandomInteger(0, 1000000));
                if (!RequireNonSpace || !text.Contains(" "))
                {
                    DirectoryUtils.Create(text);
                    ModBase.CheckPermissionWithException(text);
                    return text;
                }
            }
            catch (Exception ex)
            {
            }
            text = string.Format("{0}ProgramData\\PCL\\TaskTemp\\{1}-{2}\\", ModBase.OsDrive, ModBase.GetUuid(), ModBase.RandomInteger(0, 1000000));
            DirectoryUtils.Create(text);
            ModBase.CheckPermission(text);
            return text;
        }

        public static void Feedback(bool ShowMsgbox = true, bool ForceOpenLog = false)
        {
            int num = 0;
            int num4;
            object obj;
            int num2 = 0;
            int __onErrorFrom_IL_00C6 = -1;
            ONERR_IL_00C6:
            try
            {
                if (__onErrorFrom_IL_00C6 >= 0)
                {
                    if (__onErrorFrom_IL_00C6 < 2) goto IL_0007;
                    if (__onErrorFrom_IL_00C6 < 3) goto IL_000E;
                    if (__onErrorFrom_IL_00C6 < 4) goto IL_003D;
                    if (__onErrorFrom_IL_00C6 < 5) goto IL_0053;
                    goto IL_00C6;
                }
                IL_0000:
                ProjectData.ClearProjectError();
                num = 1;
                IL_0007:
                num2 = 2;
                ModMain.FeedbackInfo();
                IL_000E:
                num2 = 3;
                if (!ForceOpenLog && (!ShowMsgbox || ModMain.OurMsgBox("若你在汇报一个 Bug，请点击 打开文件夹 按钮，并上传 Log(1~5).txt 中包含错误信息的文件。\r\n游戏崩溃一般与启动器无关，请不要因为游戏崩溃而提交反馈。", "反馈提交提醒", "打开文件夹", "不需要", "", false, true, false, null, null, null) != 1))
                {
                    goto IL_0053;
                }
                IL_003D:
                num2 = 4;
                ModBase.OpenExplorer(Paths.Base + "PCL\\Log1.txt");
                IL_0053:
                num2 = 5;
                ModBase.OpenWebsite("https://github.com/Meloong-Git/PCL/issues/");
                IL_005F:
                goto IL_00C6;
            }
            catch (Exception) when (num != 0)
            {
                __onErrorFrom_IL_00C6 = num2;
                goto ONERR_IL_00C6;
            }
            IL_00C6:;
        }

        public static void FeedbackInfo()
        {
            int num = 0;
            int num4;
            object obj;
            int num2 = 0;
            int __onErrorFrom_IL_018F = -1;
            ONERR_IL_018F:
            try
            {
                if (__onErrorFrom_IL_018F >= 0)
                {
                    if (__onErrorFrom_IL_018F < 2) goto IL_0007;
                    goto IL_018F;
                }
                IL_0000:
                ProjectData.ClearProjectError();
                num = 1;
                IL_0007:
                num2 = 2;
                Logger.Warn(string.Concat(new string[]
                {
                    string.Format("诊断信息：{0}", "\r\n"),
                    "操作系统：",
                    OurWpfExtension.Computer.Info.OSFullName,
                    "\r\n剩余内存：",
                    Conversions.ToString(Conversion.Int(OurWpfExtension.Computer.Info.AvailablePhysicalMemory / 1024.0 / 1024.0)),
                    " M / ",
                    Conversions.ToString(Conversion.Int(OurWpfExtension.Computer.Info.TotalPhysicalMemory / 1024.0 / 1024.0)),
                    " M\r\nDPI：",
                    Conversions.ToString(ModBase.DPI),
                    "（",
                    Conversions.ToString(Math.Round((double)ModBase.DPI / 96.0, 2) * 100.0),
                    "%）\r\nMC 文件夹：",
                    ModMinecraft.McFolderSelected ?? "Nothing",
                    "\r\n文件位置：",
                    Paths.Base
                }), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                IL_0134:
                goto IL_018F;
            }
            catch (Exception) when (num != 0)
            {
                __onErrorFrom_IL_018F = num2;
                goto ONERR_IL_018F;
            }
            IL_018F:;
        }

        public static void Telemetry(string Event, params string[] Datas)
        {
            // 保留调用接口以兼容现有事件，禁用数据上报。
        }

        private static void TimerMain()
        {
            try
            {
                ModMain.HintTick();
                ModMain.OurMsgBoxTick();
                ModMain.FrmMain.DragTick();
                ModLoader.LoaderTaskbarProgressRefresh();
                if (ModSecret.ThemeDontClick == 2)
                {
                    ModSecret.ThemeRefresh(-1);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "短程主时钟执行异常", LogBehavior.AlertThenCrash, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
            }
            checked
            {
                ModMain.Timer4Count++;
                if (ModMain.Timer4Count == 4)
                {
                    ModMain.Timer4Count = 0;
                    try
                    {
                        if (ModSecret.ThemeNow == 12)
                        {
                            ModSecret.ThemeRefresh(-1);
                        }
                    }
                    catch (Exception ex2)
                    {
                        Logger.Warn(ex2, "中程主时钟执行异常", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                    }
                }
                ModMain.Timer150Count++;
                if (ModMain.Timer150Count == 150)
                {
                    ModMain.Timer150Count = 0;
                    try
                    {
                        if (ModMain.FrmMain.BtnExtraApril_ShowCheck() && ModMain.AprilDifficultiedDistance != 0)
                        {
                            ModMain.FrmMain.BtnExtraApril.Ribble();
                        }
                        ModBase.RunInUi((ModMain._Closure___._I82_0 == null) ? (ModMain._Closure___._I82_0 = new Action(ModMain._Closure___._I._Lambda___82_0)) : ModMain._Closure___._I82_0, false);
                    }
                    catch (Exception ex3)
                    {
                        Logger.Error(ex3, "长程主时钟执行异常", LogBehavior.AlertThenCrash, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                    }
                }
            }
        }

        public static void TimerMainStart()
        {
            ModBase.RunInNewThread((ModMain._Closure___._I83_0 == null) ? (ModMain._Closure___._I83_0 = new Action(ModMain._Closure___._I._Lambda___83_0)) : ModMain._Closure___._I83_0, "Timer Main", ThreadPriority.Normal);
            if (ModMain.IsAprilEnabled)
            {
                ModBase.RunInNewThread((ModMain._Closure___._I83_1 == null) ? (ModMain._Closure___._I83_1 = new Action(ModMain._Closure___._I._Lambda___83_1)) : ModMain._Closure___._I83_1, "Timer Main Fool", ThreadPriority.Normal);
            }
        }

        private static ConcurrentQueue<ModMain.HintMessage> HintWaiting = new ConcurrentQueue<ModMain.HintMessage>();

        public static ConcurrentQueue<ModMain.OurMsgBoxConverter> WaitingOurMsgBox = new ConcurrentQueue<ModMain.OurMsgBoxConverter>();

        public static FormMain FrmMain;

        public static SplashScreen FrmStart;

        public static PageLaunchLeft FrmLaunchLeft;

        public static PageLaunchRight FrmLaunchRight;

        public static PageSelectLeft FrmSelectLeft;

        public static PageSelectRight FrmSelectRight;

        public static PageSpeedLeft FrmSpeedLeft;

        public static PageSpeedRight FrmSpeedRight;

        public static PageLinkMain FrmLinkMain;

        public static PageDownloadLeft FrmDownloadLeft;

        public static PageDownloadInstall FrmDownloadInstall;

        public static PageDownloadMod FrmDownloadMod;

        public static PageDownloadPack FrmDownloadPack;

        public static PageDownloadDataPack FrmDownloadDataPack;

        public static PageDownloadShader FrmDownloadShader;

        public static PageDownloadResourcePack FrmDownloadResourcePack;

        public static PageSetupLeft FrmSetupLeft;

        public static PageSetupLaunch FrmSetupLaunch;

        public static PageSetupUI FrmSetupUI;

        public static PageSetupSystem FrmSetupSystem;

        public static PageSetupLink FrmSetupLink;

        public static PageOtherLeft FrmOtherLeft;

        public static PageOtherHelp FrmOtherHelp;

        public static PageOtherAbout FrmOtherAbout;

        public static PageOtherTest FrmOtherTest;

        public static PageLoginLegacy FrmLoginLegacy;

        public static PageLoginNide FrmLoginNide;

        public static PageLoginNideSkin FrmLoginNideSkin;

        public static PageLoginAuth FrmLoginAuth;

        public static PageLoginAuthSkin FrmLoginAuthSkin;

        public static PageLoginMs FrmLoginMs;

        public static PageLoginMsSkin FrmLoginMsSkin;

        public static PageInstanceLeft FrmInstanceLeft;

        public static PageInstanceOverall FrmInstanceOverall;

        public static PageInstanceMod FrmInstanceMod;

        public static PageInstanceModDisabled FrmInstanceModDisabled;

        public static PageInstanceSetup FrmInstanceSetup;

        public static PageInstanceExport FrmInstanceExport;

        public static PageDownloadModify FrmDownloadModify;

        public static PageDownloadResourceDetail FrmDownloadResourceDetail;

        public static ModLoader.LoaderTask<int, List<ModMain.HelpEntry>> HelpLoader = new ModLoader.LoaderTask<int, List<ModMain.HelpEntry>>("Help Page", new Action<ModLoader.LoaderTask<int, List<ModMain.HelpEntry>>>(ModMain.HelpLoad), null, ThreadPriority.BelowNormal);

        private static readonly object HelpLoadLock = RuntimeHelpers.GetObjectValue(new object());

        private static bool? _IsAprilEnabled = null;

        public static bool IsAprilGiveup = false;

        private static int AprilDifficultiedDistance = 0;

        private static bool IsTaskTempCleared = false;

        private static bool IsTaskTempClearing = false;

        public static object DragControl = null;

        private static int Timer4Count = 0;

        private static int Timer150Count = 0;

        private static int _STATIC_TimerFool_001_IdieCount;

        private static StaticLocalInitFlag _STATIC_TimerFool_001_IdieCount_Init;

        private static Point _STATIC_TimerFool_001_MousePosLast;

        private static StaticLocalInitFlag _STATIC_TimerFool_001_MousePosLast_Init;

        private static Vector _STATIC_TimerFool_001_Speed;

        private static StaticLocalInitFlag _STATIC_TimerFool_001_Speed_Init;

        private static double _STATIC_TimerFool_001_GiveUpDistance;

        private static StaticLocalInitFlag _STATIC_TimerFool_001_GiveUpDistance_Init;

        private static Func<string, string> _STATIC_ArgumentReplace_03EE151280F92EE2_Replacer;

        private static StaticLocalInitFlag _STATIC_ArgumentReplace_03EE151280F92EE2_Replacer_Init;

        public enum HintType
        {
            Blue,
            Green,
            Red
        }

        private struct HintMessage
        {
            public string Text;

            public ModMain.HintType Type;

            public bool Log;
        }

        public class OurMsgBoxConverter
        {
            public OurMsgBoxConverter()
            {
                this.HintText = "";
                this.Button1 = "确定";
                this.Button2 = "";
                this.Button3 = "";
                this.Button1Action = null;
                this.Button2Action = null;
                this.Button3Action = null;
                this.IsWarn = false;
                this.ForceWait = false;
                this.WaitFrame = new DispatcherFrame(true);
                this.IsExited = false;
            }

            public ModMain.OurMsgBoxType Type;

            public string Title;

            public string Text;

            public object Content;

            public Collection<Validate> ValidateRules;

            public string HintText;

            public bool HighLight;

            public string Button1;

            public string Button2;

            public string Button3;

            public Action Button1Action;

            public Action Button2Action;

            public Action Button3Action;

            public bool IsWarn;

            public bool ForceWait;

            public DispatcherFrame WaitFrame;

            public bool IsExited;

            public object Result;
        }

        public enum OurMsgBoxType
        {
            Text,
            Select,
            Input,
            Login
        }

        public class HelpEntry
        {
            public HelpEntry(string FilePath)
            {
                this.Logo = null;
                this.ShowInSearch = true;
                this.ShowInPublic = true;
                this.ShowInSnapshot = true;
                this.RawPath = FilePath;
                if (!FileUtils.Exists(FilePath))
                {
                    throw new FileNotFoundException("未找到帮助文件：" + FilePath, FilePath);
                }
                JObject jobject = (JObject)ModMain.ArgumentReplace(FileUtils.ReadAsString(FilePath, null, null), new Func<string, string>(StringUtils.XmlEscape), true).DeserializeJson();
                if (jobject["Title"] == null)
                {
                    throw new ArgumentException("未找到 Title 项");
                }
                this.Title = (string)jobject["Title"];
                this.Desc = (string)(jobject["Description"] ?? "");
                this.Search = (string)(jobject["Keywords"] ?? "");
                this.Logo = (string)jobject["Logo"];
                this.ShowInSearch = (bool)(jobject["ShowInSearch"] ?? this.ShowInSearch);
                this.ShowInPublic = (bool)(jobject["ShowInPublic"] ?? this.ShowInPublic);
                this.ShowInSnapshot = (bool)(jobject["ShowInSnapshot"] ?? this.ShowInSnapshot);
                this.Types = new List<string>();
                try
                {
                    foreach (JToken jtoken in ((IEnumerable<JToken>)(jobject["Types"] ?? new JArray())))
                    {
                        this.Types.Add((string)jtoken);
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
                if (((bool?)(jobject["IsEvent"] ?? false)).GetValueOrDefault())
                {
                    this.EventType = jobject["EventType"].ToObject<CustomEvent.EventType>();
                    this.EventData = (string)(jobject["EventData"] ?? "");
                    this.IsEvent = true;
                    return;
                }
                string text = Path.ChangeExtension(FilePath, ".xaml");
                if (FileUtils.Exists(text))
                {
                    this.XamlContent = FileUtils.ReadAsString(text, null, null);
                    this.IsEvent = false;
                    return;
                }
                throw new FileNotFoundException("未找到帮助条目 .json 对应的 .xaml 文件（" + text + "）");
            }

            public OurListItem ToListItem()
            {
                return this.SetToListItem(new OurListItem());
            }

            public OurListItem SetToListItem(OurListItem Item)
            {
                string text;
                if (this.IsEvent)
                {
                    if (this.EventType == CustomEvent.EventType.弹出窗口)
                    {
                        text = ModBase.PathImage + "Blocks/GrassPath.png";
                    }
                    else
                    {
                        text = ModBase.PathImage + "Blocks/CommandBlock.png";
                    }
                }
                else
                {
                    text = ModBase.PathImage + "Blocks/Grass.png";
                }
                Item.SnapsToDevicePixels = true;
                Item.Title = this.Title;
                Item.Info = this.Desc;
                Item.Logo = this.Logo ?? text;
                Item.Height = 42.0;
                Item.Type = OurListItem.CheckType.Clickable;
                Item.Tag = this;
                CustomEventService.SetEventType(Item, CustomEvent.EventType.None);
                CustomEventService.SetEventData(Item, null);
                Item.Click += ((ModMain.HelpEntry._Closure___._I15_0 == null) ? (ModMain.HelpEntry._Closure___._I15_0 = new OurListItem.ClickEventHandler(ModMain.HelpEntry._Closure___._I._Lambda___15_0)) : ModMain.HelpEntry._Closure___._I15_0);
                return Item;
            }

            public string RawPath;

            public string Title;

            public string Desc;

            public string Search;

            public List<string> Types;

            public string Logo;

            public bool ShowInSearch;

            public bool ShowInPublic;

            public bool ShowInSnapshot;

            public bool IsEvent;

            public CustomEvent.EventType EventType;

            public string EventData;

            public string XamlContent;

            [CompilerGenerated]
            [Serializable]
            private sealed class _Closure___
            {
                public _Closure___()
                {
                }

                static _Closure___()
                {
                }

                internal void _Lambda___15_0(object sender, MouseButtonEventArgs e)
                {
                    PageOtherHelp.OnItemClick((ModMain.HelpEntry)NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null));
                }

                public static readonly ModMain.HelpEntry._Closure___ _I = new ModMain.HelpEntry._Closure___();

                public static OurListItem.ClickEventHandler _I15_0;
            }
        }

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            public _Closure___()
            {
            }

            static _Closure___()
            {
            }

            internal string _Lambda___71_1()
            {
                return ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(CryptographyUtils.ComputeFileHash(ModBase.PathExe, CryptographyUtils.HashMethod.Md5));
            }

            internal string _Lambda___71_2()
            {
                return ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(CryptographyUtils.ComputeFileHash(ModBase.PathExe, CryptographyUtils.HashMethod.Sha1));
            }

            internal string _Lambda___R71_1(Match a0)
            {
                return ((ModMain._Closure___._I71_3 == null) ? (ModMain._Closure___._I71_3 = new VB_AnonymousDelegate_0<string>(ModMain._Closure___._I._Lambda___71_3)) : ModMain._Closure___._I71_3)();
            }

            internal string _Lambda___71_3()
            {
                return ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(PageOtherTest.GetRandomHint());
            }

            internal string _Lambda___R71_2(Match a0)
            {
                return ((ModMain._Closure___._I71_4 == null) ? (ModMain._Closure___._I71_4 = new VB_AnonymousDelegate_0<string>(ModMain._Closure___._I._Lambda___71_4)) : ModMain._Closure___._I71_4)();
            }

            internal string _Lambda___71_4()
            {
                return ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(PageOtherTest.GetRandomCave());
            }

            internal string _Lambda___71_5(Match m)
            {
                return ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(Conversions.ToString(Settings.GetSafe(m.Groups[1].Value, ModMinecraft.McInstanceSelected)));
            }

            internal string _Lambda___71_6(Match m)
            {
                return ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(Conversions.ToString(RegistryUtils.TryRead(string.Format("HKEY_CURRENT_USER\\Software\\{0}", "PCL"), string.Format("CustomEvent{0}", m.Groups[1].Value), m.Groups[2].Value)));
            }

            internal string _Lambda___71_7(Match m)
            {
                return ModMain._STATIC_ArgumentReplace_03EE151280F92EE2_Replacer(Conversions.ToString(RegistryUtils.TryRead(string.Format("HKEY_CURRENT_USER\\Software\\{0}", "PCL"), string.Format("CustomEvent{0}", m.Groups[1].Value), m.Groups[2].Value)));
            }

            internal void _Lambda___82_0()
            {
                if (!ModMain.FrmMain.Hidden)
                {
                    if (ModMain.FrmMain.Top < -9000.0)
                    {
                        ModMain.FrmMain.Top = 100.0;
                    }
                    if (ModMain.FrmMain.Left < -9000.0)
                    {
                        ModMain.FrmMain.Left = 100.0;
                    }
                }
            }

            internal void _Lambda___83_0()
            {
                try
                {
                    for (;;)
                    {
                        ModBase.RunInUiWait(new Action(ModMain.TimerMain));
                        Thread.Sleep(49);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "程序主时钟出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                }
            }

            internal void _Lambda___83_1()
            {
                try
                {
                    int num = OurWpfExtension.Computer.Clock.TickCount;
                    for (;;)
                    {
                        if (num != OurWpfExtension.Computer.Clock.TickCount)
                        {
                            num = OurWpfExtension.Computer.Clock.TickCount;
                            ModBase.RunInUiWait(new Action(ModMain.TimerFool));
                        }
                        Thread.Sleep(1);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "愚人节主时钟出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMain.vb");
                }
            }

            public static readonly ModMain._Closure___ _I = new ModMain._Closure___();

            public static Func<string> _I71_1;

            public static Func<string> _I71_2;

            public static VB_AnonymousDelegate_0<string> _I71_3;

            public static MatchEvaluator _IR71_1;

            public static VB_AnonymousDelegate_0<string> _I71_4;

            public static MatchEvaluator _IR71_2;

            public static MatchEvaluator _I71_5;

            public static MatchEvaluator _I71_6;

            public static MatchEvaluator _I71_7;

            public static Action _I82_0;

            public static Action _I83_0;

            public static Action _I83_1;
        }

        [CompilerGenerated]
        private sealed class _Closure___1_0
        {
            public _Closure___1_0(ModMain._Closure___1_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Events = arg0._VB_Local_Events;
                }
            }

            internal void _Lambda___0()
            {
                try
                {
                    foreach (CustomEvent customEvent in this._VB_Local_Events)
                    {
                        customEvent.Raise();
                    }
                }
                finally
                {
                    List<CustomEvent>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
            }

            public List<CustomEvent> _VB_Local_Events;
        }

        [CompilerGenerated]
        private sealed class _Closure___6_0
        {
            public _Closure___6_0(ModMain._Closure___6_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Percent = arg0._VB_Local_Percent;
                    this._VB_Local_DoubleStack = arg0._VB_Local_DoubleStack;
                    this._VB_Local_TargetColor0 = arg0._VB_Local_TargetColor0;
                    this._VB_Local_TargetColor1 = arg0._VB_Local_TargetColor1;
                }
            }

            internal void _Lambda___0(object i)
            {
                this._VB_Local_Percent = Conversions.ToDouble(Operators.AddObject(this._VB_Local_Percent, i));
                LinearGradientBrush linearGradientBrush = (LinearGradientBrush)this._VB_Local_DoubleStack.Background;
                linearGradientBrush.GradientStops[0].Color = this._VB_Local_TargetColor0 * this._VB_Local_Percent + new ModBase.OurColor(255.0, 255.0, 255.0) * (1.0 - this._VB_Local_Percent);
                linearGradientBrush.GradientStops[1].Color = this._VB_Local_TargetColor1 * this._VB_Local_Percent + new ModBase.OurColor(255.0, 255.0, 255.0) * (1.0 - this._VB_Local_Percent);
            }

            internal void _Lambda___1()
            {
                NewLateBinding.LateIndexSetComplex(this._VB_Local_DoubleStack.Tag, new object[] { 0, false }, null, false, true);
            }

            internal void _Lambda___2()
            {
                ModMain.FrmMain.PanHint.Children.Remove(this._VB_Local_DoubleStack);
            }

            public double _VB_Local_Percent;

            public Border _VB_Local_DoubleStack;

            public ModBase.OurColor _VB_Local_TargetColor0;

            public ModBase.OurColor _VB_Local_TargetColor1;
        }

        [CompilerGenerated]
        private sealed class _Closure___6_1
        {
            public _Closure___6_1(ModMain._Closure___6_1 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_NewHintControl = arg0._VB_Local_NewHintControl;
                }
            }

            internal void _Lambda___3(object i)
            {
                this._VB_NonLocal__VB_Closure_2._VB_Local_Percent = Conversions.ToDouble(Operators.AddObject(this._VB_NonLocal__VB_Closure_2._VB_Local_Percent, i));
                LinearGradientBrush linearGradientBrush = (LinearGradientBrush)this._VB_Local_NewHintControl.Background;
                linearGradientBrush.GradientStops[0].Color = this._VB_NonLocal__VB_Closure_2._VB_Local_TargetColor0 * this._VB_NonLocal__VB_Closure_2._VB_Local_Percent + new ModBase.OurColor(255.0, 255.0, 255.0) * (1.0 - this._VB_NonLocal__VB_Closure_2._VB_Local_Percent);
                linearGradientBrush.GradientStops[1].Color = this._VB_NonLocal__VB_Closure_2._VB_Local_TargetColor1 * this._VB_NonLocal__VB_Closure_2._VB_Local_Percent + new ModBase.OurColor(255.0, 255.0, 255.0) * (1.0 - this._VB_NonLocal__VB_Closure_2._VB_Local_Percent);
            }

            internal void _Lambda___4()
            {
                NewLateBinding.LateIndexSetComplex(this._VB_Local_NewHintControl.Tag, new object[] { 0, false }, null, false, true);
            }

            internal void _Lambda___5()
            {
                ModMain.FrmMain.PanHint.Children.Remove(this._VB_Local_NewHintControl);
            }

            public Border _VB_Local_NewHintControl;

            public ModMain._Closure___6_0 _VB_NonLocal__VB_Closure_2;
        }

        [CompilerGenerated]
        private sealed class _Closure___7_0
        {
            public _Closure___7_0(ModMain._Closure___7_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Control = arg0._VB_Local_Control;
                }
            }

            internal void _Lambda___0()
            {
                NewLateBinding.LateIndexSetComplex(this._VB_Local_Control.Tag, new object[] { 0, false }, null, false, true);
            }

            internal void _Lambda___1()
            {
                ModMain.FrmMain.PanHint.Children.Remove(this._VB_Local_Control);
            }

            public Border _VB_Local_Control;
        }

        [CompilerGenerated]
        private sealed class _Closure___71_0
        {
            public _Closure___71_0()
            {
            }

            internal string _Lambda___0(string s)
            {
                string text;
                if (s == null)
                {
                    text = "";
                }
                else if (this._VB_Local_EscapeHandler == null)
                {
                    text = s;
                }
                else
                {
                    if (s.Contains(":\\"))
                    {
                        bool flag = s.EndsWithF("\\", false) || s.EndsWithF("/", false);
                        s = PathUtils.ToShortPath(s);
                        if (flag)
                        {
                            s = PathUtils.AddSlashSuffix(s);
                        }
                    }
                    text = this._VB_Local_EscapeHandler(s);
                }
                return text;
            }

            public Func<string, string> _VB_Local_EscapeHandler;
        }

    }
}
