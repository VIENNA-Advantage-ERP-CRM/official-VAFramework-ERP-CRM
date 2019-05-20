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
/** Generated Model for Test
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_Test : PO
{
public X_Test (Context ctx, int Test_ID, Trx trxName) : base (ctx, Test_ID, trxName)
{
/** if (Test_ID == 0)
{
SetName (null);
SetTest_ID (0);
}
 */
}
public X_Test (Ctx ctx, int Test_ID, Trx trxName) : base (ctx, Test_ID, trxName)
{
/** if (Test_ID == 0)
{
SetName (null);
SetTest_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_Test (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_Test (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_Test (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_Test()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384663L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067874L;
/** AD_Table_ID=135 */
public static int Table_ID;
 // =135;

/** TableName=Test */
public static String Table_Name="Test";

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
StringBuilder sb = new StringBuilder ("X_Test[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Account_Acct.
@param Account_Acct Account_Acct */
public void SetAccount_Acct (int Account_Acct)
{
Set_Value ("Account_Acct", Account_Acct);
}
/** Get Account_Acct.
@return Account_Acct */
public int GetAccount_Acct() 
{
Object ii = Get_Value("Account_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set BinaryData.
@param BinaryData Binary Data */
public void SetBinaryData (Byte[] BinaryData)
{
Set_Value ("BinaryData", BinaryData);
}
/** Get BinaryData.
@return Binary Data */
public Byte[] GetBinaryData() 
{
return (Byte[])Get_Value("BinaryData");
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID <= 0) Set_Value ("C_BPartner_ID", null);
else
Set_Value ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID)
{
if (C_Currency_ID <= 0) Set_Value ("C_Currency_ID", null);
else
Set_Value ("C_Currency_ID", C_Currency_ID);
}
/** Get Currency.
@return The Currency for this record */
public int GetC_Currency_ID() 
{
Object ii = Get_Value("C_Currency_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Address.
@param C_Location_ID Location or Address */
public void SetC_Location_ID (int C_Location_ID)
{
if (C_Location_ID <= 0) Set_Value ("C_Location_ID", null);
else
Set_Value ("C_Location_ID", C_Location_ID);
}
/** Get Address.
@return Location or Address */
public int GetC_Location_ID() 
{
Object ii = Get_Value("C_Location_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment.
@param C_Payment_ID Payment identifier */
public void SetC_Payment_ID (int C_Payment_ID)
{
if (C_Payment_ID <= 0) Set_Value ("C_Payment_ID", null);
else
Set_Value ("C_Payment_ID", C_Payment_ID);
}
/** Get Payment.
@return Payment identifier */
public int GetC_Payment_ID() 
{
Object ii = Get_Value("C_Payment_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set UOM.
@param C_UOM_ID Unit of Measure */
public void SetC_UOM_ID (int C_UOM_ID)
{
if (C_UOM_ID <= 0) Set_Value ("C_UOM_ID", null);
else
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
/** Set Character Data.
@param CharacterData Long Character Field */
public void SetCharacterData (String CharacterData)
{
Set_Value ("CharacterData", CharacterData);
}
/** Get Character Data.
@return Long Character Field */
public String GetCharacterData() 
{
return (String)Get_Value("CharacterData");
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
/** Set EMail Address.
@param EMail Electronic Mail Address */
public void SetEMail (String EMail)
{
if (EMail != null && EMail.Length > 50)
{
log.Warning("Length > 50 - truncated");
EMail = EMail.Substring(0,50);
}
Set_Value ("EMail", EMail);
}
/** Get EMail Address.
@return Electronic Mail Address */
public String GetEMail() 
{
return (String)Get_Value("EMail");
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
/** Set Locator.
@param M_Locator_ID Warehouse Locator */
public void SetM_Locator_ID (int M_Locator_ID)
{
if (M_Locator_ID <= 0) Set_Value ("M_Locator_ID", null);
else
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
if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);
else
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
/** Set Mobile.
@param Mobile Mobile */
public void SetMobile (String Mobile)
{
if (Mobile != null && Mobile.Length > 50)
{
log.Warning("Length > 50 - truncated");
Mobile = Mobile.Substring(0,50);
}
Set_Value ("Mobile", Mobile);
}
/** Get Mobile.
@return Mobile */
public String GetMobile() 
{
return (String)Get_Value("Mobile");
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
/** Set Amount.
@param T_Amount Amount */
public void SetT_Amount (Decimal? T_Amount)
{
Set_Value ("T_Amount", (Decimal?)T_Amount);
}
/** Get Amount.
@return Amount */
public Decimal GetT_Amount() 
{
Object bd =Get_Value("T_Amount");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Date.
@param T_Date Date */
public void SetT_Date (DateTime? T_Date)
{
Set_Value ("T_Date", (DateTime?)T_Date);
}
/** Get Date.
@return Date */
public DateTime? GetT_Date() 
{
return (DateTime?)Get_Value("T_Date");
}
/** Set Date Time.
@param T_DateTime Date and Time */
public void SetT_DateTime (DateTime? T_DateTime)
{
Set_Value ("T_DateTime", (DateTime?)T_DateTime);
}
/** Get Date Time.
@return Date and Time */
public DateTime? GetT_DateTime() 
{
return (DateTime?)Get_Value("T_DateTime");
}
/** Set Integer.
@param T_Integer Integer */
public void SetT_Integer (int T_Integer)
{
Set_Value ("T_Integer", T_Integer);
}
/** Get Integer.
@return Integer */
public int GetT_Integer() 
{
Object ii = Get_Value("T_Integer");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Number.
@param T_Number Number */
public void SetT_Number (Decimal? T_Number)
{
Set_Value ("T_Number", (Decimal?)T_Number);
}
/** Get Number.
@return Number */
public Decimal GetT_Number() 
{
Object bd =Get_Value("T_Number");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Qty.
@param T_Qty Qty */
public void SetT_Qty (Decimal? T_Qty)
{
Set_Value ("T_Qty", (Decimal?)T_Qty);
}
/** Get Qty.
@return Qty */
public Decimal GetT_Qty() 
{
Object bd =Get_Value("T_Qty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Test ID.
@param Test_ID Test ID */
public void SetTest_ID (int Test_ID)
{
if (Test_ID < 1) throw new ArgumentException ("Test_ID is mandatory.");
Set_ValueNoCheck ("Test_ID", Test_ID);
}
/** Get Test ID.
@return Test ID */
public int GetTest_ID() 
{
Object ii = Get_Value("Test_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
