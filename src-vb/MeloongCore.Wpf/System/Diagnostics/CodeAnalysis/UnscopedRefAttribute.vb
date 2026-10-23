Imports System

Namespace System.Diagnostics.CodeAnalysis
    <AttributeUsage(AttributeTargets.Method Or AttributeTargets.[Property] Or AttributeTargets.Parameter, AllowMultiple := False, Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class UnscopedRefAttribute
        Inherits Attribute

    End Class
End Namespace
