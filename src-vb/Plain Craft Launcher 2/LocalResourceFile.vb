Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text.RegularExpressions
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json.Linq

Namespace PCL
    Public Class LocalResourceFile
        Public Sub New(FileInfo As FileInfo)
            Me._Display = Nothing
            Me._Description = Nothing
            Me._Version = Nothing
            Me.ChangelogUrls = New List(Of String)()
            Me.OnlineDataLoaded = False
            Me.File = FileInfo
        End Sub

        Public Sub New(FullName As String)
            Me.New(FileUtils.GetInfo(FullName))
        End Sub

        Public ReadOnly Property IsEnabled As Boolean
            Get
                Return New String() { ".jar", ".zip", ".litemod" }.Contains(Me.File.Extension.Lower())
            End Get
        End Property

        Public ReadOnly Property EnabledName As String
            Get
                Return Me.File.Name.Replace(".disabled", "").Replace(".old", "")
            End Get
        End Property

        Public ReadOnly Property EnabledFullName As String
            Get
                Return String.Format("{0}\{1}", Me.File.DirectoryName, Me.EnabledName)
            End Get
        End Property

        Public Event OnUpdate As LocalResourceFile.OnUpdateEventHandler

        Public Property Display As String
            Get
                If Me._Display Is Nothing Then
                    Dim projectVersion As ResourceVersion = Me.ProjectVersion
                    Me._Display = If((projectVersion IsNot Nothing), projectVersion.Display, Nothing)
                End If
                If Me._Display Is Nothing Then
                    Me._Display = PathUtils.GetFileNameWithoutExtension(Me.EnabledName)
                End If
                Return Me._Display
            End Get
            Set(value As String)
                If Me._Display Is Nothing AndAlso value IsNot Nothing AndAlso Not value.Contains("modname") AndAlso Operators.CompareString(value.Lower(), "name", False) <> 0 AndAlso value.Count() > 1 AndAlso Operators.CompareString(ModBase.Val(value).ToString(), value, False) <> 0 Then
                    Me._Display = value
                End If
            End Set
        End Property

        Public Property Description As String
            Get
                Return Me._Description
            End Get
            Set(value As String)
                If Me._Description Is Nothing AndAlso value IsNot Nothing AndAlso value.Count() > 2 Then
                    Me._Description = value.ToString().Trim(New Char() { vbLf })
                    If Me._Description.Lower().LastIndexOfAny(Conversions.ToCharArrayRankOne("qwertyuiopasdfghjklzxcvbnm0123456789")) = Me._Description.Count() - 1 Then
                        Dim ptr As __ByRef(Of String) = Me._Description
                        Me._Description = ptr + "."
                    End If
                End If
            End Set
        End Property

        Public Property Version As String
            Get
                If Me._Version Is Nothing Then
                    Dim projectVersion As ResourceVersion = Me.ProjectVersion
                    Me._Version = If((projectVersion IsNot Nothing), projectVersion.Version, Nothing)
                End If
                Return Me._Version
            End Get
            Set(value As String)
                If Me._Version Is Nothing OrElse Not Me._Version.RegexCheck("[0-9.\-]+", RegexOptions.None) Then
                    If value IsNot Nothing AndAlso value.ContainsIgnoreCase("version") Then
                        value = "version"
                    End If
                    Me._Version = value
                End If
            End Set
        End Property

        Public Property Project As ResourceProject
            Get
                Return Me._Project
            End Get
            Set(value As ResourceProject)
                Me._Project = value
                Dim onUpdateEvent As LocalResourceFile.OnUpdateEventHandler = Me.OnUpdateEvent
                If onUpdateEvent IsNot Nothing Then
                    onUpdateEvent(Me)
                End If
            End Set
        End Property

        Public Property UpdateFile As ResourceVersion
            Get
                Return Me._UpdateFile
            End Get
            Set(value As ResourceVersion)
                Me._UpdateFile = value
                Dim onUpdateEvent As LocalResourceFile.OnUpdateEventHandler = Me.OnUpdateEvent
                If onUpdateEvent IsNot Nothing Then
                    onUpdateEvent(Me)
                End If
            End Set
        End Property

        Public Function ToJson() As JObject
            Dim jobject As JObject = New JObject()
            If Me.Project IsNot Nothing Then
                jobject.Add("Project", Me.Project.ToJson())
            End If
            jobject.Add("ChangelogUrls", New JArray(Me.ChangelogUrls))
            jobject.Add("OnlineDataLoaded", Me.OnlineDataLoaded)
            If Me.ProjectVersion IsNot Nothing Then
                jobject.Add("Version", Me.ProjectVersion.ToCacheJson())
            End If
            If Me.UpdateFile IsNot Nothing Then
                jobject.Add("UpdateFile", Me.UpdateFile.ToCacheJson())
            End If
            Return jobject
        End Function

        Public Sub FromJson(Json As JObject)
            Me.OnlineDataLoaded = CBool(Json("OnlineDataLoaded"))
            If Json.ContainsKey("Project") Then
                Me.Project = New ResourceProject(CType(Json("Project"), JObject))
            End If
            If Json.ContainsKey("ChangelogUrls") Then
                Me.ChangelogUrls = Json("ChangelogUrls").ToObject(Of List(Of String))()
            End If
            If Json.ContainsKey("Version") Then
                Me.ProjectVersion = ResourceVersion.FromCacheJson(CType(Json("Version"), JObject))
            End If
            If Json.ContainsKey("UpdateFile") Then
                Me.UpdateFile = ResourceVersion.FromCacheJson(CType(Json("UpdateFile"), JObject))
            End If
        End Sub

        Public ReadOnly Property HasUpdate As Boolean
            Get
                Return Not Settings.[Get](Of Boolean)("UiHiddenFunctionModUpdate", Nothing) AndAlso Not Settings.[Get](Of Boolean)("VersionAdvanceDisableModUpdate", PageInstanceLeft.Instance) AndAlso Me.UpdateFile IsNot Nothing
            End Get
        End Property

        Public ReadOnly Property CurseForgeHash As UInteger
            Get
                ' The following expression was wrapped in a checked-statement
                If Me._CurseForgeHash Is Nothing Then
                    Dim text As String = Conversions.ToString(String.Format("{0}-{1}-{2}-C", Me.EnabledFullName, Me.File.LastWriteTime.ToLongTimeString(), Me.File.Length).GetStableHashCode())
                    Dim text2 As String = ModBase.ReadIni(ModBase.PathTemp + "Cache\ModHash.ini", text, "")
                    If Operators.CompareString(text2, "", False) <> 0 AndAlso text2.RegexCheck("^\d+$", RegexOptions.None) Then
                        Me._CurseForgeHash = New UInteger?(Conversions.ToUInteger(text2))
                        Return Me._CurseForgeHash.Value
                    End If
                    Dim list As List(Of Byte) = New List(Of Byte)()
                    For Each b As Byte In FileUtils.ReadAsBytes(Me.File.FullName, Nothing)
                        If b <> 9 AndAlso b <> 10 AndAlso b <> 13 AndAlso b <> 32 Then
                            list.Add(b)
                        End If
                    Next
                    Dim count As Integer = list.Count
                    Dim num As UInteger = CUInt((1 Xor count))
                    Dim num2 As Integer = count - 4
                    Dim j As Integer
                    j = 0
                    While j <= num2
                        Dim num3 As UInteger = CUInt((CInt(list(j)) Or (CInt(list(j + 1)) << 8) Or (CInt(list(j + 2)) << 16) Or (CInt(list(j + 3)) << 24)))
                        num3 = CUInt(((CULng(num3) * 1540483477UL) And CULng((-1))))
                        num3 = num3 Xor num3 >> 24
                        num3 = CUInt(((CULng(num3) * 1540483477UL) And CULng((-1))))
                        num = CUInt(((CULng(num) * 1540483477UL) And CULng((-1))))
                        num = num Xor num3
                        j += 4
                    End While
                    Select Case count - j
                        Case 1
                            num = num Xor CUInt(list(j))
                            num = CUInt(((CULng(num) * 1540483477UL) And CULng((-1))))
                        Case 2
                            num = num Xor CUInt((CInt(list(j)) Or (CInt(list(j + 1)) << 8)))
                            num = CUInt(((CULng(num) * 1540483477UL) And CULng((-1))))
                        Case 3
                            num = num Xor CUInt((CInt(list(j)) Or (CInt(list(j + 1)) << 8)))
                            num = num Xor CUInt((CUInt(list(j + 2)) << 16))
                            num = CUInt(((CULng(num) * 1540483477UL) And CULng((-1))))
                    End Select
                    num = num Xor num >> 13
                    num = CUInt(((CULng(num) * 1540483477UL) And CULng((-1))))
                    num = num Xor num >> 15
                    Me._CurseForgeHash = New UInteger?(num)
                    ModBase.WriteIni(ModBase.PathTemp + "Cache\ModHash.ini", text, num.ToString())
                End If
                Return Me._CurseForgeHash.Value
            End Get
        End Property

        Public ReadOnly Property ModrinthHash As String
            Get
                If Me._ModrinthHash Is Nothing Then
                    Dim text As String = Conversions.ToString(String.Format("{0}-{1}-{2}-M", Me.EnabledFullName, Me.File.LastWriteTime.ToLongTimeString(), Me.File.Length).GetStableHashCode())
                    Dim text2 As String = ModBase.ReadIni(ModBase.PathTemp + "Cache\ModHash.ini", text, "")
                    If Operators.CompareString(text2, "", False) <> 0 Then
                        Me._ModrinthHash = text2
                        Return Me._ModrinthHash
                    End If
                    Me._ModrinthHash = CryptographyUtils.ComputeFileHash(Me.File.FullName, CryptographyUtils.HashMethod.Sha1)
                    ModBase.WriteIni(ModBase.PathTemp + "Cache\ModHash.ini", text, Me._ModrinthHash)
                End If
                Return Me._ModrinthHash
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.File.FullName
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            Dim localResourceFile As LocalResourceFile = TryCast(obj, LocalResourceFile)
            Return localResourceFile IsNot Nothing AndAlso PathUtils.IsSame(Me.File.FullName, localResourceFile.File.FullName)
        End Function

        Public File As FileInfo

        Private _Display As String

        Private _Description As String

        Public _Version As String

        Private _Project As ResourceProject

        Public ProjectVersion As ResourceVersion

        Private _UpdateFile As ResourceVersion

        Public ChangelogUrls As List(Of String)

        Public OnlineDataLoaded As Boolean

        Private _CurseForgeHash As UInteger?

        Private _ModrinthHash As String

        Public Delegate Sub OnUpdateEventHandler(sender As LocalResourceFile)
    End Class
End Namespace
