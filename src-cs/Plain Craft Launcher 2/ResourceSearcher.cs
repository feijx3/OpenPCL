using System;
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
        public static ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int> GetLoader(Func<ResourceSearcher.SearchRequest> InputDelegate)
        {
            return new ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int>("社区资源获取", new Action<ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int>>(ResourceSearcher.Run), InputDelegate, ThreadPriority.Normal)
            {
                ReloadTimeout = 60000
            };
        }

        private static void Run(ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int> Task)
        {
            ResourceSearcher._Closure$__6-3 CS$<>8__locals1 = new ResourceSearcher._Closure$__6-3(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_Task = Task;
            CS$<>8__locals1.$VB$Local_Request = CS$<>8__locals1.$VB$Local_Task.Input;
            CS$<>8__locals1.$VB$Local_Storage = CS$<>8__locals1.$VB$Local_Request.Storage;
            if (CS$<>8__locals1.$VB$Local_Storage.Results.Count >= CS$<>8__locals1.$VB$Local_Request.TargetResultCount)
            {
                Logger.Info(string.Format("已有 {0} 个结果，多于所需的 {1} 个结果，结束处理", CS$<>8__locals1.$VB$Local_Storage.Results.Count, CS$<>8__locals1.$VB$Local_Request.TargetResultCount), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                return;
            }
            if (CS$<>8__locals1.$VB$Local_Request.CanContinue)
            {
                CS$<>8__locals1.$VB$Local_RawSearchText = (CS$<>8__locals1.$VB$Local_Request.SearchText ?? "").Trim();
                CS$<>8__locals1.$VB$Local_RawSearchText = CS$<>8__locals1.$VB$Local_RawSearchText.Lower();
                Logger.Info(string.Format("工程列表搜索原始文本：{0}", CS$<>8__locals1.$VB$Local_RawSearchText), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                CS$<>8__locals1.$VB$Local_RawSearchText = Strings.StrConv(CS$<>8__locals1.$VB$Local_RawSearchText, VbStrConv.SimplifiedChinese, 0);
                bool flag = CS$<>8__locals1.$VB$Local_RawSearchText.RegexCheck("[\\u4e00-\\u9fbb]", RegexOptions.None) && !string.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_RawSearchText) && (CS$<>8__locals1.$VB$Local_Request.Type == ResourceTypes.Mod || CS$<>8__locals1.$VB$Local_Request.Type == ResourceTypes.DataPack);
                CS$<>8__locals1.$VB$Local_CurseForgeAltSearchText = null;
                CS$<>8__locals1.$VB$Local_ModrinthAltSearchText = null;
                CS$<>8__locals1.$VB$Local_ModrinthSlugs = new List<string>();
                if (flag)
                {
                    VB$AnonymousDelegate_2<ModBase.SearchEntry<WikiEntry>, ResourcePlatforms, IEnumerable<string>> vb$AnonymousDelegate_ = ((ResourceSearcher._Closure$__.$I6-0 == null) ? (ResourceSearcher._Closure$__.$I6-0 = delegate(ModBase.SearchEntry<WikiEntry> Result, ResourcePlatforms Source)
                    {
                        ResourceSearcher._Closure$__6-0 CS$<>8__locals2 = new ResourceSearcher._Closure$__6-0(CS$<>8__locals2);
                        CS$<>8__locals2.$VB$Local_Candidates = new List<string>();
                        if (Result.Item.Slugs.ContainsKey(Source))
                        {
                            CS$<>8__locals2.$VB$Local_Candidates.Add(Result.Item.Slugs[Source].Replace("-", " ").Replace("/", " "));
                        }
                        if (Result.Item.ChineseName != null)
                        {
                            CS$<>8__locals2.$VB$Local_Candidates.Add(Result.Item.ChineseName.AfterLast(" (", false).TrimEnd(new char[] { ')' }).BeforeFirst(" - ", false)
                                .Replace("-", " ")
                                .Replace("/", " ")
                                .Replace(":", " ")
                                .Replace("(", " ")
                                .Replace(")", ""));
                        }
                        CS$<>8__locals2.$VB$Local_Candidates = CS$<>8__locals2.$VB$Local_Candidates.SelectMany<string, string>((ResourceSearcher._Closure$__.$I6-1 == null) ? (ResourceSearcher._Closure$__.$I6-1 = (string c) => c.Split(" ", false)) : ResourceSearcher._Closure$__.$I6-1).Select<string, string>((ResourceSearcher._Closure$__.$I6-2 == null) ? (ResourceSearcher._Closure$__.$I6-2 = (string w) => w.TrimStart(new char[] { '{' }).TrimEnd(new char[] { '}' }).Lower()) : ResourceSearcher._Closure$__.$I6-2).Where<string>((ResourceSearcher._Closure$__.$I6-3 == null) ? (ResourceSearcher._Closure$__.$I6-3 = (string w) => w.Length > 1 && !new string[] { "the", "of", "mod", "and", "forge", "fabric", "for", "quilt", "neoforge" }.Contains(w) && ModBase.Val(w) <= 0.0 && w.IsAsciiOnly()) : ResourceSearcher._Closure$__.$I6-3)
                            .Distinct<string>()
                            .ToList<string>();
                        CS$<>8__locals2.$VB$Local_CanForm = null;
                        CS$<>8__locals2.$VB$Local_CanForm = delegate(string s)
                        {
                            ResourceSearcher._Closure$__6-1 CS$<>8__locals3 = new ResourceSearcher._Closure$__6-1(CS$<>8__locals3);
                            CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals2;
                            CS$<>8__locals3.$VB$Local_s = s;
                            return CS$<>8__locals2.$VB$Local_Candidates.Contains(CS$<>8__locals3.$VB$Local_s) || CS$<>8__locals2.$VB$Local_Candidates.Any<string>((string c) => CS$<>8__locals3.$VB$Local_s.StartsWith(c) && CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_CanForm(CS$<>8__locals3.$VB$Local_s.Substring(c.Length)));
                        };
                        CS$<>8__locals2.$VB$Local_Candidates = CS$<>8__locals2.$VB$Local_Candidates.Where<string>(delegate(string w)
                        {
                            ResourceSearcher._Closure$__6-2 CS$<>8__locals4 = new ResourceSearcher._Closure$__6-2(CS$<>8__locals4);
                            CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3 = CS$<>8__locals2;
                            CS$<>8__locals4.$VB$Local_w = w;
                            return !CS$<>8__locals2.$VB$Local_Candidates.Any<string>((string c) => c.Length < CS$<>8__locals4.$VB$Local_w.Length && CS$<>8__locals4.$VB$Local_w.StartsWith(c) && CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_CanForm(CS$<>8__locals4.$VB$Local_w.Substring(c.Length)));
                        }).ToList<string>();
                        return CS$<>8__locals2.$VB$Local_Candidates;
                    }) : ResourceSearcher._Closure$__.$I6-0);
                    VB$AnonymousDelegate_3<ResourcePlatforms, IEnumerable<ModBase.SearchEntry<WikiEntry>>> vb$AnonymousDelegate_2 = ((ResourceSearcher._Closure$__.$I6-8 == null) ? (ResourceSearcher._Closure$__.$I6-8 = delegate(ResourcePlatforms Source)
                    {
                        ResourceSearcher._Closure$__.VB$StateMachine___Lambda$__6-8 vb$StateMachine___Lambda$__6- = new ResourceSearcher._Closure$__.VB$StateMachine___Lambda$__6-8(-2);
                        vb$StateMachine___Lambda$__6-.$VB$NonLocal__Closure$__ = ResourceSearcher._Closure$__.$I;
                        vb$StateMachine___Lambda$__6-.$P_Source = Source;
                        return vb$StateMachine___Lambda$__6-;
                    }) : ResourceSearcher._Closure$__.$I6-8);
                    if (CS$<>8__locals1.$VB$Local_Request.Sources.HasFlagF(ResourcePlatforms.CurseForge))
                    {
                        if (ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init == null)
                        {
                            Interlocked.CompareExchange<StaticLocalInitFlag>(ref ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init, new StaticLocalInitFlag(), null);
                        }
                        bool flag2 = false;
                        try
                        {
                            Monitor.Enter(ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init, ref flag2);
                            if (ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init.State == 0)
                            {
                                ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init.State = 2;
                                ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries = vb$AnonymousDelegate_2(ResourcePlatforms.CurseForge).ToList<ModBase.SearchEntry<WikiEntry>>();
                            }
                            else if (ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init.State == 2)
                            {
                                throw new IncompleteInitialization();
                            }
                        }
                        finally
                        {
                            ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init.State = 1;
                            if (flag2)
                            {
                                Monitor.Exit(ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init);
                            }
                        }
                        List<ModBase.SearchEntry<WikiEntry>> list = ModBase.Search<WikiEntry>(ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries, CS$<>8__locals1.$VB$Local_RawSearchText, 100, 0.25);
                        if (list.Any<ModBase.SearchEntry<WikiEntry>>())
                        {
                            ModBase.SearchEntry<WikiEntry> searchEntry = (list.First<ModBase.SearchEntry<WikiEntry>>().AbsoluteRight ? list.Where<ModBase.SearchEntry<WikiEntry>>((ResourceSearcher._Closure$__.$I6-9 == null) ? (ResourceSearcher._Closure$__.$I6-9 = (ModBase.SearchEntry<WikiEntry> s) => s.AbsoluteRight) : ResourceSearcher._Closure$__.$I6-9).ToList<ModBase.SearchEntry<WikiEntry>>() : list.MaxByAll<ModBase.SearchEntry<WikiEntry>, double>((ResourceSearcher._Closure$__.$I6-10 == null) ? (ResourceSearcher._Closure$__.$I6-10 = (ModBase.SearchEntry<WikiEntry> s) => s.Similarity) : ResourceSearcher._Closure$__.$I6-10)).MaxBy<ModBase.SearchEntry<WikiEntry>, int>((ResourceSearcher._Closure$__.$I6-11 == null) ? (ResourceSearcher._Closure$__.$I6-11 = (ModBase.SearchEntry<WikiEntry> s) => s.Item.Popularity) : ResourceSearcher._Closure$__.$I6-11);
                            CS$<>8__locals1.$VB$Local_CurseForgeAltSearchText = vb$AnonymousDelegate_(searchEntry, ResourcePlatforms.CurseForge).Join(" ");
                            Logger.Warn(string.Format("中文搜索关键词（CurseForge）：{0}", CS$<>8__locals1.$VB$Local_CurseForgeAltSearchText), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                        }
                    }
                    if (CS$<>8__locals1.$VB$Local_Request.Sources.HasFlagF(ResourcePlatforms.Modrinth))
                    {
                        if (ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init == null)
                        {
                            Interlocked.CompareExchange<StaticLocalInitFlag>(ref ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init, new StaticLocalInitFlag(), null);
                        }
                        bool flag3 = false;
                        try
                        {
                            Monitor.Enter(ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init, ref flag3);
                            if (ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init.State == 0)
                            {
                                ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init.State = 2;
                                ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries = vb$AnonymousDelegate_2(ResourcePlatforms.Modrinth).ToList<ModBase.SearchEntry<WikiEntry>>();
                            }
                            else if (ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init.State == 2)
                            {
                                throw new IncompleteInitialization();
                            }
                        }
                        finally
                        {
                            ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init.State = 1;
                            if (flag3)
                            {
                                Monitor.Exit(ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init);
                            }
                        }
                        List<ModBase.SearchEntry<WikiEntry>> list2 = ModBase.Search<WikiEntry>(ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries, CS$<>8__locals1.$VB$Local_RawSearchText, 100, 0.25);
                        if (list2.Any<ModBase.SearchEntry<WikiEntry>>())
                        {
                            Dictionary<string, double> dictionary = new Dictionary<string, double>();
                            try
                            {
                                foreach (ModBase.SearchEntry<WikiEntry> searchEntry2 in list2)
                                {
                                    try
                                    {
                                        foreach (string text in vb$AnonymousDelegate_(searchEntry2, ResourcePlatforms.Modrinth))
                                        {
                                            if (!dictionary.ContainsKey(text))
                                            {
                                                dictionary.Add(text, 0.0);
                                            }
                                            double num = (searchEntry2.SearchSource.Any<ModBase.SearchSource>((CS$<>8__locals1.$I12 == null) ? (CS$<>8__locals1.$I12 = (ModBase.SearchSource s) => s.Aliases.Contains(CS$<>8__locals1.$VB$Local_RawSearchText)) : CS$<>8__locals1.$I12) ? 1000.0 : searchEntry2.Similarity);
                                            Dictionary<string, double> dictionary2;
                                            string text2;
                                            (dictionary2 = dictionary)[text2 = text] = dictionary2[text2] + num * (double)searchEntry2.Item.Popularity;
                                        }
                                    }
                                    finally
                                    {
                                        IEnumerator<string> enumerator2;
                                        if (enumerator2 != null)
                                        {
                                            enumerator2.Dispose();
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                List<ModBase.SearchEntry<WikiEntry>>.Enumerator enumerator;
                                ((IDisposable)enumerator).Dispose();
                            }
                            CS$<>8__locals1.$VB$Local_ModrinthAltSearchText = dictionary.MaxBy<KeyValuePair<string, double>, double>((ResourceSearcher._Closure$__.$I6-13 == null) ? (ResourceSearcher._Closure$__.$I6-13 = (KeyValuePair<string, double> w) => w.Value) : ResourceSearcher._Closure$__.$I6-13).Key;
                            Logger.Warn(string.Format("中文搜索关键词（Modrinth）：{0}", CS$<>8__locals1.$VB$Local_ModrinthAltSearchText), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                            CS$<>8__locals1.$VB$Local_ModrinthSlugs = list2.Take<ModBase.SearchEntry<WikiEntry>>(100).Select<ModBase.SearchEntry<WikiEntry>, string>((ResourceSearcher._Closure$__.$I6-14 == null) ? (ResourceSearcher._Closure$__.$I6-14 = (ModBase.SearchEntry<WikiEntry> r) => r.Item.Slugs[ResourcePlatforms.Modrinth]) : ResourceSearcher._Closure$__.$I6-14).ToList<string>();
                        }
                    }
                    if (string.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_CurseForgeAltSearchText) && string.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_ModrinthAltSearchText) && !CS$<>8__locals1.$VB$Local_ModrinthSlugs.Any<string>())
                    {
                        throw new Exception("无搜索结果，请尝试搜索其英文名称");
                    }
                }
                CS$<>8__locals1.$VB$Local_Task.Progress = 0.05;
                CS$<>8__locals1.$VB$Local_RealResults = new List<ResourceProject>();
                for (;;)
                {
                    List<ResourceProject> list3 = new List<ResourceProject>();
                    List<Thread> list4 = new List<Thread>();
                    CS$<>8__locals1.$VB$Local_Errors = new ConcurrentBag<ValueTuple<Exception, ResourcePlatforms>>();
                    CS$<>8__locals1.$VB$Local_IgnoreModLoaderFilter = CS$<>8__locals1.$VB$Local_Request.ModLoaders == ModLoaders.Forge && McVersion.VersionToDrop(CS$<>8__locals1.$VB$Local_Request.GameVersion) < 140;
                    CS$<>8__locals1.$VB$Local_SearchResults = new ConcurrentBag<ResourceProject>();
                    try
                    {
                        if (CS$<>8__locals1.$VB$Local_Request.Sources.HasFlagF(ResourcePlatforms.CurseForge) && (CS$<>8__locals1.$VB$Local_Storage.CurseForgeTotal <= -1 || CS$<>8__locals1.$VB$Local_Storage.CurseForgeTotal > CS$<>8__locals1.$VB$Local_Storage.CurseForgeOffset) && (!flag || (flag && !string.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_CurseForgeAltSearchText))))
                        {
                            list4.Add(ModBase.RunInNewThread(delegate
                            {
                                try
                                {
                                    string curseForgeAddress = CS$<>8__locals1.$VB$Local_Request.GetCurseForgeAddress(CS$<>8__locals1.$VB$Local_CurseForgeAltSearchText ?? CS$<>8__locals1.$VB$Local_RawSearchText, CS$<>8__locals1.$VB$Local_IgnoreModLoaderFilter);
                                    Logger.Info(string.Format("开始 CurseForge 搜索：{0}", curseForgeAddress), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                                    JObject jobject = (JObject)ModDownload.DlModRequest(curseForgeAddress, null, null, null);
                                    List<ResourceProject> list7 = new List<ResourceProject>();
                                    try
                                    {
                                        foreach (JToken jtoken in jobject["data"])
                                        {
                                            JObject jobject2 = (JObject)jtoken;
                                            ResourceProject resourceProject3 = new ResourceProject(jobject2);
                                            if ((CS$<>8__locals1.$VB$Local_Request.Type != ResourceTypes.ResourcePack || !resourceProject3.Tags.Contains("数据包")) && (CS$<>8__locals1.$VB$Local_Request.Type == ResourceTypes.Any || resourceProject3.Types.HasFlagF(CS$<>8__locals1.$VB$Local_Request.Type)))
                                            {
                                                list7.Add(resourceProject3);
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
                                    list7.ForEach((CS$<>8__locals1.$I16 == null) ? (CS$<>8__locals1.$I16 = delegate(ResourceProject p)
                                    {
                                        CS$<>8__locals1.$VB$Local_SearchResults.Add(p);
                                    }) : CS$<>8__locals1.$I16);
                                    ResourceSearcher.SearchResult $VB$Local_Storage = CS$<>8__locals1.$VB$Local_Storage;
                                    ref int ptr = ref $VB$Local_Storage.CurseForgeOffset;
                                    $VB$Local_Storage.CurseForgeOffset = checked(ptr + jobject["data"].Count<JToken>());
                                    CS$<>8__locals1.$VB$Local_Storage.CurseForgeTotal = jobject["pagination"]["totalCount"].ToObject<int>();
                                    Logger.Info(string.Format("从 CurseForge 搜索到了 {0} 个工程（总计已获取 {1} 个，共 {2} 个）", list7.Count, CS$<>8__locals1.$VB$Local_Storage.CurseForgeOffset, CS$<>8__locals1.$VB$Local_Storage.CurseForgeTotal), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                                }
                                catch (Exception ex)
                                {
                                    Logger.Warn(ex, "CurseForge 搜索失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                                    CS$<>8__locals1.$VB$Local_Storage.CurseForgeTotal = -1;
                                    CS$<>8__locals1.$VB$Local_Errors.Add(new ValueTuple<Exception, ResourcePlatforms>(ex, ResourcePlatforms.CurseForge));
                                }
                                if (CS$<>8__locals1.$VB$Local_Task.Progress < 0.75)
                                {
                                    ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int> $VB$Local_Task;
                                    ($VB$Local_Task = CS$<>8__locals1.$VB$Local_Task).Progress = $VB$Local_Task.Progress + 0.25;
                                }
                            }, "CurseForge Search", ThreadPriority.Normal));
                        }
                        if (CS$<>8__locals1.$VB$Local_Request.Sources.HasFlagF(ResourcePlatforms.Modrinth) && (CS$<>8__locals1.$VB$Local_Storage.ModrinthTotal <= -1 || CS$<>8__locals1.$VB$Local_Storage.ModrinthTotal > CS$<>8__locals1.$VB$Local_Storage.ModrinthOffset) && (!flag || (flag && !string.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_ModrinthAltSearchText))))
                        {
                            list4.Add(ModBase.RunInNewThread(delegate
                            {
                                try
                                {
                                    string modrinthAddress = CS$<>8__locals1.$VB$Local_Request.GetModrinthAddress(CS$<>8__locals1.$VB$Local_ModrinthAltSearchText ?? CS$<>8__locals1.$VB$Local_RawSearchText, CS$<>8__locals1.$VB$Local_IgnoreModLoaderFilter);
                                    Logger.Info(string.Format("开始 Modrinth 搜索：{0}", modrinthAddress), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                                    JObject jobject3 = (JObject)ModDownload.DlModRequest(modrinthAddress, null, null, null);
                                    List<ResourceProject> list8 = new List<ResourceProject>();
                                    try
                                    {
                                        foreach (JToken jtoken2 in jobject3["hits"])
                                        {
                                            JObject jobject4 = (JObject)jtoken2;
                                            list8.Add(new ResourceProject(jobject4));
                                        }
                                    }
                                    finally
                                    {
                                        IEnumerator<JToken> enumerator9;
                                        if (enumerator9 != null)
                                        {
                                            enumerator9.Dispose();
                                        }
                                    }
                                    list8.ForEach((CS$<>8__locals1.$I18 == null) ? (CS$<>8__locals1.$I18 = delegate(ResourceProject p)
                                    {
                                        CS$<>8__locals1.$VB$Local_SearchResults.Add(p);
                                    }) : CS$<>8__locals1.$I18);
                                    ResourceSearcher.SearchResult $VB$Local_Storage2 = CS$<>8__locals1.$VB$Local_Storage;
                                    ref int ptr2 = ref $VB$Local_Storage2.ModrinthOffset;
                                    $VB$Local_Storage2.ModrinthOffset = checked(ptr2 + jobject3["hits"].Count<JToken>());
                                    CS$<>8__locals1.$VB$Local_Storage.ModrinthTotal = jobject3["total_hits"].ToObject<int>();
                                    Logger.Info(string.Format("从 Modrinth 搜索到了 {0} 个工程（总计已获取 {1} 个，共 {2} 个）", list8.Count, CS$<>8__locals1.$VB$Local_Storage.ModrinthOffset, CS$<>8__locals1.$VB$Local_Storage.ModrinthTotal), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                                }
                                catch (Exception ex2)
                                {
                                    Logger.Warn(ex2, "Modrinth 搜索失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                                    CS$<>8__locals1.$VB$Local_Storage.ModrinthTotal = -1;
                                    CS$<>8__locals1.$VB$Local_Errors.Add(new ValueTuple<Exception, ResourcePlatforms>(ex2, ResourcePlatforms.Modrinth));
                                }
                                if (CS$<>8__locals1.$VB$Local_Task.Progress < 0.75)
                                {
                                    ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int> $VB$Local_Task2;
                                    ($VB$Local_Task2 = CS$<>8__locals1.$VB$Local_Task).Progress = $VB$Local_Task2.Progress + 0.25;
                                }
                            }, "Modrinth Search", ThreadPriority.Normal));
                        }
                        if (CS$<>8__locals1.$VB$Local_Request.Sources.HasFlagF(ResourcePlatforms.Modrinth) && (CS$<>8__locals1.$VB$Local_Storage.ModrinthTotal <= -1 || CS$<>8__locals1.$VB$Local_Storage.ModrinthTotal > CS$<>8__locals1.$VB$Local_Storage.ModrinthOffset) && CS$<>8__locals1.$VB$Local_ModrinthSlugs.Any<string>())
                        {
                            list4.Add(ModBase.RunInNewThread(delegate
                            {
                                try
                                {
                                    string text3 = string.Format("https://api.modrinth.com/v2/projects?ids=[\"{0}\"]", CS$<>8__locals1.$VB$Local_ModrinthSlugs.Join("\",\""));
                                    Logger.Info(string.Format("开始 Modrinth 直接获取：{0}", text3), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                                    List<ResourceProject> list9 = new List<ResourceProject>();
                                    try
                                    {
                                        foreach (JToken jtoken3 in ModDownload.DlModRequest(text3, null, null, null))
                                        {
                                            JObject jobject5 = (JObject)jtoken3;
                                            ResourceProject resourceProject4 = new ResourceProject(jobject5);
                                            if ((CS$<>8__locals1.$VB$Local_Request.Type == ResourceTypes.Any || resourceProject4.Types.HasFlagF(CS$<>8__locals1.$VB$Local_Request.Type)) && (string.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_Request.Tag) || jobject5["categories"].Any<JToken>((CS$<>8__locals1.$I20 == null) ? (CS$<>8__locals1.$I20 = (JToken c) => Operators.CompareString(c.ToString(), CS$<>8__locals1.$VB$Local_Request.Tag.AfterLast("/", false), false) == 0) : CS$<>8__locals1.$I20)) && (CS$<>8__locals1.$VB$Local_Request.ModLoaders == ModLoaders.None || CS$<>8__locals1.$VB$Local_IgnoreModLoaderFilter || resourceProject4.ModLoaders.Flags<ModLoaders>().Intersect<ModLoaders>(CS$<>8__locals1.$VB$Local_Request.ModLoaders.Flags<ModLoaders>()).Any<ModLoaders>()) && (string.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_Request.GameVersion) || resourceProject4.UnsafeGameVersions.Any<string>((CS$<>8__locals1.$I21 == null) ? (CS$<>8__locals1.$I21 = (string d) => Operators.CompareString(d, CS$<>8__locals1.$VB$Local_Request.GameVersion, false) == 0) : CS$<>8__locals1.$I21)))
                                            {
                                                list9.Add(resourceProject4);
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
                                    if (list9.Any<ResourceProject>() && CS$<>8__locals1.$VB$Local_Request.ModLoaders != ModLoaders.None && !CS$<>8__locals1.$VB$Local_IgnoreModLoaderFilter && !string.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_Request.GameVersion))
                                    {
                                        ResourceSearcher._Closure$__6-4 CS$<>8__locals5 = new ResourceSearcher._Closure$__6-4(CS$<>8__locals5);
                                        string text4 = list9.Select<ResourceProject, string>((ResourceSearcher._Closure$__.$I6-22 == null) ? (ResourceSearcher._Closure$__.$I6-22 = (ResourceProject p) => p.Id) : ResourceSearcher._Closure$__.$I6-22).Join("\",\"project_id:");
                                        string text5 = CS$<>8__locals1.$VB$Local_Request.ModLoaders.Flags<ModLoaders>().Select<ModLoaders, string>((ResourceSearcher._Closure$__.$I6-23 == null) ? (ResourceSearcher._Closure$__.$I6-23 = (ModLoaders f) => f.ToString().Lower()) : ResourceSearcher._Closure$__.$I6-23).Join("\",\"categories:");
                                        string text6 = string.Format("https://api.modrinth.com/v2/search?limit={0}&facets=", list9.Count) + string.Format("[[\"project_id:{0}\"],[\"categories:{1}\"],[\"versions:{2}\"]]", text4, text5, CS$<>8__locals1.$VB$Local_Request.GameVersion);
                                        CS$<>8__locals5.$VB$Local_MatchedIds = ModDownload.DlModRequest(text6, null, null, null)["hits"].Select<JToken, string>((ResourceSearcher._Closure$__.$I6-24 == null) ? (ResourceSearcher._Closure$__.$I6-24 = (JToken h) => h["project_id"].ToString()) : ResourceSearcher._Closure$__.$I6-24).ToList<string>();
                                        list9.KeepIf<ResourceProject>((ResourceProject p) => CS$<>8__locals5.$VB$Local_MatchedIds.Contains(p.Id));
                                    }
                                    list9.ForEach((CS$<>8__locals1.$I26 == null) ? (CS$<>8__locals1.$I26 = delegate(ResourceProject p)
                                    {
                                        CS$<>8__locals1.$VB$Local_SearchResults.Add(p);
                                    }) : CS$<>8__locals1.$I26);
                                    Logger.Info(string.Format("从 Modrinth 直接获取到了 {0} 个工程", list9.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                                    CS$<>8__locals1.$VB$Local_ModrinthSlugs.Clear();
                                }
                                catch (Exception ex3)
                                {
                                    Logger.Warn(ex3, "Modrinth 直接获取失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                                    CS$<>8__locals1.$VB$Local_Errors.Add(new ValueTuple<Exception, ResourcePlatforms>(ex3, ResourcePlatforms.Modrinth));
                                }
                                if (CS$<>8__locals1.$VB$Local_Task.Progress < 0.75)
                                {
                                    ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int> $VB$Local_Task3;
                                    ($VB$Local_Task3 = CS$<>8__locals1.$VB$Local_Task).Progress = $VB$Local_Task3.Progress + 0.25;
                                }
                            }, "Modrinth Get", ThreadPriority.Normal));
                        }
                        try
                        {
                            foreach (Thread thread in list4)
                            {
                                thread.Join();
                                if (CS$<>8__locals1.$VB$Local_Task.IsCanceled)
                                {
                                    return;
                                }
                            }
                        }
                        finally
                        {
                            List<Thread>.Enumerator enumerator3;
                            ((IDisposable)enumerator3).Dispose();
                        }
                        list3 = new List<ResourceProject>(CS$<>8__locals1.$VB$Local_SearchResults);
                        if (CS$<>8__locals1.$VB$Local_IgnoreModLoaderFilter)
                        {
                            list3.KeepIf<ResourceProject>((ResourceSearcher._Closure$__.$I6-27 == null) ? (ResourceSearcher._Closure$__.$I6-27 = (ResourceProject p) => p.ModLoaders == ModLoaders.None || p.ModLoaders.HasFlagF(ModLoaders.Forge)) : ResourceSearcher._Closure$__.$I6-27);
                        }
                        CS$<>8__locals1.$VB$Local_Storage.ErrorMessage = null;
                        if (!list3.Any<ResourceProject>())
                        {
                            if (CS$<>8__locals1.$VB$Local_Errors.Any<ValueTuple<Exception, ResourcePlatforms>>())
                            {
                                throw CS$<>8__locals1.$VB$Local_Errors.First<ValueTuple<Exception, ResourcePlatforms>>().Item1;
                            }
                            if (flag && CS$<>8__locals1.$VB$Local_Request.Type != ResourceTypes.Mod && CS$<>8__locals1.$VB$Local_Request.Type != ResourceTypes.DataPack)
                            {
                                throw new Exception("无搜索结果，请尝试搜索其英文名称");
                            }
                            if (CS$<>8__locals1.$VB$Local_Request.Sources == ResourcePlatforms.CurseForge && CS$<>8__locals1.$VB$Local_Request.Tag.StartsWithF("/", false))
                            {
                                throw new Exception("CurseForge 不兼容所选的类型");
                            }
                            if (CS$<>8__locals1.$VB$Local_Request.Sources == ResourcePlatforms.Modrinth && CS$<>8__locals1.$VB$Local_Request.Tag.EndsWithF("/", false))
                            {
                                throw new Exception("Modrinth 不兼容所选的类型");
                            }
                            throw new Exception("没有搜索结果");
                        }
                        else if (CS$<>8__locals1.$VB$Local_Errors.Any<ValueTuple<Exception, ResourcePlatforms>>())
                        {
                            if (CS$<>8__locals1.$VB$Local_Errors.Any<ValueTuple<Exception, ResourcePlatforms>>((ResourceSearcher._Closure$__.$I6-28 == null) ? (ResourceSearcher._Closure$__.$I6-28 = ([TupleElementNames(new string[] { "Ex", "Source" })] ValueTuple<Exception, ResourcePlatforms> e) => e.Item2 == ResourcePlatforms.CurseForge) : ResourceSearcher._Closure$__.$I6-28))
                            {
                                CS$<>8__locals1.$VB$Local_Storage.ErrorMessage = string.Format("无法连接到 CurseForge，所以目前仅显示了来自 Modrinth 的内容，搜索结果可能不全。{0}请稍后再试，或使用 VPN 改善网络环境。", "\r\n");
                            }
                            else
                            {
                                CS$<>8__locals1.$VB$Local_Storage.ErrorMessage = string.Format("无法连接到 Modrinth，所以目前仅显示了来自 CurseForge 的内容，搜索结果可能不全。{0}请稍后再试，或使用 VPN 改善网络环境。", "\r\n");
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
                            List<Thread>.Enumerator enumerator4;
                            ((IDisposable)enumerator4).Dispose();
                        }
                    }
                    list3 = list3.Where<ResourceProject>((ResourceSearcher._Closure$__.$I6-29 == null) ? (ResourceSearcher._Closure$__.$I6-29 = (ResourceProject x) => x.Platform == ResourcePlatforms.CurseForge) : ResourceSearcher._Closure$__.$I6-29).Concat<ResourceProject>(list3.Where<ResourceProject>((ResourceSearcher._Closure$__.$I6-30 == null) ? (ResourceSearcher._Closure$__.$I6-30 = (ResourceProject x) => x.Platform != ResourcePlatforms.CurseForge) : ResourceSearcher._Closure$__.$I6-30)).ToList<ResourceProject>();
                    list3 = list3.Distinct<ResourceProject>((ResourceSearcher._Closure$__.$I6-31 == null) ? (ResourceSearcher._Closure$__.$I6-31 = (ResourceProject a, ResourceProject b) => a.IsLike(b)) : ResourceSearcher._Closure$__.$I6-31).ToList<ResourceProject>();
                    list3 = list3.Where<ResourceProject>(delegate(ResourceProject r)
                    {
                        ResourceSearcher._Closure$__6-5 CS$<>8__locals6 = new ResourceSearcher._Closure$__6-5(CS$<>8__locals6);
                        CS$<>8__locals6.$VB$Local_r = r;
                        return !CS$<>8__locals1.$VB$Local_RealResults.Any<ResourceProject>((ResourceProject b) => CS$<>8__locals6.$VB$Local_r.IsLike(b)) && !CS$<>8__locals1.$VB$Local_Storage.Results.Any<ResourceProject>((ResourceProject b) => CS$<>8__locals6.$VB$Local_r.IsLike(b));
                    }).ToList<ResourceProject>();
                    CS$<>8__locals1.$VB$Local_RealResults.AddRange(list3);
                    Logger.Info(string.Format("去重、筛选后累计新增结果 {0} 个（目前已有结果 {1} 个）", CS$<>8__locals1.$VB$Local_RealResults.Count, CS$<>8__locals1.$VB$Local_Storage.Results.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                    checked
                    {
                        if (CS$<>8__locals1.$VB$Local_RealResults.Count + CS$<>8__locals1.$VB$Local_Storage.Results.Count >= CS$<>8__locals1.$VB$Local_Request.TargetResultCount)
                        {
                            goto IL_0B52;
                        }
                        Logger.Info(string.Format("总结果数需求最少 {0} 个，仅获得了 {1} 个", CS$<>8__locals1.$VB$Local_Request.TargetResultCount, CS$<>8__locals1.$VB$Local_RealResults.Count + CS$<>8__locals1.$VB$Local_Storage.Results.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                        if (!CS$<>8__locals1.$VB$Local_Request.CanContinue || CS$<>8__locals1.$VB$Local_Errors.Any<ValueTuple<Exception, ResourcePlatforms>>())
                        {
                            break;
                        }
                        Logger.Info("将继续尝试加载下一页", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                    }
                }
                Logger.Info("无法继续加载，将强制结束", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
                IL_0B52:
                Dictionary<ResourceProject, double> dictionary3 = new Dictionary<ResourceProject, double>();
                VB$AnonymousDelegate_4<ResourceProject, double> vb$AnonymousDelegate_3 = delegate(ResourceProject Project)
                {
                    ResourceTypes type = CS$<>8__locals1.$VB$Local_Request.Type;
                    if (type <= ResourceTypes.ResourcePack)
                    {
                        if (type - ResourceTypes.Mod <= 1)
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
                };
                if (string.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_RawSearchText))
                {
                    try
                    {
                        foreach (ResourceProject resourceProject in CS$<>8__locals1.$VB$Local_RealResults)
                        {
                            dictionary3[resourceProject] = (double)resourceProject.DownloadCount * vb$AnonymousDelegate_3(resourceProject);
                        }
                        goto IL_0D62;
                    }
                    finally
                    {
                        List<ResourceProject>.Enumerator enumerator5;
                        ((IDisposable)enumerator5).Dispose();
                    }
                }
                List<ModBase.SearchEntry<ResourceProject>> list5 = new List<ModBase.SearchEntry<ResourceProject>>();
                try
                {
                    foreach (ResourceProject resourceProject2 in CS$<>8__locals1.$VB$Local_RealResults)
                    {
                        dictionary3[resourceProject2] = ((resourceProject2.WikiId > 0) ? 0.2 : 0.0) + Math.Log10((double)Math.Max(resourceProject2.DownloadCount, 1) * vb$AnonymousDelegate_3(resourceProject2)) / 9.0;
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
                    List<ResourceProject>.Enumerator enumerator6;
                    ((IDisposable)enumerator6).Dispose();
                }
                List<ModBase.SearchEntry<ResourceProject>> list6 = ModBase.Search<ResourceProject>(list5, CS$<>8__locals1.$VB$Local_RawSearchText, 10000, -1.0);
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
                    List<ModBase.SearchEntry<ResourceProject>>.Enumerator enumerator7;
                    ((IDisposable)enumerator7).Dispose();
                }
                IL_0D62:
                if (CS$<>8__locals1.$VB$Local_Task.IsCanceled)
                {
                    throw new OperationCanceledException();
                }
                CS$<>8__locals1.$VB$Local_Storage.Results.AddRange(dictionary3.OrderByDescending<KeyValuePair<ResourceProject, double>, double>((ResourceSearcher._Closure$__.$I6-36 == null) ? (ResourceSearcher._Closure$__.$I6-36 = (KeyValuePair<ResourceProject, double> s) => s.Value) : ResourceSearcher._Closure$__.$I6-36).Select<KeyValuePair<ResourceProject, double>, ResourceProject>((ResourceSearcher._Closure$__.$I6-37 == null) ? (ResourceSearcher._Closure$__.$I6-37 = (KeyValuePair<ResourceProject, double> r) => r.Key) : ResourceSearcher._Closure$__.$I6-37));
                return;
            }
            if (!CS$<>8__locals1.$VB$Local_Storage.Results.Any<ResourceProject>())
            {
                throw new Exception("没有符合条件的结果");
            }
            Logger.Info(string.Format("已有 {0} 个结果，少于所需的 {1} 个结果，但无法继续获取，结束处理", CS$<>8__locals1.$VB$Local_Storage.Results.Count, CS$<>8__locals1.$VB$Local_Request.TargetResultCount), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceSearcher.vb");
        }

        private const int RESULT_PAGE_SIZE = 40;

        private const string MSG_NO_CHINESE_RESULT = "无搜索结果，请尝试搜索其英文名称";

        private static List<ModBase.SearchEntry<WikiEntry>> $STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries;

        private static StaticLocalInitFlag $STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init;

        private static List<ModBase.SearchEntry<WikiEntry>> $STATIC$Run$011151286FC21283B48$ModrinthSearchEntries;

        private static StaticLocalInitFlag $STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init;

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
                    list.Add(string.Format("[\"categories:{0}\"]", this.ModLoaders.Flags<ModLoaders>().Select<ModLoaders, string>((ResourceSearcher.SearchRequest._Closure$__.$I12-0 == null) ? (ResourceSearcher.SearchRequest._Closure$__.$I12-0 = (ModLoaders f) => f.ToString().Lower()) : ResourceSearcher.SearchRequest._Closure$__.$I12-0).Join("\",\"categories:")));
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
                return searchRequest != null && this.Type == searchRequest.Type && this.TargetResultCount == searchRequest.TargetResultCount && Operators.CompareString(this.Tag, searchRequest.Tag, false) == 0 && this.ModLoaders == searchRequest.ModLoaders && this.Sources == searchRequest.Sources && Operators.CompareString(this.GameVersion, searchRequest.GameVersion, false) == 0 && Operators.CompareString(this.SearchText, searchRequest.SearchText, false) == 0;
            }

            public static bool operator ==(ResourceSearcher.SearchRequest left, ResourceSearcher.SearchRequest right)
            {
                return EqualityComparer<ResourceSearcher.SearchRequest>.Default.Equals(left, right);
            }

            public static bool operator !=(ResourceSearcher.SearchRequest left, ResourceSearcher.SearchRequest right)
            {
                return !(left == right);
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
    }
}
