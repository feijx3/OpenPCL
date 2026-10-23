Imports System
Imports System.Diagnostics.CodeAnalysis

Namespace System.Runtime.CompilerServices
    <AttributeUsage(AttributeTargets.[Module] Or AttributeTargets.[Class] Or AttributeTargets.Struct Or AttributeTargets.Constructor Or AttributeTargets.Method Or AttributeTargets.[Property] Or AttributeTargets.[Event] Or AttributeTargets.[Interface], Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class SkipLocalsInitAttribute
        Inherits Attribute

    End Class
End Namespace
