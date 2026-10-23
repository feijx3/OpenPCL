using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using MeloongCore;
using MeloongCore.Extensions;
using MeloongCore.Wpf;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;
using PCLCS;

namespace PCL
{
    [StandardModule]
    public sealed class ModModpack
    {
        public static void ModpackInstall()
        {
            ModMain.Hint("为遵守相关法律法规，PCL 暂时不再提供 MC 下载功能，非常抱歉 qwq……", ModMain.HintType.Red, true);
        }

        public static ModLoader.LoaderCombo<string> ModpackInstall(string File, string InstanceName = null, string Logo = null)
        {
            Logger.Info(string.Format("整合包安装请求：{0}", File ?? "null"), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
            ZipArchive zipArchive = null;
            try
            {
                string text = string.Format("{0}versions\\{1}\\", ModMinecraft.McFolderSelected, InstanceName);
                if (text.Contains("!") || text.Contains(";"))
                {
                    ModMain.Hint("游戏路径中不能含有感叹号或分号：" + text, ModMain.HintType.Red, true);
                    throw new OperationCanceledException();
                }
                try
                {
                    zipArchive = FileUtils.OpenZip(File, default(CancellationToken));
                    if (zipArchive.GetEntry("mcbbs.packmeta") == null)
                    {
                        if (zipArchive.GetEntry("mmc-pack.json") == null)
                        {
                            if (zipArchive.GetEntry("modrinth.index.json") == null)
                            {
                                if (zipArchive.GetEntry("manifest.json") != null)
                                {
                                    if (((JObject)zipArchive.GetEntry("manifest.json").Open().ReadString(Encoding.UTF8)
                                        .DeserializeJson())["addons"] == null)
                                    {
                                    }
                                }
                                else if (zipArchive.GetEntry("modpack.json") == null)
                                {
                                    if (zipArchive.GetEntry("modpack.zip") == null && zipArchive.GetEntry("modpack.mrpack") == null)
                                    {
                                        try
                                        {
                                            foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries)
                                            {
                                                string[] array = zipArchiveEntry.FullName.Split("/", false);
                                                string text2 = array[0] + "/";
                                                if (array.Count<string>() == 2)
                                                {
                                                    if (Operators.CompareString(array[1], "mcbbs.packmeta", false) == 0)
                                                    {
                                                        break;
                                                    }
                                                    if (Operators.CompareString(array[1], "mmc-pack.json", false) == 0)
                                                    {
                                                        break;
                                                    }
                                                    if (Operators.CompareString(array[1], "modrinth.index.json", false) == 0)
                                                    {
                                                        break;
                                                    }
                                                    if (Operators.CompareString(array[1], "manifest.json", false) == 0)
                                                    {
                                                        if (((JObject)zipArchiveEntry.Open().ReadString(Encoding.UTF8).DeserializeJson())["addons"] == null)
                                                        {
                                                            break;
                                                        }
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        if (Operators.CompareString(array[1], "modpack.json", false) == 0)
                                                        {
                                                            break;
                                                        }
                                                        if (Operators.CompareString(array[1], "modpack.zip", false) == 0 || Operators.CompareString(array[1], "modpack.mrpack", false) == 0)
                                                        {
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            IEnumerator<ZipArchiveEntry> enumerator;
                                            if (enumerator != null)
                                            {
                                                enumerator.Dispose();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetDisplay(true).Contains("Error.WinIOError"))
                    {
                        throw new Exception("打开整合包文件失败", ex);
                    }
                    if (Operators.CompareString(PathUtils.GetExtension(File), "rar", false) == 0)
                    {
                        throw new Exception("PCL 无法处理 rar 格式的压缩包，请在解压后重新压缩为 zip 格式再试", ex);
                    }
                    throw new Exception("打开整合包文件失败，文件可能损坏或为不支持的压缩包格式", ex);
                }
                throw new Exception("为遵守相关法律法规，PCL 暂时不再提供 MC 下载功能，非常抱歉 qwq……");
            }
            finally
            {
                if (zipArchive != null)
                {
                    zipArchive.Dispose();
                    goto IL_02EE;
                }
                goto IL_02EE;
                IL_02EE:;
            }
        }

        private static void ExtractModpackFiles(string InstallTemp, string FileAddress, CancellationToken c = default(CancellationToken), ProgressProvider p = null)
        {
            DirectoryUtils.Delete(InstallTemp, false);
            c.ThrowIfCancellationRequested();
            FileUtils.ExtractToDirectory(FileAddress, InstallTemp, c, p);
        }

        private static void CopyOverrideDirectory(string OverridesFolder, string VersionFolder, ModLoader.LoaderBase Loader)
        {
            OverridesFolder = PathUtils.AddSlashSuffix(OverridesFolder);
            VersionFolder = PathUtils.AddSlashSuffix(VersionFolder);
            if (DirectoryUtils.Exists(OverridesFolder))
            {
                Logger.Info(string.Format("处理整合包覆写文件夹：{0} → {1}", OverridesFolder, VersionFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                DirectoryUtils.Copy(OverridesFolder, VersionFolder);
            }
            else
            {
                Logger.Info(string.Format("整合包中没有覆写文件夹：{0}", OverridesFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
            }
            string text = string.Format("{0}PCL\\Setup.ini", OverridesFolder);
            string text2 = string.Format("{0}PCL\\Setup.ini", VersionFolder);
            if (FileUtils.Exists(text))
            {
                ModBase.WriteIni(text, "VersionArgumentIndie", Conversions.ToString(1));
                ModBase.WriteIni(text, "VersionArgumentIndieV2", Conversions.ToString(true));
                ModBase.WriteIni(text, "IsStar", Conversions.ToString(false));
                FileUtils.Copy(text, text2);
            }
            else
            {
                ModBase.WriteIni(text2, "VersionArgumentIndie", Conversions.ToString(1));
                ModBase.WriteIni(text2, "VersionArgumentIndieV2", Conversions.ToString(true));
            }
            ModBase.IniClearCache(text2);
        }

        private static void NotifyIncompatibleLoader(string LoaderName)
        {
            if (ModMain.MyMsgBox(string.Format("整合包中存在不兼容的加载器：{0}{1}如果你知道如何手动安装它，可以先选择跳过，然后在整合包下载结束后手动安装。", LoaderName, "\r\n"), "不兼容的加载器", "取消", string.Format("不安装 {0} 并继续", LoaderName), "", false, true, false, null, null, null) == 1)
            {
                throw new OperationCanceledException();
            }
        }

        private static ModLoader.LoaderCombo<string> InstallPackCurseForge(string FileAddress, ZipArchive Archive, string ArchiveBaseFolder, string InstanceName = null, string Logo = null)
        {
            ModModpack._Closure$__5-0 CS$<>8__locals1 = new ModModpack._Closure$__5-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_FileAddress = FileAddress;
            CS$<>8__locals1.$VB$Local_Logo = Logo;
            try
            {
                CS$<>8__locals1.$VB$Local_Json = (JObject)Archive.GetEntry(ArchiveBaseFolder + "manifest.json").Open().ReadString(null)
                    .DeserializeJson();
            }
            catch (Exception ex)
            {
                throw new Exception("CurseForge 整合包安装信息存在问题", ex);
            }
            if (CS$<>8__locals1.$VB$Local_Json["minecraft"] == null || CS$<>8__locals1.$VB$Local_Json["minecraft"]["version"] == null)
            {
                throw new Exception("CurseForge 整合包未提供 Minecraft 版本信息");
            }
            if (InstanceName == null)
            {
                InstanceName = (string)(CS$<>8__locals1.$VB$Local_Json["name"] ?? "");
                ValidateFolderName validateFolderName = new ValidateFolderName(ModMinecraft.McFolderSelected + "versions", true, null);
                if (Operators.CompareString(validateFolderName.Validate(InstanceName), "", false) != 0)
                {
                    InstanceName = "";
                }
                if (Operators.CompareString(InstanceName, "", false) == 0)
                {
                    InstanceName = ModMain.MyMsgBoxInput("输入版本名称", "", "", new Collection<Validate> { validateFolderName }, "", "确定", "取消", false);
                }
                if (string.IsNullOrEmpty(InstanceName))
                {
                    throw new OperationCanceledException();
                }
            }
            CS$<>8__locals1.$VB$Local_VersionFolder = PathUtils.AddSlashSuffix(Path.Combine(ModMinecraft.McFolderSelected, "versions", InstanceName) + "\\");
            CS$<>8__locals1.$VB$Local_TargetVersion = new McVersion(CS$<>8__locals1.$VB$Local_Json["minecraft"]["version"].ToString(), true);
            try
            {
                foreach (JToken jtoken in (CS$<>8__locals1.$VB$Local_Json["minecraft"]["modLoaders"] ?? new byte[0]))
                {
                    string text = (jtoken["id"] ?? "").ToString().Lower();
                    if (text.StartsWithF("forge-", false))
                    {
                        if (text.Contains("recommended"))
                        {
                            throw new Exception("该整合包版本过老，已不支持进行安装！");
                        }
                        Logger.Info(string.Format("整合包 Forge 版本：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        CS$<>8__locals1.$VB$Local_TargetVersion.ModLoaderVersions[ModLoaders.Forge] = text.Replace("forge-", "");
                    }
                    else if (text.StartsWithF("neoforge-", false))
                    {
                        Logger.Info(string.Format("整合包 NeoForge 版本：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        CS$<>8__locals1.$VB$Local_TargetVersion.ModLoaderVersions[ModLoaders.NeoForge] = text.Replace("neoforge-", "");
                    }
                    else if (text.StartsWithF("fabric-", false))
                    {
                        Logger.Info(string.Format("整合包 Fabric 版本：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        CS$<>8__locals1.$VB$Local_TargetVersion.ModLoaderVersions[ModLoaders.Fabric] = text.Replace("fabric-", "");
                    }
                    else
                    {
                        ModModpack.NotifyIncompatibleLoader(text);
                    }
                }
            }
            finally
            {
                IEnumerator<JToken> enumerator;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            CS$<>8__locals1.$VB$Local_InstallTemp = ModMain.RequestTaskTempFolder(false);
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            string text2 = (string)(CS$<>8__locals1.$VB$Local_Json["overrides"] ?? "");
            if (Operators.CompareString(text2, "", false) != 0)
            {
                ModModpack._Closure$__5-1 CS$<>8__locals2 = new ModModpack._Closure$__5-1(CS$<>8__locals2);
                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1;
                CS$<>8__locals2.$VB$Local_OverridesFolder = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_InstallTemp + ArchiveBaseFolder + ((Operators.CompareString(text2, ".", false) == 0 || Operators.CompareString(text2, "./", false) == 0) ? "" : text2);
                if (!PathUtils.IsParentOf(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_InstallTemp, CS$<>8__locals2.$VB$Local_OverridesFolder))
                {
                    throw new Exception("整合包的 overrides 路径超出了解压目录，请向整合包作者反馈此问题！");
                }
                list.Add(new ModLoader.LoaderTask<string, int>("解压整合包文件", delegate(ModLoader.LoaderTask<string, int> Task)
                {
                    ModModpack.ExtractModpackFiles(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_InstallTemp, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.6));
                    ModModpack.CopyOverrideDirectory(CS$<>8__locals2.$VB$Local_OverridesFolder, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_VersionFolder, Task);
                }, null, ThreadPriority.Normal)
                {
                    ProgressWeight = (double)FileUtils.GetInfo(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_FileAddress).Length / 1024.0 / 1024.0 / 6.0,
                    Block = false
                });
            }
            CS$<>8__locals1.$VB$Local_ModList = new List<int>();
            CS$<>8__locals1.$VB$Local_ModOptionalList = new List<int>();
            try
            {
                foreach (JToken jtoken2 in (CS$<>8__locals1.$VB$Local_Json["files"] ?? new byte[0]))
                {
                    if (jtoken2["projectID"] == null || jtoken2["fileID"] == null)
                    {
                        ModMain.Hint("某项 Mod 缺少必要信息，已跳过：" + jtoken2.ToString(), ModMain.HintType.Blue, true);
                    }
                    else
                    {
                        CS$<>8__locals1.$VB$Local_ModList.Add((int)jtoken2["fileID"]);
                        if (jtoken2["required"] != null && !jtoken2["required"].ToObject<bool>())
                        {
                            CS$<>8__locals1.$VB$Local_ModOptionalList.Add((int)jtoken2["fileID"]);
                        }
                    }
                }
            }
            finally
            {
                IEnumerator<JToken> enumerator2;
                if (enumerator2 != null)
                {
                    enumerator2.Dispose();
                }
            }
            if (CS$<>8__locals1.$VB$Local_ModList.Any<int>())
            {
                List<ModLoader.LoaderBase> list2 = new List<ModLoader.LoaderBase>();
                list2.Add(new ModLoader.LoaderTask<int, JArray>("获取 Mod 下载信息", delegate(ModLoader.LoaderTask<int, JArray> Task)
                {
                    Task.Output = (JArray)ModDownload.DlModRequest("https://api.curseforge.com/v1/mods/files", HttpMethod.Post, "{\"fileIds\": [" + CS$<>8__locals1.$VB$Local_ModList.Join(",") + "]}", "application/json")["data"];
                    if (CS$<>8__locals1.$VB$Local_ModList.Count != Task.Output.Count)
                    {
                        List<int> list5 = CS$<>8__locals1.$VB$Local_ModList.Except<int>(Task.Output.Select<JToken, int>((ModModpack._Closure$__.$I5-2 == null) ? (ModModpack._Closure$__.$I5-2 = (JToken j) => j["id"].ToObject<int>()) : ModModpack._Closure$__.$I5-2)).ToList<int>();
                        Logger.Info(string.Format("缺失的 fileIds：{0}", list5.Join("、")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        List<ValueTuple<int, int>> list6 = list5.Select<int, ValueTuple<int, int>>((CS$<>8__locals1.$I3 == null) ? (CS$<>8__locals1.$I3 = delegate(int id)
                        {
                            ModModpack._Closure$__5-2 CS$<>8__locals4 = new ModModpack._Closure$__5-2(CS$<>8__locals4);
                            CS$<>8__locals4.$VB$Local_id = id;
                            return new ValueTuple<int, int>(((JObject)CS$<>8__locals1.$VB$Local_Json["files"].AsEnumerable<JToken>().First<JToken>((JToken f) => f["fileID"].ToObject<int>() == CS$<>8__locals4.$VB$Local_id))["projectID"].ToObject<int>(), CS$<>8__locals4.$VB$Local_id);
                        }) : CS$<>8__locals1.$I3).ToList<ValueTuple<int, int>>();
                        Logger.Info(string.Format("缺失项：{0}", list6.Join("、")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        List<ResourceProject> list7 = new List<ResourceProject>();
                        try
                        {
                            list7 = ResourceProject.FromProjectIds(list6.Select<ValueTuple<int, int>, string>((ModModpack._Closure$__.$I5-5 == null) ? (ModModpack._Closure$__.$I5-5 = ([TupleElementNames(new string[] { "ProjectId", "FileId" })] ValueTuple<int, int> e) => e.Item1.ToString()) : ModModpack._Closure$__.$I5-5), ResourcePlatforms.CurseForge).ToList<ResourceProject>();
                        }
                        catch (Exception ex2)
                        {
                            Logger.Warn(ex2, "获取缺失的工程信息失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        }
                        List<string> list8 = new List<string>();
                        try
                        {
                            List<ValueTuple<int, int>>.Enumerator enumerator3 = list6.GetEnumerator();
                            while (enumerator3.MoveNext())
                            {
                                ModModpack._Closure$__5-3 CS$<>8__locals3 = new ModModpack._Closure$__5-3(CS$<>8__locals3);
                                CS$<>8__locals3.$VB$Local_Missing = enumerator3.Current;
                                ResourceProject resourceProject = list7.FirstOrDefault<ResourceProject>((ResourceProject p) => Conversions.ToDouble(p.Id) == (double)CS$<>8__locals3.$VB$Local_Missing.Item1);
                                if (resourceProject == null)
                                {
                                    list8.Add(string.Format("未知 CurseForge 项目 (项目 ID {0}，文件 ID {1})", CS$<>8__locals3.$VB$Local_Missing.Item1, CS$<>8__locals3.$VB$Local_Missing.Item2));
                                }
                                else
                                {
                                    list8.Add(string.Format("{0} (文件 ID {1})", resourceProject.TranslatedName, CS$<>8__locals3.$VB$Local_Missing.Item2));
                                }
                            }
                        }
                        finally
                        {
                            List<ValueTuple<int, int>>.Enumerator enumerator3;
                            ((IDisposable)enumerator3).Dispose();
                        }
                        if (ModMain.MyMsgBox(string.Format("未找到该整合包所需的以下内容，它们可能已被原作者删除：{0}- {1}{2}{3}是否继续安装？缺失这些内容可能导致整合包出现异常！", new object[]
                        {
                            "\r\n",
                            list8.Join("\r\n- "),
                            "\r\n",
                            "\r\n"
                        }), "整合包内容缺失", "继续", "取消", "", true, true, false, null, null, null) == 2)
                        {
                            Task.Cancel();
                        }
                    }
                }, null, ThreadPriority.Normal)
                {
                    ProgressWeight = (double)CS$<>8__locals1.$VB$Local_ModList.Count / 10.0
                });
                list2.Add(new ModLoader.LoaderTask<JArray, List<ModNet.NetFile>>("构造 Mod 下载信息", delegate(ModLoader.LoaderTask<JArray, List<ModNet.NetFile>> Task)
                {
                    Dictionary<int, ModNet.NetFile> dictionary = new Dictionary<int, ModNet.NetFile>();
                    try
                    {
                        foreach (JToken jtoken3 in Task.Input)
                        {
                            int num = jtoken3["id"].ToObject<int>();
                            if (!dictionary.ContainsKey(num) && (!CS$<>8__locals1.$VB$Local_ModOptionalList.Contains(num) || ModMain.MyMsgBox("是否要下载整合包中的可选文件 " + jtoken3["displayName"].ToString() + "？", "下载可选文件", "是", "否", "", false, true, false, null, null, null) != 2))
                            {
                                string text3;
                                ResourceTypes resourceTypes;
                                if (jtoken3["modules"].Any<JToken>())
                                {
                                    List<string> list9 = ((JArray)jtoken3["modules"]).Select<JToken, string>((ModModpack._Closure$__.$I5-8 == null) ? (ModModpack._Closure$__.$I5-8 = (JToken l) => l["name"].ToString()) : ModModpack._Closure$__.$I5-8).ToList<string>();
                                    if (!list9.Contains("META-INF") && !list9.Contains("mcmod.info") && !list9.Contains("fabric.mod.json"))
                                    {
                                        bool flag;
                                        if (jtoken3 == null)
                                        {
                                            flag = false;
                                        }
                                        else
                                        {
                                            JToken jtoken4 = jtoken3["FileName"];
                                            flag = jtoken4 != null && jtoken4.ToString().EndsWithF(".jar", true);
                                        }
                                        if (!flag)
                                        {
                                            if (list9.Contains("pack.mcmeta"))
                                            {
                                                text3 = "resourcepacks";
                                                resourceTypes = ResourceTypes.ResourcePack;
                                                goto IL_018D;
                                            }
                                            if (list9.Contains("shaders"))
                                            {
                                                text3 = "shaderpacks";
                                                resourceTypes = ResourceTypes.Shader;
                                                goto IL_018D;
                                            }
                                            text3 = "saves";
                                            resourceTypes = ResourceTypes.Map;
                                            goto IL_018D;
                                        }
                                    }
                                    text3 = "mods";
                                    resourceTypes = ResourceTypes.Mod;
                                }
                                else
                                {
                                    text3 = "mods";
                                    resourceTypes = ResourceTypes.Mod;
                                }
                                IL_018D:
                                ResourceVersion resourceVersion = ResourceVersion.FromPlatformJson((JObject)jtoken3, resourceTypes);
                                if (resourceVersion.DownloadAvailable)
                                {
                                    dictionary.Add(num, resourceVersion.ToNetFile(PathUtils.AddSlashSuffix(Path.Combine(CS$<>8__locals1.$VB$Local_VersionFolder, text3)), new ResourceVersion.DownloadReason?(ResourceVersion.DownloadReason.ModPack), CS$<>8__locals1.$VB$Local_TargetVersion.VanillaName, CS$<>8__locals1.$VB$Local_TargetVersion.ModLoaders));
                                    Task.Progress += 1.0 / (double)(checked(1 + CS$<>8__locals1.$VB$Local_ModList.Count));
                                }
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
                    Task.Output = dictionary.Values.ToList<ModNet.NetFile>();
                }, null, ThreadPriority.Normal)
                {
                    ProgressWeight = (double)CS$<>8__locals1.$VB$Local_ModList.Count / 200.0,
                    Show = false
                });
                list2.Add(new ModNet.LoaderDownload("下载 Mod", new List<ModNet.NetFile>())
                {
                    ProgressWeight = (double)CS$<>8__locals1.$VB$Local_ModList.Count * 1.5
                });
                list.Add(new ModLoader.LoaderCombo<int>("下载 Mod（主加载器）", list2)
                {
                    Show = false,
                    ProgressWeight = list2.Sum<ModLoader.LoaderBase>((ModModpack._Closure$__.$I5-9 == null) ? (ModModpack._Closure$__.$I5-9 = (ModLoader.LoaderBase l) => l.ProgressWeight) : ModModpack._Closure$__.$I5-9)
                });
            }
            McComponents mcComponents = new McComponents(CS$<>8__locals1.$VB$Local_TargetVersion.VanillaName, CS$<>8__locals1.$VB$Local_TargetVersion.ModLoaderVersions, null, null, null);
            List<ModLoader.LoaderBase> list3 = ModDownloadLib.McInstallLoadersGet(new ModDownloadLib.McInstallRequest
            {
                NewInstanceName = InstanceName,
                InstanceFolder = CS$<>8__locals1.$VB$Local_VersionFolder,
                Components = mcComponents
            });
            List<ModLoader.LoaderBase> list4 = new List<ModLoader.LoaderBase>();
            list4.Add(new ModLoader.LoaderCombo<string>("整合包安装", list)
            {
                Show = false,
                Block = false,
                ProgressWeight = list.Sum<ModLoader.LoaderBase>((ModModpack._Closure$__.$I5-10 == null) ? (ModModpack._Closure$__.$I5-10 = (ModLoader.LoaderBase l) => l.ProgressWeight) : ModModpack._Closure$__.$I5-10)
            });
            list4.Add(new ModLoader.LoaderCombo<string>("游戏安装", list3)
            {
                Show = false,
                ProgressWeight = list3.Sum<ModLoader.LoaderBase>((ModModpack._Closure$__.$I5-11 == null) ? (ModModpack._Closure$__.$I5-11 = (ModLoader.LoaderBase l) => l.ProgressWeight) : ModModpack._Closure$__.$I5-11)
            });
            list4.Add(new ModLoader.LoaderTask<string, string>("最终整理文件", delegate(ModLoader.LoaderTask<string, string> Task)
            {
                if (CS$<>8__locals1.$VB$Local_Logo != null && FileUtils.Exists(CS$<>8__locals1.$VB$Local_Logo))
                {
                    FileUtils.Copy(CS$<>8__locals1.$VB$Local_Logo, CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\\Logo.png");
                    ModBase.WriteIni(CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\\Setup.ini", "Logo", "PCL\\Logo.png");
                    ModBase.WriteIni(CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\\Setup.ini", "LogoCustom", "True");
                    Logger.Info(string.Format("已设置整合包 Logo：{0}", CS$<>8__locals1.$VB$Local_Logo), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                }
                string text4 = string.Format("{0}saves\\", CS$<>8__locals1.$VB$Local_VersionFolder);
                try
                {
                    foreach (string text5 in DirectoryUtils.EnumerateFiles(text4, false, "*.zip"))
                    {
                        FileUtils.ExtractToDirectory(text5, text4, Task.CreateCancellationToken(), null);
                        FileUtils.Delete(text5, false);
                    }
                }
                finally
                {
                    IEnumerator<string> enumerator5;
                    if (enumerator5 != null)
                    {
                        enumerator5.Dispose();
                    }
                }
                foreach (string text6 in new string[]
                {
                    CS$<>8__locals1.$VB$Local_VersionFolder + "原始整合包.zip",
                    CS$<>8__locals1.$VB$Local_VersionFolder + "原始整合包.mrpack"
                })
                {
                    if (FileUtils.Exists(text6))
                    {
                        Logger.Info(string.Format("删除原始整合包文件：{0}", text6), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        FileUtils.Delete(text6, false);
                    }
                }
                if (FileUtils.Exists(CS$<>8__locals1.$VB$Local_FileAddress) && Operators.CompareString(PathUtils.GetFileNameWithoutExtension(CS$<>8__locals1.$VB$Local_FileAddress), "modpack", false) == 0)
                {
                    Logger.Info(string.Format("删除安装整合包文件：{0}", CS$<>8__locals1.$VB$Local_FileAddress), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                    FileUtils.Delete(CS$<>8__locals1.$VB$Local_FileAddress, false);
                }
            }, null, ThreadPriority.Normal)
            {
                ProgressWeight = 0.1,
                Show = false
            });
            CS$<>8__locals1.$VB$Local_LoaderName = "CurseForge 整合包安装：" + InstanceName + " ";
            if (ModLoader.LoaderTaskbar.Any<ModLoader.LoaderBase>((ModLoader.LoaderBase l) => Operators.CompareString(l.Name, CS$<>8__locals1.$VB$Local_LoaderName, false) == 0))
            {
                ModMain.Hint("该整合包正在安装中！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>(CS$<>8__locals1.$VB$Local_LoaderName, list4);
            loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.McInstallState);
            loaderCombo.Start(CS$<>8__locals1.$VB$Local_VersionFolder, false);
            ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
            return loaderCombo;
        }

        private static ModLoader.LoaderCombo<string> InstallPackModrinth(string FileAddress, ZipArchive Archive, string ArchiveBaseFolder, string InstanceName = null, string Logo = null)
        {
            ModModpack._Closure$__6-0 CS$<>8__locals1 = new ModModpack._Closure$__6-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_FileAddress = FileAddress;
            CS$<>8__locals1.$VB$Local_ArchiveBaseFolder = ArchiveBaseFolder;
            CS$<>8__locals1.$VB$Local_Logo = Logo;
            JObject jobject;
            try
            {
                jobject = (JObject)Archive.GetEntry(CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "modrinth.index.json").Open().ReadString(null)
                    .DeserializeJson();
            }
            catch (Exception ex)
            {
                throw new Exception("Modrinth 整合包安装信息存在问题", ex);
            }
            if (jobject["dependencies"] == null || jobject["dependencies"]["minecraft"] == null)
            {
                throw new Exception("Modrinth 整合包未提供 Minecraft 版本信息");
            }
            McVersion mcVersion = new McVersion("Unknown", true);
            try
            {
                foreach (JToken jtoken in (jobject["dependencies"] ?? new byte[0]))
                {
                    JProperty jproperty = (JProperty)jtoken;
                    string text = jproperty.Name.Lower();
                    if (Operators.CompareString(text, "minecraft", false) != 0)
                    {
                        if (Operators.CompareString(text, "forge", false) != 0)
                        {
                            if (Operators.CompareString(text, "neoforge", false) != 0 && Operators.CompareString(text, "neo-forge", false) != 0)
                            {
                                if (Operators.CompareString(text, "fabric-loader", false) != 0)
                                {
                                    ModModpack.NotifyIncompatibleLoader(jproperty.Name);
                                }
                                else
                                {
                                    mcVersion.ModLoaderVersions[ModLoaders.Fabric] = jproperty.Value.ToString();
                                    Logger.Info(string.Format("整合包 Fabric 版本：{0}", mcVersion.ModLoaderVersions[ModLoaders.Fabric]), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                                }
                            }
                            else
                            {
                                mcVersion.ModLoaderVersions[ModLoaders.NeoForge] = jproperty.Value.ToString();
                                Logger.Info(string.Format("整合包 NeoForge 版本：{0}", mcVersion.ModLoaderVersions[ModLoaders.NeoForge]), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                            }
                        }
                        else
                        {
                            mcVersion.ModLoaderVersions[ModLoaders.Forge] = jproperty.Value.ToString();
                            Logger.Info(string.Format("整合包 Forge 版本：{0}", mcVersion.ModLoaderVersions[ModLoaders.Forge]), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        }
                    }
                    else
                    {
                        mcVersion.VanillaName = jproperty.Value.ToString();
                    }
                }
            }
            finally
            {
                IEnumerator<JToken> enumerator;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            if (InstanceName == null)
            {
                InstanceName = (string)(jobject["name"] ?? "");
                ValidateFolderName validateFolderName = new ValidateFolderName(ModMinecraft.McFolderSelected + "versions", true, null);
                if (Operators.CompareString(validateFolderName.Validate(InstanceName), "", false) != 0)
                {
                    InstanceName = "";
                }
                if (Operators.CompareString(InstanceName, "", false) == 0)
                {
                    InstanceName = ModMain.MyMsgBoxInput("输入版本名称", "", "", new Collection<Validate> { validateFolderName }, "", "确定", "取消", false);
                }
                if (string.IsNullOrEmpty(InstanceName))
                {
                    throw new OperationCanceledException();
                }
            }
            CS$<>8__locals1.$VB$Local_VersionFolder = Path.Combine(ModMinecraft.McFolderSelected, "versions", InstanceName) + "\\";
            CS$<>8__locals1.$VB$Local_InstallTemp = ModMain.RequestTaskTempFolder(false);
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            list.Add(new ModLoader.LoaderTask<string, int>("解压整合包文件", delegate(ModLoader.LoaderTask<string, int> Task)
            {
                ModModpack.ExtractModpackFiles(CS$<>8__locals1.$VB$Local_InstallTemp, CS$<>8__locals1.$VB$Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.5));
                ModModpack.CopyOverrideDirectory(CS$<>8__locals1.$VB$Local_InstallTemp + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "overrides", CS$<>8__locals1.$VB$Local_VersionFolder, Task);
                ModModpack.CopyOverrideDirectory(CS$<>8__locals1.$VB$Local_InstallTemp + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "client-overrides", CS$<>8__locals1.$VB$Local_VersionFolder, Task);
            }, null, ThreadPriority.Normal)
            {
                ProgressWeight = (double)FileUtils.GetInfo(CS$<>8__locals1.$VB$Local_FileAddress).Length / 1024.0 / 1024.0 / 6.0,
                Block = false
            });
            List<ModNet.NetFile> list2 = new List<ModNet.NetFile>();
            try
            {
                foreach (JToken jtoken2 in (jobject["files"] ?? new byte[0]))
                {
                    if (jtoken2["env"] != null)
                    {
                        string text2 = jtoken2["env"]["client"].ToString();
                        if (Operators.CompareString(text2, "optional", false) != 0)
                        {
                            if (Operators.CompareString(text2, "unsupported", false) == 0)
                            {
                                continue;
                            }
                        }
                        else if (ModMain.MyMsgBox("是否要下载可选文件 " + PathUtils.GetLastPart(jtoken2["path"].ToString()) + "？", "下载可选文件", "是", "否", "", false, true, false, null, null, null) == 2)
                        {
                            continue;
                        }
                    }
                    List<string> list3 = jtoken2["downloads"].SelectMany<JToken, string>((ModModpack._Closure$__.$I6-1 == null) ? (ModModpack._Closure$__.$I6-1 = (JToken t) => ResourceVersion.ParseCurseForgeDownloadUrls(t.ToString())) : ModModpack._Closure$__.$I6-1).ToList<string>();
                    list3.AddRange(list3.Select<string, string>((ModModpack._Closure$__.$I6-2 == null) ? (ModModpack._Closure$__.$I6-2 = (string u) => ModDownload.DlSourceModGet(u)) : ModModpack._Closure$__.$I6-2).ToList<string>());
                    list3 = ResourceVersion.ParseModrinthTrackArguments(list3, new ResourceVersion.DownloadReason?(ResourceVersion.DownloadReason.ModPack), mcVersion.VanillaName, mcVersion.ModLoaders).ToList<string>();
                    list3 = list3.Distinct<string>().ToList<string>();
                    string text3 = Path.Combine(CS$<>8__locals1.$VB$Local_VersionFolder, jtoken2["path"].ToString().TrimStart(new char[]
                    {
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar
                    }));
                    if (!PathUtils.IsParentOf(CS$<>8__locals1.$VB$Local_VersionFolder, text3))
                    {
                        ModMain.MyMsgBox(string.Format("整合包的文件路径超出了版本文件夹，请向整合包作者反馈此问题！{0}目标：{1}{2}错误的文件：{3}", new object[]
                        {
                            "\r\n",
                            Path.GetFullPath(text3),
                            "\r\n",
                            text3
                        }), "文件路径校验失败", "确定", "", "", true, true, false, null, null, null);
                        throw new OperationCanceledException();
                    }
                    list2.Add(new ModNet.NetFile(list3, text3, new FileChecker
                    {
                        Hash = jtoken2["hashes"]["sha1"].ToString()
                    }, true));
                }
            }
            finally
            {
                IEnumerator<JToken> enumerator2;
                if (enumerator2 != null)
                {
                    enumerator2.Dispose();
                }
            }
            if (list2.Any<ModNet.NetFile>())
            {
                list.Add(new ModNet.LoaderDownload("下载额外文件", list2)
                {
                    ProgressWeight = (double)list2.Count * 1.5
                });
            }
            McComponents mcComponents = new McComponents(mcVersion.VanillaName, mcVersion.ModLoaderVersions, null, null, null);
            List<ModLoader.LoaderBase> list4 = ModDownloadLib.McInstallLoadersGet(new ModDownloadLib.McInstallRequest
            {
                NewInstanceName = InstanceName,
                InstanceFolder = CS$<>8__locals1.$VB$Local_VersionFolder,
                Components = mcComponents
            });
            List<ModLoader.LoaderBase> list5 = new List<ModLoader.LoaderBase>();
            list5.Add(new ModLoader.LoaderCombo<string>("整合包安装", list)
            {
                Show = false,
                Block = false,
                ProgressWeight = list.Sum<ModLoader.LoaderBase>((ModModpack._Closure$__.$I6-3 == null) ? (ModModpack._Closure$__.$I6-3 = (ModLoader.LoaderBase l) => l.ProgressWeight) : ModModpack._Closure$__.$I6-3)
            });
            list5.Add(new ModLoader.LoaderCombo<string>("游戏安装", list4)
            {
                Show = false,
                ProgressWeight = list4.Sum<ModLoader.LoaderBase>((ModModpack._Closure$__.$I6-4 == null) ? (ModModpack._Closure$__.$I6-4 = (ModLoader.LoaderBase l) => l.ProgressWeight) : ModModpack._Closure$__.$I6-4)
            });
            list5.Add(new ModLoader.LoaderTask<string, string>("最终整理文件", delegate(ModLoader.LoaderTask<string, string> Task)
            {
                if (CS$<>8__locals1.$VB$Local_Logo != null && FileUtils.Exists(CS$<>8__locals1.$VB$Local_Logo))
                {
                    FileUtils.Copy(CS$<>8__locals1.$VB$Local_Logo, CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\\Logo.png");
                    ModBase.WriteIni(CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\\Setup.ini", "Logo", "PCL\\Logo.png");
                    ModBase.WriteIni(CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\\Setup.ini", "LogoCustom", "True");
                    Logger.Info(string.Format("已设置整合包 Logo：{0}", CS$<>8__locals1.$VB$Local_Logo), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                }
                foreach (string text4 in new string[]
                {
                    CS$<>8__locals1.$VB$Local_VersionFolder + "原始整合包.zip",
                    CS$<>8__locals1.$VB$Local_VersionFolder + "原始整合包.mrpack"
                })
                {
                    if (FileUtils.Exists(text4))
                    {
                        Logger.Info(string.Format("删除原始整合包文件：{0}", text4), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        FileUtils.Delete(text4, false);
                    }
                }
                if (FileUtils.Exists(CS$<>8__locals1.$VB$Local_FileAddress) && Operators.CompareString(PathUtils.GetFileNameWithoutExtension(CS$<>8__locals1.$VB$Local_FileAddress), "modpack", false) == 0)
                {
                    Logger.Info(string.Format("删除安装整合包文件：{0}", CS$<>8__locals1.$VB$Local_FileAddress), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                    FileUtils.Delete(CS$<>8__locals1.$VB$Local_FileAddress, false);
                }
            }, null, ThreadPriority.Normal)
            {
                ProgressWeight = 0.1,
                Show = false
            });
            CS$<>8__locals1.$VB$Local_LoaderName = string.Format("Modrinth 整合包安装：{0} ", InstanceName);
            if (ModLoader.LoaderTaskbar.Any<ModLoader.LoaderBase>((ModLoader.LoaderBase l) => Operators.CompareString(l.Name, CS$<>8__locals1.$VB$Local_LoaderName, false) == 0))
            {
                ModMain.Hint("该整合包正在安装中！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>(CS$<>8__locals1.$VB$Local_LoaderName, list5);
            loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.McInstallState);
            loaderCombo.Start(CS$<>8__locals1.$VB$Local_VersionFolder, false);
            ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
            return loaderCombo;
        }

        private static ModLoader.LoaderCombo<string> InstallPackHMCL(string FileAddress, ZipArchive Archive, string ArchiveBaseFolder)
        {
            JObject jobject;
            try
            {
                jobject = (JObject)Archive.GetEntry(ArchiveBaseFolder + "modpack.json").Open().ReadString(Encoding.UTF8)
                    .DeserializeJson();
            }
            catch (Exception ex)
            {
                throw new Exception("HMCL 整合包安装信息存在问题", ex);
            }
            string text = (string)(jobject["name"] ?? "");
            ValidateFolderName validateFolderName = new ValidateFolderName(ModMinecraft.McFolderSelected + "versions", true, null);
            if (Operators.CompareString(validateFolderName.Validate(text), "", false) != 0)
            {
                text = "";
            }
            if (Operators.CompareString(text, "", false) == 0)
            {
                text = ModMain.MyMsgBoxInput("输入版本名称", "", "", new Collection<Validate> { validateFolderName }, "", "确定", "取消", false);
            }
            if (string.IsNullOrEmpty(text))
            {
                throw new OperationCanceledException();
            }
            string VersionFolder = Path.Combine(ModMinecraft.McFolderSelected, "versions", text) + "\\";
            string InstallTemp = ModMain.RequestTaskTempFolder(false);
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            list.Add(new ModLoader.LoaderTask<string, int>("解压整合包文件", delegate(ModLoader.LoaderTask<string, int> Task)
            {
                ModModpack.ExtractModpackFiles(InstallTemp, FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.6));
                ModModpack.CopyOverrideDirectory(InstallTemp + ArchiveBaseFolder + "minecraft", VersionFolder, Task);
            }, null, ThreadPriority.Normal)
            {
                ProgressWeight = (double)FileUtils.GetInfo(FileAddress).Length / 1024.0 / 1024.0 / 6.0,
                Block = false
            });
            if (jobject["gameVersion"] == null)
            {
                throw new Exception("该 HMCL 整合包未提供游戏版本信息，无法安装！");
            }
            McComponents mcComponents = new McComponents(jobject["gameVersion"].ToString(), new Dictionary<ModLoaders, string>(), null, null, null);
            List<ModLoader.LoaderBase> list2 = ModDownloadLib.McInstallLoadersGet(new ModDownloadLib.McInstallRequest
            {
                NewInstanceName = text,
                InstanceFolder = VersionFolder,
                Components = mcComponents
            });
            List<ModLoader.LoaderBase> list3 = new List<ModLoader.LoaderBase>
            {
                new ModLoader.LoaderCombo<string>("整合包安装", list)
                {
                    Show = false,
                    Block = false,
                    ProgressWeight = list.Sum<ModLoader.LoaderBase>((ModModpack._Closure$__.$I7-1 == null) ? (ModModpack._Closure$__.$I7-1 = (ModLoader.LoaderBase l) => l.ProgressWeight) : ModModpack._Closure$__.$I7-1)
                },
                new ModLoader.LoaderCombo<string>("游戏安装", list2)
                {
                    Show = false,
                    ProgressWeight = list2.Sum<ModLoader.LoaderBase>((ModModpack._Closure$__.$I7-2 == null) ? (ModModpack._Closure$__.$I7-2 = (ModLoader.LoaderBase l) => l.ProgressWeight) : ModModpack._Closure$__.$I7-2)
                }
            };
            string LoaderName = "HMCL 整合包安装：" + text + " ";
            if (ModLoader.LoaderTaskbar.Any<ModLoader.LoaderBase>((ModLoader.LoaderBase l) => Operators.CompareString(l.Name, LoaderName, false) == 0))
            {
                ModMain.Hint("该整合包正在安装中！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>(LoaderName, list3);
            loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.McInstallState);
            loaderCombo.Start(VersionFolder, false);
            ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
            return loaderCombo;
        }

        private static ModLoader.LoaderCombo<string> InstallPackMMC(string FileAddress, ZipArchive Archive, string ArchiveBaseFolder)
        {
            ModModpack._Closure$__8-0 CS$<>8__locals1 = new ModModpack._Closure$__8-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_FileAddress = FileAddress;
            CS$<>8__locals1.$VB$Local_ArchiveBaseFolder = ArchiveBaseFolder;
            JObject jobject;
            string text;
            try
            {
                jobject = (JObject)Archive.GetEntry(CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "mmc-pack.json").Open().ReadString(Encoding.UTF8)
                    .DeserializeJson();
                text = Archive.GetEntry(CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "instance.cfg").Open().ReadString(Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception("MMC 整合包安装信息存在问题", ex);
            }
            string text2 = text.RegexSeek("(?<=\\nname\\=)[^\\n]+", RegexOptions.None) ?? "";
            ValidateFolderName validateFolderName = new ValidateFolderName(ModMinecraft.McFolderSelected + "versions", true, null);
            if (Operators.CompareString(validateFolderName.Validate(text2), "", false) != 0)
            {
                text2 = "";
            }
            if (Operators.CompareString(text2, "", false) == 0)
            {
                text2 = ModMain.MyMsgBoxInput("输入版本名称", "", "", new Collection<Validate> { validateFolderName }, "", "确定", "取消", false);
            }
            if (string.IsNullOrEmpty(text2))
            {
                throw new OperationCanceledException();
            }
            CS$<>8__locals1.$VB$Local_VersionFolder = Path.Combine(ModMinecraft.McFolderSelected, "versions", text2) + "\\";
            CS$<>8__locals1.$VB$Local_InstallTemp = ModMain.RequestTaskTempFolder(false);
            CS$<>8__locals1.$VB$Local_SetupFile = CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\\Setup.ini";
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            list.Add(new ModLoader.LoaderTask<string, int>("解压整合包文件", delegate(ModLoader.LoaderTask<string, int> Task)
            {
                ModModpack.ExtractModpackFiles(CS$<>8__locals1.$VB$Local_InstallTemp, CS$<>8__locals1.$VB$Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.55));
                string text5;
                if (DirectoryUtils.Exists(CS$<>8__locals1.$VB$Local_InstallTemp + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + ".minecraft") && !DirectoryUtils.Exists(CS$<>8__locals1.$VB$Local_InstallTemp + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "minecraft"))
                {
                    text5 = CS$<>8__locals1.$VB$Local_InstallTemp + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + ".minecraft";
                }
                else
                {
                    text5 = CS$<>8__locals1.$VB$Local_InstallTemp + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "minecraft";
                }
                ModModpack.CopyOverrideDirectory(text5, CS$<>8__locals1.$VB$Local_VersionFolder, Task);
                try
                {
                    string text6 = CS$<>8__locals1.$VB$Local_InstallTemp + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "instance.cfg";
                    if (FileUtils.Exists(text6))
                    {
                        List<string> list4 = new List<string>();
                        foreach (string text7 in FileUtils.ReadAsLines(text6, true, null, null))
                        {
                            if (text7.Contains("="))
                            {
                                list4.Add(text7.BeforeFirst("=", false) + ":" + text7.AfterFirst("=", false));
                            }
                        }
                        FileUtils.Write(text6, list4.Join("\r\n"), null);
                        if (Conversions.ToBoolean(ModBase.ReadIni(text6, "OverrideCommands", Conversions.ToString(false))))
                        {
                            string text8 = ModBase.ReadIni(text6, "PreLaunchCommand", "");
                            if (Operators.CompareString(text8, "", false) != 0)
                            {
                                text8 = text8.Replace("\\\"", "\"").Replace("$INST_JAVA", "{java}java.exe").Replace("$INST_MC_DIR\\", "{minecraft}")
                                    .Replace("$INST_MC_DIR", "{minecraft}")
                                    .Replace("$INST_DIR\\", "{verpath}")
                                    .Replace("$INST_DIR", "{verpath}")
                                    .Replace("$INST_ID", "{name}")
                                    .Replace("$INST_NAME", "{name}");
                                ModBase.WriteIni(CS$<>8__locals1.$VB$Local_SetupFile, "VersionAdvanceRun", text8);
                                Logger.Info(string.Format("迁移 MultiMC 版本独立设置：启动前执行命令：{0}", text8), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                            }
                        }
                        if (Conversions.ToBoolean(ModBase.ReadIni(text6, "JoinServerOnLaunch", Conversions.ToString(false))))
                        {
                            string text9 = ModBase.ReadIni(text6, "JoinServerOnLaunchAddress", "").Replace("\\\"", "\"");
                            ModBase.WriteIni(CS$<>8__locals1.$VB$Local_SetupFile, "VersionServerEnter", text9);
                            Logger.Info(string.Format("迁移 MultiMC 版本独立设置：自动进入服务器：{0}", text9), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        }
                        string text10 = ModBase.ReadIni(text6, "iconKey", "");
                        if (Operators.CompareString(text10, "", false) != 0 && FileUtils.Exists(string.Format("{0}{1}{2}.png", CS$<>8__locals1.$VB$Local_InstallTemp, CS$<>8__locals1.$VB$Local_ArchiveBaseFolder, text10)))
                        {
                            ModBase.WriteIni(CS$<>8__locals1.$VB$Local_SetupFile, "LogoCustom", Conversions.ToString(true));
                            ModBase.WriteIni(CS$<>8__locals1.$VB$Local_SetupFile, "Logo", "PCL\\Logo.png");
                            FileUtils.Copy(string.Format("{0}{1}{2}.png", CS$<>8__locals1.$VB$Local_InstallTemp, CS$<>8__locals1.$VB$Local_ArchiveBaseFolder, text10), CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\\Logo.png");
                            Logger.Info(string.Format("迁移 MultiMC 版本独立设置：版本图标（{0}.png）", text10), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        }
                        string text11 = ModBase.ReadIni(text6, "JvmArgs", "");
                        if (Operators.CompareString(text11, "", false) != 0)
                        {
                            if (Conversions.ToBoolean(ModBase.ReadIni(text6, "OverrideJavaArgs", Conversions.ToString(false))))
                            {
                                ModBase.WriteIni(CS$<>8__locals1.$VB$Local_SetupFile, "VersionAdvanceJvm", text11);
                                Logger.Info(string.Format("迁移 MultiMC 版本独立设置：JVM 参数（覆盖）：{0}", text11), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                            }
                            else
                            {
                                text11 = text11 + " " + Settings.Get<string>("LaunchAdvanceJvm", null);
                                ModBase.WriteIni(CS$<>8__locals1.$VB$Local_SetupFile, "VersionAdvanceJvm", text11);
                                Logger.Info(string.Format("迁移 MultiMC 版本独立设置：JVM 参数（追加）：{0}", text11), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                            }
                        }
                    }
                }
                catch (Exception ex2)
                {
                    Logger.Warn(ex2, string.Format("读取 MMC 配置文件失败（{0}{1}instance.cfg）", CS$<>8__locals1.$VB$Local_InstallTemp, CS$<>8__locals1.$VB$Local_ArchiveBaseFolder), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                }
            }, null, ThreadPriority.Normal)
            {
                ProgressWeight = (double)FileUtils.GetInfo(CS$<>8__locals1.$VB$Local_FileAddress).Length / 1024.0 / 1024.0 / 6.0,
                Block = false
            });
            if (jobject["components"] == null)
            {
                throw new Exception("该 MMC 整合包未提供游戏版本信息，无法安装！");
            }
            string text3 = null;
            Dictionary<ModLoaders, string> dictionary = new Dictionary<ModLoaders, string>();
            try
            {
                foreach (JToken jtoken in jobject["components"])
                {
                    JObject jobject2 = (JObject)jtoken;
                    if (jobject2.ContainsKey("uid"))
                    {
                        string text4 = jobject2["uid"].ToString();
                        if (Operators.CompareString(text4, "net.minecraft", false) != 0)
                        {
                            if (Operators.CompareString(text4, "net.minecraftforge", false) != 0)
                            {
                                if (Operators.CompareString(text4, "net.neoforged", false) != 0)
                                {
                                    if (Operators.CompareString(text4, "net.fabricmc.fabric-loader", false) != 0)
                                    {
                                        if (text4.StartsWithF("org.lwjgl", false))
                                        {
                                            Logger.Info(string.Format("已跳过 LWJGL 项：{0}", text4), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                                        }
                                        else
                                        {
                                            ModModpack.NotifyIncompatibleLoader(text4);
                                        }
                                    }
                                    else
                                    {
                                        dictionary[ModLoaders.Fabric] = (string)jobject2["version"];
                                    }
                                }
                                else
                                {
                                    dictionary[ModLoaders.NeoForge] = (string)jobject2["version"];
                                }
                            }
                            else
                            {
                                dictionary[ModLoaders.Forge] = (string)jobject2["version"];
                            }
                        }
                        else
                        {
                            text3 = (string)jobject2["version"];
                        }
                    }
                }
            }
            finally
            {
                IEnumerator<JToken> enumerator;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            McComponents mcComponents = new McComponents(text3, dictionary, null, null, null);
            List<ModLoader.LoaderBase> list2 = ModDownloadLib.McInstallLoadersGet(new ModDownloadLib.McInstallRequest
            {
                NewInstanceName = text2,
                InstanceFolder = CS$<>8__locals1.$VB$Local_VersionFolder,
                Components = mcComponents
            });
            List<ModLoader.LoaderBase> list3 = new List<ModLoader.LoaderBase>();
            list3.Add(new ModLoader.LoaderCombo<string>("整合包安装", list)
            {
                Show = false,
                Block = false,
                ProgressWeight = list.Sum<ModLoader.LoaderBase>((ModModpack._Closure$__.$I8-1 == null) ? (ModModpack._Closure$__.$I8-1 = (ModLoader.LoaderBase l) => l.ProgressWeight) : ModModpack._Closure$__.$I8-1)
            });
            list3.Add(new ModLoader.LoaderCombo<string>("游戏安装", list2)
            {
                Show = false,
                ProgressWeight = list2.Sum<ModLoader.LoaderBase>((ModModpack._Closure$__.$I8-2 == null) ? (ModModpack._Closure$__.$I8-2 = (ModLoader.LoaderBase l) => l.ProgressWeight) : ModModpack._Closure$__.$I8-2)
            });
            CS$<>8__locals1.$VB$Local_LoaderName = "MMC 整合包安装：" + text2 + " ";
            if (ModLoader.LoaderTaskbar.Any<ModLoader.LoaderBase>((ModLoader.LoaderBase l) => Operators.CompareString(l.Name, CS$<>8__locals1.$VB$Local_LoaderName, false) == 0))
            {
                ModMain.Hint("该整合包正在安装中！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>(CS$<>8__locals1.$VB$Local_LoaderName, list3);
            loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.McInstallState);
            loaderCombo.Start(CS$<>8__locals1.$VB$Local_VersionFolder, false);
            ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
            return loaderCombo;
        }

        private static ModLoader.LoaderCombo<string> InstallPackMCBBS(string FileAddress, ZipArchive Archive, string ArchiveBaseFolder, string InstanceName = null)
        {
            ModModpack._Closure$__9-0 CS$<>8__locals1 = new ModModpack._Closure$__9-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_FileAddress = FileAddress;
            CS$<>8__locals1.$VB$Local_ArchiveBaseFolder = ArchiveBaseFolder;
            try
            {
                ZipArchiveEntry zipArchiveEntry = Archive.GetEntry(CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "mcbbs.packmeta") ?? Archive.GetEntry(CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "manifest.json");
                CS$<>8__locals1.$VB$Local_Json = (JObject)zipArchiveEntry.Open().ReadString(Encoding.UTF8).DeserializeJson();
            }
            catch (Exception ex)
            {
                throw new Exception("MCBBS 整合包安装信息存在问题", ex);
            }
            if (InstanceName == null)
            {
                InstanceName = (string)(CS$<>8__locals1.$VB$Local_Json["name"] ?? "");
                ValidateFolderName validateFolderName = new ValidateFolderName(ModMinecraft.McFolderSelected + "versions", true, null);
                if (Operators.CompareString(validateFolderName.Validate(InstanceName), "", false) != 0)
                {
                    InstanceName = "";
                }
                if (Operators.CompareString(InstanceName, "", false) == 0)
                {
                    InstanceName = ModMain.MyMsgBoxInput("输入版本名称", "", "", new Collection<Validate> { validateFolderName }, "", "确定", "取消", false);
                }
                if (string.IsNullOrEmpty(InstanceName))
                {
                    throw new OperationCanceledException();
                }
            }
            CS$<>8__locals1.$VB$Local_VersionFolder = Path.Combine(ModMinecraft.McFolderSelected, "versions", InstanceName) + "\\";
            CS$<>8__locals1.$VB$Local_InstallTemp = ModMain.RequestTaskTempFolder(false);
            CS$<>8__locals1.$VB$Local_SetupFile = CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\\Setup.ini";
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            list.Add(new ModLoader.LoaderTask<string, int>("解压整合包文件", delegate(ModLoader.LoaderTask<string, int> Task)
            {
                ModModpack.ExtractModpackFiles(CS$<>8__locals1.$VB$Local_InstallTemp, CS$<>8__locals1.$VB$Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.6));
                ModModpack.CopyOverrideDirectory(CS$<>8__locals1.$VB$Local_InstallTemp + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "overrides", CS$<>8__locals1.$VB$Local_VersionFolder, Task);
                if (CS$<>8__locals1.$VB$Local_Json["launchInfo"] != null)
                {
                    JObject jobject2 = (JObject)CS$<>8__locals1.$VB$Local_Json["launchInfo"];
                    if (jobject2.ContainsKey("javaArgument"))
                    {
                        ModBase.WriteIni(CS$<>8__locals1.$VB$Local_SetupFile, "VersionAdvanceJvm", string.Join<JToken>(" ", jobject2["javaArgument"]));
                    }
                    if (jobject2.ContainsKey("launchArgument"))
                    {
                        ModBase.WriteIni(CS$<>8__locals1.$VB$Local_SetupFile, "VersionAdvanceGame", string.Join<JToken>(" ", jobject2["launchArgument"]));
                    }
                }
            }, null, ThreadPriority.Normal)
            {
                ProgressWeight = (double)FileUtils.GetInfo(CS$<>8__locals1.$VB$Local_FileAddress).Length / 1024.0 / 1024.0 / 6.0,
                Block = false
            });
            if (CS$<>8__locals1.$VB$Local_Json["addons"] == null)
            {
                throw new Exception("该 MCBBS 整合包未提供游戏版本附加信息，无法安装！");
            }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                foreach (JToken jtoken in CS$<>8__locals1.$VB$Local_Json["addons"])
                {
                    dictionary.Add((string)jtoken["id"], (string)jtoken["version"]);
                }
            }
            finally
            {
                IEnumerator<JToken> enumerator;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            if (!dictionary.ContainsKey("game"))
            {
                throw new Exception("该 MCBBS 整合包未提供游戏版本信息，无法安装！");
            }
            if (dictionary.ContainsKey("quilt"))
            {
                ModMain.Hint("PCL 不支持安装 Quilt 整合包！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            Dictionary<ModLoaders, string> dictionary2 = new Dictionary<ModLoaders, string>();
            if (dictionary.ContainsKey("forge"))
            {
                dictionary2[ModLoaders.Forge] = dictionary["forge"];
            }
            if (dictionary.ContainsKey("neoforge"))
            {
                dictionary2[ModLoaders.NeoForge] = dictionary["neoforge"];
            }
            if (dictionary.ContainsKey("fabric"))
            {
                dictionary2[ModLoaders.Fabric] = dictionary["fabric"];
            }
            JObject jobject = null;
            if (dictionary.ContainsKey("optifine"))
            {
                ModModpack._Closure$__9-1 CS$<>8__locals2 = new ModModpack._Closure$__9-1(CS$<>8__locals2);
                CS$<>8__locals2.$VB$Local_MinecraftVersion = dictionary["game"];
                if (CS$<>8__locals2.$VB$Local_MinecraftVersion.EndsWithF(".0", false))
                {
                    CS$<>8__locals2.$VB$Local_MinecraftVersion = CS$<>8__locals2.$VB$Local_MinecraftVersion.Substring(0, checked(CS$<>8__locals2.$VB$Local_MinecraftVersion.Length - 2));
                }
                CS$<>8__locals2.$VB$Local_OptiFineFileSuffix = "_HD_" + dictionary["optifine"].AfterLast("HD_", false) + ".jar";
                if (ModDownload.DlOptiFineListBmclapiLoader.State != ModBase.LoadState.Finished)
                {
                    ModDownload.DlOptiFineListBmclapiLoader.WaitForExit(null, null, false, default(CancellationToken), null);
                }
                ModDownload.DlOptiFineListEntry dlOptiFineListEntry = ModDownload.DlOptiFineListBmclapiLoader.Output.Value.FirstOrDefault<ModDownload.DlOptiFineListEntry>((ModDownload.DlOptiFineListEntry v) => Operators.CompareString(v.Inherit, CS$<>8__locals2.$VB$Local_MinecraftVersion, false) == 0 && v.FileName.EndsWithF(CS$<>8__locals2.$VB$Local_OptiFineFileSuffix, false));
                if (dlOptiFineListEntry == null)
                {
                    throw new Exception(string.Format("OptiFine 列表中没有整合包所需的 OptiFine {0}（Minecraft {1}）", dictionary["optifine"], dictionary["game"]));
                }
                jobject = JObject.FromObject(dlOptiFineListEntry);
            }
            McComponents mcComponents = new McComponents(dictionary["game"], dictionary2, jobject, null, null);
            List<ModLoader.LoaderBase> list2 = ModDownloadLib.McInstallLoadersGet(new ModDownloadLib.McInstallRequest
            {
                NewInstanceName = InstanceName,
                InstanceFolder = CS$<>8__locals1.$VB$Local_VersionFolder,
                Components = mcComponents
            });
            List<ModLoader.LoaderBase> list3 = new List<ModLoader.LoaderBase>();
            list3.Add(new ModLoader.LoaderCombo<string>("整合包安装", list)
            {
                Show = false,
                Block = false,
                ProgressWeight = list.Sum<ModLoader.LoaderBase>((ModModpack._Closure$__.$I9-2 == null) ? (ModModpack._Closure$__.$I9-2 = (ModLoader.LoaderBase l) => l.ProgressWeight) : ModModpack._Closure$__.$I9-2)
            });
            list3.Add(new ModLoader.LoaderCombo<string>("游戏安装", list2)
            {
                Show = false,
                ProgressWeight = list2.Sum<ModLoader.LoaderBase>((ModModpack._Closure$__.$I9-3 == null) ? (ModModpack._Closure$__.$I9-3 = (ModLoader.LoaderBase l) => l.ProgressWeight) : ModModpack._Closure$__.$I9-3)
            });
            CS$<>8__locals1.$VB$Local_LoaderName = "MCBBS 整合包安装：" + InstanceName + " ";
            if (ModLoader.LoaderTaskbar.Any<ModLoader.LoaderBase>((ModLoader.LoaderBase l) => Operators.CompareString(l.Name, CS$<>8__locals1.$VB$Local_LoaderName, false) == 0))
            {
                ModMain.Hint("该整合包正在安装中！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>(CS$<>8__locals1.$VB$Local_LoaderName, list3);
            loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.McInstallState);
            loaderCombo.Start(CS$<>8__locals1.$VB$Local_VersionFolder, false);
            ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
            return loaderCombo;
        }

        private static ModLoader.LoaderCombo<string> InstallPackLauncherPack(string FileAddress, ZipArchive Archive, string ArchiveBaseFolder)
        {
            ModModpack._Closure$__10-0 CS$<>8__locals1 = new ModModpack._Closure$__10-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_FileAddress = FileAddress;
            CS$<>8__locals1.$VB$Local_ArchiveBaseFolder = ArchiveBaseFolder;
            ModMain.MyMsgBox("接下来请选择一个空文件夹，它会被安装到这个文件夹里。", "安装", "继续", "", "", false, true, true, null, null, null);
            CS$<>8__locals1.$VB$Local_TargetFolder = Dialogs.SelectFolder("选择安装目标（必须是一个空文件夹）", false, null).FirstOrDefault<string>();
            if (CS$<>8__locals1.$VB$Local_TargetFolder == null)
            {
                throw new OperationCanceledException();
            }
            if (!DirectoryUtils.IsEmpty(CS$<>8__locals1.$VB$Local_TargetFolder))
            {
                ModMain.Hint("请选择一个空文件夹作为安装目标！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>("解压压缩包", new ModLoader.LoaderBase[]
            {
                new ModLoader.LoaderTask<string, int>("解压压缩包", delegate(ModLoader.LoaderTask<string, int> Task)
                {
                    ModModpack.ExtractModpackFiles(CS$<>8__locals1.$VB$Local_TargetFolder, CS$<>8__locals1.$VB$Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.9));
                    ModBase.OpenExplorer(CS$<>8__locals1.$VB$Local_TargetFolder);
                    Thread.Sleep(400);
                    string text = null;
                    try
                    {
                        foreach (string text2 in DirectoryUtils.EnumerateFiles(CS$<>8__locals1.$VB$Local_TargetFolder, false, "*.exe"))
                        {
                            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(text2);
                            Logger.Info(string.Format("文件 {0} 的产品名标识为 {1}", text2, versionInfo.ProductName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                            if (Operators.CompareString(versionInfo.ProductName, "Plain Craft Launcher", false) == 0)
                            {
                                text = text2;
                                Logger.Info(string.Format("发现整合包附带的 PCL 启动器：{0}", text2), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                            }
                            else if ((versionInfo.ProductName.ContainsIgnoreCase("Launcher") || versionInfo.ProductName.Contains("启动")) && Operators.CompareString(versionInfo.ProductName, "Plain Craft Launcher Admin Manager", false) != 0 && text == null)
                            {
                                text = text2;
                                Logger.Info(string.Format("发现整合包附带的疑似第三方启动器：{0}", text2), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
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
                    Task.Progress = 0.95;
                    if (text != null)
                    {
                        Logger.Info(string.Format("找到压缩包中附带的启动器：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        if (ModMain.MyMsgBox(string.Format("整合包里似乎自带了启动器，是否换用它继续安装？{0}即将打开：{1}", "\r\n", text), "换用整合包启动器？", "换用", "不换用", "", false, true, false, null, null, null) == 1)
                        {
                            ModBase.StartProcess(text, "--wait");
                            Logger.Info("为换用整合包中的启动器启动，强制结束程序", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                            ModMain.FrmMain.EndProgram(false);
                            return;
                        }
                    }
                    else
                    {
                        Logger.Info("未找到压缩包中附带的启动器", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                    }
                    string lastPart = PathUtils.GetLastPart(CS$<>8__locals1.$VB$Local_TargetFolder);
                    DirectoryUtils.Create(CS$<>8__locals1.$VB$Local_TargetFolder + ".minecraft\\");
                    PageSelectLeft.AddFolder(CS$<>8__locals1.$VB$Local_TargetFolder + ".minecraft\\" + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder.Replace("/", "\\").TrimStart(new char[] { '\\' }), lastPart, false);
                    string text3 = DirectoryUtils.EnumerateFiles(CS$<>8__locals1.$VB$Local_TargetFolder, true, "modpack.*").First<string>();
                    Logger.Info(string.Format("调用 modpack 文件继续安装：{0}", text3), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                    ModModpack.ModpackInstall(text3, null, null);
                }, null, ThreadPriority.Normal)
            });
            loaderCombo.Start(CS$<>8__locals1.$VB$Local_TargetFolder, false);
            ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
            ModMain.FrmMain.BtnExtraDownload.Ribble();
            return loaderCombo;
        }

        private static ModLoader.LoaderCombo<string> InstallPackCompress(string FileAddress, ZipArchive Archive)
        {
            ModModpack._Closure$__11-0 CS$<>8__locals1 = new ModModpack._Closure$__11-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_FileAddress = FileAddress;
            Match match = null;
            Regex regex = new Regex("^.*\\/(?=versions\\/(?<ver>[^\\/]+)\\/(\\k<ver>)\\.json$)", RegexOptions.IgnoreCase);
            try
            {
                foreach (ZipArchiveEntry zipArchiveEntry in Archive.Entries)
                {
                    Match match2 = regex.Match("/" + zipArchiveEntry.FullName);
                    if (match2.Success)
                    {
                        match = match2;
                        break;
                    }
                }
            }
            finally
            {
                IEnumerator<ZipArchiveEntry> enumerator;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            if (match == null)
            {
                throw new Exception("文件结构不匹配，这可能不是 Minecraft 客户端压缩包？");
            }
            CS$<>8__locals1.$VB$Local_ArchiveBaseFolder = match.Value.Replace("/", "\\").TrimStart(new char[] { '\\' });
            Logger.Info(string.Format("检测到压缩包的 .minecraft 根目录：{0}，命中的版本名：{1}", CS$<>8__locals1.$VB$Local_ArchiveBaseFolder, match.Groups[1].Value), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
            ModMain.MyMsgBox("接下来请选择一个空文件夹，它会被安装到这个文件夹里。", "安装", "继续", "", "", false, true, true, null, null, null);
            CS$<>8__locals1.$VB$Local_TargetFolder = Dialogs.SelectFolder("选择安装目标（必须是一个空文件夹）", false, null).FirstOrDefault<string>();
            if (CS$<>8__locals1.$VB$Local_TargetFolder == null)
            {
                throw new OperationCanceledException();
            }
            if (CS$<>8__locals1.$VB$Local_TargetFolder.Contains("!") || CS$<>8__locals1.$VB$Local_TargetFolder.Contains(";"))
            {
                ModMain.Hint("Minecraft 文件夹路径中不能含有感叹号或分号！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            if (!DirectoryUtils.IsEmpty(CS$<>8__locals1.$VB$Local_TargetFolder))
            {
                ModMain.Hint("请选择一个空文件夹作为安装目标！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>("解压压缩包", new ModLoader.LoaderBase[]
            {
                new ModLoader.LoaderTask<string, int>("解压压缩包", delegate(ModLoader.LoaderTask<string, int> Task)
                {
                    ModModpack.ExtractModpackFiles(CS$<>8__locals1.$VB$Local_TargetFolder, CS$<>8__locals1.$VB$Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.95));
                    PageSelectLeft.AddFolder(CS$<>8__locals1.$VB$Local_TargetFolder + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder, PathUtils.GetLastPart(CS$<>8__locals1.$VB$Local_TargetFolder), false);
                    Thread.Sleep(400);
                    ModBase.RunInUi((ModModpack._Closure$__.$I11-1 == null) ? (ModModpack._Closure$__.$I11-1 = delegate
                    {
                        ModMain.FrmMain.PageChange(FormMain.PageType.InstanceSelect, FormMain.PageSubType.Default);
                    }) : ModModpack._Closure$__.$I11-1, false);
                }, null, ThreadPriority.Normal)
            });
            loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.McInstallState);
            loaderCombo.Start(CS$<>8__locals1.$VB$Local_TargetFolder, false);
            ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
            ModMain.FrmMain.BtnExtraDownload.Ribble();
            return loaderCombo;
        }
    }
}
