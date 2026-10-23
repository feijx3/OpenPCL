Imports System
Imports System.Runtime.CompilerServices
Imports System.Windows.Controls

Namespace MeloongCore.Wpf.Extensions
    Public Module WpfExtensions
        <NullableContext(2)>
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Any(arr As UIElementCollection) As Boolean
            Return arr IsNot Nothing AndAlso arr.Count > 0
        End Function
    End Module
End Namespace
