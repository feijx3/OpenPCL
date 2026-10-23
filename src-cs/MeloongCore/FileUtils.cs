using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MeloongCore.Extensions;
using Newtonsoft.Json.Linq;
using ThrottleDebounce;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class FileUtils
    {
        public static Stream ReadAsStream(string filePath, [Nullable(2)] Type type = null)
        {
            if (type == null)
            {
                return Retrier.Attempt<FileStream>((int _) => new FileStream(PathUtils.ForApi(filePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite), new uint?(2U), (int _) => TimeSpan.FromMilliseconds(200.0), (Exception ex) => ex is IOException, null, default(CancellationToken));
            }
            Stream manifestResourceStream = type.Assembly.GetManifestResourceStream(filePath);
            if (manifestResourceStream == null)
            {
                throw new FileNotFoundException("Embedded resource \"" + filePath + "\" was not found.");
            }
            return manifestResourceStream;
        }

        public static byte[] ReadAsBytes(string filePath, [Nullable(2)] Type type = null)
        {
            byte[] array;
            using (Stream stream = FileUtils.ReadAsStream(filePath, type))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    array = memoryStream.ToArray();
                }
            }
            return array;
        }

        public static string ReadAsString(string filePath, [Nullable(2)] Encoding encoding = null, [Nullable(2)] Type type = null)
        {
            return FileUtils.ReadAsBytes(filePath, type).GetString(encoding);
        }

        [NullableContext(2)]
        public static string TryReadAsString([Nullable(1)] string filePath, Encoding encoding = null, Type type = null)
        {
            string text;
            try
            {
                if (type == null && !FileUtils.Exists(filePath))
                {
                    text = null;
                }
                else
                {
                    text = FileUtils.ReadAsBytes(filePath, type).GetString(encoding);
                }
            }
            catch
            {
                text = null;
            }
            return text;
        }

        public static string[] ReadAsLines(string filePath, bool skipEmptyLines = false, [Nullable(2)] Encoding encoding = null, [Nullable(2)] Type type = null)
        {
            return FileUtils.ReadAsString(filePath, encoding, type).SplitLines(skipEmptyLines);
        }

        public static string[] TryReadAsLines(string filePath, bool skipEmptyLines = false, [Nullable(2)] Encoding encoding = null, [Nullable(2)] Type type = null)
        {
            string text = FileUtils.TryReadAsString(filePath, encoding, type);
            return ((text != null) ? text.SplitLines(skipEmptyLines) : null) ?? Array.Empty<string>();
        }

        [NullableContext(2)]
        public static JToken ReadAsJson([Nullable(1)] string filePath, Encoding encoding = null, Type type = null)
        {
            return FileUtils.ReadAsString(filePath, encoding, type).DeserializeJson();
        }

        [NullableContext(2)]
        public static T ReadAsJson<[Nullable(0)] T>([Nullable(1)] string filePath, Encoding encoding = null, Type type = null) where T : JToken
        {
            return FileUtils.ReadAsString(filePath, encoding, type).DeserializeJson<T>();
        }

        public static void Write(string filePath, string text, [Nullable(2)] Encoding encoding = null)
        {
            FileUtils.Write(filePath, (encoding ?? new UTF8Encoding()).GetBytes(text));
        }

        public static void Write(string filePath, IEnumerable<byte> content)
        {
            FileUtils.Write(filePath, content.ToArray<byte>());
        }

        public static void Write(string filePath, byte[] content)
        {
            DirectoryUtils.Create(PathUtils.RemoveLastPart(filePath));
            bool flag;
            Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(10, 2, out flag);
            if (flag)
            {
                logInterpolatedStringHandler.AppendLiteral("写入文件：");
                logInterpolatedStringHandler.AppendFormatted(filePath);
                logInterpolatedStringHandler.AppendLiteral("（");
                logInterpolatedStringHandler.AppendFormatted<int>(content.Length);
                logInterpolatedStringHandler.AppendLiteral(" 字节）");
            }
            Logger.Trace(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
            Retrier.Attempt(delegate(int _)
            {
                File.WriteAllBytes(PathUtils.ForApi(filePath), content);
            }, new uint?(2U), (int _) => TimeSpan.FromMilliseconds(200.0), (Exception ex) => ex is IOException, null, default(CancellationToken));
        }

        public static void WriteAtomic(string filePath, string text, [Nullable(2)] Encoding encoding = null)
        {
            FileUtils.WriteAtomic(filePath, (encoding ?? new UTF8Encoding()).GetBytes(text));
        }

        public static void WriteAtomic(string filePath, byte[] content)
        {
            string text = string.Format("{0}.{1:N}.tmp", filePath, Guid.NewGuid());
            try
            {
                FileUtils.Write(text, content);
                if (FileUtils.Exists(filePath))
                {
                    File.Replace(PathUtils.ForApi(text), PathUtils.ForApi(filePath), null, true);
                }
                else
                {
                    File.Move(PathUtils.ForApi(text), PathUtils.ForApi(filePath));
                }
            }
            finally
            {
                FileUtils.Delete(text, false);
            }
        }

        public static void Write(string filePath, Stream stream)
        {
            Retrier.Attempt(delegate(int _)
            {
                FileUtils.Delete(filePath, false);
                using (FileStream fileStream = FileUtils.CreateAsStream(filePath))
                {
                    if (stream.CanSeek && stream.Position != 0L)
                    {
                        stream.Seek(0L, SeekOrigin.Begin);
                    }
                    bool flag;
                    Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(7, 3, out flag);
                    if (flag)
                    {
                        logInterpolatedStringHandler.AppendLiteral("写入文件：");
                        logInterpolatedStringHandler.AppendFormatted(filePath);
                        logInterpolatedStringHandler.AppendLiteral("（");
                        logInterpolatedStringHandler.AppendFormatted(stream.GetType().Name);
                        logInterpolatedStringHandler.AppendFormatted(stream.CanSeek ? string.Format(" {0} 字节", stream.Length) : "");
                        logInterpolatedStringHandler.AppendLiteral("）");
                    }
                    Logger.Trace(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
                    stream.CopyTo(fileStream);
                }
            }, new uint?(2U), (int _) => TimeSpan.FromMilliseconds(200.0), (Exception ex) => ex is IOException, null, default(CancellationToken));
        }

        public static FileStream CreateAsStream(string filePath)
        {
            DirectoryUtils.Create(PathUtils.RemoveLastPart(filePath));
            bool flag;
            Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(6, 1, out flag);
            if (flag)
            {
                logInterpolatedStringHandler.AppendLiteral("创建文件流：");
                logInterpolatedStringHandler.AppendFormatted(filePath);
            }
            Logger.Trace(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
            return Retrier.Attempt<FileStream>(delegate(int _)
            {
                FileUtils.Delete(filePath, false);
                return new FileStream(PathUtils.ForApi(filePath), FileMode.Create);
            }, new uint?(2U), (int _) => TimeSpan.FromMilliseconds(200.0), (Exception ex) => ex is IOException, null, default(CancellationToken));
        }

        public static string CreateRandom()
        {
            return Retrier.Attempt<string>((int _) => Path.GetTempFileName(), new uint?(2U), (int _) => TimeSpan.FromMilliseconds(200.0), (Exception ex) => ex is IOException, null, default(CancellationToken));
        }

        public static void ExtractResources(string logicalName, string localFilePath, Type type)
        {
            byte[] array = FileUtils.ReadAsBytes(logicalName, type);
            if (FileUtils.Exists(localFilePath) && CryptographyUtils.ComputeFileHash(localFilePath, CryptographyUtils.HashMethod.Md5) == CryptographyUtils.ComputeHash(array, CryptographyUtils.HashMethod.Md5))
            {
                return;
            }
            bool flag;
            Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(12, 2, out flag);
            if (flag)
            {
                logInterpolatedStringHandler.AppendLiteral("将资源写入到文件：");
                logInterpolatedStringHandler.AppendFormatted(logicalName);
                logInterpolatedStringHandler.AppendLiteral(" → ");
                logInterpolatedStringHandler.AppendFormatted(localFilePath);
            }
            Logger.Info(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
            FileUtils.Write(localFilePath, array);
        }

        public static void Copy(string sourceFilePath, string destFilePath)
        {
            sourceFilePath = PathUtils.ForCompare(sourceFilePath);
            destFilePath = PathUtils.ForCompare(destFilePath);
            bool flag;
            if (string.CompareOrdinal(sourceFilePath, destFilePath) == 0)
            {
                Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(17, 2, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler.AppendLiteral("复制文件到自身，不执行操作：");
                    logInterpolatedStringHandler.AppendFormatted(sourceFilePath);
                    logInterpolatedStringHandler.AppendLiteral(" → ");
                    logInterpolatedStringHandler.AppendFormatted(destFilePath);
                }
                Logger.Trace(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
                return;
            }
            if (PathUtils.IsSame(sourceFilePath, destFilePath))
            {
                Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler2 = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(27, 2, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler2.AppendLiteral("复制文件到自身，但大小写不同，等效于重命名文件：");
                    logInterpolatedStringHandler2.AppendFormatted(sourceFilePath);
                    logInterpolatedStringHandler2.AppendLiteral(" → ");
                    logInterpolatedStringHandler2.AppendFormatted(destFilePath);
                }
                Logger.Trace(ref logInterpolatedStringHandler2, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
                FileUtils.Move(sourceFilePath, destFilePath, false);
                return;
            }
            DirectoryUtils.Create(PathUtils.RemoveLastPart(destFilePath));
            Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler3 = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(8, 2, out flag);
            if (flag)
            {
                logInterpolatedStringHandler3.AppendLiteral("复制文件：");
                logInterpolatedStringHandler3.AppendFormatted(sourceFilePath);
                logInterpolatedStringHandler3.AppendLiteral(" → ");
                logInterpolatedStringHandler3.AppendFormatted(destFilePath);
            }
            Logger.Trace(ref logInterpolatedStringHandler3, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
            if (FileUtils.Exists(destFilePath))
            {
                FileUtils.SetReadOnly(destFilePath, false);
            }
            Retrier.Attempt(delegate(int _)
            {
                File.Copy(PathUtils.ForApi(sourceFilePath), PathUtils.ForApi(destFilePath), true);
            }, new uint?(2U), (int _) => TimeSpan.FromMilliseconds(200.0), (Exception ex) => ex is IOException, null, default(CancellationToken));
        }

        public static void Move(string sourceFilePath, string destFilePath, bool toRecycleBin = false)
        {
            sourceFilePath = PathUtils.ForCompare(sourceFilePath);
            destFilePath = PathUtils.ForCompare(destFilePath);
            bool flag;
            if (string.CompareOrdinal(sourceFilePath, destFilePath) == 0)
            {
                Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(17, 2, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler.AppendLiteral("剪切文件到自身，不执行操作：");
                    logInterpolatedStringHandler.AppendFormatted(sourceFilePath);
                    logInterpolatedStringHandler.AppendLiteral(" → ");
                    logInterpolatedStringHandler.AppendFormatted(destFilePath);
                }
                Logger.Trace(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
                return;
            }
            if (PathUtils.IsSame(sourceFilePath, destFilePath))
            {
                Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler2 = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(18, 2, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler2.AppendLiteral("剪切文件到自身，但大小写不同：");
                    logInterpolatedStringHandler2.AppendFormatted(sourceFilePath);
                    logInterpolatedStringHandler2.AppendLiteral(" → ");
                    logInterpolatedStringHandler2.AppendFormatted(destFilePath);
                }
                Logger.Trace(ref logInterpolatedStringHandler2, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
                Retrier.Attempt(delegate(int _)
                {
                    string text = Path.Combine(PathUtils.RemoveLastPart(sourceFilePath), Path.GetRandomFileName());
                    FileUtils.SetReadOnly(sourceFilePath, false);
                    if (FileUtils.Exists(destFilePath))
                    {
                        FileUtils.SetReadOnly(destFilePath, false);
                    }
                    File.Move(PathUtils.ForApi(sourceFilePath), PathUtils.ForApi(text));
                    File.Move(PathUtils.ForApi(text), PathUtils.ForApi(destFilePath));
                }, new uint?(2U), (int _) => TimeSpan.FromMilliseconds(200.0), (Exception ex) => ex is IOException, null, default(CancellationToken));
                return;
            }
            DirectoryUtils.Create(PathUtils.RemoveLastPart(destFilePath));
            FileUtils.Delete(destFilePath, toRecycleBin);
            Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler3 = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(8, 2, out flag);
            if (flag)
            {
                logInterpolatedStringHandler3.AppendLiteral("剪切文件：");
                logInterpolatedStringHandler3.AppendFormatted(sourceFilePath);
                logInterpolatedStringHandler3.AppendLiteral(" → ");
                logInterpolatedStringHandler3.AppendFormatted(destFilePath);
            }
            Logger.Trace(ref logInterpolatedStringHandler3, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
            Retrier.Attempt(delegate(int _)
            {
                FileUtils.SetReadOnly(sourceFilePath, false);
                if (FileUtils.Exists(destFilePath))
                {
                    FileUtils.SetReadOnly(destFilePath, false);
                }
                File.Move(PathUtils.ForApi(sourceFilePath), PathUtils.ForApi(destFilePath));
            }, new uint?(2U), (int _) => TimeSpan.FromMilliseconds(200.0), (Exception ex) => ex is IOException, null, default(CancellationToken));
        }

        public static bool? Delete(string filePath, bool toRecycleBin = false)
        {
            if (!FileUtils.Exists(filePath))
            {
                return null;
            }
            bool flag;
            Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(1, 2, out flag);
            if (flag)
            {
                logInterpolatedStringHandler.AppendFormatted(toRecycleBin ? "将文件删除到回收站" : "删除文件");
                logInterpolatedStringHandler.AppendLiteral("：");
                logInterpolatedStringHandler.AppendFormatted(filePath);
            }
            Logger.Trace(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
            if (toRecycleBin)
            {
                try
                {
                    FileUtils.DeleteToRecycleBin(filePath);
                    return new bool?(true);
                }
                catch (Exception ex)
                {
                    Exception ex2;
                    Logger.Warn(ex2, "无法将文件删除到回收站，回退到永久删除：" + filePath, LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
                }
            }
            Retrier.Attempt(delegate(int _)
            {
                FileUtils.SetReadOnly(filePath, false);
                File.Delete(PathUtils.ForApi(filePath));
            }, new uint?(2U), (int _) => TimeSpan.FromMilliseconds(200.0), (Exception ex) => ex is IOException, null, default(CancellationToken));
            return new bool?(false);
        }

        internal static void DeleteToRecycleBin(string target)
        {
            FileUtils.<>c__DisplayClass20_0 CS$<>8__locals1 = new FileUtils.<>c__DisplayClass20_0();
            CS$<>8__locals1.target = target;
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                Retrier.Attempt(delegate(int _)
                {
                    base.<DeleteToRecycleBin>g__Run|0();
                }, new uint?(2U), (int _) => TimeSpan.FromMilliseconds(200.0), (Exception ex) => ex is IOException, null, default(CancellationToken));
                return;
            }
            ExceptionDispatchInfo internalEx = null;
            Thread thread = new Thread(delegate
            {
                try
                {
                    Action<int> action;
                    if ((action = CS$<>8__locals1.<>9__5) == null)
                    {
                        action = (CS$<>8__locals1.<>9__5 = delegate(int _)
                        {
                            base.<DeleteToRecycleBin>g__Run|0();
                        });
                    }
                    Retrier.Attempt(action, new uint?(2U), (int _) => TimeSpan.FromMilliseconds(200.0), (Exception ex) => ex is IOException || ex is COMException, null, default(CancellationToken));
                }
                catch (Exception ex)
                {
                    Exception ex2;
                    internalEx = ExceptionDispatchInfo.Capture(ex2);
                }
            });
            thread.IsBackground = true;
            thread.Name = "DeleteToRecycleBin";
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            ExceptionDispatchInfo internalEx2 = internalEx;
            if (internalEx2 == null)
            {
                return;
            }
            internalEx2.Throw();
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out FileUtils.IShellItem ppv);

        public static ZipArchive OpenZip(string zipFilePath, CancellationToken c = default(CancellationToken))
        {
            return Retrier.Attempt<ZipArchive>(delegate(int _)
            {
                ZipArchive zipArchive;
                try
                {
                    try
                    {
                        zipArchive = base.<OpenZip>g__TryOpen|0(new UTF8Encoding(false, true));
                    }
                    catch (DecoderFallbackException)
                    {
                        zipArchive = base.<OpenZip>g__TryOpen|0(Encoding.GetEncoding("GB18030"));
                    }
                }
                catch (InvalidDataException ex)
                {
                    InvalidDataException ex2;
                    throw new InvalidDataException("文件不是压缩包，或者文件已损坏（" + zipFilePath + "）", ex2);
                }
                return zipArchive;
            }, new uint?(10U), (int _) => TimeSpan.FromMilliseconds(200.0), (Exception ex) => ex is IOException, null, c);
        }

        public static void ExtractToDirectory(string compressionFile, string outputDirectory, CancellationToken c = default(CancellationToken), [Nullable(2)] ProgressProvider p = null)
        {
            compressionFile = PathUtils.ForApi(compressionFile);
            DirectoryUtils.Create(outputDirectory);
            if (PathUtils.GetExtension(compressionFile) == "gz")
            {
                string text = Path.Combine(outputDirectory, PathUtils.GetFileNameWithoutExtension(compressionFile));
                bool flag;
                Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(12, 2, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler.AppendLiteral("解压 gz 文件：");
                    logInterpolatedStringHandler.AppendFormatted(compressionFile);
                    logInterpolatedStringHandler.AppendLiteral(" → ");
                    logInterpolatedStringHandler.AppendFormatted(text);
                }
                Logger.Trace(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
                using (Stream stream = FileUtils.ReadAsStream(compressionFile, null))
                {
                    using (GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        FileUtils.Write(text, gzipStream);
                        if (p == null)
                        {
                            return;
                        }
                        p.Finish(ProgressProvider.ChildrenAction.Finished);
                        return;
                    }
                }
            }
            using (ZipArchive zipArchive = FileUtils.OpenZip(compressionFile, c))
            {
                List<ZipArchiveEntry> list = zipArchive.Entries.Where<ZipArchiveEntry>((ZipArchiveEntry e) => !string.IsNullOrEmpty(e.Name)).ToList<ZipArchiveEntry>();
                int count = list.Count;
                int num = 0;
                bool flag;
                Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler2 = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(19, 3, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler2.AppendLiteral("解压 zip 文件：");
                    logInterpolatedStringHandler2.AppendFormatted(compressionFile);
                    logInterpolatedStringHandler2.AppendLiteral(" → ");
                    logInterpolatedStringHandler2.AppendFormatted(outputDirectory);
                    logInterpolatedStringHandler2.AppendLiteral("（共 ");
                    logInterpolatedStringHandler2.AppendFormatted<int>(count);
                    logInterpolatedStringHandler2.AppendLiteral(" 项）");
                }
                Logger.Trace(ref logInterpolatedStringHandler2, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
                foreach (ZipArchiveEntry zipArchiveEntry in list)
                {
                    c.ThrowIfCancellationRequested();
                    num++;
                    if (count > 0 && p != null)
                    {
                        p.Set((double)num / (double)count, false, ProgressProvider.ChildrenAction.Finished);
                    }
                    string text2 = Path.Combine(outputDirectory, zipArchiveEntry.FullName);
                    if (!PathUtils.IsParentOf(outputDirectory, text2))
                    {
                        throw new InvalidDataException("Zip 文件项 " + zipArchiveEntry.FullName + " 的路径在压缩包之外，这可能导致安全问题");
                    }
                    using (Stream stream2 = zipArchiveEntry.Open())
                    {
                        FileUtils.Write(text2, stream2);
                    }
                }
            }
        }

        public static void CreateZipFromDirectory(string outputFullPath, string sourceDirectory)
        {
            DirectoryUtils.Create(PathUtils.RemoveLastPart(outputFullPath));
            FileUtils.Delete(outputFullPath, false);
            bool flag;
            Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(22, 2, out flag);
            if (flag)
            {
                logInterpolatedStringHandler.AppendLiteral("将文件夹中的内容压缩为 zip 文件：");
                logInterpolatedStringHandler.AppendFormatted(sourceDirectory);
                logInterpolatedStringHandler.AppendLiteral(" → ");
                logInterpolatedStringHandler.AppendFormatted(outputFullPath);
            }
            Logger.Trace(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
            Retrier.Attempt(delegate(int _)
            {
                ZipFile.CreateFromDirectory(PathUtils.ForApi(sourceDirectory), PathUtils.ForApi(outputFullPath));
            }, new uint?(2U), (int _) => TimeSpan.FromMilliseconds(200.0), (Exception ex) => ex is IOException, null, default(CancellationToken));
        }

        public static void CreateZipFromFiles(string outputFullPath, params string[] sourceFiles)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (string text in sourceFiles)
            {
                string lastPart = PathUtils.GetLastPart(text);
                if (dictionary.ContainsKey(lastPart))
                {
                    throw new ArgumentException("尝试将多个同文件名的文件放进压缩包中（" + lastPart + "）", "sourceFiles");
                }
                dictionary.Add(lastPart, text);
            }
            FileUtils.CreateZipFromFiles(outputFullPath, dictionary);
        }

        public static void CreateZipFromFiles(string outputFullPath, IDictionary<string, string> sources)
        {
            DirectoryUtils.Create(PathUtils.RemoveLastPart(outputFullPath));
            FileUtils.Delete(outputFullPath, false);
            using (ZipArchive zipArchive = Retrier.Attempt<ZipArchive>((int _) => ZipFile.Open(PathUtils.ForApi(outputFullPath), ZipArchiveMode.Create), new uint?(2U), (int _) => TimeSpan.FromMilliseconds(200.0), (Exception ex) => ex is IOException, null, default(CancellationToken)))
            {
                bool flag;
                Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(18, 3, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler.AppendLiteral("创建 zip 文件：");
                    logInterpolatedStringHandler.AppendFormatted<int>(sources.Count);
                    logInterpolatedStringHandler.AppendLiteral(" 个文件 → ");
                    logInterpolatedStringHandler.AppendFormatted(outputFullPath);
                    logInterpolatedStringHandler.AppendLiteral("\n");
                    logInterpolatedStringHandler.AppendFormatted(sources.Select<KeyValuePair<string, string>, string>((KeyValuePair<string, string> p) => "- " + p.Value + " → " + p.Key).Join('\n'));
                }
                Logger.Trace(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
                foreach (KeyValuePair<string, string> keyValuePair in sources)
                {
                    zipArchive.CreateEntryFromFile(PathUtils.ForApi(keyValuePair.Value), keyValuePair.Key.Replace('\\', '/'), CompressionLevel.Optimal);
                }
            }
        }

        public static bool Exists(string filePath)
        {
            return File.Exists(PathUtils.ForApi(filePath));
        }

        public static bool IsAvailable(string filePath)
        {
            if (!FileUtils.Exists(filePath))
            {
                return false;
            }
            bool flag;
            Exception ex2;
            try
            {
                Retrier.Attempt(delegate(int _)
                {
                    using (new FileStream(PathUtils.ForApi(filePath), FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                    }
                }, new uint?(3U), (int _) => TimeSpan.FromMilliseconds(200.0), (Exception ex) => ex is IOException, null, default(CancellationToken));
                flag = true;
            }
            catch (Exception ex) when (ex2 is IOException || ex2 is UnauthorizedAccessException)
            {
                flag = false;
            }
            return flag;
        }

        public static FileInfo GetInfo(string path)
        {
            return new FileInfo(PathUtils.ForApi(path));
        }

        public static void SetReadOnly(string path, bool readOnly)
        {
            path = PathUtils.ForApi(path);
            FileAttributes attributes = File.GetAttributes(path);
            FileAttributes fileAttributes = (readOnly ? (attributes | FileAttributes.ReadOnly) : (attributes & ~FileAttributes.ReadOnly));
            if (attributes == fileAttributes)
            {
                return;
            }
            bool flag;
            Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(16, 4, out flag);
            if (flag)
            {
                logInterpolatedStringHandler.AppendFormatted(readOnly ? "添加" : "移除");
                logInterpolatedStringHandler.AppendLiteral("只读属性：");
                logInterpolatedStringHandler.AppendFormatted(path);
                logInterpolatedStringHandler.AppendLiteral("（原属性：");
                logInterpolatedStringHandler.AppendFormatted<FileAttributes>(attributes);
                logInterpolatedStringHandler.AppendLiteral("，新属性：");
                logInterpolatedStringHandler.AppendFormatted<FileAttributes>(fileAttributes);
                logInterpolatedStringHandler.AppendLiteral("）");
            }
            Logger.Trace(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
            File.SetAttributes(path, fileAttributes);
        }

        [Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport]
        private interface IShellItem
        {
        }

        [NullableContext(0)]
        [Guid("3AD05575-8857-4850-9277-11B85BDB8E09")]
        [ComImport]
        private class FileOperation
        {
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            public extern FileOperation();
        }

        [Guid("947AAB5F-0A5C-4C13-B4D6-4BF7836FC9F8")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport]
        private interface IFileOperation
        {
            void Advise(IntPtr pfops, out uint pdwCookie);

            void Unadvise(uint dwCookie);

            void SetOperationFlags(uint dwOperationFlags);

            void SetProgressMessage([MarshalAs(UnmanagedType.LPWStr)] string pszMessage);

            void SetProgressDialog(IntPtr popd);

            void SetProperties(IntPtr pproparray);

            void SetOwnerWindow(IntPtr hwndOwner);

            void ApplyPropertiesToItem(FileUtils.IShellItem psiItem);

            void ApplyPropertiesToItems(IntPtr punkItems);

            void RenameItem(FileUtils.IShellItem psiItem, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName, IntPtr pfopsItem);

            void RenameItems(IntPtr pUnkItems, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName);

            void MoveItem(FileUtils.IShellItem psiItem, FileUtils.IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName, IntPtr pfopsItem);

            void MoveItems(IntPtr punkItems, FileUtils.IShellItem psiDestinationFolder);

            void CopyItem(FileUtils.IShellItem psiItem, FileUtils.IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszCopyName, IntPtr pfopsItem);

            void CopyItems(IntPtr punkItems, FileUtils.IShellItem psiDestinationFolder);

            void DeleteItem(FileUtils.IShellItem psiItem, IntPtr pfopsItem);

            void DeleteItems(IntPtr punkItems);

            void NewItem(FileUtils.IShellItem psiDestinationFolder, uint dwFileAttributes, [MarshalAs(UnmanagedType.LPWStr)] string pszName, [MarshalAs(UnmanagedType.LPWStr)] string pszTemplateName, IntPtr pfopsItem);

            void PerformOperations();

            void GetAnyOperationsAborted([MarshalAs(UnmanagedType.Bool)] out bool pfAnyOperationsAborted);
        }
    }
}
