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
/** Generated Model for AD_IndexColumn
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_IndexColumn : PO
{
public X_AD_IndexColumn (Context ctx, int AD_IndexColumn_ID, Trx trxName) : base (ctx, AD_IndexColumn_ID, trxName)
{
/** if (AD_IndexColumn_ID == 0)
{
SetAD_Column_ID (0);
SetAD_IndexColumn_ID (0);
SetAD_TableIndex_ID (0);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM AD_IndexColumn WHERE AD_TableIndex_ID=@AD_TableIndex_ID@
}
 */
}
public X_AD_IndexColumn (Ctx ctx, int AD_IndexColumn_ID, Trx trxName) : base (ctx, AD_IndexColumn_ID, trxName)
{
/** if (AD_IndexColumn_ID == 0)
{
SetAD_Column_ID (0);
SetAD_IndexColumn_ID (0);
SetAD_TableIndex_ID (0);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM AD_IndexColumn WHERE AD_TableIndex_ID=@AD_TableIndex_ID@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_IndexColumn (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_IndexColumn (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_IndexColumn (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_IndexColumn()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514361734L;
/** Last Updated Timestamp 7/29/2010 1:07:24 PM */
public static long updatedMS = 1280389044945L;
/** AD_Table_ID=910 */
public static int Table_ID;
 // =910;

/** TableName=AD_IndexColumn */
public static String Table_Name="AD_IndexColumn";

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
StringBuilder sb = new StringBuilder ("X_AD_IndexColumn[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Column.
@param AD_Column_ID Column in the table */
public void SetAD_Column_ID (int AD_Column_ID)
{
if (AD_Column_ID < 1) throw new ArgumentException ("AD_Column_ID is mandatory.");
Set_Value ("AD_Column_ID", AD_Column_ID);
}
/** Get Column.
@return Column in the table */
public int GetAD_Column_ID() 
{
Object ii = Get_Value("AD_Column_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Index Column.
@param AD_IndexColumn_ID Index Column */
public void SetAD_IndexColumn_ID (int AD_IndexColumn_ID)
{
if (AD_IndexColumn_ID < 1) throw new ArgumentException ("AD_IndexColumn_ID is mandatory.");
Set_ValueNoCheck ("AD_IndexColumn_ID", AD_IndexColumn_ID);
}
/** Get Index Column.
@return Index Column */
public int GetAD_IndexColumn_ID() 
{
Object ii = Get_Value("AD_IndexColumn_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table Index.
@param AD_TableIndex_ID Table Index */
public void SetAD_TableIndex_ID (int AD_TableIndex_ID)
{
if (AD_TableIndex_ID < 1) throw new ArgumentException ("AD_TableIndex_ID is mandatory.");
Set_ValueNoCheck ("AD_TableIndex_ID", AD_TableIndex_ID);
}
/** Get Table Index.
@return Table Index */
public int GetAD_TableIndex_ID() 
{
Object ii = Get_Value("AD_TableIndex_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Column SQL.
@param ColumnSQL Virtual Column (r/o) */
public void SetColumnSQL (String ColumnSQL)
{
if (ColumnSQL != null && ColumnSQL.Length > 60)
{
log.Warning("Length > 60 - truncated");
ColumnSQL = ColumnSQL.Substring(0,60);
}
Set_Value ("ColumnSQL", ColumnSQL);
}
/** Get Column SQL.
@return Virtual Column (r/o) */
public String GetColumnSQL() 
{
return (String)Get_Value("ColumnSQL");
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
