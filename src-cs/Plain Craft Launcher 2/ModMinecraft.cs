using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Documents;
using MeloongCore;
using MeloongCore.Extensions;
using MeloongCore.Wpf;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;
using PCL.My;
using PCLCS;

namespace PCL
{
    [StandardModule]
    public sealed class ModMinecraft
    {
        public static string McFolderSelected
        {
            get
            {
                if (ModMinecraft._McFolderSelected == null)
                {
                    ModMinecraft.SetMcFolder(Settings.Get<string>("LaunchFolderSelect", null));
                }
                return ModMinecraft._McFolderSelected;
            }
            set
            {
                ModMinecraft.SetMcFolder(value);
            }
        }

        private static void SetMcFolder(string Value)
        {
            if (!PathUtils.IsSame(ModMinecraft._McFolderSelected, Value.Replace("$", Paths.Base)))
            {
                ModMinecraft._McFolderSelected = Value.Replace("$", Paths.Base);
                ModMinecraft._McFolderSelected = PathUtils.AddSlashSuffix(PathUtils.ForCompare(ModMinecraft._McFolderSelected));
                Settings.Set("LaunchFolderSelect", Value.Replace(Paths.Base, "$"), null);
                Logger.Info(string.Format("当前选择的 Minecraft 文件夹：{0}", ModMinecraft._McFolderSelected), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
            }
        }

        private static void McFolderListLoadSub()
        {
            try
            {
                List<ModMinecraft.McFolder> list = new List<ModMinecraft.McFolder>();
                try
                {
                    if (DirectoryUtils.Exists(Paths.Base + "versions\\"))
                    {
                        list.Add(new ModMinecraft.McFolder
                        {
                            Name = "当前文件夹",
                            Location = Paths.Base,
                            Type = ModMinecraft.McFolder.Types.Vanilla
                        });
                    }
                    foreach (DirectoryInfo directoryInfo in DirectoryUtils.GetInfo(Paths.Base).GetDirectories())
                    {
                        if (DirectoryUtils.Exists(directoryInfo.FullName + "versions\\") || Operators.CompareString(directoryInfo.Name, ".minecraft", false) == 0)
                        {
                            list.Add(new ModMinecraft.McFolder
                            {
                                Name = "当前文件夹",
                                Location = PathUtils.AddSlashSuffix(PathUtils.ForCompare(directoryInfo.FullName)),
                                Type = ModMinecraft.McFolder.Types.Vanilla
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "扫描 PCL 所在文件夹中是否有 MC 文件夹失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                }
                string text = Paths.AppData + ".minecraft\\";
                if ((!list.Any<ModMinecraft.McFolder>() || !PathUtils.IsSame(text, list[0].Location)) && DirectoryUtils.Exists(text + "versions\\"))
                {
                    list.Add(new ModMinecraft.McFolder
                    {
                        Name = "官方启动器文件夹",
                        Location = text,
                        Type = ModMinecraft.McFolder.Types.Vanilla
                    });
                }
                foreach (string text2 in Settings.Get<string>("LaunchFolders", null).ToString().Split('|', true))
                {
                    if (Operators.CompareString(text2, "", false) != 0)
                    {
                        if (!text2.Contains(">") || !text2.EndsWithF("\\", false))
                        {
                            ModMain.Hint("无效的 Minecraft 文件夹：" + text2, ModMain.HintType.Red, true);
                        }
                        else
                        {
                            string text3 = text2.BeforeFirst(">", false);
                            string text4 = text2.AfterFirst(">", false);
                            try
                            {
                                ModBase.CheckPermissionWithException(text4);
                                bool flag = false;
                                try
                                {
                                    foreach (ModMinecraft.McFolder mcFolder in list)
                                    {
                                        if (PathUtils.IsSame(mcFolder.Location, text4))
                                        {
                                            mcFolder.Name = text3;
                                            mcFolder.Type = ModMinecraft.McFolder.Types.RenamedVanilla;
                                            flag = true;
                                        }
                                    }
                                }
                                finally
                                {
                                    List<ModMinecraft.McFolder>.Enumerator enumerator;
                                    ((IDisposable)enumerator).Dispose();
                                }
                                if (!flag)
                                {
                                    list.Add(new ModMinecraft.McFolder
                                    {
                                        Name = text3,
                                        Location = PathUtils.AddSlashSuffix(PathUtils.ForCompare(text4)),
                                        Type = ModMinecraft.McFolder.Types.Custom
                                    });
                                }
                            }
                            catch (Exception ex2)
                            {
                                ModMain.MyMsgBox("失效的 Minecraft 文件夹：\r\n" + text4 + "\r\n\r\n" + ex2.GetDisplay(false), "Minecraft 文件夹失效", "确定", "", "", true, true, false, null, null, null);
                                Logger.Warn(ex2, string.Format("无法访问 Minecraft 文件夹 {0}", text4), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                            }
                        }
                    }
                }
                List<string> list2 = new List<string>();
                try
                {
                    foreach (ModMinecraft.McFolder mcFolder2 in list)
                    {
                        if (mcFolder2.Type != ModMinecraft.McFolder.Types.Vanilla)
                        {
                            list2.Add(mcFolder2.Name + ">" + mcFolder2.Location);
                        }
                    }
                }
                finally
                {
                    List<ModMinecraft.McFolder>.Enumerator enumerator2;
                    ((IDisposable)enumerator2).Dispose();
                }
                if (!list2.Any<string>())
                {
                    list2.Add("");
                }
                Settings.Set("LaunchFolders", list2.Join('|'), null);
                if (!list.Any<ModMinecraft.McFolder>())
                {
                    DirectoryUtils.Create(Paths.Base + ".minecraft\\versions\\");
                    list.Add(new ModMinecraft.McFolder
                    {
                        Name = "当前文件夹",
                        Location = Paths.Base + ".minecraft\\",
                        Type = ModMinecraft.McFolder.Types.Vanilla
                    });
                }
                try
                {
                    foreach (ModMinecraft.McFolder mcFolder3 in list)
                    {
                        ModMinecraft.McFolderLauncherProfilesJsonCreate(mcFolder3.Location);
                    }
                }
                finally
                {
                    List<ModMinecraft.McFolder>.Enumerator enumerator3;
                    ((IDisposable)enumerator3).Dispose();
                }
                if (Settings.Get<bool>("SystemDebugDelay", null))
                {
                    Thread.Sleep(ModBase.RandomInteger(200, 2000));
                }
                ModMinecraft.McFolderList = list;
            }
            catch (Exception ex3)
            {
                Logger.Error(ex3, "加载 Minecraft 文件夹列表失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
            }
        }

        public static void McFolderLauncherProfilesJsonCreate(string Folder)
        {
            try
            {
                if (!FileUtils.Exists(Folder + "launcher_profiles.json"))
                {
                    string text = string.Concat(new string[]
                    {
                        "{\r\n    \"profiles\":  {\r\n        \"PCL\": {\r\n            \"icon\": \"Grass\",\r\n            \"name\": \"PCL\",\r\n            \"lastVersionId\": \"latest-release\",\r\n            \"type\": \"latest-release\",\r\n            \"lastUsed\": \"",
                        DateTime.Now.ToString("yyyy'-'MM'-'dd"),
                        "T",
                        DateTime.Now.ToString("HH':'mm':'ss"),
                        ".0000Z\"\r\n        }\r\n    },\r\n    \"selectedProfile\": \"PCL\",\r\n    \"clientToken\": \"23323323323323323323323323323333\"\r\n}"
                    });
                    FileUtils.Write(Folder + "launcher_profiles.json", text, Encoding.GetEncoding("GB18030"));
                    Logger.Info(string.Format("已创建 launcher_profiles.json：{0}", Folder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, string.Format("创建 launcher_profiles.json 失败（{0}）", Folder), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
            }
        }

        public static ModMinecraft.McInstance McInstanceSelected
        {
            get
            {
                return ModMinecraft._McInstanceSelected;
            }
            set
            {
                if (ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init == null)
                {
                    Interlocked.CompareExchange<StaticLocalInitFlag>(ref ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init, new StaticLocalInitFlag(), null);
                }
                bool flag = false;
                try
                {
                    Monitor.Enter(ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init, ref flag);
                    if (ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init.State == 0)
                    {
                        ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init.State = 2;
                        ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast = 0;
                    }
                    else if (ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init.State == 2)
                    {
                        throw new IncompleteInitialization();
                    }
                }
                finally
                {
                    ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init.State = 1;
                    if (flag)
                    {
                        Monitor.Exit(ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init);
                    }
                }
                if (!object.ReferenceEquals(RuntimeHelpers.GetObjectValue(ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast), value))
                {
                    ModMinecraft._McInstanceSelected = value;
                    ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast = value;
                    Logger.Info(string.Format("当前选择的 Minecraft 版本：{0}", ((value != null) ? value.PathVersion : null) ?? "无"), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                    ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "Version", ((value != null) ? value.Name : null) ?? "");
                    if (value != null)
                    {
                        PageDownloadResourceDetail.CachedFolder.Clear();
                        if (ModAnimation.AniControlEnabled == 0 && Operators.CompareString(Settings.Get<string>("VersionServerNide", value), Settings.Get<string>("CacheNideServer", null), false) != 0 && Settings.Get<int>("VersionServerLogin", value) == 3)
                        {
                            Settings.Set("CacheNideAccess", "", null);
                            Logger.Info("服务器改变，要求重新登录统一通行证", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                        }
                        if (Settings.Get<int>("VersionServerLogin", value) == 3)
                        {
                            Settings.Set("CacheNideServer", Settings.Get<string>("VersionServerNide", value), null);
                        }
                        if (ModAnimation.AniControlEnabled == 0 && Operators.CompareString(Settings.Get<string>("VersionServerAuthServer", value), Settings.Get<string>("CacheAuthServerServer", null), false) != 0 && Settings.Get<int>("VersionServerLogin", value) == 4)
                        {
                            Settings.Set("CacheAuthAccess", "", null);
                            Logger.Info("服务器改变，要求重新登录 Authlib-Injector", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                        }
                        if (Settings.Get<int>("VersionServerLogin", value) == 4)
                        {
                            Settings.Set("CacheAuthServerServer", Settings.Get<string>("VersionServerAuthServer", value), null);
                            Settings.Set("CacheAuthServerName", Settings.Get<string>("VersionServerAuthName", value), null);
                            Settings.Set("CacheAuthServerRegister", Settings.Get<string>("VersionServerAuthRegister", value), null);
                        }
                    }
                }
            }
        }

        public static string GetMcFoolName(string Name)
        {
            Name = Name.Lower();
            string text;
            if (Name.StartsWithF("2.0", false))
            {
                text = "2013 | 这个秘密计划了两年的更新将游戏推向了一个新高度！";
            }
            else if (Operators.CompareString(Name, "15w14a", false) == 0)
            {
                text = "2015 | 作为一款全年龄向的游戏，我们需要和平，需要爱与拥抱。";
            }
            else if (Operators.CompareString(Name, "1.rv-pre1", false) == 0)
            {
                text = "2016 | 是时候将现代科技带入 Minecraft 了！";
            }
            else if (Operators.CompareString(Name, "3d shareware v1.34", false) == 0)
            {
                text = "2019 | 我们从地下室的废墟里找到了这个开发于 1994 年的杰作！";
            }
            else if (Name.StartsWithF("20w14inf", false) || Operators.CompareString(Name, "20w14∞", false) == 0)
            {
                text = "2020 | 我们加入了 20 亿个新的维度，让无限的想象变成了现实！";
            }
            else if (Operators.CompareString(Name, "22w13oneblockatatime", false) == 0)
            {
                text = "2022 | 一次一个方块更新！迎接全新的挖掘、合成与骑乘玩法吧！";
            }
            else if (Operators.CompareString(Name, "23w13a_or_b", false) == 0)
            {
                text = "2023 | 研究表明：玩家喜欢作出选择——越多越好！";
            }
            else if (Operators.CompareString(Name, "24w14potato", false) == 0)
            {
                text = "2024 | 毒马铃薯一直都被大家忽视和低估，于是我们超级加强了它！";
            }
            else if (Operators.CompareString(Name, "25w14craftmine", false) == 0)
            {
                text = "2025 | 你可以合成任何东西——包括合成你的世界！";
            }
            else if (Operators.CompareString(Name, "26w14a", false) == 0)
            {
                text = "2026 | 为什么需要物品栏？让方块们跟着你走吧！";
            }
            else
            {
                text = "";
            }
            return text;
        }

        private static void InitMcInstanceList(ModLoader.LoaderTask<string, int> Loader)
        {
            string input = Loader.Input;
            try
            {
                ModMinecraft.McInstanceList = new Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>>();
                List<string> list = new List<string>();
                if (DirectoryUtils.Exists(input + "versions"))
                {
                    try
                    {
                        foreach (DirectoryInfo directoryInfo in DirectoryUtils.GetInfo(input + "versions").GetDirectories())
                        {
                            list.Add(directoryInfo.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("无法读取版本文件夹，可能是由于没有权限（" + input + "versions）", ex);
                    }
                }
                if (!list.Any<string>())
                {
                    ModBase.WriteIni(input + "PCL.ini", "InstanceCache", "");
                }
                else
                {
                    int num = Convert.ToInt32(decimal.Remainder(new decimal((Conversions.ToString(39) + "#" + list.Join('#')).GetStableHashCode()), 2147483646m));
                    if (!ModMinecraft.McInstanceListForceRefresh && ModBase.Val(ModBase.ReadIni(input + "PCL.ini", "InstanceCache", "")) == (double)num)
                    {
                        Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>> dictionary = ModMinecraft.InitMcInstanceListWithCache(input);
                        if (dictionary != null)
                        {
                            ModMinecraft.McInstanceList = dictionary;
                            goto IL_0161;
                        }
                    }
                    ModMinecraft.McInstanceListForceRefresh = false;
                    Logger.Info("文件夹列表变更，重载所有版本", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                    ModBase.WriteIni(input + "PCL.ini", "InstanceCache", Conversions.ToString(num));
                    ModMinecraft.McInstanceList = ModMinecraft.InitMcInstanceListWithoutCache(input);
                    IL_0161:
                    ModMinecraft.IsFirstMcInstanceListLoad = false;
                }
                if (!Loader.IsCanceled)
                {
                    if (ModMinecraft.McInstanceList.Any<KeyValuePair<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>>>((ModMinecraft._Closure$__.$I22-0 == null) ? (ModMinecraft._Closure$__.$I22-0 = (KeyValuePair<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>> v) => v.Key != ModMinecraft.McInstanceCardType.Error) : ModMinecraft._Closure$__.$I22-0))
                    {
                        string text = ModBase.ReadIni(input + "PCL.ini", "Version", "");
                        if (Operators.CompareString(text, "", false) != 0)
                        {
                            try
                            {
                                foreach (KeyValuePair<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>> keyValuePair in ModMinecraft.McInstanceList)
                                {
                                    try
                                    {
                                        foreach (ModMinecraft.McInstance mcInstance in keyValuePair.Value)
                                        {
                                            if (Operators.CompareString(mcInstance.Name, text, false) == 0 && mcInstance.State != ModMinecraft.McInstance.McInstanceState.Error)
                                            {
                                                ModMinecraft.McInstanceSelected = mcInstance;
                                                Logger.Info(string.Format("选择该文件夹储存的 Minecraft 版本：{0}", ModMinecraft.McInstanceSelected.PathVersion), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                                                return;
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        List<ModMinecraft.McInstance>.Enumerator enumerator2;
                                        ((IDisposable)enumerator2).Dispose();
                                    }
                                }
                            }
                            finally
                            {
                                Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>>.Enumerator enumerator;
                                ((IDisposable)enumerator).Dispose();
                            }
                        }
                        if (ModMinecraft.McInstanceList.First<KeyValuePair<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>>>().Value[0].State != ModMinecraft.McInstance.McInstanceState.Error)
                        {
                            ModMinecraft.McInstanceSelected = ModMinecraft.McInstanceList.First<KeyValuePair<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>>>().Value[0];
                            Logger.Info(string.Format("自动选择 Minecraft 版本：{0}", ModMinecraft.McInstanceSelected.PathVersion), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                        }
                    }
                    else
                    {
                        ModMinecraft.McInstanceSelected = null;
                        Logger.Info("未找到可用 Minecraft 版本", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                    }
                    if (Settings.Get<bool>("SystemDebugDelay", null))
                    {
                        Thread.Sleep(ModBase.RandomInteger(200, 3000));
                    }
                }
            }
            catch (Exception ex2)
            {
                if (!Loader.IsCanceled && !ex2.IsCanceled())
                {
                    ModBase.WriteIni(input + "PCL.ini", "InstanceCache", "");
                    Logger.Error(ex2, "加载 .minecraft 版本列表失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                }
            }
        }

        private static Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>> InitMcInstanceListWithCache(string Folder)
        {
            Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>> dictionary = new Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>>();
            checked
            {
                Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>> dictionary2;
                try
                {
                    int num = Conversions.ToInteger(ModBase.ReadIni(Folder + "PCL.ini", "CardCount", Conversions.ToString(-1)));
                    if (num == -1)
                    {
                        dictionary2 = null;
                    }
                    else
                    {
                        int num2 = num - 1;
                        for (int i = 0; i <= num2; i++)
                        {
                            ModMinecraft.McInstanceCardType mcInstanceCardType = (ModMinecraft.McInstanceCardType)Conversions.ToInteger(ModBase.ReadIni(Folder + "PCL.ini", "CardKey" + Conversions.ToString(i + 1), ":"));
                            List<ModMinecraft.McInstance> list = new List<ModMinecraft.McInstance>();
                            foreach (string text in ModBase.ReadIni(Folder + "PCL.ini", "CardValue" + Conversions.ToString(i + 1), ":").Split(":", false))
                            {
                                if (Operators.CompareString(text, "", false) != 0)
                                {
                                    string text2 = string.Format("{0}versions\\{1}\\", Folder, text);
                                    if (FileUtils.Exists(text2 + ".pclignore"))
                                    {
                                        if (!ModMinecraft.IsFirstMcInstanceListLoad)
                                        {
                                            Logger.Info(string.Format("跳过要求忽略的项目：{0}", text2), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                                            goto IL_06D1;
                                        }
                                        Logger.Info(string.Format("清理残留的忽略项目：{0}", text2), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                                        FileUtils.Delete(text2 + ".pclignore", false);
                                    }
                                    try
                                    {
                                        ModMinecraft._Closure$__23-0 CS$<>8__locals1 = new ModMinecraft._Closure$__23-0(CS$<>8__locals1);
                                        CS$<>8__locals1.$VB$Local_Instance = new ModMinecraft.McInstance(text2);
                                        list.Add(CS$<>8__locals1.$VB$Local_Instance);
                                        CS$<>8__locals1.$VB$Local_Instance.Info = ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\\Setup.ini", "CustomInfo", "");
                                        if (Operators.CompareString(CS$<>8__locals1.$VB$Local_Instance.Info, "", false) == 0)
                                        {
                                            CS$<>8__locals1.$VB$Local_Instance.Info = ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\\Setup.ini", "Info", CS$<>8__locals1.$VB$Local_Instance.Info);
                                        }
                                        CS$<>8__locals1.$VB$Local_Instance.Logo = ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\\Setup.ini", "Logo", CS$<>8__locals1.$VB$Local_Instance.Logo);
                                        CS$<>8__locals1.$VB$Local_Instance.State = (ModMinecraft.McInstance.McInstanceState)Conversions.ToInteger(ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\\Setup.ini", "State", Conversions.ToString((int)CS$<>8__locals1.$VB$Local_Instance.State)));
                                        CS$<>8__locals1.$VB$Local_Instance.IsStar = Conversions.ToBoolean(ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\\Setup.ini", "IsStar", Conversions.ToString(false)));
                                        CS$<>8__locals1.$VB$Local_Instance.DisplayType = (ModMinecraft.McInstanceCardType)Conversions.ToInteger(ModBase.ReadIni(Folder + "PCL\\Setup.ini", "DisplayType", Conversions.ToString(0)));
                                        McInstallManifest mcInstallManifest = ((CS$<>8__locals1.$VB$Local_Instance.State == ModMinecraft.McInstance.McInstanceState.Error) ? null : Configs.McInstallManifest.Get(CS$<>8__locals1.$VB$Local_Instance.Config));
                                        if (mcInstallManifest != null && mcInstallManifest.SchemaVersion <= 1 && mcInstallManifest.ManagedFiles.Any<McInstallManifest.ManagedFile>((ModMinecraft._Closure$__.$I23-0 == null) ? (ModMinecraft._Closure$__.$I23-0 = (McInstallManifest.ManagedFile File) => File.Component != null) : ModMinecraft._Closure$__.$I23-0))
                                        {
                                            McInstallManifest mcInstallManifest2 = mcInstallManifest.RefreshModComponents(CS$<>8__locals1.$VB$Local_Instance.PathIndie);
                                            if (mcInstallManifest2 != mcInstallManifest)
                                            {
                                                mcInstallManifest = mcInstallManifest2;
                                                Configs.McInstallManifest.Set(mcInstallManifest, CS$<>8__locals1.$VB$Local_Instance.Config);
                                                CS$<>8__locals1.$VB$Local_Instance.Config.Save();
                                            }
                                        }
                                        string text3 = ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\\Setup.ini", "VersionVanillaName", "Unknown");
                                        if (CS$<>8__locals1.$VB$Local_Instance.State != ModMinecraft.McInstance.McInstanceState.Error && (mcInstallManifest != null || Operators.CompareString(text3, "Unknown", false) != 0))
                                        {
                                            McVersion mcVersion;
                                            if (mcInstallManifest != null)
                                            {
                                                mcVersion = new McVersion(mcInstallManifest.Components)
                                                {
                                                    OptiFineAsMod = mcInstallManifest.ManagedFiles.Any<McInstallManifest.ManagedFile>((ModMinecraft._Closure$__.$I23-1 == null) ? (ModMinecraft._Closure$__.$I23-1 = (McInstallManifest.ManagedFile File) => Operators.CompareString(File.Component, "OptiFine", false) == 0) : ModMinecraft._Closure$__.$I23-1)
                                                };
                                            }
                                            else
                                            {
                                                VB$AnonymousDelegate_20<string, string> vb$AnonymousDelegate_ = delegate(string Key)
                                                {
                                                    string text6 = ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\\Setup.ini", Key, null);
                                                    if (Operators.CompareString(text6, "", false) != 0)
                                                    {
                                                        return text6;
                                                    }
                                                    return null;
                                                };
                                                mcVersion = new McVersion(text3, true)
                                                {
                                                    OptiFine = vb$AnonymousDelegate_("VersionOptiFine")
                                                };
                                                foreach (ModLoaders modLoaders in new ModLoaders[]
                                                {
                                                    ModLoaders.Fabric,
                                                    ModLoaders.Forge,
                                                    ModLoaders.NeoForge
                                                })
                                                {
                                                    string text4 = vb$AnonymousDelegate_(string.Format("Version{0}", modLoaders));
                                                    if (text4 != null)
                                                    {
                                                        mcVersion.ModLoaderVersions[modLoaders] = text4;
                                                    }
                                                }
                                                if (Conversions.ToBoolean(ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\\Setup.ini", "VersionLiteLoader", Conversions.ToString(false))))
                                                {
                                                    mcVersion.ModLoaderVersions[ModLoaders.LiteLoader] = "未知版本";
                                                }
                                            }
                                            mcVersion.ReleaseTime = Conversions.ToDate(ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\\Setup.ini", "ReleaseTime", Conversions.ToString(new DateTime(1970, 1, 1, 15, 0, 0))));
                                            mcVersion.IsFool = mcVersion.IsFool || CS$<>8__locals1.$VB$Local_Instance.State == ModMinecraft.McInstance.McInstanceState.Fool;
                                            mcVersion.IsSnapshot = mcVersion.IsSnapshot || CS$<>8__locals1.$VB$Local_Instance.State == ModMinecraft.McInstance.McInstanceState.Snapshot || CS$<>8__locals1.$VB$Local_Instance.Name.ContainsIgnoreCase("combat");
                                            CS$<>8__locals1.$VB$Local_Instance.Version = mcVersion;
                                            if (mcInstallManifest != null)
                                            {
                                                CS$<>8__locals1.$VB$Local_Instance.RefreshInfo();
                                            }
                                        }
                                        else
                                        {
                                            CS$<>8__locals1.$VB$Local_Instance.Load();
                                        }
                                        if (CS$<>8__locals1.$VB$Local_Instance.State == ModMinecraft.McInstance.McInstanceState.Error)
                                        {
                                            string info = CS$<>8__locals1.$VB$Local_Instance.Info;
                                            CS$<>8__locals1.$VB$Local_Instance.State = ModMinecraft.McInstance.McInstanceState.Original;
                                            CS$<>8__locals1.$VB$Local_Instance.Check();
                                            string text5 = ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\\Setup.ini", "CustomInfo", "");
                                            if (CS$<>8__locals1.$VB$Local_Instance.State == ModMinecraft.McInstance.McInstanceState.Original || (Operators.CompareString(text5, "", false) == 0 && Operators.CompareString(info, CS$<>8__locals1.$VB$Local_Instance.Info, false) != 0))
                                            {
                                                Logger.Info(string.Format("版本 {0} 的错误状态已变更，新的状态为：{1}", CS$<>8__locals1.$VB$Local_Instance.Name, CS$<>8__locals1.$VB$Local_Instance.Info), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                                                return null;
                                            }
                                        }
                                        if (Operators.CompareString(CS$<>8__locals1.$VB$Local_Instance.Logo, "", false) == 0)
                                        {
                                            Logger.Info(string.Format("版本 {0} 未被加载", CS$<>8__locals1.$VB$Local_Instance.Name), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                                            return null;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Warn(ex, string.Format("读取版本加载缓存失败（{0}）", text), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                                        return null;
                                    }
                                }
                                IL_06D1:;
                            }
                            if (list.Any<ModMinecraft.McInstance>())
                            {
                                dictionary.Add(mcInstanceCardType, list);
                            }
                        }
                        dictionary2 = dictionary;
                    }
                }
                catch (Exception ex2)
                {
                    Logger.Warn(ex2, "读取版本缓存失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                    dictionary2 = null;
                }
                return dictionary2;
            }
        }

        private static Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>> InitMcInstanceListWithoutCache(string Folder)
        {
            List<ModMinecraft.McInstance> list = new List<ModMinecraft.McInstance>();
            foreach (DirectoryInfo directoryInfo in DirectoryUtils.GetInfo(Folder + "versions").GetDirectories())
            {
                if (!directoryInfo.Exists || !directoryInfo.EnumerateFiles().Any<FileInfo>())
                {
                    Logger.Info(string.Format("跳过空文件夹：{0}", directoryInfo.FullName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                }
                else if ((Operators.CompareString(directoryInfo.Name, "cache", false) == 0 || Operators.CompareString(directoryInfo.Name, "BLClient", false) == 0 || Operators.CompareString(directoryInfo.Name, "PCL", false) == 0) && !FileUtils.Exists(directoryInfo.FullName + "\\" + directoryInfo.Name + ".json"))
                {
                    Logger.Info(string.Format("跳过可能不是版本文件夹的项目：{0}", directoryInfo.FullName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                }
                else
                {
                    string text = directoryInfo.FullName + "\\";
                    if (FileUtils.Exists(text + ".pclignore"))
                    {
                        if (!ModMinecraft.IsFirstMcInstanceListLoad)
                        {
                            Logger.Info(string.Format("跳过要求忽略的项目：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                            goto IL_016E;
                        }
                        Logger.Info(string.Format("清理残留的忽略项目：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                        FileUtils.Delete(text + ".pclignore", false);
                    }
                    ModMinecraft.McInstance mcInstance = new ModMinecraft.McInstance(text);
                    list.Add(mcInstance);
                    mcInstance.Load();
                }
                IL_016E:;
            }
            Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>> dictionary = new Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>>();
            try
            {
                Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>> dictionary2 = new Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>>();
                List<ModMinecraft.McInstance> list2 = new List<ModMinecraft.McInstance>();
                try
                {
                    foreach (ModMinecraft.McInstance mcInstance2 in list.ToList<ModMinecraft.McInstance>())
                    {
                        if (mcInstance2.IsStar && mcInstance2.DisplayType != ModMinecraft.McInstanceCardType.Hidden)
                        {
                            list2.Add(mcInstance2);
                            list.Remove(mcInstance2);
                        }
                    }
                }
                finally
                {
                    List<ModMinecraft.McInstance>.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
                if (list2.Any<ModMinecraft.McInstance>())
                {
                    dictionary2.Add(ModMinecraft.McInstanceCardType.Star, list2);
                }
                ModMinecraft.McInstanceFilter(ref list, ref dictionary2, new ModMinecraft.McInstance.McInstanceState[1], ModMinecraft.McInstanceCardType.Error);
                ModMinecraft.McInstanceFilter(ref list, ref dictionary2, new ModMinecraft.McInstance.McInstanceState[] { ModMinecraft.McInstance.McInstanceState.Fool }, ModMinecraft.McInstanceCardType.Fool);
                ModMinecraft.McInstanceFilter(ref list, ref dictionary2, new ModMinecraft.McInstance.McInstanceState[]
                {
                    ModMinecraft.McInstance.McInstanceState.Forge,
                    ModMinecraft.McInstance.McInstanceState.NeoForge,
                    ModMinecraft.McInstance.McInstanceState.LiteLoader,
                    ModMinecraft.McInstance.McInstanceState.Fabric
                }, ModMinecraft.McInstanceCardType.API);
                List<ModMinecraft.McInstance> list3 = new List<ModMinecraft.McInstance>();
                List<ModMinecraft.McInstance> list4 = new List<ModMinecraft.McInstance>();
                ModMinecraft.McInstanceFilter(ref list, new ModMinecraft.McInstance.McInstanceState[] { ModMinecraft.McInstance.McInstanceState.Old }, ref list4);
                ModMinecraft.McInstance mcInstance3 = list.Where<ModMinecraft.McInstance>((ModMinecraft._Closure$__.$I24-0 == null) ? (ModMinecraft._Closure$__.$I24-0 = (ModMinecraft.McInstance v) => v.State == ModMinecraft.McInstance.McInstanceState.Original || v.State == ModMinecraft.McInstance.McInstanceState.Snapshot) : ModMinecraft._Closure$__.$I24-0).MaxBy<ModMinecraft.McInstance, DateTime>((ModMinecraft._Closure$__.$I24-1 == null) ? (ModMinecraft._Closure$__.$I24-1 = (ModMinecraft.McInstance v) => v.ReleaseTime) : ModMinecraft._Closure$__.$I24-1);
                if (mcInstance3 != null && mcInstance3.State == ModMinecraft.McInstance.McInstanceState.Snapshot)
                {
                    list3.Add(mcInstance3);
                    list.Remove(mcInstance3);
                }
                ModMinecraft.McInstanceFilter(ref list, new ModMinecraft.McInstance.McInstanceState[] { ModMinecraft.McInstance.McInstanceState.Snapshot }, ref list4);
                Dictionary<string, ModMinecraft.McInstance> dictionary3 = new Dictionary<string, ModMinecraft.McInstance>();
                List<int> list5 = new List<int>();
                try
                {
                    foreach (ModMinecraft.McInstance mcInstance4 in list)
                    {
                        if (mcInstance4.Version.Valid)
                        {
                            if (!list5.Contains(mcInstance4.Version.Drop))
                            {
                                list5.Add(mcInstance4.Version.Drop);
                            }
                            string text2 = Conversions.ToString(mcInstance4.Version.Drop) + "-" + Conversions.ToString((int)mcInstance4.State);
                            if (!dictionary3.ContainsKey(text2))
                            {
                                dictionary3.Add(text2, mcInstance4);
                            }
                            else if (mcInstance4.State == ModMinecraft.McInstance.McInstanceState.OptiFine)
                            {
                                if (mcInstance4.Version.OptiFineCode > dictionary3[text2].Version.OptiFineCode)
                                {
                                    dictionary3[text2] = mcInstance4;
                                }
                            }
                            else if (DateTime.Compare(mcInstance4.ReleaseTime, dictionary3[text2].ReleaseTime) > 0)
                            {
                                dictionary3[text2] = mcInstance4;
                            }
                        }
                    }
                }
                finally
                {
                    List<ModMinecraft.McInstance>.Enumerator enumerator2;
                    ((IDisposable)enumerator2).Dispose();
                }
                try
                {
                    foreach (int num in list5)
                    {
                        if (dictionary3.ContainsKey(Conversions.ToString(num) + "-" + Conversions.ToString(4)) && dictionary3.ContainsKey(Conversions.ToString(num) + "-" + Conversions.ToString(1)))
                        {
                            ModMinecraft.McInstance mcInstance5 = dictionary3[Conversions.ToString(num) + "-" + Conversions.ToString(1)];
                            ModMinecraft.McInstance mcInstance6 = dictionary3[Conversions.ToString(num) + "-" + Conversions.ToString(4)];
                            if (mcInstance5.Version.Drop > mcInstance6.Version.Drop)
                            {
                                list3.Add(mcInstance5);
                                list.Remove(mcInstance5);
                            }
                            list3.Add(mcInstance6);
                            list.Remove(mcInstance6);
                        }
                        else if (dictionary3.ContainsKey(Conversions.ToString(num) + "-" + Conversions.ToString(4)))
                        {
                            list3.Add(dictionary3[Conversions.ToString(num) + "-" + Conversions.ToString(4)]);
                            list.Remove(dictionary3[Conversions.ToString(num) + "-" + Conversions.ToString(4)]);
                        }
                        else if (dictionary3.ContainsKey(Conversions.ToString(num) + "-" + Conversions.ToString(1)))
                        {
                            list3.Add(dictionary3[Conversions.ToString(num) + "-" + Conversions.ToString(1)]);
                            list.Remove(dictionary3[Conversions.ToString(num) + "-" + Conversions.ToString(1)]);
                        }
                    }
                }
                finally
                {
                    List<int>.Enumerator enumerator3;
                    ((IDisposable)enumerator3).Dispose();
                }
                list4.AddRange(list);
                if (list3.Any<ModMinecraft.McInstance>())
                {
                    dictionary2.Add(ModMinecraft.McInstanceCardType.OriginalLike, list3);
                }
                if (list4.Any<ModMinecraft.McInstance>())
                {
                    dictionary2.Add(ModMinecraft.McInstanceCardType.Rubbish, list4);
                }
                try
                {
                    foreach (KeyValuePair<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>> keyValuePair in dictionary2)
                    {
                        try
                        {
                            foreach (ModMinecraft.McInstance mcInstance7 in keyValuePair.Value)
                            {
                                ModMinecraft.McInstanceCardType mcInstanceCardType = ((mcInstance7.DisplayType == ModMinecraft.McInstanceCardType.Auto || keyValuePair.Key == ModMinecraft.McInstanceCardType.Star) ? keyValuePair.Key : mcInstance7.DisplayType);
                                if (!dictionary.ContainsKey(mcInstanceCardType))
                                {
                                    dictionary.Add(mcInstanceCardType, new List<ModMinecraft.McInstance>());
                                }
                                dictionary[mcInstanceCardType].Add(mcInstance7);
                            }
                        }
                        finally
                        {
                            List<ModMinecraft.McInstance>.Enumerator enumerator5;
                            ((IDisposable)enumerator5).Dispose();
                        }
                    }
                }
                finally
                {
                    Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>>.Enumerator enumerator4;
                    ((IDisposable)enumerator4).Dispose();
                }
            }
            catch (Exception ex)
            {
                dictionary.Clear();
                Logger.Error(ex, "分类版本列表失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
            }
            Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>> dictionary4 = new Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>>();
            ModMinecraft.McInstanceCardType[] array = new ModMinecraft.McInstanceCardType[]
            {
                ModMinecraft.McInstanceCardType.Star,
                ModMinecraft.McInstanceCardType.API,
                ModMinecraft.McInstanceCardType.OriginalLike,
                ModMinecraft.McInstanceCardType.Rubbish,
                ModMinecraft.McInstanceCardType.Fool,
                ModMinecraft.McInstanceCardType.Error,
                ModMinecraft.McInstanceCardType.Hidden
            };
            checked
            {
                for (int j = 0; j < array.Length; j++)
                {
                    string text3 = Conversions.ToString((int)array[j]);
                    if (dictionary.ContainsKey((ModMinecraft.McInstanceCardType)Conversions.ToInteger(text3)))
                    {
                        dictionary4.Add((ModMinecraft.McInstanceCardType)Conversions.ToInteger(text3), dictionary[(ModMinecraft.McInstanceCardType)Conversions.ToInteger(text3)]);
                    }
                }
                dictionary = dictionary4;
                foreach (ModMinecraft.McInstanceCardType mcInstanceCardType2 in new ModMinecraft.McInstanceCardType[]
                {
                    ModMinecraft.McInstanceCardType.Star,
                    ModMinecraft.McInstanceCardType.API,
                    ModMinecraft.McInstanceCardType.OriginalLike,
                    ModMinecraft.McInstanceCardType.Rubbish,
                    ModMinecraft.McInstanceCardType.Fool
                })
                {
                    ModMinecraft._Closure$__24-0 CS$<>8__locals1 = new ModMinecraft._Closure$__24-0(CS$<>8__locals1);
                    if (dictionary.ContainsKey(mcInstanceCardType2))
                    {
                        CS$<>8__locals1.$VB$Local_GetComponentCode = ((ModMinecraft._Closure$__.$I24-2 == null) ? (ModMinecraft._Closure$__.$I24-2 = delegate(ModMinecraft.McInstance Instance)
                        {
                            int num3;
                            if (Instance.Version.ForgelikeCode > 0)
                            {
                                num3 = Instance.Version.ForgelikeCode;
                            }
                            else if (Instance.Version.HasOptiFine)
                            {
                                num3 = Instance.Version.OptiFineCode;
                            }
                            else
                            {
                                num3 = 0;
                            }
                            return num3;
                        }) : ModMinecraft._Closure$__.$I24-2);
                        dictionary[mcInstanceCardType2] = dictionary[mcInstanceCardType2].SortByComparison<ModMinecraft.McInstance>(delegate(ModMinecraft.McInstance Left, ModMinecraft.McInstance Right)
                        {
                            bool flag;
                            if ((Left.ReleaseTime.Year >= 2000 || Right.ReleaseTime.Year >= 2000) && DateTime.Compare(Left.ReleaseTime, Right.ReleaseTime) != 0)
                            {
                                flag = DateTime.Compare(Left.ReleaseTime, Right.ReleaseTime) > 0;
                            }
                            else if (Left.Version.ModLoaders.HasFlagF(ModLoaders.Fabric) != Right.Version.ModLoaders.HasFlagF(ModLoaders.Fabric))
                            {
                                flag = Left.Version.ModLoaders.HasFlagF(ModLoaders.Fabric);
                            }
                            else if (Left.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge) != Right.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge))
                            {
                                flag = Left.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge);
                            }
                            else if (Left.Version.ModLoaders.HasFlagF(ModLoaders.Forge) != Right.Version.ModLoaders.HasFlagF(ModLoaders.Forge))
                            {
                                flag = Left.Version.ModLoaders.HasFlagF(ModLoaders.Forge);
                            }
                            else if (Left.Version.HasOptiFine != Right.Version.HasOptiFine)
                            {
                                flag = Left.Version.HasOptiFine;
                            }
                            else if (Left.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader) != Right.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader))
                            {
                                flag = Left.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader);
                            }
                            else if (CS$<>8__locals1.$VB$Local_GetComponentCode(Left) != CS$<>8__locals1.$VB$Local_GetComponentCode(Right))
                            {
                                flag = CS$<>8__locals1.$VB$Local_GetComponentCode(Left) > CS$<>8__locals1.$VB$Local_GetComponentCode(Right);
                            }
                            else
                            {
                                flag = Operators.CompareString(Left.Name, Right.Name, false) > 0;
                            }
                            return flag;
                        });
                    }
                }
                ModBase.WriteIni(Folder + "PCL.ini", "CardCount", Conversions.ToString(dictionary.Count));
                int num2 = dictionary.Count - 1;
                for (int l = 0; l <= num2; l++)
                {
                    ModBase.WriteIni(Folder + "PCL.ini", "CardKey" + Conversions.ToString(l + 1), Conversions.ToString((int)dictionary.Keys.ElementAtOrDefault<ModMinecraft.McInstanceCardType>(l)));
                    string text4 = "";
                    try
                    {
                        foreach (ModMinecraft.McInstance mcInstance8 in dictionary.Values.ElementAtOrDefault<List<ModMinecraft.McInstance>>(l))
                        {
                            text4 = text4 + mcInstance8.Name + ":";
                        }
                    }
                    finally
                    {
                        List<ModMinecraft.McInstance>.Enumerator enumerator6;
                        ((IDisposable)enumerator6).Dispose();
                    }
                    ModBase.WriteIni(Folder + "PCL.ini", "CardValue" + Conversions.ToString(l + 1), text4);
                }
                return dictionary;
            }
        }

        private static void McInstanceFilter(ref List<ModMinecraft.McInstance> InstanceList, ref Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>> Target, ModMinecraft.McInstance.McInstanceState[] Formula, ModMinecraft.McInstanceCardType CardType)
        {
            List<ModMinecraft.McInstance> list = InstanceList.Where<ModMinecraft.McInstance>((ModMinecraft.McInstance v) => Formula.Contains(v.State)).ToList<ModMinecraft.McInstance>();
            if (list.Any<ModMinecraft.McInstance>())
            {
                Target.Add(CardType, list);
                InstanceList = InstanceList.Except<ModMinecraft.McInstance>(list).ToList<ModMinecraft.McInstance>();
            }
        }

        private static void McInstanceFilter(ref List<ModMinecraft.McInstance> InstanceList, ModMinecraft.McInstance.McInstanceState[] Formula, ref List<ModMinecraft.McInstance> KeepList)
        {
            KeepList.AddRange(InstanceList.Where<ModMinecraft.McInstance>((ModMinecraft.McInstance v) => Formula.Contains(v.State)));
            if (KeepList.Any<ModMinecraft.McInstance>())
            {
                InstanceList = InstanceList.Except<ModMinecraft.McInstance>(KeepList).ToList<ModMinecraft.McInstance>();
            }
        }

        public static ModMinecraft.McSkinInfo McSkinSelect()
        {
            string text = Dialogs.SelectFile("选择皮肤文件", false, null, new OneOf<IEnumerable<ValueTuple<string[], string>>, string>?(new ValueTuple<string[], string>[]
            {
                new ValueTuple<string[], string>(new string[] { "png", "jpg", "jpeg", "webp" }, "皮肤文件")
            })).FirstOrDefault<string>();
            ModMinecraft.McSkinInfo mcSkinInfo;
            if (string.IsNullOrEmpty(text))
            {
                mcSkinInfo = new ModMinecraft.McSkinInfo
                {
                    IsVaild = false
                };
            }
            else
            {
                try
                {
                    MyBitmap myBitmap = new MyBitmap(text);
                    if (myBitmap.Pic.Width != 64 || (myBitmap.Pic.Height != 32 && myBitmap.Pic.Height != 64))
                    {
                        ModMain.Hint("皮肤图片大小应为 64x32 像素或 64x64 像素！", ModMain.HintType.Red, true);
                        return new ModMinecraft.McSkinInfo
                        {
                            IsVaild = false
                        };
                    }
                    FileInfo info = FileUtils.GetInfo(text);
                    if (info.Length > 24576L)
                    {
                        ModMain.Hint("皮肤文件大小需小于 24 KB，而所选文件大小为 " + Conversions.ToString(Math.Round((double)info.Length / 1024.0, 2)) + " KB", ModMain.HintType.Red, true);
                        return new ModMinecraft.McSkinInfo
                        {
                            IsVaild = false
                        };
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "皮肤文件存在错误", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                    return new ModMinecraft.McSkinInfo
                    {
                        IsVaild = false
                    };
                }
                int num = ModMain.MyMsgBox("此皮肤为 Steve 模型（粗手臂）还是 Alex 模型（细手臂）？", "选择皮肤种类", "Steve 模型", "Alex 模型", "我不知道", false, false, false, null, null, null);
                if (num == 3)
                {
                    ModMain.Hint("请在皮肤下载页面确认皮肤种类后再使用此皮肤！", ModMain.HintType.Blue, true);
                    mcSkinInfo = new ModMinecraft.McSkinInfo
                    {
                        IsVaild = false
                    };
                }
                else
                {
                    mcSkinInfo = new ModMinecraft.McSkinInfo
                    {
                        IsVaild = true,
                        IsSlim = (num == 2),
                        LocalFile = text
                    };
                }
            }
            return mcSkinInfo;
        }

        public static string McSkinGetAddress(string UUID, string Type)
        {
            if (Operators.CompareString(UUID, "", false) == 0)
            {
                throw new Exception("UUID 为空。");
            }
            if (UUID.StartsWithF("00000", false) && Operators.CompareString(Type, "Auth", false) != 0)
            {
                throw new Exception("离线 UUID 无正版皮肤文件：" + UUID);
            }
            string text = ModBase.ReadIni(ModBase.PathTemp + "Cache\\Skin\\Index" + Type + ".ini", UUID, "");
            string text2;
            if (Operators.CompareString(text, "", false) != 0)
            {
                text2 = text;
            }
            else
            {
                string text3;
                if (Operators.CompareString(Type, "Mojang", false) != 0 && Operators.CompareString(Type, "Ms", false) != 0)
                {
                    if (Operators.CompareString(Type, "Nide", false) != 0)
                    {
                        if (Operators.CompareString(Type, "Auth", false) != 0)
                        {
                            throw new ArgumentException("皮肤地址种类无效：" + (Type ?? "null"));
                        }
                        text3 = ((ModMinecraft.McInstanceSelected == null) ? Settings.Get<string>("CacheAuthServerServer", null) : Settings.Get<string>("VersionServerAuthServer", ModMinecraft.McInstanceSelected)) + "/sessionserver/session/minecraft/profile/";
                    }
                    else
                    {
                        text3 = "https://auth.mc-user.com:233/" + ((ModMinecraft.McInstanceSelected == null) ? Settings.Get<string>("CacheNideServer", null) : Settings.Get<string>("VersionServerNide", ModMinecraft.McInstanceSelected)) + "/sessionserver/session/minecraft/profile/";
                    }
                }
                else
                {
                    text3 = "https://sessionserver.mojang.com/session/minecraft/profile/";
                }
                string text4 = ModNet.NetRequestByClientRetry(text3 + UUID, null, null, null, null, "*/*", null, true, null, false);
                if (Operators.CompareString(text4, "", false) == 0)
                {
                    throw new Exception("皮肤返回值为空，可能是未设置自定义皮肤的用户");
                }
                string text5;
                try
                {
                    try
                    {
                        foreach (JToken jtoken in text4.DeserializeJson()["properties"])
                        {
                            if ((string)jtoken["name"] == "textures")
                            {
                                text5 = (string)jtoken["value"];
                                goto IL_021B;
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
                    throw new Exception("未从皮肤返回值中找到符合条件的 Property");
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, string.Format("无法完成解析的皮肤返回值，可能是未设置自定义皮肤的用户：{0}", text4), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                    throw new Exception("皮肤返回值中不包含皮肤数据项，可能是未设置自定义皮肤的用户", ex);
                }
                IL_021B:
                text4 = Encoding.UTF8.GetString(Convert.FromBase64String(text5));
                JObject jobject = (JObject)text4.Lower().DeserializeJson();
                if (jobject["textures"] == null || jobject["textures"]["skin"] == null || jobject["textures"]["skin"]["url"] == null)
                {
                    throw new Exception("用户未设置自定义皮肤");
                }
                string text6 = jobject["textures"]["skin"]["url"].ToString();
                text5 = (text6.Contains("minecraft.net/") ? text6.Replace("http://", "https://") : text6);
                ModBase.WriteIni(ModBase.PathTemp + "Cache\\Skin\\Index" + Type + ".ini", UUID, text5);
                Logger.Info(string.Format("UUID {0} 对应的皮肤文件为 {1}", UUID, text5), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                text2 = text5;
            }
            return text2;
        }

        public static string McSkinDownload(string Address)
        {
            PathUtils.GetLastPart(Address);
            string text = ModBase.PathTemp + "Cache\\Skin\\" + Conversions.ToString(Address.GetStableHashCode()) + ".png";
            object mcSkinDownloadLock = ModMinecraft.McSkinDownloadLock;
            ObjectFlowControl.CheckForSyncLockOnValueType(mcSkinDownloadLock);
            string text2;
            lock (mcSkinDownloadLock)
            {
                if (!FileUtils.Exists(text))
                {
                    ModNet.NetDownloadByClient(Address, text + ".PCLDownloading", false);
                    FileUtils.Move(text + ".PCLDownloading", text, false);
                    Logger.Info(string.Format("皮肤下载成功：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                }
                text2 = text;
            }
            return text2;
        }

        public static string McSkinSex(string Uuid)
        {
            string text;
            if (Uuid.Length != 32)
            {
                text = "Steve";
            }
            else
            {
                int num = int.Parse(Conversions.ToString(Uuid[7]), NumberStyles.AllowHexSpecifier);
                int num2 = int.Parse(Conversions.ToString(Uuid[15]), NumberStyles.AllowHexSpecifier);
                int num3 = int.Parse(Conversions.ToString(Uuid[23]), NumberStyles.AllowHexSpecifier);
                int num4 = int.Parse(Conversions.ToString(Uuid[31]), NumberStyles.AllowHexSpecifier);
                text = (((num ^ num2 ^ num3 ^ num4) % 2 != 0) ? "Alex" : "Steve");
            }
            return text;
        }

        public static bool McJsonRuleCheck(JToken RuleToken)
        {
            bool flag;
            if (RuleToken == null)
            {
                flag = true;
            }
            else
            {
                bool flag2 = false;
                try
                {
                    foreach (JToken jtoken in RuleToken)
                    {
                        bool flag3 = true;
                        if (jtoken["os"] != null)
                        {
                            if (jtoken["os"]["name"] != null)
                            {
                                string text = jtoken["os"]["name"].ToString();
                                if (Operators.CompareString(text, "unknown", false) != 0)
                                {
                                    if (Operators.CompareString(text, "windows", false) == 0)
                                    {
                                        if (jtoken["os"]["version"] != null)
                                        {
                                            flag3 = flag3 && ModMinecraft.OSVersion.RegexCheck(jtoken["os"]["version"].ToString(), RegexOptions.None);
                                        }
                                    }
                                    else
                                    {
                                        flag3 = false;
                                    }
                                }
                            }
                            if (jtoken["os"]["arch"] != null)
                            {
                                flag3 = flag3 && Operators.CompareString(jtoken["os"]["arch"].ToString(), "x86", false) != 0 && !jtoken["os"]["arch"].Contains("arm");
                            }
                        }
                        if (!Information.IsNothing(jtoken["features"]))
                        {
                            flag3 = flag3 && Information.IsNothing(jtoken["features"]["is_demo_user"]);
                            if (((IEnumerable<JToken>)((JObject)jtoken["features"]).Children()).Any<JToken>((ModMinecraft._Closure$__.$IR35-2 == null) ? (ModMinecraft._Closure$__.$IR35-2 = (JToken a0) => ((ModMinecraft._Closure$__.$I35-0 == null) ? (ModMinecraft._Closure$__.$I35-0 = (JProperty j) => j.Name.Contains("quick_play")) : ModMinecraft._Closure$__.$I35-0)((JProperty)a0)) : ModMinecraft._Closure$__.$IR35-2))
                            {
                                flag3 = false;
                            }
                        }
                        if (Operators.CompareString(jtoken["action"].ToString(), "allow", false) == 0)
                        {
                            if (flag3)
                            {
                                flag2 = true;
                            }
                        }
                        else if (flag3)
                        {
                            flag2 = false;
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
                flag = flag2;
            }
            return flag;
        }

        public static List<ModMinecraft.McLibEntry> McLibListGet(ModMinecraft.McInstance Instance, bool IncludeMainJar)
        {
            Logger.Info(string.Format("获取支持库列表：{0}", Instance.Name), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
            List<ModMinecraft.McLibEntry> list = ModMinecraft.McLibListGetWithJson(Instance.JsonObject, false, null);
            if (IncludeMainJar)
            {
                JToken jtoken = Instance.JsonObject["jar"];
                string text = ((jtoken != null) ? jtoken.ToString() : null);
                ModMinecraft.McInstance mcInstance2;
                if (Instance.IsHmclFormatJson || text == null)
                {
                    ModMinecraft.McInstance mcInstance = Instance;
                    if (Instance.Version.ModLoaders.HasFlagF(ModLoaders.Forge) || Instance.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge))
                    {
                        if (Instance.Version.Vanilla.Major >= 17)
                        {
                            goto IL_00E6;
                        }
                    }
                    while (Operators.CompareString(mcInstance.InheritName, "", false) != 0 && Operators.CompareString(mcInstance.InheritName, mcInstance.Name, false) != 0)
                    {
                        mcInstance = new ModMinecraft.McInstance(ModMinecraft.McFolderSelected + "versions\\" + mcInstance.InheritName + "\\");
                    }
                    IL_00E6:
                    mcInstance2 = new ModMinecraft.McInstance(mcInstance.PathVersion);
                }
                else
                {
                    mcInstance2 = new ModMinecraft.McInstance(text);
                }
                if (!FileUtils.Exists(mcInstance2.PathVersion + mcInstance2.Name + ".json"))
                {
                    mcInstance2 = Instance;
                    Logger.Warn(string.Format("可能缺少前置版本 {0}，找不到对应的 json 文件", mcInstance2.Name), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                }
                string text2;
                string text3;
                if (mcInstance2.JsonObject["downloads"] != null && mcInstance2.JsonObject["downloads"]["client"] != null)
                {
                    text2 = (string)mcInstance2.JsonObject["downloads"]["client"]["url"];
                    text3 = (string)mcInstance2.JsonObject["downloads"]["client"]["sha1"];
                }
                else
                {
                    text2 = null;
                    text3 = null;
                }
                list.Add(new ModMinecraft.McLibEntry
                {
                    LocalPath = mcInstance2.PathVersion + mcInstance2.Name + ".jar",
                    Size = 0L,
                    IsNatives = false,
                    Url = text2,
                    SHA1 = text3
                });
            }
            return list;
        }

        public static List<ModMinecraft.McLibEntry> McLibListGetWithJson(JObject JsonObject, bool KeepSameNameDifferentVersionResult = false, string CustomMcFolder = null)
        {
            CustomMcFolder = CustomMcFolder ?? ModMinecraft.McFolderSelected;
            List<ModMinecraft.McLibEntry> list = new List<ModMinecraft.McLibEntry>();
            checked
            {
                try
                {
                    foreach (JToken jtoken in ((JArray)JsonObject["libraries"]).Children())
                    {
                        JObject jobject = (JObject)jtoken;
                        for (int i = jobject.Properties().Count<JProperty>() - 1; i >= 0; i += -1)
                        {
                            if (jobject.Properties().ElementAtOrDefault<JProperty>(i).Value.Type == 10)
                            {
                                jobject.Remove(jobject.Properties().ElementAtOrDefault<JProperty>(i).Name);
                            }
                        }
                        if (ModMinecraft.McJsonRuleCheck(jobject["rules"]))
                        {
                            string text = (string)jobject["url"];
                            if (text != null)
                            {
                                text += ModMinecraft.McLibGet((string)jobject["name"], false, true, CustomMcFolder).Replace("\\", "/");
                            }
                            if (jobject["natives"] == null)
                            {
                                string text2 = ModMinecraft.McLibGet((string)jobject["name"], true, false, CustomMcFolder);
                                try
                                {
                                    if (jobject["downloads"] != null && jobject["downloads"]["artifact"] != null)
                                    {
                                        List<ModMinecraft.McLibEntry> list2 = list;
                                        ModMinecraft.McLibEntry mcLibEntry = new ModMinecraft.McLibEntry();
                                        mcLibEntry.OriginalName = (string)jobject["name"];
                                        mcLibEntry.Url = (string)(jobject["downloads"]["artifact"]["url"] ?? text);
                                        mcLibEntry.LocalPath = ((jobject["downloads"]["artifact"]["path"] == null) ? ModMinecraft.McLibGet((string)jobject["name"], true, false, CustomMcFolder) : (CustomMcFolder + "libraries\\" + jobject["downloads"]["artifact"]["path"].ToString().Replace("/", "\\")));
                                        mcLibEntry.Size = (long)Math.Round(ModBase.Val(jobject["downloads"]["artifact"]["size"].ToString()));
                                        mcLibEntry.IsNatives = false;
                                        ModMinecraft.McLibEntry mcLibEntry2 = mcLibEntry;
                                        JToken jtoken2 = jobject["downloads"]["artifact"]["sha1"];
                                        mcLibEntry2.SHA1 = ((jtoken2 != null) ? jtoken2.ToString() : null);
                                        list2.Add(mcLibEntry);
                                    }
                                    else
                                    {
                                        list.Add(new ModMinecraft.McLibEntry
                                        {
                                            OriginalName = (string)jobject["name"],
                                            Url = text,
                                            LocalPath = text2,
                                            Size = 0L,
                                            IsNatives = false,
                                            SHA1 = null
                                        });
                                    }
                                    continue;
                                }
                                catch (Exception ex)
                                {
                                    Logger.Warn(ex, string.Format("处理实际支持库列表失败（无 Natives，{0}）", jobject["name"] ?? "Nothing"), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                                    list.Add(new ModMinecraft.McLibEntry
                                    {
                                        OriginalName = (string)jobject["name"],
                                        Url = text,
                                        LocalPath = text2,
                                        Size = 0L,
                                        IsNatives = false,
                                        SHA1 = null
                                    });
                                    continue;
                                }
                            }
                            if (jobject["natives"]["windows"] != null)
                            {
                                string text3 = (string)jobject["natives"]["windows"];
                                try
                                {
                                    if (jobject["downloads"] != null && jobject["downloads"]["classifiers"] != null && jobject["downloads"]["classifiers"][text3] != null)
                                    {
                                        JToken jtoken3 = jobject["downloads"]["classifiers"][text3];
                                        list.Add(new ModMinecraft.McLibEntry
                                        {
                                            OriginalName = (string)jobject["name"],
                                            Url = (string)(jtoken3["url"] ?? text),
                                            LocalPath = ((jtoken3["path"] == null) ? ModMinecraft.McLibGet((string)jobject["name"], true, false, CustomMcFolder).Replace(".jar", "-" + text3 + ".jar").Replace("${arch}", Environment.Is64BitOperatingSystem ? "64" : "32") : string.Format("{0}libraries\\{1}", CustomMcFolder, jtoken3["path"].ToString().Replace("/", "\\"))),
                                            Size = (long)Math.Round(ModBase.Val(jtoken3["size"].ToString())),
                                            IsNatives = true,
                                            SHA1 = jtoken3["sha1"].ToString()
                                        });
                                    }
                                    else
                                    {
                                        list.Add(new ModMinecraft.McLibEntry
                                        {
                                            OriginalName = (string)jobject["name"],
                                            Url = text,
                                            Size = 0L,
                                            IsNatives = true,
                                            SHA1 = null,
                                            LocalPath = ModMinecraft.McLibGet((string)jobject["name"], true, false, CustomMcFolder).Replace(".jar", string.Format("-{0}.jar", text3)).Replace("${arch}", Environment.Is64BitOperatingSystem ? "64" : "32")
                                        });
                                    }
                                }
                                catch (Exception ex2)
                                {
                                    Logger.Warn(ex2, string.Format("处理实际支持库列表失败（有 Natives，{0}）", jobject["name"] ?? "Nothing"), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                                    list.Add(new ModMinecraft.McLibEntry
                                    {
                                        OriginalName = (string)jobject["name"],
                                        Url = text,
                                        Size = 0L,
                                        IsNatives = true,
                                        SHA1 = null,
                                        LocalPath = ModMinecraft.McLibGet((string)jobject["name"], true, false, CustomMcFolder).Replace(".jar", string.Format("-{0}.jar", text3)).Replace("${arch}", Environment.Is64BitOperatingSystem ? "64" : "32")
                                    });
                                }
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
                Dictionary<string, ModMinecraft.McLibEntry> dictionary = new Dictionary<string, ModMinecraft.McLibEntry>();
                VB$AnonymousDelegate_23<ModMinecraft.McLibEntry, string> vb$AnonymousDelegate_ = ((ModMinecraft._Closure$__.$I38-0 == null) ? (ModMinecraft._Closure$__.$I38-0 = (ModMinecraft.McLibEntry Token) => PathUtils.GetLastPart(PathUtils.RemoveLastPart(Token.LocalPath))) : ModMinecraft._Closure$__.$I38-0);
                int num = list.Count - 1;
                for (int j = 0; j <= num; j++)
                {
                    string text4 = list[j].Name + list[j].IsNatives.ToString();
                    if (dictionary.ContainsKey(text4))
                    {
                        string text5 = vb$AnonymousDelegate_(list[j]);
                        string text6 = vb$AnonymousDelegate_(dictionary[text4]);
                        if (Operators.CompareString(text5, text6, false) != 0 && KeepSameNameDifferentVersionResult)
                        {
                            Logger.Info(string.Format("发现疑似重复的支持库：{0} ({1}) 与 {2} ({3})", new object[]
                            {
                                list[j],
                                text5,
                                dictionary[text4],
                                text6
                            }), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                            dictionary.Add(text4 + Conversions.ToString(ModBase.GetUuid()), list[j]);
                        }
                        else
                        {
                            Logger.Info(string.Format("发现重复的支持库：{0} ({1}) 与 {2} ({3})，已忽略其中之一", new object[]
                            {
                                list[j],
                                text5,
                                dictionary[text4],
                                text6
                            }), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                            if (McUtils.CompareVersionGE(text5, text6))
                            {
                                dictionary[text4] = list[j];
                            }
                        }
                    }
                    else
                    {
                        dictionary.Add(text4, list[j]);
                    }
                }
                return dictionary.Values.ToList<ModMinecraft.McLibEntry>();
            }
        }

        public static List<ModNet.NetFile> McLibNetFilesFromInstance(ModMinecraft.McInstance Instance)
        {
            if (!Instance.IsLoaded)
            {
                Instance.Load();
            }
            List<ModNet.NetFile> list = new List<ModNet.NetFile>();
            try
            {
                ModNet.NetFile netFile = ModDownload.DlClientJarGet(Instance, true);
                if (netFile != null)
                {
                    list.Add(netFile);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "版本缺失主 jar 文件所必须的信息", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
            }
            list.AddRange(ModMinecraft.McLibNetFilesFromTokens(ModMinecraft.McLibListGet(Instance, false), null));
            if (Settings.Get<int>("VersionServerLogin", Instance) == 3)
            {
                string text = ModBase.PathPure.Value + "nide8auth.jar";
                try
                {
                    Logger.Info("开始获取统一通行证下载信息", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                    JObject jobject = (JObject)ModNet.NetRequestByClientRetry("https://auth.mc-user.com:233/" + Settings.Get<string>("VersionServerNide", Instance), null, null, null, null, "*/*", null, true, null, false).DeserializeJson();
                    list.Add(new ModNet.NetFile(new string[] { "https://login.mc-user.com:233/index/jar" }, text, new FileChecker
                    {
                        Hash = jobject["jarHash"].ToString()
                    }, false));
                }
                catch (Exception ex2)
                {
                    if (!FileUtils.Exists(text))
                    {
                        throw new Exception("获取统一通行证下载信息失败", ex2);
                    }
                    Logger.Warn(ex2, "获取统一通行证下载信息失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                }
            }
            if (Settings.Get<int>("VersionServerLogin", Instance) == 4)
            {
                string text2 = ModBase.PathPure.Value + "authlib-injector.jar";
                try
                {
                    Logger.Info("开始获取 Authlib-Injector 下载信息", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                    JObject jobject2 = (JObject)ModNet.NetRequestByClientRetry("https://authlib-injector.yushi.moe/artifact/latest.json", null, "https://bmclapi2.bangbang93.com/mirrors/authlib-injector/artifact/latest.json", null, null, "*/*", null, true, null, false).DeserializeJson();
                    string text3 = jobject2["download_url"].ToString().Replace("bmclapi2.bangbang93.com/mirrors/authlib-injector", "authlib-injector.yushi.moe");
                    list.Add(new ModNet.NetFile(new string[]
                    {
                        text3,
                        text3.Replace("authlib-injector.yushi.moe", "bmclapi2.bangbang93.com/mirrors/authlib-injector")
                    }, text2, new FileChecker
                    {
                        Hash = jobject2["checksums"]["sha256"].ToString()
                    }, false));
                }
                catch (Exception ex3)
                {
                    if (!FileUtils.Exists(text2))
                    {
                        throw new Exception("获取 Authlib-Injector 下载信息失败", ex3);
                    }
                    Logger.Warn(ex3, "获取 Authlib-Injector 下载信息失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                }
            }
            if (Conversions.ToBoolean(ModMinecraft.ShouldIgnoreFileCheck(Instance)))
            {
                Logger.Warn("设置要求尽量忽略文件检查，这可能会保留有误的文件", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                list = list.Where<ModNet.NetFile>((ModMinecraft._Closure$__.$I39-0 == null) ? (ModMinecraft._Closure$__.$I39-0 = delegate(ModNet.NetFile f)
                {
                    bool flag;
                    if (FileUtils.Exists(f.LocalPath))
                    {
                        Logger.Warn(string.Format("跳过下载的支持库文件：{0}", f.LocalPath), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }
                    return flag;
                }) : ModMinecraft._Closure$__.$I39-0).ToList<ModNet.NetFile>();
            }
            return list;
        }

        public static List<ModNet.NetFile> McLibNetFilesFromTokens(List<ModMinecraft.McLibEntry> Libs, string CustomMcFolder = null)
        {
            CustomMcFolder = CustomMcFolder ?? ModMinecraft.McFolderSelected;
            List<ModNet.NetFile> list = new List<ModNet.NetFile>();
            try
            {
                foreach (ModMinecraft.McLibEntry mcLibEntry in Libs)
                {
                    FileChecker fileChecker;
                    if (mcLibEntry.Name.ContainsIgnoreCase("labymod"))
                    {
                        fileChecker = new FileChecker();
                    }
                    else
                    {
                        fileChecker = new FileChecker
                        {
                            ActualSize = ((mcLibEntry.Size == 0L) ? (-1L) : mcLibEntry.Size),
                            Hash = mcLibEntry.SHA1
                        };
                    }
                    List<string> list2 = new List<string>();
                    if (mcLibEntry.Url == null && Operators.CompareString(mcLibEntry.Name, "net.minecraftforge:forge:universal", false) == 0)
                    {
                        mcLibEntry.Url = "https://maven.minecraftforge.net" + mcLibEntry.LocalPath.Replace(CustomMcFolder + "libraries", "").Replace("\\", "/");
                    }
                    if (mcLibEntry.Url != null)
                    {
                        list2.Add(mcLibEntry.Url);
                        if (mcLibEntry.Url.Contains("launcher.mojang.com/v1/objects") || mcLibEntry.Url.Contains("client.txt") || mcLibEntry.Url.Contains(".tsrg"))
                        {
                            list2.AddRange(ModDownload.DlSourceLauncherOrMetaGet(mcLibEntry.Url));
                        }
                        if (mcLibEntry.Url.Contains("maven"))
                        {
                            string text = mcLibEntry.Url.Replace(mcLibEntry.Url.Substring(0, mcLibEntry.Url.IndexOfF("maven", false)), "https://bmclapi2.bangbang93.com/").Replace("maven.fabricmc.net", "maven").Replace("maven.minecraftforge.net", "maven")
                                .Replace("maven.neoforged.net/releases", "maven");
                            if (ModDownload.DlSourcePreferMojang)
                            {
                                list2.Add(text);
                            }
                            else
                            {
                                list2.Insert(0, text);
                            }
                        }
                    }
                    if (mcLibEntry.LocalPath.Contains("transformer-discovery-service"))
                    {
                        if (!FileUtils.Exists(mcLibEntry.LocalPath))
                        {
                            ModBase.ExtractResources(mcLibEntry.LocalPath, "Transformer");
                        }
                        Logger.Warn("已自动释放 Transformer Discovery Service", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                    }
                    else
                    {
                        if (mcLibEntry.LocalPath.Contains("optifine\\OptiFine"))
                        {
                            string text2 = mcLibEntry.LocalPath.Replace(CustomMcFolder + "libraries\\optifine\\OptiFine\\", "").Split("_", false)[0] + "/" + PathUtils.GetLastPart(mcLibEntry.LocalPath).Replace("-", "_");
                            text2 = "/maven/com/optifine/" + text2;
                            if (text2.Contains("_pre"))
                            {
                                text2 = text2.Replace("com/optifine/", "com/optifine/preview_");
                            }
                            list2.Add("https://bmclapi2.bangbang93.com" + text2);
                        }
                        else if (list2.Count <= 2)
                        {
                            list2.AddRange(ModDownload.DlSourceLibraryGet("https://libraries.minecraft.net" + mcLibEntry.LocalPath.Replace(CustomMcFolder + "libraries", "").Replace("\\", "/")));
                        }
                        list.Add(new ModNet.NetFile(list2.Distinct<string>(), mcLibEntry.LocalPath, fileChecker, false));
                    }
                }
            }
            finally
            {
                List<ModMinecraft.McLibEntry>.Enumerator enumerator;
                ((IDisposable)enumerator).Dispose();
            }
            return list.DistinctBy<ModNet.NetFile, string>((ModMinecraft._Closure$__.$I40-0 == null) ? (ModMinecraft._Closure$__.$I40-0 = (ModNet.NetFile a) => a.LocalPath) : ModMinecraft._Closure$__.$I40-0).ToList<ModNet.NetFile>();
        }

        public static string McLibGet(string Original, bool WithHead = true, bool IgnoreLiteLoader = false, string CustomMcFolder = null)
        {
            CustomMcFolder = CustomMcFolder ?? ModMinecraft.McFolderSelected;
            string[] array = Original.Split(":", false);
            return (WithHead ? (CustomMcFolder + "libraries\\") : "") + string.Format("{0}\\{1}\\{2}\\{3}.jar", new object[]
            {
                array[0].Replace(".", "\\"),
                array[1],
                array[2],
                array.Skip<string>(1).Join("-")
            });
        }

        public static object ShouldIgnoreFileCheck(ModMinecraft.McInstance Instance)
        {
            return Settings.Get<bool>("VersionAdvanceAssetsV2", Instance) || Settings.Get<int>("VersionAdvanceAssets", Instance) == 2;
        }

        public static JToken McAssetsGetIndex(ModMinecraft.McInstance Instance, bool ReturnLegacyOnError = false, bool CheckURLEmpty = false)
        {
            try
            {
                JToken jtoken;
                for (;;)
                {
                    jtoken = Instance.JsonObject["assetIndex"];
                    if (jtoken != null && jtoken["id"] != null)
                    {
                        break;
                    }
                    if (Instance.JsonObject["assets"] != null)
                    {
                        Instance.JsonObject["assets"].ToString();
                    }
                    if (CheckURLEmpty && jtoken["url"] != null)
                    {
                        goto Block_6;
                    }
                    if (Operators.CompareString(Instance.InheritName, "", false) == 0)
                    {
                        goto IL_0098;
                    }
                    Instance = new ModMinecraft.McInstance(ModMinecraft.McFolderSelected + "versions\\" + Instance.InheritName);
                }
                return jtoken;
                Block_6:
                return jtoken;
                IL_0098:;
            }
            catch (Exception ex)
            {
            }
            if (!ReturnLegacyOnError)
            {
                throw new Exception("该版本不存在资源文件索引信息");
            }
            Logger.Info("无法获取资源文件索引下载地址，使用默认的 legacy 下载地址", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
            return "{\r\n                \"id\": \"legacy\",\r\n                \"sha1\": \"c0fd82e8ce9fbc93119e40d96d5a4e62cfa3f729\",\r\n                \"size\": 134284,\r\n                \"url\": \"https://launchermeta.mojang.com/mc-staging/assets/legacy/c0fd82e8ce9fbc93119e40d96d5a4e62cfa3f729/legacy.json\",\r\n                \"totalSize\": 111220701\r\n            }".DeserializeJson();
        }

        public static string McAssetsGetIndexName(ModMinecraft.McInstance Instance)
        {
            try
            {
                while (Instance.JsonObject["assetIndex"] == null || Instance.JsonObject["assetIndex"]["id"] == null)
                {
                    if (Instance.JsonObject["assets"] != null)
                    {
                        return Instance.JsonObject["assets"].ToString();
                    }
                    if (Operators.CompareString(Instance.InheritName, "", false) == 0)
                    {
                        goto IL_00D2;
                    }
                    Instance = new ModMinecraft.McInstance(ModMinecraft.McFolderSelected + "versions\\" + Instance.InheritName);
                }
                return Instance.JsonObject["assetIndex"]["id"].ToString();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "获取资源文件索引名失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
            }
            IL_00D2:
            return "legacy";
        }

        private static List<ModMinecraft.McAssetsToken> McAssetsListGet(ModMinecraft.McInstance Instance)
        {
            string text = ModMinecraft.McAssetsGetIndexName(Instance);
            List<ModMinecraft.McAssetsToken> list2;
            try
            {
                if (!FileUtils.Exists(string.Format("{0}assets\\indexes\\{1}.json", ModMinecraft.McFolderSelected, text)))
                {
                    throw new FileNotFoundException("未找到 Asset Index", ModMinecraft.McFolderSelected + "assets\\indexes\\" + text + ".json");
                }
                List<ModMinecraft.McAssetsToken> list = new List<ModMinecraft.McAssetsToken>();
                JObject jobject = (JObject)FileUtils.ReadAsJson(string.Format("{0}assets\\indexes\\{1}.json", ModMinecraft.McFolderSelected, text), null, null);
                try
                {
                    foreach (JToken jtoken in jobject["objects"].Children())
                    {
                        JProperty jproperty = (JProperty)jtoken;
                        string text2;
                        if (jobject["map_to_resources"] != null && jobject["map_to_resources"].ToObject<bool>())
                        {
                            text2 = string.Format("{0}resources\\{1}", Instance.PathIndie, jproperty.Name);
                        }
                        else if (jobject["virtual"] != null && jobject["virtual"].ToObject<bool>())
                        {
                            text2 = string.Format("{0}assets\\virtual\\legacy\\{1}", ModMinecraft.McFolderSelected, jproperty.Name);
                        }
                        else
                        {
                            text2 = string.Format("{0}assets\\objects\\{1}\\{2}", ModMinecraft.McFolderSelected, Strings.Left(jproperty.Value["hash"].ToString(), 2), jproperty.Value["hash"]);
                        }
                        list.Add(new ModMinecraft.McAssetsToken
                        {
                            LocalPath = text2.Replace("/", "\\"),
                            SourcePath = jproperty.Name,
                            Hash = jproperty.Value["hash"].ToString(),
                            Size = Conversions.ToLong(jproperty.Value["size"].ToString())
                        });
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
                list2 = list;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, string.Format("获取资源文件列表失败：{0}", text), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                throw;
            }
            return list2;
        }

        public static List<ModNet.NetFile> McAssetsFixList(ModMinecraft.McInstance Instance, bool CheckHash, ref ModLoader.LoaderBase ProgressFeed = null)
        {
            checked
            {
                List<ModNet.NetFile> list;
                if (CheckHash)
                {
                    list = ModMinecraft.McAssetsListGet(Instance).Select<ModMinecraft.McAssetsToken, ModNet.NetFile>((ModMinecraft._Closure$__.$I47-0 == null) ? (ModMinecraft._Closure$__.$I47-0 = (ModMinecraft.McAssetsToken Token) => new ModNet.NetFile(ModDownload.DlSourceAssetsGet(string.Format("https://resources.download.minecraft.net/{0}/{1}", Strings.Left(Token.Hash, 2), Token.Hash)), Token.LocalPath, new FileChecker
                    {
                        ActualSize = ((Token.Size == 0L) ? (-1L) : Token.Size),
                        Hash = Token.Hash
                    }, false)) : ModMinecraft._Closure$__.$I47-0).ToList<ModNet.NetFile>();
                }
                else
                {
                    List<ModNet.NetFile> list2 = new List<ModNet.NetFile>();
                    try
                    {
                        List<ModMinecraft.McAssetsToken> list3 = ModMinecraft.McAssetsListGet(Instance);
                        if (ProgressFeed != null)
                        {
                            ProgressFeed.Progress = 0.04;
                        }
                        int num = list3.Count - 1;
                        for (int i = 0; i <= num; i++)
                        {
                            ModMinecraft.McAssetsToken mcAssetsToken = list3[i];
                            if (ProgressFeed != null)
                            {
                                ProgressFeed.Progress = unchecked(0.05 + 0.94 * (double)i / (double)list3.Count);
                            }
                            FileInfo info = FileUtils.GetInfo(mcAssetsToken.LocalPath);
                            if (!info.Exists || (mcAssetsToken.Size != 0L && mcAssetsToken.Size != info.Length))
                            {
                                list2.Add(new ModNet.NetFile(ModDownload.DlSourceAssetsGet(string.Format("https://resources.download.minecraft.net/{0}/{1}", Strings.Left(mcAssetsToken.Hash, 2), mcAssetsToken.Hash)), mcAssetsToken.LocalPath, new FileChecker
                                {
                                    ActualSize = ((mcAssetsToken.Size == 0L) ? (-1L) : mcAssetsToken.Size),
                                    Hash = mcAssetsToken.Hash
                                }, false));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "获取版本缺失的资源文件下载列表失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                    }
                    if (ProgressFeed != null)
                    {
                        ProgressFeed.Progress = 0.99;
                    }
                    list = list2;
                }
                return list;
            }
        }

        public static void McDownloadClientUpdateHint(string VersionName, JObject Json)
        {
            ModMinecraft._Closure$__48-1 CS$<>8__locals1 = new ModMinecraft._Closure$__48-1(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_VersionName = VersionName;
            try
            {
                ModMinecraft._Closure$__48-0 CS$<>8__locals2 = new ModMinecraft._Closure$__48-0(CS$<>8__locals2);
                CS$<>8__locals2.$VB$Local_Version = null;
                try
                {
                    foreach (JToken jtoken in Json["versions"])
                    {
                        if (jtoken["id"] != null && Operators.CompareString(jtoken["id"].ToString(), CS$<>8__locals1.$VB$Local_VersionName, false) == 0)
                        {
                            CS$<>8__locals2.$VB$Local_Version = jtoken;
                            break;
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
                if (CS$<>8__locals2.$VB$Local_Version != null)
                {
                    DateTime dateTime = (DateTime)CS$<>8__locals2.$VB$Local_Version["releaseTime"];
                    if (ModMain.MyMsgBox(string.Format("新版本：{0}{1}", CS$<>8__locals1.$VB$Local_VersionName, "\r\n") + (((DateTime.Now - dateTime).TotalDays > 1.0) ? ("更新时间：" + dateTime.ToString()) : ("更新于：" + StringUtils.FormatTimeSpan(dateTime - DateTime.Now, false))), "Minecraft 更新提示", "确定", "下载", ((DateTime.Now - dateTime).TotalHours > 3.0) ? "更新日志" : "", false, true, false, null, null, delegate
                    {
                        ModDownloadLib.McUpdateLogShow(CS$<>8__locals2.$VB$Local_Version);
                    }) == 2)
                    {
                        ModBase.RunInUi(delegate
                        {
                            PageDownloadInstall.VersionWaitingSelect = CS$<>8__locals1.$VB$Local_VersionName;
                            ModMain.FrmMain.PageChange(FormMain.PageType.Download, FormMain.PageSubType.Default);
                        }, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, string.Format("Minecraft 更新提示发送失败（{0}）", CS$<>8__locals1.$VB$Local_VersionName ?? "Nothing"), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
            }
        }

        public static string FilterAccessToken(string Raw, char FilterChar)
        {
            if (Raw.Contains("accessToken "))
            {
                try
                {
                    foreach (string text in Raw.RegexSearch("(?<=accessToken ([^ ]{5}))[^ ]+(?=[^ ]{5})", RegexOptions.None))
                    {
                        Raw = Raw.Replace(text, new string(FilterChar, text.Count<char>()));
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
            string accessToken = ModLaunch.McLoginLoader.Output.AccessToken;
            if (accessToken != null && accessToken.Length >= 10 && Raw.ContainsIgnoreCase(accessToken) && Operators.CompareString(ModLaunch.McLoginLoader.Output.Uuid, ModLaunch.McLoginLoader.Output.AccessToken, false) != 0)
            {
                Raw = Raw.Replace(accessToken, Strings.Left(accessToken, 5) + new string(FilterChar, checked(accessToken.Length - 10)) + Strings.Right(accessToken, 5));
            }
            return Raw;
        }

        public static string FilterUserName(string Raw, char FilterChar)
        {
            if (Raw.Contains(":\\Users\\"))
            {
                Raw = Regex.Replace(Raw, "(?<=:\\\\Users\\\\)[^\\\\]+", (Match Token) => new string(FilterChar, Token.Length));
            }
            if (Raw.Contains(":/Users/"))
            {
                Raw = Regex.Replace(Raw, "(?<=:/Users/)[^/]+", (Match Token) => new string(FilterChar, Token.Length));
            }
            return Raw;
        }

        private static string _McFolderSelected;

        public static List<ModMinecraft.McFolder> McFolderList = new List<ModMinecraft.McFolder>();

        public static ModLoader.LoaderTask<int, int> McFolderListLoader = new ModLoader.LoaderTask<int, int>("Minecraft Folder List", delegate(ModLoader.LoaderTask<int, int> a0)
        {
            ModMinecraft.McFolderListLoadSub();
        }, null, ThreadPriority.AboveNormal);

        private static ModMinecraft.McInstance _McInstanceSelected;

        public static Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>> McInstanceList = new Dictionary<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>>();

        public static bool McInstanceListForceRefresh = false;

        private static bool IsFirstMcInstanceListLoad = true;

        public static ModLoader.LoaderTask<string, int> McInstanceListLoader = new ModLoader.LoaderTask<string, int>("Minecraft Instance List", new Action<ModLoader.LoaderTask<string, int>>(ModMinecraft.InitMcInstanceList), null, ThreadPriority.Normal)
        {
            ReloadTimeout = 1
        };

        private static readonly object McSkinDownloadLock = RuntimeHelpers.GetObjectValue(new object());

        private static string OSVersion = MyWpfExtension.Computer.Info.OSVersion;

        private static object $STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast;

        private static StaticLocalInitFlag $STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init;

        public class McFolder
        {
            public override bool Equals(object obj)
            {
                bool flag;
                if (!(obj is ModMinecraft.McFolder))
                {
                    flag = false;
                }
                else
                {
                    ModMinecraft.McFolder mcFolder = (ModMinecraft.McFolder)obj;
                    flag = Operators.CompareString(this.Name, mcFolder.Name, false) == 0 && PathUtils.IsSame(this.Location, mcFolder.Location) && this.Type == mcFolder.Type;
                }
                return flag;
            }

            public override string ToString()
            {
                return this.Location;
            }

            public string Name;

            public string Location;

            public ModMinecraft.McFolder.Types Type;

            public enum Types
            {
                Vanilla,
                RenamedVanilla,
                Custom
            }
        }

        public class McInstance
        {
            public string PathVersion { get; }

            public string PathIndie
            {
                get
                {
                    if (!Settings.HasSaved("VersionArgumentIndieV2", this))
                    {
                        if (!this.IsLoaded)
                        {
                            this.Load();
                        }
                        VB$AnonymousDelegate_0<bool> vb$AnonymousDelegate_ = delegate
                        {
                            bool flag;
                            if (Settings.HasSaved("VersionArgumentIndie", this) && Settings.Get<int>("VersionArgumentIndie", this) > 0)
                            {
                                Logger.Info(string.Format("版本隔离初始化（{0}）：从老的版本独立设置中迁移", this.Name), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                                flag = Settings.Get<int>("VersionArgumentIndie", this) == 1;
                            }
                            else
                            {
                                DirectoryInfo info = DirectoryUtils.GetInfo(this.PathVersion + "mods\\");
                                DirectoryInfo info2 = DirectoryUtils.GetInfo(this.PathVersion + "saves\\");
                                if ((info.Exists && info.EnumerateFiles().Any<FileInfo>()) || (info2.Exists && info2.EnumerateDirectories().Any<DirectoryInfo>()))
                                {
                                    Logger.Info(string.Format("版本隔离初始化（{0}）：版本文件夹下存在 mods 或 saves 文件夹，自动开启", this.Name), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                                    flag = true;
                                }
                                else
                                {
                                    bool flag2 = this.State != ModMinecraft.McInstance.McInstanceState.Fool && this.State != ModMinecraft.McInstance.McInstanceState.Old && this.State != ModMinecraft.McInstance.McInstanceState.Snapshot;
                                    Logger.Info(string.Format("版本隔离初始化（{0}）：从全局默认设置中（{1}）判断，State {2}，IsRelease {3}，Modable {4}", new object[]
                                    {
                                        this.Name,
                                        Settings.Get<int>("LaunchArgumentIndieV2", null),
                                        this.State,
                                        flag2,
                                        this.Modable
                                    }), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                                    switch (Settings.Get<int>("LaunchArgumentIndieV2", null))
                                    {
                                    case 0:
                                        flag = false;
                                        break;
                                    case 1:
                                        flag = this.Modable;
                                        break;
                                    case 2:
                                        flag = !flag2;
                                        break;
                                    case 3:
                                        flag = !flag2 || this.Modable;
                                        break;
                                    default:
                                        flag = true;
                                        break;
                                    }
                                }
                            }
                            return flag;
                        };
                        Settings.Set("VersionArgumentIndieV2", vb$AnonymousDelegate_(), this);
                    }
                    if (!Settings.Get<bool>("VersionArgumentIndieV2", this))
                    {
                        return ModMinecraft.McFolderSelected;
                    }
                    return this.PathVersion;
                }
            }

            public string Name
            {
                get
                {
                    if (this._Name == null && Operators.CompareString(this.PathVersion, "", false) != 0)
                    {
                        this._Name = PathUtils.GetLastPart(this.PathVersion);
                    }
                    return this._Name;
                }
            }

            public bool Modable
            {
                get
                {
                    if (!this.IsLoaded)
                    {
                        this.Load();
                    }
                    return this.Version.ModLoaders.Flags<ModLoaders>().Any<ModLoaders>() || this.DisplayType == ModMinecraft.McInstanceCardType.API;
                }
            }

            public McVersion Version
            {
                get
                {
                    McVersion mcVersion;
                    if (this._Version != null)
                    {
                        mcVersion = this._Version;
                    }
                    else
                    {
                        try
                        {
                            McVersion mcVersion2 = new McVersion(this.Name, this.JsonObject, this.JsonVersion);
                            this._Version = mcVersion2;
                            if (Operators.CompareString(mcVersion2.VanillaName, "Unknown", false) == 0)
                            {
                                this.Info = "PCL 无法识别该版本的 MC 版本号";
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, "识别 Minecraft 版本时出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                            this._Version = new McVersion("Unknown", true);
                            this.Info = "无法识别：" + ex.Message;
                        }
                        mcVersion = this._Version;
                    }
                    return mcVersion;
                }
                set
                {
                    this._Version = value;
                }
            }

            public DateTime ReleaseTime
            {
                get
                {
                    return this.Version.ReleaseTime;
                }
            }

            public string GetJsonPath()
            {
                string text = this.PathVersion + this.Name + ".json";
                if (!FileUtils.Exists(text))
                {
                    text = null;
                    try
                    {
                        foreach (string text2 in DirectoryUtils.EnumerateFiles(this.PathVersion, false, "*.json"))
                        {
                            try
                            {
                                JObject jobject = (JObject)FileUtils.ReadAsJson(text2, null, null);
                                if (!jobject.ContainsKey("mainClass"))
                                {
                                    continue;
                                }
                                if (!jobject.ContainsKey("type"))
                                {
                                    continue;
                                }
                                if (!jobject.ContainsKey("id"))
                                {
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                            text = text2;
                            break;
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
                    if (text == null)
                    {
                        throw new Exception(string.Format("未找到版本 JSON 文件：{0}{1}.json", this.PathVersion, this.Name));
                    }
                    Logger.Warn(string.Format("未找到同名版本 JSON，自动换用 {0}", text), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                }
                return text;
            }

            public string JsonText
            {
                get
                {
                    VB$AnonymousDelegate_19<string, bool> vb$AnonymousDelegate_ = ((ModMinecraft.McInstance._Closure$__.$I24-0 == null) ? (ModMinecraft.McInstance._Closure$__.$I24-0 = delegate(string Json)
                    {
                        string text = Json.Trim();
                        return text.StartsWithF("{", false) && text.EndsWithF("}", false);
                    }) : ModMinecraft.McInstance._Closure$__.$I24-0);
                    if (this._JsonText == null)
                    {
                        string jsonPath = this.GetJsonPath();
                        this._JsonText = FileUtils.TryReadAsString(jsonPath, null, null) ?? "";
                        if (!vb$AnonymousDelegate_(this._JsonText))
                        {
                            if (ModBase.RunInUi())
                            {
                                Logger.Warn("版本 JSON 文件为空或有误，由于代码在主线程运行，将不再进行重试", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                                this._JsonText.DeserializeJson();
                            }
                            else
                            {
                                Logger.Warn(string.Format("版本 JSON 文件为空或有误，将在 2s 后重试读取（{0}）", jsonPath), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                                Thread.Sleep(2000);
                                this._JsonText = FileUtils.ReadAsString(jsonPath, null, null);
                                if (!vb$AnonymousDelegate_(this._JsonText))
                                {
                                    this._JsonText.DeserializeJson();
                                }
                            }
                        }
                    }
                    return this._JsonText;
                }
                set
                {
                    this._JsonText = value;
                }
            }

            public JObject JsonObject
            {
                get
                {
                    if (this._JsonObject == null)
                    {
                        string jsonText = this.JsonText;
                        try
                        {
                            this._JsonObject = (JObject)jsonText.DeserializeJson();
                            if (this._JsonObject.ContainsKey("patches") && !this._JsonObject.ContainsKey("time"))
                            {
                                this.IsHmclFormatJson = true;
                                JObject jobject = null;
                                List<JObject> list = new List<JObject>();
                                try
                                {
                                    foreach (JToken jtoken in this._JsonObject["patches"])
                                    {
                                        JObject jobject2 = (JObject)jtoken;
                                        list.Add(jobject2);
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
                                list = list.SortByComparison<JObject>((ModMinecraft.McInstance._Closure$__.$I28-0 == null) ? (ModMinecraft.McInstance._Closure$__.$I28-0 = (JObject Left, JObject Right) => ModBase.Val((Left["priority"] ?? "0").ToString()) < ModBase.Val((Right["priority"] ?? "0").ToString())) : ModMinecraft.McInstance._Closure$__.$I28-0);
                                try
                                {
                                    foreach (JObject jobject3 in list)
                                    {
                                        string text = (string)jobject3["id"];
                                        if (text != null)
                                        {
                                            Logger.Info(string.Format("合并 HMCL 分支项：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                                            if (jobject != null)
                                            {
                                                jobject.Merge(jobject3);
                                            }
                                            else
                                            {
                                                jobject = jobject3;
                                            }
                                        }
                                        else
                                        {
                                            Logger.Info("存在为空的 HMCL 分支项", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                                        }
                                    }
                                }
                                finally
                                {
                                    List<JObject>.Enumerator enumerator2;
                                    ((IDisposable)enumerator2).Dispose();
                                }
                                this._JsonObject = jobject;
                                this._JsonObject["id"] = this.Name;
                                if (this._JsonObject.ContainsKey("inheritsFrom"))
                                {
                                    this._JsonObject.Remove("inheritsFrom");
                                }
                            }
                            object obj = null;
                            try
                            {
                                obj = ((this._JsonObject["inheritsFrom"] == null) ? "" : this._JsonObject["inheritsFrom"].ToString());
                                if (Operators.ConditionalCompareObjectEqual(obj, this.Name, false))
                                {
                                    Logger.Warn(string.Format("自引用的继承版本：{0}", this.Name), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                                    obj = "";
                                }
                                else
                                {
                                    while (Operators.ConditionalCompareObjectNotEqual(obj, "", false))
                                    {
                                        ModMinecraft.McInstance mcInstance = new ModMinecraft.McInstance(Conversions.ToString(obj));
                                        if (Operators.ConditionalCompareObjectEqual(mcInstance.InheritName, obj, false))
                                        {
                                            throw new Exception(Conversions.ToString(Operators.ConcatenateObject("版本依赖项出现嵌套：", obj)));
                                        }
                                        obj = mcInstance.InheritName;
                                        JArray jarray = (JArray)this._JsonObject["libraries"].DeepClone();
                                        try
                                        {
                                            foreach (JToken jtoken2 in mcInstance.JsonObject["libraries"])
                                            {
                                                jarray.Add(jtoken2);
                                            }
                                        }
                                        finally
                                        {
                                            IEnumerator<JToken> enumerator3;
                                            if (enumerator3 != null)
                                            {
                                                enumerator3.Dispose();
                                            }
                                        }
                                        mcInstance.JsonObject.Merge(this._JsonObject);
                                        this._JsonObject = mcInstance.JsonObject;
                                        this._JsonObject["libraries"] = jarray;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Warn(ex, string.Format("合并版本依赖项 JSON 失败（{0}）", RuntimeHelpers.GetObjectValue(obj ?? "null")), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                            }
                        }
                        catch (Exception ex2)
                        {
                            throw new Exception("初始化版本 JSON 时失败（" + (this.Name ?? "null") + "）", ex2);
                        }
                    }
                    return this._JsonObject;
                }
                set
                {
                    this._JsonObject = value;
                }
            }

            public bool IsOldJson
            {
                get
                {
                    return this.JsonObject["minecraftArguments"] != null && (string)this.JsonObject["minecraftArguments"] != "";
                }
            }

            public bool IsHmclFormatJson { get; set; }

            public JObject JsonVersion
            {
                get
                {
                    if (this.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init == null)
                    {
                        Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init, new StaticLocalInitFlag(), null);
                    }
                    bool flag = false;
                    try
                    {
                        Monitor.Enter(this.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init, ref flag);
                        if (this.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init.State == 0)
                        {
                            this.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init.State = 2;
                            this.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited = false;
                        }
                        else if (this.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init.State == 2)
                        {
                            throw new IncompleteInitialization();
                        }
                    }
                    finally
                    {
                        this.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init.State = 1;
                        if (flag)
                        {
                            Monitor.Exit(this.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init);
                        }
                    }
                    if (!this.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited)
                    {
                        this.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited = true;
                        try
                        {
                            if (FileUtils.Exists(string.Format("{0}{1}.jar", this.PathVersion, this.Name)))
                            {
                                using (ZipArchive zipArchive = FileUtils.OpenZip(this.PathVersion + this.Name + ".jar", default(CancellationToken)))
                                {
                                    ZipArchiveEntry entry = zipArchive.GetEntry("version.json");
                                    if (entry != null)
                                    {
                                        using (StreamReader streamReader = new StreamReader(entry.Open()))
                                        {
                                            this._JsonVersion = (JObject)streamReader.ReadToEnd().DeserializeJson();
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, string.Format("从版本 jar 中读取 version.json 失败（{0}{1}.jar）", this.PathVersion, this.Name), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                        }
                    }
                    return this._JsonVersion;
                }
            }

            public string InheritName
            {
                get
                {
                    if (this._InheritName == null)
                    {
                        this._InheritName = (this.JsonObject["inheritsFrom"] ?? "").ToString();
                        if (this.JsonText.Contains("liteloader") && Operators.CompareString(this.Version.VanillaName, this.Name, false) != 0 && !this.JsonText.Contains("logging") && Operators.CompareString((this.JsonObject["jar"] ?? this.Version.VanillaName).ToString(), this.Version.VanillaName, false) == 0)
                        {
                            this._InheritName = this.Version.VanillaName;
                        }
                        if (this.IsHmclFormatJson)
                        {
                            this._InheritName = "";
                        }
                    }
                    return this._InheritName;
                }
            }

            public void ResetSettingsCache()
            {
                ModBase.IniClearCache(this.PathIndie + "options.txt");
                ModBase.IniClearCache(this.PathVersion + "PCL\\Setup.ini");
                this.Config.DiscardCache();
                try
                {
                    foreach (Settings.Setting setting in Settings.Entries.Values)
                    {
                        if (setting.Source == Settings.Sources.Instance)
                        {
                            setting.ValueCache.Remove(this.PathVersion);
                        }
                    }
                }
                finally
                {
                    Dictionary<string, Settings.Setting>.ValueCollection.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
            }

            public McInstance(string NameOrFullPath)
            {
                this._Name = null;
                this.Info = "该版本未被加载，请向作者反馈此问题";
                this.State = ModMinecraft.McInstance.McInstanceState.Error;
                this.IsStar = false;
                this.DisplayType = ModMinecraft.McInstanceCardType.Auto;
                this._Version = null;
                this._JsonText = null;
                this._JsonObject = null;
                this.IsHmclFormatJson = false;
                this._JsonVersion = null;
                this._InheritName = null;
                this.IsLoaded = false;
                NameOrFullPath = PathUtils.RemoveExtendedPrefix(NameOrFullPath);
                this.PathVersion = PathUtils.AddSlashSuffix((NameOrFullPath.Contains(":") ? "" : (ModMinecraft.McFolderSelected + "versions\\")) + NameOrFullPath);
                this.Config = new JsonConfigProvider(Path.Combine(this.PathVersion, "PCL\\config.json"));
            }

            public bool Check()
            {
                bool flag;
                if (!DirectoryUtils.Exists(this.PathVersion))
                {
                    this.State = ModMinecraft.McInstance.McInstanceState.Error;
                    this.Info = "未找到版本 " + this.Name;
                    flag = false;
                }
                else
                {
                    try
                    {
                        if (this.JsonObject["mainClass"] == null)
                        {
                            throw new Exception("版本 JSON 中没有 mainClass 项！");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, string.Format("版本 JSON 可用性检查失败（{0}）", this.PathVersion), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                        this.JsonText = "";
                        this.JsonObject = null;
                        this.Info = ex.Message;
                        this.State = ModMinecraft.McInstance.McInstanceState.Error;
                        return false;
                    }
                    try
                    {
                        if (string.IsNullOrEmpty(this.Version.VanillaName))
                        {
                            throw new Exception("无法获取版本号，结果为空");
                        }
                    }
                    catch (Exception ex2)
                    {
                        Logger.Warn(ex2, string.Format("版本号获取失败（{0}）", this.Name), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                        this.State = ModMinecraft.McInstance.McInstanceState.Error;
                        this.Info = "版本号获取失败：" + ex2.GetDisplay(false);
                        return false;
                    }
                    try
                    {
                        if (Operators.CompareString(this.InheritName, "", false) != 0 && !FileUtils.Exists(string.Format("{0}\\{1}\\{2}.json", PathUtils.RemoveLastPart(this.PathVersion), this.InheritName, this.InheritName)))
                        {
                            this.State = ModMinecraft.McInstance.McInstanceState.Error;
                            this.Info = "需要安装 " + this.InheritName + " 作为前置版本";
                            return false;
                        }
                    }
                    catch (Exception ex3)
                    {
                        Logger.Warn(ex3, string.Format("依赖版本检查出错（{0}）", this.Name), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                        this.State = ModMinecraft.McInstance.McInstanceState.Error;
                        this.Info = "未知错误：" + ex3.GetDisplay(false);
                        return false;
                    }
                    this.State = ModMinecraft.McInstance.McInstanceState.Original;
                    flag = true;
                }
                return flag;
            }

            public ModMinecraft.McInstance Load()
            {
                try
                {
                    if (this.Check())
                    {
                        string vanillaName = this.Version.VanillaName;
                        if (Operators.CompareString(vanillaName, "Unknown", false) != 0)
                        {
                            if (Operators.CompareString(vanillaName, "Old", false) != 0)
                            {
                                if (this.Version.IsFool)
                                {
                                    this.State = ModMinecraft.McInstance.McInstanceState.Fool;
                                }
                                else if (this.Version.IsSnapshot)
                                {
                                    this.State = ModMinecraft.McInstance.McInstanceState.Snapshot;
                                }
                                if (this.Version.HasOptiFine)
                                {
                                    this.State = ModMinecraft.McInstance.McInstanceState.OptiFine;
                                }
                                if (this.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader))
                                {
                                    this.State = ModMinecraft.McInstance.McInstanceState.LiteLoader;
                                }
                                if (this.Version.ModLoaders.HasFlagF(ModLoaders.Fabric))
                                {
                                    this.State = ModMinecraft.McInstance.McInstanceState.Fabric;
                                }
                                else if (this.Version.ModLoaders.HasFlagF(ModLoaders.Forge))
                                {
                                    this.State = ModMinecraft.McInstance.McInstanceState.Forge;
                                }
                                else if (this.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge))
                                {
                                    this.State = ModMinecraft.McInstance.McInstanceState.NeoForge;
                                }
                            }
                            else
                            {
                                this.State = ModMinecraft.McInstance.McInstanceState.Old;
                            }
                        }
                        else
                        {
                            this.State = ModMinecraft.McInstance.McInstanceState.Error;
                        }
                    }
                    this.Logo = ModBase.ReadIni(this.PathVersion + "PCL\\Setup.ini", "Logo", "");
                    if (Operators.CompareString(this.Logo, "", false) == 0 || !Conversions.ToBoolean(ModBase.ReadIni(this.PathVersion + "PCL\\Setup.ini", "LogoCustom", Conversions.ToString(false))))
                    {
                        switch (this.State)
                        {
                        case ModMinecraft.McInstance.McInstanceState.Original:
                            this.Logo = ModBase.PathImage + "Blocks/Grass.png";
                            break;
                        case ModMinecraft.McInstance.McInstanceState.Snapshot:
                            this.Logo = ModBase.PathImage + "Blocks/CommandBlock.png";
                            break;
                        case ModMinecraft.McInstance.McInstanceState.Fool:
                            this.Logo = ModBase.PathImage + "Blocks/GoldBlock.png";
                            break;
                        case ModMinecraft.McInstance.McInstanceState.OptiFine:
                            this.Logo = ModBase.PathImage + "Blocks/GrassPath.png";
                            break;
                        case ModMinecraft.McInstance.McInstanceState.Old:
                            this.Logo = ModBase.PathImage + "Blocks/CobbleStone.png";
                            break;
                        case ModMinecraft.McInstance.McInstanceState.Forge:
                            this.Logo = ModBase.PathImage + "Blocks/Anvil.png";
                            break;
                        case ModMinecraft.McInstance.McInstanceState.NeoForge:
                            this.Logo = ModBase.PathImage + "Blocks/NeoForge.png";
                            break;
                        case ModMinecraft.McInstance.McInstanceState.LiteLoader:
                            this.Logo = ModBase.PathImage + "Blocks/Egg.png";
                            break;
                        case ModMinecraft.McInstance.McInstanceState.Fabric:
                            this.Logo = ModBase.PathImage + "Blocks/Fabric.png";
                            break;
                        default:
                            this.Logo = ModBase.PathImage + "Blocks/RedstoneBlock.png";
                            break;
                        }
                    }
                    this.RefreshInfo();
                    this.IsStar = Conversions.ToBoolean(ModBase.ReadIni(this.PathVersion + "PCL\\Setup.ini", "IsStar", Conversions.ToString(false)));
                    this.DisplayType = (ModMinecraft.McInstanceCardType)Conversions.ToInteger(ModBase.ReadIni(this.PathVersion + "PCL\\Setup.ini", "DisplayType", Conversions.ToString(0)));
                    if (DirectoryUtils.Exists(this.PathVersion))
                    {
                        ModBase.WriteIni(this.PathVersion + "PCL\\Setup.ini", "State", Conversions.ToString((int)this.State));
                        ModBase.WriteIni(this.PathVersion + "PCL\\Setup.ini", "Info", this.Info);
                        ModBase.WriteIni(this.PathVersion + "PCL\\Setup.ini", "Logo", this.Logo);
                    }
                    if (this.State != ModMinecraft.McInstance.McInstanceState.Error && this.Version.Reliable)
                    {
                        ModBase.WriteIni(this.PathVersion + "PCL\\Setup.ini", "ReleaseTime", this.ReleaseTime.ToString("yyyy'-'MM'-'dd HH':'mm"));
                        ModBase.WriteIni(this.PathVersion + "PCL\\Setup.ini", "VersionFabric", this.Version.ModLoaders.HasFlagF(ModLoaders.Fabric) ? this.Version.ModLoaderVersions[ModLoaders.Fabric] : null);
                        ModBase.WriteIni(this.PathVersion + "PCL\\Setup.ini", "VersionOptiFine", this.Version.OptiFineAsMod ? null : this.Version.OptiFine);
                        ModBase.WriteIni(this.PathVersion + "PCL\\Setup.ini", "VersionLiteLoader", Conversions.ToString(this.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader)));
                        ModBase.WriteIni(this.PathVersion + "PCL\\Setup.ini", "VersionForge", this.Version.ModLoaders.HasFlagF(ModLoaders.Forge) ? this.Version.ModLoaderVersions[ModLoaders.Forge] : null);
                        ModBase.WriteIni(this.PathVersion + "PCL\\Setup.ini", "VersionNeoForge", this.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge) ? this.Version.ModLoaderVersions[ModLoaders.NeoForge] : null);
                        ModBase.WriteIni(this.PathVersion + "PCL\\Setup.ini", "VersionVanillaName", this.Version.VanillaName);
                        ModBase.WriteIni(this.PathVersion + "PCL\\Setup.ini", "VersionVanilla", this.Version.Vanilla.ToString());
                    }
                }
                catch (Exception ex)
                {
                    this.Info = ex.GetDisplay(false);
                    this.Logo = ModBase.PathImage + "Blocks/RedstoneBlock.png";
                    this.State = ModMinecraft.McInstance.McInstanceState.Error;
                    Logger.Warn(ex, string.Format("加载版本失败（{0}）", this.Name), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                }
                finally
                {
                    this.IsLoaded = true;
                }
                return this;
            }

            public void RefreshInfo()
            {
                string text = ModBase.ReadIni(this.PathVersion + "PCL\\Setup.ini", "CustomInfo", "");
                if (Operators.CompareString(text, "", false) != 0)
                {
                    this.Info = text;
                    return;
                }
                this.Info = this.VersionDisplayName();
                if (this.State == ModMinecraft.McInstance.McInstanceState.Fool)
                {
                    this.Info = ModMinecraft.GetMcFoolName(this.Version.VanillaName);
                    return;
                }
                if (this.State != ModMinecraft.McInstance.McInstanceState.Error)
                {
                    if (Settings.Get<int>("VersionServerLogin", this) == 3)
                    {
                        ref string ptr = ref this.Info;
                        this.Info = ptr + ", 统一通行证验证";
                    }
                    if (Settings.Get<int>("VersionServerLogin", this) == 4)
                    {
                        ref string ptr = ref this.Info;
                        this.Info = ptr + ", Authlib 验证";
                    }
                }
            }

            public string VersionDisplayName()
            {
                string text = "";
                if (this.Version.ModLoaders.HasFlagF(ModLoaders.Forge))
                {
                    text = text + ", Forge" + ((Operators.CompareString(this.Version.ModLoaderVersions[ModLoaders.Forge], "未知版本", false) == 0) ? "" : (" " + this.Version.ModLoaderVersions[ModLoaders.Forge]));
                }
                if (this.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge))
                {
                    text = text + ", NeoForge" + ((Operators.CompareString(this.Version.ModLoaderVersions[ModLoaders.NeoForge], "未知版本", false) == 0) ? "" : (" " + this.Version.ModLoaderVersions[ModLoaders.NeoForge]));
                }
                if (this.Version.ModLoaders.HasFlagF(ModLoaders.Fabric))
                {
                    text = text + ", Fabric" + ((Operators.CompareString(this.Version.ModLoaderVersions[ModLoaders.Fabric], "未知版本", false) == 0) ? "" : (" " + this.Version.ModLoaderVersions[ModLoaders.Fabric]));
                }
                if (this.Version.HasOptiFine)
                {
                    text = text + ", OptiFine" + ((Operators.CompareString(this.Version.OptiFine, "未知版本", false) == 0) ? "" : (" " + this.Version.OptiFine.Replace("-", " ").Replace("_", " ")));
                }
                if (this.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader))
                {
                    text += ", LiteLoader";
                }
                string text2;
                switch (this.State)
                {
                case ModMinecraft.McInstance.McInstanceState.Error:
                    return this.Info;
                case ModMinecraft.McInstance.McInstanceState.Original:
                case ModMinecraft.McInstance.McInstanceState.Snapshot:
                case ModMinecraft.McInstance.McInstanceState.OptiFine:
                case ModMinecraft.McInstance.McInstanceState.Forge:
                case ModMinecraft.McInstance.McInstanceState.NeoForge:
                case ModMinecraft.McInstance.McInstanceState.LiteLoader:
                case ModMinecraft.McInstance.McInstanceState.Fabric:
                    if (this.Version.VanillaName.ContainsIgnoreCase("pre"))
                    {
                        text2 = "预发布版 " + this.Version.VanillaName;
                    }
                    else if (this.Version.VanillaName.ContainsIgnoreCase("rc"))
                    {
                        text2 = "发布候选 " + this.Version.VanillaName;
                    }
                    else if (this.Version.VanillaName.ContainsIgnoreCase("experimental"))
                    {
                        text2 = "实验性快照" + this.Version.VanillaName;
                    }
                    else if (Operators.CompareString(this.Version.VanillaName, "pending", false) == 0)
                    {
                        text2 = "实验性快照";
                    }
                    else if (this.Version.IsSnapshot)
                    {
                        text2 = (this.Version.Reliable ? ("快照版 " + this.Version.VanillaName.Replace("-snapshot", "")) : "快照版");
                    }
                    else
                    {
                        text2 = (this.Version.Reliable ? ("正式版 " + this.Version.VanillaName) : "正式版");
                    }
                    break;
                case ModMinecraft.McInstance.McInstanceState.Fool:
                    text2 = "愚人节版本 " + this.Version.VanillaName;
                    break;
                case ModMinecraft.McInstance.McInstanceState.Old:
                    text2 = "远古版本";
                    break;
                default:
                    return "发生了未知错误，请向作者反馈此问题";
                }
                return (text2 + text).Replace("_", "-");
            }

            public MyVirtualizingElement<MyListItem> ToListItem(Action<MyListItem, EventArgs> ContentHandler = null)
            {
                return new MyVirtualizingElement<MyListItem>(delegate
                {
                    MyListItem myListItem = new MyListItem
                    {
                        Info = this.Info,
                        Height = 42.0,
                        Tag = this,
                        SnapsToDevicePixels = true,
                        Type = MyListItem.CheckType.Clickable,
                        ContentHandler = ContentHandler
                    };
                    myListItem.Inlines.Clear();
                    myListItem.Inlines.Add(new Run(this.Name));
                    if (Operators.CompareString(ModBase.ReadIni(this.PathVersion + "PCL\\Setup.ini", "CustomInfo", ""), "", false) != 0)
                    {
                        myListItem.Inlines.Add(new Run("  |  " + this.VersionDisplayName())
                        {
                            Foreground = new ModBase.MyColor(215.0, 215.0, 215.0),
                            FontSize = 12.0
                        });
                    }
                    try
                    {
                        if (this.Logo.EndsWithF("PCL\\Logo.png", false))
                        {
                            myListItem.Logo = this.PathVersion + "PCL\\Logo.png";
                        }
                        else
                        {
                            myListItem.Logo = this.Logo;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "加载版本图标失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                        myListItem.Logo = "pack://application:,,,/images/Blocks/RedstoneBlock.png";
                    }
                    return myListItem;
                })
                {
                    Height = 42.0
                };
            }

            public ModMinecraft.McInstance Rename(string NewName)
            {
                string name = this.Name;
                string pathVersion = this.PathVersion;
                string text = PathUtils.AddSlashSuffix(Path.Combine(PathUtils.RemoveLastPart(pathVersion), NewName));
                string jsonPath = this.GetJsonPath();
                JObject jobject;
                try
                {
                    jobject = (JObject)FileUtils.ReadAsJson(jsonPath, null, null);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "在重命名读取 JSON 时失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                    jobject = this.JsonObject;
                }
                this.Config.Save();
                this.ResetSettingsCache();
                DirectoryUtils.Move(pathVersion, text);
                if (DirectoryUtils.Exists(string.Format("{0}{1}-natives", text, name)))
                {
                    DirectoryUtils.Move(string.Format("{0}{1}-natives", text, name), string.Format("{0}{1}-natives", text, NewName));
                }
                if (FileUtils.Exists(string.Format("{0}{1}.jar", text, name)))
                {
                    FileUtils.Move(string.Format("{0}{1}.jar", text, name), string.Format("{0}{1}.jar", text, NewName), false);
                }
                if (FileUtils.Exists(text + "PCL\\Setup.ini"))
                {
                    FileUtils.Write(text + "PCL\\Setup.ini", FileUtils.ReadAsString(text + "PCL\\Setup.ini", null, null).Replace(pathVersion, text), null);
                }
                if (Operators.CompareString(ModBase.ReadIni(ModMinecraft.McFolderSelected + "PCL.ini", "Version", ""), name, false) == 0)
                {
                    ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "Version", NewName);
                }
                try
                {
                    jobject["id"] = NewName;
                    FileUtils.Delete(text + PathUtils.GetLastPart(jsonPath), false);
                    FileUtils.Write(text + NewName + ".json", jobject.ToString(), null);
                }
                catch (Exception ex2)
                {
                    Logger.Warn(ex2, "重命名版本 json 失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModMinecraft.vb");
                }
                return new ModMinecraft.McInstance(text).Load();
            }

            public override bool Equals(object obj)
            {
                ModMinecraft.McInstance mcInstance = obj as ModMinecraft.McInstance;
                return mcInstance != null && PathUtils.IsSame(this.PathVersion, mcInstance.PathVersion);
            }

            public static bool operator ==(ModMinecraft.McInstance a, ModMinecraft.McInstance b)
            {
                return (a == null && b == null) || (a != null && b != null && PathUtils.IsSame(a.PathVersion, b.PathVersion));
            }

            public static bool operator !=(ModMinecraft.McInstance a, ModMinecraft.McInstance b)
            {
                return !(a == b);
            }

            private string _Name;

            public string Info;

            public ModMinecraft.McInstance.McInstanceState State;

            public string Logo;

            public bool IsStar;

            public ModMinecraft.McInstanceCardType DisplayType;

            private McVersion _Version;

            private string _JsonText;

            private JObject _JsonObject;

            private JObject _JsonVersion;

            private string _InheritName;

            public JsonConfigProvider Config;

            public bool IsLoaded;

            private bool $STATIC$get_JsonVersion$2001281F1$JsonVersionInited;

            private StaticLocalInitFlag $STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init;

            public enum McInstanceState
            {
                Error,
                Original,
                Snapshot,
                Fool,
                OptiFine,
                Old,
                Forge,
                NeoForge,
                LiteLoader,
                Fabric
            }
        }

        public sealed class McInstanceLock
        {
            internal static bool IsAnyLocked
            {
                get
                {
                    return !ModMinecraft.McInstanceLock.LockedPaths.IsEmpty;
                }
            }

            public static bool TryAcquire(IWorker Worker, string InstancePath)
            {
                InstancePath = PathUtils.ForCompare(InstancePath);
                bool flag;
                if (!ModMinecraft.McInstanceLock.LockedPaths.TryAdd(InstancePath, Worker))
                {
                    flag = false;
                }
                else
                {
                    Worker.Stopped += delegate
                    {
                        ConcurrentDictionary<string, IWorker> lockedPaths = ModMinecraft.McInstanceLock.LockedPaths;
                        string $VB$Local_InstancePath = InstancePath;
                        IWorker worker = null;
                        lockedPaths.TryRemove($VB$Local_InstancePath, out worker);
                    };
                    flag = true;
                }
                return flag;
            }

            public static bool IsLocked(ModMinecraft.McInstance Instance)
            {
                return Instance != null && ModMinecraft.McInstanceLock.LockedPaths.ContainsKey(PathUtils.ForCompare(Instance.PathVersion));
            }

            private static readonly ConcurrentDictionary<string, IWorker> LockedPaths = new ConcurrentDictionary<string, IWorker>(StringComparer.OrdinalIgnoreCase);
        }

        public enum McInstanceCardType
        {
            Star = -1,
            Auto,
            Hidden,
            API,
            OriginalLike,
            Rubbish,
            Fool,
            Error
        }

        public struct McSkinInfo
        {
            public bool IsSlim;

            public string LocalFile;

            public bool IsVaild;
        }

        public class McLibEntry
        {
            public McLibEntry()
            {
                this.Size = 0L;
                this.IsNatives = false;
                this.SHA1 = null;
            }

            public string Url
            {
                get
                {
                    return this._Url;
                }
                set
                {
                    this._Url = (string.IsNullOrWhiteSpace(value) ? null : value);
                }
            }

            public string Name
            {
                get
                {
                    string text;
                    if (this.OriginalName == null)
                    {
                        text = null;
                    }
                    else
                    {
                        List<string> list = new List<string>(this.OriginalName.Split(":", false));
                        list.RemoveAt(2);
                        text = list.Join(':');
                    }
                    return text;
                }
            }

            public override string ToString()
            {
                return (this.IsNatives ? "[Native] " : "") + StringUtils.FormatByteSize(this.Size) + " | " + this.LocalPath;
            }

            public string LocalPath;

            public long Size;

            public bool IsNatives;

            public string SHA1;

            private string _Url;

            public string OriginalName;
        }

        private struct McAssetsToken
        {
            public override string ToString()
            {
                return StringUtils.FormatByteSize(this.Size) + " | " + this.LocalPath;
            }

            public string LocalPath;

            public string SourcePath;

            public long Size;

            public string Hash;
        }
    }
}
