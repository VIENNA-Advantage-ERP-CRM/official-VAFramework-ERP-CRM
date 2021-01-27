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
/** Generated Model for VAPA_FR_ColumnSet
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAPA_FR_ColumnSet : PO
{
public X_VAPA_FR_ColumnSet (Context ctx, int VAPA_FR_ColumnSet_ID, Trx trxName) : base (ctx, VAPA_FR_ColumnSet_ID, trxName)
{
/** if (VAPA_FR_ColumnSet_ID == 0)
{
SetName (null);
SetVAPA_FR_ColumnSet_ID (0);
SetProcessing (false);	// N
}
 */
}
public X_VAPA_FR_ColumnSet (Ctx ctx, int VAPA_FR_ColumnSet_ID, Trx trxName) : base (ctx, VAPA_FR_ColumnSet_ID, trxName)
{
/** if (VAPA_FR_ColumnSet_ID == 0)
{
SetName (null);
SetVAPA_FR_ColumnSet_ID (0);
SetProcessing (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_FR_ColumnSet (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_FR_ColumnSet (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_FR_ColumnSet (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAPA_FR_ColumnSet()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514382171L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065382L;
/** VAF_TableView_ID=447 */
public static int Table_ID;
 // =447;

/** TableName=VAPA_FR_ColumnSet */
public static String Table_Name="VAPA_FR_ColumnSet";

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
StringBuilder sb = new StringBuilder ("X_VAPA_FR_ColumnSet[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Report Column Set.
@param VAPA_FR_ColumnSet_ID Collection of Columns for Report */
public void SetVAPA_FR_ColumnSet_ID (int VAPA_FR_ColumnSet_ID)
{
if (VAPA_FR_ColumnSet_ID < 1) throw new ArgumentException ("VAPA_FR_ColumnSet_ID is mandatory.");
Set_ValueNoCheck ("VAPA_FR_ColumnSet_ID", VAPA_FR_ColumnSet_ID);
}
/** Get Report Column Set.
@return Collection of Columns for Report */
public int GetVAPA_FR_ColumnSet_ID() 
{
Object ii = Get_Value("VAPA_FR_ColumnSet_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}
