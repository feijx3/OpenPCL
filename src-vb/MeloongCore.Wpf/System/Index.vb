Imports System
Imports System.Diagnostics.CodeAnalysis
Imports System.Runtime.CompilerServices

Namespace System
    <ExcludeFromCodeCoverage()>
    Friend Structure Index
        Implements IEquatable(Of Index)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub New(value As Integer, Optional fromEnd As Boolean = False)
            If value < 0 Then
                Index.ThrowHelper.ThrowValueArgumentOutOfRange_NeedNonNegNumException()
            End If
            If fromEnd Then
                Me._value = Not value
                Return
            End If
            Me._value = value
        End Sub

        Private Sub New(value As Integer)
            Me._value = value
        End Sub

        Public Shared ReadOnly Property Start As Index
            Get
                Return New Index(0)
            End Get
        End Property

        Public Shared ReadOnly Property [End] As Index
            Get
                Return New Index(-1)
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function FromStart(value As Integer) As Index
            If value < 0 Then
                Index.ThrowHelper.ThrowValueArgumentOutOfRange_NeedNonNegNumException()
            End If
            Return New Index(value)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function FromEnd(value As Integer) As Index
            If value < 0 Then
                Index.ThrowHelper.ThrowValueArgumentOutOfRange_NeedNonNegNumException()
            End If
            Return New Index(Not value)
        End Function

        Public ReadOnly Property Value As Integer
            Get
                If Me._value < 0 Then
                    Return Not Me._value
                End If
                Return Me._value
            End Get
        End Property

        Public ReadOnly Property IsFromEnd As Boolean
            Get
                Return Me._value < 0
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetOffset(length As Integer) As Integer
            Dim num As Integer = Me._value
            If Me.IsFromEnd Then
                num += length + 1
            End If
            Return num
        End Function

        <NullableContext(2)>
        Public Overrides Overloads Function Equals(<NotNullWhen(True)> value As Object) As Boolean
            Return TypeOf value Is Index AndAlso Me._value = CType(value, Index)._value
        End Function

        Public Overloads Function Equals(other As Index) As Boolean Implements System.IEquatable(Of System.Index).Equals
            Return Me._value = other._value
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Me._value
        End Function

        Public Shared Widening Operator CType(value As Integer) As Index
            Return Index.FromStart(value)
        End Operator

        <NullableContext(1)>
        Public Overrides Function ToString() As String
            If Me.IsFromEnd Then
                Return Me.ToStringFromEnd()
            End If
            Return CUInt(Me.Value).ToString()
        End Function

        <NullableContext(1)>
        Private Function ToStringFromEnd() As String
            Return "^" + Me.Value.ToString()
        End Function

        Private _value As Integer

        Private NotInheritable Class ThrowHelper
            <DoesNotReturn()>
            Public Shared Sub ThrowValueArgumentOutOfRange_NeedNonNegNumException()
                Throw New ArgumentOutOfRangeException("value", "Non-negative number required.")
            End Sub
        End Class
    End Structure
End Namespace
