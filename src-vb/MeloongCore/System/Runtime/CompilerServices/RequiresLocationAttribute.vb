Imports System
Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis

Namespace System.Runtime.CompilerServices
    <AttributeUsage(AttributeTargets.Parameter, Inherited := False)>
    <EditorBrowsable(EditorBrowsableState.Never)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class RequiresLocationAttribute
        Inherits Attribute

    End Class
End Namespace
