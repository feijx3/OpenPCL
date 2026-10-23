using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using MeloongCore.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PCLCS
{
    [NullableContext(2)]
    [Nullable(0)]
    public class McComponents : IEquatable<McComponents>
    {
        [JsonConstructor]
        public McComponents([Nullable(1)] string MinecraftVersion, [Nullable(1)] Dictionary<ModLoaders, string> ModLoaderVersions, JObject OptiFineEntry = null, string FabricApiVersion = null, string OptiFabricVersion = null)
        {
            this.MinecraftVersion = MinecraftVersion;
            this.ModLoaderVersions = ModLoaderVersions;
            this.OptiFineEntry = OptiFineEntry;
            this.FabricApiVersion = FabricApiVersion;
            this.OptiFabricVersion = OptiFabricVersion;
            base..ctor();
        }

        [Nullable(1)]
        [CompilerGenerated]
        protected virtual Type EqualityContract
        {
            [NullableContext(1)]
            [CompilerGenerated]
            get
            {
                return typeof(McComponents);
            }
        }

        [Nullable(1)]
        public string MinecraftVersion
        {
            [NullableContext(1)]
            get;
            [NullableContext(1)]
            set;
        }

        [Nullable(1)]
        public Dictionary<ModLoaders, string> ModLoaderVersions
        {
            [NullableContext(1)]
            get;
            [NullableContext(1)]
            set;
        }

        [JsonProperty(NullValueHandling = 1)]
        public JObject OptiFineEntry { get; set; }

        [JsonProperty(NullValueHandling = 1)]
        public string FabricApiVersion { get; set; }

        [JsonProperty(NullValueHandling = 1)]
        public string OptiFabricVersion { get; set; }

        [JsonIgnore]
        public string OptiFineVersion
        {
            get
            {
                JObject optiFineEntry = this.OptiFineEntry;
                if (optiFineEntry == null)
                {
                    return null;
                }
                JToken jtoken = optiFineEntry["InstanceName"];
                if (jtoken == null)
                {
                    return null;
                }
                string text = Extensions.Value<string>(jtoken);
                if (text == null)
                {
                    return null;
                }
                return text.AfterLast("-OptiFine_", false);
            }
        }

        public McComponents([Nullable(1)] string minecraftVersion, JObject optiFineEntry, string forgeVersion, string neoForgeVersion, string fabricVersion, string fabricApiVersion, string optiFabricVersion, string liteLoaderVersion)
            : this(minecraftVersion, new Dictionary<ModLoaders, string>(), optiFineEntry, fabricApiVersion, optiFabricVersion)
        {
            if (forgeVersion != null)
            {
                this.ModLoaderVersions[ModLoaders.Forge] = forgeVersion;
            }
            if (neoForgeVersion != null)
            {
                this.ModLoaderVersions[ModLoaders.NeoForge] = neoForgeVersion;
            }
            if (fabricVersion != null)
            {
                this.ModLoaderVersions[ModLoaders.Fabric] = fabricVersion;
            }
            if (liteLoaderVersion != null)
            {
                this.ModLoaderVersions[ModLoaders.LiteLoader] = liteLoaderVersion;
            }
        }

        [NullableContext(1)]
        public bool IsEquivalentTo(McComponents other)
        {
            return this.MinecraftVersion == other.MinecraftVersion && this.OptiFineVersion == other.OptiFineVersion && this.FabricApiVersion == other.FabricApiVersion && this.OptiFabricVersion == other.OptiFabricVersion && this.ModLoaderVersions.Count == other.ModLoaderVersions.Count && this.ModLoaderVersions.All<KeyValuePair<ModLoaders, string>>(delegate(KeyValuePair<ModLoaders, string> entry)
            {
                string text;
                return other.ModLoaderVersions.TryGetValue(entry.Key, out text) && text == entry.Value;
            });
        }

        [NullableContext(1)]
        [CompilerGenerated]
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("McComponents");
            stringBuilder.Append(" { ");
            if (this.PrintMembers(stringBuilder))
            {
                stringBuilder.Append(' ');
            }
            stringBuilder.Append('}');
            return stringBuilder.ToString();
        }

        [NullableContext(1)]
        [CompilerGenerated]
        protected virtual bool PrintMembers(StringBuilder builder)
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();
            builder.Append("MinecraftVersion = ");
            builder.Append(this.MinecraftVersion);
            builder.Append(", ModLoaderVersions = ");
            builder.Append(this.ModLoaderVersions);
            builder.Append(", OptiFineEntry = ");
            builder.Append(this.OptiFineEntry);
            builder.Append(", FabricApiVersion = ");
            builder.Append(this.FabricApiVersion);
            builder.Append(", OptiFabricVersion = ");
            builder.Append(this.OptiFabricVersion);
            builder.Append(", OptiFineVersion = ");
            builder.Append(this.OptiFineVersion);
            return true;
        }

        [CompilerGenerated]
        public static bool operator !=(McComponents left, McComponents right)
        {
            return !(left == right);
        }

        [CompilerGenerated]
        public static bool operator ==(McComponents left, McComponents right)
        {
            return left == right || (left != null && left.Equals(right));
        }

        [CompilerGenerated]
        public override int GetHashCode()
        {
            return ((((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<MinecraftVersion>k__BackingField)) * -1521134295 + EqualityComparer<Dictionary<ModLoaders, string>>.Default.GetHashCode(this.<ModLoaderVersions>k__BackingField)) * -1521134295 + EqualityComparer<JObject>.Default.GetHashCode(this.<OptiFineEntry>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<FabricApiVersion>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<OptiFabricVersion>k__BackingField);
        }

        [CompilerGenerated]
        public override bool Equals(object obj)
        {
            return this.Equals(obj as McComponents);
        }

        [CompilerGenerated]
        public virtual bool Equals(McComponents other)
        {
            return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.<MinecraftVersion>k__BackingField, other.<MinecraftVersion>k__BackingField) && EqualityComparer<Dictionary<ModLoaders, string>>.Default.Equals(this.<ModLoaderVersions>k__BackingField, other.<ModLoaderVersions>k__BackingField) && EqualityComparer<JObject>.Default.Equals(this.<OptiFineEntry>k__BackingField, other.<OptiFineEntry>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<FabricApiVersion>k__BackingField, other.<FabricApiVersion>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<OptiFabricVersion>k__BackingField, other.<OptiFabricVersion>k__BackingField));
        }

        [CompilerGenerated]
        protected McComponents([Nullable(1)] McComponents original)
        {
            this.MinecraftVersion = original.<MinecraftVersion>k__BackingField;
            this.ModLoaderVersions = original.<ModLoaderVersions>k__BackingField;
            this.OptiFineEntry = original.<OptiFineEntry>k__BackingField;
            this.FabricApiVersion = original.<FabricApiVersion>k__BackingField;
            this.OptiFabricVersion = original.<OptiFabricVersion>k__BackingField;
        }

        [CompilerGenerated]
        public void Deconstruct([Nullable(1)] out string MinecraftVersion, [Nullable(1)] out Dictionary<ModLoaders, string> ModLoaderVersions, out JObject OptiFineEntry, out string FabricApiVersion, out string OptiFabricVersion)
        {
            MinecraftVersion = this.MinecraftVersion;
            ModLoaderVersions = this.ModLoaderVersions;
            OptiFineEntry = this.OptiFineEntry;
            FabricApiVersion = this.FabricApiVersion;
            OptiFabricVersion = this.OptiFabricVersion;
        }
    }
}
