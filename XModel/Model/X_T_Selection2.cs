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
/** Generated Model for VAT_Selection2
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAT_Selection2 : PO
{
public X_VAT_Selection2 (Context ctx, int VAT_Selection2_ID, Trx trxName) : base (ctx, VAT_Selection2_ID, trxName)
{
/** if (VAT_Selection2_ID == 0)
{
SetQuery_ID (0);
SetVAT_Selection_ID (0);
}
 */
}
public X_VAT_Selection2 (Ctx ctx, int VAT_Selection2_ID, Trx trxName) : base (ctx, VAT_Selection2_ID, trxName)
{
/** if (VAT_Selection2_ID == 0)
{
SetQuery_ID (0);
SetVAT_Selection_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_Selection2 (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_Selection2 (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_Selection2 (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAT_Selection2()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384490L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067701L;
/** VAF_TableView_ID=918 */
public static int Table_ID;
 // =918;

/** TableName=VAT_Selection2 */
public static String Table_Name="VAT_Selection2";

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
StringBuilder sb = new StringBuilder ("X_VAT_Selection2[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Query.
@param Query_ID Query */
public void SetQuery_ID (int Query_ID)
{
if (Query_ID < 1) throw new ArgumentException ("Query_ID is mandatory.");
Set_Value ("Query_ID", Query_ID);
}
/** Get Query.
@return Query */
public int GetQuery_ID() 
{
Object ii = Get_Value("Query_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Selection.
@param VAT_Selection_ID *** DO NOT USE *** */
public void SetVAT_Selection_ID (int VAT_Selection_ID)
{
if (VAT_Selection_ID < 1) throw new ArgumentException ("VAT_Selection_ID is mandatory.");
Set_Value ("VAT_Selection_ID", VAT_Selection_ID);
}
/** Get Selection.
@return *** DO NOT USE *** */
public int GetVAT_Selection_ID() 
{
Object ii = Get_Value("VAT_Selection_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
