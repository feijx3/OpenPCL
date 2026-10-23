Imports System
Imports System.IO
Imports System.Runtime.CompilerServices
Imports MeloongCore

Namespace PCLCS
    <NullableContext(1)>
    <Nullable(0)>
    Public Module LaunchUtils
        Public Function ExtractPatch(patchFilePath As String, outputDirectory As String) As String
            Dim flag As Boolean
            Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(12, 2, flag)
            If flag Then
                logInterpolatedStringHandler.AppendLiteral("选定的资源 ")
                logInterpolatedStringHandler.AppendFormatted(patchFilePath)
                logInterpolatedStringHandler.AppendLiteral(" 输出路径：")
                logInterpolatedStringHandler.AppendFormatted(outputDirectory)
            End If
            Logger.Info(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\PCLCS\Launch\LaunchUtils.cs")
            Dim extractPatchLock As Object = LaunchUtils.ExtractPatchLock
            SyncLock extractPatchLock
                Try
                    FileUtils.ExtractResources(patchFilePath, Path.Combine(outputDirectory, PathUtils.GetLastPart(patchFilePath)), GetType(LaunchUtils))
                Catch ex As Exception
                    If Not FileUtils.Exists(outputDirectory) Then
                        Throw New FileNotFoundException("释放 " + patchFilePath + " 失败", ex)
                    End If
                    Logger.Warn(ex, patchFilePath + " 文件重新释放失败，将尝试更换文件名重新生成", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\PCLCS\Launch\LaunchUtils.cs")
                    patchFilePath = Path.Combine(outputDirectory, PathUtils.GetFileNameWithoutExtension(patchFilePath) + "2." + PathUtils.GetExtension(patchFilePath))
                    Try
                        FileUtils.ExtractResources(patchFilePath, Path.Combine(outputDirectory, PathUtils.GetLastPart(patchFilePath)), GetType(LaunchUtils))
                    Catch ex2 As Exception
                        Throw New FileNotFoundException("释放 " + patchFilePath + " 最终尝试失败", ex2)
                    End Try
                End Try
            End SyncLock
            Return Path.Combine(outputDirectory, PathUtils.GetLastPart(patchFilePath))
        End Function

        Private ExtractPatchLock As Object = New Object()
    End Module
End Namespace
