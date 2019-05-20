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
/** Generated Model for M_ProductDownload
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_ProductDownload : PO
{
public X_M_ProductDownload (Context ctx, int M_ProductDownload_ID, Trx trxName) : base (ctx, M_ProductDownload_ID, trxName)
{
/** if (M_ProductDownload_ID == 0)
{
SetDownloadURL (null);
SetM_ProductDownload_ID (0);
SetM_Product_ID (0);
SetName (null);
}
 */
}
public X_M_ProductDownload (Ctx ctx, int M_ProductDownload_ID, Trx trxName) : base (ctx, M_ProductDownload_ID, trxName)
{
/** if (M_ProductDownload_ID == 0)
{
SetDownloadURL (null);
SetM_ProductDownload_ID (0);
SetM_Product_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ProductDownload (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ProductDownload (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ProductDownload (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_ProductDownload()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514380509L;
/** Last Updated Timestamp 7/29/2010 1:07:43 PM */
public static long updatedMS = 1280389063720L;
/** AD_Table_ID=777 */
public static int Table_ID;
 // =777;

/** TableName=M_ProductDownload */
public static String Table_Name="M_ProductDownload";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_M_ProductDownload[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Download URL.
@param DownloadURL URL of the Download files */
public void SetDownloadURL (String DownloadURL)
{
if (DownloadURL == null) throw new ArgumentException ("DownloadURL is mandatory.");
if (DownloadURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
DownloadURL = DownloadURL.Substring(0,120);
}
Set_Value ("DownloadURL", DownloadURL);
}
/** Get Download URL.
@return URL of the Download files */
public String GetDownloadURL() 
{
return (String)Get_Value("DownloadURL");
}
/** Set Lead Download.
@param IsLeadDownload Download can be used for Lead Generation */
public void SetIsLeadDownload (Boolean IsLeadDownload)
{
Set_Value ("IsLeadDownload", IsLeadDownload);
}
/** Get Lead Download.
@return Download can be used for Lead Generation */
public Boolean IsLeadDownload() 
{
Object oo = Get_Value("IsLeadDownload");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Product Download.
@param M_ProductDownload_ID Product downloads */
public void SetM_ProductDownload_ID (int M_ProductDownload_ID)
{
if (M_ProductDownload_ID < 1) throw new ArgumentException ("M_ProductDownload_ID is mandatory.");
Set_ValueNoCheck ("M_ProductDownload_ID", M_ProductDownload_ID);
}
/** Get Product Download.
@return Product downloads */
public int GetM_ProductDownload_ID() 
{
Object ii = Get_Value("M_ProductDownload_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID < 1) throw new ArgumentException ("M_Product_ID is mandatory.");
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
}

}
