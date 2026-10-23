Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL.My
    <HideModuleName()>
    <GeneratedCode("MyTemplate", "11.0.0.0")>
    Friend NotInheritable Module MyProject
        <EditorBrowsable(EditorBrowsableState.Never)>
        <ComVisible(False)>
        Friend NotInheritable Class ThreadSafeObjectProvider(Of T As New)
            Friend ReadOnly Property GetInstance As T
                <DebuggerHidden()>
                Get
                    If MyProject.ThreadSafeObjectProvider(Of T).m_ThreadStaticValue Is Nothing Then
                        MyProject.ThreadSafeObjectProvider(Of T).m_ThreadStaticValue = New T()
                    End If
                    Return MyProject.ThreadSafeObjectProvider(Of T).m_ThreadStaticValue
                End Get
            End Property

            <DebuggerHidden()>
            <EditorBrowsable(EditorBrowsableState.Never)>
            Public Sub New()
            End Sub

            <CompilerGenerated()>
            <ThreadStatic()>
            Private Shared m_ThreadStaticValue As T
        End Class
    End Module
End Namespace
