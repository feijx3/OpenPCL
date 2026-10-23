using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class MyMsgInput : Grid
    {
        public MyMsgInput(ModMain.MyMsgBoxConverter Converter)
        {
            base.Loaded += new RoutedEventHandler(this.Load);
            this.Uuid = ModBase.GetUuid();
            try
            {
                this.InitializeComponent();
                this.Btn1.Name = this.Btn1.Name + Conversions.ToString(ModBase.GetUuid());
                this.Btn2.Name = this.Btn2.Name + Conversions.ToString(ModBase.GetUuid());
                this.MyConverter = Converter;
                this.LabTitle.Text = Converter.Title;
                this.LabText.Text = Converter.Text;
                this.PanText.Visibility = ((Operators.CompareString(Converter.Text, "", false) == 0) ? Visibility.Collapsed : Visibility.Visible);
                this.TextArea.Text = Conversions.ToString(Converter.Content);
                this.TextArea.HintText = Converter.HintText;
                this.TextArea.ValidateRules = Converter.ValidateRules;
                this.Btn1.Text = Converter.Button1;
                if (Converter.IsWarn)
                {
                    this.Btn1.ColorType = MyButton.ColorState.Red;
                    this.LabTitle.SetResourceReference(TextBlock.ForegroundProperty, "ColorBrushRedLight");
                }
                this.Btn2.Text = Converter.Button2;
                this.Btn2.Visibility = ((Operators.CompareString(Converter.Button2, "", false) == 0) ? Visibility.Collapsed : Visibility.Visible);
                this.ShapeLine.StrokeThickness = ModBase.GetWPFSize(1.0);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "输入弹窗初始化失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyMsg\\MyMsgInput.xaml.vb");
            }
        }

        private void Load(object sender, EventArgs e)
        {
            try
            {
                if (this.Btn2.IsVisible && this.Btn1.ColorType != MyButton.ColorState.Red)
                {
                    this.Btn1.ColorType = MyButton.ColorState.Highlight;
                }
                this.TextArea.Focus();
                this.TextArea.SelectionStart = this.TextArea.Text.Length;
                base.Opacity = 0.0;
                ModAnimation.AniStart(ModAnimation.AaColor(ModMain.FrmMain.PanMsg, Panel.BackgroundProperty, (this.MyConverter.IsWarn ? new ModBase.MyColor(140.0, 80.0, 0.0, 0.0) : new ModBase.MyColor(90.0, 0.0, 0.0, 0.0)) - ModMain.FrmMain.PanMsg.Background, 200, 0, null, false), "PanMsg Background", false);
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaOpacity(this, 1.0, 120, 60, null, false),
                    ModAnimation.AaDouble(delegate(object i)
                    {
                        TranslateTransform transformPos;
                        (transformPos = this.TransformPos).Y = Conversions.ToDouble(Operators.AddObject(transformPos.Y, i));
                    }, -this.TransformPos.Y, 300, 60, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false),
                    ModAnimation.AaDouble(delegate(object i)
                    {
                        RotateTransform transformRotate;
                        (transformRotate = this.TransformRotate).Angle = Conversions.ToDouble(Operators.AddObject(transformRotate.Angle, i));
                    }, -this.TransformRotate.Angle, 300, 60, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false)
                }, "MyMsgBox " + Conversions.ToString(this.Uuid), false);
                Logger.Info(string.Format("输入弹窗：{0}", this.LabTitle.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyMsg\\MyMsgInput.xaml.vb");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "输入弹窗加载失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyMsg\\MyMsgInput.xaml.vb");
            }
        }

        private void Close()
        {
            this.MyConverter.WaitFrame.Continue = false;
            ComponentDispatcher.PopModal();
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaCode((MyMsgInput._Closure$__.$I4-0 == null) ? (MyMsgInput._Closure$__.$I4-0 = delegate
                {
                    if (!ModMain.WaitingMyMsgBox.Any<ModMain.MyMsgBoxConverter>())
                    {
                        ModAnimation.AniStart(ModAnimation.AaColor(ModMain.FrmMain.PanMsg, Panel.BackgroundProperty, new ModBase.MyColor(0.0, 0.0, 0.0, 0.0) - ModMain.FrmMain.PanMsg.Background, 200, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false), "", false);
                    }
                }) : MyMsgInput._Closure$__.$I4-0, 30, false),
                ModAnimation.AaOpacity(this, -base.Opacity, 80, 20, null, false),
                ModAnimation.AaDouble(delegate(object i)
                {
                    TranslateTransform transformPos;
                    (transformPos = this.TransformPos).Y = Conversions.ToDouble(Operators.AddObject(transformPos.Y, i));
                }, 20.0 - this.TransformPos.Y, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                ModAnimation.AaDouble(delegate(object i)
                {
                    RotateTransform transformRotate;
                    (transformRotate = this.TransformRotate).Angle = Conversions.ToDouble(Operators.AddObject(transformRotate.Angle, i));
                }, 6.0 - this.TransformRotate.Angle, 150, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), false),
                ModAnimation.AaCode(delegate
                {
                    ((Grid)base.Parent).Children.Remove(this);
                }, 0, true)
            }, "MyMsgBox " + Conversions.ToString(this.Uuid), false);
        }

        public void Btn1_Click()
        {
            this.TextArea.Validate();
            if (!this.MyConverter.IsExited && this.TextArea.IsValidated)
            {
                this.MyConverter.IsExited = true;
                this.MyConverter.Result = this.TextArea.Text;
                this.Close();
            }
        }

        public void Btn2_Click()
        {
            if (!this.MyConverter.IsExited)
            {
                this.MyConverter.IsExited = true;
                this.MyConverter.Result = null;
                this.Close();
            }
        }

        private void TextCaption_ValidateChanged(object sender, EventArgs e)
        {
            this.Btn1.IsEnabled = this.TextArea.IsValidated;
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            int num;
            int num4;
            object obj;
            try
            {
                IL_0000:
                ProjectData.ClearProjectError();
                num = 1;
                IL_0007:
                int num2 = 2;
                if (e.GetPosition(this.ShapeLine).Y > 2.0)
                {
                    goto IL_0034;
                }
                IL_0028:
                num2 = 3;
                ModMain.FrmMain.DragMove();
                IL_0034:
                goto IL_0093;
                IL_0036:
                int num3 = num4 + 1;
                num4 = 0;
                @switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num3);
                IL_0054:
                goto IL_0088;
                IL_0056:
                num4 = num2;
                @switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num);
                IL_0066:;
            }
            catch when (endfilter((obj is Exception) & (num != 0) & (num4 == 0)))
            {
                Exception ex = (Exception)obj2;
                goto IL_0056;
            }
            IL_0088:
            throw ProjectData.CreateProjectError(-2146828237);
            IL_0093:
            if (num4 != 0)
            {
                ProjectData.ClearProjectError();
            }
        }

        internal virtual RotateTransform TransformRotate
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TranslateTransform TransformPos
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Border PanBorder
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

        internal virtual DropShadowEffect EffectShadow
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanMain
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabTitle
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

        internal virtual Rectangle ShapeLine
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyScrollViewer PanText
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabText
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyTextBox TextArea
        {
            [CompilerGenerated]
            get
            {
                return this._TextArea;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyTextBox.ValidateChangedEventHandler validateChangedEventHandler = new MyTextBox.ValidateChangedEventHandler(this.TextCaption_ValidateChanged);
                if (this._TextArea != null)
                {
                    MyTextBox.ValidateChanged -= validateChangedEventHandler;
                }
                this._TextArea = value;
                if (this._TextArea != null)
                {
                    MyTextBox.ValidateChanged += validateChangedEventHandler;
                }
            }
        }

        internal virtual StackPanel PanBtn
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public virtual MyButton Btn1
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
                MyButton.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.Btn1_Click();
                };
                MyButton myButton = this._Btn1;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._Btn1 = value;
                myButton = this._Btn1;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        public virtual MyButton Btn2
        {
            [CompilerGenerated]
            get
            {
                return this._Btn2;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.Btn2_Click();
                };
                MyButton myButton = this._Btn2;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._Btn2 = value;
                myButton = this._Btn2;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        private readonly ModMain.MyMsgBoxConverter MyConverter;

        private readonly int Uuid;
    }
}
