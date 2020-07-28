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
/** Generated Model for C_AcctSchema_Element
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_AcctSchema_Element : PO
{
public X_C_AcctSchema_Element (Context ctx, int C_AcctSchema_Element_ID, Trx trxName) : base (ctx, C_AcctSchema_Element_ID, trxName)
{
/** if (C_AcctSchema_Element_ID == 0)
{
SetC_AcctSchema_Element_ID (0);
SetC_AcctSchema_ID (0);
SetElementType (null);
SetIsBalanced (false);
SetIsMandatory (false);
SetName (null);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM C_AcctSchema_Element WHERE C_AcctSchema_ID=@C_AcctSchema_ID@
}
 */
}
public X_C_AcctSchema_Element (Ctx ctx, int C_AcctSchema_Element_ID, Trx trxName) : base (ctx, C_AcctSchema_Element_ID, trxName)
{
/** if (C_AcctSchema_Element_ID == 0)
{
SetC_AcctSchema_Element_ID (0);
SetC_AcctSchema_ID (0);
SetElementType (null);
SetIsBalanced (false);
SetIsMandatory (false);
SetName (null);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM C_AcctSchema_Element WHERE C_AcctSchema_ID=@C_AcctSchema_ID@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AcctSchema_Element (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AcctSchema_Element (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AcctSchema_Element (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_AcctSchema_Element()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514369586L;
/** Last Updated Timestamp 7/29/2010 1:07:32 PM */
public static long updatedMS = 1280389052797L;
/** AD_Table_ID=279 */
public static int Table_ID;
 // =279;

/** TableName=C_AcctSchema_Element */
public static String Table_Name="C_AcctSchema_Element";

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
StringBuilder sb = new StringBuilder ("X_C_AcctSchema_Element[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Column.
@param AD_Column_ID Column in the table */
public void SetAD_Column_ID (int AD_Column_ID)
{
if (AD_Column_ID <= 0) Set_Value ("AD_Column_ID", null);
else
Set_Value ("AD_Column_ID", AD_Column_ID);
}
/** Get Column.
@return Column in the table */
public int GetAD_Column_ID() 
{
Object ii = Get_Value("AD_Column_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Acct.Schema Element.
@param C_AcctSchema_Element_ID Acct.Schema Element */
public void SetC_AcctSchema_Element_ID (int C_AcctSchema_Element_ID)
{
if (C_AcctSchema_Element_ID < 1) throw new ArgumentException ("C_AcctSchema_Element_ID is mandatory.");
Set_ValueNoCheck ("C_AcctSchema_Element_ID", C_AcctSchema_Element_ID);
}
/** Get Acct.Schema Element.
@return Acct.Schema Element */
public int GetC_AcctSchema_Element_ID() 
{
Object ii = Get_Value("C_AcctSchema_Element_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Account Element.
@param C_ElementValue_ID Account Element */
public void SetC_ElementValue_ID (int C_ElementValue_ID)
{
if (C_ElementValue_ID <= 0) Set_Value ("C_ElementValue_ID", null);
else
Set_Value ("C_ElementValue_ID", C_ElementValue_ID);
}
/** Get Account Element.
@return Account Element */
public int GetC_ElementValue_ID() 
{
Object ii = Get_Value("C_ElementValue_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Element.
@param C_Element_ID Accounting Element */
public void SetC_Element_ID (int C_Element_ID)
{
if (C_Element_ID <= 0) Set_Value ("C_Element_ID", null);
else
Set_Value ("C_Element_ID", C_Element_ID);
}
/** Get Element.
@return Accounting Element */
public int GetC_Element_ID() 
{
Object ii = Get_Value("C_Element_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Address.
@param C_Location_ID Location or Address */
public void SetC_Location_ID (int C_Location_ID)
{
if (C_Location_ID <= 0) Set_Value ("C_Location_ID", null);
else
Set_Value ("C_Location_ID", C_Location_ID);
}
/** Get Address.
@return Location or Address */
public int GetC_Location_ID() 
{
Object ii = Get_Value("C_Location_ID");
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

/** ElementType AD_Reference_ID=181 */
public static int ELEMENTTYPE_AD_Reference_ID=181;
/** Account = AC */
public static String ELEMENTTYPE_Account = "AC";
/** Activity = AY */
public static String ELEMENTTYPE_Activity = "AY";
/** BPartner = BP */
public static String ELEMENTTYPE_BPartner = "BP";
/** Location From = LF */
public static String ELEMENTTYPE_LocationFrom = "LF";
/** Location To = LT */
public static String ELEMENTTYPE_LocationTo = "LT";
/** Campaign = MC */
public static String ELEMENTTYPE_Campaign = "MC";
/** Organization = OO */
public static String ELEMENTTYPE_Organization = "OO";
/** Org Trx = OT */
public static String ELEMENTTYPE_OrgTrx = "OT";
/** Project = PJ */
public static String ELEMENTTYPE_Project = "PJ";
/** Product = PR */
public static String ELEMENTTYPE_Product = "PR";
/** Sub Account = SA */
public static String ELEMENTTYPE_SubAccount = "SA";
/** Sales Region = SR */
public static String ELEMENTTYPE_SalesRegion = "SR";
/** User List 1 = U1 */
public static String ELEMENTTYPE_UserList1 = "U1";
/** User List 2 = U2 */
public static String ELEMENTTYPE_UserList2 = "U2";
/** User Element 1 = X1 */
public static String ELEMENTTYPE_UserElement1 = "X1";
/** User Element 2 = X2 */
public static String ELEMENTTYPE_UserElement2 = "X2";
/** User Element 3 = X3 */
public static String ELEMENTTYPE_UserElement3 = "X3";
/** User Element 4 = X4 */
public static String ELEMENTTYPE_UserElement4 = "X4";
/** User Element 5 = X5 */
public static String ELEMENTTYPE_UserElement5 = "X5";
/** User Element 6 = X6 */
public static String ELEMENTTYPE_UserElement6 = "X6";
/** User Element 7 = X7 */
public static String ELEMENTTYPE_UserElement7 = "X7";
/** User Element 8 = X8 */
public static String ELEMENTTYPE_UserElement8 = "X8";
/** User Element 9 = X9 */
public static String ELEMENTTYPE_UserElement9 = "X9";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsElementTypeValid (String test)
{
    return test.Equals("AC") || test.Equals("AY") || test.Equals("BP") || test.Equals("LF") || test.Equals("LT") || test.Equals("MC") || test.Equals("OO") || test.Equals("OT") || test.Equals("PJ") || test.Equals("PR") || test.Equals("SA") || test.Equals("SR") || test.Equals("U1") || test.Equals("U2") || test.Equals("X1") || test.Equals("X2") || test.Equals("X3") || test.Equals("X4") || test.Equals("X5") || test.Equals("X6") || test.Equals("X7") || test.Equals("X8") || test.Equals("X9");
}
/** Set Type.
@param ElementType Element Type (account or user defined) */
public void SetElementType (String ElementType)
{
if (ElementType == null) throw new ArgumentException ("ElementType is mandatory");
if (!IsElementTypeValid(ElementType))
    throw new ArgumentException("ElementType Invalid value - " + ElementType + " - Reference_ID=181 - AC - AY - BP - LF - LT - MC - OO - OT - PJ - PR - SA - SR - U1 - U2 - X1 - X2 - X3 - X4 - X5 - X6 - X7 - X8 - X9");
if (ElementType.Length > 2)
{
log.Warning("Length > 2 - truncated");
ElementType = ElementType.Substring(0,2);
}
Set_Value ("ElementType", ElementType);
}
/** Get Type.
@return Element Type (account or user defined) */
public String GetElementType() 
{
return (String)Get_Value("ElementType");
}
/** Set Balanced.
@param IsBalanced Accounting Element needs to be balanced */
public void SetIsBalanced (Boolean IsBalanced)
{
Set_Value ("IsBalanced", IsBalanced);
}
/** Get Balanced.
@return Accounting Element needs to be balanced */
public Boolean IsBalanced() 
{
Object oo = Get_Value("IsBalanced");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}




/** Set Heavy Data.* @param IsHeavyData Heavy Data */
public void SetIsHeavyData(Boolean IsHeavyData) { Set_Value("IsHeavyData", IsHeavyData); }/** Get Heavy Data.
@return Heavy Data */
public Boolean IsHeavyData() { Object oo = Get_Value("IsHeavyData"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Mandatory.
                                                                                                                                                                                           * 
        




/** Set Mandatory.
@param IsMandatory Data is required in this column */
public void SetIsMandatory (Boolean IsMandatory)
{
Set_Value ("IsMandatory", IsMandatory);
}
/** Get Mandatory.
@return Data is required in this column */
public Boolean IsMandatory() 
{
Object oo = Get_Value("IsMandatory");
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
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_Value ("SeqNo", SeqNo);
}
/** Get Sequence.
@return Method of ordering elements;
 lowest number comes first */
public int GetSeqNo() 
{
Object ii = Get_Value("SeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
