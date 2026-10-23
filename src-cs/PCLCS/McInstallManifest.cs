using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace PCLCS
{
    [NullableContext(1)]
    [Nullable(0)]
    public class McInstallManifest : IEquatable<McInstallManifest>
    {
        public McInstallManifest(int SchemaVersion, McComponents Components, DateTime LastModifiedUtc, IReadOnlyList<McInstallManifest.ManagedFile> ManagedFiles)
        {
            this.SchemaVersion = SchemaVersion;
            this.Components = Components;
            this.LastModifiedUtc = LastModifiedUtc;
            this.ManagedFiles = ManagedFiles;
            base..ctor();
        }

        [CompilerGenerated]
        protected virtual Type EqualityContract
        {
            [CompilerGenerated]
            get
            {
                return typeof(McInstallManifest);
            }
        }

        public int SchemaVersion { get; set; }

        public McComponents Components { get; set; }

        public DateTime LastModifiedUtc { get; set; }

        public IReadOnlyList<McInstallManifest.ManagedFile> ManagedFiles { get; set; }

        public McInstallManifest RefreshModComponents(string gameFolder)
        {
            List<McInstallManifest.ManagedFile> invalidFiles = new List<McInstallManifest.ManagedFile>();
            foreach (McInstallManifest.ManagedFile managedFile in this.ManagedFiles)
            {
                if (managedFile.Component != null)
                {
                    FileInfo fileInfo = new FileInfo(Path.Combine(gameFolder, managedFile.RelativePath));
                    if (!fileInfo.Exists || fileInfo.LastWriteTimeUtc != managedFile.LastWriteTimeUtc.ToUniversalTime())
                    {
                        invalidFiles.Add(managedFile);
                    }
                }
            }
            if (!invalidFiles.Any<McInstallManifest.ManagedFile>())
            {
                return this;
            }
            McInstallManifest mcInstallManifest = this.<Clone>$();
            McComponents mcComponents = this.Components.<Clone>$();
            mcComponents.OptiFineEntry = (invalidFiles.Any<McInstallManifest.ManagedFile>((McInstallManifest.ManagedFile file) => file.Component == "OptiFine") ? null : this.Components.OptiFineEntry);
            mcComponents.FabricApiVersion = (invalidFiles.Any<McInstallManifest.ManagedFile>((McInstallManifest.ManagedFile file) => file.Component == "FabricApi") ? null : this.Components.FabricApiVersion);
            mcComponents.OptiFabricVersion = (invalidFiles.Any<McInstallManifest.ManagedFile>((McInstallManifest.ManagedFile file) => file.Component == "OptiFabric") ? null : this.Components.OptiFabricVersion);
            mcInstallManifest.Components = mcComponents;
            mcInstallManifest.ManagedFiles = this.ManagedFiles.Where<McInstallManifest.ManagedFile>((McInstallManifest.ManagedFile file) => !invalidFiles.Contains(file)).ToList<McInstallManifest.ManagedFile>();
            mcInstallManifest.LastModifiedUtc = DateTime.UtcNow;
            return mcInstallManifest;
        }

        [CompilerGenerated]
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("McInstallManifest");
            stringBuilder.Append(" { ");
            if (this.PrintMembers(stringBuilder))
            {
                stringBuilder.Append(' ');
            }
            stringBuilder.Append('}');
            return stringBuilder.ToString();
        }

        [CompilerGenerated]
        protected virtual bool PrintMembers(StringBuilder builder)
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();
            builder.Append("SchemaVersion = ");
            builder.Append(this.SchemaVersion.ToString());
            builder.Append(", Components = ");
            builder.Append(this.Components);
            builder.Append(", LastModifiedUtc = ");
            builder.Append(this.LastModifiedUtc.ToString());
            builder.Append(", ManagedFiles = ");
            builder.Append(this.ManagedFiles);
            return true;
        }

        [NullableContext(2)]
        [CompilerGenerated]
        public static bool operator !=(McInstallManifest left, McInstallManifest right)
        {
            return !(left == right);
        }

        [NullableContext(2)]
        [CompilerGenerated]
        public static bool operator ==(McInstallManifest left, McInstallManifest right)
        {
            return left == right || (left != null && left.Equals(right));
        }

        [CompilerGenerated]
        public override int GetHashCode()
        {
            return (((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.<SchemaVersion>k__BackingField)) * -1521134295 + EqualityComparer<McComponents>.Default.GetHashCode(this.<Components>k__BackingField)) * -1521134295 + EqualityComparer<DateTime>.Default.GetHashCode(this.<LastModifiedUtc>k__BackingField)) * -1521134295 + EqualityComparer<IReadOnlyList<McInstallManifest.ManagedFile>>.Default.GetHashCode(this.<ManagedFiles>k__BackingField);
        }

        [NullableContext(2)]
        [CompilerGenerated]
        public override bool Equals(object obj)
        {
            return this.Equals(obj as McInstallManifest);
        }

        [NullableContext(2)]
        [CompilerGenerated]
        public virtual bool Equals(McInstallManifest other)
        {
            return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<int>.Default.Equals(this.<SchemaVersion>k__BackingField, other.<SchemaVersion>k__BackingField) && EqualityComparer<McComponents>.Default.Equals(this.<Components>k__BackingField, other.<Components>k__BackingField) && EqualityComparer<DateTime>.Default.Equals(this.<LastModifiedUtc>k__BackingField, other.<LastModifiedUtc>k__BackingField) && EqualityComparer<IReadOnlyList<McInstallManifest.ManagedFile>>.Default.Equals(this.<ManagedFiles>k__BackingField, other.<ManagedFiles>k__BackingField));
        }

        [CompilerGenerated]
        protected McInstallManifest(McInstallManifest original)
        {
            this.SchemaVersion = original.<SchemaVersion>k__BackingField;
            this.Components = original.<Components>k__BackingField;
            this.LastModifiedUtc = original.<LastModifiedUtc>k__BackingField;
            this.ManagedFiles = original.<ManagedFiles>k__BackingField;
        }

        [CompilerGenerated]
        public void Deconstruct(out int SchemaVersion, out McComponents Components, out DateTime LastModifiedUtc, out IReadOnlyList<McInstallManifest.ManagedFile> ManagedFiles)
        {
            SchemaVersion = this.SchemaVersion;
            Components = this.Components;
            LastModifiedUtc = this.LastModifiedUtc;
            ManagedFiles = this.ManagedFiles;
        }

        [Nullable(0)]
        public class ManagedFile : IEquatable<McInstallManifest.ManagedFile>
        {
            public ManagedFile(string RelativePath, string Sha1, DateTime LastWriteTimeUtc, [Nullable(2)] string Component = null)
            {
                this.RelativePath = RelativePath;
                this.Sha1 = Sha1;
                this.LastWriteTimeUtc = LastWriteTimeUtc;
                this.Component = Component;
                base..ctor();
            }

            [CompilerGenerated]
            protected virtual Type EqualityContract
            {
                [CompilerGenerated]
                get
                {
                    return typeof(McInstallManifest.ManagedFile);
                }
            }

            public string RelativePath { get; set; }

            public string Sha1 { get; set; }

            public DateTime LastWriteTimeUtc { get; set; }

            [Nullable(2)]
            [JsonProperty(NullValueHandling = 1)]
            public string Component
            {
                [NullableContext(2)]
                get;
                [NullableContext(2)]
                set;
            }

            [CompilerGenerated]
            public override string ToString()
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("ManagedFile");
                stringBuilder.Append(" { ");
                if (this.PrintMembers(stringBuilder))
                {
                    stringBuilder.Append(' ');
                }
                stringBuilder.Append('}');
                return stringBuilder.ToString();
            }

            [CompilerGenerated]
            protected virtual bool PrintMembers(StringBuilder builder)
            {
                RuntimeHelpers.EnsureSufficientExecutionStack();
                builder.Append("RelativePath = ");
                builder.Append(this.RelativePath);
                builder.Append(", Sha1 = ");
                builder.Append(this.Sha1);
                builder.Append(", LastWriteTimeUtc = ");
                builder.Append(this.LastWriteTimeUtc.ToString());
                builder.Append(", Component = ");
                builder.Append(this.Component);
                return true;
            }

            [NullableContext(2)]
            [CompilerGenerated]
            public static bool operator !=(McInstallManifest.ManagedFile left, McInstallManifest.ManagedFile right)
            {
                return !(left == right);
            }

            [NullableContext(2)]
            [CompilerGenerated]
            public static bool operator ==(McInstallManifest.ManagedFile left, McInstallManifest.ManagedFile right)
            {
                return left == right || (left != null && left.Equals(right));
            }

            [CompilerGenerated]
            public override int GetHashCode()
            {
                return (((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<RelativePath>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<Sha1>k__BackingField)) * -1521134295 + EqualityComparer<DateTime>.Default.GetHashCode(this.<LastWriteTimeUtc>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<Component>k__BackingField);
            }

            [NullableContext(2)]
            [CompilerGenerated]
            public override bool Equals(object obj)
            {
                return this.Equals(obj as McInstallManifest.ManagedFile);
            }

            [NullableContext(2)]
            [CompilerGenerated]
            public virtual bool Equals(McInstallManifest.ManagedFile other)
            {
                return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.<RelativePath>k__BackingField, other.<RelativePath>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<Sha1>k__BackingField, other.<Sha1>k__BackingField) && EqualityComparer<DateTime>.Default.Equals(this.<LastWriteTimeUtc>k__BackingField, other.<LastWriteTimeUtc>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<Component>k__BackingField, other.<Component>k__BackingField));
            }

            [CompilerGenerated]
            protected ManagedFile(McInstallManifest.ManagedFile original)
            {
                this.RelativePath = original.<RelativePath>k__BackingField;
                this.Sha1 = original.<Sha1>k__BackingField;
                this.LastWriteTimeUtc = original.<LastWriteTimeUtc>k__BackingField;
                this.Component = original.<Component>k__BackingField;
            }

            [CompilerGenerated]
            public void Deconstruct(out string RelativePath, out string Sha1, out DateTime LastWriteTimeUtc, [Nullable(2)] out string Component)
            {
                RelativePath = this.RelativePath;
                Sha1 = this.Sha1;
                LastWriteTimeUtc = this.LastWriteTimeUtc;
                Component = this.Component;
            }
        }
    }
}
