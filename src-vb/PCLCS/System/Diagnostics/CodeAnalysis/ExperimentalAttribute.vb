Imports System
Imports System.Runtime.CompilerServices

Namespace System.Diagnostics.CodeAnalysis
    <NullableContext(1)>
    <Nullable(0)>
    <AttributeUsage(AttributeTargets.Assembly Or AttributeTargets.[Module] Or AttributeTargets.[Class] Or AttributeTargets.Struct Or AttributeTargets.[Enum] Or AttributeTargets.Constructor Or AttributeTargets.Method Or AttributeTargets.[Property] Or AttributeTargets.Field Or AttributeTargets.[Event] Or AttributeTargets.[Interface] Or AttributeTargets.[Delegate], Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class ExperimentalAttribute
        Inherits Attribute

        Public Sub New(diagnosticId As String)
            Me.DiagnosticId = diagnosticId
        End Sub

        Public ReadOnly Property DiagnosticId As String

        <Nullable(2)>
        Public Property UrlFormat As String
    End Class
End Namespace
