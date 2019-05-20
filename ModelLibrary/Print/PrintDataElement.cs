/********************************************************
 * Module Name    :     Report
 * Purpose        :     Generate Reports
 * Author         :     Jagmohan Bhatt
 * Date           :     
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;

using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Model;
using System.Data;
using System.Data.SqlClient;

namespace VAdvantage.Print
{
    public class PrintDataElement
    {
        public PrintDataElement(String columnName, Object value, int displayType, bool isPKey, bool isPageBreak)
        {
            if (columnName == null)
                throw new ArgumentException("PrintDataElement - Name cannot be null");
            _columnName = columnName;
            //if (value  is  DateTime)
            //{
            //    _value = value.ToString().Substring(0, 10);
            //}
            //else
            //{
            //    _value = value;
            //}
            _value = value;
            _displayType = displayType;
            _isPKey = isPKey;
            _isPageBreak = isPageBreak;
        }	//	PrintDataElement


        public PrintDataElement(String columnName, Object value, int displayType)
            : this(columnName, value, displayType, false, false)
        {

        }	//	PrintDataElement

        public PrintDataElement(String columnName, Object value, object originalValue, int displayType)
        {
            _columnName = columnName;
            _value = value;
            _originalValue = originalValue;
            _displayType = displayType;
            _isPKey = false;
            _isPageBreak = false;
        }	//	PrintDataElement


        /**	Data Name			*/
        private String _columnName;

        /** Data Value			*/
        private Object _value;

        /** Data Value			*/
        private Object _originalValue;
        /** Display Type		*/
        private int _displayType;
        /** Is Primary Key		*/
        private bool _isPKey;
        /**	Is Page Break		*/
        private bool _isPageBreak;


        /**	XML Element Name			*/
        public static String XML_TAG = "element";
        /**	XML Attribute Name			*/
        public static String XML_ATTRIBUTE_NAME = "name";
        /**	XML Attribute Key			*/
        public static String XML_ATTRIBUTE_KEY = "key";


        public String GetColumnName()
        {
            return _columnName;
        }	//	getName

        public Object GetValue()
        {
            return _value;
        }	//	getValue



        /// <summary>
        /// Get function value
        /// </summary>
        /// <returns>value of function </returns>
        public Decimal GetFunctionOriginalValue()
        {
            if (_originalValue == null)
                return Env.ZERO;

            //	Numbers - return number value
            if (_originalValue.GetType() == typeof(Decimal))
                return (Decimal)_originalValue;
            if (_originalValue.GetType() == typeof(double))
                return new Decimal(((double)_originalValue));

            //	Boolean - return 1 for true 0 for false
            if (_originalValue.GetType() == typeof(Boolean))
            {
                if (((Boolean)_originalValue))
                    return Env.ONE;
                else
                    return Env.ZERO;
            }

            //	Return Length
            String s = _originalValue.ToString();
            return (decimal)s.Length;
        }	//	getFunctionValue



        /// <summary>
        /// Get function value
        /// </summary>
        /// <returns>value of function </returns>
        public Decimal GetFunctionValue()
        {
            if (_value == null)
                return Env.ZERO;

            //	Numbers - return number value
            if (_value.GetType() == typeof(Decimal))
                return (Decimal)_value;
            if (_value.GetType() == typeof(double))
                return new Decimal(((double)_value));

            //	Boolean - return 1 for true 0 for false
            if (_value.GetType() == typeof(Boolean))
            {
                if (((Boolean)_value))
                    return Env.ONE;
                else
                    return Env.ZERO;
            }

            //	Return Length
            String s = _value.ToString();
            return (decimal)s.Length;
        }	//	getFunctionValue


        /// <summary>
        /// Get value to be displayed
        /// </summary>
        /// <param name="language">current language</param>
        /// <returns></returns>
        public String GetValueDisplay(VAdvantage.Login.Language language)
        {
            if (_value == null)
                return "";
            String retValue = _value.ToString();
            if (_displayType == DisplayType.Location)
                return GetValueDisplay_Location();
            else if (_columnName.Equals("C_BPartner_Location_ID") || _columnName.Equals("Bill_Location_ID"))
                return GetValueDisplay_BPLocation();
            else if (_displayType == 0 || _value.GetType() == typeof(String) || _value.GetType() == typeof(NamePair))
            { }
            else if (language != null)	//	Optional formatting of Numbers and Dates
            {
                if (DisplayType.IsNumeric(_displayType))
                    retValue = DisplayType.GetNumberFormat(_displayType).GetFormatedValue(_value);
                if (DisplayType.IsDate(_displayType))
                {
                    //  retValue = DisplayType.GetDateFormat(_displayType).Format(_value);
                }
            }
            return retValue;
        }	//	getValueDisplay


        /// <summary>
        /// Get value as string
        /// </summary>
        /// <returns>value in string</returns>
        public String GetValueAsString()
        {
            if (_value == null)
                return "";
            String retValue = _value.ToString();
            if (_value.GetType() == typeof(NamePair))
                retValue = ((NamePair)_value).GetID();
            return retValue;
        }	//	getValueDisplay


        /// <summary>
        /// Get value location
        /// </summary>
        /// <returns></returns>
        private String GetValueDisplay_Location()
        {
            try
            {
                int C_Location_ID = int.Parse(GetValueKey());
                if (C_Location_ID != 0)
                {
                    MLocation loc = new MLocation(Env.GetContext(), C_Location_ID, null);
                    if (loc != null)
                        return loc.ToStringCR();
                }
            }
            catch
            {
            }
            return _value.ToString();
        }	//	getValueDisplay_Location


        /// <summary>
        /// Get value BP location
        /// </summary>
        /// <returns></returns>
        private String GetValueDisplay_BPLocation()
        {
            try
            {
                int C_BPartner_Location_ID = int.Parse(GetValueKey());
                if (C_BPartner_Location_ID != 0)
                {
                    MLocation loc = MLocation.GetBPLocation(Env.GetContext(), C_BPartner_Location_ID, null);
                    if (loc != null)
                        return loc.ToStringCR();
                }
            }
            catch 
            {
            }
            return _value.ToString();
        }	//	getValueDisplay_BPLocation


        /// <summary>
        /// Get value key
        /// </summary>
        /// <returns></returns>
        public String GetValueKey()
        {
            if (_value == null)
                return "";
            if (_value is NamePair)
                return ((NamePair)_value).GetID();
            return "";
        }	//	getValueKey


        /// <summary>
        /// Is null ??
        /// </summary>
        /// <returns></returns>
        public bool IsNull()
        {
            return _value == null;
        }	//	isNull

        /// <summary>
        /// Get disply type
        /// </summary>
        /// <returns>display type value</returns>
        public int GetDisplayType()
        {
            return _displayType;
        }	//	getDisplayType

        public bool IsNumeric()
        {
            if (_displayType == 0)
                return _value.GetType() == typeof(Decimal);
            return DisplayType.IsNumeric(_displayType);
        }	//	isNumeric

        public bool IsDate()
        {
            if (_displayType == 0)
                return _value.GetType() == typeof(DateTime);
            return DisplayType.IsDate(_displayType);
        }	//	isDate


        public bool IsID()
        {
            return DisplayType.IsID(_displayType);
        }	//	isID

        public bool IsYesNo()
        {
            if (_displayType == 0)
                return _value.GetType() == typeof(Boolean);
            return DisplayType.YesNo == _displayType;
        }	//	isYesNo

        public bool IsPKey()
        {
            return _isPKey;
        }	//	isPKey

        public bool IsPageBreak()
        {
            return _isPageBreak;
        }	//	isPageBreak

        public override int GetHashCode()
        {
            if (_value == null)
                return _columnName.GetHashCode();
            return _columnName.GetHashCode() + _value.GetHashCode();

        }   //  GetHashCode

        public override bool Equals(object compare)
        {
            if (compare.GetType() == typeof(PrintDataElement))
            {
                PrintDataElement pde = (PrintDataElement)compare;
                if (pde.GetColumnName().Equals(_columnName))
                {
                    if (pde.GetValue() != null && pde.GetValue().Equals(_value))
                        return true;
                    if (pde.GetValue() == null && _value == null)
                        return true;
                }
            }
            return false;
        }

        public bool HasKey()
        {
            return _value.GetType() == typeof(NamePair);
        }	//	hasKey

        public String ToStringX()
        {
            if (_value.GetType() == typeof(NamePair))
            {
                NamePair pp = (NamePair)_value;
                StringBuilder sb = new StringBuilder(_columnName);
                sb.Append("(").Append(pp.GetID()).Append(")")
                    .Append("=").Append(pp.GetName());
                if (_isPKey)
                    sb.Append("(PK)");
                return sb.ToString();
            }
            else
                return ToString();
        }	//	toStringX
    }
}
