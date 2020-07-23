/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : 
 * Chronological Development
 * Veena Pandey     24-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace VAdvantage.Classes
{
    public class Obscure : Object
    {

        /** Obscure Digits but last 4 = 904 (default) */
        public static String OBSCURETYPE_ObscureDigitsButLast4 = "904";
        /** Obscure Digits but first/last 4 = 944 */
        public static String OBSCURETYPE_ObscureDigitsButFirstLast4 = "944";
        /** Obscure AlphaNumeric but first/last 4 = A44 */
        public static String OBSCURETYPE_ObscureAlphaNumericButFirstLast4 = "A44";
        /** Obscure AlphaNumeric but last 4 = A04 */
        public static String OBSCURETYPE_ObscureAlphaNumericButLast4 = "A04";

        /** Obscure Type			*/
        private String _type = OBSCURETYPE_ObscureDigitsButLast4;
        /**	Clear Value				*/
        private String _clearValue;
        /** Obscrure Value			*/
        private String _obscuredValue;

        public Obscure()
        {
        }

        /**
	     * 	Obscure.
	     * 	Obscure Digits but last 4
	     *	@param clearValue clear value
	     */
        public Obscure(String clearValue)
        {
            SetClearValue(clearValue);
        }

        /**
         * 	Obscure
         *	@param clearValue clear value
         *	@param obscureType Obscure Type
         */
        public Obscure(String clearValue, String obscureType)
        {
            SetClearValue(clearValue);
            SetType(obscureType);
        }

        /// <summary>
        /// Obscure clear value. Obscure Digits but last 4
        /// name changed from obscure - ObscureValue as Obscure is a constructor
        /// </summary>
        /// <param name="clearValue">clear value</param>
        /// <returns>obscured value or "-"</returns>
        public static String ObscureValue(String clearValue)
        {
            if (clearValue == null || clearValue.Length == 0)
                return "-";
            Obscure ob = new Obscure(clearValue);
            return ob.GetObscuredValue();
        }

        /// <summary>
        /// Obscure clear value
        /// name changed from obscure - ObscureValue as Obscure is a constructor
        /// </summary>
        /// <param name="clearValue">clear value</param>
        /// <param name="obscureType">Obscure Type</param>
        /// <returns>obscured value</returns>
        public static String ObscureValue(String clearValue, String obscureType)
        {
            Obscure ob = new Obscure(clearValue, obscureType);
            return ob.GetObscuredValue();
        }

        /**
         *	Set Type
         *	@param obscureType Obscure Type
         */
        public void SetType(String obscureType)
        {
            if (obscureType == null || obscureType.Equals("904") || obscureType.Equals("944") || obscureType.Equals("A44") || obscureType.Equals("A04"))
            {
                _type = obscureType;
                _obscuredValue = null;
                return;
            }
            throw new ArgumentException("ObscureType Invalid value - Reference_ID=291 - 904 - 944 - A44 - A04, passed \"" + obscureType + "\"");
        }

        /**
         * 	Get Obscure Type
         *	@return type
         */
        public new String GetType()
        {
            return _type;
        }

        /**
         *	Get Clear Value
         * 	@return Returns the clear Value.
         */
        public String GetClearValue()
        {
            return _clearValue;
        }

        /**
         *	Set Clear Value
         *	@param clearValue The clearValue to Set.
         */
        public void SetClearValue(String clearValue)
        {
            _clearValue = clearValue;
            _obscuredValue = null;
        }

        /**
         *	Get Obscured Value
         *	@param clearValue The clearValue to Set.
         *	@return Returns the obscuredValue.
         */
        public String GetObscuredValue(String clearValue)
        {
            SetClearValue(clearValue);
            return GetObscuredValue();
        }

        /**
         *	Get Obscured Value
         *	@return Returns the obscuredValue.
         */
        public String GetObscuredValue()
        {
            if (_obscuredValue != null)
                return _obscuredValue;
            if (_clearValue == null || _clearValue.Length == 0)
                return _clearValue;
            //
            Boolean alpha = _type.IndexOf('A') == 0;
            int clearStart = int.Parse(_type.Substring(1, 2));
            int clearEnd = int.Parse(_type.Substring(2));
            //
            char[] chars = _clearValue.ToCharArray();
            int length = chars.Length;
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                char c = chars[i];
                if (i < clearStart)
                    sb.Append(c);
                else if (i >= length - clearEnd)
                    sb.Append(c);
                else
                {
                    if (!alpha && !Char.IsDigit(c))
                        sb.Append(c);
                    else
                        sb.Append('*');
                }
            }
            _obscuredValue = sb.ToString();
            return _obscuredValue;
        }
    }
}
