using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;

namespace PCL
{
    public class LocalResourceFile
    {
        public LocalResourceFile(FileInfo FileInfo)
        {
            this._Display = null;
            this._Description = null;
            this._Version = null;
            this.ChangelogUrls = new List<string>();
            this.OnlineDataLoaded = false;
            this.File = FileInfo;
        }

        public LocalResourceFile(string FullName)
            : this(FileUtils.GetInfo(FullName))
        {
        }

        public bool IsEnabled
        {
            get
            {
                return new string[] { ".jar", ".zip", ".litemod" }.Contains(this.File.Extension.Lower());
            }
        }

        public string EnabledName
        {
            get
            {
                return this.File.Name.Replace(".disabled", "").Replace(".old", "");
            }
        }

        public string EnabledFullName
        {
            get
            {
                return string.Format("{0}\\{1}", this.File.DirectoryName, this.EnabledName);
            }
        }

        public event LocalResourceFile.OnUpdateEventHandler OnUpdate
        {
            [CompilerGenerated]
            add
            {
                LocalResourceFile.OnUpdateEventHandler onUpdateEventHandler = this.OnUpdateEvent;
                LocalResourceFile.OnUpdateEventHandler onUpdateEventHandler2;
                do
                {
                    onUpdateEventHandler2 = onUpdateEventHandler;
                    LocalResourceFile.OnUpdateEventHandler onUpdateEventHandler3 = (LocalResourceFile.OnUpdateEventHandler)Delegate.Combine(onUpdateEventHandler2, value);
                    onUpdateEventHandler = Interlocked.CompareExchange<LocalResourceFile.OnUpdateEventHandler>(ref this.OnUpdateEvent, onUpdateEventHandler3, onUpdateEventHandler2);
                }
                while (onUpdateEventHandler != onUpdateEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                LocalResourceFile.OnUpdateEventHandler onUpdateEventHandler = this.OnUpdateEvent;
                LocalResourceFile.OnUpdateEventHandler onUpdateEventHandler2;
                do
                {
                    onUpdateEventHandler2 = onUpdateEventHandler;
                    LocalResourceFile.OnUpdateEventHandler onUpdateEventHandler3 = (LocalResourceFile.OnUpdateEventHandler)Delegate.Remove(onUpdateEventHandler2, value);
                    onUpdateEventHandler = Interlocked.CompareExchange<LocalResourceFile.OnUpdateEventHandler>(ref this.OnUpdateEvent, onUpdateEventHandler3, onUpdateEventHandler2);
                }
                while (onUpdateEventHandler != onUpdateEventHandler2);
            }
        }

        public string Display
        {
            get
            {
                if (this._Display == null)
                {
                    ResourceVersion projectVersion = this.ProjectVersion;
                    this._Display = ((projectVersion != null) ? projectVersion.Display : null);
                }
                if (this._Display == null)
                {
                    this._Display = PathUtils.GetFileNameWithoutExtension(this.EnabledName);
                }
                return this._Display;
            }
            set
            {
                if (this._Display == null && value != null && !value.Contains("modname") && Operators.CompareString(value.Lower(), "name", false) != 0 && value.Count<char>() > 1 && Operators.CompareString(ModBase.Val(value).ToString(), value, false) != 0)
                {
                    this._Display = value;
                }
            }
        }

        public string Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                if (this._Description == null && value != null && value.Count<char>() > 2)
                {
                    this._Description = value.ToString().Trim(new char[] { '\n' });
                    if (this._Description.Lower().LastIndexOfAny(Conversions.ToCharArrayRankOne("qwertyuiopasdfghjklzxcvbnm0123456789")) == checked(this._Description.Count<char>() - 1))
                    {
                        ref string ptr = ref this._Description;
                        this._Description = ptr + ".";
                    }
                }
            }
        }

        public string Version
        {
            get
            {
                if (this._Version == null)
                {
                    ResourceVersion projectVersion = this.ProjectVersion;
                    this._Version = ((projectVersion != null) ? projectVersion.Version : null);
                }
                return this._Version;
            }
            set
            {
                if (this._Version == null || !this._Version.RegexCheck("[0-9.\\-]+", RegexOptions.None))
                {
                    if (value != null && value.ContainsIgnoreCase("version"))
                    {
                        value = "version";
                    }
                    this._Version = value;
                }
            }
        }

        public ResourceProject Project
        {
            get
            {
                return this._Project;
            }
            set
            {
                this._Project = value;
                LocalResourceFile.OnUpdateEventHandler onUpdateEvent = this.OnUpdateEvent;
                if (onUpdateEvent != null)
                {
                    onUpdateEvent(this);
                }
            }
        }

        public ResourceVersion UpdateFile
        {
            get
            {
                return this._UpdateFile;
            }
            set
            {
                this._UpdateFile = value;
                LocalResourceFile.OnUpdateEventHandler onUpdateEvent = this.OnUpdateEvent;
                if (onUpdateEvent != null)
                {
                    onUpdateEvent(this);
                }
            }
        }

        public JObject ToJson()
        {
            JObject jobject = new JObject();
            if (((object)(this.Project) != (object)null))
            {
                jobject.Add("Project", this.Project.ToJson());
            }
            jobject.Add("ChangelogUrls", new JArray(this.ChangelogUrls));
            jobject.Add("OnlineDataLoaded", this.OnlineDataLoaded);
            if (this.ProjectVersion != null)
            {
                jobject.Add("Version", this.ProjectVersion.ToCacheJson());
            }
            if (this.UpdateFile != null)
            {
                jobject.Add("UpdateFile", this.UpdateFile.ToCacheJson());
            }
            return jobject;
        }

        public void FromJson(JObject Json)
        {
            this.OnlineDataLoaded = (bool)Json["OnlineDataLoaded"];
            if (Json.ContainsKey("Project"))
            {
                this.Project = new ResourceProject((JObject)Json["Project"]);
            }
            if (Json.ContainsKey("ChangelogUrls"))
            {
                this.ChangelogUrls = Json["ChangelogUrls"].ToObject<List<string>>();
            }
            if (Json.ContainsKey("Version"))
            {
                this.ProjectVersion = ResourceVersion.FromCacheJson((JObject)Json["Version"]);
            }
            if (Json.ContainsKey("UpdateFile"))
            {
                this.UpdateFile = ResourceVersion.FromCacheJson((JObject)Json["UpdateFile"]);
            }
        }

        public bool HasUpdate
        {
            get
            {
                return !Settings.Get<bool>("UiHiddenFunctionModUpdate", null) && !Settings.Get<bool>("VersionAdvanceDisableModUpdate", PageInstanceLeft.Instance) && this.UpdateFile != null;
            }
        }

        public uint CurseForgeHash
        {
            get
            {
                checked
                {
                    if (this._CurseForgeHash == null)
                    {
                        string text = Conversions.ToString(string.Format("{0}-{1}-{2}-C", this.EnabledFullName, this.File.LastWriteTime.ToLongTimeString(), this.File.Length).GetStableHashCode());
                        string text2 = ModBase.ReadIni(ModBase.PathTemp + "Cache\\ModHash.ini", text, "");
                        if (Operators.CompareString(text2, "", false) != 0 && text2.RegexCheck("^\\d+$", RegexOptions.None))
                        {
                            this._CurseForgeHash = new uint?(Conversions.ToUInteger(text2));
                            return this._CurseForgeHash.Value;
                        }
                        List<byte> list = new List<byte>();
                        foreach (byte b in FileUtils.ReadAsBytes(this.File.FullName, null))
                        {
                            if (b != 9 && b != 10 && b != 13 && b != 32)
                            {
                                list.Add(b);
                            }
                        }
                        int count = list.Count;
                        uint num = (uint)(1 ^ count);
                        int num2 = count - 4;
                        int j;
                        for (j = 0; j <= num2; j += 4)
                        {
                            uint num3 = (uint)((int)list[j] | ((int)list[j + 1] << 8) | ((int)list[j + 2] << 16) | ((int)list[j + 3] << 24));
                            num3 = (uint)((unchecked((ulong)num3) * 1540483477UL) & unchecked((ulong)(-1)));
                            num3 ^= num3 >> 24;
                            num3 = (uint)((unchecked((ulong)num3) * 1540483477UL) & unchecked((ulong)(-1)));
                            num = (uint)((unchecked((ulong)num) * 1540483477UL) & unchecked((ulong)(-1)));
                            num ^= num3;
                        }
                        switch (count - j)
                        {
                        case 1:
                            num ^= (uint)list[j];
                            num = (uint)((unchecked((ulong)num) * 1540483477UL) & unchecked((ulong)(-1)));
                            break;
                        case 2:
                            num ^= (uint)((int)list[j] | ((int)list[j + 1] << 8));
                            num = (uint)((unchecked((ulong)num) * 1540483477UL) & unchecked((ulong)(-1)));
                            break;
                        case 3:
                            num ^= (uint)((int)list[j] | ((int)list[j + 1] << 8));
                            num ^= (uint)((uint)list[j + 2] << 16);
                            num = (uint)((unchecked((ulong)num) * 1540483477UL) & unchecked((ulong)(-1)));
                            break;
                        }
                        num ^= num >> 13;
                        num = (uint)((unchecked((ulong)num) * 1540483477UL) & unchecked((ulong)(-1)));
                        num ^= num >> 15;
                        this._CurseForgeHash = new uint?(num);
                        ModBase.WriteIni(ModBase.PathTemp + "Cache\\ModHash.ini", text, num.ToString());
                    }
                    return this._CurseForgeHash.Value;
                }
            }
        }

        public string ModrinthHash
        {
            get
            {
                if (this._ModrinthHash == null)
                {
                    string text = Conversions.ToString(string.Format("{0}-{1}-{2}-M", this.EnabledFullName, this.File.LastWriteTime.ToLongTimeString(), this.File.Length).GetStableHashCode());
                    string text2 = ModBase.ReadIni(ModBase.PathTemp + "Cache\\ModHash.ini", text, "");
                    if (Operators.CompareString(text2, "", false) != 0)
                    {
                        this._ModrinthHash = text2;
                        return this._ModrinthHash;
                    }
                    this._ModrinthHash = CryptographyUtils.ComputeFileHash(this.File.FullName, CryptographyUtils.HashMethod.Sha1);
                    ModBase.WriteIni(ModBase.PathTemp + "Cache\\ModHash.ini", text, this._ModrinthHash);
                }
                return this._ModrinthHash;
            }
        }

        public override string ToString()
        {
            return this.File.FullName;
        }

        public override bool Equals(object obj)
        {
            LocalResourceFile localResourceFile = obj as LocalResourceFile;
            return localResourceFile != null && PathUtils.IsSame(this.File.FullName, localResourceFile.File.FullName);
        }

        public readonly FileInfo File;

        [CompilerGenerated]
        private LocalResourceFile.OnUpdateEventHandler OnUpdateEvent;

        private string _Display;

        private string _Description;

        public string _Version;

        private ResourceProject _Project;

        public ResourceVersion ProjectVersion;

        private ResourceVersion _UpdateFile;

        public List<string> ChangelogUrls;

        public bool OnlineDataLoaded;

        private uint? _CurseForgeHash;

        private string _ModrinthHash;

        public delegate void OnUpdateEventHandler(LocalResourceFile sender);
    }
}
