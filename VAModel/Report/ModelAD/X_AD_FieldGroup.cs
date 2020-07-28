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
/** Generated Model for AD_FieldGroup
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_FieldGroup : PO
{
public X_AD_FieldGroup (Context ctx, int AD_FieldGroup_ID, Trx trxName) : base (ctx, AD_FieldGroup_ID, trxName)
{
/** if (AD_FieldGroup_ID == 0)
{
SetAD_FieldGroup_ID (0);
SetEntityType (null);	// U
SetName (null);
}
 */
}
public X_AD_FieldGroup (Ctx ctx, int AD_FieldGroup_ID, Trx trxName) : base (ctx, AD_FieldGroup_ID, trxName)
{
/** if (AD_FieldGroup_ID == 0)
{
SetAD_FieldGroup_ID (0);
SetEntityType (null);	// U
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_FieldGroup (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_FieldGroup (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_FieldGroup (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_FieldGroup()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514361326L;
/** Last Updated Timestamp 7/29/2010 1:07:24 PM */
public static long updatedMS = 1280389044537L;
/** AD_Table_ID=414 */
public static int Table_ID;
 // =414;

/** TableName=AD_FieldGroup */
public static String Table_Name="AD_FieldGroup";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
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
StringBuilder sb = new StringBuilder ("X_AD_FieldGroup[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Field Group.
@param AD_FieldGroup_ID Logical grouping of fields */
public void SetAD_FieldGroup_ID (int AD_FieldGroup_ID)
{
if (AD_FieldGroup_ID < 1) throw new ArgumentException ("AD_FieldGroup_ID is mandatory.");
Set_ValueNoCheck ("AD_FieldGroup_ID", AD_FieldGroup_ID);
}
/** Get Field Group.
@return Logical grouping of fields */
public int GetAD_FieldGroup_ID() 
{
Object ii = Get_Value("AD_FieldGroup_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** EntityType AD_Reference_ID=389 */
public static int ENTITYTYPE_AD_Reference_ID=389;
/** Set Entity Type.
@param EntityType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetEntityType (String EntityType)
{
if (EntityType.Length > 4)
{
log.Warning("Length > 4 - truncated");
EntityType = EntityType.Substring(0,4);
}
Set_Value ("EntityType", EntityType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetEntityType() 
{
return (String)Get_Value("EntityType");
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
}

}
