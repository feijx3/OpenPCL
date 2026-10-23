Imports System
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public NotInheritable Class ValueRange(Of T As IComparable(Of T))
        Implements IEquatable(Of ValueRange(Of T))

        <NullableContext(2)>
        Public Sub New(lower As T, upper As T, isLowerInclusive As Boolean, isUpperInclusive As Boolean, hasLower As Boolean, hasUpper As Boolean)
            If hasLower AndAlso lower Is Nothing Then
                Throw New ArgumentNullException("lower")
            End If
            Me.Lower = lower
            If Not hasUpper OrElse upper IsNot Nothing Then
                Me.Upper = upper
                Me.HasLower = hasLower
                Me.HasUpper = hasUpper
                Me.IsLowerInclusive = isLowerInclusive
                Me.IsUpperInclusive = isUpperInclusive
                MyBase..ctor()
                Return
            End If
            Throw New ArgumentNullException("upper")
        End Sub

        <Nullable(2)>
        Public Property Lower As T

        <Nullable(2)>
        Public Property Upper As T

        Public Property HasLower As Boolean

        Public Property HasUpper As Boolean

        Public Sub ClearLower()
            Me.Lower = Nothing
            Me.HasLower = False
        End Sub

        Public Sub ClearUpper()
            Me.Upper = Nothing
            Me.HasUpper = False
        End Sub

        Public Sub SetLower(lower As T, isInclusive As Boolean)
            Me.Lower = lower
            Me.HasLower = True
            Me.IsLowerInclusive = isInclusive
        End Sub

        Public Sub SetUpper(upper As T, isInclusive As Boolean)
            Me.Upper = upper
            Me.HasUpper = True
            Me.IsUpperInclusive = isInclusive
        End Sub

        Public Shared Function Closed(lower As T, upper As T) As ValueRange(Of T)
            Return New ValueRange(Of T)(lower, upper, True, True, True, True)
        End Function

        Public Shared Function ClosedOpen(lower As T, upper As T) As ValueRange(Of T)
            Return New ValueRange(Of T)(lower, upper, True, False, True, True)
        End Function

        Public Shared Function OpenClosed(lower As T, upper As T) As ValueRange(Of T)
            Return New ValueRange(Of T)(lower, upper, False, True, True, True)
        End Function

        Public Shared Function Open(lower As T, upper As T) As ValueRange(Of T)
            Return New ValueRange(Of T)(lower, upper, False, False, True, True)
        End Function

        Public Shared Function Exactly(value As T) As ValueRange(Of T)
            Return New ValueRange(Of T)(value, value, True, True, True, True)
        End Function

        Public Shared Function AtLeast(lower As T) As ValueRange(Of T)
            Return New ValueRange(Of T)(lower, Nothing, True, True, True, False)
        End Function

        Public Shared Function GreaterThan(lower As T) As ValueRange(Of T)
            Return New ValueRange(Of T)(lower, Nothing, False, True, True, False)
        End Function

        Public Shared Function AtMost(upper As T) As ValueRange(Of T)
            Return New ValueRange(Of T)(Nothing, upper, True, True, False, True)
        End Function

        Public Shared Function LessThan(upper As T) As ValueRange(Of T)
            Return New ValueRange(Of T)(Nothing, upper, True, False, False, True)
        End Function

        Public Shared Function All() As ValueRange(Of T)
            Return New ValueRange(Of T)(Nothing, Nothing, True, True, False, False)
        End Function

        Public Shared Function FromString(str As String, <Nullable(New Byte() { 2, 1, 1 })> Optional parseFunc As Func(Of String, T) = Nothing) As ValueRange(Of T)
            If String.IsNullOrEmpty(str) Then
                Throw New ArgumentException("不能为空")
            End If
            Dim flag As Boolean = str(0) = "["c
            Dim flag2 As Boolean = str(str.Length - 1) = "]"c
            If Not flag AndAlso str(0) <> "("c Then
                Throw New ArgumentException("开头必须是 [ 或 (")
            End If
            If Not flag2 AndAlso str(str.Length - 1) <> ")"c Then
                Throw New ArgumentException("结尾必须是 ] 或 )")
            End If
            Dim num As Integer = str.IndexOfAny(New Char() { ","c, "，"c })
            If num = -1 Then
                Throw New ArgumentException("需要用逗号分隔两个值")
            End If
            Dim text As String = str.Substring(1, num - 1).Trim()
            Dim num2 As Integer = num + 1
            Dim text2 As String = str.Substring(num2, str.Length - 1 - num2).Trim()
            Dim t As T = Nothing
            Dim t2 As T = Nothing
            Dim flag3 As Boolean = False
            Dim flag4 As Boolean = False
            If(text Is Nothing OrElse text.Length <> 0) AndAlso Not(text = "-∞") Then
                t = If((parseFunc IsNot Nothing), parseFunc(text), CType(CObj(Convert.ChangeType(text, GetType(T))), T))
                flag3 = True
            End If
            If(text2 Is Nothing OrElse text2.Length <> 0) AndAlso Not(text2 = "+∞") AndAlso Not(text2 = "∞") Then
                t2 = If((parseFunc IsNot Nothing), parseFunc(text2), CType(CObj(Convert.ChangeType(text2, GetType(T))), T))
                flag4 = True
            End If
            Return New ValueRange(Of T)(t, t2, flag, flag2, flag3, flag4)
        End Function

        Public Overrides Function ToString() As String
            Dim array As String() = New String(4) {}
            array(0) = If((Me.HasLower AndAlso Me.IsLowerInclusive), "[", "(")
            Dim num As Integer = 1
            Dim text As String
            If Not Me.HasLower Then
                text = "-∞"
            Else
                Dim t As T = Me.Lower
                text = t.ToString()
            End If
            array(num) = text
            array(2) = ", "
            Dim num2 As Integer = 3
            Dim text2 As String
            If Not Me.HasUpper Then
                text2 = "+∞"
            Else
                Dim t As T = Me.Upper
                text2 = t.ToString()
            End If
            array(num2) = text2
            array(4) = If((Me.HasUpper AndAlso Me.IsUpperInclusive), "]", ")")
            Return String.Concat(array)
        End Function

        Public Function Contains(value As T) As Boolean
            If Me.HasLower Then
                Dim ptr As __ByRef(Of T) = value
                If Nothing Is Nothing Then
                    Dim t As T = value
                    ptr = t
                End If
                Dim num As Integer = ptr.CompareTo(Me.Lower)
                If num < 0 Then
                    Return False
                End If
                If num = 0 AndAlso Not Me.IsLowerInclusive Then
                    Return False
                End If
            End If
            If Me.HasUpper Then
                Dim ptr2 As __ByRef(Of T) = value
                If Nothing Is Nothing Then
                    Dim t As T = value
                    ptr2 = t
                End If
                Dim num2 As Integer = ptr2.CompareTo(Me.Upper)
                If num2 > 0 Then
                    Return False
                End If
                If num2 = 0 AndAlso Not Me.IsUpperInclusive Then
                    Return False
                End If
            End If
            Return True
        End Function

        Public Function IsEmpty() As Boolean
            If Not Me.HasLower OrElse Not Me.HasUpper Then
                Return False
            End If
            Dim lower As T = Me.Lower
            Dim num As Integer = lower.CompareTo(Me.Upper)
            Return num > 0 OrElse (num = 0 AndAlso (Not Me.IsLowerInclusive OrElse Not Me.IsUpperInclusive))
        End Function

        Public Function Intersect(other As ValueRange(Of T)) As ValueRange(Of T)
            Dim t As T = Nothing
            Dim t2 As T = Nothing
            Dim flag As Boolean = True
            Dim flag2 As Boolean = True
            If Me.HasLower AndAlso other.HasLower Then
                Dim t3 As T = Me.Lower
                Dim num As Integer = t3.CompareTo(other.Lower)
                If num <= 0 Then
                    If num >= 0 Then
                        t = Me.Lower
                        flag = Me.IsLowerInclusive AndAlso other.IsLowerInclusive
                    Else
                        t = other.Lower
                        flag = other.IsLowerInclusive
                    End If
                Else
                    t = Me.Lower
                    flag = Me.IsLowerInclusive
                End If
            ElseIf Me.HasLower Then
                t = Me.Lower
                flag = Me.IsLowerInclusive
            ElseIf other.HasLower Then
                t = other.Lower
                flag = other.IsLowerInclusive
            End If
            If Me.HasUpper AndAlso other.HasUpper Then
                Dim t3 As T = Me.Upper
                Dim num As Integer = t3.CompareTo(other.Upper)
                If num >= 0 Then
                    If num <= 0 Then
                        t2 = Me.Upper
                        flag2 = Me.IsUpperInclusive AndAlso other.IsUpperInclusive
                    Else
                        t2 = other.Upper
                        flag2 = other.IsUpperInclusive
                    End If
                Else
                    t2 = Me.Upper
                    flag2 = Me.IsUpperInclusive
                End If
            ElseIf Me.HasUpper Then
                t2 = Me.Upper
                flag2 = Me.IsUpperInclusive
            ElseIf other.HasUpper Then
                t2 = other.Upper
                flag2 = other.IsUpperInclusive
            End If
            Dim flag3 As Boolean = Me.HasLower OrElse other.HasLower
            Dim flag4 As Boolean = Me.HasUpper OrElse other.HasUpper
            Return New ValueRange(Of T)(t, t2, flag, flag2, flag3, flag4)
        End Function

        <NullableContext(2)>
        Public Overrides Overloads Function Equals(obj As Object) As Boolean
            Return Me.Equals(TryCast(obj, ValueRange(Of T)))
        End Function

        Public Overloads Function Equals(<Nullable(New Byte() { 2, 1 })> other As ValueRange(Of T)) As Boolean Implements System.IEquatable(Of MeloongCore.ValueRange(Of T)).Equals
            Return other IsNot Nothing AndAlso EqualityComparer(Of T).[Default].Equals(Me.Lower, other.Lower) AndAlso EqualityComparer(Of T).[Default].Equals(Me.Upper, other.Upper) AndAlso Me.HasLower = other.HasLower AndAlso Me.HasUpper = other.HasUpper AndAlso Me.IsLowerInclusive = other.IsLowerInclusive AndAlso Me.IsUpperInclusive = other.IsUpperInclusive
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return(((((169504101 * -1521134295 + EqualityComparer(Of T).[Default].GetHashCode(Me.Lower)) * -1521134295 + EqualityComparer(Of T).[Default].GetHashCode(Me.Upper)) * -1521134295 + Me.HasLower.GetHashCode()) * -1521134295 + Me.HasUpper.GetHashCode()) * -1521134295 + Me.IsLowerInclusive.GetHashCode()) * -1521134295 + Me.IsUpperInclusive.GetHashCode()
        End Function

        Public Shared Operator =(<Nullable(New Byte() { 2, 1 })> left As ValueRange(Of T), <Nullable(New Byte() { 2, 1 })> right As ValueRange(Of T)) As Boolean
            Return EqualityComparer(Of ValueRange(Of T)).[Default].Equals(left, right)
        End Operator

        Public Shared Operator <>(<Nullable(New Byte() { 2, 1 })> left As ValueRange(Of T), <Nullable(New Byte() { 2, 1 })> right As ValueRange(Of T)) As Boolean
            Return Not(left Is right)
        End Operator

        Public IsLowerInclusive As Boolean

        Public IsUpperInclusive As Boolean
    End Class
End Namespace
