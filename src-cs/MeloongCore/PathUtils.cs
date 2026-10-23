using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MeloongCore.Extensions;
using Microsoft.Win32.SafeHandles;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class PathUtils
    {
        public static string AddSlashSuffix(string folder)
        {
            string text = ((folder.EndsWithF(Path.DirectorySeparatorChar, false) || folder.EndsWithF(Path.AltDirectorySeparatorChar, false)) ? "" : Path.DirectorySeparatorChar);
            return folder + ((text != null) ? text.ToString() : null);
        }

        public static string RemoveSlashSuffix(string folder)
        {
            return folder.TrimEnd(new char[]
            {
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar
            });
        }

        public unsafe static string ToShortPath(string pathName)
        {
            if (string.IsNullOrEmpty(pathName))
            {
                return pathName;
            }
            if (!pathName.Contains(":") || pathName.Contains("://"))
            {
                return pathName;
            }
            if (pathName.Length <= 220)
            {
                return pathName;
            }
            pathName = PathUtils.ForCompare(pathName);
            string text = "";
            string text2 = pathName;
            if (FileUtils.Exists(pathName))
            {
                string lastPart = PathUtils.GetLastPart(pathName);
                if (lastPart.Length >= 180)
                {
                    bool flag;
                    Logger.LogInterpolatedStringHandler<Logger.WarnLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.WarnLogLevel>(17, 1, out flag);
                    if (flag)
                    {
                        logInterpolatedStringHandler.AppendLiteral("文件名过长，将为文件名生成短路径：");
                        logInterpolatedStringHandler.AppendFormatted(pathName);
                    }
                    Logger.Warn(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\PathUtils.cs");
                }
                else
                {
                    text = lastPart;
                    text2 = PathUtils.RemoveLastPart(pathName);
                }
            }
            while (!DirectoryUtils.Exists(text2) && !FileUtils.Exists(text2))
            {
                string directoryName = Path.GetDirectoryName(text2);
                if (string.IsNullOrEmpty(directoryName) || PathUtils.IsSame(directoryName, text2))
                {
                    return pathName;
                }
                text = Path.Combine(PathUtils.GetLastPart(text2), text);
                text2 = directoryName;
            }
            if (text.Length >= 200)
            {
                bool flag;
                Logger.LogInterpolatedStringHandler<Logger.WarnLogLevel> logInterpolatedStringHandler2 = new Logger.LogInterpolatedStringHandler<Logger.WarnLogLevel>(32, 2, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler2.AppendLiteral("路径中的大部分内容都尚未建立，无法在内容不存在时缩短路径：");
                    logInterpolatedStringHandler2.AppendFormatted(pathName);
                    logInterpolatedStringHandler2.AppendLiteral(" → ");
                    logInterpolatedStringHandler2.AppendFormatted(text2);
                }
                Logger.Warn(ref logInterpolatedStringHandler2, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\PathUtils.cs");
                return pathName;
            }
            Func<string, string> func = delegate(string pathName)
            {
                char[] array = new char[220];
                int shortPathNameW = PathUtils.GetShortPathNameW(PathUtils.ForApi(pathName), array, array.Length);
                if (shortPathNameW <= 220 && shortPathNameW != 0)
                {
                    return new string(array, 0, shortPathNameW);
                }
                return pathName;
            };
            string text3 = func(text2);
            if (Path.Combine(text3, text).Length > 220)
            {
                bool flag;
                Logger.LogInterpolatedStringHandler<Logger.WarnLogLevel> logInterpolatedStringHandler3 = new Logger.LogInterpolatedStringHandler<Logger.WarnLogLevel>(24, 3, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler3.AppendFormatted("GetShortPathNameW");
                    logInterpolatedStringHandler3.AppendLiteral(" 未能有效缩短路径，将尝试生成新的短名称：");
                    logInterpolatedStringHandler3.AppendFormatted(text2);
                    logInterpolatedStringHandler3.AppendLiteral(" → ");
                    logInterpolatedStringHandler3.AppendFormatted(text3);
                }
                Logger.Warn(ref logInterpolatedStringHandler3, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\PathUtils.cs");
                List<ValueTuple<string, string>> list = new List<ValueTuple<string, string>>();
                string text4 = text2;
                while (!string.IsNullOrEmpty(text4))
                {
                    string lastPart2 = PathUtils.GetLastPart(text4);
                    if (lastPart2.Length > 12 && string.Equals(PathUtils.GetLastPart(func(text4)), lastPart2, StringComparison.OrdinalIgnoreCase))
                    {
                        list.Add(new ValueTuple<string, string>(text4, lastPart2));
                    }
                    text4 = Path.GetDirectoryName(text4);
                }
                foreach (ValueTuple<string, string> valueTuple in list.OrderByDescending<ValueTuple<string, string>, int>(([TupleElementNames(new string[] { "Path", "Name" })] ValueTuple<string, string> candidate) => candidate.Item2.Length))
                {
                    using (SafeFileHandle safeFileHandle = PathUtils.CreateFileW(PathUtils.ForApi(valueTuple.Item1), 1073807360U, 7U, IntPtr.Zero, 3U, 33554432U, IntPtr.Zero))
                    {
                        if (!safeFileHandle.IsInvalid)
                        {
                            bool flag2 = false;
                            try
                            {
                                try
                                {
                                    fixed (string text5 = valueTuple.Item2)
                                    {
                                        char* ptr = text5;
                                        if (ptr != null)
                                        {
                                            ptr += RuntimeHelpers.OffsetToStringData / 2;
                                        }
                                        char* ptr2 = stackalloc char[(UIntPtr)24];
                                        PathUtils.UnicodeString unicodeString = new PathUtils.UnicodeString
                                        {
                                            Length = (ushort)(valueTuple.Item2.Length * 2),
                                            MaximumLength = (ushort)((valueTuple.Item2.Length + 1) * 2),
                                            Buffer = ptr
                                        };
                                        PathUtils.UnicodeString unicodeString2 = new PathUtils.UnicodeString
                                        {
                                            MaximumLength = 24,
                                            Buffer = ptr2
                                        };
                                        PathUtils.GenerateNameContext generateNameContext = default(PathUtils.GenerateNameContext);
                                        for (int i = 0; i < 20; i++)
                                        {
                                            unicodeString2.Length = 0;
                                            if (PathUtils.RtlGenerate8dot3Name(ref unicodeString, 0, ref generateNameContext, ref unicodeString2) != 0)
                                            {
                                                break;
                                            }
                                            string text6 = new string(unicodeString2.Buffer, 0, (int)(unicodeString2.Length / 2));
                                            if (PathUtils.SetFileShortNameW(safeFileHandle, text6))
                                            {
                                                flag2 = true;
                                                break;
                                            }
                                            if (Marshal.GetLastWin32Error() != 183)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                finally
                                {
                                    string text5 = null;
                                }
                            }
                            catch (EntryPointNotFoundException)
                            {
                            }
                            if (flag2)
                            {
                                text3 = func(text2);
                                if (Path.Combine(text3, text).Length <= 220)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return PathUtils.RemoveSlashSuffix(PathUtils.RemoveExtendedPrefix(Path.Combine(text3, text)));
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetShortPathNameW(string lpszLongPath, [Nullable(2)] [Out] char[] buffer, int cchBuffer);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeFileHandle CreateFileW(string fileName, uint desiredAccess, uint shareMode, IntPtr securityAttributes, uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetFileShortNameW(SafeFileHandle file, string shortName);

        [DllImport("ntdll.dll")]
        private static extern int RtlGenerate8dot3Name(ref PathUtils.UnicodeString name, byte allowExtendedCharacters, ref PathUtils.GenerateNameContext context, ref PathUtils.UnicodeString name8dot3);

        public static string ToExtendedFormat(string pathName)
        {
            if (string.IsNullOrWhiteSpace(pathName))
            {
                return pathName;
            }
            if (!pathName.Contains(":"))
            {
                return pathName;
            }
            if (pathName.StartsWithF("\\\\?\\", false))
            {
                return pathName;
            }
            pathName = PathUtils.RemoveSlashSuffix(pathName).Replace('/', '\\');
            if (pathName.StartsWithF("\\\\", false))
            {
                return "\\\\?\\UNC\\" + pathName.Substring(2);
            }
            return "\\\\?\\" + pathName;
        }

        public static string RemoveExtendedPrefix(string pathName)
        {
            if (pathName.StartsWithF("\\\\?\\UNC\\", false))
            {
                return "\\\\" + pathName.AfterLast("\\\\?\\UNC\\", false);
            }
            if (pathName.StartsWithF("\\\\?\\", false))
            {
                return pathName.AfterLast("\\\\?\\", false);
            }
            return pathName;
        }

        public static string RemoveLastPart(string pathName)
        {
            if (pathName.Contains("://"))
            {
                pathName = PathUtils.RemoveSlashSuffix(pathName.BeforeFirst("#", false).BeforeFirst("?", false));
            }
            return PathUtils.RemoveSlashSuffix(pathName).BeforeLastOfAny(new <>z__ReadOnlyArray<string>(new string[] { "\\", "/" }), false);
        }

        public static string GetLastPart(string pathName)
        {
            if (pathName.Contains("://"))
            {
                pathName = PathUtils.RemoveSlashSuffix(pathName.BeforeFirst("#", false).BeforeFirst("?", false));
            }
            else
            {
                pathName = PathUtils.RemoveSlashSuffix(pathName);
            }
            return pathName.AfterLastOfAny(new <>z__ReadOnlyArray<string>(new string[] { "\\", "/" }), false);
        }

        public static string GetFileNameWithoutExtension(string pathName)
        {
            return PathUtils.GetLastPart(pathName).BeforeLast(".", false);
        }

        public static string GetExtension(string pathName)
        {
            string lastPart = PathUtils.GetLastPart(pathName);
            if (!lastPart.Contains("."))
            {
                return "";
            }
            return lastPart.AfterLast(".", false).Lower();
        }

        [NullableContext(2)]
        public static char? GetDiskName(string pathName)
        {
            if (string.IsNullOrEmpty(pathName))
            {
                return null;
            }
            pathName = PathUtils.RemoveExtendedPrefix(pathName);
            if (pathName.StartsWithF("\\\\", false) || pathName.StartsWithF("/", false))
            {
                return null;
            }
            return new char?(char.ToUpper(pathName[0]));
        }

        public static string ForCompare(string pathName)
        {
            string text;
            try
            {
                text = PathUtils.RemoveSlashSuffix(PathUtils.RemoveExtendedPrefix(Path.GetFullPath(pathName)).Replace("/", "\\"));
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("路径不正确：" + pathName, ex);
            }
            return text;
        }

        [NullableContext(2)]
        public static bool IsSame(string path1, string path2)
        {
            return PathUtils.Compare(path1, path2) == 0;
        }

        [NullableContext(2)]
        public static int Compare(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1))
            {
                if (!string.IsNullOrEmpty(path2))
                {
                    return -1;
                }
                return 0;
            }
            else
            {
                if (string.IsNullOrEmpty(path2))
                {
                    return 1;
                }
                return string.Compare(PathUtils.ForCompare(path1), PathUtils.ForCompare(path2), StringComparison.OrdinalIgnoreCase);
            }
        }

        public static string ForApi(string pathName)
        {
            pathName = PathUtils.ToExtendedFormat(pathName);
            if (pathName.EndsWithF(":", false))
            {
                pathName += "\\";
            }
            if (pathName.EndsWithF(":\\", false))
            {
                return pathName;
            }
            return PathUtils.RemoveSlashSuffix(pathName);
        }

        public static bool IsParentOf(string parentPath, string childPath)
        {
            parentPath = PathUtils.ForCompare(parentPath);
            childPath = PathUtils.ForCompare(childPath);
            return childPath.StartsWithF(parentPath + "\\", true) || PathUtils.IsSame(childPath, parentPath);
        }

        [NullableContext(0)]
        private struct UnicodeString
        {
            public ushort Length;

            public ushort MaximumLength;

            public unsafe char* Buffer;
        }

        [NullableContext(0)]
        private struct GenerateNameContext
        {
            public ushort Checksum;

            public byte ChecksumInserted;

            public byte NameLength;

            [FixedBuffer(typeof(char), 8)]
            public PathUtils.GenerateNameContext.<NameBuffer>e__FixedBuffer NameBuffer;

            public uint ExtensionLength;

            [FixedBuffer(typeof(char), 4)]
            public PathUtils.GenerateNameContext.<ExtensionBuffer>e__FixedBuffer ExtensionBuffer;

            public uint LastIndexValue;

            [CompilerGenerated]
            [UnsafeValueType]
            [StructLayout(LayoutKind.Sequential, Size = 8)]
            public struct <ExtensionBuffer>e__FixedBuffer
            {
                public char FixedElementField;
            }

            [CompilerGenerated]
            [UnsafeValueType]
            [StructLayout(LayoutKind.Sequential, Size = 16)]
            public struct <NameBuffer>e__FixedBuffer
            {
                public char FixedElementField;
            }
        }
    }
}
