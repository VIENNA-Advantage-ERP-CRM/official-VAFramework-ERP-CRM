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
/** Generated Model for C_CyclePhase
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_CyclePhase : PO
{
public X_C_CyclePhase (Context ctx, int C_CyclePhase_ID, Trx trxName) : base (ctx, C_CyclePhase_ID, trxName)
{
/** if (C_CyclePhase_ID == 0)
{
SetC_CycleStep_ID (0);
SetC_Phase_ID (0);
}
 */
}
public X_C_CyclePhase (Ctx ctx, int C_CyclePhase_ID, Trx trxName) : base (ctx, C_CyclePhase_ID, trxName)
{
/** if (C_CyclePhase_ID == 0)
{
SetC_CycleStep_ID (0);
SetC_Phase_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CyclePhase (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CyclePhase (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CyclePhase (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_CyclePhase()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371607L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054818L;
/** AD_Table_ID=433 */
public static int Table_ID;
 // =433;

/** TableName=C_CyclePhase */
public static String Table_Name="C_CyclePhase";

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
StringBuilder sb = new StringBuilder ("X_C_CyclePhase[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Cycle Step.
@param C_CycleStep_ID The step for this Cycle */
public void SetC_CycleStep_ID (int C_CycleStep_ID)
{
if (C_CycleStep_ID < 1) throw new ArgumentException ("C_CycleStep_ID is mandatory.");
Set_ValueNoCheck ("C_CycleStep_ID", C_CycleStep_ID);
}
/** Get Cycle Step.
@return The step for this Cycle */
public int GetC_CycleStep_ID() 
{
Object ii = Get_Value("C_CycleStep_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_CycleStep_ID().ToString());
}
/** Set Standard Phase.
@param C_Phase_ID Standard Phase of the Project Type */
public void SetC_Phase_ID (int C_Phase_ID)
{
if (C_Phase_ID < 1) throw new ArgumentException ("C_Phase_ID is mandatory.");
Set_ValueNoCheck ("C_Phase_ID", C_Phase_ID);
}
/** Get Standard Phase.
@return Standard Phase of the Project Type */
public int GetC_Phase_ID() 
{
Object ii = Get_Value("C_Phase_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
