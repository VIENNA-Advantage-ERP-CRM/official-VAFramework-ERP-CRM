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
/** Generated Model for C_ResourcePeriod
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_ResourcePeriod : PO
{
public X_C_ResourcePeriod (Context ctx, int C_ResourcePeriod_ID, Trx trxName) : base (ctx, C_ResourcePeriod_ID, trxName)
{
/** if (C_ResourcePeriod_ID == 0)
{
SetC_ResourcePeriod_ID (0);
}
 */
}
public X_C_ResourcePeriod (Ctx ctx, int C_ResourcePeriod_ID, Trx trxName) : base (ctx, C_ResourcePeriod_ID, trxName)
{
/** if (C_ResourcePeriod_ID == 0)
{
SetC_ResourcePeriod_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ResourcePeriod (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ResourcePeriod (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ResourcePeriod (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_ResourcePeriod()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27617312389210L;
/** Last Updated Timestamp 4/23/2012 6:47:52 PM */
public static long updatedMS = 1335187072421L;
/** AD_Table_ID=1000336 */
public static int Table_ID;
 // =1000336;

/** TableName=C_ResourcePeriod */
public static String Table_Name="C_ResourcePeriod";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(7);
/** AccessLevel
@return 7 - System - Client - Org 
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
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_C_ResourcePeriod[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Customer/Prospect Contact. */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);
else
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set C_ResourcePeriod_ID.
@param C_ResourcePeriod_ID C_ResourcePeriod_ID */
public void SetC_ResourcePeriod_ID (int C_ResourcePeriod_ID)
{
if (C_ResourcePeriod_ID < 1) throw new ArgumentException ("C_ResourcePeriod_ID is mandatory.");
Set_ValueNoCheck ("C_ResourcePeriod_ID", C_ResourcePeriod_ID);
}
/** Get C_ResourcePeriod_ID.
@return C_ResourcePeriod_ID */
public int GetC_ResourcePeriod_ID() 
{
Object ii = Get_Value("C_ResourcePeriod_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set From Date.
@param FROMDATE From Date */
public void SetFROMDATE (DateTime? FROMDATE)
{
Set_Value ("FROMDATE", (DateTime?)FROMDATE);
}
/** Get From Date.
@return From Date */
public DateTime? GetFROMDATE() 
{
return (DateTime?)Get_Value("FROMDATE");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetFROMDATE().ToString());
}
/** Set General Expense Report.
@param GeneralExpenseReport General Expense Report */
public void SetGeneralExpenseReport (String GeneralExpenseReport)
{
if (GeneralExpenseReport != null && GeneralExpenseReport.Length > 1)
{
log.Warning("Length > 1 - truncated");
GeneralExpenseReport = GeneralExpenseReport.Substring(0,1);
}
Set_Value ("GeneralExpenseReport", GeneralExpenseReport);
}
/** Get General Expense Report.
@return General Expense Report */
public String GetGeneralExpenseReport() 
{
return (String)Get_Value("GeneralExpenseReport");
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
}
/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed() 
{
Object oo = Get_Value("Processed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Remarks.
@param Remarks Remarks */
public void SetRemarks (String Remarks)
{
if (Remarks != null && Remarks.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Remarks = Remarks.Substring(0,2000);
}
Set_Value ("Remarks", Remarks);
}
/** Get Remarks.
@return Remarks */
public String GetRemarks() 
{
return (String)Get_Value("Remarks");
}
/** Set ToDate.
@param ToDate ToDate */
public void SetToDate (DateTime? ToDate)
{
Set_Value ("ToDate", (DateTime?)ToDate);
}
/** Get ToDate.
@return ToDate */
public DateTime? GetToDate() 
{
return (DateTime?)Get_Value("ToDate");
}
}

}
