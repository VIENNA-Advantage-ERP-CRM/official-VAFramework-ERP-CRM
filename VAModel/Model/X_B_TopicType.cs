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
/** Generated Model for B_TopicType
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_B_TopicType : PO
{
public X_B_TopicType (Context ctx, int B_TopicType_ID, Trx trxName) : base (ctx, B_TopicType_ID, trxName)
{
/** if (B_TopicType_ID == 0)
{
SetAuctionType (null);
SetB_TopicType_ID (0);
SetM_PriceList_ID (0);
SetM_ProductMember_ID (0);
SetM_Product_ID (0);
SetName (null);
}
 */
}
public X_B_TopicType (Ctx ctx, int B_TopicType_ID, Trx trxName) : base (ctx, B_TopicType_ID, trxName)
{
/** if (B_TopicType_ID == 0)
{
SetAuctionType (null);
SetB_TopicType_ID (0);
SetM_PriceList_ID (0);
SetM_ProductMember_ID (0);
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
public X_B_TopicType (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_B_TopicType (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_B_TopicType (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_B_TopicType()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514367579L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050790L;
/** AD_Table_ID=690 */
public static int Table_ID;
 // =690;

/** TableName=B_TopicType */
public static String Table_Name="B_TopicType";

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
StringBuilder sb = new StringBuilder ("X_B_TopicType[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Auction Type.
@param AuctionType Auction Type */
public void SetAuctionType (String AuctionType)
{
if (AuctionType == null) throw new ArgumentException ("AuctionType is mandatory.");
if (AuctionType.Length > 1)
{
log.Warning("Length > 1 - truncated");
AuctionType = AuctionType.Substring(0,1);
}
Set_Value ("AuctionType", AuctionType);
}
/** Get Auction Type.
@return Auction Type */
public String GetAuctionType() 
{
return (String)Get_Value("AuctionType");
}
/** Set Topic Type.
@param B_TopicType_ID Auction Topic Type */
public void SetB_TopicType_ID (int B_TopicType_ID)
{
if (B_TopicType_ID < 1) throw new ArgumentException ("B_TopicType_ID is mandatory.");
Set_ValueNoCheck ("B_TopicType_ID", B_TopicType_ID);
}
/** Get Topic Type.
@return Auction Topic Type */
public int GetB_TopicType_ID() 
{
Object ii = Get_Value("B_TopicType_ID");
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
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
}
/** Set Price List.
@param M_PriceList_ID Unique identifier of a Price List */
public void SetM_PriceList_ID (int M_PriceList_ID)
{
if (M_PriceList_ID < 1) throw new ArgumentException ("M_PriceList_ID is mandatory.");
Set_Value ("M_PriceList_ID", M_PriceList_ID);
}
/** Get Price List.
@return Unique identifier of a Price List */
public int GetM_PriceList_ID() 
{
Object ii = Get_Value("M_PriceList_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** M_ProductMember_ID AD_Reference_ID=162 */
public static int M_PRODUCTMEMBER_ID_AD_Reference_ID=162;
/** Set Membership.
@param M_ProductMember_ID Product used to deternine the price of the membership for the topic type */
public void SetM_ProductMember_ID (int M_ProductMember_ID)
{
if (M_ProductMember_ID < 1) throw new ArgumentException ("M_ProductMember_ID is mandatory.");
Set_Value ("M_ProductMember_ID", M_ProductMember_ID);
}
/** Get Membership.
@return Product used to deternine the price of the membership for the topic type */
public int GetM_ProductMember_ID() 
{
Object ii = Get_Value("M_ProductMember_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID < 1) throw new ArgumentException ("M_Product_ID is mandatory.");
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
}

}
