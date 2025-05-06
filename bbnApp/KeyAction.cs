using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp
{
    public class KeyAction
    {
        /// <summary>
        /// sm2解密
        /// </summary>
        /// <param name="value"></param>
        public static void Sm2Encrypt(string value)
        {
            string plaintext = bbnApp.Share.EncodeAndDecode.SM2Encrypt(value);

            Console.WriteLine(plaintext);
        }
        /// <summary>
        /// sm2加密
        /// </summary>
        /// <param name="value"></param>
        public static void Sm2Decrypt(string value)
        {
            string plaintext = bbnApp.Share.EncodeAndDecode.SM2Decrypt(value);

            Console.WriteLine(plaintext);
        }
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value"></param>
        public static void AESEncrypt(string value)
        {
            string plaintext = bbnApp.Share.EncodeAndDecode.AesEncrypt(value);

            Console.WriteLine(plaintext);
        }
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value"></param>
        public static void AESDecrypt(string value)
        {
            string plaintext = bbnApp.Share.EncodeAndDecode.AesDecrypt(value);

            Console.WriteLine(plaintext);
        }
        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="value"></param>
        public static void RSAEncrypt(string value)
        {
            string plaintext = bbnApp.Share.EncodeAndDecode.RsaEncrypt(value);

            Console.WriteLine(plaintext);
        }
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="value"></param>
        public static void RSADecrypt(string value)
        {
            string plaintext = bbnApp.Share.EncodeAndDecode.RsaDecrypt(value);

            Console.WriteLine(plaintext);
        }
        /// <summary>
        /// 混合解密
        /// </summary>
        /// <param name="value"></param>
        public static void MixEncrypt(string value)
        {
            string plaintext = bbnApp.Share.EncodeAndDecode.MixEncrypt(value);

            Console.WriteLine(plaintext);
        }
        /// <summary>
        /// 混合加密
        /// </summary>
        /// <param name="value"></param>
        public static void MixDecrypt(string value)
        {
            string plaintext = bbnApp.Share.EncodeAndDecode.MixDecrypt(value);

            Console.WriteLine(plaintext);
        }
    }
}
