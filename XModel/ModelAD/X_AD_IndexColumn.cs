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
/** Generated Model for VAF_IndexColumn
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_IndexColumn : PO
{
public X_VAF_IndexColumn (Context ctx, int VAF_IndexColumn_ID, Trx trxName) : base (ctx, VAF_IndexColumn_ID, trxName)
{
/** if (VAF_IndexColumn_ID == 0)
{
SetVAF_Column_ID (0);
SetVAF_IndexColumn_ID (0);
SetVAF_TableViewIndex_ID (0);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM VAF_IndexColumn WHERE VAF_TableViewIndex_ID=@VAF_TableViewIndex_ID@
}
 */
}
public X_VAF_IndexColumn (Ctx ctx, int VAF_IndexColumn_ID, Trx trxName) : base (ctx, VAF_IndexColumn_ID, trxName)
{
/** if (VAF_IndexColumn_ID == 0)
{
SetVAF_Column_ID (0);
SetVAF_IndexColumn_ID (0);
SetVAF_TableViewIndex_ID (0);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM VAF_IndexColumn WHERE VAF_TableViewIndex_ID=@VAF_TableViewIndex_ID@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_IndexColumn (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_IndexColumn (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_IndexColumn (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_IndexColumn()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514361734L;
/** Last Updated Timestamp 7/29/2010 1:07:24 PM */
public static long updatedMS = 1280389044945L;
/** VAF_TableView_ID=910 */
public static int Table_ID;
 // =910;

/** TableName=VAF_IndexColumn */
public static String Table_Name="VAF_IndexColumn";

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
StringBuilder sb = new StringBuilder ("X_VAF_IndexColumn[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Column.
@param VAF_Column_ID Column in the table */
public void SetVAF_Column_ID (int VAF_Column_ID)
{
if (VAF_Column_ID < 1) throw new ArgumentException ("VAF_Column_ID is mandatory.");
Set_Value ("VAF_Column_ID", VAF_Column_ID);
}
/** Get Column.
@return Column in the table */
public int GetVAF_Column_ID() 
{
Object ii = Get_Value("VAF_Column_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Index Column.
@param VAF_IndexColumn_ID Index Column */
public void SetVAF_IndexColumn_ID (int VAF_IndexColumn_ID)
{
if (VAF_IndexColumn_ID < 1) throw new ArgumentException ("VAF_IndexColumn_ID is mandatory.");
Set_ValueNoCheck ("VAF_IndexColumn_ID", VAF_IndexColumn_ID);
}
/** Get Index Column.
@return Index Column */
public int GetVAF_IndexColumn_ID() 
{
Object ii = Get_Value("VAF_IndexColumn_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table Index.
@param VAF_TableViewIndex_ID Table Index */
public void SetVAF_TableViewIndex_ID (int VAF_TableViewIndex_ID)
{
if (VAF_TableViewIndex_ID < 1) throw new ArgumentException ("VAF_TableViewIndex_ID is mandatory.");
Set_ValueNoCheck ("VAF_TableViewIndex_ID", VAF_TableViewIndex_ID);
}
/** Get Table Index.
@return Table Index */
public int GetVAF_TableViewIndex_ID() 
{
Object ii = Get_Value("VAF_TableViewIndex_ID");
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
