Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Forms
Imports System.Windows.Input
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices
Imports ThrottleDebounce

Namespace PCL.Controls.Behaviors
    Public NotInheritable Class ClipboardInterceptor
        Private Sub New()
        End Sub

        Public Shared Sub SetEnableSafeClipboard(element As DependencyObject, value As Boolean)
            element.SetValue(ClipboardInterceptor.EnableSafeClipboardProperty, value)
        End Sub

        Public Shared Function GetEnableSafeClipboard(element As DependencyObject) As Boolean
            Return Conversions.ToBoolean(element.GetValue(ClipboardInterceptor.EnableSafeClipboardProperty))
        End Function

        Private Shared Sub OnEnableSafeClipboardChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            If TypeOf d Is Global.System.Windows.Controls.TextBox AndAlso Conversions.ToBoolean(e.NewValue) Then
                ClipboardInterceptor.AddCommandBindingsToTextBox(CType(d, Global.System.Windows.Controls.TextBox))
                Return
            End If
            If TypeOf d Is Global.System.Windows.Controls.RichTextBox AndAlso Conversions.ToBoolean(e.NewValue) Then
                ClipboardInterceptor.AddCommandBindingsToRichTextBox(CType(d, Global.System.Windows.Controls.RichTextBox))
                Return
            End If
            If TypeOf d Is Global.System.Windows.Controls.DataGrid AndAlso Conversions.ToBoolean(e.NewValue) Then
                ClipboardInterceptor.AddCommandBindingsToDataGrid(CType(d, Global.System.Windows.Controls.DataGrid))
            End If
        End Sub

        Private Shared Sub AddCommandBindingsToTextBox(tb As Global.System.Windows.Controls.TextBox)
            tb.CommandBindings.Add(New CommandBinding(ApplicationCommands.Copy, AddressOf ClipboardInterceptor.OnCopyTextBox))
            tb.CommandBindings.Add(New CommandBinding(ApplicationCommands.Cut, AddressOf ClipboardInterceptor.OnCutTextBox, AddressOf ClipboardInterceptor.CanEditTextBox))
            tb.CommandBindings.Add(New CommandBinding(ApplicationCommands.Paste, AddressOf ClipboardInterceptor.OnPasteTextBox, AddressOf ClipboardInterceptor.CanEditTextBox))
        End Sub

        Private Shared Sub AddCommandBindingsToRichTextBox(rtb As Global.System.Windows.Controls.RichTextBox)
            rtb.CommandBindings.Add(New CommandBinding(ApplicationCommands.Copy, AddressOf ClipboardInterceptor.OnCopyRichTextBox))
            rtb.CommandBindings.Add(New CommandBinding(ApplicationCommands.Cut, AddressOf ClipboardInterceptor.OnCutRichTextBox))
            rtb.CommandBindings.Add(New CommandBinding(ApplicationCommands.Paste, AddressOf ClipboardInterceptor.OnPasteRichTextBox))
        End Sub

        Private Shared Sub AddCommandBindingsToDataGrid(dg As Global.System.Windows.Controls.DataGrid)
            dg.CommandBindings.Add(New CommandBinding(ApplicationCommands.Copy, AddressOf ClipboardInterceptor.OnCopyDataGrid))
        End Sub

        Private Shared Sub SetClipboardText(text As String)
            Global.System.Windows.Forms.Clipboard.SetDataObject(text, False, 10, 100)
            Try
                Global.System.Windows.Clipboard.Flush()
            Catch ex As Exception
                Logger.Warn(ex, String.Format("剪贴板持久化失败（错误码：0x{0:X8}）", ex.HResult), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\Behaviors\ClipboardInterceptor.vb")
            End Try
        End Sub

        Private Shared Sub OnCopyTextBox(sender As Object, e As ExecutedRoutedEventArgs)
            Dim textBox As Global.System.Windows.Controls.TextBox = TryCast(sender, Global.System.Windows.Controls.TextBox)
            If textBox IsNot Nothing AndAlso textBox.SelectionLength > 0 Then
                Try
                    ClipboardInterceptor.SetClipboardText(textBox.SelectedText)
                Catch ex As Exception
                    Logger.Warn(ex, String.Format("复制失败，请稍后再试（错误码：0x{0:X8}）", ex.HResult), LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\Behaviors\ClipboardInterceptor.vb")
                End Try
                e.Handled = True
            End If
        End Sub

        Private Shared Sub CanEditTextBox(sender As Object, e As CanExecuteRoutedEventArgs)
            Dim textBox As Global.System.Windows.Controls.TextBox = TryCast(sender, Global.System.Windows.Controls.TextBox)
            e.CanExecute = textBox IsNot Nothing AndAlso Not textBox.IsReadOnly AndAlso (e.Command IsNot ApplicationCommands.Cut OrElse textBox.SelectionLength > 0)
            e.Handled = True
        End Sub

        Private Shared Sub OnCutTextBox(sender As Object, e As ExecutedRoutedEventArgs)
            Dim textBox As Global.System.Windows.Controls.TextBox = TryCast(sender, Global.System.Windows.Controls.TextBox)
            e.Handled = True
            If textBox IsNot Nothing AndAlso Not textBox.IsReadOnly AndAlso textBox.SelectionLength > 0 Then
                Try
                    ClipboardInterceptor.SetClipboardText(textBox.SelectedText)
                Catch ex As Exception
                    Logger.Warn(ex, String.Format("剪切失败，请稍后再试（错误码：0x{0:X8}）", ex.HResult), LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\Behaviors\ClipboardInterceptor.vb")
                    Return
                End Try
                textBox.SelectedText = String.Empty
            End If
        End Sub

        Private Shared Sub OnPasteTextBox(sender As Object, e As ExecutedRoutedEventArgs)
            Dim textBox As Global.System.Windows.Controls.TextBox = TryCast(sender, Global.System.Windows.Controls.TextBox)
            e.Handled = True
            If textBox IsNot Nothing AndAlso Not textBox.IsReadOnly Then
                Try
                    Dim func As Func(Of Integer, String)
                    If ClipboardInterceptor._Closure$__.$I13-0 IsNot Nothing Then
                        func = ClipboardInterceptor._Closure$__.$I13-0
                    Else
                        Dim func2 As Func(Of Integer, String) = Function(Attempt As Integer)
                            If Not Global.System.Windows.Forms.Clipboard.ContainsText() Then
                                Return Nothing
                            End If
                            Return Global.System.Windows.Forms.Clipboard.GetText()
                        End Function
                        func = func2
                        ClipboardInterceptor._Closure$__.$I13-0 = func2
                    End If
                    Dim num As UInteger? = New UInteger?(10UI)
                    Dim func3 As Func(Of Integer, TimeSpan)
                    If ClipboardInterceptor._Closure$__.$IR13-1 IsNot Nothing Then
                        func3 = ClipboardInterceptor._Closure$__.$IR13-1
                    Else
                        Dim func4 As Func(Of Integer, TimeSpan) = Function(a0 As Integer)
                            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_0(Of TimeSpan)
                            If ClipboardInterceptor._Closure$__.$I13-1 IsNot Nothing Then
                                vb$AnonymousDelegate_ = ClipboardInterceptor._Closure$__.$I13-1
                            Else
                                Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_0(Of TimeSpan) = Function() TimeSpan.FromMilliseconds(100.0)
                                vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                                ClipboardInterceptor._Closure$__.$I13-1 = vb$AnonymousDelegate_2
                            End If
                            Return vb$AnonymousDelegate_()
                        End Function
                        func3 = func4
                        ClipboardInterceptor._Closure$__.$IR13-1 = func4
                    End If
                    Dim text As String = Retrier.Attempt(Of String)(func, num, func3, Nothing, Nothing, Nothing)
                    If text IsNot Nothing Then
                        If Not textBox.AcceptsReturn Then
                            text = text.ReplaceLineEndings(" ", True)
                        End If
                        Dim selectionStart As Integer = textBox.SelectionStart
                        textBox.SelectedText = text
                        textBox.CaretIndex = selectionStart + text.Length
                        textBox.SelectionLength = 0
                    End If
                Catch ex As Exception
                    Logger.Warn(ex, String.Format("粘贴失败，请稍后再试（错误码：0x{0:X8}）", ex.HResult), LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\Behaviors\ClipboardInterceptor.vb")
                End Try
            End If
        End Sub

        Private Shared Sub OnCopyRichTextBox(sender As Object, e As ExecutedRoutedEventArgs)
            Dim richTextBox As Global.System.Windows.Controls.RichTextBox = TryCast(sender, Global.System.Windows.Controls.RichTextBox)
            If richTextBox IsNot Nothing Then
                Dim textRange As TextRange = New TextRange(richTextBox.Selection.Start, richTextBox.Selection.[End])
                If Not String.IsNullOrEmpty(textRange.Text) Then
                    Try
                        ClipboardInterceptor.SetClipboardText(textRange.Text)
                    Catch ex As Exception
                        Logger.Warn(ex, String.Format("复制失败，请稍后再试（错误码：0x{0:X8}）", ex.HResult), LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\Behaviors\ClipboardInterceptor.vb")
                    End Try
                    e.Handled = True
                End If
            End If
        End Sub

        Private Shared Sub OnCutRichTextBox(sender As Object, e As ExecutedRoutedEventArgs)
            Dim richTextBox As Global.System.Windows.Controls.RichTextBox = TryCast(sender, Global.System.Windows.Controls.RichTextBox)
            If richTextBox IsNot Nothing Then
                Dim textRange As TextRange = New TextRange(richTextBox.Selection.Start, richTextBox.Selection.[End])
                If Not String.IsNullOrEmpty(textRange.Text) Then
                    e.Handled = True
                    Try
                        ClipboardInterceptor.SetClipboardText(textRange.Text)
                    Catch ex As Exception
                        Logger.Warn(ex, String.Format("剪切失败，请稍后再试（错误码：0x{0:X8}）", ex.HResult), LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\Behaviors\ClipboardInterceptor.vb")
                        Return
                    End Try
                    textRange.Text = String.Empty
                End If
            End If
        End Sub

        Private Shared Sub OnPasteRichTextBox(sender As Object, e As ExecutedRoutedEventArgs)
            Dim richTextBox As Global.System.Windows.Controls.RichTextBox = TryCast(sender, Global.System.Windows.Controls.RichTextBox)
            If richTextBox IsNot Nothing Then
                Try
                    Dim func As Func(Of Integer, String)
                    If ClipboardInterceptor._Closure$__.$I16-0 IsNot Nothing Then
                        func = ClipboardInterceptor._Closure$__.$I16-0
                    Else
                        Dim func2 As Func(Of Integer, String) = Function(Attempt As Integer)
                            If Not Global.System.Windows.Forms.Clipboard.ContainsText() Then
                                Return Nothing
                            End If
                            Return Global.System.Windows.Forms.Clipboard.GetText()
                        End Function
                        func = func2
                        ClipboardInterceptor._Closure$__.$I16-0 = func2
                    End If
                    Dim num As UInteger? = New UInteger?(2UI)
                    Dim func3 As Func(Of Integer, TimeSpan)
                    If ClipboardInterceptor._Closure$__.$IR16-2 IsNot Nothing Then
                        func3 = ClipboardInterceptor._Closure$__.$IR16-2
                    Else
                        Dim func4 As Func(Of Integer, TimeSpan) = Function(a0 As Integer)
                            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_0(Of TimeSpan)
                            If ClipboardInterceptor._Closure$__.$I16-1 IsNot Nothing Then
                                vb$AnonymousDelegate_ = ClipboardInterceptor._Closure$__.$I16-1
                            Else
                                Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_0(Of TimeSpan) = Function() TimeSpan.FromMilliseconds(200.0)
                                vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                                ClipboardInterceptor._Closure$__.$I16-1 = vb$AnonymousDelegate_2
                            End If
                            Return vb$AnonymousDelegate_()
                        End Function
                        func3 = func4
                        ClipboardInterceptor._Closure$__.$IR16-2 = func4
                    End If
                    Dim text As String = Retrier.Attempt(Of String)(func, num, func3, Nothing, Nothing, Nothing)
                    If text Is Nothing Then
                        Return
                    End If
                    Dim selection As TextSelection = richTextBox.Selection
                    selection.Text = text
                    Dim [end] As TextPointer = selection.[End]
                    richTextBox.CaretPosition = [end]
                    richTextBox.Selection.[Select]([end], [end])
                Catch ex As Exception
                    Logger.Warn(ex, String.Format("粘贴失败，请稍后再试（错误码：0x{0:X8}）", ex.HResult), LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\Behaviors\ClipboardInterceptor.vb")
                End Try
                e.Handled = True
            End If
        End Sub

        Private Shared Sub OnCopyDataGrid(sender As Object, e As ExecutedRoutedEventArgs)
            Dim dataGrid As Global.System.Windows.Controls.DataGrid = TryCast(sender, Global.System.Windows.Controls.DataGrid)
            If dataGrid IsNot Nothing AndAlso dataGrid.SelectedCells IsNot Nothing AndAlso dataGrid.SelectedCells.Count <> 0 Then
                Dim stringBuilder As StringBuilder = New StringBuilder()
                Dim selectedCells As IEnumerable(Of DataGridCellInfo) = dataGrid.SelectedCells
                Dim func As Func(Of DataGridCellInfo, Object)
                If ClipboardInterceptor._Closure$__.$I17-0 IsNot Nothing Then
                    func = ClipboardInterceptor._Closure$__.$I17-0
                Else
                    Dim func2 As Func(Of DataGridCellInfo, Object) = Function(c As DataGridCellInfo) c.Item
                    func = func2
                    ClipboardInterceptor._Closure$__.$I17-0 = func2
                End If
                Dim enumerable As IEnumerable(Of IGrouping(Of Object, DataGridCellInfo)) = selectedCells.GroupBy(func)
                Try
                    For Each grouping As IGrouping(Of Object, DataGridCellInfo) In enumerable
                        Dim text As String = vbTab
                        Dim enumerable2 As IEnumerable(Of DataGridCellInfo) = grouping
                        Dim func3 As Func(Of DataGridCellInfo, String)
                        If ClipboardInterceptor._Closure$__.$I17-1 IsNot Nothing Then
                            func3 = ClipboardInterceptor._Closure$__.$I17-1
                        Else
                            Dim func4 As Func(Of DataGridCellInfo, String) = Function(cell As DataGridCellInfo)
                                Dim textBlock As TextBlock = TryCast(cell.Column.GetCellContent(RuntimeHelpers.GetObjectValue(cell.Item)), TextBlock)
                                If textBlock Is Nothing Then
                                    Return ""
                                End If
                                Return textBlock.Text
                            End Function
                            func3 = func4
                            ClipboardInterceptor._Closure$__.$I17-1 = func4
                        End If
                        Dim text2 As String = String.Join(text, enumerable2.[Select](func3))
                        stringBuilder.AppendLine(text2)
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of IGrouping(Of Object, DataGridCellInfo))
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                Dim text3 As String = stringBuilder.ToString().TrimEnd(New Char() { vbCr, vbLf })
                Try
                    ClipboardInterceptor.SetClipboardText(text3)
                Catch ex As Exception
                    Logger.Warn(ex, String.Format("复制失败，请稍后再试（错误码：0x{0:X8}）", ex.HResult), LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\Behaviors\ClipboardInterceptor.vb")
                End Try
                e.Handled = True
            End If
        End Sub

        Public Shared EnableSafeClipboardProperty As DependencyProperty = DependencyProperty.RegisterAttached("EnableSafeClipboard", GetType(Boolean), GetType(ClipboardInterceptor), New PropertyMetadata(False, AddressOf ClipboardInterceptor.OnEnableSafeClipboardChanged))
    End Class
End Namespace
