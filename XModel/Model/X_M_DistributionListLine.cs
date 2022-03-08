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
/** Generated Model for M_DistributionListLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_DistributionListLine : PO
{
public X_M_DistributionListLine (Context ctx, int M_DistributionListLine_ID, Trx trxName) : base (ctx, M_DistributionListLine_ID, trxName)
{
/** if (M_DistributionListLine_ID == 0)
{
SetC_BPartner_ID (0);
SetC_BPartner_Location_ID (0);
SetM_DistributionListLine_ID (0);
SetM_DistributionList_ID (0);
SetMinQty (0.0);
}
 */
}
public X_M_DistributionListLine (Ctx ctx, int M_DistributionListLine_ID, Trx trxName) : base (ctx, M_DistributionListLine_ID, trxName)
{
/** if (M_DistributionListLine_ID == 0)
{
SetC_BPartner_ID (0);
SetC_BPartner_Location_ID (0);
SetM_DistributionListLine_ID (0);
SetM_DistributionList_ID (0);
SetMinQty (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DistributionListLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DistributionListLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DistributionListLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_DistributionListLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379256L;
/** Last Updated Timestamp 7/29/2010 1:07:42 PM */
public static long updatedMS = 1280389062467L;
/** AD_Table_ID=665 */
public static int Table_ID;
 // =665;

/** TableName=M_DistributionListLine */
public static String Table_Name="M_DistributionListLine";

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
StringBuilder sb = new StringBuilder ("X_M_DistributionListLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID < 1) throw new ArgumentException ("C_BPartner_ID is mandatory.");
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
/** Set Partner Location.
@param C_BPartner_Location_ID Identifies the (ship to) address for this Business Partner */
public void SetC_BPartner_Location_ID (int C_BPartner_Location_ID)
{
if (C_BPartner_Location_ID < 1) throw new ArgumentException ("C_BPartner_Location_ID is mandatory.");
Set_Value ("C_BPartner_Location_ID", C_BPartner_Location_ID);
}
/** Get Partner Location.
@return Identifies the (ship to) address for this Business Partner */
public int GetC_BPartner_Location_ID() 
{
Object ii = Get_Value("C_BPartner_Location_ID");
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
/** Set Distribution List Line.
@param M_DistributionListLine_ID Distribution List Line with Business Partner and Quantity/Percentage */
public void SetM_DistributionListLine_ID (int M_DistributionListLine_ID)
{
if (M_DistributionListLine_ID < 1) throw new ArgumentException ("M_DistributionListLine_ID is mandatory.");
Set_ValueNoCheck ("M_DistributionListLine_ID", M_DistributionListLine_ID);
}
/** Get Distribution List Line.
@return Distribution List Line with Business Partner and Quantity/Percentage */
public int GetM_DistributionListLine_ID() 
{
Object ii = Get_Value("M_DistributionListLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Distribution List.
@param M_DistributionList_ID Distribution Lists allow to distribute products to a selected list of partners */
public void SetM_DistributionList_ID (int M_DistributionList_ID)
{
if (M_DistributionList_ID < 1) throw new ArgumentException ("M_DistributionList_ID is mandatory.");
Set_ValueNoCheck ("M_DistributionList_ID", M_DistributionList_ID);
}
/** Get Distribution List.
@return Distribution Lists allow to distribute products to a selected list of partners */
public int GetM_DistributionList_ID() 
{
Object ii = Get_Value("M_DistributionList_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetM_DistributionList_ID().ToString());
}
/** Set Minimum Quantity.
@param MinQty Minimum quantity for the business partner */
public void SetMinQty (Decimal? MinQty)
{
if (MinQty == null) throw new ArgumentException ("MinQty is mandatory.");
Set_Value ("MinQty", (Decimal?)MinQty);
}
/** Get Minimum Quantity.
@return Minimum quantity for the business partner */
public Decimal GetMinQty() 
{
Object bd =Get_Value("MinQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Ratio.
@param Ratio Relative Ratio for Distributions */
public void SetRatio (Decimal? Ratio)
{
Set_Value ("Ratio", (Decimal?)Ratio);
}
/** Get Ratio.
@return Relative Ratio for Distributions */
public Decimal GetRatio() 
{
Object bd =Get_Value("Ratio");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
