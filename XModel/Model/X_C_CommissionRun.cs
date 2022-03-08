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
/** Generated Model for C_CommissionRun
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_CommissionRun : PO
{
public X_C_CommissionRun (Context ctx, int C_CommissionRun_ID, Trx trxName) : base (ctx, C_CommissionRun_ID, trxName)
{
/** if (C_CommissionRun_ID == 0)
{
SetC_CommissionRun_ID (0);
SetC_Commission_ID (0);
SetDocumentNo (null);
SetGrandTotal (0.0);
SetProcessed (false);	// N
SetStartDate (DateTime.Now);
}
 */
}
public X_C_CommissionRun (Ctx ctx, int C_CommissionRun_ID, Trx trxName) : base (ctx, C_CommissionRun_ID, trxName)
{
/** if (C_CommissionRun_ID == 0)
{
SetC_CommissionRun_ID (0);
SetC_Commission_ID (0);
SetDocumentNo (null);
SetGrandTotal (0.0);
SetProcessed (false);	// N
SetStartDate (DateTime.Now);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CommissionRun (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CommissionRun (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CommissionRun (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_CommissionRun()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371404L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054615L;
/** AD_Table_ID=436 */
public static int Table_ID;
 // =436;

/** TableName=C_CommissionRun */
public static String Table_Name="C_CommissionRun";

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
StringBuilder sb = new StringBuilder ("X_C_CommissionRun[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Commission Run.
@param C_CommissionRun_ID Commission Run or Process */
public void SetC_CommissionRun_ID (int C_CommissionRun_ID)
{
if (C_CommissionRun_ID < 1) throw new ArgumentException ("C_CommissionRun_ID is mandatory.");
Set_ValueNoCheck ("C_CommissionRun_ID", C_CommissionRun_ID);
}
/** Get Commission Run.
@return Commission Run or Process */
public int GetC_CommissionRun_ID() 
{
Object ii = Get_Value("C_CommissionRun_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Commission.
@param C_Commission_ID Commission */
public void SetC_Commission_ID (int C_Commission_ID)
{
if (C_Commission_ID < 1) throw new ArgumentException ("C_Commission_ID is mandatory.");
Set_ValueNoCheck ("C_Commission_ID", C_Commission_ID);
}
/** Get Commission.
@return Commission */
public int GetC_Commission_ID() 
{
Object ii = Get_Value("C_Commission_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Grand Total.
@param GrandTotal Total amount of document */
public void SetGrandTotal (Decimal? GrandTotal)
{
if (GrandTotal == null) throw new ArgumentException ("GrandTotal is mandatory.");
Set_ValueNoCheck ("GrandTotal", (Decimal?)GrandTotal);
}
/** Get Grand Total.
@return Total amount of document */
public Decimal GetGrandTotal() 
{
Object bd =Get_Value("GrandTotal");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Start Date.
@param StartDate First effective day (inclusive) */
public void SetStartDate (DateTime? StartDate)
{
if (StartDate == null) throw new ArgumentException ("StartDate is mandatory.");
Set_Value ("StartDate", (DateTime?)StartDate);
}
/** Get Start Date.
@return First effective day (inclusive) */
public DateTime? GetStartDate() 
{
return (DateTime?)Get_Value("StartDate");
}
}

}
