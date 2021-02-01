namespace ViennaAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for VAB_DimAmtAcctType
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_DimAmtAcctType : PO{public X_VAB_DimAmtAcctType (Context ctx, int VAB_DimAmtAcctType_ID, Trx trxName) : base (ctx, VAB_DimAmtAcctType_ID, trxName){/** if (VAB_DimAmtAcctType_ID == 0){SetVAB_AccountBook_ID (0);SetVAB_DimAmtAcctType_ID (0);SetVAB_DimAmt_ID (0);SetElementType (null);} */
}public X_VAB_DimAmtAcctType (Ctx ctx, int VAB_DimAmtAcctType_ID, Trx trxName) : base (ctx, VAB_DimAmtAcctType_ID, trxName){/** if (VAB_DimAmtAcctType_ID == 0){SetVAB_AccountBook_ID (0);SetVAB_DimAmtAcctType_ID (0);SetVAB_DimAmt_ID (0);SetElementType (null);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_DimAmtAcctType (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_DimAmtAcctType (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_DimAmtAcctType (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_DimAmtAcctType(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27755354003673L;/** Last Updated Timestamp 9/7/2016 11:41:26 AM */
public static long updatedMS = 1473228686884L;/** VAF_TableView_ID=1000760 */
public static int Table_ID; // =1000760;
/** TableName=VAB_DimAmtAcctType */
public static String Table_Name="VAB_DimAmtAcctType";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_VAB_DimAmtAcctType[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Accounting Schema.
@param VAB_AccountBook_ID Rules for accounting */
public void SetVAB_AccountBook_ID (int VAB_AccountBook_ID){if (VAB_AccountBook_ID < 1) throw new ArgumentException ("VAB_AccountBook_ID is mandatory.");Set_Value ("VAB_AccountBook_ID", VAB_AccountBook_ID);}/** Get Accounting Schema.
@return Rules for accounting */
public int GetVAB_AccountBook_ID() {Object ii = Get_Value("VAB_AccountBook_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set VAB_DimAmtAcctType_ID.
@param VAB_DimAmtAcctType_ID VAB_DimAmtAcctType_ID */
public void SetVAB_DimAmtAcctType_ID (int VAB_DimAmtAcctType_ID){if (VAB_DimAmtAcctType_ID < 1) throw new ArgumentException ("VAB_DimAmtAcctType_ID is mandatory.");Set_ValueNoCheck ("VAB_DimAmtAcctType_ID", VAB_DimAmtAcctType_ID);}/** Get VAB_DimAmtAcctType_ID.
@return VAB_DimAmtAcctType_ID */
public int GetVAB_DimAmtAcctType_ID() {Object ii = Get_Value("VAB_DimAmtAcctType_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set VAB_DimAmt_ID.
@param VAB_DimAmt_ID VAB_DimAmt_ID */
public void SetVAB_DimAmt_ID (int VAB_DimAmt_ID){if (VAB_DimAmt_ID < 1) throw new ArgumentException ("VAB_DimAmt_ID is mandatory.");Set_Value ("VAB_DimAmt_ID", VAB_DimAmt_ID);}/** Get VAB_DimAmt_ID.
@return VAB_DimAmt_ID */
public int GetVAB_DimAmt_ID() {Object ii = Get_Value("VAB_DimAmt_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Type.
@param ElementType Element Type (account or user defined) */
public void SetElementType (String ElementType){if (ElementType == null) throw new ArgumentException ("ElementType is mandatory.");if (ElementType.Length > 2){log.Warning("Length > 2 - truncated");ElementType = ElementType.Substring(0,2);}Set_Value ("ElementType", ElementType);}/** Get Type.
@return Element Type (account or user defined) */
public String GetElementType() {return (String)Get_Value("ElementType");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set TotalDimLineAmout.
@param TotalDimLineAmout TotalDimLineAmout */
public void SetTotalDimLineAmout (Decimal? TotalDimLineAmout){Set_Value ("TotalDimLineAmout", (Decimal?)TotalDimLineAmout);}/** Get TotalDimLineAmout.
@return TotalDimLineAmout */
public Decimal GetTotalDimLineAmout() {Object bd =Get_Value("TotalDimLineAmout");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}}
}