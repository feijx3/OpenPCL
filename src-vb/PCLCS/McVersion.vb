Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace PCLCS
    <NullableContext(1)>
    <Nullable(0)>
    Public NotInheritable Class McVersion
        Public Property VanillaName As String
            Get
                Return Me._vanillaName
            End Get
            Set(value As String)
                Me._vanillaName = If(value, "Unknown")
                Me.Vanilla = McVersion.NameToVersion(Me._vanillaName)
            End Set
        End Property

        Public ReadOnly Property Valid As Boolean
            Get
                Return Me.Vanilla.Major < 1000
            End Get
        End Property

        Public ReadOnly Property Drop As Integer
            Get
                If Not Me.Valid Then
                    Return 209
                End If
                Return Me.Vanilla.Major * 10 + Me.Vanilla.Minor
            End Get
        End Property

        Public ReadOnly Property ModLoaders As ModLoaders
            Get
                Dim keys As IEnumerable(Of ModLoaders) = Me.ModLoaderVersions.Keys
                Dim modLoaders As ModLoaders = ModLoaders.None
                Dim <>9__11_ As Func(Of ModLoaders, ModLoaders, ModLoaders) = McVersion.<>c.<>9__11_0
                Dim func As Func(Of ModLoaders, ModLoaders, ModLoaders) = <>9__11_
                If <>9__11_ Is Nothing Then
                    Dim func2 As Func(Of ModLoaders, ModLoaders, ModLoaders) = Function(result As ModLoaders, loader As ModLoaders) result Or loader
                    func = func2
                    McVersion.<>c.<>9__11_0 = func2
                End If
                Return keys.Aggregate(modLoaders, func)
            End Get
        End Property

        Public ReadOnly Property ModLoaderVersions As Dictionary(Of ModLoaders, String) = New Dictionary(Of ModLoaders, String)()

        Public ReadOnly Property HasOptiFine As Boolean
            Get
                Return Me.OptiFine <> Nothing
            End Get
        End Property

        Public ReadOnly Property OptiFineCode As Integer
            Get
                Dim optiFine As String = Me.OptiFine
                Dim flag As Boolean = optiFine Is Nothing OrElse optiFine = "未知版本"
                If flag OrElse Me.OptiFine.Length = 0 Then
                    Return 0
                End If
                Dim num As Integer = CInt((Char.ToUpperInvariant(Me.OptiFine(0)) - "A"c + ChrW(1)))
                Dim num2 As Integer
                num = num * 100 + If(Integer.TryParse(Me.OptiFine.Substring(1).RegexSeek("\d+", RegexOptions.None), num2), num2, 0)
                num *= 100
                Dim text As String = Me.OptiFine.RegexSeek("(?i)(pre|beta)\d*", RegexOptions.None)
                If text Is Nothing Then
                    Return num + 99
                End If
                Dim num4 As Integer
                Dim num3 As Integer = If(Integer.TryParse(text.RegexSeek("\d+", RegexOptions.None), num4), num4, 1)
                Return num + If(text.StartsWithF("pre", True), 50, 0) + num3
            End Get
        End Property

        Public ReadOnly Property ForgelikeCode As Integer
            Get
                Dim text As String
                If Not Me.ModLoaderVersions.TryGetValue(ModLoaders.Forge, text) Then
                    Me.ModLoaderVersions.TryGetValue(ModLoaders.NeoForge, text)
                End If
                Dim flag As Boolean = text Is Nothing OrElse text = "未知版本"
                If flag Then
                    Return 0
                End If
                Dim enumerable As IEnumerable(Of String) = text.RegexSearch("\d+", RegexOptions.None)
                Dim <>9__25_ As Func(Of String, Integer) = McVersion.<>c.<>9__25_0
                Dim func As Func(Of String, Integer) = <>9__25_
                If <>9__25_ Is Nothing Then
                    Dim func2 As Func(Of String, Integer) = Function(segment As String)
                        Dim num3 As Integer
                        If Not Integer.TryParse(segment, num3) Then
                            Return 0
                        End If
                        Return num3
                    End Function
                    func = func2
                    McVersion.<>c.<>9__25_0 = func2
                End If
                Dim array As Integer() = enumerable.[Select](func).ToArray()
                Dim num As Integer = array.Length
                Dim num2 As Integer
                If num < 4 Then
                    Select Case num
                        Case 1
                            num2 = array(0) * 1000000
                        Case 2
                            num2 = array(0) * 1000000 + array(1) * 10000
                        Case 3
                            num2 = array(0) * 1000000 + array(1) * 10000 + array(2)
                        Case Else
                            num2 = 0
                    End Select
                Else
                    num2 = array(0) * 1000000 + array(1) * 10000 + array(3)
                End If
                Return num2
            End Get
        End Property

        Public Sub New(vanillaName As String, Optional reliable As Boolean = True)
            Me.Reliable = reliable
            Me.VanillaName = vanillaName
            Me.IsFool = McVersion.IsFoolName(Me.VanillaName)
            Me.IsSnapshot = McVersion.IsSnapshotName(Me.VanillaName)
        End Sub

        Public Sub New(components As McComponents)
            Me.New(components.MinecraftVersion, True)
            Dim optiFineVersion As String = components.OptiFineVersion
            Me.OptiFine = If((optiFineVersion IsNot Nothing), optiFineVersion.AfterLast("HD_", False).AfterFirst("_", False), Nothing)
            For Each keyValuePair As KeyValuePair(Of ModLoaders, String) In components.ModLoaderVersions
                Me.ModLoaderVersions(keyValuePair.Key) = keyValuePair.Value
            Next
        End Sub

        Public Sub New(instanceName As String, json As JObject, <Nullable(2)> Optional jarVersionJson As JObject = Nothing)
            Dim text As String = json.ToString(0, Array.Empty(Of JsonConverter)())
            Dim dateTime As DateTime = New DateTime(1970, 1, 1, 15, 0, 0)
            Try
                Dim jtoken As JToken = json("releaseTime")
                If jtoken IsNot Nothing Then
                    dateTime = jtoken.ToObject(Of DateTime)()
                End If
            Catch
            End Try
            Dim flag As Boolean = True
            Dim text2 As String = (If(McVersion.GetVanillaName(instanceName, json, jarVersionJson, dateTime, flag), "Unknown")).Replace("_unobfuscated", "").Replace(" Unobfuscated", "")
            Me.Reliable = flag
            Me.VanillaName = text2
            Me.ReleaseTime = dateTime
            Dim text3 As String = If(json.Value(Of String)("type"), "")
            Me.OptiFine = If(text.ContainsIgnoreCase("optifine"), (If(text.RegexSeek("(?<=HD_U_)[^\"":/]+", RegexOptions.None), "未知版本")), Nothing)
            Dim dateTime2 As DateTime = dateTime.ToUniversalTime().AddHours(2.0)
            Me.IsFool = text3 = "fool" OrElse McVersion.IsFoolName(text2) OrElse (dateTime2.Month = 4 AndAlso dateTime2.Day = 1 AndAlso text3 = "snapshot")
            Dim flag2 As Boolean = McVersion.IsSnapshotName(text2) OrElse instanceName.ContainsIgnoreCase("combat")
            If Not flag2 Then
                Dim flag3 As Boolean = text3 = "snapshot" OrElse text3 = "pending" OrElse text3 = "fool"
                flag2 = flag3
            End If
            Me.IsSnapshot = flag2
            If Not Me.Reliable Then
                Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(17, 1, flag2)
                If flag2 Then
                    logInterpolatedStringHandler.AppendLiteral("无法完全确认 MC 版本号的版本：")
                    logInterpolatedStringHandler.AppendFormatted(Me.VanillaName)
                End If
                Logger.Info(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\PCLCS\Minecraft\McVersion.cs")
            End If
            If text.ContainsIgnoreCase("liteloader") Then
                Me.ModLoaderVersions(ModLoaders.LiteLoader) = If(text.RegexSeek("(?<=com\.mumfrey:liteloader:)[^\"":/]+", RegexOptions.None), "未知版本")
            End If
            If text.ContainsIgnoreCase("net.fabricmc:fabric-loader") OrElse text.ContainsIgnoreCase("org.quiltmc:quilt-loader") Then
                Dim modLoaderVersions As Dictionary(Of ModLoaders, String) = Me.ModLoaderVersions
                Dim modLoaders As ModLoaders = ModLoaders.Fabric
                Dim text4 As String = text.RegexSeek("(?<=(net\.fabricmc:fabric-loader:|org\.quiltmc:quilt-loader:))[0-9\.]+(\+build.[0-9]+)?", RegexOptions.None)
                modLoaderVersions(modLoaders) = If(If((text4 IsNot Nothing), text4.Replace("+build", ""), Nothing), "未知版本")
                Return
            End If
            If text.ContainsIgnoreCase("net.neoforge") Then
                Me.ModLoaderVersions(ModLoaders.NeoForge) = If(McVersion.FindArgumentValue(json, "--fml.neoForgeVersion"), If(McVersion.FindArgumentValue(json, "--fml.forgeVersion"), "未知版本"))
                Return
            End If
            If text.ContainsIgnoreCase("minecraftforge") Then
                Me.ModLoaderVersions(ModLoaders.Forge) = If(text.RegexSeek("(?<=forge:[0-9\.]+(?:_pre[0-9]*)?-)[0-9\.]+", RegexOptions.None), If(text.RegexSeek("(?<=net\.minecraftforge:minecraftforge:)[0-9\.]+", RegexOptions.None), If(text.RegexSeek("(?<=net\.minecraftforge:fmlloader:[0-9\.]+-)[0-9\.]+", RegexOptions.None), "未知版本")))
            End If
        End Sub

        Private Shared Function GetVanillaName(instanceName As String, json As JObject, <Nullable(2)> jarVersionJson As JObject, releaseTime As DateTime, ByRef reliable As Boolean) As <Nullable(2)> String
            Dim i As Integer = releaseTime.Year
            If i > 2000 AndAlso i < 2013 Then
                Return "Old"
            End If
            If json.Value(Of String)("type") = "pending" Then
                Return "pending"
            End If
            Dim jtoken As JToken = json("clientVersion")
            If jtoken IsNot Nothing Then
                Return jtoken.ToString()
            End If
            Dim jarray As JArray = TryCast(json("patches"), JArray)
            If jarray IsNot Nothing Then
                Dim enumerable As IEnumerable(Of JObject) = jarray.OfType()
                Dim <>9__29_ As Func(Of JObject, Boolean) = McVersion.<>c.<>9__29_0
                Dim func As Func(Of JObject, Boolean) = <>9__29_
                If <>9__29_ Is Nothing Then
                    Dim func2 As Func(Of JObject, Boolean) = Function(patch As JObject) patch.Value(Of String)("id") = "game"
                    func = func2
                    McVersion.<>c.<>9__29_0 = func2
                End If
                Dim jobject As JObject = enumerable.FirstOrDefault(func)
                Dim jtoken2 As JToken = If((jobject IsNot Nothing), jobject("version"), Nothing)
                If jtoken2 IsNot Nothing Then
                    Return jtoken2.ToString()
                End If
            End If
            Dim text As String = McVersion.FindArgumentValue(json, "--fml.mcVersion")
            If text IsNot Nothing AndAlso text.Length > 0 Then
                Return text
            End If
            Dim jtoken3 As JToken = json("inheritsFrom")
            Dim text2 As String = If(If((jtoken3 IsNot Nothing), jtoken3.ToString(), Nothing), "")
            If text2.Length > 0 Then
                Dim jtoken4 As JToken = json("jar")
                Dim text3 As String = If(If((jtoken4 IsNot Nothing), jtoken4.ToString(), Nothing), "")
                If text3.Length <= 0 Then
                    Return text2
                End If
                Return text3
            Else
                Dim jtoken5 As JToken = json("downloads")
                Dim text4 As String = If((jtoken5 IsNot Nothing), jtoken5.ToString(0, Array.Empty(Of JsonConverter)()).RegexSeek("(?<=launcher.mojang.com/mc/game/)[^/]*", RegexOptions.None), Nothing)
                If text4 IsNot Nothing Then
                    Return text4
                End If
                Dim text5 As String = json("libraries").ToString()
                For Each text6 As String In New String() { "(?<=net.minecraftforge:forge:)[0-9]{1,2}.[0-9+.]+", "(?<=net.minecraftforge:fmlloader:)[0-9]{1,2}.[0-9+.]+", "(?<=optifine:OptiFine:)[0-9]{1,2}.[0-9+.]+", "(?<=((fabricmc)|(quiltmc)):intermediary:)[^\""]*" }
                    Dim text7 As String = text5.RegexSeek(text6, RegexOptions.None)
                    If text7 IsNot Nothing Then
                        Return text7
                    End If
                Next
                Dim jtoken6 As JToken = json("jar")
                If jtoken6 IsNot Nothing Then
                    Return jtoken6.ToString()
                End If
                Dim jtoken7 As JToken = If((jarVersionJson IsNot Nothing), jarVersionJson("name"), Nothing)
                Dim flag As Boolean
                If jtoken7 IsNot Nothing Then
                    Dim text8 As String = jtoken7.ToString()
                    If text8 IsNot Nothing AndAlso text8.Length < 32 Then
                        Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(31, 1, flag)
                        If flag Then
                            logInterpolatedStringHandler.AppendLiteral("从版本 jar 中的 version.json 获取到版本号：")
                            logInterpolatedStringHandler.AppendFormatted(text8)
                        End If
                        Logger.Info(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\PCLCS\Minecraft\McVersion.cs")
                        Return text8
                    End If
                End If
                Dim text9 As String = "(([1-9][0-9]w[0-9]{2}[a-g])|((1|[2-9][0-9])\.[0-9]+(\.[0-9]+)?(-(pre|rc|snapshot-?)[1-9]*| Pre-Release( [1-9])?)?))(_unobfuscated)?"
                Dim text10 As String = json("id").ToString().RegexSeek(text9, RegexOptions.IgnoreCase)
                If text10 IsNot Nothing Then
                    Return text10
                End If
                Dim logInterpolatedStringHandler2 As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(17, 1, flag)
                If flag Then
                    logInterpolatedStringHandler2.AppendLiteral("无法完全确认 MC 版本号的版本：")
                    logInterpolatedStringHandler2.AppendFormatted(instanceName)
                End If
                Logger.Info(logInterpolatedStringHandler2, LogBehavior.None, "E:\Projects\PCL\本体代码\PCLCS\Minecraft\McVersion.cs")
                reliable = False
                Dim text11 As String = instanceName.RegexSeek(text9, RegexOptions.IgnoreCase)
                If text11 IsNot Nothing Then
                    Return text11
                End If
                Dim jobject2 As JObject = CType(json.DeepClone(), JObject)
                jobject2.Remove("libraries")
                Return jobject2.ToString(0, Array.Empty(Of JsonConverter)()).RegexSeek(text9, RegexOptions.IgnoreCase)
            End If
        End Function

        Private Shared Function IsFoolName(name As String) As Boolean
            name = name.Lower()
            Dim flag As Boolean = name.StartsWithF("2.0", False)
            If Not flag Then
                Dim flag2 As Boolean
                If name IsNot Nothing Then
                    Dim length As Integer = name.Length
                    If length <= 11 Then
                        If length <> 6 Then
                            If length <> 9 Then
                                If length <> 11 Then
                                    GoTo IL_0119
                                End If
                                Dim c As Char = name(1)
                                If c <> "3"c Then
                                    If c <> "4"c Then
                                        GoTo IL_0119
                                    End If
                                    If Not(name = "24w14potato") Then
                                        GoTo IL_0119
                                    End If
                                ElseIf Not(name = "23w13a_or_b") Then
                                    GoTo IL_0119
                                End If
                            ElseIf Not(name = "1.rv-pre1") Then
                                GoTo IL_0119
                            End If
                        Else
                            Dim c As Char = name(1)
                            If c <> "0"c Then
                                If c <> "5"c Then
                                    If c <> "6"c Then
                                        GoTo IL_0119
                                    End If
                                    If Not(name = "26w14a") Then
                                        GoTo IL_0119
                                    End If
                                ElseIf Not(name = "15w14a") Then
                                    GoTo IL_0119
                                End If
                            ElseIf Not(name = "20w14∞") Then
                                GoTo IL_0119
                            End If
                        End If
                    ElseIf length <> 14 Then
                        If length <> 18 Then
                            If length <> 20 Then
                                GoTo IL_0119
                            End If
                            If Not(name = "22w13oneblockatatime") Then
                                GoTo IL_0119
                            End If
                        ElseIf Not(name = "3d shareware v1.34") Then
                            GoTo IL_0119
                        End If
                    ElseIf Not(name = "25w14craftmine") Then
                        GoTo IL_0119
                    End If
                    flag2 = True
                    GoTo IL_011B
                End If
                IL_0119:
                flag2 = False
                IL_011B:
                flag = flag2
            End If
            Return flag OrElse name.StartsWithF("20w14inf", False)
        End Function

        Private Shared Function IsSnapshotName(name As String) As Boolean
            Return New String() { "w", "snapshot", "rc", "pre", "experimental", "-" }.Any(New Func(Of String, Boolean)(name.ContainsIgnoreCase))
        End Function

        Private Shared Function FindArgumentValue(json As JObject, name As String) As <Nullable(2)> String
            Dim jtoken As JToken = json("arguments")
            Dim jarray As JArray = TryCast(If((jtoken IsNot Nothing), jtoken("game"), Nothing), JArray)
            If jarray Is Nothing Then
                Return Nothing
            End If
            Dim num As Integer = 0
            While num + 1 < jarray.Count
                Dim jtoken2 As JToken = jarray(num)
                If If((jtoken2 IsNot Nothing), jtoken2.ToString(), Nothing) = name Then
                    Dim jtoken3 As JToken = jarray(num + 1)
                    If jtoken3 Is Nothing Then
                        Return Nothing
                    End If
                    Return jtoken3.ToString()
                Else
                    num += 1
                End If
            End While
            Return Nothing
        End Function

        <NullableContext(2)>
        Public Shared Function IsStandardFormat(version As String) As Boolean
            Dim num As Double
            Return version IsNot Nothing AndAlso (Regex.IsMatch(version, "^1\.\d") OrElse (Double.TryParse(version.RegexSeek("^[2-9]\d\.\d+", RegexOptions.None), NumberStyles.Float, CultureInfo.InvariantCulture, num) AndAlso num >= 26.0))
        End Function

        <NullableContext(2)>
        Public Shared Function VersionToDrop(vanillaName As String) As Integer
            If vanillaName Is Nothing Then
                Return 209
            End If
            Dim version As Version = McVersion.NameToVersion(vanillaName)
            If version.Major < 1000 Then
                Return version.Major * 10 + version.Minor
            End If
            Return 209
        End Function

        Public Shared Function DropToVersion(drop As Integer) As String
            If drop >= 250 Then
                Return String.Format("{0}.{1}", drop / 10, drop Mod 10)
            End If
            If drop <> 209 Then
                Return String.Format("1.{0}", drop / 10)
            End If
            Logger.Warn(New Exception("尝试将旧快照版 Drop 序数转换为版本字符串，这不应该发生！"), Nothing, LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\PCLCS\Minecraft\McVersion.cs")
            Return "1.20"
        End Function

        Public Shared Function NameToVersion(<Nullable(2)> name As String) As Version
            If name Is Nothing Then
                Return New Version(9999, 0, 0)
            End If
            name = name.Lower().Replace("_unobfuscated", "").Replace(" unobfuscated", "")
            If name.StartsWithF("2.0", False) Then
                Return McVersion.NameToVersion("1.5.1")
            End If
            If name = "15w14a" Then
                Return McVersion.NameToVersion("1.8.3")
            End If
            If name.ContainsIgnoreCase(".rv-pre") Then
                Return McVersion.NameToVersion("1.9.2")
            End If
            If name.ContainsIgnoreCase("shareware") Then
                Return McVersion.NameToVersion("1.13.2")
            End If
            If name.StartsWithF("20w14", False) AndAlso name <> "20w14a" Then
                Return McVersion.NameToVersion("1.15.2")
            End If
            If name.ContainsIgnoreCase("oneblockatatime") Then
                Return McVersion.NameToVersion("1.18.2")
            End If
            If name.ContainsIgnoreCase("23w13a") AndAlso name <> "20w13a" Then
                Return McVersion.NameToVersion("1.19.4")
            End If
            If name = "24w14potato" Then
                Return McVersion.NameToVersion("1.20.4")
            End If
            If name = "25w14craftmine" Then
                Return McVersion.NameToVersion("1.21.4")
            End If
            If name = "26w14a" Then
                Return McVersion.NameToVersion("26.1.1")
            End If
            Dim array As String() = name.Split(New Char() { " "c, "_"c, "-"c, "."c }, StringSplitOptions.None)
            If name.StartsWithF("1.", False) Then
                Return New Version(McVersion.<NameToVersion>g__ParseSegment|36_0(array, 1), 0, McVersion.<NameToVersion>g__ParseSegment|36_0(array, 2))
            End If
            If name.RegexCheck("^[2-9][0-9]\.", RegexOptions.None) Then
                Return New Version(McVersion.<NameToVersion>g__ParseSegment|36_0(array, 0), McVersion.<NameToVersion>g__ParseSegment|36_0(array, 1), McVersion.<NameToVersion>g__ParseSegment|36_0(array, 2))
            End If
            Return New Version(9999, 0, 0)
        End Function

        <CompilerGenerated()>
        Friend Shared Function <NameToVersion>g__ParseSegment|36_0(segments As String(), index As Integer) As Integer
            If index >= segments.Length Then
                Return 0
            End If
            Dim num As Integer
            If Not Integer.TryParse(segments(index).RegexSeek("^\d+", RegexOptions.None), num) Then
                Return 0
            End If
            Return num
        End Function

        Private _vanillaName As String = "Unknown"

        Public Vanilla As Version = New Version(9999, 0, 0)

        Public Reliable As Boolean

        <Nullable(2)>
        Public OptiFine As String

        Public OptiFineAsMod As Boolean

        Public ReleaseTime As DateTime = New DateTime(1970, 1, 1, 15, 0, 0)

        Public IsFool As Boolean

        Public IsSnapshot As Boolean
    End Class
End Namespace
