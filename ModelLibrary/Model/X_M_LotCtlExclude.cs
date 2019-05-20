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
/** Generated Model for M_LotCtlExclude
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_LotCtlExclude : PO
{
public X_M_LotCtlExclude (Context ctx, int M_LotCtlExclude_ID, Trx trxName) : base (ctx, M_LotCtlExclude_ID, trxName)
{
/** if (M_LotCtlExclude_ID == 0)
{
SetAD_Table_ID (0);
SetIsSOTrx (false);
SetM_LotCtlExclude_ID (0);
SetM_LotCtl_ID (0);
}
 */
}
public X_M_LotCtlExclude (Ctx ctx, int M_LotCtlExclude_ID, Trx trxName) : base (ctx, M_LotCtlExclude_ID, trxName)
{
/** if (M_LotCtlExclude_ID == 0)
{
SetAD_Table_ID (0);
SetIsSOTrx (false);
SetM_LotCtlExclude_ID (0);
SetM_LotCtl_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_LotCtlExclude (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_LotCtlExclude (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_LotCtlExclude (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_LotCtlExclude()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379961L;
/** Last Updated Timestamp 7/29/2010 1:07:43 PM */
public static long updatedMS = 1280389063172L;
/** AD_Table_ID=810 */
public static int Table_ID;
 // =810;

/** TableName=M_LotCtlExclude */
public static String Table_Name="M_LotCtlExclude";

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
StringBuilder sb = new StringBuilder ("X_M_LotCtlExclude[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID < 1) throw new ArgumentException ("AD_Table_ID is mandatory.");
Set_Value ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
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
@param M_LotCtlExclude_ID Exclude the ability to create Lots in Attribute Sets */
public void SetM_LotCtlExclude_ID (int M_LotCtlExclude_ID)
{
if (M_LotCtlExclude_ID < 1) throw new ArgumentException ("M_LotCtlExclude_ID is mandatory.");
Set_ValueNoCheck ("M_LotCtlExclude_ID", M_LotCtlExclude_ID);
}
/** Get Exclude Lot.
@return Exclude the ability to create Lots in Attribute Sets */
public int GetM_LotCtlExclude_ID() 
{
Object ii = Get_Value("M_LotCtlExclude_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Lot Control.
@param M_LotCtl_ID Product Lot Control */
public void SetM_LotCtl_ID (int M_LotCtl_ID)
{
if (M_LotCtl_ID < 1) throw new ArgumentException ("M_LotCtl_ID is mandatory.");
Set_ValueNoCheck ("M_LotCtl_ID", M_LotCtl_ID);
}
/** Get Lot Control.
@return Product Lot Control */
public int GetM_LotCtl_ID() 
{
Object ii = Get_Value("M_LotCtl_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
