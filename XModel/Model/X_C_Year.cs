namespace VAdvantage.Model
{

/** Generated Model - DO NOT CHANGE */
using System;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
/** Generated Model for C_Year
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_Year : PO
{
public X_C_Year (Context ctx, int C_Year_ID, Trx trxName) : base (ctx, C_Year_ID, trxName)
{
/** if (C_Year_ID == 0)
{
SetC_Calendar_ID (0);
SetC_Year_ID (0);
SetFiscalYear (null);
}
 */
}
public X_C_Year (Ctx ctx, int C_Year_ID, Trx trxName) : base (ctx, C_Year_ID, trxName)
{
/** if (C_Year_ID == 0)
{
SetC_Calendar_ID (0);
SetC_Year_ID (0);
SetFiscalYear (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Year (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Year (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Year (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_Year()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375933L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059144L;
/** AD_Table_ID=177 */
public static int Table_ID;
 // =177;

/** TableName=C_Year */
public static String Table_Name="C_Year";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
*/
protected override int Get_AccessLevel()
{
return Convert.ToInt32(accessLevel.ToString());
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO(Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_C_Year[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Calendar.
@param C_Calendar_ID Accounting Calendar Name */
public void SetC_Calendar_ID (int C_Calendar_ID)
{
if (C_Calendar_ID < 1) throw new ArgumentException ("C_Calendar_ID is mandatory.");
Set_ValueNoCheck ("C_Calendar_ID", C_Calendar_ID);
}
/** Get Calendar.
@return Accounting Calendar Name */
public int GetC_Calendar_ID() 
{
Object ii = Get_Value("C_Calendar_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Year.
@param C_Year_ID Calendar Year */
public void SetC_Year_ID (int C_Year_ID)
{
if (C_Year_ID < 1) throw new ArgumentException ("C_Year_ID is mandatory.");
Set_ValueNoCheck ("C_Year_ID", C_Year_ID);
}
/** Get Year.
@return Calendar Year */
public int GetC_Year_ID() 
{
Object ii = Get_Value("C_Year_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Year.
@param FiscalYear The Fiscal Year */
public void SetFiscalYear (String FiscalYear)
{
if (FiscalYear == null) throw new ArgumentException ("FiscalYear is mandatory.");
if (FiscalYear.Length > 10)
{
log.Warning("Length > 10 - truncated");
FiscalYear = FiscalYear.Substring(0,10);
}
Set_Value ("FiscalYear", FiscalYear);
}
/** Get Year.
@return The Fiscal Year */
public String GetFiscalYear() 
{
return (String)Get_Value("FiscalYear");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetFiscalYear());
}
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
        /** CalendarYears AD_Reference_ID=1000188 */
        public static int CALENDARYEARS_AD_Reference_ID = 1000188;/** 1975 = 1975 */
        public static String CALENDARYEARS_1975 = "1975";/** 1976 = 1976 */
        public static String CALENDARYEARS_1976 = "1976";/** 1977 = 1977 */
        public static String CALENDARYEARS_1977 = "1977";/** 1978 = 1978 */
        public static String CALENDARYEARS_1978 = "1978";/** 1979 = 1979 */
        public static String CALENDARYEARS_1979 = "1979";/** 1980 = 1980 */
        public static String CALENDARYEARS_1980 = "1980";/** 1981 = 1981 */
        public static String CALENDARYEARS_1981 = "1981";/** 1982 = 1982 */
        public static String CALENDARYEARS_1982 = "1982";/** 1983 = 1983 */
        public static String CALENDARYEARS_1983 = "1983";/** 1984 = 1984 */
        public static String CALENDARYEARS_1984 = "1984";/** 1985 = 1985 */
        public static String CALENDARYEARS_1985 = "1985";/** 1986 = 1986 */
        public static String CALENDARYEARS_1986 = "1986";/** 1987 = 1987 */
        public static String CALENDARYEARS_1987 = "1987";/** 1988 = 1988 */
        public static String CALENDARYEARS_1988 = "1988";/** 1989 = 1989 */
        public static String CALENDARYEARS_1989 = "1989";/** 1990 = 1990 */
        public static String CALENDARYEARS_1990 = "1990";/** 1991 = 1991 */
        public static String CALENDARYEARS_1991 = "1991";/** 1992 = 1992 */
        public static String CALENDARYEARS_1992 = "1992";/** 1993 = 1993 */
        public static String CALENDARYEARS_1993 = "1993";/** 1994 = 1994 */
        public static String CALENDARYEARS_1994 = "1994";/** 1995 = 1995 */
        public static String CALENDARYEARS_1995 = "1995";/** 1996 = 1996 */
        public static String CALENDARYEARS_1996 = "1996";/** 1997 = 1997 */
        public static String CALENDARYEARS_1997 = "1997";/** 1998 = 1998 */
        public static String CALENDARYEARS_1998 = "1998";/** 1999 = 1999 */
        public static String CALENDARYEARS_1999 = "1999";/** 2000 = 2000 */
        public static String CALENDARYEARS_2000 = "2000";/** 2001 = 2001 */
        public static String CALENDARYEARS_2001 = "2001";/** 2002 = 2002 */
        public static String CALENDARYEARS_2002 = "2002";/** 2003 = 2003 */
        public static String CALENDARYEARS_2003 = "2003";/** 2004 = 2004 */
        public static String CALENDARYEARS_2004 = "2004";/** 2005 = 2005 */
        public static String CALENDARYEARS_2005 = "2005";/** 2006 = 2006 */
        public static String CALENDARYEARS_2006 = "2006";/** 2007 = 2007 */
        public static String CALENDARYEARS_2007 = "2007";/** 2008 = 2008 */
        public static String CALENDARYEARS_2008 = "2008";/** 2009 = 2009 */
        public static String CALENDARYEARS_2009 = "2009";/** 2010 = 2010 */
        public static String CALENDARYEARS_2010 = "2010";/** 2011 = 2011 */
        public static String CALENDARYEARS_2011 = "2011";/** 2013 = 2013 */
        public static String CALENDARYEARS_2013 = "2013";/** 2014 = 2014 */
        public static String CALENDARYEARS_2014 = "2014";/** 2015 = 2015 */
        public static String CALENDARYEARS_2015 = "2015";/** 2017 = 2017 */
        public static String CALENDARYEARS_2017 = "2017";/** 2018 = 2018 */
        public static String CALENDARYEARS_2018 = "2018";/** 2019 = 2019 */
        public static String CALENDARYEARS_2019 = "2019";/** 2021 = 2021 */
        public static String CALENDARYEARS_2021 = "2021";/** 2022 = 2022 */
        public static String CALENDARYEARS_2022 = "2022";/** 2024 = 2024 */
        public static String CALENDARYEARS_2024 = "2024";/** 2025 = 2025 */
        public static String CALENDARYEARS_2025 = "2025";/** 2026 = 2026 */
        public static String CALENDARYEARS_2026 = "2026";/** 2028 = 2028 */
        public static String CALENDARYEARS_2028 = "2028";/** 2029 = 2029 */
        public static String CALENDARYEARS_2029 = "2029";/** 2030 = 2030 */
        public static String CALENDARYEARS_2030 = "2030";/** 2032 = 2032 */
        public static String CALENDARYEARS_2032 = "2032";/** 2033 = 2033 */
        public static String CALENDARYEARS_2033 = "2033";/** 2034 = 2034 */
        public static String CALENDARYEARS_2034 = "2034";/** 2036 = 2036 */
        public static String CALENDARYEARS_2036 = "2036";/** 2037 = 2037 */
        public static String CALENDARYEARS_2037 = "2037";/** 2039 = 2039 */
        public static String CALENDARYEARS_2039 = "2039";/** 2040 = 2040 */
        public static String CALENDARYEARS_2040 = "2040";/** 2041 = 2041 */
        public static String CALENDARYEARS_2041 = "2041";/** 2043 = 2043 */
        public static String CALENDARYEARS_2043 = "2043";/** 2044 = 2044 */
        public static String CALENDARYEARS_2044 = "2044";/** 2045 = 2045 */
        public static String CALENDARYEARS_2045 = "2045";/** 2047 = 2047 */
        public static String CALENDARYEARS_2047 = "2047";/** 2048 = 2048 */
        public static String CALENDARYEARS_2048 = "2048";/** 2049 = 2049 */
        public static String CALENDARYEARS_2049 = "2049";/** 2050 = 2050 */
        public static String CALENDARYEARS_2050 = "2050";/** 2051 = 2051 */
        public static String CALENDARYEARS_2051 = "2051";/** 2052 = 2052 */
        public static String CALENDARYEARS_2052 = "2052";/** 2053 = 2053 */
        public static String CALENDARYEARS_2053 = "2053";/** 2054 = 2054 */
        public static String CALENDARYEARS_2054 = "2054";/** 2055 = 2055 */
        public static String CALENDARYEARS_2055 = "2055";/** 2056 = 2056 */
        public static String CALENDARYEARS_2056 = "2056";/** 2057 = 2057 */
        public static String CALENDARYEARS_2057 = "2057";/** 2058 = 2058 */
        public static String CALENDARYEARS_2058 = "2058";/** 2059 = 2059 */
        public static String CALENDARYEARS_2059 = "2059";/** 2060 = 2060 */
        public static String CALENDARYEARS_2060 = "2060";/** 2061 = 2061 */
        public static String CALENDARYEARS_2061 = "2061";/** 2062 = 2062 */
        public static String CALENDARYEARS_2062 = "2062";/** 2063 = 2063 */
        public static String CALENDARYEARS_2063 = "2063";/** 2064 = 2064 */
        public static String CALENDARYEARS_2064 = "2064";/** 2065 = 2065 */
        public static String CALENDARYEARS_2065 = "2065";/** 2066 = 2066 */
        public static String CALENDARYEARS_2066 = "2066";/** 2067 = 2067 */
        public static String CALENDARYEARS_2067 = "2067";/** 2068 = 2068 */
        public static String CALENDARYEARS_2068 = "2068";/** 2069 = 2069 */
        public static String CALENDARYEARS_2069 = "2069";/** 2070 = 2070 */
        public static String CALENDARYEARS_2070 = "2070";/** 2071 = 2071 */
        public static String CALENDARYEARS_2071 = "2071";/** 2072 = 2072 */
        public static String CALENDARYEARS_2072 = "2072";/** 2073 = 2073 */
        public static String CALENDARYEARS_2073 = "2073";/** 2074 = 2074 */
        public static String CALENDARYEARS_2074 = "2074";/** 2075 = 2075 */
        public static String CALENDARYEARS_2075 = "2075";/** 2016 = 2016 */
        public static String CALENDARYEARS_2016 = "2016";/** 2020 = 2020 */
        public static String CALENDARYEARS_2020 = "2020";/** 2023 = 2023 */
        public static String CALENDARYEARS_2023 = "2023";/** 2027 = 2027 */
        public static String CALENDARYEARS_2027 = "2027";/** 2031 = 2031 */
        public static String CALENDARYEARS_2031 = "2031";/** 2035 = 2035 */
        public static String CALENDARYEARS_2035 = "2035";/** 2038 = 2038 */
        public static String CALENDARYEARS_2038 = "2038";/** 2042 = 2042 */
        public static String CALENDARYEARS_2042 = "2042";/** 2046 = 2046 */
        public static String CALENDARYEARS_2046 = "2046";/** 2012 = 2012 */
        public static String CALENDARYEARS_2012 = "2012";

        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsCalendarYearsValid(String test) { return test == null || test.Equals("1975") || test.Equals("1976") || test.Equals("1977") || test.Equals("1978") 
                || test.Equals("1979") || test.Equals("1980") || test.Equals("1981") || test.Equals("1982") || test.Equals("1983") || test.Equals("1984") || test.Equals("1985") 
                || test.Equals("1986") || test.Equals("1987") || test.Equals("1988") || test.Equals("1989") || test.Equals("1990") || test.Equals("1991") || test.Equals("1992") 
                || test.Equals("1993") || test.Equals("1994") || test.Equals("1995") || test.Equals("1996") || test.Equals("1997") || test.Equals("1998") || test.Equals("1999") 
                || test.Equals("2000") || test.Equals("2001") || test.Equals("2002") || test.Equals("2003") || test.Equals("2004") || test.Equals("2005") || test.Equals("2006") 
                || test.Equals("2007") || test.Equals("2008") || test.Equals("2009") || test.Equals("2010") || test.Equals("2011") || test.Equals("2013") || test.Equals("2014") 
                || test.Equals("2015") || test.Equals("2017") || test.Equals("2018") || test.Equals("2019") || test.Equals("2021") || test.Equals("2022") || test.Equals("2024") 
                || test.Equals("2025") || test.Equals("2026") || test.Equals("2028") || test.Equals("2029") || test.Equals("2030") || test.Equals("2032") || test.Equals("2033") 
                || test.Equals("2034") || test.Equals("2036") || test.Equals("2037") || test.Equals("2039") || test.Equals("2040") || test.Equals("2041") || test.Equals("2043") 
                || test.Equals("2044") || test.Equals("2045") || test.Equals("2047") || test.Equals("2048") || test.Equals("2049") || test.Equals("2050") || test.Equals("2051") 
                || test.Equals("2052") || test.Equals("2053") || test.Equals("2054") || test.Equals("2055") || test.Equals("2056") || test.Equals("2057") || test.Equals("2058") 
                || test.Equals("2059") || test.Equals("2060") || test.Equals("2061") || test.Equals("2062") || test.Equals("2063") || test.Equals("2064") || test.Equals("2065") 
                || test.Equals("2066") || test.Equals("2067") || test.Equals("2068") || test.Equals("2069") || test.Equals("2070") || test.Equals("2071") || test.Equals("2072") 
                || test.Equals("2073") || test.Equals("2074") || test.Equals("2075") || test.Equals("2016") || test.Equals("2020") || test.Equals("2023") || test.Equals("2027") 
                || test.Equals("2031") || test.Equals("2035") || test.Equals("2038") || test.Equals("2042") || test.Equals("2046") || test.Equals("2012"); }

        /** Set Calendar Year.
        @param CalendarYears Calendar Year */
        public void SetCalendarYears(String CalendarYears)
        {
            if (!IsCalendarYearsValid(CalendarYears))
                throw new ArgumentException("CalendarYears Invalid value - " + CalendarYears + " - Reference_ID=1000188 - 1975 - 1976 - 1977 - 1978 - 1979 - 1980 - 1981 - 1982 - 1983 - 1984 - 1985 - 1986 - 1987 - 1988 - 1989 - 1990 - 1991 - 1992 - 1993 - 1994 - 1995 - 1996 - 1997 - 1998 - 1999 - 2000 - 2001 - 2002 - 2003 - 2004 - 2005 - 2006 - 2007 - 2008 - 2009 - 2010 - 2011 - 2013 - 2014 - 2015 - 2017 - 2018 - 2019 - 2021 - 2022 - 2024 - 2025 - 2026 - 2028 - 2029 - 2030 - 2032 - 2033 - 2034 - 2036 - 2037 - 2039 - 2040 - 2041 - 2043 - 2044 - 2045 - 2047 - 2048 - 2049 - 2050 - 2051 - 2052 - 2053 - 2054 - 2055 - 2056 - 2057 - 2058 - 2059 - 2060 - 2061 - 2062 - 2063 - 2064 - 2065 - 2066 - 2067 - 2068 - 2069 - 2070 - 2071 - 2072 - 2073 - 2074 - 2075 - 2016 - 2020 - 2023 - 2027 - 2031 - 2035 - 2038 - 2042 - 2046 - 2012");
            if (CalendarYears != null && CalendarYears.Length > 4) 
            { 
                log.Warning("Length > 4 - truncated"); 
                CalendarYears = CalendarYears.Substring(0, 4); 
            }
            Set_Value("CalendarYears", CalendarYears);

        }
        
        /** Get Calendar Year.
        @return Calendar Year */
        public String GetCalendarYears() 
        { 
            return (String)Get_Value("CalendarYears"); 
        }
    }

}
