using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MeloongCore;
using MeloongCore.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PCLCS
{
    [NullableContext(1)]
    [Nullable(0)]
    public sealed class McVersion
    {
        public string VanillaName
        {
            get
            {
                return this._vanillaName;
            }
            set
            {
                this._vanillaName = value ?? "Unknown";
                this.Vanilla = McVersion.NameToVersion(this._vanillaName);
            }
        }

        public bool Valid
        {
            get
            {
                return this.Vanilla.Major < 1000;
            }
        }

        public int Drop
        {
            get
            {
                if (!this.Valid)
                {
                    return 209;
                }
                return this.Vanilla.Major * 10 + this.Vanilla.Minor;
            }
        }

        public ModLoaders ModLoaders
        {
            get
            {
                return this.ModLoaderVersions.Keys.Aggregate(ModLoaders.None, (ModLoaders result, ModLoaders loader) => result | loader);
            }
        }

        public Dictionary<ModLoaders, string> ModLoaderVersions { get; } = new Dictionary<ModLoaders, string>();

        public bool HasOptiFine
        {
            get
            {
                return this.OptiFine != null;
            }
        }

        public int OptiFineCode
        {
            get
            {
                string optiFine = this.OptiFine;
                bool flag = optiFine == null || optiFine == "未知版本";
                if (flag || this.OptiFine.Length == 0)
                {
                    return 0;
                }
                int num = (int)(char.ToUpperInvariant(this.OptiFine[0]) - 'A' + '\u0001');
                int num2;
                num = num * 100 + (int.TryParse(this.OptiFine.Substring(1).RegexSeek("\\d+", RegexOptions.None), out num2) ? num2 : 0);
                num *= 100;
                string text = this.OptiFine.RegexSeek("(?i)(pre|beta)\\d*", RegexOptions.None);
                if (text == null)
                {
                    return num + 99;
                }
                int num4;
                int num3 = (int.TryParse(text.RegexSeek("\\d+", RegexOptions.None), out num4) ? num4 : 1);
                return num + (text.StartsWithF("pre", true) ? 50 : 0) + num3;
            }
        }

        public int ForgelikeCode
        {
            get
            {
                string text;
                if (!this.ModLoaderVersions.TryGetValue(ModLoaders.Forge, out text))
                {
                    this.ModLoaderVersions.TryGetValue(ModLoaders.NeoForge, out text);
                }
                bool flag = text == null || text == "未知版本";
                if (flag)
                {
                    return 0;
                }
                int[] array = text.RegexSearch("\\d+", RegexOptions.None).Select<string, int>(delegate(string segment)
                {
                    int num3;
                    if (!int.TryParse(segment, out num3))
                    {
                        return 0;
                    }
                    return num3;
                }).ToArray<int>();
                int num = array.Length;
                int num2;
                if (num < 4)
                {
                    switch (num)
                    {
                    case 1:
                        num2 = array[0] * 1000000;
                        break;
                    case 2:
                        num2 = array[0] * 1000000 + array[1] * 10000;
                        break;
                    case 3:
                        num2 = array[0] * 1000000 + array[1] * 10000 + array[2];
                        break;
                    default:
                        num2 = 0;
                        break;
                    }
                }
                else
                {
                    num2 = array[0] * 1000000 + array[1] * 10000 + array[3];
                }
                return num2;
            }
        }

        public McVersion(string vanillaName, bool reliable = true)
        {
            this.Reliable = reliable;
            this.VanillaName = vanillaName;
            this.IsFool = McVersion.IsFoolName(this.VanillaName);
            this.IsSnapshot = McVersion.IsSnapshotName(this.VanillaName);
        }

        public McVersion(McComponents components)
            : this(components.MinecraftVersion, true)
        {
            string optiFineVersion = components.OptiFineVersion;
            this.OptiFine = ((optiFineVersion != null) ? optiFineVersion.AfterLast("HD_", false).AfterFirst("_", false) : null);
            foreach (KeyValuePair<ModLoaders, string> keyValuePair in components.ModLoaderVersions)
            {
                this.ModLoaderVersions[keyValuePair.Key] = keyValuePair.Value;
            }
        }

        public McVersion(string instanceName, JObject json, [Nullable(2)] JObject jarVersionJson = null)
        {
            string text = json.ToString(0, Array.Empty<JsonConverter>());
            DateTime dateTime = new DateTime(1970, 1, 1, 15, 0, 0);
            try
            {
                JToken jtoken = json["releaseTime"];
                if (jtoken != null)
                {
                    dateTime = jtoken.ToObject<DateTime>();
                }
            }
            catch
            {
            }
            bool flag = true;
            string text2 = (McVersion.GetVanillaName(instanceName, json, jarVersionJson, dateTime, ref flag) ?? "Unknown").Replace("_unobfuscated", "").Replace(" Unobfuscated", "");
            this.Reliable = flag;
            this.VanillaName = text2;
            this.ReleaseTime = dateTime;
            string text3 = json.Value<string>("type") ?? "";
            this.OptiFine = (text.ContainsIgnoreCase("optifine") ? (text.RegexSeek("(?<=HD_U_)[^\\\":/]+", RegexOptions.None) ?? "未知版本") : null);
            DateTime dateTime2 = dateTime.ToUniversalTime().AddHours(2.0);
            this.IsFool = text3 == "fool" || McVersion.IsFoolName(text2) || (dateTime2.Month == 4 && dateTime2.Day == 1 && text3 == "snapshot");
            bool flag2 = McVersion.IsSnapshotName(text2) || instanceName.ContainsIgnoreCase("combat");
            if (!flag2)
            {
                bool flag3 = text3 == "snapshot" || text3 == "pending" || text3 == "fool";
                flag2 = flag3;
            }
            this.IsSnapshot = flag2;
            if (!this.Reliable)
            {
                Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(17, 1, out flag2);
                if (flag2)
                {
                    logInterpolatedStringHandler.AppendLiteral("无法完全确认 MC 版本号的版本：");
                    logInterpolatedStringHandler.AppendFormatted(this.VanillaName);
                }
                Logger.Info(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\PCLCS\\Minecraft\\McVersion.cs");
            }
            if (text.ContainsIgnoreCase("liteloader"))
            {
                this.ModLoaderVersions[ModLoaders.LiteLoader] = text.RegexSeek("(?<=com\\.mumfrey:liteloader:)[^\\\":/]+", RegexOptions.None) ?? "未知版本";
            }
            if (text.ContainsIgnoreCase("net.fabricmc:fabric-loader") || text.ContainsIgnoreCase("org.quiltmc:quilt-loader"))
            {
                Dictionary<ModLoaders, string> modLoaderVersions = this.ModLoaderVersions;
                ModLoaders modLoaders = ModLoaders.Fabric;
                string text4 = text.RegexSeek("(?<=(net\\.fabricmc:fabric-loader:|org\\.quiltmc:quilt-loader:))[0-9\\.]+(\\+build.[0-9]+)?", RegexOptions.None);
                modLoaderVersions[modLoaders] = ((text4 != null) ? text4.Replace("+build", "") : null) ?? "未知版本";
                return;
            }
            if (text.ContainsIgnoreCase("net.neoforge"))
            {
                Dictionary<ModLoaders, string> modLoaderVersions2 = this.ModLoaderVersions;
                ModLoaders modLoaders2 = ModLoaders.NeoForge;
                string text5;
                if ((text5 = McVersion.FindArgumentValue(json, "--fml.neoForgeVersion")) == null)
                {
                    text5 = McVersion.FindArgumentValue(json, "--fml.forgeVersion") ?? "未知版本";
                }
                modLoaderVersions2[modLoaders2] = text5;
                return;
            }
            if (text.ContainsIgnoreCase("minecraftforge"))
            {
                Dictionary<ModLoaders, string> modLoaderVersions3 = this.ModLoaderVersions;
                ModLoaders modLoaders3 = ModLoaders.Forge;
                string text6;
                if ((text6 = text.RegexSeek("(?<=forge:[0-9\\.]+(?:_pre[0-9]*)?-)[0-9\\.]+", RegexOptions.None)) == null && (text6 = text.RegexSeek("(?<=net\\.minecraftforge:minecraftforge:)[0-9\\.]+", RegexOptions.None)) == null)
                {
                    text6 = text.RegexSeek("(?<=net\\.minecraftforge:fmlloader:[0-9\\.]+-)[0-9\\.]+", RegexOptions.None) ?? "未知版本";
                }
                modLoaderVersions3[modLoaders3] = text6;
            }
        }

        [return: Nullable(2)]
        private static string GetVanillaName(string instanceName, JObject json, [Nullable(2)] JObject jarVersionJson, DateTime releaseTime, ref bool reliable)
        {
            int i = releaseTime.Year;
            if (i > 2000 && i < 2013)
            {
                return "Old";
            }
            if (json.Value<string>("type") == "pending")
            {
                return "pending";
            }
            JToken jtoken = json["clientVersion"];
            if (jtoken != null)
            {
                return jtoken.ToString();
            }
            JArray jarray = json["patches"] as JArray;
            if (jarray != null)
            {
                JObject jobject = jarray.OfType<JObject>().FirstOrDefault<JObject>((JObject patch) => patch.Value<string>("id") == "game");
                JToken jtoken2 = ((jobject != null) ? jobject["version"] : null);
                if (jtoken2 != null)
                {
                    return jtoken2.ToString();
                }
            }
            string text = McVersion.FindArgumentValue(json, "--fml.mcVersion");
            if (text != null && text.Length > 0)
            {
                return text;
            }
            JToken jtoken3 = json["inheritsFrom"];
            string text2 = ((jtoken3 != null) ? jtoken3.ToString() : null) ?? "";
            if (text2.Length > 0)
            {
                JToken jtoken4 = json["jar"];
                string text3 = ((jtoken4 != null) ? jtoken4.ToString() : null) ?? "";
                if (text3.Length <= 0)
                {
                    return text2;
                }
                return text3;
            }
            else
            {
                JToken jtoken5 = json["downloads"];
                string text4 = ((jtoken5 != null) ? jtoken5.ToString(0, Array.Empty<JsonConverter>()).RegexSeek("(?<=launcher.mojang.com/mc/game/)[^/]*", RegexOptions.None) : null);
                if (text4 != null)
                {
                    return text4;
                }
                string text5 = json["libraries"].ToString();
                foreach (string text6 in new string[] { "(?<=net.minecraftforge:forge:)[0-9]{1,2}.[0-9+.]+", "(?<=net.minecraftforge:fmlloader:)[0-9]{1,2}.[0-9+.]+", "(?<=optifine:OptiFine:)[0-9]{1,2}.[0-9+.]+", "(?<=((fabricmc)|(quiltmc)):intermediary:)[^\\\"]*" })
                {
                    string text7 = text5.RegexSeek(text6, RegexOptions.None);
                    if (text7 != null)
                    {
                        return text7;
                    }
                }
                JToken jtoken6 = json["jar"];
                if (jtoken6 != null)
                {
                    return jtoken6.ToString();
                }
                JToken jtoken7 = ((jarVersionJson != null) ? jarVersionJson["name"] : null);
                bool flag;
                if (jtoken7 != null)
                {
                    string text8 = jtoken7.ToString();
                    if (text8 != null && text8.Length < 32)
                    {
                        Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(31, 1, out flag);
                        if (flag)
                        {
                            logInterpolatedStringHandler.AppendLiteral("从版本 jar 中的 version.json 获取到版本号：");
                            logInterpolatedStringHandler.AppendFormatted(text8);
                        }
                        Logger.Info(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\PCLCS\\Minecraft\\McVersion.cs");
                        return text8;
                    }
                }
                string text9 = "(([1-9][0-9]w[0-9]{2}[a-g])|((1|[2-9][0-9])\\.[0-9]+(\\.[0-9]+)?(-(pre|rc|snapshot-?)[1-9]*| Pre-Release( [1-9])?)?))(_unobfuscated)?";
                string text10 = json["id"].ToString().RegexSeek(text9, RegexOptions.IgnoreCase);
                if (text10 != null)
                {
                    return text10;
                }
                Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler2 = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(17, 1, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler2.AppendLiteral("无法完全确认 MC 版本号的版本：");
                    logInterpolatedStringHandler2.AppendFormatted(instanceName);
                }
                Logger.Info(ref logInterpolatedStringHandler2, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\PCLCS\\Minecraft\\McVersion.cs");
                reliable = false;
                string text11 = instanceName.RegexSeek(text9, RegexOptions.IgnoreCase);
                if (text11 != null)
                {
                    return text11;
                }
                JObject jobject2 = (JObject)json.DeepClone();
                jobject2.Remove("libraries");
                return jobject2.ToString(0, Array.Empty<JsonConverter>()).RegexSeek(text9, RegexOptions.IgnoreCase);
            }
        }

        private static bool IsFoolName(string name)
        {
            name = name.Lower();
            bool flag = name.StartsWithF("2.0", false);
            if (!flag)
            {
                bool flag2;
                if (name != null)
                {
                    int length = name.Length;
                    if (length <= 11)
                    {
                        if (length != 6)
                        {
                            if (length != 9)
                            {
                                if (length != 11)
                                {
                                    goto IL_0119;
                                }
                                char c = name[1];
                                if (c != '3')
                                {
                                    if (c != '4')
                                    {
                                        goto IL_0119;
                                    }
                                    if (!(name == "24w14potato"))
                                    {
                                        goto IL_0119;
                                    }
                                }
                                else if (!(name == "23w13a_or_b"))
                                {
                                    goto IL_0119;
                                }
                            }
                            else if (!(name == "1.rv-pre1"))
                            {
                                goto IL_0119;
                            }
                        }
                        else
                        {
                            char c = name[1];
                            if (c != '0')
                            {
                                if (c != '5')
                                {
                                    if (c != '6')
                                    {
                                        goto IL_0119;
                                    }
                                    if (!(name == "26w14a"))
                                    {
                                        goto IL_0119;
                                    }
                                }
                                else if (!(name == "15w14a"))
                                {
                                    goto IL_0119;
                                }
                            }
                            else if (!(name == "20w14∞"))
                            {
                                goto IL_0119;
                            }
                        }
                    }
                    else if (length != 14)
                    {
                        if (length != 18)
                        {
                            if (length != 20)
                            {
                                goto IL_0119;
                            }
                            if (!(name == "22w13oneblockatatime"))
                            {
                                goto IL_0119;
                            }
                        }
                        else if (!(name == "3d shareware v1.34"))
                        {
                            goto IL_0119;
                        }
                    }
                    else if (!(name == "25w14craftmine"))
                    {
                        goto IL_0119;
                    }
                    flag2 = true;
                    goto IL_011B;
                }
                IL_0119:
                flag2 = false;
                IL_011B:
                flag = flag2;
            }
            return flag || name.StartsWithF("20w14inf", false);
        }

        private static bool IsSnapshotName(string name)
        {
            return new string[] { "w", "snapshot", "rc", "pre", "experimental", "-" }.Any<string>(new Func<string, bool>(name.ContainsIgnoreCase));
        }

        [return: Nullable(2)]
        private static string FindArgumentValue(JObject json, string name)
        {
            JToken jtoken = json["arguments"];
            JArray jarray = ((jtoken != null) ? jtoken["game"] : null) as JArray;
            if (jarray == null)
            {
                return null;
            }
            int num = 0;
            while (num + 1 < jarray.Count)
            {
                JToken jtoken2 = jarray[num];
                if (((jtoken2 != null) ? jtoken2.ToString() : null) == name)
                {
                    JToken jtoken3 = jarray[num + 1];
                    if (jtoken3 == null)
                    {
                        return null;
                    }
                    return jtoken3.ToString();
                }
                else
                {
                    num++;
                }
            }
            return null;
        }

        [NullableContext(2)]
        public static bool IsStandardFormat(string version)
        {
            double num;
            return version != null && (Regex.IsMatch(version, "^1\\.\\d") || (double.TryParse(version.RegexSeek("^[2-9]\\d\\.\\d+", RegexOptions.None), NumberStyles.Float, CultureInfo.InvariantCulture, out num) && num >= 26.0));
        }

        [NullableContext(2)]
        public static int VersionToDrop(string vanillaName)
        {
            if (vanillaName == null)
            {
                return 209;
            }
            Version version = McVersion.NameToVersion(vanillaName);
            if (version.Major < 1000)
            {
                return version.Major * 10 + version.Minor;
            }
            return 209;
        }

        public static string DropToVersion(int drop)
        {
            if (drop >= 250)
            {
                return string.Format("{0}.{1}", drop / 10, drop % 10);
            }
            if (drop != 209)
            {
                return string.Format("1.{0}", drop / 10);
            }
            Logger.Warn(new Exception("尝试将旧快照版 Drop 序数转换为版本字符串，这不应该发生！"), null, LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\PCLCS\\Minecraft\\McVersion.cs");
            return "1.20";
        }

        public static Version NameToVersion([Nullable(2)] string name)
        {
            if (name == null)
            {
                return new Version(9999, 0, 0);
            }
            name = name.Lower().Replace("_unobfuscated", "").Replace(" unobfuscated", "");
            if (name.StartsWithF("2.0", false))
            {
                return McVersion.NameToVersion("1.5.1");
            }
            if (name == "15w14a")
            {
                return McVersion.NameToVersion("1.8.3");
            }
            if (name.ContainsIgnoreCase(".rv-pre"))
            {
                return McVersion.NameToVersion("1.9.2");
            }
            if (name.ContainsIgnoreCase("shareware"))
            {
                return McVersion.NameToVersion("1.13.2");
            }
            if (name.StartsWithF("20w14", false) && name != "20w14a")
            {
                return McVersion.NameToVersion("1.15.2");
            }
            if (name.ContainsIgnoreCase("oneblockatatime"))
            {
                return McVersion.NameToVersion("1.18.2");
            }
            if (name.ContainsIgnoreCase("23w13a") && name != "20w13a")
            {
                return McVersion.NameToVersion("1.19.4");
            }
            if (name == "24w14potato")
            {
                return McVersion.NameToVersion("1.20.4");
            }
            if (name == "25w14craftmine")
            {
                return McVersion.NameToVersion("1.21.4");
            }
            if (name == "26w14a")
            {
                return McVersion.NameToVersion("26.1.1");
            }
            string[] array = name.Split(new char[] { ' ', '_', '-', '.' }, StringSplitOptions.None);
            if (name.StartsWithF("1.", false))
            {
                return new Version(McVersion.<NameToVersion>g__ParseSegment|36_0(array, 1), 0, McVersion.<NameToVersion>g__ParseSegment|36_0(array, 2));
            }
            if (name.RegexCheck("^[2-9][0-9]\\.", RegexOptions.None))
            {
                return new Version(McVersion.<NameToVersion>g__ParseSegment|36_0(array, 0), McVersion.<NameToVersion>g__ParseSegment|36_0(array, 1), McVersion.<NameToVersion>g__ParseSegment|36_0(array, 2));
            }
            return new Version(9999, 0, 0);
        }

        [CompilerGenerated]
        internal static int <NameToVersion>g__ParseSegment|36_0(string[] segments, int index)
        {
            if (index >= segments.Length)
            {
                return 0;
            }
            int num;
            if (!int.TryParse(segments[index].RegexSeek("^\\d+", RegexOptions.None), out num))
            {
                return 0;
            }
            return num;
        }

        private string _vanillaName = "Unknown";

        public Version Vanilla = new Version(9999, 0, 0);

        public bool Reliable;

        [Nullable(2)]
        public string OptiFine;

        public bool OptiFineAsMod;

        public DateTime ReleaseTime = new DateTime(1970, 1, 1, 15, 0, 0);

        public bool IsFool;

        public bool IsSnapshot;
    }
}
