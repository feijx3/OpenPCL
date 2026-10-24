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
    public partial class OurMsgText : Grid
    {
        public OurMsgText(ModMain.OurMsgBoxConverter Converter)
        {
            base.Loaded += new RoutedEventHandler(this.Load);
            this.Uuid = ModBase.GetUuid();
            try
            {
                this.InitializeComponent();
                this.PanBorder_WithEvents = this.PanBorder;
                this.LabTitle_WithEvents = this.LabTitle;
                this.Btn1_WithEvents = this.Btn1;
                this.Btn2_WithEvents = this.Btn2;
                this.Btn3_WithEvents = this.Btn3;
                this.Btn1.Name = this.Btn1.Name + Conversions.ToString(ModBase.GetUuid());
                this.Btn2.Name = this.Btn2.Name + Conversions.ToString(ModBase.GetUuid());
                this.Btn3.Name = this.Btn3.Name + Conversions.ToString(ModBase.GetUuid());
                this.OurConverter = Converter;
                this.LabTitle.Text = Converter.Title;
                this.LabCaption.Text = Converter.Text;
                this.Btn1.Text = Converter.Button1;
                if (Converter.IsWarn)
                {
                    this.Btn1.ColorType = OurButton.ColorState.Red;
                    this.LabTitle.SetResourceReference(TextBlock.ForegroundProperty, "ColorBrushRedLight");
                }
                this.Btn2.Text = Converter.Button2;
                this.Btn3.Text = Converter.Button3;
                this.Btn2.Visibility = ((Operators.CompareString(Converter.Button2, "", false) == 0) ? Visibility.Collapsed : Visibility.Visible);
                this.Btn3.Visibility = ((Operators.CompareString(Converter.Button3, "", false) == 0) ? Visibility.Collapsed : Visibility.Visible);
                this.ShapeLine.StrokeThickness = ModBase.GetWPFSize(1.0);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "普通弹窗初始化失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurMsg\\OurMsgText.xaml.vb");
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
                this.Btn1.Focus();
                base.Opacity = 0.0;
                ModAnimation.AniStart(ModAnimation.AaColor(ModMain.FrmMain.PanMsg, Panel.BackgroundProperty, (this.OurConverter.IsWarn ? new ModBase.OurColor(140.0, 80.0, 0.0, 0.0) : new ModBase.OurColor(90.0, 0.0, 0.0, 0.0)) - ModMain.FrmMain.PanMsg.Background, 200, 0, null, false), "PanMsg Background", false);
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaOpacity(this, 1.0, 120, 60, null, false),
                    ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___3_0), -this.TransformPos.Y, 300, 60, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false),
                    ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___3_1), -this.TransformRotate.Angle, 300, 60, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false)
                }, "OurMsgBox " + Conversions.ToString(this.Uuid), false);
                Logger.Info(string.Format("普通弹窗：{0}{1}{2}", this.LabTitle.Text, "\r\n", this.LabCaption.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurMsg\\OurMsgText.xaml.vb");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "普通弹窗加载失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurMsg\\OurMsgText.xaml.vb");
            }
        }

        private void Close()
        {
            if (this.OurConverter.ForceWait || Operators.CompareString(this.OurConverter.Button2, "", false) != 0)
            {
                this.OurConverter.WaitFrame.Continue = false;
            }
            ComponentDispatcher.PopModal();
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaCode((OurMsgText._Closure___._I4_0 == null) ? (OurMsgText._Closure___._I4_0 = new ThreadStart(OurMsgText._Closure___._I._Lambda___4_0)) : OurMsgText._Closure___._I4_0, 30, false),
                ModAnimation.AaOpacity(this, -base.Opacity, 80, 20, null, false),
                ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___4_1), 20.0 - this.TransformPos.Y, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___4_2), 6.0 - this.TransformRotate.Angle, 150, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), false),
                ModAnimation.AaCode(new ThreadStart(this._Lambda___4_3), 0, true)
            }, "OurMsgBox " + Conversions.ToString(this.Uuid), false);
        }

        public void Btn1_Click()
        {
            if (!this.OurConverter.IsExited)
            {
                if (this.OurConverter.Button1Action != null)
                {
                    this.OurConverter.Button1Action();
                    return;
                }
                this.OurConverter.IsExited = true;
                this.OurConverter.Result = 1;
                this.Close();
            }
        }

        public void Btn2_Click()
        {
            if (!this.OurConverter.IsExited)
            {
                if (this.OurConverter.Button2Action != null)
                {
                    this.OurConverter.Button2Action();
                    return;
                }
                this.OurConverter.IsExited = true;
                this.OurConverter.Result = 2;
                this.Close();
            }
        }

        public void Btn3_Click()
        {
            if (!this.OurConverter.IsExited)
            {
                if (this.OurConverter.Button3Action != null)
                {
                    this.OurConverter.Button3Action();
                    return;
                }
                this.OurConverter.IsExited = true;
                this.OurConverter.Result = 3;
                this.Close();
            }
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R51_1);
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R55_2);
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R59_3);
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
        private void _Lambda___R51_1(object a0, MouseButtonEventArgs a1)
        {
            this.Btn1_Click();
        }

        [CompilerGenerated]
        private void _Lambda___R55_2(object a0, MouseButtonEventArgs a1)
        {
            this.Btn2_Click();
        }

        [CompilerGenerated]
        private void _Lambda___R59_3(object a0, MouseButtonEventArgs a1)
        {
            this.Btn3_Click();
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
            internal void _Lambda___4_0()
            {
                if (!ModMain.WaitingOurMsgBox.Any<ModMain.OurMsgBoxConverter>())
                {
                    ModAnimation.AniStart(ModAnimation.AaColor(ModMain.FrmMain.PanMsg, Panel.BackgroundProperty, new ModBase.OurColor(0.0, 0.0, 0.0, 0.0) - ModMain.FrmMain.PanMsg.Background, 200, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false), "", false);
                }
            }

            public static readonly OurMsgText._Closure___ _I = new OurMsgText._Closure___();

            public static ThreadStart _I4_0;
        }
    }
}
