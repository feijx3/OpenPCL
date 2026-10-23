Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class SettingService
        Public Shared Function GetKey(obj As DependencyObject) As String
            Dim text As String
            If obj Is Nothing Then
                text = Nothing
            Else
                text = Conversions.ToString(obj.GetValue(SettingService.KeyProperty))
            End If
            Return text
        End Function

        Public Shared Sub SetKey(obj As DependencyObject, value As String)
            obj.SetValue(SettingService.KeyProperty, value)
        End Sub

        Public Shared Function GetValue(obj As DependencyObject) As String
            Dim text As String
            If obj Is Nothing Then
                text = Nothing
            Else
                text = Conversions.ToString(obj.GetValue(SettingService.ValueProperty))
            End If
            Return text
        End Function

        Public Shared Sub SetValue(obj As DependencyObject, value As String)
            obj.SetValue(SettingService.ValueProperty, value)
        End Sub

        Public Shared Sub ResetSettings(Target As DependencyObject)
            Dim enumerable As IEnumerable(Of DependencyObject) = LogicalTreeHelper.GetChildren(Target).OfType()
            Dim action As Action(Of DependencyObject)
            If SettingService._Closure$__.$I8-0 IsNot Nothing Then
                action = SettingService._Closure$__.$I8-0
            Else
                Dim action2 As Action(Of DependencyObject) = Sub(c As DependencyObject)
                    SettingService.ResetSettings(c)
                End Sub
                action = action2
                SettingService._Closure$__.$I8-0 = action2
            End If
            enumerable.ForAll(action)
            Dim key As String = SettingService.GetKey(Target)
            If key IsNot Nothing Then
                Settings.Reset(key, PageInstanceLeft.Instance)
            End If
        End Sub

        Public Shared Sub RefreshSettings(Target As DependencyObject)
            Dim enumerable As IEnumerable(Of DependencyObject) = LogicalTreeHelper.GetChildren(Target).OfType()
            Dim action As Action(Of DependencyObject)
            If SettingService._Closure$__.$I9-0 IsNot Nothing Then
                action = SettingService._Closure$__.$I9-0
            Else
                Dim action2 As Action(Of DependencyObject) = Sub(c As DependencyObject)
                    SettingService.RefreshSettings(c)
                End Sub
                action = action2
                SettingService._Closure$__.$I9-0 = action2
            End If
            enumerable.ForAll(action)
            Dim key As String = SettingService.GetKey(Target)
            If key IsNot Nothing Then
                Dim objectValue As Object = RuntimeHelpers.GetObjectValue(Settings.[Get](key, PageInstanceLeft.Instance))
                If TypeOf Target Is ISettingControl Then
                    CType(Target, ISettingControl).RefreshSetting(Conversions.ToString(objectValue))
                End If
            End If
        End Sub

        Public Shared Sub SaveSetting(sender As Object)
            If ModAnimation.AniControlEnabled = 0 AndAlso TypeOf sender Is ISettingControl Then
                Dim key As String = SettingService.GetKey(CType(sender, DependencyObject))
                If key IsNot Nothing Then
                    Dim currentSetting As String = CType(sender, ISettingControl).GetCurrentSetting()
                    If currentSetting IsNot Nothing Then
                        Settings.[Set](key, currentSetting, PageInstanceLeft.Instance)
                    End If
                End If
            End If
        End Sub

        Public Shared KeyProperty As DependencyProperty = DependencyProperty.RegisterAttached("Key", GetType(String), GetType(SettingService), New PropertyMetadata(Nothing))

        Public Shared ValueProperty As DependencyProperty = DependencyProperty.RegisterAttached("Value", GetType(String), GetType(SettingService), New PropertyMetadata(Nothing))
    End Class
End Namespace
