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
/** Generated Model for VAB_Acct_ValidParameter
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_Acct_ValidParameter : PO
{
public X_VAB_Acct_ValidParameter (Context ctx, int VAB_Acct_ValidParameter_ID, Trx trxName) : base (ctx, VAB_Acct_ValidParameter_ID, trxName)
{
/** if (VAB_Acct_ValidParameter_ID == 0)
{
SetAccount_ID (0);
SetVAB_AccountBook_ID (0);
SetVAB_Acct_ValidParameter_ID (0);
SetIsFullyQualified (false);
}
 */
}
public X_VAB_Acct_ValidParameter (Ctx ctx, int VAB_Acct_ValidParameter_ID, Trx trxName) : base (ctx, VAB_Acct_ValidParameter_ID, trxName)
{
/** if (VAB_Acct_ValidParameter_ID == 0)
{
SetAccount_ID (0);
SetVAB_AccountBook_ID (0);
SetVAB_Acct_ValidParameter_ID (0);
SetIsFullyQualified (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Acct_ValidParameter (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Acct_ValidParameter (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Acct_ValidParameter (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_Acct_ValidParameter()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375855L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059066L;
/** VAF_TableView_ID=176 */
public static int Table_ID;
 // =176;

/** TableName=VAB_Acct_ValidParameter */
public static String Table_Name="VAB_Acct_ValidParameter";

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
StringBuilder sb = new StringBuilder ("X_VAB_Acct_ValidParameter[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** VAF_OrgTrx_ID VAF_Control_Ref_ID=130 */
public static int VAF_ORGTRX_ID_VAF_Control_Ref_ID=130;
/** Set Trx Organization.
@param VAF_OrgTrx_ID Performing or initiating organization */
public void SetVAF_OrgTrx_ID (int VAF_OrgTrx_ID)
{
if (VAF_OrgTrx_ID <= 0) Set_ValueNoCheck ("VAF_OrgTrx_ID", null);
else
Set_ValueNoCheck ("VAF_OrgTrx_ID", VAF_OrgTrx_ID);
}
/** Get Trx Organization.
@return Performing or initiating organization */
public int GetVAF_OrgTrx_ID() 
{
Object ii = Get_Value("VAF_OrgTrx_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Account_ID VAF_Control_Ref_ID=362 */
public static int ACCOUNT_ID_VAF_Control_Ref_ID=362;
/** Set Account.
@param Account_ID Account used */
public void SetAccount_ID (int Account_ID)
{
if (Account_ID < 1) throw new ArgumentException ("Account_ID is mandatory.");
Set_ValueNoCheck ("Account_ID", Account_ID);
}
/** Get Account.
@return Account used */
public int GetAccount_ID() 
{
Object ii = Get_Value("Account_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Alias.
@param Alias Defines an alternate method of indicating an account combination. */
public void SetAlias (String Alias)
{
if (Alias != null && Alias.Length > 40)
{
log.Warning("Length > 40 - truncated");
Alias = Alias.Substring(0,40);
}
Set_Value ("Alias", Alias);
}
/** Get Alias.
@return Defines an alternate method of indicating an account combination. */
public String GetAlias() 
{
return (String)Get_Value("Alias");
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

/** VAB_BillingCode_ID VAF_Control_Ref_ID=142 */
public static int VAB_BILLINGCODE_ID_VAF_Control_Ref_ID=142;
/** Set Activity.
@param VAB_BillingCode_ID Business Activity */
public void SetVAB_BillingCode_ID (int VAB_BillingCode_ID)
{
if (VAB_BillingCode_ID <= 0) Set_ValueNoCheck ("VAB_BillingCode_ID", null);
else
Set_ValueNoCheck ("VAB_BillingCode_ID", VAB_BillingCode_ID);
}
/** Get Activity.
@return Business Activity */
public int GetVAB_BillingCode_ID() 
{
Object ii = Get_Value("VAB_BillingCode_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAB_BusinessPartner_ID VAF_Control_Ref_ID=138 */
public static int VAB_BUSINESSPARTNER_ID_VAF_Control_Ref_ID=138;
/** Set Business Partner.
@param VAB_BusinessPartner_ID Identifies a Business Partner */
public void SetVAB_BusinessPartner_ID (int VAB_BusinessPartner_ID)
{
if (VAB_BusinessPartner_ID <= 0) Set_ValueNoCheck ("VAB_BusinessPartner_ID", null);
else
Set_ValueNoCheck ("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetVAB_BusinessPartner_ID() 
{
Object ii = Get_Value("VAB_BusinessPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAB_Promotion_ID VAF_Control_Ref_ID=143 */
public static int VAB_PROMOTION_ID_VAF_Control_Ref_ID=143;
/** Set Campaign.
@param VAB_Promotion_ID Marketing Campaign */
public void SetVAB_Promotion_ID (int VAB_Promotion_ID)
{
if (VAB_Promotion_ID <= 0) Set_ValueNoCheck ("VAB_Promotion_ID", null);
else
Set_ValueNoCheck ("VAB_Promotion_ID", VAB_Promotion_ID);
}
/** Get Campaign.
@return Marketing Campaign */
public int GetVAB_Promotion_ID() 
{
Object ii = Get_Value("VAB_Promotion_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_LocFrom_ID VAF_Control_Ref_ID=133 */
public static int C_LOCFROM_ID_VAF_Control_Ref_ID=133;
/** Set Location From.
@param C_LocFrom_ID Location that inventory was moved from */
public void SetC_LocFrom_ID (int C_LocFrom_ID)
{
if (C_LocFrom_ID <= 0) Set_ValueNoCheck ("C_LocFrom_ID", null);
else
Set_ValueNoCheck ("C_LocFrom_ID", C_LocFrom_ID);
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
if (C_LocTo_ID <= 0) Set_ValueNoCheck ("C_LocTo_ID", null);
else
Set_ValueNoCheck ("C_LocTo_ID", C_LocTo_ID);
}
/** Get Location To.
@return Location that inventory was moved to */
public int GetC_LocTo_ID() 
{
Object ii = Get_Value("C_LocTo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAB_Project_ID VAF_Control_Ref_ID=141 */
public static int VAB_PROJECT_ID_VAF_Control_Ref_ID=141;
/** Set Project.
@param VAB_Project_ID Financial Project */
public void SetVAB_Project_ID (int VAB_Project_ID)
{
if (VAB_Project_ID <= 0) Set_ValueNoCheck ("VAB_Project_ID", null);
else
Set_ValueNoCheck ("VAB_Project_ID", VAB_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetVAB_Project_ID() 
{
Object ii = Get_Value("VAB_Project_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAB_SalesRegionState_ID VAF_Control_Ref_ID=144 */
public static int VAB_SALESREGIONSTATE_ID_VAF_Control_Ref_ID=144;
/** Set Sales Region.
@param VAB_SalesRegionState_ID Sales coverage region */
public void SetVAB_SalesRegionState_ID (int VAB_SalesRegionState_ID)
{
if (VAB_SalesRegionState_ID <= 0) Set_ValueNoCheck ("VAB_SalesRegionState_ID", null);
else
Set_ValueNoCheck ("VAB_SalesRegionState_ID", VAB_SalesRegionState_ID);
}
/** Get Sales Region.
@return Sales coverage region */
public int GetVAB_SalesRegionState_ID() 
{
Object ii = Get_Value("VAB_SalesRegionState_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sub Account.
@param VAB_SubAcct_ID Sub account for Element Value */
public void SetVAB_SubAcct_ID (int VAB_SubAcct_ID)
{
if (VAB_SubAcct_ID <= 0) Set_ValueNoCheck ("VAB_SubAcct_ID", null);
else
Set_ValueNoCheck ("VAB_SubAcct_ID", VAB_SubAcct_ID);
}
/** Get Sub Account.
@return Sub account for Element Value */
public int GetVAB_SubAcct_ID() 
{
Object ii = Get_Value("VAB_SubAcct_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Combination.
@param VAB_Acct_ValidParameter_ID Valid Account Combination */
public void SetVAB_Acct_ValidParameter_ID (int VAB_Acct_ValidParameter_ID)
{
if (VAB_Acct_ValidParameter_ID < 1) throw new ArgumentException ("VAB_Acct_ValidParameter_ID is mandatory.");
Set_ValueNoCheck ("VAB_Acct_ValidParameter_ID", VAB_Acct_ValidParameter_ID);
}
/** Get Combination.
@return Valid Account Combination */
public int GetVAB_Acct_ValidParameter_ID() 
{
Object ii = Get_Value("VAB_Acct_ValidParameter_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Combination.
@param Combination Unique combination of account elements */
public void SetCombination (String Combination)
{
if (Combination != null && Combination.Length > 60)
{
log.Warning("Length > 60 - truncated");
Combination = Combination.Substring(0,60);
}
Set_ValueNoCheck ("Combination", Combination);
}
/** Get Combination.
@return Unique combination of account elements */
public String GetCombination() 
{
return (String)Get_Value("Combination");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetCombination());
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
Set_ValueNoCheck ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Fully Qualified.
@param IsFullyQualified This account is fully qualified */
public void SetIsFullyQualified (Boolean IsFullyQualified)
{
Set_ValueNoCheck ("IsFullyQualified", IsFullyQualified);
}
/** Get Fully Qualified.
@return This account is fully qualified */
public Boolean IsFullyQualified() 
{
Object oo = Get_Value("IsFullyQualified");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}

/** VAM_Product_ID VAF_Control_Ref_ID=162 */
public static int VAM_Product_ID_VAF_Control_Ref_ID=162;
/** Set Product.
@param VAM_Product_ID Product, Service, Item */
public void SetVAM_Product_ID (int VAM_Product_ID)
{
if (VAM_Product_ID <= 0) Set_ValueNoCheck ("VAM_Product_ID", null);
else
Set_ValueNoCheck ("VAM_Product_ID", VAM_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetVAM_Product_ID() 
{
Object ii = Get_Value("VAM_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** User1_ID VAF_Control_Ref_ID=134 */
public static int USER1_ID_VAF_Control_Ref_ID=134;
/** Set User List 1.
@param User1_ID User defined list element #1 */
public void SetUser1_ID (int User1_ID)
{
if (User1_ID <= 0) Set_ValueNoCheck ("User1_ID", null);
else
Set_ValueNoCheck ("User1_ID", User1_ID);
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
if (User2_ID <= 0) Set_ValueNoCheck ("User2_ID", null);
else
Set_ValueNoCheck ("User2_ID", User2_ID);
}
/** Get User List 2.
@return User defined list element #2 */
public int GetUser2_ID() 
{
Object ii = Get_Value("User2_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User Element 1.
@param UserElement1_ID User defined accounting Element */
public void SetUserElement1_ID (int UserElement1_ID)
{
if (UserElement1_ID <= 0) Set_Value ("UserElement1_ID", null);
else
Set_Value ("UserElement1_ID", UserElement1_ID);
}
/** Get User Element 1.
@return User defined accounting Element */
public int GetUserElement1_ID() 
{
Object ii = Get_Value("UserElement1_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User Element 2.
@param UserElement2_ID User defined accounting Element */
public void SetUserElement2_ID (int UserElement2_ID)
{
if (UserElement2_ID <= 0) Set_Value ("UserElement2_ID", null);
else
Set_Value ("UserElement2_ID", UserElement2_ID);
}
/** Get User Element 2.
@return User defined accounting Element */
public int GetUserElement2_ID() 
{
Object ii = Get_Value("UserElement2_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Set User Element 3.
@param UserElement3_ID User defined accounting Element */
public void SetUserElement3_ID(int UserElement3_ID)
{
    if (UserElement3_ID <= 0) Set_Value("UserElement3_ID", null);
    else
        Set_Value("UserElement3_ID", UserElement3_ID);
}
/** Get User Element 3.
@return User defined accounting Element */
public int GetUserElement3_ID()
{
    Object ii = Get_Value("UserElement3_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set User Element 4.
@param UserElement4_ID User defined accounting Element */
public void SetUserElement4_ID(int UserElement4_ID)
{
    if (UserElement4_ID <= 0) Set_Value("UserElement4_ID", null);
    else
        Set_Value("UserElement4_ID", UserElement4_ID);
}
/** Get User Element 4.
@return User defined accounting Element */
public int GetUserElement4_ID()
{
    Object ii = Get_Value("UserElement4_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set User Element 5.
@param UserElement5_ID User defined accounting Element */
public void SetUserElement5_ID(int UserElement5_ID)
{
    if (UserElement5_ID <= 0) Set_Value("UserElement5_ID", null);
    else
        Set_Value("UserElement5_ID", UserElement5_ID);
}
/** Get User Element 5.
@return User defined accounting Element */
public int GetUserElement5_ID()
{
    Object ii = Get_Value("UserElement5_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set User Element 6.
@param UserElement6_ID User defined accounting Element */
public void SetUserElement6_ID(int UserElement6_ID)
{
    if (UserElement6_ID <= 0) Set_Value("UserElement6_ID", null);
    else
        Set_Value("UserElement6_ID", UserElement6_ID);
}
/** Get User Element 6.
@return User defined accounting Element */
public int GetUserElement6_ID()
{
    Object ii = Get_Value("UserElement6_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set User Element 7.
@param UserElement7_ID User defined accounting Element */
public void SetUserElement7_ID(int UserElement7_ID)
{
    if (UserElement7_ID <= 0) Set_Value("UserElement7_ID", null);
    else
        Set_Value("UserElement7_ID", UserElement7_ID);
}
/** Get User Element 7.
@return User defined accounting Element */
public int GetUserElement7_ID()
{
    Object ii = Get_Value("UserElement7_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set User Element 8.
@param UserElement8_ID User defined accounting Element */
public void SetUserElement8_ID(int UserElement8_ID)
{
    if (UserElement8_ID <= 0) Set_Value("UserElement8_ID", null);
    else
        Set_Value("UserElement8_ID", UserElement8_ID);
}
/** Get User Element 8.
@return User defined accounting Element */
public int GetUserElement8_ID()
{
    Object ii = Get_Value("UserElement8_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set User Element 9.
@param UserElement9_ID User defined accounting Element */
public void SetUserElement9_ID(int UserElement9_ID)
{
    if (UserElement9_ID <= 0) Set_Value("UserElement9_ID", null);
    else
        Set_Value("UserElement9_ID", UserElement9_ID);
}
/** Get User Element 1.
@return User defined accounting Element */
public int GetUserElement9_ID()
{
    Object ii = Get_Value("UserElement9_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
}

}
