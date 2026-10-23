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
    public sealed class ModDownloadLib
    {
        private static List<ModLoader.LoaderBase> McDownloadClientLoader(string Id, string JsonUrl, string VersionFolder)
        {
            string InstanceName = PathUtils.GetLastPart(VersionFolder);
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            ModMinecraft.McInstance NewInstance = null;
            if (JsonUrl == null)
            {
                list.Add(new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("获取原版 json 文件下载地址", delegate(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
                {
                    string text = Conversions.ToString(ModDownload.DlClientListGet(Id));
                    Task.Output = new List<ModNet.NetFile>
                    {
                        new ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(text), VersionFolder + InstanceName + ".json", null, false)
                    };
                }, null, ThreadPriority.Normal)
                {
                    ProgressWeight = 2.0,
                    Show = false
                });
            }
            list.Add(new ModNet.LoaderDownload("下载原版 json 文件", new List<ModNet.NetFile>
            {
                new ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(JsonUrl ?? ""), VersionFolder + InstanceName + ".json", new FileChecker
                {
                    IsJson = true
                }, false)
            })
            {
                ProgressWeight = 3.0
            });
            list.Add(new ModLoader.LoaderTask<string, string>("读取 json 文件", delegate(ModLoader.LoaderTask<string, string> Task)
            {
                NewInstance = new ModMinecraft.McInstance(VersionFolder);
                NewInstance.Load();
            }, null, ThreadPriority.Normal)
            {
                ProgressWeight = 0.0,
                Show = false
            });
            list.Add(new ModLoader.LoaderCombo<string>("下载原版支持库文件", new List<ModLoader.LoaderBase>
            {
                new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("分析原版支持库文件（副加载器）", delegate(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
                {
                    Logger.Info(string.Format("开始分析原版支持库文件：{0}", VersionFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                    Task.Output = ModMinecraft.McLibNetFilesFromInstance(NewInstance);
                }, null, ThreadPriority.Normal)
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
                new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("分析资源文件索引地址（副加载器）", delegate(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
                {
                    try
                    {
                        ModNet.NetFile netFile = ModDownload.DlClientAssetIndexGet(NewInstance);
                        List<ModNet.NetFile> list2;
                        if (netFile != null)
                        {
                            (list2 = new List<ModNet.NetFile>()).Add(netFile);
                        }
                        else
                        {
                            list2 = new List<ModNet.NetFile>();
                        }
                        Task.Output = list2;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("分析资源文件索引地址失败", ex);
                    }
                    try
                    {
                        JObject jobject = (JObject)FileUtils.ReadAsJson(VersionFolder + InstanceName + ".json", null, null);
                        jobject["clientVersion"] = Id;
                        FileUtils.Write(string.Format("{0}{1}.json", VersionFolder, InstanceName), jobject.ToString(), null);
                    }
                    catch (Exception ex2)
                    {
                        throw new Exception("添加客户端版本失败", ex2);
                    }
                }, null, ThreadPriority.Normal)
                {
                    ProgressWeight = 1.0,
                    Show = false
                },
                new ModNet.LoaderDownload("下载资源文件索引（副加载器）", new List<ModNet.NetFile>())
                {
                    ProgressWeight = 3.0,
                    Show = false
                },
                new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("分析所需资源文件（副加载器）", delegate(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
                {
                    ModLoader.LoaderTask<string, List<ModNet.NetFile>> loaderTask = Task;
                    ModMinecraft.McInstance $VB$Local_NewInstance = NewInstance;
                    bool flag = true;
                    ModLoader.LoaderBase loaderBase = Task;
                    List<ModNet.NetFile> list3 = ModMinecraft.McAssetsFixList($VB$Local_NewInstance, flag, ref loaderBase);
                    Task = (ModLoader.LoaderTask<string, List<ModNet.NetFile>>)loaderBase;
                    loaderTask.Output = list3;
                }, null, ThreadPriority.Normal)
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

        public static MyVirtualizingElement<MyListItem> McDownloadListItem(JObject Entry, MyListItem.ClickEventHandler OnClick, bool IsSaveOnly)
        {
            return new MyVirtualizingElement<MyListItem>(delegate
            {
                JToken jtoken = Entry["type"];
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
                MyListItem myListItem = new MyListItem
                {
                    Logo = text,
                    SnapsToDevicePixels = true,
                    Title = Entry["id"].ToString(),
                    Height = 42.0,
                    Type = MyListItem.CheckType.Clickable,
                    Tag = Entry
                };
                if (Entry["lore"] == null)
                {
                    myListItem.Info = Extensions.Value<DateTime>(Entry["releaseTime"]).ToString("yyyy'/'MM'/'dd HH':'mm");
                }
                else
                {
                    myListItem.Info = Entry["lore"].ToString();
                }
                if (Entry["url"].ToString().Contains("pcl"))
                {
                    myListItem.Info = "[PCL 特供下载] " + myListItem.Info;
                }
                myListItem.Click += OnClick;
                if (IsSaveOnly)
                {
                    myListItem.ContentHandler = new Action<MyListItem, EventArgs>(ModDownloadLib.McDownloadSaveMenuBuild);
                }
                else
                {
                    myListItem.ContentHandler = new Action<MyListItem, EventArgs>(ModDownloadLib.McDownloadMenuBuild);
                }
                return myListItem;
            })
            {
                Height = 42.0
            };
        }

        private static void McDownloadSaveMenuBuild(object sender, EventArgs e)
        {
            MyIconButton myIconButton = new MyIconButton
            {
                LogoScale = 1.05,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                ToolTip = "更新日志"
            };
            myIconButton.Click += ((ModDownloadLib._Closure$__.$IR5-1 == null) ? (ModDownloadLib._Closure$__.$IR5-1 = delegate(object a0, EventArgs a1)
            {
                ModDownloadLib.McDownloadMenuLog(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }) : ModDownloadLib._Closure$__.$IR5-1);
            MyIconButton myIconButton2 = new MyIconButton
            {
                LogoScale = 1.0,
                Logo = "M224 160a64 64 0 0 0-64 64v576a64 64 0 0 0 64 64h576a64 64 0 0 0 64-64V224a64 64 0 0 0-64-64H224z m0 384h576v256H224v-256z m192 96v64h320v-64H416z m-128 0v64h64v-64H288zM224 224h576v256H224V224z m192 96v64h320v-64H416z m-128 0v64h64v-64H288z",
                ToolTip = "下载服务端"
            };
            myIconButton2.Click += ((ModDownloadLib._Closure$__.$IR5-2 == null) ? (ModDownloadLib._Closure$__.$IR5-2 = delegate(object a0, EventArgs a1)
            {
                ModDownloadLib.McDownloadMenuSaveServer(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }) : ModDownloadLib._Closure$__.$IR5-2);
            NewLateBinding.LateSet(sender, null, "Buttons", new object[] { new MyIconButton[] { myIconButton2, myIconButton } }, null, null);
        }

        private static void McDownloadMenuBuild(object sender, EventArgs e)
        {
            MyIconButton myIconButton = new MyIconButton
            {
                Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z",
                ToolTip = "另存为"
            };
            myIconButton.Click += ((ModDownloadLib._Closure$__.$IR6-3 == null) ? (ModDownloadLib._Closure$__.$IR6-3 = delegate(object a0, EventArgs a1)
            {
                ModDownloadLib.McDownloadMenuSave(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }) : ModDownloadLib._Closure$__.$IR6-3);
            MyIconButton myIconButton2 = new MyIconButton
            {
                LogoScale = 1.05,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                ToolTip = "更新日志"
            };
            myIconButton2.Click += ((ModDownloadLib._Closure$__.$IR6-4 == null) ? (ModDownloadLib._Closure$__.$IR6-4 = delegate(object a0, EventArgs a1)
            {
                ModDownloadLib.McDownloadMenuLog(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }) : ModDownloadLib._Closure$__.$IR6-4);
            MyIconButton myIconButton3 = new MyIconButton
            {
                LogoScale = 1.0,
                Logo = "M224 160a64 64 0 0 0-64 64v576a64 64 0 0 0 64 64h576a64 64 0 0 0 64-64V224a64 64 0 0 0-64-64H224z m0 384h576v256H224v-256z m192 96v64h320v-64H416z m-128 0v64h64v-64H288zM224 224h576v256H224V224z m192 96v64h320v-64H416z m-128 0v64h64v-64H288z",
                ToolTip = "下载服务端"
            };
            myIconButton3.Click += ((ModDownloadLib._Closure$__.$IR6-5 == null) ? (ModDownloadLib._Closure$__.$IR6-5 = delegate(object a0, EventArgs a1)
            {
                ModDownloadLib.McDownloadMenuSaveServer(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }) : ModDownloadLib._Closure$__.$IR6-5);
            NewLateBinding.LateSet(sender, null, "Buttons", new object[] { new MyIconButton[] { myIconButton, myIconButton2, myIconButton3 } }, null, null);
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
            MyListItem myListItem;
            if (sender is MyListItem)
            {
                myListItem = (MyListItem)sender;
            }
            else if (NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null) is MyListItem)
            {
                myListItem = (MyListItem)NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null);
            }
            else
            {
                myListItem = (MyListItem)NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Parent", new object[0], null, null, null);
            }
            try
            {
                ModDownloadLib._Closure$__8-0 CS$<>8__locals1 = new ModDownloadLib._Closure$__8-0(CS$<>8__locals1);
                CS$<>8__locals1.$VB$Local_Id = myListItem.Title;
                string text = NewLateBinding.LateIndexGet(myListItem.Tag, new object[] { "url" }, null).ToString();
                CS$<>8__locals1.$VB$Local_VersionFolder = Dialogs.SelectFolder("选择目标文件夹", false, null).FirstOrDefault<string>();
                if (CS$<>8__locals1.$VB$Local_VersionFolder != null)
                {
                    CS$<>8__locals1.$VB$Local_VersionFolder = CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_Id + "\\";
                    try
                    {
                        List<ModLoader.LoaderBase>.Enumerator enumerator = ModLoader.LoaderTaskbar.ToList<ModLoader.LoaderBase>().GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (Operators.CompareString(enumerator.Current.Name, string.Format("Minecraft {0} 服务端下载", CS$<>8__locals1.$VB$Local_Id), false) == 0)
                            {
                                ModMain.Hint("该服务端正在下载中！", ModMain.HintType.Red, true);
                                return;
                            }
                        }
                    }
                    finally
                    {
                        List<ModLoader.LoaderBase>.Enumerator enumerator;
                        ((IDisposable)enumerator).Dispose();
                    }
                    List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
                    list.Add(new ModNet.LoaderDownload("下载版本 JSON 文件", new List<ModNet.NetFile>
                    {
                        new ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(text), CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_Id + ".json", new FileChecker
                        {
                            IsJson = true
                        }, false)
                    })
                    {
                        ProgressWeight = 2.0
                    });
                    list.Add(new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("构建服务端", delegate(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
                    {
                        ModMinecraft.McInstance mcInstance = new ModMinecraft.McInstance(CS$<>8__locals1.$VB$Local_VersionFolder);
                        if (mcInstance.JsonObject["downloads"] == null || mcInstance.JsonObject["downloads"]["server"] == null || mcInstance.JsonObject["downloads"]["server"]["url"] == null)
                        {
                            FileUtils.Delete(CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_Id + ".json", false);
                            if (!DirectoryUtils.GetInfo(CS$<>8__locals1.$VB$Local_VersionFolder).GetFileSystemInfos().Any<FileSystemInfo>())
                            {
                                DirectoryUtils.Delete(CS$<>8__locals1.$VB$Local_VersionFolder, false);
                            }
                            Task.Output = new List<ModNet.NetFile>();
                            ModMain.Hint(string.Format("Mojang 没有给 Minecraft {0} 提供官方服务端下载，没法下，撤退！", CS$<>8__locals1.$VB$Local_Id), ModMain.HintType.Red, true);
                            Thread.Sleep(2000);
                            Task.Cancel();
                            return;
                        }
                        string text2 = (string)mcInstance.JsonObject["downloads"]["server"]["url"];
                        FileChecker fileChecker = new FileChecker
                        {
                            MinSize = 1024L,
                            ActualSize = (long)(mcInstance.JsonObject["downloads"]["server"]["size"] ?? (-1)),
                            Hash = (string)mcInstance.JsonObject["downloads"]["server"]["sha1"]
                        };
                        Task.Output = new List<ModNet.NetFile>
                        {
                            new ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(text2), CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_Id + "-server.jar", fileChecker, false)
                        };
                        string text3 = string.Format("@echo off\r\ntitle {0} 原版服务端\r\necho 如果服务端立即停止，请右键编辑该脚本，将下一行开头的 java 替换为适合该 Minecraft 版本的完整 java.exe 的路径。\r\necho 你可以在 PCL 的 [设置 → 启动选项] 中查看已安装的 java，所需的 java.exe 一般在其中的 bin 文件夹下。\r\necho ------------------------------\r\necho 如果提示 \"You need to agree to the EULA in order to run the server\"，请打开 eula.txt，按说明阅读并同意 Minecraft EULA 后，将该文件最后一行中的 eula=false 改为 eula=true。\r\necho ------------------------------\r\n\"java\" -server -XX:+UseG1GC -Xmx6144M -Xms1024M -jar {1}-server.jar nogui\r\necho ----------------------\r\necho 服务端已停止。\r\npause", CS$<>8__locals1.$VB$Local_Id, CS$<>8__locals1.$VB$Local_Id);
                        FileUtils.Write(CS$<>8__locals1.$VB$Local_VersionFolder + "Launch Server.bat", text3, Encoding.Default.Equals(Encoding.UTF8) ? Encoding.UTF8 : Encoding.GetEncoding("GB18030"));
                        FileUtils.Delete(CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_Id + ".json", false);
                    }, null, ThreadPriority.Normal)
                    {
                        ProgressWeight = 0.5,
                        Show = false
                    });
                    list.Add(new ModNet.LoaderDownload("下载服务端文件", new List<ModNet.NetFile>())
                    {
                        ProgressWeight = 5.0
                    });
                    ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>("Minecraft " + CS$<>8__locals1.$VB$Local_Id + " 服务端下载", list);
                    loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.LoaderStateChangedHintOnly);
                    loaderCombo.Start(CS$<>8__locals1.$VB$Local_Id, false);
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
            MyListItem myListItem;
            if (sender is MyListItem)
            {
                myListItem = (MyListItem)sender;
            }
            else if (NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null) is MyListItem)
            {
                myListItem = (MyListItem)NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null);
            }
            else
            {
                myListItem = (MyListItem)NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Parent", new object[0], null, null, null);
            }
            try
            {
                ModDownloadLib._Closure$__9-0 CS$<>8__locals1 = new ModDownloadLib._Closure$__9-0(CS$<>8__locals1);
                string title = myListItem.Title;
                string text = NewLateBinding.LateIndexGet(myListItem.Tag, new object[] { "url" }, null).ToString();
                CS$<>8__locals1.$VB$Local_VersionFolder = Dialogs.SelectFolder("选择目标文件夹", false, null).FirstOrDefault<string>();
                if (CS$<>8__locals1.$VB$Local_VersionFolder != null)
                {
                    CS$<>8__locals1.$VB$Local_VersionFolder = CS$<>8__locals1.$VB$Local_VersionFolder + title + "\\";
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
                        List<ModLoader.LoaderBase>.Enumerator enumerator;
                        ((IDisposable)enumerator).Dispose();
                    }
                    List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
                    list.Add(new ModNet.LoaderDownload("下载版本 JSON 文件", new List<ModNet.NetFile>
                    {
                        new ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(text), CS$<>8__locals1.$VB$Local_VersionFolder + title + ".json", new FileChecker
                        {
                            IsJson = true
                        }, false)
                    })
                    {
                        ProgressWeight = 2.0
                    });
                    list.Add(new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("分析版本 JAR 文件下载地址", delegate(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
                    {
                        Task.Output = new List<ModNet.NetFile> { ModDownload.DlClientJarGet(new ModMinecraft.McInstance(CS$<>8__locals1.$VB$Local_VersionFolder), false) };
                    }, null, ThreadPriority.Normal)
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
                        List<ModLoader.LoaderBase>.Enumerator enumerator;
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
            ModDownloadLib._Closure$__12-2 CS$<>8__locals1 = new ModDownloadLib._Closure$__12-2(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_Task = Task;
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
                Java java = ModJava.SelectOrDownloadJava(valueRange, true, CS$<>8__locals1.$VB$Local_Task.CreateCancellationToken(), CS$<>8__locals1.$VB$Local_Task.CreateSyncProgressProvider(0.0, 0.2));
                if (!CS$<>8__locals1.$VB$Local_Task.IsCanceled)
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
                        ModDownloadLib._Closure$__12-1 CS$<>8__locals2 = new ModDownloadLib._Closure$__12-1(CS$<>8__locals2);
                        CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1;
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
                        CS$<>8__locals2.$VB$Local_TotalLength = 0;
                        CS$<>8__locals2.$VB$Local_process = new Process
                        {
                            StartInfo = processStartInfo
                        };
                        CS$<>8__locals2.$VB$Local_LastResult = "";
                        ModDownloadLib._Closure$__12-0 CS$<>8__locals3 = new ModDownloadLib._Closure$__12-0(CS$<>8__locals3);
                        CS$<>8__locals3.$VB$NonLocal_$VB$Closure_3 = CS$<>8__locals2;
                        CS$<>8__locals3.$VB$Local_outputWaitHandle = new AutoResetEvent(false);
                        try
                        {
                            ModDownloadLib._Closure$__12-3 CS$<>8__locals4 = new ModDownloadLib._Closure$__12-3(CS$<>8__locals4);
                            CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4 = CS$<>8__locals3;
                            CS$<>8__locals4.$VB$Local_errorWaitHandle = new AutoResetEvent(false);
                            try
                            {
                                CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.OutputDataReceived += delegate(object a0, DataReceivedEventArgs a1)
                                {
                                    base._Lambda$__0(RuntimeHelpers.GetObjectValue(a0), a1);
                                };
                                CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.ErrorDataReceived += delegate(object a0, DataReceivedEventArgs a1)
                                {
                                    base._Lambda$__1(RuntimeHelpers.GetObjectValue(a0), a1);
                                };
                                CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.Start();
                                CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.BeginOutputReadLine();
                                CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.BeginErrorReadLine();
                                while (!CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.HasExited)
                                {
                                    Thread.Sleep(10);
                                }
                                CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$Local_outputWaitHandle.WaitOne(10000);
                                CS$<>8__locals4.$VB$Local_errorWaitHandle.WaitOne(10000);
                                CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.Dispose();
                                if (CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_TotalLength < 1000 || CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_LastResult.Contains("at "))
                                {
                                    throw new Exception("安装器运行出错，末行为 " + CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_LastResult);
                                }
                            }
                            finally
                            {
                                if (CS$<>8__locals4.$VB$Local_errorWaitHandle != null)
                                {
                                    ((IDisposable)CS$<>8__locals4.$VB$Local_errorWaitHandle).Dispose();
                                }
                            }
                        }
                        finally
                        {
                            if (CS$<>8__locals3.$VB$Local_outputWaitHandle != null)
                            {
                                ((IDisposable)CS$<>8__locals3.$VB$Local_outputWaitHandle).Dispose();
                            }
                        }
                    }
                }
            }
        }

        private static List<ModLoader.LoaderBase> McDownloadOptiFineLoader(ModDownload.DlOptiFineListEntry DownloadInfo, string McFolder, ModLoader.LoaderCombo<string> ClientDownloadLoader, string ClientFolder)
        {
            ModDownloadLib._Closure$__13-0 CS$<>8__locals1 = new ModDownloadLib._Closure$__13-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_DownloadInfo = DownloadInfo;
            CS$<>8__locals1.$VB$Local_McFolder = McFolder;
            CS$<>8__locals1.$VB$Local_ClientDownloadLoader = ClientDownloadLoader;
            CS$<>8__locals1.$VB$Local_ClientFolder = ClientFolder;
            CS$<>8__locals1.$VB$Local_Id = CS$<>8__locals1.$VB$Local_DownloadInfo.InstanceName;
            CS$<>8__locals1.$VB$Local_VersionFolder = CS$<>8__locals1.$VB$Local_McFolder + "versions\\" + CS$<>8__locals1.$VB$Local_Id + "\\";
            bool flag = CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit.Contains("w") || McVersion.VersionToDrop(CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit) >= 140;
            CS$<>8__locals1.$VB$Local_Target = (flag ? string.Format("{0}OptiFine.jar", ModMain.RequestTaskTempFolder(true)) : string.Format("{0}libraries\\optifine\\OptiFine\\{1}\\{2}", CS$<>8__locals1.$VB$Local_McFolder, PathUtils.GetFileNameWithoutExtension(CS$<>8__locals1.$VB$Local_DownloadInfo.FileName).Replace("OptiFine_", "").Replace("preview_", ""), CS$<>8__locals1.$VB$Local_DownloadInfo.FileName.Replace("OptiFine_", "OptiFine-").Replace("preview_", "")));
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            list.Add(new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("获取 OptiFine 主文件下载地址", delegate(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                List<string> list2 = new List<string>();
                string text = CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit;
                if (Operators.CompareString(text, "1.8", false) == 0 || Operators.CompareString(text, "1.9", false) == 0)
                {
                    text += ".0";
                }
                if (CS$<>8__locals1.$VB$Local_DownloadInfo.IsPreview)
                {
                    list2.Add("https://bmclapi2.bangbang93.com/optifine/" + text + "/HD_U_" + CS$<>8__locals1.$VB$Local_DownloadInfo.DisplayName.Replace(CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit + " ", "").Replace(" ", "/"));
                }
                else
                {
                    list2.Add("https://bmclapi2.bangbang93.com/optifine/" + text + "/HD_U/" + CS$<>8__locals1.$VB$Local_DownloadInfo.DisplayName.Replace(CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit + " ", ""));
                }
                try
                {
                    string text2 = ModNet.NetRequestByClientRetry("https://optifine.net/adloadx?f=" + CS$<>8__locals1.$VB$Local_DownloadInfo.FileName, null, null, null, null, "text/html", null, false, new UTF8Encoding(false), true);
                    Task.Progress = 0.8;
                    list2.Add("https://optifine.net/" + text2.RegexSearch("downloadx\\?f=[^\"']+", RegexOptions.None).First<string>());
                    Logger.Info(string.Format("OptiFine {0} 官方下载地址：{1}", CS$<>8__locals1.$VB$Local_DownloadInfo.DisplayName, list2.Last<string>()), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, string.Format("获取 OptiFine {0} 官方下载地址失败", CS$<>8__locals1.$VB$Local_DownloadInfo.DisplayName), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                Task.Output = new List<ModNet.NetFile>
                {
                    new ModNet.NetFile(list2.ToArray(), CS$<>8__locals1.$VB$Local_Target, new FileChecker
                    {
                        MinSize = 307200L
                    }, false)
                };
            }, null, ThreadPriority.Normal)
            {
                ProgressWeight = 8.0
            });
            list.Add(new ModNet.LoaderDownload("下载 OptiFine 主文件", new List<ModNet.NetFile>())
            {
                ProgressWeight = 8.0
            });
            list.Add(new ModLoader.LoaderTask<List<ModNet.NetFile>, bool>("等待原版下载", delegate(ModLoader.LoaderTask<List<ModNet.NetFile>, bool> Task)
            {
                List<ModLoader.LoaderBase> list3 = CS$<>8__locals1.$VB$Local_ClientDownloadLoader.GetLoaderList(true).Where<ModLoader.LoaderBase>((ModDownloadLib._Closure$__.$I13-2 == null) ? (ModDownloadLib._Closure$__.$I13-2 = (ModLoader.LoaderBase l) => Operators.CompareString(l.Name, "下载原版支持库文件", false) == 0 || Operators.CompareString(l.Name, "下载原版 json 文件", false) == 0) : ModDownloadLib._Closure$__.$I13-2).Where<ModLoader.LoaderBase>((ModDownloadLib._Closure$__.$I13-3 == null) ? (ModDownloadLib._Closure$__.$I13-3 = (ModLoader.LoaderBase l) => l.State != ModBase.LoadState.Finished) : ModDownloadLib._Closure$__.$I13-3)
                    .ToList<ModLoader.LoaderBase>();
                if (list3.Any<ModLoader.LoaderBase>())
                {
                    Logger.Info("OptiFine 安装正在等待原版文件下载完成", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                while (list3.Any<ModLoader.LoaderBase>() && !Task.IsCanceled)
                {
                    list3 = list3.Where<ModLoader.LoaderBase>((ModDownloadLib._Closure$__.$I13-4 == null) ? (ModDownloadLib._Closure$__.$I13-4 = (ModLoader.LoaderBase l) => l.State != ModBase.LoadState.Finished) : ModDownloadLib._Closure$__.$I13-4).ToList<ModLoader.LoaderBase>();
                    Thread.Sleep(50);
                }
                if (!Task.IsCanceled)
                {
                    object vanillaSyncLock = ModDownloadLib.VanillaSyncLock;
                    ObjectFlowControl.CheckForSyncLockOnValueType(vanillaSyncLock);
                    lock (vanillaSyncLock)
                    {
                        string lastPart = PathUtils.GetLastPart(CS$<>8__locals1.$VB$Local_ClientFolder);
                        DirectoryUtils.Create(CS$<>8__locals1.$VB$Local_McFolder + "versions\\" + CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit);
                        if (!FileUtils.Exists(string.Concat(new string[]
                        {
                            CS$<>8__locals1.$VB$Local_McFolder,
                            "versions\\",
                            CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit,
                            "\\",
                            CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit,
                            ".json"
                        })))
                        {
                            FileUtils.Copy(string.Format("{0}{1}.json", CS$<>8__locals1.$VB$Local_ClientFolder, lastPart), string.Format("{0}versions\\{1}\\{2}.json", CS$<>8__locals1.$VB$Local_McFolder, CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit));
                        }
                        if (!FileUtils.Exists(string.Concat(new string[]
                        {
                            CS$<>8__locals1.$VB$Local_McFolder,
                            "versions\\",
                            CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit,
                            "\\",
                            CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit,
                            ".jar"
                        })))
                        {
                            FileUtils.Copy(string.Format("{0}{1}.jar", CS$<>8__locals1.$VB$Local_ClientFolder, lastPart), string.Format("{0}versions\\{1}\\{2}.jar", CS$<>8__locals1.$VB$Local_McFolder, CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit));
                        }
                    }
                }
            }, null, ThreadPriority.Normal)
            {
                ProgressWeight = 0.1,
                Show = false
            });
            if (flag)
            {
                Logger.Info(string.Format("检测为新版 OptiFine：{0}", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                list.Add(new ModLoader.LoaderTask<List<ModNet.NetFile>, bool>("安装 OptiFine（方式 A）", delegate(ModLoader.LoaderTask<List<ModNet.NetFile>, bool> Task)
                {
                    string text3 = ModMain.RequestTaskTempFolder(true);
                    string text4 = text3 + ".minecraft\\";
                    try
                    {
                        if (DirectoryUtils.Exists(text4 + "versions\\" + CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit))
                        {
                            DirectoryUtils.Delete(text4 + "versions\\" + CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, false);
                        }
                        DirectoryUtils.Create(text4 + "versions\\" + CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit + "\\");
                        ModMinecraft.McFolderLauncherProfilesJsonCreate(text4);
                        FileUtils.Copy(string.Concat(new string[]
                        {
                            CS$<>8__locals1.$VB$Local_McFolder,
                            "versions\\",
                            CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit,
                            "\\",
                            CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit,
                            ".json"
                        }), string.Concat(new string[]
                        {
                            text4,
                            "versions\\",
                            CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit,
                            "\\",
                            CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit,
                            ".json"
                        }));
                        FileUtils.Copy(string.Concat(new string[]
                        {
                            CS$<>8__locals1.$VB$Local_McFolder,
                            "versions\\",
                            CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit,
                            "\\",
                            CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit,
                            ".jar"
                        }), string.Concat(new string[]
                        {
                            text4,
                            "versions\\",
                            CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit,
                            "\\",
                            CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit,
                            ".jar"
                        }));
                        Task.Progress = 0.06;
                        bool flag3 = true;
                        for (;;)
                        {
                            try
                            {
                                ModDownloadLib.McDownloadOptiFineInstall(text3, CS$<>8__locals1.$VB$Local_Target, Task, flag3);
                            }
                            catch (Exception ex2)
                            {
                                if (flag3)
                                {
                                    Logger.Warn(ex2, "使用 JavaWrapper 安装 OptiFine 失败，将不使用 JavaWrapper 并重试", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                                    flag3 = false;
                                    continue;
                                }
                                throw new Exception("运行 OptiFine 安装器失败", ex2);
                            }
                            break;
                        }
                        Task.Progress = 0.96;
                        FileUtils.Delete(text4 + "launcher_profiles.json", false);
                        DirectoryUtils.Copy(text4, CS$<>8__locals1.$VB$Local_McFolder);
                        Task.Progress = 0.98;
                        FileUtils.Delete(CS$<>8__locals1.$VB$Local_Target, false);
                        DirectoryUtils.Delete(text3, false);
                    }
                    catch (Exception ex3)
                    {
                        throw new Exception("安装 OptiFine（方式 A）失败", ex3);
                    }
                }, null, ThreadPriority.Normal)
                {
                    ProgressWeight = 8.0
                });
            }
            else
            {
                Logger.Info(string.Format("检测为旧版 OptiFine：{0}", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                list.Add(new ModLoader.LoaderTask<List<ModNet.NetFile>, bool>("安装 OptiFine（方式 B）", delegate(ModLoader.LoaderTask<List<ModNet.NetFile>, bool> Task)
                {
                    try
                    {
                        FileUtils.Copy(string.Concat(new string[]
                        {
                            CS$<>8__locals1.$VB$Local_McFolder,
                            "versions\\",
                            CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit,
                            "\\",
                            CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit,
                            ".jar"
                        }), CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_Id + ".jar");
                        Task.Progress = 0.7;
                        ModMinecraft.McInstance mcInstance = new ModMinecraft.McInstance(CS$<>8__locals1.$VB$Local_McFolder + "versions\\" + CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit);
                        string text5 = string.Concat(new string[]
                        {
                            "{\r\n    \"id\": \"",
                            CS$<>8__locals1.$VB$Local_Id,
                            "\",\r\n    \"inheritsFrom\": \"",
                            CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit,
                            "\",\r\n    \"time\": \"",
                            (Operators.CompareString(CS$<>8__locals1.$VB$Local_DownloadInfo.ReleaseTime, "", false) == 0) ? mcInstance.ReleaseTime.ToString("yyyy'-'MM'-'dd") : CS$<>8__locals1.$VB$Local_DownloadInfo.ReleaseTime.Replace("/", "-"),
                            "T23:33:33+08:00\",\r\n    \"releaseTime\": \"",
                            (Operators.CompareString(CS$<>8__locals1.$VB$Local_DownloadInfo.ReleaseTime, "", false) == 0) ? mcInstance.ReleaseTime.ToString("yyyy'-'MM'-'dd") : CS$<>8__locals1.$VB$Local_DownloadInfo.ReleaseTime.Replace("/", "-"),
                            "T23:33:33+08:00\",\r\n    \"type\": \"release\",\r\n    \"libraries\": [\r\n        {\"name\": \"optifine:OptiFine:",
                            PathUtils.GetFileNameWithoutExtension(CS$<>8__locals1.$VB$Local_DownloadInfo.FileName).Replace("OptiFine_", "").Replace("preview_", ""),
                            "\"},\r\n        {\"name\": \"net.minecraft:launchwrapper:1.12\"}\r\n    ],\r\n    \"mainClass\": \"net.minecraft.launchwrapper.Launch\","
                        });
                        Task.Progress = 0.8;
                        if (mcInstance.IsOldJson)
                        {
                            text5 = text5 + "\r\n    \"minimumLauncherVersion\": 18,\r\n    \"minecraftArguments\": \"" + mcInstance.JsonObject["minecraftArguments"].ToString() + "  --tweakClass optifine.OptiFineTweaker\"\r\n}";
                        }
                        else
                        {
                            text5 += "\r\n    \"minimumLauncherVersion\": \"21\",\r\n    \"arguments\": {\r\n        \"game\": [\r\n            \"--tweakClass\",\r\n            \"optifine.OptiFineTweaker\"\r\n        ]\r\n    }\r\n}";
                        }
                        FileUtils.Write(CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_Id + ".json", text5, null);
                    }
                    catch (Exception ex4)
                    {
                        throw new Exception("安装 OptiFine（方式 B）失败", ex4);
                    }
                }, null, ThreadPriority.Normal)
                {
                    ProgressWeight = 1.0
                });
            }
            return list;
        }

        private static List<ModLoader.LoaderBase> McDownloadOptiFineSaveLoader(ModDownload.DlOptiFineListEntry DownloadInfo, string TargetFolder)
        {
            return new List<ModLoader.LoaderBase>
            {
                new ModLoader.LoaderTask<ModDownload.DlOptiFineListEntry, List<ModNet.NetFile>>("获取 OptiFine 下载地址", delegate(ModLoader.LoaderTask<ModDownload.DlOptiFineListEntry, List<ModNet.NetFile>> Task)
                {
                    List<string> list = new List<string>();
                    string text = DownloadInfo.Inherit;
                    if (Operators.CompareString(text, "1.8", false) == 0 || Operators.CompareString(text, "1.9", false) == 0)
                    {
                        text += ".0";
                    }
                    if (DownloadInfo.IsPreview)
                    {
                        list.Add("https://bmclapi2.bangbang93.com/optifine/" + text + "/HD_U_" + DownloadInfo.DisplayName.Replace(DownloadInfo.Inherit + " ", "").Replace(" ", "/"));
                    }
                    else
                    {
                        list.Add("https://bmclapi2.bangbang93.com/optifine/" + text + "/HD_U/" + DownloadInfo.DisplayName.Replace(DownloadInfo.Inherit + " ", ""));
                    }
                    try
                    {
                        string text2 = ModNet.NetRequestByClientRetry("https://optifine.net/adloadx?f=" + DownloadInfo.FileName, null, null, null, null, "text/html", null, false, new UTF8Encoding(false), true);
                        Task.Progress = 0.8;
                        list.Add("https://optifine.net/" + text2.RegexSearch("downloadx\\?f=[^\"']+", RegexOptions.None).First<string>());
                        Logger.Info(string.Format("OptiFine {0} 官方下载地址：{1}", DownloadInfo.DisplayName, list.Last<string>()), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, string.Format("获取 OptiFine {0} 官方下载地址失败", DownloadInfo.DisplayName), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                    }
                    Task.Progress = 0.9;
                    Task.Output = new List<ModNet.NetFile>
                    {
                        new ModNet.NetFile(list.ToArray(), TargetFolder, new FileChecker
                        {
                            MinSize = 65536L
                        }, false)
                    };
                }, null, ThreadPriority.Normal)
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

        public static MyVirtualizingElement<MyListItem> OptiFineDownloadListItem(ModDownload.DlOptiFineListEntry Entry, MyListItem.ClickEventHandler OnClick, bool IsSaveOnly)
        {
            return new MyVirtualizingElement<MyListItem>(delegate
            {
                MyListItem myListItem = new MyListItem
                {
                    Title = Entry.DisplayName,
                    SnapsToDevicePixels = true,
                    Height = 42.0,
                    Type = MyListItem.CheckType.Clickable,
                    Tag = Entry,
                    Info = (Entry.IsPreview ? "测试版" : "正式版") + ((Operators.CompareString(Entry.ReleaseTime, "", false) == 0) ? "" : ("，发布于 " + Entry.ReleaseTime)) + ((Entry.RequiredForgeVersion == null) ? "，不兼容 Forge" : ((Operators.CompareString(Entry.RequiredForgeVersion, "", false) == 0) ? "" : ("，兼容 Forge " + Entry.RequiredForgeVersion))),
                    Logo = ModBase.PathImage + "Blocks/GrassPath.png"
                };
                myListItem.Click += OnClick;
                if (IsSaveOnly)
                {
                    myListItem.ContentHandler = new Action<MyListItem, EventArgs>(ModDownloadLib.OptiFineSaveContMenuBuild);
                }
                else
                {
                    myListItem.ContentHandler = new Action<MyListItem, EventArgs>(ModDownloadLib.OptiFineContMenuBuild);
                }
                return myListItem;
            })
            {
                Height = 42.0
            };
        }

        private static void OptiFineSaveContMenuBuild(object sender, EventArgs e)
        {
            MyIconButton myIconButton = new MyIconButton
            {
                LogoScale = 1.05,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                ToolTip = "更新日志"
            };
            myIconButton.Click += ((ModDownloadLib._Closure$__.$IR16-8 == null) ? (ModDownloadLib._Closure$__.$IR16-8 = delegate(object a0, EventArgs a1)
            {
                ModDownloadLib.OptiFineLog_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }) : ModDownloadLib._Closure$__.$IR16-8);
            NewLateBinding.LateSet(sender, null, "Buttons", new object[] { new MyIconButton[] { myIconButton } }, null, null);
        }

        private static void OptiFineContMenuBuild(object sender, EventArgs e)
        {
            MyIconButton myIconButton = new MyIconButton
            {
                Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z",
                ToolTip = "另存为"
            };
            myIconButton.Click += ((ModDownloadLib._Closure$__.$IR17-9 == null) ? (ModDownloadLib._Closure$__.$IR17-9 = delegate(object a0, EventArgs a1)
            {
                ModDownloadLib.OptiFineSave_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }) : ModDownloadLib._Closure$__.$IR17-9);
            MyIconButton myIconButton2 = new MyIconButton
            {
                LogoScale = 1.05,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                ToolTip = "更新日志"
            };
            myIconButton2.Click += ((ModDownloadLib._Closure$__.$IR17-10 == null) ? (ModDownloadLib._Closure$__.$IR17-10 = delegate(object a0, EventArgs a1)
            {
                ModDownloadLib.OptiFineLog_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }) : ModDownloadLib._Closure$__.$IR17-10);
            NewLateBinding.LateSet(sender, null, "Buttons", new object[] { new MyIconButton[] { myIconButton, myIconButton2 } }, null, null);
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
                        List<ModLoader.LoaderBase>.Enumerator enumerator;
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
            return new List<ModLoader.LoaderBase>
            {
                new ModLoader.LoaderTask<string, string>("安装 LiteLoader", delegate(ModLoader.LoaderTask<string, string> Task)
                {
                    try
                    {
                        string text = DownloadInfo.Inherit + "-LiteLoader";
                        string text2 = McFolder + "versions\\" + text + "\\";
                        JObject jobject = new JObject();
                        jobject.Add("id", text);
                        jobject.Add("time", DateTime.ParseExact(DownloadInfo.ReleaseTime, "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture));
                        jobject.Add("releaseTime", DateTime.ParseExact(DownloadInfo.ReleaseTime, "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture));
                        jobject.Add("type", "release");
                        jobject.Add("arguments", ("{\"game\":[\"--tweakClass\",\"" + DownloadInfo.JsonToken["tweakClass"].ToString() + "\"]}").DeserializeJson());
                        jobject.Add("libraries", DownloadInfo.JsonToken["libraries"]);
                        ((JContainer)jobject["libraries"]).Add(("{\"name\": \"com.mumfrey:liteloader:" + DownloadInfo.JsonToken["version"].ToString() + "\",\"url\": \"https://dl.liteloader.com/versions/\"}").DeserializeJson());
                        jobject.Add("mainClass", "net.minecraft.launchwrapper.Launch");
                        jobject.Add("minimumLauncherVersion", 18);
                        jobject.Add("inheritsFrom", DownloadInfo.Inherit);
                        jobject.Add("jar", DownloadInfo.Inherit);
                        FileUtils.Write(text2 + text + ".json", jobject.ToString(), null);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("安装新 LiteLoader 版本失败", ex);
                    }
                }, null, ThreadPriority.Normal)
                {
                    ProgressWeight = 1.0
                }
            };
        }

        public static MyVirtualizingElement<MyListItem> LiteLoaderDownloadListItem(ModDownload.DlLiteLoaderListEntry Entry, MyListItem.ClickEventHandler OnClick, bool IsSaveOnly)
        {
            return new MyVirtualizingElement<MyListItem>(delegate
            {
                MyListItem myListItem = new MyListItem
                {
                    Title = Entry.Inherit,
                    SnapsToDevicePixels = true,
                    Height = 42.0,
                    Type = MyListItem.CheckType.Clickable,
                    Tag = Entry,
                    Info = (Entry.IsPreview ? "测试版" : "稳定版") + ((Operators.CompareString(Entry.ReleaseTime, "", false) == 0) ? "" : ("，发布于 " + Entry.ReleaseTime)),
                    Logo = ModBase.PathImage + "Blocks/Egg.png"
                };
                myListItem.Click += OnClick;
                if (IsSaveOnly)
                {
                    myListItem.ContentHandler = new Action<MyListItem, EventArgs>(ModDownloadLib.LiteLoaderSaveContMenuBuild);
                }
                else
                {
                    myListItem.ContentHandler = new Action<MyListItem, EventArgs>(ModDownloadLib.LiteLoaderContMenuBuild);
                }
                return myListItem;
            })
            {
                Height = 42.0
            };
        }

        private static void LiteLoaderSaveContMenuBuild(MyListItem sender, EventArgs e)
        {
            if (Conversions.ToBoolean(NewLateBinding.LateGet(sender.Tag, null, "IsLegacy", new object[0], null, null, null)))
            {
                sender.Buttons = new MyIconButton[0];
                return;
            }
            MyIconButton myIconButton = new MyIconButton
            {
                Logo = "M384 128h640v128H384zM160 192m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0ZM384 448h640v128H384zM160 512m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0ZM384 768h640v128H384zM160 832m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0Z",
                ToolTip = "查看全部版本",
                Tag = sender
            };
            myIconButton.Click += ((ModDownloadLib._Closure$__.$IR23-11 == null) ? (ModDownloadLib._Closure$__.$IR23-11 = delegate(object a0, EventArgs a1)
            {
                ModDownloadLib.LiteLoaderAll_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }) : ModDownloadLib._Closure$__.$IR23-11);
            sender.Buttons = new MyIconButton[] { myIconButton };
        }

        private static void LiteLoaderContMenuBuild(MyListItem sender, EventArgs e)
        {
            MyIconButton myIconButton = new MyIconButton
            {
                Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z",
                ToolTip = "保存安装器",
                Tag = sender
            };
            myIconButton.Click += ((ModDownloadLib._Closure$__.$IR24-12 == null) ? (ModDownloadLib._Closure$__.$IR24-12 = delegate(object a0, EventArgs a1)
            {
                ModDownloadLib.LiteLoaderSave_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }) : ModDownloadLib._Closure$__.$IR24-12);
            if (Conversions.ToBoolean(NewLateBinding.LateGet(sender.Tag, null, "IsLegacy", new object[0], null, null, null)))
            {
                sender.Buttons = new MyIconButton[] { myIconButton };
                return;
            }
            MyIconButton myIconButton2 = new MyIconButton
            {
                Logo = "M384 128h640v128H384zM160 192m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0ZM384 448h640v128H384zM160 512m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0ZM384 768h640v128H384zM160 832m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0Z",
                ToolTip = "查看全部版本",
                Tag = sender
            };
            myIconButton2.Click += ((ModDownloadLib._Closure$__.$IR24-13 == null) ? (ModDownloadLib._Closure$__.$IR24-13 = delegate(object a0, EventArgs a1)
            {
                ModDownloadLib.LiteLoaderAll_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }) : ModDownloadLib._Closure$__.$IR24-13);
            sender.Buttons = new MyIconButton[] { myIconButton, myIconButton2 };
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
                        List<ModLoader.LoaderBase>.Enumerator enumerator;
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
            ModDownloadLib._Closure$__28-2 CS$<>8__locals1 = new ModDownloadLib._Closure$__28-2(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_Task = Task;
            Java java = ModJava.SelectOrDownloadJava(ValueRange<Version>.AtLeast(new Version(8, 0, 60)), true, CS$<>8__locals1.$VB$Local_Task.CreateCancellationToken(), CS$<>8__locals1.$VB$Local_Task.CreateSyncProgressProvider(0.0, 0.2));
            checked
            {
                if (!CS$<>8__locals1.$VB$Local_Task.IsCanceled)
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
                        ModDownloadLib._Closure$__28-1 CS$<>8__locals2 = new ModDownloadLib._Closure$__28-1(CS$<>8__locals2);
                        CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1;
                        ProcessStartInfo processStartInfo = new ProcessStartInfo
                        {
                            FileName = java.JavaExePath,
                            Arguments = text,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true
                        };
                        CS$<>8__locals2.$VB$Local_LoaderName = (IsNeoForge ? "NeoForge" : "Forge");
                        Logger.Info(string.Format("开始安装 {0}：{1}", CS$<>8__locals2.$VB$Local_LoaderName, text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        CS$<>8__locals2.$VB$Local_process = new Process
                        {
                            StartInfo = processStartInfo
                        };
                        CS$<>8__locals2.$VB$Local_LastResults = new Queue<string>();
                        ModDownloadLib._Closure$__28-0 CS$<>8__locals3 = new ModDownloadLib._Closure$__28-0(CS$<>8__locals3);
                        CS$<>8__locals3.$VB$NonLocal_$VB$Closure_3 = CS$<>8__locals2;
                        CS$<>8__locals3.$VB$Local_outputWaitHandle = new AutoResetEvent(false);
                        try
                        {
                            ModDownloadLib._Closure$__28-3 CS$<>8__locals4 = new ModDownloadLib._Closure$__28-3(CS$<>8__locals4);
                            CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4 = CS$<>8__locals3;
                            CS$<>8__locals4.$VB$Local_errorWaitHandle = new AutoResetEvent(false);
                            try
                            {
                                CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.OutputDataReceived += delegate(object a0, DataReceivedEventArgs a1)
                                {
                                    base._Lambda$__0(RuntimeHelpers.GetObjectValue(a0), a1);
                                };
                                CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.ErrorDataReceived += delegate(object a0, DataReceivedEventArgs a1)
                                {
                                    base._Lambda$__1(RuntimeHelpers.GetObjectValue(a0), a1);
                                };
                                CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.Start();
                                CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.BeginOutputReadLine();
                                CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.BeginErrorReadLine();
                                while (!CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.HasExited)
                                {
                                    Thread.Sleep(10);
                                }
                                CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$Local_outputWaitHandle.WaitOne(10000);
                                CS$<>8__locals4.$VB$Local_errorWaitHandle.WaitOne(10000);
                                CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.Dispose();
                                if (!CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_LastResults.Reverse<string>().Take<string>(5).Any<string>((ModDownloadLib._Closure$__.$I28-2 == null) ? (ModDownloadLib._Closure$__.$I28-2 = (string l) => Operators.CompareString(l, "true", false) == 0) : ModDownloadLib._Closure$__.$I28-2))
                                {
                                    Logger.Warn(CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_LastResults.Join("\r\n"), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                                    string text2 = "";
                                    int num = Math.Max(0, CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_LastResults.Count - 5);
                                    int num2 = CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_LastResults.Count - 1;
                                    for (int i = num; i <= num2; i++)
                                    {
                                        text2 = text2 + "\r\n" + CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_LastResults.ElementAtOrDefault<string>(i);
                                    }
                                    throw new Exception(string.Format("{0} 安装器出错，日志结束部分为：", CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_LoaderName) + text2);
                                }
                            }
                            finally
                            {
                                if (CS$<>8__locals4.$VB$Local_errorWaitHandle != null)
                                {
                                    ((IDisposable)CS$<>8__locals4.$VB$Local_errorWaitHandle).Dispose();
                                }
                            }
                        }
                        finally
                        {
                            if (CS$<>8__locals3.$VB$Local_outputWaitHandle != null)
                            {
                                ((IDisposable)CS$<>8__locals3.$VB$Local_outputWaitHandle).Dispose();
                            }
                        }
                    }
                }
            }
        }

        private static void ForgelikeInjectorLine(string Content, ModLoader.LoaderTask<bool, bool> Task)
        {
            uint num = <PrivateImplementationDetails>.ComputeStringHash(Content);
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
            ModDownloadLib._Closure$__30-0 CS$<>8__locals1 = new ModDownloadLib._Closure$__30-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_IsNeoForge = IsNeoForge;
            CS$<>8__locals1.$VB$Local_LoaderVersion = LoaderVersion;
            CS$<>8__locals1.$VB$Local_NewInstanceName = NewInstanceName;
            CS$<>8__locals1.$VB$Local_Inherit = Inherit;
            CS$<>8__locals1.$VB$Local_Info = Info;
            CS$<>8__locals1.$VB$Local_McFolder = McFolder;
            CS$<>8__locals1.$VB$Local_ClientDownloadLoader = ClientDownloadLoader;
            CS$<>8__locals1.$VB$Local_ClientFolder = ClientFolder;
            if (CS$<>8__locals1.$VB$Local_IsNeoForge && CS$<>8__locals1.$VB$Local_Info == null)
            {
                if (Operators.CompareString(CS$<>8__locals1.$VB$Local_Inherit, "1.20.1", false) == 0 && !CS$<>8__locals1.$VB$Local_LoaderVersion.StartsWithF("1.20.1-", false))
                {
                    CS$<>8__locals1.$VB$Local_Info = new ModDownload.DlNeoForgeListEntry("1.20.1-" + CS$<>8__locals1.$VB$Local_LoaderVersion);
                }
                else
                {
                    CS$<>8__locals1.$VB$Local_Info = new ModDownload.DlNeoForgeListEntry(CS$<>8__locals1.$VB$Local_LoaderVersion);
                }
            }
            if (!CS$<>8__locals1.$VB$Local_IsNeoForge && CS$<>8__locals1.$VB$Local_LoaderVersion.StartsWithF("1.", false) && CS$<>8__locals1.$VB$Local_LoaderVersion.Contains("-"))
            {
                CS$<>8__locals1.$VB$Local_Inherit = CS$<>8__locals1.$VB$Local_LoaderVersion.BeforeFirst("-", false);
                CS$<>8__locals1.$VB$Local_LoaderVersion = CS$<>8__locals1.$VB$Local_LoaderVersion.AfterLast("-", false);
            }
            CS$<>8__locals1.$VB$Local_LoaderName = (CS$<>8__locals1.$VB$Local_IsNeoForge ? "NeoForge" : "Forge");
            CS$<>8__locals1.$VB$Local_InstallerAddress = ModMain.RequestTaskTempFolder(true) + "forge_installer.jar";
            CS$<>8__locals1.$VB$Local_VersionFolder = string.Format("{0}versions\\{1}\\", CS$<>8__locals1.$VB$Local_McFolder, CS$<>8__locals1.$VB$Local_NewInstanceName);
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            if (CS$<>8__locals1.$VB$Local_Info == null)
            {
                list.Add(new ModLoader.LoaderTask<string, string>(string.Format("获取 {0} 详细信息", CS$<>8__locals1.$VB$Local_LoaderName), delegate(ModLoader.LoaderTask<string, string> Task)
                {
                    ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>> loaderTask = new ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>("McDownloadForgeLoader " + CS$<>8__locals1.$VB$Local_Inherit, new Action<ModLoader.LoaderTask<string, List<ModDownload.DlForgeVersionEntry>>>(ModDownload.DlForgeVersionMain), null, ThreadPriority.Normal);
                    loaderTask.WaitForExit(CS$<>8__locals1.$VB$Local_Inherit, null, false, default(CancellationToken), null);
                    Task.Progress = 0.8;
                    try
                    {
                        foreach (ModDownload.DlForgeVersionEntry dlForgeVersionEntry in loaderTask.Output)
                        {
                            if (McUtils.CompareVersion(dlForgeVersionEntry.Version.ToString(), CS$<>8__locals1.$VB$Local_LoaderVersion) == 0)
                            {
                                CS$<>8__locals1.$VB$Local_Info = dlForgeVersionEntry;
                                return;
                            }
                        }
                    }
                    finally
                    {
                        List<ModDownload.DlForgeVersionEntry>.Enumerator enumerator;
                        ((IDisposable)enumerator).Dispose();
                    }
                    throw new Exception(string.Concat(new string[]
                    {
                        string.Format("未能找到 {0} ", CS$<>8__locals1.$VB$Local_LoaderName),
                        CS$<>8__locals1.$VB$Local_Inherit,
                        "-",
                        CS$<>8__locals1.$VB$Local_LoaderVersion,
                        " 的详细信息！"
                    }));
                }, null, ThreadPriority.Normal)
                {
                    ProgressWeight = 3.0
                });
            }
            list.Add(new ModLoader.LoaderTask<string, List<ModNet.NetFile>>(string.Format("准备下载 {0}", CS$<>8__locals1.$VB$Local_LoaderName), delegate(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                List<ModNet.NetFile> list2 = new List<ModNet.NetFile>();
                if (CS$<>8__locals1.$VB$Local_Info.IsNeoForge)
                {
                    string text = ((ModDownload.DlNeoForgeListEntry)CS$<>8__locals1.$VB$Local_Info).UrlBase + "-installer.jar";
                    list2.Add(new ModNet.NetFile(new string[]
                    {
                        text.Replace("maven.neoforged.net/releases", "bmclapi2.bangbang93.com/maven"),
                        text
                    }, CS$<>8__locals1.$VB$Local_InstallerAddress, new FileChecker
                    {
                        MinSize = 65536L
                    }, false));
                }
                else
                {
                    ModDownload.DlForgeVersionEntry dlForgeVersionEntry2 = (ModDownload.DlForgeVersionEntry)CS$<>8__locals1.$VB$Local_Info;
                    string text2 = string.Format("{0}-{1}/forge-{2}-{3}-{4}.{5}", new object[]
                    {
                        dlForgeVersionEntry2.Inherit.Replace("-", "_"),
                        dlForgeVersionEntry2.FileVersion,
                        dlForgeVersionEntry2.Inherit.Replace("-", "_"),
                        dlForgeVersionEntry2.FileVersion,
                        dlForgeVersionEntry2.Category,
                        dlForgeVersionEntry2.FileExtension
                    });
                    list2.Add(new ModNet.NetFile(new string[]
                    {
                        string.Format("https://bmclapi2.bangbang93.com/maven/net/minecraftforge/forge/{0}", text2),
                        string.Format("https://files.minecraftforge.net/maven/net/minecraftforge/forge/{0}", text2)
                    }, CS$<>8__locals1.$VB$Local_InstallerAddress, new FileChecker
                    {
                        MinSize = 65536L,
                        Hash = dlForgeVersionEntry2.Hash
                    }, false));
                }
                Task.Output = list2;
            }, null, ThreadPriority.Normal)
            {
                ProgressWeight = 0.5,
                Show = false
            });
            list.Add(new ModNet.LoaderDownload(string.Format("下载 {0} 主文件", CS$<>8__locals1.$VB$Local_LoaderName), new List<ModNet.NetFile>())
            {
                ProgressWeight = 9.0
            });
            CS$<>8__locals1.$VB$Local_CurrentMcFolder = ModMinecraft.McFolderSelected;
            if (CS$<>8__locals1.$VB$Local_IsNeoForge || Conversions.ToDouble(CS$<>8__locals1.$VB$Local_LoaderVersion.BeforeFirst(".", false)) >= 20.0)
            {
                ModDownloadLib._Closure$__30-1 CS$<>8__locals2 = new ModDownloadLib._Closure$__30-1(CS$<>8__locals2);
                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1;
                Logger.Info(string.Format("检测为{0}Forge：{1}", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_IsNeoForge ? " Neo" : "新版 ", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderVersion), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                CS$<>8__locals2.$VB$Local_Libs = null;
                list.Add(new ModLoader.LoaderTask<string, List<ModNet.NetFile>>(string.Format("分析 {0} 支持库文件", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName), checked(delegate(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
                {
                    Task.Output = new List<ModNet.NetFile>();
                    ZipArchive zipArchive = null;
                    try
                    {
                        zipArchive = FileUtils.OpenZip(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_InstallerAddress, default(CancellationToken));
                        Task.Progress = 0.2;
                        JObject jobject = (JObject)zipArchive.GetEntry("install_profile.json").Open().ReadString(null)
                            .DeserializeJson();
                        JObject jobject2 = (JObject)zipArchive.GetEntry("version.json").Open().ReadString(null)
                            .DeserializeJson();
                        jobject.Merge(jobject2);
                        CS$<>8__locals2.$VB$Local_Libs = ModMinecraft.McLibListGetWithJson(jobject, true, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_CurrentMcFolder);
                        if (jobject["data"] != null && jobject["data"]["MOJMAPS"] != null)
                        {
                            Task.Progress = 0.4;
                            JObject jobject3 = (JObject)ModNet.NetRequestByLoader(ModDownload.DlSourceLauncherOrMetaGet(Conversions.ToString(ModDownload.DlClientListGet(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit))), 45000, true, false).DeserializeJson();
                            string text3 = jobject["data"]["MOJMAPS"]["client"].ToString().Trim("[]".ToCharArray()).BeforeFirst("@", false);
                            string text4 = Path.ChangeExtension(ModMinecraft.McLibGet(text3, true, false, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_CurrentMcFolder), "." + jobject["data"]["MOJMAPS"]["client"].ToString().Trim("[]".ToCharArray()).Split("@", false)[1]);
                            JToken jtoken = jobject3["downloads"]["client_mappings"];
                            CS$<>8__locals2.$VB$Local_Libs.Add(new ModMinecraft.McLibEntry
                            {
                                IsNatives = false,
                                LocalPath = text4,
                                OriginalName = text3,
                                Url = (string)jtoken["url"],
                                Size = (long)jtoken["size"],
                                SHA1 = (string)jtoken["sha1"]
                            });
                            Logger.Info(string.Format("需要下载 Mappings：{0} (SHA1: {1})", jtoken["url"], jtoken["sha1"]), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        }
                        Task.Progress = 0.8;
                        int num = CS$<>8__locals2.$VB$Local_Libs.Count - 1;
                        for (int i = 0; i <= num; i++)
                        {
                            if (CS$<>8__locals2.$VB$Local_Libs[i].LocalPath.EndsWithF(string.Format("{0}-{1}-{2}.jar", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName.Lower(), CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderVersion), false) || CS$<>8__locals2.$VB$Local_Libs[i].LocalPath.EndsWithF(string.Format("{0}-{1}-{2}-client.jar", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName.Lower(), CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderVersion), false))
                            {
                                Logger.Warn(string.Format("已从待下载 {0} 支持库中移除：{1}", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName, CS$<>8__locals2.$VB$Local_Libs[i].LocalPath), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                                CS$<>8__locals2.$VB$Local_Libs.RemoveAt(i);
                                break;
                            }
                        }
                        Task.Output = ModMinecraft.McLibNetFilesFromTokens(CS$<>8__locals2.$VB$Local_Libs, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_CurrentMcFolder);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("获取{0}Forge 支持库列表失败", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_IsNeoForge ? " Neo" : "新版 "), ex);
                    }
                    finally
                    {
                        if (zipArchive != null)
                        {
                            zipArchive.Dispose();
                        }
                    }
                }), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 1.0
                });
                list.Add(new ModNet.LoaderDownload(string.Format("下载 {0} 支持库文件", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName), new List<ModNet.NetFile>())
                {
                    ProgressWeight = 12.0
                });
                list.Add(new ModLoader.LoaderTask<List<ModNet.NetFile>, bool>(string.Format("获取 {0} 支持库文件", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName), delegate(ModLoader.LoaderTask<List<ModNet.NetFile>, bool> Task)
                {
                    try
                    {
                        foreach (ModMinecraft.McLibEntry mcLibEntry in CS$<>8__locals2.$VB$Local_Libs)
                        {
                            string text5 = Path.Combine(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder, mcLibEntry.LocalPath.Substring(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_CurrentMcFolder.Length));
                            if (!FileUtils.Exists(text5))
                            {
                                FileUtils.Copy(mcLibEntry.LocalPath, text5);
                            }
                            Logger.Trace(string.Format("复制的 {0} 支持库文件：{1}", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName, mcLibEntry.LocalPath), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        }
                    }
                    finally
                    {
                        List<ModMinecraft.McLibEntry>.Enumerator enumerator2;
                        ((IDisposable)enumerator2).Dispose();
                    }
                    List<ModLoader.LoaderBase> list3 = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ClientDownloadLoader.GetLoaderList(true).Where<ModLoader.LoaderBase>((ModDownloadLib._Closure$__.$I30-4 == null) ? (ModDownloadLib._Closure$__.$I30-4 = (ModLoader.LoaderBase l) => Operators.CompareString(l.Name, "下载原版支持库文件", false) == 0 || Operators.CompareString(l.Name, "下载原版 json 文件", false) == 0) : ModDownloadLib._Closure$__.$I30-4).Where<ModLoader.LoaderBase>((ModDownloadLib._Closure$__.$I30-5 == null) ? (ModDownloadLib._Closure$__.$I30-5 = (ModLoader.LoaderBase l) => l.State != ModBase.LoadState.Finished) : ModDownloadLib._Closure$__.$I30-5)
                        .ToList<ModLoader.LoaderBase>();
                    if (list3.Any<ModLoader.LoaderBase>())
                    {
                        Logger.Info(string.Format("{0} 安装正在等待原版文件下载完成", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                    }
                    while (list3.Any<ModLoader.LoaderBase>() && !Task.IsCanceled)
                    {
                        list3 = list3.Where<ModLoader.LoaderBase>((ModDownloadLib._Closure$__.$I30-6 == null) ? (ModDownloadLib._Closure$__.$I30-6 = (ModLoader.LoaderBase l) => l.State != ModBase.LoadState.Finished) : ModDownloadLib._Closure$__.$I30-6).ToList<ModLoader.LoaderBase>();
                        Thread.Sleep(50);
                    }
                    if (!Task.IsCanceled)
                    {
                        object vanillaSyncLock = ModDownloadLib.VanillaSyncLock;
                        ObjectFlowControl.CheckForSyncLockOnValueType(vanillaSyncLock);
                        lock (vanillaSyncLock)
                        {
                            string lastPart = PathUtils.GetLastPart(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ClientFolder);
                            DirectoryUtils.Create(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder + "versions\\" + CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit);
                            if (!FileUtils.Exists(string.Concat(new string[]
                            {
                                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder,
                                "versions\\",
                                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit,
                                "\\",
                                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit,
                                ".json"
                            })))
                            {
                                FileUtils.Copy(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ClientFolder + lastPart + ".json", string.Concat(new string[]
                                {
                                    CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder,
                                    "versions\\",
                                    CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit,
                                    "\\",
                                    CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit,
                                    ".json"
                                }));
                            }
                            if (!FileUtils.Exists(string.Concat(new string[]
                            {
                                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder,
                                "versions\\",
                                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit,
                                "\\",
                                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit,
                                ".jar"
                            })))
                            {
                                FileUtils.Copy(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ClientFolder + lastPart + ".jar", string.Concat(new string[]
                                {
                                    CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder,
                                    "versions\\",
                                    CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit,
                                    "\\",
                                    CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit,
                                    ".jar"
                                }));
                            }
                        }
                    }
                }, null, ThreadPriority.Normal)
                {
                    ProgressWeight = 0.1,
                    Show = false
                });
                list.Add(new ModLoader.LoaderTask<bool, bool>(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_IsNeoForge ? "安装 NeoForge" : "安装 Forge（方式 A）", delegate(ModLoader.LoaderTask<bool, bool> Task)
                {
                    try
                    {
                        ModDownloadLib._Closure$__30-2 CS$<>8__locals3 = new ModDownloadLib._Closure$__30-2(CS$<>8__locals3);
                        Logger.Info(string.Format("开始进行 Forgelike 安装：{0}", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_InstallerAddress), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        CS$<>8__locals3.$VB$Local_OldList = DirectoryUtils.EnumerateDirectories(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder + "versions\\", false, "*").ToList<string>();
                        DirectoryUtils.Create(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_VersionFolder);
                        Task.Progress = 0.04;
                        ModMinecraft.McFolderLauncherProfilesJsonCreate(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder);
                        Task.Progress = 0.05;
                        bool flag2 = true;
                        for (;;)
                        {
                            try
                            {
                                ModBase.ExtractResources(ModBase.PathTemp + "Cache\\forge_installer.jar", "ForgeInstaller");
                                Task.Progress = 0.06;
                                ModDownloadLib.ForgelikeInjector(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_InstallerAddress, Task, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder, flag2, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_IsNeoForge);
                                Task.Progress = 0.97;
                            }
                            catch (Exception ex2)
                            {
                                if (flag2)
                                {
                                    Logger.Warn(ex2, string.Format("使用 JavaWrapper 安装 {0} 失败，将不使用 JavaWrapper 并重试", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                                    flag2 = false;
                                    continue;
                                }
                                throw new Exception(string.Format("运行 {0} 安装器失败", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName), ex2);
                            }
                            break;
                        }
                        List<DirectoryInfo> list4 = DirectoryUtils.EnumerateDirectories(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder + "versions\\", false, "*").SkipWhile<string>((string i) => CS$<>8__locals3.$VB$Local_OldList.Contains(i)).Select<string, DirectoryInfo>((ModDownloadLib._Closure$__.$I30-9 == null) ? (ModDownloadLib._Closure$__.$I30-9 = (string f) => DirectoryUtils.GetInfo(f)) : ModDownloadLib._Closure$__.$I30-9)
                            .ToList<DirectoryInfo>();
                        if (list4.Count > 1)
                        {
                            list4 = list4.Where<DirectoryInfo>((ModDownloadLib._Closure$__.$I30-10 == null) ? (ModDownloadLib._Closure$__.$I30-10 = (DirectoryInfo l) => l.Name.ContainsIgnoreCase("forge") && l.EnumerateFiles().Any<FileInfo>()) : ModDownloadLib._Closure$__.$I30-10).ToList<DirectoryInfo>();
                        }
                        if (list4.Count == 1)
                        {
                            FileInfo fileInfo = list4[0].EnumerateFiles().First<FileInfo>();
                            FileUtils.Copy(fileInfo.FullName, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_VersionFolder + CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_NewInstanceName + ".json");
                            Logger.Info(string.Format("已复制新增的版本 JSON 文件：{0} -> {1}{2}.json", fileInfo.FullName, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_VersionFolder, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_NewInstanceName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        }
                        else if (list4.Count > 1)
                        {
                            Logger.Info(string.Format("有多个疑似的新增版本，无法确定：{0}", list4.Select<DirectoryInfo, string>((ModDownloadLib._Closure$__.$I30-11 == null) ? (ModDownloadLib._Closure$__.$I30-11 = (DirectoryInfo d) => d.Name) : ModDownloadLib._Closure$__.$I30-11).Join(";")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        }
                        else
                        {
                            Logger.Info("未找到新增的版本文件夹", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        }
                    }
                    catch (Exception ex3)
                    {
                        throw new Exception(string.Format("安装新 {0} 版本失败", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName), ex3);
                    }
                    finally
                    {
                        try
                        {
                            FileUtils.Delete(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_InstallerAddress, false);
                        }
                        catch (Exception ex4)
                        {
                            Logger.Warn(ex4, string.Format("安装 {0} 清理文件时出错", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        }
                    }
                }, null, ThreadPriority.Normal)
                {
                    ProgressWeight = 10.0
                });
            }
            else
            {
                Logger.Info(string.Format("检测为非新版 Forge：{0}", CS$<>8__locals1.$VB$Local_LoaderVersion), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                list.Add(new ModLoader.LoaderTask<List<ModNet.NetFile>, bool>(string.Format("安装 {0}（方式 B）", CS$<>8__locals1.$VB$Local_LoaderName), delegate(ModLoader.LoaderTask<List<ModNet.NetFile>, bool> Task)
                {
                    ZipArchive zipArchive2 = null;
                    try
                    {
                        zipArchive2 = FileUtils.OpenZip(CS$<>8__locals1.$VB$Local_InstallerAddress, default(CancellationToken));
                        Task.Progress = 0.2;
                        JObject jobject4 = (JObject)zipArchive2.GetEntry("install_profile.json").Open().ReadString(null)
                            .DeserializeJson();
                        Task.Progress = 0.4;
                        DirectoryUtils.Create(CS$<>8__locals1.$VB$Local_VersionFolder);
                        Task.Progress = 0.5;
                        if (jobject4["install"] == null)
                        {
                            Logger.Info(string.Format("开始进行 Forge 安装，Legacy 方式 1：{0}", CS$<>8__locals1.$VB$Local_InstallerAddress), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                            JObject jobject5 = (JObject)zipArchive2.GetEntry(jobject4["json"].ToString().TrimStart(new char[] { '/' })).Open().ReadString(null)
                                .DeserializeJson();
                            jobject5["id"] = CS$<>8__locals1.$VB$Local_NewInstanceName;
                            FileUtils.Write(CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_NewInstanceName + ".json", jobject5.ToString(), null);
                            Task.Progress = 0.6;
                            zipArchive2.Dispose();
                            FileUtils.ExtractToDirectory(CS$<>8__locals1.$VB$Local_InstallerAddress, CS$<>8__locals1.$VB$Local_InstallerAddress + "_unrar\\", default(CancellationToken), null);
                            if (DirectoryUtils.Exists(CS$<>8__locals1.$VB$Local_InstallerAddress + "_unrar\\maven\\"))
                            {
                                DirectoryUtils.Copy(CS$<>8__locals1.$VB$Local_InstallerAddress + "_unrar\\maven\\", CS$<>8__locals1.$VB$Local_McFolder + "libraries\\");
                            }
                            DirectoryUtils.Delete(CS$<>8__locals1.$VB$Local_InstallerAddress + "_unrar\\", false);
                        }
                        else
                        {
                            Logger.Info(string.Format("开始进行 Forge 安装，Legacy 方式 2：{0}", CS$<>8__locals1.$VB$Local_InstallerAddress), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                            FileUtils.Write(ModMinecraft.McLibGet((string)jobject4["install"]["path"], true, false, CS$<>8__locals1.$VB$Local_McFolder), zipArchive2.GetEntry((string)jobject4["install"]["filePath"]).Open());
                            Task.Progress = 0.9;
                            jobject4["versionInfo"]["id"] = CS$<>8__locals1.$VB$Local_NewInstanceName;
                            if (jobject4["versionInfo"]["inheritsFrom"] == null)
                            {
                                ((JObject)jobject4["versionInfo"]).Add("inheritsFrom", CS$<>8__locals1.$VB$Local_Inherit);
                            }
                            FileUtils.Write(CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_NewInstanceName + ".json", jobject4["versionInfo"].ToString(), null);
                        }
                    }
                    catch (Exception ex5)
                    {
                        throw new Exception("非新版方式安装 Forge 失败", ex5);
                    }
                    finally
                    {
                        try
                        {
                            if (zipArchive2 != null)
                            {
                                zipArchive2.Dispose();
                            }
                            FileUtils.Delete(CS$<>8__locals1.$VB$Local_InstallerAddress, false);
                            DirectoryUtils.Delete(CS$<>8__locals1.$VB$Local_InstallerAddress + "_unrar\\", false);
                        }
                        catch (Exception ex6)
                        {
                            Logger.Warn(ex6, "非新版方式安装 Forge 清理文件时出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        }
                    }
                }, null, ThreadPriority.Normal)
                {
                    ProgressWeight = 1.0
                });
            }
            return list;
        }

        public static void ForgeDownloadListItemPreload(StackPanel Stack, List<ModDownload.DlForgeVersionEntry> Entries, MyListItem.ClickEventHandler OnClick, bool IsSaveOnly)
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
                    List<ModDownload.DlForgeVersionEntry>.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
                if (dlForgeVersionEntry != null && dlForgeVersionEntry == dlForgeVersionEntry2)
                {
                    dlForgeVersionEntry = null;
                }
                if (dlForgeVersionEntry2 != null)
                {
                    MyListItem myListItem = ModDownloadLib.ForgeDownloadListItem(dlForgeVersionEntry2, OnClick, IsSaveOnly).Init();
                    myListItem.Info = "推荐版" + ((Operators.CompareString(myListItem.Info, "", false) == 0) ? "" : ("，" + myListItem.Info));
                    Stack.Children.Add(myListItem);
                }
                if (dlForgeVersionEntry != null)
                {
                    MyListItem myListItem2 = ModDownloadLib.ForgeDownloadListItem(dlForgeVersionEntry, OnClick, IsSaveOnly).Init();
                    myListItem2.Info = "最新版" + ((Operators.CompareString(myListItem2.Info, "", false) == 0) ? "" : ("，" + myListItem2.Info));
                    Stack.Children.Add(myListItem2);
                }
                Stack.Children.Add(new TextBlock
                {
                    Text = "全部版本 (" + Conversions.ToString(Entries.Count) + ")",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(6.0, 13.0, 0.0, 4.0)
                });
            }
        }

        public static MyVirtualizingElement<MyListItem> ForgeDownloadListItem(ModDownload.DlForgeVersionEntry Entry, MyListItem.ClickEventHandler OnClick, bool IsSaveOnly)
        {
            return new MyVirtualizingElement<MyListItem>(delegate
            {
                MyListItem myListItem = new MyListItem
                {
                    Title = Entry.VersionName,
                    SnapsToDevicePixels = true,
                    Height = 42.0,
                    Type = MyListItem.CheckType.Clickable,
                    Tag = Entry,
                    Info = new string[]
                    {
                        (Operators.CompareString(Entry.ReleaseTime, "", false) == 0) ? "" : ("发布于 " + Entry.ReleaseTime),
                        ModBase.ModeDebug ? ("种类：" + Entry.Category) : ""
                    }.Where<string>((ModDownloadLib._Closure$__.$I32-1 == null) ? (ModDownloadLib._Closure$__.$I32-1 = (string d) => Operators.CompareString(d, "", false) != 0) : ModDownloadLib._Closure$__.$I32-1).Join("，"),
                    Logo = ModBase.PathImage + "Blocks/Anvil.png"
                };
                myListItem.Click += OnClick;
                if (IsSaveOnly)
                {
                    myListItem.ContentHandler = new Action<MyListItem, EventArgs>(ModDownloadLib.ForgeSaveContMenuBuild);
                }
                else
                {
                    myListItem.ContentHandler = new Action<MyListItem, EventArgs>(ModDownloadLib.ForgeContMenuBuild);
                }
                return myListItem;
            })
            {
                Height = 42.0
            };
        }

        private static void ForgeContMenuBuild(MyListItem sender, EventArgs e)
        {
            MyIconButton myIconButton = new MyIconButton
            {
                Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z",
                ToolTip = "另存为"
            };
            myIconButton.Click += ((ModDownloadLib._Closure$__.$IR33-16 == null) ? (ModDownloadLib._Closure$__.$IR33-16 = delegate(object a0, EventArgs a1)
            {
                ModDownloadLib.ForgeSave_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }) : ModDownloadLib._Closure$__.$IR33-16);
            MyIconButton myIconButton2 = new MyIconButton
            {
                LogoScale = 1.05,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                ToolTip = "更新日志"
            };
            myIconButton2.Click += ((ModDownloadLib._Closure$__.$IR33-17 == null) ? (ModDownloadLib._Closure$__.$IR33-17 = delegate(object a0, EventArgs a1)
            {
                ModDownloadLib.ForgeLog_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }) : ModDownloadLib._Closure$__.$IR33-17);
            sender.Buttons = new MyIconButton[] { myIconButton, myIconButton2 };
        }

        private static void ForgeSaveContMenuBuild(MyListItem sender, EventArgs e)
        {
            MyIconButton myIconButton = new MyIconButton
            {
                LogoScale = 1.05,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                ToolTip = "更新日志"
            };
            myIconButton.Click += ((ModDownloadLib._Closure$__.$IR34-18 == null) ? (ModDownloadLib._Closure$__.$IR34-18 = delegate(object a0, EventArgs a1)
            {
                ModDownloadLib.ForgeLog_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }) : ModDownloadLib._Closure$__.$IR34-18);
            sender.Buttons = new MyIconButton[] { myIconButton };
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
                ModBase.RunInNewThread((ModDownloadLib._Closure$__.$I37-0 == null) ? (ModDownloadLib._Closure$__.$I37-0 = delegate
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
                            foreach (JToken jtoken in jcontainer)
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
                            IEnumerator<JToken> enumerator;
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
                }) : ModDownloadLib._Closure$__.$I37-0, "ForgeRecommendedRefresh", ThreadPriority.Normal);
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

        public static void NeoForgeDownloadListItemPreload(StackPanel Stack, List<ModDownload.DlNeoForgeListEntry> Entries, MyListItem.ClickEventHandler OnClick, bool IsSaveOnly)
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
                        List<ModDownload.DlNeoForgeListEntry>.Enumerator enumerator;
                        ((IDisposable)enumerator).Dispose();
                    }
                }
                Logger.Warn("未找到可用的 NeoForge 版本", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                IL_0069:
                if (dlNeoForgeListEntry != null)
                {
                    MyListItem myListItem = ModDownloadLib.NeoForgeDownloadListItem(dlNeoForgeListEntry, OnClick, IsSaveOnly).Init();
                    myListItem.Info = ((Operators.CompareString(myListItem.Info, "", false) == 0) ? "最新稳定版" : ("最新" + myListItem.Info));
                    Stack.Children.Add(myListItem);
                }
                if (dlNeoForgeListEntry2 != null)
                {
                    MyListItem myListItem2 = ModDownloadLib.NeoForgeDownloadListItem(dlNeoForgeListEntry2, OnClick, IsSaveOnly).Init();
                    myListItem2.Info = ((Operators.CompareString(myListItem2.Info, "", false) == 0) ? "最新测试版" : ("最新" + myListItem2.Info));
                    Stack.Children.Add(myListItem2);
                }
                Stack.Children.Add(new TextBlock
                {
                    Text = "全部版本 (" + Conversions.ToString(Entries.Count) + ")",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(6.0, 13.0, 0.0, 4.0)
                });
            }
        }

        public static MyVirtualizingElement<MyListItem> NeoForgeDownloadListItem(ModDownload.DlNeoForgeListEntry Info, MyListItem.ClickEventHandler OnClick, bool IsSaveOnly)
        {
            return new MyVirtualizingElement<MyListItem>(delegate
            {
                MyListItem myListItem = new MyListItem
                {
                    Title = Info.VersionName,
                    SnapsToDevicePixels = true,
                    Height = 42.0,
                    Type = MyListItem.CheckType.Clickable,
                    Tag = Info,
                    Info = (Info.IsBeta ? "测试版" : "稳定版"),
                    Logo = ModBase.PathImage + "Blocks/NeoForge.png"
                };
                myListItem.Click += OnClick;
                if (IsSaveOnly)
                {
                    myListItem.ContentHandler = new Action<MyListItem, EventArgs>(ModDownloadLib.NeoForgeSaveContMenuBuild);
                }
                else
                {
                    myListItem.ContentHandler = new Action<MyListItem, EventArgs>(ModDownloadLib.NeoForgeContMenuBuild);
                }
                return myListItem;
            })
            {
                Height = 42.0
            };
        }

        private static void NeoForgeContMenuBuild(MyListItem sender, EventArgs e)
        {
            MyIconButton myIconButton = new MyIconButton
            {
                Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z",
                ToolTip = "另存为"
            };
            myIconButton.Click += ((ModDownloadLib._Closure$__.$IR42-19 == null) ? (ModDownloadLib._Closure$__.$IR42-19 = delegate(object a0, EventArgs a1)
            {
                ModDownloadLib.NeoForgeSave_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }) : ModDownloadLib._Closure$__.$IR42-19);
            MyIconButton myIconButton2 = new MyIconButton
            {
                LogoScale = 1.05,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                ToolTip = "更新日志"
            };
            myIconButton2.Click += ((ModDownloadLib._Closure$__.$IR42-20 == null) ? (ModDownloadLib._Closure$__.$IR42-20 = delegate(object a0, EventArgs a1)
            {
                ModDownloadLib.NeoForgeLog_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }) : ModDownloadLib._Closure$__.$IR42-20);
            sender.Buttons = new MyIconButton[] { myIconButton, myIconButton2 };
        }

        private static void NeoForgeSaveContMenuBuild(MyListItem sender, EventArgs e)
        {
            MyIconButton myIconButton = new MyIconButton
            {
                LogoScale = 1.05,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                ToolTip = "更新日志"
            };
            myIconButton.Click += ((ModDownloadLib._Closure$__.$IR43-21 == null) ? (ModDownloadLib._Closure$__.$IR43-21 = delegate(object a0, EventArgs a1)
            {
                ModDownloadLib.NeoForgeLog_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
            }) : ModDownloadLib._Closure$__.$IR43-21);
            sender.Buttons = new MyIconButton[] { myIconButton };
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
                        List<ModLoader.LoaderBase>.Enumerator enumerator;
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
            string Id = "fabric-loader-" + FabricVersion + "-" + MinecraftName;
            string VersionFolder = McFolder + "versions\\" + Id + "\\";
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            MinecraftName = MinecraftName.Replace("∞", "infinite");
            list.Add(new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("获取 Fabric 主文件下载地址", delegate(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
            {
                Task.Output = new List<ModNet.NetFile>
                {
                    new ModNet.NetFile(new string[]
                    {
                        string.Concat(new string[] { "https://bmclapi2.bangbang93.com/fabric-meta/v2/versions/loader/", MinecraftName, "/", FabricVersion, "/profile/json" }),
                        string.Concat(new string[] { "https://meta.fabricmc.net/v2/versions/loader/", MinecraftName, "/", FabricVersion, "/profile/json" })
                    }, VersionFolder + Id + ".json", new FileChecker
                    {
                        IsJson = true
                    }, false)
                };
            }, null, ThreadPriority.Normal)
            {
                ProgressWeight = 0.5
            });
            list.Add(new ModNet.LoaderDownload("下载 Fabric 主文件", new List<ModNet.NetFile>())
            {
                ProgressWeight = 2.5
            });
            return list;
        }

        public static MyVirtualizingElement<MyListItem> FabricDownloadListItem(JObject Entry, MyListItem.ClickEventHandler OnClick)
        {
            return new MyVirtualizingElement<MyListItem>(delegate
            {
                MyListItem myListItem = new MyListItem();
                myListItem.Title = Entry["version"].ToString().Replace("+build", "");
                myListItem.SnapsToDevicePixels = true;
                myListItem.Height = 42.0;
                myListItem.Type = MyListItem.CheckType.Clickable;
                myListItem.Tag = Entry;
                myListItem.Info = (Entry["stable"].ToObject<bool>() ? "稳定版" : "测试版");
                myListItem.Logo = ModBase.PathImage + "Blocks/Fabric.png";
                myListItem.Click += OnClick;
                return myListItem;
            })
            {
                Height = 42.0
            };
        }

        public static MyVirtualizingElement<MyListItem> FabricApiDownloadListItem(ResourceVersion Entry, MyListItem.ClickEventHandler OnClick)
        {
            return new MyVirtualizingElement<MyListItem>(delegate
            {
                MyListItem myListItem = new MyListItem();
                myListItem.Title = Entry.Display.Split("]", false)[1].Replace("Fabric API ", "").Replace(" build ", ".").BeforeFirst("+", false)
                    .Trim();
                myListItem.SnapsToDevicePixels = true;
                myListItem.Height = 42.0;
                myListItem.Type = MyListItem.CheckType.Clickable;
                myListItem.Tag = Entry;
                myListItem.Info = Entry.ReleaseTypeDisplay + "，发布于 " + Entry.ReleaseDate.ToString("yyyy'/'MM'/'dd HH':'mm");
                myListItem.Logo = ModBase.PathImage + "Blocks/Fabric.png";
                myListItem.Click += OnClick;
                return myListItem;
            })
            {
                Height = 42.0
            };
        }

        public static MyVirtualizingElement<MyListItem> OptiFabricDownloadListItem(ResourceVersion Entry, MyListItem.ClickEventHandler OnClick)
        {
            return new MyVirtualizingElement<MyListItem>(delegate
            {
                MyListItem myListItem = new MyListItem
                {
                    Title = Entry.Display.Lower().Replace("optifabric-", "").Replace(".jar", "")
                        .Trim()
                        .TrimStart(new char[] { 'v' }),
                    SnapsToDevicePixels = true,
                    Height = 42.0,
                    Type = MyListItem.CheckType.Clickable,
                    Tag = Entry,
                    Info = Entry.ReleaseTypeDisplay + "，发布于 " + Entry.ReleaseDate.ToString("yyyy'/'MM'/'dd HH':'mm"),
                    Logo = ModBase.PathImage + "Blocks/OptiFabric.png"
                };
                myListItem.Click += OnClick;
                return myListItem;
            })
            {
                Height = 42.0
            };
        }

        public static List<ModLoader.LoaderBase> McInstallLoadersGet(ModDownloadLib.McInstallRequest Request)
        {
            ModDownloadLib._Closure$__52-0 CS$<>8__locals1 = new ModDownloadLib._Closure$__52-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_Request = Request;
            CS$<>8__locals1.$VB$Local_TempMcFolder = ModMain.RequestTaskTempFolder(CS$<>8__locals1.$VB$Local_Request.Components.OptiFineEntry != null || CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.Forge) || CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.NeoForge));
            JObject optiFineEntry = CS$<>8__locals1.$VB$Local_Request.Components.OptiFineEntry;
            ModDownload.DlOptiFineListEntry dlOptiFineListEntry = ((optiFineEntry != null) ? optiFineEntry.ToObject<ModDownload.DlOptiFineListEntry>() : null);
            CS$<>8__locals1.$VB$Local_OptiFineFolder = ((dlOptiFineListEntry == null) ? null : string.Format("{0}versions\\{1}", CS$<>8__locals1.$VB$Local_TempMcFolder, dlOptiFineListEntry.InstanceName));
            CS$<>8__locals1.$VB$Local_ForgeVersion = (CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.Forge) ? CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions[ModLoaders.Forge] : null);
            CS$<>8__locals1.$VB$Local_ForgeFolder = ((CS$<>8__locals1.$VB$Local_ForgeVersion == null) ? null : string.Format("{0}versions\\forge-{1}", CS$<>8__locals1.$VB$Local_TempMcFolder, CS$<>8__locals1.$VB$Local_ForgeVersion));
            CS$<>8__locals1.$VB$Local_NeoForgeVersion = (CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.NeoForge) ? CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions[ModLoaders.NeoForge] : null);
            CS$<>8__locals1.$VB$Local_NeoForgeFolder = ((CS$<>8__locals1.$VB$Local_NeoForgeVersion == null) ? null : string.Format("{0}versions\\neoforge-{1}", CS$<>8__locals1.$VB$Local_TempMcFolder, CS$<>8__locals1.$VB$Local_NeoForgeVersion));
            string text = (CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.LiteLoader) ? CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions[ModLoaders.LiteLoader] : null);
            CS$<>8__locals1.$VB$Local_LiteLoaderFolder = ((text == null) ? null : string.Format("{0}versions\\{1}-LiteLoader", CS$<>8__locals1.$VB$Local_TempMcFolder, CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion));
            string text2 = (CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.Fabric) ? CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions[ModLoaders.Fabric] : null);
            CS$<>8__locals1.$VB$Local_FabricFolder = ((text2 == null) ? null : string.Format("{0}versions\\fabric-loader-{1}-{2}", CS$<>8__locals1.$VB$Local_TempMcFolder, text2, CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion));
            CS$<>8__locals1.$VB$Local_Modable = CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions.Any<KeyValuePair<ModLoaders, string>>();
            CS$<>8__locals1.$VB$Local_ModsTempFolder = string.Format("{0}mods\\", CS$<>8__locals1.$VB$Local_TempMcFolder);
            CS$<>8__locals1.$VB$Local_OptiFineAsMod = dlOptiFineListEntry != null && CS$<>8__locals1.$VB$Local_Modable;
            if (CS$<>8__locals1.$VB$Local_OptiFineAsMod)
            {
                Logger.Info("OptiFine 将作为 Mod 进行下载", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                if (text != null)
                {
                    CS$<>8__locals1.$VB$Local_OptiFineFolder = string.Format("{0}{1}\\", CS$<>8__locals1.$VB$Local_ModsTempFolder, CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion);
                }
                else
                {
                    CS$<>8__locals1.$VB$Local_OptiFineFolder = CS$<>8__locals1.$VB$Local_ModsTempFolder;
                }
            }
            if (CS$<>8__locals1.$VB$Local_OptiFineFolder != null)
            {
                Logger.Info(string.Format("OptiFine 缓存：{0}", CS$<>8__locals1.$VB$Local_OptiFineFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }
            if (CS$<>8__locals1.$VB$Local_ForgeFolder != null)
            {
                Logger.Info(string.Format("Forge 缓存：{0}", CS$<>8__locals1.$VB$Local_ForgeFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }
            if (CS$<>8__locals1.$VB$Local_NeoForgeFolder != null)
            {
                Logger.Info(string.Format("NeoForge 缓存：{0}", CS$<>8__locals1.$VB$Local_NeoForgeFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }
            if (CS$<>8__locals1.$VB$Local_FabricFolder != null)
            {
                Logger.Info(string.Format("Fabric 缓存：{0}", CS$<>8__locals1.$VB$Local_FabricFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }
            if (CS$<>8__locals1.$VB$Local_LiteLoaderFolder != null)
            {
                Logger.Info(string.Format("LiteLoader 缓存：{0}", CS$<>8__locals1.$VB$Local_LiteLoaderFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }
            Logger.Info(string.Format("对应的原版版本：{0}", CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            CS$<>8__locals1.$VB$Local_VersionFolder = PathUtils.AddSlashSuffix(CS$<>8__locals1.$VB$Local_Request.InstanceFolder);
            if (FileUtils.Exists(string.Format("{0}{1}.json", CS$<>8__locals1.$VB$Local_VersionFolder, CS$<>8__locals1.$VB$Local_Request.NewInstanceName)))
            {
                ModMain.Hint("版本 " + CS$<>8__locals1.$VB$Local_Request.NewInstanceName + " 已经存在！", ModMain.HintType.Red, true);
                throw new OperationCanceledException();
            }
            List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
            CS$<>8__locals1.$VB$Local_ManagedFileSources = new Dictionary<string, string>();
            list.Add(new ModLoader.LoaderTask<int, int>("添加忽略标识", delegate(ModLoader.LoaderTask<int, int> a0)
            {
                base._Lambda$__0();
            }, null, ThreadPriority.Normal)
            {
                Show = false
            });
            if (CS$<>8__locals1.$VB$Local_Request.Components.FabricApiVersion != null)
            {
                ModNet.NetFile netFile = CS$<>8__locals1.$VB$Local_Request.FabricApi.ToNetFile(CS$<>8__locals1.$VB$Local_ModsTempFolder, new ResourceVersion.DownloadReason?(ResourceVersion.DownloadReason.Dependency), CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion, ModLoaders.Fabric);
                CS$<>8__locals1.$VB$Local_ManagedFileSources.Add("FabricApi", netFile.LocalPath);
                list.Add(new ModNet.LoaderDownload("下载 Fabric API", new List<ModNet.NetFile> { netFile })
                {
                    ProgressWeight = 3.0,
                    Show = true,
                    Block = false
                });
            }
            if (CS$<>8__locals1.$VB$Local_Request.Components.OptiFabricVersion != null)
            {
                ModNet.NetFile netFile2 = CS$<>8__locals1.$VB$Local_Request.OptiFabric.ToNetFile(CS$<>8__locals1.$VB$Local_ModsTempFolder, new ResourceVersion.DownloadReason?(ResourceVersion.DownloadReason.Dependency), CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion, ModLoaders.Fabric);
                CS$<>8__locals1.$VB$Local_ManagedFileSources.Add("OptiFabric", netFile2.LocalPath);
                list.Add(new ModNet.LoaderDownload("下载 OptiFabric", new List<ModNet.NetFile> { netFile2 })
                {
                    ProgressWeight = 3.0,
                    Show = true,
                    Block = false
                });
            }
            ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>("下载原版 " + CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion, ModDownloadLib.McDownloadClientLoader(CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion, CS$<>8__locals1.$VB$Local_Request.MinecraftJson, CS$<>8__locals1.$VB$Local_VersionFolder))
            {
                ProgressWeight = 39.0,
                Show = false,
                Block = (!CS$<>8__locals1.$VB$Local_Modable && dlOptiFineListEntry == null)
            };
            list.Add(loaderCombo);
            if (dlOptiFineListEntry != null)
            {
                if (CS$<>8__locals1.$VB$Local_OptiFineAsMod)
                {
                    CS$<>8__locals1.$VB$Local_ManagedFileSources.Add("OptiFine", CS$<>8__locals1.$VB$Local_OptiFineFolder + dlOptiFineListEntry.FileName);
                }
                list.Add(new ModLoader.LoaderCombo<string>("下载 OptiFine " + dlOptiFineListEntry.DisplayName, CS$<>8__locals1.$VB$Local_OptiFineAsMod ? ModDownloadLib.McDownloadOptiFineSaveLoader(dlOptiFineListEntry, CS$<>8__locals1.$VB$Local_OptiFineFolder + dlOptiFineListEntry.FileName) : ModDownloadLib.McDownloadOptiFineLoader(dlOptiFineListEntry, CS$<>8__locals1.$VB$Local_TempMcFolder, loaderCombo, CS$<>8__locals1.$VB$Local_VersionFolder))
                {
                    ProgressWeight = (double)(CS$<>8__locals1.$VB$Local_Modable ? 16 : 24),
                    Show = false,
                    Block = !CS$<>8__locals1.$VB$Local_Modable
                });
            }
            if (CS$<>8__locals1.$VB$Local_ForgeVersion != null)
            {
                list.Add(new ModLoader.LoaderCombo<string>("下载 Forge " + CS$<>8__locals1.$VB$Local_ForgeVersion, ModDownloadLib.McDownloadForgelikeLoader(false, CS$<>8__locals1.$VB$Local_ForgeVersion, "forge-" + CS$<>8__locals1.$VB$Local_ForgeVersion, CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion, CS$<>8__locals1.$VB$Local_Request.ForgeEntry, CS$<>8__locals1.$VB$Local_TempMcFolder, loaderCombo, CS$<>8__locals1.$VB$Local_VersionFolder))
                {
                    ProgressWeight = 25.0,
                    Show = false,
                    Block = (text2 == null && CS$<>8__locals1.$VB$Local_Request.LiteLoaderEntry == null && CS$<>8__locals1.$VB$Local_Request.NeoForgeEntry == null)
                });
            }
            if (CS$<>8__locals1.$VB$Local_NeoForgeVersion != null)
            {
                list.Add(new ModLoader.LoaderCombo<string>("下载 NeoForge " + CS$<>8__locals1.$VB$Local_NeoForgeVersion, ModDownloadLib.McDownloadForgelikeLoader(true, CS$<>8__locals1.$VB$Local_NeoForgeVersion, "neoforge-" + CS$<>8__locals1.$VB$Local_NeoForgeVersion, CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion, CS$<>8__locals1.$VB$Local_Request.NeoForgeEntry, CS$<>8__locals1.$VB$Local_TempMcFolder, loaderCombo, CS$<>8__locals1.$VB$Local_VersionFolder))
                {
                    ProgressWeight = 25.0,
                    Show = false,
                    Block = (CS$<>8__locals1.$VB$Local_Request.ForgeEntry == null && text2 == null && CS$<>8__locals1.$VB$Local_Request.LiteLoaderEntry == null)
                });
            }
            if (text != null)
            {
                list.Add(new ModLoader.LoaderCombo<string>("下载 LiteLoader " + CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion, ModDownloadLib.McDownloadLiteLoaderLoader(CS$<>8__locals1.$VB$Local_Request.LiteLoaderEntry, CS$<>8__locals1.$VB$Local_TempMcFolder))
                {
                    ProgressWeight = 1.0,
                    Show = false,
                    Block = (text2 == null)
                });
            }
            if (text2 != null)
            {
                list.Add(new ModLoader.LoaderCombo<string>("下载 Fabric " + text2, ModDownloadLib.McDownloadFabricLoader(text2, CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion, CS$<>8__locals1.$VB$Local_TempMcFolder))
                {
                    ProgressWeight = 2.0,
                    Show = false,
                    Block = true
                });
            }
            CS$<>8__locals1.$VB$Local_MinecraftFolder = ModMinecraft.McFolderSelected;
            CS$<>8__locals1.$VB$Local_TargetFolder = null;
            list.Add(new ModLoader.LoaderTask<string, string>("安装游戏", delegate(ModLoader.LoaderTask<string, string> Task)
            {
                ModDownloadLib.MergeJson(CS$<>8__locals1.$VB$Local_VersionFolder, CS$<>8__locals1.$VB$Local_OptiFineFolder, CS$<>8__locals1.$VB$Local_OptiFineAsMod, CS$<>8__locals1.$VB$Local_ForgeFolder, CS$<>8__locals1.$VB$Local_NeoForgeFolder, CS$<>8__locals1.$VB$Local_FabricFolder, CS$<>8__locals1.$VB$Local_LiteLoaderFolder);
                Task.Progress = 0.2;
                DirectoryUtils.Copy(CS$<>8__locals1.$VB$Local_TempMcFolder + "libraries", CS$<>8__locals1.$VB$Local_MinecraftFolder + "libraries");
                Task.Progress = 0.8;
                CS$<>8__locals1.$VB$Local_TargetFolder = (CS$<>8__locals1.$VB$Local_Request.IsIntermediateOutput ? CS$<>8__locals1.$VB$Local_VersionFolder : new ModMinecraft.McInstance(CS$<>8__locals1.$VB$Local_VersionFolder).PathIndie);
                string text3 = CS$<>8__locals1.$VB$Local_TargetFolder + "mods\\";
                if (DirectoryUtils.Exists(CS$<>8__locals1.$VB$Local_ModsTempFolder))
                {
                    DirectoryUtils.Copy(CS$<>8__locals1.$VB$Local_ModsTempFolder, text3);
                }
                else if (CS$<>8__locals1.$VB$Local_Modable)
                {
                    DirectoryUtils.Create(text3);
                    Logger.Info(string.Format("自动创建 Mod 文件夹：{0}", text3), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                }
                string text4 = CS$<>8__locals1.$VB$Local_TargetFolder + "resourcepacks\\";
                DirectoryUtils.Create(text4);
                Logger.Info(string.Format("自动创建资源包文件夹：{0}", text4), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
            }, null, ThreadPriority.Normal)
            {
                ProgressWeight = 2.0,
                Block = true
            });
            list.Add(new ModLoader.LoaderCombo<string>("下载游戏支持库文件", new List<ModLoader.LoaderBase>
            {
                new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("分析游戏支持库文件（副加载器）", delegate(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
                {
                    Task.Output = ModMinecraft.McLibNetFilesFromInstance(new ModMinecraft.McInstance(CS$<>8__locals1.$VB$Local_VersionFolder));
                }, null, ThreadPriority.Normal)
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
            list.Add(new ModLoader.LoaderTask<int, int>("记录安装清单", delegate(ModLoader.LoaderTask<int, int> a0)
            {
                base._Lambda$__3();
            }, null, ThreadPriority.Normal)
            {
                Show = false
            });
            if (!CS$<>8__locals1.$VB$Local_Request.IsIntermediateOutput)
            {
                list.Add(new ModLoader.LoaderTask<int, int>("删除忽略标识", delegate(ModLoader.LoaderTask<int, int> a0)
                {
                    base._Lambda$__4();
                }, null, ThreadPriority.Normal)
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
            Logger.Info(string.Format("开始进行版本合并，输出：{0}，", VersionFolder) + string.Join("，", list.Select<ValueTuple<string, string>, string>((ModDownloadLib._Closure$__.$I53-0 == null) ? (ModDownloadLib._Closure$__.$I53-0 = ([TupleElementNames(new string[] { "Name", "Folder" })] ValueTuple<string, string> Source) => string.Format("{0}：{1}", Source.Item1, Source.Item2)) : ModDownloadLib._Closure$__.$I53-0)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
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
                List<ValueTuple<string, string>>.Enumerator enumerator;
                ((IDisposable)enumerator).Dispose();
            }
            string text3 = list2.Select<ValueTuple<string, JObject>, string>((ModDownloadLib._Closure$__.$I53-1 == null) ? (ModDownloadLib._Closure$__.$I53-1 = delegate([TupleElementNames(new string[] { "Name", "Json" })] ValueTuple<string, JObject> Profile)
            {
                JToken jtoken = Profile.Item2["minecraftArguments"];
                return ((jtoken != null) ? jtoken.ToString() : null) ?? " ";
            }) : ModDownloadLib._Closure$__.$I53-1).Join(" ");
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
                IEnumerator<ValueTuple<string, JObject>> enumerator2;
                if (enumerator2 != null)
                {
                    enumerator2.Dispose();
                }
            }
            List<string> list3 = new List<string>();
            try
            {
                foreach (string text4 in text3.Split(" ", true).Select<string, string>((ModDownloadLib._Closure$__.$I53-2 == null) ? (ModDownloadLib._Closure$__.$I53-2 = (string Value) => Value.Trim()) : ModDownloadLib._Closure$__.$I53-2))
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
                IEnumerator<string> enumerator3;
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
            ModDownloadLib._Closure$__55-0 CS$<>8__locals1 = new ModDownloadLib._Closure$__55-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_Request = Request;
            CS$<>8__locals1.$VB$Local_Instance = Instance;
            bool flag;
            try
            {
                ModDownloadLib._Closure$__55-1 CS$<>8__locals2 = new ModDownloadLib._Closure$__55-1(CS$<>8__locals2);
                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1;
                CS$<>8__locals2.$VB$Local_TargetFolder = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.PathIndie;
                List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
                list.Add(new ModLoader.LoaderTask<int, int>("检查 Natives 文件占用", delegate(ModLoader.LoaderTask<int, int> a0)
                {
                    base._Lambda$__0();
                }, null, ThreadPriority.Normal)
                {
                    Show = false
                });
                CS$<>8__locals2.$VB$Local_BuildFolder = ModMain.RequestTaskTempFolder(false) + CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.Name + "\\";
                ModDownloadLib.McInstallRequest mcInstallRequest = new ModDownloadLib.McInstallRequest
                {
                    NewInstanceName = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.Name,
                    InstanceFolder = CS$<>8__locals2.$VB$Local_BuildFolder,
                    Components = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.Components,
                    MinecraftJson = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.MinecraftJson,
                    ForgeEntry = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.ForgeEntry,
                    NeoForgeEntry = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.NeoForgeEntry,
                    FabricApi = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.FabricApi,
                    OptiFabric = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.OptiFabric,
                    LiteLoaderEntry = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.LiteLoaderEntry,
                    IsIntermediateOutput = true
                };
                list.AddRange(ModDownloadLib.McInstallLoadersGet(mcInstallRequest));
                CS$<>8__locals2.$VB$Local_RenameFailed = false;
                list.Add(new ModLoader.LoaderTask<int, int>("修改版本", delegate(ModLoader.LoaderTask<int, int> Task)
                {
                    ModDownloadLib._Closure$__55-2 CS$<>8__locals3 = new ModDownloadLib._Closure$__55-2(CS$<>8__locals3);
                    CS$<>8__locals3.$VB$NonLocal_$VB$Closure_3 = CS$<>8__locals2;
                    ModDownloadLib.McInstallCommit(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance, Configs.McInstallManifest.Get(new ModMinecraft.McInstance(CS$<>8__locals2.$VB$Local_BuildFolder).Config), CS$<>8__locals2.$VB$Local_BuildFolder, CS$<>8__locals2.$VB$Local_TargetFolder);
                    CS$<>8__locals3.$VB$Local_NewInstance = null;
                    if (CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.NewInstanceName == null)
                    {
                        CS$<>8__locals3.$VB$Local_NewInstance = new ModMinecraft.McInstance(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.PathVersion).Load();
                    }
                    else
                    {
                        try
                        {
                            CS$<>8__locals3.$VB$Local_NewInstance = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.Rename(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.NewInstanceName);
                        }
                        catch (Exception ex2)
                        {
                            CS$<>8__locals2.$VB$Local_RenameFailed = true;
                            Logger.Warn(ex2, string.Format("版本修改成功，但重命名为 {0} 失败：{1}", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.NewInstanceName, ex2.GetDisplay(false)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                            CS$<>8__locals3.$VB$Local_NewInstance = new ModMinecraft.McInstance(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.PathVersion).Load();
                        }
                    }
                    if (CS$<>8__locals3.$VB$Local_NewInstance == null)
                    {
                        CS$<>8__locals3.$VB$Local_NewInstance = new ModMinecraft.McInstance(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.PathVersion).Load();
                    }
                    ModBase.RunInUiWait(delegate
                    {
                        ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "InstanceCache", "");
                        if (PageInstanceLeft.Instance != null && PathUtils.IsSame(PageInstanceLeft.Instance.PathVersion, CS$<>8__locals3.$VB$NonLocal_$VB$Closure_3.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.PathVersion))
                        {
                            PageInstanceLeft.Instance = CS$<>8__locals3.$VB$Local_NewInstance;
                        }
                        if (ModMinecraft.McInstanceSelected != null && PathUtils.IsSame(ModMinecraft.McInstanceSelected.PathVersion, CS$<>8__locals3.$VB$NonLocal_$VB$Closure_3.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.PathVersion))
                        {
                            ModMinecraft.McInstanceSelected = CS$<>8__locals3.$VB$Local_NewInstance;
                        }
                    });
                }, null, ThreadPriority.Normal)
                {
                    ProgressWeight = 1.0,
                    Block = true
                });
                CS$<>8__locals2.$VB$Local_Loader = new ModLoader.LoaderCombo<string>(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.Name + " 修改", list)
                {
                    OnStateChanged = delegate(ModLoader.LoaderBase CurrentLoader)
                    {
                        switch (CurrentLoader.State)
                        {
                        case ModBase.LoadState.Finished:
                            if (!CS$<>8__locals2.$VB$Local_RenameFailed)
                            {
                                ModMain.Hint(CurrentLoader.Name + "成功！", ModMain.HintType.Green, true);
                                return;
                            }
                            ModMain.Hint(CurrentLoader.Name + "成功，但重命名失败！", ModMain.HintType.Green, true);
                            return;
                        case ModBase.LoadState.Failed:
                            ModMain.MyMsgBox(CurrentLoader.Error.GetDisplay(true), CurrentLoader.Name + "失败", "确定", "", "", true, true, false, null, null, null);
                            return;
                        case ModBase.LoadState.Canceled:
                            ModMain.Hint(CurrentLoader.Name + "已取消！", ModMain.HintType.Blue, true);
                            return;
                        default:
                            return;
                        }
                    }
                };
                RedoableWorker redoableWorker = new RedoableWorker(delegate
                {
                    CS$<>8__locals2.$VB$Local_Loader.WaitForExit(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.PathVersion, null, false, default(CancellationToken), null);
                }, "McModificationStart");
                if (ModMinecraft.McInstanceLock.TryAcquire(redoableWorker, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.PathVersion))
                {
                    redoableWorker.Stopped += ((ModDownloadLib._Closure$__.$I55-5 == null) ? (ModDownloadLib._Closure$__.$I55-5 = delegate
                    {
                        ModBase.RunInUi((ModDownloadLib._Closure$__.$I55-6 == null) ? (ModDownloadLib._Closure$__.$I55-6 = delegate
                        {
                            ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
                        }) : ModDownloadLib._Closure$__.$I55-6, false);
                    }) : ModDownloadLib._Closure$__.$I55-5);
                    redoableWorker.Start(default(CancellationToken), null);
                    ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
                    ModLoader.LoaderTaskbarAdd<string>(CS$<>8__locals2.$VB$Local_Loader);
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
            ModDownloadLib._Closure$__56-0 CS$<>8__locals1 = new ModDownloadLib._Closure$__56-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_TargetFolder = TargetFolder;
            McInstallManifest mcInstallManifest = Configs.McInstallManifest.Get(Instance.Config);
            IReadOnlyList<McInstallManifest.ManagedFile> readOnlyList = ((mcInstallManifest != null) ? mcInstallManifest.ManagedFiles : null) ?? new McInstallManifest.ManagedFile[0];
            IReadOnlyList<McInstallManifest.ManagedFile> managedFiles = NewManifest.ManagedFiles;
            List<string> list = new List<string>();
            List<ValueTuple<string, string>> list2 = new List<ValueTuple<string, string>>();
            try
            {
                foreach (McInstallManifest.ManagedFile managedFile in managedFiles)
                {
                    list2.Add(new ValueTuple<string, string>(Path.Combine(BuildFolder, managedFile.RelativePath), Path.Combine(CS$<>8__locals1.$VB$Local_TargetFolder, managedFile.RelativePath)));
                }
            }
            finally
            {
                IEnumerator<McInstallManifest.ManagedFile> enumerator;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            HashSet<string> hashSet = new HashSet<string>(managedFiles.Select<McInstallManifest.ManagedFile, string>((ModDownloadLib._Closure$__.$I56-0 == null) ? (ModDownloadLib._Closure$__.$I56-0 = (McInstallManifest.ManagedFile File) => File.RelativePath) : ModDownloadLib._Closure$__.$I56-0));
            try
            {
                foreach (McInstallManifest.ManagedFile managedFile2 in readOnlyList)
                {
                    if (!hashSet.Contains(managedFile2.RelativePath))
                    {
                        list.Add(PathUtils.ForCompare(Path.Combine(CS$<>8__locals1.$VB$Local_TargetFolder, managedFile2.RelativePath)));
                    }
                }
            }
            finally
            {
                IEnumerator<McInstallManifest.ManagedFile> enumerator2;
                if (enumerator2 != null)
                {
                    enumerator2.Dispose();
                }
            }
            if (DirectoryUtils.Exists(BuildFolder + "resources\\"))
            {
                DirectoryUtils.Copy(BuildFolder + "resources\\", CS$<>8__locals1.$VB$Local_TargetFolder + "resources\\");
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
                List<string>.Enumerator enumerator3;
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
                List<ValueTuple<string, string>>.Enumerator enumerator4;
                ((IDisposable)enumerator4).Dispose();
            }
            List<McInstallManifest.ManagedFile> list3 = managedFiles.Select<McInstallManifest.ManagedFile, McInstallManifest.ManagedFile>((McInstallManifest.ManagedFile File) => new McInstallManifest.ManagedFile(File.RelativePath, File.Sha1, FileUtils.GetInfo(Path.Combine(CS$<>8__locals1.$VB$Local_TargetFolder, File.RelativePath)).LastWriteTimeUtc, File.Component)).ToList<McInstallManifest.ManagedFile>();
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
                ModMain.MyMsgBox(Loader.Error.GetDisplay(true), Loader.Name + "失败", "确定", "", "", true, true, false, null, null, null);
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
            if (!Operators.ConditionalCompareObjectNotEqual(NewLateBinding.LateGet(Loader, null, "State", new object[0], null, null, null), ModBase.LoadState.Failed, false) || !Operators.ConditionalCompareObjectNotEqual(NewLateBinding.LateGet(Loader, null, "State", new object[0], null, null, null), ModBase.LoadState.Canceled, false))
            {
                ModBase.RunInThread(delegate
                {
                    try
                    {
                        Thread.Sleep(1000);
                        if (DirectoryUtils.Exists(Conversions.ToString(Operators.ConcatenateObject(NewLateBinding.LateGet(Loader, null, "Input", new object[0], null, null, null), "saves\\"))) || DirectoryUtils.Exists(Conversions.ToString(Operators.ConcatenateObject(NewLateBinding.LateGet(Loader, null, "Input", new object[0], null, null, null), "versions\\"))))
                        {
                            Logger.Warn(string.Format("由于版本已被独立启动，不清理版本文件夹：{0}", RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(Loader, null, "Input", new object[0], null, null, null))), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                        }
                        else
                        {
                            Logger.Warn(string.Format("由于下载失败或取消，清理版本文件夹：{0}", RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(Loader, null, "Input", new object[0], null, null, null))), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                            DirectoryUtils.Delete(Conversions.ToString(NewLateBinding.LateGet(Loader, null, "Input", new object[0], null, null, null)), false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "下载失败或取消后清理版本文件夹失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\ModDownloadLib.vb");
                    }
                });
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
    }
}
