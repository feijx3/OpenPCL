Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Globalization
Imports System.Resources
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL.My.Resources
    <GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")>
    <DebuggerNonUserCode()>
    <CompilerGenerated()>
    <HideModuleName()>
    Friend NotInheritable Module Resources
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Friend ReadOnly Property ResourceManager As ResourceManager
            Get
                If Object.ReferenceEquals(Resources.resourceMan, Nothing) Then
                    Resources.resourceMan = New ResourceManager("PCL.Resources", GetType(Resources).Assembly)
                End If
                Return Resources.resourceMan
            End Get
        End Property

        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Friend Property Culture As CultureInfo
            Get
                Return Resources.resourceCulture
            End Get
            Set(value As CultureInfo)
                Resources.resourceCulture = value
            End Set
        End Property

        Friend ReadOnly Property CacheCow_Client As Byte()
            Get
                Return CType(RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("CacheCow.Client", Resources.resourceCulture)), Byte())
            End Get
        End Property

        Friend ReadOnly Property CacheCow_Client_FileStore As Byte()
            Get
                Return CType(RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("CacheCow.Client.FileStore", Resources.resourceCulture)), Byte())
            End Get
        End Property

        Friend ReadOnly Property CacheCow_Common As Byte()
            Get
                Return CType(RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("CacheCow.Common", Resources.resourceCulture)), Byte())
            End Get
        End Property

        Friend ReadOnly Property ThrottleDebounce As Byte()
            Get
                Return CType(RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("ThrottleDebounce", Resources.resourceCulture)), Byte())
            End Get
        End Property

        Friend ReadOnly Property Custom As Byte()
            Get
                Return CType(RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("Custom", Resources.resourceCulture)), Byte())
            End Get
        End Property

        Friend ReadOnly Property ForgeInstaller As Byte()
            Get
                Return CType(RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("ForgeInstaller", Resources.resourceCulture)), Byte())
            End Get
        End Property

        Friend ReadOnly Property Help As Byte()
            Get
                Return CType(RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("Help", Resources.resourceCulture)), Byte())
            End Get
        End Property

        Friend ReadOnly Property Imazen_WebP As Byte()
            Get
                Return CType(RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("Imazen.WebP", Resources.resourceCulture)), Byte())
            End Get
        End Property

        Friend ReadOnly Property libwebp64 As Byte()
            Get
                Return CType(RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("libwebp64", Resources.resourceCulture)), Byte())
            End Get
        End Property

        Friend ReadOnly Property MeloongCore As Byte()
            Get
                Return CType(RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("MeloongCore", Resources.resourceCulture)), Byte())
            End Get
        End Property

        Friend ReadOnly Property MeloongCore_Wpf As Byte()
            Get
                Return CType(RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("MeloongCore.Wpf", Resources.resourceCulture)), Byte())
            End Get
        End Property

        Friend ReadOnly Property NAudio As Byte()
            Get
                Return CType(RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("NAudio", Resources.resourceCulture)), Byte())
            End Get
        End Property

        Friend ReadOnly Property Newtonsoft_Json As Byte()
            Get
                Return CType(RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("Newtonsoft.Json", Resources.resourceCulture)), Byte())
            End Get
        End Property

        Friend ReadOnly Property Ookii_Dialogs_Wpf As Byte()
            Get
                Return CType(RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("Ookii.Dialogs.Wpf", Resources.resourceCulture)), Byte())
            End Get
        End Property

        Friend ReadOnly Property PCLCS As Byte()
            Get
                Return CType(RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("PCLCS", Resources.resourceCulture)), Byte())
            End Get
        End Property

        Friend ReadOnly Property System_Net_Http_Formatting As Byte()
            Get
                Return CType(RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("System.Net.Http.Formatting", Resources.resourceCulture)), Byte())
            End Get
        End Property

        Friend ReadOnly Property Transformer As Byte()
            Get
                Return CType(RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject("Transformer", Resources.resourceCulture)), Byte())
            End Get
        End Property

        Private resourceMan As ResourceManager

        Private resourceCulture As CultureInfo
    End Module
End Namespace
