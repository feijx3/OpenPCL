using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using ThrottleDebounce;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class DirectoryUtils
    {
        public static void Create(string folder)
        {
            if (DirectoryUtils.Exists(folder))
            {
                return;
            }
            bool flag;
            Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(6, 1, out flag);
            if (flag)
            {
                logInterpolatedStringHandler.AppendLiteral("新建文件夹：");
                logInterpolatedStringHandler.AppendFormatted(folder);
            }
            Logger.Trace(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\DirectoryUtils.cs");
            Directory.CreateDirectory(PathUtils.ForApi(folder));
        }

        public static string CreateRandom()
        {
            string text = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            DirectoryUtils.Create(text);
            return text;
        }

        public static bool Exists(string folder)
        {
            return Directory.Exists(PathUtils.ForApi(folder));
        }

        public static DirectoryInfo GetInfo(string folder)
        {
            return new DirectoryInfo(PathUtils.ForApi(folder));
        }

        public static IEnumerable<string> EnumerateFiles(string folder, bool includeSubDirectories = false, string searchPattern = "*")
        {
            if (!DirectoryUtils.Exists(folder))
            {
                return Array.Empty<string>();
            }
            IEnumerable<string> enumerable = Directory.EnumerateFiles(PathUtils.ForApi(folder), searchPattern, includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            Func<string, string> func;
            if ((func = DirectoryUtils.<>O.<0>__RemoveExtendedPrefix) == null)
            {
                func = (DirectoryUtils.<>O.<0>__RemoveExtendedPrefix = new Func<string, string>(PathUtils.RemoveExtendedPrefix));
            }
            return enumerable.Select<string, string>(func);
        }

        public static IEnumerable<string> EnumerateDirectories(string folder, bool includeSubDirectories = false, string searchPattern = "*")
        {
            if (!DirectoryUtils.Exists(folder))
            {
                return Array.Empty<string>();
            }
            IEnumerable<string> enumerable = Directory.EnumerateDirectories(PathUtils.ForApi(folder), searchPattern, includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            Func<string, string> func;
            if ((func = DirectoryUtils.<>O.<0>__RemoveExtendedPrefix) == null)
            {
                func = (DirectoryUtils.<>O.<0>__RemoveExtendedPrefix = new Func<string, string>(PathUtils.RemoveExtendedPrefix));
            }
            return enumerable.Select<string, string>(func);
        }

        public static bool IsEmpty(string folder)
        {
            return !DirectoryUtils.Exists(folder) || !Directory.EnumerateFileSystemEntries(PathUtils.ForApi(folder)).Any<string>();
        }

        public static void Copy(string sourceFolder, string destFolder)
        {
            sourceFolder = PathUtils.ForCompare(sourceFolder);
            destFolder = PathUtils.ForCompare(destFolder);
            bool flag;
            if (string.CompareOrdinal(sourceFolder, destFolder) == 0)
            {
                Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(18, 2, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler.AppendLiteral("复制文件夹到自身，不执行操作：");
                    logInterpolatedStringHandler.AppendFormatted(sourceFolder);
                    logInterpolatedStringHandler.AppendLiteral(" → ");
                    logInterpolatedStringHandler.AppendFormatted(destFolder);
                }
                Logger.Trace(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\DirectoryUtils.cs");
                return;
            }
            if (PathUtils.IsSame(sourceFolder, destFolder))
            {
                Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler2 = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(29, 2, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler2.AppendLiteral("复制文件夹到自身，但大小写不同，等效于重命名文件夹：");
                    logInterpolatedStringHandler2.AppendFormatted(sourceFolder);
                    logInterpolatedStringHandler2.AppendLiteral(" → ");
                    logInterpolatedStringHandler2.AppendFormatted(destFolder);
                }
                Logger.Trace(ref logInterpolatedStringHandler2, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\DirectoryUtils.cs");
                DirectoryUtils.Move(sourceFolder, destFolder);
                return;
            }
            Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler3 = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(9, 2, out flag);
            if (flag)
            {
                logInterpolatedStringHandler3.AppendLiteral("复制文件夹：");
                logInterpolatedStringHandler3.AppendFormatted(sourceFolder);
                logInterpolatedStringHandler3.AppendLiteral(" → ");
                logInterpolatedStringHandler3.AppendFormatted(destFolder);
            }
            Logger.Trace(ref logInterpolatedStringHandler3, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\DirectoryUtils.cs");
            foreach (string text in DirectoryUtils.EnumerateFiles(sourceFolder, true, "*").ToList<string>())
            {
                FileUtils.Copy(text, text.Replace(sourceFolder, destFolder));
            }
        }

        public static void Move(string sourceFolder, string destFolder)
        {
            DirectoryUtils.SafetyCheck(sourceFolder);
            sourceFolder = PathUtils.ForCompare(sourceFolder);
            destFolder = PathUtils.ForCompare(destFolder);
            bool flag;
            if (string.CompareOrdinal(sourceFolder, destFolder) == 0)
            {
                Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(18, 2, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler.AppendLiteral("剪切文件夹到自身，不执行操作：");
                    logInterpolatedStringHandler.AppendFormatted(sourceFolder);
                    logInterpolatedStringHandler.AppendLiteral(" → ");
                    logInterpolatedStringHandler.AppendFormatted(destFolder);
                }
                Logger.Trace(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\DirectoryUtils.cs");
                return;
            }
            if (PathUtils.IsSame(sourceFolder, destFolder))
            {
                Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler2 = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(19, 2, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler2.AppendLiteral("剪切文件夹到自身，但大小写不同：");
                    logInterpolatedStringHandler2.AppendFormatted(sourceFolder);
                    logInterpolatedStringHandler2.AppendLiteral(" → ");
                    logInterpolatedStringHandler2.AppendFormatted(destFolder);
                }
                Logger.Trace(ref logInterpolatedStringHandler2, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\DirectoryUtils.cs");
                string text = Path.Combine(PathUtils.RemoveLastPart(sourceFolder), Path.GetRandomFileName());
                DirectoryUtils.Move(sourceFolder, text);
                DirectoryUtils.Move(text, destFolder);
                return;
            }
            char? c = PathUtils.GetDiskName(sourceFolder);
            int? num = ((c != null) ? new int?((int)c.GetValueOrDefault()) : null);
            c = PathUtils.GetDiskName(destFolder);
            int? num2 = ((c != null) ? new int?((int)c.GetValueOrDefault()) : null);
            if ((num.GetValueOrDefault() == num2.GetValueOrDefault()) & (num != null == (num2 != null)))
            {
                Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler3 = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(14, 2, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler3.AppendLiteral("剪切文件夹到同一磁盘：");
                    logInterpolatedStringHandler3.AppendFormatted(sourceFolder);
                    logInterpolatedStringHandler3.AppendLiteral(" → ");
                    logInterpolatedStringHandler3.AppendFormatted(destFolder);
                }
                Logger.Trace(ref logInterpolatedStringHandler3, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\DirectoryUtils.cs");
                DirectoryUtils.Delete(destFolder, false);
                Retrier.Attempt(delegate(int _)
                {
                    FileUtils.SetReadOnly(sourceFolder, false);
                    Directory.Move(PathUtils.ForApi(sourceFolder), PathUtils.ForApi(destFolder));
                }, new uint?(2U), (int _) => TimeSpan.FromMilliseconds(200.0), (Exception ex) => ex is IOException, null, default(CancellationToken));
                return;
            }
            Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler4 = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(14, 2, out flag);
            if (flag)
            {
                logInterpolatedStringHandler4.AppendLiteral("剪切文件夹到不同磁盘：");
                logInterpolatedStringHandler4.AppendFormatted(sourceFolder);
                logInterpolatedStringHandler4.AppendLiteral(" → ");
                logInterpolatedStringHandler4.AppendFormatted(destFolder);
            }
            Logger.Trace(ref logInterpolatedStringHandler4, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\DirectoryUtils.cs");
            DirectoryUtils.Copy(sourceFolder, destFolder);
            DirectoryUtils.Delete(sourceFolder, false);
        }

        public static bool? Delete(string folder, bool toRecycleBin = false)
        {
            if (!DirectoryUtils.Exists(folder))
            {
                return null;
            }
            bool flag;
            Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(1, 2, out flag);
            if (flag)
            {
                logInterpolatedStringHandler.AppendFormatted(toRecycleBin ? "将文件夹删除到回收站" : "删除文件夹");
                logInterpolatedStringHandler.AppendLiteral("：");
                logInterpolatedStringHandler.AppendFormatted(folder);
            }
            Logger.Trace(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\DirectoryUtils.cs");
            DirectoryUtils.SafetyCheck(folder);
            if (toRecycleBin)
            {
                try
                {
                    FileUtils.DeleteToRecycleBin(folder);
                    return new bool?(true);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "无法将文件夹删除到回收站，回退到永久删除：" + folder, LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\DirectoryUtils.cs");
                }
            }
            DirectoryUtils.<Delete>g__DeleteInternal|9_0(folder);
            return new bool?(false);
        }

        private static void SafetyCheck(string folder)
        {
            folder = PathUtils.ForCompare(folder);
            if (PathUtils.IsSame(folder, Path.GetPathRoot(folder)))
            {
                throw new UnauthorizedAccessException("不应操作磁盘根目录：" + folder);
            }
            if (DirectoryUtils.criticalFolders.Value.Any<string>((string f) => PathUtils.IsParentOf(folder, f)))
            {
                throw new UnauthorizedAccessException("不应操作文件夹：" + folder);
            }
        }

        [CompilerGenerated]
        internal static void <Delete>g__DeleteInternal|9_0(string folder)
        {
            try
            {
                folder = PathUtils.ForApi(folder);
                foreach (string text in DirectoryUtils.EnumerateFiles(folder, false, "*"))
                {
                    FileUtils.Delete(text, false);
                }
                foreach (string text2 in DirectoryUtils.EnumerateDirectories(folder, false, "*"))
                {
                    DirectoryUtils.<Delete>g__DeleteInternal|9_0(text2);
                }
                Retrier.Attempt(delegate(int _)
                {
                    FileUtils.SetReadOnly(folder, false);
                    Directory.Delete(folder, true);
                }, new uint?(2U), (int _) => TimeSpan.FromMilliseconds(200.0), (Exception ex) => ex is IOException, null, default(CancellationToken));
            }
            catch (DirectoryNotFoundException ex)
            {
                if (!DirectoryUtils.Exists(folder))
                {
                    throw;
                }
                DirectoryNotFoundException ex2;
                Logger.Warn(ex2, "该文件夹可能为孤立的符号链接，尝试直接删除（" + folder + "）", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\DirectoryUtils.cs");
                FileUtils.SetReadOnly(folder, false);
                Directory.Delete(folder);
            }
        }

        private static readonly Lazy<HashSet<string>> criticalFolders = new Lazy<HashSet<string>>(() => new HashSet<string>(new string[]
        {
            Paths.Base,
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
            Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            Environment.GetFolderPath(Environment.SpecialFolder.Windows),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads")
        }.Where<string>((string f) => f.Contains(Path.VolumeSeparatorChar))));

        [CompilerGenerated]
        private static class <>O
        {
            [Nullable(0)]
            public static Func<string, string> <0>__RemoveExtendedPrefix;
        }
    }
}
