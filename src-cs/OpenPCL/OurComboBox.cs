using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MeloongCore;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class OurComboBox : ComboBox, ISettingControl
    {
        public OurComboBox()
        {
            base.PreviewMouseLeftButtonDown += this.OurComboBox_PreviewMouseLeftButtonDown;
            base.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.OurComboBox_PreviewMouseLeftButtonUp);
            base.MouseLeave += new MouseEventHandler(this.OurComboBox_PreviewMouseLeftButtonUp);
            base.IsEnabledChanged += this._Lambda___R0_1;
            base.MouseEnter += this._Lambda___R0_2;
            base.MouseLeave += this._Lambda___R0_3;
            base.PreviewMouseLeftButtonDown += this._Lambda___R0_4;
            base.PreviewMouseLeftButtonUp += this._Lambda___R0_5;
            base.GotKeyboardFocus += this._Lambda___R0_6;
            base.DropDownOpened += this.OurComboBox_DropDownOpened;
            base.DropDownClosed += this.OurComboBox_DropDownClosed;
            this.TextChanged += this.OurComboBox_TextChanged;
            base.SelectionChanged += this.OurComboBox_SelectionChanged;
            base.SelectionChanged += this._Lambda___R0_7;
            this.Uuid = ModBase.GetUuid();
            this.HintText = "";
            this._Text = Conversions.ToString(base.SelectedItem);
            this.IsMouseDown = false;
            this.IsTextChanging = false;
        }

        public event OurComboBox.TextChangedEventHandler TextChanged
        {
            [CompilerGenerated]
            add
            {
                OurComboBox.TextChangedEventHandler textChangedEventHandler = this.TextChangedEvent;
                OurComboBox.TextChangedEventHandler textChangedEventHandler2;
                do
                {
                    textChangedEventHandler2 = textChangedEventHandler;
                    OurComboBox.TextChangedEventHandler textChangedEventHandler3 = (OurComboBox.TextChangedEventHandler)Delegate.Combine(textChangedEventHandler2, value);
                    textChangedEventHandler = Interlocked.CompareExchange<OurComboBox.TextChangedEventHandler>(ref this.TextChangedEvent, textChangedEventHandler3, textChangedEventHandler2);
                }
                while (textChangedEventHandler != textChangedEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurComboBox.TextChangedEventHandler textChangedEventHandler = this.TextChangedEvent;
                OurComboBox.TextChangedEventHandler textChangedEventHandler2;
                do
                {
                    textChangedEventHandler2 = textChangedEventHandler;
                    OurComboBox.TextChangedEventHandler textChangedEventHandler3 = (OurComboBox.TextChangedEventHandler)Delegate.Remove(textChangedEventHandler2, value);
                    textChangedEventHandler = Interlocked.CompareExchange<OurComboBox.TextChangedEventHandler>(ref this.TextChangedEvent, textChangedEventHandler3, textChangedEventHandler2);
                }
                while (textChangedEventHandler != textChangedEventHandler2);
            }
        }

        public string HintText
        {
            [CompilerGenerated]
            get
            {
                return this._HintText;
            }
            [CompilerGenerated]
            set
            {
                this._HintText = value;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (base.IsEditable)
            {
                try
                {
                    this.TextBox = (OurTextBox)base.Template.FindName("PART_EditableTextBox", this);
                    this.TextBox.AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler(this._Lambda___R12_8));
                    this.TextBox.ChangedEventList.Add(new RoutedEventHandler(this._Lambda___12_0));
                    this.TextBox.Tag = RuntimeHelpers.GetObjectValue(base.Tag);
                    if (Operators.CompareString(this.Text, "", false) == 0)
                    {
                        this.TextBox.Text = this._Text;
                    }
                    else
                    {
                        OurComboBox.TextChangedEventHandler textChangedEvent = this.TextChangedEvent;
                        if (textChangedEvent != null)
                        {
                            textChangedEvent(this, null);
                        }
                    }
                    if (this.HintText.Length > 0)
                    {
                        this.TextBox.HintText = this.HintText;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, string.Format("初始化可编辑文本框失败（{0}）", base.Name ?? ""), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurComboBox.vb");
                }
            }
        }

        public new string Text
        {
            get
            {
                string text;
                if (base.IsEditable)
                {
                    if (this.TextBox == null)
                    {
                        text = this._Text ?? "";
                    }
                    else
                    {
                        text = this.TextBox.Text ?? "";
                    }
                }
                else
                {
                    text = (base.SelectedItem ?? "").ToString();
                }
                return text;
            }
            set
            {
                if (!base.IsEditable)
                {
                    throw new NotSupportedException("该 ComboBox 不支持修改文本。");
                }
                if (Information.IsNothing(this.TextBox))
                {
                    this._Text = value;
                    return;
                }
                this.TextBox.Text = value;
            }
        }

        private void OurComboBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsMouseDown = true;
        }

        private void OurComboBox_PreviewMouseLeftButtonUp(object sender, EventArgs e)
        {
            this.IsMouseDown = false;
        }

        public void RefreshColor()
        {
            string text;
            string text2;
            int num;
            if (base.IsEnabled)
            {
                if (Conversions.ToBoolean(this.IsMouseDown || base.IsDropDownOpen || (base.IsEditable && Conversions.ToBoolean(NewLateBinding.LateGet(base.Template.FindName("PART_EditableTextBox", this), null, "IsFocused", new object[0], null, null, null)))))
                {
                    text = "ColorBrush3";
                    text2 = "ColorBrush7";
                    num = 10;
                }
                else if (base.IsMouseOver)
                {
                    text = "ColorBrush4";
                    text2 = "ColorBrush7";
                    num = 100;
                }
                else
                {
                    text = "ColorBrushBg0";
                    text2 = "ColorBrushHalfWhite";
                    num = 100;
                }
            }
            else
            {
                text = "ColorBrushGray5";
                text2 = "ColorBrushGray6";
                num = 200;
            }
            if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
            {
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaColor(this, Control.ForegroundProperty, text, num, 0, null, false),
                    ModAnimation.AaColor(this, Control.BackgroundProperty, text2, num, 0, null, false)
                }, "OurComboBox Color " + Conversions.ToString(this.Uuid), false);
                return;
            }
            ModAnimation.AniStop("OurComboBox Color " + Conversions.ToString(this.Uuid));
            base.SetResourceReference(Control.ForegroundProperty, text);
            base.SetResourceReference(Control.BackgroundProperty, text2);
        }

        private void OurComboBox_DropDownOpened(object sender, EventArgs e)
        {
            this.RealWidth = base.Width;
            base.Width = base.ActualWidth;
            try
            {
                ((Grid)base.Template.FindName("PanPopup", this)).Opacity = ModMain.FrmMain.Opacity;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "设置下拉框透明度失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurComboBox.vb");
            }
        }

        private void OurComboBox_DropDownClosed(object sender, EventArgs e)
        {
            base.Width = this.RealWidth;
        }

        private void OurComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.IsTextChanging && base.IsEditable && base.SelectedItem != null && Operators.CompareString(this.Text, base.SelectedItem.ToString(), false) != 0)
            {
                string text = this.Text;
                int selectionStart = this.TextBox.SelectionStart;
                this.IsTextChanging = true;
                base.SelectedItem = null;
                this.Text = text;
                this.TextBox.SelectionStart = selectionStart;
                this.IsTextChanging = false;
            }
        }

        public ContentPresenter ContentPresenter
        {
            get
            {
                return (ContentPresenter)base.Template.FindName("PART_Content", this);
            }
        }

        private void OurComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
            {
                this.RaiseCustomEvent();
            }
        }

        void ISettingControl.RefreshSetting(string NewValue)
        {
            OurComboBox._Closure___29_0 CS_locals1 = new OurComboBox._Closure___29_0();
            CS_locals1._VB_Local_NewValue = NewValue;
            DependencyObject dependencyObject = base.Items.OfType<DependencyObject>().FirstOrDefault<DependencyObject>(new Func<DependencyObject, bool>(CS_locals1._Lambda___0));
            if (dependencyObject != null)
            {
                base.SelectedItem = dependencyObject;
                return;
            }
            if ((double)base.Items.Count > Conversions.ToDouble(CS_locals1._VB_Local_NewValue))
            {
                base.SelectedIndex = Conversions.ToInteger(CS_locals1._VB_Local_NewValue);
                return;
            }
            Logger.Warn(string.Format("尝试显示设置 {0}，但没有找到设置值 {1} 的对应项", SettingService.GetKey(this), CS_locals1._VB_Local_NewValue), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurComboBox.vb");
            base.SelectedIndex = -1;
            base.SelectedItem = null;
        }

        string ISettingControl.GetCurrentSetting()
        {
            string text;
            if (base.SelectedItem == null)
            {
                text = null;
            }
            else if (base.SelectedItem is DependencyObject)
            {
                text = Conversions.ToString((object)SettingService.GetValue((DependencyObject)base.SelectedItem) ?? (object)base.SelectedIndex);
            }
            else
            {
                text = Conversions.ToString(base.SelectedIndex);
            }
            return text;
        }

        private void SaveSetting()
        {
            if (base.SelectedItem != null)
            {
                SettingService.SaveSetting(this);
            }
        }

        [CompilerGenerated]
        private void _Lambda___R0_1(object a0, DependencyPropertyChangedEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R0_2(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R0_3(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R0_4(object a0, MouseButtonEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R0_5(object a0, MouseButtonEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R0_6(object a0, KeyboardFocusChangedEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R0_7(object a0, SelectionChangedEventArgs a1)
        {
            this.SaveSetting();
        }

        [CompilerGenerated]
        private void _Lambda___R12_8(object a0, RoutedEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___12_0(object sender, RoutedEventArgs e)
        {
            OurComboBox.TextChangedEventHandler textChangedEvent = this.TextChangedEvent;
            if (textChangedEvent != null)
            {
                textChangedEvent(RuntimeHelpers.GetObjectValue(sender), (TextChangedEventArgs)e);
            }
        }

        [CompilerGenerated]
        private OurComboBox.TextChangedEventHandler TextChangedEvent;

        public int Uuid;

        private OurTextBox TextBox;

        [CompilerGenerated]
        private string _HintText;

        private string _Text;

        private bool IsMouseDown;

        private double RealWidth;

        private bool IsTextChanging;

        public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);

        [CompilerGenerated]
        private sealed class _Closure___29_0
        {
            public _Closure___29_0()
            {
            }

            internal bool _Lambda___0(DependencyObject i)
            {
                return ModBase.Val(RuntimeHelpers.GetObjectValue((object)SettingService.GetValue(i) ?? (object)(-1))) == Conversions.ToDouble(this._VB_Local_NewValue);
            }

            public string _VB_Local_NewValue;
        }
    }
}
