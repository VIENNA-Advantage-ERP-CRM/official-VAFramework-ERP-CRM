namespace VAdvantage.Model
{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for C_RfQLine
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_RfQLine : PO{public X_C_RfQLine (Context ctx, int C_RfQLine_ID, Trx trxName) : base (ctx, C_RfQLine_ID, trxName){/** if (C_RfQLine_ID == 0){SetC_RfQLine_ID (0);SetC_RfQ_ID (0);SetLine (0);// @SQL=SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM C_RfQLine WHERE C_RfQ_ID=@C_RfQ_ID@
SetProcessed (false);} */
}public X_C_RfQLine (Ctx ctx, int C_RfQLine_ID, Trx trxName) : base (ctx, C_RfQLine_ID, trxName){/** if (C_RfQLine_ID == 0){SetC_RfQLine_ID (0);SetC_RfQ_ID (0);SetLine (0);// @SQL=SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM C_RfQLine WHERE C_RfQ_ID=@C_RfQ_ID@
SetProcessed (false);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_RfQLine(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27829052854760L;/** Last Updated Timestamp 1/8/2019 11:35:37 AM */
public static long updatedMS = 1546927537971L;/** AD_Table_ID=676 */
public static int Table_ID; // =676;
/** TableName=C_RfQLine */
public static String Table_Name="C_RfQLine";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(1);/** AccessLevel
@return 1 - Org 
*/
protected override int Get_AccessLevel(){return Convert.ToInt32(accessLevel.ToString());}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Info
@return info
*/
public override String ToString(){StringBuilder sb = new StringBuilder ("X_C_RfQLine[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set RfQ Line.
@param C_RfQLine_ID Request for Quotation Line */
public void SetC_RfQLine_ID (int C_RfQLine_ID){if (C_RfQLine_ID < 1) throw new ArgumentException ("C_RfQLine_ID is mandatory.");Set_ValueNoCheck ("C_RfQLine_ID", C_RfQLine_ID);}/** Get RfQ Line.
@return Request for Quotation Line */
public int GetC_RfQLine_ID() {Object ii = Get_Value("C_RfQLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set RfQ.
@param C_RfQ_ID Request for Quotation */
public void SetC_RfQ_ID (int C_RfQ_ID){if (C_RfQ_ID < 1) throw new ArgumentException ("C_RfQ_ID is mandatory.");Set_ValueNoCheck ("C_RfQ_ID", C_RfQ_ID);}/** Get RfQ.
@return Request for Quotation */
public int GetC_RfQ_ID() {Object ii = Get_Value("C_RfQ_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() {return new KeyNamePair(Get_ID(), GetC_RfQ_ID().ToString());}/** Set Work Complete.
@param DateWorkComplete Date when work is (planned to be) complete */
public void SetDateWorkComplete (DateTime? DateWorkComplete){Set_Value ("DateWorkComplete", (DateTime?)DateWorkComplete);}/** Get Work Complete.
@return Date when work is (planned to be) complete */
public DateTime? GetDateWorkComplete() {return (DateTime?)Get_Value("DateWorkComplete");}/** Set Work Start.
@param DateWorkStart Date when work is (planned to be) started */
public void SetDateWorkStart (DateTime? DateWorkStart){Set_Value ("DateWorkStart", (DateTime?)DateWorkStart);}/** Get Work Start.
@return Date when work is (planned to be) started */
public DateTime? GetDateWorkStart() {return (DateTime?)Get_Value("DateWorkStart");}/** Set Delivery Days.
@param DeliveryDays Number of Days (planned) until Delivery */
public void SetDeliveryDays (int DeliveryDays){Set_Value ("DeliveryDays", DeliveryDays);}/** Get Delivery Days.
@return Number of Days (planned) until Delivery */
public int GetDeliveryDays() {Object ii = Get_Value("DeliveryDays");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description){if (Description != null && Description.Length > 255){log.Warning("Length > 255 - truncated");Description = Description.Substring(0,255);}Set_Value ("Description", Description);}/** Get Description.
@return Optional short description of the record */
public String GetDescription() {return (String)Get_Value("Description");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_ValueNoCheck ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help){if (Help != null && Help.Length > 2000){log.Warning("Length > 2000 - truncated");Help = Help.Substring(0,2000);}Set_Value ("Help", Help);}/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() {return (String)Get_Value("Help");}/** Set Line No.
@param Line Unique line for this document */
public void SetLine (int Line){Set_Value ("Line", Line);}/** Get Line No.
@return Unique line for this document */
public int GetLine() {Object ii = Get_Value("Line");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Attribute Set Instance.
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
public void SetM_AttributeSetInstance_ID (int M_AttributeSetInstance_ID){if (M_AttributeSetInstance_ID <= 0) Set_Value ("M_AttributeSetInstance_ID", null);else
Set_Value ("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);}/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
public int GetM_AttributeSetInstance_ID() {Object ii = Get_Value("M_AttributeSetInstance_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID){if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);else
Set_Value ("M_Product_ID", M_Product_ID);}/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() {Object ii = Get_Value("M_Product_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Requisition Line.
@param M_RequisitionLine_ID Material Requisition Line */
public void SetM_RequisitionLine_ID (int M_RequisitionLine_ID){if (M_RequisitionLine_ID <= 0) Set_Value ("M_RequisitionLine_ID", null);else
Set_Value ("M_RequisitionLine_ID", M_RequisitionLine_ID);}/** Get Requisition Line.
@return Material Requisition Line */
public int GetM_RequisitionLine_ID() {Object ii = Get_Value("M_RequisitionLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed){Set_Value ("Processed", Processed);}/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed() {Object oo = Get_Value("Processed");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}}
}