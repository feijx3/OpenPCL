using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class MyPageRight : AdornerDecorator
    {
        public MyPageRight()
        {
            this.PageUuid = ModBase.GetUuid();
            this._PageState = MyPageRight.PageStates.Empty;
            this.DisabledPageAnimControls = new List<FrameworkElement>();
        }

        public string PanScroll
        {
            get
            {
                return Conversions.ToString(base.GetValue((DependencyProperty)MyPageRight.PanScrollProperty));
            }
            set
            {
                object[] array;
                bool[] array2;
                NewLateBinding.LateCall(this, null, "SetValue", array = new object[]
                {
                    MyPageRight.PanScrollProperty,
                    value
                }, null, null, array2 = new bool[]
                {
                    default(bool),
                    true
                }, true);
                if (array2[1])
                {
                    value = (string)Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array[1]), typeof(string));
                }
            }
        }

        public MyPageRight.PageStates PageState
        {
            get
            {
                return this._PageState;
            }
            set
            {
                if (this._PageState != value)
                {
                    this._PageState = value;
                    Logger.Trace(string.Format("页面状态切换为 {0}", value), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyPageRight.vb");
                }
            }
        }

        public void PageLoaderInit<TIn, TOut>(MyLoading LoaderUi, FrameworkElement PanLoader, FrameworkElement PanContent, FrameworkElement PanAlways, ModLoader.LoaderTask<TIn, TOut> RealLoader, Action<ModLoader.LoaderTask<TIn, TOut>> FinishedInvoke = null, Func<TIn> InputInvoke = null, bool AutoRun = true)
        {
            MyPageRight._Closure$__19-0<TIn, TOut> CS$<>8__locals1 = new MyPageRight._Closure$__19-0<TIn, TOut>(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Me = this;
            CS$<>8__locals1.$VB$Local_RealLoader = RealLoader;
            CS$<>8__locals1.$VB$Local_FinishedInvoke = FinishedInvoke;
            this.PanLoader = PanLoader;
            this.PanContent = PanContent;
            this.PanAlways = PanAlways;
            this.PageLoader = CS$<>8__locals1.$VB$Local_RealLoader;
            this.PageLoaderUi = LoaderUi;
            this.PageLoaderInputInvoke = InputInvoke;
            this.PageLoaderAutoRun = AutoRun;
            if (CS$<>8__locals1.$VB$Local_FinishedInvoke != null)
            {
                CS$<>8__locals1.$VB$Local_RealLoader.PreviewFinish += delegate(ModLoader.LoaderBase a0)
                {
                    base._Lambda$__0();
                };
            }
            CS$<>8__locals1.$VB$Local_RealLoader.OnStateChangedUi += delegate(ModLoader.LoaderBase Loader, ModBase.LoadState NewState, ModBase.LoadState OldState)
            {
                MyPageRight._Closure$__19-1<$CLS0, $CLS1> CS$<>8__locals2 = new MyPageRight._Closure$__19-1<$CLS0, $CLS1>(CS$<>8__locals2);
                CS$<>8__locals2.$VB$Me = this;
                CS$<>8__locals2.$VB$Local_Loader = Loader;
                CS$<>8__locals2.$VB$Local_NewState = NewState;
                CS$<>8__locals2.$VB$Local_OldState = OldState;
                ModBase.RunInUi(delegate
                {
                    CS$<>8__locals2.$VB$Me.PageLoaderState(CS$<>8__locals2.$VB$Local_Loader, CS$<>8__locals2.$VB$Local_NewState, CS$<>8__locals2.$VB$Local_OldState);
                }, false);
            };
            PanLoader.Visibility = Visibility.Collapsed;
            PanContent.Visibility = Visibility.Collapsed;
            if (PanAlways != null)
            {
                PanAlways.Visibility = Visibility.Collapsed;
            }
            if (this.PageLoaderAutoRun)
            {
                this.PageLoader.Start(CS$<>8__locals1.$VB$Local_RealLoader.StartGetInput(default(TIn), (Func<TIn>)this.PageLoaderInputInvoke), false);
            }
            if (this.PageLoader.State == ModBase.LoadState.Finished && CS$<>8__locals1.$VB$Local_FinishedInvoke != null)
            {
                ModBase.RunInUiWait(delegate
                {
                    CS$<>8__locals1.$VB$Local_FinishedInvoke(CS$<>8__locals1.$VB$Local_RealLoader);
                });
            }
            this.PageLoaderUi.State = CS$<>8__locals1.$VB$Local_RealLoader;
            this.PageLoaderUi.Click += delegate(object a0, MouseButtonEventArgs a1)
            {
                base._Lambda$__5();
            };
        }

        public void PageLoaderInit<TIn>(MyLoading LoaderUi, FrameworkElement PanLoader, FrameworkElement PanContent, FrameworkElement PanAlways, ModLoader.LoaderCombo<TIn> RealLoader, Action<ModLoader.LoaderCombo<TIn>> FinishedInvoke = null, Func<TIn> InputInvoke = null, bool AutoRun = true)
        {
            MyPageRight._Closure$__20-0<TIn> CS$<>8__locals1 = new MyPageRight._Closure$__20-0<TIn>(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Me = this;
            CS$<>8__locals1.$VB$Local_RealLoader = RealLoader;
            CS$<>8__locals1.$VB$Local_FinishedInvoke = FinishedInvoke;
            this.PanLoader = PanLoader;
            this.PanContent = PanContent;
            this.PanAlways = PanAlways;
            this.PageLoader = CS$<>8__locals1.$VB$Local_RealLoader;
            this.PageLoaderUi = LoaderUi;
            this.PageLoaderInputInvoke = InputInvoke;
            this.PageLoaderAutoRun = AutoRun;
            if (CS$<>8__locals1.$VB$Local_FinishedInvoke != null)
            {
                CS$<>8__locals1.$VB$Local_RealLoader.PreviewFinish += delegate(ModLoader.LoaderBase a0)
                {
                    base._Lambda$__0();
                };
            }
            CS$<>8__locals1.$VB$Local_RealLoader.OnStateChangedUi += delegate(ModLoader.LoaderBase Loader, ModBase.LoadState NewState, ModBase.LoadState OldState)
            {
                MyPageRight._Closure$__20-1<$CLS0> CS$<>8__locals2 = new MyPageRight._Closure$__20-1<$CLS0>(CS$<>8__locals2);
                CS$<>8__locals2.$VB$Me = this;
                CS$<>8__locals2.$VB$Local_Loader = Loader;
                CS$<>8__locals2.$VB$Local_NewState = NewState;
                CS$<>8__locals2.$VB$Local_OldState = OldState;
                ModBase.RunInUi(delegate
                {
                    CS$<>8__locals2.$VB$Me.PageLoaderState(CS$<>8__locals2.$VB$Local_Loader, CS$<>8__locals2.$VB$Local_NewState, CS$<>8__locals2.$VB$Local_OldState);
                }, false);
            };
            PanLoader.Visibility = Visibility.Collapsed;
            PanContent.Visibility = Visibility.Collapsed;
            if (PanAlways != null)
            {
                PanAlways.Visibility = Visibility.Collapsed;
            }
            if (this.PageLoaderAutoRun)
            {
                object obj = null;
                if (this.PageLoaderInputInvoke != null)
                {
                    obj = RuntimeHelpers.GetObjectValue(NewLateBinding.LateIndexGet(this.PageLoaderInputInvoke, new object[0], null));
                }
                this.PageLoader.Start(RuntimeHelpers.GetObjectValue(obj), false);
            }
            if (this.PageLoader.State == ModBase.LoadState.Finished && CS$<>8__locals1.$VB$Local_FinishedInvoke != null)
            {
                ModBase.RunInUiWait(delegate
                {
                    CS$<>8__locals1.$VB$Local_FinishedInvoke(CS$<>8__locals1.$VB$Local_RealLoader);
                });
            }
            this.PageLoaderUi.State = CS$<>8__locals1.$VB$Local_RealLoader;
            this.PageLoaderUi.Click += delegate(object a0, MouseButtonEventArgs a1)
            {
                base._Lambda$__5();
            };
        }

        public void PageLoaderRestart(object Input = null, bool IsForceRestart = true)
        {
            if (this.PageLoaderAutoRun)
            {
                if (GeneralUtils.IsGenericInstanceOf(this.PageLoader, typeof(ModLoader.LoaderTask<, >)) || this.PageLoader is ModLoader.LoaderWorker)
                {
                    ModLoader.LoaderBase pageLoader = this.PageLoader;
                    object pageLoader2 = this.PageLoader;
                    Type type = null;
                    string text = "StartGetInput";
                    object[] array = new object[2];
                    array[0] = Input;
                    int num = 1;
                    ref object ptr = ref this.PageLoaderInputInvoke;
                    array[num] = this.PageLoaderInputInvoke;
                    object[] array2 = array;
                    bool[] array3;
                    object obj = NewLateBinding.LateGet(pageLoader2, type, text, array, null, null, array3 = new bool[] { true, true });
                    if (array3[0])
                    {
                        Input = RuntimeHelpers.GetObjectValue(array2[0]);
                    }
                    if (array3[1])
                    {
                        ptr = RuntimeHelpers.GetObjectValue(array2[1]);
                    }
                    pageLoader.Start(RuntimeHelpers.GetObjectValue(obj), IsForceRestart);
                    return;
                }
                if (Input == null && this.PageLoaderInputInvoke != null)
                {
                    Input = RuntimeHelpers.GetObjectValue(NewLateBinding.LateIndexGet(this.PageLoaderInputInvoke, new object[0], null));
                }
                this.PageLoader.Start(RuntimeHelpers.GetObjectValue(Input), IsForceRestart);
            }
        }

        public void PageOnEnter()
        {
            Logger.Trace("已触发 PageOnEnter", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyPageRight.vb");
            MyPageRight.PageEnterEventHandler pageEnterEvent = this.PageEnterEvent;
            if (pageEnterEvent != null)
            {
                pageEnterEvent();
            }
            MyPageRight.PageStates pageState = this.PageState;
            if (pageState != MyPageRight.PageStates.Empty)
            {
                if (pageState == MyPageRight.PageStates.ContentEnter)
                {
                    return;
                }
                if (pageState != MyPageRight.PageStates.ContentExit)
                {
                    throw new Exception("在状态为 " + this.PageState.ToString() + " 时触发了 PageOnEnter 事件。");
                }
                if (this.PageLoader == null || this.PageLoader.State == ModBase.LoadState.Finished || this.PageLoader.State == ModBase.LoadState.Waiting || this.PageLoader.State == ModBase.LoadState.Canceled)
                {
                    this.PageState = MyPageRight.PageStates.ContentEnter;
                    this.TriggerEnterAnimation(new FrameworkElement[] { (FrameworkElement)(this.PanContent ?? this.Child) });
                    return;
                }
                if (this.PageLoader.State == ModBase.LoadState.Loading)
                {
                    this.PageState = MyPageRight.PageStates.LoaderWait;
                    ModAnimation.AniStart(ModAnimation.AaCode(new ThreadStart(this.PageOnLoaderWaitFinished), 400, false), "PageRight PageChange " + Conversions.ToString(this.PageUuid), false);
                    return;
                }
                this.PageState = MyPageRight.PageStates.LoaderEnter;
                this.TriggerEnterAnimation(new FrameworkElement[] { this.PanLoader });
                return;
            }
            else
            {
                if (this.PageLoader == null || this.PageLoader.State == ModBase.LoadState.Finished || this.PageLoader.State == ModBase.LoadState.Waiting || this.PageLoader.State == ModBase.LoadState.Canceled)
                {
                    this.PageState = MyPageRight.PageStates.ContentEnter;
                    this.TriggerEnterAnimation(new FrameworkElement[]
                    {
                        this.PanAlways,
                        (FrameworkElement)(this.PanContent ?? this.Child)
                    });
                    return;
                }
                if (this.PageLoader.State == ModBase.LoadState.Loading)
                {
                    this.PageState = MyPageRight.PageStates.LoaderWait;
                    ModAnimation.AniStart(ModAnimation.AaCode(new ThreadStart(this.PageOnLoaderWaitFinished), 400, false), "PageRight PageChange " + Conversions.ToString(this.PageUuid), false);
                    return;
                }
                this.PageState = MyPageRight.PageStates.LoaderEnter;
                this.TriggerEnterAnimation(new FrameworkElement[] { this.PanAlways, this.PanLoader });
                return;
            }
        }

        public event MyPageRight.PageEnterEventHandler PageEnter;

        public void PageOnExit()
        {
            Logger.Trace("已触发 PageOnExit", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyPageRight.vb");
            MyPageRight.PageExitEventHandler pageExitEvent = this.PageExitEvent;
            if (pageExitEvent != null)
            {
                pageExitEvent();
            }
            switch (this.PageState)
            {
            case MyPageRight.PageStates.Empty:
            case MyPageRight.PageStates.PageExit:
                break;
            case MyPageRight.PageStates.LoaderWait:
                this.PageState = MyPageRight.PageStates.PageExit;
                this.TriggerExitAnimation(new FrameworkElement[] { this.PanAlways });
                return;
            case MyPageRight.PageStates.LoaderEnter:
            case MyPageRight.PageStates.LoaderStayForce:
            case MyPageRight.PageStates.LoaderStay:
                this.PageState = MyPageRight.PageStates.PageExit;
                this.TriggerExitAnimation(new FrameworkElement[] { this.PanAlways, this.PanLoader });
                return;
            case MyPageRight.PageStates.LoaderExit:
            case MyPageRight.PageStates.ContentExit:
                this.PageState = MyPageRight.PageStates.PageExit;
                if (this.PanAlways != null)
                {
                    this.TriggerExitAnimation(new FrameworkElement[]
                    {
                        this.PanAlways,
                        (FrameworkElement)(this.PanContent ?? this.Child)
                    });
                }
                break;
            case MyPageRight.PageStates.ContentEnter:
            case MyPageRight.PageStates.ContentStay:
                this.PageState = MyPageRight.PageStates.PageExit;
                this.TriggerExitAnimation(new FrameworkElement[]
                {
                    this.PanAlways,
                    (FrameworkElement)(this.PanContent ?? this.Child)
                });
                return;
            default:
                return;
            }
        }

        public event MyPageRight.PageExitEventHandler PageExit;

        public void PageOnForceExit()
        {
            if (this.PageState != MyPageRight.PageStates.Empty)
            {
                Logger.Trace("已触发 PageOnForceExit", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyPageRight.vb");
                this.PageState = MyPageRight.PageStates.Empty;
                ModAnimation.AniStop("PageRight PageChange " + Conversions.ToString(this.PageUuid));
                if (this.PageLoader == null)
                {
                    this.Child.Visibility = Visibility.Collapsed;
                    return;
                }
                this.PanContent.Visibility = Visibility.Collapsed;
                this.PanLoader.Visibility = Visibility.Collapsed;
                if (this.PanAlways != null)
                {
                    this.PanAlways.Visibility = Visibility.Collapsed;
                }
            }
        }

        public void PageOnContentExit()
        {
            Logger.Trace("已触发 PageOnContentExit", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyPageRight.vb");
            if (this.PageLoader != null && this.PageLoader.State == ModBase.LoadState.Loading)
            {
                throw new Exception("在调用 PageOnContentExit 时，加载器不能为 Loading 状态");
            }
            switch (this.PageState)
            {
            case MyPageRight.PageStates.Empty:
                if (base.IsLoaded)
                {
                    this.PageOnEnter();
                }
                return;
            case MyPageRight.PageStates.LoaderWait:
                this.PageOnEnter();
                return;
            case MyPageRight.PageStates.LoaderEnter:
            case MyPageRight.PageStates.LoaderStayForce:
            case MyPageRight.PageStates.LoaderStay:
                this.PageState = MyPageRight.PageStates.ContentExit;
                this.TriggerExitAnimation(new FrameworkElement[] { this.PanLoader });
                return;
            case MyPageRight.PageStates.LoaderExit:
                this.PageState = MyPageRight.PageStates.ContentExit;
                return;
            case MyPageRight.PageStates.ContentEnter:
            case MyPageRight.PageStates.ContentStay:
                this.PageState = MyPageRight.PageStates.ContentExit;
                this.TriggerExitAnimation(new FrameworkElement[] { this.PanContent });
                return;
            default:
                return;
            }
        }

        private void PageOnEnterAnimationFinished()
        {
            Logger.Trace("已触发 PageOnEnterAnimationFinished", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyPageRight.vb");
            MyPageRight.PageStates pageState = this.PageState;
            if (pageState == MyPageRight.PageStates.LoaderEnter)
            {
                this.PageState = MyPageRight.PageStates.LoaderStayForce;
                ModAnimation.AniStart(ModAnimation.AaCode(new ThreadStart(this.PageOnLoaderStayFinished), 400, false), "PageRight PageChange " + Conversions.ToString(this.PageUuid), false);
                return;
            }
            if (pageState == MyPageRight.PageStates.ContentEnter)
            {
                this.PageState = MyPageRight.PageStates.ContentStay;
                return;
            }
            throw new Exception("在状态为 " + this.PageState.ToString() + " 时触发了 PageOnEnterAnimationFinished 事件。");
        }

        private void PageOnExitAnimationFinished()
        {
            Logger.Trace("已触发 PageOnExitAnimationFinished", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyPageRight.vb");
            switch (this.PageState)
            {
            case MyPageRight.PageStates.LoaderExit:
                this.PageState = MyPageRight.PageStates.ContentEnter;
                this.TriggerEnterAnimation(new FrameworkElement[] { this.PanContent });
                return;
            case MyPageRight.PageStates.ContentExit:
                this.PageOnEnter();
                return;
            case MyPageRight.PageStates.PageExit:
                this.PageState = MyPageRight.PageStates.Empty;
                return;
            }
            throw new Exception("在状态为 " + this.PageState.ToString() + " 时触发了 PageOnExitAnimationFinished 事件。");
        }

        private void PageOnLoaderWaitFinished()
        {
            Logger.Trace("已触发 PageOnLoaderWaitFinished", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyPageRight.vb");
            MyPageRight.PageStates pageState = this.PageState;
            if (pageState != MyPageRight.PageStates.LoaderWait)
            {
                throw new Exception("在状态为 " + this.PageState.ToString() + " 时触发了 PageOnLoaderWaitFinished 事件。");
            }
            this.PageState = MyPageRight.PageStates.LoaderEnter;
            if (this.PanAlways != null && this.PanAlways.Visibility == Visibility.Collapsed)
            {
                this.TriggerEnterAnimation(new FrameworkElement[] { this.PanAlways, this.PanLoader });
                return;
            }
            this.TriggerEnterAnimation(new FrameworkElement[] { this.PanLoader });
        }

        private void PageOnLoaderStayFinished()
        {
            Logger.Trace("已触发 PageOnLoaderStayFinished", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyPageRight.vb");
            MyPageRight.PageStates pageState = this.PageState;
            if (pageState != MyPageRight.PageStates.LoaderStayForce)
            {
                throw new Exception("在状态为 " + this.PageState.ToString() + " 时触发了 PageOnLoaderWaitFinished 事件。");
            }
            if (this.PageLoader.State == ModBase.LoadState.Finished)
            {
                this.PageState = MyPageRight.PageStates.LoaderExit;
                this.TriggerExitAnimation(new FrameworkElement[] { this.PanLoader });
                return;
            }
            this.PageState = MyPageRight.PageStates.LoaderStay;
        }

        private void PageLoaderState(object sender, ModBase.LoadState NewState, ModBase.LoadState OldState)
        {
            switch (NewState)
            {
            case ModBase.LoadState.Waiting:
            case ModBase.LoadState.Finished:
            case ModBase.LoadState.Canceled:
                if (OldState == ModBase.LoadState.Failed || OldState == ModBase.LoadState.Loading)
                {
                    Logger.Trace("已触发 PageLoaderState (Stop/Canceled)", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyPageRight.vb");
                    MyPageRight.PageStates pageState = this.PageState;
                    if (pageState != MyPageRight.PageStates.LoaderWait)
                    {
                        if (pageState != MyPageRight.PageStates.LoaderStay)
                        {
                            return;
                        }
                        this.PageState = MyPageRight.PageStates.LoaderExit;
                        this.TriggerExitAnimation(new FrameworkElement[] { this.PanLoader });
                    }
                    else
                    {
                        this.PageState = MyPageRight.PageStates.ContentEnter;
                        if (this.PanAlways != null && this.PanAlways.Visibility == Visibility.Collapsed)
                        {
                            this.TriggerEnterAnimation(new FrameworkElement[] { this.PanAlways, this.PanContent });
                            return;
                        }
                        this.TriggerEnterAnimation(new FrameworkElement[] { this.PanContent });
                        return;
                    }
                }
                break;
            case ModBase.LoadState.Loading:
            case ModBase.LoadState.Failed:
                if (OldState != ModBase.LoadState.Failed && OldState != ModBase.LoadState.Loading)
                {
                    Logger.Trace("已触发 PageLoaderState (Start/Refresh)", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyPageRight.vb");
                    MyPageRight.PageStates pageState2 = this.PageState;
                    if (pageState2 == MyPageRight.PageStates.LoaderExit)
                    {
                        this.PageState = MyPageRight.PageStates.ContentExit;
                        return;
                    }
                    if (pageState2 - MyPageRight.PageStates.ContentEnter <= 1)
                    {
                        this.PageState = MyPageRight.PageStates.ContentExit;
                        this.TriggerExitAnimation(new FrameworkElement[] { this.PanContent });
                        return;
                    }
                }
                break;
            default:
                return;
            }
        }

        public void TriggerEnterAnimation(params FrameworkElement[] Elements)
        {
            IEnumerable<FrameworkElement> enumerable = Elements.Where<FrameworkElement>((MyPageRight._Closure$__.$I41-0 == null) ? (MyPageRight._Closure$__.$I41-0 = (FrameworkElement e) => e != null) : MyPageRight._Closure$__.$I41-0);
            try
            {
                foreach (FrameworkElement frameworkElement in enumerable)
                {
                    frameworkElement.Visibility = Visibility.Visible;
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
            List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
            int num = 0;
            MyScrollBar firstScrollViewer;
            checked
            {
                try
                {
                    foreach (FrameworkElement frameworkElement2 in enumerable)
                    {
                        try
                        {
                            foreach (FrameworkElement frameworkElement3 in this.GetAllAnimControls(frameworkElement2, true))
                            {
                                frameworkElement3.IsHitTestVisible = true;
                                if (frameworkElement3.RenderTransform != null && frameworkElement3.RenderTransform is TranslateTransform)
                                {
                                    frameworkElement3.RenderTransform = null;
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator<FrameworkElement> enumerator3;
                            if (enumerator3 != null)
                            {
                                enumerator3.Dispose();
                            }
                        }
                        try
                        {
                            foreach (FrameworkElement frameworkElement4 in this.GetAllAnimControls(frameworkElement2, false))
                            {
                                if (frameworkElement4 is MyExtraTextButton)
                                {
                                    ((MyExtraTextButton)frameworkElement4).Show = true;
                                }
                                else
                                {
                                    frameworkElement4.Opacity = 0.0;
                                    frameworkElement4.RenderTransform = new TranslateTransform(0.0, -16.0);
                                    list.Add(ModAnimation.AaOpacity(frameworkElement4, 1.0, 100, num, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false));
                                    list.Add(ModAnimation.AaTranslateY(frameworkElement4, 5.0, 250, num, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false));
                                    list.Add(ModAnimation.AaTranslateY(frameworkElement4, 11.0, 350, num, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false));
                                    num += 25;
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator<FrameworkElement> enumerator4;
                            if (enumerator4 != null)
                            {
                                enumerator4.Dispose();
                            }
                        }
                    }
                }
                finally
                {
                    IEnumerator<FrameworkElement> enumerator2;
                    if (enumerator2 != null)
                    {
                        enumerator2.Dispose();
                    }
                }
                firstScrollViewer = this.GetFirstScrollViewer(enumerable);
            }
            if (firstScrollViewer != null)
            {
                if (!(firstScrollViewer.RenderTransform is TranslateTransform))
                {
                    firstScrollViewer.RenderTransform = new TranslateTransform(10.0, 0.0);
                }
                list.Add(ModAnimation.AaTranslateX(firstScrollViewer, -((TranslateTransform)firstScrollViewer.RenderTransform).X, 350, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false));
            }
            list.Add(ModAnimation.AaCode(delegate
            {
                this.PageOnEnterAnimationFinished();
            }, 0, true));
            ModAnimation.AniStart(list, "PageRight PageChange " + Conversions.ToString(this.PageUuid), true);
        }

        public void TriggerExitAnimation(params FrameworkElement[] Elements)
        {
            MyPageRight._Closure$__42-0 CS$<>8__locals1 = new MyPageRight._Closure$__42-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Me = this;
            CS$<>8__locals1.$VB$Local_RealElements = Elements.Where<FrameworkElement>((MyPageRight._Closure$__.$I42-0 == null) ? (MyPageRight._Closure$__.$I42-0 = (FrameworkElement e) => e != null) : MyPageRight._Closure$__.$I42-0);
            List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
            int num = 0;
            MyScrollBar firstScrollViewer;
            checked
            {
                try
                {
                    foreach (FrameworkElement frameworkElement in CS$<>8__locals1.$VB$Local_RealElements)
                    {
                        try
                        {
                            foreach (FrameworkElement frameworkElement2 in this.GetAllAnimControls(frameworkElement, false))
                            {
                                if (frameworkElement2 is MyExtraTextButton)
                                {
                                    ((MyExtraTextButton)frameworkElement2).Show = false;
                                }
                                else
                                {
                                    frameworkElement2.IsHitTestVisible = false;
                                    list.Add(ModAnimation.AaOpacity(frameworkElement2, -1.0, 70, num, null, false));
                                    list.Add(ModAnimation.AaTranslateY(frameworkElement2, -6.0, 70, num, null, false));
                                    num += 15;
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator<FrameworkElement> enumerator2;
                            if (enumerator2 != null)
                            {
                                enumerator2.Dispose();
                            }
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
                firstScrollViewer = this.GetFirstScrollViewer(CS$<>8__locals1.$VB$Local_RealElements);
            }
            if (firstScrollViewer != null)
            {
                if (!(firstScrollViewer.RenderTransform is TranslateTransform))
                {
                    firstScrollViewer.RenderTransform = new TranslateTransform();
                }
                list.Add(ModAnimation.AaTranslateX(firstScrollViewer, 10.0 - ((TranslateTransform)firstScrollViewer.RenderTransform).X, 90, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false));
            }
            list.Add(ModAnimation.AaCode(delegate
            {
                try
                {
                    foreach (FrameworkElement frameworkElement3 in CS$<>8__locals1.$VB$Local_RealElements)
                    {
                        frameworkElement3.Visibility = Visibility.Collapsed;
                    }
                }
                finally
                {
                    IEnumerator<FrameworkElement> enumerator3;
                    if (enumerator3 != null)
                    {
                        enumerator3.Dispose();
                    }
                }
                CS$<>8__locals1.$VB$Me.PageOnExitAnimationFinished();
            }, 0, true));
            ModAnimation.AniStart(list, "PageRight PageChange " + Conversions.ToString(this.PageUuid), false);
        }

        internal IEnumerable<FrameworkElement> GetAllAnimControls(FrameworkElement Element, bool IgnoreInvisibility = false)
        {
            List<FrameworkElement> list = new List<FrameworkElement>();
            this._GetAllAnimControls(Element, ref list, IgnoreInvisibility);
            return list.Except<FrameworkElement>(this.DisabledPageAnimControls);
        }

        private void _GetAllAnimControls(FrameworkElement Element, ref List<FrameworkElement> AllControls, bool IgnoreInvisibility)
        {
            if (IgnoreInvisibility || Element.Visibility != Visibility.Collapsed)
            {
                if (Element is MyCard || Element is MyHint || Element is MyExtraTextButton || Element is TextBlock || Element is MyTextButton)
                {
                    AllControls.Add(Element);
                    return;
                }
                if (Element is ContentControl)
                {
                    object objectValue = RuntimeHelpers.GetObjectValue(((ContentControl)Element).Content);
                    if (objectValue != null && objectValue is FrameworkElement)
                    {
                        this._GetAllAnimControls((FrameworkElement)objectValue, ref AllControls, IgnoreInvisibility);
                        return;
                    }
                }
                else if (Element is Panel)
                {
                    try
                    {
                        foreach (object obj in ((Panel)Element).Children)
                        {
                            object objectValue2 = RuntimeHelpers.GetObjectValue(obj);
                            if (objectValue2 is FrameworkElement)
                            {
                                this._GetAllAnimControls((FrameworkElement)objectValue2, ref AllControls, IgnoreInvisibility);
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
                }
            }
        }

        private MyScrollBar GetFirstScrollViewer(IEnumerable<FrameworkElement> Elements)
        {
            MyScrollViewer myScrollViewer = null;
            try
            {
                foreach (FrameworkElement frameworkElement in Elements)
                {
                    if (frameworkElement is MyScrollViewer)
                    {
                        myScrollViewer = (MyScrollViewer)frameworkElement;
                        goto IL_008D;
                    }
                    try
                    {
                        foreach (object obj in LogicalTreeHelper.GetChildren(frameworkElement))
                        {
                            object objectValue = RuntimeHelpers.GetObjectValue(obj);
                            if (objectValue is MyScrollViewer)
                            {
                                myScrollViewer = (MyScrollViewer)objectValue;
                                goto IL_008D;
                            }
                        }
                    }
                    finally
                    {
                        IEnumerator enumerator2;
                        if (enumerator2 is IDisposable)
                        {
                            (enumerator2 as IDisposable).Dispose();
                        }
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
            return null;
            IL_008D:
            MyScrollBar myScrollBar;
            if (myScrollViewer.ComputedVerticalScrollBarVisibility != Visibility.Visible)
            {
                myScrollBar = null;
            }
            else
            {
                myScrollBar = myScrollViewer.ScrollBar;
            }
            return myScrollBar;
        }

        public int PageUuid;

        private static readonly object PanScrollProperty = DependencyProperty.Register("PanScroll", typeof(string), typeof(MyPageRight), new PropertyMetadata(null));

        private MyPageRight.PageStates _PageState;

        private ModLoader.LoaderBase PageLoader;

        private object PageLoaderInputInvoke;

        private MyLoading PageLoaderUi;

        private FrameworkElement PanLoader;

        private FrameworkElement PanContent;

        private FrameworkElement PanAlways;

        private bool PageLoaderAutoRun;

        public List<FrameworkElement> DisabledPageAnimControls;

        public enum PageStates
        {
            Empty,
            LoaderWait,
            LoaderEnter,
            LoaderStayForce,
            LoaderStay,
            LoaderExit,
            ContentEnter,
            ContentStay,
            ContentExit,
            PageExit
        }

        public delegate void PageEnterEventHandler();

        public delegate void PageExitEventHandler();
    }
}
