using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace MarketSvc
{
    /// <summary>
    /// Secure Key - replaced at runtime with real key
    /// </summary>
    internal class SecureKeyE
    //internal class SecureKey
    {
        /// <summary>
        /// Security Key
        /// </summary>
        static internal byte[] key = new byte[] { 196, 23, 250, 179, 110, 131, 240, 41, 139, 146, 176, 40, 176, 224, 208, 191, 14, 63, 147, 81, 241, 205, 148, 78, 158, 191, 40, 131, 134, 130, 20, 160 };
        /// <summary>
        /// Vector IV Key
        /// </summary>
        static internal byte[] vector = new byte[] { 21, 101, 93, 18, 36, 42, 61, 77, 139, 146, 176, 191, 14, 63, 196, 223 };
    }
}
