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
/** Generated Model for TIRE_Storage
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_TIRE_Storage : PO
{
public X_TIRE_Storage (Context ctx, int TIRE_Storage_ID, Trx trxName) : base (ctx, TIRE_Storage_ID, trxName)
{
/** if (TIRE_Storage_ID == 0)
{
SetDateReceived (DateTime.Now);	// @#Date@
SetIsReturned (false);
SetIsStored (false);
SetName (null);
SetTIRE_Storage_ID (0);
}
 */
}
public X_TIRE_Storage (Ctx ctx, int TIRE_Storage_ID, Trx trxName) : base (ctx, TIRE_Storage_ID, trxName)
{
/** if (TIRE_Storage_ID == 0)
{
SetDateReceived (DateTime.Now);	// @#Date@
SetIsReturned (false);
SetIsStored (false);
SetName (null);
SetTIRE_Storage_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_TIRE_Storage (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_TIRE_Storage (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_TIRE_Storage (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_TIRE_Storage()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384083L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067294L;
/** AD_Table_ID=384 */
public static int Table_ID;
 // =384;

/** TableName=TIRE_Storage */
public static String Table_Name="TIRE_Storage";

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
StringBuilder sb = new StringBuilder ("X_TIRE_Storage[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);
else
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
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
/** Set Date received.
@param DateReceived Date a product was received */
public void SetDateReceived (DateTime? DateReceived)
{
if (DateReceived == null) throw new ArgumentException ("DateReceived is mandatory.");
Set_Value ("DateReceived", (DateTime?)DateReceived);
}
/** Get Date received.
@return Date a product was received */
public DateTime? GetDateReceived() 
{
return (DateTime?)Get_Value("DateReceived");
}
/** Set Date returned.
@param DateReturned Date a product was returned */
public void SetDateReturned (DateTime? DateReturned)
{
Set_Value ("DateReturned", (DateTime?)DateReturned);
}
/** Get Date returned.
@return Date a product was returned */
public DateTime? GetDateReturned() 
{
return (DateTime?)Get_Value("DateReturned");
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
/** Set Returned.
@param IsReturned Returned */
public void SetIsReturned (Boolean IsReturned)
{
Set_Value ("IsReturned", IsReturned);
}
/** Get Returned.
@return Returned */
public Boolean IsReturned() 
{
Object oo = Get_Value("IsReturned");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Moved to storage.
@param IsStored Moved to storage */
public void SetIsStored (Boolean IsStored)
{
Set_Value ("IsStored", IsStored);
}
/** Get Moved to storage.
@return Moved to storage */
public Boolean IsStored() 
{
Object oo = Get_Value("IsStored");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Locator.
@param M_Locator_ID Warehouse Locator */
public void SetM_Locator_ID (int M_Locator_ID)
{
if (M_Locator_ID <= 0) Set_Value ("M_Locator_ID", null);
else
Set_Value ("M_Locator_ID", M_Locator_ID);
}
/** Get Locator.
@return Warehouse Locator */
public int GetM_Locator_ID() 
{
Object ii = Get_Value("M_Locator_ID");
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
/** Set Registration.
@param Registration Vehicle registration */
public void SetRegistration (String Registration)
{
if (Registration != null && Registration.Length > 20)
{
log.Warning("Length > 20 - truncated");
Registration = Registration.Substring(0,20);
}
Set_Value ("Registration", Registration);
}
/** Get Registration.
@return Vehicle registration */
public String GetRegistration() 
{
return (String)Get_Value("Registration");
}
/** Set Remark.
@param Remark Remark */
public void SetRemark (String Remark)
{
if (Remark != null && Remark.Length > 60)
{
log.Warning("Length > 60 - truncated");
Remark = Remark.Substring(0,60);
}
Set_Value ("Remark", Remark);
}
/** Get Remark.
@return Remark */
public String GetRemark() 
{
return (String)Get_Value("Remark");
}
/** Set Rim.
@param Rim Stored rim */
public void SetRim (String Rim)
{
if (Rim != null && Rim.Length > 20)
{
log.Warning("Length > 20 - truncated");
Rim = Rim.Substring(0,20);
}
Set_Value ("Rim", Rim);
}
/** Get Rim.
@return Stored rim */
public String GetRim() 
{
return (String)Get_Value("Rim");
}
/** Set Rim Back.
@param Rim_B Rim Back */
public void SetRim_B (String Rim_B)
{
if (Rim_B != null && Rim_B.Length > 20)
{
log.Warning("Length > 20 - truncated");
Rim_B = Rim_B.Substring(0,20);
}
Set_Value ("Rim_B", Rim_B);
}
/** Get Rim Back.
@return Rim Back */
public String GetRim_B() 
{
return (String)Get_Value("Rim_B");
}
/** Set Tire Storage.
@param TIRE_Storage_ID Tire Storage */
public void SetTIRE_Storage_ID (int TIRE_Storage_ID)
{
if (TIRE_Storage_ID < 1) throw new ArgumentException ("TIRE_Storage_ID is mandatory.");
Set_ValueNoCheck ("TIRE_Storage_ID", TIRE_Storage_ID);
}
/** Get Tire Storage.
@return Tire Storage */
public int GetTIRE_Storage_ID() 
{
Object ii = Get_Value("TIRE_Storage_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tire Quality.
@param TireQuality Tire Quality */
public void SetTireQuality (String TireQuality)
{
if (TireQuality != null && TireQuality.Length > 20)
{
log.Warning("Length > 20 - truncated");
TireQuality = TireQuality.Substring(0,20);
}
Set_Value ("TireQuality", TireQuality);
}
/** Get Tire Quality.
@return Tire Quality */
public String GetTireQuality() 
{
return (String)Get_Value("TireQuality");
}
/** Set Tire Quality Back.
@param TireQuality_B Tire Quality Back */
public void SetTireQuality_B (String TireQuality_B)
{
if (TireQuality_B != null && TireQuality_B.Length > 20)
{
log.Warning("Length > 20 - truncated");
TireQuality_B = TireQuality_B.Substring(0,20);
}
Set_Value ("TireQuality_B", TireQuality_B);
}
/** Get Tire Quality Back.
@return Tire Quality Back */
public String GetTireQuality_B() 
{
return (String)Get_Value("TireQuality_B");
}
/** Set Tire size (L/R).
@param TireSize Tire size (L/R) */
public void SetTireSize (String TireSize)
{
if (TireSize != null && TireSize.Length > 20)
{
log.Warning("Length > 20 - truncated");
TireSize = TireSize.Substring(0,20);
}
Set_Value ("TireSize", TireSize);
}
/** Get Tire size (L/R).
@return Tire size (L/R) */
public String GetTireSize() 
{
return (String)Get_Value("TireSize");
}
/** Set Tire size Back.
@param TireSize_B Tire size Back */
public void SetTireSize_B (String TireSize_B)
{
if (TireSize_B != null && TireSize_B.Length > 20)
{
log.Warning("Length > 20 - truncated");
TireSize_B = TireSize_B.Substring(0,20);
}
Set_Value ("TireSize_B", TireSize_B);
}
/** Get Tire size Back.
@return Tire size Back */
public String GetTireSize_B() 
{
return (String)Get_Value("TireSize_B");
}
/** Set Tire type.
@param TireType Tire type */
public void SetTireType (String TireType)
{
if (TireType != null && TireType.Length > 20)
{
log.Warning("Length > 20 - truncated");
TireType = TireType.Substring(0,20);
}
Set_Value ("TireType", TireType);
}
/** Get Tire type.
@return Tire type */
public String GetTireType() 
{
return (String)Get_Value("TireType");
}
/** Set Tire type Back.
@param TireType_B Tire type Back */
public void SetTireType_B (String TireType_B)
{
if (TireType_B != null && TireType_B.Length > 20)
{
log.Warning("Length > 20 - truncated");
TireType_B = TireType_B.Substring(0,20);
}
Set_Value ("TireType_B", TireType_B);
}
/** Get Tire type Back.
@return Tire type Back */
public String GetTireType_B() 
{
return (String)Get_Value("TireType_B");
}
/** Set Vehicle.
@param Vehicle Vehicle */
public void SetVehicle (String Vehicle)
{
if (Vehicle != null && Vehicle.Length > 20)
{
log.Warning("Length > 20 - truncated");
Vehicle = Vehicle.Substring(0,20);
}
Set_Value ("Vehicle", Vehicle);
}
/** Get Vehicle.
@return Vehicle */
public String GetVehicle() 
{
return (String)Get_Value("Vehicle");
}
}

}
