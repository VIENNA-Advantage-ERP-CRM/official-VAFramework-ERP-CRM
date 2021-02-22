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
/** Generated Model for VAW_TickCount
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAW_TickCount : PO
{
public X_VAW_TickCount (Context ctx, int VAW_TickCount_ID, Trx trxName) : base (ctx, VAW_TickCount_ID, trxName)
{
/** if (VAW_TickCount_ID == 0)
{
SetName (null);
SetTargetURL (null);
SetVAW_TickCount_ID (0);
}
 */
}
public X_VAW_TickCount (Ctx ctx, int VAW_TickCount_ID, Trx trxName) : base (ctx, VAW_TickCount_ID, trxName)
{
/** if (VAW_TickCount_ID == 0)
{
SetName (null);
SetTargetURL (null);
SetVAW_TickCount_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAW_TickCount (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAW_TickCount (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAW_TickCount (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAW_TickCount()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384960L;
/** Last Updated Timestamp 7/29/2010 1:07:48 PM */
public static long updatedMS = 1280389068171L;
/** VAF_TableView_ID=553 */
public static int Table_ID;
 // =553;

/** TableName=VAW_TickCount */
public static String Table_Name="VAW_TickCount";

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
StringBuilder sb = new StringBuilder ("X_VAW_TickCount[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** VAB_BusinessPartner_ID VAF_Control_Ref_ID=232 */
public static int VAB_BUSINESSPARTNER_ID_VAF_Control_Ref_ID=232;
/** Set Business Partner.
@param VAB_BusinessPartner_ID Identifies a Business Partner */
public void SetVAB_BusinessPartner_ID (int VAB_BusinessPartner_ID)
{
if (VAB_BusinessPartner_ID <= 0) Set_Value ("VAB_BusinessPartner_ID", null);
else
Set_Value ("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetVAB_BusinessPartner_ID() 
{
Object ii = Get_Value("VAB_BusinessPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Counter.
@param Counter Count Value */
public void SetCounter (int Counter)
{
throw new ArgumentException ("Counter Is virtual column");
}
/** Get Counter.
@return Count Value */
public int GetCounter() 
{
Object ii = Get_Value("Counter");
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
/** Set Target URL.
@param TargetURL URL for the Target */
public void SetTargetURL (String TargetURL)
{
if (TargetURL == null) throw new ArgumentException ("TargetURL is mandatory.");
if (TargetURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
TargetURL = TargetURL.Substring(0,120);
}
Set_Value ("TargetURL", TargetURL);
}
/** Get Target URL.
@return URL for the Target */
public String GetTargetURL() 
{
return (String)Get_Value("TargetURL");
}
/** Set Click Count.
@param VAW_TickCount_ID Web Click Management */
public void SetVAW_TickCount_ID (int VAW_TickCount_ID)
{
if (VAW_TickCount_ID < 1) throw new ArgumentException ("VAW_TickCount_ID is mandatory.");
Set_ValueNoCheck ("VAW_TickCount_ID", VAW_TickCount_ID);
}
/** Get Click Count.
@return Web Click Management */
public int GetVAW_TickCount_ID() 
{
Object ii = Get_Value("VAW_TickCount_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
