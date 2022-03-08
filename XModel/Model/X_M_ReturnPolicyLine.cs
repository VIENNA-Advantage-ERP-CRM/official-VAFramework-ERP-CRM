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
/** Generated Model for M_ReturnPolicyLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_ReturnPolicyLine : PO
{
public X_M_ReturnPolicyLine (Context ctx, int M_ReturnPolicyLine_ID, Trx trxName) : base (ctx, M_ReturnPolicyLine_ID, trxName)
{
/** if (M_ReturnPolicyLine_ID == 0)
{
SetM_ReturnPolicyLine_ID (0);
SetM_ReturnPolicy_ID (0);
}
 */
}
public X_M_ReturnPolicyLine (Ctx ctx, int M_ReturnPolicyLine_ID, Trx trxName) : base (ctx, M_ReturnPolicyLine_ID, trxName)
{
/** if (M_ReturnPolicyLine_ID == 0)
{
SetM_ReturnPolicyLine_ID (0);
SetM_ReturnPolicy_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ReturnPolicyLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ReturnPolicyLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ReturnPolicyLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_ReturnPolicyLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381277L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064488L;
/** AD_Table_ID=986 */
public static int Table_ID;
 // =986;

/** TableName=M_ReturnPolicyLine */
public static String Table_Name="M_ReturnPolicyLine";

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
StringBuilder sb = new StringBuilder ("X_M_ReturnPolicyLine[").Append(Get_ID()).Append("]");
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
@param M_Product_Category_ID Category of a Product */
public void SetM_Product_Category_ID (int M_Product_Category_ID)
{
if (M_Product_Category_ID <= 0) Set_Value ("M_Product_Category_ID", null);
else
Set_Value ("M_Product_Category_ID", M_Product_Category_ID);
}
/** Get Product Category.
@return Category of a Product */
public int GetM_Product_Category_ID() 
{
Object ii = Get_Value("M_Product_Category_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);
else
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
/** Set Return Policy Line.
@param M_ReturnPolicyLine_ID Exceptions to the return policy timeframe for specific products or product categories can be specified at the line level. */
public void SetM_ReturnPolicyLine_ID (int M_ReturnPolicyLine_ID)
{
if (M_ReturnPolicyLine_ID < 1) throw new ArgumentException ("M_ReturnPolicyLine_ID is mandatory.");
Set_ValueNoCheck ("M_ReturnPolicyLine_ID", M_ReturnPolicyLine_ID);
}
/** Get Return Policy Line.
@return Exceptions to the return policy timeframe for specific products or product categories can be specified at the line level. */
public int GetM_ReturnPolicyLine_ID() 
{
Object ii = Get_Value("M_ReturnPolicyLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Return Policy.
@param M_ReturnPolicy_ID The Return Policy dictates the timeframe within which goods can be returned. */
public void SetM_ReturnPolicy_ID (int M_ReturnPolicy_ID)
{
if (M_ReturnPolicy_ID < 1) throw new ArgumentException ("M_ReturnPolicy_ID is mandatory.");
Set_ValueNoCheck ("M_ReturnPolicy_ID", M_ReturnPolicy_ID);
}
/** Get Return Policy.
@return The Return Policy dictates the timeframe within which goods can be returned. */
public int GetM_ReturnPolicy_ID() 
{
Object ii = Get_Value("M_ReturnPolicy_ID");
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
