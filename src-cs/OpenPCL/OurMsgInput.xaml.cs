using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
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
    public partial class OurMsgInput : Grid
    {
        public OurMsgInput(ModMain.OurMsgBoxConverter Converter)
        {
            base.Loaded += new RoutedEventHandler(this.Load);
            this.Uuid = ModBase.GetUuid();
            try
            {
                this.InitializeComponent();
                this.PanBorder_WithEvents = this.PanBorder;
                this.LabTitle_WithEvents = this.LabTitle;
                this.TextArea_WithEvents = this.TextArea;
                this.Btn1_WithEvents = this.Btn1;
                this.Btn2_WithEvents = this.Btn2;
                this.Btn1.Name = this.Btn1.Name + Conversions.ToString(ModBase.GetUuid());
                this.Btn2.Name = this.Btn2.Name + Conversions.ToString(ModBase.GetUuid());
                this.OurConverter = Converter;
                this.LabTitle.Text = Converter.Title;
                this.LabText.Text = Converter.Text;
                this.PanText.Visibility = ((Operators.CompareString(Converter.Text, "", false) == 0) ? Visibility.Collapsed : Visibility.Visible);
                this.TextArea.Text = Conversions.ToString(Converter.Content);
                this.TextArea.HintText = Converter.HintText;
                this.TextArea.ValidateRules = Converter.ValidateRules;
                this.Btn1.Text = Converter.Button1;
                if (Converter.IsWarn)
                {
                    this.Btn1.ColorType = OurButton.ColorState.Red;
                    this.LabTitle.SetResourceReference(TextBlock.ForegroundProperty, "ColorBrushRedLight");
                }
                this.Btn2.Text = Converter.Button2;
                this.Btn2.Visibility = ((Operators.CompareString(Converter.Button2, "", false) == 0) ? Visibility.Collapsed : Visibility.Visible);
                this.ShapeLine.StrokeThickness = ModBase.GetWPFSize(1.0);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "输入弹窗初始化失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurMsg\\OurMsgInput.xaml.vb");
            }
        }

        private void Load(object sender, EventArgs e)
        {
            try
            {
                if (this.Btn2.IsVisible && this.Btn1.ColorType != OurButton.ColorState.Red)
                {
                    this.Btn1.ColorType = OurButton.ColorState.Highlight;
                }
                this.TextArea.Focus();
                this.TextArea.SelectionStart = this.TextArea.Text.Length;
                base.Opacity = 0.0;
                ModAnimation.AniStart(ModAnimation.AaColor(ModMain.FrmMain.PanMsg, Panel.BackgroundProperty, (this.OurConverter.IsWarn ? new ModBase.OurColor(140.0, 80.0, 0.0, 0.0) : new ModBase.OurColor(90.0, 0.0, 0.0, 0.0)) - ModMain.FrmMain.PanMsg.Background, 200, 0, null, false), "PanMsg Background", false);
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaOpacity(this, 1.0, 120, 60, null, false),
                    ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___3_0), -this.TransformPos.Y, 300, 60, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false),
                    ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___3_1), -this.TransformRotate.Angle, 300, 60, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false)
                }, "OurMsgBox " + Conversions.ToString(this.Uuid), false);
                Logger.Info(string.Format("输入弹窗：{0}", this.LabTitle.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurMsg\\OurMsgInput.xaml.vb");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "输入弹窗加载失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurMsg\\OurMsgInput.xaml.vb");
            }
        }

        private void Close()
        {
            this.OurConverter.WaitFrame.Continue = false;
            ComponentDispatcher.PopModal();
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaCode((OurMsgInput._Closure___._I4_0 == null) ? (OurMsgInput._Closure___._I4_0 = new ThreadStart(OurMsgInput._Closure___._I._Lambda___4_0)) : OurMsgInput._Closure___._I4_0, 30, false),
                ModAnimation.AaOpacity(this, -base.Opacity, 80, 20, null, false),
                ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___4_1), 20.0 - this.TransformPos.Y, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___4_2), 6.0 - this.TransformRotate.Angle, 150, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), false),
                ModAnimation.AaCode(new ThreadStart(this._Lambda___4_3), 0, true)
            }, "OurMsgBox " + Conversions.ToString(this.Uuid), false);
        }

        public void Btn1_Click()
        {
            this.TextArea.Validate();
            if (!this.OurConverter.IsExited && this.TextArea.IsValidated)
            {
                this.OurConverter.IsExited = true;
                this.OurConverter.Result = this.TextArea.Text;
                this.Close();
            }
        }

        public void Btn2_Click()
        {
            if (!this.OurConverter.IsExited)
            {
                this.OurConverter.IsExited = true;
                this.OurConverter.Result = null;
                this.Close();
            }
        }

        private void TextCaption_ValidateChanged(object sender, EventArgs e)
        {
            this.Btn1.IsEnabled = this.TextArea.IsValidated;
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




        internal virtual OurTextBox TextArea_WithEvents
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
                OurTextBox.ValidateChangedEventHandler validateChangedEventHandler = new OurTextBox.ValidateChangedEventHandler(this.TextCaption_ValidateChanged);
                if (this._TextArea != null)
                {
                    OurTextBox.ValidateChanged -= validateChangedEventHandler;
                }
                this._TextArea = value;
                if (this._TextArea != null)
                {
                    OurTextBox.ValidateChanged += validateChangedEventHandler;
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R55_1);
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

        public virtual OurButton Btn2_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R59_2);
                OurButton ourButton = this._Btn2;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._Btn2 = value;
                ourButton = this._Btn2;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___3_0(object i)
        {
            TranslateTransform transformPos;
            (transformPos = this.TransformPos).Y = Conversions.ToDouble(Operators.AddObject(transformPos.Y, i));
        }

        [CompilerGenerated]
        private void _Lambda___3_1(object i)
        {
            RotateTransform transformRotate;
            (transformRotate = this.TransformRotate).Angle = Conversions.ToDouble(Operators.AddObject(transformRotate.Angle, i));
        }

        [CompilerGenerated]
        private void _Lambda___4_1(object i)
        {
            TranslateTransform transformPos;
            (transformPos = this.TransformPos).Y = Conversions.ToDouble(Operators.AddObject(transformPos.Y, i));
        }

        [CompilerGenerated]
        private void _Lambda___4_2(object i)
        {
            RotateTransform transformRotate;
            (transformRotate = this.TransformRotate).Angle = Conversions.ToDouble(Operators.AddObject(transformRotate.Angle, i));
        }

        [CompilerGenerated]
        private void _Lambda___4_3()
        {
            ((Grid)base.Parent).Children.Remove(this);
        }

        [CompilerGenerated]
        private void _Lambda___R55_1(object a0, MouseButtonEventArgs a1)
        {
            this.Btn1_Click();
        }

        [CompilerGenerated]
        private void _Lambda___R59_2(object a0, MouseButtonEventArgs a1)
        {
            this.Btn2_Click();
        }

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
        [AccessedThroughProperty("PanText")]
        private OurScrollViewer _PanText;

        [CompilerGenerated]
        [AccessedThroughProperty("LabText")]
        private TextBlock _LabText;

        [CompilerGenerated]
        [AccessedThroughProperty("TextArea")]
        private OurTextBox _TextArea;

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
        [Serializable]
        private sealed class _Closure___
        {
            internal void _Lambda___4_0()
            {
                if (!ModMain.WaitingOurMsgBox.Any<ModMain.OurMsgBoxConverter>())
                {
                    ModAnimation.AniStart(ModAnimation.AaColor(ModMain.FrmMain.PanMsg, Panel.BackgroundProperty, new ModBase.OurColor(0.0, 0.0, 0.0, 0.0) - ModMain.FrmMain.PanMsg.Background, 200, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false), "", false);
                }
            }

            public static readonly OurMsgInput._Closure___ _I = new OurMsgInput._Closure___();

            public static ThreadStart _I4_0;
        }
    }
}
