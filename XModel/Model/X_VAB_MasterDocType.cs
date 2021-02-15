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
/** Generated Model for VAB_MasterDocType
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_MasterDocType : PO
{
public X_VAB_MasterDocType (Context ctx, int VAB_MasterDocType_ID, Trx trxName) : base (ctx, VAB_MasterDocType_ID, trxName)
{
/** if (VAB_MasterDocType_ID == 0)
{
SetVAB_MasterDocType_ID (0);
SetDocBaseType (null);	// XXX
SetEntityType (null);	// U
SetName (null);
}
 */
}
public X_VAB_MasterDocType (Ctx ctx, int VAB_MasterDocType_ID, Trx trxName) : base (ctx, VAB_MasterDocType_ID, trxName)
{
/** if (VAB_MasterDocType_ID == 0)
{
SetVAB_MasterDocType_ID (0);
SetDocBaseType (null);	// XXX
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
public X_VAB_MasterDocType (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_MasterDocType (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_MasterDocType (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_MasterDocType()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371842L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055053L;
/** VAF_TableView_ID=988 */
public static int Table_ID;
 // =988;

/** TableName=VAB_MasterDocType */
public static String Table_Name="VAB_MasterDocType";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(6);
/** AccessLevel
@return 6 - System - Client 
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
StringBuilder sb = new StringBuilder ("X_VAB_MasterDocType[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID)
{
if (VAF_TableView_ID <= 0) Set_Value ("VAF_TableView_ID", null);
else
Set_Value ("VAF_TableView_ID", VAF_TableView_ID);
}
/** Get Table.
@return Database Table information */
public int GetVAF_TableView_ID() 
{
Object ii = Get_Value("VAF_TableView_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Accounting Classname.
@param AccountingClassname Classname for Accounting class */
public void SetAccountingClassname (String AccountingClassname)
{
if (AccountingClassname != null && AccountingClassname.Length > 60)
{
log.Warning("Length > 60 - truncated");
AccountingClassname = AccountingClassname.Substring(0,60);
}
Set_Value ("AccountingClassname", AccountingClassname);
}
/** Get Accounting Classname.
@return Classname for Accounting class */
public String GetAccountingClassname() 
{
return (String)Get_Value("AccountingClassname");
}
/** Set Document Base Type.
@param VAB_MasterDocType_ID Accounting Document base type */
public void SetVAB_MasterDocType_ID (int VAB_MasterDocType_ID)
{
if (VAB_MasterDocType_ID < 1) throw new ArgumentException ("VAB_MasterDocType_ID is mandatory.");
Set_ValueNoCheck ("VAB_MasterDocType_ID", VAB_MasterDocType_ID);
}
/** Get Document Base Type.
@return Accounting Document base type */
public int GetVAB_MasterDocType_ID() 
{
Object ii = Get_Value("VAB_MasterDocType_ID");
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
/** Set Document BaseType.
@param DocBaseType Logical type of document */
public void SetDocBaseType (String DocBaseType)
{
if (DocBaseType == null) throw new ArgumentException ("DocBaseType is mandatory.");
if (DocBaseType.Length > 3)
{
log.Warning("Length > 3 - truncated");
DocBaseType = DocBaseType.Substring(0,3);
}
Set_Value ("DocBaseType", DocBaseType);
}
/** Get Document BaseType.
@return Logical type of document */
public String GetDocBaseType() 
{
return (String)Get_Value("DocBaseType");
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
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 120)
{
log.Warning("Length > 120 - truncated");
Name = Name.Substring(0,120);
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
