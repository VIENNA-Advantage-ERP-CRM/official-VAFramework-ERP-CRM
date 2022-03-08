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
/** Generated Model for C_ValidCombination
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_ValidCombination : PO
{
public X_C_ValidCombination (Context ctx, int C_ValidCombination_ID, Trx trxName) : base (ctx, C_ValidCombination_ID, trxName)
{
/** if (C_ValidCombination_ID == 0)
{
SetAccount_ID (0);
SetC_AcctSchema_ID (0);
SetC_ValidCombination_ID (0);
SetIsFullyQualified (false);
}
 */
}
public X_C_ValidCombination (Ctx ctx, int C_ValidCombination_ID, Trx trxName) : base (ctx, C_ValidCombination_ID, trxName)
{
/** if (C_ValidCombination_ID == 0)
{
SetAccount_ID (0);
SetC_AcctSchema_ID (0);
SetC_ValidCombination_ID (0);
SetIsFullyQualified (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ValidCombination (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ValidCombination (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ValidCombination (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_ValidCombination()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375855L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059066L;
/** AD_Table_ID=176 */
public static int Table_ID;
 // =176;

/** TableName=C_ValidCombination */
public static String Table_Name="C_ValidCombination";

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
StringBuilder sb = new StringBuilder ("X_C_ValidCombination[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_OrgTrx_ID AD_Reference_ID=130 */
public static int AD_ORGTRX_ID_AD_Reference_ID=130;
/** Set Trx Organization.
@param AD_OrgTrx_ID Performing or initiating organization */
public void SetAD_OrgTrx_ID (int AD_OrgTrx_ID)
{
if (AD_OrgTrx_ID <= 0) Set_ValueNoCheck ("AD_OrgTrx_ID", null);
else
Set_ValueNoCheck ("AD_OrgTrx_ID", AD_OrgTrx_ID);
}
/** Get Trx Organization.
@return Performing or initiating organization */
public int GetAD_OrgTrx_ID() 
{
Object ii = Get_Value("AD_OrgTrx_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Account_ID AD_Reference_ID=362 */
public static int ACCOUNT_ID_AD_Reference_ID=362;
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
@param C_AcctSchema_ID Rules for accounting */
public void SetC_AcctSchema_ID (int C_AcctSchema_ID)
{
if (C_AcctSchema_ID < 1) throw new ArgumentException ("C_AcctSchema_ID is mandatory.");
Set_ValueNoCheck ("C_AcctSchema_ID", C_AcctSchema_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetC_AcctSchema_ID() 
{
Object ii = Get_Value("C_AcctSchema_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_Activity_ID AD_Reference_ID=142 */
public static int C_ACTIVITY_ID_AD_Reference_ID=142;
/** Set Activity.
@param C_Activity_ID Business Activity */
public void SetC_Activity_ID (int C_Activity_ID)
{
if (C_Activity_ID <= 0) Set_ValueNoCheck ("C_Activity_ID", null);
else
Set_ValueNoCheck ("C_Activity_ID", C_Activity_ID);
}
/** Get Activity.
@return Business Activity */
public int GetC_Activity_ID() 
{
Object ii = Get_Value("C_Activity_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_BPartner_ID AD_Reference_ID=138 */
public static int C_BPARTNER_ID_AD_Reference_ID=138;
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID <= 0) Set_ValueNoCheck ("C_BPartner_ID", null);
else
Set_ValueNoCheck ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_Campaign_ID AD_Reference_ID=143 */
public static int C_CAMPAIGN_ID_AD_Reference_ID=143;
/** Set Campaign.
@param C_Campaign_ID Marketing Campaign */
public void SetC_Campaign_ID (int C_Campaign_ID)
{
if (C_Campaign_ID <= 0) Set_ValueNoCheck ("C_Campaign_ID", null);
else
Set_ValueNoCheck ("C_Campaign_ID", C_Campaign_ID);
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

/** C_LocTo_ID AD_Reference_ID=133 */
public static int C_LOCTO_ID_AD_Reference_ID=133;
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

/** C_Project_ID AD_Reference_ID=141 */
public static int C_PROJECT_ID_AD_Reference_ID=141;
/** Set Project.
@param C_Project_ID Financial Project */
public void SetC_Project_ID (int C_Project_ID)
{
if (C_Project_ID <= 0) Set_ValueNoCheck ("C_Project_ID", null);
else
Set_ValueNoCheck ("C_Project_ID", C_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetC_Project_ID() 
{
Object ii = Get_Value("C_Project_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_SalesRegion_ID AD_Reference_ID=144 */
public static int C_SALESREGION_ID_AD_Reference_ID=144;
/** Set Sales Region.
@param C_SalesRegion_ID Sales coverage region */
public void SetC_SalesRegion_ID (int C_SalesRegion_ID)
{
if (C_SalesRegion_ID <= 0) Set_ValueNoCheck ("C_SalesRegion_ID", null);
else
Set_ValueNoCheck ("C_SalesRegion_ID", C_SalesRegion_ID);
}
/** Get Sales Region.
@return Sales coverage region */
public int GetC_SalesRegion_ID() 
{
Object ii = Get_Value("C_SalesRegion_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sub Account.
@param C_SubAcct_ID Sub account for Element Value */
public void SetC_SubAcct_ID (int C_SubAcct_ID)
{
if (C_SubAcct_ID <= 0) Set_ValueNoCheck ("C_SubAcct_ID", null);
else
Set_ValueNoCheck ("C_SubAcct_ID", C_SubAcct_ID);
}
/** Get Sub Account.
@return Sub account for Element Value */
public int GetC_SubAcct_ID() 
{
Object ii = Get_Value("C_SubAcct_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Combination.
@param C_ValidCombination_ID Valid Account Combination */
public void SetC_ValidCombination_ID (int C_ValidCombination_ID)
{
if (C_ValidCombination_ID < 1) throw new ArgumentException ("C_ValidCombination_ID is mandatory.");
Set_ValueNoCheck ("C_ValidCombination_ID", C_ValidCombination_ID);
}
/** Get Combination.
@return Valid Account Combination */
public int GetC_ValidCombination_ID() 
{
Object ii = Get_Value("C_ValidCombination_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Combination.
@param Combination Unique combination of account elements */
public void SetCombination (String Combination)
{
if (Combination != null && Combination.Length > 255)
{
log.Warning("Length > 255 - truncated");
Combination = Combination.Substring(0,255);
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

/** M_Product_ID AD_Reference_ID=162 */
public static int M_PRODUCT_ID_AD_Reference_ID=162;
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID <= 0) Set_ValueNoCheck ("M_Product_ID", null);
else
Set_ValueNoCheck ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** User1_ID AD_Reference_ID=134 */
public static int USER1_ID_AD_Reference_ID=134;
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

/** User2_ID AD_Reference_ID=137 */
public static int USER2_ID_AD_Reference_ID=137;
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
