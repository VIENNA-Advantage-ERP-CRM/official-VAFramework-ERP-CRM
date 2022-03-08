/********************************************************
 * Module Name    : show field in single layout
 * Purpose        : define Sysytem Display type to a contant int value 
 *                : the constant is used for different checks through out  proj 
 * Class Used     : -----------
 * Created By     : Harwinder 
 * Date           : 
**********************************************************/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
//using VAdvantage.Process;

namespace VAdvantage.Classes
{

    /*********************************************************************************
      * 
      * Class Fieldtype
      * 
      * *******************************************************************************/
    public class FieldType
    {

        #region "Declaration"
        /** Display Type 10	String	*/
        public static int String = 10;
        /** Display Type 11	Integer	*/
        public static int Integer = 11;
        /** Display Type 12	Amount	*/
        public static int Amount = 12;
        /** Display Type 13	ID	*/
        public static int ID = 13;
        /** Display Type 14	Text	*/
        public static int Text = 14;
        /** Display Ty 15	Date	*/
        public static int Date = 15;
        /** Display Type 16	DateTime	*/
        public static int DateTime = 16;
        /** Display Type 17	List	*/
        public static int List = 17;
        /** Display Type 18	Table	*/
        public static int Table = 18;
        /** Display Type 19	TableDir	*/
        public static int TableDir = 19;
        /** Display Type 20	YN	*/
        public static int YesNo = 20;
        /** Display Type 21	Location	*/
        public static int Location = 21;
        /** Display Type 22	Number	*/
        public static int Number = 22;
        /** Display Type 23	BLOB	*/
        public static int Binary = 23;
        /** Display Type 24	Time	*/
        public static int Time = 24;
        /** Display Type 25	Account	*/
        public static int Account = 25;
        /** Display Type 26	RowID	*/
        public static int RowID = 26;
        /** Display Type 27	Color   */
        public static int Color = 27;
        /** Display Type 28	Button	*/
        public static int Button = 28;
        /** Display Type 29	Quantity	*/
        public static int Quantity = 29;
        /** Display Type 30	Search	*/
        public static int Search = 30;
        /** Display Type 31	Locator	*/
        public static int Locator = 31;
        /** Display Type 32 Image	*/
        public static int Image = 32;
        /** Display Type 33 Assignment	*/
        public static int Assignment = 33;
        /** Display Type 34	Memo	*/
        public static int Memo = 34;
        /** Display Type 35	PAttribute	*/
        public static int PAttribute = 35;
        /** Display Type 36	CLOB	*/
        public static int TextLong = 36;
        /** Display Type 37	CostPrice	*/
        public static int CostPrice = 37;
        /** Display Type 36	File Path	*/
        public static int FilePath = 38;
        /** Display Type 39 File Name	*/
        public static int FileName = 39;
        /** Display Type 40	URL	*/
        public static int URL = 40;
        /** Display Type 42	PrinterName	*/
        public static int PrinterName = 42;

        /**New  Display Type 44 Label */
        public static int Label = 44;

        /** Display Type 45	Multi Search	*/
        public static int MultiKey = 45;

        /** Display Type 46	General Attribute*/
        public static int GAttribute = 46;

        /** Display Type 47	Amount Dimension*/
        public static int AmtDimension = 47;

        /** Display Type 48	Product Container*/
        public static int ProductContainer = 48;

        /** Display Type 49	ProgressBar*/
        public static int ProgressBar = 49;

        /** Display Type 101 Label */
        //public static int Label = 101;
        #endregion

        /// <summary>
        ///Returns true if (numeric) ID (Table, Search, Account, ..).
        /// </summary>
        /// <param name="displayType"></param>
        /// <returns></returns>
        public static bool IsID(int displayType)
        {
            if (displayType == ID || displayType == Table || displayType == TableDir
                || displayType == Search || displayType == Location || displayType == Locator
                || displayType == Account || displayType == Assignment || displayType == PAttribute
                || displayType == Image || displayType == Color || displayType == AmtDimension || displayType == ProductContainer|| displayType== GAttribute)
                return true;
            return false;
        }	//	isID

        /// <summary>
        ///Returns true, if DisplayType is numeric (Amount, Number, Quantity, Integer).
        /// </summary>
        /// <param name="displayType"></param>
        /// <returns></returns>
        public static bool IsNumeric(int displayType)
        {
            if (displayType == Amount || displayType == Number || displayType == CostPrice
                || displayType == Integer || displayType == Quantity || displayType == ProgressBar)
                return true;
            return false;
        }	//	isNumeric

        /// <summary>
        ///	Get Default Precision.
        /// 	Used for databases who cannot handle dynamic number precision.
        /// </summary>
        /// <param name="displayType"></param>
        /// <returns></returns>
        public static int GetDefaultPrecision(int displayType)
        {
            if (displayType == Amount )
                return 2;
            if (displayType == Number)
                return 6;
            if (displayType == CostPrice
                || displayType == Quantity)
                return 4;
            return 0;
        }	//	getDefaultPrecision

        /// <summary>
        /// Returns true, if DisplayType is text (String, Text, TextLong, Memo).
        /// </summary>
        /// <param name="displayType"></param>
        /// <returns></returns>
        public static bool IsText(int displayType)
        {
            if (displayType == String || displayType == Text
                || displayType == TextLong || displayType == Memo
                || displayType == FilePath || displayType == FileName
                || displayType == URL || displayType == PrinterName)
                return true;
            return false;
        }	//	isText

        /// <summary>
        ///Returns truem if DisplayType is a Date.
        /// </summary>
        /// <param name="displayType"></param>
        /// <returns></returns>
        public static bool IsDate(int displayType)
        {
            if (displayType == Date || displayType == DateTime || displayType == Time)
                return true;
            return false;
        }	//	isDate

        /// <summary>
        ///Returns true if DisplayType is a VLookup (List, Table, TableDir, Search).
        /// </summary>
        /// <param name="displayType"></param>
        /// <returns></returns>
        public static bool IsLookup(int displayType)
        {
            if (displayType == List || displayType == Table
                || displayType == TableDir || displayType == Search || displayType == MultiKey || displayType == ProductContainer)
                return true;
            return false;
        }	//	isLookup

        /// <summary>
        /// return True if display type is clientlook up(vlookup or locator) 
        /// </summary>
        /// <param name="displayType"></param>
        /// <returns></returns>
        public static bool IsClientLookup(int displayType)
        {
            //locator also a lookup in gwt
            return IsLookup(displayType) || displayType == FieldType.Locator;
        }

        /// <summary>
        ///Returns true if DisplayType is a Large Object
        /// </summary>
        /// <param name="displayType"></param>
        /// <returns></returns>
        public static bool IsLOB(int displayType)
        {
            if (displayType == Binary
                || displayType == TextLong)
                return true;
            return false;
        }	//	isLOB

        /// <summary>
        /// Is TextArea
        /// </summary>
        /// <param name="displayType"></param>
        /// <returns></returns>
        public static bool IsTextArea(int displayType)
        {
            return displayType == Text || displayType == Memo || displayType == TextLong;
        }

        /// <summary>
        /// Returns true if the field should be searched with case sensitivity
        /// </summary>
        /// <param name="displayType"></param>
        /// <returns></returns>
        public static bool IsCaseSensitive(int displayType)
        {
            return !IsText(displayType);
        }

        ///<summary>
        ///Get SQL DataType
        ///<param name="displayType">AD_Reference_ID</param>
        ///<param name="columnName">name</param>
        ///<param name="fieldLength">length</param>
        /// </summary>
        public static string GetSQLDataType(int displayType, String columnName, int fieldLength)
        {
            if (columnName.Equals("EntityType") || columnName.Equals("AD_Language") || columnName.Equals("DocBaseType"))
                return "VARCHAR2(" + fieldLength + ")";
            //	ID
            if (DisplayType.IsID(displayType))
            {
                if (displayType == DisplayType.Image 	//	FIXTHIS
                    && columnName.Equals("BinaryData"))
                    return "BLOB";
                //	ID, CreatedBy/UpdatedBy, Acct
                else if (columnName.EndsWith("_ID")
                    || columnName.EndsWith("tedBy")
                    || columnName.EndsWith("_Acct")
                    || columnName.EndsWith("_ID_1")
                    || columnName.EndsWith("_ID_2")
                    || columnName.EndsWith("_ID_3")
                    )
                    return "NUMBER(10)";
                else if (fieldLength < 4)
                    return "CHAR(" + fieldLength + ")";
                else	//	EntityType, AD_Language	fallback
                    return "VARCHAR2(" + fieldLength + ")";
            }
            //
            if (displayType == DisplayType.Integer)
                return "NUMBER(10)";
            if (DisplayType.IsDate(displayType))
                return "DATE";
            if (DisplayType.IsNumeric(displayType))
                return "NUMBER";
            if (displayType == DisplayType.Binary)
                return "BLOB";
            if (displayType == DisplayType.TextLong
                || (displayType == DisplayType.Text && fieldLength >= 4000))
                return "CLOB";
            if (displayType == DisplayType.YesNo)
                return "CHAR(1)";
            if (displayType == DisplayType.List)
                return "CHAR(" + fieldLength + ")";
            if (displayType == DisplayType.Color)
            {
                if (columnName.EndsWith("_ID"))
                    return "NUMBER(10)";
                else
                    return "CHAR(" + fieldLength + ")";
            }
            if (displayType == DisplayType.Button)
            {
                if (columnName.EndsWith("_ID"))
                    return "NUMBER(10)";
                else
                    return "CHAR(" + fieldLength + ")";
            }

            if (displayType == DisplayType.Label)
            {
                if (columnName.EndsWith("_ID"))
                    return "NUMBER(10)";
                else
                    return "NVARCHAR2(" + fieldLength + ")";
            }


            if (fieldLength > 2000 && fieldLength <= 4000)
                return "VARCHAR2(" + fieldLength + ")";
            if (!DisplayType.IsText(displayType))
            {
                //   s_log.severe("Unhandled Data Type = " + displayType);
            }
            return "NVARCHAR2(" + fieldLength + ")";
        }	//	getSQLDataType

        /// <summary>
        /// Get Sysytem Data Type against Display type value
        /// </summary>
        /// <param name="displayType"></param>
        /// <param name="yesNoAsBoolean"></param>
        /// <returns></returns>
        public static Type GetClass(int displayType, bool yesNoAsBoolean)
        {
            if (IsText(displayType) || displayType == List || displayType == MultiKey)
                return typeof(System.String);
            else if (IsID(displayType) || displayType == Integer)    //  note that Integer is stored as BD
                return typeof(System.Int32);
            else if (IsNumeric(displayType))
                return typeof(decimal);
            else if (IsDate(displayType))
                return typeof(System.DateTime);
            else if (displayType == YesNo)
                return yesNoAsBoolean ? typeof(System.Boolean) : typeof(System.String);
            else if (displayType == Button)
                return typeof(System.String);
            else if (IsLOB(displayType))	//	CLOB is String
                return typeof(System.Byte[]);
            //
            return typeof(System.Object);
        }   //  getClass

    }	//	FieldType

    /*************************************************************************************
     * 
     * Display Type class
     * 
     * **********************************************************************************/
    public class DisplayType : FieldType
    {
        //  See DBA_DisplayType.sql ----------------------------------------------

        /** Maximum number of digits    */
        private static int MAX_DIGITS = 28;        //  Oracle Standard Limitation 38 digits
        /** Digits of an Integer        */
        private static int INTEGER_DIGITS = 10;
        /** Maximum number of fractions */
        private static int MAX_FRACTION = 12;
        /** Default Amount Precision    */
        private static int AMOUNT_FRACTION = 2;

        /// <summary>
        ///   Return Format for numeric DisplayType	
        /// </summary>
        /// 
        /// <param name="displayType">Display Type (default Number)</param>
        /// <param name="language">Language</param>
        /// <returns>number format</returns>
        public static Format GetNumberFormat(int displayType, string language)
        {
            //Language myLanguage = language;
            //if (myLanguage == null)
            //    myLanguage = Language.getLoginLanguage();
            //Locale locale = myLanguage.getLocale();
            Format format = new Format();
            //if (locale != null)
            //    format = (DecimalFormat)NumberFormat.getNumberInstance(locale);
            //else
            //    format = (DecimalFormat)NumberFormat.getNumberInstance(Locale.US);
            //
            if (displayType == Integer ||  displayType == ProgressBar)
            {
                //format.setParseIntegerOnly(true);
                format.MaxIntDigit = INTEGER_DIGITS;
                format.MaxFractionDigit = 0;
            }
            else if (displayType == Quantity)
            {
                format.MaxIntDigit = MAX_DIGITS;
                format.MaxFractionDigit = MAX_FRACTION;
            }
            else if (displayType == Amount )
            {
                format.MaxIntDigit = MAX_DIGITS;
                format.MaxFractionDigit = MAX_FRACTION;
                format.MinFractionDigit = AMOUNT_FRACTION;
            }
            else if (displayType == CostPrice)
            {
                format.MaxIntDigit = MAX_DIGITS;
                format.MaxFractionDigit = MAX_FRACTION;
                format.MinFractionDigit = AMOUNT_FRACTION;
            }
            else //	if (displayType == Number)
            {
                format.MaxIntDigit = MAX_DIGITS;
                format.MaxFractionDigit = MAX_FRACTION;
                format.MinFractionDigit = 1;
            }
            return format;
        }	//	getDecimalFormat


        /// <summary>
        ///Return Format for numeric DisplayType
        /// </summary>
        /// <param name="displayType">Display Type</param>
        /// <returns>number format</returns>
        public static Format GetNumberFormat(int displayType)
        {
            return GetNumberFormat(displayType, null);
        }

        public static SimpleDateFormat GetDateFormat(int displayTye)
        {
            return new SimpleDateFormat(displayTye);
        }
    }


    public class SimpleDateFormat
    {

        private Dictionary<int, String> _dateTimeFormats;

        public const int DATEFULL = -1;
        public const int DATEMEDIUM = -2;
        public const int DATESHORT = -3;

        private int _displayType = DisplayType.Date;

        //"(d) Short date: . . . . . . . {0:d}\n" +
        //       "(D) Long date:. . . . . . . . {0:D}\n" +
        //       "(t) Short time: . . . . . . . {0:t}\n" +
        //       "(T) Long time:. . . . . . . . {0:T}\n" +
        //       "(f) Full date/short time: . . {0:f}\n" +
        //       "(F) Full date/long time:. . . {0:F}\n" +
        //       "(g) General date/short time:. {0:g}\n" +
        //       "(G) General date/long time: . {0:G}\n" +
        //       "    (default):. . . . . . . . {0} (default = 'G')\n" +
        //       "(M) Month:. . . . . . . . . . {0:M}\n" +
        //       "(R) RFC1123:. . . . . . . . . {0:R}\n" +
        //       "(s) Sortable: . . . . . . . . {0:s}\n" +
        //       "(u) Universal sortable: . . . {0:u} (invariant)\n" +
        //       "(U) Universal full: . . . . . {0:U}\n" +
        //       "(Y) Year: . . . . . . . . . . {0:Y}\n",




        // private string _dateFormatString = "";

        public SimpleDateFormat()
        {
            _dateTimeFormats = new Dictionary<int, string>();
            _dateTimeFormats.Add(DisplayType.Date, "{0:D}");
            _dateTimeFormats.Add(DisplayType.DateTime, "{0:F}");
            _dateTimeFormats.Add(DisplayType.Time, "{0:T}");
            _dateTimeFormats.Add(-1, "{0:F}");//not used 
            _dateTimeFormats.Add(-2, "{0:D}");//not used 
            _dateTimeFormats.Add(-3, "{0:d}");//* Short Date
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="displayType"></param>
        public SimpleDateFormat(int displayType)
            : this()
        {
            _displayType = displayType;
        }



        /// <summary>
        /// return format string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public String Format(object obj)
        {
            DateTime dt;
            if (obj is DateTime)
            {
                dt = (DateTime)obj;
            }
            else if (obj is long)
            {
                dt = CommonFunctions.CovertMilliToDate((long)obj);
            }
            else
            {
                throw new ArgumentException("Input is not in correct format(long/datetime)");
            }
            string fmtStr = _dateTimeFormats[_displayType];

            try
            {
                return String.Format(System.Globalization.CultureInfo.InvariantCulture, fmtStr, dt);
            }
            catch (Exception e)
            {
                Logging.VLogger.Get().Log(VAdvantage.Logging.Level.SEVERE, e.Message);
                return "";
            }
        }

        /// <summary>
        /// Get Formated Date according to Culture
        /// </summary>
        /// <param name="obj">Date Value</param>
        /// <param name="lang">Client Culture</param>
        /// <returns>Formated date as String</returns>
        public String Format(object obj, string lang, int format)
        {
            DateTime dt;
            CultureInfo culture;

            if (obj is DateTime)
            {
                dt = (DateTime)obj;
            }
            else if (obj is long)
            {
                dt = CommonFunctions.CovertMilliToDate((long)obj);
            }
            else
            {
                throw new ArgumentException("Input is not in correct format(long/datetime)");
            }            

            try
            {
                string fmtStr = _dateTimeFormats[format];
                if (String.IsNullOrEmpty(fmtStr))
                {
                    fmtStr = _dateTimeFormats[_displayType];
                }

                culture = new CultureInfo(lang);
                return String.Format(culture, fmtStr, dt);
            }
            catch (Exception e)
            {
                Logging.VLogger.Get().Log(VAdvantage.Logging.Level.SEVERE, e.Message);
                return obj.ToString();
            }
        }
    }



    


    public class MessageFormat
    {

        public MessageFormat(string ptrn)
            : this()
        {
            ApplyPattern(ptrn);

        }
        //"(C) Currency: . . . . . . . . {0:C}\n" +
        //"(D) Decimal:. . . . . . . . . {0:D}\n" +
        //"(E) Scientific: . . . . . . . {1:E}\n" +
        //"(F) Fixed point:. . . . . . . {1:F}\n" +
        //"(G) General:. . . . . . . . . {0:G}\n" +
        //"    (default):. . . . . . . . {0} (default = 'G')\n" +
        //"(N) Number: . . . . . . . . . {0:N}\n" +
        //"(P) Percent:. . . . . . . . . {1:P}\n" +
        //"(R) Round-trip: . . . . . . . {1:R}\n" +
        //"(X) Hexadecimal:. . . . . . . {0:X}\n",
        private Dictionary<string, string> allFormats = new Dictionary<string, string>(15);
        private String[] typeList = { "", "", "number", "", "date", "", "time", "", "choice" };
        private String[] modifierList = { "", "", "currency", "", "percent", "", "integer" };
        private String[] dateModifierList = { "", "", "short", "", "medium", "", "long", "", "full" };

        private string pattern = "";

        public MessageFormat()
        {
            allFormats.Add("currency", "C");
            allFormats.Add("decimal", "D");
            allFormats.Add("general", "G");
            allFormats.Add("number", "N");
            allFormats.Add("percent", "P");
            allFormats.Add("shortdate", "d");
            allFormats.Add("longdate", "D");
            allFormats.Add("shorttime", "t");
            allFormats.Add("longtime", "T");
            allFormats.Add("fulltime", "T");
            allFormats.Add("fulldate", "D");
            allFormats.Add("mediumdate", "d");
            allFormats.Add("mediumtime", "t");
            allFormats.Add("choice", "");
        }


        /// <summary>
        /// return fromarted value
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public string Format(object[] arguments)
        {
            try
            {
                return String.Format(System.Globalization.CultureInfo.InvariantCulture, this.pattern, arguments);
            }
            catch (FormatException e)
            {
                Logging.VLogger.Get().Log(VAdvantage.Logging.Level.WARNING, e.Message);
                return "";
            }
            catch (Exception e)
            {
                Logging.VLogger.Get().Log(VAdvantage.Logging.Level.WARNING, e.Message);
                return "";
            }
        }


        public void ApplyPattern(String pattern)
        {
            if (!CmpreFormat(pattern))
            {
                this.pattern = pattern;
                return;
            }

            List<string> segmentsList = new List<string>();

            StringBuilder segment = new StringBuilder("");
            for (int i = 0; i < pattern.Length; ++i)
            {
                char ch = pattern[i];
                if (ch == '{')
                {
                    if (segment.Length == 0)
                    {
                        segment.Append(ch);
                    }
                    else
                    {
                        segmentsList.Add(segment.ToString());
                        segment.Length = 0;
                        segment.Append(ch);
                    }
                }
                else if (ch == '}')
                {
                    segment.Append(ch);
                    segmentsList.Add(segment.ToString());
                    segment.Length = 0;
                }
                else if (i == pattern.Length)
                {
                    segmentsList.Add(segment.ToString());
                    segment.Length = 0;
                }
                else
                {
                    segment.Append(ch);
                }
            }


            this.pattern = ParsePattren(segmentsList);
        }

        private string ParsePattren(List<string> segments)
        {
            StringBuilder result = new StringBuilder(" ");
            for (int i = 0; i < segments.Count; i++)
            {
                string part = segments[i];
                if (part.StartsWith("{") && CmpreFormat(part))
                {
                    part = GetFormat(part.Split(','));
                    result.Append(part);
                    continue;
                }
                result.Append(part);
            }
            return result.ToString();
        }


        private string GetFormat(string[] segments)
        {
            StringBuilder format = new StringBuilder(segments[0]).Append(":");
            string newFormat = "";
            switch (FindKeyword(segments[1], typeList))
            {
                case 0:
                    break;
                case 1:
                case 2:// number
                    switch (FindKeyword(segments[2].ToString(), modifierList))
                    {
                        case 0: // default;
                            newFormat = allFormats["number"];
                            break;
                        case 1:
                        case 2:// currency
                            newFormat = allFormats["currency"];
                            break;
                        case 3:
                        case 4:// percent
                            newFormat = allFormats["percent"];
                            break;
                        case 5:
                        case 6:// integer
                            newFormat = allFormats["decimal"];
                            break;
                        default: // pattern
                            newFormat = allFormats["number"];
                            break;
                    }
                    break;
                case 3:
                case 4: // date
                    switch (FindKeyword(segments[2], dateModifierList))
                    {
                        case 0: // default
                            newFormat = allFormats["shortdate"];
                            break;
                        case 1:
                        case 2: // short
                            newFormat = allFormats["shortdate"];
                            break;
                        case 3:
                        case 4: // medium
                            newFormat = allFormats["mediumdate"];
                            break;
                        case 5:
                        case 6: // long
                            newFormat = allFormats["longdate"];
                            break;
                        case 7:
                        case 8: // full
                            newFormat = allFormats["fulldate"];
                            break;
                        default:
                            newFormat = segments[3].ToString();
                            break;
                    }
                    break;
                case 5:
                case 6:// time
                    switch (FindKeyword(segments[2].ToString(), dateModifierList))
                    {
                        case 0: // default
                            newFormat = allFormats["shorttime"];
                            break;
                        case 1:
                        case 2: // short
                            newFormat = allFormats["shorttime"];
                            break;
                        case 3:
                        case 4: // medium
                            newFormat = allFormats["mediumtime"];
                            break;
                        case 5:
                        case 6: // long
                            newFormat = allFormats["mediumtime"];
                            break;
                        case 7:
                        case 8: // full
                            newFormat = allFormats["mediumtime"];
                            break;
                        default:
                            newFormat = segments[2].ToString();
                            break;
                    }
                    break;
                case 7:
                case 8:// choice
                    try
                    {
                        newFormat = segments[3];
                        allFormats["choice"] = segments[3].ToString();
                    }
                    catch
                    {
                        throw new ArgumentException(
                                                 "Choice Pattern incorrect");
                    }
                    break;
                default:
                    throw new ArgumentException("unknown format type at ");
            }
            segments = null;
            format.Append(newFormat).Append("}");
            return format.ToString();
        }




        /// <summary>
        /// is it  format or not
        /// </summary>
        /// <param name="ptrn"></param>
        /// <returns></returns>
        private bool CmpreFormat(string ptrn)
        {
            if (ptrn.Contains("number,") || ptrn.Contains("date,") || ptrn.Contains("time,") || ptrn.Contains("custom,"))
            {
                return true;
            }
            return false;
        }

        private int FindKeyword(string text, string[] str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (text.Equals(str.GetValue(i)))
                    return i;
            }
            return -1;
        }
    }
}