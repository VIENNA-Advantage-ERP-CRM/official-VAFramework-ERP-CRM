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
/** Generated Model for VAB_PromotionTargetList
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_PromotionTargetList : PO
{
public X_VAB_PromotionTargetList (Context ctx, int VAB_PromotionTargetList_ID, Trx trxName) : base (ctx, VAB_PromotionTargetList_ID, trxName)
{
/** if (VAB_PromotionTargetList_ID == 0)
{
SetVAB_PromotionTargetList_ID (0);
SetVAB_Promotion_ID (0);
}
 */
}
public X_VAB_PromotionTargetList (Ctx ctx, int VAB_PromotionTargetList_ID, Trx trxName) : base (ctx, VAB_PromotionTargetList_ID, trxName)
{
/** if (VAB_PromotionTargetList_ID == 0)
{
SetVAB_PromotionTargetList_ID (0);
SetVAB_Promotion_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_PromotionTargetList (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_PromotionTargetList (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_PromotionTargetList (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_PromotionTargetList()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27609521540537L;
/** Last Updated Timestamp 1/24/2012 2:40:23 PM */
public static long updatedMS = 1327396223748L;
/** VAF_TableView_ID=1000251 */
public static int Table_ID;
 // =1000251;

/** TableName=VAB_PromotionTargetList */
public static String Table_Name="VAB_PromotionTargetList";

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
StringBuilder sb = new StringBuilder ("X_VAB_PromotionTargetList[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set VAB_PromotionTargetList_ID.
@param VAB_PromotionTargetList_ID VAB_PromotionTargetList_ID */
public void SetVAB_PromotionTargetList_ID (int VAB_PromotionTargetList_ID)
{
if (VAB_PromotionTargetList_ID < 1) throw new ArgumentException ("VAB_PromotionTargetList_ID is mandatory.");
Set_ValueNoCheck ("VAB_PromotionTargetList_ID", VAB_PromotionTargetList_ID);
}
/** Get VAB_PromotionTargetList_ID.
@return VAB_PromotionTargetList_ID */
public int GetVAB_PromotionTargetList_ID() 
{
Object ii = Get_Value("VAB_PromotionTargetList_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Campaign.
@param VAB_Promotion_ID Marketing Campaign */
public void SetVAB_Promotion_ID (int VAB_Promotion_ID)
{
if (VAB_Promotion_ID < 1) throw new ArgumentException ("VAB_Promotion_ID is mandatory.");
Set_ValueNoCheck ("VAB_Promotion_ID", VAB_Promotion_ID);
}
/** Get Campaign.
@return Marketing Campaign */
public int GetVAB_Promotion_ID() 
{
Object ii = Get_Value("VAB_Promotion_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAB_MasterTargetList_ID.
@param VAB_MasterTargetList_ID VAB_MasterTargetList_ID */
public void SetVAB_MasterTargetList_ID (int VAB_MasterTargetList_ID)
{
if (VAB_MasterTargetList_ID <= 0) Set_Value ("VAB_MasterTargetList_ID", null);
else
Set_Value ("VAB_MasterTargetList_ID", VAB_MasterTargetList_ID);
}
/** Get VAB_MasterTargetList_ID.
@return VAB_MasterTargetList_ID */
public int GetVAB_MasterTargetList_ID() 
{
Object ii = Get_Value("VAB_MasterTargetList_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Interest Area.
@param VAR_InterestArea_ID Interest Area or Topic */
public void SetR_InterestArea_ID (int VAR_InterestArea_ID)
{
if (VAR_InterestArea_ID <= 0) Set_Value ("VAR_InterestArea_ID", null);
else
Set_Value ("VAR_InterestArea_ID", VAR_InterestArea_ID);
}
/** Get Interest Area.
@return Interest Area or Topic */
public int GetR_InterestArea_ID() 
{
Object ii = Get_Value("VAR_InterestArea_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
