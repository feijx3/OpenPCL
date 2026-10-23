Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports MeloongCore.Extensions
Imports Microsoft.Win32
Imports Ookii.Dialogs.Wpf

Namespace MeloongCore.Wpf
    <NullableContext(1)>
    <Nullable(0)>
    Public Module Dialogs
        Public Function SaveFile(title As String, Optional defaultFileName As String = "", <Nullable(2)> Optional defaultDirectory As String = Nothing, <TupleElementNames(New String() { "Extension", "Display" })> <Nullable(New Byte() { 2, 0, 1, 1 })> Optional filter As IEnumerable(Of ValueTuple(Of String, String)) = Nothing) As <Nullable(2)> String
            Dim list As List(Of ValueTuple(Of String, String)) = If((filter IsNot Nothing), filter.ToList(), Nothing)
            Dim saveFileDialog As SaveFileDialog = New SaveFileDialog() With { .AddExtension = True, .Title = title, .FileName = defaultFileName }
            If Not String.IsNullOrEmpty(defaultDirectory) AndAlso DirectoryUtils.Exists(defaultDirectory) Then
                saveFileDialog.InitialDirectory = PathUtils.ToShortPath(defaultDirectory)
            End If
            If list IsNot Nothing Then
                Dim fileDialog As FileDialog = saveFileDialog
                Dim enumerable As IEnumerable(Of ValueTuple(Of String, String)) = list
                Dim <>9__0_ As Func(Of ValueTuple(Of String, String), String) = Dialogs.<>c.<>9__0_0
                Dim func As Func(Of ValueTuple(Of String, String), String) = <>9__0_
                If <>9__0_ Is Nothing Then
                    Dim func2 As Func(Of ValueTuple(Of String, String), String) = Function(<TupleElementNames(New String() { "Extension", "Display" })> f As ValueTuple(Of String, String)) String.Concat(New String() { f.Item2, "(*.", f.Item1, ")|*.", f.Item1 })
                    func = func2
                    Dialogs.<>c.<>9__0_0 = func2
                End If
                fileDialog.Filter = enumerable.[Select](func).Join("|")
            End If
            Dim flag As Boolean
            Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(44, 4, flag)
            If flag Then
                logInterpolatedStringHandler.AppendLiteral("保存弹窗：")
                logInterpolatedStringHandler.AppendFormatted(saveFileDialog.Title)
                logInterpolatedStringHandler.AppendLiteral("（FileName=")
                logInterpolatedStringHandler.AppendFormatted(saveFileDialog.FileName)
                logInterpolatedStringHandler.AppendLiteral(", InitialDirectory=")
                logInterpolatedStringHandler.AppendFormatted(saveFileDialog.InitialDirectory)
                logInterpolatedStringHandler.AppendLiteral(", Filter=")
                logInterpolatedStringHandler.AppendFormatted(saveFileDialog.Filter)
                logInterpolatedStringHandler.AppendLiteral("）")
            End If
            Logger.Info(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Wpf\General\Dialogs.cs")
            If Not saveFileDialog.ShowDialog().GetValueOrDefault() Then
                Return Nothing
            End If
            Dim result As String = saveFileDialog.FileName
            Dim logInterpolatedStringHandler2 As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(7, 1, flag)
            If flag Then
                logInterpolatedStringHandler2.AppendLiteral("保存弹窗返回：")
                logInterpolatedStringHandler2.AppendFormatted(result)
            End If
            Logger.Info(logInterpolatedStringHandler2, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Wpf\General\Dialogs.cs")
            If Not result.Contains(Path.DirectorySeparatorChar) Then
                Return Nothing
            End If
            If list IsNot Nothing AndAlso Not String.IsNullOrEmpty(result) AndAlso Not list.Any(Function(<TupleElementNames(New String() { "Extension", "Display" })> f As ValueTuple(Of String, String)) result.EndsWithF("." + f.Item1, True)) Then
                result = result + "." + list(saveFileDialog.FilterIndex - 1).Item1
                Dim logInterpolatedStringHandler3 As Logger.LogInterpolatedStringHandler(Of Logger.WarnLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.WarnLogLevel)(24, 1, flag)
                If flag Then
                    logInterpolatedStringHandler3.AppendLiteral("选择文件的返回无扩展名，将会手动添加，修改后为：")
                    logInterpolatedStringHandler3.AppendFormatted(result)
                End If
                Logger.Warn(logInterpolatedStringHandler3, LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\MeloongCore\Wpf\General\Dialogs.cs")
            End If
            Return result
        End Function

        Public Function SelectFile(title As String, multiselect As Boolean, <Nullable(2)> Optional defaultDirectory As String = Nothing, <TupleElementNames(New String() { "Extensions", "Display" })> <Nullable(New Byte() { 0, 1, 0, 1, 1, 1, 1 })> Optional filter As OneOf(Of IEnumerable(Of ValueTuple(Of String(), String)), String)? = Nothing) As List(Of String)
            Dim openFileDialog As OpenFileDialog = New OpenFileDialog() With { .AddExtension = True, .CheckFileExists = True, .Multiselect = multiselect, .Title = title, .ValidateNames = True }
            If Not String.IsNullOrEmpty(defaultDirectory) AndAlso DirectoryUtils.Exists(defaultDirectory) Then
                openFileDialog.InitialDirectory = PathUtils.ToShortPath(defaultDirectory)
            End If
            If filter IsNot Nothing Then
                If filter.Value.[Is](Of String)() Then
                    openFileDialog.Filter = filter.Value.[As](Of String)()
                Else
                    Dim fileDialog As FileDialog = openFileDialog
                    Dim enumerable As IEnumerable(Of ValueTuple(Of String(), String)) = filter.Value.[As](Of IEnumerable(Of ValueTuple(Of String(), String)))()
                    Dim <>9__1_ As Func(Of ValueTuple(Of String(), String), String) = Dialogs.<>c.<>9__1_0
                    Dim func As Func(Of ValueTuple(Of String(), String), String) = <>9__1_
                    If <>9__1_ Is Nothing Then
                        Dim func2 As Func(Of ValueTuple(Of String(), String), String) = Function(<TupleElementNames(New String() { "Extensions", "Display" })> f As ValueTuple(Of String(), String))
                            Dim text As String = ";"
                            Dim item As IEnumerable(Of String) = f.Item1
                            Dim <>9__1_3 As Func(Of String, String) = Dialogs.<>c.<>9__1_2
                            Dim func5 As Func(Of String, String) = <>9__1_3
                            If <>9__1_3 Is Nothing Then
                                Dim func6 As Func(Of String, String) = Function(e As String) "*." + e
                                func5 = func6
                                Dialogs.<>c.<>9__1_2 = func6
                            End If
                            Dim text2 As String = String.Join(text, item.[Select](func5))
                            Return String.Concat(New String() { f.Item2, "(", text2, ")|", text2 })
                        End Function
                        func = func2
                        Dialogs.<>c.<>9__1_0 = func2
                    End If
                    fileDialog.Filter = enumerable.[Select](func).Join("|")
                End If
            End If
            Dim flag As Boolean
            Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(49, 4, flag)
            If flag Then
                logInterpolatedStringHandler.AppendLiteral("选择文件弹窗：")
                logInterpolatedStringHandler.AppendFormatted(openFileDialog.Title)
                logInterpolatedStringHandler.AppendLiteral("（Multiselect=")
                logInterpolatedStringHandler.AppendFormatted(Of Boolean)(openFileDialog.Multiselect)
                logInterpolatedStringHandler.AppendLiteral(", InitialDirectory=")
                logInterpolatedStringHandler.AppendFormatted(openFileDialog.InitialDirectory)
                logInterpolatedStringHandler.AppendLiteral(", Filter=")
                logInterpolatedStringHandler.AppendFormatted(openFileDialog.Filter)
                logInterpolatedStringHandler.AppendLiteral("）")
            End If
            Logger.Info(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Wpf\General\Dialogs.cs")
            If Not openFileDialog.ShowDialog().GetValueOrDefault() Then
                Return New List(Of String)()
            End If
            Dim fileNames As IEnumerable(Of String) = openFileDialog.FileNames
            Dim <>9__1_2 As Func(Of String, Boolean) = Dialogs.<>c.<>9__1_1
            Dim func3 As Func(Of String, Boolean) = <>9__1_2
            If <>9__1_2 Is Nothing Then
                Dim func4 As Func(Of String, Boolean) = Function(f As String) f.Contains(Path.DirectorySeparatorChar)
                func3 = func4
                Dialogs.<>c.<>9__1_1 = func4
            End If
            Dim list As List(Of String) = fileNames.Where(func3).ToList()
            Dim logInterpolatedStringHandler2 As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(9, 1, flag)
            If flag Then
                logInterpolatedStringHandler2.AppendLiteral("选择文件弹窗返回：")
                logInterpolatedStringHandler2.AppendFormatted(list.Join(","))
            End If
            Logger.Info(logInterpolatedStringHandler2, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Wpf\General\Dialogs.cs")
            Return list
        End Function

        Public Function SelectFolder(title As String, multiselect As Boolean, <Nullable(2)> Optional defaultDirectory As String = Nothing) As List(Of String)
            Dim vistaFolderBrowserDialog As VistaFolderBrowserDialog = New VistaFolderBrowserDialog() With { .ShowNewFolderButton = True, .Description = title, .Multiselect = multiselect, .UseDescriptionForTitle = True }
            If Not String.IsNullOrEmpty(defaultDirectory) AndAlso DirectoryUtils.Exists(defaultDirectory) Then
                vistaFolderBrowserDialog.SelectedPath = defaultDirectory
            End If
            Dim flag As Boolean
            Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(37, 3, flag)
            If flag Then
                logInterpolatedStringHandler.AppendLiteral("选择文件夹弹窗：")
                logInterpolatedStringHandler.AppendFormatted(vistaFolderBrowserDialog.Description)
                logInterpolatedStringHandler.AppendLiteral("（Multiselect=")
                logInterpolatedStringHandler.AppendFormatted(Of Boolean)(vistaFolderBrowserDialog.Multiselect)
                logInterpolatedStringHandler.AppendLiteral(", SelectedPath=")
                logInterpolatedStringHandler.AppendFormatted(vistaFolderBrowserDialog.SelectedPath)
                logInterpolatedStringHandler.AppendLiteral("）")
            End If
            Logger.Info(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Wpf\General\Dialogs.cs")
            If Not vistaFolderBrowserDialog.ShowDialog().GetValueOrDefault() Then
                Return New List(Of String)()
            End If
            Dim selectedPaths As IEnumerable(Of String) = vistaFolderBrowserDialog.SelectedPaths
            Dim <>9__2_ As Func(Of String, Boolean) = Dialogs.<>c.<>9__2_0
            Dim func As Func(Of String, Boolean) = <>9__2_
            If <>9__2_ Is Nothing Then
                Dim func2 As Func(Of String, Boolean) = Function(f As String) f.Contains(Path.DirectorySeparatorChar)
                func = func2
                Dialogs.<>c.<>9__2_0 = func2
            End If
            Dim enumerable As IEnumerable(Of String) = selectedPaths.Where(func)
            Dim <0>__AddSlashSuffix As Func(Of String, String) = Dialogs.<>O.<0>__AddSlashSuffix
            Dim func3 As Func(Of String, String) = <0>__AddSlashSuffix
            If <0>__AddSlashSuffix Is Nothing Then
                Dim func4 As Func(Of String, String) = New Func(Of String, String)(PathUtils.AddSlashSuffix)
                func3 = func4
                Dialogs.<>O.<0>__AddSlashSuffix = func4
            End If
            Dim list As List(Of String) = enumerable.[Select](func3).ToList()
            Dim logInterpolatedStringHandler2 As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(10, 1, flag)
            If flag Then
                logInterpolatedStringHandler2.AppendLiteral("选择文件夹弹窗返回：")
                logInterpolatedStringHandler2.AppendFormatted(list.Join(","))
            End If
            Logger.Info(logInterpolatedStringHandler2, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Wpf\General\Dialogs.cs")
            Return list
        End Function

        <CompilerGenerated()>
        Private NotInheritable Class <>O
            <Nullable(0)>
            Public Shared <0>__AddSlashSuffix As Func(Of String, String)
        End Class
    End Module
End Namespace
