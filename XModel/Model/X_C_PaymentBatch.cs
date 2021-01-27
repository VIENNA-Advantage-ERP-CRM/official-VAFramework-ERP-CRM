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
/** Generated Model for VAB_PaymentBatch
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_PaymentBatch : PO
{
public X_VAB_PaymentBatch (Context ctx, int VAB_PaymentBatch_ID, Trx trxName) : base (ctx, VAB_PaymentBatch_ID, trxName)
{
/** if (VAB_PaymentBatch_ID == 0)
{
SetVAB_PaymentBatch_ID (0);
SetName (null);
SetProcessed (false);	// N
SetProcessing (false);	// N
}
 */
}
public X_VAB_PaymentBatch (Ctx ctx, int VAB_PaymentBatch_ID, Trx trxName) : base (ctx, VAB_PaymentBatch_ID, trxName)
{
/** if (VAB_PaymentBatch_ID == 0)
{
SetVAB_PaymentBatch_ID (0);
SetName (null);
SetProcessed (false);	// N
SetProcessing (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_PaymentBatch (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_PaymentBatch (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_PaymentBatch (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_PaymentBatch()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514373958L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057169L;
/** VAF_TableView_ID=411 */
public static int Table_ID;
 // =411;

/** TableName=VAB_PaymentBatch */
public static String Table_Name="VAB_PaymentBatch";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(1);
/** AccessLevel
@return 1 - Org 
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
StringBuilder sb = new StringBuilder ("X_VAB_PaymentBatch[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Payment Batch.
@param VAB_PaymentBatch_ID Payment batch for EFT */
public void SetVAB_PaymentBatch_ID (int VAB_PaymentBatch_ID)
{
if (VAB_PaymentBatch_ID < 1) throw new ArgumentException ("VAB_PaymentBatch_ID is mandatory.");
Set_ValueNoCheck ("VAB_PaymentBatch_ID", VAB_PaymentBatch_ID);
}
/** Get Payment Batch.
@return Payment batch for EFT */
public int GetVAB_PaymentBatch_ID() 
{
Object ii = Get_Value("VAB_PaymentBatch_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment Processor.
@param VAB_PaymentHandler_ID Payment processor for electronic payments */
public void SetVAB_PaymentHandler_ID (int VAB_PaymentHandler_ID)
{
if (VAB_PaymentHandler_ID <= 0) Set_Value ("VAB_PaymentHandler_ID", null);
else
Set_Value ("VAB_PaymentHandler_ID", VAB_PaymentHandler_ID);
}
/** Get Payment Processor.
@return Payment processor for electronic payments */
public int GetVAB_PaymentHandler_ID() 
{
Object ii = Get_Value("VAB_PaymentHandler_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Document No.
@param DocumentNo Document sequence number of the document */
public void SetDocumentNo (String DocumentNo)
{
if (DocumentNo != null && DocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
DocumentNo = DocumentNo.Substring(0,30);
}
Set_Value ("DocumentNo", DocumentNo);
}
/** Get Document No.
@return Document sequence number of the document */
public String GetDocumentNo() 
{
return (String)Get_Value("DocumentNo");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetDocumentNo());
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
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
}
/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed() 
{
Object oo = Get_Value("Processed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Processing date.
@param ProcessingDate Processing date */
public void SetProcessingDate (DateTime? ProcessingDate)
{
Set_Value ("ProcessingDate", (DateTime?)ProcessingDate);
}
/** Get Processing date.
@return Processing date */
public DateTime? GetProcessingDate() 
{
return (DateTime?)Get_Value("ProcessingDate");
}
}

}
