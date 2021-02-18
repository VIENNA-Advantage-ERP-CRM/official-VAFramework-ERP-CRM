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
/** Generated Model for VAB_SLevelCriteria
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_SLevelCriteria : PO
{
public X_VAB_SLevelCriteria (Context ctx, int VAB_SLevelCriteria_ID, Trx trxName) : base (ctx, VAB_SLevelCriteria_ID, trxName)
{
/** if (VAB_SLevelCriteria_ID == 0)
{
SetVAB_Rev_RecognitionStrtgy_ID (0);
SetVAB_SLevelCriteria_ID (0);
SetVAM_Product_ID (0);
SetServiceLevelInvoiced (0.0);
SetServiceLevelProvided (0.0);
}
 */
}
public X_VAB_SLevelCriteria (Ctx ctx, int VAB_SLevelCriteria_ID, Trx trxName) : base (ctx, VAB_SLevelCriteria_ID, trxName)
{
/** if (VAB_SLevelCriteria_ID == 0)
{
SetVAB_Rev_RecognitionStrtgy_ID (0);
SetVAB_SLevelCriteria_ID (0);
SetVAM_Product_ID (0);
SetServiceLevelInvoiced (0.0);
SetServiceLevelProvided (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_SLevelCriteria (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_SLevelCriteria (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_SLevelCriteria (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_SLevelCriteria()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375118L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058329L;
/** VAF_TableView_ID=337 */
public static int Table_ID;
 // =337;

/** TableName=VAB_SLevelCriteria */
public static String Table_Name="VAB_SLevelCriteria";

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
StringBuilder sb = new StringBuilder ("X_VAB_SLevelCriteria[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Revenue Recognition Plan.
@param VAB_Rev_RecognitionStrtgy_ID Plan for recognizing or recording revenue */
public void SetVAB_Rev_RecognitionStrtgy_ID (int VAB_Rev_RecognitionStrtgy_ID)
{
if (VAB_Rev_RecognitionStrtgy_ID < 1) throw new ArgumentException ("VAB_Rev_RecognitionStrtgy_ID is mandatory.");
Set_ValueNoCheck ("VAB_Rev_RecognitionStrtgy_ID", VAB_Rev_RecognitionStrtgy_ID);
}
/** Get Revenue Recognition Plan.
@return Plan for recognizing or recording revenue */
public int GetVAB_Rev_RecognitionStrtgy_ID() 
{
Object ii = Get_Value("VAB_Rev_RecognitionStrtgy_ID");
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetDescription());
}
/** Set Product.
@param VAM_Product_ID Product, Service, Item */
public void SetVAM_Product_ID (int VAM_Product_ID)
{
if (VAM_Product_ID < 1) throw new ArgumentException ("VAM_Product_ID is mandatory.");
Set_ValueNoCheck ("VAM_Product_ID", VAM_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetVAM_Product_ID() 
{
Object ii = Get_Value("VAM_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Quantity Invoiced.
@param ServiceLevelInvoiced Quantity of product or service invoiced */
public void SetServiceLevelInvoiced (Decimal? ServiceLevelInvoiced)
{
if (ServiceLevelInvoiced == null) throw new ArgumentException ("ServiceLevelInvoiced is mandatory.");
Set_ValueNoCheck ("ServiceLevelInvoiced", (Decimal?)ServiceLevelInvoiced);
}
/** Get Quantity Invoiced.
@return Quantity of product or service invoiced */
public Decimal GetServiceLevelInvoiced() 
{
Object bd =Get_Value("ServiceLevelInvoiced");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
