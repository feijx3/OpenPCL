using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class MySearchBox : MyCard
    {
        public MySearchBox()
        {
            base.Loaded += this.MySearchBox_Loaded;
            base.KeyUp += this.MySearchBox_KeyUp;
            this.InitializeComponent();
        }

        public event MySearchBox.TextChangedEventHandler TextChanged;

        private void MySearchBox_Loaded(object sender, RoutedEventArgs e)
        {
            this.TextBox.Focus();
        }

        public string HintText
        {
            get
            {
                return this.TextBox.HintText;
            }
            set
            {
                this.TextBox.HintText = value;
            }
        }

        public string Text
        {
            get
            {
                return this.TextBox.Text;
            }
            set
            {
                this.TextBox.Text = value;
            }
        }

        private void Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.TextBox.Text))
            {
                ModAnimation.AniStart(ModAnimation.AaOpacity(this.BtnClear, -this.BtnClear.Opacity, 90, 0, null, false), "MySearchBox ClearBtn " + Conversions.ToString(this.Uuid), false);
                this.BtnClear.IsHitTestVisible = false;
            }
            else
            {
                ModAnimation.AniStart(ModAnimation.AaOpacity(this.BtnClear, 1.0 - this.BtnClear.Opacity, 90, 0, null, false), "MySearchBox ClearBtn " + Conversions.ToString(this.Uuid), false);
                this.BtnClear.IsHitTestVisible = true;
            }
            MySearchBox.TextChangedEventHandler textChangedEvent = this.TextChangedEvent;
            if (textChangedEvent != null)
            {
                textChangedEvent(RuntimeHelpers.GetObjectValue(sender), e);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            this.TextBox.Text = "";
            this.TextBox.Focus();
        }

        private void MySearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.RaiseCustomEvent();
            }
        }

        internal virtual MyTextBox TextBox
        {
            [CompilerGenerated]
            get
            {
                return this._TextBox;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                global::System.Windows.Controls.TextChangedEventHandler textChangedEventHandler = new global::System.Windows.Controls.TextChangedEventHandler(this.Text_TextChanged);
                MyTextBox myTextBox = this._TextBox;
                if (myTextBox != null)
                {
                    myTextBox.TextChanged -= textChangedEventHandler;
                }
                this._TextBox = value;
                myTextBox = this._TextBox;
                if (myTextBox != null)
                {
                    myTextBox.TextChanged += textChangedEventHandler;
                }
            }
        }

        internal virtual MyIconButton BtnClear
        {
            [CompilerGenerated]
            get
            {
                return this._BtnClear;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.BtnClear_Click);
                MyIconButton myIconButton = this._BtnClear;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnClear = value;
                myIconButton = this._BtnClear;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        public delegate void TextChangedEventHandler(object sender, EventArgs e);
    }
}
