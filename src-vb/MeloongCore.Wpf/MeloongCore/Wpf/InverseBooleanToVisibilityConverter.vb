Imports System
Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Data

Namespace MeloongCore.Wpf
    Public Class InverseBooleanToVisibilityConverter
        Implements IValueConverter

        <NullableContext(2)>
        Public Function Convert(value As Object, <Nullable(1)> targetType As Type, parameter As Object, <Nullable(1)> culture As CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
            If value Is Nothing Then
                Return Visibility.Visible
            End If
            Dim flag As Boolean
            Return If(Boolean.TryParse(value.ToString(), flag), If(flag, Visibility.Collapsed, Visibility.Visible), Visibility.Visible)
        End Function

        <NullableContext(2)>
        Public Function ConvertBack(value As Object, <Nullable(1)> targetType As Type, parameter As Object, <Nullable(1)> culture As CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
            If value Is Nothing Then
                Return False
            End If
            Dim flag As Boolean
            If TypeOf value Is Visibility Then
                Dim visibility As Visibility = CType(value, Visibility)
                flag = visibility > Visibility.Visible
            Else
                flag = False
            End If
            Return flag
        End Function
    End Class
End Namespace
