Imports System
Imports System.Diagnostics.CodeAnalysis
Imports System.Runtime.CompilerServices

Namespace System.Runtime.Versioning
    <NullableContext(2)>
    <Nullable(0)>
    <AttributeUsage(AttributeTargets.Assembly Or AttributeTargets.[Module] Or AttributeTargets.[Class] Or AttributeTargets.Struct Or AttributeTargets.[Enum] Or AttributeTargets.Constructor Or AttributeTargets.Method Or AttributeTargets.[Property] Or AttributeTargets.Field Or AttributeTargets.[Event] Or AttributeTargets.[Interface] Or AttributeTargets.[Delegate], Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class RequiresPreviewFeaturesAttribute
        Inherits Attribute

        Public Sub New()
        End Sub

        Public Sub New(message As String)
            Me.Message = message
        End Sub

        Public ReadOnly Property Message As String

        Public Property Url As String
    End Class
End Namespace
