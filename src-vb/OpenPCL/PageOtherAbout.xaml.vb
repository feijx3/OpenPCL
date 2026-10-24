Imports System
Imports System.ComponentModel
Imports System.Windows
Imports System.Windows.Controls
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageOtherAbout
        Inherits OurPageRight

        Private _isInitialized As Boolean

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageOtherAbout_Loaded
            Me.InitializeComponent()
        End Sub

        Private Sub PageOtherAbout_Loaded(sender As Object, e As RoutedEventArgs)
            Me.PanBack.ScrollToHome()
            If Not Me._isInitialized Then
                Me._isInitialized = True
                Me.ItemAboutPcl.Info = Me.ItemAboutPcl.Info.Replace("%VERSION%", "快照版 2.99.0.0").Replace("%VERSIONCODE%", Conversions.ToString(414)).Replace("%BRANCH%", Conversions.ToString(0))
            End If
        End Sub

        Friend Overridable Property PanBack As OurScrollViewer
        Friend Overridable Property PanMain As StackPanel
        Friend Overridable Property ItemAboutPcl As OurListItem
    End Class
End Namespace
