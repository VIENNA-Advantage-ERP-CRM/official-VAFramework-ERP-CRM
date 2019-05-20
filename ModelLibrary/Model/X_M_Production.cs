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
/** Generated Model for M_Production
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_Production : PO
{
public X_M_Production (Context ctx, int M_Production_ID, Trx trxName) : base (ctx, M_Production_ID, trxName)
{
/** if (M_Production_ID == 0)
{
SetIsCreated (false);
SetM_Production_ID (0);
SetMovementDate (DateTime.Now);	// @#Date@
SetName (null);
SetPosted (false);
SetProcessed (false);	// N
}
 */
}
public X_M_Production (Ctx ctx, int M_Production_ID, Trx trxName) : base (ctx, M_Production_ID, trxName)
{
/** if (M_Production_ID == 0)
{
SetIsCreated (false);
SetM_Production_ID (0);
SetMovementDate (DateTime.Now);	// @#Date@
SetName (null);
SetPosted (false);
SetProcessed (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Production (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Production (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Production (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_Production()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514380948L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064159L;
/** AD_Table_ID=325 */
public static int Table_ID;
 // =325;

/** TableName=M_Production */
public static String Table_Name="M_Production";

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
StringBuilder sb = new StringBuilder ("X_M_Production[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_OrgTrx_ID AD_Reference_ID=130 */
public static int AD_ORGTRX_ID_AD_Reference_ID=130;
/** Set Trx Organization.
@param AD_OrgTrx_ID Performing or initiating organization */
public void SetAD_OrgTrx_ID (int AD_OrgTrx_ID)
{
if (AD_OrgTrx_ID <= 0) Set_Value ("AD_OrgTrx_ID", null);
else
Set_Value ("AD_OrgTrx_ID", AD_OrgTrx_ID);
}
/** Get Trx Organization.
@return Performing or initiating organization */
public int GetAD_OrgTrx_ID() 
{
Object ii = Get_Value("AD_OrgTrx_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Activity.
@param C_Activity_ID Business Activity */
public void SetC_Activity_ID (int C_Activity_ID)
{
if (C_Activity_ID <= 0) Set_Value ("C_Activity_ID", null);
else
Set_Value ("C_Activity_ID", C_Activity_ID);
}
/** Get Activity.
@return Business Activity */
public int GetC_Activity_ID() 
{
Object ii = Get_Value("C_Activity_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Campaign.
@param C_Campaign_ID Marketing Campaign */
public void SetC_Campaign_ID (int C_Campaign_ID)
{
if (C_Campaign_ID <= 0) Set_Value ("C_Campaign_ID", null);
else
Set_Value ("C_Campaign_ID", C_Campaign_ID);
}
/** Get Campaign.
@return Marketing Campaign */
public int GetC_Campaign_ID() 
{
Object ii = Get_Value("C_Campaign_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project.
@param C_Project_ID Financial Project */
public void SetC_Project_ID (int C_Project_ID)
{
if (C_Project_ID <= 0) Set_Value ("C_Project_ID", null);
else
Set_Value ("C_Project_ID", C_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetC_Project_ID() 
{
Object ii = Get_Value("C_Project_ID");
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
/** Set Records created.
@param IsCreated Records created */
public void SetIsCreated (Boolean IsCreated)
{
Set_ValueNoCheck ("IsCreated", IsCreated);
}
/** Get Records created.
@return Records created */
public Boolean IsCreated() 
{
Object oo = Get_Value("IsCreated");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Production.
@param M_Production_ID Plan for producing a product */
public void SetM_Production_ID (int M_Production_ID)
{
if (M_Production_ID < 1) throw new ArgumentException ("M_Production_ID is mandatory.");
Set_ValueNoCheck ("M_Production_ID", M_Production_ID);
}
/** Get Production.
@return Plan for producing a product */
public int GetM_Production_ID() 
{
Object ii = Get_Value("M_Production_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Movement Date.
@param MovementDate Date a product was moved in or out of inventory */
public void SetMovementDate (DateTime? MovementDate)
{
if (MovementDate == null) throw new ArgumentException ("MovementDate is mandatory.");
Set_Value ("MovementDate", (DateTime?)MovementDate);
}
/** Get Movement Date.
@return Date a product was moved in or out of inventory */
public DateTime? GetMovementDate() 
{
return (DateTime?)Get_Value("MovementDate");
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
/** Set Posted.
@param Posted Posting status */
public void SetPosted (Boolean Posted)
{
Set_Value ("Posted", Posted);
}
/** Get Posted.
@return Posting status */
public Boolean IsPosted() 
{
Object oo = Get_Value("Posted");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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

/** User1_ID AD_Reference_ID=134 */
public static int USER1_ID_AD_Reference_ID=134;
/** Set User List 1.
@param User1_ID User defined list element #1 */
public void SetUser1_ID (int User1_ID)
{
if (User1_ID <= 0) Set_Value ("User1_ID", null);
else
Set_Value ("User1_ID", User1_ID);
}
/** Get User List 1.
@return User defined list element #1 */
public int GetUser1_ID() 
{
Object ii = Get_Value("User1_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** User2_ID AD_Reference_ID=137 */
public static int USER2_ID_AD_Reference_ID=137;
/** Set User List 2.
@param User2_ID User defined list element #2 */
public void SetUser2_ID (int User2_ID)
{
if (User2_ID <= 0) Set_Value ("User2_ID", null);
else
Set_Value ("User2_ID", User2_ID);
}
/** Get User List 2.
@return User defined list element #2 */
public int GetUser2_ID() 
{
Object ii = Get_Value("User2_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Set Reversed.
@param IsReversed Reversed */
public void SetIsReversed(Boolean IsReversed) { Set_Value("IsReversed", IsReversed); }/** Get Reversed.
@return Reversed */
public Boolean IsReversed() { Object oo = Get_Value("IsReversed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

/** Set Reversed Document.
@param ReversedDocument Reversed Document */
public void SetReversedDocument(String ReversedDocument) { if (ReversedDocument != null && ReversedDocument.Length > 1) { log.Warning("Length > 1 - truncated"); ReversedDocument = ReversedDocument.Substring(0, 1); } Set_Value("ReversedDocument", ReversedDocument); }/** Get Reversed Document.
@return Reversed Document */
public String GetReversedDocument() { return (String)Get_Value("ReversedDocument"); }

/** M_Ref_Production AD_Reference_ID=1000191 */
public static int M_REF_PRODUCTION_AD_Reference_ID = 1000191;/** Set Reverse Reference.
@param M_Ref_Production Reverse Reference */
public void SetM_Ref_Production(int M_Ref_Production) { Set_Value("M_Ref_Production", M_Ref_Production); }/** Get Reverse Reference.
@return Reverse Reference */
public int GetM_Ref_Production() { Object ii = Get_Value("M_Ref_Production"); if (ii == null) return 0; return Convert.ToInt32(ii); }
}

}
