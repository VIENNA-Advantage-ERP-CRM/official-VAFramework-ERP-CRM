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
/** Generated Model for I_ProductCost
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_I_ProductCost : PO
{
public X_I_ProductCost (Context ctx, int I_ProductCost_ID, Trx trxName) : base (ctx, I_ProductCost_ID, trxName)
{
/** if (I_ProductCost_ID == 0)
{
SetI_ProductCost_ID (0);
}
 */
}
public X_I_ProductCost (Ctx ctx, int I_ProductCost_ID, Trx trxName) : base (ctx, I_ProductCost_ID, trxName)
{
/** if (I_ProductCost_ID == 0)
{
SetI_ProductCost_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_ProductCost (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_ProductCost (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_ProductCost (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_I_ProductCost()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27631208066137L;
/** Last Updated Timestamp 10/1/2012 2:42:29 PM */
public static long updatedMS = 1349082749348L;
/** AD_Table_ID=1000381 */
public static int Table_ID;
 // =1000381;

/** TableName=I_ProductCost */
public static String Table_Name="I_ProductCost";

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
StringBuilder sb = new StringBuilder ("X_I_ProductCost[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set SerialNo.
@param SerialNo SerialNo */
public void SetSerialNo(String SerialNo)
{
    if (SerialNo != null && SerialNo.Length > 100)
    {
        log.Warning("Length > 100 - truncated");
        SerialNo = SerialNo.Substring(0, 100);
    }
    Set_Value("SerialNo", SerialNo);
}
/** Get SerialNo.
@return SerialNo */
public String GetSerialNo()
{
    return (String)Get_Value("SerialNo");
}
/** Set I_ProductCost_ID.
@param I_ProductCost_ID I_ProductCost_ID */
public void SetI_ProductCost_ID (int I_ProductCost_ID)
{
if (I_ProductCost_ID < 1) throw new ArgumentException ("I_ProductCost_ID is mandatory.");
Set_ValueNoCheck ("I_ProductCost_ID", I_ProductCost_ID);
}
/** Get I_ProductCost_ID.
@return I_ProductCost_ID */
public int GetI_ProductCost_ID() 
{
Object ii = Get_Value("I_ProductCost_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set LotNo.
@param LotNo SerialNo */
public void SetLotNo(String LotNo)
{
    if (LotNo != null && LotNo.Length > 100)
    {
        log.Warning("Length > 100 - truncated");
        LotNo = LotNo.Substring(0, 100);
    }
    Set_Value("LotNo", LotNo);
}
/** Get LotNo.
@return LotNo */
public String GetLotNo()
{
    return (String)Get_Value("LotNo");
}
/** Set Name.
@param Name Name */
public void SetName (String Name)
{
if (Name != null && Name.Length > 100)
{
log.Warning("Length > 100 - truncated");
Name = Name.Substring(0,100);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Name */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set PRODUCTCOST.
@param PRODUCTCOST PRODUCTCOST */
public void SetPRODUCTCOST (Decimal? PRODUCTCOST)
{
Set_Value ("PRODUCTCOST", (Decimal?)PRODUCTCOST);
}
/** Get PRODUCTCOST.
@return PRODUCTCOST */
public Decimal GetPRODUCTCOST() 
{
Object bd =Get_Value("PRODUCTCOST");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Product Category Key.
@param ProductCategory_Value Product Category Key */
public void SetProductCategory_Value (String ProductCategory_Value)
{
if (ProductCategory_Value != null && ProductCategory_Value.Length > 100)
{
log.Warning("Length > 100 - truncated");
ProductCategory_Value = ProductCategory_Value.Substring(0,100);
}
Set_Value ("ProductCategory_Value", ProductCategory_Value);
}
/** Get Product Category Key.
@return Product Category Key */
public String GetProductCategory_Value() 
{
return (String)Get_Value("ProductCategory_Value");
}
/** Set ProductCode.
@param ProductCode ProductCode */
public void SetProductCode (String ProductCode)
{
if (ProductCode != null && ProductCode.Length > 100)
{
log.Warning("Length > 100 - truncated");
ProductCode = ProductCode.Substring(0,100);
}
Set_Value ("ProductCode", ProductCode);
}
/** Get ProductCode.
@return ProductCode */
public String GetProductCode() 
{
return (String)Get_Value("ProductCode");
}
/** Set StockQuantity.
@param StockQuantity StockQuantity */
public void SetStockQuantity(Decimal? StockQuantity)
{
    Set_Value("StockQuantity", (Decimal?)StockQuantity);
}
/** Get StockQuantity.
@return StockQuantity */
public Decimal GetStockQuantity()
{
    Object bd = Get_Value("StockQuantity");
    if (bd == null) return Env.ZERO;
    return Convert.ToDecimal(bd);
}
/** Set STOCKVALUE.
@param STOCKVALUE STOCKVALUE */
public void SetSTOCKVALUE (Decimal? STOCKVALUE)
{
Set_Value ("STOCKVALUE", (Decimal?)STOCKVALUE);
}
/** Get STOCKVALUE.
@return STOCKVALUE */
public Decimal GetSTOCKVALUE() 
{
Object bd =Get_Value("STOCKVALUE");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set UOM.
@param UOM UOM */
public void SetUOM (String UOM)
{
if (UOM != null && UOM.Length > 50)
{
log.Warning("Length > 50 - truncated");
UOM = UOM.Substring(0,50);
}
Set_Value ("UOM", UOM);
}
/** Get UOM.
@return UOM */
public String GetUOM() 
{
return (String)Get_Value("UOM");
}

public static String I_ISIMPORTED_Error = "E";
/** No = N */
public static String I_ISIMPORTED_No = "N";
/** Yes = Y */
public static String I_ISIMPORTED_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsI_IsImportedValid(String test)
{
    return test.Equals("E") || test.Equals("N") || test.Equals("Y");
}

/** Set Processed.
@param Processed The document has been processed */
public void SetI_IsImported(Boolean I_IsImported)
{
    throw new ArgumentException("Processed Is virtual column");
}
/** Get Processed.
@return The document has been processed */
public Boolean I_IsImported()
{
    Object oo = Get_Value("I_IsImported");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}
/** Set Imported.
@param I_IsImported Has this import been processed */

}

}
