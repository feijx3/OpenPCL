Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Input
Imports System.Windows.Interop
Imports System.Windows.Media
Imports System.Windows.Threading
Imports Microsoft.VisualBasic

Namespace PCL
    Public Class MyResizer
        Public Sub New(target As Window)
            Me.target = Nothing
            Me.resizeRight = False
            Me.resizeLeft = False
            Me.resizeUp = False
            Me.resizeDown = False
            Me.leftElements = New Dictionary(Of UIElement, Short)()
            Me.rightElements = New Dictionary(Of UIElement, Short)()
            Me.upElements = New Dictionary(Of UIElement, Short)()
            Me.downElements = New Dictionary(Of UIElement, Short)()
            Me.startMousePoint = Nothing
            Me.startWindowSize = Nothing
            Me.startWindowLeftUpPoint = Nothing
            Me.target = target
            If Information.IsNothing(target) Then
                Throw New Exception("Invalid Window handle")
            End If
            AddHandler target.SourceInitialized, AddressOf Me.MyMacClass_SourceInitialized
        End Sub

        Private Sub MyMacClass_SourceInitialized(sender As Object, e As EventArgs)
            Me.hs = TryCast(PresentationSource.FromVisual(CType(sender, Visual)), HwndSource)
            Me.hs.AddHook(AddressOf Me.WndProc)
        End Sub

        Private Function WndProc(hwnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr, ByRef handled As Boolean) As IntPtr
            If msg = 36 Then
                MyResizer.WmGetMinMaxInfo(hwnd, lParam)
                handled = True
            End If
            Return CType(0, IntPtr)
        End Function

        Private Shared Sub WmGetMinMaxInfo(hwnd As IntPtr, lParam As IntPtr)
            Dim obj As Object = Marshal.PtrToStructure(lParam, GetType(MyResizer.MINMAXINFO))
            Dim minmaxinfo As MyResizer.MINMAXINFO = If((obj IsNot Nothing), CType(obj, MyResizer.MINMAXINFO), Nothing)
            Dim num As Integer = 2
            Dim intPtr As IntPtr = MyResizer.MonitorFromWindow(hwnd, num)
            If intPtr <> IntPtr.Zero Then
                Dim monitorinfo As MyResizer.MONITORINFO = New MyResizer.MONITORINFO()
                MyResizer.GetMonitorInfo(intPtr, monitorinfo)
                Dim rcWork As MyResizer.RECT = monitorinfo.rcWork
                Dim rcMonitor As MyResizer.RECT = monitorinfo.rcMonitor
                minmaxinfo.ptMaxPosition.x = Math.Abs(rcWork.left - rcMonitor.left)
                minmaxinfo.ptMaxPosition.y = Math.Abs(rcWork.top - rcMonitor.top)
                minmaxinfo.ptMaxSize.y = Math.Abs(rcWork.bottom - rcWork.top)
                MyResizer.workAreaMaxHeight = minmaxinfo.ptMaxSize.y
                If rcWork.Height = rcMonitor.Height Then
                    Dim ptr As __ByRef(Of Integer) = minmaxinfo.ptMaxSize.y
                    minmaxinfo.ptMaxSize.y = ptr - 2
                End If
            End If
            Marshal.StructureToPtr(Of MyResizer.MINMAXINFO)(minmaxinfo, lParam, True)
        End Sub

        Private Declare Ansi Function GetMonitorInfo Lib "user32" (hMonitor As IntPtr, lpmi As MyResizer.MONITORINFO) As Boolean

        Private Declare Ansi Function MonitorFromWindow Lib "User32" (handle As IntPtr, flags As Integer) As IntPtr

        Private Sub connectMouseHandlers(element As UIElement)
            AddHandler element.MouseLeftButtonDown, AddressOf Me.element_MouseLeftButtonDown
        End Sub

        Public Sub addResizerRight(element As UIElement)
            Me.connectMouseHandlers(element)
            Me.rightElements.Add(element, 0S)
        End Sub

        Public Sub addResizerLeft(element As UIElement)
            Me.connectMouseHandlers(element)
            Me.leftElements.Add(element, 0S)
        End Sub

        Public Sub addResizerUp(element As UIElement)
            Me.connectMouseHandlers(element)
            Me.upElements.Add(element, 0S)
        End Sub

        Public Sub addResizerDown(element As UIElement)
            Me.connectMouseHandlers(element)
            Me.downElements.Add(element, 0S)
        End Sub

        Public Sub addResizerRightDown(element As UIElement)
            Me.connectMouseHandlers(element)
            Me.rightElements.Add(element, 0S)
            Me.downElements.Add(element, 0S)
        End Sub

        Public Sub addResizerLeftDown(element As UIElement)
            Me.connectMouseHandlers(element)
            Me.leftElements.Add(element, 0S)
            Me.downElements.Add(element, 0S)
        End Sub

        Public Sub addResizerRightUp(element As UIElement)
            Me.connectMouseHandlers(element)
            Me.rightElements.Add(element, 0S)
            Me.upElements.Add(element, 0S)
        End Sub

        Public Sub addResizerLeftUp(element As UIElement)
            Me.connectMouseHandlers(element)
            Me.leftElements.Add(element, 0S)
            Me.upElements.Add(element, 0S)
        End Sub

        Private Sub element_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
            MyResizer.GetCursorPos(Me.startMousePoint)
            Me.startMousePoint.X = CInt(Math.Round(ModBase.GetWPFSize(CDbl(Me.startMousePoint.X))))
            Me.startMousePoint.Y = CInt(Math.Round(ModBase.GetWPFSize(CDbl(Me.startMousePoint.Y))))
            Me.startWindowSize = New Size(Me.target.Width, Me.target.Height)
            Me.startWindowLeftUpPoint = New MyResizer.POINT(CInt(Math.Round(Me.target.Left)), CInt(Math.Round(Me.target.Top)))
            Dim uielement As UIElement = CType(sender, UIElement)
            If Me.leftElements.ContainsKey(uielement) Then
                Me.resizeLeft = True
            End If
            If Me.rightElements.ContainsKey(uielement) Then
                Me.resizeRight = True
            End If
            If Me.upElements.ContainsKey(uielement) Then
                Me.resizeUp = True
            End If
            If Me.downElements.ContainsKey(uielement) Then
                Me.resizeDown = True
            End If
            ModBase.RunInNewThread(AddressOf Me.updateSizeLoop, "窗口大小调整检测", ThreadPriority.Normal)
        End Sub

        Private Sub updateSizeLoop()
            Try
                While Me.resizeDown OrElse Me.resizeLeft OrElse Me.resizeRight OrElse Me.resizeUp
                    Me.target.Dispatcher.Invoke(AddressOf Me.updateSize, DispatcherPriority.Render)
                    Me.target.Dispatcher.Invoke(AddressOf Me.updateMouseDown, DispatcherPriority.Render)
                    Thread.Sleep(0)
                End While
            Catch ex As Exception
            End Try
        End Sub

        Private Sub updateSize()
            Dim pointAPI As MyResizer.PointAPI = Nothing
            MyResizer.GetCursorPos(pointAPI)
            pointAPI.X = CInt(Math.Round(ModBase.GetWPFSize(CDbl(pointAPI.X))))
            pointAPI.Y = CInt(Math.Round(ModBase.GetWPFSize(CDbl(pointAPI.Y))))
            Try
                Dim num As Double = -1.0
                Dim num2 As Double = -1.0
                Dim num3 As Double = -10000.0
                Dim num4 As Double = -10000.0
                If Me.resizeRight Then
                    ' The following expression was wrapped in a checked-expression
                    If Me.target.Width = Me.target.MinWidth Then
                        If Me.startMousePoint.X < pointAPI.X Then
                            ' The following expression was wrapped in a checked-expression
                            num = Me.startWindowSize.Width - CDbl((Me.startMousePoint.X - pointAPI.X))
                        End If
                    ElseIf Me.startWindowSize.Width - CDbl((Me.startMousePoint.X - pointAPI.X)) >= Me.target.MinWidth Then
                        ' The following expression was wrapped in a checked-expression
                        num = Me.startWindowSize.Width - CDbl((Me.startMousePoint.X - pointAPI.X))
                    Else
                        num = Me.target.MinWidth
                    End If
                End If
                If Me.resizeDown Then
                    ' The following expression was wrapped in a checked-expression
                    If Me.target.Height = Me.target.MinHeight Then
                        If Me.startMousePoint.Y < pointAPI.Y Then
                            If MyResizer.workAreaMaxHeight > 0 Then
                                ' The following expression was wrapped in a checked-expression
                                ' The following expression was wrapped in a checked-expression
                                num2 = If((Me.startWindowSize.Height - CDbl((Me.startMousePoint.Y - pointAPI.Y)) + Me.target.Top <= CDbl(MyResizer.workAreaMaxHeight)), (Me.startWindowSize.Height - CDbl((Me.startMousePoint.Y - pointAPI.Y))), (CDbl(MyResizer.workAreaMaxHeight) - Me.target.Top))
                            Else
                                ' The following expression was wrapped in a checked-expression
                                num2 = Me.startWindowSize.Height - CDbl((Me.startMousePoint.Y - pointAPI.Y))
                            End If
                        End If
                    ElseIf Me.startWindowSize.Height - CDbl((Me.startMousePoint.Y - pointAPI.Y)) >= Me.target.MinHeight Then
                        If MyResizer.workAreaMaxHeight > 0 Then
                            ' The following expression was wrapped in a checked-expression
                            ' The following expression was wrapped in a checked-expression
                            num2 = If((Me.startWindowSize.Height - CDbl((Me.startMousePoint.Y - pointAPI.Y)) + Me.target.Top <= CDbl(MyResizer.workAreaMaxHeight)), (Me.startWindowSize.Height - CDbl((Me.startMousePoint.Y - pointAPI.Y))), (CDbl(MyResizer.workAreaMaxHeight) - Me.target.Top))
                        Else
                            ' The following expression was wrapped in a checked-expression
                            num2 = Me.startWindowSize.Height - CDbl((Me.startMousePoint.Y - pointAPI.Y))
                        End If
                    Else
                        num2 = Me.target.MinHeight
                    End If
                End If
                If Me.resizeLeft Then
                    ' The following expression was wrapped in a checked-expression
                    If Me.target.Width = Me.target.MinWidth Then
                        If Me.startMousePoint.X > pointAPI.X Then
                            num = Me.startWindowSize.Width + CDbl(Me.startMousePoint.X) - CDbl(pointAPI.X)
                            num3 = CDbl((Me.startWindowLeftUpPoint.x - (Me.startMousePoint.X - pointAPI.X)))
                        Else
                            num = Me.target.MinWidth
                            num3 = CDbl(Me.startWindowLeftUpPoint.x) + Me.startWindowSize.Width - Me.target.Width
                        End If
                    ElseIf Me.startWindowSize.Width + CDbl((Me.startMousePoint.X - pointAPI.X)) >= Me.target.MinWidth Then
                        ' The following expression was wrapped in a checked-expression
                        num = Me.startWindowSize.Width + CDbl((Me.startMousePoint.X - pointAPI.X))
                        num3 = CDbl((Me.startWindowLeftUpPoint.x - (Me.startMousePoint.X - pointAPI.X)))
                    Else
                        num = Me.target.MinWidth
                        num3 = CDbl(Me.startWindowLeftUpPoint.x) + Me.startWindowSize.Width - Me.target.Width
                    End If
                End If
                If Me.resizeUp Then
                    ' The following expression was wrapped in a checked-expression
                    If Me.target.Height = Me.target.MinHeight Then
                        If Me.startMousePoint.Y > pointAPI.Y Then
                            num2 = Me.startWindowSize.Height + CDbl(Me.startMousePoint.Y) - CDbl(pointAPI.Y)
                            num4 = CDbl((Me.startWindowLeftUpPoint.y - (Me.startMousePoint.Y - pointAPI.Y)))
                        Else
                            num2 = Me.target.MinHeight
                            num4 = CDbl(Me.startWindowLeftUpPoint.y) + Me.startWindowSize.Height - Me.target.Height
                        End If
                    ElseIf Me.startWindowSize.Height + CDbl((Me.startMousePoint.Y - pointAPI.Y)) >= Me.target.MinHeight Then
                        num2 = Me.startWindowSize.Height + CDbl(Me.startMousePoint.Y) - CDbl(pointAPI.Y)
                        num4 = CDbl((Me.startWindowLeftUpPoint.y - (Me.startMousePoint.Y - pointAPI.Y)))
                    Else
                        num2 = Me.target.MinHeight
                        num4 = CDbl(Me.startWindowLeftUpPoint.y) + Me.startWindowSize.Height - Me.target.Height
                    End If
                End If
                If num > 10.0 AndAlso Math.Abs(num - Me.target.Width) > 0.7 Then
                    Me.target.Width = num
                End If
                If num2 > 10.0 AndAlso Math.Abs(num2 - Me.target.Height) > 0.7 Then
                    Me.target.Height = num2
                End If
                If num3 > -9999.0 AndAlso Math.Abs(num3 - Me.target.Left) > 0.7 Then
                    Me.target.Left = num3
                End If
                If num4 > -9999.0 AndAlso Math.Abs(num4 - Me.target.Top) > 0.7 Then
                    Me.target.Top = num4
                End If
            Catch ex As Exception
            End Try
        End Sub

        Private Sub updateMouseDown()
            If(CInt(MyResizer.GetAsyncKeyState(1)) And 32768) = 0 Then
                Me.resizeRight = False
                Me.resizeLeft = False
                Me.resizeUp = False
                Me.resizeDown = False
            End If
        End Sub

        Private Declare Ansi Function GetCursorPos Lib "user32.dll" (<System.Runtime.InteropServices.OutAttribute()> ByRef lpPoint As MyResizer.PointAPI) As Boolean

        Private Declare Ansi Function GetAsyncKeyState Lib "user32.dll" (vKey As Integer) As Short

        Private target As Window

        Private resizeRight As Boolean

        Private resizeLeft As Boolean

        Private resizeUp As Boolean

        Private resizeDown As Boolean

        Private leftElements As Dictionary(Of UIElement, Short)

        Private rightElements As Dictionary(Of UIElement, Short)

        Private upElements As Dictionary(Of UIElement, Short)

        Private downElements As Dictionary(Of UIElement, Short)

        Private startMousePoint As MyResizer.PointAPI

        Private startWindowSize As Size

        Private startWindowLeftUpPoint As MyResizer.POINT

        Private Shared workAreaMaxHeight As Integer = -1

        Private hs As HwndSource

        Private Delegate Sub RefreshDelegate()

        Private Structure POINT
            Public Sub New(x As Integer, y As Integer)
                Me = Nothing
                Me.x = x
                Me.y = y
            End Sub

            Public x As Integer

            Public y As Integer
        End Structure

        Private Structure MINMAXINFO
            Public ptReserved As MyResizer.POINT

            Public ptMaxSize As MyResizer.POINT

            Public ptMaxPosition As MyResizer.POINT

            Public ptMinTrackSize As MyResizer.POINT

            Public ptMaxTrackSize As MyResizer.POINT
        End Structure

        Private Structure RECT
            Public ReadOnly Property Width As Integer
                Get
                    ' The following expression was wrapped in a checked-expression
                    Return Math.Abs(Me.right - Me.left)
                End Get
            End Property

            Public ReadOnly Property Height As Integer
                Get
                    ' The following expression was wrapped in a checked-expression
                    Return Me.bottom - Me.top
                End Get
            End Property

            Public Sub New(left As Integer, top As Integer, right As Integer, bottom As Integer)
                Me = Nothing
                Me.left = left
                Me.top = top
                Me.right = right
                Me.bottom = bottom
            End Sub

            Public Sub New(rcSrc As MyResizer.RECT)
                Me = Nothing
                Me.left = rcSrc.left
                Me.top = rcSrc.top
                Me.right = rcSrc.right
                Me.bottom = rcSrc.bottom
            End Sub

            Public left As Integer

            Public top As Integer

            Public right As Integer

            Public bottom As Integer

            Public Shared Empty As MyResizer.RECT = Nothing
        End Structure

        <StructLayout(LayoutKind.Sequential, CharSet := CharSet.Auto)>
        Private Class MONITORINFO
            Public Sub New()
                Me.cbSize = Marshal.SizeOf(GetType(MyResizer.MONITORINFO))
                Me.rcMonitor = Nothing
                Me.rcWork = Nothing
                Me.dwFlags = 0
            End Sub

            Public cbSize As Integer

            Public rcMonitor As MyResizer.RECT

            Public rcWork As MyResizer.RECT

            Public dwFlags As Integer
        End Class

        Private Structure PointAPI
            Public X As Integer

            Public Y As Integer
        End Structure
    End Class
End Namespace
