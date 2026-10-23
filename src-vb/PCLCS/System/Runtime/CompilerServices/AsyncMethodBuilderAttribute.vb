Imports System
Imports System.Diagnostics.CodeAnalysis

Namespace System.Runtime.CompilerServices
    <NullableContext(1)>
    <Nullable(0)>
    <AttributeUsage(AttributeTargets.[Class] Or AttributeTargets.Struct Or AttributeTargets.[Enum] Or AttributeTargets.Method Or AttributeTargets.[Interface] Or AttributeTargets.[Delegate], Inherited := False, AllowMultiple := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class AsyncMethodBuilderAttribute
        Inherits Attribute

        Public Sub New(builderType As Type)
            Me.BuilderType = builderType
        End Sub

        Public ReadOnly Property BuilderType As Type
    End Class
End Namespace
