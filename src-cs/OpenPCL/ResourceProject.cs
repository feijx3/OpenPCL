using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;
using PCLCS;

namespace PCL
{
    public class ResourceProject
    {
        // Note: this type is marked as 'beforefieldinit'.
        static ResourceProject()
        {
        }

        private WikiEntry WikiEntry
        {
            get
            {
                if (!this.LoadedDatabase)
                {
                    this.LoadedDatabase = true;
                    if (this.Types.HasFlagF(ResourceTypes.Mod) || this.Types.HasFlagF(ResourceTypes.DataPack))
                    {
                        this._WikiEntry = WikiEntry.All.Value.FirstOrDefault<WikiEntry>(new Func<WikiEntry, bool>(this._Lambda___19_0));
                    }
                }
                return this._WikiEntry;
            }
            set
            {
                this.LoadedDatabase = true;
                this._WikiEntry = value;
            }
        }

        public int WikiId
        {
            get
            {
                if (this.WikiEntry != null)
                {
                    return this.WikiEntry.Id;
                }
                return 0;
            }
        }

        public string TranslatedName
        {
            get
            {
                WikiEntry wikiEntry = this.WikiEntry;
                return ((wikiEntry != null) ? wikiEntry.ChineseName : null) ?? this.RawName;
            }
        }

        public ResourceProject(JObject Data)
        {
            this.ModLoaders = ModLoaders.None;
            this.LogoUrl = null;
            this.LoadedDatabase = false;
            this._WikiEntry = null;
            if (Data.ContainsKey("Tags"))
            {
                this.Platform = (ResourcePlatforms)Data["Platform"].ToObject<int>();
                this.Types = (ResourceTypes)Data["Types"].ToObject<int>();
                this.Slug = (string)Data["Slug"];
                this.Id = (string)Data["Id"];
                if (Data.ContainsKey("CurseForgeFileIds"))
                {
                    this.CurseForgeFileIds = ((JArray)Data["CurseForgeFileIds"]).Select<JToken, int>((ResourceProject._Closure___._I25_0 == null) ? (ResourceProject._Closure___._I25_0 = new Func<JToken, int>(ResourceProject._Closure___._I._Lambda___25_0)) : ResourceProject._Closure___._I25_0).ToList<int>();
                }
                this.RawName = (string)Data["RawName"];
                this.Description = (string)Data["Description"];
                this.Website = (string)Data["Website"];
                this.LastUpdate = (DateTime)Data["LastUpdate"];
                this.DownloadCount = (int)Data["DownloadCount"];
                if (Data.ContainsKey("ModLoaders"))
                {
                    this.ModLoaders = (ModLoaders)Data["ModLoaders"].ToObject<int>();
                }
                this.Tags = ((JArray)Data["Tags"]).Select<JToken, string>((ResourceProject._Closure___._I25_1 == null) ? (ResourceProject._Closure___._I25_1 = new Func<JToken, string>(ResourceProject._Closure___._I._Lambda___25_1)) : ResourceProject._Closure___._I25_1).ToList<string>();
                if (Data.ContainsKey("LogoUrl"))
                {
                    this.LogoUrl = (string)Data["LogoUrl"];
                }
                if (Data.ContainsKey("Drops"))
                {
                    this.Drops = ((JArray)Data["Drops"]).Select<JToken, int>((ResourceProject._Closure___._I25_2 == null) ? (ResourceProject._Closure___._I25_2 = new Func<JToken, int>(ResourceProject._Closure___._I._Lambda___25_2)) : ResourceProject._Closure___._I25_2).ToList<int>();
                }
                else
                {
                    this.Drops = new List<int>();
                }
            }
            else
            {
                this.Platform = (Data.ContainsKey("summary") ? ResourcePlatforms.CurseForge : ResourcePlatforms.Modrinth);
                if (this.Platform == ResourcePlatforms.CurseForge)
                {
                    this.Id = (string)Data["id"];
                    this.Slug = (string)Data["slug"];
                    this.RawName = (string)Data["name"];
                    this.Description = (string)Data["summary"];
                    this.Website = Data["links"]["websiteUrl"].ToString().TrimEnd(new char[] { '/' });
                    this.LastUpdate = (DateTime)Data["dateReleased"];
                    this.DownloadCount = (int)Data["downloadCount"];
                    if (Data["logo"].Count<JToken>() > 0)
                    {
                        if (Data["logo"]["thumbnailUrl"] == null || (string)Data["logo"]["thumbnailUrl"] == "")
                        {
                            this.LogoUrl = (string)Data["logo"]["url"];
                        }
                        else
                        {
                            this.LogoUrl = (string)Data["logo"]["thumbnailUrl"];
                        }
                    }
                    if (Operators.CompareString(this.LogoUrl, "", false) == 0)
                    {
                        this.LogoUrl = null;
                    }
                    if (this.Website.Contains("/mc-mods/") || this.Website.Contains("/mod/"))
                    {
                        this.Types = ResourceTypes.Mod;
                    }
                    else if (this.Website.Contains("/modpacks/"))
                    {
                        this.Types = ResourceTypes.ModPack;
                    }
                    else if (this.Website.Contains("/resourcepacks/"))
                    {
                        this.Types = ResourceTypes.ResourcePack;
                    }
                    else if (this.Website.Contains("/texture-packs/"))
                    {
                        this.Types = ResourceTypes.ResourcePack;
                    }
                    else if (this.Website.Contains("/shaders/"))
                    {
                        this.Types = ResourceTypes.Shader;
                    }
                    else
                    {
                        this.Types = ResourceTypes.DataPack;
                    }
                    this.ModLoaders = ModLoaders.None;
                    List<KeyValuePair<int, List<string>>> list = new List<KeyValuePair<int, List<string>>>();
                    try
                    {
                        foreach (JToken jtoken in ((IEnumerable<JToken>)(Data["latestFiles"] ?? new JArray())))
                        {
                            ResourceVersion resourceVersion = ResourceVersion.FromPlatformJson((JObject)jtoken, this.Types);
                            if (resourceVersion.DownloadAvailable)
                            {
                                this.ModLoaders |= resourceVersion.ModLoaders;
                                List<string> list2 = jtoken["gameVersions"].ToObject<List<string>>();
                                if (list2.Any<string>((ResourceProject._Closure___._I25_3 == null) ? (ResourceProject._Closure___._I25_3 = new Func<string, bool>(ResourceProject._Closure___._I._Lambda___25_3)) : ResourceProject._Closure___._I25_3))
                                {
                                    list.Add(new KeyValuePair<int, List<string>>((int)jtoken["id"], list2));
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
                    try
                    {
                        foreach (JToken jtoken2 in ((IEnumerable<JToken>)(Data["latestFilesIndexes"] ?? new JArray())))
                        {
                            if (McVersion.IsStandardFormat((string)jtoken2["gameVersion"]))
                            {
                                list.Add(new KeyValuePair<int, List<string>>((int)jtoken2["fileId"], new string[] { jtoken2["gameVersion"].ToString() }.ToList<string>()));
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
                    this.CurseForgeFileIds = list.Select<KeyValuePair<int, List<string>>, int>((ResourceProject._Closure___._I25_4 == null) ? (ResourceProject._Closure___._I25_4 = new Func<KeyValuePair<int, List<string>>, int>(ResourceProject._Closure___._I._Lambda___25_4)) : ResourceProject._Closure___._I25_4).Distinct<int>().ToList<int>();
                    this.Drops = list.SelectMany<KeyValuePair<int, List<string>>, string>((ResourceProject._Closure___._I25_5 == null) ? (ResourceProject._Closure___._I25_5 = new Func<KeyValuePair<int, List<string>>, IEnumerable<string>>(ResourceProject._Closure___._I._Lambda___25_5)) : ResourceProject._Closure___._I25_5).Select<string, int>((ResourceProject._Closure___._I25_6 == null) ? (ResourceProject._Closure___._I25_6 = new Func<string, int>(ResourceProject._Closure___._I._Lambda___25_6)) : ResourceProject._Closure___._I25_6).Where<int>((ResourceProject._Closure___._I25_7 == null) ? (ResourceProject._Closure___._I25_7 = new Func<int, bool>(ResourceProject._Closure___._I._Lambda___25_7)) : ResourceProject._Closure___._I25_7)
                        .Distinct<int>()
                        .OrderByDescending<int, int>((ResourceProject._Closure___._I25_8 == null) ? (ResourceProject._Closure___._I25_8 = new Func<int, int>(ResourceProject._Closure___._I._Lambda___25_8)) : ResourceProject._Closure___._I25_8)
                        .ToList<int>();
                    this.Tags = new List<string>();
                    try
                    {
                        foreach (int num in (Data["categories"] ?? new JArray()).Select<JToken, int>((ResourceProject._Closure___._I25_9 == null) ? (ResourceProject._Closure___._I25_9 = new Func<JToken, int>(ResourceProject._Closure___._I._Lambda___25_9)) : ResourceProject._Closure___._I25_9).Distinct<int>().OrderByDescending<int, int>((ResourceProject._Closure___._I25_10 == null) ? (ResourceProject._Closure___._I25_10 = new Func<int, int>(ResourceProject._Closure___._I._Lambda___25_10)) : ResourceProject._Closure___._I25_10))
                        {
                            if (num <= 5128)
                            {
                                if (num <= 4558)
                                {
                                    switch (num)
                                    {
                                    case 393:
                                        this.Tags.Add("16x");
                                        break;
                                    case 394:
                                        this.Tags.Add("32x");
                                        break;
                                    case 395:
                                        this.Tags.Add("64x");
                                        break;
                                    case 396:
                                        this.Tags.Add("128x");
                                        break;
                                    case 397:
                                        this.Tags.Add("256x");
                                        break;
                                    case 398:
                                        this.Tags.Add("超高清");
                                        break;
                                    case 399:
                                        this.Tags.Add("蒸汽朋克");
                                        break;
                                    case 400:
                                        this.Tags.Add("写实风");
                                        break;
                                    case 401:
                                        this.Tags.Add("现代风");
                                        break;
                                    case 402:
                                        this.Tags.Add("中世纪");
                                        break;
                                    case 403:
                                        this.Tags.Add("原版风");
                                        break;
                                    case 404:
                                        this.Tags.Add("动态效果");
                                        break;
                                    case 405:
                                    case 413:
                                    case 418:
                                    case 425:
                                    case 426:
                                    case 427:
                                    case 428:
                                    case 429:
                                    case 430:
                                    case 431:
                                    case 432:
                                    case 433:
                                        break;
                                    case 406:
                                        this.Tags.Add("世界元素");
                                        break;
                                    case 407:
                                        this.Tags.Add("生物群系");
                                        break;
                                    case 408:
                                        this.Tags.Add("矿物/资源");
                                        break;
                                    case 409:
                                        this.Tags.Add("天然结构");
                                        break;
                                    case 410:
                                        this.Tags.Add("维度");
                                        break;
                                    case 411:
                                        this.Tags.Add("生物");
                                        break;
                                    case 412:
                                        this.Tags.Add("科技");
                                        break;
                                    case 414:
                                        this.Tags.Add("运输");
                                        break;
                                    case 415:
                                        this.Tags.Add("管道/物流");
                                        break;
                                    case 416:
                                        this.Tags.Add("农业");
                                        break;
                                    case 417:
                                        this.Tags.Add("能源");
                                        break;
                                    case 419:
                                        this.Tags.Add("魔法");
                                        break;
                                    case 420:
                                        this.Tags.Add("仓储");
                                        break;
                                    case 421:
                                        this.Tags.Add("支持库");
                                        break;
                                    case 422:
                                        this.Tags.Add("冒险");
                                        break;
                                    case 423:
                                        this.Tags.Add("信息显示");
                                        break;
                                    case 424:
                                        this.Tags.Add("装饰");
                                        break;
                                    case 434:
                                        this.Tags.Add("装备");
                                        break;
                                    case 435:
                                        this.Tags.Add("服务器");
                                        break;
                                    case 436:
                                        this.Tags.Add("食物/烹饪");
                                        break;
                                    default:
                                        switch (num)
                                        {
                                        case 4465:
                                            this.Tags.Add("兼容 Mod");
                                            break;
                                        case 4466:
                                        case 4467:
                                        case 4468:
                                        case 4469:
                                        case 4470:
                                        case 4471:
                                        case 4485:
                                        case 4486:
                                            break;
                                        case 4472:
                                            this.Tags.Add("科技");
                                            break;
                                        case 4473:
                                            this.Tags.Add("魔法");
                                            break;
                                        case 4474:
                                            this.Tags.Add("科幻");
                                            break;
                                        case 4475:
                                            this.Tags.Add("冒险");
                                            break;
                                        case 4476:
                                            this.Tags.Add("探索");
                                            break;
                                        case 4477:
                                            this.Tags.Add("小游戏");
                                            break;
                                        case 4478:
                                            this.Tags.Add("任务");
                                            break;
                                        case 4479:
                                            this.Tags.Add("硬核");
                                            break;
                                        case 4480:
                                            this.Tags.Add("基于地图");
                                            break;
                                        case 4481:
                                            this.Tags.Add("轻量");
                                            break;
                                        case 4482:
                                            this.Tags.Add("大型");
                                            break;
                                        case 4483:
                                            this.Tags.Add("战斗");
                                            break;
                                        case 4484:
                                            this.Tags.Add("多人");
                                            break;
                                        case 4487:
                                            this.Tags.Add("FTB");
                                            break;
                                        default:
                                            if (num == 4558)
                                            {
                                                this.Tags.Add("红石");
                                            }
                                            break;
                                        }
                                        break;
                                    }
                                }
                                else if (num != 4736)
                                {
                                    if (num != 4843)
                                    {
                                        if (num == 5128)
                                        {
                                            this.Tags.Add("原版改良");
                                        }
                                    }
                                    else
                                    {
                                        this.Tags.Add("自动化");
                                    }
                                }
                                else
                                {
                                    this.Tags.Add("空岛");
                                }
                            }
                            else if (num <= 5244)
                            {
                                if (num != 5191)
                                {
                                    if (num != 5193)
                                    {
                                        if (num == 5244)
                                        {
                                            this.Tags.Add("含字体");
                                        }
                                    }
                                    else
                                    {
                                        this.Tags.Add("数据包");
                                    }
                                }
                                else
                                {
                                    this.Tags.Add("改良");
                                }
                            }
                            else if (num <= 6814)
                            {
                                switch (num)
                                {
                                case 6553:
                                    this.Tags.Add("写实风");
                                    break;
                                case 6554:
                                    this.Tags.Add("幻想风");
                                    break;
                                case 6555:
                                    this.Tags.Add("原版风");
                                    break;
                                default:
                                    if (num == 6814)
                                    {
                                        this.Tags.Add("性能优化");
                                    }
                                    break;
                                }
                            }
                            else
                            {
                                switch (num)
                                {
                                case 6946:
                                    this.Tags.Add("Mod 相关");
                                    break;
                                case 6947:
                                    break;
                                case 6948:
                                    this.Tags.Add("冒险");
                                    break;
                                case 6949:
                                    this.Tags.Add("幻想");
                                    break;
                                case 6950:
                                    this.Tags.Add("支持库");
                                    break;
                                case 6951:
                                    this.Tags.Add("科技");
                                    break;
                                case 6952:
                                    this.Tags.Add("魔法");
                                    break;
                                case 6953:
                                    this.Tags.Add("实用");
                                    break;
                                default:
                                    if (num == 9026)
                                    {
                                        this.Tags.Add("创造模式");
                                    }
                                    break;
                                }
                            }
                        }
                        goto IL_1ED9;
                    }
                    finally
                    {
                        IEnumerator<int> enumerator3 = default;
                        if (enumerator3 != null)
                        {
                            enumerator3.Dispose();
                        }
                    }
                }
                this.Id = (string)(Data["project_id"] ?? Data["id"]);
                this.Slug = (string)Data["slug"];
                this.RawName = (string)Data["title"];
                this.Description = (string)Data["description"];
                this.LastUpdate = (DateTime)(Data["date_modified"] ?? Data["updated"]);
                this.DownloadCount = (int)Data["downloads"];
                this.LogoUrl = (string)Data["icon_url"];
                if (Operators.CompareString(this.LogoUrl, "", false) == 0)
                {
                    this.LogoUrl = null;
                }
                this.Website = string.Format("https://modrinth.com/{0}/{1}", Data["project_type"], this.Slug);
                this.UnsafeGameVersions = (((JArray)(Data["game_versions"] ?? Data["versions"])) ?? new JArray()).Select<JToken, string>((ResourceProject._Closure___._I25_11 == null) ? (ResourceProject._Closure___._I25_11 = new Func<JToken, string>(ResourceProject._Closure___._I._Lambda___25_11)) : ResourceProject._Closure___._I25_11).Distinct<string>().ToList<string>();
                this.Drops = this.UnsafeGameVersions.Select<string, int>((ResourceProject._Closure___._I25_12 == null) ? (ResourceProject._Closure___._I25_12 = new Func<string, int>(ResourceProject._Closure___._I._Lambda___25_12)) : ResourceProject._Closure___._I25_12).Where<int>((ResourceProject._Closure___._I25_13 == null) ? (ResourceProject._Closure___._I25_13 = new Func<int, bool>(ResourceProject._Closure___._I._Lambda___25_13)) : ResourceProject._Closure___._I25_13).Distinct<int>()
                    .OrderByDescending<int, int>((ResourceProject._Closure___._I25_14 == null) ? (ResourceProject._Closure___._I25_14 = new Func<int, int>(ResourceProject._Closure___._I._Lambda___25_14)) : ResourceProject._Closure___._I25_14)
                    .ToList<int>();
                string text = Data["project_type"].ToString();
                if (Operators.CompareString(text, "modpack", false) != 0)
                {
                    if (Operators.CompareString(text, "resourcepack", false) != 0)
                    {
                        if (Operators.CompareString(text, "shader", false) != 0)
                        {
                            this.Types = ResourceTypes.Mod;
                        }
                        else
                        {
                            this.Types = ResourceTypes.Shader;
                        }
                    }
                    else
                    {
                        this.Types = ResourceTypes.ResourcePack;
                    }
                }
                else
                {
                    this.Types = ResourceTypes.ModPack;
                }
                this.Tags = new List<string>();
                this.ModLoaders = ModLoaders.None;
                try
                {
                    JToken jtoken3 = Data["loaders"];
                    foreach (string text2 in (((jtoken3 != null) ? jtoken3.Select<JToken, string>((ResourceProject._Closure___._I25_15 == null) ? (ResourceProject._Closure___._I25_15 = new Func<JToken, string>(ResourceProject._Closure___._I._Lambda___25_15)) : ResourceProject._Closure___._I25_15) : null) ?? new string[0]).Concat<string>(Data["categories"].Select<JToken, string>((ResourceProject._Closure___._I25_16 == null) ? (ResourceProject._Closure___._I25_16 = new Func<JToken, string>(ResourceProject._Closure___._I._Lambda___25_16)) : ResourceProject._Closure___._I25_16)))
                    {
                        try
                        {
                            foreach (ModLoaders modLoaders in EnumUtils.GetAllFlags<ModLoaders>())
                            {
                                if (Operators.CompareString(text2, modLoaders.ToString().Lower(), false) == 0)
                                {
                                    this.ModLoaders |= modLoaders;
                                    break;
                                }
                            }
                        }
                        finally
                        {
                            List<ModLoaders>.Enumerator enumerator5 = default;
                            ((IDisposable)enumerator5).Dispose();
                        }
                    }
                }
                finally
                {
                    IEnumerator<string> enumerator4 = default;
                    if (enumerator4 != null)
                    {
                        enumerator4.Dispose();
                    }
                }
                try
                {
                    foreach (string text3 in Data["categories"].Select<JToken, string>((ResourceProject._Closure___._I25_17 == null) ? (ResourceProject._Closure___._I25_17 = new Func<JToken, string>(ResourceProject._Closure___._I._Lambda___25_17)) : ResourceProject._Closure___._I25_17))
                    {
                        uint num2 = PrivateImplementationDetails_.ComputeStringHash(text3);
                        if (num2 <= 1678477998U)
                        {
                            if (num2 <= 889035115U)
                            {
                                if (num2 <= 564424733U)
                                {
                                    if (num2 <= 126402435U)
                                    {
                                        if (num2 != 21694712U)
                                        {
                                            if (num2 != 30055014U)
                                            {
                                                if (num2 == 126402435U)
                                                {
                                                    if (Operators.CompareString(text3, "decoration", false) == 0)
                                                    {
                                                        if (this.Types != ResourceTypes.ResourcePack)
                                                        {
                                                            this.Tags.Add("装饰");
                                                        }
                                                    }
                                                }
                                            }
                                            else if (Operators.CompareString(text3, "storage", false) == 0)
                                            {
                                                this.Tags.Add("仓储");
                                            }
                                        }
                                        else if (Operators.CompareString(text3, "32x", false) == 0)
                                        {
                                            this.Tags.Add("32x");
                                        }
                                    }
                                    else if (num2 != 343003787U)
                                    {
                                        if (num2 != 484666913U)
                                        {
                                            if (num2 == 564424733U)
                                            {
                                                if (Operators.CompareString(text3, "game-mechanics", false) == 0)
                                                {
                                                    this.Tags.Add("游戏机制");
                                                }
                                            }
                                        }
                                        else if (Operators.CompareString(text3, "combat", false) == 0)
                                        {
                                            this.Tags.Add("战斗");
                                        }
                                    }
                                    else if (Operators.CompareString(text3, "models", false) == 0)
                                    {
                                        this.Tags.Add("含模型");
                                    }
                                }
                                else if (num2 <= 782861478U)
                                {
                                    if (num2 != 679943061U)
                                    {
                                        if (num2 != 765533517U)
                                        {
                                            if (num2 == 782861478U)
                                            {
                                                if (Operators.CompareString(text3, "iris", false) == 0)
                                                {
                                                    this.Tags.Add("Iris");
                                                }
                                            }
                                        }
                                        else if (Operators.CompareString(text3, "core-shaders", false) == 0)
                                        {
                                            this.Tags.Add("核心着色器");
                                        }
                                    }
                                    else if (Operators.CompareString(text3, "path-tracing", false) == 0)
                                    {
                                        this.Tags.Add("路径追踪");
                                    }
                                }
                                else if (num2 != 858719634U)
                                {
                                    if (num2 != 859535076U)
                                    {
                                        if (num2 == 889035115U)
                                        {
                                            if (Operators.CompareString(text3, "lightweight", false) == 0)
                                            {
                                                this.Tags.Add("轻量");
                                            }
                                        }
                                    }
                                    else if (Operators.CompareString(text3, "tweaks", false) == 0)
                                    {
                                        this.Tags.Add("改良");
                                    }
                                }
                                else if (Operators.CompareString(text3, "16x", false) == 0)
                                {
                                    this.Tags.Add("16x");
                                }
                            }
                            else if (num2 <= 1446847267U)
                            {
                                if (num2 <= 1028682697U)
                                {
                                    if (num2 != 926227488U)
                                    {
                                        if (num2 != 934491434U)
                                        {
                                            if (num2 == 1028682697U)
                                            {
                                                if (Operators.CompareString(text3, "food", false) == 0)
                                                {
                                                    this.Tags.Add("食物/烹饪");
                                                }
                                            }
                                        }
                                        else if (Operators.CompareString(text3, "128x", false) == 0)
                                        {
                                            this.Tags.Add("128x");
                                        }
                                    }
                                    else if (Operators.CompareString(text3, "gui", false) == 0)
                                    {
                                        this.Tags.Add("含 UI");
                                    }
                                }
                                else if (num2 != 1360377439U)
                                {
                                    if (num2 != 1418100143U)
                                    {
                                        if (num2 == 1446847267U)
                                        {
                                            if (Operators.CompareString(text3, "transportation", false) == 0)
                                            {
                                                this.Tags.Add("运输");
                                            }
                                        }
                                    }
                                    else if (Operators.CompareString(text3, "technology", false) == 0)
                                    {
                                        this.Tags.Add("科技");
                                    }
                                }
                                else if (Operators.CompareString(text3, "realistic", false) == 0)
                                {
                                    this.Tags.Add("写实风");
                                }
                            }
                            else if (num2 <= 1564096620U)
                            {
                                if (num2 != 1464635778U)
                                {
                                    if (num2 != 1546653040U)
                                    {
                                        if (num2 == 1564096620U)
                                        {
                                            if (Operators.CompareString(text3, "simplistic", false) == 0)
                                            {
                                                this.Tags.Add("简洁");
                                            }
                                        }
                                    }
                                    else if (Operators.CompareString(text3, "vanilla", false) == 0)
                                    {
                                        this.Tags.Add("原版可用");
                                    }
                                }
                                else if (Operators.CompareString(text3, "semi-realistic", false) == 0)
                                {
                                    this.Tags.Add("半写实风");
                                }
                            }
                            else if (num2 <= 1636821932U)
                            {
                                if (num2 != 1565668191U)
                                {
                                    if (num2 == 1636821932U)
                                    {
                                        if (Operators.CompareString(text3, "modded", false) == 0)
                                        {
                                            this.Tags.Add("兼容 Mod");
                                        }
                                    }
                                }
                                else if (Operators.CompareString(text3, "pbr", false) == 0)
                                {
                                    this.Tags.Add("PBR");
                                }
                            }
                            else if (num2 != 1657676564U)
                            {
                                if (num2 == 1678477998U)
                                {
                                    if (Operators.CompareString(text3, "256x", false) == 0)
                                    {
                                        this.Tags.Add("256x");
                                    }
                                }
                            }
                            else if (Operators.CompareString(text3, "vanilla-like", false) == 0)
                            {
                                this.Tags.Add("原版风");
                            }
                        }
                        else if (num2 <= 3217471839U)
                        {
                            if (num2 <= 2432105424U)
                            {
                                if (num2 <= 1955283044U)
                                {
                                    if (num2 != 1796984983U)
                                    {
                                        if (num2 != 1855291081U)
                                        {
                                            if (num2 == 1955283044U)
                                            {
                                                if (Operators.CompareString(text3, "optimization", false) == 0)
                                                {
                                                    this.Tags.Add("性能优化");
                                                }
                                            }
                                        }
                                        else if (Operators.CompareString(text3, "cartoon", false) == 0)
                                        {
                                            this.Tags.Add("卡通风");
                                        }
                                    }
                                    else if (Operators.CompareString(text3, "locale", false) == 0)
                                    {
                                        this.Tags.Add("含语言");
                                    }
                                }
                                else if (num2 != 2117003656U)
                                {
                                    if (num2 != 2216639593U)
                                    {
                                        if (num2 == 2432105424U)
                                        {
                                            if (Operators.CompareString(text3, "library", false) == 0)
                                            {
                                                this.Tags.Add("支持库");
                                            }
                                        }
                                    }
                                    else if (Operators.CompareString(text3, "optifine", false) == 0)
                                    {
                                        this.Tags.Add("OptiFine");
                                    }
                                }
                                else if (Operators.CompareString(text3, "colored-lighting", false) == 0)
                                {
                                    this.Tags.Add("彩色光照");
                                }
                            }
                            else if (num2 <= 2930129973U)
                            {
                                if (num2 != 2745962869U)
                                {
                                    if (num2 != 2898550409U)
                                    {
                                        if (num2 == 2930129973U)
                                        {
                                            if (Operators.CompareString(text3, "fantasy", false) == 0)
                                            {
                                                this.Tags.Add("幻想风");
                                            }
                                        }
                                    }
                                    else if (Operators.CompareString(text3, "challenging", false) == 0)
                                    {
                                        this.Tags.Add("硬核");
                                    }
                                }
                                else if (Operators.CompareString(text3, "reflections", false) == 0)
                                {
                                    this.Tags.Add("反射");
                                }
                            }
                            else if (num2 <= 3001865938U)
                            {
                                if (num2 != 2954106368U)
                                {
                                    if (num2 == 3001865938U)
                                    {
                                        if (Operators.CompareString(text3, "social", false) == 0)
                                        {
                                            this.Tags.Add("服务器");
                                        }
                                    }
                                }
                                else if (Operators.CompareString(text3, "8x-", false) == 0)
                                {
                                    this.Tags.Add("极简");
                                }
                            }
                            else if (num2 != 3037049615U)
                            {
                                if (num2 == 3217471839U)
                                {
                                    if (Operators.CompareString(text3, "utility", false) == 0)
                                    {
                                        this.Tags.Add("实用");
                                    }
                                }
                            }
                            else if (Operators.CompareString(text3, "worldgen", false) == 0)
                            {
                                this.Tags.Add("世界元素");
                            }
                        }
                        else if (num2 <= 3366373741U)
                        {
                            if (num2 <= 3294764480U)
                            {
                                if (num2 != 3249952714U)
                                {
                                    if (num2 != 3269966681U)
                                    {
                                        if (num2 == 3294764480U)
                                        {
                                            if (Operators.CompareString(text3, "datapack", false) == 0)
                                            {
                                                this.Types = ResourceTypes.DataPack;
                                            }
                                        }
                                    }
                                    else if (Operators.CompareString(text3, "fonts", false) == 0)
                                    {
                                        this.Tags.Add("含字体");
                                    }
                                }
                                else if (Operators.CompareString(text3, "quests", false) == 0)
                                {
                                    this.Tags.Add("任务");
                                }
                            }
                            else if (num2 != 3302598349U)
                            {
                                if (num2 != 3306152395U)
                                {
                                    if (num2 == 3366373741U)
                                    {
                                        if (Operators.CompareString(text3, "64x", false) == 0)
                                        {
                                            this.Tags.Add("64x");
                                        }
                                    }
                                }
                                else if (Operators.CompareString(text3, "adventure", false) == 0)
                                {
                                    this.Tags.Add("冒险");
                                }
                            }
                            else if (Operators.CompareString(text3, "kitchen-sink", false) == 0)
                            {
                                this.Tags.Add("水槽包");
                            }
                        }
                        else if (num2 <= 3796101307U)
                        {
                            if (num2 != 3463588087U)
                            {
                                if (num2 != 3764468121U)
                                {
                                    if (num2 == 3796101307U)
                                    {
                                        if (Operators.CompareString(text3, "48x", false) == 0)
                                        {
                                            this.Tags.Add("48x");
                                        }
                                    }
                                }
                                else if (Operators.CompareString(text3, "audio", false) == 0)
                                {
                                    this.Tags.Add("含声音");
                                }
                            }
                            else if (Operators.CompareString(text3, "equipment", false) == 0)
                            {
                                if (this.Types != ResourceTypes.ResourcePack)
                                {
                                    this.Tags.Add("装备");
                                }
                            }
                        }
                        else if (num2 <= 4053708971U)
                        {
                            if (num2 != 3840615820U)
                            {
                                if (num2 == 4053708971U)
                                {
                                    if (Operators.CompareString(text3, "multiplayer", false) == 0)
                                    {
                                        this.Tags.Add("多人");
                                    }
                                }
                            }
                            else if (Operators.CompareString(text3, "magic", false) == 0)
                            {
                                this.Tags.Add("魔法");
                            }
                        }
                        else if (num2 != 4091869486U)
                        {
                            if (num2 == 4269569010U)
                            {
                                if (Operators.CompareString(text3, "mobs", false) == 0)
                                {
                                    if (this.Types != ResourceTypes.ResourcePack)
                                    {
                                        this.Tags.Add("生物");
                                    }
                                }
                            }
                        }
                        else if (Operators.CompareString(text3, "512x+", false) == 0)
                        {
                            this.Tags.Add("超高清");
                        }
                    }
                }
                finally
                {
                    IEnumerator<string> enumerator6 = default;
                    if (enumerator6 != null)
                    {
                        enumerator6.Dispose();
                    }
                }
                if (this.Types == ResourceTypes.DataPack && this.ModLoaders != ModLoaders.None)
                {
                    this.Types = ResourceTypes.ModOrDataPack;
                }
                IL_1ED9:
                if (!this.Tags.Any<string>())
                {
                    this.Tags.Add("其他");
                }
                this.Tags.Sort();
            }
            ResourceProject.Cache[this.Id] = this;
        }

        public static ResourceProject FromProjectId(string ProjectId, ResourcePlatforms Platform)
        {
            if (!Platform.Flags<ResourcePlatforms>().IsSingle<ResourcePlatforms>())
            {
                throw new ArgumentException(string.Format("必须指定单一平台，当前值为 {0}", Platform));
            }
            ResourceProject resourceProject;
            if (ResourceProject.Cache.ContainsKey(ProjectId))
            {
                resourceProject = ResourceProject.Cache[ProjectId];
            }
            else if (Platform == ResourcePlatforms.CurseForge)
            {
                resourceProject = new ResourceProject((JObject)ModDownload.DlModRequest("https://api.curseforge.com/v1/mods/" + ProjectId, null, null, null)["data"]);
            }
            else
            {
                resourceProject = new ResourceProject((JObject)ModDownload.DlModRequest("https://api.modrinth.com/v2/project/" + ProjectId, null, null, null));
            }
            return resourceProject;
        }

        public static IEnumerable<ResourceProject> FromProjectIds(IEnumerable<string> ProjectIds, ResourcePlatforms Platform)
        {
            ResourceProject.VB_StateMachine_27_FromProjectIds vb_StateMachine_27_FromProjectIds = new ResourceProject.VB_StateMachine_27_FromProjectIds(-2);
            vb_StateMachine_27_FromProjectIds._P_ProjectIds = ProjectIds;
            vb_StateMachine_27_FromProjectIds._P_Platform = Platform;
            return vb_StateMachine_27_FromProjectIds;
        }

        public JObject ToJson()
        {
            JObject jobject = new JObject();
            jobject["Platform"] = (int)this.Platform;
            jobject["Types"] = (int)this.Types;
            jobject["Slug"] = this.Slug;
            jobject["Id"] = this.Id;
            if (this.CurseForgeFileIds != null)
            {
                jobject["CurseForgeFileIds"] = new JArray(this.CurseForgeFileIds);
            }
            jobject["RawName"] = this.RawName;
            jobject["Description"] = this.Description;
            jobject["Website"] = this.Website;
            jobject["LastUpdate"] = this.LastUpdate;
            jobject["DownloadCount"] = this.DownloadCount;
            if (this.ModLoaders != ModLoaders.None)
            {
                jobject["ModLoaders"] = (int)this.ModLoaders;
            }
            jobject["Tags"] = new JArray(this.Tags);
            if (this.LogoUrl != null)
            {
                jobject["LogoUrl"] = this.LogoUrl;
            }
            if (this.Drops.Any<int>())
            {
                jobject["Drops"] = new JArray(this.Drops);
            }
            jobject["CacheTime"] = DateTime.Now;
            return jobject;
        }

        public OurVirtualizingElement<OurResourceItem> ToResourceItem(bool ShowMcVersionDesc, bool ShowLoaderDesc)
        {
            ResourceProject._Closure___29_0 CS_locals1 = new ResourceProject._Closure___29_0(null);
            CS_locals1._VB_Me = this;
            CS_locals1._VB_Local_ShowMcVersionDesc = ShowMcVersionDesc;
            CS_locals1._VB_Local_ShowLoaderDesc = ShowLoaderDesc;
            checked
            {
                if (this.Drops == null || !this.Drops.Any<int>())
                {
                    CS_locals1._VB_Local_GameVersionDescription = "仅快照版本";
                }
                else
                {
                    List<string> list = new List<string>();
                    bool flag = false;
                    int num = this.Drops.Count - 1;
                    for (int i = 0; i <= num; i++)
                    {
                        int num2 = this.Drops[i];
                        int num3 = this.Drops[i];
                        if (num2 < 120)
                        {
                            if (list.Any<string>() && !flag)
                            {
                                break;
                            }
                            flag = true;
                        }
                        int num4 = i + 1;
                        int num5 = this.Drops.Count - 1;
                        int num6 = num4;
                        while (num6 <= num5 && ModDownload.AllDrops != null && ModDownload.AllDrops.IndexOf(this.Drops[num6]) == ModDownload.AllDrops.IndexOf(num3) + 1)
                        {
                            num3 = this.Drops[num6];
                            i = num6;
                            num6++;
                        }
                        string text = McVersion.DropToVersion(num2);
                        string text2 = McVersion.DropToVersion(num3);
                        if (num2 == num3)
                        {
                            list.Add(text);
                        }
                        else if (ModDownload.AllDrops != null && num2 >= ModDownload.AllDrops.First<int>())
                        {
                            if (num3 < 120)
                            {
                                list.Clear();
                                list.Add("全版本");
                                break;
                            }
                            list.Add(text2 + "+");
                        }
                        else
                        {
                            if (num3 < 120)
                            {
                                list.Add(text + "-");
                                break;
                            }
                            if (ModDownload.AllDrops == null || ModDownload.AllDrops.IndexOf(num3) - ModDownload.AllDrops.IndexOf(num2) == 1)
                            {
                                list.Add(text + ", " + text2);
                            }
                            else
                            {
                                list.Add(text + "~" + text2);
                            }
                        }
                    }
                    CS_locals1._VB_Local_GameVersionDescription = list.Join(", ");
                }
                List<ModLoaders> list2 = this.ModLoaders.Flags<ModLoaders>().ToList<ModLoaders>();
                int count = list2.Count;
                if (count != 0)
                {
                    if (count != 1)
                    {
                        int num7 = (this.Drops.Any<int>() ? this.Drops.First<int>() : 9999);
                        if (list2.Contains(ModLoaders.Forge) && (num7 < 140 || list2.Contains(ModLoaders.Fabric)) && (num7 < 200 || list2.Contains(ModLoaders.NeoForge)))
                        {
                            CS_locals1._VB_Local_ModLoaderDescriptionFull = "任意";
                            CS_locals1._VB_Local_ModLoaderDescriptionPart = "";
                        }
                        else
                        {
                            CS_locals1._VB_Local_ModLoaderDescriptionFull = list2.Join(" / ");
                            CS_locals1._VB_Local_ModLoaderDescriptionPart = list2.Join(" / ");
                        }
                    }
                    else
                    {
                        CS_locals1._VB_Local_ModLoaderDescriptionFull = "仅 " + list2.Single<ModLoaders>().ToString();
                        CS_locals1._VB_Local_ModLoaderDescriptionPart = list2.Single<ModLoaders>().ToString();
                    }
                }
                else if (list2.IsSingle<ModLoaders>())
                {
                    CS_locals1._VB_Local_ModLoaderDescriptionFull = "仅 " + list2.Single<ModLoaders>().ToString();
                    CS_locals1._VB_Local_ModLoaderDescriptionPart = list2.Single<ModLoaders>().ToString();
                }
                else
                {
                    CS_locals1._VB_Local_ModLoaderDescriptionFull = "未知";
                    CS_locals1._VB_Local_ModLoaderDescriptionPart = "";
                }
                return new OurVirtualizingElement<OurResourceItem>(new Func<OurResourceItem>(CS_locals1._Lambda___0))
                {
                    Height = 64.0
                };
            }
        }

        public void ApplyLogoToOurImage(OurImage Img)
        {
            if (string.IsNullOrEmpty(this.LogoUrl))
            {
                Img.Source = ModBase.PathImage + "Icons/NoIcon.png";
                return;
            }
            Img.Source = this.LogoUrl;
            Img.FallbackSource = ModDownload.DlSourceModGet(this.LogoUrl);
        }

        public ValueTuple<string, string> GetControlTitle(bool HasModLoaderDescription)
        {
            string text = this.RawName;
            List<string> list;
            if (Operators.CompareString(this.TranslatedName, this.RawName, false) == 0)
            {
                IEnumerable<string> enumerable = this.TranslatedName.Split(new string[] { " | ", " - ", "(", ")", "[", "]", "{", "}" }, true).Select<string, string>((ResourceProject._Closure___._I31_0 == null) ? (ResourceProject._Closure___._I31_0 = new Func<string, string>(ResourceProject._Closure___._I._Lambda___31_0)) : ResourceProject._Closure___._I31_0).Where<string>((ResourceProject._Closure___._I31_1 == null) ? (ResourceProject._Closure___._I31_1 = new Func<string, bool>(ResourceProject._Closure___._I._Lambda___31_1)) : ResourceProject._Closure___._I31_1);
                if (enumerable.IsSingle<string>())
                {
                    goto IL_05EC;
                }
                list = new List<string>();
                List<string> list2 = new List<string>();
                try
                {
                    foreach (string text2 in enumerable)
                    {
                        ResourceProject._Closure___31_0 CS_locals1 = new ResourceProject._Closure___31_0(null);
                        CS_locals1._VB_Local_LowerName = text2.Lower();
                        if (Operators.CompareString(text2, text2.Upper(), false) == 0 && Operators.CompareString(text2, "FPS", false) != 0 && Operators.CompareString(text2, "HUD", false) != 0)
                        {
                            list.Add(text2);
                        }
                        else if (new string[] { "neoforge", "forge", "fabric", "quilt" }.Any<string>(new Func<string, bool>(CS_locals1._Lambda___2)) && !CS_locals1._VB_Local_LowerName.Replace("neoforge", "").Replace("forge", "").Replace("fabric", "")
                            .Replace("quilt", "")
                            .RegexCheck("[a-z]+", RegexOptions.None))
                        {
                            list.Add(text2);
                        }
                        else
                        {
                            list2.Add(text2);
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
                if (!list2.Any<string>() || !list.Any<string>())
                {
                    goto IL_05EC;
                }
                text = list2.Join(" - ");
            }
            else
            {
                text = (this.TranslatedName.Contains(" (") ? this.TranslatedName.BeforeFirst(" (", false) : this.TranslatedName.BeforeLast(" - ", false));
                string text3 = "";
                if (this.TranslatedName.AfterLast(")", false).Contains(" - "))
                {
                    text3 = this.TranslatedName.AfterLast(")", false).AfterLast(" - ", false);
                }
                string text4 = this.TranslatedName;
                if (Operators.CompareString(text3, "", false) != 0)
                {
                    text4 = text4.Replace(" - " + text3, "");
                }
                text4 = text4.Replace(text, "").Trim(new char[] { '(', ')', ' ' });
                list = text4.Split(new string[] { " | ", " - ", "(", ")", "[", "]", "{", "}" }, true).Select<string, string>((ResourceProject._Closure___._I31_3 == null) ? (ResourceProject._Closure___._I31_3 = new Func<string, string>(ResourceProject._Closure___._I._Lambda___31_3)) : ResourceProject._Closure___._I31_3).Where<string>((ResourceProject._Closure___._I31_4 == null) ? (ResourceProject._Closure___._I31_4 = new Func<string, bool>(ResourceProject._Closure___._I._Lambda___31_4)) : ResourceProject._Closure___._I31_4)
                    .ToList<string>();
                if (list.Count > 1 && !list.Any<string>((ResourceProject._Closure___._I31_5 == null) ? (ResourceProject._Closure___._I31_5 = new Func<string, bool>(ResourceProject._Closure___._I._Lambda___31_5)) : ResourceProject._Closure___._I31_5) && (list.Count != 2 || Operators.CompareString(list.Last<string>().Upper(), list.Last<string>(), false) != 0))
                {
                    list = new List<string> { text4 };
                }
                if (Operators.CompareString(text3, "", false) != 0)
                {
                    list.Add(text3);
                }
            }
            list = list.Distinct<string>().ToList<string>();
            string text5 = "";
            if (list.Any<string>())
            {
                try
                {
                    foreach (string text6_it in list)
                    {
                        string text6 = text6_it;
                        bool flag = text6.Lower().Contains("neoforge") || text6.Lower().Contains("forge") || text6.Lower().Contains("fabric") || text6.Lower().Contains("quilt");
                        if ((HasModLoaderDescription || !flag) && (text6.Length >= 16 || !text6.Lower().Contains("fabric") || !text6.Lower().Contains("forge")))
                        {
                            if (flag && !text6.Contains("版") && text6.Lower().Replace("neoforge", "").Replace("forge", "")
                                .Replace("fabric", "")
                                .Replace("quilt", "")
                                .Length <= 3)
                            {
                                text6 = text6.Replace("Edition", "").Replace("edition", "").Trim()
                                    .Capitalize() + " 版";
                            }
                            text6 = text6.Replace("neoforge", "NeoForge").Replace("forge", "Forge").Replace("neo", "Neo")
                                .Replace("fabric", "Fabric")
                                .Replace("quilt", "Quilt");
                            text5 = text5 + "  |  " + text6.Trim();
                        }
                    }
                    goto IL_05F2;
                }
                finally
                {
                    List<string>.Enumerator enumerator2 = default;
                    ((IDisposable)enumerator2).Dispose();
                }
            }
            IL_05EC:
            text5 = "";
            IL_05F2:
            return new ValueTuple<string, string>(text, text5);
        }

        public bool IsLike(ResourceProject Project)
        {
            bool flag;
            if (Operators.CompareString(this.Id, Project.Id, false) == 0)
            {
                flag = true;
            }
            else
            {
                VB_AnonymousDelegate_1<string, string> vb_AnonymousDelegate_ = ((ResourceProject._Closure___._I33_0 == null) ? (ResourceProject._Closure___._I33_0 = new VB_AnonymousDelegate_1<string, string>(ResourceProject._Closure___._I._Lambda___33_0)) : ResourceProject._Closure___._I33_0);
                if (this.Platform == Project.Platform)
                {
                    flag = false;
                }
                else if (this.ModLoaders != Project.ModLoaders)
                {
                    flag = false;
                }
                else if (this.Types != ResourceTypes.Shader && this.Drops.Ordered<int>().SequenceEqual<int>(Project.Drops.Ordered<int>()))
                {
                    flag = false;
                }
                else if (Math.Abs((this.LastUpdate - Project.LastUpdate).TotalDays) > 7.0)
                {
                    flag = false;
                }
                else if (Operators.CompareString(this.TranslatedName, Project.TranslatedName, false) == 0 || Operators.CompareString(this.RawName, Project.RawName, false) == 0 || Operators.CompareString(this.Description, Project.Description, false) == 0 || Operators.CompareString(vb_AnonymousDelegate_(this.Slug), vb_AnonymousDelegate_(Project.Slug), false) == 0)
                {
                    Logger.Info(string.Format("将 {0} ({1}) 与 {2} ({3}) 认定为相似工程", new object[] { this.RawName, this.Slug, Project.RawName, Project.Slug }), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Resource\\ResourceProject.vb");
                    if (this.WikiEntry == null && Project.WikiEntry != null)
                    {
                        this.WikiEntry = Project.WikiEntry;
                    }
                    if (this.WikiEntry != null && Project.WikiEntry == null)
                    {
                        Project.WikiEntry = this.WikiEntry;
                    }
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            return flag;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}): {2}", this.Id, this.Slug, this.RawName);
        }

        public override bool Equals(object obj)
        {
            ResourceProject resourceProject = obj as ResourceProject;
            return ((object)resourceProject != (object)null) && Operators.CompareString(this.Id, resourceProject.Id, false) == 0;
        }

        public static bool operator ==(ResourceProject left, ResourceProject right)
        {
            return EqualityComparer<ResourceProject>.Default.Equals(left, right);
        }

        public static bool operator !=(ResourceProject left, ResourceProject right)
        {
            return !(((object)left == (object)right));
        }

        [CompilerGenerated]
        private bool _Lambda___19_0(WikiEntry c)
        {
            return Operators.CompareString(c.Slugs.GetOrDefault(this.Platform, null), this.Slug, false) == 0;
        }

        public readonly ResourcePlatforms Platform;

        public readonly ResourceTypes Types;

        public readonly string Slug;

        public readonly string Id;

        public readonly List<int> CurseForgeFileIds;

        public readonly string RawName;

        public readonly string Description;

        public readonly string Website;

        public readonly DateTime LastUpdate;

        public readonly int DownloadCount;

        public readonly ModLoaders ModLoaders;

        public readonly List<string> Tags;

        public string LogoUrl;

        public readonly List<int> Drops;

        public readonly List<string> UnsafeGameVersions;

        private bool LoadedDatabase;

        private WikiEntry _WikiEntry;

        protected internal static Dictionary<string, ResourceProject> Cache = new Dictionary<string, ResourceProject>();

        [CompilerGenerated]
        private sealed class VB_StateMachine_27_FromProjectIds : IEnumerable<ResourceProject>, IEnumerable, IEnumerator<ResourceProject>, IDisposable, IEnumerator
        {
            public VB_StateMachine_27_FromProjectIds(int _State)
            {
                this._State = _State;
                this._InitialThreadId = Environment.CurrentManagedThreadId;
            }

            void IDisposable.Dispose()
            {
                int num = this._State;
                if (num != 1)
                {
                    if (num != 2)
                    {
                        this._State = -1;
                    }
                    else
                    {
                        this._State = -4;
                    }
                }
                else
                {
                    this._State = -3;
                }
                this.MoveNext();
                this._State = -2;
            }

            [CompilerGenerated]
            public bool MoveNext()
            {
                JArray jarray = default;
                int num = this._State;
                switch (num)
                {
                case -4:
                case 2:
                    goto IL_019E;
                case -3:
                case 1:
                    break;
                default:
                    return false;
                case 0:
                    num = (this._State = -1);
                    if (!this._VB_Local_Platform.Flags<ResourcePlatforms>().IsSingle<ResourcePlatforms>())
                    {
                        throw new ArgumentException(string.Format("必须指定单一平台，当前值为 {0}", this._VB_Local_Platform));
                    }
                    this._VB_ResumableLocal_MissingProjectIds_1 = new List<string>();
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
                        this._S0 = this._VB_Local_ProjectIds.GetEnumerator();
                    }
                    else
                    {
                        num = (this._State = -1);
                    }
                    while (this._S0.MoveNext())
                    {
                        string text = this._S0.Current;
                        if (ResourceProject.Cache.ContainsKey(text))
                        {
                            this._Current = ResourceProject.Cache[text];
                            num = (this._State = 1);
                            return true;
                        }
                        this._VB_ResumableLocal_MissingProjectIds_1.Add(text);
                    }
                }
                finally
                {
                    if (num < 0 && this._S0 != null)
                    {
                        this._S0.Dispose();
                    }
                }
                if (!this._VB_ResumableLocal_MissingProjectIds_1.Any<string>())
                {
                    return false;
                }
                if (this._VB_Local_Platform.HasFlagF(ResourcePlatforms.CurseForge))
                {
                    jarray = (JArray)ModDownload.DlModRequest("https://api.curseforge.com/v1/mods", HttpMethod.Post, "{\"modIds\": [" + this._VB_ResumableLocal_MissingProjectIds_1.Join(',') + "]}", "application/json")["data"];
                }
                else
                {
                    jarray = (JArray)ModDownload.DlModRequest(string.Format("https://api.modrinth.com/v2/projects?ids=[\"{0}\"]", this._VB_ResumableLocal_MissingProjectIds_1.Join("\",\"")), null, null, null);
                }
                IL_019E:
                try
                {
                    if (num == -4)
                    {
                        num = (this._State = -1);
                        return true;
                    }
                    if (num != 2)
                    {
                        this._S2 = jarray.GetEnumerator();
                    }
                    else
                    {
                        num = (this._State = -1);
                    }
                    if (this._S2.MoveNext())
                    {
                        JToken jtoken = this._S2.Current;
                        this._Current = new ResourceProject((JObject)jtoken);
                        num = (this._State = 2);
                        return true;
                    }
                }
                finally
                {
                    if (num < 0 && this._S2 != null)
                    {
                        this._S2.Dispose();
                    }
                }
                return false;
            }

            public IEnumerator<ResourceProject> GetEnumerator()
            {
                ResourceProject.VB_StateMachine_27_FromProjectIds vb_StateMachine_27_FromProjectIds;
                if (this._State == -2 && this._InitialThreadId == Environment.CurrentManagedThreadId)
                {
                    this._State = 0;
                    vb_StateMachine_27_FromProjectIds = this;
                }
                else
                {
                    vb_StateMachine_27_FromProjectIds = new ResourceProject.VB_StateMachine_27_FromProjectIds(0);
                }
                vb_StateMachine_27_FromProjectIds._VB_Local_ProjectIds = this._P_ProjectIds;
                vb_StateMachine_27_FromProjectIds._VB_Local_Platform = this._P_Platform;
                return vb_StateMachine_27_FromProjectIds;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            ResourceProject IEnumerator<ResourceProject>.Current
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

            public ResourceProject _Current;

            public int _InitialThreadId;

            internal IEnumerable<string> _VB_Local_ProjectIds;

            internal IEnumerable<string> _P_ProjectIds;

            internal ResourcePlatforms _VB_Local_Platform;

            internal ResourcePlatforms _P_Platform;

            internal IEnumerator<string> _S0;

            internal List<string> _VB_ResumableLocal_MissingProjectIds_1;

            internal IEnumerator<JToken> _S2;
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

            internal int _Lambda___25_0(JToken t)
            {
                return t.ToObject<int>();
            }

            internal string _Lambda___25_1(JToken t)
            {
                return t.ToString();
            }

            internal int _Lambda___25_2(JToken t)
            {
                return t.ToObject<int>();
            }

            internal bool _Lambda___25_3(string v)
            {
                return McVersion.IsStandardFormat(v);
            }

            internal int _Lambda___25_4(KeyValuePair<int, List<string>> f)
            {
                return f.Key;
            }

            internal IEnumerable<string> _Lambda___25_5(KeyValuePair<int, List<string>> f)
            {
                return f.Value;
            }

            internal int _Lambda___25_6(string v)
            {
                return McVersion.VersionToDrop(v);
            }

            internal bool _Lambda___25_7(int v)
            {
                return v != 209;
            }

            internal int _Lambda___25_8(int v)
            {
                return v;
            }

            internal int _Lambda___25_9(JToken t)
            {
                return (int)t["id"];
            }

            internal int _Lambda___25_10(int c)
            {
                return c;
            }

            internal string _Lambda___25_11(JToken v)
            {
                return v.ToString();
            }

            internal int _Lambda___25_12(string v)
            {
                return McVersion.VersionToDrop(v);
            }

            internal bool _Lambda___25_13(int v)
            {
                return v != 209;
            }

            internal int _Lambda___25_14(int v)
            {
                return v;
            }

            internal string _Lambda___25_15(JToken t)
            {
                return t.ToString();
            }

            internal string _Lambda___25_16(JToken t)
            {
                return t.ToString();
            }

            internal string _Lambda___25_17(JToken t)
            {
                return t.ToString();
            }

            internal string _Lambda___31_0(string s)
            {
                return s.Trim(" /\\".ToCharArray());
            }

            internal bool _Lambda___31_1(string w)
            {
                return !string.IsNullOrEmpty(w);
            }

            internal string _Lambda___31_3(string s)
            {
                return s.Trim(" /".ToCharArray());
            }

            internal bool _Lambda___31_4(string w)
            {
                return !string.IsNullOrEmpty(w);
            }

            internal bool _Lambda___31_5(string s)
            {
                return s.Lower().Contains("forge") || s.Lower().Contains("fabric") || s.Lower().Contains("quilt");
            }

            internal string _Lambda___33_0(string Data)
            {
                StringBuilder stringBuilder = new StringBuilder();
                try
                {
                    foreach (char c in Data.Where<char>((ResourceProject._Closure___._I33_1 == null) ? (ResourceProject._Closure___._I33_1 = new Func<char, bool>(ResourceProject._Closure___._I._Lambda___33_1)) : ResourceProject._Closure___._I33_1))
                    {
                        stringBuilder.Append(c);
                    }
                }
                finally
                {
                    IEnumerator<char> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                return stringBuilder.ToString().Lower();
            }

            internal bool _Lambda___33_1(char c)
            {
                return char.IsLetterOrDigit(c);
            }

            public static readonly ResourceProject._Closure___ _I = new ResourceProject._Closure___();

            public static Func<JToken, int> _I25_0;

            public static Func<JToken, string> _I25_1;

            public static Func<JToken, int> _I25_2;

            public static Func<string, bool> _I25_3;

            public static Func<KeyValuePair<int, List<string>>, int> _I25_4;

            public static Func<KeyValuePair<int, List<string>>, IEnumerable<string>> _I25_5;

            public static Func<string, int> _I25_6;

            public static Func<int, bool> _I25_7;

            public static Func<int, int> _I25_8;

            public static Func<JToken, int> _I25_9;

            public static Func<int, int> _I25_10;

            public static Func<JToken, string> _I25_11;

            public static Func<string, int> _I25_12;

            public static Func<int, bool> _I25_13;

            public static Func<int, int> _I25_14;

            public static Func<JToken, string> _I25_15;

            public static Func<JToken, string> _I25_16;

            public static Func<JToken, string> _I25_17;

            public static Func<string, string> _I31_0;

            public static Func<string, bool> _I31_1;

            public static Func<string, string> _I31_3;

            public static Func<string, bool> _I31_4;

            public static Func<string, bool> _I31_5;

            public static Func<char, bool> _I33_1;

            public static VB_AnonymousDelegate_1<string, string> _I33_0;
        }

        [CompilerGenerated]
        private sealed class _Closure___29_0
        {
            public _Closure___29_0(ResourceProject._Closure___29_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_ShowMcVersionDesc = arg0._VB_Local_ShowMcVersionDesc;
                    this._VB_Local_ShowLoaderDesc = arg0._VB_Local_ShowLoaderDesc;
                    this._VB_Local_ModLoaderDescriptionPart = arg0._VB_Local_ModLoaderDescriptionPart;
                    this._VB_Local_GameVersionDescription = arg0._VB_Local_GameVersionDescription;
                    this._VB_Local_ModLoaderDescriptionFull = arg0._VB_Local_ModLoaderDescriptionFull;
                }
            }

            internal OurResourceItem _Lambda___0()
            {
                OurResourceItem ourResourceItem = new OurResourceItem
                {
                    Tag = this._VB_Me
                };
                this._VB_Me.ApplyLogoToOurImage(ourResourceItem.PathLogo);
                ValueTuple<string, string> controlTitle = this._VB_Me.GetControlTitle(true);
                ourResourceItem.Title = controlTitle.Item1;
                if (Operators.CompareString(controlTitle.Item2, "", false) == 0)
                {
                    ((StackPanel)ourResourceItem.LabTitleRaw.Parent).Children.Remove(ourResourceItem.LabTitleRaw);
                }
                else
                {
                    ourResourceItem.SubTitle = controlTitle.Item2;
                }
                ourResourceItem.Tags = this._VB_Me.Tags;
                ourResourceItem.Description = this._VB_Me.Description.ReplaceLineEndings("", false);
                if (!this._VB_Local_ShowMcVersionDesc && !this._VB_Local_ShowLoaderDesc)
                {
                    ((Grid)ourResourceItem.PathVersion.Parent).Children.Remove(ourResourceItem.PathVersion);
                    ((Grid)ourResourceItem.LabVersion.Parent).Children.Remove(ourResourceItem.LabVersion);
                    ourResourceItem.ColumnVersion1.Width = new GridLength(0.0);
                    ourResourceItem.ColumnVersion2.MaxWidth = 0.0;
                    ourResourceItem.ColumnVersion3.Width = new GridLength(0.0);
                }
                else if (this._VB_Local_ShowMcVersionDesc && this._VB_Local_ShowLoaderDesc)
                {
                    ourResourceItem.LabVersion.Text = ((Operators.CompareString(this._VB_Local_ModLoaderDescriptionPart, "", false) == 0) ? "" : (this._VB_Local_ModLoaderDescriptionPart + " ")) + this._VB_Local_GameVersionDescription;
                }
                else if (this._VB_Local_ShowMcVersionDesc)
                {
                    ourResourceItem.LabVersion.Text = this._VB_Local_GameVersionDescription;
                }
                else
                {
                    ourResourceItem.LabVersion.Text = this._VB_Local_ModLoaderDescriptionFull;
                }
                ourResourceItem.LabSource.Text = this._VB_Me.Platform.ToString();
                ourResourceItem.LabTime.Text = StringUtils.FormatTimeSpan(this._VB_Me.LastUpdate - DateTime.Now, true);
                ourResourceItem.LabDownload.Text = Conversions.ToString((this._VB_Me.DownloadCount > 100000000) ? (Conversions.ToString(Math.Round((double)this._VB_Me.DownloadCount / 100000000.0, 2)) + " 亿") : ((this._VB_Me.DownloadCount > 100000) ? (Conversions.ToString(Math.Floor((double)this._VB_Me.DownloadCount / 10000.0)) + " 万") : this._VB_Me.DownloadCount));
                return ourResourceItem;
            }

            public bool _VB_Local_ShowMcVersionDesc;

            public bool _VB_Local_ShowLoaderDesc;

            public string _VB_Local_ModLoaderDescriptionPart;

            public string _VB_Local_GameVersionDescription;

            public string _VB_Local_ModLoaderDescriptionFull;

            public ResourceProject _VB_Me;
        }

        [CompilerGenerated]
        private sealed class _Closure___31_0
        {
            public _Closure___31_0(ResourceProject._Closure___31_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_LowerName = arg0._VB_Local_LowerName;
                }
            }

            internal bool _Lambda___2(string l)
            {
                return this._VB_Local_LowerName.Contains(l);
            }

            public string _VB_Local_LowerName;
        }
    }
}
