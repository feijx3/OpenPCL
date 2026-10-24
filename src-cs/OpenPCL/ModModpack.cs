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
    public static class ModModpack
    {
        public static void ModpackInstall()
        {
            string file = Dialogs.SelectFile("选择整合包压缩包", false, null, new OneOf<IEnumerable<ValueTuple<string[], string>>, string>?("整合包文件(*.zip;*.rar;*.mrpack)|*.zip;*.rar;*.mrpack")).FirstOrDefault<string>();
            if (file == null)
            {
                return;
            }
            ModBase.RunInNewThread(new Action(delegate
            {
                try
                {
                    ModModpack.ModpackInstall(file, null, null);
                }
                catch (Exception ex)
                {
                    if (ex.IsCanceled())
                    {
                        Logger.Info("手动安装整合包已取消", LogBehavior.None);
                    }
                    else
                    {
                        Logger.Error(ex, "手动安装整合包失败", LogBehavior.Alert);
                    }
                }
            }), "Modpack Install", ThreadPriority.Normal);
        }

        // 0 CurseForge, 1 HMCL, 2 MultiMC/Prism, 3 MCBBS, 4 Modrinth, 5 带启动器的整合包, -1 未识别
        private static int DetectPackType(ZipArchive Archive, string Prefix)
        {
            if (Archive.GetEntry(Prefix + "mcbbs.packmeta") != null)
            {
                return 3;
            }
            if (Archive.GetEntry(Prefix + "mmc-pack.json") != null)
            {
                return 2;
            }
            if (Archive.GetEntry(Prefix + "modrinth.index.json") != null)
            {
                return 4;
            }
            ZipArchiveEntry entry = Archive.GetEntry(Prefix + "manifest.json");
            if (entry != null)
            {
                return (((JObject)entry.Open().ReadString(Encoding.UTF8).DeserializeJson())["addons"] == null) ? 0 : 3;
            }
            if (Archive.GetEntry(Prefix + "modpack.json") != null)
            {
                return 1;
            }
            if (Archive.GetEntry(Prefix + "modpack.zip") != null || Archive.GetEntry(Prefix + "modpack.mrpack") != null)
            {
                return 5;
            }
            return -1;
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
                int packType;
                string baseFolder = "";
                try
                {
                    zipArchive = FileUtils.OpenZip(File, default(CancellationToken));
                    packType = ModModpack.DetectPackType(zipArchive, "");
                    if (packType == -1)
                    {
                        string[] markers = new string[] { "mcbbs.packmeta", "mmc-pack.json", "modrinth.index.json", "manifest.json", "modpack.json", "modpack.zip", "modpack.mrpack" };
                        foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries)
                        {
                            string[] array = zipArchiveEntry.FullName.Split('/');
                            if (array.Length == 2 && markers.Contains(array[1]))
                            {
                                baseFolder = array[0] + "/";
                                packType = ModModpack.DetectPackType(zipArchive, baseFolder);
                                break;
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
                switch (packType)
                {
                case 0:
                    return ModModpack.InstallPackCurseForge(File, zipArchive, baseFolder, InstanceName, Logo);
                case 1:
                    return ModModpack.InstallPackHMCL(File, zipArchive, baseFolder);
                case 2:
                    return ModModpack.InstallPackMMC(File, zipArchive, baseFolder);
                case 3:
                    return ModModpack.InstallPackMCBBS(File, zipArchive, baseFolder, InstanceName);
                case 4:
                    return ModModpack.InstallPackModrinth(File, zipArchive, baseFolder, InstanceName, Logo);
                case 5:
                    return ModModpack.InstallPackLauncherPack(File, zipArchive, baseFolder);
                default:
                    return ModModpack.InstallPackCompress(File, zipArchive);
                }
            }
            finally
            {
                if (zipArchive != null)
                {
                    zipArchive.Dispose();
                }
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
            if (ModMain.OurMsgBox(string.Format("整合包中存在不兼容的加载器：{0}{1}如果你知道如何手动安装它，可以先选择跳过，然后在整合包下载结束后手动安装。", LoaderName, "\r\n"), "不兼容的加载器", "取消", string.Format("不安装 {0} 并继续", LoaderName), "", false, true, false, null, null, null) == 1)
            {
                throw new OperationCanceledException();
            }
        }

        private static ModLoader.LoaderCombo<string> InstallPackCurseForge(string FileAddress, ZipArchive Archive, string ArchiveBaseFolder, string InstanceName = null, string Logo = null)
        {
            ModModpack._Closure___5_0 CS_locals1 = new ModModpack._Closure___5_0(null);
            CS_locals1._VB_Local_FileAddress = FileAddress;
            CS_locals1._VB_Local_Logo = Logo;
            try
            {
                CS_locals1._VB_Local_Json = (JObject)Archive.GetEntry(ArchiveBaseFolder + "manifest.json").Open().ReadString(null)
                    .DeserializeJson();
            }
            catch (Exception ex)
            {
                throw new Exception("CurseForge 整合包安装信息存在问题", ex);
            }
            if (CS_locals1._VB_Local_Json["minecraft"] == null || CS_locals1._VB_Local_Json["minecraft"]["version"] == null)
            {
                throw new Exception("CurseForge 整合包未提供 Minecraft 版本信息");
            }
            if (InstanceName == null)
            {
                InstanceName = (string)(CS_locals1._VB_Local_Json["name"] ?? "");
                ValidateFolderName validateFolderName = new ValidateFolderName(ModMinecraft.McFolderSelected + "versions", true, null);
                if (Operators.CompareString(validateFolderName.Check(InstanceName), "", false) != 0)
                {
                    InstanceName = "";
                }
                if (Operators.CompareString(InstanceName, "", false) == 0)
                {
                    InstanceName = ModMain.OurMsgBoxInput("输入版本名称", "", "", new Collection<Validate> { validateFolderName }, "", "确定", "取消", false);
                }
                if (string.IsNullOrEmpty(InstanceName))
                {
                    throw new OperationCanceledException();
                }
            }
            CS_locals1._VB_Local_VersionFolder = PathUtils.AddSlashSuffix(Path.Combine(ModMinecraft.McFolderSelected, "versions", InstanceName) + "\\");
            CS_locals1._VB_Local_TargetVersion = new McVersion(CS_locals1._VB_Local_Json["minecraft"]["version"].ToString(), true);
            try
            {
                foreach (JToken jtoken in ((IEnumerable<JToken>)(CS_locals1._VB_Local_Json["minecraft"]["modLoaders"] ?? new byte[0])))
                {
                    string text = (jtoken["id"] ?? "").ToString().Lower();
                    if (text.StartsWithF("forge-", false))
                    {
                        if (text.Contains("recommended"))
                        {
                            throw new Exception("该整合包版本过老，已不支持进行安装！");
                        }
                        Logger.Info(string.Format("整合包 Forge 版本：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        CS_locals1._VB_Local_TargetVersion.ModLoaderVersions[ModLoaders.Forge] = text.Replace("forge-", "");
                    }
                    else if (text.StartsWithF("neoforge-", false))
                    {
                        Logger.Info(string.Format("整合包 NeoForge 版本：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        CS_locals1._VB_Local_TargetVersion.ModLoaderVersions[ModLoaders.NeoForge] = text.Replace("neoforge-", "");
                    }
                    else if (text.StartsWithF("fabric-", false))
                    {
                        Logger.Info(string.Format("整合包 Fabric 版本：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        CS_locals1._VB_Local_TargetVersion.ModLoaderVersions[ModLoaders.Fabric] = text.Replace("fabric-", "");
                    }
                    else
                    {
                        ModModpack.NotifyIncompatibleLoader(text);
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
            CS_locals1._VB_Local_InstallTemp = ModMain.RequestTaskTempFolder(false);
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            string text2 = (string)(CS_locals1._VB_Local_Json["overrides"] ?? "");
            if (Operators.CompareString(text2, "", false) != 0)
            {
                ModModpack._Closure___5_1 CS_locals2 = new ModModpack._Closure___5_1(null);
                CS_locals2._VB_NonLocal__VB_Closure_2 = CS_locals1;
                CS_locals2._VB_Local_OverridesFolder = CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_InstallTemp + ArchiveBaseFolder + ((Operators.CompareString(text2, ".", false) == 0 || Operators.CompareString(text2, "./", false) == 0) ? "" : text2);
                if (!PathUtils.IsParentOf(CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_InstallTemp, CS_locals2._VB_Local_OverridesFolder))
                {
                    throw new Exception("整合包的 overrides 路径超出了解压目录，请向整合包作者反馈此问题！");
                }
                list.Add(new ModLoader.LoaderTask<string, int>("解压整合包文件", new Action<ModLoader.LoaderTask<string, int>>(CS_locals2._Lambda___0), null, ThreadPriority.Normal)
                {
                    ProgressWeight = (double)FileUtils.GetInfo(CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_FileAddress).Length / 1024.0 / 1024.0 / 6.0,
                    Block = false
                });
            }
            CS_locals1._VB_Local_ModList = new List<int>();
            CS_locals1._VB_Local_ModOptionalList = new List<int>();
            try
            {
                foreach (JToken jtoken2 in ((IEnumerable<JToken>)(CS_locals1._VB_Local_Json["files"] ?? new byte[0])))
                {
                    if (jtoken2["projectID"] == null || jtoken2["fileID"] == null)
                    {
                        ModMain.Hint("某项 Mod 缺少必要信息，已跳过：" + jtoken2.ToString(), ModMain.HintType.Blue, true);
                    }
                    else
                    {
                        CS_locals1._VB_Local_ModList.Add((int)jtoken2["fileID"]);
                        if (jtoken2["required"] != null && !jtoken2["required"].ToObject<bool>())
                        {
                            CS_locals1._VB_Local_ModOptionalList.Add((int)jtoken2["fileID"]);
                        }
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
            if (CS_locals1._VB_Local_ModList.Any<int>())
            {
                List<ModLoader.LoaderBase> list2 = new List<ModLoader.LoaderBase>();
                list2.Add(new ModLoader.LoaderTask<int, JArray>("获取 Mod 下载信息", new Action<ModLoader.LoaderTask<int, JArray>>(CS_locals1._Lambda___1), null, ThreadPriority.Normal)
                {
                    ProgressWeight = (double)CS_locals1._VB_Local_ModList.Count / 10.0
                });
                list2.Add(new ModLoader.LoaderTask<JArray, List<ModNet.NetFile>>("构造 Mod 下载信息", new Action<ModLoader.LoaderTask<JArray, List<ModNet.NetFile>>>(CS_locals1._Lambda___7), null, ThreadPriority.Normal)
                {
                    ProgressWeight = (double)CS_locals1._VB_Local_ModList.Count / 200.0,
                    Show = false
                });
                list2.Add(new ModNet.LoaderDownload("下载 Mod", new List<ModNet.NetFile>())
                {
                    ProgressWeight = (double)CS_locals1._VB_Local_ModList.Count * 1.5
                });
                list.Add(new ModLoader.LoaderCombo<int>("下载 Mod（主加载器）", list2)
                {
                    Show = false,
                    ProgressWeight = list2.Sum<ModLoader.LoaderBase>((ModModpack._Closure___._I5_9 == null) ? (ModModpack._Closure___._I5_9 = new Func<ModLoader.LoaderBase, double>(ModModpack._Closure___._I._Lambda___5_9)) : ModModpack._Closure___._I5_9)
                });
            }
            McComponents mcComponents = new McComponents(CS_locals1._VB_Local_TargetVersion.VanillaName, CS_locals1._VB_Local_TargetVersion.ModLoaderVersions, null, null, null);
            List<ModLoader.LoaderBase> list3 = ModDownloadLib.McInstallLoadersGet(new ModDownloadLib.McInstallRequest
            {
                NewInstanceName = InstanceName,
                InstanceFolder = CS_locals1._VB_Local_VersionFolder,
                Components = mcComponents
            });
            List<ModLoader.LoaderBase> list4 = new List<ModLoader.LoaderBase>();
            list4.Add(new ModLoader.LoaderCombo<string>("整合包安装", list)
            {
                Show = false,
                Block = false,
                ProgressWeight = list.Sum<ModLoader.LoaderBase>((ModModpack._Closure___._I5_10 == null) ? (ModModpack._Closure___._I5_10 = new Func<ModLoader.LoaderBase, double>(ModModpack._Closure___._I._Lambda___5_10)) : ModModpack._Closure___._I5_10)
            });
            list4.Add(new ModLoader.LoaderCombo<string>("游戏安装", list3)
            {
                Show = false,
                ProgressWeight = list3.Sum<ModLoader.LoaderBase>((ModModpack._Closure___._I5_11 == null) ? (ModModpack._Closure___._I5_11 = new Func<ModLoader.LoaderBase, double>(ModModpack._Closure___._I._Lambda___5_11)) : ModModpack._Closure___._I5_11)
            });
            list4.Add(new ModLoader.LoaderTask<string, string>("最终整理文件", new Action<ModLoader.LoaderTask<string, string>>(CS_locals1._Lambda___12), null, ThreadPriority.Normal)
            {
                ProgressWeight = 0.1,
                Show = false
            });
            CS_locals1._VB_Local_LoaderName = "CurseForge 整合包安装：" + InstanceName + " ";
            if (ModLoader.LoaderTaskbar.Any<ModLoader.LoaderBase>(new Func<ModLoader.LoaderBase, bool>(CS_locals1._Lambda___13)))
            {
                ModMain.Hint("该整合包正在安装中！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>(CS_locals1._VB_Local_LoaderName, list4);
            loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.McInstallState);
            loaderCombo.Start(CS_locals1._VB_Local_VersionFolder, false);
            ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
            return loaderCombo;
        }

        private static ModLoader.LoaderCombo<string> InstallPackModrinth(string FileAddress, ZipArchive Archive, string ArchiveBaseFolder, string InstanceName = null, string Logo = null)
        {
            ModModpack._Closure___6_0 CS_locals1 = new ModModpack._Closure___6_0(null);
            CS_locals1._VB_Local_FileAddress = FileAddress;
            CS_locals1._VB_Local_ArchiveBaseFolder = ArchiveBaseFolder;
            CS_locals1._VB_Local_Logo = Logo;
            JObject jobject;
            try
            {
                jobject = (JObject)Archive.GetEntry(CS_locals1._VB_Local_ArchiveBaseFolder + "modrinth.index.json").Open().ReadString(null)
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
                foreach (JToken jtoken in ((IEnumerable<JToken>)(jobject["dependencies"] ?? new byte[0])))
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
                IEnumerator<JToken> enumerator = default;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            if (InstanceName == null)
            {
                InstanceName = (string)(jobject["name"] ?? "");
                ValidateFolderName validateFolderName = new ValidateFolderName(ModMinecraft.McFolderSelected + "versions", true, null);
                if (Operators.CompareString(validateFolderName.Check(InstanceName), "", false) != 0)
                {
                    InstanceName = "";
                }
                if (Operators.CompareString(InstanceName, "", false) == 0)
                {
                    InstanceName = ModMain.OurMsgBoxInput("输入版本名称", "", "", new Collection<Validate> { validateFolderName }, "", "确定", "取消", false);
                }
                if (string.IsNullOrEmpty(InstanceName))
                {
                    throw new OperationCanceledException();
                }
            }
            CS_locals1._VB_Local_VersionFolder = Path.Combine(ModMinecraft.McFolderSelected, "versions", InstanceName) + "\\";
            CS_locals1._VB_Local_InstallTemp = ModMain.RequestTaskTempFolder(false);
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            list.Add(new ModLoader.LoaderTask<string, int>("解压整合包文件", new Action<ModLoader.LoaderTask<string, int>>(CS_locals1._Lambda___0), null, ThreadPriority.Normal)
            {
                ProgressWeight = (double)FileUtils.GetInfo(CS_locals1._VB_Local_FileAddress).Length / 1024.0 / 1024.0 / 6.0,
                Block = false
            });
            List<ModNet.NetFile> list2 = new List<ModNet.NetFile>();
            try
            {
                foreach (JToken jtoken2 in ((IEnumerable<JToken>)(jobject["files"] ?? new byte[0])))
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
                        else if (ModMain.OurMsgBox("是否要下载可选文件 " + PathUtils.GetLastPart(jtoken2["path"].ToString()) + "？", "下载可选文件", "是", "否", "", false, true, false, null, null, null) == 2)
                        {
                            continue;
                        }
                    }
                    List<string> list3 = jtoken2["downloads"].SelectMany<JToken, string>((ModModpack._Closure___._I6_1 == null) ? (ModModpack._Closure___._I6_1 = new Func<JToken, IEnumerable<string>>(ModModpack._Closure___._I._Lambda___6_1)) : ModModpack._Closure___._I6_1).ToList<string>();
                    list3.AddRange(list3.Select<string, string>((ModModpack._Closure___._I6_2 == null) ? (ModModpack._Closure___._I6_2 = new Func<string, string>(ModModpack._Closure___._I._Lambda___6_2)) : ModModpack._Closure___._I6_2).ToList<string>());
                    list3 = ResourceVersion.ParseModrinthTrackArguments(list3, new ResourceVersion.DownloadReason?(ResourceVersion.DownloadReason.ModPack), mcVersion.VanillaName, mcVersion.ModLoaders).ToList<string>();
                    list3 = list3.Distinct<string>().ToList<string>();
                    string text3 = Path.Combine(CS_locals1._VB_Local_VersionFolder, jtoken2["path"].ToString().TrimStart(new char[]
                    {
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar
                    }));
                    if (!PathUtils.IsParentOf(CS_locals1._VB_Local_VersionFolder, text3))
                    {
                        ModMain.OurMsgBox(string.Format("整合包的文件路径超出了版本文件夹，请向整合包作者反馈此问题！{0}目标：{1}{2}错误的文件：{3}", new object[]
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
                IEnumerator<JToken> enumerator2 = default;
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
                InstanceFolder = CS_locals1._VB_Local_VersionFolder,
                Components = mcComponents
            });
            List<ModLoader.LoaderBase> list5 = new List<ModLoader.LoaderBase>();
            list5.Add(new ModLoader.LoaderCombo<string>("整合包安装", list)
            {
                Show = false,
                Block = false,
                ProgressWeight = list.Sum<ModLoader.LoaderBase>((ModModpack._Closure___._I6_3 == null) ? (ModModpack._Closure___._I6_3 = new Func<ModLoader.LoaderBase, double>(ModModpack._Closure___._I._Lambda___6_3)) : ModModpack._Closure___._I6_3)
            });
            list5.Add(new ModLoader.LoaderCombo<string>("游戏安装", list4)
            {
                Show = false,
                ProgressWeight = list4.Sum<ModLoader.LoaderBase>((ModModpack._Closure___._I6_4 == null) ? (ModModpack._Closure___._I6_4 = new Func<ModLoader.LoaderBase, double>(ModModpack._Closure___._I._Lambda___6_4)) : ModModpack._Closure___._I6_4)
            });
            list5.Add(new ModLoader.LoaderTask<string, string>("最终整理文件", new Action<ModLoader.LoaderTask<string, string>>(CS_locals1._Lambda___5), null, ThreadPriority.Normal)
            {
                ProgressWeight = 0.1,
                Show = false
            });
            CS_locals1._VB_Local_LoaderName = string.Format("Modrinth 整合包安装：{0} ", InstanceName);
            if (ModLoader.LoaderTaskbar.Any<ModLoader.LoaderBase>(new Func<ModLoader.LoaderBase, bool>(CS_locals1._Lambda___6)))
            {
                ModMain.Hint("该整合包正在安装中！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>(CS_locals1._VB_Local_LoaderName, list5);
            loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.McInstallState);
            loaderCombo.Start(CS_locals1._VB_Local_VersionFolder, false);
            ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
            return loaderCombo;
        }

        private static ModLoader.LoaderCombo<string> InstallPackHMCL(string FileAddress, ZipArchive Archive, string ArchiveBaseFolder)
        {
            ModModpack._Closure___7_0 CS_locals1 = new ModModpack._Closure___7_0();
            CS_locals1._VB_Local_FileAddress = FileAddress;
            CS_locals1._VB_Local_ArchiveBaseFolder = ArchiveBaseFolder;
            JObject jobject;
            try
            {
                jobject = (JObject)Archive.GetEntry(CS_locals1._VB_Local_ArchiveBaseFolder + "modpack.json").Open().ReadString(Encoding.UTF8)
                    .DeserializeJson();
            }
            catch (Exception ex)
            {
                throw new Exception("HMCL 整合包安装信息存在问题", ex);
            }
            string text = (string)(jobject["name"] ?? "");
            ValidateFolderName validateFolderName = new ValidateFolderName(ModMinecraft.McFolderSelected + "versions", true, null);
            if (Operators.CompareString(validateFolderName.Check(text), "", false) != 0)
            {
                text = "";
            }
            if (Operators.CompareString(text, "", false) == 0)
            {
                text = ModMain.OurMsgBoxInput("输入版本名称", "", "", new Collection<Validate> { validateFolderName }, "", "确定", "取消", false);
            }
            if (string.IsNullOrEmpty(text))
            {
                throw new OperationCanceledException();
            }
            CS_locals1._VB_Local_VersionFolder = Path.Combine(ModMinecraft.McFolderSelected, "versions", text) + "\\";
            CS_locals1._VB_Local_InstallTemp = ModMain.RequestTaskTempFolder(false);
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            list.Add(new ModLoader.LoaderTask<string, int>("解压整合包文件", new Action<ModLoader.LoaderTask<string, int>>(CS_locals1._Lambda___0), null, ThreadPriority.Normal)
            {
                ProgressWeight = (double)FileUtils.GetInfo(CS_locals1._VB_Local_FileAddress).Length / 1024.0 / 1024.0 / 6.0,
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
                InstanceFolder = CS_locals1._VB_Local_VersionFolder,
                Components = mcComponents
            });
            List<ModLoader.LoaderBase> list3 = new List<ModLoader.LoaderBase>
            {
                new ModLoader.LoaderCombo<string>("整合包安装", list)
                {
                    Show = false,
                    Block = false,
                    ProgressWeight = list.Sum<ModLoader.LoaderBase>((ModModpack._Closure___._I7_1 == null) ? (ModModpack._Closure___._I7_1 = new Func<ModLoader.LoaderBase, double>(ModModpack._Closure___._I._Lambda___7_1)) : ModModpack._Closure___._I7_1)
                },
                new ModLoader.LoaderCombo<string>("游戏安装", list2)
                {
                    Show = false,
                    ProgressWeight = list2.Sum<ModLoader.LoaderBase>((ModModpack._Closure___._I7_2 == null) ? (ModModpack._Closure___._I7_2 = new Func<ModLoader.LoaderBase, double>(ModModpack._Closure___._I._Lambda___7_2)) : ModModpack._Closure___._I7_2)
                }
            };
            CS_locals1._VB_Local_LoaderName = "HMCL 整合包安装：" + text + " ";
            if (ModLoader.LoaderTaskbar.Any<ModLoader.LoaderBase>(new Func<ModLoader.LoaderBase, bool>(CS_locals1._Lambda___3)))
            {
                ModMain.Hint("该整合包正在安装中！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>(CS_locals1._VB_Local_LoaderName, list3);
            loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.McInstallState);
            loaderCombo.Start(CS_locals1._VB_Local_VersionFolder, false);
            ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
            return loaderCombo;
        }

        private static ModLoader.LoaderCombo<string> InstallPackMMC(string FileAddress, ZipArchive Archive, string ArchiveBaseFolder)
        {
            ModModpack._Closure___8_0 CS_locals1 = new ModModpack._Closure___8_0(null);
            CS_locals1._VB_Local_FileAddress = FileAddress;
            CS_locals1._VB_Local_ArchiveBaseFolder = ArchiveBaseFolder;
            JObject jobject;
            string text;
            try
            {
                jobject = (JObject)Archive.GetEntry(CS_locals1._VB_Local_ArchiveBaseFolder + "mmc-pack.json").Open().ReadString(Encoding.UTF8)
                    .DeserializeJson();
                text = Archive.GetEntry(CS_locals1._VB_Local_ArchiveBaseFolder + "instance.cfg").Open().ReadString(Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception("MMC 整合包安装信息存在问题", ex);
            }
            string text2 = text.RegexSeek("(?<=\\nname\\=)[^\\n]+", RegexOptions.None) ?? "";
            ValidateFolderName validateFolderName = new ValidateFolderName(ModMinecraft.McFolderSelected + "versions", true, null);
            if (Operators.CompareString(validateFolderName.Check(text2), "", false) != 0)
            {
                text2 = "";
            }
            if (Operators.CompareString(text2, "", false) == 0)
            {
                text2 = ModMain.OurMsgBoxInput("输入版本名称", "", "", new Collection<Validate> { validateFolderName }, "", "确定", "取消", false);
            }
            if (string.IsNullOrEmpty(text2))
            {
                throw new OperationCanceledException();
            }
            CS_locals1._VB_Local_VersionFolder = Path.Combine(ModMinecraft.McFolderSelected, "versions", text2) + "\\";
            CS_locals1._VB_Local_InstallTemp = ModMain.RequestTaskTempFolder(false);
            CS_locals1._VB_Local_SetupFile = CS_locals1._VB_Local_VersionFolder + "PCL\\Setup.ini";
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            list.Add(new ModLoader.LoaderTask<string, int>("解压整合包文件", new Action<ModLoader.LoaderTask<string, int>>(CS_locals1._Lambda___0), null, ThreadPriority.Normal)
            {
                ProgressWeight = (double)FileUtils.GetInfo(CS_locals1._VB_Local_FileAddress).Length / 1024.0 / 1024.0 / 6.0,
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
                foreach (JToken jtoken in ((IEnumerable<JToken>)jobject["components"]))
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
                IEnumerator<JToken> enumerator = default;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            McComponents mcComponents = new McComponents(text3, dictionary, null, null, null);
            List<ModLoader.LoaderBase> list2 = ModDownloadLib.McInstallLoadersGet(new ModDownloadLib.McInstallRequest
            {
                NewInstanceName = text2,
                InstanceFolder = CS_locals1._VB_Local_VersionFolder,
                Components = mcComponents
            });
            List<ModLoader.LoaderBase> list3 = new List<ModLoader.LoaderBase>();
            list3.Add(new ModLoader.LoaderCombo<string>("整合包安装", list)
            {
                Show = false,
                Block = false,
                ProgressWeight = list.Sum<ModLoader.LoaderBase>((ModModpack._Closure___._I8_1 == null) ? (ModModpack._Closure___._I8_1 = new Func<ModLoader.LoaderBase, double>(ModModpack._Closure___._I._Lambda___8_1)) : ModModpack._Closure___._I8_1)
            });
            list3.Add(new ModLoader.LoaderCombo<string>("游戏安装", list2)
            {
                Show = false,
                ProgressWeight = list2.Sum<ModLoader.LoaderBase>((ModModpack._Closure___._I8_2 == null) ? (ModModpack._Closure___._I8_2 = new Func<ModLoader.LoaderBase, double>(ModModpack._Closure___._I._Lambda___8_2)) : ModModpack._Closure___._I8_2)
            });
            CS_locals1._VB_Local_LoaderName = "MMC 整合包安装：" + text2 + " ";
            if (ModLoader.LoaderTaskbar.Any<ModLoader.LoaderBase>(new Func<ModLoader.LoaderBase, bool>(CS_locals1._Lambda___3)))
            {
                ModMain.Hint("该整合包正在安装中！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>(CS_locals1._VB_Local_LoaderName, list3);
            loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.McInstallState);
            loaderCombo.Start(CS_locals1._VB_Local_VersionFolder, false);
            ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
            return loaderCombo;
        }

        private static ModLoader.LoaderCombo<string> InstallPackMCBBS(string FileAddress, ZipArchive Archive, string ArchiveBaseFolder, string InstanceName = null)
        {
            ModModpack._Closure___9_0 CS_locals1 = new ModModpack._Closure___9_0(null);
            CS_locals1._VB_Local_FileAddress = FileAddress;
            CS_locals1._VB_Local_ArchiveBaseFolder = ArchiveBaseFolder;
            try
            {
                ZipArchiveEntry zipArchiveEntry = Archive.GetEntry(CS_locals1._VB_Local_ArchiveBaseFolder + "mcbbs.packmeta") ?? Archive.GetEntry(CS_locals1._VB_Local_ArchiveBaseFolder + "manifest.json");
                CS_locals1._VB_Local_Json = (JObject)zipArchiveEntry.Open().ReadString(Encoding.UTF8).DeserializeJson();
            }
            catch (Exception ex)
            {
                throw new Exception("MCBBS 整合包安装信息存在问题", ex);
            }
            if (InstanceName == null)
            {
                InstanceName = (string)(CS_locals1._VB_Local_Json["name"] ?? "");
                ValidateFolderName validateFolderName = new ValidateFolderName(ModMinecraft.McFolderSelected + "versions", true, null);
                if (Operators.CompareString(validateFolderName.Check(InstanceName), "", false) != 0)
                {
                    InstanceName = "";
                }
                if (Operators.CompareString(InstanceName, "", false) == 0)
                {
                    InstanceName = ModMain.OurMsgBoxInput("输入版本名称", "", "", new Collection<Validate> { validateFolderName }, "", "确定", "取消", false);
                }
                if (string.IsNullOrEmpty(InstanceName))
                {
                    throw new OperationCanceledException();
                }
            }
            CS_locals1._VB_Local_VersionFolder = Path.Combine(ModMinecraft.McFolderSelected, "versions", InstanceName) + "\\";
            CS_locals1._VB_Local_InstallTemp = ModMain.RequestTaskTempFolder(false);
            CS_locals1._VB_Local_SetupFile = CS_locals1._VB_Local_VersionFolder + "PCL\\Setup.ini";
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            list.Add(new ModLoader.LoaderTask<string, int>("解压整合包文件", new Action<ModLoader.LoaderTask<string, int>>(CS_locals1._Lambda___0), null, ThreadPriority.Normal)
            {
                ProgressWeight = (double)FileUtils.GetInfo(CS_locals1._VB_Local_FileAddress).Length / 1024.0 / 1024.0 / 6.0,
                Block = false
            });
            if (CS_locals1._VB_Local_Json["addons"] == null)
            {
                throw new Exception("该 MCBBS 整合包未提供游戏版本附加信息，无法安装！");
            }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                foreach (JToken jtoken in ((IEnumerable<JToken>)CS_locals1._VB_Local_Json["addons"]))
                {
                    dictionary.Add((string)jtoken["id"], (string)jtoken["version"]);
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
                ModModpack._Closure___9_1 CS_locals2 = new ModModpack._Closure___9_1(null);
                CS_locals2._VB_Local_MinecraftVersion = dictionary["game"];
                if (CS_locals2._VB_Local_MinecraftVersion.EndsWithF(".0", false))
                {
                    CS_locals2._VB_Local_MinecraftVersion = CS_locals2._VB_Local_MinecraftVersion.Substring(0, checked(CS_locals2._VB_Local_MinecraftVersion.Length - 2));
                }
                CS_locals2._VB_Local_OptiFineFileSuffix = "_HD_" + dictionary["optifine"].AfterLast("HD_", false) + ".jar";
                if (ModDownload.DlOptiFineListBmclapiLoader.State != ModBase.LoadState.Finished)
                {
                    ModDownload.DlOptiFineListBmclapiLoader.WaitForExit(null, null, false, default(CancellationToken), null);
                }
                ModDownload.DlOptiFineListEntry dlOptiFineListEntry = ModDownload.DlOptiFineListBmclapiLoader.Output.Value.FirstOrDefault<ModDownload.DlOptiFineListEntry>(new Func<ModDownload.DlOptiFineListEntry, bool>(CS_locals2._Lambda___1));
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
                InstanceFolder = CS_locals1._VB_Local_VersionFolder,
                Components = mcComponents
            });
            List<ModLoader.LoaderBase> list3 = new List<ModLoader.LoaderBase>();
            list3.Add(new ModLoader.LoaderCombo<string>("整合包安装", list)
            {
                Show = false,
                Block = false,
                ProgressWeight = list.Sum<ModLoader.LoaderBase>((ModModpack._Closure___._I9_2 == null) ? (ModModpack._Closure___._I9_2 = new Func<ModLoader.LoaderBase, double>(ModModpack._Closure___._I._Lambda___9_2)) : ModModpack._Closure___._I9_2)
            });
            list3.Add(new ModLoader.LoaderCombo<string>("游戏安装", list2)
            {
                Show = false,
                ProgressWeight = list2.Sum<ModLoader.LoaderBase>((ModModpack._Closure___._I9_3 == null) ? (ModModpack._Closure___._I9_3 = new Func<ModLoader.LoaderBase, double>(ModModpack._Closure___._I._Lambda___9_3)) : ModModpack._Closure___._I9_3)
            });
            CS_locals1._VB_Local_LoaderName = "MCBBS 整合包安装：" + InstanceName + " ";
            if (ModLoader.LoaderTaskbar.Any<ModLoader.LoaderBase>(new Func<ModLoader.LoaderBase, bool>(CS_locals1._Lambda___4)))
            {
                ModMain.Hint("该整合包正在安装中！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>(CS_locals1._VB_Local_LoaderName, list3);
            loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.McInstallState);
            loaderCombo.Start(CS_locals1._VB_Local_VersionFolder, false);
            ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
            return loaderCombo;
        }

        private static ModLoader.LoaderCombo<string> InstallPackLauncherPack(string FileAddress, ZipArchive Archive, string ArchiveBaseFolder)
        {
            ModModpack._Closure___10_0 CS_locals1 = new ModModpack._Closure___10_0(null);
            CS_locals1._VB_Local_FileAddress = FileAddress;
            CS_locals1._VB_Local_ArchiveBaseFolder = ArchiveBaseFolder;
            ModMain.OurMsgBox("接下来请选择一个空文件夹，它会被安装到这个文件夹里。", "安装", "继续", "", "", false, true, true, null, null, null);
            CS_locals1._VB_Local_TargetFolder = Dialogs.SelectFolder("选择安装目标（必须是一个空文件夹）", false, null).FirstOrDefault<string>();
            if (CS_locals1._VB_Local_TargetFolder == null)
            {
                throw new OperationCanceledException();
            }
            if (!DirectoryUtils.IsEmpty(CS_locals1._VB_Local_TargetFolder))
            {
                ModMain.Hint("请选择一个空文件夹作为安装目标！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>("解压压缩包", new ModLoader.LoaderBase[]
            {
                new ModLoader.LoaderTask<string, int>("解压压缩包", new Action<ModLoader.LoaderTask<string, int>>(CS_locals1._Lambda___0), null, ThreadPriority.Normal)
            });
            loaderCombo.Start(CS_locals1._VB_Local_TargetFolder, false);
            ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
            ModMain.FrmMain.BtnExtraDownload.Ribble();
            return loaderCombo;
        }

        private static ModLoader.LoaderCombo<string> InstallPackCompress(string FileAddress, ZipArchive Archive)
        {
            ModModpack._Closure___11_0 CS_locals1 = new ModModpack._Closure___11_0(null);
            CS_locals1._VB_Local_FileAddress = FileAddress;
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
                IEnumerator<ZipArchiveEntry> enumerator = default;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            if (match == null)
            {
                throw new Exception("文件结构不匹配，这可能不是 Minecraft 客户端压缩包？");
            }
            CS_locals1._VB_Local_ArchiveBaseFolder = match.Value.Replace("/", "\\").TrimStart(new char[] { '\\' });
            Logger.Info(string.Format("检测到压缩包的 .minecraft 根目录：{0}，命中的版本名：{1}", CS_locals1._VB_Local_ArchiveBaseFolder, match.Groups[1].Value), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
            ModMain.OurMsgBox("接下来请选择一个空文件夹，它会被安装到这个文件夹里。", "安装", "继续", "", "", false, true, true, null, null, null);
            CS_locals1._VB_Local_TargetFolder = Dialogs.SelectFolder("选择安装目标（必须是一个空文件夹）", false, null).FirstOrDefault<string>();
            if (CS_locals1._VB_Local_TargetFolder == null)
            {
                throw new OperationCanceledException();
            }
            if (CS_locals1._VB_Local_TargetFolder.Contains("!") || CS_locals1._VB_Local_TargetFolder.Contains(";"))
            {
                ModMain.Hint("Minecraft 文件夹路径中不能含有感叹号或分号！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            if (!DirectoryUtils.IsEmpty(CS_locals1._VB_Local_TargetFolder))
            {
                ModMain.Hint("请选择一个空文件夹作为安装目标！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>("解压压缩包", new ModLoader.LoaderBase[]
            {
                new ModLoader.LoaderTask<string, int>("解压压缩包", new Action<ModLoader.LoaderTask<string, int>>(CS_locals1._Lambda___0), null, ThreadPriority.Normal)
            });
            loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.McInstallState);
            loaderCombo.Start(CS_locals1._VB_Local_TargetFolder, false);
            ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
            ModMain.FrmMain.BtnExtraDownload.Ribble();
            return loaderCombo;
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

            internal int _Lambda___5_2(JToken j)
            {
                return j["id"].ToObject<int>();
            }

            internal string _Lambda___5_5(ValueTuple<int, int> e)
            {
                return e.Item1.ToString();
            }

            internal string _Lambda___5_8(JToken l)
            {
                return l["name"].ToString();
            }

            internal double _Lambda___5_9(ModLoader.LoaderBase l)
            {
                return l.ProgressWeight;
            }

            internal double _Lambda___5_10(ModLoader.LoaderBase l)
            {
                return l.ProgressWeight;
            }

            internal double _Lambda___5_11(ModLoader.LoaderBase l)
            {
                return l.ProgressWeight;
            }

            internal IEnumerable<string> _Lambda___6_1(JToken t)
            {
                return ResourceVersion.ParseCurseForgeDownloadUrls(t.ToString());
            }

            internal string _Lambda___6_2(string u)
            {
                return ModDownload.DlSourceModGet(u);
            }

            internal double _Lambda___6_3(ModLoader.LoaderBase l)
            {
                return l.ProgressWeight;
            }

            internal double _Lambda___6_4(ModLoader.LoaderBase l)
            {
                return l.ProgressWeight;
            }

            internal double _Lambda___7_1(ModLoader.LoaderBase l)
            {
                return l.ProgressWeight;
            }

            internal double _Lambda___7_2(ModLoader.LoaderBase l)
            {
                return l.ProgressWeight;
            }

            internal double _Lambda___8_1(ModLoader.LoaderBase l)
            {
                return l.ProgressWeight;
            }

            internal double _Lambda___8_2(ModLoader.LoaderBase l)
            {
                return l.ProgressWeight;
            }

            internal double _Lambda___9_2(ModLoader.LoaderBase l)
            {
                return l.ProgressWeight;
            }

            internal double _Lambda___9_3(ModLoader.LoaderBase l)
            {
                return l.ProgressWeight;
            }

            internal void _Lambda___11_1()
            {
                ModMain.FrmMain.PageChange(FormMain.PageType.InstanceSelect, FormMain.PageSubType.Default);
            }

            public static readonly ModModpack._Closure___ _I = new ModModpack._Closure___();

            public static Func<JToken, int> _I5_2;

            public static Func<ValueTuple<int, int>, string> _I5_5;

            public static Func<JToken, string> _I5_8;

            public static Func<ModLoader.LoaderBase, double> _I5_9;

            public static Func<ModLoader.LoaderBase, double> _I5_10;

            public static Func<ModLoader.LoaderBase, double> _I5_11;

            public static Func<JToken, IEnumerable<string>> _I6_1;

            public static Func<string, string> _I6_2;

            public static Func<ModLoader.LoaderBase, double> _I6_3;

            public static Func<ModLoader.LoaderBase, double> _I6_4;

            public static Func<ModLoader.LoaderBase, double> _I7_1;

            public static Func<ModLoader.LoaderBase, double> _I7_2;

            public static Func<ModLoader.LoaderBase, double> _I8_1;

            public static Func<ModLoader.LoaderBase, double> _I8_2;

            public static Func<ModLoader.LoaderBase, double> _I9_2;

            public static Func<ModLoader.LoaderBase, double> _I9_3;

            public static Action _I11_1;
        }

        [CompilerGenerated]
        private sealed class _Closure___0_0
        {
            public _Closure___0_0()
            {
            }

            internal void _Lambda___0()
            {
                try
                {
                    ModModpack.ModpackInstall(this._VB_Local_File, null, null);
                }
                catch (Exception ex)
                {
                    ex.ThrowIfCanceled();
                    Logger.Error(ex, "手动安装整合包失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                }
            }

            public string _VB_Local_File;
        }

        [CompilerGenerated]
        private sealed class _Closure___10_0
        {
            public _Closure___10_0(ModModpack._Closure___10_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_TargetFolder = arg0._VB_Local_TargetFolder;
                    this._VB_Local_FileAddress = arg0._VB_Local_FileAddress;
                    this._VB_Local_ArchiveBaseFolder = arg0._VB_Local_ArchiveBaseFolder;
                }
            }

            internal void _Lambda___0(ModLoader.LoaderTask<string, int> Task)
            {
                ModModpack.ExtractModpackFiles(this._VB_Local_TargetFolder, this._VB_Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.9));
                ModBase.OpenExplorer(this._VB_Local_TargetFolder);
                Thread.Sleep(400);
                string text = null;
                try
                {
                    foreach (string text2 in DirectoryUtils.EnumerateFiles(this._VB_Local_TargetFolder, false, "*.exe"))
                    {
                        FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(text2);
                        Logger.Info(string.Format("文件 {0} 的产品名标识为 {1}", text2, versionInfo.ProductName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        if (Operators.CompareString(versionInfo.ProductName, "Plain Craft Launcher", false) == 0 || Operators.CompareString(versionInfo.ProductName, "OpenPCL", false) == 0)
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
                    IEnumerator<string> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                Task.Progress = 0.95;
                if (text != null)
                {
                    Logger.Info(string.Format("找到压缩包中附带的启动器：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                    if (ModMain.OurMsgBox(string.Format("整合包里似乎自带了启动器，是否换用它继续安装？{0}即将打开：{1}", "\r\n", text), "换用整合包启动器？", "换用", "不换用", "", false, true, false, null, null, null) == 1)
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
                string lastPart = PathUtils.GetLastPart(this._VB_Local_TargetFolder);
                DirectoryUtils.Create(this._VB_Local_TargetFolder + ".minecraft\\");
                PageSelectLeft.AddFolder(this._VB_Local_TargetFolder + ".minecraft\\" + this._VB_Local_ArchiveBaseFolder.Replace("/", "\\").TrimStart(new char[] { '\\' }), lastPart, false);
                string text3 = DirectoryUtils.EnumerateFiles(this._VB_Local_TargetFolder, true, "modpack.*").First<string>();
                Logger.Info(string.Format("调用 modpack 文件继续安装：{0}", text3), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                ModModpack.ModpackInstall(text3, null, null);
            }

            public string _VB_Local_TargetFolder;

            public string _VB_Local_FileAddress;

            public string _VB_Local_ArchiveBaseFolder;
        }

        [CompilerGenerated]
        private sealed class _Closure___11_0
        {
            public _Closure___11_0(ModModpack._Closure___11_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_TargetFolder = arg0._VB_Local_TargetFolder;
                    this._VB_Local_FileAddress = arg0._VB_Local_FileAddress;
                    this._VB_Local_ArchiveBaseFolder = arg0._VB_Local_ArchiveBaseFolder;
                }
            }

            internal void _Lambda___0(ModLoader.LoaderTask<string, int> Task)
            {
                ModModpack.ExtractModpackFiles(this._VB_Local_TargetFolder, this._VB_Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.95));
                PageSelectLeft.AddFolder(this._VB_Local_TargetFolder + this._VB_Local_ArchiveBaseFolder, PathUtils.GetLastPart(this._VB_Local_TargetFolder), false);
                Thread.Sleep(400);
                ModBase.RunInUi((ModModpack._Closure___._I11_1 == null) ? (ModModpack._Closure___._I11_1 = new Action(ModModpack._Closure___._I._Lambda___11_1)) : ModModpack._Closure___._I11_1, false);
            }

            public string _VB_Local_TargetFolder;

            public string _VB_Local_FileAddress;

            public string _VB_Local_ArchiveBaseFolder;
        }

        [CompilerGenerated]
        private sealed class _Closure___5_0
        {
            public _Closure___5_0(ModModpack._Closure___5_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_InstallTemp = arg0._VB_Local_InstallTemp;
                    this._VB_Local_FileAddress = arg0._VB_Local_FileAddress;
                    this._VB_Local_VersionFolder = arg0._VB_Local_VersionFolder;
                    this._VB_Local_ModList = arg0._VB_Local_ModList;
                    this._VB_Local_Json = arg0._VB_Local_Json;
                    this._VB_Local_ModOptionalList = arg0._VB_Local_ModOptionalList;
                    this._VB_Local_TargetVersion = arg0._VB_Local_TargetVersion;
                    this._VB_Local_Logo = arg0._VB_Local_Logo;
                    this._VB_Local_LoaderName = arg0._VB_Local_LoaderName;
                }
            }

            internal void _Lambda___1(ModLoader.LoaderTask<int, JArray> Task)
            {
                Task.Output = (JArray)ModDownload.DlModRequest("https://api.curseforge.com/v1/mods/files", HttpMethod.Post, "{\"fileIds\": [" + this._VB_Local_ModList.Join(",") + "]}", "application/json")["data"];
                if (this._VB_Local_ModList.Count != Task.Output.Count)
                {
                    List<int> list = this._VB_Local_ModList.Except<int>(Task.Output.Select<JToken, int>((ModModpack._Closure___._I5_2 == null) ? (ModModpack._Closure___._I5_2 = new Func<JToken, int>(ModModpack._Closure___._I._Lambda___5_2)) : ModModpack._Closure___._I5_2)).ToList<int>();
                    Logger.Info(string.Format("缺失的 fileIds：{0}", list.Join("、")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                    List<ValueTuple<int, int>> list2 = list.Select<int, ValueTuple<int, int>>((this._I3 == null) ? (this._I3 = new Func<int, ValueTuple<int, int>>(this._Lambda___3)) : this._I3).ToList<ValueTuple<int, int>>();
                    Logger.Info(string.Format("缺失项：{0}", list2.Join("、")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                    List<ResourceProject> list3 = new List<ResourceProject>();
                    try
                    {
                        list3 = ResourceProject.FromProjectIds(list2.Select<ValueTuple<int, int>, string>((ModModpack._Closure___._I5_5 == null) ? (ModModpack._Closure___._I5_5 = new Func<ValueTuple<int, int>, string>(ModModpack._Closure___._I._Lambda___5_5)) : ModModpack._Closure___._I5_5), ResourcePlatforms.CurseForge).ToList<ResourceProject>();
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "获取缺失的工程信息失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                    }
                    List<string> list4 = new List<string>();
                    try
                    {
                        List<ValueTuple<int, int>>.Enumerator enumerator = list2.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            ModModpack._Closure___5_3 CS_locals1 = new ModModpack._Closure___5_3(null);
                            CS_locals1._VB_Local_Missing = enumerator.Current;
                            ResourceProject resourceProject = list3.FirstOrDefault<ResourceProject>(new Func<ResourceProject, bool>(CS_locals1._Lambda___6));
                            if (((object)resourceProject == (object)null))
                            {
                                list4.Add(string.Format("未知 CurseForge 项目 (项目 ID {0}，文件 ID {1})", CS_locals1._VB_Local_Missing.Item1, CS_locals1._VB_Local_Missing.Item2));
                            }
                            else
                            {
                                list4.Add(string.Format("{0} (文件 ID {1})", resourceProject.TranslatedName, CS_locals1._VB_Local_Missing.Item2));
                            }
                        }
                    }
                    finally
                    {
                        List<ValueTuple<int, int>>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                    if (ModMain.OurMsgBox(string.Format("未找到该整合包所需的以下内容，它们可能已被原作者删除：{0}- {1}{2}{3}是否继续安装？缺失这些内容可能导致整合包出现异常！", new object[]
                    {
                        "\r\n",
                        list4.Join("\r\n- "),
                        "\r\n",
                        "\r\n"
                    }), "整合包内容缺失", "继续", "取消", "", true, true, false, null, null, null) == 2)
                    {
                        Task.Cancel();
                    }
                }
            }

            internal ValueTuple<int, int> _Lambda___3(int id)
            {
                ModModpack._Closure___5_2 CS_locals1 = new ModModpack._Closure___5_2(null);
                CS_locals1._VB_Local_id = id;
                return new ValueTuple<int, int>(((JObject)this._VB_Local_Json["files"].AsEnumerable<JToken>().First<JToken>(new Func<JToken, bool>(CS_locals1._Lambda___4)))["projectID"].ToObject<int>(), CS_locals1._VB_Local_id);
            }

            internal void _Lambda___7(ModLoader.LoaderTask<JArray, List<ModNet.NetFile>> Task)
            {
                Dictionary<int, ModNet.NetFile> dictionary = new Dictionary<int, ModNet.NetFile>();
                try
                {
                    foreach (JToken jtoken in Task.Input)
                    {
                        int num = jtoken["id"].ToObject<int>();
                        if (!dictionary.ContainsKey(num) && (!this._VB_Local_ModOptionalList.Contains(num) || ModMain.OurMsgBox("是否要下载整合包中的可选文件 " + jtoken["displayName"].ToString() + "？", "下载可选文件", "是", "否", "", false, true, false, null, null, null) != 2))
                        {
                            string text;
                            ResourceTypes resourceTypes;
                            if (jtoken["modules"].Any<JToken>())
                            {
                                List<string> list = ((JArray)jtoken["modules"]).Select<JToken, string>((ModModpack._Closure___._I5_8 == null) ? (ModModpack._Closure___._I5_8 = new Func<JToken, string>(ModModpack._Closure___._I._Lambda___5_8)) : ModModpack._Closure___._I5_8).ToList<string>();
                                if (!list.Contains("META-INF") && !list.Contains("mcmod.info") && !list.Contains("fabric.mod.json"))
                                {
                                    bool flag;
                                    if (jtoken == null)
                                    {
                                        flag = false;
                                    }
                                    else
                                    {
                                        JToken jtoken2 = jtoken["FileName"];
                                        flag = jtoken2 != null && jtoken2.ToString().EndsWithF(".jar", true);
                                    }
                                    if (!flag)
                                    {
                                        if (list.Contains("pack.mcmeta"))
                                        {
                                            text = "resourcepacks";
                                            resourceTypes = ResourceTypes.ResourcePack;
                                            goto IL_018D;
                                        }
                                        if (list.Contains("shaders"))
                                        {
                                            text = "shaderpacks";
                                            resourceTypes = ResourceTypes.Shader;
                                            goto IL_018D;
                                        }
                                        text = "saves";
                                        resourceTypes = ResourceTypes.Map;
                                        goto IL_018D;
                                    }
                                }
                                text = "mods";
                                resourceTypes = ResourceTypes.Mod;
                            }
                            else
                            {
                                text = "mods";
                                resourceTypes = ResourceTypes.Mod;
                            }
                            IL_018D:
                            ResourceVersion resourceVersion = ResourceVersion.FromPlatformJson((JObject)jtoken, resourceTypes);
                            if (resourceVersion.DownloadAvailable)
                            {
                                dictionary.Add(num, resourceVersion.ToNetFile(PathUtils.AddSlashSuffix(Path.Combine(this._VB_Local_VersionFolder, text)), new ResourceVersion.DownloadReason?(ResourceVersion.DownloadReason.ModPack), this._VB_Local_TargetVersion.VanillaName, this._VB_Local_TargetVersion.ModLoaders));
                                Task.Progress += 1.0 / (double)(checked(1 + this._VB_Local_ModList.Count));
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
                Task.Output = dictionary.Values.ToList<ModNet.NetFile>();
            }

            internal void _Lambda___12(ModLoader.LoaderTask<string, string> Task)
            {
                if (this._VB_Local_Logo != null && FileUtils.Exists(this._VB_Local_Logo))
                {
                    FileUtils.Copy(this._VB_Local_Logo, this._VB_Local_VersionFolder + "PCL\\Logo.png");
                    ModBase.WriteIni(this._VB_Local_VersionFolder + "PCL\\Setup.ini", "Logo", "PCL\\Logo.png");
                    ModBase.WriteIni(this._VB_Local_VersionFolder + "PCL\\Setup.ini", "LogoCustom", "True");
                    Logger.Info(string.Format("已设置整合包 Logo：{0}", this._VB_Local_Logo), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                }
                string text = string.Format("{0}saves\\", this._VB_Local_VersionFolder);
                try
                {
                    foreach (string text2 in DirectoryUtils.EnumerateFiles(text, false, "*.zip"))
                    {
                        FileUtils.ExtractToDirectory(text2, text, Task.CreateCancellationToken(), null);
                        FileUtils.Delete(text2, false);
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
                foreach (string text3 in new string[]
                {
                    this._VB_Local_VersionFolder + "原始整合包.zip",
                    this._VB_Local_VersionFolder + "原始整合包.mrpack"
                })
                {
                    if (FileUtils.Exists(text3))
                    {
                        Logger.Info(string.Format("删除原始整合包文件：{0}", text3), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        FileUtils.Delete(text3, false);
                    }
                }
                if (FileUtils.Exists(this._VB_Local_FileAddress) && Operators.CompareString(PathUtils.GetFileNameWithoutExtension(this._VB_Local_FileAddress), "modpack", false) == 0)
                {
                    Logger.Info(string.Format("删除安装整合包文件：{0}", this._VB_Local_FileAddress), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                    FileUtils.Delete(this._VB_Local_FileAddress, false);
                }
            }

            internal bool _Lambda___13(ModLoader.LoaderBase l)
            {
                return Operators.CompareString(l.Name, this._VB_Local_LoaderName, false) == 0;
            }

            public string _VB_Local_InstallTemp;

            public string _VB_Local_FileAddress;

            public string _VB_Local_VersionFolder;

            public List<int> _VB_Local_ModList;

            public JObject _VB_Local_Json;

            public List<int> _VB_Local_ModOptionalList;

            public McVersion _VB_Local_TargetVersion;

            public string _VB_Local_Logo;

            public string _VB_Local_LoaderName;

            public Func<int, ValueTuple<int, int>> _I3;
        }

        [CompilerGenerated]
        private sealed class _Closure___5_1
        {
            public _Closure___5_1(ModModpack._Closure___5_1 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_OverridesFolder = arg0._VB_Local_OverridesFolder;
                }
            }

            internal void _Lambda___0(ModLoader.LoaderTask<string, int> Task)
            {
                ModModpack.ExtractModpackFiles(this._VB_NonLocal__VB_Closure_2._VB_Local_InstallTemp, this._VB_NonLocal__VB_Closure_2._VB_Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.6));
                ModModpack.CopyOverrideDirectory(this._VB_Local_OverridesFolder, this._VB_NonLocal__VB_Closure_2._VB_Local_VersionFolder, Task);
            }

            public string _VB_Local_OverridesFolder;

            public ModModpack._Closure___5_0 _VB_NonLocal__VB_Closure_2;
        }

        [CompilerGenerated]
        private sealed class _Closure___5_2
        {
            public _Closure___5_2(ModModpack._Closure___5_2 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_id = arg0._VB_Local_id;
                }
            }

            internal bool _Lambda___4(JToken f)
            {
                return f["fileID"].ToObject<int>() == this._VB_Local_id;
            }

            public int _VB_Local_id;
        }

        [CompilerGenerated]
        private sealed class _Closure___5_3
        {
            public _Closure___5_3(ModModpack._Closure___5_3 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Missing = arg0._VB_Local_Missing;
                }
            }

            internal bool _Lambda___6(ResourceProject p)
            {
                return Conversions.ToDouble(p.Id) == (double)this._VB_Local_Missing.Item1;
            }

            public ValueTuple<int, int> _VB_Local_Missing;
        }

        [CompilerGenerated]
        private sealed class _Closure___6_0
        {
            public _Closure___6_0(ModModpack._Closure___6_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_InstallTemp = arg0._VB_Local_InstallTemp;
                    this._VB_Local_FileAddress = arg0._VB_Local_FileAddress;
                    this._VB_Local_ArchiveBaseFolder = arg0._VB_Local_ArchiveBaseFolder;
                    this._VB_Local_VersionFolder = arg0._VB_Local_VersionFolder;
                    this._VB_Local_Logo = arg0._VB_Local_Logo;
                    this._VB_Local_LoaderName = arg0._VB_Local_LoaderName;
                }
            }

            internal void _Lambda___0(ModLoader.LoaderTask<string, int> Task)
            {
                ModModpack.ExtractModpackFiles(this._VB_Local_InstallTemp, this._VB_Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.5));
                ModModpack.CopyOverrideDirectory(this._VB_Local_InstallTemp + this._VB_Local_ArchiveBaseFolder + "overrides", this._VB_Local_VersionFolder, Task);
                ModModpack.CopyOverrideDirectory(this._VB_Local_InstallTemp + this._VB_Local_ArchiveBaseFolder + "client-overrides", this._VB_Local_VersionFolder, Task);
            }

            internal void _Lambda___5(ModLoader.LoaderTask<string, string> Task)
            {
                if (this._VB_Local_Logo != null && FileUtils.Exists(this._VB_Local_Logo))
                {
                    FileUtils.Copy(this._VB_Local_Logo, this._VB_Local_VersionFolder + "PCL\\Logo.png");
                    ModBase.WriteIni(this._VB_Local_VersionFolder + "PCL\\Setup.ini", "Logo", "PCL\\Logo.png");
                    ModBase.WriteIni(this._VB_Local_VersionFolder + "PCL\\Setup.ini", "LogoCustom", "True");
                    Logger.Info(string.Format("已设置整合包 Logo：{0}", this._VB_Local_Logo), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                }
                foreach (string text in new string[]
                {
                    this._VB_Local_VersionFolder + "原始整合包.zip",
                    this._VB_Local_VersionFolder + "原始整合包.mrpack"
                })
                {
                    if (FileUtils.Exists(text))
                    {
                        Logger.Info(string.Format("删除原始整合包文件：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        FileUtils.Delete(text, false);
                    }
                }
                if (FileUtils.Exists(this._VB_Local_FileAddress) && Operators.CompareString(PathUtils.GetFileNameWithoutExtension(this._VB_Local_FileAddress), "modpack", false) == 0)
                {
                    Logger.Info(string.Format("删除安装整合包文件：{0}", this._VB_Local_FileAddress), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                    FileUtils.Delete(this._VB_Local_FileAddress, false);
                }
            }

            internal bool _Lambda___6(ModLoader.LoaderBase l)
            {
                return Operators.CompareString(l.Name, this._VB_Local_LoaderName, false) == 0;
            }

            public string _VB_Local_InstallTemp;

            public string _VB_Local_FileAddress;

            public string _VB_Local_ArchiveBaseFolder;

            public string _VB_Local_VersionFolder;

            public string _VB_Local_Logo;

            public string _VB_Local_LoaderName;
        }

        [CompilerGenerated]
        private sealed class _Closure___7_0
        {
            public _Closure___7_0()
            {
            }

            internal void _Lambda___0(ModLoader.LoaderTask<string, int> Task)
            {
                ModModpack.ExtractModpackFiles(this._VB_Local_InstallTemp, this._VB_Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.6));
                ModModpack.CopyOverrideDirectory(this._VB_Local_InstallTemp + this._VB_Local_ArchiveBaseFolder + "minecraft", this._VB_Local_VersionFolder, Task);
            }

            internal bool _Lambda___3(ModLoader.LoaderBase l)
            {
                return Operators.CompareString(l.Name, this._VB_Local_LoaderName, false) == 0;
            }

            public string _VB_Local_InstallTemp;

            public string _VB_Local_FileAddress;

            public string _VB_Local_ArchiveBaseFolder;

            public string _VB_Local_VersionFolder;

            public string _VB_Local_LoaderName;
        }

        [CompilerGenerated]
        private sealed class _Closure___8_0
        {
            public _Closure___8_0(ModModpack._Closure___8_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_InstallTemp = arg0._VB_Local_InstallTemp;
                    this._VB_Local_FileAddress = arg0._VB_Local_FileAddress;
                    this._VB_Local_ArchiveBaseFolder = arg0._VB_Local_ArchiveBaseFolder;
                    this._VB_Local_VersionFolder = arg0._VB_Local_VersionFolder;
                    this._VB_Local_SetupFile = arg0._VB_Local_SetupFile;
                    this._VB_Local_LoaderName = arg0._VB_Local_LoaderName;
                }
            }

            internal void _Lambda___0(ModLoader.LoaderTask<string, int> Task)
            {
                ModModpack.ExtractModpackFiles(this._VB_Local_InstallTemp, this._VB_Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.55));
                string text;
                if (DirectoryUtils.Exists(this._VB_Local_InstallTemp + this._VB_Local_ArchiveBaseFolder + ".minecraft") && !DirectoryUtils.Exists(this._VB_Local_InstallTemp + this._VB_Local_ArchiveBaseFolder + "minecraft"))
                {
                    text = this._VB_Local_InstallTemp + this._VB_Local_ArchiveBaseFolder + ".minecraft";
                }
                else
                {
                    text = this._VB_Local_InstallTemp + this._VB_Local_ArchiveBaseFolder + "minecraft";
                }
                ModModpack.CopyOverrideDirectory(text, this._VB_Local_VersionFolder, Task);
                try
                {
                    string text2 = this._VB_Local_InstallTemp + this._VB_Local_ArchiveBaseFolder + "instance.cfg";
                    if (FileUtils.Exists(text2))
                    {
                        List<string> list = new List<string>();
                        foreach (string text3 in FileUtils.ReadAsLines(text2, true, null, null))
                        {
                            if (text3.Contains("="))
                            {
                                list.Add(text3.BeforeFirst("=", false) + ":" + text3.AfterFirst("=", false));
                            }
                        }
                        FileUtils.Write(text2, list.Join("\r\n"), null);
                        if (Conversions.ToBoolean(ModBase.ReadIni(text2, "OverrideCommands", Conversions.ToString(false))))
                        {
                            string text4 = ModBase.ReadIni(text2, "PreLaunchCommand", "");
                            if (Operators.CompareString(text4, "", false) != 0)
                            {
                                text4 = text4.Replace("\\\"", "\"").Replace("$INST_JAVA", "{java}java.exe").Replace("$INST_MC_DIR\\", "{minecraft}")
                                    .Replace("$INST_MC_DIR", "{minecraft}")
                                    .Replace("$INST_DIR\\", "{verpath}")
                                    .Replace("$INST_DIR", "{verpath}")
                                    .Replace("$INST_ID", "{name}")
                                    .Replace("$INST_NAME", "{name}");
                                ModBase.WriteIni(this._VB_Local_SetupFile, "VersionAdvanceRun", text4);
                                Logger.Info(string.Format("迁移 MultiMC 版本独立设置：启动前执行命令：{0}", text4), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                            }
                        }
                        if (Conversions.ToBoolean(ModBase.ReadIni(text2, "JoinServerOnLaunch", Conversions.ToString(false))))
                        {
                            string text5 = ModBase.ReadIni(text2, "JoinServerOnLaunchAddress", "").Replace("\\\"", "\"");
                            ModBase.WriteIni(this._VB_Local_SetupFile, "VersionServerEnter", text5);
                            Logger.Info(string.Format("迁移 MultiMC 版本独立设置：自动进入服务器：{0}", text5), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        }
                        string text6 = ModBase.ReadIni(text2, "iconKey", "");
                        if (Operators.CompareString(text6, "", false) != 0 && FileUtils.Exists(string.Format("{0}{1}{2}.png", this._VB_Local_InstallTemp, this._VB_Local_ArchiveBaseFolder, text6)))
                        {
                            ModBase.WriteIni(this._VB_Local_SetupFile, "LogoCustom", Conversions.ToString(true));
                            ModBase.WriteIni(this._VB_Local_SetupFile, "Logo", "PCL\\Logo.png");
                            FileUtils.Copy(string.Format("{0}{1}{2}.png", this._VB_Local_InstallTemp, this._VB_Local_ArchiveBaseFolder, text6), this._VB_Local_VersionFolder + "PCL\\Logo.png");
                            Logger.Info(string.Format("迁移 MultiMC 版本独立设置：版本图标（{0}.png）", text6), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                        }
                        string text7 = ModBase.ReadIni(text2, "JvmArgs", "");
                        if (Operators.CompareString(text7, "", false) != 0)
                        {
                            if (Conversions.ToBoolean(ModBase.ReadIni(text2, "OverrideJavaArgs", Conversions.ToString(false))))
                            {
                                ModBase.WriteIni(this._VB_Local_SetupFile, "VersionAdvanceJvm", text7);
                                Logger.Info(string.Format("迁移 MultiMC 版本独立设置：JVM 参数（覆盖）：{0}", text7), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                            }
                            else
                            {
                                text7 = text7 + " " + Settings.Get<string>("LaunchAdvanceJvm", null);
                                ModBase.WriteIni(this._VB_Local_SetupFile, "VersionAdvanceJvm", text7);
                                Logger.Info(string.Format("迁移 MultiMC 版本独立设置：JVM 参数（追加）：{0}", text7), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, string.Format("读取 MMC 配置文件失败（{0}{1}instance.cfg）", this._VB_Local_InstallTemp, this._VB_Local_ArchiveBaseFolder), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModModpack.vb");
                }
            }

            internal bool _Lambda___3(ModLoader.LoaderBase l)
            {
                return Operators.CompareString(l.Name, this._VB_Local_LoaderName, false) == 0;
            }

            public string _VB_Local_InstallTemp;

            public string _VB_Local_FileAddress;

            public string _VB_Local_ArchiveBaseFolder;

            public string _VB_Local_VersionFolder;

            public string _VB_Local_SetupFile;

            public string _VB_Local_LoaderName;
        }

        [CompilerGenerated]
        private sealed class _Closure___9_0
        {
            public _Closure___9_0(ModModpack._Closure___9_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_InstallTemp = arg0._VB_Local_InstallTemp;
                    this._VB_Local_FileAddress = arg0._VB_Local_FileAddress;
                    this._VB_Local_ArchiveBaseFolder = arg0._VB_Local_ArchiveBaseFolder;
                    this._VB_Local_VersionFolder = arg0._VB_Local_VersionFolder;
                    this._VB_Local_Json = arg0._VB_Local_Json;
                    this._VB_Local_SetupFile = arg0._VB_Local_SetupFile;
                    this._VB_Local_LoaderName = arg0._VB_Local_LoaderName;
                }
            }

            internal void _Lambda___0(ModLoader.LoaderTask<string, int> Task)
            {
                ModModpack.ExtractModpackFiles(this._VB_Local_InstallTemp, this._VB_Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.6));
                ModModpack.CopyOverrideDirectory(this._VB_Local_InstallTemp + this._VB_Local_ArchiveBaseFolder + "overrides", this._VB_Local_VersionFolder, Task);
                if (this._VB_Local_Json["launchInfo"] != null)
                {
                    JObject jobject = (JObject)this._VB_Local_Json["launchInfo"];
                    if (jobject.ContainsKey("javaArgument"))
                    {
                        ModBase.WriteIni(this._VB_Local_SetupFile, "VersionAdvanceJvm", string.Join<JToken>(" ", jobject["javaArgument"]));
                    }
                    if (jobject.ContainsKey("launchArgument"))
                    {
                        ModBase.WriteIni(this._VB_Local_SetupFile, "VersionAdvanceGame", string.Join<JToken>(" ", jobject["launchArgument"]));
                    }
                }
            }

            internal bool _Lambda___4(ModLoader.LoaderBase l)
            {
                return Operators.CompareString(l.Name, this._VB_Local_LoaderName, false) == 0;
            }

            public string _VB_Local_InstallTemp;

            public string _VB_Local_FileAddress;

            public string _VB_Local_ArchiveBaseFolder;

            public string _VB_Local_VersionFolder;

            public JObject _VB_Local_Json;

            public string _VB_Local_SetupFile;

            public string _VB_Local_LoaderName;
        }

        [CompilerGenerated]
        private sealed class _Closure___9_1
        {
            public _Closure___9_1(ModModpack._Closure___9_1 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_MinecraftVersion = arg0._VB_Local_MinecraftVersion;
                    this._VB_Local_OptiFineFileSuffix = arg0._VB_Local_OptiFineFileSuffix;
                }
            }

            internal bool _Lambda___1(ModDownload.DlOptiFineListEntry v)
            {
                return Operators.CompareString(v.Inherit, this._VB_Local_MinecraftVersion, false) == 0 && v.FileName.EndsWithF(this._VB_Local_OptiFineFileSuffix, false);
            }

            public string _VB_Local_MinecraftVersion;

            public string _VB_Local_OptiFineFileSuffix;
        }
    }
}
