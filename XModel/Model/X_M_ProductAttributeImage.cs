namespace VAdvantage.Model{
//namespace ViennaAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for VAM_ProdcutFeatureImage
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_ProdcutFeatureImage : PO{public X_VAM_ProdcutFeatureImage (Context ctx, int VAM_ProdcutFeatureImage_ID, Trx trxName) : base (ctx, VAM_ProdcutFeatureImage_ID, trxName){/** if (VAM_ProdcutFeatureImage_ID == 0){SetVAM_PFeature_Value_ID (0);SetVAM_ProdcutFeatureImage_ID (0);SetVAM_Product_ID (0);} */
}public X_VAM_ProdcutFeatureImage (Ctx ctx, int VAM_ProdcutFeatureImage_ID, Trx trxName) : base (ctx, VAM_ProdcutFeatureImage_ID, trxName){/** if (VAM_ProdcutFeatureImage_ID == 0){SetVAM_PFeature_Value_ID (0);SetVAM_ProdcutFeatureImage_ID (0);SetVAM_Product_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ProdcutFeatureImage (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ProdcutFeatureImage (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ProdcutFeatureImage (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_ProdcutFeatureImage(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27757178044507L;/** Last Updated Timestamp 9/28/2016 2:22:07 PM */
public static long updatedMS = 1475052727718L;/** VAF_TableView_ID=1001001 */
public static int Table_ID; // =1001001;
/** TableName=VAM_ProdcutFeatureImage */
public static String Table_Name="VAM_ProdcutFeatureImage";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_VAM_ProdcutFeatureImage[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Image.
@param VAF_Image_ID Image or Icon */
public void SetVAF_Image_ID (int VAF_Image_ID){if (VAF_Image_ID <= 0) Set_Value ("VAF_Image_ID", null);else
Set_Value ("VAF_Image_ID", VAF_Image_ID);}/** Get Image.
@return Image or Icon */
public int GetVAF_Image_ID() {Object ii = Get_Value("VAF_Image_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Attribute Value.
@param VAM_PFeature_Value_ID Product Attribute Value */
public void SetVAM_PFeature_Value_ID (int VAM_PFeature_Value_ID){if (VAM_PFeature_Value_ID < 1) throw new ArgumentException ("VAM_PFeature_Value_ID is mandatory.");Set_ValueNoCheck ("VAM_PFeature_Value_ID", VAM_PFeature_Value_ID);}/** Get Attribute Value.
@return Product Attribute Value */
public int GetVAM_PFeature_Value_ID() {Object ii = Get_Value("VAM_PFeature_Value_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set VAM_ProdcutFeatureImage_ID.
@param VAM_ProdcutFeatureImage_ID VAM_ProdcutFeatureImage_ID */
public void SetVAM_ProdcutFeatureImage_ID (int VAM_ProdcutFeatureImage_ID){if (VAM_ProdcutFeatureImage_ID < 1) throw new ArgumentException ("VAM_ProdcutFeatureImage_ID is mandatory.");Set_ValueNoCheck ("VAM_ProdcutFeatureImage_ID", VAM_ProdcutFeatureImage_ID);}/** Get VAM_ProdcutFeatureImage_ID.
@return VAM_ProdcutFeatureImage_ID */
public int GetVAM_ProdcutFeatureImage_ID() {Object ii = Get_Value("VAM_ProdcutFeatureImage_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Product.
@param VAM_Product_ID Product, Service, Item */
public void SetVAM_Product_ID (int VAM_Product_ID){if (VAM_Product_ID < 1) throw new ArgumentException ("VAM_Product_ID is mandatory.");Set_ValueNoCheck ("VAM_Product_ID", VAM_Product_ID);}/** Get Product.
@return Product, Service, Item */
public int GetVAM_Product_ID() {Object ii = Get_Value("VAM_Product_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}}
}