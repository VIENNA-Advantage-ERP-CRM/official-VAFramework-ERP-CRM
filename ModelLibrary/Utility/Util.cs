/*
 * Module Name    : General Utilities
 * Purpose        : General Utilities
 * Author         : Jagmohan Bhatt
 * Date           : 03-June-2009
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Utility
{
    public class Util
    {
        public static String Replace(String value, String oldPart, String newPart)
        {
            if (value == null)
                return null;
            return Replace(new StringBuilder(value), oldPart, newPart).ToString();
        }

        /// <summary>
        /// Replace String values.
        /// </summary>
        /// <param name="value">string to be processed</param>
        /// <param name="oldPart">old part</param>
        /// <param name="newPart">replacement - can be null or "" </param>
        /// <returns>String with replaced values</returns>
        public static StringBuilder Replace(StringBuilder value, String oldPart, String newPart)
        {
            if (value == null || value.Length == 0
                || oldPart == null || oldPart.Length == 0)
                return value;
            //
            int oldPartLength = oldPart.Length;
            String oldValue = value.ToString();
            StringBuilder retValue = new StringBuilder();
            int pos = oldValue.IndexOf(oldPart);
            while (pos != -1)
            {
                retValue.Append(oldValue.Substring(0, pos));
                if (newPart != null && newPart.Length > 0)
                    retValue.Append(newPart);
                oldValue = oldValue.Substring(pos + oldPartLength);
                pos = oldValue.IndexOf(oldPart);
            }
            retValue.Append(oldValue);
            //	log.fine( "Env.replace - " + value + " - Old=" + oldPart + ", New=" + newPart + ", Result=" + retValue.toString());
            return retValue;
        }	//	replace


        /// <summary>
        /// Remove CR / LF from String
        /// </summary>
        /// <param name="inn">input</param>
        /// <returns>cleaned string</returns>
        public static String RemoveCRLF(String inn)
        {
            char[] inArray = inn.ToCharArray();
            StringBuilder outt = new StringBuilder(inArray.Length);
            for (int i = 0; i < inArray.Length; i++)
            {
                char c = inArray[i];
                if (c == '\n' || c == '\r')
                {
                }
                else
                    outt.Append(c);
            }
            return outt.ToString();
        }	//	removeCRLF


        /// <summary>
        /// Clean - Remove all white spaces
        /// </summary>
        /// <param name="inn">in</param>
        /// <returns>cleaned string</returns>
        public static String CleanWhitespace(String inn)
        {
            if (inn == null)
                return null;
            char[] inArray = inn.ToCharArray();
            StringBuilder outt = new StringBuilder(inArray.Length);
            bool lastWasSpace = false;
            for (int i = 0; i < inArray.Length; i++)
            {

                char c = inArray[i];
                if (Char.IsWhiteSpace(c))
                {
                    if (!lastWasSpace)
                        outt.Append(' ');
                    lastWasSpace = true;
                }
                else
                {
                    outt.Append(c);
                    lastWasSpace = false;
                }
            }
            return outt.ToString();
        }	//	cleanWhitespace

        /// <summary>
        /// Mask HTML content.
        /// i.e. replace characters with &values;
        /// </summary>
        /// <param name="content">content</param>
        /// <returns> masked content</returns>
        public static String MaskHTML(String content)
        {
            return MaskHTML(content, false);
        }	//	maskHTML

        /// <summary>
        /// Mask HTML content.
        /// i.e. replace characters with &values;
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="maskCR">convert CR into <br></param>
        /// <returns> masked content</returns>
        public static String MaskHTML(String content, bool maskCR)
        {
            if (content == null || content.Length == 0 || content.Equals(" "))
                return "&nbsp";
            //
            StringBuilder outt = new StringBuilder();
            char[] chars = content.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                switch (c)
                {
                    case '<':
                        outt.Append("&lt;");
                        break;
                    case '>':
                        outt.Append("&gt;");
                        break;
                    case '&':
                        outt.Append("&amp;");
                        break;
                    case '"':
                        outt.Append("&quot;");
                        break;
                    case '\'':
                        outt.Append("&#039;");
                        break;
                    case '\n':
                        if (maskCR)
                            outt.Append("<br>");
                        break;
                    //
                    default:
                        int ii = (int)c;
                        if (ii > 255)		//	Write Unicode
                            outt.Append("&#").Append(ii).Append(";");
                        else
                            outt.Append(c);
                        break;
                }
            }
            return outt.ToString();
        }	//	maskHTML


        /// <summary>
        /// Init Cap Words With Spaces
        /// </summary>
        /// <param name="inn">input</param>
        /// <returns>init cap</returns>
        public static String InitCap(String inn)
        {
            if (inn == null || inn.Length == 0)
                return inn;
            //
            bool capitalize = true;
            char[] data = inn.ToCharArray();
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == ' ' || Char.IsWhiteSpace(data[i]))
                    capitalize = true;
                else if (capitalize)
                {
                    data[i] = Char.ToUpper(data[i]);
                    capitalize = false;
                }
                else
                    data[i] = Char.ToLower(data[i]);
            }
            return new String(data);
        }	//	initCap

        /// <summary>
        /// Get Char Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>char or 'N' if null</returns>
        public static char GetValueOfChar(string value)
        {
            return value != "" ? char.Parse(value) : 'N';
        }


        /// <summary>
        /// Get Integer value
        /// </summary>
        /// <param name="value">value to be parsed</param>
        /// <returns>if value is blank, return 0</returns>
        public static int GetValueOfInt(string value)
        {
            try
            {
                return value != "" ? Convert.ToInt32(value) : 0;
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// Get integer value from object
        /// </summary>
        /// <param name="value">object parameter</param>
        /// <returns>integer value or 0 if null</returns>
        public static int GetValueOfInt(Object value)
        {
            return (value == null || value.ToString().Trim() == "") ? 0 : Convert.ToInt32(value);
        }
        /// <summary>
        /// To handel Dbnull value from database
        /// 23-Sep-2009
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// 
        public static DateTime? GetValueOfDateTime(Object value)
        {
            if (value != null && value.ToString().Trim() == "")
            {
                return null;
            }

            DateTime? retval = null;

            try
            {
                retval = System.Convert.ToDateTime(value);
            }
            catch
            {

            }
            return retval;
        }


        //for html 
        //get date as string from client side 
        public static DateTime? GetValueOfDateTime(String value)
        {
            if (value != null && value.ToString().Trim() == "")
            {
                return null;
            }

            DateTime? retval = null;

            try
            {
                retval = System.Convert.ToDateTime(value).ToUniversalTime();
            }
            catch
            {

            }
            return retval;
        }



        //public static DateTime? GetValueOfDateTime(Object value)  //Commented By Sarab
        //{
        //    if (value != null && value.ToString().Trim() == "")
        //    {
        //        value = null;
        //    }
        //    return (DateTime?)value;
        //}
        /// <summary>
        /// Get Decimal value from object
        /// </summary>
        /// <param name="value">object</param>
        /// <returns>if value is blank, return 0</returns>
        public static Decimal GetValueOfDecimal(Object value)
        {
            return (value != null && value.ToString() != "" && value != DBNull.Value) ? Convert.ToDecimal(value) : Env.ZERO;
        }

        /// <summary>
        /// Get Decimal value from string
        /// </summary>
        /// <param name="value">string type</param>
        /// <returns>if value is blank, return 0</returns>
        public static Decimal GetValueOfDecimal(string value)
        {
            return value != "" ? Convert.ToDecimal(value) : Env.ZERO;
        }

        /// <summary>
        /// Get Bool value from string
        /// </summary>
        /// <param name="value">string type</param>
        /// <returns>if value is blank, return 0</returns>
        public static bool GetValueOfBool(Object value)
        {
            bool? bValue = (bool?)value;

            if (bValue != null)
            {
                return (bool)bValue;
            }
            return false;
        }
        public static bool GetValueOfBool(String value)
        {
            bool? bValue =System.Convert.ToBoolean(value);

            if (bValue != null)
            {
                return (bool)bValue;
            }
            return false;
        }
        public static Decimal? GetNullableDecimal(object value)
        {
            if (value == null || value.ToString().Trim() == "")
            {
                return null;
            }
            return Convert.ToDecimal(value);
        }

        public static bool IsNUll(object value)
        {
            if (value == null || value.Equals(DBNull.Value))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get Double value from object
        /// </summary>
        /// <param name="value">object value to be parsed</param>
        /// <returns>if value is blank, return 0</returns>
        public static Double GetValueOfDouble(Object value)
        {
            return (value != null && value.ToString() != "" && value != DBNull.Value) ? Convert.ToDouble(value) : 0.0;

        }

        /// <summary>
        /// Get Double value from object
        /// </summary>
        /// <param name="value">object value to be parsed</param>
        /// <returns>if value is blank, return 0</returns>
        public static Double GetValueOfDouble(String value)
        {
            return (value != null || value != "") ? Convert.ToDouble(value) : 0.0;

        }
        /// <summary>
        /// Get Double value from object
        /// </summary>
        /// <param name="value">object value to be parsed</param>
        /// <returns>if value is blank, return 0</returns>
        public static String GetValueOfString(Object value)
        {
            return (value != null) ? value.ToString().Trim() : "";

        }

        /// <summary>
        /// Finds the Max out of two supplied decimals
        /// </summary>
        /// <param name="val1">value1</param>
        /// <param name="val2">value2</param>
        /// <returns>Max</returns>
        public static Decimal Max(Decimal val1, Decimal val2)
        {
            if (val1 < val2)
                return val2;
            else if (val2 < val1)
                return val1;
            else
                return val1;
        }

        /// <summary>
        /// Finds the Minimum out of two supplied decimals
        /// </summary>
        /// <param name="val1">value1</param>
        /// <param name="val2">value2</param>
        /// <returns>Min</returns>
        public static Decimal Min(Decimal val1, Decimal val2)
        {
            if (val1 < val2)
                return val1;
            else if (val2 < val1)
                return val2;
            else
                return val1;
        }

        /// <summary>
        /// Add two supplied decimals
        /// </summary>
        /// <param name="val1">value1</param>
        /// <param name="val2">value2</param>
        /// <returns>Added result</returns>
        public static Decimal Add(Decimal val1, Decimal val2)
        {
            return val1 + val2;
        }

        /// Multiplies two supplied decimals
        /// </summary>
        /// <param name="val1">value1</param>
        /// <param name="val2">value2</param>
        /// <returns>Multiplied result</returns>
        public static Decimal Multiply(Decimal val1, Decimal val2)
        {
            return val1 * val2;
        }

        /// <summary>
        /// Is String Empty
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>true if >= 1 char</returns>
        public static bool IsEmpty(String str)
        {
            return (str == null || str.Length == 0);
        }

        /// <summary>
        ///Clean mnemonic Ampersand (used to indicate shortcut) 
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static String CleanMnemonic(String inStr)
        {
            if (inStr == null || inStr.Length == 0)
                return inStr;
            int pos = inStr.IndexOf('&');
            if (pos == -1)
                return inStr;
            //	Single & - '&&' or '& ' -> &
            if (pos + 1 < inStr.Length && inStr[pos + 1] != ' ')
                inStr = inStr.Substring(0, pos) + inStr.Substring(pos + 1);
            return inStr;
        }

        /// <summary>
        /// Is Equal.
        /// (including null compare, binary array)
        /// </summary>
        /// <param name="o1">one</param>
        /// <param name="o2">two</param>
        /// <returns>true if one == two</returns>
        /// <author>Raghu</author>
        public static Boolean IsEqual(Object o1, Object o2)
        {
            if (o1 == null && o2 == null)
            {
                return true;
            }
            if (o1 == null && o2 != null)
            {
                return false;
            }
            if (o1 != null && o2 == null)
            {
                return false;
            }
            //
            if (o1 is byte[] && o2 is byte[])
            {
                byte[] b1 = (byte[])o1;
                byte[] b2 = (byte[])o2;
                if (b1.Length != b2.Length)
                {
                    return false;
                }
                for (int i = 0; i < b1.Length; i++)
                {
                    if (b1[i] != b2[i])
                        return false;
                }
                return true;
            }

            if (o1 is DateTime && o2 is DateTime)
            {
                DateTime d1 = (DateTime)o1;
                DateTime d2 = (DateTime)o2;
                if (d1.Equals(d2))
                    return true;
                else if (d1.Date == d2.Date && d1.TimeOfDay.Hours == d2.TimeOfDay.Hours && d1.TimeOfDay.Minutes==d2.TimeOfDay.Minutes && d1.TimeOfDay.Seconds == d2.TimeOfDay.Seconds)
                    return true;

            }
            return o1.Equals(o2) || o1.ToString() == o2.ToString();
        }

      
        /// <summary>
        ///Replace String values.
        /// </summary>
        /// <param name="value">string to be processed</param>
        /// <param name="oldPart">old part</param>
        /// <param name="newPart">replacement - can be null or ""</param>
        /// <returns>String with replaced values</returns>
        //public static String Replace(String value, String oldPart, String newPart)
        //{
        //    if (value == null || value.Length == 0
        //        || oldPart == null || oldPart.Length == 0)
        //        return value;
        //    //
        //    int oldPartLength = oldPart.Length;
        //    String oldValue = value;
        //    StringBuilder retValue = new StringBuilder();
        //    int pos = oldValue.IndexOf(oldPart);
        //    while (pos != -1)
        //    {
        //        retValue.Append(oldValue.Substring(0, pos));
        //        if (newPart != null && newPart.Length > 0)
        //            retValue.Append(newPart);
        //        oldValue = oldValue.Substring(pos + oldPartLength);
        //        pos = oldValue.IndexOf(oldPart);
        //    }
        //    retValue.Append(oldValue);
        //    //	log.fine( "Env.replace - " + value + " - Old=" + oldPart + ", New=" + newPart + ", Result=" + retValue.toString());
        //    return retValue.ToString();
        //}	//	replace

        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }



      /// <summary>
      /// Find index of search character in str.
	  /// This ignores content in () and 'texts'
      /// </summary>
      /// <param name="str">string</param>
      /// <param name="search">search search character</param>
      /// <returns>index or -1 if not found</returns>
        public static int FindIndexOf(String str, char search)
        {
            return FindIndexOf(str, search, search);
        }   //  findIndexOf


      /// <summary>
      /// Find index of search characters in str.
	  ///  This ignores content in () and 'texts'
      /// </summary>
      /// <param name="str">String</param>
      /// <param name="search1">search1 first search character</param>
      /// <param name="search2">second search character (or)</param>
      /// <returns>index or -1 if not found</returns>
        public static int FindIndexOf(String str, char search1, char search2)
        {
            if (str == null)
                return -1;
            //
            int endIndex = -1;
            int parCount = 0;
            bool ignoringText = false;
            int size = str.Length;
            while (++endIndex < size)
            {
                char c = str[endIndex];
                if (c == '\'')
                    ignoringText = !ignoringText;
                else if (!ignoringText)
                {
                    if (parCount == 0 && (c == search1 || c == search2))
                        return endIndex;
                    else if (c == ')')
                        parCount--;
                    else if (c == '(')
                        parCount++;
                }
            }
            return -1;
        }


      /// <summary>
      /// Find index of search character in str.
	 ///  This ignores content in () and 'texts'
	  /// </summary>
      /// <param name="str">string</param>
      /// <param name="search">search charactor</param>
      /// <returns>index or -1 if not found</returns>
        public static int FindIndexOf(String str, String search)
        {
            if (str == null || search == null || search.Length == 0)
                return -1;
            //
            int endIndex = -1;
            int parCount = 0;
            bool ignoringText = false;
            int size = str.Length;
            while (++endIndex < size)
            {
                char c = str[endIndex];
                if (c == '\'')
                    ignoringText = !ignoringText;
                else if (!ignoringText)
                {
                    if (parCount == 0 && c == search[0])
                    {
                        if (str.Substring(endIndex).StartsWith(search))
                            return endIndex;
                    }
                    else if (c == ')')
                        parCount--;
                    else if (c == '(')
                        parCount++;
                }
            }
            return -1;
        }   //  findIndexOf




        public static bool Is8Bit(String str)
        {
            if (str == null || str.Length == 0)
                return true;
            char[] cc = str.ToCharArray();
            for (int i = 0; i < cc.Length; i++)
            {
                if (cc[i] > 255)
                {
                    //	System.out.println("Not 8 Bit - " + str);
                    return false;
                }
            }
            return true;
        }	//	is8Bit

        public static int GetCount(String str, char countChar)
        {
            if (str == null || str.Length == 0)
                return 0;
            int counter = 0;
            char[] array = str.ToCharArray();
            foreach (char element in array)
            {
                if (element == countChar)
                    counter++;
            }
            return counter;
        }
        //	getCount
        /// <summary>
        /// Convert the binary to hex string  
        /// </summary>
        /// <param name="bytes">byte array</param>
        /// <returns>hex string</returns>
        static public String ToHexString(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 0)
                return null;

            StringBuilder retString = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; ++i)
            {
                //retString.Append(
                //Integer.toHexString(0x0100 + (bytes[i] & 0x00FF)).substring(1));
                retString.Append(
                    Convert.ToInt32(0 * 0100 + (bytes[i] & 0x00FF)).ToString().Substring(1));
            }
            return retString.ToString();
        }
        /// <summary>
        ///Count Words    
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>number of words</returns>
        public static int CountWords(String str)
        {
            if (str == null || str.Length == 0)
                return 0;
            int words = 0;
            bool lastWasWhiteSpace = false;
            char[] chars = str.Trim().ToCharArray();// str.Trim().ToCharArray();
            //for (char c in chars)
            for (int i = 0; i < chars.Length; i++)
            {
                if (Char.IsWhiteSpace(chars[i]))// Character.isWhitespace(c))
                {
                    if (!lastWasWhiteSpace)
                        words++;
                    lastWasWhiteSpace = true;
                }
                else
                    lastWasWhiteSpace = false;
            }
            return words + 1;
        }

    }
}
