using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class OurLoading : Grid
    {
        public event OurLoading.IsErrorChangedEventHandler IsErrorChanged
        {
            [CompilerGenerated]
            add
            {
                OurLoading.IsErrorChangedEventHandler isErrorChangedEventHandler = this.IsErrorChangedEvent;
                OurLoading.IsErrorChangedEventHandler isErrorChangedEventHandler2;
                do
                {
                    isErrorChangedEventHandler2 = isErrorChangedEventHandler;
                    OurLoading.IsErrorChangedEventHandler isErrorChangedEventHandler3 = (OurLoading.IsErrorChangedEventHandler)Delegate.Combine(isErrorChangedEventHandler2, value);
                    isErrorChangedEventHandler = Interlocked.CompareExchange<OurLoading.IsErrorChangedEventHandler>(ref this.IsErrorChangedEvent, isErrorChangedEventHandler3, isErrorChangedEventHandler2);
                }
                while (isErrorChangedEventHandler != isErrorChangedEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurLoading.IsErrorChangedEventHandler isErrorChangedEventHandler = this.IsErrorChangedEvent;
                OurLoading.IsErrorChangedEventHandler isErrorChangedEventHandler2;
                do
                {
                    isErrorChangedEventHandler2 = isErrorChangedEventHandler;
                    OurLoading.IsErrorChangedEventHandler isErrorChangedEventHandler3 = (OurLoading.IsErrorChangedEventHandler)Delegate.Remove(isErrorChangedEventHandler2, value);
                    isErrorChangedEventHandler = Interlocked.CompareExchange<OurLoading.IsErrorChangedEventHandler>(ref this.IsErrorChangedEvent, isErrorChangedEventHandler3, isErrorChangedEventHandler2);
                }
                while (isErrorChangedEventHandler != isErrorChangedEventHandler2);
            }
        }

        public event OurLoading.StateChangedEventHandler StateChanged
        {
            [CompilerGenerated]
            add
            {
                OurLoading.StateChangedEventHandler stateChangedEventHandler = this.StateChangedEvent;
                OurLoading.StateChangedEventHandler stateChangedEventHandler2;
                do
                {
                    stateChangedEventHandler2 = stateChangedEventHandler;
                    OurLoading.StateChangedEventHandler stateChangedEventHandler3 = (OurLoading.StateChangedEventHandler)Delegate.Combine(stateChangedEventHandler2, value);
                    stateChangedEventHandler = Interlocked.CompareExchange<OurLoading.StateChangedEventHandler>(ref this.StateChangedEvent, stateChangedEventHandler3, stateChangedEventHandler2);
                }
                while (stateChangedEventHandler != stateChangedEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurLoading.StateChangedEventHandler stateChangedEventHandler = this.StateChangedEvent;
                OurLoading.StateChangedEventHandler stateChangedEventHandler2;
                do
                {
                    stateChangedEventHandler2 = stateChangedEventHandler;
                    OurLoading.StateChangedEventHandler stateChangedEventHandler3 = (OurLoading.StateChangedEventHandler)Delegate.Remove(stateChangedEventHandler2, value);
                    stateChangedEventHandler = Interlocked.CompareExchange<OurLoading.StateChangedEventHandler>(ref this.StateChangedEvent, stateChangedEventHandler3, stateChangedEventHandler2);
                }
                while (stateChangedEventHandler != stateChangedEventHandler2);
            }
        }

        public event OurLoading.ClickEventHandler Click
        {
            [CompilerGenerated]
            add
            {
                OurLoading.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurLoading.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurLoading.ClickEventHandler clickEventHandler3 = (OurLoading.ClickEventHandler)Delegate.Combine(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurLoading.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurLoading.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurLoading.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurLoading.ClickEventHandler clickEventHandler3 = (OurLoading.ClickEventHandler)Delegate.Remove(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurLoading.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
        }

        public bool AutoRun
        {
            [CompilerGenerated]
            get
            {
                return this._AutoRun;
            }
            [CompilerGenerated]
            set
            {
                this._AutoRun = value;
            }
        }

        public SolidColorBrush Foreground
        {
            get
            {
                return (SolidColorBrush)base.GetValue(OurLoading.ForegroundProperty);
            }
            set
            {
                base.SetValue(OurLoading.ForegroundProperty, value);
            }
        }

        public OurLoading()
        {
            this.IsErrorChanged += this._Lambda___R25_1;
            base.Loaded += this._Lambda___R25_2;
            base.Loaded += this._Lambda___R25_3;
            base.Loaded += this._Lambda___R25_4;
            base.Unloaded += this._Lambda___R25_5;
            base.MouseLeftButtonUp += this.Button_MouseUp;
            base.MouseLeftButtonDown += this.Button_MouseDown;
            base.MouseLeave += new MouseEventHandler(this.Button_MouseLeave);
            base.MouseLeftButtonUp += new MouseButtonEventHandler(this.Button_MouseLeave);
            this.AutoRun = true;
            this.Uuid = ModBase.GetUuid();
            this._ShowProgress = false;
            this._Text = "加载中";
            this._TextError = "加载失败";
            this.TextErrorInherit = true;
            this._OuterState = OurLoading.OurLoadingState.Unloaded;
            this._InnerState = OurLoading.OurLoadingState.Unloaded;
            this.HasAnimation = true;
            this.IsLooping = false;
            this.ErrorAnimationWaiting = false;
            this.IsMouseDown = false;
            this.InitializeComponent();
            base.SetResourceReference(OurLoading.ForegroundProperty, "ColorBrush3");
        }

        private bool _ShowProgress
        {
            [CompilerGenerated]
            get
            {
                return this.__ShowProgress;
            }
            [CompilerGenerated]
            set
            {
                this.__ShowProgress = value;
            }
        }

        public bool ShowProgress
        {
            get
            {
                return this._ShowProgress;
            }
            set
            {
                if (this._ShowProgress != value)
                {
                    this._ShowProgress = value;
                    this.RefreshText();
                }
            }
        }

        public string Text
        {
            get
            {
                return this._Text;
            }
            set
            {
                this._Text = value;
                this.RefreshText();
            }
        }

        public string TextError
        {
            get
            {
                return this._TextError;
            }
            set
            {
                this._TextError = value;
                this.RefreshText();
            }
        }

        public bool TextErrorInherit
        {
            [CompilerGenerated]
            get
            {
                return this._TextErrorInherit;
            }
            [CompilerGenerated]
            set
            {
                this._TextErrorInherit = value;
            }
        }

        private void RefreshText()
        {
            ModBase.RunInUi(new Action(this._Lambda___45_0), false);
        }

        private ILoadingTrigger _State
        {
            [CompilerGenerated]
            get
            {
                return this.__State;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                ILoadingTrigger.ProgressChangedEventHandler progressChangedEventHandler = new ILoadingTrigger.ProgressChangedEventHandler(this._Lambda___R49_6);
                ILoadingTrigger.LoadingStateChangedEventHandler loadingStateChangedEventHandler = new ILoadingTrigger.LoadingStateChangedEventHandler(this._Lambda___R49_7);
                ILoadingTrigger loadingTrigger = this.__State;
                if (loadingTrigger != null)
                {
                    loadingTrigger.ProgressChanged -= progressChangedEventHandler;
                    loadingTrigger.LoadingStateChanged -= loadingStateChangedEventHandler;
                }
                this.__State = value;
                loadingTrigger = this.__State;
                if (loadingTrigger != null)
                {
                    loadingTrigger.ProgressChanged += progressChangedEventHandler;
                    loadingTrigger.LoadingStateChanged += loadingStateChangedEventHandler;
                }
            }
        }

        public ILoadingTrigger State
        {
            get
            {
                this.InitState();
                return this._State;
            }
            set
            {
                this._State = value;
                this.RefreshState();
            }
        }

        private void InitState()
        {
            if (this._State == null)
            {
                this._State = new OurLoadingStateSimulator();
                if (this.AutoRun)
                {
                    this._State.LoadingState = OurLoading.OurLoadingState.Run;
                }
            }
        }

        private void RefreshState()
        {
            if (this._State.LoadingState == OurLoading.OurLoadingState.Run && !base.IsLoaded)
            {
                this.InnerState = OurLoading.OurLoadingState.Stop;
            }
            this.InnerState = this._State.LoadingState;
            this.OuterState = this._State.LoadingState;
            this.AniLoop();
        }

        private OurLoading.OurLoadingState _OuterState
        {
            [CompilerGenerated]
            get
            {
                return this.__OuterState;
            }
            [CompilerGenerated]
            set
            {
                this.__OuterState = value;
            }
        }

        private OurLoading.OurLoadingState OuterState
        {
            get
            {
                return this._OuterState;
            }
            set
            {
                if (this._OuterState != value)
                {
                    OurLoading.OurLoadingState outerState = this._OuterState;
                    this._OuterState = value;
                    OurLoading.StateChangedEventHandler stateChangedEvent = this.StateChangedEvent;
                    if (stateChangedEvent != null)
                    {
                        stateChangedEvent(this, value, outerState);
                    }
                    if (outerState == OurLoading.OurLoadingState.Error != (value == OurLoading.OurLoadingState.Error))
                    {
                        OurLoading.IsErrorChangedEventHandler isErrorChangedEvent = this.IsErrorChangedEvent;
                        if (isErrorChangedEvent != null)
                        {
                            isErrorChangedEvent(this, value == OurLoading.OurLoadingState.Error);
                        }
                    }
                }
            }
        }

        private OurLoading.OurLoadingState _InnerState
        {
            [CompilerGenerated]
            get
            {
                return this.__InnerState;
            }
            [CompilerGenerated]
            set
            {
                this.__InnerState = value;
            }
        }

        private OurLoading.OurLoadingState InnerState
        {
            get
            {
                return this._InnerState;
            }
            set
            {
                if (this._InnerState != value)
                {
                    int innerState = (int)this._InnerState;
                    this._InnerState = value;
                    this.AniLoop();
                    if (innerState == 2 != (value == OurLoading.OurLoadingState.Error))
                    {
                        this.ErrorAnimation(this, value == OurLoading.OurLoadingState.Error);
                    }
                }
            }
        }

        public bool HasAnimation
        {
            [CompilerGenerated]
            get
            {
                return this._HasAnimation;
            }
            [CompilerGenerated]
            set
            {
                this._HasAnimation = value;
            }
        }

        private void AniLoop()
        {
            if (this.HasAnimation && !this.IsLooping && this.InnerState == OurLoading.OurLoadingState.Run && ModAnimation.AniSpeed <= 10.0 && base.IsLoaded)
            {
                this.IsLooping = true;
                this.ErrorAnimationWaiting = true;
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaRotateTransform(this.PathPickaxe, -20.0 - ((RotateTransform)this.PathPickaxe.RenderTransform).Angle, 350, 250, new ModAnimation.AniEaseInBack(ModAnimation.AniEasePower.Weak), false),
                    ModAnimation.AaRotateTransform(this.PathPickaxe, 50.0, 900, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), true),
                    ModAnimation.AaRotateTransform(this.PathPickaxe, 25.0, 900, 0, new ModAnimation.AniEaseOutElastic(ModAnimation.AniEasePower.Weak), false),
                    ModAnimation.AaCode(new ThreadStart(this._Lambda___75_0), 0, false),
                    ModAnimation.AaOpacity(this.PathLeft, -1.0, 100, 50, null, false),
                    ModAnimation.AaX(this.PathLeft, -5.0, 180, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaY(this.PathLeft, -6.0, 180, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaOpacity(this.PathRight, -1.0, 100, 50, null, false),
                    ModAnimation.AaX(this.PathRight, 5.0, 180, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaY(this.PathRight, -6.0, 180, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaCode(new ThreadStart(this._Lambda___75_1), 0, true)
                }, "OurLoader Loop " + Conversions.ToString(this.Uuid) + "/" + Conversions.ToString(ModBase.GetUuid()), false);
            }
        }

        private void ErrorAnimation(object sender, bool isError)
        {
            if (isError)
            {
                int num = (this.ErrorAnimationWaiting ? 400 : 0);
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaColor(this.PanBack, OurLoading.ForegroundProperty, "ColorBrushRedLight", 300, 0, null, false),
                    ModAnimation.AaOpacity(this.PathError, 1.0 - this.PathError.Opacity, 100, checked(300 + num), null, false),
                    ModAnimation.AaScaleTransform(this.PathError, 1.0 - ((ScaleTransform)this.PathError.RenderTransform).ScaleX, 400, checked(300 + num), new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false)
                }, "OurLoader Error " + Conversions.ToString(this.Uuid), false);
                return;
            }
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaOpacity(this.PathError, -this.PathError.Opacity, 100, 0, null, false),
                ModAnimation.AaScaleTransform(this.PathError, 0.5 - ((ScaleTransform)this.PathError.RenderTransform).ScaleX, 200, 0, null, false),
                ModAnimation.AaColor(this.PanBack, OurLoading.ForegroundProperty, "ColorBrush3", 300, 0, null, false)
            }, "OurLoader Error " + Conversions.ToString(this.Uuid), false);
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OurLoading.ClickEventHandler clickEvent = this.ClickEvent;
            if (clickEvent != null)
            {
                clickEvent(RuntimeHelpers.GetObjectValue(sender), e);
            }
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.IsMouseDown = true;
        }

        private void Button_MouseLeave(object sender, object e)
        {
            this.IsMouseDown = false;
        }







        [CompilerGenerated]
        private void _Lambda___R25_1(object a0, bool a1)
        {
            this.RefreshText();
        }

        [CompilerGenerated]
        private void _Lambda___R25_2(object a0, RoutedEventArgs a1)
        {
            this.RefreshText();
        }

        [CompilerGenerated]
        private void _Lambda___R25_3(object a0, RoutedEventArgs a1)
        {
            this.InitState();
        }

        [CompilerGenerated]
        private void _Lambda___R25_4(object a0, RoutedEventArgs a1)
        {
            this.RefreshState();
        }

        [CompilerGenerated]
        private void _Lambda___R25_5(object a0, RoutedEventArgs a1)
        {
            this.RefreshState();
        }

        [CompilerGenerated]
        private void _Lambda___45_0()
        {
            if (this.InnerState == OurLoading.OurLoadingState.Error)
            {
                if (!this.TextErrorInherit || !this.State.IsLoader)
                {
                    this.LabText.Text = this.TextError;
                    return;
                }
                Exception ex = (Exception)NewLateBinding.LateGet(this.State, null, "Error", new object[0], null, null, null);
                if (ex == null)
                {
                    this.LabText.Text = "未知错误";
                    return;
                }
                this.LabText.Text = ex.GetDisplay(false);
                return;
            }
            else
            {
                if (this.ShowProgress && this.State.IsLoader)
                {
                    this.LabText.Text = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(this.Text + " - ", NewLateBinding.LateGet(null, typeof(Math), "Floor", new object[] { Operators.MultiplyObject(NewLateBinding.LateGet(this.State, null, "Progress", new object[0], null, null, null), 100) }, null, null, null)), "%"));
                    return;
                }
                this.LabText.Text = this.Text;
                return;
            }
        }

        [CompilerGenerated]
        private void _Lambda___R49_6(double a0, double a1)
        {
            this.RefreshText();
        }

        [CompilerGenerated]
        private void _Lambda___R49_7(OurLoading.OurLoadingState a0, OurLoading.OurLoadingState a1)
        {
            this.RefreshState();
        }

        [CompilerGenerated]
        private void _Lambda___75_0()
        {
            this.PathLeft.Opacity = 1.0;
            this.PathLeft.Margin = new Thickness(7.0, 41.0, 0.0, 0.0);
            this.PathRight.Opacity = 1.0;
            this.PathRight.Margin = new Thickness(14.0, 41.0, 0.0, 0.0);
            this.ErrorAnimationWaiting = false;
        }

        [CompilerGenerated]
        private void _Lambda___75_1()
        {
            this.IsLooping = false;
            this.AniLoop();
        }

        [CompilerGenerated]
        private OurLoading.IsErrorChangedEventHandler IsErrorChangedEvent;

        [CompilerGenerated]
        private OurLoading.StateChangedEventHandler StateChangedEvent;

        [CompilerGenerated]
        private OurLoading.ClickEventHandler ClickEvent;

        [CompilerGenerated]
        private bool _AutoRun;

        private int Uuid;

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(SolidColorBrush), typeof(OurLoading));

        [CompilerGenerated]
        private bool __ShowProgress;

        private string _Text;

        private string _TextError;

        [CompilerGenerated]
        private bool _TextErrorInherit;

        [CompilerGenerated]
        [AccessedThroughProperty("_State")]
        private ILoadingTrigger __State;

        [CompilerGenerated]
        private OurLoading.OurLoadingState __OuterState;

        [CompilerGenerated]
        private OurLoading.OurLoadingState __InnerState;

        [CompilerGenerated]
        private bool _HasAnimation;

        private bool IsLooping;

        private bool ErrorAnimationWaiting;

        private bool IsMouseDown;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurLoading _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PathPickaxe")]
        private Path _PathPickaxe;

        [CompilerGenerated]
        [AccessedThroughProperty("PathLeft")]
        private Path _PathLeft;

        [CompilerGenerated]
        [AccessedThroughProperty("PathRight")]
        private Path _PathRight;

        [CompilerGenerated]
        [AccessedThroughProperty("PathError")]
        private Path _PathError;

        [CompilerGenerated]
        [AccessedThroughProperty("LabText")]
        private TextBlock _LabText;

        public delegate void IsErrorChangedEventHandler(object sender, bool isError);

        public delegate void StateChangedEventHandler(object sender, OurLoading.OurLoadingState newState, OurLoading.OurLoadingState oldState);

        public delegate void ClickEventHandler(object sender, MouseButtonEventArgs e);

        public enum OurLoadingState
        {
            Unloaded = -1,
            Run,
            Stop,
            Error
        }
    }
}
