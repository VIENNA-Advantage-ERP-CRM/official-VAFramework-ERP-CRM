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
/** Generated Model for T_Selection
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_T_Selection : PO
{
public X_T_Selection (Context ctx, int T_Selection_ID, Trx trxName) : base (ctx, T_Selection_ID, trxName)
{
/** if (T_Selection_ID == 0)
{
SetT_Selection_ID (0);
}
 */
}
public X_T_Selection (Ctx ctx, int T_Selection_ID, Trx trxName) : base (ctx, T_Selection_ID, trxName)
{
/** if (T_Selection_ID == 0)
{
SetT_Selection_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_Selection (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_Selection (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_Selection (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_T_Selection()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384459L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067670L;
/** AD_Table_ID=917 */
public static int Table_ID;
 // =917;

/** TableName=T_Selection */
public static String Table_Name="T_Selection";

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
StringBuilder sb = new StringBuilder ("X_T_Selection[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Selection.
@param T_Selection_ID *** DO NOT USE *** */
public void SetT_Selection_ID (int T_Selection_ID)
{
if (T_Selection_ID < 1) throw new ArgumentException ("T_Selection_ID is mandatory.");
Set_ValueNoCheck ("T_Selection_ID", T_Selection_ID);
}
/** Get Selection.
@return *** DO NOT USE *** */
public int GetT_Selection_ID() 
{
Object ii = Get_Value("T_Selection_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
