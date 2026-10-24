using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCLCS;

namespace PCL
{
    [StandardModule]
    public static class LocalResourceLoaders
    {
        // Note: this type is marked as 'beforefieldinit'.
        static LocalResourceLoaders()
        {
        }

        private static void LocalResourceFileLoad(ModLoader.LoaderTask<string, List<LocalResourceFile>> Loader)
        {
            try
            {
                ModBase.RunInUiWait((LocalResourceLoaders._Closure___._I2_0 == null) ? (LocalResourceLoaders._Closure___._I2_0 = new Action(LocalResourceLoaders._Closure___._I._Lambda___2_0)) : LocalResourceLoaders._Closure___._I2_0);
                if (PageInstanceMod.UpdatingInstanceModFolders.Contains(Loader.Input))
                {
                    Logger.Info(string.Format("等待 Mod 更新完成后才能继续加载 Mod 列表：{0}", Loader.Input), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                    try
                    {
                        ModBase.RunInUiWait((LocalResourceLoaders._Closure___._I2_1 == null) ? (LocalResourceLoaders._Closure___._I2_1 = new Action(LocalResourceLoaders._Closure___._I._Lambda___2_1)) : LocalResourceLoaders._Closure___._I2_1);
                        while (PageInstanceMod.UpdatingInstanceModFolders.Contains(Loader.Input))
                        {
                            if (Loader.IsCanceled)
                            {
                                return;
                            }
                            Thread.Sleep(100);
                        }
                    }
                    finally
                    {
                        ModBase.RunInUiWait((LocalResourceLoaders._Closure___._I2_2 == null) ? (LocalResourceLoaders._Closure___._I2_2 = new Action(LocalResourceLoaders._Closure___._I._Lambda___2_2)) : LocalResourceLoaders._Closure___._I2_2);
                    }
                    ModMain.FrmInstanceMod.LoaderRun(ModLoader.LoaderFolderRunType.UpdateOnly);
                }
                List<string> list = new List<string>();
                if (DirectoryUtils.Exists(Loader.Input))
                {
                    try
                    {
                        foreach (string text in DirectoryUtils.EnumerateFiles(Loader.Input, true, "*"))
                        {
                            if (PathUtils.IsSame(PathUtils.RemoveLastPart(text), Loader.Input) || (((object)(PageInstanceLeft.Instance) != (object)null) && PageInstanceLeft.Instance.Version.ModLoaders.HasFlagF(ModLoaders.Forge) && PageInstanceLeft.Instance.Version.Vanilla.Major < 13 && Operators.CompareString(PathUtils.GetLastPart(PathUtils.RemoveLastPart(text)), string.Format("1.{0}.{1}", PageInstanceLeft.Instance.Version.Vanilla.Major, PageInstanceLeft.Instance.Version.Vanilla.Build), false) == 0))
                            {
                                if (LocalResourceLoaders._STATIC_LocalResourceFileLoad_011151286FC2E1512816911280C8_IsModFile_Init == null)
                                {
                                    Interlocked.CompareExchange<StaticLocalInitFlag>(ref LocalResourceLoaders._STATIC_LocalResourceFileLoad_011151286FC2E1512816911280C8_IsModFile_Init, new StaticLocalInitFlag(), null);
                                }
                                bool flag = false;
                                try
                                {
                                    Monitor.Enter(LocalResourceLoaders._STATIC_LocalResourceFileLoad_011151286FC2E1512816911280C8_IsModFile_Init, ref flag);
                                    if (LocalResourceLoaders._STATIC_LocalResourceFileLoad_011151286FC2E1512816911280C8_IsModFile_Init.State == 0)
                                    {
                                        LocalResourceLoaders._STATIC_LocalResourceFileLoad_011151286FC2E1512816911280C8_IsModFile_Init.State = 2;
                                        LocalResourceLoaders._STATIC_LocalResourceFileLoad_011151286FC2E1512816911280C8_IsModFile = ((LocalResourceLoaders._Closure___._I2_3 == null) ? (LocalResourceLoaders._Closure___._I2_3 = new Func<string, bool>(LocalResourceLoaders._Closure___._I._Lambda___2_3)) : LocalResourceLoaders._Closure___._I2_3);
                                    }
                                    else if (LocalResourceLoaders._STATIC_LocalResourceFileLoad_011151286FC2E1512816911280C8_IsModFile_Init.State == 2)
                                    {
                                        throw new IncompleteInitialization();
                                    }
                                }
                                finally
                                {
                                    LocalResourceLoaders._STATIC_LocalResourceFileLoad_011151286FC2E1512816911280C8_IsModFile_Init.State = 1;
                                    if (flag)
                                    {
                                        Monitor.Exit(LocalResourceLoaders._STATIC_LocalResourceFileLoad_011151286FC2E1512816911280C8_IsModFile_Init);
                                    }
                                }
                                if (LocalResourceLoaders._STATIC_LocalResourceFileLoad_011151286FC2E1512816911280C8_IsModFile(text))
                                {
                                    list.Add(text);
                                }
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
                }
                string text2 = ModBase.PathTemp + "Cache\\LocalMod.json";
                JObject jobject = new JObject();
                try
                {
                    string text3 = FileUtils.TryReadAsString(text2, null, null);
                    if (!string.IsNullOrWhiteSpace(text3))
                    {
                        jobject = (JObject)text3.DeserializeJson();
                        if (!jobject.ContainsKey("version") || jobject["version"].ToObject<int>() != 21)
                        {
                            Logger.Warn("本地 Mod 信息缓存版本已过期，将弃用这些缓存信息", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                            jobject = new JObject();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "读取本地 Mod 信息缓存失败，已重置", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                    jobject = new JObject();
                }
                jobject["version"] = 21;
                List<LocalResourceFile> list2 = new List<LocalResourceFile>();
                try
                {
                    foreach (string text4 in list)
                    {
                        LocalResourceLoaders._Closure___2_0 CS_locals1 = new LocalResourceLoaders._Closure___2_0(null);
                        if (Loader.IsCanceled)
                        {
                            return;
                        }
                        CS_locals1._VB_Local_ModEntry = new LocalResourceFile(text4);
                        LocalResourceFile localResourceFile = list2.FirstOrDefault<LocalResourceFile>(new Func<LocalResourceFile, bool>(CS_locals1._Lambda___4));
                        if (localResourceFile != null)
                        {
                            LocalResourceFile localResourceFile2 = (localResourceFile.IsEnabled ? CS_locals1._VB_Local_ModEntry : localResourceFile);
                            Logger.Warn(string.Format("重复的 Mod 文件：{0} 与 {1}，已忽略 {2}", localResourceFile.File.Name, CS_locals1._VB_Local_ModEntry.File.Name, localResourceFile2.File.Name), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                            if (localResourceFile2 == CS_locals1._VB_Local_ModEntry)
                            {
                                continue;
                            }
                            list2.Remove(localResourceFile2);
                        }
                        list2.Add(CS_locals1._VB_Local_ModEntry);
                    }
                }
                finally
                {
                    List<string>.Enumerator enumerator2 = default;
                    ((IDisposable)enumerator2).Dispose();
                }
                Logger.Info(string.Format("共发现 {0} 个 Mod", list2.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                list2 = list2.OrderBy<LocalResourceFile, string>((LocalResourceLoaders._Closure___._I2_5 == null) ? (LocalResourceLoaders._Closure___._I2_5 = new Func<LocalResourceFile, string>(LocalResourceLoaders._Closure___._I._Lambda___2_5)) : LocalResourceLoaders._Closure___._I2_5).ToList<LocalResourceFile>();
                if (!Loader.IsCanceled)
                {
                    Loader.Output = list2;
                    LocalResourceLoaders.LocalResourceOnlineLoader.Start(new ValueTuple<List<LocalResourceFile>, JObject, McVersion>(list2, jobject, PageInstanceLeft.Instance.Version), true);
                }
            }
            catch (Exception ex2)
            {
                Logger.Warn(ex2, "Mod 列表加载失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                throw;
            }
        }

        private static void LocalResourceOnlineLoad(ModLoader.LoaderTask<ValueTuple<List<LocalResourceFile>, JObject, McVersion>, int> Loader)
        {
            LocalResourceLoaders._Closure___4_0 CS_locals1 = new LocalResourceLoaders._Closure___4_0(null);
            CS_locals1._VB_Local_Loader = Loader;
            CS_locals1._VB_Local_Mods = new List<LocalResourceFile>();
            JObject item = CS_locals1._VB_Local_Loader.Input.Item2;
            ModLoaders modLoaders = CS_locals1._VB_Local_Loader.Input.Item3.ModLoaders;
            try
            {
                foreach (LocalResourceFile localResourceFile in CS_locals1._VB_Local_Loader.Input.Item1)
                {
                    if (CS_locals1._VB_Local_Loader.IsCanceled)
                    {
                        return;
                    }
                    string text = localResourceFile.ModrinthHash + CS_locals1._VB_Local_Loader.Input.Item3.VanillaName + Conversions.ToString((int)modLoaders);
                    if (item.ContainsKey(text))
                    {
                        localResourceFile.FromJson((JObject)item[text]);
                        if (localResourceFile.OnlineDataLoaded && DateTime.Now - item[text]["Project"]["CacheTime"].ToObject<DateTime>() < new TimeSpan(6, 0, 0))
                        {
                            continue;
                        }
                    }
                    CS_locals1._VB_Local_Mods.Add(localResourceFile);
                }
            }
            finally
            {
                List<LocalResourceFile>.Enumerator enumerator = default;
                ((IDisposable)enumerator).Dispose();
            }
            Logger.Info(string.Format("有 {0} 个 Mod 需要联网获取信息，{1} 个 Mod 需要更新信息", CS_locals1._VB_Local_Mods.Where<LocalResourceFile>((LocalResourceLoaders._Closure___._I4_0 == null) ? (LocalResourceLoaders._Closure___._I4_0 = new Func<LocalResourceFile, bool>(LocalResourceLoaders._Closure___._I._Lambda___4_0)) : LocalResourceLoaders._Closure___._I4_0).Count<LocalResourceFile>(), CS_locals1._VB_Local_Mods.Where<LocalResourceFile>((LocalResourceLoaders._Closure___._I4_1 == null) ? (LocalResourceLoaders._Closure___._I4_1 = new Func<LocalResourceFile, bool>(LocalResourceLoaders._Closure___._I._Lambda___4_1)) : LocalResourceLoaders._Closure___._I4_1).Count<LocalResourceFile>()), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
            if (CS_locals1._VB_Local_Mods.Any<LocalResourceFile>())
            {
                McVersion item2 = CS_locals1._VB_Local_Loader.Input.Item3;
                CS_locals1._VB_Local_VanillaVersion = item2.VanillaName;
                Logger.Info(string.Format("正在检查 Mod 信息的版本：{0}，加载器 {1}", CS_locals1._VB_Local_VanillaVersion, modLoaders), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                CS_locals1._VB_Local_EndedThreadCount = 0;
                CS_locals1._VB_Local_IsFailed = false;
                CS_locals1._VB_Local_CurrentTaskThread = Thread.CurrentThread;
                ModBase.RunInNewThread(new Action(CS_locals1._Lambda___2), "Mod List Detail Loader Modrinth", ThreadPriority.Normal);
                ModBase.RunInNewThread(new Action(CS_locals1._Lambda___11), "Mod List Detail Loader CurseForge", ThreadPriority.Normal);
                while (CS_locals1._VB_Local_EndedThreadCount != 2)
                {
                    Thread.Sleep(10);
                    if (CS_locals1._VB_Local_Loader.IsCanceled)
                    {
                        return;
                    }
                }
                CS_locals1._VB_Local_Mods = CS_locals1._VB_Local_Mods.Where<LocalResourceFile>((LocalResourceLoaders._Closure___._I4_16 == null) ? (LocalResourceLoaders._Closure___._I4_16 = new Func<LocalResourceFile, bool>(LocalResourceLoaders._Closure___._I._Lambda___4_16)) : LocalResourceLoaders._Closure___._I4_16).ToList<LocalResourceFile>();
                Logger.Info(string.Format("联网获取本地 Mod 信息完成，为 {0} 个 Mod 更新缓存", CS_locals1._VB_Local_Mods.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                if (CS_locals1._VB_Local_Mods.Any<LocalResourceFile>() && !CS_locals1._VB_Local_Loader.IsCanceled)
                {
                    try
                    {
                        foreach (LocalResourceFile localResourceFile2 in CS_locals1._VB_Local_Mods)
                        {
                            localResourceFile2.OnlineDataLoaded = !CS_locals1._VB_Local_IsFailed;
                            item[localResourceFile2.ModrinthHash + CS_locals1._VB_Local_VanillaVersion + Conversions.ToString((int)modLoaders)] = localResourceFile2.ToJson();
                        }
                    }
                    finally
                    {
                        List<LocalResourceFile>.Enumerator enumerator2 = default;
                        ((IDisposable)enumerator2).Dispose();
                    }
                    FileUtils.Write(ModBase.PathTemp + "Cache\\LocalMod.json", item.ToString(ModBase.ModeDebug ? Formatting.Indented : Formatting.None, new JsonConverter[0]), null);
                    if (!CS_locals1._VB_Local_Loader.IsCanceled)
                    {
                        PageInstanceMod frmInstanceMod = ModMain.FrmInstanceMod;
                        if (frmInstanceMod != null && frmInstanceMod.Filter == PageInstanceMod.FilterType.CanUpdate)
                        {
                            ModBase.RunInUi((LocalResourceLoaders._Closure___._I4_17 == null) ? (LocalResourceLoaders._Closure___._I4_17 = new Action(LocalResourceLoaders._Closure___._I._Lambda___4_17)) : LocalResourceLoaders._Closure___._I4_17, false);
                            return;
                        }
                        ModBase.RunInUi((LocalResourceLoaders._Closure___._I4_18 == null) ? (LocalResourceLoaders._Closure___._I4_18 = new Action(LocalResourceLoaders._Closure___._I._Lambda___4_18)) : LocalResourceLoaders._Closure___._I4_18, false);
                    }
                }
            }
        }

        public static ModLoader.LoaderTask<string, List<LocalResourceFile>> LocalResourceLoader = new ModLoader.LoaderTask<string, List<LocalResourceFile>>("LocalResourceLoader", new Action<ModLoader.LoaderTask<string, List<LocalResourceFile>>>(LocalResourceLoaders.LocalResourceFileLoad), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<ValueTuple<List<LocalResourceFile>, JObject, McVersion>, int> LocalResourceOnlineLoader = new ModLoader.LoaderTask<ValueTuple<List<LocalResourceFile>, JObject, McVersion>, int>("LocalResourceOnlineLoader", new Action<ModLoader.LoaderTask<ValueTuple<List<LocalResourceFile>, JObject, McVersion>, int>>(LocalResourceLoaders.LocalResourceOnlineLoad), null, ThreadPriority.Normal);

        private static Func<string, bool> _STATIC_LocalResourceFileLoad_011151286FC2E1512816911280C8_IsModFile;

        private static StaticLocalInitFlag _STATIC_LocalResourceFileLoad_011151286FC2E1512816911280C8_IsModFile_Init;

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

            internal void _Lambda___2_0()
            {
                if (ModMain.FrmInstanceMod != null)
                {
                    ModMain.FrmInstanceMod.Load.ShowProgress = false;
                }
            }

            internal void _Lambda___2_1()
            {
                if (ModMain.FrmInstanceMod != null)
                {
                    ModMain.FrmInstanceMod.Load.Text = "正在更新 Mod";
                }
            }

            internal void _Lambda___2_2()
            {
                if (ModMain.FrmInstanceMod != null)
                {
                    ModMain.FrmInstanceMod.Load.Text = "正在加载 Mod 列表";
                }
            }

            internal bool _Lambda___2_3(string FullName)
            {
                bool flag;
                if (FullName == null)
                {
                    flag = false;
                }
                else
                {
                    if (new string[] { "disabled", "old" }.Contains(PathUtils.GetExtension(FullName)))
                    {
                        FullName = PathUtils.GetFileNameWithoutExtension(FullName);
                    }
                    flag = new string[] { "jar", "zip", "litemod" }.Contains(PathUtils.GetExtension(FullName));
                }
                return flag;
            }

            internal string _Lambda___2_5(LocalResourceFile m)
            {
                return m.File.Name;
            }

            internal bool _Lambda___4_0(LocalResourceFile m)
            {
                return ((object)(m.Project) == (object)null);
            }

            internal bool _Lambda___4_1(LocalResourceFile m)
            {
                return ((object)(m.Project) != (object)null);
            }

            internal string _Lambda___4_3(LocalResourceFile m)
            {
                return m.ModrinthHash;
            }

            internal string _Lambda___4_4(JToken l)
            {
                return l.ToString().Lower();
            }

            internal string _Lambda___4_5(string l)
            {
                return l;
            }

            internal IEnumerable<LocalResourceFile> _Lambda___4_6(KeyValuePair<string, List<LocalResourceFile>> l)
            {
                return l.Value;
            }

            internal string _Lambda___4_8(JToken l)
            {
                return l.ToString().Lower();
            }

            internal string _Lambda___4_9(string l)
            {
                return l;
            }

            internal string _Lambda___4_13(JToken Entry)
            {
                return Entry["gameVersion"].ToString();
            }

            internal int _Lambda___4_14(string x, string y)
            {
                return McUtils.CompareVersion(x, y);
            }

            internal bool _Lambda___4_16(LocalResourceFile m)
            {
                return ((object)(m.Project) != (object)null);
            }

            internal void _Lambda___4_17()
            {
                PageInstanceMod frmInstanceMod = ModMain.FrmInstanceMod;
                if (frmInstanceMod == null)
                {
                    return;
                }
                frmInstanceMod.RefreshUI();
            }

            internal void _Lambda___4_18()
            {
                PageInstanceMod frmInstanceMod = ModMain.FrmInstanceMod;
                if (frmInstanceMod == null)
                {
                    return;
                }
                frmInstanceMod.RefreshBars();
            }

            public static readonly LocalResourceLoaders._Closure___ _I = new LocalResourceLoaders._Closure___();

            public static Action _I2_0;

            public static Action _I2_1;

            public static Action _I2_2;

            public static Func<string, bool> _I2_3;

            public static Func<LocalResourceFile, string> _I2_5;

            public static Func<LocalResourceFile, bool> _I4_0;

            public static Func<LocalResourceFile, bool> _I4_1;

            public static Func<LocalResourceFile, string> _I4_3;

            public static Func<JToken, string> _I4_4;

            public static Func<string, string> _I4_5;

            public static Func<KeyValuePair<string, List<LocalResourceFile>>, IEnumerable<LocalResourceFile>> _I4_6;

            public static Func<JToken, string> _I4_8;

            public static Func<string, string> _I4_9;

            public static Func<JToken, string> _I4_13;

            public static Comparison<string> _I4_14;

            public static Func<LocalResourceFile, bool> _I4_16;

            public static Action _I4_17;

            public static Action _I4_18;
        }

        [CompilerGenerated]
        private sealed class _Closure___2_0
        {
            public _Closure___2_0(LocalResourceLoaders._Closure___2_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_ModEntry = arg0._VB_Local_ModEntry;
                }
            }

            internal bool _Lambda___4(LocalResourceFile m)
            {
                return Operators.CompareString(m.EnabledName, this._VB_Local_ModEntry.EnabledName, false) == 0;
            }

            public LocalResourceFile _VB_Local_ModEntry;
        }

        [CompilerGenerated]
        private sealed class _Closure___4_0
        {
            public _Closure___4_0(LocalResourceLoaders._Closure___4_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Mods = arg0._VB_Local_Mods;
                    this._VB_Local_Loader = arg0._VB_Local_Loader;
                    this._VB_Local_CurrentTaskThread = arg0._VB_Local_CurrentTaskThread;
                    this._VB_Local_VanillaVersion = arg0._VB_Local_VanillaVersion;
                    this._VB_Local_IsFailed = arg0._VB_Local_IsFailed;
                    this._VB_Local_EndedThreadCount = arg0._VB_Local_EndedThreadCount;
                }
            }

            internal void _Lambda___2()
            {
                checked
                {
                    try
                    {
                        LocalResourceLoaders._Closure___4_1 CS_locals1 = new LocalResourceLoaders._Closure___4_1(null);
                        CS_locals1._VB_NonLocal__VB_Closure_2 = this;
                        List<string> list = this._VB_Local_Mods.Select<LocalResourceFile, string>((LocalResourceLoaders._Closure___._I4_3 == null) ? (LocalResourceLoaders._Closure___._I4_3 = new Func<LocalResourceFile, string>(LocalResourceLoaders._Closure___._I._Lambda___4_3)) : LocalResourceLoaders._Closure___._I4_3).ToList<string>();
                        JObject jobject = (JObject)ModDownload.DlModRequest("https://api.modrinth.com/v2/version_files", HttpMethod.Post, string.Format("{{\"hashes\": [\"{0}\"], \"algorithm\": \"sha1\"}}", list.Join("\",\"")), "application/json");
                        Logger.Info(string.Format("从 Modrinth 获取到 {0} 个本地 Mod 的对应信息", jobject.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                        if (jobject.Count != 0)
                        {
                            Dictionary<string, List<LocalResourceFile>> dictionary = new Dictionary<string, List<LocalResourceFile>>();
                            CS_locals1._VB_Local_ModrinthLoaders = new Dictionary<LocalResourceFile, List<string>>();
                            try
                            {
                                foreach (LocalResourceFile localResourceFile in this._VB_Local_Mods)
                                {
                                    if (jobject.ContainsKey(localResourceFile.ModrinthHash) && !((string)jobject[localResourceFile.ModrinthHash]["files"][0]["hashes"]["sha1"] != localResourceFile.ModrinthHash))
                                    {
                                        string text = jobject[localResourceFile.ModrinthHash]["project_id"].ToString();
                                        if (ResourceProject.Cache.ContainsKey(text) && ((object)(localResourceFile.Project) == (object)null))
                                        {
                                            localResourceFile.Project = ResourceProject.Cache[text];
                                        }
                                        if (!dictionary.ContainsKey(text))
                                        {
                                            dictionary[text] = new List<LocalResourceFile>();
                                        }
                                        dictionary[text].Add(localResourceFile);
                                        ResourceVersion resourceVersion = ResourceVersion.FromPlatformJson((JObject)jobject[localResourceFile.ModrinthHash], ResourceTypes.Mod);
                                        CS_locals1._VB_Local_ModrinthLoaders[localResourceFile] = jobject[localResourceFile.ModrinthHash]["loaders"].Select<JToken, string>((LocalResourceLoaders._Closure___._I4_4 == null) ? (LocalResourceLoaders._Closure___._I4_4 = new Func<JToken, string>(LocalResourceLoaders._Closure___._I._Lambda___4_4)) : LocalResourceLoaders._Closure___._I4_4).Distinct<string>().OrderBy<string, string>((LocalResourceLoaders._Closure___._I4_5 == null) ? (LocalResourceLoaders._Closure___._I4_5 = new Func<string, string>(LocalResourceLoaders._Closure___._I._Lambda___4_5)) : LocalResourceLoaders._Closure___._I4_5)
                                            .ToList<string>();
                                        if (localResourceFile.ProjectVersion == null || DateTime.Compare(localResourceFile.ProjectVersion.ReleaseDate, resourceVersion.ReleaseDate) < 0)
                                        {
                                            localResourceFile.ProjectVersion = resourceVersion;
                                        }
                                        else
                                        {
                                            localResourceFile.ProjectVersion.Version = resourceVersion.Version;
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                List<LocalResourceFile>.Enumerator enumerator = default;
                                ((IDisposable)enumerator).Dispose();
                            }
                            if (!this._VB_Local_Loader.IsCanceledWithThread(this._VB_Local_CurrentTaskThread))
                            {
                                Logger.Info(string.Format("需要从 Modrinth 获取 {0} 个本地 Mod 的工程信息", dictionary.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                                if (dictionary.Any<KeyValuePair<string, List<LocalResourceFile>>>())
                                {
                                    JArray jarray = (JArray)ModDownload.DlModRequest(string.Format("https://api.modrinth.com/v2/projects?ids=[\"{0}\"]", dictionary.Keys.Join("\",\"")), null, null, null);
                                    try
                                    {
                                        foreach (JToken jtoken in jarray)
                                        {
                                            ResourceProject resourceProject = new ResourceProject((JObject)jtoken);
                                            try
                                            {
                                                foreach (LocalResourceFile localResourceFile2 in dictionary[resourceProject.Id])
                                                {
                                                    localResourceFile2.Project = resourceProject;
                                                }
                                            }
                                            finally
                                            {
                                                List<LocalResourceFile>.Enumerator enumerator3 = default;
                                                ((IDisposable)enumerator3).Dispose();
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        IEnumerator<JToken> enumerator2 = default;
                                        if (enumerator2 != null)
                                        {
                                            enumerator2.Dispose();
                                        }
                                    }
                                    Logger.Info("已从 Modrinth 获取本地 Mod 信息，继续获取更新信息", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                                    List<LocalResourceFile> list2 = dictionary.SelectMany<KeyValuePair<string, List<LocalResourceFile>>, LocalResourceFile>((LocalResourceLoaders._Closure___._I4_6 == null) ? (LocalResourceLoaders._Closure___._I4_6 = new Func<KeyValuePair<string, List<LocalResourceFile>>, IEnumerable<LocalResourceFile>>(LocalResourceLoaders._Closure___._I._Lambda___4_6)) : LocalResourceLoaders._Closure___._I4_6).ToList<LocalResourceFile>();
                                    if (list2.Any<LocalResourceFile>())
                                    {
                                        string text2 = list2.Select<LocalResourceFile, string>(new Func<LocalResourceFile, string>(CS_locals1._Lambda___7)).Join(",");
                                        JObject jobject2 = (JObject)ModDownload.DlModRequest("https://api.modrinth.com/v2/version_files/update_individual", HttpMethod.Post, string.Format("{{\"algorithm\": \"sha1\", \"hashes\": [{0}]}}", text2), "application/json");
                                        try
                                        {
                                            List<LocalResourceFile>.Enumerator enumerator4 = this._VB_Local_Mods.GetEnumerator();
                                            while (enumerator4.MoveNext())
                                            {
                                                LocalResourceLoaders._Closure___4_2 CS_locals2 = new LocalResourceLoaders._Closure___4_2(null);
                                                CS_locals2._VB_Local_Entry = enumerator4.Current;
                                                LocalResourceLoaders._Closure___4_3 CS_locals3 = new LocalResourceLoaders._Closure___4_3(null);
                                                CS_locals3._VB_NonLocal__VB_Closure_3 = CS_locals2;
                                                if (jobject2.ContainsKey(CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry.ModrinthHash) && CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry.ProjectVersion != null)
                                                {
                                                    CS_locals3._VB_Local_UpdateFile = ResourceVersion.FromPlatformJson((JObject)jobject2[CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry.ModrinthHash], ResourceTypes.Mod);
                                                    if (CS_locals3._VB_Local_UpdateFile.DownloadAvailable && jobject2[CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry.ModrinthHash]["loaders"].Select<JToken, string>((LocalResourceLoaders._Closure___._I4_8 == null) ? (LocalResourceLoaders._Closure___._I4_8 = new Func<JToken, string>(LocalResourceLoaders._Closure___._I._Lambda___4_8)) : LocalResourceLoaders._Closure___._I4_8).Distinct<string>().OrderBy<string, string>((LocalResourceLoaders._Closure___._I4_9 == null) ? (LocalResourceLoaders._Closure___._I4_9 = new Func<string, string>(LocalResourceLoaders._Closure___._I._Lambda___4_9)) : LocalResourceLoaders._Closure___._I4_9)
                                                        .ToList<string>()
                                                        .SequenceEqual<string>(CS_locals1._VB_Local_ModrinthLoaders[CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry]))
                                                    {
                                                        Logger.Trace(new Func<string>(CS_locals3._Lambda___10), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                                                        if (DateTime.Compare(CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry.ProjectVersion.ReleaseDate, CS_locals3._VB_Local_UpdateFile.ReleaseDate) < 0 && Operators.CompareString(CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry.ProjectVersion.Hash, CS_locals3._VB_Local_UpdateFile.Hash, false) != 0)
                                                        {
                                                            if (CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry.UpdateFile != null && Operators.CompareString(CS_locals3._VB_Local_UpdateFile.Hash, CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry.UpdateFile.Hash, false) == 0)
                                                            {
                                                                CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry.ChangelogUrls.Add(string.Format("https://modrinth.com/mod/{0}/changelog?g={1}", jobject2[CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry.ModrinthHash]["project_id"], this._VB_Local_VanillaVersion));
                                                                CS_locals3._VB_Local_UpdateFile.DownloadUrls.AddRange(CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry.UpdateFile.DownloadUrls);
                                                                CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry.UpdateFile = CS_locals3._VB_Local_UpdateFile;
                                                            }
                                                            else if (CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry.UpdateFile == null || DateTime.Compare(CS_locals3._VB_Local_UpdateFile.ReleaseDate, CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry.UpdateFile.ReleaseDate) >= 0)
                                                            {
                                                                CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry.ChangelogUrls = new List<string> { string.Format("https://modrinth.com/mod/{0}/changelog?g={1}", jobject2[CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry.ModrinthHash]["project_id"], this._VB_Local_VanillaVersion) };
                                                                CS_locals3._VB_NonLocal__VB_Closure_3._VB_Local_Entry.UpdateFile = CS_locals3._VB_Local_UpdateFile;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            List<LocalResourceFile>.Enumerator enumerator4 = default;
                                            ((IDisposable)enumerator4).Dispose();
                                        }
                                        Logger.Info("从 Modrinth 获取本地 Mod 信息结束", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "从 Modrinth 获取本地 Mod 信息失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                        this._VB_Local_IsFailed = true;
                    }
                    finally
                    {
                        this._VB_Local_EndedThreadCount++;
                    }
                }
            }

            internal void _Lambda___11()
            {
                checked
                {
                    try
                    {
                        LocalResourceLoaders._Closure___4_4 CS_locals1 = new LocalResourceLoaders._Closure___4_4(null);
                        CS_locals1._VB_NonLocal__VB_Closure_4 = this;
                        List<uint> list = new List<uint>();
                        try
                        {
                            foreach (LocalResourceFile localResourceFile in this._VB_Local_Mods)
                            {
                                list.Add(localResourceFile.CurseForgeHash);
                                if (this._VB_Local_Loader.IsCanceledWithThread(this._VB_Local_CurrentTaskThread))
                                {
                                    return;
                                }
                            }
                        }
                        finally
                        {
                            List<LocalResourceFile>.Enumerator enumerator = default;
                            ((IDisposable)enumerator).Dispose();
                        }
                        JContainer jcontainer = (JContainer)ModDownload.DlModRequest("https://api.curseforge.com/v1/fingerprints/432", HttpMethod.Post, string.Format("{{\"fingerprints\": [{0}]}}", list.Join(",")), "application/json")["data"]["exactMatches"];
                        Logger.Info(string.Format("从 CurseForge 获取到 {0} 个本地 Mod 的对应信息", jcontainer.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                        if (jcontainer.Any<JToken>())
                        {
                            Dictionary<int, List<LocalResourceFile>> dictionary = new Dictionary<int, List<LocalResourceFile>>();
                            CS_locals1._VB_Local_CurseForgeLoaders = new Dictionary<LocalResourceFile, ModLoaders>();
                            try
                            {
                                foreach (JToken jtoken in ((IEnumerable<JToken>)jcontainer))
                                {
                                    string text = jtoken["id"].ToString();
                                    uint num = (uint)jtoken["file"]["fileFingerprint"];
                                    try
                                    {
                                        foreach (LocalResourceFile localResourceFile2 in this._VB_Local_Mods)
                                        {
                                            if (localResourceFile2.CurseForgeHash == num)
                                            {
                                                if (ResourceProject.Cache.ContainsKey(text) && ((object)(localResourceFile2.Project) == (object)null))
                                                {
                                                    localResourceFile2.Project = ResourceProject.Cache[text];
                                                }
                                                dictionary.AddIntoValueCollection(Conversions.ToInteger(text), localResourceFile2);
                                                ResourceVersion resourceVersion = ResourceVersion.FromPlatformJson((JObject)jtoken["file"], ResourceTypes.Mod);
                                                CS_locals1._VB_Local_CurseForgeLoaders[localResourceFile2] = resourceVersion.ModLoaders;
                                                if (localResourceFile2.ProjectVersion == null || DateTime.Compare(localResourceFile2.ProjectVersion.ReleaseDate, resourceVersion.ReleaseDate) < 0)
                                                {
                                                    localResourceFile2.ProjectVersion = resourceVersion;
                                                }
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        List<LocalResourceFile>.Enumerator enumerator3 = default;
                                        ((IDisposable)enumerator3).Dispose();
                                    }
                                }
                            }
                            finally
                            {
                                IEnumerator<JToken> enumerator2 = default;
                                if (enumerator2 != null)
                                {
                                    enumerator2.Dispose();
                                }
                            }
                            if (!this._VB_Local_Loader.IsCanceledWithThread(this._VB_Local_CurrentTaskThread))
                            {
                                Logger.Info(string.Format("需要从 CurseForge 获取 {0} 个本地 Mod 的工程信息", dictionary.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                                if (dictionary.Any<KeyValuePair<int, List<LocalResourceFile>>>())
                                {
                                    JToken jtoken2 = ModDownload.DlModRequest("https://api.curseforge.com/v1/mods", HttpMethod.Post, string.Format("{{\"modIds\": [{0}]}}", dictionary.Keys.Join(",")), "application/json")["data"];
                                    Dictionary<int, List<LocalResourceFile>> dictionary2 = new Dictionary<int, List<LocalResourceFile>>();
                                    Dictionary<int, string> dictionary3 = new Dictionary<int, string>();
                                    try
                                    {
                                        foreach (JToken jtoken3 in ((IEnumerable<JToken>)jtoken2))
                                        {
                                            if (jtoken3["isAvailable"] == null || jtoken3["isAvailable"].ToObject<bool>())
                                            {
                                                ResourceProject resourceProject = new ResourceProject((JObject)jtoken3);
                                                try
                                                {
                                                    List<LocalResourceFile>.Enumerator enumerator5 = dictionary[Conversions.ToInteger(resourceProject.Id)].GetEnumerator();
                                                    while (enumerator5.MoveNext())
                                                    {
                                                        LocalResourceLoaders._Closure___4_5 CS_locals2 = new LocalResourceLoaders._Closure___4_5(null);
                                                        CS_locals2._VB_NonLocal__VB_Closure_5 = CS_locals1;
                                                        CS_locals2._VB_Local_LocalFile = enumerator5.Current;
                                                        ResourceProject project = CS_locals2._VB_Local_LocalFile.Project;
                                                        if (((object)project != (object)null) && project.Platform == ResourcePlatforms.Modrinth)
                                                        {
                                                            CS_locals2._VB_Local_LocalFile.Project = CS_locals2._VB_Local_LocalFile.Project;
                                                        }
                                                        else
                                                        {
                                                            CS_locals2._VB_Local_LocalFile.Project = resourceProject;
                                                        }
                                                        List<JToken> list2 = jtoken3["latestFilesIndexes"].Where<JToken>(new Func<JToken, bool>(CS_locals2._Lambda___12)).MaxByAll<JToken, string>((LocalResourceLoaders._Closure___._I4_13 == null) ? (LocalResourceLoaders._Closure___._I4_13 = new Func<JToken, string>(LocalResourceLoaders._Closure___._I._Lambda___4_13)) : LocalResourceLoaders._Closure___._I4_13, Comparer<string>.Create((LocalResourceLoaders._Closure___._I4_14 == null) ? (LocalResourceLoaders._Closure___._I4_14 = new Comparison<string>(LocalResourceLoaders._Closure___._I._Lambda___4_14)) : LocalResourceLoaders._Closure___._I4_14));
                                                        try
                                                        {
                                                            foreach (JToken jtoken4 in list2)
                                                            {
                                                                int num2 = jtoken4["fileId"].ToObject<int>();
                                                                if (!dictionary2.ContainsKey(num2))
                                                                {
                                                                    dictionary2[num2] = new List<LocalResourceFile>();
                                                                }
                                                                dictionary2[num2].Add(CS_locals2._VB_Local_LocalFile);
                                                                dictionary3[num2] = resourceProject.Slug;
                                                            }
                                                        }
                                                        finally
                                                        {
                                                            List<JToken>.Enumerator enumerator6 = default;
                                                            ((IDisposable)enumerator6).Dispose();
                                                        }
                                                    }
                                                }
                                                finally
                                                {
                                                    List<LocalResourceFile>.Enumerator enumerator5 = default;
                                                    ((IDisposable)enumerator5).Dispose();
                                                }
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        IEnumerator<JToken> enumerator4 = default;
                                        if (enumerator4 != null)
                                        {
                                            enumerator4.Dispose();
                                        }
                                    }
                                    Logger.Info(string.Format("已从 CurseForge 获取本地 Mod 信息，需要获取 {0} 个用于检查更新的文件信息", dictionary2.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                                    if (dictionary2.Any<KeyValuePair<int, List<LocalResourceFile>>>())
                                    {
                                        JToken jtoken5 = ModDownload.DlModRequest("https://api.curseforge.com/v1/mods/files", HttpMethod.Post, string.Format("{{\"fileIds\": [{0}]}}", dictionary2.Keys.Join(",")), "application/json")["data"];
                                        Dictionary<LocalResourceFile, ResourceVersion> dictionary4 = new Dictionary<LocalResourceFile, ResourceVersion>();
                                        try
                                        {
                                            foreach (JToken jtoken6 in ((IEnumerable<JToken>)jtoken5))
                                            {
                                                ResourceVersion resourceVersion2 = ResourceVersion.FromPlatformJson((JObject)jtoken6, ResourceTypes.Mod);
                                                if (resourceVersion2.DownloadAvailable)
                                                {
                                                    try
                                                    {
                                                        foreach (LocalResourceFile localResourceFile3 in dictionary2[Conversions.ToInteger(resourceVersion2.Id)])
                                                        {
                                                            if (resourceVersion2.ModLoaders != CS_locals1._VB_Local_CurseForgeLoaders[localResourceFile3])
                                                            {
                                                                Logger.Warn(string.Format("从 CurseForge 获取到的更新文件 {0} 加载器为 {1}，与本地文件 {2} 的加载器 {3} 不一致，已忽略", new object[]
                                                                {
                                                                    resourceVersion2.FileName,
                                                                    resourceVersion2.ModLoaders,
                                                                    localResourceFile3.File.Name,
                                                                    CS_locals1._VB_Local_CurseForgeLoaders[localResourceFile3]
                                                                }), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                                                            }
                                                            else if (!dictionary4.ContainsKey(localResourceFile3) || DateTime.Compare(dictionary4[localResourceFile3].ReleaseDate, resourceVersion2.ReleaseDate) < 0)
                                                            {
                                                                dictionary4[localResourceFile3] = resourceVersion2;
                                                            }
                                                        }
                                                    }
                                                    finally
                                                    {
                                                        List<LocalResourceFile>.Enumerator enumerator8 = default;
                                                        ((IDisposable)enumerator8).Dispose();
                                                    }
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            IEnumerator<JToken> enumerator7 = default;
                                            if (enumerator7 != null)
                                            {
                                                enumerator7.Dispose();
                                            }
                                        }
                                        try
                                        {
                                            foreach (KeyValuePair<LocalResourceFile, ResourceVersion> keyValuePair in dictionary4)
                                            {
                                                LocalResourceLoaders._Closure___4_6 CS_locals3 = new LocalResourceLoaders._Closure___4_6(null);
                                                CS_locals3._VB_Local_Entry = keyValuePair.Key;
                                                CS_locals3._VB_Local_UpdateFile = keyValuePair.Value;
                                                Logger.Trace(new Func<string>(CS_locals3._Lambda___15), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                                                if (DateTime.Compare(CS_locals3._VB_Local_Entry.ProjectVersion.ReleaseDate, CS_locals3._VB_Local_UpdateFile.ReleaseDate) < 0 && Operators.CompareString(CS_locals3._VB_Local_Entry.ProjectVersion.Hash, CS_locals3._VB_Local_UpdateFile.Hash, false) != 0)
                                                {
                                                    if (CS_locals3._VB_Local_Entry.UpdateFile != null && Operators.CompareString(CS_locals3._VB_Local_UpdateFile.Hash, CS_locals3._VB_Local_Entry.UpdateFile.Hash, false) == 0)
                                                    {
                                                        CS_locals3._VB_Local_Entry.ChangelogUrls.Add(string.Format("https://www.curseforge.com/minecraft/mc-mods/{0}/files/{1}", dictionary3[Conversions.ToInteger(CS_locals3._VB_Local_UpdateFile.Id)], CS_locals3._VB_Local_UpdateFile.Id));
                                                        CS_locals3._VB_Local_Entry.UpdateFile.DownloadUrls.AddRange(CS_locals3._VB_Local_UpdateFile.DownloadUrls);
                                                    }
                                                    else if (CS_locals3._VB_Local_Entry.UpdateFile == null || DateTime.Compare(CS_locals3._VB_Local_UpdateFile.ReleaseDate, CS_locals3._VB_Local_Entry.UpdateFile.ReleaseDate) > 0)
                                                    {
                                                        CS_locals3._VB_Local_Entry.ChangelogUrls = new List<string> { string.Format("https://www.curseforge.com/minecraft/mc-mods/{0}/files/{1}", dictionary3[Conversions.ToInteger(CS_locals3._VB_Local_UpdateFile.Id)], CS_locals3._VB_Local_UpdateFile.Id) };
                                                        CS_locals3._VB_Local_Entry.UpdateFile = CS_locals3._VB_Local_UpdateFile;
                                                    }
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            Dictionary<LocalResourceFile, ResourceVersion>.Enumerator enumerator9 = default;
                                            ((IDisposable)enumerator9).Dispose();
                                        }
                                        Logger.Info("从 CurseForge 获取 Mod 更新信息结束", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "从 CurseForge 获取本地 Mod 信息失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                        this._VB_Local_IsFailed = true;
                    }
                    finally
                    {
                        this._VB_Local_EndedThreadCount++;
                    }
                }
            }

            public List<LocalResourceFile> _VB_Local_Mods;

            public ModLoader.LoaderTask<ValueTuple<List<LocalResourceFile>, JObject, McVersion>, int> _VB_Local_Loader;

            public Thread _VB_Local_CurrentTaskThread;

            public string _VB_Local_VanillaVersion;

            public bool _VB_Local_IsFailed;

            public int _VB_Local_EndedThreadCount;
        }

        [CompilerGenerated]
        private sealed class _Closure___4_1
        {
            public _Closure___4_1(LocalResourceLoaders._Closure___4_1 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_ModrinthLoaders = arg0._VB_Local_ModrinthLoaders;
                }
            }

            internal string _Lambda___7(LocalResourceFile m)
            {
                return string.Format("{{\"hash\": \"{0}\", \"loaders\": [\"{1}\"], \"game_versions\": [\"{2}\"]}}", m.ModrinthHash, this._VB_Local_ModrinthLoaders[m].Join("\",\""), this._VB_NonLocal__VB_Closure_2._VB_Local_VanillaVersion);
            }

            public Dictionary<LocalResourceFile, List<string>> _VB_Local_ModrinthLoaders;

            public LocalResourceLoaders._Closure___4_0 _VB_NonLocal__VB_Closure_2;
        }

        [CompilerGenerated]
        private sealed class _Closure___4_2
        {
            public _Closure___4_2(LocalResourceLoaders._Closure___4_2 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Entry = arg0._VB_Local_Entry;
                }
            }

            public LocalResourceFile _VB_Local_Entry;
        }

        [CompilerGenerated]
        private sealed class _Closure___4_3
        {
            public _Closure___4_3(LocalResourceLoaders._Closure___4_3 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_UpdateFile = arg0._VB_Local_UpdateFile;
                }
            }

            internal string _Lambda___10()
            {
                return string.Format("本地文件 {0} 在 Modrinth 上的最新版为 {1}", this._VB_NonLocal__VB_Closure_3._VB_Local_Entry.ProjectVersion.FileName, this._VB_Local_UpdateFile.FileName);
            }

            public ResourceVersion _VB_Local_UpdateFile;

            public LocalResourceLoaders._Closure___4_2 _VB_NonLocal__VB_Closure_3;
        }

        [CompilerGenerated]
        private sealed class _Closure___4_4
        {
            public _Closure___4_4(LocalResourceLoaders._Closure___4_4 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_CurseForgeLoaders = arg0._VB_Local_CurseForgeLoaders;
                }
            }

            public Dictionary<LocalResourceFile, ModLoaders> _VB_Local_CurseForgeLoaders;

            public LocalResourceLoaders._Closure___4_0 _VB_NonLocal__VB_Closure_4;
        }

        [CompilerGenerated]
        private sealed class _Closure___4_5
        {
            public _Closure___4_5(LocalResourceLoaders._Closure___4_5 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_LocalFile = arg0._VB_Local_LocalFile;
                }
            }

            internal bool _Lambda___12(JToken Entry)
            {
                ModLoaders modLoaders = ((Entry["modLoader"] == null) ? ModLoaders.None : Resource.FromCurseForgeModLoaderType(Entry["modLoader"].ToObject<int>()));
                if (this._VB_NonLocal__VB_Closure_5._VB_Local_CurseForgeLoaders[this._VB_Local_LocalFile] == ModLoaders.None)
                {
                    if (modLoaders != ModLoaders.None)
                    {
                        return false;
                    }
                }
                else if (!this._VB_NonLocal__VB_Closure_5._VB_Local_CurseForgeLoaders[this._VB_Local_LocalFile].HasFlagF(modLoaders))
                {
                    return false;
                }
                string text = (string)Entry["gameVersion"];
                return text != null && Operators.CompareString(text, this._VB_NonLocal__VB_Closure_5._VB_NonLocal__VB_Closure_4._VB_Local_VanillaVersion, false) == 0;
            }

            public LocalResourceFile _VB_Local_LocalFile;

            public LocalResourceLoaders._Closure___4_4 _VB_NonLocal__VB_Closure_5;
        }

        [CompilerGenerated]
        private sealed class _Closure___4_6
        {
            public _Closure___4_6(LocalResourceLoaders._Closure___4_6 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Entry = arg0._VB_Local_Entry;
                    this._VB_Local_UpdateFile = arg0._VB_Local_UpdateFile;
                }
            }

            internal string _Lambda___15()
            {
                return string.Format("本地文件 {0} 在 CurseForge 上的最新版为 {1}", this._VB_Local_Entry.ProjectVersion.FileName, this._VB_Local_UpdateFile.FileName);
            }

            public LocalResourceFile _VB_Local_Entry;

            public ResourceVersion _VB_Local_UpdateFile;
        }
    }
}
