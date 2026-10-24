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
            base.Unloaded += this._Lambda___R0_1;
            this.PendingComponents = new Dictionary<OurLoading, string>();
        }

        public override void LoaderInit()
        {
            base.ControlInit();
            this.InitLoader = new ModLoader.LoaderTask<ModMinecraft.McInstance, object>("初始化版本修改", new Action<ModLoader.LoaderTask<ModMinecraft.McInstance, object>>(this.LoadInstance), null, ThreadPriority.Normal);
            base.PageLoaderInit<ModMinecraft.McInstance, object>(this.LoadMinecraft, this.PanLoad, this.PanAllBack, null, this.InitLoader, new Action<ModLoader.LoaderTask<ModMinecraft.McInstance, object>>(this.LoadInstanceFinished), (PageDownloadModify._Closure___._I9_0 == null) ? (PageDownloadModify._Closure___._I9_0 = new Func<ModMinecraft.McInstance>(PageDownloadModify._Closure___._I._Lambda___9_0)) : PageDownloadModify._Closure___._I9_0, true);
        }

        public override void Init()
        {
            this.PanBack.ScrollToHome();
            if (this._STATIC_Init_2001_IsLoad_Init == null)
            {
                Interlocked.CompareExchange<StaticLocalInitFlag>(ref this._STATIC_Init_2001_IsLoad_Init, new StaticLocalInitFlag(), null);
            }
            bool flag = false;
            try
            {
                Monitor.Enter(this._STATIC_Init_2001_IsLoad_Init, ref flag);
                if (this._STATIC_Init_2001_IsLoad_Init.State == 0)
                {
                    this._STATIC_Init_2001_IsLoad_Init.State = 2;
                    this._STATIC_Init_2001_IsLoad = false;
                }
                else if (this._STATIC_Init_2001_IsLoad_Init.State == 2)
                {
                    throw new IncompleteInitialization();
                }
            }
            finally
            {
                this._STATIC_Init_2001_IsLoad_Init.State = 1;
                if (flag)
                {
                    Monitor.Exit(this._STATIC_Init_2001_IsLoad_Init);
                }
            }
            if (this._STATIC_Init_2001_IsLoad)
            {
                base.PageLoaderRestart(null, true);
            }
            this._STATIC_Init_2001_IsLoad = true;
        }

        private void PageUnloaded()
        {
            this.InitLoader.Cancel();
        }

        private void LoadInstance(ModLoader.LoaderTask<ModMinecraft.McInstance, object> Task)
        {
            PageDownloadModify._Closure___12_0 CS_locals1 = new PageDownloadModify._Closure___12_0(null);
            CS_locals1._VB_Me = this;
            CS_locals1._VB_Local_Task = Task;
            ModBase.RunInUiWait(new Action(this._Lambda___12_0));
            CS_locals1._VB_Local_Task.Input.Config.DiscardCache();
            McInstallManifest mcInstallManifest = Configs.McInstallManifest.Get(CS_locals1._VB_Local_Task.Input.Config);
            if (mcInstallManifest != null && mcInstallManifest.SchemaVersion > 1)
            {
                Logger.Warn(string.Format("安装清单由更新版本的 PCL 创建（清单版本：{0}，当前版本：{1}）", mcInstallManifest.SchemaVersion, 1), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadModify.vb");
                throw new Exception("更新 PCL 后才能修改它！点击【设置 → 其他 → 检查更新】以更新。");
            }
            if (mcInstallManifest != null)
            {
                McInstallManifest mcInstallManifest2 = mcInstallManifest.RefreshModComponents(CS_locals1._VB_Local_Task.Input.PathIndie);
                if (mcInstallManifest2 != mcInstallManifest)
                {
                    mcInstallManifest = mcInstallManifest2;
                    Configs.McInstallManifest.Set(mcInstallManifest, CS_locals1._VB_Local_Task.Input.Config);
                    CS_locals1._VB_Local_Task.Input.Config.Save();
                }
            }
            CS_locals1._VB_Local_OptiFineVersion = null;
            if (mcInstallManifest != null)
            {
                CS_locals1._VB_Local_Components = mcInstallManifest.Components;
                if (CS_locals1._VB_Local_Components == null)
                {
                    throw new Exception("安装清单缺少组件信息，无法还原已安装组件。");
                }
                this.IsVanillaVersionReliable = true;
            }
            else
            {
                McVersion version = new ModMinecraft.McInstance(CS_locals1._VB_Local_Task.Input.PathVersion).Version;
                if (Operators.CompareString(version.VanillaName, "Unknown", false) == 0)
                {
                    throw new Exception("无法确定 Minecraft 版本，无法修改");
                }
                List<string> list = version.ModLoaderVersions.Where<KeyValuePair<ModLoaders, string>>((PageDownloadModify._Closure___._I12_1 == null) ? (PageDownloadModify._Closure___._I12_1 = new Func<KeyValuePair<ModLoaders, string>, bool>(PageDownloadModify._Closure___._I._Lambda___12_1)) : PageDownloadModify._Closure___._I12_1).Select<KeyValuePair<ModLoaders, string>, string>((PageDownloadModify._Closure___._I12_2 == null) ? (PageDownloadModify._Closure___._I12_2 = new Func<KeyValuePair<ModLoaders, string>, string>(PageDownloadModify._Closure___._I._Lambda___12_2)) : PageDownloadModify._Closure___._I12_2).ToList<string>();
                if (list.Any<string>())
                {
                    throw new Exception(string.Format("无法确定 {0} 的版本，无法修改", list.Join("、")));
                }
                CS_locals1._VB_Local_Components = new McComponents(version.VanillaName, version.ModLoaderVersions, null, null, null);
                CS_locals1._VB_Local_OptiFineVersion = version.OptiFine;
                this.IsVanillaVersionReliable = version.Reliable;
            }
            CS_locals1._VB_Local_MinecraftVersion = CS_locals1._VB_Local_Components.MinecraftVersion;
            if (string.IsNullOrWhiteSpace(CS_locals1._VB_Local_MinecraftVersion))
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
            if (McVersion.IsStandardFormat(CS_locals1._VB_Local_MinecraftVersion))
            {
                loaderTask = new ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>("DlForgeVersion " + CS_locals1._VB_Local_MinecraftVersion, new Action<ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>>(ModDownload.DlForgeVersionMain), null, ThreadPriority.Normal);
                dictionary.Add("Forge", loaderTask);
            }
            try
            {
                foreach (ModLoader.LoaderBase loaderBase in dictionary.Values)
                {
                    loaderBase.Start((loaderBase == loaderTask) ? CS_locals1._VB_Local_MinecraftVersion : null, CS_locals1._VB_Local_Task.IsForceRestarting);
                }
            }
            finally
            {
                Dictionary<string, ModLoader.LoaderBase>.ValueCollection.Enumerator enumerator = default;
                ((IDisposable)enumerator).Dispose();
            }
            while (!CS_locals1._VB_Local_Task.IsCanceled)
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
                    Dictionary<string, ModLoader.LoaderBase>.Enumerator enumerator2 = default;
                    ((IDisposable)enumerator2).Dispose();
                }
                if (!dictionary.Values.All<ModLoader.LoaderBase>((PageDownloadModify._Closure___._I12_3 == null) ? (PageDownloadModify._Closure___._I12_3 = new Func<ModLoader.LoaderBase, bool>(PageDownloadModify._Closure___._I._Lambda___12_3)) : PageDownloadModify._Closure___._I12_3))
                {
                    CS_locals1._VB_Local_Task.Progress = 0.7 * dictionary.Values.Average<ModLoader.LoaderBase>((PageDownloadModify._Closure___._I12_4 == null) ? (PageDownloadModify._Closure___._I12_4 = new Func<ModLoader.LoaderBase, double>(PageDownloadModify._Closure___._I._Lambda___12_4)) : PageDownloadModify._Closure___._I12_4);
                    Thread.Sleep(10);
                }
                else
                {
                    if (CS_locals1._VB_Local_Task.IsCanceled)
                    {
                        break;
                    }
                    CS_locals1._VB_Local_Task.Progress = 0.7;
                    this.MinecraftVersions = (JArray)ModDownload.DlClientListLoader.Output.Value["versions"].DeepClone();
                    this.VanillaData = this.MinecraftVersions.OfType<JObject>().FirstOrDefault<JObject>(new Func<JObject, bool>(CS_locals1._Lambda___5));
                    if (this.VanillaData == null)
                    {
                        throw new Exception(string.Format("Minecraft 列表中没有已安装的 {0}", CS_locals1._VB_Local_MinecraftVersion));
                    }
                    this.VanillaData["id"] = CS_locals1._VB_Local_MinecraftVersion;
                    if (mcInstallManifest != null)
                    {
                        JObject optiFineEntry = CS_locals1._VB_Local_Components.OptiFineEntry;
                        this.SelectedOptiFine = ((optiFineEntry != null) ? optiFineEntry.ToObject<ModDownload.DlOptiFineListEntry>() : null);
                        if (this.SelectedOptiFine != null && !ModDownload.DlOptiFineListLoader.Output.Value.Any<ModDownload.DlOptiFineListEntry>(new Func<ModDownload.DlOptiFineListEntry, bool>(this._Lambda___12_6)))
                        {
                            if (ModDownload.DlOptiFineListBmclapiLoader.State != ModBase.LoadState.Finished)
                            {
                                ModDownload.DlOptiFineListBmclapiLoader.WaitForExit(null, null, false, default(CancellationToken), null);
                            }
                            ModDownload.DlOptiFineListLoader.Output = ModDownload.DlOptiFineListBmclapiLoader.Output;
                        }
                    }
                    else if (CS_locals1._VB_Local_OptiFineVersion != null && Operators.CompareString(CS_locals1._VB_Local_OptiFineVersion, "未知版本", false) != 0)
                    {
                        this.SelectedOptiFine = ModDownload.DlOptiFineListLoader.Output.Value.FirstOrDefault<ModDownload.DlOptiFineListEntry>(new Func<ModDownload.DlOptiFineListEntry, bool>(CS_locals1._Lambda___7));
                        if (this.SelectedOptiFine == null)
                        {
                            throw new Exception(string.Format("OptiFine 列表中没有已安装的 {0}", CS_locals1._VB_Local_OptiFineVersion));
                        }
                    }
                    if (CS_locals1._VB_Local_Components.ModLoaderVersions.ContainsKey(ModLoaders.Forge))
                    {
                        ModDownload.DlForgeVersionEntry dlForgeVersionEntry;
                        if (loaderTask == null)
                        {
                            dlForgeVersionEntry = null;
                        }
                        else
                        {
                            List<ModDownload.DlForgeVersionEntry> output = loaderTask.Output;
                            dlForgeVersionEntry = ((output != null) ? output.FirstOrDefault<ModDownload.DlForgeVersionEntry>(new Func<ModDownload.DlForgeVersionEntry, bool>(CS_locals1._Lambda___8)) : null);
                        }
                        this.SelectedForge = dlForgeVersionEntry;
                        if (this.SelectedForge == null)
                        {
                            throw new Exception(string.Format("Forge 列表中没有已安装的 {0}", CS_locals1._VB_Local_Components.ModLoaderVersions[ModLoaders.Forge]));
                        }
                    }
                    if (CS_locals1._VB_Local_Components.ModLoaderVersions.ContainsKey(ModLoaders.NeoForge))
                    {
                        this.SelectedNeoForge = ModDownload.DlNeoForgeListLoader.Output.Value.FirstOrDefault<ModDownload.DlNeoForgeListEntry>(new Func<ModDownload.DlNeoForgeListEntry, bool>(CS_locals1._Lambda___9));
                        if (this.SelectedNeoForge == null)
                        {
                            throw new Exception(string.Format("NeoForge 列表中没有已安装的 {0}", CS_locals1._VB_Local_Components.ModLoaderVersions[ModLoaders.NeoForge]));
                        }
                    }
                    if (CS_locals1._VB_Local_Components.ModLoaderVersions.ContainsKey(ModLoaders.Fabric))
                    {
                        JObject jobject = ModDownload.DlFabricListLoader.Output.Value["loader"].OfType<JObject>().FirstOrDefault<JObject>(new Func<JObject, bool>[]
                        {
                            new Func<JObject, bool>(CS_locals1._Lambda___10),
                            new Func<JObject, bool>(CS_locals1._Lambda___11)
                        });
                        if (jobject == null || !ModDownload.DlFabricListLoader.Output.Value["game"].OfType<JObject>().Any<JObject>(new Func<JObject, bool>(CS_locals1._Lambda___12)))
                        {
                            throw new Exception(string.Format("Fabric 列表中没有已安装的 {0}", CS_locals1._VB_Local_Components.ModLoaderVersions[ModLoaders.Fabric]));
                        }
                        this.SelectedFabric = jobject["version"].ToString();
                    }
                    if (CS_locals1._VB_Local_Components.ModLoaderVersions.ContainsKey(ModLoaders.LiteLoader))
                    {
                        this.SelectedLiteLoader = ModDownload.DlLiteLoaderListLoader.Output.Value.FirstOrDefault<ModDownload.DlLiteLoaderListEntry>(new Func<ModDownload.DlLiteLoaderListEntry, bool>(CS_locals1._Lambda___13));
                        if (this.SelectedLiteLoader == null)
                        {
                            throw new Exception(string.Format("LiteLoader 列表中没有已安装的 {0}", CS_locals1._VB_Local_Components.ModLoaderVersions[ModLoaders.LiteLoader]));
                        }
                    }
                    if (CS_locals1._VB_Local_Components.FabricApiVersion != null)
                    {
                        this.SelectedFabricApi = ModDownload.DlFabricApiLoader.Output.FirstOrDefault<ResourceVersion>(new Func<ResourceVersion, bool>(CS_locals1._Lambda___14));
                        if (this.SelectedFabricApi == null)
                        {
                            throw new Exception(string.Format("Fabric API 列表中没有已安装的 {0}", CS_locals1._VB_Local_Components.FabricApiVersion));
                        }
                    }
                    if (CS_locals1._VB_Local_Components.OptiFabricVersion != null)
                    {
                        this.SelectedOptiFabric = ModDownload.DlOptiFabricLoader.Output.FirstOrDefault<ResourceVersion>(new Func<ResourceVersion, bool>(CS_locals1._Lambda___15));
                        if (this.SelectedOptiFabric == null)
                        {
                            throw new Exception(string.Format("OptiFabric 列表中没有已安装的 {0}", CS_locals1._VB_Local_Components.OptiFabricVersion));
                        }
                    }
                    List<McInstallManifest.ManagedFile> list2 = new List<McInstallManifest.ManagedFile>();
                    if (mcInstallManifest != null)
                    {
                        list2.AddRange(mcInstallManifest.ManagedFiles);
                    }
                    Lazy<List<ValueTuple<FileInfo, string>>> lazy = new Lazy<List<ValueTuple<FileInfo, string>>>(new Func<List<ValueTuple<FileInfo, string>>>(CS_locals1._Lambda___16));
                    if (this.SelectedFabric != null && this.SelectedFabricApi == null)
                    {
                        List<ResourceVersion> list3 = ModDownload.DlFabricApiLoader.Output.Where<ResourceVersion>(new Func<ResourceVersion, bool>(CS_locals1._Lambda___17)).ToList<ResourceVersion>();
                        List<ValueTuple<FileInfo, string, ResourceVersion>> list4 = new List<ValueTuple<FileInfo, string, ResourceVersion>>();
                        try
                        {
                            List<ValueTuple<FileInfo, string>>.Enumerator enumerator3 = lazy.Value.GetEnumerator();
                            while (enumerator3.MoveNext())
                            {
                                PageDownloadModify._Closure___12_1 CS_locals2 = new PageDownloadModify._Closure___12_1(null);
                                CS_locals2._VB_Local_Entry = enumerator3.Current;
                                ResourceVersion resourceVersion = list3.FirstOrDefault<ResourceVersion>(new Func<ResourceVersion, bool>(CS_locals2._Lambda___18));
                                if (resourceVersion != null)
                                {
                                    list4.Add(new ValueTuple<FileInfo, string, ResourceVersion>(CS_locals2._VB_Local_Entry.Item1, CS_locals2._VB_Local_Entry.Item2, resourceVersion));
                                }
                            }
                        }
                        finally
                        {
                            List<ValueTuple<FileInfo, string>>.Enumerator enumerator3 = default;
                            ((IDisposable)enumerator3).Dispose();
                        }
                        ValueTuple<FileInfo, string, ResourceVersion>? valueTuple = this.SelectMod<ResourceVersion>("Fabric API", list4);
                        if (valueTuple != null)
                        {
                            this.SelectedFabricApi = valueTuple.Value.Item3;
                            list2.Add(new McInstallManifest.ManagedFile("mods/" + valueTuple.Value.Item1.Name.Lower(), valueTuple.Value.Item2, valueTuple.Value.Item1.LastWriteTimeUtc, "FabricApi"));
                        }
                    }
                    if (CS_locals1._VB_Local_Task.IsCanceled)
                    {
                        break;
                    }
                    if (this.SelectedFabric != null && this.SelectedOptiFabric == null)
                    {
                        List<ResourceVersion> list5 = ModDownload.DlOptiFabricLoader.Output.Where<ResourceVersion>(new Func<ResourceVersion, bool>(CS_locals1._Lambda___19)).ToList<ResourceVersion>();
                        bool flag = list5.Any<ResourceVersion>((PageDownloadModify._Closure___._I12_20 == null) ? (PageDownloadModify._Closure___._I12_20 = new Func<ResourceVersion, bool>(PageDownloadModify._Closure___._I._Lambda___12_20)) : PageDownloadModify._Closure___._I12_20);
                        List<ValueTuple<FileInfo, string, ResourceVersion>> list6 = new List<ValueTuple<FileInfo, string, ResourceVersion>>();
                        try
                        {
                            List<ValueTuple<FileInfo, string>>.Enumerator enumerator4 = lazy.Value.GetEnumerator();
                            while (enumerator4.MoveNext())
                            {
                                PageDownloadModify._Closure___12_2 CS_locals3 = new PageDownloadModify._Closure___12_2(null);
                                CS_locals3._VB_Local_Entry = enumerator4.Current;
                                ResourceVersion resourceVersion2 = list5.FirstOrDefault<ResourceVersion>(new Func<ResourceVersion, bool>(CS_locals3._Lambda___21));
                                if (resourceVersion2 == null && flag)
                                {
                                    PageDownloadModify._Closure___12_3 CS_locals4 = new PageDownloadModify._Closure___12_3(null);
                                    CS_locals4._VB_Local_Md5 = CryptographyUtils.ComputeFileHash(CS_locals3._VB_Local_Entry.Item1.FullName, CryptographyUtils.HashMethod.Md5);
                                    resourceVersion2 = list5.FirstOrDefault<ResourceVersion>(new Func<ResourceVersion, bool>(CS_locals4._Lambda___22));
                                }
                                if (resourceVersion2 != null)
                                {
                                    list6.Add(new ValueTuple<FileInfo, string, ResourceVersion>(CS_locals3._VB_Local_Entry.Item1, CS_locals3._VB_Local_Entry.Item2, resourceVersion2));
                                }
                            }
                        }
                        finally
                        {
                            List<ValueTuple<FileInfo, string>>.Enumerator enumerator4 = default;
                            ((IDisposable)enumerator4).Dispose();
                        }
                        ValueTuple<FileInfo, string, ResourceVersion>? valueTuple2 = this.SelectMod<ResourceVersion>("OptiFabric", list6);
                        if (valueTuple2 != null)
                        {
                            this.SelectedOptiFabric = valueTuple2.Value.Item3;
                            list2.Add(new McInstallManifest.ManagedFile("mods/" + valueTuple2.Value.Item1.Name.Lower(), valueTuple2.Value.Item2, valueTuple2.Value.Item1.LastWriteTimeUtc, "OptiFabric"));
                        }
                    }
                    if (CS_locals1._VB_Local_Task.IsCanceled)
                    {
                        break;
                    }
                    List<ValueTuple<FileInfo, string, string>> list7 = new List<ValueTuple<FileInfo, string, string>>();
                    if (this.SelectedOptiFine == null && (this.SelectedForge != null || this.SelectedLiteLoader != null || this.SelectedFabric != null))
                    {
                        if (this._STATIC_LoadInstance_2011151286FC21287E81C_OptiFineHashes_Init == null)
                        {
                            Interlocked.CompareExchange<StaticLocalInitFlag>(ref this._STATIC_LoadInstance_2011151286FC21287E81C_OptiFineHashes_Init, new StaticLocalInitFlag(), null);
                        }
                        bool flag2 = false;
                        try
                        {
                            Monitor.Enter(this._STATIC_LoadInstance_2011151286FC21287E81C_OptiFineHashes_Init, ref flag2);
                            if (this._STATIC_LoadInstance_2011151286FC21287E81C_OptiFineHashes_Init.State == 0)
                            {
                                this._STATIC_LoadInstance_2011151286FC21287E81C_OptiFineHashes_Init.State = 2;
                                this._STATIC_LoadInstance_2011151286FC21287E81C_OptiFineHashes = (JObject)FileUtils.ReadAsJson("Minecraft\\OptiFineHashes.json", null, typeof(Resource));
                            }
                            else if (this._STATIC_LoadInstance_2011151286FC21287E81C_OptiFineHashes_Init.State == 2)
                            {
                                throw new IncompleteInitialization();
                            }
                        }
                        finally
                        {
                            this._STATIC_LoadInstance_2011151286FC21287E81C_OptiFineHashes_Init.State = 1;
                            if (flag2)
                            {
                                Monitor.Exit(this._STATIC_LoadInstance_2011151286FC21287E81C_OptiFineHashes_Init);
                            }
                        }
                        list7 = lazy.Value.Select<ValueTuple<FileInfo, string>, ValueTuple<FileInfo, string, string>>(new Func<ValueTuple<FileInfo, string>, ValueTuple<FileInfo, string, string>>(this._Lambda___12_23)).Where<ValueTuple<FileInfo, string, string>>((PageDownloadModify._Closure___._I12_24 == null) ? (PageDownloadModify._Closure___._I12_24 = new Func<ValueTuple<FileInfo, string, string>, bool>(PageDownloadModify._Closure___._I._Lambda___12_24)) : PageDownloadModify._Closure___._I12_24).ToList<ValueTuple<FileInfo, string, string>>();
                        List<ValueTuple<FileInfo, string, ModDownload.DlOptiFineListEntry>> list8 = new List<ValueTuple<FileInfo, string, ModDownload.DlOptiFineListEntry>>();
                        try
                        {
                            List<ValueTuple<FileInfo, string, string>>.Enumerator enumerator5 = list7.GetEnumerator();
                            while (enumerator5.MoveNext())
                            {
                                PageDownloadModify._Closure___12_4 CS_locals5 = new PageDownloadModify._Closure___12_4(null);
                                CS_locals5._VB_NonLocal__VB_Closure_2 = CS_locals1;
                                CS_locals5._VB_Local_Entry = enumerator5.Current;
                                ModDownload.DlOptiFineListEntry dlOptiFineListEntry = ModDownload.DlOptiFineListLoader.Output.Value.FirstOrDefault<ModDownload.DlOptiFineListEntry>(new Func<ModDownload.DlOptiFineListEntry, bool>(CS_locals5._Lambda___25));
                                if (dlOptiFineListEntry == null)
                                {
                                    ModMain.Hint(string.Format("无法从 OptiFine 列表还原 {0} 对应的 {1}，将跳过该文件。", CS_locals5._VB_Local_Entry.Item1.Name, CS_locals5._VB_Local_Entry.Item3), ModMain.HintType.Blue, true);
                                }
                                else
                                {
                                    list8.Add(new ValueTuple<FileInfo, string, ModDownload.DlOptiFineListEntry>(CS_locals5._VB_Local_Entry.Item1, CS_locals5._VB_Local_Entry.Item2, dlOptiFineListEntry));
                                }
                            }
                        }
                        finally
                        {
                            List<ValueTuple<FileInfo, string, string>>.Enumerator enumerator5 = default;
                            ((IDisposable)enumerator5).Dispose();
                        }
                        ValueTuple<FileInfo, string, ModDownload.DlOptiFineListEntry>? valueTuple3 = this.SelectMod<ModDownload.DlOptiFineListEntry>("OptiFine", list8);
                        if (valueTuple3 != null)
                        {
                            this.SelectedOptiFine = valueTuple3.Value.Item3;
                            string text = PathUtils.RemoveExtendedPrefix(valueTuple3.Value.Item1.FullName).Substring(CS_locals1._VB_Local_Task.Input.PathIndie.Length).Replace("\\", "/")
                                .Lower();
                            list2.Add(new McInstallManifest.ManagedFile(text, valueTuple3.Value.Item2, valueTuple3.Value.Item1.LastWriteTimeUtc, "OptiFine"));
                        }
                    }
                    if (CS_locals1._VB_Local_Task.IsCanceled)
                    {
                        break;
                    }
                    if (this.IsVanillaVersionReliable)
                    {
                        PageDownloadModify._Closure___12_0 CS_locals6 = CS_locals1;
                        string _VB_Local_MinecraftVersion = CS_locals1._VB_Local_MinecraftVersion;
                        Dictionary<ModLoaders, string> modLoaderVersions = CS_locals1._VB_Local_Components.ModLoaderVersions;
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
                        CS_locals6._VB_Local_Components = new McComponents(_VB_Local_MinecraftVersion, modLoaderVersions, jobject2, text2, text3);
                        if (mcInstallManifest == null || !CS_locals1._VB_Local_Components.IsEquivalentTo(mcInstallManifest.Components) || !list2.SequenceEqual<McInstallManifest.ManagedFile>(mcInstallManifest.ManagedFiles))
                        {
                            Configs.McInstallManifest.Set(new McInstallManifest(1, CS_locals1._VB_Local_Components, DateTime.UtcNow, list2), CS_locals1._VB_Local_Task.Input.Config);
                            CS_locals1._VB_Local_Task.Input.Config.Save();
                            Logger.Info(string.Format("已为 {0} 更新安装清单", CS_locals1._VB_Local_Task.Input.Name), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadModify.vb");
                        }
                    }
                    if (!CS_locals1._VB_Local_Task.IsCanceled)
                    {
                        this.ForgeVersionLoader = loaderTask;
                        break;
                    }
                    break;
                }
            }
        }

        private ValueTuple<FileInfo, string, T>? SelectMod<T>(string ComponentName, List<ValueTuple<FileInfo, string, T>> Matches)
        {
            PageDownloadModify._Closure___13_0<T> CS_locals1 = new PageDownloadModify._Closure___13_0<T>();
            CS_locals1._VB_Local_ComponentName = ComponentName;
            CS_locals1._VB_Local_Matches = Matches;
            ValueTuple<FileInfo, string, T>? valueTuple;
            if (CS_locals1._VB_Local_Matches.Count == 0)
            {
                valueTuple = null;
            }
            else if (CS_locals1._VB_Local_Matches.Count == 1)
            {
                valueTuple = new ValueTuple<FileInfo, string, T>?(CS_locals1._VB_Local_Matches[0]);
            }
            else
            {
                CS_locals1._VB_Local_Choice = null;
                ModBase.RunInUiWait(new Action(CS_locals1._Lambda___0));
                if (CS_locals1._VB_Local_Choice == null)
                {
                    throw new Exception(string.Format("发现了多个 {0}", CS_locals1._VB_Local_ComponentName));
                }
                valueTuple = new ValueTuple<FileInfo, string, T>?(CS_locals1._VB_Local_Matches[CS_locals1._VB_Local_Choice.Value]);
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
            OurListItem ourListItem = ModDownloadLib.McDownloadListItem(this.VanillaData, null, false).Init();
            this.VanillaName = ourListItem.Title;
            this.VanillaIcon = ourListItem.Logo;
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
                ValueTuple<OurLoading, string>[] array = new ValueTuple<OurLoading, string>[7];
                array[0] = new ValueTuple<OurLoading, string>(this.LoadOptiFine, selectedComponents.OptiFineVersion);
                array[1] = new ValueTuple<OurLoading, string>(this.LoadFabricApi, selectedComponents.FabricApiVersion);
                array[2] = new ValueTuple<OurLoading, string>(this.LoadOptiFabric, selectedComponents.OptiFabricVersion);
                int num = 3;
                OurLoading loadForge = this.LoadForge;
                ModDownload.DlForgeVersionEntry selectedForge = this.SelectedForge;
                array[num] = new ValueTuple<OurLoading, string>(loadForge, (selectedForge != null) ? selectedForge.VersionName : null);
                int num2 = 4;
                OurLoading loadNeoForge = this.LoadNeoForge;
                ModDownload.DlNeoForgeListEntry selectedNeoForge = this.SelectedNeoForge;
                array[num2] = new ValueTuple<OurLoading, string>(loadNeoForge, (selectedNeoForge != null) ? selectedNeoForge.VersionName : null);
                array[5] = new ValueTuple<OurLoading, string>(this.LoadFabric, this.SelectedFabric);
                int num3 = 6;
                OurLoading loadLiteLoader = this.LoadLiteLoader;
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
                array[num3] = new ValueTuple<OurLoading, string>(loadLiteLoader, text);
                foreach (ValueTuple<OurLoading, string> valueTuple in array)
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
                if (this.IsInSelectPage && this.VanillaName != null && this.PendingComponents.Any<KeyValuePair<OurLoading, string>>())
                {
                    this.IsRestoringComponents = true;
                    try
                    {
                        bool flag = false;
                        foreach (OurLoading ourLoading in new OurLoading[] { this.LoadFabric, this.LoadForge, this.LoadNeoForge, this.LoadLiteLoader, this.LoadOptiFine, this.LoadFabricApi, this.LoadOptiFabric })
                        {
                            PageDownloadModify._Closure___17_0 CS_locals1 = new PageDownloadModify._Closure___17_0(null);
                            CS_locals1._VB_Local_Version = null;
                            if (this.PendingComponents.TryGetValue(ourLoading, out CS_locals1._VB_Local_Version) && ourLoading.State.IsLoader)
                            {
                                ModLoader.LoaderBase loaderBase = (ModLoader.LoaderBase)ourLoading.State;
                                if (ourLoading == this.LoadOptiFine)
                                {
                                    if (new OurLoading[] { this.LoadFabric, this.LoadForge, this.LoadNeoForge, this.LoadLiteLoader }.Any<OurLoading>(new Func<OurLoading, bool>(this._Lambda___17_0)))
                                    {
                                        goto IL_05FC;
                                    }
                                }
                                else if (ourLoading == this.LoadFabricApi)
                                {
                                    if (this.PendingComponents.ContainsKey(this.LoadFabric))
                                    {
                                        goto IL_05FC;
                                    }
                                }
                                else if (ourLoading == this.LoadForge)
                                {
                                    if (!McVersion.IsStandardFormat(this.VanillaName))
                                    {
                                        this.PendingComponents.Remove(ourLoading);
                                        goto IL_05FC;
                                    }
                                    if (Operators.CompareString(((ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>)loaderBase).Input, this.VanillaName, false) != 0)
                                    {
                                        goto IL_05FC;
                                    }
                                    if (loaderBase.State == ModBase.LoadState.Failed && Operators.CompareString(loaderBase.Error.Message, "无", false) == 0)
                                    {
                                        this.PendingComponents.Remove(ourLoading);
                                        goto IL_05FC;
                                    }
                                }
                                else if (ourLoading == this.LoadOptiFabric && (this.PendingComponents.ContainsKey(this.LoadFabric) || this.PendingComponents.ContainsKey(this.LoadOptiFine)))
                                {
                                    goto IL_05FC;
                                }
                                if (loaderBase.State == ModBase.LoadState.Finished)
                                {
                                    this.PendingComponents.Remove(ourLoading);
                                    flag = true;
                                    if (ourLoading == this.LoadFabric)
                                    {
                                        if (this.SelectedFabric == null && base.LoadFabricGetError() == null)
                                        {
                                            IEnumerable<JObject> enumerable = ModDownload.DlFabricListLoader.Output.Value["loader"].OfType<JObject>();
                                            JObject jobject = enumerable.FirstOrDefault<JObject>(new Func<JObject, bool>(CS_locals1._Lambda___1)) ?? enumerable.FirstOrDefault<JObject>();
                                            this.SelectedFabric = ((jobject != null) ? jobject["version"].ToString() : null);
                                        }
                                    }
                                    else if (ourLoading == this.LoadOptiFine)
                                    {
                                        if (this.SelectedOptiFine == null && base.LoadOptiFineGetError() == null)
                                        {
                                            IEnumerable<ModDownload.DlOptiFineListEntry> enumerable2 = ModDownload.DlOptiFineListLoader.Output.Value.Where<ModDownload.DlOptiFineListEntry>(new Func<ModDownload.DlOptiFineListEntry, bool>(this._Lambda___17_2));
                                            ModDownload.DlOptiFineListEntry dlOptiFineListEntry;
                                            if ((dlOptiFineListEntry = enumerable2.FirstOrDefault<ModDownload.DlOptiFineListEntry>(new Func<ModDownload.DlOptiFineListEntry, bool>(CS_locals1._Lambda___3))) == null)
                                            {
                                                dlOptiFineListEntry = enumerable2.OrderBy<ModDownload.DlOptiFineListEntry, bool>((PageDownloadModify._Closure___._I17_4 == null) ? (PageDownloadModify._Closure___._I17_4 = new Func<ModDownload.DlOptiFineListEntry, bool>(PageDownloadModify._Closure___._I._Lambda___17_4)) : PageDownloadModify._Closure___._I17_4).ThenByDescending<ModDownload.DlOptiFineListEntry, string>((PageDownloadModify._Closure___._I17_5 == null) ? (PageDownloadModify._Closure___._I17_5 = new Func<ModDownload.DlOptiFineListEntry, string>(PageDownloadModify._Closure___._I._Lambda___17_5)) : PageDownloadModify._Closure___._I17_5, Comparer<string>.Create(new Comparison<string>(McUtils.CompareVersion))).FirstOrDefault<ModDownload.DlOptiFineListEntry>();
                                            }
                                            this.SelectedOptiFine = dlOptiFineListEntry;
                                        }
                                    }
                                    else if (ourLoading == this.LoadForge)
                                    {
                                        if (this.SelectedForge == null && base.LoadForgeGetError() == null)
                                        {
                                            IEnumerable<ModDownload.DlForgeVersionEntry> enumerable3 = ((ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>)loaderBase).Output.Where<ModDownload.DlForgeVersionEntry>(new Func<ModDownload.DlForgeVersionEntry, bool>(this._Lambda___17_6));
                                            ModDownload.DlForgeVersionEntry dlForgeVersionEntry;
                                            if ((dlForgeVersionEntry = enumerable3.FirstOrDefault<ModDownload.DlForgeVersionEntry>(new Func<ModDownload.DlForgeVersionEntry, bool>(CS_locals1._Lambda___7))) == null)
                                            {
                                                dlForgeVersionEntry = enumerable3.OrderByDescending<ModDownload.DlForgeVersionEntry, ModDownload.DlForgeVersionEntry>((PageDownloadModify._Closure___._I17_8 == null) ? (PageDownloadModify._Closure___._I17_8 = new Func<ModDownload.DlForgeVersionEntry, ModDownload.DlForgeVersionEntry>(PageDownloadModify._Closure___._I._Lambda___17_8)) : PageDownloadModify._Closure___._I17_8).FirstOrDefault<ModDownload.DlForgeVersionEntry>();
                                            }
                                            this.SelectedForge = dlForgeVersionEntry;
                                        }
                                    }
                                    else if (ourLoading == this.LoadNeoForge)
                                    {
                                        if (this.SelectedNeoForge == null && base.LoadNeoForgeGetError() == null)
                                        {
                                            IEnumerable<ModDownload.DlNeoForgeListEntry> enumerable4 = ModDownload.DlNeoForgeListLoader.Output.Value.Where<ModDownload.DlNeoForgeListEntry>(new Func<ModDownload.DlNeoForgeListEntry, bool>(this._Lambda___17_9));
                                            this.SelectedNeoForge = enumerable4.FirstOrDefault<ModDownload.DlNeoForgeListEntry>(new Func<ModDownload.DlNeoForgeListEntry, bool>(CS_locals1._Lambda___10)) ?? enumerable4.FirstOrDefault<ModDownload.DlNeoForgeListEntry>();
                                        }
                                    }
                                    else if (ourLoading == this.LoadLiteLoader)
                                    {
                                        if (this.SelectedLiteLoader == null && base.LoadLiteLoaderGetError() == null)
                                        {
                                            IEnumerable<ModDownload.DlLiteLoaderListEntry> enumerable5 = ModDownload.DlLiteLoaderListLoader.Output.Value.Where<ModDownload.DlLiteLoaderListEntry>(new Func<ModDownload.DlLiteLoaderListEntry, bool>(this._Lambda___17_11));
                                            this.SelectedLiteLoader = enumerable5.FirstOrDefault<ModDownload.DlLiteLoaderListEntry>(new Func<ModDownload.DlLiteLoaderListEntry, bool>(CS_locals1._Lambda___12)) ?? enumerable5.FirstOrDefault<ModDownload.DlLiteLoaderListEntry>();
                                        }
                                    }
                                    else if (ourLoading == this.LoadFabricApi)
                                    {
                                        if (this.SelectedFabricApi == null && base.LoadFabricApiGetError() == null)
                                        {
                                            IEnumerable<ResourceVersion> enumerable6 = ModDownload.DlFabricApiLoader.Output.Where<ResourceVersion>(new Func<ResourceVersion, bool>(this._Lambda___17_13));
                                            ResourceVersion resourceVersion;
                                            if ((resourceVersion = enumerable6.FirstOrDefault<ResourceVersion>(new Func<ResourceVersion, bool>(CS_locals1._Lambda___14))) == null)
                                            {
                                                resourceVersion = enumerable6.OrderByDescending<ResourceVersion, DateTime>((PageDownloadModify._Closure___._I17_15 == null) ? (PageDownloadModify._Closure___._I17_15 = new Func<ResourceVersion, DateTime>(PageDownloadModify._Closure___._I._Lambda___17_15)) : PageDownloadModify._Closure___._I17_15).FirstOrDefault<ResourceVersion>();
                                            }
                                            this.SelectedFabricApi = resourceVersion;
                                        }
                                        if (this.SelectedFabricApi != null)
                                        {
                                            this.AutoSelectedFabricApi = true;
                                        }
                                    }
                                    else if (ourLoading == this.LoadOptiFabric)
                                    {
                                        if (this.SelectedOptiFabric == null && base.LoadOptiFabricGetError() == null)
                                        {
                                            IEnumerable<ResourceVersion> enumerable7 = ModDownload.DlOptiFabricLoader.Output.Where<ResourceVersion>(new Func<ResourceVersion, bool>(this._Lambda___17_16));
                                            ResourceVersion resourceVersion2;
                                            if ((resourceVersion2 = enumerable7.FirstOrDefault<ResourceVersion>(new Func<ResourceVersion, bool>(CS_locals1._Lambda___17))) == null)
                                            {
                                                resourceVersion2 = enumerable7.OrderByDescending<ResourceVersion, DateTime>((PageDownloadModify._Closure___._I17_18 == null) ? (PageDownloadModify._Closure___._I17_18 = new Func<ResourceVersion, DateTime>(PageDownloadModify._Closure___._I._Lambda___17_18)) : PageDownloadModify._Closure___._I17_18).FirstOrDefault<ResourceVersion>();
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
            foreach (ValueTuple<ModLoaders, OurCard, Grid, Grid, TextBlock, OurIconButton, OurIconButton> valueTuple in new ValueTuple<ModLoaders, OurCard, Grid, Grid, TextBlock, OurIconButton, OurIconButton>[]
            {
                new ValueTuple<ModLoaders, OurCard, Grid, Grid, TextBlock, OurIconButton, OurIconButton>(ModLoaders.Forge, this.CardForge, this.PanForgeCompare, this.PanForgeInfo, this.LabForge, this.BtnForgeRestore, this.BtnForgeClear),
                new ValueTuple<ModLoaders, OurCard, Grid, Grid, TextBlock, OurIconButton, OurIconButton>(ModLoaders.NeoForge, this.CardNeoForge, this.PanNeoForgeCompare, this.PanNeoForgeInfo, this.LabNeoForge, this.BtnNeoForgeRestore, this.BtnNeoForgeClear),
                new ValueTuple<ModLoaders, OurCard, Grid, Grid, TextBlock, OurIconButton, OurIconButton>(ModLoaders.Fabric, this.CardFabric, this.PanFabricCompare, this.PanFabricInfo, this.LabFabric, this.BtnFabricRestore, this.BtnFabricClear),
                new ValueTuple<ModLoaders, OurCard, Grid, Grid, TextBlock, OurIconButton, OurIconButton>(ModLoaders.LiteLoader, this.CardLiteLoader, this.PanLiteLoaderCompare, this.PanLiteLoaderInfo, this.LabLiteLoader, this.BtnLiteLoaderRestore, this.BtnLiteLoaderClear)
            })
            {
                PageDownloadModify._Closure___18_0 CS_locals1 = new PageDownloadModify._Closure___18_0(null);
                CS_locals1._VB_Local_OldVersion = null;
                string text = null;
                this.InitialTarget.ModLoaderVersions.TryGetValue(valueTuple.Item1, out CS_locals1._VB_Local_OldVersion);
                selectedComponents.ModLoaderVersions.TryGetValue(valueTuple.Item1, out text);
                bool flag2 = CS_locals1._VB_Local_OldVersion == null || (flag && valueTuple.Item2.MainSwap.Visibility == Visibility.Visible && valueTuple.Item6.Tag != null);
                if (valueTuple.Item1 == ModLoaders.Forge && CS_locals1._VB_Local_OldVersion != null && flag2)
                {
                    flag2 = this.SelectedOptiFine == null || PageDownloadInstall.IsOptiFineSuitForForge(this.SelectedOptiFine, (ModDownload.DlForgeVersionEntry)valueTuple.Item6.Tag);
                }
                if (valueTuple.Item1 == ModLoaders.Fabric && CS_locals1._VB_Local_OldVersion != null)
                {
                    PageDownloadModify._Closure___18_1 CS_locals2 = new PageDownloadModify._Closure___18_1(null);
                    CS_locals2._VB_Local_TargetVersion = this.VanillaName.Replace("∞", "infinite").Replace("Combat Test 7c", "1.16_combat-3");
                    flag2 = this.CardFabric.Visibility == Visibility.Visible && this.CardFabric.MainSwap.Visibility == Visibility.Visible && ModDownload.DlFabricListLoader.Output.Value["loader"].OfType<JObject>().Any<JObject>(new Func<JObject, bool>(CS_locals1._Lambda___0)) && ModDownload.DlFabricListLoader.Output.Value["game"].OfType<JObject>().Any<JObject>(new Func<JObject, bool>(CS_locals2._Lambda___1));
                }
                this.ShowComparison(valueTuple.Item3, valueTuple.Item4, CS_locals1._VB_Local_OldVersion, text, valueTuple.Item5.Text, valueTuple.Item6, flag2, valueTuple.Item7);
            }
            this.UpdateStartButton();
        }

        private void ShowComparison(Grid ComparePanel, FrameworkElement CurrentPanel, string OldVersion, string NewVersion, string NewText, OurIconButton RestoreButton, bool CanRestore, FrameworkElement ClearButton = null)
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
                    OurCard ourCard = (OurCard)ComparePanel.Parent;
                    if (ourCard.Visibility == Visibility.Collapsed)
                    {
                        ourCard.IsSwapped = true;
                        if (ourCard.MainSwap != null)
                        {
                            ourCard.MainSwap.Visibility = Visibility.Collapsed;
                        }
                        ourCard.Visibility = Visibility.Visible;
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
            OurIconButton ourIconButton = (OurIconButton)sender;
            if (ourIconButton == this.BtnMinecraftRestore)
            {
                this.IsRestoringComponents = true;
                try
                {
                    this.PendingComponents.Clear();
                    this.MinecraftSelected(ModDownloadLib.McDownloadListItem((JObject)ourIconButton.Tag, null, false).Init(), null);
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
            if (ourIconButton == this.BtnOptiFineRestore)
            {
                this.SelectedOptiFine = ourIconButton.Tag as ModDownload.DlOptiFineListEntry;
                this.AutoSelectedOptiFabric = this.SelectedOptiFine != null && this.SelectedFabric != null;
                if (this.SelectedOptiFine == null)
                {
                    this.SelectedOptiFabric = null;
                }
            }
            else if (ourIconButton == this.BtnForgeRestore)
            {
                this.SelectedForge = ourIconButton.Tag as ModDownload.DlForgeVersionEntry;
            }
            else if (ourIconButton == this.BtnNeoForgeRestore)
            {
                this.SelectedNeoForge = ourIconButton.Tag as ModDownload.DlNeoForgeListEntry;
            }
            else if (ourIconButton == this.BtnFabricRestore)
            {
                this.SelectedFabric = ourIconButton.Tag as string;
                this.AutoSelectedFabricApi = this.SelectedFabric != null;
                this.AutoSelectedOptiFabric = this.SelectedFabric != null && this.SelectedOptiFine != null;
                if (this.SelectedFabric == null)
                {
                    this.SelectedFabricApi = null;
                    this.SelectedOptiFabric = null;
                }
            }
            else if (ourIconButton == this.BtnFabricApiRestore)
            {
                this.SelectedFabricApi = ourIconButton.Tag as ResourceVersion;
                this.AutoSelectedFabricApi = true;
            }
            else if (ourIconButton == this.BtnOptiFabricRestore)
            {
                this.SelectedOptiFabric = ourIconButton.Tag as ResourceVersion;
                this.AutoSelectedOptiFabric = true;
            }
            else if (ourIconButton == this.BtnLiteLoaderRestore)
            {
                this.SelectedLiteLoader = ourIconButton.Tag as ModDownload.DlLiteLoaderListEntry;
            }
            ((OurCard)ourIconButton.Parent).IsSwapped = true;
            base.ReloadComponentLists();
        }

        private void CardMinecraft_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ShowMinecraftVersionList();
            e.Handled = true;
        }

        private void UpdateStartButton()
        {
            this.BtnStart.IsEnabled = this.TextSelectName.IsValidated && this.InitialTarget != null && !this.PendingComponents.Any<KeyValuePair<OurLoading, string>>() && (!this.IsVanillaVersionReliable || !this.InitialTarget.IsEquivalentTo(base.GetSelectedComponents()) || !string.Equals(this.TextSelectName.Text, this.OriginalName, StringComparison.Ordinal));
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
                    ModAnimation.AniStart(ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___23_0), (double)num - this.TextSelectName.Margin.Right, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false), "DownloadModify Name Margin", false);
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
                    IEnumerator<ModLoaders> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                if ((flag2 || flag) && this.InitialTarget.ModLoaderVersions.Any<KeyValuePair<ModLoaders, string>>())
                {
                    list.Add("现有的 Mod、资源包等在修改后可能会不兼容！");
                }
                if (!list.Any<string>() || ModMain.OurMsgBox("请注意：\r\n• " + list.Join("\r\n• "), "修改确认", "开始修改", "取消", "", true, true, false, null, null, null) == 1)
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
                            if (((object)(PageInstanceLeft.Instance) != (object)null) && PathUtils.IsSame(PageInstanceLeft.Instance.PathVersion, pathVersion))
                            {
                                PageInstanceLeft.Instance = mcInstance;
                            }
                            if (((object)(ModMinecraft.McInstanceSelected) != (object)null) && PathUtils.IsSame(ModMinecraft.McInstanceSelected.PathVersion, pathVersion))
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

        [CompilerGenerated]
        private void _Lambda___R0_1(object a0, RoutedEventArgs a1)
        {
            this.PageUnloaded();
        }

        [CompilerGenerated]
        private void _Lambda___12_0()
        {
            ModAnimation.AniStop("FrmDownloadInstall SelectPageSwitch");
            this.IsSelectNameEdited = false;
            base.ClearSelected();
            this.PendingComponents.Clear();
            this.InitialTarget = null;
            this.IsInSelectPage = false;
            this.PanAllBack.IsHitTestVisible = false;
        }

        [CompilerGenerated]
        private bool _Lambda___12_6(ModDownload.DlOptiFineListEntry v)
        {
            return Operators.CompareString(v.FileName, this.SelectedOptiFine.FileName, false) == 0;
        }

        [CompilerGenerated]
        private ValueTuple<FileInfo, string, string> _Lambda___12_23(ValueTuple<FileInfo, string> Entry)
        {
            FileInfo item = Entry.Item1;
            string item2 = Entry.Item2;
            JToken jtoken = this._STATIC_LoadInstance_2011151286FC21287E81C_OptiFineHashes[Entry.Item2];
            return new ValueTuple<FileInfo, string, string>(item, item2, (jtoken != null) ? jtoken.ToString() : null);
        }

        [CompilerGenerated]
        private bool _Lambda___17_0(OurLoading Component)
        {
            return this.PendingComponents.ContainsKey(Component);
        }

        [CompilerGenerated]
        private bool _Lambda___17_2(ModDownload.DlOptiFineListEntry v)
        {
            return Operators.CompareString(v.Inherit, this.VanillaName, false) == 0 && (this.SelectedForge == null || PageDownloadInstall.IsOptiFineSuitForForge(v, this.SelectedForge));
        }

        [CompilerGenerated]
        private bool _Lambda___17_6(ModDownload.DlForgeVersionEntry v)
        {
            return Operators.CompareString(v.Inherit, this.VanillaName, false) == 0 && Operators.CompareString(v.Category, "universal", false) != 0 && Operators.CompareString(v.Category, "client", false) != 0 && (this.SelectedOptiFine == null || PageDownloadInstall.IsOptiFineSuitForForge(this.SelectedOptiFine, v));
        }

        [CompilerGenerated]
        private bool _Lambda___17_9(ModDownload.DlNeoForgeListEntry v)
        {
            return Operators.CompareString(v.Inherit, this.VanillaName, false) == 0;
        }

        [CompilerGenerated]
        private bool _Lambda___17_11(ModDownload.DlLiteLoaderListEntry v)
        {
            return Operators.CompareString(v.Inherit, this.VanillaName, false) == 0;
        }

        [CompilerGenerated]
        private bool _Lambda___17_13(ResourceVersion v)
        {
            return PageDownloadInstall.IsFabricApiCompatible(v, this.VanillaName);
        }

        [CompilerGenerated]
        private bool _Lambda___17_16(ResourceVersion v)
        {
            return PageDownloadInstall.IsOptiFabricCompatible(v, this.VanillaName);
        }

        internal override OurIconButton BtnMinecraftRestore_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return base.BtnMinecraftRestore_WithEvents;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this.RestoreComponent);
                OurIconButton ourIconButton = base.BtnMinecraftRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                base.BtnMinecraftRestore_WithEvents = value;
                ourIconButton = base.BtnMinecraftRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }

        internal override OurIconButton BtnOptiFineRestore_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return base.BtnOptiFineRestore_WithEvents;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this.RestoreComponent);
                OurIconButton ourIconButton = base.BtnOptiFineRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                base.BtnOptiFineRestore_WithEvents = value;
                ourIconButton = base.BtnOptiFineRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }

        internal override OurIconButton BtnForgeRestore_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return base.BtnForgeRestore_WithEvents;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this.RestoreComponent);
                OurIconButton ourIconButton = base.BtnForgeRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                base.BtnForgeRestore_WithEvents = value;
                ourIconButton = base.BtnForgeRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }

        internal override OurIconButton BtnNeoForgeRestore_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return base.BtnNeoForgeRestore_WithEvents;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this.RestoreComponent);
                OurIconButton ourIconButton = base.BtnNeoForgeRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                base.BtnNeoForgeRestore_WithEvents = value;
                ourIconButton = base.BtnNeoForgeRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }

        internal override OurIconButton BtnFabricRestore_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return base.BtnFabricRestore_WithEvents;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this.RestoreComponent);
                OurIconButton ourIconButton = base.BtnFabricRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                base.BtnFabricRestore_WithEvents = value;
                ourIconButton = base.BtnFabricRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }

        internal override OurIconButton BtnFabricApiRestore_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return base.BtnFabricApiRestore_WithEvents;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this.RestoreComponent);
                OurIconButton ourIconButton = base.BtnFabricApiRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                base.BtnFabricApiRestore_WithEvents = value;
                ourIconButton = base.BtnFabricApiRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }

        internal override OurIconButton BtnOptiFabricRestore_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return base.BtnOptiFabricRestore_WithEvents;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this.RestoreComponent);
                OurIconButton ourIconButton = base.BtnOptiFabricRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                base.BtnOptiFabricRestore_WithEvents = value;
                ourIconButton = base.BtnOptiFabricRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }

        internal override OurIconButton BtnLiteLoaderRestore_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return base.BtnLiteLoaderRestore_WithEvents;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this.RestoreComponent);
                OurIconButton ourIconButton = base.BtnLiteLoaderRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                base.BtnLiteLoaderRestore_WithEvents = value;
                ourIconButton = base.BtnLiteLoaderRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }

        internal override OurCard CardMinecraft_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return base.CardMinecraft_WithEvents;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this.CardMinecraft_MouseLeftButtonUp);
                OurCard ourCard = base.CardMinecraft_WithEvents;
                if (ourCard != null)
                {
                    ourCard.MouseLeftButtonUp -= mouseButtonEventHandler;
                }
                base.CardMinecraft_WithEvents = value;
                ourCard = base.CardMinecraft_WithEvents;
                if (ourCard != null)
                {
                    ourCard.MouseLeftButtonUp += mouseButtonEventHandler;
                }
            }
        }

        internal override OurTextBox TextSelectName_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return base.TextSelectName_WithEvents;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                TextChangedEventHandler textChangedEventHandler = new TextChangedEventHandler(this._Lambda___R9_2);
                OurTextBox.ValidateChangedEventHandler validateChangedEventHandler = new OurTextBox.ValidateChangedEventHandler(this._Lambda___R9_3);
                TextChangedEventHandler textChangedEventHandler2 = new TextChangedEventHandler(this._Lambda___R9_4);
                OurTextBox ourTextBox = base.TextSelectName_WithEvents;
                if (ourTextBox != null)
                {
                    ourTextBox.TextChanged -= textChangedEventHandler;
                    OurTextBox.ValidateChanged -= validateChangedEventHandler;
                    ourTextBox.TextChanged -= textChangedEventHandler2;
                }
                base.TextSelectName_WithEvents = value;
                ourTextBox = base.TextSelectName_WithEvents;
                if (ourTextBox != null)
                {
                    ourTextBox.TextChanged += textChangedEventHandler;
                    OurTextBox.ValidateChanged += validateChangedEventHandler;
                    ourTextBox.TextChanged += textChangedEventHandler2;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R9_2(object a0, TextChangedEventArgs a1)
        {
            this.UpdateStartButton();
        }

        [CompilerGenerated]
        private void _Lambda___R9_3(object a0, EventArgs a1)
        {
            this.UpdateStartButton();
        }

        [CompilerGenerated]
        private void _Lambda___R9_4(object a0, TextChangedEventArgs a1)
        {
            this.UpdateOriginalNameVisibility();
        }

        [CompilerGenerated]
        private void _Lambda___23_0(object Delta)
        {
            this.TextSelectName.Margin = new Thickness(56.0, 16.0, Conversions.ToDouble(Operators.AddObject(this.TextSelectName.Margin.Right, Delta)), 0.0);
        }

        internal override OurIconButton BtnOriginalNameRestore_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return base.BtnOriginalNameRestore_WithEvents;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this.BtnOriginalNameRestore_Click);
                OurIconButton ourIconButton = base.BtnOriginalNameRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                base.BtnOriginalNameRestore_WithEvents = value;
                ourIconButton = base.BtnOriginalNameRestore_WithEvents;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }

        private ModLoader.LoaderTask<ModMinecraft.McInstance, object> InitLoader;

        private ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>> ForgeVersionLoader;

        private McComponents InitialTarget;

        private bool IsVanillaVersionReliable;

        private string OriginalName;

        private string CustomLogo;

        private readonly Dictionary<OurLoading, string> PendingComponents;

        private bool IsRestoringComponents;

        private bool _STATIC_Init_2001_IsLoad;

        private StaticLocalInitFlag _STATIC_Init_2001_IsLoad_Init;

        private JObject _STATIC_LoadInstance_2011151286FC21287E81C_OptiFineHashes;

        private StaticLocalInitFlag _STATIC_LoadInstance_2011151286FC21287E81C_OptiFineHashes_Init;

        [CompilerGenerated]
        [Serializable]
        private new sealed class _Closure___
        {
            public _Closure___()
            {
            }

            static _Closure___()
            {
            }

            internal ModMinecraft.McInstance _Lambda___9_0()
            {
                return PageInstanceLeft.Instance;
            }

            internal bool _Lambda___12_1(KeyValuePair<ModLoaders, string> Loader)
            {
                return string.IsNullOrWhiteSpace(Loader.Value) || Operators.CompareString(Loader.Value, "未知版本", false) == 0;
            }

            internal string _Lambda___12_2(KeyValuePair<ModLoaders, string> Loader)
            {
                return Loader.Key.ToString();
            }

            internal bool _Lambda___12_3(ModLoader.LoaderBase Loader)
            {
                return Loader.State == ModBase.LoadState.Finished || Loader.State == ModBase.LoadState.Failed;
            }

            internal double _Lambda___12_4(ModLoader.LoaderBase Loader)
            {
                return Loader.Progress;
            }

            internal bool _Lambda___12_20(ResourceVersion v)
            {
                return (v.Hash ?? "").Length == 32;
            }

            internal bool _Lambda___12_24(ValueTuple<FileInfo, string, string> Entry)
            {
                return Entry.Item3 != null;
            }

            internal bool _Lambda___17_4(ModDownload.DlOptiFineListEntry v)
            {
                return v.IsPreview;
            }

            internal string _Lambda___17_5(ModDownload.DlOptiFineListEntry v)
            {
                return v.DisplayName;
            }

            internal ModDownload.DlForgeVersionEntry _Lambda___17_8(ModDownload.DlForgeVersionEntry v)
            {
                return v;
            }

            internal DateTime _Lambda___17_15(ResourceVersion v)
            {
                return v.ReleaseDate;
            }

            internal DateTime _Lambda___17_18(ResourceVersion v)
            {
                return v.ReleaseDate;
            }

            public static readonly PageDownloadModify._Closure___ _I = new PageDownloadModify._Closure___();

            public static Func<ModMinecraft.McInstance> _I9_0;

            public static Func<KeyValuePair<ModLoaders, string>, bool> _I12_1;

            public static Func<KeyValuePair<ModLoaders, string>, string> _I12_2;

            public static Func<ModLoader.LoaderBase, bool> _I12_3;

            public static Func<ModLoader.LoaderBase, double> _I12_4;

            public static Func<ResourceVersion, bool> _I12_20;

            public static Func<ValueTuple<FileInfo, string, string>, bool> _I12_24;

            public static Func<ModDownload.DlOptiFineListEntry, bool> _I17_4;

            public static Func<ModDownload.DlOptiFineListEntry, string> _I17_5;

            public static Func<ModDownload.DlForgeVersionEntry, ModDownload.DlForgeVersionEntry> _I17_8;

            public static Func<ResourceVersion, DateTime> _I17_15;

            public static Func<ResourceVersion, DateTime> _I17_18;
        }

        [CompilerGenerated]
        private sealed class _Closure___12_0
        {
            public _Closure___12_0(PageDownloadModify._Closure___12_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_MinecraftVersion = arg0._VB_Local_MinecraftVersion;
                    this._VB_Local_OptiFineVersion = arg0._VB_Local_OptiFineVersion;
                    this._VB_Local_Components = arg0._VB_Local_Components;
                    this._VB_Local_Task = arg0._VB_Local_Task;
                }
            }

            internal bool _Lambda___5(JObject v)
            {
                return Operators.CompareString(v["id"].ToString().Replace("∞", "infinite"), this._VB_Local_MinecraftVersion.Replace("∞", "infinite"), false) == 0;
            }

            internal bool _Lambda___7(ModDownload.DlOptiFineListEntry v)
            {
                return Operators.CompareString(v.Inherit, this._VB_Local_MinecraftVersion, false) == 0 && Operators.CompareString(v.InstanceName.AfterLast("-OptiFine_", false).Replace("HD_U_", ""), this._VB_Local_OptiFineVersion.Replace("HD_U_", ""), false) == 0;
            }

            internal bool _Lambda___8(ModDownload.DlForgeVersionEntry v)
            {
                return Operators.CompareString(v.Inherit, this._VB_Local_MinecraftVersion, false) == 0 && Operators.CompareString(v.VersionName, this._VB_Local_Components.ModLoaderVersions[ModLoaders.Forge], false) == 0 && Operators.CompareString(v.Category, "universal", false) != 0 && Operators.CompareString(v.Category, "client", false) != 0 && (this._VB_Me.SelectedOptiFine == null || PageDownloadInstall.IsOptiFineSuitForForge(this._VB_Me.SelectedOptiFine, v));
            }

            internal bool _Lambda___9(ModDownload.DlNeoForgeListEntry v)
            {
                return Operators.CompareString(v.Inherit, this._VB_Local_MinecraftVersion, false) == 0 && Operators.CompareString(v.VersionName, this._VB_Local_Components.ModLoaderVersions[ModLoaders.NeoForge], false) == 0;
            }

            internal bool _Lambda___10(JObject v)
            {
                return Operators.CompareString(v["version"].ToString(), this._VB_Local_Components.ModLoaderVersions[ModLoaders.Fabric], false) == 0;
            }

            internal bool _Lambda___11(JObject v)
            {
                return Operators.CompareString(v["version"].ToString().Replace("+build", ""), this._VB_Local_Components.ModLoaderVersions[ModLoaders.Fabric], false) == 0;
            }

            internal bool _Lambda___12(JObject v)
            {
                return Operators.CompareString(v["version"].ToString(), this._VB_Local_MinecraftVersion.Replace("∞", "infinite").Replace("Combat Test 7c", "1.16_combat-3"), false) == 0;
            }

            internal bool _Lambda___13(ModDownload.DlLiteLoaderListEntry v)
            {
                return Operators.CompareString(v.Inherit, this._VB_Local_MinecraftVersion, false) == 0 && Operators.CompareString(v.JsonToken["version"].ToString(), this._VB_Local_Components.ModLoaderVersions[ModLoaders.LiteLoader], false) == 0;
            }

            internal bool _Lambda___14(ResourceVersion v)
            {
                return (Operators.CompareString(v.Version, this._VB_Local_Components.FabricApiVersion, false) == 0 || Operators.CompareString(v.Id, this._VB_Local_Components.FabricApiVersion, false) == 0 || Operators.CompareString(v.Display, this._VB_Local_Components.FabricApiVersion, false) == 0) && PageDownloadInstall.IsFabricApiCompatible(v, this._VB_Local_MinecraftVersion);
            }

            internal bool _Lambda___15(ResourceVersion v)
            {
                return (Operators.CompareString(v.Version, this._VB_Local_Components.OptiFabricVersion, false) == 0 || Operators.CompareString(v.Id, this._VB_Local_Components.OptiFabricVersion, false) == 0 || Operators.CompareString(v.Display, this._VB_Local_Components.OptiFabricVersion, false) == 0) && PageDownloadInstall.IsOptiFabricCompatible(v, this._VB_Local_MinecraftVersion);
            }

            internal List<ValueTuple<FileInfo, string>> _Lambda___16()
            {
                string text = this._VB_Local_Task.Input.PathIndie + "mods\\";
                List<string> list = DirectoryUtils.EnumerateFiles(text, false, "*.jar").ToList<string>();
                if (this._VB_Me.SelectedLiteLoader != null)
                {
                    list.AddRange(DirectoryUtils.EnumerateFiles(Path.Combine(text, this._VB_Local_MinecraftVersion), false, "*.jar"));
                }
                List<ValueTuple<FileInfo, string>> list2 = new List<ValueTuple<FileInfo, string>>();
                try
                {
                    foreach (string text2 in list)
                    {
                        if (this._VB_Local_Task.IsCanceled)
                        {
                            return list2;
                        }
                        FileInfo info = FileUtils.GetInfo(text2);
                        list2.Add(new ValueTuple<FileInfo, string>(info, new LocalResourceFile(info).ModrinthHash));
                        this._VB_Local_Task.Progress = 0.7 + 0.28 * (double)list2.Count / (double)list.Count;
                    }
                }
                finally
                {
                    List<string>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
                return list2;
            }

            internal bool _Lambda___17(ResourceVersion v)
            {
                return PageDownloadInstall.IsFabricApiCompatible(v, this._VB_Local_MinecraftVersion);
            }

            internal bool _Lambda___19(ResourceVersion v)
            {
                return PageDownloadInstall.IsOptiFabricCompatible(v, this._VB_Local_MinecraftVersion);
            }

            public string _VB_Local_MinecraftVersion;

            public string _VB_Local_OptiFineVersion;

            public McComponents _VB_Local_Components;

            public ModLoader.LoaderTask<ModMinecraft.McInstance, object> _VB_Local_Task;

            public PageDownloadModify _VB_Me;
        }

        [CompilerGenerated]
        private sealed class _Closure___12_1
        {
            public _Closure___12_1(PageDownloadModify._Closure___12_1 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Entry = arg0._VB_Local_Entry;
                }
            }

            internal bool _Lambda___18(ResourceVersion v)
            {
                return Operators.CompareString(v.Hash, this._VB_Local_Entry.Item2, false) == 0;
            }

            public ValueTuple<FileInfo, string> _VB_Local_Entry;
        }

        [CompilerGenerated]
        private sealed class _Closure___12_2
        {
            public _Closure___12_2(PageDownloadModify._Closure___12_2 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Entry = arg0._VB_Local_Entry;
                }
            }

            internal bool _Lambda___21(ResourceVersion v)
            {
                return Operators.CompareString(v.Hash, this._VB_Local_Entry.Item2, false) == 0;
            }

            public ValueTuple<FileInfo, string> _VB_Local_Entry;
        }

        [CompilerGenerated]
        private sealed class _Closure___12_3
        {
            public _Closure___12_3(PageDownloadModify._Closure___12_3 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Md5 = arg0._VB_Local_Md5;
                }
            }

            internal bool _Lambda___22(ResourceVersion v)
            {
                return Operators.CompareString(v.Hash, this._VB_Local_Md5, false) == 0;
            }

            public string _VB_Local_Md5;
        }

        [CompilerGenerated]
        private sealed class _Closure___12_4
        {
            public _Closure___12_4(PageDownloadModify._Closure___12_4 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Entry = arg0._VB_Local_Entry;
                }
            }

            internal bool _Lambda___25(ModDownload.DlOptiFineListEntry v)
            {
                return Operators.CompareString(v.Inherit, this._VB_NonLocal__VB_Closure_2._VB_Local_MinecraftVersion, false) == 0 && Operators.CompareString(v.FileName, this._VB_Local_Entry.Item3, false) == 0 && (this._VB_NonLocal__VB_Closure_2._VB_Me.SelectedForge == null || PageDownloadInstall.IsOptiFineSuitForForge(v, this._VB_NonLocal__VB_Closure_2._VB_Me.SelectedForge));
            }

            public ValueTuple<FileInfo, string, string> _VB_Local_Entry;

            public PageDownloadModify._Closure___12_0 _VB_NonLocal__VB_Closure_2;
        }

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___13<_CLS0>
        {
            public _Closure___13()
            {
            }

            static _Closure___13()
            {
            }

            internal IOurRadio _Lambda___13_1(ValueTuple<FileInfo, string, _CLS0> Entry)
            {
                return new OurRadioBox
                {
                    Text = Entry.Item1.Name
                };
            }

            public static readonly PageDownloadModify._Closure___13<_CLS0> _I = new PageDownloadModify._Closure___13<_CLS0>();

            public static Func<ValueTuple<FileInfo, string, _CLS0>, IOurRadio> _I13_1;
        }

        [CompilerGenerated]
        private sealed class _Closure___13_0<_CLS0>
        {
            public _Closure___13_0()
            {
            }

            internal void _Lambda___0()
            {
                this._VB_Local_Choice = ModMain.OurMsgBoxSelect(this._VB_Local_Matches.Select<ValueTuple<FileInfo, string, _CLS0>, IOurRadio>((PageDownloadModify._Closure___13<_CLS0>._I13_1 == null) ? (PageDownloadModify._Closure___13<_CLS0>._I13_1 = new Func<ValueTuple<FileInfo, string, _CLS0>, IOurRadio>(PageDownloadModify._Closure___13<_CLS0>._I._Lambda___13_1)) : PageDownloadModify._Closure___13<_CLS0>._I13_1), string.Format("选择当前使用的 {0}", this._VB_Local_ComponentName), "确定", "", false);
            }

            public int? _VB_Local_Choice;

            public List<ValueTuple<FileInfo, string, _CLS0>> _VB_Local_Matches;

            public string _VB_Local_ComponentName;
        }

        [CompilerGenerated]
        private sealed class _Closure___17_0
        {
            public _Closure___17_0(PageDownloadModify._Closure___17_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Version = arg0._VB_Local_Version;
                }
            }

            internal bool _Lambda___1(JObject v)
            {
                return Operators.CompareString(v["version"].ToString(), this._VB_Local_Version, false) == 0;
            }

            internal bool _Lambda___3(ModDownload.DlOptiFineListEntry v)
            {
                return Operators.CompareString(v.InstanceName.AfterLast("-OptiFine_", false), this._VB_Local_Version, false) == 0;
            }

            internal bool _Lambda___7(ModDownload.DlForgeVersionEntry v)
            {
                return Operators.CompareString(v.VersionName, this._VB_Local_Version, false) == 0;
            }

            internal bool _Lambda___10(ModDownload.DlNeoForgeListEntry v)
            {
                return Operators.CompareString(v.VersionName, this._VB_Local_Version, false) == 0;
            }

            internal bool _Lambda___12(ModDownload.DlLiteLoaderListEntry v)
            {
                return Operators.CompareString(v.JsonToken["version"].ToString(), this._VB_Local_Version, false) == 0;
            }

            internal bool _Lambda___14(ResourceVersion v)
            {
                return Operators.CompareString(v.Version, this._VB_Local_Version, false) == 0 || Operators.CompareString(v.Id, this._VB_Local_Version, false) == 0 || Operators.CompareString(v.Display, this._VB_Local_Version, false) == 0;
            }

            internal bool _Lambda___17(ResourceVersion v)
            {
                return Operators.CompareString(v.Version, this._VB_Local_Version, false) == 0 || Operators.CompareString(v.Id, this._VB_Local_Version, false) == 0 || Operators.CompareString(v.Display, this._VB_Local_Version, false) == 0;
            }

            public string _VB_Local_Version;
        }

        [CompilerGenerated]
        private sealed class _Closure___18_0
        {
            public _Closure___18_0(PageDownloadModify._Closure___18_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_OldVersion = arg0._VB_Local_OldVersion;
                }
            }

            internal bool _Lambda___0(JObject v)
            {
                return Operators.CompareString(v["version"].ToString(), this._VB_Local_OldVersion, false) == 0;
            }

            public string _VB_Local_OldVersion;
        }

        [CompilerGenerated]
        private sealed class _Closure___18_1
        {
            public _Closure___18_1(PageDownloadModify._Closure___18_1 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_TargetVersion = arg0._VB_Local_TargetVersion;
                }
            }

            internal bool _Lambda___1(JObject v)
            {
                return Operators.CompareString(v["version"].ToString(), this._VB_Local_TargetVersion, false) == 0;
            }

            public string _VB_Local_TargetVersion;
        }
    }
}
