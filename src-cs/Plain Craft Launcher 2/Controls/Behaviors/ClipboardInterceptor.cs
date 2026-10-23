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
                    string text = Retrier.Attempt<string>((ClipboardInterceptor._Closure$__.$I13-0 == null) ? (ClipboardInterceptor._Closure$__.$I13-0 = delegate(int Attempt)
                    {
                        if (!global::System.Windows.Forms.Clipboard.ContainsText())
                        {
                            return null;
                        }
                        return global::System.Windows.Forms.Clipboard.GetText();
                    }) : ClipboardInterceptor._Closure$__.$I13-0, new uint?(10U), (ClipboardInterceptor._Closure$__.$IR13-1 == null) ? (ClipboardInterceptor._Closure$__.$IR13-1 = (int a0) => ((ClipboardInterceptor._Closure$__.$I13-1 == null) ? (ClipboardInterceptor._Closure$__.$I13-1 = () => TimeSpan.FromMilliseconds(100.0)) : ClipboardInterceptor._Closure$__.$I13-1)()) : ClipboardInterceptor._Closure$__.$IR13-1, null, null, default(CancellationToken));
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
                    string text = Retrier.Attempt<string>((ClipboardInterceptor._Closure$__.$I16-0 == null) ? (ClipboardInterceptor._Closure$__.$I16-0 = delegate(int Attempt)
                    {
                        if (!global::System.Windows.Forms.Clipboard.ContainsText())
                        {
                            return null;
                        }
                        return global::System.Windows.Forms.Clipboard.GetText();
                    }) : ClipboardInterceptor._Closure$__.$I16-0, new uint?(2U), (ClipboardInterceptor._Closure$__.$IR16-2 == null) ? (ClipboardInterceptor._Closure$__.$IR16-2 = (int a0) => ((ClipboardInterceptor._Closure$__.$I16-1 == null) ? (ClipboardInterceptor._Closure$__.$I16-1 = () => TimeSpan.FromMilliseconds(200.0)) : ClipboardInterceptor._Closure$__.$I16-1)()) : ClipboardInterceptor._Closure$__.$IR16-2, null, null, default(CancellationToken));
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
                IEnumerable<IGrouping<object, DataGridCellInfo>> enumerable = dataGrid.SelectedCells.GroupBy<DataGridCellInfo, object>((ClipboardInterceptor._Closure$__.$I17-0 == null) ? (ClipboardInterceptor._Closure$__.$I17-0 = (DataGridCellInfo c) => c.Item) : ClipboardInterceptor._Closure$__.$I17-0);
                try
                {
                    foreach (IGrouping<object, DataGridCellInfo> grouping in enumerable)
                    {
                        string text = string.Join("\t", grouping.Select<DataGridCellInfo, string>((ClipboardInterceptor._Closure$__.$I17-1 == null) ? (ClipboardInterceptor._Closure$__.$I17-1 = delegate(DataGridCellInfo cell)
                        {
                            TextBlock textBlock = cell.Column.GetCellContent(RuntimeHelpers.GetObjectValue(cell.Item)) as TextBlock;
                            if (textBlock == null)
                            {
                                return "";
                            }
                            return textBlock.Text;
                        }) : ClipboardInterceptor._Closure$__.$I17-1));
                        stringBuilder.AppendLine(text);
                    }
                }
                finally
                {
                    IEnumerator<IGrouping<object, DataGridCellInfo>> enumerator;
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
    }
}
