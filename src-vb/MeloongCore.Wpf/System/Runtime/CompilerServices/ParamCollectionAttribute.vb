Imports System
Imports System.Diagnostics.CodeAnalysis

Namespace System.Runtime.CompilerServices
    <AttributeUsage(AttributeTargets.Parameter, Inherited := True, AllowMultiple := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class ParamCollectionAttribute
        Inherits Attribute

    End Class
End Namespace
