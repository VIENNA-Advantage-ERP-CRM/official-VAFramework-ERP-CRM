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
/** Generated Model for C_CampaignTargetList
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_CampaignTargetList : PO
{
public X_C_CampaignTargetList (Context ctx, int C_CampaignTargetList_ID, Trx trxName) : base (ctx, C_CampaignTargetList_ID, trxName)
{
/** if (C_CampaignTargetList_ID == 0)
{
SetC_CampaignTargetList_ID (0);
SetC_Campaign_ID (0);
}
 */
}
public X_C_CampaignTargetList (Ctx ctx, int C_CampaignTargetList_ID, Trx trxName) : base (ctx, C_CampaignTargetList_ID, trxName)
{
/** if (C_CampaignTargetList_ID == 0)
{
SetC_CampaignTargetList_ID (0);
SetC_Campaign_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CampaignTargetList (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CampaignTargetList (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CampaignTargetList (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_CampaignTargetList()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27609521540537L;
/** Last Updated Timestamp 1/24/2012 2:40:23 PM */
public static long updatedMS = 1327396223748L;
/** AD_Table_ID=1000251 */
public static int Table_ID;
 // =1000251;

/** TableName=C_CampaignTargetList */
public static String Table_Name="C_CampaignTargetList";

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
StringBuilder sb = new StringBuilder ("X_C_CampaignTargetList[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set C_CampaignTargetList_ID.
@param C_CampaignTargetList_ID C_CampaignTargetList_ID */
public void SetC_CampaignTargetList_ID (int C_CampaignTargetList_ID)
{
if (C_CampaignTargetList_ID < 1) throw new ArgumentException ("C_CampaignTargetList_ID is mandatory.");
Set_ValueNoCheck ("C_CampaignTargetList_ID", C_CampaignTargetList_ID);
}
/** Get C_CampaignTargetList_ID.
@return C_CampaignTargetList_ID */
public int GetC_CampaignTargetList_ID() 
{
Object ii = Get_Value("C_CampaignTargetList_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Campaign.
@param C_Campaign_ID Marketing Campaign */
public void SetC_Campaign_ID (int C_Campaign_ID)
{
if (C_Campaign_ID < 1) throw new ArgumentException ("C_Campaign_ID is mandatory.");
Set_ValueNoCheck ("C_Campaign_ID", C_Campaign_ID);
}
/** Get Campaign.
@return Marketing Campaign */
public int GetC_Campaign_ID() 
{
Object ii = Get_Value("C_Campaign_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set C_MasterTargetList_ID.
@param C_MasterTargetList_ID C_MasterTargetList_ID */
public void SetC_MasterTargetList_ID (int C_MasterTargetList_ID)
{
if (C_MasterTargetList_ID <= 0) Set_Value ("C_MasterTargetList_ID", null);
else
Set_Value ("C_MasterTargetList_ID", C_MasterTargetList_ID);
}
/** Get C_MasterTargetList_ID.
@return C_MasterTargetList_ID */
public int GetC_MasterTargetList_ID() 
{
Object ii = Get_Value("C_MasterTargetList_ID");
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
@param R_InterestArea_ID Interest Area or Topic */
public void SetR_InterestArea_ID (int R_InterestArea_ID)
{
if (R_InterestArea_ID <= 0) Set_Value ("R_InterestArea_ID", null);
else
Set_Value ("R_InterestArea_ID", R_InterestArea_ID);
}
/** Get Interest Area.
@return Interest Area or Topic */
public int GetR_InterestArea_ID() 
{
Object ii = Get_Value("R_InterestArea_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
