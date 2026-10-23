Imports System
Imports System.Runtime.CompilerServices
Imports System.Threading.Tasks

Namespace MeloongCore.Extensions
    <NullableContext(1)>
    <Nullable(0)>
    Public Module TaskExtensions
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Sub Run(task As Task)
            task.GetAwaiter().GetResult()
        End Sub

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Run(Of T)(task As Task(Of T)) As T
            Return task.GetAwaiter().GetResult()
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function NoCapture(task As Task) As ConfiguredTaskAwaitable
            Return task.ConfigureAwait(False)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function NoCapture(Of T)(task As Task(Of T)) As <Nullable(New Byte() { 0, 1 })> ConfiguredTaskAwaitable(Of T)
            Return task.ConfigureAwait(False)
        End Function
    End Module
End Namespace
