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
/** Generated Model for PA_GoalRestriction
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_PA_GoalRestriction : PO
{
public X_PA_GoalRestriction (Context ctx, int PA_GoalRestriction_ID, Trx trxName) : base (ctx, PA_GoalRestriction_ID, trxName)
{
/** if (PA_GoalRestriction_ID == 0)
{
SetGoalRestrictionType (null);
SetName (null);
SetPA_GoalRestriction_ID (0);
SetPA_Goal_ID (0);
}
 */
}
public X_PA_GoalRestriction (Ctx ctx, int PA_GoalRestriction_ID, Trx trxName) : base (ctx, PA_GoalRestriction_ID, trxName)
{
/** if (PA_GoalRestriction_ID == 0)
{
SetGoalRestrictionType (null);
SetName (null);
SetPA_GoalRestriction_ID (0);
SetPA_Goal_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_GoalRestriction (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_GoalRestriction (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_GoalRestriction (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_PA_GoalRestriction()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381857L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065068L;
/** AD_Table_ID=832 */
public static int Table_ID;
 // =832;

/** TableName=PA_GoalRestriction */
public static String Table_Name="PA_GoalRestriction";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(6);
/** AccessLevel
@return 6 - System - Client 
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
StringBuilder sb = new StringBuilder ("X_PA_GoalRestriction[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Business Partner Group.
@param C_BP_Group_ID Business Partner Group */
public void SetC_BP_Group_ID (int C_BP_Group_ID)
{
if (C_BP_Group_ID <= 0) Set_Value ("C_BP_Group_ID", null);
else
Set_Value ("C_BP_Group_ID", C_BP_Group_ID);
}
/** Get Business Partner Group.
@return Business Partner Group */
public int GetC_BP_Group_ID() 
{
Object ii = Get_Value("C_BP_Group_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID <= 0) Set_Value ("C_BPartner_ID", null);
else
Set_Value ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** GoalRestrictionType AD_Reference_ID=368 */
public static int GOALRESTRICTIONTYPE_AD_Reference_ID=368;
/** Business Partner = B */
public static String GOALRESTRICTIONTYPE_BusinessPartner = "B";
/** Product Category = C */
public static String GOALRESTRICTIONTYPE_ProductCategory = "C";
/** Bus.Partner Group = G */
public static String GOALRESTRICTIONTYPE_BusPartnerGroup = "G";
/** Organization = O */
public static String GOALRESTRICTIONTYPE_Organization = "O";
/** Product = P */
public static String GOALRESTRICTIONTYPE_Product = "P";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsGoalRestrictionTypeValid (String test)
{
return test.Equals("B") || test.Equals("C") || test.Equals("G") || test.Equals("O") || test.Equals("P");
}
/** Set Restriction Type.
@param GoalRestrictionType Goal Restriction Type */
public void SetGoalRestrictionType (String GoalRestrictionType)
{
if (GoalRestrictionType == null) throw new ArgumentException ("GoalRestrictionType is mandatory");
if (!IsGoalRestrictionTypeValid(GoalRestrictionType))
throw new ArgumentException ("GoalRestrictionType Invalid value - " + GoalRestrictionType + " - Reference_ID=368 - B - C - G - O - P");
if (GoalRestrictionType.Length > 1)
{
log.Warning("Length > 1 - truncated");
GoalRestrictionType = GoalRestrictionType.Substring(0,1);
}
Set_Value ("GoalRestrictionType", GoalRestrictionType);
}
/** Get Restriction Type.
@return Goal Restriction Type */
public String GetGoalRestrictionType() 
{
return (String)Get_Value("GoalRestrictionType");
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

/** Org_ID AD_Reference_ID=322 */
public static int ORG_ID_AD_Reference_ID=322;
/** Set Organization.
@param Org_ID Organizational entity within client */
public void SetOrg_ID (int Org_ID)
{
if (Org_ID <= 0) Set_Value ("Org_ID", null);
else
Set_Value ("Org_ID", Org_ID);
}
/** Get Organization.
@return Organizational entity within client */
public int GetOrg_ID() 
{
Object ii = Get_Value("Org_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Goal Restriction.
@param PA_GoalRestriction_ID Performance Goal Restriction */
public void SetPA_GoalRestriction_ID (int PA_GoalRestriction_ID)
{
if (PA_GoalRestriction_ID < 1) throw new ArgumentException ("PA_GoalRestriction_ID is mandatory.");
Set_ValueNoCheck ("PA_GoalRestriction_ID", PA_GoalRestriction_ID);
}
/** Get Goal Restriction.
@return Performance Goal Restriction */
public int GetPA_GoalRestriction_ID() 
{
Object ii = Get_Value("PA_GoalRestriction_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Goal.
@param PA_Goal_ID Performance Goal */
public void SetPA_Goal_ID (int PA_Goal_ID)
{
if (PA_Goal_ID < 1) throw new ArgumentException ("PA_Goal_ID is mandatory.");
Set_ValueNoCheck ("PA_Goal_ID", PA_Goal_ID);
}
/** Get Goal.
@return Performance Goal */
public int GetPA_Goal_ID() 
{
Object ii = Get_Value("PA_Goal_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
