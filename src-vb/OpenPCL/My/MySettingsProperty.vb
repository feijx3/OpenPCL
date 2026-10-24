Imports System
Imports System.ComponentModel.Design
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL.My
    <HideModuleName()>
    <DebuggerNonUserCode()>
    <CompilerGenerated()>
    Friend NotInheritable Module MySettingsProperty
        <HelpKeyword("My.Settings")>
        Friend ReadOnly Property Settings As MySettings
            Get
                Return MySettings.[Default]
            End Get
        End Property
    End Module
End Namespace
