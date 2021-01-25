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
/** Generated Model for VAB_ProjectCycleStep
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_ProjectCycleStep : PO
{
public X_VAB_ProjectCycleStep (Context ctx, int VAB_ProjectCycleStep_ID, Trx trxName) : base (ctx, VAB_ProjectCycleStep_ID, trxName)
{
/** if (VAB_ProjectCycleStep_ID == 0)
{
SetVAB_ProjectCycleStep_ID (0);
SetVAB_ProjectCycle_ID (0);
SetName (null);
SetRelativeWeight (0.0);	// 1
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM VAB_ProjectCycleStep WHERE VAB_ProjectCycle_ID=@VAB_ProjectCycle_ID@
}
 */
}
public X_VAB_ProjectCycleStep (Ctx ctx, int VAB_ProjectCycleStep_ID, Trx trxName) : base (ctx, VAB_ProjectCycleStep_ID, trxName)
{
/** if (VAB_ProjectCycleStep_ID == 0)
{
SetVAB_ProjectCycleStep_ID (0);
SetVAB_ProjectCycle_ID (0);
SetName (null);
SetRelativeWeight (0.0);	// 1
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM VAB_ProjectCycleStep WHERE VAB_ProjectCycle_ID=@VAB_ProjectCycle_ID@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectCycleStep (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectCycleStep (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectCycleStep (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_ProjectCycleStep()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371639L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054850L;
/** VAF_TableView_ID=590 */
public static int Table_ID;
 // =590;

/** TableName=VAB_ProjectCycleStep */
public static String Table_Name="VAB_ProjectCycleStep";

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
StringBuilder sb = new StringBuilder ("X_VAB_ProjectCycleStep[").Append(Get_ID()).Append("]");
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
/** Set Project Cycle.
@param VAB_ProjectCycle_ID Identifier for this Project Reporting Cycle */
public void SetVAB_ProjectCycle_ID (int VAB_ProjectCycle_ID)
{
if (VAB_ProjectCycle_ID < 1) throw new ArgumentException ("VAB_ProjectCycle_ID is mandatory.");
Set_ValueNoCheck ("VAB_ProjectCycle_ID", VAB_ProjectCycle_ID);
}
/** Get Project Cycle.
@return Identifier for this Project Reporting Cycle */
public int GetVAB_ProjectCycle_ID() 
{
Object ii = Get_Value("VAB_ProjectCycle_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
/** Set Relative Weight.
@param RelativeWeight Relative weight of this step (0 = ignored) */
public void SetRelativeWeight (Decimal? RelativeWeight)
{
if (RelativeWeight == null) throw new ArgumentException ("RelativeWeight is mandatory.");
Set_Value ("RelativeWeight", (Decimal?)RelativeWeight);
}
/** Get Relative Weight.
@return Relative weight of this step (0 = ignored) */
public Decimal GetRelativeWeight() 
{
Object bd =Get_Value("RelativeWeight");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_Value ("SeqNo", SeqNo);
}
/** Get Sequence.
@return Method of ordering elements;
 lowest number comes first */
public int GetSeqNo() 
{
Object ii = Get_Value("SeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
