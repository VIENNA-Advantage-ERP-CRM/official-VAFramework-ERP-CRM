namespace VAdvantage.Model{
//namespace ViennaAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for M_ProductAttributeImage
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_ProductAttributeImage : PO{public X_M_ProductAttributeImage (Context ctx, int M_ProductAttributeImage_ID, Trx trxName) : base (ctx, M_ProductAttributeImage_ID, trxName){/** if (M_ProductAttributeImage_ID == 0){SetM_AttributeValue_ID (0);SetM_ProductAttributeImage_ID (0);SetM_Product_ID (0);} */
}public X_M_ProductAttributeImage (Ctx ctx, int M_ProductAttributeImage_ID, Trx trxName) : base (ctx, M_ProductAttributeImage_ID, trxName){/** if (M_ProductAttributeImage_ID == 0){SetM_AttributeValue_ID (0);SetM_ProductAttributeImage_ID (0);SetM_Product_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ProductAttributeImage (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ProductAttributeImage (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ProductAttributeImage (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_ProductAttributeImage(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27757178044507L;/** Last Updated Timestamp 9/28/2016 2:22:07 PM */
public static long updatedMS = 1475052727718L;/** AD_Table_ID=1001001 */
public static int Table_ID; // =1001001;
/** TableName=M_ProductAttributeImage */
public static String Table_Name="M_ProductAttributeImage";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(7);/** AccessLevel
@return 7 - System - Client - Org 
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_M_ProductAttributeImage[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Image.
@param AD_Image_ID Image or Icon */
public void SetAD_Image_ID (int AD_Image_ID){if (AD_Image_ID <= 0) Set_Value ("AD_Image_ID", null);else
Set_Value ("AD_Image_ID", AD_Image_ID);}/** Get Image.
@return Image or Icon */
public int GetAD_Image_ID() {Object ii = Get_Value("AD_Image_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Attribute Value.
@param M_AttributeValue_ID Product Attribute Value */
public void SetM_AttributeValue_ID (int M_AttributeValue_ID){if (M_AttributeValue_ID < 1) throw new ArgumentException ("M_AttributeValue_ID is mandatory.");Set_ValueNoCheck ("M_AttributeValue_ID", M_AttributeValue_ID);}/** Get Attribute Value.
@return Product Attribute Value */
public int GetM_AttributeValue_ID() {Object ii = Get_Value("M_AttributeValue_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set M_ProductAttributeImage_ID.
@param M_ProductAttributeImage_ID M_ProductAttributeImage_ID */
public void SetM_ProductAttributeImage_ID (int M_ProductAttributeImage_ID){if (M_ProductAttributeImage_ID < 1) throw new ArgumentException ("M_ProductAttributeImage_ID is mandatory.");Set_ValueNoCheck ("M_ProductAttributeImage_ID", M_ProductAttributeImage_ID);}/** Get M_ProductAttributeImage_ID.
@return M_ProductAttributeImage_ID */
public int GetM_ProductAttributeImage_ID() {Object ii = Get_Value("M_ProductAttributeImage_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID){if (M_Product_ID < 1) throw new ArgumentException ("M_Product_ID is mandatory.");Set_ValueNoCheck ("M_Product_ID", M_Product_ID);}/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() {Object ii = Get_Value("M_Product_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}}
}