Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class Settings
        Public Shared Sub [Set](Key As String, Value As Object, Optional Instance As ModMinecraft.McInstance = Nothing)
            Dim setting As Settings.Setting = Nothing
            If Not Settings.Entries.TryGetValue(Key, setting) Then
                Throw New KeyNotFoundException("未找到设置项：" + Key)
            End If
            Try
                Value = RuntimeHelpers.GetObjectValue(Conversion.CTypeDynamic(RuntimeHelpers.GetObjectValue(Value), setting.Type))
                If setting.GetCache(Instance) Is Nothing Then
                    Settings.[Get](Key, Instance)
                End If
                If Not Operators.ConditionalCompareObjectEqual(setting.GetCache(Instance), Value, False) Then
                    setting.[Set](RuntimeHelpers.GetObjectValue(Value), Instance)
                    setting.Save(Instance)
                    If setting.OnChanged IsNot Nothing Then
                        setting.OnChanged(RuntimeHelpers.GetObjectValue(Value))
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, String.Format("设置设置项时出错（{0}, {1}）", Key, RuntimeHelpers.GetObjectValue(Value)), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\Settings.vb")
            End Try
        End Sub

        Public Shared Sub SetSafe(Key As String, Value As Object, Optional Instance As ModMinecraft.McInstance = Nothing)
            Dim setting As Settings.Setting = Nothing
            If Not Settings.Entries.TryGetValue(Key, setting) Then
                Throw New KeyNotFoundException("未找到设置项：" + Key)
            End If
            If setting.Encrypted Then
                Throw New InvalidOperationException("禁止写入加密设置项：" + Key)
            End If
            Settings.[Set](Key, RuntimeHelpers.GetObjectValue(Value), Instance)
        End Sub

        Public Shared Function [Get](Key As String, Optional Instance As ModMinecraft.McInstance = Nothing) As Object
            Dim setting As Settings.Setting = Nothing
            If Not Settings.Entries.TryGetValue(Key, setting) Then
                Throw New KeyNotFoundException("未找到设置项：" + Key)
            End If
            Dim objectValue As Object = RuntimeHelpers.GetObjectValue(setting.GetCache(Instance))
            Dim obj As Object
            If objectValue IsNot Nothing Then
                obj = objectValue
            Else
                If Settings.$STATIC$Get$021CE1287E8$ForcedSettings$Init Is Nothing Then
                    Interlocked.CompareExchange(Of StaticLocalInitFlag)(Settings.$STATIC$Get$021CE1287E8$ForcedSettings$Init, New StaticLocalInitFlag(), Nothing)
                End If
                Dim flag As Boolean = False
                Try
                    Monitor.Enter(Settings.$STATIC$Get$021CE1287E8$ForcedSettings$Init, flag)
                    If Settings.$STATIC$Get$021CE1287E8$ForcedSettings$Init.State = 0S Then
                        Settings.$STATIC$Get$021CE1287E8$ForcedSettings$Init.State = 2S
                        Settings.$STATIC$Get$021CE1287E8$ForcedSettings = New Dictionary(Of String, Object)() From { { "UiHiddenPageLink", True }, { "UiHiddenSetupLink", True } }
                    ElseIf Settings.$STATIC$Get$021CE1287E8$ForcedSettings$Init.State = 2S Then
                        Throw New IncompleteInitialization()
                    End If
                Finally
                    Settings.$STATIC$Get$021CE1287E8$ForcedSettings$Init.State = 1S
                    If flag Then
                        Monitor.[Exit](Settings.$STATIC$Get$021CE1287E8$ForcedSettings$Init)
                    End If
                End Try
                Dim obj2 As Object = Nothing
                If Settings.$STATIC$Get$021CE1287E8$ForcedSettings.TryGetValue(Key, obj2) Then
                    setting.[Set](RuntimeHelpers.GetObjectValue(obj2), Instance)
                    obj = obj2
                Else
                    Try
                        Dim text As String = Nothing
                        Dim text2 As String = Conversions.ToString(If(setting.Encrypted, Settings.DesEncrypt(Conversions.ToString(setting.DefaultValue), "PCL" + ModBase.Identify), setting.DefaultValue))
                        Select Case setting.Source
                            Case Settings.Sources.Normal
                                text = ModBase.ReadIni("Setup", Key, text2)
                            Case Settings.Sources.Registry
                                text = Conversions.ToString(RegistryUtils.TryRead(String.Format("HKEY_CURRENT_USER\Software\{0}", "PCL"), Key, text2))
                            Case Settings.Sources.Instance
                                If Instance Is Nothing Then
                                    Throw New Exception(String.Format("读取版本设置 {0} 时未提供目标版本", Key))
                                End If
                                text = ModBase.ReadIni(Instance.PathVersion + "PCL\Setup.ini", Key, text2)
                        End Select
                        If setting.Encrypted Then
                            If Operators.CompareString(text, text2, False) = 0 Then
                                text = Conversions.ToString(setting.DefaultValue)
                            Else
                                Try
                                    text = Settings.DesDecrypt(text, "PCL" + ModBase.Identify)
                                Catch ex As Exception
                                    Logger.Warn(ex, String.Format("解密设置失败：{0}", Key), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\Settings.vb")
                                    text = Conversions.ToString(setting.DefaultValue)
                                    setting.[Set](RuntimeHelpers.GetObjectValue(setting.DefaultValue), Instance)
                                    setting.Save(Instance)
                                End Try
                            End If
                        End If
                        setting.[Set](RuntimeHelpers.GetObjectValue(Conversion.CTypeDynamic(text, setting.Type)), Instance)
                    Catch ex2 As Exception
                        Logger.[Error](ex2, String.Format("读取设置失败：{0}", Key), LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\Settings.vb")
                        setting.[Set](RuntimeHelpers.GetObjectValue(Conversion.CTypeDynamic(RuntimeHelpers.GetObjectValue(setting.DefaultValue), setting.Type)), Instance)
                    End Try
                    obj = setting.GetCache(Instance)
                End If
            End If
            Return obj
        End Function

        Public Shared Function [Get](Of T)(Key As String, Optional Instance As ModMinecraft.McInstance = Nothing) As T
            Return Conversions.ToGenericParameter(Of T)(Settings.[Get](Key, Instance))
        End Function

        Public Shared Function GetSafe(Key As String, Optional Instance As ModMinecraft.McInstance = Nothing) As Object
            Dim setting As Settings.Setting = Nothing
            If Not Settings.Entries.TryGetValue(Key, setting) Then
                Throw New KeyNotFoundException("未找到设置项：" + Key)
            End If
            If setting.Encrypted Then
                Throw New InvalidOperationException("禁止读取加密设置项：" + Key)
            End If
            Return Settings.[Get](Key, Instance)
        End Function

        Public Shared Sub Reset(Key As String, Optional Instance As ModMinecraft.McInstance = Nothing)
            Dim setting As Settings.Setting = Nothing
            If Not Settings.Entries.TryGetValue(Key, setting) Then
                Throw New KeyNotFoundException("未找到设置项：" + Key)
            End If
            Try
                If setting.GetCache(Instance) Is Nothing Then
                    Settings.[Get](Key, Instance)
                End If
                If Not Operators.ConditionalCompareObjectEqual(setting.GetCache(Instance), setting.DefaultValue, False) Then
                    setting.[Set](RuntimeHelpers.GetObjectValue(setting.DefaultValue), Instance)
                    Select Case setting.Source
                        Case Settings.Sources.Normal
                            ModBase.DeleteIniKey("Setup", Key)
                            GoTo IL_00EF
                        Case Settings.Sources.Registry
                            Try
                                RegistryUtils.Delete(String.Format("HKEY_CURRENT_USER\Software\{0}", "PCL"), Key)
                                GoTo IL_00EF
                            Catch ex As Exception
                                Logger.Warn(ex, String.Format("删除注册表失败（{0}）", Key), LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\Settings.vb")
                                GoTo IL_00EF
                            End Try
                        Case Settings.Sources.Instance
                        Case Else
                            GoTo IL_00EF
                    End Select
                    If Instance Is Nothing Then
                        Throw New Exception(String.Format("重置版本设置 {0} 时未提供目标版本", Key))
                    End If
                    ModBase.DeleteIniKey(Instance.PathVersion + "PCL\Setup.ini", Key)
                    IL_00EF:
                    If setting.OnChanged IsNot Nothing Then
                        setting.OnChanged(RuntimeHelpers.GetObjectValue(setting.DefaultValue))
                    End If
                End If
            Catch ex2 As Exception
                Logger.[Error](ex2, String.Format("重置设置项时出错（{0}）", Key), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\Settings.vb")
            End Try
        End Sub

        Public Shared Function GetDefault(Key As String) As String
            Return Conversions.ToString(Settings.Entries(Key).DefaultValue)
        End Function

        Public Shared Function HasSaved(Key As String, Optional Instance As ModMinecraft.McInstance = Nothing) As Boolean
            Dim source As Settings.Sources = Settings.Entries(Key).Source
            Dim flag As Boolean
            If source <> Settings.Sources.Normal Then
                If source = Settings.Sources.Registry Then
                    Try
                        Return RegistryUtils.Has(String.Format("HKEY_CURRENT_USER\Software\{0}", "PCL"), Key)
                    Catch ex As Exception
                        Logger.Warn(ex, String.Format("判断注册表 {0} 是否存在时出错", Key), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\Settings.vb")
                        Return False
                    End Try
                End If
                If Instance Is Nothing Then
                    Throw New Exception(String.Format("判断版本设置 {0} 是否存在时未提供目标版本", Key))
                End If
                flag = ModBase.HasIniKey(Instance.PathVersion + "PCL\Setup.ini", Key)
            Else
                flag = ModBase.HasIniKey("Setup", Key)
            End If
            Return flag
        End Function

        Private Shared Function DesEncrypt(sourceString As String, Optional key As String = Nothing) As String
            key = If((key Is Nothing), "@;$ Abv2", key.GetStableHashCode().ToString().EnsureLength("X"c, 8).Substring(0, 8))
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(key)
            Dim text As String
            Using descryptoServiceProvider As DESCryptoServiceProvider = New DESCryptoServiceProvider()
                Using memoryStream As MemoryStream = New MemoryStream()
                    Using cryptoStream As CryptoStream = New CryptoStream(memoryStream, descryptoServiceProvider.CreateEncryptor(bytes, Settings.desInitialVector), CryptoStreamMode.Write)
                        Dim bytes2 As Byte() = Encoding.UTF8.GetBytes(sourceString)
                        cryptoStream.Write(bytes2, 0, bytes2.Length)
                        cryptoStream.FlushFinalBlock()
                        text = Convert.ToBase64String(memoryStream.ToArray())
                    End Using
                End Using
            End Using
            Return text
        End Function

        Private Shared Function DesDecrypt(encryptedString As String, Optional key As String = Nothing) As String
            key = If((key Is Nothing), "@;$ Abv2", key.GetStableHashCode().ToString().EnsureLength("X"c, 8).Substring(0, 8))
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(key)
            Dim [string] As String
            Using descryptoServiceProvider As DESCryptoServiceProvider = New DESCryptoServiceProvider()
                Using memoryStream As MemoryStream = New MemoryStream()
                    Using cryptoStream As CryptoStream = New CryptoStream(memoryStream, descryptoServiceProvider.CreateDecryptor(bytes, Settings.desInitialVector), CryptoStreamMode.Write)
                        Dim array As Byte() = Convert.FromBase64String(encryptedString)
                        cryptoStream.Write(array, 0, array.Length)
                        cryptoStream.FlushFinalBlock()
                        [string] = Encoding.UTF8.GetString(memoryStream.ToArray())
                    End Using
                End Using
            End Using
            Return [string]
        End Function

        Public Shared Entries As Dictionary(Of String, Settings.Setting) = New List(Of Settings.Setting)() From { New Settings.Setting("Identify", "", Settings.Sources.Registry, False, Nothing), New Settings.Setting("WindowHeight", 550, Settings.Sources.Normal, False, Nothing), New Settings.Setting("WindowWidth", 900, Settings.Sources.Normal, False, Nothing), New Settings.Setting("AprilYear", 0, Settings.Sources.Registry, True, Nothing), New Settings.Setting("Potatoes", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("HintDownloadThread", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("HintNotice", 0, Settings.Sources.Registry, False, Nothing), New Settings.Setting("HintDownload", 0, Settings.Sources.Registry, False, Nothing), New Settings.Setting("HintHide", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("HintHandInstall", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("HintBuy", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("HintClearRubbish", 0, Settings.Sources.Registry, False, Nothing), New Settings.Setting("HintUpdateMod", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("HintCustomCommand", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("HintCustomWarn", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("HintMoreAdvancedSetup", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("HintIndieSetup", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("HintExportConfig", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("HintSnapshot", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("SystemEulaVersion", 0, Settings.Sources.Registry, False, Nothing), New Settings.Setting("SystemCount", 0, Settings.Sources.Registry, True, Nothing), New Settings.Setting("SystemLaunchCount", 0, Settings.Sources.Registry, True, Nothing), New Settings.Setting("SystemLastVersionReg", 0, Settings.Sources.Registry, True, Nothing), New Settings.Setting("SystemHighestSavedBetaVersionReg", 0, Settings.Sources.Registry, True, Nothing), New Settings.Setting("SystemHighestBetaVersionReg", 0, Settings.Sources.Registry, True, Nothing), New Settings.Setting("SystemHighestAlphaVersionReg", 0, Settings.Sources.Registry, True, Nothing), New Settings.Setting("SystemHelpVersion", 0, Settings.Sources.Registry, False, Nothing), New Settings.Setting("SystemDebugMode", False, Settings.Sources.Registry, False, Sub(a0 As Object)
            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_10
            If Settings._Closure$__.$I0-0 IsNot Nothing Then
                vb$AnonymousDelegate_ = Settings._Closure$__.$I0-0
            Else
                Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_10 = Sub()
                    Logger.Instance.MinLevel = If(ModBase.ModeDebug, LogLevel.Trace, LogLevel.Info)
                End Sub
                vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                Settings._Closure$__.$I0-0 = vb$AnonymousDelegate_2
            End If
            vb$AnonymousDelegate_()
        End Sub), New Settings.Setting("SystemDebugAnim", 9, Settings.Sources.Registry, False, Nothing), New Settings.Setting("SystemDebugDelay", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("SystemDebugSkipCopy", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("SystemSystemCache", "", Settings.Sources.Registry, False, Nothing), New Settings.Setting("SystemSystemUpdate", 0, Settings.Sources.Normal, False, Nothing), New Settings.Setting("SystemSystemActivity", 0, Settings.Sources.Normal, False, Nothing), New Settings.Setting("SystemSystemTelemetry", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("CacheDrops", "", Settings.Sources.Registry, False, Nothing), New Settings.Setting("CacheConfig", 0, Settings.Sources.Registry, False, Nothing), New Settings.Setting("CacheExportConfig", "", Settings.Sources.Registry, False, Nothing), New Settings.Setting("CacheSavedPageUrl", "", Settings.Sources.Registry, False, Nothing), New Settings.Setting("CacheSavedPageVersion", "", Settings.Sources.Registry, False, Nothing), New Settings.Setting("CacheMsOAuthRefresh", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheMsAccess", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheMsProfileJson", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheMsUuid", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheMsName", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheMsV2Migrated", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("CacheMsV2OAuthRefresh", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheMsV2Access", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheMsV2ProfileJson", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheMsV2Uuid", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheMsV2Name", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheMsV2Expires", 0L, Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheNideAccess", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheNideClient", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheNideUuid", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheNideName", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheNideUsername", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheNidePass", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheNideServer", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheAuthAccess", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheAuthClient", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheAuthUuid", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheAuthName", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheAuthUsername", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheAuthPass", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheAuthServerServer", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheAuthServerName", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheAuthServerRegister", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("CacheDownloadFolder", "", Settings.Sources.Registry, False, Nothing), New Settings.Setting("CacheJavaListVersion", 0, Settings.Sources.Registry, False, Nothing), New Settings.Setting("LoginRemember", True, Settings.Sources.Registry, True, Nothing), New Settings.Setting("LoginLegacyName", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("LoginMsJson", "{}", Settings.Sources.Registry, True, Nothing), New Settings.Setting("LoginNideEmail", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("LoginNidePass", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("LoginAuthEmail", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("LoginAuthPass", "", Settings.Sources.Registry, True, Nothing), New Settings.Setting("LoginType", ModLaunch.McLoginType.Legacy, Settings.Sources.Registry, False, Nothing), New Settings.Setting("LoginPageType", 0, Settings.Sources.Normal, False, Nothing), New Settings.Setting("LaunchSkinID", "", Settings.Sources.Registry, False, Sub(a0 As Object)
            Dim vb$AnonymousDelegate_3 As VB$AnonymousDelegate_10
            If Settings._Closure$__.$I0-1 IsNot Nothing Then
                vb$AnonymousDelegate_3 = Settings._Closure$__.$I0-1
            Else
                Dim vb$AnonymousDelegate_4 As VB$AnonymousDelegate_10 = Sub()
                    PageLaunchLeft.SkinLegacy.Start(Nothing, False)
                End Sub
                vb$AnonymousDelegate_3 = vb$AnonymousDelegate_4
                Settings._Closure$__.$I0-1 = vb$AnonymousDelegate_4
            End If
            vb$AnonymousDelegate_3()
        End Sub), New Settings.Setting("LaunchSkinType", 0, Settings.Sources.Registry, False, Sub(a0 As Object)
            PageSetupLaunch.UpdateSkinType()
        End Sub), New Settings.Setting("LaunchSkinSlim", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("LaunchFolderSelect", "", Settings.Sources.Normal, False, Nothing), New Settings.Setting("LaunchFolders", "", Settings.Sources.Registry, False, Nothing), New Settings.Setting("LaunchArgumentTitle", "", Settings.Sources.Normal, False, Nothing), New Settings.Setting("LaunchArgumentInfo", "PCL", Settings.Sources.Normal, False, Nothing), New Settings.Setting("LaunchArgumentJavaSelect", "", Settings.Sources.Registry, False, Nothing), New Settings.Setting("LaunchArgumentJavaAll", "[]", Settings.Sources.Registry, False, Nothing), New Settings.Setting("LaunchArgumentIndie", 0, Settings.Sources.Normal, False, Nothing), New Settings.Setting("LaunchArgumentIndieV2", 4, Settings.Sources.Normal, False, Nothing), New Settings.Setting("LaunchArgumentVisible", 5, Settings.Sources.Registry, False, Nothing), New Settings.Setting("LaunchArgumentPriority", 1, Settings.Sources.Registry, False, Nothing), New Settings.Setting("LaunchArgumentWindowWidth", 854, Settings.Sources.Normal, False, Nothing), New Settings.Setting("LaunchArgumentWindowHeight", 480, Settings.Sources.Normal, False, Nothing), New Settings.Setting("LaunchArgumentWindowType", 1, Settings.Sources.Normal, False, Nothing), New Settings.Setting("LaunchArgumentRam", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("LaunchAdvanceJvm", "-XX:-OmitStackTraceInFastThrow -Djdk.lang.Process.allowAmbiguousCommands=True -Dfml.ignoreInvalidMinecraftCertificates=True -Dfml.ignorePatchDiscrepancies=True", Settings.Sources.Normal, False, Nothing), New Settings.Setting("LaunchAdvanceGame", "", Settings.Sources.Normal, False, Nothing), New Settings.Setting("LaunchAdvanceRun", "", Settings.Sources.Normal, False, Nothing), New Settings.Setting("LaunchAdvanceRunWait", True, Settings.Sources.Normal, False, Nothing), New Settings.Setting("LaunchAdvanceDisableJLW", False, Settings.Sources.Normal, False, Nothing), New Settings.Setting("LaunchAdvanceDisableLUA", False, Settings.Sources.Normal, False, Nothing), New Settings.Setting("LaunchAdvanceGraphicCard", True, Settings.Sources.Registry, False, Nothing), New Settings.Setting("LaunchAdvanceGC", 4, Settings.Sources.Normal, False, Nothing), New Settings.Setting("LaunchRamType", 0, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupLaunch.UpdateRamType()
        End Sub), New Settings.Setting("LaunchRamCustom", 15, Settings.Sources.Normal, False, Nothing), New Settings.Setting("LinkLastAutoJoinInviteCode", "", Settings.Sources.Registry, False, Nothing), New Settings.Setting("LinkLatencyMode", 0, Settings.Sources.Registry, False, Nothing), New Settings.Setting("LinkCustomPeer", "", Settings.Sources.Normal, False, Nothing), New Settings.Setting("LinkEasyTierVersion", -1, Settings.Sources.Registry, False, Nothing), New Settings.Setting("ToolHelpChinese", True, Settings.Sources.Registry, False, Nothing), New Settings.Setting("ToolDownloadThread", 63, Settings.Sources.Registry, False, Nothing), New Settings.Setting("ToolDownloadSpeed", 42, Settings.Sources.Registry, False, Sub(a0 As Object)
            ModNet.UpdateNetTaskSpeedLimitHigh(Conversions.ToInteger(a0))
        End Sub), New Settings.Setting("ToolDownloadSource", 1, Settings.Sources.Registry, False, Nothing), New Settings.Setting("ToolDownloadVersion", 1, Settings.Sources.Registry, False, Nothing), New Settings.Setting("ToolDownloadTranslate", 0, Settings.Sources.Registry, False, Nothing), New Settings.Setting("ToolDownloadTranslateV2", 1, Settings.Sources.Registry, False, Nothing), New Settings.Setting("ToolDownloadCert", False, Settings.Sources.Registry, False, Sub(a0 As Object)
            ModNet.ShouldValidateSslCertificateOnLogin(Conversions.ToBoolean(a0))
        End Sub), New Settings.Setting("ToolDownloadMod", 2, Settings.Sources.Registry, False, Nothing), New Settings.Setting("ToolModLocalNameStyle", 0, Settings.Sources.Registry, False, Nothing), New Settings.Setting("ToolUpdateRelease", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("ToolUpdateSnapshot", False, Settings.Sources.Registry, False, Nothing), New Settings.Setting("ToolUpdateReleaseLast", "", Settings.Sources.Registry, False, Nothing), New Settings.Setting("ToolUpdateSnapshotLast", "", Settings.Sources.Registry, False, Nothing), New Settings.Setting("UiLauncherTransparent", 600, Settings.Sources.Normal, False, Sub(a0 As Object)
            Dim vb$AnonymousDelegate_5 As VB$AnonymousDelegate_24(Of Integer)
            If Settings._Closure$__.$I0-2 IsNot Nothing Then
                vb$AnonymousDelegate_5 = Settings._Closure$__.$I0-2
            Else
                Dim vb$AnonymousDelegate_6 As VB$AnonymousDelegate_24(Of Integer) = Sub(Value As Integer)
                    If ModMain.FrmMain IsNot Nothing Then
                        ModMain.FrmMain.Opacity = CDbl(Value) / 1000.0 + 0.4
                    End If
                End Sub
                vb$AnonymousDelegate_5 = vb$AnonymousDelegate_6
                Settings._Closure$__.$I0-2 = vb$AnonymousDelegate_6
            End If
            vb$AnonymousDelegate_5(Conversions.ToInteger(a0))
        End Sub), New Settings.Setting("UiLauncherHue", 180, Settings.Sources.Normal, False, Nothing), New Settings.Setting("UiLauncherSat", 80, Settings.Sources.Normal, False, Nothing), New Settings.Setting("UiLauncherDelta", 90, Settings.Sources.Normal, False, Nothing), New Settings.Setting("UiLauncherLight", 20, Settings.Sources.Normal, False, Nothing), New Settings.Setting("UiLauncherTheme", 0, Settings.Sources.Normal, False, Sub(a0 As Object)
            ModSecret.ThemeRefresh(Conversions.ToInteger(a0))
        End Sub), New Settings.Setting("UiLauncherThemeHide", "0|1|2|3|4", Settings.Sources.Registry, True, Nothing), New Settings.Setting("UiLauncherThemeHide2", "0|1|2|3|4", Settings.Sources.Registry, True, Nothing), New Settings.Setting("UiLauncherLogo", True, Settings.Sources.Normal, False, Nothing), New Settings.Setting("UiLauncherEmail", False, Settings.Sources.Normal, False, Nothing), New Settings.Setting("UiBackgroundColorful", True, Settings.Sources.Normal, False, Sub(a0 As Object)
            Dim vb$AnonymousDelegate_7 As VB$AnonymousDelegate_10
            If Settings._Closure$__.$I0-3 IsNot Nothing Then
                vb$AnonymousDelegate_7 = Settings._Closure$__.$I0-3
            Else
                Dim vb$AnonymousDelegate_8 As VB$AnonymousDelegate_10 = Sub()
                    ModSecret.ThemeRefresh(-1)
                End Sub
                vb$AnonymousDelegate_7 = vb$AnonymousDelegate_8
                Settings._Closure$__.$I0-3 = vb$AnonymousDelegate_8
            End If
            vb$AnonymousDelegate_7()
        End Sub), New Settings.Setting("UiBackgroundOpacity", 1000, Settings.Sources.Normal, False, AddressOf FormMain.UpdateBackgroundAndTitleBar), New Settings.Setting("UiBackgroundBlur", 0, Settings.Sources.Normal, False, AddressOf FormMain.UpdateBackgroundAndTitleBar), New Settings.Setting("UiBackgroundSuit", 0, Settings.Sources.Normal, False, AddressOf FormMain.UpdateBackgroundAndTitleBar), New Settings.Setting("UiCustomType", 0, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.OnMainPageTypeChanged()
        End Sub), New Settings.Setting("UiCustomPreset", 0, Settings.Sources.Normal, False, Nothing), New Settings.Setting("UiCustomNet", "", Settings.Sources.Normal, False, Nothing), New Settings.Setting("UiLogoType", 1, Settings.Sources.Normal, False, AddressOf FormMain.UpdateBackgroundAndTitleBar), New Settings.Setting("UiLogoText", "", Settings.Sources.Normal, False, AddressOf FormMain.UpdateBackgroundAndTitleBar), New Settings.Setting("UiLogoLeft", False, Settings.Sources.Normal, False, AddressOf FormMain.UpdateBackgroundAndTitleBar), New Settings.Setting("UiMusicVolume", 500, Settings.Sources.Normal, False, Nothing), New Settings.Setting("UiMusicStop", False, Settings.Sources.Normal, False, Nothing), New Settings.Setting("UiMusicStart", False, Settings.Sources.Normal, False, Nothing), New Settings.Setting("UiMusicRandom", True, Settings.Sources.Normal, False, Nothing), New Settings.Setting("UiMusicAuto", True, Settings.Sources.Normal, False, Nothing), New Settings.Setting("UiHiddenPageDownload", False, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.HiddenRefresh()
        End Sub), New Settings.Setting("UiHiddenPageLink", False, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.HiddenRefresh()
        End Sub), New Settings.Setting("UiHiddenPageSetup", False, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.HiddenRefresh()
        End Sub), New Settings.Setting("UiHiddenPageOther", False, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.HiddenRefresh()
        End Sub), New Settings.Setting("UiHiddenFunctionSelect", False, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.HiddenRefresh()
        End Sub), New Settings.Setting("UiHiddenFunctionModUpdate", False, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.HiddenRefresh()
        End Sub), New Settings.Setting("UiHiddenFunctionInstanceModify", False, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.HiddenRefresh()
        End Sub), New Settings.Setting("UiHiddenFunctionHidden", False, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.HiddenRefresh()
        End Sub), New Settings.Setting("UiHiddenSetupLaunch", False, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.HiddenRefresh()
        End Sub), New Settings.Setting("UiHiddenSetupUi", False, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.HiddenRefresh()
        End Sub), New Settings.Setting("UiHiddenSetupLink", False, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.HiddenRefresh()
        End Sub), New Settings.Setting("UiHiddenSetupSystem", False, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.HiddenRefresh()
        End Sub), New Settings.Setting("UiHiddenOtherHelp", False, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.HiddenRefresh()
        End Sub), New Settings.Setting("UiHiddenOtherFeedback", False, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.HiddenRefresh()
        End Sub), New Settings.Setting("UiHiddenOtherVote", False, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.HiddenRefresh()
        End Sub), New Settings.Setting("UiHiddenOtherAbout", False, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.HiddenRefresh()
        End Sub), New Settings.Setting("UiHiddenOtherTest", False, Settings.Sources.Normal, False, Sub(a0 As Object)
            PageSetupUI.HiddenRefresh()
        End Sub), New Settings.Setting("VersionAdvanceJvm", "", Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionAdvanceGame", "", Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionAdvanceAssets", 0, Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionAdvanceAssetsV2", False, Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionAdvanceRun", "", Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionAdvanceRunWait", True, Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionAdvanceDisableJLW", False, Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionAdvanceDisableLUA", False, Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionAdvanceDisableModUpdate", False, Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionAdvanceGC", 0, Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionRamType", 2, Settings.Sources.Instance, False, Sub(a0 As Object)
            PageInstanceSetup.OnVersionRamTypeChanged(Conversions.ToInteger(a0))
        End Sub), New Settings.Setting("VersionRamCustom", 15, Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionRamOptimize", 0, Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionArgumentTitle", "", Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionArgumentInfo", "", Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionArgumentIndie", -1, Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionArgumentIndieV2", False, Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionArgumentJavaSelect", "使用全局设置", Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionArgumentJavaV2", 0, Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionArgumentJavaRange", "", Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionServerEnter", "", Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionServerLogin", 0, Settings.Sources.Instance, False, Sub(a0 As Object)
            PageInstanceSetup.OnVersionServerLoginChanged(Conversions.ToInteger(a0))
        End Sub), New Settings.Setting("VersionServerNide", "", Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionServerAuthRegister", "", Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionServerAuthName", "", Settings.Sources.Instance, False, Nothing), New Settings.Setting("VersionServerAuthServer", "", Settings.Sources.Instance, False, Nothing) }.ToDictionary(Function(e As Settings.Setting) e.Key)

        Private Shared desInitialVector As Byte() = Encoding.UTF8.GetBytes("95168702")

        Private Shared $STATIC$Get$021CE1287E8$ForcedSettings As Dictionary(Of String, Object)

        Private Shared $STATIC$Get$021CE1287E8$ForcedSettings$Init As StaticLocalInitFlag

        Public Enum Sources
            Normal
            Registry
            Instance
        End Enum

        Public Class Setting
            Public Sub New(Key As String, Value As Object, Optional Source As Settings.Sources = Settings.Sources.Normal, Optional Encrypted As Boolean = False, Optional OnChanged As Action(Of Object) = Nothing)
                Me.ValueCache = New ConcurrentDictionary(Of String, Object)()
                Try
                    Me.Key = Key
                    Me.DefaultValue = RuntimeHelpers.GetObjectValue(Value)
                    Me.Encrypted = Encrypted
                    Me.Source = Source
                    Me.Type = (If(Value, "")).[GetType]()
                    Me.OnChanged = OnChanged
                Catch ex As Exception
                    Logger.[Error](ex, "初始化设置项失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\Settings.vb")
                End Try
            End Sub

            Public Function GetCache(Instance As ModMinecraft.McInstance) As Object
                Dim text As String = If((Me.Source = Settings.Sources.Instance), Instance.PathVersion, "")
                Dim obj As Object = Nothing
                If Not Me.ValueCache.TryGetValue(text, obj) Then
                    Return Nothing
                End If
                Return obj
            End Function

            Public Sub [Set](Value As Object, Instance As ModMinecraft.McInstance)
                Dim text As String = If((Me.Source = Settings.Sources.Instance), Instance.PathVersion, "")
                Me.ValueCache(text) = RuntimeHelpers.GetObjectValue(Value)
            End Sub

            Public Sub Save(Optional Instance As ModMinecraft.McInstance = Nothing)
                Dim text As String = Conversions.ToString(Me.GetCache(Instance))
                Logger.Trace(String.Format("保存设置：{0} {1}{2}", Me.Key, If(Me.Encrypted, "", ("= " + text)), If((Instance Is Nothing), "", String.Format("（实例：{0}）", If((Instance IsNot Nothing), Instance.PathVersion, Nothing)))), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\Settings.vb")
                If Me.Encrypted Then
                    Try
                        If text Is Nothing Then
                            text = ""
                        End If
                        text = Settings.DesEncrypt(text, "PCL" + ModBase.Identify)
                    Catch ex As Exception
                        Logger.Warn(ex, String.Format("加密设置失败：{0}", Me.Key), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\Settings.vb")
                    End Try
                End If
                Select Case Me.Source
                    Case Settings.Sources.Normal
                        ModBase.WriteIni("Setup", Me.Key, text)
                        Return
                    Case Settings.Sources.Registry
                        Try
                            RegistryUtils.Write(String.Format("HKEY_CURRENT_USER\Software\{0}", "PCL"), Me.Key, text)
                            Return
                        Catch ex2 As Exception
                            Logger.Warn(ex2, String.Format("写入注册表失败（{0} → {1}）", Me.Key, text), LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\Settings.vb")
                            Return
                        End Try
                    Case Settings.Sources.Instance
                    Case Else
                        Return
                End Select
                If Instance Is Nothing Then
                    Throw New Exception(String.Format("保存版本独立设置 {0} 时未提供目标版本", Me.Key))
                End If
                ModBase.WriteIni(Instance.PathVersion + "PCL\Setup.ini", Me.Key, text)
            End Sub

            Public Key As String

            Public Encrypted As Boolean

            Public DefaultValue As Object

            Public Source As Settings.Sources

            Public OnChanged As Action(Of Object)

            Public Type As Type

            Public ValueCache As ConcurrentDictionary(Of String, Object)
        End Class
    End Class
End Namespace
