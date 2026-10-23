Imports System

Namespace System.Diagnostics.CodeAnalysis
    <AttributeUsage(AttributeTargets.[Property] Or AttributeTargets.Field Or AttributeTargets.Parameter Or AttributeTargets.ReturnValue, Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class NotNullAttribute
        Inherits Attribute

    End Class
End Namespace
