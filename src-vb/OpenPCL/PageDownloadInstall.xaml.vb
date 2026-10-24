Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json.Linq
Imports PCLCS

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageDownloadInstall
        Inherits OurPageRight

        Public Sub New()
            AddHandler MyBase.Initialized, Sub(a0 As Object, a1 As EventArgs)
                Me.LoaderInit()
            End Sub
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.Init()
            End Sub
            Me.IsLoad = False
            Me.IsInSelectPage = False
            Me.SelectedOptiFine = Nothing
            Me.SelectedLiteLoader = Nothing
            Me.SelectedForge = Nothing
            Me.SelectedNeoForge = Nothing
            Me.SelectedFabric = Nothing
            Me.SelectedFabricApi = Nothing
            Me.SelectedOptiFabric = Nothing
            Me.IsSelectNameEdited = False
            Me.AutoSelectedFabricApi = False
            Me.AutoSelectedOptiFabric = False
            Me.InitializeComponent()
        End Sub

        Public Sub ControlInit()
            Me.DisabledPageAnimControls.Add(Me.BtnStart)
            Me.LoadOptiFine.State = ModDownload.DlOptiFineListLoader
            Me.LoadLiteLoader.State = ModDownload.DlLiteLoaderListLoader
            Me.LoadFabric.State = ModDownload.DlFabricListLoader
            Me.LoadFabricApi.State = ModDownload.DlFabricApiLoader
            Me.LoadNeoForge.State = ModDownload.DlNeoForgeListLoader
            Me.LoadOptiFabric.State = ModDownload.DlOptiFabricLoader
            ModDownloadLib.McDownloadForgeRecommendedRefresh()
        End Sub

        Public Overridable Sub LoaderInit()
            Me.ControlInit()
            Dim loadMinecraft As OurLoading = Me.LoadMinecraft
            Dim panLoad As FrameworkElement = Me.PanLoad
            Dim panAllBack As FrameworkElement = Me.PanAllBack
            Dim frameworkElement As FrameworkElement = Nothing
            MyBase.PageLoaderInit(Of String, ModDownload.DlClientListResult)(loadMinecraft, panLoad, panAllBack, frameworkElement, ModDownload.DlClientListLoader, Sub(a0 As ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult))
                Me.LoadMinecraft_OnFinish()
            End Sub, Nothing, True)
        End Sub

        Public Overridable Sub Init()
            If Me.IsLoad Then
                Me.IsSelectNameEdited = False
                Me.ExitSelectPage()
                If ModDownload.DlClientListLoader.State = ModBase.LoadState.Finished Then
                    Me.TrySelectWaitingVersion()
                End If
            End If
            Me.IsLoad = True
            Me.PanBack.ScrollToHome()
            ModDownload.DlOptiFineListLoader.Start(Nothing, False)
            ModDownload.DlLiteLoaderListLoader.Start(Nothing, False)
            ModDownload.DlFabricListLoader.Start(Nothing, False)
            ModDownload.DlNeoForgeListLoader.Start(Nothing, False)
            Me.TextSelectName.ValidateRules = New Collection(Of Validate)() From { New ValidateFolderName(ModMinecraft.McFolderSelected + "versions", True, Nothing) }
            Me.TextSelectName.Validate()
            Me.ReloadSelected()
        End Sub

        Public Sub PrepareSelectPage()
            Me.IsInSelectPage = True
            Me.PanInner.Margin = New Thickness(25.0, 10.0, 25.0, 40.0)
            Me.PanSelect.Visibility = Visibility.Visible
            Me.PanSelect.IsHitTestVisible = True
            Me.PanMinecraft.IsHitTestVisible = False
            Me.PanBack.IsHitTestVisible = False
            Me.PanBack.ScrollToHome()
            Me.DisabledPageAnimControls.Remove(Me.BtnStart)
            Me.BtnStart.Show = True
            Dim array As OurCard() = New OurCard() { Me.CardOptiFine, Me.CardLiteLoader, Me.CardForge, Me.CardNeoForge, Me.CardFabric, Me.CardFabricApi, Me.CardOptiFabric }
            For i As Integer = 0 To array.Length - 1
                array(i).IsSwapped = True
            Next
            Try
                For Each frameworkElement As FrameworkElement In MyBase.GetAllAnimControls(Me.PanSelect, False)
                    frameworkElement.Opacity = 1.0
                    If frameworkElement.RenderTransform Is Nothing OrElse TypeOf frameworkElement.RenderTransform Is TranslateTransform Then
                        frameworkElement.RenderTransform = New TranslateTransform()
                    End If
                Next
            Finally
                Dim enumerator As IEnumerator(Of FrameworkElement)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
        End Sub

        Private Sub EnterSelectPage()
            If Not Me.IsInSelectPage Then
                Me.AutoSelectedFabricApi = False
                Me.AutoSelectedOptiFabric = False
                Me.PrepareSelectPage()
                If McVersion.IsStandardFormat(Me.VanillaName) Then
                    Dim loaderTask As ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)) = New ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry))("DlForgeVersion " + Me.VanillaName, AddressOf ModDownload.DlForgeVersionMain, Nothing, ThreadPriority.Normal)
                    Me.LoadForge.State = loaderTask
                    loaderTask.Start(Me.VanillaName, False)
                End If
                ModDownload.DlFabricApiLoader.Start(Nothing, False)
                ModDownload.DlOptiFabricLoader.Start(Nothing, False)
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.PanMinecraft, -Me.PanMinecraft.Opacity, 70, 10, Nothing, False), ModAnimation.AaTranslateX(Me.PanMinecraft, -50.0 - CType(Me.PanMinecraft.RenderTransform, TranslateTransform).X, 90, 10, Nothing, False), ModAnimation.AaCode(Sub()
                    Me.PanBack.ScrollToHome()
                    Me.ReloadComponentLists()
                    Me.PanMinecraft.Visibility = Visibility.Collapsed
                End Sub, 0, True), ModAnimation.AaOpacity(Me.PanSelect, 1.0 - Me.PanSelect.Opacity, 70, 100, Nothing, False), ModAnimation.AaTranslateX(Me.PanSelect, -CType(Me.PanSelect.RenderTransform, TranslateTransform).X, 160, 100, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.ExtraStrong), False), ModAnimation.AaCode(Sub()
                    Me.PanBack.IsHitTestVisible = True
                End Sub, 0, True) }, "FrmDownloadInstall SelectPageSwitch", True)
            End If
        End Sub

        Public Sub ReloadComponentLists()
            Me.TextSelectName.Validate()
            Me.OptiFine_Loaded()
            Me.LiteLoader_Loaded()
            Me.Forge_Loaded()
            Me.NeoForge_Loaded()
            Me.Fabric_Loaded()
            Me.FabricApi_Loaded()
            Me.OptiFabric_Loaded()
            Me.ReloadSelected()
        End Sub

        Public Sub ExitSelectPage()
            If Me.IsInSelectPage Then
                Me.IsInSelectPage = False
                Me.PanInner.Margin = New Thickness(25.0, 10.0, 25.0, 25.0)
                Me.DisabledPageAnimControls.Add(Me.BtnStart)
                Me.BtnStart.Show = False
                Me.ClearSelected()
                Me.PanMinecraft.Visibility = Visibility.Visible
                Me.PanSelect.IsHitTestVisible = False
                Me.PanMinecraft.IsHitTestVisible = True
                Me.PanBack.IsHitTestVisible = False
                Me.PanBack.ScrollToHome()
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.PanSelect, -Me.PanSelect.Opacity, 70, 10, Nothing, False), ModAnimation.AaTranslateX(Me.PanSelect, 50.0 - CType(Me.PanSelect.RenderTransform, TranslateTransform).X, 90, 10, Nothing, False), ModAnimation.AaCode(Sub()
                    Me.PanBack.ScrollToHome()
                End Sub, 0, True), ModAnimation.AaOpacity(Me.PanMinecraft, 1.0 - Me.PanMinecraft.Opacity, 70, 100, Nothing, False), ModAnimation.AaTranslateX(Me.PanMinecraft, -CType(Me.PanMinecraft.RenderTransform, TranslateTransform).X, 160, 100, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.ExtraStrong), False), ModAnimation.AaCode(Sub()
                    Me.PanSelect.Visibility = Visibility.Collapsed
                    Me.PanBack.IsHitTestVisible = True
                End Sub, 0, True) }, "FrmDownloadInstall SelectPageSwitch", False)
            End If
        End Sub

        Public Overridable Sub MinecraftSelected(sender As OurListItem, e As MouseButtonEventArgs)
            Me.VanillaName = sender.Title
            Me.VanillaData = CType(sender.Tag, JObject)
            Me.VanillaIcon = sender.Logo
            Me.LabMinecraftVersion.Text = Me.VanillaName
            Me.EnterSelectPage()
        End Sub

        Private ReadOnly Property VanillaDrop As Integer
            Get
                Return McVersion.VersionToDrop(Me.VanillaName)
            End Get
        End Property

        Public Overridable Sub ReloadSelected()
            If Me.$STATIC$ReloadSelected$2001$Ongoing$Init Is Nothing Then
                Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$ReloadSelected$2001$Ongoing$Init, New StaticLocalInitFlag(), Nothing)
            End If
            Dim flag As Boolean = False
            Try
                Monitor.Enter(Me.$STATIC$ReloadSelected$2001$Ongoing$Init, flag)
                If Me.$STATIC$ReloadSelected$2001$Ongoing$Init.State = 0S Then
                    Me.$STATIC$ReloadSelected$2001$Ongoing$Init.State = 2S
                    Me.$STATIC$ReloadSelected$2001$Ongoing = False
                ElseIf Me.$STATIC$ReloadSelected$2001$Ongoing$Init.State = 2S Then
                    Throw New IncompleteInitialization()
                End If
            Finally
                Me.$STATIC$ReloadSelected$2001$Ongoing$Init.State = 1S
                If flag Then
                    Monitor.[Exit](Me.$STATIC$ReloadSelected$2001$Ongoing$Init)
                End If
            End Try
            If Me.VanillaName IsNot Nothing AndAlso Not Me.$STATIC$ReloadSelected$2001$Ongoing Then
                Me.$STATIC$ReloadSelected$2001$Ongoing = True
                Me.SelectNameUpdate()
                Me.ImgLogo.Source = Me.GetSelectLogo()
                Me.ImgMinecraft.Source = Me.VanillaIcon
                Dim text As String = Me.LoadOptiFineGetError()
                Me.CardOptiFine.MainSwap.Visibility = If((text Is Nothing), Visibility.Visible, Visibility.Collapsed)
                If text IsNot Nothing Then
                    Me.CardOptiFine.IsSwapped = True
                End If
                Me.SetPanelVisibility(Me.PanOptiFineInfo, Me.CardOptiFine.IsSwapped)
                If Me.SelectedOptiFine Is Nothing Then
                    Me.BtnOptiFineClear.Visibility = Visibility.Collapsed
                    Me.ImgOptiFine.Visibility = Visibility.Collapsed
                    Me.LabOptiFine.Text = If(text, "可以添加")
                    Me.LabOptiFine.Foreground = ModSecret.ColorGray4
                Else
                    Me.BtnOptiFineClear.Visibility = Visibility.Visible
                    Me.ImgOptiFine.Visibility = Visibility.Visible
                    Me.LabOptiFine.Text = Me.SelectedOptiFine.DisplayName.Replace(Me.VanillaName + " ", "")
                    Me.LabOptiFine.Foreground = ModSecret.ColorGray1
                End If
                If Me.VanillaDrop >= 130 Then
                    Me.CardLiteLoader.Visibility = Visibility.Collapsed
                Else
                    Me.CardLiteLoader.Visibility = Visibility.Visible
                    Dim text2 As String = Me.LoadLiteLoaderGetError()
                    Me.CardLiteLoader.MainSwap.Visibility = If((text2 Is Nothing), Visibility.Visible, Visibility.Collapsed)
                    If text2 IsNot Nothing Then
                        Me.CardLiteLoader.IsSwapped = True
                    End If
                    Me.SetPanelVisibility(Me.PanLiteLoaderInfo, Me.CardLiteLoader.IsSwapped)
                    If Me.SelectedLiteLoader Is Nothing Then
                        Me.BtnLiteLoaderClear.Visibility = Visibility.Collapsed
                        Me.ImgLiteLoader.Visibility = Visibility.Collapsed
                        Me.LabLiteLoader.Text = If(text2, "可以添加")
                        Me.LabLiteLoader.Foreground = ModSecret.ColorGray4
                    Else
                        Me.BtnLiteLoaderClear.Visibility = Visibility.Visible
                        Me.ImgLiteLoader.Visibility = Visibility.Visible
                        Me.LabLiteLoader.Text = Me.SelectedLiteLoader.Inherit
                        Me.LabLiteLoader.Foreground = ModSecret.ColorGray1
                    End If
                End If
                If Not McVersion.IsStandardFormat(Me.VanillaName) Then
                    Me.CardForge.Visibility = Visibility.Collapsed
                Else
                    Me.CardForge.Visibility = Visibility.Visible
                    Dim text3 As String = Me.LoadForgeGetError()
                    Me.CardForge.MainSwap.Visibility = If((text3 Is Nothing), Visibility.Visible, Visibility.Collapsed)
                    If text3 IsNot Nothing Then
                        Me.CardForge.IsSwapped = True
                    End If
                    Me.SetPanelVisibility(Me.PanForgeInfo, Me.CardForge.IsSwapped)
                    If Me.SelectedForge Is Nothing Then
                        Me.BtnForgeClear.Visibility = Visibility.Collapsed
                        Me.ImgForge.Visibility = Visibility.Collapsed
                        Me.LabForge.Text = If(text3, "可以添加")
                        Me.LabForge.Foreground = ModSecret.ColorGray4
                    Else
                        Me.BtnForgeClear.Visibility = Visibility.Visible
                        Me.ImgForge.Visibility = Visibility.Visible
                        Me.LabForge.Text = Me.SelectedForge.VersionName
                        Me.LabForge.Foreground = ModSecret.ColorGray1
                    End If
                End If
                If DateTime.Compare(Me.VanillaData("releaseTime").ToObject(Of DateTime)(), New DateTime(2023, 6, 11)) < 0 Then
                    Me.CardNeoForge.Visibility = Visibility.Collapsed
                Else
                    Me.CardNeoForge.Visibility = Visibility.Visible
                    Dim text4 As String = Me.LoadNeoForgeGetError()
                    Me.CardNeoForge.MainSwap.Visibility = If((text4 Is Nothing), Visibility.Visible, Visibility.Collapsed)
                    If text4 IsNot Nothing Then
                        Me.CardNeoForge.IsSwapped = True
                    End If
                    Me.SetPanelVisibility(Me.PanNeoForgeInfo, Me.CardNeoForge.IsSwapped)
                    If Me.SelectedNeoForge Is Nothing Then
                        Me.BtnNeoForgeClear.Visibility = Visibility.Collapsed
                        Me.ImgNeoForge.Visibility = Visibility.Collapsed
                        Me.LabNeoForge.Text = If(text4, "可以添加")
                        Me.LabNeoForge.Foreground = ModSecret.ColorGray4
                    Else
                        Me.BtnNeoForgeClear.Visibility = Visibility.Visible
                        Me.ImgNeoForge.Visibility = Visibility.Visible
                        Me.LabNeoForge.Text = Me.SelectedNeoForge.VersionName
                        Me.LabNeoForge.Foreground = ModSecret.ColorGray1
                    End If
                End If
                If Me.VanillaDrop <= 130 Then
                    Me.CardFabric.Visibility = Visibility.Collapsed
                Else
                    Me.CardFabric.Visibility = Visibility.Visible
                    Dim text5 As String = Me.LoadFabricGetError()
                    Me.CardFabric.MainSwap.Visibility = If((text5 Is Nothing), Visibility.Visible, Visibility.Collapsed)
                    If text5 IsNot Nothing Then
                        Me.CardFabric.IsSwapped = True
                    End If
                    Me.SetPanelVisibility(Me.PanFabricInfo, Me.CardFabric.IsSwapped)
                    If Me.SelectedFabric Is Nothing Then
                        Me.BtnFabricClear.Visibility = Visibility.Collapsed
                        Me.ImgFabric.Visibility = Visibility.Collapsed
                        Me.LabFabric.Text = If(text5, "可以添加")
                        Me.LabFabric.Foreground = ModSecret.ColorGray4
                    Else
                        Me.BtnFabricClear.Visibility = Visibility.Visible
                        Me.ImgFabric.Visibility = Visibility.Visible
                        Me.LabFabric.Text = Me.SelectedFabric.Replace("+build", "")
                        Me.LabFabric.Foreground = ModSecret.ColorGray1
                    End If
                End If
                If Me.SelectedFabric Is Nothing Then
                    Me.CardFabricApi.Visibility = Visibility.Collapsed
                Else
                    Me.CardFabricApi.Visibility = Visibility.Visible
                    Dim text6 As String = Me.LoadFabricApiGetError()
                    Me.CardFabricApi.MainSwap.Visibility = If((text6 Is Nothing), Visibility.Visible, Visibility.Collapsed)
                    If text6 IsNot Nothing OrElse Me.SelectedFabric Is Nothing Then
                        Me.CardFabricApi.IsSwapped = True
                    End If
                    Me.SetPanelVisibility(Me.PanFabricApiInfo, Me.CardFabricApi.IsSwapped)
                    If Me.SelectedFabricApi Is Nothing Then
                        Me.BtnFabricApiClear.Visibility = Visibility.Collapsed
                        Me.ImgFabricApi.Visibility = Visibility.Collapsed
                        Me.LabFabricApi.Text = If(text6, "可以添加")
                        Me.LabFabricApi.Foreground = ModSecret.ColorGray4
                    Else
                        Me.BtnFabricApiClear.Visibility = Visibility.Visible
                        Me.ImgFabricApi.Visibility = Visibility.Visible
                        Me.LabFabricApi.Text = Me.SelectedFabricApi.Display.Split("]", False)(1).Replace("Fabric API ", "").Replace(" build ", ".").Split("+", False).First().Trim()
                        Me.LabFabricApi.Foreground = ModSecret.ColorGray1
                    End If
                End If
                If Me.SelectedFabric Is Nothing OrElse Me.SelectedOptiFine Is Nothing Then
                    Me.CardOptiFabric.Visibility = Visibility.Collapsed
                Else
                    Me.CardOptiFabric.Visibility = Visibility.Visible
                    Dim text7 As String = Me.LoadOptiFabricGetError()
                    Me.CardOptiFabric.MainSwap.Visibility = If((text7 Is Nothing), Visibility.Visible, Visibility.Collapsed)
                    If text7 IsNot Nothing OrElse Me.SelectedFabric Is Nothing Then
                        Me.CardOptiFabric.IsSwapped = True
                    End If
                    Me.SetPanelVisibility(Me.PanOptiFabricInfo, Me.CardOptiFabric.IsSwapped)
                    If Me.SelectedOptiFabric Is Nothing Then
                        Me.BtnOptiFabricClear.Visibility = Visibility.Collapsed
                        Me.ImgOptiFabric.Visibility = Visibility.Collapsed
                        Me.LabOptiFabric.Text = If(text7, "可以添加")
                        Me.LabOptiFabric.Foreground = ModSecret.ColorGray4
                    Else
                        Me.BtnOptiFabricClear.Visibility = Visibility.Visible
                        Me.ImgOptiFabric.Visibility = Visibility.Visible
                        Me.LabOptiFabric.Text = Me.SelectedOptiFabric.Display.Lower().Replace("optifabric-", "").Replace(".jar", "").Trim().TrimStart(New Char() { "v"c })
                        Me.LabOptiFabric.Foreground = ModSecret.ColorGray1
                    End If
                End If
                If Me.SelectedFabric IsNot Nothing AndAlso Me.SelectedFabricApi Is Nothing Then
                    Me.HintFabricAPI.Visibility = Visibility.Visible
                Else
                    Me.HintFabricAPI.Visibility = Visibility.Collapsed
                End If
                If Me.SelectedFabric IsNot Nothing AndAlso Me.SelectedOptiFine IsNot Nothing AndAlso Me.SelectedOptiFabric Is Nothing Then
                    If Me.VanillaDrop >= 140 AndAlso Me.VanillaDrop <= 150 Then
                        Me.HintOptiFabric.Visibility = Visibility.Collapsed
                        Me.HintOptiFabricOld.Visibility = Visibility.Visible
                    Else
                        Me.HintOptiFabric.Visibility = Visibility.Visible
                        Me.HintOptiFabricOld.Visibility = Visibility.Collapsed
                    End If
                Else
                    Me.HintOptiFabric.Visibility = Visibility.Collapsed
                    Me.HintOptiFabricOld.Visibility = Visibility.Collapsed
                End If
                If Me.VanillaDrop >= 160 AndAlso Me.SelectedOptiFine IsNot Nothing AndAlso (Me.SelectedForge IsNot Nothing OrElse Me.SelectedFabric IsNot Nothing) Then
                    Me.HintModOptiFine.Visibility = Visibility.Visible
                Else
                    Me.HintModOptiFine.Visibility = Visibility.Collapsed
                End If
                Me.$STATIC$ReloadSelected$2001$Ongoing = False
            End If
        End Sub

        Public Sub ClearSelected()
            Me.VanillaName = Nothing
            Me.VanillaData = Nothing
            Me.VanillaIcon = Nothing
            Me.SelectedOptiFine = Nothing
            Me.SelectedLiteLoader = Nothing
            Me.SelectedForge = Nothing
            Me.SelectedNeoForge = Nothing
            Me.SelectedFabric = Nothing
            Me.SelectedFabricApi = Nothing
            Me.SelectedOptiFabric = Nothing
            Dim array As OurIconButton() = New OurIconButton() { Me.BtnOptiFineClear, Me.BtnLiteLoaderClear, Me.BtnForgeClear, Me.BtnNeoForgeClear, Me.BtnFabricClear, Me.BtnFabricApiClear, Me.BtnOptiFabricClear }
            For i As Integer = 0 To array.Length - 1
                array(i).Visibility = Visibility.Collapsed
            Next
        End Sub

        Private Sub SetPanelVisibility(Panel As Grid, IsVisible As Boolean)
            If Not Operators.ConditionalCompareObjectEqual(Panel.Tag, IsVisible.ToString(), False) Then
                Panel.Tag = IsVisible.ToString()
                If IsVisible Then
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaTranslateY(Panel, -CType(Panel.RenderTransform, TranslateTransform).Y, 150, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaOpacity(Panel, 1.0 - Panel.Opacity, 60, 0, Nothing, False) }, "PageDownloadInstall Visibility " + Panel.Name, False)
                    Return
                End If
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaTranslateY(Panel, 6.0 - CType(Panel.RenderTransform, TranslateTransform).Y, 60, 0, Nothing, False), ModAnimation.AaOpacity(Panel, -Panel.Opacity, 60, 0, Nothing, False) }, "PageDownloadInstall Visibility " + Panel.Name, False)
            End If
        End Sub

        Public Overridable Function GetSelectLogo() As String
            Dim text As String
            If Me.SelectedFabric IsNot Nothing Then
                text = "pack://application:,,,/images/Blocks/Fabric.png"
            ElseIf Me.SelectedForge IsNot Nothing Then
                text = "pack://application:,,,/images/Blocks/Anvil.png"
            ElseIf Me.SelectedNeoForge IsNot Nothing Then
                text = "pack://application:,,,/images/Blocks/NeoForge.png"
            ElseIf Me.SelectedLiteLoader IsNot Nothing Then
                text = "pack://application:,,,/images/Blocks/Egg.png"
            ElseIf Me.SelectedOptiFine IsNot Nothing Then
                text = "pack://application:,,,/images/Blocks/GrassPath.png"
            Else
                text = Me.VanillaIcon
            End If
            Return text
        End Function

        Public Function GetSelectName() As String
            Dim text As String = Me.VanillaName
            If Me.SelectedFabric IsNot Nothing Then
                text = text + "-Fabric " + Me.SelectedFabric.BeforeFirst("+", False)
            End If
            If Me.SelectedForge IsNot Nothing Then
                text = text + "-Forge_" + Me.SelectedForge.VersionName
            End If
            If Me.SelectedNeoForge IsNot Nothing Then
                text = text + "-NeoForge_" + Me.SelectedNeoForge.VersionName.BeforeFirst("+", False)
            End If
            If Me.SelectedLiteLoader IsNot Nothing Then
                text += "-LiteLoader"
            End If
            If Me.SelectedOptiFine IsNot Nothing Then
                text = text + "-OptiFine_" + Me.SelectedOptiFine.DisplayName.Replace(Me.VanillaName + " ", "").Replace(" ", "_")
            End If
            Return text
        End Function

        Private Sub SelectNameUpdate()
            If Not Me.IsSelectNameEdited Then
                Me.TextSelectName.Text = Me.GetSelectName()
                Me.IsSelectNameEdited = False
            End If
        End Sub

        Private Sub TextSelectName_TextChanged(sender As Object, e As TextChangedEventArgs)
            Me.IsSelectNameEdited = True
        End Sub

        Private Sub TextSelectName_ValidateChanged(sender As Object, e As EventArgs)
            Me.BtnStart.IsEnabled = Me.TextSelectName.IsValidated
            Me.HintSelectName.Text = Me.TextSelectName.ValidateResult
            Me.HintSelectName.Visibility = If(Me.TextSelectName.IsValidated, Visibility.Collapsed, Visibility.Visible)
        End Sub

        Private Sub LoadMinecraft_OnFinish()
            Me.MinecraftVersions = CType(ModDownload.DlClientListLoader.Output.Value("versions").DeepClone(), JArray)
            Me.ShowMinecraftVersionList()
            Me.TrySelectWaitingVersion()
        End Sub

        Public Overridable Sub ShowMinecraftVersionList()
            Me.ExitSelectPage()
            Try
                Dim dictionary As Dictionary(Of String, List(Of JObject)) = New Dictionary(Of String, List(Of JObject))() From { { "正式版", New List(Of JObject)() }, { "预览版", New List(Of JObject)() }, { "远古版", New List(Of JObject)() }, { "愚人节版", New List(Of JObject)() } }
                Try
                    For Each jtoken As JToken In Me.MinecraftVersions
                        Dim jobject As JObject = CType(jtoken, JObject)
                        Dim text As String = CStr(jobject("type"))
                        If Operators.CompareString(text, "release", False) <> 0 Then
                            If Operators.CompareString(text, "snapshot", False) <> 0 Then
                                If Operators.CompareString(text, "special", False) <> 0 Then
                                    text = "远古版"
                                Else
                                    text = "愚人节版"
                                End If
                            Else
                                text = "预览版"
                                If jobject("id").ToString().StartsWithF("1.", False) AndAlso Not jobject("id").ToString().Lower().Contains("combat") AndAlso Not jobject("id").ToString().Lower().Contains("rc") AndAlso Not jobject("id").ToString().Lower().Contains("experimental") AndAlso Not jobject("id").ToString().Lower().Contains("pre") Then
                                    text = "正式版"
                                    jobject("type") = "release"
                                End If
                                Dim text2 As String = jobject("id").ToString().Lower()
                                Dim num As UInteger = <PrivateImplementationDetails>.ComputeStringHash(text2)
                                If num <= 2812350463UI Then
                                    If num <= 1442174922UI Then
                                        If num <> 673058499UI Then
                                            If num <> 1442174922UI Then
                                                GoTo IL_03F7
                                            End If
                                            If Operators.CompareString(text2, "26w14a", False) <> 0 Then
                                                GoTo IL_03F7
                                            End If
                                        ElseIf Operators.CompareString(text2, "2.0", False) <> 0 Then
                                            GoTo IL_03F7
                                        End If
                                    ElseIf num <> 1946126673UI Then
                                        If num <> 2783260049UI Then
                                            If num <> 2812350463UI Then
                                                GoTo IL_03F7
                                            End If
                                            If Operators.CompareString(text2, "23w13a_or_b", False) <> 0 Then
                                                GoTo IL_03F7
                                            End If
                                        ElseIf Operators.CompareString(text2, "1.rv-pre1", False) <> 0 Then
                                            GoTo IL_03F7
                                        End If
                                    ElseIf Operators.CompareString(text2, "25w14craftmine", False) <> 0 Then
                                        GoTo IL_03F7
                                    End If
                                ElseIf num <= 3741345573UI Then
                                    If num <> 2819146544UI Then
                                        If num <> 3653809737UI Then
                                            If num <> 3741345573UI Then
                                                GoTo IL_03F7
                                            End If
                                            If Operators.CompareString(text2, "20w14infinite", False) <> 0 Then
                                                GoTo IL_03F7
                                            End If
                                        ElseIf Operators.CompareString(text2, "20w14∞", False) <> 0 Then
                                            GoTo IL_03F7
                                        End If
                                        text = "愚人节版"
                                        jobject("id") = "20w14∞"
                                        jobject("type") = "special"
                                        jobject.Add("lore", ModMinecraft.GetMcFoolName(CStr(jobject("id"))))
                                        GoTo IL_0468
                                    End If
                                    If Operators.CompareString(text2, "15w14a", False) <> 0 Then
                                        GoTo IL_03F7
                                    End If
                                ElseIf num <> 3766348906UI Then
                                    If num <> 3830073160UI Then
                                        If num <> 4285466550UI Then
                                            GoTo IL_03F7
                                        End If
                                        If Operators.CompareString(text2, "24w14potato", False) <> 0 Then
                                            GoTo IL_03F7
                                        End If
                                    ElseIf Operators.CompareString(text2, "22w13oneblockatatime", False) <> 0 Then
                                        GoTo IL_03F7
                                    End If
                                ElseIf Operators.CompareString(text2, "3d shareware v1.34", False) <> 0 Then
                                    GoTo IL_03F7
                                End If
                                text = "愚人节版"
                                jobject("type") = "special"
                                jobject.Add("lore", ModMinecraft.GetMcFoolName(CStr(jobject("id"))))
                                GoTo IL_0468
                                IL_03F7:
                                Dim dateTime As DateTime = Extensions.Value(Of DateTime)(jobject("releaseTime")).ToUniversalTime().AddHours(2.0)
                                If dateTime.Month = 4 AndAlso dateTime.Day = 1 Then
                                    text = "愚人节版"
                                    jobject("type") = "special"
                                End If
                            End If
                        Else
                            text = "正式版"
                        End If
                        IL_0468:
                        dictionary(text).Add(jobject)
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of JToken)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                Try
                    For Each keyValuePair As KeyValuePair(Of String, List(Of JObject)) In dictionary.ToList()
                        Dim dictionary2 As Dictionary(Of String, List(Of JObject)) = dictionary
                        Dim key As String = keyValuePair.Key
                        Dim value As IEnumerable(Of JObject) = keyValuePair.Value
                        Dim func As Func(Of JObject, DateTime)
                        If PageDownloadInstall._Closure$__.$I35-0 IsNot Nothing Then
                            func = PageDownloadInstall._Closure$__.$I35-0
                        Else
                            Dim func2 As Func(Of JObject, DateTime) = Function(j As JObject) Extensions.Value(Of DateTime)(j("releaseTime"))
                            func = func2
                            PageDownloadInstall._Closure$__.$I35-0 = func2
                        End If
                        dictionary2(key) = value.OrderByDescending(func).ToList()
                    Next
                Finally
                    Dim enumerator2 As List(Of KeyValuePair(Of String, List(Of JObject))).Enumerator
                    CType(enumerator2, IDisposable).Dispose()
                End Try
                Me.PanMinecraft.Children.Clear()
                Dim myCard As OurCard = New OurCard() With { .Title = "最新版本", .Margin = New Thickness(0.0, 15.0, 0.0, 15.0) }
                Dim list As List(Of JObject) = New List(Of JObject)()
                Dim jobject2 As JObject = CType(dictionary("正式版")(0).DeepClone(), JObject)
                jobject2("lore") = "最新正式版，发布于 " + Extensions.Value(Of DateTime)(jobject2("releaseTime")).ToString("yyyy'/'MM'/'dd HH':'mm")
                list.Add(jobject2)
                If DateTime.Compare(Extensions.Value(Of DateTime)(dictionary("正式版")(0)("releaseTime")), Extensions.Value(Of DateTime)(dictionary("预览版")(0)("releaseTime"))) < 0 Then
                    Dim jobject3 As JObject = CType(dictionary("预览版")(0).DeepClone(), JObject)
                    jobject3("lore") = "最新预览版，发布于 " + Extensions.Value(Of DateTime)(jobject3("releaseTime")).ToString("yyyy'/'MM'/'dd HH':'mm")
                    list.Add(jobject3)
                End If
                Dim stackPanel As StackPanel = New StackPanel() With { .Margin = New Thickness(20.0, 40.0, 18.0, 0.0), .VerticalAlignment = VerticalAlignment.Top, .RenderTransform = New TranslateTransform(0.0, 0.0) }
                myCard.Children.Add(stackPanel)
                myCard.SwapControl = stackPanel
                myCard.SetContentBuilder(Of JObject)(list, Function(Version As JObject) ModDownloadLib.McDownloadListItem(Version, Sub(sender As Object, e As MouseButtonEventArgs)
                    Me.MinecraftSelected(CType(sender, OurListItem), e)
                End Sub, False))
                myCard.BuildContent()
                Me.PanMinecraft.Children.Insert(0, myCard)
                Try
                    For Each keyValuePair2 As KeyValuePair(Of String, List(Of JObject)) In dictionary
                        If keyValuePair2.Value.Any() Then
                            Dim myCard2 As OurCard = New OurCard() With { .Title = keyValuePair2.Key, .Margin = New Thickness(0.0, 0.0, 0.0, 15.0) }
                            Dim stackPanel2 As StackPanel = New StackPanel() With { .Margin = New Thickness(20.0, 40.0, 18.0, 0.0), .VerticalAlignment = VerticalAlignment.Top, .RenderTransform = New TranslateTransform(0.0, 0.0) }
                            myCard2.Children.Add(stackPanel2)
                            myCard2.SwapControl = stackPanel2
                            myCard2.SetContentBuilder(Of JObject)(keyValuePair2.Value, Function(Version As JObject) ModDownloadLib.McDownloadListItem(Version, Sub(sender As Object, e As MouseButtonEventArgs)
                                Me.MinecraftSelected(CType(sender, OurListItem), e)
                            End Sub, False))
                            myCard2.IsSwapped = True
                            Me.PanMinecraft.Children.Add(myCard2)
                        End If
                    Next
                Finally
                    Dim enumerator3 As Dictionary(Of String, List(Of JObject)).Enumerator
                    CType(enumerator3, IDisposable).Dispose()
                End Try
            Catch ex As Exception
                Logger.[Error](ex, "可视化安装版本列表出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\PageDownloadInstall.xaml.vb")
            End Try
        End Sub

        Private Sub TrySelectWaitingVersion()
            If PageDownloadInstall.VersionWaitingSelect IsNot Nothing Then
                Try
                    For Each jtoken As JToken In Me.MinecraftVersions
                        Dim jobject As JObject = CType(jtoken, JObject)
                        If Operators.CompareString(jobject("id").ToString(), PageDownloadInstall.VersionWaitingSelect, False) = 0 Then
                            Logger.Info(String.Format("自动选择 MC 版本：{0}", PageDownloadInstall.VersionWaitingSelect), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\PageDownloadInstall.xaml.vb")
                            Me.MinecraftSelected(ModDownloadLib.McDownloadListItem(jobject, Nothing, False).Init(), Nothing)
                            PageDownloadInstall.VersionWaitingSelect = Nothing
                            Exit For
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of JToken)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
            End If
        End Sub

        Public Function LoadOptiFineGetError() As String
            Dim text As String
            If Me.SelectedNeoForge IsNot Nothing Then
                text = "与 NeoForge 不兼容"
            ElseIf Me.SelectedForge IsNot Nothing AndAlso McUtils.CompareVersionGE(Me.VanillaName, "1.13") AndAlso McUtils.CompareVersionGE("1.14.3", Me.VanillaName) Then
                text = "与 Forge 不兼容"
            ElseIf Me.SelectedFabric IsNot Nothing AndAlso McUtils.CompareVersion(Me.VanillaName, "1.20.4") > 0 Then
                text = "与 Fabric 不兼容"
            ElseIf Me.GetLoaderError(Me.LoadOptiFine) IsNot Nothing Then
                text = Me.GetLoaderError(Me.LoadOptiFine)
            Else
                Dim flag As Boolean = False
                Dim flag2 As Boolean = False
                Try
                    For Each dlOptiFineListEntry As ModDownload.DlOptiFineListEntry In ModDownload.DlOptiFineListLoader.Output.Value
                        If dlOptiFineListEntry.DisplayName.StartsWithF(Me.VanillaName + " ", False) Then
                            flag = True
                            If Me.SelectedForge Is Nothing Then
                                Return Nothing
                            End If
                            If PageDownloadInstall.IsOptiFineSuitForForge(dlOptiFineListEntry, Me.SelectedForge) Then
                                Return Nothing
                            End If
                            If dlOptiFineListEntry.RequiredForgeVersion IsNot Nothing Then
                                flag2 = True
                            End If
                        End If
                    Next
                Finally
                    Dim enumerator As List(Of ModDownload.DlOptiFineListEntry).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                If Not flag Then
                    text = "无"
                ElseIf flag2 Then
                    text = "仅兼容特定版本的 Forge"
                Else
                    text = "与 Forge 不兼容"
                End If
            End If
            Return text
        End Function

        Public Shared Function IsOptiFineSuitForForge(OptiFine As ModDownload.DlOptiFineListEntry, Forge As ModDownload.DlForgeVersionEntry) As Boolean
            Dim flag As Boolean
            If Operators.CompareString(Forge.Inherit, OptiFine.Inherit, False) <> 0 Then
                flag = False
            ElseIf OptiFine.RequiredForgeVersion Is Nothing Then
                flag = False
            ElseIf String.IsNullOrWhiteSpace(OptiFine.RequiredForgeVersion) Then
                flag = True
            ElseIf OptiFine.RequiredForgeVersion.Contains(".") Then
                flag = McUtils.CompareVersion(Forge.Version.ToString(), OptiFine.RequiredForgeVersion) = 0
            Else
                flag = CDbl(Forge.Version.Revision) = Conversions.ToDouble(OptiFine.RequiredForgeVersion)
            End If
            Return flag
        End Function

        Private Sub CardOptiFine_PreviewSwap(sender As Object, e As ModBase.RouteEventArgs)
            If Me.LoadOptiFineGetError() IsNot Nothing Then
                e.Handled = True
            End If
        End Sub

        Private Sub OptiFine_Loaded()
            Try
                If ModDownload.DlOptiFineListLoader.State = ModBase.LoadState.Finished Then
                    Dim list As List(Of ModDownload.DlOptiFineListEntry) = New List(Of ModDownload.DlOptiFineListEntry)()
                    Try
                        For Each dlOptiFineListEntry As ModDownload.DlOptiFineListEntry In ModDownload.DlOptiFineListLoader.Output.Value
                            If(Me.SelectedForge Is Nothing OrElse PageDownloadInstall.IsOptiFineSuitForForge(dlOptiFineListEntry, Me.SelectedForge)) AndAlso dlOptiFineListEntry.DisplayName.StartsWithF(Me.VanillaName + " ", False) Then
                                list.Add(dlOptiFineListEntry)
                            End If
                        Next
                    Finally
                        Dim enumerator As List(Of ModDownload.DlOptiFineListEntry).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                    If list.Any() Then
                        Dim list2 As IList(Of ModDownload.DlOptiFineListEntry) = list
                        Dim func As Func(Of ModDownload.DlOptiFineListEntry, ModDownload.DlOptiFineListEntry, Boolean)
                        If PageDownloadInstall._Closure$__.$I41-0 IsNot Nothing Then
                            func = PageDownloadInstall._Closure$__.$I41-0
                        Else
                            Dim func2 As Func(Of ModDownload.DlOptiFineListEntry, ModDownload.DlOptiFineListEntry, Boolean) = Function(Left As ModDownload.DlOptiFineListEntry, Right As ModDownload.DlOptiFineListEntry) (Not Left.IsPreview AndAlso Right.IsPreview) OrElse ((Not Left.IsPreview OrElse Right.IsPreview) AndAlso McUtils.CompareVersionGE(Left.DisplayName, Right.DisplayName))
                            func = func2
                            PageDownloadInstall._Closure$__.$I41-0 = func2
                        End If
                        list = list2.SortByComparison(func)
                        Me.PanOptiFine.Children.Clear()
                        Try
                            For Each dlOptiFineListEntry2 As ModDownload.DlOptiFineListEntry In list
                                Me.PanOptiFine.Children.Add(ModDownloadLib.OptiFineDownloadListItem(dlOptiFineListEntry2, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                                    Me.OptiFine_Selected(CType(a0, OurListItem), a1)
                                End Sub, False))
                            Next
                        Finally
                            Dim enumerator2 As List(Of ModDownload.DlOptiFineListEntry).Enumerator
                            CType(enumerator2, IDisposable).Dispose()
                        End Try
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, "可视化 OptiFine 安装版本列表出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\PageDownloadInstall.xaml.vb")
            End Try
        End Sub

        Private Sub OptiFine_Selected(sender As OurListItem, e As EventArgs)
            Me.SelectedOptiFine = CType(sender.Tag, ModDownload.DlOptiFineListEntry)
            If Me.SelectedForge IsNot Nothing AndAlso Not PageDownloadInstall.IsOptiFineSuitForForge(Me.SelectedOptiFine, Me.SelectedForge) Then
                Me.SelectedForge = Nothing
            End If
            Me.OptiFabric_Loaded()
            Me.Forge_Loaded()
            Me.NeoForge_Loaded()
            Me.CardOptiFine.IsSwapped = True
            Me.ReloadSelected()
        End Sub

        Private Sub OptiFine_Clear(sender As Object, e As EventArgs)
            Me.SelectedOptiFine = Nothing
            Me.SelectedOptiFabric = Nothing
            Me.BtnOptiFabricClear.Visibility = Visibility.Collapsed
            Me.AutoSelectedOptiFabric = False
            Me.CardOptiFine.IsSwapped = True
            Me.Forge_Loaded()
            Me.NeoForge_Loaded()
            Me.ReloadSelected()
        End Sub

        Public Function LoadLiteLoaderGetError() As String
            Dim text As String
            If Me.GetLoaderError(Me.LoadLiteLoader) IsNot Nothing Then
                text = Me.GetLoaderError(Me.LoadLiteLoader)
            Else
                text = If(ModDownload.DlLiteLoaderListLoader.Output.Value.Any(Function(v As ModDownload.DlLiteLoaderListEntry) Operators.CompareString(v.Inherit, Me.VanillaName, False) = 0), Nothing, "无")
            End If
            Return text
        End Function

        Private Sub CardLiteLoader_PreviewSwap(sender As Object, e As ModBase.RouteEventArgs)
            If Me.LoadLiteLoaderGetError() IsNot Nothing Then
                e.Handled = True
            End If
        End Sub

        Private Sub LiteLoader_Loaded()
            Try
                If ModDownload.DlLiteLoaderListLoader.State = ModBase.LoadState.Finished Then
                    Dim list As List(Of ModDownload.DlLiteLoaderListEntry) = New List(Of ModDownload.DlLiteLoaderListEntry)()
                    Try
                        For Each dlLiteLoaderListEntry As ModDownload.DlLiteLoaderListEntry In ModDownload.DlLiteLoaderListLoader.Output.Value
                            If Operators.CompareString(dlLiteLoaderListEntry.Inherit, Me.VanillaName, False) = 0 Then
                                list.Add(dlLiteLoaderListEntry)
                            End If
                        Next
                    Finally
                        Dim enumerator As List(Of ModDownload.DlLiteLoaderListEntry).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                    If list.Any() Then
                        Me.PanLiteLoader.Children.Clear()
                        Try
                            For Each dlLiteLoaderListEntry2 As ModDownload.DlLiteLoaderListEntry In list
                                Me.PanLiteLoader.Children.Add(ModDownloadLib.LiteLoaderDownloadListItem(dlLiteLoaderListEntry2, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                                    Me.LiteLoader_Selected(CType(a0, OurListItem), a1)
                                End Sub, False))
                            Next
                        Finally
                            Dim enumerator2 As List(Of ModDownload.DlLiteLoaderListEntry).Enumerator
                            CType(enumerator2, IDisposable).Dispose()
                        End Try
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, "可视化 LiteLoader 安装版本列表出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\PageDownloadInstall.xaml.vb")
            End Try
        End Sub

        Private Sub LiteLoader_Selected(sender As OurListItem, e As EventArgs)
            Me.SelectedLiteLoader = CType(sender.Tag, ModDownload.DlLiteLoaderListEntry)
            Me.CardLiteLoader.IsSwapped = True
            Me.ReloadSelected()
        End Sub

        Private Sub LiteLoader_Clear(sender As Object, e As EventArgs)
            Me.SelectedLiteLoader = Nothing
            Me.CardLiteLoader.IsSwapped = True
            Me.ReloadSelected()
        End Sub

        Public Function LoadForgeGetError() As String
            Dim text As String
            If McUtils.CompareVersionGE("1.5.1", Me.VanillaName) AndAlso McUtils.CompareVersionGE(Me.VanillaName, "1.1") Then
                text = "无"
            ElseIf Me.GetLoaderError(Me.LoadForge) IsNot Nothing Then
                text = Me.GetLoaderError(Me.LoadForge)
            Else
                Dim loaderTask As ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)) = CType(Me.LoadForge.State, ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)))
                If Operators.CompareString(Me.VanillaName, loaderTask.Input, False) <> 0 Then
                    text = "获取中……"
                Else
                    Try
                        For Each dlForgeVersionEntry As ModDownload.DlForgeVersionEntry In loaderTask.Output
                            If Operators.CompareString(dlForgeVersionEntry.Category, "universal", False) <> 0 AndAlso Operators.CompareString(dlForgeVersionEntry.Category, "client", False) <> 0 Then
                                If Me.SelectedNeoForge IsNot Nothing Then
                                    Return "与 NeoForge 不兼容"
                                End If
                                If Me.SelectedFabric IsNot Nothing Then
                                    Return "与 Fabric 不兼容"
                                End If
                                If Me.SelectedOptiFine IsNot Nothing AndAlso McUtils.CompareVersionGE(Me.VanillaName, "1.13") AndAlso McUtils.CompareVersionGE("1.14.3", Me.VanillaName) Then
                                    Return "与 OptiFine 不兼容"
                                End If
                                If Me.SelectedOptiFine Is Nothing OrElse PageDownloadInstall.IsOptiFineSuitForForge(Me.SelectedOptiFine, dlForgeVersionEntry) Then
                                    Return Nothing
                                End If
                            End If
                        Next
                    Finally
                        Dim enumerator As List(Of ModDownload.DlForgeVersionEntry).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                    text = "与 OptiFine 不兼容"
                End If
            End If
            Return text
        End Function

        Private Sub CardForge_PreviewSwap(sender As Object, e As ModBase.RouteEventArgs)
            If Me.LoadForgeGetError() IsNot Nothing Then
                e.Handled = True
            End If
        End Sub

        Private Sub Forge_Loaded()
            Try
                If Me.LoadForge.State.IsLoader Then
                    Dim loaderTask As ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)) = CType(Me.LoadForge.State, ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)))
                    If Operators.CompareString(Me.VanillaName, loaderTask.Input, False) = 0 Then
                        If loaderTask.State = ModBase.LoadState.Finished Then
                            Dim list As List(Of ModDownload.DlForgeVersionEntry) = loaderTask.Output.ToList()
                            If loaderTask.Output.Any() Then
                                Me.PanForge.Children.Clear()
                                Dim enumerable As IEnumerable(Of ModDownload.DlForgeVersionEntry) = list.Where(Function(v As ModDownload.DlForgeVersionEntry) Operators.CompareString(v.Category, "universal", False) <> 0 AndAlso Operators.CompareString(v.Category, "client", False) <> 0 AndAlso (Me.SelectedOptiFine Is Nothing OrElse PageDownloadInstall.IsOptiFineSuitForForge(Me.SelectedOptiFine, v)))
                                Dim func As Func(Of ModDownload.DlForgeVersionEntry, ModDownload.DlForgeVersionEntry)
                                If PageDownloadInstall._Closure$__.$I51-1 IsNot Nothing Then
                                    func = PageDownloadInstall._Closure$__.$I51-1
                                Else
                                    Dim func2 As Func(Of ModDownload.DlForgeVersionEntry, ModDownload.DlForgeVersionEntry) = Function(v As ModDownload.DlForgeVersionEntry) v
                                    func = func2
                                    PageDownloadInstall._Closure$__.$I51-1 = func2
                                End If
                                list = enumerable.OrderByDescending(func).ToList()
                                ModDownloadLib.ForgeDownloadListItemPreload(Me.PanForge, list, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                                    Me.Forge_Selected(CType(a0, OurListItem), a1)
                                End Sub, False)
                                Try
                                    For Each dlForgeVersionEntry As ModDownload.DlForgeVersionEntry In list
                                        Me.PanForge.Children.Add(ModDownloadLib.ForgeDownloadListItem(dlForgeVersionEntry, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                                            Me.Forge_Selected(CType(a0, OurListItem), a1)
                                        End Sub, False))
                                    Next
                                Finally
                                    Dim enumerator As List(Of ModDownload.DlForgeVersionEntry).Enumerator
                                    CType(enumerator, IDisposable).Dispose()
                                End Try
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, "可视化 Forge 安装版本列表出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\PageDownloadInstall.xaml.vb")
            End Try
        End Sub

        Private Sub Forge_Selected(sender As OurListItem, e As EventArgs)
            Me.SelectedForge = CType(sender.Tag, ModDownload.DlForgeVersionEntry)
            Me.CardForge.IsSwapped = True
            If Me.SelectedOptiFine IsNot Nothing AndAlso Not PageDownloadInstall.IsOptiFineSuitForForge(Me.SelectedOptiFine, Me.SelectedForge) Then
                Me.SelectedOptiFine = Nothing
            End If
            Me.OptiFine_Loaded()
            Me.ReloadSelected()
        End Sub

        Private Sub Forge_Clear(sender As Object, e As EventArgs)
            Me.SelectedForge = Nothing
            Me.CardForge.IsSwapped = True
            Me.OptiFine_Loaded()
            Me.ReloadSelected()
        End Sub

        Public Function LoadNeoForgeGetError() As String
            Dim text As String
            If Me.SelectedOptiFine IsNot Nothing Then
                text = "与 OptiFine 不兼容"
            ElseIf Me.SelectedForge IsNot Nothing Then
                text = "与 Forge 不兼容"
            ElseIf Me.SelectedFabric IsNot Nothing Then
                text = "与 Fabric 不兼容"
            ElseIf Me.GetLoaderError(Me.LoadNeoForge) IsNot Nothing Then
                text = Me.GetLoaderError(Me.LoadNeoForge)
            Else
                text = If(ModDownload.DlNeoForgeListLoader.Output.Value.Any(Function(v As ModDownload.DlNeoForgeListEntry) Operators.CompareString(v.Inherit, Me.VanillaName, False) = 0), Nothing, "无")
            End If
            Return text
        End Function

        Private Sub CardNeoForge_PreviewSwap(sender As Object, e As ModBase.RouteEventArgs)
            If Me.LoadNeoForgeGetError() IsNot Nothing Then
                e.Handled = True
            End If
        End Sub

        Private Sub NeoForge_Loaded()
            Try
                If ModDownload.DlNeoForgeListLoader.State = ModBase.LoadState.Finished Then
                    Dim list As List(Of ModDownload.DlNeoForgeListEntry) = ModDownload.DlNeoForgeListLoader.Output.Value.Where(Function(v As ModDownload.DlNeoForgeListEntry) Operators.CompareString(v.Inherit, Me.VanillaName, False) = 0).ToList()
                    If list.Any() Then
                        Me.PanNeoForge.Children.Clear()
                        ModDownloadLib.NeoForgeDownloadListItemPreload(Me.PanNeoForge, list, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                            Me.NeoForge_Selected(CType(a0, OurListItem), a1)
                        End Sub, False)
                        Try
                            For Each dlNeoForgeListEntry As ModDownload.DlNeoForgeListEntry In list
                                Me.PanNeoForge.Children.Add(ModDownloadLib.NeoForgeDownloadListItem(dlNeoForgeListEntry, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                                    Me.NeoForge_Selected(CType(a0, OurListItem), a1)
                                End Sub, False))
                            Next
                        Finally
                            Dim enumerator As List(Of ModDownload.DlNeoForgeListEntry).Enumerator
                            CType(enumerator, IDisposable).Dispose()
                        End Try
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, "可视化 NeoForge 安装版本列表出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\PageDownloadInstall.xaml.vb")
            End Try
        End Sub

        Private Sub NeoForge_Selected(sender As OurListItem, e As EventArgs)
            Me.SelectedNeoForge = CType(sender.Tag, ModDownload.DlNeoForgeListEntry)
            Me.CardNeoForge.IsSwapped = True
            Me.OptiFine_Loaded()
            Me.ReloadSelected()
        End Sub

        Private Sub NeoForge_Clear(sender As Object, e As EventArgs)
            Me.SelectedNeoForge = Nothing
            Me.CardNeoForge.IsSwapped = True
            Me.OptiFine_Loaded()
            Me.ReloadSelected()
        End Sub

        Public Function LoadFabricGetError() As String
            Dim text As String
            If Me.SelectedOptiFine IsNot Nothing AndAlso McUtils.CompareVersionGE(Me.VanillaName, "1.20.5") Then
                text = "与 OptiFine 不兼容"
            ElseIf Me.GetLoaderError(Me.LoadFabric) IsNot Nothing Then
                text = Me.GetLoaderError(Me.LoadFabric)
            Else
                Try
                    Dim enumerator As IEnumerator(Of JToken) = ModDownload.DlFabricListLoader.Output.Value("game").GetEnumerator()
                    While enumerator.MoveNext()
                        If Operators.CompareString(CType(enumerator.Current, JObject)("version").ToString(), Me.VanillaName.Replace("∞", "infinite").Replace("Combat Test 7c", "1.16_combat-3"), False) = 0 Then
                            If Me.SelectedForge IsNot Nothing Then
                                Return "与 Forge 不兼容"
                            End If
                            If Me.SelectedNeoForge IsNot Nothing Then
                                Return "与 NeoForge 不兼容"
                            End If
                            Return Nothing
                        End If
                    End While
                Finally
                    Dim enumerator As IEnumerator(Of JToken)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                text = "无"
            End If
            Return text
        End Function

        Private Sub CardFabric_PreviewSwap(sender As Object, e As ModBase.RouteEventArgs)
            If Me.LoadFabricGetError() IsNot Nothing Then
                e.Handled = True
            End If
        End Sub

        Private Sub Fabric_Loaded()
            Try
                If ModDownload.DlFabricListLoader.State = ModBase.LoadState.Finished Then
                    Dim jarray As JArray = CType(ModDownload.DlFabricListLoader.Output.Value("loader"), JArray)
                    If jarray.Any() Then
                        Me.PanFabric.Children.Clear()
                        Me.CardFabric.SwapControl = Me.PanFabric
                        Me.CardFabric.SetContentBuilder(Of JObject)(jarray.OfType(), Function(Version As JObject) ModDownloadLib.FabricDownloadListItem(Version, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                            Me.Fabric_Selected(CType(a0, OurListItem), a1)
                        End Sub))
                        If Not Me.CardFabric.IsSwapped Then
                            Me.CardFabric.BuildContent()
                        End If
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, "可视化 Fabric 安装版本列表出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\PageDownloadInstall.xaml.vb")
            End Try
        End Sub

        Public Sub Fabric_Selected(sender As OurListItem, e As EventArgs)
            Me.SelectedFabric = NewLateBinding.LateIndexGet(sender.Tag, New Object() { "version" }, Nothing).ToString()
            Me.FabricApi_Loaded()
            Me.OptiFabric_Loaded()
            Me.CardFabric.IsSwapped = True
            Me.ReloadSelected()
        End Sub

        Private Sub Fabric_Clear(sender As Object, e As EventArgs)
            Me.SelectedFabric = Nothing
            Me.SelectedFabricApi = Nothing
            Me.BtnFabricApiClear.Visibility = Visibility.Collapsed
            Me.AutoSelectedFabricApi = False
            Me.SelectedOptiFabric = Nothing
            Me.BtnOptiFabricClear.Visibility = Visibility.Collapsed
            Me.AutoSelectedOptiFabric = False
            Me.CardFabric.IsSwapped = True
            Me.ReloadSelected()
        End Sub

        Public Shared Function IsFabricApiCompatible(FabricApi As ResourceVersion, MinecraftVersion As String) As Boolean
            Dim text As String = FabricApi.Display
            Dim flag As Boolean
            Try
                If text Is Nothing OrElse MinecraftVersion Is Nothing Then
                    flag = False
                Else
                    text = text.Lower()
                    Dim text2 As String = MinecraftVersion.Replace("∞", "infinite").Replace("Combat Test 7c", "1.16_combat-3").Lower()
                    If text.StartsWithF("[" + text2 + "]", False) Then
                        flag = True
                    ElseIf Not text.Contains("/") OrElse Not text.Contains("]") Then
                        flag = False
                    Else
                        Dim array As String() = text.BeforeFirst("]", False).TrimStart(New Char() { "["c }).Split("/", False)
                        For i As Integer = 0 To array.Length - 1
                            If Operators.CompareString(array(i), text2, False) = 0 Then
                                Return True
                            End If
                        Next
                        Dim list As List(Of String) = text.BeforeFirst("]", False).RegexSearch("[a-z/]+|[0-9/]+", RegexOptions.None).ToList()
                        Dim list2 As List(Of String) = text2.BeforeFirst("]", False).RegexSearch("[a-z/]+|[0-9/]+", RegexOptions.None).ToList()
                        Dim num As Integer = 0
                        While list.Count - 1 >= num OrElse list2.Count - 1 >= num
                            Dim text3 As String = If((list.Count - 1 < num), "-1", list(num))
                            Dim text4 As String = If((list2.Count - 1 < num), "-1", list2(num))
                            If Not text3.Contains("/") Then
                                If Operators.CompareString(text3, text4, False) <> 0 Then
                                    Return False
                                End If
                            ElseIf Not text3.Contains(text4) Then
                                Return False
                            End If
                            num += 1
                        End While
                        flag = True
                    End If
                End If
            Catch ex As Exception
                Logger.Warn(ex, String.Format("判断 Fabric API 版本适配性出错（{0}, {1}）", text, MinecraftVersion), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\PageDownloadInstall.xaml.vb")
                flag = False
            End Try
            Return flag
        End Function

        Public Function LoadFabricApiGetError() As String
            Dim text As String
            If Me.GetLoaderError(Me.LoadFabricApi) IsNot Nothing Then
                text = Me.GetLoaderError(Me.LoadFabricApi)
            ElseIf ModDownload.DlFabricApiLoader.Output Is Nothing Then
                text = If((Me.SelectedFabric Is Nothing), "需要安装 Fabric", "获取中……")
            ElseIf ModDownload.DlFabricApiLoader.Output.Any(Function(f As ResourceVersion) PageDownloadInstall.IsFabricApiCompatible(f, Me.VanillaName)) Then
                text = If((Me.SelectedFabric Is Nothing), "需要安装 Fabric", Nothing)
            Else
                text = "无"
            End If
            Return text
        End Function

        Private Sub CardFabricApi_PreviewSwap(sender As Object, e As ModBase.RouteEventArgs)
            If Me.LoadFabricApiGetError() IsNot Nothing Then
                e.Handled = True
            End If
        End Sub

        Private Sub FabricApi_Loaded()
            Try
                If ModDownload.DlFabricApiLoader.State = ModBase.LoadState.Finished Then
                    If Me.VanillaName IsNot Nothing AndAlso Me.SelectedFabric IsNot Nothing Then
                        Dim list As List(Of ResourceVersion) = New List(Of ResourceVersion)()
                        Try
                            For Each resourceVersion As ResourceVersion In ModDownload.DlFabricApiLoader.Output
                                If PageDownloadInstall.IsFabricApiCompatible(resourceVersion, Me.VanillaName) Then
                                    If Not resourceVersion.Display.StartsWithF("[", False) Then
                                        Logger.Warn(String.Format("已特判修改 Fabric API 显示名：{0}", resourceVersion.Display), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\PageDownloadInstall.xaml.vb")
                                        resourceVersion.Display = "[" + Me.VanillaName + "] " + resourceVersion.Display
                                    End If
                                    list.Add(resourceVersion)
                                End If
                            Next
                        Finally
                            Dim enumerator As List(Of ResourceVersion).Enumerator
                            CType(enumerator, IDisposable).Dispose()
                        End Try
                        If list.Any() Then
                            Dim enumerable As IEnumerable(Of ResourceVersion) = list
                            Dim func As Func(Of ResourceVersion, DateTime)
                            If PageDownloadInstall._Closure$__.$I68-0 IsNot Nothing Then
                                func = PageDownloadInstall._Closure$__.$I68-0
                            Else
                                Dim func2 As Func(Of ResourceVersion, DateTime) = Function(v As ResourceVersion) v.ReleaseDate
                                func = func2
                                PageDownloadInstall._Closure$__.$I68-0 = func2
                            End If
                            list = enumerable.OrderByDescending(func).ToList()
                            Me.PanFabricApi.Children.Clear()
                            Try
                                For Each resourceVersion2 As ResourceVersion In list
                                    Me.PanFabricApi.Children.Add(ModDownloadLib.FabricApiDownloadListItem(resourceVersion2, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                                        Me.FabricApi_Selected(CType(a0, OurListItem), a1)
                                    End Sub))
                                Next
                            Finally
                                Dim enumerator2 As List(Of ResourceVersion).Enumerator
                                CType(enumerator2, IDisposable).Dispose()
                            End Try
                            If Not Me.AutoSelectedFabricApi Then
                                Me.AutoSelectedFabricApi = True
                                Dim myListItem As OurListItem = CType(OurVirtualizingElement.TryInit(CType(Me.PanFabricApi.Children(0), FrameworkElement)), OurListItem)
                                Logger.Info(String.Format("已自动选择 Fabric API：{0}", myListItem.Title), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\PageDownloadInstall.xaml.vb")
                                Me.FabricApi_Selected(myListItem, Nothing)
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, "可视化 Fabric API 安装版本列表出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\PageDownloadInstall.xaml.vb")
            End Try
        End Sub

        Private Sub FabricApi_Selected(sender As OurListItem, e As EventArgs)
            Me.SelectedFabricApi = CType(sender.Tag, ResourceVersion)
            Me.CardFabricApi.IsSwapped = True
            Me.ReloadSelected()
        End Sub

        Private Sub FabricApi_Clear(sender As Object, e As EventArgs)
            Me.SelectedFabricApi = Nothing
            Me.CardFabricApi.IsSwapped = True
            Me.ReloadSelected()
        End Sub

        Public Shared Function IsOptiFabricCompatible(ModFile As ResourceVersion, MinecraftVersion As String) As Boolean
            Dim flag As Boolean
            Try
                If MinecraftVersion Is Nothing Then
                    flag = False
                Else
                    flag = ModFile.GameVersions.Contains(MinecraftVersion)
                End If
            Catch ex As Exception
                Logger.Warn(ex, String.Format("判断 OptiFabric 版本适配性出错（{0}）", MinecraftVersion), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\PageDownloadInstall.xaml.vb")
                flag = False
            End Try
            Return flag
        End Function

        Public Function LoadOptiFabricGetError() As String
            Dim text As String
            If Me.VanillaDrop >= 140 AndAlso Me.VanillaDrop <= 150 Then
                text = "不兼容老版本 Fabric，请手动下载 OptiFabric Origins"
            ElseIf Me.GetLoaderError(Me.LoadOptiFabric) IsNot Nothing Then
                text = Me.GetLoaderError(Me.LoadOptiFabric)
            ElseIf ModDownload.DlOptiFabricLoader.Output Is Nothing Then
                If Me.SelectedFabric Is Nothing AndAlso Me.SelectedOptiFine Is Nothing Then
                    text = "需要安装 OptiFine 与 Fabric"
                ElseIf Me.SelectedFabric Is Nothing Then
                    text = "需要安装 Fabric"
                ElseIf Me.SelectedOptiFine Is Nothing Then
                    text = "需要安装 OptiFine"
                Else
                    text = "获取中……"
                End If
            Else
                Try
                    Dim enumerator As List(Of ResourceVersion).Enumerator = ModDownload.DlOptiFabricLoader.Output.GetEnumerator()
                    While enumerator.MoveNext()
                        If PageDownloadInstall.IsOptiFabricCompatible(enumerator.Current, Me.VanillaName) Then
                            If Me.SelectedFabric Is Nothing AndAlso Me.SelectedOptiFine Is Nothing Then
                                Return "需要安装 OptiFine 与 Fabric"
                            End If
                            If Me.SelectedFabric Is Nothing Then
                                Return "需要安装 Fabric"
                            End If
                            If Me.SelectedOptiFine Is Nothing Then
                                Return "需要安装 OptiFine"
                            End If
                            Return Nothing
                        End If
                    End While
                Finally
                    Dim enumerator As List(Of ResourceVersion).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                text = "无"
            End If
            Return text
        End Function

        Private Sub CardOptiFabric_PreviewSwap(sender As Object, e As ModBase.RouteEventArgs)
            If Me.LoadOptiFabricGetError() IsNot Nothing Then
                e.Handled = True
            End If
        End Sub

        Private Sub OptiFabric_Loaded()
            Try
                If ModDownload.DlOptiFabricLoader.State = ModBase.LoadState.Finished Then
                    If Me.VanillaName IsNot Nothing AndAlso Me.SelectedFabric IsNot Nothing AndAlso Me.SelectedOptiFine IsNot Nothing Then
                        Dim list As List(Of ResourceVersion) = New List(Of ResourceVersion)()
                        Try
                            For Each resourceVersion As ResourceVersion In ModDownload.DlOptiFabricLoader.Output
                                If PageDownloadInstall.IsOptiFabricCompatible(resourceVersion, Me.VanillaName) Then
                                    list.Add(resourceVersion)
                                End If
                            Next
                        Finally
                            Dim enumerator As List(Of ResourceVersion).Enumerator
                            CType(enumerator, IDisposable).Dispose()
                        End Try
                        If list.Any() Then
                            Dim enumerable As IEnumerable(Of ResourceVersion) = list
                            Dim func As Func(Of ResourceVersion, DateTime)
                            If PageDownloadInstall._Closure$__.$I75-0 IsNot Nothing Then
                                func = PageDownloadInstall._Closure$__.$I75-0
                            Else
                                Dim func2 As Func(Of ResourceVersion, DateTime) = Function(v As ResourceVersion) v.ReleaseDate
                                func = func2
                                PageDownloadInstall._Closure$__.$I75-0 = func2
                            End If
                            list = enumerable.OrderByDescending(func).ToList()
                            Me.PanOptiFabric.Children.Clear()
                            Try
                                For Each resourceVersion2 As ResourceVersion In list
                                    Me.PanOptiFabric.Children.Add(ModDownloadLib.OptiFabricDownloadListItem(resourceVersion2, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                                        Me.OptiFabric_Selected(CType(a0, OurListItem), a1)
                                    End Sub))
                                Next
                            Finally
                                Dim enumerator2 As List(Of ResourceVersion).Enumerator
                                CType(enumerator2, IDisposable).Dispose()
                            End Try
                            If Not Me.AutoSelectedOptiFabric AndAlso (Me.VanillaDrop < 140 OrElse Me.VanillaDrop > 150) Then
                                Me.AutoSelectedOptiFabric = True
                                Dim myListItem As OurListItem = CType(OurVirtualizingElement.TryInit(CType(Me.PanOptiFabric.Children(0), FrameworkElement)), OurListItem)
                                Logger.Info(String.Format("已自动选择 OptiFabric：{0}", myListItem.Title), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\PageDownloadInstall.xaml.vb")
                                Me.OptiFabric_Selected(myListItem, Nothing)
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, "可视化 OptiFabric 安装版本列表出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\PageDownloadInstall.xaml.vb")
            End Try
        End Sub

        Private Sub OptiFabric_Selected(sender As OurListItem, e As EventArgs)
            Me.SelectedOptiFabric = CType(sender.Tag, ResourceVersion)
            Me.CardOptiFabric.IsSwapped = True
            Me.ReloadSelected()
        End Sub

        Private Sub OptiFabric_Clear(sender As Object, e As EventArgs)
            Me.SelectedOptiFabric = Nothing
            Me.CardOptiFabric.IsSwapped = True
            Me.ReloadSelected()
        End Sub

        Public Function GetSelectedComponents() As McComponents
            Dim vanillaName As String = Me.VanillaName
            Dim jobject As JObject = If((Me.SelectedOptiFine Is Nothing), Nothing, JObject.FromObject(Me.SelectedOptiFine))
            Dim selectedForge As ModDownload.DlForgeVersionEntry = Me.SelectedForge
            Dim text As String = If((selectedForge IsNot Nothing), selectedForge.VersionName, Nothing)
            Dim selectedNeoForge As ModDownload.DlNeoForgeListEntry = Me.SelectedNeoForge
            Dim text2 As String = If((selectedNeoForge IsNot Nothing), selectedNeoForge.VersionName, Nothing)
            Dim selectedFabric As String = Me.SelectedFabric
            Dim selectedFabricApi As ResourceVersion = Me.SelectedFabricApi
            Dim text3 As String = If((selectedFabricApi IsNot Nothing), selectedFabricApi.Version, Nothing)
            Dim text4 As String = text3
            If text3 Is Nothing Then
                Dim selectedFabricApi2 As ResourceVersion = Me.SelectedFabricApi
                Dim text5 As String = If((selectedFabricApi2 IsNot Nothing), selectedFabricApi2.Id, Nothing)
                text4 = text5
                If text5 Is Nothing Then
                    Dim selectedFabricApi3 As ResourceVersion = Me.SelectedFabricApi
                    text4 = If((selectedFabricApi3 IsNot Nothing), selectedFabricApi3.Display, Nothing)
                End If
            End If
            Dim selectedOptiFabric As ResourceVersion = Me.SelectedOptiFabric
            Dim text6 As String = If((selectedOptiFabric IsNot Nothing), selectedOptiFabric.Version, Nothing)
            Dim text7 As String = text6
            If text6 Is Nothing Then
                Dim selectedOptiFabric2 As ResourceVersion = Me.SelectedOptiFabric
                Dim text8 As String = If((selectedOptiFabric2 IsNot Nothing), selectedOptiFabric2.Id, Nothing)
                text7 = text8
                If text8 Is Nothing Then
                    Dim selectedOptiFabric3 As ResourceVersion = Me.SelectedOptiFabric
                    text7 = If((selectedOptiFabric3 IsNot Nothing), selectedOptiFabric3.Display, Nothing)
                End If
            End If
            Dim selectedLiteLoader As ModDownload.DlLiteLoaderListEntry = Me.SelectedLiteLoader
            Dim text9 As String
            If selectedLiteLoader Is Nothing Then
                text9 = Nothing
            Else
                Dim jsonToken As JToken = selectedLiteLoader.JsonToken
                text9 = If((jsonToken IsNot Nothing), jsonToken("version").ToString(), Nothing)
            End If
            Return New McComponents(vanillaName, jobject, text, text2, selectedFabric, text4, text7, text9)
        End Function

        Private Sub TextSelectName_KeyDown(sender As Object, e As KeyEventArgs)
            If e.Key = Key.[Return] AndAlso Me.IsInSelectPage AndAlso Me.BtnStart.IsEnabled Then
                Me.BtnStart_Click()
            End If
        End Sub

        Private Sub BtnStart_Click()
            Dim text As String = Me.TextSelectName.Text
            Dim mcInstallRequest As ModDownloadLib.McInstallRequest = New ModDownloadLib.McInstallRequest() With { .NewInstanceName = text, .InstanceFolder = String.Format("{0}versions\{1}\", ModMinecraft.McFolderSelected, text), .Components = Me.GetSelectedComponents(), .MinecraftJson = Me.VanillaData("url").ToString(), .ForgeEntry = Me.SelectedForge, .NeoForgeEntry = Me.SelectedNeoForge, .FabricApi = Me.SelectedFabricApi, .OptiFabric = Me.SelectedOptiFabric, .LiteLoaderEntry = Me.SelectedLiteLoader }
            Me.StartInstall(mcInstallRequest)
        End Sub

        Public Overridable Sub StartInstall(Request As ModDownloadLib.McInstallRequest)
            If((Me.SelectedForge Is Nothing AndAlso Me.SelectedNeoForge Is Nothing AndAlso Me.SelectedFabric Is Nothing) OrElse (Settings.[Get](Of Integer)("LaunchArgumentIndieV2", Nothing) <> 0 AndAlso Settings.[Get](Of Integer)("LaunchArgumentIndieV2", Nothing) <> 2) OrElse ModMain.OurMsgBox("你尚未开启版本隔离，多个 MC 版本会共用同一个 Mod 文件夹。" & vbCrLf & "因此，游戏可能会因为读取到与当前版本不符的 Mod 而崩溃。" & vbCrLf & "推荐先在 设置 → 启动选项 → 默认版本隔离 中开启版本隔离！", "版本隔离提示", "取消下载", "继续", "", False, True, False, Nothing, Nothing, Nothing) <> 1) AndAlso ModDownloadLib.McInstallStart(Request) Then
                Me.ExitSelectPage()
            End If
        End Sub

        Private Function GetLoaderError(Loader As OurLoading) As String
            Dim text As String
            If Loader Is Nothing Then
                text = "获取中……"
            ElseIf Not Loader.State.IsLoader Then
                text = "获取中……"
            Else
                Select Case Loader.State.LoadingState
                    Case OurLoading.OurLoadingState.Unloaded
                        Return "未知错误，状态为 Unloaded"
                    Case OurLoading.OurLoadingState.Run
                        Return "获取中……"
                    Case OurLoading.OurLoadingState.[Error]
                        Dim message As String = CType(Loader.State, ModLoader.LoaderBase).[Error].Message
                        Return If((Operators.CompareString(message, "无", False) = 0), "无", ("获取失败：" + message))
                End Select
                text = Nothing
            End If
            Return text
        End Function

        Friend Overridable Property PanAllBack As Grid

        Friend Overridable Property PanBack As OurScrollViewer

        Friend Overridable Property PanInner As Grid

        Friend Overridable Property PanMinecraft As StackPanel

        Friend Overridable Property PanSelect As StackPanel

        Friend Overridable Property CardSelectName As OurCard

        Friend Overridable Property BtnBack As OurIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnBack
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconButton)
                Dim clickEventHandler As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    Me.ExitSelectPage()
                End Sub
                Dim myIconButton As OurIconButton = Me._BtnBack
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnBack = value
                myIconButton = Me._BtnBack
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ImgLogo As OurImage

        Friend Overridable Property TextSelectName As OurTextBox
            <CompilerGenerated()>
            Get
                Return Me._TextSelectName
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurTextBox)
                Dim textChangedEventHandler As TextChangedEventHandler = AddressOf Me.TextSelectName_TextChanged
                Dim validateChangedEventHandler As OurTextBox.ValidateChangedEventHandler = AddressOf Me.TextSelectName_ValidateChanged
                Dim keyEventHandler As KeyEventHandler = AddressOf Me.TextSelectName_KeyDown
                Dim myTextBox As OurTextBox = Me._TextSelectName
                If myTextBox IsNot Nothing Then
                    RemoveHandler myTextBox.TextChanged, textChangedEventHandler
                    RemoveHandler OurTextBox.ValidateChanged, validateChangedEventHandler
                    RemoveHandler myTextBox.KeyDown, keyEventHandler
                End If
                Me._TextSelectName = value
                myTextBox = Me._TextSelectName
                If myTextBox IsNot Nothing Then
                    AddHandler myTextBox.TextChanged, textChangedEventHandler
                    AddHandler OurTextBox.ValidateChanged, validateChangedEventHandler
                    AddHandler myTextBox.KeyDown, keyEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property LabSelectName As TextBlock

        Friend Overridable Property BtnOriginalNameRestore As OurIconButton

        Friend Overridable Property HintSelectName As OurHint

        Friend Overridable Property HintFabricAPI As OurHint

        Friend Overridable Property HintOptiFabric As OurHint

        Friend Overridable Property HintOptiFabricOld As OurHint

        Friend Overridable Property HintModOptiFine As OurHint

        Friend Overridable Property CardMinecraft As OurCard

        Friend Overridable Property PanMinecraftInfo As Grid

        Friend Overridable Property ImgMinecraft As OurImage

        Friend Overridable Property LabMinecraftVersion As TextBlock

        Friend Overridable Property PanMinecraftCompare As Grid

        Friend Overridable Property ImgMinecraftOld As OurImage

        Friend Overridable Property LabMinecraftOld As TextBlock

        Friend Overridable Property ImgMinecraftNew As OurImage

        Friend Overridable Property LabMinecraftNew As TextBlock

        Friend Overridable Property BtnMinecraftRestore As OurIconButton

        Friend Overridable Property CardForge As OurCard
            <CompilerGenerated()>
            Get
                Return Me._CardForge
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurCard)
                Dim swapEventHandler As OurCard.SwapEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.ReloadSelected()
                End Sub
                Dim previewSwapEventHandler As OurCard.PreviewSwapEventHandler = AddressOf Me.CardForge_PreviewSwap
                Dim myCard As OurCard = Me._CardForge
                If myCard IsNot Nothing Then
                    RemoveHandler myCard.Swap, swapEventHandler
                    RemoveHandler myCard.PreviewSwap, previewSwapEventHandler
                End If
                Me._CardForge = value
                myCard = Me._CardForge
                If myCard IsNot Nothing Then
                    AddHandler myCard.Swap, swapEventHandler
                    AddHandler myCard.PreviewSwap, previewSwapEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanForge As StackPanel

        Friend Overridable Property PanForgeInfo As Grid

        Friend Overridable Property ImgForge As OurImage

        Friend Overridable Property LabForge As TextBlock

        Friend Overridable Property PanForgeCompare As Grid

        Friend Overridable Property LabForgeOld As TextBlock

        Friend Overridable Property BtnForgeClear As OurIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnForgeClear
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconButton)
                Dim clickEventHandler As OurIconButton.ClickEventHandler = AddressOf Me.Forge_Clear
                Dim myIconButton As OurIconButton = Me._BtnForgeClear
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnForgeClear = value
                myIconButton = Me._BtnForgeClear
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnForgeRestore As OurIconButton

        Friend Overridable Property CardNeoForge As OurCard
            <CompilerGenerated()>
            Get
                Return Me._CardNeoForge
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurCard)
                Dim swapEventHandler As OurCard.SwapEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.ReloadSelected()
                End Sub
                Dim previewSwapEventHandler As OurCard.PreviewSwapEventHandler = AddressOf Me.CardNeoForge_PreviewSwap
                Dim myCard As OurCard = Me._CardNeoForge
                If myCard IsNot Nothing Then
                    RemoveHandler myCard.Swap, swapEventHandler
                    RemoveHandler myCard.PreviewSwap, previewSwapEventHandler
                End If
                Me._CardNeoForge = value
                myCard = Me._CardNeoForge
                If myCard IsNot Nothing Then
                    AddHandler myCard.Swap, swapEventHandler
                    AddHandler myCard.PreviewSwap, previewSwapEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanNeoForge As StackPanel

        Friend Overridable Property PanNeoForgeInfo As Grid

        Friend Overridable Property ImgNeoForge As OurImage

        Friend Overridable Property LabNeoForge As TextBlock

        Friend Overridable Property PanNeoForgeCompare As Grid

        Friend Overridable Property LabNeoForgeOld As TextBlock

        Friend Overridable Property BtnNeoForgeClear As OurIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnNeoForgeClear
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconButton)
                Dim clickEventHandler As OurIconButton.ClickEventHandler = AddressOf Me.NeoForge_Clear
                Dim myIconButton As OurIconButton = Me._BtnNeoForgeClear
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnNeoForgeClear = value
                myIconButton = Me._BtnNeoForgeClear
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnNeoForgeRestore As OurIconButton

        Friend Overridable Property CardFabric As OurCard
            <CompilerGenerated()>
            Get
                Return Me._CardFabric
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurCard)
                Dim swapEventHandler As OurCard.SwapEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.ReloadSelected()
                End Sub
                Dim previewSwapEventHandler As OurCard.PreviewSwapEventHandler = AddressOf Me.CardFabric_PreviewSwap
                Dim myCard As OurCard = Me._CardFabric
                If myCard IsNot Nothing Then
                    RemoveHandler myCard.Swap, swapEventHandler
                    RemoveHandler myCard.PreviewSwap, previewSwapEventHandler
                End If
                Me._CardFabric = value
                myCard = Me._CardFabric
                If myCard IsNot Nothing Then
                    AddHandler myCard.Swap, swapEventHandler
                    AddHandler myCard.PreviewSwap, previewSwapEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanFabric As StackPanel

        Friend Overridable Property PanFabricInfo As Grid

        Friend Overridable Property ImgFabric As OurImage

        Friend Overridable Property LabFabric As TextBlock

        Friend Overridable Property PanFabricCompare As Grid

        Friend Overridable Property LabFabricOld As TextBlock

        Friend Overridable Property BtnFabricClear As OurIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnFabricClear
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconButton)
                Dim clickEventHandler As OurIconButton.ClickEventHandler = AddressOf Me.Fabric_Clear
                Dim myIconButton As OurIconButton = Me._BtnFabricClear
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnFabricClear = value
                myIconButton = Me._BtnFabricClear
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnFabricRestore As OurIconButton

        Friend Overridable Property CardFabricApi As OurCard
            <CompilerGenerated()>
            Get
                Return Me._CardFabricApi
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurCard)
                Dim swapEventHandler As OurCard.SwapEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.ReloadSelected()
                End Sub
                Dim previewSwapEventHandler As OurCard.PreviewSwapEventHandler = AddressOf Me.CardFabricApi_PreviewSwap
                Dim myCard As OurCard = Me._CardFabricApi
                If myCard IsNot Nothing Then
                    RemoveHandler myCard.Swap, swapEventHandler
                    RemoveHandler myCard.PreviewSwap, previewSwapEventHandler
                End If
                Me._CardFabricApi = value
                myCard = Me._CardFabricApi
                If myCard IsNot Nothing Then
                    AddHandler myCard.Swap, swapEventHandler
                    AddHandler myCard.PreviewSwap, previewSwapEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanFabricApi As StackPanel

        Friend Overridable Property PanFabricApiInfo As Grid

        Friend Overridable Property ImgFabricApi As OurImage

        Friend Overridable Property LabFabricApi As TextBlock

        Friend Overridable Property PanFabricApiCompare As Grid

        Friend Overridable Property LabFabricApiOld As TextBlock

        Friend Overridable Property BtnFabricApiClear As OurIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnFabricApiClear
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconButton)
                Dim clickEventHandler As OurIconButton.ClickEventHandler = AddressOf Me.FabricApi_Clear
                Dim myIconButton As OurIconButton = Me._BtnFabricApiClear
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnFabricApiClear = value
                myIconButton = Me._BtnFabricApiClear
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnFabricApiRestore As OurIconButton

        Friend Overridable Property CardOptiFine As OurCard
            <CompilerGenerated()>
            Get
                Return Me._CardOptiFine
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurCard)
                Dim swapEventHandler As OurCard.SwapEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.ReloadSelected()
                End Sub
                Dim previewSwapEventHandler As OurCard.PreviewSwapEventHandler = AddressOf Me.CardOptiFine_PreviewSwap
                Dim myCard As OurCard = Me._CardOptiFine
                If myCard IsNot Nothing Then
                    RemoveHandler myCard.Swap, swapEventHandler
                    RemoveHandler myCard.PreviewSwap, previewSwapEventHandler
                End If
                Me._CardOptiFine = value
                myCard = Me._CardOptiFine
                If myCard IsNot Nothing Then
                    AddHandler myCard.Swap, swapEventHandler
                    AddHandler myCard.PreviewSwap, previewSwapEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanOptiFine As StackPanel

        Friend Overridable Property PanOptiFineInfo As Grid

        Friend Overridable Property ImgOptiFine As OurImage

        Friend Overridable Property LabOptiFine As TextBlock

        Friend Overridable Property PanOptiFineCompare As Grid

        Friend Overridable Property LabOptiFineOld As TextBlock

        Friend Overridable Property BtnOptiFineClear As OurIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnOptiFineClear
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconButton)
                Dim clickEventHandler As OurIconButton.ClickEventHandler = AddressOf Me.OptiFine_Clear
                Dim myIconButton As OurIconButton = Me._BtnOptiFineClear
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnOptiFineClear = value
                myIconButton = Me._BtnOptiFineClear
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnOptiFineRestore As OurIconButton

        Friend Overridable Property CardOptiFabric As OurCard
            <CompilerGenerated()>
            Get
                Return Me._CardOptiFabric
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurCard)
                Dim swapEventHandler As OurCard.SwapEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.ReloadSelected()
                End Sub
                Dim previewSwapEventHandler As OurCard.PreviewSwapEventHandler = AddressOf Me.CardOptiFabric_PreviewSwap
                Dim myCard As OurCard = Me._CardOptiFabric
                If myCard IsNot Nothing Then
                    RemoveHandler myCard.Swap, swapEventHandler
                    RemoveHandler myCard.PreviewSwap, previewSwapEventHandler
                End If
                Me._CardOptiFabric = value
                myCard = Me._CardOptiFabric
                If myCard IsNot Nothing Then
                    AddHandler myCard.Swap, swapEventHandler
                    AddHandler myCard.PreviewSwap, previewSwapEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanOptiFabric As StackPanel

        Friend Overridable Property PanOptiFabricInfo As Grid

        Friend Overridable Property ImgOptiFabric As OurImage

        Friend Overridable Property LabOptiFabric As TextBlock

        Friend Overridable Property PanOptiFabricCompare As Grid

        Friend Overridable Property LabOptiFabricOld As TextBlock

        Friend Overridable Property BtnOptiFabricClear As OurIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnOptiFabricClear
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconButton)
                Dim clickEventHandler As OurIconButton.ClickEventHandler = AddressOf Me.OptiFabric_Clear
                Dim myIconButton As OurIconButton = Me._BtnOptiFabricClear
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnOptiFabricClear = value
                myIconButton = Me._BtnOptiFabricClear
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnOptiFabricRestore As OurIconButton

        Friend Overridable Property CardLiteLoader As OurCard
            <CompilerGenerated()>
            Get
                Return Me._CardLiteLoader
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurCard)
                Dim swapEventHandler As OurCard.SwapEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.ReloadSelected()
                End Sub
                Dim previewSwapEventHandler As OurCard.PreviewSwapEventHandler = AddressOf Me.CardLiteLoader_PreviewSwap
                Dim myCard As OurCard = Me._CardLiteLoader
                If myCard IsNot Nothing Then
                    RemoveHandler myCard.Swap, swapEventHandler
                    RemoveHandler myCard.PreviewSwap, previewSwapEventHandler
                End If
                Me._CardLiteLoader = value
                myCard = Me._CardLiteLoader
                If myCard IsNot Nothing Then
                    AddHandler myCard.Swap, swapEventHandler
                    AddHandler myCard.PreviewSwap, previewSwapEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanLiteLoader As StackPanel

        Friend Overridable Property PanLiteLoaderInfo As Grid

        Friend Overridable Property ImgLiteLoader As OurImage

        Friend Overridable Property LabLiteLoader As TextBlock

        Friend Overridable Property PanLiteLoaderCompare As Grid

        Friend Overridable Property LabLiteLoaderOld As TextBlock

        Friend Overridable Property BtnLiteLoaderClear As OurIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnLiteLoaderClear
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconButton)
                Dim clickEventHandler As OurIconButton.ClickEventHandler = AddressOf Me.LiteLoader_Clear
                Dim myIconButton As OurIconButton = Me._BtnLiteLoaderClear
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnLiteLoaderClear = value
                myIconButton = Me._BtnLiteLoaderClear
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnLiteLoaderRestore As OurIconButton

        Friend Overridable Property BtnStart As OurExtraTextButton
            <CompilerGenerated()>
            Get
                Return Me._BtnStart
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurExtraTextButton)
                Dim clickEventHandler As OurExtraTextButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.BtnStart_Click()
                End Sub
                Dim myExtraTextButton As OurExtraTextButton = Me._BtnStart
                If myExtraTextButton IsNot Nothing Then
                    RemoveHandler myExtraTextButton.Click, clickEventHandler
                End If
                Me._BtnStart = value
                myExtraTextButton = Me._BtnStart
                If myExtraTextButton IsNot Nothing Then
                    AddHandler myExtraTextButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanLoad As OurCard

        Friend Overridable Property LoadMinecraft As OurLoading

        Friend Overridable Property LoadOptiFine As OurLoading
            <CompilerGenerated()>
            Get
                Return Me._LoadOptiFine
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurLoading)
                Dim stateChangedEventHandler As OurLoading.StateChangedEventHandler = Sub(a0 As Object, a1 As OurLoading.OurLoadingState, a2 As OurLoading.OurLoadingState)
                    Me.ReloadSelected()
                End Sub
                Dim stateChangedEventHandler2 As OurLoading.StateChangedEventHandler = Sub(a0 As Object, a1 As OurLoading.OurLoadingState, a2 As OurLoading.OurLoadingState)
                    Me.OptiFine_Loaded()
                End Sub
                Dim myLoading As OurLoading = Me._LoadOptiFine
                If myLoading IsNot Nothing Then
                    RemoveHandler myLoading.StateChanged, stateChangedEventHandler
                    RemoveHandler myLoading.StateChanged, stateChangedEventHandler2
                End If
                Me._LoadOptiFine = value
                myLoading = Me._LoadOptiFine
                If myLoading IsNot Nothing Then
                    AddHandler myLoading.StateChanged, stateChangedEventHandler
                    AddHandler myLoading.StateChanged, stateChangedEventHandler2
                End If
            End Set
        End Property

        Friend Overridable Property LoadForge As OurLoading
            <CompilerGenerated()>
            Get
                Return Me._LoadForge
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurLoading)
                Dim stateChangedEventHandler As OurLoading.StateChangedEventHandler = Sub(a0 As Object, a1 As OurLoading.OurLoadingState, a2 As OurLoading.OurLoadingState)
                    Me.ReloadSelected()
                End Sub
                Dim stateChangedEventHandler2 As OurLoading.StateChangedEventHandler = Sub(a0 As Object, a1 As OurLoading.OurLoadingState, a2 As OurLoading.OurLoadingState)
                    Me.Forge_Loaded()
                End Sub
                Dim myLoading As OurLoading = Me._LoadForge
                If myLoading IsNot Nothing Then
                    RemoveHandler myLoading.StateChanged, stateChangedEventHandler
                    RemoveHandler myLoading.StateChanged, stateChangedEventHandler2
                End If
                Me._LoadForge = value
                myLoading = Me._LoadForge
                If myLoading IsNot Nothing Then
                    AddHandler myLoading.StateChanged, stateChangedEventHandler
                    AddHandler myLoading.StateChanged, stateChangedEventHandler2
                End If
            End Set
        End Property

        Friend Overridable Property LoadNeoForge As OurLoading
            <CompilerGenerated()>
            Get
                Return Me._LoadNeoForge
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurLoading)
                Dim stateChangedEventHandler As OurLoading.StateChangedEventHandler = Sub(a0 As Object, a1 As OurLoading.OurLoadingState, a2 As OurLoading.OurLoadingState)
                    Me.ReloadSelected()
                End Sub
                Dim stateChangedEventHandler2 As OurLoading.StateChangedEventHandler = Sub(a0 As Object, a1 As OurLoading.OurLoadingState, a2 As OurLoading.OurLoadingState)
                    Me.NeoForge_Loaded()
                End Sub
                Dim myLoading As OurLoading = Me._LoadNeoForge
                If myLoading IsNot Nothing Then
                    RemoveHandler myLoading.StateChanged, stateChangedEventHandler
                    RemoveHandler myLoading.StateChanged, stateChangedEventHandler2
                End If
                Me._LoadNeoForge = value
                myLoading = Me._LoadNeoForge
                If myLoading IsNot Nothing Then
                    AddHandler myLoading.StateChanged, stateChangedEventHandler
                    AddHandler myLoading.StateChanged, stateChangedEventHandler2
                End If
            End Set
        End Property

        Friend Overridable Property LoadLiteLoader As OurLoading
            <CompilerGenerated()>
            Get
                Return Me._LoadLiteLoader
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurLoading)
                Dim stateChangedEventHandler As OurLoading.StateChangedEventHandler = Sub(a0 As Object, a1 As OurLoading.OurLoadingState, a2 As OurLoading.OurLoadingState)
                    Me.ReloadSelected()
                End Sub
                Dim stateChangedEventHandler2 As OurLoading.StateChangedEventHandler = Sub(a0 As Object, a1 As OurLoading.OurLoadingState, a2 As OurLoading.OurLoadingState)
                    Me.LiteLoader_Loaded()
                End Sub
                Dim myLoading As OurLoading = Me._LoadLiteLoader
                If myLoading IsNot Nothing Then
                    RemoveHandler myLoading.StateChanged, stateChangedEventHandler
                    RemoveHandler myLoading.StateChanged, stateChangedEventHandler2
                End If
                Me._LoadLiteLoader = value
                myLoading = Me._LoadLiteLoader
                If myLoading IsNot Nothing Then
                    AddHandler myLoading.StateChanged, stateChangedEventHandler
                    AddHandler myLoading.StateChanged, stateChangedEventHandler2
                End If
            End Set
        End Property

        Friend Overridable Property LoadFabric As OurLoading
            <CompilerGenerated()>
            Get
                Return Me._LoadFabric
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurLoading)
                Dim stateChangedEventHandler As OurLoading.StateChangedEventHandler = Sub(a0 As Object, a1 As OurLoading.OurLoadingState, a2 As OurLoading.OurLoadingState)
                    Me.ReloadSelected()
                End Sub
                Dim stateChangedEventHandler2 As OurLoading.StateChangedEventHandler = Sub(a0 As Object, a1 As OurLoading.OurLoadingState, a2 As OurLoading.OurLoadingState)
                    Me.Fabric_Loaded()
                End Sub
                Dim myLoading As OurLoading = Me._LoadFabric
                If myLoading IsNot Nothing Then
                    RemoveHandler myLoading.StateChanged, stateChangedEventHandler
                    RemoveHandler myLoading.StateChanged, stateChangedEventHandler2
                End If
                Me._LoadFabric = value
                myLoading = Me._LoadFabric
                If myLoading IsNot Nothing Then
                    AddHandler myLoading.StateChanged, stateChangedEventHandler
                    AddHandler myLoading.StateChanged, stateChangedEventHandler2
                End If
            End Set
        End Property

        Friend Overridable Property LoadFabricApi As OurLoading
            <CompilerGenerated()>
            Get
                Return Me._LoadFabricApi
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurLoading)
                Dim stateChangedEventHandler As OurLoading.StateChangedEventHandler = Sub(a0 As Object, a1 As OurLoading.OurLoadingState, a2 As OurLoading.OurLoadingState)
                    Me.ReloadSelected()
                End Sub
                Dim stateChangedEventHandler2 As OurLoading.StateChangedEventHandler = Sub(a0 As Object, a1 As OurLoading.OurLoadingState, a2 As OurLoading.OurLoadingState)
                    Me.FabricApi_Loaded()
                End Sub
                Dim myLoading As OurLoading = Me._LoadFabricApi
                If myLoading IsNot Nothing Then
                    RemoveHandler myLoading.StateChanged, stateChangedEventHandler
                    RemoveHandler myLoading.StateChanged, stateChangedEventHandler2
                End If
                Me._LoadFabricApi = value
                myLoading = Me._LoadFabricApi
                If myLoading IsNot Nothing Then
                    AddHandler myLoading.StateChanged, stateChangedEventHandler
                    AddHandler myLoading.StateChanged, stateChangedEventHandler2
                End If
            End Set
        End Property

        Friend Overridable Property LoadOptiFabric As OurLoading
            <CompilerGenerated()>
            Get
                Return Me._LoadOptiFabric
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurLoading)
                Dim stateChangedEventHandler As OurLoading.StateChangedEventHandler = Sub(a0 As Object, a1 As OurLoading.OurLoadingState, a2 As OurLoading.OurLoadingState)
                    Me.ReloadSelected()
                End Sub
                Dim stateChangedEventHandler2 As OurLoading.StateChangedEventHandler = Sub(a0 As Object, a1 As OurLoading.OurLoadingState, a2 As OurLoading.OurLoadingState)
                    Me.OptiFabric_Loaded()
                End Sub
                Dim myLoading As OurLoading = Me._LoadOptiFabric
                If myLoading IsNot Nothing Then
                    RemoveHandler myLoading.StateChanged, stateChangedEventHandler
                    RemoveHandler myLoading.StateChanged, stateChangedEventHandler2
                End If
                Me._LoadOptiFabric = value
                myLoading = Me._LoadOptiFabric
                If myLoading IsNot Nothing Then
                    AddHandler myLoading.StateChanged, stateChangedEventHandler
                    AddHandler myLoading.StateChanged, stateChangedEventHandler2
                End If
            End Set
        End Property

        Private IsLoad As Boolean

        Public IsInSelectPage As Boolean

        Public VanillaName As String

        Public VanillaData As JObject

        Public VanillaIcon As String

        Public SelectedOptiFine As ModDownload.DlOptiFineListEntry

        Public SelectedLiteLoader As ModDownload.DlLiteLoaderListEntry

        Public SelectedForge As ModDownload.DlForgeVersionEntry

        Public SelectedNeoForge As ModDownload.DlNeoForgeListEntry

        Public SelectedFabric As String

        Public SelectedFabricApi As ResourceVersion

        Public SelectedOptiFabric As ResourceVersion

        Public IsSelectNameEdited As Boolean

        Public MinecraftVersions As JArray

        Public Shared VersionWaitingSelect As String = Nothing

        Public AutoSelectedFabricApi As Boolean

        Public AutoSelectedOptiFabric As Boolean

        Private $STATIC$ReloadSelected$2001$Ongoing As Boolean

        Private $STATIC$ReloadSelected$2001$Ongoing$Init As StaticLocalInitFlag
    End Class
End Namespace
