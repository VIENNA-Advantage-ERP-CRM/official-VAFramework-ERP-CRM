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
/** Generated Model for T_Report
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_T_Report : PO
{
public X_T_Report (Context ctx, int T_Report_ID, Trx trxName) : base (ctx, T_Report_ID, trxName)
{
/** if (T_Report_ID == 0)
{
SetAD_PInstance_ID (0);
SetFact_Acct_ID (0);
SetPA_ReportLine_ID (0);
SetRecord_ID (0);
}
 */
}
public X_T_Report (Ctx ctx, int T_Report_ID, Trx trxName) : base (ctx, T_Report_ID, trxName)
{
/** if (T_Report_ID == 0)
{
SetAD_PInstance_ID (0);
SetFact_Acct_ID (0);
SetPA_ReportLine_ID (0);
SetRecord_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_Report (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_Report (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_Report (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_T_Report()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384349L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067560L;
/** AD_Table_ID=544 */
public static int Table_ID;
 // =544;

/** TableName=T_Report */
public static String Table_Name="T_Report";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
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
StringBuilder sb = new StringBuilder ("X_T_Report[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Process Instance.
@param AD_PInstance_ID Instance of the process */
public void SetAD_PInstance_ID (int AD_PInstance_ID)
{
if (AD_PInstance_ID < 1) throw new ArgumentException ("AD_PInstance_ID is mandatory.");
Set_ValueNoCheck ("AD_PInstance_ID", AD_PInstance_ID);
}
/** Get Process Instance.
@return Instance of the process */
public int GetAD_PInstance_ID() 
{
Object ii = Get_Value("AD_PInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_PInstance_ID().ToString());
}
/** Set Col_0.
@param Col_0 Col_0 */
public void SetCol_0 (Decimal? Col_0)
{
Set_ValueNoCheck ("Col_0", (Decimal?)Col_0);
}
/** Get Col_0.
@return Col_0 */
public Decimal GetCol_0() 
{
Object bd =Get_Value("Col_0");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_1.
@param Col_1 Col_1 */
public void SetCol_1 (Decimal? Col_1)
{
Set_ValueNoCheck ("Col_1", (Decimal?)Col_1);
}
/** Get Col_1.
@return Col_1 */
public Decimal GetCol_1() 
{
Object bd =Get_Value("Col_1");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_10.
@param Col_10 Col_10 */
public void SetCol_10 (Decimal? Col_10)
{
Set_ValueNoCheck ("Col_10", (Decimal?)Col_10);
}
/** Get Col_10.
@return Col_10 */
public Decimal GetCol_10() 
{
Object bd =Get_Value("Col_10");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_11.
@param Col_11 Col_11 */
public void SetCol_11 (Decimal? Col_11)
{
Set_ValueNoCheck ("Col_11", (Decimal?)Col_11);
}
/** Get Col_11.
@return Col_11 */
public Decimal GetCol_11() 
{
Object bd =Get_Value("Col_11");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_12.
@param Col_12 Col_12 */
public void SetCol_12 (Decimal? Col_12)
{
Set_ValueNoCheck ("Col_12", (Decimal?)Col_12);
}
/** Get Col_12.
@return Col_12 */
public Decimal GetCol_12() 
{
Object bd =Get_Value("Col_12");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_13.
@param Col_13 Col_13 */
public void SetCol_13 (Decimal? Col_13)
{
Set_ValueNoCheck ("Col_13", (Decimal?)Col_13);
}
/** Get Col_13.
@return Col_13 */
public Decimal GetCol_13() 
{
Object bd =Get_Value("Col_13");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_14.
@param Col_14 Col_14 */
public void SetCol_14 (Decimal? Col_14)
{
Set_ValueNoCheck ("Col_14", (Decimal?)Col_14);
}
/** Get Col_14.
@return Col_14 */
public Decimal GetCol_14() 
{
Object bd =Get_Value("Col_14");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_15.
@param Col_15 Col_15 */
public void SetCol_15 (Decimal? Col_15)
{
Set_ValueNoCheck ("Col_15", (Decimal?)Col_15);
}
/** Get Col_15.
@return Col_15 */
public Decimal GetCol_15() 
{
Object bd =Get_Value("Col_15");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_16.
@param Col_16 Col_16 */
public void SetCol_16 (Decimal? Col_16)
{
Set_ValueNoCheck ("Col_16", (Decimal?)Col_16);
}
/** Get Col_16.
@return Col_16 */
public Decimal GetCol_16() 
{
Object bd =Get_Value("Col_16");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_17.
@param Col_17 Col_17 */
public void SetCol_17 (Decimal? Col_17)
{
Set_ValueNoCheck ("Col_17", (Decimal?)Col_17);
}
/** Get Col_17.
@return Col_17 */
public Decimal GetCol_17() 
{
Object bd =Get_Value("Col_17");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_18.
@param Col_18 Col_18 */
public void SetCol_18 (Decimal? Col_18)
{
Set_ValueNoCheck ("Col_18", (Decimal?)Col_18);
}
/** Get Col_18.
@return Col_18 */
public Decimal GetCol_18() 
{
Object bd =Get_Value("Col_18");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_19.
@param Col_19 Col_19 */
public void SetCol_19 (Decimal? Col_19)
{
Set_ValueNoCheck ("Col_19", (Decimal?)Col_19);
}
/** Get Col_19.
@return Col_19 */
public Decimal GetCol_19() 
{
Object bd =Get_Value("Col_19");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_2.
@param Col_2 Col_2 */
public void SetCol_2 (Decimal? Col_2)
{
Set_ValueNoCheck ("Col_2", (Decimal?)Col_2);
}
/** Get Col_2.
@return Col_2 */
public Decimal GetCol_2() 
{
Object bd =Get_Value("Col_2");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_20.
@param Col_20 Col_20 */
public void SetCol_20 (Decimal? Col_20)
{
Set_ValueNoCheck ("Col_20", (Decimal?)Col_20);
}
/** Get Col_20.
@return Col_20 */
public Decimal GetCol_20() 
{
Object bd =Get_Value("Col_20");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_3.
@param Col_3 Col_3 */
public void SetCol_3 (Decimal? Col_3)
{
Set_ValueNoCheck ("Col_3", (Decimal?)Col_3);
}
/** Get Col_3.
@return Col_3 */
public Decimal GetCol_3() 
{
Object bd =Get_Value("Col_3");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_4.
@param Col_4 Col_4 */
public void SetCol_4 (Decimal? Col_4)
{
Set_ValueNoCheck ("Col_4", (Decimal?)Col_4);
}
/** Get Col_4.
@return Col_4 */
public Decimal GetCol_4() 
{
Object bd =Get_Value("Col_4");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_5.
@param Col_5 Col_5 */
public void SetCol_5 (Decimal? Col_5)
{
Set_ValueNoCheck ("Col_5", (Decimal?)Col_5);
}
/** Get Col_5.
@return Col_5 */
public Decimal GetCol_5() 
{
Object bd =Get_Value("Col_5");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_6.
@param Col_6 Col_6 */
public void SetCol_6 (Decimal? Col_6)
{
Set_ValueNoCheck ("Col_6", (Decimal?)Col_6);
}
/** Get Col_6.
@return Col_6 */
public Decimal GetCol_6() 
{
Object bd =Get_Value("Col_6");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_7.
@param Col_7 Col_7 */
public void SetCol_7 (Decimal? Col_7)
{
Set_ValueNoCheck ("Col_7", (Decimal?)Col_7);
}
/** Get Col_7.
@return Col_7 */
public Decimal GetCol_7() 
{
Object bd =Get_Value("Col_7");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_8.
@param Col_8 Col_8 */
public void SetCol_8 (Decimal? Col_8)
{
Set_ValueNoCheck ("Col_8", (Decimal?)Col_8);
}
/** Get Col_8.
@return Col_8 */
public Decimal GetCol_8() 
{
Object bd =Get_Value("Col_8");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Col_9.
@param Col_9 Col_9 */
public void SetCol_9 (Decimal? Col_9)
{
Set_ValueNoCheck ("Col_9", (Decimal?)Col_9);
}
/** Get Col_9.
@return Col_9 */
public Decimal GetCol_9() 
{
Object bd =Get_Value("Col_9");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
Set_ValueNoCheck ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Accounting Fact.
@param Fact_Acct_ID Accounting Fact */
public void SetFact_Acct_ID (int Fact_Acct_ID)
{
if (Fact_Acct_ID < 1) throw new ArgumentException ("Fact_Acct_ID is mandatory.");
Set_ValueNoCheck ("Fact_Acct_ID", Fact_Acct_ID);
}
/** Get Accounting Fact.
@return Accounting Fact */
public int GetFact_Acct_ID() 
{
Object ii = Get_Value("Fact_Acct_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Level no.
@param LevelNo Level no */
public void SetLevelNo (int LevelNo)
{
Set_ValueNoCheck ("LevelNo", LevelNo);
}
/** Get Level no.
@return Level no */
public int GetLevelNo() 
{
Object ii = Get_Value("LevelNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_ValueNoCheck ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set Report Line.
@param PA_ReportLine_ID Report Line */
public void SetPA_ReportLine_ID (int PA_ReportLine_ID)
{
if (PA_ReportLine_ID < 1) throw new ArgumentException ("PA_ReportLine_ID is mandatory.");
Set_ValueNoCheck ("PA_ReportLine_ID", PA_ReportLine_ID);
}
/** Get Report Line.
@return Report Line */
public int GetPA_ReportLine_ID() 
{
Object ii = Get_Value("PA_ReportLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID < 0) throw new ArgumentException ("Record_ID is mandatory.");
Set_ValueNoCheck ("Record_ID", Record_ID);
}
/** Get Record ID.
@return Direct internal record ID */
public int GetRecord_ID() 
{
Object ii = Get_Value("Record_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_ValueNoCheck ("SeqNo", SeqNo);
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
