Imports System
Imports System.Runtime.CompilerServices

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Structure OneOf(Of T1, T2, T3)
        Public Shared Widening Operator CType(value As T1) As <Nullable(New Byte() { 0, 1, 1, 1 })> OneOf(Of T1, T2, T3)
            Return New OneOf(Of T1, T2, T3)(1, value, Nothing, Nothing)
        End Operator

        Public Shared Widening Operator CType(value As T2) As <Nullable(New Byte() { 0, 1, 1, 1 })> OneOf(Of T1, T2, T3)
            Return New OneOf(Of T1, T2, T3)(2, Nothing, value, Nothing)
        End Operator

        Public Shared Widening Operator CType(value As T3) As <Nullable(New Byte() { 0, 1, 1, 1 })> OneOf(Of T1, T2, T3)
            Return New OneOf(Of T1, T2, T3)(3, Nothing, Nothing, value)
        End Operator

        Public Shared Widening Operator CType(<Nullable(New Byte() { 0, 1, 1, 1 })> value As OneOf(Of T1, T2, T3)) As T1
            Return value.[As](Of T1)()
        End Operator

        Public Shared Widening Operator CType(<Nullable(New Byte() { 0, 1, 1, 1 })> value As OneOf(Of T1, T2, T3)) As T2
            Return value.[As](Of T2)()
        End Operator

        Public Shared Widening Operator CType(<Nullable(New Byte() { 0, 1, 1, 1 })> value As OneOf(Of T1, T2, T3)) As T3
            Return value.[As](Of T3)()
        End Operator

        <NullableContext(2)>
        Private Sub New(index As Integer, value1 As T1, value2 As T2, value3 As T3)
            Me._index = -1
            Me._index = index
            Me._value1 = value1
            Me._value2 = value2
            Me._value3 = value3
        End Sub

        Public Function Switch(Of TResult)(f1 As Func(Of T1, TResult), f2 As Func(Of T2, TResult), f3 As Func(Of T3, TResult)) As TResult
            Dim tresult As TResult
            Select Case Me._index
                Case 1
                    tresult = f1(Me._value1)
                Case 2
                    tresult = f2(Me._value2)
                Case 3
                    tresult = f3(Me._value3)
                Case Else
                    Throw New InvalidOperationException()
            End Select
            Return tresult
        End Function

        Public Sub Switch(f1 As Action(Of T1), f2 As Action(Of T2), f3 As Action(Of T3))
            If Me._index = 1 Then
                f1(Me._value1)
                Return
            End If
            If Me._index = 2 Then
                f2(Me._value2)
                Return
            End If
            If Me._index = 3 Then
                f3(Me._value3)
                Return
            End If
            Throw New InvalidOperationException()
        End Sub

        <NullableContext(2)>
        Public Function [Is](Of T)() As Boolean
            Select Case Me._index
                Case 1
                    If GetType(T) = GetType(T1) Then
                        Return True
                    End If
                Case 2
                    If GetType(T) = GetType(T2) Then
                        Return True
                    End If
                Case 3
                    If GetType(T) = GetType(T3) Then
                        Return True
                    End If
            End Select
            Return False
        End Function

        Public Function IsT1() As Boolean
            Return Me._index = 1
        End Function

        Public Function IsT2() As Boolean
            Return Me._index = 2
        End Function

        Public Function IsT3() As Boolean
            Return Me._index = 3
        End Function

        Public Function [As](Of T)() As T
            Select Case Me._index
                Case 1
                    If GetType(T) = GetType(T1) Then
                        Return CType(CObj(Me._value1), T)
                    End If
                Case 2
                    If GetType(T) = GetType(T2) Then
                        Return CType(CObj(Me._value2), T)
                    End If
                Case 3
                    If GetType(T) = GetType(T3) Then
                        Return CType(CObj(Me._value3), T)
                    End If
            End Select
            Throw New InvalidOperationException()
        End Function

        Public Function AsT1() As T1
            If Me._index = 1 Then
                Return Me._value1
            End If
            Throw New InvalidOperationException()
        End Function

        Public Function AsT2() As T2
            If Me._index = 2 Then
                Return Me._value2
            End If
            Throw New InvalidOperationException()
        End Function

        Public Function AsT3() As T3
            If Me._index = 3 Then
                Return Me._value3
            End If
            Throw New InvalidOperationException()
        End Function

        Private _index As Integer

        <Nullable(2)>
        Private _value1 As T1

        <Nullable(2)>
        Private _value2 As T2

        <Nullable(2)>
        Private _value3 As T3
    End Structure
End Namespace
