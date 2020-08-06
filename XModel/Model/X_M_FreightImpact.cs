namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for M_FreightImpact
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_FreightImpact : PO{public X_M_FreightImpact (Context ctx, int M_FreightImpact_ID, Trx trxName) : base (ctx, M_FreightImpact_ID, trxName){/** if (M_FreightImpact_ID == 0){SetM_FreightImpact_ID (0);} */
}public X_M_FreightImpact (Ctx ctx, int M_FreightImpact_ID, Trx trxName) : base (ctx, M_FreightImpact_ID, trxName){/** if (M_FreightImpact_ID == 0){SetM_FreightImpact_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_FreightImpact (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_FreightImpact (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_FreightImpact (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_FreightImpact(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27824842352455L;/** Last Updated Timestamp 11/20/2018 6:00:35 PM */
public static long updatedMS = 1542717035666L;/** AD_Table_ID=1000529 */
public static int Table_ID; // =1000529;
/** TableName=M_FreightImpact */
public static String Table_Name="M_FreightImpact";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_M_FreightImpact[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID){if (AD_Table_ID <= 0) Set_Value ("AD_Table_ID", null);else
Set_Value ("AD_Table_ID", AD_Table_ID);}/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() {Object ii = Get_Value("AD_Table_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set M_FreightImpact_ID.
@param M_FreightImpact_ID M_FreightImpact_ID */
public void SetM_FreightImpact_ID (int M_FreightImpact_ID){if (M_FreightImpact_ID < 1) throw new ArgumentException ("M_FreightImpact_ID is mandatory.");Set_ValueNoCheck ("M_FreightImpact_ID", M_FreightImpact_ID);}/** Get M_FreightImpact_ID.
@return M_FreightImpact_ID */
public int GetM_FreightImpact_ID() {Object ii = Get_Value("M_FreightImpact_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Quantity.
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
@param C_AcctSchema_ID Rules for accounting */
public void SetC_AcctSchema_ID(int C_AcctSchema_ID)
{
    if (C_AcctSchema_ID <= 0) Set_Value("C_AcctSchema_ID", null);
    else
        Set_Value("C_AcctSchema_ID", C_AcctSchema_ID);
}/** Get Accounting Schema.
@return Rules for accounting */
public int GetC_AcctSchema_ID() { Object ii = Get_Value("C_AcctSchema_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
/** Set Cost Element.
@param M_CostElement_ID Product Cost Element */
public void SetM_CostElement_ID(int M_CostElement_ID)
{
    if (M_CostElement_ID <= 0) Set_Value("M_CostElement_ID", null);
    else
        Set_Value("M_CostElement_ID", M_CostElement_ID);
}/** Get Cost Element.
@return Product Cost Element */
public int GetM_CostElement_ID() { Object ii = Get_Value("M_CostElement_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID(int M_Product_ID)
{
    if (M_Product_ID <= 0) Set_Value("M_Product_ID", null);
    else
        Set_Value("M_Product_ID", M_Product_ID);
}/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() { Object ii = Get_Value("M_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
}
}