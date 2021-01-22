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
/** Generated Model for VAF_ModuleField
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_ModuleField : PO
{
public X_VAF_ModuleField (Context ctx, int VAF_ModuleField_ID, Trx trxName) : base (ctx, VAF_ModuleField_ID, trxName)
{
/** if (VAF_ModuleField_ID == 0)
{
SetVAF_ModuleField_ID (0);
SetVAF_ModuleTab_ID (0);
SetIsDisplayed (true);	// Y
SetName (null);
}
 */
}
public X_VAF_ModuleField (Ctx ctx, int VAF_ModuleField_ID, Trx trxName) : base (ctx, VAF_ModuleField_ID, trxName)
{
/** if (VAF_ModuleField_ID == 0)
{
SetVAF_ModuleField_ID (0);
SetVAF_ModuleTab_ID (0);
SetIsDisplayed (true);	// Y
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleField (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleField (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleField (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_ModuleField()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27622811900152L;
/** Last Updated Timestamp 6/26/2012 10:26:23 AM */
public static long updatedMS = 1340686583363L;
/** VAF_TableView_ID=1000059 */
public static int Table_ID;
 // =1000059;

/** TableName=VAF_ModuleField */
public static String Table_Name="VAF_ModuleField";

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
StringBuilder sb = new StringBuilder ("X_VAF_ModuleField[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Field.
@param VAF_Field_ID Field on a tab in a window */
public void SetVAF_Field_ID (int VAF_Field_ID)
{
if (VAF_Field_ID <= 0) Set_Value ("VAF_Field_ID", null);
else
Set_Value ("VAF_Field_ID", VAF_Field_ID);
}
/** Get Field.
@return Field on a tab in a window */
public int GetVAF_Field_ID() 
{
Object ii = Get_Value("VAF_Field_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAF_ModuleField_ID.
@param VAF_ModuleField_ID VAF_ModuleField_ID */
public void SetVAF_ModuleField_ID (int VAF_ModuleField_ID)
{
if (VAF_ModuleField_ID < 1) throw new ArgumentException ("VAF_ModuleField_ID is mandatory.");
Set_ValueNoCheck ("VAF_ModuleField_ID", VAF_ModuleField_ID);
}
/** Get VAF_ModuleField_ID.
@return VAF_ModuleField_ID */
public int GetVAF_ModuleField_ID() 
{
Object ii = Get_Value("VAF_ModuleField_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAF_ModuleTab_ID.
@param VAF_ModuleTab_ID VAF_ModuleTab_ID */
public void SetVAF_ModuleTab_ID (int VAF_ModuleTab_ID)
{
if (VAF_ModuleTab_ID < 1) throw new ArgumentException ("VAF_ModuleTab_ID is mandatory.");
Set_ValueNoCheck ("VAF_ModuleTab_ID", VAF_ModuleTab_ID);
}
/** Get VAF_ModuleTab_ID.
@return VAF_ModuleTab_ID */
public int GetVAF_ModuleTab_ID() 
{
Object ii = Get_Value("VAF_ModuleTab_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 50)
{
log.Warning("Length > 50 - truncated");
Description = Description.Substring(0,50);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Displayed.
@param IsDisplayed Determines, if this field is displayed */
public void SetIsDisplayed (Boolean IsDisplayed)
{
Set_Value ("IsDisplayed", IsDisplayed);
}
/** Get Displayed.
@return Determines, if this field is displayed */
public Boolean IsDisplayed() 
{
Object oo = Get_Value("IsDisplayed");
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
if (Name.Length > 50)
{
log.Warning("Length > 50 - truncated");
Name = Name.Substring(0,50);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
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
