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
/** Generated Model for VAF_Record_Seq_Audit
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_Record_Seq_Audit : PO
{
public X_VAF_Record_Seq_Audit (Context ctx, int VAF_Record_Seq_Audit_ID, Trx trxName) : base (ctx, VAF_Record_Seq_Audit_ID, trxName)
{
/** if (VAF_Record_Seq_Audit_ID == 0)
{
SetVAF_Record_Seq_ID (0);
SetVAF_TableView_ID (0);
SetDocumentNo (null);
SetRecord_ID (0);
}
 */
}
public X_VAF_Record_Seq_Audit (Ctx ctx, int VAF_Record_Seq_Audit_ID, Trx trxName) : base (ctx, VAF_Record_Seq_Audit_ID, trxName)
{
/** if (VAF_Record_Seq_Audit_ID == 0)
{
SetVAF_Record_Seq_ID (0);
SetVAF_TableView_ID (0);
SetDocumentNo (null);
SetRecord_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Record_Seq_Audit (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Record_Seq_Audit (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Record_Seq_Audit (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_Record_Seq_Audit()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514364100L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047311L;
/** VAF_TableView_ID=121 */
public static int Table_ID;
 // =121;

/** TableName=VAF_Record_Seq_Audit */
public static String Table_Name="VAF_Record_Seq_Audit";

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
StringBuilder sb = new StringBuilder ("X_VAF_Record_Seq_Audit[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Sequence.
@param VAF_Record_Seq_ID Document Sequence */
public void SetVAF_Record_Seq_ID (int VAF_Record_Seq_ID)
{
if (VAF_Record_Seq_ID < 1) throw new ArgumentException ("VAF_Record_Seq_ID is mandatory.");
Set_ValueNoCheck ("VAF_Record_Seq_ID", VAF_Record_Seq_ID);
}
/** Get Sequence.
@return Document Sequence */
public int GetVAF_Record_Seq_ID() 
{
Object ii = Get_Value("VAF_Record_Seq_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_Record_Seq_ID().ToString());
}
/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID)
{
if (VAF_TableView_ID < 1) throw new ArgumentException ("VAF_TableView_ID is mandatory.");
Set_ValueNoCheck ("VAF_TableView_ID", VAF_TableView_ID);
}
/** Get Table.
@return Database Table information */
public int GetVAF_TableView_ID() 
{
Object ii = Get_Value("VAF_TableView_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Document No.
@param DocumentNo Document sequence number of the document */
public void SetDocumentNo (String DocumentNo)
{
if (DocumentNo == null) throw new ArgumentException ("DocumentNo is mandatory.");
if (DocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
DocumentNo = DocumentNo.Substring(0,30);
}
Set_ValueNoCheck ("DocumentNo", DocumentNo);
}
/** Get Document No.
@return Document sequence number of the document */
public String GetDocumentNo() 
{
return (String)Get_Value("DocumentNo");
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
}

}
