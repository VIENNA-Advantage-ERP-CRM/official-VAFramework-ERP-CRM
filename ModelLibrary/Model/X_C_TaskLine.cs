namespace VAdvantage.Model
{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for C_TaskLine
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_TaskLine : PO{public X_C_TaskLine (Context ctx, int C_TaskLine_ID, Trx trxName) : base (ctx, C_TaskLine_ID, trxName){/** if (C_TaskLine_ID == 0){SetC_TaskLine_ID (0);SetC_Task_ID (0);SetName (null);SetSeqNo (0);// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM C_TaskLine WHERE C_Task_ID=@C_Task_ID@
SetStandardQty (0.0);// 1
} */
}public X_C_TaskLine (Ctx ctx, int C_TaskLine_ID, Trx trxName) : base (ctx, C_TaskLine_ID, trxName){/** if (C_TaskLine_ID == 0){SetC_TaskLine_ID (0);SetC_Task_ID (0);SetName (null);SetSeqNo (0);// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM C_TaskLine WHERE C_Task_ID=@C_Task_ID@
SetStandardQty (0.0);// 1
} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TaskLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TaskLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TaskLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_TaskLine(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27897050882476L;/** Last Updated Timestamp 3/5/2021 6:26:05 AM */
public static long updatedMS = 1614925565687L;/** AD_Table_ID=1000548 */
public static int Table_ID; // =1000548;
/** TableName=C_TaskLine */
public static String Table_Name="C_TaskLine";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_C_TaskLine[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set C_TaskLine_ID.
@param C_TaskLine_ID C_TaskLine_ID */
public void SetC_TaskLine_ID (int C_TaskLine_ID){if (C_TaskLine_ID < 1) throw new ArgumentException ("C_TaskLine_ID is mandatory.");Set_ValueNoCheck ("C_TaskLine_ID", C_TaskLine_ID);}/** Get C_TaskLine_ID.
@return C_TaskLine_ID */
public int GetC_TaskLine_ID() {Object ii = Get_Value("C_TaskLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Standard Task.
@param C_Task_ID Standard Project Type Task */
public void SetC_Task_ID (int C_Task_ID){if (C_Task_ID < 1) throw new ArgumentException ("C_Task_ID is mandatory.");Set_ValueNoCheck ("C_Task_ID", C_Task_ID);}/** Get Standard Task.
@return Standard Project Type Task */
public int GetC_Task_ID() {Object ii = Get_Value("C_Task_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description){if (Description != null && Description.Length > 255){log.Warning("Length > 255 - truncated");Description = Description.Substring(0,255);}Set_Value ("Description", Description);}/** Get Description.
@return Optional short description of the record */
public String GetDescription() {return (String)Get_Value("Description");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help){if (Help != null && Help.Length > 2000){log.Warning("Length > 2000 - truncated");Help = Help.Substring(0,2000);}Set_Value ("Help", Help);}/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() {return (String)Get_Value("Help");}/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID){if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);else
Set_Value ("M_Product_ID", M_Product_ID);}/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() {Object ii = Get_Value("M_Product_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name){if (Name == null) throw new ArgumentException ("Name is mandatory.");if (Name.Length > 60){log.Warning("Length > 60 - truncated");Name = Name.Substring(0,60);}Set_Value ("Name", Name);}/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() {return (String)Get_Value("Name");}/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() {return new KeyNamePair(Get_ID(), GetName());}/** Set Sequence.
@param SeqNo Method of ordering elements; lowest number comes first */
public void SetSeqNo (int SeqNo){Set_Value ("SeqNo", SeqNo);}/** Get Sequence.
@return Method of ordering elements; lowest number comes first */
public int GetSeqNo() {Object ii = Get_Value("SeqNo");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Standard Quantity.
@param StandardQty Standard Quantity */
public void SetStandardQty (Decimal? StandardQty){if (StandardQty == null) throw new ArgumentException ("StandardQty is mandatory.");Set_Value ("StandardQty", (Decimal?)StandardQty);}/** Get Standard Quantity.
@return Standard Quantity */
public Decimal GetStandardQty() {Object bd =Get_Value("StandardQty");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}}
}