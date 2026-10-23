Imports System
Imports System.Runtime.CompilerServices

Namespace System.Diagnostics.CodeAnalysis
    <NullableContext(1)>
    <Nullable(0)>
    <AttributeUsage(AttributeTargets.[Property] Or AttributeTargets.Field Or AttributeTargets.Parameter, AllowMultiple := False, Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class StringSyntaxAttribute
        Inherits Attribute

        Public Sub New(syntax As String)
            Me.Syntax = syntax
            Me.Arguments = New Object(-1) {}
        End Sub

        Public Sub New(syntax As String, <Nullable(New Byte() { 1, 2 })> ParamArray arguments As Object())
            Me.Syntax = syntax
            Me.Arguments = arguments
        End Sub

        Public ReadOnly Property Syntax As String

        <Nullable(New Byte() { 1, 2 })>
        Public ReadOnly Property Arguments As Object()

        Public Const CompositeFormat As String = "CompositeFormat"

        Public Const DateOnlyFormat As String = "DateOnlyFormat"

        Public Const DateTimeFormat As String = "DateTimeFormat"

        Public Const EnumFormat As String = "EnumFormat"

        Public Const GuidFormat As String = "GuidFormat"

        Public Const Json As String = "Json"

        Public Const NumericFormat As String = "NumericFormat"

        Public Const Regex As String = "Regex"

        Public Const TimeOnlyFormat As String = "TimeOnlyFormat"

        Public Const TimeSpanFormat As String = "TimeSpanFormat"

        Public Const Uri As String = "Uri"

        Public Const Xml As String = "Xml"
    End Class
End Namespace
