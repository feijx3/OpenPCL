Imports System
Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports System.Windows.Data

Namespace MeloongCore.Wpf
    Public Class AdditionConverter
        Implements IValueConverter

        <NullableContext(2)>
        Public Function Convert(value As Object, <Nullable(1)> targetType As Type, parameter As Object, <Nullable(1)> culture As CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
            If value Is Nothing Then
                Return 0
            End If
            Dim num As Double
            If Not Double.TryParse(value.ToString(), num) Then
                Return 0
            End If
            Dim num2 As Double = 1.0
            If parameter IsNot Nothing Then
                Double.TryParse(parameter.ToString(), num2)
            End If
            Return num + num2
        End Function

        <NullableContext(2)>
        Public Function ConvertBack(value As Object, <Nullable(1)> targetType As Type, parameter As Object, <Nullable(1)> culture As CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
            If value Is Nothing Then
                Return Binding.DoNothing
            End If
            Dim num As Double
            If Not Double.TryParse(value.ToString(), num) Then
                Return Binding.DoNothing
            End If
            Dim num2 As Double = 1.0
            If parameter IsNot Nothing Then
                Double.TryParse(parameter.ToString(), num2)
            End If
            Return num - num2
        End Function
    End Class
End Namespace
