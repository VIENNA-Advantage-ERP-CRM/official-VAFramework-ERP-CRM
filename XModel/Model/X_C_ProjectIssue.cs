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
/** Generated Model for VAB_ProjectSupply
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_ProjectSupply : PO
{
public X_VAB_ProjectSupply (Context ctx, int VAB_ProjectSupply_ID, Trx trxName) : base (ctx, VAB_ProjectSupply_ID, trxName)
{
/** if (VAB_ProjectSupply_ID == 0)
{
SetVAB_ProjectSupply_ID (0);
SetVAB_Project_ID (0);
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAB_ProjectSupply WHERE VAB_Project_ID=@VAB_Project_ID@
SetM_Locator_ID (0);
SetM_Product_ID (0);
SetMovementDate (DateTime.Now);
SetMovementQty (0.0);
SetPosted (false);	// N
SetProcessed (false);	// N
}
 */
}
public X_VAB_ProjectSupply (Ctx ctx, int VAB_ProjectSupply_ID, Trx trxName) : base (ctx, VAB_ProjectSupply_ID, trxName)
{
/** if (VAB_ProjectSupply_ID == 0)
{
SetVAB_ProjectSupply_ID (0);
SetVAB_Project_ID (0);
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAB_ProjectSupply WHERE VAB_Project_ID=@VAB_Project_ID@
SetM_Locator_ID (0);
SetM_Product_ID (0);
SetMovementDate (DateTime.Now);
SetMovementQty (0.0);
SetPosted (false);	// N
SetProcessed (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectSupply (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectSupply (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectSupply (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_ProjectSupply()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374256L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057467L;
/** VAF_TableView_ID=623 */
public static int Table_ID;
 // =623;

/** TableName=VAB_ProjectSupply */
public static String Table_Name="VAB_ProjectSupply";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_VAB_ProjectSupply[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Project Issue.
@param VAB_ProjectSupply_ID Project Issues (Material, Labor) */
public void SetVAB_ProjectSupply_ID (int VAB_ProjectSupply_ID)
{
if (VAB_ProjectSupply_ID < 1) throw new ArgumentException ("VAB_ProjectSupply_ID is mandatory.");
Set_ValueNoCheck ("VAB_ProjectSupply_ID", VAB_ProjectSupply_ID);
}
/** Get Project Issue.
@return Project Issues (Material, Labor) */
public int GetVAB_ProjectSupply_ID() 
{
Object ii = Get_Value("VAB_ProjectSupply_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project.
@param VAB_Project_ID Financial Project */
public void SetVAB_Project_ID (int VAB_Project_ID)
{
if (VAB_Project_ID < 1) throw new ArgumentException ("VAB_Project_ID is mandatory.");
Set_ValueNoCheck ("VAB_Project_ID", VAB_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetVAB_Project_ID() 
{
Object ii = Get_Value("VAB_Project_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_Project_ID().ToString());
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
/** Set Line No.
@param Line Unique line for this document */
public void SetLine (int Line)
{
Set_Value ("Line", Line);
}
/** Get Line No.
@return Unique line for this document */
public int GetLine() 
{
Object ii = Get_Value("Line");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Attribute Set Instance.
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
public void SetM_AttributeSetInstance_ID (int M_AttributeSetInstance_ID)
{
if (M_AttributeSetInstance_ID <= 0) Set_Value ("M_AttributeSetInstance_ID", null);
else
Set_Value ("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
}
/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
public int GetM_AttributeSetInstance_ID() 
{
Object ii = Get_Value("M_AttributeSetInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Shipment/Receipt Line.
@param M_InOutLine_ID Line on Shipment or Receipt document */
public void SetM_InOutLine_ID (int M_InOutLine_ID)
{
if (M_InOutLine_ID <= 0) Set_Value ("M_InOutLine_ID", null);
else
Set_Value ("M_InOutLine_ID", M_InOutLine_ID);
}
/** Get Shipment/Receipt Line.
@return Line on Shipment or Receipt document */
public int GetM_InOutLine_ID() 
{
Object ii = Get_Value("M_InOutLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Locator.
@param M_Locator_ID Warehouse Locator */
public void SetM_Locator_ID (int M_Locator_ID)
{
if (M_Locator_ID < 1) throw new ArgumentException ("M_Locator_ID is mandatory.");
Set_Value ("M_Locator_ID", M_Locator_ID);
}
/** Get Locator.
@return Warehouse Locator */
public int GetM_Locator_ID() 
{
Object ii = Get_Value("M_Locator_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID < 1) throw new ArgumentException ("M_Product_ID is mandatory.");
Set_Value ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Movement Date.
@param MovementDate Date a product was moved in or out of inventory */
public void SetMovementDate (DateTime? MovementDate)
{
if (MovementDate == null) throw new ArgumentException ("MovementDate is mandatory.");
Set_Value ("MovementDate", (DateTime?)MovementDate);
}
/** Get Movement Date.
@return Date a product was moved in or out of inventory */
public DateTime? GetMovementDate() 
{
return (DateTime?)Get_Value("MovementDate");
}
/** Set Movement Quantity.
@param MovementQty Quantity of a product moved. */
public void SetMovementQty (Decimal? MovementQty)
{
if (MovementQty == null) throw new ArgumentException ("MovementQty is mandatory.");
Set_Value ("MovementQty", (Decimal?)MovementQty);
}
/** Get Movement Quantity.
@return Quantity of a product moved. */
public Decimal GetMovementQty() 
{
Object bd =Get_Value("MovementQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Posted.
@param Posted Posting status */
public void SetPosted (Boolean Posted)
{
Set_Value ("Posted", Posted);
}
/** Get Posted.
@return Posting status */
public Boolean IsPosted() 
{
Object oo = Get_Value("Posted");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Expense Line.
@param VAS_ExpenseReportLine_ID Time and Expense Report Line */
public void SetVAS_ExpenseReportLine_ID (int VAS_ExpenseReportLine_ID)
{
if (VAS_ExpenseReportLine_ID <= 0) Set_Value ("VAS_ExpenseReportLine_ID", null);
else
Set_Value ("VAS_ExpenseReportLine_ID", VAS_ExpenseReportLine_ID);
}
/** Get Expense Line.
@return Time and Expense Report Line */
public int GetVAS_ExpenseReportLine_ID() 
{
Object ii = Get_Value("VAS_ExpenseReportLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
