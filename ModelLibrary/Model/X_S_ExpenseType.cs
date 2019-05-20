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
/** Generated Model for S_ExpenseType
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_S_ExpenseType : PO
{
public X_S_ExpenseType (Context ctx, int S_ExpenseType_ID, Trx trxName) : base (ctx, S_ExpenseType_ID, trxName)
{
/** if (S_ExpenseType_ID == 0)
{
SetC_TaxCategory_ID (0);
SetC_UOM_ID (0);
SetIsInvoiced (false);
SetM_Product_Category_ID (0);
SetName (null);
SetS_ExpenseType_ID (0);
SetValue (null);
}
 */
}
public X_S_ExpenseType (Ctx ctx, int S_ExpenseType_ID, Trx trxName) : base (ctx, S_ExpenseType_ID, trxName)
{
/** if (S_ExpenseType_ID == 0)
{
SetC_TaxCategory_ID (0);
SetC_UOM_ID (0);
SetIsInvoiced (false);
SetM_Product_Category_ID (0);
SetName (null);
SetS_ExpenseType_ID (0);
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_S_ExpenseType (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_S_ExpenseType (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_S_ExpenseType (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_S_ExpenseType()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383550L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066761L;
/** AD_Table_ID=481 */
public static int Table_ID;
 // =481;

/** TableName=S_ExpenseType */
public static String Table_Name="S_ExpenseType";

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
StringBuilder sb = new StringBuilder ("X_S_ExpenseType[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Tax Category.
@param C_TaxCategory_ID Tax Category */
public void SetC_TaxCategory_ID (int C_TaxCategory_ID)
{
if (C_TaxCategory_ID < 1) throw new ArgumentException ("C_TaxCategory_ID is mandatory.");
Set_Value ("C_TaxCategory_ID", C_TaxCategory_ID);
}
/** Get Tax Category.
@return Tax Category */
public int GetC_TaxCategory_ID() 
{
Object ii = Get_Value("C_TaxCategory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set UOM.
@param C_UOM_ID Unit of Measure */
public void SetC_UOM_ID (int C_UOM_ID)
{
if (C_UOM_ID < 1) throw new ArgumentException ("C_UOM_ID is mandatory.");
Set_Value ("C_UOM_ID", C_UOM_ID);
}
/** Get UOM.
@return Unit of Measure */
public int GetC_UOM_ID() 
{
Object ii = Get_Value("C_UOM_ID");
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
/** Set Invoiced.
@param IsInvoiced Is this invoiced? */
public void SetIsInvoiced (Boolean IsInvoiced)
{
Set_Value ("IsInvoiced", IsInvoiced);
}
/** Get Invoiced.
@return Is this invoiced? */
public Boolean IsInvoiced() 
{
Object oo = Get_Value("IsInvoiced");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Product Category.
@param M_Product_Category_ID Category of a Product */
public void SetM_Product_Category_ID (int M_Product_Category_ID)
{
if (M_Product_Category_ID < 1) throw new ArgumentException ("M_Product_Category_ID is mandatory.");
Set_Value ("M_Product_Category_ID", M_Product_Category_ID);
}
/** Get Product Category.
@return Category of a Product */
public int GetM_Product_Category_ID() 
{
Object ii = Get_Value("M_Product_Category_ID");
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
/** Set Expense Type.
@param S_ExpenseType_ID Expense report type */
public void SetS_ExpenseType_ID (int S_ExpenseType_ID)
{
if (S_ExpenseType_ID < 1) throw new ArgumentException ("S_ExpenseType_ID is mandatory.");
Set_ValueNoCheck ("S_ExpenseType_ID", S_ExpenseType_ID);
}
/** Get Expense Type.
@return Expense report type */
public int GetS_ExpenseType_ID() 
{
Object ii = Get_Value("S_ExpenseType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value == null) throw new ArgumentException ("Value is mandatory.");
if (Value.Length > 40)
{
log.Warning("Length > 40 - truncated");
Value = Value.Substring(0,40);
}
Set_Value ("Value", Value);
}
/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() 
{
return (String)Get_Value("Value");
}
}

}
