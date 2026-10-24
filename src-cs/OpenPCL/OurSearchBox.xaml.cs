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
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class OurSearchBox : OurCard
    {
        public OurSearchBox()
        {
            base.Loaded += this.OurSearchBox_Loaded;
            base.KeyUp += this.OurSearchBox_KeyUp;
            this.InitializeComponent();
            this.TextBox_WithEvents = this.TextBox;
            this.BtnClear_WithEvents = this.BtnClear;
        }

        public event OurSearchBox.TextChangedEventHandler TextChanged
        {
            [CompilerGenerated]
            add
            {
                OurSearchBox.TextChangedEventHandler textChangedEventHandler = this.TextChangedEvent;
                OurSearchBox.TextChangedEventHandler textChangedEventHandler2;
                do
                {
                    textChangedEventHandler2 = textChangedEventHandler;
                    OurSearchBox.TextChangedEventHandler textChangedEventHandler3 = (OurSearchBox.TextChangedEventHandler)Delegate.Combine(textChangedEventHandler2, value);
                    textChangedEventHandler = Interlocked.CompareExchange<OurSearchBox.TextChangedEventHandler>(ref this.TextChangedEvent, textChangedEventHandler3, textChangedEventHandler2);
                }
                while (textChangedEventHandler != textChangedEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurSearchBox.TextChangedEventHandler textChangedEventHandler = this.TextChangedEvent;
                OurSearchBox.TextChangedEventHandler textChangedEventHandler2;
                do
                {
                    textChangedEventHandler2 = textChangedEventHandler;
                    OurSearchBox.TextChangedEventHandler textChangedEventHandler3 = (OurSearchBox.TextChangedEventHandler)Delegate.Remove(textChangedEventHandler2, value);
                    textChangedEventHandler = Interlocked.CompareExchange<OurSearchBox.TextChangedEventHandler>(ref this.TextChangedEvent, textChangedEventHandler3, textChangedEventHandler2);
                }
                while (textChangedEventHandler != textChangedEventHandler2);
            }
        }

        private void OurSearchBox_Loaded(object sender, RoutedEventArgs e)
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
                ModAnimation.AniStart(ModAnimation.AaOpacity(this.BtnClear, -this.BtnClear.Opacity, 90, 0, null, false), "OurSearchBox ClearBtn " + Conversions.ToString(this.Uuid), false);
                this.BtnClear.IsHitTestVisible = false;
            }
            else
            {
                ModAnimation.AniStart(ModAnimation.AaOpacity(this.BtnClear, 1.0 - this.BtnClear.Opacity, 90, 0, null, false), "OurSearchBox ClearBtn " + Conversions.ToString(this.Uuid), false);
                this.BtnClear.IsHitTestVisible = true;
            }
            OurSearchBox.TextChangedEventHandler textChangedEvent = this.TextChangedEvent;
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

        private void OurSearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.RaiseCustomEvent();
            }
        }

        internal virtual OurTextBox TextBox_WithEvents
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
                OurTextBox ourTextBox = this._TextBox;
                if (ourTextBox != null)
                {
                    ourTextBox.TextChanged -= textChangedEventHandler;
                }
                this._TextBox = value;
                ourTextBox = this._TextBox;
                if (ourTextBox != null)
                {
                    ourTextBox.TextChanged += textChangedEventHandler;
                }
            }
        }

        internal virtual OurIconButton BtnClear_WithEvents
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
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this.BtnClear_Click);
                OurIconButton ourIconButton = this._BtnClear;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                this._BtnClear = value;
                ourIconButton = this._BtnClear;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private OurSearchBox.TextChangedEventHandler TextChangedEvent;

        [CompilerGenerated]
        [AccessedThroughProperty("TextBox")]
        private OurTextBox _TextBox;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnClear")]
        private OurIconButton _BtnClear;

        public delegate void TextChangedEventHandler(object sender, EventArgs e);
    }
}
