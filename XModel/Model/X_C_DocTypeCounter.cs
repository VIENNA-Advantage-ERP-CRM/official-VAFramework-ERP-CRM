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
/** Generated Model for C_DocTypeCounter
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_DocTypeCounter : PO
{
public X_C_DocTypeCounter (Context ctx, int C_DocTypeCounter_ID, Trx trxName) : base (ctx, C_DocTypeCounter_ID, trxName)
{
/** if (C_DocTypeCounter_ID == 0)
{
SetC_DocTypeCounter_ID (0);
SetC_DocType_ID (0);
SetIsCreateCounter (true);	// Y
SetIsValid (false);
SetName (null);
}
 */
}
public X_C_DocTypeCounter (Ctx ctx, int C_DocTypeCounter_ID, Trx trxName) : base (ctx, C_DocTypeCounter_ID, trxName)
{
/** if (C_DocTypeCounter_ID == 0)
{
SetC_DocTypeCounter_ID (0);
SetC_DocType_ID (0);
SetIsCreateCounter (true);	// Y
SetIsValid (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_DocTypeCounter (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_DocTypeCounter (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_DocTypeCounter (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_DocTypeCounter()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371921L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055132L;
/** AD_Table_ID=718 */
public static int Table_ID;
 // =718;

/** TableName=C_DocTypeCounter */
public static String Table_Name="C_DocTypeCounter";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_C_DocTypeCounter[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Counter Document.
@param C_DocTypeCounter_ID Counter Document Relationship */
public void SetC_DocTypeCounter_ID (int C_DocTypeCounter_ID)
{
if (C_DocTypeCounter_ID < 1) throw new ArgumentException ("C_DocTypeCounter_ID is mandatory.");
Set_ValueNoCheck ("C_DocTypeCounter_ID", C_DocTypeCounter_ID);
}
/** Get Counter Document.
@return Counter Document Relationship */
public int GetC_DocTypeCounter_ID() 
{
Object ii = Get_Value("C_DocTypeCounter_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Document Type.
@param C_DocType_ID Document type or rules */
public void SetC_DocType_ID (int C_DocType_ID)
{
if (C_DocType_ID < 0) throw new ArgumentException ("C_DocType_ID is mandatory.");
Set_Value ("C_DocType_ID", C_DocType_ID);
}
/** Get Document Type.
@return Document type or rules */
public int GetC_DocType_ID() 
{
Object ii = Get_Value("C_DocType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Counter_C_DocType_ID AD_Reference_ID=170 */
public static int COUNTER_C_DOCTYPE_ID_AD_Reference_ID=170;
/** Set Counter Document Type.
@param Counter_C_DocType_ID Generated Counter Document Type (To) */
public void SetCounter_C_DocType_ID (int Counter_C_DocType_ID)
{
if (Counter_C_DocType_ID <= 0) Set_Value ("Counter_C_DocType_ID", null);
else
Set_Value ("Counter_C_DocType_ID", Counter_C_DocType_ID);
}
/** Get Counter Document Type.
@return Generated Counter Document Type (To) */
public int GetCounter_C_DocType_ID() 
{
Object ii = Get_Value("Counter_C_DocType_ID");
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

/** DocAction AD_Reference_ID=135 */
public static int DOCACTION_AD_Reference_ID=135;
/** <None> = -- */
public static String DOCACTION_None = "--";
/** Approve = AP */
public static String DOCACTION_Approve = "AP";
/** Close = CL */
public static String DOCACTION_Close = "CL";
/** Complete = CO */
public static String DOCACTION_Complete = "CO";
/** Invalidate = IN */
public static String DOCACTION_Invalidate = "IN";
/** Post = PO */
public static String DOCACTION_Post = "PO";
/** Prepare = PR */
public static String DOCACTION_Prepare = "PR";
/** Reverse - Accrual = RA */
public static String DOCACTION_Reverse_Accrual = "RA";
/** Reverse - Correct = RC */
public static String DOCACTION_Reverse_Correct = "RC";
/** Re-activate = RE */
public static String DOCACTION_Re_Activate = "RE";
/** Reject = RJ */
public static String DOCACTION_Reject = "RJ";
/** Void = VO */
public static String DOCACTION_Void = "VO";
/** Wait Complete = WC */
public static String DOCACTION_WaitComplete = "WC";
/** Unlock = XL */
public static String DOCACTION_Unlock = "XL";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDocActionValid (String test)
{
return test == null || test.Equals("--") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("IN") || test.Equals("PO") || test.Equals("PR") || test.Equals("RA") || test.Equals("RC") || test.Equals("RE") || test.Equals("RJ") || test.Equals("VO") || test.Equals("WC") || test.Equals("XL");
}
/** Set Document Action.
@param DocAction The targeted status of the document */
public void SetDocAction (String DocAction)
{
if (!IsDocActionValid(DocAction))
throw new ArgumentException ("DocAction Invalid value - " + DocAction + " - Reference_ID=135 - -- - AP - CL - CO - IN - PO - PR - RA - RC - RE - RJ - VO - WC - XL");
if (DocAction != null && DocAction.Length > 2)
{
log.Warning("Length > 2 - truncated");
DocAction = DocAction.Substring(0,2);
}
Set_Value ("DocAction", DocAction);
}
/** Get Document Action.
@return The targeted status of the document */
public String GetDocAction() 
{
return (String)Get_Value("DocAction");
}
/** Set Create Counter Document.
@param IsCreateCounter Create Counter Document */
public void SetIsCreateCounter (Boolean IsCreateCounter)
{
Set_Value ("IsCreateCounter", IsCreateCounter);
}
/** Get Create Counter Document.
@return Create Counter Document */
public Boolean IsCreateCounter() 
{
Object oo = Get_Value("IsCreateCounter");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Valid.
@param IsValid Element is valid */
public void SetIsValid (Boolean IsValid)
{
Set_Value ("IsValid", IsValid);
}
/** Get Valid.
@return Element is valid */
public Boolean IsValid() 
{
Object oo = Get_Value("IsValid");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
}

}
