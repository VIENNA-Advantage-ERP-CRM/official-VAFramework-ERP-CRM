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
/** Generated Model for VAB_ProjectCycleStage
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_ProjectCycleStage : PO
{
public X_VAB_ProjectCycleStage (Context ctx, int VAB_ProjectCycleStage_ID, Trx trxName) : base (ctx, VAB_ProjectCycleStage_ID, trxName)
{
/** if (VAB_ProjectCycleStage_ID == 0)
{
SetVAB_ProjectCycleStep_ID (0);
SetC_Phase_ID (0);
}
 */
}
public X_VAB_ProjectCycleStage (Ctx ctx, int VAB_ProjectCycleStage_ID, Trx trxName) : base (ctx, VAB_ProjectCycleStage_ID, trxName)
{
/** if (VAB_ProjectCycleStage_ID == 0)
{
SetVAB_ProjectCycleStep_ID (0);
SetC_Phase_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectCycleStage (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectCycleStage (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectCycleStage (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_ProjectCycleStage()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371607L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054818L;
/** VAF_TableView_ID=433 */
public static int Table_ID;
 // =433;

/** TableName=VAB_ProjectCycleStage */
public static String Table_Name="VAB_ProjectCycleStage";

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
StringBuilder sb = new StringBuilder ("X_VAB_ProjectCycleStage[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Cycle Step.
@param VAB_ProjectCycleStep_ID The step for this Cycle */
public void SetVAB_ProjectCycleStep_ID (int VAB_ProjectCycleStep_ID)
{
if (VAB_ProjectCycleStep_ID < 1) throw new ArgumentException ("VAB_ProjectCycleStep_ID is mandatory.");
Set_ValueNoCheck ("VAB_ProjectCycleStep_ID", VAB_ProjectCycleStep_ID);
}
/** Get Cycle Step.
@return The step for this Cycle */
public int GetVAB_ProjectCycleStep_ID() 
{
Object ii = Get_Value("VAB_ProjectCycleStep_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_ProjectCycleStep_ID().ToString());
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
