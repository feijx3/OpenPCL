Imports System
Imports System.Runtime.CompilerServices

Namespace MeloongCore
    <NullableContext(2)>
    <Nullable(0)>
    Public Class PreviewGetEventArgs(Of T)
        Inherits EventArgs

        Public Sub New(value As T)
        End Sub

        Public Property Value As T = value
    End Class
End Namespace
