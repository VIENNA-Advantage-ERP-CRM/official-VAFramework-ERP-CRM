using System;

using System.IO;

using System.Security.Cryptography;
using System.Text;

using VAdvantage.Utility;

namespace VIS.Classes
{
    /// <summary>
    /// Adapater class encrypt and Decrypt data from server to client side and vice versa
    /// </summary>
    public class SecureEngineBridge
    {

        /// <summary>
        /// encrypt Clent side encrytion to Server Side Encryption
        /// - first decrypt client side encrypted by client key , then encrypt that value by server key;
        /// </summary>
        /// <param name="value">encrypted value(client)</param>
        /// <param name="key">client key</param>
        /// <returns>encrypted value (server side)</returns>
        public static string EncryptFromClientToServer(string value, string key)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            string val = SecureEngineH5.Decrypt(value, key, key);
            return SecureEngine.Encrypt(val);
        }

        /// <summary>
        /// convert server side encrypted value to client side encrypted value
        /// - first decrypt value by server key , and then encrypt by client's key
        /// </summary>
        /// <param name="value">encrypted value(server)</param>
        /// <param name="key">client key</param>
        /// <returns>encrypted value(client)</returns>
        public static string EncryptFromSeverToClient(string value, string key)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            string val = value;
            if (SecureEngine.IsEncrypted(value))
            {
                val = SecureEngine.Decrypt(value);
            }
            return SecureEngineH5.Encrypt(val, key, key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string EncryptByClientKey(string value, string key)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return SecureEngineH5.Encrypt(value, key, key);
        }

        /// <summary>
        /// Decrypt value by client key
        /// </summary>
        /// <param name="value">encrypted value</param>
        /// <param name="key">decrypted value</param>
        /// <returns></returns>
        public static string DecryptByClientKey(string value, string key)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return SecureEngineH5.Decrypt(value, key, key);
        }

        /// <summary>
        /// Generate sercure key (client side)
        /// </summary>
        /// <returns>string key</returns>
        public static string GetRandomKey()
        {
            var RnNum = new Random().Next(10000000, 99999999);
            var RnNum2 = new Random().Next(10000000, 99999999);
            return RnNum.ToString() + RnNum2.ToString();
        }



        /// <summary>
        /// secure Engine class to encrypt and decrypt Client Side data
        /// </summary>

        //class SecureEngineH5
        //{

        //    public static string Decrypt(string cipherText, string key, string iv)
        //    {
        //        //var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);


        //        // Check arguments.
        //        if (cipherText == null || cipherText.Length <= 0)
        //        {
        //            throw new ArgumentNullException("cipherText");
        //        }
        //        if (key == null)
        //        {
        //            throw new ArgumentNullException("key");
        //        }
        //        if (iv == null)
        //        {
        //            throw new ArgumentNullException("key");
        //        }

        //        var encrypted = Convert.FromBase64String(cipherText);
        //        var keybytes = Encoding.UTF8.GetBytes(key);
        //        var ivbytes = Encoding.UTF8.GetBytes(iv);


        //        // Declare the string used to hold
        //        // the decrypted text.
        //        string plaintext = null;

        //        // Create an RijndaelManaged object
        //        // with the specified key and IV.
        //        using (var rijAlg = new RijndaelManaged())
        //        {
        //            //Settings
        //            rijAlg.Mode = CipherMode.CBC;
        //            rijAlg.Padding = PaddingMode.PKCS7;
        //            rijAlg.FeedbackSize = 128;

        //            rijAlg.Key = keybytes;
        //            rijAlg.IV = ivbytes;

        //            // Create a decrytor to perform the stream transform.
        //            var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

        //            // Create the streams used for decryption.
        //            using (var msDecrypt = new MemoryStream(encrypted))
        //            {
        //                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //                {
        //                    using (var srDecrypt = new StreamReader(csDecrypt))
        //                    {
        //                        // Read the decrypted bytes from the decrypting stream
        //                        // and place them in a string.
        //                        plaintext = srDecrypt.ReadToEnd();
        //                    }
        //                }
        //            }
        //        }

        //        return plaintext;
        //    }

        //    public static String Encrypt(string plainText, string key, string iv)
        //    {
        //        // Check arguments.
        //        if (plainText == null || plainText.Length <= 0)
        //        {
        //            throw new ArgumentNullException("plainText");
        //        }
        //        if (key == null || key.Length <= 0)
        //        {
        //            throw new ArgumentNullException("key");
        //        }
        //        if (iv == null || iv.Length <= 0)
        //        {
        //            throw new ArgumentNullException("key");
        //        }


        //        string enText = "";

        //        byte[] encrypted;
        //        // Create a RijndaelManaged object
        //        // with the specified key and IV.
        //        using (var rijAlg = new RijndaelManaged())
        //        {
        //            rijAlg.Mode = CipherMode.CBC;
        //            rijAlg.Padding = PaddingMode.PKCS7;
        //            rijAlg.FeedbackSize = 128;

        //            rijAlg.Key = Encoding.UTF8.GetBytes(key);
        //            rijAlg.IV = Encoding.UTF8.GetBytes(iv);

        //            // Create a decrytor to perform the stream transform.
        //            var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

        //            // Create the streams used for encryption.
        //            using (var msEncrypt = new MemoryStream())
        //            {
        //                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //                {
        //                    using (var swEncrypt = new StreamWriter(csEncrypt))
        //                    {
        //                        //Write all data to the stream.
        //                        swEncrypt.Write(plainText);
        //                    }
        //                    encrypted = msEncrypt.ToArray();
        //                }
        //            }
        //        }

        //        if (encrypted != null)
        //        {
        //            enText = Convert.ToBase64String(encrypted);
        //        }

        //        // Return the encrypted bytes from the memory stream.
        //        return enText;
        //    }
        //}
    }


    public class SecureEngineH5
    {

        public static string Decrypt(string cipherText, string key, string iv)
        {
            //var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);


            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null)
            {
                throw new ArgumentNullException("key");
            }

            var encrypted = Convert.FromBase64String(cipherText);
            var keybytes = Encoding.UTF8.GetBytes(key);
            var ivbytes = Encoding.UTF8.GetBytes(iv);


            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = keybytes;
                rijAlg.IV = ivbytes;

                // Create a decrytor to perform the stream transform.
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(encrypted))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        public static String Encrypt(string plainText, string key, string iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }


            string enText = "";

            byte[] encrypted;
            // Create a RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = Encoding.UTF8.GetBytes(key);
                rijAlg.IV = Encoding.UTF8.GetBytes(iv);

                // Create a decrytor to perform the stream transform.
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            if (encrypted != null)
            {
                enText = Convert.ToBase64String(encrypted);
            }

            // Return the encrypted bytes from the memory stream.
            return enText;
        }

        public static string Decrypt(string cipherText, Ctx ctx)
        {
            return Decrypt(cipherText, ctx.GetSecureKey(), ctx.GetSecureKey());
        }

        public static string Encrypt(string cipherText, Ctx ctx)
        {
            return Encrypt(cipherText, ctx.GetSecureKey(), ctx.GetSecureKey());
        }

    }


}