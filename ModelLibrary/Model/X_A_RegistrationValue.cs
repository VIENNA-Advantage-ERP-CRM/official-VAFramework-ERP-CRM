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
/** Generated Model for A_RegistrationValue
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_A_RegistrationValue : PO
{
public X_A_RegistrationValue (Context ctx, int A_RegistrationValue_ID, Trx trxName) : base (ctx, A_RegistrationValue_ID, trxName)
{
/** if (A_RegistrationValue_ID == 0)
{
SetA_RegistrationAttribute_ID (0);
SetA_Registration_ID (0);
SetName (null);
}
 */
}
public X_A_RegistrationValue (Ctx ctx, int A_RegistrationValue_ID, Trx trxName) : base (ctx, A_RegistrationValue_ID, trxName)
{
/** if (A_RegistrationValue_ID == 0)
{
SetA_RegistrationAttribute_ID (0);
SetA_Registration_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_A_RegistrationValue (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_A_RegistrationValue (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_A_RegistrationValue (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_A_RegistrationValue()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514367219L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050430L;
/** AD_Table_ID=653 */
public static int Table_ID;
 // =653;

/** TableName=A_RegistrationValue */
public static String Table_Name="A_RegistrationValue";

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
StringBuilder sb = new StringBuilder ("X_A_RegistrationValue[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Registration.
@param A_Registration_ID User Asset Registration */
public void SetA_Registration_ID (int A_Registration_ID)
{
if (A_Registration_ID < 1) throw new ArgumentException ("A_Registration_ID is mandatory.");
Set_ValueNoCheck ("A_Registration_ID", A_Registration_ID);
}
/** Get Registration.
@return User Asset Registration */
public int GetA_Registration_ID() 
{
Object ii = Get_Value("A_Registration_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetA_Registration_ID().ToString());
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
}

}
