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
    public sealed class ModJava
    {
        public static void JavaInit()
        {
            try
            {
                if (Configs.JavaConfigVersion.Get() == 0)
                {
                    try
                    {
                        Logger.Warn("正在从老版本迁移 Java 列表", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                        List<Java> list = Settings.Get<string>("LaunchArgumentJavaAll", null).DeserializeJson<JArray>().Select<JToken, Java>((ModJava._Closure$__.$I1-0 == null) ? (ModJava._Closure$__.$I1-0 = (JToken JavaObject) => new Java(JavaObject["Path"].ToString(), null)) : ModJava._Closure$__.$I1-0)
                            .ToList<Java>();
                        list = JavaUtils.SortAsync(list, default(CancellationToken), null).Run<List<Java>>();
                        Logger.Info(string.Format("已从老版本设置中发现 {0} 个 Java", list.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                        string text = Settings.Get<string>("LaunchArgumentJavaSelect", null);
                        string SelectedJava = (text.StartsWithF("{", false) ? text.DeserializeJson<JObject>()["Path"].ToString() : "");
                        Java java = list.FirstOrDefault<Java>((Java j) => PathUtils.IsSame(j.Folder, SelectedJava));
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
                    Logger.Info(string.Format("共有 {0} 个可用的 Java：{1}- {2}", concurrentList.Count, "\r\n", concurrentList.Select<Java, string>((ModJava._Closure$__.$I1-2 == null) ? (ModJava._Closure$__.$I1-2 = (Java j) => j.ToString()) : ModJava._Closure$__.$I1-2).Join("\r\n- ")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
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
        } = new RedoableWorker(delegate(CancellationToken? c, ProgressProvider p)
        {
            JavaUtils.RefreshListAsync(c.Value, p).Run();
        }, "JavaListRefreshWorker");

        public static void UpdateJavaLists()
        {
            ModBase.RunInUi((ModJava._Closure$__.$I6-0 == null) ? (ModJava._Closure$__.$I6-0 = delegate
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
            }) : ModJava._Closure$__.$I6-0, false);
        }

        public static void ManuallyImportJava()
        {
            string text = Dialogs.SelectFile("选择 java.exe（可能在 bin 文件夹中）", false, null, new OneOf<IEnumerable<ValueTuple<string[], string>>, string>?("java.exe|java.exe")).FirstOrDefault<string>();
            if (!string.IsNullOrEmpty(text))
            {
                Java NewJava = new Java(PathUtils.RemoveLastPart(text), null);
                if (!NewJava.CheckAsync(default(CancellationToken)).Run<bool>())
                {
                    ModMain.Hint("该 Java 存在异常，无法使用", ModMain.HintType.Red, true);
                    return;
                }
                Func<string, bool> $I1;
                Configs.JavaRemovedList.Edit(delegate(ref List<string> List)
                {
                    List.RemoveIf<string>(($I1 == null) ? ($I1 = (string f) => PathUtils.IsSame(f, NewJava.Folder)) : $I1);
                });
                Configs.JavaList.Edit(delegate(ref ConcurrentList<Java> List)
                {
                    List.Remove(NewJava);
                    List.Insert(0, NewJava);
                });
                ModMain.Hint(string.Format("已将 Java 加入列表：{0}", text), ModMain.HintType.Green, true);
                ModJava.UpdateJavaLists();
            }
        }

        public static void ManuallyRemoveJava(Java JavaEntry)
        {
            Configs.JavaList.Edit(delegate(ref ConcurrentList<Java> List)
            {
                List.Remove(JavaEntry);
            });
            Configs.JavaRemovedList.Edit((List<string> List) => List.Append(JavaEntry.Folder).DistinctBy<string, string>((ModJava._Closure$__.$I8-2 == null) ? (ModJava._Closure$__.$I8-2 = (string s) => s.Lower()) : ModJava._Closure$__.$I8-2).ToList<string>());
            ModMain.Hint(string.Format("已将 Java 移除出列表：{0}", JavaEntry), ModMain.HintType.Green, true);
            ModJava.UpdateJavaLists();
            PageInstanceLeft.ReloadCurrentJava();
        }

        public static void ManuallySearchJava(CancellationToken CancellationToken = default(CancellationToken))
        {
            ModBase.RunInThread(delegate
            {
                Configs.JavaList.Get().ForAll<Java>((ModJava._Closure$__.$I9-1 == null) ? (ModJava._Closure$__.$I9-1 = delegate(Java java)
                {
                    java.InvalidateChecked();
                }) : ModJava._Closure$__.$I9-1);
                ModJava.JavaListRefreshWorker.Start(CancellationToken, null);
                ModJava.JavaListRefreshWorker.WaitIfRunningAsync(-1, CancellationToken).Run<bool>();
                if (Configs.JavaList.Get().Any<Java>())
                {
                    ModMain.Hint("已找到 " + Conversions.ToString(Configs.JavaList.Get().Count) + " 个 Java！", ModMain.HintType.Green, true);
                    return;
                }
                ModMain.Hint("未找到可用的 Java！", ModMain.HintType.Red, true);
            });
        }

        [return: TupleElementNames(new string[] { "Range", "RecommendedComponent" })]
        public static ValueTuple<ValueRange<Version>, string> GetJavaRequirement(ModMinecraft.McInstance Instance)
        {
            if (Settings.Get<int>("VersionArgumentJavaV2", Instance) == 1)
            {
                try
                {
                    ValueRange<Version> valueRange = ValueRange<Version>.FromString(Settings.Get<string>("VersionArgumentJavaRange", Instance), (ModJava._Closure$__.$I10-0 == null) ? (ModJava._Closure$__.$I10-0 = (string s) => StringUtils.ParseVersionWithDefaults(s)) : ModJava._Closure$__.$I10-0);
                    return new ValueTuple<ValueRange<Version>, string>(valueRange, null);
                }
                catch (Exception ex)
                {
                    Logger.Warn(string.Format("版本设置中指定的 Java 版本范围有误（{0}），这可能是因为首次修改此设置，尚未初始化", Settings.Get<string>("VersionArgumentJavaRange", Instance)), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                }
            }
            ValueRange<Version> JavaRange = ValueRange<Version>.All();
            VB$AnonymousDelegate_5<ValueRange<Version>> vb$AnonymousDelegate_ = delegate(ValueRange<Version> Constraint)
            {
                ValueRange<Version> $VB$Local_JavaRange = JavaRange;
                JavaRange = (($VB$Local_JavaRange != null) ? $VB$Local_JavaRange.Intersect(Constraint) : null);
                if (JavaRange.IsEmpty())
                {
                    Logger.Warn(string.Format("Java 版本要求冲突，若启动失败请在版本设置手动指定 Java：当前追加的要求为 {0}，游戏版本为 {1}", Constraint, Instance.VersionDisplayName()), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                    JavaRange = Constraint;
                }
            };
            if ((!Instance.Version.Valid && DateTime.Compare(Instance.ReleaseTime, new DateTime(2024, 4, 2)) >= 0) || (Instance.Version.Valid && Instance.Version.Vanilla >= new Version(20, 0, 5)))
            {
                vb$AnonymousDelegate_(ValueRange<Version>.AtLeast(new Version(21, 0)));
            }
            else if ((!Instance.Version.Valid && DateTime.Compare(Instance.ReleaseTime, new DateTime(2021, 11, 16)) >= 0) || (Instance.Version.Valid && Instance.Version.Vanilla.Major >= 18))
            {
                vb$AnonymousDelegate_(ValueRange<Version>.AtLeast(new Version(17, 0)));
            }
            else if ((!Instance.Version.Valid && DateTime.Compare(Instance.ReleaseTime, new DateTime(2021, 5, 11)) >= 0) || (Instance.Version.Valid && Instance.Version.Vanilla.Major >= 17))
            {
                vb$AnonymousDelegate_(ValueRange<Version>.AtLeast(new Version(16, 0)));
            }
            else if (Instance.ReleaseTime.Year >= 2017)
            {
                vb$AnonymousDelegate_(ValueRange<Version>.AtLeast(new Version(8, 0)));
            }
            else if (DateTime.Compare(Instance.ReleaseTime, new DateTime(2013, 5, 1)) <= 0 && Instance.ReleaseTime.Year >= 2001)
            {
                vb$AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(9, 0)));
            }
            string text = null;
            JObject jsonObject = Instance.JsonObject;
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
                JObject jsonVersion = Instance.JsonVersion;
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
                vb$AnonymousDelegate_(ValueRange<Version>.AtLeast(new Version(num6, 0)));
                JObject jsonObject2 = Instance.JsonObject;
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
                    JObject jsonVersion2 = Instance.JsonVersion;
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
                text = obj2;
                if (Operators.CompareString(text, "", false) == 0)
                {
                    text = null;
                }
            }
            if (Instance.Version.HasOptiFine && Instance.Version.Valid)
            {
                if (Instance.Version.Vanilla.Major < 7)
                {
                    vb$AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(9, 0)));
                }
                else if (Instance.Version.Vanilla.Major >= 8 && Instance.Version.Vanilla.Major < 12)
                {
                    vb$AnonymousDelegate_(ValueRange<Version>.ClosedOpen(new Version(8, 0), new Version(9, 0)));
                }
                else if (Instance.Version.Vanilla.Major == 12)
                {
                    vb$AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(9, 0)));
                }
            }
            if (Instance.Version.ModLoaders.HasFlagF(ModLoaders.Forge))
            {
                if (Instance.Version.Vanilla >= new Version(6, 0, 1) && Instance.Version.Vanilla <= new Version(7, 0, 2))
                {
                    vb$AnonymousDelegate_(ValueRange<Version>.ClosedOpen(new Version(7, 0), new Version(8, 0)));
                }
                else if (Instance.Version.Vanilla.Major <= 12 || !Instance.Version.Valid)
                {
                    vb$AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(9, 0)));
                }
                else if (Instance.Version.Vanilla.Major <= 14)
                {
                    vb$AnonymousDelegate_(ValueRange<Version>.ClosedOpen(new Version(8, 0), new Version(11, 0)));
                }
                else if (Instance.Version.Vanilla.Major == 15)
                {
                    vb$AnonymousDelegate_(ValueRange<Version>.ClosedOpen(new Version(8, 0), new Version(16, 0)));
                }
                else if (McUtils.CompareVersionGE(Instance.Version.ModLoaderVersions[ModLoaders.Forge], "34.0.0") && McUtils.CompareVersionGE("36.2.25", Instance.Version.ModLoaderVersions[ModLoaders.Forge]))
                {
                    vb$AnonymousDelegate_(ValueRange<Version>.AtMost(new Version(8, 0, 320)));
                }
                else if (McUtils.CompareVersionGE(Instance.Version.ModLoaderVersions[ModLoaders.Forge], "36.2.26") && McUtils.CompareVersionGE("36.999999.999999", Instance.Version.ModLoaderVersions[ModLoaders.Forge]))
                {
                    vb$AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(24, 0)));
                }
                else if (McUtils.CompareVersionGE(Instance.Version.ModLoaderVersions[ModLoaders.Forge], "37.0.0") && McUtils.CompareVersionGE("37.0.79", Instance.Version.ModLoaderVersions[ModLoaders.Forge]))
                {
                    vb$AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(17, 0)));
                }
                else if (Instance.Version.Vanilla.Major == 18 && Instance.Version.HasOptiFine)
                {
                    vb$AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(19, 0)));
                }
                else if (McUtils.CompareVersionGE(Instance.Version.ModLoaderVersions[ModLoaders.Forge], "45.0.21") && McUtils.CompareVersionGE("45.0.65", Instance.Version.ModLoaderVersions[ModLoaders.Forge]))
                {
                    vb$AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(20, 0)));
                }
                else if (McUtils.CompareVersionGE(Instance.Version.ModLoaderVersions[ModLoaders.Forge], "45.0.66") && McUtils.CompareVersionGE("47.4.8", Instance.Version.ModLoaderVersions[ModLoaders.Forge]))
                {
                    vb$AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(22, 0)));
                }
            }
            if (Instance.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge) && (Instance.Version.Vanilla == new Version(20, 0, 1) || (McUtils.CompareVersionGE("20.2.62-beta", Instance.Version.ModLoaderVersions[ModLoaders.NeoForge]) && !Instance.Version.ModLoaderVersions[ModLoaders.NeoForge].Contains("25w14craftmine"))))
            {
                vb$AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(22, 0)));
            }
            if (Instance.Version.ModLoaders.HasFlagF(ModLoaders.Fabric) && Instance.Version.Valid)
            {
                if (Instance.Version.Vanilla.Major >= 15 && Instance.Version.Vanilla.Major <= 16)
                {
                    vb$AnonymousDelegate_(ValueRange<Version>.AtLeast(new Version(8, 0)));
                }
                else if (Instance.Version.Vanilla.Major >= 18)
                {
                    vb$AnonymousDelegate_(ValueRange<Version>.AtLeast(new Version(17, 0)));
                }
                if (Operators.CompareString(Instance.Version.ModLoaderVersions[ModLoaders.Fabric], "未知版本", false) != 0 && McUtils.CompareVersion(Instance.Version.ModLoaderVersions[ModLoaders.Fabric], "0.17.0") < 0)
                {
                    vb$AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(25, 0)));
                }
            }
            if (Instance.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader) && Instance.Version.Valid)
            {
                vb$AnonymousDelegate_(ValueRange<Version>.LessThan(new Version(9, 0)));
            }
            if (Settings.Get<ModLaunch.McLoginType>("LoginType", null) == ModLaunch.McLoginType.Nide)
            {
                vb$AnonymousDelegate_(ValueRange<Version>.AtLeast(new Version(8, 0, 141)));
            }
            ValueTuple<ValueRange<Version>, string> valueTuple = new ValueTuple<ValueRange<Version>, string>(JavaRange, text);
            return valueTuple;
        }

        public static Java SelectOrDownloadJava(OneOf<ValueRange<Version>, ModMinecraft.McInstance> Target, bool TryFixOrHint, CancellationToken c = default(CancellationToken), ProgressProvider p = null)
        {
            ModJava._Closure$__11-0 CS$<>8__locals1 = new ModJava._Closure$__11-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_c = c;
            checked
            {
                Java java2;
                try
                {
                    if (Target.Is<ModMinecraft.McInstance>())
                    {
                        ModJava.MigrateInstanceJavaSettings(Target.As<ModMinecraft.McInstance>(), CS$<>8__locals1.$VB$Local_c, (p != null) ? p.SplitTo(0.2) : null);
                    }
                    ModMinecraft.McInstance mcInstance = (Target.Is<ModMinecraft.McInstance>() ? Target.As<ModMinecraft.McInstance>() : null);
                    int num = ((mcInstance == null) ? 0 : Settings.Get<int>("VersionArgumentJavaV2", mcInstance));
                    if (num != 2)
                    {
                        if (num != 3)
                        {
                            ModJava._Closure$__11-1 CS$<>8__locals2 = new ModJava._Closure$__11-1(CS$<>8__locals2);
                            CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1;
                            CS$<>8__locals2.$VB$Local_Range = (Target.Is<ModMinecraft.McInstance>() ? ModJava.GetJavaRequirement(mcInstance).Item1 : Target.As<ValueRange<Version>>());
                            CS$<>8__locals2.$VB$Local_RecommendedComponent = (Target.Is<ModMinecraft.McInstance>() ? ModJava.GetJavaRequirement(mcInstance).Item2 : null);
                            Logger.Info(string.Format("自动选择以下范围的 Java：{0} {1}", CS$<>8__locals2.$VB$Local_Range, CS$<>8__locals2.$VB$Local_RecommendedComponent), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                            if (CS$<>8__locals2.$VB$Local_Range.IsEmpty())
                            {
                                throw new Exception("Java 版本需求存在冲突，导致没有任何可能适配的 Java，请查看 PCL 的日志了解详细信息！");
                            }
                            VB$AnonymousDelegate_0<Java> vb$AnonymousDelegate_ = delegate
                            {
                                ModJava.JavaListRefreshWorker.WaitIfRunning(-1, default(CancellationToken));
                                try
                                {
                                    foreach (Java java5 in Configs.JavaList.Get())
                                    {
                                        if (!java5.CheckAsync(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_c).Run<bool>())
                                        {
                                            Logger.Warn(string.Format("Java 检查失败，将尝试列表中的下一个 Java：{0}", java5), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                                        }
                                        else if (java5.Version != null && CS$<>8__locals2.$VB$Local_Range.Contains(java5.Version))
                                        {
                                            Logger.Info(string.Format("已选定 Java：{0}", java5), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                                            return java5;
                                        }
                                    }
                                }
                                finally
                                {
                                    IEnumerator<Java> enumerator;
                                    if (enumerator != null)
                                    {
                                        enumerator.Dispose();
                                    }
                                }
                                return null;
                            };
                            if (p != null)
                            {
                                p.Set(0.25, false, ProgressProvider.ChildrenAction.Finished);
                            }
                            Java java = vb$AnonymousDelegate_();
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
                                    ModJava.JavaListRefreshWorker.Start(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_c, (p != null) ? p.SplitTo(0.3) : null);
                                    java = vb$AnonymousDelegate_();
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
                                        if (ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init == null)
                                        {
                                            Interlocked.CompareExchange<StaticLocalInitFlag>(ref ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init, new StaticLocalInitFlag(), null);
                                        }
                                        bool flag2 = false;
                                        try
                                        {
                                            Monitor.Enter(ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init, ref flag2);
                                            if (ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init.State == 0)
                                            {
                                                ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init.State = 2;
                                                ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker = new RedoableWorker<List<ModJava.JavaDownloadInfo>>((ModJava._Closure$__.$I11-1 == null) ? (ModJava._Closure$__.$I11-1 = delegate(CancellationToken? cancellationToken)
                                                {
                                                    string text2 = ModNet.NetRequestByLoader(ModDownload.DlVersionListOrder(new string[] { "https://piston-meta.mojang.com/v1/products/java-runtime/2ec0cc96c44e5a76b9c8b7c39df7210883d12871/all.json" }, new string[] { "https://bmclapi2.bangbang93.com/v1/products/java-runtime/2ec0cc96c44e5a76b9c8b7c39df7210883d12871/all.json" }), 45000, true, false);
                                                    if (cancellationToken != null)
                                                    {
                                                        cancellationToken.GetValueOrDefault().ThrowIfCancellationRequested();
                                                    }
                                                    List<ModJava.JavaDownloadInfo> list = new List<ModJava.JavaDownloadInfo>();
                                                    try
                                                    {
                                                        foreach (JToken jtoken in text2.DeserializeJson<JObject>()["windows-x64"])
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
                                                                        string text3 = jtoken2["version"]["name"].ToString();
                                                                        if (text3.Contains("."))
                                                                        {
                                                                            string[] array = text3.Split(".", false);
                                                                            list.Add(new ModJava.JavaDownloadInfo
                                                                            {
                                                                                ComponentName = jproperty.Name,
                                                                                Java = (JObject)jtoken2,
                                                                                JavaVersion = new Version((int)Math.Round(ModBase.Val(array[0])), (int)Math.Round(ModBase.Val(array[1])), (int)Math.Round((array.Length >= 3) ? ModBase.Val(array[2]) : 0.0), (int)Math.Round((array.Length >= 4) ? ModBase.Val(array[3]) : 0.0))
                                                                            });
                                                                        }
                                                                        else if (text3.StartsWithF("8u51-", false))
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
                                                                                JavaVersion = new Version((int)Math.Round(ModBase.Val(text3.BeforeFirstOfAny(new string[] { "u", "." }, false))), 0, 0, 0)
                                                                            });
                                                                        }
                                                                    }
                                                                }
                                                                catch (Exception ex2)
                                                                {
                                                                    Logger.Warn(ex2, string.Format("解析 Mojang 提供的 Java 列表项失败，已跳过（{0}）", jproperty.Name), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                                                                }
                                                            }
                                                        }
                                                    }
                                                    finally
                                                    {
                                                        IEnumerator<JToken> enumerator2;
                                                        if (enumerator2 != null)
                                                        {
                                                            enumerator2.Dispose();
                                                        }
                                                    }
                                                    return list;
                                                }) : ModJava._Closure$__.$I11-1, "SelectOrDownloadJava");
                                            }
                                            else if (ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init.State == 2)
                                            {
                                                throw new IncompleteInitialization();
                                            }
                                        }
                                        finally
                                        {
                                            ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init.State = 1;
                                            if (flag2)
                                            {
                                                Monitor.Exit(ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init);
                                            }
                                        }
                                        if (!ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker.HasSucceeded)
                                        {
                                            ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker.Start(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_c, (p != null) ? p.SplitTo(0.4) : null);
                                        }
                                        ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker.WaitIfRunning(-1, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_c);
                                        if (p != null)
                                        {
                                            p.Set(0.4, true, ProgressProvider.ChildrenAction.Finished);
                                        }
                                        ModJava.JavaDownloadInfo javaDownloadInfo = ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker.LastResult.FirstOrDefault<ModJava.JavaDownloadInfo>(new Func<ModJava.JavaDownloadInfo, bool>[]
                                        {
                                            (ModJava.JavaDownloadInfo Comp) => Operators.CompareString(Comp.Java["version"]["name"].ToString(), CS$<>8__locals2.$VB$Local_RecommendedComponent, false) == 0,
                                            (ModJava.JavaDownloadInfo Comp) => CS$<>8__locals2.$VB$Local_Range.Contains(Comp.JavaVersion)
                                        });
                                        if (javaDownloadInfo == null)
                                        {
                                            Logger.Warn("Mojang 提供的 Java 列表中没有合适的 Java，无法自动下载", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                                            bool flag3 = CS$<>8__locals2.$VB$Local_Range.HasLower && CS$<>8__locals2.$VB$Local_Range.IsLowerInclusive && CS$<>8__locals2.$VB$Local_Range.Lower.Minor <= 0 && CS$<>8__locals2.$VB$Local_Range.Lower.Build <= 0 && CS$<>8__locals2.$VB$Local_Range.Lower.Revision <= 0;
                                            bool flag4 = CS$<>8__locals2.$VB$Local_Range.HasUpper && !CS$<>8__locals2.$VB$Local_Range.IsUpperInclusive && CS$<>8__locals2.$VB$Local_Range.Upper.Minor <= 0 && CS$<>8__locals2.$VB$Local_Range.Upper.Build <= 0 && CS$<>8__locals2.$VB$Local_Range.Upper.Revision <= 0;
                                            string text;
                                            if (CS$<>8__locals2.$VB$Local_Range.HasLower && CS$<>8__locals2.$VB$Local_Range.HasUpper)
                                            {
                                                if (flag3 && flag4)
                                                {
                                                    if (CS$<>8__locals2.$VB$Local_Range.Lower.Major == CS$<>8__locals2.$VB$Local_Range.Upper.Major - 1)
                                                    {
                                                        text = string.Format(" Java {0}", CS$<>8__locals2.$VB$Local_Range.Lower.Major);
                                                    }
                                                    else
                                                    {
                                                        text = string.Format(" Java {0} ~ {1}", CS$<>8__locals2.$VB$Local_Range.Lower.Major, CS$<>8__locals2.$VB$Local_Range.Upper.Major - 1);
                                                    }
                                                }
                                                else
                                                {
                                                    text = string.Format(" Java {0}", CS$<>8__locals2.$VB$Local_Range);
                                                }
                                            }
                                            else if (CS$<>8__locals2.$VB$Local_Range.HasLower)
                                            {
                                                if (flag3)
                                                {
                                                    text = string.Format(" Java {0} 或更高版本的 Java", CS$<>8__locals2.$VB$Local_Range.Lower.Major);
                                                }
                                                else if (CS$<>8__locals2.$VB$Local_Range.IsLowerInclusive)
                                                {
                                                    text = string.Format(" Java {0} 或更高版本的 Java", CS$<>8__locals2.$VB$Local_Range.Lower);
                                                }
                                                else
                                                {
                                                    text = string.Format("版本高于 {0} 的 Java", CS$<>8__locals2.$VB$Local_Range.Lower);
                                                }
                                            }
                                            else if (flag4)
                                            {
                                                text = string.Format(" Java {0} 或更低版本的 Java", CS$<>8__locals2.$VB$Local_Range.Upper.Major - 1);
                                            }
                                            else if (CS$<>8__locals2.$VB$Local_Range.IsUpperInclusive)
                                            {
                                                text = string.Format(" Java {0} 或更低版本的 Java", CS$<>8__locals2.$VB$Local_Range.Upper);
                                            }
                                            else
                                            {
                                                text = string.Format("版本低于 {0} 的 Java", CS$<>8__locals2.$VB$Local_Range.Upper);
                                            }
                                            ModMain.MyMsgBox(string.Format("PCL 没能找到合适的 Java。{0}请在网上搜索并手动安装{1}， 然后在【设置 → 启动 → 高级选项 → Java 列表】中导入它。", "\r\n", text), "需要手动安装 Java", "确定", "", "", false, true, false, null, null, null);
                                            java2 = null;
                                        }
                                        else
                                        {
                                            ModLoader.LoaderCombo<int> loaderCombo = ModJava.CreateJavaDownloadLoader(javaDownloadInfo);
                                            try
                                            {
                                                loaderCombo.WaitForExit(null, null, true, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_c, (p != null) ? p.SplitTo(0.95) : null);
                                            }
                                            finally
                                            {
                                                loaderCombo.Cancel();
                                            }
                                            java = vb$AnonymousDelegate_();
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
                                    ModMain.MyMsgBox(string.Format("该版本的版本设置选择了 {0}使用指定的 Java{1}，但还没有指定任何 Java。{2}请先选择你想使用的 Java，然后再试。", ModBase.vbLQ, ModBase.vbRQ, "\r\n"), "未选择 Java", "确定", "", "", true, true, true, null, null, null);
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
                            else if (!java3.CheckAsync(CS$<>8__locals1.$VB$Local_c).Run<bool>())
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
                        Java java4 = JavaUtils.SearchFoldersAsync(true, new string[] { mcInstance.PathVersion }, CS$<>8__locals1.$VB$Local_c, (p != null) ? p.SplitTo(1.0) : null).Run<List<Java>>().FirstOrDefault<Java>();
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
                                ModMain.MyMsgBox(string.Format("该版本的版本设置选择了 {0}使用版本文件夹中的 Java{1}，但版本文件夹里没能找到 Java。{2}请先修改启动选项中的 Java 设置，或在版本文件夹中放一个 Java，然后再试。", ModBase.vbLQ, ModBase.vbRQ, "\r\n"), "未找到 Java", "确定", "", "", true, true, true, null, null, null);
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
            ModBase.RunInUi(delegate
            {
                PageInstanceLeft.Instance = Instance;
                ModMain.FrmMain.PageChange(FormMain.PageType.InstanceSetup, FormMain.PageSubType.SetupLink);
            }, false);
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
                    ModBase.RunInUiWait((ModJava._Closure$__.$I14-0 == null) ? (ModJava._Closure$__.$I14-0 = delegate
                    {
                        ModMain.FrmInstanceSetup.ComboArgumentJava.IsEnabled = false;
                    }) : ModJava._Closure$__.$I14-0);
                }
                try
                {
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
                        Java ForcedJava = new Java(text.DeserializeJson<JObject>()["Path"].ToString(), null);
                        if (!ForcedJava.CheckAsync(c).Run<bool>())
                        {
                            ModMain.Hint("该版本的设置中强制指定的 Java 存在异常，该设置已重置", ModMain.HintType.Red, true);
                            if (p != null)
                            {
                                p.Set(0.4, false, ProgressProvider.ChildrenAction.Finished);
                            }
                        }
                        else
                        {
                            Logger.Info(string.Format("将版本独立设置改为老版本中强制指定的 Java：{0}", ForcedJava), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                            if (!Configs.JavaList.Get().Contains(ForcedJava))
                            {
                                Configs.JavaList.Edit(delegate(ref ConcurrentList<Java> List)
                                {
                                    List.Add(ForcedJava);
                                });
                                Logger.Info(string.Format("已将强制指定的 Java 加入 Java 列表底部：{0}", ForcedJava), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
                            }
                            Configs.JavaForced.Set(ForcedJava, Instance.Config);
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
                        ModBase.RunInUiWait((ModJava._Closure$__.$I14-2 == null) ? (ModJava._Closure$__.$I14-2 = delegate
                        {
                            SettingService.RefreshSettings(ModMain.FrmInstanceSetup.ComboArgumentJava);
                            SettingService.RefreshSettings(ModMain.FrmInstanceSetup.ComboArgumentJavaSelect);
                            ModMain.FrmInstanceSetup.ComboArgumentJava.IsEnabled = true;
                        }) : ModJava._Closure$__.$I14-2);
                    }
                }
            }
        }

        private static ModLoader.LoaderCombo<int> CreateJavaDownloadLoader(ModJava.JavaDownloadInfo Target)
        {
            ModJava._Closure$__16-0 CS$<>8__locals1 = new ModJava._Closure$__16-0(CS$<>8__locals1);
            string text = (string)Target.Java["manifest"]["url"];
            Logger.Info(string.Format("准备下载 Java {0}（{1}）：{2}", Target.Java["version"]["name"], Target.ComponentName, text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
            string text2 = ModNet.NetRequestByLoader(ModDownload.DlSourceOrder(new string[] { text }, new string[] { text.Replace("piston-meta.mojang.com", "bmclapi2.bangbang93.com") }), 45000, true, false);
            CS$<>8__locals1.$VB$Local_BaseDir = string.Format("{0}.minecraft\\runtime\\{1}\\", Paths.AppData, Target.ComponentName);
            List<ModNet.NetFile> list = new List<ModNet.NetFile>();
            try
            {
                foreach (JToken jtoken in ((JObject)text2.DeserializeJson())["files"])
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
                            list.Add(new ModNet.NetFile(ModDownload.DlSourceOrder(new string[] { text3 }, new string[] { text3.Replace("piston-data.mojang.com", "bmclapi2.bangbang93.com") }), CS$<>8__locals1.$VB$Local_BaseDir + jproperty.Name, fileChecker, false));
                        }
                    }
                }
            }
            finally
            {
                IEnumerator<JToken> enumerator;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            Logger.Info(string.Format("该 Java 需要下载 {0} 个文件，目标文件夹：{1}", list.Count, CS$<>8__locals1.$VB$Local_BaseDir), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModJava.vb");
            return new ModLoader.LoaderCombo<int>("下载 Java", new ModLoader.LoaderBase[]
            {
                new ModNet.LoaderDownload("下载 Java 文件", list)
                {
                    ProgressWeight = 10.0
                },
                new ModLoader.LoaderWorker("刷新 Java 列表", ModJava.JavaListRefreshWorker)
            })
            {
                OnStateChanged = delegate(ModLoader.LoaderBase a0)
                {
                    base._Lambda$__0((ModLoader.LoaderCombo<int>)a0);
                }
            };
        }

        private static readonly object JavaLock = RuntimeHelpers.GetObjectValue(new object());

        private static RedoableWorker<List<ModJava.JavaDownloadInfo>> $STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker;

        private static StaticLocalInitFlag $STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init;

        private class JavaDownloadInfo
        {
            public string ComponentName { get; set; }

            public JObject Java { get; set; }

            public Version JavaVersion { get; set; }
        }
    }
}
