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
/** Generated Model for K_EntryCategory
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_K_EntryCategory : PO
{
public X_K_EntryCategory (Context ctx, int K_EntryCategory_ID, Trx trxName) : base (ctx, K_EntryCategory_ID, trxName)
{
/** if (K_EntryCategory_ID == 0)
{
SetK_CategoryValue_ID (0);
SetK_Category_ID (0);
SetK_Entry_ID (0);
}
 */
}
public X_K_EntryCategory (Ctx ctx, int K_EntryCategory_ID, Trx trxName) : base (ctx, K_EntryCategory_ID, trxName)
{
/** if (K_EntryCategory_ID == 0)
{
SetK_CategoryValue_ID (0);
SetK_Category_ID (0);
SetK_Entry_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_EntryCategory (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_EntryCategory (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_EntryCategory (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_K_EntryCategory()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514378017L;
/** Last Updated Timestamp 7/29/2010 1:07:41 PM */
public static long updatedMS = 1280389061228L;
/** AD_Table_ID=611 */
public static int Table_ID;
 // =611;

/** TableName=K_EntryCategory */
public static String Table_Name="K_EntryCategory";

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
StringBuilder sb = new StringBuilder ("X_K_EntryCategory[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Category Value.
@param K_CategoryValue_ID The value of the category */
public void SetK_CategoryValue_ID (int K_CategoryValue_ID)
{
if (K_CategoryValue_ID < 1) throw new ArgumentException ("K_CategoryValue_ID is mandatory.");
Set_Value ("K_CategoryValue_ID", K_CategoryValue_ID);
}
/** Get Category Value.
@return The value of the category */
public int GetK_CategoryValue_ID() 
{
Object ii = Get_Value("K_CategoryValue_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetK_CategoryValue_ID().ToString());
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
}

}
