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
/** Generated Model for PA_Achievement
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_PA_Achievement : PO
{
public X_PA_Achievement (Context ctx, int PA_Achievement_ID, Trx trxName) : base (ctx, PA_Achievement_ID, trxName)
{
/** if (PA_Achievement_ID == 0)
{
SetIsAchieved (false);
SetManualActual (0.0);
SetName (null);
SetPA_Achievement_ID (0);
SetPA_Measure_ID (0);
SetSeqNo (0);
}
 */
}
public X_PA_Achievement (Ctx ctx, int PA_Achievement_ID, Trx trxName) : base (ctx, PA_Achievement_ID, trxName)
{
/** if (PA_Achievement_ID == 0)
{
SetIsAchieved (false);
SetManualActual (0.0);
SetName (null);
SetPA_Achievement_ID (0);
SetPA_Measure_ID (0);
SetSeqNo (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_Achievement (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_Achievement (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_Achievement (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_PA_Achievement()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381559L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064770L;
/** AD_Table_ID=438 */
public static int Table_ID;
 // =438;

/** TableName=PA_Achievement */
public static String Table_Name="PA_Achievement";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(6);
/** AccessLevel
@return 6 - System - Client 
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
StringBuilder sb = new StringBuilder ("X_PA_Achievement[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Document Date.
@param DateDoc Date of the Document */
public void SetDateDoc (DateTime? DateDoc)
{
Set_Value ("DateDoc", (DateTime?)DateDoc);
}
/** Get Document Date.
@return Date of the Document */
public DateTime? GetDateDoc() 
{
return (DateTime?)Get_Value("DateDoc");
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Achieved.
@param IsAchieved The goal is achieved */
public void SetIsAchieved (Boolean IsAchieved)
{
Set_Value ("IsAchieved", IsAchieved);
}
/** Get Achieved.
@return The goal is achieved */
public Boolean IsAchieved() 
{
Object oo = Get_Value("IsAchieved");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Manual Actual.
@param ManualActual Manually entered actual value */
public void SetManualActual (Decimal? ManualActual)
{
if (ManualActual == null) throw new ArgumentException ("ManualActual is mandatory.");
Set_Value ("ManualActual", (Decimal?)ManualActual);
}
/** Get Manual Actual.
@return Manually entered actual value */
public Decimal GetManualActual() 
{
Object bd =Get_Value("ManualActual");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Note.
@param Note Optional additional user defined information */
public void SetNote (String Note)
{
if (Note != null && Note.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Note = Note.Substring(0,2000);
}
Set_Value ("Note", Note);
}
/** Get Note.
@return Optional additional user defined information */
public String GetNote() 
{
return (String)Get_Value("Note");
}
/** Set Achievement.
@param PA_Achievement_ID Performance Achievement */
public void SetPA_Achievement_ID (int PA_Achievement_ID)
{
if (PA_Achievement_ID < 1) throw new ArgumentException ("PA_Achievement_ID is mandatory.");
Set_ValueNoCheck ("PA_Achievement_ID", PA_Achievement_ID);
}
/** Get Achievement.
@return Performance Achievement */
public int GetPA_Achievement_ID() 
{
Object ii = Get_Value("PA_Achievement_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Measure.
@param PA_Measure_ID Concrete Performance Measurement */
public void SetPA_Measure_ID (int PA_Measure_ID)
{
if (PA_Measure_ID < 1) throw new ArgumentException ("PA_Measure_ID is mandatory.");
Set_ValueNoCheck ("PA_Measure_ID", PA_Measure_ID);
}
/** Get Measure.
@return Concrete Performance Measurement */
public int GetPA_Measure_ID() 
{
Object ii = Get_Value("PA_Measure_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
