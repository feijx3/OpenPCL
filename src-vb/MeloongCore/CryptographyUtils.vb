Imports System
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Security.Cryptography
Imports System.Text
Imports MeloongCore.Extensions

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Module CryptographyUtils
        Private Function GetHashAlgorithm(method As CryptographyUtils.HashMethod) As HashAlgorithm
            Dim hashAlgorithm As HashAlgorithm
            Select Case method
                Case CryptographyUtils.HashMethod.Md5
                    hashAlgorithm = MD5.Create()
                Case CryptographyUtils.HashMethod.Sha1
                    hashAlgorithm = SHA1.Create()
                Case CryptographyUtils.HashMethod.Sha256
                    hashAlgorithm = SHA256.Create()
                Case CryptographyUtils.HashMethod.Sha512
                    hashAlgorithm = SHA512.Create()
                Case Else
                    Throw New ArgumentOutOfRangeException("method")
            End Select
            Return hashAlgorithm
        End Function

        Public Function ComputeFileHash(filePath As String, Optional method As CryptographyUtils.HashMethod = CryptographyUtils.HashMethod.Md5) As String
            Dim text As String
            Using hashAlgorithm As HashAlgorithm = CryptographyUtils.GetHashAlgorithm(method)
                Dim flag As Boolean
                Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(6, 2, flag)
                If flag Then
                    logInterpolatedStringHandler.AppendLiteral("计算文件 ")
                    logInterpolatedStringHandler.AppendFormatted(Of CryptographyUtils.HashMethod)(method)
                    logInterpolatedStringHandler.AppendLiteral("：")
                    logInterpolatedStringHandler.AppendFormatted(filePath)
                End If
                Logger.Trace(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\CryptographyUtils.cs")
                Using stream As Stream = FileUtils.ReadAsStream(filePath, Nothing)
                    text = BitConverter.ToString(hashAlgorithm.ComputeHash(stream)).Replace("-", "").Lower()
                End Using
            End Using
            Return text
        End Function

        Public Function ComputeHash(input As Byte(), Optional method As CryptographyUtils.HashMethod = CryptographyUtils.HashMethod.Md5) As String
            Dim text As String
            Using hashAlgorithm As HashAlgorithm = CryptographyUtils.GetHashAlgorithm(method)
                text = BitConverter.ToString(hashAlgorithm.ComputeHash(input)).Replace("-", "").Lower()
            End Using
            Return text
        End Function

        Public Function ComputeHash(input As String, Optional method As CryptographyUtils.HashMethod = CryptographyUtils.HashMethod.Md5) As String
            Return CryptographyUtils.ComputeHash(Encoding.UTF8.GetBytes(input), method)
        End Function

        <NullableContext(2)>
        Public Function AesEncrypt(sourceString As String, <Nullable(1)> Optional key As String = "EncryptKey") As String
            If String.IsNullOrEmpty(sourceString) Then
                Return sourceString
            End If
            Dim array As Byte() = New Byte(15) {}
            Dim text As String
            Using randomNumberGenerator As RandomNumberGenerator = RandomNumberGenerator.Create()
                randomNumberGenerator.GetBytes(array)
                Using sha As SHA256 = SHA256.Create()
                    Dim array2 As Byte() = sha.ComputeHash(Encoding.UTF8.GetBytes(key))
                    Dim array3 As Byte() = sha.ComputeHash(Encoding.UTF8.GetBytes("AES-HMAC|" + key))
                    Using aes As Aes = Aes.Create()
                        aes.Mode = CipherMode.CBC
                        aes.Padding = PaddingMode.PKCS7
                        Using cryptoTransform As ICryptoTransform = aes.CreateEncryptor(array2, array)
                            Dim bytes As Byte() = Encoding.UTF8.GetBytes(sourceString)
                            Dim array4 As Byte() = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length)
                            Dim array5 As Byte() = New Byte(1 + array.Length + array4.Length + 4 - 1) {}
                            array5(0) = 1
                            Array.Copy(array, 0, array5, 1, array.Length)
                            Array.Copy(array4, 0, array5, 1 + array.Length, array4.Length)
                            Using hmacsha As HMACSHA256 = New HMACSHA256(array3)
                                Array.Copy(hmacsha.ComputeHash(array5, 0, 1 + array.Length + array4.Length), 0, array5, 1 + array.Length + array4.Length, 4)
                                text = Convert.ToBase64String(array5)
                            End Using
                        End Using
                    End Using
                End Using
            End Using
            Return text
        End Function

        <NullableContext(2)>
        Public Function AesDecrypt(encryptedString As String, <Nullable(1)> Optional key As String = "EncryptKey") As String
            If String.IsNullOrEmpty(encryptedString) Then
                Return encryptedString
            End If
            Dim array As Byte()
            Try
                array = Convert.FromBase64String(encryptedString)
            Catch ex As FormatException
                Throw New CryptographicException("数据不是一个有效的 Base64 字符串（" + encryptedString + "）", ex)
            End Try
            If array.Length = 0 Then
                Throw New CryptographicException("对称加密数据长度有误（" + encryptedString + "）")
            End If
            Dim num As Integer = array.Length - 1 - 16 - 4
            If array(0) <> 1 OrElse num < 16 OrElse num Mod 16 <> 0 Then
                Throw New CryptographicException("对称加密数据长度有误（" + encryptedString + "）")
            End If
            Dim [string] As String
            Using sha As SHA256 = SHA256.Create()
                Using hmacsha As HMACSHA256 = New HMACSHA256(sha.ComputeHash(Encoding.UTF8.GetBytes("AES-HMAC|" + key)))
                    Dim array2 As Byte() = hmacsha.ComputeHash(array, 0, 17 + num)
                    Dim num2 As Integer = 17 + num
                    Dim num3 As Integer = 0
                    For i As Integer = 0 To 4 - 1
                        num3 = num3 Or CInt((array(num2 + i) Xor array2(i)))
                    Next
                    If num3 <> 0 Then
                        Throw New CryptographicException("对称加密数据校验失败（" + encryptedString + "）")
                    End If
                    Dim array3 As Byte() = sha.ComputeHash(Encoding.UTF8.GetBytes(key))
                    Dim array4 As Byte() = New Byte(15) {}
                    Array.Copy(array, 1, array4, 0, array4.Length)
                    Using aes As Aes = Aes.Create()
                        aes.Mode = CipherMode.CBC
                        aes.Padding = PaddingMode.PKCS7
                        Using cryptoTransform As ICryptoTransform = aes.CreateDecryptor(array3, array4)
                            Dim array5 As Byte() = cryptoTransform.TransformFinalBlock(array, 1 + array4.Length, num)
                            [string] = Encoding.UTF8.GetString(array5)
                        End Using
                    End Using
                End Using
            End Using
            Return [string]
        End Function

        Public Sub EcdsaVerify(sourceString As String, sign As String, publicKey As String)
            Using hashAlgorithm As HashAlgorithm = CryptographyUtils.GetHashAlgorithm(CryptographyUtils.HashMethod.Sha256)
                Dim array As Byte() = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(sourceString))
                Dim zero As IntPtr = IntPtr.Zero
                Dim zero2 As IntPtr = IntPtr.Zero
                Try
                    Dim num As Integer = CryptographyUtils.BCryptOpenAlgorithmProvider(zero, "ECDSA_P256", Nothing, 0)
                    If num < 0 Then
                        Throw New CryptographicException(String.Format("{0} 失败，错误码 {1}", "BCryptOpenAlgorithmProvider", num))
                    End If
                    num = CryptographyUtils.BCryptImportKeyPair(zero, IntPtr.Zero, "ECCPUBLICBLOB", zero2, Convert.FromBase64String(publicKey), 72, 0)
                    If num < 0 Then
                        Throw New CryptographicException(String.Format("{0} 失败，错误码 {1}", "BCryptImportKeyPair", num))
                    End If
                    num = CryptographyUtils.BCryptVerifySignature(zero2, IntPtr.Zero, array, array.Length, Convert.FromBase64String(sign), 64, 0)
                    If num = -1073700864 Then
                        Throw New SecurityException("签名验证失败")
                    End If
                    If num < 0 Then
                        Throw New CryptographicException(String.Format("{0} 失败，错误码 {1}", "BCryptVerifySignature", num))
                    End If
                Finally
                    If zero2 <> IntPtr.Zero Then
                        CryptographyUtils.BCryptDestroyKey(zero2)
                    End If
                    If zero <> IntPtr.Zero Then
                        CryptographyUtils.BCryptCloseAlgorithmProvider(zero, 0)
                    End If
                End Try
            End Using
        End Sub

        Public Function EcdsaSign(sourceString As String, privateKey As String) As String
            Dim text As String
            Using hashAlgorithm As HashAlgorithm = CryptographyUtils.GetHashAlgorithm(CryptographyUtils.HashMethod.Sha256)
                Dim array As Byte() = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(sourceString))
                Dim array2 As Byte() = Convert.FromBase64String(privateKey)
                Dim zero As IntPtr = IntPtr.Zero
                Dim zero2 As IntPtr = IntPtr.Zero
                Try
                    Dim num As Integer = CryptographyUtils.BCryptOpenAlgorithmProvider(zero, "ECDSA_P256", Nothing, 0)
                    If num < 0 Then
                        Throw New CryptographicException(String.Format("{0} 失败，错误码 {1}", "BCryptOpenAlgorithmProvider", num))
                    End If
                    num = CryptographyUtils.BCryptImportKeyPair(zero, IntPtr.Zero, "ECCPRIVATEBLOB", zero2, array2, array2.Length, 0)
                    If num < 0 Then
                        Throw New CryptographicException(String.Format("{0} 失败，错误码 {1}", "BCryptImportKeyPair", num))
                    End If
                    Dim num2 As Integer
                    num = CryptographyUtils.BCryptSignHash(zero2, IntPtr.Zero, array, array.Length, Nothing, 0, num2, 0)
                    If num < 0 Then
                        Throw New CryptographicException(String.Format("{0} 失败，错误码 {1}", "BCryptSignHash", num))
                    End If
                    Dim array3 As Byte() = New Byte(num2 - 1) {}
                    Dim num3 As Integer
                    num = CryptographyUtils.BCryptSignHash(zero2, IntPtr.Zero, array, array.Length, array3, array3.Length, num3, 0)
                    If num < 0 Then
                        Throw New CryptographicException(String.Format("{0} 失败，错误码 {1}", "BCryptSignHash", num))
                    End If
                    If num3 <> array3.Length Then
                        Array.Resize(Of Byte)(array3, num3)
                    End If
                    text = Convert.ToBase64String(array3)
                Finally
                    If zero2 <> IntPtr.Zero Then
                        CryptographyUtils.BCryptDestroyKey(zero2)
                    End If
                    If zero <> IntPtr.Zero Then
                        CryptographyUtils.BCryptCloseAlgorithmProvider(zero, 0)
                    End If
                End Try
            End Using
            Return text
        End Function

        Private Declare Unicode Function BCryptOpenAlgorithmProvider Lib "bcrypt.dll" (<System.Runtime.InteropServices.OutAttribute()> ByRef algorithmHandle As IntPtr, algorithmId As String, <Nullable(2)> implementation As String, flags As Integer) As Integer

        Private Declare Unicode Function BCryptImportKeyPair Lib "bcrypt.dll" (algorithmHandle As IntPtr, importKey As IntPtr, blobType As String, <System.Runtime.InteropServices.OutAttribute()> ByRef keyHandle As IntPtr, input As Byte(), inputSize As Integer, flags As Integer) As Integer

        Private Declare Function BCryptVerifySignature Lib "bcrypt.dll" (keyHandle As IntPtr, paddingInfo As IntPtr, hash As Byte(), hashSize As Integer, signature As Byte(), signatureSize As Integer, flags As Integer) As Integer

        Private Declare Function BCryptDestroyKey Lib "bcrypt.dll" (keyHandle As IntPtr) As Integer

        Private Declare Function BCryptCloseAlgorithmProvider Lib "bcrypt.dll" (algorithmHandle As IntPtr, flags As Integer) As Integer

        Private Declare Function BCryptSignHash Lib "bcrypt.dll" (keyHandle As IntPtr, paddingInfo As IntPtr, hash As Byte(), hashSize As Integer, <Nullable(2)> signature As Byte(), signatureSize As Integer, <System.Runtime.InteropServices.OutAttribute()> ByRef resultSize As Integer, flags As Integer) As Integer

        Private Const AesDataVersion As Byte = 1

        Private Const AesIvSize As Integer = 16

        Private Const AesCheckSize As Integer = 4

        <NullableContext(0)>
        Public Enum HashMethod
            Md5
            Sha1
            Sha256
            Sha512
        End Enum
    End Module
End Namespace
