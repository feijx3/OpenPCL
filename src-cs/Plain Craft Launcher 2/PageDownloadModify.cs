using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;
using PCLCS;

namespace PCL
{
    public class PageDownloadModify : PageDownloadInstall
    {
        public PageDownloadModify()
        {
            base.Unloaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.PageUnloaded();
            };
            this.PendingComponents = new Dictionary<MyLoading, string>();
        }

        public override void LoaderInit()
        {
            base.ControlInit();
            this.InitLoader = new ModLoader.LoaderTask<ModMinecraft.McInstance, object>("初始化版本修改", new Action<ModLoader.LoaderTask<ModMinecraft.McInstance, object>>(this.LoadInstance), null, ThreadPriority.Normal);
            base.PageLoaderInit<ModMinecraft.McInstance, object>(this.LoadMinecraft, this.PanLoad, this.PanAllBack, null, this.InitLoader, new Action<ModLoader.LoaderTask<ModMinecraft.McInstance, object>>(this.LoadInstanceFinished), (PageDownloadModify._Closure$__.$I9-0 == null) ? (PageDownloadModify._Closure$__.$I9-0 = () => PageInstanceLeft.Instance) : PageDownloadModify._Closure$__.$I9-0, true);
        }

        public override void Init()
        {
            this.PanBack.ScrollToHome();
            if (this.$STATIC$Init$2001$IsLoad$Init == null)
            {
                Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$Init$2001$IsLoad$Init, new StaticLocalInitFlag(), null);
            }
            bool flag = false;
            try
            {
                Monitor.Enter(this.$STATIC$Init$2001$IsLoad$Init, ref flag);
                if (this.$STATIC$Init$2001$IsLoad$Init.State == 0)
                {
                    this.$STATIC$Init$2001$IsLoad$Init.State = 2;
                    this.$STATIC$Init$2001$IsLoad = false;
                }
                else if (this.$STATIC$Init$2001$IsLoad$Init.State == 2)
                {
                    throw new IncompleteInitialization();
                }
            }
            finally
            {
                this.$STATIC$Init$2001$IsLoad$Init.State = 1;
                if (flag)
                {
                    Monitor.Exit(this.$STATIC$Init$2001$IsLoad$Init);
                }
            }
            if (this.$STATIC$Init$2001$IsLoad)
            {
                base.PageLoaderRestart(null, true);
            }
            this.$STATIC$Init$2001$IsLoad = true;
        }

        private void PageUnloaded()
        {
            this.InitLoader.Cancel();
        }

        private void LoadInstance(ModLoader.LoaderTask<ModMinecraft.McInstance, object> Task)
        {
            PageDownloadModify._Closure$__12-0 CS$<>8__locals1 = new PageDownloadModify._Closure$__12-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Me = this;
            CS$<>8__locals1.$VB$Local_Task = Task;
            ModBase.RunInUiWait(delegate
            {
                ModAnimation.AniStop("FrmDownloadInstall SelectPageSwitch");
                this.IsSelectNameEdited = false;
                base.ClearSelected();
                this.PendingComponents.Clear();
                this.InitialTarget = null;
                this.IsInSelectPage = false;
                this.PanAllBack.IsHitTestVisible = false;
            });
            CS$<>8__locals1.$VB$Local_Task.Input.Config.DiscardCache();
            McInstallManifest mcInstallManifest = Configs.McInstallManifest.Get(CS$<>8__locals1.$VB$Local_Task.Input.Config);
            if (mcInstallManifest != null && mcInstallManifest.SchemaVersion > 1)
            {
                Logger.Warn(string.Format("安装清单由更新版本的 PCL 创建（清单版本：{0}，当前版本：{1}）", mcInstallManifest.SchemaVersion, 1), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadModify.vb");
                throw new Exception("更新 PCL 后才能修改它！点击【设置 → 其他 → 检查更新】以更新。");
            }
            if (mcInstallManifest != null)
            {
                McInstallManifest mcInstallManifest2 = mcInstallManifest.RefreshModComponents(CS$<>8__locals1.$VB$Local_Task.Input.PathIndie);
                if (mcInstallManifest2 != mcInstallManifest)
                {
                    mcInstallManifest = mcInstallManifest2;
                    Configs.McInstallManifest.Set(mcInstallManifest, CS$<>8__locals1.$VB$Local_Task.Input.Config);
                    CS$<>8__locals1.$VB$Local_Task.Input.Config.Save();
                }
            }
            CS$<>8__locals1.$VB$Local_OptiFineVersion = null;
            if (mcInstallManifest != null)
            {
                CS$<>8__locals1.$VB$Local_Components = mcInstallManifest.Components;
                if (CS$<>8__locals1.$VB$Local_Components == null)
                {
                    throw new Exception("安装清单缺少组件信息，无法还原已安装组件。");
                }
                this.IsVanillaVersionReliable = true;
            }
            else
            {
                McVersion version = new ModMinecraft.McInstance(CS$<>8__locals1.$VB$Local_Task.Input.PathVersion).Version;
                if (Operators.CompareString(version.VanillaName, "Unknown", false) == 0)
                {
                    throw new Exception("无法确定 Minecraft 版本，无法修改");
                }
                List<string> list = version.ModLoaderVersions.Where<KeyValuePair<ModLoaders, string>>((PageDownloadModify._Closure$__.$I12-1 == null) ? (PageDownloadModify._Closure$__.$I12-1 = (KeyValuePair<ModLoaders, string> Loader) => string.IsNullOrWhiteSpace(Loader.Value) || Operators.CompareString(Loader.Value, "未知版本", false) == 0) : PageDownloadModify._Closure$__.$I12-1).Select<KeyValuePair<ModLoaders, string>, string>((PageDownloadModify._Closure$__.$I12-2 == null) ? (PageDownloadModify._Closure$__.$I12-2 = (KeyValuePair<ModLoaders, string> Loader) => Loader.Key.ToString()) : PageDownloadModify._Closure$__.$I12-2).ToList<string>();
                if (list.Any<string>())
                {
                    throw new Exception(string.Format("无法确定 {0} 的版本，无法修改", list.Join("、")));
                }
                CS$<>8__locals1.$VB$Local_Components = new McComponents(version.VanillaName, version.ModLoaderVersions, null, null, null);
                CS$<>8__locals1.$VB$Local_OptiFineVersion = version.OptiFine;
                this.IsVanillaVersionReliable = version.Reliable;
            }
            CS$<>8__locals1.$VB$Local_MinecraftVersion = CS$<>8__locals1.$VB$Local_Components.MinecraftVersion;
            if (string.IsNullOrWhiteSpace(CS$<>8__locals1.$VB$Local_MinecraftVersion))
            {
                throw new Exception("无法确定当前 Minecraft 版本，请检查版本文件");
            }
            Dictionary<string, ModLoader.LoaderBase> dictionary = new Dictionary<string, ModLoader.LoaderBase>
            {
                {
                    "Minecraft",
                    ModDownload.DlClientListLoader
                },
                {
                    "OptiFine",
                    ModDownload.DlOptiFineListLoader
                },
                {
                    "LiteLoader",
                    ModDownload.DlLiteLoaderListLoader
                },
                {
                    "NeoForge",
                    ModDownload.DlNeoForgeListLoader
                },
                {
                    "Fabric",
                    ModDownload.DlFabricListLoader
                },
                {
                    "Fabric API",
                    ModDownload.DlFabricApiLoader
                },
                {
                    "OptiFabric",
                    ModDownload.DlOptiFabricLoader
                }
            };
            ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>> loaderTask = null;
            if (McVersion.IsStandardFormat(CS$<>8__locals1.$VB$Local_MinecraftVersion))
            {
                loaderTask = new ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>("DlForgeVersion " + CS$<>8__locals1.$VB$Local_MinecraftVersion, new Action<ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>>(ModDownload.DlForgeVersionMain), null, ThreadPriority.Normal);
                dictionary.Add("Forge", loaderTask);
            }
            try
            {
                foreach (ModLoader.LoaderBase loaderBase in dictionary.Values)
                {
                    loaderBase.Start((loaderBase == loaderTask) ? CS$<>8__locals1.$VB$Local_MinecraftVersion : null, CS$<>8__locals1.$VB$Local_Task.IsForceRestarting);
                }
            }
            finally
            {
                Dictionary<string, ModLoader.LoaderBase>.ValueCollection.Enumerator enumerator;
                ((IDisposable)enumerator).Dispose();
            }
            while (!CS$<>8__locals1.$VB$Local_Task.IsCanceled)
            {
                try
                {
                    foreach (KeyValuePair<string, ModLoader.LoaderBase> keyValuePair in dictionary)
                    {
                        ModLoader.LoaderBase value = keyValuePair.Value;
                        if (value.State == ModBase.LoadState.Failed)
                        {
                            if (value != loaderTask || Operators.CompareString(value.Error.Message, "无", false) != 0)
                            {
                                throw new Exception(string.Format("获取 {0} 版本列表失败，请稍后再试，或使用 VPN 改善网络环境", keyValuePair.Key), value.Error);
                            }
                        }
                        else if (value.State == ModBase.LoadState.Canceled)
                        {
                            throw new Exception(string.Format("获取 {0} 版本列表被中断，请稍后再试，或使用 VPN 改善网络环境", keyValuePair.Key));
                        }
                    }
                }
                finally
                {
                    Dictionary<string, ModLoader.LoaderBase>.Enumerator enumerator2;
                    ((IDisposable)enumerator2).Dispose();
                }
                if (!dictionary.Values.All<ModLoader.LoaderBase>((PageDownloadModify._Closure$__.$I12-3 == null) ? (PageDownloadModify._Closure$__.$I12-3 = (ModLoader.LoaderBase Loader) => Loader.State == ModBase.LoadState.Finished || Loader.State == ModBase.LoadState.Failed) : PageDownloadModify._Closure$__.$I12-3))
                {
                    CS$<>8__locals1.$VB$Local_Task.Progress = 0.7 * dictionary.Values.Average<ModLoader.LoaderBase>((PageDownloadModify._Closure$__.$I12-4 == null) ? (PageDownloadModify._Closure$__.$I12-4 = (ModLoader.LoaderBase Loader) => Loader.Progress) : PageDownloadModify._Closure$__.$I12-4);
                    Thread.Sleep(10);
                }
                else
                {
                    if (CS$<>8__locals1.$VB$Local_Task.IsCanceled)
                    {
                        break;
                    }
                    CS$<>8__locals1.$VB$Local_Task.Progress = 0.7;
                    this.MinecraftVersions = (JArray)ModDownload.DlClientListLoader.Output.Value["versions"].DeepClone();
                    this.VanillaData = this.MinecraftVersions.OfType<JObject>().FirstOrDefault<JObject>((JObject v) => Operators.CompareString(v["id"].ToString().Replace("∞", "infinite"), CS$<>8__locals1.$VB$Local_MinecraftVersion.Replace("∞", "infinite"), false) == 0);
                    if (this.VanillaData == null)
                    {
                        throw new Exception(string.Format("Minecraft 列表中没有已安装的 {0}", CS$<>8__locals1.$VB$Local_MinecraftVersion));
                    }
                    this.VanillaData["id"] = CS$<>8__locals1.$VB$Local_MinecraftVersion;
                    if (mcInstallManifest != null)
                    {
                        JObject optiFineEntry = CS$<>8__locals1.$VB$Local_Components.OptiFineEntry;
                        this.SelectedOptiFine = ((optiFineEntry != null) ? optiFineEntry.ToObject<ModDownload.DlOptiFineListEntry>() : null);
                        if (this.SelectedOptiFine != null && !ModDownload.DlOptiFineListLoader.Output.Value.Any<ModDownload.DlOptiFineListEntry>((ModDownload.DlOptiFineListEntry v) => Operators.CompareString(v.FileName, this.SelectedOptiFine.FileName, false) == 0))
                        {
                            if (ModDownload.DlOptiFineListBmclapiLoader.State != ModBase.LoadState.Finished)
                            {
                                ModDownload.DlOptiFineListBmclapiLoader.WaitForExit(null, null, false, default(CancellationToken), null);
                            }
                            ModDownload.DlOptiFineListLoader.Output = ModDownload.DlOptiFineListBmclapiLoader.Output;
                        }
                    }
                    else if (CS$<>8__locals1.$VB$Local_OptiFineVersion != null && Operators.CompareString(CS$<>8__locals1.$VB$Local_OptiFineVersion, "未知版本", false) != 0)
                    {
                        this.SelectedOptiFine = ModDownload.DlOptiFineListLoader.Output.Value.FirstOrDefault<ModDownload.DlOptiFineListEntry>((ModDownload.DlOptiFineListEntry v) => Operators.CompareString(v.Inherit, CS$<>8__locals1.$VB$Local_MinecraftVersion, false) == 0 && Operators.CompareString(v.InstanceName.AfterLast("-OptiFine_", false).Replace("HD_U_", ""), CS$<>8__locals1.$VB$Local_OptiFineVersion.Replace("HD_U_", ""), false) == 0);
                        if (this.SelectedOptiFine == null)
                        {
                            throw new Exception(string.Format("OptiFine 列表中没有已安装的 {0}", CS$<>8__locals1.$VB$Local_OptiFineVersion));
                        }
                    }
                    if (CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions.ContainsKey(ModLoaders.Forge))
                    {
                        ModDownload.DlForgeVersionEntry dlForgeVersionEntry;
                        if (loaderTask == null)
                        {
                            dlForgeVersionEntry = null;
                        }
                        else
                        {
                            List<ModDownload.DlForgeVersionEntry> output = loaderTask.Output;
                            dlForgeVersionEntry = ((output != null) ? output.FirstOrDefault<ModDownload.DlForgeVersionEntry>((ModDownload.DlForgeVersionEntry v) => Operators.CompareString(v.Inherit, CS$<>8__locals1.$VB$Local_MinecraftVersion, false) == 0 && Operators.CompareString(v.VersionName, CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions[ModLoaders.Forge], false) == 0 && Operators.CompareString(v.Category, "universal", false) != 0 && Operators.CompareString(v.Category, "client", false) != 0 && (CS$<>8__locals1.$VB$Me.SelectedOptiFine == null || PageDownloadInstall.IsOptiFineSuitForForge(CS$<>8__locals1.$VB$Me.SelectedOptiFine, v))) : null);
                        }
                        this.SelectedForge = dlForgeVersionEntry;
                        if (this.SelectedForge == null)
                        {
                            throw new Exception(string.Format("Forge 列表中没有已安装的 {0}", CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions[ModLoaders.Forge]));
                        }
                    }
                    if (CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions.ContainsKey(ModLoaders.NeoForge))
                    {
                        this.SelectedNeoForge = ModDownload.DlNeoForgeListLoader.Output.Value.FirstOrDefault<ModDownload.DlNeoForgeListEntry>((ModDownload.DlNeoForgeListEntry v) => Operators.CompareString(v.Inherit, CS$<>8__locals1.$VB$Local_MinecraftVersion, false) == 0 && Operators.CompareString(v.VersionName, CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions[ModLoaders.NeoForge], false) == 0);
                        if (this.SelectedNeoForge == null)
                        {
                            throw new Exception(string.Format("NeoForge 列表中没有已安装的 {0}", CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions[ModLoaders.NeoForge]));
                        }
                    }
                    if (CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions.ContainsKey(ModLoaders.Fabric))
                    {
                        JObject jobject = ModDownload.DlFabricListLoader.Output.Value["loader"].OfType<JObject>().FirstOrDefault<JObject>(new Func<JObject, bool>[]
                        {
                            (JObject v) => Operators.CompareString(v["version"].ToString(), CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions[ModLoaders.Fabric], false) == 0,
                            (JObject v) => Operators.CompareString(v["version"].ToString().Replace("+build", ""), CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions[ModLoaders.Fabric], false) == 0
                        });
                        if (jobject == null || !ModDownload.DlFabricListLoader.Output.Value["game"].OfType<JObject>().Any<JObject>((JObject v) => Operators.CompareString(v["version"].ToString(), CS$<>8__locals1.$VB$Local_MinecraftVersion.Replace("∞", "infinite").Replace("Combat Test 7c", "1.16_combat-3"), false) == 0))
                        {
                            throw new Exception(string.Format("Fabric 列表中没有已安装的 {0}", CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions[ModLoaders.Fabric]));
                        }
                        this.SelectedFabric = jobject["version"].ToString();
                    }
                    if (CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions.ContainsKey(ModLoaders.LiteLoader))
                    {
                        this.SelectedLiteLoader = ModDownload.DlLiteLoaderListLoader.Output.Value.FirstOrDefault<ModDownload.DlLiteLoaderListEntry>((ModDownload.DlLiteLoaderListEntry v) => Operators.CompareString(v.Inherit, CS$<>8__locals1.$VB$Local_MinecraftVersion, false) == 0 && Operators.CompareString(v.JsonToken["version"].ToString(), CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions[ModLoaders.LiteLoader], false) == 0);
                        if (this.SelectedLiteLoader == null)
                        {
                            throw new Exception(string.Format("LiteLoader 列表中没有已安装的 {0}", CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions[ModLoaders.LiteLoader]));
                        }
                    }
                    if (CS$<>8__locals1.$VB$Local_Components.FabricApiVersion != null)
                    {
                        this.SelectedFabricApi = ModDownload.DlFabricApiLoader.Output.FirstOrDefault<ResourceVersion>((ResourceVersion v) => (Operators.CompareString(v.Version, CS$<>8__locals1.$VB$Local_Components.FabricApiVersion, false) == 0 || Operators.CompareString(v.Id, CS$<>8__locals1.$VB$Local_Components.FabricApiVersion, false) == 0 || Operators.CompareString(v.Display, CS$<>8__locals1.$VB$Local_Components.FabricApiVersion, false) == 0) && PageDownloadInstall.IsFabricApiCompatible(v, CS$<>8__locals1.$VB$Local_MinecraftVersion));
                        if (this.SelectedFabricApi == null)
                        {
                            throw new Exception(string.Format("Fabric API 列表中没有已安装的 {0}", CS$<>8__locals1.$VB$Local_Components.FabricApiVersion));
                        }
                    }
                    if (CS$<>8__locals1.$VB$Local_Components.OptiFabricVersion != null)
                    {
                        this.SelectedOptiFabric = ModDownload.DlOptiFabricLoader.Output.FirstOrDefault<ResourceVersion>((ResourceVersion v) => (Operators.CompareString(v.Version, CS$<>8__locals1.$VB$Local_Components.OptiFabricVersion, false) == 0 || Operators.CompareString(v.Id, CS$<>8__locals1.$VB$Local_Components.OptiFabricVersion, false) == 0 || Operators.CompareString(v.Display, CS$<>8__locals1.$VB$Local_Components.OptiFabricVersion, false) == 0) && PageDownloadInstall.IsOptiFabricCompatible(v, CS$<>8__locals1.$VB$Local_MinecraftVersion));
                        if (this.SelectedOptiFabric == null)
                        {
                            throw new Exception(string.Format("OptiFabric 列表中没有已安装的 {0}", CS$<>8__locals1.$VB$Local_Components.OptiFabricVersion));
                        }
                    }
                    List<McInstallManifest.ManagedFile> list2 = new List<McInstallManifest.ManagedFile>();
                    if (mcInstallManifest != null)
                    {
                        list2.AddRange(mcInstallManifest.ManagedFiles);
                    }
                    Lazy<List<ValueTuple<FileInfo, string>>> lazy = new Lazy<List<ValueTuple<FileInfo, string>>>(delegate
                    {
                        string text4 = CS$<>8__locals1.$VB$Local_Task.Input.PathIndie + "mods\\";
                        List<string> list9 = DirectoryUtils.EnumerateFiles(text4, false, "*.jar").ToList<string>();
                        if (CS$<>8__locals1.$VB$Me.SelectedLiteLoader != null)
                        {
                            list9.AddRange(DirectoryUtils.EnumerateFiles(Path.Combine(text4, CS$<>8__locals1.$VB$Local_MinecraftVersion), false, "*.jar"));
                        }
                        List<ValueTuple<FileInfo, string>> list10 = new List<ValueTuple<FileInfo, string>>();
                        try
                        {
                            foreach (string text5 in list9)
                            {
                                if (CS$<>8__locals1.$VB$Local_Task.IsCanceled)
                                {
                                    return list10;
                                }
                                FileInfo info = FileUtils.GetInfo(text5);
                                list10.Add(new ValueTuple<FileInfo, string>(info, new LocalResourceFile(info).ModrinthHash));
                                CS$<>8__locals1.$VB$Local_Task.Progress = 0.7 + 0.28 * (double)list10.Count / (double)list9.Count;
                            }
                        }
                        finally
                        {
                            List<string>.Enumerator enumerator6;
                            ((IDisposable)enumerator6).Dispose();
                        }
                        return list10;
                    });
                    if (this.SelectedFabric != null && this.SelectedFabricApi == null)
                    {
                        List<ResourceVersion> list3 = ModDownload.DlFabricApiLoader.Output.Where<ResourceVersion>((ResourceVersion v) => PageDownloadInstall.IsFabricApiCompatible(v, CS$<>8__locals1.$VB$Local_MinecraftVersion)).ToList<ResourceVersion>();
                        List<ValueTuple<FileInfo, string, ResourceVersion>> list4 = new List<ValueTuple<FileInfo, string, ResourceVersion>>();
                        try
                        {
                            List<ValueTuple<FileInfo, string>>.Enumerator enumerator3 = lazy.Value.GetEnumerator();
                            while (enumerator3.MoveNext())
                            {
                                PageDownloadModify._Closure$__12-1 CS$<>8__locals2 = new PageDownloadModify._Closure$__12-1(CS$<>8__locals2);
                                CS$<>8__locals2.$VB$Local_Entry = enumerator3.Current;
                                ResourceVersion resourceVersion = list3.FirstOrDefault<ResourceVersion>((ResourceVersion v) => Operators.CompareString(v.Hash, CS$<>8__locals2.$VB$Local_Entry.Item2, false) == 0);
                                if (resourceVersion != null)
                                {
                                    list4.Add(new ValueTuple<FileInfo, string, ResourceVersion>(CS$<>8__locals2.$VB$Local_Entry.Item1, CS$<>8__locals2.$VB$Local_Entry.Item2, resourceVersion));
                                }
                            }
                        }
                        finally
                        {
                            List<ValueTuple<FileInfo, string>>.Enumerator enumerator3;
                            ((IDisposable)enumerator3).Dispose();
                        }
                        ValueTuple<FileInfo, string, ResourceVersion>? valueTuple = this.SelectMod<ResourceVersion>("Fabric API", list4);
                        if (valueTuple != null)
                        {
                            this.SelectedFabricApi = valueTuple.Value.Item3;
                            list2.Add(new McInstallManifest.ManagedFile("mods/" + valueTuple.Value.Item1.Name.Lower(), valueTuple.Value.Item2, valueTuple.Value.Item1.LastWriteTimeUtc, "FabricApi"));
                        }
                    }
                    if (CS$<>8__locals1.$VB$Local_Task.IsCanceled)
                    {
                        break;
                    }
                    if (this.SelectedFabric != null && this.SelectedOptiFabric == null)
                    {
                        List<ResourceVersion> list5 = ModDownload.DlOptiFabricLoader.Output.Where<ResourceVersion>((ResourceVersion v) => PageDownloadInstall.IsOptiFabricCompatible(v, CS$<>8__locals1.$VB$Local_MinecraftVersion)).ToList<ResourceVersion>();
                        bool flag = list5.Any<ResourceVersion>((PageDownloadModify._Closure$__.$I12-20 == null) ? (PageDownloadModify._Closure$__.$I12-20 = (ResourceVersion v) => (v.Hash ?? "").Length == 32) : PageDownloadModify._Closure$__.$I12-20);
                        List<ValueTuple<FileInfo, string, ResourceVersion>> list6 = new List<ValueTuple<FileInfo, string, ResourceVersion>>();
                        try
                        {
                            List<ValueTuple<FileInfo, string>>.Enumerator enumerator4 = lazy.Value.GetEnumerator();
                            while (enumerator4.MoveNext())
                            {
                                PageDownloadModify._Closure$__12-2 CS$<>8__locals3 = new PageDownloadModify._Closure$__12-2(CS$<>8__locals3);
                                CS$<>8__locals3.$VB$Local_Entry = enumerator4.Current;
                                ResourceVersion resourceVersion2 = list5.FirstOrDefault<ResourceVersion>((ResourceVersion v) => Operators.CompareString(v.Hash, CS$<>8__locals3.$VB$Local_Entry.Item2, false) == 0);
                                if (resourceVersion2 == null && flag)
                                {
                                    PageDownloadModify._Closure$__12-3 CS$<>8__locals4 = new PageDownloadModify._Closure$__12-3(CS$<>8__locals4);
                                    CS$<>8__locals4.$VB$Local_Md5 = CryptographyUtils.ComputeFileHash(CS$<>8__locals3.$VB$Local_Entry.Item1.FullName, CryptographyUtils.HashMethod.Md5);
                                    resourceVersion2 = list5.FirstOrDefault<ResourceVersion>((ResourceVersion v) => Operators.CompareString(v.Hash, CS$<>8__locals4.$VB$Local_Md5, false) == 0);
                                }
                                if (resourceVersion2 != null)
                                {
                                    list6.Add(new ValueTuple<FileInfo, string, ResourceVersion>(CS$<>8__locals3.$VB$Local_Entry.Item1, CS$<>8__locals3.$VB$Local_Entry.Item2, resourceVersion2));
                                }
                            }
                        }
                        finally
                        {
                            List<ValueTuple<FileInfo, string>>.Enumerator enumerator4;
                            ((IDisposable)enumerator4).Dispose();
                        }
                        ValueTuple<FileInfo, string, ResourceVersion>? valueTuple2 = this.SelectMod<ResourceVersion>("OptiFabric", list6);
                        if (valueTuple2 != null)
                        {
                            this.SelectedOptiFabric = valueTuple2.Value.Item3;
                            list2.Add(new McInstallManifest.ManagedFile("mods/" + valueTuple2.Value.Item1.Name.Lower(), valueTuple2.Value.Item2, valueTuple2.Value.Item1.LastWriteTimeUtc, "OptiFabric"));
                        }
                    }
                    if (CS$<>8__locals1.$VB$Local_Task.IsCanceled)
                    {
                        break;
                    }
                    List<ValueTuple<FileInfo, string, string>> list7 = new List<ValueTuple<FileInfo, string, string>>();
                    if (this.SelectedOptiFine == null && (this.SelectedForge != null || this.SelectedLiteLoader != null || this.SelectedFabric != null))
                    {
                        if (this.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init == null)
                        {
                            Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init, new StaticLocalInitFlag(), null);
                        }
                        bool flag2 = false;
                        try
                        {
                            Monitor.Enter(this.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init, ref flag2);
                            if (this.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init.State == 0)
                            {
                                this.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init.State = 2;
                                this.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes = (JObject)FileUtils.ReadAsJson("Minecraft\\OptiFineHashes.json", null, typeof(Resource));
                            }
                            else if (this.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init.State == 2)
                            {
                                throw new IncompleteInitialization();
                            }
                        }
                        finally
                        {
                            this.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init.State = 1;
                            if (flag2)
                            {
                                Monitor.Exit(this.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init);
                            }
                        }
                        list7 = lazy.Value.Select<ValueTuple<FileInfo, string>, ValueTuple<FileInfo, string, string>>(delegate([TupleElementNames(new string[] { "File", "Sha1" })] ValueTuple<FileInfo, string> Entry)
                        {
                            FileInfo item = Entry.Item1;
                            string item2 = Entry.Item2;
                            JToken jtoken = this.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes[Entry.Item2];
                            return new ValueTuple<FileInfo, string, string>(item, item2, (jtoken != null) ? jtoken.ToString() : null);
                        }).Where<ValueTuple<FileInfo, string, string>>((PageDownloadModify._Closure$__.$I12-24 == null) ? (PageDownloadModify._Closure$__.$I12-24 = ([TupleElementNames(new string[] { "File", "Sha1", "OriginalName" })] ValueTuple<FileInfo, string, string> Entry) => Entry.Item3 != null) : PageDownloadModify._Closure$__.$I12-24).ToList<ValueTuple<FileInfo, string, string>>();
                        List<ValueTuple<FileInfo, string, ModDownload.DlOptiFineListEntry>> list8 = new List<ValueTuple<FileInfo, string, ModDownload.DlOptiFineListEntry>>();
                        try
                        {
                            List<ValueTuple<FileInfo, string, string>>.Enumerator enumerator5 = list7.GetEnumerator();
                            while (enumerator5.MoveNext())
                            {
                                PageDownloadModify._Closure$__12-4 CS$<>8__locals5 = new PageDownloadModify._Closure$__12-4(CS$<>8__locals5);
                                CS$<>8__locals5.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1;
                                CS$<>8__locals5.$VB$Local_Entry = enumerator5.Current;
                                ModDownload.DlOptiFineListEntry dlOptiFineListEntry = ModDownload.DlOptiFineListLoader.Output.Value.FirstOrDefault<ModDownload.DlOptiFineListEntry>((ModDownload.DlOptiFineListEntry v) => Operators.CompareString(v.Inherit, CS$<>8__locals5.$VB$NonLocal_$VB$Closure_2.$VB$Local_MinecraftVersion, false) == 0 && Operators.CompareString(v.FileName, CS$<>8__locals5.$VB$Local_Entry.Item3, false) == 0 && (CS$<>8__locals5.$VB$NonLocal_$VB$Closure_2.$VB$Me.SelectedForge == null || PageDownloadInstall.IsOptiFineSuitForForge(v, CS$<>8__locals5.$VB$NonLocal_$VB$Closure_2.$VB$Me.SelectedForge)));
                                if (dlOptiFineListEntry == null)
                                {
                                    ModMain.Hint(string.Format("无法从 OptiFine 列表还原 {0} 对应的 {1}，将跳过该文件。", CS$<>8__locals5.$VB$Local_Entry.Item1.Name, CS$<>8__locals5.$VB$Local_Entry.Item3), ModMain.HintType.Blue, true);
                                }
                                else
                                {
                                    list8.Add(new ValueTuple<FileInfo, string, ModDownload.DlOptiFineListEntry>(CS$<>8__locals5.$VB$Local_Entry.Item1, CS$<>8__locals5.$VB$Local_Entry.Item2, dlOptiFineListEntry));
                                }
                            }
                        }
                        finally
                        {
                            List<ValueTuple<FileInfo, string, string>>.Enumerator enumerator5;
                            ((IDisposable)enumerator5).Dispose();
                        }
                        ValueTuple<FileInfo, string, ModDownload.DlOptiFineListEntry>? valueTuple3 = this.SelectMod<ModDownload.DlOptiFineListEntry>("OptiFine", list8);
                        if (valueTuple3 != null)
                        {
                            this.SelectedOptiFine = valueTuple3.Value.Item3;
                            string text = PathUtils.RemoveExtendedPrefix(valueTuple3.Value.Item1.FullName).Substring(CS$<>8__locals1.$VB$Local_Task.Input.PathIndie.Length).Replace("\\", "/")
                                .Lower();
                            list2.Add(new McInstallManifest.ManagedFile(text, valueTuple3.Value.Item2, valueTuple3.Value.Item1.LastWriteTimeUtc, "OptiFine"));
                        }
                    }
                    if (CS$<>8__locals1.$VB$Local_Task.IsCanceled)
                    {
                        break;
                    }
                    if (this.IsVanillaVersionReliable)
                    {
                        PageDownloadModify._Closure$__12-0 CS$<>8__locals6 = CS$<>8__locals1;
                        string $VB$Local_MinecraftVersion = CS$<>8__locals1.$VB$Local_MinecraftVersion;
                        Dictionary<ModLoaders, string> modLoaderVersions = CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions;
                        JObject jobject2 = ((this.SelectedOptiFine == null) ? null : JObject.FromObject(this.SelectedOptiFine));
                        ResourceVersion selectedFabricApi = this.SelectedFabricApi;
                        string text2;
                        if ((text2 = ((selectedFabricApi != null) ? selectedFabricApi.Version : null)) == null)
                        {
                            ResourceVersion selectedFabricApi2 = this.SelectedFabricApi;
                            if ((text2 = ((selectedFabricApi2 != null) ? selectedFabricApi2.Id : null)) == null)
                            {
                                ResourceVersion selectedFabricApi3 = this.SelectedFabricApi;
                                text2 = ((selectedFabricApi3 != null) ? selectedFabricApi3.Display : null);
                            }
                        }
                        ResourceVersion selectedOptiFabric = this.SelectedOptiFabric;
                        string text3;
                        if ((text3 = ((selectedOptiFabric != null) ? selectedOptiFabric.Version : null)) == null)
                        {
                            ResourceVersion selectedOptiFabric2 = this.SelectedOptiFabric;
                            if ((text3 = ((selectedOptiFabric2 != null) ? selectedOptiFabric2.Id : null)) == null)
                            {
                                ResourceVersion selectedOptiFabric3 = this.SelectedOptiFabric;
                                text3 = ((selectedOptiFabric3 != null) ? selectedOptiFabric3.Display : null);
                            }
                        }
                        CS$<>8__locals6.$VB$Local_Components = new McComponents($VB$Local_MinecraftVersion, modLoaderVersions, jobject2, text2, text3);
                        if (mcInstallManifest == null || !CS$<>8__locals1.$VB$Local_Components.IsEquivalentTo(mcInstallManifest.Components) || !list2.SequenceEqual<McInstallManifest.ManagedFile>(mcInstallManifest.ManagedFiles))
                        {
                            Configs.McInstallManifest.Set(new McInstallManifest(1, CS$<>8__locals1.$VB$Local_Components, DateTime.UtcNow, list2), CS$<>8__locals1.$VB$Local_Task.Input.Config);
                            CS$<>8__locals1.$VB$Local_Task.Input.Config.Save();
                            Logger.Info(string.Format("已为 {0} 更新安装清单", CS$<>8__locals1.$VB$Local_Task.Input.Name), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadModify.vb");
                        }
                    }
                    if (!CS$<>8__locals1.$VB$Local_Task.IsCanceled)
                    {
                        this.ForgeVersionLoader = loaderTask;
                        break;
                    }
                    break;
                }
            }
        }

        [return: TupleElementNames(new string[] { "File", "Sha1", "Version" })]
        private ValueTuple<FileInfo, string, T>? SelectMod<T>(string ComponentName, [TupleElementNames(new string[] { "File", "Sha1", "Version" })] List<ValueTuple<FileInfo, string, T>> Matches)
        {
            ValueTuple<FileInfo, string, T>? valueTuple;
            if (Matches.Count == 0)
            {
                valueTuple = null;
            }
            else if (Matches.Count == 1)
            {
                valueTuple = new ValueTuple<FileInfo, string, T>?(Matches[0]);
            }
            else
            {
                int? Choice = null;
                ModBase.RunInUiWait(delegate
                {
                    Choice = ModMain.MyMsgBoxSelect(Matches.Select<ValueTuple<FileInfo, string, $CLS0>, IMyRadio>((PageDownloadModify._Closure$__13<$CLS0>.$I13-1 == null) ? (PageDownloadModify._Closure$__13<$CLS0>.$I13-1 = ([TupleElementNames(new string[] { "File", "Sha1", "Version" })] ValueTuple<FileInfo, string, $CLS0> Entry) => new MyRadioBox
                    {
                        Text = Entry.Item1.Name
                    }) : PageDownloadModify._Closure$__13<$CLS0>.$I13-1), string.Format("选择当前使用的 {0}", ComponentName), "确定", "", false);
                });
                if (Choice == null)
                {
                    throw new Exception(string.Format("发现了多个 {0}", ComponentName));
                }
                valueTuple = new ValueTuple<FileInfo, string, T>?(Matches[Choice.Value]);
            }
            return valueTuple;
        }

        private void LoadInstanceFinished(ModLoader.LoaderTask<ModMinecraft.McInstance, object> Task)
        {
            this.OriginalName = Task.Input.Name;
            this.CustomLogo = null;
            if (Conversions.ToBoolean(ModBase.ReadIni(Task.Input.PathVersion + "PCL\\Setup.ini", "LogoCustom", Conversions.ToString(false))))
            {
                this.CustomLogo = (Task.Input.Logo.EndsWithF("PCL\\Logo.png", false) ? (Task.Input.PathVersion + "PCL\\Logo.png") : Task.Input.Logo);
            }
            MyListItem myListItem = ModDownloadLib.McDownloadListItem(this.VanillaData, null, false).Init();
            this.VanillaName = myListItem.Title;
            this.VanillaIcon = myListItem.Logo;
            this.LabMinecraftVersion.Text = this.VanillaName;
            this.TextSelectName.Text = this.OriginalName;
            this.IsSelectNameEdited = !string.Equals(this.OriginalName, base.GetSelectName(), StringComparison.Ordinal);
            this.TextSelectName.ValidateRules = new Collection<Validate>
            {
                new ValidateFolderName(ModMinecraft.McFolderSelected + "versions", true, new List<string> { this.OriginalName })
            };
            this.CardMinecraft.Visibility = Visibility.Collapsed;
            this.BtnBack.Visibility = Visibility.Collapsed;
            this.ImgLogo.Margin = new Thickness(15.0, 15.0, 0.0, 0.0);
            this.TextSelectName.Margin = new Thickness(56.0, 16.0, 16.0, 0.0);
            this.LabSelectName.Text = string.Format("原名\u200a:\u200a\u200a\u200a{0}", this.OriginalName);
            this.UpdateOriginalNameVisibility();
            this.BtnStart.Text = "开始修改";
            this.BtnStart.Logo = ModMain.FrmInstanceLeft.ItemModify.Logo;
            this.AutoSelectedFabricApi = this.SelectedFabric != null;
            this.AutoSelectedOptiFabric = this.SelectedFabric != null && this.SelectedOptiFine != null;
            if (this.ForgeVersionLoader != null)
            {
                this.LoadForge.State = this.ForgeVersionLoader;
            }
            base.PrepareSelectPage();
            base.ReloadComponentLists();
            this.PanMinecraft.Visibility = Visibility.Collapsed;
            this.PanMinecraft.Opacity = 0.0;
            this.PanMinecraft.RenderTransform = new TranslateTransform(-50.0, 0.0);
            this.PanSelect.Opacity = 1.0;
            this.PanSelect.RenderTransform = new TranslateTransform();
            this.PanBack.IsHitTestVisible = true;
            this.PanAllBack.IsHitTestVisible = true;
            this.InitialTarget = base.GetSelectedComponents();
            this.BtnMinecraftRestore.Tag = this.VanillaData;
            this.BtnOptiFineRestore.Tag = this.SelectedOptiFine;
            this.BtnForgeRestore.Tag = this.SelectedForge;
            this.BtnNeoForgeRestore.Tag = this.SelectedNeoForge;
            this.BtnFabricRestore.Tag = this.SelectedFabric;
            this.BtnFabricApiRestore.Tag = this.SelectedFabricApi;
            this.BtnOptiFabricRestore.Tag = this.SelectedOptiFabric;
            this.BtnLiteLoaderRestore.Tag = this.SelectedLiteLoader;
            this.ImgMinecraftOld.Source = this.VanillaIcon;
            this.LabMinecraftOld.Text = this.LabMinecraftVersion.Text;
            this.LabOptiFineOld.Text = ((this.SelectedOptiFine == null) ? "无" : this.LabOptiFine.Text);
            this.LabForgeOld.Text = ((this.SelectedForge == null) ? "无" : this.LabForge.Text);
            this.LabNeoForgeOld.Text = ((this.SelectedNeoForge == null) ? "无" : this.LabNeoForge.Text);
            this.LabFabricOld.Text = ((this.SelectedFabric == null) ? "无" : this.LabFabric.Text);
            this.LabLiteLoaderOld.Text = ((this.SelectedLiteLoader == null) ? "无" : this.LabLiteLoader.Text);
            this.LabFabricApiOld.Text = ((this.SelectedFabricApi == null) ? "无" : this.LabFabricApi.Text);
            this.LabOptiFabricOld.Text = ((this.SelectedOptiFabric == null) ? "无" : this.LabOptiFabric.Text);
            this.RefreshComparisons();
        }

        public override string GetSelectLogo()
        {
            return this.CustomLogo ?? base.GetSelectLogo();
        }

        public override void ShowMinecraftVersionList()
        {
            if (this.IsInSelectPage)
            {
                McComponents selectedComponents = base.GetSelectedComponents();
                ValueTuple<MyLoading, string>[] array = new ValueTuple<MyLoading, string>[7];
                array[0] = new ValueTuple<MyLoading, string>(this.LoadOptiFine, selectedComponents.OptiFineVersion);
                array[1] = new ValueTuple<MyLoading, string>(this.LoadFabricApi, selectedComponents.FabricApiVersion);
                array[2] = new ValueTuple<MyLoading, string>(this.LoadOptiFabric, selectedComponents.OptiFabricVersion);
                int num = 3;
                MyLoading loadForge = this.LoadForge;
                ModDownload.DlForgeVersionEntry selectedForge = this.SelectedForge;
                array[num] = new ValueTuple<MyLoading, string>(loadForge, (selectedForge != null) ? selectedForge.VersionName : null);
                int num2 = 4;
                MyLoading loadNeoForge = this.LoadNeoForge;
                ModDownload.DlNeoForgeListEntry selectedNeoForge = this.SelectedNeoForge;
                array[num2] = new ValueTuple<MyLoading, string>(loadNeoForge, (selectedNeoForge != null) ? selectedNeoForge.VersionName : null);
                array[5] = new ValueTuple<MyLoading, string>(this.LoadFabric, this.SelectedFabric);
                int num3 = 6;
                MyLoading loadLiteLoader = this.LoadLiteLoader;
                ModDownload.DlLiteLoaderListEntry selectedLiteLoader = this.SelectedLiteLoader;
                string text;
                if (selectedLiteLoader == null)
                {
                    text = null;
                }
                else
                {
                    JToken jsonToken = selectedLiteLoader.JsonToken;
                    text = ((jsonToken != null) ? jsonToken["version"].ToString() : null);
                }
                array[num3] = new ValueTuple<MyLoading, string>(loadLiteLoader, text);
                foreach (ValueTuple<MyLoading, string> valueTuple in array)
                {
                    if (valueTuple.Item2 != null)
                    {
                        this.PendingComponents[valueTuple.Item1] = valueTuple.Item2;
                    }
                }
            }
            base.ShowMinecraftVersionList();
        }

        public override void ReloadSelected()
        {
            if (!this.IsRestoringComponents)
            {
                if (this.IsInSelectPage && this.VanillaName != null && this.PendingComponents.Any<KeyValuePair<MyLoading, string>>())
                {
                    this.IsRestoringComponents = true;
                    try
                    {
                        bool flag = false;
                        foreach (MyLoading myLoading in new MyLoading[] { this.LoadFabric, this.LoadForge, this.LoadNeoForge, this.LoadLiteLoader, this.LoadOptiFine, this.LoadFabricApi, this.LoadOptiFabric })
                        {
                            PageDownloadModify._Closure$__17-0 CS$<>8__locals1 = new PageDownloadModify._Closure$__17-0(CS$<>8__locals1);
                            CS$<>8__locals1.$VB$Local_Version = null;
                            if (this.PendingComponents.TryGetValue(myLoading, out CS$<>8__locals1.$VB$Local_Version) && myLoading.State.IsLoader)
                            {
                                ModLoader.LoaderBase loaderBase = (ModLoader.LoaderBase)myLoading.State;
                                if (myLoading == this.LoadOptiFine)
                                {
                                    if (new MyLoading[] { this.LoadFabric, this.LoadForge, this.LoadNeoForge, this.LoadLiteLoader }.Any<MyLoading>((MyLoading Component) => this.PendingComponents.ContainsKey(Component)))
                                    {
                                        goto IL_05FC;
                                    }
                                }
                                else if (myLoading == this.LoadFabricApi)
                                {
                                    if (this.PendingComponents.ContainsKey(this.LoadFabric))
                                    {
                                        goto IL_05FC;
                                    }
                                }
                                else if (myLoading == this.LoadForge)
                                {
                                    if (!McVersion.IsStandardFormat(this.VanillaName))
                                    {
                                        this.PendingComponents.Remove(myLoading);
                                        goto IL_05FC;
                                    }
                                    if (Operators.CompareString(((ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>)loaderBase).Input, this.VanillaName, false) != 0)
                                    {
                                        goto IL_05FC;
                                    }
                                    if (loaderBase.State == ModBase.LoadState.Failed && Operators.CompareString(loaderBase.Error.Message, "无", false) == 0)
                                    {
                                        this.PendingComponents.Remove(myLoading);
                                        goto IL_05FC;
                                    }
                                }
                                else if (myLoading == this.LoadOptiFabric && (this.PendingComponents.ContainsKey(this.LoadFabric) || this.PendingComponents.ContainsKey(this.LoadOptiFine)))
                                {
                                    goto IL_05FC;
                                }
                                if (loaderBase.State == ModBase.LoadState.Finished)
                                {
                                    this.PendingComponents.Remove(myLoading);
                                    flag = true;
                                    if (myLoading == this.LoadFabric)
                                    {
                                        if (this.SelectedFabric == null && base.LoadFabricGetError() == null)
                                        {
                                            IEnumerable<JObject> enumerable = ModDownload.DlFabricListLoader.Output.Value["loader"].OfType<JObject>();
                                            JObject jobject = enumerable.FirstOrDefault<JObject>((JObject v) => Operators.CompareString(v["version"].ToString(), CS$<>8__locals1.$VB$Local_Version, false) == 0) ?? enumerable.FirstOrDefault<JObject>();
                                            this.SelectedFabric = ((jobject != null) ? jobject["version"].ToString() : null);
                                        }
                                    }
                                    else if (myLoading == this.LoadOptiFine)
                                    {
                                        if (this.SelectedOptiFine == null && base.LoadOptiFineGetError() == null)
                                        {
                                            IEnumerable<ModDownload.DlOptiFineListEntry> enumerable2 = ModDownload.DlOptiFineListLoader.Output.Value.Where<ModDownload.DlOptiFineListEntry>((ModDownload.DlOptiFineListEntry v) => Operators.CompareString(v.Inherit, this.VanillaName, false) == 0 && (this.SelectedForge == null || PageDownloadInstall.IsOptiFineSuitForForge(v, this.SelectedForge)));
                                            ModDownload.DlOptiFineListEntry dlOptiFineListEntry;
                                            if ((dlOptiFineListEntry = enumerable2.FirstOrDefault<ModDownload.DlOptiFineListEntry>((ModDownload.DlOptiFineListEntry v) => Operators.CompareString(v.InstanceName.AfterLast("-OptiFine_", false), CS$<>8__locals1.$VB$Local_Version, false) == 0)) == null)
                                            {
                                                dlOptiFineListEntry = enumerable2.OrderBy<ModDownload.DlOptiFineListEntry, bool>((PageDownloadModify._Closure$__.$I17-4 == null) ? (PageDownloadModify._Closure$__.$I17-4 = (ModDownload.DlOptiFineListEntry v) => v.IsPreview) : PageDownloadModify._Closure$__.$I17-4).ThenByDescending<ModDownload.DlOptiFineListEntry, string>((PageDownloadModify._Closure$__.$I17-5 == null) ? (PageDownloadModify._Closure$__.$I17-5 = (ModDownload.DlOptiFineListEntry v) => v.DisplayName) : PageDownloadModify._Closure$__.$I17-5, Comparer<string>.Create(new Comparison<string>(McUtils.CompareVersion))).FirstOrDefault<ModDownload.DlOptiFineListEntry>();
                                            }
                                            this.SelectedOptiFine = dlOptiFineListEntry;
                                        }
                                    }
                                    else if (myLoading == this.LoadForge)
                                    {
                                        if (this.SelectedForge == null && base.LoadForgeGetError() == null)
                                        {
                                            IEnumerable<ModDownload.DlForgeVersionEntry> enumerable3 = ((ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>)loaderBase).Output.Where<ModDownload.DlForgeVersionEntry>((ModDownload.DlForgeVersionEntry v) => Operators.CompareString(v.Inherit, this.VanillaName, false) == 0 && Operators.CompareString(v.Category, "universal", false) != 0 && Operators.CompareString(v.Category, "client", false) != 0 && (this.SelectedOptiFine == null || PageDownloadInstall.IsOptiFineSuitForForge(this.SelectedOptiFine, v)));
                                            ModDownload.DlForgeVersionEntry dlForgeVersionEntry;
                                            if ((dlForgeVersionEntry = enumerable3.FirstOrDefault<ModDownload.DlForgeVersionEntry>((ModDownload.DlForgeVersionEntry v) => Operators.CompareString(v.VersionName, CS$<>8__locals1.$VB$Local_Version, false) == 0)) == null)
                                            {
                                                dlForgeVersionEntry = enumerable3.OrderByDescending<ModDownload.DlForgeVersionEntry, ModDownload.DlForgeVersionEntry>((PageDownloadModify._Closure$__.$I17-8 == null) ? (PageDownloadModify._Closure$__.$I17-8 = (ModDownload.DlForgeVersionEntry v) => v) : PageDownloadModify._Closure$__.$I17-8).FirstOrDefault<ModDownload.DlForgeVersionEntry>();
                                            }
                                            this.SelectedForge = dlForgeVersionEntry;
                                        }
                                    }
                                    else if (myLoading == this.LoadNeoForge)
                                    {
                                        if (this.SelectedNeoForge == null && base.LoadNeoForgeGetError() == null)
                                        {
                                            IEnumerable<ModDownload.DlNeoForgeListEntry> enumerable4 = ModDownload.DlNeoForgeListLoader.Output.Value.Where<ModDownload.DlNeoForgeListEntry>((ModDownload.DlNeoForgeListEntry v) => Operators.CompareString(v.Inherit, this.VanillaName, false) == 0);
                                            this.SelectedNeoForge = enumerable4.FirstOrDefault<ModDownload.DlNeoForgeListEntry>((ModDownload.DlNeoForgeListEntry v) => Operators.CompareString(v.VersionName, CS$<>8__locals1.$VB$Local_Version, false) == 0) ?? enumerable4.FirstOrDefault<ModDownload.DlNeoForgeListEntry>();
                                        }
                                    }
                                    else if (myLoading == this.LoadLiteLoader)
                                    {
                                        if (this.SelectedLiteLoader == null && base.LoadLiteLoaderGetError() == null)
                                        {
                                            IEnumerable<ModDownload.DlLiteLoaderListEntry> enumerable5 = ModDownload.DlLiteLoaderListLoader.Output.Value.Where<ModDownload.DlLiteLoaderListEntry>((ModDownload.DlLiteLoaderListEntry v) => Operators.CompareString(v.Inherit, this.VanillaName, false) == 0);
                                            this.SelectedLiteLoader = enumerable5.FirstOrDefault<ModDownload.DlLiteLoaderListEntry>((ModDownload.DlLiteLoaderListEntry v) => Operators.CompareString(v.JsonToken["version"].ToString(), CS$<>8__locals1.$VB$Local_Version, false) == 0) ?? enumerable5.FirstOrDefault<ModDownload.DlLiteLoaderListEntry>();
                                        }
                                    }
                                    else if (myLoading == this.LoadFabricApi)
                                    {
                                        if (this.SelectedFabricApi == null && base.LoadFabricApiGetError() == null)
                                        {
                                            IEnumerable<ResourceVersion> enumerable6 = ModDownload.DlFabricApiLoader.Output.Where<ResourceVersion>((ResourceVersion v) => PageDownloadInstall.IsFabricApiCompatible(v, this.VanillaName));
                                            ResourceVersion resourceVersion;
                                            if ((resourceVersion = enumerable6.FirstOrDefault<ResourceVersion>((ResourceVersion v) => Operators.CompareString(v.Version, CS$<>8__locals1.$VB$Local_Version, false) == 0 || Operators.CompareString(v.Id, CS$<>8__locals1.$VB$Local_Version, false) == 0 || Operators.CompareString(v.Display, CS$<>8__locals1.$VB$Local_Version, false) == 0)) == null)
                                            {
                                                resourceVersion = enumerable6.OrderByDescending<ResourceVersion, DateTime>((PageDownloadModify._Closure$__.$I17-15 == null) ? (PageDownloadModify._Closure$__.$I17-15 = (ResourceVersion v) => v.ReleaseDate) : PageDownloadModify._Closure$__.$I17-15).FirstOrDefault<ResourceVersion>();
                                            }
                                            this.SelectedFabricApi = resourceVersion;
                                        }
                                        if (this.SelectedFabricApi != null)
                                        {
                                            this.AutoSelectedFabricApi = true;
                                        }
                                    }
                                    else if (myLoading == this.LoadOptiFabric)
                                    {
                                        if (this.SelectedOptiFabric == null && base.LoadOptiFabricGetError() == null)
                                        {
                                            IEnumerable<ResourceVersion> enumerable7 = ModDownload.DlOptiFabricLoader.Output.Where<ResourceVersion>((ResourceVersion v) => PageDownloadInstall.IsOptiFabricCompatible(v, this.VanillaName));
                                            ResourceVersion resourceVersion2;
                                            if ((resourceVersion2 = enumerable7.FirstOrDefault<ResourceVersion>((ResourceVersion v) => Operators.CompareString(v.Version, CS$<>8__locals1.$VB$Local_Version, false) == 0 || Operators.CompareString(v.Id, CS$<>8__locals1.$VB$Local_Version, false) == 0 || Operators.CompareString(v.Display, CS$<>8__locals1.$VB$Local_Version, false) == 0)) == null)
                                            {
                                                resourceVersion2 = enumerable7.OrderByDescending<ResourceVersion, DateTime>((PageDownloadModify._Closure$__.$I17-18 == null) ? (PageDownloadModify._Closure$__.$I17-18 = (ResourceVersion v) => v.ReleaseDate) : PageDownloadModify._Closure$__.$I17-18).FirstOrDefault<ResourceVersion>();
                                            }
                                            this.SelectedOptiFabric = resourceVersion2;
                                        }
                                        if (this.SelectedOptiFabric != null)
                                        {
                                            this.AutoSelectedOptiFabric = true;
                                        }
                                    }
                                }
                            }
                            IL_05FC:;
                        }
                        if (flag)
                        {
                            base.ReloadComponentLists();
                        }
                    }
                    finally
                    {
                        this.IsRestoringComponents = false;
                    }
                }
                base.ReloadSelected();
                if (this.InitialTarget != null)
                {
                    this.RefreshComparisons();
                }
            }
        }

        private void RefreshComparisons()
        {
            McComponents selectedComponents = base.GetSelectedComponents();
            this.ImgMinecraftNew.Source = this.VanillaIcon;
            bool flag = Operators.CompareString(this.InitialTarget.MinecraftVersion, selectedComponents.MinecraftVersion, false) == 0;
            this.ShowComparison(this.PanMinecraftCompare, this.PanMinecraftInfo, this.InitialTarget.MinecraftVersion, selectedComponents.MinecraftVersion, this.LabMinecraftVersion.Text, this.BtnMinecraftRestore, true, null);
            ModDownload.DlOptiFineListEntry dlOptiFineListEntry = this.BtnOptiFineRestore.Tag as ModDownload.DlOptiFineListEntry;
            this.ShowComparison(this.PanOptiFineCompare, this.PanOptiFineInfo, this.InitialTarget.OptiFineVersion, selectedComponents.OptiFineVersion, this.LabOptiFine.Text, this.BtnOptiFineRestore, this.InitialTarget.OptiFineEntry == null || (flag && this.CardOptiFine.MainSwap.Visibility == Visibility.Visible && dlOptiFineListEntry != null && (this.SelectedForge == null || PageDownloadInstall.IsOptiFineSuitForForge(dlOptiFineListEntry, this.SelectedForge))), this.BtnOptiFineClear);
            this.ShowComparison(this.PanFabricApiCompare, this.PanFabricApiInfo, this.InitialTarget.FabricApiVersion, selectedComponents.FabricApiVersion, this.LabFabricApi.Text, this.BtnFabricApiRestore, this.InitialTarget.FabricApiVersion == null || (flag && this.SelectedFabric != null && this.CardFabricApi.MainSwap.Visibility == Visibility.Visible), this.BtnFabricApiClear);
            this.ShowComparison(this.PanOptiFabricCompare, this.PanOptiFabricInfo, this.InitialTarget.OptiFabricVersion, selectedComponents.OptiFabricVersion, this.LabOptiFabric.Text, this.BtnOptiFabricRestore, this.InitialTarget.OptiFabricVersion == null || (flag && this.SelectedFabric != null && this.SelectedOptiFine != null && this.CardOptiFabric.MainSwap.Visibility == Visibility.Visible), this.BtnOptiFabricClear);
            foreach (ValueTuple<ModLoaders, MyCard, Grid, Grid, TextBlock, MyIconButton, MyIconButton> valueTuple in new ValueTuple<ModLoaders, MyCard, Grid, Grid, TextBlock, MyIconButton, MyIconButton>[]
            {
                new ValueTuple<ModLoaders, MyCard, Grid, Grid, TextBlock, MyIconButton, MyIconButton>(ModLoaders.Forge, this.CardForge, this.PanForgeCompare, this.PanForgeInfo, this.LabForge, this.BtnForgeRestore, this.BtnForgeClear),
                new ValueTuple<ModLoaders, MyCard, Grid, Grid, TextBlock, MyIconButton, MyIconButton>(ModLoaders.NeoForge, this.CardNeoForge, this.PanNeoForgeCompare, this.PanNeoForgeInfo, this.LabNeoForge, this.BtnNeoForgeRestore, this.BtnNeoForgeClear),
                new ValueTuple<ModLoaders, MyCard, Grid, Grid, TextBlock, MyIconButton, MyIconButton>(ModLoaders.Fabric, this.CardFabric, this.PanFabricCompare, this.PanFabricInfo, this.LabFabric, this.BtnFabricRestore, this.BtnFabricClear),
                new ValueTuple<ModLoaders, MyCard, Grid, Grid, TextBlock, MyIconButton, MyIconButton>(ModLoaders.LiteLoader, this.CardLiteLoader, this.PanLiteLoaderCompare, this.PanLiteLoaderInfo, this.LabLiteLoader, this.BtnLiteLoaderRestore, this.BtnLiteLoaderClear)
            })
            {
                PageDownloadModify._Closure$__18-0 CS$<>8__locals1 = new PageDownloadModify._Closure$__18-0(CS$<>8__locals1);
                CS$<>8__locals1.$VB$Local_OldVersion = null;
                string text = null;
                this.InitialTarget.ModLoaderVersions.TryGetValue(valueTuple.Item1, out CS$<>8__locals1.$VB$Local_OldVersion);
                selectedComponents.ModLoaderVersions.TryGetValue(valueTuple.Item1, out text);
                bool flag2 = CS$<>8__locals1.$VB$Local_OldVersion == null || (flag && valueTuple.Item2.MainSwap.Visibility == Visibility.Visible && valueTuple.Item6.Tag != null);
                if (valueTuple.Item1 == ModLoaders.Forge && CS$<>8__locals1.$VB$Local_OldVersion != null && flag2)
                {
                    flag2 = this.SelectedOptiFine == null || PageDownloadInstall.IsOptiFineSuitForForge(this.SelectedOptiFine, (ModDownload.DlForgeVersionEntry)valueTuple.Item6.Tag);
                }
                if (valueTuple.Item1 == ModLoaders.Fabric && CS$<>8__locals1.$VB$Local_OldVersion != null)
                {
                    PageDownloadModify._Closure$__18-1 CS$<>8__locals2 = new PageDownloadModify._Closure$__18-1(CS$<>8__locals2);
                    CS$<>8__locals2.$VB$Local_TargetVersion = this.VanillaName.Replace("∞", "infinite").Replace("Combat Test 7c", "1.16_combat-3");
                    flag2 = this.CardFabric.Visibility == Visibility.Visible && this.CardFabric.MainSwap.Visibility == Visibility.Visible && ModDownload.DlFabricListLoader.Output.Value["loader"].OfType<JObject>().Any<JObject>((JObject v) => Operators.CompareString(v["version"].ToString(), CS$<>8__locals1.$VB$Local_OldVersion, false) == 0) && ModDownload.DlFabricListLoader.Output.Value["game"].OfType<JObject>().Any<JObject>((JObject v) => Operators.CompareString(v["version"].ToString(), CS$<>8__locals2.$VB$Local_TargetVersion, false) == 0);
                }
                this.ShowComparison(valueTuple.Item3, valueTuple.Item4, CS$<>8__locals1.$VB$Local_OldVersion, text, valueTuple.Item5.Text, valueTuple.Item6, flag2, valueTuple.Item7);
            }
            this.UpdateStartButton();
        }

        private void ShowComparison(Grid ComparePanel, FrameworkElement CurrentPanel, string OldVersion, string NewVersion, string NewText, MyIconButton RestoreButton, bool CanRestore, FrameworkElement ClearButton = null)
        {
            string text = null;
            if (NewVersion == null)
            {
                if (RestoreButton == this.BtnOptiFineRestore)
                {
                    text = base.LoadOptiFineGetError();
                }
                else if (RestoreButton == this.BtnForgeRestore)
                {
                    text = (McVersion.IsStandardFormat(this.VanillaName) ? base.LoadForgeGetError() : "无");
                }
                else if (RestoreButton == this.BtnNeoForgeRestore)
                {
                    text = ((this.CardNeoForge.Visibility == Visibility.Collapsed) ? "无" : base.LoadNeoForgeGetError());
                }
                else if (RestoreButton == this.BtnFabricRestore)
                {
                    text = ((this.CardFabric.Visibility == Visibility.Collapsed) ? "无" : base.LoadFabricGetError());
                }
                else if (RestoreButton == this.BtnLiteLoaderRestore)
                {
                    text = ((this.CardLiteLoader.Visibility == Visibility.Collapsed) ? "无" : base.LoadLiteLoaderGetError());
                }
                else if (RestoreButton == this.BtnFabricApiRestore)
                {
                    text = base.LoadFabricApiGetError();
                }
                else if (RestoreButton == this.BtnOptiFabricRestore)
                {
                    text = base.LoadOptiFabricGetError();
                }
                NewText = text ?? "可以添加";
            }
            bool flag = NewVersion == null && (Operators.CompareString(NewText, "获取中……", false) == 0 || NewText.StartsWithF("获取失败：", false));
            bool flag2 = Operators.CompareString(OldVersion, NewVersion, false) != 0 && !flag;
            CanRestore = flag2 && OldVersion != null && CanRestore;
            ComparePanel.Visibility = (flag2 ? Visibility.Visible : Visibility.Collapsed);
            CurrentPanel.Visibility = (flag2 ? Visibility.Collapsed : Visibility.Visible);
            RestoreButton.Visibility = (CanRestore ? Visibility.Visible : Visibility.Collapsed);
            if (ClearButton != null)
            {
                ClearButton.ToolTip = ((OldVersion == null) ? "取消安装" : "移除");
            }
            bool flag3 = ClearButton != null && ClearButton.Visibility == Visibility.Visible;
            RestoreButton.Margin = new Thickness(0.0, 5.0, (double)((CanRestore && flag3) ? 60 : 32), 0.0);
            ComparePanel.Margin = new Thickness(ComparePanel.Margin.Left, ComparePanel.Margin.Top, (double)((CanRestore && flag3) ? 110 : 80), ComparePanel.Margin.Bottom);
            if (flag2)
            {
                if (OldVersion != null)
                {
                    MyCard myCard = (MyCard)ComparePanel.Parent;
                    if (myCard.Visibility == Visibility.Collapsed)
                    {
                        myCard.IsSwapped = true;
                        if (myCard.MainSwap != null)
                        {
                            myCard.MainSwap.Visibility = Visibility.Collapsed;
                        }
                        myCard.Visibility = Visibility.Visible;
                    }
                }
                DockPanel dockPanel = (DockPanel)ComparePanel.Children[0];
                DockPanel dockPanel2 = (DockPanel)ComparePanel.Children[2];
                dockPanel.Children[0].Visibility = ((OldVersion == null) ? Visibility.Collapsed : Visibility.Visible);
                ((TextBlock)dockPanel.Children[1]).Margin = ((OldVersion == null) ? new Thickness(0.0) : new Thickness(7.0, 0.0, 0.0, 0.0));
                dockPanel2.Children[0].Visibility = ((NewVersion == null) ? Visibility.Collapsed : Visibility.Visible);
                TextBlock textBlock = (TextBlock)dockPanel2.Children[1];
                textBlock.Margin = ((NewVersion == null) ? new Thickness(0.0) : new Thickness(7.0, 0.0, 0.0, 0.0));
                textBlock.Text = ((NewVersion == null) ? "移除" : NewText);
                if (NewVersion == null && text != null)
                {
                    textBlock.Inlines.Add(new Run(string.Format("（{0}）", (Operators.CompareString(text, "无", false) == 0) ? "无可用版本" : text))
                    {
                        Foreground = ModSecret.ColorGray4
                    });
                }
            }
        }

        private void RestoreComponent(object sender, EventArgs e)
        {
            MyIconButton myIconButton = (MyIconButton)sender;
            if (myIconButton == this.BtnMinecraftRestore)
            {
                this.IsRestoringComponents = true;
                try
                {
                    this.PendingComponents.Clear();
                    this.MinecraftSelected(ModDownloadLib.McDownloadListItem((JObject)myIconButton.Tag, null, false).Init(), null);
                    if (this.ForgeVersionLoader != null)
                    {
                        this.LoadForge.State = this.ForgeVersionLoader;
                    }
                    this.SelectedOptiFine = this.BtnOptiFineRestore.Tag as ModDownload.DlOptiFineListEntry;
                    this.SelectedForge = this.BtnForgeRestore.Tag as ModDownload.DlForgeVersionEntry;
                    this.SelectedNeoForge = this.BtnNeoForgeRestore.Tag as ModDownload.DlNeoForgeListEntry;
                    this.SelectedFabric = this.BtnFabricRestore.Tag as string;
                    this.SelectedLiteLoader = this.BtnLiteLoaderRestore.Tag as ModDownload.DlLiteLoaderListEntry;
                    this.SelectedFabricApi = this.BtnFabricApiRestore.Tag as ResourceVersion;
                    this.SelectedOptiFabric = this.BtnOptiFabricRestore.Tag as ResourceVersion;
                    this.AutoSelectedFabricApi = this.SelectedFabric != null;
                    this.AutoSelectedOptiFabric = this.SelectedFabric != null && this.SelectedOptiFine != null;
                }
                finally
                {
                    this.IsRestoringComponents = false;
                }
                base.ReloadComponentLists();
                return;
            }
            if (myIconButton == this.BtnOptiFineRestore)
            {
                this.SelectedOptiFine = myIconButton.Tag as ModDownload.DlOptiFineListEntry;
                this.AutoSelectedOptiFabric = this.SelectedOptiFine != null && this.SelectedFabric != null;
                if (this.SelectedOptiFine == null)
                {
                    this.SelectedOptiFabric = null;
                }
            }
            else if (myIconButton == this.BtnForgeRestore)
            {
                this.SelectedForge = myIconButton.Tag as ModDownload.DlForgeVersionEntry;
            }
            else if (myIconButton == this.BtnNeoForgeRestore)
            {
                this.SelectedNeoForge = myIconButton.Tag as ModDownload.DlNeoForgeListEntry;
            }
            else if (myIconButton == this.BtnFabricRestore)
            {
                this.SelectedFabric = myIconButton.Tag as string;
                this.AutoSelectedFabricApi = this.SelectedFabric != null;
                this.AutoSelectedOptiFabric = this.SelectedFabric != null && this.SelectedOptiFine != null;
                if (this.SelectedFabric == null)
                {
                    this.SelectedFabricApi = null;
                    this.SelectedOptiFabric = null;
                }
            }
            else if (myIconButton == this.BtnFabricApiRestore)
            {
                this.SelectedFabricApi = myIconButton.Tag as ResourceVersion;
                this.AutoSelectedFabricApi = true;
            }
            else if (myIconButton == this.BtnOptiFabricRestore)
            {
                this.SelectedOptiFabric = myIconButton.Tag as ResourceVersion;
                this.AutoSelectedOptiFabric = true;
            }
            else if (myIconButton == this.BtnLiteLoaderRestore)
            {
                this.SelectedLiteLoader = myIconButton.Tag as ModDownload.DlLiteLoaderListEntry;
            }
            ((MyCard)myIconButton.Parent).IsSwapped = true;
            base.ReloadComponentLists();
        }

        private void CardMinecraft_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ShowMinecraftVersionList();
            e.Handled = true;
        }

        private void UpdateStartButton()
        {
            this.BtnStart.IsEnabled = this.TextSelectName.IsValidated && this.InitialTarget != null && !this.PendingComponents.Any<KeyValuePair<MyLoading, string>>() && (!this.IsVanillaVersionReliable || !this.InitialTarget.IsEquivalentTo(base.GetSelectedComponents()) || !string.Equals(this.TextSelectName.Text, this.OriginalName, StringComparison.Ordinal));
        }

        private void UpdateOriginalNameVisibility()
        {
            bool flag = !string.Equals(this.TextSelectName.Text, this.OriginalName, StringComparison.Ordinal);
            Visibility visibility = (flag ? Visibility.Visible : Visibility.Collapsed);
            if (this.LabSelectName.Visibility != visibility)
            {
                this.LabSelectName.Visibility = visibility;
                this.BtnOriginalNameRestore.Visibility = visibility;
                int num = (flag ? 40 : 16);
                if (this.IsInSelectPage && ModAnimation.AniControlEnabled == 0)
                {
                    ModAnimation.AniStart(ModAnimation.AaDouble(delegate(object Delta)
                    {
                        this.TextSelectName.Margin = new Thickness(56.0, 16.0, Conversions.ToDouble(Operators.AddObject(this.TextSelectName.Margin.Right, Delta)), 0.0);
                    }, (double)num - this.TextSelectName.Margin.Right, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false), "DownloadModify Name Margin", false);
                    return;
                }
                ModAnimation.AniStop("DownloadModify Name Margin");
                this.TextSelectName.Margin = new Thickness(56.0, 16.0, (double)num, 0.0);
            }
        }

        private void BtnOriginalNameRestore_Click(object sender, EventArgs e)
        {
            this.TextSelectName.Text = this.OriginalName;
            this.IsSelectNameEdited = !string.Equals(this.OriginalName, base.GetSelectName(), StringComparison.Ordinal);
        }

        public override void StartInstall(ModDownloadLib.McInstallRequest Request)
        {
            this.UpdateStartButton();
            if (this.BtnStart.IsEnabled)
            {
                ModMinecraft.McInstance input = this.InitLoader.Input;
                McComponents components = Request.Components;
                string newInstanceName = Request.NewInstanceName;
                Request.NewInstanceName = ((!string.Equals(newInstanceName, input.Name, StringComparison.Ordinal)) ? newInstanceName : null);
                Request.InstanceFolder = input.PathVersion;
                List<string> list = new List<string>();
                if ((this.SelectedForge != null || this.SelectedNeoForge != null || this.SelectedFabric != null) && !PathUtils.IsSame(input.PathIndie, input.PathVersion))
                {
                    list.Add("该版本尚未开启版本隔离，可能与别的 MC 版本共用同一个 Mod 和存档文件夹。\r\n修改版本后，已有的 Mod、存档等需要手动迁移到新的游戏目录。");
                }
                bool flag = Operators.CompareString(this.InitialTarget.MinecraftVersion, components.MinecraftVersion, false) != 0;
                if (!this.IsVanillaVersionReliable && !flag)
                {
                    list.Add(string.Format("PCL 未能精准识别该版本，请确认它是不是 Minecraft {0}，如果不正确，请取消并选择正确的版本！", this.InitialTarget.MinecraftVersion));
                }
                if (!Configs.HintModifyInstanceWarning.Get())
                {
                    list.Add("如果该版本安装了 PCL 不兼容的加载器，或经过第三方修改，这些改动将会丢失！");
                }
                bool flag2 = false;
                try
                {
                    foreach (ModLoaders modLoaders in this.InitialTarget.ModLoaderVersions.Keys.Union<ModLoaders>(components.ModLoaderVersions.Keys))
                    {
                        string text = null;
                        string text2 = null;
                        bool flag3 = this.InitialTarget.ModLoaderVersions.TryGetValue(modLoaders, out text);
                        bool flag4 = components.ModLoaderVersions.TryGetValue(modLoaders, out text2);
                        if (flag3 != flag4 || Operators.CompareString(text, text2, false) != 0)
                        {
                            flag2 = true;
                            break;
                        }
                    }
                }
                finally
                {
                    IEnumerator<ModLoaders> enumerator;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                if ((flag2 || flag) && this.InitialTarget.ModLoaderVersions.Any<KeyValuePair<ModLoaders, string>>())
                {
                    list.Add("现有的 Mod、资源包等在修改后可能会不兼容！");
                }
                if (!list.Any<string>() || ModMain.MyMsgBox("请注意：\r\n• " + list.Join("\r\n• "), "修改确认", "开始修改", "取消", "", true, true, false, null, null, null) == 1)
                {
                    if (!Configs.HintModifyInstanceWarning.Get())
                    {
                        Configs.HintModifyInstanceWarning.Set(true);
                    }
                    if (this.IsVanillaVersionReliable && this.InitialTarget.IsEquivalentTo(components))
                    {
                        try
                        {
                            string pathVersion = input.PathVersion;
                            ModMinecraft.McInstance mcInstance = input.Rename(newInstanceName);
                            if (PageInstanceLeft.Instance != null && PathUtils.IsSame(PageInstanceLeft.Instance.PathVersion, pathVersion))
                            {
                                PageInstanceLeft.Instance = mcInstance;
                            }
                            if (ModMinecraft.McInstanceSelected != null && PathUtils.IsSame(ModMinecraft.McInstanceSelected.PathVersion, pathVersion))
                            {
                                ModMinecraft.McInstanceSelected = mcInstance;
                            }
                            PageInstanceLeft.ReloadCurrentJava();
                            ModMain.Hint("重命名成功！", ModMain.HintType.Green, true);
                            ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
                            ModMain.FrmMain.PageBack();
                            return;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "重命名版本失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadModify.vb");
                            return;
                        }
                    }
                    if (ModDownloadLib.McModificationStart(Request, input))
                    {
                        ModMain.FrmMain.PageBack();
                    }
                }
            }
        }

        internal override MyIconButton BtnMinecraftRestore
        {
            [CompilerGenerated]
            get
            {
                return base.BtnMinecraftRestore;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.RestoreComponent);
                MyIconButton myIconButton = base.BtnMinecraftRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                base.BtnMinecraftRestore = value;
                myIconButton = base.BtnMinecraftRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal override MyIconButton BtnOptiFineRestore
        {
            [CompilerGenerated]
            get
            {
                return base.BtnOptiFineRestore;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.RestoreComponent);
                MyIconButton myIconButton = base.BtnOptiFineRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                base.BtnOptiFineRestore = value;
                myIconButton = base.BtnOptiFineRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal override MyIconButton BtnForgeRestore
        {
            [CompilerGenerated]
            get
            {
                return base.BtnForgeRestore;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.RestoreComponent);
                MyIconButton myIconButton = base.BtnForgeRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                base.BtnForgeRestore = value;
                myIconButton = base.BtnForgeRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal override MyIconButton BtnNeoForgeRestore
        {
            [CompilerGenerated]
            get
            {
                return base.BtnNeoForgeRestore;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.RestoreComponent);
                MyIconButton myIconButton = base.BtnNeoForgeRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                base.BtnNeoForgeRestore = value;
                myIconButton = base.BtnNeoForgeRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal override MyIconButton BtnFabricRestore
        {
            [CompilerGenerated]
            get
            {
                return base.BtnFabricRestore;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.RestoreComponent);
                MyIconButton myIconButton = base.BtnFabricRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                base.BtnFabricRestore = value;
                myIconButton = base.BtnFabricRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal override MyIconButton BtnFabricApiRestore
        {
            [CompilerGenerated]
            get
            {
                return base.BtnFabricApiRestore;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.RestoreComponent);
                MyIconButton myIconButton = base.BtnFabricApiRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                base.BtnFabricApiRestore = value;
                myIconButton = base.BtnFabricApiRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal override MyIconButton BtnOptiFabricRestore
        {
            [CompilerGenerated]
            get
            {
                return base.BtnOptiFabricRestore;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.RestoreComponent);
                MyIconButton myIconButton = base.BtnOptiFabricRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                base.BtnOptiFabricRestore = value;
                myIconButton = base.BtnOptiFabricRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal override MyIconButton BtnLiteLoaderRestore
        {
            [CompilerGenerated]
            get
            {
                return base.BtnLiteLoaderRestore;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.RestoreComponent);
                MyIconButton myIconButton = base.BtnLiteLoaderRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                base.BtnLiteLoaderRestore = value;
                myIconButton = base.BtnLiteLoaderRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal override MyCard CardMinecraft
        {
            [CompilerGenerated]
            get
            {
                return base.CardMinecraft;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this.CardMinecraft_MouseLeftButtonUp);
                MyCard myCard = base.CardMinecraft;
                if (myCard != null)
                {
                    myCard.MouseLeftButtonUp -= mouseButtonEventHandler;
                }
                base.CardMinecraft = value;
                myCard = base.CardMinecraft;
                if (myCard != null)
                {
                    myCard.MouseLeftButtonUp += mouseButtonEventHandler;
                }
            }
        }

        internal override MyTextBox TextSelectName
        {
            [CompilerGenerated]
            get
            {
                return base.TextSelectName;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                TextChangedEventHandler textChangedEventHandler = delegate(object a0, TextChangedEventArgs a1)
                {
                    this.UpdateStartButton();
                };
                MyTextBox.ValidateChangedEventHandler validateChangedEventHandler = delegate(object a0, EventArgs a1)
                {
                    this.UpdateStartButton();
                };
                TextChangedEventHandler textChangedEventHandler2 = delegate(object a0, TextChangedEventArgs a1)
                {
                    this.UpdateOriginalNameVisibility();
                };
                MyTextBox myTextBox = base.TextSelectName;
                if (myTextBox != null)
                {
                    myTextBox.TextChanged -= textChangedEventHandler;
                    MyTextBox.ValidateChanged -= validateChangedEventHandler;
                    myTextBox.TextChanged -= textChangedEventHandler2;
                }
                base.TextSelectName = value;
                myTextBox = base.TextSelectName;
                if (myTextBox != null)
                {
                    myTextBox.TextChanged += textChangedEventHandler;
                    MyTextBox.ValidateChanged += validateChangedEventHandler;
                    myTextBox.TextChanged += textChangedEventHandler2;
                }
            }
        }

        internal override MyIconButton BtnOriginalNameRestore
        {
            [CompilerGenerated]
            get
            {
                return base.BtnOriginalNameRestore;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.BtnOriginalNameRestore_Click);
                MyIconButton myIconButton = base.BtnOriginalNameRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                base.BtnOriginalNameRestore = value;
                myIconButton = base.BtnOriginalNameRestore;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        private ModLoader.LoaderTask<ModMinecraft.McInstance, object> InitLoader;

        private ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>> ForgeVersionLoader;

        private McComponents InitialTarget;

        private bool IsVanillaVersionReliable;

        private string OriginalName;

        private string CustomLogo;

        private readonly Dictionary<MyLoading, string> PendingComponents;

        private bool IsRestoringComponents;

        private bool $STATIC$Init$2001$IsLoad;

        private StaticLocalInitFlag $STATIC$Init$2001$IsLoad$Init;

        private JObject $STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes;

        private StaticLocalInitFlag $STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init;
    }
}
