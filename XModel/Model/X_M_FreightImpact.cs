namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for VAM_FreightImpact
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_FreightImpact : PO{public X_VAM_FreightImpact (Context ctx, int VAM_FreightImpact_ID, Trx trxName) : base (ctx, VAM_FreightImpact_ID, trxName){/** if (VAM_FreightImpact_ID == 0){SetVAM_FreightImpact_ID (0);} */
}public X_VAM_FreightImpact (Ctx ctx, int VAM_FreightImpact_ID, Trx trxName) : base (ctx, VAM_FreightImpact_ID, trxName){/** if (VAM_FreightImpact_ID == 0){SetVAM_FreightImpact_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_FreightImpact (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_FreightImpact (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_FreightImpact (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_FreightImpact(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27824842352455L;/** Last Updated Timestamp 11/20/2018 6:00:35 PM */
public static long updatedMS = 1542717035666L;/** VAF_TableView_ID=1000529 */
public static int Table_ID; // =1000529;
/** TableName=VAM_FreightImpact */
public static String Table_Name="VAM_FreightImpact";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_VAM_FreightImpact[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID){if (VAF_TableView_ID <= 0) Set_Value ("VAF_TableView_ID", null);else
Set_Value ("VAF_TableView_ID", VAF_TableView_ID);}/** Get Table.
@return Database Table information */
public int GetVAF_TableView_ID() {Object ii = Get_Value("VAF_TableView_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set VAM_FreightImpact_ID.
@param VAM_FreightImpact_ID VAM_FreightImpact_ID */
public void SetVAM_FreightImpact_ID (int VAM_FreightImpact_ID){if (VAM_FreightImpact_ID < 1) throw new ArgumentException ("VAM_FreightImpact_ID is mandatory.");Set_ValueNoCheck ("VAM_FreightImpact_ID", VAM_FreightImpact_ID);}/** Get VAM_FreightImpact_ID.
@return VAM_FreightImpact_ID */
public int GetVAM_FreightImpact_ID() {Object ii = Get_Value("VAM_FreightImpact_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Quantity.
@param Qty Quantity */
public void SetQty (Decimal? Qty){Set_Value ("Qty", (Decimal?)Qty);}/** Get Quantity.
@return Quantity */
public Decimal GetQty() {Object bd =Get_Value("Qty");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID){if (Record_ID <= 0) Set_Value ("Record_ID", null);else
Set_Value ("Record_ID", Record_ID);}/** Get Record ID.
@return Direct internal record ID */
public int GetRecord_ID() {Object ii = Get_Value("Record_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** Set Accounting Schema.
@param VAB_AccountBook_ID Rules for accounting */
public void SetVAB_AccountBook_ID(int VAB_AccountBook_ID)
{
    if (VAB_AccountBook_ID <= 0) Set_Value("VAB_AccountBook_ID", null);
    else
        Set_Value("VAB_AccountBook_ID", VAB_AccountBook_ID);
}/** Get Accounting Schema.
@return Rules for accounting */
public int GetVAB_AccountBook_ID() { Object ii = Get_Value("VAB_AccountBook_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
/** Set Cost Element.
@param VAM_ProductCostElement_ID Product Cost Element */
public void SetVAM_ProductCostElement_ID(int VAM_ProductCostElement_ID)
{
    if (VAM_ProductCostElement_ID <= 0) Set_Value("VAM_ProductCostElement_ID", null);
    else
        Set_Value("VAM_ProductCostElement_ID", VAM_ProductCostElement_ID);
}/** Get Cost Element.
@return Product Cost Element */
public int GetVAM_ProductCostElement_ID() { Object ii = Get_Value("VAM_ProductCostElement_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
/** Set Product.
@param VAM_Product_ID Product, Service, Item */
public void SetVAM_Product_ID(int VAM_Product_ID)
{
    if (VAM_Product_ID <= 0) Set_Value("VAM_Product_ID", null);
    else
        Set_Value("VAM_Product_ID", VAM_Product_ID);
}/** Get Product.
@return Product, Service, Item */
public int GetVAM_Product_ID() { Object ii = Get_Value("VAM_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
}
}