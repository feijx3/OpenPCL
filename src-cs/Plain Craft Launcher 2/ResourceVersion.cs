using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;
using PCLCS;

namespace PCL
{
    public class ResourceVersion
    {
        public ResourceVersion()
        {
            this.ModLoaders = ModLoaders.None;
            this.FileName = null;
            this.Hash = null;
            this.Size = -1;
            this.RawDependencies = new List<string>();
            this.Dependencies = new List<string>();
        }

        public string ReleaseTypeDisplay
        {
            get
            {
                ResourceVersion.ReleaseTypes releaseType = this.ReleaseType;
                string text;
                if (releaseType != ResourceVersion.ReleaseTypes.Release)
                {
                    if (releaseType != ResourceVersion.ReleaseTypes.Beta)
                    {
                        text = (ModBase.ModeDebug ? "Alpha 版" : "早期测试版");
                    }
                    else
                    {
                        text = (ModBase.ModeDebug ? "Beta 版" : "测试版");
                    }
                }
                else
                {
                    text = "正式版";
                }
                return text;
            }
        }

        public bool DownloadAvailable
        {
            get
            {
                return this.FileName != null && this.DownloadUrls != null;
            }
        }

        public static ResourceVersion FromPlatformJson(JObject Data, ResourceTypes ExpectedResourceType)
        {
            ResourceVersion resourceVersion = new ResourceVersion();
            ResourceVersion resourceVersion2 = resourceVersion;
            resourceVersion2.ResourceType = ExpectedResourceType;
            if (Data.ContainsKey("gameId"))
            {
                resourceVersion2.Id = (string)Data["id"];
                resourceVersion2.Display = Data["displayName"].ToString().Replace("\t", "").Trim(new char[] { ' ' });
                resourceVersion2.Version = null;
                resourceVersion2.ReleaseDate = (DateTime)Data["fileDate"];
                resourceVersion2.ReleaseType = (ResourceVersion.ReleaseTypes)Data["releaseType"].ToObject<int>();
                resourceVersion2.DownloadCount = (int)Data["downloadCount"];
                resourceVersion2.FileName = (string)Data["fileName"];
                resourceVersion2.Size = (int)Data["fileLength"];
                if (resourceVersion2.Size == 0)
                {
                    resourceVersion2.Size = -1;
                }
                ResourceVersion resourceVersion3 = resourceVersion2;
                JToken jtoken = ((JArray)Data["hashes"]).ToList<JToken>().FirstOrDefault<JToken>((ResourceVersion._Closure$__.$I22-0 == null) ? (ResourceVersion._Closure$__.$I22-0 = (JToken s) => s["algo"].ToObject<int>() == 1) : ResourceVersion._Closure$__.$I22-0);
                string text;
                if (jtoken == null)
                {
                    text = null;
                }
                else
                {
                    JToken jtoken2 = jtoken["value"];
                    text = ((jtoken2 != null) ? jtoken2.ToString().Lower() : null);
                }
                resourceVersion3.Hash = text;
                if (resourceVersion2.Hash == null)
                {
                    ResourceVersion resourceVersion4 = resourceVersion2;
                    JToken jtoken3 = ((JArray)Data["hashes"]).ToList<JToken>().FirstOrDefault<JToken>((ResourceVersion._Closure$__.$I22-1 == null) ? (ResourceVersion._Closure$__.$I22-1 = (JToken s) => s["algo"].ToObject<int>() == 2) : ResourceVersion._Closure$__.$I22-1);
                    string text2;
                    if (jtoken3 == null)
                    {
                        text2 = null;
                    }
                    else
                    {
                        JToken jtoken4 = jtoken3["value"];
                        text2 = ((jtoken4 != null) ? jtoken4.ToString().Lower() : null);
                    }
                    resourceVersion4.Hash = text2;
                }
                string text3 = Data["downloadUrl"].ToString();
                if (Operators.CompareString(text3, "", false) == 0)
                {
                    text3 = string.Format("https://edge.forgecdn.net/files/{0}/{1}/{2}", Conversions.ToInteger(resourceVersion2.Id.ToString().Substring(0, 4)), Conversions.ToInteger(resourceVersion2.Id.ToString().Substring(4)), resourceVersion2.FileName);
                }
                resourceVersion2.DownloadUrls = ResourceVersion.ParseCurseForgeDownloadUrls(text3.Replace(resourceVersion2.FileName, StringUtils.UrlEscape(resourceVersion2.FileName)));
                resourceVersion2.DownloadUrls.Add(ModDownload.DlSourceModGet(text3));
                resourceVersion2.DownloadUrls = resourceVersion2.DownloadUrls.Distinct<string>().ToList<string>();
                if (Data.ContainsKey("dependencies"))
                {
                    resourceVersion2.RawDependencies = Data["dependencies"].Where<JToken>((ResourceVersion._Closure$__.$I22-2 == null) ? (ResourceVersion._Closure$__.$I22-2 = (JToken d) => d["relationType"].ToObject<int>() == 3 && d["modId"].ToObject<int>() != 306612 && d["modId"].ToObject<int>() != 634179) : ResourceVersion._Closure$__.$I22-2).Select<JToken, string>((ResourceVersion._Closure$__.$I22-3 == null) ? (ResourceVersion._Closure$__.$I22-3 = (JToken d) => d["modId"].ToString()) : ResourceVersion._Closure$__.$I22-3).ToList<string>();
                }
                List<string> list = Data["gameVersions"].Select<JToken, string>((ResourceVersion._Closure$__.$I22-4 == null) ? (ResourceVersion._Closure$__.$I22-4 = (JToken t) => t.ToString().Trim().Lower()) : ResourceVersion._Closure$__.$I22-4).ToList<string>();
                resourceVersion2.GameVersions = list.Where<string>((ResourceVersion._Closure$__.$I22-5 == null) ? (ResourceVersion._Closure$__.$I22-5 = (string v) => McVersion.IsStandardFormat(v)) : ResourceVersion._Closure$__.$I22-5).Select<string, string>((ResourceVersion._Closure$__.$I22-6 == null) ? (ResourceVersion._Closure$__.$I22-6 = (string v) => v.Replace("-snapshot", " 预览版")) : ResourceVersion._Closure$__.$I22-6).Distinct<string>()
                    .ToList<string>();
                if (resourceVersion2.GameVersions.IsSingle<string>())
                {
                    resourceVersion2.GameVersions = resourceVersion2.GameVersions.ToList<string>();
                }
                else if (resourceVersion2.GameVersions.Count > 1)
                {
                    resourceVersion2.GameVersions = resourceVersion2.GameVersions.SortByComparison<string>(new Func<string, string, bool>(McUtils.CompareVersionGE)).ToList<string>();
                    if (resourceVersion2.ResourceType == ResourceTypes.ModPack)
                    {
                        resourceVersion2.GameVersions = new string[] { resourceVersion2.GameVersions[0] }.ToList<string>();
                    }
                }
                else
                {
                    resourceVersion2.GameVersions = new List<string> { "未知版本" };
                }
                resourceVersion2.ModLoaders = ModLoaders.None;
                try
                {
                    foreach (ModLoaders modLoaders in EnumUtils.GetAllFlags<ModLoaders>())
                    {
                        if (list.Contains(modLoaders.ToString().Lower()))
                        {
                            resourceVersion2.ModLoaders |= modLoaders;
                        }
                    }
                    goto IL_0947;
                }
                finally
                {
                    List<ModLoaders>.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
            }
            resourceVersion2.Id = (string)Data["id"];
            resourceVersion2.Display = Data["name"].ToString().Replace("\t", "").Trim(new char[] { ' ' });
            resourceVersion2.Version = (string)Data["version_number"];
            resourceVersion2.ReleaseDate = (DateTime)Data["date_published"];
            resourceVersion2.ReleaseType = ((Operators.CompareString(Data["version_type"].ToString(), "release", false) == 0) ? ResourceVersion.ReleaseTypes.Release : ((Operators.CompareString(Data["version_type"].ToString(), "beta", false) == 0) ? ResourceVersion.ReleaseTypes.Beta : ResourceVersion.ReleaseTypes.Alpha));
            resourceVersion2.DownloadCount = (int)Data["downloads"];
            if (((JArray)Data["files"]).Any<JToken>())
            {
                JToken jtoken5 = Data["files"][0];
                resourceVersion2.FileName = (string)jtoken5["filename"];
                resourceVersion2.DownloadUrls = new List<string>
                {
                    (string)jtoken5["url"],
                    ModDownload.DlSourceModGet((string)jtoken5["url"])
                }.Distinct<string>().ToList<string>();
                resourceVersion2.Size = (int)jtoken5["size"];
                ResourceVersion resourceVersion5 = resourceVersion2;
                JToken jtoken6 = jtoken5["hashes"]["sha1"];
                resourceVersion5.Hash = ((jtoken6 != null) ? jtoken6.ToString().Lower() : null);
            }
            List<string> list2 = Data["loaders"].Select<JToken, string>((ResourceVersion._Closure$__.$I22-7 == null) ? (ResourceVersion._Closure$__.$I22-7 = (JToken v) => v.ToString()) : ResourceVersion._Closure$__.$I22-7).ToList<string>();
            resourceVersion2.ModLoaders = ModLoaders.None;
            try
            {
                foreach (ModLoaders modLoaders2 in EnumUtils.GetAllFlags<ModLoaders>())
                {
                    if (list2.Contains(modLoaders2.ToString().Lower()))
                    {
                        resourceVersion2.ModLoaders |= modLoaders2;
                    }
                }
            }
            finally
            {
                List<ModLoaders>.Enumerator enumerator2;
                ((IDisposable)enumerator2).Dispose();
            }
            if (resourceVersion2.ResourceType.HasFlagF(ResourceTypes.Mod) || resourceVersion2.ResourceType.HasFlagF(ResourceTypes.DataPack))
            {
                if (list2.Intersect<string>(new string[] { "bukkit", "folia", "paper", "purpur", "spigot" }).Any<string>())
                {
                    resourceVersion2.ResourceType = ResourceTypes.Plugin;
                }
                if (list2.Contains("datapack"))
                {
                    resourceVersion2.ResourceType = ResourceTypes.DataPack;
                }
                if (resourceVersion2.ModLoaders.Flags<ModLoaders>().Any<ModLoaders>())
                {
                    resourceVersion2.ResourceType = ResourceTypes.Mod;
                }
            }
            if (Data.ContainsKey("dependencies"))
            {
                resourceVersion2.RawDependencies = Data["dependencies"].Where<JToken>((ResourceVersion._Closure$__.$I22-8 == null) ? (ResourceVersion._Closure$__.$I22-8 = (JToken d) => (string)d["dependency_type"] == "required" && (string)d["project_id"] != "P7dR8mSH" && (string)d["project_id"] != "qvIfYCYJ" && d["project_id"].ToString().Length > 0) : ResourceVersion._Closure$__.$I22-8).Select<JToken, string>((ResourceVersion._Closure$__.$I22-9 == null) ? (ResourceVersion._Closure$__.$I22-9 = (JToken d) => d["project_id"].ToString()) : ResourceVersion._Closure$__.$I22-9).ToList<string>();
            }
            List<string> list3 = Data["game_versions"].Select<JToken, string>((ResourceVersion._Closure$__.$I22-10 == null) ? (ResourceVersion._Closure$__.$I22-10 = (JToken t) => t.ToString().Trim().Lower()) : ResourceVersion._Closure$__.$I22-10).ToList<string>();
            resourceVersion2.GameVersions = list3.Where<string>((ResourceVersion._Closure$__.$I22-11 == null) ? (ResourceVersion._Closure$__.$I22-11 = (string v) => v.Contains(".")) : ResourceVersion._Closure$__.$I22-11).Select<string, string>((ResourceVersion._Closure$__.$I22-12 == null) ? (ResourceVersion._Closure$__.$I22-12 = delegate(string v)
            {
                if (v.Contains("-"))
                {
                    return v.BeforeFirst("-", false) + " 预览版";
                }
                if (!v.StartsWithF("b1.", false))
                {
                    return v;
                }
                return "远古版本";
            }) : ResourceVersion._Closure$__.$I22-12).Distinct<string>()
                .ToList<string>();
            if (!resourceVersion2.GameVersions.IsSingle<string>())
            {
                if (resourceVersion2.GameVersions.Count > 1)
                {
                    resourceVersion2.GameVersions = resourceVersion2.GameVersions.SortByComparison<string>(new Func<string, string, bool>(McUtils.CompareVersionGE)).ToList<string>();
                    if (resourceVersion2.ResourceType == ResourceTypes.ModPack)
                    {
                        resourceVersion2.GameVersions = new List<string> { resourceVersion2.GameVersions[0] };
                    }
                }
                else if (list3.Any<string>((ResourceVersion._Closure$__.$I22-13 == null) ? (ResourceVersion._Closure$__.$I22-13 = (string v) => v.RegexCheck("[0-9]{2}w[0-9]{2}[a-z]", RegexOptions.None)) : ResourceVersion._Closure$__.$I22-13))
                {
                    resourceVersion2.GameVersions = list3.Where<string>((ResourceVersion._Closure$__.$I22-14 == null) ? (ResourceVersion._Closure$__.$I22-14 = (string v) => v.RegexCheck("[0-9]{2}w[0-9]{2}[a-z]", RegexOptions.None)) : ResourceVersion._Closure$__.$I22-14).ToList<string>();
                }
                else
                {
                    resourceVersion2.GameVersions = new List<string> { "未知版本" };
                }
            }
            IL_0947:
            resourceVersion2 = null;
            return resourceVersion;
        }

        public static List<ResourceVersion> FromProjectId(string ProjectId, ResourcePlatforms Platform, bool LoadDependencies = true)
        {
            ResourceVersion._Closure$__23-0 CS$<>8__locals1 = new ResourceVersion._Closure$__23-0(CS$<>8__locals1);
            if (!Platform.Flags<ResourcePlatforms>().IsSingle<ResourcePlatforms>())
            {
                throw new ArgumentException(string.Format("必须指定单一平台，当前值为 {0}", Platform));
            }
            CS$<>8__locals1.$VB$Local_TargetProject = ResourceProject.FromProjectId(ProjectId, Platform);
            checked
            {
                if (!ResourceVersion.ProjectFilesCache.ContainsKey(ProjectId))
                {
                    Logger.Info(string.Format("开始获取项目版本列表：{0}", ProjectId), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceVersion.vb");
                    JArray jarray = null;
                    if (Platform == ResourcePlatforms.CurseForge)
                    {
                        int num = 0;
                        JObject jobject;
                        for (;;)
                        {
                            jobject = (JObject)ModDownload.DlModRequest(string.Format("https://api.curseforge.com/v1/mods/{0}/files?pageSize=", ProjectId) + Conversions.ToString(10000 + num), null, null, null);
                            if (jobject["pagination"]["resultCount"].ToObject<int>() == jobject["pagination"]["totalCount"].ToObject<int>())
                            {
                                break;
                            }
                            if (num >= 2)
                            {
                                goto IL_0127;
                            }
                            Logger.Warn(string.Format("CurseForge 返回的资源版本列表存在缺失，即将进行第 {0} 次重试", num + 1), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceVersion.vb");
                            Logger.Info(string.Format("返回的原始内容如下：{0}{1}", "\r\n", jobject), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceVersion.vb");
                            num++;
                            if (num > 2)
                            {
                                goto Block_6;
                            }
                        }
                        jarray = (JArray)jobject["data"];
                        goto IL_0178;
                        IL_0127:
                        Logger.Info(string.Format("CurseForge 返回的资源版本列表存在缺失，返回的原始内容如下：{0}{1}", "\r\n", jobject), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceVersion.vb");
                        throw new Exception("CurseForge 返回的资源版本列表存在缺失");
                        Block_6:;
                    }
                    else
                    {
                        jarray = (JArray)ModDownload.DlModRequest(string.Format("https://api.modrinth.com/v2/project/{0}/version?include_changelog=false", ProjectId), null, null, null);
                    }
                    IL_0178:
                    ResourceVersion.ProjectFilesCache[ProjectId] = jarray.Select<JToken, ResourceVersion>((JToken a) => ResourceVersion.FromPlatformJson((JObject)a, CS$<>8__locals1.$VB$Local_TargetProject.Types)).Where<ResourceVersion>((ResourceVersion._Closure$__.$I23-1 == null) ? (ResourceVersion._Closure$__.$I23-1 = (ResourceVersion a) => a.DownloadAvailable) : ResourceVersion._Closure$__.$I23-1).DistinctBy<ResourceVersion, string>((ResourceVersion._Closure$__.$I23-2 == null) ? (ResourceVersion._Closure$__.$I23-2 = (ResourceVersion a) => a.Id) : ResourceVersion._Closure$__.$I23-2)
                        .ToList<ResourceVersion>();
                }
                List<ResourceVersion> list = ResourceVersion.ProjectFilesCache[ProjectId];
                List<ResourceVersion> list2;
                if (!LoadDependencies)
                {
                    list2 = list;
                }
                else
                {
                    List<string> list3 = list.SelectMany<ResourceVersion, string>((ResourceVersion._Closure$__.$I23-3 == null) ? (ResourceVersion._Closure$__.$I23-3 = (ResourceVersion f) => f.RawDependencies) : ResourceVersion._Closure$__.$I23-3).Distinct<string>().ToList<string>();
                    if (list3.Any<string>())
                    {
                        Logger.Info(string.Format("项目 {0} 中需要加载的前置：{1}", ProjectId, list3.Join("，")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceVersion.vb");
                        try
                        {
                            foreach (ResourceProject resourceProject in ResourceProject.FromProjectIds(list3, CS$<>8__locals1.$VB$Local_TargetProject.Platform))
                            {
                                try
                                {
                                    foreach (ResourceVersion resourceVersion in list)
                                    {
                                        if (resourceVersion.RawDependencies.Contains(resourceProject.Id) && Operators.CompareString(resourceProject.Id, ProjectId, false) != 0 && !resourceVersion.Dependencies.Contains(resourceProject.Id))
                                        {
                                            resourceVersion.Dependencies.Add(resourceProject.Id);
                                        }
                                    }
                                }
                                finally
                                {
                                    List<ResourceVersion>.Enumerator enumerator2;
                                    ((IDisposable)enumerator2).Dispose();
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator<ResourceProject> enumerator;
                            if (enumerator != null)
                            {
                                enumerator.Dispose();
                            }
                        }
                    }
                    list2 = list;
                }
                return list2;
            }
        }

        public static ResourceVersion FromCacheJson(JObject Data)
        {
            ResourceVersion resourceVersion2;
            ResourceVersion resourceVersion = (resourceVersion2 = new ResourceVersion());
            resourceVersion2.ResourceType = Data["ResourceType"].ToObject<ResourceTypes>();
            resourceVersion2.Id = Data["Id"].ToString();
            resourceVersion2.Display = Data["DisplayName"].ToString();
            if (Data.ContainsKey("Version"))
            {
                resourceVersion2.Version = Data["Version"].ToString();
            }
            resourceVersion2.ReleaseDate = Data["ReleaseDate"].ToObject<DateTime>();
            resourceVersion2.DownloadCount = Data["DownloadCount"].ToObject<int>();
            resourceVersion2.ReleaseType = Data["ReleaseType"].ToObject<ResourceVersion.ReleaseTypes>();
            if (Data.ContainsKey("FileName"))
            {
                resourceVersion2.FileName = Data["FileName"].ToString();
            }
            if (Data.ContainsKey("DownloadUrls"))
            {
                resourceVersion2.DownloadUrls = Data["DownloadUrls"].ToObject<List<string>>();
            }
            if (Data.ContainsKey("ModLoaders"))
            {
                resourceVersion2.ModLoaders = Data["ModLoaders"].ToObject<ModLoaders>();
            }
            if (Data.ContainsKey("Size"))
            {
                resourceVersion2.Size = Data["Size"].ToObject<int>();
            }
            if (Data.ContainsKey("Hash"))
            {
                resourceVersion2.Hash = Data["Hash"].ToString();
            }
            if (Data.ContainsKey("GameVersions"))
            {
                resourceVersion2.GameVersions = Data["GameVersions"].ToObject<List<string>>();
            }
            if (Data.ContainsKey("RawDependencies"))
            {
                resourceVersion2.RawDependencies = Data["RawDependencies"].ToObject<List<string>>();
            }
            if (Data.ContainsKey("Dependencies"))
            {
                resourceVersion2.Dependencies = Data["Dependencies"].ToObject<List<string>>();
            }
            return resourceVersion;
        }

        public JObject ToCacheJson()
        {
            JObject jobject2;
            JObject jobject = (jobject2 = new JObject());
            jobject2.Add("ResourceType", (int)this.ResourceType);
            jobject2.Add("Id", this.Id);
            if (this.Version != null)
            {
                jobject2.Add("Version", this.Version);
            }
            jobject2.Add("DisplayName", this.Display);
            jobject2.Add("ReleaseDate", this.ReleaseDate);
            jobject2.Add("DownloadCount", this.DownloadCount);
            jobject2.Add("ModLoaders", (int)this.ModLoaders);
            jobject2.Add("GameVersions", new JArray(this.GameVersions));
            jobject2.Add("ReleaseType", (int)this.ReleaseType);
            if (this.FileName != null)
            {
                jobject2.Add("FileName", this.FileName);
            }
            if (this.DownloadUrls != null)
            {
                jobject2.Add("DownloadUrls", new JArray(this.DownloadUrls));
            }
            if (this.Hash != null)
            {
                jobject2.Add("Hash", this.Hash);
            }
            if (this.Size >= 0)
            {
                jobject2.Add("Size", this.Size);
            }
            jobject2.Add("RawDependencies", new JArray(this.RawDependencies));
            jobject2.Add("Dependencies", new JArray(this.Dependencies));
            return jobject;
        }

        public MyVirtualizingElement<MyListItem> ToListItem(MyListItem.ClickEventHandler ClickHandler, MyIconButton.ClickEventHandler SaveAsButtonHandler = null, bool IsBadDisplay = false)
        {
            ResourceVersion._Closure$__27-0 CS$<>8__locals1 = new ResourceVersion._Closure$__27-0();
            CS$<>8__locals1.$VB$Me = this;
            CS$<>8__locals1.$VB$Local_ClickHandler = ClickHandler;
            CS$<>8__locals1.$VB$Local_SaveAsButtonHandler = SaveAsButtonHandler;
            CS$<>8__locals1.$VB$Local_IsBadDisplay = IsBadDisplay;
            return new MyVirtualizingElement<MyListItem>(delegate
            {
                MyListItem myListItem = new MyListItem();
                ResourceVersion._Closure$__27-1 CS$<>8__locals2 = new ResourceVersion._Closure$__27-1();
                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_3 = CS$<>8__locals1;
                CS$<>8__locals2.$W2 = myListItem;
                CS$<>8__locals2.$W2.SnapsToDevicePixels = true;
                CS$<>8__locals2.$W2.Height = 42.0;
                CS$<>8__locals2.$W2.Type = MyListItem.CheckType.Clickable;
                CS$<>8__locals2.$W2.Tag = CS$<>8__locals1.$VB$Me;
                CS$<>8__locals2.$W2.Title = (CS$<>8__locals1.$VB$Local_IsBadDisplay ? CS$<>8__locals1.$VB$Me.FileName : CS$<>8__locals1.$VB$Me.Display).BeforeLastOfAny(new string[] { ".zip", ".jar", ".mrpack", ".litemod" }, true);
                CS$<>8__locals2.$W2.Logo = string.Format("{0}ReleaseTypes/{1}.png", ModBase.PathImage, CS$<>8__locals1.$VB$Me.ReleaseType);
                CS$<>8__locals2.$W2.Click += CS$<>8__locals1.$VB$Local_ClickHandler;
                CS$<>8__locals2.$W2.Info = CS$<>8__locals2._Lambda$__1().Join('，');
                if (CS$<>8__locals1.$VB$Local_SaveAsButtonHandler != null)
                {
                    MyIconButton myIconButton = new MyIconButton
                    {
                        Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z",
                        ToolTip = "另存为"
                    };
                    myIconButton.Click += CS$<>8__locals1.$VB$Local_SaveAsButtonHandler;
                    CS$<>8__locals2.$W2.Buttons = new MyIconButton[] { myIconButton };
                }
                return myListItem;
            })
            {
                Height = 42.0
            };
        }

        public ModNet.NetFile ToNetFile(string LocalFileOrFolder, ResourceVersion.DownloadReason? TrackDownloadReason = null, string TrackGameVersion = null, ModLoaders TrackLoader = ModLoaders.None)
        {
            return new ModNet.NetFile(ResourceVersion.ParseModrinthTrackArguments(this.DownloadUrls, TrackDownloadReason, TrackGameVersion, TrackLoader), LocalFileOrFolder + (LocalFileOrFolder.EndsWithF("\\", false) ? this.FileName : ""), new FileChecker
            {
                Hash = this.Hash,
                ActualSize = (long)this.Size
            }, true);
        }

        public static IEnumerable<string> ParseModrinthTrackArguments(List<string> Urls, ResourceVersion.DownloadReason? TrackDownloadReason = null, string TrackGameVersion = null, ModLoaders TrackLoader = ModLoaders.None)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (TrackDownloadReason != null)
            {
                dictionary.Add("mr_download_reason", TrackDownloadReason.ToString().Lower());
            }
            if (TrackGameVersion != null)
            {
                dictionary.Add("mr_game_version", TrackGameVersion);
            }
            if (TrackLoader.Flags<ModLoaders>().Any<ModLoaders>())
            {
                dictionary.Add("mr_loader", TrackLoader.Flags<ModLoaders>().First<ModLoaders>().ToString()
                    .Lower());
            }
            string Argument = dictionary.Select<KeyValuePair<string, string>, string>((ResourceVersion._Closure$__.$I29-0 == null) ? (ResourceVersion._Closure$__.$I29-0 = (KeyValuePair<string, string> a) => string.Format("{0}={1}", a.Key, a.Value)) : ResourceVersion._Closure$__.$I29-0).Join("&");
            try
            {
                foreach (string Url in Urls)
                {
                    if (!Url.ContainsIgnoreCase("cdn.modrinth.com/"))
                    {
                        yield return Url;
                    }
                    yield return string.Format("{0}{1}{2}", Url, Url.Contains("?") ? "&" : "?", Argument);
                }
                yield break;
            }
            finally
            {
                List<string>.Enumerator enumerator;
                ((IDisposable)enumerator).Dispose();
            }
            yield break;
        }

        public static List<string> ParseCurseForgeDownloadUrls(string Url)
        {
            return new string[]
            {
                Url.Replace("-service.overwolf.wtf", ".forgecdn.net").Replace("://edge.", "://mediafilez.").Replace("://media.", "://mediafilez."),
                Url.Replace("://edge.", "://mediafilez.").Replace("://media.", "://mediafilez."),
                Url.Replace("-service.overwolf.wtf", ".forgecdn.net"),
                Url.Replace("://media.", "://edge."),
                Url
            }.Distinct<string>().ToList<string>();
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Id, this.FileName);
        }

        public string Id;

        public ResourceTypes ResourceType;

        public string Display;

        public DateTime ReleaseDate;

        public int DownloadCount;

        public string Version;

        public ModLoaders ModLoaders;

        public List<string> GameVersions;

        public ResourceVersion.ReleaseTypes ReleaseType;

        public string FileName;

        public List<string> DownloadUrls;

        public string Hash;

        public int Size;

        private List<string> RawDependencies;

        public List<string> Dependencies;

        protected internal static Dictionary<string, List<ResourceVersion>> ProjectFilesCache = new Dictionary<string, List<ResourceVersion>>();

        public enum ReleaseTypes
        {
            Release = 1,
            Beta,
            Alpha
        }

        public enum DownloadReason
        {
            Standalone,
            Dependency,
            ModPack,
            Update
        }
    }
}
