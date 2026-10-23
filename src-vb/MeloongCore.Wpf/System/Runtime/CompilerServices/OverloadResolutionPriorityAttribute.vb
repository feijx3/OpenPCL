Imports System
Imports System.Diagnostics.CodeAnalysis

Namespace System.Runtime.CompilerServices
    <AttributeUsage(AttributeTargets.Constructor Or AttributeTargets.Method Or AttributeTargets.[Property], AllowMultiple := False, Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class OverloadResolutionPriorityAttribute
        Inherits Attribute

        Public Sub New(priority As Integer)
            Me.Priority = priority
        End Sub

        Public ReadOnly Property Priority As Integer
    End Class
End Namespace
