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
/** Generated Model for VAM_LotControlExclude
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_LotControlExclude : PO
{
public X_VAM_LotControlExclude (Context ctx, int VAM_LotControlExclude_ID, Trx trxName) : base (ctx, VAM_LotControlExclude_ID, trxName)
{
/** if (VAM_LotControlExclude_ID == 0)
{
SetVAF_TableView_ID (0);
SetIsSOTrx (false);
SetVAM_LotControlExclude_ID (0);
SetVAM_LotControl_ID (0);
}
 */
}
public X_VAM_LotControlExclude (Ctx ctx, int VAM_LotControlExclude_ID, Trx trxName) : base (ctx, VAM_LotControlExclude_ID, trxName)
{
/** if (VAM_LotControlExclude_ID == 0)
{
SetVAF_TableView_ID (0);
SetIsSOTrx (false);
SetVAM_LotControlExclude_ID (0);
SetVAM_LotControl_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_LotControlExclude (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_LotControlExclude (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_LotControlExclude (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_LotControlExclude()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379961L;
/** Last Updated Timestamp 7/29/2010 1:07:43 PM */
public static long updatedMS = 1280389063172L;
/** VAF_TableView_ID=810 */
public static int Table_ID;
 // =810;

/** TableName=VAM_LotControlExclude */
public static String Table_Name="VAM_LotControlExclude";

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
StringBuilder sb = new StringBuilder ("X_VAM_LotControlExclude[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID)
{
if (VAF_TableView_ID < 1) throw new ArgumentException ("VAF_TableView_ID is mandatory.");
Set_Value ("VAF_TableView_ID", VAF_TableView_ID);
}
/** Get Table.
@return Database Table information */
public int GetVAF_TableView_ID() 
{
Object ii = Get_Value("VAF_TableView_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sales Transaction.
@param IsSOTrx This is a Sales Transaction */
public void SetIsSOTrx (Boolean IsSOTrx)
{
Set_Value ("IsSOTrx", IsSOTrx);
}
/** Get Sales Transaction.
@return This is a Sales Transaction */
public Boolean IsSOTrx() 
{
Object oo = Get_Value("IsSOTrx");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Exclude Lot.
@param VAM_LotControlExclude_ID Exclude the ability to create Lots in Attribute Sets */
public void SetVAM_LotControlExclude_ID (int VAM_LotControlExclude_ID)
{
if (VAM_LotControlExclude_ID < 1) throw new ArgumentException ("VAM_LotControlExclude_ID is mandatory.");
Set_ValueNoCheck ("VAM_LotControlExclude_ID", VAM_LotControlExclude_ID);
}
/** Get Exclude Lot.
@return Exclude the ability to create Lots in Attribute Sets */
public int GetVAM_LotControlExclude_ID() 
{
Object ii = Get_Value("VAM_LotControlExclude_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Lot Control.
@param VAM_LotControl_ID Product Lot Control */
public void SetVAM_LotControl_ID (int VAM_LotControl_ID)
{
if (VAM_LotControl_ID < 1) throw new ArgumentException ("VAM_LotControl_ID is mandatory.");
Set_ValueNoCheck ("VAM_LotControl_ID", VAM_LotControl_ID);
}
/** Get Lot Control.
@return Product Lot Control */
public int GetVAM_LotControl_ID() 
{
Object ii = Get_Value("VAM_LotControl_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
