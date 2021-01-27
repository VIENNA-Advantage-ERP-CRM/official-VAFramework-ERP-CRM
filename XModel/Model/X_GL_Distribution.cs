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
/** Generated Model for VAGL_Distribution
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAGL_Distribution : PO
{
public X_VAGL_Distribution (Context ctx, int VAGL_Distribution_ID, Trx trxName) : base (ctx, VAGL_Distribution_ID, trxName)
{
/** if (VAGL_Distribution_ID == 0)
{
SetAnyAcct (true);	// Y
SetAnyActivity (true);	// Y
SetAnyBPartner (true);	// Y
SetAnyCampaign (true);	// Y
SetAnyLocFrom (true);	// Y
SetAnyLocTo (true);	// Y
SetAnyOrg (true);	// Y
SetAnyOrgTrx (true);	// Y
SetAnyProduct (true);	// Y
SetAnyProject (true);	// Y
SetAnySalesRegion (true);	// Y
SetAnyUser1 (true);	// Y
SetAnyUser2 (true);	// Y
SetVAB_AccountBook_ID (0);
SetVAGL_Distribution_ID (0);
SetIsValid (false);	// N
SetName (null);
SetPercentTotal (0.0);
}
 */
}
public X_VAGL_Distribution (Ctx ctx, int VAGL_Distribution_ID, Trx trxName) : base (ctx, VAGL_Distribution_ID, trxName)
{
/** if (VAGL_Distribution_ID == 0)
{
SetAnyAcct (true);	// Y
SetAnyActivity (true);	// Y
SetAnyBPartner (true);	// Y
SetAnyCampaign (true);	// Y
SetAnyLocFrom (true);	// Y
SetAnyLocTo (true);	// Y
SetAnyOrg (true);	// Y
SetAnyOrgTrx (true);	// Y
SetAnyProduct (true);	// Y
SetAnyProject (true);	// Y
SetAnySalesRegion (true);	// Y
SetAnyUser1 (true);	// Y
SetAnyUser2 (true);	// Y
SetVAB_AccountBook_ID (0);
SetVAGL_Distribution_ID (0);
SetIsValid (false);	// N
SetName (null);
SetPercentTotal (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAGL_Distribution (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAGL_Distribution (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAGL_Distribution (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAGL_Distribution()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376403L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059614L;
/** VAF_TableView_ID=708 */
public static int Table_ID;
 // =708;

/** TableName=VAGL_Distribution */
public static String Table_Name="VAGL_Distribution";

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
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_VAGL_Distribution[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** VAF_OrgTrx_ID VAF_Control_Ref_ID=130 */
public static int VAF_ORGTRX_ID_VAF_Control_Ref_ID=130;
/** Set Trx Organization.
@param VAF_OrgTrx_ID Performing or initiating organization */
public void SetVAF_OrgTrx_ID (int VAF_OrgTrx_ID)
{
if (VAF_OrgTrx_ID <= 0) Set_Value ("VAF_OrgTrx_ID", null);
else
Set_Value ("VAF_OrgTrx_ID", VAF_OrgTrx_ID);
}
/** Get Trx Organization.
@return Performing or initiating organization */
public int GetVAF_OrgTrx_ID() 
{
Object ii = Get_Value("VAF_OrgTrx_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Account_ID VAF_Control_Ref_ID=132 */
public static int ACCOUNT_ID_VAF_Control_Ref_ID=132;
/** Set Account.
@param Account_ID Account used */
public void SetAccount_ID (int Account_ID)
{
if (Account_ID <= 0) Set_Value ("Account_ID", null);
else
Set_Value ("Account_ID", Account_ID);
}
/** Get Account.
@return Account used */
public int GetAccount_ID() 
{
Object ii = Get_Value("Account_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Any Account.
@param AnyAcct Match any value of the Account segment */
public void SetAnyAcct (Boolean AnyAcct)
{
Set_Value ("AnyAcct", AnyAcct);
}
/** Get Any Account.
@return Match any value of the Account segment */
public Boolean IsAnyAcct() 
{
Object oo = Get_Value("AnyAcct");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Any Activity.
@param AnyActivity Match any value of the Activity segment */
public void SetAnyActivity (Boolean AnyActivity)
{
Set_Value ("AnyActivity", AnyActivity);
}
/** Get Any Activity.
@return Match any value of the Activity segment */
public Boolean IsAnyActivity() 
{
Object oo = Get_Value("AnyActivity");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Any Bus.Partner.
@param AnyBPartner Match any value of the Business Partner segment */
public void SetAnyBPartner (Boolean AnyBPartner)
{
Set_Value ("AnyBPartner", AnyBPartner);
}
/** Get Any Bus.Partner.
@return Match any value of the Business Partner segment */
public Boolean IsAnyBPartner() 
{
Object oo = Get_Value("AnyBPartner");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Any Campaign.
@param AnyCampaign Match any value of the Campaign segment */
public void SetAnyCampaign (Boolean AnyCampaign)
{
Set_Value ("AnyCampaign", AnyCampaign);
}
/** Get Any Campaign.
@return Match any value of the Campaign segment */
public Boolean IsAnyCampaign() 
{
Object oo = Get_Value("AnyCampaign");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Any Location From.
@param AnyLocFrom Match any value of the Location From segment */
public void SetAnyLocFrom (Boolean AnyLocFrom)
{
Set_Value ("AnyLocFrom", AnyLocFrom);
}
/** Get Any Location From.
@return Match any value of the Location From segment */
public Boolean IsAnyLocFrom() 
{
Object oo = Get_Value("AnyLocFrom");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Any Location To.
@param AnyLocTo Match any value of the Location To segment */
public void SetAnyLocTo (Boolean AnyLocTo)
{
Set_Value ("AnyLocTo", AnyLocTo);
}
/** Get Any Location To.
@return Match any value of the Location To segment */
public Boolean IsAnyLocTo() 
{
Object oo = Get_Value("AnyLocTo");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Any Organization.
@param AnyOrg Match any value of the Organization segment */
public void SetAnyOrg (Boolean AnyOrg)
{
Set_Value ("AnyOrg", AnyOrg);
}
/** Get Any Organization.
@return Match any value of the Organization segment */
public Boolean IsAnyOrg() 
{
Object oo = Get_Value("AnyOrg");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Any Trx Organization.
@param AnyOrgTrx Match any value of the Transaction Organization segment */
public void SetAnyOrgTrx (Boolean AnyOrgTrx)
{
Set_Value ("AnyOrgTrx", AnyOrgTrx);
}
/** Get Any Trx Organization.
@return Match any value of the Transaction Organization segment */
public Boolean IsAnyOrgTrx() 
{
Object oo = Get_Value("AnyOrgTrx");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Any Product.
@param AnyProduct Match any value of the Product segment */
public void SetAnyProduct (Boolean AnyProduct)
{
Set_Value ("AnyProduct", AnyProduct);
}
/** Get Any Product.
@return Match any value of the Product segment */
public Boolean IsAnyProduct() 
{
Object oo = Get_Value("AnyProduct");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Any Project.
@param AnyProject Match any value of the Project segment */
public void SetAnyProject (Boolean AnyProject)
{
Set_Value ("AnyProject", AnyProject);
}
/** Get Any Project.
@return Match any value of the Project segment */
public Boolean IsAnyProject() 
{
Object oo = Get_Value("AnyProject");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Any Sales Region.
@param AnySalesRegion Match any value of the Sales Region segment */
public void SetAnySalesRegion (Boolean AnySalesRegion)
{
Set_Value ("AnySalesRegion", AnySalesRegion);
}
/** Get Any Sales Region.
@return Match any value of the Sales Region segment */
public Boolean IsAnySalesRegion() 
{
Object oo = Get_Value("AnySalesRegion");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Any User 1.
@param AnyUser1 Match any value of the User 1 segment */
public void SetAnyUser1 (Boolean AnyUser1)
{
Set_Value ("AnyUser1", AnyUser1);
}
/** Get Any User 1.
@return Match any value of the User 1 segment */
public Boolean IsAnyUser1() 
{
Object oo = Get_Value("AnyUser1");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Any User 2.
@param AnyUser2 Match any value of the User 2 segment */
public void SetAnyUser2 (Boolean AnyUser2)
{
Set_Value ("AnyUser2", AnyUser2);
}
/** Get Any User 2.
@return Match any value of the User 2 segment */
public Boolean IsAnyUser2() 
{
Object oo = Get_Value("AnyUser2");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Accounting Schema.
@param VAB_AccountBook_ID Rules for accounting */
public void SetVAB_AccountBook_ID (int VAB_AccountBook_ID)
{
if (VAB_AccountBook_ID < 1) throw new ArgumentException ("VAB_AccountBook_ID is mandatory.");
Set_ValueNoCheck ("VAB_AccountBook_ID", VAB_AccountBook_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetVAB_AccountBook_ID() 
{
Object ii = Get_Value("VAB_AccountBook_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Activity.
@param VAB_BillingCode_ID Business Activity */
public void SetVAB_BillingCode_ID (int VAB_BillingCode_ID)
{
if (VAB_BillingCode_ID <= 0) Set_Value ("VAB_BillingCode_ID", null);
else
Set_Value ("VAB_BillingCode_ID", VAB_BillingCode_ID);
}
/** Get Activity.
@return Business Activity */
public int GetVAB_BillingCode_ID() 
{
Object ii = Get_Value("VAB_BillingCode_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param VAB_BusinessPartner_ID Identifies a Business Partner */
public void SetVAB_BusinessPartner_ID (int VAB_BusinessPartner_ID)
{
if (VAB_BusinessPartner_ID <= 0) Set_Value ("VAB_BusinessPartner_ID", null);
else
Set_Value ("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetVAB_BusinessPartner_ID() 
{
Object ii = Get_Value("VAB_BusinessPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Campaign.
@param VAB_Promotion_ID Marketing Campaign */
public void SetVAB_Promotion_ID (int VAB_Promotion_ID)
{
if (VAB_Promotion_ID <= 0) Set_Value ("VAB_Promotion_ID", null);
else
Set_Value ("VAB_Promotion_ID", VAB_Promotion_ID);
}
/** Get Campaign.
@return Marketing Campaign */
public int GetVAB_Promotion_ID() 
{
Object ii = Get_Value("VAB_Promotion_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Document Type.
@param VAB_DocTypes_ID Document type or rules */
public void SetVAB_DocTypes_ID (int VAB_DocTypes_ID)
{
if (VAB_DocTypes_ID <= 0) Set_Value ("VAB_DocTypes_ID", null);
else
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

/** C_LocFrom_ID VAF_Control_Ref_ID=133 */
public static int C_LOCFROM_ID_VAF_Control_Ref_ID=133;
/** Set Location From.
@param C_LocFrom_ID Location that inventory was moved from */
public void SetC_LocFrom_ID (int C_LocFrom_ID)
{
if (C_LocFrom_ID <= 0) Set_Value ("C_LocFrom_ID", null);
else
Set_Value ("C_LocFrom_ID", C_LocFrom_ID);
}
/** Get Location From.
@return Location that inventory was moved from */
public int GetC_LocFrom_ID() 
{
Object ii = Get_Value("C_LocFrom_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_LocTo_ID VAF_Control_Ref_ID=133 */
public static int C_LOCTO_ID_VAF_Control_Ref_ID=133;
/** Set Location To.
@param C_LocTo_ID Location that inventory was moved to */
public void SetC_LocTo_ID (int C_LocTo_ID)
{
if (C_LocTo_ID <= 0) Set_Value ("C_LocTo_ID", null);
else
Set_Value ("C_LocTo_ID", C_LocTo_ID);
}
/** Get Location To.
@return Location that inventory was moved to */
public int GetC_LocTo_ID() 
{
Object ii = Get_Value("C_LocTo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project.
@param VAB_Project_ID Financial Project */
public void SetVAB_Project_ID (int VAB_Project_ID)
{
if (VAB_Project_ID <= 0) Set_Value ("VAB_Project_ID", null);
else
Set_Value ("VAB_Project_ID", VAB_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetVAB_Project_ID() 
{
Object ii = Get_Value("VAB_Project_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sales Region.
@param VAB_SalesRegionState_ID Sales coverage region */
public void SetVAB_SalesRegionState_ID (int VAB_SalesRegionState_ID)
{
if (VAB_SalesRegionState_ID <= 0) Set_Value ("VAB_SalesRegionState_ID", null);
else
Set_Value ("VAB_SalesRegionState_ID", VAB_SalesRegionState_ID);
}
/** Get Sales Region.
@return Sales coverage region */
public int GetVAB_SalesRegionState_ID() 
{
Object ii = Get_Value("VAB_SalesRegionState_ID");
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
/** Set GL Distribution.
@param VAGL_Distribution_ID General Ledger Distribution */
public void SetVAGL_Distribution_ID (int VAGL_Distribution_ID)
{
if (VAGL_Distribution_ID < 1) throw new ArgumentException ("VAGL_Distribution_ID is mandatory.");
Set_ValueNoCheck ("VAGL_Distribution_ID", VAGL_Distribution_ID);
}
/** Get GL Distribution.
@return General Ledger Distribution */
public int GetVAGL_Distribution_ID() 
{
Object ii = Get_Value("VAGL_Distribution_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
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
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);
else
Set_Value ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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

/** Org_ID VAF_Control_Ref_ID=130 */
public static int ORG_ID_VAF_Control_Ref_ID=130;
/** Set Organization.
@param Org_ID Organizational entity within client */
public void SetOrg_ID (int Org_ID)
{
if (Org_ID <= 0) Set_Value ("Org_ID", null);
else
Set_Value ("Org_ID", Org_ID);
}
/** Get Organization.
@return Organizational entity within client */
public int GetOrg_ID() 
{
Object ii = Get_Value("Org_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Total Percent.
@param PercentTotal Sum of the Percent details */
public void SetPercentTotal (Decimal? PercentTotal)
{
if (PercentTotal == null) throw new ArgumentException ("PercentTotal is mandatory.");
Set_Value ("PercentTotal", (Decimal?)PercentTotal);
}
/** Get Total Percent.
@return Sum of the Percent details */
public Decimal GetPercentTotal() 
{
Object bd =Get_Value("PercentTotal");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** PostingType VAF_Control_Ref_ID=125 */
public static int POSTINGTYPE_VAF_Control_Ref_ID=125;
/** Actual = A */
public static String POSTINGTYPE_Actual = "A";
/** Budget = B */
public static String POSTINGTYPE_Budget = "B";
/** Commitment = E */
public static String POSTINGTYPE_Commitment = "E";
/** Reservation = R */
public static String POSTINGTYPE_Reservation = "R";
/** Statistical = S */
public static String POSTINGTYPE_Statistical = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPostingTypeValid (String test)
{
return test == null || test.Equals("A") || test.Equals("B") || test.Equals("E") || test.Equals("R") || test.Equals("S");
}
/** Set PostingType.
@param PostingType The type of posted amount for the transaction */
public void SetPostingType (String PostingType)
{
if (!IsPostingTypeValid(PostingType))
throw new ArgumentException ("PostingType Invalid value - " + PostingType + " - Reference_ID=125 - A - B - E - R - S");
if (PostingType != null && PostingType.Length > 1)
{
log.Warning("Length > 1 - truncated");
PostingType = PostingType.Substring(0,1);
}
Set_Value ("PostingType", PostingType);
}
/** Get PostingType.
@return The type of posted amount for the transaction */
public String GetPostingType() 
{
return (String)Get_Value("PostingType");
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

/** User1_ID VAF_Control_Ref_ID=134 */
public static int USER1_ID_VAF_Control_Ref_ID=134;
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

/** User2_ID VAF_Control_Ref_ID=137 */
public static int USER2_ID_VAF_Control_Ref_ID=137;
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
}

}
