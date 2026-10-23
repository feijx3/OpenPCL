Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.Devices
Imports Microsoft.VisualBasic.Logging

Namespace PCL.My
    <HideModuleName()>
    Friend NotInheritable Module MyWpfExtension
        Friend ReadOnly Property Application As Application
            Get
                Return CType(Global.System.Windows.Application.Current, Application)
            End Get
        End Property

        Friend ReadOnly Property Computer As Computer
            Get
                Return MyWpfExtension.s_Computer.GetInstance
            End Get
        End Property

        Friend ReadOnly Property User As User
            Get
                Return MyWpfExtension.s_User.GetInstance
            End Get
        End Property

        Friend ReadOnly Property Log As Log
            Get
                Return MyWpfExtension.s_Log.GetInstance
            End Get
        End Property

        Friend ReadOnly Property Windows As MyWpfExtension.MyWindows
            <DebuggerHidden()>
            Get
                Return MyWpfExtension.s_Windows.GetInstance
            End Get
        End Property

        Private s_Computer As MyProject.ThreadSafeObjectProvider(Of Computer) = New MyProject.ThreadSafeObjectProvider(Of Computer)()

        Private s_User As MyProject.ThreadSafeObjectProvider(Of User) = New MyProject.ThreadSafeObjectProvider(Of User)()

        Private s_Windows As MyProject.ThreadSafeObjectProvider(Of MyWpfExtension.MyWindows) = New MyProject.ThreadSafeObjectProvider(Of MyWpfExtension.MyWindows)()

        Private s_Log As MyProject.ThreadSafeObjectProvider(Of Log) = New MyProject.ThreadSafeObjectProvider(Of Log)()

        <EditorBrowsable(EditorBrowsableState.Never)>
        <MyGroupCollection("System.Windows.Window", "Create__Instance__", "Dispose__Instance__", "My.MyWpfExtenstionModule.Windows")>
        Friend NotInheritable Class MyWindows
            <DebuggerHidden()>
            Private Shared Function Create__Instance__(Of T As{Window, New})(Instance As T) As T
                Dim t As T
                If Instance Is Nothing Then
                    If MyWpfExtension.MyWindows.s_WindowBeingCreated IsNot Nothing Then
                        If MyWpfExtension.MyWindows.s_WindowBeingCreated.ContainsKey(GetType(T)) Then
                            Throw New InvalidOperationException("The window cannot be accessed via My.Windows from the Window constructor.")
                        End If
                    Else
                        MyWpfExtension.MyWindows.s_WindowBeingCreated = New Hashtable()
                    End If
                    MyWpfExtension.MyWindows.s_WindowBeingCreated.Add(GetType(T), Nothing)
                    t = New T()
                Else
                    t = Instance
                End If
                Return t
            End Function

            <DebuggerHidden()>
            Private Sub Dispose__Instance__(Of T As Window)(ByRef instance As T)
                instance = Nothing
            End Sub

            <DebuggerHidden()>
            <EditorBrowsable(EditorBrowsableState.Never)>
            Public Sub New()
            End Sub

            <EditorBrowsable(EditorBrowsableState.Never)>
            Public Overrides Function Equals(o As Object) As Boolean
                Return MyBase.Equals(RuntimeHelpers.GetObjectValue(o))
            End Function

            <EditorBrowsable(EditorBrowsableState.Never)>
            Public Overrides Function GetHashCode() As Integer
                Return MyBase.GetHashCode()
            End Function

            <EditorBrowsable(EditorBrowsableState.Never)>
            Friend Function [GetType]() As Type
                Return GetType(MyWpfExtension.MyWindows)
            End Function

            <EditorBrowsable(EditorBrowsableState.Never)>
            Public Overrides Function ToString() As String
                Return MyBase.ToString()
            End Function

            Public Property FormMain As FormMain
                Get
                    Me.m_FormMain = MyWpfExtension.MyWindows.Create__Instance__(Of FormMain)(Me.m_FormMain)
                    Return Me.m_FormMain
                End Get
                Set(value As FormMain)
                    If value IsNot Me.m_FormMain Then
                        If value IsNot Nothing Then
                            Throw New ArgumentException("Property can only be set to Nothing")
                        End If
                        Me.Dispose__Instance__(Of FormMain)(Me.m_FormMain)
                    End If
                End Set
            End Property

            <ThreadStatic()>
            Private Shared s_WindowBeingCreated As Hashtable

            <EditorBrowsable(EditorBrowsableState.Never)>
            Public m_FormMain As FormMain
        End Class
    End Module
End Namespace
