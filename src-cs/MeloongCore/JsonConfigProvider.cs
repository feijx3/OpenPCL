using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public class JsonConfigProvider : IConfigProvider
    {
        public JsonConfigProvider(string filePath)
        {
            this.filePath = filePath;
            JsonConfigProvider.allProviders.Add(new WeakReference<JsonConfigProvider>(this));
            this.saveWorker = new RateLimitedWorker(new Action(this.Save), 500.0, RateLimitMode.WaitUnillNoNewInvoke);
            this.InitJson();
        }

        private void InitJson()
        {
            this.json = new Lazy<JObject>(delegate
            {
                JObject jobject;
                try
                {
                    jobject = (FileUtils.Exists(this.filePath) ? (((JObject)FileUtils.ReadAsJson(this.filePath, null, null)) ?? new JObject()) : new JObject());
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "读取配置文件失败（" + this.filePath + "）", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\General\\Config.cs");
                    jobject = new JObject();
                }
                return jobject;
            });
        }

        [NullableContext(2)]
        public void Set<T>([Nullable(1)] string key, T value, bool encrypted)
        {
            this.cache[key] = value;
            lock (this)
            {
                if (value == null)
                {
                    this.json.Value[key] = JValue.CreateNull();
                    bool flag2;
                    Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(15, 2, out flag2);
                    if (flag2)
                    {
                        logInterpolatedStringHandler.AppendLiteral("配置已修改：");
                        logInterpolatedStringHandler.AppendFormatted(key);
                        logInterpolatedStringHandler.AppendLiteral(" = null（");
                        logInterpolatedStringHandler.AppendFormatted(this.filePath);
                        logInterpolatedStringHandler.AppendLiteral("）");
                    }
                    Logger.Trace(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\General\\Config.cs");
                }
                else
                {
                    if (encrypted)
                    {
                        throw new NotImplementedException("加密需要改为使用识别码，现在尚未实现。");
                    }
                    JToken jtoken = JToken.FromObject(value);
                    this.json.Value[key] = jtoken;
                    bool flag2;
                    Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler2 = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(11, 3, out flag2);
                    if (flag2)
                    {
                        logInterpolatedStringHandler2.AppendLiteral("配置已修改：");
                        logInterpolatedStringHandler2.AppendFormatted(key);
                        logInterpolatedStringHandler2.AppendLiteral(" = ");
                        logInterpolatedStringHandler2.AppendFormatted(jtoken.ToString(0, Array.Empty<JsonConverter>()));
                        logInterpolatedStringHandler2.AppendLiteral("（");
                        logInterpolatedStringHandler2.AppendFormatted(this.filePath);
                        logInterpolatedStringHandler2.AppendLiteral("）");
                    }
                    Logger.Trace(ref logInterpolatedStringHandler2, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\General\\Config.cs");
                }
            }
            this.MakeDirty();
        }

        [NullableContext(2)]
        public T Read<T>([Nullable(1)] string key, T defaultValue, bool encrypted)
        {
            object obj;
            if (this.cache.TryGetValue(key, out obj))
            {
                return (T)((object)obj);
            }
            JToken jtoken;
            lock (this)
            {
                if (!this.json.Value.TryGetValue(key, ref jtoken))
                {
                    return defaultValue;
                }
                jtoken = jtoken.DeepClone();
            }
            T t2;
            try
            {
                JTokenType type = jtoken.Type;
                bool flag = type - 10 <= 1;
                T t;
                if (flag)
                {
                    t = default(T);
                }
                else if (encrypted)
                {
                    string text = CryptographyUtils.AesDecrypt(Extensions.Value<string>(jtoken), "EncryptKey");
                    t = ((typeof(T) == typeof(string)) ? ((T)((object)text)) : JsonConvert.DeserializeObject<T>(text));
                }
                else
                {
                    t = jtoken.ToObject<T>();
                }
                this.cache[key] = t;
                t2 = t;
            }
            catch (CryptographicException ex)
            {
                Logger.Error(ex, "解密配置失败，该配置将被重置（" + key + "）", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\General\\Config.cs");
                this.Remove(key);
                t2 = defaultValue;
            }
            return t2;
        }

        public bool HasValue(string key)
        {
            bool flag2;
            lock (this)
            {
                flag2 = this.json.Value.ContainsKey(key);
            }
            return flag2;
        }

        public void Remove(string key)
        {
            lock (this)
            {
                this.json.Value.Remove(key);
            }
            object obj;
            this.cache.TryRemove(key, out obj);
            this.MakeDirty();
        }

        public void DiscardCache()
        {
            lock (this)
            {
                this.InitJson();
            }
            this.cache.Clear();
        }

        public void MakeDirty()
        {
            lock (this)
            {
                this.isDirty = true;
            }
            this.saveWorker.Start();
        }

        public void Save()
        {
            lock (this)
            {
                if (this.isDirty)
                {
                    FileUtils.WriteAtomic(this.filePath, this.json.Value.ToString(1, Array.Empty<JsonConverter>()), null);
                    this.isDirty = false;
                }
            }
        }

        public static readonly ConcurrentBag<WeakReference<JsonConfigProvider>> allProviders = new ConcurrentBag<WeakReference<JsonConfigProvider>>();

        private readonly string filePath;

        private Lazy<JObject> json;

        [Nullable(new byte[] { 1, 1, 2 })]
        private readonly ConcurrentDictionary<string, object> cache = new ConcurrentDictionary<string, object>();

        private readonly RateLimitedWorker saveWorker;

        private bool isDirty;
    }
}
