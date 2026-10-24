using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;
using PCLCS;

namespace PCL
{
    public class ResourceSearcher
    {
        public ResourceSearcher()
        {
        }

        public static ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int> GetLoader(Func<ResourceSearcher.SearchRequest> InputDelegate)
        {
            return new ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int>("社区资源获取", new Action<ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int>>(ResourceSearcher.Run), InputDelegate, ThreadPriority.Normal)
            {
                ReloadTimeout = 60000
            };
        }

        private static void Run(ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int> Task)
        {
            ResourceSearcher._Closure___6_3 CS_locals1 = new ResourceSearcher._Closure___6_3(null);
            CS_locals1._VB_Local_Task = Task;
            CS_locals1._VB_Local_Request = CS_locals1._VB_Local_Task.Input;
            CS_locals1._VB_Local_Storage = CS_locals1._VB_Local_Request.Storage;
            if (CS_locals1._VB_Local_Storage.Results.Count >= CS_locals1._VB_Local_Request.TargetResultCount)
            {
                Logger.Info(string.Format("已有 {0} 个结果，多于所需的 {1} 个结果，结束处理", CS_locals1._VB_Local_Storage.Results.Count, CS_locals1._VB_Local_Request.TargetResultCount), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                return;
            }
            if (CS_locals1._VB_Local_Request.CanContinue)
            {
                CS_locals1._VB_Local_RawSearchText = (CS_locals1._VB_Local_Request.SearchText ?? "").Trim();
                CS_locals1._VB_Local_RawSearchText = CS_locals1._VB_Local_RawSearchText.Lower();
                Logger.Info(string.Format("工程列表搜索原始文本：{0}", CS_locals1._VB_Local_RawSearchText), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                CS_locals1._VB_Local_RawSearchText = Strings.StrConv(CS_locals1._VB_Local_RawSearchText, VbStrConv.SimplifiedChinese, 0);
                bool flag = CS_locals1._VB_Local_RawSearchText.RegexCheck("[\\u4e00-\\u9fbb]", RegexOptions.None) && !string.IsNullOrEmpty(CS_locals1._VB_Local_RawSearchText) && (CS_locals1._VB_Local_Request.Type == ResourceTypes.Mod || CS_locals1._VB_Local_Request.Type == ResourceTypes.DataPack);
                CS_locals1._VB_Local_CurseForgeAltSearchText = null;
                CS_locals1._VB_Local_ModrinthAltSearchText = null;
                CS_locals1._VB_Local_ModrinthSlugs = new List<string>();
                if (flag)
                {
                    VB_AnonymousDelegate_2<ModBase.SearchEntry<WikiEntry>, ResourcePlatforms, IEnumerable<string>> vb_AnonymousDelegate_ = ((ResourceSearcher._Closure___._I6_0 == null) ? (ResourceSearcher._Closure___._I6_0 = new VB_AnonymousDelegate_2<ModBase.SearchEntry<WikiEntry>, ResourcePlatforms, IEnumerable<string>>(ResourceSearcher._Closure___._I._Lambda___6_0)) : ResourceSearcher._Closure___._I6_0);
                    VB_AnonymousDelegate_3<ResourcePlatforms, IEnumerable<ModBase.SearchEntry<WikiEntry>>> vb_AnonymousDelegate_2 = ((ResourceSearcher._Closure___._I6_8 == null) ? (ResourceSearcher._Closure___._I6_8 = new VB_AnonymousDelegate_3<ResourcePlatforms, IEnumerable<ModBase.SearchEntry<WikiEntry>>>(ResourceSearcher._Closure___._I._Lambda___6_8)) : ResourceSearcher._Closure___._I6_8);
                    if (CS_locals1._VB_Local_Request.Sources.HasFlagF(ResourcePlatforms.CurseForge))
                    {
                        if (ResourceSearcher._STATIC_Run_011151286FC21283B48_CurseForgeSearchEntries_Init == null)
                        {
                            Interlocked.CompareExchange<StaticLocalInitFlag>(ref ResourceSearcher._STATIC_Run_011151286FC21283B48_CurseForgeSearchEntries_Init, new StaticLocalInitFlag(), null);
                        }
                        bool flag2 = false;
                        try
                        {
                            Monitor.Enter(ResourceSearcher._STATIC_Run_011151286FC21283B48_CurseForgeSearchEntries_Init, ref flag2);
                            if (ResourceSearcher._STATIC_Run_011151286FC21283B48_CurseForgeSearchEntries_Init.State == 0)
                            {
                                ResourceSearcher._STATIC_Run_011151286FC21283B48_CurseForgeSearchEntries_Init.State = 2;
                                ResourceSearcher._STATIC_Run_011151286FC21283B48_CurseForgeSearchEntries = vb_AnonymousDelegate_2(ResourcePlatforms.CurseForge).ToList<ModBase.SearchEntry<WikiEntry>>();
                            }
                            else if (ResourceSearcher._STATIC_Run_011151286FC21283B48_CurseForgeSearchEntries_Init.State == 2)
                            {
                                throw new IncompleteInitialization();
                            }
                        }
                        finally
                        {
                            ResourceSearcher._STATIC_Run_011151286FC21283B48_CurseForgeSearchEntries_Init.State = 1;
                            if (flag2)
                            {
                                Monitor.Exit(ResourceSearcher._STATIC_Run_011151286FC21283B48_CurseForgeSearchEntries_Init);
                            }
                        }
                        List<ModBase.SearchEntry<WikiEntry>> list = ModBase.Search<WikiEntry>(ResourceSearcher._STATIC_Run_011151286FC21283B48_CurseForgeSearchEntries, CS_locals1._VB_Local_RawSearchText, 100, 0.25);
                        if (list.Any<ModBase.SearchEntry<WikiEntry>>())
                        {
                            ModBase.SearchEntry<WikiEntry> searchEntry = (list.First<ModBase.SearchEntry<WikiEntry>>().AbsoluteRight ? list.Where<ModBase.SearchEntry<WikiEntry>>((ResourceSearcher._Closure___._I6_9 == null) ? (ResourceSearcher._Closure___._I6_9 = new Func<ModBase.SearchEntry<WikiEntry>, bool>(ResourceSearcher._Closure___._I._Lambda___6_9)) : ResourceSearcher._Closure___._I6_9).ToList<ModBase.SearchEntry<WikiEntry>>() : list.MaxByAll<ModBase.SearchEntry<WikiEntry>, double>((ResourceSearcher._Closure___._I6_10 == null) ? (ResourceSearcher._Closure___._I6_10 = new Func<ModBase.SearchEntry<WikiEntry>, double>(ResourceSearcher._Closure___._I._Lambda___6_10)) : ResourceSearcher._Closure___._I6_10)).MaxBy<ModBase.SearchEntry<WikiEntry>, int>((ResourceSearcher._Closure___._I6_11 == null) ? (ResourceSearcher._Closure___._I6_11 = new Func<ModBase.SearchEntry<WikiEntry>, int>(ResourceSearcher._Closure___._I._Lambda___6_11)) : ResourceSearcher._Closure___._I6_11);
                            CS_locals1._VB_Local_CurseForgeAltSearchText = vb_AnonymousDelegate_(searchEntry, ResourcePlatforms.CurseForge).Join(" ");
                            Logger.Warn(string.Format("中文搜索关键词（CurseForge）：{0}", CS_locals1._VB_Local_CurseForgeAltSearchText), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                        }
                    }
                    if (CS_locals1._VB_Local_Request.Sources.HasFlagF(ResourcePlatforms.Modrinth))
                    {
                        if (ResourceSearcher._STATIC_Run_011151286FC21283B48_ModrinthSearchEntries_Init == null)
                        {
                            Interlocked.CompareExchange<StaticLocalInitFlag>(ref ResourceSearcher._STATIC_Run_011151286FC21283B48_ModrinthSearchEntries_Init, new StaticLocalInitFlag(), null);
                        }
                        bool flag3 = false;
                        try
                        {
                            Monitor.Enter(ResourceSearcher._STATIC_Run_011151286FC21283B48_ModrinthSearchEntries_Init, ref flag3);
                            if (ResourceSearcher._STATIC_Run_011151286FC21283B48_ModrinthSearchEntries_Init.State == 0)
                            {
                                ResourceSearcher._STATIC_Run_011151286FC21283B48_ModrinthSearchEntries_Init.State = 2;
                                ResourceSearcher._STATIC_Run_011151286FC21283B48_ModrinthSearchEntries = vb_AnonymousDelegate_2(ResourcePlatforms.Modrinth).ToList<ModBase.SearchEntry<WikiEntry>>();
                            }
                            else if (ResourceSearcher._STATIC_Run_011151286FC21283B48_ModrinthSearchEntries_Init.State == 2)
                            {
                                throw new IncompleteInitialization();
                            }
                        }
                        finally
                        {
                            ResourceSearcher._STATIC_Run_011151286FC21283B48_ModrinthSearchEntries_Init.State = 1;
                            if (flag3)
                            {
                                Monitor.Exit(ResourceSearcher._STATIC_Run_011151286FC21283B48_ModrinthSearchEntries_Init);
                            }
                        }
                        List<ModBase.SearchEntry<WikiEntry>> list2 = ModBase.Search<WikiEntry>(ResourceSearcher._STATIC_Run_011151286FC21283B48_ModrinthSearchEntries, CS_locals1._VB_Local_RawSearchText, 100, 0.25);
                        if (list2.Any<ModBase.SearchEntry<WikiEntry>>())
                        {
                            Dictionary<string, double> dictionary = new Dictionary<string, double>();
                            try
                            {
                                foreach (ModBase.SearchEntry<WikiEntry> searchEntry2 in list2)
                                {
                                    try
                                    {
                                        foreach (string text in vb_AnonymousDelegate_(searchEntry2, ResourcePlatforms.Modrinth))
                                        {
                                            if (!dictionary.ContainsKey(text))
                                            {
                                                dictionary.Add(text, 0.0);
                                            }
                                            double num = (searchEntry2.SearchSource.Any<ModBase.SearchSource>((CS_locals1._I12 == null) ? (CS_locals1._I12 = new Func<ModBase.SearchSource, bool>(CS_locals1._Lambda___12)) : CS_locals1._I12) ? 1000.0 : searchEntry2.Similarity);
                                            Dictionary<string, double> dictionary2;
                                            string text2;
                                            (dictionary2 = dictionary)[text2 = text] = dictionary2[text2] + num * (double)searchEntry2.Item.Popularity;
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
                                }
                            }
                            finally
                            {
                                List<ModBase.SearchEntry<WikiEntry>>.Enumerator enumerator = default;
                                ((IDisposable)enumerator).Dispose();
                            }
                            CS_locals1._VB_Local_ModrinthAltSearchText = dictionary.MaxBy<KeyValuePair<string, double>, double>((ResourceSearcher._Closure___._I6_13 == null) ? (ResourceSearcher._Closure___._I6_13 = new Func<KeyValuePair<string, double>, double>(ResourceSearcher._Closure___._I._Lambda___6_13)) : ResourceSearcher._Closure___._I6_13).Key;
                            Logger.Warn(string.Format("中文搜索关键词（Modrinth）：{0}", CS_locals1._VB_Local_ModrinthAltSearchText), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                            CS_locals1._VB_Local_ModrinthSlugs = list2.Take<ModBase.SearchEntry<WikiEntry>>(100).Select<ModBase.SearchEntry<WikiEntry>, string>((ResourceSearcher._Closure___._I6_14 == null) ? (ResourceSearcher._Closure___._I6_14 = new Func<ModBase.SearchEntry<WikiEntry>, string>(ResourceSearcher._Closure___._I._Lambda___6_14)) : ResourceSearcher._Closure___._I6_14).ToList<string>();
                        }
                    }
                    if (string.IsNullOrEmpty(CS_locals1._VB_Local_CurseForgeAltSearchText) && string.IsNullOrEmpty(CS_locals1._VB_Local_ModrinthAltSearchText) && !CS_locals1._VB_Local_ModrinthSlugs.Any<string>())
                    {
                        throw new Exception("无搜索结果，请尝试搜索其英文名称");
                    }
                }
                CS_locals1._VB_Local_Task.Progress = 0.05;
                CS_locals1._VB_Local_RealResults = new List<ResourceProject>();
                checked
                {
                    for (;;)
                    {
                        List<ResourceProject> list3 = new List<ResourceProject>();
                        List<Thread> list4 = new List<Thread>();
                        CS_locals1._VB_Local_Errors = new ConcurrentBag<ValueTuple<Exception, ResourcePlatforms>>();
                        CS_locals1._VB_Local_IgnoreModLoaderFilter = CS_locals1._VB_Local_Request.ModLoaders == ModLoaders.Forge && McVersion.VersionToDrop(CS_locals1._VB_Local_Request.GameVersion) < 140;
                        CS_locals1._VB_Local_SearchResults = new ConcurrentBag<ResourceProject>();
                        try
                        {
                            if (CS_locals1._VB_Local_Request.Sources.HasFlagF(ResourcePlatforms.CurseForge) && (CS_locals1._VB_Local_Storage.CurseForgeTotal <= -1 || CS_locals1._VB_Local_Storage.CurseForgeTotal > CS_locals1._VB_Local_Storage.CurseForgeOffset) && (!flag || (flag && !string.IsNullOrEmpty(CS_locals1._VB_Local_CurseForgeAltSearchText))))
                            {
                                list4.Add(ModBase.RunInNewThread(new Action(CS_locals1._Lambda___15), "CurseForge Search", ThreadPriority.Normal));
                            }
                            if (CS_locals1._VB_Local_Request.Sources.HasFlagF(ResourcePlatforms.Modrinth) && (CS_locals1._VB_Local_Storage.ModrinthTotal <= -1 || CS_locals1._VB_Local_Storage.ModrinthTotal > CS_locals1._VB_Local_Storage.ModrinthOffset) && (!flag || (flag && !string.IsNullOrEmpty(CS_locals1._VB_Local_ModrinthAltSearchText))))
                            {
                                list4.Add(ModBase.RunInNewThread(new Action(CS_locals1._Lambda___17), "Modrinth Search", ThreadPriority.Normal));
                            }
                            if (CS_locals1._VB_Local_Request.Sources.HasFlagF(ResourcePlatforms.Modrinth) && (CS_locals1._VB_Local_Storage.ModrinthTotal <= -1 || CS_locals1._VB_Local_Storage.ModrinthTotal > CS_locals1._VB_Local_Storage.ModrinthOffset) && CS_locals1._VB_Local_ModrinthSlugs.Any<string>())
                            {
                                list4.Add(ModBase.RunInNewThread(new Action(CS_locals1._Lambda___19), "Modrinth Get", ThreadPriority.Normal));
                            }
                            try
                            {
                                foreach (Thread thread in list4)
                                {
                                    thread.Join();
                                    if (CS_locals1._VB_Local_Task.IsCanceled)
                                    {
                                        return;
                                    }
                                }
                            }
                            finally
                            {
                                List<Thread>.Enumerator enumerator3 = default;
                                ((IDisposable)enumerator3).Dispose();
                            }
                            list3 = new List<ResourceProject>(CS_locals1._VB_Local_SearchResults);
                            if (CS_locals1._VB_Local_IgnoreModLoaderFilter)
                            {
                                list3.KeepIf<ResourceProject>((ResourceSearcher._Closure___._I6_27 == null) ? (ResourceSearcher._Closure___._I6_27 = new Func<ResourceProject, bool>(ResourceSearcher._Closure___._I._Lambda___6_27)) : ResourceSearcher._Closure___._I6_27);
                            }
                            CS_locals1._VB_Local_Storage.ErrorMessage = null;
                            if (!list3.Any<ResourceProject>())
                            {
                                if (CS_locals1._VB_Local_Errors.Any<ValueTuple<Exception, ResourcePlatforms>>())
                                {
                                    throw CS_locals1._VB_Local_Errors.First<ValueTuple<Exception, ResourcePlatforms>>().Item1;
                                }
                                if (flag && CS_locals1._VB_Local_Request.Type != ResourceTypes.Mod && CS_locals1._VB_Local_Request.Type != ResourceTypes.DataPack)
                                {
                                    throw new Exception("无搜索结果，请尝试搜索其英文名称");
                                }
                                if (CS_locals1._VB_Local_Request.Sources == ResourcePlatforms.CurseForge && CS_locals1._VB_Local_Request.Tag.StartsWithF("/", false))
                                {
                                    throw new Exception("CurseForge 不兼容所选的类型");
                                }
                                if (CS_locals1._VB_Local_Request.Sources == ResourcePlatforms.Modrinth && CS_locals1._VB_Local_Request.Tag.EndsWithF("/", false))
                                {
                                    throw new Exception("Modrinth 不兼容所选的类型");
                                }
                                throw new Exception("没有搜索结果");
                            }
                            else if (CS_locals1._VB_Local_Errors.Any<ValueTuple<Exception, ResourcePlatforms>>())
                            {
                                if (CS_locals1._VB_Local_Errors.Any<ValueTuple<Exception, ResourcePlatforms>>((ResourceSearcher._Closure___._I6_28 == null) ? (ResourceSearcher._Closure___._I6_28 = new Func<ValueTuple<Exception, ResourcePlatforms>, bool>(ResourceSearcher._Closure___._I._Lambda___6_28)) : ResourceSearcher._Closure___._I6_28))
                                {
                                    CS_locals1._VB_Local_Storage.ErrorMessage = string.Format("无法连接到 CurseForge，所以目前仅显示了来自 Modrinth 的内容，搜索结果可能不全。{0}请稍后再试，或使用 VPN 改善网络环境。", "\r\n");
                                }
                                else
                                {
                                    CS_locals1._VB_Local_Storage.ErrorMessage = string.Format("无法连接到 Modrinth，所以目前仅显示了来自 CurseForge 的内容，搜索结果可能不全。{0}请稍后再试，或使用 VPN 改善网络环境。", "\r\n");
                                }
                            }
                        }
                        finally
                        {
                            try
                            {
                                foreach (Thread thread2 in list4)
                                {
                                    if (thread2.IsAlive)
                                    {
                                        thread2.Interrupt();
                                    }
                                }
                            }
                            finally
                            {
                                List<Thread>.Enumerator enumerator4 = default;
                                ((IDisposable)enumerator4).Dispose();
                            }
                        }
                        list3 = list3.Where<ResourceProject>((ResourceSearcher._Closure___._I6_29 == null) ? (ResourceSearcher._Closure___._I6_29 = new Func<ResourceProject, bool>(ResourceSearcher._Closure___._I._Lambda___6_29)) : ResourceSearcher._Closure___._I6_29).Concat<ResourceProject>(list3.Where<ResourceProject>((ResourceSearcher._Closure___._I6_30 == null) ? (ResourceSearcher._Closure___._I6_30 = new Func<ResourceProject, bool>(ResourceSearcher._Closure___._I._Lambda___6_30)) : ResourceSearcher._Closure___._I6_30)).ToList<ResourceProject>();
                        list3 = list3.Distinct<ResourceProject>((ResourceSearcher._Closure___._I6_31 == null) ? (ResourceSearcher._Closure___._I6_31 = new Func<ResourceProject, ResourceProject, bool>(ResourceSearcher._Closure___._I._Lambda___6_31)) : ResourceSearcher._Closure___._I6_31).ToList<ResourceProject>();
                        list3 = list3.Where<ResourceProject>(new Func<ResourceProject, bool>(CS_locals1._Lambda___32)).ToList<ResourceProject>();
                        CS_locals1._VB_Local_RealResults.AddRange(list3);
                        Logger.Info(string.Format("去重、筛选后累计新增结果 {0} 个（目前已有结果 {1} 个）", CS_locals1._VB_Local_RealResults.Count, CS_locals1._VB_Local_Storage.Results.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                        if (CS_locals1._VB_Local_RealResults.Count + CS_locals1._VB_Local_Storage.Results.Count >= CS_locals1._VB_Local_Request.TargetResultCount)
                        {
                            goto IL_0B52;
                        }
                        Logger.Info(string.Format("总结果数需求最少 {0} 个，仅获得了 {1} 个", CS_locals1._VB_Local_Request.TargetResultCount, CS_locals1._VB_Local_RealResults.Count + CS_locals1._VB_Local_Storage.Results.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                        if (!CS_locals1._VB_Local_Request.CanContinue || CS_locals1._VB_Local_Errors.Any<ValueTuple<Exception, ResourcePlatforms>>())
                        {
                            break;
                        }
                        Logger.Info("将继续尝试加载下一页", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                    }
                    Logger.Info("无法继续加载，将强制结束", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                }
                IL_0B52:
                Dictionary<ResourceProject, double> dictionary3 = new Dictionary<ResourceProject, double>();
                VB_AnonymousDelegate_4<ResourceProject, double> vb_AnonymousDelegate_3 = new VB_AnonymousDelegate_4<ResourceProject, double>(CS_locals1._Lambda___35);
                if (string.IsNullOrEmpty(CS_locals1._VB_Local_RawSearchText))
                {
                    try
                    {
                        foreach (ResourceProject resourceProject in CS_locals1._VB_Local_RealResults)
                        {
                            dictionary3[resourceProject] = (double)resourceProject.DownloadCount * vb_AnonymousDelegate_3(resourceProject);
                        }
                        goto IL_0D62;
                    }
                    finally
                    {
                        List<ResourceProject>.Enumerator enumerator5 = default;
                        ((IDisposable)enumerator5).Dispose();
                    }
                }
                List<ModBase.SearchEntry<ResourceProject>> list5 = new List<ModBase.SearchEntry<ResourceProject>>();
                try
                {
                    foreach (ResourceProject resourceProject2 in CS_locals1._VB_Local_RealResults)
                    {
                        dictionary3[resourceProject2] = ((resourceProject2.WikiId > 0) ? 0.2 : 0.0) + Math.Log10((double)Math.Max(resourceProject2.DownloadCount, 1) * vb_AnonymousDelegate_3(resourceProject2)) / 9.0;
                        list5.Add(new ModBase.SearchEntry<ResourceProject>
                        {
                            Item = resourceProject2,
                            SearchSource = new List<ModBase.SearchSource>
                            {
                                new ModBase.SearchSource((flag ? resourceProject2.TranslatedName : resourceProject2.RawName).Split('/', true), 1.0),
                                new ModBase.SearchSource(resourceProject2.Description, 0.05)
                            }
                        });
                    }
                }
                finally
                {
                    List<ResourceProject>.Enumerator enumerator6 = default;
                    ((IDisposable)enumerator6).Dispose();
                }
                List<ModBase.SearchEntry<ResourceProject>> list6 = ModBase.Search<ResourceProject>(list5, CS_locals1._VB_Local_RawSearchText, 10000, -1.0);
                try
                {
                    foreach (ModBase.SearchEntry<ResourceProject> searchEntry3 in list6)
                    {
                        Dictionary<ResourceProject, double> dictionary4;
                        ResourceProject item;
                        (dictionary4 = dictionary3)[item = searchEntry3.Item] = dictionary4[item] + (searchEntry3.AbsoluteRight ? 10.0 : searchEntry3.Similarity) / (list6.First<ModBase.SearchEntry<ResourceProject>>().AbsoluteRight ? 10.0 : list6.First<ModBase.SearchEntry<ResourceProject>>().Similarity);
                    }
                }
                finally
                {
                    List<ModBase.SearchEntry<ResourceProject>>.Enumerator enumerator7 = default;
                    ((IDisposable)enumerator7).Dispose();
                }
                IL_0D62:
                if (CS_locals1._VB_Local_Task.IsCanceled)
                {
                    throw new OperationCanceledException();
                }
                CS_locals1._VB_Local_Storage.Results.AddRange(dictionary3.OrderByDescending<KeyValuePair<ResourceProject, double>, double>((ResourceSearcher._Closure___._I6_36 == null) ? (ResourceSearcher._Closure___._I6_36 = new Func<KeyValuePair<ResourceProject, double>, double>(ResourceSearcher._Closure___._I._Lambda___6_36)) : ResourceSearcher._Closure___._I6_36).Select<KeyValuePair<ResourceProject, double>, ResourceProject>((ResourceSearcher._Closure___._I6_37 == null) ? (ResourceSearcher._Closure___._I6_37 = new Func<KeyValuePair<ResourceProject, double>, ResourceProject>(ResourceSearcher._Closure___._I._Lambda___6_37)) : ResourceSearcher._Closure___._I6_37));
                return;
            }
            if (!CS_locals1._VB_Local_Storage.Results.Any<ResourceProject>())
            {
                throw new Exception("没有符合条件的结果");
            }
            Logger.Info(string.Format("已有 {0} 个结果，少于所需的 {1} 个结果，但无法继续获取，结束处理", CS_locals1._VB_Local_Storage.Results.Count, CS_locals1._VB_Local_Request.TargetResultCount), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
        }

        private const int RESULT_PAGE_SIZE = 40;

        private const string MSG_NO_CHINESE_RESULT = "无搜索结果，请尝试搜索其英文名称";

        private static List<ModBase.SearchEntry<WikiEntry>> _STATIC_Run_011151286FC21283B48_CurseForgeSearchEntries;

        private static StaticLocalInitFlag _STATIC_Run_011151286FC21283B48_CurseForgeSearchEntries_Init;

        private static List<ModBase.SearchEntry<WikiEntry>> _STATIC_Run_011151286FC21283B48_ModrinthSearchEntries;

        private static StaticLocalInitFlag _STATIC_Run_011151286FC21283B48_ModrinthSearchEntries_Init;

        public class SearchRequest
        {
            public bool CanContinue
            {
                get
                {
                    if (this.Tag.StartsWithF("/", false) || !this.Sources.HasFlagF(ResourcePlatforms.CurseForge))
                    {
                        this.Storage.CurseForgeTotal = 0;
                    }
                    if (this.Tag.EndsWithF("/", false) || !this.Sources.HasFlagF(ResourcePlatforms.Modrinth))
                    {
                        this.Storage.ModrinthTotal = 0;
                    }
                    return this.Storage.CurseForgeTotal == -1 || this.Storage.ModrinthTotal == -1 || this.Storage.CurseForgeOffset < this.Storage.CurseForgeTotal || this.Storage.ModrinthOffset < this.Storage.ModrinthTotal;
                }
            }

            public SearchRequest(ResourceTypes Type, ResourceSearcher.SearchResult Storage, int TargetResultCount)
            {
                this.Tag = "";
                this.ModLoaders = ModLoaders.None;
                this.GameVersion = null;
                this.SearchText = null;
                this.Sources = ResourcePlatforms.Any;
                this.Type = Type;
                this.Storage = Storage;
                this.TargetResultCount = TargetResultCount;
            }

            public string GetCurseForgeAddress(string SearchText, bool IgnoreModLoaderFilter)
            {
                if (this.Tag.StartsWithF("/", false))
                {
                    this.Storage.CurseForgeTotal = 0;
                }
                string text = string.Format("https://api.curseforge.com/v1/mods/search?gameId=432&sortField=2&sortOrder=desc&pageSize={0}", 40);
                ResourceTypes type = this.Type;
                switch (type)
                {
                case ResourceTypes.Mod:
                    text += "&classId=6";
                    break;
                case ResourceTypes.ModPack:
                    text += "&classId=4471";
                    break;
                case ResourceTypes.Mod | ResourceTypes.ModPack:
                    break;
                case ResourceTypes.ResourcePack:
                    text += "&classId=12";
                    break;
                default:
                    if (type != ResourceTypes.Shader)
                    {
                        if (type == ResourceTypes.DataPack)
                        {
                            text += "&classId=6945";
                        }
                    }
                    else
                    {
                        text += "&classId=6552";
                    }
                    break;
                }
                if (Operators.CompareString(this.Tag, "", false) != 0)
                {
                    text = text + "&categoryId=" + this.Tag.BeforeFirst("/", false);
                }
                if (this.ModLoaders != ModLoaders.None && !IgnoreModLoaderFilter)
                {
                    text = text + "&modLoaderType=" + Conversions.ToString(Resource.ToCurseForgeModLoaderType(this.ModLoaders.Flags<ModLoaders>().First<ModLoaders>()));
                }
                if (!string.IsNullOrEmpty(this.GameVersion))
                {
                    text = text + "&gameVersion=" + this.GameVersion;
                }
                if (!string.IsNullOrEmpty(SearchText))
                {
                    text = text + "&searchFilter=" + StringUtils.UrlEscape(SearchText);
                }
                if (this.Storage.CurseForgeOffset > 0)
                {
                    text = text + "&index=" + Conversions.ToString(this.Storage.CurseForgeOffset);
                }
                return text;
            }

            public string GetModrinthAddress(string SearchText, bool IgnoreModLoaderFilter)
            {
                if (this.Tag.EndsWithF("/", false))
                {
                    this.Storage.ModrinthTotal = 0;
                }
                string text = string.Format("https://api.modrinth.com/v2/search?limit={0}&index=relevance", 40);
                if (!string.IsNullOrEmpty(SearchText))
                {
                    text = text + "&query=" + StringUtils.UrlEscape(SearchText);
                }
                if (this.Storage.ModrinthOffset > 0)
                {
                    text = text + "&offset=" + Conversions.ToString(this.Storage.ModrinthOffset);
                }
                List<string> list = new List<string>();
                list.Add(string.Format("[\"project_type:{0}\"]", this.Type.ToString().Lower()));
                if (!string.IsNullOrEmpty(this.Tag))
                {
                    list.Add(string.Format("[\"categories:'{0}'\"]", this.Tag.AfterLast("/", false)));
                }
                if (this.ModLoaders != ModLoaders.None && !IgnoreModLoaderFilter)
                {
                    list.Add(string.Format("[\"categories:{0}\"]", this.ModLoaders.Flags<ModLoaders>().Select<ModLoaders, string>((ResourceSearcher.SearchRequest._Closure___._I12_0 == null) ? (ResourceSearcher.SearchRequest._Closure___._I12_0 = new Func<ModLoaders, string>(ResourceSearcher.SearchRequest._Closure___._I._Lambda___12_0)) : ResourceSearcher.SearchRequest._Closure___._I12_0).Join("\",\"categories:")));
                }
                if (!string.IsNullOrEmpty(this.GameVersion))
                {
                    list.Add(string.Format("[\"versions:'{0}'\"]", this.GameVersion));
                }
                return text + "&facets=[" + string.Join(",", list) + "]";
            }

            public override bool Equals(object obj)
            {
                ResourceSearcher.SearchRequest searchRequest = obj as ResourceSearcher.SearchRequest;
                return ((object)searchRequest != (object)null) && this.Type == searchRequest.Type && this.TargetResultCount == searchRequest.TargetResultCount && Operators.CompareString(this.Tag, searchRequest.Tag, false) == 0 && this.ModLoaders == searchRequest.ModLoaders && this.Sources == searchRequest.Sources && Operators.CompareString(this.GameVersion, searchRequest.GameVersion, false) == 0 && Operators.CompareString(this.SearchText, searchRequest.SearchText, false) == 0;
            }

            public static bool operator ==(ResourceSearcher.SearchRequest left, ResourceSearcher.SearchRequest right)
            {
                return EqualityComparer<ResourceSearcher.SearchRequest>.Default.Equals(left, right);
            }

            public static bool operator !=(ResourceSearcher.SearchRequest left, ResourceSearcher.SearchRequest right)
            {
                return !(((object)left == (object)right));
            }

            public override int GetHashCode()
            {
                return new ValueTuple<ResourceTypes, string, ModLoaders, string, string, ResourcePlatforms>(this.Type, this.Tag, this.ModLoaders, this.GameVersion, this.SearchText, this.Sources).GetHashCode();
            }

            public ResourceSearcher.SearchResult Storage;

            public int TargetResultCount;

            public ResourceTypes Type;

            public string Tag;

            public ModLoaders ModLoaders;

            public string GameVersion;

            public string SearchText;

            public ResourcePlatforms Sources;

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

                internal string _Lambda___12_0(ModLoaders f)
                {
                    return f.ToString().Lower();
                }

                public static readonly ResourceSearcher.SearchRequest._Closure___ _I = new ResourceSearcher.SearchRequest._Closure___();

                public static Func<ModLoaders, string> _I12_0;
            }
        }

        public class SearchResult
        {
            public SearchResult()
            {
                this.CurseForgeOffset = 0;
                this.CurseForgeTotal = -1;
                this.ModrinthOffset = 0;
                this.ModrinthTotal = -1;
                this.Results = new List<ResourceProject>();
                this.ErrorMessage = null;
            }

            public int CurseForgeOffset;

            public int CurseForgeTotal;

            public int ModrinthOffset;

            public int ModrinthTotal;

            public List<ResourceProject> Results;

            public string ErrorMessage;
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

            internal IEnumerable<string> _Lambda___6_0(ModBase.SearchEntry<WikiEntry> Result, ResourcePlatforms Source)
            {
                ResourceSearcher._Closure___6_0 CS_locals1 = new ResourceSearcher._Closure___6_0(null);
                CS_locals1._VB_Local_Candidates = new List<string>();
                if (Result.Item.Slugs.ContainsKey(Source))
                {
                    CS_locals1._VB_Local_Candidates.Add(Result.Item.Slugs[Source].Replace("-", " ").Replace("/", " "));
                }
                if (Result.Item.ChineseName != null)
                {
                    CS_locals1._VB_Local_Candidates.Add(Result.Item.ChineseName.AfterLast(" (", false).TrimEnd(new char[] { ')' }).BeforeFirst(" - ", false)
                        .Replace("-", " ")
                        .Replace("/", " ")
                        .Replace(":", " ")
                        .Replace("(", " ")
                        .Replace(")", ""));
                }
                CS_locals1._VB_Local_Candidates = CS_locals1._VB_Local_Candidates.SelectMany<string, string>((ResourceSearcher._Closure___._I6_1 == null) ? (ResourceSearcher._Closure___._I6_1 = new Func<string, IEnumerable<string>>(ResourceSearcher._Closure___._I._Lambda___6_1)) : ResourceSearcher._Closure___._I6_1).Select<string, string>((ResourceSearcher._Closure___._I6_2 == null) ? (ResourceSearcher._Closure___._I6_2 = new Func<string, string>(ResourceSearcher._Closure___._I._Lambda___6_2)) : ResourceSearcher._Closure___._I6_2).Where<string>((ResourceSearcher._Closure___._I6_3 == null) ? (ResourceSearcher._Closure___._I6_3 = new Func<string, bool>(ResourceSearcher._Closure___._I._Lambda___6_3)) : ResourceSearcher._Closure___._I6_3)
                    .Distinct<string>()
                    .ToList<string>();
                CS_locals1._VB_Local_CanForm = null;
                CS_locals1._VB_Local_CanForm = new Func<string, bool>(CS_locals1._Lambda___4);
                CS_locals1._VB_Local_Candidates = CS_locals1._VB_Local_Candidates.Where<string>(new Func<string, bool>(CS_locals1._Lambda___6)).ToList<string>();
                return CS_locals1._VB_Local_Candidates;
            }

            internal IEnumerable<string> _Lambda___6_1(string c)
            {
                return c.Split(" ", false);
            }

            internal string _Lambda___6_2(string w)
            {
                return w.TrimStart(new char[] { '{' }).TrimEnd(new char[] { '}' }).Lower();
            }

            internal bool _Lambda___6_3(string w)
            {
                return w.Length > 1 && !new string[] { "the", "of", "mod", "and", "forge", "fabric", "for", "quilt", "neoforge" }.Contains(w) && ModBase.Val(w) <= 0.0 && w.IsAsciiOnly();
            }

            internal IEnumerable<ModBase.SearchEntry<WikiEntry>> _Lambda___6_8(ResourcePlatforms Source)
            {
                ResourceSearcher._Closure___.VB_StateMachine___Lambda___6_8 vb_StateMachine___Lambda___6_ = new ResourceSearcher._Closure___.VB_StateMachine___Lambda___6_8(-2);
                vb_StateMachine___Lambda___6_._VB_NonLocal__Closure___ = this;
                vb_StateMachine___Lambda___6_._P_Source = Source;
                return vb_StateMachine___Lambda___6_;
            }

            internal bool _Lambda___6_9(ModBase.SearchEntry<WikiEntry> s)
            {
                return s.AbsoluteRight;
            }

            internal double _Lambda___6_10(ModBase.SearchEntry<WikiEntry> s)
            {
                return s.Similarity;
            }

            internal int _Lambda___6_11(ModBase.SearchEntry<WikiEntry> s)
            {
                return s.Item.Popularity;
            }

            internal double _Lambda___6_13(KeyValuePair<string, double> w)
            {
                return w.Value;
            }

            internal string _Lambda___6_14(ModBase.SearchEntry<WikiEntry> r)
            {
                return r.Item.Slugs[ResourcePlatforms.Modrinth];
            }

            internal string _Lambda___6_22(ResourceProject p)
            {
                return p.Id;
            }

            internal string _Lambda___6_23(ModLoaders f)
            {
                return f.ToString().Lower();
            }

            internal string _Lambda___6_24(JToken h)
            {
                return h["project_id"].ToString();
            }

            internal bool _Lambda___6_27(ResourceProject p)
            {
                return p.ModLoaders == ModLoaders.None || p.ModLoaders.HasFlagF(ModLoaders.Forge);
            }

            internal bool _Lambda___6_28(ValueTuple<Exception, ResourcePlatforms> e)
            {
                return e.Item2 == ResourcePlatforms.CurseForge;
            }

            internal bool _Lambda___6_29(ResourceProject x)
            {
                return x.Platform == ResourcePlatforms.CurseForge;
            }

            internal bool _Lambda___6_30(ResourceProject x)
            {
                return x.Platform != ResourcePlatforms.CurseForge;
            }

            internal bool _Lambda___6_31(ResourceProject a, ResourceProject b)
            {
                return a.IsLike(b);
            }

            internal double _Lambda___6_36(KeyValuePair<ResourceProject, double> s)
            {
                return s.Value;
            }

            internal ResourceProject _Lambda___6_37(KeyValuePair<ResourceProject, double> r)
            {
                return r.Key;
            }

            public static readonly ResourceSearcher._Closure___ _I = new ResourceSearcher._Closure___();

            public static Func<string, IEnumerable<string>> _I6_1;

            public static Func<string, string> _I6_2;

            public static Func<string, bool> _I6_3;

            public static VB_AnonymousDelegate_2<ModBase.SearchEntry<WikiEntry>, ResourcePlatforms, IEnumerable<string>> _I6_0;

            public static VB_AnonymousDelegate_3<ResourcePlatforms, IEnumerable<ModBase.SearchEntry<WikiEntry>>> _I6_8;

            public static Func<ModBase.SearchEntry<WikiEntry>, bool> _I6_9;

            public static Func<ModBase.SearchEntry<WikiEntry>, double> _I6_10;

            public static Func<ModBase.SearchEntry<WikiEntry>, int> _I6_11;

            public static Func<KeyValuePair<string, double>, double> _I6_13;

            public static Func<ModBase.SearchEntry<WikiEntry>, string> _I6_14;

            public static Func<ResourceProject, string> _I6_22;

            public static Func<ModLoaders, string> _I6_23;

            public static Func<JToken, string> _I6_24;

            public static Func<ResourceProject, bool> _I6_27;

            public static Func<ValueTuple<Exception, ResourcePlatforms>, bool> _I6_28;

            public static Func<ResourceProject, bool> _I6_29;

            public static Func<ResourceProject, bool> _I6_30;

            public static Func<ResourceProject, ResourceProject, bool> _I6_31;

            public static Func<KeyValuePair<ResourceProject, double>, double> _I6_36;

            public static Func<KeyValuePair<ResourceProject, double>, ResourceProject> _I6_37;

            [CompilerGenerated]
            private sealed class VB_StateMachine___Lambda___6_8 : IEnumerable<ModBase.SearchEntry<WikiEntry>>, IEnumerable, IEnumerator<ModBase.SearchEntry<WikiEntry>>, IDisposable, IEnumerator
            {
                public VB_StateMachine___Lambda___6_8(int _State)
                {
                    this._State = _State;
                    this._InitialThreadId = Environment.CurrentManagedThreadId;
                }

                void IDisposable.Dispose()
                {
                    int num = this._State;
                    if (num - 1 <= 1)
                    {
                        this._State = -3;
                    }
                    else
                    {
                        this._State = -1;
                    }
                    this.MoveNext();
                    this._State = -2;
                }

                [CompilerGenerated]
                public bool MoveNext()
                {
                    int num = this._State;
                    switch (num)
                    {
                    case -3:
                    case 1:
                    case 2:
                        break;
                    default:
                        return false;
                    case 0:
                        num = (this._State = -1);
                        break;
                    }
                    try
                    {
                        if (num == -3)
                        {
                            num = (this._State = -1);
                            return true;
                        }
                        if (num != 1)
                        {
                            if (num != 2)
                            {
                                this._S0 = WikiEntry.All.Value.GetEnumerator();
                            }
                            else
                            {
                                num = (this._State = -1);
                            }
                        }
                        else
                        {
                            num = (this._State = -1);
                        }
                        while (this._S0.MoveNext())
                        {
                            WikiEntry wikiEntry = this._S0.Current;
                            if (wikiEntry.Slugs.ContainsKey(this._VB_Local_Source))
                            {
                                if (wikiEntry.ChineseName != null)
                                {
                                    this._Current = new ModBase.SearchEntry<WikiEntry>
                                    {
                                        Item = wikiEntry,
                                        SearchSource = new List<ModBase.SearchSource>
                                        {
                                            new ModBase.SearchSource(Strings.StrConv(wikiEntry.ChineseName.BeforeFirst(" (", false), VbStrConv.SimplifiedChinese, 0).Split('/', true), 1.0),
                                            new ModBase.SearchSource(Strings.StrConv(wikiEntry.ChineseName.AfterFirst(" (", false), VbStrConv.SimplifiedChinese, 0) + wikiEntry.Slugs[this._VB_Local_Source], 0.5)
                                        }
                                    };
                                    num = (this._State = 1);
                                    return true;
                                }
                                this._Current = new ModBase.SearchEntry<WikiEntry>
                                {
                                    Item = wikiEntry,
                                    SearchSource = new List<ModBase.SearchSource>
                                    {
                                        new ModBase.SearchSource(wikiEntry.Slugs[this._VB_Local_Source], 0.5)
                                    }
                                };
                                num = (this._State = 2);
                                return true;
                            }
                        }
                    }
                    finally
                    {
                        if (num < 0)
                        {
                            ((IDisposable)this._S0).Dispose();
                        }
                    }
                    return false;
                }

                public IEnumerator<ModBase.SearchEntry<WikiEntry>> GetEnumerator()
                {
                    ResourceSearcher._Closure___.VB_StateMachine___Lambda___6_8 vb_StateMachine___Lambda___6_;
                    if (this._State == -2 && this._InitialThreadId == Environment.CurrentManagedThreadId)
                    {
                        this._State = 0;
                        vb_StateMachine___Lambda___6_ = this;
                    }
                    else
                    {
                        vb_StateMachine___Lambda___6_ = new ResourceSearcher._Closure___.VB_StateMachine___Lambda___6_8(0);
                        vb_StateMachine___Lambda___6_._VB_NonLocal__Closure___ = this._VB_NonLocal__Closure___;
                    }
                    vb_StateMachine___Lambda___6_._VB_Local_Source = this._P_Source;
                    return vb_StateMachine___Lambda___6_;
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                ModBase.SearchEntry<WikiEntry> IEnumerator<ModBase.SearchEntry<WikiEntry>>.Current
                {
                    get
                    {
                        return this._Current;
                    }
                }

                void IEnumerator.Reset()
                {
                    throw new NotSupportedException();
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return this._Current;
                    }
                }

                public int _State;

                public ModBase.SearchEntry<WikiEntry> _Current;

                public int _InitialThreadId;

                internal ResourcePlatforms _VB_Local_Source;

                internal ResourcePlatforms _P_Source;

                internal ResourceSearcher._Closure___ _VB_NonLocal__Closure___;

                internal List<WikiEntry>.Enumerator _S0;
            }
        }

        [CompilerGenerated]
        private sealed class _Closure___6_0
        {
            public _Closure___6_0(ResourceSearcher._Closure___6_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Candidates = arg0._VB_Local_Candidates;
                    this._VB_Local_CanForm = arg0._VB_Local_CanForm;
                }
            }

            internal bool _Lambda___4(string s)
            {
                ResourceSearcher._Closure___6_1 CS_locals1 = new ResourceSearcher._Closure___6_1(null);
                CS_locals1._VB_NonLocal__VB_Closure_2 = this;
                CS_locals1._VB_Local_s = s;
                return this._VB_Local_Candidates.Contains(CS_locals1._VB_Local_s) || this._VB_Local_Candidates.Any<string>(new Func<string, bool>(CS_locals1._Lambda___5));
            }

            internal bool _Lambda___6(string w)
            {
                ResourceSearcher._Closure___6_2 CS_locals1 = new ResourceSearcher._Closure___6_2(null);
                CS_locals1._VB_NonLocal__VB_Closure_3 = this;
                CS_locals1._VB_Local_w = w;
                return !this._VB_Local_Candidates.Any<string>(new Func<string, bool>(CS_locals1._Lambda___7));
            }

            public List<string> _VB_Local_Candidates;

            public Func<string, bool> _VB_Local_CanForm;
        }

        [CompilerGenerated]
        private sealed class _Closure___6_1
        {
            public _Closure___6_1(ResourceSearcher._Closure___6_1 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_s = arg0._VB_Local_s;
                }
            }

            internal bool _Lambda___5(string c)
            {
                return this._VB_Local_s.StartsWith(c) && this._VB_NonLocal__VB_Closure_2._VB_Local_CanForm(this._VB_Local_s.Substring(c.Length));
            }

            public string _VB_Local_s;

            public ResourceSearcher._Closure___6_0 _VB_NonLocal__VB_Closure_2;
        }

        [CompilerGenerated]
        private sealed class _Closure___6_2
        {
            public _Closure___6_2(ResourceSearcher._Closure___6_2 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_w = arg0._VB_Local_w;
                }
            }

            internal bool _Lambda___7(string c)
            {
                return c.Length < this._VB_Local_w.Length && this._VB_Local_w.StartsWith(c) && this._VB_NonLocal__VB_Closure_3._VB_Local_CanForm(this._VB_Local_w.Substring(c.Length));
            }

            public string _VB_Local_w;

            public ResourceSearcher._Closure___6_0 _VB_NonLocal__VB_Closure_3;
        }

        [CompilerGenerated]
        private sealed class _Closure___6_3
        {
            public _Closure___6_3(ResourceSearcher._Closure___6_3 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_RawSearchText = arg0._VB_Local_RawSearchText;
                    this._VB_Local_Request = arg0._VB_Local_Request;
                    this._VB_Local_CurseForgeAltSearchText = arg0._VB_Local_CurseForgeAltSearchText;
                    this._VB_Local_IgnoreModLoaderFilter = arg0._VB_Local_IgnoreModLoaderFilter;
                    this._VB_Local_SearchResults = arg0._VB_Local_SearchResults;
                    this._VB_Local_Storage = arg0._VB_Local_Storage;
                    this._VB_Local_Errors = arg0._VB_Local_Errors;
                    this._VB_Local_Task = arg0._VB_Local_Task;
                    this._VB_Local_ModrinthAltSearchText = arg0._VB_Local_ModrinthAltSearchText;
                    this._VB_Local_ModrinthSlugs = arg0._VB_Local_ModrinthSlugs;
                    this._VB_Local_RealResults = arg0._VB_Local_RealResults;
                }
            }

            internal bool _Lambda___12(ModBase.SearchSource s)
            {
                return s.Aliases.Contains(this._VB_Local_RawSearchText);
            }

            internal void _Lambda___15()
            {
                try
                {
                    string curseForgeAddress = this._VB_Local_Request.GetCurseForgeAddress(this._VB_Local_CurseForgeAltSearchText ?? this._VB_Local_RawSearchText, this._VB_Local_IgnoreModLoaderFilter);
                    Logger.Info(string.Format("开始 CurseForge 搜索：{0}", curseForgeAddress), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                    JObject jobject = (JObject)ModDownload.DlModRequest(curseForgeAddress, null, null, null);
                    List<ResourceProject> list = new List<ResourceProject>();
                    try
                    {
                        foreach (JToken jtoken in ((IEnumerable<JToken>)jobject["data"]))
                        {
                            JObject jobject2 = (JObject)jtoken;
                            ResourceProject resourceProject = new ResourceProject(jobject2);
                            if ((this._VB_Local_Request.Type != ResourceTypes.ResourcePack || !resourceProject.Tags.Contains("数据包")) && (this._VB_Local_Request.Type == ResourceTypes.Any || resourceProject.Types.HasFlagF(this._VB_Local_Request.Type)))
                            {
                                list.Add(resourceProject);
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
                    list.ForEach((this._I16 == null) ? (this._I16 = new Action<ResourceProject>(this._Lambda___16)) : this._I16);
                    ResourceSearcher.SearchResult searchResult = this._VB_Local_Storage;
                    ref int ptr = ref searchResult.CurseForgeOffset;
                    searchResult.CurseForgeOffset = checked(ptr + jobject["data"].Count<JToken>());
                    this._VB_Local_Storage.CurseForgeTotal = jobject["pagination"]["totalCount"].ToObject<int>();
                    Logger.Info(string.Format("从 CurseForge 搜索到了 {0} 个工程（总计已获取 {1} 个，共 {2} 个）", list.Count, this._VB_Local_Storage.CurseForgeOffset, this._VB_Local_Storage.CurseForgeTotal), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "CurseForge 搜索失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                    this._VB_Local_Storage.CurseForgeTotal = -1;
                    this._VB_Local_Errors.Add(new ValueTuple<Exception, ResourcePlatforms>(ex, ResourcePlatforms.CurseForge));
                }
                if (this._VB_Local_Task.Progress < 0.75)
                {
                    ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int> loaderTask;
                    (loaderTask = this._VB_Local_Task).Progress = loaderTask.Progress + 0.25;
                }
            }

            internal void _Lambda___16(ResourceProject p)
            {
                this._VB_Local_SearchResults.Add(p);
            }

            internal void _Lambda___17()
            {
                try
                {
                    string modrinthAddress = this._VB_Local_Request.GetModrinthAddress(this._VB_Local_ModrinthAltSearchText ?? this._VB_Local_RawSearchText, this._VB_Local_IgnoreModLoaderFilter);
                    Logger.Info(string.Format("开始 Modrinth 搜索：{0}", modrinthAddress), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                    JObject jobject = (JObject)ModDownload.DlModRequest(modrinthAddress, null, null, null);
                    List<ResourceProject> list = new List<ResourceProject>();
                    try
                    {
                        foreach (JToken jtoken in ((IEnumerable<JToken>)jobject["hits"]))
                        {
                            JObject jobject2 = (JObject)jtoken;
                            list.Add(new ResourceProject(jobject2));
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
                    list.ForEach((this._I18 == null) ? (this._I18 = new Action<ResourceProject>(this._Lambda___18)) : this._I18);
                    ResourceSearcher.SearchResult searchResult = this._VB_Local_Storage;
                    ref int ptr = ref searchResult.ModrinthOffset;
                    searchResult.ModrinthOffset = checked(ptr + jobject["hits"].Count<JToken>());
                    this._VB_Local_Storage.ModrinthTotal = jobject["total_hits"].ToObject<int>();
                    Logger.Info(string.Format("从 Modrinth 搜索到了 {0} 个工程（总计已获取 {1} 个，共 {2} 个）", list.Count, this._VB_Local_Storage.ModrinthOffset, this._VB_Local_Storage.ModrinthTotal), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Modrinth 搜索失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                    this._VB_Local_Storage.ModrinthTotal = -1;
                    this._VB_Local_Errors.Add(new ValueTuple<Exception, ResourcePlatforms>(ex, ResourcePlatforms.Modrinth));
                }
                if (this._VB_Local_Task.Progress < 0.75)
                {
                    ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int> loaderTask;
                    (loaderTask = this._VB_Local_Task).Progress = loaderTask.Progress + 0.25;
                }
            }

            internal void _Lambda___18(ResourceProject p)
            {
                this._VB_Local_SearchResults.Add(p);
            }

            internal void _Lambda___19()
            {
                try
                {
                    string text = string.Format("https://api.modrinth.com/v2/projects?ids=[\"{0}\"]", this._VB_Local_ModrinthSlugs.Join("\",\""));
                    Logger.Info(string.Format("开始 Modrinth 直接获取：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                    List<ResourceProject> list = new List<ResourceProject>();
                    try
                    {
                        foreach (JToken jtoken in ((IEnumerable<JToken>)ModDownload.DlModRequest(text, null, null, null)))
                        {
                            JObject jobject = (JObject)jtoken;
                            ResourceProject resourceProject = new ResourceProject(jobject);
                            if ((this._VB_Local_Request.Type == ResourceTypes.Any || resourceProject.Types.HasFlagF(this._VB_Local_Request.Type)) && (string.IsNullOrEmpty(this._VB_Local_Request.Tag) || jobject["categories"].Any<JToken>((this._I20 == null) ? (this._I20 = new Func<JToken, bool>(this._Lambda___20)) : this._I20)) && (this._VB_Local_Request.ModLoaders == ModLoaders.None || this._VB_Local_IgnoreModLoaderFilter || resourceProject.ModLoaders.Flags<ModLoaders>().Intersect<ModLoaders>(this._VB_Local_Request.ModLoaders.Flags<ModLoaders>()).Any<ModLoaders>()) && (string.IsNullOrEmpty(this._VB_Local_Request.GameVersion) || resourceProject.UnsafeGameVersions.Any<string>((this._I21 == null) ? (this._I21 = new Func<string, bool>(this._Lambda___21)) : this._I21)))
                            {
                                list.Add(resourceProject);
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
                    if (list.Any<ResourceProject>() && this._VB_Local_Request.ModLoaders != ModLoaders.None && !this._VB_Local_IgnoreModLoaderFilter && !string.IsNullOrEmpty(this._VB_Local_Request.GameVersion))
                    {
                        ResourceSearcher._Closure___6_4 CS_locals1 = new ResourceSearcher._Closure___6_4(null);
                        string text2 = list.Select<ResourceProject, string>((ResourceSearcher._Closure___._I6_22 == null) ? (ResourceSearcher._Closure___._I6_22 = new Func<ResourceProject, string>(ResourceSearcher._Closure___._I._Lambda___6_22)) : ResourceSearcher._Closure___._I6_22).Join("\",\"project_id:");
                        string text3 = this._VB_Local_Request.ModLoaders.Flags<ModLoaders>().Select<ModLoaders, string>((ResourceSearcher._Closure___._I6_23 == null) ? (ResourceSearcher._Closure___._I6_23 = new Func<ModLoaders, string>(ResourceSearcher._Closure___._I._Lambda___6_23)) : ResourceSearcher._Closure___._I6_23).Join("\",\"categories:");
                        string text4 = string.Format("https://api.modrinth.com/v2/search?limit={0}&facets=", list.Count) + string.Format("[[\"project_id:{0}\"],[\"categories:{1}\"],[\"versions:{2}\"]]", text2, text3, this._VB_Local_Request.GameVersion);
                        CS_locals1._VB_Local_MatchedIds = ModDownload.DlModRequest(text4, null, null, null)["hits"].Select<JToken, string>((ResourceSearcher._Closure___._I6_24 == null) ? (ResourceSearcher._Closure___._I6_24 = new Func<JToken, string>(ResourceSearcher._Closure___._I._Lambda___6_24)) : ResourceSearcher._Closure___._I6_24).ToList<string>();
                        list.KeepIf<ResourceProject>(new Func<ResourceProject, bool>(CS_locals1._Lambda___25));
                    }
                    list.ForEach((this._I26 == null) ? (this._I26 = new Action<ResourceProject>(this._Lambda___26)) : this._I26);
                    Logger.Info(string.Format("从 Modrinth 直接获取到了 {0} 个工程", list.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                    this._VB_Local_ModrinthSlugs.Clear();
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Modrinth 直接获取失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                    this._VB_Local_Errors.Add(new ValueTuple<Exception, ResourcePlatforms>(ex, ResourcePlatforms.Modrinth));
                }
                if (this._VB_Local_Task.Progress < 0.75)
                {
                    ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int> loaderTask;
                    (loaderTask = this._VB_Local_Task).Progress = loaderTask.Progress + 0.25;
                }
            }

            internal bool _Lambda___20(JToken c)
            {
                return Operators.CompareString(c.ToString(), this._VB_Local_Request.Tag.AfterLast("/", false), false) == 0;
            }

            internal bool _Lambda___21(string d)
            {
                return Operators.CompareString(d, this._VB_Local_Request.GameVersion, false) == 0;
            }

            internal void _Lambda___26(ResourceProject p)
            {
                this._VB_Local_SearchResults.Add(p);
            }

            internal bool _Lambda___32(ResourceProject r)
            {
                ResourceSearcher._Closure___6_5 CS_locals1 = new ResourceSearcher._Closure___6_5(null);
                CS_locals1._VB_Local_r = r;
                return !this._VB_Local_RealResults.Any<ResourceProject>(new Func<ResourceProject, bool>(CS_locals1._Lambda___33)) && !this._VB_Local_Storage.Results.Any<ResourceProject>(new Func<ResourceProject, bool>(CS_locals1._Lambda___34));
            }

            internal double _Lambda___35(ResourceProject Project)
            {
                ResourceTypes type = this._VB_Local_Request.Type;
                if (type <= ResourceTypes.ResourcePack)
                {
                    if (type == ResourceTypes.Mod || type == ResourceTypes.ModPack)
                    {
                        return (double)((Project.Platform == ResourcePlatforms.CurseForge) ? 1 : 5);
                    }
                    if (type != ResourceTypes.ResourcePack)
                    {
                        goto IL_005B;
                    }
                }
                else if (type != ResourceTypes.Shader)
                {
                    if (type != ResourceTypes.DataPack)
                    {
                        goto IL_005B;
                    }
                    return (double)((Project.Platform == ResourcePlatforms.CurseForge) ? 10 : 1);
                }
                return (double)((Project.Platform == ResourcePlatforms.CurseForge) ? 1 : 4);
                IL_005B:
                return 1.0;
            }

            public string _VB_Local_RawSearchText;

            public ResourceSearcher.SearchRequest _VB_Local_Request;

            public string _VB_Local_CurseForgeAltSearchText;

            public bool _VB_Local_IgnoreModLoaderFilter;

            public ConcurrentBag<ResourceProject> _VB_Local_SearchResults;

            public ResourceSearcher.SearchResult _VB_Local_Storage;

            public ConcurrentBag<ValueTuple<Exception, ResourcePlatforms>> _VB_Local_Errors;

            public ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int> _VB_Local_Task;

            public string _VB_Local_ModrinthAltSearchText;

            public List<string> _VB_Local_ModrinthSlugs;

            public List<ResourceProject> _VB_Local_RealResults;

            public Func<ModBase.SearchSource, bool> _I12;

            public Action<ResourceProject> _I16;

            public Action<ResourceProject> _I18;

            public Func<JToken, bool> _I20;

            public Func<string, bool> _I21;

            public Action<ResourceProject> _I26;
        }

        [CompilerGenerated]
        private sealed class _Closure___6_4
        {
            public _Closure___6_4(ResourceSearcher._Closure___6_4 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_MatchedIds = arg0._VB_Local_MatchedIds;
                }
            }

            internal bool _Lambda___25(ResourceProject p)
            {
                return this._VB_Local_MatchedIds.Contains(p.Id);
            }

            public List<string> _VB_Local_MatchedIds;
        }

        [CompilerGenerated]
        private sealed class _Closure___6_5
        {
            public _Closure___6_5(ResourceSearcher._Closure___6_5 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_r = arg0._VB_Local_r;
                }
            }

            internal bool _Lambda___33(ResourceProject b)
            {
                return this._VB_Local_r.IsLike(b);
            }

            internal bool _Lambda___34(ResourceProject b)
            {
                return this._VB_Local_r.IsLike(b);
            }

            public ResourceProject _VB_Local_r;
        }
    }
}
