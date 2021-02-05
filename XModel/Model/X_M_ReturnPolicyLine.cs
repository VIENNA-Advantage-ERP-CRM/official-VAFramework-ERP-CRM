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
/** Generated Model for VAM_ReturnRuleLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_ReturnRuleLine : PO
{
public X_VAM_ReturnRuleLine (Context ctx, int VAM_ReturnRuleLine_ID, Trx trxName) : base (ctx, VAM_ReturnRuleLine_ID, trxName)
{
/** if (VAM_ReturnRuleLine_ID == 0)
{
SetVAM_ReturnRuleLine_ID (0);
SetVAM_ReturnRule_ID (0);
}
 */
}
public X_VAM_ReturnRuleLine (Ctx ctx, int VAM_ReturnRuleLine_ID, Trx trxName) : base (ctx, VAM_ReturnRuleLine_ID, trxName)
{
/** if (VAM_ReturnRuleLine_ID == 0)
{
SetVAM_ReturnRuleLine_ID (0);
SetVAM_ReturnRule_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ReturnRuleLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ReturnRuleLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ReturnRuleLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_ReturnRuleLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381277L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064488L;
/** VAF_TableView_ID=986 */
public static int Table_ID;
 // =986;

/** TableName=VAM_ReturnRuleLine */
public static String Table_Name="VAM_ReturnRuleLine";

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
StringBuilder sb = new StringBuilder ("X_VAM_ReturnRuleLine[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Return Policy Line.
@param VAM_ReturnRuleLine_ID Exceptions to the return policy timeframe for specific products or product categories can be specified at the line level. */
public void SetVAM_ReturnRuleLine_ID (int VAM_ReturnRuleLine_ID)
{
if (VAM_ReturnRuleLine_ID < 1) throw new ArgumentException ("VAM_ReturnRuleLine_ID is mandatory.");
Set_ValueNoCheck ("VAM_ReturnRuleLine_ID", VAM_ReturnRuleLine_ID);
}
/** Get Return Policy Line.
@return Exceptions to the return policy timeframe for specific products or product categories can be specified at the line level. */
public int GetVAM_ReturnRuleLine_ID() 
{
Object ii = Get_Value("VAM_ReturnRuleLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Return Policy.
@param VAM_ReturnRule_ID The Return Policy dictates the timeframe within which goods can be returned. */
public void SetVAM_ReturnRule_ID (int VAM_ReturnRule_ID)
{
if (VAM_ReturnRule_ID < 1) throw new ArgumentException ("VAM_ReturnRule_ID is mandatory.");
Set_ValueNoCheck ("VAM_ReturnRule_ID", VAM_ReturnRule_ID);
}
/** Get Return Policy.
@return The Return Policy dictates the timeframe within which goods can be returned. */
public int GetVAM_ReturnRule_ID() 
{
Object ii = Get_Value("VAM_ReturnRule_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set TimeFrame (in days).
@param TimeFrame The timeframe dictates the number of days after shipment that the goods can be returned. */
public void SetTimeFrame (int TimeFrame)
{
Set_Value ("TimeFrame", TimeFrame);
}
/** Get TimeFrame (in days).
@return The timeframe dictates the number of days after shipment that the goods can be returned. */
public int GetTimeFrame() 
{
Object ii = Get_Value("TimeFrame");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
