using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
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
    [StandardModule]
    public static class ModDownload
    {
        // Note: this type is marked as 'beforefieldinit'.
        static ModDownload()
        {
        }

        public static ModNet.NetFile DlClientJarGet(ModMinecraft.McInstance Instance, bool ReturnNothingOnFileUseable)
        {
            try
            {
                while (!string.IsNullOrEmpty(Instance.InheritName))
                {
                    Instance = new ModMinecraft.McInstance(Instance.InheritName);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "获取底层继承版本失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModDownload.vb");
            }
            if (Instance.JsonObject["downloads"] == null || Instance.JsonObject["downloads"]["client"] == null || Instance.JsonObject["downloads"]["client"]["url"] == null)
            {
                throw new Exception("底层版本 " + Instance.Name + " 中无 jar 文件下载信息");
            }
            FileChecker fileChecker = new FileChecker
            {
                MinSize = 1024L,
                ActualSize = (long)(Instance.JsonObject["downloads"]["client"]["size"] ?? (-1)),
                Hash = (string)Instance.JsonObject["downloads"]["client"]["sha1"]
            };
            ModNet.NetFile netFile;
            if (ReturnNothingOnFileUseable && fileChecker.Check(Instance.PathVersion + Instance.Name + ".jar") == null)
            {
                netFile = null;
            }
            else
            {
                string text = (string)Instance.JsonObject["downloads"]["client"]["url"];
                netFile = new ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(text), Instance.PathVersion + Instance.Name + ".jar", fileChecker, false);
            }
            return netFile;
        }

        public static ModNet.NetFile DlClientAssetIndexGet(ModMinecraft.McInstance Instance)
        {
            while (!string.IsNullOrEmpty(Instance.InheritName))
            {
                Instance = new ModMinecraft.McInstance(Instance.InheritName);
            }
            JToken jtoken = ModMinecraft.McAssetsGetIndex(Instance, true, true);
            string text = ModMinecraft.McFolderSelected + "assets\\indexes\\" + jtoken["id"].ToString() + ".json";
            Logger.Info(string.Format("版本 {0} 对应的资源文件索引为 {1}", Instance.Name, jtoken["id"]), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModDownload.vb");
            string text2 = (string)(jtoken["url"] ?? "");
            ModNet.NetFile netFile;
            if (Operators.CompareString(text2, "", false) == 0)
            {
                netFile = null;
            }
            else
            {
                netFile = new ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(text2), text, new FileChecker
                {
                    IsJson = true
                }, false);
            }
            return netFile;
        }

        public static List<ModLoader.LoaderBase> DlClientFix(ModMinecraft.McInstance Instance, bool CheckAssetsHash, bool DownloadAssetIndexInBackground)
        {
            ModDownload._Closure___3_0 CS_locals1 = new ModDownload._Closure___3_0();
            CS_locals1._VB_Local_Instance = Instance;
            CS_locals1._VB_Local_CheckAssetsHash = CheckAssetsHash;
            CS_locals1._VB_Local_DownloadAssetIndexInBackground = DownloadAssetIndexInBackground;
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            List<ModLoader.LoaderBase> list2 = new List<ModLoader.LoaderBase>
            {
                new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("分析缺失支持库文件", new Action<ModLoader.LoaderTask<string, List<ModNet.NetFile>>>(CS_locals1._Lambda___0), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 1.0
                },
                new ModNet.LoaderDownload("下载支持库文件", new List<ModNet.NetFile>())
                {
                    ProgressWeight = 15.0
                }
            };
            list.Add(new ModLoader.LoaderCombo<string>("下载支持库文件（主加载器）", list2)
            {
                Block = false,
                Show = false,
                ProgressWeight = 16.0
            });
            if (Conversions.ToBoolean(ModMinecraft.ShouldIgnoreFileCheck(CS_locals1._VB_Local_Instance)))
            {
                Logger.Warn("设置要求跳过所有 Assets 检查，文件可能不是最新的", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModDownload.vb");
            }
            else
            {
                ModDownload._Closure___3_1 CS_locals2 = new ModDownload._Closure___3_1();
                CS_locals2._VB_NonLocal__VB_Closure_2 = CS_locals1;
                List<ModLoader.LoaderBase> list3 = new List<ModLoader.LoaderBase>();
                CS_locals2._VB_Local_TempAddressMain = null;
                CS_locals2._VB_Local_RealAddressMain = null;
                list3.Add(new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("分析资源文件索引地址", new Action<ModLoader.LoaderTask<string, List<ModNet.NetFile>>>(CS_locals2._Lambda___1), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 0.5,
                    Show = false
                });
                list3.Add(new ModNet.LoaderDownload("下载资源文件索引", new List<ModNet.NetFile>())
                {
                    ProgressWeight = 2.0
                });
                list3.Add(new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("获取资源文件列表", new Action<ModLoader.LoaderTask<string, List<ModNet.NetFile>>>(CS_locals2._Lambda___2), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 0.01
                });
                list3.Add(new ModNet.LoaderDownload("下载资源文件", new List<ModNet.NetFile>())
                {
                    ProgressWeight = 25.0
                });
                list.Add(new ModLoader.LoaderCombo<string>("下载资源文件（主加载器）", list3)
                {
                    Block = false,
                    Show = false,
                    ProgressWeight = 27.0
                });
                if (CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_DownloadAssetIndexInBackground)
                {
                    ModDownload._Closure___3_2 CS_locals3 = new ModDownload._Closure___3_2();
                    CS_locals3._VB_NonLocal__VB_Closure_3 = CS_locals2;
                    CS_locals3._VB_Local_RealAddressBg = null;
                    CS_locals3._VB_Local_TempAddressBg = null;
                    ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>("后台更新资源文件索引", new List<ModLoader.LoaderBase>
                    {
                        new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("后台分析资源文件索引地址", new Action<ModLoader.LoaderTask<string, List<ModNet.NetFile>>>(CS_locals3._Lambda___3), null, ThreadPriority.Normal),
                        new ModNet.LoaderDownload("后台下载资源文件索引", new List<ModNet.NetFile>())
                    });
                    Logger.Info("开始后台检查资源文件索引", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModDownload.vb");
                    loaderCombo.Start(null, false);
                }
            }
            return list;
        }

        public static List<int> AllDrops
        {
            get
            {
                if (ModDownload._AllDrops == null)
                {
                    ModDownload._AllDrops = Settings.Get<string>("CacheDrops", null).ToString().Split(",", true)
                        .Select<string, int>((ModDownload._Closure___._I5_0 == null) ? (ModDownload._Closure___._I5_0 = new Func<string, int>(ModDownload._Closure___._I._Lambda___5_0)) : ModDownload._Closure___._I5_0)
                        .ToList<int>();
                }
                if (!ModDownload._AllDrops.Any<int>())
                {
                    return null;
                }
                return ModDownload._AllDrops;
            }
            set
            {
                ModDownload._AllDrops = value;
                Settings.Set("CacheDrops", value.Join(','), null);
            }
        }

        private static void DlClientListMain(ModLoader.LoaderTask<string, ModDownload.DlClientListResult> Loader)
        {
            int num = Settings.Get<int>("ToolDownloadVersion", null);
            if (num != 0)
            {
                if (num != 1)
                {
                    ModDownload.DlSourceLoader<string, ModDownload.DlClientListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<string, ModDownload.DlClientListResult>, int>>
                    {
                        new KeyValuePair<ModLoader.LoaderTask<string, ModDownload.DlClientListResult>, int>(ModDownload.DlClientListMojangLoader, 60),
                        new KeyValuePair<ModLoader.LoaderTask<string, ModDownload.DlClientListResult>, int>(ModDownload.DlClientListBmclapiLoader, 120)
                    }, Loader.IsForceRestarting);
                }
                else
                {
                    ModDownload.DlSourceLoader<string, ModDownload.DlClientListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<string, ModDownload.DlClientListResult>, int>>
                    {
                        new KeyValuePair<ModLoader.LoaderTask<string, ModDownload.DlClientListResult>, int>(ModDownload.DlClientListMojangLoader, 5),
                        new KeyValuePair<ModLoader.LoaderTask<string, ModDownload.DlClientListResult>, int>(ModDownload.DlClientListBmclapiLoader, 35)
                    }, Loader.IsForceRestarting);
                }
            }
            else
            {
                ModDownload.DlSourceLoader<string, ModDownload.DlClientListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<string, ModDownload.DlClientListResult>, int>>
                {
                    new KeyValuePair<ModLoader.LoaderTask<string, ModDownload.DlClientListResult>, int>(ModDownload.DlClientListBmclapiLoader, 30),
                    new KeyValuePair<ModLoader.LoaderTask<string, ModDownload.DlClientListResult>, int>(ModDownload.DlClientListMojangLoader, 90)
                }, Loader.IsForceRestarting);
            }
            List<int> list = new List<int>();
            try
            {
                foreach (JToken jtoken in ((IEnumerable<JToken>)Loader.Output.Value["versions"]))
                {
                    string text = ((JObject)jtoken)["id"].ToString();
                    if (!text.Contains("-"))
                    {
                        int num2 = McVersion.VersionToDrop(text);
                        if (num2 != 209)
                        {
                            list.Add(num2);
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
            ModDownload.AllDrops = list.Distinct<int>().OrderByDescending<int, int>((ModDownload._Closure___._I10_0 == null) ? (ModDownload._Closure___._I10_0 = new Func<int, int>(ModDownload._Closure___._I._Lambda___10_0)) : ModDownload._Closure___._I10_0).ToList<int>();
        }

        private static void DlClientListMojangMain(ModLoader.LoaderTask<string, ModDownload.DlClientListResult> Loader)
        {
            long timeMs = ModBase.GetTimeMs();
            JObject jobject = (JObject)ModNet.NetRequestByClientRetry("https://launchermeta.mojang.com/mc/game/version_manifest.json", null, null, null, null, "*/*", null, true, null, false).DeserializeJson();
            try
            {
                if (((JArray)jobject["versions"]).Count < 200)
                {
                    throw new Exception("获取到的版本列表长度不足（" + jobject.ToString() + "）");
                }
                if (!ModDownload.DlPreferMojang)
                {
                    long num = checked(ModBase.GetTimeMs() - timeMs);
                    ModDownload.DlPreferMojang = num < 4000L;
                    Logger.Info(string.Format("Mojang 官方源加载耗时：{0}ms，{1}", num, ModDownload.DlPreferMojang ? "可优先使用官方源" : "不优先使用官方源"), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModDownload.vb");
                }
                Loader.Output = new ModDownload.DlClientListResult
                {
                    IsOfficial = true,
                    SourceName = "Mojang 官方源",
                    Value = jobject
                };
                if (ModDownload._STATIC_DlClientListMojangMain_011151286FC2E118758_IsHinted_Init == null)
                {
                    Interlocked.CompareExchange<StaticLocalInitFlag>(ref ModDownload._STATIC_DlClientListMojangMain_011151286FC2E118758_IsHinted_Init, new StaticLocalInitFlag(), null);
                }
                bool flag = false;
                try
                {
                    Monitor.Enter(ModDownload._STATIC_DlClientListMojangMain_011151286FC2E118758_IsHinted_Init, ref flag);
                    if (ModDownload._STATIC_DlClientListMojangMain_011151286FC2E118758_IsHinted_Init.State == 0)
                    {
                        ModDownload._STATIC_DlClientListMojangMain_011151286FC2E118758_IsHinted_Init.State = 2;
                        ModDownload._STATIC_DlClientListMojangMain_011151286FC2E118758_IsHinted = false;
                    }
                    else if (ModDownload._STATIC_DlClientListMojangMain_011151286FC2E118758_IsHinted_Init.State == 2)
                    {
                        throw new IncompleteInitialization();
                    }
                }
                finally
                {
                    ModDownload._STATIC_DlClientListMojangMain_011151286FC2E118758_IsHinted_Init.State = 1;
                    if (flag)
                    {
                        Monitor.Exit(ModDownload._STATIC_DlClientListMojangMain_011151286FC2E118758_IsHinted_Init);
                    }
                }
                string text = (string)jobject["latest"]["snapshot"];
                if (Settings.Get<bool>("ToolUpdateSnapshot", null) && Operators.CompareString(Settings.Get<string>("ToolUpdateSnapshotLast", null), "", false) != 0 && Operators.CompareString(Settings.Get<string>("ToolUpdateSnapshotLast", null), text, false) != 0 && !ModDownload._STATIC_DlClientListMojangMain_011151286FC2E118758_IsHinted)
                {
                    ModDownload._STATIC_DlClientListMojangMain_011151286FC2E118758_IsHinted = true;
                    ModMinecraft.McDownloadClientUpdateHint(text, jobject);
                }
                Settings.Set("ToolUpdateSnapshotLast", text ?? "Nothing", null);
                text = (string)jobject["latest"]["release"];
                if (Settings.Get<bool>("ToolUpdateRelease", null) && Operators.CompareString(Settings.Get<string>("ToolUpdateReleaseLast", null), "", false) != 0 && Operators.CompareString(Settings.Get<string>("ToolUpdateReleaseLast", null), text, false) != 0 && !ModDownload._STATIC_DlClientListMojangMain_011151286FC2E118758_IsHinted)
                {
                    ModDownload._STATIC_DlClientListMojangMain_011151286FC2E118758_IsHinted = true;
                    ModMinecraft.McDownloadClientUpdateHint(text, jobject);
                }
                Settings.Set("ToolUpdateReleaseLast", text, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Minecraft 官方源版本列表解析失败", ex);
            }
        }

        private static void DlClientListBmclapiMain(ModLoader.LoaderTask<string, ModDownload.DlClientListResult> Loader)
        {
            JObject jobject = (JObject)ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/mc/game/version_manifest.json", null, null, null, null, "*/*", null, true, null, false).DeserializeJson();
            try
            {
                JArray jarray = (JArray)jobject["versions"];
                if (jarray.Count < 200)
                {
                    throw new Exception("获取到的版本列表长度不足（" + jobject.ToString() + "）");
                }
                if (FileUtils.Exists(ModBase.PathTemp + "Cache\\download.json"))
                {
                    jarray.Merge(FileUtils.ReadAsJson(ModBase.PathTemp + "Cache\\download.json", null, null));
                }
                if (!string.IsNullOrEmpty(Loader.Input))
                {
                    ModDownload._Closure___14_0 CS_locals1 = new ModDownload._Closure___14_0();
                    CS_locals1._VB_Local_Id = Loader.Input;
                    if (ModDownload.DlClientListLoader.Output.Value != null && !ModDownload.DlClientListLoader.Output.Value["versions"].Any<JToken>(new Func<JToken, bool>(CS_locals1._Lambda___0)))
                    {
                        throw new Exception("BMCLAPI 源未包含目标版本 " + CS_locals1._VB_Local_Id);
                    }
                }
                Loader.Output = new ModDownload.DlClientListResult
                {
                    IsOfficial = false,
                    SourceName = "BMCLAPI",
                    Value = jobject
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Minecraft BMCLAPI 版本列表解析失败（" + jobject.ToString() + "）", ex);
            }
        }

        public static object DlClientListGet(string Id)
        {
            object obj;
            try
            {
                Id = Id.Replace("_", "-");
                if (Operators.CompareString(Id, "1.0", false) != 0 && Id.EndsWithF(".0", false))
                {
                    Id = Strings.Left(Id, checked(Id.Length - 2));
                }
                switch (ModDownload.DlClientListLoader.State)
                {
                case ModBase.LoadState.Waiting:
                case ModBase.LoadState.Failed:
                case ModBase.LoadState.Canceled:
                    ModDownload.DlClientListLoader.WaitForExit(Id, null, true, default(CancellationToken), null);
                    break;
                case ModBase.LoadState.Loading:
                    ModDownload.DlClientListLoader.WaitForExit(Id, null, false, default(CancellationToken), null);
                    break;
                case ModBase.LoadState.Finished:
                    try
                    {
                        foreach (JToken jtoken in ((IEnumerable<JToken>)ModDownload.DlClientListLoader.Output.Value["versions"]))
                        {
                            JObject jobject = (JObject)jtoken;
                            if ((string)jobject["id"] == Id)
                            {
                                return jobject["url"].ToString();
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
                    ModDownload.DlClientListLoader.WaitForExit(Id, null, true, default(CancellationToken), null);
                    break;
                }
                try
                {
                    foreach (JToken jtoken2 in ((IEnumerable<JToken>)ModDownload.DlClientListLoader.Output.Value["versions"]))
                    {
                        JObject jobject2 = (JObject)jtoken2;
                        if ((string)jobject2["id"] == Id)
                        {
                            return jobject2["url"].ToString();
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
                Logger.Warn(string.Format("未发现版本 {0} 的 json 下载地址，版本列表返回为：{1}{2}", Id, "\r\n", ModDownload.DlClientListLoader.Output.Value), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModDownload.vb");
                obj = null;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, string.Format("获取版本 {0} 的 json 下载地址失败", Id), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModDownload.vb");
                obj = null;
            }
            return obj;
        }

        private static void DlOptiFineListMain(ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult> Loader)
        {
            int num = Settings.Get<int>("ToolDownloadVersion", null);
            if (num == 0)
            {
                ModDownload.DlSourceLoader<int, ModDownload.DlOptiFineListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult>, int>>
                {
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult>, int>(ModDownload.DlOptiFineListBmclapiLoader, 30),
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult>, int>(ModDownload.DlOptiFineListOfficialLoader, 90)
                }, Loader.IsForceRestarting);
                return;
            }
            if (num != 1)
            {
                ModDownload.DlSourceLoader<int, ModDownload.DlOptiFineListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult>, int>>
                {
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult>, int>(ModDownload.DlOptiFineListOfficialLoader, 60),
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult>, int>(ModDownload.DlOptiFineListBmclapiLoader, 120)
                }, Loader.IsForceRestarting);
                return;
            }
            ModDownload.DlSourceLoader<int, ModDownload.DlOptiFineListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult>, int>>
            {
                new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult>, int>(ModDownload.DlOptiFineListOfficialLoader, 5),
                new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult>, int>(ModDownload.DlOptiFineListBmclapiLoader, 35)
            }, Loader.IsForceRestarting);
        }

        private static void DlOptiFineListOfficialMain(ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult> Loader)
        {
            string text = ModNet.NetRequestByClientRetry("https://optifine.net/downloads", null, null, null, null, "*/*", null, false, Encoding.Default, false);
            if (text.Length < 200)
            {
                throw new Exception("获取到的版本列表长度不足（" + text + "）");
            }
            checked
            {
                try
                {
                    List<string> list = text.RegexSearch("(?<=colForge'>)[^<]*", RegexOptions.None).ToList<string>();
                    List<string> list2 = text.RegexSearch("(?<=colDate'>)[^<]+", RegexOptions.None).ToList<string>();
                    List<string> list3 = text.RegexSearch("(?<=OptiFine_)[0-9A-Za-z_.]+(?=.jar\")", RegexOptions.None).ToList<string>();
                    if (list2.Count != list3.Count)
                    {
                        throw new Exception("版本与发布时间数据无法对应");
                    }
                    if (list.Count != list3.Count)
                    {
                        throw new Exception("版本与 Forge 兼容数据无法对应");
                    }
                    if (list2.Count < 10)
                    {
                        throw new Exception("获取到的版本数量不足（" + text + "）");
                    }
                    List<ModDownload.DlOptiFineListEntry> list4 = new List<ModDownload.DlOptiFineListEntry>();
                    int num = list2.Count - 1;
                    for (int i = 0; i <= num; i++)
                    {
                        list3[i] = list3[i].Replace("_", " ");
                        ModDownload.DlOptiFineListEntry dlOptiFineListEntry = new ModDownload.DlOptiFineListEntry
                        {
                            DisplayName = list3[i].Replace("HD U ", "").Replace(".0 ", " "),
                            ReleaseTime = new string[]
                            {
                                list2[i].Split(".", false)[2],
                                list2[i].Split(".", false)[1],
                                list2[i].Split(".", false)[0]
                            }.Join('/'),
                            IsPreview = list3[i].ContainsIgnoreCase("pre"),
                            Inherit = list3[i].ToString().Split(" ", false)[0],
                            FileName = (list3[i].ContainsIgnoreCase("pre") ? "preview_" : "") + "OptiFine_" + list3[i].Replace(" ", "_") + ".jar",
                            RequiredForgeVersion = list[i].Replace("Forge ", "").Replace("#", "")
                        };
                        if (dlOptiFineListEntry.RequiredForgeVersion.Contains("N/A"))
                        {
                            dlOptiFineListEntry.RequiredForgeVersion = null;
                        }
                        dlOptiFineListEntry.InstanceName = dlOptiFineListEntry.Inherit + "-OptiFine_" + list3[i].AfterFirst(" ", false).Replace(" ", "_");
                        list4.Add(dlOptiFineListEntry);
                    }
                    Loader.Output = new ModDownload.DlOptiFineListResult
                    {
                        IsOfficial = true,
                        SourceName = "OptiFine 官方源",
                        Value = list4
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception("OptiFine 官方源版本列表解析失败（" + text + "）", ex);
                }
            }
        }

        private static void DlOptiFineListBmclapiMain(ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult> Loader)
        {
            JArray jarray = (JArray)ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/optifine/versionList", null, null, null, null, "*/*", null, true, null, false).DeserializeJson();
            try
            {
                List<ModDownload.DlOptiFineListEntry> list = new List<ModDownload.DlOptiFineListEntry>();
                try
                {
                    foreach (JToken jtoken in jarray)
                    {
                        JObject jobject = (JObject)jtoken;
                        ModDownload.DlOptiFineListEntry dlOptiFineListEntry = new ModDownload.DlOptiFineListEntry
                        {
                            DisplayName = (jobject["mcversion"].ToString() + jobject["type"].ToString().Replace("HD_U", "").Replace("_", " ") + " " + jobject["patch"].ToString()).Replace(".0 ", " "),
                            ReleaseTime = "",
                            IsPreview = jobject["patch"].ToString().ContainsIgnoreCase("pre"),
                            Inherit = jobject["mcversion"].ToString(),
                            FileName = jobject["filename"].ToString(),
                            RequiredForgeVersion = (jobject["forge"] ?? "").ToString().Replace("Forge ", "").Replace("#", "")
                        };
                        if (dlOptiFineListEntry.RequiredForgeVersion.Contains("N/A"))
                        {
                            dlOptiFineListEntry.RequiredForgeVersion = null;
                        }
                        dlOptiFineListEntry.InstanceName = dlOptiFineListEntry.Inherit + "-OptiFine_" + (jobject["type"].ToString() + " " + jobject["patch"].ToString()).Replace(".0 ", " ").Replace(" ", "_").Replace(dlOptiFineListEntry.Inherit + "_", "");
                        list.Add(dlOptiFineListEntry);
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
                Loader.Output = new ModDownload.DlOptiFineListResult
                {
                    IsOfficial = false,
                    SourceName = "BMCLAPI",
                    Value = list
                };
            }
            catch (Exception ex)
            {
                throw new Exception("OptiFine BMCLAPI 版本列表解析失败（" + jarray.ToString() + "）", ex);
            }
        }

        private static void DlForgeListMain(ModLoader.LoaderTask<int, ModDownload.DlForgeListResult> Loader)
        {
            int num = Settings.Get<int>("ToolDownloadVersion", null);
            if (num == 0)
            {
                ModDownload.DlSourceLoader<int, ModDownload.DlForgeListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlForgeListResult>, int>>
                {
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlForgeListResult>, int>(ModDownload.DlForgeListBmclapiLoader, 30),
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlForgeListResult>, int>(ModDownload.DlForgeListOfficialLoader, 90)
                }, Loader.IsForceRestarting);
                return;
            }
            if (num != 1)
            {
                ModDownload.DlSourceLoader<int, ModDownload.DlForgeListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlForgeListResult>, int>>
                {
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlForgeListResult>, int>(ModDownload.DlForgeListOfficialLoader, 60),
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlForgeListResult>, int>(ModDownload.DlForgeListBmclapiLoader, 120)
                }, Loader.IsForceRestarting);
                return;
            }
            ModDownload.DlSourceLoader<int, ModDownload.DlForgeListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlForgeListResult>, int>>
            {
                new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlForgeListResult>, int>(ModDownload.DlForgeListOfficialLoader, 5),
                new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlForgeListResult>, int>(ModDownload.DlForgeListBmclapiLoader, 35)
            }, Loader.IsForceRestarting);
        }

        private static void DlForgeListOfficialMain(ModLoader.LoaderTask<int, ModDownload.DlForgeListResult> Loader)
        {
            string text = ModNet.NetRequestByClientRetry("https://files.minecraftforge.net/maven/net/minecraftforge/forge/index_1.2.4.html", null, null, null, null, "text/html", null, false, Encoding.Default, true);
            if (text.Length < 200)
            {
                throw new Exception("获取到的版本列表长度不足（" + text + "）");
            }
            List<string> list = text.RegexSearch("(?<=a href=\"index_)[0-9.]+((_pre|[_-]snapshot[_-]?)[0-9]?)?(?=.html)", RegexOptions.None).ToList<string>();
            list.Add("1.2.4");
            if (list.Count < 10)
            {
                throw new Exception("获取到的版本数量不足（" + text + "）");
            }
            Loader.Output = new ModDownload.DlForgeListResult
            {
                IsOfficial = true,
                SourceName = "Forge 官方源",
                Value = list
            };
        }

        private static void DlForgeListBmclapiMain(ModLoader.LoaderTask<int, ModDownload.DlForgeListResult> Loader)
        {
            List<string> list = ((JArray)ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/forge/minecraft", null, null, null, null, "*/*", null, true, Encoding.Default, false).DeserializeJson()).Select<JToken, string>((ModDownload._Closure___._I30_0 == null) ? (ModDownload._Closure___._I30_0 = new Func<JToken, string>(ModDownload._Closure___._I._Lambda___30_0)) : ModDownload._Closure___._I30_0).ToList<string>();
            if (list.Count < 10)
            {
                throw new Exception("获取到的版本数量不足");
            }
            Loader.Output = new ModDownload.DlForgeListResult
            {
                IsOfficial = false,
                SourceName = "BMCLAPI",
                Value = list
            };
        }

        public static void DlForgeVersionMain(ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>> Loader)
        {
            ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>> loaderTask = new ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>("DlForgeVersion Official", new Action<ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>>(ModDownload.DlForgeVersionOfficialMain), null, ThreadPriority.Normal);
            ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>> loaderTask2 = new ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>("DlForgeVersion Bmclapi", new Action<ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>>(ModDownload.DlForgeVersionBmclapiMain), null, ThreadPriority.Normal);
            int num = Settings.Get<int>("ToolDownloadVersion", null);
            if (num == 0)
            {
                ModDownload.DlSourceLoader<string, List<ModDownload.DlForgeVersionEntry>>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>, int>>
                {
                    new KeyValuePair<ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>, int>(loaderTask2, 30),
                    new KeyValuePair<ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>, int>(loaderTask, 90)
                }, Loader.IsForceRestarting);
                return;
            }
            if (num != 1)
            {
                ModDownload.DlSourceLoader<string, List<ModDownload.DlForgeVersionEntry>>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>, int>>
                {
                    new KeyValuePair<ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>, int>(loaderTask, 60),
                    new KeyValuePair<ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>, int>(loaderTask2, 120)
                }, Loader.IsForceRestarting);
                return;
            }
            ModDownload.DlSourceLoader<string, List<ModDownload.DlForgeVersionEntry>>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>, int>>
            {
                new KeyValuePair<ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>, int>(loaderTask, 5),
                new KeyValuePair<ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>, int>(loaderTask2, 35)
            }, Loader.IsForceRestarting);
        }

        public static void DlForgeVersionOfficialMain(ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>> Loader)
        {
            string text;
            try
            {
                text = ModNet.NetRequestByLoader("https://files.minecraftforge.net/maven/net/minecraftforge/forge/index_" + Loader.Input.Replace("-", "_") + ".html", 45000, false, true);
            }
            catch (Exception ex)
            {
                if (ex.GetDisplay(false).Contains("(404)"))
                {
                    throw new Exception("无");
                }
                throw;
            }
            if (text.Length < 1000)
            {
                throw new Exception("获取到的版本列表长度不足（" + text + "）");
            }
            List<ModDownload.DlForgeVersionEntry> list = new List<ModDownload.DlForgeVersionEntry>();
            checked
            {
                try
                {
                    string[] array = text.Substring(0, text.LastIndexOfF("</table>", false)).Split("<td class=\"download-version", false);
                    int num = array.Count<string>() - 1;
                    for (int i = 1; i <= num; i++)
                    {
                        string text2 = array[i];
                        try
                        {
                            string text3 = text2.RegexSeek("(?<=[^(0-9)]+)[0-9\\.]+", RegexOptions.None);
                            bool flag = text2.Contains("fa promo-recommended");
                            string input = Loader.Input;
                            string text4 = text2.RegexSeek(string.Format("(?<=-{0}-)[^-\"]+(?=-[a-z]+.[a-z]{{3}})", text3), RegexOptions.None);
                            if (string.IsNullOrWhiteSpace(text4))
                            {
                                text4 = null;
                            }
                            string[] array2 = text2.RegexSeek("(?<=\"download-time\" title=\")[^\"]+", RegexOptions.None).Split(" -:".ToCharArray());
                            string text5 = new DateTime(Conversions.ToInteger(array2[0]), Conversions.ToInteger(array2[1]), Conversions.ToInteger(array2[2]), Conversions.ToInteger(array2[3]), Conversions.ToInteger(array2[4]), Conversions.ToInteger(array2[5]), 0, DateTimeKind.Utc).ToLocalTime().ToString("yyyy'/'MM'/'dd HH':'mm");
                            string text6;
                            string text7;
                            if (text2.Contains("classifier-installer\""))
                            {
                                text2 = text2.Substring(text2.IndexOfF("installer.jar", false));
                                text6 = text2.RegexSeek("(?<=MD5:</strong> )[^<]+", RegexOptions.None);
                                text7 = "installer";
                            }
                            else if (text2.Contains("classifier-universal\""))
                            {
                                text2 = text2.Substring(text2.IndexOfF("universal.zip", false));
                                text6 = text2.RegexSeek("(?<=MD5:</strong> )[^<]+", RegexOptions.None);
                                text7 = "universal";
                            }
                            else
                            {
                                if (!text2.Contains("client.zip"))
                                {
                                    goto IL_028A;
                                }
                                text2 = text2.Substring(text2.IndexOfF("client.zip", false));
                                text6 = text2.RegexSeek("(?<=MD5:</strong> )[^<]+", RegexOptions.None);
                                text7 = "client";
                            }
                            list.Add(new ModDownload.DlForgeVersionEntry(text3, text4, input)
                            {
                                Category = text7,
                                IsRecommended = flag,
                                Hash = text6.ReplaceLineEndings("", false),
                                ReleaseTime = text5
                            });
                        }
                        catch (Exception ex2)
                        {
                            throw new Exception("Forge 官方源版本信息提取失败（" + text2 + "）", ex2);
                        }
                        IL_028A:;
                    }
                }
                catch (Exception ex3)
                {
                    throw new Exception("Forge 官方源版本列表解析失败（" + text + "）", ex3);
                }
                if (!list.Any<ModDownload.DlForgeVersionEntry>())
                {
                    throw new Exception("无");
                }
                Loader.Output = list;
            }
        }

        public static void DlForgeVersionBmclapiMain(ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>> Loader)
        {
            JArray jarray = (JArray)ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/forge/minecraft/" + Loader.Input.Replace("-", "_"), null, null, null, null, "*/*", null, true, null, false).DeserializeJson();
            List<ModDownload.DlForgeVersionEntry> list = new List<ModDownload.DlForgeVersionEntry>();
            try
            {
                string text = ModDownloadLib.McDownloadForgeRecommendedGet(Loader.Input);
                try
                {
                    foreach (JToken jtoken in jarray)
                    {
                        JObject jobject = (JObject)jtoken;
                        string text2 = null;
                        string text3 = "unknown";
                        int num = -1;
                        try
                        {
                            foreach (JToken jtoken2 in ((IEnumerable<JToken>)jobject["files"]))
                            {
                                JObject jobject2 = (JObject)jtoken2;
                                string text4 = jobject2["category"].ToString();
                                if (Operators.CompareString(text4, "installer", false) != 0)
                                {
                                    if (Operators.CompareString(text4, "universal", false) != 0)
                                    {
                                        if (Operators.CompareString(text4, "client", false) == 0)
                                        {
                                            if (num <= 0 && Operators.CompareString(jobject2["format"].ToString(), "zip", false) == 0)
                                            {
                                                text2 = (string)jobject2["hash"];
                                                text3 = "client";
                                                num = 0;
                                            }
                                        }
                                    }
                                    else if (num <= 1 && Operators.CompareString(jobject2["format"].ToString(), "zip", false) == 0)
                                    {
                                        text2 = (string)jobject2["hash"];
                                        text3 = "universal";
                                        num = 1;
                                    }
                                }
                                else if (Operators.CompareString(jobject2["format"].ToString(), "jar", false) == 0)
                                {
                                    text2 = (string)jobject2["hash"];
                                    text3 = "installer";
                                    num = 2;
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
                        string text5 = (string)jobject["branch"];
                        string text6 = (string)jobject["version"];
                        ModDownload.DlForgeVersionEntry dlForgeVersionEntry = new ModDownload.DlForgeVersionEntry(text6, text5, Loader.Input)
                        {
                            Hash = text2,
                            Category = text3,
                            IsRecommended = (Operators.CompareString(text, text6, false) == 0)
                        };
                        jobject["modified"].ToString().Split(new char[] { '-', 'T', ':', '.', ' ', '/' });
                        dlForgeVersionEntry.ReleaseTime = jobject["modified"].ToObject<DateTime>().ToLocalTime().ToString("yyyy'/'MM'/'dd HH':'mm");
                        list.Add(dlForgeVersionEntry);
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
            }
            catch (Exception ex)
            {
                throw new Exception("Forge BMCLAPI 版本列表解析失败（" + jarray.ToString() + "）", ex);
            }
            if (!list.Any<ModDownload.DlForgeVersionEntry>())
            {
                throw new Exception("无");
            }
            Loader.Output = list;
        }

        private static void DlNeoForgeListMain(ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult> Loader)
        {
            int num = Settings.Get<int>("ToolDownloadVersion", null);
            if (num == 0)
            {
                ModDownload.DlSourceLoader<int, ModDownload.DlNeoForgeListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult>, int>>
                {
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult>, int>(ModDownload.DlNeoForgeListBmclapiLoader, 30),
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult>, int>(ModDownload.DlNeoForgeListOfficialLoader, 90)
                }, Loader.IsForceRestarting);
                return;
            }
            if (num != 1)
            {
                ModDownload.DlSourceLoader<int, ModDownload.DlNeoForgeListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult>, int>>
                {
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult>, int>(ModDownload.DlNeoForgeListOfficialLoader, 60),
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult>, int>(ModDownload.DlNeoForgeListBmclapiLoader, 120)
                }, Loader.IsForceRestarting);
                return;
            }
            ModDownload.DlSourceLoader<int, ModDownload.DlNeoForgeListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult>, int>>
            {
                new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult>, int>(ModDownload.DlNeoForgeListOfficialLoader, 5),
                new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult>, int>(ModDownload.DlNeoForgeListBmclapiLoader, 35)
            }, Loader.IsForceRestarting);
        }

        private static void DlNeoForgeListOfficialMain(ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult> Loader)
        {
            string text = ModNet.NetRequestByClientRetry("https://maven.neoforged.net/api/maven/versions/releases/net/neoforged/neoforge", null, null, null, null, "*/*", null, true, null, false);
            string text2 = ModNet.NetRequestByClientRetry("https://maven.neoforged.net/api/maven/versions/releases/net/neoforged/forge", null, null, null, null, "*/*", null, true, null, false);
            if (text.Length < 100 || text2.Length < 100)
            {
                throw new Exception("获取到的版本列表长度不足（" + text + "）");
            }
            try
            {
                Loader.Output = new ModDownload.DlNeoForgeListResult
                {
                    IsOfficial = true,
                    SourceName = "NeoForge 官方源",
                    Value = ModDownload.GetNeoForgeEntries(text, text2)
                };
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat(new string[] { "NeoForge 官方源版本列表解析失败（", text, "\r\n\r\n", text2, "）" }), ex);
            }
        }

        public static void DlNeoForgeListBmclapiMain(ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult> Loader)
        {
            string text = ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/neoforge/meta/api/maven/details/releases/net/neoforged/neoforge", null, null, null, null, "*/*", null, true, null, false);
            string text2 = ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/neoforge/meta/api/maven/details/releases/net/neoforged/forge", null, null, null, null, "*/*", null, true, null, false);
            if (text.Length < 100 || text2.Length < 100)
            {
                throw new Exception("获取到的版本列表长度不足（" + text + "）");
            }
            try
            {
                Loader.Output = new ModDownload.DlNeoForgeListResult
                {
                    IsOfficial = true,
                    SourceName = "BMCLAPI",
                    Value = ModDownload.GetNeoForgeEntries(text, text2)
                };
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat(new string[] { "NeoForge BMCLAPI 版本列表解析失败（", text, "\r\n\r\n", text2, "）" }), ex);
            }
        }

        private static List<ModDownload.DlNeoForgeListEntry> GetNeoForgeEntries(string LatestJson, string LatestLegacyJson)
        {
            List<ModDownload.DlNeoForgeListEntry> list = (LatestLegacyJson + LatestJson).RegexSearch("(?<=\")(1\\.20\\.1-)?\\d+\\.[^\\.]+\\.\\d+(\\.\\d+)?(-(beta|alpha)(\\.\\d+)?)?(\\+snapshot-\\d+)?(?=\")", RegexOptions.None).Where<string>((ModDownload._Closure___._I44_0 == null) ? (ModDownload._Closure___._I44_0 = new Func<string, bool>(ModDownload._Closure___._I._Lambda___44_0)) : ModDownload._Closure___._I44_0).Select<string, ModDownload.DlNeoForgeListEntry>((ModDownload._Closure___._I44_1 == null) ? (ModDownload._Closure___._I44_1 = new Func<string, ModDownload.DlNeoForgeListEntry>(ModDownload._Closure___._I._Lambda___44_1)) : ModDownload._Closure___._I44_1)
                .OrderByDescending<ModDownload.DlNeoForgeListEntry, ModDownload.DlNeoForgeListEntry>((ModDownload._Closure___._I44_2 == null) ? (ModDownload._Closure___._I44_2 = new Func<ModDownload.DlNeoForgeListEntry, ModDownload.DlNeoForgeListEntry>(ModDownload._Closure___._I._Lambda___44_2)) : ModDownload._Closure___._I44_2)
                .ToList<ModDownload.DlNeoForgeListEntry>();
            if (!list.Any<ModDownload.DlNeoForgeListEntry>())
            {
                throw new Exception("无");
            }
            return list;
        }

        private static void DlLiteLoaderListMain(ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult> Loader)
        {
            int num = Settings.Get<int>("ToolDownloadVersion", null);
            if (num == 0)
            {
                ModDownload.DlSourceLoader<int, ModDownload.DlLiteLoaderListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult>, int>>
                {
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult>, int>(ModDownload.DlLiteLoaderListBmclapiLoader, 30),
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult>, int>(ModDownload.DlLiteLoaderListOfficialLoader, 90)
                }, Loader.IsForceRestarting);
                return;
            }
            if (num != 1)
            {
                ModDownload.DlSourceLoader<int, ModDownload.DlLiteLoaderListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult>, int>>
                {
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult>, int>(ModDownload.DlLiteLoaderListOfficialLoader, 60),
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult>, int>(ModDownload.DlLiteLoaderListBmclapiLoader, 120)
                }, Loader.IsForceRestarting);
                return;
            }
            ModDownload.DlSourceLoader<int, ModDownload.DlLiteLoaderListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult>, int>>
            {
                new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult>, int>(ModDownload.DlLiteLoaderListOfficialLoader, 5),
                new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult>, int>(ModDownload.DlLiteLoaderListBmclapiLoader, 35)
            }, Loader.IsForceRestarting);
        }

        private static void DlLiteLoaderListOfficialMain(ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult> Loader)
        {
            JObject jobject = (JObject)ModNet.NetRequestByClientRetry("https://dl.liteloader.com/versions/versions.json", null, null, null, null, "*/*", null, true, null, false).DeserializeJson();
            try
            {
                JObject jobject2 = (JObject)jobject["versions"];
                List<ModDownload.DlLiteLoaderListEntry> list = new List<ModDownload.DlLiteLoaderListEntry>();
                try
                {
                    foreach (KeyValuePair<string, JToken> keyValuePair in jobject2)
                    {
                        if (!keyValuePair.Key.StartsWithF("1.6", false) && !keyValuePair.Key.StartsWithF("1.5", false))
                        {
                            JToken jtoken = (keyValuePair.Value["artefacts"] ?? keyValuePair.Value["snapshots"])["com.mumfrey:liteloader"]["latest"];
                            list.Add(new ModDownload.DlLiteLoaderListEntry
                            {
                                Inherit = keyValuePair.Key,
                                IsLegacy = (Conversions.ToDouble(keyValuePair.Key.Split(".", false)[1]) < 8.0),
                                IsPreview = (Operators.CompareString(jtoken["stream"].ToString().Lower(), "snapshot", false) == 0),
                                FileName = "liteloader-installer-" + keyValuePair.Key + ((Operators.CompareString(keyValuePair.Key, "1.8", false) == 0 || Operators.CompareString(keyValuePair.Key, "1.9", false) == 0) ? ".0" : "") + "-00-SNAPSHOT.jar",
                                MD5 = (string)jtoken["md5"],
                                ReleaseTime = ModBase.GetLocalTime(ModBase.GetDate((int)jtoken["timestamp"])).ToString("yyyy'/'MM'/'dd HH':'mm"),
                                JsonToken = jtoken
                            });
                        }
                    }
                }
                finally
                {
                    IEnumerator<KeyValuePair<string, JToken>> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                Loader.Output = new ModDownload.DlLiteLoaderListResult
                {
                    IsOfficial = true,
                    SourceName = "LiteLoader 官方源",
                    Value = list
                };
            }
            catch (Exception ex)
            {
                throw new Exception("LiteLoader 官方源版本列表解析失败（" + jobject.ToString() + "）", ex);
            }
        }

        private static void DlLiteLoaderListBmclapiMain(ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult> Loader)
        {
            JObject jobject = (JObject)ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/maven/com/mumfrey/liteloader/versions.json", null, null, null, null, "*/*", null, true, null, false).DeserializeJson();
            try
            {
                JObject jobject2 = (JObject)jobject["versions"];
                List<ModDownload.DlLiteLoaderListEntry> list = new List<ModDownload.DlLiteLoaderListEntry>();
                try
                {
                    foreach (KeyValuePair<string, JToken> keyValuePair in jobject2)
                    {
                        if (!keyValuePair.Key.StartsWithF("1.6", false) && !keyValuePair.Key.StartsWithF("1.5", false))
                        {
                            JToken jtoken = (keyValuePair.Value["artefacts"] ?? keyValuePair.Value["snapshots"])["com.mumfrey:liteloader"]["latest"];
                            list.Add(new ModDownload.DlLiteLoaderListEntry
                            {
                                Inherit = keyValuePair.Key,
                                IsLegacy = (Conversions.ToDouble(keyValuePair.Key.Split(".", false)[1]) < 8.0),
                                IsPreview = (Operators.CompareString(jtoken["stream"].ToString().Lower(), "snapshot", false) == 0),
                                FileName = "liteloader-installer-" + keyValuePair.Key + ((Operators.CompareString(keyValuePair.Key, "1.8", false) == 0 || Operators.CompareString(keyValuePair.Key, "1.9", false) == 0) ? ".0" : "") + "-00-SNAPSHOT.jar",
                                MD5 = (string)jtoken["md5"],
                                ReleaseTime = ModBase.GetLocalTime(ModBase.GetDate((int)jtoken["timestamp"])).ToString("yyyy'/'MM'/'dd HH':'mm"),
                                JsonToken = jtoken
                            });
                        }
                    }
                }
                finally
                {
                    IEnumerator<KeyValuePair<string, JToken>> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                Loader.Output = new ModDownload.DlLiteLoaderListResult
                {
                    IsOfficial = false,
                    SourceName = "BMCLAPI",
                    Value = list
                };
            }
            catch (Exception ex)
            {
                throw new Exception("LiteLoader BMCLAPI 版本列表解析失败（" + jobject.ToString() + "）", ex);
            }
        }

        private static void DlFabricListMain(ModLoader.LoaderTask<int, ModDownload.DlFabricListResult> Loader)
        {
            int num = Settings.Get<int>("ToolDownloadVersion", null);
            if (num == 0)
            {
                ModDownload.DlSourceLoader<int, ModDownload.DlFabricListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlFabricListResult>, int>>
                {
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlFabricListResult>, int>(ModDownload.DlFabricListBmclapiLoader, 30),
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlFabricListResult>, int>(ModDownload.DlFabricListOfficialLoader, 90)
                }, Loader.IsForceRestarting);
                return;
            }
            if (num != 1)
            {
                ModDownload.DlSourceLoader<int, ModDownload.DlFabricListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlFabricListResult>, int>>
                {
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlFabricListResult>, int>(ModDownload.DlFabricListOfficialLoader, 60),
                    new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlFabricListResult>, int>(ModDownload.DlFabricListBmclapiLoader, 120)
                }, Loader.IsForceRestarting);
                return;
            }
            ModDownload.DlSourceLoader<int, ModDownload.DlFabricListResult>(Loader, new List<KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlFabricListResult>, int>>
            {
                new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlFabricListResult>, int>(ModDownload.DlFabricListOfficialLoader, 5),
                new KeyValuePair<ModLoader.LoaderTask<int, ModDownload.DlFabricListResult>, int>(ModDownload.DlFabricListBmclapiLoader, 35)
            }, Loader.IsForceRestarting);
        }

        private static void DlFabricListOfficialMain(ModLoader.LoaderTask<int, ModDownload.DlFabricListResult> Loader)
        {
            JObject jobject = (JObject)ModNet.NetRequestByClientRetry("https://meta.fabricmc.net/v2/versions", null, null, null, null, "*/*", null, true, null, false).DeserializeJson();
            try
            {
                ModDownload.DlFabricListResult dlFabricListResult = new ModDownload.DlFabricListResult
                {
                    IsOfficial = true,
                    SourceName = "Fabric 官方源",
                    Value = jobject
                };
                if (dlFabricListResult.Value["game"] == null || dlFabricListResult.Value["loader"] == null || dlFabricListResult.Value["installer"] == null)
                {
                    throw new Exception("获取到的列表缺乏必要项");
                }
                Loader.Output = dlFabricListResult;
            }
            catch (Exception ex)
            {
                throw new Exception("Fabric 官方源版本列表解析失败（" + jobject.ToString() + "）", ex);
            }
        }

        private static void DlFabricListBmclapiMain(ModLoader.LoaderTask<int, ModDownload.DlFabricListResult> Loader)
        {
            JObject jobject = (JObject)ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/fabric-meta/v2/versions", null, null, null, null, "*/*", null, true, null, false).DeserializeJson();
            try
            {
                ModDownload.DlFabricListResult dlFabricListResult = new ModDownload.DlFabricListResult
                {
                    IsOfficial = false,
                    SourceName = "BMCLAPI",
                    Value = jobject
                };
                if (dlFabricListResult.Value["game"] == null || dlFabricListResult.Value["loader"] == null || dlFabricListResult.Value["installer"] == null)
                {
                    throw new Exception("获取到的列表缺乏必要项");
                }
                Loader.Output = dlFabricListResult;
            }
            catch (Exception ex)
            {
                throw new Exception("Fabric BMCLAPI 版本列表解析失败（" + jobject.ToString() + "）", ex);
            }
        }

        public static JToken DlModRequest(string Url, HttpMethod Method = null, string Content = null, string ContentType = null)
        {
            List<ValueTuple<string, int>> list = new List<ValueTuple<string, int>>();
            string text = ModDownload.DlSourceModGet(Url);
            if (Operators.CompareString(text, Url, false) != 0)
            {
                int num = Settings.Get<int>("ToolDownloadMod", null);
                if (num != 0)
                {
                    if (num != 1)
                    {
                        list.Add(new ValueTuple<string, int>(Url, Url.Contains("modrinth") ? 20 : 10));
                        list.Add(new ValueTuple<string, int>(Url, 30));
                        list.Add(new ValueTuple<string, int>(text, 30));
                    }
                    else
                    {
                        list.Add(new ValueTuple<string, int>(Url, Url.Contains("modrinth") ? 20 : 10));
                        list.Add(new ValueTuple<string, int>(text, 10));
                        list.Add(new ValueTuple<string, int>(Url, 30));
                        list.Add(new ValueTuple<string, int>(text, 30));
                    }
                }
                else
                {
                    list.Add(new ValueTuple<string, int>(text, 10));
                    list.Add(new ValueTuple<string, int>(text, 10));
                    list.Add(new ValueTuple<string, int>(Url, 30));
                }
            }
            else
            {
                list.Add(new ValueTuple<string, int>(Url, Url.Contains("modrinth") ? 20 : 10));
                list.Add(new ValueTuple<string, int>(Url, 30));
            }
            List<Exception> list2 = new List<Exception>();
            try
            {
                foreach (ValueTuple<string, int> valueTuple in list)
                {
                    try
                    {
                        return ModNet.NetRequestByClient(valueTuple.Item1, Method, Content, ContentType, "*/*", checked(valueTuple.Item2 * 1000), null, true, Encoding.UTF8, false, true).DeserializeJson();
                    }
                    catch (Exception ex)
                    {
                        list2.Add(ex);
                    }
                }
            }
            finally
            {
                List<ValueTuple<string, int>>.Enumerator enumerator = default;
                ((IDisposable)enumerator).Dispose();
            }
            throw new AggregateException(list2);
        }

        public static bool DlSourcePreferMojang
        {
            get
            {
                return Settings.Get<int>("ToolDownloadSource", null) == 2 || (Settings.Get<int>("ToolDownloadSource", null) == 1 && ModDownload.DlPreferMojang);
            }
        }

        public static IEnumerable<string> DlSourceOrder(IEnumerable<string> OfficialUrls, IEnumerable<string> MirrorUrls)
        {
            if (!ModDownload.DlSourcePreferMojang)
            {
                return MirrorUrls.Union<string>(OfficialUrls);
            }
            return OfficialUrls.Union<string>(MirrorUrls);
        }

        public static bool DlVersionListPreferMojang
        {
            get
            {
                return Settings.Get<int>("ToolDownloadVersion", null) == 2 || (Settings.Get<int>("ToolDownloadVersion", null) == 1 && ModDownload.DlPreferMojang);
            }
        }

        public static IEnumerable<string> DlVersionListOrder(IEnumerable<string> OfficialUrls, IEnumerable<string> MirrorUrls)
        {
            if (!ModDownload.DlVersionListPreferMojang)
            {
                return MirrorUrls.Union<string>(OfficialUrls);
            }
            return OfficialUrls.Union<string>(MirrorUrls);
        }

        public static IEnumerable<string> DlSourceAssetsGet(string Original)
        {
            Original = Original.Replace("http://resources.download.minecraft.net", "https://resources.download.minecraft.net");
            return ModDownload.DlSourceOrder(new string[] { Original }, new string[] { Original.Replace("https://piston-data.mojang.com", "https://bmclapi2.bangbang93.com/assets").Replace("https://piston-meta.mojang.com", "https://bmclapi2.bangbang93.com/assets").Replace("https://resources.download.minecraft.net", "https://bmclapi2.bangbang93.com/assets") });
        }

        public static IEnumerable<string> DlSourceLibraryGet(string Original)
        {
            ModDownload._Closure___71_0 CS_locals1 = new ModDownload._Closure___71_0();
            CS_locals1._VB_Local_Original = Original;
            IEnumerable<string> enumerable;
            if (new string[] { "minecraftforge", "fabricmc", "neoforged" }.Any<string>(new Func<string, bool>(CS_locals1._Lambda___0)))
            {
                enumerable = new string[]
                {
                    CS_locals1._VB_Local_Original.Replace("https://piston-data.mojang.com", "https://bmclapi2.bangbang93.com/maven").Replace("https://piston-meta.mojang.com", "https://bmclapi2.bangbang93.com/maven").Replace("https://libraries.minecraft.net", "https://bmclapi2.bangbang93.com/maven"),
                    CS_locals1._VB_Local_Original.Replace("https://piston-data.mojang.com", "https://bmclapi2.bangbang93.com/libraries").Replace("https://piston-meta.mojang.com", "https://bmclapi2.bangbang93.com/libraries").Replace("https://libraries.minecraft.net", "https://bmclapi2.bangbang93.com/libraries")
                };
            }
            else
            {
                enumerable = ModDownload.DlSourceOrder(new string[] { CS_locals1._VB_Local_Original }, new string[]
                {
                    CS_locals1._VB_Local_Original.Replace("https://piston-data.mojang.com", "https://bmclapi2.bangbang93.com/maven").Replace("https://piston-meta.mojang.com", "https://bmclapi2.bangbang93.com/maven").Replace("https://libraries.minecraft.net", "https://bmclapi2.bangbang93.com/maven"),
                    CS_locals1._VB_Local_Original.Replace("https://piston-data.mojang.com", "https://bmclapi2.bangbang93.com/libraries").Replace("https://piston-meta.mojang.com", "https://bmclapi2.bangbang93.com/libraries").Replace("https://libraries.minecraft.net", "https://bmclapi2.bangbang93.com/libraries")
                });
            }
            return enumerable;
        }

        public static IEnumerable<string> DlSourceLauncherOrMetaGet(string Original)
        {
            if (Original == null)
            {
                throw new Exception("无对应的 json 下载地址");
            }
            return ModDownload.DlSourceOrder(new string[] { Original }, new string[] { Original.Replace("https://piston-data.mojang.com", "https://bmclapi2.bangbang93.com").Replace("https://piston-meta.mojang.com", "https://bmclapi2.bangbang93.com").Replace("https://launcher.mojang.com", "https://bmclapi2.bangbang93.com")
                .Replace("https://launchermeta.mojang.com", "https://bmclapi2.bangbang93.com") });
        }

        public static string DlSourceModGet(string Original)
        {
            return Original.Replace("api.modrinth.com", "mod.mcimirror.top/modrinth").Replace("staging-api.modrinth.com", "mod.mcimirror.top/modrinth").Replace("cdn.modrinth.com", "mod.mcimirror.top")
                .Replace("api.curseforge.com", "mod.mcimirror.top/curseforge")
                .Replace("edge.forgecdn.net", "mod.mcimirror.top")
                .Replace("mediafilez.forgecdn.net", "mod.mcimirror.top")
                .Replace("media.forgecdn.net", "mod.mcimirror.top");
        }

        private static void DlSourceLoader<InputType, OutputType>(ModLoader.LoaderTask<InputType, OutputType> MainLoader, List<KeyValuePair<ModLoader.LoaderTask<InputType, OutputType>, int>> LoaderList, bool IsForceRestart = false)
        {
            int num = 0;
            checked
            {
                for (;;)
                {
                    bool flag = true;
                    try
                    {
                        foreach (KeyValuePair<ModLoader.LoaderTask<InputType, OutputType>, int> keyValuePair in LoaderList)
                        {
                            if (num == 0)
                            {
                                if (IsForceRestart || ((keyValuePair.Key.Input == null) ^ (MainLoader.Input == null)))
                                {
                                    continue;
                                }
                                if (keyValuePair.Key.Input != null)
                                {
                                    ModLoader.LoaderTask<InputType, OutputType> key = keyValuePair.Key;
                                    if (!object.Equals(key.Input, MainLoader.Input))
                                    {
                                        continue;
                                    }
                                }
                            }
                            if (keyValuePair.Key.State != ModBase.LoadState.Failed)
                            {
                                flag = false;
                            }
                            if (keyValuePair.Key.State == ModBase.LoadState.Finished)
                            {
                                MainLoader.Output = keyValuePair.Key.Output;
                                ModDownload.DlSourceLoaderCancel<InputType, OutputType>(LoaderList);
                                return;
                            }
                            if (flag && num < keyValuePair.Value * 100)
                            {
                                num = keyValuePair.Value * 100;
                            }
                        }
                    }
                    finally
                    {
                        List<KeyValuePair<ModLoader.LoaderTask<InputType, OutputType>, int>>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                    if (num == 0)
                    {
                        LoaderList.First<KeyValuePair<ModLoader.LoaderTask<InputType, OutputType>, int>>().Key.Start(MainLoader.Input, IsForceRestart);
                        try
                        {
                            foreach (KeyValuePair<ModLoader.LoaderTask<InputType, OutputType>, int> keyValuePair2 in LoaderList.Skip<KeyValuePair<ModLoader.LoaderTask<InputType, OutputType>, int>>(1))
                            {
                                keyValuePair2.Key.State = ModBase.LoadState.Waiting;
                            }
                        }
                        finally
                        {
                            IEnumerator<KeyValuePair<ModLoader.LoaderTask<InputType, OutputType>, int>> enumerator2 = default;
                            if (enumerator2 != null)
                            {
                                enumerator2.Dispose();
                            }
                        }
                    }
                    int num2 = LoaderList.Count - 1;
                    int i = 0;
                    while (i <= num2)
                    {
                        if (num == LoaderList[i].Value * 100)
                        {
                            if (i < LoaderList.Count - 1 && !LoaderList.All<KeyValuePair<ModLoader.LoaderTask<InputType, OutputType>, int>>((ModDownload._Closure___74<InputType, OutputType>._I74_0 == null) ? (ModDownload._Closure___74<InputType, OutputType>._I74_0 = new Func<KeyValuePair<ModLoader.LoaderTask<InputType, OutputType>, int>, bool>(ModDownload._Closure___74<InputType, OutputType>._I._Lambda___74_0)) : ModDownload._Closure___74<InputType, OutputType>._I74_0))
                            {
                                LoaderList[i + 1].Key.Start(MainLoader.Input, IsForceRestart);
                                break;
                            }
                            goto IL_01FB;
                        }
                        else
                        {
                            i++;
                        }
                    }
                    Thread.Sleep(10);
                    num++;
                    if (MainLoader.IsCanceled)
                    {
                        goto Block_12;
                    }
                }
            }
            IL_01FB:
            Exception ex = null;
            checked
            {
                int num3 = LoaderList.Count - 1;
                for (int j = 0; j <= num3; j++)
                {
                    LoaderList[j].Key.Input = default(InputType);
                    if (LoaderList[j].Key.Error != null && (ex == null || Operators.CompareString(LoaderList[j].Key.Error.Message, "无", false) == 0))
                    {
                        ex = LoaderList[j].Key.Error;
                    }
                }
                if (ex == null)
                {
                    ex = new TimeoutException("下载源连接超时");
                }
                ModDownload.DlSourceLoaderCancel<InputType, OutputType>(LoaderList);
                throw ex;
            }
            Block_12:
            ModDownload.DlSourceLoaderCancel<InputType, OutputType>(LoaderList);
        }

        private static void DlSourceLoaderCancel<InputType, OutputType>(List<KeyValuePair<ModLoader.LoaderTask<InputType, OutputType>, int>> LoaderList)
        {
            try
            {
                foreach (KeyValuePair<ModLoader.LoaderTask<InputType, OutputType>, int> keyValuePair in LoaderList)
                {
                    if (keyValuePair.Key.State == ModBase.LoadState.Loading)
                    {
                        keyValuePair.Key.Cancel();
                    }
                }
            }
            finally
            {
                List<KeyValuePair<ModLoader.LoaderTask<InputType, OutputType>, int>>.Enumerator enumerator = default;
                ((IDisposable)enumerator).Dispose();
            }
        }

        private static List<int> _AllDrops = null;

        public static ModLoader.LoaderTask<string, ModDownload.DlClientListResult> DlClientListLoader = new ModLoader.LoaderTask<string, ModDownload.DlClientListResult>("DlClientList Main", new Action<ModLoader.LoaderTask<string, ModDownload.DlClientListResult>>(ModDownload.DlClientListMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<string, ModDownload.DlClientListResult> DlClientListMojangLoader = new ModLoader.LoaderTask<string, ModDownload.DlClientListResult>("DlClientList Mojang", new Action<ModLoader.LoaderTask<string, ModDownload.DlClientListResult>>(ModDownload.DlClientListMojangMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<string, ModDownload.DlClientListResult> DlClientListBmclapiLoader = new ModLoader.LoaderTask<string, ModDownload.DlClientListResult>("DlClientList Bmclapi", new Action<ModLoader.LoaderTask<string, ModDownload.DlClientListResult>>(ModDownload.DlClientListBmclapiMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult> DlOptiFineListLoader = new ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult>("DlOptiFineList Main", new Action<ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult>>(ModDownload.DlOptiFineListMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult> DlOptiFineListOfficialLoader = new ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult>("DlOptiFineList Official", new Action<ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult>>(ModDownload.DlOptiFineListOfficialMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult> DlOptiFineListBmclapiLoader = new ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult>("DlOptiFineList Bmclapi", new Action<ModLoader.LoaderTask<int, ModDownload.DlOptiFineListResult>>(ModDownload.DlOptiFineListBmclapiMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<int, ModDownload.DlForgeListResult> DlForgeListLoader = new ModLoader.LoaderTask<int, ModDownload.DlForgeListResult>("DlForgeList Main", new Action<ModLoader.LoaderTask<int, ModDownload.DlForgeListResult>>(ModDownload.DlForgeListMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<int, ModDownload.DlForgeListResult> DlForgeListOfficialLoader = new ModLoader.LoaderTask<int, ModDownload.DlForgeListResult>("DlForgeList Official", new Action<ModLoader.LoaderTask<int, ModDownload.DlForgeListResult>>(ModDownload.DlForgeListOfficialMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<int, ModDownload.DlForgeListResult> DlForgeListBmclapiLoader = new ModLoader.LoaderTask<int, ModDownload.DlForgeListResult>("DlForgeList Bmclapi", new Action<ModLoader.LoaderTask<int, ModDownload.DlForgeListResult>>(ModDownload.DlForgeListBmclapiMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult> DlNeoForgeListLoader = new ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult>("DlNeoForgeList Main", new Action<ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult>>(ModDownload.DlNeoForgeListMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult> DlNeoForgeListOfficialLoader = new ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult>("DlNeoForgeList Official", new Action<ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult>>(ModDownload.DlNeoForgeListOfficialMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult> DlNeoForgeListBmclapiLoader = new ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult>("DlNeoForgeList Bmclapi", new Action<ModLoader.LoaderTask<int, ModDownload.DlNeoForgeListResult>>(ModDownload.DlNeoForgeListBmclapiMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult> DlLiteLoaderListLoader = new ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult>("DlLiteLoaderList Main", new Action<ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult>>(ModDownload.DlLiteLoaderListMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult> DlLiteLoaderListOfficialLoader = new ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult>("DlLiteLoaderList Official", new Action<ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult>>(ModDownload.DlLiteLoaderListOfficialMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult> DlLiteLoaderListBmclapiLoader = new ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult>("DlLiteLoaderList Bmclapi", new Action<ModLoader.LoaderTask<int, ModDownload.DlLiteLoaderListResult>>(ModDownload.DlLiteLoaderListBmclapiMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<int, ModDownload.DlFabricListResult> DlFabricListLoader = new ModLoader.LoaderTask<int, ModDownload.DlFabricListResult>("DlFabricList Main", new Action<ModLoader.LoaderTask<int, ModDownload.DlFabricListResult>>(ModDownload.DlFabricListMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<int, ModDownload.DlFabricListResult> DlFabricListOfficialLoader = new ModLoader.LoaderTask<int, ModDownload.DlFabricListResult>("DlFabricList Official", new Action<ModLoader.LoaderTask<int, ModDownload.DlFabricListResult>>(ModDownload.DlFabricListOfficialMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<int, ModDownload.DlFabricListResult> DlFabricListBmclapiLoader = new ModLoader.LoaderTask<int, ModDownload.DlFabricListResult>("DlFabricList Bmclapi", new Action<ModLoader.LoaderTask<int, ModDownload.DlFabricListResult>>(ModDownload.DlFabricListBmclapiMain), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<int, List<ResourceVersion>> DlFabricApiLoader = new ModLoader.LoaderTask<int, List<ResourceVersion>>("Fabric API List Loader", new Action<ModLoader.LoaderTask<int, List<ResourceVersion>>>(ModDownload._Closure___._I._Lambda___0_0), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<int, List<ResourceVersion>> DlOptiFabricLoader = new ModLoader.LoaderTask<int, List<ResourceVersion>>("OptiFabric List Loader", new Action<ModLoader.LoaderTask<int, List<ResourceVersion>>>(ModDownload._Closure___._I._Lambda___0_1), null, ThreadPriority.Normal);

        private static bool DlPreferMojang = false;

        private static bool _STATIC_DlClientListMojangMain_011151286FC2E118758_IsHinted;

        private static StaticLocalInitFlag _STATIC_DlClientListMojangMain_011151286FC2E118758_IsHinted_Init;

        public struct DlClientListResult
        {
            public string SourceName;

            public bool IsOfficial;

            public JObject Value;
        }

        public struct DlOptiFineListResult
        {
            public string SourceName;

            public bool IsOfficial;

            public List<ModDownload.DlOptiFineListEntry> Value;
        }

        public class DlOptiFineListEntry
        {
            public DlOptiFineListEntry()
            {
            }

            public string Inherit
            {
                get
                {
                    return this._inherit;
                }
                set
                {
                    if (value.EndsWithF(".0", false))
                    {
                        value = Strings.Left(value, checked(value.Length - 2));
                    }
                    this._inherit = value;
                }
            }

            public string DisplayName;

            public string FileName;

            public string InstanceName;

            public bool IsPreview;

            private string _inherit;

            public string ReleaseTime;

            public string RequiredForgeVersion;
        }

        public struct DlForgeListResult
        {
            public string SourceName;

            public bool IsOfficial;

            public List<string> Value;
        }

        public abstract class DlForgelikeEntry : IComparable<ModDownload.DlForgelikeEntry>
        {
            protected DlForgelikeEntry()
            {
            }

            public abstract string LoaderName { get; }

            public abstract string FileExtension { get; }

            public int CompareTo(ModDownload.DlForgelikeEntry other)
            {
                int num;
                if (this.Version != other.Version)
                {
                    num = this.Version.CompareTo(other.Version);
                }
                else
                {
                    num = McUtils.CompareVersion(this.VersionName, other.VersionName);
                }
                return num;
            }

            public bool IsNeoForge;

            public Version Version;

            public string VersionName;

            public string Inherit;
        }

        public class DlForgeVersionEntry : ModDownload.DlForgelikeEntry
        {
            public override string LoaderName
            {
                get
                {
                    return "Forge";
                }
            }

            public override string FileExtension
            {
                get
                {
                    if (Operators.CompareString(this.Category, "installer", false) != 0)
                    {
                        return "zip";
                    }
                    return "jar";
                }
            }

            public DlForgeVersionEntry(string Version, string Branch, string Inherit)
            {
                this.Hash = null;
                if (Operators.CompareString(Version, "11.15.1.2318", false) == 0 || Operators.CompareString(Version, "11.15.1.1902", false) == 0 || Operators.CompareString(Version, "11.15.1.1890", false) == 0)
                {
                    Branch = "1.8.9";
                }
                if (Branch == null && Operators.CompareString(Inherit, "1.7.10", false) == 0 && Conversions.ToDouble(Version.Split(".", false)[3]) >= 1300.0)
                {
                    Branch = "1.7.10";
                }
                this.IsNeoForge = false;
                this.VersionName = Version;
                this.Version = new Version(Version);
                this.Inherit = Inherit;
                this.FileVersion = Version + ((Branch == null) ? "" : ("-" + Branch));
            }

            public string ReleaseTime;

            public string Hash;

            public bool IsRecommended;

            public string Category;

            public string FileVersion;
        }

        public struct DlNeoForgeListResult
        {
            public string SourceName;

            public bool IsOfficial;

            public List<ModDownload.DlNeoForgeListEntry> Value;
        }

        public class DlNeoForgeListEntry : ModDownload.DlForgelikeEntry
        {
            public string UrlBase
            {
                get
                {
                    string text = ((Operators.CompareString(this.Inherit, "1.20.1", false) == 0) ? "forge" : "neoforge");
                    return string.Format("https://maven.neoforged.net/releases/net/neoforged/{0}/{1}/{2}-{3}", new object[] { text, this.ApiName, text, this.ApiName });
                }
            }

            public override string LoaderName
            {
                get
                {
                    return "NeoForge";
                }
            }

            public override string FileExtension
            {
                get
                {
                    return "jar";
                }
            }

            public DlNeoForgeListEntry(string ApiName)
            {
                this.IsNeoForge = true;
                this.ApiName = ApiName;
                this.IsBeta = ApiName.ContainsIgnoreCase("beta") || ApiName.ContainsIgnoreCase("alpha");
                if (ApiName.Contains("1.20.1"))
                {
                    this.VersionName = ApiName.Replace("1.20.1-", "");
                    this.Version = new Version("19." + this.VersionName);
                    this.Inherit = "1.20.1";
                    return;
                }
                if (ApiName.StartsWithF("0.", false))
                {
                    this.VersionName = ApiName;
                    string[] array = ApiName.BeforeFirst("-", false).Split(new char[] { '.' });
                    this.Version = new Version(0, 0, Conversions.ToInteger(array.Last<string>()));
                    this.Inherit = array[1];
                    return;
                }
                this.VersionName = ApiName;
                this.Version = new Version(ApiName.BeforeFirst("-", false));
                if (this.Version.Major >= 24)
                {
                    this.Inherit = string.Format("{0}.{1}.{2}", this.Version.Major, this.Version.Minor, this.Version.Build);
                }
                else
                {
                    this.Inherit = string.Format("1.{0}.{1}", this.Version.Major, this.Version.Minor);
                }
                if (this.Inherit.EndsWithF(".0", false))
                {
                    this.Inherit = Strings.Left(this.Inherit, checked(this.Inherit.Length - 2));
                }
                if (this.VersionName.Contains("+"))
                {
                    ref string ptr = ref this.Inherit;
                    this.Inherit = ptr + "-" + this.VersionName.AfterFirst("+", false);
                }
            }

            public bool IsBeta;

            public string ApiName;
        }

        public struct DlLiteLoaderListResult
        {
            public string SourceName;

            public bool IsOfficial;

            public List<ModDownload.DlLiteLoaderListEntry> Value;

            public Exception OfficialError;
        }

        public class DlLiteLoaderListEntry
        {
            public DlLiteLoaderListEntry()
            {
            }

            public string FileName;

            public bool IsPreview;

            public string Inherit;

            public bool IsLegacy;

            public string ReleaseTime;

            public string MD5;

            public JToken JsonToken;
        }

        public struct DlFabricListResult
        {
            public string SourceName;

            public bool IsOfficial;

            public JObject Value;
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

            internal void _Lambda___0_0(ModLoader.LoaderTask<int, List<ResourceVersion>> Task)
            {
                Task.Output = ResourceVersion.FromProjectId("fabric-api", ResourcePlatforms.Modrinth, false);
            }

            internal void _Lambda___0_1(ModLoader.LoaderTask<int, List<ResourceVersion>> Task)
            {
                Task.Output = ResourceVersion.FromProjectId("322385", ResourcePlatforms.CurseForge, false);
            }

            internal int _Lambda___5_0(string d)
            {
                return checked((int)Math.Round(ModBase.Val(d)));
            }

            internal int _Lambda___10_0(int d)
            {
                return d;
            }

            internal string _Lambda___30_0(JToken v)
            {
                return v.ToString();
            }

            internal bool _Lambda___44_0(string name)
            {
                return Operators.CompareString(name, "47.1.82", false) != 0;
            }

            internal ModDownload.DlNeoForgeListEntry _Lambda___44_1(string name)
            {
                return new ModDownload.DlNeoForgeListEntry(name);
            }

            internal ModDownload.DlNeoForgeListEntry _Lambda___44_2(ModDownload.DlNeoForgeListEntry a)
            {
                return a;
            }

            public static readonly ModDownload._Closure___ _I = new ModDownload._Closure___();

            public static Func<string, int> _I5_0;

            public static Func<int, int> _I10_0;

            public static Func<JToken, string> _I30_0;

            public static Func<string, bool> _I44_0;

            public static Func<string, ModDownload.DlNeoForgeListEntry> _I44_1;

            public static Func<ModDownload.DlNeoForgeListEntry, ModDownload.DlNeoForgeListEntry> _I44_2;
        }

        [CompilerGenerated]
        private sealed class _Closure___14_0
        {
            public _Closure___14_0()
            {
            }

            internal bool _Lambda___0(JToken v)
            {
                return (string)v["id"] == this._VB_Local_Id;
            }

            public string _VB_Local_Id;
        }

        [CompilerGenerated]
        private sealed class _Closure___3_0
        {
            public _Closure___3_0()
            {
            }

            internal void _Lambda___0(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                Task.Output = ModMinecraft.McLibNetFilesFromInstance(this._VB_Local_Instance);
            }

            public ModMinecraft.McInstance _VB_Local_Instance;

            public bool _VB_Local_DownloadAssetIndexInBackground;

            public bool _VB_Local_CheckAssetsHash;
        }

        [CompilerGenerated]
        private sealed class _Closure___3_1
        {
            public _Closure___3_1()
            {
            }

            internal void _Lambda___1(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                try
                {
                    ModNet.NetFile netFile = ModDownload.DlClientAssetIndexGet(this._VB_NonLocal__VB_Closure_2._VB_Local_Instance);
                    if (netFile == null)
                    {
                        Task.Output = new List<ModNet.NetFile>();
                        Logger.Warn(string.Format("未找到版本 {0} 的合适的资源索引下载地址，游戏 assets 可能缺失", this._VB_NonLocal__VB_Closure_2._VB_Local_Instance.Name), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModDownload.vb");
                    }
                    else
                    {
                        string text = ModBase.PathTemp + "Cache\\" + netFile.LocalName + "_background";
                        if (FileUtils.Exists(text) && DateTime.Compare(FileUtils.GetInfo(text).LastWriteTime.Date, FileUtils.GetInfo(netFile.LocalPath).LastWriteTime.Date) > 0)
                        {
                            Logger.Info(string.Format("发现后台更新的资源索引缓存文件：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModDownload.vb");
                            FileUtils.Move(text, netFile.LocalPath, false);
                        }
                        if (this._VB_NonLocal__VB_Closure_2._VB_Local_DownloadAssetIndexInBackground)
                        {
                            if (netFile.Check.Check(netFile.LocalPath) == null)
                            {
                                Task.Output = new List<ModNet.NetFile>();
                            }
                            else
                            {
                                Task.Output = new List<ModNet.NetFile> { netFile };
                            }
                        }
                        else
                        {
                            this._VB_Local_RealAddressMain = netFile.LocalPath;
                            this._VB_Local_TempAddressMain = ModBase.PathTemp + "Cache\\" + netFile.LocalName;
                            netFile.LocalPath = this._VB_Local_TempAddressMain;
                            Task.Output = new List<ModNet.NetFile> { netFile };
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("分析资源文件索引地址失败", ex);
                }
            }

            internal void _Lambda___2(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                if (this._VB_Local_TempAddressMain != null)
                {
                    FileUtils.Move(this._VB_Local_TempAddressMain, this._VB_Local_RealAddressMain, false);
                }
                ModLoader.LoaderTask<string, List<ModNet.NetFile>> loaderTask = Task;
                ModMinecraft.McInstance _VB_Local_Instance = this._VB_NonLocal__VB_Closure_2._VB_Local_Instance;
                bool _VB_Local_CheckAssetsHash = this._VB_NonLocal__VB_Closure_2._VB_Local_CheckAssetsHash;
                ModLoader.LoaderBase loaderBase = Task;
                List<ModNet.NetFile> list = ModMinecraft.McAssetsFixList(_VB_Local_Instance, _VB_Local_CheckAssetsHash, ref loaderBase);
                Task = (ModLoader.LoaderTask<string, List<ModNet.NetFile>>)loaderBase;
                loaderTask.Output = list;
            }

            public string _VB_Local_RealAddressMain;

            public string _VB_Local_TempAddressMain;

            public ModDownload._Closure___3_0 _VB_NonLocal__VB_Closure_2;
        }

        [CompilerGenerated]
        private sealed class _Closure___3_2
        {
            public _Closure___3_2()
            {
            }

            internal void _Lambda___3(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                ModNet.NetFile netFile = ModDownload.DlClientAssetIndexGet(this._VB_NonLocal__VB_Closure_3._VB_NonLocal__VB_Closure_2._VB_Local_Instance);
                if (netFile == null)
                {
                    Logger.Warn(string.Format("未找到版本 {0} 的合适的资源索引下载地址，游戏 assets 可能缺失", this._VB_NonLocal__VB_Closure_3._VB_NonLocal__VB_Closure_2._VB_Local_Instance.Name), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModDownload.vb");
                    Task.Cancel();
                    throw new OperationCanceledException();
                }
                this._VB_Local_RealAddressBg = netFile.LocalPath;
                this._VB_Local_TempAddressBg = ModBase.PathTemp + "Cache\\" + netFile.LocalName + "_background";
                netFile.LocalPath = this._VB_Local_TempAddressBg;
                Task.Output = new List<ModNet.NetFile> { netFile };
                DateTime date = FileUtils.GetInfo(this._VB_Local_RealAddressBg).LastWriteTime.Date;
                if (FileUtils.Exists(this._VB_Local_RealAddressBg) && Math.Abs((date - DateAndTime.Now.Date).TotalDays) < 1.0)
                {
                    Logger.Info(string.Format("无需更新资源文件索引，上次更新于 {0}", date), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModDownload.vb");
                    Task.Cancel();
                }
            }

            public string _VB_Local_RealAddressBg;

            public string _VB_Local_TempAddressBg;

            public ModDownload._Closure___3_1 _VB_NonLocal__VB_Closure_3;
        }

        [CompilerGenerated]
        private sealed class _Closure___71_0
        {
            public _Closure___71_0()
            {
            }

            internal bool _Lambda___0(string k)
            {
                return this._VB_Local_Original.Contains(k);
            }

            public string _VB_Local_Original;
        }

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___74<_CLS0, _CLS1>
        {
            public _Closure___74()
            {
            }

            static _Closure___74()
            {
            }

            internal bool _Lambda___74_0(KeyValuePair<ModLoader.LoaderTask<_CLS0, _CLS1>, int> l)
            {
                return l.Key.State == ModBase.LoadState.Failed;
            }

            public static readonly ModDownload._Closure___74<_CLS0, _CLS1> _I = new ModDownload._Closure___74<_CLS0, _CLS1>();

            public static Func<KeyValuePair<ModLoader.LoaderTask<_CLS0, _CLS1>, int>, bool> _I74_0;
        }
    }
}
