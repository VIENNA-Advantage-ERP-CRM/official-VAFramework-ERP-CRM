namespace ViennaAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for M_CostAllocationSetting
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_CostAllocationSetting : PO{public X_M_CostAllocationSetting (Context ctx, int M_CostAllocationSetting_ID, Trx trxName) : base (ctx, M_CostAllocationSetting_ID, trxName){/** if (M_CostAllocationSetting_ID == 0){SetAllocationType (null);// CD
SetC_DocType_ID (0);SetDocStatus (null);// CO
SetInvRef_DocType_ID (0);SetM_CostAllocationSetting_ID (0);SetName (null);SetRef_DocType_ID (0);} */
}public X_M_CostAllocationSetting (Ctx ctx, int M_CostAllocationSetting_ID, Trx trxName) : base (ctx, M_CostAllocationSetting_ID, trxName){/** if (M_CostAllocationSetting_ID == 0){SetAllocationType (null);// CD
SetC_DocType_ID (0);SetDocStatus (null);// CO
SetInvRef_DocType_ID (0);SetM_CostAllocationSetting_ID (0);SetName (null);SetRef_DocType_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_CostAllocationSetting (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_CostAllocationSetting (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_CostAllocationSetting (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_CostAllocationSetting(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27751660379476L;/** Last Updated Timestamp 7/26/2016 5:41:02 PM */
public static long updatedMS = 1469535062687L;/** AD_Table_ID=1000750 */
public static int Table_ID; // =1000750;
/** TableName=M_CostAllocationSetting */
public static String Table_Name="M_CostAllocationSetting";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_M_CostAllocationSetting[").Append(Get_ID()).Append("]");return sb.ToString();}
/** AllocationType AD_Reference_ID=1000323 */
public static int ALLOCATIONTYPE_AD_Reference_ID=1000323;/** All = AL */
public static String ALLOCATIONTYPE_All = "AL";/** Currency Difference & Discount = CA */
public static String ALLOCATIONTYPE_CurrencyDifferenceDiscount = "CA";/** Currency Difference = CD */
public static String ALLOCATIONTYPE_CurrencyDifference = "CD";/** Currency Difference & Write-Off = CW */
public static String ALLOCATIONTYPE_CurrencyDifferenceWrite_Off = "CW";/** Discount = DI */
public static String ALLOCATIONTYPE_Discount = "DI";/** Discount & Write-Off = DW */
public static String ALLOCATIONTYPE_DiscountWrite_Off = "DW";/** Write-Off = WO */
public static String ALLOCATIONTYPE_Write_Off = "WO";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAllocationTypeValid (String test){return test.Equals("AL") || test.Equals("CA") || test.Equals("CD") || test.Equals("CW") || test.Equals("DI") || test.Equals("DW") || test.Equals("WO");}/** Set Allocation Type.
@param AllocationType Allocation Type */
public void SetAllocationType (String AllocationType){if (AllocationType == null) throw new ArgumentException ("AllocationType is mandatory");if (!IsAllocationTypeValid(AllocationType))
throw new ArgumentException ("AllocationType Invalid value - " + AllocationType + " - Reference_ID=1000323 - AL - CA - CD - CW - DI - DW - WO");if (AllocationType.Length > 2){log.Warning("Length > 2 - truncated");AllocationType = AllocationType.Substring(0,2);}Set_Value ("AllocationType", AllocationType);}/** Get Allocation Type.
@return Allocation Type */
public String GetAllocationType() {return (String)Get_Value("AllocationType");}/** Set Document Type.
@param C_DocType_ID Document type or rules */
public void SetC_DocType_ID (int C_DocType_ID){if (C_DocType_ID < 0) throw new ArgumentException ("C_DocType_ID is mandatory.");Set_Value ("C_DocType_ID", C_DocType_ID);}/** Get Document Type.
@return Document type or rules */
public int GetC_DocType_ID() {Object ii = Get_Value("C_DocType_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description){if (Description != null && Description.Length > 225){log.Warning("Length > 225 - truncated");Description = Description.Substring(0,225);}Set_Value ("Description", Description);}/** Get Description.
@return Optional short description of the record */
public String GetDescription() {return (String)Get_Value("Description");}
/** DocStatus AD_Reference_ID=131 */
public static int DOCSTATUS_AD_Reference_ID=131;/** Unknown = ?? */
public static String DOCSTATUS_Unknown = "??";/** Approved = AP */
public static String DOCSTATUS_Approved = "AP";/** Closed = CL */
public static String DOCSTATUS_Closed = "CL";/** Completed = CO */
public static String DOCSTATUS_Completed = "CO";/** Drafted = DR */
public static String DOCSTATUS_Drafted = "DR";/** Invalid = IN */
public static String DOCSTATUS_Invalid = "IN";/** In Progress = IP */
public static String DOCSTATUS_InProgress = "IP";/** Not Approved = NA */
public static String DOCSTATUS_NotApproved = "NA";/** Reversed = RE */
public static String DOCSTATUS_Reversed = "RE";/** Voided = VO */
public static String DOCSTATUS_Voided = "VO";/** Waiting Confirmation = WC */
public static String DOCSTATUS_WaitingConfirmation = "WC";/** Waiting Payment = WP */
public static String DOCSTATUS_WaitingPayment = "WP";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDocStatusValid (String test){return test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP");}/** Set Document Status.
@param DocStatus The current status of the document */
public void SetDocStatus (String DocStatus){if (DocStatus == null) throw new ArgumentException ("DocStatus is mandatory");if (!IsDocStatusValid(DocStatus))
throw new ArgumentException ("DocStatus Invalid value - " + DocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP");if (DocStatus.Length > 2){log.Warning("Length > 2 - truncated");DocStatus = DocStatus.Substring(0,2);}Set_Value ("DocStatus", DocStatus);}/** Get Document Status.
@return The current status of the document */
public String GetDocStatus() {return (String)Get_Value("DocStatus");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}
/** InvRef_DocType_ID AD_Reference_ID=1000324 */
public static int INVREF_DOCTYPE_ID_AD_Reference_ID=1000324;/** Set Invoice Document Type.
@param InvRef_DocType_ID Invoice Document Type */
public void SetInvRef_DocType_ID (int InvRef_DocType_ID){if (InvRef_DocType_ID < 1) throw new ArgumentException ("InvRef_DocType_ID is mandatory.");Set_Value ("InvRef_DocType_ID", InvRef_DocType_ID);}/** Get Invoice Document Type.
@return Invoice Document Type */
public int GetInvRef_DocType_ID() {Object ii = Get_Value("InvRef_DocType_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set M_CostAllocationSetting_ID.
@param M_CostAllocationSetting_ID M_CostAllocationSetting_ID */
public void SetM_CostAllocationSetting_ID (int M_CostAllocationSetting_ID){if (M_CostAllocationSetting_ID < 1) throw new ArgumentException ("M_CostAllocationSetting_ID is mandatory.");Set_ValueNoCheck ("M_CostAllocationSetting_ID", M_CostAllocationSetting_ID);}/** Get M_CostAllocationSetting_ID.
@return M_CostAllocationSetting_ID */
public int GetM_CostAllocationSetting_ID() {Object ii = Get_Value("M_CostAllocationSetting_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name){if (Name == null) throw new ArgumentException ("Name is mandatory.");if (Name.Length > 100){log.Warning("Length > 100 - truncated");Name = Name.Substring(0,100);}Set_Value ("Name", Name);}/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() {return (String)Get_Value("Name");}
/** Ref_DocType_ID AD_Reference_ID=1000324 */
public static int REF_DOCTYPE_ID_AD_Reference_ID=1000324;/** Set Allocation Document Type.
@param Ref_DocType_ID Allocation Document Type */
public void SetRef_DocType_ID (int Ref_DocType_ID){if (Ref_DocType_ID < 1) throw new ArgumentException ("Ref_DocType_ID is mandatory.");Set_Value ("Ref_DocType_ID", Ref_DocType_ID);}/** Get Allocation Document Type.
@return Allocation Document Type */
public int GetRef_DocType_ID() {Object ii = Get_Value("Ref_DocType_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}}
}