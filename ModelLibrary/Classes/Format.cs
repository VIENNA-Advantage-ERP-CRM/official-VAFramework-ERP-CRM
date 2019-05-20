/********************************************************
// Clasa Name    : Format
// Purpose        : Generate the format of number for different referece quantity, amount, interger etc.
//
// Class Used     : 
// Created By     : Mukesh Arora
// Date           : 18 july 2009
**********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Classes
{
    public class Format
    {
        //store max int digit
        private int _maxIntDigit;
        //store max fraction digit
        private int _maxFractionDigit;
        //store minimum fraction digit
        private int _minFractionDigit;

        public int MaxIntDigit
        {
            get {
                return _maxIntDigit;
            }
            set
            {
                _maxIntDigit = value;
            }
        }
        public int MaxFractionDigit
        {
            set
            {
                _maxFractionDigit = value;
            }
        }
        public int MinFractionDigit
        {
            set
            {
                _minFractionDigit = value;
            }
        }
        /// <summary>
        /// return the format of a number
        /// </summary>
        /// <returns>string</returns>
        public string GetFormat()
        {
            StringBuilder retValue = new StringBuilder();
            retValue.Append("{0:#,0.");

            for (int i = 0; i < _minFractionDigit; i++)
            {
                retValue.Append("0");
            }
            for (int i = 0; i < _maxFractionDigit; i++)
            {
                retValue.Append("#");
            }
            retValue.Append("}");
            return retValue.ToString();
        }
        /// <summary>
        /// Set the integer part of a number
        /// </summary>
        /// <param name="val">object value</param>
        /// <returns>object</returns>
        public object SetIntDigit(object val)
        {
            string orgStr = val.ToString();

            int deciPos = orgStr.IndexOf('.');
            //if sending object is decimal type
            if (deciPos != -1)
            {
                string beforeDeciStr = orgStr.Substring(0, deciPos);
                if (beforeDeciStr.Length > _maxIntDigit)
                {
                    beforeDeciStr = beforeDeciStr.Substring(0, _maxIntDigit);
                    string finalStr = beforeDeciStr + orgStr.Substring(deciPos, orgStr.Length - deciPos);
                    return (object)finalStr;
                }

            }
            else //if sending object is integer type
            {
                if (orgStr.Length > _maxIntDigit)
                {

                    string finalStr = orgStr.Substring(0, _maxIntDigit);
                    if (Int64.Parse(finalStr) > Int32.MaxValue)
                        return (object)Int32.MaxValue;
                    return (object)Convert.ToInt32(orgStr.Substring(0, _maxIntDigit));
                    
                }
            }
            
            return val;
        }

        /// <summary>
        /// return formatted string 
        /// if value is greate than system default integer max value
        /// then slice value to set integer max
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>

        public string GetFormatedValue(object val)
        {
            object o = SetIntDigit(val);
            return string.Format(GetFormat(), o);
        }
    }
}
