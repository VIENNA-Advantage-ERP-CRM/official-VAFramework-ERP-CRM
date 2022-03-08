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
/** Generated Model for C_POS
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_POS : PO
{
public X_C_POS (Context ctx, int C_POS_ID, Trx trxName) : base (ctx, C_POS_ID, trxName)
{
/** if (C_POS_ID == 0)
{
SetC_CashBook_ID (0);
SetC_POS_ID (0);
SetIsModifyPrice (false);	// N
SetM_PriceList_ID (0);
SetM_Warehouse_ID (0);
SetName (null);
SetSalesRep_ID (0);
}
 */
}
public X_C_POS (Ctx ctx, int C_POS_ID, Trx trxName) : base (ctx, C_POS_ID, trxName)
{
/** if (C_POS_ID == 0)
{
SetC_CashBook_ID (0);
SetC_POS_ID (0);
SetIsModifyPrice (false);	// N
SetM_PriceList_ID (0);
SetM_Warehouse_ID (0);
SetName (null);
SetSalesRep_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_POS (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_POS (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_POS (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_POS()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514373488L;
/** Last Updated Timestamp 7/29/2010 1:07:36 PM */
public static long updatedMS = 1280389056699L;
/** AD_Table_ID=748 */
public static int Table_ID;
 // =748;

/** TableName=C_POS */
public static String Table_Name="C_POS";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_C_POS[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** C_BPartnerCashTrx_ID AD_Reference_ID=173 */
public static int C_BPARTNERCASHTRX_ID_AD_Reference_ID=173;
/** Set Template B.Partner.
@param C_BPartnerCashTrx_ID Business Partner used for creating new Business Partners on the fly */
public void SetC_BPartnerCashTrx_ID (int C_BPartnerCashTrx_ID)
{
if (C_BPartnerCashTrx_ID <= 0) Set_Value ("C_BPartnerCashTrx_ID", null);
else
Set_Value ("C_BPartnerCashTrx_ID", C_BPartnerCashTrx_ID);
}
/** Get Template B.Partner.
@return Business Partner used for creating new Business Partners on the fly */
public int GetC_BPartnerCashTrx_ID() 
{
Object ii = Get_Value("C_BPartnerCashTrx_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Cash Book.
@param C_CashBook_ID Cash Book for recording petty cash transactions */
public void SetC_CashBook_ID (int C_CashBook_ID)
{
if (C_CashBook_ID < 1) throw new ArgumentException ("C_CashBook_ID is mandatory.");
Set_Value ("C_CashBook_ID", C_CashBook_ID);
}
/** Get Cash Book.
@return Cash Book for recording petty cash transactions */
public int GetC_CashBook_ID() 
{
Object ii = Get_Value("C_CashBook_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Document Type.
@param C_DocType_ID Document type or rules */
public void SetC_DocType_ID (int C_DocType_ID)
{
if (C_DocType_ID <= 0) Set_Value ("C_DocType_ID", null);
else
Set_Value ("C_DocType_ID", C_DocType_ID);
}
/** Get Document Type.
@return Document type or rules */
public int GetC_DocType_ID() 
{
Object ii = Get_Value("C_DocType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set POS Key Layout.
@param C_POSKeyLayout_ID POS Function Key Layout */
public void SetC_POSKeyLayout_ID (int C_POSKeyLayout_ID)
{
if (C_POSKeyLayout_ID <= 0) Set_Value ("C_POSKeyLayout_ID", null);
else
Set_Value ("C_POSKeyLayout_ID", C_POSKeyLayout_ID);
}
/** Get POS Key Layout.
@return POS Function Key Layout */
public int GetC_POSKeyLayout_ID() 
{
Object ii = Get_Value("C_POSKeyLayout_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set POS Terminal.
@param C_POS_ID Point of Sales Terminal */
public void SetC_POS_ID (int C_POS_ID)
{
if (C_POS_ID < 1) throw new ArgumentException ("C_POS_ID is mandatory.");
Set_ValueNoCheck ("C_POS_ID", C_POS_ID);
}
/** Get POS Terminal.
@return Point of Sales Terminal */
public int GetC_POS_ID() 
{
Object ii = Get_Value("C_POS_ID");
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
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
}
/** Set Modify Price.
@param IsModifyPrice Allow modifying the price */
public void SetIsModifyPrice (Boolean IsModifyPrice)
{
Set_Value ("IsModifyPrice", IsModifyPrice);
}
/** Get Modify Price.
@return Allow modifying the price */
public Boolean IsModifyPrice() 
{
Object oo = Get_Value("IsModifyPrice");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Price List.
@param M_PriceList_ID Unique identifier of a Price List */
public void SetM_PriceList_ID (int M_PriceList_ID)
{
if (M_PriceList_ID < 1) throw new ArgumentException ("M_PriceList_ID is mandatory.");
Set_Value ("M_PriceList_ID", M_PriceList_ID);
}
/** Get Price List.
@return Unique identifier of a Price List */
public int GetM_PriceList_ID() 
{
Object ii = Get_Value("M_PriceList_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Warehouse.
@param M_Warehouse_ID Storage Warehouse and Service Point */
public void SetM_Warehouse_ID (int M_Warehouse_ID)
{
if (M_Warehouse_ID < 1) throw new ArgumentException ("M_Warehouse_ID is mandatory.");
Set_Value ("M_Warehouse_ID", M_Warehouse_ID);
}
/** Get Warehouse.
@return Storage Warehouse and Service Point */
public int GetM_Warehouse_ID() 
{
Object ii = Get_Value("M_Warehouse_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
/** Set Printer Name.
@param PrinterName Name of the Printer */
public void SetPrinterName (String PrinterName)
{
if (PrinterName != null && PrinterName.Length > 60)
{
log.Warning("Length > 60 - truncated");
PrinterName = PrinterName.Substring(0,60);
}
Set_Value ("PrinterName", PrinterName);
}
/** Get Printer Name.
@return Name of the Printer */
public String GetPrinterName() 
{
return (String)Get_Value("PrinterName");
}

/** SalesRep_ID AD_Reference_ID=190 */
public static int SALESREP_ID_AD_Reference_ID=190;
/** Set Representative.
@param SalesRep_ID Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
public void SetSalesRep_ID (int SalesRep_ID)
{
if (SalesRep_ID < 1) throw new ArgumentException ("SalesRep_ID is mandatory.");
Set_Value ("SalesRep_ID", SalesRep_ID);
}
/** Get Representative.
@return Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
public int GetSalesRep_ID() 
{
Object ii = Get_Value("SalesRep_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
