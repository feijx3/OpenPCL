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
    public sealed class LocalResourceLoaders
    {
        private static void LocalResourceFileLoad(ModLoader.LoaderTask<string, List<LocalResourceFile>> Loader)
        {
            try
            {
                ModBase.RunInUiWait((LocalResourceLoaders._Closure$__.$I2-0 == null) ? (LocalResourceLoaders._Closure$__.$I2-0 = delegate
                {
                    if (ModMain.FrmInstanceMod != null)
                    {
                        ModMain.FrmInstanceMod.Load.ShowProgress = false;
                    }
                }) : LocalResourceLoaders._Closure$__.$I2-0);
                if (PageInstanceMod.UpdatingInstanceModFolders.Contains(Loader.Input))
                {
                    Logger.Info(string.Format("等待 Mod 更新完成后才能继续加载 Mod 列表：{0}", Loader.Input), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                    try
                    {
                        ModBase.RunInUiWait((LocalResourceLoaders._Closure$__.$I2-1 == null) ? (LocalResourceLoaders._Closure$__.$I2-1 = delegate
                        {
                            if (ModMain.FrmInstanceMod != null)
                            {
                                ModMain.FrmInstanceMod.Load.Text = "正在更新 Mod";
                            }
                        }) : LocalResourceLoaders._Closure$__.$I2-1);
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
                        ModBase.RunInUiWait((LocalResourceLoaders._Closure$__.$I2-2 == null) ? (LocalResourceLoaders._Closure$__.$I2-2 = delegate
                        {
                            if (ModMain.FrmInstanceMod != null)
                            {
                                ModMain.FrmInstanceMod.Load.Text = "正在加载 Mod 列表";
                            }
                        }) : LocalResourceLoaders._Closure$__.$I2-2);
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
                            if (PathUtils.IsSame(PathUtils.RemoveLastPart(text), Loader.Input) || (PageInstanceLeft.Instance != null && PageInstanceLeft.Instance.Version.ModLoaders.HasFlagF(ModLoaders.Forge) && PageInstanceLeft.Instance.Version.Vanilla.Major < 13 && Operators.CompareString(PathUtils.GetLastPart(PathUtils.RemoveLastPart(text)), string.Format("1.{0}.{1}", PageInstanceLeft.Instance.Version.Vanilla.Major, PageInstanceLeft.Instance.Version.Vanilla.Build), false) == 0))
                            {
                                if (LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init == null)
                                {
                                    Interlocked.CompareExchange<StaticLocalInitFlag>(ref LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init, new StaticLocalInitFlag(), null);
                                }
                                bool flag = false;
                                try
                                {
                                    Monitor.Enter(LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init, ref flag);
                                    if (LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init.State == 0)
                                    {
                                        LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init.State = 2;
                                        LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile = ((LocalResourceLoaders._Closure$__.$I2-3 == null) ? (LocalResourceLoaders._Closure$__.$I2-3 = delegate(string FullName)
                                        {
                                            bool flag2;
                                            if (FullName == null)
                                            {
                                                flag2 = false;
                                            }
                                            else
                                            {
                                                if (new string[] { "disabled", "old" }.Contains(PathUtils.GetExtension(FullName)))
                                                {
                                                    FullName = PathUtils.GetFileNameWithoutExtension(FullName);
                                                }
                                                flag2 = new string[] { "jar", "zip", "litemod" }.Contains(PathUtils.GetExtension(FullName));
                                            }
                                            return flag2;
                                        }) : LocalResourceLoaders._Closure$__.$I2-3);
                                    }
                                    else if (LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init.State == 2)
                                    {
                                        throw new IncompleteInitialization();
                                    }
                                }
                                finally
                                {
                                    LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init.State = 1;
                                    if (flag)
                                    {
                                        Monitor.Exit(LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init);
                                    }
                                }
                                if (LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile(text))
                                {
                                    list.Add(text);
                                }
                            }
                        }
                    }
                    finally
                    {
                        IEnumerator<string> enumerator;
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
                        LocalResourceLoaders._Closure$__2-0 CS$<>8__locals1 = new LocalResourceLoaders._Closure$__2-0(CS$<>8__locals1);
                        if (Loader.IsCanceled)
                        {
                            return;
                        }
                        CS$<>8__locals1.$VB$Local_ModEntry = new LocalResourceFile(text4);
                        LocalResourceFile localResourceFile = list2.FirstOrDefault<LocalResourceFile>((LocalResourceFile m) => Operators.CompareString(m.EnabledName, CS$<>8__locals1.$VB$Local_ModEntry.EnabledName, false) == 0);
                        if (localResourceFile != null)
                        {
                            LocalResourceFile localResourceFile2 = (localResourceFile.IsEnabled ? CS$<>8__locals1.$VB$Local_ModEntry : localResourceFile);
                            Logger.Warn(string.Format("重复的 Mod 文件：{0} 与 {1}，已忽略 {2}", localResourceFile.File.Name, CS$<>8__locals1.$VB$Local_ModEntry.File.Name, localResourceFile2.File.Name), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                            if (localResourceFile2 == CS$<>8__locals1.$VB$Local_ModEntry)
                            {
                                continue;
                            }
                            list2.Remove(localResourceFile2);
                        }
                        list2.Add(CS$<>8__locals1.$VB$Local_ModEntry);
                    }
                }
                finally
                {
                    List<string>.Enumerator enumerator2;
                    ((IDisposable)enumerator2).Dispose();
                }
                Logger.Info(string.Format("共发现 {0} 个 Mod", list2.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                list2 = list2.OrderBy<LocalResourceFile, string>((LocalResourceLoaders._Closure$__.$I2-5 == null) ? (LocalResourceLoaders._Closure$__.$I2-5 = (LocalResourceFile m) => m.File.Name) : LocalResourceLoaders._Closure$__.$I2-5).ToList<LocalResourceFile>();
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

        private static void LocalResourceOnlineLoad([TupleElementNames(new string[] { "ModList", "Cache", "Target" })] ModLoader.LoaderTask<ValueTuple<List<LocalResourceFile>, JObject, McVersion>, int> Loader)
        {
            LocalResourceLoaders._Closure$__4-0 CS$<>8__locals1 = new LocalResourceLoaders._Closure$__4-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_Loader = Loader;
            CS$<>8__locals1.$VB$Local_Mods = new List<LocalResourceFile>();
            JObject item = CS$<>8__locals1.$VB$Local_Loader.Input.Item2;
            ModLoaders modLoaders = CS$<>8__locals1.$VB$Local_Loader.Input.Item3.ModLoaders;
            try
            {
                foreach (LocalResourceFile localResourceFile in CS$<>8__locals1.$VB$Local_Loader.Input.Item1)
                {
                    if (CS$<>8__locals1.$VB$Local_Loader.IsCanceled)
                    {
                        return;
                    }
                    string text = localResourceFile.ModrinthHash + CS$<>8__locals1.$VB$Local_Loader.Input.Item3.VanillaName + Conversions.ToString((int)modLoaders);
                    if (item.ContainsKey(text))
                    {
                        localResourceFile.FromJson((JObject)item[text]);
                        if (localResourceFile.OnlineDataLoaded && DateTime.Now - item[text]["Project"]["CacheTime"].ToObject<DateTime>() < new TimeSpan(6, 0, 0))
                        {
                            continue;
                        }
                    }
                    CS$<>8__locals1.$VB$Local_Mods.Add(localResourceFile);
                }
            }
            finally
            {
                List<LocalResourceFile>.Enumerator enumerator;
                ((IDisposable)enumerator).Dispose();
            }
            Logger.Info(string.Format("有 {0} 个 Mod 需要联网获取信息，{1} 个 Mod 需要更新信息", CS$<>8__locals1.$VB$Local_Mods.Where<LocalResourceFile>((LocalResourceLoaders._Closure$__.$I4-0 == null) ? (LocalResourceLoaders._Closure$__.$I4-0 = (LocalResourceFile m) => m.Project == null) : LocalResourceLoaders._Closure$__.$I4-0).Count<LocalResourceFile>(), CS$<>8__locals1.$VB$Local_Mods.Where<LocalResourceFile>((LocalResourceLoaders._Closure$__.$I4-1 == null) ? (LocalResourceLoaders._Closure$__.$I4-1 = (LocalResourceFile m) => m.Project != null) : LocalResourceLoaders._Closure$__.$I4-1).Count<LocalResourceFile>()), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
            checked
            {
                if (CS$<>8__locals1.$VB$Local_Mods.Any<LocalResourceFile>())
                {
                    McVersion item2 = CS$<>8__locals1.$VB$Local_Loader.Input.Item3;
                    CS$<>8__locals1.$VB$Local_VanillaVersion = item2.VanillaName;
                    Logger.Info(string.Format("正在检查 Mod 信息的版本：{0}，加载器 {1}", CS$<>8__locals1.$VB$Local_VanillaVersion, modLoaders), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                    CS$<>8__locals1.$VB$Local_EndedThreadCount = 0;
                    CS$<>8__locals1.$VB$Local_IsFailed = false;
                    CS$<>8__locals1.$VB$Local_CurrentTaskThread = Thread.CurrentThread;
                    ModBase.RunInNewThread(delegate
                    {
                        try
                        {
                            LocalResourceLoaders._Closure$__4-1 CS$<>8__locals2 = new LocalResourceLoaders._Closure$__4-1(CS$<>8__locals2);
                            CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1;
                            List<string> list = CS$<>8__locals1.$VB$Local_Mods.Select<LocalResourceFile, string>((LocalResourceLoaders._Closure$__.$I4-3 == null) ? (LocalResourceLoaders._Closure$__.$I4-3 = (LocalResourceFile m) => m.ModrinthHash) : LocalResourceLoaders._Closure$__.$I4-3).ToList<string>();
                            JObject jobject = (JObject)ModDownload.DlModRequest("https://api.modrinth.com/v2/version_files", HttpMethod.Post, string.Format("{{\"hashes\": [\"{0}\"], \"algorithm\": \"sha1\"}}", list.Join("\",\"")), "application/json");
                            Logger.Info(string.Format("从 Modrinth 获取到 {0} 个本地 Mod 的对应信息", jobject.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                            if (jobject.Count != 0)
                            {
                                Dictionary<string, List<LocalResourceFile>> dictionary = new Dictionary<string, List<LocalResourceFile>>();
                                CS$<>8__locals2.$VB$Local_ModrinthLoaders = new Dictionary<LocalResourceFile, List<string>>();
                                try
                                {
                                    foreach (LocalResourceFile localResourceFile3 in CS$<>8__locals1.$VB$Local_Mods)
                                    {
                                        if (jobject.ContainsKey(localResourceFile3.ModrinthHash) && !((string)jobject[localResourceFile3.ModrinthHash]["files"][0]["hashes"]["sha1"] != localResourceFile3.ModrinthHash))
                                        {
                                            string text2 = jobject[localResourceFile3.ModrinthHash]["project_id"].ToString();
                                            if (ResourceProject.Cache.ContainsKey(text2) && localResourceFile3.Project == null)
                                            {
                                                localResourceFile3.Project = ResourceProject.Cache[text2];
                                            }
                                            if (!dictionary.ContainsKey(text2))
                                            {
                                                dictionary[text2] = new List<LocalResourceFile>();
                                            }
                                            dictionary[text2].Add(localResourceFile3);
                                            ResourceVersion resourceVersion = ResourceVersion.FromPlatformJson((JObject)jobject[localResourceFile3.ModrinthHash], ResourceTypes.Mod);
                                            CS$<>8__locals2.$VB$Local_ModrinthLoaders[localResourceFile3] = jobject[localResourceFile3.ModrinthHash]["loaders"].Select<JToken, string>((LocalResourceLoaders._Closure$__.$I4-4 == null) ? (LocalResourceLoaders._Closure$__.$I4-4 = (JToken l) => l.ToString().Lower()) : LocalResourceLoaders._Closure$__.$I4-4).Distinct<string>().OrderBy<string, string>((LocalResourceLoaders._Closure$__.$I4-5 == null) ? (LocalResourceLoaders._Closure$__.$I4-5 = (string l) => l) : LocalResourceLoaders._Closure$__.$I4-5)
                                                .ToList<string>();
                                            if (localResourceFile3.ProjectVersion == null || DateTime.Compare(localResourceFile3.ProjectVersion.ReleaseDate, resourceVersion.ReleaseDate) < 0)
                                            {
                                                localResourceFile3.ProjectVersion = resourceVersion;
                                            }
                                            else
                                            {
                                                localResourceFile3.ProjectVersion.Version = resourceVersion.Version;
                                            }
                                        }
                                    }
                                }
                                finally
                                {
                                    List<LocalResourceFile>.Enumerator enumerator3;
                                    ((IDisposable)enumerator3).Dispose();
                                }
                                if (!CS$<>8__locals1.$VB$Local_Loader.IsCanceledWithThread(CS$<>8__locals1.$VB$Local_CurrentTaskThread))
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
                                                    foreach (LocalResourceFile localResourceFile4 in dictionary[resourceProject.Id])
                                                    {
                                                        localResourceFile4.Project = resourceProject;
                                                    }
                                                }
                                                finally
                                                {
                                                    List<LocalResourceFile>.Enumerator enumerator5;
                                                    ((IDisposable)enumerator5).Dispose();
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            IEnumerator<JToken> enumerator4;
                                            if (enumerator4 != null)
                                            {
                                                enumerator4.Dispose();
                                            }
                                        }
                                        Logger.Info("已从 Modrinth 获取本地 Mod 信息，继续获取更新信息", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                                        List<LocalResourceFile> list2 = dictionary.SelectMany<KeyValuePair<string, List<LocalResourceFile>>, LocalResourceFile>((LocalResourceLoaders._Closure$__.$I4-6 == null) ? (LocalResourceLoaders._Closure$__.$I4-6 = (KeyValuePair<string, List<LocalResourceFile>> l) => l.Value) : LocalResourceLoaders._Closure$__.$I4-6).ToList<LocalResourceFile>();
                                        if (list2.Any<LocalResourceFile>())
                                        {
                                            string text3 = list2.Select<LocalResourceFile, string>((LocalResourceFile m) => string.Format("{{\"hash\": \"{0}\", \"loaders\": [\"{1}\"], \"game_versions\": [\"{2}\"]}}", m.ModrinthHash, CS$<>8__locals2.$VB$Local_ModrinthLoaders[m].Join("\",\""), CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_VanillaVersion)).Join(",");
                                            JObject jobject2 = (JObject)ModDownload.DlModRequest("https://api.modrinth.com/v2/version_files/update_individual", HttpMethod.Post, string.Format("{{\"algorithm\": \"sha1\", \"hashes\": [{0}]}}", text3), "application/json");
                                            try
                                            {
                                                List<LocalResourceFile>.Enumerator enumerator6 = CS$<>8__locals1.$VB$Local_Mods.GetEnumerator();
                                                while (enumerator6.MoveNext())
                                                {
                                                    LocalResourceLoaders._Closure$__4-2 CS$<>8__locals3 = new LocalResourceLoaders._Closure$__4-2(CS$<>8__locals3);
                                                    CS$<>8__locals3.$VB$Local_Entry = enumerator6.Current;
                                                    LocalResourceLoaders._Closure$__4-3 CS$<>8__locals4 = new LocalResourceLoaders._Closure$__4-3(CS$<>8__locals4);
                                                    CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3 = CS$<>8__locals3;
                                                    if (jobject2.ContainsKey(CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ModrinthHash) && CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ProjectVersion != null)
                                                    {
                                                        CS$<>8__locals4.$VB$Local_UpdateFile = ResourceVersion.FromPlatformJson((JObject)jobject2[CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ModrinthHash], ResourceTypes.Mod);
                                                        if (CS$<>8__locals4.$VB$Local_UpdateFile.DownloadAvailable && jobject2[CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ModrinthHash]["loaders"].Select<JToken, string>((LocalResourceLoaders._Closure$__.$I4-8 == null) ? (LocalResourceLoaders._Closure$__.$I4-8 = (JToken l) => l.ToString().Lower()) : LocalResourceLoaders._Closure$__.$I4-8).Distinct<string>().OrderBy<string, string>((LocalResourceLoaders._Closure$__.$I4-9 == null) ? (LocalResourceLoaders._Closure$__.$I4-9 = (string l) => l) : LocalResourceLoaders._Closure$__.$I4-9)
                                                            .ToList<string>()
                                                            .SequenceEqual<string>(CS$<>8__locals2.$VB$Local_ModrinthLoaders[CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry]))
                                                        {
                                                            Logger.Trace(() => string.Format("本地文件 {0} 在 Modrinth 上的最新版为 {1}", CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ProjectVersion.FileName, CS$<>8__locals4.$VB$Local_UpdateFile.FileName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                                                            if (DateTime.Compare(CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ProjectVersion.ReleaseDate, CS$<>8__locals4.$VB$Local_UpdateFile.ReleaseDate) < 0 && Operators.CompareString(CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ProjectVersion.Hash, CS$<>8__locals4.$VB$Local_UpdateFile.Hash, false) != 0)
                                                            {
                                                                if (CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.UpdateFile != null && Operators.CompareString(CS$<>8__locals4.$VB$Local_UpdateFile.Hash, CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.UpdateFile.Hash, false) == 0)
                                                                {
                                                                    CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ChangelogUrls.Add(string.Format("https://modrinth.com/mod/{0}/changelog?g={1}", jobject2[CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ModrinthHash]["project_id"], CS$<>8__locals1.$VB$Local_VanillaVersion));
                                                                    CS$<>8__locals4.$VB$Local_UpdateFile.DownloadUrls.AddRange(CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.UpdateFile.DownloadUrls);
                                                                    CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.UpdateFile = CS$<>8__locals4.$VB$Local_UpdateFile;
                                                                }
                                                                else if (CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.UpdateFile == null || DateTime.Compare(CS$<>8__locals4.$VB$Local_UpdateFile.ReleaseDate, CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.UpdateFile.ReleaseDate) >= 0)
                                                                {
                                                                    CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ChangelogUrls = new List<string> { string.Format("https://modrinth.com/mod/{0}/changelog?g={1}", jobject2[CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ModrinthHash]["project_id"], CS$<>8__locals1.$VB$Local_VanillaVersion) };
                                                                    CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.UpdateFile = CS$<>8__locals4.$VB$Local_UpdateFile;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            finally
                                            {
                                                List<LocalResourceFile>.Enumerator enumerator6;
                                                ((IDisposable)enumerator6).Dispose();
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
                            CS$<>8__locals1.$VB$Local_IsFailed = true;
                        }
                        finally
                        {
                            CS$<>8__locals1.$VB$Local_EndedThreadCount++;
                        }
                    }, "Mod List Detail Loader Modrinth", ThreadPriority.Normal);
                    ModBase.RunInNewThread(delegate
                    {
                        try
                        {
                            LocalResourceLoaders._Closure$__4-4 CS$<>8__locals5 = new LocalResourceLoaders._Closure$__4-4(CS$<>8__locals5);
                            CS$<>8__locals5.$VB$NonLocal_$VB$Closure_4 = CS$<>8__locals1;
                            List<uint> list3 = new List<uint>();
                            try
                            {
                                foreach (LocalResourceFile localResourceFile5 in CS$<>8__locals1.$VB$Local_Mods)
                                {
                                    list3.Add(localResourceFile5.CurseForgeHash);
                                    if (CS$<>8__locals1.$VB$Local_Loader.IsCanceledWithThread(CS$<>8__locals1.$VB$Local_CurrentTaskThread))
                                    {
                                        return;
                                    }
                                }
                            }
                            finally
                            {
                                List<LocalResourceFile>.Enumerator enumerator7;
                                ((IDisposable)enumerator7).Dispose();
                            }
                            JContainer jcontainer = (JContainer)ModDownload.DlModRequest("https://api.curseforge.com/v1/fingerprints/432", HttpMethod.Post, string.Format("{{\"fingerprints\": [{0}]}}", list3.Join(",")), "application/json")["data"]["exactMatches"];
                            Logger.Info(string.Format("从 CurseForge 获取到 {0} 个本地 Mod 的对应信息", jcontainer.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                            if (jcontainer.Any<JToken>())
                            {
                                Dictionary<int, List<LocalResourceFile>> dictionary2 = new Dictionary<int, List<LocalResourceFile>>();
                                CS$<>8__locals5.$VB$Local_CurseForgeLoaders = new Dictionary<LocalResourceFile, ModLoaders>();
                                try
                                {
                                    foreach (JToken jtoken2 in jcontainer)
                                    {
                                        string text4 = jtoken2["id"].ToString();
                                        uint num = (uint)jtoken2["file"]["fileFingerprint"];
                                        try
                                        {
                                            foreach (LocalResourceFile localResourceFile6 in CS$<>8__locals1.$VB$Local_Mods)
                                            {
                                                if (localResourceFile6.CurseForgeHash == num)
                                                {
                                                    if (ResourceProject.Cache.ContainsKey(text4) && localResourceFile6.Project == null)
                                                    {
                                                        localResourceFile6.Project = ResourceProject.Cache[text4];
                                                    }
                                                    dictionary2.AddIntoValueCollection(Conversions.ToInteger(text4), localResourceFile6);
                                                    ResourceVersion resourceVersion2 = ResourceVersion.FromPlatformJson((JObject)jtoken2["file"], ResourceTypes.Mod);
                                                    CS$<>8__locals5.$VB$Local_CurseForgeLoaders[localResourceFile6] = resourceVersion2.ModLoaders;
                                                    if (localResourceFile6.ProjectVersion == null || DateTime.Compare(localResourceFile6.ProjectVersion.ReleaseDate, resourceVersion2.ReleaseDate) < 0)
                                                    {
                                                        localResourceFile6.ProjectVersion = resourceVersion2;
                                                    }
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            List<LocalResourceFile>.Enumerator enumerator9;
                                            ((IDisposable)enumerator9).Dispose();
                                        }
                                    }
                                }
                                finally
                                {
                                    IEnumerator<JToken> enumerator8;
                                    if (enumerator8 != null)
                                    {
                                        enumerator8.Dispose();
                                    }
                                }
                                if (!CS$<>8__locals1.$VB$Local_Loader.IsCanceledWithThread(CS$<>8__locals1.$VB$Local_CurrentTaskThread))
                                {
                                    Logger.Info(string.Format("需要从 CurseForge 获取 {0} 个本地 Mod 的工程信息", dictionary2.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                                    if (dictionary2.Any<KeyValuePair<int, List<LocalResourceFile>>>())
                                    {
                                        JToken jtoken3 = ModDownload.DlModRequest("https://api.curseforge.com/v1/mods", HttpMethod.Post, string.Format("{{\"modIds\": [{0}]}}", dictionary2.Keys.Join(",")), "application/json")["data"];
                                        Dictionary<int, List<LocalResourceFile>> dictionary3 = new Dictionary<int, List<LocalResourceFile>>();
                                        Dictionary<int, string> dictionary4 = new Dictionary<int, string>();
                                        try
                                        {
                                            foreach (JToken jtoken4 in jtoken3)
                                            {
                                                if (jtoken4["isAvailable"] == null || jtoken4["isAvailable"].ToObject<bool>())
                                                {
                                                    ResourceProject resourceProject2 = new ResourceProject((JObject)jtoken4);
                                                    try
                                                    {
                                                        List<LocalResourceFile>.Enumerator enumerator11 = dictionary2[Conversions.ToInteger(resourceProject2.Id)].GetEnumerator();
                                                        while (enumerator11.MoveNext())
                                                        {
                                                            LocalResourceLoaders._Closure$__4-5 CS$<>8__locals6 = new LocalResourceLoaders._Closure$__4-5(CS$<>8__locals6);
                                                            CS$<>8__locals6.$VB$NonLocal_$VB$Closure_5 = CS$<>8__locals5;
                                                            CS$<>8__locals6.$VB$Local_LocalFile = enumerator11.Current;
                                                            ResourceProject project = CS$<>8__locals6.$VB$Local_LocalFile.Project;
                                                            if (project != null && project.Platform == ResourcePlatforms.Modrinth)
                                                            {
                                                                CS$<>8__locals6.$VB$Local_LocalFile.Project = CS$<>8__locals6.$VB$Local_LocalFile.Project;
                                                            }
                                                            else
                                                            {
                                                                CS$<>8__locals6.$VB$Local_LocalFile.Project = resourceProject2;
                                                            }
                                                            List<JToken> list4 = jtoken4["latestFilesIndexes"].Where<JToken>(delegate(JToken Entry)
                                                            {
                                                                ModLoaders modLoaders2 = ((Entry["modLoader"] == null) ? ModLoaders.None : Resource.FromCurseForgeModLoaderType(Entry["modLoader"].ToObject<int>()));
                                                                if (CS$<>8__locals6.$VB$NonLocal_$VB$Closure_5.$VB$Local_CurseForgeLoaders[CS$<>8__locals6.$VB$Local_LocalFile] == ModLoaders.None)
                                                                {
                                                                    if (modLoaders2 != ModLoaders.None)
                                                                    {
                                                                        return false;
                                                                    }
                                                                }
                                                                else if (!CS$<>8__locals6.$VB$NonLocal_$VB$Closure_5.$VB$Local_CurseForgeLoaders[CS$<>8__locals6.$VB$Local_LocalFile].HasFlagF(modLoaders2))
                                                                {
                                                                    return false;
                                                                }
                                                                string text5 = (string)Entry["gameVersion"];
                                                                return text5 != null && Operators.CompareString(text5, CS$<>8__locals6.$VB$NonLocal_$VB$Closure_5.$VB$NonLocal_$VB$Closure_4.$VB$Local_VanillaVersion, false) == 0;
                                                            }).MaxByAll<JToken, string>((LocalResourceLoaders._Closure$__.$I4-13 == null) ? (LocalResourceLoaders._Closure$__.$I4-13 = (JToken Entry) => Entry["gameVersion"].ToString()) : LocalResourceLoaders._Closure$__.$I4-13, Comparer<string>.Create((LocalResourceLoaders._Closure$__.$I4-14 == null) ? (LocalResourceLoaders._Closure$__.$I4-14 = (string x, string y) => McUtils.CompareVersion(x, y)) : LocalResourceLoaders._Closure$__.$I4-14));
                                                            try
                                                            {
                                                                foreach (JToken jtoken5 in list4)
                                                                {
                                                                    int num2 = jtoken5["fileId"].ToObject<int>();
                                                                    if (!dictionary3.ContainsKey(num2))
                                                                    {
                                                                        dictionary3[num2] = new List<LocalResourceFile>();
                                                                    }
                                                                    dictionary3[num2].Add(CS$<>8__locals6.$VB$Local_LocalFile);
                                                                    dictionary4[num2] = resourceProject2.Slug;
                                                                }
                                                            }
                                                            finally
                                                            {
                                                                List<JToken>.Enumerator enumerator12;
                                                                ((IDisposable)enumerator12).Dispose();
                                                            }
                                                        }
                                                    }
                                                    finally
                                                    {
                                                        List<LocalResourceFile>.Enumerator enumerator11;
                                                        ((IDisposable)enumerator11).Dispose();
                                                    }
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            IEnumerator<JToken> enumerator10;
                                            if (enumerator10 != null)
                                            {
                                                enumerator10.Dispose();
                                            }
                                        }
                                        Logger.Info(string.Format("已从 CurseForge 获取本地 Mod 信息，需要获取 {0} 个用于检查更新的文件信息", dictionary3.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                                        if (dictionary3.Any<KeyValuePair<int, List<LocalResourceFile>>>())
                                        {
                                            JToken jtoken6 = ModDownload.DlModRequest("https://api.curseforge.com/v1/mods/files", HttpMethod.Post, string.Format("{{\"fileIds\": [{0}]}}", dictionary3.Keys.Join(",")), "application/json")["data"];
                                            Dictionary<LocalResourceFile, ResourceVersion> dictionary5 = new Dictionary<LocalResourceFile, ResourceVersion>();
                                            try
                                            {
                                                foreach (JToken jtoken7 in jtoken6)
                                                {
                                                    ResourceVersion resourceVersion3 = ResourceVersion.FromPlatformJson((JObject)jtoken7, ResourceTypes.Mod);
                                                    if (resourceVersion3.DownloadAvailable)
                                                    {
                                                        try
                                                        {
                                                            foreach (LocalResourceFile localResourceFile7 in dictionary3[Conversions.ToInteger(resourceVersion3.Id)])
                                                            {
                                                                if (resourceVersion3.ModLoaders != CS$<>8__locals5.$VB$Local_CurseForgeLoaders[localResourceFile7])
                                                                {
                                                                    Logger.Warn(string.Format("从 CurseForge 获取到的更新文件 {0} 加载器为 {1}，与本地文件 {2} 的加载器 {3} 不一致，已忽略", new object[]
                                                                    {
                                                                        resourceVersion3.FileName,
                                                                        resourceVersion3.ModLoaders,
                                                                        localResourceFile7.File.Name,
                                                                        CS$<>8__locals5.$VB$Local_CurseForgeLoaders[localResourceFile7]
                                                                    }), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                                                                }
                                                                else if (!dictionary5.ContainsKey(localResourceFile7) || DateTime.Compare(dictionary5[localResourceFile7].ReleaseDate, resourceVersion3.ReleaseDate) < 0)
                                                                {
                                                                    dictionary5[localResourceFile7] = resourceVersion3;
                                                                }
                                                            }
                                                        }
                                                        finally
                                                        {
                                                            List<LocalResourceFile>.Enumerator enumerator14;
                                                            ((IDisposable)enumerator14).Dispose();
                                                        }
                                                    }
                                                }
                                            }
                                            finally
                                            {
                                                IEnumerator<JToken> enumerator13;
                                                if (enumerator13 != null)
                                                {
                                                    enumerator13.Dispose();
                                                }
                                            }
                                            try
                                            {
                                                foreach (KeyValuePair<LocalResourceFile, ResourceVersion> keyValuePair in dictionary5)
                                                {
                                                    LocalResourceLoaders._Closure$__4-6 CS$<>8__locals7 = new LocalResourceLoaders._Closure$__4-6(CS$<>8__locals7);
                                                    CS$<>8__locals7.$VB$Local_Entry = keyValuePair.Key;
                                                    CS$<>8__locals7.$VB$Local_UpdateFile = keyValuePair.Value;
                                                    Logger.Trace(() => string.Format("本地文件 {0} 在 CurseForge 上的最新版为 {1}", CS$<>8__locals7.$VB$Local_Entry.ProjectVersion.FileName, CS$<>8__locals7.$VB$Local_UpdateFile.FileName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                                                    if (DateTime.Compare(CS$<>8__locals7.$VB$Local_Entry.ProjectVersion.ReleaseDate, CS$<>8__locals7.$VB$Local_UpdateFile.ReleaseDate) < 0 && Operators.CompareString(CS$<>8__locals7.$VB$Local_Entry.ProjectVersion.Hash, CS$<>8__locals7.$VB$Local_UpdateFile.Hash, false) != 0)
                                                    {
                                                        if (CS$<>8__locals7.$VB$Local_Entry.UpdateFile != null && Operators.CompareString(CS$<>8__locals7.$VB$Local_UpdateFile.Hash, CS$<>8__locals7.$VB$Local_Entry.UpdateFile.Hash, false) == 0)
                                                        {
                                                            CS$<>8__locals7.$VB$Local_Entry.ChangelogUrls.Add(string.Format("https://www.curseforge.com/minecraft/mc-mods/{0}/files/{1}", dictionary4[Conversions.ToInteger(CS$<>8__locals7.$VB$Local_UpdateFile.Id)], CS$<>8__locals7.$VB$Local_UpdateFile.Id));
                                                            CS$<>8__locals7.$VB$Local_Entry.UpdateFile.DownloadUrls.AddRange(CS$<>8__locals7.$VB$Local_UpdateFile.DownloadUrls);
                                                        }
                                                        else if (CS$<>8__locals7.$VB$Local_Entry.UpdateFile == null || DateTime.Compare(CS$<>8__locals7.$VB$Local_UpdateFile.ReleaseDate, CS$<>8__locals7.$VB$Local_Entry.UpdateFile.ReleaseDate) > 0)
                                                        {
                                                            CS$<>8__locals7.$VB$Local_Entry.ChangelogUrls = new List<string> { string.Format("https://www.curseforge.com/minecraft/mc-mods/{0}/files/{1}", dictionary4[Conversions.ToInteger(CS$<>8__locals7.$VB$Local_UpdateFile.Id)], CS$<>8__locals7.$VB$Local_UpdateFile.Id) };
                                                            CS$<>8__locals7.$VB$Local_Entry.UpdateFile = CS$<>8__locals7.$VB$Local_UpdateFile;
                                                        }
                                                    }
                                                }
                                            }
                                            finally
                                            {
                                                Dictionary<LocalResourceFile, ResourceVersion>.Enumerator enumerator15;
                                                ((IDisposable)enumerator15).Dispose();
                                            }
                                            Logger.Info("从 CurseForge 获取 Mod 更新信息结束", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex2)
                        {
                            Logger.Warn(ex2, "从 CurseForge 获取本地 Mod 信息失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                            CS$<>8__locals1.$VB$Local_IsFailed = true;
                        }
                        finally
                        {
                            CS$<>8__locals1.$VB$Local_EndedThreadCount++;
                        }
                    }, "Mod List Detail Loader CurseForge", ThreadPriority.Normal);
                    while (CS$<>8__locals1.$VB$Local_EndedThreadCount != 2)
                    {
                        Thread.Sleep(10);
                        if (CS$<>8__locals1.$VB$Local_Loader.IsCanceled)
                        {
                            return;
                        }
                    }
                    CS$<>8__locals1.$VB$Local_Mods = CS$<>8__locals1.$VB$Local_Mods.Where<LocalResourceFile>((LocalResourceLoaders._Closure$__.$I4-16 == null) ? (LocalResourceLoaders._Closure$__.$I4-16 = (LocalResourceFile m) => m.Project != null) : LocalResourceLoaders._Closure$__.$I4-16).ToList<LocalResourceFile>();
                    Logger.Info(string.Format("联网获取本地 Mod 信息完成，为 {0} 个 Mod 更新缓存", CS$<>8__locals1.$VB$Local_Mods.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\LocalResourceLoaders.vb");
                    if (CS$<>8__locals1.$VB$Local_Mods.Any<LocalResourceFile>() && !CS$<>8__locals1.$VB$Local_Loader.IsCanceled)
                    {
                        try
                        {
                            foreach (LocalResourceFile localResourceFile2 in CS$<>8__locals1.$VB$Local_Mods)
                            {
                                localResourceFile2.OnlineDataLoaded = !CS$<>8__locals1.$VB$Local_IsFailed;
                                item[localResourceFile2.ModrinthHash + CS$<>8__locals1.$VB$Local_VanillaVersion + Conversions.ToString((int)modLoaders)] = localResourceFile2.ToJson();
                            }
                        }
                        finally
                        {
                            List<LocalResourceFile>.Enumerator enumerator2;
                            ((IDisposable)enumerator2).Dispose();
                        }
                        FileUtils.Write(ModBase.PathTemp + "Cache\\LocalMod.json", item.ToString(ModBase.ModeDebug ? 1 : 0, new JsonConverter[0]), null);
                        if (!CS$<>8__locals1.$VB$Local_Loader.IsCanceled)
                        {
                            PageInstanceMod frmInstanceMod = ModMain.FrmInstanceMod;
                            if (frmInstanceMod != null && frmInstanceMod.Filter == PageInstanceMod.FilterType.CanUpdate)
                            {
                                ModBase.RunInUi((LocalResourceLoaders._Closure$__.$I4-17 == null) ? (LocalResourceLoaders._Closure$__.$I4-17 = delegate
                                {
                                    PageInstanceMod frmInstanceMod2 = ModMain.FrmInstanceMod;
                                    if (frmInstanceMod2 == null)
                                    {
                                        return;
                                    }
                                    frmInstanceMod2.RefreshUI();
                                }) : LocalResourceLoaders._Closure$__.$I4-17, false);
                                return;
                            }
                            ModBase.RunInUi((LocalResourceLoaders._Closure$__.$I4-18 == null) ? (LocalResourceLoaders._Closure$__.$I4-18 = delegate
                            {
                                PageInstanceMod frmInstanceMod3 = ModMain.FrmInstanceMod;
                                if (frmInstanceMod3 == null)
                                {
                                    return;
                                }
                                frmInstanceMod3.RefreshBars();
                            }) : LocalResourceLoaders._Closure$__.$I4-18, false);
                        }
                    }
                }
            }
        }

        public static ModLoader.LoaderTask<string, List<LocalResourceFile>> LocalResourceLoader = new ModLoader.LoaderTask<string, List<LocalResourceFile>>("LocalResourceLoader", new Action<ModLoader.LoaderTask<string, List<LocalResourceFile>>>(LocalResourceLoaders.LocalResourceFileLoad), null, ThreadPriority.Normal);

        [TupleElementNames(new string[] { "ModList", "Cache", "Target" })]
        public static ModLoader.LoaderTask<ValueTuple<List<LocalResourceFile>, JObject, McVersion>, int> LocalResourceOnlineLoader = new ModLoader.LoaderTask<ValueTuple<List<LocalResourceFile>, JObject, McVersion>, int>("LocalResourceOnlineLoader", new Action<ModLoader.LoaderTask<ValueTuple<List<LocalResourceFile>, JObject, McVersion>, int>>(LocalResourceLoaders.LocalResourceOnlineLoad), null, ThreadPriority.Normal);

        private static Func<string, bool> $STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile;

        private static StaticLocalInitFlag $STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init;
    }
}
