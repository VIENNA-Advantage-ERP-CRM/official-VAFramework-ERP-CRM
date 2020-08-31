using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using VAdvantage.Logging;

namespace MarketSvc
{
    internal class SecureE :   SecureInterfaceE
    //internal class Secure : SecureInterface
    {
	    /** Default Class Name implementing SecureInterface	*/
        public static String VIENNA_SECURE_DEFAULT = "WCFServiceImplementation.SecureE";

	    /** Clear Text Indicator xyz	*/
        public static String CLEARVALUE_START = "xyz";
	    /** Clear Text Indicator		*/
        public static String CLEARVALUE_END = "";
	    /** Encrypted Text Indicator ~	*/
        public static String ENCRYPTEDVALUE_START = "~";
	    /** Encrypted Text Indicator ~	*/
        public static String ENCRYPTEDVALUE_END = "~";

        	/**	Logger						*/
        private static Logger log = Logger.GetLogger(typeof(SecureE).FullName);

        /// <summary>
        /// Encrypt the text
        /// </summary>
        /// <param name="value">Value to be encrypted</param>
        /// <returns>Encrypted Value</returns>
        public string Encrypt(string value)
        {
            // Declare the streams used
            // to encrypt to an in memory
            // array of bytes.
            MemoryStream msEncrypt = null;
            CryptoStream csEncrypt = null;
            StreamWriter swEncrypt = null;

            // Declare the RijndaelManaged object
            // used to encrypt the data.
            RijndaelManaged aesAlg = null;

            try
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = SecureKeyE.key;
                aesAlg.IV = SecureKeyE.vector;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                msEncrypt = new MemoryStream();
                csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                swEncrypt = new StreamWriter(csEncrypt);

                //Write all data to the stream.
                swEncrypt.Write(value);

            }
            finally
            {
                // Clean things up.

                // Close the streams.
                if (swEncrypt != null)
                    swEncrypt.Close();
                if (csEncrypt != null)
                    csEncrypt.Close();
                if (msEncrypt != null)
                    msEncrypt.Close();

                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            byte[] bFinal =  msEncrypt.ToArray();
            string encString = ByteArrayToString(bFinal);
            log.Log(Level.ALL, value + " => " + encString);
            return encString;

        }



        /// <summary>
        /// Decrypt the Text
        /// </summary>
        /// <param name="value">Value to be decrypted</param>
        /// <returns>Decrypted Value</returns>
        public string Decrypt(string value)
        {
            byte[] cipherText = StringToByteArray(value);
            // TDeclare the streams used
            // to decrypt to an in memory
            // array of bytes.
            MemoryStream msDecrypt = null;
            CryptoStream csDecrypt = null;
            StreamReader srDecrypt = null;

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = SecureKeyE.key;
                aesAlg.IV = SecureKeyE.vector;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                msDecrypt = new MemoryStream(cipherText);
                csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                srDecrypt = new StreamReader(csDecrypt);

                // Read the decrypted bytes from the decrypting stream
                // and place them in a string.
                plaintext = srDecrypt.ReadToEnd();
            }
            finally
            {
                // Clean things up.

                // Close the streams.
                if (srDecrypt != null)
                    srDecrypt.Close();
                if (csDecrypt != null)
                    csDecrypt.Close();
                if (msDecrypt != null)
                    msDecrypt.Close();

                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            log.Log(Level.ALL, value + " => " + plaintext);
            return plaintext;

        }

        /// <summary>
        /// Decrypt the Text
        /// </summary>
        /// <param name="value">Value to be decrypted</param>
        /// <returns>Decrypted Value</returns>
        public int Encrypt(int value)
        {
            return value;
        }

        /// <summary>
        /// Decrypt the Text
        /// </summary>
        /// <param name="value">Value to be decrypted</param>
        /// <returns>Decrypted Value</returns>
        public int Decrypt(int value)
        {
            return value;
        }

        /// <summary>
        /// Decrypt the Text
        /// </summary>
        /// <param name="value">Value to be decrypted</param>
        /// <returns>Decrypted Value</returns>
        public Decimal Encrypt(Decimal value)
        {
            return value;
        }

        /// <summary>
        /// Decrypt the Text
        /// </summary>
        /// <param name="value">Value to be decrypted</param>
        /// <returns>Decrypted Value</returns>
        public Decimal Decrypt(Decimal value)
        {
            return value;
        }

        /// <summary>
        /// Decrypt the Text
        /// </summary>
        /// <param name="value">Value to be decrypted</param>
        /// <returns>Decrypted Value</returns>
        public DateTime Encrypt(DateTime value)
        {
            return value;
        }

        /// <summary>
        /// Decrypt the Text
        /// </summary>
        /// <param name="value">Value to be decrypted</param>
        /// <returns>Decrypted Value</returns>
        public DateTime Decrypt(DateTime value)
        {
            return value;
        }

        /// <summary>
        /// Convert ByteArray to String
        /// </summary>
        /// <param name="ba">byteArray</param>
        /// <returns>Hexed String</returns>
        private string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        /// <summary>
        /// Convert to HexString to Byte Array
        /// </summary>
        /// <param name="hex">Hexed String</param>
        /// <returns>Byte Array</returns>
        private byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }


        /// <summary>
        /// Is Encrypted
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>true, if value is an encrypted string</returns>
        public bool IsEncrypted(String value)
        {
            try
            {
                byte[] b = StringToByteArray(value);
                return true;
            }
            catch(Exception ex)
            {
                log.Finer("Failed: " + value + " - " + ex.Message);
            }
            return false;
        }


        public static String ConvertToHexString(byte[] bytes)
        {
            //	see also Utility.toHex
            int size = bytes.Length;
            StringBuilder buffer = new StringBuilder(size * 2);
            for (int i = 0; i < size; i++)
            {
                // convert byte to an int
                int x = bytes[i];
                // account for int being a signed type and byte being unsigned
                if (x < 0)
                    x += 256;
                String tmp = x.ToString("x");
                // pad out "1" to "01" etc.
                if (tmp.Length == 1)
                    buffer.Append("0");
                buffer.Append(tmp);
            }
            return buffer.ToString();
        }   //  convertToHexString

        public static byte[] ConvertHexString(String hexString)
        {
            if (hexString == null || hexString.Length == 0)
                return null;
            int size = hexString.Length / 2;
            byte[] retValue = new byte[size];
            String inString = hexString.ToLower();

            try
            {
                for (int i = 0; i < size; i++)
                {
                    int index = i * 2;
                    int ii = int.Parse(inString.Substring(index, 2), System.Globalization.NumberStyles.HexNumber);
                    retValue[i] = (byte)ii;
                }
                return retValue;
            }
            catch 
            {
            }
            return null;
        }   //  convertToHexString
    }
}
