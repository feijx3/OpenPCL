using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using MeloongCore;
using MeloongCore.Extensions;
using MeloongCore.Wpf;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;
using PCLCS;

namespace PCL
{
    [StandardModule]
    public static class ModDownloadLib
    {
        // Note: this type is marked as 'beforefieldinit'.
        static ModDownloadLib()
        {
        }

        private static List<ModLoader.LoaderBase> McDownloadClientLoader(string Id, string JsonUrl, string VersionFolder)
        {
            ModDownloadLib._Closure___1_0 CS_locals1 = new ModDownloadLib._Closure___1_0();
            CS_locals1._VB_Local_Id = Id;
            CS_locals1._VB_Local_VersionFolder = VersionFolder;
            CS_locals1._VB_Local_InstanceName = PathUtils.GetLastPart(CS_locals1._VB_Local_VersionFolder);
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            CS_locals1._VB_Local_NewInstance = null;
            if (JsonUrl == null)
            {
                list.Add(new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("获取原版 json 文件下载地址", new Action<ModLoader.LoaderTask<string, List<ModNet.NetFile>>>(CS_locals1._Lambda___0), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 2.0,
                    Show = false
                });
            }
            list.Add(new ModNet.LoaderDownload("下载原版 json 文件", new List<ModNet.NetFile>
            {
                new ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(JsonUrl ?? ""), CS_locals1._VB_Local_VersionFolder + CS_locals1._VB_Local_InstanceName + ".json", new FileChecker
                {
                    IsJson = true
                }, false)
            })
            {
                ProgressWeight = 3.0
            });
            list.Add(new ModLoader.LoaderTask<string, string>("读取 json 文件", new Action<ModLoader.LoaderTask<string, string>>(CS_locals1._Lambda___1), null, ThreadPriority.Normal)
            {
                ProgressWeight = 0.0,
                Show = false
            });
            list.Add(new ModLoader.LoaderCombo<string>("下载原版支持库文件", new List<ModLoader.LoaderBase>
            {
                new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("分析原版支持库文件（副加载器）", new Action<ModLoader.LoaderTask<string, List<ModNet.NetFile>>>(CS_locals1._Lambda___2), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 1.0,
                    Show = false
                },
                new ModNet.LoaderDownload("下载原版支持库文件（副加载器）", new List<ModNet.NetFile>())
                {
                    ProgressWeight = 13.0,
                    Show = false
                }
            })
            {
                Block = false,
                ProgressWeight = 14.0
            });
            list.Add(new ModLoader.LoaderCombo<string>("下载原版资源文件", new List<ModLoader.LoaderBase>
            {
                new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("分析资源文件索引地址（副加载器）", new Action<ModLoader.LoaderTask<string, List<ModNet.NetFile>>>(CS_locals1._Lambda___3), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 1.0,
                    Show = false
                },
                new ModNet.LoaderDownload("下载资源文件索引（副加载器）", new List<ModNet.NetFile>())
                {
                    ProgressWeight = 3.0,
                    Show = false
                },
                new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("分析所需资源文件（副加载器）", new Action<ModLoader.LoaderTask<string, List<ModNet.NetFile>>>(CS_locals1._Lambda___4), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 0.01,
                    Show = false
                },
                new ModNet.LoaderDownload("下载资源文件（副加载器）", new List<ModNet.NetFile>())
                {
                    ProgressWeight = 14.0,
                    Show = false
                }
            })
            {
                Block = false,
                ProgressWeight = 18.0
            });
            return list;
        }

        public static OurVirtualizingElement<OurListItem> McDownloadListItem(JObject Entry, OurListItem.ClickEventHandler OnClick, bool IsSaveOnly)
        {
            ModDownloadLib._Closure___4_0 CS_locals1 = new ModDownloadLib._Closure___4_0();
            CS_locals1._VB_Local_Entry = Entry;
            CS_locals1._VB_Local_OnClick = OnClick;
            CS_locals1._VB_Local_IsSaveOnly = IsSaveOnly;
            return new OurVirtualizingElement<OurListItem>(new Func<OurListItem>(CS_locals1._Lambda___0))
            {
                Height = 42.0
            };
        }

        private static void McDownloadSaveMenuBuild(object sender, EventArgs e)
        {
            OurIconButton ourIconButton = new OurIconButton
            {
                LogoScale = 1.05,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                ToolTip = "更新日志"
            };
            ourIconButton.Click += ((ModDownloadLib._Closure___._IR5_1 == null) ? (ModDownloadLib._Closure___._IR5_1 = new OurIconButton.ClickEventHandler(ModDownloadLib._Closure___._I._Lambda___R5_1)) : ModDownloadLib._Closure___._IR5_1);
            OurIconButton ourIconButton2 = new OurIconButton
            {
                LogoScale = 1.0,
                Logo = "M224 160a64 64 0 0 0-64 64v576a64 64 0 0 0 64 64h576a64 64 0 0 0 64-64V224a64 64 0 0 0-64-64H224z m0 384h576v256H224v-256z m192 96v64h320v-64H416z m-128 0v64h64v-64H288zM224 224h576v256H224V224z m192 96v64h320v-64H416z m-128 0v64h64v-64H288z",
                ToolTip = "下载服务端"
            };
            ourIconButton2.Click += ((ModDownloadLib._Closure___._IR5_2 == null) ? (ModDownloadLib._Closure___._IR5_2 = new OurIconButton.ClickEventHandler(ModDownloadLib._Closure___._I._Lambda___R5_2)) : ModDownloadLib._Closure___._IR5_2);
            NewLateBinding.LateSet(sender, null, "Buttons", new object[] { new OurIconButton[] { ourIconButton2, ourIconButton } }, null, null);
        }

        private static void McDownloadMenuBuild(object sender, EventArgs e)
        {
            OurIconButton ourIconButton = new OurIconButton
            {
                Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z",
                ToolTip = "另存为"
            };
            ourIconButton.Click += ((ModDownloadLib._Closure___._IR6_3 == null) ? (ModDownloadLib._Closure___._IR6_3 = new OurIconButton.ClickEventHandler(ModDownloadLib._Closure___._I._Lambda___R6_3)) : ModDownloadLib._Closure___._IR6_3);
            OurIconButton ourIconButton2 = new OurIconButton
            {
                LogoScale = 1.05,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                ToolTip = "更新日志"
            };
            ourIconButton2.Click += ((ModDownloadLib._Closure___._IR6_4 == null) ? (ModDownloadLib._Closure___._IR6_4 = new OurIconButton.ClickEventHandler(ModDownloadLib._Closure___._I._Lambda___R6_4)) : ModDownloadLib._Closure___._IR6_4);
            OurIconButton ourIconButton3 = new OurIconButton
            {
                LogoScale = 1.0,
                Logo = "M224 160a64 64 0 0 0-64 64v576a64 64 0 0 0 64 64h576a64 64 0 0 0 64-64V224a64 64 0 0 0-64-64H224z m0 384h576v256H224v-256z m192 96v64h320v-64H416z m-128 0v64h64v-64H288zM224 224h576v256H224V224z m192 96v64h320v-64H416z m-128 0v64h64v-64H288z",
                ToolTip = "下载服务端"
            };
            ourIconButton3.Click += ((ModDownloadLib._Closure___._IR6_5 == null) ? (ModDownloadLib._Closure___._IR6_5 = new OurIconButton.ClickEventHandler(ModDownloadLib._Closure___._I._Lambda___R6_5)) : ModDownloadLib._Closure___._IR6_5);
            NewLateBinding.LateSet(sender, null, "Buttons", new object[] { new OurIconButton[] { ourIconButton, ourIconButton2, ourIconButton3 } }, null, null);
        }

        private static void McDownloadMenuLog(object sender, RoutedEventArgs e)
        {
            JToken jtoken;
            if (NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null) != null)
            {
                jtoken = (JToken)NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null);
            }
            else if (NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null) != null)
            {
                jtoken = (JToken)NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            }
            else
            {
                jtoken = (JToken)NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            }
            ModDownloadLib.McUpdateLogShow(jtoken);
        }

        private static void McDownloadMenuSaveServer(object sender, RoutedEventArgs e)
        {
            OurListItem ourListItem;
            if (sender is OurListItem)
            {
                ourListItem = (OurListItem)sender;
            }
            else if (NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null) is OurListItem)
            {
                ourListItem = (OurListItem)NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null);
            }
            else
            {
                ourListItem = (OurListItem)NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Parent", new object[0], null, null, null);
            }
            try
            {
                ModDownloadLib._Closure___8_0 CS_locals1 = new ModDownloadLib._Closure___8_0(null);
                CS_locals1._VB_Local_Id = ourListItem.Title;
                string text = NewLateBinding.LateIndexGet(ourListItem.Tag, new object[] { "url" }, null).ToString();
                CS_locals1._VB_Local_VersionFolder = Dialogs.SelectFolder("选择目标文件夹", false, null).FirstOrDefault<string>();
                if (CS_locals1._VB_Local_VersionFolder != null)
                {
                    CS_locals1._VB_Local_VersionFolder = CS_locals1._VB_Local_VersionFolder + CS_locals1._VB_Local_Id + "\\";
                    try
                    {
                        List<ModLoader.LoaderBase>.Enumerator enumerator = ModLoader.LoaderTaskbar.ToList<ModLoader.LoaderBase>().GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (Operators.CompareString(enumerator.Current.Name, string.Format("Minecraft {0} 服务端下载", CS_locals1._VB_Local_Id), false) == 0)
                            {
                                ModMain.Hint("该服务端正在下载中！", ModMain.HintType.Red, true);
                                return;
                            }
                        }
                    }
                    finally
                    {
                        List<ModLoader.LoaderBase>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                    List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
                    list.Add(new ModNet.LoaderDownload("下载版本 JSON 文件", new List<ModNet.NetFile>
                    {
                        new ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(text), CS_locals1._VB_Local_VersionFolder + CS_locals1._VB_Local_Id + ".json", new FileChecker
                        {
                            IsJson = true
                        }, false)
                    })
                    {
                        ProgressWeight = 2.0
                    });
                    list.Add(new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("构建服务端", new Action<ModLoader.LoaderTask<string, List<ModNet.NetFile>>>(CS_locals1._Lambda___0), null, ThreadPriority.Normal)
                    {
                        ProgressWeight = 0.5,
                        Show = false
                    });
                    list.Add(new ModNet.LoaderDownload("下载服务端文件", new List<ModNet.NetFile>())
                    {
                        ProgressWeight = 5.0
                    });
                    ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>("Minecraft " + CS_locals1._VB_Local_Id + " 服务端下载", list);
                    loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.LoaderStateChangedHintOnly);
                    loaderCombo.Start(CS_locals1._VB_Local_Id, false);
                    ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                    ModMain.FrmMain.BtnExtraDownload.Ribble();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "开始 Minecraft 服务端下载失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }
        }

        public static void McDownloadMenuSave(object sender, RoutedEventArgs e)
        {
            OurListItem ourListItem;
            if (sender is OurListItem)
            {
                ourListItem = (OurListItem)sender;
            }
            else if (NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null) is OurListItem)
            {
                ourListItem = (OurListItem)NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null);
            }
            else
            {
                ourListItem = (OurListItem)NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Parent", new object[0], null, null, null);
            }
            try
            {
                ModDownloadLib._Closure___9_0 CS_locals1 = new ModDownloadLib._Closure___9_0(null);
                string title = ourListItem.Title;
                string text = NewLateBinding.LateIndexGet(ourListItem.Tag, new object[] { "url" }, null).ToString();
                CS_locals1._VB_Local_VersionFolder = Dialogs.SelectFolder("选择目标文件夹", false, null).FirstOrDefault<string>();
                if (CS_locals1._VB_Local_VersionFolder != null)
                {
                    CS_locals1._VB_Local_VersionFolder = CS_locals1._VB_Local_VersionFolder + title + "\\";
                    try
                    {
                        List<ModLoader.LoaderBase>.Enumerator enumerator = ModLoader.LoaderTaskbar.ToList<ModLoader.LoaderBase>().GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (Operators.CompareString(enumerator.Current.Name, string.Format("Minecraft {0} 下载", title), false) == 0)
                            {
                                ModMain.Hint("该版本正在下载中！", ModMain.HintType.Red, true);
                                return;
                            }
                        }
                    }
                    finally
                    {
                        List<ModLoader.LoaderBase>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                    List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
                    list.Add(new ModNet.LoaderDownload("下载版本 JSON 文件", new List<ModNet.NetFile>
                    {
                        new ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(text), CS_locals1._VB_Local_VersionFolder + title + ".json", new FileChecker
                        {
                            IsJson = true
                        }, false)
                    })
                    {
                        ProgressWeight = 2.0
                    });
                    list.Add(new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("分析版本 JAR 文件下载地址", new Action<ModLoader.LoaderTask<string, List<ModNet.NetFile>>>(CS_locals1._Lambda___0), null, ThreadPriority.Normal)
                    {
                        ProgressWeight = 0.5,
                        Show = false
                    });
                    list.Add(new ModNet.LoaderDownload("下载版本 JAR 文件", new List<ModNet.NetFile>())
                    {
                        ProgressWeight = 5.0
                    });
                    ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>("Minecraft " + title + " 下载", list);
                    loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.LoaderStateChangedHintOnly);
                    loaderCombo.Start(title, false);
                    ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                    ModMain.FrmMain.BtnExtraDownload.Ribble();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "开始 Minecraft 下载失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }
        }

        public static void McUpdateLogShow(JToken InstanceJson)
        {
            string text = InstanceJson["id"].ToString().Lower();
            string text2;
            if (Operators.CompareString(text, "3d shareware v1.34", false) == 0)
            {
                text2 = "3D_Shareware_v1.34";
            }
            else if (Operators.CompareString(text, "2.0", false) == 0)
            {
                text2 = "Java版2.0";
            }
            else if (Operators.CompareString(text, "1.rv-pre1", false) == 0)
            {
                text2 = "Java版1.RV-Pre1";
            }
            else if (Operators.CompareString(text, "combat test 1", false) == 0 || text.Contains("combat-1") || text.Contains("combat-212796"))
            {
                text2 = "Java版1.14.3_-_Combat_Test";
            }
            else if (Operators.CompareString(text, "combat test 2", false) == 0 || text.Contains("combat-2") || text.Contains("combat-0"))
            {
                text2 = "Java版Combat_Test_2";
            }
            else if (Operators.CompareString(text, "combat test 3", false) == 0 || Operators.CompareString(text, "1.14_combat-3", false) == 0)
            {
                text2 = "Java版Combat_Test_3";
            }
            else if (Operators.CompareString(text, "combat test 4", false) == 0 || Operators.CompareString(text, "1.15_combat-1", false) == 0)
            {
                text2 = "Java版Combat_Test_4";
            }
            else if (Operators.CompareString(text, "combat test 5", false) == 0 || Operators.CompareString(text, "1.15_combat-6", false) == 0)
            {
                text2 = "Java版Combat_Test_5";
            }
            else if (Operators.CompareString(text, "combat test 6", false) == 0 || Operators.CompareString(text, "1.16_combat-0", false) == 0)
            {
                text2 = "Java版Combat_Test_6";
            }
            else if (Operators.CompareString(text, "combat test 7c", false) == 0 || Operators.CompareString(text, "1.16_combat-3", false) == 0)
            {
                text2 = "Java版Combat_Test_7c";
            }
            else if (Operators.CompareString(text, "combat test 8b", false) == 0 || Operators.CompareString(text, "1.16_combat-5", false) == 0)
            {
                text2 = "Java版Combat_Test_8b";
            }
            else if (Operators.CompareString(text, "combat test 8c", false) == 0 || Operators.CompareString(text, "1.16_combat-6", false) == 0)
            {
                text2 = "Java版Combat_Test_8c";
            }
            else if (Operators.CompareString(text, "1.0.0-rc2-2", false) == 0)
            {
                text2 = "Java版RC2";
            }
            else if (text.StartsWithF("1.19_deep_dark_experimental_snapshot-", false) || text.StartsWithF("1_19_deep_dark_experimental_snapshot-", false))
            {
                text2 = text.Replace("1_19", "1.19").Replace("1.19_deep_dark_experimental_snapshot-", "Java版Deep_Dark_Experimental_Snapshot_");
            }
            else if (Operators.CompareString(text, "b1.9-pre6", false) == 0)
            {
                text2 = "Java版Beta_1.9_Prerelease_6";
            }
            else if (text.Contains("b1.9"))
            {
                text2 = "Java版Beta_1.9_Prerelease";
            }
            else if ((string)InstanceJson["type"] == "release" || (string)InstanceJson["type"] == "snapshot" || (string)InstanceJson["type"] == "special")
            {
                text2 = (text.Contains("w") ? "" : "Java版") + text.Replace(" Pre-Release ", "-pre");
            }
            else if (text.StartsWithF("b", false))
            {
                text2 = "Java版" + text.TrimEnd(new char[] { 'a', 'b', 'c', 'd', 'e' }).Replace("b", "Beta_");
            }
            else if (text.StartsWithF("a", false))
            {
                text2 = "Java版" + text.TrimEnd(new char[] { 'a', 'b', 'c', 'd', 'e' }).Replace("a", "Alpha_v");
            }
            else if (Operators.CompareString(text, "inf-20100618", false) == 0)
            {
                text2 = "Java版Infdev_20100618";
            }
            else if (Operators.CompareString(text, "c0.30_01c", false) == 0 || Operators.CompareString(text, "c0.30_survival", false) == 0 || text.Contains("生存测试"))
            {
                text2 = "Java版Classic_0.30（生存模式）";
            }
            else if (text.StartsWithF("c0.31", false))
            {
                text2 = "Java版Indev_0.31_20100130";
            }
            else if (text.StartsWithF("c", false))
            {
                text2 = "Java版" + text.Replace("c", "Classic_");
            }
            else
            {
                if (!text.StartsWithF("rd-", false))
                {
                    Logger.Error(string.Format("未知的版本格式：{0}。", text), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                    return;
                }
                text2 = "Java版Pre-classic_" + text;
            }
            ModBase.OpenWebsite("https://zh.minecraft.wiki/w/" + text2.Replace("_experimental-snapshot-", "-exp"));
        }

        private static void McDownloadOptiFineSave(ModDownload.DlOptiFineListEntry DownloadInfo)
        {
            try
            {
                string text = Dialogs.SaveFile("选择保存位置", DownloadInfo.FileName, null, new ValueTuple<string, string>[]
                {
                    new ValueTuple<string, string>("jar", "OptiFine Jar")
                });
                if (text != null)
                {
                    try
                    {
                        List<ModLoader.LoaderBase>.Enumerator enumerator = ModLoader.LoaderTaskbar.ToList<ModLoader.LoaderBase>().GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (Operators.CompareString(enumerator.Current.Name, string.Format("OptiFine {0} 下载", DownloadInfo.DisplayName), false) == 0)
                            {
                                ModMain.Hint("该版本正在下载中！", ModMain.HintType.Red, true);
                                return;
                            }
                        }
                    }
                    finally
                    {
                        List<ModLoader.LoaderBase>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                    ModLoader.LoaderCombo<ModDownload.DlOptiFineListEntry> loaderCombo = new ModLoader.LoaderCombo<ModDownload.DlOptiFineListEntry>("OptiFine " + DownloadInfo.DisplayName + " 下载", ModDownloadLib.McDownloadOptiFineSaveLoader(DownloadInfo, text));
                    loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.LoaderStateChangedHintOnly);
                    loaderCombo.Start(DownloadInfo, false);
                    ModLoader.LoaderTaskbarAdd<ModDownload.DlOptiFineListEntry>(loaderCombo);
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                    ModMain.FrmMain.BtnExtraDownload.Ribble();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "开始 OptiFine 下载失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }
        }

        private static void McDownloadOptiFineInstall(string BaseMcFolderHome, string Target, ModLoader.LoaderTask<List<ModNet.NetFile>, bool> Task, bool UseJavaWrapper)
        {
            ModDownloadLib._Closure___12_2 CS_locals1 = new ModDownloadLib._Closure___12_2(null);
            CS_locals1._VB_Local_Task = Task;
            ValueRange<Version> valueRange = ValueRange<Version>.AtLeast(new Version(21, 0));
            checked
            {
                try
                {
                    using (ZipArchive zipArchive = FileUtils.OpenZip(Target, default(CancellationToken)))
                    {
                        ZipArchiveEntry entry = zipArchive.GetEntry("optifine/Installer.class");
                        if (entry == null)
                        {
                            throw new FileNotFoundException("未找到 optifine/Installer.class");
                        }
                        using (Stream stream = entry.Open())
                        {
                            using (BinaryReader binaryReader = new BinaryReader(stream))
                            {
                                byte[] array = binaryReader.ReadBytes(8);
                                if (array.Length < 8)
                                {
                                    throw new IOException("Installer.class 长度不足");
                                }
                                if (array[0] != 202 || array[1] != 254 || array[2] != 186 || array[3] != 190)
                                {
                                    throw new InvalidDataException("Installer.class 文件头无效");
                                }
                                int num = (int)array[6] * 256 + (int)array[7];
                                if (num < 49)
                                {
                                    throw new InvalidDataException("Installer.class 版本过低");
                                }
                                if (num > 100)
                                {
                                    throw new InvalidDataException("Installer.class 版本过高");
                                }
                                valueRange = ValueRange<Version>.AtLeast(new Version(num - 44, 0));
                                Logger.Info(string.Format("OptiFine 安装器 class 版本：{0}，需要 Java {1}", num, valueRange), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "读取 OptiFine 安装器 Java 版本失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                Java java = ModJava.SelectOrDownloadJava(valueRange, true, CS_locals1._VB_Local_Task.CreateCancellationToken(), CS_locals1._VB_Local_Task.CreateSyncProgressProvider(0.0, 0.2));
                if (!CS_locals1._VB_Local_Task.IsCanceled)
                {
                    if (java == null)
                    {
                        throw new OperationCanceledException();
                    }
                    string text;
                    if (UseJavaWrapper && !Settings.Get<bool>("LaunchAdvanceDisableJLW", null))
                    {
                        text = string.Format("-Doolloo.jlw.tmpdir=\"{0}\" -Duser.home=\"{1}\" -cp \"{2}\" -jar \"{3}\" optifine.Installer", new object[]
                        {
                            PathUtils.RemoveSlashSuffix(ModBase.PathPure.Value),
                            PathUtils.RemoveSlashSuffix(BaseMcFolderHome),
                            Target,
                            LaunchUtils.ExtractPatch("Launch\\JavaWrapper.jar", ModBase.PathPure.Value)
                        });
                    }
                    else
                    {
                        text = string.Format("-Duser.home=\"{0}\" -cp \"{1}\" optifine.Installer", PathUtils.RemoveSlashSuffix(BaseMcFolderHome), Target);
                    }
                    if (java.Version.Major >= 9)
                    {
                        text = "--add-exports cpw.mods.bootstraplauncher/cpw.mods.bootstraplauncher=ALL-UNNAMED " + text;
                    }
                    object installSyncLock = ModDownloadLib.InstallSyncLock;
                    ObjectFlowControl.CheckForSyncLockOnValueType(installSyncLock);
                    lock (installSyncLock)
                    {
                        ModDownloadLib._Closure___12_1 CS_locals2 = new ModDownloadLib._Closure___12_1(null);
                        CS_locals2._VB_NonLocal__VB_Closure_2 = CS_locals1;
                        ProcessStartInfo processStartInfo = new ProcessStartInfo
                        {
                            FileName = java.JavaExePath,
                            Arguments = text,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true,
                            WorkingDirectory = PathUtils.ToShortPath(BaseMcFolderHome)
                        };
                        if (processStartInfo.EnvironmentVariables.ContainsKey("appdata"))
                        {
                            processStartInfo.EnvironmentVariables["appdata"] = BaseMcFolderHome;
                        }
                        else
                        {
                            processStartInfo.EnvironmentVariables.Add("appdata", BaseMcFolderHome);
                        }
                        Logger.Info(string.Format("开始安装 OptiFine：{0}", Target), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        CS_locals2._VB_Local_TotalLength = 0;
                        CS_locals2._VB_Local_process = new Process
                        {
                            StartInfo = processStartInfo
                        };
                        CS_locals2._VB_Local_LastResult = "";
                        ModDownloadLib._Closure___12_0 CS_locals3 = new ModDownloadLib._Closure___12_0(null);
                        CS_locals3._VB_NonLocal__VB_Closure_3 = CS_locals2;
                        CS_locals3._VB_Local_outputWaitHandle = new AutoResetEvent(false);
                        try
                        {
                            ModDownloadLib._Closure___12_3 CS_locals4 = new ModDownloadLib._Closure___12_3(null);
                            CS_locals4._VB_NonLocal__VB_Closure_4 = CS_locals3;
                            CS_locals4._VB_Local_errorWaitHandle = new AutoResetEvent(false);
                            try
                            {
                                CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.OutputDataReceived += CS_locals4._VB_NonLocal__VB_Closure_4._Lambda___R6;
                                CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.ErrorDataReceived += CS_locals4._Lambda___R7;
                                CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.Start();
                                CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.BeginOutputReadLine();
                                CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.BeginErrorReadLine();
                                while (!CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.HasExited)
                                {
                                    Thread.Sleep(10);
                                }
                                CS_locals4._VB_NonLocal__VB_Closure_4._VB_Local_outputWaitHandle.WaitOne(10000);
                                CS_locals4._VB_Local_errorWaitHandle.WaitOne(10000);
                                CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.Dispose();
                                if (CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_TotalLength < 1000 || CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_LastResult.Contains("at "))
                                {
                                    throw new Exception("安装器运行出错，末行为 " + CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_LastResult);
                                }
                            }
                            finally
                            {
                                if (CS_locals4._VB_Local_errorWaitHandle != null)
                                {
                                    ((IDisposable)CS_locals4._VB_Local_errorWaitHandle).Dispose();
                                }
                            }
                        }
                        finally
                        {
                            if (CS_locals3._VB_Local_outputWaitHandle != null)
                            {
                                ((IDisposable)CS_locals3._VB_Local_outputWaitHandle).Dispose();
                            }
                        }
                    }
                }
            }
        }

        private static List<ModLoader.LoaderBase> McDownloadOptiFineLoader(ModDownload.DlOptiFineListEntry DownloadInfo, string McFolder, ModLoader.LoaderCombo<string> ClientDownloadLoader, string ClientFolder)
        {
            ModDownloadLib._Closure___13_0 CS_locals1 = new ModDownloadLib._Closure___13_0(null);
            CS_locals1._VB_Local_DownloadInfo = DownloadInfo;
            CS_locals1._VB_Local_McFolder = McFolder;
            CS_locals1._VB_Local_ClientDownloadLoader = ClientDownloadLoader;
            CS_locals1._VB_Local_ClientFolder = ClientFolder;
            CS_locals1._VB_Local_Id = CS_locals1._VB_Local_DownloadInfo.InstanceName;
            CS_locals1._VB_Local_VersionFolder = CS_locals1._VB_Local_McFolder + "versions\\" + CS_locals1._VB_Local_Id + "\\";
            bool flag = CS_locals1._VB_Local_DownloadInfo.Inherit.Contains("w") || McVersion.VersionToDrop(CS_locals1._VB_Local_DownloadInfo.Inherit) >= 140;
            CS_locals1._VB_Local_Target = (flag ? string.Format("{0}OptiFine.jar", ModMain.RequestTaskTempFolder(true)) : string.Format("{0}libraries\\optifine\\OptiFine\\{1}\\{2}", CS_locals1._VB_Local_McFolder, PathUtils.GetFileNameWithoutExtension(CS_locals1._VB_Local_DownloadInfo.FileName).Replace("OptiFine_", "").Replace("preview_", ""), CS_locals1._VB_Local_DownloadInfo.FileName.Replace("OptiFine_", "OptiFine-").Replace("preview_", "")));
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            list.Add(new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("获取 OptiFine 主文件下载地址", new Action<ModLoader.LoaderTask<string, List<ModNet.NetFile>>>(CS_locals1._Lambda___0), null, ThreadPriority.Normal)
            {
                ProgressWeight = 8.0
            });
            list.Add(new ModNet.LoaderDownload("下载 OptiFine 主文件", new List<ModNet.NetFile>())
            {
                ProgressWeight = 8.0
            });
            list.Add(new ModLoader.LoaderTask<List<ModNet.NetFile>, bool>("等待原版下载", new Action<ModLoader.LoaderTask<List<ModNet.NetFile>, bool>>(CS_locals1._Lambda___1), null, ThreadPriority.Normal)
            {
                ProgressWeight = 0.1,
                Show = false
            });
            if (flag)
            {
                Logger.Info(string.Format("检测为新版 OptiFine：{0}", CS_locals1._VB_Local_DownloadInfo.Inherit), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                list.Add(new ModLoader.LoaderTask<List<ModNet.NetFile>, bool>("安装 OptiFine（方式 A）", new Action<ModLoader.LoaderTask<List<ModNet.NetFile>, bool>>(CS_locals1._Lambda___5), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 8.0
                });
            }
            else
            {
                Logger.Info(string.Format("检测为旧版 OptiFine：{0}", CS_locals1._VB_Local_DownloadInfo.Inherit), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                list.Add(new ModLoader.LoaderTask<List<ModNet.NetFile>, bool>("安装 OptiFine（方式 B）", new Action<ModLoader.LoaderTask<List<ModNet.NetFile>, bool>>(CS_locals1._Lambda___6), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 1.0
                });
            }
            return list;
        }

        private static List<ModLoader.LoaderBase> McDownloadOptiFineSaveLoader(ModDownload.DlOptiFineListEntry DownloadInfo, string TargetFolder)
        {
            ModDownloadLib._Closure___14_0 CS_locals1 = new ModDownloadLib._Closure___14_0();
            CS_locals1._VB_Local_DownloadInfo = DownloadInfo;
            CS_locals1._VB_Local_TargetFolder = TargetFolder;
            return new List<ModLoader.LoaderBase>
            {
                new ModLoader.LoaderTask<ModDownload.DlOptiFineListEntry, List<ModNet.NetFile>>("获取 OptiFine 下载地址", new Action<ModLoader.LoaderTask<ModDownload.DlOptiFineListEntry, List<ModNet.NetFile>>>(CS_locals1._Lambda___0), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 6.0
                },
                new ModNet.LoaderDownload("下载 OptiFine 主文件", new List<ModNet.NetFile>())
                {
                    ProgressWeight = 10.0,
                    Block = true
                }
            };
        }

        public static OurVirtualizingElement<OurListItem> OptiFineDownloadListItem(ModDownload.DlOptiFineListEntry Entry, OurListItem.ClickEventHandler OnClick, bool IsSaveOnly)
        {
            ModDownloadLib._Closure___15_0 CS_locals1 = new ModDownloadLib._Closure___15_0();
            CS_locals1._VB_Local_Entry = Entry;
            CS_locals1._VB_Local_OnClick = OnClick;
            CS_locals1._VB_Local_IsSaveOnly = IsSaveOnly;
            return new OurVirtualizingElement<OurListItem>(new Func<OurListItem>(CS_locals1._Lambda___0))
            {
                Height = 42.0
            };
        }

        private static void OptiFineSaveContMenuBuild(object sender, EventArgs e)
        {
            OurIconButton ourIconButton = new OurIconButton
            {
                LogoScale = 1.05,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                ToolTip = "更新日志"
            };
            ourIconButton.Click += ((ModDownloadLib._Closure___._IR16_8 == null) ? (ModDownloadLib._Closure___._IR16_8 = new OurIconButton.ClickEventHandler(ModDownloadLib._Closure___._I._Lambda___R16_8)) : ModDownloadLib._Closure___._IR16_8);
            NewLateBinding.LateSet(sender, null, "Buttons", new object[] { new OurIconButton[] { ourIconButton } }, null, null);
        }

        private static void OptiFineContMenuBuild(object sender, EventArgs e)
        {
            OurIconButton ourIconButton = new OurIconButton
            {
                Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z",
                ToolTip = "另存为"
            };
            ourIconButton.Click += ((ModDownloadLib._Closure___._IR17_9 == null) ? (ModDownloadLib._Closure___._IR17_9 = new OurIconButton.ClickEventHandler(ModDownloadLib._Closure___._I._Lambda___R17_9)) : ModDownloadLib._Closure___._IR17_9);
            OurIconButton ourIconButton2 = new OurIconButton
            {
                LogoScale = 1.05,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                ToolTip = "更新日志"
            };
            ourIconButton2.Click += ((ModDownloadLib._Closure___._IR17_10 == null) ? (ModDownloadLib._Closure___._IR17_10 = new OurIconButton.ClickEventHandler(ModDownloadLib._Closure___._I._Lambda___R17_10)) : ModDownloadLib._Closure___._IR17_10);
            NewLateBinding.LateSet(sender, null, "Buttons", new object[] { new OurIconButton[] { ourIconButton, ourIconButton2 } }, null, null);
        }

        private static void OptiFineLog_Click(object sender, RoutedEventArgs e)
        {
            ModDownload.DlOptiFineListEntry dlOptiFineListEntry;
            if (NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null) != null)
            {
                dlOptiFineListEntry = (ModDownload.DlOptiFineListEntry)NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null);
            }
            else if (NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null) != null)
            {
                dlOptiFineListEntry = (ModDownload.DlOptiFineListEntry)NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            }
            else
            {
                dlOptiFineListEntry = (ModDownload.DlOptiFineListEntry)NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            }
            ModBase.OpenWebsite("https://optifine.net/changelog?f=" + dlOptiFineListEntry.FileName);
        }

        public static void OptiFineSave_Click(object sender, RoutedEventArgs e)
        {
            ModDownload.DlOptiFineListEntry dlOptiFineListEntry;
            if (NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null) != null)
            {
                dlOptiFineListEntry = (ModDownload.DlOptiFineListEntry)NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null);
            }
            else if (NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null) != null)
            {
                dlOptiFineListEntry = (ModDownload.DlOptiFineListEntry)NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            }
            else
            {
                dlOptiFineListEntry = (ModDownload.DlOptiFineListEntry)NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            }
            ModDownloadLib.McDownloadOptiFineSave(dlOptiFineListEntry);
        }

        private static void McDownloadLiteLoaderSave(ModDownload.DlLiteLoaderListEntry DownloadInfo)
        {
            try
            {
                string inherit = DownloadInfo.Inherit;
                string text = Dialogs.SaveFile("选择保存位置", DownloadInfo.FileName.Replace("-SNAPSHOT", ""), null, new ValueTuple<string, string>[]
                {
                    new ValueTuple<string, string>("jar", "LiteLoader 安装器")
                });
                if (text != null)
                {
                    try
                    {
                        List<ModLoader.LoaderBase>.Enumerator enumerator = ModLoader.LoaderTaskbar.ToList<ModLoader.LoaderBase>().GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (Operators.CompareString(enumerator.Current.Name, string.Format("LiteLoader {0} 下载", inherit), false) == 0)
                            {
                                ModMain.Hint("该版本正在下载中！", ModMain.HintType.Red, true);
                                return;
                            }
                        }
                    }
                    finally
                    {
                        List<ModLoader.LoaderBase>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                    List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
                    List<string> list2 = new List<string>();
                    if (DownloadInfo.IsLegacy)
                    {
                        string inherit2 = DownloadInfo.Inherit;
                        if (Operators.CompareString(inherit2, "1.7.10", false) != 0)
                        {
                            if (Operators.CompareString(inherit2, "1.7.2", false) != 0)
                            {
                                if (Operators.CompareString(inherit2, "1.6.4", false) != 0)
                                {
                                    if (Operators.CompareString(inherit2, "1.6.2", false) != 0)
                                    {
                                        if (Operators.CompareString(inherit2, "1.5.2", false) != 0)
                                        {
                                            throw new NotSupportedException("未知的 Minecraft 版本（" + DownloadInfo.Inherit + "）");
                                        }
                                        list2.Add("https://dl.liteloader.com/redist/1.5.2/liteloader-installer-1.5.2-01.jar");
                                    }
                                    else
                                    {
                                        list2.Add("https://dl.liteloader.com/redist/1.6.2/liteloader-installer-1.6.2-04.jar");
                                    }
                                }
                                else
                                {
                                    list2.Add("https://dl.liteloader.com/redist/1.6.4/liteloader-installer-1.6.4-01.jar");
                                }
                            }
                            else
                            {
                                list2.Add("https://dl.liteloader.com/redist/1.7.2/liteloader-installer-1.7.2-04.jar");
                            }
                        }
                        else
                        {
                            list2.Add("https://dl.liteloader.com/redist/1.7.10/liteloader-installer-1.7.10-04.jar");
                        }
                    }
                    else
                    {
                        list2.Add(string.Concat(new string[]
                        {
                            "http://jenkins.liteloader.com/job/LiteLoaderInstaller%20",
                            DownloadInfo.Inherit,
                            "/lastSuccessfulBuild/artifact/",
                            (Operators.CompareString(DownloadInfo.Inherit, "1.8", false) == 0) ? "ant/dist/" : "build/libs/",
                            DownloadInfo.FileName
                        }));
                    }
                    list.Add(new ModNet.LoaderDownload("下载主文件", new List<ModNet.NetFile>
                    {
                        new ModNet.NetFile(list2.ToArray(), text, new FileChecker
                        {
                            MinSize = 1048576L
                        }, false)
                    })
                    {
                        ProgressWeight = 15.0
                    });
                    ModLoader.LoaderCombo<ModDownload.DlLiteLoaderListEntry> loaderCombo = new ModLoader.LoaderCombo<ModDownload.DlLiteLoaderListEntry>("LiteLoader " + inherit + " 安装器下载", list);
                    loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.LoaderStateChangedHintOnly);
                    loaderCombo.Start(DownloadInfo, false);
                    ModLoader.LoaderTaskbarAdd<ModDownload.DlLiteLoaderListEntry>(loaderCombo);
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                    ModMain.FrmMain.BtnExtraDownload.Ribble();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "开始 LiteLoader 安装器下载失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }
        }

        private static List<ModLoader.LoaderBase> McDownloadLiteLoaderLoader(ModDownload.DlLiteLoaderListEntry DownloadInfo, string McFolder)
        {
            ModDownloadLib._Closure___21_0 CS_locals1 = new ModDownloadLib._Closure___21_0();
            CS_locals1._VB_Local_DownloadInfo = DownloadInfo;
            CS_locals1._VB_Local_McFolder = McFolder;
            return new List<ModLoader.LoaderBase>
            {
                new ModLoader.LoaderTask<string, string>("安装 LiteLoader", new Action<ModLoader.LoaderTask<string, string>>(CS_locals1._Lambda___0), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 1.0
                }
            };
        }

        public static OurVirtualizingElement<OurListItem> LiteLoaderDownloadListItem(ModDownload.DlLiteLoaderListEntry Entry, OurListItem.ClickEventHandler OnClick, bool IsSaveOnly)
        {
            ModDownloadLib._Closure___22_0 CS_locals1 = new ModDownloadLib._Closure___22_0();
            CS_locals1._VB_Local_Entry = Entry;
            CS_locals1._VB_Local_OnClick = OnClick;
            CS_locals1._VB_Local_IsSaveOnly = IsSaveOnly;
            return new OurVirtualizingElement<OurListItem>(new Func<OurListItem>(CS_locals1._Lambda___0))
            {
                Height = 42.0
            };
        }

        private static void LiteLoaderSaveContMenuBuild(OurListItem sender, EventArgs e)
        {
            if (Conversions.ToBoolean(NewLateBinding.LateGet(sender.Tag, null, "IsLegacy", new object[0], null, null, null)))
            {
                sender.Buttons = new OurIconButton[0];
                return;
            }
            OurIconButton ourIconButton = new OurIconButton
            {
                Logo = "M384 128h640v128H384zM160 192m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0ZM384 448h640v128H384zM160 512m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0ZM384 768h640v128H384zM160 832m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0Z",
                ToolTip = "查看全部版本",
                Tag = sender
            };
            ourIconButton.Click += ((ModDownloadLib._Closure___._IR23_11 == null) ? (ModDownloadLib._Closure___._IR23_11 = new OurIconButton.ClickEventHandler(ModDownloadLib._Closure___._I._Lambda___R23_11)) : ModDownloadLib._Closure___._IR23_11);
            sender.Buttons = new OurIconButton[] { ourIconButton };
        }

        private static void LiteLoaderContMenuBuild(OurListItem sender, EventArgs e)
        {
            OurIconButton ourIconButton = new OurIconButton
            {
                Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z",
                ToolTip = "保存安装器",
                Tag = sender
            };
            ourIconButton.Click += ((ModDownloadLib._Closure___._IR24_12 == null) ? (ModDownloadLib._Closure___._IR24_12 = new OurIconButton.ClickEventHandler(ModDownloadLib._Closure___._I._Lambda___R24_12)) : ModDownloadLib._Closure___._IR24_12);
            if (Conversions.ToBoolean(NewLateBinding.LateGet(sender.Tag, null, "IsLegacy", new object[0], null, null, null)))
            {
                sender.Buttons = new OurIconButton[] { ourIconButton };
                return;
            }
            OurIconButton ourIconButton2 = new OurIconButton
            {
                Logo = "M384 128h640v128H384zM160 192m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0ZM384 448h640v128H384zM160 512m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0ZM384 768h640v128H384zM160 832m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0Z",
                ToolTip = "查看全部版本",
                Tag = sender
            };
            ourIconButton2.Click += ((ModDownloadLib._Closure___._IR24_13 == null) ? (ModDownloadLib._Closure___._IR24_13 = new OurIconButton.ClickEventHandler(ModDownloadLib._Closure___._I._Lambda___R24_13)) : ModDownloadLib._Closure___._IR24_13);
            sender.Buttons = new OurIconButton[] { ourIconButton, ourIconButton2 };
        }

        private static void LiteLoaderAll_Click(object sender, RoutedEventArgs e)
        {
            ModDownload.DlLiteLoaderListEntry dlLiteLoaderListEntry;
            if (NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null) is ModDownload.DlLiteLoaderListEntry)
            {
                dlLiteLoaderListEntry = (ModDownload.DlLiteLoaderListEntry)NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null);
            }
            else
            {
                dlLiteLoaderListEntry = (ModDownload.DlLiteLoaderListEntry)NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            }
            ModBase.OpenWebsite("https://jenkins.liteloader.com/view/" + dlLiteLoaderListEntry.Inherit);
        }

        public static void LiteLoaderSave_Click(object sender, RoutedEventArgs e)
        {
            ModDownload.DlLiteLoaderListEntry dlLiteLoaderListEntry;
            if (NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null) is ModDownload.DlLiteLoaderListEntry)
            {
                dlLiteLoaderListEntry = (ModDownload.DlLiteLoaderListEntry)NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null);
            }
            else
            {
                dlLiteLoaderListEntry = (ModDownload.DlLiteLoaderListEntry)NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            }
            ModDownloadLib.McDownloadLiteLoaderSave(dlLiteLoaderListEntry);
        }

        public static void McDownloadForgelikeSave(ModDownload.DlForgelikeEntry Info)
        {
            try
            {
                string text = Dialogs.SaveFile("选择保存位置", string.Format("{0}-{1}-{2}.{3}", new object[] { Info.LoaderName, Info.Inherit, Info.VersionName, Info.FileExtension }), null, new ValueTuple<string, string>[]
                {
                    new ValueTuple<string, string>(Info.FileExtension, string.Format("{0} 安装器", Info.LoaderName))
                });
                if (text != null)
                {
                    string text2 = string.Format("{0} {1} - {2}", Info.LoaderName, Info.Inherit, Info.VersionName);
                    try
                    {
                        List<ModLoader.LoaderBase>.Enumerator enumerator = ModLoader.LoaderTaskbar.ToList<ModLoader.LoaderBase>().GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (Operators.CompareString(enumerator.Current.Name, string.Format("{0} 下载", text2), false) == 0)
                            {
                                ModMain.Hint("该版本正在下载中！", ModMain.HintType.Red, true);
                                return;
                            }
                        }
                    }
                    finally
                    {
                        List<ModLoader.LoaderBase>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                    List<ModNet.NetFile> list = new List<ModNet.NetFile>();
                    if (Info.IsNeoForge)
                    {
                        string text3 = ((ModDownload.DlNeoForgeListEntry)Info).UrlBase + "-installer.jar";
                        list.Add(new ModNet.NetFile(new string[]
                        {
                            text3.Replace("maven.neoforged.net/releases", "bmclapi2.bangbang93.com/maven"),
                            text3
                        }, text, new FileChecker
                        {
                            MinSize = 65536L
                        }, false));
                    }
                    else
                    {
                        ModDownload.DlForgeVersionEntry dlForgeVersionEntry = (ModDownload.DlForgeVersionEntry)Info;
                        list.Add(new ModNet.NetFile(new string[]
                        {
                            string.Format("https://bmclapi2.bangbang93.com/maven/net/minecraftforge/forge/{0}-{1}/forge-{2}-{3}-{4}.{5}", new object[] { dlForgeVersionEntry.Inherit, dlForgeVersionEntry.FileVersion, dlForgeVersionEntry.Inherit, dlForgeVersionEntry.FileVersion, dlForgeVersionEntry.Category, dlForgeVersionEntry.FileExtension }),
                            string.Format("https://files.minecraftforge.net/maven/net/minecraftforge/forge/{0}-{1}/forge-{2}-{3}-{4}.{5}", new object[] { dlForgeVersionEntry.Inherit, dlForgeVersionEntry.FileVersion, dlForgeVersionEntry.Inherit, dlForgeVersionEntry.FileVersion, dlForgeVersionEntry.Category, dlForgeVersionEntry.FileExtension })
                        }, text, new FileChecker
                        {
                            MinSize = 65536L,
                            Hash = dlForgeVersionEntry.Hash
                        }, false));
                    }
                    List<ModLoader.LoaderBase> list2 = new List<ModLoader.LoaderBase>();
                    list2.Add(new ModNet.LoaderDownload("下载主文件", list)
                    {
                        ProgressWeight = 6.0
                    });
                    ModLoader.LoaderCombo<ModDownload.DlForgelikeEntry> loaderCombo = new ModLoader.LoaderCombo<ModDownload.DlForgelikeEntry>(text2 + " 下载", list2);
                    loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.LoaderStateChangedHintOnly);
                    loaderCombo.Start(Info, false);
                    ModLoader.LoaderTaskbarAdd<ModDownload.DlForgelikeEntry>(loaderCombo);
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                    ModMain.FrmMain.BtnExtraDownload.Ribble();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, string.Format("开始 {0} 安装器下载失败", Info.LoaderName), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }
        }

        private static void ForgelikeInjector(string Target, ModLoader.LoaderTask<bool, bool> Task, string McFolder, bool UseJavaWrapper, bool IsNeoForge)
        {
            ModDownloadLib._Closure___28_2 CS_locals1 = new ModDownloadLib._Closure___28_2(null);
            CS_locals1._VB_Local_Task = Task;
            Java java = ModJava.SelectOrDownloadJava(ValueRange<Version>.AtLeast(new Version(8, 0, 60)), true, CS_locals1._VB_Local_Task.CreateCancellationToken(), CS_locals1._VB_Local_Task.CreateSyncProgressProvider(0.0, 0.2));
            checked
            {
                if (!CS_locals1._VB_Local_Task.IsCanceled)
                {
                    if (java == null)
                    {
                        throw new OperationCanceledException();
                    }
                    string text;
                    if (UseJavaWrapper && !Settings.Get<bool>("LaunchAdvanceDisableJLW", null))
                    {
                        text = string.Format("-Doolloo.jlw.tmpdir=\"{0}\" -cp \"{1}Cache\\forge_installer.jar;{2}\" -jar \"{3}\" com.bangbang93.ForgeInstaller \"{4}", new object[]
                        {
                            PathUtils.RemoveSlashSuffix(ModBase.PathPure.Value),
                            ModBase.PathTemp,
                            Target,
                            LaunchUtils.ExtractPatch("Launch\\JavaWrapper.jar", ModBase.PathPure.Value),
                            McFolder
                        });
                    }
                    else
                    {
                        text = string.Format("-cp \"{0}Cache\\forge_installer.jar;{1}\" com.bangbang93.ForgeInstaller \"{2}", ModBase.PathTemp, Target, McFolder);
                    }
                    if (java.Version.Major >= 9)
                    {
                        text = "--add-exports cpw.mods.bootstraplauncher/cpw.mods.bootstraplauncher=ALL-UNNAMED " + text;
                    }
                    object installSyncLock = ModDownloadLib.InstallSyncLock;
                    ObjectFlowControl.CheckForSyncLockOnValueType(installSyncLock);
                    lock (installSyncLock)
                    {
                        ModDownloadLib._Closure___28_1 CS_locals2 = new ModDownloadLib._Closure___28_1(null);
                        CS_locals2._VB_NonLocal__VB_Closure_2 = CS_locals1;
                        ProcessStartInfo processStartInfo = new ProcessStartInfo
                        {
                            FileName = java.JavaExePath,
                            Arguments = text,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true
                        };
                        CS_locals2._VB_Local_LoaderName = (IsNeoForge ? "NeoForge" : "Forge");
                        Logger.Info(string.Format("开始安装 {0}：{1}", CS_locals2._VB_Local_LoaderName, text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        CS_locals2._VB_Local_process = new Process
                        {
                            StartInfo = processStartInfo
                        };
                        CS_locals2._VB_Local_LastResults = new Queue<string>();
                        ModDownloadLib._Closure___28_0 CS_locals3 = new ModDownloadLib._Closure___28_0(null);
                        CS_locals3._VB_NonLocal__VB_Closure_3 = CS_locals2;
                        CS_locals3._VB_Local_outputWaitHandle = new AutoResetEvent(false);
                        try
                        {
                            ModDownloadLib._Closure___28_3 CS_locals4 = new ModDownloadLib._Closure___28_3(null);
                            CS_locals4._VB_NonLocal__VB_Closure_4 = CS_locals3;
                            CS_locals4._VB_Local_errorWaitHandle = new AutoResetEvent(false);
                            try
                            {
                                CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.OutputDataReceived += CS_locals4._VB_NonLocal__VB_Closure_4._Lambda___R14;
                                CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.ErrorDataReceived += CS_locals4._Lambda___R15;
                                CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.Start();
                                CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.BeginOutputReadLine();
                                CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.BeginErrorReadLine();
                                while (!CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.HasExited)
                                {
                                    Thread.Sleep(10);
                                }
                                CS_locals4._VB_NonLocal__VB_Closure_4._VB_Local_outputWaitHandle.WaitOne(10000);
                                CS_locals4._VB_Local_errorWaitHandle.WaitOne(10000);
                                CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.Dispose();
                                if (!CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_LastResults.Reverse<string>().Take<string>(5).Any<string>((ModDownloadLib._Closure___._I28_2 == null) ? (ModDownloadLib._Closure___._I28_2 = new Func<string, bool>(ModDownloadLib._Closure___._I._Lambda___28_2)) : ModDownloadLib._Closure___._I28_2))
                                {
                                    Logger.Warn(CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_LastResults.Join("\r\n"), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                                    string text2 = "";
                                    int num = Math.Max(0, CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_LastResults.Count - 5);
                                    int num2 = CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_LastResults.Count - 1;
                                    for (int i = num; i <= num2; i++)
                                    {
                                        text2 = text2 + "\r\n" + CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_LastResults.ElementAtOrDefault<string>(i);
                                    }
                                    throw new Exception(string.Format("{0} 安装器出错，日志结束部分为：", CS_locals4._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_LoaderName) + text2);
                                }
                            }
                            finally
                            {
                                if (CS_locals4._VB_Local_errorWaitHandle != null)
                                {
                                    ((IDisposable)CS_locals4._VB_Local_errorWaitHandle).Dispose();
                                }
                            }
                        }
                        finally
                        {
                            if (CS_locals3._VB_Local_outputWaitHandle != null)
                            {
                                ((IDisposable)CS_locals3._VB_Local_outputWaitHandle).Dispose();
                            }
                        }
                    }
                }
            }
        }

        private static void ForgelikeInjectorLine(string Content, ModLoader.LoaderTask<bool, bool> Task)
        {
            uint num = PrivateImplementationDetails_.ComputeStringHash(Content);
            if (num <= 2947477999U)
            {
                if (num <= 867564423U)
                {
                    if (num != 214422873U)
                    {
                        if (num != 538128555U)
                        {
                            if (num == 867564423U)
                            {
                                if (Operators.CompareString(Content, "Remapping jar... 50%", false) == 0)
                                {
                                    Task.Progress = 0.9;
                                    goto IL_036F;
                                }
                            }
                        }
                        else if (Operators.CompareString(Content, "Remapping final jar", false) == 0)
                        {
                            Task.Progress = 0.85;
                            goto IL_036F;
                        }
                    }
                    else if (Operators.CompareString(Content, "Splitting: ", false) == 0)
                    {
                        Task.Progress = 0.55;
                        goto IL_036F;
                    }
                }
                else if (num <= 2394603510U)
                {
                    if (num != 1592277622U)
                    {
                        if (num == 2394603510U)
                        {
                            if (Operators.CompareString(Content, "Building Processors", false) == 0)
                            {
                                Task.Progress = 0.38;
                                goto IL_036F;
                            }
                        }
                    }
                    else if (Operators.CompareString(Content, "log: null", false) == 0)
                    {
                        Task.Progress = 0.67;
                        goto IL_036F;
                    }
                }
                else if (num != 2846730644U)
                {
                    if (num == 2947477999U)
                    {
                        if (Operators.CompareString(Content, "Sorting", false) == 0)
                        {
                            Task.Progress = 0.8;
                            goto IL_036F;
                        }
                    }
                }
                else if (Operators.CompareString(Content, "Task: DOWNLOAD_MOJMAPS", false) == 0)
                {
                    Task.Progress = 0.4;
                    goto IL_036F;
                }
            }
            else if (num <= 3369270864U)
            {
                if (num <= 3198574777U)
                {
                    if (num != 2971959160U)
                    {
                        if (num == 3198574777U)
                        {
                            if (Operators.CompareString(Content, "Injecting profile", false) == 0)
                            {
                                Task.Progress = 0.98;
                                goto IL_036F;
                            }
                        }
                    }
                    else if (Operators.CompareString(Content, "  File exists: Checksum validated.", false) == 0)
                    {
                        Task.Progress += 0.003;
                        goto IL_036F;
                    }
                }
                else if (num != 3296415987U)
                {
                    if (num == 3369270864U)
                    {
                        if (Operators.CompareString(Content, "Extracting json", false) == 0)
                        {
                            Task.Progress = 0.27;
                            goto IL_036F;
                        }
                    }
                }
                else if (Operators.CompareString(Content, "Remapping jar... 100%", false) == 0)
                {
                    Task.Progress = 0.95;
                    goto IL_036F;
                }
            }
            else if (num <= 3905690050U)
            {
                if (num != 3587956665U)
                {
                    if (num == 3905690050U)
                    {
                        if (Operators.CompareString(Content, "Parameter Annotations", false) == 0)
                        {
                            Task.Progress = 0.6;
                            goto IL_036F;
                        }
                    }
                }
                else if (Operators.CompareString(Content, "Processing Complete", false) == 0)
                {
                    Task.Progress = 0.67;
                    goto IL_036F;
                }
            }
            else if (num != 3935946959U)
            {
                if (num == 4046473188U)
                {
                    if (Operators.CompareString(Content, "Downloading libraries", false) == 0)
                    {
                        Task.Progress = 0.28;
                        goto IL_036F;
                    }
                }
            }
            else if (Operators.CompareString(Content, "Task: MERGE_MAPPING", false) == 0)
            {
                Task.Progress = 0.5;
                goto IL_036F;
            }
            Logger.Trace(Content, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            return;
            IL_036F:
            Logger.Info(Content, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
        }

        private static List<ModLoader.LoaderBase> McDownloadForgelikeLoader(bool IsNeoForge, string LoaderVersion, string NewInstanceName, string Inherit, ModDownload.DlForgelikeEntry Info, string McFolder, ModLoader.LoaderCombo<string> ClientDownloadLoader, string ClientFolder)
        {
            ModDownloadLib._Closure___30_0 CS_locals1 = new ModDownloadLib._Closure___30_0(null);
            CS_locals1._VB_Local_IsNeoForge = IsNeoForge;
            CS_locals1._VB_Local_LoaderVersion = LoaderVersion;
            CS_locals1._VB_Local_NewInstanceName = NewInstanceName;
            CS_locals1._VB_Local_Inherit = Inherit;
            CS_locals1._VB_Local_Info = Info;
            CS_locals1._VB_Local_McFolder = McFolder;
            CS_locals1._VB_Local_ClientDownloadLoader = ClientDownloadLoader;
            CS_locals1._VB_Local_ClientFolder = ClientFolder;
            if (CS_locals1._VB_Local_IsNeoForge && CS_locals1._VB_Local_Info == null)
            {
                if (Operators.CompareString(CS_locals1._VB_Local_Inherit, "1.20.1", false) == 0 && !CS_locals1._VB_Local_LoaderVersion.StartsWithF("1.20.1-", false))
                {
                    CS_locals1._VB_Local_Info = new ModDownload.DlNeoForgeListEntry("1.20.1-" + CS_locals1._VB_Local_LoaderVersion);
                }
                else
                {
                    CS_locals1._VB_Local_Info = new ModDownload.DlNeoForgeListEntry(CS_locals1._VB_Local_LoaderVersion);
                }
            }
            if (!CS_locals1._VB_Local_IsNeoForge && CS_locals1._VB_Local_LoaderVersion.StartsWithF("1.", false) && CS_locals1._VB_Local_LoaderVersion.Contains("-"))
            {
                CS_locals1._VB_Local_Inherit = CS_locals1._VB_Local_LoaderVersion.BeforeFirst("-", false);
                CS_locals1._VB_Local_LoaderVersion = CS_locals1._VB_Local_LoaderVersion.AfterLast("-", false);
            }
            CS_locals1._VB_Local_LoaderName = (CS_locals1._VB_Local_IsNeoForge ? "NeoForge" : "Forge");
            CS_locals1._VB_Local_InstallerAddress = ModMain.RequestTaskTempFolder(true) + "forge_installer.jar";
            CS_locals1._VB_Local_VersionFolder = string.Format("{0}versions\\{1}\\", CS_locals1._VB_Local_McFolder, CS_locals1._VB_Local_NewInstanceName);
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            if (CS_locals1._VB_Local_Info == null)
            {
                list.Add(new ModLoader.LoaderTask<string, string>(string.Format("获取 {0} 详细信息", CS_locals1._VB_Local_LoaderName), new Action<ModLoader.LoaderTask<string, string>>(CS_locals1._Lambda___0), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 3.0
                });
            }
            list.Add(new ModLoader.LoaderTask<string, List<ModNet.NetFile>>(string.Format("准备下载 {0}", CS_locals1._VB_Local_LoaderName), new Action<ModLoader.LoaderTask<string, List<ModNet.NetFile>>>(CS_locals1._Lambda___1), null, ThreadPriority.Normal)
            {
                ProgressWeight = 0.5,
                Show = false
            });
            list.Add(new ModNet.LoaderDownload(string.Format("下载 {0} 主文件", CS_locals1._VB_Local_LoaderName), new List<ModNet.NetFile>())
            {
                ProgressWeight = 9.0
            });
            CS_locals1._VB_Local_CurrentMcFolder = ModMinecraft.McFolderSelected;
            if (CS_locals1._VB_Local_IsNeoForge || Conversions.ToDouble(CS_locals1._VB_Local_LoaderVersion.BeforeFirst(".", false)) >= 20.0)
            {
                ModDownloadLib._Closure___30_1 CS_locals2 = new ModDownloadLib._Closure___30_1(null);
                CS_locals2._VB_NonLocal__VB_Closure_2 = CS_locals1;
                Logger.Info(string.Format("检测为{0}Forge：{1}", CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_IsNeoForge ? " Neo" : "新版 ", CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_LoaderVersion), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                CS_locals2._VB_Local_Libs = null;
                list.Add(new ModLoader.LoaderTask<string, List<ModNet.NetFile>>(string.Format("分析 {0} 支持库文件", CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_LoaderName), new Action<ModLoader.LoaderTask<string, List<ModNet.NetFile>>>(CS_locals2._Lambda___2), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 1.0
                });
                list.Add(new ModNet.LoaderDownload(string.Format("下载 {0} 支持库文件", CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_LoaderName), new List<ModNet.NetFile>())
                {
                    ProgressWeight = 12.0
                });
                list.Add(new ModLoader.LoaderTask<List<ModNet.NetFile>, bool>(string.Format("获取 {0} 支持库文件", CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_LoaderName), new Action<ModLoader.LoaderTask<List<ModNet.NetFile>, bool>>(CS_locals2._Lambda___3), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 0.1,
                    Show = false
                });
                list.Add(new ModLoader.LoaderTask<bool, bool>(CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_IsNeoForge ? "安装 NeoForge" : "安装 Forge（方式 A）", new Action<ModLoader.LoaderTask<bool, bool>>(CS_locals2._VB_NonLocal__VB_Closure_2._Lambda___7), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 10.0
                });
            }
            else
            {
                Logger.Info(string.Format("检测为非新版 Forge：{0}", CS_locals1._VB_Local_LoaderVersion), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                list.Add(new ModLoader.LoaderTask<List<ModNet.NetFile>, bool>(string.Format("安装 {0}（方式 B）", CS_locals1._VB_Local_LoaderName), new Action<ModLoader.LoaderTask<List<ModNet.NetFile>, bool>>(CS_locals1._Lambda___12), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 1.0
                });
            }
            return list;
        }

        public static void ForgeDownloadListItemPreload(StackPanel Stack, List<ModDownload.DlForgeVersionEntry> Entries, OurListItem.ClickEventHandler OnClick, bool IsSaveOnly)
        {
            if (!Entries.IsSingle<ModDownload.DlForgeVersionEntry>())
            {
                ModDownload.DlForgeVersionEntry dlForgeVersionEntry = null;
                if (Entries.Any<ModDownload.DlForgeVersionEntry>())
                {
                    dlForgeVersionEntry = Entries[0];
                }
                else
                {
                    Logger.Info("未找到可用的 Forge 版本", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                ModDownload.DlForgeVersionEntry dlForgeVersionEntry2 = null;
                try
                {
                    foreach (ModDownload.DlForgeVersionEntry dlForgeVersionEntry3 in Entries)
                    {
                        if (dlForgeVersionEntry3.IsRecommended)
                        {
                            dlForgeVersionEntry2 = dlForgeVersionEntry3;
                        }
                    }
                }
                finally
                {
                    List<ModDownload.DlForgeVersionEntry>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
                if (dlForgeVersionEntry != null && dlForgeVersionEntry == dlForgeVersionEntry2)
                {
                    dlForgeVersionEntry = null;
                }
                if (dlForgeVersionEntry2 != null)
                {
                    OurListItem ourListItem = ModDownloadLib.ForgeDownloadListItem(dlForgeVersionEntry2, OnClick, IsSaveOnly).Init();
                    ourListItem.Info = "推荐版" + ((Operators.CompareString(ourListItem.Info, "", false) == 0) ? "" : ("，" + ourListItem.Info));
                    Stack.Children.Add(ourListItem);
                }
                if (dlForgeVersionEntry != null)
                {
                    OurListItem ourListItem2 = ModDownloadLib.ForgeDownloadListItem(dlForgeVersionEntry, OnClick, IsSaveOnly).Init();
                    ourListItem2.Info = "最新版" + ((Operators.CompareString(ourListItem2.Info, "", false) == 0) ? "" : ("，" + ourListItem2.Info));
                    Stack.Children.Add(ourListItem2);
                }
                Stack.Children.Add(new TextBlock
                {
                    Text = "全部版本 (" + Conversions.ToString(Entries.Count) + ")",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(6.0, 13.0, 0.0, 4.0)
                });
            }
        }

        public static OurVirtualizingElement<OurListItem> ForgeDownloadListItem(ModDownload.DlForgeVersionEntry Entry, OurListItem.ClickEventHandler OnClick, bool IsSaveOnly)
        {
            ModDownloadLib._Closure___32_0 CS_locals1 = new ModDownloadLib._Closure___32_0();
            CS_locals1._VB_Local_Entry = Entry;
            CS_locals1._VB_Local_OnClick = OnClick;
            CS_locals1._VB_Local_IsSaveOnly = IsSaveOnly;
            return new OurVirtualizingElement<OurListItem>(new Func<OurListItem>(CS_locals1._Lambda___0))
            {
                Height = 42.0
            };
        }

        private static void ForgeContMenuBuild(OurListItem sender, EventArgs e)
        {
            OurIconButton ourIconButton = new OurIconButton
            {
                Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z",
                ToolTip = "另存为"
            };
            ourIconButton.Click += ((ModDownloadLib._Closure___._IR33_16 == null) ? (ModDownloadLib._Closure___._IR33_16 = new OurIconButton.ClickEventHandler(ModDownloadLib._Closure___._I._Lambda___R33_16)) : ModDownloadLib._Closure___._IR33_16);
            OurIconButton ourIconButton2 = new OurIconButton
            {
                LogoScale = 1.05,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                ToolTip = "更新日志"
            };
            ourIconButton2.Click += ((ModDownloadLib._Closure___._IR33_17 == null) ? (ModDownloadLib._Closure___._IR33_17 = new OurIconButton.ClickEventHandler(ModDownloadLib._Closure___._I._Lambda___R33_17)) : ModDownloadLib._Closure___._IR33_17);
            sender.Buttons = new OurIconButton[] { ourIconButton, ourIconButton2 };
        }

        private static void ForgeSaveContMenuBuild(OurListItem sender, EventArgs e)
        {
            OurIconButton ourIconButton = new OurIconButton
            {
                LogoScale = 1.05,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                ToolTip = "更新日志"
            };
            ourIconButton.Click += ((ModDownloadLib._Closure___._IR34_18 == null) ? (ModDownloadLib._Closure___._IR34_18 = new OurIconButton.ClickEventHandler(ModDownloadLib._Closure___._I._Lambda___R34_18)) : ModDownloadLib._Closure___._IR34_18);
            sender.Buttons = new OurIconButton[] { ourIconButton };
        }

        private static void ForgeLog_Click(object sender, RoutedEventArgs e)
        {
            ModDownload.DlForgeVersionEntry dlForgeVersionEntry;
            if (NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null) != null)
            {
                dlForgeVersionEntry = (ModDownload.DlForgeVersionEntry)NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null);
            }
            else if (NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null) != null)
            {
                dlForgeVersionEntry = (ModDownload.DlForgeVersionEntry)NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            }
            else
            {
                dlForgeVersionEntry = (ModDownload.DlForgeVersionEntry)NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            }
            ModBase.OpenWebsite(string.Format("https://files.minecraftforge.net/maven/net/minecraftforge/forge/{0}-{1}/forge-{2}-{3}-changelog.txt", new object[] { dlForgeVersionEntry.Inherit, dlForgeVersionEntry.VersionName, dlForgeVersionEntry.Inherit, dlForgeVersionEntry.VersionName }));
        }

        public static void ForgeSave_Click(object sender, RoutedEventArgs e)
        {
            ModDownload.DlForgeVersionEntry dlForgeVersionEntry;
            if (NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null) != null)
            {
                dlForgeVersionEntry = (ModDownload.DlForgeVersionEntry)NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null);
            }
            else if (NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null) != null)
            {
                dlForgeVersionEntry = (ModDownload.DlForgeVersionEntry)NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            }
            else
            {
                dlForgeVersionEntry = (ModDownload.DlForgeVersionEntry)NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            }
            ModDownloadLib.McDownloadForgelikeSave(dlForgeVersionEntry);
        }

        public static void McDownloadForgeRecommendedRefresh()
        {
            if (!ModDownloadLib.IsForgeRecommendedRefreshed)
            {
                ModDownloadLib.IsForgeRecommendedRefreshed = true;
                ModBase.RunInNewThread((ModDownloadLib._Closure___._I37_0 == null) ? (ModDownloadLib._Closure___._I37_0 = new Action(ModDownloadLib._Closure___._I._Lambda___37_0)) : ModDownloadLib._Closure___._I37_0, "ForgeRecommendedRefresh", ThreadPriority.Normal);
            }
        }

        public static string McDownloadForgeRecommendedGet(string McVersionName)
        {
            string text;
            try
            {
                if (McVersionName == null)
                {
                    text = null;
                }
                else
                {
                    string text2 = FileUtils.TryReadAsString(ModBase.PathTemp + "Cache\\ForgeRecommendedList.json", null, null);
                    if (string.IsNullOrEmpty(text2))
                    {
                        Logger.Info("没有 Forge 推荐版本缓存文件", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        text = null;
                    }
                    else
                    {
                        JObject jobject = (JObject)text2.DeserializeJson();
                        if (jobject == null || !(McVersionName ?? "").Contains(".") || !jobject.ContainsKey(McVersionName))
                        {
                            text = null;
                        }
                        else
                        {
                            text = (jobject[McVersionName] ?? "").ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, string.Format("获取 Forge 推荐版本失败（{0}）", McVersionName ?? "null"), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                text = null;
            }
            return text;
        }

        public static void NeoForgeDownloadListItemPreload(StackPanel Stack, List<ModDownload.DlNeoForgeListEntry> Entries, OurListItem.ClickEventHandler OnClick, bool IsSaveOnly)
        {
            if (!Entries.IsSingle<ModDownload.DlNeoForgeListEntry>())
            {
                ModDownload.DlNeoForgeListEntry dlNeoForgeListEntry = null;
                ModDownload.DlNeoForgeListEntry dlNeoForgeListEntry2 = null;
                if (Entries.Any<ModDownload.DlNeoForgeListEntry>())
                {
                    try
                    {
                        foreach (ModDownload.DlNeoForgeListEntry dlNeoForgeListEntry3 in Entries.ToList<ModDownload.DlNeoForgeListEntry>())
                        {
                            if (!dlNeoForgeListEntry3.IsBeta)
                            {
                                dlNeoForgeListEntry = dlNeoForgeListEntry3;
                                break;
                            }
                            if (dlNeoForgeListEntry2 == null)
                            {
                                dlNeoForgeListEntry2 = dlNeoForgeListEntry3;
                            }
                        }
                        goto IL_0069;
                    }
                    finally
                    {
                        List<ModDownload.DlNeoForgeListEntry>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                }
                Logger.Warn("未找到可用的 NeoForge 版本", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                IL_0069:
                if (dlNeoForgeListEntry != null)
                {
                    OurListItem ourListItem = ModDownloadLib.NeoForgeDownloadListItem(dlNeoForgeListEntry, OnClick, IsSaveOnly).Init();
                    ourListItem.Info = ((Operators.CompareString(ourListItem.Info, "", false) == 0) ? "最新稳定版" : ("最新" + ourListItem.Info));
                    Stack.Children.Add(ourListItem);
                }
                if (dlNeoForgeListEntry2 != null)
                {
                    OurListItem ourListItem2 = ModDownloadLib.NeoForgeDownloadListItem(dlNeoForgeListEntry2, OnClick, IsSaveOnly).Init();
                    ourListItem2.Info = ((Operators.CompareString(ourListItem2.Info, "", false) == 0) ? "最新测试版" : ("最新" + ourListItem2.Info));
                    Stack.Children.Add(ourListItem2);
                }
                Stack.Children.Add(new TextBlock
                {
                    Text = "全部版本 (" + Conversions.ToString(Entries.Count) + ")",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(6.0, 13.0, 0.0, 4.0)
                });
            }
        }

        public static OurVirtualizingElement<OurListItem> NeoForgeDownloadListItem(ModDownload.DlNeoForgeListEntry Info, OurListItem.ClickEventHandler OnClick, bool IsSaveOnly)
        {
            ModDownloadLib._Closure___41_0 CS_locals1 = new ModDownloadLib._Closure___41_0();
            CS_locals1._VB_Local_Info = Info;
            CS_locals1._VB_Local_OnClick = OnClick;
            CS_locals1._VB_Local_IsSaveOnly = IsSaveOnly;
            return new OurVirtualizingElement<OurListItem>(new Func<OurListItem>(CS_locals1._Lambda___0))
            {
                Height = 42.0
            };
        }

        private static void NeoForgeContMenuBuild(OurListItem sender, EventArgs e)
        {
            OurIconButton ourIconButton = new OurIconButton
            {
                Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z",
                ToolTip = "另存为"
            };
            ourIconButton.Click += ((ModDownloadLib._Closure___._IR42_19 == null) ? (ModDownloadLib._Closure___._IR42_19 = new OurIconButton.ClickEventHandler(ModDownloadLib._Closure___._I._Lambda___R42_19)) : ModDownloadLib._Closure___._IR42_19);
            OurIconButton ourIconButton2 = new OurIconButton
            {
                LogoScale = 1.05,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                ToolTip = "更新日志"
            };
            ourIconButton2.Click += ((ModDownloadLib._Closure___._IR42_20 == null) ? (ModDownloadLib._Closure___._IR42_20 = new OurIconButton.ClickEventHandler(ModDownloadLib._Closure___._I._Lambda___R42_20)) : ModDownloadLib._Closure___._IR42_20);
            sender.Buttons = new OurIconButton[] { ourIconButton, ourIconButton2 };
        }

        private static void NeoForgeSaveContMenuBuild(OurListItem sender, EventArgs e)
        {
            OurIconButton ourIconButton = new OurIconButton
            {
                LogoScale = 1.05,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                ToolTip = "更新日志"
            };
            ourIconButton.Click += ((ModDownloadLib._Closure___._IR43_21 == null) ? (ModDownloadLib._Closure___._IR43_21 = new OurIconButton.ClickEventHandler(ModDownloadLib._Closure___._I._Lambda___R43_21)) : ModDownloadLib._Closure___._IR43_21);
            sender.Buttons = new OurIconButton[] { ourIconButton };
        }

        private static void NeoForgeLog_Click(object sender, RoutedEventArgs e)
        {
            ModDownload.DlNeoForgeListEntry dlNeoForgeListEntry;
            if (NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null) != null)
            {
                dlNeoForgeListEntry = (ModDownload.DlNeoForgeListEntry)NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null);
            }
            else if (NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null) != null)
            {
                dlNeoForgeListEntry = (ModDownload.DlNeoForgeListEntry)NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            }
            else
            {
                dlNeoForgeListEntry = (ModDownload.DlNeoForgeListEntry)NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            }
            ModBase.OpenWebsite(dlNeoForgeListEntry.UrlBase + "-changelog.txt");
        }

        public static void NeoForgeSave_Click(object sender, RoutedEventArgs e)
        {
            ModDownload.DlNeoForgeListEntry dlNeoForgeListEntry;
            if (NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null) != null)
            {
                dlNeoForgeListEntry = (ModDownload.DlNeoForgeListEntry)NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null);
            }
            else if (NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null) != null)
            {
                dlNeoForgeListEntry = (ModDownload.DlNeoForgeListEntry)NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            }
            else
            {
                dlNeoForgeListEntry = (ModDownload.DlNeoForgeListEntry)NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            }
            ModDownloadLib.McDownloadForgelikeSave(dlNeoForgeListEntry);
        }

        public static void McDownloadFabricLoaderSave(JObject DownloadInfo)
        {
            try
            {
                string text = DownloadInfo["url"].ToString();
                string lastPart = PathUtils.GetLastPart(text);
                string lastPart2 = PathUtils.GetLastPart(DownloadInfo["version"].ToString());
                string text2 = Dialogs.SaveFile("选择保存位置", lastPart, null, new ValueTuple<string, string>[]
                {
                    new ValueTuple<string, string>("jar", "Fabric 安装器")
                });
                if (text2 != null)
                {
                    try
                    {
                        List<ModLoader.LoaderBase>.Enumerator enumerator = ModLoader.LoaderTaskbar.ToList<ModLoader.LoaderBase>().GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (Operators.CompareString(enumerator.Current.Name, string.Format("Fabric {0} 安装器下载", lastPart2), false) == 0)
                            {
                                ModMain.Hint("该版本正在下载中！", ModMain.HintType.Red, true);
                                return;
                            }
                        }
                    }
                    finally
                    {
                        List<ModLoader.LoaderBase>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                    List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
                    List<string> list2 = new List<string>();
                    list2.Add(text);
                    list.Add(new ModNet.LoaderDownload("下载主文件", new List<ModNet.NetFile>
                    {
                        new ModNet.NetFile(list2.ToArray(), text2, new FileChecker
                        {
                            MinSize = 65536L
                        }, false)
                    })
                    {
                        ProgressWeight = 15.0
                    });
                    ModLoader.LoaderCombo<JObject> loaderCombo = new ModLoader.LoaderCombo<JObject>("Fabric " + lastPart2 + " 安装器下载", list);
                    loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.LoaderStateChangedHintOnly);
                    loaderCombo.Start(DownloadInfo, false);
                    ModLoader.LoaderTaskbarAdd<JObject>(loaderCombo);
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                    ModMain.FrmMain.BtnExtraDownload.Ribble();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "开始 Fabric 安装器下载失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }
        }

        private static List<ModLoader.LoaderBase> McDownloadFabricLoader(string FabricVersion, string MinecraftName, string McFolder)
        {
            ModDownloadLib._Closure___47_0 CS_locals1 = new ModDownloadLib._Closure___47_0();
            CS_locals1._VB_Local_FabricVersion = FabricVersion;
            CS_locals1._VB_Local_MinecraftName = MinecraftName;
            CS_locals1._VB_Local_Id = "fabric-loader-" + CS_locals1._VB_Local_FabricVersion + "-" + CS_locals1._VB_Local_MinecraftName;
            CS_locals1._VB_Local_VersionFolder = McFolder + "versions\\" + CS_locals1._VB_Local_Id + "\\";
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            CS_locals1._VB_Local_MinecraftName = CS_locals1._VB_Local_MinecraftName.Replace("∞", "infinite");
            list.Add(new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("获取 Fabric 主文件下载地址", new Action<ModLoader.LoaderTask<string, List<ModNet.NetFile>>>(CS_locals1._Lambda___0), null, ThreadPriority.Normal)
            {
                ProgressWeight = 0.5
            });
            list.Add(new ModNet.LoaderDownload("下载 Fabric 主文件", new List<ModNet.NetFile>())
            {
                ProgressWeight = 2.5
            });
            return list;
        }

        public static OurVirtualizingElement<OurListItem> FabricDownloadListItem(JObject Entry, OurListItem.ClickEventHandler OnClick)
        {
            ModDownloadLib._Closure___48_0 CS_locals1 = new ModDownloadLib._Closure___48_0();
            CS_locals1._VB_Local_Entry = Entry;
            CS_locals1._VB_Local_OnClick = OnClick;
            return new OurVirtualizingElement<OurListItem>(new Func<OurListItem>(CS_locals1._Lambda___0))
            {
                Height = 42.0
            };
        }

        public static OurVirtualizingElement<OurListItem> FabricApiDownloadListItem(ResourceVersion Entry, OurListItem.ClickEventHandler OnClick)
        {
            ModDownloadLib._Closure___49_0 CS_locals1 = new ModDownloadLib._Closure___49_0();
            CS_locals1._VB_Local_Entry = Entry;
            CS_locals1._VB_Local_OnClick = OnClick;
            return new OurVirtualizingElement<OurListItem>(new Func<OurListItem>(CS_locals1._Lambda___0))
            {
                Height = 42.0
            };
        }

        public static OurVirtualizingElement<OurListItem> OptiFabricDownloadListItem(ResourceVersion Entry, OurListItem.ClickEventHandler OnClick)
        {
            ModDownloadLib._Closure___50_0 CS_locals1 = new ModDownloadLib._Closure___50_0();
            CS_locals1._VB_Local_Entry = Entry;
            CS_locals1._VB_Local_OnClick = OnClick;
            return new OurVirtualizingElement<OurListItem>(new Func<OurListItem>(CS_locals1._Lambda___0))
            {
                Height = 42.0
            };
        }

        public static List<ModLoader.LoaderBase> McInstallLoadersGet(ModDownloadLib.McInstallRequest Request)
        {
            ModDownloadLib._Closure___52_0 CS_locals1 = new ModDownloadLib._Closure___52_0(null);
            CS_locals1._VB_Local_Request = Request;
            CS_locals1._VB_Local_TempMcFolder = ModMain.RequestTaskTempFolder(CS_locals1._VB_Local_Request.Components.OptiFineEntry != null || CS_locals1._VB_Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.Forge) || CS_locals1._VB_Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.NeoForge));
            JObject optiFineEntry = CS_locals1._VB_Local_Request.Components.OptiFineEntry;
            ModDownload.DlOptiFineListEntry dlOptiFineListEntry = ((optiFineEntry != null) ? optiFineEntry.ToObject<ModDownload.DlOptiFineListEntry>() : null);
            CS_locals1._VB_Local_OptiFineFolder = ((dlOptiFineListEntry == null) ? null : string.Format("{0}versions\\{1}", CS_locals1._VB_Local_TempMcFolder, dlOptiFineListEntry.InstanceName));
            CS_locals1._VB_Local_ForgeVersion = (CS_locals1._VB_Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.Forge) ? CS_locals1._VB_Local_Request.Components.ModLoaderVersions[ModLoaders.Forge] : null);
            CS_locals1._VB_Local_ForgeFolder = ((CS_locals1._VB_Local_ForgeVersion == null) ? null : string.Format("{0}versions\\forge-{1}", CS_locals1._VB_Local_TempMcFolder, CS_locals1._VB_Local_ForgeVersion));
            CS_locals1._VB_Local_NeoForgeVersion = (CS_locals1._VB_Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.NeoForge) ? CS_locals1._VB_Local_Request.Components.ModLoaderVersions[ModLoaders.NeoForge] : null);
            CS_locals1._VB_Local_NeoForgeFolder = ((CS_locals1._VB_Local_NeoForgeVersion == null) ? null : string.Format("{0}versions\\neoforge-{1}", CS_locals1._VB_Local_TempMcFolder, CS_locals1._VB_Local_NeoForgeVersion));
            string text = (CS_locals1._VB_Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.LiteLoader) ? CS_locals1._VB_Local_Request.Components.ModLoaderVersions[ModLoaders.LiteLoader] : null);
            CS_locals1._VB_Local_LiteLoaderFolder = ((text == null) ? null : string.Format("{0}versions\\{1}-LiteLoader", CS_locals1._VB_Local_TempMcFolder, CS_locals1._VB_Local_Request.Components.MinecraftVersion));
            string text2 = (CS_locals1._VB_Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.Fabric) ? CS_locals1._VB_Local_Request.Components.ModLoaderVersions[ModLoaders.Fabric] : null);
            CS_locals1._VB_Local_FabricFolder = ((text2 == null) ? null : string.Format("{0}versions\\fabric-loader-{1}-{2}", CS_locals1._VB_Local_TempMcFolder, text2, CS_locals1._VB_Local_Request.Components.MinecraftVersion));
            CS_locals1._VB_Local_Modable = CS_locals1._VB_Local_Request.Components.ModLoaderVersions.Any<KeyValuePair<ModLoaders, string>>();
            CS_locals1._VB_Local_ModsTempFolder = string.Format("{0}mods\\", CS_locals1._VB_Local_TempMcFolder);
            CS_locals1._VB_Local_OptiFineAsMod = dlOptiFineListEntry != null && CS_locals1._VB_Local_Modable;
            if (CS_locals1._VB_Local_OptiFineAsMod)
            {
                Logger.Info("OptiFine 将作为 Mod 进行下载", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                if (text != null)
                {
                    CS_locals1._VB_Local_OptiFineFolder = string.Format("{0}{1}\\", CS_locals1._VB_Local_ModsTempFolder, CS_locals1._VB_Local_Request.Components.MinecraftVersion);
                }
                else
                {
                    CS_locals1._VB_Local_OptiFineFolder = CS_locals1._VB_Local_ModsTempFolder;
                }
            }
            if (CS_locals1._VB_Local_OptiFineFolder != null)
            {
                Logger.Info(string.Format("OptiFine 缓存：{0}", CS_locals1._VB_Local_OptiFineFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }
            if (CS_locals1._VB_Local_ForgeFolder != null)
            {
                Logger.Info(string.Format("Forge 缓存：{0}", CS_locals1._VB_Local_ForgeFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }
            if (CS_locals1._VB_Local_NeoForgeFolder != null)
            {
                Logger.Info(string.Format("NeoForge 缓存：{0}", CS_locals1._VB_Local_NeoForgeFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }
            if (CS_locals1._VB_Local_FabricFolder != null)
            {
                Logger.Info(string.Format("Fabric 缓存：{0}", CS_locals1._VB_Local_FabricFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }
            if (CS_locals1._VB_Local_LiteLoaderFolder != null)
            {
                Logger.Info(string.Format("LiteLoader 缓存：{0}", CS_locals1._VB_Local_LiteLoaderFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }
            Logger.Info(string.Format("对应的原版版本：{0}", CS_locals1._VB_Local_Request.Components.MinecraftVersion), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            CS_locals1._VB_Local_VersionFolder = PathUtils.AddSlashSuffix(CS_locals1._VB_Local_Request.InstanceFolder);
            if (FileUtils.Exists(string.Format("{0}{1}.json", CS_locals1._VB_Local_VersionFolder, CS_locals1._VB_Local_Request.NewInstanceName)))
            {
                ModMain.Hint("版本 " + CS_locals1._VB_Local_Request.NewInstanceName + " 已经存在！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            CS_locals1._VB_Local_ManagedFileSources = new Dictionary<string, string>();
            list.Add(new ModLoader.LoaderTask<int, int>("添加忽略标识", new Action<ModLoader.LoaderTask<int, int>>(CS_locals1._Lambda___R22), null, ThreadPriority.Normal)
            {
                Show = false
            });
            if (CS_locals1._VB_Local_Request.Components.FabricApiVersion != null)
            {
                ModNet.NetFile netFile = CS_locals1._VB_Local_Request.FabricApi.ToNetFile(CS_locals1._VB_Local_ModsTempFolder, new ResourceVersion.DownloadReason?(ResourceVersion.DownloadReason.Dependency), CS_locals1._VB_Local_Request.Components.MinecraftVersion, ModLoaders.Fabric);
                CS_locals1._VB_Local_ManagedFileSources.Add("FabricApi", netFile.LocalPath);
                list.Add(new ModNet.LoaderDownload("下载 Fabric API", new List<ModNet.NetFile> { netFile })
                {
                    ProgressWeight = 3.0,
                    Show = true,
                    Block = false
                });
            }
            if (CS_locals1._VB_Local_Request.Components.OptiFabricVersion != null)
            {
                ModNet.NetFile netFile2 = CS_locals1._VB_Local_Request.OptiFabric.ToNetFile(CS_locals1._VB_Local_ModsTempFolder, new ResourceVersion.DownloadReason?(ResourceVersion.DownloadReason.Dependency), CS_locals1._VB_Local_Request.Components.MinecraftVersion, ModLoaders.Fabric);
                CS_locals1._VB_Local_ManagedFileSources.Add("OptiFabric", netFile2.LocalPath);
                list.Add(new ModNet.LoaderDownload("下载 OptiFabric", new List<ModNet.NetFile> { netFile2 })
                {
                    ProgressWeight = 3.0,
                    Show = true,
                    Block = false
                });
            }
            ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>("下载原版 " + CS_locals1._VB_Local_Request.Components.MinecraftVersion, ModDownloadLib.McDownloadClientLoader(CS_locals1._VB_Local_Request.Components.MinecraftVersion, CS_locals1._VB_Local_Request.MinecraftJson, CS_locals1._VB_Local_VersionFolder))
            {
                ProgressWeight = 39.0,
                Show = false,
                Block = (!CS_locals1._VB_Local_Modable && dlOptiFineListEntry == null)
            };
            list.Add(loaderCombo);
            if (dlOptiFineListEntry != null)
            {
                if (CS_locals1._VB_Local_OptiFineAsMod)
                {
                    CS_locals1._VB_Local_ManagedFileSources.Add("OptiFine", CS_locals1._VB_Local_OptiFineFolder + dlOptiFineListEntry.FileName);
                }
                list.Add(new ModLoader.LoaderCombo<string>("下载 OptiFine " + dlOptiFineListEntry.DisplayName, CS_locals1._VB_Local_OptiFineAsMod ? ModDownloadLib.McDownloadOptiFineSaveLoader(dlOptiFineListEntry, CS_locals1._VB_Local_OptiFineFolder + dlOptiFineListEntry.FileName) : ModDownloadLib.McDownloadOptiFineLoader(dlOptiFineListEntry, CS_locals1._VB_Local_TempMcFolder, loaderCombo, CS_locals1._VB_Local_VersionFolder))
                {
                    ProgressWeight = (double)(CS_locals1._VB_Local_Modable ? 16 : 24),
                    Show = false,
                    Block = !CS_locals1._VB_Local_Modable
                });
            }
            if (CS_locals1._VB_Local_ForgeVersion != null)
            {
                list.Add(new ModLoader.LoaderCombo<string>("下载 Forge " + CS_locals1._VB_Local_ForgeVersion, ModDownloadLib.McDownloadForgelikeLoader(false, CS_locals1._VB_Local_ForgeVersion, "forge-" + CS_locals1._VB_Local_ForgeVersion, CS_locals1._VB_Local_Request.Components.MinecraftVersion, CS_locals1._VB_Local_Request.ForgeEntry, CS_locals1._VB_Local_TempMcFolder, loaderCombo, CS_locals1._VB_Local_VersionFolder))
                {
                    ProgressWeight = 25.0,
                    Show = false,
                    Block = (text2 == null && CS_locals1._VB_Local_Request.LiteLoaderEntry == null && CS_locals1._VB_Local_Request.NeoForgeEntry == null)
                });
            }
            if (CS_locals1._VB_Local_NeoForgeVersion != null)
            {
                list.Add(new ModLoader.LoaderCombo<string>("下载 NeoForge " + CS_locals1._VB_Local_NeoForgeVersion, ModDownloadLib.McDownloadForgelikeLoader(true, CS_locals1._VB_Local_NeoForgeVersion, "neoforge-" + CS_locals1._VB_Local_NeoForgeVersion, CS_locals1._VB_Local_Request.Components.MinecraftVersion, CS_locals1._VB_Local_Request.NeoForgeEntry, CS_locals1._VB_Local_TempMcFolder, loaderCombo, CS_locals1._VB_Local_VersionFolder))
                {
                    ProgressWeight = 25.0,
                    Show = false,
                    Block = (CS_locals1._VB_Local_Request.ForgeEntry == null && text2 == null && CS_locals1._VB_Local_Request.LiteLoaderEntry == null)
                });
            }
            if (text != null)
            {
                list.Add(new ModLoader.LoaderCombo<string>("下载 LiteLoader " + CS_locals1._VB_Local_Request.Components.MinecraftVersion, ModDownloadLib.McDownloadLiteLoaderLoader(CS_locals1._VB_Local_Request.LiteLoaderEntry, CS_locals1._VB_Local_TempMcFolder))
                {
                    ProgressWeight = 1.0,
                    Show = false,
                    Block = (text2 == null)
                });
            }
            if (text2 != null)
            {
                list.Add(new ModLoader.LoaderCombo<string>("下载 Fabric " + text2, ModDownloadLib.McDownloadFabricLoader(text2, CS_locals1._VB_Local_Request.Components.MinecraftVersion, CS_locals1._VB_Local_TempMcFolder))
                {
                    ProgressWeight = 2.0,
                    Show = false,
                    Block = true
                });
            }
            CS_locals1._VB_Local_MinecraftFolder = ModMinecraft.McFolderSelected;
            CS_locals1._VB_Local_TargetFolder = null;
            list.Add(new ModLoader.LoaderTask<string, string>("安装游戏", new Action<ModLoader.LoaderTask<string, string>>(CS_locals1._Lambda___1), null, ThreadPriority.Normal)
            {
                ProgressWeight = 2.0,
                Block = true
            });
            list.Add(new ModLoader.LoaderCombo<string>("下载游戏支持库文件", new List<ModLoader.LoaderBase>
            {
                new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("分析游戏支持库文件（副加载器）", new Action<ModLoader.LoaderTask<string, List<ModNet.NetFile>>>(CS_locals1._Lambda___2), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 1.0,
                    Show = false
                },
                new ModNet.LoaderDownload("下载游戏支持库文件（副加载器）", new List<ModNet.NetFile>())
                {
                    ProgressWeight = 7.0,
                    Show = false
                }
            })
            {
                ProgressWeight = 8.0
            });
            list.Add(new ModLoader.LoaderTask<int, int>("记录安装清单", new Action<ModLoader.LoaderTask<int, int>>(CS_locals1._Lambda___R23), null, ThreadPriority.Normal)
            {
                Show = false
            });
            if (!CS_locals1._VB_Local_Request.IsIntermediateOutput)
            {
                list.Add(new ModLoader.LoaderTask<int, int>("删除忽略标识", new Action<ModLoader.LoaderTask<int, int>>(CS_locals1._Lambda___R24), null, ThreadPriority.Normal)
                {
                    Show = false
                });
            }
            return list;
        }

        private static void MergeJson(string VersionFolder, string OptiFineFolder, bool OptiFineAsMod, string ForgeFolder, string NeoForgeFolder, string FabricFolder, string LiteLoaderFolder)
        {
            List<ValueTuple<string, string>> list = new List<ValueTuple<string, string>>
            {
                new ValueTuple<string, string>("Minecraft", VersionFolder)
            };
            if (OptiFineFolder != null && !OptiFineAsMod)
            {
                list.Add(new ValueTuple<string, string>("OptiFine", OptiFineFolder));
            }
            if (ForgeFolder != null)
            {
                list.Add(new ValueTuple<string, string>("Forge", ForgeFolder));
            }
            if (NeoForgeFolder != null)
            {
                list.Add(new ValueTuple<string, string>("NeoForge", NeoForgeFolder));
            }
            if (LiteLoaderFolder != null)
            {
                list.Add(new ValueTuple<string, string>("LiteLoader", LiteLoaderFolder));
            }
            if (FabricFolder != null)
            {
                list.Add(new ValueTuple<string, string>("Fabric", FabricFolder));
            }
            Logger.Info(string.Format("开始进行版本合并，输出：{0}，", VersionFolder) + string.Join("，", list.Select<ValueTuple<string, string>, string>((ModDownloadLib._Closure___._I53_0 == null) ? (ModDownloadLib._Closure___._I53_0 = new Func<ValueTuple<string, string>, string>(ModDownloadLib._Closure___._I._Lambda___53_0)) : ModDownloadLib._Closure___._I53_0)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            List<ValueTuple<string, JObject>> list2 = new List<ValueTuple<string, JObject>>();
            try
            {
                foreach (ValueTuple<string, string> valueTuple in list)
                {
                    string text = Path.Combine(valueTuple.Item2, PathUtils.GetLastPart(valueTuple.Item2) + ".json");
                    string text2 = FileUtils.TryReadAsString(text, null, null) ?? "";
                    if (!text2.StartsWithF("{", false))
                    {
                        throw new Exception(string.Format("{0} json 有误，地址：{1}，前段内容：{2}", valueTuple.Item1, text, text2.Substring(0, Math.Min(text2.Length, 1000))));
                    }
                    list2.Add(new ValueTuple<string, JObject>(valueTuple.Item1, text2.DeserializeJson<JObject>()));
                }
            }
            finally
            {
                List<ValueTuple<string, string>>.Enumerator enumerator = default;
                ((IDisposable)enumerator).Dispose();
            }
            string text3 = list2.Select<ValueTuple<string, JObject>, string>((ModDownloadLib._Closure___._I53_1 == null) ? (ModDownloadLib._Closure___._I53_1 = new Func<ValueTuple<string, JObject>, string>(ModDownloadLib._Closure___._I._Lambda___53_1)) : ModDownloadLib._Closure___._I53_1).Join(" ");
            JObject item = list2[0].Item2;
            try
            {
                foreach (ValueTuple<string, JObject> valueTuple2 in list2.Skip<ValueTuple<string, JObject>>(1))
                {
                    valueTuple2.Item2.Remove("releaseTime");
                    valueTuple2.Item2.Remove("time");
                    item.Merge(valueTuple2.Item2);
                }
            }
            finally
            {
                IEnumerator<ValueTuple<string, JObject>> enumerator2 = default;
                if (enumerator2 != null)
                {
                    enumerator2.Dispose();
                }
            }
            List<string> list3 = new List<string>();
            try
            {
                foreach (string text4 in text3.Split(" ", true).Select<string, string>((ModDownloadLib._Closure___._I53_2 == null) ? (ModDownloadLib._Closure___._I53_2 = new Func<string, string>(ModDownloadLib._Closure___._I._Lambda___53_2)) : ModDownloadLib._Closure___._I53_2))
                {
                    if (!text4.StartsWithF("-", false) && list3.Any<string>())
                    {
                        string text5 = list3.Last<string>();
                        if (text5.StartsWithF("-", false) && !text5.Contains(" "))
                        {
                            list3[checked(list3.Count - 1)] = text5 + " " + text4;
                            continue;
                        }
                    }
                    list3.Add(text4);
                }
            }
            finally
            {
                IEnumerator<string> enumerator3 = default;
                if (enumerator3 != null)
                {
                    enumerator3.Dispose();
                }
            }
            if (list3.Any<string>())
            {
                item["minecraftArguments"] = list3.Distinct<string>().Join(" ");
            }
            item.Remove("_comment_");
            item.Remove("inheritsFrom");
            item.Remove("jar");
            string lastPart = PathUtils.GetLastPart(VersionFolder);
            item["id"] = lastPart;
            FileUtils.Write(Path.Combine(VersionFolder, lastPart + ".json"), item.ToString(), null);
            Logger.Info(string.Format("版本合并 {0} 完成", lastPart), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
        }

        public static bool McInstallStart(ModDownloadLib.McInstallRequest Request)
        {
            bool flag;
            try
            {
                ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>(Request.NewInstanceName + " 安装", ModDownloadLib.McInstallLoadersGet(Request));
                loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.McInstallState);
                loaderCombo.Start(Request.InstanceFolder, false);
                ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
                ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                ModMain.FrmMain.BtnExtraDownload.Ribble();
                flag = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "开始合并安装失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                flag = false;
            }
            return flag;
        }

        public static bool McModificationStart(ModDownloadLib.McInstallRequest Request, ModMinecraft.McInstance Instance)
        {
            ModDownloadLib._Closure___55_0 CS_locals1 = new ModDownloadLib._Closure___55_0(null);
            CS_locals1._VB_Local_Request = Request;
            CS_locals1._VB_Local_Instance = Instance;
            bool flag;
            try
            {
                ModDownloadLib._Closure___55_1 CS_locals2 = new ModDownloadLib._Closure___55_1(null);
                CS_locals2._VB_NonLocal__VB_Closure_2 = CS_locals1;
                CS_locals2._VB_Local_TargetFolder = CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_Instance.PathIndie;
                List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
                list.Add(new ModLoader.LoaderTask<int, int>("检查 Natives 文件占用", new Action<ModLoader.LoaderTask<int, int>>(CS_locals2._VB_NonLocal__VB_Closure_2._Lambda___R25), null, ThreadPriority.Normal)
                {
                    Show = false
                });
                CS_locals2._VB_Local_BuildFolder = ModMain.RequestTaskTempFolder(false) + CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_Instance.Name + "\\";
                ModDownloadLib.McInstallRequest mcInstallRequest = new ModDownloadLib.McInstallRequest
                {
                    NewInstanceName = CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_Instance.Name,
                    InstanceFolder = CS_locals2._VB_Local_BuildFolder,
                    Components = CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_Request.Components,
                    MinecraftJson = CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_Request.MinecraftJson,
                    ForgeEntry = CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_Request.ForgeEntry,
                    NeoForgeEntry = CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_Request.NeoForgeEntry,
                    FabricApi = CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_Request.FabricApi,
                    OptiFabric = CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_Request.OptiFabric,
                    LiteLoaderEntry = CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_Request.LiteLoaderEntry,
                    IsIntermediateOutput = true
                };
                list.AddRange(ModDownloadLib.McInstallLoadersGet(mcInstallRequest));
                CS_locals2._VB_Local_RenameFailed = false;
                list.Add(new ModLoader.LoaderTask<int, int>("修改版本", new Action<ModLoader.LoaderTask<int, int>>(CS_locals2._Lambda___1), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 1.0,
                    Block = true
                });
                CS_locals2._VB_Local_Loader = new ModLoader.LoaderCombo<string>(CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_Instance.Name + " 修改", list)
                {
                    OnStateChanged = new Action<ModLoader.LoaderBase>(CS_locals2._Lambda___3)
                };
                RedoableWorker redoableWorker = new RedoableWorker(new Action(CS_locals2._Lambda___4), "McModificationStart");
                if (ModMinecraft.McInstanceLock.TryAcquire(redoableWorker, CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_Instance.PathVersion))
                {
                    redoableWorker.Stopped += ((ModDownloadLib._Closure___._I55_5 == null) ? (ModDownloadLib._Closure___._I55_5 = new Action(ModDownloadLib._Closure___._I._Lambda___55_5)) : ModDownloadLib._Closure___._I55_5);
                    redoableWorker.Start(default(CancellationToken), null);
                    ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
                    ModLoader.LoaderTaskbarAdd<string>(CS_locals2._VB_Local_Loader);
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                    ModMain.FrmMain.BtnExtraDownload.Ribble();
                    flag = true;
                }
                else
                {
                    Logger.Error("版本被其他 Worker 占用，版本修改失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                    flag = false;
                }
            }
            catch (Exception ex)
            {
                ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
                Logger.Error(ex, "开始修改版本失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                flag = false;
            }
            return flag;
        }

        public static void McInstallCommit(ModMinecraft.McInstance Instance, McInstallManifest NewManifest, string BuildFolder, string TargetFolder)
        {
            ModDownloadLib._Closure___56_0 CS_locals1 = new ModDownloadLib._Closure___56_0(null);
            CS_locals1._VB_Local_TargetFolder = TargetFolder;
            McInstallManifest mcInstallManifest = Configs.McInstallManifest.Get(Instance.Config);
            IReadOnlyList<McInstallManifest.ManagedFile> readOnlyList = ((mcInstallManifest != null) ? mcInstallManifest.ManagedFiles : null) ?? new McInstallManifest.ManagedFile[0];
            IReadOnlyList<McInstallManifest.ManagedFile> managedFiles = NewManifest.ManagedFiles;
            List<string> list = new List<string>();
            List<ValueTuple<string, string>> list2 = new List<ValueTuple<string, string>>();
            try
            {
                foreach (McInstallManifest.ManagedFile managedFile in managedFiles)
                {
                    list2.Add(new ValueTuple<string, string>(Path.Combine(BuildFolder, managedFile.RelativePath), Path.Combine(CS_locals1._VB_Local_TargetFolder, managedFile.RelativePath)));
                }
            }
            finally
            {
                IEnumerator<McInstallManifest.ManagedFile> enumerator = default;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            HashSet<string> hashSet = new HashSet<string>(managedFiles.Select<McInstallManifest.ManagedFile, string>((ModDownloadLib._Closure___._I56_0 == null) ? (ModDownloadLib._Closure___._I56_0 = new Func<McInstallManifest.ManagedFile, string>(ModDownloadLib._Closure___._I._Lambda___56_0)) : ModDownloadLib._Closure___._I56_0));
            try
            {
                foreach (McInstallManifest.ManagedFile managedFile2 in readOnlyList)
                {
                    if (!hashSet.Contains(managedFile2.RelativePath))
                    {
                        list.Add(PathUtils.ForCompare(Path.Combine(CS_locals1._VB_Local_TargetFolder, managedFile2.RelativePath)));
                    }
                }
            }
            finally
            {
                IEnumerator<McInstallManifest.ManagedFile> enumerator2 = default;
                if (enumerator2 != null)
                {
                    enumerator2.Dispose();
                }
            }
            if (DirectoryUtils.Exists(BuildFolder + "resources\\"))
            {
                DirectoryUtils.Copy(BuildFolder + "resources\\", CS_locals1._VB_Local_TargetFolder + "resources\\");
            }
            string jsonPath = Instance.GetJsonPath();
            list.Add(jsonPath);
            list.Add(Path.ChangeExtension(jsonPath, ".jar"));
            list.Add(Instance.PathVersion + Instance.Name + ".jar");
            list = list.Distinct<string>(StringComparer.OrdinalIgnoreCase).ToList<string>();
            Instance.Config.Save();
            try
            {
                foreach (string text in list)
                {
                    FileUtils.Delete(text, true);
                }
            }
            finally
            {
                List<string>.Enumerator enumerator3 = default;
                ((IDisposable)enumerator3).Dispose();
            }
            FileUtils.Move(BuildFolder + Instance.Name + ".jar", Instance.PathVersion + Instance.Name + ".jar", false);
            FileUtils.Move(BuildFolder + Instance.Name + ".json", jsonPath, false);
            try
            {
                foreach (ValueTuple<string, string> valueTuple in list2)
                {
                    DirectoryUtils.Create(PathUtils.RemoveLastPart(valueTuple.Item2));
                    FileUtils.Copy(valueTuple.Item1, valueTuple.Item2);
                }
            }
            finally
            {
                List<ValueTuple<string, string>>.Enumerator enumerator4 = default;
                ((IDisposable)enumerator4).Dispose();
            }
            List<McInstallManifest.ManagedFile> list3 = managedFiles.Select<McInstallManifest.ManagedFile, McInstallManifest.ManagedFile>(new Func<McInstallManifest.ManagedFile, McInstallManifest.ManagedFile>(CS_locals1._Lambda___1)).ToList<McInstallManifest.ManagedFile>();
            Configs.McInstallManifest.Set(new McInstallManifest(NewManifest.SchemaVersion, NewManifest.Components, NewManifest.LastModifiedUtc, list3), Instance.Config);
            Instance.Config.Save();
        }

        public static void LoaderStateChangedHintOnly(ModLoader.LoaderBase Loader)
        {
            switch (Loader.State)
            {
            case ModBase.LoadState.Finished:
                ModMain.Hint(Loader.Name + "成功！", ModMain.HintType.Green, true);
                return;
            case ModBase.LoadState.Failed:
                ModMain.Hint(Loader.Name + "失败：" + Loader.Error.GetDisplay(false), ModMain.HintType.Red, true);
                return;
            case ModBase.LoadState.Canceled:
                ModMain.Hint(Loader.Name + "已取消！", ModMain.HintType.Blue, true);
                return;
            default:
                return;
            }
        }

        public static void McInstallState(ModLoader.LoaderBase Loader)
        {
            switch (Loader.State)
            {
            case ModBase.LoadState.Loading:
                return;
            case ModBase.LoadState.Finished:
                ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "InstanceCache", "");
                ModMain.Hint(Loader.Name + "成功！", ModMain.HintType.Green, true);
                break;
            case ModBase.LoadState.Failed:
                ModMain.OurMsgBox(Loader.Error.GetDisplay(true), Loader.Name + "失败", "确定", "", "", true, true, false, null, null, null);
                break;
            case ModBase.LoadState.Canceled:
                ModMain.Hint(Loader.Name + "已取消！", ModMain.HintType.Blue, true);
                break;
            }
            ModDownloadLib.McInstallFailedClearFolder(Loader);
            ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
        }

        public static void McInstallFailedClearFolder(object Loader)
        {
            ModDownloadLib._Closure___59_0 CS_locals1 = new ModDownloadLib._Closure___59_0();
            CS_locals1._VB_Local_Loader = Loader;
            if (!Operators.ConditionalCompareObjectNotEqual(NewLateBinding.LateGet(CS_locals1._VB_Local_Loader, null, "State", new object[0], null, null, null), ModBase.LoadState.Failed, false) || !Operators.ConditionalCompareObjectNotEqual(NewLateBinding.LateGet(CS_locals1._VB_Local_Loader, null, "State", new object[0], null, null, null), ModBase.LoadState.Canceled, false))
            {
                ModBase.RunInThread(new Action(CS_locals1._Lambda___0));
            }
        }

        private const string McDownloadClientLibName = "下载原版支持库文件";

        private const string McDownloadClientJsonName = "下载原版 json 文件";

        private static bool IsForgeRecommendedRefreshed = false;

        private static object InstallSyncLock = RuntimeHelpers.GetObjectValue(new object());

        private static object VanillaSyncLock = RuntimeHelpers.GetObjectValue(new object());

        public class McInstallRequest
        {
            public McInstallRequest()
            {
                this.MinecraftJson = null;
                this.ForgeEntry = null;
                this.NeoForgeEntry = null;
                this.FabricApi = null;
                this.OptiFabric = null;
                this.LiteLoaderEntry = null;
                this.IsIntermediateOutput = false;
            }

            public string NewInstanceName;

            public string InstanceFolder;

            public McComponents Components;

            public string MinecraftJson;

            public ModDownload.DlForgeVersionEntry ForgeEntry;

            public ModDownload.DlNeoForgeListEntry NeoForgeEntry;

            public ResourceVersion FabricApi;

            public ResourceVersion OptiFabric;

            public ModDownload.DlLiteLoaderListEntry LiteLoaderEntry;

            public bool IsIntermediateOutput;
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

            internal void _Lambda___R5_1(object a0, EventArgs a1)
            {
                ModDownloadLib.McDownloadMenuLog(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }

            internal void _Lambda___R5_2(object a0, EventArgs a1)
            {
                ModDownloadLib.McDownloadMenuSaveServer(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }

            internal void _Lambda___R6_3(object a0, EventArgs a1)
            {
                ModDownloadLib.McDownloadMenuSave(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }

            internal void _Lambda___R6_4(object a0, EventArgs a1)
            {
                ModDownloadLib.McDownloadMenuLog(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }

            internal void _Lambda___R6_5(object a0, EventArgs a1)
            {
                ModDownloadLib.McDownloadMenuSaveServer(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }

            internal bool _Lambda___13_2(ModLoader.LoaderBase l)
            {
                return Operators.CompareString(l.Name, "下载原版支持库文件", false) == 0 || Operators.CompareString(l.Name, "下载原版 json 文件", false) == 0;
            }

            internal bool _Lambda___13_3(ModLoader.LoaderBase l)
            {
                return l.State != ModBase.LoadState.Finished;
            }

            internal bool _Lambda___13_4(ModLoader.LoaderBase l)
            {
                return l.State != ModBase.LoadState.Finished;
            }

            internal void _Lambda___R16_8(object a0, EventArgs a1)
            {
                ModDownloadLib.OptiFineLog_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }

            internal void _Lambda___R17_9(object a0, EventArgs a1)
            {
                ModDownloadLib.OptiFineSave_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }

            internal void _Lambda___R17_10(object a0, EventArgs a1)
            {
                ModDownloadLib.OptiFineLog_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }

            internal void _Lambda___R23_11(object a0, EventArgs a1)
            {
                ModDownloadLib.LiteLoaderAll_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }

            internal void _Lambda___R24_12(object a0, EventArgs a1)
            {
                ModDownloadLib.LiteLoaderSave_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }

            internal void _Lambda___R24_13(object a0, EventArgs a1)
            {
                ModDownloadLib.LiteLoaderAll_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }

            internal bool _Lambda___28_2(string l)
            {
                return Operators.CompareString(l, "true", false) == 0;
            }

            internal bool _Lambda___30_4(ModLoader.LoaderBase l)
            {
                return Operators.CompareString(l.Name, "下载原版支持库文件", false) == 0 || Operators.CompareString(l.Name, "下载原版 json 文件", false) == 0;
            }

            internal bool _Lambda___30_5(ModLoader.LoaderBase l)
            {
                return l.State != ModBase.LoadState.Finished;
            }

            internal bool _Lambda___30_6(ModLoader.LoaderBase l)
            {
                return l.State != ModBase.LoadState.Finished;
            }

            internal DirectoryInfo _Lambda___30_9(string f)
            {
                return DirectoryUtils.GetInfo(f);
            }

            internal bool _Lambda___30_10(DirectoryInfo l)
            {
                return l.Name.ContainsIgnoreCase("forge") && l.EnumerateFiles().Any<FileInfo>();
            }

            internal string _Lambda___30_11(DirectoryInfo d)
            {
                return d.Name;
            }

            internal bool _Lambda___32_1(string d)
            {
                return Operators.CompareString(d, "", false) != 0;
            }

            internal void _Lambda___R33_16(object a0, EventArgs a1)
            {
                ModDownloadLib.ForgeSave_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }

            internal void _Lambda___R33_17(object a0, EventArgs a1)
            {
                ModDownloadLib.ForgeLog_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }

            internal void _Lambda___R34_18(object a0, EventArgs a1)
            {
                ModDownloadLib.ForgeLog_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }

            internal void _Lambda___37_0()
            {
                try
                {
                    Logger.Info("刷新 Forge 推荐版本缓存开始", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                    string text = ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/forge/promos", null, null, null, null, "*/*", null, true, null, false);
                    if (text.Length < 1000)
                    {
                        throw new Exception("获取的结果过短（" + text + "）");
                    }
                    JContainer jcontainer = (JContainer)text.DeserializeJson();
                    List<string> list = new List<string>();
                    try
                    {
                        foreach (JToken jtoken in ((IEnumerable<JToken>)jcontainer))
                        {
                            JObject jobject = (JObject)jtoken;
                            if (jobject["name"] != null && jobject["build"] != null)
                            {
                                string text2 = (string)jobject["name"];
                                if (text2.EndsWithF("-recommended", false))
                                {
                                    list.Add("\"" + text2.Replace("-recommended", "\":\"" + jobject["build"]["version"].ToString() + "\""));
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
                    if (list.Count < 5)
                    {
                        throw new Exception("获取的推荐版本数过少（" + text + "）");
                    }
                    string text3 = "{" + list.Join(",") + "}";
                    FileUtils.Write(ModBase.PathTemp + "Cache\\ForgeRecommendedList.json", text3, null);
                    Logger.Info("刷新 Forge 推荐版本缓存成功", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "刷新 Forge 推荐版本缓存失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
            }

            internal void _Lambda___R42_19(object a0, EventArgs a1)
            {
                ModDownloadLib.NeoForgeSave_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }

            internal void _Lambda___R42_20(object a0, EventArgs a1)
            {
                ModDownloadLib.NeoForgeLog_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }

            internal void _Lambda___R43_21(object a0, EventArgs a1)
            {
                ModDownloadLib.NeoForgeLog_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }

            internal string _Lambda___53_0(ValueTuple<string, string> Source)
            {
                return string.Format("{0}：{1}", Source.Item1, Source.Item2);
            }

            internal string _Lambda___53_1(ValueTuple<string, JObject> Profile)
            {
                JToken jtoken = Profile.Item2["minecraftArguments"];
                return ((jtoken != null) ? jtoken.ToString() : null) ?? " ";
            }

            internal string _Lambda___53_2(string Value)
            {
                return Value.Trim();
            }

            internal void _Lambda___55_5()
            {
                ModBase.RunInUi((ModDownloadLib._Closure___._I55_6 == null) ? (ModDownloadLib._Closure___._I55_6 = new Action(ModDownloadLib._Closure___._I._Lambda___55_6)) : ModDownloadLib._Closure___._I55_6, false);
            }

            internal void _Lambda___55_6()
            {
                ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
            }

            internal string _Lambda___56_0(McInstallManifest.ManagedFile File)
            {
                return File.RelativePath;
            }

            public static readonly ModDownloadLib._Closure___ _I = new ModDownloadLib._Closure___();

            public static OurIconButton.ClickEventHandler _IR5_1;

            public static OurIconButton.ClickEventHandler _IR5_2;

            public static OurIconButton.ClickEventHandler _IR6_3;

            public static OurIconButton.ClickEventHandler _IR6_4;

            public static OurIconButton.ClickEventHandler _IR6_5;

            public static Func<ModLoader.LoaderBase, bool> _I13_2;

            public static Func<ModLoader.LoaderBase, bool> _I13_3;

            public static Func<ModLoader.LoaderBase, bool> _I13_4;

            public static OurIconButton.ClickEventHandler _IR16_8;

            public static OurIconButton.ClickEventHandler _IR17_9;

            public static OurIconButton.ClickEventHandler _IR17_10;

            public static OurIconButton.ClickEventHandler _IR23_11;

            public static OurIconButton.ClickEventHandler _IR24_12;

            public static OurIconButton.ClickEventHandler _IR24_13;

            public static Func<string, bool> _I28_2;

            public static Func<ModLoader.LoaderBase, bool> _I30_4;

            public static Func<ModLoader.LoaderBase, bool> _I30_5;

            public static Func<ModLoader.LoaderBase, bool> _I30_6;

            public static Func<string, DirectoryInfo> _I30_9;

            public static Func<DirectoryInfo, bool> _I30_10;

            public static Func<DirectoryInfo, string> _I30_11;

            public static Func<string, bool> _I32_1;

            public static OurIconButton.ClickEventHandler _IR33_16;

            public static OurIconButton.ClickEventHandler _IR33_17;

            public static OurIconButton.ClickEventHandler _IR34_18;

            public static Action _I37_0;

            public static OurIconButton.ClickEventHandler _IR42_19;

            public static OurIconButton.ClickEventHandler _IR42_20;

            public static OurIconButton.ClickEventHandler _IR43_21;

            public static Func<ValueTuple<string, string>, string> _I53_0;

            public static Func<ValueTuple<string, JObject>, string> _I53_1;

            public static Func<string, string> _I53_2;

            public static Action _I55_6;

            public static Action _I55_5;

            public static Func<McInstallManifest.ManagedFile, string> _I56_0;
        }

        [CompilerGenerated]
        private sealed class _Closure___1_0
        {
            public _Closure___1_0()
            {
            }

            internal void _Lambda___0(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                string text = Conversions.ToString(ModDownload.DlClientListGet(this._VB_Local_Id));
                Task.Output = new List<ModNet.NetFile>
                {
                    new ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(text), this._VB_Local_VersionFolder + this._VB_Local_InstanceName + ".json", null, false)
                };
            }

            internal void _Lambda___1(ModLoader.LoaderTask<string, string> Task)
            {
                this._VB_Local_NewInstance = new ModMinecraft.McInstance(this._VB_Local_VersionFolder);
                this._VB_Local_NewInstance.Load();
            }

            internal void _Lambda___2(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                Logger.Info(string.Format("开始分析原版支持库文件：{0}", this._VB_Local_VersionFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                Task.Output = ModMinecraft.McLibNetFilesFromInstance(this._VB_Local_NewInstance);
            }

            internal void _Lambda___3(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                try
                {
                    ModNet.NetFile netFile = ModDownload.DlClientAssetIndexGet(this._VB_Local_NewInstance);
                    List<ModNet.NetFile> list;
                    if (netFile != null)
                    {
                        (list = new List<ModNet.NetFile>()).Add(netFile);
                    }
                    else
                    {
                        list = new List<ModNet.NetFile>();
                    }
                    Task.Output = list;
                }
                catch (Exception ex)
                {
                    throw new Exception("分析资源文件索引地址失败", ex);
                }
                try
                {
                    JObject jobject = (JObject)FileUtils.ReadAsJson(this._VB_Local_VersionFolder + this._VB_Local_InstanceName + ".json", null, null);
                    jobject["clientVersion"] = this._VB_Local_Id;
                    FileUtils.Write(string.Format("{0}{1}.json", this._VB_Local_VersionFolder, this._VB_Local_InstanceName), jobject.ToString(), null);
                }
                catch (Exception ex2)
                {
                    throw new Exception("添加客户端版本失败", ex2);
                }
            }

            internal void _Lambda___4(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                ModLoader.LoaderTask<string, List<ModNet.NetFile>> loaderTask = Task;
                ModMinecraft.McInstance mcInstance = this._VB_Local_NewInstance;
                bool flag = true;
                ModLoader.LoaderBase loaderBase = Task;
                List<ModNet.NetFile> list = ModMinecraft.McAssetsFixList(mcInstance, flag, ref loaderBase);
                Task = (ModLoader.LoaderTask<string, List<ModNet.NetFile>>)loaderBase;
                loaderTask.Output = list;
            }

            public string _VB_Local_Id;

            public string _VB_Local_VersionFolder;

            public string _VB_Local_InstanceName;

            public ModMinecraft.McInstance _VB_Local_NewInstance;
        }

        [CompilerGenerated]
        private sealed class _Closure___12_0
        {
            public _Closure___12_0(ModDownloadLib._Closure___12_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_outputWaitHandle = arg0._VB_Local_outputWaitHandle;
                }
            }

            internal void _Lambda___R6(object a0, DataReceivedEventArgs a1)
            {
                this._Lambda___0(RuntimeHelpers.GetObjectValue(a0), a1);
            }

            internal object _Lambda___0(object sender, DataReceivedEventArgs e)
            {
                try
                {
                    if (e.Data == null)
                    {
                        this._VB_Local_outputWaitHandle.Set();
                    }
                    else
                    {
                        this._VB_NonLocal__VB_Closure_3._VB_Local_LastResult = e.Data;
                        Logger.Trace(this._VB_NonLocal__VB_Closure_3._VB_Local_LastResult, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        this._VB_NonLocal__VB_Closure_3._VB_Local_TotalLength = checked(this._VB_NonLocal__VB_Closure_3._VB_Local_TotalLength + 1);
                        ModLoader.LoaderTask<List<ModNet.NetFile>, bool> _VB_Local_Task;
                        (_VB_Local_Task = this._VB_NonLocal__VB_Closure_3._VB_NonLocal__VB_Closure_2._VB_Local_Task).Progress = _VB_Local_Task.Progress + 9.999999999999999E-05;
                    }
                }
                catch (ObjectDisposedException ex)
                {
                }
                catch (Exception ex2)
                {
                    Logger.Warn(ex2, "读取 OptiFine 安装器信息失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                try
                {
                    if (this._VB_NonLocal__VB_Closure_3._VB_NonLocal__VB_Closure_2._VB_Local_Task.State == ModBase.LoadState.Canceled && !this._VB_NonLocal__VB_Closure_3._VB_Local_process.HasExited)
                    {
                        Logger.Info("由于任务取消，已中止 OptiFine 安装", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        this._VB_NonLocal__VB_Closure_3._VB_Local_process.Kill();
                    }
                }
                catch (Exception ex3)
                {
                }
                return null;
            }

            public AutoResetEvent _VB_Local_outputWaitHandle;

            public ModDownloadLib._Closure___12_1 _VB_NonLocal__VB_Closure_3;
        }

        [CompilerGenerated]
        private sealed class _Closure___12_1
        {
            public _Closure___12_1(ModDownloadLib._Closure___12_1 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_LastResult = arg0._VB_Local_LastResult;
                    this._VB_Local_TotalLength = arg0._VB_Local_TotalLength;
                    this._VB_Local_process = arg0._VB_Local_process;
                }
            }

            public string _VB_Local_LastResult;

            public int _VB_Local_TotalLength;

            public Process _VB_Local_process;

            public ModDownloadLib._Closure___12_2 _VB_NonLocal__VB_Closure_2;
        }

        [CompilerGenerated]
        private sealed class _Closure___12_2
        {
            public _Closure___12_2(ModDownloadLib._Closure___12_2 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Task = arg0._VB_Local_Task;
                }
            }

            public ModLoader.LoaderTask<List<ModNet.NetFile>, bool> _VB_Local_Task;
        }

        [CompilerGenerated]
        private sealed class _Closure___12_3
        {
            public _Closure___12_3(ModDownloadLib._Closure___12_3 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_errorWaitHandle = arg0._VB_Local_errorWaitHandle;
                }
            }

            internal void _Lambda___R7(object a0, DataReceivedEventArgs a1)
            {
                this._Lambda___1(RuntimeHelpers.GetObjectValue(a0), a1);
            }

            internal object _Lambda___1(object sender, DataReceivedEventArgs e)
            {
                try
                {
                    if (e.Data == null)
                    {
                        this._VB_Local_errorWaitHandle.Set();
                    }
                    else
                    {
                        this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_LastResult = e.Data;
                        Logger.Trace(this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_LastResult, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_TotalLength = checked(this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_TotalLength + 1);
                        ModLoader.LoaderTask<List<ModNet.NetFile>, bool> _VB_Local_Task;
                        (_VB_Local_Task = this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_NonLocal__VB_Closure_2._VB_Local_Task).Progress = _VB_Local_Task.Progress + 9.999999999999999E-05;
                    }
                }
                catch (ObjectDisposedException ex)
                {
                }
                catch (Exception ex2)
                {
                    Logger.Warn(ex2, "读取 OptiFine 安装器错误信息失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                try
                {
                    if (this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_NonLocal__VB_Closure_2._VB_Local_Task.State == ModBase.LoadState.Canceled && !this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.HasExited)
                    {
                        Logger.Info("由于任务取消，已中止 OptiFine 安装", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.Kill();
                    }
                }
                catch (Exception ex3)
                {
                }
                return null;
            }

            public AutoResetEvent _VB_Local_errorWaitHandle;

            public ModDownloadLib._Closure___12_0 _VB_NonLocal__VB_Closure_4;
        }

        [CompilerGenerated]
        private sealed class _Closure___13_0
        {
            public _Closure___13_0(ModDownloadLib._Closure___13_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_DownloadInfo = arg0._VB_Local_DownloadInfo;
                    this._VB_Local_Target = arg0._VB_Local_Target;
                    this._VB_Local_ClientDownloadLoader = arg0._VB_Local_ClientDownloadLoader;
                    this._VB_Local_ClientFolder = arg0._VB_Local_ClientFolder;
                    this._VB_Local_McFolder = arg0._VB_Local_McFolder;
                    this._VB_Local_VersionFolder = arg0._VB_Local_VersionFolder;
                    this._VB_Local_Id = arg0._VB_Local_Id;
                }
            }

            internal void _Lambda___0(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                List<string> list = new List<string>();
                string text = this._VB_Local_DownloadInfo.Inherit;
                if (Operators.CompareString(text, "1.8", false) == 0 || Operators.CompareString(text, "1.9", false) == 0)
                {
                    text += ".0";
                }
                if (this._VB_Local_DownloadInfo.IsPreview)
                {
                    list.Add("https://bmclapi2.bangbang93.com/optifine/" + text + "/HD_U_" + this._VB_Local_DownloadInfo.DisplayName.Replace(this._VB_Local_DownloadInfo.Inherit + " ", "").Replace(" ", "/"));
                }
                else
                {
                    list.Add("https://bmclapi2.bangbang93.com/optifine/" + text + "/HD_U/" + this._VB_Local_DownloadInfo.DisplayName.Replace(this._VB_Local_DownloadInfo.Inherit + " ", ""));
                }
                try
                {
                    string text2 = ModNet.NetRequestByClientRetry("https://optifine.net/adloadx?f=" + this._VB_Local_DownloadInfo.FileName, null, null, null, null, "text/html", null, false, new UTF8Encoding(false), true);
                    Task.Progress = 0.8;
                    list.Add("https://optifine.net/" + text2.RegexSearch("downloadx\\?f=[^\"']+", RegexOptions.None).First<string>());
                    Logger.Info(string.Format("OptiFine {0} 官方下载地址：{1}", this._VB_Local_DownloadInfo.DisplayName, list.Last<string>()), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, string.Format("获取 OptiFine {0} 官方下载地址失败", this._VB_Local_DownloadInfo.DisplayName), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                Task.Output = new List<ModNet.NetFile>
                {
                    new ModNet.NetFile(list.ToArray(), this._VB_Local_Target, new FileChecker
                    {
                        MinSize = 307200L
                    }, false)
                };
            }

            internal void _Lambda___1(ModLoader.LoaderTask<List<ModNet.NetFile>, bool> Task)
            {
                List<ModLoader.LoaderBase> list = this._VB_Local_ClientDownloadLoader.GetLoaderList(true).Where<ModLoader.LoaderBase>((ModDownloadLib._Closure___._I13_2 == null) ? (ModDownloadLib._Closure___._I13_2 = new Func<ModLoader.LoaderBase, bool>(ModDownloadLib._Closure___._I._Lambda___13_2)) : ModDownloadLib._Closure___._I13_2).Where<ModLoader.LoaderBase>((ModDownloadLib._Closure___._I13_3 == null) ? (ModDownloadLib._Closure___._I13_3 = new Func<ModLoader.LoaderBase, bool>(ModDownloadLib._Closure___._I._Lambda___13_3)) : ModDownloadLib._Closure___._I13_3)
                    .ToList<ModLoader.LoaderBase>();
                if (list.Any<ModLoader.LoaderBase>())
                {
                    Logger.Info("OptiFine 安装正在等待原版文件下载完成", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                while (list.Any<ModLoader.LoaderBase>() && !Task.IsCanceled)
                {
                    list = list.Where<ModLoader.LoaderBase>((ModDownloadLib._Closure___._I13_4 == null) ? (ModDownloadLib._Closure___._I13_4 = new Func<ModLoader.LoaderBase, bool>(ModDownloadLib._Closure___._I._Lambda___13_4)) : ModDownloadLib._Closure___._I13_4).ToList<ModLoader.LoaderBase>();
                    Thread.Sleep(50);
                }
                if (!Task.IsCanceled)
                {
                    object vanillaSyncLock = ModDownloadLib.VanillaSyncLock;
                    ObjectFlowControl.CheckForSyncLockOnValueType(vanillaSyncLock);
                    lock (vanillaSyncLock)
                    {
                        string lastPart = PathUtils.GetLastPart(this._VB_Local_ClientFolder);
                        DirectoryUtils.Create(this._VB_Local_McFolder + "versions\\" + this._VB_Local_DownloadInfo.Inherit);
                        if (!FileUtils.Exists(string.Concat(new string[]
                        {
                            this._VB_Local_McFolder,
                            "versions\\",
                            this._VB_Local_DownloadInfo.Inherit,
                            "\\",
                            this._VB_Local_DownloadInfo.Inherit,
                            ".json"
                        })))
                        {
                            FileUtils.Copy(string.Format("{0}{1}.json", this._VB_Local_ClientFolder, lastPart), string.Format("{0}versions\\{1}\\{2}.json", this._VB_Local_McFolder, this._VB_Local_DownloadInfo.Inherit, this._VB_Local_DownloadInfo.Inherit));
                        }
                        if (!FileUtils.Exists(string.Concat(new string[]
                        {
                            this._VB_Local_McFolder,
                            "versions\\",
                            this._VB_Local_DownloadInfo.Inherit,
                            "\\",
                            this._VB_Local_DownloadInfo.Inherit,
                            ".jar"
                        })))
                        {
                            FileUtils.Copy(string.Format("{0}{1}.jar", this._VB_Local_ClientFolder, lastPart), string.Format("{0}versions\\{1}\\{2}.jar", this._VB_Local_McFolder, this._VB_Local_DownloadInfo.Inherit, this._VB_Local_DownloadInfo.Inherit));
                        }
                    }
                }
            }

            internal void _Lambda___5(ModLoader.LoaderTask<List<ModNet.NetFile>, bool> Task)
            {
                string text = ModMain.RequestTaskTempFolder(true);
                string text2 = text + ".minecraft\\";
                try
                {
                    if (DirectoryUtils.Exists(text2 + "versions\\" + this._VB_Local_DownloadInfo.Inherit))
                    {
                        DirectoryUtils.Delete(text2 + "versions\\" + this._VB_Local_DownloadInfo.Inherit, false);
                    }
                    DirectoryUtils.Create(text2 + "versions\\" + this._VB_Local_DownloadInfo.Inherit + "\\");
                    ModMinecraft.McFolderLauncherProfilesJsonCreate(text2);
                    FileUtils.Copy(string.Concat(new string[]
                    {
                        this._VB_Local_McFolder,
                        "versions\\",
                        this._VB_Local_DownloadInfo.Inherit,
                        "\\",
                        this._VB_Local_DownloadInfo.Inherit,
                        ".json"
                    }), string.Concat(new string[]
                    {
                        text2,
                        "versions\\",
                        this._VB_Local_DownloadInfo.Inherit,
                        "\\",
                        this._VB_Local_DownloadInfo.Inherit,
                        ".json"
                    }));
                    FileUtils.Copy(string.Concat(new string[]
                    {
                        this._VB_Local_McFolder,
                        "versions\\",
                        this._VB_Local_DownloadInfo.Inherit,
                        "\\",
                        this._VB_Local_DownloadInfo.Inherit,
                        ".jar"
                    }), string.Concat(new string[]
                    {
                        text2,
                        "versions\\",
                        this._VB_Local_DownloadInfo.Inherit,
                        "\\",
                        this._VB_Local_DownloadInfo.Inherit,
                        ".jar"
                    }));
                    Task.Progress = 0.06;
                    bool flag = true;
                    for (;;)
                    {
                        try
                        {
                            ModDownloadLib.McDownloadOptiFineInstall(text, this._VB_Local_Target, Task, flag);
                        }
                        catch (Exception ex)
                        {
                            if (flag)
                            {
                                Logger.Warn(ex, "使用 JavaWrapper 安装 OptiFine 失败，将不使用 JavaWrapper 并重试", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                                flag = false;
                                continue;
                            }
                            throw new Exception("运行 OptiFine 安装器失败", ex);
                        }
                        break;
                    }
                    Task.Progress = 0.96;
                    FileUtils.Delete(text2 + "launcher_profiles.json", false);
                    DirectoryUtils.Copy(text2, this._VB_Local_McFolder);
                    Task.Progress = 0.98;
                    FileUtils.Delete(this._VB_Local_Target, false);
                    DirectoryUtils.Delete(text, false);
                }
                catch (Exception ex2)
                {
                    throw new Exception("安装 OptiFine（方式 A）失败", ex2);
                }
            }

            internal void _Lambda___6(ModLoader.LoaderTask<List<ModNet.NetFile>, bool> Task)
            {
                try
                {
                    FileUtils.Copy(string.Concat(new string[]
                    {
                        this._VB_Local_McFolder,
                        "versions\\",
                        this._VB_Local_DownloadInfo.Inherit,
                        "\\",
                        this._VB_Local_DownloadInfo.Inherit,
                        ".jar"
                    }), this._VB_Local_VersionFolder + this._VB_Local_Id + ".jar");
                    Task.Progress = 0.7;
                    ModMinecraft.McInstance mcInstance = new ModMinecraft.McInstance(this._VB_Local_McFolder + "versions\\" + this._VB_Local_DownloadInfo.Inherit);
                    string text = string.Concat(new string[]
                    {
                        "{\r\n    \"id\": \"",
                        this._VB_Local_Id,
                        "\",\r\n    \"inheritsFrom\": \"",
                        this._VB_Local_DownloadInfo.Inherit,
                        "\",\r\n    \"time\": \"",
                        (Operators.CompareString(this._VB_Local_DownloadInfo.ReleaseTime, "", false) == 0) ? mcInstance.ReleaseTime.ToString("yyyy'-'MM'-'dd") : this._VB_Local_DownloadInfo.ReleaseTime.Replace("/", "-"),
                        "T23:33:33+08:00\",\r\n    \"releaseTime\": \"",
                        (Operators.CompareString(this._VB_Local_DownloadInfo.ReleaseTime, "", false) == 0) ? mcInstance.ReleaseTime.ToString("yyyy'-'MM'-'dd") : this._VB_Local_DownloadInfo.ReleaseTime.Replace("/", "-"),
                        "T23:33:33+08:00\",\r\n    \"type\": \"release\",\r\n    \"libraries\": [\r\n        {\"name\": \"optifine:OptiFine:",
                        PathUtils.GetFileNameWithoutExtension(this._VB_Local_DownloadInfo.FileName).Replace("OptiFine_", "").Replace("preview_", ""),
                        "\"},\r\n        {\"name\": \"net.minecraft:launchwrapper:1.12\"}\r\n    ],\r\n    \"mainClass\": \"net.minecraft.launchwrapper.Launch\","
                    });
                    Task.Progress = 0.8;
                    if (mcInstance.IsOldJson)
                    {
                        text = text + "\r\n    \"minimumLauncherVersion\": 18,\r\n    \"minecraftArguments\": \"" + mcInstance.JsonObject["minecraftArguments"].ToString() + "  --tweakClass optifine.OptiFineTweaker\"\r\n}";
                    }
                    else
                    {
                        text += "\r\n    \"minimumLauncherVersion\": \"21\",\r\n    \"arguments\": {\r\n        \"game\": [\r\n            \"--tweakClass\",\r\n            \"optifine.OptiFineTweaker\"\r\n        ]\r\n    }\r\n}";
                    }
                    FileUtils.Write(this._VB_Local_VersionFolder + this._VB_Local_Id + ".json", text, null);
                }
                catch (Exception ex)
                {
                    throw new Exception("安装 OptiFine（方式 B）失败", ex);
                }
            }

            public ModDownload.DlOptiFineListEntry _VB_Local_DownloadInfo;

            public string _VB_Local_Target;

            public ModLoader.LoaderCombo<string> _VB_Local_ClientDownloadLoader;

            public string _VB_Local_ClientFolder;

            public string _VB_Local_McFolder;

            public string _VB_Local_VersionFolder;

            public string _VB_Local_Id;
        }

        [CompilerGenerated]
        private sealed class _Closure___14_0
        {
            public _Closure___14_0()
            {
            }

            internal void _Lambda___0(ModLoader.LoaderTask<ModDownload.DlOptiFineListEntry, List<ModNet.NetFile>> Task)
            {
                List<string> list = new List<string>();
                string text = this._VB_Local_DownloadInfo.Inherit;
                if (Operators.CompareString(text, "1.8", false) == 0 || Operators.CompareString(text, "1.9", false) == 0)
                {
                    text += ".0";
                }
                if (this._VB_Local_DownloadInfo.IsPreview)
                {
                    list.Add("https://bmclapi2.bangbang93.com/optifine/" + text + "/HD_U_" + this._VB_Local_DownloadInfo.DisplayName.Replace(this._VB_Local_DownloadInfo.Inherit + " ", "").Replace(" ", "/"));
                }
                else
                {
                    list.Add("https://bmclapi2.bangbang93.com/optifine/" + text + "/HD_U/" + this._VB_Local_DownloadInfo.DisplayName.Replace(this._VB_Local_DownloadInfo.Inherit + " ", ""));
                }
                try
                {
                    string text2 = ModNet.NetRequestByClientRetry("https://optifine.net/adloadx?f=" + this._VB_Local_DownloadInfo.FileName, null, null, null, null, "text/html", null, false, new UTF8Encoding(false), true);
                    Task.Progress = 0.8;
                    list.Add("https://optifine.net/" + text2.RegexSearch("downloadx\\?f=[^\"']+", RegexOptions.None).First<string>());
                    Logger.Info(string.Format("OptiFine {0} 官方下载地址：{1}", this._VB_Local_DownloadInfo.DisplayName, list.Last<string>()), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, string.Format("获取 OptiFine {0} 官方下载地址失败", this._VB_Local_DownloadInfo.DisplayName), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                Task.Progress = 0.9;
                Task.Output = new List<ModNet.NetFile>
                {
                    new ModNet.NetFile(list.ToArray(), this._VB_Local_TargetFolder, new FileChecker
                    {
                        MinSize = 65536L
                    }, false)
                };
            }

            public ModDownload.DlOptiFineListEntry _VB_Local_DownloadInfo;

            public string _VB_Local_TargetFolder;
        }

        [CompilerGenerated]
        private sealed class _Closure___15_0
        {
            public _Closure___15_0()
            {
            }

            internal OurListItem _Lambda___0()
            {
                OurListItem ourListItem = new OurListItem
                {
                    Title = this._VB_Local_Entry.DisplayName,
                    SnapsToDevicePixels = true,
                    Height = 42.0,
                    Type = OurListItem.CheckType.Clickable,
                    Tag = this._VB_Local_Entry,
                    Info = (this._VB_Local_Entry.IsPreview ? "测试版" : "正式版") + ((Operators.CompareString(this._VB_Local_Entry.ReleaseTime, "", false) == 0) ? "" : ("，发布于 " + this._VB_Local_Entry.ReleaseTime)) + ((this._VB_Local_Entry.RequiredForgeVersion == null) ? "，不兼容 Forge" : ((Operators.CompareString(this._VB_Local_Entry.RequiredForgeVersion, "", false) == 0) ? "" : ("，兼容 Forge " + this._VB_Local_Entry.RequiredForgeVersion))),
                    Logo = ModBase.PathImage + "Blocks/GrassPath.png"
                };
                ourListItem.Click += this._VB_Local_OnClick;
                if (this._VB_Local_IsSaveOnly)
                {
                    ourListItem.ContentHandler = new Action<OurListItem, EventArgs>(ModDownloadLib.OptiFineSaveContMenuBuild);
                }
                else
                {
                    ourListItem.ContentHandler = new Action<OurListItem, EventArgs>(ModDownloadLib.OptiFineContMenuBuild);
                }
                return ourListItem;
            }

            public ModDownload.DlOptiFineListEntry _VB_Local_Entry;

            public OurListItem.ClickEventHandler _VB_Local_OnClick;

            public bool _VB_Local_IsSaveOnly;
        }

        [CompilerGenerated]
        private sealed class _Closure___21_0
        {
            public _Closure___21_0()
            {
            }

            internal void _Lambda___0(ModLoader.LoaderTask<string, string> Task)
            {
                try
                {
                    string text = this._VB_Local_DownloadInfo.Inherit + "-LiteLoader";
                    string text2 = this._VB_Local_McFolder + "versions\\" + text + "\\";
                    JObject jobject = new JObject();
                    jobject.Add("id", text);
                    jobject.Add("time", DateTime.ParseExact(this._VB_Local_DownloadInfo.ReleaseTime, "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture));
                    jobject.Add("releaseTime", DateTime.ParseExact(this._VB_Local_DownloadInfo.ReleaseTime, "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture));
                    jobject.Add("type", "release");
                    jobject.Add("arguments", ("{\"game\":[\"--tweakClass\",\"" + this._VB_Local_DownloadInfo.JsonToken["tweakClass"].ToString() + "\"]}").DeserializeJson());
                    jobject.Add("libraries", this._VB_Local_DownloadInfo.JsonToken["libraries"]);
                    ((JContainer)jobject["libraries"]).Add(("{\"name\": \"com.mumfrey:liteloader:" + this._VB_Local_DownloadInfo.JsonToken["version"].ToString() + "\",\"url\": \"https://dl.liteloader.com/versions/\"}").DeserializeJson());
                    jobject.Add("mainClass", "net.minecraft.launchwrapper.Launch");
                    jobject.Add("minimumLauncherVersion", 18);
                    jobject.Add("inheritsFrom", this._VB_Local_DownloadInfo.Inherit);
                    jobject.Add("jar", this._VB_Local_DownloadInfo.Inherit);
                    FileUtils.Write(text2 + text + ".json", jobject.ToString(), null);
                }
                catch (Exception ex)
                {
                    throw new Exception("安装新 LiteLoader 版本失败", ex);
                }
            }

            public ModDownload.DlLiteLoaderListEntry _VB_Local_DownloadInfo;

            public string _VB_Local_McFolder;
        }

        [CompilerGenerated]
        private sealed class _Closure___22_0
        {
            public _Closure___22_0()
            {
            }

            internal OurListItem _Lambda___0()
            {
                OurListItem ourListItem = new OurListItem
                {
                    Title = this._VB_Local_Entry.Inherit,
                    SnapsToDevicePixels = true,
                    Height = 42.0,
                    Type = OurListItem.CheckType.Clickable,
                    Tag = this._VB_Local_Entry,
                    Info = (this._VB_Local_Entry.IsPreview ? "测试版" : "稳定版") + ((Operators.CompareString(this._VB_Local_Entry.ReleaseTime, "", false) == 0) ? "" : ("，发布于 " + this._VB_Local_Entry.ReleaseTime)),
                    Logo = ModBase.PathImage + "Blocks/Egg.png"
                };
                ourListItem.Click += this._VB_Local_OnClick;
                if (this._VB_Local_IsSaveOnly)
                {
                    ourListItem.ContentHandler = new Action<OurListItem, EventArgs>(ModDownloadLib.LiteLoaderSaveContMenuBuild);
                }
                else
                {
                    ourListItem.ContentHandler = new Action<OurListItem, EventArgs>(ModDownloadLib.LiteLoaderContMenuBuild);
                }
                return ourListItem;
            }

            public ModDownload.DlLiteLoaderListEntry _VB_Local_Entry;

            public OurListItem.ClickEventHandler _VB_Local_OnClick;

            public bool _VB_Local_IsSaveOnly;
        }

        [CompilerGenerated]
        private sealed class _Closure___28_0
        {
            public _Closure___28_0(ModDownloadLib._Closure___28_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_outputWaitHandle = arg0._VB_Local_outputWaitHandle;
                }
            }

            internal void _Lambda___R14(object a0, DataReceivedEventArgs a1)
            {
                this._Lambda___0(RuntimeHelpers.GetObjectValue(a0), a1);
            }

            internal object _Lambda___0(object sender, DataReceivedEventArgs e)
            {
                try
                {
                    if (e.Data == null)
                    {
                        this._VB_Local_outputWaitHandle.Set();
                    }
                    else
                    {
                        this._VB_NonLocal__VB_Closure_3._VB_Local_LastResults.Enqueue(e.Data);
                        if (this._VB_NonLocal__VB_Closure_3._VB_Local_LastResults.Count > 100)
                        {
                            this._VB_NonLocal__VB_Closure_3._VB_Local_LastResults.Dequeue();
                        }
                        ModDownloadLib.ForgelikeInjectorLine(e.Data, this._VB_NonLocal__VB_Closure_3._VB_NonLocal__VB_Closure_2._VB_Local_Task);
                    }
                }
                catch (ObjectDisposedException ex)
                {
                }
                catch (Exception ex2)
                {
                    Logger.Warn(ex2, string.Format("读取 {0} 安装器信息失败", this._VB_NonLocal__VB_Closure_3._VB_Local_LoaderName), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                try
                {
                    if (this._VB_NonLocal__VB_Closure_3._VB_NonLocal__VB_Closure_2._VB_Local_Task.State == ModBase.LoadState.Canceled && !this._VB_NonLocal__VB_Closure_3._VB_Local_process.HasExited)
                    {
                        Logger.Info(string.Format("由于任务取消，已中止 {0} 安装", this._VB_NonLocal__VB_Closure_3._VB_Local_LoaderName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        this._VB_NonLocal__VB_Closure_3._VB_Local_process.Kill();
                    }
                }
                catch (Exception ex3)
                {
                }
                return null;
            }

            public AutoResetEvent _VB_Local_outputWaitHandle;

            public ModDownloadLib._Closure___28_1 _VB_NonLocal__VB_Closure_3;
        }

        [CompilerGenerated]
        private sealed class _Closure___28_1
        {
            public _Closure___28_1(ModDownloadLib._Closure___28_1 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_LastResults = arg0._VB_Local_LastResults;
                    this._VB_Local_LoaderName = arg0._VB_Local_LoaderName;
                    this._VB_Local_process = arg0._VB_Local_process;
                }
            }

            public Queue<string> _VB_Local_LastResults;

            public string _VB_Local_LoaderName;

            public Process _VB_Local_process;

            public ModDownloadLib._Closure___28_2 _VB_NonLocal__VB_Closure_2;
        }

        [CompilerGenerated]
        private sealed class _Closure___28_2
        {
            public _Closure___28_2(ModDownloadLib._Closure___28_2 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Task = arg0._VB_Local_Task;
                }
            }

            public ModLoader.LoaderTask<bool, bool> _VB_Local_Task;
        }

        [CompilerGenerated]
        private sealed class _Closure___28_3
        {
            public _Closure___28_3(ModDownloadLib._Closure___28_3 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_errorWaitHandle = arg0._VB_Local_errorWaitHandle;
                }
            }

            internal void _Lambda___R15(object a0, DataReceivedEventArgs a1)
            {
                this._Lambda___1(RuntimeHelpers.GetObjectValue(a0), a1);
            }

            internal object _Lambda___1(object sender, DataReceivedEventArgs e)
            {
                try
                {
                    if (e.Data == null)
                    {
                        this._VB_Local_errorWaitHandle.Set();
                    }
                    else
                    {
                        this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_LastResults.Enqueue(e.Data);
                        if (this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_LastResults.Count > 100)
                        {
                            this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_LastResults.Dequeue();
                        }
                        ModDownloadLib.ForgelikeInjectorLine(e.Data, this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_NonLocal__VB_Closure_2._VB_Local_Task);
                    }
                }
                catch (ObjectDisposedException ex)
                {
                }
                catch (Exception ex2)
                {
                    Logger.Warn(ex2, string.Format("读取 {0} 安装器错误信息失败", this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_LoaderName), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                try
                {
                    if (this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_NonLocal__VB_Closure_2._VB_Local_Task.State == ModBase.LoadState.Canceled && !this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.HasExited)
                    {
                        Logger.Info(string.Format("由于任务取消，已中止 {0} 安装", this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_LoaderName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_3._VB_Local_process.Kill();
                    }
                }
                catch (Exception ex3)
                {
                }
                return null;
            }

            public AutoResetEvent _VB_Local_errorWaitHandle;

            public ModDownloadLib._Closure___28_0 _VB_NonLocal__VB_Closure_4;
        }

        [CompilerGenerated]
        private sealed class _Closure___30_0
        {
            public _Closure___30_0(ModDownloadLib._Closure___30_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Inherit = arg0._VB_Local_Inherit;
                    this._VB_Local_LoaderVersion = arg0._VB_Local_LoaderVersion;
                    this._VB_Local_Info = arg0._VB_Local_Info;
                    this._VB_Local_LoaderName = arg0._VB_Local_LoaderName;
                    this._VB_Local_InstallerAddress = arg0._VB_Local_InstallerAddress;
                    this._VB_Local_CurrentMcFolder = arg0._VB_Local_CurrentMcFolder;
                    this._VB_Local_IsNeoForge = arg0._VB_Local_IsNeoForge;
                    this._VB_Local_McFolder = arg0._VB_Local_McFolder;
                    this._VB_Local_ClientDownloadLoader = arg0._VB_Local_ClientDownloadLoader;
                    this._VB_Local_ClientFolder = arg0._VB_Local_ClientFolder;
                    this._VB_Local_VersionFolder = arg0._VB_Local_VersionFolder;
                    this._VB_Local_NewInstanceName = arg0._VB_Local_NewInstanceName;
                }
            }

            internal void _Lambda___0(ModLoader.LoaderTask<string, string> Task)
            {
                ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>> loaderTask = new ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>("McDownloadForgeLoader " + this._VB_Local_Inherit, new Action<ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>>(ModDownload.DlForgeVersionMain), null, ThreadPriority.Normal);
                loaderTask.WaitForExit(this._VB_Local_Inherit, null, false, default(CancellationToken), null);
                Task.Progress = 0.8;
                try
                {
                    foreach (ModDownload.DlForgeVersionEntry dlForgeVersionEntry in loaderTask.Output)
                    {
                        if (McUtils.CompareVersion(dlForgeVersionEntry.Version.ToString(), this._VB_Local_LoaderVersion) == 0)
                        {
                            this._VB_Local_Info = dlForgeVersionEntry;
                            return;
                        }
                    }
                }
                finally
                {
                    List<ModDownload.DlForgeVersionEntry>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
                throw new Exception(string.Concat(new string[]
                {
                    string.Format("未能找到 {0} ", this._VB_Local_LoaderName),
                    this._VB_Local_Inherit,
                    "-",
                    this._VB_Local_LoaderVersion,
                    " 的详细信息！"
                }));
            }

            internal void _Lambda___1(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                List<ModNet.NetFile> list = new List<ModNet.NetFile>();
                if (this._VB_Local_Info.IsNeoForge)
                {
                    string text = ((ModDownload.DlNeoForgeListEntry)this._VB_Local_Info).UrlBase + "-installer.jar";
                    list.Add(new ModNet.NetFile(new string[]
                    {
                        text.Replace("maven.neoforged.net/releases", "bmclapi2.bangbang93.com/maven"),
                        text
                    }, this._VB_Local_InstallerAddress, new FileChecker
                    {
                        MinSize = 65536L
                    }, false));
                }
                else
                {
                    ModDownload.DlForgeVersionEntry dlForgeVersionEntry = (ModDownload.DlForgeVersionEntry)this._VB_Local_Info;
                    string text2 = string.Format("{0}-{1}/forge-{2}-{3}-{4}.{5}", new object[]
                    {
                        dlForgeVersionEntry.Inherit.Replace("-", "_"),
                        dlForgeVersionEntry.FileVersion,
                        dlForgeVersionEntry.Inherit.Replace("-", "_"),
                        dlForgeVersionEntry.FileVersion,
                        dlForgeVersionEntry.Category,
                        dlForgeVersionEntry.FileExtension
                    });
                    list.Add(new ModNet.NetFile(new string[]
                    {
                        string.Format("https://bmclapi2.bangbang93.com/maven/net/minecraftforge/forge/{0}", text2),
                        string.Format("https://files.minecraftforge.net/maven/net/minecraftforge/forge/{0}", text2)
                    }, this._VB_Local_InstallerAddress, new FileChecker
                    {
                        MinSize = 65536L,
                        Hash = dlForgeVersionEntry.Hash
                    }, false));
                }
                Task.Output = list;
            }

            internal void _Lambda___7(ModLoader.LoaderTask<bool, bool> Task)
            {
                try
                {
                    ModDownloadLib._Closure___30_2 CS_locals1 = new ModDownloadLib._Closure___30_2(null);
                    Logger.Info(string.Format("开始进行 Forgelike 安装：{0}", this._VB_Local_InstallerAddress), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                    CS_locals1._VB_Local_OldList = DirectoryUtils.EnumerateDirectories(this._VB_Local_McFolder + "versions\\", false, "*").ToList<string>();
                    DirectoryUtils.Create(this._VB_Local_VersionFolder);
                    Task.Progress = 0.04;
                    ModMinecraft.McFolderLauncherProfilesJsonCreate(this._VB_Local_McFolder);
                    Task.Progress = 0.05;
                    bool flag = true;
                    for (;;)
                    {
                        try
                        {
                            ModBase.ExtractResources(ModBase.PathTemp + "Cache\\forge_installer.jar", "ForgeInstaller");
                            Task.Progress = 0.06;
                            ModDownloadLib.ForgelikeInjector(this._VB_Local_InstallerAddress, Task, this._VB_Local_McFolder, flag, this._VB_Local_IsNeoForge);
                            Task.Progress = 0.97;
                        }
                        catch (Exception ex)
                        {
                            if (flag)
                            {
                                Logger.Warn(ex, string.Format("使用 JavaWrapper 安装 {0} 失败，将不使用 JavaWrapper 并重试", this._VB_Local_LoaderName), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                                flag = false;
                                continue;
                            }
                            throw new Exception(string.Format("运行 {0} 安装器失败", this._VB_Local_LoaderName), ex);
                        }
                        break;
                    }
                    List<DirectoryInfo> list = DirectoryUtils.EnumerateDirectories(this._VB_Local_McFolder + "versions\\", false, "*").SkipWhile<string>(new Func<string, bool>(CS_locals1._Lambda___8)).Select<string, DirectoryInfo>((ModDownloadLib._Closure___._I30_9 == null) ? (ModDownloadLib._Closure___._I30_9 = new Func<string, DirectoryInfo>(ModDownloadLib._Closure___._I._Lambda___30_9)) : ModDownloadLib._Closure___._I30_9)
                        .ToList<DirectoryInfo>();
                    if (list.Count > 1)
                    {
                        list = list.Where<DirectoryInfo>((ModDownloadLib._Closure___._I30_10 == null) ? (ModDownloadLib._Closure___._I30_10 = new Func<DirectoryInfo, bool>(ModDownloadLib._Closure___._I._Lambda___30_10)) : ModDownloadLib._Closure___._I30_10).ToList<DirectoryInfo>();
                    }
                    if (list.Count == 1)
                    {
                        FileInfo fileInfo = list[0].EnumerateFiles().First<FileInfo>();
                        FileUtils.Copy(fileInfo.FullName, this._VB_Local_VersionFolder + this._VB_Local_NewInstanceName + ".json");
                        Logger.Info(string.Format("已复制新增的版本 JSON 文件：{0} -> {1}{2}.json", fileInfo.FullName, this._VB_Local_VersionFolder, this._VB_Local_NewInstanceName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                    }
                    else if (list.Count > 1)
                    {
                        Logger.Info(string.Format("有多个疑似的新增版本，无法确定：{0}", list.Select<DirectoryInfo, string>((ModDownloadLib._Closure___._I30_11 == null) ? (ModDownloadLib._Closure___._I30_11 = new Func<DirectoryInfo, string>(ModDownloadLib._Closure___._I._Lambda___30_11)) : ModDownloadLib._Closure___._I30_11).Join(";")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                    }
                    else
                    {
                        Logger.Info("未找到新增的版本文件夹", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                    }
                }
                catch (Exception ex2)
                {
                    throw new Exception(string.Format("安装新 {0} 版本失败", this._VB_Local_LoaderName), ex2);
                }
                finally
                {
                    try
                    {
                        FileUtils.Delete(this._VB_Local_InstallerAddress, false);
                    }
                    catch (Exception ex3)
                    {
                        Logger.Warn(ex3, string.Format("安装 {0} 清理文件时出错", this._VB_Local_LoaderName), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                    }
                }
            }

            internal void _Lambda___12(ModLoader.LoaderTask<List<ModNet.NetFile>, bool> Task)
            {
                ZipArchive zipArchive = null;
                try
                {
                    zipArchive = FileUtils.OpenZip(this._VB_Local_InstallerAddress, default(CancellationToken));
                    Task.Progress = 0.2;
                    JObject jobject = (JObject)zipArchive.GetEntry("install_profile.json").Open().ReadString(null)
                        .DeserializeJson();
                    Task.Progress = 0.4;
                    DirectoryUtils.Create(this._VB_Local_VersionFolder);
                    Task.Progress = 0.5;
                    if (jobject["install"] == null)
                    {
                        Logger.Info(string.Format("开始进行 Forge 安装，Legacy 方式 1：{0}", this._VB_Local_InstallerAddress), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        JObject jobject2 = (JObject)zipArchive.GetEntry(jobject["json"].ToString().TrimStart(new char[] { '/' })).Open().ReadString(null)
                            .DeserializeJson();
                        jobject2["id"] = this._VB_Local_NewInstanceName;
                        FileUtils.Write(this._VB_Local_VersionFolder + this._VB_Local_NewInstanceName + ".json", jobject2.ToString(), null);
                        Task.Progress = 0.6;
                        zipArchive.Dispose();
                        FileUtils.ExtractToDirectory(this._VB_Local_InstallerAddress, this._VB_Local_InstallerAddress + "_unrar\\", default(CancellationToken), null);
                        if (DirectoryUtils.Exists(this._VB_Local_InstallerAddress + "_unrar\\maven\\"))
                        {
                            DirectoryUtils.Copy(this._VB_Local_InstallerAddress + "_unrar\\maven\\", this._VB_Local_McFolder + "libraries\\");
                        }
                        DirectoryUtils.Delete(this._VB_Local_InstallerAddress + "_unrar\\", false);
                    }
                    else
                    {
                        Logger.Info(string.Format("开始进行 Forge 安装，Legacy 方式 2：{0}", this._VB_Local_InstallerAddress), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        FileUtils.Write(ModMinecraft.McLibGet((string)jobject["install"]["path"], true, false, this._VB_Local_McFolder), zipArchive.GetEntry((string)jobject["install"]["filePath"]).Open());
                        Task.Progress = 0.9;
                        jobject["versionInfo"]["id"] = this._VB_Local_NewInstanceName;
                        if (jobject["versionInfo"]["inheritsFrom"] == null)
                        {
                            ((JObject)jobject["versionInfo"]).Add("inheritsFrom", this._VB_Local_Inherit);
                        }
                        FileUtils.Write(this._VB_Local_VersionFolder + this._VB_Local_NewInstanceName + ".json", jobject["versionInfo"].ToString(), null);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("非新版方式安装 Forge 失败", ex);
                }
                finally
                {
                    try
                    {
                        if (zipArchive != null)
                        {
                            zipArchive.Dispose();
                        }
                        FileUtils.Delete(this._VB_Local_InstallerAddress, false);
                        DirectoryUtils.Delete(this._VB_Local_InstallerAddress + "_unrar\\", false);
                    }
                    catch (Exception ex2)
                    {
                        Logger.Warn(ex2, "非新版方式安装 Forge 清理文件时出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                    }
                }
            }

            public string _VB_Local_Inherit;

            public string _VB_Local_LoaderVersion;

            public ModDownload.DlForgelikeEntry _VB_Local_Info;

            public string _VB_Local_LoaderName;

            public string _VB_Local_InstallerAddress;

            public string _VB_Local_CurrentMcFolder;

            public bool _VB_Local_IsNeoForge;

            public string _VB_Local_McFolder;

            public ModLoader.LoaderCombo<string> _VB_Local_ClientDownloadLoader;

            public string _VB_Local_ClientFolder;

            public string _VB_Local_VersionFolder;

            public string _VB_Local_NewInstanceName;
        }

        [CompilerGenerated]
        private sealed class _Closure___30_1
        {
            public _Closure___30_1(ModDownloadLib._Closure___30_1 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Libs = arg0._VB_Local_Libs;
                }
            }

            internal void _Lambda___2(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                Task.Output = new List<ModNet.NetFile>();
                ZipArchive zipArchive = null;
                checked
                {
                    try
                    {
                        zipArchive = FileUtils.OpenZip(this._VB_NonLocal__VB_Closure_2._VB_Local_InstallerAddress, default(CancellationToken));
                        Task.Progress = 0.2;
                        JObject jobject = (JObject)zipArchive.GetEntry("install_profile.json").Open().ReadString(null)
                            .DeserializeJson();
                        JObject jobject2 = (JObject)zipArchive.GetEntry("version.json").Open().ReadString(null)
                            .DeserializeJson();
                        jobject.Merge(jobject2);
                        this._VB_Local_Libs = ModMinecraft.McLibListGetWithJson(jobject, true, this._VB_NonLocal__VB_Closure_2._VB_Local_CurrentMcFolder);
                        if (jobject["data"] != null && jobject["data"]["MOJMAPS"] != null)
                        {
                            Task.Progress = 0.4;
                            JObject jobject3 = (JObject)ModNet.NetRequestByLoader(ModDownload.DlSourceLauncherOrMetaGet(Conversions.ToString(ModDownload.DlClientListGet(this._VB_NonLocal__VB_Closure_2._VB_Local_Inherit))), 45000, true, false).DeserializeJson();
                            string text = jobject["data"]["MOJMAPS"]["client"].ToString().Trim("[]".ToCharArray()).BeforeFirst("@", false);
                            string text2 = Path.ChangeExtension(ModMinecraft.McLibGet(text, true, false, this._VB_NonLocal__VB_Closure_2._VB_Local_CurrentMcFolder), "." + jobject["data"]["MOJMAPS"]["client"].ToString().Trim("[]".ToCharArray()).Split("@", false)[1]);
                            JToken jtoken = jobject3["downloads"]["client_mappings"];
                            this._VB_Local_Libs.Add(new ModMinecraft.McLibEntry
                            {
                                IsNatives = false,
                                LocalPath = text2,
                                OriginalName = text,
                                Url = (string)jtoken["url"],
                                Size = (long)jtoken["size"],
                                SHA1 = (string)jtoken["sha1"]
                            });
                            Logger.Info(string.Format("需要下载 Mappings：{0} (SHA1: {1})", jtoken["url"], jtoken["sha1"]), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        }
                        Task.Progress = 0.8;
                        int num = this._VB_Local_Libs.Count - 1;
                        for (int i = 0; i <= num; i++)
                        {
                            if (this._VB_Local_Libs[i].LocalPath.EndsWithF(string.Format("{0}-{1}-{2}.jar", this._VB_NonLocal__VB_Closure_2._VB_Local_LoaderName.Lower(), this._VB_NonLocal__VB_Closure_2._VB_Local_Inherit, this._VB_NonLocal__VB_Closure_2._VB_Local_LoaderVersion), false) || this._VB_Local_Libs[i].LocalPath.EndsWithF(string.Format("{0}-{1}-{2}-client.jar", this._VB_NonLocal__VB_Closure_2._VB_Local_LoaderName.Lower(), this._VB_NonLocal__VB_Closure_2._VB_Local_Inherit, this._VB_NonLocal__VB_Closure_2._VB_Local_LoaderVersion), false))
                            {
                                Logger.Warn(string.Format("已从待下载 {0} 支持库中移除：{1}", this._VB_NonLocal__VB_Closure_2._VB_Local_LoaderName, this._VB_Local_Libs[i].LocalPath), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                                this._VB_Local_Libs.RemoveAt(i);
                                break;
                            }
                        }
                        Task.Output = ModMinecraft.McLibNetFilesFromTokens(this._VB_Local_Libs, this._VB_NonLocal__VB_Closure_2._VB_Local_CurrentMcFolder);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("获取{0}Forge 支持库列表失败", this._VB_NonLocal__VB_Closure_2._VB_Local_IsNeoForge ? " Neo" : "新版 "), ex);
                    }
                    finally
                    {
                        if (zipArchive != null)
                        {
                            zipArchive.Dispose();
                        }
                    }
                }
            }

            internal void _Lambda___3(ModLoader.LoaderTask<List<ModNet.NetFile>, bool> Task)
            {
                try
                {
                    foreach (ModMinecraft.McLibEntry mcLibEntry in this._VB_Local_Libs)
                    {
                        string text = Path.Combine(this._VB_NonLocal__VB_Closure_2._VB_Local_McFolder, mcLibEntry.LocalPath.Substring(this._VB_NonLocal__VB_Closure_2._VB_Local_CurrentMcFolder.Length));
                        if (!FileUtils.Exists(text))
                        {
                            FileUtils.Copy(mcLibEntry.LocalPath, text);
                        }
                        Logger.Trace(string.Format("复制的 {0} 支持库文件：{1}", this._VB_NonLocal__VB_Closure_2._VB_Local_LoaderName, mcLibEntry.LocalPath), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                    }
                }
                finally
                {
                    List<ModMinecraft.McLibEntry>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
                List<ModLoader.LoaderBase> list = this._VB_NonLocal__VB_Closure_2._VB_Local_ClientDownloadLoader.GetLoaderList(true).Where<ModLoader.LoaderBase>((ModDownloadLib._Closure___._I30_4 == null) ? (ModDownloadLib._Closure___._I30_4 = new Func<ModLoader.LoaderBase, bool>(ModDownloadLib._Closure___._I._Lambda___30_4)) : ModDownloadLib._Closure___._I30_4).Where<ModLoader.LoaderBase>((ModDownloadLib._Closure___._I30_5 == null) ? (ModDownloadLib._Closure___._I30_5 = new Func<ModLoader.LoaderBase, bool>(ModDownloadLib._Closure___._I._Lambda___30_5)) : ModDownloadLib._Closure___._I30_5)
                    .ToList<ModLoader.LoaderBase>();
                if (list.Any<ModLoader.LoaderBase>())
                {
                    Logger.Info(string.Format("{0} 安装正在等待原版文件下载完成", this._VB_NonLocal__VB_Closure_2._VB_Local_LoaderName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                while (list.Any<ModLoader.LoaderBase>() && !Task.IsCanceled)
                {
                    list = list.Where<ModLoader.LoaderBase>((ModDownloadLib._Closure___._I30_6 == null) ? (ModDownloadLib._Closure___._I30_6 = new Func<ModLoader.LoaderBase, bool>(ModDownloadLib._Closure___._I._Lambda___30_6)) : ModDownloadLib._Closure___._I30_6).ToList<ModLoader.LoaderBase>();
                    Thread.Sleep(50);
                }
                if (!Task.IsCanceled)
                {
                    object vanillaSyncLock = ModDownloadLib.VanillaSyncLock;
                    ObjectFlowControl.CheckForSyncLockOnValueType(vanillaSyncLock);
                    lock (vanillaSyncLock)
                    {
                        string lastPart = PathUtils.GetLastPart(this._VB_NonLocal__VB_Closure_2._VB_Local_ClientFolder);
                        DirectoryUtils.Create(this._VB_NonLocal__VB_Closure_2._VB_Local_McFolder + "versions\\" + this._VB_NonLocal__VB_Closure_2._VB_Local_Inherit);
                        if (!FileUtils.Exists(string.Concat(new string[]
                        {
                            this._VB_NonLocal__VB_Closure_2._VB_Local_McFolder,
                            "versions\\",
                            this._VB_NonLocal__VB_Closure_2._VB_Local_Inherit,
                            "\\",
                            this._VB_NonLocal__VB_Closure_2._VB_Local_Inherit,
                            ".json"
                        })))
                        {
                            FileUtils.Copy(this._VB_NonLocal__VB_Closure_2._VB_Local_ClientFolder + lastPart + ".json", string.Concat(new string[]
                            {
                                this._VB_NonLocal__VB_Closure_2._VB_Local_McFolder,
                                "versions\\",
                                this._VB_NonLocal__VB_Closure_2._VB_Local_Inherit,
                                "\\",
                                this._VB_NonLocal__VB_Closure_2._VB_Local_Inherit,
                                ".json"
                            }));
                        }
                        if (!FileUtils.Exists(string.Concat(new string[]
                        {
                            this._VB_NonLocal__VB_Closure_2._VB_Local_McFolder,
                            "versions\\",
                            this._VB_NonLocal__VB_Closure_2._VB_Local_Inherit,
                            "\\",
                            this._VB_NonLocal__VB_Closure_2._VB_Local_Inherit,
                            ".jar"
                        })))
                        {
                            FileUtils.Copy(this._VB_NonLocal__VB_Closure_2._VB_Local_ClientFolder + lastPart + ".jar", string.Concat(new string[]
                            {
                                this._VB_NonLocal__VB_Closure_2._VB_Local_McFolder,
                                "versions\\",
                                this._VB_NonLocal__VB_Closure_2._VB_Local_Inherit,
                                "\\",
                                this._VB_NonLocal__VB_Closure_2._VB_Local_Inherit,
                                ".jar"
                            }));
                        }
                    }
                }
            }

            public List<ModMinecraft.McLibEntry> _VB_Local_Libs;

            public ModDownloadLib._Closure___30_0 _VB_NonLocal__VB_Closure_2;
        }

        [CompilerGenerated]
        private sealed class _Closure___30_2
        {
            public _Closure___30_2(ModDownloadLib._Closure___30_2 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_OldList = arg0._VB_Local_OldList;
                }
            }

            internal bool _Lambda___8(string i)
            {
                return this._VB_Local_OldList.Contains(i);
            }

            public List<string> _VB_Local_OldList;
        }

        [CompilerGenerated]
        private sealed class _Closure___32_0
        {
            public _Closure___32_0()
            {
            }

            internal OurListItem _Lambda___0()
            {
                OurListItem ourListItem = new OurListItem
                {
                    Title = this._VB_Local_Entry.VersionName,
                    SnapsToDevicePixels = true,
                    Height = 42.0,
                    Type = OurListItem.CheckType.Clickable,
                    Tag = this._VB_Local_Entry,
                    Info = new string[]
                    {
                        (Operators.CompareString(this._VB_Local_Entry.ReleaseTime, "", false) == 0) ? "" : ("发布于 " + this._VB_Local_Entry.ReleaseTime),
                        ModBase.ModeDebug ? ("种类：" + this._VB_Local_Entry.Category) : ""
                    }.Where<string>((ModDownloadLib._Closure___._I32_1 == null) ? (ModDownloadLib._Closure___._I32_1 = new Func<string, bool>(ModDownloadLib._Closure___._I._Lambda___32_1)) : ModDownloadLib._Closure___._I32_1).Join("，"),
                    Logo = ModBase.PathImage + "Blocks/Anvil.png"
                };
                ourListItem.Click += this._VB_Local_OnClick;
                if (this._VB_Local_IsSaveOnly)
                {
                    ourListItem.ContentHandler = new Action<OurListItem, EventArgs>(ModDownloadLib.ForgeSaveContMenuBuild);
                }
                else
                {
                    ourListItem.ContentHandler = new Action<OurListItem, EventArgs>(ModDownloadLib.ForgeContMenuBuild);
                }
                return ourListItem;
            }

            public ModDownload.DlForgeVersionEntry _VB_Local_Entry;

            public OurListItem.ClickEventHandler _VB_Local_OnClick;

            public bool _VB_Local_IsSaveOnly;
        }

        [CompilerGenerated]
        private sealed class _Closure___4_0
        {
            public _Closure___4_0()
            {
            }

            internal OurListItem _Lambda___0()
            {
                JToken jtoken = this._VB_Local_Entry["type"];
                string text;
                if ((string)jtoken == "release")
                {
                    text = ModBase.PathImage + "Blocks/Grass.png";
                }
                else if ((string)jtoken == "snapshot")
                {
                    text = ModBase.PathImage + "Blocks/CommandBlock.png";
                }
                else if ((string)jtoken == "special")
                {
                    text = ModBase.PathImage + "Blocks/GoldBlock.png";
                }
                else
                {
                    text = ModBase.PathImage + "Blocks/CobbleStone.png";
                }
                OurListItem ourListItem = new OurListItem
                {
                    Logo = text,
                    SnapsToDevicePixels = true,
                    Title = this._VB_Local_Entry["id"].ToString(),
                    Height = 42.0,
                    Type = OurListItem.CheckType.Clickable,
                    Tag = this._VB_Local_Entry
                };
                if (this._VB_Local_Entry["lore"] == null)
                {
                    ourListItem.Info = this._VB_Local_Entry["releaseTime"].Value<DateTime>().ToString("yyyy'/'MM'/'dd HH':'mm");
                }
                else
                {
                    ourListItem.Info = this._VB_Local_Entry["lore"].ToString();
                }
                if (this._VB_Local_Entry["url"].ToString().Contains("pcl"))
                {
                    ourListItem.Info = "[PCL 特供下载] " + ourListItem.Info;
                }
                ourListItem.Click += this._VB_Local_OnClick;
                if (this._VB_Local_IsSaveOnly)
                {
                    ourListItem.ContentHandler = new Action<OurListItem, EventArgs>(ModDownloadLib.McDownloadSaveMenuBuild);
                }
                else
                {
                    ourListItem.ContentHandler = new Action<OurListItem, EventArgs>(ModDownloadLib.McDownloadMenuBuild);
                }
                return ourListItem;
            }

            public JObject _VB_Local_Entry;

            public OurListItem.ClickEventHandler _VB_Local_OnClick;

            public bool _VB_Local_IsSaveOnly;
        }

        [CompilerGenerated]
        private sealed class _Closure___41_0
        {
            public _Closure___41_0()
            {
            }

            internal OurListItem _Lambda___0()
            {
                OurListItem ourListItem = new OurListItem
                {
                    Title = this._VB_Local_Info.VersionName,
                    SnapsToDevicePixels = true,
                    Height = 42.0,
                    Type = OurListItem.CheckType.Clickable,
                    Tag = this._VB_Local_Info,
                    Info = (this._VB_Local_Info.IsBeta ? "测试版" : "稳定版"),
                    Logo = ModBase.PathImage + "Blocks/NeoForge.png"
                };
                ourListItem.Click += this._VB_Local_OnClick;
                if (this._VB_Local_IsSaveOnly)
                {
                    ourListItem.ContentHandler = new Action<OurListItem, EventArgs>(ModDownloadLib.NeoForgeSaveContMenuBuild);
                }
                else
                {
                    ourListItem.ContentHandler = new Action<OurListItem, EventArgs>(ModDownloadLib.NeoForgeContMenuBuild);
                }
                return ourListItem;
            }

            public ModDownload.DlNeoForgeListEntry _VB_Local_Info;

            public OurListItem.ClickEventHandler _VB_Local_OnClick;

            public bool _VB_Local_IsSaveOnly;
        }

        [CompilerGenerated]
        private sealed class _Closure___47_0
        {
            public _Closure___47_0()
            {
            }

            internal void _Lambda___0(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                Task.Output = new List<ModNet.NetFile>
                {
                    new ModNet.NetFile(new string[]
                    {
                        string.Concat(new string[] { "https://bmclapi2.bangbang93.com/fabric-meta/v2/versions/loader/", this._VB_Local_MinecraftName, "/", this._VB_Local_FabricVersion, "/profile/json" }),
                        string.Concat(new string[] { "https://meta.fabricmc.net/v2/versions/loader/", this._VB_Local_MinecraftName, "/", this._VB_Local_FabricVersion, "/profile/json" })
                    }, this._VB_Local_VersionFolder + this._VB_Local_Id + ".json", new FileChecker
                    {
                        IsJson = true
                    }, false)
                };
            }

            public string _VB_Local_MinecraftName;

            public string _VB_Local_FabricVersion;

            public string _VB_Local_VersionFolder;

            public string _VB_Local_Id;
        }

        [CompilerGenerated]
        private sealed class _Closure___48_0
        {
            public _Closure___48_0()
            {
            }

            internal OurListItem _Lambda___0()
            {
                OurListItem ourListItem = new OurListItem();
                ourListItem.Title = this._VB_Local_Entry["version"].ToString().Replace("+build", "");
                ourListItem.SnapsToDevicePixels = true;
                ourListItem.Height = 42.0;
                ourListItem.Type = OurListItem.CheckType.Clickable;
                ourListItem.Tag = this._VB_Local_Entry;
                ourListItem.Info = (this._VB_Local_Entry["stable"].ToObject<bool>() ? "稳定版" : "测试版");
                ourListItem.Logo = ModBase.PathImage + "Blocks/Fabric.png";
                ourListItem.Click += this._VB_Local_OnClick;
                return ourListItem;
            }

            public JObject _VB_Local_Entry;

            public OurListItem.ClickEventHandler _VB_Local_OnClick;
        }

        [CompilerGenerated]
        private sealed class _Closure___49_0
        {
            public _Closure___49_0()
            {
            }

            internal OurListItem _Lambda___0()
            {
                OurListItem ourListItem = new OurListItem();
                ourListItem.Title = this._VB_Local_Entry.Display.Split("]", false)[1].Replace("Fabric API ", "").Replace(" build ", ".").BeforeFirst("+", false)
                    .Trim();
                ourListItem.SnapsToDevicePixels = true;
                ourListItem.Height = 42.0;
                ourListItem.Type = OurListItem.CheckType.Clickable;
                ourListItem.Tag = this._VB_Local_Entry;
                ourListItem.Info = this._VB_Local_Entry.ReleaseTypeDisplay + "，发布于 " + this._VB_Local_Entry.ReleaseDate.ToString("yyyy'/'MM'/'dd HH':'mm");
                ourListItem.Logo = ModBase.PathImage + "Blocks/Fabric.png";
                ourListItem.Click += this._VB_Local_OnClick;
                return ourListItem;
            }

            public ResourceVersion _VB_Local_Entry;

            public OurListItem.ClickEventHandler _VB_Local_OnClick;
        }

        [CompilerGenerated]
        private sealed class _Closure___50_0
        {
            public _Closure___50_0()
            {
            }

            internal OurListItem _Lambda___0()
            {
                OurListItem ourListItem = new OurListItem
                {
                    Title = this._VB_Local_Entry.Display.Lower().Replace("optifabric-", "").Replace(".jar", "")
                        .Trim()
                        .TrimStart(new char[] { 'v' }),
                    SnapsToDevicePixels = true,
                    Height = 42.0,
                    Type = OurListItem.CheckType.Clickable,
                    Tag = this._VB_Local_Entry,
                    Info = this._VB_Local_Entry.ReleaseTypeDisplay + "，发布于 " + this._VB_Local_Entry.ReleaseDate.ToString("yyyy'/'MM'/'dd HH':'mm"),
                    Logo = ModBase.PathImage + "Blocks/OptiFabric.png"
                };
                ourListItem.Click += this._VB_Local_OnClick;
                return ourListItem;
            }

            public ResourceVersion _VB_Local_Entry;

            public OurListItem.ClickEventHandler _VB_Local_OnClick;
        }

        [CompilerGenerated]
        private sealed class _Closure___52_0
        {
            public _Closure___52_0(ModDownloadLib._Closure___52_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_VersionFolder = arg0._VB_Local_VersionFolder;
                    this._VB_Local_OptiFineFolder = arg0._VB_Local_OptiFineFolder;
                    this._VB_Local_OptiFineAsMod = arg0._VB_Local_OptiFineAsMod;
                    this._VB_Local_ForgeFolder = arg0._VB_Local_ForgeFolder;
                    this._VB_Local_NeoForgeFolder = arg0._VB_Local_NeoForgeFolder;
                    this._VB_Local_FabricFolder = arg0._VB_Local_FabricFolder;
                    this._VB_Local_LiteLoaderFolder = arg0._VB_Local_LiteLoaderFolder;
                    this._VB_Local_TempMcFolder = arg0._VB_Local_TempMcFolder;
                    this._VB_Local_MinecraftFolder = arg0._VB_Local_MinecraftFolder;
                    this._VB_Local_TargetFolder = arg0._VB_Local_TargetFolder;
                    this._VB_Local_Request = arg0._VB_Local_Request;
                    this._VB_Local_ModsTempFolder = arg0._VB_Local_ModsTempFolder;
                    this._VB_Local_Modable = arg0._VB_Local_Modable;
                    this._VB_Local_ManagedFileSources = arg0._VB_Local_ManagedFileSources;
                    this._VB_Local_ForgeVersion = arg0._VB_Local_ForgeVersion;
                    this._VB_Local_NeoForgeVersion = arg0._VB_Local_NeoForgeVersion;
                }
            }

            internal void _Lambda___R22(ModLoader.LoaderTask<int, int> a0)
            {
                this._Lambda___0();
            }

            internal void _Lambda___0()
            {
                FileUtils.Write(this._VB_Local_VersionFolder + ".pclignore", "用于临时地在 PCL 的版本列表中屏蔽此版本。", null);
            }

            internal void _Lambda___1(ModLoader.LoaderTask<string, string> Task)
            {
                ModDownloadLib.MergeJson(this._VB_Local_VersionFolder, this._VB_Local_OptiFineFolder, this._VB_Local_OptiFineAsMod, this._VB_Local_ForgeFolder, this._VB_Local_NeoForgeFolder, this._VB_Local_FabricFolder, this._VB_Local_LiteLoaderFolder);
                Task.Progress = 0.2;
                DirectoryUtils.Copy(this._VB_Local_TempMcFolder + "libraries", this._VB_Local_MinecraftFolder + "libraries");
                Task.Progress = 0.8;
                this._VB_Local_TargetFolder = (this._VB_Local_Request.IsIntermediateOutput ? this._VB_Local_VersionFolder : new ModMinecraft.McInstance(this._VB_Local_VersionFolder).PathIndie);
                string text = this._VB_Local_TargetFolder + "mods\\";
                if (DirectoryUtils.Exists(this._VB_Local_ModsTempFolder))
                {
                    DirectoryUtils.Copy(this._VB_Local_ModsTempFolder, text);
                }
                else if (this._VB_Local_Modable)
                {
                    DirectoryUtils.Create(text);
                    Logger.Info(string.Format("自动创建 Mod 文件夹：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                string text2 = this._VB_Local_TargetFolder + "resourcepacks\\";
                DirectoryUtils.Create(text2);
                Logger.Info(string.Format("自动创建资源包文件夹：{0}", text2), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }

            internal void _Lambda___2(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                Task.Output = ModMinecraft.McLibNetFilesFromInstance(new ModMinecraft.McInstance(this._VB_Local_VersionFolder));
            }

            internal void _Lambda___R23(ModLoader.LoaderTask<int, int> a0)
            {
                this._Lambda___3();
            }

            internal void _Lambda___3()
            {
                List<McInstallManifest.ManagedFile> list = new List<McInstallManifest.ManagedFile>();
                try
                {
                    foreach (KeyValuePair<string, string> keyValuePair in this._VB_Local_ManagedFileSources)
                    {
                        string text = keyValuePair.Value.Substring(this._VB_Local_ModsTempFolder.Length);
                        string text2 = PathUtils.ForCompare(Path.Combine(this._VB_Local_TargetFolder, "mods", text));
                        list.Add(new McInstallManifest.ManagedFile("mods/" + text.Replace("\\", "/").ToLowerInvariant(), CryptographyUtils.ComputeFileHash(text2, CryptographyUtils.HashMethod.Sha1), FileUtils.GetInfo(text2).LastWriteTimeUtc, keyValuePair.Key));
                    }
                }
                finally
                {
                    Dictionary<string, string>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
                Dictionary<ModLoaders, string> dictionary = new Dictionary<ModLoaders, string>(this._VB_Local_Request.Components.ModLoaderVersions);
                if (this._VB_Local_ForgeVersion != null && this._VB_Local_ForgeVersion.StartsWithF("1.", false) && this._VB_Local_ForgeVersion.Contains("-"))
                {
                    dictionary[ModLoaders.Forge] = this._VB_Local_ForgeVersion.AfterLast("-", false);
                }
                if (this._VB_Local_NeoForgeVersion != null && this._VB_Local_NeoForgeVersion.StartsWithF("1.20.1-", false))
                {
                    dictionary[ModLoaders.NeoForge] = this._VB_Local_NeoForgeVersion.Replace("1.20.1-", "");
                }
                McComponents mcComponents = new McComponents(this._VB_Local_Request.Components.MinecraftVersion, dictionary, this._VB_Local_Request.Components.OptiFineEntry, this._VB_Local_Request.Components.FabricApiVersion, this._VB_Local_Request.Components.OptiFabricVersion);
                JsonConfigProvider config = new ModMinecraft.McInstance(this._VB_Local_VersionFolder).Config;
                Configs.McInstallManifest.Set(new McInstallManifest(1, mcComponents, DateTime.UtcNow, list), config);
                config.Save();
            }

            internal void _Lambda___R24(ModLoader.LoaderTask<int, int> a0)
            {
                this._Lambda___4();
            }

            internal void _Lambda___4()
            {
                FileUtils.Delete(this._VB_Local_VersionFolder + ".pclignore", false);
            }

            public string _VB_Local_VersionFolder;

            public string _VB_Local_OptiFineFolder;

            public bool _VB_Local_OptiFineAsMod;

            public string _VB_Local_ForgeFolder;

            public string _VB_Local_NeoForgeFolder;

            public string _VB_Local_FabricFolder;

            public string _VB_Local_LiteLoaderFolder;

            public string _VB_Local_TempMcFolder;

            public string _VB_Local_MinecraftFolder;

            public string _VB_Local_TargetFolder;

            public ModDownloadLib.McInstallRequest _VB_Local_Request;

            public string _VB_Local_ModsTempFolder;

            public bool _VB_Local_Modable;

            public Dictionary<string, string> _VB_Local_ManagedFileSources;

            public string _VB_Local_ForgeVersion;

            public string _VB_Local_NeoForgeVersion;
        }

        [CompilerGenerated]
        private sealed class _Closure___55_0
        {
            public _Closure___55_0(ModDownloadLib._Closure___55_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Instance = arg0._VB_Local_Instance;
                    this._VB_Local_Request = arg0._VB_Local_Request;
                }
            }

            internal void _Lambda___R25(ModLoader.LoaderTask<int, int> a0)
            {
                this._Lambda___0();
            }

            internal void _Lambda___0()
            {
                try
                {
                    foreach (string text in DirectoryUtils.EnumerateFiles(ModLaunch.GetNativesFolder(this._VB_Local_Instance), true, "*.dll"))
                    {
                        if (!FileUtils.IsAvailable(text))
                        {
                            throw new Exception(string.Format("Natives 文件正在被占用，请关闭游戏后再次尝试：{0}", text));
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
            }

            public ModMinecraft.McInstance _VB_Local_Instance;

            public ModDownloadLib.McInstallRequest _VB_Local_Request;
        }

        [CompilerGenerated]
        private sealed class _Closure___55_1
        {
            public _Closure___55_1(ModDownloadLib._Closure___55_1 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_BuildFolder = arg0._VB_Local_BuildFolder;
                    this._VB_Local_TargetFolder = arg0._VB_Local_TargetFolder;
                    this._VB_Local_RenameFailed = arg0._VB_Local_RenameFailed;
                    this._VB_Local_Loader = arg0._VB_Local_Loader;
                }
            }

            internal void _Lambda___1(ModLoader.LoaderTask<int, int> Task)
            {
                ModDownloadLib._Closure___55_2 CS_locals1 = new ModDownloadLib._Closure___55_2(null);
                CS_locals1._VB_NonLocal__VB_Closure_3 = this;
                ModDownloadLib.McInstallCommit(this._VB_NonLocal__VB_Closure_2._VB_Local_Instance, Configs.McInstallManifest.Get(new ModMinecraft.McInstance(this._VB_Local_BuildFolder).Config), this._VB_Local_BuildFolder, this._VB_Local_TargetFolder);
                CS_locals1._VB_Local_NewInstance = null;
                if (this._VB_NonLocal__VB_Closure_2._VB_Local_Request.NewInstanceName == null)
                {
                    CS_locals1._VB_Local_NewInstance = new ModMinecraft.McInstance(this._VB_NonLocal__VB_Closure_2._VB_Local_Instance.PathVersion).Load();
                }
                else
                {
                    try
                    {
                        CS_locals1._VB_Local_NewInstance = this._VB_NonLocal__VB_Closure_2._VB_Local_Instance.Rename(this._VB_NonLocal__VB_Closure_2._VB_Local_Request.NewInstanceName);
                    }
                    catch (Exception ex)
                    {
                        this._VB_Local_RenameFailed = true;
                        Logger.Warn(ex, string.Format("版本修改成功，但重命名为 {0} 失败：{1}", this._VB_NonLocal__VB_Closure_2._VB_Local_Request.NewInstanceName, ex.GetDisplay(false)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        CS_locals1._VB_Local_NewInstance = new ModMinecraft.McInstance(this._VB_NonLocal__VB_Closure_2._VB_Local_Instance.PathVersion).Load();
                    }
                }
                if (((object)(CS_locals1._VB_Local_NewInstance) == (object)null))
                {
                    CS_locals1._VB_Local_NewInstance = new ModMinecraft.McInstance(this._VB_NonLocal__VB_Closure_2._VB_Local_Instance.PathVersion).Load();
                }
                ModBase.RunInUiWait(new Action(CS_locals1._Lambda___2));
            }

            internal void _Lambda___3(ModLoader.LoaderBase CurrentLoader)
            {
                switch (CurrentLoader.State)
                {
                case ModBase.LoadState.Finished:
                    if (!this._VB_Local_RenameFailed)
                    {
                        ModMain.Hint(CurrentLoader.Name + "成功！", ModMain.HintType.Green, true);
                        return;
                    }
                    ModMain.Hint(CurrentLoader.Name + "成功，但重命名失败！", ModMain.HintType.Green, true);
                    return;
                case ModBase.LoadState.Failed:
                    ModMain.OurMsgBox(CurrentLoader.Error.GetDisplay(true), CurrentLoader.Name + "失败", "确定", "", "", true, true, false, null, null, null);
                    return;
                case ModBase.LoadState.Canceled:
                    ModMain.Hint(CurrentLoader.Name + "已取消！", ModMain.HintType.Blue, true);
                    return;
                default:
                    return;
                }
            }

            internal void _Lambda___4()
            {
                this._VB_Local_Loader.WaitForExit(this._VB_NonLocal__VB_Closure_2._VB_Local_Instance.PathVersion, null, false, default(CancellationToken), null);
            }

            public string _VB_Local_BuildFolder;

            public string _VB_Local_TargetFolder;

            public bool _VB_Local_RenameFailed;

            public ModLoader.LoaderCombo<string> _VB_Local_Loader;

            public ModDownloadLib._Closure___55_0 _VB_NonLocal__VB_Closure_2;
        }

        [CompilerGenerated]
        private sealed class _Closure___55_2
        {
            public _Closure___55_2(ModDownloadLib._Closure___55_2 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_NewInstance = arg0._VB_Local_NewInstance;
                }
            }

            internal void _Lambda___2()
            {
                ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "InstanceCache", "");
                if (((object)(PageInstanceLeft.Instance) != (object)null) && PathUtils.IsSame(PageInstanceLeft.Instance.PathVersion, this._VB_NonLocal__VB_Closure_3._VB_NonLocal__VB_Closure_2._VB_Local_Instance.PathVersion))
                {
                    PageInstanceLeft.Instance = this._VB_Local_NewInstance;
                }
                if (((object)(ModMinecraft.McInstanceSelected) != (object)null) && PathUtils.IsSame(ModMinecraft.McInstanceSelected.PathVersion, this._VB_NonLocal__VB_Closure_3._VB_NonLocal__VB_Closure_2._VB_Local_Instance.PathVersion))
                {
                    ModMinecraft.McInstanceSelected = this._VB_Local_NewInstance;
                }
            }

            public ModMinecraft.McInstance _VB_Local_NewInstance;

            public ModDownloadLib._Closure___55_1 _VB_NonLocal__VB_Closure_3;
        }

        [CompilerGenerated]
        private sealed class _Closure___56_0
        {
            public _Closure___56_0(ModDownloadLib._Closure___56_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_TargetFolder = arg0._VB_Local_TargetFolder;
                }
            }

            internal McInstallManifest.ManagedFile _Lambda___1(McInstallManifest.ManagedFile File)
            {
                return new McInstallManifest.ManagedFile(File.RelativePath, File.Sha1, FileUtils.GetInfo(Path.Combine(this._VB_Local_TargetFolder, File.RelativePath)).LastWriteTimeUtc, File.Component);
            }

            public string _VB_Local_TargetFolder;
        }

        [CompilerGenerated]
        private sealed class _Closure___59_0
        {
            public _Closure___59_0()
            {
            }

            internal void _Lambda___0()
            {
                try
                {
                    Thread.Sleep(1000);
                    if (DirectoryUtils.Exists(Conversions.ToString(Operators.ConcatenateObject(NewLateBinding.LateGet(this._VB_Local_Loader, null, "Input", new object[0], null, null, null), "saves\\"))) || DirectoryUtils.Exists(Conversions.ToString(Operators.ConcatenateObject(NewLateBinding.LateGet(this._VB_Local_Loader, null, "Input", new object[0], null, null, null), "versions\\"))))
                    {
                        Logger.Warn(string.Format("由于版本已被独立启动，不清理版本文件夹：{0}", RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(this._VB_Local_Loader, null, "Input", new object[0], null, null, null))), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                    }
                    else
                    {
                        Logger.Warn(string.Format("由于下载失败或取消，清理版本文件夹：{0}", RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(this._VB_Local_Loader, null, "Input", new object[0], null, null, null))), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        DirectoryUtils.Delete(Conversions.ToString(NewLateBinding.LateGet(this._VB_Local_Loader, null, "Input", new object[0], null, null, null)), false);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "下载失败或取消后清理版本文件夹失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
            }

            public object _VB_Local_Loader;
        }

        [CompilerGenerated]
        private sealed class _Closure___8_0
        {
            public _Closure___8_0(ModDownloadLib._Closure___8_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_VersionFolder = arg0._VB_Local_VersionFolder;
                    this._VB_Local_Id = arg0._VB_Local_Id;
                }
            }

            internal void _Lambda___0(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                ModMinecraft.McInstance mcInstance = new ModMinecraft.McInstance(this._VB_Local_VersionFolder);
                if (mcInstance.JsonObject["downloads"] == null || mcInstance.JsonObject["downloads"]["server"] == null || mcInstance.JsonObject["downloads"]["server"]["url"] == null)
                {
                    FileUtils.Delete(this._VB_Local_VersionFolder + this._VB_Local_Id + ".json", false);
                    if (!DirectoryUtils.GetInfo(this._VB_Local_VersionFolder).GetFileSystemInfos().Any<FileSystemInfo>())
                    {
                        DirectoryUtils.Delete(this._VB_Local_VersionFolder, false);
                    }
                    Task.Output = new List<ModNet.NetFile>();
                    ModMain.Hint(string.Format("Mojang 没有给 Minecraft {0} 提供官方服务端下载，没法下，撤退！", this._VB_Local_Id), ModMain.HintType.Red, true);
                    Thread.Sleep(2000);
                    Task.Cancel();
                    return;
                }
                string text = (string)mcInstance.JsonObject["downloads"]["server"]["url"];
                FileChecker fileChecker = new FileChecker
                {
                    MinSize = 1024L,
                    ActualSize = (long)(mcInstance.JsonObject["downloads"]["server"]["size"] ?? (-1)),
                    Hash = (string)mcInstance.JsonObject["downloads"]["server"]["sha1"]
                };
                Task.Output = new List<ModNet.NetFile>
                {
                    new ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(text), this._VB_Local_VersionFolder + this._VB_Local_Id + "-server.jar", fileChecker, false)
                };
                string text2 = string.Format("@echo off\r\ntitle {0} 原版服务端\r\necho 如果服务端立即停止，请右键编辑该脚本，将下一行开头的 java 替换为适合该 Minecraft 版本的完整 java.exe 的路径。\r\necho 你可以在 PCL 的 [设置 → 启动选项] 中查看已安装的 java，所需的 java.exe 一般在其中的 bin 文件夹下。\r\necho ------------------------------\r\necho 如果提示 \"You need to agree to the EULA in order to run the server\"，请打开 eula.txt，按说明阅读并同意 Minecraft EULA 后，将该文件最后一行中的 eula=false 改为 eula=true。\r\necho ------------------------------\r\n\"java\" -server -XX:+UseG1GC -Xmx6144M -Xms1024M -jar {1}-server.jar nogui\r\necho ----------------------\r\necho 服务端已停止。\r\npause", this._VB_Local_Id, this._VB_Local_Id);
                FileUtils.Write(this._VB_Local_VersionFolder + "Launch Server.bat", text2, Encoding.Default.Equals(Encoding.UTF8) ? Encoding.UTF8 : Encoding.GetEncoding("GB18030"));
                FileUtils.Delete(this._VB_Local_VersionFolder + this._VB_Local_Id + ".json", false);
            }

            public string _VB_Local_VersionFolder;

            public string _VB_Local_Id;
        }

        [CompilerGenerated]
        private sealed class _Closure___9_0
        {
            public _Closure___9_0(ModDownloadLib._Closure___9_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_VersionFolder = arg0._VB_Local_VersionFolder;
                }
            }

            internal void _Lambda___0(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                Task.Output = new List<ModNet.NetFile> { ModDownload.DlClientJarGet(new ModMinecraft.McInstance(this._VB_Local_VersionFolder), false) };
            }

            public string _VB_Local_VersionFolder;
        }
    }
}
