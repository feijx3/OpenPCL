Imports System
Imports System.Collections.Generic
Imports System.Numerics
Imports System.Runtime.CompilerServices
Imports MeloongCore.Extensions

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Module MathUtils
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function ConvertRadix(input As String, fromRadix As Integer, toRadix As Integer) As String
            If fromRadix < 2 OrElse fromRadix > "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz/+=!?@#$%^&*()[]{}<>;:',".Length Then
                Throw New ArgumentOutOfRangeException("fromRadix", "fromRadix must be between 2 and 86.")
            End If
            If String.IsNullOrEmpty(input) Then
                Return "0"
            End If
            Dim flag As Boolean = input.StartsWithF("-", False)
            If flag Then
                input = input.TrimStart(New Char() { "-"c })
            End If
            Dim bigInteger As BigInteger = 0
            For Each c As Char In input
                Dim num As Integer = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz/+=!?@#$%^&*()[]{}<>;:',".IndexOf(c)
                If num = -1 OrElse num >= fromRadix Then
                    Throw New ArgumentException(String.Format("Character '{0}' in input '{1}' is not a valid digit for radix {2}.", c, input, fromRadix))
                End If
                bigInteger = bigInteger * fromRadix + num
            Next
            Return(bigInteger * If(flag, (-1), 1)).ConvertRadix(toRadix)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function ConvertRadix(input As Long, toRadix As Integer) As String
            Return input.ConvertRadix(toRadix)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function ConvertRadix(input As ULong, toRadix As Integer) As String
            Return input.ConvertRadix(toRadix)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function ConvertRadix(input As Integer, toRadix As Integer) As String
            Return input.ConvertRadix(toRadix)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function ConvertRadix(input As Byte, toRadix As Integer) As String
            Return input.ConvertRadix(toRadix)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function ConvertRadix(input As BigInteger, toRadix As Integer) As String
            If toRadix < 2 OrElse toRadix > "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz/+=!?@#$%^&*()[]{}<>;:',".Length Then
                Throw New ArgumentOutOfRangeException("toRadix", "toRadix must be between 2 and 86.")
            End If
            If input = 0L Then
                Return "0"
            End If
            Dim flag As Boolean = input < 0L
            Dim list As List(Of Char) = New List(Of Char)()
            Dim bigInteger As BigInteger = BigInteger.Abs(input)
            Dim bigInteger2 As BigInteger = toRadix
            While bigInteger > 0L
                Dim bigInteger3 As BigInteger
                bigInteger = BigInteger.DivRem(bigInteger, bigInteger2, bigInteger3)
                list.Add("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz/+=!?@#$%^&*()[]{}<>;:',"(CInt(bigInteger3)))
            End While
            If flag Then
                list.Add("-"c)
            End If
            list.Reverse()
            Return New String(list.ToArray())
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Clamp(Of T As IComparable(Of T))(value As T, min As T, max As T) As T
            If value.CompareTo(min) < 0 Then
                Return min
            End If
            If value.CompareTo(max) > 0 Then
                Return max
            End If
            Return value
        End Function

        Public Function Lerp(start As Double, [end] As Double, percentage As Double) As Double
            Return Math.Round(start + ([end] - start) * percentage, 7)
        End Function

        Public Function Bezier(t As Double, x1 As Double, y1 As Double, x2 As Double, y2 As Double, Optional accuracy As Double = 0.01) As Double
            If t < 1E-07 OrElse Double.IsNaN(t) Then
                Return 0.0
            End If
            If t > 0.9999999 Then
                Return 1.0
            End If
            Dim num As Double = t
            Dim num2 As Integer = 1000
            Dim num3 As Double
            Do
                num3 = 3.0 * num * ((0.33333333 + x1 - x2) * num * num + (x2 - 2.0 * x1) * num + x1)
                num += (t - num3) * 0.5
                num2 -= 1
            Loop While Math.Abs(num3 - t) >= accuracy AndAlso num2 > 0
            Return 3.0 * num * ((0.33333333 + y1 - y2) * num * num + (y2 - 2.0 * y1) * num + y1)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function IsNear(value As Double, target As Double, Optional tolerance As Double = 1E-06) As Boolean
            Return Math.Abs(value - target) < tolerance
        End Function

        Public Function GetPrimeFactors(num As Integer) As IEnumerable(Of Integer)
            Dim <GetPrimeFactors>d__ As MathUtils.<GetPrimeFactors>d__10 = New MathUtils.<GetPrimeFactors>d__10(-2)
            <GetPrimeFactors>d__.<>3__num = num
            Return <GetPrimeFactors>d__
        End Function

        Public Function Gcd(a As Integer, b As Integer) As Integer
            a = Math.Abs(a)
            b = Math.Abs(b)
            While b <> 0
                Dim num As Integer = b
                b = a Mod b
                a = num
            End While
            Return a
        End Function

        Public Function Gcd(a As Double, b As Integer) As Integer
            Return MathUtils.Gcd(CInt(Math.Round(a)), b)
        End Function
    End Module
End Namespace
