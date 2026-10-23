using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class MsDelta
    {
        public static void Create(string oldFilePath, string newFilePath, string deltaFilePath)
        {
            if (!FileUtils.Exists(oldFilePath))
            {
                throw new FileNotFoundException("旧文件不存在：" + oldFilePath, oldFilePath);
            }
            if (!FileUtils.Exists(newFilePath))
            {
                throw new FileNotFoundException("新文件不存在：" + newFilePath, newFilePath);
            }
            DirectoryUtils.Create(PathUtils.RemoveLastPart(deltaFilePath));
            bool flag;
            Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(20, 3, out flag);
            if (flag)
            {
                logInterpolatedStringHandler.AppendLiteral("正在生成补丁：");
                logInterpolatedStringHandler.AppendFormatted(oldFilePath);
                logInterpolatedStringHandler.AppendLiteral(" → ");
                logInterpolatedStringHandler.AppendFormatted(newFilePath);
                logInterpolatedStringHandler.AppendLiteral("，补丁文件将输出到：");
                logInterpolatedStringHandler.AppendFormatted(deltaFilePath);
            }
            Logger.Info(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\General\\MsDelta.cs");
            if (!MsDelta.CreateDelta(15L, 131072L, 0L, PathUtils.ForApi(oldFilePath), PathUtils.ForApi(newFilePath), null, null, default(MsDelta.DeltaInput), IntPtr.Zero, 32U, PathUtils.ForApi(deltaFilePath)))
            {
                throw new Win32Exception();
            }
        }

        public static void Apply(string oldFilePath, string deltaFilePath, string newFilePath)
        {
            if (!FileUtils.Exists(oldFilePath))
            {
                throw new FileNotFoundException("旧文件不存在：" + oldFilePath, oldFilePath);
            }
            if (!FileUtils.Exists(deltaFilePath))
            {
                throw new FileNotFoundException("补丁文件不存在：" + deltaFilePath, deltaFilePath);
            }
            DirectoryUtils.Create(PathUtils.RemoveLastPart(newFilePath));
            bool flag;
            Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(27, 3, out flag);
            if (flag)
            {
                logInterpolatedStringHandler.AppendLiteral("正在应用补丁：原始文件 ");
                logInterpolatedStringHandler.AppendFormatted(oldFilePath);
                logInterpolatedStringHandler.AppendLiteral(" + 补丁文件 ");
                logInterpolatedStringHandler.AppendFormatted(deltaFilePath);
                logInterpolatedStringHandler.AppendLiteral(" → 输出至 ");
                logInterpolatedStringHandler.AppendFormatted(newFilePath);
            }
            Logger.Info(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\General\\MsDelta.cs");
            if (!MsDelta.ApplyDelta(1L, PathUtils.ForApi(oldFilePath), PathUtils.ForApi(deltaFilePath), PathUtils.ForApi(newFilePath)))
            {
                throw new Win32Exception();
            }
        }

        [DllImport("msdelta.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CreateDelta(long fileTypeSet, long setFlags, long resetFlags, string sourceName, string targetName, [Nullable(2)] string sourceOptionsName, [Nullable(2)] string targetOptionsName, MsDelta.DeltaInput globalOptions, IntPtr targetFileTime, uint hashAlgId, string deltaName);

        [DllImport("msdelta.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ApplyDelta(long applyFlags, string sourceName, string deltaName, string targetName);

        [NullableContext(0)]
        private struct DeltaInput
        {
            public IntPtr Start;

            public IntPtr Size;

            [MarshalAs(UnmanagedType.Bool)]
            public bool Editable;
        }
    }
}
