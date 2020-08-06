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
/** Generated Model for D_ChartAccess
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_D_ChartAccess : PO
{
public X_D_ChartAccess (Context ctx, int D_ChartAccess_ID, Trx trxName) : base (ctx, D_ChartAccess_ID, trxName)
{
/** if (D_ChartAccess_ID == 0)
{
SetD_ChartAccess_ID (0);
SetD_Chart_ID (0);
}
 */
}
public X_D_ChartAccess (Ctx ctx, int D_ChartAccess_ID, Trx trxName) : base (ctx, D_ChartAccess_ID, trxName)
{
/** if (D_ChartAccess_ID == 0)
{
SetD_ChartAccess_ID (0);
SetD_Chart_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_D_ChartAccess (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_D_ChartAccess (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_D_ChartAccess (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_D_ChartAccess()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376011L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059222L;
/** AD_Table_ID=1000006 */
public static int Table_ID;
 // =1000006;

/** TableName=D_ChartAccess */
public static String Table_Name="D_ChartAccess";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(7);
/** AccessLevel
@return 7 - System - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_D_ChartAccess[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Role.
@param AD_Role_ID Responsibility Role */
public void SetAD_Role_ID (int AD_Role_ID)
{
if (AD_Role_ID <= 0) Set_Value ("AD_Role_ID", null);
else
Set_Value ("AD_Role_ID", AD_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetAD_Role_ID() 
{
Object ii = Get_Value("AD_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set D_ChartAccess_ID.
@param D_ChartAccess_ID D_ChartAccess_ID */
public void SetD_ChartAccess_ID (int D_ChartAccess_ID)
{
if (D_ChartAccess_ID < 1) throw new ArgumentException ("D_ChartAccess_ID is mandatory.");
Set_ValueNoCheck ("D_ChartAccess_ID", D_ChartAccess_ID);
}
/** Get D_ChartAccess_ID.
@return D_ChartAccess_ID */
public int GetD_ChartAccess_ID() 
{
Object ii = Get_Value("D_ChartAccess_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Chart Name.
@param D_Chart_ID Chart Name */
public void SetD_Chart_ID (int D_Chart_ID)
{
if (D_Chart_ID < 1) throw new ArgumentException ("D_Chart_ID is mandatory.");
Set_ValueNoCheck ("D_Chart_ID", D_Chart_ID);
}
/** Get Chart Name.
@return Chart Name */
public int GetD_Chart_ID() 
{
Object ii = Get_Value("D_Chart_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Read Write.
@param IsReadWrite Field is read / write */
public void SetIsReadWrite (Boolean IsReadWrite)
{
Set_Value ("IsReadWrite", IsReadWrite);
}
/** Get Read Write.
@return Field is read / write */
public Boolean IsReadWrite() 
{
Object oo = Get_Value("IsReadWrite");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}
