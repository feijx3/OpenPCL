Imports System
Imports System.Diagnostics.CodeAnalysis

Namespace System.Runtime.CompilerServices
    <NullableContext(1)>
    <Nullable(0)>
    <AttributeUsage(AttributeTargets.All, AllowMultiple := True, Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class CompilerFeatureRequiredAttribute
        Inherits Attribute

        Public Sub New(featureName As String)
            Me.FeatureName = featureName
        End Sub

        Public ReadOnly Property FeatureName As String

        Public Property IsOptional As Boolean

        Public Const RefStructs As String = "RefStructs"

        Public Const RequiredMembers As String = "RequiredMembers"
    End Class
End Namespace
