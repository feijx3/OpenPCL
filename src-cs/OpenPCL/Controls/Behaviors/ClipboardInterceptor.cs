using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;
using ThrottleDebounce;

namespace PCL.Controls.Behaviors
{
    public sealed class ClipboardInterceptor
    {
        // Note: this type is marked as 'beforefieldinit'.
        static ClipboardInterceptor()
        {
        }

        private ClipboardInterceptor()
        {
        }

        public static void SetEnableSafeClipboard(DependencyObject element, bool value)
        {
            element.SetValue(ClipboardInterceptor.EnableSafeClipboardProperty, value);
        }

        public static bool GetEnableSafeClipboard(DependencyObject element)
        {
            return Conversions.ToBoolean(element.GetValue(ClipboardInterceptor.EnableSafeClipboardProperty));
        }

        private static void OnEnableSafeClipboardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is global::System.Windows.Controls.TextBox && Conversions.ToBoolean(e.NewValue))
            {
                ClipboardInterceptor.AddCommandBindingsToTextBox((global::System.Windows.Controls.TextBox)d);
                return;
            }
            if (d is global::System.Windows.Controls.RichTextBox && Conversions.ToBoolean(e.NewValue))
            {
                ClipboardInterceptor.AddCommandBindingsToRichTextBox((global::System.Windows.Controls.RichTextBox)d);
                return;
            }
            if (d is global::System.Windows.Controls.DataGrid && Conversions.ToBoolean(e.NewValue))
            {
                ClipboardInterceptor.AddCommandBindingsToDataGrid((global::System.Windows.Controls.DataGrid)d);
            }
        }

        private static void AddCommandBindingsToTextBox(global::System.Windows.Controls.TextBox tb)
        {
            tb.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, new ExecutedRoutedEventHandler(ClipboardInterceptor.OnCopyTextBox)));
            tb.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, new ExecutedRoutedEventHandler(ClipboardInterceptor.OnCutTextBox), new CanExecuteRoutedEventHandler(ClipboardInterceptor.CanEditTextBox)));
            tb.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, new ExecutedRoutedEventHandler(ClipboardInterceptor.OnPasteTextBox), new CanExecuteRoutedEventHandler(ClipboardInterceptor.CanEditTextBox)));
        }

        private static void AddCommandBindingsToRichTextBox(global::System.Windows.Controls.RichTextBox rtb)
        {
            rtb.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, new ExecutedRoutedEventHandler(ClipboardInterceptor.OnCopyRichTextBox)));
            rtb.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, new ExecutedRoutedEventHandler(ClipboardInterceptor.OnCutRichTextBox)));
            rtb.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, new ExecutedRoutedEventHandler(ClipboardInterceptor.OnPasteRichTextBox)));
        }

        private static void AddCommandBindingsToDataGrid(global::System.Windows.Controls.DataGrid dg)
        {
            dg.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, new ExecutedRoutedEventHandler(ClipboardInterceptor.OnCopyDataGrid)));
        }

        private static void SetClipboardText(string text)
        {
            global::System.Windows.Forms.Clipboard.SetDataObject(text, false, 10, 100);
            try
            {
                global::System.Windows.Clipboard.Flush();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, string.Format("剪贴板持久化失败（错误码：0x{0:X8}）", ex.HResult), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\Behaviors\\ClipboardInterceptor.vb");
            }
        }

        private static void OnCopyTextBox(object sender, ExecutedRoutedEventArgs e)
        {
            global::System.Windows.Controls.TextBox textBox = sender as global::System.Windows.Controls.TextBox;
            if (textBox != null && textBox.SelectionLength > 0)
            {
                try
                {
                    ClipboardInterceptor.SetClipboardText(textBox.SelectedText);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, string.Format("复制失败，请稍后再试（错误码：0x{0:X8}）", ex.HResult), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\Behaviors\\ClipboardInterceptor.vb");
                }
                e.Handled = true;
            }
        }

        private static void CanEditTextBox(object sender, CanExecuteRoutedEventArgs e)
        {
            global::System.Windows.Controls.TextBox textBox = sender as global::System.Windows.Controls.TextBox;
            e.CanExecute = textBox != null && !textBox.IsReadOnly && (e.Command != ApplicationCommands.Cut || textBox.SelectionLength > 0);
            e.Handled = true;
        }

        private static void OnCutTextBox(object sender, ExecutedRoutedEventArgs e)
        {
            global::System.Windows.Controls.TextBox textBox = sender as global::System.Windows.Controls.TextBox;
            e.Handled = true;
            if (textBox != null && !textBox.IsReadOnly && textBox.SelectionLength > 0)
            {
                try
                {
                    ClipboardInterceptor.SetClipboardText(textBox.SelectedText);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, string.Format("剪切失败，请稍后再试（错误码：0x{0:X8}）", ex.HResult), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\Behaviors\\ClipboardInterceptor.vb");
                    return;
                }
                textBox.SelectedText = string.Empty;
            }
        }

        private static void OnPasteTextBox(object sender, ExecutedRoutedEventArgs e)
        {
            global::System.Windows.Controls.TextBox textBox = sender as global::System.Windows.Controls.TextBox;
            e.Handled = true;
            if (textBox != null && !textBox.IsReadOnly)
            {
                try
                {
                    string text = Retrier.Attempt<string>((ClipboardInterceptor._Closure___._I13_0 == null) ? (ClipboardInterceptor._Closure___._I13_0 = new Func<int, string>(ClipboardInterceptor._Closure___._I._Lambda___13_0)) : ClipboardInterceptor._Closure___._I13_0, new uint?(10U), (ClipboardInterceptor._Closure___._IR13_1 == null) ? (ClipboardInterceptor._Closure___._IR13_1 = new Func<int, TimeSpan>(ClipboardInterceptor._Closure___._I._Lambda___R13_1)) : ClipboardInterceptor._Closure___._IR13_1, null, null, default(CancellationToken));
                    if (text != null)
                    {
                        if (!textBox.AcceptsReturn)
                        {
                            text = text.ReplaceLineEndings(" ", true);
                        }
                        int selectionStart = textBox.SelectionStart;
                        textBox.SelectedText = text;
                        textBox.CaretIndex = checked(selectionStart + text.Length);
                        textBox.SelectionLength = 0;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, string.Format("粘贴失败，请稍后再试（错误码：0x{0:X8}）", ex.HResult), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\Behaviors\\ClipboardInterceptor.vb");
                }
            }
        }

        private static void OnCopyRichTextBox(object sender, ExecutedRoutedEventArgs e)
        {
            global::System.Windows.Controls.RichTextBox richTextBox = sender as global::System.Windows.Controls.RichTextBox;
            if (richTextBox != null)
            {
                TextRange textRange = new TextRange(richTextBox.Selection.Start, richTextBox.Selection.End);
                if (!string.IsNullOrEmpty(textRange.Text))
                {
                    try
                    {
                        ClipboardInterceptor.SetClipboardText(textRange.Text);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, string.Format("复制失败，请稍后再试（错误码：0x{0:X8}）", ex.HResult), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\Behaviors\\ClipboardInterceptor.vb");
                    }
                    e.Handled = true;
                }
            }
        }

        private static void OnCutRichTextBox(object sender, ExecutedRoutedEventArgs e)
        {
            global::System.Windows.Controls.RichTextBox richTextBox = sender as global::System.Windows.Controls.RichTextBox;
            if (richTextBox != null)
            {
                TextRange textRange = new TextRange(richTextBox.Selection.Start, richTextBox.Selection.End);
                if (!string.IsNullOrEmpty(textRange.Text))
                {
                    e.Handled = true;
                    try
                    {
                        ClipboardInterceptor.SetClipboardText(textRange.Text);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, string.Format("剪切失败，请稍后再试（错误码：0x{0:X8}）", ex.HResult), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\Behaviors\\ClipboardInterceptor.vb");
                        return;
                    }
                    textRange.Text = string.Empty;
                }
            }
        }

        private static void OnPasteRichTextBox(object sender, ExecutedRoutedEventArgs e)
        {
            global::System.Windows.Controls.RichTextBox richTextBox = sender as global::System.Windows.Controls.RichTextBox;
            if (richTextBox != null)
            {
                try
                {
                    string text = Retrier.Attempt<string>((ClipboardInterceptor._Closure___._I16_0 == null) ? (ClipboardInterceptor._Closure___._I16_0 = new Func<int, string>(ClipboardInterceptor._Closure___._I._Lambda___16_0)) : ClipboardInterceptor._Closure___._I16_0, new uint?(2U), (ClipboardInterceptor._Closure___._IR16_2 == null) ? (ClipboardInterceptor._Closure___._IR16_2 = new Func<int, TimeSpan>(ClipboardInterceptor._Closure___._I._Lambda___R16_2)) : ClipboardInterceptor._Closure___._IR16_2, null, null, default(CancellationToken));
                    if (text == null)
                    {
                        return;
                    }
                    TextSelection selection = richTextBox.Selection;
                    selection.Text = text;
                    TextPointer end = selection.End;
                    richTextBox.CaretPosition = end;
                    richTextBox.Selection.Select(end, end);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, string.Format("粘贴失败，请稍后再试（错误码：0x{0:X8}）", ex.HResult), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\Behaviors\\ClipboardInterceptor.vb");
                }
                e.Handled = true;
            }
        }

        private static void OnCopyDataGrid(object sender, ExecutedRoutedEventArgs e)
        {
            global::System.Windows.Controls.DataGrid dataGrid = sender as global::System.Windows.Controls.DataGrid;
            if (dataGrid != null && dataGrid.SelectedCells != null && dataGrid.SelectedCells.Count != 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                IEnumerable<IGrouping<object, DataGridCellInfo>> enumerable = dataGrid.SelectedCells.GroupBy<DataGridCellInfo, object>((ClipboardInterceptor._Closure___._I17_0 == null) ? (ClipboardInterceptor._Closure___._I17_0 = new Func<DataGridCellInfo, object>(ClipboardInterceptor._Closure___._I._Lambda___17_0)) : ClipboardInterceptor._Closure___._I17_0);
                try
                {
                    foreach (IGrouping<object, DataGridCellInfo> grouping in enumerable)
                    {
                        string text = string.Join("\t", grouping.Select<DataGridCellInfo, string>((ClipboardInterceptor._Closure___._I17_1 == null) ? (ClipboardInterceptor._Closure___._I17_1 = new Func<DataGridCellInfo, string>(ClipboardInterceptor._Closure___._I._Lambda___17_1)) : ClipboardInterceptor._Closure___._I17_1));
                        stringBuilder.AppendLine(text);
                    }
                }
                finally
                {
                    IEnumerator<IGrouping<object, DataGridCellInfo>> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                string text2 = stringBuilder.ToString().TrimEnd(new char[] { '\r', '\n' });
                try
                {
                    ClipboardInterceptor.SetClipboardText(text2);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, string.Format("复制失败，请稍后再试（错误码：0x{0:X8}）", ex.HResult), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\Behaviors\\ClipboardInterceptor.vb");
                }
                e.Handled = true;
            }
        }

        public static readonly DependencyProperty EnableSafeClipboardProperty = DependencyProperty.RegisterAttached("EnableSafeClipboard", typeof(bool), typeof(ClipboardInterceptor), new PropertyMetadata(false, new PropertyChangedCallback(ClipboardInterceptor.OnEnableSafeClipboardChanged)));

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

            internal string _Lambda___13_0(int Attempt)
            {
                if (!global::System.Windows.Forms.Clipboard.ContainsText())
                {
                    return null;
                }
                return global::System.Windows.Forms.Clipboard.GetText();
            }

            internal TimeSpan _Lambda___R13_1(int a0)
            {
                return ((ClipboardInterceptor._Closure___._I13_1 == null) ? (ClipboardInterceptor._Closure___._I13_1 = new VB_AnonymousDelegate_0<TimeSpan>(ClipboardInterceptor._Closure___._I._Lambda___13_1)) : ClipboardInterceptor._Closure___._I13_1)();
            }

            internal TimeSpan _Lambda___13_1()
            {
                return TimeSpan.FromMilliseconds(100.0);
            }

            internal string _Lambda___16_0(int Attempt)
            {
                if (!global::System.Windows.Forms.Clipboard.ContainsText())
                {
                    return null;
                }
                return global::System.Windows.Forms.Clipboard.GetText();
            }

            internal TimeSpan _Lambda___R16_2(int a0)
            {
                return ((ClipboardInterceptor._Closure___._I16_1 == null) ? (ClipboardInterceptor._Closure___._I16_1 = new VB_AnonymousDelegate_0<TimeSpan>(ClipboardInterceptor._Closure___._I._Lambda___16_1)) : ClipboardInterceptor._Closure___._I16_1)();
            }

            internal TimeSpan _Lambda___16_1()
            {
                return TimeSpan.FromMilliseconds(200.0);
            }

            internal object _Lambda___17_0(DataGridCellInfo c)
            {
                return c.Item;
            }

            internal string _Lambda___17_1(DataGridCellInfo cell)
            {
                TextBlock textBlock = cell.Column.GetCellContent(RuntimeHelpers.GetObjectValue(cell.Item)) as TextBlock;
                if (textBlock == null)
                {
                    return "";
                }
                return textBlock.Text;
            }

            public static readonly ClipboardInterceptor._Closure___ _I = new ClipboardInterceptor._Closure___();

            public static Func<int, string> _I13_0;

            public static VB_AnonymousDelegate_0<TimeSpan> _I13_1;

            public static Func<int, TimeSpan> _IR13_1;

            public static Func<int, string> _I16_0;

            public static VB_AnonymousDelegate_0<TimeSpan> _I16_1;

            public static Func<int, TimeSpan> _IR16_2;

            public static Func<DataGridCellInfo, object> _I17_0;

            public static Func<DataGridCellInfo, string> _I17_1;
        }
    }
}
