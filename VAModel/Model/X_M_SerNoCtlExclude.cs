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
/** Generated Model for M_SerNoCtlExclude
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_SerNoCtlExclude : PO
{
public X_M_SerNoCtlExclude (Context ctx, int M_SerNoCtlExclude_ID, Trx trxName) : base (ctx, M_SerNoCtlExclude_ID, trxName)
{
/** if (M_SerNoCtlExclude_ID == 0)
{
SetAD_Table_ID (0);
SetIsSOTrx (false);
SetM_SerNoCtlExclude_ID (0);
SetM_SerNoCtl_ID (0);
}
 */
}
public X_M_SerNoCtlExclude (Ctx ctx, int M_SerNoCtlExclude_ID, Trx trxName) : base (ctx, M_SerNoCtlExclude_ID, trxName)
{
/** if (M_SerNoCtlExclude_ID == 0)
{
SetAD_Table_ID (0);
SetIsSOTrx (false);
SetM_SerNoCtlExclude_ID (0);
SetM_SerNoCtl_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_SerNoCtlExclude (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_SerNoCtlExclude (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_SerNoCtlExclude (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_SerNoCtlExclude()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381309L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064520L;
/** AD_Table_ID=811 */
public static int Table_ID;
 // =811;

/** TableName=M_SerNoCtlExclude */
public static String Table_Name="M_SerNoCtlExclude";

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
StringBuilder sb = new StringBuilder ("X_M_SerNoCtlExclude[").Append(Get_ID()).Append("]");
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
/** Set Exclude SerNo.
@param M_SerNoCtlExclude_ID Exclude the ability to create Serial Numbers in Attribute Sets */
public void SetM_SerNoCtlExclude_ID (int M_SerNoCtlExclude_ID)
{
if (M_SerNoCtlExclude_ID < 1) throw new ArgumentException ("M_SerNoCtlExclude_ID is mandatory.");
Set_ValueNoCheck ("M_SerNoCtlExclude_ID", M_SerNoCtlExclude_ID);
}
/** Get Exclude SerNo.
@return Exclude the ability to create Serial Numbers in Attribute Sets */
public int GetM_SerNoCtlExclude_ID() 
{
Object ii = Get_Value("M_SerNoCtlExclude_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Serial No Control.
@param M_SerNoCtl_ID Product Serial Number Control */
public void SetM_SerNoCtl_ID (int M_SerNoCtl_ID)
{
if (M_SerNoCtl_ID < 1) throw new ArgumentException ("M_SerNoCtl_ID is mandatory.");
Set_ValueNoCheck ("M_SerNoCtl_ID", M_SerNoCtl_ID);
}
/** Get Serial No Control.
@return Product Serial Number Control */
public int GetM_SerNoCtl_ID() 
{
Object ii = Get_Value("M_SerNoCtl_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
