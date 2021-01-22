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
/** Generated Model for VAF_FieldSection
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_FieldSection : PO
{
public X_VAF_FieldSection (Context ctx, int VAF_FieldSection_ID, Trx trxName) : base (ctx, VAF_FieldSection_ID, trxName)
{
/** if (VAF_FieldSection_ID == 0)
{
SetVAF_FieldSection_ID (0);
SetEntityType (null);	// U
SetName (null);
}
 */
}
public X_VAF_FieldSection (Ctx ctx, int VAF_FieldSection_ID, Trx trxName) : base (ctx, VAF_FieldSection_ID, trxName)
{
/** if (VAF_FieldSection_ID == 0)
{
SetVAF_FieldSection_ID (0);
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
public X_VAF_FieldSection (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_FieldSection (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_FieldSection (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_FieldSection()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514361326L;
/** Last Updated Timestamp 7/29/2010 1:07:24 PM */
public static long updatedMS = 1280389044537L;
/** VAF_TableView_ID=414 */
public static int Table_ID;
 // =414;

/** TableName=VAF_FieldSection */
public static String Table_Name="VAF_FieldSection";

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
StringBuilder sb = new StringBuilder ("X_VAF_FieldSection[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Field Group.
@param VAF_FieldSection_ID Logical grouping of fields */
public void SetVAF_FieldSection_ID (int VAF_FieldSection_ID)
{
if (VAF_FieldSection_ID < 1) throw new ArgumentException ("VAF_FieldSection_ID is mandatory.");
Set_ValueNoCheck ("VAF_FieldSection_ID", VAF_FieldSection_ID);
}
/** Get Field Group.
@return Logical grouping of fields */
public int GetVAF_FieldSection_ID() 
{
Object ii = Get_Value("VAF_FieldSection_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** EntityType VAF_Control_Ref_ID=389 */
public static int ENTITYTYPE_VAF_Control_Ref_ID=389;
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
