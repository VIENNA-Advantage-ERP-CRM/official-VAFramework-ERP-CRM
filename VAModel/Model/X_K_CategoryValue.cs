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
/** Generated Model for K_CategoryValue
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_K_CategoryValue : PO
{
public X_K_CategoryValue (Context ctx, int K_CategoryValue_ID, Trx trxName) : base (ctx, K_CategoryValue_ID, trxName)
{
/** if (K_CategoryValue_ID == 0)
{
SetK_CategoryValue_ID (0);
SetK_Category_ID (0);
SetName (null);
}
 */
}
public X_K_CategoryValue (Ctx ctx, int K_CategoryValue_ID, Trx trxName) : base (ctx, K_CategoryValue_ID, trxName)
{
/** if (K_CategoryValue_ID == 0)
{
SetK_CategoryValue_ID (0);
SetK_Category_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_CategoryValue (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_CategoryValue (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_CategoryValue (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_K_CategoryValue()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514377955L;
/** Last Updated Timestamp 7/29/2010 1:07:41 PM */
public static long updatedMS = 1280389061166L;
/** AD_Table_ID=614 */
public static int Table_ID;
 // =614;

/** TableName=K_CategoryValue */
public static String Table_Name="K_CategoryValue";

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
StringBuilder sb = new StringBuilder ("X_K_CategoryValue[").Append(Get_ID()).Append("]");
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
/** Set Category Value.
@param K_CategoryValue_ID The value of the category */
public void SetK_CategoryValue_ID (int K_CategoryValue_ID)
{
if (K_CategoryValue_ID < 1) throw new ArgumentException ("K_CategoryValue_ID is mandatory.");
Set_ValueNoCheck ("K_CategoryValue_ID", K_CategoryValue_ID);
}
/** Get Category Value.
@return The value of the category */
public int GetK_CategoryValue_ID() 
{
Object ii = Get_Value("K_CategoryValue_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Knowledge Category.
@param K_Category_ID Knowledge Category */
public void SetK_Category_ID (int K_Category_ID)
{
if (K_Category_ID < 1) throw new ArgumentException ("K_Category_ID is mandatory.");
Set_ValueNoCheck ("K_Category_ID", K_Category_ID);
}
/** Get Knowledge Category.
@return Knowledge Category */
public int GetK_Category_ID() 
{
Object ii = Get_Value("K_Category_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
