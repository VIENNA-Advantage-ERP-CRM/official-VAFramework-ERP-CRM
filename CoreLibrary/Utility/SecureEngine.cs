using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Logging;
using System.Security.Cryptography;
using System.IO;

namespace VAdvantage.Utility
{
    public class SecureEngine
    //internal class SecureEngine
    {
        /// <summary>
        /// Encrypt the text
        /// </summary>
        /// <param name="value">Value to be encrypted</param>
        /// <returns>Encrypted Value</returns>
        public static String Encrypt(string value)
        {
            return  SecureEngineUtility.SecureEngine.Encrypt(value);
        }	//	encrypt
        /// <summary>
        /// Encrypt the text
        /// </summary>
        /// <param name="value">Value to be encrypted</param>
        /// <returns>Encrypted Value</returns>
        public static byte[] Encrypt(char[] value)
        {
            return SecureEngineUtility.SecureEngine.Encrypt(value);
        }	//	encrypt

        /// <summary>
        /// Decrypt the Text
        /// </summary>
        /// <param name="value">Value to be decrypted</param>
        /// <returns>Decrypted Text</returns>
        public static String Decrypt(String value)
        {
            return SecureEngineUtility.SecureEngine.Decrypt(value);
        }	//	decrypt



        /// <summary>
        /// Decrypt the Text
        /// </summary>
        /// <param name="value">Value to be decrypted</param>
        /// <returns>Decrypted Text</returns>
        public static byte[] Decrypt(byte[] value)
        {
            return SecureEngineUtility.SecureEngine.Decrypt(value);
        }	//	decrypt

        /// <summary>
        /// 	Encryption.
        /// The methods must recognize clear values
        /// </summary>
        /// <param name="value">@param value clear value</param>
        /// <returns>   @return encrypted String</returns>
        public static Object Encrypt(Object value)
        {
            return SecureEngineUtility.SecureEngine.Encrypt(value);
        }	//	encrypt

        /// <summary>
        ///	Decryption.
        /// the methods must recognize clear values
        /// </summary>
        /// <param name="value">value encrypted value</param>
        /// <returns>Decrypted value</returns>
        public static Object Decrypt(Object value)
        {
            return SecureEngineUtility.SecureEngine.Decrypt(value);
        }	//	decrypt

        public static bool IsEncrypted(String value)
        {
            return SecureEngineUtility.SecureEngine.IsEncrypted(value);
        }	//	isEncrypted
    

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <returns>
        /// returns Fullpath of file
        /// </returns>
        public static byte[] EncryptFile(byte[] inputFile, string password)
        {
            return SecureEngineUtility.SecureEngine.EncryptFile(inputFile, password);
        }

        public static byte[] DecryptFile(byte[] inputFile, string password)
        {
            return SecureEngineUtility.SecureEngine.DecryptFile(inputFile, password);
        }



        public static bool EncryptFile(string inputFilePath, string password, string outputFileName)
        {
            return SecureEngineUtility.SecureEngine.EncryptFile(inputFilePath, password, outputFileName);
        }



        public static bool DecryptFile(string inputFilePath, string password, string outputFileName)
        {
            return SecureEngineUtility.SecureEngine.DecryptFile(inputFilePath, password, outputFileName);
        }


        public static string GetClassName()
        {
            return SecureEngineUtility.SecureEngine.GetClassName();
        }
    }
}
