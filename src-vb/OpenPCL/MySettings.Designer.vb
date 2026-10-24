Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Configuration
Imports System.Runtime.CompilerServices

Namespace PCL
    <CompilerGenerated()>
    <GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.0.3.0")>
    <EditorBrowsable(EditorBrowsableState.Advanced)>
    Friend NotInheritable Partial Class MySettings
        Inherits ApplicationSettingsBase

        Public Shared ReadOnly Property [Default] As MySettings
            Get
                Return MySettings.defaultInstance
            End Get
        End Property

        Private Shared defaultInstance As MySettings = CType(SettingsBase.Synchronized(New MySettings()), MySettings)
    End Class
End Namespace
