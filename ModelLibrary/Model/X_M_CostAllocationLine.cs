namespace ViennaAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for M_CostAllocationLine
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_CostAllocationLine : PO{public X_M_CostAllocationLine (Context ctx, int M_CostAllocationLine_ID, Trx trxName) : base (ctx, M_CostAllocationLine_ID, trxName){/** if (M_CostAllocationLine_ID == 0){SetAllocationType (null);SetAmount (0.0);SetC_InvoiceLine_ID (0);SetM_CostAllocationLine_ID (0);SetM_CostAllocation_ID (0);SetM_Product_ID (0);} */
}public X_M_CostAllocationLine (Ctx ctx, int M_CostAllocationLine_ID, Trx trxName) : base (ctx, M_CostAllocationLine_ID, trxName){/** if (M_CostAllocationLine_ID == 0){SetAllocationType (null);SetAmount (0.0);SetC_InvoiceLine_ID (0);SetM_CostAllocationLine_ID (0);SetM_CostAllocation_ID (0);SetM_Product_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_CostAllocationLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_CostAllocationLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_CostAllocationLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_CostAllocationLine(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27751641102143L;/** Last Updated Timestamp 7/26/2016 12:19:45 PM */
public static long updatedMS = 1469515785354L;/** AD_Table_ID=1000753 */
public static int Table_ID; // =1000753;
/** TableName=M_CostAllocationLine */
public static String Table_Name="M_CostAllocationLine";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_M_CostAllocationLine[").Append(Get_ID()).Append("]");return sb.ToString();}
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
public String GetAllocationType() {return (String)Get_Value("AllocationType");}/** Set Amount.
@param Amount Amount in a defined currency */
public void SetAmount (Decimal? Amount){if (Amount == null) throw new ArgumentException ("Amount is mandatory.");Set_Value ("Amount", (Decimal?)Amount);}/** Get Amount.
@return Amount in a defined currency */
public Decimal GetAmount() {Object bd =Get_Value("Amount");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Invoice Line.
@param C_InvoiceLine_ID Invoice Detail Line */
public void SetC_InvoiceLine_ID (int C_InvoiceLine_ID){if (C_InvoiceLine_ID < 1) throw new ArgumentException ("C_InvoiceLine_ID is mandatory.");Set_Value ("C_InvoiceLine_ID", C_InvoiceLine_ID);}/** Get Invoice Line.
@return Invoice Detail Line */
public int GetC_InvoiceLine_ID() {Object ii = Get_Value("C_InvoiceLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description){if (Description != null && Description.Length > 255){log.Warning("Length > 255 - truncated");Description = Description.Substring(0,255);}Set_Value ("Description", Description);}/** Get Description.
@return Optional short description of the record */
public String GetDescription() {return (String)Get_Value("Description");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set IsProcessed.
@param IsProcessed IsProcessed */
public void SetIsProcessed (Boolean IsProcessed){Set_Value ("IsProcessed", IsProcessed);}/** Get IsProcessed.
@return IsProcessed */
public Boolean IsProcessed() {Object oo = Get_Value("IsProcessed");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set M_CostAllocationLine_ID.
@param M_CostAllocationLine_ID M_CostAllocationLine_ID */
public void SetM_CostAllocationLine_ID (int M_CostAllocationLine_ID){if (M_CostAllocationLine_ID < 1) throw new ArgumentException ("M_CostAllocationLine_ID is mandatory.");Set_ValueNoCheck ("M_CostAllocationLine_ID", M_CostAllocationLine_ID);}/** Get M_CostAllocationLine_ID.
@return M_CostAllocationLine_ID */
public int GetM_CostAllocationLine_ID() {Object ii = Get_Value("M_CostAllocationLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Cost Allocation.
@param M_CostAllocation_ID Cost Allocation */
public void SetM_CostAllocation_ID (int M_CostAllocation_ID){if (M_CostAllocation_ID < 1) throw new ArgumentException ("M_CostAllocation_ID is mandatory.");Set_Value ("M_CostAllocation_ID", M_CostAllocation_ID);}/** Get Cost Allocation.
@return Cost Allocation */
public int GetM_CostAllocation_ID() {Object ii = Get_Value("M_CostAllocation_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID){if (M_Product_ID < 1) throw new ArgumentException ("M_Product_ID is mandatory.");Set_Value ("M_Product_ID", M_Product_ID);}/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() {Object ii = Get_Value("M_Product_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}}
}