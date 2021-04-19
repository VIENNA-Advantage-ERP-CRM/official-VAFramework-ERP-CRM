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
/** Generated Model for VAGL_DistributionLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAGL_DistributionLine : PO
{
public X_VAGL_DistributionLine (Context ctx, int VAGL_DistributionLine_ID, Trx trxName) : base (ctx, VAGL_DistributionLine_ID, trxName)
{
/** if (VAGL_DistributionLine_ID == 0)
{
SetVAGL_DistributionLine_ID (0);
SetVAGL_Distribution_ID (0);
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAGL_DistributionLine WHERE VAGL_Distribution_ID=@VAGL_Distribution_ID@
SetOverwriteAcct (false);
SetOverwriteActivity (false);
SetOverwriteBPartner (false);
SetOverwriteCampaign (false);
SetOverwriteLocFrom (false);
SetOverwriteLocTo (false);
SetOverwriteOrg (false);
SetOverwriteOrgTrx (false);
SetOverwriteProduct (false);
SetOverwriteProject (false);
SetOverwriteSalesRegion (false);
SetOverwriteUser1 (false);
SetOverwriteUser2 (false);
SetPercentDistribution (0.0);
}
 */
}
public X_VAGL_DistributionLine (Ctx ctx, int VAGL_DistributionLine_ID, Trx trxName) : base (ctx, VAGL_DistributionLine_ID, trxName)
{
/** if (VAGL_DistributionLine_ID == 0)
{
SetVAGL_DistributionLine_ID (0);
SetVAGL_Distribution_ID (0);
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAGL_DistributionLine WHERE VAGL_Distribution_ID=@VAGL_Distribution_ID@
SetOverwriteAcct (false);
SetOverwriteActivity (false);
SetOverwriteBPartner (false);
SetOverwriteCampaign (false);
SetOverwriteLocFrom (false);
SetOverwriteLocTo (false);
SetOverwriteOrg (false);
SetOverwriteOrgTrx (false);
SetOverwriteProduct (false);
SetOverwriteProject (false);
SetOverwriteSalesRegion (false);
SetOverwriteUser1 (false);
SetOverwriteUser2 (false);
SetPercentDistribution (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAGL_DistributionLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAGL_DistributionLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAGL_DistributionLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAGL_DistributionLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376466L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059677L;
/** VAF_TableView_ID=707 */
public static int Table_ID;
 // =707;

/** TableName=VAGL_DistributionLine */
public static String Table_Name="VAGL_DistributionLine";

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
StringBuilder sb = new StringBuilder ("X_VAGL_DistributionLine[").Append(Get_ID()).Append("]");
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

/** VAB_LocFrom_ID VAF_Control_Ref_ID=133 */
public static int VAB_LocFrom_ID_VAF_Control_Ref_ID=133;
/** Set Location From.
@param VAB_LocFrom_ID Location that inventory was moved from */
public void SetVAB_LocFrom_ID (int VAB_LocFrom_ID)
{
if (VAB_LocFrom_ID <= 0) Set_Value ("VAB_LocFrom_ID", null);
else
Set_Value ("VAB_LocFrom_ID", VAB_LocFrom_ID);
}
/** Get Location From.
@return Location that inventory was moved from */
public int GetVAB_LocFrom_ID() 
{
Object ii = Get_Value("VAB_LocFrom_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAB_LocTo_ID VAF_Control_Ref_ID=133 */
public static int VAB_LocTo_ID_VAF_Control_Ref_ID=133;
/** Set Location To.
@param VAB_LocTo_ID Location that inventory was moved to */
public void SetVAB_LocTo_ID (int VAB_LocTo_ID)
{
if (VAB_LocTo_ID <= 0) Set_Value ("VAB_LocTo_ID", null);
else
Set_Value ("VAB_LocTo_ID", VAB_LocTo_ID);
}
/** Get Location To.
@return Location that inventory was moved to */
public int GetVAB_LocTo_ID() 
{
Object ii = Get_Value("VAB_LocTo_ID");
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
/** Set GL Distribution Line.
@param VAGL_DistributionLine_ID General Ledger Distribution Line */
public void SetVAGL_DistributionLine_ID (int VAGL_DistributionLine_ID)
{
if (VAGL_DistributionLine_ID < 1) throw new ArgumentException ("VAGL_DistributionLine_ID is mandatory.");
Set_ValueNoCheck ("VAGL_DistributionLine_ID", VAGL_DistributionLine_ID);
}
/** Get GL Distribution Line.
@return General Ledger Distribution Line */
public int GetVAGL_DistributionLine_ID() 
{
Object ii = Get_Value("VAGL_DistributionLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Line No.
@param Line Unique line for this document */
public void SetLine (int Line)
{
Set_Value ("Line", Line);
}
/** Get Line No.
@return Unique line for this document */
public int GetLine() 
{
Object ii = Get_Value("Line");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetLine().ToString());
}
/** Set Product.
@param VAM_Product_ID Product, Service, Item */
public void SetVAM_Product_ID (int VAM_Product_ID)
{
if (VAM_Product_ID <= 0) Set_Value ("VAM_Product_ID", null);
else
Set_Value ("VAM_Product_ID", VAM_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetVAM_Product_ID() 
{
Object ii = Get_Value("VAM_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Overwrite Account.
@param OverwriteAcct Overwrite the account segment Account with the value specified */
public void SetOverwriteAcct (Boolean OverwriteAcct)
{
Set_Value ("OverwriteAcct", OverwriteAcct);
}
/** Get Overwrite Account.
@return Overwrite the account segment Account with the value specified */
public Boolean IsOverwriteAcct() 
{
Object oo = Get_Value("OverwriteAcct");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Overwrite Activity.
@param OverwriteActivity Overwrite the account segment Activity with the value specified */
public void SetOverwriteActivity (Boolean OverwriteActivity)
{
Set_Value ("OverwriteActivity", OverwriteActivity);
}
/** Get Overwrite Activity.
@return Overwrite the account segment Activity with the value specified */
public Boolean IsOverwriteActivity() 
{
Object oo = Get_Value("OverwriteActivity");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Overwrite Bus.Partner.
@param OverwriteBPartner Overwrite the account segment Business Partner with the value specified */
public void SetOverwriteBPartner (Boolean OverwriteBPartner)
{
Set_Value ("OverwriteBPartner", OverwriteBPartner);
}
/** Get Overwrite Bus.Partner.
@return Overwrite the account segment Business Partner with the value specified */
public Boolean IsOverwriteBPartner() 
{
Object oo = Get_Value("OverwriteBPartner");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Overwrite Campaign.
@param OverwriteCampaign Overwrite the account segment Campaign with the value specified */
public void SetOverwriteCampaign (Boolean OverwriteCampaign)
{
Set_Value ("OverwriteCampaign", OverwriteCampaign);
}
/** Get Overwrite Campaign.
@return Overwrite the account segment Campaign with the value specified */
public Boolean IsOverwriteCampaign() 
{
Object oo = Get_Value("OverwriteCampaign");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Overwrite Location From.
@param OverwriteLocFrom Overwrite the account segment Location From with the value specified */
public void SetOverwriteLocFrom (Boolean OverwriteLocFrom)
{
Set_Value ("OverwriteLocFrom", OverwriteLocFrom);
}
/** Get Overwrite Location From.
@return Overwrite the account segment Location From with the value specified */
public Boolean IsOverwriteLocFrom() 
{
Object oo = Get_Value("OverwriteLocFrom");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Overwrite Location To.
@param OverwriteLocTo Overwrite the account segment Location From with the value specified */
public void SetOverwriteLocTo (Boolean OverwriteLocTo)
{
Set_Value ("OverwriteLocTo", OverwriteLocTo);
}
/** Get Overwrite Location To.
@return Overwrite the account segment Location From with the value specified */
public Boolean IsOverwriteLocTo() 
{
Object oo = Get_Value("OverwriteLocTo");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Overwrite Organization.
@param OverwriteOrg Overwrite the account segment Organization with the value specified */
public void SetOverwriteOrg (Boolean OverwriteOrg)
{
Set_Value ("OverwriteOrg", OverwriteOrg);
}
/** Get Overwrite Organization.
@return Overwrite the account segment Organization with the value specified */
public Boolean IsOverwriteOrg() 
{
Object oo = Get_Value("OverwriteOrg");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Overwrite Trx Organuzation.
@param OverwriteOrgTrx Overwrite the account segment Transaction Organization with the value specified */
public void SetOverwriteOrgTrx (Boolean OverwriteOrgTrx)
{
Set_Value ("OverwriteOrgTrx", OverwriteOrgTrx);
}
/** Get Overwrite Trx Organuzation.
@return Overwrite the account segment Transaction Organization with the value specified */
public Boolean IsOverwriteOrgTrx() 
{
Object oo = Get_Value("OverwriteOrgTrx");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Overwrite Product.
@param OverwriteProduct Overwrite the account segment Product with the value specified */
public void SetOverwriteProduct (Boolean OverwriteProduct)
{
Set_Value ("OverwriteProduct", OverwriteProduct);
}
/** Get Overwrite Product.
@return Overwrite the account segment Product with the value specified */
public Boolean IsOverwriteProduct() 
{
Object oo = Get_Value("OverwriteProduct");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Overwrite Project.
@param OverwriteProject Overwrite the account segment Project with the value specified */
public void SetOverwriteProject (Boolean OverwriteProject)
{
Set_Value ("OverwriteProject", OverwriteProject);
}
/** Get Overwrite Project.
@return Overwrite the account segment Project with the value specified */
public Boolean IsOverwriteProject() 
{
Object oo = Get_Value("OverwriteProject");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Overwrite Sales Region.
@param OverwriteSalesRegion Overwrite the account segment Sales Region with the value specified */
public void SetOverwriteSalesRegion (Boolean OverwriteSalesRegion)
{
Set_Value ("OverwriteSalesRegion", OverwriteSalesRegion);
}
/** Get Overwrite Sales Region.
@return Overwrite the account segment Sales Region with the value specified */
public Boolean IsOverwriteSalesRegion() 
{
Object oo = Get_Value("OverwriteSalesRegion");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Overwrite User1.
@param OverwriteUser1 Overwrite the account segment User 1 with the value specified */
public void SetOverwriteUser1 (Boolean OverwriteUser1)
{
Set_Value ("OverwriteUser1", OverwriteUser1);
}
/** Get Overwrite User1.
@return Overwrite the account segment User 1 with the value specified */
public Boolean IsOverwriteUser1() 
{
Object oo = Get_Value("OverwriteUser1");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Overwrite User2.
@param OverwriteUser2 Overwrite the account segment User 2 with the value specified */
public void SetOverwriteUser2 (Boolean OverwriteUser2)
{
Set_Value ("OverwriteUser2", OverwriteUser2);
}
/** Get Overwrite User2.
@return Overwrite the account segment User 2 with the value specified */
public Boolean IsOverwriteUser2() 
{
Object oo = Get_Value("OverwriteUser2");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Percent.
@param PercentDistribution Percentage to be distributed */
public void SetPercentDistribution (Decimal? PercentDistribution)
{
if (PercentDistribution == null) throw new ArgumentException ("PercentDistribution is mandatory.");
Set_Value ("PercentDistribution", (Decimal?)PercentDistribution);
}
/** Get Percent.
@return Percentage to be distributed */
public Decimal GetPercentDistribution() 
{
Object bd =Get_Value("PercentDistribution");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
