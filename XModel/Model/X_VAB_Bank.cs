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
/** Generated Model for VAB_Bank
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_Bank : PO
{
public X_VAB_Bank (Context ctx, int VAB_Bank_ID, Trx trxName) : base (ctx, VAB_Bank_ID, trxName)
{
/** if (VAB_Bank_ID == 0)
{
SetVAB_Bank_ID (0);
SetIsOwnBank (true);	// Y
SetName (null);
SetRoutingNo (null);
}
 */
}
public X_VAB_Bank (Ctx ctx, int VAB_Bank_ID, Trx trxName) : base (ctx, VAB_Bank_ID, trxName)
{
/** if (VAB_Bank_ID == 0)
{
SetVAB_Bank_ID (0);
SetIsOwnBank (true);	// Y
SetName (null);
SetRoutingNo (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Bank (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Bank (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Bank (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_Bank()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514370604L;
/** Last Updated Timestamp 7/29/2010 1:07:33 PM */
public static long updatedMS = 1280389053815L;
/** VAF_TableView_ID=296 */
public static int Table_ID;
 // =296;

/** TableName=VAB_Bank */
public static String Table_Name="VAB_Bank";

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
StringBuilder sb = new StringBuilder ("X_VAB_Bank[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Bank Verification Class.
@param BankVerificationClass Bank Data Verification Class */
public void SetBankVerificationClass (String BankVerificationClass)
{
if (BankVerificationClass != null && BankVerificationClass.Length > 60)
{
log.Warning("Length > 60 - truncated");
BankVerificationClass = BankVerificationClass.Substring(0,60);
}
Set_Value ("BankVerificationClass", BankVerificationClass);
}
/** Get Bank Verification Class.
@return Bank Data Verification Class */
public String GetBankVerificationClass() 
{
return (String)Get_Value("BankVerificationClass");
}
/** Set Bank.
@param VAB_Bank_ID Bank */
public void SetVAB_Bank_ID (int VAB_Bank_ID)
{
if (VAB_Bank_ID < 1) throw new ArgumentException ("VAB_Bank_ID is mandatory.");
Set_ValueNoCheck ("VAB_Bank_ID", VAB_Bank_ID);
}
/** Get Bank.
@return Bank */
public int GetVAB_Bank_ID() 
{
Object ii = Get_Value("VAB_Bank_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Address.
@param VAB_Address_ID Location or Address */
public void SetVAB_Address_ID (int VAB_Address_ID)
{
if (VAB_Address_ID <= 0) Set_Value ("VAB_Address_ID", null);
else
Set_Value ("VAB_Address_ID", VAB_Address_ID);
}
/** Get Address.
@return Location or Address */
public int GetVAB_Address_ID() 
{
Object ii = Get_Value("VAB_Address_ID");
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
/** Set Own Bank.
@param IsOwnBank Bank for this Organization */
public void SetIsOwnBank (Boolean IsOwnBank)
{
Set_Value ("IsOwnBank", IsOwnBank);
}
/** Get Own Bank.
@return Bank for this Organization */
public Boolean IsOwnBank() 
{
Object oo = Get_Value("IsOwnBank");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Routing No.
@param RoutingNo Bank Routing Number */
public void SetRoutingNo (String RoutingNo)
{
if (RoutingNo == null) throw new ArgumentException ("RoutingNo is mandatory.");
if (RoutingNo.Length > 20)
{
log.Warning("Length > 20 - truncated");
RoutingNo = RoutingNo.Substring(0,20);
}
Set_Value ("RoutingNo", RoutingNo);
}
/** Get Routing No.
@return Bank Routing Number */
public String GetRoutingNo() 
{
return (String)Get_Value("RoutingNo");
}
/** Set Swift code.
@param SwiftCode Swift Code or BIC */
public void SetSwiftCode (String SwiftCode)
{
if (SwiftCode != null && SwiftCode.Length > 20)
{
log.Warning("Length > 20 - truncated");
SwiftCode = SwiftCode.Substring(0,20);
}
Set_Value ("SwiftCode", SwiftCode);
}
/** Get Swift code.
@return Swift Code or BIC */
public String GetSwiftCode() 
{
return (String)Get_Value("SwiftCode");
}
}

}
