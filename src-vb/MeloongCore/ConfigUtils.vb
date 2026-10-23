Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Newtonsoft.Json.Linq

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Module ConfigUtils
        Public Sub SaveAll()
            Using enumerator As IEnumerator(Of WeakReference(Of JsonConfigProvider)) = JsonConfigProvider.allProviders.GetEnumerator()
                While enumerator.MoveNext()
                    Dim jsonConfigProvider As JsonConfigProvider
                    If enumerator.Current.TryGetTarget(jsonConfigProvider) Then
                        jsonConfigProvider.Save()
                    End If
                End While
            End Using
        End Sub

        Public Function GetSecret(key As String) As String
            Dim text As String = ConfigUtils.secret.Value.Value(Of String)(key)
            If text Is Nothing Then
                Throw New KeyNotFoundException("未找到密钥：" + key)
            End If
            Dim text2 As String = text
            Dim text3 As String
            Try
                text3 = CryptographyUtils.AesDecrypt(text2, "EncryptKey")
            Catch
                Throw New InvalidDataException("读取密钥失败（" + key + "），请检查文件内容与加密格式。")
            End Try
            Return text3
        End Function

        Private secret As Lazy(Of JObject) = New Lazy(Of JObject)(Function()
            Dim text As String = Path.Combine(Paths.AppData, "Meloong", "Dev", "secret.json")
            Dim jobject As JObject
            Try
                jobject = If(FileUtils.Exists(text), JObject.Parse(FileUtils.ReadAsString(text, Nothing, Nothing)), New JObject())
            Catch
                Throw New InvalidDataException("读取密钥文件失败（" + text + "），请检查文件内容与加密格式。")
            End Try
            Return jobject
        End Function)
    End Module
End Namespace
