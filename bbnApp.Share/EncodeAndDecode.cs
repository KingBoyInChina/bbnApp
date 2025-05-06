using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using System.Security.Cryptography;
using System.Text;

namespace bbnApp.Share
{
    /// <summary>
    /// 加密和解密
    /// </summary>
    public class EncodeAndDecode
    {
        #region 系统初始化密钥
        /// <summary>
        /// 系统默认AESKEY
        /// </summary>
        protected static readonly AesObj aesKey=new AesObj{AESKey= "SZlBkINHFUEo8SdxatBU4jQCmkvopdyh", AESIv= "HNnOGA9X1nvwTZFO" };
        /// <summary>
        /// 系统默认RSAKEY
        /// </summary>
        protected static readonly KeyObj rsaKey = new KeyObj {PublicKey= "-----BEGIN PUBLIC KEY-----\r\nMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA0J9VDPnfKWv8i56bviqM\r\nkA16Oc8/a5rBoFeo9d91pEHambD5lw40QQLuT6L9zG+D6CWlBRopGVTBuwl4HLFK\r\n/KzIrrg1LkeVpyz3RZ8LmKn+dVWRNy2aKhpT0wYtPN+Mz/+SK4eXkj54+uZQb2wb\r\nUNf/Qxh/0lp0SyjFCXfl27FDkXuAJxS0Jhh2xMc8ScVGaKeom2tr33Wg7a+p87qN\r\ntfGjj9JYMSM6tIMKkROaii08ETUgzudKWAevuE/vbE1X976o6LMEGDmvI0y6QKoE\r\n9Xeh3wlwwWzirI1IgJlUdHxSqLVO6A2xsKz5zfJsSwHbSPKInFDoXghZx+9m0HJ9\r\nQQIDAQAB\r\n-----END PUBLIC KEY-----",PrivateKey= "-----BEGIN PRIVATE KEY-----\r\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDQn1UM+d8pa/yL\r\nnpu+KoyQDXo5zz9rmsGgV6j133WkQdqZsPmXDjRBAu5Pov3Mb4PoJaUFGikZVMG7\r\nCXgcsUr8rMiuuDUuR5WnLPdFnwuYqf51VZE3LZoqGlPTBi0834zP/5Irh5eSPnj6\r\n5lBvbBtQ1/9DGH/SWnRLKMUJd+XbsUORe4AnFLQmGHbExzxJxUZop6iba2vfdaDt\r\nr6nzuo218aOP0lgxIzq0gwqRE5qKLTwRNSDO50pYB6+4T+9sTVf3vqjoswQYOa8j\r\nTLpAqgT1d6HfCXDBbOKsjUiAmVR0fFKotU7oDbGwrPnN8mxLAdtI8oicUOheCFnH\r\n72bQcn1BAgMBAAECggEAXrpaZh/3gtlzvPnyAfFUDfzwqNtAEt4cWXA0WiBzTHhS\r\niELhsgNTGsn359U8pN1l6b1eJujwDH590S3FilG+mOjj3uHp0+/RNi2mwcgr/dAC\r\nqMiHvaRJQiDeOw2s8N0ZFUEY4Acbfd48FMELeOx/or/ROmgfciZ8c7w5a8kzaANN\r\ni7QAAW2rDCy1k3Vc2WO83vpQp0nmBPX/T+aqoQsSn/DOFEQ1Z9nreYL3sUKYMCeG\r\naD8fX1kxkhZ46zJjggB9AzgWaWWt1EWsLlvMGGhlo44RMo1UUjZC5RFCHKeMbqUH\r\nkpw0hiM2nRo8THnar3WmIj55jPqV91B/PqMZeHSHeQKBgQDfftgal/2LO+HT8jMb\r\n3juMheDkxObnv8wwixWHV/AWATE6QJWTc7ikRqRL2oK+K8LrL63d9Zu7bEHm24GX\r\nYqeePPzYNY18e9gUwyA1YdDxT9Q16H5MsBT2fn0I9Mqo4ro+t4gPTf4BfY9zU8dS\r\nfTEZqJXNytT3fBdVjCVgX575JwKBgQDu9rwkAtWaWhF1CZOOZBDWnOCt+SXBIHhr\r\nFjXFbSHbYsFbhMulPHeeMZh3r+M+y3dItdkGVUo+JcWWMf8Hft5BAFf81hKpeLxg\r\nCB3Wv9+3t/temzZhGXWcE7zgrQ0ZdX6CnFarX7PfnX1HYPGf5FoKzVlpLt/LFULo\r\n4mL7El5HVwKBgQDFwjFqZq788sHCjv9WW3CJyTTUeL5wti/xjEBCgDd64Rc0Gk3A\r\nKZdqFO/wBqvFpmdVP90zF87zKrmtkvG6iJsU05ZPoiNN0S/EP1xSn/kIbcCy6sRH\r\nC0+hRQ9SS3i/s89lC3UjRbnKi9XREILApPI7aAcWD7IeQvlKwzWXb0T15QKBgQCp\r\nqr5Cw1BuW2zxsG74NsxN1O0iGThZO3jEb+yV1LqpTiEDnfIJzDkSNRyQPm4W89gp\r\n5BPHj187aQ41aiItbELZ3Cic+FIfyf3WIW0uQOXTR+pObd3aa3056Dm/PJ+EaAv/\r\nKInQ/A52wxdkSszEPhpAeTT2nbWAOARd3f9xTMVr2QKBgQDHUtI8a1Nx2tBYxMMr\r\nIHk8KPbJmrw1xmmsunyuL8j0sQa3hZlQXwsfDFbzQnaHhKqXQONb5I/oqRlrXRlu\r\nIwd635FReKf5DfL/EXmLIov/b6qTdOg5+tH5bSampdiw/pvo91STJEIHQQWOZv51\r\nqIoO/0ivyWjffSV/Sy294v3thg==\r\n-----END PRIVATE KEY-----" };
        /// <summary>
        /// 系统默认sm2密钥
        /// </summary>
        protected static readonly KeyObj sm2Key = new KeyObj {PublicKey= "BI5voh2vFE/AW/Eh8lZc3IdOP6RsiS5KKeIPSbIXHVYNc5rHGKw9xMZ/0uN2B8xj+nOK8uT3xOOBspotijEiQ3g=", PrivateKey= "z0G7csGhsjH3M53hmsE/owfKZPcRgZ/M81I+yyQK2GM=" };
        #endregion
        #region Base64加解密
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(string plainText)
        {
            try
            {

                // 将字符串转换为字节数组
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

                // 将字节数组转换为 Base64 字符串
                return Convert.ToBase64String(plainTextBytes);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <returns></returns>
        public static string Base64Decode(string base64EncodedData)
        {
            try
            {

                // 将 Base64 字符串转换为字节数组
                byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);

                // 将字节数组转换为字符串
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region AES加解密
        /// <summary>
        /// 获取AES密钥和向量
        /// </summary>
        /// <returns></returns>
        public static AesObj GetAesKey() {
            try
            {

                AesObj Obj = new AesObj
                {
                    AESKey =CommMethod.GetStrRandom(32),
                    AESIv = CommMethod.GetStrRandom(16)
                };
                return Obj;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string AesEncrypt(string plainText, string key, string iv)
        {
            try
            {
                // 检查密钥和 IV 的长度
                if (string.IsNullOrEmpty(key) || key.Length != 32)
                    throw new ArgumentException("密钥必须是 32 个字符（256 位）。");
                if (string.IsNullOrEmpty(iv) || iv.Length != 16)
                    throw new ArgumentException("IV 必须是 16 个字符（128 位）。");

                byte[] encrypted;

                // 创建 AES 实例
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = Encoding.UTF8.GetBytes(iv);

                    // 创建加密器
                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    // 加密数据
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(cs))
                            {
                                sw.Write(plainText);
                            }
                            encrypted = ms.ToArray();
                        }
                    }
                }

                // 返回 Base64 编码的加密结果
                return Convert.ToBase64String(encrypted);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string AesEncrypt(string plainText)
        {
            try
            {
                byte[] encrypted;

                // 创建 AES 实例
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(aesKey.AESKey);
                    aes.IV = Encoding.UTF8.GetBytes(aesKey.AESIv);

                    // 创建加密器
                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    // 加密数据
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(cs))
                            {
                                sw.Write(plainText);
                            }
                            encrypted = ms.ToArray();
                        }
                    }
                }

                // 返回 Base64 编码的加密结果
                return Convert.ToBase64String(encrypted);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string AesDecrypt(string cipherText, string key, string iv)
        {
            try
            {

                // 检查密钥和 IV 的长度
                if (string.IsNullOrEmpty(key) || key.Length != 32)
                    throw new ArgumentException("密钥必须是 32 个字符（256 位）。");
                if (string.IsNullOrEmpty(iv) || iv.Length != 16)
                    throw new ArgumentException("IV 必须是 16 个字符（128 位）。");

                string plainText;

                // 创建 AES 实例
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = Encoding.UTF8.GetBytes(iv);

                    // 创建解密器
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    // 解密数据
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                plainText = sr.ReadToEnd();
                            }
                        }
                    }
                }

                return plainText;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string AesDecrypt(string cipherText)
        {
            try
            {
                string plainText;

                // 创建 AES 实例
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(aesKey.AESKey);
                    aes.IV = Encoding.UTF8.GetBytes(aesKey.AESIv);

                    // 创建解密器
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    // 解密数据
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                plainText = sr.ReadToEnd();
                            }
                        }
                    }
                }

                return plainText;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region RSA加解密
        /// <summary>
        /// 获取RSA密钥
        /// </summary>
        /// <returns></returns>
        public static KeyObj GetRsaKey(int keySize=2048)
        {
            try
            {
                // 创建 RSA 实例
                using (RSA rsa = RSA.Create())
                {
                    rsa.KeySize = keySize;
                    // 导出公钥和私钥
                    string publicKey = rsa.ExportSubjectPublicKeyInfoPem();
                    string privateKey = rsa.ExportPkcs8PrivateKeyPem();
                    KeyObj key = new KeyObj
                    {
                        PublicKey = publicKey,
                        PrivateKey = privateKey
                    };
                    return key;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// RSA 加密方法
        /// </summary>
        /// <param name="plainText">文本</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        public static string RsaEncrypt(string plainText, string publicKey)
        {
            try
            {
                using (RSA rsa = RSA.Create())
                {
                    rsa.ImportFromPem(publicKey);
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.OaepSHA256);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="cipherText">文本</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public static string RsaDecrypt(string cipherText, string privateKey)
        {
            try
            {
                // 创建 RSA 实例
                using (RSA rsa = RSA.Create())
                {
                    // 导入私钥
                    rsa.ImportFromPem(privateKey);

                    // 将 Base64 字符串转换为字节数组
                    byte[] encryptedBytes = Convert.FromBase64String(cipherText);

                    // 使用私钥解密
                    byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);

                    // 将解密后的字节数组转换为字符串
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string RsaEncrypt(string plainText)
        {
            try
            {
                using (RSA rsa = RSA.Create())
                {
                    rsa.ImportFromPem(rsaKey.PublicKey);
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.OaepSHA256);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string RsaDecrypt(string cipherText)
        {
            try
            {
                // 创建 RSA 实例
                using (RSA rsa = RSA.Create())
                {
                    // 导入私钥
                    rsa.ImportFromPem(rsaKey.PrivateKey);

                    // 将 Base64 字符串转换为字节数组
                    byte[] encryptedBytes = Convert.FromBase64String(cipherText);

                    // 使用私钥解密
                    byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);

                    // 将解密后的字节数组转换为字符串
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region RSA+AES加解密
        /// <summary>
        /// RSA_AES混合加密
        /// 文本通过AES加密，AES密钥和向量通过RSA加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="publicKey"></param>
        /// <param name="AesKey"></param>
        /// <param name="AesIv"></param>
        /// <returns></returns>
        public static MixEnObj MixEncrypt(string plainText, string publicKey,string AesKey,string AesIv)
        {
            try
            {
                string AesEnKey = RsaEncrypt(AesKey, publicKey);
                string AesEnIv = RsaEncrypt(AesIv, publicKey);
                string plainEnText = AesEncrypt(plainText,AesKey,AesIv);

                MixEnObj Obj = new MixEnObj
                {
                    AESKey = AesEnKey,
                    AESIv= AesEnIv,
                    encryptedData = plainEnText
                };
                return Obj;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// RSA_AES混合加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string MixEncrypt(string plainText)
        {
            try
            {
                string AesEnKey = RsaEncrypt(aesKey.AESKey, rsaKey.PublicKey);
                string AesEnIv = RsaEncrypt(aesKey.AESIv, rsaKey.PublicKey);
                string plainEnText = AesEncrypt(plainText, aesKey.AESKey, aesKey.AESIv);

                return plainEnText;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// RSA+AES混合解密
        /// 先将AES密钥和向量进行解密，再将文本通过Aes进行解密
        /// </summary>
        /// <param name="cipherText">加密的文本内容</param>
        /// <param name="privatekey">RSA私钥</param>
        /// <param name="AesEnKey">加密的AES密钥</param>
        /// <param name="AesEnIv">加密的AES向量</param>
        /// <returns></returns>
        public static string MixDecrypt(string cipherText,string privatekey,string AesEnKey,string AesEnIv)
        {
            try
            {
                string AesKey = AesEnKey;
                string AesIv = AesEnIv;
                string plainText= AesDecrypt(cipherText, AesKey,AesIv);
                return plainText;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// RSA+AES混合解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string MixDecrypt(string cipherText)
        {
            try
            {
                string AesKey = aesKey.AESKey;
                string AesIv = aesKey.AESIv;
                string plainText = AesDecrypt(cipherText, AesKey, AesIv);
                return plainText;
            }
            catch (Exception ex)
            {
                throw ;
            }
        }
        #endregion
        #region MD5加密
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Md5Encrypt(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
        #endregion
        #region SM2加解密
        /// <summary>
        ///  SM2 公钥加密
        /// </summary>
        /// <param name="plainText">待加密字符串</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        public static string SM2Encrypt(string plainText, string publicKey)
        {
            // 获取 SM2 曲线参数
            X9ECParameters curve = ECNamedCurveTable.GetByName("sm2p256v1");

            Org.BouncyCastle.Math.EC.ECPoint q = curve.Curve.DecodePoint(Base64.Decode(publicKey));
            ECDomainParameters domain = new ECDomainParameters(curve);
            ECPublicKeyParameters pubk = new ECPublicKeyParameters("EC", q, domain);

            // 创建SM2加密器
            SM2Engine sm2Engine = new SM2Engine();
            sm2Engine.Init(true, new ParametersWithRandom(pubk, new SecureRandom()));

            // 将原始数据转换为字节数组
            byte[] dataBytes = Encoding.UTF8.GetBytes(plainText);

            // 执行加密操作
            byte[] encryptedData = sm2Engine.ProcessBlock(dataBytes, 0, dataBytes.Length);

            // 将加密结果转换为 Base64 字符串
            return Base64.ToBase64String(encryptedData);
        }

        /// <summary>
        /// SM2 私钥解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string SM2Decrypt(string cipherText, string privateKey)
        {
            // 获取 SM2 曲线参数
            X9ECParameters curve = ECNamedCurveTable.GetByName("sm2p256v1");

            ECDomainParameters domain = new ECDomainParameters(curve);
            BigInteger d = new BigInteger(1, Base64.Decode(privateKey));
            ECPrivateKeyParameters prik = new ECPrivateKeyParameters(d, domain);

            // 创建SM2加密器
            SM2Engine sm2Engine = new SM2Engine();
            sm2Engine.Init(false, prik);

            byte[] encryptedData = Base64.Decode(cipherText);

            // 执行解密操作
            byte[] decryptedData = sm2Engine.ProcessBlock(encryptedData, 0, encryptedData.Length);

            // 将解密结果转换为字符串
            return Encoding.UTF8.GetString(decryptedData);
        }

        /// <summary>
        ///  SM2 公钥加密
        /// </summary>
        /// <param name="plainText">待加密字符串</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        public static string SM2Encrypt(string plainText)
        {
            // 获取 SM2 曲线参数
            X9ECParameters curve = ECNamedCurveTable.GetByName("sm2p256v1");

            Org.BouncyCastle.Math.EC.ECPoint q = curve.Curve.DecodePoint(Base64.Decode(sm2Key.PublicKey));
            ECDomainParameters domain = new ECDomainParameters(curve);
            ECPublicKeyParameters pubk = new ECPublicKeyParameters("EC", q, domain);

            // 创建SM2加密器
            SM2Engine sm2Engine = new SM2Engine();
            sm2Engine.Init(true, new ParametersWithRandom(pubk, new SecureRandom()));

            // 将原始数据转换为字节数组
            byte[] dataBytes = Encoding.UTF8.GetBytes(plainText);

            // 执行加密操作
            byte[] encryptedData = sm2Engine.ProcessBlock(dataBytes, 0, dataBytes.Length);

            // 将加密结果转换为 Base64 字符串
            return Base64.ToBase64String(encryptedData);
        }

        /// <summary>
        /// SM2 私钥解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string SM2Decrypt(string cipherText)
        {
            // 获取 SM2 曲线参数
            X9ECParameters curve = ECNamedCurveTable.GetByName("sm2p256v1");

            ECDomainParameters domain = new ECDomainParameters(curve);
            BigInteger d = new BigInteger(1, Base64.Decode(sm2Key.PrivateKey));
            ECPrivateKeyParameters prik = new ECPrivateKeyParameters(d, domain);

            // 创建SM2加密器
            SM2Engine sm2Engine = new SM2Engine();
            sm2Engine.Init(false, prik);

            byte[] encryptedData = Base64.Decode(cipherText);

            // 执行解密操作
            byte[] decryptedData = sm2Engine.ProcessBlock(encryptedData, 0, encryptedData.Length);

            // 将解密结果转换为字符串
            return Encoding.UTF8.GetString(decryptedData);
        }

        /// <summary>
        /// 生成 SM2 密钥对，密钥对使用 Base64 进行编码
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="publicKey"></param>
        public static KeyObj GenerateSM2KeyPair()
        {
            // 获取 SM2 曲线参数
            X9ECParameters curve = ECNamedCurveTable.GetByName("sm2p256v1");
            KeyGenerationParameters parameters = new ECKeyGenerationParameters(new ECDomainParameters(curve), new SecureRandom());

            // 创建 SM2 密钥对生成器
            ECKeyPairGenerator generator = new ECKeyPairGenerator();
            generator.Init(parameters);

            // 创建密钥对
            var keyPair = generator.GenerateKeyPair();

            // 私钥
            ECPrivateKeyParameters privateKeyParameters = (ECPrivateKeyParameters)keyPair.Private;
            string privateKey = Base64.ToBase64String(privateKeyParameters.D.ToByteArrayUnsigned());

            // 公钥
            ECPublicKeyParameters publicKeyParameters = (ECPublicKeyParameters)keyPair.Public;
            string publicKey = Base64.ToBase64String(publicKeyParameters.Q.GetEncoded());
            KeyObj keyObj = new KeyObj
            {
                PublicKey = publicKey,
                PrivateKey = privateKey,
            };
            return keyObj;
        }

        #endregion
    }

    /// <summary>
    /// AES密钥和向量
    /// </summary>
    public class AesObj
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public string AESKey { get; set; }
        /// <summary>
        /// 向量
        /// </summary>
        public string AESIv { get; set; }
    }
    /// <summary>
    /// 密钥对
    /// </summary>
    public class KeyObj
    {
        /// <summary>
        /// 公钥
        /// </summary>
        public string PublicKey { get; set; }
        /// <summary>
        /// 私钥
        /// </summary>
        public string PrivateKey { get; set; }
    }
    /// <summary>
    /// RSA+AES混合加密结果
    /// </summary>
    public class MixEnObj
    {
        /// <summary>
        /// 加密后的AES密钥
        /// </summary>
        public string AESKey { get; set; }
        /// <summary>
        /// 加密后的AES向量
        /// </summary>
        public string AESIv { get; set; }
        /// <summary>
        /// AES加密后的内容
        /// </summary>
        public string encryptedData { get; set; }
    }
    /// <summary>
    /// SM2密钥
    /// </summary>
    public class Sm2Keys
    {
        /// <summary>
        /// 公钥
        /// </summary>
        public ECPublicKeyParameters PublicKey { get; set; }
        /// <summary>
        /// 私钥
        /// </summary>
        public ECPrivateKeyParameters PrivateKey { get; set; }
    }

}
