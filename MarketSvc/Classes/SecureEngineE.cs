using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Logging;

namespace MarketSvc
{
   internal class SecureEngineE
    //internal class SecureEngineE
    {
        /** SecureE Engine 				*/
        private static SecureEngineE s_engine = null;	
	    /**	Logger						*/
        private static Logger log = Logger.GetLogger(typeof(SecureEngineE).FullName);

        /** The real Engine				*/
        private SecureInterfaceE implementation = null;

        /// <summary>
        /// Initialize Security
        /// </summary>
        public static void Init()
        {
            if (s_engine == null)
            {
                String className = "WCFServiceImplementation.SecureE";
                s_engine = new SecureEngineE(className);
            }
        }	//	init

        /// <summary>
        /// SecureEngineE constructor
        /// </summary>
        /// <param name="className">class name if null defaults to envision.ad.Utility.SecureE</param>
        public SecureEngineE(string className)
        {
            String realClass = className;
            if(string.IsNullOrEmpty(realClass))
                realClass = SecureE.VIENNA_SECURE_DEFAULT;

            Exception cause = null;
            try
            {
                //try instantiating the class
                Type clazz = Type.GetType(realClass);
                implementation = (SecureInterfaceE)Activator.CreateInstance(clazz);
            }
            catch (Exception ex)
            {
                try
                {
                    implementation = new SecureE();
                }
                catch
                {
                }
                cause = ex;
            }
            if (implementation == null)
            {
                //incase class could not be initialized
                String msg = "Could not initialize: " + realClass + " - " + cause.Message + "\nCheck start script parameter ENVISION_SECURE";
                log.Severe(msg);
            }
        }

        /// <summary>
        /// Reset - don't call when application is up!
        /// </summary>
        internal static void Reset()
        {
            log.Info("");
            s_engine = null;
            Init();
        }	//	reset
        
        /// <summary>
        /// Get the class name
        /// </summary>
        /// <returns>Name of the current class</returns>
        public static String GetClassName()
        {
            if (s_engine == null)
                return null;
            return s_engine.implementation.GetType().FullName;
        }	//	getClassName


        /// <summary>
        /// Encrypt the text
        /// </summary>
        /// <param name="value">Value to be encrypted</param>
        /// <returns>Encrypted Value</returns>
        public static String Encrypt(String value)
        {
            //'kamal'
            if(string.IsNullOrEmpty(value))
                return value;
            if (s_engine == null)
                Init();
            //
            bool inQuotes = value.StartsWith("'") && value.EndsWith("'");
            if (inQuotes)
                value = value.Substring(1, value.Length - 2);
            //
            String retValue = s_engine.implementation.Encrypt(value);
            if (inQuotes)
                return "'" + retValue + "'";
            return retValue;
        }	//	encrypt


        /// <summary>
        /// Decrypt the Text
        /// </summary>
        /// <param name="value">Value to be decrypted</param>
        /// <returns>Decrypted Text</returns>
        public static String Decrypt(String value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            if (s_engine == null)
                Init();
            bool inQuotes = value.StartsWith("'") && value.EndsWith("'");
            if (inQuotes)
                value = value.Substring(1, value.Length - 2);
            String retValue = null;
            if (value.StartsWith(SecureE.CLEARVALUE_START) && value.EndsWith(SecureE.CLEARVALUE_END))
                retValue = value.Substring(SecureE.CLEARVALUE_START.Length, SecureE.CLEARVALUE_START.Length - (value.Length - SecureE.CLEARVALUE_END.Length));
            else
                retValue = s_engine.implementation.Decrypt(value);
            if (inQuotes)
                return "'" + retValue + "'";
            return retValue;
        }	//	decrypt

        /// <summary>
        /// 	Encryption.
        /// The methods must recognize clear values
        /// </summary>
        /// <param name="value">@param value clear value</param>
        /// <returns>   @return encrypted String</returns>
        public static Object Encrypt(Object value)
        {
            if (value is String)
                return Encrypt((String)value);
            return value;
        }	//	encrypt

        /// <summary>
        ///	Decryption.
        /// the methods must recognize clear values
        /// </summary>
        /// <param name="value">value encrypted value</param>
        /// <returns>Decrypted value</returns>
        public static Object Decrypt(Object value)
        {
            if (value is String)
                return Decrypt((String)value);
            return value;
        }	//	decrypt

        public static bool IsEncrypted(String value)
        {
            return s_engine.implementation.IsEncrypted(value);
        }	//	isEncrypted
    }
}
