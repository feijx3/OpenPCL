Imports System
Imports System.Runtime.CompilerServices
Imports MeloongCore.Extensions
Imports Microsoft.Win32

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Module RegistryUtils
        Public Function Read(folder As String, entry As String) As <TupleElementNames(New String() { "result", "exists" })> <Nullable(New Byte() { 0, 2 })> ValueTuple(Of Object, Boolean)
            Dim valueTuple As ValueTuple(Of Object, Boolean)
            Using registryKey As RegistryKey = RegistryUtils.GetRegistryKey(folder, False, False)
                Dim obj As Object = If((registryKey IsNot Nothing), registryKey.GetValue(entry), Nothing)
                If obj Is Nothing Then
                    valueTuple = New ValueTuple(Of Object, Boolean)(Nothing, False)
                Else
                    Dim text As String = TryCast(obj, String)
                    valueTuple = New ValueTuple(Of Object, Boolean)(If((text IsNot Nothing AndAlso text = "__NULL__"), Nothing, obj), True)
                End If
            End Using
            Return valueTuple
        End Function

        Public Function TryRead(folder As String, entry As String, <Nullable(2)> Optional defaultValue As Object = Nothing) As <Nullable(2)> Object
            Dim obj As Object
            Try
                Dim valueTuple As ValueTuple(Of Object, Boolean) = RegistryUtils.Read(folder, entry)
                Dim item As Object = valueTuple.Item1
                obj = If(valueTuple.Item2, item, defaultValue)
            Catch ex As Exception
                Logger.Warn(ex, String.Concat(New String() { "读取注册表值失败（", folder, "，", entry, "）" }), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\RegistryUtils.cs")
                obj = defaultValue
            End Try
            Return obj
        End Function

        Public Sub Write(folder As String, entry As String, <Nullable(2)> value As Object)
            Using registryKey As RegistryKey = RegistryUtils.GetRegistryKey(folder, False, True)
                registryKey.SetValue(entry, If(value, "__NULL__"))
            End Using
        End Sub

        Public Function Has(folder As String, entry As String) As Boolean
            Dim flag As Boolean
            Using registryKey As RegistryKey = RegistryUtils.GetRegistryKey(folder, False, False)
                If registryKey Is Nothing Then
                    flag = False
                Else
                    Dim obj As Object = New Object()
                    flag = registryKey.GetValue(entry, obj) IsNot obj
                End If
            End Using
            Return flag
        End Function

        Public Sub Delete(folder As String, entry As String)
            Using registryKey As RegistryKey = RegistryUtils.GetRegistryKey(folder, True, False)
                If registryKey IsNot Nothing Then
                    registryKey.DeleteValue(entry, False)
                End If
            End Using
        End Sub

        Private Function GetRegistryKey(fullPath As String, Optional writable As Boolean = False, Optional create As Boolean = False) As <Nullable(2)> RegistryKey
            Dim text As String = fullPath.BeforeFirst("\", False).ToUpperInvariant()
            Dim registryKey As RegistryKey
            If Not(text = "HKEY_CLASSES_ROOT") Then
                If Not(text = "HKEY_CURRENT_USER") Then
                    If Not(text = "HKEY_LOCAL_MACHINE") Then
                        If Not(text = "HKEY_USERS") Then
                            If Not(text = "HKEY_CURRENT_CONFIG") Then
                                Throw New ArgumentException("注册表路径有误（" + fullPath + "）", "fullPath")
                            End If
                            registryKey = Registry.CurrentConfig
                        Else
                            registryKey = Registry.Users
                        End If
                    Else
                        registryKey = Registry.LocalMachine
                    End If
                Else
                    registryKey = Registry.CurrentUser
                End If
            Else
                registryKey = Registry.ClassesRoot
            End If
            Using registryKey2 As RegistryKey = registryKey
                Dim text2 As String = fullPath.AfterFirst("\", False)
                registryKey = If(create, registryKey2.CreateSubKey(text2), registryKey2.OpenSubKey(text2, writable))
            End Using
            Return registryKey
        End Function

        Public Const NULL_STRING As String = "__NULL__"
    End Module
End Namespace
