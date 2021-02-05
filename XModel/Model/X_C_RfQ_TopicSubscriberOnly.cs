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
/** Generated Model for VAB_RFQ_SubjectMem_Allow
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_RFQ_SubjectMem_Allow : PO
{
public X_VAB_RFQ_SubjectMem_Allow (Context ctx, int VAB_RFQ_SubjectMem_Allow_ID, Trx trxName) : base (ctx, VAB_RFQ_SubjectMem_Allow_ID, trxName)
{
/** if (VAB_RFQ_SubjectMem_Allow_ID == 0)
{
SetVAB_RFQ_SubjectMem_Allow_ID (0);
SetVAB_RFQ_SubjectMember_ID (0);
}
 */
}
public X_VAB_RFQ_SubjectMem_Allow (Ctx ctx, int VAB_RFQ_SubjectMem_Allow_ID, Trx trxName) : base (ctx, VAB_RFQ_SubjectMem_Allow_ID, trxName)
{
/** if (VAB_RFQ_SubjectMem_Allow_ID == 0)
{
SetVAB_RFQ_SubjectMem_Allow_ID (0);
SetVAB_RFQ_SubjectMember_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_RFQ_SubjectMem_Allow (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_RFQ_SubjectMem_Allow (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_RFQ_SubjectMem_Allow (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_RFQ_SubjectMem_Allow()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375087L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058298L;
/** VAF_TableView_ID=747 */
public static int Table_ID;
 // =747;

/** TableName=VAB_RFQ_SubjectMem_Allow */
public static String Table_Name="VAB_RFQ_SubjectMem_Allow";

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
StringBuilder sb = new StringBuilder ("X_VAB_RFQ_SubjectMem_Allow[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set RfQ Topic Subscriber Restriction.
@param VAB_RFQ_SubjectMem_Allow_ID Include Subscriber only for certain products or product categories */
public void SetVAB_RFQ_SubjectMem_Allow_ID (int VAB_RFQ_SubjectMem_Allow_ID)
{
if (VAB_RFQ_SubjectMem_Allow_ID < 1) throw new ArgumentException ("VAB_RFQ_SubjectMem_Allow_ID is mandatory.");
Set_ValueNoCheck ("VAB_RFQ_SubjectMem_Allow_ID", VAB_RFQ_SubjectMem_Allow_ID);
}
/** Get RfQ Topic Subscriber Restriction.
@return Include Subscriber only for certain products or product categories */
public int GetVAB_RFQ_SubjectMem_Allow_ID() 
{
Object ii = Get_Value("VAB_RFQ_SubjectMem_Allow_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set RfQ Subscriber.
@param VAB_RFQ_SubjectMember_ID Request for Quotation Topic Subscriber */
public void SetVAB_RFQ_SubjectMember_ID (int VAB_RFQ_SubjectMember_ID)
{
if (VAB_RFQ_SubjectMember_ID < 1) throw new ArgumentException ("VAB_RFQ_SubjectMember_ID is mandatory.");
Set_ValueNoCheck ("VAB_RFQ_SubjectMember_ID", VAB_RFQ_SubjectMember_ID);
}
/** Get RfQ Subscriber.
@return Request for Quotation Topic Subscriber */
public int GetVAB_RFQ_SubjectMember_ID() 
{
Object ii = Get_Value("VAB_RFQ_SubjectMember_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Product Category.
@param VAM_ProductCategory_ID Category of a Product */
public void SetVAM_ProductCategory_ID (int VAM_ProductCategory_ID)
{
if (VAM_ProductCategory_ID <= 0) Set_Value ("VAM_ProductCategory_ID", null);
else
Set_Value ("VAM_ProductCategory_ID", VAM_ProductCategory_ID);
}
/** Get Product Category.
@return Category of a Product */
public int GetVAM_ProductCategory_ID() 
{
Object ii = Get_Value("VAM_ProductCategory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAM_ProductCategory_ID().ToString());
}
/** Set Product.
@param VAM_Product_ID Product, Service, Item */
public void SetVAM_Product_ID (int VAM_Product_ID)
{
if (VAM_Product_ID <= 0) Set_Value ("VAM_Product_ID", null);
else
Set_Value ("VAM_Product_ID", VAM_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetVAM_Product_ID() 
{
Object ii = Get_Value("VAM_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
