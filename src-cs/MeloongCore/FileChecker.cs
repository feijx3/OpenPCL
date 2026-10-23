using System;
using System.IO;
using System.Runtime.CompilerServices;
using MeloongCore.Extensions;

namespace MeloongCore
{
    public class FileChecker
    {
        [NullableContext(1)]
        [return: Nullable(2)]
        public string Check(string localPath)
        {
            string text;
            try
            {
                FileInfo info = FileUtils.GetInfo(localPath);
                if (!info.Exists)
                {
                    text = "文件不存在：" + localPath;
                }
                else
                {
                    long length = info.Length;
                    if (this.ActualSize >= 0L && this.ActualSize != length)
                    {
                        text = string.Format("文件大小应为 {0} B，实际为 {1} B", this.ActualSize, length) + ((length < 2000L) ? ("，内容为：" + FileUtils.ReadAsString(localPath, null, null)) : "");
                    }
                    else if (this.MinSize >= 0L && this.MinSize > length)
                    {
                        text = string.Format("文件大小应至少为 {0} B，实际为 {1} B", this.MinSize, length) + ((length < 2000L) ? ("，内容为：" + FileUtils.ReadAsString(localPath, null, null)) : "");
                    }
                    else
                    {
                        string text2 = this.Hash;
                        if (!string.IsNullOrEmpty(text2))
                        {
                            text2 = text2.Lower();
                            int length2 = text2.Length;
                            if (length2 <= 40)
                            {
                                if (length2 != 32)
                                {
                                    if (length2 == 40)
                                    {
                                        string text3 = CryptographyUtils.ComputeFileHash(localPath, CryptographyUtils.HashMethod.Sha1);
                                        if (text2 != text3)
                                        {
                                            return "文件 SHA1 应为 " + text2 + "，实际为 " + text3;
                                        }
                                        goto IL_01F4;
                                    }
                                }
                                else
                                {
                                    string text4 = CryptographyUtils.ComputeFileHash(localPath, CryptographyUtils.HashMethod.Md5);
                                    if (text2 != text4)
                                    {
                                        return "文件 MD5 应为 " + text2 + "，实际为 " + text4;
                                    }
                                    goto IL_01F4;
                                }
                            }
                            else if (length2 != 64)
                            {
                                if (length2 == 128)
                                {
                                    string text5 = CryptographyUtils.ComputeFileHash(localPath, CryptographyUtils.HashMethod.Sha512);
                                    if (text2 != text5)
                                    {
                                        return "文件 SHA512 应为 " + text2 + "，实际为 " + text5;
                                    }
                                    goto IL_01F4;
                                }
                            }
                            else
                            {
                                string text6 = CryptographyUtils.ComputeFileHash(localPath, CryptographyUtils.HashMethod.Sha256);
                                if (text2 != text6)
                                {
                                    return "文件 SHA256 应为 " + text2 + "，实际为 " + text6;
                                }
                                goto IL_01F4;
                            }
                            return string.Format("无法识别的 Hash 类型（长度为 {0}）：{1}", text2.Length, text2);
                        }
                        IL_01F4:
                        if (this.IsJson)
                        {
                            string text7 = FileUtils.ReadAsString(localPath, null, null);
                            if (text7 == "")
                            {
                                return "读取到的文件为空";
                            }
                            text7.DeserializeJson();
                        }
                        text = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "检查文件出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\FileUtils.cs");
                text = ex.GetDisplay(false);
            }
            return text;
        }

        public long ActualSize = -1L;

        public long MinSize = -1L;

        [Nullable(2)]
        public string Hash;

        public bool IsJson;
    }
}
