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
/** Generated Model for VAM_ExcludeCtlSerialNo
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_ExcludeCtlSerialNo : PO
{
public X_VAM_ExcludeCtlSerialNo (Context ctx, int VAM_ExcludeCtlSerialNo_ID, Trx trxName) : base (ctx, VAM_ExcludeCtlSerialNo_ID, trxName)
{
/** if (VAM_ExcludeCtlSerialNo_ID == 0)
{
SetVAF_TableView_ID (0);
SetIsSOTrx (false);
SetVAM_ExcludeCtlSerialNo_ID (0);
SetVAM_CtlSerialNo_ID (0);
}
 */
}
public X_VAM_ExcludeCtlSerialNo (Ctx ctx, int VAM_ExcludeCtlSerialNo_ID, Trx trxName) : base (ctx, VAM_ExcludeCtlSerialNo_ID, trxName)
{
/** if (VAM_ExcludeCtlSerialNo_ID == 0)
{
SetVAF_TableView_ID (0);
SetIsSOTrx (false);
SetVAM_ExcludeCtlSerialNo_ID (0);
SetVAM_CtlSerialNo_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ExcludeCtlSerialNo (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ExcludeCtlSerialNo (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ExcludeCtlSerialNo (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_ExcludeCtlSerialNo()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381309L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064520L;
/** VAF_TableView_ID=811 */
public static int Table_ID;
 // =811;

/** TableName=VAM_ExcludeCtlSerialNo */
public static String Table_Name="VAM_ExcludeCtlSerialNo";

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
StringBuilder sb = new StringBuilder ("X_VAM_ExcludeCtlSerialNo[").Append(Get_ID()).Append("]");
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
/** Set Exclude SerNo.
@param VAM_ExcludeCtlSerialNo_ID Exclude the ability to create Serial Numbers in Attribute Sets */
public void SetVAM_ExcludeCtlSerialNo_ID (int VAM_ExcludeCtlSerialNo_ID)
{
if (VAM_ExcludeCtlSerialNo_ID < 1) throw new ArgumentException ("VAM_ExcludeCtlSerialNo_ID is mandatory.");
Set_ValueNoCheck ("VAM_ExcludeCtlSerialNo_ID", VAM_ExcludeCtlSerialNo_ID);
}
/** Get Exclude SerNo.
@return Exclude the ability to create Serial Numbers in Attribute Sets */
public int GetVAM_ExcludeCtlSerialNo_ID() 
{
Object ii = Get_Value("VAM_ExcludeCtlSerialNo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Serial No Control.
@param VAM_CtlSerialNo_ID Product Serial Number Control */
public void SetVAM_CtlSerialNo_ID (int VAM_CtlSerialNo_ID)
{
if (VAM_CtlSerialNo_ID < 1) throw new ArgumentException ("VAM_CtlSerialNo_ID is mandatory.");
Set_ValueNoCheck ("VAM_CtlSerialNo_ID", VAM_CtlSerialNo_ID);
}
/** Get Serial No Control.
@return Product Serial Number Control */
public int GetVAM_CtlSerialNo_ID() 
{
Object ii = Get_Value("VAM_CtlSerialNo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
