using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MeloongCore;
using MeloongCore.Extensions;
using Newtonsoft.Json;

namespace PCLCS
{
    [NullableContext(1)]
    [Nullable(0)]
    [Serializable]
    public class Java : IEquatable<Java>
    {
        public Java(string folder, [Nullable(2)] Version version = null)
        {
        }

        [JsonIgnore]
        public string JavaExePath
        {
            get
            {
                return Path.Combine(this.Folder, "java.exe");
            }
        }

        public Task<bool> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Java.<CheckAsync>d__7 <CheckAsync>d__;
            <CheckAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
            <CheckAsync>d__.<>4__this = this;
            <CheckAsync>d__.cancellationToken = cancellationToken;
            <CheckAsync>d__.<>1__state = -1;
            <CheckAsync>d__.<>t__builder.Start<Java.<CheckAsync>d__7>(ref <CheckAsync>d__);
            return <CheckAsync>d__.<>t__builder.Task;
        }

        public void InvalidateChecked()
        {
            this.isChecked = 0;
            this.available = null;
        }

        [NullableContext(2)]
        public bool Equals(Java other)
        {
            return other != null && PathUtils.IsSame(this.Folder, other.Folder);
        }

        [NullableContext(2)]
        public override bool Equals(object obj)
        {
            Java java = obj as Java;
            return java != null && this.Equals(java);
        }

        public override int GetHashCode()
        {
            return StringComparer.CurrentCultureIgnoreCase.GetHashCode(PathUtils.ForCompare(this.Folder));
        }

        public override string ToString()
        {
            string text = "Java {0} ({1})：{2}";
            Version version = this.Version;
            return string.Format(text, ((version != null) ? version.Major.ToString() : null) ?? "尚未检查", this.Version, this.Folder);
        }

        [CompilerGenerated]
        [return: Nullable(2)]
        internal static string <CheckAsync>g__GetProperty|7_0(string name, ref Java.<>c__DisplayClass7_0 A_1)
        {
            string prefix = name + " = ";
            string text = (from l in A_1.output.Split(new char[] { '\n' })
                select l.Trim()).FirstOrDefault<string>((string l) => l.StartsWithF(prefix, false));
            if (text == null)
            {
                return null;
            }
            return text.Substring(prefix.Length).Trim();
        }

        public readonly string Folder = PathUtils.AddSlashSuffix(PathUtils.ForCompare(folder));

        [Nullable(2)]
        public Version Version = version;

        [Nullable(2)]
        [JsonIgnore]
        public string FileEncodingName;

        [Nullable(2)]
        [JsonIgnore]
        public string NativeEncodingName;

        [JsonIgnore]
        private int isChecked;

        [JsonIgnore]
        internal bool? available;
    }
}
