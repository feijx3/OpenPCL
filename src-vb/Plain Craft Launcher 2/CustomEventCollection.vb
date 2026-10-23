Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Windows.Markup

Namespace PCL
    <ContentProperty("Events")>
    Public Class CustomEventCollection
        Implements IEnumerable(Of CustomEvent)

        Public Sub New()
            Me._Events = New List(Of CustomEvent)()
        End Sub

        Public ReadOnly Property Events As List(Of CustomEvent)
            Get
                Return Me._Events
            End Get
        End Property

        Public Function GetEnumerator() As IEnumerator(Of CustomEvent) Implements System.Collections.Generic.IEnumerable(Of PCL.CustomEvent).GetEnumerator
            Return CType(Me.Events, IEnumerable(Of CustomEvent)).GetEnumerator()
        End Function

        Function IEnumerable_GetEnumerator() As IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return CType(Me.Events, IEnumerable).GetEnumerator()
        End Function

        Private _Events As List(Of CustomEvent)
    End Class
End Namespace
