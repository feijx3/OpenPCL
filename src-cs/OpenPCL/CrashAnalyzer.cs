using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using MeloongCore;
using MeloongCore.Extensions;
using MeloongCore.Wpf;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class CrashAnalyzer
    {
        public CrashAnalyzer(ModMinecraft.McInstance TargetInstance = null)
        {
            this.AnalyzeRawFiles = new List<KeyValuePair<string, string[]>>();
            this.DirectFile = null;
            this.LogMc = null;
            this.LogMcDebug = null;
            this.LogHs = null;
            this.LogCrash = null;
            this.CrashReasons = new Dictionary<CrashAnalyzer.CrashReason, List<string>>();
            this.OutputFiles = new List<string>();
            this.TargetInstance = TargetInstance;
            this.TempFolder = ModMain.RequestTaskTempFolder(false);
            DirectoryUtils.Create(this.TempFolder + "Temp\\");
            DirectoryUtils.Create(this.TempFolder + "Report\\");
            Logger.Info(string.Format("崩溃分析暂存文件夹：{0}", this.TempFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
        }

        public void Collect(string VersionPathIndie, IList<string> LatestLog = null)
        {
            Logger.Info("步骤 1：收集日志文件", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
            List<string> list = new List<string>();
            try
            {
                list.AddRange(DirectoryUtils.EnumerateFiles(VersionPathIndie + "crash-reports\\", true, "*"));
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "收集 Minecraft 崩溃日志文件夹下的日志失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
            }
            try
            {
                try
                {
                    foreach (FileInfo fileInfo in DirectoryUtils.GetInfo(VersionPathIndie).Parent.Parent.EnumerateFiles())
                    {
                        if (Operators.CompareString(fileInfo.Extension ?? "", ".log", false) == 0)
                        {
                            list.Add(PathUtils.RemoveExtendedPrefix(fileInfo.FullName));
                        }
                    }
                }
                finally
                {
                    IEnumerator<FileInfo> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
            }
            catch (Exception ex2)
            {
                Logger.Warn(ex2, "收集 Minecraft 主文件夹下的日志失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
            }
            try
            {
                try
                {
                    foreach (FileInfo fileInfo2 in DirectoryUtils.GetInfo(VersionPathIndie).EnumerateFiles())
                    {
                        if (Operators.CompareString(fileInfo2.Extension ?? "", ".log", false) == 0)
                        {
                            list.Add(PathUtils.RemoveExtendedPrefix(fileInfo2.FullName));
                        }
                    }
                }
                finally
                {
                    IEnumerator<FileInfo> enumerator2 = default;
                    if (enumerator2 != null)
                    {
                        enumerator2.Dispose();
                    }
                }
            }
            catch (Exception ex3)
            {
                Logger.Warn(ex3, "收集 Minecraft 隔离文件夹下的日志失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
            }
            list.Add(VersionPathIndie + "logs\\latest.log");
            list.Add(VersionPathIndie + "logs\\debug.log");
            list = list.Distinct<string>().ToList<string>();
            List<string> list2 = new List<string>();
            try
            {
                foreach (string text in list)
                {
                    try
                    {
                        FileInfo info = FileUtils.GetInfo(text);
                        if (info.Exists)
                        {
                            double num = Math.Abs((info.LastWriteTime - DateTime.Now).TotalMinutes);
                            if (num < 3.0 && info.Length > 0L)
                            {
                                list2.Add(text);
                                Logger.Info(string.Format("可能可用的日志文件：{0}（{1} 分钟）", text, Math.Round(num, 1)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                            }
                        }
                    }
                    catch (Exception ex4)
                    {
                        Logger.Warn(ex4, string.Format("确认崩溃日志时间失败（{0}）", text), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                    }
                }
            }
            finally
            {
                List<string>.Enumerator enumerator3 = default;
                ((IDisposable)enumerator3).Dispose();
            }
            if (!list2.Any<string>())
            {
                Logger.Info("未发现可能可用的日志文件", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
            }
            try
            {
                foreach (string text2 in list2)
                {
                    try
                    {
                        this.AnalyzeRawFiles.Add(new KeyValuePair<string, string[]>(text2, FileUtils.ReadAsLines(text2, false, null, null)));
                    }
                    catch (Exception ex5)
                    {
                        Logger.Warn(ex5, string.Format("读取可能的崩溃日志文件失败（{0}）", text2), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                    }
                }
            }
            finally
            {
                List<string>.Enumerator enumerator4 = default;
                ((IDisposable)enumerator4).Dispose();
            }
            if (LatestLog != null && LatestLog.Any<string>())
            {
                string text3 = LatestLog.Join("\r\n");
                Logger.Info(string.Format("以下为游戏输出的最后一段内容：{0}{1}", "\r\n", text3), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                FileUtils.Write(this.TempFolder + "RawOutput.log", text3, null);
                this.AnalyzeRawFiles.Add(new KeyValuePair<string, string[]>(this.TempFolder + "RawOutput.log", LatestLog.ToArray<string>()));
                LatestLog.Clear();
            }
            Logger.Info(string.Format("步骤 1：收集日志文件完成，收集到 {0} 个文件", this.AnalyzeRawFiles.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
        }

        public void Import(string FilePath)
        {
            Logger.Info("步骤 1：自主导入日志文件", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
            try
            {
                FileInfo info = FileUtils.GetInfo(FilePath);
                if (info.Exists && info.Length > 0L && Operators.CompareString(PathUtils.GetExtension(FilePath), "jar", false) != 0)
                {
                    FileUtils.ExtractToDirectory(FilePath, this.TempFolder + "Temp\\", default(CancellationToken), null);
                    Logger.Info(string.Format("已解压导入的日志文件：{0}", FilePath), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                    goto IL_00B4;
                }
            }
            catch (Exception ex)
            {
            }
            FileUtils.Copy(FilePath, this.TempFolder + "Temp\\" + PathUtils.GetLastPart(FilePath));
            Logger.Info(string.Format("已复制导入的日志文件：{0}", FilePath), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
            IL_00B4:
            try
            {
                foreach (FileInfo fileInfo in DirectoryUtils.GetInfo(this.TempFolder + "Temp\\").EnumerateFiles())
                {
                    try
                    {
                        if (fileInfo.Exists && fileInfo.Length != 0L)
                        {
                            string text = fileInfo.Extension.Lower();
                            if (Operators.CompareString(text, ".log", false) == 0 || Operators.CompareString(text, ".txt", false) == 0)
                            {
                                this.AnalyzeRawFiles.Add(new KeyValuePair<string, string[]>(PathUtils.RemoveExtendedPrefix(fileInfo.FullName), FileUtils.ReadAsLines(fileInfo.FullName, false, null, null)));
                            }
                            else
                            {
                                FileUtils.Delete(fileInfo.FullName, false);
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        Logger.Warn(ex2, "导入单个日志文件失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                    }
                }
            }
            finally
            {
                IEnumerator<FileInfo> enumerator = default;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            Logger.Info(string.Format("步骤 1：自主导入日志文件，收集到 {0} 个文件", this.AnalyzeRawFiles.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
        }

        public bool Prepare()
        {
            Logger.Info("步骤 2：准备日志文本", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
            this.DirectFile = null;
            List<KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>>> list = new List<KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>>>();
            try
            {
                foreach (KeyValuePair<string, string[]> keyValuePair in this.AnalyzeRawFiles)
                {
                    string text = PathUtils.GetLastPart(keyValuePair.Key).Lower();
                    CrashAnalyzer.AnalyzeFileType analyzeFileType;
                    if (text.StartsWithF("hs_err", false))
                    {
                        analyzeFileType = CrashAnalyzer.AnalyzeFileType.HsErr;
                        if (this.DirectFile == null)
                        {
                            this.DirectFile = new KeyValuePair<string, string[]>?(keyValuePair);
                        }
                    }
                    else if (text.StartsWithF("crash-", false))
                    {
                        analyzeFileType = CrashAnalyzer.AnalyzeFileType.CrashReport;
                        if (this.DirectFile == null)
                        {
                            this.DirectFile = new KeyValuePair<string, string[]>?(keyValuePair);
                        }
                    }
                    else if (Operators.CompareString(text, "latest.log", false) == 0 || Operators.CompareString(text, "latest log.txt", false) == 0 || Operators.CompareString(text, "debug.log", false) == 0 || Operators.CompareString(text, "debug log.txt", false) == 0 || Operators.CompareString(text, "游戏崩溃前的输出.txt", false) == 0 || Operators.CompareString(text, "rawoutput.log", false) == 0)
                    {
                        analyzeFileType = CrashAnalyzer.AnalyzeFileType.MinecraftLog;
                        this.DirectFile = new KeyValuePair<string, string[]>?(keyValuePair);
                    }
                    else if (Operators.CompareString(text, "启动器日志.txt", false) == 0 || Operators.CompareString(text, "PCL2 启动器日志.txt", false) == 0 || Operators.CompareString(text, "PCL 启动器日志.txt", false) == 0 || Operators.CompareString(text, "log1.txt", false) == 0)
                    {
                        if (keyValuePair.Value.Any<string>((CrashAnalyzer._Closure___._I8_0 == null) ? (CrashAnalyzer._Closure___._I8_0 = new Func<string, bool>(CrashAnalyzer._Closure___._I._Lambda___8_0)) : CrashAnalyzer._Closure___._I8_0))
                        {
                            analyzeFileType = CrashAnalyzer.AnalyzeFileType.MinecraftLog;
                            if (this.DirectFile == null)
                            {
                                this.DirectFile = new KeyValuePair<string, string[]>?(keyValuePair);
                            }
                        }
                        else
                        {
                            analyzeFileType = CrashAnalyzer.AnalyzeFileType.ExtraLogFile;
                        }
                    }
                    else if (Operators.CompareString(PathUtils.GetExtension(text), "log", false) == 0)
                    {
                        analyzeFileType = CrashAnalyzer.AnalyzeFileType.ExtraLogFile;
                    }
                    else
                    {
                        if (Operators.CompareString(PathUtils.GetExtension(text), "txt", false) != 0)
                        {
                            Logger.Info(string.Format("{0} 分类为 Ignore", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                            continue;
                        }
                        analyzeFileType = CrashAnalyzer.AnalyzeFileType.ExtraReportFile;
                    }
                    if (keyValuePair.Value.Any<string>())
                    {
                        list.Add(new KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>>(analyzeFileType, keyValuePair));
                        Logger.Info(string.Format("{0} 分类为 {1}", text, analyzeFileType), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                    }
                    else
                    {
                        Logger.Info(string.Format("{0} 由于内容为空跳过", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                    }
                }
            }
            finally
            {
                List<KeyValuePair<string, string[]>>.Enumerator enumerator = default;
                ((IDisposable)enumerator).Dispose();
            }
            if (list.Any<KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>>>() && list.All<KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>>>((CrashAnalyzer._Closure___._I8_1 == null) ? (CrashAnalyzer._Closure___._I8_1 = new Func<KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>>, bool>(CrashAnalyzer._Closure___._I._Lambda___8_1)) : CrashAnalyzer._Closure___._I8_1))
            {
                Logger.Info("由于仅发现了额外日志，将它们视作 Minecraft 日志进行分析", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                list = list.Select<KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>>, KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>>>((CrashAnalyzer._Closure___._I8_2 == null) ? (CrashAnalyzer._Closure___._I8_2 = new Func<KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>>, KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>>>(CrashAnalyzer._Closure___._I._Lambda___8_2)) : CrashAnalyzer._Closure___._I8_2).ToList<KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>>>();
            }
            foreach (CrashAnalyzer.AnalyzeFileType analyzeFileType2 in new CrashAnalyzer.AnalyzeFileType[]
            {
                CrashAnalyzer.AnalyzeFileType.MinecraftLog,
                CrashAnalyzer.AnalyzeFileType.HsErr,
                CrashAnalyzer.AnalyzeFileType.ExtraLogFile,
                CrashAnalyzer.AnalyzeFileType.CrashReport
            })
            {
                List<KeyValuePair<string, string[]>> list2 = new List<KeyValuePair<string, string[]>>();
                try
                {
                    foreach (KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>> keyValuePair2 in list)
                    {
                        if (analyzeFileType2 == keyValuePair2.Key)
                        {
                            list2.Add(keyValuePair2.Value);
                        }
                    }
                }
                finally
                {
                    List<KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>>>.Enumerator enumerator2 = default;
                    ((IDisposable)enumerator2).Dispose();
                }
                if (list2.Any<KeyValuePair<string, string[]>>())
                {
                    try
                    {
                        switch (analyzeFileType2)
                        {
                        case CrashAnalyzer.AnalyzeFileType.HsErr:
                        case CrashAnalyzer.AnalyzeFileType.CrashReport:
                        {
                            SortedList<DateTime, KeyValuePair<string, string[]>> sortedList = new SortedList<DateTime, KeyValuePair<string, string[]>>();
                            try
                            {
                                foreach (KeyValuePair<string, string[]> keyValuePair3 in list2)
                                {
                                    try
                                    {
                                        sortedList.Add(FileUtils.GetInfo(keyValuePair3.Key).LastWriteTime, keyValuePair3);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Warn(ex, "获取日志文件修改时间失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                                        sortedList.Add(new DateTime(1900, 1, 1), keyValuePair3);
                                    }
                                }
                            }
                            finally
                            {
                                List<KeyValuePair<string, string[]>>.Enumerator enumerator3 = default;
                                ((IDisposable)enumerator3).Dispose();
                            }
                            KeyValuePair<string, string[]> value = sortedList.Last<KeyValuePair<DateTime, KeyValuePair<string, string[]>>>().Value;
                            this.OutputFiles.Add(value.Key);
                            if (analyzeFileType2 == CrashAnalyzer.AnalyzeFileType.HsErr)
                            {
                                this.LogHs = this.GetHeadTailLines(value.Value, 200, 100);
                                Logger.Info(string.Format("输出报告：{0}，作为虚拟机错误信息", value.Key), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                                Logger.Info(string.Format("导入分析：{0}，作为虚拟机错误信息", value.Key), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                            }
                            else
                            {
                                this.LogCrash = this.GetHeadTailLines(value.Value, 300, 700);
                                Logger.Info(string.Format("输出报告：{0}，作为 Minecraft 崩溃报告", value.Key), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                                Logger.Info(string.Format("导入分析：{0}，作为 Minecraft 崩溃报告", value.Key), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                            }
                            break;
                        }
                        case CrashAnalyzer.AnalyzeFileType.MinecraftLog:
                        {
                            this.LogMc = "";
                            this.LogMcDebug = "";
                            Dictionary<string, KeyValuePair<string, string[]>> dictionary = new Dictionary<string, KeyValuePair<string, string[]>>();
                            try
                            {
                                foreach (KeyValuePair<string, string[]> keyValuePair4 in list2)
                                {
                                    dictionary[PathUtils.GetLastPart(keyValuePair4.Key).Lower()] = keyValuePair4;
                                    this.OutputFiles.Add(keyValuePair4.Key);
                                    Logger.Info(string.Format("输出报告：{0}，作为 Minecraft 或启动器日志", keyValuePair4.Key), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                                }
                            }
                            finally
                            {
                                List<KeyValuePair<string, string[]>>.Enumerator enumerator4 = default;
                                ((IDisposable)enumerator4).Dispose();
                            }
                            foreach (string text2 in new string[] { "rawoutput.log", "启动器日志.txt", "log1.txt", "游戏崩溃前的输出.txt", "PCL2 启动器日志.txt", "PCL 启动器日志.txt" })
                            {
                                if (dictionary.ContainsKey(text2))
                                {
                                    KeyValuePair<string, string[]> keyValuePair5 = dictionary[text2];
                                    bool flag = false;
                                    foreach (string text3 in keyValuePair5.Value)
                                    {
                                        if (flag)
                                        {
                                            ref string ptr = ref this.LogMc;
                                            this.LogMc = ptr + text3 + "\n";
                                        }
                                        else if (text3.Contains("以下为游戏输出的最后一段内容"))
                                        {
                                            flag = true;
                                            Logger.Info("找到 PCL 输出的游戏实时日志头", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                                        }
                                    }
                                    if (!flag)
                                    {
                                        ref string ptr = ref this.LogMc;
                                        this.LogMc = ptr + this.GetHeadTailLines(keyValuePair5.Value, 0, 500);
                                    }
                                    this.LogMc = this.LogMc.TrimEnd("\r\n".ToCharArray());
                                    Logger.Info(string.Format("导入分析：{0}，作为启动器日志", keyValuePair5.Key), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                                    break;
                                }
                            }
                            foreach (string text4 in new string[] { "latest.log", "latest log.txt", "debug.log", "debug log.txt" })
                            {
                                if (dictionary.ContainsKey(text4))
                                {
                                    KeyValuePair<string, string[]> keyValuePair6 = dictionary[text4];
                                    ref string ptr = ref this.LogMc;
                                    this.LogMc = ptr + this.GetHeadTailLines(keyValuePair6.Value, 1500, 500);
                                    Logger.Info(string.Format("导入分析：{0}，作为 Minecraft 日志", keyValuePair6.Key), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                                    break;
                                }
                            }
                            foreach (string text5 in new string[] { "debug.log", "debug log.txt" })
                            {
                                if (dictionary.ContainsKey(text5))
                                {
                                    KeyValuePair<string, string[]> keyValuePair7 = dictionary[text5];
                                    ref string ptr = ref this.LogMcDebug;
                                    this.LogMcDebug = ptr + this.GetHeadTailLines(keyValuePair7.Value, 1000, 0);
                                    Logger.Info(string.Format("导入分析：{0}，作为 Minecraft Debug 日志", keyValuePair7.Key), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                                    break;
                                }
                            }
                            if (Operators.CompareString(this.LogMc, "", false) == 0)
                            {
                                if (Operators.CompareString(this.LogMcDebug, "", false) != 0)
                                {
                                    this.LogMc = this.LogMcDebug;
                                }
                                else
                                {
                                    if (!dictionary.Any<KeyValuePair<string, KeyValuePair<string, string[]>>>())
                                    {
                                        this.LogMc = null;
                                        throw new Exception("无法找到匹配的 Minecraft Log");
                                    }
                                    KeyValuePair<string, string[]> value3 = dictionary.First<KeyValuePair<string, KeyValuePair<string, string[]>>>().Value;
                                    ref string ptr = ref this.LogMc;
                                    this.LogMc = ptr + this.GetHeadTailLines(value3.Value, 1500, 500);
                                    Logger.Info(string.Format("导入分析：{0}，作为兜底日志", value3.Key), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                                }
                            }
                            if (Operators.CompareString(this.LogMcDebug, "", false) == 0)
                            {
                                this.LogMcDebug = null;
                            }
                            break;
                        }
                        case CrashAnalyzer.AnalyzeFileType.ExtraLogFile:
                        case CrashAnalyzer.AnalyzeFileType.ExtraReportFile:
                            try
                            {
                                foreach (KeyValuePair<string, string[]> keyValuePair8 in list2)
                                {
                                    this.OutputFiles.Add(keyValuePair8.Key);
                                    Logger.Info(string.Format("输出报告：{0}，不用作分析", keyValuePair8.Key), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                                }
                            }
                            finally
                            {
                                List<KeyValuePair<string, string[]>>.Enumerator enumerator5 = default;
                                ((IDisposable)enumerator5).Dispose();
                            }
                            break;
                        }
                    }
                    catch (Exception ex2)
                    {
                        Logger.Warn(ex2, "分类处理日志文件时出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                    }
                }
            }
            bool flag2 = this.LogMc != null || this.LogHs != null || this.LogCrash != null;
            if (flag2)
            {
                Logger.Info(string.Format("{0}用作分析", string.Concat(new string[]
                {
                    "[Crash] 步骤 2：准备日志文本完成，找到",
                    (this.LogMc == null) ? "" : "游戏日志、",
                    (this.LogMcDebug == null) ? "" : "游戏 Debug 日志、",
                    (this.LogHs == null) ? "" : "虚拟机日志、",
                    (this.LogCrash == null) ? "" : "崩溃日志、"
                }).TrimEnd(new char[] { '、' })), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
            }
            else
            {
                Logger.Info("步骤 2：准备日志文本完成，没有任何可供分析的日志", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
            }
            return flag2;
        }

        private string GetHeadTailLines(string[] Raw, int HeadLines, int TailLines)
        {
            checked
            {
                string text;
                if (Raw.Length <= HeadLines + TailLines)
                {
                    text = Raw.Distinct<string>().Join("\n");
                }
                else
                {
                    List<string> list = new List<string>();
                    int num = 0;
                    int num2 = Raw.Length - 1;
                    int i;
                    for (i = 0; i <= num2; i++)
                    {
                        if (!list.Contains(Raw[i]))
                        {
                            num++;
                            list.Add(Raw[i]);
                            if (num >= HeadLines)
                            {
                                break;
                            }
                        }
                    }
                    int num3 = 0;
                    int num4 = Raw.Length - 1;
                    int num5 = i;
                    for (int j = num4; j >= num5; j += -1)
                    {
                        if (!list.Contains(Raw[j]))
                        {
                            num3++;
                            list.Insert(num, Raw[j]);
                            if (num3 >= TailLines)
                            {
                                break;
                            }
                        }
                    }
                    StringBuilder stringBuilder = new StringBuilder();
                    try
                    {
                        foreach (string text2 in list)
                        {
                            if (Operators.CompareString(text2, "", false) != 0)
                            {
                                stringBuilder.Append(text2);
                                stringBuilder.Append("\n");
                            }
                        }
                    }
                    finally
                    {
                        List<string>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                    text = stringBuilder.ToString();
                }
                return text;
            }
        }

        public void Analyze()
        {
            Logger.Info("步骤 3：分析崩溃原因", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
            string text;
            if ((text = this.LogMc) == null)
            {
                text = this.LogMcDebug ?? "";
            }
            this.LogAll = text + this.LogHs + this.LogCrash;
            this.AnalyzeCrit1();
            if (!this.CrashReasons.Any<KeyValuePair<CrashAnalyzer.CrashReason, List<string>>>())
            {
                this.AnalyzeCrit2();
                if (!this.CrashReasons.Any<KeyValuePair<CrashAnalyzer.CrashReason, List<string>>>())
                {
                    if (this.LogAll.Contains("orge") || this.LogAll.Contains("abric") || this.LogAll.Contains("uilt") || this.LogAll.Contains("iteloader"))
                    {
                        List<string> list = new List<string>();
                        if (this.LogCrash != null)
                        {
                            Logger.Info("开始进行崩溃日志堆栈分析", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                            list.AddRange(this.AnalyzeStackKeyword(this.LogCrash.BeforeFirst("System Details", false)));
                        }
                        if (this.LogMc != null)
                        {
                            List<string> list2 = this.LogMc.RegexSearch("/FATAL] .+?(?=[\\n]+\\[)", RegexOptions.None).ToList<string>();
                            if (this.LogMc.Contains("Unreported exception thrown!"))
                            {
                                list2.Add(this.LogMc.Between("Unreported exception thrown!", "at oolloo.jlw.Wrapper", false));
                            }
                            Logger.Info(string.Format("开始进行 Minecraft 日志堆栈分析，发现 {0} 个报错项", list2.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                            try
                            {
                                foreach (string text2 in list2)
                                {
                                    list.AddRange(this.AnalyzeStackKeyword(text2));
                                }
                            }
                            finally
                            {
                                List<string>.Enumerator enumerator = default;
                                ((IDisposable)enumerator).Dispose();
                            }
                        }
                        if (this.LogHs != null)
                        {
                            Logger.Info("开始进行虚拟机堆栈分析", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                            string text3 = this.LogHs.Between("T H R E A D", "Registers:", false);
                            list.AddRange(this.AnalyzeStackKeyword(text3));
                        }
                        if (list.Any<string>())
                        {
                            List<string> list3 = this.AnalyzeModName(list);
                            if (list3 == null)
                            {
                                this.AppendReason(CrashAnalyzer.CrashReason.堆栈分析发现关键字, list);
                                goto IL_0219;
                            }
                            this.AppendReason(CrashAnalyzer.CrashReason.堆栈分析发现Mod名称, list3);
                            goto IL_0219;
                        }
                    }
                    else
                    {
                        Logger.Info("可能并未安装 Mod，不进行堆栈分析", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                    }
                    this.AnalyzeCrit3();
                }
            }
            IL_0219:
            if (!this.CrashReasons.Any<KeyValuePair<CrashAnalyzer.CrashReason, List<string>>>())
            {
                Logger.Info("步骤 3：分析崩溃原因完成，未找到可能的原因", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                return;
            }
            Logger.Info(string.Format("步骤 3：分析崩溃原因完成，找到 {0} 条可能的原因", this.CrashReasons.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
            try
            {
                foreach (KeyValuePair<CrashAnalyzer.CrashReason, List<string>> keyValuePair in this.CrashReasons)
                {
                    Logger.Info(string.Format("- {0}{1}", keyValuePair.Key, keyValuePair.Value.Any<string>() ? ("（" + keyValuePair.Value.Join('；') + "）") : ""), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                }
            }
            finally
            {
                Dictionary<CrashAnalyzer.CrashReason, List<string>>.Enumerator enumerator2 = default;
                ((IDisposable)enumerator2).Dispose();
            }
        }

        private void AppendReason(CrashAnalyzer.CrashReason Reason, IEnumerable<string> Additional = null)
        {
            if (this.CrashReasons.ContainsKey(Reason))
            {
                if (Additional != null)
                {
                    this.CrashReasons[Reason].AddRange(Additional);
                    this.CrashReasons[Reason] = this.CrashReasons[Reason].Distinct<string>().ToList<string>();
                }
            }
            else
            {
                this.CrashReasons.Add(Reason, new List<string>((Additional ?? new string[0]).ToList<string>()));
            }
            Logger.Info(string.Format("可能的崩溃原因：{0}{1}", Reason, (Additional != null && Additional.Any<string>()) ? ("（" + Additional.Join('；') + "）") : ""), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
        }

        private void AppendReason(CrashAnalyzer.CrashReason Reason, string Additional)
        {
            List<string> list;
            if (!string.IsNullOrEmpty(Additional))
            {
                (list = new List<string>()).Add(Additional);
            }
            else
            {
                list = null;
            }
            this.AppendReason(Reason, list);
        }

        private void AnalyzeCrit1()
        {
            if (this.LogMc == null && this.LogHs == null && this.LogCrash == null)
            {
                this.AppendReason(CrashAnalyzer.CrashReason.没有可用的分析文件, (IEnumerable<string>)null);
                return;
            }
            if (this.LogCrash != null)
            {
                if (this.LogCrash.Contains("Unable to make protected final java.lang.Class java.lang.ClassLoader.defineClass"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Java版本过高, (IEnumerable<string>)null);
                }
                if (this.LogCrash.Contains("Failed loading config file "))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Mod配置文件导致游戏崩溃, new string[]
                    {
                        this.TryAnalyzeModName((this.LogCrash.RegexSeek("(?<=Failed loading config file .+ for modid )[^\\n]+", RegexOptions.None) ?? "").TrimEnd(new char[] { '\r' })).First<string>(),
                        (this.LogCrash.RegexSeek("(?<=Failed loading config file ).+(?= of type)", RegexOptions.None) ?? "").TrimEnd(new char[] { '\r' })
                    });
                }
            }
            if (this.LogMc != null)
            {
                if (this.LogMc.Contains("Unrecognized option:"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Java虚拟机参数有误, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("Found multiple arguments for option fml.forgeVersion, but you asked for only one"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.版本Json中存在多个Forge, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("The driver does not appear to support OpenGL"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.显卡不支持OpenGL, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.ClassCastException: java.base/jdk"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.使用JDK, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.ClassCastException: class jdk."))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.使用JDK, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.NoSuchMethodError: 'void net.minecraft.client.renderer.texture.SpriteContents.<init>"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.OptiFine与Forge不兼容, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.NoSuchMethodError: 'java.lang.String com.mojang.blaze3d.systems.RenderSystem.getBackendDescription"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.OptiFine与Forge不兼容, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.NoSuchMethodError: 'void net.minecraft.client.renderer.block.model.BakedQuad.<init>"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.OptiFine与Forge不兼容, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.NoSuchMethodError: 'void net.minecraftforge.client.gui.overlay.ForgeGui.renderSelectedItemName"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.OptiFine与Forge不兼容, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.NoSuchMethodError: 'void net.minecraft.server.level.DistanceManager"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.OptiFine与Forge不兼容, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.NoSuchMethodError: 'net.minecraft.network.chat.FormattedText net.minecraft.client.gui.Font.ellipsize"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.OptiFine与Forge不兼容, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("Open J9 is not supported") || this.LogMc.Contains("OpenJ9 is incompatible") || this.LogMc.Contains(".J9VMInternals."))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.使用OpenJ9, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.NoSuchFieldException: ucp"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Java版本过高, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("because module java.base does not export"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Java版本过高, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.ClassNotFoundException: jdk.nashorn.api.scripting.NashornScriptEngineFactory"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Java版本过高, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.ClassNotFoundException: java.lang.invoke.LambdaMetafactory"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Java版本过高, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("The directories below appear to be extracted jar files. Fix this before you continue."))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Mod文件被解压, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("Extracted mod jars found, loading will NOT continue"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Mod文件被解压, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.ClassNotFoundException: org.spongepowered.asm.launch.MixinTweaker"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.MixinBootstrap缺失, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("Couldn't set pixel format"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.显卡驱动不支持导致无法设置像素格式, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.OutOfMemoryError") || this.LogMc.Contains("an out of memory error"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.内存不足, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.RuntimeException: Shaders Mod detected. Please remove it, OptiFine has built-in support for shaders."))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.ShadersMod与OptiFine同时安装, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.NoSuchMethodError: sun.security.util.ManifestEntryVerifier") || this.LogMc.Contains("java.lang.NoSuchMethodError: 'void sun.security.util.ManifestEntryVerifier"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.低版本Forge与高版本Java不兼容, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("1282: Invalid operation"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.光影或资源包导致OpenGL1282错误, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("signer information does not match signer information of other classes in the same package"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.文件或内容校验失败, (this.LogMc.RegexSeek("(?<=class \")[^']+(?=\"'s signer information)", RegexOptions.None) ?? "").TrimEnd(new char[] { '\r' }));
                }
                if (this.LogMc.Contains("Maybe try a lower resolution resourcepack?"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.材质过大或显卡配置不足, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.NoSuchMethodError: net.minecraft.world.server.ChunkManager$ProxyTicketManager.shouldForceTicks(J)Z") && this.LogMc.Contains("OptiFine"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.OptiFine导致无法加载世界, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("com.electronwill.nightconfig.core.io.ParsingException: Not enough data available") && !this.CrashReasons.ContainsKey(CrashAnalyzer.CrashReason.Mod配置文件导致游戏崩溃))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.NightConfig的Bug, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("Cannot find launch target fmlclient, unable to launch"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Forge安装不完整, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("Invalid paths argument, contained no existing paths") && this.LogMc.Contains("libraries\\net\\minecraftforge\\fmlcore"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Forge安装不完整, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("Invalid module name: '' is not a Java identifier"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Mod名称包含特殊字符, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("has been compiled by a more recent version of the Java Runtime (class file version 55.0), this version of the Java Runtime only recognizes class file versions up to"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Mod需要Java11, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.RuntimeException: java.lang.NoSuchMethodException: no such method: sun.misc.Unsafe.defineAnonymousClass(Class,byte[],Object[])Class/invokeVirtual"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Mod需要Java11, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("java.lang.IllegalArgumentException: The requested compatibility level JAVA_11 could not be set. Level is not supported by the active JRE or ASM version"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Mod需要Java11, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("Unsupported class file major version"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Java版本不兼容, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("Unsupported major.minor version"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Java版本不兼容, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("Level is not supported by the active JRE or ASM version"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Java版本不兼容, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("Invalid maximum heap size"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.使用32位Java导致JVM无法分配足够多的内存, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("Could not reserve enough space"))
                {
                    if (this.LogMc.Contains("for 1048576KB object heap"))
                    {
                        this.AppendReason(CrashAnalyzer.CrashReason.使用32位Java导致JVM无法分配足够多的内存, (IEnumerable<string>)null);
                    }
                    else
                    {
                        this.AppendReason(CrashAnalyzer.CrashReason.内存不足, (IEnumerable<string>)null);
                    }
                }
                if (this.LogMc.Contains("Caught exception from "))
                {
                    CrashAnalyzer.CrashReason crashReason = CrashAnalyzer.CrashReason.确定Mod导致游戏崩溃;
                    string text = this.LogMc.RegexSeek("(?<=Caught exception from )[^\\n]+?", RegexOptions.None);
                    this.AppendReason(crashReason, this.TryAnalyzeModName((text != null) ? text.TrimEnd("\r\n ".ToCharArray()) : null));
                }
                if (this.LogMc.Contains("DuplicateModsFoundException"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Mod重复安装, this.LogMc.RegexSearch("(?<=\\n\\t[\\w]+ : [A-Z]:[^\\n]+(/|\\\\))[^/\\\\\\n]+?.jar", RegexOptions.IgnoreCase));
                }
                if (this.LogMc.Contains("Found a duplicate mod"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Mod重复安装, (this.LogMc.RegexSeek("Found a duplicate mod[^\\n]+", RegexOptions.None) ?? "").RegexSearch("[^\\\\/]+.jar", RegexOptions.IgnoreCase));
                }
                if (this.LogMc.Contains("Found duplicate mods"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Mod重复安装, this.LogMc.RegexSearch("(?<=Mod ID: ')\\w+?(?=' from mod files:)", RegexOptions.None).Distinct<string>().ToList<string>());
                }
                if (this.LogMc.Contains("ModResolutionException: Duplicate"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Mod重复安装, (this.LogMc.RegexSeek("ModResolutionException: Duplicate[^\\n]+", RegexOptions.None) ?? "").RegexSearch("[^\\\\/]+.jar", RegexOptions.IgnoreCase));
                }
                if (this.LogMc.Contains("Incompatible mods found!"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Mod互不兼容, (this.LogMc.RegexSeek("(?<=Incompatible mods found![\\s\\S]+: )[\\s\\S]+?(?=\\tat )", RegexOptions.None) ?? "").BeforeFirst("更多信息：", false).Replace("Some of your mods are incompatible with the game or each other!", "").Trim("\r\n ".ToCharArray()));
                }
                if (this.LogMc.Contains("Missing or unsupported mandatory dependencies:"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Mod缺少前置或MC版本错误, this.LogMc.RegexSearch("(?<=Missing or unsupported mandatory dependencies:)([\\n\\r]+\\t(.*))+", RegexOptions.IgnoreCase).Select<string, string>((CrashAnalyzer._Closure___._I20_0 == null) ? (CrashAnalyzer._Closure___._I20_0 = new Func<string, string>(CrashAnalyzer._Closure___._I._Lambda___20_0)) : CrashAnalyzer._Closure___._I20_0).Distinct<string>()
                        .ToList<string>());
                }
            }
            if (this.LogHs != null)
            {
                if (this.LogHs.Contains("The system is out of physical RAM or swap space"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.内存不足, (IEnumerable<string>)null);
                }
                if (this.LogHs.Contains("Out of Memory Error"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.内存不足, (IEnumerable<string>)null);
                }
                if (this.LogHs.Contains("EXCEPTION_ACCESS_VIOLATION"))
                {
                    if (this.LogHs.Contains("# C  [ig"))
                    {
                        this.AppendReason(CrashAnalyzer.CrashReason.Intel驱动不兼容导致EXCEPTION_ACCESS_VIOLATION, (IEnumerable<string>)null);
                    }
                    if (this.LogHs.Contains("# C  [atio"))
                    {
                        this.AppendReason(CrashAnalyzer.CrashReason.AMD驱动不兼容导致EXCEPTION_ACCESS_VIOLATION, (IEnumerable<string>)null);
                    }
                    if (this.LogHs.Contains("# C  [nvoglv"))
                    {
                        this.AppendReason(CrashAnalyzer.CrashReason.Nvidia驱动不兼容导致EXCEPTION_ACCESS_VIOLATION, (IEnumerable<string>)null);
                    }
                }
            }
            if (this.LogCrash != null)
            {
                if (this.LogCrash.Contains("maximum id range exceeded"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Mod过多导致超出ID限制, (IEnumerable<string>)null);
                }
                if (this.LogCrash.Contains("java.lang.OutOfMemoryError"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.内存不足, (IEnumerable<string>)null);
                }
                if (this.LogCrash.Contains("Pixel format not accelerated"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.显卡驱动不支持导致无法设置像素格式, (IEnumerable<string>)null);
                }
                if (this.LogCrash.Contains("Manually triggered debug crash"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.玩家手动触发调试崩溃, (IEnumerable<string>)null);
                }
                if (this.LogCrash.Contains("has mods that were not found") && this.LogCrash.RegexCheck("The Mod File [^\\n]+optifine\\\\OptiFine[^\\n]+ has mods that were not found", RegexOptions.None))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.OptiFine与Forge不兼容, (IEnumerable<string>)null);
                }
                if (this.LogCrash.Contains("-- MOD "))
                {
                    string text2 = this.LogCrash.Between("-- MOD ", "Failure message:", false);
                    if (text2.ContainsIgnoreCase(".jar"))
                    {
                        this.AppendReason(CrashAnalyzer.CrashReason.确定Mod导致游戏崩溃, (text2.RegexSeek("(?<=Mod File: ).+", RegexOptions.None) ?? "").TrimEnd("\r\n ".ToCharArray()));
                    }
                    else
                    {
                        this.AppendReason(CrashAnalyzer.CrashReason.Mod加载器报错, (this.LogCrash.RegexSeek("(?<=Failure message: )[\\w\\W]+?(?=\\tMod)", RegexOptions.None) ?? "").Replace("\t", " ").TrimEnd("\r\n ".ToCharArray()));
                    }
                }
                if (this.LogCrash.Contains("Multiple entries with same key: "))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.确定Mod导致游戏崩溃, this.TryAnalyzeModName((this.LogCrash.RegexSeek("(?<=Multiple entries with same key: )[^=]+", RegexOptions.None) ?? "").TrimEnd("\r\n ".ToCharArray())));
                }
                if (this.LogCrash.Contains("LoaderExceptionModCrash: Caught exception from "))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.确定Mod导致游戏崩溃, this.TryAnalyzeModName((this.LogCrash.RegexSeek("(?<=LoaderExceptionModCrash: Caught exception from )[^\\n]+", RegexOptions.None) ?? "").TrimEnd("\r\n ".ToCharArray())));
                }
                if (this.LogCrash.Contains("Failed loading config file "))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Mod配置文件导致游戏崩溃, new string[]
                    {
                        this.TryAnalyzeModName((this.LogCrash.RegexSeek("(?<=Failed loading config file .+ for modid )[^\\n]+", RegexOptions.None) ?? "").TrimEnd(new char[] { '\r' })).First<string>(),
                        (this.LogCrash.RegexSeek("(?<=Failed loading config file ).+(?= of type)", RegexOptions.None) ?? "").TrimEnd(new char[] { '\r' })
                    });
                }
            }
        }

        private void AnalyzeCrit2()
        {
            VB_AnonymousDelegate_9<string, bool> vb_AnonymousDelegate_ = new VB_AnonymousDelegate_9<string, bool>(this._Lambda___21_0);
            if (this.LogMc != null)
            {
                bool flag = vb_AnonymousDelegate_(this.LogMc);
                if (this.LogMc.Contains("An exception was thrown, the game will display an error screen and halt."))
                {
                    CrashAnalyzer.CrashReason crashReason = CrashAnalyzer.CrashReason.Forge报错;
                    string text = this.LogMc.RegexSeek("(?<=the game will display an error screen and halt.[\\n\\r]+[^\\n]+?Exception: )[\\s\\S]+?(?=\\n\\tat)", RegexOptions.None);
                    this.AppendReason(crashReason, (text != null) ? text.Trim(new char[] { '\r' }) : null);
                }
                if (this.LogMc.Contains("A potential solution has been determined:"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Fabric报错并给出解决方案, (this.LogMc.RegexSeek("(?<=A potential solution has been determined:\\n)(\\s+ - [^\\n]+\\n)+", RegexOptions.None) ?? "").RegexSearch("(?<=\\s+)[^\\n]+", RegexOptions.None).Join("\n"));
                }
                if (this.LogMc.Contains("A potential solution has been determined, this may resolve your problem:"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Fabric报错并给出解决方案, (this.LogMc.RegexSeek("(?<=A potential solution has been determined, this may resolve your problem:\\n)(\\s+ - [^\\n]+\\n)+", RegexOptions.None) ?? "").RegexSearch("(?<=\\s+)[^\\n]+", RegexOptions.None).Join("\n"));
                }
                if (this.LogMc.Contains("确定了一种可能的解决方法，这样做可能会解决你的问题："))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Fabric报错并给出解决方案, (this.LogMc.RegexSeek("(?<=确定了一种可能的解决方法，这样做可能会解决你的问题：\\n)(\\s+ - [^\\n]+\\n)+", RegexOptions.None) ?? "").RegexSearch("(?<=\\s+)[^\\n]+", RegexOptions.None).Join("\n"));
                }
                if (!flag && this.LogMc.Contains("due to errors, provided by "))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.确定Mod导致游戏崩溃, this.TryAnalyzeModName((this.LogMc.RegexSeek("(?<=due to errors, provided by ')[^']+", RegexOptions.None) ?? "").TrimEnd("\r\n ".ToCharArray())));
                }
            }
            if (this.LogCrash != null)
            {
                vb_AnonymousDelegate_(this.LogCrash);
                if (this.LogCrash.Contains("Suspected Mod"))
                {
                    string text2 = this.LogCrash.Between("Suspected Mod", "Stacktrace", false);
                    if (!text2.StartsWithF("s: None", false))
                    {
                        IEnumerable<string> enumerable = text2.RegexSearch("(?<=\\n\\t[^(\\t]+\\()[^)\\n]+", RegexOptions.None);
                        if (enumerable.Any<string>())
                        {
                            this.AppendReason(CrashAnalyzer.CrashReason.怀疑Mod导致游戏崩溃, this.TryAnalyzeModName(enumerable));
                        }
                    }
                }
            }
        }

        private void AnalyzeCrit3()
        {
            if (this.LogMc != null)
            {
                if (!this.LogMc.Contains("at net.") && !this.LogMc.Contains("INFO]") && this.LogHs == null && this.LogCrash == null && this.LogMc.Length < 100)
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.极短的程序输出, this.LogMc);
                }
                if (this.LogMc.Contains("Mod resolution failed"))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.Mod加载器报错, (IEnumerable<string>)null);
                }
                if (this.LogMc.Contains("Failed to create mod instance."))
                {
                    CrashAnalyzer.CrashReason crashReason = CrashAnalyzer.CrashReason.Mod初始化失败;
                    string text;
                    if ((text = this.LogMc.RegexSeek("(?<=Failed to create mod instance. ModID: )[^,]+", RegexOptions.None)) == null)
                    {
                        text = this.LogMc.RegexSeek("(?<=Failed to create mod instance. ModId )[^\\n]+(?= for )", RegexOptions.None) ?? "";
                    }
                    this.AppendReason(crashReason, this.TryAnalyzeModName(text.TrimEnd(new char[] { '\r' })));
                }
            }
            if (this.LogCrash != null)
            {
                if (this.LogCrash.Contains("\tBlock location: World: "))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.特定方块导致崩溃, this.LogCrash.RegexSeek("(?<=\\tBlock: Block\\{)[^\\}]+", RegexOptions.None) + " " + this.LogCrash.RegexSeek("(?<=\\tBlock location: World: )\\([^\\)]+\\)", RegexOptions.None));
                }
                if (this.LogCrash.Contains("\tEntity's Exact location: "))
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.特定实体导致崩溃, this.LogCrash.RegexSeek("(?<=\\tEntity Type: )[^\\n]+(?= \\()", RegexOptions.None) + " (" + (this.LogCrash.RegexSeek("(?<=\\tEntity's Exact location: )[^\\n]+", RegexOptions.None) ?? "").TrimEnd("\r\n".ToCharArray()) + ")");
                }
            }
        }

        private List<string> AnalyzeStackKeyword(string ErrorStack)
        {
            ErrorStack = "\n" + ErrorStack + "\n";
            List<string> list = new List<string>();
            list.AddRange(ErrorStack.RegexSearch("(?<=\\n[^{]+)[a-zA-Z_]+\\w+\\.[a-zA-Z_]+[\\w\\.]+(?=\\.[\\w\\.$]+\\.)", RegexOptions.None));
            list.AddRange(ErrorStack.RegexSearch("(?<=at [^(]+?\\.\\w+\\$\\w+\\$)[\\w\\$]+?(?=\\$\\w+\\()", RegexOptions.None).Select<string, string>((CrashAnalyzer._Closure___._I23_0 == null) ? (CrashAnalyzer._Closure___._I23_0 = new Func<string, string>(CrashAnalyzer._Closure___._I._Lambda___23_0)) : CrashAnalyzer._Closure___._I23_0));
            list = list.Distinct<string>().ToList<string>();
            List<string> list2 = new List<string>();
            try
            {
                List<string>.Enumerator enumerator = list.GetEnumerator();
                IL_0174:
                while (enumerator.MoveNext())
                {
                    string text = enumerator.Current;
                    foreach (string text2 in new string[]
                    {
                        "java", "sun", "javax", "jdk", "oolloo", "org.lwjgl", "com.sun", "net.minecraftforge", "paulscode.sound", "com.mojang",
                        "net.minecraft", "cpw.mods", "com.google", "org.apache", "org.spongepowered", "net.fabricmc", "com.mumfrey", "com.electronwill.nightconfig", "it.unimi.dsi", "MojangTricksIntelDriversForPerformance_javaw"
                    })
                    {
                        if (text.StartsWithF(text2, false))
                        {
                            goto IL_0174;
                        }
                    }
                    list2.Add(text.Trim());
                }
            }
            finally
            {
                List<string>.Enumerator enumerator = default;
                ((IDisposable)enumerator).Dispose();
            }
            list2 = list2.Distinct<string>().ToList<string>();
            Logger.Info(string.Format("找到 {0} 条可能的堆栈信息", list2.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
            checked
            {
                List<string> list3;
                if (!list2.Any<string>())
                {
                    list3 = new List<string>();
                }
                else
                {
                    try
                    {
                        foreach (string text3 in list2)
                        {
                            Logger.Info(string.Format("- {0}", text3), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                        }
                    }
                    finally
                    {
                        List<string>.Enumerator enumerator2 = default;
                        ((IDisposable)enumerator2).Dispose();
                    }
                    List<string> list4 = new List<string>();
                    try
                    {
                        foreach (string text4 in list2)
                        {
                            string[] array2 = text4.Split(".", false);
                            int num = Math.Min(3, array2.Count<string>() - 1);
                            for (int j = 0; j <= num; j++)
                            {
                                string text5 = array2[j];
                                if (text5.Length > 2 && !text5.StartsWithF("func_", false) && !new string[]
                                {
                                    "com", "org", "net", "asm", "fml", "mod", "jar", "sun", "lib", "map",
                                    "gui", "dev", "nio", "api", "dsi", "top", "mcp", "core", "init", "mods",
                                    "main", "file", "game", "load", "read", "done", "util", "tile", "item", "base",
                                    "fake", "oshi", "impl", "data", "pool", "task", "forge", "setup", "block", "model",
                                    "mixin", "event", "unimi", "netty", "world", "lwjgl", "fakes", "fabric", "gitlab", "common",
                                    "server", "config", "mixins", "compat", "loader", "launch", "script", "entity", "assist", "client",
                                    "plugin", "modapi", "mojang", "shader", "events", "github", "recipe", "render", "packet", "events",
                                    "preinit", "preload", "machine", "reflect", "channel", "general", "handler", "content", "systems", "modules",
                                    "service", "scripts", "network", "fastutil", "optifine", "internal", "platform", "override", "fabricmc", "neoforge",
                                    "external", "injection", "listeners", "scheduler", "minecraft", "universal", "multipart", "neoforged", "microsoft", "transformer",
                                    "transformers", "minecraftforge", "blockentity", "spongepowered", "electronwill", "concurrent"
                                }.Contains(text5.Lower()))
                                {
                                    list4.Add(text5.Trim());
                                }
                            }
                        }
                    }
                    finally
                    {
                        List<string>.Enumerator enumerator3 = default;
                        ((IDisposable)enumerator3).Dispose();
                    }
                    list4 = list4.Distinct<string>().ToList<string>();
                    Logger.Info(string.Format("从堆栈信息中找到 {0} 个可能的 Mod ID 关键词", list4.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                    if (list4.Any<string>())
                    {
                        Logger.Info(string.Format("- {0}", list4.Join(", ")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                    }
                    if (list4.Count > 10)
                    {
                        Logger.Info("关键词过多，考虑匹配出错，不纳入考虑", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                        list3 = new List<string>();
                    }
                    else
                    {
                        list3 = list4;
                    }
                }
                return list3;
            }
        }

        private List<string> AnalyzeModName(IEnumerable<string> Keywords)
        {
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            try
            {
                foreach (string text in Keywords)
                {
                    foreach (string text2 in text.Split("(", false))
                    {
                        list2.Add(text2.Trim(" )".ToCharArray()));
                    }
                }
            }
            finally
            {
                IEnumerator<string> enumerator = default;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            Keywords = list2;
            if (this.LogCrash != null && this.LogCrash.Contains("A detailed walkthrough of the error"))
            {
                string text3 = this.LogCrash.Replace("A detailed walkthrough of the error", "\u00a8");
                bool flag = text3.Contains("Fabric Mods");
                if (flag)
                {
                    text3 = text3.Replace("Fabric Mods", "\u00a8");
                    Logger.Info("崩溃报告中检测到 Fabric Mod 信息格式", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                }
                text3 = text3.AfterLast("\u00a8", false);
                List<string> list3 = new List<string>();
                foreach (string text4 in text3.Split("\n", false))
                {
                    if ((text4.ContainsIgnoreCase(".jar") && checked(text4.Length - text4.Replace(".jar", "").Length) == 4) || (flag && text4.StartsWithF("\t\t", false) && !text4.RegexCheck("\\t\\tfabric[\\w-]*: Fabric", RegexOptions.None)))
                    {
                        list3.Add(text4);
                    }
                }
                Logger.Info(string.Format("崩溃报告中找到 {0} 个可能的 Mod 项目行", list3.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                List<string> list4 = new List<string>();
                try
                {
                    foreach (string text5 in Keywords)
                    {
                        try
                        {
                            foreach (string text6 in list3)
                            {
                                string text7 = text6.Lower().Replace("_", "");
                                if (text7.Contains(text5.Lower().Replace("_", "")) && !text7.Contains("minecraft.jar") && !text7.Contains(" forge-") && !text7.Contains(" mixin-"))
                                {
                                    list4.Add(text6.Trim("\r\n".ToCharArray()));
                                    break;
                                }
                            }
                        }
                        finally
                        {
                            List<string>.Enumerator enumerator3 = default;
                            ((IDisposable)enumerator3).Dispose();
                        }
                    }
                }
                finally
                {
                    IEnumerator<string> enumerator2 = default;
                    if (enumerator2 != null)
                    {
                        enumerator2.Dispose();
                    }
                }
                list4 = list4.Distinct<string>().ToList<string>();
                Logger.Info(string.Format("崩溃报告中找到 {0} 个可能的崩溃 Mod 匹配行", list4.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                try
                {
                    foreach (string text8 in list4)
                    {
                        Logger.Info(string.Format("- {0}", text8), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                    }
                }
                finally
                {
                    List<string>.Enumerator enumerator4 = default;
                    ((IDisposable)enumerator4).Dispose();
                }
                try
                {
                    foreach (string text9 in list4)
                    {
                        string text10;
                        if (flag)
                        {
                            text10 = text9.RegexSeek("(?<=: )[^\\n]+(?= [^\\n]+)", RegexOptions.None);
                        }
                        else
                        {
                            text10 = text9.RegexSeek("(?<=\\()[^\\t]+.jar(?=\\))|(?<=(\\t\\t)|(\\| ))[^\\t\\|]+.jar", RegexOptions.IgnoreCase);
                        }
                        if (text10 != null)
                        {
                            list.Add(text10);
                        }
                    }
                }
                finally
                {
                    List<string>.Enumerator enumerator5 = default;
                    ((IDisposable)enumerator5).Dispose();
                }
            }
            if (this.LogMcDebug != null)
            {
                IEnumerable<string> enumerable = this.LogMcDebug.RegexSearch("(?<=valid mod file ).*", RegexOptions.Multiline);
                Logger.Info(string.Format("Debug 信息中找到 {0} 个可能的 Mod 项目行", enumerable.Count<string>()), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                List<string> list5 = new List<string>();
                try
                {
                    foreach (string text11 in Keywords)
                    {
                        try
                        {
                            foreach (string text12 in enumerable)
                            {
                                if (text12.Contains(string.Format("{{{0}}}", text11)))
                                {
                                    list5.Add(text12);
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator<string> enumerator7 = default;
                            if (enumerator7 != null)
                            {
                                enumerator7.Dispose();
                            }
                        }
                    }
                }
                finally
                {
                    IEnumerator<string> enumerator6 = default;
                    if (enumerator6 != null)
                    {
                        enumerator6.Dispose();
                    }
                }
                list5 = list5.Distinct<string>().ToList<string>();
                Logger.Info(string.Format("Debug 信息中找到 {0} 个可能的崩溃 Mod 匹配行", list5.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                try
                {
                    foreach (string text13 in list5)
                    {
                        Logger.Info(string.Format("- {0}", text13), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                    }
                }
                finally
                {
                    List<string>.Enumerator enumerator8 = default;
                    ((IDisposable)enumerator8).Dispose();
                }
                try
                {
                    foreach (string text14 in list5)
                    {
                        string text15 = text14.RegexSeek(".*(?= with)", RegexOptions.None);
                        if (text15 != null)
                        {
                            list.Add(text15);
                        }
                    }
                }
                finally
                {
                    List<string>.Enumerator enumerator9 = default;
                    ((IDisposable)enumerator9).Dispose();
                }
            }
            list = list.Distinct<string>().ToList<string>();
            List<string> list6;
            if (!list.Any<string>())
            {
                list6 = null;
            }
            else
            {
                Logger.Info(string.Format("找到 {0} 个可能的崩溃 Mod 文件名", list.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                try
                {
                    foreach (string text16 in list)
                    {
                        Logger.Info(string.Format("- {0}", text16), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                    }
                }
                finally
                {
                    List<string>.Enumerator enumerator10 = default;
                    ((IDisposable)enumerator10).Dispose();
                }
                list6 = list;
            }
            return list6;
        }

        private List<string> TryAnalyzeModName(string Keyword)
        {
            List<string> list = new List<string> { Keyword ?? "" };
            List<string> list2;
            if (string.IsNullOrEmpty(Keyword))
            {
                list2 = list;
            }
            else
            {
                list2 = this.AnalyzeModName(list) ?? list;
            }
            return list2;
        }

        private List<string> TryAnalyzeModName(IEnumerable<string> Keywords)
        {
            List<string> list;
            if (!Keywords.Any<string>())
            {
                list = Keywords.ToList<string>();
            }
            else
            {
                list = this.AnalyzeModName(Keywords) ?? Keywords.ToList<string>();
            }
            return list;
        }

        public void Output(bool IsManualAnalyze, List<string> ExtraFiles = null)
        {
            ModMain.FrmMain.ShowWindowToTop();
            string analyzeResult = this.GetAnalyzeResult(IsManualAnalyze);
            string text = analyzeResult;
            string text2 = (IsManualAnalyze ? "错误报告分析结果" : "Minecraft 出现错误");
            string text3 = "确定";
            string text4 = ((IsManualAnalyze || this.DirectFile == null) ? "" : "查看输出");
            string text5 = (IsManualAnalyze ? "" : "导出错误报告");
            bool flag = false;
            bool flag2 = true;
            bool flag3 = false;
            Action action = null;
            VB_AnonymousDelegate_10 vb_AnonymousDelegate_ = ((IsManualAnalyze || this.DirectFile == null) ? null : new VB_AnonymousDelegate_10(this._Lambda___28_0));
            int num = ModMain.OurMsgBox(text, text2, text3, text4, text5, flag, flag2, flag3, action, (vb_AnonymousDelegate_ == null) ? null : new Action(vb_AnonymousDelegate_.Invoke), null);
            if (num == 3)
            {
                CrashAnalyzer._Closure___28_0 CS_locals1 = new CrashAnalyzer._Closure___28_0(null);
                CS_locals1._VB_Local_FileAddress = null;
                try
                {
                    ModBase.RunInUiWait(new Action(CS_locals1._Lambda___1));
                    if (CS_locals1._VB_Local_FileAddress == null)
                    {
                        return;
                    }
                    FileUtils.Delete(CS_locals1._VB_Local_FileAddress, false);
                    ModMain.FeedbackInfo();
                    Logger.Instance.Flush();
                    if (ExtraFiles != null)
                    {
                        this.OutputFiles.AddRange(ExtraFiles);
                    }
                    try
                    {
                        foreach (string text6 in this.OutputFiles)
                        {
                            string text7 = PathUtils.GetLastPart(text6);
                            Encoding encoding = null;
                            if (Operators.CompareString(text7, "LatestLaunch.bat", false) != 0)
                            {
                                if (Operators.CompareString(text7, "Log1.txt", false) != 0)
                                {
                                    if (Operators.CompareString(text7, "RawOutput.log", false) == 0)
                                    {
                                        text7 = "游戏崩溃前的输出.txt";
                                        encoding = Encoding.UTF8;
                                    }
                                }
                                else
                                {
                                    text7 = "PCL 启动器日志.txt";
                                    encoding = Encoding.UTF8;
                                }
                            }
                            else
                            {
                                text7 = "启动脚本.bat";
                            }
                            if (FileUtils.Exists(text6))
                            {
                                string text8 = FileUtils.ReadAsString(text6, encoding, null);
                                text8 = ModMinecraft.FilterUserName(ModMinecraft.FilterAccessToken(text8, Conversions.ToChar((Operators.CompareString(text7, "启动脚本.bat", false) == 0) ? "F" : "*")), '*');
                                FileUtils.Write(this.TempFolder + "Report\\" + text7, text8, encoding);
                                Logger.Info(string.Format("导出错误报告文件：{0}", text7), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                            }
                        }
                    }
                    finally
                    {
                        List<string>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                    FileUtils.CreateZipFromDirectory(CS_locals1._VB_Local_FileAddress, this.TempFolder + "Report\\");
                    DirectoryUtils.Delete(this.TempFolder + "Report\\", false);
                    ModMain.Hint("错误报告已导出！", ModMain.HintType.Green, true);
                    ModBase.OpenExplorer(CS_locals1._VB_Local_FileAddress);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "导出错误报告失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModCrash.vb");
                    return;
                }
            }
            if (!IsManualAnalyze)
            {
                ModMain.Telemetry("Minecraft 崩溃", new string[]
                {
                    "Version",
                    (((object)(this.TargetInstance) == (object)null)) ? "null" : this.TargetInstance.VersionDisplayName(),
                    "Detail",
                    ModMinecraft.FilterUserName(analyzeResult, '*')
                });
            }
        }

        private string GetAnalyzeResult(bool IsHandAnalyze)
        {
            string text;
            if (!this.CrashReasons.Any<KeyValuePair<CrashAnalyzer.CrashReason, List<string>>>())
            {
                if (IsHandAnalyze)
                {
                    text = "很抱歉，PCL 无法确定错误原因。";
                }
                else
                {
                    text = string.Format("很抱歉，你的游戏出现了一些问题……{0}如果要寻求帮助，请把错误报告文件发给对方，而不是发送这个窗口的照片或者截图。", "\r\n");
                }
            }
            else
            {
                List<string> list = new List<string>();
                try
                {
                    foreach (KeyValuePair<CrashAnalyzer.CrashReason, List<string>> keyValuePair in this.CrashReasons)
                    {
                        List<string> value = keyValuePair.Value;
                        switch (keyValuePair.Key)
                        {
                        case CrashAnalyzer.CrashReason.Java虚拟机参数有误:
                            list.Add("由于 Java 参数有误，导致游戏无法继续运行。\\n请检查高级选项中的 Java 虚拟机参数和游戏参数设置是否有误。");
                            break;
                        case CrashAnalyzer.CrashReason.Mod文件被解压:
                            list.Add("由于 Mod 文件被解压了，导致游戏无法继续运行。\\n直接把整个 Mod 文件放进 Mod 文件夹中即可，若解压就会导致游戏出错。\\n\\n请删除 Mod 文件夹中已被解压的 Mod，然后再启动游戏。");
                            break;
                        case CrashAnalyzer.CrashReason.MixinBootstrap缺失:
                            list.Add("由于缺失 MixinBootstrap，导致游戏崩溃。\\n请尝试安装 MixinBootstrap。若安装后依然崩溃，可以尝试在文件名前添加英文感叹号。");
                            break;
                        case CrashAnalyzer.CrashReason.内存不足:
                            list.Add("Minecraft 内存不足，导致其无法继续运行。\\n这很可能是因为电脑内存不足、游戏分配的内存不足，或是配置要求过高。\\n\\n你可以尝试在 更多 → 百宝箱 中选择 内存优化，然后再启动游戏。\\n如果还是不行，请在启动设置中增加为游戏分配的内存，并删除配置要求较高的材质、Mod、光影。\\n如果依然不奏效，请在开始游戏前尽量关闭其他软件，或者……换台电脑？\\h");
                            break;
                        case CrashAnalyzer.CrashReason.使用JDK:
                            list.Add("游戏似乎因为使用 JDK，或 Java 版本过高而崩溃了。\\n请在版本设置的 Java 选项中改用 Java 8，然后再启动游戏。\\n如果你没有安装 Java 8，可以从网络中下载、安装一个。");
                            break;
                        case CrashAnalyzer.CrashReason.显卡不支持OpenGL:
                        case CrashAnalyzer.CrashReason.显卡驱动不支持导致无法设置像素格式:
                        case CrashAnalyzer.CrashReason.Intel驱动不兼容导致EXCEPTION_ACCESS_VIOLATION:
                        case CrashAnalyzer.CrashReason.AMD驱动不兼容导致EXCEPTION_ACCESS_VIOLATION:
                        case CrashAnalyzer.CrashReason.Nvidia驱动不兼容导致EXCEPTION_ACCESS_VIOLATION:
                            if (this.LogAll.Contains("hd graphics "))
                            {
                                list.Add("你的显卡驱动存在问题，或未使用独立显卡，导致游戏无法正常运行。\\n\\n如果你的电脑存在独立显卡，请使用独立显卡而非 Intel 核显启动 PCL 与 Minecraft。\\n如果问题依然存在，请尝试升级你的显卡驱动到最新版本，或回退到出厂版本。\\n如果还是不行，还可以尝试使用 8.0.51 或更低版本的 Java。\\h");
                            }
                            else
                            {
                                list.Add("你的显卡驱动存在问题，导致游戏无法正常运行。\\n\\n请尝试升级你的显卡驱动到最新版本，或回退到出厂版本，然后再启动游戏。\\n如果还是不行，可以尝试使用 8.0.51 或更低版本的 Java。\\n如果问题依然存在，那么你可能需要换个更好的显卡……\\h");
                            }
                            break;
                        case CrashAnalyzer.CrashReason.使用OpenJ9:
                            list.Add("游戏因为使用 OpenJ9 而崩溃了。\\n请在版本设置的 Java 选项中改用非 OpenJ9 的 Java，然后再启动游戏。");
                            break;
                        case CrashAnalyzer.CrashReason.Java版本过高:
                            list.Add("游戏似乎因为你所使用的 Java 版本过高而崩溃了。\\n请在版本设置的 Java 选项中改用较低版本的 Java，然后再启动游戏。\\n如果没有，可以从网络中下载、安装一个。");
                            break;
                        case CrashAnalyzer.CrashReason.Java版本不兼容:
                            list.Add("游戏不兼容你当前使用的 Java。\\n请在版本设置的 Java 选项中改用合适版本的 Java，然后再启动游戏。\\n如果你没有安装任何 Mod 并且因此崩溃，请向 PCL 提交反馈。");
                            break;
                        case CrashAnalyzer.CrashReason.Mod名称包含特殊字符:
                            list.Add("由于有 Mod 的名称包含特殊字符，导致游戏崩溃。\\n请尝试修改 Mod 文件名，让它只包含英文字母、数字、减号（-）、下划线（_）和小数点，然后再启动游戏。");
                            break;
                        case CrashAnalyzer.CrashReason.极短的程序输出:
                            list.Add(string.Format("程序返回了以下信息：\\n{0}\\n\\h", value.First<string>()));
                            break;
                        case CrashAnalyzer.CrashReason.玩家手动触发调试崩溃:
                            list.Add("* 事实上，你的游戏没有任何问题，这是你自己触发的崩溃。\\n* 你难道没有更重要的事要做吗？");
                            break;
                        case CrashAnalyzer.CrashReason.光影或资源包导致OpenGL1282错误:
                            list.Add("你所使用的光影或材质导致游戏出现了一些问题……\\n\\n请尝试删除你所添加的这些额外资源。\\h");
                            break;
                        case CrashAnalyzer.CrashReason.文件或内容校验失败:
                            list.Add("部分文件或内容校验失败，导致游戏出现了问题。\\n\\n请尝试删除游戏（包括 Mod）并重新下载，或尝试在重新下载时使用 VPN。\\h");
                            break;
                        case CrashAnalyzer.CrashReason.确定Mod导致游戏崩溃:
                            if (value.IsSingle<string>())
                            {
                                list.Add("名为 " + value.First<string>() + " 的 Mod 导致了游戏出错。\\n你可以尝试禁用此 Mod，然后观察游戏是否还会崩溃。\\n\\e\\h");
                            }
                            else
                            {
                                list.Add("以下 Mod 导致了游戏出错：\\n - " + value.Join("\\n - ") + "\\n\\n你可以尝试依次禁用上述 Mod，然后观察游戏是否还会崩溃。\\n\\e\\h");
                            }
                            break;
                        case CrashAnalyzer.CrashReason.怀疑Mod导致游戏崩溃:
                        case CrashAnalyzer.CrashReason.堆栈分析发现Mod名称:
                            if (value.IsSingle<string>())
                            {
                                list.Add("PCL 怀疑名为 " + value.First<string>() + " 的 Mod 导致了游戏出错，但不能完全确定。\\n你可以尝试禁用此 Mod，然后观察游戏是否还会崩溃。\\n\\e\\h");
                            }
                            else
                            {
                                list.Add("PCL 怀疑以下 Mod 导致了游戏出错，但不能完全确定：\\n - " + value.Join("\\n - ") + "\\n\\n你可以尝试依次禁用上述 Mod，然后观察游戏是否还会崩溃。\\n\\e\\h");
                            }
                            break;
                        case CrashAnalyzer.CrashReason.Mod配置文件导致游戏崩溃:
                            if (value[1] == null)
                            {
                                list.Add("名为 " + value.First<string>() + " 的 Mod 导致了游戏出错。\\n\\e\\h");
                            }
                            else
                            {
                                list.Add(string.Concat(new string[]
                                {
                                    "名为 ",
                                    value.First<string>(),
                                    " 的 Mod 导致了游戏出错：\\n其配置文件 ",
                                    value[1],
                                    " 存在异常，无法读取。"
                                }));
                            }
                            break;
                        case CrashAnalyzer.CrashReason.ModMixin失败:
                            if (value.Count == 0)
                            {
                                list.Add("部分 Mod 注入失败，导致游戏出错。\\n这一般代表着部分 Mod 与其他 Mod 或当前环境不兼容，或是它存在 Bug。\\n你可以尝试逐步禁用 Mod，然后观察游戏是否还会崩溃，以此定位导致崩溃的 Mod。\\n\\e\\h");
                            }
                            else if (value.IsSingle<string>())
                            {
                                list.Add("名为 " + value.First<string>() + " 的 Mod 注入失败，导致游戏出错。\\n这一般代表着它与其他 Mod 或当前环境不兼容，或是它存在 Bug。\\n你可以尝试禁用此 Mod，然后观察游戏是否还会崩溃。\\n\\e\\h");
                            }
                            else
                            {
                                list.Add("以下 Mod 导致了游戏出错：\\n - " + value.Join("\\n - ") + "\\n这一般代表着它们与其他 Mod 或当前环境不兼容，或是它存在 Bug。\\n你可以尝试依次禁用上述 Mod，然后观察游戏是否还会崩溃。\\n\\e\\h");
                            }
                            break;
                        case CrashAnalyzer.CrashReason.Mod加载器报错:
                            if (value.IsSingle<string>())
                            {
                                list.Add("Mod 加载器提供了以下错误信息：\\n" + value.First<string>() + "\\n\\n请根据上述信息进行对应处理，如果看不懂英文可以使用翻译软件。");
                            }
                            else
                            {
                                list.Add("Mod 加载器可能已经提供了错误信息，请根据错误报告中的日志信息进行对应处理，如果看不懂英文可以使用翻译软件。\\h");
                            }
                            break;
                        case CrashAnalyzer.CrashReason.Mod初始化失败:
                            if (value.IsSingle<string>())
                            {
                                list.Add("名为 " + value.First<string>() + " 的 Mod 初始化失败，导致游戏无法继续加载。\\n你可以尝试禁用此 Mod，然后观察游戏是否还会崩溃。\\n\\e\\h");
                            }
                            else
                            {
                                list.Add("以下 Mod 初始化失败，导致游戏出错：\\n - " + value.Join("\\n - ") + "\\n\\n你可以尝试依次禁用上述 Mod，然后观察游戏是否还会崩溃。\\n\\e\\h");
                            }
                            break;
                        case CrashAnalyzer.CrashReason.堆栈分析发现关键字:
                            if (value.IsSingle<string>())
                            {
                                list.Add("你的游戏遇到了一些问题，PCL 为此找到了一个可疑的关键词：" + value.First<string>() + "。\\n\\n如果你知道某个关键词对应的 Mod，那么有可能就是它引起的错误，你也可以查看错误报告获取详情。\\h");
                            }
                            else
                            {
                                list.Add("你的游戏遇到了一些问题，PCL 为此找到了以下可疑的关键词：\\n - " + value.Join(", ") + "\\n\\n如果你知道某个关键词对应的 Mod，那么有可能就是它引起的错误，你也可以查看错误报告获取详情。\\h");
                            }
                            break;
                        case CrashAnalyzer.CrashReason.OptiFine导致无法加载世界:
                            list.Add("你所使用的 OptiFine 可能导致了你的游戏出现问题。\\n\\n该问题只在特定 OptiFine 版本中出现，你可以尝试更换 OptiFine 的版本。\\h");
                            break;
                        case CrashAnalyzer.CrashReason.特定方块导致崩溃:
                            if (value.IsSingle<string>())
                            {
                                list.Add("游戏似乎因为方块 " + value.First<string>() + " 出现了问题。\\n\\n你可以创建一个新世界，并观察游戏的运行情况：\\n - 若正常运行，则是该方块导致出错，你或许需要使用一些方式删除此方块。\\n - 若仍然出错，问题就可能来自其他原因……\\h");
                            }
                            else
                            {
                                list.Add("游戏似乎因为世界中的某些方块出现了问题。\\n\\n你可以创建一个新世界，并观察游戏的运行情况：\\n - 若正常运行，则是某些方块导致出错，你或许需要删除该世界。\\n - 若仍然出错，问题就可能来自其他原因……\\h");
                            }
                            break;
                        case CrashAnalyzer.CrashReason.特定实体导致崩溃:
                            if (value.IsSingle<string>())
                            {
                                if (value.First<string>().Contains("minecraft:player"))
                                {
                                    if (value.First<string>().Contains(" "))
                                    {
                                        list.Add(string.Format("游戏因为位于 {0} 的玩家实体导致了崩溃。\\h", value.First<string>().AfterFirst(" ", false)));
                                    }
                                    else
                                    {
                                        list.Add("游戏因为玩家实体导致了崩溃。\\h");
                                    }
                                }
                                else
                                {
                                    list.Add(string.Format("游戏因为实体 {0} 导致了崩溃。\\h", value.First<string>()));
                                }
                            }
                            else
                            {
                                list.Add(string.Format("游戏因为下列实体导致了崩溃：{0}\\h", value.Join('、')));
                            }
                            break;
                        case CrashAnalyzer.CrashReason.材质过大或显卡配置不足:
                            list.Add("你所使用的材质分辨率过高，或显卡配置不足，导致游戏无法继续运行。\\n\\n如果你正在使用高清材质，请将它移除。\\n如果你没有使用材质，那么你可能需要更新显卡驱动，或者换个更好的显卡……\\h");
                            break;
                        case CrashAnalyzer.CrashReason.没有可用的分析文件:
                            list.Add("你的游戏出现了一些问题，但 PCL 未能找到相关记录文件，因此无法进行分析。\\h");
                            break;
                        case CrashAnalyzer.CrashReason.使用32位Java导致JVM无法分配足够多的内存:
                            if (Environment.Is64BitOperatingSystem)
                            {
                                list.Add("你似乎正在使用 32 位 Java，这会导致 Minecraft 无法使用所需的内存，进而造成崩溃。\\n\\n请在启动设置的 Java 选择一项中改用 64 位的 Java 再启动游戏，然后再启动游戏。\\n如果你没有安装 64 位的 Java，你可以从网络中下载、安装一个。");
                            }
                            else
                            {
                                list.Add("你正在使用 32 位的操作系统，这会导致 Minecraft 无法使用所需的内存，进而造成崩溃。\\n\\n你或许只能重装 64 位的操作系统来解决此问题。\\n如果你的电脑内存在 2GB 以内，那或许只能换台电脑了……\\h");
                            }
                            break;
                        case CrashAnalyzer.CrashReason.Mod重复安装:
                            if (value.Count >= 2)
                            {
                                list.Add("你重复安装了多个相同的 Mod：\\n - " + value.Join("\\n - ") + "\\n\\n每个 Mod 只能出现一次，请删除重复的 Mod，然后再启动游戏。");
                            }
                            else
                            {
                                list.Add("你可能重复安装了多个相同的 Mod，导致游戏出错。\\n\\n每个 Mod 只能出现一次，请删除重复的 Mod，然后再启动游戏。\\e\\h");
                            }
                            break;
                        case CrashAnalyzer.CrashReason.Mod互不兼容:
                            if (value.IsSingle<string>())
                            {
                                list.Add("你所安装的 Mod 不兼容：\\n" + value.First<string>() + "\\n\\n请根据上述信息进行对应处理，如果看不懂英文可以使用翻译软件。");
                            }
                            else
                            {
                                list.Add("你所安装的 Mod 不兼容，Mod 加载器可能已经提供了错误信息，请根据错误报告中的日志信息进行对应处理，如果看不懂英文可以使用翻译软件。\\h");
                            }
                            break;
                        case CrashAnalyzer.CrashReason.OptiFine与Forge不兼容:
                            list.Add("由于 OptiFine 与当前版本的 Forge 不兼容，导致了游戏崩溃。\\n\\n请前往 OptiFine 官网（https://optifine.net/downloads）查看 OptiFine 所兼容的 Forge 版本，并严格按照对应版本重新安装游戏。");
                            break;
                        case CrashAnalyzer.CrashReason.Fabric报错:
                            if (value.IsSingle<string>())
                            {
                                list.Add("Fabric 提供了以下错误信息：\\n" + value.First<string>() + "\\n\\n请根据上述信息进行对应处理，如果看不懂英文可以使用翻译软件。");
                            }
                            else
                            {
                                list.Add("Fabric 可能已经提供了错误信息，请根据错误报告中的日志信息进行对应处理，如果看不懂英文可以使用翻译软件。\\h");
                            }
                            break;
                        case CrashAnalyzer.CrashReason.Fabric报错并给出解决方案:
                            if (value.IsSingle<string>())
                            {
                                list.Add("Fabric 提供了以下解决方案：\\n" + value.First<string>() + "\\n\\n请根据上述信息进行对应处理，如果看不懂英文可以使用翻译软件。");
                            }
                            else
                            {
                                list.Add("Fabric 可能已经提供了解决方案，请根据错误报告中的日志信息进行对应处理，如果看不懂英文可以使用翻译软件。\\h");
                            }
                            break;
                        case CrashAnalyzer.CrashReason.Forge报错:
                            if (value.IsSingle<string>())
                            {
                                list.Add("Forge 提供了以下错误信息：\\n" + value.First<string>() + "\\n\\n请根据上述信息进行对应处理，如果看不懂英文可以使用翻译软件。");
                            }
                            else
                            {
                                list.Add("Forge 可能已经提供了错误信息，请根据错误报告中的日志信息进行对应处理，如果看不懂英文可以使用翻译软件。\\h");
                            }
                            break;
                        case CrashAnalyzer.CrashReason.低版本Forge与高版本Java不兼容:
                            list.Add("由于低版本 Forge 与当前 Java 不兼容，导致了游戏崩溃。\\n\\n请尝试以下解决方案：\\n - 更新 Forge 到 36.2.26 或更高版本\\n - 换用版本低于 1.8.0.320 的 Java");
                            break;
                        case CrashAnalyzer.CrashReason.版本Json中存在多个Forge:
                            list.Add("可能由于其他启动器修改了 Forge 版本，当前版本的文件存在异常，导致了游戏崩溃。\\n请尝试重新全新安装 Forge，而非使用其他启动器修改 Forge 版本。");
                            break;
                        case CrashAnalyzer.CrashReason.Mod过多导致超出ID限制:
                            list.Add("你所安装的 Mod 过多，超出了游戏的 ID 限制，导致了游戏崩溃。\\n请尝试安装 JEID 等修复 Mod，或删除部分大型 Mod。");
                            break;
                        case CrashAnalyzer.CrashReason.NightConfig的Bug:
                            list.Add("由于 Night Config 存在问题，导致了游戏崩溃。\\n你可以尝试安装 Night Config Fixes 模组，这或许能解决此问题。\\h");
                            break;
                        case CrashAnalyzer.CrashReason.ShadersMod与OptiFine同时安装:
                            list.Add("无需同时安装 OptiFine 和 Shaders Mod，OptiFine 已经集成了 Shaders Mod 的功能。\\n在删除 Shaders Mod 后，游戏即可正常运行。");
                            break;
                        case CrashAnalyzer.CrashReason.Forge安装不完整:
                            list.Add("由于安装的 Forge 文件丢失，导致游戏无法正常运行。\\n请重新安装一次相同版本的 Forge，然后再启动游戏。\\n在打包游戏时删除 libraries 文件夹可能导致此错误。\\h");
                            break;
                        case CrashAnalyzer.CrashReason.Mod需要Java11:
                            list.Add("你所安装的部分 Mod 似乎需要使用 Java 11 启动。\\n请在启动设置的 Java 选择一项中改用 Java 11，然后再启动游戏。\\n如果你没有安装 Java 11，你可以从网络中下载、安装一个。");
                            break;
                        case CrashAnalyzer.CrashReason.Mod缺少前置或MC版本错误:
                            if (value.Any<string>())
                            {
                                list.Add("由于未安装正确的前置 Mod，导致游戏退出。\\n缺失的依赖项：\\n - " + value.Join("\\n - ") + "\\n\\n请根据上述信息进行对应处理，如果看不懂英文可以使用翻译软件。");
                            }
                            else
                            {
                                list.Add("由于未安装正确的前置 Mod，导致游戏退出。\\n请根据错误报告中的日志信息进行对应处理，如果看不懂英文可以使用翻译软件。\\h");
                            }
                            break;
                        default:
                            list.Add("PCL 获取到了没有详细信息的错误原因（" + Conversions.ToString((int)this.CrashReasons.First<KeyValuePair<CrashAnalyzer.CrashReason, List<string>>>().Key) + "），请向 PCL 作者提交反馈以获取详情。\\h");
                            break;
                        }
                    }
                }
                finally
                {
                    Dictionary<CrashAnalyzer.CrashReason, List<string>>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
                text = list.Join("\\n\\n此外，").Replace("\\n", "\r\n").Replace("\\h", "")
                    .Replace("\\e", IsHandAnalyze ? "" : "\r\n你可以查看错误报告了解错误具体是如何发生的。")
                    .ReplaceLineEndings("\r\n", false)
                    .Trim("\r\n".ToCharArray()) + ((!list.Any<string>((CrashAnalyzer._Closure___._I29_0 == null) ? (CrashAnalyzer._Closure___._I29_0 = new Func<string, bool>(CrashAnalyzer._Closure___._I._Lambda___29_0)) : CrashAnalyzer._Closure___._I29_0) || IsHandAnalyze) ? "" : ("\r\n如果要寻求帮助，请把错误报告文件发给对方，而不是发送这个窗口的照片或者截图。" + (PageSetupSystem.IsLauncherNewest().GetValueOrDefault(true) ? "" : "\r\n\r\n此外，你正在使用老版本 PCL，更新 PCL 或许也能解决这个问题。\r\n你可以点击 [设置 → 其他 → 启动器 → 检查更新] 更新 PCL。")));
            }
            return text;
        }

        [CompilerGenerated]
        private bool _Lambda___21_0(string LogText)
        {
            bool flag;
            if (!LogText.Contains("Mixin prepare failed ") && !LogText.Contains("Mixin apply failed ") && !LogText.Contains("MixinApplyError") && !LogText.Contains("MixinTransformerError") && !LogText.Contains("mixin.injection.throwables.") && !LogText.Contains(".json] FAILED during )"))
            {
                flag = false;
            }
            else
            {
                string text = LogText.RegexSeek("(?<=from mod )[^.\\/ ]+(?=\\] from)", RegexOptions.None);
                if (text == null)
                {
                    text = LogText.RegexSeek("(?<=for mod )[^.\\/ ]+(?= failed)", RegexOptions.None);
                }
                if (text != null)
                {
                    this.AppendReason(CrashAnalyzer.CrashReason.ModMixin失败, this.TryAnalyzeModName(text.TrimEnd("\r\n ".ToCharArray())));
                    flag = true;
                }
                else
                {
                    try
                    {
                        IEnumerator<string> enumerator = LogText.RegexSearch("(?<=^[^\\t]+[ \\[{(])[^ \\[{(]+\\.[^ ]+(?=\\.json)", RegexOptions.Multiline).GetEnumerator();
                        if (enumerator.MoveNext())
                        {
                            string text2 = enumerator.Current;
                            this.AppendReason(CrashAnalyzer.CrashReason.ModMixin失败, this.TryAnalyzeModName(text2.Replace("mixins", "mixin").Replace(".mixin", "").Replace("mixin.", "")));
                            return true;
                        }
                    }
                    finally
                    {
                        IEnumerator<string> enumerator = default;
                        if (enumerator != null)
                        {
                            enumerator.Dispose();
                        }
                    }
                    this.AppendReason(CrashAnalyzer.CrashReason.ModMixin失败, (IEnumerable<string>)null);
                    flag = true;
                }
            }
            return flag;
        }

        [CompilerGenerated]
        private void _Lambda___28_0()
        {
            if (FileUtils.Exists(this.DirectFile.Value.Key))
            {
                ModBase.StartProcess(this.DirectFile.Value.Key, "");
                return;
            }
            string text = ModBase.PathTemp + "Crash.txt";
            FileUtils.Write(text, this.DirectFile.Value.Value.Join("\r\n"), null);
            ModBase.StartProcess(text, "");
        }

        private string TempFolder;

        private ModMinecraft.McInstance TargetInstance;

        private List<KeyValuePair<string, string[]>> AnalyzeRawFiles;

        private KeyValuePair<string, string[]>? DirectFile;

        private string LogMc;

        private string LogMcDebug;

        private string LogHs;

        private string LogCrash;

        private string LogAll;

        private Dictionary<CrashAnalyzer.CrashReason, List<string>> CrashReasons;

        private List<string> OutputFiles;

        private enum AnalyzeFileType
        {
            HsErr,
            MinecraftLog,
            ExtraLogFile,
            ExtraReportFile,
            CrashReport
        }

        private enum CrashReason
        {
            Java虚拟机参数有误,
            Mod文件被解压,
            MixinBootstrap缺失,
            内存不足,
            使用JDK,
            显卡不支持OpenGL,
            使用OpenJ9,
            Java版本过高,
            Java版本不兼容,
            Mod名称包含特殊字符,
            显卡驱动不支持导致无法设置像素格式,
            极短的程序输出,
            Intel驱动不兼容导致EXCEPTION_ACCESS_VIOLATION,
            AMD驱动不兼容导致EXCEPTION_ACCESS_VIOLATION,
            Nvidia驱动不兼容导致EXCEPTION_ACCESS_VIOLATION,
            玩家手动触发调试崩溃,
            光影或资源包导致OpenGL1282错误,
            文件或内容校验失败,
            确定Mod导致游戏崩溃,
            怀疑Mod导致游戏崩溃,
            Mod配置文件导致游戏崩溃,
            ModMixin失败,
            Mod加载器报错,
            Mod初始化失败,
            堆栈分析发现关键字,
            堆栈分析发现Mod名称,
            OptiFine导致无法加载世界,
            特定方块导致崩溃,
            特定实体导致崩溃,
            材质过大或显卡配置不足,
            没有可用的分析文件,
            使用32位Java导致JVM无法分配足够多的内存,
            Mod重复安装,
            Mod互不兼容,
            OptiFine与Forge不兼容,
            Fabric报错,
            Fabric报错并给出解决方案,
            Forge报错,
            低版本Forge与高版本Java不兼容,
            版本Json中存在多个Forge,
            Mod过多导致超出ID限制,
            NightConfig的Bug,
            ShadersMod与OptiFine同时安装,
            Forge安装不完整,
            Mod需要Java11,
            Mod缺少前置或MC版本错误
        }

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            public _Closure___()
            {
            }

            static _Closure___()
            {
            }

            internal bool _Lambda___8_0(string s)
            {
                return s.Contains("以下为游戏输出的最后一段内容");
            }

            internal bool _Lambda___8_1(KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>> p)
            {
                return p.Key == CrashAnalyzer.AnalyzeFileType.ExtraLogFile;
            }

            internal KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>> _Lambda___8_2(KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>> p)
            {
                return new KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>>(CrashAnalyzer.AnalyzeFileType.MinecraftLog, p.Value);
            }

            internal string _Lambda___20_0(string s)
            {
                return s.Trim("\r\n\t ".ToCharArray());
            }

            internal string _Lambda___23_0(string s)
            {
                return s.Replace("$", ".");
            }

            internal bool _Lambda___29_0(string r)
            {
                return r.EndsWithF("\\h", false);
            }

            public static readonly CrashAnalyzer._Closure___ _I = new CrashAnalyzer._Closure___();

            public static Func<string, bool> _I8_0;

            public static Func<KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>>, bool> _I8_1;

            public static Func<KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>>, KeyValuePair<CrashAnalyzer.AnalyzeFileType, KeyValuePair<string, string[]>>> _I8_2;

            public static Func<string, string> _I20_0;

            public static Func<string, string> _I23_0;

            public static Func<string, bool> _I29_0;
        }

        [CompilerGenerated]
        private sealed class _Closure___28_0
        {
            public _Closure___28_0(CrashAnalyzer._Closure___28_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_FileAddress = arg0._VB_Local_FileAddress;
                }
            }

            internal void _Lambda___1()
            {
                this._VB_Local_FileAddress = Dialogs.SaveFile("选择保存位置", "错误报告-" + DateTime.Now.ToString("G").Replace("/", "-").Replace(":", ".")
                    .Replace(" ", "_") + ".zip", null, new ValueTuple<string, string>[]
                {
                    new ValueTuple<string, string>("zip", "Minecraft 错误报告")
                });
            }

            public string _VB_Local_FileAddress;
        }
    }
}
