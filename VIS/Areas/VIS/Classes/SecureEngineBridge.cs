using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Hosting;
using VAdvantage.Model;
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
        internal static string GetRandomKey()
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


    public class CommonFunctions
    {
        /// <summary>
        /// Save Image into data base/folder
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="buffer">image Byte array</param>
        /// <param name="imageID">image ID for the file</param>
        /// <param name="imageName">name of the i,age</param>
        /// <param name="isSaveInDB">is want to save into the data base</param>
        /// <returns></returns>
        public static int SaveImage(Ctx ctx, byte[] buffer, int imageID, string imageName, bool isSaveInDB)
        {
            MImage mimg = new MImage(ctx, imageID, null);
            mimg.ByteArray = buffer;
            mimg.ImageFormat = imageName.Substring(imageName.LastIndexOf('.'));
            mimg.SetName(imageName);
            if (isSaveInDB)
            {
                mimg.SetBinaryData(buffer);
                //mimg.SetImageURL(string.Empty);
            }
            else
            {
                // if user uncheck the save in db checkbox
                mimg.SetBinaryData(null);
            }
            mimg.SetImageURL("Images");
            //else
            //{
            //    mimg.SetImageURL("Images");//Image Saved in File System so instead of byteArray image Url will be set
            //    mimg.SetBinaryData(new byte[0]);
            //}
            if (!mimg.Save())
            {
                return 0;
            }
            return mimg.Get_ID();
        }

        /// <summary>
        /// Delete image
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="ad_image_id"></param>
        /// <returns></returns>
        public static int DeleteImage(Ctx ctx, int ad_image_id)
        {
            MImage mimg = new MImage(ctx, ad_image_id, null);
            if (mimg.Delete(true))
                return 1;
            else
                return 0;
        }


        public static int SaveUserImage(Ctx ctx, byte[] buffer, string imageName, bool isSaveInDB, int userID)
        {

            MUser user = new MUser(ctx, userID, null);
            int imageID = Util.GetValueOfInt(user.GetAD_Image_ID());

            MImage mimg = new MImage(ctx, imageID, null);
            mimg.ByteArray = buffer;
            mimg.ImageFormat = imageName.Substring(imageName.LastIndexOf('.'));
            mimg.SetName(imageName);
            if (isSaveInDB)
            {
                mimg.SetBinaryData(buffer);
                //mimg.SetImageURL(string.Empty);
            }
            mimg.SetImageURL("Images/Thumb100x100");
            //else
            //{
            //    //mimg.SetImageURL(HostingEnvironment.MapPath(@"~/Images/100by100"));//Image Saved in File System so instead of byteArray image Url will be set
            //    mimg.SetImageURL("Images/Thumb100x100");//Image Saved in File System so instead of byteArray image Url will be set
            //    mimg.SetBinaryData(new byte[0]);
            //}
            if (!mimg.Save())
            {
                return 0;
            }
            user.SetAD_Image_ID(mimg.GetAD_Image_ID());
            if (!user.Save())
            {
                return 0;
            }

            return mimg.GetAD_Image_ID();
        }

        public static void ConvertByteArrayToThumbnail(byte[] imageBytes, string imageName)
        {

            try
            {
                //********** Save Original Image ********************
                CreateThumbnail(0, 0, imageBytes, imageName);

                //*********Create Thumbnail of 320/240 *******************
                CreateThumbnail(320, 240, imageBytes, imageName);

                //*********Create Thumbnail of 140/120 *******************
                CreateThumbnail(140, 120, imageBytes, imageName);

                //*********Create Thumbnail of 46/46 *******************
                CreateThumbnail(46, 46, imageBytes, imageName);

                //*********Create Thumbnail of 32/32 *******************
                CreateThumbnail(32, 32, imageBytes, imageName);

                //*********Create Thumbnail of 16/16 *******************
                CreateThumbnail(16, 16, imageBytes, imageName);

                // CreateThumbnail(320, 180, imageBytes, imageName);

                //CreateThumbnail(320, 150, imageBytes, imageName);
                CreateThumbnail(320, 185, imageBytes, imageName);
            }
            catch
            {
            }
            finally
            {
            }


        }
        private static void CreateThumbnail(int width, int height, byte[] byteArray, string imageName)
        {
            int dimWidth = width;
            int dimHeight = height;
            MemoryStream ms = null;
            try
            {
                // System.Drawing.Image imThumbnailImage;
                Bitmap imThumbnailImage;
                // System.Drawing.Image OriginalImage;
                ms = new MemoryStream();
                // Stream / Write Image to Memory Stream from the Byte Array.
                ms.Write(byteArray, 0, byteArray.Length);
                Bitmap OriginalImage = (Bitmap)Image.FromStream(ms);
                //OriginalImage = System.Drawing.Image.FromStream(ms);
                if (height == 0 && width == 0)
                {
                    OriginalImage.Save(Path.Combine(HostingEnvironment.MapPath(@"~/Images"), imageName));
                    return;
                }
                if (!Directory.Exists((Path.Combine(HostingEnvironment.MapPath(@"~/Images"), "Thumb" + width.ToString() + "x" + height.ToString()))))
                {
                    Directory.CreateDirectory(Path.Combine(HostingEnvironment.MapPath(@"~/Images"), "Thumb" + width.ToString() + "x" + height.ToString()));       //Create Thumbnail Folder if doesnot exists
                }
                // Shrink the Original Image to a thumbnail size.
                int percenetage = 0;
                string filepath = Path.Combine(HostingEnvironment.MapPath(@"~/Images"), "Thumb" + width.ToString() + "x" + height.ToString() + "/" + imageName);
                if (!(OriginalImage.Height < height && OriginalImage.Width < width))
                {
                    if (OriginalImage.Height > OriginalImage.Width)
                    {
                        percenetage = GetPercentage(OriginalImage.Width, OriginalImage.Height);
                        // height = width*100 / percenetage;

                        width = (height * percenetage) / 100;
                        if (width > dimWidth)
                        {
                            width = dimWidth;
                            height = (width * 100) / percenetage;
                        }
                        // height =height+( width*GetPercentage(OriginalImage.Width, OriginalImage.Height))/100;
                        // width = (width * GetPercentage(OriginalImage.Width, OriginalImage.Height)) / 100;
                    }
                    else if (OriginalImage.Height == OriginalImage.Width)
                    {
                        width = height;
                    }
                    else
                    {
                        percenetage = GetPercentage(OriginalImage.Height, OriginalImage.Width);
                        //  width = height*100 / percenetage;

                        height = (width * percenetage) / 100;
                        if (height > dimHeight)
                        {
                            height = dimHeight;
                            width = (dimHeight * 100) / percenetage;
                        }
                        //width =width+(height*GetPercentage(OriginalImage.Height, OriginalImage.Width))/100;
                        //height = (height * GetPercentage(OriginalImage.Height, OriginalImage.Width)) / 100;
                    }
                    // imThumbnailImage = OriginalImage.GetThumbnailImage(width, height, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                    imThumbnailImage = new Bitmap(OriginalImage, new Size(width, height));
                    imThumbnailImage.Save(filepath);
                }
                else
                {
                    OriginalImage.Save(filepath);
                }


            }
            catch
            {
            }
            finally
            {
                if (ms != null)
                {
                    ms.Close();
                }
            }
            //return myMS.ToArray();
        }
        private static int GetPercentage(int value, int total)
        {
            return (value * 100) / total;
        }
        public static byte[] GetThumbnailByte(int height, int width, string ImageName)
        {
            byte[] fileBytes = null;
            string url = string.Empty;
            if (ImageName != null)
            {
                url = Path.Combine(HostingEnvironment.MapPath(@"~/Images//"), "Thumb" + height.ToString() + "x" + width.ToString() + "\\" + ImageName);

                if (File.Exists(url))
                {
                    FileStream stream = null;
                    try
                    {
                        //string filepath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, dsData.Tables[0].Rows[0]["imageurl"].ToString());
                        if (File.Exists(url))
                        {
                            stream = File.OpenRead(url);
                            fileBytes = new byte[stream.Length];
                            stream.Read(fileBytes, 0, fileBytes.Length);
                            stream.Close();
                            return fileBytes;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                    finally
                    {
                        if (stream != null)
                            stream.Close();
                    }
                }
                else
                {
                    return null;
                }
            }

            return fileBytes;

        }
    }
}