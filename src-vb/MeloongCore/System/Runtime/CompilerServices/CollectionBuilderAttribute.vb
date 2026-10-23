Imports System
Imports System.Diagnostics.CodeAnalysis

Namespace System.Runtime.CompilerServices
    <NullableContext(1)>
    <Nullable(0)>
    <AttributeUsage(AttributeTargets.[Class] Or AttributeTargets.Struct Or AttributeTargets.[Interface], Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class CollectionBuilderAttribute
        Inherits Attribute

        Public Sub New(builderType As Type, methodName As String)
            Me.BuilderType = builderType
            Me.MethodName = methodName
        End Sub

        Public ReadOnly Property BuilderType As Type

        Public ReadOnly Property MethodName As String
    End Class
End Namespace
