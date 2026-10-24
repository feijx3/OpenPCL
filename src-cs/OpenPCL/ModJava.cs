using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using MeloongCore;
using MeloongCore.Extensions;
using MeloongCore.Wpf;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;
using PCLCS;

namespace PCL
{
    [StandardModule]
    public static class ModJava
    {
        // Note: this type is marked as 'beforefieldinit'.
        static ModJava()
        {
            ModJava.JavaListRefreshWorker = new RedoableWorker(new Action<CancellationToken?, ProgressProvider>(ModJava._Closure___._I._Lambda___0_0), "JavaListRefreshWorker");
        }

        public static void JavaInit()
        {
            try
            {
                if (Configs.JavaConfigVersion.Get() == 0)
                {
                    try
                    {
                        ModJava._Closure___1_0 CS_locals1 = new ModJava._Closure___1_0();
                        Logger.Warn("正在从老版本迁移 Java 列表", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                        List<Java> list = Settings.Get<string>("LaunchArgumentJavaAll", null).DeserializeJson<JArray>().Select<JToken, Java>((ModJava._Closure___._I1_0 == null) ? (ModJava._Closure___._I1_0 = new Func<JToken, Java>(ModJava._Closure___._I._Lambda___1_0)) : ModJava._Closure___._I1_0)
                            .ToList<Java>();
                        list = JavaUtils.SortAsync(list, default(CancellationToken), null).Run<List<Java>>();
                        Logger.Info(string.Format("已从老版本设置中发现 {0} 个 Java", list.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                        string text = Settings.Get<string>("LaunchArgumentJavaSelect", null);
                        CS_locals1._VB_Local_SelectedJava = (text.StartsWithF("{", false) ? text.DeserializeJson<JObject>()["Path"].ToString() : "");
                        Java java = list.FirstOrDefault<Java>(new Func<Java, bool>(CS_locals1._Lambda___1));
                        if (java != null)
                        {
                            list.Remove(java);
                            list.Insert(0, java);
                            Logger.Info(string.Format("已将老版本设置中强制选择的 Java 置顶：{0}", java), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                        }
                        Configs.JavaList.Set(new ConcurrentList<Java>(list));
                        Configs.JavaConfigVersion.Set(1);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "从老版本 PCL 迁移 Java 列表失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                    }
                }
                ConcurrentList<Java> concurrentList = Configs.JavaList.Get();
                if (concurrentList.Any<Java>())
                {
                    Logger.Info(string.Format("共有 {0} 个可用的 Java：{1}- {2}", concurrentList.Count, "\r\n", concurrentList.Select<Java, string>((ModJava._Closure___._I1_2 == null) ? (ModJava._Closure___._I1_2 = new Func<Java, string>(ModJava._Closure___._I._Lambda___1_2)) : ModJava._Closure___._I1_2).Join("\r\n- ")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                }
                else
                {
                    Logger.Warn("初始化未找到可用的 Java，将自动触发搜索", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                    ModJava.JavaListRefreshWorker.Start(default(CancellationToken), null);
                }
            }
            catch (Exception ex2)
            {
                Logger.Error(ex2, "初始化 Java 列表失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                Configs.JavaList.Reset();
            }
        }

        public static RedoableWorker JavaListRefreshWorker
        {
            [CompilerGenerated]
            get
            {
                return ModJava._JavaListRefreshWorker;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                Action action = new Action(ModJava.UpdateJavaLists);
                Action action2 = new Action(ModJava.UpdateJavaLists);
                RedoableWorker redoableWorker = ModJava._JavaListRefreshWorker;
                if (redoableWorker != null)
                {
                    redoableWorker.Started -= action;
                    redoableWorker.Stopped -= action2;
                }
                ModJava._JavaListRefreshWorker = value;
                redoableWorker = ModJava._JavaListRefreshWorker;
                if (redoableWorker != null)
                {
                    redoableWorker.Started += action;
                    redoableWorker.Stopped += action2;
                }
            }
        }

        public static void UpdateJavaLists()
        {
            ModBase.RunInUi((ModJava._Closure___._I6_0 == null) ? (ModJava._Closure___._I6_0 = new Action(ModJava._Closure___._I._Lambda___6_0)) : ModJava._Closure___._I6_0, false);
        }

        public static void ManuallyImportJava()
        {
            ModJava._Closure___7_0 CS_locals1 = new ModJava._Closure___7_0();
            string text = Dialogs.SelectFile("选择 java.exe（可能在 bin 文件夹中）", false, null, new OneOf<IEnumerable<ValueTuple<string[], string>>, string>?("java.exe|java.exe")).FirstOrDefault<string>();
            if (!string.IsNullOrEmpty(text))
            {
                CS_locals1._VB_Local_NewJava = new Java(PathUtils.RemoveLastPart(text), null);
                if (!CS_locals1._VB_Local_NewJava.CheckAsync(default(CancellationToken)).Run<bool>())
                {
                    ModMain.Hint("该 Java 存在异常，无法使用", ModMain.HintType.Red, true);
                    return;
                }
                Configs.JavaRemovedList.Edit(new ConfigEntry<List<string>>.RefAction(CS_locals1._Lambda___0));
                Configs.JavaList.Edit(new ConfigEntry<ConcurrentList<Java>>.RefAction(CS_locals1._Lambda___2));
                ModMain.Hint(string.Format("已将 Java 加入列表：{0}", text), ModMain.HintType.Green, true);
                ModJava.UpdateJavaLists();
            }
        }

        public static void ManuallyRemoveJava(Java JavaEntry)
        {
            ModJava._Closure___8_0 CS_locals1 = new ModJava._Closure___8_0();
            CS_locals1._VB_Local_JavaEntry = JavaEntry;
            Configs.JavaList.Edit(new ConfigEntry<ConcurrentList<Java>>.RefAction(CS_locals1._Lambda___0));
            Configs.JavaRemovedList.Edit(new Func<List<string>, List<string>>(CS_locals1._Lambda___1));
            ModMain.Hint(string.Format("已将 Java 移除出列表：{0}", CS_locals1._VB_Local_JavaEntry), ModMain.HintType.Green, true);
            ModJava.UpdateJavaLists();
            PageInstanceLeft.ReloadCurrentJava();
        }

        public static void ManuallySearchJava(CancellationToken CancellationToken = default(CancellationToken))
        {
            ModJava._Closure___9_0 CS_locals1 = new ModJava._Closure___9_0();
            CS_locals1._VB_Local_CancellationToken = CancellationToken;
            ModBase.RunInThread(new Action(CS_locals1._Lambda___0));
        }

        public static ValueTuple<ValueRange<Version>, string> GetJavaRequirement(ModMinecraft.McInstance Instance)
        {
            ModJava._Closure___10_0 CS_locals1 = new ModJava._Closure___10_0();
            CS_locals1._VB_Local_Instance = Instance;
            if (Settings.Get<int>("VersionArgumentJavaV2", CS_locals1._VB_Local_Instance) == 1)
            {
                try
                {
                    ValueRange<Version> valueRange = ValueRange<Version>.FromString(Settings.Get<string>("VersionArgumentJavaRange", CS_locals1._VB_Local_Instance), (ModJava._Closure___._I10_0 == null) ? (ModJava._Closure___._I10_0 = new Func<string, Version>(ModJava._Closure___._I._Lambda___10_0)) : ModJava._Closure___._I10_0);
                    return new ValueTuple<ValueRange<Version>, string>(valueRange, null);
                }
                catch (Exception ex)
                {
                    Logger.Warn(string.Format("版本设置中指定的 Java 版本范围有误（{0}），这可能是因为首次修改此设置，尚未初始化", Settings.Get<string>("VersionArgumentJavaRange", CS_locals1._VB_Local_Instance)), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                }
            }
            CS_locals1._VB_Local_JavaRange = ValueRange<Version>.All();
            VB_AnonymousDelegate_5<ValueRange<Version>> vb_AnonymousDelegate_ = new VB_AnonymousDelegate_5<ValueRange<Version>>(CS_locals1._Lambda___1);
            if ((!CS_locals1._VB_Local_Instance.Version.Valid && DateTime.Compare(CS_locals1._VB_Local_Instance.ReleaseTime, new DateTime(2024, 4, 2)) >= 0) || (CS_locals1._VB_Local_Instance.Version.Valid && CS_locals1._VB_Local_Instance.Version.Vanilla >= new Version(20, 0, 5)))
            {
                vb_AnonymousDelegate_(ValueRange<Version>.AtLeast(new Version(21, 0)));
            }
            else if ((!CS_locals1._VB_Local_Instance.Version.Valid && DateTime.Compare(CS_locals1._VB_Local_Instance.ReleaseTime, new DateTime(2021, 11, 16)) >= 0) || (CS_locals1._VB_Local_Instance.Version.Valid && CS_locals1._VB_Local_Instance.Version.Vanilla.Major >= 18))
            {
                vb_AnonymousDelegate_(ValueRange<Version>.AtLeast(new Version(17, 0)));
            }
            else if ((!CS_locals1._VB_Local_Instance.Version.Valid && DateTime.Compare(CS_locals1._VB_Local_Instance.ReleaseTime, new DateTime(2021, 5, 11)) >= 0) || (CS_locals1._VB_Local_Instance.Version.Valid && CS_locals1._VB_Local_Instance.Version.Vanilla.Major >= 17))
            {
                vb_AnonymousDelegate_(ValueRange<Version>.AtLeast(new Version(16, 0)));
            }
            else if (CS_locals1._VB_Local_Instance.ReleaseTime.Year >= 2017)
            {
                vb_AnonymousDelegate_(ValueRange<Version>.AtLeast(new Version(8, 0)));
            }
            else if (DateTime.Compare(CS_locals1._VB_Local_Instance.ReleaseTime, new DateTime(2013, 5, 1)) <= 0 && CS_locals1._VB_Local_Instance.ReleaseTime.Year >= 2001)
            {
                vb_AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(9, 0)));
            }
            string text = null;
            JObject jsonObject = CS_locals1._VB_Local_Instance.JsonObject;
            int? num;
            if (jsonObject == null)
            {
                num = null;
            }
            else
            {
                JToken jtoken = jsonObject["javaVersion"];
                if (jtoken == null)
                {
                    num = null;
                }
                else
                {
                    JToken jtoken2 = jtoken["majorVersion"];
                    num = ((jtoken2 != null) ? new int?(jtoken2.ToObject<int>()) : null);
                }
            }
            int? num3;
            int? num2 = (num3 = num);
            int num5;
            if (num3 == null)
            {
                JObject jsonVersion = CS_locals1._VB_Local_Instance.JsonVersion;
                int? num4;
                if (jsonVersion == null)
                {
                    num4 = null;
                }
                else
                {
                    JToken jtoken3 = jsonVersion["java_version"];
                    num4 = ((jtoken3 != null) ? new int?(jtoken3.ToObject<int>()) : null);
                }
                num3 = num4;
                num5 = num3.GetValueOrDefault();
            }
            else
            {
                num5 = num2.GetValueOrDefault();
            }
            int num6 = num5;
            if (num6 >= 22)
            {
                Logger.Info("Mojang 要求至少使用 Java " + Conversions.ToString(num6), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                vb_AnonymousDelegate_(ValueRange<Version>.AtLeast(new Version(num6, 0)));
                JObject jsonObject2 = CS_locals1._VB_Local_Instance.JsonObject;
                object obj;
                if (jsonObject2 == null)
                {
                    obj = null;
                }
                else
                {
                    JToken jtoken4 = jsonObject2["javaVersion"];
                    if (jtoken4 == null)
                    {
                        obj = null;
                    }
                    else
                    {
                        JToken jtoken5 = jtoken4["component"];
                        obj = ((jtoken5 != null) ? jtoken5.ToString() : null);
                    }
                }
                object obj2;
                if ((obj2 = obj) == null)
                {
                    JObject jsonVersion2 = CS_locals1._VB_Local_Instance.JsonVersion;
                    if (jsonVersion2 == null)
                    {
                        obj2 = null;
                    }
                    else
                    {
                        JToken jtoken6 = jsonVersion2["java_component"];
                        obj2 = ((jtoken6 != null) ? jtoken6.ToString() : null);
                    }
                }
                text = (string)obj2;
                if (Operators.CompareString(text, "", false) == 0)
                {
                    text = null;
                }
            }
            if (CS_locals1._VB_Local_Instance.Version.HasOptiFine && CS_locals1._VB_Local_Instance.Version.Valid)
            {
                if (CS_locals1._VB_Local_Instance.Version.Vanilla.Major < 7)
                {
                    vb_AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(9, 0)));
                }
                else if (CS_locals1._VB_Local_Instance.Version.Vanilla.Major >= 8 && CS_locals1._VB_Local_Instance.Version.Vanilla.Major < 12)
                {
                    vb_AnonymousDelegate_(ValueRange<Version>.ClosedOpen(new Version(8, 0), new Version(9, 0)));
                }
                else if (CS_locals1._VB_Local_Instance.Version.Vanilla.Major == 12)
                {
                    vb_AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(9, 0)));
                }
            }
            if (CS_locals1._VB_Local_Instance.Version.ModLoaders.HasFlagF(ModLoaders.Forge))
            {
                if (CS_locals1._VB_Local_Instance.Version.Vanilla >= new Version(6, 0, 1) && CS_locals1._VB_Local_Instance.Version.Vanilla <= new Version(7, 0, 2))
                {
                    vb_AnonymousDelegate_(ValueRange<Version>.ClosedOpen(new Version(7, 0), new Version(8, 0)));
                }
                else if (CS_locals1._VB_Local_Instance.Version.Vanilla.Major <= 12 || !CS_locals1._VB_Local_Instance.Version.Valid)
                {
                    vb_AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(9, 0)));
                }
                else if (CS_locals1._VB_Local_Instance.Version.Vanilla.Major <= 14)
                {
                    vb_AnonymousDelegate_(ValueRange<Version>.ClosedOpen(new Version(8, 0), new Version(11, 0)));
                }
                else if (CS_locals1._VB_Local_Instance.Version.Vanilla.Major == 15)
                {
                    vb_AnonymousDelegate_(ValueRange<Version>.ClosedOpen(new Version(8, 0), new Version(16, 0)));
                }
                else if (McUtils.CompareVersionGE(CS_locals1._VB_Local_Instance.Version.ModLoaderVersions[ModLoaders.Forge], "34.0.0") && McUtils.CompareVersionGE("36.2.25", CS_locals1._VB_Local_Instance.Version.ModLoaderVersions[ModLoaders.Forge]))
                {
                    vb_AnonymousDelegate_(ValueRange<Version>.AtMost(new Version(8, 0, 320)));
                }
                else if (McUtils.CompareVersionGE(CS_locals1._VB_Local_Instance.Version.ModLoaderVersions[ModLoaders.Forge], "36.2.26") && McUtils.CompareVersionGE("36.999999.999999", CS_locals1._VB_Local_Instance.Version.ModLoaderVersions[ModLoaders.Forge]))
                {
                    vb_AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(24, 0)));
                }
                else if (McUtils.CompareVersionGE(CS_locals1._VB_Local_Instance.Version.ModLoaderVersions[ModLoaders.Forge], "37.0.0") && McUtils.CompareVersionGE("37.0.79", CS_locals1._VB_Local_Instance.Version.ModLoaderVersions[ModLoaders.Forge]))
                {
                    vb_AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(17, 0)));
                }
                else if (CS_locals1._VB_Local_Instance.Version.Vanilla.Major == 18 && CS_locals1._VB_Local_Instance.Version.HasOptiFine)
                {
                    vb_AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(19, 0)));
                }
                else if (McUtils.CompareVersionGE(CS_locals1._VB_Local_Instance.Version.ModLoaderVersions[ModLoaders.Forge], "45.0.21") && McUtils.CompareVersionGE("45.0.65", CS_locals1._VB_Local_Instance.Version.ModLoaderVersions[ModLoaders.Forge]))
                {
                    vb_AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(20, 0)));
                }
                else if (McUtils.CompareVersionGE(CS_locals1._VB_Local_Instance.Version.ModLoaderVersions[ModLoaders.Forge], "45.0.66") && McUtils.CompareVersionGE("47.4.8", CS_locals1._VB_Local_Instance.Version.ModLoaderVersions[ModLoaders.Forge]))
                {
                    vb_AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(22, 0)));
                }
            }
            if (CS_locals1._VB_Local_Instance.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge) && (CS_locals1._VB_Local_Instance.Version.Vanilla == new Version(20, 0, 1) || (McUtils.CompareVersionGE("20.2.62-beta", CS_locals1._VB_Local_Instance.Version.ModLoaderVersions[ModLoaders.NeoForge]) && !CS_locals1._VB_Local_Instance.Version.ModLoaderVersions[ModLoaders.NeoForge].Contains("25w14craftmine"))))
            {
                vb_AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(22, 0)));
            }
            if (CS_locals1._VB_Local_Instance.Version.ModLoaders.HasFlagF(ModLoaders.Fabric) && CS_locals1._VB_Local_Instance.Version.Valid)
            {
                if (CS_locals1._VB_Local_Instance.Version.Vanilla.Major >= 15 && CS_locals1._VB_Local_Instance.Version.Vanilla.Major <= 16)
                {
                    vb_AnonymousDelegate_(ValueRange<Version>.AtLeast(new Version(8, 0)));
                }
                else if (CS_locals1._VB_Local_Instance.Version.Vanilla.Major >= 18)
                {
                    vb_AnonymousDelegate_(ValueRange<Version>.AtLeast(new Version(17, 0)));
                }
                if (Operators.CompareString(CS_locals1._VB_Local_Instance.Version.ModLoaderVersions[ModLoaders.Fabric], "未知版本", false) != 0 && McUtils.CompareVersion(CS_locals1._VB_Local_Instance.Version.ModLoaderVersions[ModLoaders.Fabric], "0.17.0") < 0)
                {
                    vb_AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(25, 0)));
                }
            }
            if (CS_locals1._VB_Local_Instance.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader) && CS_locals1._VB_Local_Instance.Version.Valid)
            {
                vb_AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(9, 0)));
            }
            if (Settings.Get<ModLaunch.McLoginType>("LoginType", null) == ModLaunch.McLoginType.Nide)
            {
                vb_AnonymousDelegate_(ValueRange<Version>.AtLeast(new Version(8, 0, 141)));
            }
            ValueTuple<ValueRange<Version>, string> valueTuple = new ValueTuple<ValueRange<Version>, string>(CS_locals1._VB_Local_JavaRange, text);
            return valueTuple;
        }

        public static Java SelectOrDownloadJava(OneOf<ValueRange<Version>, ModMinecraft.McInstance> Target, bool TryFixOrHint, CancellationToken c = default(CancellationToken), ProgressProvider p = null)
        {
            ModJava._Closure___11_0 CS_locals1 = new ModJava._Closure___11_0(null);
            CS_locals1._VB_Local_c = c;
            checked
            {
                Java java2;
                try
                {
                    if (Target.Is<ModMinecraft.McInstance>())
                    {
                        ModJava.MigrateInstanceJavaSettings(Target.As<ModMinecraft.McInstance>(), CS_locals1._VB_Local_c, (p != null) ? p.SplitTo(0.2) : null);
                    }
                    ModMinecraft.McInstance mcInstance = (Target.Is<ModMinecraft.McInstance>() ? Target.As<ModMinecraft.McInstance>() : null);
                    int num = ((((object)mcInstance == (object)null)) ? 0 : Settings.Get<int>("VersionArgumentJavaV2", mcInstance));
                    if (num != 2)
                    {
                        if (num != 3)
                        {
                            ModJava._Closure___11_1 CS_locals2 = new ModJava._Closure___11_1(null);
                            CS_locals2._VB_NonLocal__VB_Closure_2 = CS_locals1;
                            CS_locals2._VB_Local_Range = (Target.Is<ModMinecraft.McInstance>() ? ModJava.GetJavaRequirement(mcInstance).Item1 : Target.As<ValueRange<Version>>());
                            CS_locals2._VB_Local_RecommendedComponent = (Target.Is<ModMinecraft.McInstance>() ? ModJava.GetJavaRequirement(mcInstance).Item2 : null);
                            Logger.Info(string.Format("自动选择以下范围的 Java：{0} {1}", CS_locals2._VB_Local_Range, CS_locals2._VB_Local_RecommendedComponent), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                            if (CS_locals2._VB_Local_Range.IsEmpty())
                            {
                                throw new Exception("Java 版本需求存在冲突，导致没有任何可能适配的 Java，请查看 PCL 的日志了解详细信息！");
                            }
                            VB_AnonymousDelegate_0<Java> vb_AnonymousDelegate_ = new VB_AnonymousDelegate_0<Java>(CS_locals2._Lambda___0);
                            if (p != null)
                            {
                                p.Set(0.25, false, ProgressProvider.ChildrenAction.Finished);
                            }
                            Java java = vb_AnonymousDelegate_();
                            if (java != null)
                            {
                                if (p != null)
                                {
                                    p.Skip(ProgressProvider.ChildrenAction.Skiped);
                                }
                                java2 = java;
                            }
                            else
                            {
                                object javaLock = ModJava.JavaLock;
                                ObjectFlowControl.CheckForSyncLockOnValueType(javaLock);
                                lock (javaLock)
                                {
                                    Logger.Warn("既有列表中没有合适的 Java，重新触发搜索", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                                    ModJava.JavaListRefreshWorker.Start(CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_c, (p != null) ? p.SplitTo(0.3) : null);
                                    java = vb_AnonymousDelegate_();
                                    if (java != null)
                                    {
                                        if (p != null)
                                        {
                                            p.Skip(ProgressProvider.ChildrenAction.Skiped);
                                        }
                                        java2 = java;
                                    }
                                    else if (!TryFixOrHint)
                                    {
                                        java2 = null;
                                    }
                                    else
                                    {
                                        Logger.Warn("电脑中没有合适的 Java，开始获取 Mojang 提供的 Java 下载列表", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                                        if (ModJava._STATIC_SelectOrDownloadJava_041282411511824D21512824511282491287E821180D9128251_DownloadListWorker_Init == null)
                                        {
                                            Interlocked.CompareExchange<StaticLocalInitFlag>(ref ModJava._STATIC_SelectOrDownloadJava_041282411511824D21512824511282491287E821180D9128251_DownloadListWorker_Init, new StaticLocalInitFlag(), null);
                                        }
                                        bool flag2 = false;
                                        try
                                        {
                                            Monitor.Enter(ModJava._STATIC_SelectOrDownloadJava_041282411511824D21512824511282491287E821180D9128251_DownloadListWorker_Init, ref flag2);
                                            if (ModJava._STATIC_SelectOrDownloadJava_041282411511824D21512824511282491287E821180D9128251_DownloadListWorker_Init.State == 0)
                                            {
                                                ModJava._STATIC_SelectOrDownloadJava_041282411511824D21512824511282491287E821180D9128251_DownloadListWorker_Init.State = 2;
                                                ModJava._STATIC_SelectOrDownloadJava_041282411511824D21512824511282491287E821180D9128251_DownloadListWorker = new RedoableWorker<List<ModJava.JavaDownloadInfo>>((ModJava._Closure___._I11_1 == null) ? (ModJava._Closure___._I11_1 = new Func<CancellationToken?, List<ModJava.JavaDownloadInfo>>(ModJava._Closure___._I._Lambda___11_1)) : ModJava._Closure___._I11_1, "SelectOrDownloadJava");
                                            }
                                            else if (ModJava._STATIC_SelectOrDownloadJava_041282411511824D21512824511282491287E821180D9128251_DownloadListWorker_Init.State == 2)
                                            {
                                                throw new IncompleteInitialization();
                                            }
                                        }
                                        finally
                                        {
                                            ModJava._STATIC_SelectOrDownloadJava_041282411511824D21512824511282491287E821180D9128251_DownloadListWorker_Init.State = 1;
                                            if (flag2)
                                            {
                                                Monitor.Exit(ModJava._STATIC_SelectOrDownloadJava_041282411511824D21512824511282491287E821180D9128251_DownloadListWorker_Init);
                                            }
                                        }
                                        if (!ModJava._STATIC_SelectOrDownloadJava_041282411511824D21512824511282491287E821180D9128251_DownloadListWorker.HasSucceeded)
                                        {
                                            ModJava._STATIC_SelectOrDownloadJava_041282411511824D21512824511282491287E821180D9128251_DownloadListWorker.Start(CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_c, (p != null) ? p.SplitTo(0.4) : null);
                                        }
                                        ModJava._STATIC_SelectOrDownloadJava_041282411511824D21512824511282491287E821180D9128251_DownloadListWorker.WaitIfRunning(-1, CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_c);
                                        if (p != null)
                                        {
                                            p.Set(0.4, true, ProgressProvider.ChildrenAction.Finished);
                                        }
                                        ModJava.JavaDownloadInfo javaDownloadInfo = ModJava._STATIC_SelectOrDownloadJava_041282411511824D21512824511282491287E821180D9128251_DownloadListWorker.LastResult.FirstOrDefault<ModJava.JavaDownloadInfo>(new Func<ModJava.JavaDownloadInfo, bool>[]
                                        {
                                            new Func<ModJava.JavaDownloadInfo, bool>(CS_locals2._Lambda___2),
                                            new Func<ModJava.JavaDownloadInfo, bool>(CS_locals2._Lambda___3)
                                        });
                                        if (javaDownloadInfo == null)
                                        {
                                            Logger.Warn("Mojang 提供的 Java 列表中没有合适的 Java，无法自动下载", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                                            bool flag3 = CS_locals2._VB_Local_Range.HasLower && CS_locals2._VB_Local_Range.IsLowerInclusive && CS_locals2._VB_Local_Range.Lower.Minor <= 0 && CS_locals2._VB_Local_Range.Lower.Build <= 0 && CS_locals2._VB_Local_Range.Lower.Revision <= 0;
                                            bool flag4 = CS_locals2._VB_Local_Range.HasUpper && !CS_locals2._VB_Local_Range.IsUpperInclusive && CS_locals2._VB_Local_Range.Upper.Minor <= 0 && CS_locals2._VB_Local_Range.Upper.Build <= 0 && CS_locals2._VB_Local_Range.Upper.Revision <= 0;
                                            string text;
                                            if (CS_locals2._VB_Local_Range.HasLower && CS_locals2._VB_Local_Range.HasUpper)
                                            {
                                                if (flag3 && flag4)
                                                {
                                                    if (CS_locals2._VB_Local_Range.Lower.Major == CS_locals2._VB_Local_Range.Upper.Major - 1)
                                                    {
                                                        text = string.Format(" Java {0}", CS_locals2._VB_Local_Range.Lower.Major);
                                                    }
                                                    else
                                                    {
                                                        text = string.Format(" Java {0} ~ {1}", CS_locals2._VB_Local_Range.Lower.Major, CS_locals2._VB_Local_Range.Upper.Major - 1);
                                                    }
                                                }
                                                else
                                                {
                                                    text = string.Format(" Java {0}", CS_locals2._VB_Local_Range);
                                                }
                                            }
                                            else if (CS_locals2._VB_Local_Range.HasLower)
                                            {
                                                if (flag3)
                                                {
                                                    text = string.Format(" Java {0} 或更高版本的 Java", CS_locals2._VB_Local_Range.Lower.Major);
                                                }
                                                else if (CS_locals2._VB_Local_Range.IsLowerInclusive)
                                                {
                                                    text = string.Format(" Java {0} 或更高版本的 Java", CS_locals2._VB_Local_Range.Lower);
                                                }
                                                else
                                                {
                                                    text = string.Format("版本高于 {0} 的 Java", CS_locals2._VB_Local_Range.Lower);
                                                }
                                            }
                                            else if (flag4)
                                            {
                                                text = string.Format(" Java {0} 或更低版本的 Java", CS_locals2._VB_Local_Range.Upper.Major - 1);
                                            }
                                            else if (CS_locals2._VB_Local_Range.IsUpperInclusive)
                                            {
                                                text = string.Format(" Java {0} 或更低版本的 Java", CS_locals2._VB_Local_Range.Upper);
                                            }
                                            else
                                            {
                                                text = string.Format("版本低于 {0} 的 Java", CS_locals2._VB_Local_Range.Upper);
                                            }
                                            ModMain.OurMsgBox(string.Format("PCL 没能找到合适的 Java。{0}请在网上搜索并手动安装{1}， 然后在【设置 → 启动 → 高级选项 → Java 列表】中导入它。", "\r\n", text), "需要手动安装 Java", "确定", "", "", false, true, false, null, null, null);
                                            java2 = null;
                                        }
                                        else
                                        {
                                            ModLoader.LoaderCombo<int> loaderCombo = ModJava.CreateJavaDownloadLoader(javaDownloadInfo);
                                            try
                                            {
                                                loaderCombo.WaitForExit(null, null, true, CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_c, (p != null) ? p.SplitTo(0.95) : null);
                                            }
                                            finally
                                            {
                                                loaderCombo.Cancel();
                                            }
                                            java = vb_AnonymousDelegate_();
                                            if (java == null)
                                            {
                                                throw new Exception(string.Format("下载 Java {0} 后仍然无法找到合适的 Java", javaDownloadInfo.ComponentName));
                                            }
                                            if (p != null)
                                            {
                                                p.Finish(ProgressProvider.ChildrenAction.Finished);
                                            }
                                            java2 = java;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Java java3 = Configs.JavaForced.Get(mcInstance.Config);
                            Logger.Info(string.Format("版本设置中强制指定的 Java：{0} → {1}", mcInstance.PathVersion, java3), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                            if (java3 == null)
                            {
                                if (TryFixOrHint)
                                {
                                    ModJava.SwitchToInstanceSetup(mcInstance);
                                    ModMain.OurMsgBox(string.Format("该版本的版本设置选择了 {0}使用指定的 Java{1}，但还没有指定任何 Java。{2}请先选择你想使用的 Java，然后再试。", ModBase.vbLQ, ModBase.vbRQ, "\r\n"), "未选择 Java", "确定", "", "", true, true, true, null, null, null);
                                }
                                if (p != null)
                                {
                                    p.Skip(ProgressProvider.ChildrenAction.Skiped);
                                }
                                java2 = null;
                            }
                            else if (!Configs.JavaList.Get().Contains(java3))
                            {
                                Configs.JavaForced.Reset(mcInstance.Config);
                                ModJava.UpdateJavaLists();
                                if (TryFixOrHint)
                                {
                                    ModJava.SwitchToInstanceSetup(mcInstance);
                                    Logger.Error("版本设置中指定的 Java 不在 Java 列表中，请重新选择一个 Java！", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                                }
                                if (p != null)
                                {
                                    p.Skip(ProgressProvider.ChildrenAction.Skiped);
                                }
                                java2 = null;
                            }
                            else if (!java3.CheckAsync(CS_locals1._VB_Local_c).Run<bool>())
                            {
                                Configs.JavaForced.Reset(mcInstance.Config);
                                ModJava.UpdateJavaLists();
                                if (TryFixOrHint)
                                {
                                    ModJava.SwitchToInstanceSetup(mcInstance);
                                    Logger.Error("版本设置中指定的 Java 已无法使用，请重新选择一个 Java，然后再试。", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                                }
                                else
                                {
                                    Logger.Error("版本设置中指定的 Java 已无法使用，请重新选择一个 Java！", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                                }
                                if (p != null)
                                {
                                    p.Finish(ProgressProvider.ChildrenAction.Finished);
                                }
                                java2 = null;
                            }
                            else
                            {
                                if (p != null)
                                {
                                    p.Finish(ProgressProvider.ChildrenAction.Finished);
                                }
                                java2 = java3;
                            }
                        }
                    }
                    else
                    {
                        Logger.Info(string.Format("版本设置要求使用版本文件夹中的 Java：{0}", mcInstance.PathVersion), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                        Java java4 = JavaUtils.SearchFoldersAsync(true, new string[] { mcInstance.PathVersion }, CS_locals1._VB_Local_c, (p != null) ? p.SplitTo(1.0) : null).Run<List<Java>>().FirstOrDefault<Java>();
                        if (java4 != null)
                        {
                            Logger.Info(string.Format("已发现版本文件夹中的 Java：{0}", java4), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                            java2 = java4;
                        }
                        else
                        {
                            if (TryFixOrHint)
                            {
                                ModJava.SwitchToInstanceSetup(mcInstance);
                                ModMain.OurMsgBox(string.Format("该版本的版本设置选择了 {0}使用版本文件夹中的 Java{1}，但版本文件夹里没能找到 Java。{2}请先修改启动选项中的 Java 设置，或在版本文件夹中放一个 Java，然后再试。", ModBase.vbLQ, ModBase.vbRQ, "\r\n"), "未找到 Java", "确定", "", "", true, true, true, null, null, null);
                            }
                            java2 = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.ThrowIfCanceled();
                    Logger.Error(ex, "选择 Java 时出现异常", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                    if (p != null)
                    {
                        p.Skip(ProgressProvider.ChildrenAction.Skiped);
                    }
                    java2 = null;
                }
                return java2;
            }
        }

        private static void SwitchToInstanceSetup(ModMinecraft.McInstance Instance)
        {
            ModJava._Closure___12_0 CS_locals1 = new ModJava._Closure___12_0();
            CS_locals1._VB_Local_Instance = Instance;
            ModBase.RunInUi(new Action(CS_locals1._Lambda___0), false);
        }

        private static void MigrateInstanceJavaSettings(ModMinecraft.McInstance Instance, CancellationToken c = default(CancellationToken), ProgressProvider p = null)
        {
            if (Configs.JavaMigrated.Get(Instance.Config))
            {
                if (p != null)
                {
                    p.Skip(ProgressProvider.ChildrenAction.Skiped);
                    return;
                }
            }
            else
            {
                if (ModMain.FrmInstanceSetup != null)
                {
                    ModBase.RunInUiWait((ModJava._Closure___._I14_0 == null) ? (ModJava._Closure___._I14_0 = new Action(ModJava._Closure___._I._Lambda___14_0)) : ModJava._Closure___._I14_0);
                }
                try
                {
                    ModJava._Closure___14_0 CS_locals1 = new ModJava._Closure___14_0();
                    Logger.Info(string.Format("正在尝试迁移版本独立的 Java 设置（{0}）", Instance.Name), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                    string text = Settings.Get<string>("VersionArgumentJavaSelect", Instance);
                    if (!text.StartsWithF("{", false))
                    {
                        if (p != null)
                        {
                            p.Set(0.4, true, ProgressProvider.ChildrenAction.Finished);
                        }
                    }
                    else
                    {
                        CS_locals1._VB_Local_ForcedJava = new Java(text.DeserializeJson<JObject>()["Path"].ToString(), null);
                        if (!CS_locals1._VB_Local_ForcedJava.CheckAsync(c).Run<bool>())
                        {
                            ModMain.Hint("该版本的设置中强制指定的 Java 存在异常，该设置已重置", ModMain.HintType.Red, true);
                            if (p != null)
                            {
                                p.Set(0.4, false, ProgressProvider.ChildrenAction.Finished);
                            }
                        }
                        else
                        {
                            Logger.Info(string.Format("将版本独立设置改为老版本中强制指定的 Java：{0}", CS_locals1._VB_Local_ForcedJava), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                            if (!Configs.JavaList.Get().Contains(CS_locals1._VB_Local_ForcedJava))
                            {
                                Configs.JavaList.Edit(new ConfigEntry<ConcurrentList<Java>>.RefAction(CS_locals1._Lambda___1));
                                Logger.Info(string.Format("已将强制指定的 Java 加入 Java 列表底部：{0}", CS_locals1._VB_Local_ForcedJava), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                            }
                            Configs.JavaForced.Set(CS_locals1._VB_Local_ForcedJava, Instance.Config);
                            Settings.Set("VersionArgumentJavaV2", 3, Instance);
                            ModJava.UpdateJavaLists();
                            Configs.JavaMigrated.Set(true, Instance.Config);
                            if (p == null)
                            {
                                return;
                            }
                            p.Finish(ProgressProvider.ChildrenAction.Finished);
                            return;
                        }
                    }
                    List<Java> list = JavaUtils.SearchFoldersAsync(true, new string[] { Instance.PathVersion }, c, (p != null) ? p.SplitTo(1.0) : null).Run<List<Java>>();
                    if (list.Any<Java>())
                    {
                        Logger.Info(string.Format("在版本文件夹中发现了 Java，将版本独立设置改为优先使用版本文件夹的 Java：{0}", list), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                        Settings.Set("VersionArgumentJavaV2", 2, Instance);
                    }
                    else
                    {
                        Logger.Info("没有需要迁移的内容，自动选择 Java", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                        Settings.Set("VersionArgumentJavaV2", 0, Instance);
                    }
                    Configs.JavaMigrated.Set(true, Instance.Config);
                }
                finally
                {
                    if (ModMain.FrmInstanceSetup != null)
                    {
                        ModBase.RunInUiWait((ModJava._Closure___._I14_2 == null) ? (ModJava._Closure___._I14_2 = new Action(ModJava._Closure___._I._Lambda___14_2)) : ModJava._Closure___._I14_2);
                    }
                }
            }
        }

        private static ModLoader.LoaderCombo<int> CreateJavaDownloadLoader(ModJava.JavaDownloadInfo Target)
        {
            ModJava._Closure___16_0 CS_locals1 = new ModJava._Closure___16_0(null);
            string text = (string)Target.Java["manifest"]["url"];
            Logger.Info(string.Format("准备下载 Java {0}（{1}）：{2}", Target.Java["version"]["name"], Target.ComponentName, text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
            string text2 = ModNet.NetRequestByLoader(ModDownload.DlSourceOrder(new string[] { text }, new string[] { text.Replace("piston-meta.mojang.com", "bmclapi2.bangbang93.com") }), 45000, true, false);
            CS_locals1._VB_Local_BaseDir = string.Format("{0}.minecraft\\runtime\\{1}\\", Paths.AppData, Target.ComponentName);
            List<ModNet.NetFile> list = new List<ModNet.NetFile>();
            try
            {
                foreach (JToken jtoken in ((IEnumerable<JToken>)((JObject)text2.DeserializeJson())["files"]))
                {
                    JProperty jproperty = (JProperty)jtoken;
                    JToken jtoken2 = ((JObject)jproperty.Value)["downloads"];
                    if (((jtoken2 != null) ? jtoken2["raw"] : null) != null)
                    {
                        JObject jobject = (JObject)((JObject)jproperty.Value)["downloads"]["raw"];
                        FileChecker fileChecker = new FileChecker
                        {
                            ActualSize = (long)jobject["size"],
                            Hash = (string)jobject["sha1"]
                        };
                        if (Operators.CompareString(fileChecker.Hash, "12976a6c2b227cbac58969c1455444596c894656", false) != 0 && Operators.CompareString(fileChecker.Hash, "c80e4bab46e34d02826eab226a4441d0970f2aba", false) != 0 && Operators.CompareString(fileChecker.Hash, "84d2102ad171863db04e7ee22a259d1f6c5de4a5", false) != 0)
                        {
                            string text3 = (string)jobject["url"];
                            list.Add(new ModNet.NetFile(ModDownload.DlSourceOrder(new string[] { text3 }, new string[] { text3.Replace("piston-data.mojang.com", "bmclapi2.bangbang93.com") }), CS_locals1._VB_Local_BaseDir + jproperty.Name, fileChecker, false));
                        }
                    }
                }
            }
            finally
            {
                IEnumerator<JToken> enumerator = default;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            Logger.Info(string.Format("该 Java 需要下载 {0} 个文件，目标文件夹：{1}", list.Count, CS_locals1._VB_Local_BaseDir), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
            return new ModLoader.LoaderCombo<int>("下载 Java", new ModLoader.LoaderBase[]
            {
                new ModNet.LoaderDownload("下载 Java 文件", list)
                {
                    ProgressWeight = 10.0
                },
                new ModLoader.LoaderWorker("刷新 Java 列表", ModJava.JavaListRefreshWorker)
            })
            {
                OnStateChanged = new Action<ModLoader.LoaderBase>(CS_locals1._Lambda___R1)
            };
        }

        [CompilerGenerated]
        [AccessedThroughProperty("JavaListRefreshWorker")]
        private static RedoableWorker _JavaListRefreshWorker;

        private static readonly object JavaLock = RuntimeHelpers.GetObjectValue(new object());

        private static RedoableWorker<List<ModJava.JavaDownloadInfo>> _STATIC_SelectOrDownloadJava_041282411511824D21512824511282491287E821180D9128251_DownloadListWorker;

        private static StaticLocalInitFlag _STATIC_SelectOrDownloadJava_041282411511824D21512824511282491287E821180D9128251_DownloadListWorker_Init;

        private class JavaDownloadInfo
        {
            public JavaDownloadInfo()
            {
            }

            public string ComponentName
            {
                [CompilerGenerated]
                get
                {
                    return this._ComponentName;
                }
                [CompilerGenerated]
                set
                {
                    this._ComponentName = value;
                }
            }

            public JObject Java
            {
                [CompilerGenerated]
                get
                {
                    return this._Java;
                }
                [CompilerGenerated]
                set
                {
                    this._Java = value;
                }
            }

            public Version JavaVersion
            {
                [CompilerGenerated]
                get
                {
                    return this._JavaVersion;
                }
                [CompilerGenerated]
                set
                {
                    this._JavaVersion = value;
                }
            }

            [CompilerGenerated]
            private string _ComponentName;

            [CompilerGenerated]
            private JObject _Java;

            [CompilerGenerated]
            private Version _JavaVersion;
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

            internal void _Lambda___0_0(CancellationToken? c, ProgressProvider p)
            {
                JavaUtils.RefreshListAsync(c.Value, p).Run();
            }

            internal Java _Lambda___1_0(JToken JavaObject)
            {
                return new Java(JavaObject["Path"].ToString(), null);
            }

            internal string _Lambda___1_2(Java j)
            {
                return j.ToString();
            }

            internal void _Lambda___6_0()
            {
                PageSetupLaunch frmSetupLaunch = ModMain.FrmSetupLaunch;
                if (frmSetupLaunch != null)
                {
                    frmSetupLaunch.UpdateJavaList();
                }
                PageInstanceSetup frmInstanceSetup = ModMain.FrmInstanceSetup;
                if (frmInstanceSetup == null)
                {
                    return;
                }
                frmInstanceSetup.UpdateJavaList();
            }

            internal string _Lambda___8_2(string s)
            {
                return s.Lower();
            }

            internal void _Lambda___9_1(Java java)
            {
                java.InvalidateChecked();
            }

            internal Version _Lambda___10_0(string s)
            {
                return StringUtils.ParseVersionWithDefaults(s);
            }

            internal List<ModJava.JavaDownloadInfo> _Lambda___11_1(CancellationToken? cancellationToken)
            {
                string text = ModNet.NetRequestByLoader(ModDownload.DlVersionListOrder(new string[] { "https://piston-meta.mojang.com/v1/products/java-runtime/2ec0cc96c44e5a76b9c8b7c39df7210883d12871/all.json" }, new string[] { "https://bmclapi2.bangbang93.com/v1/products/java-runtime/2ec0cc96c44e5a76b9c8b7c39df7210883d12871/all.json" }), 45000, true, false);
                if (cancellationToken != null)
                {
                    cancellationToken.GetValueOrDefault().ThrowIfCancellationRequested();
                }
                List<ModJava.JavaDownloadInfo> list = new List<ModJava.JavaDownloadInfo>();
                checked
                {
                    try
                    {
                        foreach (JToken jtoken in ((IEnumerable<JToken>)text.DeserializeJson<JObject>()["windows-x64"]))
                        {
                            JProperty jproperty = (JProperty)jtoken;
                            if (cancellationToken != null)
                            {
                                cancellationToken.GetValueOrDefault().ThrowIfCancellationRequested();
                            }
                            if (Operators.CompareString(jproperty.Name, "minecraft-java-exe", false) != 0)
                            {
                                try
                                {
                                    JToken jtoken2 = jproperty.Value.FirstOrDefault<JToken>();
                                    if (jtoken2 != null)
                                    {
                                        string text2 = jtoken2["version"]["name"].ToString();
                                        if (text2.Contains("."))
                                        {
                                            string[] array = text2.Split(".", false);
                                            list.Add(new ModJava.JavaDownloadInfo
                                            {
                                                ComponentName = jproperty.Name,
                                                Java = (JObject)jtoken2,
                                                JavaVersion = new Version((int)Math.Round(ModBase.Val(array[0])), (int)Math.Round(ModBase.Val(array[1])), (int)Math.Round((array.Length >= 3) ? ModBase.Val(array[2]) : 0.0), (int)Math.Round((array.Length >= 4) ? ModBase.Val(array[3]) : 0.0))
                                            });
                                        }
                                        else if (text2.StartsWithF("8u51-", false))
                                        {
                                            list.Add(new ModJava.JavaDownloadInfo
                                            {
                                                ComponentName = jproperty.Name,
                                                Java = (JObject)jtoken2,
                                                JavaVersion = new Version(8, 0, 51, 0)
                                            });
                                        }
                                        else
                                        {
                                            list.Add(new ModJava.JavaDownloadInfo
                                            {
                                                ComponentName = jproperty.Name,
                                                Java = (JObject)jtoken2,
                                                JavaVersion = new Version((int)Math.Round(ModBase.Val(text2.BeforeFirstOfAny(new string[] { "u", "." }, false))), 0, 0, 0)
                                            });
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Warn(ex, string.Format("解析 Mojang 提供的 Java 列表项失败，已跳过（{0}）", jproperty.Name), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                                }
                            }
                        }
                    }
                    finally
                    {
                        IEnumerator<JToken> enumerator = default;
                        if (enumerator != null)
                        {
                            enumerator.Dispose();
                        }
                    }
                    return list;
                }
            }

            internal void _Lambda___14_0()
            {
                ModMain.FrmInstanceSetup.ComboArgumentJava.IsEnabled = false;
            }

            internal void _Lambda___14_2()
            {
                SettingService.RefreshSettings(ModMain.FrmInstanceSetup.ComboArgumentJava);
                SettingService.RefreshSettings(ModMain.FrmInstanceSetup.ComboArgumentJavaSelect);
                ModMain.FrmInstanceSetup.ComboArgumentJava.IsEnabled = true;
            }

            public static readonly ModJava._Closure___ _I = new ModJava._Closure___();

            public static Func<JToken, Java> _I1_0;

            public static Func<Java, string> _I1_2;

            public static Action _I6_0;

            public static Func<string, string> _I8_2;

            public static Action<Java> _I9_1;

            public static Func<string, Version> _I10_0;

            public static Func<CancellationToken?, List<ModJava.JavaDownloadInfo>> _I11_1;

            public static Action _I14_0;

            public static Action _I14_2;
        }

        [CompilerGenerated]
        private sealed class _Closure___1_0
        {
            public _Closure___1_0()
            {
            }

            internal bool _Lambda___1(Java j)
            {
                return PathUtils.IsSame(j.Folder, this._VB_Local_SelectedJava);
            }

            public string _VB_Local_SelectedJava;
        }

        [CompilerGenerated]
        private sealed class _Closure___10_0
        {
            public _Closure___10_0()
            {
            }

            internal void _Lambda___1(ValueRange<Version> Constraint)
            {
                ValueRange<Version> valueRange = this._VB_Local_JavaRange;
                this._VB_Local_JavaRange = ((valueRange != null) ? valueRange.Intersect(Constraint) : null);
                if (this._VB_Local_JavaRange.IsEmpty())
                {
                    Logger.Warn(string.Format("Java 版本要求冲突，若启动失败请在版本设置手动指定 Java：当前追加的要求为 {0}，游戏版本为 {1}", Constraint, this._VB_Local_Instance.VersionDisplayName()), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                    this._VB_Local_JavaRange = Constraint;
                }
            }

            public ValueRange<Version> _VB_Local_JavaRange;

            public ModMinecraft.McInstance _VB_Local_Instance;
        }

        [CompilerGenerated]
        private sealed class _Closure___11_0
        {
            public _Closure___11_0(ModJava._Closure___11_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_c = arg0._VB_Local_c;
                }
            }

            public CancellationToken _VB_Local_c;
        }

        [CompilerGenerated]
        private sealed class _Closure___11_1
        {
            public _Closure___11_1(ModJava._Closure___11_1 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Range = arg0._VB_Local_Range;
                    this._VB_Local_RecommendedComponent = arg0._VB_Local_RecommendedComponent;
                }
            }

            internal Java _Lambda___0()
            {
                ModJava.JavaListRefreshWorker.WaitIfRunning(-1, default(CancellationToken));
                try
                {
                    foreach (Java java in Configs.JavaList.Get())
                    {
                        if (!java.CheckAsync(this._VB_NonLocal__VB_Closure_2._VB_Local_c).Run<bool>())
                        {
                            Logger.Warn(string.Format("Java 检查失败，将尝试列表中的下一个 Java：{0}", java), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                        }
                        else if (java.Version != null && this._VB_Local_Range.Contains(java.Version))
                        {
                            Logger.Info(string.Format("已选定 Java：{0}", java), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                            return java;
                        }
                    }
                }
                finally
                {
                    IEnumerator<Java> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                return null;
            }

            internal bool _Lambda___2(ModJava.JavaDownloadInfo Comp)
            {
                return Operators.CompareString(Comp.Java["version"]["name"].ToString(), this._VB_Local_RecommendedComponent, false) == 0;
            }

            internal bool _Lambda___3(ModJava.JavaDownloadInfo Comp)
            {
                return this._VB_Local_Range.Contains(Comp.JavaVersion);
            }

            public ValueRange<Version> _VB_Local_Range;

            public string _VB_Local_RecommendedComponent;

            public ModJava._Closure___11_0 _VB_NonLocal__VB_Closure_2;
        }

        [CompilerGenerated]
        private sealed class _Closure___12_0
        {
            public _Closure___12_0()
            {
            }

            internal void _Lambda___0()
            {
                PageInstanceLeft.Instance = this._VB_Local_Instance;
                ModMain.FrmMain.PageChange(FormMain.PageType.InstanceSetup, FormMain.PageSubType.SetupLink);
            }

            public ModMinecraft.McInstance _VB_Local_Instance;
        }

        [CompilerGenerated]
        private sealed class _Closure___14_0
        {
            public _Closure___14_0()
            {
            }

            internal void _Lambda___1(ref ConcurrentList<Java> List)
            {
                List.Add(this._VB_Local_ForcedJava);
            }

            public Java _VB_Local_ForcedJava;
        }

        [CompilerGenerated]
        private sealed class _Closure___16_0
        {
            public _Closure___16_0(ModJava._Closure___16_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_BaseDir = arg0._VB_Local_BaseDir;
                }
            }

            internal void _Lambda___R1(ModLoader.LoaderBase a0)
            {
                this._Lambda___0((ModLoader.LoaderCombo<int>)a0);
            }

            internal void _Lambda___0(ModLoader.LoaderCombo<int> RawLoader)
            {
                if (RawLoader.State == ModBase.LoadState.Failed || RawLoader.State == ModBase.LoadState.Canceled)
                {
                    Logger.Warn(string.Format("由于 Java 下载未完成，清理未下载完成的 Java 文件夹：{0}", this._VB_Local_BaseDir), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                    DirectoryUtils.Delete(this._VB_Local_BaseDir, false);
                }
            }

            public string _VB_Local_BaseDir;
        }

        [CompilerGenerated]
        private sealed class _Closure___7_0
        {
            public _Closure___7_0()
            {
            }

            internal void _Lambda___0(ref List<string> List)
            {
                List.RemoveIf<string>((this._I1 == null) ? (this._I1 = new Func<string, bool>(this._Lambda___1)) : this._I1);
            }

            internal bool _Lambda___1(string f)
            {
                return PathUtils.IsSame(f, this._VB_Local_NewJava.Folder);
            }

            internal void _Lambda___2(ref ConcurrentList<Java> List)
            {
                List.Remove(this._VB_Local_NewJava);
                List.Insert(0, this._VB_Local_NewJava);
            }

            public Java _VB_Local_NewJava;

            public Func<string, bool> _I1;
        }

        [CompilerGenerated]
        private sealed class _Closure___8_0
        {
            public _Closure___8_0()
            {
            }

            internal void _Lambda___0(ref ConcurrentList<Java> List)
            {
                List.Remove(this._VB_Local_JavaEntry);
            }

            internal List<string> _Lambda___1(List<string> List)
            {
                return List.Append(this._VB_Local_JavaEntry.Folder).DistinctBy<string, string>((ModJava._Closure___._I8_2 == null) ? (ModJava._Closure___._I8_2 = new Func<string, string>(ModJava._Closure___._I._Lambda___8_2)) : ModJava._Closure___._I8_2).ToList<string>();
            }

            public Java _VB_Local_JavaEntry;
        }

        [CompilerGenerated]
        private sealed class _Closure___9_0
        {
            public _Closure___9_0()
            {
            }

            internal void _Lambda___0()
            {
                Configs.JavaList.Get().ForAll<Java>((ModJava._Closure___._I9_1 == null) ? (ModJava._Closure___._I9_1 = new Action<Java>(ModJava._Closure___._I._Lambda___9_1)) : ModJava._Closure___._I9_1);
                ModJava.JavaListRefreshWorker.Start(this._VB_Local_CancellationToken, null);
                ModJava.JavaListRefreshWorker.WaitIfRunningAsync(-1, this._VB_Local_CancellationToken).Run<bool>();
                if (Configs.JavaList.Get().Any<Java>())
                {
                    ModMain.Hint("已找到 " + Conversions.ToString(Configs.JavaList.Get().Count) + " 个 Java！", ModMain.HintType.Green, true);
                    return;
                }
                ModMain.Hint("未找到可用的 Java！", ModMain.HintType.Red, true);
            }

            public CancellationToken _VB_Local_CancellationToken;
        }
    }
}
