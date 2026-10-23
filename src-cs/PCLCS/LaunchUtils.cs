using System;
using System.IO;
using System.Runtime.CompilerServices;
using MeloongCore;

namespace PCLCS
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class LaunchUtils
    {
        public static string ExtractPatch(string patchFilePath, string outputDirectory)
        {
            bool flag;
            Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(12, 2, out flag);
            if (flag)
            {
                logInterpolatedStringHandler.AppendLiteral("选定的资源 ");
                logInterpolatedStringHandler.AppendFormatted(patchFilePath);
                logInterpolatedStringHandler.AppendLiteral(" 输出路径：");
                logInterpolatedStringHandler.AppendFormatted(outputDirectory);
            }
            Logger.Info(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\PCLCS\\Launch\\LaunchUtils.cs");
            object extractPatchLock = LaunchUtils.ExtractPatchLock;
            lock (extractPatchLock)
            {
                try
                {
                    FileUtils.ExtractResources(patchFilePath, Path.Combine(outputDirectory, PathUtils.GetLastPart(patchFilePath)), typeof(LaunchUtils));
                }
                catch (Exception ex)
                {
                    if (!FileUtils.Exists(outputDirectory))
                    {
                        throw new FileNotFoundException("释放 " + patchFilePath + " 失败", ex);
                    }
                    Logger.Warn(ex, patchFilePath + " 文件重新释放失败，将尝试更换文件名重新生成", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\PCLCS\\Launch\\LaunchUtils.cs");
                    patchFilePath = Path.Combine(outputDirectory, PathUtils.GetFileNameWithoutExtension(patchFilePath) + "2." + PathUtils.GetExtension(patchFilePath));
                    try
                    {
                        FileUtils.ExtractResources(patchFilePath, Path.Combine(outputDirectory, PathUtils.GetLastPart(patchFilePath)), typeof(LaunchUtils));
                    }
                    catch (Exception ex2)
                    {
                        throw new FileNotFoundException("释放 " + patchFilePath + " 最终尝试失败", ex2);
                    }
                }
            }
            return Path.Combine(outputDirectory, PathUtils.GetLastPart(patchFilePath));
        }

        private static readonly object ExtractPatchLock = new object();
    }
}
