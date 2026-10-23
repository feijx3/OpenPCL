using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class ConfigUtils
    {
        public static void SaveAll()
        {
            using (IEnumerator<WeakReference<JsonConfigProvider>> enumerator = JsonConfigProvider.allProviders.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    JsonConfigProvider jsonConfigProvider;
                    if (enumerator.Current.TryGetTarget(out jsonConfigProvider))
                    {
                        jsonConfigProvider.Save();
                    }
                }
            }
        }

        public static string GetSecret(string key)
        {
            string text = ConfigUtils.secret.Value.Value<string>(key);
            if (text == null)
            {
                throw new KeyNotFoundException("未找到密钥：" + key);
            }
            string text2 = text;
            string text3;
            try
            {
                text3 = CryptographyUtils.AesDecrypt(text2, "EncryptKey");
            }
            catch
            {
                throw new InvalidDataException("读取密钥失败（" + key + "），请检查文件内容与加密格式。");
            }
            return text3;
        }

        private static readonly Lazy<JObject> secret = new Lazy<JObject>(delegate
        {
            string text = Path.Combine(Paths.AppData, "Meloong", "Dev", "secret.json");
            JObject jobject;
            try
            {
                jobject = (FileUtils.Exists(text) ? JObject.Parse(FileUtils.ReadAsString(text, null, null)) : new JObject());
            }
            catch
            {
                throw new InvalidDataException("读取密钥文件失败（" + text + "），请检查文件内容与加密格式。");
            }
            return jobject;
        });
    }
}
