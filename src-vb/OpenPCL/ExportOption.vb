Imports System

Namespace PCL
    Public Class ExportOption
        Public Sub New()
            Me.RequireModLoader = False
            Me.RequireOptiFine = False
            Me.RequireModLoaderOrOptiFine = False
        End Sub

        Public Property Title As String

        Public Property Description As String

        Public Property Rules As String

        Public Property ShowRules As String

        Public Property DefaultChecked As Boolean

        Public Property RequireModLoader As Boolean

        Public Property RequireOptiFine As Boolean

        Public Property RequireModLoaderOrOptiFine As Boolean
    End Class
End Namespace
