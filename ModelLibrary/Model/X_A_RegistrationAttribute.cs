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
/** Generated Model for A_RegistrationAttribute
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_A_RegistrationAttribute : PO
{
public X_A_RegistrationAttribute (Context ctx, int A_RegistrationAttribute_ID, Trx trxName) : base (ctx, A_RegistrationAttribute_ID, trxName)
{
/** if (A_RegistrationAttribute_ID == 0)
{
SetAD_Reference_ID (0);
SetA_RegistrationAttribute_ID (0);
SetIsSelfService (true);	// Y
SetName (null);
SetSeqNo (0);
}
 */
}
public X_A_RegistrationAttribute (Ctx ctx, int A_RegistrationAttribute_ID, Trx trxName) : base (ctx, A_RegistrationAttribute_ID, trxName)
{
/** if (A_RegistrationAttribute_ID == 0)
{
SetAD_Reference_ID (0);
SetA_RegistrationAttribute_ID (0);
SetIsSelfService (true);	// Y
SetName (null);
SetSeqNo (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_A_RegistrationAttribute (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_A_RegistrationAttribute (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_A_RegistrationAttribute (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_A_RegistrationAttribute()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514367141L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050352L;
/** AD_Table_ID=652 */
public static int Table_ID;
 // =652;

/** TableName=A_RegistrationAttribute */
public static String Table_Name="A_RegistrationAttribute";

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
StringBuilder sb = new StringBuilder ("X_A_RegistrationAttribute[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_Reference_ID AD_Reference_ID=1 */
public static int AD_REFERENCE_ID_AD_Reference_ID=1;
/** Set Reference.
@param AD_Reference_ID System Reference and Validation */
public void SetAD_Reference_ID (int AD_Reference_ID)
{
if (AD_Reference_ID < 1) throw new ArgumentException ("AD_Reference_ID is mandatory.");
Set_Value ("AD_Reference_ID", AD_Reference_ID);
}
/** Get Reference.
@return System Reference and Validation */
public int GetAD_Reference_ID() 
{
Object ii = Get_Value("AD_Reference_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** AD_Reference_Value_ID AD_Reference_ID=4 */
public static int AD_REFERENCE_VALUE_ID_AD_Reference_ID=4;
/** Set Reference Key.
@param AD_Reference_Value_ID Required to specify, if data type is Table or List */
public void SetAD_Reference_Value_ID (int AD_Reference_Value_ID)
{
if (AD_Reference_Value_ID <= 0) Set_Value ("AD_Reference_Value_ID", null);
else
Set_Value ("AD_Reference_Value_ID", AD_Reference_Value_ID);
}
/** Get Reference Key.
@return Required to specify, if data type is Table or List */
public int GetAD_Reference_Value_ID() 
{
Object ii = Get_Value("AD_Reference_Value_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Registration Attribute.
@param A_RegistrationAttribute_ID Asset Registration Attribute */
public void SetA_RegistrationAttribute_ID (int A_RegistrationAttribute_ID)
{
if (A_RegistrationAttribute_ID < 1) throw new ArgumentException ("A_RegistrationAttribute_ID is mandatory.");
Set_ValueNoCheck ("A_RegistrationAttribute_ID", A_RegistrationAttribute_ID);
}
/** Get Registration Attribute.
@return Asset Registration Attribute */
public int GetA_RegistrationAttribute_ID() 
{
Object ii = Get_Value("A_RegistrationAttribute_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set DB Column Name.
@param ColumnName Name of the column in the database */
public void SetColumnName (String ColumnName)
{
if (ColumnName != null && ColumnName.Length > 40)
{
log.Warning("Length > 40 - truncated");
ColumnName = ColumnName.Substring(0,40);
}
Set_Value ("ColumnName", ColumnName);
}
/** Get DB Column Name.
@return Name of the column in the database */
public String GetColumnName() 
{
return (String)Get_Value("ColumnName");
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
/** Set Self-Service.
@param IsSelfService This is a Self-Service entry or this entry can be changed via Self-Service */
public void SetIsSelfService (Boolean IsSelfService)
{
Set_Value ("IsSelfService", IsSelfService);
}
/** Get Self-Service.
@return This is a Self-Service entry or this entry can be changed via Self-Service */
public Boolean IsSelfService() 
{
Object oo = Get_Value("IsSelfService");
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
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_Value ("SeqNo", SeqNo);
}
/** Get Sequence.
@return Method of ordering elements;
 lowest number comes first */
public int GetSeqNo() 
{
Object ii = Get_Value("SeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
