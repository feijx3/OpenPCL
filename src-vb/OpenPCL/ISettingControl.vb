Imports System

Namespace PCL
    Public Interface ISettingControl
        Sub RefreshSetting(NewValue As String)

        Function GetCurrentSetting() As String
    End Interface
End Namespace
