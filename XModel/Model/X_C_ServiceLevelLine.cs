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
/** Generated Model for VAB_SLevelCriteriaLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_SLevelCriteriaLine : PO
{
public X_VAB_SLevelCriteriaLine (Context ctx, int VAB_SLevelCriteriaLine_ID, Trx trxName) : base (ctx, VAB_SLevelCriteriaLine_ID, trxName)
{
/** if (VAB_SLevelCriteriaLine_ID == 0)
{
SetVAB_SLevelCriteriaLine_ID (0);
SetVAB_SLevelCriteria_ID (0);
SetServiceDate (DateTime.Now);
SetServiceLevelProvided (0.0);
}
 */
}
public X_VAB_SLevelCriteriaLine (Ctx ctx, int VAB_SLevelCriteriaLine_ID, Trx trxName) : base (ctx, VAB_SLevelCriteriaLine_ID, trxName)
{
/** if (VAB_SLevelCriteriaLine_ID == 0)
{
SetVAB_SLevelCriteriaLine_ID (0);
SetVAB_SLevelCriteria_ID (0);
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
public X_VAB_SLevelCriteriaLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_SLevelCriteriaLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_SLevelCriteriaLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_SLevelCriteriaLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375196L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058407L;
/** VAF_TableView_ID=338 */
public static int Table_ID;
 // =338;

/** TableName=VAB_SLevelCriteriaLine */
public static String Table_Name="VAB_SLevelCriteriaLine";

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
StringBuilder sb = new StringBuilder ("X_VAB_SLevelCriteriaLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Service Level Line.
@param VAB_SLevelCriteriaLine_ID Product Revenue Recognition Service Level Line */
public void SetVAB_SLevelCriteriaLine_ID (int VAB_SLevelCriteriaLine_ID)
{
if (VAB_SLevelCriteriaLine_ID < 1) throw new ArgumentException ("VAB_SLevelCriteriaLine_ID is mandatory.");
Set_ValueNoCheck ("VAB_SLevelCriteriaLine_ID", VAB_SLevelCriteriaLine_ID);
}
/** Get Service Level Line.
@return Product Revenue Recognition Service Level Line */
public int GetVAB_SLevelCriteriaLine_ID() 
{
Object ii = Get_Value("VAB_SLevelCriteriaLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Service Level.
@param VAB_SLevelCriteria_ID Product Revenue Recognition Service Level */
public void SetVAB_SLevelCriteria_ID (int VAB_SLevelCriteria_ID)
{
if (VAB_SLevelCriteria_ID < 1) throw new ArgumentException ("VAB_SLevelCriteria_ID is mandatory.");
Set_ValueNoCheck ("VAB_SLevelCriteria_ID", VAB_SLevelCriteria_ID);
}
/** Get Service Level.
@return Product Revenue Recognition Service Level */
public int GetVAB_SLevelCriteria_ID() 
{
Object ii = Get_Value("VAB_SLevelCriteria_ID");
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
