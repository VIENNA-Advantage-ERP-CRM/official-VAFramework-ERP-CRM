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
/** Generated Model for C_GenAttributeSetExclude
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_GenAttributeSetExclude : PO
{
public X_C_GenAttributeSetExclude (Context ctx, int C_GenAttributeSetExclude_ID, Trx trxName) : base (ctx, C_GenAttributeSetExclude_ID, trxName)
{
/** if (C_GenAttributeSetExclude_ID == 0)
{
SetAD_Table_ID (0);
SetC_GenAttributeSetExclude_ID (0);
SetC_GenAttributeSet_ID (0);
SetIsSOTrx (false);
}
 */
}
public X_C_GenAttributeSetExclude (Ctx ctx, int C_GenAttributeSetExclude_ID, Trx trxName) : base (ctx, C_GenAttributeSetExclude_ID, trxName)
{
/** if (C_GenAttributeSetExclude_ID == 0)
{
SetAD_Table_ID (0);
SetC_GenAttributeSetExclude_ID (0);
SetC_GenAttributeSet_ID (0);
SetIsSOTrx (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeSetExclude (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeSetExclude (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeSetExclude (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_GenAttributeSetExclude()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27667169105344L;
/** Last Updated Timestamp 11/21/2013 7:53:08 PM */
public static long updatedMS = 1385043788555L;
/** AD_Table_ID=1000422 */
public static int Table_ID;
 // =1000422;

/** TableName=C_GenAttributeSetExclude */
public static String Table_Name="C_GenAttributeSetExclude";

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
StringBuilder sb = new StringBuilder ("X_C_GenAttributeSetExclude[").Append(Get_ID()).Append("]");
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
/** Set C_GenAttributeSetExclude_ID.
@param C_GenAttributeSetExclude_ID C_GenAttributeSetExclude_ID */
public void SetC_GenAttributeSetExclude_ID (int C_GenAttributeSetExclude_ID)
{
if (C_GenAttributeSetExclude_ID < 1) throw new ArgumentException ("C_GenAttributeSetExclude_ID is mandatory.");
Set_ValueNoCheck ("C_GenAttributeSetExclude_ID", C_GenAttributeSetExclude_ID);
}
/** Get C_GenAttributeSetExclude_ID.
@return C_GenAttributeSetExclude_ID */
public int GetC_GenAttributeSetExclude_ID() 
{
Object ii = Get_Value("C_GenAttributeSetExclude_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set C_GenAttributeSet_ID.
@param C_GenAttributeSet_ID C_GenAttributeSet_ID */
public void SetC_GenAttributeSet_ID (int C_GenAttributeSet_ID)
{
if (C_GenAttributeSet_ID < 1) throw new ArgumentException ("C_GenAttributeSet_ID is mandatory.");
Set_ValueNoCheck ("C_GenAttributeSet_ID", C_GenAttributeSet_ID);
}
/** Get C_GenAttributeSet_ID.
@return C_GenAttributeSet_ID */
public int GetC_GenAttributeSet_ID() 
{
Object ii = Get_Value("C_GenAttributeSet_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID)
{
if (Export_ID != null && Export_ID.Length > 50)
{
log.Warning("Length > 50 - truncated");
Export_ID = Export_ID.Substring(0,50);
}
Set_Value ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
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
}

}
