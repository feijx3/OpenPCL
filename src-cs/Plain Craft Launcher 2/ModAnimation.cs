using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using MeloongCore;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [StandardModule]
    public sealed class ModAnimation
    {
        public static void AniDispose(MyCard Control, bool RemoveFromChildren, ParameterizedThreadStart CallBack = null)
        {
            if (Control.IsHitTestVisible)
            {
                Control.IsHitTestVisible = false;
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaScaleTransform(Control, -0.08, 200, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaOpacity(Control, -1.0, 200, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaHeight(Control, -Control.ActualHeight, 150, 100, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaCode(delegate
                    {
                        if (RemoveFromChildren)
                        {
                            if (Control.Parent == null)
                            {
                                return;
                            }
                            object[] array3;
                            bool[] array4;
                            NewLateBinding.LateCall(NewLateBinding.LateGet(Control.Parent, null, "Children", new object[0], null, null, null), null, "Remove", array3 = new object[] { Control }, null, null, array4 = new bool[] { true }, true);
                            if (array4[0])
                            {
                                Control = (MyCard)Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array3[0]), typeof(MyCard));
                            }
                        }
                        else
                        {
                            Control.Visibility = Visibility.Collapsed;
                        }
                        if (CallBack != null)
                        {
                            CallBack(Control);
                        }
                    }, 0, true)
                }, "MyCard Dispose " + Conversions.ToString(Control.Uuid), false);
                return;
            }
            if (RemoveFromChildren)
            {
                if (Control.Parent == null)
                {
                    return;
                }
                object[] array;
                bool[] array2;
                NewLateBinding.LateCall(NewLateBinding.LateGet(Control.Parent, null, "Children", new object[0], null, null, null), null, "Remove", array = new object[] { Control }, null, null, array2 = new bool[] { true }, true);
                if (array2[0])
                {
                    Control = (MyCard)Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array[0]), typeof(MyCard));
                }
            }
            else
            {
                Control.Visibility = Visibility.Collapsed;
            }
            if (CallBack != null)
            {
                CallBack(Control);
            }
        }

        public static void AniDispose(MyHint Control, bool RemoveFromChildren, ParameterizedThreadStart CallBack = null)
        {
            if (Control.IsHitTestVisible)
            {
                Control.IsHitTestVisible = false;
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaScaleTransform(Control, -0.08, 200, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaOpacity(Control, -1.0, 200, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaHeight(Control, -Control.ActualHeight, 150, 100, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaCode(delegate
                    {
                        if (RemoveFromChildren)
                        {
                            object[] array;
                            bool[] array2;
                            NewLateBinding.LateCall(NewLateBinding.LateGet(Control.Parent, null, "Children", new object[0], null, null, null), null, "Remove", array = new object[] { Control }, null, null, array2 = new bool[] { true }, true);
                            if (array2[0])
                            {
                                Control = (MyHint)Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array[0]), typeof(MyHint));
                            }
                        }
                        else
                        {
                            Control.Visibility = Visibility.Collapsed;
                        }
                        if (CallBack != null)
                        {
                            CallBack(Control);
                        }
                    }, 0, true)
                }, "MyCard Dispose " + Conversions.ToString(Control.Uuid), false);
            }
        }

        public static void AniDispose(MyIconButton Control, bool RemoveFromChildren, ParameterizedThreadStart CallBack = null)
        {
            if (Control.IsHitTestVisible)
            {
                Control.IsHitTestVisible = false;
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaScaleTransform(Control, -1.5, 200, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaCode(delegate
                    {
                        if (RemoveFromChildren)
                        {
                            object[] array;
                            bool[] array2;
                            NewLateBinding.LateCall(NewLateBinding.LateGet(Control.Parent, null, "Children", new object[0], null, null, null), null, "Remove", array = new object[] { Control }, null, null, array2 = new bool[] { true }, true);
                            if (array2[0])
                            {
                                Control = (MyIconButton)Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array[0]), typeof(MyIconButton));
                            }
                        }
                        else
                        {
                            Control.Visibility = Visibility.Collapsed;
                        }
                        if (CallBack != null)
                        {
                            CallBack(Control);
                        }
                    }, 0, true)
                }, "MyIconButton Dispose " + Conversions.ToString(Control.Uuid), false);
            }
        }

        public static double AniSpeed
        {
            get
            {
                int num = Settings.Get<int>("SystemDebugAnim", null);
                if (num < 30)
                {
                    return ((double)num * 0.1 + 0.1).Clamp(0.1, 3.0);
                }
                return 200.0;
            }
        }

        public static int AniControlEnabled
        {
            get
            {
                return ModAnimation._AniControlEnabled;
            }
            set
            {
                object aniControlEnabledLock = ModAnimation.AniControlEnabledLock;
                ObjectFlowControl.CheckForSyncLockOnValueType(aniControlEnabledLock);
                lock (aniControlEnabledLock)
                {
                    ModAnimation._AniControlEnabled = value;
                }
            }
        }

        public static ModAnimation.AniData AaX(object Obj, double Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Number,
                TypeSub = ModAnimation.AniTypeSub.X,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = RuntimeHelpers.GetObjectValue(Obj),
                Value = Value,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static ModAnimation.AniData AaY(object Obj, double Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Number,
                TypeSub = ModAnimation.AniTypeSub.Y,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = RuntimeHelpers.GetObjectValue(Obj),
                Value = Value,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static ModAnimation.AniData AaWidth(object Obj, double Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Number,
                TypeSub = ModAnimation.AniTypeSub.Width,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = RuntimeHelpers.GetObjectValue(Obj),
                Value = Value,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static ModAnimation.AniData AaHeight(object Obj, double Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Number,
                TypeSub = ModAnimation.AniTypeSub.Height,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = RuntimeHelpers.GetObjectValue(Obj),
                Value = Value,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static ModAnimation.AniData AaOpacity(object Obj, double Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Number,
                TypeSub = ModAnimation.AniTypeSub.Opacity,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = RuntimeHelpers.GetObjectValue(Obj),
                Value = Value,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static ModAnimation.AniData AaValue(object Obj, double Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Number,
                TypeSub = ModAnimation.AniTypeSub.Value,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = RuntimeHelpers.GetObjectValue(Obj),
                Value = Value,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static ModAnimation.AniData AaRadius(object Obj, double Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Number,
                TypeSub = ModAnimation.AniTypeSub.Radius,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = RuntimeHelpers.GetObjectValue(Obj),
                Value = Value,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static ModAnimation.AniData AaBorderThickness(object Obj, double Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Number,
                TypeSub = ModAnimation.AniTypeSub.BorderThickness,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = RuntimeHelpers.GetObjectValue(Obj),
                Value = Value,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static ModAnimation.AniData AaStrokeThickness(object Obj, double Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Number,
                TypeSub = ModAnimation.AniTypeSub.StrokeThickness,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = RuntimeHelpers.GetObjectValue(Obj),
                Value = Value,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static ModAnimation.AniData AaGridLengthWidth(object Obj, double Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Number,
                TypeSub = ModAnimation.AniTypeSub.GridLengthWidth,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = RuntimeHelpers.GetObjectValue(Obj),
                Value = Value,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static ModAnimation.AniData AaDouble(object Obj, DependencyProperty Prop, double Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Number,
                TypeSub = ModAnimation.AniTypeSub.Double,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = new object[] { Obj, Prop, "" },
                Value = Value,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static ModAnimation.AniData AaDouble(ParameterizedThreadStart Lambda, double Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Number,
                TypeSub = ModAnimation.AniTypeSub.DoubleParam,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = Lambda,
                Value = Value,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static ModAnimation.AniData AaColor(FrameworkElement Obj, DependencyProperty Prop, ModBase.MyColor Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Color,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = new object[] { Obj, Prop, "" },
                Value = Value,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay)),
                ValueLast = new ModBase.MyColor(0.0, 0.0, 0.0, 0.0)
            };
        }

        public static ModAnimation.AniData AaColor(FrameworkElement Obj, DependencyProperty Prop, string Res, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Color,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = new object[] { Obj, Prop, Res },
                Value = new ModBase.MyColor(RuntimeHelpers.GetObjectValue(Application.Current.FindResource(Res))) - new ModBase.MyColor(RuntimeHelpers.GetObjectValue(Obj.GetValue(Prop))),
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay)),
                ValueLast = new ModBase.MyColor(0.0, 0.0, 0.0, 0.0)
            };
        }

        public static ModAnimation.AniData AaScale(object Obj, double Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false, bool Absolute = false)
        {
            ModBase.MyRect myRect;
            if (Absolute)
            {
                myRect = new ModBase.MyRect(-0.5 * Value, -0.5 * Value, Value, Value);
            }
            else
            {
                myRect = new ModBase.MyRect(Conversions.ToDouble(Operators.MultiplyObject(Operators.MultiplyObject(-0.5, NewLateBinding.LateGet(Obj, null, "ActualWidth", new object[0], null, null, null)), Value)), Conversions.ToDouble(Operators.MultiplyObject(Operators.MultiplyObject(-0.5, NewLateBinding.LateGet(Obj, null, "ActualHeight", new object[0], null, null, null)), Value)), Conversions.ToDouble(Operators.MultiplyObject(NewLateBinding.LateGet(Obj, null, "ActualWidth", new object[0], null, null, null), Value)), Conversions.ToDouble(Operators.MultiplyObject(NewLateBinding.LateGet(Obj, null, "ActualHeight", new object[0], null, null, null), Value)));
            }
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Scale,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = RuntimeHelpers.GetObjectValue(Obj),
                Value = myRect,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static ModAnimation.AniData AaTextAppear(object Obj, bool Hide = false, bool TimePerText = true, int Time = 70, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return checked(new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.TextAppear,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                TimeTotal = (TimePerText ? (Time * ((Obj is TextBlock) ? NewLateBinding.LateGet(Obj, null, "Text", new object[0], null, null, null) : NewLateBinding.LateGet(Obj, null, "Context", new object[0], null, null, null).ToString()).ToString().Length) : Time),
                Obj = RuntimeHelpers.GetObjectValue(Obj),
                Value = new object[]
                {
                    (Obj is TextBlock) ? NewLateBinding.LateGet(Obj, null, "Text", new object[0], null, null, null) : NewLateBinding.LateGet(Obj, null, "Context", new object[0], null, null, null).ToString(),
                    Hide
                },
                IsAfter = After,
                TimeFinished = (double)(0 - Delay)
            });
        }

        public static ModAnimation.AniData AaCode(ThreadStart Code, int Delay = 0, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Code,
                TimeTotal = 1,
                Value = Code,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static ModAnimation.AniData AaScaleTransform(object Obj, double Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.ScaleTransform,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = RuntimeHelpers.GetObjectValue(Obj),
                Value = Value,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static ModAnimation.AniData AaRotateTransform(object Obj, double Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.RotateTransform,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = RuntimeHelpers.GetObjectValue(Obj),
                Value = Value,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static ModAnimation.AniData AaTranslateX(object Obj, double Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Number,
                TypeSub = ModAnimation.AniTypeSub.TranslateX,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = RuntimeHelpers.GetObjectValue(Obj),
                Value = Value,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static ModAnimation.AniData AaTranslateY(object Obj, double Value, int Time = 400, int Delay = 0, ModAnimation.AniEase Ease = null, bool After = false)
        {
            return new ModAnimation.AniData
            {
                TypeMain = ModAnimation.AniType.Number,
                TypeSub = ModAnimation.AniTypeSub.TranslateY,
                TimeTotal = Time,
                Ease = (Ease ?? new ModAnimation.AniEaseLinear()),
                Obj = RuntimeHelpers.GetObjectValue(Obj),
                Value = Value,
                IsAfter = After,
                TimeFinished = (double)(checked(0 - Delay))
            };
        }

        public static List<ModAnimation.AniData> AaStack(StackPanel Stack, int Time = 100, int Delay = 25)
        {
            List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
            int num = 0;
            checked
            {
                try
                {
                    foreach (object obj in Stack.Children)
                    {
                        object objectValue = RuntimeHelpers.GetObjectValue(obj);
                        NewLateBinding.LateSet(objectValue, null, "Opacity", new object[] { 0 }, null, null);
                        list.Add(ModAnimation.AaOpacity(RuntimeHelpers.GetObjectValue(objectValue), 1.0, Time, num, null, false));
                        num += Delay;
                    }
                }
                finally
                {
                    IEnumerator enumerator;
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                return list;
            }
        }

        public static void AniStart(IList AniGroup, string Name = "", bool RefreshTime = false)
        {
            if (RefreshTime)
            {
                ModAnimation.AniLastTick = ModBase.GetTimeMs();
            }
            List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
            try
            {
                foreach (object obj in AniGroup)
                {
                    object objectValue = RuntimeHelpers.GetObjectValue(obj);
                    if (objectValue is ICollection)
                    {
                        list.AddRange((IEnumerable<ModAnimation.AniData>)objectValue);
                    }
                    else
                    {
                        List<ModAnimation.AniData> list2 = list;
                        object obj2 = objectValue;
                        list2.Add((obj2 != null) ? ((ModAnimation.AniData)obj2) : default(ModAnimation.AniData));
                    }
                }
            }
            finally
            {
                IEnumerator enumerator;
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            ModAnimation.AniGroupEntry aniGroupEntry = new ModAnimation.AniGroupEntry
            {
                Data = list,
                StartTick = ModBase.GetTimeMs()
            };
            if (Operators.CompareString(Name, "", false) == 0)
            {
                Name = Conversions.ToString(aniGroupEntry.Uuid);
            }
            else
            {
                ModAnimation.AniStop(Name);
            }
            ModAnimation.AniGroups.Add(Name, aniGroupEntry);
        }

        public static void AniStart(ModAnimation.AniData AniGroup, string Name = "", bool RefreshTime = false)
        {
            ModAnimation.AniStart(new List<ModAnimation.AniData> { AniGroup }, Name, RefreshTime);
        }

        public static void AniStop(string Name)
        {
            ModAnimation.AniGroups.Remove(Name);
        }

        public static bool AniIsRun(string Name)
        {
            return ModAnimation.AniGroups.ContainsKey(Name);
        }

        public static void AniStart()
        {
            ModAnimation.AniLastTick = ModBase.GetTimeMs();
            ModAnimation.AniFPSTimer = ModAnimation.AniLastTick;
            ModAnimation.AniRunning = true;
            checked
            {
                ModBase.RunInNewThread((ModAnimation._Closure$__.$I62-0 == null) ? (ModAnimation._Closure$__.$I62-0 = delegate
                {
                    try
                    {
                        Logger.Info("动画线程开始", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModAnimation.vb");
                        for (;;)
                        {
                            ModAnimation._Closure$__62-0 CS$<>8__locals1 = new ModAnimation._Closure$__62-0(CS$<>8__locals1);
                            CS$<>8__locals1.$VB$Local_DeltaTime = (ModBase.GetTimeMs() - ModAnimation.AniLastTick).Clamp(0L, 100000L);
                            if (CS$<>8__locals1.$VB$Local_DeltaTime >= 3L)
                            {
                                ModAnimation.AniLastTick = ModBase.GetTimeMs();
                                if (ModBase.ModeDebug)
                                {
                                    if ((ModAnimation.AniLastTick - ModAnimation.AniFPSTimer).Clamp(0L, 100000L) >= 500L)
                                    {
                                        ModAnimation.AniFPS = ModAnimation.AniFPSCounter;
                                        ModAnimation.AniFPSCounter = 0;
                                        ModAnimation.AniFPSTimer = ModAnimation.AniLastTick;
                                    }
                                    ModAnimation.AniFPSCounter += 2;
                                }
                                ModBase.RunInUiWait(delegate
                                {
                                    ModAnimation.AniCount = 0;
                                    ModAnimation.AniTimer(unchecked((double)CS$<>8__locals1.$VB$Local_DeltaTime * ModAnimation.AniSpeed));
                                    if (ModBase.RandomInteger(0, 64 * (ModBase.ModeDebug ? 5 : 30)) == 0 && ((ModAnimation.AniFPS < 62 && ModAnimation.AniFPS > 0) || ModAnimation.AniCount > 4 || ModNet.NetManager.FileRemain != 0))
                                    {
                                        Logger.Info(string.Format("FPS {0}, 动画 {1}, 下载中 {2}（{3}/s）", new object[]
                                        {
                                            ModAnimation.AniFPS,
                                            ModAnimation.AniCount,
                                            ModNet.NetManager.FileRemain,
                                            StringUtils.FormatByteSize(ModNet.NetManager.Speed)
                                        }), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModAnimation.vb");
                                        if (ModNet.NetManager.FileRemain < 5)
                                        {
                                            try
                                            {
                                                foreach (ModNet.NetFile netFile in ModNet.NetManager.AllFiles.Values)
                                                {
                                                    if (netFile.State < ModNet.NetState.Finished)
                                                    {
                                                        Logger.Trace(string.Format("- {0} {1}/s {2}", netFile.State, StringUtils.FormatByteSize(netFile.Speed), netFile.LocalPath), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModAnimation.vb");
                                                    }
                                                }
                                            }
                                            finally
                                            {
                                                IEnumerator<ModNet.NetFile> enumerator;
                                                if (enumerator != null)
                                                {
                                                    enumerator.Dispose();
                                                }
                                            }
                                        }
                                    }
                                });
                            }
                            Thread.Sleep(1);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "动画帧执行失败", LogBehavior.AlertThenCrash, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModAnimation.vb");
                    }
                }) : ModAnimation._Closure$__.$I62-0, "Animation", ThreadPriority.AboveNormal);
            }
        }

        public static void AniTimer(double DeltaTick)
        {
            checked
            {
                try
                {
                    if (DeltaTick / ModAnimation.AniSpeed > 100.0)
                    {
                        Logger.Info(string.Format("两个动画帧间隔 {0} ms", DeltaTick), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModAnimation.vb");
                    }
                    int num = -1;
                    while (num + 1 < ModAnimation.AniGroups.Count)
                    {
                        num++;
                        ModAnimation.AniGroupEntry aniGroupEntry = ModAnimation.AniGroups.Values.ElementAtOrDefault<ModAnimation.AniGroupEntry>(num);
                        if (aniGroupEntry.StartTick <= ModAnimation.AniLastTick)
                        {
                            bool flag = true;
                            int i = 0;
                            while (i < aniGroupEntry.Data.Count)
                            {
                                ModAnimation.AniData aniData = aniGroupEntry.Data[i];
                                if (!aniData.IsAfter)
                                {
                                    flag = false;
                                    ref double ptr = ref aniData.TimeFinished;
                                    aniData.TimeFinished = unchecked(ptr + DeltaTick);
                                    if (aniData.TimeFinished > 0.0)
                                    {
                                        aniData = ModAnimation.AniRun(aniData);
                                        ModAnimation.AniCount++;
                                    }
                                    if (aniData.TimeFinished >= (double)aniData.TimeTotal)
                                    {
                                        if (Conversions.ToBoolean(aniData.TypeMain == ModAnimation.AniType.Color && Conversions.ToBoolean(Operators.NotObject(Operators.CompareObjectEqual(NewLateBinding.LateIndexGet(aniData.Obj, new object[] { 2 }, null), "", false)))))
                                        {
                                            object obj = NewLateBinding.LateIndexGet(aniData.Obj, new object[] { 0 }, null);
                                            Type type = null;
                                            string text = "SetResourceReference";
                                            object[] array = new object[2];
                                            int num2 = 0;
                                            object obj2 = aniData.Obj;
                                            object obj3 = obj2;
                                            object[] array2 = new object[1];
                                            object obj4 = (array2[0] = 1);
                                            array[num2] = NewLateBinding.LateIndexGet(obj3, array2, null);
                                            int num3 = 1;
                                            object obj5 = aniData.Obj;
                                            object obj6 = obj5;
                                            object[] array3 = new object[1];
                                            object obj7 = (array3[0] = 2);
                                            array[num3] = NewLateBinding.LateIndexGet(obj6, array3, null);
                                            object[] array4 = array;
                                            bool[] array5;
                                            NewLateBinding.LateCall(obj, type, text, array, null, null, array5 = new bool[] { true, true }, true);
                                            if (array5[0])
                                            {
                                                NewLateBinding.LateIndexSetComplex(obj2, new object[]
                                                {
                                                    obj4,
                                                    array4[0]
                                                }, null, true, false);
                                            }
                                            if (array5[1])
                                            {
                                                NewLateBinding.LateIndexSetComplex(obj5, new object[]
                                                {
                                                    obj7,
                                                    array4[1]
                                                }, null, true, false);
                                            }
                                        }
                                        aniGroupEntry.Data.RemoveAt(i);
                                    }
                                    else
                                    {
                                        aniGroupEntry.Data[i] = aniData;
                                        i++;
                                    }
                                }
                                else
                                {
                                    if (!flag)
                                    {
                                        break;
                                    }
                                    flag = false;
                                    aniData.IsAfter = false;
                                    aniGroupEntry.Data[i] = aniData;
                                }
                            }
                            if (!aniGroupEntry.Data.Any<ModAnimation.AniData>())
                            {
                                int num4 = ModAnimation.AniGroups.Count - 1;
                                for (int j = 0; j <= num4; j++)
                                {
                                    if (ModAnimation.AniGroups.ElementAt<KeyValuePair<string, ModAnimation.AniGroupEntry>>(j).Value.Uuid == aniGroupEntry.Uuid)
                                    {
                                        ModAnimation.AniGroups.Remove(ModAnimation.AniGroups.ElementAt<KeyValuePair<string, ModAnimation.AniGroupEntry>>(j).Key);
                                        break;
                                    }
                                }
                                num--;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "动画刻执行失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModAnimation.vb");
                }
            }
        }

        private static ModAnimation.AniData AniRun(ModAnimation.AniData Ani)
        {
            try
            {
                switch (Ani.TypeMain)
                {
                case ModAnimation.AniType.Number:
                {
                    double num = MathUtils.Lerp(0.0, Conversions.ToDouble(Ani.Value), Ani.Ease.GetDelta(Ani.TimeFinished / (double)Ani.TimeTotal, Ani.TimePercent));
                    if (num != 0.0)
                    {
                        switch (Ani.TypeSub)
                        {
                        case ModAnimation.AniTypeSub.X:
                            if (Ani.Obj is Window)
                            {
                                Window window;
                                (window = (Window)Ani.Obj).Left = window.Left + num;
                            }
                            else
                            {
                                object obj = NewLateBinding.LateGet(Ani.Obj, null, "HorizontalAlignment", new object[0], null, null, null);
                                if (Conversions.ToBoolean(Conversions.ToBoolean(Operators.CompareObjectEqual(obj, HorizontalAlignment.Left, false)) || Conversions.ToBoolean(Operators.CompareObjectEqual(obj, HorizontalAlignment.Stretch, false))))
                                {
                                    NewLateBinding.LateSet(Ani.Obj, null, "Margin", new object[]
                                    {
                                        new Thickness(Conversions.ToDouble(Operators.AddObject(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, null, "Margin", new object[0], null, null, null), null, "Left", new object[0], null, null, null), num)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, null, "Margin", new object[0], null, null, null), null, "Top", new object[0], null, null, null)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, null, "Margin", new object[0], null, null, null), null, "Right", new object[0], null, null, null)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, null, "Margin", new object[0], null, null, null), null, "Bottom", new object[0], null, null, null)))
                                    }, null, null);
                                }
                                else if (Operators.ConditionalCompareObjectEqual(obj, HorizontalAlignment.Right, false))
                                {
                                    NewLateBinding.LateSet(Ani.Obj, null, "Margin", new object[]
                                    {
                                        new Thickness(Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, null, "Margin", new object[0], null, null, null), null, "Left", new object[0], null, null, null)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, null, "Margin", new object[0], null, null, null), null, "Top", new object[0], null, null, null)), Conversions.ToDouble(Operators.SubtractObject(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, null, "Margin", new object[0], null, null, null), null, "Right", new object[0], null, null, null), num)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, null, "Margin", new object[0], null, null, null), null, "Bottom", new object[0], null, null, null)))
                                    }, null, null);
                                }
                            }
                            break;
                        case ModAnimation.AniTypeSub.Y:
                            if (Ani.Obj is Window)
                            {
                                Window window;
                                (window = (Window)Ani.Obj).Top = window.Top + num;
                            }
                            else
                            {
                                object obj2 = NewLateBinding.LateGet(Ani.Obj, null, "VerticalAlignment", new object[0], null, null, null);
                                if (Operators.ConditionalCompareObjectEqual(obj2, VerticalAlignment.Top, false))
                                {
                                    NewLateBinding.LateSet(Ani.Obj, null, "Margin", new object[]
                                    {
                                        new Thickness(Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, null, "Margin", new object[0], null, null, null), null, "Left", new object[0], null, null, null)), Conversions.ToDouble(Operators.AddObject(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, null, "Margin", new object[0], null, null, null), null, "Top", new object[0], null, null, null), num)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, null, "Margin", new object[0], null, null, null), null, "Right", new object[0], null, null, null)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, null, "Margin", new object[0], null, null, null), null, "Bottom", new object[0], null, null, null)))
                                    }, null, null);
                                }
                                else if (Operators.ConditionalCompareObjectEqual(obj2, VerticalAlignment.Bottom, false))
                                {
                                    NewLateBinding.LateSet(Ani.Obj, null, "Margin", new object[]
                                    {
                                        new Thickness(Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, null, "Margin", new object[0], null, null, null), null, "Left", new object[0], null, null, null)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, null, "Margin", new object[0], null, null, null), null, "Top", new object[0], null, null, null)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, null, "Margin", new object[0], null, null, null), null, "Right", new object[0], null, null, null)), Conversions.ToDouble(Operators.SubtractObject(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, null, "Margin", new object[0], null, null, null), null, "Bottom", new object[0], null, null, null), num)))
                                    }, null, null);
                                }
                            }
                            break;
                        case ModAnimation.AniTypeSub.Width:
                        {
                            FrameworkElement frameworkElement = (FrameworkElement)Ani.Obj;
                            frameworkElement.Width = Math.Max((double.IsNaN(frameworkElement.Width) ? frameworkElement.ActualWidth : frameworkElement.Width) + num, 0.0);
                            break;
                        }
                        case ModAnimation.AniTypeSub.Height:
                        {
                            FrameworkElement frameworkElement2 = (FrameworkElement)Ani.Obj;
                            frameworkElement2.Height = Math.Max((double.IsNaN(frameworkElement2.Height) ? frameworkElement2.ActualHeight : frameworkElement2.Height) + num, 0.0);
                            break;
                        }
                        case ModAnimation.AniTypeSub.Opacity:
                            if (Ani.Obj is UIElement)
                            {
                                UIElement uielement = (UIElement)Ani.Obj;
                                uielement.Opacity = (uielement.Opacity + num).Clamp(0.0, 1.0);
                            }
                            else if (Ani.Obj is DropShadowEffect)
                            {
                                DropShadowEffect dropShadowEffect = (DropShadowEffect)Ani.Obj;
                                dropShadowEffect.Opacity = (dropShadowEffect.Opacity + num).Clamp(0.0, 1.0);
                            }
                            break;
                        case ModAnimation.AniTypeSub.Value:
                        {
                            object obj3 = Ani.Obj;
                            NewLateBinding.LateSet(obj3, null, "Value", new object[] { Operators.AddObject(NewLateBinding.LateGet(obj3, null, "Value", new object[0], null, null, null), num) }, null, null);
                            break;
                        }
                        case ModAnimation.AniTypeSub.Radius:
                        {
                            object obj3 = Ani.Obj;
                            NewLateBinding.LateSet(obj3, null, "Radius", new object[] { Operators.AddObject(NewLateBinding.LateGet(obj3, null, "Radius", new object[0], null, null, null), num) }, null, null);
                            break;
                        }
                        case ModAnimation.AniTypeSub.BorderThickness:
                        {
                            object obj4 = Ani.Obj;
                            Type type = null;
                            string text = "BorderThickness";
                            object[] array = new object[1];
                            int num2 = 0;
                            object obj5 = NewLateBinding.LateGet(Ani.Obj, null, "BorderThickness", new object[0], null, null, null);
                            array[num2] = new Thickness(((obj5 != null) ? ((Thickness)obj5) : default(Thickness)).Bottom + num);
                            NewLateBinding.LateSet(obj4, type, text, array, null, null);
                            break;
                        }
                        case ModAnimation.AniTypeSub.StrokeThickness:
                            NewLateBinding.LateSet(Ani.Obj, null, "StrokeThickness", new object[] { NewLateBinding.LateGet(null, typeof(Math), "Max", new object[]
                            {
                                Operators.AddObject(NewLateBinding.LateGet(Ani.Obj, null, "StrokeThickness", new object[0], null, null, null), num),
                                0
                            }, null, null, null) }, null, null);
                            break;
                        case ModAnimation.AniTypeSub.TranslateX:
                        {
                            if (Information.IsNothing(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(Ani.Obj, null, "RenderTransform", new object[0], null, null, null))) || !(NewLateBinding.LateGet(Ani.Obj, null, "RenderTransform", new object[0], null, null, null) is TranslateTransform))
                            {
                                NewLateBinding.LateSet(Ani.Obj, null, "RenderTransform", new object[]
                                {
                                    new TranslateTransform(0.0, 0.0)
                                }, null, null);
                            }
                            TranslateTransform translateTransform;
                            (translateTransform = (TranslateTransform)NewLateBinding.LateGet(Ani.Obj, null, "RenderTransform", new object[0], null, null, null)).X = translateTransform.X + num;
                            break;
                        }
                        case ModAnimation.AniTypeSub.TranslateY:
                        {
                            if (Information.IsNothing(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(Ani.Obj, null, "RenderTransform", new object[0], null, null, null))) || !(NewLateBinding.LateGet(Ani.Obj, null, "RenderTransform", new object[0], null, null, null) is TranslateTransform))
                            {
                                NewLateBinding.LateSet(Ani.Obj, null, "RenderTransform", new object[]
                                {
                                    new TranslateTransform(0.0, 0.0)
                                }, null, null);
                            }
                            TranslateTransform translateTransform;
                            (translateTransform = (TranslateTransform)NewLateBinding.LateGet(Ani.Obj, null, "RenderTransform", new object[0], null, null, null)).Y = translateTransform.Y + num;
                            break;
                        }
                        case ModAnimation.AniTypeSub.Double:
                        {
                            object obj6 = NewLateBinding.LateIndexGet(Ani.Obj, new object[] { 0 }, null);
                            Type type2 = null;
                            string text2 = "SetValue";
                            object[] array2 = new object[2];
                            int num3 = 0;
                            object obj3 = Ani.Obj;
                            object obj7 = obj3;
                            object[] array3 = new object[1];
                            object obj8 = (array3[0] = 1);
                            array2[num3] = NewLateBinding.LateIndexGet(obj7, array3, null);
                            int num4 = 1;
                            object obj9 = NewLateBinding.LateIndexGet(Ani.Obj, new object[] { 0 }, null);
                            Type type3 = null;
                            string text3 = "GetValue";
                            object[] array4 = new object[1];
                            int num5 = 0;
                            object obj10 = Ani.Obj;
                            object obj11 = obj10;
                            object[] array5 = new object[1];
                            object obj12 = (array5[0] = 1);
                            array4[num5] = NewLateBinding.LateIndexGet(obj11, array5, null);
                            object[] array6 = array4;
                            bool[] array7;
                            object obj13 = NewLateBinding.LateGet(obj9, type3, text3, array4, null, null, array7 = new bool[] { true });
                            if (array7[0])
                            {
                                NewLateBinding.LateIndexSetComplex(obj10, new object[]
                                {
                                    obj12,
                                    array6[0]
                                }, null, true, false);
                            }
                            array2[num4] = Operators.AddObject(obj13, num);
                            object[] array8 = array2;
                            string[] array9 = null;
                            Type[] array10 = null;
                            bool[] array11 = new bool[2];
                            array11[0] = true;
                            bool[] array12 = array11;
                            NewLateBinding.LateCall(obj6, type2, text2, array2, array9, array10, array11, true);
                            if (array12[0])
                            {
                                NewLateBinding.LateIndexSetComplex(obj3, new object[]
                                {
                                    obj8,
                                    array8[0]
                                }, null, true, false);
                            }
                            break;
                        }
                        case ModAnimation.AniTypeSub.DoubleParam:
                            ((ParameterizedThreadStart)Ani.Obj)(num);
                            break;
                        case ModAnimation.AniTypeSub.GridLengthWidth:
                            NewLateBinding.LateSet(Ani.Obj, null, "Width", new object[]
                            {
                                new GridLength(Conversions.ToDouble(NewLateBinding.LateGet(null, typeof(Math), "Max", new object[]
                                {
                                    Operators.AddObject(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, null, "Width", new object[0], null, null, null), null, "Value", new object[0], null, null, null), num),
                                    0
                                }, null, null, null)), GridUnitType.Star)
                            }, null, null);
                            break;
                        }
                    }
                    break;
                }
                case ModAnimation.AniType.Color:
                {
                    ModBase.MyColor myColor = ModBase.MyColor.Lerp(new ModBase.MyColor(0.0, 0.0, 0.0, 0.0), (ModBase.MyColor)Ani.Value, Ani.Ease.GetDelta(Ani.TimeFinished / (double)Ani.TimeTotal, Ani.TimePercent)) + (ModBase.MyColor)Ani.ValueLast;
                    FrameworkElement frameworkElement3 = (FrameworkElement)NewLateBinding.LateIndexGet(Ani.Obj, new object[] { 0 }, null);
                    DependencyProperty dependencyProperty = (DependencyProperty)NewLateBinding.LateIndexGet(Ani.Obj, new object[] { 1 }, null);
                    ModBase.MyColor myColor2 = new ModBase.MyColor(RuntimeHelpers.GetObjectValue(frameworkElement3.GetValue(dependencyProperty))) + myColor;
                    frameworkElement3.SetValue(dependencyProperty, RuntimeHelpers.GetObjectValue((Operators.CompareString(dependencyProperty.PropertyType.Name, "Color", false) == 0) ? myColor2 : myColor2));
                    Ani.ValueLast = myColor2 - new ModBase.MyColor(RuntimeHelpers.GetObjectValue(frameworkElement3.GetValue(dependencyProperty)));
                    break;
                }
                case ModAnimation.AniType.Scale:
                {
                    FrameworkElement frameworkElement4 = (FrameworkElement)Ani.Obj;
                    double delta = Ani.Ease.GetDelta(Ani.TimeFinished / (double)Ani.TimeTotal, Ani.TimePercent);
                    frameworkElement4.Margin = new Thickness(frameworkElement4.Margin.Left + MathUtils.Lerp(0.0, Conversions.ToDouble(NewLateBinding.LateGet(Ani.Value, null, "Left", new object[0], null, null, null)), delta), frameworkElement4.Margin.Top + MathUtils.Lerp(0.0, Conversions.ToDouble(NewLateBinding.LateGet(Ani.Value, null, "Top", new object[0], null, null, null)), delta), frameworkElement4.Margin.Right + MathUtils.Lerp(0.0, Conversions.ToDouble(NewLateBinding.LateGet(Ani.Value, null, "Left", new object[0], null, null, null)), delta), frameworkElement4.Margin.Bottom + MathUtils.Lerp(0.0, Conversions.ToDouble(NewLateBinding.LateGet(Ani.Value, null, "Top", new object[0], null, null, null)), delta));
                    frameworkElement4.Width = Math.Max(frameworkElement4.Width + MathUtils.Lerp(0.0, Conversions.ToDouble(NewLateBinding.LateGet(Ani.Value, null, "Width", new object[0], null, null, null)), delta), 0.0);
                    frameworkElement4.Height = Math.Max(frameworkElement4.Height + MathUtils.Lerp(0.0, Conversions.ToDouble(NewLateBinding.LateGet(Ani.Value, null, "Height", new object[0], null, null, null)), delta), 0.0);
                    break;
                }
                case ModAnimation.AniType.TextAppear:
                {
                    string text4 = Conversions.ToString(NewLateBinding.LateIndexGet(Ani.Value, new object[] { 0 }, null));
                    checked
                    {
                        int num6 = (int)Math.Round(unchecked((double)(Conversions.ToBoolean(NewLateBinding.LateIndexGet(Ani.Value, new object[] { 1 }, null)) ? text4.Length : 0) + Math.Round((double)(checked(text4.Length * (Conversions.ToBoolean(NewLateBinding.LateIndexGet(Ani.Value, new object[] { 1 }, null)) ? (-1) : 1))) * Ani.Ease.GetDelta(Ani.TimeFinished / (double)Ani.TimeTotal, 0.0))));
                        string text5 = text4.Substring(0, num6);
                        if (num6 < text4.Length)
                        {
                            if (Convert.ToInt32(Convert.ToChar(text4.Substring(num6, 1))) >= Convert.ToInt32(Convert.ToChar(128)))
                            {
                                text5 += Encoding.GetEncoding("GB18030").GetString(new byte[]
                                {
                                    (byte)ModBase.RandomInteger(176, 247),
                                    (byte)ModBase.RandomInteger(161, 249)
                                });
                            }
                            else
                            {
                                text5 += ModBase.RandomInteger(0, 85).ConvertRadix(86);
                            }
                        }
                        if (Ani.Obj is TextBlock)
                        {
                            NewLateBinding.LateSet(Ani.Obj, null, "Text", new object[] { text5 }, null, null);
                        }
                        else
                        {
                            NewLateBinding.LateSet(Ani.Obj, null, "Context", new object[] { text5 }, null, null);
                        }
                        break;
                    }
                }
                case ModAnimation.AniType.Code:
                    Ani.TimeFinished = (double)Ani.TimeTotal;
                    ((ThreadStart)Ani.Value)();
                    break;
                case ModAnimation.AniType.ScaleTransform:
                {
                    FrameworkElement frameworkElement5 = (FrameworkElement)Ani.Obj;
                    if (!(frameworkElement5.RenderTransform is ScaleTransform))
                    {
                        if (frameworkElement5.RenderTransformOrigin == new Point(0.0, 0.0))
                        {
                            frameworkElement5.RenderTransformOrigin = new Point(0.5, 0.5);
                        }
                        frameworkElement5.RenderTransform = new ScaleTransform(1.0, 1.0);
                    }
                    double num7 = MathUtils.Lerp(0.0, Conversions.ToDouble(Ani.Value), Ani.Ease.GetDelta(Ani.TimeFinished / (double)Ani.TimeTotal, Ani.TimePercent));
                    ((ScaleTransform)frameworkElement5.RenderTransform).ScaleX = Math.Max(((ScaleTransform)frameworkElement5.RenderTransform).ScaleX + num7, 0.0);
                    ((ScaleTransform)frameworkElement5.RenderTransform).ScaleY = Math.Max(((ScaleTransform)frameworkElement5.RenderTransform).ScaleY + num7, 0.0);
                    break;
                }
                case ModAnimation.AniType.RotateTransform:
                {
                    FrameworkElement frameworkElement6 = (FrameworkElement)Ani.Obj;
                    if (!(frameworkElement6.RenderTransform is RotateTransform))
                    {
                        if (frameworkElement6.RenderTransformOrigin == new Point(0.0, 0.0))
                        {
                            frameworkElement6.RenderTransformOrigin = new Point(0.5, 0.5);
                        }
                        frameworkElement6.RenderTransform = new RotateTransform(0.0);
                    }
                    double num8 = MathUtils.Lerp(0.0, Conversions.ToDouble(Ani.Value), Ani.Ease.GetDelta(Ani.TimeFinished / (double)Ani.TimeTotal, Ani.TimePercent));
                    ((RotateTransform)frameworkElement6.RenderTransform).Angle = ((RotateTransform)frameworkElement6.RenderTransform).Angle + num8;
                    break;
                }
                }
                Ani.TimePercent = Ani.TimeFinished / (double)Ani.TimeTotal;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, string.Format("执行动画失败：{0}", Ani), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModAnimation.vb");
            }
            return Ani;
        }

        public static Dictionary<string, ModAnimation.AniGroupEntry> AniGroups = new Dictionary<string, ModAnimation.AniGroupEntry>();

        private static long AniLastTick;

        public static bool AniRunning = false;

        private static int _AniControlEnabled = 0;

        private static readonly object AniControlEnabledLock = RuntimeHelpers.GetObjectValue(new object());

        private static int AniCount = 0;

        private static int AniFPSCounter = 0;

        private static long AniFPSTimer = 0L;

        public static int AniFPS = 0;

        public class AniGroupEntry
        {
            public AniGroupEntry()
            {
                this.Uuid = ModBase.GetUuid();
            }

            public List<ModAnimation.AniData> Data;

            public long StartTick;

            public int Uuid;
        }

        public struct AniData
        {
            public override string ToString()
            {
                return string.Concat(new string[]
                {
                    this.TypeMain.ToString(),
                    " | ",
                    Conversions.ToString(this.TimeFinished),
                    "/",
                    Conversions.ToString(this.TimeTotal),
                    "(",
                    Conversions.ToString(Math.Round(this.TimePercent * 100.0)),
                    "%)",
                    (this.Obj == null) ? "" : string.Concat(new string[]
                    {
                        " | ",
                        this.Obj.ToString(),
                        "(",
                        this.Obj.GetType().Name,
                        ")"
                    })
                });
            }

            public ModAnimation.AniType TypeMain;

            public ModAnimation.AniTypeSub TypeSub;

            public int TimeTotal;

            public double TimeFinished;

            public double TimePercent;

            public bool IsAfter;

            public ModAnimation.AniEase Ease;

            public object Obj;

            public object Value;

            public object ValueLast;
        }

        public enum AniType
        {
            Number,
            Color,
            Scale,
            TextAppear,
            Code,
            ScaleTransform,
            RotateTransform
        }

        public enum AniTypeSub
        {
            X,
            Y,
            Width,
            Height,
            Opacity,
            Value,
            Radius,
            BorderThickness,
            StrokeThickness,
            TranslateX,
            TranslateY,
            Double,
            DoubleParam,
            GridLengthWidth
        }

        public enum AniEasePower
        {
            Weak = 2,
            Middle,
            Strong,
            ExtraStrong
        }

        public abstract class AniEase
        {
            public abstract double GetValue(double t);

            public virtual double GetDelta(double t1, double t0)
            {
                return this.GetValue(t1) - this.GetValue(t0);
            }
        }

        public class AniEaseInout : ModAnimation.AniEase
        {
            public AniEaseInout(ModAnimation.AniEase EaseIn, ModAnimation.AniEase EaseOut, double EaseInPercent = 0.5)
            {
                this.EaseIn = EaseIn;
                this.EaseOut = EaseOut;
                this.EaseInPercent = EaseInPercent;
            }

            public override double GetValue(double t)
            {
                double num;
                if (t < this.EaseInPercent)
                {
                    num = this.EaseInPercent * this.EaseIn.GetValue(t / this.EaseInPercent);
                }
                else
                {
                    num = (1.0 - this.EaseInPercent) * this.EaseOut.GetValue((t - this.EaseInPercent) / (1.0 - this.EaseInPercent)) + this.EaseInPercent;
                }
                return num;
            }

            private readonly ModAnimation.AniEase EaseIn;

            private readonly ModAnimation.AniEase EaseOut;

            private readonly double EaseInPercent;
        }

        public class AniEaseLinear : ModAnimation.AniEase
        {
            public override double GetValue(double t)
            {
                return t.Clamp(0.0, 1.0);
            }

            public override double GetDelta(double t1, double t0)
            {
                return t1.Clamp(0.0, 1.0) - t0.Clamp(0.0, 1.0);
            }
        }

        public class AniEaseInFluent : ModAnimation.AniEase
        {
            public AniEaseInFluent(ModAnimation.AniEasePower Power = ModAnimation.AniEasePower.Middle)
            {
                this.p = Power;
            }

            public override double GetValue(double t)
            {
                return Math.Pow(t.Clamp(0.0, 1.0), (double)this.p);
            }

            private readonly ModAnimation.AniEasePower p;
        }

        public class AniEaseOutFluent : ModAnimation.AniEase
        {
            public AniEaseOutFluent(ModAnimation.AniEasePower Power = ModAnimation.AniEasePower.Middle)
            {
                this.p = Power;
            }

            public override double GetValue(double t)
            {
                return 1.0 - Math.Pow((1.0 - t).Clamp(0.0, 1.0), (double)this.p);
            }

            private readonly ModAnimation.AniEasePower p;
        }

        public class AniEaseInoutFluent : ModAnimation.AniEase
        {
            public AniEaseInoutFluent(ModAnimation.AniEasePower Power = ModAnimation.AniEasePower.Middle, double Middle = 0.5)
            {
                this.Ease = new ModAnimation.AniEaseInout(new ModAnimation.AniEaseInFluent(Power), new ModAnimation.AniEaseOutFluent(Power), Middle);
            }

            public override double GetValue(double t)
            {
                return this.Ease.GetValue(t);
            }

            private ModAnimation.AniEaseInout Ease;
        }

        public class AniEaseOutFluentWithInitial : ModAnimation.AniEase
        {
            public AniEaseOutFluentWithInitial(double InitialPixelPerSecond, double TotalSecond, double TotalDistance)
            {
                double num = InitialPixelPerSecond * TotalSecond / TotalDistance;
                this.alpha = num - 1.0;
                if (this.alpha < 0.0)
                {
                    this.alpha = 0.0;
                }
            }

            public override double GetValue(double percent)
            {
                double num = percent.Clamp(0.0, 1.0);
                double num2;
                if (this.alpha == 0.0)
                {
                    num2 = num;
                }
                else
                {
                    num2 = (this.alpha + 1.0) * num / (1.0 + this.alpha * num);
                }
                return num2;
            }

            private readonly double alpha;
        }

        public class AniEaseInBack : ModAnimation.AniEase
        {
            public AniEaseInBack(ModAnimation.AniEasePower Power = ModAnimation.AniEasePower.Middle)
            {
                this.p = 3.0 - (double)Power * 0.5;
            }

            public override double GetValue(double t)
            {
                t = t.Clamp(0.0, 1.0);
                return Math.Pow(t, this.p) * Math.Cos(4.71238898038469 * (1.0 - t));
            }

            private readonly double p;
        }

        public class AniEaseOutBack : ModAnimation.AniEase
        {
            public AniEaseOutBack(ModAnimation.AniEasePower Power = ModAnimation.AniEasePower.Middle)
            {
                this.p = 3.0 - (double)Power * 0.5;
            }

            public override double GetValue(double t)
            {
                t = t.Clamp(0.0, 1.0);
                return 1.0 - Math.Pow(1.0 - t, this.p) * Math.Cos(4.71238898038469 * t);
            }

            private readonly double p;
        }

        public class AniEaseInCar : ModAnimation.AniEase
        {
            public AniEaseInCar(double Middle = 0.7, ModAnimation.AniEasePower Power = ModAnimation.AniEasePower.Middle)
            {
                this.Ease = new ModAnimation.AniEaseInout(new ModAnimation.AniEaseInBack(Power), new ModAnimation.AniEaseOutFluent(Power), Middle);
            }

            public override double GetValue(double t)
            {
                return this.Ease.GetValue(t);
            }

            private ModAnimation.AniEaseInout Ease;
        }

        public class AniEaseOutCar : ModAnimation.AniEase
        {
            public AniEaseOutCar(double Middle = 0.3, ModAnimation.AniEasePower Power = ModAnimation.AniEasePower.Middle)
            {
                this.Ease = new ModAnimation.AniEaseInout(new ModAnimation.AniEaseInFluent(Power), new ModAnimation.AniEaseOutBack(Power), Middle);
            }

            public override double GetValue(double t)
            {
                return this.Ease.GetValue(t);
            }

            private ModAnimation.AniEaseInout Ease;
        }

        public class AniEaseInElastic : ModAnimation.AniEase
        {
            public AniEaseInElastic(ModAnimation.AniEasePower Power = ModAnimation.AniEasePower.Middle)
            {
                this.p = (int)(checked(Power + 4));
            }

            public override double GetValue(double t)
            {
                t = t.Clamp(0.0, 1.0);
                return Math.Pow(t, (double)(checked(this.p - 1)) * 0.25) * Math.Cos(((double)this.p - 3.5) * 3.141592653589793 * Math.Pow(1.0 - t, 1.5));
            }

            private readonly int p;
        }

        public class AniEaseOutElastic : ModAnimation.AniEase
        {
            public AniEaseOutElastic(ModAnimation.AniEasePower Power = ModAnimation.AniEasePower.Middle)
            {
                this.p = (int)(checked(Power + 4));
            }

            public override double GetValue(double t)
            {
                t = 1.0 - t.Clamp(0.0, 1.0);
                return 1.0 - Math.Pow(t, (double)(checked(this.p - 1)) * 0.25) * Math.Cos(((double)this.p - 3.5) * 3.141592653589793 * Math.Pow(1.0 - t, 1.5));
            }

            private readonly int p;
        }
    }
}
