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
/** Generated Model for B_TopicCategory
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_B_TopicCategory : PO
{
public X_B_TopicCategory (Context ctx, int B_TopicCategory_ID, Trx trxName) : base (ctx, B_TopicCategory_ID, trxName)
{
/** if (B_TopicCategory_ID == 0)
{
SetB_TopicCategory_ID (0);
SetB_TopicType_ID (0);
SetName (null);
}
 */
}
public X_B_TopicCategory (Ctx ctx, int B_TopicCategory_ID, Trx trxName) : base (ctx, B_TopicCategory_ID, trxName)
{
/** if (B_TopicCategory_ID == 0)
{
SetB_TopicCategory_ID (0);
SetB_TopicType_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_B_TopicCategory (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_B_TopicCategory (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_B_TopicCategory (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_B_TopicCategory()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514367564L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050775L;
/** AD_Table_ID=691 */
public static int Table_ID;
 // =691;

/** TableName=B_TopicCategory */
public static String Table_Name="B_TopicCategory";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_B_TopicCategory[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Topic Category.
@param B_TopicCategory_ID Auction Topic Category */
public void SetB_TopicCategory_ID (int B_TopicCategory_ID)
{
if (B_TopicCategory_ID < 1) throw new ArgumentException ("B_TopicCategory_ID is mandatory.");
Set_ValueNoCheck ("B_TopicCategory_ID", B_TopicCategory_ID);
}
/** Get Topic Category.
@return Auction Topic Category */
public int GetB_TopicCategory_ID() 
{
Object ii = Get_Value("B_TopicCategory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Topic Type.
@param B_TopicType_ID Auction Topic Type */
public void SetB_TopicType_ID (int B_TopicType_ID)
{
if (B_TopicType_ID < 1) throw new ArgumentException ("B_TopicType_ID is mandatory.");
Set_ValueNoCheck ("B_TopicType_ID", B_TopicType_ID);
}
/** Get Topic Type.
@return Auction Topic Type */
public int GetB_TopicType_ID() 
{
Object ii = Get_Value("B_TopicType_ID");
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
}

}
