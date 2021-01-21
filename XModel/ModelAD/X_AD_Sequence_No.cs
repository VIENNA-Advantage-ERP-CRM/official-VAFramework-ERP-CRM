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
/** Generated Model for VAF_Record_Seq_No
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_Record_Seq_No : PO
{
public X_VAF_Record_Seq_No (Context ctx, int VAF_Record_Seq_No_ID, Trx trxName) : base (ctx, VAF_Record_Seq_No_ID, trxName)
{
/** if (VAF_Record_Seq_No_ID == 0)
{
SetVAF_Record_Seq_ID (0);
SetCalendarYear (null);
SetCurrentNext (0);
}
 */
}
public X_VAF_Record_Seq_No (Ctx ctx, int VAF_Record_Seq_No_ID, Trx trxName) : base (ctx, VAF_Record_Seq_No_ID, trxName)
{
/** if (VAF_Record_Seq_No_ID == 0)
{
SetVAF_Record_Seq_ID (0);
SetCalendarYear (null);
SetCurrentNext (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Record_Seq_No (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Record_Seq_No (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Record_Seq_No (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_Record_Seq_No()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514364147L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047358L;
/** VAF_TableView_ID=122 */
public static int Table_ID;
 // =122;

/** TableName=VAF_Record_Seq_No */
public static String Table_Name="VAF_Record_Seq_No";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(6);
/** AccessLevel
@return 6 - System - Client 
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
StringBuilder sb = new StringBuilder ("X_VAF_Record_Seq_No[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Sequence.
@param VAF_Record_Seq_ID Document Sequence */
public void SetVAF_Record_Seq_ID (int VAF_Record_Seq_ID)
{
if (VAF_Record_Seq_ID < 1) throw new ArgumentException ("VAF_Record_Seq_ID is mandatory.");
Set_ValueNoCheck ("VAF_Record_Seq_ID", VAF_Record_Seq_ID);
}
/** Get Sequence.
@return Document Sequence */
public int GetVAF_Record_Seq_ID() 
{
Object ii = Get_Value("VAF_Record_Seq_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_Record_Seq_ID().ToString());
}
/** Set Year & Month.
@param CalendarYear Calendar Year & Month */
public void SetCalendarYear(String CalendarYear)
{
    if (CalendarYear == null)
        throw new ArgumentException("CalendarYear is mandatory.");
    if (CalendarYear.Length > 6)
    {
        log.Warning("Length > 6 - truncated");
        CalendarYear = CalendarYear.Substring(0, 6);
    }
    Set_ValueNoCheck("CalendarYear", CalendarYear);
}
/** Get Year & Month.
@return Calendar Year & Month */
public String GetCalendarYear()
{
    return (String)Get_Value("CalendarYear");
}
        /** Set Current Next.
@param CurrentNext The next number to be used */
        public void SetCurrentNext (int CurrentNext)
{
Set_Value ("CurrentNext", CurrentNext);
}
/** Get Current Next.
@return The next number to be used */
public int GetCurrentNext() 
{
Object ii = Get_Value("CurrentNext");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
