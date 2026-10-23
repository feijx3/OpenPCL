Imports System
Imports System.Collections.Concurrent
Imports System.Runtime.CompilerServices
Imports System.Security.Cryptography
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Class JsonConfigProvider
        Implements IConfigProvider

        Public Sub New(filePath As String)
            Me.filePath = filePath
            JsonConfigProvider.allProviders.Add(New WeakReference(Of JsonConfigProvider)(Me))
            Me.saveWorker = AddressOf Me.Save
            Me.InitJson()
        End Sub

        Private Sub InitJson()
            Me.json = New Lazy(Of JObject)(Function()
                Dim jobject As JObject
                Try
                    jobject = If(FileUtils.Exists(Me.filePath), (If(CType(FileUtils.ReadAsJson(Me.filePath, Nothing, Nothing), JObject), New JObject())), New JObject())
                Catch ex As Exception
                    Logger.[Error](ex, "读取配置文件失败（" + Me.filePath + "）", LogBehavior.Alert, "E:\Projects\PCL\本体代码\MeloongCore\Shared\General\Config.cs")
                    jobject = New JObject()
                End Try
                Return jobject
            End Function)
        End Sub

        <NullableContext(2)>
        Public Sub [Set](Of T)(<Nullable(1)> key As String, value As T, encrypted As Boolean) Implements MeloongCore.IConfigProvider.[Set]
            Me.cache(key) = value
            SyncLock Me
                If value Is Nothing Then
                    Me.json.Value(key) = JValue.CreateNull()
                    Dim flag2 As Boolean
                    Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(15, 2, flag2)
                    If flag2 Then
                        logInterpolatedStringHandler.AppendLiteral("配置已修改：")
                        logInterpolatedStringHandler.AppendFormatted(key)
                        logInterpolatedStringHandler.AppendLiteral(" = null（")
                        logInterpolatedStringHandler.AppendFormatted(Me.filePath)
                        logInterpolatedStringHandler.AppendLiteral("）")
                    End If
                    Logger.Trace(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\General\Config.cs")
                Else
                    If encrypted Then
                        Throw New NotImplementedException("加密需要改为使用识别码，现在尚未实现。")
                    End If
                    Dim jtoken As JToken = JToken.FromObject(value)
                    Me.json.Value(key) = jtoken
                    Dim flag2 As Boolean
                    Dim logInterpolatedStringHandler2 As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(11, 3, flag2)
                    If flag2 Then
                        logInterpolatedStringHandler2.AppendLiteral("配置已修改：")
                        logInterpolatedStringHandler2.AppendFormatted(key)
                        logInterpolatedStringHandler2.AppendLiteral(" = ")
                        logInterpolatedStringHandler2.AppendFormatted(jtoken.ToString(0, Array.Empty(Of JsonConverter)()))
                        logInterpolatedStringHandler2.AppendLiteral("（")
                        logInterpolatedStringHandler2.AppendFormatted(Me.filePath)
                        logInterpolatedStringHandler2.AppendLiteral("）")
                    End If
                    Logger.Trace(logInterpolatedStringHandler2, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\General\Config.cs")
                End If
            End SyncLock
            Me.MakeDirty()
        End Sub

        <NullableContext(2)>
        Public Function Read(Of T)(<Nullable(1)> key As String, defaultValue As T, encrypted As Boolean) As T Implements MeloongCore.IConfigProvider.Read
            Dim obj As Object
            If Me.cache.TryGetValue(key, obj) Then
                Return CType(CObj(obj), T)
            End If
            Dim jtoken As JToken
            SyncLock Me
                If Not Me.json.Value.TryGetValue(key, jtoken) Then
                    Return defaultValue
                End If
                jtoken = jtoken.DeepClone()
            End SyncLock
            Dim t2 As T
            Try
                Dim type As JTokenType = jtoken.Type
                Dim flag As Boolean = type - 10 <= 1
                Dim t As T
                If flag Then
                    t = Nothing
                ElseIf encrypted Then
                    Dim text As String = CryptographyUtils.AesDecrypt(Extensions.Value(Of String)(jtoken), "EncryptKey")
                    t = If((GetType(T) = GetType(String)), CType(CObj(text), T), JsonConvert.DeserializeObject(Of T)(text))
                Else
                    t = jtoken.ToObject(Of T)()
                End If
                Me.cache(key) = t
                t2 = t
            Catch ex As CryptographicException
                Logger.[Error](ex, "解密配置失败，该配置将被重置（" + key + "）", LogBehavior.Alert, "E:\Projects\PCL\本体代码\MeloongCore\Shared\General\Config.cs")
                Me.Remove(key)
                t2 = defaultValue
            End Try
            Return t2
        End Function

        Public Function HasValue(key As String) As Boolean Implements MeloongCore.IConfigProvider.HasValue
            Dim flag2 As Boolean
            SyncLock Me
                flag2 = Me.json.Value.ContainsKey(key)
            End SyncLock
            Return flag2
        End Function

        Public Sub Remove(key As String) Implements MeloongCore.IConfigProvider.Remove
            SyncLock Me
                Me.json.Value.Remove(key)
            End SyncLock
            Dim obj As Object
            Me.cache.TryRemove(key, obj)
            Me.MakeDirty()
        End Sub

        Public Sub DiscardCache() Implements MeloongCore.IConfigProvider.DiscardCache
            SyncLock Me
                Me.InitJson()
            End SyncLock
            Me.cache.Clear()
        End Sub

        Public Sub MakeDirty()
            SyncLock Me
                Me.isDirty = True
            End SyncLock
            Me.saveWorker.Start()
        End Sub

        Public Sub Save()
            SyncLock Me
                If Me.isDirty Then
                    FileUtils.WriteAtomic(Me.filePath, Me.json.Value.ToString(1, Array.Empty(Of JsonConverter)()), Nothing)
                    Me.isDirty = False
                End If
            End SyncLock
        End Sub

        Public Shared allProviders As ConcurrentBag(Of WeakReference(Of JsonConfigProvider)) = New ConcurrentBag(Of WeakReference(Of JsonConfigProvider))()

        Private filePath As String

        Private json As Lazy(Of JObject)

        <Nullable(New Byte() { 1, 1, 2 })>
        Private cache As ConcurrentDictionary(Of String, Object) = New ConcurrentDictionary(Of String, Object)()

        Private saveWorker As RateLimitedWorker

        Private isDirty As Boolean
    End Class
End Namespace
