using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using MeloongCore;
using MeloongCore.Extensions;
using MeloongCore.Wpf;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCLCS;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageInstanceExport : OurPageRight, IRefreshable
    {
        public PageInstanceExport()
        {
            base.Initialized += this.PageInstanceExport_Initialized;
            base.Loaded += this._Lambda___R0_1;
            base.Loaded += this._Lambda___R0_2;
            this.CurrentInstance = "";
            this.SubOptionBlackList = new string[] { "Quark Programmer Art.zip", "+ EuphoriaPatches_", "PCL2 Skin.zip" };
            this._RulesOverrides = null;
            this.ExtraFiles = null;
            this.ConfigPackPath = null;
            this.InitializeComponent();
            this.TextExportName_WithEvents = this.TextExportName;
            this.CardOptions_WithEvents = this.CardOptions;
            this.CheckOptionsPcl_WithEvents = this.CheckOptionsPcl;
            this.CheckAdvancedInclude_WithEvents = this.CheckAdvancedInclude;
            this.CheckAdvancedModrinth_WithEvents = this.CheckAdvancedModrinth;
            this.BtnAdvancedImport_WithEvents = this.BtnAdvancedImport;
            this.BtnAdvancedExport_WithEvents = this.BtnAdvancedExport;
            this.BtnExport_WithEvents = this.BtnExport;
        }

        private void PageInstanceExport_Initialized(object sender, EventArgs e)
        {
            PageInstanceLeft.CurrentJavaWorker.Stopped += this._Lambda___2_0;
        }

        private void PageInstanceExport_Loaded()
        {
            checked
            {
                ModAnimation.AniControlEnabled++;
                if (!PathUtils.IsSame(this.CurrentInstance, PageInstanceLeft.Instance.PathVersion))
                {
                    this.RefreshAll();
                }
                CustomEventService.SetEventData(this.BtnAdvancedHelp, "指南/整合包制作 - Snapshot.json");
                ModAnimation.AniControlEnabled--;
            }
        }

        void IRefreshable.Refresh()
        {
            this.RefreshAll();
        }

        public void RefreshAll()
        {
            Logger.Info("刷新导出页面", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
            this.HintOptiFine.Visibility = ((PageInstanceLeft.Instance.Version.HasOptiFine && !PageInstanceLeft.Instance.Version.OptiFineAsMod) ? Visibility.Visible : Visibility.Collapsed);
            this.CurrentInstance = PageInstanceLeft.Instance.PathVersion;
            this.TextExportName.Text = "";
            this.TextExportName.HintText = PageInstanceLeft.Instance.Name;
            this.TextExportVersion.Text = "";
            this.TextExportVersion.HintText = "1.0.0";
            this.CheckAdvancedInclude.Checked = false;
            this.CheckAdvancedModrinth.Checked = false;
            this.GetExportOption(this.CheckOptionsBasic).Description = PageInstanceLeft.Instance.VersionDisplayName();
            this.ResetConfigOverrides();
            this.ReloadAllSubOptions();
            this.RefreshAllOptionsUI();
            this.RefreshJavaInfo();
            this.PanBack.ScrollToHome();
        }

        private void ReloadAllSubOptions()
        {
            this.ReloadSubOptions(this.PanOptionsResourcePacks, true, true, new string[] { "resourcepacks", "texturepacks" });
            this.ReloadSubOptions(this.PanOptionsSaves, false, true, new string[] { "saves" });
            this.ReloadSubOptions(this.PanOptionsShaderPacks, true, true, new string[] { "shaderpacks" });
        }

        private void ReloadSubOptions(StackPanel Panel, bool AcceptCompressedFile, bool AcceptFolder, params string[] Folders)
        {
            Panel.Children.Clear();
            foreach (string text in Folders)
            {
                DirectoryInfo info = DirectoryUtils.GetInfo(PageInstanceLeft.Instance.PathIndie + text);
                if (info.Exists)
                {
                    if (AcceptCompressedFile)
                    {
                        try
                        {
                            IEnumerator<FileInfo> enumerator = info.EnumerateFiles("*.zip").Concat<FileInfo>(info.EnumerateFiles("*.rar")).GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                PageInstanceExport._Closure___7_0 CS_locals1 = new PageInstanceExport._Closure___7_0(null);
                                CS_locals1._VB_Local_File = enumerator.Current;
                                if (!this.SubOptionBlackList.Any<string>(new Func<string, bool>(CS_locals1._Lambda___0)))
                                {
                                    Panel.Children.Add(new OurCheckBox
                                    {
                                        Tag = new ExportOption
                                        {
                                            Title = CS_locals1._VB_Local_File.Name,
                                            DefaultChecked = true,
                                            Rules = StringUtils.LikePatternEscape(string.Format("{0}/{1}", text, CS_locals1._VB_Local_File.Name))
                                        }
                                    });
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator<FileInfo> enumerator = default;
                            if (enumerator != null)
                            {
                                enumerator.Dispose();
                            }
                        }
                    }
                    if (AcceptFolder)
                    {
                        try
                        {
                            IEnumerator<DirectoryInfo> enumerator2 = info.EnumerateDirectories().OrderByDescending<DirectoryInfo, DateTime>((PageInstanceExport._Closure___._I7_1 == null) ? (PageInstanceExport._Closure___._I7_1 = new Func<DirectoryInfo, DateTime>(PageInstanceExport._Closure___._I._Lambda___7_1)) : PageInstanceExport._Closure___._I7_1).GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                PageInstanceExport._Closure___7_1 CS_locals2 = new PageInstanceExport._Closure___7_1(null);
                                CS_locals2._VB_Local_SubFolder = enumerator2.Current;
                                if (!this.SubOptionBlackList.Any<string>(new Func<string, bool>(CS_locals2._Lambda___2)) && CS_locals2._VB_Local_SubFolder.EnumerateFileSystemInfos().Any<FileSystemInfo>())
                                {
                                    OurCheckBox ourCheckBox = new OurCheckBox
                                    {
                                        Tag = new ExportOption
                                        {
                                            Title = CS_locals2._VB_Local_SubFolder.Name,
                                            DefaultChecked = true,
                                            Rules = StringUtils.LikePatternEscape(string.Format("{0}/{1}/", text, CS_locals2._VB_Local_SubFolder.Name))
                                        }
                                    };
                                    if (AcceptCompressedFile)
                                    {
                                        this.GetExportOption(ourCheckBox).Description = "文件夹";
                                    }
                                    if (Panel == this.PanOptionsSaves)
                                    {
                                        this.GetExportOption(ourCheckBox).Description = CS_locals2._VB_Local_SubFolder.LastWriteTime.ToString("yyyy'/'MM'/'dd HH':'mm");
                                    }
                                    Panel.Children.Add(ourCheckBox);
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator<DirectoryInfo> enumerator2 = default;
                            if (enumerator2 != null)
                            {
                                enumerator2.Dispose();
                            }
                        }
                    }
                }
            }
        }

        private void RefreshAllOptionsUI()
        {
            PageInstanceExport._Closure___8_2 CS_locals1 = new PageInstanceExport._Closure___8_2(null);
            CS_locals1._VB_Me = this;
            CS_locals1._VB_Local_AllEntries = new List<string>();
            CS_locals1._VB_Local_IsValidDirectory = new VB_AnonymousDelegate_14<DirectoryInfo, bool>(this._Lambda___8_0);
            DirectoryInfo info = DirectoryUtils.GetInfo(PageInstanceLeft.Instance.PathIndie);
            CS_locals1._VB_Local_AllEntries.AddRange(info.EnumerateFiles().Select<FileInfo, string>((PageInstanceExport._Closure___._I8_3 == null) ? (PageInstanceExport._Closure___._I8_3 = new Func<FileInfo, string>(PageInstanceExport._Closure___._I._Lambda___8_3)) : PageInstanceExport._Closure___._I8_3));
            try
            {
                IEnumerable<DirectoryInfo> enumerable = info.EnumerateDirectories();
                VB_AnonymousDelegate_14<DirectoryInfo, bool> vb_AnonymousDelegate_ = CS_locals1._VB_Local_IsValidDirectory;
                IEnumerator<DirectoryInfo> enumerator = enumerable.Where<DirectoryInfo>((vb_AnonymousDelegate_ == null) ? null : new Func<DirectoryInfo, bool>(vb_AnonymousDelegate_.Invoke)).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    PageInstanceExport._Closure___8_1 CS_locals2 = new PageInstanceExport._Closure___8_1(null);
                    CS_locals2._VB_Local_SubFolder = enumerator.Current;
                    CS_locals1._VB_Local_AllEntries.Add(string.Format("{0}\\", CS_locals2._VB_Local_SubFolder.Name));
                    CS_locals1._VB_Local_AllEntries.AddRange(CS_locals2._VB_Local_SubFolder.EnumerateFiles().Select<FileInfo, string>(new Func<FileInfo, string>(CS_locals2._Lambda___4)));
                    List<string> _VB_Local_AllEntries = CS_locals1._VB_Local_AllEntries;
                    IEnumerable<DirectoryInfo> enumerable2 = CS_locals2._VB_Local_SubFolder.EnumerateDirectories();
                    vb_AnonymousDelegate_ = CS_locals1._VB_Local_IsValidDirectory;
                    _VB_Local_AllEntries.AddRange(enumerable2.Where<DirectoryInfo>((vb_AnonymousDelegate_ == null) ? null : new Func<DirectoryInfo, bool>(vb_AnonymousDelegate_.Invoke)).Select<DirectoryInfo, string>(new Func<DirectoryInfo, string>(CS_locals2._Lambda___5)));
                }
            }
            finally
            {
                IEnumerator<DirectoryInfo> enumerator = default;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            Logger.Info(string.Format("共发现 {0} 个可行的二级文件/文件夹", CS_locals1._VB_Local_AllEntries.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
            VB_AnonymousDelegate_15<ExportOption, bool> vb_AnonymousDelegate_2 = new VB_AnonymousDelegate_15<ExportOption, bool>(CS_locals1._Lambda___6);
            try
            {
                foreach (OurCheckBox ourCheckBox in this.GetAllOptions(true))
                {
                    ExportOption exportOption = this.GetExportOption(ourCheckBox);
                    if (string.IsNullOrEmpty(exportOption.Rules) && string.IsNullOrEmpty(exportOption.ShowRules))
                    {
                        ourCheckBox.Visibility = Visibility.Visible;
                        ourCheckBox.Checked = exportOption.DefaultChecked;
                    }
                    else
                    {
                        bool flag = vb_AnonymousDelegate_2(exportOption);
                        ourCheckBox.Visibility = (flag ? Visibility.Visible : Visibility.Collapsed);
                        ourCheckBox.Checked = exportOption.DefaultChecked && flag;
                    }
                    ourCheckBox.Inlines.Clear();
                    if (ourCheckBox.Visibility == Visibility.Visible)
                    {
                        ourCheckBox.Inlines.Add(new Run(exportOption.Title));
                        if (!string.IsNullOrEmpty(exportOption.Description))
                        {
                            ourCheckBox.Inlines.Add(new Run("   " + exportOption.Description)
                            {
                                Foreground = ModSecret.ColorGray5
                            });
                        }
                    }
                }
            }
            finally
            {
                IEnumerator<OurCheckBox> enumerator2 = default;
                if (enumerator2 != null)
                {
                    enumerator2.Dispose();
                }
            }
        }

        private IEnumerable<string> StandardizeLines(IEnumerable<string> Raw, bool AddSuffixStarToFolderPath)
        {
            PageInstanceExport.VB_StateMachine_9_StandardizeLines vb_StateMachine_9_StandardizeLines = new PageInstanceExport.VB_StateMachine_9_StandardizeLines(-2);
            vb_StateMachine_9_StandardizeLines._VB_Me = this;
            vb_StateMachine_9_StandardizeLines._P_Raw = Raw;
            vb_StateMachine_9_StandardizeLines._P_AddSuffixStarToFolderPath = AddSuffixStarToFolderPath;
            return vb_StateMachine_9_StandardizeLines;
        }

        private IEnumerable<OurCheckBox> GetAllOptions(bool IncludeHidden)
        {
            foreach (object objectValue in this.PanOptions.Children)
            {
                if (IncludeHidden || !Operators.ConditionalCompareObjectNotEqual(NewLateBinding.LateGet(objectValue, null, "Visibility", new object[0], null, null, null), Visibility.Visible, false))
                {
                    if (objectValue is OurCheckBox)
                    {
                        yield return (OurCheckBox)objectValue;
                    }
                    if (objectValue is StackPanel)
                    {
                        foreach (object objectValue2 in ((StackPanel)objectValue).Children)
                        {
                            if ((IncludeHidden || !Operators.ConditionalCompareObjectNotEqual(NewLateBinding.LateGet(objectValue2, null, "Visibility", new object[0], null, null, null), Visibility.Visible, false)) && objectValue2 is OurCheckBox)
                            {
                                yield return (OurCheckBox)objectValue2;
                            }
                        }
                    }
                }
            }
        }

        private ExportOption GetExportOption(OurCheckBox CheckBox)
        {
            return (ExportOption)CheckBox.Tag;
        }

        private List<string> RulesOverrides
        {
            get
            {
                return this._RulesOverrides;
            }
            set
            {
                this._RulesOverrides = value;
                if (value == null)
                {
                    this.BtnOverrideCancel.Visibility = Visibility.Collapsed;
                    this.PanOptions.Visibility = Visibility.Visible;
                    this.CardOptions.Inlines.Clear();
                    this.CardOptions.Inlines.Add(new Run("导出内容列表")
                    {
                        FontWeight = FontWeights.Bold
                    });
                    return;
                }
                this.BtnOverrideCancel.Visibility = Visibility.Visible;
                this.PanOptions.Visibility = Visibility.Collapsed;
                this.CardOptions.Inlines.Clear();
                this.CardOptions.Inlines.Add(new Run("导出内容列表:\u200a\u200a\u200a\u200a")
                {
                    FontWeight = FontWeights.Bold
                });
                this.CardOptions.Inlines.Add(new Run("从配置文件中读取")
                {
                    FontWeight = FontWeights.Normal
                });
            }
        }

        private IEnumerable<string> GetAllRules()
        {
            PageInstanceExport.VB_StateMachine_17_GetAllRules vb_StateMachine_17_GetAllRules = new PageInstanceExport.VB_StateMachine_17_GetAllRules(-2);
            vb_StateMachine_17_GetAllRules._VB_Me = this;
            return vb_StateMachine_17_GetAllRules;
        }

        private IEnumerable<string> GetExtraFileLines()
        {
            PageInstanceExport.VB_StateMachine_19_GetExtraFileLines vb_StateMachine_19_GetExtraFileLines = new PageInstanceExport.VB_StateMachine_19_GetExtraFileLines(-2);
            vb_StateMachine_19_GetExtraFileLines._VB_Me = this;
            return vb_StateMachine_19_GetExtraFileLines;
        }

        private void ResetConfigOverrides()
        {
            this.RulesOverrides = null;
            this.ConfigPackPath = null;
            this.ExtraFiles = null;
            this.PanBack.ScrollToHome();
        }

        private void CardOptions_MouseLeftButtonDown()
        {
            if (this.RulesOverrides != null)
            {
                this.ResetConfigOverrides();
            }
        }

        private void ExportConfig()
        {
            try
            {
                string text = Dialogs.SaveFile("选择文件位置", "export_config.txt", Settings.Get<string>("CacheExportConfig", null), new ValueTuple<string, string>[]
                {
                    new ValueTuple<string, string>("txt", "整合包导出配置")
                });
                if (text != null)
                {
                    Settings.Set("CacheExportConfig", text, null);
                    List<string> list = new List<string>();
                    list.Add("Name:" + this.TextExportName.Text);
                    list.Add("Version:" + this.TextExportVersion.Text);
                    list.Add("");
                    list.Add(string.Format("# 当版本设置中选择了 {0}使用版本文件夹中的 Java{1} 时，是否打包版本文件夹中的 Java。", ModBase.vbLQ, ModBase.vbRQ));
                    list.Add("IncludeJava:" + Conversions.ToString(this.CheckOptionsJava.Checked));
                    list.Add("");
                    list.Add("# 是否打包正式版 PCL，以便没有启动器的玩家安装整合包。");
                    list.Add("IncludeLauncher:" + Conversions.ToString(this.CheckOptionsPcl.Checked));
                    list.Add("");
                    list.Add("# 是否打包 PCL 个性化内容，例如功能隐藏设置、主页、背景音乐和图片等。");
                    list.Add("IncludeLauncherCustom:" + Conversions.ToString(this.CheckOptionsPclCustom.Checked));
                    list.Add("");
                    list.Add("# 是否将 Mod、资源包、光影包的文件直接放入整合包中，这样在导入时就无需联网下载它们。");
                    list.Add("# 建议仅在无法稳定连接 CurseForge 或 Modrinth 时才考虑启用。");
                    list.Add("# 二次分发可能违反使用协议，请尽量不要公开发布包含资源文件的整合包！");
                    list.Add("DontCheckHostedAssets:" + Conversions.ToString(this.CheckAdvancedInclude.Checked));
                    list.Add("");
                    list.Add("# 若资源未在 Modrinth 上托管，则将其原始文件直接放入整合包中。");
                    list.Add("# 如果你想将整合包上传到 Modrinth，这能帮助你分辨哪些文件未在 Modrinth 上托管。");
                    list.Add("# 此选项与 DontCheckHostedAssets 冲突。");
                    list.Add("ModrinthUploadMode:" + Conversions.ToString(this.CheckAdvancedModrinth.Checked));
                    list.Add("");
                    list.Add("# 导出的文件的存放位置。");
                    list.Add("# 若设置了此项，在导出时会直接将文件放到此路径，不会弹窗要求选择。");
                    list.Add("# 若 IncludeLauncher 为 True，应以 .zip 结尾；若为 False，应以 .mrpack 结尾。");
                    list.Add("PackPath:" + this.ConfigPackPath);
                    list.Add("");
                    list.Add("==============================================================");
                    list.AddRange(this.GetAllRules());
                    list.Add("==============================================================");
                    list.AddRange(this.GetExtraFileLines());
                    FileUtils.Write(text, list.Join("\r\n"), null);
                    ModMain.Hint("已保存配置文件：" + text, ModMain.HintType.Green, true);
                    ModBase.OpenExplorer(text);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "保存配置失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
            }
        }

        private void ImportConfig()
        {
            try
            {
                string text = Dialogs.SelectFile("选择配置文件", false, Settings.Get<string>("CacheExportConfig", null), new OneOf<IEnumerable<ValueTuple<string[], string>>, string>?(new ValueTuple<string[], string>[]
                {
                    new ValueTuple<string[], string>(new string[] { "txt" }, "整合包导出配置")
                })).FirstOrDefault<string>();
                if (!string.IsNullOrEmpty(text))
                {
                    Settings.Set("CacheExportConfig", text, null);
                    string[] array = FileUtils.ReadAsString(text, null, null).Split("==============================================================", false);
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    foreach (string text2_it in array[0].SplitLines(true))
                    {
                        string text2 = text2_it;
                        text2 = text2.Trim();
                        if (Operators.CompareString(text2, "", false) != 0 && !text2.StartsWithF("#", false) && !text2.StartsWithF("=", false))
                        {
                            int num = text2.IndexOfF(":", false);
                            if (num > 0)
                            {
                                dictionary[text2.Substring(0, num)] = text2.Substring(checked(num + 1));
                            }
                        }
                    }
                    this.TextExportName.Text = dictionary.GetOrDefault("Name", "");
                    this.TextExportVersion.Text = dictionary.GetOrDefault("Version", "");
                    this.CheckOptionsJava.Checked = Conversions.ToBoolean(dictionary.GetOrDefault("IncludeJava", Conversions.ToString(true)));
                    this.CheckOptionsPcl.Checked = Conversions.ToBoolean(dictionary.GetOrDefault("IncludeLauncher", Conversions.ToString(true)));
                    this.CheckOptionsPclCustom.Checked = Conversions.ToBoolean(dictionary.GetOrDefault("IncludeLauncherCustom", Conversions.ToString(true)));
                    this.CheckAdvancedInclude.Checked = Conversions.ToBoolean(dictionary.GetOrDefault("DontCheckHostedAssets", Conversions.ToString(false)));
                    if (!this.CheckAdvancedInclude.Checked)
                    {
                        this.CheckAdvancedModrinth.Checked = Conversions.ToBoolean(dictionary.GetOrDefault("ModrinthUploadMode", Conversions.ToString(false)));
                    }
                    this.ConfigPackPath = dictionary.GetOrDefault("PackPath", null);
                    this.RulesOverrides = array[1].SplitLines(true).ToList<string>();
                    if (array.Length > 2)
                    {
                        this.ExtraFiles = array[2].SplitLines(true).ToList<string>();
                    }
                    else
                    {
                        this.ExtraFiles = null;
                    }
                    ModMain.Hint("已读取配置文件：" + text, ModMain.HintType.Green, true);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "读取配置失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
            }
        }

        private void StartExport()
        {
            PageInstanceExport._Closure___25_0 CS_locals1 = new PageInstanceExport._Closure___25_0(null);
            CS_locals1._VB_Local_PackName = (string.IsNullOrEmpty(this.TextExportName.Text) ? this.TextExportName.HintText : this.TextExportName.Text);
            CS_locals1._VB_Local_PackVersion = (string.IsNullOrEmpty(this.TextExportVersion.Text) ? "1.0.0" : this.TextExportVersion.Text);
            CS_locals1._VB_Local_LoaderName = "导出整合包：" + CS_locals1._VB_Local_PackName;
            try
            {
                IEnumerator<ModLoader.LoaderBase> enumerator = ModLoader.LoaderTaskbar.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (Operators.CompareString(enumerator.Current.Name, CS_locals1._VB_Local_LoaderName, false) == 0)
                    {
                        ModMain.FrmMain.PageChange(FormMain.PageType.TaskManager, FormMain.PageSubType.Default);
                        return;
                    }
                }
            }
            finally
            {
                IEnumerator<ModLoader.LoaderBase> enumerator = default;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            CS_locals1._VB_Local_PackPath = null;
            if (!string.IsNullOrWhiteSpace(this.ConfigPackPath) && !this.ConfigPackPath.EndsWithF("\\", false) && !this.ConfigPackPath.EndsWithF("/", false))
            {
                try
                {
                    DirectoryUtils.Create(PathUtils.RemoveLastPart(this.ConfigPackPath));
                    CS_locals1._VB_Local_PackPath = this.ConfigPackPath;
                    Logger.Info(string.Format("使用配置文件中指定的导出路径：{0}", this.ConfigPackPath), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, string.Format("无法使用配置文件中指定的导出路径（{0}）", this.ConfigPackPath), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                    if (ModMain.OurMsgBox(string.Format("指定的路径：{0}{1}{2}{3}", new object[]
                    {
                        this.ConfigPackPath,
                        "\r\n",
                        "\r\n",
                        ex.GetDisplay(true)
                    }), "无法使用配置文件中指定的导出路径", "确定", "取消", "", false, true, false, null, null, null) == 2)
                    {
                        return;
                    }
                }
            }
            if (CS_locals1._VB_Local_PackPath == null)
            {
                List<ValueTuple<string, string>> list = new List<ValueTuple<string, string>>();
                if (!this.CheckOptionsPcl.Checked && this.CheckAdvancedModrinth.Checked)
                {
                    list.Add(new ValueTuple<string, string>("mrpack", "Modrinth 整合包文件"));
                }
                list.Add(new ValueTuple<string, string>("zip", "压缩文件"));
                if (!this.CheckOptionsPcl.Checked && !this.CheckAdvancedModrinth.Checked)
                {
                    list.Add(new ValueTuple<string, string>("mrpack", "Modrinth 整合包文件"));
                }
                CS_locals1._VB_Local_PackPath = Dialogs.SaveFile("选择导出位置", CS_locals1._VB_Local_PackName + (string.IsNullOrEmpty(this.TextExportVersion.Text) ? "" : (" " + this.TextExportVersion.Text)), null, list);
                Logger.Info(string.Format("手动指定的导出路径：{0}", CS_locals1._VB_Local_PackPath), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
            }
            if (CS_locals1._VB_Local_PackPath != null)
            {
                CS_locals1._VB_Local_CacheFolder = ModMain.RequestTaskTempFolder(false);
                CS_locals1._VB_Local_OverridesFolder = CS_locals1._VB_Local_CacheFolder + "modpack\\overrides\\";
                CS_locals1._VB_Local_TargetInstance = PageInstanceLeft.Instance;
                CS_locals1._VB_Local_IncludeJava = (this.RulesOverrides == null && this.CheckOptionsJava.Visibility == Visibility.Visible && this.CheckOptionsJava.Checked) || (this.RulesOverrides != null && this.CheckOptionsJava.Checked && Settings.Get<int>("VersionArgumentJavaV2", CS_locals1._VB_Local_TargetInstance) == 2);
                CS_locals1._VB_Local_PathIndie = CS_locals1._VB_Local_TargetInstance.PathIndie;
                CS_locals1._VB_Local_CheckHostedAssets = !this.CheckAdvancedInclude.Checked;
                CS_locals1._VB_Local_ModrinthUploadMode = this.CheckAdvancedModrinth.Checked;
                CS_locals1._VB_Local_IncludePCL = this.CheckOptionsPcl.Checked;
                CS_locals1._VB_Local_IncludePCLCustom = CS_locals1._VB_Local_IncludePCL && this.CheckOptionsPclCustom.Checked;
                CS_locals1._VB_Local_AllRules = this.StandardizeLines(this.GetAllRules(), true).ToList<string>();
                CS_locals1._VB_Local_AllExtraFiles = this.StandardizeLines(this.GetExtraFileLines(), false).ToList<string>();
                Logger.Info(string.Format("准备导出整合包，共有 {0} 条规则，{1} 条追加内容行", CS_locals1._VB_Local_AllRules.Count, CS_locals1._VB_Local_AllExtraFiles.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                List<ModLoader.LoaderBase> list2 = new List<ModLoader.LoaderBase>();
                if (CS_locals1._VB_Local_IncludePCL)
                {
                    list2.Add(new ModLoader.LoaderTask<int, int>("下载 PCL 正式版", new Action<ModLoader.LoaderTask<int, int>>(CS_locals1._Lambda___0), null, ThreadPriority.Normal)
                    {
                        ProgressWeight = 0.5,
                        Block = false
                    });
                }
                list2.Add(new ModLoader.LoaderTask<int, List<LocalResourceFile>>("复制导出内容", new Action<ModLoader.LoaderTask<int, List<LocalResourceFile>>>(CS_locals1._Lambda___1), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 5.0
                });
                list2.Add(new ModLoader.LoaderTask<List<LocalResourceFile>, Dictionary<LocalResourceFile, List<string>>>("联网获取文件信息", new Action<ModLoader.LoaderTask<List<LocalResourceFile>, Dictionary<LocalResourceFile, List<string>>>>(CS_locals1._Lambda___4), null, ThreadPriority.Normal)
                {
                    Show = CS_locals1._VB_Local_CheckHostedAssets,
                    ProgressWeight = (CS_locals1._VB_Local_CheckHostedAssets ? 2.0 : 0.01)
                });
                list2.Add(new ModLoader.LoaderTask<Dictionary<LocalResourceFile, List<string>>, int>("生成压缩包", new Action<ModLoader.LoaderTask<Dictionary<LocalResourceFile, List<string>>, int>>(CS_locals1._Lambda___10), null, ThreadPriority.Normal)
                {
                    ProgressWeight = 6.0
                });
                ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>(CS_locals1._VB_Local_LoaderName, list2);
                loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(ModDownloadLib.LoaderStateChangedHintOnly);
                loaderCombo.Start(null, false);
                ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
                ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                ModMain.FrmMain.BtnExtraDownload.Ribble();
                ModMain.FrmMain.PageChange(FormMain.PageType.TaskManager, FormMain.PageSubType.Default);
            }
        }

        private void RefreshJavaInfo()
        {
            int num = Settings.Get<int>("VersionArgumentJavaV2", PageInstanceLeft.Instance);
            RedoableWorker<Java> currentJavaWorker = PageInstanceLeft.CurrentJavaWorker;
            Java java = ((num == 2 && currentJavaWorker.LastSucceeded && !currentJavaWorker.Running) ? currentJavaWorker.LastResult : null);
            if (java == null)
            {
                this.CheckOptionsJava.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.CheckOptionsJava.Visibility = Visibility.Visible;
                this.CheckOptionsJava.Inlines.Clear();
                this.CheckOptionsJava.Inlines.Add(new Run(this.GetExportOption(this.CheckOptionsJava).Title));
                this.CheckOptionsJava.Inlines.Add(new Run("   " + string.Format("Java {0} ({1})：{2}", java.Version.Major, java.Version, PathUtils.AddSlashSuffix(PathUtils.RemoveLastPart(java.Folder.Replace(PathUtils.RemoveLastPart(PathUtils.RemoveLastPart(PageInstanceLeft.Instance.PathVersion)), ""))).TrimStart(new char[] { '\\' })))
                {
                    Foreground = ModSecret.ColorGray5
                });
            }
            if (num == 3)
            {
                this.BtnExport.IsEnabled = true;
                this.HintJava.Visibility = Visibility.Visible;
                this.HintJava.Theme = OurHint.Themes.Yellow;
                this.HintJava.Text = string.Format("该版本设置要求 {0}使用指定的 Java{1}，这在导出后很可能会失效。建议修改为 {2}使用版本文件夹中的 Java{3}。", new object[]
                {
                    ModBase.vbLQ,
                    ModBase.vbRQ,
                    ModBase.vbLQ,
                    ModBase.vbRQ
                });
                return;
            }
            if (num == 2 && currentJavaWorker.HasSucceeded && currentJavaWorker.LastResult == null)
            {
                this.BtnExport.IsEnabled = false;
                this.HintJava.Visibility = Visibility.Visible;
                this.HintJava.Theme = OurHint.Themes.Red;
                this.HintJava.Text = string.Format("该版本设置要求 {0}使用版本文件夹中的 Java{1}，但版本文件夹中没能找到任何 Java。", ModBase.vbLQ, ModBase.vbRQ);
                return;
            }
            this.BtnExport.IsEnabled = true;
            this.HintJava.Visibility = Visibility.Collapsed;
        }

        private void TextExportName_GotFocus()
        {
            if (Operators.CompareString(this.TextExportName.Text, "", false) == 0)
            {
                this.TextExportName.Text = this.TextExportName.HintText;
                this.TextExportName.SelectionStart = this.TextExportName.Text.Length;
            }
        }

        private void CheckAdvancedInclude_Change(object sender, bool user)
        {
            if (this.CheckAdvancedInclude.Checked)
            {
                this.CheckAdvancedModrinth.Checked = false;
            }
            this.CheckAdvancedModrinth.IsEnabled = !this.CheckAdvancedInclude.Checked;
        }

        private void CheckAdvancedModrinth_Change(object sender, bool user)
        {
            this.HintModrinthFormat.Visibility = (this.CheckAdvancedModrinth.Checked && this.CheckOptionsPcl.Checked).ToVisibility();
        }





        internal virtual OurTextBox TextExportName_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._TextExportName;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                RoutedEventHandler routedEventHandler = new RoutedEventHandler(this._Lambda___R48_3);
                OurTextBox ourTextBox = this._TextExportName;
                if (ourTextBox != null)
                {
                    ourTextBox.GotFocus -= routedEventHandler;
                }
                this._TextExportName = value;
                ourTextBox = this._TextExportName;
                if (ourTextBox != null)
                {
                    ourTextBox.GotFocus += routedEventHandler;
                }
            }
        }


        internal virtual OurCard CardOptions_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._CardOptions;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this._Lambda___R56_4);
                OurCard ourCard = this._CardOptions;
                if (ourCard != null)
                {
                    ourCard.MouseLeftButtonDown -= mouseButtonEventHandler;
                }
                this._CardOptions = value;
                ourCard = this._CardOptions;
                if (ourCard != null)
                {
                    ourCard.MouseLeftButtonDown += mouseButtonEventHandler;
                }
            }
        }














        internal virtual OurCheckBox CheckOptionsPcl_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._CheckOptionsPcl;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this.CheckAdvancedModrinth_Change);
                OurCheckBox ourCheckBox = this._CheckOptionsPcl;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                }
                this._CheckOptionsPcl = value;
                ourCheckBox = this._CheckOptionsPcl;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                }
            }
        }


        internal virtual OurCheckBox CheckAdvancedInclude_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._CheckAdvancedInclude;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this.CheckAdvancedInclude_Change);
                OurCheckBox ourCheckBox = this._CheckAdvancedInclude;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                }
                this._CheckAdvancedInclude = value;
                ourCheckBox = this._CheckAdvancedInclude;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual OurCheckBox CheckAdvancedModrinth_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._CheckAdvancedModrinth;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this.CheckAdvancedModrinth_Change);
                OurCheckBox ourCheckBox = this._CheckAdvancedModrinth;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                }
                this._CheckAdvancedModrinth = value;
                ourCheckBox = this._CheckAdvancedModrinth;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                }
            }
        }


        internal virtual OurButton BtnAdvancedImport_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnAdvancedImport;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R132_5);
                OurButton ourButton = this._BtnAdvancedImport;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnAdvancedImport = value;
                ourButton = this._BtnAdvancedImport;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnAdvancedExport_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnAdvancedExport;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R136_6);
                OurButton ourButton = this._BtnAdvancedExport;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnAdvancedExport = value;
                ourButton = this._BtnAdvancedExport;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }


        internal virtual OurExtraTextButton BtnExport_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnExport;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurExtraTextButton.ClickEventHandler clickEventHandler = new OurExtraTextButton.ClickEventHandler(this._Lambda___R144_7);
                OurExtraTextButton ourExtraTextButton = this._BtnExport;
                if (ourExtraTextButton != null)
                {
                    ourExtraTextButton.Click -= clickEventHandler;
                }
                this._BtnExport = value;
                ourExtraTextButton = this._BtnExport;
                if (ourExtraTextButton != null)
                {
                    ourExtraTextButton.Click += clickEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R0_1(object a0, RoutedEventArgs a1)
        {
            this.PageInstanceExport_Loaded();
        }

        [CompilerGenerated]
        private void _Lambda___R0_2(object a0, RoutedEventArgs a1)
        {
            this.RefreshJavaInfo();
        }

        [CompilerGenerated]
        private void _Lambda___2_0()
        {
            ModBase.RunInUi(new Action(this.RefreshJavaInfo), false);
        }

        [CompilerGenerated]
        private bool _Lambda___8_0(DirectoryInfo Folder)
        {
            bool flag;
            try
            {
                flag = Folder.Exists && Folder.EnumerateFileSystemInfos().Any<FileSystemInfo>(new Func<FileSystemInfo, bool>(this._Lambda___8_1));
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }

        [CompilerGenerated]
        private bool _Lambda___8_1(FileSystemInfo i)
        {
            PageInstanceExport._Closure___8_0 CS_locals1 = new PageInstanceExport._Closure___8_0(null);
            CS_locals1._VB_Local_i = i;
            return !this.SubOptionBlackList.Any<string>(new Func<string, bool>(CS_locals1._Lambda___2));
        }

        [CompilerGenerated]
        private void _Lambda___R48_3(object a0, RoutedEventArgs a1)
        {
            this.TextExportName_GotFocus();
        }

        [CompilerGenerated]
        private void _Lambda___R56_4(object a0, MouseButtonEventArgs a1)
        {
            this.CardOptions_MouseLeftButtonDown();
        }

        [CompilerGenerated]
        private void _Lambda___R132_5(object a0, MouseButtonEventArgs a1)
        {
            this.ImportConfig();
        }

        [CompilerGenerated]
        private void _Lambda___R136_6(object a0, MouseButtonEventArgs a1)
        {
            this.ExportConfig();
        }

        [CompilerGenerated]
        private void _Lambda___R144_7(object a0, MouseButtonEventArgs a1)
        {
            this.StartExport();
        }

        private string CurrentInstance;

        private string[] SubOptionBlackList;

        private const string Sperator = "==============================================================";

        private List<string> _RulesOverrides;

        private List<string> ExtraFiles;

        private string ConfigPackPath;

        [CompilerGenerated]
        [AccessedThroughProperty("PanAllBack")]
        private Grid _PanAllBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurScrollViewer _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("HintOptiFine")]
        private OurHint _HintOptiFine;

        [CompilerGenerated]
        [AccessedThroughProperty("HintJava")]
        private OurHint _HintJava;

        [CompilerGenerated]
        [AccessedThroughProperty("TextExportName")]
        private OurTextBox _TextExportName;

        [CompilerGenerated]
        [AccessedThroughProperty("TextExportVersion")]
        private OurTextBox _TextExportVersion;

        [CompilerGenerated]
        [AccessedThroughProperty("CardOptions")]
        private OurCard _CardOptions;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnOverrideCancel")]
        private OurIconTextButton _BtnOverrideCancel;

        [CompilerGenerated]
        [AccessedThroughProperty("PanOptions")]
        private StackPanel _PanOptions;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckOptionsBasic")]
        private OurCheckBox _CheckOptionsBasic;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckOptionsOptions")]
        private OurCheckBox _CheckOptionsOptions;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckOptionsMod")]
        private OurCheckBox _CheckOptionsMod;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckOptionsResourcePacks")]
        private OurCheckBox _CheckOptionsResourcePacks;

        [CompilerGenerated]
        [AccessedThroughProperty("PanOptionsResourcePacks")]
        private StackPanel _PanOptionsResourcePacks;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckOptionsShaderPacks")]
        private OurCheckBox _CheckOptionsShaderPacks;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckOptionsShaderPackSettings")]
        private OurCheckBox _CheckOptionsShaderPackSettings;

        [CompilerGenerated]
        [AccessedThroughProperty("PanOptionsShaderPacks")]
        private StackPanel _PanOptionsShaderPacks;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckOptionsSaves")]
        private OurCheckBox _CheckOptionsSaves;

        [CompilerGenerated]
        [AccessedThroughProperty("PanOptionsSaves")]
        private StackPanel _PanOptionsSaves;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckOptionsJava")]
        private OurCheckBox _CheckOptionsJava;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckOptionsPcl")]
        private OurCheckBox _CheckOptionsPcl;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckOptionsPclCustom")]
        private OurCheckBox _CheckOptionsPclCustom;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckAdvancedInclude")]
        private OurCheckBox _CheckAdvancedInclude;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckAdvancedModrinth")]
        private OurCheckBox _CheckAdvancedModrinth;

        [CompilerGenerated]
        [AccessedThroughProperty("HintModrinthFormat")]
        private OurHint _HintModrinthFormat;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnAdvancedImport")]
        private OurButton _BtnAdvancedImport;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnAdvancedExport")]
        private OurButton _BtnAdvancedExport;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnAdvancedHelp")]
        private OurButton _BtnAdvancedHelp;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnExport")]
        private OurExtraTextButton _BtnExport;

        [CompilerGenerated]
        private sealed class VB_StateMachine_17_GetAllRules : IEnumerable<string>, IEnumerable, IEnumerator<string>, IDisposable, IEnumerator
        {
            public VB_StateMachine_17_GetAllRules(int _State)
            {
                this._State = _State;
                this._InitialThreadId = Environment.CurrentManagedThreadId;
            }

            void IDisposable.Dispose()
            {
                int num = this._State;
                if (num != 1)
                {
                    switch (num)
                    {
                    case 6:
                    case 8:
                        this._State = -4;
                        break;
                    case 7:
                        this._State = -5;
                        break;
                    default:
                        this._State = -1;
                        break;
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
                int num = this._State;
                switch (num)
                {
                case -5:
                case -4:
                case 6:
                case 7:
                case 8:
                    goto IL_0183;
                case -3:
                case 1:
                    break;
                default:
                    return false;
                case 0:
                    num = (this._State = -1);
                    if (this._VB_Me.RulesOverrides == null)
                    {
                        goto IL_0107;
                    }
                    break;
                case 2:
                    num = (this._State = -1);
                    this._Current = "# 修改下方的规则以控制需要导出的内容。";
                    num = (this._State = 3);
                    return true;
                case 3:
                    num = (this._State = -1);
                    this._Current = "# 以 ! 开头以反选。可以使用 *、?、[] 通配符。靠后的行覆盖靠前的。";
                    num = (this._State = 4);
                    return true;
                case 4:
                    num = (this._State = -1);
                    this._Current = "";
                    num = (this._State = 5);
                    return true;
                case 5:
                    num = (this._State = -1);
                    goto IL_0183;
                case 9:
                    num = (this._State = -1);
                    this._Current = "!*.log";
                    num = (this._State = 10);
                    return true;
                case 10:
                    num = (this._State = -1);
                    this._Current = "!*.dat_old";
                    num = (this._State = 11);
                    return true;
                case 11:
                    num = (this._State = -1);
                    this._Current = "!*.BakaCoreInfo";
                    num = (this._State = 12);
                    return true;
                case 12:
                    num = (this._State = -1);
                    this._Current = "!hmclversion.cfg";
                    num = (this._State = 13);
                    return true;
                case 13:
                    num = (this._State = -1);
                    this._Current = "!log4j2.xml";
                    num = (this._State = 14);
                    return true;
                case 14:
                    num = (this._State = -1);
                    this._Current = "";
                    num = (this._State = 15);
                    return true;
                case 15:
                    num = (this._State = -1);
                    return false;
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
                        this._S0 = this._VB_Me.RulesOverrides.GetEnumerator();
                    }
                    else
                    {
                        num = (this._State = -1);
                    }
                    if (!this._S0.MoveNext())
                    {
                        return false;
                    }
                    string text = this._S0.Current;
                    this._Current = text;
                    num = (this._State = 1);
                    return true;
                }
                finally
                {
                    if (num < 0)
                    {
                        ((IDisposable)this._S0).Dispose();
                    }
                }
                IL_0107:
                this._Current = "";
                num = (this._State = 2);
                return true;
                IL_0183:
                try
                {
                    if (num != -5)
                    {
                        if (num != -4)
                        {
                            switch (num)
                            {
                            case 6:
                                num = (this._State = -1);
                                goto IL_0326;
                            case 7:
                                goto IL_0326;
                            case 8:
                                num = (this._State = -1);
                                break;
                            default:
                                this._S1 = this._VB_Me.GetAllOptions(false).GetEnumerator();
                                break;
                            }
                            while (this._S1.MoveNext())
                            {
                                OurCheckBox ourCheckBox = this._S1.Current;
                                if (ourCheckBox.Checked)
                                {
                                    ExportOption exportOption = this._VB_Me.GetExportOption(ourCheckBox);
                                    string rules = exportOption.Rules;
                                    this._VB_ResumableLocal_Rules_3 = ((rules != null) ? rules.Split(new char[] { '|' }).ToList<string>() : null) ?? new List<string>();
                                    if (ourCheckBox == this._VB_Me.CheckOptionsShaderPackSettings)
                                    {
                                        try
                                        {
                                            this._S2 = this._VB_Me.PanOptionsShaderPacks.Children.OfType<OurCheckBox>().GetEnumerator();
                                            while (this._S2.MoveNext())
                                            {
                                                OurCheckBox ourCheckBox2 = this._S2.Current;
                                                if (ourCheckBox2.Checked)
                                                {
                                                    ExportOption exportOption2 = this._VB_Me.GetExportOption(ourCheckBox2);
                                                    string text2 = string.Format("shaderpacks/{0}.txt", exportOption2.Title);
                                                    if (FileUtils.Exists(Path.Combine(PageInstanceLeft.Instance.PathIndie, text2)))
                                                    {
                                                        this._VB_ResumableLocal_Rules_3.Add(StringUtils.LikePatternEscape(text2));
                                                    }
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            if (num < 0 && this._S2 != null)
                                            {
                                                this._S2.Dispose();
                                            }
                                        }
                                    }
                                    if (this._VB_ResumableLocal_Rules_3.Any<string>())
                                    {
                                        this._Current = string.Format("# {0}", exportOption.Title);
                                        num = (this._State = 6);
                                        return true;
                                    }
                                }
                            }
                            goto IL_03F6;
                        }
                        num = (this._State = -1);
                        return true;
                    }
                    IL_0326:
                    try
                    {
                        if (num == -5)
                        {
                            num = (this._State = -1);
                            return true;
                        }
                        if (num != 7)
                        {
                            this._S4 = this._VB_ResumableLocal_Rules_3.GetEnumerator();
                        }
                        else
                        {
                            num = (this._State = -1);
                        }
                        if (this._S4.MoveNext())
                        {
                            string text3 = this._S4.Current;
                            this._Current = text3;
                            num = (this._State = 7);
                            return true;
                        }
                    }
                    finally
                    {
                        if (num < 0)
                        {
                            ((IDisposable)this._S4).Dispose();
                        }
                    }
                    this._Current = "";
                    num = (this._State = 8);
                    return true;
                }
                finally
                {
                    if (num < 0 && this._S1 != null)
                    {
                        this._S1.Dispose();
                    }
                }
                IL_03F6:
                this._Current = "# 排除的文件";
                num = (this._State = 9);
                return true;
            }

            public IEnumerator<string> GetEnumerator()
            {
                PageInstanceExport.VB_StateMachine_17_GetAllRules vb_StateMachine_17_GetAllRules;
                if (this._State == -2 && this._InitialThreadId == Environment.CurrentManagedThreadId)
                {
                    this._State = 0;
                    vb_StateMachine_17_GetAllRules = this;
                }
                else
                {
                    vb_StateMachine_17_GetAllRules = new PageInstanceExport.VB_StateMachine_17_GetAllRules(0);
                    vb_StateMachine_17_GetAllRules._VB_Me = this._VB_Me;
                }
                return vb_StateMachine_17_GetAllRules;
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

            internal PageInstanceExport _VB_Me;

            internal List<string>.Enumerator _S0;

            internal IEnumerator<OurCheckBox> _S1;

            internal IEnumerator<OurCheckBox> _S2;

            internal List<string> _VB_ResumableLocal_Rules_3;

            internal List<string>.Enumerator _S4;
        }

        [CompilerGenerated]
        private sealed class VB_StateMachine_19_GetExtraFileLines : IEnumerable<string>, IEnumerable, IEnumerator<string>, IDisposable, IEnumerator
        {
            public VB_StateMachine_19_GetExtraFileLines(int _State)
            {
                this._State = _State;
                this._InitialThreadId = Environment.CurrentManagedThreadId;
            }

            void IDisposable.Dispose()
            {
                int num = this._State;
                if (num == 1)
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
                    break;
                default:
                    return false;
                case 0:
                    num = (this._State = -1);
                    if (this._VB_Me.ExtraFiles == null)
                    {
                        goto IL_00D7;
                    }
                    break;
                case 2:
                    num = (this._State = -1);
                    this._Current = "# 如果想将额外的文件自动放到压缩包根目录中，可以将它们的路径写在下方。";
                    num = (this._State = 3);
                    return true;
                case 3:
                    num = (this._State = -1);
                    this._Current = "# 必须是完整路径。每行中，若以 \\ 结尾则代表是文件夹，不以 \\ 结尾则代表是文件。";
                    num = (this._State = 4);
                    return true;
                case 4:
                    num = (this._State = -1);
                    this._Current = "";
                    num = (this._State = 5);
                    return true;
                case 5:
                    num = (this._State = -1);
                    return false;
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
                        this._S0 = this._VB_Me.ExtraFiles.GetEnumerator();
                    }
                    else
                    {
                        num = (this._State = -1);
                    }
                    if (!this._S0.MoveNext())
                    {
                        return false;
                    }
                    string text = this._S0.Current;
                    this._Current = text;
                    num = (this._State = 1);
                    return true;
                }
                finally
                {
                    if (num < 0)
                    {
                        ((IDisposable)this._S0).Dispose();
                    }
                }
                IL_00D7:
                this._Current = "";
                num = (this._State = 2);
                return true;
            }

            public IEnumerator<string> GetEnumerator()
            {
                PageInstanceExport.VB_StateMachine_19_GetExtraFileLines vb_StateMachine_19_GetExtraFileLines;
                if (this._State == -2 && this._InitialThreadId == Environment.CurrentManagedThreadId)
                {
                    this._State = 0;
                    vb_StateMachine_19_GetExtraFileLines = this;
                }
                else
                {
                    vb_StateMachine_19_GetExtraFileLines = new PageInstanceExport.VB_StateMachine_19_GetExtraFileLines(0);
                    vb_StateMachine_19_GetExtraFileLines._VB_Me = this._VB_Me;
                }
                return vb_StateMachine_19_GetExtraFileLines;
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

            internal PageInstanceExport _VB_Me;

            internal List<string>.Enumerator _S0;
        }

        [CompilerGenerated]
        private sealed class VB_StateMachine_9_StandardizeLines : IEnumerable<string>, IEnumerable, IEnumerator<string>, IDisposable, IEnumerator
        {
            public VB_StateMachine_9_StandardizeLines(int _State)
            {
                this._State = _State;
                this._InitialThreadId = Environment.CurrentManagedThreadId;
            }

            void IDisposable.Dispose()
            {
                int num = this._State;
                if (num == 1)
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
                    break;
                default:
                    return false;
                case 0:
                    num = (this._State = -1);
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
                        this._S0 = this._VB_Local_Raw.GetEnumerator();
                    }
                    else
                    {
                        num = (this._State = -1);
                    }
                    while (this._S0.MoveNext())
                    {
                        string text = this._S0.Current;
                        text = text.Trim();
                        if (Operators.CompareString(text, "", false) != 0 && !text.StartsWithF("#", false) && !text.StartsWithF("=", false))
                        {
                            text = text.Replace("/", "\\");
                            this._Current = text + ((text.EndsWithF("\\", false) && this._VB_Local_AddSuffixStarToFolderPath) ? "*" : "");
                            num = (this._State = 1);
                            return true;
                        }
                    }
                }
                finally
                {
                    if (num < 0 && this._S0 != null)
                    {
                        this._S0.Dispose();
                    }
                }
                return false;
            }

            public IEnumerator<string> GetEnumerator()
            {
                PageInstanceExport.VB_StateMachine_9_StandardizeLines vb_StateMachine_9_StandardizeLines;
                if (this._State == -2 && this._InitialThreadId == Environment.CurrentManagedThreadId)
                {
                    this._State = 0;
                    vb_StateMachine_9_StandardizeLines = this;
                }
                else
                {
                    vb_StateMachine_9_StandardizeLines = new PageInstanceExport.VB_StateMachine_9_StandardizeLines(0);
                    vb_StateMachine_9_StandardizeLines._VB_Me = this._VB_Me;
                }
                vb_StateMachine_9_StandardizeLines._VB_Local_Raw = this._P_Raw;
                vb_StateMachine_9_StandardizeLines._VB_Local_AddSuffixStarToFolderPath = this._P_AddSuffixStarToFolderPath;
                return vb_StateMachine_9_StandardizeLines;
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

            internal IEnumerable<string> _VB_Local_Raw;

            internal IEnumerable<string> _P_Raw;

            internal bool _VB_Local_AddSuffixStarToFolderPath;

            internal bool _P_AddSuffixStarToFolderPath;

            internal PageInstanceExport _VB_Me;

            internal IEnumerator<string> _S0;
        }

        [CompilerGenerated]
        [Serializable]
        private new sealed class _Closure___
        {
            internal DateTime _Lambda___7_1(DirectoryInfo f)
            {
                return f.LastWriteTime;
            }

            internal string _Lambda___8_3(FileInfo f)
            {
                return f.Name;
            }

            internal string _Lambda___25_6(LocalResourceFile m)
            {
                return m.ModrinthHash;
            }

            internal uint _Lambda___25_8(LocalResourceFile m)
            {
                return m.CurseForgeHash;
            }

            internal bool _Lambda___25_11(string u)
            {
                return u.Contains("modrinth.com");
            }

            public static readonly PageInstanceExport._Closure___ _I = new PageInstanceExport._Closure___();

            public static Func<DirectoryInfo, DateTime> _I7_1;

            public static Func<FileInfo, string> _I8_3;

            public static Func<LocalResourceFile, string> _I25_6;

            public static Func<LocalResourceFile, uint> _I25_8;

            public static Func<string, bool> _I25_11;
        }

        [CompilerGenerated]
        private sealed class _Closure___25_0
        {
            public _Closure___25_0(PageInstanceExport._Closure___25_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_CacheFolder = arg0._VB_Local_CacheFolder;
                    this._VB_Local_PathIndie = arg0._VB_Local_PathIndie;
                    this._VB_Local_AllRules = arg0._VB_Local_AllRules;
                    this._VB_Local_OverridesFolder = arg0._VB_Local_OverridesFolder;
                    this._VB_Local_CheckHostedAssets = arg0._VB_Local_CheckHostedAssets;
                    this._VB_Local_IncludeJava = arg0._VB_Local_IncludeJava;
                    this._VB_Local_TargetInstance = arg0._VB_Local_TargetInstance;
                    this._VB_Local_IncludePCL = arg0._VB_Local_IncludePCL;
                    this._VB_Local_AllExtraFiles = arg0._VB_Local_AllExtraFiles;
                    this._VB_Local_IncludePCLCustom = arg0._VB_Local_IncludePCLCustom;
                    this._VB_Local_LoaderName = arg0._VB_Local_LoaderName;
                    this._VB_Local_ModrinthUploadMode = arg0._VB_Local_ModrinthUploadMode;
                    this._VB_Local_PackVersion = arg0._VB_Local_PackVersion;
                    this._VB_Local_PackName = arg0._VB_Local_PackName;
                    this._VB_Local_PackPath = arg0._VB_Local_PackPath;
                }
            }

            internal void _Lambda___0(ModLoader.LoaderTask<int, int> Loader)
            {
                ModSecret.DownloadLatestPCL(Loader);
                FileUtils.Copy(ModBase.PathTemp + "Latest.exe", this._VB_Local_CacheFolder + "OpenPCL.exe");
            }

            internal void _Lambda___1(ModLoader.LoaderTask<int, List<LocalResourceFile>> Loader)
            {
                PageInstanceExport._Closure___25_1 CS_locals1 = new PageInstanceExport._Closure___25_1(null);
                CS_locals1._VB_NonLocal__VB_Closure_2 = this;
                CS_locals1._VB_Local_Loader = Loader;
                CS_locals1._VB_Local_Loader.Output = new List<LocalResourceFile>();
                CS_locals1._VB_Local_Progress = 0;
                CS_locals1._VB_Local_SearchFolder = new Action<DirectoryInfo>(CS_locals1._Lambda___2);
                CS_locals1._VB_Local_SearchFolder(DirectoryUtils.GetInfo(this._VB_Local_PathIndie));
                Logger.Info(string.Format("复制 overrides 文件完成，有 {0} 个文件需要联网检查", CS_locals1._VB_Local_Loader.Output.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                CS_locals1._VB_Local_Loader.Progress = 0.85;
                if (this._VB_Local_IncludeJava)
                {
                    Java java = ModJava.SelectOrDownloadJava(this._VB_Local_TargetInstance, false, CS_locals1._VB_Local_Loader.CreateCancellationToken(), CS_locals1._VB_Local_Loader.CreateSyncProgressProvider(0.85, 0.93));
                    if (java == null)
                    {
                        throw new FileNotFoundException("版本文件夹中没有找到可导出的 Java。");
                    }
                    string text = PathUtils.RemoveLastPart(java.Folder);
                    string text2 = Path.Combine(this._VB_Local_OverridesFolder, text.AfterFirst(this._VB_Local_TargetInstance.PathVersion, true));
                    DirectoryUtils.Copy(text, text2);
                    Logger.Info(string.Format("已复制版本文件夹中的 Java：{0} → {1}", text, text2), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                }
                CS_locals1._VB_Local_Loader.Progress = 0.95;
                string text3 = (this._VB_Local_IncludePCL ? this._VB_Local_CacheFolder : (this._VB_Local_CacheFolder + "modpack\\"));
                try
                {
                    foreach (string text4 in this._VB_Local_AllExtraFiles)
                    {
                        if (text4.EndsWithF("\\", false) || text4.EndsWithF("/", false))
                        {
                            if (DirectoryUtils.Exists(text4))
                            {
                                DirectoryUtils.Copy(text4, text3 + PathUtils.GetLastPart(text4) + "\\");
                            }
                            else
                            {
                                ModMain.Hint(string.Format("未找到配置文件中指定的文件夹：{0}", text4), ModMain.HintType.Red, true);
                            }
                        }
                        else if (FileUtils.Exists(text4))
                        {
                            FileUtils.Copy(text4, text3 + PathUtils.GetLastPart(text4));
                        }
                        else
                        {
                            ModMain.Hint(string.Format("未找到配置文件中指定的单个文件：{0}", text4), ModMain.HintType.Red, true);
                        }
                    }
                }
                finally
                {
                    List<string>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
                CS_locals1._VB_Local_Loader.Progress = 0.97;
                if (DirectoryUtils.Exists(this._VB_Local_TargetInstance.PathVersion + "PCL\\"))
                {
                    DirectoryUtils.Copy(this._VB_Local_TargetInstance.PathVersion + "PCL\\", this._VB_Local_OverridesFolder + "PCL\\");
                }
                ModBase.WriteIni(this._VB_Local_OverridesFolder + "PCL\\Setup.ini", "IsStar", Conversions.ToString(false));
                if (this._VB_Local_IncludePCLCustom)
                {
                    if (DirectoryUtils.Exists(Paths.Base + "PCL\\Pictures\\"))
                    {
                        DirectoryUtils.Copy(Paths.Base + "PCL\\Pictures\\", this._VB_Local_CacheFolder + "PCL\\Pictures\\");
                    }
                    if (DirectoryUtils.Exists(Paths.Base + "PCL\\Musics\\"))
                    {
                        DirectoryUtils.Copy(Paths.Base + "PCL\\Musics\\", this._VB_Local_CacheFolder + "PCL\\Musics\\");
                    }
                    if (DirectoryUtils.Exists(Paths.Base + "PCL\\Help\\"))
                    {
                        DirectoryUtils.Copy(Paths.Base + "PCL\\Help\\", this._VB_Local_CacheFolder + "PCL\\Help\\");
                    }
                    if (FileUtils.Exists(Paths.Base + "PCL\\Custom.xaml"))
                    {
                        FileUtils.Copy(Paths.Base + "PCL\\Custom.xaml", this._VB_Local_CacheFolder + "PCL\\Custom.xaml");
                    }
                    if (FileUtils.Exists(Paths.Base + "PCL\\Setup.ini"))
                    {
                        FileUtils.Copy(Paths.Base + "PCL\\Setup.ini", this._VB_Local_CacheFolder + "PCL\\Setup.ini");
                    }
                    if (FileUtils.Exists(Paths.Base + "PCL\\hints.txt"))
                    {
                        FileUtils.Copy(Paths.Base + "PCL\\hints.txt", this._VB_Local_CacheFolder + "PCL\\hints.txt");
                    }
                    if (FileUtils.Exists(Paths.Base + "PCL\\Logo.png"))
                    {
                        FileUtils.Copy(Paths.Base + "PCL\\Logo.png", this._VB_Local_CacheFolder + "PCL\\Logo.png");
                    }
                }
            }

            internal void _Lambda___4(ModLoader.LoaderTask<List<LocalResourceFile>, Dictionary<LocalResourceFile, List<string>>> Loader)
            {
                PageInstanceExport._Closure___25_3 CS_locals1 = new PageInstanceExport._Closure___25_3(null);
                CS_locals1._VB_NonLocal__VB_Closure_3 = this;
                CS_locals1._VB_Local_Loader = Loader;
                CS_locals1._VB_Local_Loader.Output = new Dictionary<LocalResourceFile, List<string>>();
                if (!this._VB_Local_CheckHostedAssets)
                {
                    Logger.Info("要求跳过联网获取步骤", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                    return;
                }
                if (!CS_locals1._VB_Local_Loader.Input.Any<LocalResourceFile>())
                {
                    Logger.Info("没有需要联网检查的文件，跳过联网获取步骤", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                    return;
                }
                CS_locals1._VB_Local_EndedThreadCount = 0;
                CS_locals1._VB_Local_FailedExceptions = new List<Exception>();
                ModBase.RunInNewThread(new Action(CS_locals1._Lambda___5), "Modrinth - " + this._VB_Local_LoaderName, ThreadPriority.Normal);
                ModBase.RunInNewThread(new Action(CS_locals1._Lambda___7), "CurseForge - " + this._VB_Local_LoaderName, ThreadPriority.Normal);
                while (CS_locals1._VB_Local_EndedThreadCount != 2)
                {
                    if (CS_locals1._VB_Local_Loader.IsCanceled)
                    {
                        return;
                    }
                    Thread.Sleep(10);
                }
                if (CS_locals1._VB_Local_FailedExceptions.Count == 1)
                {
                    if (ModMain.OurMsgBox("联网获取部分文件信息失败，是否继续导出？\r\n\r\n若继续，无法获取信息的文件将被直接打包。\r\n由于二次分发可能违反使用协议，请尽量不要公开发布导出的整合包！", "部分文件信息获取失败", "继续", "取消", "", false, true, false, null, null, null) == 2)
                    {
                        throw CS_locals1._VB_Local_FailedExceptions.First<Exception>();
                    }
                    return;
                }
                else
                {
                    if (CS_locals1._VB_Local_FailedExceptions.Count > 1 && ModMain.OurMsgBox("联网获取文件信息失败，是否继续导出？\r\n\r\n若继续，所有文件都将被直接打包。\r\n由于二次分发可能违反使用协议，请尽量不要公开发布导出的整合包！", "文件信息获取失败", "继续", "取消", "", false, true, false, null, null, null) == 2)
                    {
                        throw CS_locals1._VB_Local_FailedExceptions.First<Exception>();
                    }
                    return;
                }
            }

            internal void _Lambda___10(ModLoader.LoaderTask<Dictionary<LocalResourceFile, List<string>>, int> Loader)
            {
                JArray jarray = new JArray();
                try
                {
                    foreach (KeyValuePair<LocalResourceFile, List<string>> keyValuePair in Loader.Input)
                    {
                        LocalResourceFile key = keyValuePair.Key;
                        jarray.Add(new JObject
                        {
                            {
                                "path",
                                PathUtils.RemoveExtendedPrefix(key.File.FullName).AfterFirst(this._VB_Local_OverridesFolder, false).Replace("\\", "/")
                            },
                            {
                                "hashes",
                                new JObject
                                {
                                    { "sha1", key.ModrinthHash },
                                    {
                                        "sha512",
                                        CryptographyUtils.ComputeFileHash(key.File.FullName, CryptographyUtils.HashMethod.Sha512)
                                    }
                                }
                            },
                            {
                                "downloads",
                                new JArray(keyValuePair.Value.OrderBy<string, bool>((PageInstanceExport._Closure___._I25_11 == null) ? (PageInstanceExport._Closure___._I25_11 = new Func<string, bool>(PageInstanceExport._Closure___._I._Lambda___25_11)) : PageInstanceExport._Closure___._I25_11))
                            },
                            {
                                "fileSize",
                                key.File.Length
                            }
                        });
                        key.File.Delete();
                    }
                }
                finally
                {
                    Dictionary<LocalResourceFile, List<string>>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
                Loader.Progress = 0.2;
                JObject jobject = new JObject { 
                {
                    "minecraft",
                    this._VB_Local_TargetInstance.Version.VanillaName
                } };
                if (this._VB_Local_TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.Forge))
                {
                    jobject.Add("forge", this._VB_Local_TargetInstance.Version.ModLoaderVersions[ModLoaders.Forge]);
                }
                if (this._VB_Local_TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.Fabric))
                {
                    jobject.Add("fabric-loader", this._VB_Local_TargetInstance.Version.ModLoaderVersions[ModLoaders.Fabric]);
                }
                if (this._VB_Local_TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge))
                {
                    jobject.Add("neoforge", this._VB_Local_TargetInstance.Version.ModLoaderVersions[ModLoaders.NeoForge]);
                }
                JObject jobject2 = new JObject
                {
                    { "game", "minecraft" },
                    { "formatVersion", 1 },
                    { "versionId", this._VB_Local_PackVersion },
                    { "name", this._VB_Local_PackName },
                    {
                        "summary",
                        this._VB_Local_TargetInstance.Info
                    },
                    { "files", jarray },
                    { "dependencies", jobject }
                };
                FileUtils.Write(this._VB_Local_CacheFolder + "modpack\\modrinth.index.json", jobject2.ToString(Formatting.Indented, new JsonConverter[0]), null);
                if (this._VB_Local_IncludePCL)
                {
                    FileUtils.CreateZipFromDirectory(this._VB_Local_CacheFolder + "modpack.mrpack", this._VB_Local_CacheFolder + "modpack\\");
                    Loader.Progress = 0.5;
                    DirectoryUtils.Delete(this._VB_Local_CacheFolder + "modpack\\", false);
                    Loader.Progress = 0.6;
                    FileUtils.CreateZipFromDirectory(this._VB_Local_PackPath, this._VB_Local_CacheFolder);
                    Loader.Progress = 0.9;
                }
                else
                {
                    FileUtils.CreateZipFromDirectory(this._VB_Local_PackPath, this._VB_Local_CacheFolder + "modpack\\");
                    Loader.Progress = 0.8;
                }
                ModBase.OpenExplorer(this._VB_Local_PackPath);
            }

            public string _VB_Local_CacheFolder;

            public string _VB_Local_PathIndie;

            public List<string> _VB_Local_AllRules;

            public string _VB_Local_OverridesFolder;

            public bool _VB_Local_CheckHostedAssets;

            public bool _VB_Local_IncludeJava;

            public ModMinecraft.McInstance _VB_Local_TargetInstance;

            public bool _VB_Local_IncludePCL;

            public List<string> _VB_Local_AllExtraFiles;

            public bool _VB_Local_IncludePCLCustom;

            public string _VB_Local_LoaderName;

            public bool _VB_Local_ModrinthUploadMode;

            public string _VB_Local_PackVersion;

            public string _VB_Local_PackName;

            public string _VB_Local_PackPath;
        }

        [CompilerGenerated]
        private sealed class _Closure___25_1
        {
            public _Closure___25_1(PageInstanceExport._Closure___25_1 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_SearchFolder = arg0._VB_Local_SearchFolder;
                    this._VB_Local_Loader = arg0._VB_Local_Loader;
                    this._VB_Local_Progress = arg0._VB_Local_Progress;
                }
            }

            internal void _Lambda___2(DirectoryInfo Folder)
            {
                try
                {
                    foreach (DirectoryInfo directoryInfo in Folder.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
                    {
                        if ((!new string[] { "assets", "versions", "libraries" }.Contains(directoryInfo.Name) || !PathUtils.IsSame(this._VB_NonLocal__VB_Closure_2._VB_Local_PathIndie, Folder.FullName)) && !new string[] { "structureCacheV1", ".fabric", ".git", "avatar-cache", "cosmetic-cache" }.Contains(directoryInfo.Name))
                        {
                            this._VB_Local_SearchFolder(directoryInfo);
                        }
                    }
                }
                finally
                {
                    IEnumerator<DirectoryInfo> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                try
                {
                    foreach (FileInfo fileInfo in Folder.EnumerateFiles("*", SearchOption.TopDirectoryOnly))
                    {
                        PageInstanceExport._Closure___25_2 CS_locals1 = new PageInstanceExport._Closure___25_2(null);
                        CS_locals1._VB_Local_RelativePath = fileInfo.FullName.AfterFirst(this._VB_NonLocal__VB_Closure_2._VB_Local_PathIndie, false);
                        bool flag = false;
                        try
                        {
                            foreach (string text in this._VB_NonLocal__VB_Closure_2._VB_Local_AllRules)
                            {
                                bool flag2 = text.StartsWithF("!", false);
                                if (LikeOperator.LikeString(CS_locals1._VB_Local_RelativePath.Lower(), text.TrimStart(new char[] { '!' }).Lower(), CompareMethod.Binary))
                                {
                                    flag = !flag2;
                                }
                            }
                        }
                        finally
                        {
                            List<string>.Enumerator enumerator3 = default;
                            ((IDisposable)enumerator3).Dispose();
                        }
                        if (flag)
                        {
                            string text2 = this._VB_NonLocal__VB_Closure_2._VB_Local_OverridesFolder + CS_locals1._VB_Local_RelativePath;
                            FileUtils.Copy(fileInfo.FullName, text2);
                            if (this._VB_NonLocal__VB_Closure_2._VB_Local_CheckHostedAssets && new string[] { ".zip", ".rar", ".jar", ".disabled", ".old" }.Contains(fileInfo.Extension.Lower()) && new string[] { "mods", "packs", "openloader", "resource" }.Any<string>(new Func<string, bool>(CS_locals1._Lambda___3)))
                            {
                                LocalResourceFile localResourceFile = new LocalResourceFile(text2);
                                string modrinthHash = localResourceFile.ModrinthHash;
                                Conversions.ToString(localResourceFile.CurseForgeHash);
                                this._VB_Local_Loader.Output.Add(localResourceFile);
                            }
                            checked
                            {
                                this._VB_Local_Progress++;
                            }
                            if (this._VB_Local_Progress == 25)
                            {
                                ModLoader.LoaderTask<int, List<LocalResourceFile>> loaderTask;
                                (loaderTask = this._VB_Local_Loader).Progress = loaderTask.Progress + (0.84 - this._VB_Local_Loader.Progress) * 0.012;
                                this._VB_Local_Progress = 0;
                            }
                        }
                    }
                }
                finally
                {
                    IEnumerator<FileInfo> enumerator2 = default;
                    if (enumerator2 != null)
                    {
                        enumerator2.Dispose();
                    }
                }
            }

            public Action<DirectoryInfo> _VB_Local_SearchFolder;

            public ModLoader.LoaderTask<int, List<LocalResourceFile>> _VB_Local_Loader;

            public int _VB_Local_Progress;

            public PageInstanceExport._Closure___25_0 _VB_NonLocal__VB_Closure_2;
        }

        [CompilerGenerated]
        private sealed class _Closure___25_2
        {
            public _Closure___25_2(PageInstanceExport._Closure___25_2 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_RelativePath = arg0._VB_Local_RelativePath;
                }
            }

            internal bool _Lambda___3(string s)
            {
                return this._VB_Local_RelativePath.Contains(s);
            }

            public string _VB_Local_RelativePath;
        }

        [CompilerGenerated]
        private sealed class _Closure___25_3
        {
            public _Closure___25_3(PageInstanceExport._Closure___25_3 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Loader = arg0._VB_Local_Loader;
                    this._VB_Local_FailedExceptions = arg0._VB_Local_FailedExceptions;
                    this._VB_Local_EndedThreadCount = arg0._VB_Local_EndedThreadCount;
                }
            }

            internal void _Lambda___5()
            {
                try
                {
                    IEnumerable<string> enumerable = this._VB_Local_Loader.Input.Select<LocalResourceFile, string>((PageInstanceExport._Closure___._I25_6 == null) ? (PageInstanceExport._Closure___._I25_6 = new Func<LocalResourceFile, string>(PageInstanceExport._Closure___._I._Lambda___25_6)) : PageInstanceExport._Closure___._I25_6);
                    JObject jobject = (JObject)ModDownload.DlModRequest("https://api.modrinth.com/v2/version_files", HttpMethod.Post, string.Format("{{\"hashes\": [\"{0}\"], \"algorithm\": \"sha1\"}}", enumerable.Join("\",\"")), "application/json");
                    try
                    {
                        foreach (LocalResourceFile localResourceFile in this._VB_Local_Loader.Input)
                        {
                            if (jobject.ContainsKey(localResourceFile.ModrinthHash))
                            {
                                JToken jtoken = jobject[localResourceFile.ModrinthHash];
                                JToken jtoken2;
                                if (jtoken == null)
                                {
                                    jtoken2 = null;
                                }
                                else
                                {
                                    JToken jtoken3 = jtoken["files"];
                                    if (jtoken3 == null)
                                    {
                                        jtoken2 = null;
                                    }
                                    else
                                    {
                                        JToken jtoken4 = jtoken3[0]["hashes"];
                                        jtoken2 = ((jtoken4 != null) ? jtoken4["sha1"] : null);
                                    }
                                }
                                if (!((string)jtoken2 != localResourceFile.ModrinthHash))
                                {
                                    this._VB_Local_Loader.Output.AddIntoValueCollection(localResourceFile, (string)jobject[localResourceFile.ModrinthHash]["files"][0]["url"]);
                                }
                            }
                        }
                    }
                    finally
                    {
                        List<LocalResourceFile>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                    Logger.Info(string.Format("从 Modrinth 获取到 {0} 个本地资源项的对应信息", jobject.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "从 Modrinth 获取本地 Mod 信息失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                    this._VB_Local_FailedExceptions.Add(ex);
                }
                finally
                {
                    checked
                    {
                        this._VB_Local_EndedThreadCount++;
                    }
                    ModLoader.LoaderTask<List<LocalResourceFile>, Dictionary<LocalResourceFile, List<string>>> loaderTask;
                    (loaderTask = this._VB_Local_Loader).Progress = loaderTask.Progress + 0.45;
                }
            }

            internal void _Lambda___7()
            {
                try
                {
                    if (!this._VB_NonLocal__VB_Closure_3._VB_Local_ModrinthUploadMode)
                    {
                        IEnumerable<uint> enumerable = this._VB_Local_Loader.Input.Select<LocalResourceFile, uint>((PageInstanceExport._Closure___._I25_8 == null) ? (PageInstanceExport._Closure___._I25_8 = new Func<LocalResourceFile, uint>(PageInstanceExport._Closure___._I._Lambda___25_8)) : PageInstanceExport._Closure___._I25_8);
                        JContainer jcontainer = (JContainer)ModDownload.DlModRequest("https://api.curseforge.com/v1/fingerprints/432", HttpMethod.Post, string.Format("{{\"fingerprints\": [{0}]}}", enumerable.Join(",")), "application/json")["data"]["exactMatches"];
                        try
                        {
                            foreach (JToken jtoken in ((IEnumerable<JToken>)jcontainer))
                            {
                                JObject jobject = (JObject)jtoken;
                                PageInstanceExport._Closure___25_4 CS_locals1 = new PageInstanceExport._Closure___25_4(null);
                                if (jobject.ContainsKey("file"))
                                {
                                    CS_locals1._VB_Local_File = (JObject)jobject["file"];
                                    if (!string.IsNullOrEmpty((string)CS_locals1._VB_Local_File["downloadUrl"]))
                                    {
                                        LocalResourceFile localResourceFile = this._VB_Local_Loader.Input.FirstOrDefault<LocalResourceFile>(new Func<LocalResourceFile, bool>(CS_locals1._Lambda___9));
                                        if (localResourceFile != null)
                                        {
                                            try
                                            {
                                                foreach (string text in ResourceVersion.ParseCurseForgeDownloadUrls(CS_locals1._VB_Local_File["downloadUrl"].ToString()))
                                                {
                                                    this._VB_Local_Loader.Output.AddIntoValueCollection(localResourceFile, text);
                                                }
                                            }
                                            finally
                                            {
                                                List<string>.Enumerator enumerator2 = default;
                                                ((IDisposable)enumerator2).Dispose();
                                            }
                                        }
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
                        Logger.Info(string.Format("从 CurseForge 获取到 {0} 个本地资源项的对应信息", jcontainer.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "从 CurseForge 获取本地 Mod 信息失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                    this._VB_Local_FailedExceptions.Add(ex);
                }
                finally
                {
                    checked
                    {
                        this._VB_Local_EndedThreadCount++;
                    }
                    ModLoader.LoaderTask<List<LocalResourceFile>, Dictionary<LocalResourceFile, List<string>>> loaderTask;
                    (loaderTask = this._VB_Local_Loader).Progress = loaderTask.Progress + 0.45;
                }
            }

            public ModLoader.LoaderTask<List<LocalResourceFile>, Dictionary<LocalResourceFile, List<string>>> _VB_Local_Loader;

            public List<Exception> _VB_Local_FailedExceptions;

            public int _VB_Local_EndedThreadCount;

            public PageInstanceExport._Closure___25_0 _VB_NonLocal__VB_Closure_3;
        }

        [CompilerGenerated]
        private sealed class _Closure___25_4
        {
            public _Closure___25_4(PageInstanceExport._Closure___25_4 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_File = arg0._VB_Local_File;
                }
            }

            internal bool _Lambda___9(LocalResourceFile m)
            {
                return m.CurseForgeHash == this._VB_Local_File["fileFingerprint"].ToObject<uint>();
            }

            public JObject _VB_Local_File;
        }

        [CompilerGenerated]
        private sealed class _Closure___7_0
        {
            public _Closure___7_0(PageInstanceExport._Closure___7_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_File = arg0._VB_Local_File;
                }
            }

            internal bool _Lambda___0(string b)
            {
                return this._VB_Local_File.Name.Contains(b);
            }

            public FileInfo _VB_Local_File;
        }

        [CompilerGenerated]
        private sealed class _Closure___7_1
        {
            public _Closure___7_1(PageInstanceExport._Closure___7_1 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_SubFolder = arg0._VB_Local_SubFolder;
                }
            }

            internal bool _Lambda___2(string b)
            {
                return this._VB_Local_SubFolder.Name.Contains(b);
            }

            public DirectoryInfo _VB_Local_SubFolder;
        }

        [CompilerGenerated]
        private sealed class _Closure___8_0
        {
            public _Closure___8_0(PageInstanceExport._Closure___8_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_i = arg0._VB_Local_i;
                }
            }

            internal bool _Lambda___2(string b)
            {
                return this._VB_Local_i.Name.Contains(b);
            }

            public FileSystemInfo _VB_Local_i;
        }

        [CompilerGenerated]
        private sealed class _Closure___8_1
        {
            public _Closure___8_1(PageInstanceExport._Closure___8_1 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_SubFolder = arg0._VB_Local_SubFolder;
                }
            }

            internal string _Lambda___4(FileInfo f)
            {
                return string.Format("{0}\\{1}", this._VB_Local_SubFolder.Name, f.Name);
            }

            internal string _Lambda___5(DirectoryInfo d)
            {
                return string.Format("{0}\\{1}\\", this._VB_Local_SubFolder.Name, d.Name);
            }

            public DirectoryInfo _VB_Local_SubFolder;
        }

        [CompilerGenerated]
        private sealed class _Closure___8_2
        {
            public _Closure___8_2(PageInstanceExport._Closure___8_2 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_AllEntries = arg0._VB_Local_AllEntries;
                    this._VB_Local_IsValidDirectory = arg0._VB_Local_IsValidDirectory;
                }
            }

            internal bool _Lambda___6(ExportOption TargetOption)
            {
                return (!TargetOption.RequireOptiFine || PageInstanceLeft.Instance.Version.HasOptiFine) && (!TargetOption.RequireModLoader || PageInstanceLeft.Instance.Modable) && (!TargetOption.RequireModLoaderOrOptiFine || PageInstanceLeft.Instance.Version.HasOptiFine || PageInstanceLeft.Instance.Modable) && this._VB_Me.StandardizeLines((TargetOption.Rules ?? TargetOption.ShowRules).Split(new char[] { '|' }), true).Any<string>((this._I7 == null) ? (this._I7 = new Func<string, bool>(this._Lambda___7)) : this._I7);
            }

            internal bool _Lambda___7(string Rule)
            {
                PageInstanceExport._Closure___8_3 CS_locals1 = new PageInstanceExport._Closure___8_3(null);
                CS_locals1._VB_Local_Rule = Rule;
                bool flag;
                if (CS_locals1._VB_Local_Rule.StartsWithF("!", false))
                {
                    flag = false;
                }
                else
                {
                    try
                    {
                        if (this._VB_Local_AllEntries.Any<string>(new Func<string, bool>(CS_locals1._Lambda___8)))
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, string.Format("错误的规则：{0}", CS_locals1._VB_Local_Rule), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                        return false;
                    }
                    CS_locals1._VB_Local_Rule = CS_locals1._VB_Local_Rule.Trim("*?".ToCharArray());
                    if (CS_locals1._VB_Local_Rule.Split('\\', true).Count<string>() >= 3)
                    {
                        if (CS_locals1._VB_Local_Rule.EndsWithF("\\", false))
                        {
                            flag = this._VB_Local_IsValidDirectory(DirectoryUtils.GetInfo(PageInstanceLeft.Instance.PathIndie + CS_locals1._VB_Local_Rule));
                        }
                        else
                        {
                            flag = FileUtils.Exists(PageInstanceLeft.Instance.PathIndie + CS_locals1._VB_Local_Rule);
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                }
                return flag;
            }

            public List<string> _VB_Local_AllEntries;

            public VB_AnonymousDelegate_14<DirectoryInfo, bool> _VB_Local_IsValidDirectory;

            public PageInstanceExport _VB_Me;

            public Func<string, bool> _I7;
        }

        [CompilerGenerated]
        private sealed class _Closure___8_3
        {
            public _Closure___8_3(PageInstanceExport._Closure___8_3 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Rule = arg0._VB_Local_Rule;
                }
            }

            internal bool _Lambda___8(string Entry)
            {
                return LikeOperator.LikeString(Entry.Lower(), this._VB_Local_Rule.Lower(), CompareMethod.Binary);
            }

            public string _VB_Local_Rule;
        }
    }
}
