using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MeloongCore;
using MeloongCore.Extensions;

namespace PCLCS
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class JavaUtils
    {
        public static async Task<List<Java>> CheckAllAsync(IEnumerable<Java> javas, CancellationToken cancellationToken = default(CancellationToken), [Nullable(2)] ProgressProvider progress = null)
        {
            JavaUtils.<>c__DisplayClass0_0 CS$<>8__locals1 = new JavaUtils.<>c__DisplayClass0_0();
            CS$<>8__locals1.cancellationToken = cancellationToken;
            await TaskUtils.WhenAll<bool>(javas.Where<Java>((Java j) => j.available == null).Select<Java, Task<bool>>(delegate(Java java)
            {
                JavaUtils.<>c__DisplayClass0_0.<<CheckAllAsync>b__1>d <<CheckAllAsync>b__1>d;
                <<CheckAllAsync>b__1>d.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
                <<CheckAllAsync>b__1>d.<>4__this = CS$<>8__locals1;
                <<CheckAllAsync>b__1>d.java = java;
                <<CheckAllAsync>b__1>d.<>1__state = -1;
                <<CheckAllAsync>b__1>d.<>t__builder.Start<JavaUtils.<>c__DisplayClass0_0.<<CheckAllAsync>b__1>d>(ref <<CheckAllAsync>b__1>d);
                return <<CheckAllAsync>b__1>d.<>t__builder.Task;
            }), progress).NoCapture<bool[]>();
            return javas.Where<Java>((Java j) => j.available.GetValueOrDefault()).ToList<Java>();
        }

        public static async Task<List<Java>> SortAsync(List<Java> javas, CancellationToken cancellationToken = default(CancellationToken), [Nullable(2)] ProgressProvider progress = null)
        {
            List<Java> list = await JavaUtils.CheckAllAsync(javas, cancellationToken, progress).NoCapture<List<Java>>();
            javas = list;
            javas.Sort(delegate(Java left, Java right)
            {
                bool flag = JavaUtils.CandidateFolders.Value.Any<string>((string folder) => PathUtils.IsParentOf(folder, left.Folder));
                bool flag2 = JavaUtils.CandidateFolders.Value.Any<string>((string folder) => PathUtils.IsParentOf(folder, right.Folder));
                if (flag == flag2)
                {
                    int num = ((left.Version == null) ? int.MaxValue : Math.Abs(left.Version.Major - 21));
                    int num2 = ((right.Version == null) ? int.MaxValue : Math.Abs(right.Version.Major - 21));
                    return num.CompareTo(num2);
                }
                if (!flag)
                {
                    return 1;
                }
                return -1;
            });
            return javas;
        }

        public static async Task RefreshListAsync(CancellationToken c = default(CancellationToken), [Nullable(2)] ProgressProvider p = null)
        {
            List<Java> list = await JavaUtils.SearchFoldersAsync(true, JavaUtils.CandidateFolders.Value, c, (p != null) ? p.SplitTo(0.5) : null).NoCapture<List<Java>>();
            List<Java> javaList = list;
            ConcurrentList<Java> oldJavaList = Configs.JavaList.Get();
            list = await JavaUtils.CheckAllAsync(oldJavaList, c, (p != null) ? p.SplitTo(0.9) : null).NoCapture<List<Java>>();
            oldJavaList = list;
            javaList.RemoveAll<Java>(oldJavaList);
            list = await JavaUtils.SortAsync(javaList, c, null).NoCapture<List<Java>>();
            javaList = list;
            javaList.AddRange(oldJavaList);
            javaList = javaList.Distinct<Java>().ToList<Java>();
            javaList.RemoveIf<Java>((Java j) => Configs.JavaRemovedList.Get().Contains(j.Folder, StringComparer.OrdinalIgnoreCase));
            ConcurrentList<Java> concurrentList = new ConcurrentList<Java>();
            foreach (Java java in javaList)
            {
                concurrentList.Add(java);
            }
            Configs.JavaList.Set(concurrentList);
            bool flag;
            Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(20, 1, out flag);
            if (flag)
            {
                logInterpolatedStringHandler.AppendLiteral("Java 搜索完成，发现 ");
                logInterpolatedStringHandler.AppendFormatted<int>(javaList.Count);
                logInterpolatedStringHandler.AppendLiteral(" 个 Java");
            }
            Logger.Info(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\PCLCS\\Java.cs");
        }

        public static async Task<List<Java>> SearchFoldersAsync(bool includeSubDirectories, IEnumerable<string> folders, CancellationToken cancellationToken = default(CancellationToken), [Nullable(2)] ProgressProvider progress = null)
        {
            JavaUtils.<>c__DisplayClass4_0 CS$<>8__locals1 = new JavaUtils.<>c__DisplayClass4_0();
            CS$<>8__locals1.includeSubDirectories = includeSubDirectories;
            CS$<>8__locals1.cancellationToken = cancellationToken;
            CS$<>8__locals1.results = new ConcurrentBag<ValueTuple<string, bool, bool>>();
            await TaskUtils.ForEachAsync<string>(folders.Distinct<string>(StringComparer.OrdinalIgnoreCase), 4, delegate(string folder, [Nullable(2)] ProgressProvider cp)
            {
                JavaUtils.<>c__DisplayClass4_0.<<SearchFoldersAsync>b__0>d <<SearchFoldersAsync>b__0>d;
                <<SearchFoldersAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
                <<SearchFoldersAsync>b__0>d.<>4__this = CS$<>8__locals1;
                <<SearchFoldersAsync>b__0>d.folder = folder;
                <<SearchFoldersAsync>b__0>d.<>1__state = -1;
                <<SearchFoldersAsync>b__0>d.<>t__builder.Start<JavaUtils.<>c__DisplayClass4_0.<<SearchFoldersAsync>b__0>d>(ref <<SearchFoldersAsync>b__0>d);
                return <<SearchFoldersAsync>b__0>d.<>t__builder.Task;
            }, CS$<>8__locals1.cancellationToken, (progress != null) ? progress.SplitTo(0.2) : null).NoCapture();
            List<Java> list;
            if (!CS$<>8__locals1.results.Any<ValueTuple<string, bool, bool>>())
            {
                list = new List<Java>();
            }
            else
            {
                bool flag;
                if (CS$<>8__locals1.results.All<ValueTuple<string, bool, bool>>(([TupleElementNames(new string[] { "Folder", "HasReparsePoint", "IsSpecialPath" })] ValueTuple<string, bool, bool> r) => r.Item2))
                {
                    Logger.Warn("找到的所有 Java 都包含重解析点，保留全部结果", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\PCLCS\\Java.cs");
                }
                else if (CS$<>8__locals1.results.Any<ValueTuple<string, bool, bool>>(([TupleElementNames(new string[] { "Folder", "HasReparsePoint", "IsSpecialPath" })] ValueTuple<string, bool, bool> r) => r.Item2))
                {
                    Logger.LogInterpolatedStringHandler<Logger.WarnLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.WarnLogLevel>(28, 1, out flag);
                    if (flag)
                    {
                        logInterpolatedStringHandler.AppendLiteral("找到的以下 Java 包含重解析点，将它们从列表中排除：");
                        logInterpolatedStringHandler.AppendFormatted((from r in CS$<>8__locals1.results
                            where r.Item2
                            select r.Item1).Join('、'));
                    }
                    Logger.Warn(ref logInterpolatedStringHandler, LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\PCLCS\\Java.cs");
                    ConcurrentBag<ValueTuple<string, bool, bool>> concurrentBag = new ConcurrentBag<ValueTuple<string, bool, bool>>();
                    foreach (ValueTuple<string, bool, bool> valueTuple in CS$<>8__locals1.results.Where<ValueTuple<string, bool, bool>>(([TupleElementNames(new string[] { "Folder", "HasReparsePoint", "IsSpecialPath" })] ValueTuple<string, bool, bool> r) => !r.Item2))
                    {
                        concurrentBag.Add(valueTuple);
                    }
                    CS$<>8__locals1.results = concurrentBag;
                }
                if (CS$<>8__locals1.results.All<ValueTuple<string, bool, bool>>(([TupleElementNames(new string[] { "Folder", "HasReparsePoint", "IsSpecialPath" })] ValueTuple<string, bool, bool> r) => r.Item3))
                {
                    Logger.Warn("找到的所有 Java 都包含特殊路径，保留全部结果", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\PCLCS\\Java.cs");
                }
                else if (CS$<>8__locals1.results.Any<ValueTuple<string, bool, bool>>(([TupleElementNames(new string[] { "Folder", "HasReparsePoint", "IsSpecialPath" })] ValueTuple<string, bool, bool> r) => r.Item3))
                {
                    Logger.LogInterpolatedStringHandler<Logger.WarnLogLevel> logInterpolatedStringHandler2 = new Logger.LogInterpolatedStringHandler<Logger.WarnLogLevel>(28, 1, out flag);
                    if (flag)
                    {
                        logInterpolatedStringHandler2.AppendLiteral("找到的以下 Java 包含特殊路径，将它们从列表中排除：");
                        logInterpolatedStringHandler2.AppendFormatted((from r in CS$<>8__locals1.results
                            where r.Item3
                            select r.Item1).Join('、'));
                    }
                    Logger.Warn(ref logInterpolatedStringHandler2, LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\PCLCS\\Java.cs");
                    ConcurrentBag<ValueTuple<string, bool, bool>> concurrentBag = new ConcurrentBag<ValueTuple<string, bool, bool>>();
                    foreach (ValueTuple<string, bool, bool> valueTuple2 in CS$<>8__locals1.results.Where<ValueTuple<string, bool, bool>>(([TupleElementNames(new string[] { "Folder", "HasReparsePoint", "IsSpecialPath" })] ValueTuple<string, bool, bool> r) => !r.Item3))
                    {
                        concurrentBag.Add(valueTuple2);
                    }
                    CS$<>8__locals1.results = concurrentBag;
                }
                List<Java> list2 = (from folder in CS$<>8__locals1.results.Select<ValueTuple<string, bool, bool>, string>(([TupleElementNames(new string[] { "Folder", "HasReparsePoint", "IsSpecialPath" })] ValueTuple<string, bool, bool> r) => r.Item1).Distinct<string>(StringComparer.OrdinalIgnoreCase)
                    select new Java(folder, null)).ToList<Java>();
                list2 = (await JavaUtils.CheckAllAsync(list2, CS$<>8__locals1.cancellationToken, (progress != null) ? progress.SplitTo(1.0) : null).NoCapture<List<Java>>()).ToList<Java>();
                list2 = await JavaUtils.SortAsync(list2, CS$<>8__locals1.cancellationToken, null).NoCapture<List<Java>>();
                Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler3 = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(21, 2, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler3.AppendLiteral("快速查找 Java 完成，共有 ");
                    logInterpolatedStringHandler3.AppendFormatted<int>(list2.Count);
                    logInterpolatedStringHandler3.AppendLiteral(" 个候选：");
                    logInterpolatedStringHandler3.AppendFormatted(list2.Join('、'));
                }
                Logger.Info(ref logInterpolatedStringHandler3, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\PCLCS\\Java.cs");
                list = list2;
            }
            return list;
        }

        [CompilerGenerated]
        internal static bool <SearchFoldersAsync>g__HasReparsePoint|4_8(FileSystemInfo info)
        {
            if (PathUtils.IsParentOf(Paths.AppData + ".minecraft\\runtime\\", info.FullName))
            {
                return false;
            }
            while (!info.Attributes.HasFlagF(FileAttributes.ReparsePoint))
            {
                FileInfo fileInfo = info as FileInfo;
                info = ((fileInfo != null) ? fileInfo.Directory : ((DirectoryInfo)info).Parent);
                if (info == null)
                {
                    return false;
                }
            }
            return true;
        }

        internal static readonly Lazy<List<string>> CandidateFolders = new Lazy<List<string>>(delegate
        {
            List<string> list = new List<string>();
            list.Add(Paths.AppData + ".minecraft\\runtime\\");
            list.Add(Paths.AppData + ".hmcl\\java\\");
            list.Add(Paths.AppData + "ATLauncher\\runtimes\\minecraft\\");
            list.Add(Paths.AppData + "ModrinthApp\\meta\\java_versions\\");
            list.Add(Paths.AppData + "PrismLauncher\\java\\");
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\curseforge\\minecraft\\Install\\runtime\\");
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.jdks\\");
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.sdkman\\candidates\\java\\");
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\.ftba\\bin\\runtime\\");
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Packages\\Microsoft.4297127D64EC6_8wekyb3d8bbwe\\LocalCache\\Local\\runtime\\");
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Minecraft Launcher\\runtime\\");
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Minecraft\\runtime\\");
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Curse\\Minecraft\\Install\\runtime\\");
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Java\\");
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Eclipse Adoptium\\");
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Amazon Corretto\\");
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Zulu\\");
            list.AddRange(DirectoryUtils.EnumerateDirectories(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Microsoft\\", false, "jdk-*"));
            list.AddRange(from path in (Environment.GetEnvironmentVariable("JDK_HOME") + ";" + Environment.GetEnvironmentVariable("JAVA_HOME")).Split(";", true)
                select path.Trim(new char[] { ' ', '"' }));
            return (from f in list.Select<string, string>(delegate(string f)
                {
                    string text;
                    try
                    {
                        text = PathUtils.ForCompare(f);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "Java 候选文件夹路径无效（" + f + "）", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\PCLCS\\Java.cs");
                        text = null;
                    }
                    return text;
                })
                where f != null
                select f).Cast<string>().ToList<string>();
        });
    }
}
