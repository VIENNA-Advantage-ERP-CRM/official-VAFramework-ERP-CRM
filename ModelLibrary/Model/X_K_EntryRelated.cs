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
/** Generated Model for K_EntryRelated
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_K_EntryRelated : PO
{
public X_K_EntryRelated (Context ctx, int K_EntryRelated_ID, Trx trxName) : base (ctx, K_EntryRelated_ID, trxName)
{
/** if (K_EntryRelated_ID == 0)
{
SetK_EntryRelated_ID (0);
SetK_Entry_ID (0);
}
 */
}
public X_K_EntryRelated (Ctx ctx, int K_EntryRelated_ID, Trx trxName) : base (ctx, K_EntryRelated_ID, trxName)
{
/** if (K_EntryRelated_ID == 0)
{
SetK_EntryRelated_ID (0);
SetK_Entry_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_EntryRelated (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_EntryRelated (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_EntryRelated (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_K_EntryRelated()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514378017L;
/** Last Updated Timestamp 7/29/2010 1:07:41 PM */
public static long updatedMS = 1280389061228L;
/** AD_Table_ID=610 */
public static int Table_ID;
 // =610;

/** TableName=K_EntryRelated */
public static String Table_Name="K_EntryRelated";

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
StringBuilder sb = new StringBuilder ("X_K_EntryRelated[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** K_EntryRelated_ID AD_Reference_ID=285 */
public static int K_ENTRYRELATED_ID_AD_Reference_ID=285;
/** Set Related Entry.
@param K_EntryRelated_ID Related Entry for this Enntry */
public void SetK_EntryRelated_ID (int K_EntryRelated_ID)
{
if (K_EntryRelated_ID < 1) throw new ArgumentException ("K_EntryRelated_ID is mandatory.");
Set_ValueNoCheck ("K_EntryRelated_ID", K_EntryRelated_ID);
}
/** Get Related Entry.
@return Related Entry for this Enntry */
public int GetK_EntryRelated_ID() 
{
Object ii = Get_Value("K_EntryRelated_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetK_EntryRelated_ID().ToString());
}
/** Set Entry.
@param K_Entry_ID Knowledge Entry */
public void SetK_Entry_ID (int K_Entry_ID)
{
if (K_Entry_ID < 1) throw new ArgumentException ("K_Entry_ID is mandatory.");
Set_ValueNoCheck ("K_Entry_ID", K_Entry_ID);
}
/** Get Entry.
@return Knowledge Entry */
public int GetK_Entry_ID() 
{
Object ii = Get_Value("K_Entry_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 60)
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
