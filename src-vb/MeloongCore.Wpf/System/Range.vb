Imports System
Imports System.Diagnostics.CodeAnalysis
Imports System.Runtime.CompilerServices

Namespace System
    <ExcludeFromCodeCoverage()>
    Friend Structure Range
        Implements IEquatable(Of Range)

        Public ReadOnly Property Start As Index

        Public ReadOnly Property [End] As Index

        Public Sub New(start As Index, [end] As Index)
            Me.Start = start
            Me.[End] = [end]
        End Sub

        <NullableContext(2)>
        Public Overrides Overloads Function Equals(<NotNullWhen(True)> value As Object) As Boolean
            If TypeOf value Is Range Then
                Dim range As Range = CType(value, Range)
                If range.Start.Equals(Me.Start) Then
                    Return range.[End].Equals(Me.[End])
                End If
            End If
            Return False
        End Function

        Public Overloads Function Equals(other As Range) As Boolean Implements System.IEquatable(Of System.Range).Equals
            Return other.Start.Equals(Me.Start) AndAlso other.[End].Equals(Me.[End])
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Range.HashHelpers.Combine(Me.Start.GetHashCode(), Me.[End].GetHashCode())
        End Function

        <NullableContext(1)>
        Public Overrides Function ToString() As String
            Return Me.Start.ToString() + ".." + Me.[End].ToString()
        End Function

        Public Shared Function StartAt(start As Index) As Range
            Return New Range(start, Index.[End])
        End Function

        Public Shared Function EndAt([end] As Index) As Range
            Return New Range(Index.Start, [end])
        End Function

        Public Shared ReadOnly Property All As Range
            Get
                Return New Range(Index.Start, Index.[End])
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetOffsetAndLength(length As Integer) As <TupleElementNames(New String() { "Offset", "Length" })> ValueTuple(Of Integer, Integer)
            Dim start As Index = Me.Start
            Dim num As Integer
            If start.IsFromEnd Then
                num = length - start.Value
            Else
                num = start.Value
            End If
            Dim [end] As Index = Me.[End]
            Dim num2 As Integer
            If [end].IsFromEnd Then
                num2 = length - [end].Value
            Else
                num2 = [end].Value
            End If
            If num2 > length OrElse num > num2 Then
                Range.ThrowHelper.ThrowArgumentOutOfRangeException()
            End If
            Return New ValueTuple(Of Integer, Integer)(num, num2 - num)
        End Function

        Private NotInheritable Class HashHelpers
            Public Shared Function Combine(h1 As Integer, h2 As Integer) As Integer
                Return(((h1 << 5) Or CInt((CUInt(h1) >> 27))) + h1) Xor h2
            End Function
        End Class

        Private NotInheritable Class ThrowHelper
            <DoesNotReturn()>
            Public Shared Sub ThrowArgumentOutOfRangeException()
                Throw New ArgumentOutOfRangeException("length")
            End Sub
        End Class
    End Structure
End Namespace
