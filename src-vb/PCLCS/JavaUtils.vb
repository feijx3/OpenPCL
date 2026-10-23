Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Threading.Tasks
Imports MeloongCore
Imports MeloongCore.Extensions

Namespace PCLCS
    <NullableContext(1)>
    <Nullable(0)>
    Public Module JavaUtils
        Public Async Function CheckAllAsync(javas As IEnumerable(Of Java), Optional cancellationToken As CancellationToken = Nothing, <Nullable(2)> Optional progress As ProgressProvider = Nothing) As Task(Of List(Of Java))
            Dim CS$<>8__locals1 As JavaUtils.<>c__DisplayClass0_0 = New JavaUtils.<>c__DisplayClass0_0()
            CS$<>8__locals1.cancellationToken = cancellationToken
            Dim <>9__0_ As Func(Of Java, Boolean) = JavaUtils.<>c.<>9__0_0
            Dim func As Func(Of Java, Boolean) = <>9__0_
            If <>9__0_ Is Nothing Then
                Dim func2 As Func(Of Java, Boolean) = Function(j As Java) j.available Is Nothing
                func = func2
                JavaUtils.<>c.<>9__0_0 = func2
            End If
            Await TaskUtils.WhenAll(Of Boolean)(javas.Where(func).[Select](Function(java As Java)
                Dim <<CheckAllAsync>b__1>d As JavaUtils.<>c__DisplayClass0_0.<<CheckAllAsync>b__1>d
                <<CheckAllAsync>b__1>d.<>t__builder = AsyncTaskMethodBuilder(Of Boolean).Create()
                <<CheckAllAsync>b__1>d.<>4__this = CS$<>8__locals1
                <<CheckAllAsync>b__1>d.java = java
                <<CheckAllAsync>b__1>d.<>1__state = -1
                <<CheckAllAsync>b__1>d.<>t__builder.Start(Of JavaUtils.<>c__DisplayClass0_0.<<CheckAllAsync>b__1>d)(<<CheckAllAsync>b__1>d)
                Return <<CheckAllAsync>b__1>d.<>t__builder.Task
            End Function), progress).NoCapture()
            Dim <>9__0_2 As Func(Of Java, Boolean) = JavaUtils.<>c.<>9__0_2
            Dim func3 As Func(Of Java, Boolean) = <>9__0_2
            If <>9__0_2 Is Nothing Then
                Dim func4 As Func(Of Java, Boolean) = Function(j As Java) j.available.GetValueOrDefault()
                func3 = func4
                JavaUtils.<>c.<>9__0_2 = func4
            End If
            Return javas.Where(func3).ToList()
        End Function

        Public Async Function SortAsync(javas As List(Of Java), Optional cancellationToken As CancellationToken = Nothing, <Nullable(2)> Optional progress As ProgressProvider = Nothing) As Task(Of List(Of Java))
            Dim list As List(Of Java) = Await JavaUtils.CheckAllAsync(javas, cancellationToken, progress).NoCapture()
            javas = list
            Dim list2 As List(Of Java) = javas
            Dim <>9__1_ As Comparison(Of Java) = JavaUtils.<>c.<>9__1_0
            Dim comparison As Comparison(Of Java) = <>9__1_
            If <>9__1_ Is Nothing Then
                Dim comparison2 As Comparison(Of Java) = Function(left As Java, right As Java)
                    Dim flag As Boolean = JavaUtils.CandidateFolders.Value.Any(Function(folder As String) PathUtils.IsParentOf(folder, left.Folder))
                    Dim flag2 As Boolean = JavaUtils.CandidateFolders.Value.Any(Function(folder As String) PathUtils.IsParentOf(folder, right.Folder))
                    If flag = flag2 Then
                        Dim num As Integer = If((left.Version Is Nothing), Integer.MaxValue, Math.Abs(left.Version.Major - 21))
                        Dim num2 As Integer = If((right.Version Is Nothing), Integer.MaxValue, Math.Abs(right.Version.Major - 21))
                        Return num.CompareTo(num2)
                    End If
                    If Not flag Then
                        Return 1
                    End If
                    Return -1
                End Function
                comparison = comparison2
                JavaUtils.<>c.<>9__1_0 = comparison2
            End If
            list2.Sort(comparison)
            Return javas
        End Function

        Public Async Function RefreshListAsync(Optional c As CancellationToken = Nothing, <Nullable(2)> Optional p As ProgressProvider = Nothing) As Task
            Dim list As List(Of Java) = Await JavaUtils.SearchFoldersAsync(True, JavaUtils.CandidateFolders.Value, c, If((p IsNot Nothing), p.SplitTo(0.5), Nothing)).NoCapture()
            Dim javaList As List(Of Java) = list
            Dim oldJavaList As ConcurrentList(Of Java) = Configs.JavaList.[Get]()
            list = Await JavaUtils.CheckAllAsync(oldJavaList, c, If((p IsNot Nothing), p.SplitTo(0.9), Nothing)).NoCapture()
            oldJavaList = list
            javaList.RemoveAll(oldJavaList)
            list = Await JavaUtils.SortAsync(javaList, c, Nothing).NoCapture()
            javaList = list
            javaList.AddRange(oldJavaList)
            javaList = javaList.Distinct().ToList()
            Dim collection As ICollection(Of Java) = javaList
            Dim <>9__3_ As Func(Of Java, Boolean) = JavaUtils.<>c.<>9__3_0
            Dim func As Func(Of Java, Boolean) = <>9__3_
            If <>9__3_ Is Nothing Then
                Dim func2 As Func(Of Java, Boolean) = Function(j As Java) Configs.JavaRemovedList.[Get]().Contains(j.Folder, StringComparer.OrdinalIgnoreCase)
                func = func2
                JavaUtils.<>c.<>9__3_0 = func2
            End If
            collection.RemoveIf(func)
            Dim concurrentList As ConcurrentList(Of Java) = New ConcurrentList(Of Java)()
            For Each java As Java In javaList
                concurrentList.Add(java)
            Next
            Configs.JavaList.[Set](concurrentList)
            Dim flag As Boolean
            Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(20, 1, flag)
            If flag Then
                logInterpolatedStringHandler.AppendLiteral("Java 搜索完成，发现 ")
                logInterpolatedStringHandler.AppendFormatted(Of Integer)(javaList.Count)
                logInterpolatedStringHandler.AppendLiteral(" 个 Java")
            End If
            Logger.Info(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\PCLCS\Java.cs")
        End Function

        Public Async Function SearchFoldersAsync(includeSubDirectories As Boolean, folders As IEnumerable(Of String), Optional cancellationToken As CancellationToken = Nothing, <Nullable(2)> Optional progress As ProgressProvider = Nothing) As Task(Of List(Of Java))
            Dim CS$<>8__locals1 As JavaUtils.<>c__DisplayClass4_0 = New JavaUtils.<>c__DisplayClass4_0()
            CS$<>8__locals1.includeSubDirectories = includeSubDirectories
            CS$<>8__locals1.cancellationToken = cancellationToken
            CS$<>8__locals1.results = New ConcurrentBag(Of ValueTuple(Of String, Boolean, Boolean))()
            Await TaskUtils.ForEachAsync(Of String)(folders.Distinct(StringComparer.OrdinalIgnoreCase), 4, Function(folder As String, <Nullable(2)> cp As ProgressProvider)
                Dim <<SearchFoldersAsync>b__0>d As JavaUtils.<>c__DisplayClass4_0.<<SearchFoldersAsync>b__0>d
                <<SearchFoldersAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create()
                <<SearchFoldersAsync>b__0>d.<>4__this = CS$<>8__locals1
                <<SearchFoldersAsync>b__0>d.folder = folder
                <<SearchFoldersAsync>b__0>d.<>1__state = -1
                <<SearchFoldersAsync>b__0>d.<>t__builder.Start(Of JavaUtils.<>c__DisplayClass4_0.<<SearchFoldersAsync>b__0>d)(<<SearchFoldersAsync>b__0>d)
                Return <<SearchFoldersAsync>b__0>d.<>t__builder.Task
            End Function, CS$<>8__locals1.cancellationToken, If((progress IsNot Nothing), progress.SplitTo(0.2), Nothing)).NoCapture()
            Dim list As List(Of Java)
            If Not CS$<>8__locals1.results.Any() Then
                list = New List(Of Java)()
            Else
                Dim results As IEnumerable(Of ValueTuple(Of String, Boolean, Boolean)) = CS$<>8__locals1.results
                Dim <>9__4_ As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = JavaUtils.<>c.<>9__4_1
                Dim func As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = <>9__4_
                If <>9__4_ Is Nothing Then
                    Dim func2 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = Function(<TupleElementNames(New String() { "Folder", "HasReparsePoint", "IsSpecialPath" })> r As ValueTuple(Of String, Boolean, Boolean)) r.Item2
                    func = func2
                    JavaUtils.<>c.<>9__4_1 = func2
                End If
                Dim flag As Boolean
                If results.All(func) Then
                    Logger.Warn("找到的所有 Java 都包含重解析点，保留全部结果", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\PCLCS\Java.cs")
                Else
                    Dim results2 As IEnumerable(Of ValueTuple(Of String, Boolean, Boolean)) = CS$<>8__locals1.results
                    Dim <>9__4_2 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = JavaUtils.<>c.<>9__4_2
                    Dim func3 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = <>9__4_2
                    If <>9__4_2 Is Nothing Then
                        Dim func4 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = Function(<TupleElementNames(New String() { "Folder", "HasReparsePoint", "IsSpecialPath" })> r As ValueTuple(Of String, Boolean, Boolean)) r.Item2
                        func3 = func4
                        JavaUtils.<>c.<>9__4_2 = func4
                    End If
                    If results2.Any(func3) Then
                        Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.WarnLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.WarnLogLevel)(28, 1, flag)
                        If flag Then
                            logInterpolatedStringHandler.AppendLiteral("找到的以下 Java 包含重解析点，将它们从列表中排除：")
                            Dim results3 As IEnumerable(Of ValueTuple(Of String, Boolean, Boolean)) = CS$<>8__locals1.results
                            Dim <>9__4_3 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = JavaUtils.<>c.<>9__4_9
                            Dim func5 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = <>9__4_3
                            If <>9__4_3 Is Nothing Then
                                Dim func6 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = Function(<TupleElementNames(New String() { "Folder", "HasReparsePoint", "IsSpecialPath" })> r As ValueTuple(Of String, Boolean, Boolean)) r.Item2
                                func5 = func6
                                JavaUtils.<>c.<>9__4_9 = func6
                            End If
                            Dim enumerable As IEnumerable(Of ValueTuple(Of String, Boolean, Boolean)) = results3.Where(func5)
                            Dim <>9__4_4 As Func(Of ValueTuple(Of String, Boolean, Boolean), String) = JavaUtils.<>c.<>9__4_10
                            Dim func7 As Func(Of ValueTuple(Of String, Boolean, Boolean), String) = <>9__4_4
                            If <>9__4_4 Is Nothing Then
                                Dim func8 As Func(Of ValueTuple(Of String, Boolean, Boolean), String) = Function(<TupleElementNames(New String() { "Folder", "HasReparsePoint", "IsSpecialPath" })> r As ValueTuple(Of String, Boolean, Boolean)) r.Item1
                                func7 = func8
                                JavaUtils.<>c.<>9__4_10 = func8
                            End If
                            logInterpolatedStringHandler.AppendFormatted(enumerable.[Select](func7).Join("、"c))
                        End If
                        Logger.Warn(logInterpolatedStringHandler, LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\PCLCS\Java.cs")
                        Dim concurrentBag As ConcurrentBag(Of ValueTuple(Of String, Boolean, Boolean)) = New ConcurrentBag(Of ValueTuple(Of String, Boolean, Boolean))()
                        Dim results4 As IEnumerable(Of ValueTuple(Of String, Boolean, Boolean)) = CS$<>8__locals1.results
                        Dim <>9__4_5 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = JavaUtils.<>c.<>9__4_11
                        Dim func9 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = <>9__4_5
                        If <>9__4_5 Is Nothing Then
                            Dim func10 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = Function(<TupleElementNames(New String() { "Folder", "HasReparsePoint", "IsSpecialPath" })> r As ValueTuple(Of String, Boolean, Boolean)) Not r.Item2
                            func9 = func10
                            JavaUtils.<>c.<>9__4_11 = func10
                        End If
                        For Each valueTuple As ValueTuple(Of String, Boolean, Boolean) In results4.Where(func9)
                            concurrentBag.Add(valueTuple)
                        Next
                        CS$<>8__locals1.results = concurrentBag
                    End If
                End If
                Dim results5 As IEnumerable(Of ValueTuple(Of String, Boolean, Boolean)) = CS$<>8__locals1.results
                Dim <>9__4_6 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = JavaUtils.<>c.<>9__4_3
                Dim func11 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = <>9__4_6
                If <>9__4_6 Is Nothing Then
                    Dim func12 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = Function(<TupleElementNames(New String() { "Folder", "HasReparsePoint", "IsSpecialPath" })> r As ValueTuple(Of String, Boolean, Boolean)) r.Item3
                    func11 = func12
                    JavaUtils.<>c.<>9__4_3 = func12
                End If
                If results5.All(func11) Then
                    Logger.Warn("找到的所有 Java 都包含特殊路径，保留全部结果", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\PCLCS\Java.cs")
                Else
                    Dim results6 As IEnumerable(Of ValueTuple(Of String, Boolean, Boolean)) = CS$<>8__locals1.results
                    Dim <>9__4_7 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = JavaUtils.<>c.<>9__4_4
                    Dim func13 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = <>9__4_7
                    If <>9__4_7 Is Nothing Then
                        Dim func14 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = Function(<TupleElementNames(New String() { "Folder", "HasReparsePoint", "IsSpecialPath" })> r As ValueTuple(Of String, Boolean, Boolean)) r.Item3
                        func13 = func14
                        JavaUtils.<>c.<>9__4_4 = func14
                    End If
                    If results6.Any(func13) Then
                        Dim logInterpolatedStringHandler2 As Logger.LogInterpolatedStringHandler(Of Logger.WarnLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.WarnLogLevel)(28, 1, flag)
                        If flag Then
                            logInterpolatedStringHandler2.AppendLiteral("找到的以下 Java 包含特殊路径，将它们从列表中排除：")
                            Dim results7 As IEnumerable(Of ValueTuple(Of String, Boolean, Boolean)) = CS$<>8__locals1.results
                            Dim <>9__4_8 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = JavaUtils.<>c.<>9__4_12
                            Dim func15 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = <>9__4_8
                            If <>9__4_8 Is Nothing Then
                                Dim func16 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = Function(<TupleElementNames(New String() { "Folder", "HasReparsePoint", "IsSpecialPath" })> r As ValueTuple(Of String, Boolean, Boolean)) r.Item3
                                func15 = func16
                                JavaUtils.<>c.<>9__4_12 = func16
                            End If
                            Dim enumerable2 As IEnumerable(Of ValueTuple(Of String, Boolean, Boolean)) = results7.Where(func15)
                            Dim <>9__4_9 As Func(Of ValueTuple(Of String, Boolean, Boolean), String) = JavaUtils.<>c.<>9__4_13
                            Dim func17 As Func(Of ValueTuple(Of String, Boolean, Boolean), String) = <>9__4_9
                            If <>9__4_9 Is Nothing Then
                                Dim func18 As Func(Of ValueTuple(Of String, Boolean, Boolean), String) = Function(<TupleElementNames(New String() { "Folder", "HasReparsePoint", "IsSpecialPath" })> r As ValueTuple(Of String, Boolean, Boolean)) r.Item1
                                func17 = func18
                                JavaUtils.<>c.<>9__4_13 = func18
                            End If
                            logInterpolatedStringHandler2.AppendFormatted(enumerable2.[Select](func17).Join("、"c))
                        End If
                        Logger.Warn(logInterpolatedStringHandler2, LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\PCLCS\Java.cs")
                        Dim concurrentBag As ConcurrentBag(Of ValueTuple(Of String, Boolean, Boolean)) = New ConcurrentBag(Of ValueTuple(Of String, Boolean, Boolean))()
                        Dim results8 As IEnumerable(Of ValueTuple(Of String, Boolean, Boolean)) = CS$<>8__locals1.results
                        Dim <>9__4_10 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = JavaUtils.<>c.<>9__4_14
                        Dim func19 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = <>9__4_10
                        If <>9__4_10 Is Nothing Then
                            Dim func20 As Func(Of ValueTuple(Of String, Boolean, Boolean), Boolean) = Function(<TupleElementNames(New String() { "Folder", "HasReparsePoint", "IsSpecialPath" })> r As ValueTuple(Of String, Boolean, Boolean)) Not r.Item3
                            func19 = func20
                            JavaUtils.<>c.<>9__4_14 = func20
                        End If
                        For Each valueTuple2 As ValueTuple(Of String, Boolean, Boolean) In results8.Where(func19)
                            concurrentBag.Add(valueTuple2)
                        Next
                        CS$<>8__locals1.results = concurrentBag
                    End If
                End If
                Dim results9 As IEnumerable(Of ValueTuple(Of String, Boolean, Boolean)) = CS$<>8__locals1.results
                Dim <>9__4_11 As Func(Of ValueTuple(Of String, Boolean, Boolean), String) = JavaUtils.<>c.<>9__4_5
                Dim func21 As Func(Of ValueTuple(Of String, Boolean, Boolean), String) = <>9__4_11
                If <>9__4_11 Is Nothing Then
                    Dim func22 As Func(Of ValueTuple(Of String, Boolean, Boolean), String) = Function(<TupleElementNames(New String() { "Folder", "HasReparsePoint", "IsSpecialPath" })> r As ValueTuple(Of String, Boolean, Boolean)) r.Item1
                    func21 = func22
                    JavaUtils.<>c.<>9__4_5 = func22
                End If
                Dim enumerable3 As IEnumerable(Of String) = results9.[Select](func21).Distinct(StringComparer.OrdinalIgnoreCase)
                Dim <>9__4_12 As Func(Of String, Java) = JavaUtils.<>c.<>9__4_6
                Dim func23 As Func(Of String, Java) = <>9__4_12
                If <>9__4_12 Is Nothing Then
                    Dim func24 As Func(Of String, Java) = Function(folder As String) New Java(folder, Nothing)
                    func23 = func24
                    JavaUtils.<>c.<>9__4_6 = func24
                End If
                Dim list2 As List(Of Java) = enumerable3.[Select](func23).ToList()
                list2 = ( Await JavaUtils.CheckAllAsync(list2, CS$<>8__locals1.cancellationToken, If((progress IsNot Nothing), progress.SplitTo(1.0), Nothing)).NoCapture()).ToList()
                list2 = Await JavaUtils.SortAsync(list2, CS$<>8__locals1.cancellationToken, Nothing).NoCapture()
                Dim logInterpolatedStringHandler3 As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(21, 2, flag)
                If flag Then
                    logInterpolatedStringHandler3.AppendLiteral("快速查找 Java 完成，共有 ")
                    logInterpolatedStringHandler3.AppendFormatted(Of Integer)(list2.Count)
                    logInterpolatedStringHandler3.AppendLiteral(" 个候选：")
                    logInterpolatedStringHandler3.AppendFormatted(list2.Join("、"c))
                End If
                Logger.Info(logInterpolatedStringHandler3, LogBehavior.None, "E:\Projects\PCL\本体代码\PCLCS\Java.cs")
                list = list2
            End If
            Return list
        End Function

        <CompilerGenerated()>
        Friend Function <SearchFoldersAsync>g__HasReparsePoint|4_8(info As FileSystemInfo) As Boolean
            If PathUtils.IsParentOf(Paths.AppData + ".minecraft\runtime\", info.FullName) Then
                Return False
            End If
            While Not info.Attributes.HasFlagF(FileAttributes.ReparsePoint)
                Dim fileInfo As FileInfo = TryCast(info, FileInfo)
                info = If((fileInfo IsNot Nothing), fileInfo.Directory, CType(info, DirectoryInfo).Parent)
                If info Is Nothing Then
                    Return False
                End If
            End While
            Return True
        End Function

        Friend CandidateFolders As Lazy(Of List(Of String)) = New Lazy(Of List(Of String))(Function()
            Dim list As List(Of String) = New List(Of String)()
            list.Add(Paths.AppData + ".minecraft\runtime\")
            list.Add(Paths.AppData + ".hmcl\java\")
            list.Add(Paths.AppData + "ATLauncher\runtimes\minecraft\")
            list.Add(Paths.AppData + "ModrinthApp\meta\java_versions\")
            list.Add(Paths.AppData + "PrismLauncher\java\")
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\curseforge\minecraft\Install\runtime\")
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\.jdks\")
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\.sdkman\candidates\java\")
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\.ftba\bin\runtime\")
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\Packages\Microsoft.4297127D64EC6_8wekyb3d8bbwe\LocalCache\Local\runtime\")
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\Minecraft Launcher\runtime\")
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\Minecraft\runtime\")
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\Curse\Minecraft\Install\runtime\")
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\Java\")
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\Eclipse Adoptium\")
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\Amazon Corretto\")
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\Zulu\")
            list.AddRange(DirectoryUtils.EnumerateDirectories(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\Microsoft\", False, "jdk-*"))
            Dim enumerable As IEnumerable(Of String) = (Environment.GetEnvironmentVariable("JDK_HOME") + ";" + Environment.GetEnvironmentVariable("JAVA_HOME")).Split(";", True)
            Dim <>9__5_ As Func(Of String, String) = JavaUtils.<>c.<>9__5_3
            Dim func As Func(Of String, String) = <>9__5_
            If <>9__5_ Is Nothing Then
                Dim func2 As Func(Of String, String) = Function(path As String) path.Trim(New Char() { " "c, """"c })
                func = func2
                JavaUtils.<>c.<>9__5_3 = func2
            End If
            list.AddRange(enumerable.[Select](func))
            Dim <>9__5_2 As Func(Of String, String) = JavaUtils.<>c.<>9__5_1
            Dim func3 As Func(Of String, String) = <>9__5_2
            If <>9__5_2 Is Nothing Then
                Dim func4 As Func(Of String, String) = Function(f As String)
                    Dim text As String
                    Try
                        text = PathUtils.ForCompare(f)
                    Catch ex As Exception
                        Logger.Warn(ex, "Java 候选文件夹路径无效（" + f + "）", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\PCLCS\Java.cs")
                        text = Nothing
                    End Try
                    Return text
                End Function
                func3 = func4
                JavaUtils.<>c.<>9__5_1 = func4
            End If
            Dim enumerable2 As IEnumerable(Of String) = list.[Select](func3)
            Dim <>9__5_3 As Func(Of String, Boolean) = JavaUtils.<>c.<>9__5_2
            Dim func5 As Func(Of String, Boolean) = <>9__5_3
            If <>9__5_3 Is Nothing Then
                Dim func6 As Func(Of String, Boolean) = Function(f As String) f <> Nothing
                func5 = func6
                JavaUtils.<>c.<>9__5_2 = func6
            End If
            Return enumerable2.Where(func5).Cast().ToList()
        End Function)
    End Module
End Namespace
