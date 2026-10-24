using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        // Note: this type is marked as 'beforefieldinit'.
        static ResourceVersion()
        {
        }

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
                JToken jtoken = ((JArray)Data["hashes"]).ToList<JToken>().FirstOrDefault<JToken>((ResourceVersion._Closure___._I22_0 == null) ? (ResourceVersion._Closure___._I22_0 = new Func<JToken, bool>(ResourceVersion._Closure___._I._Lambda___22_0)) : ResourceVersion._Closure___._I22_0);
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
                    JToken jtoken3 = ((JArray)Data["hashes"]).ToList<JToken>().FirstOrDefault<JToken>((ResourceVersion._Closure___._I22_1 == null) ? (ResourceVersion._Closure___._I22_1 = new Func<JToken, bool>(ResourceVersion._Closure___._I._Lambda___22_1)) : ResourceVersion._Closure___._I22_1);
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
                    resourceVersion2.RawDependencies = Data["dependencies"].Where<JToken>((ResourceVersion._Closure___._I22_2 == null) ? (ResourceVersion._Closure___._I22_2 = new Func<JToken, bool>(ResourceVersion._Closure___._I._Lambda___22_2)) : ResourceVersion._Closure___._I22_2).Select<JToken, string>((ResourceVersion._Closure___._I22_3 == null) ? (ResourceVersion._Closure___._I22_3 = new Func<JToken, string>(ResourceVersion._Closure___._I._Lambda___22_3)) : ResourceVersion._Closure___._I22_3).ToList<string>();
                }
                List<string> list = Data["gameVersions"].Select<JToken, string>((ResourceVersion._Closure___._I22_4 == null) ? (ResourceVersion._Closure___._I22_4 = new Func<JToken, string>(ResourceVersion._Closure___._I._Lambda___22_4)) : ResourceVersion._Closure___._I22_4).ToList<string>();
                resourceVersion2.GameVersions = list.Where<string>((ResourceVersion._Closure___._I22_5 == null) ? (ResourceVersion._Closure___._I22_5 = new Func<string, bool>(ResourceVersion._Closure___._I._Lambda___22_5)) : ResourceVersion._Closure___._I22_5).Select<string, string>((ResourceVersion._Closure___._I22_6 == null) ? (ResourceVersion._Closure___._I22_6 = new Func<string, string>(ResourceVersion._Closure___._I._Lambda___22_6)) : ResourceVersion._Closure___._I22_6).Distinct<string>()
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
                    List<ModLoaders>.Enumerator enumerator = default;
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
            List<string> list2 = Data["loaders"].Select<JToken, string>((ResourceVersion._Closure___._I22_7 == null) ? (ResourceVersion._Closure___._I22_7 = new Func<JToken, string>(ResourceVersion._Closure___._I._Lambda___22_7)) : ResourceVersion._Closure___._I22_7).ToList<string>();
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
                List<ModLoaders>.Enumerator enumerator2 = default;
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
                resourceVersion2.RawDependencies = Data["dependencies"].Where<JToken>((ResourceVersion._Closure___._I22_8 == null) ? (ResourceVersion._Closure___._I22_8 = new Func<JToken, bool>(ResourceVersion._Closure___._I._Lambda___22_8)) : ResourceVersion._Closure___._I22_8).Select<JToken, string>((ResourceVersion._Closure___._I22_9 == null) ? (ResourceVersion._Closure___._I22_9 = new Func<JToken, string>(ResourceVersion._Closure___._I._Lambda___22_9)) : ResourceVersion._Closure___._I22_9).ToList<string>();
            }
            List<string> list3 = Data["game_versions"].Select<JToken, string>((ResourceVersion._Closure___._I22_10 == null) ? (ResourceVersion._Closure___._I22_10 = new Func<JToken, string>(ResourceVersion._Closure___._I._Lambda___22_10)) : ResourceVersion._Closure___._I22_10).ToList<string>();
            resourceVersion2.GameVersions = list3.Where<string>((ResourceVersion._Closure___._I22_11 == null) ? (ResourceVersion._Closure___._I22_11 = new Func<string, bool>(ResourceVersion._Closure___._I._Lambda___22_11)) : ResourceVersion._Closure___._I22_11).Select<string, string>((ResourceVersion._Closure___._I22_12 == null) ? (ResourceVersion._Closure___._I22_12 = new Func<string, string>(ResourceVersion._Closure___._I._Lambda___22_12)) : ResourceVersion._Closure___._I22_12).Distinct<string>()
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
                else if (list3.Any<string>((ResourceVersion._Closure___._I22_13 == null) ? (ResourceVersion._Closure___._I22_13 = new Func<string, bool>(ResourceVersion._Closure___._I._Lambda___22_13)) : ResourceVersion._Closure___._I22_13))
                {
                    resourceVersion2.GameVersions = list3.Where<string>((ResourceVersion._Closure___._I22_14 == null) ? (ResourceVersion._Closure___._I22_14 = new Func<string, bool>(ResourceVersion._Closure___._I._Lambda___22_14)) : ResourceVersion._Closure___._I22_14).ToList<string>();
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
            ResourceVersion._Closure___23_0 CS_locals1 = new ResourceVersion._Closure___23_0(null);
            if (!Platform.Flags<ResourcePlatforms>().IsSingle<ResourcePlatforms>())
            {
                throw new ArgumentException(string.Format("必须指定单一平台，当前值为 {0}", Platform));
            }
            CS_locals1._VB_Local_TargetProject = ResourceProject.FromProjectId(ProjectId, Platform);
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
                    ResourceVersion.ProjectFilesCache[ProjectId] = jarray.Select<JToken, ResourceVersion>(new Func<JToken, ResourceVersion>(CS_locals1._Lambda___0)).Where<ResourceVersion>((ResourceVersion._Closure___._I23_1 == null) ? (ResourceVersion._Closure___._I23_1 = new Func<ResourceVersion, bool>(ResourceVersion._Closure___._I._Lambda___23_1)) : ResourceVersion._Closure___._I23_1).DistinctBy<ResourceVersion, string>((ResourceVersion._Closure___._I23_2 == null) ? (ResourceVersion._Closure___._I23_2 = new Func<ResourceVersion, string>(ResourceVersion._Closure___._I._Lambda___23_2)) : ResourceVersion._Closure___._I23_2)
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
                    List<string> list3 = list.SelectMany<ResourceVersion, string>((ResourceVersion._Closure___._I23_3 == null) ? (ResourceVersion._Closure___._I23_3 = new Func<ResourceVersion, IEnumerable<string>>(ResourceVersion._Closure___._I._Lambda___23_3)) : ResourceVersion._Closure___._I23_3).Distinct<string>().ToList<string>();
                    if (list3.Any<string>())
                    {
                        Logger.Info(string.Format("项目 {0} 中需要加载的前置：{1}", ProjectId, list3.Join("，")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceVersion.vb");
                        try
                        {
                            foreach (ResourceProject resourceProject in ResourceProject.FromProjectIds(list3, CS_locals1._VB_Local_TargetProject.Platform))
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
                                    List<ResourceVersion>.Enumerator enumerator2 = default;
                                    ((IDisposable)enumerator2).Dispose();
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator<ResourceProject> enumerator = default;
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

        public OurVirtualizingElement<OurListItem> ToListItem(OurListItem.ClickEventHandler ClickHandler, OurIconButton.ClickEventHandler SaveAsButtonHandler = null, bool IsBadDisplay = false)
        {
            ResourceVersion._Closure___27_0 CS_locals1 = new ResourceVersion._Closure___27_0();
            CS_locals1._VB_Me = this;
            CS_locals1._VB_Local_ClickHandler = ClickHandler;
            CS_locals1._VB_Local_SaveAsButtonHandler = SaveAsButtonHandler;
            CS_locals1._VB_Local_IsBadDisplay = IsBadDisplay;
            return new OurVirtualizingElement<OurListItem>(new Func<OurListItem>(CS_locals1._Lambda___0))
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
            ResourceVersion.VB_StateMachine_29_ParseModrinthTrackArguments vb_StateMachine_29_ParseModrinthTrackArguments = new ResourceVersion.VB_StateMachine_29_ParseModrinthTrackArguments(-2);
            vb_StateMachine_29_ParseModrinthTrackArguments._P_Urls = Urls;
            vb_StateMachine_29_ParseModrinthTrackArguments._P_TrackDownloadReason = TrackDownloadReason;
            vb_StateMachine_29_ParseModrinthTrackArguments._P_TrackGameVersion = TrackGameVersion;
            vb_StateMachine_29_ParseModrinthTrackArguments._P_TrackLoader = TrackLoader;
            return vb_StateMachine_29_ParseModrinthTrackArguments;
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

        [CompilerGenerated]
        private sealed class VB_StateMachine_29_ParseModrinthTrackArguments : IEnumerable<string>, IEnumerable, IEnumerator<string>, IDisposable, IEnumerator
        {
            public VB_StateMachine_29_ParseModrinthTrackArguments(int _State)
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
                {
                    num = (this._State = -1);
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    if (this._VB_Local_TrackDownloadReason != null)
                    {
                        dictionary.Add("mr_download_reason", this._VB_Local_TrackDownloadReason.ToString().Lower());
                    }
                    if (this._VB_Local_TrackGameVersion != null)
                    {
                        dictionary.Add("mr_game_version", this._VB_Local_TrackGameVersion);
                    }
                    if (this._VB_Local_TrackLoader.Flags<ModLoaders>().Any<ModLoaders>())
                    {
                        dictionary.Add("mr_loader", this._VB_Local_TrackLoader.Flags<ModLoaders>().First<ModLoaders>().ToString()
                            .Lower());
                    }
                    this._VB_ResumableLocal_Argument_1 = dictionary.Select<KeyValuePair<string, string>, string>((ResourceVersion._Closure___._I29_0 == null) ? (ResourceVersion._Closure___._I29_0 = new Func<KeyValuePair<string, string>, string>(ResourceVersion._Closure___._I._Lambda___29_0)) : ResourceVersion._Closure___._I29_0).Join("&");
                    break;
                }
                }
                try
                {
                    if (num != -3)
                    {
                        if (num != 1)
                        {
                            if (num != 2)
                            {
                                this._S2 = this._VB_Local_Urls.GetEnumerator();
                            }
                            else
                            {
                                num = (this._State = -1);
                            }
                            if (!this._S2.MoveNext())
                            {
                                return false;
                            }
                            this._VB_ResumableLocal_Url_0 = this._S2.Current;
                            if (!this._VB_ResumableLocal_Url_0.ContainsIgnoreCase("cdn.modrinth.com/"))
                            {
                                this._Current = this._VB_ResumableLocal_Url_0;
                                num = (this._State = 1);
                                return true;
                            }
                        }
                        else
                        {
                            num = (this._State = -1);
                        }
                        this._Current = string.Format("{0}{1}{2}", this._VB_ResumableLocal_Url_0, this._VB_ResumableLocal_Url_0.Contains("?") ? "&" : "?", this._VB_ResumableLocal_Argument_1);
                        num = (this._State = 2);
                        return true;
                    }
                    num = (this._State = -1);
                    return true;
                }
                finally
                {
                    if (num < 0)
                    {
                        ((IDisposable)this._S2).Dispose();
                    }
                }
                return false;
            }

            public IEnumerator<string> GetEnumerator()
            {
                ResourceVersion.VB_StateMachine_29_ParseModrinthTrackArguments vb_StateMachine_29_ParseModrinthTrackArguments;
                if (this._State == -2 && this._InitialThreadId == Environment.CurrentManagedThreadId)
                {
                    this._State = 0;
                    vb_StateMachine_29_ParseModrinthTrackArguments = this;
                }
                else
                {
                    vb_StateMachine_29_ParseModrinthTrackArguments = new ResourceVersion.VB_StateMachine_29_ParseModrinthTrackArguments(0);
                }
                vb_StateMachine_29_ParseModrinthTrackArguments._VB_Local_Urls = this._P_Urls;
                vb_StateMachine_29_ParseModrinthTrackArguments._VB_Local_TrackDownloadReason = this._P_TrackDownloadReason;
                vb_StateMachine_29_ParseModrinthTrackArguments._VB_Local_TrackGameVersion = this._P_TrackGameVersion;
                vb_StateMachine_29_ParseModrinthTrackArguments._VB_Local_TrackLoader = this._P_TrackLoader;
                return vb_StateMachine_29_ParseModrinthTrackArguments;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            string IEnumerator<string>.Current
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

            public string _Current;

            public int _InitialThreadId;

            internal List<string> _VB_Local_Urls;

            internal List<string> _P_Urls;

            internal ResourceVersion.DownloadReason? _VB_Local_TrackDownloadReason;

            internal ResourceVersion.DownloadReason? _P_TrackDownloadReason;

            internal string _VB_Local_TrackGameVersion;

            internal string _P_TrackGameVersion;

            internal ModLoaders _VB_Local_TrackLoader;

            internal ModLoaders _P_TrackLoader;

            internal string _VB_ResumableLocal_Url_0;

            internal string _VB_ResumableLocal_Argument_1;

            internal List<string>.Enumerator _S2;
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

            internal bool _Lambda___22_0(JToken s)
            {
                return s["algo"].ToObject<int>() == 1;
            }

            internal bool _Lambda___22_1(JToken s)
            {
                return s["algo"].ToObject<int>() == 2;
            }

            internal bool _Lambda___22_2(JToken d)
            {
                return d["relationType"].ToObject<int>() == 3 && d["modId"].ToObject<int>() != 306612 && d["modId"].ToObject<int>() != 634179;
            }

            internal string _Lambda___22_3(JToken d)
            {
                return d["modId"].ToString();
            }

            internal string _Lambda___22_4(JToken t)
            {
                return t.ToString().Trim().Lower();
            }

            internal bool _Lambda___22_5(string v)
            {
                return McVersion.IsStandardFormat(v);
            }

            internal string _Lambda___22_6(string v)
            {
                return v.Replace("-snapshot", " 预览版");
            }

            internal string _Lambda___22_7(JToken v)
            {
                return v.ToString();
            }

            internal bool _Lambda___22_8(JToken d)
            {
                return (string)d["dependency_type"] == "required" && (string)d["project_id"] != "P7dR8mSH" && (string)d["project_id"] != "qvIfYCYJ" && d["project_id"].ToString().Length > 0;
            }

            internal string _Lambda___22_9(JToken d)
            {
                return d["project_id"].ToString();
            }

            internal string _Lambda___22_10(JToken t)
            {
                return t.ToString().Trim().Lower();
            }

            internal bool _Lambda___22_11(string v)
            {
                return v.Contains(".");
            }

            internal string _Lambda___22_12(string v)
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
            }

            internal bool _Lambda___22_13(string v)
            {
                return v.RegexCheck("[0-9]{2}w[0-9]{2}[a-z]", RegexOptions.None);
            }

            internal bool _Lambda___22_14(string v)
            {
                return v.RegexCheck("[0-9]{2}w[0-9]{2}[a-z]", RegexOptions.None);
            }

            internal bool _Lambda___23_1(ResourceVersion a)
            {
                return a.DownloadAvailable;
            }

            internal string _Lambda___23_2(ResourceVersion a)
            {
                return a.Id;
            }

            internal IEnumerable<string> _Lambda___23_3(ResourceVersion f)
            {
                return f.RawDependencies;
            }

            internal bool _Lambda___27_2(string v)
            {
                ResourceVersion._Closure___27_2 CS_locals1 = new ResourceVersion._Closure___27_2();
                CS_locals1._VB_Local_v = v;
                return !CS_locals1._VB_Local_v.Contains(".") || new string[] { "w", "snapshot", "rc", "pre", "experimental", "-" }.Any<string>(new Func<string, bool>(CS_locals1._Lambda___3));
            }

            internal string _Lambda___29_0(KeyValuePair<string, string> a)
            {
                return string.Format("{0}={1}", a.Key, a.Value);
            }

            public static readonly ResourceVersion._Closure___ _I = new ResourceVersion._Closure___();

            public static Func<JToken, bool> _I22_0;

            public static Func<JToken, bool> _I22_1;

            public static Func<JToken, bool> _I22_2;

            public static Func<JToken, string> _I22_3;

            public static Func<JToken, string> _I22_4;

            public static Func<string, bool> _I22_5;

            public static Func<string, string> _I22_6;

            public static Func<JToken, string> _I22_7;

            public static Func<JToken, bool> _I22_8;

            public static Func<JToken, string> _I22_9;

            public static Func<JToken, string> _I22_10;

            public static Func<string, bool> _I22_11;

            public static Func<string, string> _I22_12;

            public static Func<string, bool> _I22_13;

            public static Func<string, bool> _I22_14;

            public static Func<ResourceVersion, bool> _I23_1;

            public static Func<ResourceVersion, string> _I23_2;

            public static Func<ResourceVersion, IEnumerable<string>> _I23_3;

            public static Func<string, bool> _I27_2;

            public static Func<KeyValuePair<string, string>, string> _I29_0;
        }

        [CompilerGenerated]
        private sealed class _Closure___23_0
        {
            public _Closure___23_0(ResourceVersion._Closure___23_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_TargetProject = arg0._VB_Local_TargetProject;
                }
            }

            internal ResourceVersion _Lambda___0(JToken a)
            {
                return ResourceVersion.FromPlatformJson((JObject)a, this._VB_Local_TargetProject.Types);
            }

            public ResourceProject _VB_Local_TargetProject;
        }

        [CompilerGenerated]
        private sealed class _Closure___27_0
        {
            public _Closure___27_0()
            {
            }

            internal OurListItem _Lambda___0()
            {
                OurListItem ourListItem = new OurListItem();
                ResourceVersion._Closure___27_1 CS_locals1 = new ResourceVersion._Closure___27_1();
                CS_locals1._VB_NonLocal__VB_Closure_3 = this;
                CS_locals1._W2 = ourListItem;
                CS_locals1._W2.SnapsToDevicePixels = true;
                CS_locals1._W2.Height = 42.0;
                CS_locals1._W2.Type = OurListItem.CheckType.Clickable;
                CS_locals1._W2.Tag = this._VB_Me;
                CS_locals1._W2.Title = (this._VB_Local_IsBadDisplay ? this._VB_Me.FileName : this._VB_Me.Display).BeforeLastOfAny(new string[] { ".zip", ".jar", ".mrpack", ".litemod" }, true);
                CS_locals1._W2.Logo = string.Format("{0}ReleaseTypes/{1}.png", ModBase.PathImage, this._VB_Me.ReleaseType);
                CS_locals1._W2.Click += this._VB_Local_ClickHandler;
                CS_locals1._W2.Info = CS_locals1._Lambda___1().Join('，');
                if (this._VB_Local_SaveAsButtonHandler != null)
                {
                    OurIconButton ourIconButton = new OurIconButton
                    {
                        Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z",
                        ToolTip = "另存为"
                    };
                    ourIconButton.Click += this._VB_Local_SaveAsButtonHandler;
                    CS_locals1._W2.Buttons = new OurIconButton[] { ourIconButton };
                }
                return ourListItem;
            }

            public bool _VB_Local_IsBadDisplay;

            public OurListItem.ClickEventHandler _VB_Local_ClickHandler;

            public OurIconButton.ClickEventHandler _VB_Local_SaveAsButtonHandler;

            public ResourceVersion _VB_Me;
        }

        [CompilerGenerated]
        private sealed class _Closure___27_1
        {
            public _Closure___27_1()
            {
            }

            internal IEnumerable<string> _Lambda___1()
            {
                ResourceVersion._Closure___27_1.VB_StateMachine___Lambda___1 vb_StateMachine___Lambda___ = new ResourceVersion._Closure___27_1.VB_StateMachine___Lambda___1(-2);
                vb_StateMachine___Lambda___._VB_NonLocal__Closure___27_1 = this;
                return vb_StateMachine___Lambda___;
            }

            public OurListItem _W2;

            public ResourceVersion._Closure___27_0 _VB_NonLocal__VB_Closure_3;

            [CompilerGenerated]
            private sealed class VB_StateMachine___Lambda___1 : IEnumerable<string>, IEnumerable, IEnumerator<string>, IDisposable, IEnumerator
            {
                public VB_StateMachine___Lambda___1(int _State)
                {
                    this._State = _State;
                    this._InitialThreadId = Environment.CurrentManagedThreadId;
                }

                void IDisposable.Dispose()
                {
                    this._State = -2;
                }

                [CompilerGenerated]
                public bool MoveNext()
                {
                    switch (this._State)
                    {
                    case 0:
                        this._State = -1;
                        if (this._VB_NonLocal__Closure___27_1._VB_NonLocal__VB_Closure_3._VB_Me.ModLoaders != ModLoaders.None)
                        {
                            this._Current = this._VB_NonLocal__Closure___27_1._VB_NonLocal__VB_Closure_3._VB_Me.ModLoaders.Flags<ModLoaders>().Join('、');
                            this._State = 1;
                            return true;
                        }
                        break;
                    case 1:
                        this._State = -1;
                        break;
                    case 2:
                        this._State = -1;
                        goto IL_00F3;
                    case 3:
                        this._State = -1;
                        goto IL_0152;
                    case 4:
                        this._State = -1;
                        goto IL_01D5;
                    case 5:
                        this._State = -1;
                        goto IL_028A;
                    case 6:
                        this._State = -1;
                        if (this._VB_NonLocal__Closure___27_1._VB_NonLocal__VB_Closure_3._VB_Me.ReleaseType != ResourceVersion.ReleaseTypes.Release)
                        {
                            this._Current = this._VB_NonLocal__Closure___27_1._VB_NonLocal__VB_Closure_3._VB_Me.ReleaseTypeDisplay;
                            this._State = 7;
                            return true;
                        }
                        return false;
                    case 7:
                        this._State = -1;
                        return false;
                    default:
                        return false;
                    }
                    if (Operators.CompareString(this._VB_NonLocal__Closure___27_1._W2.Title, PathUtils.GetFileNameWithoutExtension(this._VB_NonLocal__Closure___27_1._VB_NonLocal__VB_Closure_3._VB_Me.FileName), false) != 0)
                    {
                        this._Current = PathUtils.GetFileNameWithoutExtension(this._VB_NonLocal__Closure___27_1._VB_NonLocal__VB_Closure_3._VB_Me.FileName);
                        this._State = 2;
                        return true;
                    }
                    IL_00F3:
                    if (this._VB_NonLocal__Closure___27_1._VB_NonLocal__VB_Closure_3._VB_Me.Dependencies.Any<string>())
                    {
                        this._Current = string.Format("{0} 项前置", this._VB_NonLocal__Closure___27_1._VB_NonLocal__VB_Closure_3._VB_Me.Dependencies.Count);
                        this._State = 3;
                        return true;
                    }
                    IL_0152:
                    if (this._VB_NonLocal__Closure___27_1._VB_NonLocal__VB_Closure_3._VB_Me.GameVersions.All<string>((ResourceVersion._Closure___._I27_2 == null) ? (ResourceVersion._Closure___._I27_2 = new Func<string, bool>(ResourceVersion._Closure___._I._Lambda___27_2)) : ResourceVersion._Closure___._I27_2))
                    {
                        this._Current = string.Format("游戏版本 {0}", this._VB_NonLocal__Closure___27_1._VB_NonLocal__VB_Closure_3._VB_Me.GameVersions.Join('、'));
                        this._State = 4;
                        return true;
                    }
                    IL_01D5:
                    if (this._VB_NonLocal__Closure___27_1._VB_NonLocal__VB_Closure_3._VB_Me.DownloadCount > 0)
                    {
                        this._Current = string.Format("下载 {0}次", (this._VB_NonLocal__Closure___27_1._VB_NonLocal__VB_Closure_3._VB_Me.DownloadCount > 100000) ? (Conversions.ToString(Math.Round((double)this._VB_NonLocal__Closure___27_1._VB_NonLocal__VB_Closure_3._VB_Me.DownloadCount / 10000.0)) + " 万") : (this._VB_NonLocal__Closure___27_1._VB_NonLocal__VB_Closure_3._VB_Me.DownloadCount.ToString() + " "));
                        this._State = 5;
                        return true;
                    }
                    IL_028A:
                    this._Current = string.Format("更新于 {0}", StringUtils.FormatTimeSpan(this._VB_NonLocal__Closure___27_1._VB_NonLocal__VB_Closure_3._VB_Me.ReleaseDate - DateTime.Now, false));
                    this._State = 6;
                    return true;
                }

                public IEnumerator<string> GetEnumerator()
                {
                    ResourceVersion._Closure___27_1.VB_StateMachine___Lambda___1 vb_StateMachine___Lambda___;
                    if (this._State == -2 && this._InitialThreadId == Environment.CurrentManagedThreadId)
                    {
                        this._State = 0;
                        vb_StateMachine___Lambda___ = this;
                    }
                    else
                    {
                        vb_StateMachine___Lambda___ = new ResourceVersion._Closure___27_1.VB_StateMachine___Lambda___1(0);
                        vb_StateMachine___Lambda___._VB_NonLocal__Closure___27_1 = this._VB_NonLocal__Closure___27_1;
                    }
                    return vb_StateMachine___Lambda___;
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                string IEnumerator<string>.Current
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

                public string _Current;

                public int _InitialThreadId;

                internal ResourceVersion._Closure___27_1 _VB_NonLocal__Closure___27_1;
            }
        }

        [CompilerGenerated]
        private sealed class _Closure___27_2
        {
            public _Closure___27_2()
            {
            }

            internal bool _Lambda___3(string s)
            {
                return this._VB_Local_v.ContainsIgnoreCase(s);
            }

            public string _VB_Local_v;
        }
    }
}
