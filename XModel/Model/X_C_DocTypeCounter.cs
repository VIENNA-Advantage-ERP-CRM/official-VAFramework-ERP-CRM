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
/** Generated Model for VAB_InterCompanyDoc
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_InterCompanyDoc : PO
{
public X_VAB_InterCompanyDoc (Context ctx, int VAB_InterCompanyDoc_ID, Trx trxName) : base (ctx, VAB_InterCompanyDoc_ID, trxName)
{
/** if (VAB_InterCompanyDoc_ID == 0)
{
SetVAB_InterCompanyDoc_ID (0);
SetVAB_DocTypes_ID (0);
SetIsCreateCounter (true);	// Y
SetIsValid (false);
SetName (null);
}
 */
}
public X_VAB_InterCompanyDoc (Ctx ctx, int VAB_InterCompanyDoc_ID, Trx trxName) : base (ctx, VAB_InterCompanyDoc_ID, trxName)
{
/** if (VAB_InterCompanyDoc_ID == 0)
{
SetVAB_InterCompanyDoc_ID (0);
SetVAB_DocTypes_ID (0);
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
public X_VAB_InterCompanyDoc (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_InterCompanyDoc (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_InterCompanyDoc (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_InterCompanyDoc()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371921L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055132L;
/** VAF_TableView_ID=718 */
public static int Table_ID;
 // =718;

/** TableName=VAB_InterCompanyDoc */
public static String Table_Name="VAB_InterCompanyDoc";

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
StringBuilder sb = new StringBuilder ("X_VAB_InterCompanyDoc[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Counter Document.
@param VAB_InterCompanyDoc_ID Counter Document Relationship */
public void SetVAB_InterCompanyDoc_ID (int VAB_InterCompanyDoc_ID)
{
if (VAB_InterCompanyDoc_ID < 1) throw new ArgumentException ("VAB_InterCompanyDoc_ID is mandatory.");
Set_ValueNoCheck ("VAB_InterCompanyDoc_ID", VAB_InterCompanyDoc_ID);
}
/** Get Counter Document.
@return Counter Document Relationship */
public int GetVAB_InterCompanyDoc_ID() 
{
Object ii = Get_Value("VAB_InterCompanyDoc_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Document Type.
@param VAB_DocTypes_ID Document type or rules */
public void SetVAB_DocTypes_ID (int VAB_DocTypes_ID)
{
if (VAB_DocTypes_ID < 0) throw new ArgumentException ("VAB_DocTypes_ID is mandatory.");
Set_Value ("VAB_DocTypes_ID", VAB_DocTypes_ID);
}
/** Get Document Type.
@return Document type or rules */
public int GetVAB_DocTypes_ID() 
{
Object ii = Get_Value("VAB_DocTypes_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Counter_VAB_DocTypes_ID VAF_Control_Ref_ID=170 */
public static int COUNTER_VAB_DocTypes_ID_VAF_Control_Ref_ID=170;
/** Set Counter Document Type.
@param Counter_VAB_DocTypes_ID Generated Counter Document Type (To) */
public void SetCounter_VAB_DocTypes_ID (int Counter_VAB_DocTypes_ID)
{
if (Counter_VAB_DocTypes_ID <= 0) Set_Value ("Counter_VAB_DocTypes_ID", null);
else
Set_Value ("Counter_VAB_DocTypes_ID", Counter_VAB_DocTypes_ID);
}
/** Get Counter Document Type.
@return Generated Counter Document Type (To) */
public int GetCounter_VAB_DocTypes_ID() 
{
Object ii = Get_Value("Counter_VAB_DocTypes_ID");
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

/** DocAction VAF_Control_Ref_ID=135 */
public static int DOCACTION_VAF_Control_Ref_ID=135;
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
