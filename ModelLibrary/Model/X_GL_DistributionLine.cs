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
/** Generated Model for GL_DistributionLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_GL_DistributionLine : PO
{
public X_GL_DistributionLine (Context ctx, int GL_DistributionLine_ID, Trx trxName) : base (ctx, GL_DistributionLine_ID, trxName)
{
/** if (GL_DistributionLine_ID == 0)
{
SetGL_DistributionLine_ID (0);
SetGL_Distribution_ID (0);
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM GL_DistributionLine WHERE GL_Distribution_ID=@GL_Distribution_ID@
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
public X_GL_DistributionLine (Ctx ctx, int GL_DistributionLine_ID, Trx trxName) : base (ctx, GL_DistributionLine_ID, trxName)
{
/** if (GL_DistributionLine_ID == 0)
{
SetGL_DistributionLine_ID (0);
SetGL_Distribution_ID (0);
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM GL_DistributionLine WHERE GL_Distribution_ID=@GL_Distribution_ID@
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
public X_GL_DistributionLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_DistributionLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_DistributionLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_GL_DistributionLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376466L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059677L;
/** AD_Table_ID=707 */
public static int Table_ID;
 // =707;

/** TableName=GL_DistributionLine */
public static String Table_Name="GL_DistributionLine";

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
StringBuilder sb = new StringBuilder ("X_GL_DistributionLine[").Append(Get_ID()).Append("]");
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

/** Account_ID AD_Reference_ID=132 */
public static int ACCOUNT_ID_AD_Reference_ID=132;
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
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID <= 0) Set_Value ("C_BPartner_ID", null);
else
Set_Value ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
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

/** C_LocFrom_ID AD_Reference_ID=133 */
public static int C_LOCFROM_ID_AD_Reference_ID=133;
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

/** C_LocTo_ID AD_Reference_ID=133 */
public static int C_LOCTO_ID_AD_Reference_ID=133;
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
/** Set Sales Region.
@param C_SalesRegion_ID Sales coverage region */
public void SetC_SalesRegion_ID (int C_SalesRegion_ID)
{
if (C_SalesRegion_ID <= 0) Set_Value ("C_SalesRegion_ID", null);
else
Set_Value ("C_SalesRegion_ID", C_SalesRegion_ID);
}
/** Get Sales Region.
@return Sales coverage region */
public int GetC_SalesRegion_ID() 
{
Object ii = Get_Value("C_SalesRegion_ID");
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
@param GL_DistributionLine_ID General Ledger Distribution Line */
public void SetGL_DistributionLine_ID (int GL_DistributionLine_ID)
{
if (GL_DistributionLine_ID < 1) throw new ArgumentException ("GL_DistributionLine_ID is mandatory.");
Set_ValueNoCheck ("GL_DistributionLine_ID", GL_DistributionLine_ID);
}
/** Get GL Distribution Line.
@return General Ledger Distribution Line */
public int GetGL_DistributionLine_ID() 
{
Object ii = Get_Value("GL_DistributionLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set GL Distribution.
@param GL_Distribution_ID General Ledger Distribution */
public void SetGL_Distribution_ID (int GL_Distribution_ID)
{
if (GL_Distribution_ID < 1) throw new ArgumentException ("GL_Distribution_ID is mandatory.");
Set_ValueNoCheck ("GL_Distribution_ID", GL_Distribution_ID);
}
/** Get GL Distribution.
@return General Ledger Distribution */
public int GetGL_Distribution_ID() 
{
Object ii = Get_Value("GL_Distribution_ID");
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

/** Org_ID AD_Reference_ID=130 */
public static int ORG_ID_AD_Reference_ID=130;
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
}

}
