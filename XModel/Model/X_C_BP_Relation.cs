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
/** Generated Model for C_BP_Relation
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_BP_Relation : PO
{
public X_C_BP_Relation (Context ctx, int C_BP_Relation_ID, Trx trxName) : base (ctx, C_BP_Relation_ID, trxName)
{
/** if (C_BP_Relation_ID == 0)
{
SetC_BP_Relation_ID (0);
SetC_BPartnerRelation_ID (0);
SetC_BPartnerRelation_Location_ID (0);
SetC_BPartner_ID (0);
SetIsBillTo (false);
SetIsPayFrom (false);
SetIsRemitTo (false);
SetIsShipTo (false);	// N
SetName (null);
}
 */
}
public X_C_BP_Relation (Ctx ctx, int C_BP_Relation_ID, Trx trxName) : base (ctx, C_BP_Relation_ID, trxName)
{
/** if (C_BP_Relation_ID == 0)
{
SetC_BP_Relation_ID (0);
SetC_BPartnerRelation_ID (0);
SetC_BPartnerRelation_Location_ID (0);
SetC_BPartner_ID (0);
SetIsBillTo (false);
SetIsPayFrom (false);
SetIsRemitTo (false);
SetIsShipTo (false);	// N
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_Relation (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_Relation (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_Relation (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_BP_Relation()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514370259L;
/** Last Updated Timestamp 7/29/2010 1:07:33 PM */
public static long updatedMS = 1280389053470L;
/** AD_Table_ID=678 */
public static int Table_ID;
 // =678;

/** TableName=C_BP_Relation */
public static String Table_Name="C_BP_Relation";

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
StringBuilder sb = new StringBuilder ("X_C_BP_Relation[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Partner Relation.
@param C_BP_Relation_ID Business Partner Relation */
public void SetC_BP_Relation_ID (int C_BP_Relation_ID)
{
if (C_BP_Relation_ID < 1) throw new ArgumentException ("C_BP_Relation_ID is mandatory.");
Set_ValueNoCheck ("C_BP_Relation_ID", C_BP_Relation_ID);
}
/** Get Partner Relation.
@return Business Partner Relation */
public int GetC_BP_Relation_ID() 
{
Object ii = Get_Value("C_BP_Relation_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_BPartnerRelation_ID AD_Reference_ID=138 */
public static int C_BPARTNERRELATION_ID_AD_Reference_ID=138;
/** Set Related Partner.
@param C_BPartnerRelation_ID Related Business Partner */
public void SetC_BPartnerRelation_ID (int C_BPartnerRelation_ID)
{
if (C_BPartnerRelation_ID < 1) throw new ArgumentException ("C_BPartnerRelation_ID is mandatory.");
Set_Value ("C_BPartnerRelation_ID", C_BPartnerRelation_ID);
}
/** Get Related Partner.
@return Related Business Partner */
public int GetC_BPartnerRelation_ID() 
{
Object ii = Get_Value("C_BPartnerRelation_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_BPartnerRelation_Location_ID AD_Reference_ID=159 */
public static int C_BPARTNERRELATION_LOCATION_ID_AD_Reference_ID=159;
/** Set Related Partner Location.
@param C_BPartnerRelation_Location_ID Location of the related Business Partner */
public void SetC_BPartnerRelation_Location_ID (int C_BPartnerRelation_Location_ID)
{
if (C_BPartnerRelation_Location_ID < 1) throw new ArgumentException ("C_BPartnerRelation_Location_ID is mandatory.");
Set_Value ("C_BPartnerRelation_Location_ID", C_BPartnerRelation_Location_ID);
}
/** Get Related Partner Location.
@return Location of the related Business Partner */
public int GetC_BPartnerRelation_Location_ID() 
{
Object ii = Get_Value("C_BPartnerRelation_Location_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
if (C_BPartner_Location_ID <= 0) Set_Value ("C_BPartner_Location_ID", null);
else
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
/** Set Invoice Address.
@param IsBillTo Business Partner Invoice/Bill Address */
public void SetIsBillTo (Boolean IsBillTo)
{
Set_Value ("IsBillTo", IsBillTo);
}
/** Get Invoice Address.
@return Business Partner Invoice/Bill Address */
public Boolean IsBillTo() 
{
Object oo = Get_Value("IsBillTo");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Pay-From Address.
@param IsPayFrom Business Partner pays from that address and we'll send dunning letters there */
public void SetIsPayFrom (Boolean IsPayFrom)
{
Set_Value ("IsPayFrom", IsPayFrom);
}
/** Get Pay-From Address.
@return Business Partner pays from that address and we'll send dunning letters there */
public Boolean IsPayFrom() 
{
Object oo = Get_Value("IsPayFrom");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Remit-To Address.
@param IsRemitTo Business Partner payment address */
public void SetIsRemitTo (Boolean IsRemitTo)
{
Set_Value ("IsRemitTo", IsRemitTo);
}
/** Get Remit-To Address.
@return Business Partner payment address */
public Boolean IsRemitTo() 
{
Object oo = Get_Value("IsRemitTo");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Ship Address.
@param IsShipTo Business Partner Shipment Address */
public void SetIsShipTo (Boolean IsShipTo)
{
Set_ValueNoCheck ("IsShipTo", IsShipTo);
}
/** Get Ship Address.
@return Business Partner Shipment Address */
public Boolean IsShipTo() 
{
Object oo = Get_Value("IsShipTo");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
