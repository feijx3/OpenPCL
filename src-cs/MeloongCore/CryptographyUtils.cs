using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using MeloongCore.Extensions;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class CryptographyUtils
    {
        private static HashAlgorithm GetHashAlgorithm(CryptographyUtils.HashMethod method)
        {
            HashAlgorithm hashAlgorithm;
            switch (method)
            {
            case CryptographyUtils.HashMethod.Md5:
                hashAlgorithm = MD5.Create();
                break;
            case CryptographyUtils.HashMethod.Sha1:
                hashAlgorithm = SHA1.Create();
                break;
            case CryptographyUtils.HashMethod.Sha256:
                hashAlgorithm = SHA256.Create();
                break;
            case CryptographyUtils.HashMethod.Sha512:
                hashAlgorithm = SHA512.Create();
                break;
            default:
                throw new ArgumentOutOfRangeException("method");
            }
            return hashAlgorithm;
        }

        public static string ComputeFileHash(string filePath, CryptographyUtils.HashMethod method = CryptographyUtils.HashMethod.Md5)
        {
            string text;
            using (HashAlgorithm hashAlgorithm = CryptographyUtils.GetHashAlgorithm(method))
            {
                bool flag;
                Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(6, 2, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler.AppendLiteral("计算文件 ");
                    logInterpolatedStringHandler.AppendFormatted<CryptographyUtils.HashMethod>(method);
                    logInterpolatedStringHandler.AppendLiteral("：");
                    logInterpolatedStringHandler.AppendFormatted(filePath);
                }
                Logger.Trace(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\CryptographyUtils.cs");
                using (Stream stream = FileUtils.ReadAsStream(filePath, null))
                {
                    text = BitConverter.ToString(hashAlgorithm.ComputeHash(stream)).Replace("-", "").Lower();
                }
            }
            return text;
        }

        public static string ComputeHash(byte[] input, CryptographyUtils.HashMethod method = CryptographyUtils.HashMethod.Md5)
        {
            string text;
            using (HashAlgorithm hashAlgorithm = CryptographyUtils.GetHashAlgorithm(method))
            {
                text = BitConverter.ToString(hashAlgorithm.ComputeHash(input)).Replace("-", "").Lower();
            }
            return text;
        }

        public static string ComputeHash(string input, CryptographyUtils.HashMethod method = CryptographyUtils.HashMethod.Md5)
        {
            return CryptographyUtils.ComputeHash(Encoding.UTF8.GetBytes(input), method);
        }

        [NullableContext(2)]
        public static string AesEncrypt(string sourceString, [Nullable(1)] string key = "EncryptKey")
        {
            if (string.IsNullOrEmpty(sourceString))
            {
                return sourceString;
            }
            byte[] array = new byte[16];
            string text;
            using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(array);
                using (SHA256 sha = SHA256.Create())
                {
                    byte[] array2 = sha.ComputeHash(Encoding.UTF8.GetBytes(key));
                    byte[] array3 = sha.ComputeHash(Encoding.UTF8.GetBytes("AES-HMAC|" + key));
                    using (Aes aes = Aes.Create())
                    {
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;
                        using (ICryptoTransform cryptoTransform = aes.CreateEncryptor(array2, array))
                        {
                            byte[] bytes = Encoding.UTF8.GetBytes(sourceString);
                            byte[] array4 = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
                            byte[] array5 = new byte[1 + array.Length + array4.Length + 4];
                            array5[0] = 1;
                            Array.Copy(array, 0, array5, 1, array.Length);
                            Array.Copy(array4, 0, array5, 1 + array.Length, array4.Length);
                            using (HMACSHA256 hmacsha = new HMACSHA256(array3))
                            {
                                Array.Copy(hmacsha.ComputeHash(array5, 0, 1 + array.Length + array4.Length), 0, array5, 1 + array.Length + array4.Length, 4);
                                text = Convert.ToBase64String(array5);
                            }
                        }
                    }
                }
            }
            return text;
        }

        [NullableContext(2)]
        public static string AesDecrypt(string encryptedString, [Nullable(1)] string key = "EncryptKey")
        {
            if (string.IsNullOrEmpty(encryptedString))
            {
                return encryptedString;
            }
            byte[] array;
            try
            {
                array = Convert.FromBase64String(encryptedString);
            }
            catch (FormatException ex)
            {
                throw new CryptographicException("数据不是一个有效的 Base64 字符串（" + encryptedString + "）", ex);
            }
            if (array.Length == 0)
            {
                throw new CryptographicException("对称加密数据长度有误（" + encryptedString + "）");
            }
            int num = array.Length - 1 - 16 - 4;
            if (array[0] != 1 || num < 16 || num % 16 != 0)
            {
                throw new CryptographicException("对称加密数据长度有误（" + encryptedString + "）");
            }
            string @string;
            using (SHA256 sha = SHA256.Create())
            {
                using (HMACSHA256 hmacsha = new HMACSHA256(sha.ComputeHash(Encoding.UTF8.GetBytes("AES-HMAC|" + key))))
                {
                    byte[] array2 = hmacsha.ComputeHash(array, 0, 17 + num);
                    int num2 = 17 + num;
                    int num3 = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        num3 |= (int)(array[num2 + i] ^ array2[i]);
                    }
                    if (num3 != 0)
                    {
                        throw new CryptographicException("对称加密数据校验失败（" + encryptedString + "）");
                    }
                    byte[] array3 = sha.ComputeHash(Encoding.UTF8.GetBytes(key));
                    byte[] array4 = new byte[16];
                    Array.Copy(array, 1, array4, 0, array4.Length);
                    using (Aes aes = Aes.Create())
                    {
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;
                        using (ICryptoTransform cryptoTransform = aes.CreateDecryptor(array3, array4))
                        {
                            byte[] array5 = cryptoTransform.TransformFinalBlock(array, 1 + array4.Length, num);
                            @string = Encoding.UTF8.GetString(array5);
                        }
                    }
                }
            }
            return @string;
        }

        public static void EcdsaVerify(string sourceString, string sign, string publicKey)
        {
            using (HashAlgorithm hashAlgorithm = CryptographyUtils.GetHashAlgorithm(CryptographyUtils.HashMethod.Sha256))
            {
                byte[] array = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(sourceString));
                IntPtr zero = IntPtr.Zero;
                IntPtr zero2 = IntPtr.Zero;
                try
                {
                    int num = CryptographyUtils.BCryptOpenAlgorithmProvider(out zero, "ECDSA_P256", null, 0);
                    if (num < 0)
                    {
                        throw new CryptographicException(string.Format("{0} 失败，错误码 {1}", "BCryptOpenAlgorithmProvider", num));
                    }
                    num = CryptographyUtils.BCryptImportKeyPair(zero, IntPtr.Zero, "ECCPUBLICBLOB", out zero2, Convert.FromBase64String(publicKey), 72, 0);
                    if (num < 0)
                    {
                        throw new CryptographicException(string.Format("{0} 失败，错误码 {1}", "BCryptImportKeyPair", num));
                    }
                    num = CryptographyUtils.BCryptVerifySignature(zero2, IntPtr.Zero, array, array.Length, Convert.FromBase64String(sign), 64, 0);
                    if (num == -1073700864)
                    {
                        throw new SecurityException("签名验证失败");
                    }
                    if (num < 0)
                    {
                        throw new CryptographicException(string.Format("{0} 失败，错误码 {1}", "BCryptVerifySignature", num));
                    }
                }
                finally
                {
                    if (zero2 != IntPtr.Zero)
                    {
                        CryptographyUtils.BCryptDestroyKey(zero2);
                    }
                    if (zero != IntPtr.Zero)
                    {
                        CryptographyUtils.BCryptCloseAlgorithmProvider(zero, 0);
                    }
                }
            }
        }

        public static string EcdsaSign(string sourceString, string privateKey)
        {
            string text;
            using (HashAlgorithm hashAlgorithm = CryptographyUtils.GetHashAlgorithm(CryptographyUtils.HashMethod.Sha256))
            {
                byte[] array = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(sourceString));
                byte[] array2 = Convert.FromBase64String(privateKey);
                IntPtr zero = IntPtr.Zero;
                IntPtr zero2 = IntPtr.Zero;
                try
                {
                    int num = CryptographyUtils.BCryptOpenAlgorithmProvider(out zero, "ECDSA_P256", null, 0);
                    if (num < 0)
                    {
                        throw new CryptographicException(string.Format("{0} 失败，错误码 {1}", "BCryptOpenAlgorithmProvider", num));
                    }
                    num = CryptographyUtils.BCryptImportKeyPair(zero, IntPtr.Zero, "ECCPRIVATEBLOB", out zero2, array2, array2.Length, 0);
                    if (num < 0)
                    {
                        throw new CryptographicException(string.Format("{0} 失败，错误码 {1}", "BCryptImportKeyPair", num));
                    }
                    int num2;
                    num = CryptographyUtils.BCryptSignHash(zero2, IntPtr.Zero, array, array.Length, null, 0, out num2, 0);
                    if (num < 0)
                    {
                        throw new CryptographicException(string.Format("{0} 失败，错误码 {1}", "BCryptSignHash", num));
                    }
                    byte[] array3 = new byte[num2];
                    int num3;
                    num = CryptographyUtils.BCryptSignHash(zero2, IntPtr.Zero, array, array.Length, array3, array3.Length, out num3, 0);
                    if (num < 0)
                    {
                        throw new CryptographicException(string.Format("{0} 失败，错误码 {1}", "BCryptSignHash", num));
                    }
                    if (num3 != array3.Length)
                    {
                        Array.Resize<byte>(ref array3, num3);
                    }
                    text = Convert.ToBase64String(array3);
                }
                finally
                {
                    if (zero2 != IntPtr.Zero)
                    {
                        CryptographyUtils.BCryptDestroyKey(zero2);
                    }
                    if (zero != IntPtr.Zero)
                    {
                        CryptographyUtils.BCryptCloseAlgorithmProvider(zero, 0);
                    }
                }
            }
            return text;
        }

        [DllImport("bcrypt.dll", CharSet = CharSet.Unicode)]
        private static extern int BCryptOpenAlgorithmProvider(out IntPtr algorithmHandle, string algorithmId, [Nullable(2)] string implementation, int flags);

        [DllImport("bcrypt.dll", CharSet = CharSet.Unicode)]
        private static extern int BCryptImportKeyPair(IntPtr algorithmHandle, IntPtr importKey, string blobType, out IntPtr keyHandle, byte[] input, int inputSize, int flags);

        [DllImport("bcrypt.dll")]
        private static extern int BCryptVerifySignature(IntPtr keyHandle, IntPtr paddingInfo, byte[] hash, int hashSize, byte[] signature, int signatureSize, int flags);

        [DllImport("bcrypt.dll")]
        private static extern int BCryptDestroyKey(IntPtr keyHandle);

        [DllImport("bcrypt.dll")]
        private static extern int BCryptCloseAlgorithmProvider(IntPtr algorithmHandle, int flags);

        [DllImport("bcrypt.dll")]
        private static extern int BCryptSignHash(IntPtr keyHandle, IntPtr paddingInfo, byte[] hash, int hashSize, [Nullable(2)] byte[] signature, int signatureSize, out int resultSize, int flags);

        private const byte AesDataVersion = 1;

        private const int AesIvSize = 16;

        private const int AesCheckSize = 4;

        [NullableContext(0)]
        public enum HashMethod
        {
            Md5,
            Sha1,
            Sha256,
            Sha512
        }
    }
}
