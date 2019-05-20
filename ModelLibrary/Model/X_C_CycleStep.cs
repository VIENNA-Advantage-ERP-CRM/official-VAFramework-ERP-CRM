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
/** Generated Model for C_CycleStep
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_CycleStep : PO
{
public X_C_CycleStep (Context ctx, int C_CycleStep_ID, Trx trxName) : base (ctx, C_CycleStep_ID, trxName)
{
/** if (C_CycleStep_ID == 0)
{
SetC_CycleStep_ID (0);
SetC_Cycle_ID (0);
SetName (null);
SetRelativeWeight (0.0);	// 1
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM C_CycleStep WHERE C_Cycle_ID=@C_Cycle_ID@
}
 */
}
public X_C_CycleStep (Ctx ctx, int C_CycleStep_ID, Trx trxName) : base (ctx, C_CycleStep_ID, trxName)
{
/** if (C_CycleStep_ID == 0)
{
SetC_CycleStep_ID (0);
SetC_Cycle_ID (0);
SetName (null);
SetRelativeWeight (0.0);	// 1
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM C_CycleStep WHERE C_Cycle_ID=@C_Cycle_ID@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CycleStep (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CycleStep (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CycleStep (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_CycleStep()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371639L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054850L;
/** AD_Table_ID=590 */
public static int Table_ID;
 // =590;

/** TableName=C_CycleStep */
public static String Table_Name="C_CycleStep";

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
StringBuilder sb = new StringBuilder ("X_C_CycleStep[").Append(Get_ID()).Append("]");
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
/** Set Project Cycle.
@param C_Cycle_ID Identifier for this Project Reporting Cycle */
public void SetC_Cycle_ID (int C_Cycle_ID)
{
if (C_Cycle_ID < 1) throw new ArgumentException ("C_Cycle_ID is mandatory.");
Set_ValueNoCheck ("C_Cycle_ID", C_Cycle_ID);
}
/** Get Project Cycle.
@return Identifier for this Project Reporting Cycle */
public int GetC_Cycle_ID() 
{
Object ii = Get_Value("C_Cycle_ID");
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
