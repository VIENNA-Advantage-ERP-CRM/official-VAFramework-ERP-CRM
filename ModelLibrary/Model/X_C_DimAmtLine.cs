namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for C_DimAmtLine
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_DimAmtLine : PO{public X_C_DimAmtLine (Context ctx, int C_DimAmtLine_ID, Trx trxName) : base (ctx, C_DimAmtLine_ID, trxName){/** if (C_DimAmtLine_ID == 0){SetAmount (0.0);SetC_DimAmtAcctType_ID (0);SetC_DimAmtLine_ID (0);SetC_DimAmt_ID (0);} */
}public X_C_DimAmtLine (Ctx ctx, int C_DimAmtLine_ID, Trx trxName) : base (ctx, C_DimAmtLine_ID, trxName){/** if (C_DimAmtLine_ID == 0){SetAmount (0.0);SetC_DimAmtAcctType_ID (0);SetC_DimAmtLine_ID (0);SetC_DimAmt_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_DimAmtLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_DimAmtLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_DimAmtLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_DimAmtLine(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27753729039653L;/** Last Updated Timestamp 8/19/2016 4:18:42 PM */
public static long updatedMS = 1471603722864L;/** AD_Table_ID=1000761 */
public static int Table_ID; // =1000761;
/** TableName=C_DimAmtLine */
public static String Table_Name="C_DimAmtLine";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
*/
protected override int Get_AccessLevel(){return Convert.ToInt32(accessLevel.ToString());}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Info
@return info
*/
public override String ToString(){StringBuilder sb = new StringBuilder ("X_C_DimAmtLine[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Column.
@param AD_Column_ID Column in the table */
public void SetAD_Column_ID (int AD_Column_ID){if (AD_Column_ID <= 0) Set_Value ("AD_Column_ID", null);else
Set_Value ("AD_Column_ID", AD_Column_ID);}/** Get Column.
@return Column in the table */
public int GetAD_Column_ID() {Object ii = Get_Value("AD_Column_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Amount.
@param Amount Amount in a defined currency */
public void SetAmount (Decimal? Amount){if (Amount == null) throw new ArgumentException ("Amount is mandatory.");Set_Value ("Amount", (Decimal?)Amount);}/** Get Amount.
@return Amount in a defined currency */
public Decimal GetAmount() {Object bd =Get_Value("Amount");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Activity.
@param C_Activity_ID Business Activity */
public void SetC_Activity_ID (int C_Activity_ID){if (C_Activity_ID <= 0) Set_Value ("C_Activity_ID", null);else
Set_Value ("C_Activity_ID", C_Activity_ID);}/** Get Activity.
@return Business Activity */
public int GetC_Activity_ID() {Object ii = Get_Value("C_Activity_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Business Partner.
@param C_BPartner_ID Identifies a Customer/Prospect */
public void SetC_BPartner_ID (int C_BPartner_ID){if (C_BPartner_ID <= 0) Set_Value ("C_BPartner_ID", null);else
Set_Value ("C_BPartner_ID", C_BPartner_ID);}/** Get Business Partner.
@return Identifies a Customer/Prospect */
public int GetC_BPartner_ID() {Object ii = Get_Value("C_BPartner_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Campaign.
@param C_Campaign_ID Marketing Campaign */
public void SetC_Campaign_ID (int C_Campaign_ID){if (C_Campaign_ID <= 0) Set_Value ("C_Campaign_ID", null);else
Set_Value ("C_Campaign_ID", C_Campaign_ID);}/** Get Campaign.
@return Marketing Campaign */
public int GetC_Campaign_ID() {Object ii = Get_Value("C_Campaign_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set C_DimAmtAcctType_ID.
@param C_DimAmtAcctType_ID C_DimAmtAcctType_ID */
public void SetC_DimAmtAcctType_ID (int C_DimAmtAcctType_ID){if (C_DimAmtAcctType_ID < 1) throw new ArgumentException ("C_DimAmtAcctType_ID is mandatory.");Set_Value ("C_DimAmtAcctType_ID", C_DimAmtAcctType_ID);}/** Get C_DimAmtAcctType_ID.
@return C_DimAmtAcctType_ID */
public int GetC_DimAmtAcctType_ID() {Object ii = Get_Value("C_DimAmtAcctType_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set C_DimAmtLine_ID.
@param C_DimAmtLine_ID C_DimAmtLine_ID */
public void SetC_DimAmtLine_ID (int C_DimAmtLine_ID){if (C_DimAmtLine_ID < 1) throw new ArgumentException ("C_DimAmtLine_ID is mandatory.");Set_ValueNoCheck ("C_DimAmtLine_ID", C_DimAmtLine_ID);}/** Get C_DimAmtLine_ID.
@return C_DimAmtLine_ID */
public int GetC_DimAmtLine_ID() {Object ii = Get_Value("C_DimAmtLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set C_DimAmt_ID.
@param C_DimAmt_ID C_DimAmt_ID */
public void SetC_DimAmt_ID (int C_DimAmt_ID){if (C_DimAmt_ID < 1) throw new ArgumentException ("C_DimAmt_ID is mandatory.");Set_Value ("C_DimAmt_ID", C_DimAmt_ID);}/** Get C_DimAmt_ID.
@return C_DimAmt_ID */
public int GetC_DimAmt_ID() {Object ii = Get_Value("C_DimAmt_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Account Element.
@param C_ElementValue_ID Account Element */
public void SetC_ElementValue_ID (int C_ElementValue_ID){if (C_ElementValue_ID <= 0) Set_Value ("C_ElementValue_ID", null);else
Set_Value ("C_ElementValue_ID", C_ElementValue_ID);}/** Get Account Element.
@return Account Element */
public int GetC_ElementValue_ID() {Object ii = Get_Value("C_ElementValue_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Element.
@param C_Element_ID Accounting Element */
public void SetC_Element_ID (int C_Element_ID){if (C_Element_ID <= 0) Set_Value ("C_Element_ID", null);else
Set_Value ("C_Element_ID", C_Element_ID);}/** Get Element.
@return Accounting Element */
public int GetC_Element_ID() {Object ii = Get_Value("C_Element_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Address.
@param C_Location_ID Location or Address */
public void SetC_Location_ID (int C_Location_ID){if (C_Location_ID <= 0) Set_Value ("C_Location_ID", null);else
Set_Value ("C_Location_ID", C_Location_ID);}/** Get Address.
@return Location or Address */
public int GetC_Location_ID() {Object ii = Get_Value("C_Location_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Opportunity.
@param C_Project_ID Business Opportunity */
public void SetC_Project_ID (int C_Project_ID){if (C_Project_ID <= 0) Set_Value ("C_Project_ID", null);else
Set_Value ("C_Project_ID", C_Project_ID);}/** Get Opportunity.
@return Business Opportunity */
public int GetC_Project_ID() {Object ii = Get_Value("C_Project_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Sales Region.
@param C_SalesRegion_ID Sales coverage region */
public void SetC_SalesRegion_ID (int C_SalesRegion_ID){if (C_SalesRegion_ID <= 0) Set_Value ("C_SalesRegion_ID", null);else
Set_Value ("C_SalesRegion_ID", C_SalesRegion_ID);}/** Get Sales Region.
@return Sales coverage region */
public int GetC_SalesRegion_ID() {Object ii = Get_Value("C_SalesRegion_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID){if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);else
Set_Value ("M_Product_ID", M_Product_ID);}/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() {Object ii = Get_Value("M_Product_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Organization.
@param Org_ID Organizational entity within client */
public void SetOrg_ID (int Org_ID){if (Org_ID <= 0) Set_Value ("Org_ID", null);else
Set_Value ("Org_ID", Org_ID);}/** Get Organization.
@return Organizational entity within client */
public int GetOrg_ID() {Object ii = Get_Value("Org_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}}
}