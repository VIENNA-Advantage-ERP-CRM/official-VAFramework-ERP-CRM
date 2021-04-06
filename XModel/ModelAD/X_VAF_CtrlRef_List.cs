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
/** Generated Model for VAF_CtrlRef_List
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_CtrlRef_List : PO
{
public X_VAF_CtrlRef_List (Context ctx, int VAF_CtrlRef_List_ID, Trx trxName) : base (ctx, VAF_CtrlRef_List_ID, trxName)
{
/** if (VAF_CtrlRef_List_ID == 0)
{
SetVAF_CtrlRef_List_ID (0);
SetVAF_Control_Ref_ID (0);
SetRecordType (null);	// U
SetName (null);
SetValue (null);
}
 */
}
public X_VAF_CtrlRef_List (Ctx ctx, int VAF_CtrlRef_List_ID, Trx trxName) : base (ctx, VAF_CtrlRef_List_ID, trxName)
{
/** if (VAF_CtrlRef_List_ID == 0)
{
SetVAF_CtrlRef_List_ID (0);
SetVAF_Control_Ref_ID (0);
SetRecordType (null);	// U
SetName (null);
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_CtrlRef_List (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_CtrlRef_List (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_CtrlRef_List (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_CtrlRef_List()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514363317L;
/** Last Updated Timestamp 7/29/2010 1:07:26 PM */
public static long updatedMS = 1280389046528L;
/** VAF_TableView_ID=104 */
public static int Table_ID;
 // =104;

/** TableName=VAF_CtrlRef_List */
public static String Table_Name="VAF_CtrlRef_List";

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
StringBuilder sb = new StringBuilder ("X_VAF_CtrlRef_List[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Reference List.
@param VAF_CtrlRef_List_ID Reference List based on Table */
public void SetVAF_CtrlRef_List_ID (int VAF_CtrlRef_List_ID)
{
if (VAF_CtrlRef_List_ID < 1) throw new ArgumentException ("VAF_CtrlRef_List_ID is mandatory.");
Set_ValueNoCheck ("VAF_CtrlRef_List_ID", VAF_CtrlRef_List_ID);
}
/** Get Reference List.
@return Reference List based on Table */
public int GetVAF_CtrlRef_List_ID() 
{
Object ii = Get_Value("VAF_CtrlRef_List_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Reference.
@param VAF_Control_Ref_ID System Reference and Validation */
public void SetVAF_Control_Ref_ID (int VAF_Control_Ref_ID)
{
if (VAF_Control_Ref_ID < 1) throw new ArgumentException ("VAF_Control_Ref_ID is mandatory.");
Set_ValueNoCheck ("VAF_Control_Ref_ID", VAF_Control_Ref_ID);
}
/** Get Reference.
@return System Reference and Validation */
public int GetVAF_Control_Ref_ID() 
{
Object ii = Get_Value("VAF_Control_Ref_ID");
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

/** RecordType VAF_Control_Ref_ID=389 */
public static int RecordType_VAF_Control_Ref_ID=389;
/** Set Entity Type.
@param RecordType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetRecordType (String RecordType)
{
if (RecordType.Length > 4)
{
log.Warning("Length > 4 - truncated");
RecordType = RecordType.Substring(0,4);
}
Set_Value ("RecordType", RecordType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetRecordType() 
{
return (String)Get_Value("RecordType");
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
/** Set Valid from.
@param ValidFrom Valid from including this date (first day) */
public void SetValidFrom (DateTime? ValidFrom)
{
Set_Value ("ValidFrom", (DateTime?)ValidFrom);
}
/** Get Valid from.
@return Valid from including this date (first day) */
public DateTime? GetValidFrom() 
{
return (DateTime?)Get_Value("ValidFrom");
}
/** Set Valid to.
@param ValidTo Valid to including this date (last day) */
public void SetValidTo (DateTime? ValidTo)
{
Set_Value ("ValidTo", (DateTime?)ValidTo);
}
/** Get Valid to.
@return Valid to including this date (last day) */
public DateTime? GetValidTo() 
{
return (DateTime?)Get_Value("ValidTo");
}
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value == null) throw new ArgumentException ("Value is mandatory.");
if (Value.Length > 60)
{
log.Warning("Length > 60 - truncated");
Value = Value.Substring(0,60);
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
