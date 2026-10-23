Imports System
Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Interop

Namespace PCL
    Public Class DragHelper
        Public Event DragDrop As EventHandler

        Public Property DropFilePaths As String()
            Get
                Return Me._DropFilePathsBackingField
            End Get
            Private Set(value As String())
                Me._DropFilePathsBackingField = value
            End Set
        End Property

        Public Property DropPoint As DragHelper.POINT
            Get
                Return Me._DropPointBackingField
            End Get
            Private Set(value As DragHelper.POINT)
                Me._DropPointBackingField = value
            End Set
        End Property

        Public Property HwndIntPtrSource As HwndSource

        Public Sub AddHook()
            Me.RemoveDragHook()
            Me.HwndIntPtrSource.AddHook(AddressOf Me.WndProc)
            Dim handle As IntPtr = Me.HwndIntPtrSource.Handle
            If DragHelper.IsUserAnAdmin() Then
                DragHelper.RevokeDragDrop(handle)
            End If
            DragHelper.DragAcceptFiles(handle, True)
            DragHelper.ChangeMessageFilter(handle)
        End Sub

        Public Sub RemoveDragHook()
            Me.HwndIntPtrSource.RemoveHook(AddressOf Me.WndProc)
            DragHelper.DragAcceptFiles(Me.HwndIntPtrSource.Handle, False)
        End Sub

        Private Function WndProc(hwnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr, ByRef handled As Boolean) As IntPtr
            Dim array As String() = Nothing
            Dim point As DragHelper.POINT = Nothing
            If DragHelper.TryGetDropInfo(msg, wParam, array, point) Then
                Me.DropPoint = point
                Me.DropFilePaths = array
                Dim dragDropEvent As EventHandler = Me.DragDropEvent
                If dragDropEvent IsNot Nothing Then
                    dragDropEvent(Me, EventArgs.Empty)
                End If
                handled = True
            End If
            Return IntPtr.Zero
        End Function

        Private Declare Function ChangeWindowMessageFilterEx Lib "user32.dll" (hWnd As IntPtr, msg As UInteger, action As UInteger, ByRef pChangeFilterStruct As DragHelper.CHANGEFILTERSTRUCT) As Boolean

        Private Declare Function ChangeWindowMessageFilter Lib "user32.dll" (msg As UInteger, flags As UInteger) As Boolean

        Private Declare Sub DragAcceptFiles Lib "shell32.dll" (hWnd As IntPtr, fAccept As Boolean)

        Private Declare Unicode Function DragQueryFile Lib "shell32.dll" (hWnd As IntPtr, iFile As UInteger, lpszFile As StringBuilder, cch As Integer) As UInteger

        Private Declare Function DragQueryPoint Lib "shell32.dll" (hDrop As IntPtr, ByRef lppt As DragHelper.POINT) As Boolean

        Private Declare Sub DragFinish Lib "shell32.dll" (hDrop As IntPtr)

        Private Declare Function RevokeDragDrop Lib "ole32.dll" (hWnd As IntPtr) As Integer

        Private Declare Function IsUserAnAdmin Lib "shell32.dll" () As Boolean

        Private Shared Sub ChangeMessageFilter(handle As IntPtr)
            Dim version As Version = Environment.OSVersion.Version
            Dim flag As Boolean = version >= New Version(6, 0)
            Dim flag2 As Boolean = version >= New Version(6, 1)
            If flag Then
                Dim changefilterstruct As DragHelper.CHANGEFILTERSTRUCT = Nothing
                changefilterstruct.cbSize = 8UI
                For Each num As UInteger In New UInteger() { 563UI, 73UI, 74UI }
                    Dim flag3 As Boolean
                    If flag2 Then
                        flag3 = Not DragHelper.ChangeWindowMessageFilterEx(handle, num, 1UI, changefilterstruct)
                    Else
                        flag3 = Not DragHelper.ChangeWindowMessageFilter(num, 1UI)
                    End If
                    If flag3 Then
                        Throw New Win32Exception(Marshal.GetLastWin32Error())
                    End If
                Next
            End If
        End Sub

        Private Shared Function TryGetDropInfo(msg As Integer, wParam As IntPtr, ByRef dropFilePaths As String(), ByRef dropPoint As DragHelper.POINT) As Boolean
            dropFilePaths = Nothing
            dropPoint = Nothing
            Dim flag As Boolean
            If CLng(msg) <> 563L Then
                flag = False
            Else
                Dim num As UInteger = DragHelper.DragQueryFile(wParam, UInteger.MaxValue, Nothing, 0)
                dropFilePaths = New String(CInt(num) - 1 + 1 - 1) {}
                Dim num2 As UInteger = CUInt((CInt(num) - 1))
                For num3 As UInteger = 0UI To num2
                    Dim stringBuilder As StringBuilder = New StringBuilder(260)
                    If CULng(DragHelper.DragQueryFile(wParam, num3, stringBuilder, stringBuilder.Capacity)) > 0UL Then
                        dropFilePaths(CInt(num3)) = stringBuilder.ToString()
                    End If
                Next
                DragHelper.DragQueryPoint(wParam, dropPoint)
                DragHelper.DragFinish(wParam)
                flag = True
            End If
            Return flag
        End Function

        Private _DropFilePathsBackingField As String()

        Private _DropPointBackingField As DragHelper.POINT

        Private Const WM_COPYGLOBALDATA As UInteger = 73UI

        Private Const WM_COPYDATA As UInteger = 74UI

        Private Const WM_DROPFILES As UInteger = 563UI

        Private Const MSGFLT_ALLOW As UInteger = 1UI

        Private Const MSGFLT_ADD As UInteger = 1UI

        Private Const MAX_PATH As Integer = 260

        Public Structure POINT
            Public X As Integer

            Public Y As Integer
        End Structure

        Private Structure CHANGEFILTERSTRUCT
            Public cbSize As UInteger

            Public ExtStatus As UInteger
        End Structure
    End Class
End Namespace
