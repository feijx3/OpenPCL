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
    public partial class PageInstanceExport : MyPageRight, IRefreshable
    {
        public PageInstanceExport()
        {
            base.Initialized += this.PageInstanceExport_Initialized;
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.PageInstanceExport_Loaded();
            };
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.RefreshJavaInfo();
            };
            this.CurrentInstance = "";
            this.SubOptionBlackList = new string[] { "Quark Programmer Art.zip", "+ EuphoriaPatches_", "PCL2 Skin.zip" };
            this._RulesOverrides = null;
            this.ExtraFiles = null;
            this.ConfigPackPath = null;
            this.InitializeComponent();
        }

        private void PageInstanceExport_Initialized(object sender, EventArgs e)
        {
            PageInstanceLeft.CurrentJavaWorker.Stopped += delegate
            {
                ModBase.RunInUi(new Action(this.RefreshJavaInfo), false);
            };
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
                                PageInstanceExport._Closure$__7-0 CS$<>8__locals1 = new PageInstanceExport._Closure$__7-0(CS$<>8__locals1);
                                CS$<>8__locals1.$VB$Local_File = enumerator.Current;
                                if (!this.SubOptionBlackList.Any<string>((string b) => CS$<>8__locals1.$VB$Local_File.Name.Contains(b)))
                                {
                                    Panel.Children.Add(new MyCheckBox
                                    {
                                        Tag = new ExportOption
                                        {
                                            Title = CS$<>8__locals1.$VB$Local_File.Name,
                                            DefaultChecked = true,
                                            Rules = StringUtils.LikePatternEscape(string.Format("{0}/{1}", text, CS$<>8__locals1.$VB$Local_File.Name))
                                        }
                                    });
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator<FileInfo> enumerator;
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
                            IEnumerator<DirectoryInfo> enumerator2 = info.EnumerateDirectories().OrderByDescending<DirectoryInfo, DateTime>((PageInstanceExport._Closure$__.$I7-1 == null) ? (PageInstanceExport._Closure$__.$I7-1 = (DirectoryInfo f) => f.LastWriteTime) : PageInstanceExport._Closure$__.$I7-1).GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                PageInstanceExport._Closure$__7-1 CS$<>8__locals2 = new PageInstanceExport._Closure$__7-1(CS$<>8__locals2);
                                CS$<>8__locals2.$VB$Local_SubFolder = enumerator2.Current;
                                if (!this.SubOptionBlackList.Any<string>((string b) => CS$<>8__locals2.$VB$Local_SubFolder.Name.Contains(b)) && CS$<>8__locals2.$VB$Local_SubFolder.EnumerateFileSystemInfos().Any<FileSystemInfo>())
                                {
                                    MyCheckBox myCheckBox = new MyCheckBox
                                    {
                                        Tag = new ExportOption
                                        {
                                            Title = CS$<>8__locals2.$VB$Local_SubFolder.Name,
                                            DefaultChecked = true,
                                            Rules = StringUtils.LikePatternEscape(string.Format("{0}/{1}/", text, CS$<>8__locals2.$VB$Local_SubFolder.Name))
                                        }
                                    };
                                    if (AcceptCompressedFile)
                                    {
                                        this.GetExportOption(myCheckBox).Description = "文件夹";
                                    }
                                    if (Panel == this.PanOptionsSaves)
                                    {
                                        this.GetExportOption(myCheckBox).Description = CS$<>8__locals2.$VB$Local_SubFolder.LastWriteTime.ToString("yyyy'/'MM'/'dd HH':'mm");
                                    }
                                    Panel.Children.Add(myCheckBox);
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator<DirectoryInfo> enumerator2;
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
            PageInstanceExport._Closure$__8-2 CS$<>8__locals1 = new PageInstanceExport._Closure$__8-2(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Me = this;
            CS$<>8__locals1.$VB$Local_AllEntries = new List<string>();
            CS$<>8__locals1.$VB$Local_IsValidDirectory = delegate(DirectoryInfo Folder)
            {
                bool flag2;
                try
                {
                    flag2 = Folder.Exists && Folder.EnumerateFileSystemInfos().Any<FileSystemInfo>(delegate(FileSystemInfo i)
                    {
                        PageInstanceExport._Closure$__8-0 CS$<>8__locals3 = new PageInstanceExport._Closure$__8-0(CS$<>8__locals3);
                        CS$<>8__locals3.$VB$Local_i = i;
                        return !this.SubOptionBlackList.Any<string>((string b) => CS$<>8__locals3.$VB$Local_i.Name.Contains(b));
                    });
                }
                catch (Exception ex)
                {
                    flag2 = false;
                }
                return flag2;
            };
            DirectoryInfo info = DirectoryUtils.GetInfo(PageInstanceLeft.Instance.PathIndie);
            CS$<>8__locals1.$VB$Local_AllEntries.AddRange(info.EnumerateFiles().Select<FileInfo, string>((PageInstanceExport._Closure$__.$I8-3 == null) ? (PageInstanceExport._Closure$__.$I8-3 = (FileInfo f) => f.Name) : PageInstanceExport._Closure$__.$I8-3));
            try
            {
                IEnumerable<DirectoryInfo> enumerable = info.EnumerateDirectories();
                VB$AnonymousDelegate_14<DirectoryInfo, bool> vb$AnonymousDelegate_ = CS$<>8__locals1.$VB$Local_IsValidDirectory;
                IEnumerator<DirectoryInfo> enumerator = enumerable.Where<DirectoryInfo>((vb$AnonymousDelegate_ == null) ? null : new Func<DirectoryInfo, bool>(vb$AnonymousDelegate_.Invoke)).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    PageInstanceExport._Closure$__8-1 CS$<>8__locals2 = new PageInstanceExport._Closure$__8-1(CS$<>8__locals2);
                    CS$<>8__locals2.$VB$Local_SubFolder = enumerator.Current;
                    CS$<>8__locals1.$VB$Local_AllEntries.Add(string.Format("{0}\\", CS$<>8__locals2.$VB$Local_SubFolder.Name));
                    CS$<>8__locals1.$VB$Local_AllEntries.AddRange(from f in CS$<>8__locals2.$VB$Local_SubFolder.EnumerateFiles()
                        select string.Format("{0}\\{1}", CS$<>8__locals2.$VB$Local_SubFolder.Name, f.Name));
                    List<string> $VB$Local_AllEntries = CS$<>8__locals1.$VB$Local_AllEntries;
                    IEnumerable<DirectoryInfo> enumerable2 = CS$<>8__locals2.$VB$Local_SubFolder.EnumerateDirectories();
                    vb$AnonymousDelegate_ = CS$<>8__locals1.$VB$Local_IsValidDirectory;
                    $VB$Local_AllEntries.AddRange(from d in enumerable2.Where<DirectoryInfo>((vb$AnonymousDelegate_ == null) ? null : new Func<DirectoryInfo, bool>(vb$AnonymousDelegate_.Invoke))
                        select string.Format("{0}\\{1}\\", CS$<>8__locals2.$VB$Local_SubFolder.Name, d.Name));
                }
            }
            finally
            {
                IEnumerator<DirectoryInfo> enumerator;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            Logger.Info(string.Format("共发现 {0} 个可行的二级文件/文件夹", CS$<>8__locals1.$VB$Local_AllEntries.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
            VB$AnonymousDelegate_15<ExportOption, bool> vb$AnonymousDelegate_2 = (ExportOption TargetOption) => (!TargetOption.RequireOptiFine || PageInstanceLeft.Instance.Version.HasOptiFine) && (!TargetOption.RequireModLoader || PageInstanceLeft.Instance.Modable) && (!TargetOption.RequireModLoaderOrOptiFine || PageInstanceLeft.Instance.Version.HasOptiFine || PageInstanceLeft.Instance.Modable) && CS$<>8__locals1.$VB$Me.StandardizeLines((TargetOption.Rules ?? TargetOption.ShowRules).Split(new char[] { '|' }), true).Any<string>((CS$<>8__locals1.$I7 == null) ? (CS$<>8__locals1.$I7 = delegate(string Rule)
            {
                PageInstanceExport._Closure$__8-3 CS$<>8__locals4 = new PageInstanceExport._Closure$__8-3(CS$<>8__locals4);
                CS$<>8__locals4.$VB$Local_Rule = Rule;
                bool flag3;
                if (CS$<>8__locals4.$VB$Local_Rule.StartsWithF("!", false))
                {
                    flag3 = false;
                }
                else
                {
                    try
                    {
                        if (CS$<>8__locals1.$VB$Local_AllEntries.Any<string>((string Entry) => LikeOperator.LikeString(Entry.Lower(), CS$<>8__locals4.$VB$Local_Rule.Lower(), CompareMethod.Binary)))
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, string.Format("错误的规则：{0}", CS$<>8__locals4.$VB$Local_Rule), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                        return false;
                    }
                    CS$<>8__locals4.$VB$Local_Rule = CS$<>8__locals4.$VB$Local_Rule.Trim("*?".ToCharArray());
                    if (CS$<>8__locals4.$VB$Local_Rule.Split('\\', true).Count<string>() >= 3)
                    {
                        if (CS$<>8__locals4.$VB$Local_Rule.EndsWithF("\\", false))
                        {
                            flag3 = CS$<>8__locals1.$VB$Local_IsValidDirectory(DirectoryUtils.GetInfo(PageInstanceLeft.Instance.PathIndie + CS$<>8__locals4.$VB$Local_Rule));
                        }
                        else
                        {
                            flag3 = FileUtils.Exists(PageInstanceLeft.Instance.PathIndie + CS$<>8__locals4.$VB$Local_Rule);
                        }
                    }
                    else
                    {
                        flag3 = false;
                    }
                }
                return flag3;
            }) : CS$<>8__locals1.$I7);
            try
            {
                foreach (MyCheckBox myCheckBox in this.GetAllOptions(true))
                {
                    ExportOption exportOption = this.GetExportOption(myCheckBox);
                    if (string.IsNullOrEmpty(exportOption.Rules) && string.IsNullOrEmpty(exportOption.ShowRules))
                    {
                        myCheckBox.Visibility = Visibility.Visible;
                        myCheckBox.Checked = exportOption.DefaultChecked;
                    }
                    else
                    {
                        bool flag = vb$AnonymousDelegate_2(exportOption);
                        myCheckBox.Visibility = (flag ? Visibility.Visible : Visibility.Collapsed);
                        myCheckBox.Checked = exportOption.DefaultChecked && flag;
                    }
                    myCheckBox.Inlines.Clear();
                    if (myCheckBox.Visibility == Visibility.Visible)
                    {
                        myCheckBox.Inlines.Add(new Run(exportOption.Title));
                        if (!string.IsNullOrEmpty(exportOption.Description))
                        {
                            myCheckBox.Inlines.Add(new Run("   " + exportOption.Description)
                            {
                                Foreground = ModSecret.ColorGray5
                            });
                        }
                    }
                }
            }
            finally
            {
                IEnumerator<MyCheckBox> enumerator2;
                if (enumerator2 != null)
                {
                    enumerator2.Dispose();
                }
            }
        }

        private IEnumerable<string> StandardizeLines(IEnumerable<string> Raw, bool AddSuffixStarToFolderPath)
        {
            try
            {
                foreach (string text in Raw)
                {
                    text = text.Trim();
                    if (Operators.CompareString(text, "", false) != 0 && !text.StartsWithF("#", false) && !text.StartsWithF("=", false))
                    {
                        text = text.Replace("/", "\\");
                        yield return text + ((text.EndsWithF("\\", false) && AddSuffixStarToFolderPath) ? "*" : "");
                    }
                }
                yield break;
            }
            finally
            {
                IEnumerator<string> enumerator;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            yield break;
        }

        private IEnumerable<MyCheckBox> GetAllOptions(bool IncludeHidden)
        {
            try
            {
                foreach (object obj in this.PanOptions.Children)
                {
                    object objectValue = RuntimeHelpers.GetObjectValue(obj);
                    if (IncludeHidden || !Operators.ConditionalCompareObjectNotEqual(NewLateBinding.LateGet(objectValue, null, "Visibility", new object[0], null, null, null), Visibility.Visible, false))
                    {
                        if (objectValue is MyCheckBox)
                        {
                            yield return (MyCheckBox)objectValue;
                        }
                        else if (objectValue is StackPanel)
                        {
                            try
                            {
                                foreach (object obj2 in ((StackPanel)objectValue).Children)
                                {
                                    object objectValue2 = RuntimeHelpers.GetObjectValue(obj2);
                                    if ((IncludeHidden || !Operators.ConditionalCompareObjectNotEqual(NewLateBinding.LateGet(objectValue2, null, "Visibility", new object[0], null, null, null), Visibility.Visible, false)) && objectValue2 is MyCheckBox)
                                    {
                                        yield return (MyCheckBox)objectValue2;
                                    }
                                }
                            }
                            finally
                            {
                                IEnumerator enumerator2;
                                if (enumerator2 is IDisposable)
                                {
                                    (enumerator2 as IDisposable).Dispose();
                                }
                            }
                        }
                    }
                }
                yield break;
            }
            finally
            {
                IEnumerator enumerator;
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            yield break;
        }

        private ExportOption GetExportOption(MyCheckBox CheckBox)
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
            if (this.RulesOverrides != null)
            {
                try
                {
                    foreach (string text in this.RulesOverrides)
                    {
                        yield return text;
                    }
                    yield break;
                }
                finally
                {
                    List<string>.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
            }
            yield return "";
            yield return "# 修改下方的规则以控制需要导出的内容。";
            yield return "# 以 ! 开头以反选。可以使用 *、?、[] 通配符。靠后的行覆盖靠前的。";
            yield return "";
            try
            {
                foreach (MyCheckBox myCheckBox in this.GetAllOptions(false))
                {
                    if (myCheckBox.Checked)
                    {
                        ExportOption exportOption = this.GetExportOption(myCheckBox);
                        string rules = exportOption.Rules;
                        List<string> Rules = ((rules != null) ? rules.Split(new char[] { '|' }).ToList<string>() : null) ?? new List<string>();
                        if (myCheckBox == this.CheckOptionsShaderPackSettings)
                        {
                            try
                            {
                                foreach (MyCheckBox myCheckBox2 in this.PanOptionsShaderPacks.Children.OfType<MyCheckBox>())
                                {
                                    if (myCheckBox2.Checked)
                                    {
                                        ExportOption exportOption2 = this.GetExportOption(myCheckBox2);
                                        string text2 = string.Format("shaderpacks/{0}.txt", exportOption2.Title);
                                        if (FileUtils.Exists(Path.Combine(PageInstanceLeft.Instance.PathIndie, text2)))
                                        {
                                            Rules.Add(StringUtils.LikePatternEscape(text2));
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                IEnumerator<MyCheckBox> enumerator3;
                                if (enumerator3 != null)
                                {
                                    enumerator3.Dispose();
                                }
                            }
                        }
                        if (Rules.Any<string>())
                        {
                            yield return string.Format("# {0}", exportOption.Title);
                            try
                            {
                                foreach (string text3 in Rules)
                                {
                                    yield return text3;
                                }
                            }
                            finally
                            {
                                List<string>.Enumerator enumerator4;
                                ((IDisposable)enumerator4).Dispose();
                            }
                            yield return "";
                        }
                    }
                }
            }
            finally
            {
                IEnumerator<MyCheckBox> enumerator2;
                if (enumerator2 != null)
                {
                    enumerator2.Dispose();
                }
            }
            yield return "# 排除的文件";
            yield return "!*.log";
            yield return "!*.dat_old";
            yield return "!*.BakaCoreInfo";
            yield return "!hmclversion.cfg";
            yield return "!log4j2.xml";
            yield return "";
            yield break;
        }

        private IEnumerable<string> GetExtraFileLines()
        {
            if (this.ExtraFiles != null)
            {
                try
                {
                    foreach (string text in this.ExtraFiles)
                    {
                        yield return text;
                    }
                    yield break;
                }
                finally
                {
                    List<string>.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
            }
            yield return "";
            yield return "# 如果想将额外的文件自动放到压缩包根目录中，可以将它们的路径写在下方。";
            yield return "# 必须是完整路径。每行中，若以 \\ 结尾则代表是文件夹，不以 \\ 结尾则代表是文件。";
            yield return "";
            yield break;
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
                    foreach (string text2 in array[0].SplitLines(true))
                    {
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
            PageInstanceExport._Closure$__25-0 CS$<>8__locals1 = new PageInstanceExport._Closure$__25-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_PackName = (string.IsNullOrEmpty(this.TextExportName.Text) ? this.TextExportName.HintText : this.TextExportName.Text);
            CS$<>8__locals1.$VB$Local_PackVersion = (string.IsNullOrEmpty(this.TextExportVersion.Text) ? "1.0.0" : this.TextExportVersion.Text);
            CS$<>8__locals1.$VB$Local_LoaderName = "导出整合包：" + CS$<>8__locals1.$VB$Local_PackName;
            try
            {
                IEnumerator<ModLoader.LoaderBase> enumerator = ModLoader.LoaderTaskbar.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (Operators.CompareString(enumerator.Current.Name, CS$<>8__locals1.$VB$Local_LoaderName, false) == 0)
                    {
                        ModMain.FrmMain.PageChange(FormMain.PageType.TaskManager, FormMain.PageSubType.Default);
                        return;
                    }
                }
            }
            finally
            {
                IEnumerator<ModLoader.LoaderBase> enumerator;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            CS$<>8__locals1.$VB$Local_PackPath = null;
            if (!string.IsNullOrWhiteSpace(this.ConfigPackPath) && !this.ConfigPackPath.EndsWithF("\\", false) && !this.ConfigPackPath.EndsWithF("/", false))
            {
                try
                {
                    DirectoryUtils.Create(PathUtils.RemoveLastPart(this.ConfigPackPath));
                    CS$<>8__locals1.$VB$Local_PackPath = this.ConfigPackPath;
                    Logger.Info(string.Format("使用配置文件中指定的导出路径：{0}", this.ConfigPackPath), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, string.Format("无法使用配置文件中指定的导出路径（{0}）", this.ConfigPackPath), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                    if (ModMain.MyMsgBox(string.Format("指定的路径：{0}{1}{2}{3}", new object[]
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
            if (CS$<>8__locals1.$VB$Local_PackPath == null)
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
                CS$<>8__locals1.$VB$Local_PackPath = Dialogs.SaveFile("选择导出位置", CS$<>8__locals1.$VB$Local_PackName + (string.IsNullOrEmpty(this.TextExportVersion.Text) ? "" : (" " + this.TextExportVersion.Text)), null, list);
                Logger.Info(string.Format("手动指定的导出路径：{0}", CS$<>8__locals1.$VB$Local_PackPath), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
            }
            if (CS$<>8__locals1.$VB$Local_PackPath != null)
            {
                CS$<>8__locals1.$VB$Local_CacheFolder = ModMain.RequestTaskTempFolder(false);
                CS$<>8__locals1.$VB$Local_OverridesFolder = CS$<>8__locals1.$VB$Local_CacheFolder + "modpack\\overrides\\";
                CS$<>8__locals1.$VB$Local_TargetInstance = PageInstanceLeft.Instance;
                CS$<>8__locals1.$VB$Local_IncludeJava = (this.RulesOverrides == null && this.CheckOptionsJava.Visibility == Visibility.Visible && this.CheckOptionsJava.Checked) || (this.RulesOverrides != null && this.CheckOptionsJava.Checked && Settings.Get<int>("VersionArgumentJavaV2", CS$<>8__locals1.$VB$Local_TargetInstance) == 2);
                CS$<>8__locals1.$VB$Local_PathIndie = CS$<>8__locals1.$VB$Local_TargetInstance.PathIndie;
                CS$<>8__locals1.$VB$Local_CheckHostedAssets = !this.CheckAdvancedInclude.Checked;
                CS$<>8__locals1.$VB$Local_ModrinthUploadMode = this.CheckAdvancedModrinth.Checked;
                CS$<>8__locals1.$VB$Local_IncludePCL = this.CheckOptionsPcl.Checked;
                CS$<>8__locals1.$VB$Local_IncludePCLCustom = CS$<>8__locals1.$VB$Local_IncludePCL && this.CheckOptionsPclCustom.Checked;
                CS$<>8__locals1.$VB$Local_AllRules = this.StandardizeLines(this.GetAllRules(), true).ToList<string>();
                CS$<>8__locals1.$VB$Local_AllExtraFiles = this.StandardizeLines(this.GetExtraFileLines(), false).ToList<string>();
                Logger.Info(string.Format("准备导出整合包，共有 {0} 条规则，{1} 条追加内容行", CS$<>8__locals1.$VB$Local_AllRules.Count, CS$<>8__locals1.$VB$Local_AllExtraFiles.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                List<ModLoader.LoaderBase> list2 = new List<ModLoader.LoaderBase>();
                if (CS$<>8__locals1.$VB$Local_IncludePCL)
                {
                    list2.Add(new ModLoader.LoaderTask<int, int>("下载 PCL 正式版", delegate(ModLoader.LoaderTask<int, int> Loader)
                    {
                        ModSecret.DownloadLatestPCL(Loader);
                        FileUtils.Copy(ModBase.PathTemp + "Latest.exe", CS$<>8__locals1.$VB$Local_CacheFolder + "Plain Craft Launcher.exe");
                    }, null, ThreadPriority.Normal)
                    {
                        ProgressWeight = 0.5,
                        Block = false
                    });
                }
                list2.Add(new ModLoader.LoaderTask<int, List<LocalResourceFile>>("复制导出内容", delegate(ModLoader.LoaderTask<int, List<LocalResourceFile>> Loader)
                {
                    PageInstanceExport._Closure$__25-1 CS$<>8__locals2 = new PageInstanceExport._Closure$__25-1(CS$<>8__locals2);
                    CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1;
                    CS$<>8__locals2.$VB$Local_Loader = Loader;
                    CS$<>8__locals2.$VB$Local_Loader.Output = new List<LocalResourceFile>();
                    CS$<>8__locals2.$VB$Local_Progress = 0;
                    CS$<>8__locals2.$VB$Local_SearchFolder = delegate(DirectoryInfo Folder)
                    {
                        try
                        {
                            foreach (DirectoryInfo directoryInfo in Folder.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
                            {
                                if ((!new string[] { "assets", "versions", "libraries" }.Contains(directoryInfo.Name) || !PathUtils.IsSame(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_PathIndie, Folder.FullName)) && !new string[] { "structureCacheV1", ".fabric", ".git", "avatar-cache", "cosmetic-cache" }.Contains(directoryInfo.Name))
                                {
                                    CS$<>8__locals2.$VB$Local_SearchFolder(directoryInfo);
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator<DirectoryInfo> enumerator3;
                            if (enumerator3 != null)
                            {
                                enumerator3.Dispose();
                            }
                        }
                        try
                        {
                            foreach (FileInfo fileInfo in Folder.EnumerateFiles("*", SearchOption.TopDirectoryOnly))
                            {
                                PageInstanceExport._Closure$__25-2 CS$<>8__locals3 = new PageInstanceExport._Closure$__25-2(CS$<>8__locals3);
                                CS$<>8__locals3.$VB$Local_RelativePath = fileInfo.FullName.AfterFirst(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_PathIndie, false);
                                bool flag = false;
                                try
                                {
                                    foreach (string text5 in CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_AllRules)
                                    {
                                        bool flag2 = text5.StartsWithF("!", false);
                                        if (LikeOperator.LikeString(CS$<>8__locals3.$VB$Local_RelativePath.Lower(), text5.TrimStart(new char[] { '!' }).Lower(), CompareMethod.Binary))
                                        {
                                            flag = !flag2;
                                        }
                                    }
                                }
                                finally
                                {
                                    List<string>.Enumerator enumerator5;
                                    ((IDisposable)enumerator5).Dispose();
                                }
                                if (flag)
                                {
                                    string text6 = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_OverridesFolder + CS$<>8__locals3.$VB$Local_RelativePath;
                                    FileUtils.Copy(fileInfo.FullName, text6);
                                    if (CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_CheckHostedAssets && new string[] { ".zip", ".rar", ".jar", ".disabled", ".old" }.Contains(fileInfo.Extension.Lower()) && new string[] { "mods", "packs", "openloader", "resource" }.Any<string>((string s) => CS$<>8__locals3.$VB$Local_RelativePath.Contains(s)))
                                    {
                                        LocalResourceFile localResourceFile = new LocalResourceFile(text6);
                                        string modrinthHash = localResourceFile.ModrinthHash;
                                        Conversions.ToString(localResourceFile.CurseForgeHash);
                                        CS$<>8__locals2.$VB$Local_Loader.Output.Add(localResourceFile);
                                    }
                                    checked
                                    {
                                        CS$<>8__locals2.$VB$Local_Progress++;
                                    }
                                    if (CS$<>8__locals2.$VB$Local_Progress == 25)
                                    {
                                        ModLoader.LoaderTask<int, List<LocalResourceFile>> $VB$Local_Loader;
                                        ($VB$Local_Loader = CS$<>8__locals2.$VB$Local_Loader).Progress = $VB$Local_Loader.Progress + (0.84 - CS$<>8__locals2.$VB$Local_Loader.Progress) * 0.012;
                                        CS$<>8__locals2.$VB$Local_Progress = 0;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator<FileInfo> enumerator4;
                            if (enumerator4 != null)
                            {
                                enumerator4.Dispose();
                            }
                        }
                    };
                    CS$<>8__locals2.$VB$Local_SearchFolder(DirectoryUtils.GetInfo(CS$<>8__locals1.$VB$Local_PathIndie));
                    Logger.Info(string.Format("复制 overrides 文件完成，有 {0} 个文件需要联网检查", CS$<>8__locals2.$VB$Local_Loader.Output.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                    CS$<>8__locals2.$VB$Local_Loader.Progress = 0.85;
                    if (CS$<>8__locals1.$VB$Local_IncludeJava)
                    {
                        Java java = ModJava.SelectOrDownloadJava(CS$<>8__locals1.$VB$Local_TargetInstance, false, CS$<>8__locals2.$VB$Local_Loader.CreateCancellationToken(), CS$<>8__locals2.$VB$Local_Loader.CreateSyncProgressProvider(0.85, 0.93));
                        if (java == null)
                        {
                            throw new FileNotFoundException("版本文件夹中没有找到可导出的 Java。");
                        }
                        string text = PathUtils.RemoveLastPart(java.Folder);
                        string text2 = Path.Combine(CS$<>8__locals1.$VB$Local_OverridesFolder, text.AfterFirst(CS$<>8__locals1.$VB$Local_TargetInstance.PathVersion, true));
                        DirectoryUtils.Copy(text, text2);
                        Logger.Info(string.Format("已复制版本文件夹中的 Java：{0} → {1}", text, text2), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                    }
                    CS$<>8__locals2.$VB$Local_Loader.Progress = 0.95;
                    string text3 = (CS$<>8__locals1.$VB$Local_IncludePCL ? CS$<>8__locals1.$VB$Local_CacheFolder : (CS$<>8__locals1.$VB$Local_CacheFolder + "modpack\\"));
                    try
                    {
                        foreach (string text4 in CS$<>8__locals1.$VB$Local_AllExtraFiles)
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
                        List<string>.Enumerator enumerator2;
                        ((IDisposable)enumerator2).Dispose();
                    }
                    CS$<>8__locals2.$VB$Local_Loader.Progress = 0.97;
                    if (DirectoryUtils.Exists(CS$<>8__locals1.$VB$Local_TargetInstance.PathVersion + "PCL\\"))
                    {
                        DirectoryUtils.Copy(CS$<>8__locals1.$VB$Local_TargetInstance.PathVersion + "PCL\\", CS$<>8__locals1.$VB$Local_OverridesFolder + "PCL\\");
                    }
                    ModBase.WriteIni(CS$<>8__locals1.$VB$Local_OverridesFolder + "PCL\\Setup.ini", "IsStar", Conversions.ToString(false));
                    if (CS$<>8__locals1.$VB$Local_IncludePCLCustom)
                    {
                        if (DirectoryUtils.Exists(Paths.Base + "PCL\\Pictures\\"))
                        {
                            DirectoryUtils.Copy(Paths.Base + "PCL\\Pictures\\", CS$<>8__locals1.$VB$Local_CacheFolder + "PCL\\Pictures\\");
                        }
                        if (DirectoryUtils.Exists(Paths.Base + "PCL\\Musics\\"))
                        {
                            DirectoryUtils.Copy(Paths.Base + "PCL\\Musics\\", CS$<>8__locals1.$VB$Local_CacheFolder + "PCL\\Musics\\");
                        }
                        if (DirectoryUtils.Exists(Paths.Base + "PCL\\Help\\"))
                        {
                            DirectoryUtils.Copy(Paths.Base + "PCL\\Help\\", CS$<>8__locals1.$VB$Local_CacheFolder + "PCL\\Help\\");
                        }
                        if (FileUtils.Exists(Paths.Base + "PCL\\Custom.xaml"))
                        {
                            FileUtils.Copy(Paths.Base + "PCL\\Custom.xaml", CS$<>8__locals1.$VB$Local_CacheFolder + "PCL\\Custom.xaml");
                        }
                        if (FileUtils.Exists(Paths.Base + "PCL\\Setup.ini"))
                        {
                            FileUtils.Copy(Paths.Base + "PCL\\Setup.ini", CS$<>8__locals1.$VB$Local_CacheFolder + "PCL\\Setup.ini");
                        }
                        if (FileUtils.Exists(Paths.Base + "PCL\\hints.txt"))
                        {
                            FileUtils.Copy(Paths.Base + "PCL\\hints.txt", CS$<>8__locals1.$VB$Local_CacheFolder + "PCL\\hints.txt");
                        }
                        if (FileUtils.Exists(Paths.Base + "PCL\\Logo.png"))
                        {
                            FileUtils.Copy(Paths.Base + "PCL\\Logo.png", CS$<>8__locals1.$VB$Local_CacheFolder + "PCL\\Logo.png");
                        }
                    }
                }, null, ThreadPriority.Normal)
                {
                    ProgressWeight = 5.0
                });
                list2.Add(new ModLoader.LoaderTask<List<LocalResourceFile>, Dictionary<LocalResourceFile, List<string>>>("联网获取文件信息", delegate(ModLoader.LoaderTask<List<LocalResourceFile>, Dictionary<LocalResourceFile, List<string>>> Loader)
                {
                    PageInstanceExport._Closure$__25-3 CS$<>8__locals4 = new PageInstanceExport._Closure$__25-3(CS$<>8__locals4);
                    CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3 = CS$<>8__locals1;
                    CS$<>8__locals4.$VB$Local_Loader = Loader;
                    CS$<>8__locals4.$VB$Local_Loader.Output = new Dictionary<LocalResourceFile, List<string>>();
                    if (!CS$<>8__locals1.$VB$Local_CheckHostedAssets)
                    {
                        Logger.Info("要求跳过联网获取步骤", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                        return;
                    }
                    if (!CS$<>8__locals4.$VB$Local_Loader.Input.Any<LocalResourceFile>())
                    {
                        Logger.Info("没有需要联网检查的文件，跳过联网获取步骤", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                        return;
                    }
                    CS$<>8__locals4.$VB$Local_EndedThreadCount = 0;
                    CS$<>8__locals4.$VB$Local_FailedExceptions = new List<Exception>();
                    ModBase.RunInNewThread(delegate
                    {
                        try
                        {
                            IEnumerable<string> enumerable = CS$<>8__locals4.$VB$Local_Loader.Input.Select<LocalResourceFile, string>((PageInstanceExport._Closure$__.$I25-6 == null) ? (PageInstanceExport._Closure$__.$I25-6 = (LocalResourceFile m) => m.ModrinthHash) : PageInstanceExport._Closure$__.$I25-6);
                            JObject jobject = (JObject)ModDownload.DlModRequest("https://api.modrinth.com/v2/version_files", HttpMethod.Post, string.Format("{{\"hashes\": [\"{0}\"], \"algorithm\": \"sha1\"}}", enumerable.Join("\",\"")), "application/json");
                            try
                            {
                                foreach (LocalResourceFile localResourceFile2 in CS$<>8__locals4.$VB$Local_Loader.Input)
                                {
                                    if (jobject.ContainsKey(localResourceFile2.ModrinthHash))
                                    {
                                        JToken jtoken = jobject[localResourceFile2.ModrinthHash];
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
                                        if (!((string)jtoken2 != localResourceFile2.ModrinthHash))
                                        {
                                            CS$<>8__locals4.$VB$Local_Loader.Output.AddIntoValueCollection(localResourceFile2, (string)jobject[localResourceFile2.ModrinthHash]["files"][0]["url"]);
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                List<LocalResourceFile>.Enumerator enumerator6;
                                ((IDisposable)enumerator6).Dispose();
                            }
                            Logger.Info(string.Format("从 Modrinth 获取到 {0} 个本地资源项的对应信息", jobject.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                        }
                        catch (Exception ex2)
                        {
                            Logger.Warn(ex2, "从 Modrinth 获取本地 Mod 信息失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                            CS$<>8__locals4.$VB$Local_FailedExceptions.Add(ex2);
                        }
                        finally
                        {
                            checked
                            {
                                CS$<>8__locals4.$VB$Local_EndedThreadCount++;
                            }
                            ModLoader.LoaderTask<List<LocalResourceFile>, Dictionary<LocalResourceFile, List<string>>> $VB$Local_Loader2;
                            ($VB$Local_Loader2 = CS$<>8__locals4.$VB$Local_Loader).Progress = $VB$Local_Loader2.Progress + 0.45;
                        }
                    }, "Modrinth - " + CS$<>8__locals1.$VB$Local_LoaderName, ThreadPriority.Normal);
                    ModBase.RunInNewThread(delegate
                    {
                        try
                        {
                            if (!CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_ModrinthUploadMode)
                            {
                                IEnumerable<uint> enumerable2 = CS$<>8__locals4.$VB$Local_Loader.Input.Select<LocalResourceFile, uint>((PageInstanceExport._Closure$__.$I25-8 == null) ? (PageInstanceExport._Closure$__.$I25-8 = (LocalResourceFile m) => m.CurseForgeHash) : PageInstanceExport._Closure$__.$I25-8);
                                JContainer jcontainer = (JContainer)ModDownload.DlModRequest("https://api.curseforge.com/v1/fingerprints/432", HttpMethod.Post, string.Format("{{\"fingerprints\": [{0}]}}", enumerable2.Join(",")), "application/json")["data"]["exactMatches"];
                                try
                                {
                                    foreach (JToken jtoken5 in jcontainer)
                                    {
                                        JObject jobject2 = (JObject)jtoken5;
                                        PageInstanceExport._Closure$__25-4 CS$<>8__locals5 = new PageInstanceExport._Closure$__25-4(CS$<>8__locals5);
                                        if (jobject2.ContainsKey("file"))
                                        {
                                            CS$<>8__locals5.$VB$Local_File = (JObject)jobject2["file"];
                                            if (!string.IsNullOrEmpty((string)CS$<>8__locals5.$VB$Local_File["downloadUrl"]))
                                            {
                                                LocalResourceFile localResourceFile3 = CS$<>8__locals4.$VB$Local_Loader.Input.FirstOrDefault<LocalResourceFile>((LocalResourceFile m) => m.CurseForgeHash == CS$<>8__locals5.$VB$Local_File["fileFingerprint"].ToObject<uint>());
                                                if (localResourceFile3 != null)
                                                {
                                                    try
                                                    {
                                                        foreach (string text7 in ResourceVersion.ParseCurseForgeDownloadUrls(CS$<>8__locals5.$VB$Local_File["downloadUrl"].ToString()))
                                                        {
                                                            CS$<>8__locals4.$VB$Local_Loader.Output.AddIntoValueCollection(localResourceFile3, text7);
                                                        }
                                                    }
                                                    finally
                                                    {
                                                        List<string>.Enumerator enumerator8;
                                                        ((IDisposable)enumerator8).Dispose();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                finally
                                {
                                    IEnumerator<JToken> enumerator7;
                                    if (enumerator7 != null)
                                    {
                                        enumerator7.Dispose();
                                    }
                                }
                                Logger.Info(string.Format("从 CurseForge 获取到 {0} 个本地资源项的对应信息", jcontainer.Count), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                            }
                        }
                        catch (Exception ex3)
                        {
                            Logger.Warn(ex3, "从 CurseForge 获取本地 Mod 信息失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceExport.xaml.vb");
                            CS$<>8__locals4.$VB$Local_FailedExceptions.Add(ex3);
                        }
                        finally
                        {
                            checked
                            {
                                CS$<>8__locals4.$VB$Local_EndedThreadCount++;
                            }
                            ModLoader.LoaderTask<List<LocalResourceFile>, Dictionary<LocalResourceFile, List<string>>> $VB$Local_Loader3;
                            ($VB$Local_Loader3 = CS$<>8__locals4.$VB$Local_Loader).Progress = $VB$Local_Loader3.Progress + 0.45;
                        }
                    }, "CurseForge - " + CS$<>8__locals1.$VB$Local_LoaderName, ThreadPriority.Normal);
                    while (CS$<>8__locals4.$VB$Local_EndedThreadCount != 2)
                    {
                        if (CS$<>8__locals4.$VB$Local_Loader.IsCanceled)
                        {
                            return;
                        }
                        Thread.Sleep(10);
                    }
                    if (CS$<>8__locals4.$VB$Local_FailedExceptions.Count == 1)
                    {
                        if (ModMain.MyMsgBox("联网获取部分文件信息失败，是否继续导出？\r\n\r\n若继续，无法获取信息的文件将被直接打包。\r\n由于二次分发可能违反使用协议，请尽量不要公开发布导出的整合包！", "部分文件信息获取失败", "继续", "取消", "", false, true, false, null, null, null) == 2)
                        {
                            throw CS$<>8__locals4.$VB$Local_FailedExceptions.First<Exception>();
                        }
                        return;
                    }
                    else
                    {
                        if (CS$<>8__locals4.$VB$Local_FailedExceptions.Count > 1 && ModMain.MyMsgBox("联网获取文件信息失败，是否继续导出？\r\n\r\n若继续，所有文件都将被直接打包。\r\n由于二次分发可能违反使用协议，请尽量不要公开发布导出的整合包！", "文件信息获取失败", "继续", "取消", "", false, true, false, null, null, null) == 2)
                        {
                            throw CS$<>8__locals4.$VB$Local_FailedExceptions.First<Exception>();
                        }
                        return;
                    }
                }, null, ThreadPriority.Normal)
                {
                    Show = CS$<>8__locals1.$VB$Local_CheckHostedAssets,
                    ProgressWeight = (CS$<>8__locals1.$VB$Local_CheckHostedAssets ? 2.0 : 0.01)
                });
                list2.Add(new ModLoader.LoaderTask<Dictionary<LocalResourceFile, List<string>>, int>("生成压缩包", delegate(ModLoader.LoaderTask<Dictionary<LocalResourceFile, List<string>>, int> Loader)
                {
                    JArray jarray = new JArray();
                    try
                    {
                        foreach (KeyValuePair<LocalResourceFile, List<string>> keyValuePair in Loader.Input)
                        {
                            LocalResourceFile key = keyValuePair.Key;
                            JArray jarray2 = jarray;
                            JObject jobject3 = new JObject();
                            jobject3.Add("path", PathUtils.RemoveExtendedPrefix(key.File.FullName).AfterFirst(CS$<>8__locals1.$VB$Local_OverridesFolder, false).Replace("\\", "/"));
                            string text8 = "hashes";
                            JObject jobject4 = new JObject();
                            jobject4.Add("sha1", key.ModrinthHash);
                            jobject4.Add("sha512", CryptographyUtils.ComputeFileHash(key.File.FullName, CryptographyUtils.HashMethod.Sha512));
                            jobject3.Add(text8, jobject4);
                            jobject3.Add("downloads", new JArray(keyValuePair.Value.OrderBy<string, bool>((PageInstanceExport._Closure$__.$I25-11 == null) ? (PageInstanceExport._Closure$__.$I25-11 = (string u) => u.Contains("modrinth.com")) : PageInstanceExport._Closure$__.$I25-11)));
                            jobject3.Add("fileSize", key.File.Length);
                            jarray2.Add(jobject3);
                            key.File.Delete();
                        }
                    }
                    finally
                    {
                        Dictionary<LocalResourceFile, List<string>>.Enumerator enumerator9;
                        ((IDisposable)enumerator9).Dispose();
                    }
                    Loader.Progress = 0.2;
                    JObject jobject5 = new JObject();
                    jobject5.Add("minecraft", CS$<>8__locals1.$VB$Local_TargetInstance.Version.VanillaName);
                    JObject jobject6 = jobject5;
                    if (CS$<>8__locals1.$VB$Local_TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.Forge))
                    {
                        jobject6.Add("forge", CS$<>8__locals1.$VB$Local_TargetInstance.Version.ModLoaderVersions[ModLoaders.Forge]);
                    }
                    if (CS$<>8__locals1.$VB$Local_TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.Fabric))
                    {
                        jobject6.Add("fabric-loader", CS$<>8__locals1.$VB$Local_TargetInstance.Version.ModLoaderVersions[ModLoaders.Fabric]);
                    }
                    if (CS$<>8__locals1.$VB$Local_TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge))
                    {
                        jobject6.Add("neoforge", CS$<>8__locals1.$VB$Local_TargetInstance.Version.ModLoaderVersions[ModLoaders.NeoForge]);
                    }
                    JObject jobject7 = new JObject();
                    jobject7.Add("game", "minecraft");
                    jobject7.Add("formatVersion", 1);
                    jobject7.Add("versionId", CS$<>8__locals1.$VB$Local_PackVersion);
                    jobject7.Add("name", CS$<>8__locals1.$VB$Local_PackName);
                    jobject7.Add("summary", CS$<>8__locals1.$VB$Local_TargetInstance.Info);
                    jobject7.Add("files", jarray);
                    jobject7.Add("dependencies", jobject6);
                    JObject jobject8 = jobject7;
                    FileUtils.Write(CS$<>8__locals1.$VB$Local_CacheFolder + "modpack\\modrinth.index.json", jobject8.ToString(1, new JsonConverter[0]), null);
                    if (CS$<>8__locals1.$VB$Local_IncludePCL)
                    {
                        FileUtils.CreateZipFromDirectory(CS$<>8__locals1.$VB$Local_CacheFolder + "modpack.mrpack", CS$<>8__locals1.$VB$Local_CacheFolder + "modpack\\");
                        Loader.Progress = 0.5;
                        DirectoryUtils.Delete(CS$<>8__locals1.$VB$Local_CacheFolder + "modpack\\", false);
                        Loader.Progress = 0.6;
                        FileUtils.CreateZipFromDirectory(CS$<>8__locals1.$VB$Local_PackPath, CS$<>8__locals1.$VB$Local_CacheFolder);
                        Loader.Progress = 0.9;
                    }
                    else
                    {
                        FileUtils.CreateZipFromDirectory(CS$<>8__locals1.$VB$Local_PackPath, CS$<>8__locals1.$VB$Local_CacheFolder + "modpack\\");
                        Loader.Progress = 0.8;
                    }
                    ModBase.OpenExplorer(CS$<>8__locals1.$VB$Local_PackPath);
                }, null, ThreadPriority.Normal)
                {
                    ProgressWeight = 6.0
                });
                ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>(CS$<>8__locals1.$VB$Local_LoaderName, list2);
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
                this.HintJava.Theme = MyHint.Themes.Yellow;
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
                this.HintJava.Theme = MyHint.Themes.Red;
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

        internal virtual Grid PanAllBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyScrollViewer PanBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyHint HintOptiFine
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyHint HintJava
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyTextBox TextExportName
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
                RoutedEventHandler routedEventHandler = delegate(object a0, RoutedEventArgs a1)
                {
                    this.TextExportName_GotFocus();
                };
                MyTextBox myTextBox = this._TextExportName;
                if (myTextBox != null)
                {
                    myTextBox.GotFocus -= routedEventHandler;
                }
                this._TextExportName = value;
                myTextBox = this._TextExportName;
                if (myTextBox != null)
                {
                    myTextBox.GotFocus += routedEventHandler;
                }
            }
        }

        internal virtual MyTextBox TextExportVersion
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard CardOptions
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
                MouseButtonEventHandler mouseButtonEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.CardOptions_MouseLeftButtonDown();
                };
                MyCard myCard = this._CardOptions;
                if (myCard != null)
                {
                    myCard.MouseLeftButtonDown -= mouseButtonEventHandler;
                }
                this._CardOptions = value;
                myCard = this._CardOptions;
                if (myCard != null)
                {
                    myCard.MouseLeftButtonDown += mouseButtonEventHandler;
                }
            }
        }

        internal virtual MyIconTextButton BtnOverrideCancel
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanOptions
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckOptionsBasic
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckOptionsOptions
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckOptionsMod
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckOptionsResourcePacks
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanOptionsResourcePacks
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckOptionsShaderPacks
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckOptionsShaderPackSettings
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanOptionsShaderPacks
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckOptionsSaves
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanOptionsSaves
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckOptionsJava
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckOptionsPcl
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
                MyCheckBox.ChangeEventHandler changeEventHandler = new MyCheckBox.ChangeEventHandler(this.CheckAdvancedModrinth_Change);
                MyCheckBox myCheckBox = this._CheckOptionsPcl;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckOptionsPcl = value;
                myCheckBox = this._CheckOptionsPcl;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyCheckBox CheckOptionsPclCustom
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckAdvancedInclude
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
                MyCheckBox.ChangeEventHandler changeEventHandler = new MyCheckBox.ChangeEventHandler(this.CheckAdvancedInclude_Change);
                MyCheckBox myCheckBox = this._CheckAdvancedInclude;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckAdvancedInclude = value;
                myCheckBox = this._CheckAdvancedInclude;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyCheckBox CheckAdvancedModrinth
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
                MyCheckBox.ChangeEventHandler changeEventHandler = new MyCheckBox.ChangeEventHandler(this.CheckAdvancedModrinth_Change);
                MyCheckBox myCheckBox = this._CheckAdvancedModrinth;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckAdvancedModrinth = value;
                myCheckBox = this._CheckAdvancedModrinth;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyHint HintModrinthFormat
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnAdvancedImport
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
                MyButton.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.ImportConfig();
                };
                MyButton myButton = this._BtnAdvancedImport;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnAdvancedImport = value;
                myButton = this._BtnAdvancedImport;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnAdvancedExport
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
                MyButton.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.ExportConfig();
                };
                MyButton myButton = this._BtnAdvancedExport;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnAdvancedExport = value;
                myButton = this._BtnAdvancedExport;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnAdvancedHelp
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyExtraTextButton BtnExport
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
                MyExtraTextButton.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.StartExport();
                };
                MyExtraTextButton myExtraTextButton = this._BtnExport;
                if (myExtraTextButton != null)
                {
                    myExtraTextButton.Click -= clickEventHandler;
                }
                this._BtnExport = value;
                myExtraTextButton = this._BtnExport;
                if (myExtraTextButton != null)
                {
                    myExtraTextButton.Click += clickEventHandler;
                }
            }
        }

        private string CurrentInstance;

        private string[] SubOptionBlackList;

        private const string Sperator = "==============================================================";

        private List<string> _RulesOverrides;

        private List<string> ExtraFiles;

        private string ConfigPackPath;
    }
}
