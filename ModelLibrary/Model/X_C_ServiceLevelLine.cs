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
/** Generated Model for C_ServiceLevelLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_ServiceLevelLine : PO
{
public X_C_ServiceLevelLine (Context ctx, int C_ServiceLevelLine_ID, Trx trxName) : base (ctx, C_ServiceLevelLine_ID, trxName)
{
/** if (C_ServiceLevelLine_ID == 0)
{
SetC_ServiceLevelLine_ID (0);
SetC_ServiceLevel_ID (0);
SetServiceDate (DateTime.Now);
SetServiceLevelProvided (0.0);
}
 */
}
public X_C_ServiceLevelLine (Ctx ctx, int C_ServiceLevelLine_ID, Trx trxName) : base (ctx, C_ServiceLevelLine_ID, trxName)
{
/** if (C_ServiceLevelLine_ID == 0)
{
SetC_ServiceLevelLine_ID (0);
SetC_ServiceLevel_ID (0);
SetServiceDate (DateTime.Now);
SetServiceLevelProvided (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ServiceLevelLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ServiceLevelLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ServiceLevelLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_ServiceLevelLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375196L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058407L;
/** AD_Table_ID=338 */
public static int Table_ID;
 // =338;

/** TableName=C_ServiceLevelLine */
public static String Table_Name="C_ServiceLevelLine";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(1);
/** AccessLevel
@return 1 - Org 
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
StringBuilder sb = new StringBuilder ("X_C_ServiceLevelLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Service Level Line.
@param C_ServiceLevelLine_ID Product Revenue Recognition Service Level Line */
public void SetC_ServiceLevelLine_ID (int C_ServiceLevelLine_ID)
{
if (C_ServiceLevelLine_ID < 1) throw new ArgumentException ("C_ServiceLevelLine_ID is mandatory.");
Set_ValueNoCheck ("C_ServiceLevelLine_ID", C_ServiceLevelLine_ID);
}
/** Get Service Level Line.
@return Product Revenue Recognition Service Level Line */
public int GetC_ServiceLevelLine_ID() 
{
Object ii = Get_Value("C_ServiceLevelLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Service Level.
@param C_ServiceLevel_ID Product Revenue Recognition Service Level */
public void SetC_ServiceLevel_ID (int C_ServiceLevel_ID)
{
if (C_ServiceLevel_ID < 1) throw new ArgumentException ("C_ServiceLevel_ID is mandatory.");
Set_ValueNoCheck ("C_ServiceLevel_ID", C_ServiceLevel_ID);
}
/** Get Service Level.
@return Product Revenue Recognition Service Level */
public int GetC_ServiceLevel_ID() 
{
Object ii = Get_Value("C_ServiceLevel_ID");
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
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_ValueNoCheck ("Processed", Processed);
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
/** Set Service date.
@param ServiceDate Date service was provided */
public void SetServiceDate (DateTime? ServiceDate)
{
if (ServiceDate == null) throw new ArgumentException ("ServiceDate is mandatory.");
Set_ValueNoCheck ("ServiceDate", (DateTime?)ServiceDate);
}
/** Get Service date.
@return Date service was provided */
public DateTime? GetServiceDate() 
{
return (DateTime?)Get_Value("ServiceDate");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetServiceDate().ToString());
}
/** Set Quantity Provided.
@param ServiceLevelProvided Quantity of service or product provided */
public void SetServiceLevelProvided (Decimal? ServiceLevelProvided)
{
if (ServiceLevelProvided == null) throw new ArgumentException ("ServiceLevelProvided is mandatory.");
Set_ValueNoCheck ("ServiceLevelProvided", (Decimal?)ServiceLevelProvided);
}
/** Get Quantity Provided.
@return Quantity of service or product provided */
public Decimal GetServiceLevelProvided() 
{
Object bd =Get_Value("ServiceLevelProvided");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
