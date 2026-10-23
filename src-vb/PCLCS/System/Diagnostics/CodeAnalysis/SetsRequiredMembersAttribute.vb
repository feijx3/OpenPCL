Imports System

Namespace System.Diagnostics.CodeAnalysis
    <AttributeUsage(AttributeTargets.Constructor, AllowMultiple := False, Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class SetsRequiredMembersAttribute
        Inherits Attribute

    End Class
End Namespace
