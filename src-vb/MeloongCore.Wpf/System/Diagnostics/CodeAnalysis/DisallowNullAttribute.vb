Imports System

Namespace System.Diagnostics.CodeAnalysis
    <AttributeUsage(AttributeTargets.[Property] Or AttributeTargets.Field Or AttributeTargets.Parameter, Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class DisallowNullAttribute
        Inherits Attribute

    End Class
End Namespace
