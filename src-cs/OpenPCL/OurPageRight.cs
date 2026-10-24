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
    public class OurPageRight : AdornerDecorator
    {
        // Note: this type is marked as 'beforefieldinit'.
        static OurPageRight()
        {
        }

        public OurPageRight()
        {
            this.PageUuid = ModBase.GetUuid();
            this._PageState = OurPageRight.PageStates.Empty;
            this.DisabledPageAnimControls = new List<FrameworkElement>();
        }

        public string PanScroll
        {
            get
            {
                return Conversions.ToString(base.GetValue((DependencyProperty)OurPageRight.PanScrollProperty));
            }
            set
            {
                object[] array;
                bool[] array2;
                NewLateBinding.LateCall(this, null, "SetValue", array = new object[]
                {
                    OurPageRight.PanScrollProperty,
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

        public OurPageRight.PageStates PageState
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
                    Logger.Trace(string.Format("页面状态切换为 {0}", value), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurPageRight.vb");
                }
            }
        }

        public void PageLoaderInit<TIn, TOut>(OurLoading LoaderUi, FrameworkElement PanLoader, FrameworkElement PanContent, FrameworkElement PanAlways, ModLoader.LoaderTask<TIn, TOut> RealLoader, Action<ModLoader.LoaderTask<TIn, TOut>> FinishedInvoke = null, Func<TIn> InputInvoke = null, bool AutoRun = true)
        {
            OurPageRight._Closure___19_0<TIn, TOut> CS_locals1 = new OurPageRight._Closure___19_0<TIn, TOut>(null);
            CS_locals1._VB_Me = this;
            CS_locals1._VB_Local_RealLoader = RealLoader;
            CS_locals1._VB_Local_FinishedInvoke = FinishedInvoke;
            this.PanLoader = PanLoader;
            this.PanContent = PanContent;
            this.PanAlways = PanAlways;
            this.PageLoader = CS_locals1._VB_Local_RealLoader;
            this.PageLoaderUi = LoaderUi;
            this.PageLoaderInputInvoke = InputInvoke;
            this.PageLoaderAutoRun = AutoRun;
            if (CS_locals1._VB_Local_FinishedInvoke != null)
            {
                CS_locals1._VB_Local_RealLoader.PreviewFinish += CS_locals1._Lambda___R1;
            }
            CS_locals1._VB_Local_RealLoader.OnStateChangedUi += this._Lambda___19_2<TIn, TOut>;
            PanLoader.Visibility = Visibility.Collapsed;
            PanContent.Visibility = Visibility.Collapsed;
            if (PanAlways != null)
            {
                PanAlways.Visibility = Visibility.Collapsed;
            }
            if (this.PageLoaderAutoRun)
            {
                this.PageLoader.Start(CS_locals1._VB_Local_RealLoader.StartGetInput(default(TIn), (Func<TIn>)this.PageLoaderInputInvoke), false);
            }
            if (this.PageLoader.State == ModBase.LoadState.Finished && CS_locals1._VB_Local_FinishedInvoke != null)
            {
                ModBase.RunInUiWait(new Action(CS_locals1._Lambda___4));
            }
            this.PageLoaderUi.State = CS_locals1._VB_Local_RealLoader;
            this.PageLoaderUi.Click += CS_locals1._Lambda___R2;
        }

        public void PageLoaderInit<TIn>(OurLoading LoaderUi, FrameworkElement PanLoader, FrameworkElement PanContent, FrameworkElement PanAlways, ModLoader.LoaderCombo<TIn> RealLoader, Action<ModLoader.LoaderCombo<TIn>> FinishedInvoke = null, Func<TIn> InputInvoke = null, bool AutoRun = true)
        {
            OurPageRight._Closure___20_0<TIn> CS_locals1 = new OurPageRight._Closure___20_0<TIn>(null);
            CS_locals1._VB_Me = this;
            CS_locals1._VB_Local_RealLoader = RealLoader;
            CS_locals1._VB_Local_FinishedInvoke = FinishedInvoke;
            this.PanLoader = PanLoader;
            this.PanContent = PanContent;
            this.PanAlways = PanAlways;
            this.PageLoader = CS_locals1._VB_Local_RealLoader;
            this.PageLoaderUi = LoaderUi;
            this.PageLoaderInputInvoke = InputInvoke;
            this.PageLoaderAutoRun = AutoRun;
            if (CS_locals1._VB_Local_FinishedInvoke != null)
            {
                CS_locals1._VB_Local_RealLoader.PreviewFinish += CS_locals1._Lambda___R3;
            }
            CS_locals1._VB_Local_RealLoader.OnStateChangedUi += this._Lambda___20_2<TIn>;
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
            if (this.PageLoader.State == ModBase.LoadState.Finished && CS_locals1._VB_Local_FinishedInvoke != null)
            {
                ModBase.RunInUiWait(new Action(CS_locals1._Lambda___4));
            }
            this.PageLoaderUi.State = CS_locals1._VB_Local_RealLoader;
            this.PageLoaderUi.Click += CS_locals1._Lambda___R4;
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
            Logger.Trace("已触发 PageOnEnter", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurPageRight.vb");
            OurPageRight.PageEnterEventHandler pageEnterEvent = this.PageEnterEvent;
            if (pageEnterEvent != null)
            {
                pageEnterEvent();
            }
            OurPageRight.PageStates pageState = this.PageState;
            if (pageState != OurPageRight.PageStates.Empty)
            {
                if (pageState == OurPageRight.PageStates.ContentEnter)
                {
                    return;
                }
                if (pageState != OurPageRight.PageStates.ContentExit)
                {
                    throw new Exception("在状态为 " + this.PageState.ToString() + " 时触发了 PageOnEnter 事件。");
                }
                if (this.PageLoader == null || this.PageLoader.State == ModBase.LoadState.Finished || this.PageLoader.State == ModBase.LoadState.Waiting || this.PageLoader.State == ModBase.LoadState.Canceled)
                {
                    this.PageState = OurPageRight.PageStates.ContentEnter;
                    this.TriggerEnterAnimation(new FrameworkElement[] { (FrameworkElement)(this.PanContent ?? this.Child) });
                    return;
                }
                if (this.PageLoader.State == ModBase.LoadState.Loading)
                {
                    this.PageState = OurPageRight.PageStates.LoaderWait;
                    ModAnimation.AniStart(ModAnimation.AaCode(new ThreadStart(this.PageOnLoaderWaitFinished), 400, false), "PageRight PageChange " + Conversions.ToString(this.PageUuid), false);
                    return;
                }
                this.PageState = OurPageRight.PageStates.LoaderEnter;
                this.TriggerEnterAnimation(new FrameworkElement[] { this.PanLoader });
                return;
            }
            else
            {
                if (this.PageLoader == null || this.PageLoader.State == ModBase.LoadState.Finished || this.PageLoader.State == ModBase.LoadState.Waiting || this.PageLoader.State == ModBase.LoadState.Canceled)
                {
                    this.PageState = OurPageRight.PageStates.ContentEnter;
                    this.TriggerEnterAnimation(new FrameworkElement[]
                    {
                        this.PanAlways,
                        (FrameworkElement)(this.PanContent ?? this.Child)
                    });
                    return;
                }
                if (this.PageLoader.State == ModBase.LoadState.Loading)
                {
                    this.PageState = OurPageRight.PageStates.LoaderWait;
                    ModAnimation.AniStart(ModAnimation.AaCode(new ThreadStart(this.PageOnLoaderWaitFinished), 400, false), "PageRight PageChange " + Conversions.ToString(this.PageUuid), false);
                    return;
                }
                this.PageState = OurPageRight.PageStates.LoaderEnter;
                this.TriggerEnterAnimation(new FrameworkElement[] { this.PanAlways, this.PanLoader });
                return;
            }
        }

        public event OurPageRight.PageEnterEventHandler PageEnter
        {
            [CompilerGenerated]
            add
            {
                OurPageRight.PageEnterEventHandler pageEnterEventHandler = this.PageEnterEvent;
                OurPageRight.PageEnterEventHandler pageEnterEventHandler2;
                do
                {
                    pageEnterEventHandler2 = pageEnterEventHandler;
                    OurPageRight.PageEnterEventHandler pageEnterEventHandler3 = (OurPageRight.PageEnterEventHandler)Delegate.Combine(pageEnterEventHandler2, value);
                    pageEnterEventHandler = Interlocked.CompareExchange<OurPageRight.PageEnterEventHandler>(ref this.PageEnterEvent, pageEnterEventHandler3, pageEnterEventHandler2);
                }
                while (pageEnterEventHandler != pageEnterEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurPageRight.PageEnterEventHandler pageEnterEventHandler = this.PageEnterEvent;
                OurPageRight.PageEnterEventHandler pageEnterEventHandler2;
                do
                {
                    pageEnterEventHandler2 = pageEnterEventHandler;
                    OurPageRight.PageEnterEventHandler pageEnterEventHandler3 = (OurPageRight.PageEnterEventHandler)Delegate.Remove(pageEnterEventHandler2, value);
                    pageEnterEventHandler = Interlocked.CompareExchange<OurPageRight.PageEnterEventHandler>(ref this.PageEnterEvent, pageEnterEventHandler3, pageEnterEventHandler2);
                }
                while (pageEnterEventHandler != pageEnterEventHandler2);
            }
        }

        public void PageOnExit()
        {
            Logger.Trace("已触发 PageOnExit", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurPageRight.vb");
            OurPageRight.PageExitEventHandler pageExitEvent = this.PageExitEvent;
            if (pageExitEvent != null)
            {
                pageExitEvent();
            }
            switch (this.PageState)
            {
            case OurPageRight.PageStates.Empty:
            case OurPageRight.PageStates.PageExit:
                break;
            case OurPageRight.PageStates.LoaderWait:
                this.PageState = OurPageRight.PageStates.PageExit;
                this.TriggerExitAnimation(new FrameworkElement[] { this.PanAlways });
                return;
            case OurPageRight.PageStates.LoaderEnter:
            case OurPageRight.PageStates.LoaderStayForce:
            case OurPageRight.PageStates.LoaderStay:
                this.PageState = OurPageRight.PageStates.PageExit;
                this.TriggerExitAnimation(new FrameworkElement[] { this.PanAlways, this.PanLoader });
                return;
            case OurPageRight.PageStates.LoaderExit:
            case OurPageRight.PageStates.ContentExit:
                this.PageState = OurPageRight.PageStates.PageExit;
                if (this.PanAlways != null)
                {
                    this.TriggerExitAnimation(new FrameworkElement[]
                    {
                        this.PanAlways,
                        (FrameworkElement)(this.PanContent ?? this.Child)
                    });
                }
                break;
            case OurPageRight.PageStates.ContentEnter:
            case OurPageRight.PageStates.ContentStay:
                this.PageState = OurPageRight.PageStates.PageExit;
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

        public event OurPageRight.PageExitEventHandler PageExit
        {
            [CompilerGenerated]
            add
            {
                OurPageRight.PageExitEventHandler pageExitEventHandler = this.PageExitEvent;
                OurPageRight.PageExitEventHandler pageExitEventHandler2;
                do
                {
                    pageExitEventHandler2 = pageExitEventHandler;
                    OurPageRight.PageExitEventHandler pageExitEventHandler3 = (OurPageRight.PageExitEventHandler)Delegate.Combine(pageExitEventHandler2, value);
                    pageExitEventHandler = Interlocked.CompareExchange<OurPageRight.PageExitEventHandler>(ref this.PageExitEvent, pageExitEventHandler3, pageExitEventHandler2);
                }
                while (pageExitEventHandler != pageExitEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurPageRight.PageExitEventHandler pageExitEventHandler = this.PageExitEvent;
                OurPageRight.PageExitEventHandler pageExitEventHandler2;
                do
                {
                    pageExitEventHandler2 = pageExitEventHandler;
                    OurPageRight.PageExitEventHandler pageExitEventHandler3 = (OurPageRight.PageExitEventHandler)Delegate.Remove(pageExitEventHandler2, value);
                    pageExitEventHandler = Interlocked.CompareExchange<OurPageRight.PageExitEventHandler>(ref this.PageExitEvent, pageExitEventHandler3, pageExitEventHandler2);
                }
                while (pageExitEventHandler != pageExitEventHandler2);
            }
        }

        public void PageOnForceExit()
        {
            if (this.PageState != OurPageRight.PageStates.Empty)
            {
                Logger.Trace("已触发 PageOnForceExit", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurPageRight.vb");
                this.PageState = OurPageRight.PageStates.Empty;
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
            Logger.Trace("已触发 PageOnContentExit", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurPageRight.vb");
            if (this.PageLoader != null && this.PageLoader.State == ModBase.LoadState.Loading)
            {
                throw new Exception("在调用 PageOnContentExit 时，加载器不能为 Loading 状态");
            }
            switch (this.PageState)
            {
            case OurPageRight.PageStates.Empty:
                if (base.IsLoaded)
                {
                    this.PageOnEnter();
                }
                return;
            case OurPageRight.PageStates.LoaderWait:
                this.PageOnEnter();
                return;
            case OurPageRight.PageStates.LoaderEnter:
            case OurPageRight.PageStates.LoaderStayForce:
            case OurPageRight.PageStates.LoaderStay:
                this.PageState = OurPageRight.PageStates.ContentExit;
                this.TriggerExitAnimation(new FrameworkElement[] { this.PanLoader });
                return;
            case OurPageRight.PageStates.LoaderExit:
                this.PageState = OurPageRight.PageStates.ContentExit;
                return;
            case OurPageRight.PageStates.ContentEnter:
            case OurPageRight.PageStates.ContentStay:
                this.PageState = OurPageRight.PageStates.ContentExit;
                this.TriggerExitAnimation(new FrameworkElement[] { this.PanContent });
                return;
            default:
                return;
            }
        }

        private void PageOnEnterAnimationFinished()
        {
            Logger.Trace("已触发 PageOnEnterAnimationFinished", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurPageRight.vb");
            OurPageRight.PageStates pageState = this.PageState;
            if (pageState == OurPageRight.PageStates.LoaderEnter)
            {
                this.PageState = OurPageRight.PageStates.LoaderStayForce;
                ModAnimation.AniStart(ModAnimation.AaCode(new ThreadStart(this.PageOnLoaderStayFinished), 400, false), "PageRight PageChange " + Conversions.ToString(this.PageUuid), false);
                return;
            }
            if (pageState == OurPageRight.PageStates.ContentEnter)
            {
                this.PageState = OurPageRight.PageStates.ContentStay;
                return;
            }
            throw new Exception("在状态为 " + this.PageState.ToString() + " 时触发了 PageOnEnterAnimationFinished 事件。");
        }

        private void PageOnExitAnimationFinished()
        {
            Logger.Trace("已触发 PageOnExitAnimationFinished", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurPageRight.vb");
            switch (this.PageState)
            {
            case OurPageRight.PageStates.LoaderExit:
                this.PageState = OurPageRight.PageStates.ContentEnter;
                this.TriggerEnterAnimation(new FrameworkElement[] { this.PanContent });
                return;
            case OurPageRight.PageStates.ContentExit:
                this.PageOnEnter();
                return;
            case OurPageRight.PageStates.PageExit:
                this.PageState = OurPageRight.PageStates.Empty;
                return;
            }
            throw new Exception("在状态为 " + this.PageState.ToString() + " 时触发了 PageOnExitAnimationFinished 事件。");
        }

        private void PageOnLoaderWaitFinished()
        {
            Logger.Trace("已触发 PageOnLoaderWaitFinished", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurPageRight.vb");
            OurPageRight.PageStates pageState = this.PageState;
            if (pageState != OurPageRight.PageStates.LoaderWait)
            {
                throw new Exception("在状态为 " + this.PageState.ToString() + " 时触发了 PageOnLoaderWaitFinished 事件。");
            }
            this.PageState = OurPageRight.PageStates.LoaderEnter;
            if (this.PanAlways != null && this.PanAlways.Visibility == Visibility.Collapsed)
            {
                this.TriggerEnterAnimation(new FrameworkElement[] { this.PanAlways, this.PanLoader });
                return;
            }
            this.TriggerEnterAnimation(new FrameworkElement[] { this.PanLoader });
        }

        private void PageOnLoaderStayFinished()
        {
            Logger.Trace("已触发 PageOnLoaderStayFinished", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurPageRight.vb");
            OurPageRight.PageStates pageState = this.PageState;
            if (pageState != OurPageRight.PageStates.LoaderStayForce)
            {
                throw new Exception("在状态为 " + this.PageState.ToString() + " 时触发了 PageOnLoaderWaitFinished 事件。");
            }
            if (this.PageLoader.State == ModBase.LoadState.Finished)
            {
                this.PageState = OurPageRight.PageStates.LoaderExit;
                this.TriggerExitAnimation(new FrameworkElement[] { this.PanLoader });
                return;
            }
            this.PageState = OurPageRight.PageStates.LoaderStay;
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
                    Logger.Trace("已触发 PageLoaderState (Stop/Canceled)", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurPageRight.vb");
                    OurPageRight.PageStates pageState = this.PageState;
                    if (pageState != OurPageRight.PageStates.LoaderWait)
                    {
                        if (pageState != OurPageRight.PageStates.LoaderStay)
                        {
                            return;
                        }
                        this.PageState = OurPageRight.PageStates.LoaderExit;
                        this.TriggerExitAnimation(new FrameworkElement[] { this.PanLoader });
                    }
                    else
                    {
                        this.PageState = OurPageRight.PageStates.ContentEnter;
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
                    Logger.Trace("已触发 PageLoaderState (Start/Refresh)", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurPageRight.vb");
                    OurPageRight.PageStates pageState2 = this.PageState;
                    if (pageState2 == OurPageRight.PageStates.LoaderExit)
                    {
                        this.PageState = OurPageRight.PageStates.ContentExit;
                        return;
                    }
                    if (pageState2 == OurPageRight.PageStates.ContentEnter || pageState2 == OurPageRight.PageStates.ContentStay)
                    {
                        this.PageState = OurPageRight.PageStates.ContentExit;
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
            IEnumerable<FrameworkElement> enumerable = Elements.Where<FrameworkElement>((OurPageRight._Closure___._I41_0 == null) ? (OurPageRight._Closure___._I41_0 = new Func<FrameworkElement, bool>(OurPageRight._Closure___._I._Lambda___41_0)) : OurPageRight._Closure___._I41_0);
            try
            {
                foreach (FrameworkElement frameworkElement in enumerable)
                {
                    frameworkElement.Visibility = Visibility.Visible;
                }
            }
            finally
            {
                IEnumerator<FrameworkElement> enumerator = default;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
            int num = 0;
            OurScrollBar firstScrollViewer;
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
                            IEnumerator<FrameworkElement> enumerator3 = default;
                            if (enumerator3 != null)
                            {
                                enumerator3.Dispose();
                            }
                        }
                        try
                        {
                            foreach (FrameworkElement frameworkElement4 in this.GetAllAnimControls(frameworkElement2, false))
                            {
                                if (frameworkElement4 is OurExtraTextButton)
                                {
                                    ((OurExtraTextButton)frameworkElement4).Show = true;
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
                            IEnumerator<FrameworkElement> enumerator4 = default;
                            if (enumerator4 != null)
                            {
                                enumerator4.Dispose();
                            }
                        }
                    }
                }
                finally
                {
                    IEnumerator<FrameworkElement> enumerator2 = default;
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
            list.Add(ModAnimation.AaCode(new ThreadStart(this._Lambda___41_1), 0, true));
            ModAnimation.AniStart(list, "PageRight PageChange " + Conversions.ToString(this.PageUuid), true);
        }

        public void TriggerExitAnimation(params FrameworkElement[] Elements)
        {
            OurPageRight._Closure___42_0 CS_locals1 = new OurPageRight._Closure___42_0(null);
            CS_locals1._VB_Me = this;
            CS_locals1._VB_Local_RealElements = Elements.Where<FrameworkElement>((OurPageRight._Closure___._I42_0 == null) ? (OurPageRight._Closure___._I42_0 = new Func<FrameworkElement, bool>(OurPageRight._Closure___._I._Lambda___42_0)) : OurPageRight._Closure___._I42_0);
            List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
            int num = 0;
            OurScrollBar firstScrollViewer;
            checked
            {
                try
                {
                    foreach (FrameworkElement frameworkElement in CS_locals1._VB_Local_RealElements)
                    {
                        try
                        {
                            foreach (FrameworkElement frameworkElement2 in this.GetAllAnimControls(frameworkElement, false))
                            {
                                if (frameworkElement2 is OurExtraTextButton)
                                {
                                    ((OurExtraTextButton)frameworkElement2).Show = false;
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
                            IEnumerator<FrameworkElement> enumerator2 = default;
                            if (enumerator2 != null)
                            {
                                enumerator2.Dispose();
                            }
                        }
                    }
                }
                finally
                {
                    IEnumerator<FrameworkElement> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                firstScrollViewer = this.GetFirstScrollViewer(CS_locals1._VB_Local_RealElements);
            }
            if (firstScrollViewer != null)
            {
                if (!(firstScrollViewer.RenderTransform is TranslateTransform))
                {
                    firstScrollViewer.RenderTransform = new TranslateTransform();
                }
                list.Add(ModAnimation.AaTranslateX(firstScrollViewer, 10.0 - ((TranslateTransform)firstScrollViewer.RenderTransform).X, 90, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false));
            }
            list.Add(ModAnimation.AaCode(new ThreadStart(CS_locals1._Lambda___1), 0, true));
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
                if (Element is OurCard || Element is OurHint || Element is OurExtraTextButton || Element is TextBlock || Element is OurTextButton)
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
                        IEnumerator enumerator = default;
                        if (enumerator is IDisposable)
                        {
                            (enumerator as IDisposable).Dispose();
                        }
                    }
                }
            }
        }

        private OurScrollBar GetFirstScrollViewer(IEnumerable<FrameworkElement> Elements)
        {
            OurScrollViewer ourScrollViewer = null;
            try
            {
                foreach (FrameworkElement frameworkElement in Elements)
                {
                    if (frameworkElement is OurScrollViewer)
                    {
                        ourScrollViewer = (OurScrollViewer)frameworkElement;
                        goto IL_008D;
                    }
                    try
                    {
                        foreach (object obj in LogicalTreeHelper.GetChildren(frameworkElement))
                        {
                            object objectValue = RuntimeHelpers.GetObjectValue(obj);
                            if (objectValue is OurScrollViewer)
                            {
                                ourScrollViewer = (OurScrollViewer)objectValue;
                                goto IL_008D;
                            }
                        }
                    }
                    finally
                    {
                        IEnumerator enumerator2 = default;
                        if (enumerator2 is IDisposable)
                        {
                            (enumerator2 as IDisposable).Dispose();
                        }
                    }
                }
            }
            finally
            {
                IEnumerator<FrameworkElement> enumerator = default;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            return null;
            IL_008D:
            OurScrollBar ourScrollBar;
            if (ourScrollViewer.ComputedVerticalScrollBarVisibility != Visibility.Visible)
            {
                ourScrollBar = null;
            }
            else
            {
                ourScrollBar = ourScrollViewer.ScrollBar;
            }
            return ourScrollBar;
        }

        [CompilerGenerated]
        private void _Lambda___19_2<_CLS0, _CLS1>(ModLoader.LoaderBase Loader, ModBase.LoadState NewState, ModBase.LoadState OldState)
        {
            OurPageRight._Closure___19_1<_CLS0, _CLS1> CS_locals1 = new OurPageRight._Closure___19_1<_CLS0, _CLS1>(null);
            CS_locals1._VB_Me = this;
            CS_locals1._VB_Local_Loader = Loader;
            CS_locals1._VB_Local_NewState = NewState;
            CS_locals1._VB_Local_OldState = OldState;
            ModBase.RunInUi(new Action(CS_locals1._Lambda___3), false);
        }

        [CompilerGenerated]
        private void _Lambda___20_2<_CLS0>(ModLoader.LoaderBase Loader, ModBase.LoadState NewState, ModBase.LoadState OldState)
        {
            OurPageRight._Closure___20_1<_CLS0> CS_locals1 = new OurPageRight._Closure___20_1<_CLS0>(null);
            CS_locals1._VB_Me = this;
            CS_locals1._VB_Local_Loader = Loader;
            CS_locals1._VB_Local_NewState = NewState;
            CS_locals1._VB_Local_OldState = OldState;
            ModBase.RunInUi(new Action(CS_locals1._Lambda___3), false);
        }

        [CompilerGenerated]
        private void _Lambda___41_1()
        {
            this.PageOnEnterAnimationFinished();
        }

        public int PageUuid;

        private static readonly object PanScrollProperty = DependencyProperty.Register("PanScroll", typeof(string), typeof(OurPageRight), new PropertyMetadata(null));

        private OurPageRight.PageStates _PageState;

        private ModLoader.LoaderBase PageLoader;

        private object PageLoaderInputInvoke;

        private OurLoading PageLoaderUi;

        private FrameworkElement PanLoader;

        private FrameworkElement PanContent;

        private FrameworkElement PanAlways;

        private bool PageLoaderAutoRun;

        [CompilerGenerated]
        private OurPageRight.PageEnterEventHandler PageEnterEvent;

        [CompilerGenerated]
        private OurPageRight.PageExitEventHandler PageExitEvent;

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

            internal bool _Lambda___41_0(FrameworkElement e)
            {
                return e != null;
            }

            internal bool _Lambda___42_0(FrameworkElement e)
            {
                return e != null;
            }

            public static readonly OurPageRight._Closure___ _I = new OurPageRight._Closure___();

            public static Func<FrameworkElement, bool> _I41_0;

            public static Func<FrameworkElement, bool> _I42_0;
        }

        [CompilerGenerated]
        private sealed class _Closure___19_0<_CLS0, _CLS1>
        {
            public _Closure___19_0(OurPageRight._Closure___19_0<_CLS0, _CLS1> arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_FinishedInvoke = arg0._VB_Local_FinishedInvoke;
                    this._VB_Local_RealLoader = arg0._VB_Local_RealLoader;
                }
            }

            internal void _Lambda___R1(ModLoader.LoaderBase a0)
            {
                this._Lambda___0();
            }

            internal void _Lambda___0()
            {
                while (this._VB_Me.PageState == OurPageRight.PageStates.PageExit || this._VB_Me.PageState == OurPageRight.PageStates.ContentExit)
                {
                    Thread.Sleep(10);
                }
                ModBase.RunInUiWait((this._I1 == null) ? (this._I1 = new Action(this._Lambda___1)) : this._I1);
                Thread.Sleep(20);
            }

            internal void _Lambda___1()
            {
                this._VB_Local_FinishedInvoke(this._VB_Local_RealLoader);
            }

            internal void _Lambda___4()
            {
                this._VB_Local_FinishedInvoke(this._VB_Local_RealLoader);
            }

            internal void _Lambda___R2(object a0, MouseButtonEventArgs a1)
            {
                this._Lambda___5();
            }

            internal void _Lambda___5()
            {
                if (this._VB_Local_RealLoader.State == ModBase.LoadState.Failed)
                {
                    this._VB_Me.PageLoaderRestart(null, true);
                }
            }

            public Action<ModLoader.LoaderTask<_CLS0, _CLS1>> _VB_Local_FinishedInvoke;

            public ModLoader.LoaderTask<_CLS0, _CLS1> _VB_Local_RealLoader;

            public OurPageRight _VB_Me;

            public Action _I1;
        }

        [CompilerGenerated]
        private sealed class _Closure___19_1<_CLS0, _CLS1>
        {
            public _Closure___19_1(OurPageRight._Closure___19_1<_CLS0, _CLS1> arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Loader = arg0._VB_Local_Loader;
                    this._VB_Local_NewState = arg0._VB_Local_NewState;
                    this._VB_Local_OldState = arg0._VB_Local_OldState;
                }
            }

            internal void _Lambda___3()
            {
                this._VB_Me.PageLoaderState(this._VB_Local_Loader, this._VB_Local_NewState, this._VB_Local_OldState);
            }

            public ModLoader.LoaderBase _VB_Local_Loader;

            public ModBase.LoadState _VB_Local_NewState;

            public ModBase.LoadState _VB_Local_OldState;

            public OurPageRight _VB_Me;
        }

        [CompilerGenerated]
        private sealed class _Closure___20_0<_CLS0>
        {
            public _Closure___20_0(OurPageRight._Closure___20_0<_CLS0> arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_FinishedInvoke = arg0._VB_Local_FinishedInvoke;
                    this._VB_Local_RealLoader = arg0._VB_Local_RealLoader;
                }
            }

            internal void _Lambda___R3(ModLoader.LoaderBase a0)
            {
                this._Lambda___0();
            }

            internal void _Lambda___0()
            {
                while (this._VB_Me.PageState == OurPageRight.PageStates.PageExit || this._VB_Me.PageState == OurPageRight.PageStates.ContentExit)
                {
                    Thread.Sleep(10);
                }
                ModBase.RunInUiWait((this._I1 == null) ? (this._I1 = new Action(this._Lambda___1)) : this._I1);
                Thread.Sleep(20);
            }

            internal void _Lambda___1()
            {
                this._VB_Local_FinishedInvoke(this._VB_Local_RealLoader);
            }

            internal void _Lambda___4()
            {
                this._VB_Local_FinishedInvoke(this._VB_Local_RealLoader);
            }

            internal void _Lambda___R4(object a0, MouseButtonEventArgs a1)
            {
                this._Lambda___5();
            }

            internal void _Lambda___5()
            {
                if (this._VB_Local_RealLoader.State == ModBase.LoadState.Failed)
                {
                    this._VB_Me.PageLoaderRestart(null, true);
                }
            }

            public Action<ModLoader.LoaderCombo<_CLS0>> _VB_Local_FinishedInvoke;

            public ModLoader.LoaderCombo<_CLS0> _VB_Local_RealLoader;

            public OurPageRight _VB_Me;

            public Action _I1;
        }

        [CompilerGenerated]
        private sealed class _Closure___20_1<_CLS0>
        {
            public _Closure___20_1(OurPageRight._Closure___20_1<_CLS0> arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Loader = arg0._VB_Local_Loader;
                    this._VB_Local_NewState = arg0._VB_Local_NewState;
                    this._VB_Local_OldState = arg0._VB_Local_OldState;
                }
            }

            internal void _Lambda___3()
            {
                this._VB_Me.PageLoaderState(this._VB_Local_Loader, this._VB_Local_NewState, this._VB_Local_OldState);
            }

            public ModLoader.LoaderBase _VB_Local_Loader;

            public ModBase.LoadState _VB_Local_NewState;

            public ModBase.LoadState _VB_Local_OldState;

            public OurPageRight _VB_Me;
        }

        [CompilerGenerated]
        private sealed class _Closure___42_0
        {
            public _Closure___42_0(OurPageRight._Closure___42_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_RealElements = arg0._VB_Local_RealElements;
                }
            }

            internal void _Lambda___1()
            {
                try
                {
                    foreach (FrameworkElement frameworkElement in this._VB_Local_RealElements)
                    {
                        frameworkElement.Visibility = Visibility.Collapsed;
                    }
                }
                finally
                {
                    IEnumerator<FrameworkElement> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                this._VB_Me.PageOnExitAnimationFinished();
            }

            public IEnumerable<FrameworkElement> _VB_Local_RealElements;

            public OurPageRight _VB_Me;
        }
    }
}
