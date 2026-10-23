Imports System
Imports System.Runtime.CompilerServices

Namespace System.Diagnostics.CodeAnalysis
    <NullableContext(2)>
    <Nullable(0)>
    <AttributeUsage(AttributeTargets.Parameter, Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class ConstantExpectedAttribute
        Inherits Attribute

        Public Property Min As Object

        Public Property Max As Object
    End Class
End Namespace
