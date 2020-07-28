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
/** Generated Model for M_RMACategory
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_RMACategory : PO
{
public X_M_RMACategory (Context ctx, int M_RMACategory_ID, Trx trxName) : base (ctx, M_RMACategory_ID, trxName)
{
/** if (M_RMACategory_ID == 0)
{
SetM_RMACategory_ID (0);
SetName (null);
}
 */
}
public X_M_RMACategory (Ctx ctx, int M_RMACategory_ID, Trx trxName) : base (ctx, M_RMACategory_ID, trxName)
{
/** if (M_RMACategory_ID == 0)
{
SetM_RMACategory_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_RMACategory (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_RMACategory (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_RMACategory (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_RMACategory()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381058L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064269L;
/** AD_Table_ID=987 */
public static int Table_ID;
 // =987;

/** TableName=M_RMACategory */
public static String Table_Name="M_RMACategory";

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
StringBuilder sb = new StringBuilder ("X_M_RMACategory[").Append(Get_ID()).Append("]");
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
/** Set RMA Category.
@param M_RMACategory_ID Return Material Authorization Category */
public void SetM_RMACategory_ID (int M_RMACategory_ID)
{
if (M_RMACategory_ID < 1) throw new ArgumentException ("M_RMACategory_ID is mandatory.");
Set_ValueNoCheck ("M_RMACategory_ID", M_RMACategory_ID);
}
/** Get RMA Category.
@return Return Material Authorization Category */
public int GetM_RMACategory_ID() 
{
Object ii = Get_Value("M_RMACategory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
