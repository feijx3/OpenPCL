Imports System
Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports System.Windows.Data

Namespace MeloongCore.Wpf
    Public Class InverseBooleanConverter
        Implements IValueConverter

        <NullableContext(2)>
        Public Function Convert(value As Object, <Nullable(1)> targetType As Type, parameter As Object, <Nullable(1)> culture As CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
            If value Is Nothing Then
                Return False
            End If
            Dim flag As Boolean
            Return Boolean.TryParse(value.ToString(), flag) AndAlso Not flag
        End Function

        <NullableContext(2)>
        Public Function ConvertBack(value As Object, <Nullable(1)> targetType As Type, parameter As Object, <Nullable(1)> culture As CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
            If value Is Nothing Then
                Return False
            End If
            Dim flag As Boolean
            Return Boolean.TryParse(value.ToString(), flag) AndAlso Not flag
        End Function
    End Class
End Namespace
