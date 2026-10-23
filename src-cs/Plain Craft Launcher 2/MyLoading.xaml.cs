using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
    public partial class MyLoading : Grid
    {
        public event MyLoading.IsErrorChangedEventHandler IsErrorChanged;

        public event MyLoading.StateChangedEventHandler StateChanged;

        public event MyLoading.ClickEventHandler Click;

        public bool AutoRun { get; set; }

        public SolidColorBrush Foreground
        {
            get
            {
                return (SolidColorBrush)base.GetValue(MyLoading.ForegroundProperty);
            }
            set
            {
                base.SetValue(MyLoading.ForegroundProperty, value);
            }
        }

        public MyLoading()
        {
            this.IsErrorChanged += delegate(object a0, bool a1)
            {
                this.RefreshText();
            };
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.RefreshText();
            };
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.InitState();
            };
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.RefreshState();
            };
            base.Unloaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.RefreshState();
            };
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
            this._OuterState = MyLoading.MyLoadingState.Unloaded;
            this._InnerState = MyLoading.MyLoadingState.Unloaded;
            this.HasAnimation = true;
            this.IsLooping = false;
            this.ErrorAnimationWaiting = false;
            this.IsMouseDown = false;
            this.InitializeComponent();
            base.SetResourceReference(MyLoading.ForegroundProperty, "ColorBrush3");
        }

        private bool _ShowProgress { get; set; }

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

        public bool TextErrorInherit { get; set; }

        private void RefreshText()
        {
            ModBase.RunInUi(delegate
            {
                if (this.InnerState == MyLoading.MyLoadingState.Error)
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
            }, false);
        }

        private virtual ILoadingTrigger _State
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
                ILoadingTrigger.ProgressChangedEventHandler progressChangedEventHandler = delegate(double a0, double a1)
                {
                    this.RefreshText();
                };
                ILoadingTrigger.LoadingStateChangedEventHandler loadingStateChangedEventHandler = delegate(MyLoading.MyLoadingState a0, MyLoading.MyLoadingState a1)
                {
                    this.RefreshState();
                };
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
                this._State = new MyLoadingStateSimulator();
                if (this.AutoRun)
                {
                    this._State.LoadingState = MyLoading.MyLoadingState.Run;
                }
            }
        }

        private void RefreshState()
        {
            if (this._State.LoadingState == MyLoading.MyLoadingState.Run && !base.IsLoaded)
            {
                this.InnerState = MyLoading.MyLoadingState.Stop;
            }
            this.InnerState = this._State.LoadingState;
            this.OuterState = this._State.LoadingState;
            this.AniLoop();
        }

        private MyLoading.MyLoadingState _OuterState { get; set; }

        private MyLoading.MyLoadingState OuterState
        {
            get
            {
                return this._OuterState;
            }
            set
            {
                if (this._OuterState != value)
                {
                    MyLoading.MyLoadingState outerState = this._OuterState;
                    this._OuterState = value;
                    MyLoading.StateChangedEventHandler stateChangedEvent = this.StateChangedEvent;
                    if (stateChangedEvent != null)
                    {
                        stateChangedEvent(this, value, outerState);
                    }
                    if (outerState == MyLoading.MyLoadingState.Error != (value == MyLoading.MyLoadingState.Error))
                    {
                        MyLoading.IsErrorChangedEventHandler isErrorChangedEvent = this.IsErrorChangedEvent;
                        if (isErrorChangedEvent != null)
                        {
                            isErrorChangedEvent(this, value == MyLoading.MyLoadingState.Error);
                        }
                    }
                }
            }
        }

        private MyLoading.MyLoadingState _InnerState { get; set; }

        private MyLoading.MyLoadingState InnerState
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
                    if (innerState == 2 != (value == MyLoading.MyLoadingState.Error))
                    {
                        this.ErrorAnimation(this, value == MyLoading.MyLoadingState.Error);
                    }
                }
            }
        }

        public bool HasAnimation { get; set; }

        private void AniLoop()
        {
            if (this.HasAnimation && !this.IsLooping && this.InnerState == MyLoading.MyLoadingState.Run && ModAnimation.AniSpeed <= 10.0 && base.IsLoaded)
            {
                this.IsLooping = true;
                this.ErrorAnimationWaiting = true;
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaRotateTransform(this.PathPickaxe, -20.0 - ((RotateTransform)this.PathPickaxe.RenderTransform).Angle, 350, 250, new ModAnimation.AniEaseInBack(ModAnimation.AniEasePower.Weak), false),
                    ModAnimation.AaRotateTransform(this.PathPickaxe, 50.0, 900, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), true),
                    ModAnimation.AaRotateTransform(this.PathPickaxe, 25.0, 900, 0, new ModAnimation.AniEaseOutElastic(ModAnimation.AniEasePower.Weak), false),
                    ModAnimation.AaCode(delegate
                    {
                        this.PathLeft.Opacity = 1.0;
                        this.PathLeft.Margin = new Thickness(7.0, 41.0, 0.0, 0.0);
                        this.PathRight.Opacity = 1.0;
                        this.PathRight.Margin = new Thickness(14.0, 41.0, 0.0, 0.0);
                        this.ErrorAnimationWaiting = false;
                    }, 0, false),
                    ModAnimation.AaOpacity(this.PathLeft, -1.0, 100, 50, null, false),
                    ModAnimation.AaX(this.PathLeft, -5.0, 180, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaY(this.PathLeft, -6.0, 180, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaOpacity(this.PathRight, -1.0, 100, 50, null, false),
                    ModAnimation.AaX(this.PathRight, 5.0, 180, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaY(this.PathRight, -6.0, 180, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaCode(delegate
                    {
                        this.IsLooping = false;
                        this.AniLoop();
                    }, 0, true)
                }, "MyLoader Loop " + Conversions.ToString(this.Uuid) + "/" + Conversions.ToString(ModBase.GetUuid()), false);
            }
        }

        private void ErrorAnimation(object sender, bool isError)
        {
            if (isError)
            {
                int num = (this.ErrorAnimationWaiting ? 400 : 0);
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaColor(this.PanBack, MyLoading.ForegroundProperty, "ColorBrushRedLight", 300, 0, null, false),
                    ModAnimation.AaOpacity(this.PathError, 1.0 - this.PathError.Opacity, 100, checked(300 + num), null, false),
                    ModAnimation.AaScaleTransform(this.PathError, 1.0 - ((ScaleTransform)this.PathError.RenderTransform).ScaleX, 400, checked(300 + num), new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false)
                }, "MyLoader Error " + Conversions.ToString(this.Uuid), false);
                return;
            }
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaOpacity(this.PathError, -this.PathError.Opacity, 100, 0, null, false),
                ModAnimation.AaScaleTransform(this.PathError, 0.5 - ((ScaleTransform)this.PathError.RenderTransform).ScaleX, 200, 0, null, false),
                ModAnimation.AaColor(this.PanBack, MyLoading.ForegroundProperty, "ColorBrush3", 300, 0, null, false)
            }, "MyLoader Error " + Conversions.ToString(this.Uuid), false);
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MyLoading.ClickEventHandler clickEvent = this.ClickEvent;
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

        internal virtual MyLoading PanBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Path PathPickaxe
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Path PathLeft
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Path PathRight
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Path PathError
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabText
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        private int Uuid;

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(SolidColorBrush), typeof(MyLoading));

        private string _Text;

        private string _TextError;

        private bool IsLooping;

        private bool ErrorAnimationWaiting;

        private bool IsMouseDown;

        public delegate void IsErrorChangedEventHandler(object sender, bool isError);

        public delegate void StateChangedEventHandler(object sender, MyLoading.MyLoadingState newState, MyLoading.MyLoadingState oldState);

        public delegate void ClickEventHandler(object sender, MouseButtonEventArgs e);

        public enum MyLoadingState
        {
            Unloaded = -1,
            Run,
            Stop,
            Error
        }
    }
}
