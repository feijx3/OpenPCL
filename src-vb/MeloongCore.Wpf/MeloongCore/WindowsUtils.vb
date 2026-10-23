Imports System
Imports System.Security.Principal

Namespace MeloongCore
    Public Module WindowsUtils
        Public Function HasAdminRole() As Boolean
            Dim flag As Boolean
            Using current As WindowsIdentity = WindowsIdentity.GetCurrent()
                flag = New WindowsPrincipal(current).IsInRole(WindowsBuiltInRole.Administrator)
            End Using
            Return flag
        End Function
    End Module
End Namespace
