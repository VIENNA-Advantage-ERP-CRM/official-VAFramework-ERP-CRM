using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Utility
{
    /// <summary>
    /// Envision Security Inteface
    /// </summary>
    interface SecureInterface
    {
        /// <summary>
        /// Encrypt the text
        /// </summary>
        /// <param name="value">Value to be encrypted</param>
        /// <returns>Encrypted Value</returns>
        String Encrypt(String value);

        /// <summary>
        /// Decrypt the Text
        /// </summary>
        /// <param name="value">Value to be decrypted</param>
        /// <returns>Decrypted Value</returns>
        String Decrypt(String value);

        /// <summary>
        /// Encrypt the text
        /// </summary>
        /// <param name="value">Value to be encrypted</param>
        /// <returns>Encrypted Value</returns>
        int Encrypt(int value);

        /// <summary>
        /// Decrypt the Text
        /// </summary>
        /// <param name="value">Value to be decrypted</param>
        /// <returns>Decrypted Value</returns>
        int Decrypt(int value);


        /// <summary>
        /// Encrypt the text
        /// </summary>
        /// <param name="value">Value to be encrypted</param>
        /// <returns>Encrypted Value</returns>
        Decimal Encrypt(Decimal value);

        /// <summary>
        /// Decrypt the Text
        /// </summary>
        /// <param name="value">Value to be decrypted</param>
        /// <returns>Decrypted Value</returns>
        Decimal Decrypt(Decimal value);

        /// <summary>
        /// Encrypt the text
        /// </summary>
        /// <param name="value">Value to be encrypted</param>
        /// <returns>Encrypted Value</returns>
        DateTime Encrypt(DateTime value);

        /// <summary>
        /// Decrypt the Text
        /// </summary>
        /// <param name="value">Value to be decrypted</param>
        /// <returns>Decrypted Value</returns>
        DateTime Decrypt(DateTime value);

        /// <summary>
        /// Is Encrypted
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>true, if value is an enctypted text</returns>
        bool IsEncrypted(String value);

        /// <summary>
        /// Encrypt the text
        /// </summary>
        /// <param name="value">Value to be encrypted</param>
        /// <returns>Encrypted Value</returns>
        byte[] Encrypt(char[] value);

        /// <summary>
        /// Decrypt the Text
        /// </summary>
        /// <param name="value">Value to be decrypted</param>
        /// <returns>Decrypted Value</returns>
        byte[] Decrypt(byte[] value);
    }
}
