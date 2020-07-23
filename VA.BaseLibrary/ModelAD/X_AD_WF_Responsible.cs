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
/** Generated Model for AD_WF_Responsible
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_WF_Responsible : PO
{
public X_AD_WF_Responsible (Context ctx, int AD_WF_Responsible_ID, Trx trxName) : base (ctx, AD_WF_Responsible_ID, trxName)
{
/** if (AD_WF_Responsible_ID == 0)
{
SetAD_WF_Responsible_ID (0);
SetEntityType (null);	// U
SetName (null);
SetResponsibleType (null);
}
 */
}
public X_AD_WF_Responsible (Ctx ctx, int AD_WF_Responsible_ID, Trx trxName) : base (ctx, AD_WF_Responsible_ID, trxName)
{
/** if (AD_WF_Responsible_ID == 0)
{
SetAD_WF_Responsible_ID (0);
SetEntityType (null);	// U
SetName (null);
SetResponsibleType (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_Responsible (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_Responsible (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_Responsible (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_WF_Responsible()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514366357L;
/** Last Updated Timestamp 7/29/2010 1:07:29 PM */
public static long updatedMS = 1280389049568L;
/** AD_Table_ID=646 */
public static int Table_ID;
 // =646;

/** TableName=AD_WF_Responsible */
public static String Table_Name="AD_WF_Responsible";

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
StringBuilder sb = new StringBuilder ("X_AD_WF_Responsible[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Role.
@param AD_Role_ID Responsibility Role */
public void SetAD_Role_ID (int AD_Role_ID)
{
if (AD_Role_ID <= 0) Set_Value ("AD_Role_ID", null);
else
Set_Value ("AD_Role_ID", AD_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetAD_Role_ID() 
{
Object ii = Get_Value("AD_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** AD_User_ID AD_Reference_ID=286 */
public static int AD_USER_ID_AD_Reference_ID=286;
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);
else
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Workflow Responsible.
@param AD_WF_Responsible_ID Responsible for Workflow Execution */
public void SetAD_WF_Responsible_ID (int AD_WF_Responsible_ID)
{
if (AD_WF_Responsible_ID < 1) throw new ArgumentException ("AD_WF_Responsible_ID is mandatory.");
Set_ValueNoCheck ("AD_WF_Responsible_ID", AD_WF_Responsible_ID);
}
/** Get Workflow Responsible.
@return Responsible for Workflow Execution */
public int GetAD_WF_Responsible_ID() 
{
Object ii = Get_Value("AD_WF_Responsible_ID");
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

/** ResponsibleType AD_Reference_ID=304 */
public static int RESPONSIBLETYPE_AD_Reference_ID=304;
/** Human = H */
public static String RESPONSIBLETYPE_Human = "H";
/** Organization = O */
public static String RESPONSIBLETYPE_Organization = "O";
/** Role = R */
public static String RESPONSIBLETYPE_Role = "R";
/** System Resource = S */
public static String RESPONSIBLETYPE_SystemResource = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsResponsibleTypeValid (String test)
{
return test.Equals("H") || test.Equals("O") || test.Equals("R") || test.Equals("S");
}
/** Set Responsible Type.
@param ResponsibleType Type of the Responsibility for a workflow */
public void SetResponsibleType (String ResponsibleType)
{
if (ResponsibleType == null) throw new ArgumentException ("ResponsibleType is mandatory");
if (!IsResponsibleTypeValid(ResponsibleType))
throw new ArgumentException ("ResponsibleType Invalid value - " + ResponsibleType + " - Reference_ID=304 - H - O - R - S");
if (ResponsibleType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ResponsibleType = ResponsibleType.Substring(0,1);
}
Set_Value ("ResponsibleType", ResponsibleType);
}
/** Get Responsible Type.
@return Type of the Responsibility for a workflow */
public String GetResponsibleType() 
{
return (String)Get_Value("ResponsibleType");
}
}

}
