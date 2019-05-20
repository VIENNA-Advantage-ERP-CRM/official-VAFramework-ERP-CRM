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
/** Generated Model for R_RequestUpdate
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_R_RequestUpdate : PO
{
public X_R_RequestUpdate (Context ctx, int R_RequestUpdate_ID, Trx trxName) : base (ctx, R_RequestUpdate_ID, trxName)
{
/** if (R_RequestUpdate_ID == 0)
{
SetConfidentialTypeEntry (null);
SetR_RequestUpdate_ID (0);
SetR_Request_ID (0);
}
 */
}
public X_R_RequestUpdate (Ctx ctx, int R_RequestUpdate_ID, Trx trxName) : base (ctx, R_RequestUpdate_ID, trxName)
{
/** if (R_RequestUpdate_ID == 0)
{
SetConfidentialTypeEntry (null);
SetR_RequestUpdate_ID (0);
SetR_Request_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_RequestUpdate (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_RequestUpdate (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_RequestUpdate (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_R_RequestUpdate()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383362L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066573L;
/** AD_Table_ID=802 */
public static int Table_ID;
 // =802;

/** TableName=R_RequestUpdate */
public static String Table_Name="R_RequestUpdate";

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
StringBuilder sb = new StringBuilder ("X_R_RequestUpdate[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** ConfidentialTypeEntry AD_Reference_ID=340 */
public static int CONFIDENTIALTYPEENTRY_AD_Reference_ID=340;
/** Public Information = A */
public static String CONFIDENTIALTYPEENTRY_PublicInformation = "A";
/** Partner Confidential = C */
public static String CONFIDENTIALTYPEENTRY_PartnerConfidential = "C";
/** Internal = I */
public static String CONFIDENTIALTYPEENTRY_Internal = "I";
/** Private Information = P */
public static String CONFIDENTIALTYPEENTRY_PrivateInformation = "P";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsConfidentialTypeEntryValid (String test)
{
return test.Equals("A") || test.Equals("C") || test.Equals("I") || test.Equals("P");
}
/** Set Entry Access Level.
@param ConfidentialTypeEntry Confidentiality of the individual entry */
public void SetConfidentialTypeEntry (String ConfidentialTypeEntry)
{
if (ConfidentialTypeEntry == null) throw new ArgumentException ("ConfidentialTypeEntry is mandatory");
if (!IsConfidentialTypeEntryValid(ConfidentialTypeEntry))
throw new ArgumentException ("ConfidentialTypeEntry Invalid value - " + ConfidentialTypeEntry + " - Reference_ID=340 - A - C - I - P");
if (ConfidentialTypeEntry.Length > 1)
{
log.Warning("Length > 1 - truncated");
ConfidentialTypeEntry = ConfidentialTypeEntry.Substring(0,1);
}
Set_Value ("ConfidentialTypeEntry", ConfidentialTypeEntry);
}
/** Get Entry Access Level.
@return Confidentiality of the individual entry */
public String GetConfidentialTypeEntry() 
{
return (String)Get_Value("ConfidentialTypeEntry");
}
/** Set End Time.
@param EndTime End of the time span */
public void SetEndTime (DateTime? EndTime)
{
Set_Value ("EndTime", (DateTime?)EndTime);
}
/** Get End Time.
@return End of the time span */
public DateTime? GetEndTime() 
{
return (DateTime?)Get_Value("EndTime");
}

/** M_ProductSpent_ID AD_Reference_ID=162 */
public static int M_PRODUCTSPENT_ID_AD_Reference_ID=162;
/** Set Product Used.
@param M_ProductSpent_ID Product/Resource/Service used in Request */
public void SetM_ProductSpent_ID (int M_ProductSpent_ID)
{
if (M_ProductSpent_ID <= 0) Set_Value ("M_ProductSpent_ID", null);
else
Set_Value ("M_ProductSpent_ID", M_ProductSpent_ID);
}
/** Get Product Used.
@return Product/Resource/Service used in Request */
public int GetM_ProductSpent_ID() 
{
Object ii = Get_Value("M_ProductSpent_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Quantity Invoiced.
@param QtyInvoiced Invoiced Quantity */
public void SetQtyInvoiced (Decimal? QtyInvoiced)
{
Set_Value ("QtyInvoiced", (Decimal?)QtyInvoiced);
}
/** Get Quantity Invoiced.
@return Invoiced Quantity */
public Decimal GetQtyInvoiced() 
{
Object bd =Get_Value("QtyInvoiced");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Quantity Used.
@param QtySpent Quantity used for this event */
public void SetQtySpent (Decimal? QtySpent)
{
Set_Value ("QtySpent", (Decimal?)QtySpent);
}
/** Get Quantity Used.
@return Quantity used for this event */
public Decimal GetQtySpent() 
{
Object bd =Get_Value("QtySpent");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Request Update.
@param R_RequestUpdate_ID Request Updates */
public void SetR_RequestUpdate_ID (int R_RequestUpdate_ID)
{
if (R_RequestUpdate_ID < 1) throw new ArgumentException ("R_RequestUpdate_ID is mandatory.");
Set_ValueNoCheck ("R_RequestUpdate_ID", R_RequestUpdate_ID);
}
/** Get Request Update.
@return Request Updates */
public int GetR_RequestUpdate_ID() 
{
Object ii = Get_Value("R_RequestUpdate_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetR_RequestUpdate_ID().ToString());
}
/** Set Request.
@param R_Request_ID Request from a Business Partner or Prospect */
public void SetR_Request_ID (int R_Request_ID)
{
if (R_Request_ID < 1) throw new ArgumentException ("R_Request_ID is mandatory.");
Set_ValueNoCheck ("R_Request_ID", R_Request_ID);
}
/** Get Request.
@return Request from a Business Partner or Prospect */
public int GetR_Request_ID() 
{
Object ii = Get_Value("R_Request_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Result.
@param Result Result of the action taken */
public void SetResult (String Result)
{
if (Result != null && Result.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Result = Result.Substring(0,2000);
}
Set_ValueNoCheck ("Result", Result);
}
/** Get Result.
@return Result of the action taken */
public String GetResult() 
{
return (String)Get_Value("Result");
}
/** Set Start Time.
@param StartTime Time started */
public void SetStartTime (DateTime? StartTime)
{
Set_Value ("StartTime", (DateTime?)StartTime);
}
/** Get Start Time.
@return Time started */
public DateTime? GetStartTime() 
{
return (DateTime?)Get_Value("StartTime");
}
}

}
